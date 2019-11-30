// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EAppType
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EAppType
  {
    k_EAppType_DepotOnly = -2147483648, // 0x80000000
    k_EAppType_Invalid = 0,
    k_EAppType_Game = 1,
    k_EAppType_Application = 2,
    k_EAppType_Tool = 4,
    k_EAppType_Demo = 8,
    k_EAppType_Media_DEPRECATED = 16, // 0x00000010
    k_EAppType_DLC = 32, // 0x00000020
    k_EAppType_Guide = 64, // 0x00000040
    k_EAppType_Driver = 128, // 0x00000080
    k_EAppType_Config = 256, // 0x00000100
    k_EAppType_Hardware = 512, // 0x00000200
    k_EAppType_Video = 2048, // 0x00000800
    k_EAppType_Plugin = 4096, // 0x00001000
    k_EAppType_Music = 8192, // 0x00002000
    k_EAppType_Shortcut = 1073741824, // 0x40000000
  }
}
