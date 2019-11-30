// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamUserStats
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamUserStats
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool RequestCurrentStats();

    public abstract bool GetStat(string pchName, ref int pData);

    public abstract bool GetStat0(string pchName, ref float pData);

    public abstract bool SetStat(string pchName, int nData);

    public abstract bool SetStat0(string pchName, float fData);

    public abstract bool UpdateAvgRateStat(
      string pchName,
      float flCountThisSession,
      double dSessionLength);

    public abstract bool GetAchievement(string pchName, out bool pbAchieved);

    public abstract bool SetAchievement(string pchName);

    public abstract bool ClearAchievement(string pchName);

    public abstract bool GetAchievementAndUnlockTime(
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime);

    public abstract bool StoreStats();

    public abstract int GetAchievementIcon(string pchName);

    public abstract string GetAchievementDisplayAttribute(string pchName, string pchKey);

    public abstract bool IndicateAchievementProgress(
      string pchName,
      uint nCurProgress,
      uint nMaxProgress);

    public abstract uint GetNumAchievements();

    public abstract string GetAchievementName(uint iAchievement);

    public abstract ulong RequestUserStats(ulong steamIDUser);

    public abstract bool GetUserStat(ulong steamIDUser, string pchName, ref int pData);

    public abstract bool GetUserStat0(ulong steamIDUser, string pchName, ref float pData);

    public abstract bool GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved);

    public abstract bool GetUserAchievementAndUnlockTime(
      ulong steamIDUser,
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime);

    public abstract bool ResetAllStats(bool bAchievementsToo);

    public abstract ulong FindOrCreateLeaderboard(
      string pchLeaderboardName,
      uint eLeaderboardSortMethod,
      uint eLeaderboardDisplayType);

    public abstract ulong FindLeaderboard(string pchLeaderboardName);

    public abstract string GetLeaderboardName(ulong hSteamLeaderboard);

    public abstract int GetLeaderboardEntryCount(ulong hSteamLeaderboard);

    public abstract uint GetLeaderboardSortMethod(ulong hSteamLeaderboard);

    public abstract uint GetLeaderboardDisplayType(ulong hSteamLeaderboard);

    public abstract ulong DownloadLeaderboardEntries(
      ulong hSteamLeaderboard,
      uint eLeaderboardDataRequest,
      int nRangeStart,
      int nRangeEnd);

    public abstract ulong DownloadLeaderboardEntriesForUsers(
      ulong hSteamLeaderboard,
      ulong[] prgUsers);

    public abstract bool GetDownloadedLeaderboardEntry(
      ulong hSteamLeaderboardEntries,
      int index,
      ref LeaderboardEntry_t pLeaderboardEntry,
      ref int pDetails,
      int cDetailsMax);

    public abstract ulong UploadLeaderboardScore(
      ulong hSteamLeaderboard,
      uint eLeaderboardUploadScoreMethod,
      int nScore,
      ref int pScoreDetails,
      int cScoreDetailsCount);

    public abstract ulong AttachLeaderboardUGC(ulong hSteamLeaderboard, ulong hUGC);

    public abstract ulong GetNumberOfCurrentPlayers();

    public abstract ulong RequestGlobalAchievementPercentages();

    public abstract int GetMostAchievedAchievementInfo(
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved);

    public abstract int GetNextMostAchievedAchievementInfo(
      int iIteratorPrevious,
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved);

    public abstract bool GetAchievementAchievedPercent(string pchName, ref float pflPercent);

    public abstract ulong RequestGlobalStats(int nHistoryDays);

    public abstract bool GetGlobalStat(string pchStatName, ref long pData);

    public abstract bool GetGlobalStat0(string pchStatName, ref double pData);

    public abstract int GetGlobalStatHistory(string pchStatName, long[] pData);

    public abstract int GetGlobalStatHistory0(string pchStatName, double[] pData);
  }
}
