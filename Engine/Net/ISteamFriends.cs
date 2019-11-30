// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamFriends
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamFriends
  {
    public abstract IntPtr GetIntPtr();

    public abstract string GetPersonaName();

    public abstract ulong SetPersonaName(string pchPersonaName);

    public abstract uint GetPersonaState();

    public abstract int GetFriendCount(int iFriendFlags);

    public abstract ulong GetFriendByIndex(int iFriend, int iFriendFlags);

    public abstract uint GetFriendRelationship(ulong steamIDFriend);

    public abstract uint GetFriendPersonaState(ulong steamIDFriend);

    public abstract string GetFriendPersonaName(ulong steamIDFriend);

    public abstract bool GetFriendGamePlayed(
      ulong steamIDFriend,
      out FriendGameInfo_t pFriendGameInfo);

    public abstract string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName);

    public abstract int GetFriendSteamLevel(ulong steamIDFriend);

    public abstract string GetPlayerNickname(ulong steamIDPlayer);

    public abstract int GetFriendsGroupCount();

    public abstract char GetFriendsGroupIDByIndex(int iFG);

    public abstract string GetFriendsGroupName(char friendsGroupID);

    public abstract int GetFriendsGroupMembersCount(char friendsGroupID);

    public abstract void GetFriendsGroupMembersList(
      char friendsGroupID,
      out ulong[] pOutSteamIDMembers);

    public abstract bool HasFriend(ulong steamIDFriend, int iFriendFlags);

    public abstract int GetClanCount();

    public abstract ulong GetClanByIndex(int iClan);

    public abstract string GetClanName(ulong steamIDClan);

    public abstract string GetClanTag(ulong steamIDClan);

    public abstract bool GetClanActivityCounts(
      ulong steamIDClan,
      ref int pnOnline,
      ref int pnInGame,
      ref int pnChatting);

    public abstract ulong DownloadClanActivityCounts(ulong[] psteamIDClans);

    public abstract int GetFriendCountFromSource(ulong steamIDSource);

    public abstract ulong GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend);

    public abstract bool IsUserInSource(ulong steamIDUser, ulong steamIDSource);

    public abstract void SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking);

    public abstract void ActivateGameOverlay(string pchDialog);

    public abstract void ActivateGameOverlayToUser(string pchDialog, ulong steamID);

    public abstract void ActivateGameOverlayToWebPage(string pchURL);

    public abstract void ActivateGameOverlayToStore(uint nAppID, EOverlayToStoreFlag eFlag);

    public abstract void SetPlayedWith(ulong steamIDUserPlayedWith);

    public abstract void ActivateGameOverlayInviteDialog(ulong steamIDLobby);

    public abstract int GetSmallFriendAvatar(ulong steamIDFriend);

    public abstract int GetMediumFriendAvatar(ulong steamIDFriend);

    public abstract int GetLargeFriendAvatar(ulong steamIDFriend);

    public abstract bool RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly);

    public abstract ulong RequestClanOfficerList(ulong steamIDClan);

    public abstract ulong GetClanOwner(ulong steamIDClan);

    public abstract int GetClanOfficerCount(ulong steamIDClan);

    public abstract ulong GetClanOfficerByIndex(ulong steamIDClan, int iOfficer);

    public abstract uint GetUserRestrictions();

    public abstract bool SetRichPresence(string pchKey, string pchValue);

    public abstract void ClearRichPresence();

    public abstract string GetFriendRichPresence(ulong steamIDFriend, string pchKey);

    public abstract int GetFriendRichPresenceKeyCount(ulong steamIDFriend);

    public abstract string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey);

    public abstract void RequestFriendRichPresence(ulong steamIDFriend);

    public abstract bool InviteUserToGame(ulong steamIDFriend, string pchConnectString);

    public abstract int GetCoplayFriendCount();

    public abstract ulong GetCoplayFriend(int iCoplayFriend);

    public abstract int GetFriendCoplayTime(ulong steamIDFriend);

    public abstract uint GetFriendCoplayGame(ulong steamIDFriend);

    public abstract ulong JoinClanChatRoom(ulong steamIDClan);

    public abstract bool LeaveClanChatRoom(ulong steamIDClan);

    public abstract int GetClanChatMemberCount(ulong steamIDClan);

    public abstract ulong GetChatMemberByIndex(ulong steamIDClan, int iUser);

    public abstract bool SendClanChatMessage(ulong steamIDClanChat, string pchText);

    public abstract int GetClanChatMessage(
      ulong steamIDClanChat,
      int iMessage,
      IntPtr prgchText,
      int cchTextMax,
      ref uint peChatEntryType,
      out ulong psteamidChatter);

    public abstract bool IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser);

    public abstract bool IsClanChatWindowOpenInSteam(ulong steamIDClanChat);

    public abstract bool OpenClanChatWindowInSteam(ulong steamIDClanChat);

    public abstract bool CloseClanChatWindowInSteam(ulong steamIDClanChat);

    public abstract bool SetListenForFriendsMessages(bool bInterceptEnabled);

    public abstract bool ReplyToFriendMessage(ulong steamIDFriend, string pchMsgToSend);

    public abstract int GetFriendMessage(
      ulong steamIDFriend,
      int iMessageID,
      IntPtr pvData,
      int cubData,
      ref uint peChatEntryType);

    public abstract ulong GetFollowerCount(ulong steamID);

    public abstract ulong IsFollowing(ulong steamID);

    public abstract ulong EnumerateFollowingList(uint unStartIndex);
  }
}
