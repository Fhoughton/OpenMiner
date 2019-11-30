// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DesertBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal class DesertBiome : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<DesertBiome> Pool = new StudioForge.Engine.Core.Pool<DesertBiome>();
    private int[] wallHeight = new int[7]
    {
      3,
      6,
      12,
      22,
      29,
      32,
      34
    };
    private ushort maxNoise;
    private ushort maxNoiseOver2;
    private float noise;
    private int wallHeightLength;
    private ushort columnBsseHeight;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
    }

    protected override void Initialize(BiomeParams biomeParams, ushort seaLevel)
    {
      base.Initialize(biomeParams, seaLevel);
      this.maxHeight = (int) seaLevel + biomeParams.MaxHeight;
      this.maxNoise = (ushort) ((double) (this.maxHeight - (int) seaLevel + this.maxSeaDepth) * 0.699999988079071);
      this.maxNoiseOver2 = (ushort) ((uint) this.maxNoise / 2U);
      this.columnBsseHeight = (ushort) ((uint) seaLevel + 13U);
      this.wallHeightLength = this.wallHeight.Length;
      this.seaEffect = (int) seaLevel - this.maxSeaDepth + (int) this.maxNoiseOver2 - this.waterSaturation;
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return this.GetPlaneData(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    protected override int GetPlaneData(int x, int z)
    {
      this.noise = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      int num1 = (int) ((double) this.noise * (double) this.maxNoise + (double) this.seaEffect);
      if (num1 < (int) this.seaLevel)
      {
        num1 += 16;
        if (num1 > (int) this.seaLevel)
          num1 = (int) this.seaLevel;
      }
      else if (num1 > (int) this.columnBsseHeight)
      {
        int num2 = num1 - (int) this.columnBsseHeight - 1;
        bool flag = num2 < this.wallHeightLength;
        int num3 = this.wallHeight[flag ? num2 : this.wallHeightLength - 1];
        if (!flag)
          num3 += num2 - this.wallHeightLength + 1;
        num1 = (int) this.columnBsseHeight + num3;
        if (this.random.Next(10) == 0)
          num1 += this.random.Next(3) - 1;
      }
      else if (num1 > this.maxHeight)
        num1 = this.maxHeight;
      return (int) (ushort) num1;
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
        if (globalY > (int) this.seaLevel - 8 && globalY != 0)
        {
          if (globalY > (int) this.columnBsseHeight && globalY < this.maxHeight)
          {
            int num1 = this.random.Next(2) + 2;
            if (globalY > (int) this.columnBsseHeight + 2 + num1)
            {
              int num2;
              switch (this.random.Next(30))
              {
                case 0:
                  num2 = 3;
                  break;
                case 1:
                  num2 = 15;
                  break;
                default:
                  num2 = 16;
                  break;
              }
              this.getBlockResultBlockID = (byte) num2;
            }
            else
            {
              int num2;
              switch (this.random.Next(20))
              {
                case 0:
                  num2 = 3;
                  break;
                case 1:
                  num2 = 16;
                  break;
                default:
                  num2 = 15;
                  break;
              }
              this.getBlockResultBlockID = (byte) num2;
            }
          }
          else
            this.getBlockResultBlockID = (byte) 3;
        }
        else
          this.SetDefaultGroundAndBelowBlock(p.X, p.Z, globalY, 200);
        this.getBlockResultLight = (byte) 0;
      }
    }

    public override Color[] GetColorTable(int height)
    {
      Color[] colorArray = new Color[height];
      int index = 0;
      int num1 = 0;
      Color color = new Color(20, 40, 180) * 0.6f;
      for (; index < (int) this.seaLevel - this.biomeParams.MaxSeaDepth && index < height; ++index)
        colorArray[index] = color;
      int num2 = index;
      color = new Color(20, 40, 180);
      for (; index < (int) this.seaLevel && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.6f, color, (float) (index - num2) / (float) this.biomeParams.MaxSeaDepth);
      int num3 = index;
      color = new Color(225, 205, 130);
      for (; index <= (int) this.columnBsseHeight && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.8f, color, (float) ((index - num3) / ((int) this.columnBsseHeight - (int) this.seaLevel + 1)));
      int num4 = index;
      color = new Color(179, 153, 106);
      for (; index <= (int) this.columnBsseHeight + 3 && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.9f, color, (float) (index - num4) / 3f);
      int num5 = index;
      color = new Color(190, 136, 90);
      for (; index < this.maxHeight && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.8f, color, (float) (index - num5) / 3f);
      num1 = index;
      color = new Color(225, 205, 130);
      for (; index < height; ++index)
        colorArray[index] = color;
      return colorArray;
    }

    protected override void DecorateChunkCore()
    {
      this.GenerateOres(BiomeType.Desert, 10f, 15);
      this.GenerateCaves();
      this.TreeDecoration(this.biomeParams.TreeFrequency / 100f, this.biomeParams.TreeDensityMin, this.biomeParams.TreeDensityMax, this.maxHeight);
      this.GenerateDirtAndSaplings();
      this.GenerateSurfaceMobSpawns();
    }

    protected override void DecorateChunkHostOnlyCore()
    {
      if (this.map.IsChunkPending(this.chunk))
        return;
      this.AddBlastPointsOfInterest();
      this.GenerateRares();
    }

    private void GenerateDirtAndSaplings()
    {
      if (this.chunkGlobalOffset.Y > (int) this.seaLevel || this.chunkGlobalOffset.Y + this.chunkSizeY <= (int) this.seaLevel)
        return;
      GlobalPoint3D p = new GlobalPoint3D();
      for (int index = 0; index < 40; ++index)
      {
        int num1 = this.random.Next(this.chunkSizeX - 2) + 1;
        int num2 = this.random.Next(this.chunkSizeZ - 2) + 1;
        p.X = this.chunkGlobalOffset.X + num1;
        p.Z = this.chunkGlobalOffset.Z + num2;
        p.Y = (int) this.map.GetHeight(p);
        if (p.Y <= (int) this.seaLevel && this.map.GetBlockID(p) == (byte) 11)
        {
          --p.X;
          this.AddDirtAndSapling(p);
          p.X += 2;
          this.AddDirtAndSapling(p);
          --p.X;
          --p.Z;
          this.AddDirtAndSapling(p);
          p.Z += 2;
          this.AddDirtAndSapling(p);
        }
      }
    }

    private void AddDirtAndSapling(GlobalPoint3D p)
    {
      if (this.map.GetBlockID(p) == (byte) 11)
        return;
      this.map.SetBlockData(p, (byte) 2, (byte) 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
      ++p.Y;
      this.map.SetBlockData(p, (byte) 58, (byte) 4, UpdateBlockMethod.Generation, GamerID.Sys1, false);
    }

    protected override void OnOreDeposited(ref GlobalPoint3D p, byte blockID)
    {
      if (blockID != (byte) 2)
        return;
      GlobalPoint3D p1 = p;
      ++p1.Y;
      if (this.map.GetBlockID(p1) != (byte) 0)
        return;
      byte blockID1 = this.random.Next(3) == 0 ? (byte) 223 : (byte) 112;
      this.map.SetBlockData(p1, blockID1, (byte) 4, UpdateBlockMethod.Generation, GamerID.Sys1, false);
    }

    protected override bool IsCorrectBlockForTreeBase(Block blockID)
    {
      return blockID == Block.Sand;
    }

    protected override ModelPlacement AddTree(
      GameInstance instance,
      Map map,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method)
    {
      return VegetationGenerator.AddCactus(instance, map, p, random, UpdateBlockMethod.Generation);
    }

    private void AddSandstone()
    {
      if (this.random.Next(5) != 0)
        return;
      int num = this.random.Next(3, 10);
      GlobalPoint3D p = new GlobalPoint3D();
      for (int index = 0; index < num; ++index)
      {
        p.X = this.random.Next(this.chunkSizeX) + this.chunkGlobalOffset.X;
        p.Z = this.random.Next(this.chunkSizeZ) + this.chunkGlobalOffset.Z;
        p.Y = (int) this.region.HeightMap.GetHeight(p.X, p.Y);
        Point3D localPoint = this.chunk.GetLocalPoint(p);
        if (localPoint.Y >= 0 && localPoint.Y < this.chunkSizeY)
        {
          int mapIndex = this.chunk.GetMapIndex(localPoint);
          if (this.chunk.BlockData.GetData(this.chunk, mapIndex) == (byte) 3)
          {
            this.random.Next(3);
            this.chunk.BlockData.SetData(this.chunk, mapIndex, (byte) 157);
          }
        }
      }
    }
  }
}
