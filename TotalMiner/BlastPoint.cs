// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlastPoint
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  internal struct BlastPoint
  {
    public GlobalPoint3D Point;
    public Vector3 Direction;
    public float Strength;
    public int Radius;
    public bool TreasureChest;
    public bool Torch;
    public bool MobSpawn;
  }
}
