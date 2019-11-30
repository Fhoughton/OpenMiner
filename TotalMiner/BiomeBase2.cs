// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BiomeBase2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal abstract class BiomeBase2 : BiomeBase
  {
    protected float temperature;
    protected BiomeBase2.TemperatureType temperatureType;
    protected ushort[,] chunkHeights;
    protected float[,] chunkTemperatures;
    protected float dryTempThreshold;
    protected float humidTempThreshold;
    protected static int[] perm3D;
    protected int maxNoise;
    protected int minHeight;
    protected float sealLevelNoise;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      if (this.chunkHeights == null)
      {
        this.chunkHeights = new ushort[this.chunkSizeX + 1, this.chunkSizeZ + 1];
        this.chunkTemperatures = new float[this.chunkSizeX + 1, this.chunkSizeZ + 1];
      }
      if (BiomeBase2.perm3D != null)
        return;
      BiomeBase2.perm3D = SimplexNoise1.GetSimplexNoisePermTable(map.Seed);
    }

    public override void InitializeForGeneralUse(GlobalPoint3D p)
    {
      base.InitializeForGeneralUse(p);
      this.noiseGlobalOffset = this.chunkGlobalOffset + new GlobalPoint3D(this.biomeParams.OffsetX, 1000000, this.biomeParams.OffsetZ);
    }

    public override void InitializeForGeneralUse(MapChunk newChunk)
    {
      base.InitializeForGeneralUse(newChunk);
      this.noiseGlobalOffset = this.chunkGlobalOffset + new GlobalPoint3D(this.biomeParams.OffsetX, 1000000, this.biomeParams.OffsetZ);
    }

    protected override void GenerateChunkCore()
    {
      int y = this.chunkGlobalOffset.Y;
      if (y > this.maxHeight)
        return;
      this.noiseGlobalOffset = this.chunkGlobalOffset + new GlobalPoint3D(this.biomeParams.OffsetX, 1000000, this.biomeParams.OffsetZ);
      BlockDataXML[] blockData = this.map.BlockData;
      int num1 = this.chunkSizeX * this.chunkSizeZ;
      short cacheID1 = -1;
      short cacheID2 = -1;
      short cacheID3 = -1;
      int cacheIndex1 = 0;
      int cacheIndex2 = 0;
      int cacheIndex3 = 0;
      try
      {
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out cacheID1, out cacheIndex1);
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out cacheID2, out cacheIndex2);
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out cacheID3, out cacheIndex3);
        byte[] numArray1 = this.map.ChunkCacheManager.Cache[(int) cacheID1];
        byte[] numArray2 = this.map.ChunkCacheManager.Cache[(int) cacheID2];
        byte[] numArray3 = this.map.ChunkCacheManager.Cache[(int) cacheID3];
        Point3D p = new Point3D();
        for (p.Z = 0; p.Z <= this.chunkSizeZ; p.Z += 2)
        {
          for (p.X = 0; p.X <= this.chunkSizeX; p.X += 2)
          {
            this.groundHeight = this.GetPlaneData(p.X, p.Z);
            this.chunkHeights[p.X, p.Z] = (ushort) this.groundHeight;
            this.chunkTemperatures[p.X, p.Z] = this.temperature;
            if (p.X > 1)
            {
              this.chunkHeights[p.X - 1, p.Z] = (ushort) ((double) ((int) this.chunkHeights[p.X - 2, p.Z] + this.groundHeight) * 0.5);
              this.chunkTemperatures[p.X - 1, p.Z] = (float) (((double) this.chunkTemperatures[p.X - 2, p.Z] + (double) this.temperature) * 0.5);
            }
            if (p.Z > 1)
            {
              this.chunkHeights[p.X, p.Z - 1] = (ushort) ((double) ((int) this.chunkHeights[p.X, p.Z - 2] + this.groundHeight) * 0.5);
              this.chunkTemperatures[p.X, p.Z - 1] = (float) (((double) this.chunkTemperatures[p.X, p.Z - 2] + (double) this.temperature) * 0.5);
              if (p.X > 1)
              {
                this.chunkHeights[p.X - 1, p.Z - 1] = (ushort) ((double) ((int) this.chunkHeights[p.X - 2, p.Z - 2] + (int) this.chunkHeights[p.X, p.Z - 2] + (int) this.chunkHeights[p.X - 2, p.Z] + this.groundHeight) * 0.25);
                this.chunkTemperatures[p.X - 1, p.Z - 1] = (float) (((double) this.chunkTemperatures[p.X - 2, p.Z - 2] + (double) this.chunkTemperatures[p.X, p.Z - 2] + (double) this.chunkTemperatures[p.X - 2, p.Z] + (double) this.temperature) * 0.25);
              }
            }
          }
        }
        for (p.Z = 0; p.Z < this.chunkSizeZ; ++p.Z)
        {
          for (p.X = 0; p.X < this.chunkSizeX; ++p.X)
          {
            this.groundHeight = (int) this.chunkHeights[p.X, p.Z];
            this.temperature = this.chunkTemperatures[p.X, p.Z];
            this.temperatureType = this.GetTemperatureType(this.temperature);
            if (this.groundHeight > (int) this.seaLevel && this.groundHeight >= this.chunkGlobalOffset.Y && (this.groundHeight < this.chunkGlobalOffset.Y + this.chunkSizeY && this.SetHeightCheck))
              this.region.HeightMap.SetHeight(this.chunkGlobalOffset.X + p.X, this.chunkGlobalOffset.Z + p.Z, (ushort) this.groundHeight, (ushort) this.groundHeight);
            int num2 = p.X + p.Z * this.chunkSizeX;
            int globalY = y;
            for (p.Y = 0; p.Y < this.chunkSizeY; ++p.Y)
            {
              this.GetBlock(p, globalY);
              ++globalY;
              if (this.getBlockResultBlockID != (byte) 0)
              {
                this.allAirBlocks = false;
                BlockDataXML blockDataXml = blockData[(int) this.getBlockResultBlockID];
                if (this.allSolidBlocks && blockDataXml.Buffer > (byte) 1)
                  this.allSolidBlocks = false;
                if (blockDataXml.Luminance > (byte) 0)
                  this.isDirtyLight = true;
              }
              else
                this.allSolidBlocks = false;
              numArray1[cacheIndex1 + num2] = this.getBlockResultBlockID;
              numArray2[cacheIndex2 + num2] = this.getBlockResultLight;
              numArray3[cacheIndex3 + num2] = this.getBlockResultAux;
              num2 += num1;
            }
          }
        }
        this.chunk.BlockData.SetStream(this.chunk, cacheID1, cacheIndex1);
        this.chunk.LightData.SetStream(this.chunk, cacheID2, cacheIndex2);
        this.chunk.AuxData.SetStream(this.chunk, cacheID3, cacheIndex3);
      }
      finally
      {
        this.map.ChunkCacheManager.DecRefCount(cacheID3, cacheIndex3);
        this.map.ChunkCacheManager.DecRefCount(cacheID2, cacheIndex2);
        this.map.ChunkCacheManager.DecRefCount(cacheID1, cacheIndex1);
      }
    }

    protected BiomeBase2.TemperatureType GetTemperatureType(float temperature)
    {
      if ((double) temperature <= (double) this.dryTempThreshold)
        return BiomeBase2.TemperatureType.Dry;
      return (double) temperature < (double) this.humidTempThreshold ? BiomeBase2.TemperatureType.Temperate : BiomeBase2.TemperatureType.Humid;
    }

    protected override void SetDefaultGroundAndBelowBlock(
      int x,
      int z,
      int globalY,
      int grassyRockChance)
    {
      if (globalY <= this.layersStart)
      {
        if (globalY == 0)
          this.getBlockResultBlockID = (byte) 29;
        else
          this.getBlockResultBlockID = (byte) BiomeBase.layers[(int) ((double) (globalY - 1) / (double) this.layerHeight)];
      }
      else if (globalY < this.groundHeight - 30)
        this.getBlockResultBlockID = (byte) 17;
      else if (globalY < this.groundHeight - 12)
        this.getBlockResultBlockID = (byte) 16;
      else if (globalY < this.groundHeight - 3)
      {
        this.getBlockResultBlockID = (byte) 15;
      }
      else
      {
        float num1 = 0.025f;
        switch (this.temperatureType)
        {
          case BiomeBase2.TemperatureType.Dry:
            if (globalY < this.groundHeight)
            {
              this.getBlockResultBlockID = (byte) 3;
              break;
            }
            if (this.random.Next(grassyRockChance) == 0)
            {
              this.getBlockResultBlockID = (byte) 157;
              break;
            }
            if ((double) this.temperature > (double) this.dryTempThreshold - (double) num1)
            {
              this.getBlockResultBlockID = this.random.NextDouble() * (double) num1 < (double) this.dryTempThreshold - (double) this.temperature ? (byte) 3 : (byte) 2;
              if (this.getBlockResultBlockID != (byte) 2 || (double) this.temperature > (double) this.dryTempThreshold - (double) num1 * 0.5)
                break;
              int num2 = this.random.Next(10);
              switch (num2)
              {
                case 0:
                  this.getBlockResultBlockID = (byte) 4;
                  return;
                case 1:
                  this.getBlockResultBlockID = (byte) 157;
                  return;
                case 2:
                  this.getBlockResultBlockID = (byte) 15;
                  return;
                default:
                  if (num2 >= 5)
                    return;
                  this.getBlockResultBlockID = (byte) 16;
                  return;
              }
            }
            else
            {
              this.getBlockResultBlockID = (byte) 3;
              break;
            }
          case BiomeBase2.TemperatureType.Humid:
            if (globalY < this.groundHeight)
            {
              this.getBlockResultBlockID = (byte) 2;
              break;
            }
            if (this.random.Next(grassyRockChance) == 0)
            {
              this.getBlockResultBlockID = (byte) 79;
              break;
            }
            this.getBlockResultBlockID = (byte) 161;
            break;
          case BiomeBase2.TemperatureType.Temperate:
            if (globalY < this.groundHeight)
            {
              this.getBlockResultBlockID = (byte) 2;
              break;
            }
            if (this.random.Next(grassyRockChance) == 0)
            {
              this.getBlockResultBlockID = (byte) 79;
              break;
            }
            if ((double) this.temperature < (double) this.dryTempThreshold + (double) num1)
            {
              this.getBlockResultBlockID = this.random.NextDouble() * (double) num1 < (double) this.dryTempThreshold + (double) num1 - (double) this.temperature ? (byte) 2 : (byte) 1;
              break;
            }
            if ((double) this.temperature > (double) this.humidTempThreshold - (double) num1)
            {
              this.getBlockResultBlockID = this.random.NextDouble() * (double) num1 < (double) this.humidTempThreshold - (double) this.temperature ? (byte) 1 : (byte) 2;
              break;
            }
            this.getBlockResultBlockID = (byte) 1;
            break;
        }
      }
    }

    protected abstract float GetTemperatureNoise(int x, int z);

    public enum TemperatureType
    {
      None,
      Dry,
      Humid,
      Temperate,
    }
  }
}
