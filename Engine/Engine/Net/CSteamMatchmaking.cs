// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamMatchmaking
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public class CSteamMatchmaking : ISteamMatchmaking
  {
    private IntPtr m_pSteamMatchmaking;

    public CSteamMatchmaking(IntPtr SteamMatchmaking)
    {
      this.m_pSteamMatchmaking = SteamMatchmaking;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamMatchmaking;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamMatchmaking == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override int GetFavoriteGameCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetFavoriteGameCount(this.m_pSteamMatchmaking);
    }

    public override bool GetFavoriteGame(
      int iGame,
      ref uint pnAppID,
      ref uint pnIP,
      ref char pnConnPort,
      ref char pnQueryPort,
      ref uint punFlags,
      ref uint pRTime32LastPlayedOnServer)
    {
      this.CheckIfUsable();
      pnAppID = 0U;
      pnIP = 0U;
      pnConnPort = char.MinValue;
      pnQueryPort = char.MinValue;
      punFlags = 0U;
      pRTime32LastPlayedOnServer = 0U;
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetFavoriteGame(this.m_pSteamMatchmaking, iGame, ref pnAppID, ref pnIP, ref pnConnPort, ref pnQueryPort, ref punFlags, ref pRTime32LastPlayedOnServer);
    }

    public override int AddFavoriteGame(
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags,
      uint rTime32LastPlayedOnServer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_AddFavoriteGame(this.m_pSteamMatchmaking, nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
    }

    public override bool RemoveFavoriteGame(
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_RemoveFavoriteGame(this.m_pSteamMatchmaking, nAppID, nIP, nConnPort, nQueryPort, unFlags);
    }

    public override ulong RequestLobbyList()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_RequestLobbyList(this.m_pSteamMatchmaking);
    }

    public override void AddRequestLobbyListStringFilter(
      string pchKeyToMatch,
      string pchValueToMatch,
      ELobbyComparison eComparisonType)
    {
      this.CheckIfUsable();
      IntPtr hglobalAnsi1 = Marshal.StringToHGlobalAnsi(pchKeyToMatch);
      IntPtr hglobalAnsi2 = Marshal.StringToHGlobalAnsi(pchValueToMatch);
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter(this.m_pSteamMatchmaking, hglobalAnsi1, hglobalAnsi2, eComparisonType);
      Marshal.FreeHGlobal(hglobalAnsi1);
      Marshal.FreeHGlobal(hglobalAnsi2);
    }

    public override void AddRequestLobbyListNumericalFilter(
      string pchKeyToMatch,
      int nValueToMatch,
      uint eComparisonType)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter(this.m_pSteamMatchmaking, pchKeyToMatch, nValueToMatch, eComparisonType);
    }

    public override void AddRequestLobbyListNearValueFilter(
      string pchKeyToMatch,
      int nValueToBeCloseTo)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter(this.m_pSteamMatchmaking, pchKeyToMatch, nValueToBeCloseTo);
    }

    public override void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(this.m_pSteamMatchmaking, nSlotsAvailable);
    }

    public override void AddRequestLobbyListDistanceFilter(uint eLobbyDistanceFilter)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter(this.m_pSteamMatchmaking, eLobbyDistanceFilter);
    }

    public override void AddRequestLobbyListResultCountFilter(int cMaxResults)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter(this.m_pSteamMatchmaking, cMaxResults);
    }

    public override void AddRequestLobbyListCompatibleMembersFilter(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override ulong GetLobbyByIndex(int iLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyByIndex(this.m_pSteamMatchmaking, iLobby);
    }

    public override ulong CreateLobby(ELobbyType eLobbyType, int cMaxMembers)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_CreateLobby(this.m_pSteamMatchmaking, eLobbyType, cMaxMembers);
    }

    public override ulong JoinLobby(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_JoinLobby(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override void LeaveLobby(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_LeaveLobby(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_InviteUserToLobby(this.m_pSteamMatchmaking, steamIDLobby, steamIDInvitee);
    }

    public override int GetNumLobbyMembers(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetNumLobbyMembers(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override ulong GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex(this.m_pSteamMatchmaking, steamIDLobby, iMember);
    }

    public override string GetLobbyData(ulong steamIDLobby, string pchKey)
    {
      this.CheckIfUsable();
      IntPtr hglobalAnsi = Marshal.StringToHGlobalAnsi(pchKey);
      IntPtr lobbyData = NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyData(this.m_pSteamMatchmaking, steamIDLobby, hglobalAnsi);
      Marshal.FreeHGlobal(hglobalAnsi);
      return InteropHelp.IntPtrToUTF8(lobbyData);
    }

    public override bool SetLobbyData(ulong steamIDLobby, string pchKey, string pchValue)
    {
      this.CheckIfUsable();
      IntPtr hglobalAnsi1 = Marshal.StringToHGlobalAnsi(pchKey);
      IntPtr hglobalAnsi2 = Marshal.StringToHGlobalAnsi(pchValue);
      bool flag = NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyData(this.m_pSteamMatchmaking, steamIDLobby, hglobalAnsi1, hglobalAnsi2);
      Marshal.FreeHGlobal(hglobalAnsi1);
      Marshal.FreeHGlobal(hglobalAnsi2);
      return flag;
    }

    public override int GetLobbyDataCount(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyDataCount(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override bool GetLobbyDataByIndex(
      ulong steamIDLobby,
      int iLobbyData,
      string pchKey,
      int cchKeyBufferSize,
      string pchValue,
      int cchValueBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex(this.m_pSteamMatchmaking, steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
    }

    public override bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_DeleteLobbyData(this.m_pSteamMatchmaking, steamIDLobby, pchKey);
    }

    public override string GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyMemberData(this.m_pSteamMatchmaking, steamIDLobby, steamIDUser, pchKey));
    }

    public override void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyMemberData(this.m_pSteamMatchmaking, steamIDLobby, pchKey, pchValue);
    }

    public override bool SendLobbyChatMsg(ulong steamIDLobby, byte[] pvMsgBody, int cubMsgBody)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SendLobbyChatMsg(this.m_pSteamMatchmaking, steamIDLobby, pvMsgBody, cubMsgBody);
    }

    public override int GetLobbyChatEntry(
      ulong steamIDLobby,
      int iChatID,
      out ulong pSteamIDUser,
      byte[] pvData,
      int cubData,
      out EChatEntryType peChatEntryType)
    {
      this.CheckIfUsable();
      pSteamIDUser = 0UL;
      peChatEntryType = EChatEntryType.k_EChatEntryTypeInvalid;
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyChatEntry(this.m_pSteamMatchmaking, steamIDLobby, iChatID, out pSteamIDUser, pvData, cubData, out peChatEntryType);
    }

    public override bool RequestLobbyData(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_RequestLobbyData(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override void SetLobbyGameServer(
      ulong steamIDLobby,
      uint unGameServerIP,
      char unGameServerPort,
      ulong steamIDGameServer)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyGameServer(this.m_pSteamMatchmaking, steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
    }

    public override bool GetLobbyGameServer(
      ulong steamIDLobby,
      ref uint punGameServerIP,
      ref char punGameServerPort,
      out ulong psteamIDGameServer)
    {
      this.CheckIfUsable();
      punGameServerIP = 0U;
      punGameServerPort = char.MinValue;
      psteamIDGameServer = 0UL;
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyGameServer(this.m_pSteamMatchmaking, steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer);
    }

    public override bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit(this.m_pSteamMatchmaking, steamIDLobby, cMaxMembers);
    }

    public override int GetLobbyMemberLimit(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override bool SetLobbyType(ulong steamIDLobby, uint eLobbyType)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyType(this.m_pSteamMatchmaking, steamIDLobby, eLobbyType);
    }

    public override bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyJoinable(this.m_pSteamMatchmaking, steamIDLobby, bLobbyJoinable);
    }

    public override ulong GetLobbyOwner(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_GetLobbyOwner(this.m_pSteamMatchmaking, steamIDLobby);
    }

    public override bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SetLobbyOwner(this.m_pSteamMatchmaking, steamIDLobby, steamIDNewOwner);
    }

    public override bool SetLinkedLobby(ulong steamIDLobby, ulong steamIDLobbyDependent)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamMatchmaking_SetLinkedLobby(this.m_pSteamMatchmaking, steamIDLobby, steamIDLobbyDependent);
    }
  }
}
