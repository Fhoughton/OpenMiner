// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ZoneType
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  public enum ZoneType : byte
  {
    None = 0,
    Spawn = 1,
    NoEdit = 2,
    NoCombat = 4,
    Moving = 8,
    Jail = 16, // 0x10
    NoFly = 32, // 0x20
    NoMobs = 64, // 0x40
    NoEscape = 128, // 0x80
  }
}
