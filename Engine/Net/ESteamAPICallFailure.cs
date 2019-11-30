// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ESteamAPICallFailure
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum ESteamAPICallFailure
  {
    k_ESteamAPICallFailureNone = -1, // 0xFFFFFFFF
    k_ESteamAPICallFailureSteamGone = 0,
    k_ESteamAPICallFailureNetworkFailure = 1,
    k_ESteamAPICallFailureInvalidHandle = 2,
    k_ESteamAPICallFailureMismatchedCallback = 3,
  }
}
