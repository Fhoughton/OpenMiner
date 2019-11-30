// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GrasslandsBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  internal class GrasslandsBiome : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<GrasslandsBiome> Pool = new StudioForge.Engine.Core.Pool<GrasslandsBiome>();
    private int maxNoise;
    private int maxNoiseOver2;
    private float noise;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
    }

    protected override void Initialize(BiomeParams biomeParams, ushort seaLevel)
    {
      base.Initialize(biomeParams, seaLevel);
      this.maxHeight = (int) seaLevel + biomeParams.MaxHeight;
      this.maxNoise = (int) ((double) (this.maxHeight - (int) seaLevel + this.maxSeaDepth) * 0.800000011920929);
      this.maxNoiseOver2 = this.maxNoise / 2;
      this.seaEffect = (int) seaLevel - this.maxSeaDepth + this.maxNoiseOver2 - this.waterSaturation;
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return this.GetPlaneData(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    protected override int GetPlaneData(int x, int z)
    {
      this.noise = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      ushort num = (ushort) ((double) this.noise * (double) this.maxNoise + (double) this.seaEffect);
      if ((int) num < (int) this.seaLevel)
      {
        num += (ushort) 18;
        if ((int) num > (int) this.seaLevel)
          num = this.seaLevel;
      }
      else if ((int) num > this.maxHeight)
        num = (ushort) this.maxHeight;
      return (int) num;
    }

    protected override void GetBlock(Point3D p, int globalY)
    {
      if (globalY > this.groundHeight)
      {
        if (globalY > (int) this.seaLevel)
        {
          this.getBlockResultBlockID = (byte) 0;
          this.getBlockResultLight = this.mapSunlight;
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
        this.SetDefaultGroundAndBelowBlock(p.X, p.Z, globalY, 200);
        this.getBlockResultLight = (byte) 0;
      }
    }

    public override Color[] GetColorTable(int height)
    {
      Color[] colorArray = new Color[height];
      int index = 0;
      Color color = new Color(20, 40, 180) * 0.6f;
      for (; index < (int) this.seaLevel - this.biomeParams.MaxSeaDepth && index < height; ++index)
        colorArray[index] = color;
      int num1 = index;
      color = new Color(20, 40, 200);
      for (; index < (int) this.seaLevel && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.5f, color, (float) (index - num1) / (float) this.biomeParams.MaxSeaDepth);
      int num2 = index;
      color = new Color(99, 133, 55);
      for (; index < this.maxHeight - 10 && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.6f, color, (float) (index - num2) / (float) (this.maxHeight - (int) this.seaLevel - 10));
      for (; index < height; ++index)
        colorArray[index] = color;
      return colorArray;
    }

    protected override void DecorateChunkCore()
    {
      this.GenerateOres(BiomeType.Grasslands, 10f, 15);
      this.GenerateCaves();
      this.TreeDecoration(this.biomeParams.TreeFrequency / 100f, this.biomeParams.TreeDensityMin, this.biomeParams.TreeDensityMax, this.maxHeight);
      this.FlowerDecoration(0.2f, 1, 3, 7, 0.4f, this.maxHeight);
      this.GrassDecoration(0.3f, 1, 3, 20, 0.2f, 20, (int) this.seaLevel + 10);
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
