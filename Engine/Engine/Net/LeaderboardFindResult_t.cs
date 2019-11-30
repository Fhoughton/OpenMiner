// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.LeaderboardFindResult_t
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [CallbackIdentity(1104)]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct LeaderboardFindResult_t
  {
    public ulong m_hSteamLeaderboard;
    public byte m_bLeaderboardFound;
  }
}
