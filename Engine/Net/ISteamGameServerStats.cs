// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamGameServerStats
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamGameServerStats
  {
    public abstract IntPtr GetIntPtr();

    public abstract ulong RequestUserStats(ulong steamIDUser);

    public abstract bool GetUserStat(ulong steamIDUser, string pchName, ref int pData);

    public abstract bool GetUserStat0(ulong steamIDUser, string pchName, ref float pData);

    public abstract bool GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved);

    public abstract bool SetUserStat(ulong steamIDUser, string pchName, int nData);

    public abstract bool SetUserStat0(ulong steamIDUser, string pchName, float fData);

    public abstract bool UpdateUserAvgRateStat(
      ulong steamIDUser,
      string pchName,
      float flCountThisSession,
      double dSessionLength);

    public abstract bool SetUserAchievement(ulong steamIDUser, string pchName);

    public abstract bool ClearUserAchievement(ulong steamIDUser, string pchName);

    public abstract ulong StoreUserStats(ulong steamIDUser);
  }
}
