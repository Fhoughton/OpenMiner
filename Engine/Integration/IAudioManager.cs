// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IAudioManager
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StudioForge.Engine.Integration
{
  public interface IAudioManager : IHasUpdate, IHasContent, IUnmanagedBuffer
  {
    event EventHandler SongStartedPlaying;

    Cue CurrentCue { get; }

    float MusicVolume { get; set; }

    float SoundVolume { get; set; }

    bool IsMusicPlayerBusy { get; set; }

    bool IsMusicPlaying { get; }

    bool PlaySong(string asset);

    bool PlaySong(string asset, out Cue cue);

    bool PlaySound(string asset);

    bool PlaySound(string asset, out Cue cue);

    bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener);

    bool PlaySound(string asset, AudioEmitter emitter, AudioListener listener, out Cue cue);
  }
}
