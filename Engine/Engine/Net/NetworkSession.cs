// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.NetworkSession
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using StudioForge.Engine.GamerServices;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Net
{
  public class NetworkSession : INetworkSession, IDisposable
  {
    private bool isDisposed;
    private SteamCallback<P2PSessionConnectFail_t> connectFailCallback;
    private SteamCallback<P2PSessionRequest_t> sessionRequestCallback;

    private event EventHandler<GamerEventArgs> GamerJoined1;

    public event EventHandler<GamerEventArgs> GamerLeft;

    public event EventHandler<GameEventArgs> GameStarted;

    public event EventHandler<GameEventArgs> GameEnded;

    public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

    public event EventHandler<GamerEventArgs> GamerJoined
    {
      add
      {
        this.GamerJoined1 += value;
        foreach (NetworkGamer localGamer in this.LocalGamers)
          value((object) this, new GamerEventArgs(localGamer));
      }
      remove
      {
        this.GamerJoined1 -= value;
      }
    }

    private void RaiseGamerJoined(NetworkGamer gamer)
    {
      if (this.GamerJoined1 == null)
        return;
      this.GamerJoined1((object) this, new GamerEventArgs(gamer));
    }

    private void RaiseGamerLeft(NetworkGamer gamer)
    {
      if (this.GamerLeft == null)
        return;
      this.GamerLeft((object) this, new GamerEventArgs(gamer));
    }

    private void RaiseGameStarted()
    {
      if (this.GameStarted == null)
        return;
      this.GameStarted((object) this, new GameEventArgs());
    }

    private void RaiseGameEnded()
    {
      if (this.GameEnded == null)
        return;
      this.GameEnded((object) this, new GameEventArgs());
    }

    private void RaiseSessionEnded(NetworkSessionEndReason reason)
    {
      if (this.SessionEnded == null)
        return;
      this.SessionEnded((object) this, new NetworkSessionEndedEventArgs(reason));
    }

    public NetworkSessionType SessionType { get; private set; }

    public NetworkSessionState SessionState { get; private set; }

    public object SessionProperties { get; private set; }

    public NetworkGamer Host { get; private set; }

    public bool IsHost
    {
      get
      {
        return this.Host.IsLocal;
      }
    }

    public bool IsDisposed
    {
      get
      {
        return this.isDisposed;
      }
    }

    public bool AllowJoinInProgress { get; set; }

    public List<NetworkGamer> AllGamers { get; private set; }

    public List<NetworkGamer> LocalGamers { get; private set; }

    public List<NetworkGamer> RemoteGamers { get; private set; }

    public int PrivateGamerSlots { get; set; }

    public int MaxGamers { get; set; }

    public int BytesPerSecondReceived { get; private set; }

    public int BytesPerSecondSent { get; private set; }

    public TimeSpan SimulatedLatency { get; set; }

    public float SimulatedPacketLoss { get; set; }

    public NetworkSession()
    {
      this.MaxGamers = 1;
      this.PrivateGamerSlots = 1;
      this.SessionProperties = (object) new NetworkSessionProperties();
      this.connectFailCallback = new SteamCallback<P2PSessionConnectFail_t>(new SteamCallback<P2PSessionConnectFail_t>.DispatchDelegate(this.OnConnectionFail), false);
      this.sessionRequestCallback = new SteamCallback<P2PSessionRequest_t>(new SteamCallback<P2PSessionRequest_t>.DispatchDelegate(this.OnSessionRequest), false);
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
    }

    private void OnSessionRequest(P2PSessionRequest_t sessionRequestInfo)
    {
    }

    private void OnConnectionFail(P2PSessionConnectFail_t connectionFailInfo)
    {
      switch (connectionFailInfo.m_eP2PSessionError)
      {
        case 0:
          break;
        case 1:
          CoreGlobals.LogWarningMessage("Steam Warning", "The remote user isn't running the same game (appID) as you are.");
          break;
        case 2:
          CoreGlobals.LogWarningMessage("Steam Warning", "The local user doesn't own this game.");
          break;
        case 3:
          CoreGlobals.LogWarningMessage("Steam Warning", "The remote user doesn't have a connection to Steam.");
          break;
        case 4:
          CoreGlobals.LogWarningMessage("Steam Warning", "The remote user isn't responding. This could be because no physical connection could be made, or the remote end isn't calling AcceptP2PSessionWithUser()");
          break;
        default:
          CoreGlobals.LogWarningMessage("Steam Warning", "The remote user didn't answer, but we got no failure reason. Maybe you are not connected to the internet?");
          break;
      }
    }

    public void StartGame()
    {
      this.RaiseGameStarted();
    }

    public void EndGame()
    {
      this.RaiseGameEnded();
    }

    public NetworkGamer AddGamer(ulong fullId)
    {
      throw new Exception();
    }

    public void Update()
    {
    }

    public NetworkGamer FindGamerById(GamerID id)
    {
      foreach (NetworkGamer allGamer in this.AllGamers)
      {
        if (allGamer.ID == id)
          return allGamer;
      }
      return (NetworkGamer) null;
    }

    public static NetworkSession Create(
      NetworkSessionType sessionType,
      Gamer host,
      int maxGamers,
      int privateGamerSlots,
      object sessionProperties)
    {
      if (sessionType != NetworkSessionType.Local)
        SteamManager.Instance.LobbyMatchmaker.CreateLobby();
      NetworkSession networkSession = new NetworkSession();
      networkSession.SessionType = sessionType;
      networkSession.MaxGamers = maxGamers;
      networkSession.PrivateGamerSlots = privateGamerSlots;
      networkSession.SessionProperties = sessionProperties;
      List<NetworkGamer> networkGamerList1 = new List<NetworkGamer>();
      List<NetworkGamer> gamers = new List<NetworkGamer>();
      List<NetworkGamer> networkGamerList2 = new List<NetworkGamer>();
      int num1 = 0;
      Gamer gamer = host;
      NetworkGamer networkGamer1 = new NetworkGamer(gamer.ID, gamer.Gamertag);
      networkGamer1.AddGamerState(GamerStates.Local | GamerStates.Ready);
      if (num1 == 0)
      {
        networkSession.Host = networkGamer1;
        networkGamer1.AddGamerState(GamerStates.Host);
      }
      networkGamerList1.Add(networkGamer1);
      networkGamerList2.Add(networkGamer1);
      gamers.Add(networkGamer1);
      int num2 = num1 + 1;
      networkSession.LocalGamers = new List<NetworkGamer>((IEnumerable<NetworkGamer>) networkGamerList2);
      networkSession.AllGamers = new List<NetworkGamer>((IEnumerable<NetworkGamer>) networkGamerList1);
      networkSession.RemoteGamers = new List<NetworkGamer>();
      foreach (NetworkGamer networkGamer2 in networkGamerList2)
        networkGamer2.Machine = new NetworkMachine(gamers);
      return networkSession;
    }

    private static NetworkSession Create(
      NetworkSessionType sessionType,
      int maxLocalGamers,
      int maxGamers,
      int privateGamerSlots,
      NetworkSessionProperties sessionProperties,
      int hostGamer,
      bool isHost)
    {
      return (NetworkSession) null;
    }

    public static List<IAvailableNetworkSession> Find(
      NetworkSessionType sessionType,
      int maxLocalGamers,
      NetworkSessionProperties searchProperties)
    {
      return (List<IAvailableNetworkSession>) null;
    }

    public static IAsyncResult BeginJoin(
      IAvailableNetworkSession availableSession,
      AsyncCallback callback,
      object asyncState)
    {
      return (IAsyncResult) null;
    }

    public static NetworkSession EndJoin(IAsyncResult result)
    {
      return (NetworkSession) null;
    }

    public static NetworkSession Join(IAvailableNetworkSession availableSession)
    {
      return NetworkSession.EndJoin(NetworkSession.BeginJoin(availableSession, (AsyncCallback) null, (object) null));
    }
  }
}
