// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NativeCalls
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace StudioForge.Engine.Net
{
  internal class NativeCalls
  {
    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamClient_CreateSteamPipe(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamClient_BReleaseSteamPipe(
      IntPtr instancePtr,
      uint hSteamPipe);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamClient_ConnectToGlobalUser(
      IntPtr instancePtr,
      uint hSteamPipe);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamClient_CreateLocalUser(
      IntPtr instancePtr,
      ref uint phSteamPipe,
      uint eAccountType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_ReleaseUser(
      IntPtr instancePtr,
      uint hSteamPipe,
      uint hUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamUser(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamGameServer(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_SetLocalIPBinding(
      IntPtr instancePtr,
      uint unIP,
      char usPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamFriends(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamUtils(
      IntPtr instancePtr,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamMatchmaking(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamMatchmakingServers(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamUserStats(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamGameServerStats(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamApps(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamNetworking(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamRemoteStorage(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamScreenshots(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_RunFrame(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamClient_GetIPCCallCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_SetWarningMessageHook(
      IntPtr instancePtr,
      IntPtr pFunction);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamHTTP(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamUnifiedMessages(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamController(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamUGC(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamAppList(
      IntPtr instancePtr,
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamMusic(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamMusicRemote(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamHTMLSurface(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_Set_SteamAPI_CPostAPIResultInProcess(
      IntPtr instancePtr,
      IntPtr func);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_Remove_SteamAPI_CPostAPIResultInProcess(
      IntPtr instancePtr,
      IntPtr func);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamClient_Set_SteamAPI_CCheckCallbackRegisteredInProcess(
      IntPtr instancePtr,
      IntPtr func);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamInventory(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamClient_GetISteamVideo(
      IntPtr instancePtr,
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUser_GetHSteamUser(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUser_BLoggedOn(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUser_GetSteamID(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUser_InitiateGameConnection(
      IntPtr instancePtr,
      IntPtr pAuthBlob,
      int cbMaxAuthBlob,
      ulong steamIDGameServer,
      uint unIPServer,
      char usPortServer,
      bool bSecure);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_TerminateGameConnection(
      IntPtr instancePtr,
      uint unIPServer,
      char usPortServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_TrackAppUsageEvent(
      IntPtr instancePtr,
      ulong gameID,
      int eAppUsageEvent,
      string pchExtraInfo);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUser_GetUserDataFolder(
      IntPtr instancePtr,
      string pchBuffer,
      int cubBuffer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_StartVoiceRecording(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_StopVoiceRecording(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern EVoiceResult SteamAPI_ISteamUser_GetAvailableVoice(
      IntPtr instancePtr,
      out uint pcbCompressed,
      out uint pcbUncompressed,
      uint nUncompressedVoiceDesiredSampleRate);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern EVoiceResult SteamAPI_ISteamUser_GetVoice(
      IntPtr instancePtr,
      [MarshalAs(UnmanagedType.I1)] bool bWantCompressed,
      [In, Out] byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      [MarshalAs(UnmanagedType.I1)] bool bWantUncompressed,
      [In, Out] byte[] pUncompressedDestBuffer,
      uint cbUncompressedDestBufferSize,
      out uint nUncompressBytesWritten,
      uint nUncompressedVoiceDesiredSampleRate);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern EVoiceResult SteamAPI_ISteamUser_DecompressVoice(
      IntPtr instancePtr,
      [In, Out] byte[] pCompressed,
      uint cbCompressed,
      [In, Out] byte[] pDestBuffer,
      uint cbDestBufferSize,
      out uint nBytesWritten,
      uint nDesiredSampleRate);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUser_GetVoiceOptimalSampleRate(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUser_GetAuthSessionTicket(
      IntPtr instancePtr,
      IntPtr pTicket,
      int cbMaxTicket,
      ref uint pcbTicket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUser_BeginAuthSession(
      IntPtr instancePtr,
      IntPtr pAuthTicket,
      int cbAuthTicket,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_EndAuthSession(
      IntPtr instancePtr,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_CancelAuthTicket(
      IntPtr instancePtr,
      uint hAuthTicket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUser_UserHasLicenseForApp(
      IntPtr instancePtr,
      ulong steamID,
      uint appID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUser_BIsBehindNAT(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUser_AdvertiseGame(
      IntPtr instancePtr,
      ulong steamIDGameServer,
      uint unIPServer,
      char usPortServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUser_RequestEncryptedAppTicket(
      IntPtr instancePtr,
      IntPtr pDataToInclude,
      int cbDataToInclude);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUser_GetEncryptedAppTicket(
      IntPtr instancePtr,
      IntPtr pTicket,
      int cbMaxTicket,
      ref uint pcbTicket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUser_GetGameBadgeLevel(
      IntPtr instancePtr,
      int nSeries,
      bool bFoil);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUser_GetPlayerSteamLevel(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUser_RequestStoreAuthURL(
      IntPtr instancePtr,
      string pchRedirectURL);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetPersonaName(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_SetPersonaName(
      IntPtr instancePtr,
      string pchPersonaName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamFriends_GetPersonaState(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendCount(
      IntPtr instancePtr,
      int iFriendFlags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetFriendByIndex(
      IntPtr instancePtr,
      int iFriend,
      int iFriendFlags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamFriends_GetFriendRelationship(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamFriends_GetFriendPersonaState(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetFriendPersonaName(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamFriends_GetFriendGamePlayed(
      IntPtr instancePtr,
      ulong steamIDFriend,
      ref FriendGameInfo_t pFriendGameInfo);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(
      IntPtr instancePtr,
      ulong steamIDFriend,
      int iPersonaName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendSteamLevel(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetPlayerNickname(
      IntPtr instancePtr,
      ulong steamIDPlayer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendsGroupCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern char SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(
      IntPtr instancePtr,
      int iFG);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetFriendsGroupName(
      IntPtr instancePtr,
      char friendsGroupID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(
      IntPtr instancePtr,
      char friendsGroupID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_GetFriendsGroupMembersList(
      IntPtr instancePtr,
      char friendsGroupID,
      [In, Out] ulong[] pOutSteamIDMembers,
      int nMembersCount);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_HasFriend(
      IntPtr instancePtr,
      ulong steamIDFriend,
      int iFriendFlags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetClanCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetClanByIndex(
      IntPtr instancePtr,
      int iClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetClanName(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetClanTag(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_GetClanActivityCounts(
      IntPtr instancePtr,
      ulong steamIDClan,
      ref int pnOnline,
      ref int pnInGame,
      ref int pnChatting);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_DownloadClanActivityCounts(
      IntPtr instancePtr,
      [In, Out] ulong[] psteamIDClans,
      int cClansToRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendCountFromSource(
      IntPtr instancePtr,
      ulong steamIDSource);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(
      IntPtr instancePtr,
      ulong steamIDSource,
      int iFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_IsUserInSource(
      IntPtr instancePtr,
      ulong steamIDUser,
      ulong steamIDSource);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(
      IntPtr instancePtr,
      ulong steamIDUser,
      [MarshalAs(UnmanagedType.I1)] bool bSpeaking);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ActivateGameOverlay(
      IntPtr instancePtr,
      string pchDialog);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ActivateGameOverlayToUser(
      IntPtr instancePtr,
      string pchDialog,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage(
      IntPtr instancePtr,
      string pchURL);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(
      IntPtr instancePtr,
      uint nAppID,
      EOverlayToStoreFlag eFlag);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_SetPlayedWith(
      IntPtr instancePtr,
      ulong steamIDUserPlayedWith);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetSmallFriendAvatar(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetMediumFriendAvatar(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetLargeFriendAvatar(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_RequestUserInformation(
      IntPtr instancePtr,
      ulong steamIDUser,
      bool bRequireNameOnly);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_RequestClanOfficerList(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetClanOwner(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetClanOfficerCount(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetClanOfficerByIndex(
      IntPtr instancePtr,
      ulong steamIDClan,
      int iOfficer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamFriends_GetUserRestrictions(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_SetRichPresence(
      IntPtr instancePtr,
      string pchKey,
      string pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_ClearRichPresence(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetFriendRichPresence(
      IntPtr instancePtr,
      ulong steamIDFriend,
      string pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(
      IntPtr instancePtr,
      ulong steamIDFriend,
      int iKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamFriends_RequestFriendRichPresence(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_InviteUserToGame(
      IntPtr instancePtr,
      ulong steamIDFriend,
      string pchConnectString);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetCoplayFriendCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetCoplayFriend(
      IntPtr instancePtr,
      int iCoplayFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendCoplayTime(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamFriends_GetFriendCoplayGame(
      IntPtr instancePtr,
      ulong steamIDFriend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_JoinClanChatRoom(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_LeaveClanChatRoom(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetClanChatMemberCount(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetChatMemberByIndex(
      IntPtr instancePtr,
      ulong steamIDClan,
      int iUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_SendClanChatMessage(
      IntPtr instancePtr,
      ulong steamIDClanChat,
      string pchText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetClanChatMessage(
      IntPtr instancePtr,
      ulong steamIDClanChat,
      int iMessage,
      IntPtr prgchText,
      int cchTextMax,
      ref uint peChatEntryType,
      ref ulong psteamidChatter);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_IsClanChatAdmin(
      IntPtr instancePtr,
      ulong steamIDClanChat,
      ulong steamIDUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(
      IntPtr instancePtr,
      ulong steamIDClanChat);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(
      IntPtr instancePtr,
      ulong steamIDClanChat);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(
      IntPtr instancePtr,
      ulong steamIDClanChat);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(
      IntPtr instancePtr,
      bool bInterceptEnabled);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamFriends_ReplyToFriendMessage(
      IntPtr instancePtr,
      ulong steamIDFriend,
      string pchMsgToSend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamFriends_GetFriendMessage(
      IntPtr instancePtr,
      ulong steamIDFriend,
      int iMessageID,
      IntPtr pvData,
      int cubData,
      ref uint peChatEntryType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_GetFollowerCount(
      IntPtr instancePtr,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_IsFollowing(
      IntPtr instancePtr,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamFriends_EnumerateFollowingList(
      IntPtr instancePtr,
      uint unStartIndex);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetSecondsSinceAppActive(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetSecondsSinceComputerActive(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUtils_GetConnectedUniverse(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetServerRealTime(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamUtils_GetIPCountry(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_GetImageSize(
      IntPtr instancePtr,
      int iImage,
      ref uint pnWidth,
      ref uint pnHeight);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamUtils_GetImageRGBA(
      IntPtr instancePtr,
      int iImage,
      [In, Out] byte[] pubDest,
      int nDestBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_GetCSERIPPort(
      IntPtr instancePtr,
      ref uint unIP,
      ref char usPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern byte SteamAPI_ISteamUtils_GetCurrentBatteryPower(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetAppID(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUtils_SetOverlayNotificationPosition(
      IntPtr instancePtr,
      uint eNotificationPosition);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_IsAPICallCompleted(
      IntPtr instancePtr,
      ulong hSteamAPICall,
      ref bool pbFailed);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUtils_GetAPICallFailureReason(
      IntPtr instancePtr,
      ulong hSteamAPICall);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_GetAPICallResult(
      IntPtr instancePtr,
      ulong hSteamAPICall,
      IntPtr pCallback,
      int cubCallback,
      int iCallbackExpected,
      ref bool pbFailed);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUtils_RunFrame(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetIPCCallCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUtils_SetWarningMessageHook(
      IntPtr instancePtr,
      SteamWarningMessageHookDelegate pFunction);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_IsOverlayEnabled(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_BOverlayNeedsPresent(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUtils_CheckFileSignature(
      IntPtr instancePtr,
      string szFileName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_ShowGamepadTextInput(
      IntPtr instancePtr,
      int eInputMode,
      int eLineInputMode,
      string pchDescription,
      uint unCharMax,
      string pchExistingText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUtils_GetEnteredGamepadTextLength(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_GetEnteredGamepadTextInput(
      IntPtr instancePtr,
      string pchText,
      uint cchText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamUtils_GetSteamUILanguage(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUtils_IsSteamRunningInVR(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUtils_SetOverlayNotificationInset(
      IntPtr instancePtr,
      int nHorizontalInset,
      int nVerticalInset);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_GetFavoriteGameCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_GetFavoriteGame(
      IntPtr instancePtr,
      int iGame,
      ref uint pnAppID,
      ref uint pnIP,
      ref char pnConnPort,
      ref char pnQueryPort,
      ref uint punFlags,
      ref uint pRTime32LastPlayedOnServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_AddFavoriteGame(
      IntPtr instancePtr,
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags,
      uint rTime32LastPlayedOnServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_RemoveFavoriteGame(
      IntPtr instancePtr,
      uint nAppID,
      uint nIP,
      char nConnPort,
      char nQueryPort,
      uint unFlags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_RequestLobbyList(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter(
      IntPtr instancePtr,
      IntPtr pchKeyToMatch,
      IntPtr pchValueToMatch,
      ELobbyComparison eComparisonType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter(
      IntPtr instancePtr,
      string pchKeyToMatch,
      int nValueToMatch,
      uint eComparisonType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter(
      IntPtr instancePtr,
      string pchKeyToMatch,
      int nValueToBeCloseTo);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(
      IntPtr instancePtr,
      int nSlotsAvailable);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter(
      IntPtr instancePtr,
      uint eLobbyDistanceFilter);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter(
      IntPtr instancePtr,
      int cMaxResults);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_GetLobbyByIndex(
      IntPtr instancePtr,
      int iLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_CreateLobby(
      IntPtr instancePtr,
      ELobbyType eLobbyType,
      int cMaxMembers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_JoinLobby(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_LeaveLobby(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_InviteUserToLobby(
      IntPtr instancePtr,
      ulong steamIDLobby,
      ulong steamIDInvitee);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_GetNumLobbyMembers(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex(
      IntPtr instancePtr,
      ulong steamIDLobby,
      int iMember);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamMatchmaking_GetLobbyData(
      IntPtr instancePtr,
      ulong steamIDLobby,
      IntPtr pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLobbyData(
      IntPtr instancePtr,
      ulong steamIDLobby,
      IntPtr pchKey,
      IntPtr pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_GetLobbyDataCount(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex(
      IntPtr instancePtr,
      ulong steamIDLobby,
      int iLobbyData,
      string pchKey,
      int cchKeyBufferSize,
      string pchValue,
      int cchValueBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_DeleteLobbyData(
      IntPtr instancePtr,
      ulong steamIDLobby,
      string pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamMatchmaking_GetLobbyMemberData(
      IntPtr instancePtr,
      ulong steamIDLobby,
      ulong steamIDUser,
      string pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_SetLobbyMemberData(
      IntPtr instancePtr,
      ulong steamIDLobby,
      string pchKey,
      string pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SendLobbyChatMsg(
      IntPtr instancePtr,
      ulong steamIDLobby,
      [In, Out] byte[] pvMsgBody,
      int cubMsgBody);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_GetLobbyChatEntry(
      IntPtr instancePtr,
      ulong steamIDLobby,
      int iChatID,
      out ulong pSteamIDUser,
      [In, Out] byte[] pvData,
      int cubData,
      out EChatEntryType peChatEntryType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_RequestLobbyData(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmaking_SetLobbyGameServer(
      IntPtr instancePtr,
      ulong steamIDLobby,
      uint unGameServerIP,
      char unGameServerPort,
      ulong steamIDGameServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_GetLobbyGameServer(
      IntPtr instancePtr,
      ulong steamIDLobby,
      ref uint punGameServerIP,
      ref char punGameServerPort,
      ref ulong psteamIDGameServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit(
      IntPtr instancePtr,
      ulong steamIDLobby,
      int cMaxMembers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLobbyType(
      IntPtr instancePtr,
      ulong steamIDLobby,
      uint eLobbyType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLobbyJoinable(
      IntPtr instancePtr,
      ulong steamIDLobby,
      bool bLobbyJoinable);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamMatchmaking_GetLobbyOwner(
      IntPtr instancePtr,
      ulong steamIDLobby);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLobbyOwner(
      IntPtr instancePtr,
      ulong steamIDLobby,
      ulong steamIDNewOwner);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmaking_SetLinkedLobby(
      IntPtr instancePtr,
      ulong steamIDLobby,
      ulong steamIDLobbyDependent);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServerListResponse_ServerResponded(
      IntPtr instancePtr,
      uint hRequest,
      int iServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServerListResponse_ServerFailedToRespond(
      IntPtr instancePtr,
      uint hRequest,
      int iServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServerListResponse_RefreshComplete(
      IntPtr instancePtr,
      uint hRequest,
      uint response);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingPingResponse_ServerResponded(
      IntPtr instancePtr,
      IntPtr server);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingPingResponse_ServerFailedToRespond(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingPlayersResponse_AddPlayerToList(
      IntPtr instancePtr,
      string pchName,
      int nScore,
      float flTimePlayed);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingPlayersResponse_PlayersFailedToRespond(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingPlayersResponse_PlayersRefreshComplete(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingRulesResponse_RulesResponded(
      IntPtr instancePtr,
      string pchRule,
      string pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingRulesResponse_RulesFailedToRespond(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingRulesResponse_RulesRefreshComplete(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestInternetServerList(
      IntPtr instancePtr,
      uint iApp,
      [In, Out] MatchMakingKeyValuePair_t[] ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestLANServerList(
      IntPtr instancePtr,
      uint iApp,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList(
      IntPtr instancePtr,
      uint iApp,
      [In, Out] MatchMakingKeyValuePair_t[] ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList(
      IntPtr instancePtr,
      uint iApp,
      [In, Out] MatchMakingKeyValuePair_t[] ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList(
      IntPtr instancePtr,
      uint iApp,
      [In, Out] MatchMakingKeyValuePair_t[] ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList(
      IntPtr instancePtr,
      uint iApp,
      [In, Out] MatchMakingKeyValuePair_t[] ppchFilters,
      uint nFilters,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServers_ReleaseRequest(
      IntPtr instancePtr,
      uint hServerListRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamMatchmakingServers_GetServerDetails(
      IntPtr instancePtr,
      uint hRequest,
      int iServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServers_CancelQuery(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServers_RefreshQuery(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMatchmakingServers_IsRefreshing(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMatchmakingServers_GetServerCount(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServers_RefreshServer(
      IntPtr instancePtr,
      uint hRequest,
      int iServer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_PingServer(
      IntPtr instancePtr,
      uint unIP,
      char usPort,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_PlayerDetails(
      IntPtr instancePtr,
      uint unIP,
      char usPort,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamMatchmakingServers_ServerRules(
      IntPtr instancePtr,
      uint unIP,
      char usPort,
      IntPtr pRequestServersResponse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMatchmakingServers_CancelServerQuery(
      IntPtr instancePtr,
      uint hServerQuery);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileWrite(
      IntPtr instancePtr,
      string pchFile,
      IntPtr pvData,
      int cubData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamRemoteStorage_FileRead(
      IntPtr instancePtr,
      string pchFile,
      IntPtr pvData,
      int cubDataToRead);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_FileWriteAsync(
      IntPtr instancePtr,
      string pchFile,
      IntPtr pvData,
      uint cubData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_FileReadAsync(
      IntPtr instancePtr,
      string pchFile,
      uint nOffset,
      uint cubToRead);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(
      IntPtr instancePtr,
      ulong hReadCall,
      IntPtr pvBuffer,
      uint cubToRead);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileForget(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileDelete(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_FileShare(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(
      IntPtr instancePtr,
      string pchFile,
      uint eRemoteStoragePlatform);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(
      IntPtr instancePtr,
      ulong writeHandle,
      IntPtr pvData,
      int cubData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(
      IntPtr instancePtr,
      ulong writeHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(
      IntPtr instancePtr,
      ulong writeHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FileExists(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_FilePersisted(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamRemoteStorage_GetFileSize(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern long SteamAPI_ISteamRemoteStorage_GetFileTimestamp(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(
      IntPtr instancePtr,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamRemoteStorage_GetFileCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(
      IntPtr instancePtr,
      int iFile,
      ref int pnFileSizeInBytes);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_GetQuota(
      IntPtr instancePtr,
      ref int pnTotalBytes,
      ref int puAvailableBytes);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(
      IntPtr instancePtr,
      bool bEnabled);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_UGCDownload(
      IntPtr instancePtr,
      ulong hContent,
      uint unPriority);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(
      IntPtr instancePtr,
      ulong hContent,
      ref int pnBytesDownloaded,
      ref int pnBytesExpected);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_GetUGCDetails(
      IntPtr instancePtr,
      ulong hContent,
      ref uint pnAppID,
      string ppchName,
      ref int pnFileSizeInBytes,
      ref ulong pSteamIDOwner);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamRemoteStorage_UGCRead(
      IntPtr instancePtr,
      ulong hContent,
      IntPtr pvData,
      int cubDataToRead,
      uint cOffset,
      uint eAction);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamRemoteStorage_GetCachedUGCCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(
      IntPtr instancePtr,
      int iCachedContent);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(
      IntPtr instancePtr,
      string pchFile,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags,
      uint eWorkshopFileType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(
      IntPtr instancePtr,
      ulong updateHandle,
      string pchFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(
      IntPtr instancePtr,
      ulong updateHandle,
      string pchPreviewFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(
      IntPtr instancePtr,
      ulong updateHandle,
      string pchTitle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(
      IntPtr instancePtr,
      ulong updateHandle,
      string pchDescription);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(
      IntPtr instancePtr,
      ulong updateHandle,
      uint eVisibility);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(
      IntPtr instancePtr,
      ulong updateHandle,
      ref SteamParamStringArray_t pTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(
      IntPtr instancePtr,
      ulong updateHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(
      IntPtr instancePtr,
      ulong unPublishedFileId,
      uint unMaxSecondsOld);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_DeletePublishedFile(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(
      IntPtr instancePtr,
      uint unStartIndex);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(
      IntPtr instancePtr,
      uint unStartIndex);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(
      IntPtr instancePtr,
      ulong updateHandle,
      string pchChangeDescription);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(
      IntPtr instancePtr,
      ulong unPublishedFileId,
      bool bVoteUp);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(
      IntPtr instancePtr,
      ulong unPublishedFileId);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(
      IntPtr instancePtr,
      ulong steamId,
      uint unStartIndex,
      ref SteamParamStringArray_t pRequiredTags,
      ref SteamParamStringArray_t pExcludedTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_PublishVideo(
      IntPtr instancePtr,
      uint eVideoProvider,
      string pchVideoAccount,
      string pchVideoIdentifier,
      string pchPreviewFile,
      uint nConsumerAppId,
      string pchTitle,
      string pchDescription,
      uint eVisibility,
      ref SteamParamStringArray_t pTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(
      IntPtr instancePtr,
      ulong unPublishedFileId,
      uint eAction);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(
      IntPtr instancePtr,
      uint eAction,
      uint unStartIndex);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(
      IntPtr instancePtr,
      uint eEnumerationType,
      uint unStartIndex,
      uint unCount,
      uint unDays,
      ref SteamParamStringArray_t pTags,
      ref SteamParamStringArray_t pUserTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(
      IntPtr instancePtr,
      ulong hContent,
      string pchLocation,
      uint unPriority);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_RequestCurrentStats(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetStat(
      IntPtr instancePtr,
      string pchName,
      ref int pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetStat0(
      IntPtr instancePtr,
      string pchName,
      ref float pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_SetStat(
      IntPtr instancePtr,
      string pchName,
      int nData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_SetStat0(
      IntPtr instancePtr,
      string pchName,
      float fData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_UpdateAvgRateStat(
      IntPtr instancePtr,
      string pchName,
      float flCountThisSession,
      double dSessionLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetAchievement(
      IntPtr instancePtr,
      string pchName,
      out bool pbAchieved);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_SetAchievement(
      IntPtr instancePtr,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_ClearAchievement(
      IntPtr instancePtr,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime(
      IntPtr instancePtr,
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_StoreStats(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetAchievementIcon(
      IntPtr instancePtr,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute(
      IntPtr instancePtr,
      IntPtr pchName,
      IntPtr pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_IndicateAchievementProgress(
      IntPtr instancePtr,
      string pchName,
      uint nCurProgress,
      uint nMaxProgress);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUserStats_GetNumAchievements(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamUserStats_GetAchievementName(
      IntPtr instancePtr,
      uint iAchievement);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_RequestUserStats(
      IntPtr instancePtr,
      ulong steamIDUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetUserStat(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref int pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetUserStat0(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref float pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetUserAchievement(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref bool pbAchieved);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref bool pbAchieved,
      ref uint punUnlockTime);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_ResetAllStats(
      IntPtr instancePtr,
      bool bAchievementsToo);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_FindOrCreateLeaderboard(
      IntPtr instancePtr,
      string pchLeaderboardName,
      uint eLeaderboardSortMethod,
      uint eLeaderboardDisplayType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_FindLeaderboard(
      IntPtr instancePtr,
      string pchLeaderboardName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamUserStats_GetLeaderboardName(
      IntPtr instancePtr,
      ulong hSteamLeaderboard);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetLeaderboardEntryCount(
      IntPtr instancePtr,
      ulong hSteamLeaderboard);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUserStats_GetLeaderboardSortMethod(
      IntPtr instancePtr,
      ulong hSteamLeaderboard);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUserStats_GetLeaderboardDisplayType(
      IntPtr instancePtr,
      ulong hSteamLeaderboard);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_DownloadLeaderboardEntries(
      IntPtr instancePtr,
      ulong hSteamLeaderboard,
      uint eLeaderboardDataRequest,
      int nRangeStart,
      int nRangeEnd);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers(
      IntPtr instancePtr,
      ulong hSteamLeaderboard,
      [In, Out] ulong[] prgUsers,
      int cUsers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry(
      IntPtr instancePtr,
      ulong hSteamLeaderboardEntries,
      int index,
      ref LeaderboardEntry_t pLeaderboardEntry,
      ref int pDetails,
      int cDetailsMax);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_UploadLeaderboardScore(
      IntPtr instancePtr,
      ulong hSteamLeaderboard,
      uint eLeaderboardUploadScoreMethod,
      int nScore,
      ref int pScoreDetails,
      int cScoreDetailsCount);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_AttachLeaderboardUGC(
      IntPtr instancePtr,
      ulong hSteamLeaderboard,
      ulong hUGC);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo(
      IntPtr instancePtr,
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo(
      IntPtr instancePtr,
      int iIteratorPrevious,
      string pchName,
      uint unNameBufLen,
      ref float pflPercent,
      ref bool pbAchieved);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetAchievementAchievedPercent(
      IntPtr instancePtr,
      string pchName,
      ref float pflPercent);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUserStats_RequestGlobalStats(
      IntPtr instancePtr,
      int nHistoryDays);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetGlobalStat(
      IntPtr instancePtr,
      string pchStatName,
      ref long pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUserStats_GetGlobalStat0(
      IntPtr instancePtr,
      string pchStatName,
      ref double pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetGlobalStatHistory(
      IntPtr instancePtr,
      string pchStatName,
      [In, Out] long[] pData,
      uint cubData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamUserStats_GetGlobalStatHistory0(
      IntPtr instancePtr,
      string pchStatName,
      [In, Out] double[] pData,
      uint cubData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsSubscribed(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsLowViolence(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsCybercafe(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsVACBanned(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamApps_GetCurrentGameLanguage(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamApps_GetAvailableGameLanguages(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsSubscribedApp(IntPtr instancePtr, uint appID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamApps_BIsDlcInstalled(IntPtr instancePtr, uint appID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(
      IntPtr instancePtr,
      uint nAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamApps_GetDLCCount(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BGetDLCDataByIndex(
      IntPtr instancePtr,
      int iDLC,
      ref uint pAppID,
      ref bool pbAvailable,
      string pchName,
      int cchNameBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamApps_InstallDLC(IntPtr instancePtr, uint nAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamApps_UninstallDLC(IntPtr instancePtr, uint nAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(
      IntPtr instancePtr,
      uint nAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_GetCurrentBetaName(
      IntPtr instancePtr,
      string pchName,
      int cchNameBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_MarkContentCorrupt(
      IntPtr instancePtr,
      bool bMissingFilesOnly);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamApps_GetInstalledDepots(
      IntPtr instancePtr,
      uint appID,
      ref uint pvecDepots,
      uint cMaxDepots);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamApps_GetAppInstallDir(
      IntPtr instancePtr,
      uint appID,
      string pchFolder,
      uint cchFolderBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_BIsAppInstalled(IntPtr instancePtr, uint appID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamApps_GetAppOwner(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_ISteamApps_GetLaunchQueryParam(
      IntPtr instancePtr,
      string pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamApps_GetDlcDownloadProgress(
      IntPtr instancePtr,
      uint nAppID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamApps_GetAppBuildId(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_SendP2PPacket(
      IntPtr instancePtr,
      ulong steamIDRemote,
      [In, Out] byte[] pubData,
      uint cubData,
      EP2PSend eP2PSendType,
      int nChannel);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_IsP2PPacketAvailable(
      IntPtr instancePtr,
      out uint pcubMsgSize,
      int nChannel);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_ReadP2PPacket(
      IntPtr instancePtr,
      [In, Out] byte[] pubDest,
      uint cubDest,
      out uint pcubMsgSize,
      out ulong psteamIDRemote,
      int nChannel);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser(
      IntPtr instancePtr,
      ulong steamIDRemote);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_CloseP2PSessionWithUser(
      IntPtr instancePtr,
      ulong steamIDRemote);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_CloseP2PChannelWithUser(
      IntPtr instancePtr,
      ulong steamIDRemote,
      int nChannel);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_GetP2PSessionState(
      IntPtr instancePtr,
      ulong steamIDRemote,
      ref P2PSessionState_t pConnectionState);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_AllowP2PPacketRelay(
      IntPtr instancePtr,
      [MarshalAs(UnmanagedType.I1)] bool bAllow);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworking_CreateListenSocket(
      IntPtr instancePtr,
      int nVirtualP2PPort,
      uint nIP,
      char nPort,
      bool bAllowUseOfPacketRelay);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworking_CreateP2PConnectionSocket(
      IntPtr instancePtr,
      ulong steamIDTarget,
      int nVirtualPort,
      int nTimeoutSec,
      bool bAllowUseOfPacketRelay);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworking_CreateConnectionSocket(
      IntPtr instancePtr,
      uint nIP,
      char nPort,
      int nTimeoutSec);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_DestroySocket(
      IntPtr instancePtr,
      uint hSocket,
      bool bNotifyRemoteEnd);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_DestroyListenSocket(
      IntPtr instancePtr,
      uint hSocket,
      bool bNotifyRemoteEnd);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_SendDataOnSocket(
      IntPtr instancePtr,
      uint hSocket,
      IntPtr pubData,
      uint cubData,
      bool bReliable);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_IsDataAvailableOnSocket(
      IntPtr instancePtr,
      uint hSocket,
      ref uint pcubMsgSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_RetrieveDataFromSocket(
      IntPtr instancePtr,
      uint hSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_IsDataAvailable(
      IntPtr instancePtr,
      uint hListenSocket,
      ref uint pcubMsgSize,
      ref uint phSocket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_RetrieveData(
      IntPtr instancePtr,
      uint hListenSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize,
      ref uint phSocket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_GetSocketInfo(
      IntPtr instancePtr,
      uint hSocket,
      ref ulong pSteamIDRemote,
      ref int peSocketStatus,
      ref uint punIPRemote,
      ref char punPortRemote);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool SteamAPI_ISteamNetworking_GetListenSocketInfo(
      IntPtr instancePtr,
      uint hListenSocket,
      ref uint pnIP,
      ref char pnPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworking_GetSocketConnectionType(
      IntPtr instancePtr,
      uint hSocket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamNetworking_GetMaxPacketSize(
      IntPtr instancePtr,
      uint hSocket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamScreenshots_WriteScreenshot(
      IntPtr instancePtr,
      IntPtr pubRGB,
      uint cubRGB,
      int nWidth,
      int nHeight);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamScreenshots_AddScreenshotToLibrary(
      IntPtr instancePtr,
      string pchFilename,
      string pchThumbnailFilename,
      int nWidth,
      int nHeight);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamScreenshots_TriggerScreenshot(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamScreenshots_HookScreenshots(
      IntPtr instancePtr,
      bool bHook);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamScreenshots_SetLocation(
      IntPtr instancePtr,
      uint hScreenshot,
      string pchLocation);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamScreenshots_TagUser(
      IntPtr instancePtr,
      uint hScreenshot,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamScreenshots_TagPublishedFile(
      IntPtr instancePtr,
      uint hScreenshot,
      ulong unPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusic_BIsEnabled(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusic_BIsPlaying(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamMusic_GetPlaybackStatus(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMusic_Play(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMusic_Pause(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMusic_PlayPrevious(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMusic_PlayNext(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamMusic_SetVolume(IntPtr instancePtr, float flVolume);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern float SteamAPI_ISteamMusic_GetVolume(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote(
      IntPtr instancePtr,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_BActivationSuccess(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetDisplayName(
      IntPtr instancePtr,
      string pchDisplayName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64(
      IntPtr instancePtr,
      IntPtr pvBuffer,
      uint cbBufferLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnablePlayPrevious(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnablePlayNext(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnableShuffled(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnableLooped(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnableQueue(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_EnablePlaylists(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus(
      IntPtr instancePtr,
      int nStatus);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateShuffled(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateLooped(
      IntPtr instancePtr,
      bool bValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateVolume(
      IntPtr instancePtr,
      float flValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_CurrentEntryWillChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable(
      IntPtr instancePtr,
      bool bAvailable);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText(
      IntPtr instancePtr,
      string pchText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(
      IntPtr instancePtr,
      int nValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt(
      IntPtr instancePtr,
      IntPtr pvBuffer,
      uint cbBufferLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_CurrentEntryDidChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_QueueWillChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_ResetQueueEntries(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetQueueEntry(
      IntPtr instancePtr,
      int nID,
      int nPosition,
      string pchEntryText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry(
      IntPtr instancePtr,
      int nID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_QueueDidChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_PlaylistWillChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_ResetPlaylistEntries(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetPlaylistEntry(
      IntPtr instancePtr,
      int nID,
      int nPosition,
      string pchEntryText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry(
      IntPtr instancePtr,
      int nID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamMusicRemote_PlaylistDidChange(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamHTTP_CreateHTTPRequest(
      IntPtr instancePtr,
      uint eHTTPRequestMethod,
      string pchAbsoluteURL);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(
      IntPtr instancePtr,
      uint hRequest,
      ulong ulContextValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(
      IntPtr instancePtr,
      uint hRequest,
      uint unTimeoutSeconds);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(
      IntPtr instancePtr,
      uint hRequest,
      string pchHeaderName,
      string pchHeaderValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(
      IntPtr instancePtr,
      uint hRequest,
      string pchParamName,
      string pchParamValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SendHTTPRequest(
      IntPtr instancePtr,
      uint hRequest,
      ref ulong pCallHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(
      IntPtr instancePtr,
      uint hRequest,
      ref ulong pCallHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_DeferHTTPRequest(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(
      IntPtr instancePtr,
      uint hRequest,
      string pchHeaderName,
      ref uint unResponseHeaderSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(
      IntPtr instancePtr,
      uint hRequest,
      string pchHeaderName,
      IntPtr pHeaderValueBuffer,
      uint unBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(
      IntPtr instancePtr,
      uint hRequest,
      ref uint unBodySize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(
      IntPtr instancePtr,
      uint hRequest,
      IntPtr pBodyDataBuffer,
      uint unBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(
      IntPtr instancePtr,
      uint hRequest,
      uint cOffset,
      IntPtr pBodyDataBuffer,
      uint unBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_ReleaseHTTPRequest(
      IntPtr instancePtr,
      uint hRequest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(
      IntPtr instancePtr,
      uint hRequest,
      ref float pflPercentOut);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(
      IntPtr instancePtr,
      uint hRequest,
      string pchContentType,
      IntPtr pubBody,
      uint unBodyLen);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamHTTP_CreateCookieContainer(
      IntPtr instancePtr,
      bool bAllowResponsesToModify);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_ReleaseCookieContainer(
      IntPtr instancePtr,
      uint hCookieContainer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetCookie(
      IntPtr instancePtr,
      uint hCookieContainer,
      string pchHost,
      string pchUrl,
      string pchCookie);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(
      IntPtr instancePtr,
      uint hRequest,
      uint hCookieContainer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(
      IntPtr instancePtr,
      uint hRequest,
      string pchUserAgentInfo);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(
      IntPtr instancePtr,
      uint hRequest,
      bool bRequireVerifiedCertificate);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(
      IntPtr instancePtr,
      uint hRequest,
      uint unMilliseconds);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(
      IntPtr instancePtr,
      uint hRequest,
      ref bool pbWasTimedOut);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUnifiedMessages_SendMethod(
      IntPtr instancePtr,
      string pchServiceMethod,
      IntPtr pRequestBuffer,
      uint unRequestBufferSize,
      ulong unContext);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUnifiedMessages_GetMethodResponseInfo(
      IntPtr instancePtr,
      ulong hHandle,
      ref uint punResponseSize,
      ref uint peResult);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUnifiedMessages_GetMethodResponseData(
      IntPtr instancePtr,
      ulong hHandle,
      IntPtr pResponseBuffer,
      uint unResponseBufferSize,
      bool bAutoRelease);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUnifiedMessages_ReleaseMethod(
      IntPtr instancePtr,
      ulong hHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUnifiedMessages_SendNotification(
      IntPtr instancePtr,
      string pchServiceNotification,
      IntPtr pNotificationBuffer,
      uint unNotificationBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamController_Init(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamController_Shutdown(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamController_RunFrame(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamController_GetConnectedControllers(
      IntPtr instancePtr,
      [In, Out] ulong[] handlesOut);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamController_ShowBindingPanel(
      IntPtr instancePtr,
      ulong controllerHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamController_GetActionSetHandle(
      IntPtr instancePtr,
      string pszActionSetName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamController_ActivateActionSet(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong actionSetHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamController_GetCurrentActionSet(
      IntPtr instancePtr,
      ulong controllerHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamController_GetDigitalActionHandle(
      IntPtr instancePtr,
      string pszActionName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ControllerDigitalActionData_t SteamAPI_ISteamController_GetDigitalActionData(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong digitalActionHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamController_GetDigitalActionOrigins(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong digitalActionHandle,
      [In, Out] EControllerActionOrigin[] originsOut);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamController_GetAnalogActionHandle(
      IntPtr instancePtr,
      string pszActionName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ControllerAnalogActionData_t SteamAPI_ISteamController_GetAnalogActionData(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong analogActionHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamController_GetAnalogActionOrigins(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong actionSetHandle,
      ulong analogActionHandle,
      [In, Out] EControllerActionOrigin[] originsOut);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamController_StopAnalogActionMomentum(
      IntPtr instancePtr,
      ulong controllerHandle,
      ulong eAction);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamController_TriggerHapticPulse(
      IntPtr instancePtr,
      ulong controllerHandle,
      ESteamControllerPad eTargetPad,
      ushort usDurationMicroSec);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_CreateQueryUserUGCRequest(
      IntPtr instancePtr,
      uint unAccountID,
      uint eListType,
      uint eMatchingUGCType,
      uint eSortOrder,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_CreateQueryAllUGCRequest(
      IntPtr instancePtr,
      uint eQueryType,
      uint eMatchingeMatchingUGCTypeFileType,
      uint nCreatorAppID,
      uint nConsumerAppID,
      uint unPage);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest(
      IntPtr instancePtr,
      ref ulong pvecPublishedFileID,
      uint unNumPublishedFileIDs);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_SendQueryUGCRequest(
      IntPtr instancePtr,
      ulong handle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCResult(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      ref SteamUGCDetails_t pDetails);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCPreviewURL(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      string pchURL,
      uint cchURLSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCMetadata(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      string pchMetadata,
      uint cchMetadatasize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCChildren(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      ref ulong pvecPublishedFileID,
      uint cMaxEntries);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCStatistic(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      uint eStatType,
      ref uint pStatValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews(
      IntPtr instancePtr,
      ulong handle,
      uint index);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      uint previewIndex,
      string pchURLOrVideoID,
      uint cchURLSize,
      ref bool pbIsImage);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags(
      IntPtr instancePtr,
      ulong handle,
      uint index);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag(
      IntPtr instancePtr,
      ulong handle,
      uint index,
      uint keyValueTagIndex,
      string pchKey,
      uint cchKeySize,
      string pchValue,
      uint cchValueSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_ReleaseQueryUGCRequest(
      IntPtr instancePtr,
      ulong handle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_AddRequiredTag(
      IntPtr instancePtr,
      ulong handle,
      string pTagName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_AddExcludedTag(
      IntPtr instancePtr,
      ulong handle,
      string pTagName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnKeyValueTags(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnKeyValueTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnLongDescription(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnLongDescription);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnMetadata(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnMetadata);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnChildren(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnChildren);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnAdditionalPreviews(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnAdditionalPreviews);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetReturnTotalOnly(
      IntPtr instancePtr,
      ulong handle,
      bool bReturnTotalOnly);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetLanguage(
      IntPtr instancePtr,
      ulong handle,
      string pchLanguage);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetAllowCachedResponse(
      IntPtr instancePtr,
      ulong handle,
      uint unMaxAgeSeconds);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetCloudFileNameFilter(
      IntPtr instancePtr,
      ulong handle,
      string pMatchCloudFileName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetMatchAnyTag(
      IntPtr instancePtr,
      ulong handle,
      bool bMatchAnyTag);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetSearchText(
      IntPtr instancePtr,
      ulong handle,
      string pSearchText);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetRankedByTrendDays(
      IntPtr instancePtr,
      ulong handle,
      uint unDays);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_AddRequiredKeyValueTag(
      IntPtr instancePtr,
      ulong handle,
      string pKey,
      string pValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_RequestUGCDetails(
      IntPtr instancePtr,
      ulong nPublishedFileID,
      uint unMaxAgeSeconds);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_CreateItem(
      IntPtr instancePtr,
      uint nConsumerAppId,
      uint eFileType);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_StartItemUpdate(
      IntPtr instancePtr,
      uint nConsumerAppId,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemTitle(
      IntPtr instancePtr,
      ulong handle,
      string pchTitle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemDescription(
      IntPtr instancePtr,
      ulong handle,
      string pchDescription);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemUpdateLanguage(
      IntPtr instancePtr,
      ulong handle,
      string pchLanguage);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemMetadata(
      IntPtr instancePtr,
      ulong handle,
      string pchMetaData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemVisibility(
      IntPtr instancePtr,
      ulong handle,
      uint eVisibility);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemTags(
      IntPtr instancePtr,
      ulong updateHandle,
      ref SteamParamStringArray_t pTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemContent(
      IntPtr instancePtr,
      ulong handle,
      string pszContentFolder);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_SetItemPreview(
      IntPtr instancePtr,
      ulong handle,
      string pszPreviewFile);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_RemoveItemKeyValueTags(
      IntPtr instancePtr,
      ulong handle,
      string pchKey);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_AddItemKeyValueTag(
      IntPtr instancePtr,
      ulong handle,
      string pchKey,
      string pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_SubmitItemUpdate(
      IntPtr instancePtr,
      ulong handle,
      string pchChangeNote);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetItemUpdateProgress(
      IntPtr instancePtr,
      ulong handle,
      ref ulong punBytesProcessed,
      ref ulong punBytesTotal);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_SetUserItemVote(
      IntPtr instancePtr,
      ulong nPublishedFileID,
      bool bVoteUp);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_GetUserItemVote(
      IntPtr instancePtr,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_AddItemToFavorites(
      IntPtr instancePtr,
      uint nAppId,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_RemoveItemFromFavorites(
      IntPtr instancePtr,
      uint nAppId,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_SubscribeItem(
      IntPtr instancePtr,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamUGC_UnsubscribeItem(
      IntPtr instancePtr,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetNumSubscribedItems(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetSubscribedItems(
      IntPtr instancePtr,
      ref ulong pvecPublishedFileID,
      uint cMaxEntries);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamUGC_GetItemState(
      IntPtr instancePtr,
      ulong nPublishedFileID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetItemInstallInfo(
      IntPtr instancePtr,
      ulong nPublishedFileID,
      ref ulong punSizeOnDisk,
      string pchFolder,
      uint cchFolderSize,
      ref uint punTimeStamp);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_GetItemDownloadInfo(
      IntPtr instancePtr,
      ulong nPublishedFileID,
      ref ulong punBytesDownloaded,
      ref ulong punBytesTotal);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_DownloadItem(
      IntPtr instancePtr,
      ulong nPublishedFileID,
      bool bHighPriority);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamUGC_BInitWorkshopForGameServer(
      IntPtr instancePtr,
      uint unWorkshopDepotID,
      string pszFolder);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamUGC_SuspendDownloads(
      IntPtr instancePtr,
      bool bSuspend);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamAppList_GetNumInstalledApps(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamAppList_GetInstalledApps(
      IntPtr instancePtr,
      ref uint pvecAppID,
      uint unMaxAppIDs);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamAppList_GetAppName(
      IntPtr instancePtr,
      uint nAppID,
      string pchName,
      int cchNameMax);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamAppList_GetAppInstallDir(
      IntPtr instancePtr,
      uint nAppID,
      string pchDirectory,
      int cchNameMax);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamAppList_GetAppBuildId(IntPtr instancePtr, uint nAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_DestructISteamHTMLSurface(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTMLSurface_Init(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamHTMLSurface_Shutdown(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamHTMLSurface_CreateBrowser(
      IntPtr instancePtr,
      string pchUserAgent,
      string pchUserCSS);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_RemoveBrowser(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_LoadURL(
      IntPtr instancePtr,
      uint unBrowserHandle,
      string pchURL,
      string pchPostData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetSize(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint unWidth,
      uint unHeight);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_StopLoad(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_Reload(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_GoBack(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_GoForward(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_AddHeader(
      IntPtr instancePtr,
      uint unBrowserHandle,
      string pchKey,
      string pchValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_ExecuteJavascript(
      IntPtr instancePtr,
      uint unBrowserHandle,
      string pchScript);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_MouseUp(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint eMouseButton);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_MouseDown(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint eMouseButton);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_MouseDoubleClick(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint eMouseButton);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_MouseMove(
      IntPtr instancePtr,
      uint unBrowserHandle,
      int x,
      int y);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_MouseWheel(
      IntPtr instancePtr,
      uint unBrowserHandle,
      int nDelta);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_KeyDown(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint nNativeKeyCode,
      uint eHTMLKeyModifiers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_KeyUp(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint nNativeKeyCode,
      uint eHTMLKeyModifiers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_KeyChar(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint cUnicodeChar,
      uint eHTMLKeyModifiers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetHorizontalScroll(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint nAbsolutePixelScroll);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetVerticalScroll(
      IntPtr instancePtr,
      uint unBrowserHandle,
      uint nAbsolutePixelScroll);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetKeyFocus(
      IntPtr instancePtr,
      uint unBrowserHandle,
      bool bHasKeyFocus);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_ViewSource(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_CopyToClipboard(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_PasteFromClipboard(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_Find(
      IntPtr instancePtr,
      uint unBrowserHandle,
      string pchSearchStr,
      bool bCurrentlyInFind,
      bool bReverse);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_StopFind(
      IntPtr instancePtr,
      uint unBrowserHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_GetLinkAtPosition(
      IntPtr instancePtr,
      uint unBrowserHandle,
      int x,
      int y);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetCookie(
      IntPtr instancePtr,
      string pchHostname,
      string pchKey,
      string pchValue,
      string pchPath,
      ulong nExpires,
      bool bSecure,
      bool bHTTPOnly);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetPageScaleFactor(
      IntPtr instancePtr,
      uint unBrowserHandle,
      float flZoom,
      int nPointX,
      int nPointY);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_SetBackgroundMode(
      IntPtr instancePtr,
      uint unBrowserHandle,
      bool bBackgroundMode);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_AllowStartRequest(
      IntPtr instancePtr,
      uint unBrowserHandle,
      bool bAllowed);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_JSDialogResponse(
      IntPtr instancePtr,
      uint unBrowserHandle,
      bool bResult);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse(
      IntPtr instancePtr,
      uint unBrowserHandle,
      string pchSelectedFiles);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamInventory_GetResultStatus(
      IntPtr instancePtr,
      int resultHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GetResultItems(
      IntPtr instancePtr,
      int resultHandle,
      [In, Out] SteamItemDetails_t[] pOutItemsArray,
      ref uint punOutItemsArraySize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamInventory_GetResultTimestamp(
      IntPtr instancePtr,
      int resultHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_CheckResultSteamID(
      IntPtr instancePtr,
      int resultHandle,
      ulong steamIDExpected);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamInventory_DestroyResult(
      IntPtr instancePtr,
      int resultHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GetAllItems(
      IntPtr instancePtr,
      ref int pResultHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GetItemsByID(
      IntPtr instancePtr,
      ref int pResultHandle,
      [In, Out] ulong[] pInstanceIDs,
      uint unCountInstanceIDs);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_SerializeResult(
      IntPtr instancePtr,
      int resultHandle,
      IntPtr pOutBuffer,
      ref uint punOutBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_DeserializeResult(
      IntPtr instancePtr,
      ref int pOutResultHandle,
      IntPtr pBuffer,
      uint unBufferSize,
      bool bRESERVED_MUST_BE_FALSE);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GenerateItems(
      IntPtr instancePtr,
      ref int pResultHandle,
      [In, Out] int[] pArrayItemDefs,
      [In, Out] uint[] punArrayQuantity,
      uint unArrayLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GrantPromoItems(
      IntPtr instancePtr,
      ref int pResultHandle);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_AddPromoItem(
      IntPtr instancePtr,
      ref int pResultHandle,
      int itemDef);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_AddPromoItems(
      IntPtr instancePtr,
      ref int pResultHandle,
      [In, Out] int[] pArrayItemDefs,
      uint unArrayLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_ConsumeItem(
      IntPtr instancePtr,
      ref int pResultHandle,
      ulong itemConsume,
      uint unQuantity);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_ExchangeItems(
      IntPtr instancePtr,
      ref int pResultHandle,
      [In, Out] int[] pArrayGenerate,
      [In, Out] uint[] punArrayGenerateQuantity,
      uint unArrayGenerateLength,
      [In, Out] ulong[] pArrayDestroy,
      [In, Out] uint[] punArrayDestroyQuantity,
      uint unArrayDestroyLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_TransferItemQuantity(
      IntPtr instancePtr,
      ref int pResultHandle,
      ulong itemIdSource,
      uint unQuantity,
      ulong itemIdDest);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamInventory_SendItemDropHeartbeat(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_TriggerItemDrop(
      IntPtr instancePtr,
      ref int pResultHandle,
      int dropListDefinition);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_TradeItems(
      IntPtr instancePtr,
      ref int pResultHandle,
      ulong steamIDTradePartner,
      [In, Out] ulong[] pArrayGive,
      [In, Out] uint[] pArrayGiveQuantity,
      uint nArrayGiveLength,
      [In, Out] ulong[] pArrayGet,
      [In, Out] uint[] pArrayGetQuantity,
      uint nArrayGetLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_LoadItemDefinitions(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GetItemDefinitionIDs(
      IntPtr instancePtr,
      [In, Out] int[] pItemDefIDs,
      ref uint punItemDefIDsArraySize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamInventory_GetItemDefinitionProperty(
      IntPtr instancePtr,
      int iDefinition,
      string pchPropertyName,
      StringBuilder pchValueBuffer,
      ref uint punValueBufferSize);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamVideo_GetVideoURL(
      IntPtr instancePtr,
      uint unVideoAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamVideo_IsBroadcasting(
      IntPtr instancePtr,
      ref int pnNumViewers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_InitGameServer(
      IntPtr instancePtr,
      uint unIP,
      char usGamePort,
      char usQueryPort,
      uint unFlags,
      uint nGameAppId,
      string pchVersionString);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetProduct(
      IntPtr instancePtr,
      string pszProduct);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetGameDescription(
      IntPtr instancePtr,
      string pszGameDescription);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetModDir(
      IntPtr instancePtr,
      string pszModDir);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetDedicatedServer(
      IntPtr instancePtr,
      bool bDedicated);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_LogOn(IntPtr instancePtr, string pszToken);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_LogOnAnonymous(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_LogOff(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_BLoggedOn(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_BSecure(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServer_GetSteamID(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_WasRestartRequested(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetMaxPlayerCount(
      IntPtr instancePtr,
      int cPlayersMax);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetBotPlayerCount(
      IntPtr instancePtr,
      int cBotplayers);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetServerName(
      IntPtr instancePtr,
      string pszServerName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetMapName(
      IntPtr instancePtr,
      string pszMapName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetPasswordProtected(
      IntPtr instancePtr,
      bool bPasswordProtected);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetSpectatorPort(
      IntPtr instancePtr,
      char unSpectatorPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetSpectatorServerName(
      IntPtr instancePtr,
      string pszSpectatorServerName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_ClearAllKeyValues(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetKeyValue(
      IntPtr instancePtr,
      string pKey,
      string pValue);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetGameTags(
      IntPtr instancePtr,
      string pchGameTags);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetGameData(
      IntPtr instancePtr,
      string pchGameData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetRegion(
      IntPtr instancePtr,
      string pszRegion);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_SendUserConnectAndAuthenticate(
      IntPtr instancePtr,
      uint unIPClient,
      IntPtr pvAuthBlob,
      uint cubAuthBlobSize,
      ref ulong pSteamIDUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServer_CreateUnauthenticatedUserConnection(
      IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SendUserDisconnect(
      IntPtr instancePtr,
      ulong steamIDUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_BUpdateUserData(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchPlayerName,
      uint uScore);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamGameServer_GetAuthSessionTicket(
      IntPtr instancePtr,
      IntPtr pTicket,
      int cbMaxTicket,
      ref uint pcbTicket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamGameServer_BeginAuthSession(
      IntPtr instancePtr,
      IntPtr pAuthTicket,
      int cbAuthTicket,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_EndAuthSession(
      IntPtr instancePtr,
      ulong steamID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_CancelAuthTicket(
      IntPtr instancePtr,
      uint hAuthTicket);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamGameServer_UserHasLicenseForApp(
      IntPtr instancePtr,
      ulong steamID,
      uint appID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_RequestUserGroupStatus(
      IntPtr instancePtr,
      ulong steamIDUser,
      ulong steamIDGroup);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_GetGameplayStats(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServer_GetServerReputation(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamGameServer_GetPublicIP(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServer_HandleIncomingPacket(
      IntPtr instancePtr,
      IntPtr pData,
      int cbData,
      uint srcIP,
      char srcPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamGameServer_GetNextOutgoingPacket(
      IntPtr instancePtr,
      IntPtr pOut,
      int cbMaxOut,
      ref uint pNetAdr,
      ref char pPort);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_EnableHeartbeats(
      IntPtr instancePtr,
      bool bActive);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_SetHeartbeatInterval(
      IntPtr instancePtr,
      int iHeartbeatInterval);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamGameServer_ForceHeartbeat(IntPtr instancePtr);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServer_AssociateWithClan(
      IntPtr instancePtr,
      ulong steamIDClan);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServer_ComputeNewPlayerCompatibility(
      IntPtr instancePtr,
      ulong steamIDNewPlayer);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServerStats_RequestUserStats(
      IntPtr instancePtr,
      ulong steamIDUser);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_GetUserStat(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref int pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_GetUserStat0(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref float pData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_GetUserAchievement(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      ref bool pbAchieved);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_SetUserStat(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      int nData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_SetUserStat0(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      float fData);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName,
      float flCountThisSession,
      double dSessionLength);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_SetUserAchievement(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamGameServerStats_ClearUserAchievement(
      IntPtr instancePtr,
      ulong steamIDUser,
      string pchName);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_ISteamGameServerStats_StoreUserStats(
      IntPtr instancePtr,
      ulong steamIDUser);
  }
}
