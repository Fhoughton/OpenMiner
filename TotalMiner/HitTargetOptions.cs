// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.HitTargetOptions
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  internal enum HitTargetOptions
  {
    None = 0,
    Players = 1,
    Npcs = 2,
    CriticalHit = 4,
    PlayersAndNpcs = Npcs | Players, // 0x00000003
    All = PlayersAndNpcs | CriticalHit, // 0x00000007
  }
}
