// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DigDeepBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class DigDeepBiome : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<DigDeepBiome> Pool = new StudioForge.Engine.Core.Pool<DigDeepBiome>();
    private List<Blueprint> bpsToRemove = new List<Blueprint>();
    private List<WisdomItem> scrollsToRemove = new List<WisdomItem>();
    private const int lavaLevel0Bottom = 2;
    private const int lavaLevel1Bottom = 1080;
    private const int lavaLevel2Bottom = 2147;
    private int maxNoise;
    private int maxNoiseOver2;
    private float noise;
    private int mapHeight;
    private int roofHeight;
    private int lavaLevelBottom;
    private byte lavaLight;
    private byte goldLight;
    private int seaDirtHeight;
    private int seaBasaltHeight;
    private int seaSnowHeight;
    private int seaSnowLayerHeight;
    public static int DecoratedRefCount;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
      this.lavaLight = map.BlockData[13].Luminance;
      this.goldLight = map.BlockData[37].Luminance;
      this.mapHeight = map.MapHeight;
    }

    protected override void Initialize(BiomeParams biomeParams, ushort seaLevel)
    {
      base.Initialize(biomeParams, seaLevel);
      this.maxHeight = (int) seaLevel + biomeParams.MaxHeight;
      this.maxNoise = (int) ((double) (this.maxHeight - (int) seaLevel + this.maxSeaDepth) * 0.850000023841858);
      this.maxNoiseOver2 = this.maxNoise / 2;
      this.seaEffect = (int) seaLevel - this.maxSeaDepth + this.maxNoiseOver2 - this.waterSaturation;
      this.seaDirtHeight = (int) seaLevel + biomeParams.DirtHeight;
      this.seaBasaltHeight = (int) seaLevel + biomeParams.BasaltHeight;
      this.seaSnowHeight = (int) seaLevel + biomeParams.SnowHeight;
      this.seaSnowLayerHeight = (int) seaLevel + biomeParams.SnowLayerHeight;
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return (int) this.GetGroundHeightLocal(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    private ushort GetGroundHeightLocal(int x, int z)
    {
      return (ushort) ((double) this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z) * (double) this.maxNoise + (double) this.seaEffect);
    }

    protected float GetBlockNoiseUnderground(int x, int z)
    {
      switch (this.lavaLevelBottom)
      {
        case 2:
        case 1080:
          return (float) (((double) SimplexNoise1.noise((float) x / 100f, (float) z / 100f, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) x / 10f, (float) z / 10f, BiomeBase.perm) * 1.0 + (double) SimplexNoise1.noise((float) x / 1f, (float) z / 1f, BiomeBase.perm) * 0.100000001490116) / 10.0);
        case 2147:
          return (float) (((double) SimplexNoise1.noise((float) x / 250f, (float) z / 250f, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) x / 25f, (float) z / 25f, BiomeBase.perm) * 3.0 + (double) SimplexNoise1.noise((float) x / 1f, (float) z / 1f, BiomeBase.perm) * 0.300000011920929) / 10.0);
        default:
          return 0.0f;
      }
    }

    protected override int GetPlaneData(int x, int z)
    {
      if (this.chunkGlobalOffset.Y <= (int) this.seaLevel - this.chunkSizeY)
      {
        this.lavaLevelBottom = this.GetLavaLevelBottom(this.chunkGlobalOffset.Y);
        if (this.lavaLevelBottom >= 0)
        {
          this.noise = this.GetBlockNoiseUnderground(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
          ushort num = (ushort) ((double) this.noise * 30.0 - 10.0 + (double) this.lavaLevelBottom);
          if ((int) num < this.lavaLevelBottom || (int) num >= this.mapBoundMax.Y)
            num = (ushort) this.lavaLevelBottom;
          this.roofHeight = (int) ((double) this.noise * 12.0 + 16.0) + this.lavaLevelBottom;
          if (this.roofHeight < this.lavaLevelBottom + 10)
            this.roofHeight = this.lavaLevelBottom + 10;
          return (int) num;
        }
      }
      this.lavaLevelBottom = -1;
      this.noise = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      return (int) (ushort) ((double) this.noise * (double) this.maxNoise + (double) this.seaEffect);
    }

    private int GetLavaLevelBottom(int globalY)
    {
      if (globalY >= 2147 - this.chunkSizeY)
        return globalY >= 2227 ? -1 : 2147;
      if (globalY >= 1080 - this.chunkSizeY)
        return globalY >= 1160 ? -1 : 1080;
      return globalY >= 2 - this.chunkSizeY && globalY < 82 ? 2 : -1;
    }

    public static int GetLavaLevelID(Map map, int globalY)
    {
      int y = map.ChunkSize.Y;
      if (globalY >= 2147 - y)
        return globalY >= 2227 ? -1 : 2;
      if (globalY >= 1080 - y)
        return globalY >= 1160 ? -1 : 1;
      return globalY >= 2 - y && globalY < 82 ? 0 : -1;
    }

    protected override bool SetHeightCheck
    {
      get
      {
        return this.lavaLevelBottom == -1;
      }
    }

    protected override void GetBlock(Point3D p, int globalY)
    {
      if (this.lavaLevelBottom == -1)
        this.GetBlockCore(p, globalY);
      else
        this.GetBlockLavaLevel(p, globalY);
    }

    private void GetBlockCore(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      if (globalY > this.groundHeight)
      {
        if (globalY > (int) this.seaLevel)
        {
          if (globalY == this.groundHeight + 1 && globalY > this.seaSnowLayerHeight + this.random.Next(3) && globalY < this.seaSnowHeight + 10)
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

    private void GetBlockLavaLevel(Point3D p, int globalY)
    {
      if (p.X + this.chunkGlobalOffset.X > this.mapBoundMin.X && p.X + this.chunkGlobalOffset.X < this.mapBoundMax.X - 1 && (p.Z + this.chunkGlobalOffset.Z > this.mapBoundMin.Z && p.Z + this.chunkGlobalOffset.Z < this.mapBoundMax.Z - 1))
      {
        if (globalY > this.groundHeight)
        {
          if (globalY >= this.roofHeight)
          {
            if (globalY == this.roofHeight && this.random.Next(700) == 0)
            {
              this.getBlockResultBlockID = (byte) 37;
              this.getBlockResultLight = this.goldLight;
              return;
            }
          }
          else
          {
            if (globalY > this.lavaLevelBottom && globalY < this.lavaLevelBottom + 5)
            {
              if (this.lavaLevelBottom == 1080)
              {
                this.getBlockResultBlockID = (byte) 11;
                this.getBlockResultLight = (byte) 0;
                return;
              }
              this.getBlockResultBlockID = (byte) 13;
              this.getBlockResultLight = this.lavaLight;
              return;
            }
            this.getBlockResultBlockID = (byte) 0;
            this.getBlockResultLight = (byte) 0;
            return;
          }
        }
        else if (this.lavaLevelBottom == 1080 && globalY == this.groundHeight)
        {
          this.getBlockResultLight = (byte) 0;
          if (globalY < this.lavaLevelBottom + 6)
          {
            if (this.random.Next(50) == 0)
            {
              this.getBlockResultBlockID = (byte) 37;
              this.getBlockResultLight = this.goldLight;
              return;
            }
            this.getBlockResultBlockID = (byte) 3;
            return;
          }
          if (globalY < this.lavaLevelBottom + 7)
          {
            this.getBlockResultBlockID = (byte) 2;
            return;
          }
          if (globalY < this.lavaLevelBottom + 9)
          {
            this.getBlockResultBlockID = (byte) 1;
            return;
          }
        }
      }
      if (globalY == 0)
        this.getBlockResultBlockID = (byte) 29;
      else
        this.getBlockResultBlockID = (byte) BiomeBase.layers[(int) ((double) (globalY - 1) / (double) this.layerHeight)];
      this.getBlockResultLight = (byte) 0;
    }

    protected override void DecorateChunkCore()
    {
      this.GenerateOres(BiomeType.DigDeep, 35f, 40);
      this.GenerateCaves();
      this.DecorateLavaLevels();
      this.TreeDecoration(this.biomeParams.TreeFrequency / 100f, this.biomeParams.TreeDensityMin, this.biomeParams.TreeDensityMax, this.seaDirtHeight);
      this.FlowerDecoration(0.3f, 1, 3, 7, 0.4f, this.seaDirtHeight + 4);
      this.GrassDecoration(0.4f, 1, 3, 8, 0.2f, 15, this.seaDirtHeight + 4);
      this.GenerateSurfaceMobSpawns();
    }

    protected override void DecorateChunkHostOnlyCore()
    {
      if (this.map.IsChunkPending(this.chunk))
        return;
      this.AddBlastPointsOfInterest();
      this.GenerateRares();
      this.GenerateWisdomScrolls();
      this.GenerateBlueprints();
    }

    private void DecorateLavaLevels()
    {
      switch (this.GetLavaLevelBottom(this.chunk.GlobalOffset.Y))
      {
        case 2:
          this.GenerateTower();
          this.GenerateLavaLevelDiablos();
          break;
        case 2147:
          this.GenerateLavaLevelSpiderEggs();
          break;
      }
    }

    private void GenerateLavaLevelSpiderEggs()
    {
      this.InitializeRandom();
      NpcSpawnBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(this.chunkGlobalOffset + this.PlaceBlockOnGround(Block.SpiderEgg, (byte) 0, new BiomeBase.BlockValid(((BiomeBase) this).IsBlockNone), new BiomeBase.BlockValid(((BiomeBase) this).IsBlockSuitableBase))) as NpcSpawnBlock;
      if (dataBlock == null)
        return;
      dataBlock.DayOrNight = DayOrNight.None;
    }

    private void GenerateLavaLevelDiablos()
    {
      if (this.chunkGlobalOffset.Y != this.mapBoundMin.Y)
        return;
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      if (mapStrategyTm == null)
        return;
      this.InitializeRandom();
      Point3D point3D = this.PlaceBlockOnGround(Block.NPCSpawn, (byte) 0, new BiomeBase.BlockValid(((BiomeBase) this).IsBlockLava), new BiomeBase.BlockValid(((BiomeBase) this).IsBlockSuitableDiabloBase), new BiomeBase.BlockValid(((BiomeBase) this).IsBlockLava));
      NpcSpawnBlock npcSpawnBlock = mapStrategyTm.AddNpcSpawnBlock(this.chunkGlobalOffset + point3D, UpdateBlockMethod.Generation);
      if (npcSpawnBlock == null)
        return;
      npcSpawnBlock.SetActorType(ActorType.Diablo);
      npcSpawnBlock.DayOrNight = DayOrNight.None;
      npcSpawnBlock.Proximity = 80;
      npcSpawnBlock.SpawnFrequency = 30f;
    }

    private void GenerateTower()
    {
      if (this.chunkGlobalOffset.Y != this.mapBoundMin.Y || !this.IsCenterChunk(this.chunk))
        return;
      this.InitializeRandom();
      this.AddTowerModel();
      this.AddGrenadeBP();
      this.ArmSentryTurrets();
      this.AddProximityMines();
    }

    private void AddTowerModel()
    {
      if (this.instance.SystemVoxelModelManager == null)
        return;
      MapModel mapModel = this.instance.SystemVoxelModelManager.LoadComponent("System", "Objects_Dig Deep Tower (SYS)", false);
      GlobalPoint3D one = GlobalPoint3D.One;
      GlobalPoint3D destOffset = this.chunkGlobalOffset + new GlobalPoint3D(1, 2, 1);
      destOffset.X += (this.chunkSizeX - mapModel.Map.MapSize.X) / 2;
      destOffset.Z += (this.chunkSizeZ - mapModel.Map.MapSize.Z) / 2;
      GlobalPoint3D size = mapModel.Map.MapSize - GlobalPoint3D.One - GlobalPoint3D.One;
      mapModel.Map.CopyTo((Map) this.map, one, destOffset, size, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, 0, UpdateBlockMethod.Generation, Map.CopyType.Overwrite, Map.CopyAccess.Full, GamerID.Sys1, false, (IProgressBar) null);
    }

    private void AddGrenadeBP()
    {
      Blueprint grenadeLauncher = Blueprints.GrenadeLauncher;
      if (grenadeLauncher.IsEnabled)
        return;
      byte num = 80;
      for (int index = 0; index < this.map.ChunkLength; ++index)
      {
        if ((int) this.chunk.GetBlockID(index) == (int) num)
        {
          GlobalPoint3D p = (GlobalPoint3D) this.chunk.GetPoint(index) + this.chunkGlobalOffset;
          this.map.SetBlockData(p, (byte) 57, (byte) 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
          this.strategy.AddDataBlock((DataBlock) new BlueprintBlock(p)
          {
            ID = grenadeLauncher.ID
          }, UpdateBlockMethod.Generation);
          grenadeLauncher.IsGenerated = true;
          grenadeLauncher.Point = p;
          break;
        }
      }
    }

    private void ArmSentryTurrets()
    {
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy == null)
        return;
      byte num = 142;
      for (int index = 0; index < this.map.ChunkLength; ++index)
      {
        if ((int) this.chunk.GetBlockID(index) == (int) num)
        {
          GlobalPoint3D p = (GlobalPoint3D) this.chunk.GetPoint(index) + this.chunkGlobalOffset;
          SentryTurretBlock dataBlock = mapStrategy.GetDataBlock(p) as SentryTurretBlock;
          if (dataBlock != null)
          {
            dataBlock.TargetTypes = BlockTargetTypes.Owner | BlockTargetTypes.Players | BlockTargetTypes.Admins;
            dataBlock.Inventory[0] = new InventoryItem(Item.WoodBow, 1);
            dataBlock.Inventory[1] = new InventoryItem(Item.IronArrow, 100);
            dataBlock.Inventory[2] = new InventoryItem(Item.IronArrow, 100);
          }
        }
      }
    }

    private void AddProximityMines()
    {
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy == null)
        return;
      int num1 = this.random.Next(40, 50);
      MapModel mapModel = this.instance.SystemVoxelModelManager.LoadComponent("System", "Objects_Dig Deep Tower (SYS)", false);
      int num2 = (this.chunkSizeX - mapModel.Map.MapSize.X) / 2;
      int num3 = (this.chunkSizeZ - mapModel.Map.MapSize.Z) / 2;
      for (int index = 0; index < num1; ++index)
      {
        int x;
        int z;
        do
        {
          x = this.random.Next(this.chunkSizeX);
          z = this.random.Next(this.chunkSizeZ);
        }
        while (x >= num2 && x <= this.chunkSizeX - num2 || z >= num3 && z <= this.chunkSizeZ - num3);
        Point3D p1 = new Point3D(x, 2, z);
        while (this.chunk.GetBlockID(p1) != (byte) 0 && p1.Y < this.chunkSizeY)
          ++p1.Y;
        if (p1.Y > 3 && p1.Y < this.chunkSizeY)
        {
          p1.Y -= 2;
          GlobalPoint3D p2 = this.chunkGlobalOffset + p1;
          this.map.SetBlockData(p2, (byte) 143, (byte) 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
          ProximityDetectorBlock dataBlock = mapStrategy.GetDataBlock(p2) as ProximityDetectorBlock;
          if (dataBlock != null)
          {
            dataBlock.TargetTypes = BlockTargetTypes.Owner | BlockTargetTypes.Players | BlockTargetTypes.Admins;
            dataBlock.Range = (byte) 5;
            --p2.Y;
            this.map.SetBlockData(p2, (byte) 55, (byte) 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
          }
        }
      }
    }

    protected override int DepthRaresStart
    {
      get
      {
        return 2227;
      }
    }

    private void GenerateWisdomScrolls()
    {
      if (this.random.Next(this.instance.WisdomsToPlace.Count / 10) > 0 || (double) GlobalPoint3D.Distance(this.map.GetPoint((this.instance.NetworkManager.LocalGamers[0].Tag as Player).Position), this.chunkGlobalOffset + new GlobalPoint3D(this.chunkSizeX / 2, this.chunkSizeY / 2, this.chunkSizeZ / 2)) < 96.0)
        return;
      int y = this.chunkGlobalOffset.Y;
      int num1 = y + this.chunkSizeY;
      this.scrollsToRemove.Clear();
      for (int index = 0; index < this.instance.WisdomsToPlace.Count; ++index)
      {
        WisdomItem wisdom = this.instance.WisdomsToPlace[index];
        if (!wisdom.IsGenerated)
        {
          int num2 = this.mapHeight - (int) ((double) Math.Max(0, wisdom.Level - 1) / 9.0 * (double) this.mapHeight);
          if (y <= num2)
          {
            int num3 = this.mapHeight - (int) ((double) Math.Min(9, wisdom.Level + 2) / 9.0 * (double) this.mapHeight);
            if (num1 > num3 && DigDeepBiome.PlaceWisdomScroll(this.instance, this.map, wisdom, this.chunk, this.random))
            {
              this.scrollsToRemove.Add(wisdom);
              break;
            }
          }
        }
        else
          this.scrollsToRemove.Add(wisdom);
      }
      lock (this.instance.WisdomsToPlace)
      {
        foreach (WisdomItem wisdomItem in this.scrollsToRemove)
          this.instance.WisdomsToPlace.Remove(wisdomItem);
      }
    }

    public static bool PlaceWisdomScroll(
      GameInstance instance,
      MapTM map,
      WisdomItem wisdom,
      MapChunk chunk,
      PcgRandom random)
    {
      return DigDeepBiome.PlaceWisdomScrollCore(instance, map, wisdom, chunk, new Point3D()
      {
        X = random.Next(map.ChunkSize.X),
        Y = map.ChunkSize.Y - 1,
        Z = random.Next(map.ChunkSize.Z)
      });
    }

    private static bool PlaceWisdomScrollCore(
      GameInstance instance,
      MapTM map,
      WisdomItem wisdom,
      MapChunk chunk,
      Point3D p)
    {
      Block data1;
      for (data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)); p.Y > 0 && data1 != Block.None; data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)))
        --p.Y;
      for (; p.Y > 0 && data1 == Block.None; data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)))
        --p.Y;
      if (p.Y > 0 && (data1 == Block.SnowLayer || map.BlockData[(int) data1].Buffer == (byte) 0 && data1 != Block.Wood))
      {
        if (data1 != Block.SnowLayer)
          ++p.Y;
        MapBlock data2 = new MapBlock()
        {
          BlockID = 56,
          Light = chunk.GetLight(p)
        };
        lock (wisdom)
        {
          if (!wisdom.IsGenerated)
          {
            chunk.SetBlockData(p, data2, UpdateBlockMethod.Generation);
            GlobalPoint3D p1 = chunk.GlobalOffset + p;
            WisdomScrollBlock wisdomScrollBlock = new WisdomScrollBlock(p1)
            {
              Index = (ushort) Wisdom.GetWisdomIndex(wisdom)
            };
            instance.MapStrategyTM.AddDataBlock((DataBlock) wisdomScrollBlock, UpdateBlockMethod.Generation);
            chunk.SetChunkFlag(ChunkFlags.HasSpecialBlocks);
            wisdom.IsGenerated = true;
            wisdom.Point = p1;
            return true;
          }
        }
      }
      return false;
    }

    private void GenerateBlueprints()
    {
      if ((double) GlobalPoint3D.Distance(this.map.GetPoint((this.instance.NetworkManager.LocalGamers[0].Tag as Player).Position), this.chunkGlobalOffset + new GlobalPoint3D(this.chunkSizeX / 2, this.chunkSizeY / 2, this.chunkSizeZ / 2)) < 96.0)
        return;
      int y = this.chunkGlobalOffset.Y;
      int num1 = y + this.chunkSizeY;
      this.bpsToRemove.Clear();
      for (int index = 0; index < this.instance.BlueprintsToPlace.Count; ++index)
      {
        Blueprint bp = this.instance.BlueprintsToPlace[index];
        if (!bp.IsGenerated)
        {
          if ((double) bp.Depth.X > 0.0 || this.random.Next(8) == 0)
          {
            int num2 = this.mapHeight - (int) ((double) bp.Depth.X * (double) this.mapHeight);
            if (y <= num2)
            {
              int num3 = this.mapHeight - (int) ((double) bp.Depth.Y * (double) this.mapHeight);
              if ((num1 > num3 || num2 > (int) this.map.SeaLevel && num1 > (int) this.map.SeaLevel) && DigDeepBiome.PlaceBlueprint(this.instance, this.map, bp, this.chunk, this.random))
              {
                this.bpsToRemove.Add(bp);
                break;
              }
            }
          }
        }
        else
          this.bpsToRemove.Add(bp);
      }
      lock (this.instance.BlueprintsToPlace)
      {
        foreach (Blueprint blueprint in this.bpsToRemove)
          this.instance.BlueprintsToPlace.Remove(blueprint);
      }
    }

    public static bool PlaceBlueprint(
      GameInstance instance,
      MapTM map,
      Blueprint bp,
      MapChunk chunk,
      PcgRandom random)
    {
      return DigDeepBiome.PlaceBlueprintCore(instance, map, bp, chunk, new Point3D()
      {
        X = random.Next(map.ChunkSize.X),
        Y = map.ChunkSize.Y - 1,
        Z = random.Next(map.ChunkSize.Z)
      });
    }

    private static bool PlaceBlueprintCore(
      GameInstance instance,
      MapTM map,
      Blueprint bp,
      MapChunk chunk,
      Point3D p)
    {
      Block data1;
      for (data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)); p.Y > 0 && data1 != Block.None; data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)))
        --p.Y;
      for (; p.Y > 0 && data1 == Block.None; data1 = (Block) chunk.BlockData.GetData(chunk, chunk.GetMapIndex(p)))
        --p.Y;
      if (p.Y > 0 && (data1 == Block.SnowLayer || map.BlockData[(int) data1].Buffer == (byte) 0 && data1 != Block.Wood))
      {
        if (data1 != Block.SnowLayer)
          ++p.Y;
        MapBlock data2 = new MapBlock()
        {
          BlockID = 57,
          Light = chunk.GetLight(p)
        };
        lock (bp)
        {
          if (!bp.IsGenerated)
          {
            chunk.SetBlockData(p, data2, UpdateBlockMethod.Generation);
            GlobalPoint3D p1 = chunk.GlobalOffset + p;
            BlueprintBlock blueprintBlock = new BlueprintBlock(p1)
            {
              ID = bp.ID
            };
            instance.MapStrategyTM.AddDataBlock((DataBlock) blueprintBlock, UpdateBlockMethod.Generation);
            chunk.SetChunkFlag(ChunkFlags.LightDirty | ChunkFlags.HasSpecialBlocks);
            bp.IsGenerated = true;
            bp.Point = p1;
            return true;
          }
        }
      }
      return false;
    }
  }
}
