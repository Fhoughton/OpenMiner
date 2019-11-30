// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMusic
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMusic
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool BIsEnabled();

    public abstract bool BIsPlaying();

    public abstract int GetPlaybackStatus();

    public abstract void Play();

    public abstract void Pause();

    public abstract void PlayPrevious();

    public abstract void PlayNext();

    public abstract void SetVolume(float flVolume);

    public abstract float GetVolume();
  }
}
