// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BiomeBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal abstract class BiomeBase : TerrainGeneratorBase
  {
    protected static Block[] layers = new Block[14]
    {
      Block.Rhyolite,
      Block.Marble,
      Block.Komatiite,
      Block.Granite,
      Block.Gabbro,
      Block.Serpentine,
      Block.Tuff,
      Block.Diorite,
      Block.Dacite,
      Block.Andesite,
      Block.Basalt,
      Block.Limestone,
      Block.Sandstone,
      Block.Clay
    };
    protected List<MapChunk> neighbours = new List<MapChunk>(27);
    protected BoxInt chunkIntersectTestBox = new BoxInt();
    protected List<BlastPoint> blastPointsOfInterest = new List<BlastPoint>(10);
    protected static int[] perm;
    protected int maxHeight;
    protected int groundHeight;
    protected float layerHeight;
    protected int layersStart;
    protected int seaEffect;
    protected GlobalPoint3D mapBoundMin;
    protected GlobalPoint3D mapBoundMax;
    protected MapChunk chunk;
    protected MapRegion region;
    protected bool isDirtyLight;
    protected byte mapSunlight;
    protected byte maxLight;
    protected byte waterOpacity;
    protected byte getBlockResultBlockID;
    protected byte getBlockResultLight;
    protected byte getBlockResultAux;
    protected GlobalPoint3D chunkGlobalOffset;
    protected GlobalPoint3D noiseGlobalOffset;
    protected bool allAirBlocks;
    protected bool allSolidBlocks;
    protected PcgRandom random;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.mapBoundMin = map.MapBound.Min;
      this.mapBoundMax = map.MapBound.Max;
      this.maxLight = (byte) map.MaxLight;
      this.mapSunlight = (byte) ((uint) map.SunLight.SunLight << 4);
      this.layersStart = (int) map.SeaLevel - 2;
      this.layerHeight = (float) this.layersStart / (float) BiomeBase.layers.Length;
      this.waterOpacity = map.BlockData[11].Opacity;
      if (BiomeBase.perm != null)
        return;
      BiomeBase.perm = SimplexNoise1.GetSimplexNoisePermTable(map.Seed);
    }

    public static void AllBiomesReleased()
    {
      BiomeBase.perm = (int[]) null;
    }

    public override void InitializeRandom()
    {
      if (this.chunk == null)
        return;
      this.InitializeRandom(this.chunk.Seed);
    }

    public override void InitializeRandom(int seed)
    {
      if (this.random == null)
        this.random = new PcgRandom((ulong) (uint) seed, 15726070495360670683UL);
      else
        this.random.Seed((ulong) (uint) seed, 15726070495360670683UL);
    }

    public override void InitializeForGeneralUse(GlobalPoint3D p)
    {
      if (this.chunk != null && p.X >= this.chunkGlobalOffset.X && (p.X < this.chunkGlobalOffset.X + this.chunkSizeX && p.Y >= this.chunkGlobalOffset.Y) && (p.Y < this.chunkGlobalOffset.Y + this.chunkSizeY && p.Z >= this.chunkGlobalOffset.Z && p.Z < this.chunkGlobalOffset.Z + this.chunkSizeZ) || this.map == null)
        return;
      this.chunk = this.map.GetChunk(p);
      if (this.chunk == null)
        return;
      this.region = this.chunk.Region;
      this.noiseGlobalOffset = this.chunkGlobalOffset = this.chunk.GlobalOffset;
      this.noiseGlobalOffset.X += this.biomeParams.OffsetX;
      this.noiseGlobalOffset.Z += this.biomeParams.OffsetZ;
      this.InitializeRandom();
    }

    public override void InitializeForGeneralUse(MapChunk newChunk)
    {
      if (this.chunk == newChunk)
        return;
      this.chunk = newChunk;
      this.region = this.chunk.Region;
      this.noiseGlobalOffset = this.chunkGlobalOffset = this.chunk.GlobalOffset;
      this.noiseGlobalOffset.X += this.biomeParams.OffsetX;
      this.noiseGlobalOffset.Z += this.biomeParams.OffsetZ;
      this.InitializeRandom();
    }

    public override void GenerateChunk(MapChunk chunk)
    {
      this.chunk = chunk;
      this.region = chunk.Region;
      this.chunkGlobalOffset = chunk.GlobalOffset;
      this.InitializeRandom();
      this.isDirtyLight = false;
      this.allAirBlocks = true;
      this.allSolidBlocks = true;
      this.GenerateChunkCore();
      this.LightChunkAndNeighbours(ChunkLoadFlags.Generate);
      if (this.allAirBlocks)
      {
        chunk.SetChunkFlag(ChunkFlags.ChunkIsAllAir);
      }
      else
      {
        if (!this.allSolidBlocks)
          return;
        chunk.SetChunkFlag(ChunkFlags.ChunkIsAllSolid);
      }
    }

    protected virtual void PreGenerateChunkCore()
    {
    }

    protected virtual void PostGenerateChunkCore()
    {
    }

    protected virtual void GenerateChunkCore()
    {
      int y = this.chunkGlobalOffset.Y;
      if (y > this.maxHeight)
        return;
      this.noiseGlobalOffset = this.chunkGlobalOffset;
      this.noiseGlobalOffset.X += this.biomeParams.OffsetX;
      this.noiseGlobalOffset.Z += this.biomeParams.OffsetZ;
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
        this.PreGenerateChunkCore();
        Point3D p = new Point3D();
        for (p.Z = 0; p.Z < this.chunkSizeZ; ++p.Z)
        {
          for (p.X = 0; p.X < this.chunkSizeX; ++p.X)
          {
            this.groundHeight = this.GetPlaneData(p.X, p.Z);
            if (this.groundHeight > (int) this.seaLevel && this.groundHeight >= this.chunkGlobalOffset.Y && (this.groundHeight < this.chunkGlobalOffset.Y + this.chunkSizeY && this.SetHeightCheck))
              this.region.HeightMap.SetHeight(this.chunkGlobalOffset.X + p.X, this.chunkGlobalOffset.Z + p.Z, (ushort) this.groundHeight, (ushort) this.groundHeight);
            int num2 = p.X + p.Z * this.chunkSizeX;
            int num3 = y;
            for (p.Y = 0; p.Y < this.chunkSizeY; ++p.Y)
            {
              this.GetBlock(p, num3++);
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
        this.PostGenerateChunkCore();
        this.map.ChunkCacheManager.DecRefCount(cacheID3, cacheIndex3);
        this.map.ChunkCacheManager.DecRefCount(cacheID2, cacheIndex2);
        this.map.ChunkCacheManager.DecRefCount(cacheID1, cacheIndex1);
      }
    }

    private void LightChunkAndNeighbours(ChunkLoadFlags type)
    {
      if (!this.isDirtyLight)
        return;
      bool flag = false;
      switch (type)
      {
        case ChunkLoadFlags.Generate:
          flag = !this.allAirBlocks && !this.allSolidBlocks;
          break;
        case ChunkLoadFlags.Decorate:
          flag = !this.chunk.IsChunkFlagSet(ChunkFlags.ChunkIsAllAir | ChunkFlags.ChunkIsAllSolid);
          break;
      }
      if (!flag || this.map.IsChunkPending(this.chunk))
        return;
      this.chunk.SetChunkFlag(ChunkFlags.LightDirty);
      if (this.neighbours.Count == 0)
        this.chunk.GetNeighbours(this.neighbours, (ChunkTest) null);
      foreach (MapChunk neighbour in this.neighbours)
        neighbour.SetChunkFlag(ChunkFlags.LightDirty);
      this.neighbours.Clear();
    }

    protected virtual void SetDefaultGroundAndBelowBlock(
      int x,
      int z,
      int globalY,
      int grassyStoneChance)
    {
      if (globalY <= this.layersStart)
      {
        if (globalY == 0)
        {
          this.getBlockResultBlockID = (byte) 29;
        }
        else
        {
          int num1 = globalY - 1;
          int index = (int) ((double) num1 / (double) this.layerHeight);
          if (index > 0)
          {
            int num2 = (int) ((double) num1 - (double) this.layerHeight * (double) index);
            if (num2 < (int) this.rockTransMap.Range)
            {
              int num3 = (int) this.rockTransMap.GetValue(this.chunkGlobalOffset.X + x, this.chunkGlobalOffset.Z + z, index % 4);
              if (num2 < num3)
                --index;
            }
          }
          this.getBlockResultBlockID = (byte) BiomeBase.layers[index];
        }
      }
      else if (globalY < this.groundHeight - 30)
        this.getBlockResultBlockID = (byte) 17;
      else if (globalY < this.groundHeight - 12)
        this.getBlockResultBlockID = (byte) 16;
      else if (globalY < this.groundHeight - 3)
        this.getBlockResultBlockID = (byte) 15;
      else if (globalY < this.groundHeight)
        this.getBlockResultBlockID = (byte) 2;
      else if (this.random.Next(grassyStoneChance) == 0)
        this.getBlockResultBlockID = (byte) 79;
      else
        this.getBlockResultBlockID = (byte) 1;
    }

    protected virtual bool SetHeightCheck
    {
      get
      {
        return true;
      }
    }

    protected abstract int GetPlaneData(int x, int z);

    protected abstract void GetBlock(Point3D p, int globalY);

    public override void GenerateToHeightMap(
      HeightField map,
      BiomeParams biomeParams,
      ushort seaLevel,
      int seed,
      int xStep,
      int zStep,
      Action<int> rowGenerated)
    {
      this.Initialize(biomeParams, seaLevel);
      this.InitializeRandom(seed);
      BiomeBase.perm = SimplexNoise1.GetSimplexNoisePermTable(seed);
      int sizeX = map.SizeX;
      int sizeZ = map.SizeZ;
      this.chunkGlobalOffset.Y = (int) seaLevel;
      int z = 0;
      int offsetZ = biomeParams.OffsetZ;
      while (z < sizeZ)
      {
        int x = 0;
        int offsetX = biomeParams.OffsetX;
        while (x < sizeX)
        {
          map.SetHeight(x, z, (float) this.GetPlaneData(offsetX, offsetZ));
          ++x;
          offsetX += xStep;
        }
        if (rowGenerated != null)
          rowGenerated(z);
        ++z;
        offsetZ += zStep;
      }
    }

    public override void DecorateChunk(MapChunk chunk)
    {
      this.chunk = chunk;
      this.region = chunk.Region;
      if (this.MustDecorate)
      {
        this.chunkGlobalOffset = chunk.GlobalOffset;
        this.noiseGlobalOffset = this.chunkGlobalOffset;
        this.noiseGlobalOffset.X += this.biomeParams.OffsetX;
        this.noiseGlobalOffset.Z += this.biomeParams.OffsetZ;
        this.InitializeRandom();
        this.blastPointsOfInterest.Clear();
        this.isDirtyLight = false;
        this.DecorateChunkCore();
        if (this.map.IsHost)
          this.DecorateChunkHostOnlyCore();
        this.LightChunkAndNeighbours(ChunkLoadFlags.Decorate);
      }
      else
        ++this.region.ChunksDecoratedCount;
      chunk.ClearTimeStamps();
    }

    protected virtual void DecorateChunkCore()
    {
    }

    protected virtual void DecorateChunkHostOnlyCore()
    {
    }

    protected virtual bool MustDecorate
    {
      get
      {
        return !this.chunk.IsChunkFlagSet(ChunkFlags.ChunkIsAllAir);
      }
    }

    protected float GetBlockNoise(int x, int z)
    {
      return (float) ((double) SimplexNoise1.noise((float) x / this.bn, (float) z / this.bn, BiomeBase.perm) * (double) this.bm + (double) SimplexNoise1.noise((float) x / this.mn, (float) z / this.mn, BiomeBase.perm) * (double) this.mm + (double) SimplexNoise1.noise((float) x / this.fn, (float) z / this.fn, BiomeBase.perm) * (double) this.fm) / this.td;
    }

    protected float GetBlockNoise3D(int x, int y, int z)
    {
      return (SimplexNoise1.noise((float) x / this.mn, (float) z / this.mn, (float) y, BiomeBase.perm) + SimplexNoise1.noise((float) x / this.fn, (float) z / this.fn, (float) y, BiomeBase.perm) * this.fm) / this.td;
    }

    protected Point3D PlaceBlockOnGround(
      Block blockID,
      byte aux,
      BiomeBase.BlockValid isSuitableStart,
      BiomeBase.BlockValid isSuitableBase)
    {
      return this.PlaceBlockOnGround(blockID, aux, isSuitableStart, isSuitableBase, (BiomeBase.BlockValid) null);
    }

    protected Point3D PlaceBlockOnGround(
      Block blockID,
      byte aux,
      BiomeBase.BlockValid isSuitableStart,
      BiomeBase.BlockValid isSuitableBase,
      BiomeBase.BlockValid ignoreGoingDown)
    {
      Point3D zero = Point3D.Zero;
      zero.X = this.random.Next(this.chunkSizeX);
      zero.Y = this.random.Next(this.chunkSizeY);
      zero.Z = this.random.Next(this.chunkSizeZ);
      if (isSuitableStart == null || isSuitableStart(new GlobalPoint3D()
      {
        X = zero.X + this.chunkGlobalOffset.X,
        Y = zero.Y + this.chunkGlobalOffset.Y,
        Z = zero.Z + this.chunkGlobalOffset.Z
      }, blockID, (Block) this.chunk.GetBlockID(zero)))
        return this.PlaceBlockOnGround(zero, blockID, aux, isSuitableBase, ignoreGoingDown);
      return Point3D.Invalid;
    }

    protected Point3D PlaceBlockOnGround(
      Point3D p,
      Block blockID,
      byte aux,
      BiomeBase.BlockValid isSuitableBase,
      BiomeBase.BlockValid ignoreGoingDown)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      p1.X = p.X + this.chunkGlobalOffset.X;
      p1.Y = p.Y + this.chunkGlobalOffset.Y;
      p1.Z = p.Z + this.chunkGlobalOffset.Z;
      Block data;
      for (data = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(p)); p.Y > 0 && (ignoreGoingDown == null && data == Block.None || ignoreGoingDown != null && ignoreGoingDown(p1, blockID, data)); data = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(p)))
      {
        --p.Y;
        --p1.Y;
      }
      if (p.Y > 0 && (isSuitableBase == null || isSuitableBase(p1, blockID, data)))
      {
        ++p.Y;
        ++p1.Y;
        if (this.map.SetBlockData(p1, (byte) blockID, aux, UpdateBlockMethod.Generation, GamerID.Sys1, false) != null)
        {
          if (this.map.BlockData[(int) blockID].Luminance > (byte) 0)
            this.isDirtyLight = true;
          return p;
        }
      }
      return Point3D.Invalid;
    }

    protected GlobalPoint3D PlaceBlockOnGround(
      GlobalPoint3D p,
      Block blockID,
      byte aux,
      BiomeBase.BlockValid isSuitableBase,
      UpdateBlockMethod method)
    {
      return this.PlaceBlockOnGround(p, blockID, aux, isSuitableBase, (BiomeBase.BlockValid) null, method);
    }

    protected GlobalPoint3D PlaceBlockOnGround(
      GlobalPoint3D p,
      Block blockID,
      byte aux,
      BiomeBase.BlockValid isSuitableBase,
      BiomeBase.BlockValid ignoreGoingDown,
      UpdateBlockMethod method)
    {
      Block blockIdNoCache;
      for (blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p); p.Y > this.mapBoundMin.Y + 1 && (ignoreGoingDown == null && blockIdNoCache == Block.None || ignoreGoingDown != null && ignoreGoingDown(p, blockID, blockIdNoCache)); blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p))
        --p.Y;
      if (p.Y - this.chunkGlobalOffset.Y >= 0 && (isSuitableBase == null || isSuitableBase(p, blockID, blockIdNoCache)))
      {
        ++p.Y;
        if (this.map.SetBlockData(p, (byte) blockID, aux, method, GamerID.Sys1, false) != null)
        {
          if (this.map.BlockData[(int) blockID].Luminance > (byte) 0)
            this.isDirtyLight = true;
          return p;
        }
      }
      return new GlobalPoint3D(0, 0, 0);
    }

    protected bool IsCenterChunk(MapChunk chunk)
    {
      int num1 = (int) ((double) (this.mapBoundMax.X - this.mapBoundMin.X) * 0.5 + (double) this.mapBoundMin.X);
      if (num1 - num1 % this.chunkSizeX != this.chunkGlobalOffset.X)
        return false;
      int num2 = (int) ((double) (this.mapBoundMax.Z - this.mapBoundMin.Z) * 0.5 + (double) this.mapBoundMin.Z);
      return num2 - num2 % this.chunkSizeZ == this.chunkGlobalOffset.Z;
    }

    protected virtual int TreeDecoration(float chance, int minCount, int maxCount, int maxY)
    {
      int num1 = 0;
      if (this.chunkGlobalOffset.Y > (int) this.seaLevel - this.chunkSizeY && this.chunkGlobalOffset.Y <= maxY)
      {
        this.InitializeRandom();
        if (this.random.NextDouble() <= (double) chance)
        {
          int num2 = this.random.Next(minCount, maxCount + 1);
          for (int index = 0; index < num2; ++index)
          {
            if (this.TreeDecorationCore(maxY))
              ++num1;
          }
        }
      }
      return num1;
    }

    protected virtual bool TreeDecorationCore(int maxY)
    {
      Point3D p1 = new Point3D();
      p1.X = this.random.Next(this.chunkSizeX);
      p1.Z = this.random.Next(this.chunkSizeZ);
      GlobalPoint3D p2 = new GlobalPoint3D()
      {
        X = p1.X + this.chunkGlobalOffset.X,
        Z = p1.Z + this.chunkGlobalOffset.Z
      };
      p2.Y = (int) this.map.GetHeight(p2);
      if (p2.Y <= maxY)
      {
        p1.Y = p2.Y - this.chunkGlobalOffset.Y;
        if (p1.Y >= 0 && p1.Y < this.chunkSizeY && (this.IsCorrectBlockForTreeBase((Block) this.chunk.GetBlockID(p1)) && this.map.IsInsideMap(p2, new Point3D(5, 11, 5))))
        {
          ModelPlacement result = this.AddTree(this.instance, (Map) this.map, p2, this.random, UpdateBlockMethod.Generation);
          int val2 = Math.Max(result.Model.ModelSize.X, result.Model.ModelSize.Z) / 2 - 2;
          this.AddShadedGrass((Map) this.map, p2, Math.Max(2, val2), this.random, UpdateBlockMethod.Generation);
          this.FlagNeighboursIfModelTouches(this.chunk, result);
          return true;
        }
      }
      return false;
    }

    protected virtual bool IsCorrectBlockForTreeBase(Block blockID)
    {
      return BlockData.IsGrassOrDirt(blockID);
    }

    protected virtual ModelPlacement AddTree(
      GameInstance instance,
      Map map,
      GlobalPoint3D p,
      PcgRandom random,
      UpdateBlockMethod method)
    {
      return VegetationGenerator.AddTree(instance, map, p, random, UpdateBlockMethod.Generation, false);
    }

    protected virtual void AddShadedGrass(
      Map map,
      GlobalPoint3D p,
      int radius,
      PcgRandom random,
      UpdateBlockMethod method)
    {
      VegetationGenerator.AddTreeShadedGrass(map, this, p, radius, random, method, false);
    }

    protected virtual void FlowerDecoration(
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int maxY)
    {
      this.InitializeRandom();
      VegetationGenerator.FlowerDecoration((Map) this.map, this.chunkGlobalOffset, chance, minCount, maxCount, maxWidth, density, maxY, this.random, this.chunkSizeX, this.chunkSizeY, this.chunkSizeZ, this.mapBoundMin, this.mapBoundMax, UpdateBlockMethod.Generation, GamerID.Sys1, false);
    }

    protected virtual void GrassDecoration(
      float chance,
      int minCount,
      int maxCount,
      int maxWidth,
      float density,
      int berryBushChance,
      int maxY)
    {
      this.InitializeRandom();
      VegetationGenerator.GrassDecoration((Map) this.map, this.chunkGlobalOffset, chance, minCount, maxCount, maxWidth, density, berryBushChance, maxY, this.random, this.chunkSizeX, this.chunkSizeY, this.chunkSizeZ, this.mapBoundMin, this.mapBoundMax, UpdateBlockMethod.Generation, GamerID.Sys1, false);
    }

    protected void GenerateOres(BiomeType biome, float density, int maxHeightAboveSealevel)
    {
      this.InitializeRandom();
      if (this.instance.IsLegendaryDifficulty)
        density *= 0.5f;
      density *= (float) Globals2.GameProperties.SaveGame.Header.BiomeParams.OreDensity / 100f;
      this.GenerateOres(OreProperties.Ores, density, maxHeightAboveSealevel);
    }

    private void GenerateOres(
      OreProperty[] oreProperties,
      float density,
      int maxHeightAboveSealevel)
    {
      if ((double) (((float) this.chunkGlobalOffset.Y + (float) this.chunkSizeY / 2f) / (float) Math.Min(this.mapBoundMax.Y, (int) this.seaLevel + maxHeightAboveSealevel)) > 1.0)
        ;
      float num1 = (float) (this.mapBoundMax.Y - this.mapBoundMin.Y);
      float num2 = (float) this.map.TotalChunks * ((float) ((int) this.seaLevel + maxHeightAboveSealevel) / num1);
      for (int index = 0; index < oreProperties.Length; ++index)
      {
        OreProperty oreProperty = oreProperties[index];
        if (this.random.NextDouble() <= (double) oreProperty.DepositFrequency * (double) density / ((double) num2 * ((double) oreProperty.MaxDepth - (double) oreProperty.MinDepth)))
          this.AddOreDeposit(oreProperty);
      }
    }

    private int AddOreDeposit(OreProperty ore)
    {
      Point3D point3D = new Point3D();
      point3D.X = this.random.Next(this.chunkSizeX - 4) + 2;
      point3D.Z = this.random.Next(this.chunkSizeZ - 4) + 2;
      int groundHeightGlobal = this.GetGroundHeightGlobal((Map) this.map, point3D.X + this.chunkGlobalOffset.X, point3D.Z + this.chunkGlobalOffset.Z);
      if (groundHeightGlobal < this.chunkGlobalOffset.Y)
        return 0;
      int num1 = this.chunkGlobalOffset.Y + this.random.Next(this.chunkSizeY - 4) + 2;
      if (num1 > groundHeightGlobal)
        num1 = groundHeightGlobal;
      if ((double) num1 > (double) groundHeightGlobal - (double) groundHeightGlobal * (double) ore.MinDepth || (double) num1 < (double) groundHeightGlobal - (double) groundHeightGlobal * (double) ore.MaxDepth)
        return 0;
      point3D.Y = num1 - this.chunkGlobalOffset.Y;
      int num2 = 0;
      int num3 = (int) (this.random.NextDouble() * (double) ore.DepositSize * 0.200000002980232 + (double) ore.DepositSize * 0.800000011920929);
      MapBlock mapBlock = new MapBlock();
      MapBlock newBlockData = new MapBlock()
      {
        BlockID = (byte) ore.BlockID
      };
      if (this.map.BlockData[(int) ore.BlockID].Luminance > (byte) 0)
        this.isDirtyLight = true;
      for (int index = 0; index < num3; ++index)
      {
        point3D.X += this.random.Next(3) - 1;
        point3D.Y += this.random.Next(3) - 1;
        point3D.Z += this.random.Next(3) - 1;
        GlobalPoint3D p = this.map.Clamp(this.chunkGlobalOffset + point3D, 2);
        MapBlock blockData = this.map.GetBlockData(p);
        Block blockId = (Block) blockData.BlockID;
        if (blockId > Block.None && blockId < Block.Bedrock && (blockId != Block.Water && blockId != Block.Leaves) && this.map.SetBlockData(p, blockData, newBlockData, UpdateBlockMethod.Generation, GamerID.Sys1, false) != null)
        {
          ++num2;
          this.OnOreDeposited(ref p, newBlockData.BlockID);
        }
      }
      return num2;
    }

    protected virtual void OnOreDeposited(ref GlobalPoint3D p, byte blockID)
    {
    }

    protected void GenerateCaves()
    {
      if (Globals2.GameProperties.BlastPoints == null)
        return;
      List<BlastPoint> points;
      lock (Globals2.GameProperties.BlastPoints)
      {
        if (Globals2.GameProperties.BlastPoints.TryGetValue(this.chunk, out points))
          Globals2.GameProperties.BlastPoints.Remove(this.chunk);
      }
      if (points == null || points.Count <= 0)
        return;
      this.InitializeRandom();
      this.CreateBlasts(points);
    }

    private void CreateBlasts(List<BlastPoint> points)
    {
      BoxInt boxInt = new BoxInt()
      {
        Min = this.chunkGlobalOffset,
        Max = {
          X = this.chunkGlobalOffset.X + this.chunkSizeX,
          Y = this.chunkGlobalOffset.Y + this.chunkSizeY,
          Z = this.chunkGlobalOffset.Z + this.chunkSizeZ
        }
      };
      int num = 0;
      BlastPoint blastPoint = new BlastPoint();
      try
      {
        for (int index = 0; index < points.Count; ++index)
        {
          BlastPoint point = points[index];
          if (point.Point != blastPoint.Point)
          {
            BlastResult blast = this.map.CreateBlast(point.Point, point.Strength, point.Radius, this.random, UpdateBlockMethod.Generation, false, GamerID.Sys1, (ushort) this.random.Next());
            if (blast.LowestY > num && !point.TreasureChest && !point.MobSpawn)
              this.LiquidHole(blastPoint.Point);
            num = blast.LowestY;
            blastPoint = point;
          }
        }
        for (int index = 0; index < points.Count; ++index)
          this.TorchesChestsAndMobs(points[index]);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(50, ex);
      }
    }

    private void LiquidHole(GlobalPoint3D p)
    {
      Block blockID = this.random.Next(5) == 0 ? Block.Lava : Block.Water;
      this.isDirtyLight = blockID == Block.Lava;
      p = this.FindDryFloor(p);
      p.Y += 2;
      new FloodFillerNew().FloodFill((Map) this.map, p, blockID, 200, UpdateBlockMethod.Generation, GamerID.Sys1);
    }

    private void TorchesChestsAndMobs(BlastPoint bp)
    {
      if (this.map.IsHost && bp.TreasureChest)
      {
        Vector3 vector3 = bp.Direction * 2f;
        if (this.map.IsInsideMap(bp.Point, new Point3D(2, 2, 2)))
        {
          bp.Point.X += (int) vector3.X;
          bp.Point.Z += (int) vector3.Z;
          this.blastPointsOfInterest.Add(bp);
          bp.Point.X -= (int) vector3.X;
          bp.Point.Z -= (int) vector3.Z;
        }
      }
      if (bp.MobSpawn)
      {
        this.blastPointsOfInterest.Add(bp);
      }
      else
      {
        if (!bp.Torch && bp.Radius <= 8 || bp.Point.Y >= (int) this.map.GetHeight(bp.Point))
          return;
        this.AddTorches(bp);
      }
    }

    private void AddTorches(BlastPoint bp)
    {
      GlobalPoint3D point = bp.Point;
      if (point.X > this.mapBoundMin.X + 4 && point.X < this.mapBoundMax.X - 4)
        point.X += this.random.Next(6) - 3;
      if (point.Z > this.mapBoundMin.Z + 4 && point.Z < this.mapBoundMax.Z - 4)
        point.Z += this.random.Next(6) - 3;
      this.PlaceBlockOnGround(point, Block.Torch, (byte) 4, new BiomeBase.BlockValid(this.IsBlockSuitableBase), UpdateBlockMethod.Generation);
      if (bp.Radius <= 9)
        return;
      this.AddTorchOnCaveWall(bp.Point, bp.Direction, 1.570796f);
      this.AddTorchOnCaveWall(bp.Point, bp.Direction, -1.570796f);
    }

    private void AddTorchOnCaveWall(GlobalPoint3D p, Vector3 dir, float angle)
    {
      dir.Y = 0.0f;
      dir.Normalize();
      Matrix rotationY = Matrix.CreateRotationY(angle);
      dir = Vector3.Transform(dir, rotationY);
      Vector3 vector3 = p.ToVector3();
      int num = 32;
      do
      {
        vector3.X += dir.X;
        vector3.Z += dir.Z;
        p.X = (int) vector3.X;
        p.Z = (int) vector3.Z;
      }
      while (this.map.GetBlockIDNoCache(p) == (byte) 0 && --num > 0);
      if (num <= 0)
        return;
      vector3.X -= dir.X;
      vector3.Z -= dir.Z;
      p.X = (int) vector3.X;
      p.Z = (int) vector3.Z;
      if (!this.map.IsInsideMap(p, Point3D.One))
        return;
      byte blockID = 46;
      BlockDataXML blockDataXml = this.map.BlockData[(int) blockID];
      byte randomAttachment = this.GetRandomAttachment(p);
      if (randomAttachment == byte.MaxValue || this.map.SetBlockData(p, blockID, randomAttachment, UpdateBlockMethod.Generation, GamerID.Sys1, false) == null)
        return;
      this.isDirtyLight = true;
    }

    protected virtual void AddCaveMobSpawn(GlobalPoint3D gp)
    {
    }

    protected void GenerateSurfaceMobSpawns()
    {
      if (this.chunkGlobalOffset.Y + this.chunkSizeY - 1 <= (int) this.seaLevel || this.random.Next(9) != 0)
        return;
      this.InitializeRandom();
      GlobalPoint3D randomXzPointInChunk = this.GetRandomXZPointInChunk(1);
      randomXzPointInChunk.Y = (int) this.map.GetHeight(randomXzPointInChunk) - this.random.Next(2) - 2;
      if (randomXzPointInChunk.Y <= this.chunkGlobalOffset.Y || randomXzPointInChunk.Y >= this.chunkGlobalOffset.Y + this.chunkSizeY || !this.IsSurfaceGroundBlock((Block) this.map.GetBlockID(randomXzPointInChunk)))
        return;
      this.map.SetBlockData(randomXzPointInChunk, (byte) 111, (byte) 1, UpdateBlockMethod.Generation, GamerID.Sys1, false);
      NpcSpawnBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(randomXzPointInChunk) as NpcSpawnBlock;
      dataBlock.DayOrNight = DayOrNight.Night;
      dataBlock.BehaviourTree = "System\\AI\\Spider";
    }

    protected void GenerateRares()
    {
      if (this.chunkGlobalOffset.Y >= this.DepthRaresStart)
        return;
      this.InitializeRandom();
      if (this.random.Next(this.RareFrequency) != 0)
        return;
      RareDataXML rareDataXml = Globals1.RareData[this.random.Next(Globals1.RareData.Length)];
      if (this.random.Next((int) rareDataXml.Level * 2) != 0)
        return;
      int num = this.PlaceBlockOnGround(Block.RaresChest, (byte) (((int) rareDataXml.Level << 4) + 4), (BiomeBase.BlockValid) null, new BiomeBase.BlockValid(this.IsBlockSuitableBase), (BiomeBase.BlockValid) null) != Point3D.Invalid ? 1 : 0;
    }

    protected virtual int DepthRaresStart
    {
      get
      {
        return (int) this.map.SeaLevel - (int) this.map.SeaLevel / 2;
      }
    }

    protected virtual int RareFrequency
    {
      get
      {
        return 500 / Globals1.RareData.Length;
      }
    }

    protected void AddBlastPointsOfInterest()
    {
      if (this.blastPointsOfInterest.Count <= 0)
        return;
      this.InitializeRandom();
      foreach (BlastPoint bp in this.blastPointsOfInterest)
      {
        if (bp.TreasureChest)
          this.AddTreasureChest(bp);
        else if (bp.MobSpawn)
          this.AddCaveMobSpawn(bp);
      }
    }

    protected virtual bool IsAddCaveMobSpawn(BlastPoint bp)
    {
      return false;
    }

    protected virtual void CustomizeCaveMobSpawn(NpcSpawnBlock spawnBlock)
    {
      spawnBlock.SetActorType(ActorType.Caveman);
      spawnBlock.BehaviourTree = "System\\AI\\Default";
    }

    private void AddCaveMobSpawn(BlastPoint bp)
    {
      if (!this.IsAddCaveMobSpawn(bp))
        return;
      Point3D point3D;
      point3D.X = bp.Point.X - this.chunkGlobalOffset.X;
      point3D.Y = bp.Point.Y - this.chunkGlobalOffset.Y;
      point3D.Z = bp.Point.Z - this.chunkGlobalOffset.Z;
      int num = 0;
      Block data1 = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(point3D));
      while (point3D.Y > 1 && data1 == Block.None)
      {
        --point3D.Y;
        data1 = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(point3D));
        ++num;
      }
      if (point3D.Y <= 0 || data1 <= Block.None || (num <= 1 || this.map.BlockData[(int) data1].Buffer != (byte) 0))
        return;
      --point3D.Y;
      bp.Point.Y = point3D.Y + this.chunkGlobalOffset.Y;
      if (this.map.IsNextTo(bp.Point, (byte) 0))
        return;
      MapBlock data2 = new MapBlock()
      {
        BlockID = 137
      };
      this.chunk.SetBlockData(point3D, data2, UpdateBlockMethod.Generation);
      NpcSpawnBlock spawnBlock = this.instance.MapStrategyTM.AddNpcSpawnBlock(bp.Point, UpdateBlockMethod.Generation);
      if (spawnBlock == null)
        return;
      spawnBlock.DayOrNight = DayOrNight.None;
      spawnBlock.Proximity = 40;
      spawnBlock.SpawnFrequency = 20f;
      spawnBlock.MaxActiveInstances = this.random.Next(3) + 1;
      this.CustomizeCaveMobSpawn(spawnBlock);
    }

    private void AddTreasureChest(BlastPoint bp)
    {
      GlobalPoint3D p = this.PlaceBlockOnGround(bp.Point, Block.Chest, (byte) 2, new BiomeBase.BlockValid(this.IsBlockSuitableBaseForTreasureChest), UpdateBlockMethod.Generation);
      if (p.Y == 0)
        return;
      Inventory inventory = new Inventory(50);
      this.AddRandomLoot(p, inventory);
      this.strategy.AddDataBlock((DataBlock) new ChestBlock(p, inventory), UpdateBlockMethod.Generation);
      if (this.random.Next(3) >= 2)
        return;
      this.AddSpiderEggInTreasureChestCave(bp);
    }

    private void AddRandomLoot(GlobalPoint3D p, Inventory inventory)
    {
      int num1 = 0;
      Inventory inventory1 = inventory;
      int index1 = num1;
      int num2 = index1 + 1;
      InventoryItem inventoryItem1 = new InventoryItem(Item.GoldPieces, (this.map.MapHeight - p.Y) * (this.random.Next(3) + 6));
      inventory1[index1] = inventoryItem1;
      for (Item itemID = Item.Obsidian; itemID < Item.Cobblestone; ++itemID)
      {
        if (this.random.Next(10) == 0)
        {
          InventoryItem inventoryItem2 = new InventoryItem(itemID, this.random.Next(10) + 1);
          while (inventoryItem2.PurchaseValue > 100000)
            --inventoryItem2.Count;
          if (inventoryItem2.Count > 0)
            inventory[num2++] = inventoryItem2;
        }
      }
      if (this.random.Next(2) == 0 && this.random.Next(15) < Globals1.RareData.Length)
      {
        int index2 = this.random.Next(Globals1.RareData.Length);
        inventory[num2++] = new InventoryItem(Globals1.RareData[index2].ItemID, 1);
      }
      lock (this.instance.KeysToPlace)
      {
        if (this.instance.KeysToPlace.Count <= 0)
          return;
        int index2 = this.random.Next(this.instance.KeysToPlace.Count);
        Inventory inventory2 = inventory;
        int index3 = num2;
        int num3 = index3 + 1;
        InventoryItem inventoryItem2 = new InventoryItem(this.instance.KeysToPlace[index2], 1);
        inventory2[index3] = inventoryItem2;
        this.instance.KeysToPlace.RemoveAt(index2);
      }
    }

    private void AddSpiderEggInTreasureChestCave(BlastPoint bp)
    {
      this.AddBlockOnCaveWall(bp.Point, Block.SpiderEgg, 0.6f, (float) bp.Radius * 1.5f, bp.Point, 0.0f);
    }

    protected GlobalPoint3D GetRandomXZPointInChunk(int edge)
    {
      GlobalPoint3D chunkGlobalOffset = this.chunkGlobalOffset;
      chunkGlobalOffset.X += this.random.Next(this.chunkSizeX - edge * 2) + edge;
      chunkGlobalOffset.Z += this.random.Next(this.chunkSizeZ - edge * 2) + edge;
      return chunkGlobalOffset;
    }

    protected bool IsSurfaceGroundBlock(Block blockID)
    {
      if (blockID == Block.None)
        return false;
      switch (blockID)
      {
        case Block.Dirt:
        case Block.Sand:
        case Block.Clay:
        case Block.Basalt:
          return true;
        default:
          return false;
      }
    }

    public bool IsBlockNone(GlobalPoint3D p, Block blockIDToPlace, Block blockBaseID)
    {
      return blockBaseID == Block.None;
    }

    public bool IsBlockLava(GlobalPoint3D p, Block blockIDToPlace, Block blockBaseID)
    {
      return blockBaseID == Block.Lava;
    }

    public bool IsBlockSuitableBase(GlobalPoint3D p, Block blockIDToPlace, Block blockBaseID)
    {
      return BiomeBase.IsBlockSuitableBase(this.map, p, blockIDToPlace, blockBaseID);
    }

    public bool IsBlockSuitableDiabloBase(
      GlobalPoint3D p,
      Block blockIDToPlace,
      Block blockBaseID)
    {
      return blockBaseID == Block.Rhyolite;
    }

    public static bool IsBlockSuitableBase(
      MapTM map,
      GlobalPoint3D p,
      Block blockIDToPlace,
      Block blockBaseID)
    {
      if (map.BlockData[(int) blockIDToPlace].IsIcon)
      {
        if (blockIDToPlace == Block.Blueprint || blockIDToPlace == Block.Wisdom)
        {
          if (ItemData.IsSubTypeAny(blockBaseID, ItemSubType.Leaves))
            return false;
        }
        else if (map.IsNextTo(p, (byte) 11, -1, true, true) || map.IsNextTo(p, (byte) 13, -1, true, true))
          return false;
      }
      if (blockBaseID != Block.Water && blockBaseID != Block.Lava && blockBaseID != Block.SnowLayer)
        return map.BlockData[(int) blockBaseID].Buffer == (byte) 0;
      return false;
    }

    protected bool IsBlockSuitableBaseForTreasureChest(
      GlobalPoint3D p,
      Block blockIDToPlace,
      Block blockBaseID)
    {
      return this.map.BlockData[(int) blockIDToPlace].Buffer < (byte) 2;
    }

    private GlobalPoint3D AddBlockOnCaveWall(
      GlobalPoint3D p,
      Block blockID,
      float heightBetweenFloorToCeiling,
      float range,
      GlobalPoint3D lastP,
      float minDistance)
    {
      int y1 = p.Y;
      ++p.Y;
      while (p.Y < this.mapBoundMax.Y - 1 && !this.map.IsSolid(p))
        ++p.Y;
      int y2 = p.Y;
      p.Y = y1;
      while (p.Y > this.mapBoundMin.Y + 1 && !this.map.IsSolid(p))
        --p.Y;
      int y3 = p.Y;
      p.Y = (int) ((double) (y2 - y3) * (double) heightBetweenFloorToCeiling + (double) y3);
      Vector2 zero;
      for (zero = Vector2.Zero; (double) zero.X == 0.0 && (double) zero.Y == 0.0; zero.Y = (float) (this.random.NextDouble() * 2.0 - 1.0))
        zero.X = (float) (this.random.NextDouble() * 2.0 - 1.0);
      zero.Normalize();
      Vector3 vector3_1 = p.ToVector3();
      Vector3 vector3_2 = vector3_1;
      do
      {
        vector3_1.X += zero.X;
        vector3_1.Z += zero.Y;
        p.X = (int) vector3_1.X;
        p.Z = (int) vector3_1.Z;
        if ((double) Vector3.Distance(vector3_1, vector3_2) > (double) range)
          return GlobalPoint3D.Zero;
      }
      while (this.map.GetBlockIDNoCache(p) == (byte) 0);
      vector3_1.X -= zero.X;
      vector3_1.Z -= zero.Y;
      p.X = (int) vector3_1.X;
      p.Z = (int) vector3_1.Z;
      if (!this.map.IsInsideMap(p, Point3D.One) || (double) GlobalPoint3D.Distance(p, lastP) < (double) minDistance)
        return GlobalPoint3D.Zero;
      byte auxData = 0;
      if (this.map.BlockData[(int) blockID].IsAttached)
      {
        auxData = this.GetRandomAttachment(p);
        if (auxData == byte.MaxValue)
          return GlobalPoint3D.Zero;
      }
      this.map.SetBlockData(p, (byte) blockID, auxData, UpdateBlockMethod.Generation, GamerID.Sys1, false);
      if (blockID == Block.SpiderEgg)
      {
        NpcSpawnBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(p) as NpcSpawnBlock;
        if (dataBlock != null)
        {
          dataBlock.SetActorType(ActorType.Spider);
          dataBlock.DayOrNight = DayOrNight.None;
          dataBlock.Proximity = 80;
          dataBlock.SpawnFrequency = 10f;
          dataBlock.MaxActiveInstances = 5;
          dataBlock.BehaviourTree = "System\\AI\\Spider";
        }
      }
      return p;
    }

    private byte GetRandomAttachment(GlobalPoint3D p)
    {
      --p.Y;
      if (this.map.IsSolid(p))
        return 4;
      ++p.Y;
      --p.X;
      if (this.map.IsSolid(p))
        return 2;
      p.X += 2;
      if (this.map.IsSolid(p))
        return 0;
      --p.X;
      --p.Z;
      if (this.map.IsSolid(p))
        return 3;
      p.Z += 2;
      return this.map.IsSolid(p) ? (byte) 1 : byte.MaxValue;
    }

    private GlobalPoint3D FindDryFloor(GlobalPoint3D p)
    {
      while (p.Y > this.map.MapBound.Min.Y + 1 && this.map.GetBlockIDNoCache(p) == (byte) 0)
        --p.Y;
      return p;
    }

    protected void FlagNeighboursIfModelTouches(MapChunk chunk, ModelPlacement result)
    {
      this.neighbours.Clear();
      this.chunkIntersectTestBox.Min = result.Point;
      this.chunkIntersectTestBox.Max = result.Point + result.Model.ModelSize;
      chunk.GetNeighbours(this.neighbours, new ChunkTest(this.ChunkBoxIntersectTest));
      chunk.SetChunkFlag(ChunkFlags.LightDirty | ChunkFlags.MeshDirty);
      foreach (MapChunk neighbour in this.neighbours)
        neighbour?.SetChunkFlag(ChunkFlags.LightDirty | ChunkFlags.MeshDirty);
    }

    public bool ChunkBoxIntersectTest(MapChunk chunk)
    {
      BoxInt boxInt = new BoxInt()
      {
        Min = this.chunkGlobalOffset
      };
      boxInt.Max.X = this.chunkGlobalOffset.X + this.chunkSizeX;
      boxInt.Max.Y = this.chunkGlobalOffset.Y + this.chunkSizeY;
      boxInt.Max.Z = this.chunkGlobalOffset.Z + this.chunkSizeZ;
      return boxInt.Intersects(this.chunkIntersectTestBox);
    }

    public override Color[] GetColorTable(int height)
    {
      Color[] colorArray = new Color[height];
      int index = 0;
      Color color = new Color(20, 40, 180) * 0.6f;
      for (; index < (int) this.seaLevel - this.biomeParams.MaxSeaDepth && index < height; ++index)
        colorArray[index] = color;
      int num1 = index;
      color = new Color(20, 40, 180);
      for (; index < (int) this.seaLevel && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.6f, color, (float) (index - num1) / (float) this.biomeParams.MaxSeaDepth);
      int num2 = index;
      color = new Color(225, 205, 130);
      for (; index < (int) this.seaLevel + 3 && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.9f, color, (float) (index - num2) / 3f);
      int num3 = index;
      color = new Color(99, 133, 55);
      for (; index < (int) this.seaLevel + this.biomeParams.DirtHeight && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.6f, color, (float) (index - num3) / (float) (this.biomeParams.DirtHeight - 3));
      int num4 = index;
      color = new Color(118, 80, 40);
      for (; index < (int) this.seaLevel + this.biomeParams.BasaltHeight && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.7f, color, (float) (index - num4) / (float) (this.biomeParams.BasaltHeight - this.biomeParams.DirtHeight));
      int num5 = index;
      color = new Color(112, 112, 112);
      for (; index < (int) this.seaLevel + this.biomeParams.SnowHeight && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.6f, color, (float) (index - num5) / (float) (this.biomeParams.SnowHeight - this.biomeParams.BasaltHeight));
      int num6 = index;
      color = Color.White;
      for (; index < (int) this.seaLevel + this.biomeParams.SnowHeight + (this.biomeParams.MaxHeight - this.biomeParams.SnowHeight) / 4 && index < height; ++index)
        colorArray[index] = Color.Lerp(color * 0.7f, color, (float) (index - num6) / (float) ((this.biomeParams.MaxHeight - this.biomeParams.SnowHeight) / 4));
      for (; index < height; ++index)
        colorArray[index] = color;
      return colorArray;
    }

    protected delegate bool BlockValid(GlobalPoint3D p, Block blockIDToPlace, Block blockBaseID);
  }
}
