// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamVideo
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamVideo : ISteamVideo
  {
    private IntPtr m_pSteamVideo;

    public CSteamVideo(IntPtr SteamVideo)
    {
      this.m_pSteamVideo = SteamVideo;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamVideo;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamVideo == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void GetVideoURL(uint unVideoAppID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamVideo_GetVideoURL(this.m_pSteamVideo, unVideoAppID);
    }

    public override bool IsBroadcasting(ref int pnNumViewers)
    {
      this.CheckIfUsable();
      pnNumViewers = 0;
      return NativeCalls.SteamAPI_ISteamVideo_IsBroadcasting(this.m_pSteamVideo, ref pnNumViewers);
    }
  }
}
