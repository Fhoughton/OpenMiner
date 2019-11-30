// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.NpcQueryPreference
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner.AI
{
  [Flags]
  public enum NpcQueryPreference : ushort
  {
    None = 0,
    Source = 1,
    Visible = 2,
    Agressive = 4,
    Closest = 8,
    Weakest = 16, // 0x0010
    Strongest = 32, // 0x0020
    LowestHP = 64, // 0x0040
    HighestHP = 128, // 0x0080
    QueryTypes = Agressive | Visible | Source, // 0x0007
  }
}
