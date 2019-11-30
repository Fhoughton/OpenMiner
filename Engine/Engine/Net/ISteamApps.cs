// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamApps
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamApps
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool BIsSubscribed();

    public abstract bool BIsLowViolence();

    public abstract bool BIsCybercafe();

    public abstract bool BIsVACBanned();

    public abstract string GetCurrentGameLanguage();

    public abstract string GetAvailableGameLanguages();

    public abstract bool BIsSubscribedApp(uint appID);

    public abstract bool BIsDlcInstalled(uint appID);

    public abstract uint GetEarliestPurchaseUnixTime(uint nAppID);

    public abstract bool BIsSubscribedFromFreeWeekend();

    public abstract int GetDLCCount();

    public abstract bool BGetDLCDataByIndex(
      int iDLC,
      ref uint pAppID,
      ref bool pbAvailable,
      string pchName,
      int cchNameBufferSize);

    public abstract void InstallDLC(uint nAppID);

    public abstract void UninstallDLC(uint nAppID);

    public abstract void RequestAppProofOfPurchaseKey(uint nAppID);

    public abstract bool GetCurrentBetaName(string pchName, int cchNameBufferSize);

    public abstract bool MarkContentCorrupt(bool bMissingFilesOnly);

    public abstract uint GetInstalledDepots(uint appID, ref uint pvecDepots, uint cMaxDepots);

    public abstract uint GetAppInstallDir(uint appID, string pchFolder, uint cchFolderBufferSize);

    public abstract bool BIsAppInstalled(uint appID);

    public abstract ulong GetAppOwner();

    public abstract string GetLaunchQueryParam(string pchKey);

    public abstract bool GetDlcDownloadProgress(
      uint nAppID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal);

    public abstract int GetAppBuildId();
  }
}
