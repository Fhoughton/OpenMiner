// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlastResult
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public struct BlastResult
  {
    public int LowestY;
    public List<GlobalPoint3D> PointsCleared;
    public bool BuildPointsOnly;
  }
}
