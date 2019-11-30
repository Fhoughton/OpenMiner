// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamMatchmaking
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamMatchmaking
  {
    public abstract IntPtr GetIntPtr();

    public abstract int GetFavoriteGameCount();

    public abstract bool GetFavoriteGame(
      int iGame,
      ref uint pnAppID,
      ref uint pnIP,
      ref char pnConnPort,
      ref char pnQueryPort,
      ref uint punFlags,
      ref uint pRTime32LastPlayedOnServer);

    public abstract int AddFavoriteGame(
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags,
      uint rTime32LastPlayedOnServer);

    public abstract bool RemoveFavoriteGame(
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags);

    public abstract ulong RequestLobbyList();

    public abstract void AddRequestLobbyListStringFilter(
      string pchKeyToMatch,
      string pchValueToMatch,
      ELobbyComparison eComparisonType);

    public abstract void AddRequestLobbyListNumericalFilter(
      string pchKeyToMatch,
      int nValueToMatch,
      uint eComparisonType);

    public abstract void AddRequestLobbyListNearValueFilter(
      string pchKeyToMatch,
      int nValueToBeCloseTo);

    public abstract void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable);

    public abstract void AddRequestLobbyListDistanceFilter(uint eLobbyDistanceFilter);

    public abstract void AddRequestLobbyListResultCountFilter(int cMaxResults);

    public abstract void AddRequestLobbyListCompatibleMembersFilter(ulong steamIDLobby);

    public abstract ulong GetLobbyByIndex(int iLobby);

    public abstract ulong CreateLobby(ELobbyType eLobbyType, int cMaxMembers);

    public abstract ulong JoinLobby(ulong steamIDLobby);

    public abstract void LeaveLobby(ulong steamIDLobby);

    public abstract bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee);

    public abstract int GetNumLobbyMembers(ulong steamIDLobby);

    public abstract ulong GetLobbyMemberByIndex(ulong steamIDLobby, int iMember);

    public abstract string GetLobbyData(ulong steamIDLobby, string pchKey);

    public abstract bool SetLobbyData(ulong steamIDLobby, string pchKey, string pchValue);

    public abstract int GetLobbyDataCount(ulong steamIDLobby);

    public abstract bool GetLobbyDataByIndex(
      ulong steamIDLobby,
      int iLobbyData,
      string pchKey,
      int cchKeyBufferSize,
      string pchValue,
      int cchValueBufferSize);

    public abstract bool DeleteLobbyData(ulong steamIDLobby, string pchKey);

    public abstract string GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey);

    public abstract void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue);

    public abstract bool SendLobbyChatMsg(ulong steamIDLobby, byte[] pvMsgBody, int cubMsgBody);

    public abstract int GetLobbyChatEntry(
      ulong steamIDLobby,
      int iChatID,
      out ulong pSteamIDUser,
      byte[] pvData,
      int cubData,
      out EChatEntryType peChatEntryType);

    public abstract bool RequestLobbyData(ulong steamIDLobby);

    public abstract void SetLobbyGameServer(
      ulong steamIDLobby,
      uint unGameServerIP,
      char unGameServerPort,
      ulong steamIDGameServer);

    public abstract bool GetLobbyGameServer(
      ulong steamIDLobby,
      ref uint punGameServerIP,
      ref char punGameServerPort,
      out ulong psteamIDGameServer);

    public abstract bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers);

    public abstract int GetLobbyMemberLimit(ulong steamIDLobby);

    public abstract bool SetLobbyType(ulong steamIDLobby, uint eLobbyType);

    public abstract bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable);

    public abstract ulong GetLobbyOwner(ulong steamIDLobby);

    public abstract bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner);

    public abstract bool SetLinkedLobby(ulong steamIDLobby, ulong steamIDLobbyDependent);
  }
}
