// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamAppList
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamAppList : ISteamAppList
  {
    private IntPtr m_pSteamAppList;

    public CSteamAppList(IntPtr SteamAppList)
    {
      this.m_pSteamAppList = SteamAppList;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamAppList;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamAppList == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint GetNumInstalledApps()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamAppList_GetNumInstalledApps(this.m_pSteamAppList);
    }

    public override uint GetInstalledApps(ref uint pvecAppID, uint unMaxAppIDs)
    {
      this.CheckIfUsable();
      pvecAppID = 0U;
      return NativeCalls.SteamAPI_ISteamAppList_GetInstalledApps(this.m_pSteamAppList, ref pvecAppID, unMaxAppIDs);
    }

    public override int GetAppName(uint nAppID, string pchName, int cchNameMax)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamAppList_GetAppName(this.m_pSteamAppList, nAppID, pchName, cchNameMax);
    }

    public override int GetAppInstallDir(uint nAppID, string pchDirectory, int cchNameMax)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamAppList_GetAppInstallDir(this.m_pSteamAppList, nAppID, pchDirectory, cchNameMax);
    }

    public override int GetAppBuildId(uint nAppID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamAppList_GetAppBuildId(this.m_pSteamAppList, nAppID);
    }
  }
}
