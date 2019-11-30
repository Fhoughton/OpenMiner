// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.PlayerStateDataToSend
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner.Net
{
  [Flags]
  internal enum PlayerStateDataToSend : byte
  {
    None = 0,
    HotBar = 1,
    RefreshItemModels = 2,
    IsFlying = 4,
    IceEffectActive = 8,
    PositionReset = 16, // 0x10
    FootSound = 32, // 0x20
  }
}
