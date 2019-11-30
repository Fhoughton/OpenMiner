// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapTM
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class MapTM : Map, ITMMap
  {
    public static Block[] CoverBlockTop = new Block[6]
    {
      Block.Grass,
      Block.Snow,
      Block.Ice,
      Block.Dirt,
      Block.Sand,
      Block.Crop
    };
    public static string[] DecalNames = new string[45]
    {
      "No Decal",
      "Cracks",
      "Blood Splat",
      "Slime",
      "Dirt",
      "Scorch",
      "Cobweb",
      "Blood Scratch",
      "Mesh",
      "Wood Bracing",
      "Print",
      "Moss",
      "Bones",
      "Rock Path",
      "Stone Path",
      "Sand",
      "Snow",
      "Shells",
      "Cracks2",
      "Cracks3",
      "Cracks4",
      "Cracks5",
      "Cracks6",
      "Cracks7",
      "Cracks8",
      "Cracks9",
      "Cobble Mortar",
      "Small Mortar",
      "Large Mortar",
      "Tint1",
      "Tint2",
      "Tint3",
      "Tint4",
      "Tint5",
      "Tint6",
      "Tint7",
      "Tint8",
      "Tint9",
      "Tint10",
      "Tint11",
      "Tint12",
      "Tint13",
      "Tint14",
      "Tint15",
      "Tint16"
    };
    private static object biomePoolLock = new object();
    public static Matrix[] RotatedBlockMatrices = new Matrix[6]
    {
      Matrix.Identity,
      Matrix.CreateRotationY(-1.570796f),
      Matrix.CreateRotationY(3.141593f),
      Matrix.CreateRotationY(1.570796f),
      Matrix.CreateRotationZ(-1.570796f),
      Matrix.CreateRotationZ(1.570796f)
    };
    public Block[,] BlockTextures = new Block[62, 16];
    public MapModel[,] CustomBlockModels = new MapModel[10, 16];
    public Dictionary<long, MapChunkContentData[]> MapChunkContentBreakdown = new Dictionary<long, MapChunkContentData[]>();
    public ChunkVertexBufferBreakdownPool MapChunkContentBreakdownPool = new ChunkVertexBufferBreakdownPool();
    public List<string> SignTextCache = new List<string>();
    public List<ushort> SignTextCacheRTIndex = new List<ushort>();
    private MapTM.RLEStreamBytePending tempRLEStream = new MapTM.RLEStreamBytePending();
    private Dictionary<long, MapChunkPendingData> chunkPendingList = new Dictionary<long, MapChunkPendingData>();
    private List<KeyValuePair<long, MapChunkPendingData>> pendingToProcess = new List<KeyValuePair<long, MapChunkPendingData>>();
    private List<MapChunkTM> chunksBeingCommitted = new List<MapChunkTM>();
    private Pool<List<int>> dataBlockIndexListPool = new Pool<List<int>>(8);
    private Pool<List<DataBlock>> dataBlocksPostLoadPool = new Pool<List<DataBlock>>(8);
    private List<DataBlock> postLoadDataBlocks = new List<DataBlock>(20);
    private List<DataBlock> copiedDataBlocks = new List<DataBlock>(10);
    public const int MaxBlockTextures = 16;
    public const int MaxBlocksWithSelectableTextures = 62;
    public GameInstance Instance;
    public BlockDataXML[] BlockData;
    public ChunkTest CanProcessPendingChunkTest;
    public bool AllowMeshCreatorToSplitOrFade;
    public bool SignTextCacheChanged;
    public bool AllChunksOutsideViewGenerated;
    public Matrix TranslateDownOneTile;
    private List<WifiTransmitterBlock> transmittersCopyTemp;
    private List<MapTM.ReceiverReLinkInfo> receiversCopyTemp;
    private List<WifiReceiverBlock> receiversPasteTemp;
    private List<MapTM.TransmitterLinkInfo> transmittersPasteTemp;

    float ITMMap.TileSize
    {
      get
      {
        return this.TileSize;
      }
    }

    void ITMMap.Commit()
    {
      this.Commit();
    }

    GlobalPoint3D ITMMap.GetPoint(Vector3 pos)
    {
      return this.GetPoint(pos);
    }

    Vector3 ITMMap.GetPosition(GlobalPoint3D p)
    {
      return this.GetPosition(p);
    }

    Vector3 ITMMap.GetBlockCenter(GlobalPoint3D p)
    {
      return this.GetBlockCenter(p);
    }

    Block ITMMap.GetBlockID(Vector3 position)
    {
      return (Block) this.GetBlockID(position);
    }

    Block ITMMap.GetBlockID(GlobalPoint3D p)
    {
      return (Block) this.GetBlockID(p);
    }

    Block ITMMap.GetBlockIDNoCache(GlobalPoint3D p)
    {
      return (Block) this.GetBlockIDNoCache(p);
    }

    MapBlock ITMMap.GetBlockData(GlobalPoint3D p)
    {
      return this.GetBlockData(p);
    }

    MapBlock ITMMap.GetBlockIDAndAux(GlobalPoint3D p)
    {
      return this.GetBlockIDAndAux(p);
    }

    MapBlock ITMMap.GetBlockIDAndAuxNoCache(GlobalPoint3D p)
    {
      return this.GetBlockIDAndAuxNoCache(p);
    }

    MapBlock ITMMap.GetBlockAndLight(GlobalPoint3D p)
    {
      return this.GetBlockAndLight(p);
    }

    bool ITMMap.IsBlockDataEqual(Vector3 pos, Block blockID, byte aux)
    {
      return this.IsBlockDataEqual(pos, (byte) blockID, aux);
    }

    void ITMMap.SetBlockData(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      this.SetBlockData(p, (byte) blockID, auxData, method, gamerID, transmit);
    }

    void ITMMap.SetBlockData(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      this.SetBlockData(p, oldBlockData, newBlockData, method, gamerID, transmit);
    }

    ClearBlockResult ITMMap.ClearBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      return this.ClearBlock(p, method, gamerID, transmit);
    }

    byte ITMMap.GetAuxData(GlobalPoint3D p)
    {
      return this.GetAuxData(p);
    }

    byte ITMMap.GetAuxDataNoCache(GlobalPoint3D p)
    {
      return this.GetAuxDataNoCache(p);
    }

    byte ITMMap.GetAuxHighData(GlobalPoint3D p)
    {
      return this.GetAuxHighData(p);
    }

    byte ITMMap.GetAuxHighDataNoCache(GlobalPoint3D p)
    {
      return this.GetAuxHighDataNoCache(p);
    }

    byte ITMMap.GetAuxFullData(GlobalPoint3D p)
    {
      return this.GetAuxFullData(p);
    }

    byte ITMMap.GetAuxFullDataNoCache(GlobalPoint3D p)
    {
      return this.GetAuxFullDataNoCache(p);
    }

    bool ITMMap.HasChanged(byte auxData)
    {
      return this.HasChanged(auxData);
    }

    bool ITMMap.HasChanged(MapBlock blockData)
    {
      return this.HasChanged(blockData);
    }

    bool ITMMap.HasChanged(GlobalPoint3D p)
    {
      return this.HasChanged(p);
    }

    void ITMMap.SetAuxData(
      GlobalPoint3D p,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      this.SetAuxData(p, auxData, method, gamerID, transmit);
    }

    void ITMMap.SetAuxData(
      GlobalPoint3D p,
      byte oldAuxData,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      this.SetAuxData(p, oldAuxData, auxData, method, gamerID, transmit);
    }

    MapLight ITMMap.GetLight(GlobalPoint3D p)
    {
      return this.GetLight(p);
    }

    MapLight ITMMap.GetLightNoCache(GlobalPoint3D p)
    {
      return this.GetLightNoCache(p);
    }

    byte ITMMap.GetSunLight(GlobalPoint3D p)
    {
      return this.GetSunLight(p);
    }

    byte ITMMap.GetBlockLight(GlobalPoint3D p)
    {
      return this.GetBlockLight(p);
    }

    Vector2 ITMMap.GetSunAndBlockLightNormalized(GlobalPoint3D p)
    {
      return this.GetSunAndBlockLightNormalized(p);
    }

    MapLight ITMMap.GetMaxNeighbourLight(GlobalPoint3D p)
    {
      return this.GetMaxNeighbourLight(p);
    }

    MapLight ITMMap.GetMaxNeighbourLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      return this.GetMaxNeighbourLight(p, op);
    }

    byte ITMMap.GetMaxNeighbourSunLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      return this.GetMaxNeighbourSunLight(p, op);
    }

    byte ITMMap.GetMaxNeighbourBlockLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      return this.GetMaxNeighbourBlockLight(p, op);
    }

    bool ITMMap.CanBlockSeeTheSky(GlobalPoint3D p)
    {
      return this.CanBlockSeeTheSky(p);
    }

    float ITMMap.GetLightNormalized(byte light)
    {
      return this.GetLightNormalized(light);
    }

    float ITMMap.GetLightNormalized(GlobalPoint3D p)
    {
      return this.GetLightNormalized(p);
    }

    float ITMMap.GetLightNormalized(MapBlock data)
    {
      return this.GetLightNormalized(data);
    }

    float ITMMap.GetLightNormalized(MapLight data)
    {
      return this.GetLightNormalized(data);
    }

    float ITMMap.GetSunLightNormalized(GlobalPoint3D p)
    {
      return this.GetSunLightNormalized(p);
    }

    float ITMMap.GetBlockLightNormalized(GlobalPoint3D p)
    {
      return this.GetBlockLightNormalized(p);
    }

    ITMInventory ITMMap.GetBlockInventory(
      GlobalPoint3D p,
      GamerID gamerID,
      bool createIfNotExist)
    {
      MapStrategyTM mapStrategyTm = this.MapStrategyTM;
      if (mapStrategyTm != null)
      {
        DataBlock dataBlock = createIfNotExist ? mapStrategyTm.GetOrAddDataBlock(p, (Block) this.GetBlockIDNoCache(p), UpdateBlockMethod.Strategy, gamerID, true) : mapStrategyTm.GetDataBlock(p);
        if (dataBlock != null)
        {
          ChestBlock chestBlock = dataBlock as ChestBlock;
          if (chestBlock != null)
            return (ITMInventory) chestBlock.Inventory;
          FurnaceBlock furnaceBlock = dataBlock as FurnaceBlock;
          if (furnaceBlock != null)
            return (ITMInventory) furnaceBlock.Inventory;
        }
      }
      return (ITMInventory) null;
    }

    DataBlock ITMMap.GetOrAddDataBlock(GlobalPoint3D p)
    {
      if (this.MapStrategyTM != null)
        return this.MapStrategyTM.GetOrAddDataBlock(p, (Block) this.GetBlockIDNoCache(p), UpdateBlockMethod.Strategy, GamerID.Sys1, true);
      return (DataBlock) null;
    }

    public MapStrategyTM MapStrategyTM
    {
      get
      {
        return this.MapStrategy as MapStrategyTM;
      }
    }

    public int SignTextCacheCount
    {
      get
      {
        int num = 0;
        if (this.IsHost)
        {
          List<SignBlock> signBlocks = this.MapStrategyTM.SignBlocks;
          lock (this.SignTextCache)
          {
            if (signBlocks.Count > 0)
            {
              for (int index = 0; index < this.SignTextCache.Count; ++index)
              {
                string str = this.SignTextCache[index];
                if (str != null && str.Length > 0)
                {
                  if (SignBlock.IsTextUsed(signBlocks, index))
                    ++num;
                  else if (this.IsHost)
                    this.SignTextCache[index] = (string) null;
                }
              }
            }
          }
        }
        else
          num = this.SignTextCache.Count;
        return num;
      }
    }

    public int PendingChunkCount
    {
      get
      {
        lock (this.chunkPendingList)
          return this.chunkPendingList.Count;
      }
    }

    public override byte GetOpacity(byte blockID)
    {
      return this.BlockData[(int) blockID].Opacity;
    }

    public override byte GetLuminance(ref GlobalPoint3D p)
    {
      return this.GetLuminance(ref p, this.GetBlockID(p));
    }

    public override byte GetLuminance(ref GlobalPoint3D p, byte blockID)
    {
      if (blockID == (byte) 166 && !(this.MapStrategy as MapStrategyTM).IsBlockReceivingPower(p))
        return 0;
      return this.BlockData[(int) blockID].Luminance;
    }

    public override ushort GetBlastResistance(byte blockID)
    {
      return this.BlockData[(int) blockID].BlastResistance;
    }

    public override byte GetBlockBufferType(byte blockID)
    {
      return this.BlockData[(int) blockID].Buffer;
    }

    public override bool IsBlockLiquid(byte blockID)
    {
      if ((int) blockID != (int) this.WaterBlockID)
        return (int) blockID == (int) this.LavaBlockID;
      return true;
    }

    public override bool IsBlockLightSource(byte blockID)
    {
      return this.BlockData[(int) blockID].Luminance > (byte) 0;
    }

    public override bool IsBlockPassable(byte blockID)
    {
      return this.BlockData[(int) blockID].IsPassable;
    }

    public override bool IsBlockIcon(byte blockID)
    {
      return this.BlockData[(int) blockID].IsIcon;
    }

    public override bool IsBlockAttachable(byte blockID)
    {
      if (!this.BlockData[(int) blockID].IsAttached)
        return blockID == (byte) 145;
      return true;
    }

    public override bool IsBlockRotated(byte blockID)
    {
      if (this.BlockData[(int) blockID].IsRotated)
        return this.BlockData[(int) blockID].Buffer > (byte) 1;
      return false;
    }

    public override bool IsBlockSolid(byte blockID)
    {
      return this.BlockData[(int) blockID].Buffer < (byte) 2;
    }

    public override bool IsBlockOre(byte blockID)
    {
      return this.BlockData[(int) blockID].IsOreDeposit;
    }

    public MapTM(
      GameInstance instance,
      string name,
      float tileSize,
      bool isInfinite,
      BoxInt totalMapBound,
      BoxInt mapBound,
      Point3D regionSize,
      Point3D chunkSize,
      BlockDataXML[] blockData,
      int maxLight,
      int seed,
      ushort initialCacheCount,
      int cacheExpandSize,
      MapStrategy strategy,
      bool isHost,
      bool allowMeshCreatorToSplitOrFade)
      : base(name, tileSize, isInfinite, totalMapBound, mapBound, regionSize, chunkSize, maxLight, seed, (int) initialCacheCount, cacheExpandSize, strategy, isHost)
    {
      this.Instance = instance;
      this.BlockData = blockData;
      this.AllowMeshCreatorToSplitOrFade = allowMeshCreatorToSplitOrFade;
      this.TranslateDownOneTile = Matrix.CreateTranslation(0.0f, -tileSize, 0.0f);
      this.Initialize();
    }

    private void Initialize()
    {
      this.CanProcessPendingChunkTest = new ChunkTest(this.CanProcessPendingChunkTestCore);
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stairs), 0] = Block.Stairs;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stairs), 1] = Block.WoodPlank;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stairs), 2] = Block.ConcreteBrick;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stairs2), 0] = Block.Stairs2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.HalfBlock), 0] = Block.HalfBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.HalfBlock2), 0] = Block.HalfBlock2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Ramp), 0] = Block.Ramp;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Ramp2), 0] = Block.Ramp2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Sign), 0] = Block.Sign;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Fence), 0] = Block.Fence;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Cylinder), 0] = Block.Cylinder;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Obsidian), 0] = Block.Obsidian;
      this.BlockTextures[this.GetBlockTextureIndex(Block.MultiTextureBlock), 0] = Block.MultiTextureBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.MultiTextureBlock2), 0] = Block.MultiTextureBlock2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.LockedChest), 0] = Block.None;
      this.BlockTextures[this.GetBlockTextureIndex(Block.LockedDoorBottom), 0] = Block.None;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Table), 0] = Block.Table;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Painting), 0] = Block.None;
      this.BlockTextures[this.GetBlockTextureIndex(Block.PressurePlate), 0] = Block.PressurePlate;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Button), 0] = Block.Button;
      this.BlockTextures[this.GetBlockTextureIndex(Block.WifiTransmitter), 0] = Block.WifiTransmitter;
      this.BlockTextures[this.GetBlockTextureIndex(Block.WifiReceiver), 0] = Block.WifiReceiver;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ScriptBlock), 0] = Block.ScriptBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.NPCSpawn), 0] = Block.NPCSpawn;
      this.BlockTextures[this.GetBlockTextureIndex(Block.MobSpawn), 0] = Block.MobSpawn;
      this.BlockTextures[this.GetBlockTextureIndex(Block.TrapDoor), 0] = Block.TrapDoor;
      this.BlockTextures[this.GetBlockTextureIndex(Block.SteelSpikes), 0] = Block.ColorGray;
      this.BlockTextures[this.GetBlockTextureIndex(Block.OneWayGlass), 0] = Block.OneWayGlass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ProximityDetector), 0] = Block.ProximityDetector;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Teflon), 0] = Block.Teflon;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 0] = Block.StainedGlass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 1] = Block.Grass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 2] = Block.Dirt;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 3] = Block.Sand;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 4] = Block.Scoria;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 5] = Block.Wood;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 6] = Block.WoodPlank;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 7] = Block.WoodVeneer;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 8] = Block.Leaves;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 9] = Block.Glass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 10] = Block.Cloud;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 11] = Block.Water;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 12] = Block.Copper;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 13] = Block.Lava;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 14] = Block.Cassiterite;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlass), 15] = Block.Clay;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Pane), 0] = Block.Pane;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 0] = Block.StainedGlassPane;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 1] = Block.Grass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 2] = Block.Dirt;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 3] = Block.Sand;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 4] = Block.Scoria;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 5] = Block.Wood;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 6] = Block.WoodPlank;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 7] = Block.WoodVeneer;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 8] = Block.Leaves;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 9] = Block.Glass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 10] = Block.Cloud;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 11] = Block.Water;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 12] = Block.Copper;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 13] = Block.Lava;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 14] = Block.Cassiterite;
      this.BlockTextures[this.GetBlockTextureIndex(Block.StainedGlassPane), 15] = Block.Clay;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Post), 0] = Block.Post;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Post2), 0] = Block.Post2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Cloud), 0] = Block.Cloud;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ArcadeMachine), 0] = Block.ArcadeMachine;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ArcadeMachine), 1] = Block.Grass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ArcadeMachine), 2] = Block.Dirt;
      this.BlockTextures[this.GetBlockTextureIndex(Block.ParticleEmitter), 0] = Block.ParticleEmitter;
      this.BlockTextures[this.GetBlockTextureIndex(Block.SunBox), 0] = Block.SunBox;
      this.BlockTextures[this.GetBlockTextureIndex(Block.PoweredLight), 0] = Block.PoweredLight;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 0] = Block.CoverBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 1] = Block.Grass;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 2] = Block.Dirt;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 3] = Block.Sand;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 4] = Block.Scoria;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CoverBlock), 5] = Block.Wood;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stack), 0] = Block.Stack;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Stack2), 0] = Block.Stack2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.UpsideDownStack), 0] = Block.UpsideDownStack;
      this.BlockTextures[this.GetBlockTextureIndex(Block.zLastBlockID), 0] = Block.zLastBlockID;
      this.BlockTextures[this.GetBlockTextureIndex(Block.HealthBlock), 0] = Block.HealthBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.SidePost), 0] = Block.SidePost;
      this.BlockTextures[this.GetBlockTextureIndex(Block.SidePost2), 0] = Block.SidePost2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CornerBlock), 0] = Block.CornerBlock;
      this.BlockTextures[this.GetBlockTextureIndex(Block.CornerBlock2), 0] = Block.CornerBlock2;
      this.BlockTextures[this.GetBlockTextureIndex(Block.Console), 0] = Block.Console;
      this.BlockTextures[this.GetBlockTextureIndex(Block.TechLamp), 0] = Block.TechLamp;
      this.BlockTextures[this.GetBlockTextureIndex(Block.TechFurnace), 0] = Block.TechFurnace;
      this.BlockTextures[this.GetBlockTextureIndex(Block.PlasmaConduit), 0] = Block.PlasmaConduit;
    }

    protected override MapRegion CreateRegion()
    {
      return (MapRegion) new MapRegionTM();
    }

    protected override BlockFace GetFaceFromAux(byte blockID, byte auxFull)
    {
      byte num = (byte) ((uint) auxFull & 7U);
      Block block = (Block) blockID;
      if ((uint) block <= 121U)
      {
        switch (block)
        {
          case Block.Wisdom:
          case Block.Blueprint:
          case Block.Book:
            break;
          case Block.Sign:
            if (num >= (byte) 4)
              return (BlockFace) ((uint) num - 4U);
            return BlockFace.Up;
          default:
            goto label_11;
        }
      }
      else
      {
        switch (block)
        {
          case Block.Crop:
            break;
          case Block.SnowLayer:
            return BlockFace.Up;
          case Block.Painting:
            if (num <= (byte) 3)
              return (BlockFace) ((uint) num & 3U);
            return BlockFace.Up;
          default:
            goto label_11;
        }
      }
      return BlockFace.Up;
label_11:
      return (BlockFace) num;
    }

    protected override void SaveRegionCore(MapRegion region)
    {
      if (!this.IsHost)
        return;
      MapRegionTM region1 = region as MapRegionTM;
      if (region1 == null)
        return;
      if (Globals2.GameProperties.SaveGame.DirNumber == 0)
      {
        while (true)
        {
          try
          {
            MapSaver.SaveMapToFile(this.Instance, (IProgressBar) null, false);
            break;
          }
          catch (OtherDiskActivityInProgressException ex)
          {
            Thread.Sleep(1000);
          }
        }
      }
      try
      {
        MapSaver.SaveRegion(this.Instance, region1, (IProgressBar) null);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(113, ex);
      }
    }

    protected override void LoadRegionCore(MapRegion region)
    {
      MapRegionTM region1 = region as MapRegionTM;
      if (region1 == null)
        return;
      new MapRegionLoader().LoadRegion(region1, (IProgressBar) null);
    }

    protected override void OnMapShiftEnd(BlockFace direction)
    {
      base.OnMapShiftEnd(direction);
      this.AllChunksOutsideViewGenerated = false;
    }

    public bool IsChunkPending(MapChunk chunk)
    {
      if (chunk == null)
        return false;
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.chunkPendingList)
        return this.chunkPendingList.ContainsKey(globalHashCode);
    }

    public void AddChunkPendingData(MapChunk chunk, MapChunkPendingData data)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.chunkPendingList)
      {
        if (!this.chunkPendingList.ContainsKey(globalHashCode))
          this.chunkPendingList.Add(globalHashCode, data);
        else
          this.chunkPendingList[globalHashCode] = data;
      }
    }

    public bool GetChunkPendingData(MapChunk chunk, out MapChunkPendingData data)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.chunkPendingList)
        return this.chunkPendingList.TryGetValue(globalHashCode, out data);
    }

    public bool GetAndRemoveChunkPendingData(MapChunk chunk, out MapChunkPendingData data)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.chunkPendingList)
      {
        if (!this.chunkPendingList.TryGetValue(globalHashCode, out data))
          return false;
        this.chunkPendingList.Remove(globalHashCode);
        return true;
      }
    }

    public void RemoveChunkPendingData(MapChunk chunk)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.chunkPendingList)
        this.chunkPendingList.Remove(globalHashCode);
    }

    private bool CanProcessPendingChunkTestCore(MapChunk chunk)
    {
      if (chunk != null)
        return chunk.IsDecoratedWithoutReceivedCheck;
      return true;
    }

    public MapBlock GetBlockIDAndAuxFromPending(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region != null)
      {
        MapChunk chunk = region.GetChunk(p);
        if (chunk != null)
        {
          Point3D localPoint = chunk.GetLocalPoint(p);
          int mapIndex = chunk.GetMapIndex(localPoint);
          long globalHashCode = chunk.GetGlobalHashCode();
          lock (this.chunkPendingList)
          {
            MapChunkPendingData chunkPendingData;
            if (this.chunkPendingList.TryGetValue(globalHashCode, out chunkPendingData))
            {
              MapBlock mapBlock = new MapBlock();
              mapBlock.Chunk = chunk;
              this.tempRLEStream.StreamID = chunkPendingData.BlockData.StreamID;
              this.tempRLEStream.StreamIndex = chunkPendingData.BlockData.StreamIndex;
              this.tempRLEStream.StreamSize = chunkPendingData.BlockData.StreamSize;
              mapBlock.BlockID = this.tempRLEStream.GetDataNoCacheUnsafe(mapIndex);
              this.tempRLEStream.StreamID = chunkPendingData.AuxData.StreamID;
              this.tempRLEStream.StreamIndex = chunkPendingData.AuxData.StreamIndex;
              this.tempRLEStream.StreamSize = chunkPendingData.AuxData.StreamSize;
              mapBlock.AuxData = this.tempRLEStream.GetDataNoCacheUnsafe(mapIndex);
              return mapBlock;
            }
          }
          return chunk.GetBlockIDAndAuxNoCache(mapIndex);
        }
      }
      return this.BuildBlockData((MapChunk) null, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
    }

    public void LoadDataBlocksFromChunkDataAndDoAnyChunkDataConversion(
      MapChunk chunk,
      NetworkGamer sender)
    {
      int next1 = this.dataBlockIndexListPool.GetNext();
      List<int> torchList = this.dataBlockIndexListPool.List[next1];
      torchList.Clear();
      int next2 = this.dataBlockIndexListPool.GetNext();
      List<int> dataBlockList = this.dataBlockIndexListPool.List[next2];
      dataBlockList.Clear();
      int next3 = this.dataBlocksPostLoadPool.GetNext();
      List<DataBlock> dataBlocksPostLoad = this.dataBlocksPostLoadPool.List[next3];
      dataBlocksPostLoad.Clear();
      try
      {
        this.LoadDataBlocksFromChunkDataAndDoAnyChunkDataConversionCore(chunk, sender, torchList, dataBlockList, dataBlocksPostLoad);
      }
      finally
      {
        this.dataBlocksPostLoadPool.Release(next3);
        this.dataBlockIndexListPool.Release(next2);
        this.dataBlockIndexListPool.Release(next1);
      }
    }

    private void LoadDataBlocksFromChunkDataAndDoAnyChunkDataConversionCore(
      MapChunk chunk,
      NetworkGamer sender,
      List<int> torchList,
      List<int> dataBlockList,
      List<DataBlock> dataBlocksPostLoad)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      int num1 = 0;
      byte num2 = 0;
      int y = chunk.GlobalOffset.Y;
      byte num3 = 29;
      byte num4 = 117;
      bool flag1 = false;
      bool flag2 = true;
      byte num5 = 137;
      bool isHostLoading = sender == null && this.IsHost;
      MapStrategyTM mapStrategy = this.MapStrategy as MapStrategyTM;
      int saveVersion = Globals2.GameProperties.SaveGame.Header.SaveVersion;
      lock (mapStrategy.DataBlocks)
      {
        lock (chunk.RleLock)
        {
          RLEStreamByte blockData = chunk.BlockData;
          int streamId = (int) blockData.StreamID;
          int streamIndex = blockData.StreamIndex;
          int streamSize = blockData.StreamSize;
          byte[] numArray = Map.RLEStreamBufferManager.Stream[streamId];
          bool flag3 = true;
          bool flag4 = true;
          BlockDataXML blockDataXml = this.BlockData[0];
          for (int index1 = 0; index1 < streamSize; index1 += 2)
          {
            int count = (int) numArray[streamIndex + index1] + 1;
            byte num6 = numArray[streamIndex + index1 + 1];
            if ((int) num6 != (int) num2)
            {
              blockDataXml = this.BlockData[(int) num6];
              flag2 = Globals1.ItemData[(int) num6].IsEnabled;
              if (!flag2)
                flag2 = ((int) chunk.AuxData.GetDataNoCache(chunk, num1) & 8) == 0;
            }
            if (num6 != (byte) 0)
            {
              flag3 = false;
              if (!flag2 || (int) num6 == (int) num3 && y > 0)
              {
                numArray[streamIndex + index1 + 1] = (byte) 0;
                num6 = (byte) 0;
                blockDataXml.Buffer = (byte) 3;
                if (blockDataXml.ClassType != DataBlockType.None)
                  this.MapStrategyTM.RemoveDataBlock(this.GetGlobalHashCode(chunk.GlobalOffset + chunk.GetPoint(num1)));
              }
              else
              {
                if (isHostLoading)
                {
                  if (saveVersion < 180)
                  {
                    switch (num6)
                    {
                      case 119:
                        for (int index2 = 0; index2 < count; ++index2)
                        {
                          if (chunk.AuxData.GetData(chunk, num1 + index2) == (byte) 0)
                            chunk.AuxData.SetDataNoLock(chunk, num1 + index2, (byte) 4);
                        }
                        break;
                      case 121:
                        for (int index2 = 0; index2 < count; ++index2)
                        {
                          if (mapStrategy != null)
                          {
                            BookBlock orAddDataBlock = mapStrategy.GetOrAddDataBlock((GlobalPoint3D) chunk.GetPoint(num1 + index2) + chunk.GlobalOffset, Block.Book, UpdateBlockMethod.Strategy, GamerID.Sys1, true) as BookBlock;
                            int num7 = (int) chunk.AuxData.GetData(chunk, num1 + index2) & 247;
                            if (num7 > 0)
                              orAddDataBlock.ID = (ushort) MapTM.GetIDFromAux((byte) num7);
                          }
                          chunk.AuxData.SetDataNoLock(chunk, num1 + index2, (byte) 0);
                        }
                        break;
                    }
                  }
                  if (saveVersion < 257 && num6 == (byte) 175)
                  {
                    for (int index2 = 0; index2 < count; ++index2)
                    {
                      if (mapStrategy != null)
                      {
                        GlobalPoint3D p = (GlobalPoint3D) chunk.GetPoint(num1 + index2) + chunk.GlobalOffset;
                        NpcSpawnBlock orAddDataBlock = mapStrategy.GetOrAddDataBlock(p, Block.NPCSpawn, UpdateBlockMethod.Strategy, GamerID.Sys1, true) as NpcSpawnBlock;
                        NpcSpawnBlock npcSpawnBlock = mapStrategy.AddNpcSpawnBlock(p, UpdateBlockMethod.Strategy);
                        npcSpawnBlock.ActorType = orAddDataBlock.ActorType;
                        npcSpawnBlock.MaxActiveInstances = 1;
                        npcSpawnBlock.SpawnFrequency = 5f;
                        npcSpawnBlock.Name = orAddDataBlock.Name;
                        npcSpawnBlock.OwnerGamertag = orAddDataBlock.OwnerGamertag;
                        npcSpawnBlock.OwnerHasAvatarUnlocked = orAddDataBlock.OwnerHasAvatarUnlocked;
                        npcSpawnBlock.ShowOwnerData = orAddDataBlock.ShowOwnerData;
                        npcSpawnBlock.BehaviourTree = "System\\AI\\Default";
                        mapStrategy.ReplacePendingDataBlock(globalHashCode, (DataBlock) npcSpawnBlock);
                      }
                      chunk.BlockData.SetDataNoLock(chunk, num1 + index2, num5);
                    }
                  }
                }
                if (blockDataXml.ClassType != DataBlockType.None)
                {
                  this.LoadDataBlockFromStream(globalHashCode, (Block) num6, num1, count, torchList, dataBlockList, dataBlocksPostLoad, isHostLoading);
                  if (!flag1 && (int) num6 == (int) num4)
                    flag1 = true;
                }
              }
            }
            if (flag4 && blockDataXml.Buffer > (byte) 1)
              flag4 = false;
            num1 += count;
            num2 = num6;
          }
          if (flag3)
            chunk.SetChunkFlag(ChunkFlags.ChunkIsAllAir);
          else
            chunk.ClearChunkFlag(ChunkFlags.ChunkIsAllAir);
          if (flag4)
            chunk.SetChunkFlag(ChunkFlags.ChunkIsAllSolid);
          else
            chunk.ClearChunkFlag(ChunkFlags.ChunkIsAllSolid);
        }
      }
      this.PostLoadDataBlocks(dataBlocksPostLoad);
      if (flag1 && this.Instance != null && this.Instance.MapRenderer != null)
        this.Instance.MapRenderer.SignsChanged(false);
      this.Instance.NetworkManager.SendDataBlockInfoRequest(chunk, dataBlockList, sender, false);
      torchList.Clear();
      dataBlockList.Clear();
    }

    private void LoadDataBlockFromStream(
      long chunkHash,
      Block blockID,
      int index,
      int count,
      List<int> torchList,
      List<int> dataBlockList,
      List<DataBlock> dataBlocksPostLoad,
      bool isHostLoading)
    {
      switch (blockID)
      {
        case Block.Torch:
          for (int index1 = 0; index1 < count; ++index1)
            torchList.Add(index + index1);
          break;
        case Block.Painting:
          if (this.IsHost)
            break;
          for (int index1 = 0; index1 < count; ++index1)
            this.LoadPainting(chunkHash, index + index1);
          break;
        default:
          if (isHostLoading)
          {
            if (blockID != Block.NPCSpawn)
              break;
            MapChunk chunk = this.GetChunk(chunkHash);
            for (int index1 = 0; index1 < count; ++index1)
            {
              DataBlock dataBlock = this.MapStrategyTM.GetDataBlock(chunk.GetGlobalPoint(chunk.GetPoint(index + index1)));
              if (dataBlock != null)
                dataBlocksPostLoad.Add(dataBlock);
            }
            break;
          }
          for (int index1 = 0; index1 < count; ++index1)
            dataBlockList.Add(index + index1);
          break;
      }
    }

    private void LoadPainting(long chunkHash, int index)
    {
      byte auxHighDataNoCache = this.GetChunk(chunkHash).GetAUXHighDataNoCache(index);
      if (auxHighDataNoCache <= (byte) 0)
        return;
      this.Instance.NetworkManager.SendPhotoThumbnailRequest(auxHighDataNoCache);
    }

    private void PostLoadDataBlocks(List<DataBlock> dataBlocksPostLoad)
    {
    }

    public override bool IsBlockAffectSunlightForHeightCalculation(byte blockID)
    {
      return !this.BlockData[(int) blockID].IsVertSunlightUnhindered;
    }

    protected override void BlastEdgeCleared(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (method == UpdateBlockMethod.Generation)
        return;
      ++p.Y;
      --p.X;
      this.AddFireFromBlast(p);
      --p.Z;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
      --p.Z;
      --p.Y;
      ++p.X;
      --p.X;
      this.AddFireFromBlast(p);
      --p.Z;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
      --p.Z;
      --p.Y;
      ++p.X;
      --p.X;
      this.AddFireFromBlast(p);
      --p.Z;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.X;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      ++p.Z;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
      --p.X;
      this.AddFireFromBlast(p);
    }

    private void AddFireFromBlast(GlobalPoint3D p)
    {
      GlobalPoint3D fp = p;
      byte blockId = this.GetBlockID(p);
      if (ItemData2.GetBurnTime(this, p, (Item) blockId) <= (ushort) 0)
        return;
      --fp.X;
      if (this.StartFireFromBlast(p, blockId, fp))
        return;
      ++fp.X;
      --fp.Z;
      if (this.StartFireFromBlast(p, blockId, fp))
        return;
      ++fp.X;
      ++fp.Z;
      if (this.StartFireFromBlast(p, blockId, fp))
        return;
      --fp.X;
      ++fp.Z;
      if (this.StartFireFromBlast(p, blockId, fp))
        return;
      --fp.Z;
      ++fp.Y;
      if (this.StartFireFromBlast(p, blockId, fp))
        return;
      --fp.Y;
      --fp.Y;
      this.StartFireFromBlast(p, blockId, fp);
    }

    private bool StartFireFromBlast(GlobalPoint3D p, byte blockID, GlobalPoint3D fp)
    {
      if (this.GetBlockID(fp) != (byte) 0)
        return false;
      this.Instance.StartLiveFire(p, (Block) blockID, fp, UpdateBlockMethod.Blast, GamerID.Sys1, false);
      return true;
    }

    protected override void PreCopy(
      Map srcMap,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      base.PreCopy(srcMap, facing, method, playerID);
      this.copiedDataBlocks.Clear();
    }

    public override void RotateBlock(ref MapBlock blockData, int facing, bool from)
    {
      if (facing == 0 || !this.BlockData[(int) blockData.BlockID].IsOrientated)
        return;
      if (this.BlockData[(int) blockData.BlockID].IsRotated)
      {
        byte buffer = this.BlockData[(int) blockData.BlockID].Buffer;
        if (buffer > (byte) 1)
        {
          Block blockId = (Block) blockData.BlockID;
          bool flag = ItemData.IsSubType((Item) blockId, ItemSubType.Door);
          if (!flag)
          {
            Block block = blockId;
            if ((uint) block <= 117U)
            {
              if (block != Block.Stairs && block != Block.Sign)
                goto label_11;
            }
            else
            {
              switch (block)
              {
                case Block.Ramp:
                case Block.Painting:
                case Block.Stairs2:
                case Block.Ramp2:
                  break;
                default:
                  goto label_11;
              }
            }
            flag = true;
          }
label_11:
          if (flag)
          {
            int num = (((int) blockData.AuxData & 3) + facing & 3) + ((int) blockData.AuxData & 252);
            blockData.AuxData = (byte) num;
          }
          else
          {
            int num1 = (int) blockData.AuxData & 7;
            if (num1 >= 4)
              return;
            int num2 = (num1 + facing & 3) + ((int) blockData.AuxData & 252);
            blockData.AuxData = (byte) num2;
          }
        }
        else
        {
          if (buffer != (byte) 0)
            return;
          int num1 = (int) blockData.AuxData & 3;
          if (num1 <= 0)
            return;
          int num2 = (num1 + facing) % 2 + ((int) blockData.AuxData & 252);
          blockData.AuxData = (byte) num2;
        }
      }
      else
      {
        int num1 = (int) blockData.AuxData & 7;
        int num2 = (int) blockData.AuxData & 248;
        switch ((Block) blockData.BlockID)
        {
          case Block.Post:
          case Block.Post2:
            if (num1 <= 0)
              break;
            int num3 = num1 + facing;
            if (num3 > 4)
              num3 -= 4;
            blockData.AuxData = (byte) (num2 + num3);
            break;
          case Block.SidePost:
          case Block.SidePost2:
            switch (facing)
            {
              case 1:
                switch (num1)
                {
                  case 0:
                    num1 = 5;
                    break;
                  case 1:
                    num1 = 4;
                    break;
                  case 2:
                    num1 = 7;
                    break;
                  case 3:
                    num1 = 6;
                    break;
                  case 4:
                    num1 = 0;
                    break;
                  case 5:
                    num1 = 1;
                    break;
                  case 6:
                    num1 = 2;
                    break;
                  case 7:
                    num1 = 3;
                    break;
                }
                break;
              case 2:
                switch (num1)
                {
                  case 0:
                    num1 = 1;
                    break;
                  case 1:
                    num1 = 0;
                    break;
                  case 2:
                    num1 = 3;
                    break;
                  case 3:
                    num1 = 2;
                    break;
                  case 4:
                    num1 = 5;
                    break;
                  case 5:
                    num1 = 4;
                    break;
                  case 6:
                    num1 = 7;
                    break;
                  case 7:
                    num1 = 6;
                    break;
                }
                break;
              default:
                switch (num1)
                {
                  case 0:
                    num1 = 4;
                    break;
                  case 1:
                    num1 = 5;
                    break;
                  case 2:
                    num1 = 6;
                    break;
                  case 3:
                    num1 = 7;
                    break;
                  case 4:
                    num1 = 1;
                    break;
                  case 5:
                    num1 = 0;
                    break;
                  case 6:
                    num1 = 3;
                    break;
                  case 7:
                    num1 = 2;
                    break;
                }
                break;
            }
            blockData.AuxData = (byte) (num2 + num1);
            break;
          case Block.CornerBlock:
          case Block.CornerBlock2:
            int num4 = (num1 & 3) + facing & 3;
            if (num1 > 3)
              num4 += 4;
            blockData.AuxData = (byte) (num2 + num4);
            break;
        }
      }
    }

    protected override void AdjustBlockDataForMove(ref MapBlock blockData)
    {
      StudioForge.TotalMiner.BlockData.AdjustBlockDataForMove(ref blockData);
    }

    protected override MapChunk SetCopySameBlockData(
      GlobalPoint3D p,
      byte blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (this.BlockData[(int) blockID].ClassType != DataBlockType.None)
      {
        MapChunk chunk = this.GetChunk(p);
        if (chunk != null)
        {
          MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
          MapBlock newBlock = new MapBlock()
          {
            BlockID = blockID,
            AuxData = auxData
          };
          blockIdAndAux.Chunk = newBlock.Chunk = chunk;
          this.MapStrategy.BlockChanged(p, blockIdAndAux, newBlock, method, playerID, transmit);
          return chunk;
        }
      }
      return (MapChunk) null;
    }

    protected override void CopyToSetBlock(
      Map srcMap,
      GlobalPoint3D srcPoint,
      GlobalPoint3D p,
      byte blockID,
      byte auxData,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (blockID <= (byte) 0)
        return;
      MapStrategyTM mapStrategy = srcMap.MapStrategy as MapStrategyTM;
      if (mapStrategy == null)
        return;
      DataBlock dataBlock = mapStrategy.GetDataBlock(srcPoint);
      if (dataBlock == null)
        return;
      DataBlock orAddDataBlock = this.MapStrategyTM.GetOrAddDataBlock(p, (Block) blockID, method, playerID, true);
      if (orAddDataBlock == null)
        return;
      this.UpdateNewDataBlockFromSrc(p, dataBlock, orAddDataBlock, playerID);
      this.SetNewBlockDataForCopy(srcMap, orAddDataBlock, p, method);
    }

    private void UpdateNewDataBlockFromSrc(
      GlobalPoint3D p,
      DataBlock srcBlock,
      DataBlock newBlock,
      GamerID playerID)
    {
      newBlock.CopyFrom(srcBlock);
      newBlock.Point = p;
      PlayerBlock playerBlock = newBlock as PlayerBlock;
      if (playerBlock != null)
      {
        Player player = this.Instance.GetPlayer(playerID);
        if (player != null)
          playerBlock.Gamertag = player.Gamertag;
      }
      if (newBlock.ClassType != DataBlockType.Furnace)
        return;
      (newBlock as FurnaceBlock).Inventory.ClearItems();
    }

    private void SetNewBlockDataForCopy(
      Map srcMap,
      DataBlock newBlock,
      GlobalPoint3D p,
      UpdateBlockMethod method)
    {
      if (newBlock == null)
        return;
      switch (newBlock.ClassType)
      {
        case DataBlockType.AmbientSound:
          if (method != UpdateBlockMethod.Paste)
            break;
          this.Instance.AmbientSoundManager.SetBlock(newBlock as AmbientSoundBlock);
          break;
        case DataBlockType.NPCSpawn:
          if (method != UpdateBlockMethod.Paste)
            break;
          this.Instance.NpcManager.NpcSpawnAdded(newBlock as NpcSpawnBlock);
          break;
        case DataBlockType.Sign:
          this.SetNewSignDataFromCopy(srcMap as MapTM, newBlock as SignBlock);
          break;
        case DataBlockType.WifiReceiver:
          if (method != UpdateBlockMethod.Paste || !this.IsHost)
            break;
          this.copiedDataBlocks.Add(newBlock);
          break;
        case DataBlockType.WifiTransmitter:
          if (method != UpdateBlockMethod.Paste || !this.IsHost)
            break;
          this.copiedDataBlocks.Add(newBlock);
          break;
      }
    }

    private void SetNewSignDataFromCopy(MapTM srcMap, SignBlock signBlock)
    {
      if (srcMap == null || signBlock == null)
        return;
      signBlock.Text1 = this.AddNewSignTextFromCopy(srcMap, signBlock.Text1);
      signBlock.Text2 = this.AddNewSignTextFromCopy(srcMap, signBlock.Text2);
      signBlock.Text3 = this.AddNewSignTextFromCopy(srcMap, signBlock.Text3);
      signBlock.Text4 = this.AddNewSignTextFromCopy(srcMap, signBlock.Text4);
    }

    private short AddNewSignTextFromCopy(MapTM srcMap, short textIndex)
    {
      if (textIndex < (short) 0 || (int) textIndex >= srcMap.SignTextCache.Count)
        return -1;
      string str = srcMap.SignTextCache[(int) textIndex];
      textIndex = (short) this.SignTextCache.IndexOf(str);
      if (textIndex == (short) -1)
      {
        lock (this.SignTextCache)
        {
          this.SignTextCache.Add(str);
          textIndex = (short) (this.SignTextCache.Count - 1);
        }
        this.SignTextCacheChanged = true;
      }
      this.Instance.MapRenderer.SignsChanged(false);
      return textIndex;
    }

    protected override void PostCopy(
      Map srcMap,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (this.MapStrategyTM != null)
      {
        switch (method)
        {
          case UpdateBlockMethod.Copy:
            this.CompressTransmitterFrequencies(srcMap);
            break;
          case UpdateBlockMethod.Paste:
            if (this.IsHost)
            {
              this.UpdateWifiFrequencies(this.copiedDataBlocks);
              break;
            }
            break;
        }
      }
      this.copiedDataBlocks.Clear();
    }

    protected override bool CanCopy(ref MapBlock blockData)
    {
      if (blockData.BlockID != (byte) 29)
        return Globals1.ItemData[(int) blockData.BlockID].IsEnabled;
      return false;
    }

    public void UpdateWifiFrequencies(List<DataBlock> datablocks)
    {
      if (this.transmittersPasteTemp == null)
        this.transmittersPasteTemp = new List<MapTM.TransmitterLinkInfo>();
      if (this.receiversPasteTemp == null)
        this.receiversPasteTemp = new List<WifiReceiverBlock>();
      foreach (DataBlock datablock in datablocks)
        this.AddWifiBlockFrequencyUpdateData(datablock);
      this.RelinkReceiversToTransmitters();
      this.transmittersPasteTemp.Clear();
      this.receiversPasteTemp.Clear();
    }

    private void AddWifiBlockFrequencyUpdateData(DataBlock block)
    {
      switch (block.ClassType)
      {
        case DataBlockType.WifiReceiver:
          this.receiversPasteTemp.Add(block as WifiReceiverBlock);
          break;
        case DataBlockType.WifiTransmitter:
          WifiTransmitterBlock transmitterBlock = block as WifiTransmitterBlock;
          ushort transmitterFrequency = this.MapStrategyTM.GetNextFreeTransmitterFrequency();
          this.transmittersPasteTemp.Add(new MapTM.TransmitterLinkInfo()
          {
            OldFreq = transmitterBlock.Frequency,
            NewFreq = transmitterFrequency
          });
          transmitterBlock.Frequency = transmitterFrequency;
          break;
      }
    }

    private void RelinkReceiversToTransmitters()
    {
      if (this.receiversPasteTemp.Count <= 0 || this.transmittersPasteTemp.Count <= 0)
        return;
      foreach (WifiReceiverBlock wifiReceiverBlock in this.receiversPasteTemp)
      {
        bool flag1 = false;
        bool flag2 = false;
        foreach (MapTM.TransmitterLinkInfo transmitterLinkInfo in this.transmittersPasteTemp)
        {
          if (!flag1 && (int) wifiReceiverBlock.Frequency1 == (int) transmitterLinkInfo.OldFreq)
          {
            wifiReceiverBlock.Frequency1 = transmitterLinkInfo.NewFreq;
            flag1 = true;
          }
          if (!flag2 && (int) wifiReceiverBlock.Frequency2 == (int) transmitterLinkInfo.OldFreq)
          {
            wifiReceiverBlock.Frequency2 = transmitterLinkInfo.NewFreq;
            flag2 = true;
          }
        }
        if (!flag1)
          wifiReceiverBlock.Frequency1 = (ushort) 0;
        if (!flag2)
          wifiReceiverBlock.Frequency2 = (ushort) 0;
      }
    }

    private void CompressTransmitterFrequencies(Map srcMap)
    {
      foreach (DataBlock dataBlock in this.MapStrategyTM.DataBlocks.Values)
      {
        WifiTransmitterBlock transmitterBlock = dataBlock as WifiTransmitterBlock;
        if (transmitterBlock != null)
        {
          if (this.transmittersCopyTemp == null)
            this.transmittersCopyTemp = new List<WifiTransmitterBlock>();
          this.transmittersCopyTemp.Add(transmitterBlock);
        }
        else
        {
          WifiReceiverBlock wifiReceiverBlock = dataBlock as WifiReceiverBlock;
          if (wifiReceiverBlock != null)
          {
            if (this.receiversCopyTemp == null)
              this.receiversCopyTemp = new List<MapTM.ReceiverReLinkInfo>();
            this.receiversCopyTemp.Add(new MapTM.ReceiverReLinkInfo()
            {
              Receiver = wifiReceiverBlock
            });
          }
        }
      }
      if (this.transmittersCopyTemp == null || this.transmittersCopyTemp.Count <= 0)
        return;
      this.transmittersCopyTemp.Sort(new Comparison<WifiTransmitterBlock>(this.SortTransmitterByFrequency));
      for (int index = 0; index < this.transmittersCopyTemp.Count; ++index)
      {
        WifiTransmitterBlock transmitterBlock = this.transmittersCopyTemp[index];
        if (transmitterBlock.Frequency > (ushort) 0)
        {
          ushort newFreq = (ushort) (index + 1);
          this.AdjustMatchingReceivers(transmitterBlock.Frequency, newFreq);
          transmitterBlock.Frequency = newFreq;
        }
      }
      this.transmittersCopyTemp.Clear();
      if (this.receiversCopyTemp == null)
        return;
      this.DisableAllUnlinkedReceivers();
      this.receiversCopyTemp.Clear();
    }

    private void AdjustMatchingReceivers(ushort oldFreq, ushort newFreq)
    {
      if (this.receiversCopyTemp == null)
        return;
      for (int index = 0; index < this.receiversCopyTemp.Count; ++index)
      {
        MapTM.ReceiverReLinkInfo receiverReLinkInfo = this.receiversCopyTemp[index];
        if ((int) receiverReLinkInfo.Receiver.Frequency1 == (int) oldFreq)
        {
          receiverReLinkInfo.Receiver.Frequency1 = newFreq;
          receiverReLinkInfo.Freq1Adjusted = true;
        }
        if ((int) receiverReLinkInfo.Receiver.Frequency2 == (int) oldFreq)
        {
          receiverReLinkInfo.Receiver.Frequency2 = newFreq;
          receiverReLinkInfo.Freq2Adjusted = true;
        }
        this.receiversCopyTemp[index] = receiverReLinkInfo;
      }
    }

    private void DisableAllUnlinkedReceivers()
    {
      foreach (MapTM.ReceiverReLinkInfo receiverReLinkInfo in this.receiversCopyTemp)
      {
        if (!receiverReLinkInfo.Freq1Adjusted)
          receiverReLinkInfo.Receiver.Frequency1 = (ushort) 0;
        if (!receiverReLinkInfo.Freq2Adjusted)
          receiverReLinkInfo.Receiver.Frequency2 = (ushort) 0;
      }
    }

    private int SortTransmitterByFrequency(WifiTransmitterBlock t1, WifiTransmitterBlock t2)
    {
      return t1.Frequency.CompareTo(t2.Frequency);
    }

    public static void GetBiome(
      BiomeType biomeType,
      out TerrainGeneratorBase biome,
      out int handle)
    {
      lock (MapTM.biomePoolLock)
      {
        handle = -1;
        biome = (TerrainGeneratorBase) null;
        int num = 10;
label_2:
        try
        {
          switch (biomeType)
          {
            case BiomeType.Flat:
              handle = FlatBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) FlatBiome.Pool.List[handle];
              break;
            case BiomeType.Desert:
              handle = DesertBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) DesertBiome.Pool.List[handle];
              break;
            case BiomeType.Grasslands:
              handle = GrasslandsBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) GrasslandsBiome.Pool.List[handle];
              break;
            case BiomeType.SemiAlphine:
              handle = SemiAlpineBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) SemiAlpineBiome.Pool.List[handle];
              break;
            case BiomeType.DigDeep:
              if (Globals2.GameProperties.SaveGame.Header.SaveVersion > 291)
              {
                handle = DigDeepBiome2.Pool.GetNext();
                biome = (TerrainGeneratorBase) DigDeepBiome2.Pool.List[handle];
                break;
              }
              handle = DigDeepBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) DigDeepBiome.Pool.List[handle];
              break;
            case BiomeType.Infinite:
              handle = InfiniteBiome.Pool.GetNext();
              biome = (TerrainGeneratorBase) InfiniteBiome.Pool.List[handle];
              break;
          }
        }
        catch (CoreException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(89, (Exception) ex);
          if (--num <= 0)
            throw new CoreException("Could not allocate biome");
          goto label_2;
        }
      }
    }

    public static void ReleaseBiome(BiomeType biomeType, TerrainGeneratorBase biome, int handle)
    {
      lock (MapTM.biomePoolLock)
      {
        switch (biomeType)
        {
          case BiomeType.Flat:
            FlatBiome.Pool.Release(handle);
            break;
          case BiomeType.Desert:
            DesertBiome.Pool.Release(handle);
            break;
          case BiomeType.Grasslands:
            GrasslandsBiome.Pool.Release(handle);
            break;
          case BiomeType.SemiAlphine:
            SemiAlpineBiome.Pool.Release(handle);
            break;
          case BiomeType.DigDeep:
            if (Globals2.GameProperties.SaveGame.Header.SaveVersion > 291)
            {
              DigDeepBiome2.Pool.Release(handle);
              break;
            }
            DigDeepBiome.Pool.Release(handle);
            break;
          case BiomeType.Infinite:
            InfiniteBiome.Pool.Release(handle);
            break;
        }
      }
    }

    public bool HasDecal(GlobalPoint3D p)
    {
      if (this.BlockData[(int) this.GetBlockIDNoCache(p)].Buffer == (byte) 0)
        return this.GetAuxHighDataNoCache(p) > (byte) 0;
      return false;
    }

    public int GetBlockTextureIndex(Block blockID)
    {
      return (int) this.BlockData[(int) blockID].TextureID - 1;
    }

    public bool UsesBlockTextureTable(Item blockID)
    {
      Item idForTextureIndex = this.Instance.ConvertItemIDToBlockIDForTextureIndex(blockID);
      if (idForTextureIndex > Item.zLastBlockID)
        return false;
      return this.GetBlockTextureIndex((Block) idForTextureIndex) >= 0;
    }

    public bool UsesBlockTextureTable(Block blockID)
    {
      return this.GetBlockTextureIndex(blockID) >= 0;
    }

    public MapTM.BlockTextureChangeResult ChangeBlockTexture(
      Player player,
      GlobalPoint3D p,
      Block blockID,
      Block textureID)
    {
      MapTM.BlockTextureChangeResult textureChangeResult = MapTM.BlockTextureChangeResult.None;
      MapStrategyTM mapStrategy = this.MapStrategy as MapStrategyTM;
      if (mapStrategy != null && !mapStrategy.IsInZoneType(p, ZoneType.NoEdit, player.GamerID))
      {
        if (this.IsHost)
          textureChangeResult = this.ChangeBlockTextureFromHost(player, p, blockID, textureID);
        this.Instance.NetworkManager.SendBlockTextureChange(player, p, blockID, textureID);
      }
      return textureChangeResult;
    }

    public MapTM.BlockTextureChangeResult ChangeBlockTextureFromHost(
      Player player,
      GlobalPoint3D point,
      Block blockID,
      Block textureID)
    {
      MapTM.BlockTextureChangeResult textureChangeResult = MapTM.BlockTextureChangeResult.None;
      int blockTextureIndex = this.GetBlockTextureIndex(blockID);
      if (blockTextureIndex >= 0)
      {
        textureChangeResult = MapTM.BlockTextureChangeResult.ExistingTextureUsed;
        int texture = 1;
        if (textureID == Block.None || blockID == textureID && textureID != Block.Painting)
        {
          texture = 0;
        }
        else
        {
          if (blockID == Block.SteelSpikes)
            texture = 0;
          int num = texture;
          while (texture < 16 && this.BlockTextures[blockTextureIndex, texture] != textureID)
            ++texture;
          if (texture == 16)
          {
            texture = num;
            while (texture < 16 && this.BlockTextures[blockTextureIndex, texture] != Block.None)
              ++texture;
            if (texture == 16)
              return MapTM.BlockTextureChangeResult.None;
          }
          if (this.BlockTextures[blockTextureIndex, texture] == Block.None)
          {
            this.BlockTextures[blockTextureIndex, texture] = textureID;
            textureChangeResult = MapTM.BlockTextureChangeResult.NewTextureSelected;
          }
        }
        player?.SetCurrentBlockTexture(blockID, texture);
        byte auxFullData = this.GetAuxFullData(point);
        byte oldAuxData = auxFullData;
        byte auxData = (byte) ((uint) (byte) ((uint) auxFullData & 15U) + (uint) (byte) (texture << 4));
        this.SetAuxData(point, oldAuxData, auxData, UpdateBlockMethod.Player, Player.GetGamerID(player), false);
        this.Commit();
      }
      return textureChangeResult;
    }

    private bool DoesBlockUsesKey(Block blockID)
    {
      switch (blockID)
      {
        case Block.LockedChest:
        case Block.LockedDoorTop:
        case Block.LockedDoorBottom:
          return true;
        default:
          return false;
      }
    }

    public void SetBlockTexture(Block block, int index, Block texture)
    {
      int blockTextureIndex = this.GetBlockTextureIndex(block);
      if (blockTextureIndex < 0)
        return;
      this.BlockTextures[blockTextureIndex, index] = texture;
    }

    public Block GetBlockTextureIDForDrawing(Block blockID, GlobalPoint3D p)
    {
      Block block = blockID;
      if ((uint) block <= 132U)
      {
        switch (block)
        {
          case Block.StainedGlassPane:
          case Block.LockedChest:
            break;
          case Block.CoverBlock:
            byte auxHighDataNoCache1 = this.GetAuxHighDataNoCache(p);
            if ((int) auxHighDataNoCache1 >= MapTM.CoverBlockTop.Length)
              return blockID;
            return MapTM.CoverBlockTop[(int) auxHighDataNoCache1];
          case Block.ArcadeMachine:
            byte auxHighDataNoCache2 = this.GetAuxHighDataNoCache(p);
            if (auxHighDataNoCache2 <= (byte) 2)
              return blockID + auxHighDataNoCache2;
            return blockID;
          default:
            goto label_12;
        }
      }
      else if ((uint) block <= 160U)
      {
        if (block != Block.LockedDoorTop && block != Block.Painting)
          goto label_12;
      }
      else if (block != Block.StainedGlass && block != Block.LockedDoorBottom)
        goto label_12;
      return blockID;
label_12:
      int index = (int) this.BlockData[(int) blockID].TextureID - 1;
      if (index < 0)
        return blockID;
      Block blockTexture = this.BlockTextures[index, (int) this.GetAuxHighDataNoCache(p)];
      if (blockTexture == Block.None)
        blockTexture = this.BlockTextures[index, 0];
      return blockTexture;
    }

    public Block GetBlockTextureIDForDrawing(Block blockID, int textureIndex)
    {
      Block block = blockID;
      if ((uint) block <= 132U)
      {
        switch (block)
        {
          case Block.StainedGlassPane:
          case Block.LockedChest:
            break;
          case Block.ArcadeMachine:
            if (textureIndex != 0 && textureIndex <= 2)
              return blockID + (byte) textureIndex;
            return Block.ArcadeMachine;
          default:
            goto label_9;
        }
      }
      else if ((uint) block <= 160U)
      {
        if (block != Block.LockedDoorTop && block != Block.Painting)
          goto label_9;
      }
      else if (block != Block.StainedGlass && block != Block.LockedDoorBottom)
        goto label_9;
      return blockID;
label_9:
      return this.GetBlockTextureID(blockID, textureIndex);
    }

    public Block GetBlockTextureID(Block blockID, int textureIndex)
    {
      int index = (int) this.BlockData[(int) blockID].TextureID - 1;
      if (index < 0 || index >= this.BlockTextures.GetLength(0))
        return blockID;
      Block blockTexture = this.BlockTextures[index, textureIndex];
      if (blockTexture == Block.None && blockID != Block.Painting)
        blockTexture = this.BlockTextures[index, 0];
      return blockTexture;
    }

    public int GetBlockTextureIndexFromExistingBlock(GlobalPoint3D p)
    {
      return this.GetBlockTextureIndexFromExistingBlock(p, (Item) this.GetBlockIDNoCache(p));
    }

    public int GetBlockTextureIndexFromExistingBlock(GlobalPoint3D p, Item blockID)
    {
      if (!this.UsesBlockTextureTable(blockID))
        return -1;
      return (int) this.GetAuxHighDataNoCache(p);
    }

    public bool HasFreeBlockTextureSlot(Block block)
    {
      int blockTextureIndex = this.GetBlockTextureIndex(block);
      if (blockTextureIndex < 0)
        return false;
      for (int index = 1; index < 16; ++index)
      {
        if (this.BlockTextures[blockTextureIndex, index] == Block.None)
          return true;
      }
      return false;
    }

    public bool HasBlockTexture(Block block, Item texture)
    {
      int blockTextureIndex = this.GetBlockTextureIndex(block);
      if (blockTextureIndex < 0)
        return false;
      Item obj = block == Block.Painting ? texture : this.Instance.ConvertItemIDToBlockIDForTextureIndex(texture);
      if (obj < Item.zLastBlockID)
      {
        Block block1 = (Block) obj;
        if (this.DoesBlockUsesKey(block))
          block1 = (Block) (texture - (ushort) 330);
        for (int index = 0; index < 16; ++index)
        {
          if (this.BlockTextures[blockTextureIndex, index] == block1)
            return true;
        }
      }
      return false;
    }

    public int GetOrAddBlockTextureIndex(Block blockID, Block blockTextureID)
    {
      int blockTextureIndex = this.GetBlockTextureIndex(blockID);
      int index1 = -1;
      for (int index2 = 0; index2 < 16; ++index2)
      {
        if (this.BlockTextures[blockTextureIndex, index2] == blockTextureID)
          return index2;
        if (this.BlockTextures[blockTextureIndex, index2] == Block.None && index1 == -1)
          index1 = index2;
      }
      if (index1 < 0)
        return -1;
      this.BlockTextures[blockTextureIndex, index1] = blockTextureID;
      return index1;
    }

    public void LoadBlockTextures(Block[,] data)
    {
      if (data != null)
      {
        int length = data.GetLength(0);
        for (int index1 = 0; index1 < length; ++index1)
        {
          for (int index2 = 0; index2 < data.GetLength(1); ++index2)
            this.BlockTextures[index1, index2] = data[index1, index2];
        }
        int blockTextureIndex = this.GetBlockTextureIndex(Block.zLastBlockID);
        if (length > 0 && length <= blockTextureIndex)
        {
          for (int index = 1; index < 13; ++index)
            this.BlockTextures[blockTextureIndex, index] = (Block) index;
        }
      }
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.TypeCounts.ArcadeMachine > (ushort) 0)
        {
          for (int arcadeMachine = (int) activeMod.TypeOffsets.ArcadeMachine; arcadeMachine < (int) activeMod.TypeOffsets.ArcadeMachine + (int) activeMod.TypeCounts.ArcadeMachine; ++arcadeMachine)
            this.BlockTextures[this.GetBlockTextureIndex(Block.ArcadeMachine), arcadeMachine] = (Block) arcadeMachine;
        }
      }
    }

    public static byte GetAuxFromID(byte id)
    {
      return (byte) ((((int) id & 120) << 1) + ((int) id & 7));
    }

    public static byte GetIDFromAux(byte aux)
    {
      return (byte) ((((int) aux & 240) >> 1) + ((int) aux & 7));
    }

    public byte GetIDFromAux(GlobalPoint3D p)
    {
      return MapTM.GetIDFromAux(this.GetAuxFullDataNoCache(p));
    }

    bool ITMMap.IsValidPoint([In] GlobalPoint3D obj0)
    {
      return this.IsValidPoint(obj0);
    }

    public enum BlockTextureChangeResult
    {
      None,
      ExistingTextureUsed,
      NewTextureSelected,
    }

    private class RLEStreamBytePending : RLEStreamByte
    {
      public byte GetDataNoCacheUnsafe(int mapIndex)
      {
        if (this.StreamSize == 2)
          return Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex + 1];
        return this.GetStreamDataNoLock(this.GetStreamIndex(mapIndex));
      }
    }

    private struct ReceiverReLinkInfo
    {
      public WifiReceiverBlock Receiver;
      public bool Freq1Adjusted;
      public bool Freq2Adjusted;
    }

    private struct TransmitterLinkInfo
    {
      public ushort OldFreq;
      public ushort NewFreq;
    }
  }
}
