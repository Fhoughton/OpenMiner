// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamClient
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamClient
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint CreateSteamPipe();

    public abstract bool BReleaseSteamPipe(uint hSteamPipe);

    public abstract uint ConnectToGlobalUser(uint hSteamPipe);

    public abstract uint CreateLocalUser(ref uint phSteamPipe, uint eAccountType);

    public abstract void ReleaseUser(uint hSteamPipe, uint hUser);

    public abstract ISteamUser GetISteamUser(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamGameServer GetISteamGameServer(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract void SetLocalIPBinding(uint unIP, char usPort);

    public abstract ISteamFriends GetISteamFriends(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamUtils GetISteamUtils(uint hSteamPipe, string pchVersion);

    public abstract ISteamMatchmaking GetISteamMatchmaking(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamMatchmakingServers GetISteamMatchmakingServers(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract IntPtr GetISteamGenericInterface(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamUserStats GetISteamUserStats(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamGameServerStats GetISteamGameServerStats(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamApps GetISteamApps(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamNetworking GetISteamNetworking(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamRemoteStorage GetISteamRemoteStorage(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamScreenshots GetISteamScreenshots(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract void RunFrame();

    public abstract uint GetIPCCallCount();

    public abstract void SetWarningMessageHook(SteamWarningMessageHookDelegate hook);

    public abstract bool BShutdownIfAllPipesClosed();

    public abstract ISteamHTTP GetISteamHTTP(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamUnifiedMessages GetISteamUnifiedMessages(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamController GetISteamController(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamUGC GetISteamUGC(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamAppList GetISteamAppList(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamMusic GetISteamMusic(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamMusicRemote GetISteamMusicRemote(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamHTMLSurface GetISteamHTMLSurface(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract void Set_SteamAPI_CPostAPIResultInProcess(IntPtr func);

    public abstract void Remove_SteamAPI_CPostAPIResultInProcess(IntPtr func);

    public abstract void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr func);

    public abstract ISteamInventory GetISteamInventory(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);

    public abstract ISteamVideo GetISteamVideo(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion);
  }
}
