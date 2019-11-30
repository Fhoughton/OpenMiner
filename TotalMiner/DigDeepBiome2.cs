// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DigDeepBiome2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
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
  internal class DigDeepBiome2 : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<DigDeepBiome2> Pool = new StudioForge.Engine.Core.Pool<DigDeepBiome2>();
    private List<Blueprint> bpsToRemove = new List<Blueprint>();
    private List<WisdomItem> scrollsToRemove = new List<WisdomItem>();
    private const int lavaLevel0Bottom = 0;
    private const int lavaLevel1Bottom = 1088;
    private const int lavaLevel2Bottom = 2144;
    private int maxNoise;
    private int maxNoiseOver2;
    private float noise;
    private int mapHeight;
    private int roofHeight;
    private int lavaLevelBottom;
    private byte lavaLight;
    private byte torchLight;
    private byte goldLight;
    private int seaDirtHeight;
    private int seaBasaltHeight;
    private int seaSnowHeight;
    private int seaSnowLayerHeight;
    private float[] chunkNoise;
    public static int DecoratedRefCount;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
      this.lavaLight = map.BlockData[13].Luminance;
      this.goldLight = map.BlockData[37].Luminance;
      this.torchLight = map.BlockData[46].Luminance;
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
      return this.GetGroundHeightLocal(x - this.chunkGlobalOffset.X, z - this.chunkGlobalOffset.Z);
    }

    private int GetGroundHeightLocal(int x, int z)
    {
      return (int) (ushort) ((double) this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z) * (double) this.maxNoise + (double) this.seaEffect);
    }

    protected float GetBlockNoiseUnderground(int x, int y, int z)
    {
      switch (this.lavaLevelBottom)
      {
        case 0:
        case 1088:
          return (float) (((double) SimplexNoise1.noise((float) x / 100f, (float) z / 100f, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) x / 30f, (float) z / 30f, BiomeBase.perm) * 1.0 + (double) SimplexNoise1.noise((float) x / 5f, (float) z / 5f, BiomeBase.perm) * 0.100000001490116) / 10.0);
        case 2144:
          return (float) (((double) SimplexNoise1.noise((float) x / 90f, (float) z / 90f, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) x / 25f, (float) z / 25f, BiomeBase.perm) + (double) SimplexNoise1.noise((float) x / 4f, (float) z / 4f, BiomeBase.perm) * 0.100000001490116) / 11.1000003814697);
        default:
          return 0.0f;
      }
    }

    protected override int GetPlaneData(int x, int z)
    {
      if (this.chunkGlobalOffset.Y <= (int) this.seaLevel - this.chunkSizeY)
      {
        this.lavaLevelBottom = this.GetLavaLevelBottom();
        if (this.lavaLevelBottom >= 0)
        {
          this.noise = this.GetBlockNoiseUnderground(x + this.noiseGlobalOffset.X, 0, z + this.noiseGlobalOffset.Z);
          int num1 = this.lavaLevelBottom == 2144 ? 48 : 64;
          int num2 = num1 >> 2;
          int num3 = this.lavaLevelBottom + (int) ((double) this.noise * (double) num2 - (double) (num2 >> 2));
          if (num3 < this.lavaLevelBottom)
            num3 = this.lavaLevelBottom;
          float noiseUnderground = this.GetBlockNoiseUnderground(x + this.noiseGlobalOffset.X + 10000, 0, z + this.noiseGlobalOffset.Z + 10000);
          this.roofHeight = this.lavaLevelBottom + num1 - (int) ((double) noiseUnderground * (double) num2);
          return num3;
        }
      }
      this.lavaLevelBottom = -1;
      this.noise = this.GetBlockNoise(x + this.noiseGlobalOffset.X, z + this.noiseGlobalOffset.Z);
      return (int) (ushort) ((double) this.noise * (double) this.maxNoise + (double) this.seaEffect);
    }

    private int GetLavaLevelBottom()
    {
      int y = this.chunkGlobalOffset.Y;
      if (y >= 2144)
        return y >= 2208 ? -1 : 2144;
      if (y >= 1088)
        return y >= 1152 ? -1 : 1088;
      return y >= 0 && y < 64 ? 0 : -1;
    }

    public static int GetLavaLevelID(Map map, int globalY)
    {
      int y = map.ChunkSize.Y;
      if (globalY >= 2144)
        return globalY >= 2192 ? -1 : 2;
      if (globalY >= 1088)
        return globalY >= 1152 ? -1 : 1;
      return globalY >= 0 && globalY < 64 ? 0 : -1;
    }

    public static int GetLavaLevelViewingID(Map map, int globalY)
    {
      int y = map.ChunkSize.Y;
      if (globalY >= 2144 - y)
        return globalY >= 2192 + y ? -1 : 2;
      if (globalY >= 1088 - y)
        return globalY >= 1152 + y ? -1 : 1;
      return globalY >= -y && globalY < 64 + y ? 0 : -1;
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
      {
        this.GetBlockCore(p, globalY);
      }
      else
      {
        switch (this.lavaLevelBottom)
        {
          case 0:
            this.GetBlockLavaLevel0(p, globalY);
            break;
          case 1088:
            this.GetBlockLavaLevel1(p, globalY);
            break;
          case 2144:
            this.GetBlockLavaLevel2(p, globalY);
            break;
        }
      }
    }

    private void GetBlockCore(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      this.getBlockResultLight = (byte) 0;
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
      else if (globalY < (int) this.seaLevel + 3 && globalY > this.groundHeight - 2 && globalY != 0)
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
    }

    private void GetBlockLavaLevel0(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      this.getBlockResultLight = (byte) 0;
      if (globalY >= this.roofHeight || globalY <= this.groundHeight)
      {
        this.getBlockResultBlockID = (byte) 28;
        if (globalY != 0)
          return;
        this.getBlockResultBlockID = (byte) 29;
      }
      else
      {
        int num1 = p.X + this.chunkGlobalOffset.X;
        int num2 = p.Z + this.chunkGlobalOffset.Z;
        if (num1 == this.mapBoundMin.X || num1 == this.mapBoundMax.X - 1 || (num2 == this.mapBoundMin.Z || num2 == this.mapBoundMax.Z - 1))
          this.getBlockResultBlockID = (byte) 28;
        else if ((double) this.CalcNoiseLavaLevel0(p.X, p.Y, p.Z) < 0.0)
        {
          if (globalY >= this.lavaLevelBottom + 5)
          {
            this.getBlockResultBlockID = (byte) 0;
          }
          else
          {
            this.getBlockResultBlockID = (byte) 13;
            this.getBlockResultLight = this.lavaLight;
          }
        }
        else
          this.getBlockResultBlockID = (byte) 28;
      }
    }

    private void GetBlockLavaLevel1(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      this.getBlockResultLight = (byte) 0;
      if (globalY >= this.roofHeight)
      {
        this.getBlockResultBlockID = (byte) 23;
        if (globalY != this.roofHeight || this.random.Next(500) != 0)
          return;
        this.getBlockResultBlockID = (byte) 37;
        this.getBlockResultLight = this.goldLight;
      }
      else
      {
        int num1 = p.X + this.chunkGlobalOffset.X;
        int num2 = p.Z + this.chunkGlobalOffset.Z;
        if (num1 == this.mapBoundMin.X || num1 == this.mapBoundMax.X - 1 || (num2 == this.mapBoundMin.Z || num2 == this.mapBoundMax.Z - 1))
          this.getBlockResultBlockID = (byte) 23;
        else if (globalY == this.groundHeight)
        {
          if (globalY < this.lavaLevelBottom + 6)
          {
            if (this.random.Next(150) == 0)
            {
              this.getBlockResultBlockID = (byte) 37;
              this.getBlockResultLight = this.goldLight;
            }
            else
              this.getBlockResultBlockID = (byte) 3;
          }
          else if (globalY < this.lavaLevelBottom + 7)
            this.getBlockResultBlockID = (byte) 2;
          else
            this.getBlockResultBlockID = (byte) 1;
        }
        else if (globalY < this.groundHeight)
          this.getBlockResultBlockID = globalY == this.groundHeight - 1 ? (byte) 2 : (byte) 23;
        else if ((double) this.CalcNoiseLavaLevel1(p.X, p.Y, p.Z) < 0.0)
        {
          if (globalY >= this.lavaLevelBottom + 4)
          {
            this.getBlockResultBlockID = (byte) 0;
            if (globalY != this.groundHeight + 1 || this.random.Next(150) != 0)
              return;
            this.getBlockResultBlockID = (byte) 46;
            this.getBlockResultLight = this.torchLight;
            this.getBlockResultAux = (byte) 4;
          }
          else
            this.getBlockResultBlockID = (byte) 11;
        }
        else
          this.getBlockResultBlockID = (byte) 23;
      }
    }

    private void GetBlockLavaLevel2(Point3D p, int globalY)
    {
      this.getBlockResultAux = (byte) 0;
      this.getBlockResultLight = (byte) 0;
      if (globalY >= this.roofHeight || globalY <= this.groundHeight)
      {
        this.getBlockResultBlockID = (byte) 18;
        if (globalY != this.roofHeight || this.random.Next(700) != 0)
          return;
        this.getBlockResultBlockID = (byte) 37;
        this.getBlockResultLight = this.goldLight;
      }
      else
      {
        int num1 = p.X + this.chunkGlobalOffset.X;
        int num2 = p.Z + this.chunkGlobalOffset.Z;
        if (num1 == this.mapBoundMin.X || num1 == this.mapBoundMax.X - 1 || (num2 == this.mapBoundMin.Z || num2 == this.mapBoundMax.Z - 1))
          this.getBlockResultBlockID = (byte) 18;
        else if ((double) this.CalcNoiseLavaLevel2(p.X, p.Y, p.Z) < 0.0)
        {
          if (globalY >= this.lavaLevelBottom + 5)
          {
            this.getBlockResultBlockID = (byte) 0;
          }
          else
          {
            this.getBlockResultBlockID = (byte) 13;
            this.getBlockResultLight = this.lavaLight;
          }
        }
        else
          this.getBlockResultBlockID = (byte) 18;
      }
    }

    protected override void PreGenerateChunkCore()
    {
    }

    private void GenerateNoiseTable()
    {
      int lavaLevelId = DigDeepBiome2.GetLavaLevelID((Map) this.map, this.chunkGlobalOffset.Y);
      if (lavaLevelId == -1)
        return;
      int index = 0;
      int num1 = 1;
      int num2 = this.chunkSizeX * this.chunkSizeZ;
      if (this.chunkNoise == null || this.chunkNoise.Length < this.chunkSizeY * num2)
        this.chunkNoise = new float[this.chunkSizeY * num2];
      switch (lavaLevelId)
      {
        case 0:
          for (int y = 0; y < this.chunkSizeY; ++y)
          {
            for (int z = 0; z < this.chunkSizeZ; ++z)
            {
              for (int x = 0; x < this.chunkSizeX; ++x)
              {
                this.chunkNoise[index] = this.CalcNoiseLavaLevel0(x, y, z);
                ++index;
              }
            }
            ++num1;
          }
          break;
        case 1:
          for (int y = 0; y < this.chunkSizeY; ++y)
          {
            for (int z = 0; z < this.chunkSizeZ; ++z)
            {
              for (int x = 0; x < this.chunkSizeX; ++x)
              {
                this.chunkNoise[index] = this.CalcNoiseLavaLevel1(x, y, z);
                ++index;
              }
            }
            ++num1;
          }
          break;
        case 2:
          for (int y = 0; y < this.chunkSizeY; ++y)
          {
            for (int z = 0; z < this.chunkSizeZ; ++z)
            {
              for (int x = 0; x < this.chunkSizeX; ++x)
              {
                this.chunkNoise[index] = this.CalcNoiseLavaLevel2(x, y, z);
                ++index;
              }
            }
            ++num1;
          }
          break;
      }
    }

    private float CalcNoiseLavaLevel0(int x, int y, int z)
    {
      int num1 = x + this.noiseGlobalOffset.X;
      int num2 = y + this.noiseGlobalOffset.Y;
      int num3 = z + this.noiseGlobalOffset.Z;
      float num4 = 40f;
      if ((double) SimplexNoise1.noise((float) num1 / num4, (float) num2 / num4, (float) num3 / num4, BiomeBase.perm) < -0.5)
        return -1f;
      float num5 = 90f;
      float num6 = 12f;
      float num7 = (float) (((double) SimplexNoise1.noise((float) num1 / num5, (float) num2 / num5, (float) num3 / num5, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) num1 / num6, (float) num2 / num6, (float) num3 / num6, BiomeBase.perm)) / 11.0 - 0.600000023841858);
      if ((double) num7 <= 0.0 && (double) num7 >= -0.5)
      {
        float num8 = (float) (this.roofHeight - this.groundHeight) / 3f;
        float num9 = (float) (y + this.chunkGlobalOffset.Y - this.groundHeight);
        if ((double) num9 <= (double) num8)
          num7 += MathHelper.Lerp(0.5f, 0.0f, num9 / num8);
        else if ((double) (num9 - num8 * 2f) >= 0.0)
          num7 += MathHelper.Lerp(0.5f, 0.0f, num9 / num8);
      }
      float num10 = Vector3.DistanceSquared(new Vector3((float) (x + this.chunkGlobalOffset.X), (float) (y + this.chunkGlobalOffset.Y), (float) (z + this.chunkGlobalOffset.Z)), new Vector3(271f, 16f, 271f));
      if ((double) num10 < 500.0)
        num7 -= MathHelper.Lerp(1f, 0.0f, num10 / 500f);
      return num7;
    }

    private float CalcNoiseLavaLevel1(int x, int y, int z)
    {
      int num1 = x + this.noiseGlobalOffset.X;
      int num2 = y + this.noiseGlobalOffset.Y;
      int num3 = z + this.noiseGlobalOffset.Z;
      float num4 = 60f;
      if ((double) SimplexNoise1.noise((float) num1 / num4, (float) num2 / num4, (float) num3 / num4, BiomeBase.perm) < -0.25)
        return -1f;
      float num5 = 80f;
      float num6 = 16f;
      float num7 = (float) (((double) SimplexNoise1.noise((float) num1 / num5, (float) num2 / num5, (float) num3 / num5, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) num1 / num6, (float) num2 / num6, (float) num3 / num6, BiomeBase.perm)) / 11.0 - 0.400000005960464);
      if ((double) num7 <= 0.0 && (double) num7 >= -0.5)
      {
        float num8 = (float) (this.roofHeight - this.groundHeight) / 3f;
        float num9 = (float) (y + this.chunkGlobalOffset.Y - this.groundHeight);
        if ((double) num9 <= (double) num8)
          num7 += MathHelper.Lerp(0.5f, 0.0f, num9 / num8);
        else if ((double) (num9 - num8 * 2f) >= 0.0)
          num7 += MathHelper.Lerp(0.5f, 0.0f, num9 / num8);
      }
      return num7;
    }

    private float CalcNoiseLavaLevel2(int x, int y, int z)
    {
      int num1 = x + this.noiseGlobalOffset.X;
      int num2 = y + this.noiseGlobalOffset.Y;
      int num3 = z + this.noiseGlobalOffset.Z;
      float num4 = 40f;
      float num5 = 5f;
      if (((double) SimplexNoise1.noise((float) num1 / num4, (float) num2 / num4, (float) num3 / num4, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) num1 / num5, (float) num2 / num5, (float) num3 / num5, BiomeBase.perm)) / 11.0 < -0.649999976158142)
        return -1f;
      float num6 = 60f;
      float num7 = 15f;
      float num8 = (float) (((double) SimplexNoise1.noise((float) num1 / num6, (float) num2 / num6, (float) num3 / num6, BiomeBase.perm) * 10.0 + (double) SimplexNoise1.noise((float) num1 / num7, (float) num2 / num7, (float) num3 / num7, BiomeBase.perm)) / 11.0 - 0.449999988079071);
      if ((double) num8 <= 0.0 && (double) num8 >= -0.5)
      {
        float num9 = (float) (this.roofHeight - this.groundHeight) / 3f;
        float num10 = (float) (y + this.chunkGlobalOffset.Y - this.groundHeight);
        if ((double) num10 <= (double) num9)
        {
          num8 += MathHelper.Lerp(0.5f, 0.0f, num10 / num9);
        }
        else
        {
          float num11 = (float) (this.roofHeight - (y + this.chunkGlobalOffset.Y));
          if ((double) num11 <= (double) num9)
            num8 += MathHelper.Lerp(0.5f, 0.0f, num11 / num9);
        }
      }
      return num8;
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
      switch (this.GetLavaLevelBottom())
      {
        case 0:
          this.GenerateTower();
          this.GenerateLavaLevelDiablos();
          break;
        case 2144:
          this.GenerateLavaLevelSpiderEggs();
          break;
      }
    }

    private void GenerateLavaLevelSpiderEggs()
    {
      this.InitializeRandom();
      Point3D point3D = this.PlaceBlockOnGround(Block.SpiderEgg, (byte) 0, new BiomeBase.BlockValid(((BiomeBase) this).IsBlockNone), new BiomeBase.BlockValid(((BiomeBase) this).IsBlockSuitableBase));
      if (!(point3D != Point3D.Invalid))
        return;
      NpcSpawnBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(this.chunkGlobalOffset + point3D) as NpcSpawnBlock;
      if (dataBlock == null)
        return;
      dataBlock.SetActorType(ActorType.Spider);
      dataBlock.DayOrNight = DayOrNight.None;
      dataBlock.Proximity = 80;
      dataBlock.SpawnFrequency = 10f;
      dataBlock.MaxActiveInstances = 10;
      dataBlock.BehaviourTree = "System\\AI\\Spider";
    }

    private void GenerateLavaLevelDiablos()
    {
      if (this.chunkGlobalOffset.Y != this.mapBoundMin.Y || this.random.Next(2) != 0)
        return;
      Point3D point3D;
      point3D.X = this.random.Next(this.chunkSizeX);
      point3D.Y = 4;
      point3D.Z = this.random.Next(this.chunkSizeX);
      Block data1 = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(point3D));
      if (data1 != Block.Lava)
        return;
      for (; data1 == Block.Lava; data1 = (Block) this.chunk.BlockData.GetData(this.chunk, this.chunk.GetMapIndex(point3D)))
        --point3D.Y;
      if (point3D.Y <= 0)
        return;
      MapBlock data2 = new MapBlock()
      {
        BlockID = 137
      };
      this.chunk.SetBlockData(point3D, data2, UpdateBlockMethod.Generation);
      NpcSpawnBlock npcSpawnBlock = this.instance.MapStrategyTM.AddNpcSpawnBlock(new GlobalPoint3D(point3D.X, point3D.Y, point3D.Z) + this.chunkGlobalOffset, UpdateBlockMethod.Generation);
      if (npcSpawnBlock == null)
        return;
      npcSpawnBlock.SetActorType(ActorType.Diablo);
      npcSpawnBlock.BehaviourTree = "System\\AI\\Diablo";
      npcSpawnBlock.DayOrNight = DayOrNight.None;
      npcSpawnBlock.Proximity = 40;
      npcSpawnBlock.SpawnFrequency = 20f;
      npcSpawnBlock.MaxActiveInstances = 1;
    }

    protected override bool IsAddCaveMobSpawn(BlastPoint bp)
    {
      return bp.Point.Y < this.mapHeight - 200;
    }

    protected override void CustomizeCaveMobSpawn(NpcSpawnBlock spawnBlock)
    {
      if (this.random.Next(10) == 0)
        return;
      if (this.random.Next(10) == 0)
      {
        spawnBlock.SetActorType(ActorType.Orc);
        spawnBlock.BehaviourTree = "System\\AI\\Orc";
      }
      else if (spawnBlock.Point.Y < 1000)
      {
        spawnBlock.SetActorType(ActorType.TrollChief);
        spawnBlock.BehaviourTree = "System\\AI\\TrollChief";
      }
      else if (spawnBlock.Point.Y < 1600)
      {
        spawnBlock.SetActorType(ActorType.Orc);
        spawnBlock.BehaviourTree = "System\\AI\\Orc";
      }
      else if (spawnBlock.Point.Y < 2400)
      {
        spawnBlock.SetActorType(ActorType.Zombie);
        spawnBlock.BehaviourTree = "System\\AI\\Zombie";
      }
      else
      {
        spawnBlock.SetActorType(ActorType.Goblin);
        spawnBlock.BehaviourTree = "System\\AI\\Goblin";
      }
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
        return 2224;
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
            if (num1 > num3 && DigDeepBiome2.PlaceWisdomScroll(this.instance, this.map, wisdom, this.chunk, this.random))
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
      return DigDeepBiome2.PlaceWisdomScrollCore(instance, map, wisdom, chunk, new Point3D()
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
              if ((num1 > num3 || num2 > (int) this.map.SeaLevel && num1 > (int) this.map.SeaLevel) && DigDeepBiome2.PlaceBlueprint(this.instance, this.map, bp, this.chunk, this.random))
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
      return DigDeepBiome2.PlaceBlueprintCore(instance, map, bp, chunk, new Point3D()
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
