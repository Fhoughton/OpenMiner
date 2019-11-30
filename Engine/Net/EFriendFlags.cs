// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EFriendFlags
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EFriendFlags
  {
    k_EFriendFlagNone = 0,
    k_EFriendFlagBlocked = 1,
    k_EFriendFlagFriendshipRequested = 2,
    k_EFriendFlagImmediate = 4,
    k_EFriendFlagClanMember = 8,
    k_EFriendFlagOnGameServer = 16, // 0x00000010
    k_EFriendFlagRequestingFriendship = 128, // 0x00000080
    k_EFriendFlagRequestingInfo = 256, // 0x00000100
    k_EFriendFlagIgnored = 512, // 0x00000200
    k_EFriendFlagIgnoredFriend = 1024, // 0x00000400
    k_EFriendFlagSuggested = 2048, // 0x00000800
    k_EFriendFlagAll = 65535, // 0x0000FFFF
  }
}
