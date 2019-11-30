// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GameProperties
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class GameProperties
  {
    private static PcgRandom random = new PcgRandom(new Random().Next());
    public NetworkSessionType NetworkSessionType;
    public HostOrJoin HostOrJoin;
    public SaveGameFileInfo SaveGame;
    public bool IsNewMap;
    public bool UseOldGenerator;
    public GlobalPoint3D ShopPoint;
    public BiomeType BiomeType;
    public int IsRandomSeed;
    public bool[] ItemIsValid;
    public Dictionary<MapChunk, List<BlastPoint>> BlastPoints;

    public bool IsSystemMap
    {
      get
      {
        return this.SaveGame.MapType == MapType.System;
      }
    }

    public GameProperties(MapType mapType)
    {
      this.SaveGame = new SaveGameFileInfo(mapType);
    }

    public int GetNextIsRandomSeed()
    {
      lock (GameProperties.random)
      {
        GameProperties.random.Seed(this.IsRandomSeed);
        return this.IsRandomSeed = GameProperties.random.Next();
      }
    }

    public void CleanupInstanceStatics()
    {
      if (this.BlastPoints == null)
        return;
      this.BlastPoints.Clear();
    }
  }
}
