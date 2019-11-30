// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ESNetSocketState
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum ESNetSocketState
  {
    k_ESNetSocketStateInvalid = 0,
    k_ESNetSocketStateConnected = 1,
    k_ESNetSocketStateInitiated = 10, // 0x0000000A
    k_ESNetSocketStateLocalCandidatesFound = 11, // 0x0000000B
    k_ESNetSocketStateReceivedRemoteCandidates = 12, // 0x0000000C
    k_ESNetSocketStateChallengeHandshake = 15, // 0x0000000F
    k_ESNetSocketStateDisconnecting = 21, // 0x00000015
    k_ESNetSocketStateLocalDisconnect = 22, // 0x00000016
    k_ESNetSocketStateTimeoutDuringConnect = 23, // 0x00000017
    k_ESNetSocketStateRemoteEndDisconnected = 24, // 0x00000018
    k_ESNetSocketStateConnectionBroken = 25, // 0x00000019
  }
}
