// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.INetworkSession
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using StudioForge.Engine.GamerServices;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Net
{
  public interface INetworkSession : IDisposable
  {
    event EventHandler<GamerEventArgs> GamerJoined;

    event EventHandler<GamerEventArgs> GamerLeft;

    event EventHandler<GameEventArgs> GameStarted;

    event EventHandler<GameEventArgs> GameEnded;

    event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

    bool IsDisposed { get; }

    List<NetworkGamer> AllGamers { get; }

    List<NetworkGamer> LocalGamers { get; }

    List<NetworkGamer> RemoteGamers { get; }

    NetworkGamer Host { get; }

    bool IsHost { get; }

    NetworkSessionState SessionState { get; }

    NetworkSessionType SessionType { get; }

    object SessionProperties { get; }

    NetworkGamer FindGamerById(GamerID id);

    void StartGame();

    void EndGame();

    void Update();
  }
}
