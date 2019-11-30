// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AmbientSoundWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class AmbientSoundWorker : IThreadWorkItem
  {
    public static ushort[] LoopDelays = new ushort[10]
    {
      (ushort) 0,
      (ushort) 1,
      (ushort) 2,
      (ushort) 5,
      (ushort) 10,
      (ushort) 20,
      (ushort) 30,
      (ushort) 60,
      (ushort) 120,
      (ushort) 300
    };
    public const int MaxCurrentlyPlayingCount = 5;
    private GameInstance instance;
    private PriorityLevel priority;
    private long lastTimeStamp;
    private List<AmbientSoundWorker.SoundInstance> sounds;
    private List<int> soundsToDelete;

    public string Name
    {
      get
      {
        return nameof (AmbientSoundWorker);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public List<AmbientSoundWorker.SoundInstance> Sounds
    {
      get
      {
        return this.sounds;
      }
    }

    private bool IsPowered(AmbientSoundWorker.SoundInstance sound)
    {
      if (sound.Block != null)
        return this.instance.MapStrategyTM.IsBlockReceivingPower(sound.Block.Point);
      return false;
    }

    public AmbientSoundWorker(GameInstance instance, PriorityLevel priority)
    {
      this.instance = instance;
      this.priority = priority;
      this.sounds = new List<AmbientSoundWorker.SoundInstance>();
      this.soundsToDelete = new List<int>();
    }

    public void UnloadContent()
    {
      lock (this.sounds)
      {
        for (int index = 0; index < this.sounds.Count; ++index)
        {
          AmbientSoundWorker.SoundInstance sound = this.sounds[index];
          if (sound.Cue != null)
          {
            sound.Cue.Dispose();
            sound.Cue = (Cue) null;
          }
        }
        this.sounds.Clear();
      }
      this.instance = (GameInstance) null;
    }

    public void Update()
    {
      try
      {
        this.UpdateCore();
      }
      finally
      {
        this.lastTimeStamp = Globals1.ElapsedWatch.ElapsedMilliseconds;
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }

    private void UpdateCore()
    {
      int num = 0;
      float elapsed = (float) (Globals1.ElapsedWatch.ElapsedMilliseconds - this.lastTimeStamp) / 1000f;
      lock (this.sounds)
      {
        for (int index = 0; index < this.sounds.Count; ++index)
        {
          AmbientSoundWorker.SoundInstance sound = this.sounds[index];
          bool flag = num < 5;
          if (sound.Block != null)
            sound.Block.DisplayNotPlayingMessage = !flag;
          if (flag && sound.CueName != null && this.UpdateSound(sound, elapsed))
            ++num;
        }
        foreach (int index in this.soundsToDelete)
        {
          if (index >= 0 && index < this.sounds.Count)
          {
            if (this.sounds[index].Cue != null)
              this.sounds[index].Cue.Dispose();
            this.sounds.RemoveAt(index);
          }
        }
        this.soundsToDelete.Clear();
      }
    }

    private bool UpdateSound(AmbientSoundWorker.SoundInstance sound, float elapsed)
    {
      Player player = (Player) null;
      SunMoon sunMoon = this.instance.SunMoon;
      if (sound.DayOrNight == DayOrNight.None || sound.DayOrNight == DayOrNight.Day && sunMoon.IsDayTime || sound.DayOrNight == DayOrNight.Night && sunMoon.IsNightTime)
      {
        float num1 = float.MaxValue;
        float num2 = sound.Range * sound.Range;
        float num3 = sound.Block != null ? 10000f : 1048576f;
        foreach (Player localEnabledPlayer in NetworkManager.Instance.LocalEnabledPlayers)
        {
          if (!sound.RequiresPower || this.IsPowered(sound))
          {
            float num4 = Vector3.DistanceSquared(localEnabledPlayer.EyePosition, sound.Emitter.Position);
            if ((double) num4 <= (double) num2 && (double) num4 < (double) num1 && (double) num4 < (double) num3 && (!sound.Bound.HasValue || sound.Bound.Value.Contains(localEnabledPlayer.EyePosition) == ContainmentType.Contains))
            {
              num1 = num4;
              player = localEnabledPlayer;
            }
          }
        }
        if (player != null)
        {
          bool flag = true;
          if (sound.Cue == null || sound.Cue.IsStopped)
          {
            if (sound.Cue != null)
            {
              sound.Cue.Dispose();
              sound.Cue = (Cue) null;
            }
            if ((double) sound.LoopDelay > 0.0)
            {
              sound.LoopTimer -= elapsed;
              if ((double) sound.LoopTimer < 0.0)
                sound.LoopTimer = sound.LoopDelay;
              else
                flag = false;
            }
            if (flag && sound.LoopCount < 1)
            {
              flag = false;
              this.soundsToDelete.Add(this.GetSoundIndex(sound));
            }
            if (flag)
            {
              CoreGlobals.AudioManager.PlaySound(sound.CueName, out sound.Cue);
              --sound.LoopCount;
            }
          }
          if (flag && sound.Cue != null && (!sound.Cue.IsDisposed && !sound.Cue.IsStopping))
          {
            sound.Cue.SetVariable("AmbientDistance", num1 / num2);
            sound.Cue.SetVariable("Volume", sound.Volume);
          }
        }
      }
      if (player != null)
        return true;
      if (sound.Cue != null)
      {
        sound.Cue.Dispose();
        sound.Cue = (Cue) null;
      }
      return false;
    }

    public void AddSound(
      string cueName,
      GlobalPoint3D p,
      float volume,
      float range,
      int loopCount,
      float loopDelay)
    {
      if (cueName == null)
        return;
      lock (this.sounds)
      {
        int soundIndex = this.GetSoundIndex(cueName, p);
        AmbientSoundWorker.SoundInstance soundInstance = soundIndex >= 0 ? this.sounds[soundIndex] : new AmbientSoundWorker.SoundInstance();
        soundInstance.Range = range;
        soundInstance.Volume = volume;
        soundInstance.LoopCount = loopCount == 0 ? int.MaxValue : loopCount;
        soundInstance.LoopDelay = loopDelay;
        if ((double) soundInstance.LoopTimer > (double) loopDelay)
          soundInstance.LoopTimer = loopDelay;
        if (soundIndex >= 0)
          return;
        soundInstance.CueName = cueName;
        soundInstance.Emitter = new AudioEmitter()
        {
          Position = this.instance.Map.GetBlockCenter(p)
        };
        this.sounds.Add(soundInstance);
      }
    }

    public void AddSound(
      string cueName,
      GlobalPoint3D min,
      GlobalPoint3D max,
      float volume,
      int loopCount,
      float loopDelay)
    {
      if (cueName == null)
        return;
      lock (this.sounds)
      {
        int soundIndex = this.GetSoundIndex(cueName, min, max);
        AmbientSoundWorker.SoundInstance soundInstance = soundIndex >= 0 ? this.sounds[soundIndex] : new AmbientSoundWorker.SoundInstance();
        soundInstance.Volume = volume;
        soundInstance.LoopCount = loopCount == 0 ? int.MaxValue : loopCount;
        soundInstance.LoopDelay = loopDelay;
        if ((double) soundInstance.LoopTimer > (double) loopDelay)
          soundInstance.LoopTimer = loopDelay;
        if (soundIndex >= 0)
          return;
        soundInstance.CueName = cueName;
        soundInstance.Bound = new BoundingBox?(this.GetBox(min, max));
        soundInstance.Range = Vector3.Distance(soundInstance.Bound.Value.Min, soundInstance.Bound.Value.Max) * 0.5f;
        soundInstance.Emitter = new AudioEmitter()
        {
          Position = (soundInstance.Bound.Value.Max - soundInstance.Bound.Value.Min) * 0.5f + soundInstance.Bound.Value.Min
        };
        this.sounds.Add(soundInstance);
      }
    }

    private BoundingBox GetBox(GlobalPoint3D min, GlobalPoint3D max)
    {
      GlobalPoint3D point1 = GlobalPoint3D.Min(min, max);
      GlobalPoint3D point2 = GlobalPoint3D.Max(min, max);
      Vector3 position1 = this.instance.Map.GetPosition(point1);
      --position1.Y;
      Vector3 position2 = this.instance.Map.GetPosition(point2);
      ++position2.X;
      ++position2.Z;
      return new BoundingBox(position1, position2);
    }

    public void SetBlock(AmbientSoundBlock block)
    {
      if (block == null)
        return;
      lock (this.sounds)
      {
        int blockIndex = this.GetBlockIndex(block);
        AmbientSoundWorker.SoundInstance soundInstance = blockIndex >= 0 ? this.sounds[blockIndex] : new AmbientSoundWorker.SoundInstance();
        string str = block.SoundID > 0 ? Globals1.AmbientSoundData[block.SoundID].CueName : (string) null;
        if (str != soundInstance.CueName && soundInstance.Cue != null)
        {
          soundInstance.Cue.Dispose();
          soundInstance.Cue = (Cue) null;
        }
        soundInstance.CueName = str;
        soundInstance.DayOrNight = block.DayOrNight;
        soundInstance.LoopCount = int.MaxValue;
        soundInstance.LoopDelay = (float) AmbientSoundWorker.LoopDelays[(int) block.LoopDelayIndex];
        if ((double) soundInstance.LoopTimer > (double) soundInstance.LoopDelay)
          soundInstance.LoopTimer = soundInstance.LoopDelay;
        soundInstance.Range = (float) block.Distance;
        soundInstance.RequiresPower = block.RequiresPower;
        soundInstance.Volume = block.Volume;
        if (blockIndex >= 0)
          return;
        soundInstance.Block = block;
        soundInstance.Emitter = new AudioEmitter()
        {
          Position = this.instance.Map.GetBlockCenter(block.Point)
        };
        this.sounds.Add(soundInstance);
      }
    }

    private int GetSoundIndex(string cueName, GlobalPoint3D p)
    {
      Vector3 blockCenter = this.instance.Map.GetBlockCenter(p);
      for (int index = 0; index < this.sounds.Count; ++index)
      {
        if (this.sounds[index].CueName == cueName && this.sounds[index].Emitter.Position == blockCenter)
          return index;
      }
      return -1;
    }

    private int GetSoundIndex(string cueName, GlobalPoint3D min, GlobalPoint3D max)
    {
      BoundingBox box = this.GetBox(min, max);
      for (int index = 0; index < this.sounds.Count; ++index)
      {
        AmbientSoundWorker.SoundInstance sound = this.sounds[index];
        if (sound.CueName == cueName && sound.Bound.HasValue && (sound.Bound.Value.Min == box.Min && sound.Bound.Value.Max == box.Max))
          return index;
      }
      return -1;
    }

    private int GetSoundIndex(AmbientSoundWorker.SoundInstance sound)
    {
      for (int index = 0; index < this.sounds.Count; ++index)
      {
        if (this.sounds[index] == sound)
          return index;
      }
      return -1;
    }

    private int GetBlockIndex(AmbientSoundBlock block)
    {
      for (int index = 0; index < this.sounds.Count; ++index)
      {
        if (this.sounds[index].Block == block)
          return index;
      }
      return -1;
    }

    public void RemoveSound(string cueName, GlobalPoint3D p)
    {
      lock (this.sounds)
      {
        int soundIndex = this.GetSoundIndex(cueName, p);
        if (soundIndex < 0)
          return;
        if (this.sounds[soundIndex].Cue != null)
          this.sounds[soundIndex].Cue.Dispose();
        this.sounds.RemoveAt(soundIndex);
      }
    }

    public void RemoveSound(string cueName, GlobalPoint3D min, GlobalPoint3D max)
    {
      lock (this.sounds)
      {
        int soundIndex = this.GetSoundIndex(cueName, min, max);
        if (soundIndex < 0)
          return;
        if (this.sounds[soundIndex].Cue != null)
          this.sounds[soundIndex].Cue.Dispose();
        this.sounds.RemoveAt(soundIndex);
      }
    }

    public void RemoveBlock(AmbientSoundBlock block)
    {
      if (block == null)
        return;
      lock (this.sounds)
      {
        for (int index = this.sounds.Count - 1; index >= 0; --index)
        {
          AmbientSoundWorker.SoundInstance sound = this.sounds[index];
          if (sound.Block == block)
          {
            if (sound.Cue != null)
              sound.Cue.Dispose();
            this.sounds.RemoveAt(index);
            break;
          }
        }
      }
    }

    public class SoundInstance
    {
      public Cue Cue;
      public string CueName;
      public AudioEmitter Emitter;
      public BoundingBox? Bound;
      public float Volume;
      public float Range;
      public float LoopDelay;
      public int LoopCount;
      public DayOrNight DayOrNight;
      public bool RequiresPower;
      public float LoopTimer;
      public AmbientSoundBlock Block;
    }
  }
}
