// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMatchmakingServers
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMatchmakingServers
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint RequestInternetServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract uint RequestLANServerList(
      uint iApp,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract uint RequestFriendsServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract uint RequestFavoritesServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract uint RequestHistoryServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract uint RequestSpectatorServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse);

    public abstract void ReleaseRequest(uint hServerListRequest);

    public abstract gameserveritem_t GetServerDetails(uint hRequest, int iServer);

    public abstract void CancelQuery(uint hRequest);

    public abstract void RefreshQuery(uint hRequest);

    public abstract bool IsRefreshing(uint hRequest);

    public abstract int GetServerCount(uint hRequest);

    public abstract void RefreshServer(uint hRequest, int iServer);

    public abstract uint PingServer(
      uint unIP,
      char usPort,
      ISteamMatchmakingPingResponse pRequestServersResponse);

    public abstract uint PlayerDetails(
      uint unIP,
      char usPort,
      ISteamMatchmakingPlayersResponse pRequestServersResponse);

    public abstract uint ServerRules(
      uint unIP,
      char usPort,
      ISteamMatchmakingRulesResponse pRequestServersResponse);

    public abstract void CancelServerQuery(uint hServerQuery);
  }
}
