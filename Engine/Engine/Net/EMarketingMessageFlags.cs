// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EMarketingMessageFlags
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EMarketingMessageFlags
  {
    k_EMarketingMessageFlagsNone = 0,
    k_EMarketingMessageFlagsHighPriority = 1,
    k_EMarketingMessageFlagsPlatformWindows = 2,
    k_EMarketingMessageFlagsPlatformMac = 4,
    k_EMarketingMessageFlagsPlatformLinux = 8,
    k_EMarketingMessageFlagsPlatformRestrictions = 14, // 0x0000000E
  }
}
