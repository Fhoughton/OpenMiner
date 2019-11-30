// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ParticleType
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  internal enum ParticleType : byte
  {
    None = 0,
    Loot = 1,
    Debris = 2,
    SetPower = 4,
    Projectile = 8,
  }
}
