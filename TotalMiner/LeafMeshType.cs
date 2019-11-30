// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.LeafMeshType
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  internal enum LeafMeshType : ushort
  {
    None = 0,
    Above = 1,
    Below = 2,
    Sides = 4,
    AboveAndBelow = Below | Above, // 0x0003
    SidesAndBelow = Sides | Below, // 0x0006
    All = SidesAndBelow | Above, // 0x0007
  }
}
