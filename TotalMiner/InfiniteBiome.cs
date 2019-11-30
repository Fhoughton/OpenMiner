// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.InfiniteBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal class InfiniteBiome : BiomeBase2
  {
    public static StudioForge.Engine.Core.Pool<InfiniteBiome> Pool = new StudioForge.Engine.Core.Pool<InfiniteBiome>();
    private int[] temperatureCounts = new int[3];
    private int snowHeight;
    private int snowLayerHeight;
    private int basaltHeight;
    private int lowlandsHeight;
    private float dryTempSmoothFac;
    private float humidTempSmoothFac;
    protected static int[] perm2;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.seaLevel = map.SeaLevel;
      this.sealLevelNoise = 0.25f;
      this.maxHeight = map.MapHeight - 50;
      this.minHeight = (int) ((double) ((int) this.seaLevel + 1) - (double) (this.maxHeight - (int) this.seaLevel) * ((double) this.sealLevelNoise * 10.0 / ((1.0 - (double) this.sealLevelNoise) * 10.0)));
      this.maxNoise = this.maxHeight - this.minHeight;
      this.snowHeight = this.maxHeight - 120;
      this.basaltHeight = this.snowHeight - 50;
      this.snowLayerHeight = this.basaltHeight + 15;
      this.lowlandsHeight = this.basaltHeight - 20;
      this.dryTempThreshold = 0.35f;
      this.humidTempThreshold = 0.8f;
      this.dryTempSmoothFac = 1f / this.dryTempThreshold;
      this.humidTempSmoothFac = (float) (1.0 / (1.0 - (double) this.humidTempThreshold));
      if (InfiniteBiome.perm2 != null)
        return;
      InfiniteBiome.perm2 = SimplexNoise1.GetSimplexNoisePermTable(map.Seed / 3);
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return this.GetPlaneData(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    protected override int GetPlaneData(int x, int z)
    {
      float num1 = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      float num2 = this.sealLevelNoise + (float) (1.0 / (double) this.maxNoise * 3.0);
      if ((double) num1 > (double) num2)
      {
        float num3 = (num1 - num2) / (1f - num2);
        num1 = num3 * num3 * (1f - num2) + num2;
      }
      return (int) (ushort) ((double) num1 * (double) this.maxNoise + (double) this.minHeight);
    }

    private new float GetBlockNoise(int x, int z)
    {
      float num1 = 2700f;
      float num2 = 195f;
      float num3 = 23f;
      this.temperature = this.GetTemperatureNoise(x, z);
      this.temperatureType = BiomeBase2.TemperatureType.Temperate;
      float num4 = SimplexNoise1.noise((float) x / num1, (float) z / num1, BiomeBase.perm);
      float num5 = SimplexNoise1.noise((float) x / num2, (float) z / num2, BiomeBase.perm);
      float num6 = SimplexNoise1.noise((float) x / num3, (float) z / num3, BiomeBase.perm);
      float num7 = 10f;
      float num8 = 1.5f;
      float num9 = 0.1f;
      if ((double) this.temperature <= (double) this.dryTempThreshold)
      {
        float num10 = this.dryTempThreshold - this.dryTempThreshold * 0.05f;
        if ((double) this.temperature <= (double) num10)
        {
          num8 = 1f;
          num9 = 0.01f;
        }
        else
        {
          float amount = (float) (((double) this.temperature - (double) num10) * (1.0 / ((double) this.dryTempThreshold * 0.0500000007450581)));
          num8 = MathHelper.Lerp(1f, num8, amount);
          num9 = MathHelper.Lerp(0.01f, num9, amount);
        }
      }
      else if ((double) this.temperature >= (double) this.humidTempThreshold)
      {
        float num10 = this.humidTempThreshold + this.humidTempThreshold * 0.05f;
        if ((double) this.temperature >= (double) num10)
        {
          num8 = 3f;
          num9 = 0.3f;
        }
        else
        {
          float amount = (float) (((double) num10 - (double) this.temperature) * (1.0 / ((double) this.humidTempThreshold * 0.0500000007450581)));
          num8 = MathHelper.Lerp(num8, 3f, amount);
          num9 = MathHelper.Lerp(num9, 0.3f, amount);
        }
      }
      if ((double) num4 > 0.699999988079071)
      {
        float amount = (float) (((double) num4 - 0.699999988079071) * 0.333299994468689);
        num8 = MathHelper.Lerp(num8, 4f, amount);
        num9 = MathHelper.Lerp(num9, 0.5f, amount);
      }
      return (float) (((double) num4 * (double) num7 + (double) num5 * (double) num8 + (double) num6 * (double) num9) / ((double) num7 + (double) num8 + (double) num9));
    }

    protected override void GetBlock(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      if (globalY > this.groundHeight)
      {
        if (globalY > (int) this.seaLevel)
        {
          if (globalY == this.groundHeight + 1 && globalY > this.snowLayerHeight + this.random.Next(3) && globalY < (int) this.seaLevel + this.snowHeight + 10)
          {
            this.getBlockResultBlockID = (byte) 145;
            this.getBlockResultAux = (byte) this.random.Next(4);
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
          if (globalY >= this.snowHeight + num)
            this.getBlockResultBlockID = (byte) 144;
          else if (globalY >= this.basaltHeight + num)
            this.getBlockResultBlockID = (byte) 18;
          else if (globalY >= this.lowlandsHeight + num)
            this.getBlockResultBlockID = (byte) 2;
          else
            this.SetDefaultGroundAndBelowBlock(p.X, p.Z, globalY, 120);
        }
        this.getBlockResultLight = (byte) 0;
      }
    }

    protected override float GetTemperatureNoise(int x, int z)
    {
      float num1 = 5000f;
      float num2 = 350f;
      return (float) (((double) SimplexNoise1.noise((float) x / num1, (float) z / num1, InfiniteBiome.perm2) * 10.0 + (double) SimplexNoise1.noise((float) x / num2, (float) z / num2, InfiniteBiome.perm2)) / 11.0);
    }

    protected override void DecorateChunkCore()
    {
      this.noiseGlobalOffset = this.chunkGlobalOffset + new GlobalPoint3D(this.biomeParams.OffsetX, 1000000, this.biomeParams.OffsetZ);
      this.CalcMostCommonTemperature();
      this.GenerateOres(this.temperatureType == BiomeBase2.TemperatureType.Dry ? BiomeType.Desert : BiomeType.SemiAlphine, 4f, 40);
      switch (this.temperatureType)
      {
        case BiomeBase2.TemperatureType.Dry:
          this.TreeDecoration(0.125f, 1, 4, this.lowlandsHeight + 4);
          break;
        case BiomeBase2.TemperatureType.Humid:
          this.TreeDecoration(1f, 35, 60, this.basaltHeight + 6);
          this.FlowerDecoration(0.7f, 3, 5, 6, 0.4f, this.lowlandsHeight + 8);
          this.GrassDecoration(1f, 3, 8, 6, 0.7f, 15, this.lowlandsHeight + 14);
          if (this.random.NextDouble() <= 0.5)
            break;
          this.AddTreeSoundBlock();
          break;
        case BiomeBase2.TemperatureType.Temperate:
          this.TreeDecoration(0.4f, 1, 20, this.basaltHeight + 4);
          this.FlowerDecoration(0.3f, 1, 3, 7, 0.4f, this.lowlandsHeight + 8);
          this.GrassDecoration(0.4f, 1, 3, 8, 0.2f, 20, this.lowlandsHeight + 14);
          break;
      }
    }

    protected override void DecorateChunkHostOnlyCore()
    {
    }

    private void AddTreeSoundBlock()
    {
      int groundHeightGlobal = this.GetGroundHeightGlobal((Map) this.map, this.chunkSizeX / 2 + this.chunkGlobalOffset.X, this.chunkSizeZ / 2 + this.chunkGlobalOffset.Z);
      GlobalPoint3D globalPoint3D = new GlobalPoint3D()
      {
        X = this.chunk.Offset.X + this.chunkSizeX / 2,
        Z = this.chunk.Offset.X + this.chunkSizeX / 2,
        Y = groundHeightGlobal - 3
      };
    }

    private int GetRandomForestSoundID()
    {
      double num = this.random.NextDouble();
      if (num < 0.2)
        return 9;
      return num < 0.4 ? 1 : 6;
    }

    protected override bool IsCorrectBlockForTreeBase(Block blockID)
    {
      if (this.temperatureType == BiomeBase2.TemperatureType.Dry)
        return blockID == Block.Sand;
      return BlockData.IsGrassOrDirt(blockID);
    }

    protected override ModelPlacement AddTree(
      GameInstance instance,
      Map map,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method)
    {
      if (this.temperatureType == BiomeBase2.TemperatureType.Dry)
        return VegetationGenerator.AddCactus(instance, map, p, random, UpdateBlockMethod.Generation);
      return base.AddTree(instance, map, p, random, method);
    }

    private void CalcMostCommonTemperature()
    {
      for (int index = 0; index < this.temperatureCounts.Length; ++index)
        this.temperatureCounts[index] = 0;
      Point3D point3D = new Point3D();
      for (point3D.Z = 0; point3D.Z < this.chunkSizeZ; point3D.Z += 4)
      {
        for (point3D.X = 0; point3D.X < this.chunkSizeX; point3D.X += 4)
        {
          double temperatureNoise = (double) this.GetTemperatureNoise(point3D.X + this.noiseGlobalOffset.X, point3D.Z + this.noiseGlobalOffset.Z);
          int index = 2;
          if ((double) this.temperature < 0.330000013113022)
            index = 0;
          else if ((double) this.temperature < 0.660000026226044)
            index = 1;
          ++this.temperatureCounts[index];
        }
      }
      if (this.temperatureCounts[0] > this.temperatureCounts[1])
      {
        if (this.temperatureCounts[0] > this.temperatureCounts[2])
        {
          this.temperature = 0.0f;
          this.temperatureType = BiomeBase2.TemperatureType.Dry;
        }
        else
        {
          this.temperature = 1f;
          this.temperatureType = BiomeBase2.TemperatureType.Humid;
        }
      }
      else if (this.temperatureCounts[1] > this.temperatureCounts[2])
      {
        this.temperature = 0.5f;
        this.temperatureType = BiomeBase2.TemperatureType.Temperate;
      }
      else
      {
        this.temperature = 1f;
        this.temperatureType = BiomeBase2.TemperatureType.Humid;
      }
    }
  }
}
