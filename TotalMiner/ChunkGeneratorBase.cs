// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkGeneratorBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal abstract class ChunkGeneratorBase : IThreadWorkItem
  {
    protected MapRegion region;
    protected MapChunk chunk;
    protected TerrainGeneratorBase biome;
    protected int poolHandle;
    protected int biomeHandle;
    protected BiomeType biomeType;

    public abstract string Name { get; }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return false;
      }
    }

    public void Initialize(
      int poolHandle,
      GameInstance instance,
      BiomeType biomeType,
      MapChunk chunk)
    {
      this.poolHandle = poolHandle;
      this.chunk = chunk;
      this.region = chunk.Region;
      this.biomeType = biomeType;
      this.biome = this.GetBiome();
      this.biome.Initialize(instance, this.region.Map as MapTM, Globals2.GameProperties.SaveGame.Header.BiomeParams);
    }

    public void Update()
    {
      try
      {
        this.UpdateCore();
      }
      finally
      {
        this.ReleaseBiome();
      }
    }

    protected virtual void UpdateCore()
    {
    }

    private TerrainGeneratorBase GetBiome()
    {
      MapTM.GetBiome(this.biomeType, out this.biome, out this.biomeHandle);
      return this.biome;
    }

    private void ReleaseBiome()
    {
      MapTM.ReleaseBiome(this.biomeType, this.biome, this.biomeHandle);
    }
  }
}
