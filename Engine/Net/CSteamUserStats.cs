// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamUserStats
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public class CSteamUserStats : ISteamUserStats
  {
    private IntPtr m_pSteamUserStats;

    public CSteamUserStats(IntPtr SteamUserStats)
    {
      this.m_pSteamUserStats = SteamUserStats;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamUserStats;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamUserStats == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool RequestCurrentStats()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_RequestCurrentStats(this.m_pSteamUserStats);
    }

    public override bool GetStat(string pchName, ref int pData)
    {
      this.CheckIfUsable();
      pData = 0;
      return NativeCalls.SteamAPI_ISteamUserStats_GetStat(this.m_pSteamUserStats, pchName, ref pData);
    }

    public override bool GetStat0(string pchName, ref float pData)
    {
      this.CheckIfUsable();
      pData = 0.0f;
      return NativeCalls.SteamAPI_ISteamUserStats_GetStat0(this.m_pSteamUserStats, pchName, ref pData);
    }

    public override bool SetStat(string pchName, int nData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_SetStat(this.m_pSteamUserStats, pchName, nData);
    }

    public override bool SetStat0(string pchName, float fData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_SetStat0(this.m_pSteamUserStats, pchName, fData);
    }

    public override bool UpdateAvgRateStat(
      string pchName,
      float flCountThisSession,
      double dSessionLength)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_UpdateAvgRateStat(this.m_pSteamUserStats, pchName, flCountThisSession, dSessionLength);
    }

    public override bool GetAchievement(string pchName, out bool pbAchieved)
    {
      this.CheckIfUsable();
      pbAchieved = false;
      return NativeCalls.SteamAPI_ISteamUserStats_GetAchievement(this.m_pSteamUserStats, pchName, out pbAchieved);
    }

    public override bool SetAchievement(string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_SetAchievement(this.m_pSteamUserStats, pchName);
    }

    public override bool ClearAchievement(string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_ClearAchievement(this.m_pSteamUserStats, pchName);
    }

    public override bool GetAchievementAndUnlockTime(
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime)
    {
      this.CheckIfUsable();
      pbAchieved = false;
      punUnlockTime = 0U;
      return NativeCalls.SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime(this.m_pSteamUserStats, pchName, ref pbAchieved, ref punUnlockTime);
    }

    public override bool StoreStats()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_StoreStats(this.m_pSteamUserStats);
    }

    public override int GetAchievementIcon(string pchName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetAchievementIcon(this.m_pSteamUserStats, pchName);
    }

    public override string GetAchievementDisplayAttribute(string pchName, string pchKey)
    {
      this.CheckIfUsable();
      IntPtr hglobalAnsi1 = Marshal.StringToHGlobalAnsi(pchName);
      IntPtr hglobalAnsi2 = Marshal.StringToHGlobalAnsi(pchKey);
      IntPtr displayAttribute = NativeCalls.SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute(this.m_pSteamUserStats, hglobalAnsi1, hglobalAnsi2);
      Marshal.FreeHGlobal(hglobalAnsi1);
      Marshal.FreeHGlobal(hglobalAnsi2);
      return InteropHelp.IntPtrToUTF8(displayAttribute);
    }

    public override bool IndicateAchievementProgress(
      string pchName,
      uint nCurProgress,
      uint nMaxProgress)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_IndicateAchievementProgress(this.m_pSteamUserStats, pchName, nCurProgress, nMaxProgress);
    }

    public override uint GetNumAchievements()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetNumAchievements(this.m_pSteamUserStats);
    }

    public override string GetAchievementName(uint iAchievement)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamUserStats_GetAchievementName(this.m_pSteamUserStats, iAchievement));
    }

    public override ulong RequestUserStats(ulong steamIDUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_RequestUserStats(this.m_pSteamUserStats, steamIDUser);
    }

    public override bool GetUserStat(ulong steamIDUser, string pchName, ref int pData)
    {
      this.CheckIfUsable();
      pData = 0;
      return NativeCalls.SteamAPI_ISteamUserStats_GetUserStat(this.m_pSteamUserStats, steamIDUser, pchName, ref pData);
    }

    public override bool GetUserStat0(ulong steamIDUser, string pchName, ref float pData)
    {
      this.CheckIfUsable();
      pData = 0.0f;
      return NativeCalls.SteamAPI_ISteamUserStats_GetUserStat0(this.m_pSteamUserStats, steamIDUser, pchName, ref pData);
    }

    public override bool GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved)
    {
      this.CheckIfUsable();
      pbAchieved = false;
      return NativeCalls.SteamAPI_ISteamUserStats_GetUserAchievement(this.m_pSteamUserStats, steamIDUser, pchName, ref pbAchieved);
    }

    public override bool GetUserAchievementAndUnlockTime(
      ulong steamIDUser,
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime)
    {
      this.CheckIfUsable();
      pbAchieved = false;
      punUnlockTime = 0U;
      return NativeCalls.SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime(this.m_pSteamUserStats, steamIDUser, pchName, ref pbAchieved, ref punUnlockTime);
    }

    public override bool ResetAllStats(bool bAchievementsToo)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_ResetAllStats(this.m_pSteamUserStats, bAchievementsToo);
    }

    public override ulong FindOrCreateLeaderboard(
      string pchLeaderboardName,
      uint eLeaderboardSortMethod,
      uint eLeaderboardDisplayType)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_FindOrCreateLeaderboard(this.m_pSteamUserStats, pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
    }

    public override ulong FindLeaderboard(string pchLeaderboardName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_FindLeaderboard(this.m_pSteamUserStats, pchLeaderboardName);
    }

    public override string GetLeaderboardName(ulong hSteamLeaderboard)
    {
      this.CheckIfUsable();
      return (string) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamUserStats_GetLeaderboardName(this.m_pSteamUserStats, hSteamLeaderboard), typeof (string));
    }

    public override int GetLeaderboardEntryCount(ulong hSteamLeaderboard)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetLeaderboardEntryCount(this.m_pSteamUserStats, hSteamLeaderboard);
    }

    public override uint GetLeaderboardSortMethod(ulong hSteamLeaderboard)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetLeaderboardSortMethod(this.m_pSteamUserStats, hSteamLeaderboard);
    }

    public override uint GetLeaderboardDisplayType(ulong hSteamLeaderboard)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetLeaderboardDisplayType(this.m_pSteamUserStats, hSteamLeaderboard);
    }

    public override ulong DownloadLeaderboardEntries(
      ulong hSteamLeaderboard,
      uint eLeaderboardDataRequest,
      int nRangeStart,
      int nRangeEnd)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_DownloadLeaderboardEntries(this.m_pSteamUserStats, hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
    }

    public override ulong DownloadLeaderboardEntriesForUsers(
      ulong hSteamLeaderboard,
      ulong[] prgUsers)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers(this.m_pSteamUserStats, hSteamLeaderboard, prgUsers, prgUsers.Length);
    }

    public override bool GetDownloadedLeaderboardEntry(
      ulong hSteamLeaderboardEntries,
      int index,
      ref LeaderboardEntry_t pLeaderboardEntry,
      ref int pDetails,
      int cDetailsMax)
    {
      this.CheckIfUsable();
      pDetails = 0;
      return NativeCalls.SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry(this.m_pSteamUserStats, hSteamLeaderboardEntries, index, ref pLeaderboardEntry, ref pDetails, cDetailsMax);
    }

    public override ulong UploadLeaderboardScore(
      ulong hSteamLeaderboard,
      uint eLeaderboardUploadScoreMethod,
      int nScore,
      ref int pScoreDetails,
      int cScoreDetailsCount)
    {
      this.CheckIfUsable();
      pScoreDetails = 0;
      return NativeCalls.SteamAPI_ISteamUserStats_UploadLeaderboardScore(this.m_pSteamUserStats, hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, ref pScoreDetails, cScoreDetailsCount);
    }

    public override ulong AttachLeaderboardUGC(ulong hSteamLeaderboard, ulong hUGC)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_AttachLeaderboardUGC(this.m_pSteamUserStats, hSteamLeaderboard, hUGC);
    }

    public override ulong GetNumberOfCurrentPlayers()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers(this.m_pSteamUserStats);
    }

    public override ulong RequestGlobalAchievementPercentages()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages(this.m_pSteamUserStats);
    }

    public override int GetMostAchievedAchievementInfo(
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved)
    {
      this.CheckIfUsable();
      pflPercent = 0.0f;
      pbAchieved = false;
      return NativeCalls.SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo(this.m_pSteamUserStats, pchName, unNameBufLen, ref pflPercent, ref pbAchieved);
    }

    public override int GetNextMostAchievedAchievementInfo(
      int iIteratorPrevious,
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved)
    {
      this.CheckIfUsable();
      pflPercent = 0.0f;
      pbAchieved = false;
      return NativeCalls.SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo(this.m_pSteamUserStats, iIteratorPrevious, pchName, unNameBufLen, ref pflPercent, ref pbAchieved);
    }

    public override bool GetAchievementAchievedPercent(string pchName, ref float pflPercent)
    {
      this.CheckIfUsable();
      pflPercent = 0.0f;
      return NativeCalls.SteamAPI_ISteamUserStats_GetAchievementAchievedPercent(this.m_pSteamUserStats, pchName, ref pflPercent);
    }

    public override ulong RequestGlobalStats(int nHistoryDays)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_RequestGlobalStats(this.m_pSteamUserStats, nHistoryDays);
    }

    public override bool GetGlobalStat(string pchStatName, ref long pData)
    {
      this.CheckIfUsable();
      pData = 0L;
      return NativeCalls.SteamAPI_ISteamUserStats_GetGlobalStat(this.m_pSteamUserStats, pchStatName, ref pData);
    }

    public override bool GetGlobalStat0(string pchStatName, ref double pData)
    {
      this.CheckIfUsable();
      pData = 0.0;
      return NativeCalls.SteamAPI_ISteamUserStats_GetGlobalStat0(this.m_pSteamUserStats, pchStatName, ref pData);
    }

    public override int GetGlobalStatHistory(string pchStatName, long[] pData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetGlobalStatHistory(this.m_pSteamUserStats, pchStatName, pData, (uint) pData.Length);
    }

    public override int GetGlobalStatHistory0(string pchStatName, double[] pData)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamUserStats_GetGlobalStatHistory0(this.m_pSteamUserStats, pchStatName, pData, (uint) pData.Length);
    }
  }
}
