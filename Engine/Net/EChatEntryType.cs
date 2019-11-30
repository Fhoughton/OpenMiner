// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EChatEntryType
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EChatEntryType
  {
    k_EChatEntryTypeInvalid = 0,
    k_EChatEntryTypeChatMsg = 1,
    k_EChatEntryTypeTyping = 2,
    k_EChatEntryTypeInviteGame = 3,
    k_EChatEntryTypeEmote = 4,
    k_EChatEntryTypeLeftConversation = 6,
    k_EChatEntryTypeEntered = 7,
    k_EChatEntryTypeWasKicked = 8,
    k_EChatEntryTypeWasBanned = 9,
    k_EChatEntryTypeDisconnected = 10, // 0x0000000A
    k_EChatEntryTypeHistoricalChat = 11, // 0x0000000B
    k_EChatEntryTypeReserved1 = 12, // 0x0000000C
    k_EChatEntryTypeReserved2 = 13, // 0x0000000D
    k_EChatEntryTypeLinkBlocked = 14, // 0x0000000E
  }
}
