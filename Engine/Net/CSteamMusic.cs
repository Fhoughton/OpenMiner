// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMusic
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamMusic : ISteamMusic
  {
    private IntPtr m_pSteamMusic;

    public CSteamMusic(IntPtr SteamMusic)
    {
      this.m_pSteamMusic = SteamMusic;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMusic;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMusic == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool BIsEnabled()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusic_BIsEnabled(this.m_pSteamMusic);
    }

    public override bool BIsPlaying()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusic_BIsPlaying(this.m_pSteamMusic);
    }

    public override int GetPlaybackStatus()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusic_GetPlaybackStatus(this.m_pSteamMusic);
    }

    public override void Play()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMusic_Play(this.m_pSteamMusic);
    }

    public override void Pause()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMusic_Pause(this.m_pSteamMusic);
    }

    public override void PlayPrevious()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMusic_PlayPrevious(this.m_pSteamMusic);
    }

    public override void PlayNext()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMusic_PlayNext(this.m_pSteamMusic);
    }

    public override void SetVolume(float flVolume)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMusic_SetVolume(this.m_pSteamMusic, flVolume);
    }

    public override float GetVolume()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMusic_GetVolume(this.m_pSteamMusic);
    }
  }
}
