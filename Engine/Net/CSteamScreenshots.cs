// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamScreenshots
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamScreenshots : ISteamScreenshots
  {
    private IntPtr m_pSteamScreenshots;

    public CSteamScreenshots(IntPtr SteamScreenshots)
    {
      this.m_pSteamScreenshots = SteamScreenshots;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamScreenshots;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamScreenshots == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamScreenshots_WriteScreenshot(this.m_pSteamScreenshots, pubRGB, cubRGB, nWidth, nHeight);
    }

    public override uint AddScreenshotToLibrary(
      string pchFilename,
      string pchThumbnailFilename,
      int nWidth,
      int nHeight)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamScreenshots_AddScreenshotToLibrary(this.m_pSteamScreenshots, pchFilename, pchThumbnailFilename, nWidth, nHeight);
    }

    public override void TriggerScreenshot()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamScreenshots_TriggerScreenshot(this.m_pSteamScreenshots);
    }

    public override void HookScreenshots(bool bHook)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamScreenshots_HookScreenshots(this.m_pSteamScreenshots, bHook);
    }

    public override bool SetLocation(uint hScreenshot, string pchLocation)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamScreenshots_SetLocation(this.m_pSteamScreenshots, hScreenshot, pchLocation);
    }

    public override bool TagUser(uint hScreenshot, ulong steamID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamScreenshots_TagUser(this.m_pSteamScreenshots, hScreenshot, steamID);
    }

    public override bool TagPublishedFile(uint hScreenshot, ulong unPublishedFileID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamScreenshots_TagPublishedFile(this.m_pSteamScreenshots, hScreenshot, unPublishedFileID);
    }
  }
}
