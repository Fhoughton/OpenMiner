// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamGameServerStats
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamGameServerStats : ISteamGameServerStats
  {
    private IntPtr m_pSteamGameServerStats;

    public CSteamGameServerStats(IntPtr SteamGameServerStats)
    {
      this.m_pSteamGameServerStats = SteamGameServerStats;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamGameServerStats;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamGameServerStats == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override ulong RequestUserStats(ulong steamIDUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_RequestUserStats(this.m_pSteamGameServerStats, steamIDUser);
    }

    public override bool GetUserStat(ulong steamIDUser, string pchName, ref int pData)
    {
      this.CheckIfUsable();
      pData = 0;
      return NativeCalls.SteamAPI_ISteamGameServerStats_GetUserStat(this.m_pSteamGameServerStats, steamIDUser, pchName, ref pData);
    }

    public override bool GetUserStat0(ulong steamIDUser, string pchName, ref float pData)
    {
      this.CheckIfUsable();
      pData = 0.0f;
      return NativeCalls.SteamAPI_ISteamGameServerStats_GetUserStat0(this.m_pSteamGameServerStats, steamIDUser, pchName, ref pData);
    }

    public override bool GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved)
    {
      this.CheckIfUsable();
      pbAchieved = false;
      return NativeCalls.SteamAPI_ISteamGameServerStats_GetUserAchievement(this.m_pSteamGameServerStats, steamIDUser, pchName, ref pbAchieved);
    }

    public override bool SetUserStat(ulong steamIDUser, string pchName, int nData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_SetUserStat(this.m_pSteamGameServerStats, steamIDUser, pchName, nData);
    }

    public override bool SetUserStat0(ulong steamIDUser, string pchName, float fData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_SetUserStat0(this.m_pSteamGameServerStats, steamIDUser, pchName, fData);
    }

    public override bool UpdateUserAvgRateStat(
      ulong steamIDUser,
      string pchName,
      float flCountThisSession,
      double dSessionLength)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(this.m_pSteamGameServerStats, steamIDUser, pchName, flCountThisSession, dSessionLength);
    }

    public override bool SetUserAchievement(ulong steamIDUser, string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_SetUserAchievement(this.m_pSteamGameServerStats, steamIDUser, pchName);
    }

    public override bool ClearUserAchievement(ulong steamIDUser, string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_ClearUserAchievement(this.m_pSteamGameServerStats, steamIDUser, pchName);
    }

    public override ulong StoreUserStats(ulong steamIDUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamGameServerStats_StoreUserStats(this.m_pSteamGameServerStats, steamIDUser);
    }
  }
}
