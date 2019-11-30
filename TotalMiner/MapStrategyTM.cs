// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapStrategyTM
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
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class MapStrategyTM : MapStrategy
  {
    private Dictionary<long, List<DataBlock>> dataBlocksPendingLoad = new Dictionary<long, List<DataBlock>>();
    private Pool<List<MapChunk>> receiverSearchPool = new Pool<List<MapChunk>>(2, 1, true);
    private const int MaxRopeLength = 80;
    private float cropUpdateTimer;
    private int currentSentryTurret;
    private List<Zone> zones;
    private List<ushort> bookIDList;
    private List<Zone> tempZoneList;
    private List<Zone> largeZoneList;
    private Dictionary<long, List<Zone>> localZoneList;
    private List<SaveNPCState> unloadedNpcs;
    private Dictionary<ushort, BookData> bookData;
    private Dictionary<long, DataBlock> dataBlocks;
    private List<CropBlock> cropBlocks;
    private List<SignBlock> signBlocks;
    private List<MapStrategyTM.TimedBlock> timedBlocks;
    private List<StudioForge.TotalMiner.Blocks.MarkerBlock> markerBlocks;
    private List<HealthBlock> healthBlocks;
    private List<FurnaceBlock> activeFurnaces;
    private List<SentryTurretBlock> sentryTurrets;
    private List<ParticleEmitterBlock> particleEmitters;
    private List<ProximityDetectorBlock> proximityDetectors;
    private List<TeleportBlock> teleports;
    private List<FireBlock> burningFireBlocks;
    private List<MapStrategyTM.LiquidAddition> liquidAdditionsUpdate;
    private List<MapStrategyTM.LiquidAddition> liquidAdditionsUpdate2;
    private Dictionary<long, MapStrategyTM.LiquidRemoval> liquidRemovalUpdate;
    private Dictionary<long, MapStrategyTM.LiquidRemoval> liquidRemovalUpdate2;
    private Dictionary<long, MapChunk> tempChunkList;
    private Dictionary<long, byte> blocksReceivingPower;
    private Dictionary<long, byte> blocksDeliveringPower;
    private Dictionary<long, List<GamerID>> charactersOnPlate;
    private MapTM map;
    private GameInstance instance;
    private NetworkManager networkManager;
    private EnvManager envManager;
    private ushort lastTransmitterFrequency;

    public event EventHandler<EventArgs> MapDataChanged;

    private void RaiseMapDataChanged()
    {
      if (this.MapDataChanged == null)
        return;
      this.MapDataChanged((object) this, EventArgs.Empty);
    }

    public List<Zone> Zones
    {
      get
      {
        return this.zones;
      }
    }

    public List<SignBlock> SignBlocks
    {
      get
      {
        return this.signBlocks;
      }
    }

    public List<HealthBlock> HealthBlocks
    {
      get
      {
        return this.healthBlocks;
      }
    }

    public List<StudioForge.TotalMiner.Blocks.MarkerBlock> MarkerBlocks
    {
      get
      {
        return this.markerBlocks;
      }
    }

    public List<TeleportBlock> Teleports
    {
      get
      {
        return this.teleports;
      }
    }

    public List<FurnaceBlock> ActiveFurnaces
    {
      get
      {
        return this.activeFurnaces;
      }
    }

    public List<FireBlock> BurningFireBlocks
    {
      get
      {
        return this.burningFireBlocks;
      }
    }

    public List<ProximityDetectorBlock> ProximityDetectors
    {
      get
      {
        return this.proximityDetectors;
      }
    }

    public List<ParticleEmitterBlock> ParticleEmitterBlocks
    {
      get
      {
        return this.particleEmitters;
      }
    }

    public Dictionary<ushort, BookData> BookDataList
    {
      get
      {
        return this.bookData;
      }
    }

    public Dictionary<long, DataBlock> DataBlocks
    {
      get
      {
        return this.dataBlocks;
      }
    }

    public Dictionary<long, byte> BlocksReceivingPower
    {
      get
      {
        return this.blocksReceivingPower;
      }
    }

    public Dictionary<long, byte> BlocksDeliveringPower
    {
      get
      {
        return this.blocksDeliveringPower;
      }
    }

    public EnvManager EnvManager
    {
      get
      {
        return this.envManager;
      }
    }

    public ushort LastTransmitterFrequency
    {
      get
      {
        return this.lastTransmitterFrequency;
      }
    }

    public void ReplaceDataBlocks(Dictionary<long, DataBlock> dataBlocks)
    {
      this.dataBlocks = dataBlocks;
    }

    public MapStrategyTM(GameInstance instance)
    {
      this.instance = instance;
      this.networkManager = NetworkManager.Instance;
      this.timedBlocks = new List<MapStrategyTM.TimedBlock>(10);
      this.unloadedNpcs = new List<SaveNPCState>(10);
      this.zones = new List<Zone>(4);
      this.tempZoneList = new List<Zone>(4);
      this.largeZoneList = new List<Zone>(4);
      this.localZoneList = new Dictionary<long, List<Zone>>();
      this.bookData = new Dictionary<ushort, BookData>(4);
      this.bookIDList = new List<ushort>(10);
      this.dataBlocks = new Dictionary<long, DataBlock>(10);
      this.signBlocks = new List<SignBlock>(10);
      this.markerBlocks = new List<StudioForge.TotalMiner.Blocks.MarkerBlock>(4);
      this.activeFurnaces = new List<FurnaceBlock>(4);
      this.cropBlocks = new List<CropBlock>(10);
      this.healthBlocks = new List<HealthBlock>(10);
      this.sentryTurrets = new List<SentryTurretBlock>(10);
      this.particleEmitters = new List<ParticleEmitterBlock>(10);
      this.proximityDetectors = new List<ProximityDetectorBlock>(10);
      this.teleports = new List<TeleportBlock>(10);
      this.burningFireBlocks = new List<FireBlock>();
      this.tempChunkList = new Dictionary<long, MapChunk>(10);
      this.liquidAdditionsUpdate = new List<MapStrategyTM.LiquidAddition>();
      this.liquidAdditionsUpdate2 = new List<MapStrategyTM.LiquidAddition>();
      this.liquidRemovalUpdate = new Dictionary<long, MapStrategyTM.LiquidRemoval>();
      this.liquidRemovalUpdate2 = new Dictionary<long, MapStrategyTM.LiquidRemoval>();
      this.blocksDeliveringPower = new Dictionary<long, byte>();
      this.blocksReceivingPower = new Dictionary<long, byte>();
      this.charactersOnPlate = new Dictionary<long, List<GamerID>>(4);
    }

    public override void Initialize(Map map)
    {
      this.map = map as MapTM;
      this.envManager = new EnvManager(this.instance, this.map);
      this.envManager.LoadContent();
    }

    protected override void BeginCore()
    {
      this.map.IsCommitAllowed = true;
      this.instance.FlagStrategyIsSet();
    }

    public override void UnloadContentCore()
    {
      base.UnloadContentCore();
      if (this.timedBlocks != null)
        this.timedBlocks.Clear();
      if (this.unloadedNpcs != null)
        this.unloadedNpcs.Clear();
      if (this.zones != null)
        this.zones.Clear();
      if (this.largeZoneList != null)
        this.largeZoneList.Clear();
      if (this.localZoneList != null)
        this.localZoneList.Clear();
      if (this.tempZoneList != null)
        this.tempZoneList.Clear();
      if (this.bookData != null)
        this.bookData.Clear();
      if (this.bookIDList != null)
        this.bookIDList.Clear();
      if (this.dataBlocks != null)
        this.dataBlocks.Clear();
      if (this.blocksReceivingPower != null)
        this.blocksReceivingPower.Clear();
      if (this.blocksDeliveringPower != null)
        this.blocksDeliveringPower.Clear();
      if (this.signBlocks != null)
        this.signBlocks.Clear();
      if (this.healthBlocks != null)
        this.healthBlocks.Clear();
      if (this.markerBlocks != null)
        this.markerBlocks.Clear();
      if (this.activeFurnaces != null)
        this.activeFurnaces.Clear();
      if (this.cropBlocks != null)
        this.cropBlocks.Clear();
      if (this.sentryTurrets != null)
        this.sentryTurrets.Clear();
      if (this.particleEmitters != null)
        this.particleEmitters.Clear();
      if (this.proximityDetectors != null)
        this.proximityDetectors.Clear();
      if (this.teleports != null)
        this.teleports.Clear();
      if (this.burningFireBlocks != null)
        this.burningFireBlocks.Clear();
      if (this.liquidAdditionsUpdate != null)
        this.liquidAdditionsUpdate.Clear();
      if (this.liquidAdditionsUpdate2 != null)
        this.liquidAdditionsUpdate2.Clear();
      if (this.envManager == null)
        return;
      this.envManager.UnloadContent();
    }

    public void ApplyLoadData(SaveDataResult data)
    {
      this.dataBlocks = data.SerializedData.DataBlocks;
      this.blocksDeliveringPower = data.SerializedData.BlocksDeliveringPower;
      if (data.SaveData.Map.IsHost)
        this.RebuildBlocksReceivingPower();
      else
        this.blocksReceivingPower = data.SerializedData.BlocksReceivingPower;
      this.map.SignTextCache = data.SerializedData.SignTextCache;
      this.lastTransmitterFrequency = (ushort) data.SaveData.GameState.LastTransmitterFrequency;
      this.ApplyLoadDataOld(data.SaveData);
      this.ProcessLoadData(data);
    }

    private void ProcessLoadData(SaveDataResult data)
    {
      if (data.SaveData.Header.ExeVersion <= 20032)
      {
        Dictionary<long, DataBlock> dictionary = new Dictionary<long, DataBlock>(this.dataBlocks.Count);
        foreach (KeyValuePair<long, DataBlock> dataBlock in this.dataBlocks)
        {
          long globalHashCode = this.map.GetGlobalHashCode(dataBlock.Value.Point);
          dictionary.Add(globalHashCode, dataBlock.Value);
        }
        data.SerializedData.DataBlocks = this.dataBlocks = dictionary;
      }
      List<long> longList = new List<long>();
      bool isHost = this.map.IsHost;
      foreach (KeyValuePair<long, DataBlock> dataBlock in this.dataBlocks)
      {
        DataBlockType classType = dataBlock.Value.ClassType;
        if (classType == DataBlockType.None)
          longList.Add(dataBlock.Key);
        else if (isHost && classType == DataBlockType.Sign)
        {
          SignBlock signBlock = dataBlock.Value as SignBlock;
          if (signBlock != null)
            this.signBlocks.Add(signBlock);
        }
        else if (classType != DataBlockType.Teleport)
        {
          long chunkGlobalHashCode = this.map.GetChunkGlobalHashCode(this.map.GetChunk(dataBlock.Value.Point));
          List<DataBlock> dataBlockList;
          if (this.dataBlocksPendingLoad.TryGetValue(chunkGlobalHashCode, out dataBlockList))
          {
            dataBlockList.Add(dataBlock.Value);
          }
          else
          {
            dataBlockList = new List<DataBlock>();
            dataBlockList.Add(dataBlock.Value);
            this.dataBlocksPendingLoad.Add(chunkGlobalHashCode, dataBlockList);
          }
        }
        else if (classType == DataBlockType.Blueprint)
        {
          Blueprint blueprint = Blueprints.GetBlueprint((int) (dataBlock.Value as BlueprintBlock).ID);
          if (blueprint != null)
            this.instance.BlueprintsToPlace.Remove(blueprint);
        }
        else if (classType == DataBlockType.WisdomScroll)
        {
          WisdomItem wisdom = Wisdom.GetWisdom((int) (dataBlock.Value as WisdomScrollBlock).Index);
          if (wisdom != null)
            this.instance.WisdomsToPlace.Remove(wisdom);
        }
      }
      foreach (long key in longList)
        this.dataBlocks.Remove(key);
    }

    public void ApplyDataBlocksOnChunkDecorated(MapChunk chunk)
    {
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (this.dataBlocksPendingLoad)
      {
        List<DataBlock> dataBlockList;
        if (!this.dataBlocksPendingLoad.TryGetValue(globalHashCode, out dataBlockList))
          return;
        foreach (DataBlock block in dataBlockList)
          this.ApplyDataBlock(chunk, block);
        this.dataBlocksPendingLoad.Remove(globalHashCode);
      }
    }

    public void ReplacePendingDataBlock(long hash, DataBlock newBlock)
    {
      List<DataBlock> dataBlockList;
      if (!this.dataBlocksPendingLoad.TryGetValue(hash, out dataBlockList))
        return;
      for (int index = 0; index < dataBlockList.Count; ++index)
      {
        if (dataBlockList[index].Point == newBlock.Point)
        {
          dataBlockList[index] = newBlock;
          break;
        }
      }
    }

    private void ApplyDataBlock(MapChunk chunk, DataBlock block)
    {
      DataBlockType classType = block.ClassType;
      Block blockId = (Block) chunk.GetBlockID(chunk.GetMapIndex(block.Point));
      if (classType != this.map.BlockData[(int) blockId].ClassType)
      {
        this.RemoveDataBlock(block.Point, GamerType.Generation);
      }
      else
      {
        switch (classType)
        {
          case DataBlockType.ParticleEmitter:
            ParticleEmitterBlock particleEmitterBlock = block as ParticleEmitterBlock;
            if (particleEmitterBlock == null)
              break;
            this.particleEmitters.Add(particleEmitterBlock);
            break;
          case DataBlockType.Furnace:
            FurnaceBlock furnaceBlock = block as FurnaceBlock;
            if (furnaceBlock == null)
              break;
            furnaceBlock.Map = (Map) this.map;
            furnaceBlock.FurnaceBurnStarted += new EventHandler(this.OnFurnaceBurnStarted);
            furnaceBlock.FurnaceBurnEnded += new EventHandler(this.OnFurnaceBurnEnded);
            furnaceBlock.GetProduct();
            if (furnaceBlock.HasFuel)
              this.activeFurnaces.Add(furnaceBlock);
            if (!this.map.IsHost)
              break;
            furnaceBlock.Gamertag = (string) null;
            break;
          case DataBlockType.AmbientSound:
            AmbientSoundBlock block1 = block as AmbientSoundBlock;
            if (block1 == null)
              break;
            this.instance.AmbientSoundManager.SetBlock(block1);
            break;
          case DataBlockType.SentryTurret:
            SentryTurretBlock sentryTurretBlock = block as SentryTurretBlock;
            if (sentryTurretBlock == null)
              break;
            this.sentryTurrets.Add(sentryTurretBlock);
            break;
          case DataBlockType.ProximityDetector:
            ProximityDetectorBlock proximityDetectorBlock = block as ProximityDetectorBlock;
            if (proximityDetectorBlock == null)
              break;
            lock (this.proximityDetectors)
            {
              this.proximityDetectors.Add(proximityDetectorBlock);
              break;
            }
          case DataBlockType.Fire:
            FireBlock fireBlock = block as FireBlock;
            if (fireBlock == null)
              break;
            this.burningFireBlocks.Add(fireBlock);
            break;
          case DataBlockType.NPCSpawn:
            NpcSpawnBlock block2 = block as NpcSpawnBlock;
            if (block2 == null)
              break;
            this.instance.NpcManager.NpcSpawnAdded(block2);
            break;
          case DataBlockType.Sign:
            if (this.map.IsHost)
              break;
            SignBlock signBlock = block as SignBlock;
            if (signBlock == null)
              break;
            this.signBlocks.Add(signBlock);
            this.map.SignTextCacheChanged = true;
            break;
          case DataBlockType.Crop:
            CropBlock cropBlock = block as CropBlock;
            if (cropBlock == null)
              break;
            this.cropBlocks.Add(cropBlock);
            break;
        }
      }
    }

    private void ApplyLoadDataOld(SaveData data)
    {
      this.LoadChestData(data);
      this.LoadBookcaseData(data);
      this.LoadLockedDoorData(data);
      this.LoadSentryTurretData(data);
      this.LoadMineBlockData(data);
      this.LoadSignData(data);
      this.LoadNPCData(data);
      this.LoadZoneData(data);
      this.LoadBookData(data);
      this.LoadShopData(data);
      this.LoadTeleportData(data);
      this.LoadAmbientSoundData(data);
    }

    private void LoadLockedDoorData(SaveData data)
    {
      foreach (SavePlayerBlockState lockedDoor in data.GameState.LockedDoors)
      {
        DoorBlock doorBlock = new DoorBlock(lockedDoor.Point);
        doorBlock.LoadFromSaveData(lockedDoor);
        this.AddDataBlock((DataBlock) doorBlock, UpdateBlockMethod.Strategy, false);
      }
    }

    private void LoadChestData(SaveData data)
    {
      foreach (SaveChestState chest in data.GameState.Chests)
      {
        if (chest.Items.Count > 0)
        {
          ChestBlock chestBlock = new ChestBlock(chest.Point, (int) chest.PackSize);
          chestBlock.LoadFromSaveData(chest);
          this.AddDataBlock((DataBlock) chestBlock, UpdateBlockMethod.Strategy, false);
        }
      }
    }

    private void LoadBookcaseData(SaveData data)
    {
      foreach (SaveChestState bookcase in data.GameState.Bookcases)
      {
        if (bookcase.Items.Count > 0)
        {
          BookcaseBlock bookcaseBlock = new BookcaseBlock(bookcase.Point);
          bookcaseBlock.LoadFromSaveData(bookcase);
          this.AddDataBlock((DataBlock) bookcaseBlock, UpdateBlockMethod.Strategy, false);
        }
      }
    }

    private void LoadSentryTurretData(SaveData data)
    {
      foreach (SaveSentryTurretState sentryTurret in data.GameState.SentryTurrets)
      {
        SentryTurretBlock sentryTurretBlock = new SentryTurretBlock(sentryTurret.Point);
        sentryTurretBlock.LoadFromSaveData(sentryTurret);
        this.AddDataBlock((DataBlock) sentryTurretBlock, UpdateBlockMethod.Strategy, false);
      }
    }

    private void LoadMineBlockData(SaveData data)
    {
      foreach (SaveMineBlockState mineBlock in data.GameState.MineBlocks)
      {
        ProximityDetectorBlock proximityDetectorBlock = new ProximityDetectorBlock(mineBlock.Point);
        proximityDetectorBlock.LoadFromSaveData(mineBlock);
        this.AddDataBlock((DataBlock) proximityDetectorBlock, UpdateBlockMethod.Strategy, false);
      }
    }

    private void LoadSignData(SaveData data)
    {
      SaveSignsState signs = data.GameState.Signs;
      if (signs.SignCount <= 0)
        return;
      for (int index = 0; index < signs.Signs.Count; ++index)
      {
        SaveSignState sign = signs.Signs[index];
        if (sign.Text1 >= (short) 0 || sign.Text2 >= (short) 0 || (sign.Text3 >= (short) 0 || sign.Text4 >= (short) 0))
          this.AddDataBlock((DataBlock) SignBlock.LoadFromSaveData(sign), UpdateBlockMethod.Strategy, false);
      }
      this.map.SignTextCache = signs.SignText;
    }

    private void LoadNPCData(SaveData data)
    {
      foreach (SaveNPCState npC in data.GameState.NPCs)
      {
        NpcSpawnBlock npcSpawnBlock = new NpcSpawnBlock(npC.Point);
        npcSpawnBlock.LoadFromSaveData(npC);
        this.AddDataBlock((DataBlock) npcSpawnBlock, UpdateBlockMethod.Strategy, false);
      }
    }

    private void LoadZoneData(SaveData data)
    {
      foreach (SaveZoneState zone1 in data.GameState.Zones)
      {
        Zone zone2 = new Zone(zone1.Name, zone1.Type, zone1.Min, zone1.Max);
        if (zone1.BuilderType == ZoneBuilderType.None || zone1.Builder != null && (zone1.Builder == "" || zone1.Builder.Length == 0))
          zone1.Builder = (string) null;
        zone2.Builder = zone1.Builder;
        zone2.BuilderType = zone1.BuilderType;
        zone2.OnEntryScriptName = zone1.OnEntryScript;
        zone2.OnExitScriptName = zone1.OnExitScript;
        zone2.CombatLevelDifference = zone1.CombatLevelDifference;
        zone2.SpeedMultiplier = zone1.SpeedMultiplier;
        zone2.GravityMultiplier = zone1.GravityMultiplier;
        this.AddZone(zone2);
      }
    }

    private void LoadBookData(SaveData data)
    {
      foreach (SaveBookState book1 in data.GameState.Books)
      {
        BookData book2 = new BookData();
        book2.LoadFromSaveData(book1);
        this.AddBookData(book2);
      }
    }

    private void LoadShopData(SaveData data)
    {
      if (data.GameState.ShopBlocks == null)
        return;
      foreach (SaveShopBlockState shopBlock1 in data.GameState.ShopBlocks)
      {
        ShopBlock shopBlock2 = new ShopBlock(shopBlock1.Point, (Inventory) null);
        shopBlock2.LoadFromSaveData(shopBlock1);
        this.AddDataBlock((DataBlock) shopBlock2, UpdateBlockMethod.Strategy, false);
      }
    }

    private void LoadTeleportData(SaveData data)
    {
      foreach (SaveTeleportState teleport in data.GameState.Teleports)
      {
        TeleportBlock teleportBlock = new TeleportBlock();
        teleportBlock.Point = teleport.Point;
        teleportBlock.Channel = teleport.Channel;
        this.AddDataBlock((DataBlock) teleportBlock, UpdateBlockMethod.Strategy, true);
      }
    }

    private void LoadAmbientSoundData(SaveData data)
    {
      foreach (SaveAmbientSoundState ambientSoundBlock1 in data.GameState.AmbientSoundBlocks)
      {
        AmbientSoundBlock ambientSoundBlock2 = new AmbientSoundBlock();
        ambientSoundBlock2.LoadFromSaveData(ambientSoundBlock1);
        this.AddDataBlock((DataBlock) ambientSoundBlock2, UpdateBlockMethod.Strategy, false);
      }
    }

    public override void Update(UpdateState state)
    {
      try
      {
        if (this.map.IsHost)
        {
          this.UpdateSentryTurretBlocks();
          this.UpdateCrops();
        }
        this.UpdateEnvironment();
        this.UpdateActiveFurnaceBlocks();
        this.UpdateTimedBlocks();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(108, ex);
      }
    }

    public override void BlockChanged(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (newBlockData.Chunk == null)
        return;
      Block blockId1 = (Block) oldBlockData.BlockID;
      Block blockId2 = (Block) newBlockData.BlockID;
      Block block1 = blockId1;
      if ((uint) block1 <= 72U)
      {
        if ((uint) block1 <= 13U)
        {
          switch (block1)
          {
            case Block.Wood:
              this.TreeBlockCleared(p, method, playerID, ((int) oldBlockData.AuxData & 8) == 0);
              goto label_37;
            case Block.Water:
            case Block.Lava:
              if ((int) newBlockData.BlockID != (int) oldBlockData.BlockID)
              {
                this.RemoveLiquidFlow(p, oldBlockData.BlockID, (byte) ((uint) oldBlockData.AuxData & 7U), method);
                goto label_37;
              }
              else
                goto label_37;
            default:
              goto label_37;
          }
        }
        else
        {
          switch (block1)
          {
            case Block.Obsidian:
              this.RemoveTeleport(p);
              goto label_37;
            case Block.Bookcase:
            case Block.Chest:
              break;
            case Block.Torch:
            case Block.Sapling:
              this.RemoveTimedBlock(p, blockId1);
              goto label_37;
            case Block.Furnace:
              if (blockId2 != Block.LitFurnace)
              {
                this.RemoveFurnaceBlock(p, method, playerID);
                goto label_37;
              }
              else
                goto label_37;
            case Block.WoodDoorTop:
            case Block.SteelDoorTop:
              goto label_19;
            case Block.TNT:
            case Block.C4:
              if (method == UpdateBlockMethod.Blast)
              {
                this.instance.CreateBlast(p, blockId1, playerID);
                goto label_37;
              }
              else
                goto label_37;
            case Block.Wisdom:
            case Block.Blueprint:
              goto label_24;
            case Block.ItemShop:
            case Block.BlockShop:
              if (((int) oldBlockData.AuxData & 7) == 1)
              {
                this.RemoveChestBlock(p, method, playerID);
                goto label_37;
              }
              else
                goto label_37;
            case Block.Rope:
              this.RopeCleared(p);
              goto label_37;
            default:
              goto label_37;
          }
        }
      }
      else if ((uint) block1 <= 121U)
      {
        switch (block1)
        {
          case Block.WoodDoorBottom:
          case Block.SteelDoorBottom:
            goto label_19;
          case Block.Book:
            goto label_24;
          default:
            goto label_37;
        }
      }
      else
      {
        switch (block1)
        {
          case Block.ArcadeMachine:
            this.RemoveArcadeMachineBlock(p);
            goto label_37;
          case Block.LockedChest:
          case Block.Crate:
          case Block.Safe:
            break;
          case Block.LitFurnace:
            if (blockId2 != Block.Furnace)
            {
              this.RemoveFurnaceBlock(p, method, playerID);
              goto label_37;
            }
            else
              goto label_37;
          case Block.BedHead:
          case Block.BedFoot:
            this.RemoveBedBlock(p, oldBlockData, method, playerID);
            goto label_37;
          case Block.LockedDoorTop:
          case Block.LockedDoorBottom:
            goto label_19;
          case Block.SentryTurret:
            this.RemoveSentryTurretBlock(p, method, playerID);
            goto label_37;
          case Block.WifiTransmitter:
            long globalHashCode = this.map.GetGlobalHashCode(p);
            lock (this.blocksReceivingPower)
            {
              if (this.blocksReceivingPower.ContainsKey(globalHashCode))
              {
                this.EmitPowerSignal(p, false, method, playerID);
                goto label_37;
              }
              else
                goto label_37;
            }
          case Block.WifiReceiver:
            WifiReceiverBlock dataBlock = this.GetDataBlock(p) as WifiReceiverBlock;
            if (dataBlock != null && dataBlock.Transmitters != null && dataBlock.Transmitters.Count > 0)
            {
              dataBlock.Transmitters = (List<long>) null;
              this.UpdateReceiver(dataBlock, false, method, playerID);
              goto label_37;
            }
            else
              goto label_37;
          case Block.PressurePlate:
            this.RemovePlateBlock(p);
            goto label_37;
          default:
            goto label_37;
        }
      }
      this.RemoveChestBlock(p, method, playerID);
      goto label_37;
label_19:
      this.RemoveDoorBlock(p, blockId1, method, playerID);
      goto label_37;
label_24:
      this.ImportantIconCleared(p, oldBlockData, method, playerID, transmit);
label_37:
      bool flag = this.map.BlockData[(int) blockId1].ClassType != DataBlockType.None;
      if (flag)
      {
        if (blockId1 == Block.Furnace && blockId2 == Block.LitFurnace)
          flag = false;
        else if (blockId1 == Block.LitFurnace && blockId2 == Block.Furnace)
          flag = false;
        if (flag)
          this.RemoveDataBlock(p, Globals2.GetGamerType(playerID));
      }
      Block block2 = blockId2;
      if ((uint) block2 <= 72U)
      {
        if ((uint) block2 <= 31U)
        {
          switch (block2)
          {
            case Block.None:
              this.HandleBlockCleared(p, oldBlockData, blockId2, method, playerID);
              break;
            case Block.Water:
            case Block.Lava:
                if (newBlockData.AuxData == (byte) 0)
                {
                switch (method)
                    {
                        case UpdateBlockMethod.Player:
                        case UpdateBlockMethod.PlayerRelated:
                        case UpdateBlockMethod.Strategy:
                        this.AddLiquidFlow(p, newBlockData.BlockID, (byte) 0, method);
                        break;
                    }
                }
                break;
            case Block.Obsidian:
              if (method != UpdateBlockMethod.Generation)
              {
                this.AddTeleport(p, (byte) ((uint) newBlockData.AuxData >> 4), method, transmit);
                break;
              }
              break;
          }
        }
        else if ((uint) block2 <= 58U)
        {
          if ((block2 == Block.Torch || block2 == Block.Sapling) && method != UpdateBlockMethod.Generation)
            this.TimedBlockAdded(p, blockId2, playerID);
        }
        else
        {
          switch (block2)
          {
            case Block.Teflon:
              this.AddSliderBlock(p, playerID);
              break;
            case Block.Rope:
              if (method != UpdateBlockMethod.Generation)
              {
                this.AddRope(p);
                break;
              }
              break;
          }
        }
      }
      else
      {
        if ((uint) block2 <= 132U)
        {
          if ((uint) block2 <= 118U)
          {
            switch (block2)
            {
              case Block.SpiderEgg:
                this.AddNpcSpawnBlock(p, blockId2, method);
                goto label_80;
              case Block.Fire:
                if (((int) newBlockData.AuxData & 7) == 1)
                {
                  this.AddFireBlock(p, method);
                  goto label_80;
                }
                else
                  goto label_80;
              default:
                goto label_80;
            }
          }
          else
          {
            switch (block2)
            {
              case Block.Crop:
                this.AddCropBlock(p, method);
                goto label_80;
              case Block.LockedChest:
                break;
              default:
                goto label_80;
            }
          }
        }
        else if ((uint) block2 <= 164U)
        {
          switch (block2)
          {
            case Block.SentryTurret:
            case Block.ProximityDetector:
              this.AddDataBlock(p, blockId2, method, playerID);
              goto label_80;
            case Block.WifiTransmitter:
              this.AddWifiTransmitterBlock(p, method, playerID, transmit);
              goto label_80;
            case Block.WifiReceiver:
              this.AddWifiReceiverBlock(p, method, playerID);
              goto label_80;
            default:
              goto label_80;
          }
        }
        else
        {
          switch (block2)
          {
            case Block.LockedDoorBottom:
              break;
            case Block.Marker:
              this.AddMarkerBlock(p, method, playerID, false);
              goto label_80;
            case Block.ExcludeMarker:
              this.AddMarkerBlock(p, method, playerID, true);
              goto label_80;
            default:
              goto label_80;
          }
        }
        if (playerID.IsGamer)
          this.AddDataBlock(p, blockId2, method, playerID);
      }
label_80:
      if ((method == UpdateBlockMethod.Player || method == UpdateBlockMethod.PlayerRelated || (method == UpdateBlockMethod.Strategy || method == UpdateBlockMethod.Blast)) && newBlockData.Chunk.LastBlockEditedIndex == -1)
        newBlockData.Chunk.LastBlockEditedIndex = newBlockData.Chunk.GetMapIndex(p);
      this.map.AddChunkToCommitList(newBlockData.Chunk, method);
      if (transmit)
        this.networkManager.SendBlockChange(p, oldBlockData, newBlockData, method, playerID, false);
      if (method != UpdateBlockMethod.Generation)
      {
        if ((int) oldBlockData.BlockID != (int) newBlockData.BlockID)
        {
          this.UpdateLight(p, oldBlockData, newBlockData, method);
          this.SetChunkUpdateFlags(p, newBlockData, method);
          this.RaiseMapDataChanged();
        }
        else if (this.DoesAuxChangeAppearance(newBlockData.BlockID, Math.Max(oldBlockData.AuxData, newBlockData.AuxData)))
          newBlockData.Chunk.SetChunkFlag(ChunkFlags.MeshDirty);
      }
      if (!this.map.BlockData[(int) blockId1].IsPowerEmitter)
        return;
      this.instance.DeliverPower(p, blockId1, (BlockFace) ((uint) oldBlockData.AuxData & 7U), false, method, playerID, transmit, false);
    }

    private void UpdateLight(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method)
    {
      switch (method)
      {
        case UpdateBlockMethod.CreativeHelper:
        case UpdateBlockMethod.CreativeHelperPriority:
        case UpdateBlockMethod.Copy:
        case UpdateBlockMethod.Paste:
          newBlockData.Chunk.SetChunkFlag(ChunkFlags.LightDirty);
          break;
        default:
          this.UpdateLightCore(p, oldBlockData, newBlockData, method);
          break;
      }
    }

    private void UpdateLightCore(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method)
    {
      int next = MapLightingByPointThreadedWrapper.Pool.GetNext();
      MapLightingByPointThreadedWrapper pointThreadedWrapper = MapLightingByPointThreadedWrapper.Pool.List[next];
      pointThreadedWrapper.Initialize((Map) this.map, next, p, oldBlockData, newBlockData);
      if (this.IsMethodUsuallyThreaded(method))
        pointThreadedWrapper.Update();
      else
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) pointThreadedWrapper, false, PriorityLevel.Priority);
    }

    private void SetChunkUpdateFlags(
      GlobalPoint3D p,
      MapBlock newBlockData,
      UpdateBlockMethod method)
    {
      MapChunk chunk = newBlockData.Chunk;
      if (newBlockData.BlockID != (byte) 0)
        return;
      Point3D chunkSize = this.map.ChunkSize;
      int num1 = p.X % chunkSize.X;
      int num2 = p.Y % chunkSize.Y;
      int num3 = p.Z % chunkSize.Z;
      if (num1 == 0)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.LeftChunkBorder);
        chunk.LeftNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.RightSegmentBorder);
      }
      if (num2 == 0)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.DownChunkBorder);
        chunk.DownNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.UpSegmentBorder);
      }
      if (num3 == 0)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.ForwardChunkBorder);
        chunk.ForwardNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.BackSegmentBorder);
      }
      if (num1 == chunkSize.X - 1)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.RightChunkBorder);
        chunk.RightNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.LeftSegmentBorder);
      }
      if (num2 == chunkSize.Y - 1)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.UpChunkBorder);
        chunk.UpNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.DownSegmentBorder);
      }
      if (num3 == chunkSize.Z - 1)
      {
        chunk.SetUpdateFlag(ChunkUpdateFlags.BackChunkBorder);
        chunk.BackwardNeighbour()?.SetUpdateFlag(ChunkUpdateFlags.ForwardSegmentBorder);
      }
      chunkSize.X /= 2;
      chunkSize.Y /= 2;
      chunkSize.Z /= 2;
      if (num1 == chunkSize.X)
        newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.LeftSegmentBorder);
      if (num2 == chunkSize.Y)
        newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.DownSegmentBorder);
      if (num3 == chunkSize.Z)
        newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.ForwardSegmentBorder);
      if (num1 == chunkSize.X - 1)
        newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.RightSegmentBorder);
      if (num2 == chunkSize.Y - 1)
        newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.BackSegmentBorder);
      if (num3 != chunkSize.Z - 1)
        return;
      newBlockData.Chunk.SetUpdateFlag(ChunkUpdateFlags.UpSegmentBorder);
    }

    private bool IsMethodUsuallyThreaded(UpdateBlockMethod method)
    {
      if (method != UpdateBlockMethod.Generation && method != UpdateBlockMethod.CreativeHelper && (method != UpdateBlockMethod.CreativeHelperPriority && method != UpdateBlockMethod.Copy) && (method != UpdateBlockMethod.Paste && method != UpdateBlockMethod.Blast))
        return method == UpdateBlockMethod.Flood;
      return true;
    }

    private void HandleBlockCleared(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      Block newBlockID,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      switch (method)
      {
        case UpdateBlockMethod.Player:
        case UpdateBlockMethod.PlayerRelated:
          this.HandleBlockClearedByPlayer(p, oldBlockData, method, playerID);
          break;
        case UpdateBlockMethod.Strategy:
          this.HandleBlockClearedByStrategy(p, oldBlockData, method);
          break;
        case UpdateBlockMethod.Blast:
          this.HandleBlockClearedByBlast(p, oldBlockData, method);
          break;
        case UpdateBlockMethod.DropTimeShort:
          this.HandleBlockClearedByFire(p, oldBlockData, method);
          break;
      }
      this.instance.ParticleManager.BlockCleared(p);
      ++p.Y;
      if (p.Y < this.map.MapBound.Max.Y)
      {
        switch ((Block) this.map.GetBlockID(p))
        {
          case Block.Teflon:
            ++p.Y;
            Block block = p.Y == this.map.MapBound.Max.Y ? Block.None : (Block) this.map.GetBlockID(p);
            --p.Y;
            if (block != Block.Water && block != Block.Lava)
            {
              this.AddSliderBlock(p, playerID);
              break;
            }
            break;
          case Block.Rope:
            this.AddRope(p);
            break;
        }
      }
      p.Y -= 2;
      if (p.Y <= this.map.MapBound.Min.Y)
        return;
      ++p.Y;
      this.RopeCleared(p);
    }

    private void HandleBlockClearedByPlayer(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      Player player = this.instance.GetPlayer(playerID);
      if (player == null)
        return;
      BlockClearedEventArgs e = new BlockClearedEventArgs()
      {
        Point = p,
        BlockData = oldBlockData,
        Method = method,
        PlayerID = playerID,
        Result = ClearBlockResult.Success
      };
      player.OnSuccessfulBlockClear(ref e);
    }

    private void HandleBlockClearedByStrategy(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      UpdateBlockMethod method)
    {
      if (!this.map.IsHost || !this.ShouldAddPickupForStrategyClear((Block) oldBlockData.BlockID))
        return;
      BlockClearedEventArgs e = new BlockClearedEventArgs()
      {
        Point = p,
        BlockData = oldBlockData,
        Method = method,
        PlayerID = GamerID.Sys1,
        Result = ClearBlockResult.Success
      };
      this.instance.AddPickup(ref e);
    }

    private bool ShouldAddPickupForStrategyClear(Block blockID)
    {
      if (this.instance.ShouldAddPickUp((Player) null, blockID) && ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves))
        return this.instance.Random.Next(20) == 0;
      return false;
    }

    private void HandleBlockClearedByBlast(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      UpdateBlockMethod method)
    {
      if (this.instance.Random.Next(8) == 0)
        this.instance.AddMiningParticles(this.map.GetBlockCenter(p), (Block) oldBlockData.BlockID, 4, 0.1f, 0.3f, 300f, 12f);
      if (!this.IsBlastPickup((Block) oldBlockData.BlockID) || !this.map.IsHost)
        return;
      BlockClearedEventArgs e = new BlockClearedEventArgs()
      {
        Point = p,
        BlockData = oldBlockData,
        Method = method,
        PlayerID = GamerID.Sys1,
        Result = ClearBlockResult.Success
      };
      this.instance.AddPickup(ref e);
    }

    private bool IsBlastPickup(Block blockID)
    {
      return blockID > Block.Bedrock && blockID < Block.Cobblestone || blockID >= Block.ItemShop && blockID < Block.WovenLeaves;
    }

    private void HandleBlockClearedByFire(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      UpdateBlockMethod method)
    {
    }

    private void ImportantIconCleared(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (method == UpdateBlockMethod.Generation || !this.map.IsHost)
        return;
      BlockClearedEventArgs e = new BlockClearedEventArgs()
      {
        Point = p,
        BlockData = oldBlockData,
        Method = method,
        PlayerID = playerID,
        Result = ClearBlockResult.Success,
        IgnoreFiniteModePickupRestriction = oldBlockData.BlockID != (byte) 121
      };
      this.instance.AddPickup(ref e);
    }

    public override void AuxChanged(
      GlobalPoint3D p,
      byte oldAuxData,
      byte newAuxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(p);
      Block blockId = (Block) blockIdAndAux.BlockID;
      if ((int) oldAuxData == (int) newAuxData && !this.map.UsesBlockTextureTable(blockId))
        return;
      blockIdAndAux.AuxData = oldAuxData;
      byte num1 = (byte) ((uint) oldAuxData & 7U);
      byte num2 = (byte) ((uint) newAuxData & 7U);
      switch (blockId)
      {
        case Block.Obsidian:
          this.AddTeleportAndChannel(p, (byte) ((uint) newAuxData >> 4), method);
          break;
        case Block.ItemShop:
        case Block.BlockShop:
          if (num1 == (byte) 0 && num2 == (byte) 1)
          {
            Player player = this.instance.GetPlayer(playerID);
            this.AddEconomizedShopBlock(p, player, method);
            break;
          }
          break;
      }
      if (method == UpdateBlockMethod.Player || method == UpdateBlockMethod.PlayerRelated || (method == UpdateBlockMethod.Strategy || method == UpdateBlockMethod.Blast))
      {
        if (blockIdAndAux.Chunk.LastBlockEditedIndex == -1)
          blockIdAndAux.Chunk.LastBlockEditedIndex = blockIdAndAux.Chunk.GetMapIndex(p);
        if (this.DoesAuxChangeAppearance(blockIdAndAux.BlockID, Math.Max(oldAuxData, newAuxData)) && blockIdAndAux.Chunk != null)
        {
          this.map.AddChunkToCommitList(blockIdAndAux.Chunk, method);
          blockIdAndAux.Chunk.SetChunkFlag(ChunkFlags.MeshDirty);
          if (blockIdAndAux.BlockID == (byte) 166)
          {
            MapBlock newBlockData = blockIdAndAux;
            newBlockData.AuxData = newAuxData;
            this.UpdateLightCore(p, blockIdAndAux, newBlockData, method);
          }
        }
      }
      if (method != UpdateBlockMethod.Generation)
        this.RaiseMapDataChanged();
      if (!transmit)
        return;
      this.networkManager.SendBlockAuxChange(p, oldAuxData, newAuxData, method, playerID);
    }

    private bool DoesAuxChangeAppearance(byte blockID, byte aux)
    {
      if (aux > (byte) 15 || this.map.BlockData[(int) blockID].IsRotated)
        return true;
      switch ((Block) blockID)
      {
        case Block.ItemShop:
        case Block.BlockShop:
        case Block.LockedChest:
          return false;
        default:
          return true;
      }
    }

    public override ClearBlockResult GetClearBlockResult(
      GlobalPoint3D p,
      byte blockID,
      UpdateBlockMethod method,
      GamerID playerID,
      bool isRelatedClear)
    {
      ClearBlockResult clearBlockResult = ClearBlockResult.Success;
      if (method != UpdateBlockMethod.Generation)
      {
        Block block = (Block) blockID;
        if ((uint) block <= 117U)
        {
          if ((uint) block <= 50U)
          {
            switch (block)
            {
              case Block.Bookcase:
                goto label_36;
              case Block.Furnace:
                goto label_21;
              case Block.Chest:
                break;
              default:
                goto label_38;
            }
          }
          else
          {
            switch (block)
            {
              case Block.ItemShop:
              case Block.BlockShop:
                ShopBlock dataBlock1 = this.GetDataBlock(p) as ShopBlock;
                if (dataBlock1 != null && this.map.GetAuxData(p) == (byte) 1)
                {
                  if (this.instance.IsBlockOpen(p).IsGamer)
                  {
                    clearBlockResult = ClearBlockResult.AnotherPlayerHasThisBlockOpen;
                    goto label_38;
                  }
                  else if (!this.HasPermission(playerID, Permissions.Admin) && playerID != (short) -1)
                  {
                    Player player = this.instance.GetPlayer(playerID);
                    if (!dataBlock1.IsOwner(player))
                    {
                      clearBlockResult = ClearBlockResult.PermissionDenied;
                      goto label_38;
                    }
                    else
                      goto label_38;
                  }
                  else
                    goto label_38;
                }
                else
                  goto label_38;
              case Block.AmbientSoundBlock:
                goto label_32;
              case Block.Sign:
                goto label_36;
              default:
                goto label_38;
            }
          }
        }
        else
        {
          if ((uint) block <= 143U)
          {
            switch (block)
            {
              case Block.Book:
                goto label_36;
              case Block.InvisibleBarrier:
              case Block.NPCSpawn:
                goto label_32;
              case Block.LockedChest:
              case Block.Crate:
                goto label_8;
              case Block.LitFurnace:
              case Block.SentryTurret:
              case Block.ProximityDetector:
                goto label_21;
              case Block.LockedDoorTop:
                break;
              default:
                goto label_38;
            }
          }
          else
          {
            switch (block)
            {
              case Block.WifiTransmitter:
              case Block.WifiReceiver:
              case Block.ScriptBlock:
                goto label_32;
              case Block.Safe:
                goto label_8;
              case Block.LockedDoorBottom:
                break;
              default:
                goto label_38;
            }
          }
          if (blockID == (byte) 140)
            --p.Y;
          PlayerBlock dataBlock2 = this.GetDataBlock(p) as PlayerBlock;
          if (dataBlock2 != null && !this.HasPermission(playerID, Permissions.Admin) && playerID != GamerID.Sys1)
          {
            Player player = this.instance.GetPlayer(playerID);
            if (!dataBlock2.IsOwner(player))
            {
              clearBlockResult = ClearBlockResult.PermissionDenied;
              goto label_38;
            }
            else
              goto label_38;
          }
          else
            goto label_38;
        }
label_8:
        ChestBlock dataBlock3 = this.GetDataBlock(p) as ChestBlock;
        if (dataBlock3 != null)
        {
          if (this.instance.IsBlockOpen(p).IsGamer)
          {
            clearBlockResult = ClearBlockResult.AnotherPlayerHasThisBlockOpen;
            goto label_38;
          }
          else if (blockID != (byte) 138 && method != UpdateBlockMethod.Generation && (playerID != GamerID.Sys1 && !this.HasPermission(playerID, Permissions.Creative)))
          {
            clearBlockResult = ClearBlockResult.PermissionDenied;
            goto label_38;
          }
          else if (blockID == (byte) 132 && !this.HasPermission(playerID, Permissions.Admin) && playerID != GamerID.Sys1)
          {
            Player player = this.instance.GetPlayer(playerID);
            if (!dataBlock3.IsOwner(player))
            {
              clearBlockResult = ClearBlockResult.PermissionDenied;
              goto label_38;
            }
            else
              goto label_38;
          }
          else
            goto label_38;
        }
        else
          goto label_38;
label_21:
        if (this.GetDataBlock(p) != null)
        {
          if (this.instance.IsBlockOpen(p).IsGamer)
          {
            clearBlockResult = ClearBlockResult.AnotherPlayerHasThisBlockOpen;
            goto label_38;
          }
          else if (method != UpdateBlockMethod.Generation && playerID != (short) -1 && !this.HasPermission(playerID, Permissions.Creative))
          {
            clearBlockResult = ClearBlockResult.PermissionDenied;
            goto label_38;
          }
          else
            goto label_38;
        }
        else
          goto label_38;
label_32:
        if (method != UpdateBlockMethod.Generation && playerID != (short) -1 && !this.HasPermission(playerID, Permissions.Creative))
          clearBlockResult = ClearBlockResult.PermissionDenied;
        if (this.instance.IsBlockOpen(p).IsGamer)
        {
          clearBlockResult = ClearBlockResult.AnotherPlayerHasThisBlockOpen;
          goto label_38;
        }
        else
          goto label_38;
label_36:
        if (this.instance.IsBlockOpen(p).IsGamer)
          clearBlockResult = ClearBlockResult.AnotherPlayerHasThisBlockOpen;
label_38:
        if (!isRelatedClear && Globals2.GetGamerType(playerID) != GamerType.ScriptMove && this.IsInZoneType(p, ZoneType.NoEdit, playerID))
          clearBlockResult = ClearBlockResult.NoEditZone;
        if (clearBlockResult == ClearBlockResult.PermissionDenied)
        {
          Player player = this.instance.GetPlayer(playerID);
          if (player != null && player.IsGod)
            clearBlockResult = ClearBlockResult.Success;
        }
      }
      if (clearBlockResult == ClearBlockResult.Success && method == UpdateBlockMethod.Generation || method == UpdateBlockMethod.Blast)
      {
        switch (blockID)
        {
          case 56:
            clearBlockResult = ClearBlockResult.PermissionDenied;
            break;
          case 57:
            clearBlockResult = ClearBlockResult.PermissionDenied;
            break;
          default:
            if (p.Y < this.map.MapBound.Max.Y - 1)
            {
              ++p.Y;
              switch (this.map.GetBlockID(p))
              {
                case 56:
                case 57:
                  clearBlockResult = ClearBlockResult.PermissionDenied;
                  break;
              }
              --p.Y;
              break;
            }
            break;
        }
      }
      return clearBlockResult;
    }

    private bool HasPermission(GamerID playerID, Permissions permission)
    {
      Player player = this.instance.GetPlayer(playerID);
      if (player == null)
        return true;
      return player.HasPermission(permission);
    }

    public DataBlock GetDataBlock(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      DataBlock dataBlock;
      lock (this.dataBlocks)
        this.dataBlocks.TryGetValue(globalHashCode, out dataBlock);
      return dataBlock;
    }

    public DataBlock GetOrAddDataBlock(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method,
      GamerID playerID,
      bool replace)
    {
      DataBlock dataBlock = this.GetDataBlock(p);
      if (dataBlock == null || replace && dataBlock.ClassType != this.map.BlockData[(int) blockID].ClassType)
        dataBlock = this.AddDataBlock(p, blockID, method, playerID);
      return dataBlock;
    }

    private DataBlock AddDataBlock(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method)
    {
      return this.AddDataBlock(p, blockID, method, Globals2.GetGamerType(GamerType.Generation));
    }

    private DataBlock AddDataBlock(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      DataBlock block = this.NewDataBlock(p, blockID, playerID);
      this.AddDataBlock(block, method, method != UpdateBlockMethod.Copy && Globals2.GetGamerType(playerID) != GamerType.ScriptMove);
      return block;
    }

    private void AddDataBlockToLocalLists(DataBlock block, UpdateBlockMethod method)
    {
      FireBlock fireBlock = block as FireBlock;
      if (fireBlock != null)
      {
        lock (this.burningFireBlocks)
        {
          if (this.burningFireBlocks.Contains(fireBlock))
            return;
          this.burningFireBlocks.Add(fireBlock);
        }
      }
      else
      {
        CropBlock cropBlock = block as CropBlock;
        if (cropBlock != null)
        {
          lock (this.cropBlocks)
          {
            if (this.cropBlocks.Contains(cropBlock))
              return;
            this.cropBlocks.Add(cropBlock);
          }
        }
        else
        {
          SignBlock signBlock = block as SignBlock;
          if (signBlock != null)
          {
            lock (this.signBlocks)
            {
              if (this.signBlocks.Contains(signBlock))
                return;
              this.signBlocks.Add(signBlock);
              if (!signBlock.HasText)
                return;
              this.instance.MapRenderer.SignsChanged(true);
            }
          }
          else
          {
            HealthBlock healthBlock = block as HealthBlock;
            if (healthBlock != null)
            {
              lock (this.healthBlocks)
              {
                if (this.healthBlocks.Contains(healthBlock))
                  return;
                this.healthBlocks.Add(healthBlock);
              }
            }
            else
            {
              TeleportBlock teleportBlock = block as TeleportBlock;
              if (teleportBlock != null)
              {
                lock (this.teleports)
                {
                  if (this.teleports.Contains(teleportBlock))
                    return;
                  this.teleports.Add(teleportBlock);
                }
              }
              else
              {
                NpcSpawnBlock block1 = block as NpcSpawnBlock;
                if (block1 != null)
                {
                  if (this.instance.NpcManager == null)
                    return;
                  this.instance.NpcManager.NpcSpawnAdded(block1);
                }
                else
                {
                  FurnaceBlock furnaceBlock = block as FurnaceBlock;
                  if (furnaceBlock != null)
                  {
                    furnaceBlock.FurnaceBurnStarted += new EventHandler(this.OnFurnaceBurnStarted);
                    furnaceBlock.FurnaceBurnEnded += new EventHandler(this.OnFurnaceBurnEnded);
                  }
                  else
                  {
                    AmbientSoundBlock block2 = block as AmbientSoundBlock;
                    if (block2 != null)
                    {
                      this.instance.AmbientSoundManager.SetBlock(block2);
                    }
                    else
                    {
                      SentryTurretBlock sentryTurretBlock = block as SentryTurretBlock;
                      if (sentryTurretBlock != null)
                      {
                        lock (this.sentryTurrets)
                        {
                          if (this.sentryTurrets.Contains(sentryTurretBlock))
                            return;
                          this.sentryTurrets.Add(sentryTurretBlock);
                        }
                      }
                      else
                      {
                        ParticleEmitterBlock particleEmitterBlock = block as ParticleEmitterBlock;
                        if (particleEmitterBlock != null)
                        {
                          lock (this.particleEmitters)
                          {
                            if (this.particleEmitters.Contains(particleEmitterBlock))
                              return;
                            this.particleEmitters.Add(particleEmitterBlock);
                          }
                        }
                        else
                        {
                          ProximityDetectorBlock proximityDetectorBlock = block as ProximityDetectorBlock;
                          if (proximityDetectorBlock == null)
                            return;
                          lock (this.proximityDetectors)
                          {
                            if (this.proximityDetectors.Contains(proximityDetectorBlock))
                              return;
                            this.proximityDetectors.Add(proximityDetectorBlock);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    private void RemoveDataBlockFromLocalLists(DataBlock block)
    {
      FireBlock fireBlock = block as FireBlock;
      if (fireBlock != null)
      {
        lock (this.burningFireBlocks)
          this.burningFireBlocks.Remove(fireBlock);
      }
      CropBlock cropBlock = block as CropBlock;
      if (cropBlock != null)
      {
        lock (this.cropBlocks)
          this.cropBlocks.Remove(cropBlock);
      }
      else
      {
        StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock = block as StudioForge.TotalMiner.Blocks.MarkerBlock;
        if (markerBlock != null)
        {
          lock (this.markerBlocks)
            this.markerBlocks.Remove(markerBlock);
        }
        else
        {
          SignBlock sign = block as SignBlock;
          if (sign != null)
          {
            lock (this.signBlocks)
              this.signBlocks.Remove(sign);
            if (!SignBlock.RemoveText(this.map, this.signBlocks, sign))
              return;
            this.instance.MapRenderer.SignsChanged(true);
          }
          else
          {
            HealthBlock healthBlock = block as HealthBlock;
            if (healthBlock != null)
            {
              lock (this.healthBlocks)
                this.healthBlocks.Remove(healthBlock);
              this.instance.MapRenderer.HealthBlockChanged();
            }
            else
            {
              TeleportBlock teleportBlock = block as TeleportBlock;
              if (teleportBlock != null)
              {
                lock (this.teleports)
                  this.teleports.Remove(teleportBlock);
              }
              else
              {
                NpcSpawnBlock block1 = block as NpcSpawnBlock;
                if (block1 != null)
                {
                  if (this.instance.NpcManager == null)
                    return;
                  this.instance.NpcManager.NpcSpawnRemoved(block1);
                }
                else
                {
                  FurnaceBlock furnaceBlock = block as FurnaceBlock;
                  if (furnaceBlock != null)
                  {
                    furnaceBlock.FurnaceBurnStarted -= new EventHandler(this.OnFurnaceBurnStarted);
                    furnaceBlock.FurnaceBurnEnded -= new EventHandler(this.OnFurnaceBurnEnded);
                  }
                  else
                  {
                    AmbientSoundBlock block2 = block as AmbientSoundBlock;
                    if (block2 != null)
                    {
                      if (this.instance.AmbientSoundManager == null)
                        return;
                      this.instance.AmbientSoundManager.RemoveBlock(block2);
                    }
                    else
                    {
                      SentryTurretBlock sentryTurretBlock = block as SentryTurretBlock;
                      if (sentryTurretBlock != null)
                      {
                        lock (this.sentryTurrets)
                          this.sentryTurrets.Remove(sentryTurretBlock);
                      }
                      else
                      {
                        ParticleEmitterBlock particleEmitterBlock = block as ParticleEmitterBlock;
                        if (particleEmitterBlock != null)
                        {
                          lock (this.particleEmitters)
                            this.particleEmitters.Remove(particleEmitterBlock);
                        }
                        else
                        {
                          ProximityDetectorBlock proximityDetectorBlock = block as ProximityDetectorBlock;
                          if (proximityDetectorBlock == null)
                            return;
                          lock (this.proximityDetectors)
                            this.proximityDetectors.Remove(proximityDetectorBlock);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    public void AddDataBlock(DataBlock block, UpdateBlockMethod method)
    {
      this.AddDataBlock(block, method, true);
    }

    public void AddDataBlock(DataBlock block, UpdateBlockMethod method, bool addToLocalLists)
    {
      if (block == null)
        return;
      long globalHashCode = this.map.GetGlobalHashCode(block.Point);
      bool flag = true;
      lock (this.dataBlocks)
      {
        DataBlock block1;
        if (this.dataBlocks.TryGetValue(globalHashCode, out block1))
        {
          if (block1 != block)
          {
            this.dataBlocks.Remove(globalHashCode);
            this.RemoveDataBlockFromLocalLists(block1);
          }
          else
            flag = false;
        }
        if (flag)
          this.dataBlocks.Add(globalHashCode, block);
      }
      if (!addToLocalLists || !flag && method != UpdateBlockMethod.Generation)
        return;
      this.AddDataBlockToLocalLists(block, method);
    }

    public DataBlock NewDataBlock(GlobalPoint3D p, Block blockID, GamerID playerID)
    {
      if (blockID >= Block.zLastBlockID)
        return ModManager.GetPluginBlocks(blockID).NewDataBlock(p, blockID, playerID);
      Block block = blockID;
      if ((uint) block <= 114U)
      {
        if ((uint) block <= 50U)
        {
          switch (block)
          {
            case Block.Obsidian:
              return (DataBlock) new TeleportBlock(p);
            case Block.Bookcase:
              return (DataBlock) new BookcaseBlock(p);
            case Block.Furnace:
              break;
            case Block.Chest:
              goto label_17;
            default:
              goto label_40;
          }
        }
        else if ((uint) block <= 78U)
        {
          switch (block)
          {
            case Block.ItemShop:
            case Block.BlockShop:
              return (DataBlock) new ShopBlock(p, (Inventory) null);
            case Block.HealthBlock:
              return (DataBlock) new HealthBlock(p);
            default:
              goto label_40;
          }
        }
        else if (block != Block.SpiderEgg)
        {
          if (block == Block.AmbientSoundBlock)
            return (DataBlock) new AmbientSoundBlock(p);
          goto label_40;
        }
        else
          goto label_31;
      }
      else if ((uint) block <= 143U)
      {
        if ((uint) block <= 121U)
        {
          if (block == Block.Sign)
            return (DataBlock) new SignBlock(p);
          if (block == Block.Book)
            return (DataBlock) new BookBlock(p);
          goto label_40;
        }
        else
        {
          switch (block)
          {
            case Block.ParticleEmitter:
              return (DataBlock) new ParticleEmitterBlock(p);
            case Block.LockedChest:
              ChestBlock chestBlock = new ChestBlock(p, blockID, (Inventory) null);
              if (playerID.IsGamer)
              {
                Player player = this.instance.GetPlayer(playerID);
                if (player != null)
                  chestBlock.Gamertag = player.Gamertag;
              }
              return (DataBlock) chestBlock;
            case Block.LitFurnace:
              break;
            case Block.NPCSpawn:
              goto label_31;
            case Block.Crate:
              goto label_17;
            case Block.SentryTurret:
              return (DataBlock) new SentryTurretBlock(p, this.instance.GetPlayer(playerID));
            case Block.ProximityDetector:
              return (DataBlock) new ProximityDetectorBlock(p, this.instance.GetPlayer(playerID));
            default:
              goto label_40;
          }
        }
      }
      else if ((uint) block <= 172U)
      {
        switch (block)
        {
          case Block.WifiTransmitter:
            return (DataBlock) new WifiTransmitterBlock(p);
          case Block.WifiReceiver:
            return (DataBlock) new WifiReceiverBlock(p);
          case Block.Safe:
            goto label_17;
          case Block.Sundial:
            return (DataBlock) new SundialBlock(p);
          case Block.ScriptBlock:
            return (DataBlock) new ScriptBlock(p);
          default:
            goto label_40;
        }
      }
      else
      {
        switch (block)
        {
          case Block.LockedDoorBottom:
            return (DataBlock) new DoorBlock(p, this.instance.GetPlayer(playerID));
          case Block.Marker:
            return (DataBlock) new StudioForge.TotalMiner.Blocks.MarkerBlock(p);
          case Block.ExcludeMarker:
            return (DataBlock) new StudioForge.TotalMiner.Blocks.MarkerBlock(p)
            {
              Exclude = true
            };
          default:
            goto label_40;
        }
      }
      return (DataBlock) new FurnaceBlock((Map) this.map, p);
label_17:
      return (DataBlock) new ChestBlock(p, blockID, (Inventory) null);
label_31:
      return (DataBlock) new NpcSpawnBlock(p);
label_40:
      return (DataBlock) null;
    }

    public void RemoveDataBlock(DataBlock block)
    {
      this.RemoveDataBlock(block.Point, GamerType.Gamer);
    }

    public void RemoveDataBlock(DataBlock block, GamerType gamerType)
    {
      if (block == null)
        return;
      this.RemoveDataBlock(block.Point, gamerType);
    }

    public void RemoveDataBlock(long hash)
    {
      this.RemoveDataBlockCore(hash, GamerType.Generation);
    }

    private DataBlock RemoveDataBlock(GlobalPoint3D p, GamerType gamerType)
    {
      return this.RemoveDataBlockCore(this.map.GetGlobalHashCode(p), gamerType);
    }

    private DataBlock RemoveDataBlockCore(long hash, GamerType gamerType)
    {
      DataBlock block;
      lock (this.dataBlocks)
      {
        if (this.dataBlocks.TryGetValue(hash, out block))
        {
          this.dataBlocks.Remove(hash);
          if (gamerType != GamerType.ScriptMove)
            this.RemoveDataBlockFromLocalLists(block);
        }
      }
      return block;
    }

    private void TreeBlockCleared(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID,
      bool naturalWood)
    {
      if (!naturalWood || !this.map.IsHost || method == UpdateBlockMethod.Generation)
        return;
      this.instance.TreeRemoved(p);
      if (!this.instance.IsFiniteResources || this.map.Random.Next(8) != 0)
        return;
      this.SpawnSapling(p, playerID);
    }

    private void SpawnSapling(GlobalPoint3D p, GamerID playerID)
    {
      int num = 5;
      do
        ;
      while (--num > 0 && !this.SpawnSaplingCore(p, playerID));
    }

    private bool SpawnSaplingCore(GlobalPoint3D p, GamerID playerID)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      int num1 = this.map.Random.Next(10);
      int num2;
      for (num2 = this.map.Random.Next(10); num1 > 2 && num1 < 7 && (num2 > 2 && num2 < 7); num2 = this.map.Random.Next(10))
        num1 = this.map.Random.Next(10);
      p1.X = p.X + (num1 - 5);
      p1.Z = p.Z + (num2 - 5);
      if (this.map.IsValidPoint(p1) && p1.Y < this.map.MapBound.Max.Y - 1)
      {
        p1.Y = (int) this.map.GetRegion(p1).GetHeight(p1);
        if (BlockData.IsGrassOrDirt((Block) this.map.GetBlockID(p1)))
        {
          ++p1.Y;
          if (this.map.GetBlockID(p1) == (byte) 0 && !this.IsInZoneType(p1, ZoneType.NoEdit, playerID))
          {
            this.instance.AddSapling(p1 + GlobalPoint3D.Up * 3, playerID);
            return true;
          }
        }
      }
      return false;
    }

    public bool SpawnTree(GlobalPoint3D p, bool transmit)
    {
      if (this.map.GetBlockID(p) == (byte) 58 && BlockData.IsGrassOrDirt((Block) this.map.GetBlockID(p + GlobalPoint3D.Down)) && this.map.IsClearAndAbove(p + GlobalPoint3D.Up, 1))
      {
        --p.Y;
        BoundingBox blockBox = this.instance.GetBlockBox(p);
        blockBox.Min -= new Vector3(this.map.TileSize);
        blockBox.Max += new Vector3(this.map.TileSize, this.map.TileSize * 4f, this.map.TileSize);
        if (this.instance.GetFirstHitTarget(blockBox, HitTargetOptions.PlayersAndNpcs, false).Target == null)
        {
          VegetationGenerator.AddTree(this.instance, (Map) this.map, p, this.instance.Random, UpdateBlockMethod.Strategy, transmit);
          this.map.Commit();
          return true;
        }
      }
      return false;
    }

    public void AddTimedBlock(GlobalPoint3D p, float timer, int data, GamerID playerID)
    {
      this.AddTimedBlock(p, (Block) this.map.GetBlockID(p), timer, data, playerID);
    }

    public void AddTimedBlock(
      GlobalPoint3D p,
      Block blockID,
      float timer,
      int data,
      GamerID playerID)
    {
      MapStrategyTM.TimedBlock timedBlock = new MapStrategyTM.TimedBlock()
      {
        Point = p,
        BlockID = blockID,
        Timer = timer,
        PlayerID = playerID,
        Data = data
      };
      lock (this.timedBlocks)
        this.timedBlocks.Add(timedBlock);
    }

    public bool ActivateTimedBlock(GlobalPoint3D p)
    {
      lock (this.timedBlocks)
      {
        for (int index = 0; index < this.timedBlocks.Count; ++index)
        {
          MapStrategyTM.TimedBlock timedBlock = this.timedBlocks[index];
          if (timedBlock.Point == p)
          {
            timedBlock.Timer = 0.0f;
            this.timedBlocks[index] = timedBlock;
            return true;
          }
        }
      }
      return false;
    }

    public void RemoveTimedBlock(GlobalPoint3D p, Block blockID)
    {
      lock (this.timedBlocks)
      {
        for (int index = 0; index < this.timedBlocks.Count; ++index)
        {
          MapStrategyTM.TimedBlock timedBlock = this.timedBlocks[index];
          if (timedBlock.BlockID == blockID && timedBlock.Point == p)
          {
            this.timedBlocks.RemoveAt(index);
            break;
          }
        }
      }
    }

    private void UpdateTimedBlocks()
    {
      lock (this.timedBlocks)
      {
        for (int index = this.timedBlocks.Count - 1; index >= 0; --index)
        {
          MapStrategyTM.TimedBlock timedBlock = this.timedBlocks[index];
          timedBlock.Timer -= Services.ElapsedTime;
          if ((double) timedBlock.Timer <= 0.0)
          {
            if (this.ActivateTimedBlock(timedBlock))
              this.timedBlocks.Remove(timedBlock);
          }
          else
            this.timedBlocks[index] = timedBlock;
        }
      }
    }

    private void TimedBlockAdded(GlobalPoint3D p, Block blockID, GamerID playerID)
    {
      switch (blockID)
      {
        case Block.Torch:
          if (this.map.IsNextTo(p, (byte) 55))
          {
            this.AddDetonator(p, Block.Torch, Block.C4, playerID);
            break;
          }
          if (!this.map.IsNextTo(p, (byte) 54))
            break;
          this.AddDetonator(p, Block.Torch, Block.TNT, playerID);
          break;
        case Block.Sapling:
          this.AddSaplingSeed(p);
          break;
      }
    }

    private void AddDetonator(GlobalPoint3D p, Block detonator, Block explosive, GamerID playerID)
    {
      if (!this.instance.IsHost)
        return;
      this.AddTimedBlock(p, detonator, this.GetTimer(detonator, explosive), 0, playerID);
    }

    private void AddSaplingSeed(GlobalPoint3D p)
    {
      if (!this.instance.IsHost || !this.instance.IsFiniteResources)
        return;
      this.AddTimedBlock(p, Block.Sapling, 60f, 0, GamerID.Sys1);
    }

    public bool TimedBlockExists(GlobalPoint3D p)
    {
      lock (this.timedBlocks)
      {
        for (int index = 0; index < this.timedBlocks.Count; ++index)
        {
          if (this.timedBlocks[index].Point == p)
            return true;
        }
      }
      return false;
    }

    private float GetTimer(Block block1, Block block2)
    {
      return 5f;
    }

    private bool ActivateTimedBlock(MapStrategyTM.TimedBlock block)
    {
      if (block.BlockID == Block.Button)
      {
        this.instance.ReleaseButton(block.Point, UpdateBlockMethod.Strategy, block.PlayerID);
        return true;
      }
      if (this.map.IsHost)
      {
        switch (block.BlockID)
        {
          case Block.Torch:
            this.ActivateTorch(block);
            return true;
          case Block.Sapling:
            return this.SpawnTree(block.Point, true);
          case Block.Rope:
            return this.ActivateRope(block.Point, (MapStrategyTM.RopeActivation) block.Data);
        }
      }
      return true;
    }

    private void ActivateTorch(MapStrategyTM.TimedBlock block)
    {
      if (this.map.IsNextTo(block.Point, (byte) 55))
      {
        this.IgniteExplosive(this.GetNeighbour(block.Point, Block.C4), Block.C4, block.PlayerID);
      }
      else
      {
        if (!this.map.IsNextTo(block.Point, (byte) 54))
          return;
        this.IgniteExplosive(this.GetNeighbour(block.Point, Block.TNT), Block.TNT, block.PlayerID);
      }
    }

    private void IgniteExplosive(GlobalPoint3D p, Block blockID, GamerID playerID)
    {
      if (!this.instance.HasPermission(playerID, Permissions.Grief))
        return;
      int num = (int) this.map.ClearBlock(p, UpdateBlockMethod.Strategy, playerID, true);
      this.instance.CreateBlast(p, blockID, playerID);
    }

    private void IgniteExplosive(
      GlobalPoint3D p,
      Block blockID,
      float strength,
      int radius,
      GamerID playerID)
    {
      if (!this.instance.HasPermission(playerID, Permissions.Grief))
        return;
      int num = (int) this.map.ClearBlock(p, UpdateBlockMethod.Strategy, playerID, true);
      this.instance.CreateBlast(p, (Item) blockID, strength, radius, playerID);
    }

    public void ResetAllButtons()
    {
      lock (this.timedBlocks)
      {
        for (int index = this.timedBlocks.Count - 1; index >= 0; --index)
        {
          if (this.timedBlocks[index].BlockID == Block.Button)
          {
            this.instance.ReleaseButton(this.timedBlocks[index].Point, UpdateBlockMethod.Strategy, this.timedBlocks[index].PlayerID);
            this.timedBlocks.RemoveAt(index);
          }
        }
      }
    }

    public BookData GetBookData(ushort id)
    {
      BookData bookData;
      lock (this.bookData)
        this.bookData.TryGetValue(id, out bookData);
      return bookData;
    }

    public BookData AddBookData(BookData book)
    {
      lock (this.bookData)
      {
        if (this.bookData.Count < (int) ushort.MaxValue)
        {
          if (book == null)
            book = new BookData();
          if (book.ID < (ushort) 2)
            book.ID = this.GetNextFreeBookBlockID();
          if (!this.bookData.ContainsKey(book.ID))
          {
            this.bookData.Add(book.ID, book);
            return book;
          }
        }
      }
      return (BookData) null;
    }

    private ushort GetNextFreeBookBlockID()
    {
      this.bookIDList.Clear();
      this.bookIDList.Add((ushort) 1);
      foreach (BookData bookData in this.bookData.Values)
        this.bookIDList.Add(bookData.ID);
      this.bookIDList.Sort();
      ushort num = 1;
      for (int index = 0; index < this.bookIDList.Count - 1; ++index)
      {
        num = (ushort) ((uint) this.bookIDList[index] + 1U);
        if ((int) this.bookIDList[index + 1] > (int) num)
          return num;
      }
      return (ushort) ((uint) num + 1U);
    }

    private void UpdateEnvironment()
    {
      if (this.envManager == null)
        return;
      if (this.instance.IsWeatherEnabled && this.map.IsHost)
      {
        if (Globals2.GameSettings.ViewClouds)
        {
          if (this.envManager.RainCount < 3)
            this.AddRainfall();
          if (this.envManager.HailCount < 2)
            this.AddHailStorm();
        }
        if (this.envManager.FogCount == 0)
          this.AddFog();
      }
      this.envManager.Update();
    }

    private void AddRainfall()
    {
      double frequency;
      int duration;
      float intensity;
      this.GetRainSettings(this.instance.SunMoon.Season, out frequency, out duration, out intensity);
      if (!this.map.Random.RandomChanceTime(frequency))
        return;
      Player randomLocalPlayer = this.instance.GetRandomLocalPlayer();
      if (randomLocalPlayer == null)
        return;
      int val1 = this.map.MapBound.Min.X + (this.map.MapBound.Max.X - this.map.MapBound.Min.X) / 4;
      int val2 = this.map.MapBound.Min.Z + (this.map.MapBound.Max.Z - this.map.MapBound.Min.Z) / 4;
      GlobalPoint3D point = this.map.GetPoint(randomLocalPlayer.Position);
      int num = this.map.Random.Next(100) + Math.Max(val1, val2);
      this.envManager.AddRain(point, (float) num, (float) duration, intensity, true);
      this.envManager.AddFog(point, (float) num, (float) duration, intensity, true);
    }

    private void GetRainSettings(
      SeasonType season,
      out double frequency,
      out int duration,
      out float intensity)
    {
      switch (this.instance.SunMoon.Season)
      {
        case SeasonType.Autumn:
          frequency = 700.0;
          duration = this.map.Random.Next(60) + 60;
          intensity = (float) (0.5 + this.map.Random.NextDouble() * 0.5);
          break;
        case SeasonType.Winter:
          frequency = 500.0;
          duration = this.map.Random.Next(120) + 60;
          intensity = Math.Min(1f, (float) (0.800000011920929 + this.map.Random.NextDouble() * 0.300000011920929));
          break;
        case SeasonType.Spring:
          frequency = 600.0;
          duration = this.map.Random.Next(30) + 30;
          intensity = (float) (0.800000011920929 + this.map.Random.NextDouble() * 0.200000002980232);
          break;
        default:
          frequency = 900.0;
          duration = this.map.Random.Next(20) + 30;
          intensity = (float) (0.300000011920929 + this.map.Random.NextDouble() * 0.699999988079071);
          break;
      }
    }

    private void AddHailStorm()
    {
      double frequency;
      int duration;
      float intensity;
      this.GetHailSettings(this.instance.SunMoon.Season, out frequency, out duration, out intensity);
      if (!this.map.Random.RandomChanceTime(frequency))
        return;
      Player randomLocalPlayer = this.instance.GetRandomLocalPlayer();
      if (randomLocalPlayer == null)
        return;
      int val1 = this.map.MapBound.Min.X + (this.map.MapBound.Max.X - this.map.MapBound.Min.X) / 4;
      int val2 = this.map.MapBound.Min.Z + (this.map.MapBound.Max.Z - this.map.MapBound.Min.Z) / 4;
      GlobalPoint3D point = this.map.GetPoint(randomLocalPlayer.Position);
      int num = this.map.Random.Next(100) + Math.Max(val1, val2);
      this.envManager.AddHail(point, (float) num, (float) duration, intensity, true);
      this.envManager.AddFog(point, (float) num, (float) duration, intensity * 0.75f, true);
    }

    private void GetHailSettings(
      SeasonType season,
      out double frequency,
      out int duration,
      out float intensity)
    {
      switch (this.instance.SunMoon.Season)
      {
        case SeasonType.Autumn:
          frequency = 1700.0;
          duration = this.map.Random.Next(30) + 50;
          intensity = (float) (0.600000023841858 + this.map.Random.NextDouble() * 0.400000005960464);
          break;
        case SeasonType.Winter:
          frequency = 1300.0;
          duration = this.map.Random.Next(50) + 60;
          intensity = (float) (0.800000011920929 + this.map.Random.NextDouble() * 0.200000002980232);
          break;
        case SeasonType.Spring:
          frequency = 2400.0;
          duration = this.map.Random.Next(30) + 30;
          intensity = (float) (0.400000005960464 + this.map.Random.NextDouble() * 0.600000023841858);
          break;
        default:
          frequency = 2800.0;
          duration = this.map.Random.Next(20) + 30;
          intensity = (float) (0.200000002980232 + this.map.Random.NextDouble() * 0.800000011920929);
          break;
      }
    }

    private void AddFog()
    {
      double frequency;
      int duration;
      float intensity;
      this.GetFogSettings(this.instance.SunMoon.Season, out frequency, out duration, out intensity);
      if (!this.map.Random.RandomChanceTime(frequency))
        return;
      Player randomLocalPlayer = this.instance.GetRandomLocalPlayer();
      if (randomLocalPlayer == null)
        return;
      int val1 = this.map.MapBound.Min.X + (this.map.MapBound.Max.X - this.map.MapBound.Min.X) / 4;
      int val2 = this.map.MapBound.Min.Z + (this.map.MapBound.Max.Z - this.map.MapBound.Min.Z) / 4;
      GlobalPoint3D point = this.map.GetPoint(randomLocalPlayer.Position);
      int num = this.map.Random.Next(100) + Math.Max(val1, val2);
      int visibility = this.map.Random.Next(30, 150);
      this.envManager.AddFog(point, (float) num, (float) duration, 10f, intensity, Color.White, visibility, true);
    }

    private void GetFogSettings(
      SeasonType season,
      out double frequency,
      out int duration,
      out float intensity)
    {
      switch (this.instance.SunMoon.Season)
      {
        case SeasonType.Autumn:
          frequency = 2000.0;
          duration = this.map.Random.Next(30) + 50;
          intensity = (float) (0.5 + this.map.Random.NextDouble() * 0.5);
          break;
        case SeasonType.Winter:
          frequency = 1600.0;
          duration = this.map.Random.Next(50) + 60;
          intensity = (float) (0.699999988079071 + this.map.Random.NextDouble() * 0.300000011920929);
          break;
        case SeasonType.Spring:
          frequency = 2700.0;
          duration = this.map.Random.Next(30) + 30;
          intensity = (float) (0.300000011920929 + this.map.Random.NextDouble() * 0.699999988079071);
          break;
        default:
          frequency = 3100.0;
          duration = this.map.Random.Next(20) + 30;
          intensity = (float) (0.200000002980232 + this.map.Random.NextDouble() * 0.800000011920929);
          break;
      }
    }

    public void RemoveAllWeather()
    {
      if (this.envManager == null)
        return;
      this.envManager.RemoveAllWeather();
    }

    public ScriptBlock AddScriptBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      ScriptBlock scriptBlock = this.GetDataBlock(p) as ScriptBlock;
      if (scriptBlock == null)
      {
        scriptBlock = new ScriptBlock(p);
        this.AddDataBlock((DataBlock) scriptBlock, method);
      }
      return scriptBlock;
    }

    private void AddMarkerBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID,
      bool exclude)
    {
      if (!playerID.IsGamer)
        return;
      StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock = new StudioForge.TotalMiner.Blocks.MarkerBlock(p, playerID)
      {
        Exclude = exclude
      };
      this.AddDataBlock((DataBlock) markerBlock, method);
      lock (this.markerBlocks)
        this.markerBlocks.Add(markerBlock);
    }

    private void AddTeleport(
      GlobalPoint3D p,
      byte channel,
      UpdateBlockMethod method,
      bool transmit)
    {
      if (p.Y >= this.map.MapBound.Max.Y - 2)
        return;
      ++p.Y;
      byte blockId1 = this.map.GetBlockID(p);
      switch (blockId1)
      {
        case 0:
        case 53:
          ++p.Y;
          byte blockId2 = this.map.GetBlockID(p);
          switch (blockId2)
          {
            case 0:
            case 53:
              if (blockId2 == (byte) 0)
                this.map.SetBlockData(p, (byte) 53, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
              --p.Y;
              if (blockId1 == (byte) 0)
                this.map.SetBlockData(p, (byte) 53, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
              --p.Y;
              this.AddTeleportAndChannel(p, channel, method);
              return;
            default:
              return;
          }
      }
    }

    private TeleportBlock AddTeleportAndChannel(
      GlobalPoint3D p,
      byte channel,
      UpdateBlockMethod method)
    {
      TeleportBlock orAddDataBlock = this.GetOrAddDataBlock(p, Block.Obsidian, method, GamerID.Sys1, true) as TeleportBlock;
      orAddDataBlock.Channel = channel;
      return orAddDataBlock;
    }

    private void RemoveTeleport(GlobalPoint3D p)
    {
      if (p.Y < this.map.MapBound.Max.Y - 1)
      {
        int y = p.Y;
        ++p.Y;
        if (this.map.GetBlockID(p) == (byte) 53)
        {
          this.map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
          if (p.Y < this.map.MapBound.Max.Y - 1)
          {
            ++p.Y;
            if (this.map.GetBlockID(p) == (byte) 53)
              this.map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
          }
        }
        p.Y = y;
      }
      this.RemoveDataBlock(p, GamerType.Gamer);
    }

    public GlobalPoint3D GetRandomTeleport(Player player, GlobalPoint3D portal)
    {
      if (this.teleports.Count > 1)
      {
        int auxHighDataNoCache = (int) this.map.GetAuxHighDataNoCache(portal);
        if (player != null)
        {
          Block blockTextureId = this.instance.Map.GetBlockTextureID(Block.Obsidian, auxHighDataNoCache);
          if (!player.IsAdmin && blockTextureId == Block.ColorRed)
            return portal;
        }
        int num = 200;
        while (--num > 0)
        {
          TeleportBlock teleport = this.teleports[this.instance.Random.Next(this.teleports.Count)];
          if (teleport.Point != portal && (int) teleport.Channel == auxHighDataNoCache)
            return teleport.Point;
        }
      }
      return portal;
    }

    public bool TeleportExists(GlobalPoint3D p)
    {
      return this.GetDataBlock(p) is TeleportBlock;
    }

    public bool GetTeleportChannel(GlobalPoint3D p, out byte channel)
    {
      TeleportBlock dataBlock = this.GetDataBlock(p) as TeleportBlock;
      channel = dataBlock == null ? (byte) 0 : dataBlock.Channel;
      return dataBlock != null;
    }

    private void AddRope(GlobalPoint3D p)
    {
      this.AddTimedBlock(p, Block.Rope, 0.1f, 0, GamerID.Sys1);
    }

    private void RopeCleared(GlobalPoint3D p)
    {
      --p.Y;
      if (this.map.GetBlockID(p) != (byte) 72)
        return;
      this.AddTimedBlock(p, Block.Rope, 0.1f, 1, GamerID.Sys1);
    }

    private bool ActivateRope(GlobalPoint3D p, MapStrategyTM.RopeActivation activation)
    {
      if (activation == MapStrategyTM.RopeActivation.Cut)
      {
        if (this.map.GetBlockID(p) == (byte) 72)
        {
          this.map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
          this.map.Commit();
        }
      }
      else
      {
        --p.Y;
        if (p.Y > this.map.MapBound.Min.Y && this.map.GetBlockID(p) == (byte) 0)
        {
          ++p.Y;
          if (this.GetRopeLength(p) < 80)
          {
            --p.Y;
            this.map.SetBlockData(p, (byte) 72, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            this.map.Commit();
          }
        }
      }
      return true;
    }

    private int GetRopeLength(GlobalPoint3D p)
    {
      int num = 0;
      while (this.map.GetBlockID(p) == (byte) 72)
      {
        ++p.Y;
        ++num;
      }
      return num;
    }

    private ChestBlock AddChestBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      int inventorySize)
    {
      ChestBlock chestBlock = new ChestBlock(p, inventorySize);
      this.AddDataBlock((DataBlock) chestBlock, method);
      return chestBlock;
    }

    private ChestBlock AddLockedChestBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID,
      Block blockID)
    {
      if (method == UpdateBlockMethod.Player && playerID.IsGamer)
      {
        Player player = this.instance.GetPlayer(playerID);
        if (player != null)
        {
          string gamertag = player.Gamertag;
          ChestBlock chestBlock = this.AddChestBlock(p, method, blockID == Block.Crate ? 20 : 50);
          chestBlock.Gamertag = gamertag;
          return chestBlock;
        }
      }
      return (ChestBlock) null;
    }

    private void RemoveChestBlock(GlobalPoint3D p, UpdateBlockMethod method, GamerID playerID)
    {
      ChestBlock chestBlock = this.RemoveDataBlock(p, Globals2.GetGamerType(playerID)) as ChestBlock;
      if (chestBlock == null)
        return;
      switch (method)
      {
        case UpdateBlockMethod.Player:
          if (!playerID.IsGamer)
            break;
          goto case UpdateBlockMethod.Blast;
        case UpdateBlockMethod.Blast:
          this.DropItems(chestBlock.Point, chestBlock.Inventory, UpdateBlockMethod.DropTimeShort, playerID, false);
          break;
      }
    }

    public ChestBlock GetOrAddChest(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method)
    {
      return this.AddChestBlock(p, method, blockID == Block.Crate ? 20 : 50);
    }

    public ChestBlock GetOrCreateBaseChest(
      GlobalPoint3D p,
      Block blockID,
      Inventory inventory)
    {
      ChestBlock chestBlock = this.GetDataBlock(p) as ChestBlock;
      switch (blockID)
      {
        case Block.Bookcase:
          if (!(chestBlock is BookcaseBlock))
          {
            chestBlock = (ChestBlock) new BookcaseBlock(p);
            break;
          }
          break;
        case Block.ItemShop:
        case Block.BlockShop:
          if (!(chestBlock is ShopBlock))
          {
            chestBlock = (ChestBlock) new ShopBlock(p, inventory);
            break;
          }
          break;
        default:
          if (chestBlock == null)
          {
            chestBlock = new ChestBlock(p, blockID, inventory);
            break;
          }
          break;
      }
      return chestBlock;
    }

    public ChestBlock AddChestManually_MapGenerataion(GlobalPoint3D p, Block blockID)
    {
      return this.GetOrAddChest(p, blockID, UpdateBlockMethod.Generation);
    }

    public FurnaceBlock GetOrAddFurnaceBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      return this.GetOrAddFurnaceBlock(p, (Inventory) null, method);
    }

    public FurnaceBlock GetOrAddFurnaceBlock(
      GlobalPoint3D p,
      Inventory inventory,
      UpdateBlockMethod method)
    {
      FurnaceBlock furnaceBlock = this.GetDataBlock(p) as FurnaceBlock;
      if (furnaceBlock == null)
      {
        furnaceBlock = this.AddFurnaceBlock(p, method);
        if (inventory != null)
          furnaceBlock.Inventory = inventory;
      }
      return furnaceBlock;
    }

    private void UpdateActiveFurnaceBlocks()
    {
      lock (this.activeFurnaces)
      {
        for (int index = this.activeFurnaces.Count - 1; index >= 0; --index)
          this.activeFurnaces[index].Update(this.instance);
      }
    }

    private FurnaceBlock AddFurnaceBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      FurnaceBlock furnaceBlock = new FurnaceBlock((Map) this.map, p);
      this.AddDataBlock((DataBlock) furnaceBlock, method);
      return furnaceBlock;
    }

    private void RemoveFurnaceBlock(GlobalPoint3D p, UpdateBlockMethod method, GamerID playerID)
    {
      FurnaceBlock dataBlock = this.GetDataBlock(p) as FurnaceBlock;
      if (dataBlock == null)
        return;
      dataBlock.FurnaceBurnEnded -= new EventHandler(this.OnFurnaceBurnEnded);
      dataBlock.FurnaceBurnStarted -= new EventHandler(this.OnFurnaceBurnStarted);
      switch (method)
      {
        case UpdateBlockMethod.Player:
          if (!playerID.IsGamer)
            break;
          goto case UpdateBlockMethod.Blast;
        case UpdateBlockMethod.Blast:
          this.DropFurnaceItems(dataBlock, playerID);
          break;
      }
      if (Globals2.GetGamerType(playerID) == GamerType.ScriptMove)
        return;
      lock (this.activeFurnaces)
        this.activeFurnaces.Remove(dataBlock);
    }

    private void OnFurnaceBurnStarted(object sender, EventArgs e)
    {
      FurnaceBlock furnaceBlock = sender as FurnaceBlock;
      if (furnaceBlock == null)
        return;
      lock (this.activeFurnaces)
      {
        if (this.activeFurnaces.Contains(furnaceBlock))
          return;
        this.activeFurnaces.Add(furnaceBlock);
      }
    }

    private void OnFurnaceBurnEnded(object sender, EventArgs e)
    {
      FurnaceBlock furnaceBlock = sender as FurnaceBlock;
      if (furnaceBlock == null)
        return;
      lock (this.activeFurnaces)
        this.activeFurnaces.Remove(furnaceBlock);
    }

    private void DropFurnaceItems(FurnaceBlock furnace, GamerID playerID)
    {
      if (!this.map.IsHost)
        return;
      if (furnace.Ore1ItemCount > 0)
        this.instance.DropItem(ParticleType.None, furnace.Point, furnace.Ore1Item, UpdateBlockMethod.DropTimeShort, playerID);
      if (furnace.Ore2ItemCount > 0)
        this.instance.DropItem(ParticleType.None, furnace.Point, furnace.Ore2Item, UpdateBlockMethod.DropTimeShort, playerID);
      if (furnace.ProductItemCount > 0)
        this.instance.DropItem(ParticleType.None, furnace.Point, furnace.ProductItem, UpdateBlockMethod.DropTimeShort, playerID);
      InventoryItem fuelItem = furnace.FuelItem;
      if ((double) furnace.CurrentBurnTime > 0.0)
        --fuelItem.Count;
      if (fuelItem.Count <= 0)
        return;
      this.instance.DropItem(ParticleType.None, furnace.Point, fuelItem, UpdateBlockMethod.DropTimeShort, playerID);
    }

    private void UpdateSentryTurretBlocks()
    {
      lock (this.sentryTurrets)
      {
        if (++this.currentSentryTurret >= this.sentryTurrets.Count)
          this.currentSentryTurret = 0;
        if (this.currentSentryTurret >= this.sentryTurrets.Count)
          return;
        this.sentryTurrets[this.currentSentryTurret].Update(this.map, Services.ElapsedTime * (float) this.sentryTurrets.Count);
      }
    }

    private void RemoveSentryTurretBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      SentryTurretBlock sentryTurretBlock = this.RemoveDataBlock(p, Globals2.GetGamerType(playerID)) as SentryTurretBlock;
      if (sentryTurretBlock == null)
        return;
      switch (method)
      {
        case UpdateBlockMethod.Player:
          if (!playerID.IsGamer)
            break;
          goto case UpdateBlockMethod.Blast;
        case UpdateBlockMethod.Blast:
          this.DropItems(sentryTurretBlock.Point, sentryTurretBlock.Inventory, UpdateBlockMethod.DropTimeShort, playerID, false);
          break;
      }
    }

    private void RemoveDoorBlock(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (Globals2.GetGamerType(playerID) == GamerType.ScriptMove)
        return;
      switch (blockID)
      {
        case Block.WoodDoorTop:
        case Block.SteelDoorTop:
        case Block.LockedDoorTop:
          --p.Y;
          switch ((Block) this.map.GetBlockID(p))
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
            case Block.LockedDoorBottom:
              int num1 = (int) this.map.ClearBlock(p, method, playerID, false, true, false);
              return;
            default:
              return;
          }
        case Block.WoodDoorBottom:
        case Block.SteelDoorBottom:
        case Block.LockedDoorBottom:
          if (blockID == Block.LockedDoorBottom)
            this.RemoveDataBlock(p, Globals2.GetGamerType(playerID));
          ++p.Y;
          switch ((Block) this.map.GetBlockID(p))
          {
            case Block.WoodDoorTop:
            case Block.SteelDoorTop:
            case Block.LockedDoorTop:
              int num2 = (int) this.map.ClearBlock(p, method, playerID, false, true, false);
              return;
            default:
              return;
          }
      }
    }

    private void RemoveBedBlock(
      GlobalPoint3D p,
      MapBlock blockData,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      Block blockId = (Block) blockData.BlockID;
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      switch ((int) blockData.AuxData & 7)
      {
        case 1:
          zero.Z = -1;
          break;
        case 2:
          zero.X = 1;
          break;
        case 3:
          zero.Z = 1;
          break;
        default:
          zero.X = -1;
          break;
      }
      if (blockId == Block.BedHead)
      {
        zero.X = -zero.X;
        zero.Z = -zero.Z;
      }
      p.X += zero.X;
      p.Z += zero.Z;
      switch ((Block) this.map.GetBlockID(p))
      {
        case Block.BedHead:
        case Block.BedFoot:
          int num = (int) this.map.ClearBlock(p, method, playerID, false, true, false);
          break;
      }
    }

    public bool AddSignBlock(GlobalPoint3D p, string text, UpdateBlockMethod method)
    {
      if (text == null || text.Length <= 0)
        return false;
      SignBlock signBlock = new SignBlock(this.map, p, text);
      lock (this.signBlocks)
        this.signBlocks.Add(signBlock);
      this.AddDataBlock((DataBlock) signBlock, method, false);
      return true;
    }

    private void RemovePlateBlock(GlobalPoint3D p)
    {
      this.RemoveCharactersOnPlate(p);
    }

    private void RemoveCharactersOnPlate(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.charactersOnPlate)
        this.charactersOnPlate.Remove(globalHashCode);
    }

    private bool IsCharacterOnPlateChanged(
      GlobalPoint3D p,
      bool pressure,
      GamerID gamerID,
      bool transmit)
    {
      bool flag = false;
      if (gamerID.IsGamer)
      {
        long globalHashCode = this.map.GetGlobalHashCode(p);
        lock (this.charactersOnPlate)
        {
          List<GamerID> gamerIdList;
          if (this.charactersOnPlate.TryGetValue(globalHashCode, out gamerIdList))
          {
            if (pressure)
            {
              if (!gamerIdList.Contains(gamerID))
              {
                gamerIdList.Add(gamerID);
                flag = gamerIdList.Count == 1;
              }
            }
            else if (gamerIdList.Contains(gamerID))
            {
              gamerIdList.Remove(gamerID);
              if (gamerIdList.Count == 0)
              {
                this.charactersOnPlate.Remove(globalHashCode);
                flag = true;
              }
            }
          }
          else if (pressure)
          {
            gamerIdList = new List<GamerID>();
            gamerIdList.Add(gamerID);
            this.charactersOnPlate.Add(globalHashCode, gamerIdList);
            flag = true;
          }
          else
            flag = true;
        }
      }
      return flag;
    }

    public void RemoveCharacterFromPlates(GamerID characterID)
    {
      List<long> longList = new List<long>();
      lock (this.charactersOnPlate)
      {
        foreach (KeyValuePair<long, List<GamerID>> keyValuePair in this.charactersOnPlate)
        {
          if (keyValuePair.Value.Contains(characterID))
            longList.Add(keyValuePair.Key);
        }
      }
      foreach (long hash in longList)
        this.instance.DeliverPower(this.map.GetPointFromGlobalHash(hash), Block.PressurePlate, BlockFace.ProxyDefault, false, UpdateBlockMethod.Strategy, characterID, false, false);
    }

    private void RemoveArcadeMachineBlock(GlobalPoint3D p)
    {
      this.instance.RemoveArcadeMachine(p, UpdateBlockMethod.Player);
    }

    public NpcSpawnBlock AddNpcSpawnBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      return this.AddNpcSpawnBlock(p, Block.NPCSpawn, method);
    }

    private NpcSpawnBlock AddNpcSpawnBlock(
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method)
    {
      NpcSpawnBlock npcSpawnBlock = this.GetDataBlock(p) as NpcSpawnBlock;
      if (npcSpawnBlock == null)
      {
        npcSpawnBlock = new NpcSpawnBlock(p);
        this.AddDataBlock((DataBlock) npcSpawnBlock, method);
      }
      return npcSpawnBlock;
    }

    public NpcSpawnBlock AddSpiderEggManually_MapGenerataion(GlobalPoint3D p)
    {
      return this.AddNpcSpawnBlock(p, Block.SpiderEgg, UpdateBlockMethod.Generation);
    }

    private void AddSliderBlock(GlobalPoint3D p, GamerID playerID)
    {
      if (p.Y <= -this.map.MapSize.Y - 2)
        return;
      --p.Y;
      if (this.map.GetBlockID(p) != (byte) 0)
        return;
      ++p.Y;
      if (!this.instance.CreateSliderBlock(p, playerID, UpdateBlockMethod.Strategy, false))
        return;
      this.instance.GetPlayer(playerID)?.DisableSwingTarget(0.5f);
    }

    private void AddFireBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      if (!this.map.IsHost || this.GetDataBlock(p) is FireBlock)
        return;
      this.AddDataBlock((DataBlock) new FireBlock(p), method);
    }

    public void AddEconomizedShopBlock(GlobalPoint3D p, Player player, UpdateBlockMethod method)
    {
      if (player == null)
        return;
      Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
      switch (blockIdNoCache)
      {
        case Block.ItemShop:
        case Block.BlockShop:
          ShopBlock shopBlock1 = new ShopBlock(p, new Inventory((int) short.MaxValue, 0, 0, true));
          shopBlock1.Gamertag = player.Gamertag;
          shopBlock1.PriceList = (PriceList) null;
          ShopBlock shopBlock2 = shopBlock1;
          if (player.DefaultPriceList == null)
            player.DefaultPriceList = new PriceList(PriceList.PriceListType.PlayerDefault);
          ShopScreen.AddToShopInventory(blockIdNoCache, this.instance, player, shopBlock2.Inventory, true);
          shopBlock2.Inventory = new Inventory((short) (shopBlock2.Inventory.Count + 1), (short) 0, (short) 0, shopBlock2.Inventory);
          shopBlock2.Inventory.SetAllItemCounts((ushort) 0);
          this.AddDataBlock((DataBlock) shopBlock2, method);
          break;
      }
    }

    private WifiTransmitterBlock AddWifiTransmitterBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      WifiTransmitterBlock orAddDataBlock = this.GetOrAddDataBlock(p, Block.WifiTransmitter, method, playerID, true) as WifiTransmitterBlock;
      if (this.map.IsHost && method != UpdateBlockMethod.Paste)
      {
        ushort transmitterFrequency = this.GetNextFreeTransmitterFrequency();
        orAddDataBlock.Frequency = transmitterFrequency;
        Player player = this.instance.GetPlayer(playerID);
        if (player != null)
          player.LastTransmitterFrequency = transmitterFrequency;
        if (transmit)
          this.networkManager.SendTransmitterFrequency(p, playerID, transmitterFrequency);
      }
      return orAddDataBlock;
    }

    public ushort GetNextFreeTransmitterFrequency()
    {
      return ++this.lastTransmitterFrequency;
    }

    public void UpdateWifiTransmitterFrequency(GlobalPoint3D p, GamerID playerID, ushort frequency)
    {
      WifiTransmitterBlock dataBlock = this.GetDataBlock(p) as WifiTransmitterBlock;
      if (dataBlock == null)
        return;
      dataBlock.Frequency = frequency;
      Player player = this.instance.GetPlayer(playerID);
      if (player == null)
        return;
      player.LastTransmitterFrequency = frequency;
    }

    public WifiReceiverBlock AddWifiReceiverBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      WifiReceiverBlock wifiReceiverBlock = (WifiReceiverBlock) null;
      Player player = this.instance.GetPlayer(playerID);
      if (player != null && player.LastTransmitterFrequency > (ushort) 0)
      {
        wifiReceiverBlock = this.GetOrAddDataBlock(p, Block.WifiReceiver, method, playerID, true) as WifiReceiverBlock;
        wifiReceiverBlock.Frequency1 = player.LastTransmitterFrequency;
      }
      return wifiReceiverBlock;
    }

    private void FlagChunkMeshDirty(GlobalPoint3D p, UpdateBlockMethod method)
    {
      MapChunk chunk = this.map.GetChunk(p);
      chunk.SetChunkFlag(ChunkFlags.MeshDirty);
      this.map.AddChunkToCommitList(chunk, method);
    }

    public bool IsBlockReceivingPower(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.blocksReceivingPower)
        return this.blocksReceivingPower.ContainsKey(globalHashCode);
    }

    public bool IsBlockDeliveringPower(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.blocksDeliveringPower)
        return this.blocksDeliveringPower.ContainsKey(globalHashCode);
    }

    public void TogglePowerReceipt(GlobalPoint3D p)
    {
      this.SetBlockPowerCore(p, !this.IsBlockReceivingPower(p), false);
    }

    private void ClearPowerReceipt(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.blocksReceivingPower)
        this.blocksReceivingPower.Remove(globalHashCode);
    }

    public bool ConfirmDeliverPower(
      GlobalPoint3D p,
      Block blockID,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      bool flag = false;
      if ((!transmit || this.networkManager.IsHost) && this.map.BlockData[(int) blockID].IsPowerEmitter)
      {
        long globalHashCode = this.map.GetGlobalHashCode(p);
        lock (this.blocksDeliveringPower)
        {
          if (power)
          {
            if (!this.blocksDeliveringPower.ContainsKey(globalHashCode))
              flag = true;
          }
          else if (this.blocksDeliveringPower.ContainsKey(globalHashCode))
            flag = true;
        }
      }
      return flag;
    }

    public bool DeliverPower(
      GlobalPoint3D p,
      Block blockID,
      BlockFace face,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      bool flag = false;
      if (this.map.BlockData[(int) blockID].IsPowerEmitter)
      {
        if (transmit)
          this.networkManager.SendPowerDeliver(p, blockID, face, power, method, playerID);
        flag = this.DeliverPowerLocal(p, blockID, face, power, true, method, playerID);
        if (flag)
        {
          switch (blockID)
          {
            case Block.PressurePlate:
            case Block.Switch:
              this.FlagChunkMeshDirty(p, method);
              break;
            case Block.Button:
              this.FlagChunkMeshDirty(p, method);
              if (power && this.networkManager.IsHost)
              {
                this.AddTimedBlock(p, 1.5f, 0, playerID);
                break;
              }
              break;
          }
          this.map.Commit();
        }
      }
      return flag;
    }

    private bool DeliverPowerLocal(
      GlobalPoint3D p,
      Block blockID,
      BlockFace face,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      bool flag = false;
      lock (this.blocksDeliveringPower)
      {
        if (power)
        {
          if (!this.blocksDeliveringPower.ContainsKey(globalHashCode))
          {
            this.blocksDeliveringPower.Add(globalHashCode, (byte) 0);
            flag = true;
          }
        }
        else if (this.blocksDeliveringPower.ContainsKey(globalHashCode))
        {
          this.blocksDeliveringPower.Remove(globalHashCode);
          flag = true;
        }
      }
      if (flag)
        this.EmitLocalPowerFlow(p, blockID, face, power, flow, method, playerID);
      return flag;
    }

    private void EmitLocalPowerFlow(
      GlobalPoint3D p,
      Block blockID,
      BlockFace face,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      switch (blockID)
      {
        case Block.PressurePlate:
          if (!this.IsCharacterOnPlateChanged(p, power, playerID, false))
            break;
          this.EmitLocalPowerFlowForPlate(p, power, flow, method, playerID);
          break;
        case Block.Switch:
        case Block.Button:
          this.EmitLocalPowerFlowForSwitches(p, face, power, flow, method, playerID);
          break;
        default:
          this.EmitLocalPowerFlowForAdjacent(p, power, flow, method, playerID);
          break;
      }
    }

    private void EmitLocalPowerFlowForAdjacent(
      GlobalPoint3D p,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      this.SignalPointCore(p + GlobalPoint3D.Left, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Forward, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Right, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Backward, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Down, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Up, power, flow, method, playerID);
    }

    private void EmitLocalPowerFlowForPlate(
      GlobalPoint3D p,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      this.SignalPointCore(p + GlobalPoint3D.Left, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Left + GlobalPoint3D.Down, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Forward, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Forward + GlobalPoint3D.Down, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Right, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Right + GlobalPoint3D.Down, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Backward, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Backward + GlobalPoint3D.Down, power, flow, method, playerID);
      this.SignalPointCore(p + GlobalPoint3D.Down, power, flow, method, playerID);
    }

    private void EmitLocalPowerFlowForSwitches(
      GlobalPoint3D p,
      BlockFace face,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      switch (face)
      {
        case BlockFace.Left:
        case BlockFace.Right:
          if (face == BlockFace.Left)
            ++p.X;
          else
            --p.X;
          this.SignalPointCore(p, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Up, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Down, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Forward, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Backward, power, flow, method, playerID);
          break;
        case BlockFace.Forward:
        case BlockFace.Backward:
          if (face == BlockFace.Forward)
            ++p.Z;
          else
            --p.Z;
          this.SignalPointCore(p, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Up, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Down, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Left, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Right, power, flow, method, playerID);
          break;
        case BlockFace.Up:
        case BlockFace.Down:
          if (face == BlockFace.Down)
            ++p.Y;
          else
            --p.Y;
          this.SignalPointCore(p, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Left, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Forward, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Right, power, flow, method, playerID);
          this.SignalPointCore(p + GlobalPoint3D.Backward, power, flow, method, playerID);
          break;
      }
    }

    public int GetPowerCount(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      byte num = 0;
      lock (this.blocksReceivingPower)
        return this.blocksReceivingPower.TryGetValue(globalHashCode, out num) ? (int) num : -1;
    }

    public void SignalPointExternal(SignalData data)
    {
      this.SignalPointCore(data.Point, data.Power, data.Flow, data.Method, data.PlayerID);
    }

    private void SignalPointCore(
      GlobalPoint3D p,
      bool power,
      bool flow,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (!this.SetBlockPowerCore(p, power, flow))
        return;
      if (method != UpdateBlockMethod.PowerRebuild)
      {
        MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
        Block blockId = (Block) blockIdAndAuxNoCache.BlockID;
        Block block = blockId;
        if ((uint) block <= 140U)
        {
          if ((uint) block <= 55U)
          {
            switch (block)
            {
              case Block.SteelDoorTop:
                break;
              case Block.C4:
                if (!power)
                  return;
                this.IgniteExplosive(p, Block.C4, method == UpdateBlockMethod.Player ? playerID : GamerID.Sys1);
                return;
              default:
                return;
            }
          }
          else
          {
            switch (block)
            {
              case Block.SteelDoorBottom:
                break;
              case Block.LockedDoorTop:
                goto label_16;
              default:
                return;
            }
          }
          Player player = this.instance.GetPlayer(playerID);
          byte aux = (byte) ((uint) this.instance.SwitchDoorState(blockIdAndAuxNoCache.AuxData) + ((uint) blockIdAndAuxNoCache.AuxData & 240U));
          this.instance.HitDoorCore(p, p + (blockId == Block.SteelDoorBottom ? GlobalPoint3D.Up : GlobalPoint3D.Down), aux, (Actor) player);
          this.FlagChunkMeshDirty(p, method);
          return;
        }
        if ((uint) block <= 166U)
        {
          switch (block)
          {
            case Block.WifiTransmitter:
              this.EmitPowerSignal(p, power, method, playerID);
              return;
            case Block.PoweredLight:
              int num = (int) blockIdAndAuxNoCache.AuxData & 254;
              if (power)
                ++num;
              this.map.SetAuxData(p, blockIdAndAuxNoCache.AuxData, (byte) num, method, playerID, false);
              return;
            default:
              return;
          }
        }
        else
        {
          switch (block)
          {
            case Block.TrapDoor:
              Player player1 = this.instance.GetPlayer(playerID);
              byte aux1 = (byte) ((uint) this.instance.SwitchDoorState(blockIdAndAuxNoCache.AuxData) + ((uint) blockIdAndAuxNoCache.AuxData & 240U));
              this.instance.HitDoorCore(p, aux1, (Actor) player1);
              this.FlagChunkMeshDirty(p, method);
              return;
            case Block.ScriptBlock:
              ScriptBlock dataBlock = this.GetDataBlock(p) as ScriptBlock;
              if (dataBlock == null)
                return;
              ScriptExecuteData data = new ScriptExecuteData()
              {
                Actor = (Actor) this.instance.GetPlayer(playerID),
                BlockOffset = new GlobalPoint3D?(p)
              };
              this.instance.ExecuteScript(power ? dataBlock.PowerOnScript : dataBlock.PowerOffScript, data, false);
              return;
            case Block.LockedDoorBottom:
              break;
            default:
              return;
          }
        }
label_16:
        Player player2 = this.instance.GetPlayer(playerID);
        if (!this.instance.CanOpenDoor(p, player2, (Hand) null))
          return;
        byte aux2 = (byte) ((uint) this.instance.SwitchDoorState(blockIdAndAuxNoCache.AuxData) + ((uint) blockIdAndAuxNoCache.AuxData & 240U));
        this.instance.HitDoorCore(p, p + (blockId == Block.LockedDoorBottom ? GlobalPoint3D.Up : GlobalPoint3D.Down), aux2, (Actor) player2);
        this.FlagChunkMeshDirty(p, method);
      }
      else
      {
        if (this.map.GetBlockIDAndAuxFromPending(p).BlockID != (byte) 163)
          return;
        this.EmitPowerSignal(p, power, method, playerID);
      }
    }

    private bool SetBlockPowerCore(GlobalPoint3D p, bool power, bool flow)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      byte num1 = 0;
      lock (this.blocksReceivingPower)
      {
        if (this.blocksReceivingPower.TryGetValue(globalHashCode, out num1))
        {
          byte num2;
          if (power)
          {
            if (flow)
              this.blocksReceivingPower[globalHashCode] = num2 = (byte) ((uint) num1 + 1U);
            return false;
          }
          if (flow)
          {
            if (num1 > (byte) 1)
            {
              this.blocksReceivingPower[globalHashCode] = num2 = (byte) ((uint) num1 - 1U);
              return false;
            }
            this.blocksReceivingPower.Remove(globalHashCode);
          }
          else if (num1 == (byte) 0)
            this.blocksReceivingPower.Remove(globalHashCode);
        }
        else
        {
          if (!power)
            return false;
          this.blocksReceivingPower.Add(globalHashCode, flow ? (byte) 1 : (byte) 0);
        }
      }
      return true;
    }

    public void UpdateSundials(int hour)
    {
      lock (this.dataBlocks)
      {
        foreach (KeyValuePair<long, DataBlock> dataBlock in this.dataBlocks)
        {
          SundialBlock sundialBlock = dataBlock.Value as SundialBlock;
          if (sundialBlock != null)
            this.EmitPowerSignalCore(sundialBlock.Point, (ushort) hour, sundialBlock.SignalType, UpdateBlockMethod.Strategy, GamerID.Sys1);
        }
      }
    }

    private void EmitPowerSignal(
      GlobalPoint3D p,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      WifiTransmitterBlock dataBlock = this.GetDataBlock(p) as WifiTransmitterBlock;
      if (dataBlock == null || dataBlock.Frequency <= (ushort) 0)
        return;
      this.EmitPowerSignalCore(p, dataBlock.Frequency, power, method, playerID);
    }

    private void EmitPowerSignalCore(
      GlobalPoint3D p,
      ushort frequency,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      int num = 4096;
      int next = this.receiverSearchPool.GetNext();
      this.receiverSearchPool.List[next].Clear();
      long globalHashCode = this.map.GetGlobalHashCode(p);
      try
      {
        lock (this.dataBlocks)
        {
          foreach (KeyValuePair<long, DataBlock> dataBlock in this.dataBlocks)
          {
            WifiReceiverBlock receiver = dataBlock.Value as WifiReceiverBlock;
            if (receiver != null)
            {
              bool flag = false;
              if (receiver.Transmitters != null)
              {
                int count = receiver.Transmitters.Count;
              }
              if (receiver.Transmitters != null && receiver.Transmitters.Contains(globalHashCode))
              {
                receiver.Transmitters.Remove(globalHashCode);
                flag = true;
              }
              if (power && ((int) receiver.Frequency1 == (int) frequency || (int) receiver.Frequency2 == (int) frequency) && (double) GlobalPoint3D.DistanceSquared(p, receiver.Point) <= (double) num)
              {
                if (receiver.Transmitters == null)
                  receiver.Transmitters = new List<long>();
                receiver.Transmitters.Add(globalHashCode);
                flag = true;
              }
              if (flag)
                this.UpdateReceiver(receiver, power, method, playerID);
            }
          }
        }
      }
      finally
      {
        this.receiverSearchPool.Release(next);
      }
    }

    private void UpdateReceiver(
      WifiReceiverBlock receiver,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      bool input1 = false;
      bool input2 = false;
      if (receiver.Transmitters != null && receiver.Transmitters.Count > 0)
      {
        for (int index = 0; index < receiver.Transmitters.Count && (!input1 || !input2); ++index)
        {
          GlobalPoint3D pointFromGlobalHash = this.map.GetPointFromGlobalHash(receiver.Transmitters[index]);
          WifiTransmitterBlock dataBlock = this.GetDataBlock(pointFromGlobalHash) as WifiTransmitterBlock;
          if (dataBlock != null)
          {
            if ((int) dataBlock.Frequency == (int) receiver.Frequency1)
            {
              if (this.IsBlockReceivingPower(pointFromGlobalHash))
                input1 = true;
            }
            else if ((int) dataBlock.Frequency == (int) receiver.Frequency2 && this.IsBlockReceivingPower(pointFromGlobalHash))
              input2 = true;
          }
        }
      }
      bool power1 = this.ApplyGateLogic(input1, input2, receiver.Gate, power);
      if (power1 == receiver.CurrentOutput)
        return;
      receiver.CurrentOutput = power1;
      this.EmitLocalPowerFlow(receiver.Point, Block.WifiReceiver, BlockFace.ProxyDefault, power1, true, method, playerID);
    }

    private bool ApplyGateLogic(bool input1, bool input2, BinaryOperatorType op, bool power)
    {
      switch (op)
      {
        case BinaryOperatorType.OR:
          return input1 | input2;
        case BinaryOperatorType.AND:
          return input1 & input2;
        case BinaryOperatorType.NOR:
          return !(input1 | input2);
        case BinaryOperatorType.NAND:
          return !(input1 & input2);
        case BinaryOperatorType.XOR:
          return input1 ^ input2;
        case BinaryOperatorType.XNOR:
          return !(input1 ^ input2);
        default:
          return power | input1 | input2;
      }
    }

    private void RebuildBlocksReceivingPower()
    {
      this.blocksReceivingPower.Clear();
      foreach (KeyValuePair<long, DataBlock> dataBlock in this.dataBlocks)
      {
        WifiReceiverBlock wifiReceiverBlock = dataBlock.Value as WifiReceiverBlock;
        if (wifiReceiverBlock != null)
          wifiReceiverBlock.CurrentOutput = false;
      }
      foreach (KeyValuePair<long, byte> keyValuePair in this.blocksDeliveringPower)
      {
        GlobalPoint3D pointFromGlobalHash = this.map.GetPointFromGlobalHash(keyValuePair.Key);
        MapBlock andAuxFromPending = this.map.GetBlockIDAndAuxFromPending(pointFromGlobalHash);
        switch ((Block) andAuxFromPending.BlockID)
        {
          case Block.ProximityDetector:
          case Block.WifiTransmitter:
            this.EmitLocalPowerFlow(pointFromGlobalHash, (Block) andAuxFromPending.BlockID, BlockFace.ProxyDefault, true, true, UpdateBlockMethod.PowerRebuild, GamerID.Sys1);
            continue;
          case Block.Switch:
            this.EmitLocalPowerFlow(pointFromGlobalHash, (Block) andAuxFromPending.BlockID, (BlockFace) ((uint) andAuxFromPending.AuxData & 7U), true, true, UpdateBlockMethod.PowerRebuild, GamerID.Sys1);
            continue;
          default:
            continue;
        }
      }
    }

    public void AddZone(Zone zone)
    {
      if (zone == null)
        return;
      lock (this.zones)
        this.zones.Add(zone);
      zone.Min.Clamp(this.map.MapBound.Min, this.map.MapBound.Max - GlobalPoint3D.One);
      zone.Max.Clamp(this.map.MapBound.Min, this.map.MapBound.Max - GlobalPoint3D.One);
      this.AddZoneToLists(zone);
    }

    private void AddZoneToLists(Zone zone)
    {
      Dictionary<long, MapChunk> dictionary = new Dictionary<long, MapChunk>(100);
      this.map.GetChunks(zone.Min, zone.Max, dictionary);
      if (dictionary.Count > 27)
        this.AddZoneToLargeZoneList(zone);
      else
        this.AddZoneToLocalZoneList(zone, dictionary);
    }

    private void AddZoneToLargeZoneList(Zone zone)
    {
      lock (this.largeZoneList)
        this.largeZoneList.Add(zone);
    }

    private void AddZoneToLocalZoneList(Zone zone, List<long> hashes)
    {
      foreach (long hash in hashes)
        this.AddZoneToLocalZoneList(zone, hash);
    }

    private void AddZoneToLocalZoneList(Zone zone, Dictionary<long, MapChunk> chunks)
    {
      foreach (KeyValuePair<long, MapChunk> chunk in chunks)
        this.AddZoneToLocalZoneList(zone, chunk.Key);
    }

    private void AddZoneToLocalZoneList(Zone zone, long hash)
    {
      lock (this.localZoneList)
      {
        List<Zone> zoneList;
        if (this.localZoneList.TryGetValue(hash, out zoneList))
        {
          lock (zoneList)
            zoneList.Add(zone);
        }
        else
        {
          zoneList = new List<Zone>(1);
          zoneList.Add(zone);
          this.localZoneList.Add(hash, zoneList);
        }
      }
    }

    public void RemoveZone(Zone zone)
    {
      if (zone == null)
        return;
      lock (this.zones)
        this.zones.Remove(zone);
      this.RemoveZoneFromLargeZoneList(zone);
      this.RemoveZoneFromLocalZoneList(zone);
    }

    private void RemoveZoneFromLargeZoneList(Zone zone)
    {
      lock (this.largeZoneList)
        this.largeZoneList.Remove(zone);
    }

    private void RemoveZoneFromLocalZoneList(Zone zone)
    {
      List<long> result = new List<long>(10);
      this.map.GetChunks(zone.Min, zone.Max, result);
      lock (this.localZoneList)
      {
        foreach (long key in result)
        {
          List<Zone> zoneList;
          if (this.localZoneList.TryGetValue(key, out zoneList))
          {
            lock (zoneList)
            {
              zoneList.Remove(zone);
              if (zoneList.Count == 0)
                this.localZoneList.Remove(key);
            }
          }
        }
      }
    }

    public void UpdateZoneBound(Zone zone)
    {
      this.RemoveZoneFromLargeZoneList(zone);
      this.RemoveZoneFromLocalZoneList(zone);
      this.AddZoneToLists(zone);
    }

    public void ReplaceZone(Zone zone1, Zone zone2)
    {
      lock (this.zones)
      {
        int index = this.zones.IndexOf(zone1);
        if (index < 0)
          return;
        this.zones[index] = zone2;
      }
    }

    public Zone GetZone(string name)
    {
      return this.GetZone(name, GamerID.Sys1);
    }

    public Zone GetZone(string name, GamerID gamerID)
    {
      if (name != null && name.Length > 0)
      {
        lock (this.zones)
        {
          foreach (Zone zone in this.zones)
          {
            if (gamerID == zone.GamerID && zone.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
              return zone;
          }
        }
      }
      return (Zone) null;
    }

    private List<Zone> GetZones(BoundingBox box)
    {
      lock (this.tempZoneList)
      {
        this.tempZoneList.Clear();
        this.GetZones(box, this.tempZoneList);
      }
      return this.tempZoneList;
    }

    public void GetZones(BoundingBox box, List<Zone> result)
    {
      this.LoadZonesFrom(box, this.largeZoneList, result);
      lock (this.tempChunkList)
      {
        this.map.GetChunks(box, this.tempChunkList);
        foreach (KeyValuePair<long, MapChunk> tempChunk in this.tempChunkList)
        {
          lock (this.localZoneList)
          {
            List<Zone> source;
            if (this.localZoneList.TryGetValue(tempChunk.Key, out source))
              this.LoadZonesFrom(box, source, result);
          }
        }
        this.tempChunkList.Clear();
      }
    }

    private void LoadZonesFrom(BoundingBox box, List<Zone> source, List<Zone> dest)
    {
      lock (source)
      {
        foreach (Zone zone in source)
        {
          if (zone.IsInZone((Map) this.map, box))
            dest.Add(zone);
        }
      }
    }

    public List<Zone> GetZones(ZoneType type)
    {
      lock (this.tempZoneList)
      {
        this.tempZoneList.Clear();
        lock (this.zones)
        {
          foreach (Zone zone in this.zones)
          {
            if (zone.HasZoneType(type))
              this.tempZoneList.Add(zone);
          }
        }
      }
      return this.tempZoneList;
    }

    public bool IsInZone(BoundingBox box, string zoneName)
    {
      lock (this.tempZoneList)
      {
        this.tempZoneList.Clear();
        this.GetZones(box, this.tempZoneList);
        foreach (Zone tempZone in this.tempZoneList)
        {
          if (string.Equals(zoneName, tempZone.Name, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public bool IsInZoneType(GlobalPoint3D p, ZoneType type, GamerID gamerID)
    {
      float tileSize = this.map.TileSize;
      BoundingBox box = new BoundingBox()
      {
        Min = this.map.GetPosition(p)
      };
      box.Max.Y = box.Min.Y;
      box.Min.Y -= tileSize;
      box.Max.X = (float) ((double) box.Min.X + (double) tileSize - 0.00999999977648258);
      box.Max.Y -= 0.01f;
      box.Max.Z = (float) ((double) box.Min.Z + (double) tileSize - 0.00999999977648258);
      return this.IsInZoneType(box, type, gamerID);
    }

    public bool IsInZoneType(GlobalPoint3D min, GlobalPoint3D max, ZoneType type, GamerID gamerID)
    {
      float tileSize = this.map.TileSize;
      BoundingBox box = new BoundingBox();
      box.Min = this.map.GetPosition(min);
      box.Min.Y -= tileSize;
      box.Max = this.map.GetPosition(max);
      box.Max.X += tileSize - 0.01f;
      box.Max.Y -= 0.01f;
      box.Max.Z += tileSize - 0.01f;
      return this.IsInZoneType(box, type, gamerID);
    }

    public bool IsInZoneType(BoundingBox box, ZoneType type, GamerID gamerID)
    {
      if (type == ZoneType.None)
        return true;
      Player player = this.instance.GetPlayer(gamerID);
      bool admin = Globals2.GetGamerType(gamerID) == GamerType.Script || player != null && player.IsAdmin;
      if (admin && (type & ZoneType.NoCombat) == ZoneType.NoCombat && !Globals2.GameProperties.SaveGame.Header.CombatEnabled)
        return true;
      bool noedit = (type & ZoneType.NoEdit) == ZoneType.NoEdit;
      lock (this.tempZoneList)
      {
        this.tempZoneList.Clear();
        this.GetZones(box, this.tempZoneList);
        foreach (Zone tempZone1 in this.tempZoneList)
        {
          if (this.IsInZoneType(tempZone1, type, player, admin, noedit))
          {
            bool flag = true;
            foreach (Zone tempZone2 in this.tempZoneList)
            {
              if (tempZone2 != tempZone1 && !this.IsInZoneType(tempZone2, type, player, admin, noedit) && tempZone1.Contains(tempZone2))
              {
                flag = false;
                break;
              }
            }
            if (flag)
              return true;
          }
        }
      }
      return false;
    }

    private bool IsInZoneType(Zone zone, ZoneType type, Player player, bool admin, bool noedit)
    {
      return (zone.ZoneType & type) == type && (!noedit || (zone.ZoneType & ZoneType.Spawn) == ZoneType.Spawn || !admin && (zone.Builder == null || player == null || (zone.BuilderType != ZoneBuilderType.Player || !(player.Gamertag == zone.Builder)) && (zone.BuilderType != ZoneBuilderType.Clan || !(player.ClanName == zone.Builder))));
    }

    public void GetZoneModifiers(
      BoundingBox box,
      out short combatLevelDifference,
      out float speedModifier,
      out float gravityModifier)
    {
      combatLevelDifference = (short) 0;
      speedModifier = 1f;
      gravityModifier = 1f;
      lock (this.tempZoneList)
      {
        this.tempZoneList.Clear();
        this.GetZones(box, this.tempZoneList);
        this.tempZoneList.Sort(new Comparison<Zone>(this.SortZonesBySize));
        short num = 0;
        bool flag = false;
        for (int index1 = 0; index1 < this.tempZoneList.Count && !flag; ++index1)
        {
          Zone tempZone = this.tempZoneList[index1];
          speedModifier *= tempZone.SpeedMultiplier;
          gravityModifier *= tempZone.GravityMultiplier;
          if (!flag)
          {
            for (int index2 = index1 + 1; index2 < this.tempZoneList.Count; ++index2)
            {
              if (this.tempZoneList[index2].Contains(tempZone))
              {
                flag = true;
                combatLevelDifference = tempZone.CombatLevelDifference;
                break;
              }
            }
            if ((int) tempZone.CombatLevelDifference > (int) num)
              num = tempZone.CombatLevelDifference;
          }
        }
        if (flag)
          return;
        combatLevelDifference = num;
      }
    }

    private int SortZonesBySize(Zone z1, Zone z2)
    {
      if (z1.Contains(z2))
        return 1;
      return z2.Contains(z1) ? -1 : 0;
    }

    private void UpdateCrops()
    {
      if (!this.instance.IsFiniteResources)
        return;
      this.cropUpdateTimer += Services.ElapsedTime;
      int num1 = 5;
      if ((double) this.cropUpdateTimer < (double) num1)
        return;
      int num2;
      switch (this.instance.SunMoon.Season)
      {
        case SeasonType.Summer:
          num2 = 700;
          break;
        case SeasonType.Autumn:
          num2 = 800;
          break;
        case SeasonType.Winter:
          num2 = 900;
          break;
        default:
          num2 = 600;
          break;
      }
      this.cropUpdateTimer = 0.0f;
      bool flag = false;
      lock (this.cropBlocks)
      {
        for (int index = this.cropBlocks.Count - 1; index >= 0; --index)
        {
          CropBlock cropBlock = this.cropBlocks[index];
          if ((int) ++cropBlock.Timer >= num2 / num1 && this.GrowCrop(cropBlock, false))
            flag = true;
        }
      }
      if (!flag)
        return;
      this.map.Commit();
    }

    private bool GrowCrop(CropBlock cropBlock, bool overrideRequiredGrowthConditions)
    {
      cropBlock.Timer = (ushort) 0;
      if (!overrideRequiredGrowthConditions && !this.CanCropGrow(cropBlock))
        return false;
      byte auxFullData = this.map.GetAuxFullData(cropBlock.Point);
      int num = ((int) auxFullData & 7) + 1;
      if (num > 5)
        num = 5;
      byte auxData = (byte) (((int) auxFullData & 248) + num);
      this.map.SetAuxData(cropBlock.Point, auxData, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
      if (num >= 5)
      {
        this.cropBlocks.Remove(cropBlock);
        this.RemoveDataBlock((DataBlock) cropBlock);
      }
      return true;
    }

    public void GrowCropBlockOneStage(GlobalPoint3D p, bool overrideRequiredGrowthConditions)
    {
      CropBlock dataBlock = this.GetDataBlock(p) as CropBlock;
      if (dataBlock == null)
        return;
      lock (this.cropBlocks)
      {
        if (!this.GrowCrop(dataBlock, overrideRequiredGrowthConditions))
          return;
        this.map.Commit();
      }
    }

    private bool CanCropGrow(CropBlock cropBlock)
    {
      GlobalPoint3D point = cropBlock.Point;
      ++point.Y;
      if (point.Y < this.map.MapBound.Max.Y && this.map.GetBlockIDNoCache(point) == (byte) 0)
      {
        --point.Y;
        MapLight lightNoCache = this.map.GetLightNoCache(point);
        if (lightNoCache.SunLight >= (byte) 8 || lightNoCache.BlockLight >= (byte) 9)
          return this.map.IsNextTo(point - new GlobalPoint3D(4, 2, 4), point + new GlobalPoint3D(4, 3, 4), (byte) 11);
      }
      return false;
    }

    private void AddCropBlock(GlobalPoint3D p, UpdateBlockMethod method)
    {
      if (!this.map.IsHost || this.GetDataBlock(p) is CropBlock)
        return;
      this.AddDataBlock((DataBlock) new CropBlock(p), method);
    }

    public override void AddLiquidFlow(GlobalPoint3D p, byte blockID, UpdateBlockMethod method)
    {
      this.AddLiquidFlow(p, blockID, this.map.GetAuxDataNoCache(p), method);
    }

    private void AddLiquidFlow(
      GlobalPoint3D p,
      byte blockID,
      byte level,
      UpdateBlockMethod method)
    {
      if (blockID == (byte) 11 && level > (byte) 7 || blockID == (byte) 13 && level > (byte) 5)
        return;
      if (method != UpdateBlockMethod.Generation)
        method = UpdateBlockMethod.Strategy;
      lock (this.liquidAdditionsUpdate)
        this.liquidAdditionsUpdate.Add(new MapStrategyTM.LiquidAddition()
        {
          Point = p,
          BlockID = blockID,
          Level = level,
          Method = method
        });
    }

    private void RemoveLiquidFlow(
      GlobalPoint3D p,
      byte blockID,
      byte level,
      UpdateBlockMethod method)
    {
      if (method != UpdateBlockMethod.Generation)
        method = UpdateBlockMethod.Strategy;
      lock (this.liquidRemovalUpdate)
      {
        MapStrategyTM.LiquidRemoval liquidRemoval = new MapStrategyTM.LiquidRemoval();
        long globalHashCode = this.map.GetGlobalHashCode(p);
        if (this.liquidRemovalUpdate.TryGetValue(globalHashCode, out liquidRemoval))
        {
          if ((int) liquidRemoval.BlockID != (int) blockID || (int) liquidRemoval.StartLevel >= (int) level)
            return;
          liquidRemoval.StartLevel = level;
          liquidRemoval.Method = method;
          this.liquidRemovalUpdate[globalHashCode] = liquidRemoval;
        }
        else
        {
          liquidRemoval.BlockID = blockID;
          liquidRemoval.StartLevel = level;
          liquidRemoval.Method = method;
          this.liquidRemovalUpdate.Add(globalHashCode, liquidRemoval);
        }
      }
    }

    public void UpdateLiquidAdditions()
    {
      bool flag = false;
      lock (this.liquidAdditionsUpdate)
      {
        if (this.liquidAdditionsUpdate.Count > 0)
        {
          List<MapStrategyTM.LiquidAddition> additionsUpdate2 = this.liquidAdditionsUpdate2;
          this.liquidAdditionsUpdate2 = this.liquidAdditionsUpdate;
          this.liquidAdditionsUpdate = additionsUpdate2;
          for (int index = this.liquidAdditionsUpdate2.Count - 1; index >= 0; --index)
            flag |= this.AddLiquid(this.liquidAdditionsUpdate2[index]);
          this.liquidAdditionsUpdate2.Clear();
        }
      }
      if (!flag)
        return;
      this.map.Commit();
    }

    private bool AddLiquid(MapStrategyTM.LiquidAddition liquid)
    {
      GlobalPoint3D point = liquid.Point;
      byte blockId1 = liquid.BlockID;
      int y = point.Y;
      MapBlock blockIdAndAuxNoCache1 = this.map.GetBlockIDAndAuxNoCache(point);
      byte blockId2 = blockIdAndAuxNoCache1.BlockID;
      if ((int) blockId2 != (int) blockId1)
        return false;
      bool flag = false;
      --point.Y;
      MapBlock blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(point);
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) blockId2 && ((int) blockIdAndAuxNoCache2.AuxData & 15) > 0)
      {
        ++point.Y;
        if (this.CountAdjacentRetainerBlocks(point, blockId1) == 0)
          return false;
        --point.Y;
      }
      if (blockId1 == (byte) 11 && blockIdAndAuxNoCache2.BlockID == (byte) 13)
      {
        this.map.SetBlockData(point, (byte) 42, (byte) 0, liquid.Method, GamerID.Sys1, false);
        return true;
      }
      if (blockId1 == (byte) 13 && blockIdAndAuxNoCache2.BlockID == (byte) 11)
      {
        this.map.SetBlockData(point, (byte) 18, (byte) 0, liquid.Method, GamerID.Sys1, false);
        return true;
      }
      if (this.CanLiquidReplaceBlock(blockId1, blockIdAndAuxNoCache2.BlockID))
      {
        this.AddLiquid(point, blockId2, (byte) 1, liquid.Method);
        return true;
      }
      if ((int) liquid.Level < (blockId1 == (byte) 11 ? 7 : 5))
      {
        int val1 = 4;
        int val2_1 = 4;
        int val2_2 = 4;
        int val2_3 = 4;
        point.Y = y;
        int x = point.X;
        int z = point.Z;
        for (--point.X; point.X > x - 4 && this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)); --point.X)
        {
          --point.Y;
          if (this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)))
          {
            val1 = x - point.X;
            break;
          }
          ++point.Y;
        }
        point.Y = y;
        for (point.X = x + 1; point.X < x + 4 && this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)); ++point.X)
        {
          --point.Y;
          if (this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)))
          {
            val2_1 = point.X - x;
            break;
          }
          ++point.Y;
        }
        point.Y = y;
        point.X = x;
        for (--point.Z; point.Z > z - 4 && this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)); --point.Z)
        {
          --point.Y;
          if (this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)))
          {
            val2_2 = z - point.Z;
            break;
          }
          ++point.Y;
        }
        point.Y = y;
        for (point.Z = z + 1; point.Z < z + 4 && this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)); ++point.Z)
        {
          --point.Y;
          if (this.CanLiquidReplaceBlock(blockId1, this.map.GetBlockIDNoCache(point)))
          {
            val2_3 = point.Z - z;
            break;
          }
          ++point.Y;
        }
        point.X = x;
        point.Y = y;
        point.Z = z;
        int num = Math.Min(Math.Min(Math.Min(val1, val2_1), val2_2), val2_3);
        if (val1 == num)
        {
          --point.X;
          MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(point);
          byte level = (byte) ((uint) liquid.Level + 1U);
          if ((int) blockIdAndAuxNoCache3.BlockID == (int) blockId2 && ((int) blockIdAndAuxNoCache3.AuxData & 7) > (int) level || this.CanLiquidReplaceBlock(blockId1, blockIdAndAuxNoCache3.BlockID))
          {
            --point.Y;
            MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(point);
            ++point.Y;
            if ((int) blockIdAndAuxNoCache4.BlockID == (int) blockId1 && ((int) blockIdAndAuxNoCache1.AuxData & 7) == 0)
              level = byte.MaxValue;
            this.AddLiquid(point, blockId2, level, liquid.Method);
            flag = true;
          }
          ++point.X;
        }
        if (val2_1 == num)
        {
          ++point.X;
          MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(point);
          byte level = (byte) ((uint) liquid.Level + 1U);
          if ((int) blockIdAndAuxNoCache3.BlockID == (int) blockId2 && ((int) blockIdAndAuxNoCache3.AuxData & 7) > (int) level || this.CanLiquidReplaceBlock(blockId1, blockIdAndAuxNoCache3.BlockID))
          {
            --point.Y;
            MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(point);
            ++point.Y;
            if ((int) blockIdAndAuxNoCache4.BlockID == (int) blockId1 && ((int) blockIdAndAuxNoCache1.AuxData & 7) == 0)
              level = byte.MaxValue;
            this.AddLiquid(point, blockId2, level, liquid.Method);
            flag = true;
          }
          --point.X;
        }
        if (val2_2 == num)
        {
          --point.Z;
          MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(point);
          byte level = (byte) ((uint) liquid.Level + 1U);
          if ((int) blockIdAndAuxNoCache3.BlockID == (int) blockId2 && ((int) blockIdAndAuxNoCache3.AuxData & 7) > (int) level || this.CanLiquidReplaceBlock(blockId1, blockIdAndAuxNoCache3.BlockID))
          {
            --point.Y;
            MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(point);
            ++point.Y;
            if ((int) blockIdAndAuxNoCache4.BlockID == (int) blockId1 && ((int) blockIdAndAuxNoCache1.AuxData & 7) == 0)
              level = byte.MaxValue;
            this.AddLiquid(point, blockId2, level, liquid.Method);
            flag = true;
          }
          ++point.Z;
        }
        if (val2_3 == num)
        {
          ++point.Z;
          MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(point);
          byte level = (byte) ((uint) liquid.Level + 1U);
          if ((int) blockIdAndAuxNoCache3.BlockID == (int) blockId2 && ((int) blockIdAndAuxNoCache3.AuxData & 7) > (int) level || this.CanLiquidReplaceBlock(blockId1, blockIdAndAuxNoCache3.BlockID))
          {
            --point.Y;
            MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(point);
            ++point.Y;
            if ((int) blockIdAndAuxNoCache4.BlockID == (int) blockId1 && ((int) blockIdAndAuxNoCache1.AuxData & 7) == 0)
              level = byte.MaxValue;
            this.AddLiquid(point, blockId2, level, liquid.Method);
            flag = true;
          }
          --point.Z;
        }
      }
      return flag;
    }

    private void AddLiquid(GlobalPoint3D p, byte blockID, byte level, UpdateBlockMethod method)
    {
      level = this.ChangeToLiquidSourceBlock(p, blockID, level);
      Block blockId = (Block) this.map.GetBlockID(p);
      if (blockID == (byte) 11 && blockId == Block.Lava || blockID == (byte) 13 && blockId == Block.Water)
      {
        this.map.SetBlockData(p, (byte) 42, (byte) 0, method, GamerID.Sys1, false);
      }
      else
      {
        this.map.SetBlockData(p, blockID, level == byte.MaxValue ? (byte) 1 : level, method, GamerID.Sys1, false);
        int num = blockID == (byte) 11 ? 7 : 5;
        switch (level)
        {
          case 1:
            if (p.Y > this.map.MapBound.Min.Y + 1)
            {
              --p.Y;
              MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
              if ((int) blockIdAndAuxNoCache.BlockID == (int) blockID && ((int) blockIdAndAuxNoCache.AuxData & 247) == 0)
                return;
              ++p.Y;
              break;
            }
            break;
          case byte.MaxValue:
            return;
        }
        if ((level <= (byte) 0 || (int) level >= num) && ((int) level != num || !this.CanLiquidReplaceBlock(blockID, this.map.GetBlockIDNoCache(p + GlobalPoint3D.Down))))
          return;
        lock (this.liquidAdditionsUpdate)
          this.liquidAdditionsUpdate.Add(new MapStrategyTM.LiquidAddition()
          {
            Point = p,
            BlockID = blockID,
            Level = level,
            Method = method
          });
      }
    }

    public void UpdateLiquidRemovals()
    {
      if (this.liquidRemovalUpdate.Count <= 0)
        return;
      lock (this.liquidRemovalUpdate)
      {
        Dictionary<long, MapStrategyTM.LiquidRemoval> liquidRemovalUpdate2 = this.liquidRemovalUpdate2;
        this.liquidRemovalUpdate2 = this.liquidRemovalUpdate;
        this.liquidRemovalUpdate = liquidRemovalUpdate2;
      }
      lock (this.liquidRemovalUpdate2)
      {
        if (this.liquidRemovalUpdate2.Count <= 0)
          return;
        bool flag = false;
        foreach (KeyValuePair<long, MapStrategyTM.LiquidRemoval> keyValuePair in this.liquidRemovalUpdate2)
        {
          GlobalPoint3D pointFromGlobalHash = this.map.GetPointFromGlobalHash(keyValuePair.Key);
          flag |= this.RemoveLiquid(keyValuePair.Key, pointFromGlobalHash, keyValuePair.Value);
        }
        this.liquidRemovalUpdate2.Clear();
        if (!flag)
          return;
        this.map.Commit();
      }
    }

    private bool RemoveLiquid(long hash, GlobalPoint3D p, MapStrategyTM.LiquidRemoval liquid)
    {
      bool flag1 = false;
      byte blockId = liquid.BlockID;
      MapBlock blockIdAndAuxNoCache1 = this.map.GetBlockIDAndAuxNoCache(p);
      byte num = (byte) ((uint) blockIdAndAuxNoCache1.AuxData & 7U);
      if ((int) blockIdAndAuxNoCache1.BlockID == (int) blockId)
      {
        byte highestAdjacentLevel = this.GetHighestAdjacentLevel(p, blockId);
        if (highestAdjacentLevel <= (byte) 0 || (int) num > (int) highestAdjacentLevel)
          return flag1;
        byte auxData = (byte) ((uint) num + 2U);
        if ((int) auxData > (int) highestAdjacentLevel + 1)
          auxData = (byte) ((uint) highestAdjacentLevel + 1U);
        if (auxData > (byte) 7)
          auxData = (byte) 0;
        byte blockID = auxData == (byte) 0 ? (byte) 0 : blockId;
        ++p.Y;
        bool flag2 = (int) this.map.GetBlockIDNoCache(p) == (int) blockID;
        --p.Y;
        if (flag2)
          auxData = (byte) 0;
        this.map.SetBlockData(p, blockID, auxData, liquid.Method, GamerID.Sys1, false);
        flag1 = true;
        if (!flag2)
          this.AddLiquidRemoval(hash, p, blockId, liquid.StartLevel, liquid.Method);
      }
      else
      {
        --p.Y;
        MapBlock blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
        if ((int) blockIdAndAuxNoCache2.BlockID == (int) blockId)
          this.AddLiquidRemoval(p, blockId, (byte) ((uint) blockIdAndAuxNoCache2.AuxData & 7U), liquid.Method);
        ++p.Y;
      }
      --p.X;
      this.SpreadLiquidRemove(p, blockId, liquid.StartLevel, liquid.Method);
      ++p.X;
      ++p.X;
      this.SpreadLiquidRemove(p, blockId, liquid.StartLevel, liquid.Method);
      --p.X;
      --p.Z;
      this.SpreadLiquidRemove(p, blockId, liquid.StartLevel, liquid.Method);
      ++p.Z;
      ++p.Z;
      this.SpreadLiquidRemove(p, blockId, liquid.StartLevel, liquid.Method);
      --p.Z;
      return flag1;
    }

    private void AddLiquidRemoval(
      GlobalPoint3D p,
      byte liquidID,
      byte level,
      UpdateBlockMethod method)
    {
      this.AddLiquidRemoval(this.map.GetGlobalHashCode(p), p, liquidID, level, method);
    }

    private void AddLiquidRemoval(
      long hash,
      GlobalPoint3D p,
      byte liquidID,
      byte level,
      UpdateBlockMethod method)
    {
      lock (this.liquidRemovalUpdate)
      {
        if (this.liquidRemovalUpdate.ContainsKey(hash))
          return;
        MapStrategyTM.LiquidRemoval liquidRemoval = new MapStrategyTM.LiquidRemoval()
        {
          BlockID = liquidID,
          StartLevel = level,
          Method = method
        };
        this.liquidRemovalUpdate.Add(hash, liquidRemoval);
      }
    }

    private bool SpreadLiquidRemove(
      GlobalPoint3D p,
      byte liquidID,
      byte sourceLevel,
      UpdateBlockMethod method)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.liquidRemovalUpdate)
      {
        if (!this.liquidRemovalUpdate.ContainsKey(globalHashCode))
        {
          MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
          if ((int) blockIdAndAuxNoCache.BlockID == (int) liquidID)
          {
            byte num = (byte) ((uint) blockIdAndAuxNoCache.AuxData & 7U);
            if ((int) num > (int) sourceLevel)
            {
              MapStrategyTM.LiquidRemoval liquidRemoval = new MapStrategyTM.LiquidRemoval()
              {
                BlockID = liquidID,
                StartLevel = num,
                Method = method
              };
              this.liquidRemovalUpdate.Add(globalHashCode, liquidRemoval);
              return true;
            }
          }
        }
      }
      return false;
    }

    private bool CanLiquidReplaceBlock(byte liquidID, byte blockID)
    {
      if (blockID == (byte) 0)
        return true;
      Block block = (Block) blockID;
      if ((uint) block <= 46U)
      {
        switch (block)
        {
          case Block.Water:
            return liquidID == (byte) 13;
          case Block.Lava:
            return liquidID == (byte) 11;
          case Block.Torch:
            break;
          default:
            goto label_9;
        }
      }
      else
      {
        switch (block)
        {
          case Block.Wisdom:
          case Block.Blueprint:
            return false;
          case Block.Fire:
          case Block.SnowLayer:
            break;
          default:
            goto label_9;
        }
      }
      return true;
label_9:
      return this.map.IsBlockIcon(blockID);
    }

    private byte ChangeToLiquidSourceBlock(GlobalPoint3D p, byte blockID, byte level)
    {
      return level;
    }

    private int CountAdjacentSourceBlocks(GlobalPoint3D p, byte blockID)
    {
      int num = 0;
      --p.X;
      MapBlock blockIdAndAuxNoCache1 = this.map.GetBlockIDAndAuxNoCache(p);
      ++p.X;
      if ((int) blockIdAndAuxNoCache1.BlockID == (int) blockID && ((int) blockIdAndAuxNoCache1.AuxData & 7) == 0)
        ++num;
      ++p.X;
      MapBlock blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
      --p.X;
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) blockID && ((int) blockIdAndAuxNoCache2.AuxData & 7) == 0)
        ++num;
      if (num < 2)
      {
        --p.Z;
        MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(p);
        ++p.Z;
        if ((int) blockIdAndAuxNoCache3.BlockID == (int) blockID && ((int) blockIdAndAuxNoCache3.AuxData & 7) == 0)
          ++num;
        if (num < 2)
        {
          ++p.Z;
          MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(p);
          --p.Z;
          if ((int) blockIdAndAuxNoCache4.BlockID == (int) blockID && ((int) blockIdAndAuxNoCache4.AuxData & 7) == 0)
            ++num;
        }
      }
      return num;
    }

    private int CountAdjacentRetainerBlocks(GlobalPoint3D p, byte liquidID)
    {
      int num = 0;
      --p.X;
      byte blockIdNoCache1 = this.map.GetBlockIDNoCache(p);
      if ((int) blockIdNoCache1 != (int) liquidID && blockIdNoCache1 != (byte) 0)
        ++num;
      ++p.X;
      ++p.X;
      byte blockIdNoCache2 = this.map.GetBlockIDNoCache(p);
      if ((int) blockIdNoCache2 != (int) liquidID && blockIdNoCache2 != (byte) 0)
        ++num;
      --p.X;
      --p.Z;
      byte blockIdNoCache3 = this.map.GetBlockIDNoCache(p);
      if ((int) blockIdNoCache3 != (int) liquidID && blockIdNoCache3 != (byte) 0)
        ++num;
      ++p.Z;
      ++p.Z;
      byte blockIdNoCache4 = this.map.GetBlockIDNoCache(p);
      if ((int) blockIdNoCache4 != (int) liquidID && blockIdNoCache4 != (byte) 0)
        ++num;
      --p.Z;
      return num;
    }

    private byte GetHighestAdjacentLevel(GlobalPoint3D p, byte liquidID)
    {
      byte num = byte.MaxValue;
      --p.X;
      MapBlock blockIdAndAuxNoCache1 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache1.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache1.AuxData & 7) < (int) num)
        num = (byte) ((uint) blockIdAndAuxNoCache1.AuxData & 7U);
      ++p.X;
      ++p.X;
      MapBlock blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache2.AuxData & 7) < (int) num)
        num = (byte) ((uint) blockIdAndAuxNoCache2.AuxData & 7U);
      --p.X;
      --p.Z;
      blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache2.AuxData & 7) < (int) num)
        num = (byte) ((uint) blockIdAndAuxNoCache2.AuxData & 7U);
      ++p.Z;
      ++p.Z;
      blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache2.AuxData & 7) < (int) num)
        num = (byte) ((uint) blockIdAndAuxNoCache2.AuxData & 7U);
      --p.Z;
      return num;
    }

    private bool HasHigherAdjacentLiquid(GlobalPoint3D p, byte liquidID, byte level)
    {
      --p.X;
      MapBlock blockIdAndAuxNoCache1 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache1.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache1.AuxData & 7) < (int) level)
        return true;
      ++p.X;
      ++p.X;
      MapBlock blockIdAndAuxNoCache2 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache2.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache2.AuxData & 7) < (int) level)
        return true;
      --p.X;
      --p.Z;
      MapBlock blockIdAndAuxNoCache3 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache3.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache3.AuxData & 7) < (int) level)
        return true;
      ++p.Z;
      ++p.Z;
      MapBlock blockIdAndAuxNoCache4 = this.map.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache4.BlockID == (int) liquidID && ((int) blockIdAndAuxNoCache4.AuxData & 7) < (int) level)
        return true;
      --p.Z;
      return false;
    }

    public void GamerLeft(NetworkGamer gamer)
    {
      lock (this.activeFurnaces)
      {
        foreach (FurnaceBlock activeFurnace in this.activeFurnaces)
        {
          if (activeFurnace.Gamertag == gamer.Gamertag)
            activeFurnace.Gamertag = (string) null;
        }
      }
    }

    private GlobalPoint3D GetNeighbour(GlobalPoint3D p, Block blockID)
    {
      byte num = (byte) blockID;
      if ((int) this.map.GetBlockID(p + GlobalPoint3D.Left) == (int) num)
        return p + GlobalPoint3D.Left;
      if ((int) this.map.GetBlockID(p + GlobalPoint3D.Right) == (int) num)
        return p + GlobalPoint3D.Right;
      if ((int) this.map.GetBlockID(p + GlobalPoint3D.Forward) == (int) num)
        return p + GlobalPoint3D.Forward;
      if ((int) this.map.GetBlockID(p + GlobalPoint3D.Backward) == (int) num)
        return p + GlobalPoint3D.Backward;
      if ((int) this.map.GetBlockID(p + GlobalPoint3D.Down) == (int) num)
        return p + GlobalPoint3D.Down;
      return new GlobalPoint3D(0, 1, 0);
    }

    private void DropItems(
      GlobalPoint3D p,
      Inventory inventory,
      UpdateBlockMethod method,
      GamerID playerID,
      bool resetDurability)
    {
      if (!this.map.IsHost)
        return;
      for (int index = 0; index < inventory.Count; ++index)
      {
        InventoryItem inventoryItem = inventory[index];
        if (resetDurability)
          inventoryItem.Durability = ItemData.GetItemDurability(inventoryItem.ItemID);
        this.instance.DropItem(ParticleType.None, p, inventoryItem, method, playerID);
      }
    }

    private struct TimedBlock : IEquatable<MapStrategyTM.TimedBlock>
    {
      public GlobalPoint3D Point;
      public Block BlockID;
      public float Timer;
      public GamerID PlayerID;
      public int Data;

      public bool Equals(MapStrategyTM.TimedBlock block)
      {
        if (block.Point == this.Point && block.BlockID == this.BlockID && block.PlayerID == this.PlayerID)
          return block.Data == this.Data;
        return false;
      }
    }

    private struct LiquidAddition
    {
      public GlobalPoint3D Point;
      public byte BlockID;
      public byte Level;
      public UpdateBlockMethod Method;
    }

    private struct LiquidRemoval
    {
      public byte BlockID;
      public byte StartLevel;
      public UpdateBlockMethod Method;
    }

    private enum RopeActivation
    {
      Lower,
      Cut,
    }
  }
}
