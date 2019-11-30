// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BlockTargetTypes
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner.Blocks
{
  [Flags]
  internal enum BlockTargetTypes : byte
  {
    None = 0,
    Owner = 1,
    Players = 2,
    Mobs = 4,
    Admins = 8,
    Strongest = 16, // 0x10
    Weakest = 32, // 0x20
  }
}
