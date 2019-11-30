// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ParticleEmitterWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ParticleEmitterWorker : IThreadWorkItem
  {
    private GameInstance instance;
    private long lastTime;
    private PriorityLevel priority;
    private CustomArray<ProceduralEmitter> proceduralEmitters;

    public string Name
    {
      get
      {
        return nameof (ParticleEmitterWorker);
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

    public ParticleEmitterWorker(GameInstance instance, PriorityLevel priority)
    {
      this.instance = instance;
      this.priority = priority;
      this.proceduralEmitters = new CustomArray<ProceduralEmitter>();
    }

    public void Update()
    {
      try
      {
        long elapsedMilliseconds = Globals1.ElapsedWatch.ElapsedMilliseconds;
        int elapsed = (int) (elapsedMilliseconds - this.lastTime);
        this.lastTime = elapsedMilliseconds;
        if (!this.instance.IsMapActive)
          return;
        MapTM map = this.instance.Map;
        this.RemoveExpiredProceduralEmitters(elapsed);
        this.UpdateEmitters(map, this.proceduralEmitters, elapsed);
        this.UpdateEmitters(map, map.MapStrategyTM.ParticleEmitterBlocks, elapsed);
      }
      finally
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }

    private void UpdateEmitters(MapTM map, List<ParticleEmitterBlock> emitters, int elapsed)
    {
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      lock (emitters)
      {
        for (int index = 0; index < emitters.Count; ++index)
        {
          ParticleEmitterBlock emitter = emitters[index];
          emitter.EmitCounter -= elapsed;
          if (emitter.EmitCounter <= 0)
          {
            emitter.EmitCounter = emitter.Data.EmitFreq;
            Vector3 blockCenter = map.GetBlockCenter(emitter.Point);
            if (((double) emitter.Data.Proximity <= 0.0 || this.instance.IsAnyLocalPlayerInProximity(blockCenter, emitter.Data.Proximity, true)) && (!emitter.RequiresPower || mapStrategyTm.IsBlockReceivingPower(emitter.Point)) && !this.EmitParticle(map, blockCenter, ref emitter.Data))
              break;
          }
        }
      }
    }

    private int SortEmittersClosestToPlayers(ParticleEmitterBlock b1, ParticleEmitterBlock b2)
    {
      float num1 = float.MaxValue;
      float num2 = float.MaxValue;
      Vector3 position1 = this.instance.Map.GetPosition(b1.Point);
      Vector3 position2 = this.instance.Map.GetPosition(b2.Point);
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
      {
        float num3 = Vector3.DistanceSquared(position1, localEnabledPlayer.Position);
        if ((double) num3 < (double) num1)
          num1 = num3;
        float num4 = Vector3.DistanceSquared(position2, localEnabledPlayer.Position);
        if ((double) num4 < (double) num2)
          num2 = num4;
      }
      return num1.CompareTo(num2);
    }

    private void UpdateEmitters(MapTM map, CustomArray<ProceduralEmitter> emitters, int elapsed)
    {
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      lock (emitters)
      {
        for (int index = 0; index < emitters.Count; ++index)
        {
          emitters.Array[index].EmitCounter -= elapsed;
          if (emitters.Array[index].EmitCounter <= 0)
          {
            ProceduralEmitter proceduralEmitter = emitters.Array[index];
            emitters.Array[index].EmitCounter = proceduralEmitter.Data.EmitFreq;
            if (!this.EmitParticle(map, proceduralEmitter.Position, ref proceduralEmitter.Data))
              break;
          }
        }
      }
    }

    private bool EmitParticle(MapTM map, Vector3 pos, ref ParticleData data)
    {
      if (data.EmitFreq > 0 && data.Duration > (ushort) 0 && (data.StartColor.A > (byte) 0 || data.EndColor.A > (byte) 0) && ((double) data.Size.X > 0.0 || (double) data.Size.Y > 0.0 || (double) data.Size.Z > 0.0))
      {
        MapChunk chunk = map.GetChunk(pos);
        if (chunk != null && chunk.IsMeshLoaded)
          return this.instance.EmitterParticleSystem.AddParticle(pos, ref data);
      }
      return true;
    }

    private void RemoveExpiredProceduralEmitters(int elapsed)
    {
      lock (this.proceduralEmitters)
      {
        for (int i = this.proceduralEmitters.Count - 1; i >= 0; --i)
        {
          this.proceduralEmitters.Array[i].EmitterDuration -= elapsed;
          if (this.proceduralEmitters.Array[i].EmitterDuration < 0)
            this.proceduralEmitters.RemoveAt(i);
        }
      }
    }

    public void AddEmitter(Vector3 pos, int duration, ref ParticleData data)
    {
      ProceduralEmitter t = new ProceduralEmitter()
      {
        Position = pos,
        EmitterDuration = duration,
        Data = data
      };
      lock (this.proceduralEmitters)
        this.proceduralEmitters.Add(t);
    }
  }
}
