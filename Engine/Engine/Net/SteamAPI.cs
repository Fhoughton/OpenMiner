// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamAPI
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class SteamAPI
  {
    public const int PackSize = 8;
    public const int k_iSteamUserCallbacks = 100;
    public const int k_iSteamGameServerCallbacks = 200;
    public const int k_iSteamFriendsCallbacks = 300;
    public const int k_iSteamBillingCallbacks = 400;
    public const int k_iSteamMatchmakingCallbacks = 500;
    public const int k_iSteamContentServerCallbacks = 600;
    public const int k_iSteamUtilsCallbacks = 700;
    public const int k_iClientFriendsCallbacks = 800;
    public const int k_iClientUserCallbacks = 900;
    public const int k_iSteamAppsCallbacks = 1000;
    public const int k_iSteamUserStatsCallbacks = 1100;
    public const int k_iSteamNetworkingCallbacks = 1200;
    public const int k_iClientRemoteStorageCallbacks = 1300;
    public const int k_iClientDepotBuilderCallbacks = 1400;
    public const int k_iSteamGameServerItemsCallbacks = 1500;
    public const int k_iClientUtilsCallbacks = 1600;
    public const int k_iSteamGameCoordinatorCallbacks = 1700;
    public const int k_iSteamGameServerStatsCallbacks = 1800;
    public const int k_iSteam2AsyncCallbacks = 1900;
    public const int k_iSteamGameStatsCallbacks = 2000;
    public const int k_iClientHTTPCallbacks = 2100;
    public const int k_iClientScreenshotsCallbacks = 2200;
    public const int k_iSteamScreenshotsCallbacks = 2300;
    public const int k_iClientAudioCallbacks = 2400;
    public const int k_iClientUnifiedMessagesCallbacks = 2500;
    public const int k_iSteamStreamLauncherCallbacks = 2600;
    public const int k_iClientControllerCallbacks = 2700;
    public const int k_iSteamControllerCallbacks = 2800;
    public const int k_iClientParentalSettingsCallbacks = 2900;
    public const int k_iClientDeviceAuthCallbacks = 3000;
    public const int k_iClientNetworkDeviceManagerCallbacks = 3100;
    public const int k_iClientMusicCallbacks = 3200;
    public const int k_iClientRemoteClientManagerCallbacks = 3300;
    public const int k_iClientUGCCallbacks = 3400;
    public const int k_iSteamStreamClientCallbacks = 3500;
    public const int k_IClientProductBuilderCallbacks = 3600;
    public const int k_iClientShortcutsCallbacks = 3700;
    public const int k_iClientRemoteControlManagerCallbacks = 3800;
    public const int k_iSteamAppListCallbacks = 3900;
    public const int k_iSteamMusicCallbacks = 4000;
    public const int k_iSteamMusicRemoteCallbacks = 4100;
    public const int k_iClientVRCallbacks = 4200;
    public const int k_iClientReservedCallbacks = 4300;
    public const int k_iSteamReservedCallbacks = 4400;
    public const int k_iSteamHTMLSurfaceCallbacks = 4500;
    public const int k_iClientVideoCallbacks = 4600;
    public const int k_iClientInventoryCallbacks = 4700;
    public const int k_cchPersonaNameMax = 128;
    public const int k_cwchPersonaNameMax = 32;
    public const int k_cchMaxRichPresenceKeys = 20;
    public const int k_cchMaxRichPresenceKeyLength = 64;
    public const int k_cchMaxRichPresenceValueLength = 256;
    public const int k_cchStatNameMax = 128;
    public const int k_cchLeaderboardNameMax = 128;
    public const int k_cLeaderboardDetailsMax = 64;
    public const int k_InvalidUnifiedMessageHandle = 0;
    public const ulong k_SteamItemInstanceIDInvalid = 18446744073709551615;
    public const int k_SteamInventoryResultInvalid = -1;

    public static bool Init(uint appId)
    {
      SteamAPIInterop.SteamAPI_RestartAppIfNecessary(appId);
      return SteamAPIInterop.SteamAPI_Init();
    }

    public static void RunCallbacks()
    {
      SteamAPIInterop.SteamAPI_RunCallbacks();
    }

    public static void RegisterCallback(IntPtr pCallback, int iCallback)
    {
      SteamAPIInterop.SteamAPI_RegisterCallback(pCallback, iCallback);
    }

    public static void UnregisterCallback(IntPtr pCallback)
    {
      SteamAPIInterop.SteamAPI_UnregisterCallback(pCallback);
    }

    public static void RegisterCallResult(IntPtr pCallResult, ulong hAPICall)
    {
      SteamAPIInterop.SteamAPI_RegisterCallResult(pCallResult, hAPICall);
    }

    public static void UnregisterCallResult(IntPtr pCallback, ulong hAPICall)
    {
      SteamAPIInterop.SteamAPI_UnregisterCallResult(pCallback, hAPICall);
    }

    public static ISteamClient SteamClient()
    {
      return (ISteamClient) new CSteamClient(SteamAPIInterop.SteamClient());
    }

    public static ISteamUser SteamUser()
    {
      return (ISteamUser) new CSteamUser(SteamAPIInterop.SteamUser());
    }

    public static ISteamFriends SteamFriends()
    {
      return (ISteamFriends) new CSteamFriends(SteamAPIInterop.SteamFriends());
    }

    public static ISteamUtils SteamUtils()
    {
      return (ISteamUtils) new CSteamUtils(SteamAPIInterop.SteamUtils());
    }

    public static ISteamMatchmaking SteamMatchmaking()
    {
      return (ISteamMatchmaking) new CSteamMatchmaking(SteamAPIInterop.SteamMatchmaking());
    }

    public static ISteamMatchmakingServerListResponse SteamMatchmakingServerListResponse()
    {
      return (ISteamMatchmakingServerListResponse) new CSteamMatchmakingServerListResponse(SteamAPIInterop.SteamMatchmakingServerListResponse());
    }

    public static ISteamMatchmakingPingResponse SteamMatchmakingPingResponse()
    {
      return (ISteamMatchmakingPingResponse) new CSteamMatchmakingPingResponse(SteamAPIInterop.SteamMatchmakingPingResponse());
    }

    public static ISteamMatchmakingPlayersResponse SteamMatchmakingPlayersResponse()
    {
      return (ISteamMatchmakingPlayersResponse) new CSteamMatchmakingPlayersResponse(SteamAPIInterop.SteamMatchmakingPlayersResponse());
    }

    public static ISteamMatchmakingRulesResponse SteamMatchmakingRulesResponse()
    {
      return (ISteamMatchmakingRulesResponse) new CSteamMatchmakingRulesResponse(SteamAPIInterop.SteamMatchmakingRulesResponse());
    }

    public static ISteamMatchmakingServers SteamMatchmakingServers()
    {
      return (ISteamMatchmakingServers) new CSteamMatchmakingServers(SteamAPIInterop.SteamMatchmakingServers());
    }

    public static ISteamRemoteStorage SteamRemoteStorage()
    {
      return (ISteamRemoteStorage) new CSteamRemoteStorage(SteamAPIInterop.SteamRemoteStorage());
    }

    public static ISteamUserStats SteamUserStats()
    {
      return (ISteamUserStats) new CSteamUserStats(SteamAPIInterop.SteamUserStats());
    }

    public static ISteamApps SteamApps()
    {
      return (ISteamApps) new CSteamApps(SteamAPIInterop.SteamApps());
    }

    public static ISteamNetworking SteamNetworking()
    {
      return (ISteamNetworking) new CSteamNetworking(SteamAPIInterop.SteamNetworking());
    }

    public static ISteamScreenshots SteamScreenshots()
    {
      return (ISteamScreenshots) new CSteamScreenshots(SteamAPIInterop.SteamScreenshots());
    }

    public static ISteamMusic SteamMusic()
    {
      return (ISteamMusic) new CSteamMusic(SteamAPIInterop.SteamMusic());
    }

    public static ISteamMusicRemote SteamMusicRemote()
    {
      return (ISteamMusicRemote) new CSteamMusicRemote(SteamAPIInterop.SteamMusicRemote());
    }

    public static ISteamHTTP SteamHTTP()
    {
      return (ISteamHTTP) new CSteamHTTP(SteamAPIInterop.SteamHTTP());
    }

    public static ISteamUnifiedMessages SteamUnifiedMessages()
    {
      return (ISteamUnifiedMessages) new CSteamUnifiedMessages(SteamAPIInterop.SteamUnifiedMessages());
    }

    public static ISteamController SteamController()
    {
      return (ISteamController) new CSteamController(SteamAPIInterop.SteamController());
    }

    public static ISteamUGC SteamUGC()
    {
      return (ISteamUGC) new CSteamUGC(SteamAPIInterop.SteamUGC());
    }

    public static ISteamAppList SteamAppList()
    {
      return (ISteamAppList) new CSteamAppList(SteamAPIInterop.SteamAppList());
    }

    public static ISteamHTMLSurface SteamHTMLSurface()
    {
      return (ISteamHTMLSurface) new CSteamHTMLSurface(SteamAPIInterop.SteamHTMLSurface());
    }

    public static ISteamInventory SteamInventory()
    {
      return (ISteamInventory) new CSteamInventory(SteamAPIInterop.SteamInventory());
    }

    public static ISteamVideo SteamVideo()
    {
      return (ISteamVideo) new CSteamVideo(SteamAPIInterop.SteamVideo());
    }

    public static ISteamGameServer SteamGameServer()
    {
      return (ISteamGameServer) new CSteamGameServer(SteamAPIInterop.SteamGameServer());
    }

    public static ISteamGameServerStats SteamGameServerStats()
    {
      return (ISteamGameServerStats) new CSteamGameServerStats(SteamAPIInterop.SteamGameServerStats());
    }
  }
}
