// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmakingServers
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmakingServers : ISteamMatchmakingServers
  {
    private IntPtr m_pSteamMatchmakingServers;

    public CSteamMatchmakingServers(IntPtr SteamMatchmakingServers)
    {
      this.m_pSteamMatchmakingServers = SteamMatchmakingServers;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmakingServers;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmakingServers == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint RequestInternetServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestInternetServerList(this.m_pSteamMatchmakingServers, iApp, ppchFilters, (uint) ppchFilters.Length, pRequestServersResponse.GetIntPtr());
    }

    public override uint RequestLANServerList(
      uint iApp,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestLANServerList(this.m_pSteamMatchmakingServers, iApp, pRequestServersResponse.GetIntPtr());
    }

    public override uint RequestFriendsServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList(this.m_pSteamMatchmakingServers, iApp, ppchFilters, (uint) ppchFilters.Length, pRequestServersResponse.GetIntPtr());
    }

    public override uint RequestFavoritesServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList(this.m_pSteamMatchmakingServers, iApp, ppchFilters, (uint) ppchFilters.Length, pRequestServersResponse.GetIntPtr());
    }

    public override uint RequestHistoryServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList(this.m_pSteamMatchmakingServers, iApp, ppchFilters, (uint) ppchFilters.Length, pRequestServersResponse.GetIntPtr());
    }

    public override uint RequestSpectatorServerList(
      uint iApp,
      MatchMakingKeyValuePair_t[] ppchFilters,
      ISteamMatchmakingServerListResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList(this.m_pSteamMatchmakingServers, iApp, ppchFilters, (uint) ppchFilters.Length, pRequestServersResponse.GetIntPtr());
    }

    public override void ReleaseRequest(uint hServerListRequest)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServers_ReleaseRequest(this.m_pSteamMatchmakingServers, hServerListRequest);
    }

    public override gameserveritem_t GetServerDetails(uint hRequest, int iServer)
    {
      this.CheckIfUsable();
      return (gameserveritem_t) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamMatchmakingServers_GetServerDetails(this.m_pSteamMatchmakingServers, hRequest, iServer), typeof (gameserveritem_t));
    }

    public override void CancelQuery(uint hRequest)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServers_CancelQuery(this.m_pSteamMatchmakingServers, hRequest);
    }

    public override void RefreshQuery(uint hRequest)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServers_RefreshQuery(this.m_pSteamMatchmakingServers, hRequest);
    }

    public override bool IsRefreshing(uint hRequest)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_IsRefreshing(this.m_pSteamMatchmakingServers, hRequest);
    }

    public override int GetServerCount(uint hRequest)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_GetServerCount(this.m_pSteamMatchmakingServers, hRequest);
    }

    public override void RefreshServer(uint hRequest, int iServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServers_RefreshServer(this.m_pSteamMatchmakingServers, hRequest, iServer);
    }

    public override uint PingServer(
      uint unIP,
      char usPort,
      ISteamMatchmakingPingResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_PingServer(this.m_pSteamMatchmakingServers, unIP, usPort, pRequestServersResponse.GetIntPtr());
    }

    public override uint PlayerDetails(
      uint unIP,
      char usPort,
      ISteamMatchmakingPlayersResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_PlayerDetails(this.m_pSteamMatchmakingServers, unIP, usPort, pRequestServersResponse.GetIntPtr());
    }

    public override uint ServerRules(
      uint unIP,
      char usPort,
      ISteamMatchmakingRulesResponse pRequestServersResponse)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmakingServers_ServerRules(this.m_pSteamMatchmakingServers, unIP, usPort, pRequestServersResponse.GetIntPtr());
    }

    public override void CancelServerQuery(uint hServerQuery)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmakingServers_CancelServerQuery(this.m_pSteamMatchmakingServers, hServerQuery);
    }
  }
}
