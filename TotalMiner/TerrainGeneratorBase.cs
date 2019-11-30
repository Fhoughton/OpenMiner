// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TerrainGeneratorBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using System;

namespace StudioForge.TotalMiner
{
  internal abstract class TerrainGeneratorBase
  {
    protected MapTM map;
    protected MapStrategyTM strategy;
    protected GameInstance instance;
    protected ushort seaLevel;
    protected int chunkSizeX;
    protected int chunkSizeY;
    protected int chunkSizeZ;
    protected BiomeParams biomeParams;
    protected RockLayerTransitionMap rockTransMap;
    protected int maxSeaDepth;
    protected int waterSaturation;
    protected float bn;
    protected float mn;
    protected float fn;
    protected float bm;
    protected float mm;
    protected float fm;
    protected float td;

    public virtual void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      this.instance = instance;
      this.map = map;
      this.rockTransMap = instance?.RockTransMap;
      this.strategy = map.MapStrategy as MapStrategyTM;
      this.chunkSizeX = map.ChunkSize.X;
      this.chunkSizeY = map.ChunkSize.Y;
      this.chunkSizeZ = map.ChunkSize.Z;
    }

    protected virtual void Initialize(BiomeParams biomeParams, ushort seaLevel)
    {
      this.biomeParams = biomeParams;
      this.seaLevel = seaLevel;
      this.bn = biomeParams.BigDetailNoise;
      this.mn = biomeParams.MediumDetailNoise;
      this.fn = biomeParams.FineDetailNoise;
      this.bm = biomeParams.BigDetailMultiplier;
      this.mm = biomeParams.MediumDetailMultiplier;
      this.fm = biomeParams.FineDetailMultiplier;
      this.td = biomeParams.TotalNoiseDivisor;
      this.maxSeaDepth = biomeParams.MaxSeaDepth;
      this.waterSaturation = biomeParams.WaterSaturation;
    }

    public abstract void GenerateChunk(MapChunk chunk);

    public abstract void DecorateChunk(MapChunk chunk);

    public abstract void InitializeForGeneralUse(GlobalPoint3D p);

    public abstract void InitializeForGeneralUse(MapChunk chunk);

    public abstract int GetGroundHeightGlobal(Map map, int x, int z);

    public abstract void InitializeRandom();

    public abstract void InitializeRandom(int seed);

    public abstract void GenerateToHeightMap(
      HeightField map,
      BiomeParams biomeParams,
      ushort seaLevel,
      int seed,
      int xStep,
      int zStep,
      Action<int> rowGenerated);

    public abstract Color[] GetColorTable(int height);
  }
}
