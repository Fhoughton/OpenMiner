// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamNetworking
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamNetworking
  {
    public abstract IntPtr GetIntPtr();

    public abstract bool SendP2PPacket(
      ulong steamIDRemote,
      byte[] pubData,
      uint cubData,
      EP2PSend eP2PSendType,
      int nChannel);

    public abstract bool IsP2PPacketAvailable(out uint pcubMsgSize, int nChannel);

    public abstract bool ReadP2PPacket(
      byte[] pubDest,
      uint cubDest,
      out uint pcubMsgSize,
      out ulong psteamIDRemote,
      int nChannel);

    public abstract bool AcceptP2PSessionWithUser(ulong steamIDRemote);

    public abstract bool CloseP2PSessionWithUser(ulong steamIDRemote);

    public abstract bool CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel);

    public abstract bool GetP2PSessionState(
      ulong steamIDRemote,
      ref P2PSessionState_t pConnectionState);

    public abstract bool AllowP2PPacketRelay(bool bAllow);

    public abstract uint CreateListenSocket(
      int nVirtualP2PPort,
      uint nIP,
      char nPort,
      bool bAllowUseOfPacketRelay);

    public abstract uint CreateP2PConnectionSocket(
      ulong steamIDTarget,
      int nVirtualPort,
      int nTimeoutSec,
      bool bAllowUseOfPacketRelay);

    public abstract uint CreateConnectionSocket(uint nIP, char nPort, int nTimeoutSec);

    public abstract bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd);

    public abstract bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd);

    public abstract bool SendDataOnSocket(
      uint hSocket,
      IntPtr pubData,
      uint cubData,
      bool bReliable);

    public abstract bool IsDataAvailableOnSocket(uint hSocket, ref uint pcubMsgSize);

    public abstract bool RetrieveDataFromSocket(
      uint hSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize);

    public abstract bool IsDataAvailable(
      uint hListenSocket,
      ref uint pcubMsgSize,
      ref uint phSocket);

    public abstract bool RetrieveData(
      uint hListenSocket,
      IntPtr pubDest,
      uint cubDest,
      ref uint pcubMsgSize,
      ref uint phSocket);

    public abstract bool GetSocketInfo(
      uint hSocket,
      ref ulong pSteamIDRemote,
      ref int peSocketStatus,
      ref uint punIPRemote,
      ref char punPortRemote);

    public abstract bool GetListenSocketInfo(uint hListenSocket, ref uint pnIP, ref char pnPort);

    public abstract uint GetSocketConnectionType(uint hSocket);

    public abstract int GetMaxPacketSize(uint hSocket);
  }
}
