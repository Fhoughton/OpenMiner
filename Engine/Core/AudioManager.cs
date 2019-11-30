// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.AudioManager
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.Engine.Core
{
  public class AudioManager : GameObjectBase, IAudioManager, IHasUpdate, IHasContent, IUnmanagedBuffer, IAudioManagerStream
  {
    public static string RootSongsDirectory = "Songs\\";
    public static string RootSoundsDirectory = "Sounds\\";
    public static int EffectInstanceMax = 5;
    private float musicVolume = 0.5f;
    private float soundVolume = 1f;
    private object contentLock = new object();
    private Dictionary<string, Song> songCache;
    private Dictionary<string, SoundEffect> soundCache;
    private Dictionary<string, List<SoundEffectInstance>> soundInstanceCache;
    private IContentManager contentSound;
    private IContentManager contentSong;
    private FloatInterpolator fadeInInterpolator;
    private FloatInterpolator fadeOutInterpolator;
    private List<AudioManager.SoundQueueInstance> soundQueue;
    private List<AudioManager.SoundEffectFadeOut> soundEffectFadeouts;
    private float volumeFadeTimer;
    private bool songIsLoaded;
    private volatile Song nextSongToPlay;
    private IServiceProvider serviceProvider;

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
        long num = 0;
        lock (this.contentLock)
        {
          if (this.contentSound != null)
            num += this.contentSound.BufferSize;
          if (this.contentSong != null)
            num += this.contentSong.BufferSize;
        }
        return num;
      }
    }

    public Cue CurrentCue
    {
      get
      {
        return (Cue) null;
      }
    }

    public bool IsSongsCached { get; set; }

    public bool IsRepeating
    {
      get
      {
        return MediaPlayer.IsRepeating;
      }
      set
      {
        MediaPlayer.IsRepeating = value;
      }
    }

    public bool IsMusicPlaying
    {
      get
      {
        return MediaPlayer.State == MediaState.Playing;
      }
    }

    public bool IsMusicPlayerBusy { get; set; }

    public float FadePeriod { get; set; }

    public float MusicVolume
    {
      get
      {
        return this.musicVolume;
      }
      set
      {
        if ((double) this.musicVolume == (double) value)
          return;
        this.musicVolume = value;
        if ((double) this.musicVolume < 0.00999999977648258)
          this.musicVolume = 0.0f;
        else if ((double) this.musicVolume > 1.0)
          this.musicVolume = 1f;
        if ((double) MediaPlayer.Volume == (double) this.musicVolume)
          return;
        MediaPlayer.Volume = this.musicVolume;
      }
    }

    public float SoundVolume
    {
      get
      {
        return this.soundVolume;
      }
      set
      {
        if ((double) this.soundVolume == (double) value)
          return;
        this.soundVolume = value;
        if ((double) this.soundVolume < 0.00999999977648258)
        {
          this.soundVolume = 0.0f;
        }
        else
        {
          if ((double) this.soundVolume <= 1.0)
            return;
          this.soundVolume = 1f;
        }
      }
    }

    public AudioManager(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    protected override void InitializeCore(InitState state)
    {
      base.InitializeCore(state);
      this.IsSongsCached = true;
      this.FadePeriod = 2f;
      this.fadeInInterpolator = new FloatInterpolator();
      this.fadeOutInterpolator = new FloatInterpolator();
      this.soundQueue = new List<AudioManager.SoundQueueInstance>(10);
      this.soundEffectFadeouts = new List<AudioManager.SoundEffectFadeOut>();
    }

    protected override void LoadContentCore(InitState state)
    {
      this.contentSound = (IContentManager) new ContentManager(this.serviceProvider);
      this.contentSound.RootDirectory = "Content";
      this.contentSong = (IContentManager) new ContentManager(this.serviceProvider);
      this.contentSong.RootDirectory = "Content";
      this.songCache = new Dictionary<string, Song>();
      this.soundCache = new Dictionary<string, SoundEffect>();
      this.soundInstanceCache = new Dictionary<string, List<SoundEffectInstance>>();
    }

    protected override void UnloadContentCore()
    {
      lock (this.contentLock)
      {
        if (this.contentSound != null)
        {
          this.soundCache.Clear();
          this.soundInstanceCache.Clear();
          this.soundEffectFadeouts.Clear();
          this.contentSound.Unload();
        }
        if (this.contentSong != null)
        {
          this.songCache.Clear();
          this.contentSong.Unload();
        }
      }
      base.UnloadContentCore();
    }

    public Song LoadSong(string asset)
    {
      Song song = (Song) null;
      if (!asset.IsEmpty())
      {
        lock (this.contentLock)
        {
          if (this.IsSongsCached)
          {
            if (!this.songCache.TryGetValue(asset, out song))
            {
              song = this.contentSong.Load<Song>(AudioManager.RootSongsDirectory + asset);
              if (song != (Song) null)
                this.songCache.Add(asset, song);
            }
          }
          else
          {
            this.contentSong.Unload();
            song = this.contentSong.Load<Song>(AudioManager.RootSongsDirectory + asset);
          }
        }
      }
      return song;
    }

    public bool PlaySong(string asset)
    {
      return this.PlaySong(asset, this.MusicVolume);
    }

    public bool PlaySong(string asset, out Cue cue)
    {
      cue = (Cue) null;
      return this.PlaySong(asset, this.MusicVolume);
    }

    private bool PlaySong(string asset, float volume)
    {
      volume = MathHelper.Clamp(volume, 0.0f, this.MusicVolume);
      if ((double) volume <= 0.0)
        return false;
      this.nextSongToPlay = this.LoadSong(asset);
      if (this.IsMusicPlaying)
        this.fadeOutInterpolator.Start(MediaPlayer.Volume, 0.0f, (double) this.FadePeriod);
      else
        this.StartPlayingSong();
      this.fadeInInterpolator.Start(0.0f, volume, (double) this.FadePeriod);
      return true;
    }

    private void StartPlayingSong()
    {
      if ((double) this.MusicVolume <= 0.0 || !(this.nextSongToPlay != (Song) null))
        return;
      MediaPlayer.Volume = 0.0f;
      try
      {
        MediaPlayer.Play(this.nextSongToPlay);
        this.songIsLoaded = true;
        this.Raise_SongStartedPlaying();
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.nextSongToPlay = (Song) null;
      }
    }

    public void StopSong()
    {
      if (this.IsMusicPlaying)
      {
        this.fadeOutInterpolator.Start(MediaPlayer.Volume, 0.0f, (double) this.FadePeriod);
        this.nextSongToPlay = (Song) null;
      }
      else
        this.StopSongImmediately();
    }

    public void StopSongImmediately()
    {
      MediaPlayer.Stop();
      if (!this.IsSongsCached)
      {
        lock (this.contentLock)
          this.contentSong.Unload();
      }
      this.nextSongToPlay = (Song) null;
      this.songIsLoaded = false;
    }

    public SoundEffect LoadSoundEffect(string asset)
    {
      SoundEffect soundEffect = (SoundEffect) null;
      lock (this.contentLock)
      {
        if (this.soundCache.TryGetValue(asset, out soundEffect))
        {
          if (!soundEffect.IsDisposed)
            goto label_8;
        }
        soundEffect = this.contentSound.Load<SoundEffect>(AudioManager.RootSoundsDirectory + asset);
        if (soundEffect != null)
          this.soundCache.Add(asset, soundEffect);
      }
label_8:
      return soundEffect;
    }

    public SoundEffectInstance GetNewSoundEffectInstance(string asset)
    {
      return this.GetNewSoundEffectInstance(asset, this.SoundVolume);
    }

    public SoundEffectInstance GetNewSoundEffectInstance(
      string asset,
      float volume)
    {
      SoundEffect soundEffect = this.LoadSoundEffect(asset);
      if (soundEffect != null)
      {
        try
        {
          SoundEffectInstance instance = soundEffect.CreateInstance();
          instance.Volume = volume;
          return instance;
        }
        catch (Exception ex)
        {
        }
      }
      return (SoundEffectInstance) null;
    }

    public bool PlaySound(string asset)
    {
      lock (this.contentLock)
      {
        SoundEffect soundEffect = this.contentSound.Load<SoundEffect>(AudioManager.RootSoundsDirectory + asset);
        if (soundEffect != null)
        {
          soundEffect.Play();
          return true;
        }
      }
      return false;
    }

    public bool PlaySound(string asset, out Cue cue)
    {
      cue = (Cue) null;
      return this.PlaySound(asset);
    }

    public bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener)
    {
      return this.PlaySound(asset);
    }

    public bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener, out Cue cue)
    {
      return this.PlaySound(asset, out cue);
    }

    public bool PlaySound(Stream stream)
    {
      try
      {
        SoundEffect soundEffect = SoundEffect.FromStream(stream);
        if (soundEffect != null)
        {
          soundEffect.Play();
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public SoundEffect LoadSoundEffectFromStream(string asset)
    {
      SoundEffect soundEffect = (SoundEffect) null;
      lock (this.contentLock)
      {
        if (this.soundCache.TryGetValue(asset, out soundEffect))
        {
          if (!soundEffect.IsDisposed)
            goto label_14;
        }
        if (File.Exists(asset))
        {
          try
          {
            using (FileStream fileStream = File.OpenRead(asset))
            {
              soundEffect = SoundEffect.FromStream((Stream) fileStream);
              if (soundEffect != null)
                this.soundCache.Add(asset, soundEffect);
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
label_14:
      return soundEffect;
    }

    public int PlaySoundFromStream(string asset)
    {
      if (!File.Exists(asset))
        return 1;
      using (FileStream fileStream = File.OpenRead(asset))
        return this.PlaySound((Stream) fileStream) ? 2 : 0;
    }

    public int PlaySoundFromStream(string asset, AudioEmitter emitter)
    {
      return this.PlaySoundFromStream(asset);
    }

    public void QueueSound(string asset, float timeToWait)
    {
      lock (this.contentLock)
        this.soundQueue.Add(new AudioManager.SoundQueueInstance()
        {
          Asset = asset,
          Time = timeToWait
        });
    }

    public SoundEffectInstance PlayNewSoundEffectInstance(string asset)
    {
      return this.PlayNewSoundEffectInstance(asset, this.SoundVolume);
    }

    public SoundEffectInstance PlayNewSoundEffectInstance(
      string asset,
      float volume)
    {
      if (asset != null && asset.Length > 0)
      {
        volume = MathHelper.Clamp(volume, 0.0f, 1f);
        volume *= this.SoundVolume;
        if ((double) volume > 0.0)
        {
          lock (this.contentLock)
          {
            List<SoundEffectInstance> soundEffectInstanceList;
            if (!this.soundInstanceCache.TryGetValue(asset, out soundEffectInstanceList))
            {
              soundEffectInstanceList = new List<SoundEffectInstance>();
              this.soundInstanceCache.Add(asset, soundEffectInstanceList);
            }
            foreach (SoundEffectInstance soundEffectInstance in soundEffectInstanceList)
            {
              if (soundEffectInstance.State == SoundState.Stopped)
              {
                soundEffectInstance.Volume = volume;
                soundEffectInstance.Play();
                return soundEffectInstance;
              }
            }
            if (soundEffectInstanceList.Count < AudioManager.EffectInstanceMax)
            {
              SoundEffectInstance soundEffectInstance = this.GetNewSoundEffectInstance(asset, volume);
              if (soundEffectInstance != null)
              {
                soundEffectInstance.Play();
                soundEffectInstanceList.Add(soundEffectInstance);
              }
              return soundEffectInstance;
            }
          }
        }
      }
      return (SoundEffectInstance) null;
    }

    public void Fadeout(SoundEffectInstance effect, float time)
    {
      float num = time / effect.Volume;
      lock (this.contentLock)
        this.soundEffectFadeouts.Add(new AudioManager.SoundEffectFadeOut()
        {
          Effect = effect,
          Time = time,
          NormalizedFactor = num
        });
    }

    protected override void UpdateCore(UpdateState state)
    {
      base.UpdateCore(state);
      if (this.fadeOutInterpolator.IsActive)
      {
        double num = (double) this.fadeOutInterpolator.Update();
        this.FadeVolume(this.fadeOutInterpolator.CurrentValue);
        if (!this.fadeOutInterpolator.IsActive && this.nextSongToPlay == (Song) null)
          this.StopSongImmediately();
      }
      else
      {
        if (this.songIsLoaded && !this.IsSongsCached && !this.IsMusicPlaying)
        {
          lock (this.contentLock)
            this.contentSong.Unload();
          this.songIsLoaded = false;
        }
        this.StartPlayingSong();
        if (this.fadeInInterpolator.IsActive)
        {
          double num = (double) this.fadeInInterpolator.Update();
          this.FadeVolume(this.fadeInInterpolator.CurrentValue);
        }
      }
      this.UpdateQueuedSounds();
      this.UpdateEffectFadeouts();
    }

    private void UpdateQueuedSounds()
    {
      lock (this.contentLock)
      {
        for (int index = this.soundQueue.Count - 1; index >= 0; --index)
        {
          AudioManager.SoundQueueInstance sound = this.soundQueue[index];
          sound.Time -= Services.ElapsedTime;
          if ((double) sound.Time <= 0.0)
          {
            this.PlaySound(sound.Asset);
            this.soundQueue.RemoveAt(index);
          }
          else
            this.soundQueue[index] = sound;
        }
      }
    }

    private void UpdateEffectFadeouts()
    {
      float elapsedTime = Services.ElapsedTime;
      lock (this.contentLock)
      {
        for (int index = this.soundEffectFadeouts.Count - 1; index >= 0; --index)
        {
          AudioManager.SoundEffectFadeOut soundEffectFadeout = this.soundEffectFadeouts[index];
          soundEffectFadeout.Time -= elapsedTime;
          if ((double) soundEffectFadeout.Time <= 0.0 || (double) soundEffectFadeout.NormalizedFactor == 0.0)
          {
            soundEffectFadeout.Effect.Stop();
            this.soundEffectFadeouts.RemoveAt(index);
          }
          else
          {
            soundEffectFadeout.Effect.Volume = soundEffectFadeout.Time / soundEffectFadeout.NormalizedFactor;
            this.soundEffectFadeouts[index] = soundEffectFadeout;
          }
        }
      }
    }

    private void FadeVolume(float volume)
    {
      this.volumeFadeTimer -= Services.ElapsedTime;
      if ((double) this.volumeFadeTimer > 0.0)
        return;
      MediaPlayer.Volume = volume;
      this.volumeFadeTimer += 0.25f;
    }

    private enum SongState
    {
      None,
      FadingIn,
      FadingOut,
    }

    private struct SoundQueueInstance
    {
      public string Asset;
      public float Time;
    }

    private struct SoundEffectFadeOut
    {
      public SoundEffectInstance Effect;
      public float Time;
      public float NormalizedFactor;
    }
  }
}
