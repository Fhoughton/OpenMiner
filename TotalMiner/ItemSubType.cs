// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemSubType
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  public enum ItemSubType : ushort
  {
    None = 0,
    Bow = 1,
    Arrow = 2,
    Shield = 4,
    Edible = 8,
    TillTool = 16, // 0x0010
    HarvestTool = 32, // 0x0020
    Grenade = 64, // 0x0040
    GrenadeLauncher = 128, // 0x0080
    Key = 256, // 0x0100
    Door = 512, // 0x0200
    RangedWeapon = 1024, // 0x0400
    BlockCanBeOpened = 2048, // 0x0800
    Leaves = 4096, // 0x1000
    Gun = 8192, // 0x2000
    RapidSwing = 16384, // 0x4000
    Potion = 32768, // 0x8000
  }
}
