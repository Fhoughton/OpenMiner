// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TerrainGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class TerrainGenerator : TerrainGeneratorBase
  {
    public static StudioForge.Engine.Core.Pool<TerrainGenerator> Pool = new StudioForge.Engine.Core.Pool<TerrainGenerator>();
    private const float f = 2000f;
    private MapChunk chunk;
    private MapRegion region;
    private GlobalPoint3D chunkGlobalOffset;
    private GlobalPoint3D noiseGlobalOffset;
    private int maxHeight;
    private int[] perm;
    private short blockCacheID;
    private short lightCacheID;
    private short auxCacheID;
    private int blockCacheIndex;
    private int lightCacheIndex;
    private int auxCacheIndex;
    private byte[] blockCache;
    private byte[] lightCache;
    private byte[] auxCache;
    private float[,] noiseHeight1;
    private float[,] noiseHeight2;
    private float[,] noiseHeight3;
    private float[,,] noiseDensity;
    private BiomeGenerator[] biomeGenerators;
    private bool allAirBlocks;
    private bool allSolidBlocks;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      bool flag = this.chunkSizeX != map.ChunkSize.X || this.chunkSizeY != map.ChunkSize.Y || this.chunkSizeZ != map.ChunkSize.Z;
      base.Initialize(instance, map, biomeParams);
      this.maxHeight = map.MapHeight - 50;
      if (this.perm == null)
      {
        this.perm = SimplexNoise1.GetSimplexNoisePermTable(map.Seed);
        this.biomeGenerators = new BiomeGenerator[1]
        {
          (BiomeGenerator) new TemperateBiomeGenerator()
        };
        foreach (BiomeGenerator biomeGenerator in this.biomeGenerators)
          biomeGenerator.Initialize(map, this.perm);
      }
      if (!flag)
        return;
      this.noiseHeight1 = new float[this.chunkSizeX + 1, this.chunkSizeZ + 1];
      this.noiseHeight2 = new float[this.chunkSizeX + 1, this.chunkSizeZ + 1];
      this.noiseHeight3 = new float[this.chunkSizeX + 1, this.chunkSizeZ + 1];
      this.noiseDensity = new float[this.chunkSizeX + 1, this.chunkSizeY + 1, this.chunkSizeZ + 1];
    }

    public override void InitializeForGeneralUse(GlobalPoint3D p)
    {
    }

    public override void InitializeForGeneralUse(MapChunk chunk)
    {
    }

    public override void InitializeRandom()
    {
    }

    public override void InitializeRandom(int seed)
    {
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      float heightNoise1 = this.GetHeightNoise1((float) (((double) x * 2000.0 + 1000000.0) / 2000.0), (float) (((double) z * 2000.0 + 1000000.0) / 2000.0));
      float num = (float) (map.MapHeight - 50 - (int) this.seaLevel);
      return (int) ((double) map.SeaLevel + (double) heightNoise1 * (double) num - 50.0);
    }

    public override void GenerateChunk(MapChunk chunk)
    {
      this.chunk = chunk;
      this.chunkGlobalOffset = chunk.GlobalOffset;
      if (this.chunkGlobalOffset.Y > this.maxHeight)
        return;
      this.region = chunk.Region;
      this.noiseGlobalOffset.X = 1000000;
      this.noiseGlobalOffset.Y = 0;
      this.noiseGlobalOffset.Z = 1000000;
      this.allAirBlocks = true;
      this.allSolidBlocks = true;
      this.blockCacheID = (short) -1;
      this.lightCacheID = (short) -1;
      this.auxCacheID = (short) -1;
      try
      {
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out this.blockCacheID, out this.blockCacheIndex);
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out this.lightCacheID, out this.lightCacheIndex);
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out this.auxCacheID, out this.auxCacheIndex);
        this.blockCache = this.map.ChunkCacheManager.Cache[(int) this.blockCacheID];
        this.lightCache = this.map.ChunkCacheManager.Cache[(int) this.lightCacheID];
        this.auxCache = this.map.ChunkCacheManager.Cache[(int) this.auxCacheID];
        this.GenerateNoise();
        this.GenerateBlocks();
        chunk.BlockData.SetStream(chunk, this.blockCacheID, this.blockCacheIndex);
        chunk.LightData.SetStream(chunk, this.lightCacheID, this.lightCacheIndex);
        chunk.AuxData.SetStream(chunk, this.auxCacheID, this.auxCacheIndex);
      }
      finally
      {
        if (this.allAirBlocks)
          chunk.SetChunkFlag(ChunkFlags.ChunkIsAllAir);
        else if (this.allSolidBlocks)
          chunk.SetChunkFlag(ChunkFlags.ChunkIsAllSolid);
        this.map.ChunkCacheManager.DecRefCount(this.auxCacheID, this.auxCacheIndex);
        this.map.ChunkCacheManager.DecRefCount(this.lightCacheID, this.lightCacheIndex);
        this.map.ChunkCacheManager.DecRefCount(this.blockCacheID, this.blockCacheIndex);
      }
    }

    private void GenerateNoise()
    {
      int num1 = 1;
      int num2 = 1;
      Point3D point3D = new Point3D();
      for (point3D.Z = 0; point3D.Z < this.chunkSizeZ + 1; point3D.Z += num1)
      {
        for (point3D.X = 0; point3D.X < this.chunkSizeX + 1; point3D.X += num1)
        {
          float heightNoise1 = this.GetHeightNoise1((float) (((double) (point3D.X + this.chunkGlobalOffset.X) * 2000.0 + (double) this.noiseGlobalOffset.X) / 2000.0), (float) (((double) (point3D.Z + this.chunkGlobalOffset.Z) * 2000.0 + (double) this.noiseGlobalOffset.Z) / 2000.0));
          this.noiseHeight1[point3D.X, point3D.Z] = heightNoise1;
        }
      }
      for (point3D.Z = 0; point3D.Z < this.chunkSizeZ + 1; point3D.Z += num1)
      {
        for (point3D.X = 0; point3D.X < this.chunkSizeX + 1; point3D.X += num1)
        {
          float num3 = this.noiseHeight1[point3D.X, point3D.Z];
          float x = (float) (((double) (point3D.X + this.chunkGlobalOffset.X) * 2000.0 + (double) this.noiseGlobalOffset.X) / 2000.0);
          float z = (float) (((double) (point3D.Z + this.chunkGlobalOffset.Z) * 2000.0 + (double) this.noiseGlobalOffset.Z) / 2000.0);
          for (point3D.Y = 0; point3D.Y < this.chunkSizeY + 1; point3D.Y += num2)
          {
            float y = (float) (point3D.Y + this.chunkGlobalOffset.Y);
            this.noiseDensity[point3D.X, point3D.Y, point3D.Z] = this.GetDensityNoise(x, y, z, num3, num3);
          }
        }
      }
    }

    private void GenerateBlocks()
    {
      int num1 = this.chunkSizeX * this.chunkSizeZ;
      Point3D point3D = new Point3D();
      GlobalPoint3D chunkGlobalOffset = this.chunkGlobalOffset;
      Point3D offset = this.chunk.Offset;
      byte num2 = 240;
      int num3 = this.maxHeight - (int) this.seaLevel;
      float num4 = 0.2f;
      bool flag = false;
      for (point3D.Z = 0; point3D.Z < this.chunkSizeZ; ++point3D.Z)
      {
        for (point3D.X = 0; point3D.X < this.chunkSizeX; ++point3D.X)
        {
          int num5 = (int) ((double) this.seaLevel + (double) this.noiseHeight1[point3D.X, point3D.Z] * (double) num3 - 50.0);
          int num6 = num5;
          ushort heightLocal = this.region.HeightMap.GetHeightLocal(point3D.X + offset.X, point3D.Z + offset.Z);
          int num7 = (int) heightLocal;
          point3D.Y = 0;
          chunkGlobalOffset.Y = this.chunkGlobalOffset.Y;
          int num8 = point3D.X + point3D.Z * this.chunkSizeX;
          while (point3D.Y < this.chunkSizeY)
          {
            if (chunkGlobalOffset.Y <= num6)
            {
              flag = true;
              if (chunkGlobalOffset.Y > num5)
              {
                if ((double) this.noiseDensity[point3D.X, point3D.Y, point3D.Z] >= (double) num4)
                {
                  this.blockCache[this.blockCacheIndex + num8] = (byte) 17;
                  this.lightCache[this.lightCacheIndex + num8] = (byte) 0;
                  if (chunkGlobalOffset.Y > num7)
                    num7 = chunkGlobalOffset.Y;
                  this.allAirBlocks = false;
                }
                else
                {
                  this.blockCache[this.blockCacheIndex + num8] = (byte) 0;
                  this.lightCache[this.lightCacheIndex + num8] = num2;
                  this.allSolidBlocks = false;
                }
              }
              else
              {
                this.blockCache[this.blockCacheIndex + num8] = (byte) 17;
                this.lightCache[this.lightCacheIndex + num8] = (byte) 0;
                if (chunkGlobalOffset.Y > num7)
                  num7 = chunkGlobalOffset.Y;
                this.allAirBlocks = false;
              }
            }
            else
            {
              this.blockCache[this.blockCacheIndex + num8] = (byte) 0;
              this.lightCache[this.lightCacheIndex + num8] = num2;
              this.allSolidBlocks = false;
            }
            ++point3D.Y;
            ++chunkGlobalOffset.Y;
            num8 += num1;
          }
          if (num7 > (int) heightLocal)
            this.region.HeightMap.SetHeightLocal(point3D.X + offset.X, point3D.Z + offset.Z, (ushort) num7, (ushort) num7);
          point3D.Y = 0;
          chunkGlobalOffset.Y = this.chunkGlobalOffset.Y;
          int num9 = point3D.X + point3D.Z * this.chunkSizeX;
          while (point3D.Y < this.chunkSizeY)
          {
            if (this.blockCache[this.blockCacheIndex + num9] > (byte) 0)
            {
              Block block = chunkGlobalOffset.Y != num7 ? (chunkGlobalOffset.Y <= num7 - 4 ? Block.Limestone : Block.Dirt) : Block.Grass;
              this.blockCache[this.blockCacheIndex + num9] = (byte) block;
              flag = true;
            }
            ++point3D.Y;
            ++chunkGlobalOffset.Y;
            num9 += num1;
          }
        }
      }
      if (!flag)
        return;
      this.chunk.SetChunkFlag(ChunkFlags.LightDirty);
    }

    private float GetHeightNoise1(float x, float z)
    {
      return this.biomeGenerators[0].GetHeightNoise1(x, z);
    }

    private float GetHeightNoise2(float x, float z)
    {
      return this.biomeGenerators[0].GetHeightNoise2(x, z);
    }

    private float GetHeightNoise3(float x, float z)
    {
      return this.biomeGenerators[0].GetHeightNoise3(x, z);
    }

    private float GetDensityNoise(float x, float y, float z, float n1, float n2)
    {
      return this.biomeGenerators[0].GetDensityNoise(x, y, z, n1, n2);
    }

    private void InterpolateNoise2D(float[,] noise, int hScale)
    {
      int length1 = noise.GetLength(0);
      int length2 = noise.GetLength(1);
      this.InterpolateNoise2DLayerX(noise, hScale, 0);
      for (int z = hScale; z < length2; z += hScale)
      {
        this.InterpolateNoise2DLayerX(noise, hScale, z);
        for (int index = 0; index < length1; ++index)
        {
          float num1 = noise[index, z - hScale];
          float num2 = (noise[index, z] - num1) / (float) hScale;
          float num3 = num1 + num2;
          int num4 = 1;
          while (num4 < hScale)
          {
            noise[index, z - hScale + num4] = num3;
            ++num4;
            num3 += num2;
          }
        }
      }
    }

    private void InterpolateNoise2DLayerX(float[,] noise, int hScale, int z)
    {
      int length = noise.GetLength(0);
      for (int index = hScale; index < length; index += hScale)
      {
        float num1 = noise[index - hScale, z];
        float num2 = (noise[index, z] - num1) / (float) hScale;
        float num3 = num1 + num2;
        int num4 = 1;
        while (num4 < hScale)
        {
          noise[index - hScale + num4, z] = num3;
          ++num4;
          num3 += num2;
        }
      }
    }

    private void InterpolateNoise3D(float[,,] noise, int hScale, int vScale)
    {
      int length1 = noise.GetLength(0);
      int length2 = noise.GetLength(1);
      int length3 = noise.GetLength(2);
      this.InterpolateNoise3DLayerY(noise, hScale, 0);
      for (int y = vScale; y < length2; y += vScale)
      {
        this.InterpolateNoise3DLayerY(noise, hScale, y);
        for (int index1 = 0; index1 < length3; ++index1)
        {
          for (int index2 = 0; index2 < length1; ++index2)
          {
            float num1 = noise[index2, y - vScale, index1];
            float num2 = (noise[index2, y, index1] - num1) / (float) vScale;
            float num3 = num1 + num2;
            int num4 = 1;
            while (num4 < vScale)
            {
              noise[index2, y - vScale + num4, index1] = num3;
              ++num4;
              num3 += num2;
            }
          }
        }
      }
    }

    private void InterpolateNoise3DLayerY(float[,,] noise, int hScale, int y)
    {
      int length1 = noise.GetLength(0);
      int length2 = noise.GetLength(1);
      this.InterpolateNoise2DLayerX(noise, hScale, y, 0);
      for (int z = hScale; z < length2; z += hScale)
      {
        this.InterpolateNoise2DLayerX(noise, hScale, y, z);
        for (int index = 0; index < length1; ++index)
        {
          float num1 = noise[index, y, z - hScale];
          float num2 = (noise[index, y, z] - num1) / (float) hScale;
          float num3 = num1 + num2;
          int num4 = 1;
          while (num4 < hScale)
          {
            noise[index, y, z - hScale + num4] = num3;
            ++num4;
            num3 += num2;
          }
        }
      }
    }

    private void InterpolateNoise2DLayerX(float[,,] noise, int hScale, int y, int z)
    {
      int length = noise.GetLength(0);
      for (int index = hScale; index < length; index += hScale)
      {
        float num1 = noise[index - hScale, y, z];
        float num2 = (noise[index, y, z] - num1) / (float) hScale;
        float num3 = num1 + num2;
        int num4 = 1;
        while (num4 < hScale)
        {
          noise[index - hScale + num4, y, z] = num3;
          ++num4;
          num3 += num2;
        }
      }
    }

    public override void GenerateToHeightMap(
      HeightField map,
      BiomeParams biomeParams,
      ushort seaLevel,
      int seed,
      int xStep,
      int zStep,
      Action<int> rowGenerated)
    {
    }

    public override Color[] GetColorTable(int height)
    {
      return (Color[]) null;
    }

    public override void DecorateChunk(MapChunk chunk)
    {
    }
  }
}
