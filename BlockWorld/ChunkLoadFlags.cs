// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.ChunkLoadFlags
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  [Flags]
  public enum ChunkLoadFlags
  {
    None = 0,
    Generate = 1,
    Decorate = 2,
    Light = 4,
    LoadMesh = 8,
  }
}
