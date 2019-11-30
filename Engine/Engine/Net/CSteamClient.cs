// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamClient
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  public class CSteamClient : ISteamClient
  {
    private IntPtr m_pSteamClient;

    public CSteamClient(IntPtr SteamClient)
    {
      this.m_pSteamClient = SteamClient;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamClient;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamClient == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint CreateSteamPipe()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamClient_CreateSteamPipe(this.m_pSteamClient);
    }

    public override bool BReleaseSteamPipe(uint hSteamPipe)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamClient_BReleaseSteamPipe(this.m_pSteamClient, hSteamPipe);
    }

    public override uint ConnectToGlobalUser(uint hSteamPipe)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamClient_ConnectToGlobalUser(this.m_pSteamClient, hSteamPipe);
    }

    public override uint CreateLocalUser(ref uint phSteamPipe, uint eAccountType)
    {
      this.CheckIfUsable();
      phSteamPipe = 0U;
      return NativeCalls.SteamAPI_ISteamClient_CreateLocalUser(this.m_pSteamClient, ref phSteamPipe, eAccountType);
    }

    public override void ReleaseUser(uint hSteamPipe, uint hUser)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_ReleaseUser(this.m_pSteamClient, hSteamPipe, hUser);
    }

    public override ISteamUser GetISteamUser(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamUser) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamUser(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamUser));
    }

    public override ISteamGameServer GetISteamGameServer(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamGameServer) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamGameServer(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamGameServer));
    }

    public override void SetLocalIPBinding(uint unIP, char usPort)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_SetLocalIPBinding(this.m_pSteamClient, unIP, usPort);
    }

    public override ISteamFriends GetISteamFriends(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamFriends) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamFriends(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamFriends));
    }

    public override ISteamUtils GetISteamUtils(uint hSteamPipe, string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamUtils) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamUtils(this.m_pSteamClient, hSteamPipe, pchVersion), typeof (ISteamUtils));
    }

    public override ISteamMatchmaking GetISteamMatchmaking(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamMatchmaking) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamMatchmaking(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamMatchmaking));
    }

    public override ISteamMatchmakingServers GetISteamMatchmakingServers(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamMatchmakingServers) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamMatchmakingServers(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamMatchmakingServers));
    }

    public override IntPtr GetISteamGenericInterface(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (IntPtr) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamGenericInterface(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (IntPtr));
    }

    public override ISteamUserStats GetISteamUserStats(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamUserStats) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamUserStats(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamUserStats));
    }

    public override ISteamGameServerStats GetISteamGameServerStats(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamGameServerStats) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamGameServerStats(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamGameServerStats));
    }

    public override ISteamApps GetISteamApps(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamApps) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamApps(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamApps));
    }

    public override ISteamNetworking GetISteamNetworking(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamNetworking) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamNetworking(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamNetworking));
    }

    public override ISteamRemoteStorage GetISteamRemoteStorage(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamRemoteStorage) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamRemoteStorage(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamRemoteStorage));
    }

    public override ISteamScreenshots GetISteamScreenshots(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamScreenshots) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamScreenshots(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamScreenshots));
    }

    public override void RunFrame()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_RunFrame(this.m_pSteamClient);
    }

    public override uint GetIPCCallCount()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamClient_GetIPCCallCount(this.m_pSteamClient);
    }

    public override void SetWarningMessageHook(SteamWarningMessageHookDelegate pFunction)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_SetWarningMessageHook(this.m_pSteamClient, Marshal.GetFunctionPointerForDelegate((Delegate) pFunction));
    }

    public override bool BShutdownIfAllPipesClosed()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamClient_BShutdownIfAllPipesClosed(this.m_pSteamClient);
    }

    public override ISteamHTTP GetISteamHTTP(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamHTTP) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamHTTP(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamHTTP));
    }

    public override ISteamUnifiedMessages GetISteamUnifiedMessages(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamUnifiedMessages) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamUnifiedMessages(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamUnifiedMessages));
    }

    public override ISteamController GetISteamController(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamController) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamController(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamController));
    }

    public override ISteamUGC GetISteamUGC(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamUGC) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamUGC(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamUGC));
    }

    public override ISteamAppList GetISteamAppList(
      uint hSteamUser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamAppList) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamAppList(this.m_pSteamClient, hSteamUser, hSteamPipe, pchVersion), typeof (ISteamAppList));
    }

    public override ISteamMusic GetISteamMusic(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamMusic) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamMusic(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamMusic));
    }

    public override ISteamMusicRemote GetISteamMusicRemote(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamMusicRemote) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamMusicRemote(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamMusicRemote));
    }

    public override ISteamHTMLSurface GetISteamHTMLSurface(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamHTMLSurface) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamHTMLSurface(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamHTMLSurface));
    }

    public override void Set_SteamAPI_CPostAPIResultInProcess(IntPtr func)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_Set_SteamAPI_CPostAPIResultInProcess(this.m_pSteamClient, func);
    }

    public override void Remove_SteamAPI_CPostAPIResultInProcess(IntPtr func)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_Remove_SteamAPI_CPostAPIResultInProcess(this.m_pSteamClient, func);
    }

    public override void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr func)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamClient_Set_SteamAPI_CCheckCallbackRegisteredInProcess(this.m_pSteamClient, func);
    }

    public override ISteamInventory GetISteamInventory(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamInventory) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamInventory(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamInventory));
    }

    public override ISteamVideo GetISteamVideo(
      uint hSteamuser,
      uint hSteamPipe,
      string pchVersion)
    {
      this.CheckIfUsable();
      return (ISteamVideo) Marshal.PtrToStructure(NativeCalls.SteamAPI_ISteamClient_GetISteamVideo(this.m_pSteamClient, hSteamuser, hSteamPipe, pchVersion), typeof (ISteamVideo));
    }
  }
}
