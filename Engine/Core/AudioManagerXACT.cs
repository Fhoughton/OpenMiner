// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.AudioManagerXACT
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework.Audio;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class AudioManagerXACT : GameObjectBase, IAudioManager, IHasUpdate, IHasContent, IUnmanagedBuffer
  {
    private string settingsFile;
    private AudioEngine engine;
    private WaveBank musicWaveBank;
    private WaveBank soundWaveBank;
    private WaveBank soundStreamWaveBank;
    private SoundBank musicSoundBank;
    private SoundBank soundSoundBank;
    private SoundBank soundStreamSoundBank;
    private AudioCategory musicCategory;
    private AudioCategory soundsCategory;
    private float soundsVolume;
    private float musicVolume;
    private Dictionary<string, bool> streamedSounds;

    public event EventHandler SongStartedPlaying;

    private void Raise_SongStartedPlaying()
    {
      if (this.SongStartedPlaying == null)
        return;
      this.SongStartedPlaying((object) this, EventArgs.Empty);
    }

    public long BufferSize
    {
      get
      {
        return 0;
      }
    }

    public float MusicVolume
    {
      get
      {
        return this.musicVolume;
      }
      set
      {
        if (this.engine == null)
          return;
        this.musicCategory.SetVolume(this.musicVolume = value);
      }
    }

    public float SoundVolume
    {
      get
      {
        return this.soundsVolume;
      }
      set
      {
        if (this.engine == null)
          return;
        this.soundsCategory.SetVolume(this.soundsVolume = value);
      }
    }

    public bool IsMusicPlayerBusy { get; set; }

    public Cue CurrentCue { get; private set; }

    public AudioManagerXACT(string settingsFile)
    {
      this.settingsFile = settingsFile;
    }

    protected override void LoadContentCore(InitState state)
    {
      base.LoadContentCore(state);
      try
      {
        this.engine = new AudioEngine(this.settingsFile);
        this.soundWaveBank = new WaveBank(this.engine, "Content\\Audio\\Sounds.xwb");
        this.soundSoundBank = new SoundBank(this.engine, "Content\\Audio\\Sounds.xsb");
        this.soundStreamWaveBank = new WaveBank(this.engine, "Content\\Audio\\SoundStream.xwb", 0, (short) 8);
        this.soundStreamSoundBank = new SoundBank(this.engine, "Content\\Audio\\SoundStream.xsb");
        this.musicWaveBank = new WaveBank(this.engine, "Content\\Audio\\Music.xwb", 0, (short) 6);
        this.musicSoundBank = new SoundBank(this.engine, "Content\\Audio\\Music.xsb");
        this.soundsCategory = this.engine.GetCategory("Sounds");
        this.musicCategory = this.engine.GetCategory("Music");
      }
      catch (Exception ex)
      {
      }
      string[] strArray = Utils.Deserialize1<string[]>("Content\\Map\\StreamedSounds.xml");
      this.streamedSounds = new Dictionary<string, bool>(strArray.Length);
      foreach (string key in strArray)
        this.streamedSounds.Add(key, true);
    }

    public AudioCategory GetCategory(string name)
    {
      if (this.engine == null)
        return new AudioCategory();
      return this.engine.GetCategory(name);
    }

    public bool IsMusicPlaying
    {
      get
      {
        if (this.CurrentCue != null)
          return this.CurrentCue.IsPlaying;
        return false;
      }
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (this.engine == null)
        return;
      this.engine.Update();
    }

    public bool PlaySong(string asset)
    {
      Cue cue;
      return this.PlaySong(asset, out cue);
    }

    public bool PlaySong(string asset, out Cue cue)
    {
      cue = (Cue) null;
      if (asset != null && asset.Length > 0 && (this.engine != null && this.musicSoundBank != null))
      {
        if (this.musicWaveBank.IsPrepared)
        {
          try
          {
            cue = this.musicSoundBank.GetCue(asset);
            if (this.CurrentCue != null && !this.CurrentCue.IsDisposed)
              this.CurrentCue.Dispose();
            cue.Play();
            this.CurrentCue = cue;
            this.Raise_SongStartedPlaying();
            return true;
          }
          catch (ArgumentException ex)
          {
          }
          catch (InstancePlayLimitException ex)
          {
          }
        }
      }
      return false;
    }

    public bool PlaySound(string asset)
    {
      Cue cue;
      return this.PlaySound(asset, out cue);
    }

    public bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener)
    {
      Cue cue;
      return this.PlaySound(asset, emitter, listener, out cue);
    }

    public bool PlaySound(string asset, out Cue cue)
    {
      cue = (Cue) null;
      if (asset != null && asset.Length > 0 && (this.engine != null && this.soundWaveBank != null) && (this.soundStreamWaveBank != null && this.soundWaveBank.IsPrepared))
      {
        if (this.soundStreamWaveBank.IsPrepared)
        {
          try
          {
            cue = this.GetSoundBank(asset).GetCue(asset);
            cue.Play();
            return true;
          }
          catch (ArgumentException ex)
          {
          }
          catch (InstancePlayLimitException ex)
          {
          }
        }
      }
      return false;
    }

    public bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener, out Cue cue)
    {
      cue = (Cue) null;
      if (asset != null && asset.Length > 0)
      {
        if (this.engine != null)
        {
          try
          {
            cue = this.GetSoundBank(asset).GetCue(asset);
            if (emitter != null && listener != null)
              cue.Apply3D(listener, emitter);
            cue.Play();
            return true;
          }
          catch (ArgumentException ex)
          {
          }
          catch (InstancePlayLimitException ex)
          {
          }
        }
      }
      return false;
    }

    private SoundBank GetSoundBank(string asset)
    {
      if (!this.streamedSounds.ContainsKey(asset))
        return this.soundSoundBank;
      return this.soundStreamSoundBank;
    }
  }
}
