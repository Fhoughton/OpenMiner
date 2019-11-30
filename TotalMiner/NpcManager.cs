// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.NpcManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StudioForge.TotalMiner
{
  internal class NpcManager : GameObjectBase, ITMNpcManager
  {
    public static Stopwatch SpawnTimer = new Stopwatch();
    public short NextNpcID = (short) byte.MaxValue;
    private List<NpcBase> npcsToUpdate = new List<NpcBase>();
    private Pool<List<Actor>> actorListPool = new Pool<List<Actor>>(20);
    private Point ZombieRange = new Point(20, 40);
    private Point GoblinRange = new Point(20, 40);
    private Point TrollRange = new Point(25, 45);
    private Point DryadRange = new Point(25, 45);
    private Point DjinnRange = new Point(25, 45);
    private Point WerewolfRange = new Point(20, 40);
    private Point OrcRange = new Point(25, 45);
    private Point DiabloRange = new Point(30, 50);
    private Point DuckRange = new Point(30, 50);
    private Point SheepRange = new Point(30, 50);
    private Point AlpacaRange = new Point(30, 50);
    private Point AyrshireCowRange = new Point(30, 50);
    private Point HighlandCowRange = new Point(30, 50);
    private Point CustomPassiveRange = new Point(30, 50);
    private Point CustomEnemyRange = new Point(25, 45);
    private List<NpcBase> cubeNpcsToDraw = new List<NpcBase>();
    private List<NpcBase> inactiveMobs = new List<NpcBase>();
    public const int MaxNpcCount = 500;
    public const int MinNpcCount = 50;
    private const int npcFindSpaceRange = 8;
    public IndexBuffer IndexBuffer;
    public long TotalBufferSize;
    public int CurrentMaxNpcCount;
    public VertexBufferBinding[] CubaAvatarBindings;
    public VertexBuffer CubeAvatarModel;
    public DynamicVertexBuffer CubeAvatarInstanceData;
    public int CubeAvatarModelInstanceCount;
    private MapTM map;
    private GameInstance instance;
    private GraphicsDevice graphicsDevice;
    private List<NpcBase> npcList;
    private List<ITMActor> actorList;
    private List<NpcSpawnBlock> npcSpawnBlocks;
    private List<NpcSpawnBlock> npcSpawnsToUpdate;
    private List<NpcSpawnBlock> npcSpawnsToDelete;
    private Dictionary<GamerID, NpcBase> npcServerList;
    private Dictionary<ActorType, NpcAnimContent> npcContent;
    private Dictionary<int, NpcContentFrame> contentFrames;
    private Dictionary<int, List<Actor>> activeContentFrames;
    private List<NpcManager.NaturalMobSpawn> naturalMobs;
    private int currentNpcSpawnIndex;
    private bool lightingChanged;
    private bool nightNpcsCanSpawn;
    private FindSpace spaceFinder;
    private double lastUpdateSpawnerTimer;
    private float timeSinceLastSpawnerUpdate;
    private int goodFPSCounter;
    private HealthEffect healthEffect;
    private List<NpcBase> tempNpcList;

    List<ITMActor> ITMNpcManager.NpcList
    {
      get
      {
        return this.actorList;
      }
    }

    void ITMNpcManager.GetNpcs(
      Vector3 min,
      Vector3 max,
      ActorType actorType,
      List<ITMActor> result)
    {
      bool flag = actorType == ActorType.None;
      foreach (NpcBase npc in this.npcList)
      {
        if ((flag || npc.ActorType == actorType) && ((double) npc.Position.Y >= (double) min.Y && (double) npc.Position.Y <= (double) max.Y) && ((double) npc.Position.X >= (double) min.X && (double) npc.Position.X <= (double) max.X && ((double) npc.Position.Z >= (double) min.Z && (double) npc.Position.Z <= (double) max.Z)))
          result.Add((ITMActor) npc);
      }
    }

    ITMActor ITMNpcManager.SpawnNpc(
      ActorType actorType,
      Vector3 pos,
      string ai,
      DayOrNight dayOrNight,
      string killScript,
      LootTable lootTable,
      CombatStats? combatStats)
    {
      return (ITMActor) this.SpawnNpc(actorType, pos, ai, dayOrNight, this.instance.GetScript(killScript), lootTable, combatStats);
    }

    void ITMNpcManager.DeactivateNpc(ITMActor npc)
    {
      this.DeactivateNpc(npc as NpcBase);
    }

    private ActorType GetNpcType(string actorType)
    {
      ActorType? enumFromString = Utils.GetEnumFromString<ActorType>(actorType);
      if (!enumFromString.HasValue)
        return ActorType.None;
      return enumFromString.Value;
    }

    public List<NpcBase> GetNpcList()
    {
      return this.npcList;
    }

    public bool HasFreeNpcSlots
    {
      get
      {
        return this.npcList.Count < this.CurrentMaxNpcCount;
      }
    }

    private bool IsPassiveNpcsActive
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.PassiveMobs)
          return !this.nightNpcsCanSpawn;
        return false;
      }
    }

    private bool IsEnemyNpcsActive
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.EnemyMobs)
          return this.nightNpcsCanSpawn;
        return false;
      }
    }

    public Dictionary<int, List<Actor>> ActiveContent
    {
      get
      {
        return this.activeContentFrames;
      }
    }

    public NpcContentFrame GetFrameContent(int id)
    {
      NpcContentFrame npcContentFrame;
      this.contentFrames.TryGetValue(id, out npcContentFrame);
      return npcContentFrame;
    }

    public NpcAnimContent GetAnimContent(ActorType actorType)
    {
      NpcAnimContent npcAnimContent;
      this.npcContent.TryGetValue(actorType, out npcAnimContent);
      return npcAnimContent;
    }

    public NpcManager(GameInstance instance, MapTM map)
    {
      this.instance = instance;
      this.map = map;
      this.spaceFinder = new FindSpace();
      this.npcList = new List<NpcBase>();
      this.actorList = new List<ITMActor>();
      this.naturalMobs = new List<NpcManager.NaturalMobSpawn>();
      this.npcSpawnBlocks = new List<NpcSpawnBlock>();
      this.npcSpawnsToDelete = new List<NpcSpawnBlock>();
      this.npcSpawnsToUpdate = new List<NpcSpawnBlock>();
      this.npcServerList = new Dictionary<GamerID, NpcBase>();
      this.npcContent = new Dictionary<ActorType, NpcAnimContent>();
      this.contentFrames = new Dictionary<int, NpcContentFrame>(100);
      this.activeContentFrames = new Dictionary<int, List<Actor>>(100);
      this.healthEffect = new HealthEffect();
      this.tempNpcList = new List<NpcBase>();
    }

    protected override void LoadContentCore(StudioForge.Engine.Integration.InitState state)
    {
      this.graphicsDevice = CoreGlobals.GraphicsDevice;
      this.IndexBuffer = MapChunkContent.IndexBuffer;
      this.InitNaturalSpawnTypes();
      this.HookEvents();
      this.lastUpdateSpawnerTimer = Globals1.ElapsedWatch.Elapsed.TotalSeconds;
    }

    private void InitNaturalSpawnTypes()
    {
      foreach (ActorTypeDataXML actorTypeDataXml in Globals1.NpcTypeData)
      {
        if (actorTypeDataXml.IsValid && (double) actorTypeDataXml.NaturalSpawnFreq > 0.0)
        {
          switch (actorTypeDataXml.ActorType)
          {
            case ActorType.Dryad:
              if (this.instance.CurrentBiome != BiomeType.Desert)
                break;
              continue;
            case ActorType.Djinn:
              if (this.instance.CurrentBiome != BiomeType.Desert)
                continue;
              break;
          }
          this.naturalMobs.Add(new NpcManager.NaturalMobSpawn()
          {
            ActorType = actorTypeDataXml.ActorType,
            SpawnFreq = actorTypeDataXml.NaturalSpawnFreq
          });
        }
      }
    }

    protected override void UnloadContentCore()
    {
      this.UnhookEvents();
      base.UnloadContentCore();
      foreach (GameObjectBase npc in this.npcList)
        npc.UnloadContent();
      lock (this.npcList)
      {
        this.npcList.Clear();
        this.actorList.Clear();
      }
    }

    private void HookEvents()
    {
      if (this.instance.IsPeacefulMode)
        return;
      this.instance.SunMoon.SunsetEnded += new EventHandler(this.OnSunsetEnded);
      this.instance.SunMoon.SunriseStarted += new EventHandler(this.OnSunriseStarted);
    }

    private void UnhookEvents()
    {
      if (this.instance.IsPeacefulMode)
        return;
      this.instance.SunMoon.SunsetEnded -= new EventHandler(this.OnSunsetEnded);
      this.instance.SunMoon.SunriseStarted -= new EventHandler(this.OnSunriseStarted);
    }

    private void OnSunsetEnded(object sender, EventArgs e)
    {
      this.nightNpcsCanSpawn = true;
    }

    private void OnSunriseStarted(object sender, EventArgs e)
    {
      this.nightNpcsCanSpawn = false;
    }

    public void AddNpcContentFrame(Actor actor, NpcContentFrame frame)
    {
      List<Actor> actorList;
      if (this.activeContentFrames.TryGetValue(frame.ContentID, out actorList))
      {
        actorList.Add(actor);
      }
      else
      {
        List<Actor> nextItem = this.actorListPool.GetNextItem();
        nextItem.Clear();
        nextItem.Add(actor);
        this.activeContentFrames.Add(frame.ContentID, nextItem);
      }
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (NetworkManager.Instance.IsHost)
        this.UpdateMaxNpcCapacity();
      this.UpdateNpcs();
    }

    public void UpdateNpcSpawns()
    {
      if (!this.HasFreeNpcSlots)
        return;
      this.ManageSpawnBlocks();
      this.ManageDynamicNpcSpawns();
    }

    private void UpdateMaxNpcCapacity()
    {
      this.goodFPSCounter = (int) MathHelper.Clamp(TotalMinerGame.Instance.LastUpdateAndDrawMillisecs >= 15L ? (float) (this.goodFPSCounter - 1) : (float) (this.goodFPSCounter + 1), -60f, 60f);
      if (this.goodFPSCounter > 0)
        this.CurrentMaxNpcCount = Math.Min(this.CurrentMaxNpcCount + 1, 500);
      else
        this.CurrentMaxNpcCount = Math.Max(this.CurrentMaxNpcCount - 1, 50);
    }

    private void UpdateNpcs()
    {
      this.activeContentFrames.Clear();
      this.actorListPool.ReleaseAll();
      for (int index = this.npcList.Count - 1; index >= 0; --index)
        this.npcList[index].Update((UpdateState) null);
      this.npcsToUpdate.Clear();
    }

    private void ManageSpawnBlocks()
    {
      if (this.npcSpawnBlocks == null)
        return;
      this.npcSpawnsToUpdate.Clear();
      this.npcSpawnsToDelete.Clear();
      float totalSeconds = (float) Globals1.ElapsedWatch.Elapsed.TotalSeconds;
      int num = 50;
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      lock (this.npcSpawnBlocks)
      {
        for (int currentNpcSpawnIndex = this.currentNpcSpawnIndex; currentNpcSpawnIndex < this.npcSpawnBlocks.Count && currentNpcSpawnIndex < this.currentNpcSpawnIndex + num; ++currentNpcSpawnIndex)
        {
          NpcSpawnBlock npcSpawnBlock = this.npcSpawnBlocks[currentNpcSpawnIndex];
          MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(npcSpawnBlock.Point);
          if (this.map.BlockData[(int) blockIdAndAuxNoCache.BlockID].ClassType != DataBlockType.NPCSpawn)
            this.npcSpawnsToDelete.Add(npcSpawnBlock);
          else if (npcSpawnBlock.ActorType != ActorType.None && (npcSpawnBlock.DayOrNight == DayOrNight.None || !this.nightNpcsCanSpawn && npcSpawnBlock.DayOrNight == DayOrNight.Day || this.nightNpcsCanSpawn && npcSpawnBlock.DayOrNight == DayOrNight.Night) && ((double) totalSeconds - (double) npcSpawnBlock.SpawnTime > (double) npcSpawnBlock.SpawnFrequency && (((int) blockIdAndAuxNoCache.AuxData & 7) == 0 || this.IsEnemyNpcsActive) && (!npcSpawnBlock.RequiresPower || mapStrategyTm.IsBlockReceivingPower(npcSpawnBlock.Point))))
            this.npcSpawnsToUpdate.Add(npcSpawnBlock);
        }
        this.currentNpcSpawnIndex += num;
        if (this.currentNpcSpawnIndex >= this.npcSpawnBlocks.Count)
          this.currentNpcSpawnIndex = 0;
      }
      foreach (DataBlock block in this.npcSpawnsToDelete)
        this.map.MapStrategyTM.RemoveDataBlock(block);
      foreach (NpcSpawnBlock block in this.npcSpawnsToUpdate)
      {
        Player randomPlayer = this.instance.GetRandomPlayer();
        if (randomPlayer != null && this.SpawnNpcInPlayerRange(randomPlayer, block))
        {
          block.SpawnTime = totalSeconds;
          break;
        }
      }
    }

    private void ManageDynamicNpcSpawns()
    {
      double totalSeconds = Globals1.ElapsedWatch.Elapsed.TotalSeconds;
      this.timeSinceLastSpawnerUpdate = (float) (totalSeconds - this.lastUpdateSpawnerTimer);
      this.lastUpdateSpawnerTimer = totalSeconds;
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      int minLight = this.nightNpcsCanSpawn ? 0 : 8;
      int maxLight = this.nightNpcsCanSpawn ? 8 : 15;
      for (int index = 0; index < this.naturalMobs.Count; ++index)
      {
        NpcManager.NaturalMobSpawn naturalMob = this.naturalMobs[index];
        ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) naturalMob.ActorType];
        if (actorTypeDataXml.IsPassive && header.PassiveMobs && !this.nightNpcsCanSpawn || !actorTypeDataXml.IsPassive && header.EnemyMobs && this.nightNpcsCanSpawn)
        {
          naturalMob.Timer += this.timeSinceLastSpawnerUpdate;
          if ((double) naturalMob.Timer >= (double) naturalMob.SpawnFreq)
          {
            this.SpawnNaturalNpc(naturalMob.ActorType, new Point(10, 30), minLight, maxLight);
            naturalMob.Timer = 0.0f;
          }
          this.naturalMobs[index] = naturalMob;
        }
      }
    }

    public void PrepareForDraw()
    {
      this.lightingChanged = false;
    }

    private void PrepareCubeAvatarsForDraw()
    {
      if (this.CubeAvatarModel == null)
      {
        this.CubeAvatarModel = new VertexBuffer(this.graphicsDevice, StudioForge.TotalMiner.Graphics.VertexPosition.vertexDeclaration, 24, BufferUsage.WriteOnly);
        this.CubeAvatarModel.SetData<StudioForge.TotalMiner.Graphics.VertexPosition>(new StudioForge.TotalMiner.Graphics.VertexPosition[24]
        {
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, 0.25f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, -0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, 0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(-0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, -0.5f)),
          new StudioForge.TotalMiner.Graphics.VertexPosition(new Vector3(0.5f, 0.5f, 0.5f))
        });
      }
      this.cubeNpcsToDraw.Clear();
      this.GetAllNpcs(this.cubeNpcsToDraw, false);
      this.CubeAvatarModelInstanceCount = this.cubeNpcsToDraw.Count;
      int modelInstanceCount = this.CubeAvatarModelInstanceCount;
    }

    public static void SetLightInWorldMatrix(
      Map map,
      Vector3 footPos,
      Vector3 eyePos,
      ref Matrix world,
      byte alpha,
      bool perFrameUpdate)
    {
      footPos.Y += 0.1f;
      eyePos.Y += 0.1f;
      GlobalPoint3D point1 = map.GetPoint(footPos);
      GlobalPoint3D point2 = map.GetPoint(eyePos);
      MapLight mapLight1 = perFrameUpdate ? map.GetLight(point1) : map.GetLightNoCache(point1);
      MapLight mapLight2 = perFrameUpdate ? map.GetLight(point2) : map.GetLightNoCache(point2);
      byte num1 = (int) mapLight1.SunLight > (int) mapLight2.SunLight ? mapLight1.SunLight : mapLight2.SunLight;
      byte num2 = (int) mapLight1.BlockLight > (int) mapLight2.BlockLight ? mapLight1.BlockLight : mapLight2.BlockLight;
      world.M44 = (float) (((double) num1 * ((double) map.MaxLight + 1.0) + (double) num2) * 256.0) + (float) alpha;
    }

    public NpcBase GetNpcUsingServerID(GamerID npcID)
    {
      NpcBase npcBase;
      lock (this.npcServerList)
        this.npcServerList.TryGetValue(npcID, out npcBase);
      return npcBase;
    }

    public void UpdateNpcSpawnData(GamerID npcID, long? spawnBlockHash, CombatStats? stats)
    {
      NpcBase npcUsingServerId = this.GetNpcUsingServerID(npcID);
      if (npcUsingServerId == null)
        return;
      if (spawnBlockHash.HasValue)
      {
        GlobalPoint3D pointFromGlobalHash = this.map.GetPointFromGlobalHash(spawnBlockHash.Value);
        npcUsingServerId.SpawnBlock = this.GetNpcSpawnBlock(pointFromGlobalHash);
      }
      if (!stats.HasValue)
        return;
      npcUsingServerId.SetCombatStats(stats.Value);
    }

    private void GetNpcs(
      GlobalPoint3D min,
      GlobalPoint3D max,
      ActorType actorType,
      List<NpcBase> result)
    {
      this.GetNpcs(min.ToVector3(), max.ToVector3() + new Vector3(this.map.TileSize), actorType, result);
    }

    private void GetNpcs(Vector3 min, Vector3 max, ActorType actorType, List<NpcBase> result)
    {
      bool flag = actorType == ActorType.None;
      foreach (NpcBase npc in this.npcList)
      {
        if ((flag || npc.ActorType == actorType) && ((double) npc.Position.Y >= (double) min.Y && (double) npc.Position.Y <= (double) max.Y) && ((double) npc.Position.X >= (double) min.X && (double) npc.Position.X <= (double) max.X && ((double) npc.Position.Z >= (double) min.Z && (double) npc.Position.Z <= (double) max.Z)))
          result.Add(npc);
      }
    }

    public NpcBase SpawnNpc(
      ActorType actorType,
      Vector3 pos,
      string ai,
      Script killScript,
      LootTable lootTable,
      CombatStats? combatStats)
    {
      return this.SpawnNpc(actorType, pos, ai, DayOrNight.None, killScript, lootTable, combatStats);
    }

    private NpcBase SpawnNpc(
      ActorType actorType,
      Vector3 pos,
      string ai,
      DayOrNight dayOrNight,
      Script killScript,
      LootTable lootTable,
      CombatStats? combatStats)
    {
      return this.SpawnNpc(actorType, pos, ai, dayOrNight, new GamerID(++this.NextNpcID), killScript, lootTable, combatStats);
    }

    private NpcBase SpawnNpc(
      ActorType actorType,
      Vector3 pos,
      string ai,
      DayOrNight dayOrNight,
      GamerID npcID,
      Script killScript,
      LootTable lootTable,
      CombatStats? combatStats)
    {
      if (!Globals1.NpcTypeData[(int) actorType].IsValid)
        return (NpcBase) null;
      NpcBase npcBase = new NpcBase(this.instance, this.map, this.GetContent(actorType));
      npcBase.Initialize();
      npcBase.LoadContent();
      npcBase.DayOrNight = dayOrNight;
      if (combatStats.HasValue)
        npcBase.CombatStats.MergeNotZero(combatStats.Value);
      npcBase.NpcSpawn(pos, npcID, killScript);
      npcBase.LoadBehaviour(BehaviourTreeType.AI, ai.IsEmpty() ? "System\\AI\\Default" : ai);
      npcBase.LootTable = lootTable;
      lock (this.npcList)
      {
        this.npcList.Add(npcBase);
        this.actorList.Add((ITMActor) npcBase);
      }
      lock (this.npcServerList)
        this.npcServerList.Add(npcID, npcBase);
      this.instance.AddCentralCharacter((Actor) npcBase);
      return npcBase;
    }

    private NpcAnimContent GetContent(ActorType actorType)
    {
      NpcAnimContent npcAnimContent;
      if (!this.npcContent.TryGetValue(actorType, out npcAnimContent))
      {
        npcAnimContent = new NpcAnimContent(this.instance, actorType);
        npcAnimContent.LoadContent((StudioForge.Engine.Integration.InitState) null);
        this.npcContent.Add(actorType, npcAnimContent);
        foreach (NpcContentFrame frame in npcAnimContent.Frames)
          this.contentFrames.Add(frame.ContentID, frame);
      }
      return npcAnimContent;
    }

    public void NpcSpawnAdded(NpcSpawnBlock block)
    {
      if (block == null)
        return;
      lock (this.npcSpawnBlocks)
        this.npcSpawnBlocks.Add(block);
    }

    public void NpcSpawnRemoved(NpcSpawnBlock block)
    {
      if (block == null)
        return;
      lock (this.npcSpawnBlocks)
        this.npcSpawnBlocks.Remove(block);
      for (int i = this.npcList.Count - 1; i >= 0; --i)
      {
        if (this.npcList[i] != null && this.npcList[i].SpawnBlock == block)
          this.DeactivateNpc(i);
      }
    }

    private bool SpawnNpcInPlayerRange(Player player, NpcSpawnBlock block)
    {
      bool flag = false;
      Vector3 blockCenter = this.map.GetBlockCenter(block.Point);
      if ((double) Vector3.DistanceSquared(player.EyePosition, blockCenter) <= (double) (block.Proximity * block.Proximity))
      {
        int npcCountForBlock = this.GetActiveNpcCountForBlock(block);
        int num = block.MaxActiveInstances;
        if (num == 0)
          num = 20;
        if (npcCountForBlock < num)
          flag = this.SpawnNpc(block);
      }
      return flag;
    }

    private bool SpawnNpc(NpcSpawnBlock block)
    {
      if (this.map.IsPassable(block.Point + new GlobalPoint3D(0, 1, 0)) && this.map.IsPassable(block.Point + new GlobalPoint3D(0, 2, 0)))
        this.SpawnNpc(block, block.Point + new GlobalPoint3D(0, 2, 0));
      else if (this.map.IsPassable(block.Point + new GlobalPoint3D(0, 2, 0)) && this.map.IsPassable(block.Point + new GlobalPoint3D(0, 3, 0)))
        this.SpawnNpc(block, block.Point + new GlobalPoint3D(0, 3, 0));
      else if (!this.spaceFinder.IsBusy && block.ActorType != ActorType.None)
      {
        this.spaceFinder.Initialize(this.instance, new Action<GlobalPoint3D, GlobalPoint3D>(this.SpawnNpc), block.Point, 1, 8);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.spaceFinder, false, PriorityLevel.Normal);
        block.SpawnTime = (float) Globals1.ElapsedWatch.Elapsed.TotalSeconds;
      }
      return true;
    }

    private void SpawnNpc(GlobalPoint3D origin, GlobalPoint3D p)
    {
      this.SpawnNpc(this.GetNpcSpawnBlock(origin), p);
    }

    private void SpawnNpc(NpcSpawnBlock block, GlobalPoint3D p)
    {
      if (block == null)
        return;
      --p.Y;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.Y -= this.map.TileSize * 0.5f;
      NpcBase npcBase = this.SpawnNpc(block.ActorType, blockCenter, block.BehaviourTree, block.DayOrNight, this.instance.GetScript(block.KillScript), block.LootTable, new CombatStats?());
      if (npcBase == null)
        return;
      npcBase.Name = block.Name;
      npcBase.SpawnBlock = block;
      npcBase.SetCombatStats(block.CombatStats);
      npcBase.Health = npcBase.MaxHealth;
      if (block.Inventory == null || !block.Inventory.HasItems())
        return;
      npcBase.Inventory.CopyFrom(block.Inventory);
      for (int inventoryIndex = 0; inventoryIndex < (int) npcBase.Inventory.PackSize && inventoryIndex < npcBase.Inventory.Count; ++inventoryIndex)
      {
        InventoryItem inventoryItem = npcBase.Inventory[inventoryIndex];
        int equipSlotId = npcBase.Inventory.GetEquipSlotID(inventoryItem.ItemID);
        if (equipSlotId < (int) npcBase.Inventory.EquipIndexStart && npcBase.Inventory[equipSlotId].ItemID == Item.None)
          npcBase.EquipFromInventory((Hand) null, inventoryIndex);
      }
      npcBase.EquipBodyFromInventory();
    }

    private NpcSpawnBlock GetNpcSpawnBlock(GlobalPoint3D p)
    {
      return this.instance.MapStrategyTM.GetDataBlock(p) as NpcSpawnBlock;
    }

    private void SpawnNaturalNpc(ActorType actorType, Point range, int minLight, int maxLight)
    {
      Player randomPlayer = this.instance.GetRandomPlayer();
      if (randomPlayer == null)
        return;
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) actorType];
      if (header.SkillsEnabled && !actorTypeDataXml.IsPassive)
      {
        ActorLevelDataXML actorLevelDataXml = Globals1.NpcLevelData[(int) actorTypeDataXml.LevelType];
        int max = SkillData.CombatLevel((float) actorLevelDataXml.HealthLevel, (float) actorLevelDataXml.StrengthLevel, (float) actorLevelDataXml.AttackLevel, (float) actorLevelDataXml.DefenceLevel, (float) actorLevelDataXml.RangedLevel);
        int combatLevel = randomPlayer.CombatLevel;
        int num = (int) Math.Min(4f, (float) combatLevel * 0.75f);
        if (max - num > combatLevel && this.instance.Random.Next(max) > 0)
          return;
      }
      if (this.GetNpcCountNearPosition(actorType, randomPlayer.Position, 80f) >= (actorTypeDataXml.IsPassive ? 2 : 10))
        return;
      GlobalPoint3D point = this.map.GetPoint(randomPlayer.Position);
      this.SpawnNaturalNpc(actorType, point, range, minLight, maxLight);
    }

    private void SpawnNaturalNpc(
      ActorType actorType,
      GlobalPoint3D spawnCenter,
      Point range,
      int minLight,
      int maxLight)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      int num = 0;
      while (++num < 10)
      {
        zero.X = this.instance.Random.Next(range.X, range.Y);
        zero.Z = this.instance.Random.Next(range.X, range.Y);
        if (this.instance.Random.Next(2) == 0)
          zero.X = -zero.X;
        if (this.instance.Random.Next(2) == 0)
          zero.Z = -zero.Z;
        spawnCenter.X += zero.X;
        spawnCenter.Z += zero.Z;
        BoxInt mapBound = this.map.MapBound;
        if (spawnCenter.X >= mapBound.Min.X && spawnCenter.X < mapBound.Max.X && (spawnCenter.Z >= mapBound.Min.Z && spawnCenter.Z < mapBound.Max.Z))
        {
          spawnCenter.Y = (int) this.map.GetHeight(spawnCenter);
          Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(spawnCenter);
          ++spawnCenter.Y;
          byte maxLight1 = this.map.GetLightNoCache(spawnCenter).GetMaxLight((Map) this.map);
          if ((int) maxLight1 >= minLight && (int) maxLight1 <= maxLight && this.CanSpawnOnBlock(actorType, blockIdNoCache))
          {
            this.SpawnNaturalNpc(actorType, spawnCenter);
            break;
          }
        }
      }
    }

    private bool CanSpawnOnBlock(ActorType actorType, Block blockID)
    {
      switch (actorType)
      {
        case ActorType.Duck:
          if (!BlockData.IsGrassOrDirt(blockID) && blockID != Block.Sand && blockID != Block.Water)
            return ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves);
          return true;
        case ActorType.AyrshireCow:
        case ActorType.Sheep:
          if (!BlockData.IsGrassOrDirt(blockID))
            return blockID == Block.Sand;
          return true;
        case ActorType.HighlandCow:
        case ActorType.Alpaca:
          if (!BlockData.IsGrassOrDirt(blockID) && blockID != Block.Sand && blockID != Block.Snow)
            return blockID == Block.SnowLayer;
          return true;
        default:
          return true;
      }
    }

    private bool SpawnNaturalNpc(ActorType actorType, GlobalPoint3D p)
    {
      if (this.instance.IsSurvivalMode || !this.instance.IsInZoneType(p, ZoneType.NoMobs, GamerID.Sys1))
      {
        ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) actorType];
        string str = actorTypeDataXml.NaturalBehaviour;
        if (str.IsEmpty())
          str = "System\\AI\\Passive";
        DayOrNight dayOrNight = !this.nightNpcsCanSpawn || actorTypeDataXml.IsPassive ? DayOrNight.None : DayOrNight.Night;
        NpcBase npcBase = this.SpawnNpc(actorType, this.map.GetBlockCenter(p), str, dayOrNight, (Script) null, (LootTable) null, new CombatStats?());
        if (npcBase != null)
        {
          npcBase.Alpha = 0.0f;
          return true;
        }
      }
      return false;
    }

    private bool IsTimeToSpawnNaturalMob(int freq, float timer)
    {
      if ((double) timer > (double) freq * 0.5)
        return this.instance.Random.Next((int) ((double) (freq * 90) / (double) timer)) == 0;
      return false;
    }

    public int GetNpcCountNearPosition(ActorType type, Vector3 pos, float range)
    {
      int num1 = 0;
      float num2 = range * range;
      for (int index = this.npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && (type == ActorType.None || type == npc.ActorType) && (double) Vector3.DistanceSquared(pos, npc.Position) <= (double) num2)
          ++num1;
      }
      return num1;
    }

    public int GetActiveNpcCountForBlock(NpcSpawnBlock block)
    {
      int num = 0;
      for (int index = this.npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && npc.SpawnBlock == block)
          ++num;
      }
      return num;
    }

    public int GetNpcCount(ActorType actorType)
    {
      if (actorType == ActorType.None)
        return this.npcList.Count;
      int num = 0;
      for (int index = this.npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && npc.ActorType == actorType)
          ++num;
      }
      return num;
    }

    public int GetNpcCountNearPosition(ActorType actorType, Vector3 pos1, Vector3 pos2)
    {
      int num = 0;
      BoundingBox boundingBox = new BoundingBox(pos1, pos2);
      for (int index = this.npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && (actorType == ActorType.None || npc.ActorType == actorType) && boundingBox.Intersects(this.npcList[index].Box))
          ++num;
      }
      return num;
    }

    public void EffectAddHealth(
      ActorType mobType,
      Actor applier,
      GlobalPoint3D p1,
      GlobalPoint3D p2,
      int qty,
      int millisecs,
      int duration)
    {
      lock (this.tempNpcList)
      {
        this.tempNpcList.Clear();
        this.GetNpcs(p1, p2, mobType, this.tempNpcList);
        foreach (Actor tempNpc in this.tempNpcList)
          this.EffectAddHealth(tempNpc, applier, qty, millisecs, duration);
        this.tempNpcList.Clear();
      }
    }

    public void EffectAddHealth(
      Actor receiver,
      Actor applier,
      int qty,
      int millisecs,
      int duration)
    {
      if (millisecs > 0)
      {
        receiver.EffectAddHealth((string) null, qty, millisecs, duration);
      }
      else
      {
        this.healthEffect.Points = qty;
        this.healthEffect.Update((ITMActor) receiver, (ITMActor) applier);
      }
    }

    public void DeactivateNpc(GamerID npcID)
    {
      this.DeactivateNpc(this.GetNpcUsingServerID(npcID));
    }

    public void DeactivateNpc(NpcBase npc)
    {
      if (npc == null)
        return;
      lock (this.npcList)
      {
        this.npcList.Remove(npc);
        this.actorList.Remove((ITMActor) npc);
        this.NpcDeactivated(npc);
      }
    }

    private void DeactivateNpc(int i)
    {
      if (i < 0 || i >= this.npcList.Count)
        return;
      lock (this.npcList)
      {
        NpcBase npc = this.npcList[i];
        this.npcList.RemoveAt(i);
        this.actorList.RemoveAt(i);
        this.NpcDeactivated(npc);
      }
    }

    public void SetNpcState(Zone zone, ActorState state, ActorType npcType)
    {
      this.SetNpcState(zone.Min, zone.Max, state, npcType);
    }

    public void SetNpcState(
      GlobalPoint3D p1,
      GlobalPoint3D p2,
      ActorState state,
      ActorType npcType)
    {
      this.SetNpcState(p1.ToVector3(), p2.ToVector3() + new Vector3(this.map.TileSize), state, npcType);
    }

    public void SetNpcState(Vector3 pos1, Vector3 pos2, ActorState state, ActorType npcType)
    {
      BoundingBox boundingBox = new BoundingBox(pos1, pos2);
      for (int index = this.npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && (npcType == ActorType.None || npc.ActorType == npcType) && boundingBox.Intersects(npc.Box))
          npc.ChangeState(state);
      }
    }

    public void LightHasChanged(MapChunk chunk)
    {
      this.lightingChanged = true;
    }

    public void GetAllNpcs(List<NpcBase> list, bool excludePassive)
    {
      list.Clear();
      list.AddRange((IEnumerable<NpcBase>) this.npcList);
    }

    public void GetAllNpcsToSend(List<NpcBase> npcsToSend)
    {
      npcsToSend.AddRange((IEnumerable<NpcBase>) this.inactiveMobs);
      this.inactiveMobs.Clear();
      this.GetAllNpcs(npcsToSend, false);
    }

    public void GetNpcsToSend(List<NpcBase> npcsToSend, int maxCount, bool resendAll)
    {
      if (!resendAll)
        return;
      this.GetAllNpcsToSend(npcsToSend);
    }

    public void GetActiveNpcIDs(List<short> list)
    {
    }

    public void NpcDeactivated(NpcBase npc)
    {
      this.instance.RemoveCentralCharacter((Actor) npc);
      if (this.instance.IsHost)
        this.inactiveMobs.Add(npc);
      lock (this.npcServerList)
        this.npcServerList.Remove(npc.GamerID);
      TargetingSystem.TargetInactive((INPCBehaviour) npc);
    }

    public NpcBase GetOrAddNpcUsingServerID(ActorType actorType, Vector3 pos, GamerID npcID)
    {
      return this.GetNpcUsingServerID(npcID);
    }

    public void ValidateFullNpcList(List<short> mobIDs)
    {
    }

    public void NotifyBehaviourChanged(string treeName)
    {
      for (int index = 0; index < this.npcList.Count; ++index)
      {
        NpcBase npc = this.npcList[index];
        if (npc != null && !npc.IsDeadOrInactiveOrDisabled)
          npc.ReloadBehaviour(treeName);
      }
    }

    private struct NaturalMobSpawn
    {
      public ActorType ActorType;
      public float SpawnFreq;
      public float Timer;
    }
  }
}
