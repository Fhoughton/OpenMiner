// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMNetworkManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.API
{
  public interface ITMNetworkManager
  {
    /// <summary>
    /// Returns the current Network Session or null if no session connected.
    /// </summary>
    INetworkSession Session { get; }

    /// <summary>
    /// Retrieves a list of network sessions available to join.
    /// </summary>
    /// <param name="match">Matchmaking properties.</param>
    /// <returns>List of network sessions available to join.</returns>
    List<IAvailableNetworkSession> FindSessions(SessionMatching match);

    /// <summary>
    /// Called by a client to join an existing network session.
    /// </summary>
    /// <param name="session">The available session to join.</param>
    /// <param name="joiner">The gamer joining the session.</param>
    /// <returns>A connected network session or null if coould not join.</returns>
    INetworkSession JoinSession(IAvailableNetworkSession session, Gamer joiner);

    /// <summary>
    /// Called by the Host Gamer to create a new network session for others to join.
    /// </summary>
    /// <param name="type">Type of network session.</param>
    /// <param name="host">The gamer who is the host of the session (session creator).</param>
    /// <param name="properties">Properties describing the session.</param>
    /// <returns>A connected session or null if it could not be created.</returns>
    INetworkSession CreateSession(
      NetworkSessionType type,
      Gamer host,
      SessionProperties properties);

    /// <summary>
    /// Called if the Host ends the current network session or the session is disconnected.
    /// </summary>
    void EndSession();

    /// <summary>
    /// Send a packet to a remote gamer. If recipient is null, send the packet to all remote gamers.
    /// </summary>
    /// <param name="data">Packet to send.</param>
    /// <param name="options">Send options.</param>
    /// <param name="recipient">Gamer to receive the packet. If null all remote gamers in the session receive the packet.</param>
    void SendData(PacketWriter data, SendDataOptions options, NetworkGamer recipient);

    /// <summary>
    /// Fill a PacketReader with packet data intended for the local gamer.
    /// </summary>
    /// <param name="data">The PacketReader stream to be filled with the packet data intended for the local gamer.</param>
    /// <param name="sender">The gamer who sent the packet.</param>
    /// <returns>True if there is still packets to read (from other senders). False if no more packets to read.</returns>
    bool ReadData(PacketReader data, out NetworkGamer sender);

    /// <summary>Allows Mods to read and parse custom packets.</summary>
    /// <param name="data"></param>
    /// <param name="sender">The gamer who sent the packet.</param>
    /// <returns>Returns True if the packet was processed, false if not (a different mods packet).</returns>
    bool ParseCustomPacket(PacketReader data, NetworkGamer sender);
  }
}
