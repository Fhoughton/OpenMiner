// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamNetworking
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamNetworking : ISteamNetworking
  {
    private IntPtr m_pSteamNetworking;

    public CSteamNetworking(IntPtr SteamNetworking)
    {
      this.m_pSteamNetworking = SteamNetworking;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamNetworking;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamNetworking == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override bool SendP2PPacket(
      ulong steamIDRemote,
      byte[] pubData,
      uint cubData,
      EP2PSend eP2PSendType,
      int nChannel)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_SendP2PPacket(this.m_pSteamNetworking, steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
    }

    public override bool IsP2PPacketAvailable(out uint pcubMsgSize, int nChannel)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_IsP2PPacketAvailable(this.m_pSteamNetworking, out pcubMsgSize, nChannel);
    }

    public override bool ReadP2PPacket(
      byte[] pubDest,
      uint cubDest,
      out uint pcubMsgSize,
      out ulong psteamIDRemote,
      int nChannel)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_ReadP2PPacket(this.m_pSteamNetworking, pubDest, cubDest, out pcubMsgSize, out psteamIDRemote, nChannel);
    }

    public override bool AcceptP2PSessionWithUser(ulong steamIDRemote)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser(this.m_pSteamNetworking, steamIDRemote);
    }

    public override bool CloseP2PSessionWithUser(ulong steamIDRemote)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_CloseP2PSessionWithUser(this.m_pSteamNetworking, steamIDRemote);
    }

    public override bool CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_CloseP2PChannelWithUser(this.m_pSteamNetworking, steamIDRemote, nChannel);
    }

    public override bool GetP2PSessionState(
      ulong steamIDRemote,
      ref P2PSessionState_t pConnectionState)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_GetP2PSessionState(this.m_pSteamNetworking, steamIDRemote, ref pConnectionState);
    }

    public override bool AllowP2PPacketRelay(bool bAllow)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_AllowP2PPacketRelay(this.m_pSteamNetworking, bAllow);
    }

    public override uint CreateListenSocket(
      int nVirtualP2PPort,
      uint nIP,
      char nPort,
      bool bAllowUseOfPacketRelay)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_CreateListenSocket(this.m_pSteamNetworking, nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
    }

    public override uint CreateP2PConnectionSocket(
      ulong steamIDTarget,
      int nVirtualPort,
      int nTimeoutSec,
      bool bAllowUseOfPacketRelay)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_CreateP2PConnectionSocket(this.m_pSteamNetworking, steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
    }

    public override uint CreateConnectionSocket(uint nIP, char nPort, int nTimeoutSec)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_CreateConnectionSocket(this.m_pSteamNetworking, nIP, nPort, nTimeoutSec);
    }

    public override bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_DestroySocket(this.m_pSteamNetworking, hSocket, bNotifyRemoteEnd);
    }

    public override bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_DestroyListenSocket(this.m_pSteamNetworking, hSocket, bNotifyRemoteEnd);
    }

    public override bool SendDataOnSocket(
      uint hSocket,
      IntPtr pubData,
      uint cubData,
      bool bReliable)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_SendDataOnSocket(this.m_pSteamNetworking, hSocket, pubData, cubData, bReliable);
    }

    public override bool IsDataAvailableOnSocket(uint hSocket, ref uint pcubMsgSize)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_IsDataAvailableOnSocket(this.m_pSteamNetworking, hSocket, ref pcubMsgSize);
    }

    public override bool RetrieveDataFromSocket(
      uint hSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_RetrieveDataFromSocket(this.m_pSteamNetworking, hSocket, pubDest, cubDest, ref pcubMsgSize);
    }

    public override bool IsDataAvailable(
      uint hListenSocket,
      ref uint pcubMsgSize,
      ref uint phSocket)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      phSocket = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_IsDataAvailable(this.m_pSteamNetworking, hListenSocket, ref pcubMsgSize, ref phSocket);
    }

    public override bool RetrieveData(
      uint hListenSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize,
      ref uint phSocket)
    {
      this.CheckIfUsable();
      pcubMsgSize = 0U;
      phSocket = 0U;
      return NativeCalls.SteamAPI_ISteamNetworking_RetrieveData(this.m_pSteamNetworking, hListenSocket, pubDest, cubDest, ref pcubMsgSize, ref phSocket);
    }

    public override bool GetSocketInfo(
      uint hSocket,
      ref ulong pSteamIDRemote,
      ref int peSocketStatus,
      ref uint punIPRemote,
      ref char punPortRemote)
    {
      this.CheckIfUsable();
      peSocketStatus = 0;
      punIPRemote = 0U;
      punPortRemote = char.MinValue;
      return NativeCalls.SteamAPI_ISteamNetworking_GetSocketInfo(this.m_pSteamNetworking, hSocket, ref pSteamIDRemote, ref peSocketStatus, ref punIPRemote, ref punPortRemote);
    }

    public override bool GetListenSocketInfo(uint hListenSocket, ref uint pnIP, ref char pnPort)
    {
      this.CheckIfUsable();
      pnIP = 0U;
      pnPort = char.MinValue;
      return NativeCalls.SteamAPI_ISteamNetworking_GetListenSocketInfo(this.m_pSteamNetworking, hListenSocket, ref pnIP, ref pnPort);
    }

    public override uint GetSocketConnectionType(uint hSocket)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_GetSocketConnectionType(this.m_pSteamNetworking, hSocket);
    }

    public override int GetMaxPacketSize(uint hSocket)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamNetworking_GetMaxPacketSize(this.m_pSteamNetworking, hSocket);
    }
  }
}
