// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EItemState
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EItemState
  {
    k_EItemStateNone = 0,
    k_EItemStateSubscribed = 1,
    k_EItemStateLegacyItem = 2,
    k_EItemStateInstalled = 4,
    k_EItemStateNeedsUpdate = 8,
    k_EItemStateDownloading = 16, // 0x00000010
    k_EItemStateDownloadPending = 32, // 0x00000020
  }
}
