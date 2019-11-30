// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamAPIInterop
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public class SteamAPIInterop
  {
    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_Init();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_RunCallbacks();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_RegisterCallback(IntPtr pCallback, int iCallback);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_UnregisterCallback(IntPtr pCallback);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_RegisterCallResult(IntPtr pCallback, ulong hAPICall);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_UnregisterCallResult(IntPtr pCallback, ulong hAPICall);

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamClient();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamUser();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamFriends();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamUtils();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmaking();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmakingServerListResponse();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmakingPingResponse();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmakingPlayersResponse();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmakingRulesResponse();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMatchmakingServers();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamRemoteStorage();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamUserStats();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamApps();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamNetworking();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamScreenshots();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMusic();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamMusicRemote();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamHTTP();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamUnifiedMessages();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamController();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamUGC();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAppList();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamHTMLSurface();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamInventory();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamVideo();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamGameServer();

    [DllImport("Steam_api", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamGameServerStats();
  }
}
