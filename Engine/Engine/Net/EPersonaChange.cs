// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EPersonaChange
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EPersonaChange
  {
    k_EPersonaChangeName = 1,
    k_EPersonaChangeStatus = 2,
    k_EPersonaChangeComeOnline = 4,
    k_EPersonaChangeGoneOffline = 8,
    k_EPersonaChangeGamePlayed = 16, // 0x00000010
    k_EPersonaChangeGameServer = 32, // 0x00000020
    k_EPersonaChangeAvatar = 64, // 0x00000040
    k_EPersonaChangeJoinedSource = 128, // 0x00000080
    k_EPersonaChangeLeftSource = 256, // 0x00000100
    k_EPersonaChangeRelationshipChanged = 512, // 0x00000200
    k_EPersonaChangeNameFirstSet = 1024, // 0x00000400
    k_EPersonaChangeFacebookInfo = 2048, // 0x00000800
    k_EPersonaChangeNickname = 4096, // 0x00001000
    k_EPersonaChangeSteamLevel = 8192, // 0x00002000
  }
}
