// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamApps
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamApps : ISteamApps
  {
    private IntPtr m_pSteamApps;

    public CSteamApps(IntPtr SteamApps)
    {
      this.m_pSteamApps = SteamApps;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamApps;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamApps == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool BIsSubscribed()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsSubscribed(this.m_pSteamApps);
    }

    public override bool BIsLowViolence()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsLowViolence(this.m_pSteamApps);
    }

    public override bool BIsCybercafe()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsCybercafe(this.m_pSteamApps);
    }

    public override bool BIsVACBanned()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsVACBanned(this.m_pSteamApps);
    }

    public override string GetCurrentGameLanguage()
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamApps_GetCurrentGameLanguage(this.m_pSteamApps));
    }

    public override string GetAvailableGameLanguages()
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamApps_GetAvailableGameLanguages(this.m_pSteamApps));
    }

    public override bool BIsSubscribedApp(uint appID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsSubscribedApp(this.m_pSteamApps, appID);
    }

    public override bool BIsDlcInstalled(uint appID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsDlcInstalled(this.m_pSteamApps, appID);
    }

    public override uint GetEarliestPurchaseUnixTime(uint nAppID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(this.m_pSteamApps, nAppID);
    }

    public override bool BIsSubscribedFromFreeWeekend()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend(this.m_pSteamApps);
    }

    public override int GetDLCCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetDLCCount(this.m_pSteamApps);
    }

    public override bool BGetDLCDataByIndex(
      int iDLC,
      ref uint pAppID,
      ref bool pbAvailable,
      string pchName,
      int cchNameBufferSize)
    {
      this.CheckIfUsable();
      pAppID = 0U;
      pbAvailable = false;
      return NativeCalls.SteamAPI_ISteamApps_BGetDLCDataByIndex(this.m_pSteamApps, iDLC, ref pAppID, ref pbAvailable, pchName, cchNameBufferSize);
    }

    public override void InstallDLC(uint nAppID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamApps_InstallDLC(this.m_pSteamApps, nAppID);
    }

    public override void UninstallDLC(uint nAppID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamApps_UninstallDLC(this.m_pSteamApps, nAppID);
    }

    public override void RequestAppProofOfPurchaseKey(uint nAppID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(this.m_pSteamApps, nAppID);
    }

    public override bool GetCurrentBetaName(string pchName, int cchNameBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetCurrentBetaName(this.m_pSteamApps, pchName, cchNameBufferSize);
    }

    public override bool MarkContentCorrupt(bool bMissingFilesOnly)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_MarkContentCorrupt(this.m_pSteamApps, bMissingFilesOnly);
    }

    public override uint GetInstalledDepots(uint appID, ref uint pvecDepots, uint cMaxDepots)
    {
      this.CheckIfUsable();
      pvecDepots = 0U;
      return NativeCalls.SteamAPI_ISteamApps_GetInstalledDepots(this.m_pSteamApps, appID, ref pvecDepots, cMaxDepots);
    }

    public override uint GetAppInstallDir(uint appID, string pchFolder, uint cchFolderBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetAppInstallDir(this.m_pSteamApps, appID, pchFolder, cchFolderBufferSize);
    }

    public override bool BIsAppInstalled(uint appID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_BIsAppInstalled(this.m_pSteamApps, appID);
    }

    public override ulong GetAppOwner()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetAppOwner(this.m_pSteamApps);
    }

    public override string GetLaunchQueryParam(string pchKey)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamApps_GetLaunchQueryParam(this.m_pSteamApps, pchKey));
    }

    public override bool GetDlcDownloadProgress(
      uint nAppID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal)
    {
      this.CheckIfUsable();
      punBytesDownloaded = 0UL;
      punBytesTotal = 0UL;
      return NativeCalls.SteamAPI_ISteamApps_GetDlcDownloadProgress(this.m_pSteamApps, nAppID, ref punBytesDownloaded, ref punBytesTotal);
    }

    public override int GetAppBuildId()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamApps_GetAppBuildId(this.m_pSteamApps);
    }
  }
}
