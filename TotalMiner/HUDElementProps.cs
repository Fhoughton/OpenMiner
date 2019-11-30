// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.HUDElementProps
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  internal enum HUDElementProps : byte
  {
    None = 0,
    Vertical = 1,
    ShowLabel = 2,
    ShowNumbers = 4,
    RightJustify = 8,
    Absolute = 16, // 0x10
  }
}
