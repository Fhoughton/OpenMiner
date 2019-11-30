// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.QueuedBlast
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;

namespace StudioForge.TotalMiner
{
  internal struct QueuedBlast
  {
    public GlobalPoint3D Point;
    public Item ItemID;
    public float Strength;
    public int Radius;
    public PcgRandom Random;
    public GamerID PlayerID;
    public ushort Seed;
  }
}
