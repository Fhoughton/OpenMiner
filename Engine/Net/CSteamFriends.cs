// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamFriends
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamFriends : ISteamFriends
  {
    private IntPtr m_pSteamFriends;

    public CSteamFriends(IntPtr SteamFriends)
    {
      this.m_pSteamFriends = SteamFriends;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamFriends;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamFriends == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override string GetPersonaName()
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetPersonaName(this.m_pSteamFriends));
    }

    public override ulong SetPersonaName(string pchPersonaName)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_SetPersonaName(this.m_pSteamFriends, pchPersonaName);
    }

    public override uint GetPersonaState()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetPersonaState(this.m_pSteamFriends);
    }

    public override int GetFriendCount(int iFriendFlags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendCount(this.m_pSteamFriends, iFriendFlags);
    }

    public override ulong GetFriendByIndex(int iFriend, int iFriendFlags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendByIndex(this.m_pSteamFriends, iFriend, iFriendFlags);
    }

    public override uint GetFriendRelationship(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendRelationship(this.m_pSteamFriends, steamIDFriend);
    }

    public override uint GetFriendPersonaState(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendPersonaState(this.m_pSteamFriends, steamIDFriend);
    }

    public override string GetFriendPersonaName(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetFriendPersonaName(this.m_pSteamFriends, steamIDFriend));
    }

    public override bool GetFriendGamePlayed(
      ulong steamIDFriend,
      out FriendGameInfo_t pFriendGameInfo)
    {
      this.CheckIfUsable();
      pFriendGameInfo = new FriendGameInfo_t();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendGamePlayed(this.m_pSteamFriends, steamIDFriend, ref pFriendGameInfo);
    }

    public override string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(this.m_pSteamFriends, steamIDFriend, iPersonaName));
    }

    public override int GetFriendSteamLevel(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendSteamLevel(this.m_pSteamFriends, steamIDFriend);
    }

    public override string GetPlayerNickname(ulong steamIDPlayer)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetPlayerNickname(this.m_pSteamFriends, steamIDPlayer));
    }

    public override int GetFriendsGroupCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendsGroupCount(this.m_pSteamFriends);
    }

    public override char GetFriendsGroupIDByIndex(int iFG)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(this.m_pSteamFriends, iFG);
    }

    public override string GetFriendsGroupName(char friendsGroupID)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetFriendsGroupName(this.m_pSteamFriends, friendsGroupID));
    }

    public override int GetFriendsGroupMembersCount(char friendsGroupID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(this.m_pSteamFriends, friendsGroupID);
    }

    public override void GetFriendsGroupMembersList(
      char friendsGroupID,
      out ulong[] pOutSteamIDMembers)
    {
      this.CheckIfUsable();
      int groupMembersCount = this.GetFriendsGroupMembersCount(friendsGroupID);
      pOutSteamIDMembers = new ulong[groupMembersCount];
      NativeCalls.SteamAPI_ISteamFriends_GetFriendsGroupMembersList(this.m_pSteamFriends, friendsGroupID, pOutSteamIDMembers, groupMembersCount);
    }

    public override bool HasFriend(ulong steamIDFriend, int iFriendFlags)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_HasFriend(this.m_pSteamFriends, steamIDFriend, iFriendFlags);
    }

    public override int GetClanCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanCount(this.m_pSteamFriends);
    }

    public override ulong GetClanByIndex(int iClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanByIndex(this.m_pSteamFriends, iClan);
    }

    public override string GetClanName(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetClanName(this.m_pSteamFriends, steamIDClan));
    }

    public override string GetClanTag(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetClanTag(this.m_pSteamFriends, steamIDClan));
    }

    public override bool GetClanActivityCounts(
      ulong steamIDClan,
      ref int pnOnline,
      ref int pnInGame,
      ref int pnChatting)
    {
      this.CheckIfUsable();
      pnOnline = 0;
      pnInGame = 0;
      pnChatting = 0;
      return NativeCalls.SteamAPI_ISteamFriends_GetClanActivityCounts(this.m_pSteamFriends, steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
    }

    public override ulong DownloadClanActivityCounts(ulong[] psteamIDClans)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_DownloadClanActivityCounts(this.m_pSteamFriends, psteamIDClans, psteamIDClans.Length);
    }

    public override int GetFriendCountFromSource(ulong steamIDSource)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendCountFromSource(this.m_pSteamFriends, steamIDSource);
    }

    public override ulong GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(this.m_pSteamFriends, steamIDSource, iFriend);
    }

    public override bool IsUserInSource(ulong steamIDUser, ulong steamIDSource)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_IsUserInSource(this.m_pSteamFriends, steamIDUser, steamIDSource);
    }

    public override void SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(this.m_pSteamFriends, steamIDUser, bSpeaking);
    }

    public override void ActivateGameOverlay(string pchDialog)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ActivateGameOverlay(this.m_pSteamFriends, pchDialog);
    }

    public override void ActivateGameOverlayToUser(string pchDialog, ulong steamID)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ActivateGameOverlayToUser(this.m_pSteamFriends, pchDialog, steamID);
    }

    public override void ActivateGameOverlayToWebPage(string pchURL)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage(this.m_pSteamFriends, pchURL);
    }

    public override void ActivateGameOverlayToStore(uint nAppID, EOverlayToStoreFlag eFlag)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ActivateGameOverlayToStore(this.m_pSteamFriends, nAppID, eFlag);
    }

    public override void SetPlayedWith(ulong steamIDUserPlayedWith)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_SetPlayedWith(this.m_pSteamFriends, steamIDUserPlayedWith);
    }

    public override void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(this.m_pSteamFriends, steamIDLobby);
    }

    public override int GetSmallFriendAvatar(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetSmallFriendAvatar(this.m_pSteamFriends, steamIDFriend);
    }

    public override int GetMediumFriendAvatar(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetMediumFriendAvatar(this.m_pSteamFriends, steamIDFriend);
    }

    public override int GetLargeFriendAvatar(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetLargeFriendAvatar(this.m_pSteamFriends, steamIDFriend);
    }

    public override bool RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_RequestUserInformation(this.m_pSteamFriends, steamIDUser, bRequireNameOnly);
    }

    public override ulong RequestClanOfficerList(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_RequestClanOfficerList(this.m_pSteamFriends, steamIDClan);
    }

    public override ulong GetClanOwner(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanOwner(this.m_pSteamFriends, steamIDClan);
    }

    public override int GetClanOfficerCount(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanOfficerCount(this.m_pSteamFriends, steamIDClan);
    }

    public override ulong GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanOfficerByIndex(this.m_pSteamFriends, steamIDClan, iOfficer);
    }

    public override uint GetUserRestrictions()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetUserRestrictions(this.m_pSteamFriends);
    }

    public override bool SetRichPresence(string pchKey, string pchValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_SetRichPresence(this.m_pSteamFriends, pchKey, pchValue);
    }

    public override void ClearRichPresence()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_ClearRichPresence(this.m_pSteamFriends);
    }

    public override string GetFriendRichPresence(ulong steamIDFriend, string pchKey)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetFriendRichPresence(this.m_pSteamFriends, steamIDFriend, pchKey));
    }

    public override int GetFriendRichPresenceKeyCount(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(this.m_pSteamFriends, steamIDFriend);
    }

    public override string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
    {
      this.CheckIfUsable();
      return InteropHelp.IntPtrToUTF8(NativeCalls.SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(this.m_pSteamFriends, steamIDFriend, iKey));
    }

    public override void RequestFriendRichPresence(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamFriends_RequestFriendRichPresence(this.m_pSteamFriends, steamIDFriend);
    }

    public override bool InviteUserToGame(ulong steamIDFriend, string pchConnectString)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_InviteUserToGame(this.m_pSteamFriends, steamIDFriend, pchConnectString);
    }

    public override int GetCoplayFriendCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetCoplayFriendCount(this.m_pSteamFriends);
    }

    public override ulong GetCoplayFriend(int iCoplayFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetCoplayFriend(this.m_pSteamFriends, iCoplayFriend);
    }

    public override int GetFriendCoplayTime(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendCoplayTime(this.m_pSteamFriends, steamIDFriend);
    }

    public override uint GetFriendCoplayGame(ulong steamIDFriend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendCoplayGame(this.m_pSteamFriends, steamIDFriend);
    }

    public override ulong JoinClanChatRoom(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_JoinClanChatRoom(this.m_pSteamFriends, steamIDClan);
    }

    public override bool LeaveClanChatRoom(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_LeaveClanChatRoom(this.m_pSteamFriends, steamIDClan);
    }

    public override int GetClanChatMemberCount(ulong steamIDClan)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetClanChatMemberCount(this.m_pSteamFriends, steamIDClan);
    }

    public override ulong GetChatMemberByIndex(ulong steamIDClan, int iUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetChatMemberByIndex(this.m_pSteamFriends, steamIDClan, iUser);
    }

    public override bool SendClanChatMessage(ulong steamIDClanChat, string pchText)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_SendClanChatMessage(this.m_pSteamFriends, steamIDClanChat, pchText);
    }

    public override int GetClanChatMessage(
      ulong steamIDClanChat,
      int iMessage,
      IntPtr prgchText,
      int cchTextMax,
      ref uint peChatEntryType,
      out ulong psteamidChatter)
    {
      this.CheckIfUsable();
      peChatEntryType = 0U;
      psteamidChatter = 0UL;
      return NativeCalls.SteamAPI_ISteamFriends_GetClanChatMessage(this.m_pSteamFriends, steamIDClanChat, iMessage, prgchText, cchTextMax, ref peChatEntryType, ref psteamidChatter);
    }

    public override bool IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_IsClanChatAdmin(this.m_pSteamFriends, steamIDClanChat, steamIDUser);
    }

    public override bool IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(this.m_pSteamFriends, steamIDClanChat);
    }

    public override bool OpenClanChatWindowInSteam(ulong steamIDClanChat)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(this.m_pSteamFriends, steamIDClanChat);
    }

    public override bool CloseClanChatWindowInSteam(ulong steamIDClanChat)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(this.m_pSteamFriends, steamIDClanChat);
    }

    public override bool SetListenForFriendsMessages(bool bInterceptEnabled)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_SetListenForFriendsMessages(this.m_pSteamFriends, bInterceptEnabled);
    }

    public override bool ReplyToFriendMessage(ulong steamIDFriend, string pchMsgToSend)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_ReplyToFriendMessage(this.m_pSteamFriends, steamIDFriend, pchMsgToSend);
    }

    public override int GetFriendMessage(
      ulong steamIDFriend,
      int iMessageID,
      IntPtr pvData,
      int cubData,
      ref uint peChatEntryType)
    {
      this.CheckIfUsable();
      peChatEntryType = 0U;
      return NativeCalls.SteamAPI_ISteamFriends_GetFriendMessage(this.m_pSteamFriends, steamIDFriend, iMessageID, pvData, cubData, ref peChatEntryType);
    }

    public override ulong GetFollowerCount(ulong steamID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_GetFollowerCount(this.m_pSteamFriends, steamID);
    }

    public override ulong IsFollowing(ulong steamID)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_IsFollowing(this.m_pSteamFriends, steamID);
    }

    public override ulong EnumerateFollowingList(uint unStartIndex)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamFriends_EnumerateFollowingList(this.m_pSteamFriends, unStartIndex);
    }
  }
}
