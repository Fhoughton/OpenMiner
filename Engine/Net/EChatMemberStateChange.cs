// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EChatMemberStateChange
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EChatMemberStateChange
  {
    k_EChatMemberStateChangeEntered = 1,
    k_EChatMemberStateChangeLeft = 2,
    k_EChatMemberStateChangeDisconnected = 4,
    k_EChatMemberStateChangeKicked = 8,
    k_EChatMemberStateChangeBanned = 16, // 0x00000010
  }
}
