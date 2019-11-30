// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SemiAlpineBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  internal class SemiAlpineBiome : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<SemiAlpineBiome> Pool = new StudioForge.Engine.Core.Pool<SemiAlpineBiome>();
    private int maxNoise;
    private int maxNoiseOver2;
    private float noise;
    private int seaDirtHeight;
    private int seaBasaltHeight;
    private int seaSnowHeight;
    private int seaSnowLayerHeight;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
    }

    protected override void Initialize(BiomeParams biomeParams, ushort seaLevel)
    {
      base.Initialize(biomeParams, seaLevel);
      this.maxHeight = (int) seaLevel + biomeParams.MaxHeight;
      this.maxNoise = (int) ((double) (this.maxHeight - (int) seaLevel + this.maxSeaDepth) * 0.899999976158142);
      this.maxNoiseOver2 = this.maxNoise / 2;
      this.seaEffect = (int) seaLevel - this.maxSeaDepth + this.maxNoiseOver2 - this.waterSaturation;
      this.seaDirtHeight = (int) seaLevel + biomeParams.DirtHeight;
      this.seaBasaltHeight = (int) seaLevel + biomeParams.BasaltHeight;
      this.seaSnowHeight = (int) seaLevel + biomeParams.SnowHeight;
      this.seaSnowLayerHeight = (int) seaLevel + biomeParams.SnowLayerHeight;
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return this.GetPlaneData(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    protected override int GetPlaneData(int x, int z)
    {
      this.noise = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      return (int) (ushort) ((double) this.noise * (double) this.maxNoise + (double) this.seaEffect);
    }

    protected override void GetBlock(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      if (globalY > this.groundHeight)
      {
        if (globalY > (int) this.seaLevel)
        {
          if (globalY == this.groundHeight + 1 && globalY > this.seaSnowLayerHeight + this.random.Next(3) && globalY < this.seaSnowHeight + 10)
          {
            this.getBlockResultBlockID = (byte) 145;
            this.getBlockResultAux = this.random.Next(10) == 0 ? (byte) this.random.Next(4) : (byte) 0;
            this.getBlockResultLight = (byte) ((int) this.maxLight - (int) this.map.BlockData[145].Opacity << 4);
          }
          else
          {
            this.getBlockResultBlockID = (byte) 0;
            this.getBlockResultLight = this.mapSunlight;
          }
        }
        else
        {
          this.getBlockResultBlockID = (byte) 11;
          int num = (int) this.maxLight - ((int) this.seaLevel + 1 - globalY) * (int) this.waterOpacity;
          if (num < 0)
            num = 0;
          this.getBlockResultLight = (byte) (num << 4);
        }
      }
      else
      {
        if (globalY < (int) this.seaLevel + 3 && globalY > this.groundHeight - 2 && globalY != 0)
        {
          this.getBlockResultBlockID = (byte) 3;
        }
        else
        {
          int num = this.random.Next(5);
          if (globalY >= this.seaSnowHeight + num)
            this.getBlockResultBlockID = (byte) 144;
          else if (globalY >= this.seaBasaltHeight + num)
            this.getBlockResultBlockID = (byte) 18;
          else if (globalY >= this.seaDirtHeight + num)
            this.getBlockResultBlockID = (byte) 2;
          else
            this.SetDefaultGroundAndBelowBlock(p.X, p.Z, globalY, 120);
        }
        this.getBlockResultLight = (byte) 0;
      }
    }

    protected override void DecorateChunkCore()
    {
      this.GenerateOres(BiomeType.SemiAlphine, 10f, 40);
      this.GenerateCaves();
      this.TreeDecoration(this.biomeParams.TreeFrequency / 100f, this.biomeParams.TreeDensityMin, this.biomeParams.TreeDensityMax, this.seaDirtHeight);
      this.FlowerDecoration(0.3f, 1, 3, 7, 0.4f, this.seaDirtHeight + 4);
      this.GrassDecoration(0.4f, 1, 3, 8, 0.2f, 12, this.seaDirtHeight + 4);
      this.GenerateSurfaceMobSpawns();
    }

    protected override void DecorateChunkHostOnlyCore()
    {
      if (this.map.IsChunkPending(this.chunk))
        return;
      this.AddBlastPointsOfInterest();
      this.GenerateRares();
    }
  }
}
