// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GameInstance
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Arcade;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Renderers;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Screens2;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class GameInstance : DrawableGameObjectBase, ITMGame, ITMWorld
  {
    private static string[] arcadeMachineNames = new string[3]
    {
      "Arcade Machine",
      "Total Invaders",
      "Total Rush"
    };
    public Viewport[] Viewports = new Viewport[4];
    public History History = new History();
    public List<TextMessage> TextMessages = new List<TextMessage>();
    private Dictionary<long, GamerID> blockOpenTable = new Dictionary<long, GamerID>();
    private PcgRandom blastRandom = new PcgRandom(new System.Random().Next());
    private BoundingFrustum tempFrustum = new BoundingFrustum(Matrix.Identity);
    private Stopwatch oomTimer = new Stopwatch();
    private ShiftMapInfiniteWorker shiftLeftWorker = new ShiftMapInfiniteWorker();
    private ShiftMapInfiniteWorker shiftRightWorker = new ShiftMapInfiniteWorker();
    private ShiftMapInfiniteWorker shiftForwardWorker = new ShiftMapInfiniteWorker();
    private ShiftMapInfiniteWorker shiftBackwardWorker = new ShiftMapInfiniteWorker();
    private List<long> blockFlagToRemove = new List<long>();
    private List<Item> tempRares = new List<Item>(100);
    private List<Script> usedScriptList = new List<Script>();
    private const int blastExplosionsWorkingMax = 20;
    private const int maxMemoryThreshold = 1200000000;
    private const int memPerOOM = 1000000;
    private const int maxMemReduction = 300000000;
    private const int timeForOOMDecrement = 60000;
    private const float weight = 0.5f;
    public const float DropTimeDeath = 420f;
    public const float RealtimeToGameTimeFac = 0.01388889f;
    public static float Gravity;
    public static float SlowGravity;
    public static GameInstance Instance;
    public List<GlobalPoint3D> WideCavernPoints;
    public List<GlobalPoint3D> NarrowCavernPoints;
    public List<ArcadeMachine> ArcadeMachines;
    public PcgRandom Random;
    public ParticleModifiers ParticleModifiers;
    public Blueprint[] ClosestBlueprints;
    public double TotalGameTime;
    public MapRenderer MapRenderer;
    public AmbientSoundWorker AmbientSoundManager;
    public IMiniGame MiniGame;
    public CloudMapManager CloudMapManager;
    public VoxelModelManager VoxelModelManager;
    public VoxelModelManager SystemVoxelModelManager;
    public Item[] KeyList;
    public List<Blueprint> BlueprintsToPlace;
    public List<WisdomItem> WisdomsToPlace;
    public List<Item> KeysToPlace;
    public List<MapMarker> MapMarkers;
    public List<MapMarker> GraveMarkers;
    public SunMoon SunMoon;
    public int DrawChunkSearchMillisecs;
    public int DrawChunksMillisecs;
    public int ChunkSortCount;
    public float MaxFarClip;
    public GameScreen CurrentOpenBlock;
    public EmitterParticleSystem EmitterParticleSystem;
    public CreativeCommandQueue CreativeCommandQueue;
    public Wind Wind;
    public HUDElementManager HUDElementManager;
    public List<ScriptIntersectDisplay> ScriptIntersectDisplays;
    public List<string> ScriptCatchupCommands;
    public EntityManager EntityManager;
    public RockLayerTransitionMap RockTransMap;
    private MapTM map;
    private Starfield starMap;
    private SkyCurtain skyCurtain;
    private HudRenderer hudRenderer;
    private HotBarRenderer hotBarRenderer;
    private DialogRenderer dialogRenderer;
    private ParticleManager particleManager;
    private NpcManager npcManager;
    private PlayerIndex? controllingPlayer;
    private ChunkLoader chunkLoader;
    private ChunkLoaderPriority chunkLoaderPriority;
    private List<SavePlayerState> playerSaveState;
    private bool[] lockedTable;
    private IProgressBar loadProgressBar;
    private List<GameInstance.ScreenToAdd> screensToAdd;
    private List<Player> centralPlayerList;
    private List<Actor> actorList;
    private List<Actor> actorList2;
    private List<Actor> actorList2ToAdd;
    private List<Actor> actorList2ToDel;
    private List<GameInstance.PointToIgnore> pointsForCollisionToIgnore;
    private bool strategyIsSet;
    private NetworkManager networkManager;
    private GlobalPoint3D newSignPoint;
    private List<FloodFiller> flooders;
    private byte[] gameDataFromHost;
    private Queue<QueuedBlast> queuedBlasts;
    private int blastExplosionsWorkingCount;
    private FloraManager floraManager;
    private AmbientMusicManager ambientMusic;
    private SleepUpdate sleeper;
    private float sleepTimer;
    private float sleepPeriod;
    private MemoryUsageCalculator memoryCalculator;
    private CaveIn cavein;
    private MapSaveWorker autoSaver;
    private float autoSaveTimer;
    private float autoSaveInProgressTimer;
    private TexturePackLoader texturePackLoader;
    private bool allPlayersSleeping;
    private Inventory spawnInventory;
    private BufferedChangeProcessor bufferedChangeProcessor;
    private List<Script> scripts;
    private List<Script> adventureScripts;
    private Dictionary<ScriptEvent, Script> eventScripts;
    private ChunkCacheManagerCacheRemoval cacheRemovelWorker;
    private List<Actor> localCharacters;
    private ScriptRuntimeWorker scriptRuntimeWorker;
    private ProximityChecker proximityChecker;
    private int lastHour;
    private bool isGamePaused;
    private Dictionary<string, History> clanHistory;
    private ParticleEmitterWorker particleEmitterWorker;
    private bool texturePackNeedsReload;
    private int texturePackReloadCounter;
    private List<GameInstance.ItemCustomSetup> itemCustomSetup;
    private Script[] eventScriptItemSwing;
    private Script[] eventScriptItemEquip;
    private Script[] eventScriptItemUnequip;
    private List<Action<Item, Hand>>[] eventItemSwing;
    private List<Action<Block, byte, GlobalPoint3D, Hand>>[] eventBlockMined;
    private List<Action<Block, GlobalPoint3D, Hand>>[] eventBlockPlaced;
    private List<SoundBroadcast> broadcastSounds;
    private List<string> chatLog;
    private int isActiveEvenIfGamePaused;

    ITMWorld ITMGame.World
    {
      get
      {
        return (ITMWorld) this;
      }
    }

    PcgRandom ITMGame.Random
    {
      get
      {
        return this.Random;
      }
    }

    GraphicsDevice ITMGame.GraphicsDevice
    {
      get
      {
        return TotalMinerGame.Instance.GraphicsDevice;
      }
    }

    IAudioManagerStream ITMGame.AudioManager
    {
      get
      {
        return TotalMinerGame.Instance.AudioManagerFiles;
      }
    }

    WindowManager ITMGame.WindowManager
    {
      get
      {
        return StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.WindowManager;
      }
    }

    ITMTexturePack ITMGame.TexturePack
    {
      get
      {
        return (ITMTexturePack) GraphicStatics.TexturePack;
      }
    }

    ITMPlayer ITMGame.GetPlayer(string gamertag)
    {
      return (ITMPlayer) this.GetPlayer(gamertag);
    }

    ITMPlayer ITMGame.GetLocalPlayer(PlayerIndex playerIndex)
    {
      return (ITMPlayer) this.GetLocalPlayer(playerIndex);
    }

    void ITMGame.GetAllPlayers(List<ITMPlayer> result)
    {
      result.Clear();
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        ITMPlayer tag = allGamer.Tag as ITMPlayer;
        if (tag != null)
          result.Add(tag);
      }
    }

    void ITMGame.AddNotification(string message)
    {
      this.AddNotification(message, NotifyRecipient.Local);
    }

    void ITMGame.AddEventBlockMined(
      Block blockID,
      Action<Block, byte, GlobalPoint3D, ITMHand> action)
    {
      this.AddEventBlockMined(blockID, (Action<Block, byte, GlobalPoint3D, Hand>) action);
    }

    void ITMGame.AddEventBlockPlaced(
      Block blockID,
      Action<Block, GlobalPoint3D, ITMHand> action)
    {
      this.AddEventBlockPlaced(blockID, (Action<Block, GlobalPoint3D, Hand>) action);
    }

    void ITMGame.AddEventItemSwing(Item itemID, Action<Item, ITMHand> action)
    {
      this.AddEventItemSwing(itemID, (Action<Item, Hand>) action);
    }

    void ITMGame.AddConsoleCommand(
      Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog> action,
      string cmd,
      string briefHelp,
      string fullHelp)
    {
      GameConsole.AddCommand(action, cmd, briefHelp, fullHelp);
    }

    bool ITMGame.RunConsoleCommand(
      string command,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      return this.RunConsoleCommand(command, caller, player, log);
    }

    bool ITMGame.RunScript(string scriptName, ITMActor actor)
    {
      Script script = this.GetScript(scriptName);
      if (script == null)
        return false;
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = actor as Actor
      };
      this.ExecuteScript(script, data, true);
      return true;
    }

    void ITMGame.RunSingleScriptCommand(string command, ITMActor actor)
    {
      Script script = new Script("temp", 1);
      script.Commands.Add(command);
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = actor as Actor
      };
      this.ExecuteScript(script, data, true);
    }

    void ITMGame.ReceiveTextMessage(
      string message,
      NetworkGamer sender,
      TextMsgTarget target)
    {
      this.ReceiveTextMessage(target, sender, message);
    }

    void ITMGame.SendTextMessage(
      string message,
      ITMPlayer sender,
      ITMPlayer recipient,
      bool clan,
      bool admins)
    {
      this.SendTextMessage(recipient?.Gamer, sender as Player, !clan || recipient == null ? (string) null : recipient.ClanName, admins, message);
    }

    void ITMGame.RemoveEventBlockMined(
      Block blockID,
      Action<Block, byte, GlobalPoint3D, ITMHand> action)
    {
      this.RemoveEventBlockMined(blockID, (Action<Block, byte, GlobalPoint3D, Hand>) action);
    }

    void ITMGame.RemoveEventBlockPlaced(
      Block blockID,
      Action<Block, GlobalPoint3D, ITMHand> action)
    {
      this.RemoveEventBlockPlaced(blockID, (Action<Block, GlobalPoint3D, Hand>) action);
    }

    void ITMGame.RemoveEventItemSwing(Item itemID, Action<Item, ITMHand> action)
    {
      this.RemoveEventItemSwing(itemID, (Action<Item, Hand>) action);
    }

    void ITMGame.AddItemCustomSetup(Item itemID, Permissions permission)
    {
      this.AddItemCustomSetup(itemID, permission);
    }

    void ITMGame.OpenPauseMenu(NewGuiMenu menu, ITMPlayer player)
    {
      Player player1 = player as Player;
      if (player1 == null)
        return;
      if (Globals2.UseOldMenu)
        this.AddScreen((GameScreen) new PauseMenuScreen(this, player1), player1);
      else
        this.AddScreen((GameScreen) new PauseMenuScreen2(this, player1, menu), player1);
    }

    bool ITMWorld.IsCreativeMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Creative;
      }
    }

    bool ITMWorld.IsSurvivalMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Survival;
      }
    }

    bool ITMWorld.IsPeacefulMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Peaceful;
      }
    }

    bool ITMWorld.IsDigDeepMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.DigDeep;
      }
    }

    bool ITMWorld.IsFiniteResources
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.FiniteMode;
      }
    }

    bool ITMWorld.IsSkillsEnabled
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.SkillsEnabled)
          return this.IsFiniteResources;
        return false;
      }
    }

    bool ITMWorld.IsLocalSkillsEnabled
    {
      get
      {
        if (this.IsSkillsEnabled)
          return Globals2.GameProperties.SaveGame.Header.SkillsLocal;
        return false;
      }
    }

    bool ITMWorld.IsLocalSkills
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      }
    }

    bool ITMWorld.IsPeacefulDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Peaceful;
      }
    }

    bool ITMWorld.IsEasyDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Easy;
      }
    }

    bool ITMWorld.IsNormalDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Normal;
      }
    }

    bool ITMWorld.IsLegendaryDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Legendary;
      }
    }

    ITMMap ITMWorld.Map
    {
      get
      {
        return (ITMMap) this.map;
      }
    }

    SaveMapHead ITMWorld.Header
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.Clone();
      }
    }

    BiomeType ITMWorld.CurrentBiome
    {
      get
      {
        return this.CurrentBiome;
      }
    }

    ITMEntityManager ITMWorld.EntityManager
    {
      get
      {
        return (ITMEntityManager) this.EntityManager;
      }
    }

    ITMEnvManager ITMWorld.EnvironManager
    {
      get
      {
        return (ITMEnvManager) this.MapStrategyTM.EnvManager;
      }
    }

    ITMNpcManager ITMWorld.NpcManager
    {
      get
      {
        return (ITMNpcManager) this.npcManager;
      }
    }

    List<MapMarker> ITMWorld.MapMarkers
    {
      get
      {
        return this.MapMarkers;
      }
    }

    List<MapMarker> ITMWorld.GraveMarkers
    {
      get
      {
        return this.GraveMarkers;
      }
    }

    List<Zone> ITMWorld.Zones
    {
      get
      {
        return this.MapStrategyTM.Zones;
      }
    }

    string ITMWorld.WorldPath
    {
      get
      {
        return Globals2.GameProperties.SaveGame.MapFilePath;
      }
    }

    BoundingBox ITMWorld.GetBlockBox(GlobalPoint3D p, Block blockID)
    {
      return this.GetBlockBox(p, blockID);
    }

    bool ITMWorld.AddParticle(Vector3 pos, ref ParticleData data)
    {
      return this.EmitterParticleSystem.AddParticle(pos, ref data);
    }

    void ITMWorld.AddProjectile(
      Item itemID,
      Vector3 position,
      Vector3 velocity,
      ITMPlayer player,
      bool transmit)
    {
      this.AddProjectile(itemID, position, velocity, player != null ? player.GamerID : GamerID.Sys1, false, transmit);
    }

    void ITMWorld.AddMapMarker(
      GlobalPoint3D p,
      string text,
      MapMarkerType type,
      bool transmit)
    {
      this.AddMapMarker(p, text, type, transmit);
    }

    HitTest ITMWorld.RayBlockTest(Vector3 position, Vector3 dir, float range)
    {
      return this.CalcBlockTarget(position, dir, range);
    }

    bool ITMWorld.IsBlockDeliveringPower(GlobalPoint3D p)
    {
      return this.MapStrategyTM.IsBlockDeliveringPower(p);
    }

    bool ITMWorld.IsBlockReceivingPower(GlobalPoint3D p)
    {
      return this.MapStrategyTM.IsBlockReceivingPower(p);
    }

    void ITMWorld.SetPower(GlobalPoint3D p, bool power, ITMPlayer player)
    {
      this.SetPower(p, power, player != null ? player.GamerID : GamerID.Sys1);
    }

    AudioListener ITMWorld.GetClosestListener(Vector3 position)
    {
      return this.GetClosestListener(position);
    }

    void ITMWorld.BroadcastSound(Vector3 origin, ITMActor actor, SoundType soundType)
    {
      this.BroadcastSound(origin, actor as Actor, soundType);
    }

    bool ITMWorld.IsAnyLocalPlayerInProximity(Vector3 pos, float range, bool eye)
    {
      return this.IsAnyLocalPlayerInProximity(pos, range, eye);
    }

    void ITMWorld.CreateBlast(
      GlobalPoint3D p,
      Item itemID,
      float strength,
      int radius,
      ITMPlayer player)
    {
      this.CreateBlast(p, itemID, strength, radius, player != null ? player.GamerID : GamerID.Sys1);
    }

    bool ITMWorld.CreateFallingBlock(
      GlobalPoint3D p,
      ITMPlayer player,
      UpdateBlockMethod method,
      bool transmit)
    {
      return this.CreateSliderBlock(p, player != null ? player.GamerID : GamerID.Sys1, method, transmit);
    }

    void ITMWorld.FloodPhysics(
      GlobalPoint3D p,
      Block blockID,
      ITMPlayer player,
      bool transmit)
    {
      this.FloodPhysics(p, blockID, player != null ? player.GamerID : GamerID.Sys1, transmit);
    }

    void ITMWorld.TeleportEntities(
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D dest,
      bool relative)
    {
      this.TeleportEntities(min, max, dest, relative);
    }

    public event EventHandler MapNameChanged;

    public event GameInstance.BookIDConfirmedHandler BookIDConfirmed;

    public void Raise_MapNameChanged()
    {
      if (this.MapNameChanged == null)
        return;
      this.MapNameChanged((object) this, EventArgs.Empty);
    }

    private void RaiseBookIDConfirmed(Player player, BookData book, int slotID)
    {
      if (this.BookIDConfirmed == null)
        return;
      this.BookIDConfirmed((object) this, player, book, slotID);
    }

    public void RaiseEventItemSwing(Item itemID, Hand hand)
    {
      List<Action<Item, Hand>> actionList1 = this.eventItemSwing[0];
      if (actionList1 != null)
      {
        foreach (Action<Item, Hand> action in actionList1)
          action(itemID, hand);
      }
      List<Action<Item, Hand>> actionList2 = this.eventItemSwing[(int) itemID];
      if (actionList2 == null)
        return;
      foreach (Action<Item, Hand> action in actionList2)
        action(itemID, hand);
    }

    public void RaiseEventBlockMined(Block blockID, byte aux, GlobalPoint3D p, Hand hand)
    {
      List<Action<Block, byte, GlobalPoint3D, Hand>> actionList1 = this.eventBlockMined[0];
      if (actionList1 != null)
      {
        foreach (Action<Block, byte, GlobalPoint3D, Hand> action in actionList1)
          action(blockID, aux, p, hand);
      }
      List<Action<Block, byte, GlobalPoint3D, Hand>> actionList2 = this.eventBlockMined[(int) blockID];
      if (actionList2 == null)
        return;
      foreach (Action<Block, byte, GlobalPoint3D, Hand> action in actionList2)
        action(blockID, aux, p, hand);
    }

    public void RaiseEventBlockPlaced(Block blockID, GlobalPoint3D p, Hand hand)
    {
      List<Action<Block, GlobalPoint3D, Hand>> actionList1 = this.eventBlockPlaced[0];
      if (actionList1 != null)
      {
        foreach (Action<Block, GlobalPoint3D, Hand> action in actionList1)
          action(blockID, p, hand);
      }
      List<Action<Block, GlobalPoint3D, Hand>> actionList2 = this.eventBlockPlaced[(int) blockID];
      if (actionList2 == null)
        return;
      foreach (Action<Block, GlobalPoint3D, Hand> action in actionList2)
        action(blockID, p, hand);
    }

    public void AddEventItemSwing(Item itemID, Action<Item, Hand> action)
    {
      List<Action<Item, Hand>> actionList = this.eventItemSwing[(int) itemID];
      if (actionList == null)
      {
        actionList = new List<Action<Item, Hand>>();
        this.eventItemSwing[(int) itemID] = actionList;
      }
      if (actionList.Contains(action))
        return;
      actionList.Add(action);
    }

    public void RemoveEventItemSwing(Item itemID, Action<Item, Hand> action)
    {
      this.eventItemSwing[(int) itemID]?.Remove(action);
    }

    public void AddEventBlockMined(Block blockID, Action<Block, byte, GlobalPoint3D, Hand> action)
    {
      List<Action<Block, byte, GlobalPoint3D, Hand>> actionList = this.eventBlockMined[(int) blockID];
      if (actionList == null)
      {
        actionList = new List<Action<Block, byte, GlobalPoint3D, Hand>>();
        this.eventBlockMined[(int) blockID] = actionList;
      }
      if (actionList.Contains(action))
        return;
      actionList.Add(action);
    }

    public void RemoveEventBlockMined(
      Block blockID,
      Action<Block, byte, GlobalPoint3D, Hand> action)
    {
      this.eventBlockMined[(int) blockID]?.Remove(action);
    }

    public void AddEventBlockPlaced(Block blockID, Action<Block, GlobalPoint3D, Hand> action)
    {
      List<Action<Block, GlobalPoint3D, Hand>> actionList = this.eventBlockPlaced[(int) blockID];
      if (actionList == null)
      {
        actionList = new List<Action<Block, GlobalPoint3D, Hand>>();
        this.eventBlockPlaced[(int) blockID] = actionList;
      }
      if (actionList.Contains(action))
        return;
      actionList.Add(action);
    }

    public void RemoveEventBlockPlaced(Block blockID, Action<Block, GlobalPoint3D, Hand> action)
    {
      this.eventBlockPlaced[(int) blockID]?.Remove(action);
    }

    public MapTM Map
    {
      get
      {
        return this.map;
      }
      internal set
      {
        this.map = value;
      }
    }

    public Starfield StarMap
    {
      get
      {
        return this.starMap;
      }
    }

    public SkyCurtain SkyCurtain
    {
      get
      {
        return this.skyCurtain;
      }
    }

    public NpcManager NpcManager
    {
      get
      {
        return this.npcManager;
      }
    }

    public ParticleManager ParticleManager
    {
      get
      {
        return this.particleManager;
      }
    }

    public Matrix WorldNoShake
    {
      get
      {
        return Matrix.Identity;
      }
    }

    public int PlayerCountToSave
    {
      get
      {
        return this.playerSaveState.Count;
      }
    }

    public List<SavePlayerState> PlayerSaves
    {
      get
      {
        return this.playerSaveState;
      }
    }

    public MemoryUsageCalculator MemoryCalculator
    {
      get
      {
        return this.memoryCalculator;
      }
    }

    public Inventory SpawnInventory
    {
      get
      {
        return this.spawnInventory;
      }
    }

    public CreativeModeHelper CreativeModeHelper { get; private set; }

    public List<Script> Scripts
    {
      get
      {
        return this.scripts;
      }
    }

    public int ScriptCount
    {
      get
      {
        return this.scripts.Count;
      }
    }

    public int AdventureScriptCount
    {
      get
      {
        return this.adventureScripts.Count;
      }
    }

    public List<Actor> LocalCharacters
    {
      get
      {
        return this.localCharacters;
      }
    }

    public List<Actor> AllMoveableCharacters
    {
      get
      {
        return this.actorList2;
      }
    }

    public Dictionary<string, History> ClanHistory
    {
      get
      {
        return this.clanHistory;
      }
    }

    public ParticleEmitterWorker ParticleEmitterWorker
    {
      get
      {
        return this.particleEmitterWorker;
      }
    }

    public List<SoundBroadcast> BroadcastSounds
    {
      get
      {
        return this.broadcastSounds;
      }
    }

    public MapStrategyTM MapStrategyTM
    {
      get
      {
        if (this.map == null)
          return (MapStrategyTM) null;
        return this.map.MapStrategy as MapStrategyTM;
      }
    }

    public List<string> ChatLog
    {
      get
      {
        return this.chatLog ?? (this.chatLog = new List<string>());
      }
    }

    public int TopViewMapOpenCount
    {
      get
      {
        int num = 0;
        foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
        {
          if (localEnabledPlayer.IsViewingMainMap)
            ++num;
        }
        return num;
      }
    }

    public long TotalMemoryUsed
    {
      get
      {
        return this.memoryCalculator.LastGCTotalMemory + this.memoryCalculator.UnmanagedMemorySize + this.memoryCalculator.TotalMeshSize;
      }
    }

    public bool MustReduceMeshSize
    {
      get
      {
        if (BaseGame.OOMCount > TotalMinerGame.LastOOMCount)
        {
          int num = 300;
          if (BaseGame.OOMCount > num)
            BaseGame.OOMCount = num;
          TotalMinerGame.LastOOMCount = BaseGame.OOMCount;
          this.oomTimer.Reset();
          this.oomTimer.Start();
        }
        else if (this.oomTimer.ElapsedMilliseconds > 60000L)
        {
          this.oomTimer.Reset();
          this.oomTimer.Start();
          if (BaseGame.OOMCount > 0)
            --BaseGame.OOMCount;
          TotalMinerGame.LastOOMCount = BaseGame.OOMCount;
        }
        return this.TotalMemoryUsed > (long) (1200000000 - Math.Min(300000000, BaseGame.OOMCount * 1000000));
      }
    }

    private bool IsMemoryCompactionImportant
    {
      get
      {
        return this.TotalMemoryUsed > 300000000L;
      }
    }

    public bool CanLoadMeshesOutOfView
    {
      get
      {
        if (this.TotalMemoryUsed >= 400000000L)
          return this.memoryCalculator.TotalMeshSize < 150000000L;
        return true;
      }
    }

    public void MeshSizeReduced(long bytes)
    {
      this.memoryCalculator.TotalMeshSize -= bytes;
    }

    public NetworkManager NetworkManager
    {
      get
      {
        return this.networkManager;
      }
    }

    public int ParticleSystemManagedMemoryUsed
    {
      get
      {
        int num = this.particleManager.ManagedMemoryUsed + this.EmitterParticleSystem.ManagedMemoryUsed;
        foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
        {
          num += localEnabledPlayer.RainParticleSystem.ManagedMemoryUsed;
          num += localEnabledPlayer.HailParticleSystem.ManagedMemoryUsed;
        }
        return num;
      }
    }

    public int ParticleSystemUnmanagedMemoryUsed
    {
      get
      {
        int num = this.particleManager.UnmanagedMemoryUsed + this.EmitterParticleSystem.UnmanagedMemoryUsed;
        foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
        {
          num += localEnabledPlayer.RainParticleSystem.UnmanagedMemoryUsed;
          num += localEnabledPlayer.HailParticleSystem.UnmanagedMemoryUsed;
        }
        return num;
      }
    }

    public bool CanOpenCreativeMenu(Player player)
    {
      if (player.IsGodOrTester)
        return true;
      if (!this.IsCreativeMode)
        return false;
      if (player.IsAdmin)
        return true;
      if (!this.IsFiniteResources)
        return player.HasPermission(Permissions.Edit);
      return false;
    }

    public bool GameDataIsForLateJoiner { get; private set; }

    public void OnViewDistanceChanged()
    {
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
        (localGamer.Tag as Player)?.OnViewDistanceChanged();
    }

    public float CloudHeight
    {
      get
      {
        return (float) Math.Min(this.map.MapBound.Max.Y - 25, (int) this.map.SeaLevel + (Globals2.GameProperties.SaveGame.Header.TerrainData.Biome == BiomeType.Grasslands ? 120 : 100)) * this.map.TileSize;
      }
    }

    public TimeSpan CurrentDaysGameTime
    {
      get
      {
        if (this.IsAvatarDesigner)
          return TimeSpan.FromDays(0.0);
        int num = (int) (((double) this.SunMoon.Rotation + 3.14159274101257) / 6.28318548202515 * 86400.0);
        int hours = num % 3600;
        int minutes = (num - hours * 3600) % 60;
        int seconds = num - hours * 3600 - minutes * 60;
        return new TimeSpan(hours, minutes, seconds);
      }
    }

    public bool IsCombatEnabled
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.CombatEnabled;
      }
      set
      {
        Globals2.GameProperties.SaveGame.Header.CombatEnabled = value;
      }
    }

    public string CanEnablePeaceful
    {
      get
      {
        string canDisableCombat = this.CanDisableCombat;
        if (canDisableCombat != null)
          return canDisableCombat;
        if (this.cavein.IsActive)
          return "\na cave-in is in progress.";
        return (string) null;
      }
    }

    public bool IsEatingOrHealingAllowed
    {
      get
      {
        if (this.MiniGame == null)
          return true;
        return this.MiniGame.IsEatingAllowed;
      }
    }

    public string CanDisableCombat
    {
      get
      {
        return (string) null;
      }
    }

    private bool IsEnemyWithinRangeOfAnyPlayer(int rangeSq)
    {
      return false;
    }

    private bool IsPlayersWithinRangeOfMob(NpcBase mob, int rangeSq)
    {
      if (mob != null)
      {
        foreach (Gamer allEnabledGamer in this.networkManager.AllEnabledGamers)
        {
          Player tag = allEnabledGamer.Tag as Player;
          if (tag != null && (double) Vector3.DistanceSquared(tag.Position, mob.Position) <= (double) rangeSq)
            return true;
        }
      }
      return false;
    }

    public bool IsPlayerBusy()
    {
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        if (localEnabledPlayer.IsBusy)
          return true;
      }
      return false;
    }

    public bool CanRunQueue2Item(IThreadWorkItem item)
    {
      if (!this.IsPlayerBusy() && item != this.chunkLoader)
        return item != this.memoryCalculator;
      return false;
    }

    public bool IsAllLocalPlayersOnTheSurface
    {
      get
      {
        foreach (Gamer localGamer in this.networkManager.LocalGamers)
        {
          Player tag = localGamer.Tag as Player;
          if (tag != null)
          {
            GlobalPoint3D point = this.Map.GetPoint(tag.Position);
            if (point.Y < (int) this.Map.GetHeight(point))
              return false;
          }
        }
        return true;
      }
    }

    public int AllPlayerCount
    {
      get
      {
        return this.networkManager.AllGamerCount;
      }
    }

    public int AllPlayerEnabledCount
    {
      get
      {
        return this.networkManager.AllGamerEnabledCount;
      }
    }

    public int PlayersSleepingCount
    {
      get
      {
        int num = 0;
        foreach (Gamer allEnabledGamer in this.networkManager.AllEnabledGamers)
        {
          Player tag = allEnabledGamer.Tag as Player;
          if (tag != null && tag.IsSleeping)
            ++num;
        }
        return num;
      }
    }

    public BiomeType CurrentBiome { get; set; }

    public bool IsFiniteResources
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.FiniteMode;
      }
    }

    public bool IsWeatherEnabled
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.WeatherActive)
          return Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock != Item.SpaceWorld;
        return false;
      }
    }

    public bool IsSkillsEnabled
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.SkillsEnabled)
          return this.IsFiniteResources;
        return false;
      }
    }

    public bool IsLocalSkillsEnabled
    {
      get
      {
        if (this.IsSkillsEnabled)
          return Globals2.GameProperties.SaveGame.Header.SkillsLocal;
        return false;
      }
    }

    public bool IsLocalSkills
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      }
    }

    public bool IsCreativeMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Creative;
      }
    }

    public bool IsSurvivalMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Survival;
      }
    }

    public bool IsPeacefulMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Peaceful;
      }
    }

    public bool IsDigDeepMode
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.DigDeep;
      }
    }

    public bool IsAvatarDesigner
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.Attribute == MapAttribute.AvatarDesigner;
      }
    }

    public bool IsPeacefulDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Peaceful;
      }
    }

    public bool IsEasyDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Easy;
      }
    }

    public bool IsNormalDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Normal;
      }
    }

    public bool IsLegendaryDifficulty
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.GameDifficulty == GameDifficulty.Legendary;
      }
    }

    public bool IsHost
    {
      get
      {
        if (this.networkManager == null)
          return false;
        return this.networkManager.IsHost;
      }
    }

    public bool IsRemote
    {
      get
      {
        if (this.networkManager == null)
          return true;
        return this.networkManager.IsRemote;
      }
    }

    public void AddMapActiveNotPausedOverride()
    {
      ++this.isActiveEvenIfGamePaused;
    }

    public void RemoveMapActiveNotPausedOverride()
    {
      --this.isActiveEvenIfGamePaused;
    }

    public bool IsMapActive
    {
      get
      {
        if (this.map == null || !this.IsEnabledField || !this.strategyIsSet)
          return false;
        if (this.isGamePaused)
          return this.isActiveEvenIfGamePaused > 0;
        return true;
      }
    }

    public bool IsMapActiveIgnoreGuide
    {
      get
      {
        if (this.map == null || !this.IsEnabledField || !this.strategyIsSet)
          return false;
        if (this.isGamePaused)
          return this.isActiveEvenIfGamePaused > 0;
        return true;
      }
    }

    public void FlagStrategyIsSet()
    {
      this.strategyIsSet = true;
    }

    public Color BackColor
    {
      get
      {
        return TotalMinerGame.Instance.BackColor;
      }
      set
      {
        TotalMinerGame.Instance.BackColor = value;
      }
    }

    public void ToggleHud()
    {
      this.hudRenderer.IsEnabled = !this.hudRenderer.IsEnabled;
    }

    public bool IsSplitScreen
    {
      get
      {
        if (this.networkManager != null)
          return this.networkManager.LocalGamerCount > 1;
        return false;
      }
    }

    public bool IsMultiplayer
    {
      get
      {
        if (!this.networkManager.IsSessionOpen)
          return false;
        if (this.networkManager.Session.SessionType != NetworkSessionType.SystemLink)
          return this.networkManager.Session.SessionType == NetworkSessionType.PlayerMatch;
        return true;
      }
    }

    public bool IsSinglePlayer
    {
      get
      {
        if (!this.IsSplitScreen)
          return !this.IsMultiplayer;
        return false;
      }
    }

    public PlayerIndex? ControllingPlayer
    {
      get
      {
        return this.controllingPlayer;
      }
    }

    public void AddCentralCharacter(Actor c)
    {
      this.actorList.Add(c);
      this.actorList2ToAdd.Add(c);
      if (c.IsLocalGamer)
        this.localCharacters.Add(c);
      Player player = c as Player;
      if (player == null)
        return;
      this.centralPlayerList.Add(player);
    }

    public void RemoveCentralCharacter(Actor c)
    {
      if (c == null)
        return;
      if (this.actorList != null)
      {
        this.actorList.Remove(c);
        this.actorList2ToDel.Add(c);
      }
      if (c.IsLocalGamer && this.localCharacters != null)
        this.localCharacters.Remove(c);
      Player player = c as Player;
      if (player != null && this.centralPlayerList != null)
      {
        this.centralPlayerList.Remove(player);
        this.UnloadUnusedAvatars();
      }
      if (this.map == null)
        return;
      (this.map.MapStrategy as MapStrategyTM)?.RemoveCharacterFromPlates(c.GamerID);
    }

    public float GetClosestPlayerDistance(Vector3 position)
    {
      if (this.networkManager.LocalEnabledPlayers.Count == 1)
      {
        Player localEnabledPlayer = this.networkManager.LocalEnabledPlayers[0];
        return Vector3.Distance(position, localEnabledPlayer.EyePosition);
      }
      float num1 = float.MaxValue;
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        float num2 = Vector3.Distance(position, localEnabledPlayer.EyePosition);
        if ((double) num2 < (double) num1)
          num1 = num2;
      }
      return num1;
    }

    private bool IsTargeting(BlockTargetTypes targets, BlockTargetTypes test)
    {
      return (targets & test) == test;
    }

    public Actor GetClosestCharacter(
      Vector3 position,
      Player owner,
      int randomness,
      float range,
      BlockTargetTypes targets)
    {
      float num1 = float.MaxValue;
      float num2 = float.MinValue;
      float num3 = float.MaxValue;
      range *= range;
      Actor actor1 = (Actor) null;
      bool flag1 = this.IsTargeting(targets, BlockTargetTypes.Owner);
      bool flag2 = this.IsTargeting(targets, BlockTargetTypes.Players);
      bool flag3 = this.IsTargeting(targets, BlockTargetTypes.Mobs);
      bool flag4 = this.IsTargeting(targets, BlockTargetTypes.Admins);
      bool flag5 = this.IsTargeting(targets, BlockTargetTypes.Strongest);
      bool flag6 = this.IsTargeting(targets, BlockTargetTypes.Weakest);
      if (!flag3)
      {
        randomness *= this.centralPlayerList.Count;
        for (int index = 0; index < this.centralPlayerList.Count; ++index)
        {
          Player centralPlayer = this.centralPlayerList[index];
          if (!centralPlayer.IsDeadOrInactiveOrDisabled)
          {
            if (centralPlayer != owner)
            {
              if (!flag4 && centralPlayer.IsAdmin || !flag2)
                continue;
            }
            else if (!flag1)
              continue;
            float num4 = Vector3.DistanceSquared(position, centralPlayer.Position);
            if ((double) num4 <= (double) range)
            {
              float num5 = flag5 || flag6 ? centralPlayer.MaxHealth : 0.0f;
              if (flag5)
              {
                if ((double) num5 >= (double) num2)
                {
                  if ((double) num5 > (double) num2)
                  {
                    num2 = num5;
                    num1 = num4;
                  }
                }
                else
                  continue;
              }
              else if (flag6)
              {
                if ((double) num5 <= (double) num3)
                {
                  if ((double) num5 < (double) num3)
                  {
                    num3 = num5;
                    num1 = num4;
                  }
                }
                else
                  continue;
              }
              if ((double) num4 < (double) num1 || randomness > 0 && this.Random.Next(randomness) == 0)
              {
                num1 = num4;
                actor1 = (Actor) centralPlayer;
              }
            }
          }
        }
      }
      else
      {
        randomness *= this.actorList.Count;
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor2 = this.actorList[index];
          if (!actor2.IsDeadOrInactiveOrDisabled)
          {
            Player player = actor2 as Player;
            if (player != null)
            {
              if (actor2 != owner)
              {
                if (!flag4 && player.IsAdmin || !flag2)
                  continue;
              }
              else if (!flag1)
                continue;
            }
            float num4 = Vector3.DistanceSquared(position, actor2.Position);
            if ((double) num4 <= (double) range)
            {
              float num5 = flag5 || flag6 ? actor2.MaxHealth : 0.0f;
              if (flag5)
              {
                if ((double) num5 >= (double) num2)
                {
                  if ((double) num5 > (double) num2)
                  {
                    num2 = num5;
                    num1 = num4;
                  }
                }
                else
                  continue;
              }
              else if (flag6)
              {
                if ((double) num5 <= (double) num3)
                {
                  if ((double) num5 < (double) num3)
                  {
                    num3 = num5;
                    num1 = num4;
                  }
                }
                else
                  continue;
              }
              if ((double) num4 <= (double) num1 || randomness > 0 && this.Random.Next(randomness) == 0)
              {
                num1 = num4;
                actor1 = actor2;
              }
            }
          }
        }
      }
      return actor1;
    }

    public bool IsItemLocked(Item itemID)
    {
      return this.lockedTable[(int) itemID];
    }

    public bool IsItemUnlocked(Item itemID)
    {
      return !this.lockedTable[(int) itemID];
    }

    public void UnlockItem(Player player, Item itemID, bool transmit)
    {
      this.UnlockItem(itemID, transmit);
    }

    private void UnlockItem(Item itemID, bool transmit)
    {
      if (this.lockedTable[(int) itemID])
      {
        this.lockedTable[(int) itemID] = false;
        if (transmit)
          this.networkManager.SendItemUnlocked(itemID);
      }
      Item blockId = ItemData.ConvertItemIDToBlockID(itemID);
      if (blockId == itemID)
        return;
      this.UnlockItem(blockId, transmit);
    }

    public bool[] LockedTable
    {
      get
      {
        bool[] flagArray = new bool[this.lockedTable.Length];
        this.lockedTable.CopyTo((Array) flagArray, 0);
        return flagArray;
      }
    }

    public bool IsInAnyPlayerSpace(GlobalPoint3D p)
    {
      foreach (Gamer allEnabledGamer in this.networkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null && tag.IsInPlayerSpace(p))
          return true;
      }
      return false;
    }

    public bool IsInAnyLocalPlayerView(BoundingBox box)
    {
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        this.tempFrustum.Matrix = localEnabledPlayer.VirtualPlayer.ViewMatrix * localEnabledPlayer.ProjectionMatrix;
        if (this.tempFrustum.FastIntersect(ref box))
          return true;
      }
      return false;
    }

    public bool IsInAnyLocalPlayerRange(MapChunk chunk, int range, bool ignoreIfFlying)
    {
      GlobalPoint3D globalOffset1 = chunk.GlobalOffset;
      Point3D chunkSize = this.map.ChunkSize;
      Point3D point3D = chunkSize * range;
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        if (!ignoreIfFlying || !localEnabledPlayer.IsFlying)
        {
          MapChunk chunk1 = this.map.GetChunk(localEnabledPlayer.EyePosition);
          if (chunk1 != null)
          {
            GlobalPoint3D globalOffset2 = chunk1.GlobalOffset;
            if (globalOffset2.X >= globalOffset1.X - point3D.X && globalOffset2.X < globalOffset1.X + chunkSize.X + point3D.X && (globalOffset2.Y >= globalOffset1.Y - point3D.Y && globalOffset2.Y < globalOffset1.Y + chunkSize.Y + point3D.Y) && (globalOffset2.Z >= globalOffset1.Z - point3D.Z && globalOffset2.Z < globalOffset1.Z + chunkSize.Z + point3D.Z))
              return true;
          }
        }
      }
      return false;
    }

    public bool IsAnyLocalPlayerInProximity(Vector3 pos, float range, bool eye)
    {
      float num = range * range;
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        if ((double) Vector3.DistanceSquared(pos, eye ? localEnabledPlayer.EyePosition : localEnabledPlayer.Position) <= (double) num)
          return true;
      }
      return false;
    }

    public bool HasPermission(GamerID playerID, Permissions flag)
    {
      if (!playerID.IsGamer)
        return true;
      NetworkGamer gamerById = this.networkManager.Session.FindGamerById(playerID);
      if (gamerById != null)
      {
        Player tag = gamerById.Tag as Player;
        if (tag != null)
          return tag.HasPermission(flag, true);
      }
      return false;
    }

    public AudioListener GetClosestListener(Vector3 pos)
    {
      AudioListener audioListener = (AudioListener) null;
      float num1 = float.MaxValue;
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        float num2 = Vector3.Distance(localEnabledPlayer.Position, pos);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          audioListener = localEnabledPlayer.AudioListener;
        }
      }
      return audioListener;
    }

    public int TotalClipboardsSizeCapacity
    {
      get
      {
        return 30000000;
      }
    }

    public long TotalClipboardsSizeInBytes
    {
      get
      {
        long num = 0;
        foreach (StudioForge.BlockWorld.Map liveMap in StudioForge.BlockWorld.Map.LiveMaps)
        {
          if (!(liveMap is CloudMap) && liveMap.MapSize != this.map.MapSize)
          {
            num += (long) liveMap.MemorySize;
            num += this.GetMeshSize(liveMap);
          }
        }
        return num;
      }
    }

    public long GetMeshSize(StudioForge.BlockWorld.Map map)
    {
      long num = 0;
      if (map != null && map.Regions != null)
      {
        foreach (MapRegion mapRegion in map.Regions.Values)
          num += (long) ((MapRegionTM) mapRegion).TotalMeshSize;
      }
      return num;
    }

    public void TexturePackNeedsReload()
    {
      if (this.texturePackNeedsReload)
        return;
      this.texturePackNeedsReload = true;
      this.texturePackReloadCounter = 30;
    }

    public GameInstance(PlayerIndex? controllingPlayer, IProgressBar loadProgressBar)
    {
      GameInstance.Instance = this;
      this.controllingPlayer = controllingPlayer;
      this.loadProgressBar = loadProgressBar;
      Globals2.Reinitialize();
      ThreadQueueManager.Instance.InitWorkerThreads();
      this.MaxFarClip = (float) ThreadQueueManager.Instance.GetProcessorSpeedScale(352, 1504);
      this.networkManager = NetworkManager.Instance;
      MessageQueue.Initialize();
      TargetingSystem.Initialize();
      this.sleeper = new SleepUpdate(this);
      this.ScriptCatchupCommands = new List<string>(100);
      this.flooders = new List<FloodFiller>();
      this.screensToAdd = new List<GameInstance.ScreenToAdd>();
      this.ClosestBlueprints = new Blueprint[4];
      this.actorList = new List<Actor>(100);
      this.actorList2 = new List<Actor>(100);
      this.actorList2ToAdd = new List<Actor>(100);
      this.actorList2ToDel = new List<Actor>(100);
      this.centralPlayerList = new List<Player>();
      this.localCharacters = new List<Actor>();
      this.pointsForCollisionToIgnore = new List<GameInstance.PointToIgnore>();
      this.ArcadeMachines = new List<ArcadeMachine>();
      this.queuedBlasts = new Queue<QueuedBlast>();
      this.broadcastSounds = new List<SoundBroadcast>();
      this.memoryCalculator = new MemoryUsageCalculator(this, PriorityLevel.Normal, 5000);
      this.GraveMarkers = new List<MapMarker>();
      this.WideCavernPoints = new List<GlobalPoint3D>();
      this.NarrowCavernPoints = new List<GlobalPoint3D>();
      this.itemCustomSetup = new List<GameInstance.ItemCustomSetup>();
      ScriptEditScreen.Clipboard.Clear();
      this.Random = new PcgRandom(new System.Random().Next());
      this.Wind = new Wind(this.Random);
      this.HUDElementManager = new HUDElementManager();
      this.ScriptIntersectDisplays = new List<ScriptIntersectDisplay>();
      this.EntityManager = new EntityManager(this);
      this.ModsLoaded();
    }

    protected override void LoadContentCore(StudioForge.Engine.Integration.InitState state)
    {
      if (this.IsHost)
        Globals2.GameProperties.IsRandomSeed = this.Random.Next();
      Globals2.ResetGameInstance();
      BaseGame.OOMCount = 0;
      GraphicStatics.PhotoData.ClearPhotoThumbnailColorData();
      if (this.loadProgressBar != null)
      {
        this.loadProgressBar.Text = "Initializing Game...";
        this.loadProgressBar.Reset();
        this.loadProgressBar.Factor = 0.03f;
      }
      Sounds.Initialize((ITMGame) this);
      Wisdom.InitializeWisdomScrolls(this);
      Blueprints.InitializeBlueprints(this);
      this.InitializeKeys();
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(0.25f);
      this.VoxelModelManager = new VoxelModelManager(this, (string) null, false);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(0.25f);
      this.SystemVoxelModelManager = new VoxelModelManager(this, "Content\\Map", true);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(0.25f);
      MapChunkContent.BuildChunkIndices(80000, true);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(0.25f);
      if (this.loadProgressBar != null)
        this.loadProgressBar.Factor = 0.8f;
      SaveDataResult data = this.LoadData();
      if (data == null)
        throw new CoreException("The World Header File could not be loaded");
      if (this.loadProgressBar != null)
        this.loadProgressBar.Factor = 0.17f;
      float increment = 0.05882353f;
      if (Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.DigDeep && (double) this.MaxFarClip > 500.0)
        this.MaxFarClip = 500f;
      this.map = data.SaveData.Map as MapTM;
      this.map.SignTextCacheChanged = true;
      this.map.LightCycle = 1f;
      data.SaveData.Header.Attribute = Globals2.GameProperties.SaveGame.Header.Attribute;
      data.SaveData.Header.GameDifficulty = Globals2.GameProperties.SaveGame.Header.GameDifficulty;
      data.SaveData.Header.TexturePack = Globals2.GameProperties.SaveGame.Header.TexturePack;
      Globals2.GameProperties.SaveGame.Header = data.SaveData.Header;
      Globals2.GameSettings = data.SaveData.GameSettings;
      GlobalGamerSettings globalGamerSettings = Globals2.GamertagData.GetGlobalGamerSettings(this.ControllingPlayer.Value);
      if (globalGamerSettings.GlobalOverwrite || Globals2.GameProperties.IsNewMap)
      {
        Globals2.GameSettings = globalGamerSettings.GameSettings.Clone();
        if (Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld)
          Globals2.GameSettings.ViewClouds = false;
      }
      this.CurrentBiome = data.SaveData.Header.TerrainData.Biome;
      OreProperties.Initialize(this.CurrentBiome);
      if (this.CurrentBiome != BiomeType.Flat || data.SaveData.Header.TerrainData.GroundBlock == Item.NaturalWorld)
      {
        this.RockTransMap = new RockLayerTransitionMap();
        this.RockTransMap.Generate((StudioForge.BlockWorld.Map) this.map, this.CurrentBiome == BiomeType.DigDeep ? (byte) 20 : (byte) 10);
      }
      this.scripts = new List<Script>((IEnumerable<Script>) data.SaveData.GameState.Scripts);
      this.AddSystemScriptsToLocalList();
      if (this.IsHost)
        this.AddGlobalScriptsToLocalList();
      this.adventureScripts = new List<Script>(data.SaveData.GameState.AdventureScripts.Count);
      foreach (string adventureScript in data.SaveData.GameState.AdventureScripts)
        this.AddAdventureScript(this.GetScript(adventureScript));
      this.eventScripts = new Dictionary<ScriptEvent, Script>(data.SaveData.GameState.EventScripts.Count);
      foreach (KeyValuePair<ScriptEvent, string> eventScript in data.SaveData.GameState.EventScripts)
        this.SetEventScript(eventScript.Key, this.GetScript(eventScript.Value));
      this.History = new History(data.SaveData.GameState.History);
      if (data.SaveData.GameState.ClanHistory != null)
      {
        this.clanHistory = new Dictionary<string, History>(data.SaveData.GameState.ClanHistory.Count);
        foreach (KeyValuePair<string, History> keyValuePair in data.SaveData.GameState.ClanHistory)
          this.clanHistory.Add(keyValuePair.Key, new History(keyValuePair.Value));
      }
      this.ResetGravity();
      this.lockedTable = data.SaveData.GameState.LockedTable;
      this.playerSaveState = data.SaveData.PlayerState;
      this.spawnInventory = new Inventory(20);
      this.spawnInventory.LoadFromSaveData(data.SaveData.GameState.SpawnInventory);
      this.TotalGameTime = data.SaveData.GameState.TotalGameTime;
      CoreGlobals.AudioManager.SoundVolume = Globals2.GameSettings.SoundVolume;
      CoreGlobals.AudioManager.MusicVolume = Globals2.GameSettings.MusicVolume;
      TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      Globals2.MaxConcurrentPlayers = data.SaveData.GameState.MaxConcurrentPlayerCount;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore = data.SaveData.ArcadeState.TotalInvadersHighScore;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer = data.SaveData.ArcadeState.TotalInvadersHighScoreGamer;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion = data.SaveData.ArcadeState.TotalInvadersHighScoreVersion;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore = data.SaveData.ArcadeState.TotalRushHighScore;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer = data.SaveData.ArcadeState.TotalRushHighScoreGamer;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion = data.SaveData.ArcadeState.TotalRushHighScoreVersion;
      this.scriptRuntimeWorker = new ScriptRuntimeWorker(this, this.map, PriorityLevel.Priority);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      if (!this.IsAvatarDesigner)
      {
        this.SunMoon = new SunMoon(0.005f, 10);
        this.SunMoon.Initialize(state);
        this.SunMoon.Rotation = data.SaveData.GameState.SunRotation;
      }
      this.map.LoadBlockTextures(data.SaveData.GameState.BlockTextures);
      this.LoadTexturePack(false);
      if (!this.IsAvatarDesigner)
      {
        this.SunMoon.LoadGeometry((StudioForge.BlockWorld.Map) this.map, this.MaxFarClip);
        this.SunMoon.SeasonChange += new EventHandler(this.SeasonChange);
      }
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.MapMarkers = data.SaveData.GameState.MapMarkers != null ? new List<MapMarker>((IEnumerable<MapMarker>) data.SaveData.GameState.MapMarkers) : new List<MapMarker>();
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.CreativeModeHelper = new CreativeModeHelper(this, this.map);
      if (!this.IsAvatarDesigner)
      {
        this.starMap = new Starfield(this);
        this.starMap.Initialize(state);
        this.starMap.LoadContent(state);
      }
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.skyCurtain = new SkyCurtain(this);
      this.skyCurtain.Initialize(state);
      this.skyCurtain.LoadContent(state);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.MapRenderer = new MapRenderer(this, this.loadProgressBar);
      this.MapRenderer.Initialize(state);
      this.MapRenderer.LoadContent(state);
      this.MapRenderer.IsEnabled = true;
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.RepairBlueprints();
      this.RepairWisdomScrolls();
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.IsValid && (blueprint.IsDefault || this.IsSurvivalMode || (this.IsPeacefulMode || this.IsCreativeMode)))
          blueprint.IsEnabled = blueprint.IsUnearthed = blueprint.IsGenerated = true;
        if (blueprint.IsEnabled || blueprint.IsUnearthed)
          this.UnlockItem(blueprint.Result.ItemID, false);
        if (blueprint.IsGenerated && this.BlueprintsToPlace != null)
          this.BlueprintsToPlace.Remove(blueprint);
      }
      foreach (WisdomItem wisdom in Wisdom.WisdomList)
      {
        if (wisdom.IsGenerated && this.WisdomsToPlace != null)
          this.WisdomsToPlace.Remove(wisdom);
      }
      this.particleManager = new ParticleManager(this, this.map);
      this.particleManager.Initialize(state);
      this.particleManager.LoadContent(state);
      this.EmitterParticleSystem = new EmitterParticleSystem();
      this.EmitterParticleSystem.Initialize(this.map);
      this.EmitterParticleSystem.LoadContent();
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      if (!this.IsAvatarDesigner)
      {
        this.cavein = new CaveIn(this);
        this.npcManager = new NpcManager(this, this.map);
        this.npcManager.Initialize(state);
        this.npcManager.LoadContent(state);
        this.npcManager.IsEnabled = true;
      }
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.ParticleModifiers = new ParticleModifiers(this, (StudioForge.BlockWorld.Map) this.map);
      this.ambientMusic = new AmbientMusicManager(this, (StudioForge.BlockWorld.Map) this.map);
      this.ambientMusic.Initialize();
      this.ambientMusic.LoadContent();
      this.ambientMusic.IsEnabled = true;
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      MapStrategy mapStrategy = this.map.MapStrategy;
      if (this.IsHost)
      {
        if (!this.IsAvatarDesigner)
          this.floraManager = new FloraManager(this, (StudioForge.BlockWorld.Map) this.map);
        this.autoSaver = new MapSaveWorker(this, true, (IProgressBar) null, new Action<bool, bool>(this.OnAutoSaveComplete));
        this.autoSaveTimer = MapSaveWorker.GetNewAutoSaveTime();
      }
      this.MapStrategyTM.ApplyLoadData(data);
      this.MapRenderer.SignsChanged(true);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      this.hudRenderer = new HudRenderer(this);
      this.hudRenderer.Initialize(state);
      this.hudRenderer.LoadContent(state);
      this.hudRenderer.IsEnabled = true;
      this.hotBarRenderer = new HotBarRenderer();
      this.hotBarRenderer.LoadContent();
      this.dialogRenderer = new DialogRenderer();
      this.dialogRenderer.LoadContent();
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      data.SaveData.Changes = (List<CustomArray<SaveDataBlock>>) null;
      this.bufferedChangeProcessor = new BufferedChangeProcessor();
      this.bufferedChangeProcessor.Initialize(this, this.networkManager);
      this.chunkLoader = new ChunkLoader();
      this.chunkLoader.Initialize(this, this.map, true);
      this.chunkLoaderPriority = new ChunkLoaderPriority(this, (StudioForge.BlockWorld.Map) this.map);
      this.networkManager.GameInstance = this;
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.Plugin != null)
          activeMod.Plugin.InitializeGame((ITMGame) this);
        if (activeMod.PluginBlocks != null)
          activeMod.PluginBlocks.InitializeGame((ITMGame) this);
      }
      this.HookSessionEvents();
      if (this.networkManager.LocalGamerCount == 0)
        throw new CoreException("Your connection to the Host has been lost.\nEither the Host has closed their world or the connection timed out.");
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      Globals2.GameProperties.IsNewMap = false;
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      TotalMinerGame.GameInstance = this;
      if (!this.IsHost)
      {
        this.networkManager.SendLockedInfoRequest();
        this.networkManager.SendCustomDataRequest();
        this.networkManager.SendPlayerSettingsRequest();
        this.loadProgressBar.Text = this.IsHost ? "Preparing Session" : "Waiting For Host...";
        while (!this.networkManager.HostIsReady)
          Thread.Sleep(10);
      }
      ThreadQueueManager.Instance.StartWorkerThreads(new ThreadedWorkerQueueQuery(this.CanRunQueue2Item));
      this.cacheRemovelWorker = new ChunkCacheManagerCacheRemoval();
      this.cacheRemovelWorker.Initialize(this.map);
      if (this.loadProgressBar != null)
        this.loadProgressBar.AddProgress(increment);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.chunkLoader, false, PriorityLevel.Normal);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.chunkLoaderPriority, false, PriorityLevel.Priority);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) (this.AmbientSoundManager = new AmbientSoundWorker(this, PriorityLevel.Normal)), false, PriorityLevel.Normal);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) (this.CreativeCommandQueue = new CreativeCommandQueue(this, PriorityLevel.Normal)), false, PriorityLevel.Normal);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.memoryCalculator, false, PriorityLevel.Normal);
      if (!this.IsAvatarDesigner)
      {
        this.CloudMapManager = new CloudMapManager();
        this.CloudMapManager.Initialise(this, (StudioForge.BlockWorld.Map) this.map);
        if (this.IsHost)
        {
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new FireUpdateWorker(this, PriorityLevel.Normal), false, PriorityLevel.Normal);
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new MobSpawnWorker(this, PriorityLevel.Priority), false, PriorityLevel.Priority);
        }
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) (this.proximityChecker = new ProximityChecker(this, PriorityLevel.Priority)), false, PriorityLevel.Priority);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.scriptRuntimeWorker, false, PriorityLevel.Priority);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new LiquidPhysicsWorker(this, PriorityLevel.Priority), false, PriorityLevel.Priority);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) (this.particleEmitterWorker = new ParticleEmitterWorker(this, PriorityLevel.Priority)), false, PriorityLevel.Priority);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new PlayerSurroundings(this, PriorityLevel.Normal), false, PriorityLevel.Normal);
        if (this.IsDigDeepMode)
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new BlueprintFinderWorker(this, this.map), false, PriorityLevel.Normal);
      }
      if (this.SunMoon != null)
        this.SunMoon.StartPlay();
      this.lastHour = this.CurrentDaysGameTime.Hours;
      if (this.IsHost)
        this.networkManager.SendCommand(NetworkCommand.HostIsReady);
      else
        this.networkManager.SendCommand(NetworkCommand.RemoteIsLoaded, this.networkManager.Session.Host, SendDataOptions.Reliable);
      this.IsEnabled = this.IsEnabledField = true;
      GC.Collect();
      TotalMinerGame.Instance.Activated += new EventHandler<EventArgs>(this.GameActivated);
      this.oomTimer.Start();
      while (InputManager.UseVirtualMouse)
        InputManager.PopVirtualMouse();
    }

    public void ModsLoaded()
    {
      Script[] scriptArray1 = new Script[Globals1.ItemData.Length];
      Script[] scriptArray2 = new Script[Globals1.ItemData.Length];
      Script[] scriptArray3 = new Script[Globals1.ItemData.Length];
      List<Action<Item, Hand>>[] actionListArray1 = new List<Action<Item, Hand>>[Globals1.ItemData.Length];
      List<Action<Block, byte, GlobalPoint3D, Hand>>[] actionListArray2 = new List<Action<Block, byte, GlobalPoint3D, Hand>>[Globals1.BlockData.Length];
      List<Action<Block, GlobalPoint3D, Hand>>[] actionListArray3 = new List<Action<Block, GlobalPoint3D, Hand>>[Globals1.BlockData.Length];
      if (this.eventScriptItemSwing != null)
      {
        this.eventScriptItemSwing.CopyTo((Array) scriptArray1, 0);
        this.eventScriptItemEquip.CopyTo((Array) scriptArray2, 0);
        this.eventScriptItemUnequip.CopyTo((Array) scriptArray3, 0);
        this.eventItemSwing.CopyTo((Array) actionListArray1, 0);
        this.eventBlockMined.CopyTo((Array) actionListArray2, 0);
        this.eventBlockPlaced.CopyTo((Array) actionListArray3, 0);
      }
      this.eventScriptItemSwing = scriptArray1;
      this.eventScriptItemEquip = scriptArray2;
      this.eventScriptItemUnequip = scriptArray3;
      this.eventItemSwing = actionListArray1;
      this.eventBlockMined = actionListArray2;
      this.eventBlockPlaced = actionListArray3;
    }

    private void GameActivated(object sender, EventArgs e)
    {
      if (this.MapRenderer == null)
        return;
      this.MapRenderer.SignsChanged(true);
    }

    protected override void UnloadContentCore()
    {
      TotalMinerGame.Instance.Activated -= new EventHandler<EventArgs>(this.GameActivated);
      this.IsEnabled = this.IsEnabledField = false;
      this.oomTimer.Stop();
      this.oomTimer.Reset();
      List<NetworkGamer> allGamers = this.networkManager.AllGamers;
      List<NetworkGamer> networkGamerList1 = this.networkManager.Session != null ? this.networkManager.Session.AllGamers : (List<NetworkGamer>) null;
      List<NetworkGamer> localGamers = this.networkManager.LocalGamers;
      List<NetworkGamer> networkGamerList2 = this.networkManager.Session != null ? this.networkManager.Session.LocalGamers : (List<NetworkGamer>) null;
      ThreadQueueManager.Instance.ClearAllQueuesAndThreads();
      this.UnhookSessionEvents();
      this.networkManager.EndSession();
      foreach (ArcadeMachine arcadeMachine in this.ArcadeMachines)
        arcadeMachine.UnloadContent();
      this.ArcadeMachines.Clear();
      this.WideCavernPoints.Clear();
      this.NarrowCavernPoints.Clear();
      if (this.CreativeCommandQueue != null)
        this.CreativeCommandQueue.UnloadContent();
      if (this.npcManager != null)
        this.npcManager.UnloadContent();
      if (this.hudRenderer != null)
        this.hudRenderer.UnloadContent();
      if (this.MapRenderer != null)
        this.MapRenderer.UnloadContent();
      if (this.hotBarRenderer != null)
        this.hotBarRenderer.UnloadContent();
      if (this.dialogRenderer != null)
        this.dialogRenderer.UnloadContent();
      if (this.cavein != null)
        this.cavein.UnloadContent();
      if (this.particleManager != null)
        this.particleManager.UnloadContent();
      if (this.CloudMapManager != null)
        this.CloudMapManager.UnloadContent();
      if (this.SystemVoxelModelManager != null)
        this.SystemVoxelModelManager.UnloadContent();
      if (this.VoxelModelManager != null)
        this.VoxelModelManager.UnloadContent();
      if (this.ambientMusic != null)
        this.ambientMusic.UnloadContent();
      if (this.AmbientSoundManager != null)
        this.AmbientSoundManager.UnloadContent();
      if (this.SunMoon != null)
        this.SunMoon.UnloadContent();
      if (this.starMap != null)
        this.starMap.UnloadContent();
      if (this.skyCurtain != null)
        this.skyCurtain.UnloadContent();
      for (int index = StudioForge.BlockWorld.Map.LiveMaps.Count - 1; index >= 0; --index)
        StudioForge.BlockWorld.Map.LiveMaps[index].UnloadContent();
      StudioForge.BlockWorld.Map.LiveMaps.Clear();
      MapChunk.UnloadStaticContent();
      CoreGlobals.AudioManager.UnloadContent();
      MapTopViewScreen.DisposeRT(this, true);
      ChunkGenerator.Pool.ReleaseAll();
      ChunkDecorator.Pool.ReleaseAll();
      ChunkDecoratorPending.Pool.ReleaseAll();
      ChunkMeshCreator.Pool.ReleaseAll();
      ChunksInPlayerViewLoader.ChunksToDrawPool.ReleaseAll();
      OctreeLeavesInPlayerViewLoader.LeavesToDrawPool.ReleaseAll();
      FlatBiome.Pool.ReleaseAll();
      DigDeepBiome.Pool.ReleaseAll();
      DigDeepBiome2.Pool.ReleaseAll();
      SemiAlpineBiome.Pool.ReleaseAll();
      InfiniteBiome.Pool.ReleaseAll();
      DesertBiome.Pool.ReleaseAll();
      GrasslandsBiome.Pool.ReleaseAll();
      TerrainGenerator.Pool.ReleaseAll();
      VoxelMeshBuilder.ReleasePools();
      BiomeBase.AllBiomesReleased();
      MapLightingByChunkThreadedWrapper.Pool.ReleaseAll();
      MapLightingByChunkThreadedWrapper.LightingPool.ReleaseAll();
      MapLightingByPointThreadedWrapper.Pool.ReleaseAll();
      MapLightingByPointThreadedWrapper.LightingPool.ReleaseAll();
      RLEStreamByte.WorkStreamPool.ReleaseAll();
      StudioForge.BlockWorld.Map.RLEStreamBufferManager.Release();
      Actor.Logs.Clear();
      DungeonGenerator.ClearStaticData();
      VegetationGenerator.ClearStaticData();
      foreach (NetworkGamer networkGamer in allGamers)
      {
        (networkGamer.Tag as Player)?.UnloadContent();
        networkGamer.Tag = (object) null;
      }
      if (networkGamerList1 != null)
      {
        foreach (Gamer gamer in networkGamerList1)
          gamer.Tag = (object) null;
      }
      foreach (Gamer gamer in localGamers)
        gamer.Tag = (object) null;
      if (networkGamerList2 != null)
      {
        foreach (Gamer gamer in networkGamerList2)
          gamer.Tag = (object) null;
      }
      Globals2.ResetGameInstance();
      GraphicStatics.GameInstanceCleanup();
      this.BackColor = Color.Black;
      MapChunkContent.VBNewCount = 0;
      MapChunkContent.VBRecycledCount = 0;
      base.UnloadContentCore();
      TotalMinerGame.GameInstance = GameInstance.Instance = NetworkManager.Instance.GameInstance = (GameInstance) null;
      Sounds.Initialize((ITMGame) null);
    }

    private void AddSystemScriptsToLocalList()
    {
      for (int index = this.scripts.Count - 1; index >= 0; --index)
      {
        if (this.scripts[index].Name.StartsWith("system\\", StringComparison.OrdinalIgnoreCase))
        {
          if (Globals2.GetSystemScript(this.scripts[index].Name) == null)
            Globals2.SystemScripts.Add(this.scripts[index]);
          this.scripts.RemoveAt(index);
        }
      }
      this.scripts.AddRange((IEnumerable<Script>) Globals2.SystemScripts);
    }

    private void AddGlobalScriptsToLocalList()
    {
      for (int index = this.scripts.Count - 1; index >= 0; --index)
      {
        if (this.scripts[index].Name.StartsWith("global\\", StringComparison.OrdinalIgnoreCase))
        {
          if (Globals2.GetGlobalScript(this.scripts[index].Name) == null)
            Globals2.GlobalScripts.Add(this.scripts[index]);
          this.scripts.RemoveAt(index);
        }
      }
      this.scripts.AddRange((IEnumerable<Script>) Globals2.GlobalScripts);
    }

    public void ResetMusicShuffle()
    {
      if (this.ambientMusic == null)
        return;
      this.ambientMusic.ResetMusicShuffle();
    }

    public int GetGeneratedHeight(GlobalPoint3D p)
    {
      if (this.CurrentBiome == BiomeType.Flat)
        return (int) this.map.SeaLevel;
      if (this.CurrentBiome == BiomeType.Archipelago || this.CurrentBiome == BiomeType.Continental)
        return (int) this.map.GetHeight(p);
      TerrainGeneratorBase biome;
      int handle;
      MapTM.GetBiome(this.CurrentBiome, out biome, out handle);
      biome.Initialize(this, this.map, Globals2.GameProperties.SaveGame.Header.BiomeParams);
      biome.InitializeForGeneralUse(p);
      int groundHeightGlobal = biome.GetGroundHeightGlobal((StudioForge.BlockWorld.Map) this.map, p.X, p.Z);
      MapTM.ReleaseBiome(this.CurrentBiome, biome, handle);
      return groundHeightGlobal;
    }

    private SaveDataResult LoadData()
    {
      SaveDataResult saveDataResult;
      if (this.IsHost)
      {
        Permissions defaultPermission = Globals2.GameProperties.SaveGame.Header.DefaultPermission;
        saveDataResult = new MapLoader().Load(this, Globals2.GameProperties.SaveGame.MapFilePath, Globals2.GameProperties.IsNewMap, this.loadProgressBar);
        Globals2.GameProperties.SaveGame.Header.DefaultPermission = defaultPermission;
      }
      else if (!Globals2.GameProperties.IsNewMap)
      {
        this.GetGameData();
        if (!this.networkManager.IsSessionOpen)
          throw new Exception("Your connection to the Host has been lost.\nEither the Host has closed their world or the connection timed out.");
        saveDataResult = new MapLoader().LoadMapForClient(this, this.gameDataFromHost, this.loadProgressBar);
        this.gameDataFromHost = (byte[]) null;
      }
      else
        saveDataResult = new MapLoader().NewMap(this, this.loadProgressBar);
      return saveDataResult;
    }

    public void LoadTexturePack(bool threaded)
    {
      this.LoadTexturePack(Globals2.GameProperties.SaveGame.Header.TexturePack, threaded, true, true);
    }

    public void LoadTexturePack(bool threaded, bool ignoreSeasons)
    {
      this.LoadTexturePack(Globals2.GameProperties.SaveGame.Header.TexturePack, threaded, true, ignoreSeasons);
    }

    public void LoadTexturePack(string asset, bool threaded, bool isReload, bool ignoreSeasons)
    {
      if (asset == null)
        asset = "Original HD";
      if (!ignoreSeasons && this.SunMoon != null && (asset.StartsWith("Original") && asset.EndsWith(" HD")))
      {
        switch (this.SunMoon.Season)
        {
          case SeasonType.Autumn:
            asset = "Original Autumn HD";
            break;
          case SeasonType.Winter:
            asset = "Original Winter HD";
            break;
          case SeasonType.Spring:
            asset = "Original Spring HD";
            break;
          default:
            asset = "Original HD";
            break;
        }
      }
      if (threaded)
      {
        this.texturePackLoader = new TexturePackLoader(this.map, asset, new Action<string, string>(this.OnTexturePackLoaded), true, isReload);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.texturePackLoader, false, PriorityLevel.Urgent);
      }
      else
      {
        GraphicStatics.LoadTexturePack(this.map, asset, true, isReload);
        Globals2.GameProperties.SaveGame.Header.TexturePack = GraphicStatics.TexturePack.Name;
      }
    }

    private void OnTexturePackLoaded(string assetSpecified, string texturePackLoaded)
    {
      Globals2.GameProperties.SaveGame.Header.TexturePack = texturePackLoaded == null ? "Original HD" : texturePackLoaded;
      if (!this.IsEnabledField)
        return;
      ItemModelManager.ClearItemCache();
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
        (localGamer.Tag as Player)?.OnTexturePackChanged();
    }

    private void InitializeKeys()
    {
      List<Item> objList = new List<Item>();
      for (int index = 0; index < Globals1.ItemData.Length; ++index)
      {
        string lower = Utils.InsertSpacesBeforeCapitals(((Item) index).ToString()).ToLower();
        if (lower.Length >= 5 && lower.Substring(lower.Length - 4, 4).ToLower() == " key" || lower.Length >= 8 && lower.Substring(0, 7).ToLower() == "key of ")
          objList.Add((Item) index);
      }
      this.KeyList = objList.ToArray();
      this.KeysToPlace = new List<Item>((IEnumerable<Item>) objList);
      this.KeysToPlace.Remove(Item.SkeletonKey);
    }

    public int GetKeyID(Item key)
    {
      for (int index = 0; index < this.KeyList.Length; ++index)
      {
        if (this.KeyList[index] == key)
          return index;
      }
      return -1;
    }

    private void ResetGravity()
    {
      if (!this.map.IsMoonSeed())
      {
        GameInstance.Gravity = -0.01f;
        GameInstance.SlowGravity = -0.005f;
      }
      else
      {
        GameInstance.Gravity = -0.004f;
        GameInstance.SlowGravity = -1f / 400f;
      }
    }

    private void GetGameData()
    {
      this.networkManager.GameDataReceived += new EventHandler<EventArgs>(this.OnGameDataReceived);
      this.networkManager.SendGameDataRequest();
      bool flag = this.loadProgressBar != null;
      float num1 = 0.0f;
      string str = (string) null;
      if (flag)
      {
        str = this.loadProgressBar.Text;
        this.loadProgressBar.Text = "Waiting for Host data...";
        num1 = this.loadProgressBar.Progress;
      }
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      int num2 = 0;
      while (this.networkManager.IsSessionOpen && this.gameDataFromHost == null)
      {
        if (stopwatch.ElapsedMilliseconds > 20L)
        {
          if (flag)
          {
            this.loadProgressBar.AddProgress(1f / 1000f);
            if ((double) this.loadProgressBar.Progress >= 1.0)
              this.loadProgressBar.Reset(num1);
          }
          if (++num2 >= 250)
          {
            this.networkManager.SendGameDataRequest();
            num2 = 0;
          }
          stopwatch.Reset();
          stopwatch.Start();
        }
      }
      if (flag)
      {
        this.loadProgressBar.Reset(num1);
        this.loadProgressBar.Text = str;
      }
      this.networkManager.GameDataReceived -= new EventHandler<EventArgs>(this.OnGameDataReceived);
    }

    private void OnGameDataReceived(object sender, EventArgs e)
    {
      this.gameDataFromHost = (byte[]) sender;
    }

    private void HookSessionEvents()
    {
      while (true)
      {
        try
        {
          this.networkManager.GamerJoined += new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
          break;
        }
        catch (InvalidOperationException ex)
        {
          Thread.Sleep(100);
        }
      }
      this.networkManager.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
    }

    private void UnhookSessionEvents()
    {
      this.networkManager.GamerJoined -= new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
      this.networkManager.GamerLeft -= new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
    }

    private void RLECacheExpandedEventHandler(object sender, EventArgs e)
    {
    }

    private void ChunkCacheExpandedEventHandler(object sender, EventArgs e)
    {
    }

    private void ChunkCacheCompactedEventHandler(object sender, EventArgs e)
    {
    }

    private void SeasonChange(object sender, EventArgs e)
    {
      this.LoadTexturePack(true, false);
      switch (this.SunMoon.Season)
      {
        case SeasonType.Summer:
          this.AddNotification("Summer has arrived", NotifyRecipient.Local);
          break;
        case SeasonType.Autumn:
          this.AddNotification("Autumn has fallen upon us", NotifyRecipient.Local);
          break;
        case SeasonType.Winter:
          this.AddNotification("Winter has crept up on us", NotifyRecipient.Local);
          break;
        case SeasonType.Spring:
          this.AddNotification("Spring has burst forth", NotifyRecipient.Local);
          break;
      }
    }

    public void AddPlayer(Player player)
    {
      player.InitToPlay(this, this.map, 0, (CharacterSkillsData) null);
      player.Initialize();
      player.LoadContent();
      this.AddCentralCharacter((Actor) player);
      SavePlayerState playerStateData = this.GetPlayerStateData(player);
      player.LoadData(playerStateData, Globals2.GameProperties.SaveGame.Header.SaveVersion);
      if (!player.IsLocalGamer)
        return;
      this.ResetPlayerViewports();
      if (this.map == null)
        return;
      player.Raise_EnterMap((StudioForge.BlockWorld.Map) this.map);
    }

    public SavePlayerState GetPlayerStateData(string gamertag)
    {
      foreach (SavePlayerState savePlayerState in this.playerSaveState)
      {
        if (savePlayerState.Gamertag == gamertag)
          return savePlayerState;
      }
      return this.GetNewPlayerStateData(this.GetPlayer(gamertag));
    }

    public SavePlayerState GetPlayerStateData(Player player)
    {
      foreach (SavePlayerState savePlayerState in this.playerSaveState)
      {
        if (savePlayerState.Gamertag == player.Gamer.Gamertag)
          return savePlayerState;
      }
      return this.GetNewPlayerStateData(player);
    }

    public int GetPlayerStateDataIndex(Gamer gamer)
    {
      for (int index = 0; index < this.playerSaveState.Count; ++index)
      {
        if (gamer.Gamertag == this.playerSaveState[index].Gamertag)
          return index;
      }
      return -1;
    }

    private SavePlayerState GetNewPlayerStateData(Player player)
    {
      SavePlayerState savePlayerState = new SavePlayerState()
      {
        IsNewPlayer = true
      };
      if (player == null || player.Gamer == null || (this.map == null || this.playerSaveState == null))
        return savePlayerState;
      savePlayerState.MobType = ActorType.Boy;
      savePlayerState.Gamertag = player.Gamer.Gamertag;
      savePlayerState.Health = player.MaxHealth;
      savePlayerState.Oxygen = player.MaxOxygen;
      savePlayerState.Position = this.map.GetBlockCenter(player.GetShopSpawnPoint());
      savePlayerState.Position = this.map.GetBlockCenter(player.SpawnPoint);
      savePlayerState.Seed = this.Random.Next();
      savePlayerState.ItemsCrafted = new ushort[0];
      savePlayerState.Inventory = new SaveInventoryState();
      savePlayerState.ActionLog = new ActionLog();
      savePlayerState.History = new History();
      if (player.Gamer.IsLocal)
      {
        Gamer signedInGamer = Globals2.GetSignedInGamer(player.PlayerIndex);
        GamertagData gamertagData = Globals2.GamertagData.GetGamertagData(signedInGamer);
        if (gamertagData != null)
          savePlayerState.Settings = gamertagData.Settings.PlayerSettings.Clone();
      }
      savePlayerState.Settings.MapVisible = !this.IsCreativeMode;
      savePlayerState.Settings.BlueprintFinderVisible = this.IsDigDeepMode;
      savePlayerState.MobType = savePlayerState.Settings.MobType;
      this.playerSaveState.Add(savePlayerState);
      if (this.playerSaveState.Count > 1)
      {
        player.NewVisitorTimer = 180;
        player.NewVisitorMsg = "You are visitor " + (object) this.playerSaveState.Count + " to this world";
      }
      savePlayerState.Permission = player.Permission;
      return savePlayerState;
    }

    public void RemovePlayer(Player player, bool callerIsGamerLeftEvent, bool kicked)
    {
      if (player == null)
        return;
      if (kicked)
        player.Permission = Permissions.None;
      MapSaver.BuildPlayerData(this, player, this.GetPlayerStateData(player));
      this.RemoveCentralCharacter((Actor) player);
      if (player.Gamer == null)
        return;
      if (!callerIsGamerLeftEvent)
      {
        player.UnloadContent();
        player.Gamer.Tag = (object) null;
        if (this.networkManager != null)
          this.networkManager.BuildGamerList();
      }
      if (!player.Gamer.IsLocal)
        return;
      if (this.networkManager.LocalPlayerCount > 0)
      {
        if (this.ClosestBlueprints != null && this.ClosestBlueprints.Length > player.ScreenID)
          this.ClosestBlueprints[player.ScreenID] = (Blueprint) null;
        this.ResetPlayerViewports();
      }
      else
        TotalMinerGame.Instance.ExitBackToMainMenu();
    }

    public void ResetPlayerViewports()
    {
      if (this.LocalInitializedPlayerCount > 1)
        this.ResetViewportsForMultiplePlayers();
      else
        this.ResetViewportsForOnePlayer();
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
          tag.FOVNormalized = tag.FOVNormalized;
      }
      if (StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance == null)
        return;
      StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.SetupRenderTargets();
      StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.SetupViewports();
    }

    private void ResetViewportsForOnePlayer()
    {
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
          this.Viewports[tag.ScreenID] = GraphicStatics.DefaultViewport;
      }
    }

    private void ResetViewportsForMultiplePlayers()
    {
      Viewport defaultViewport = GraphicStatics.DefaultViewport;
      Viewport viewport1 = new Viewport();
      int num = 0;
      int initializedPlayerCount = this.LocalInitializedPlayerCount;
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        Viewport viewport2;
        if (tag != null)
        {
          if (initializedPlayerCount == 2)
          {
            if (Globals2.GameSettings.SplitScreenVertical)
            {
              switch (num++)
              {
                case 0:
                  viewport2 = defaultViewport;
                  viewport2.X = 0;
                  viewport2.Y = 0;
                  viewport2.Width = defaultViewport.Width / 2 - 1;
                  viewport2.Height = defaultViewport.Height;
                  this.Viewports[tag.ScreenID] = viewport2;
                  continue;
                case 1:
                  viewport2 = defaultViewport;
                  viewport2.X = defaultViewport.Width / 2 + 1;
                  viewport2.Y = 0;
                  viewport2.Width = defaultViewport.Width / 2 - 1;
                  viewport2.Height = defaultViewport.Height;
                  this.Viewports[tag.ScreenID] = viewport2;
                  continue;
                default:
                  continue;
              }
            }
            else
            {
              switch (num++)
              {
                case 0:
                  viewport2 = defaultViewport;
                  viewport2.X = 0;
                  viewport2.Y = 0;
                  viewport2.Width = defaultViewport.Width;
                  viewport2.Height = defaultViewport.Height / 2 - 1;
                  this.Viewports[tag.ScreenID] = viewport2;
                  continue;
                case 1:
                  viewport2 = defaultViewport;
                  viewport2.X = 0;
                  viewport2.Y = defaultViewport.Height / 2 + 1;
                  viewport2.Width = defaultViewport.Width;
                  viewport2.Height = defaultViewport.Height / 2 - 1;
                  this.Viewports[tag.ScreenID] = viewport2;
                  continue;
                default:
                  continue;
              }
            }
          }
          else
          {
            switch (num++)
            {
              case 0:
                viewport2 = defaultViewport;
                viewport2.X = 0;
                viewport2.Y = 0;
                viewport2.Width = defaultViewport.Width / 2 - 1;
                viewport2.Height = defaultViewport.Height / 2 - 1;
                this.Viewports[tag.ScreenID] = viewport2;
                continue;
              case 1:
                viewport2 = defaultViewport;
                viewport2.X = defaultViewport.Width / 2 + 1;
                viewport2.Y = 0;
                viewport2.Width = defaultViewport.Width / 2 - 1;
                viewport2.Height = defaultViewport.Height / 2 - 1;
                this.Viewports[tag.ScreenID] = viewport2;
                continue;
              case 2:
                viewport2 = defaultViewport;
                viewport2.X = 0;
                viewport2.Y = defaultViewport.Height / 2 + 1;
                viewport2.Width = defaultViewport.Width / 2 - 1;
                viewport2.Height = defaultViewport.Height / 2 - 1;
                this.Viewports[tag.ScreenID] = viewport2;
                continue;
              case 3:
                viewport2 = defaultViewport;
                viewport2.X = defaultViewport.Width / 2 + 1;
                viewport2.Y = defaultViewport.Height / 2 + 1;
                viewport2.Width = defaultViewport.Width / 2 - 1;
                viewport2.Height = defaultViewport.Height / 2 - 1;
                this.Viewports[tag.ScreenID] = viewport2;
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    private string GetFileName(string mapName)
    {
      if (mapName == null)
        return (string) null;
      return mapName + ".dat";
    }

    public void GraphicsDeviceSettingsChanged()
    {
      if (this.MapRenderer == null)
        return;
      this.MapRenderer.GraphicsDeviceSettingsChanged();
    }

    protected override bool HandleInputCore(InputState input, PlayerIndex donotuse)
    {
      return false;
    }

    public bool HandleInput(InputState input)
    {
      bool flag = false;
      foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
      {
        if (!TotalMinerGame.Instance.ScreenManager.IsScreenInputAlreadyHandled(new PlayerIndex?(localEnabledPlayer.PlayerIndex)))
        {
          if (InputManager.IsKeyReleasedNew(localEnabledPlayer.PlayerIndex, Keys.F8))
          {
            ModManager.HotLoadMods(this);
            this.AddScreen((GameScreen) new MessageBoxScreenTM("Mods reloaded", "Ok", localEnabledPlayer), localEnabledPlayer);
            flag = true;
          }
          else if (InputManager.IsKeyReleasedNew(localEnabledPlayer.PlayerIndex, Keys.F2))
          {
            if (++CoreGlobals.DebugVerbosity == 5)
              CoreGlobals.DebugVerbosity = 0;
          }
          else
          {
            GamePadState currentGamePadState = input.CurrentGamePadStates[(int) localEnabledPlayer.PlayerIndex];
            GamePadState lastGamePadState = input.LastGamePadStates[(int) localEnabledPlayer.PlayerIndex];
            flag |= localEnabledPlayer.HandleInput(currentGamePadState, lastGamePadState);
          }
        }
        else
          localEnabledPlayer.ClearInput();
      }
      return flag;
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (this.IsMapActiveIgnoreGuide)
      {
        this.TotalGameTime += (double) Services.ElapsedTime;
        if (!this.networkManager.IsSessionOpen)
        {
          TotalMinerGame.Instance.ExitMessage = "The Game Session has unexpectedly ended";
          TotalMinerGame.Instance.ExitBackToMainMenu();
          return;
        }
        if (this.networkManager.HasBufferedChangesToProcess)
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.bufferedChangeProcessor, true, PriorityLevel.Priority);
        this.EntityManager.Update();
        if (!this.IsSleeping)
          this.UpdateGame(state);
        if (this.npcManager != null && this.npcManager.IsEnabledField)
        {
          MessageQueue.Update();
          TargetingSystem.Update();
          this.npcManager.Update(state);
        }
        this.UpdatePlayers(state);
        this.UpdateCharacterCollision();
        this.UpdatePointsToIgnore();
        this.map.Update(state);
        this.Wind.Update();
        this.UpdateMarkers();
        this.particleManager.Update((UpdateState) null);
        this.EmitterParticleSystem.Update();
        if (this.MapRenderer.IsEnabledField)
          this.MapRenderer.Update(state);
        if (this.ambientMusic.IsEnabledField)
          this.ambientMusic.Update();
        if (this.floraManager != null)
          this.floraManager.Update();
        if (this.cavein != null)
          this.cavein.Update();
        this.ExplodeBlasts();
        for (int index = 0; index < this.ArcadeMachines.Count; ++index)
          this.ArcadeMachines[index].Update();
        foreach (Mod activePlugin in ModManager.ActivePlugins)
          activePlugin.Plugin.Update();
        foreach (Actor actor in this.actorList2ToDel)
          this.actorList2.Remove(actor);
        foreach (Actor actor in this.actorList2ToAdd)
          this.actorList2.Add(actor);
        this.actorList2ToDel.Clear();
        this.actorList2ToAdd.Clear();
      }
      if (this.map == null || !this.IsEnabledField || !this.strategyIsSet)
        return;
      this.UpdateTextMessages();
      this.UpdateBroadcastSounds();
      if (this.IsHost && !Globals2.GameProperties.IsSystemMap)
        this.UpdateAutoSave();
      MapTopViewScreen.DisposeRT(this, false);
      this.ManageCacheRemoval();
      if (!this.texturePackNeedsReload || --this.texturePackReloadCounter > 0)
        return;
      this.texturePackNeedsReload = false;
      this.LoadTexturePack(true);
    }

    public bool IsGamePaused
    {
      get
      {
        return this.isGamePaused;
      }
    }

    public void PauseGame()
    {
      if (!this.IsSinglePlayer)
        return;
      this.isGamePaused = true;
      Globals1.ElapsedWatch.Stop();
    }

    private void ManageCacheRemoval()
    {
      ++this.map.TimeStampRleCache;
      if (this.map.ChunkCacheManager.CachesUsed > 1000)
      {
        ThreadQueueManager.Instance.CancelQueueItem((IThreadWorkItem) this.cacheRemovelWorker, PriorityLevel.Normal);
        this.map.ChunkCacheManager.RemoveCachesStaggered(0.5f);
      }
      else
      {
        if (ThreadQueueManager.Instance.QueueContainsItem((IThreadWorkItem) this.cacheRemovelWorker, PriorityLevel.Normal))
          return;
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.cacheRemovelWorker, false, PriorityLevel.Normal);
      }
    }

    public void UpdateGame(UpdateState state)
    {
      float num = state == null ? Services.ElapsedTime : state.Elapsed;
      if (this.SunMoon != null)
      {
        this.SunMoon.Update(state);
        if (this.IsSleeping)
          this.sleepTimer += num * this.SunMoon.RotationSpeed;
      }
      this.CloudMapManager.Update(state);
    }

    private void UpdatePlayers(UpdateState state)
    {
      bool flag = true;
      foreach (NetworkGamer allEnabledGamer in this.networkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null)
        {
          if (allEnabledGamer.IsLocal)
          {
            tag.Update(state);
            if (this.map.IsInfinite)
              this.CheckForMapShift(tag);
          }
          else
            tag.UpdateRemote();
          if (!tag.IsSleeping)
            flag = false;
        }
      }
      this.allPlayersSleeping = flag;
    }

    private void CheckForMapShift(Player player)
    {
      if (!this.shiftLeftWorker.IsBusy && this.map.LeftBoundaryBox.Contains(player.Position) != ContainmentType.Disjoint)
        this.QueueShiftMapWorker(this.shiftLeftWorker, BlockFace.Left);
      else if (!this.shiftRightWorker.IsBusy && this.map.RightBoundaryBox.Contains(player.Position) != ContainmentType.Disjoint)
        this.QueueShiftMapWorker(this.shiftRightWorker, BlockFace.Right);
      if (!this.shiftForwardWorker.IsBusy && this.map.ForwardBoundaryBox.Contains(player.Position) != ContainmentType.Disjoint)
      {
        this.QueueShiftMapWorker(this.shiftForwardWorker, BlockFace.Forward);
      }
      else
      {
        if (this.shiftBackwardWorker.IsBusy || this.map.BackwardBoundaryBox.Contains(player.Position) == ContainmentType.Disjoint)
          return;
        this.QueueShiftMapWorker(this.shiftBackwardWorker, BlockFace.Backward);
      }
    }

    private void QueueShiftMapWorker(ShiftMapInfiniteWorker worker, BlockFace direction)
    {
      worker.Initialize((StudioForge.BlockWorld.Map) this.map, direction);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) worker, false, PriorityLevel.Normal);
    }

    private void TestEnumSpeed()
    {
    }

    public void QueueChunksForRemoteGenerating(List<long> chunks)
    {
      this.chunkLoader.QueueChunksForRemoteGenerating(chunks);
    }

    public bool AreChunksDecorated(NetworkManager.BufferedChangeBase change)
    {
      if (change.ChunksList != null)
        return this.AreChunksDecorated(change.ChunksList);
      if (!change.ChunkHash.HasValue)
        return true;
      MapChunk chunk = this.map.GetChunk(change.ChunkHash.Value);
      if (chunk != null)
        return chunk.IsDecorated;
      return true;
    }

    private bool AreChunksDecorated(List<long> chunks)
    {
      if (chunks != null && chunks.Count > 0)
      {
        foreach (long chunk1 in chunks)
        {
          MapChunk chunk2 = this.map.GetChunk(chunk1);
          if (chunk2 != null && !chunk2.IsDecorated)
            return false;
        }
      }
      return true;
    }

    private void AddScreens()
    {
      if (StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance == null)
        return;
      lock (this.screensToAdd)
      {
        if (this.screensToAdd.Count <= 0)
          return;
        for (int index = 0; index < this.screensToAdd.Count; ++index)
        {
          try
          {
            GameInstance.ScreenToAdd screenToAdd = this.screensToAdd[index];
            PlayerIndex? controllingPlayer = new PlayerIndex?();
            if (screenToAdd.Player != null)
            {
              controllingPlayer = new PlayerIndex?(screenToAdd.Player.PlayerIndex);
              screenToAdd.Player.ClearInput();
            }
            StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager.AddScreen(screenToAdd.Screen, controllingPlayer);
          }
          catch (Exception ex)
          {
            Services.ExceptionReporter.ReportExceptionCaught(119, ex);
          }
        }
        this.screensToAdd.Clear();
      }
    }

    private void UpdateSundials(int hour)
    {
      this.MapStrategyTM.UpdateSundials(hour);
    }

    private void UpdatePointsToIgnore()
    {
      for (int index = this.pointsForCollisionToIgnore.Count - 1; index >= 0; --index)
      {
        GameInstance.PointToIgnore pointToIgnore = this.pointsForCollisionToIgnore[index];
        --pointToIgnore.Counter;
        if (pointToIgnore.Counter == 0)
          this.pointsForCollisionToIgnore.RemoveAt(index);
        else
          this.pointsForCollisionToIgnore[index] = pointToIgnore;
      }
    }

    public void PlayerEscape(PlayerIndex playerIndex)
    {
      Player localPlayer = this.GetLocalPlayer(playerIndex);
      if (localPlayer == null)
        return;
      if (!localPlayer.IsGod)
        localPlayer.DropAllItems((Item[]) null, UpdateBlockMethod.DropTimeLong);
      localPlayer.DefaultRespawn();
      localPlayer.Raise_EscapedToSurface();
      this.AddNotification(localPlayer, " has escaped to the surface", NotifyRecipient.Remote);
    }

    public int PlayerCount
    {
      get
      {
        return this.networkManager.AllGamerCount;
      }
    }

    public int PlayerEnabledCount
    {
      get
      {
        return this.networkManager.AllGamerEnabledCount;
      }
    }

    public int LocalPlayerCount
    {
      get
      {
        return this.networkManager.LocalGamerCount;
      }
    }

    public int RemotePlayerCount
    {
      get
      {
        return this.networkManager.RemoteGamerCount;
      }
    }

    private int LocalInitializedPlayerCount
    {
      get
      {
        int num = 0;
        foreach (Gamer localGamer in this.networkManager.LocalGamers)
        {
          Player tag = localGamer.Tag as Player;
          if (tag != null && tag.HasBeenInitializedForPlay)
            ++num;
        }
        return num;
      }
    }

    public Actor GetCharacter(GamerID id)
    {
      if (id.ID < (short) 256)
        return (Actor) this.GetPlayer(id);
      if (this.npcManager != null)
        return (Actor) this.npcManager.GetNpcUsingServerID(id);
      return (Actor) null;
    }

    public Actor GetLocalCharacter(GamerID id)
    {
      if (id.ID < (short) 256)
        return (Actor) this.GetLocalPlayer(id);
      if (this.IsHost && this.npcManager != null)
        return (Actor) this.npcManager.GetNpcUsingServerID(id);
      return (Actor) null;
    }

    public Player GetPlayer(GamerID gamerID)
    {
      return this.GetPlayerByID(this.networkManager.AllGamers, gamerID);
    }

    public Player GetPlayer(string gamertag)
    {
      return this.GetPlayerByGamertag(this.networkManager.AllGamers, gamertag);
    }

    public Player GetLocalPlayer(GamerID gamerID)
    {
      if (gamerID.IsGamer)
      {
        foreach (NetworkGamer localGamer in this.networkManager.LocalGamers)
        {
          if (localGamer.ID == gamerID)
            return localGamer.Tag as Player;
        }
      }
      return (Player) null;
    }

    public Player GetLocalPlayer(string gamertag)
    {
      foreach (NetworkGamer localGamer in this.networkManager.LocalGamers)
      {
        if (localGamer != null && localGamer.Gamertag == gamertag)
          return localGamer.Tag as Player;
      }
      return (Player) null;
    }

    public Player GetLocalPlayer(PlayerIndex index)
    {
      foreach (NetworkGamer localGamer in this.networkManager.LocalGamers)
      {
        if (localGamer != null)
        {
          Player tag = localGamer.Tag as Player;
          if (tag != null && tag.PlayerIndex == index)
            return tag;
        }
      }
      return (Player) null;
    }

    public Player GetLocalPlayerByScreenID(int index)
    {
      if (index >= 0)
      {
        List<NetworkGamer> localGamers = this.networkManager.LocalGamers;
        if (index < localGamers.Count)
        {
          NetworkGamer networkGamer = localGamers[index];
          if (networkGamer != null)
            return networkGamer.Tag as Player;
        }
      }
      return (Player) null;
    }

    public Player GetRemotePlayer(GamerID gamerID)
    {
      return this.GetPlayerByID(this.networkManager.RemoteGamers, gamerID);
    }

    public Player GetRemotePlayer(string gamertag)
    {
      return this.GetPlayerByGamertag(this.networkManager.RemoteGamers, gamertag);
    }

    public Player GetPlayerByID(List<NetworkGamer> gamers, GamerID gamerID)
    {
      if (gamerID.IsGamer)
      {
        foreach (NetworkGamer gamer in gamers)
        {
          if (gamer != null && gamer.ID == gamerID)
            return gamer.Tag as Player;
        }
      }
      return (Player) null;
    }

    public Player GetPlayerByGamertag(List<NetworkGamer> gamers, string gamertag)
    {
      if (gamertag != null && gamertag.Length > 0)
      {
        foreach (NetworkGamer gamer in gamers)
        {
          if (gamer != null && gamer.Gamertag == gamertag)
            return gamer.Tag as Player;
        }
      }
      return (Player) null;
    }

    public Player GetRandomPlayer()
    {
      List<NetworkGamer> allEnabledGamers = this.networkManager.AllEnabledGamers;
      if (allEnabledGamers.Count > 0)
      {
        NetworkGamer networkGamer = allEnabledGamers[this.Random.Next(allEnabledGamers.Count)];
        if (networkGamer != null)
        {
          Player tag = networkGamer.Tag as Player;
          if (tag != null && tag.IsEnabledField)
            return tag;
        }
      }
      return (Player) null;
    }

    public Player GetRandomLocalPlayer()
    {
      List<Player> localEnabledPlayers = this.networkManager.LocalEnabledPlayers;
      if (localEnabledPlayers.Count > 0)
        return localEnabledPlayers[this.Random.Next(localEnabledPlayers.Count)];
      return (Player) null;
    }

    public void AddScreen(GameScreen screen, Player player)
    {
      if (!player.IsLocalGamer)
        return;
      lock (this.screensToAdd)
        this.screensToAdd.Add(new GameInstance.ScreenToAdd()
        {
          Player = player,
          Screen = screen
        });
    }

    public void OpenLoadingPlayerViewScreen(Player player)
    {
      if (player == null || !player.IsLocalGamer)
        return;
      player.UpdateBounds();
      if (!player.ShouldUpdatePhysics(player.Box))
      {
        this.ClosePlayerLoadingScreens(player);
        StudioForge.TotalMiner.Screens.BackgroundScreen backgroundScreen = new StudioForge.TotalMiner.Screens.BackgroundScreen();
        this.AddScreen((GameScreen) backgroundScreen, player);
        this.AddScreen((GameScreen) new LoadingPlayerViewScreen(player, this.chunkLoader, (GameScreen) backgroundScreen, false), player);
      }
      else
      {
        if (player.IsEnabledField)
          return;
        player.IsEnabled = true;
        this.networkManager.BuildGamerList();
      }
    }

    private void ClosePlayerLoadingScreens(Player player)
    {
      for (GameScreen topScreen = TotalMinerGame.Instance.ScreenManager.GetTopScreen(new PlayerIndex?(player.PlayerIndex)); topScreen is LoadingPlayerViewScreen || topScreen is StudioForge.TotalMiner.Screens.BackgroundScreen; topScreen = TotalMinerGame.Instance.ScreenManager.GetTopScreen(new PlayerIndex?(player.PlayerIndex)))
        TotalMinerGame.Instance.ScreenManager.RemoveScreen(topScreen);
      for (int index = this.screensToAdd.Count - 1; index >= 0; --index)
      {
        if (this.screensToAdd[index].Player == player && (this.screensToAdd[index].Screen is LoadingPlayerViewScreen || this.screensToAdd[index].Screen is StudioForge.TotalMiner.Screens.BackgroundScreen))
          this.screensToAdd.RemoveAt(index);
      }
    }

    public bool AddBlock(
      GlobalPoint3D p,
      Block blockID,
      out byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool autoTrigger,
      bool transmit,
      bool commit,
      GlobalPoint3D swingTarget,
      BlockFace swingFace,
      int facePos,
      Item itemRawID,
      object tagData)
    {
      auxData = (byte) 0;
      bool flag = false;
      Player player = this.GetPlayer(playerID);
      if (player == null || this.CanPlayerPlaceItem(player, itemRawID, p))
      {
        switch (blockID)
        {
          case Block.Stack:
          case Block.Stack2:
            if (swingFace == BlockFace.Down && this.map.GetBlockID(p) == (byte) 131)
            {
              blockID = Block.UpsideDownStack;
              break;
            }
            break;
          case Block.UpsideDownStack:
            if (swingFace == BlockFace.Up)
            {
              switch ((Block) this.map.GetBlockID(p))
              {
                case Block.Stack:
                case Block.Stack2:
                  blockID = Block.Stack;
                  break;
              }
            }
            else
              break;
                        break;
        }
        auxData = this.CalcAuxForBlockPlacement(p, blockID, player, autoTrigger, swingTarget, swingFace, facePos, itemRawID);
        flag = this.AddBlockNoPermissionCheck(p, blockID, auxData, method, playerID, autoTrigger, transmit, commit, tagData);
      }
      return flag;
    }

    private byte CalcAuxForBlockPlacement(
      GlobalPoint3D p,
      Block blockID,
      Player player,
      bool autoTrigger,
      GlobalPoint3D swingTarget,
      BlockFace swingFace,
      int facePos,
      Item itemRawID)
    {
      byte num1 = 0;
      if (player == null)
        return num1;
      int num2 = facePos & 3;
      player.SetQtyPlaced((byte) 1);
      Block block = blockID;
      if ((uint) block <= 136U)
      {
        if ((uint) block <= 113U)
        {
          switch (block)
          {
            case Block.WoodDoorTop:
            case Block.SteelDoorTop:
              goto label_11;
            case Block.Post:
              goto label_20;
            case Block.Stairs:
              break;
            default:
              goto label_65;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Sign:
              num1 = autoTrigger ? (byte) 0 : this.GetSignAux(p, swingFace, player);
              goto label_66;
            case Block.Stack:
            case Block.UpsideDownStack:
              goto label_17;
            case Block.BedFoot:
              num1 = this.GetBedAux(p, player);
              goto label_66;
            default:
              goto label_65;
          }
        }
      }
      else
      {
        if ((uint) block <= 153U)
        {
          switch (block)
          {
            case Block.LockedDoorTop:
              goto label_11;
            case Block.SnowLayer:
              if ((Block) this.map.GetBlockID(p) == blockID)
              {
                num1 = (byte) Math.Min(7, (int) this.map.GetAuxData(p) + 1);
                player.SetQtyPlaced((byte) 1);
                goto label_66;
              }
              else
              {
                num1 = player.GetCurrentBlockAux(blockID);
                player.SetQtyPlaced((byte) ((uint) num1 + 1U));
                goto label_66;
              }
            case Block.HalfBlock:
              break;
            case Block.Ramp:
            case Block.OneWayGlass:
              goto label_9;
            default:
              goto label_65;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Painting:
            case Block.TrapDoor:
            case Block.Stairs2:
            case Block.Ramp2:
              goto label_9;
            case Block.HalfBlock2:
              break;
            case Block.Stack2:
              goto label_17;
            case Block.Post2:
              goto label_20;
            case Block.SidePost:
            case Block.SidePost2:
              switch (swingFace)
              {
                case BlockFace.Left:
                  num1 = facePos == 0 || facePos == 3 ? (byte) 1 : (byte) 3;
                  goto label_66;
                case BlockFace.Forward:
                  num1 = facePos == 0 || facePos == 3 ? (byte) 4 : (byte) 6;
                  goto label_66;
                case BlockFace.Right:
                  num1 = facePos == 0 || facePos == 3 ? (byte) 0 : (byte) 2;
                  goto label_66;
                case BlockFace.Backward:
                  num1 = facePos == 0 || facePos == 3 ? (byte) 5 : (byte) 7;
                  goto label_66;
                case BlockFace.Up:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 5;
                      goto label_66;
                    case 1:
                      num1 = (byte) 1;
                      goto label_66;
                    case 2:
                      num1 = (byte) 4;
                      goto label_66;
                    case 3:
                      num1 = (byte) 0;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                case BlockFace.Down:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 6;
                      goto label_66;
                    case 1:
                      num1 = (byte) 3;
                      goto label_66;
                    case 2:
                      num1 = (byte) 7;
                      goto label_66;
                    case 3:
                      num1 = (byte) 2;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                default:
                  goto label_66;
              }
            case Block.CornerBlock:
            case Block.CornerBlock2:
              switch (swingFace)
              {
                case BlockFace.Left:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 1;
                      goto label_66;
                    case 1:
                      num1 = (byte) 5;
                      goto label_66;
                    case 2:
                      num1 = (byte) 6;
                      goto label_66;
                    case 3:
                      num1 = (byte) 2;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                case BlockFace.Forward:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 2;
                      goto label_66;
                    case 1:
                      num1 = (byte) 6;
                      goto label_66;
                    case 2:
                      num1 = (byte) 7;
                      goto label_66;
                    case 3:
                      num1 = (byte) 3;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                case BlockFace.Right:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 3;
                      goto label_66;
                    case 1:
                      num1 = (byte) 7;
                      goto label_66;
                    case 2:
                      num1 = (byte) 4;
                      goto label_66;
                    case 3:
                      num1 = (byte) 0;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                case BlockFace.Backward:
                  switch (num2)
                  {
                    case 0:
                      num1 = (byte) 0;
                      goto label_66;
                    case 1:
                      num1 = (byte) 4;
                      goto label_66;
                    case 2:
                      num1 = (byte) 5;
                      goto label_66;
                    case 3:
                      num1 = (byte) 1;
                      goto label_66;
                    default:
                      goto label_66;
                  }
                case BlockFace.Up:
                  num1 = (byte) num2;
                  goto label_66;
                case BlockFace.Down:
                  num1 = (byte) (num2 + 4);
                  goto label_66;
                default:
                  goto label_66;
              }
            default:
              goto label_65;
          }
        }
        num1 = this.GetHalfBlockAux(swingFace);
        goto label_66;
      }
label_9:
      num1 = this.GetAuxRotate(p, blockID, swingFace, player);
      goto label_66;
label_11:
      num1 = this.GetDoorAux(p, player);
      goto label_66;
label_17:
      switch ((Block) this.map.GetBlockID(p))
      {
        case Block.Stack:
        case Block.UpsideDownStack:
        case Block.Stack2:
          num1 = (byte) Math.Min(7, (int) this.map.GetAuxData(p) + 1);
          player.SetQtyPlaced((byte) 1);
          goto label_66;
        default:
          num1 = player.GetCurrentBlockAux(blockID);
          player.SetQtyPlaced((byte) ((uint) num1 + 1U));
          goto label_66;
      }
label_20:
      switch (swingFace)
      {
        case BlockFace.Left:
          num1 = (facePos & 3) >= 2 ? (byte) 3 : (byte) 2;
          goto label_66;
        case BlockFace.Forward:
          num1 = (facePos & 3) >= 2 ? (byte) 4 : (byte) 3;
          goto label_66;
        case BlockFace.Right:
          num1 = (facePos & 3) >= 2 ? (byte) 1 : (byte) 4;
          goto label_66;
        case BlockFace.Backward:
          num1 = (facePos & 3) >= 2 ? (byte) 2 : (byte) 1;
          goto label_66;
        case BlockFace.Up:
          num1 = facePos <= 3 ? (byte) (facePos + 1) : (byte) 0;
          goto label_66;
        case BlockFace.Down:
          num1 = facePos <= 3 ? (byte) (facePos + 1) : (byte) 0;
          goto label_66;
        default:
          goto label_66;
      }
label_65:
      num1 = this.UseSwingFaceForPlacement(blockID) ? (byte) swingFace : this.GetSpecialAuxForPlacement(player.ViewDirection, swingTarget, swingFace, facePos, new InventoryItem((Item) blockID), itemRawID);
label_66:
      if (player != null && this.Map.UsesBlockTextureTable(blockID) && blockID != Block.LockedDoorTop)
        num1 += (byte) (player.GetCurrentBlockTexture(blockID) << 4);
      return num1;
    }

    public bool UseSwingFaceForPlacement(Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 145U)
      {
        switch (block)
        {
          case Block.ClimbingIvy:
          case Block.Book:
          case Block.Key:
          case Block.Crop:
          case Block.Stack:
          case Block.UpsideDownStack:
          case Block.SnowLayer:
            break;
          default:
            goto label_7;
        }
      }
      else
      {
        if ((uint) block <= 154U)
        {
          if (block != Block.HalfBlock && block != Block.Cylinder)
            goto label_7;
        }
        else
        {
          switch (block)
          {
            case Block.PressurePlate:
            case Block.Stack2:
              goto label_5;
            case Block.HalfBlock2:
              break;
            default:
              goto label_7;
          }
        }
        return true;
      }
label_5:
      return false;
label_7:
      if (!this.map.BlockData[(int) blockID].IsIcon)
        return this.map.BlockData[(int) blockID].IsAttached;
      return true;
    }

    private byte GetSpecialAuxForPlacement(
      Vector3 viewDirection,
      GlobalPoint3D swingTarget,
      BlockFace swingFace,
      int facePos,
      InventoryItem item,
      Item itemRaw)
    {
      BlockDataXML blockDataXml = this.map.BlockData[(int) item.ItemID];
      if (blockDataXml.IsRotated && blockDataXml.Buffer == (byte) 0)
      {
        switch (swingFace)
        {
          case BlockFace.Left:
          case BlockFace.Right:
            return 1;
          case BlockFace.Forward:
          case BlockFace.Backward:
            return 2;
          case BlockFace.Up:
          case BlockFace.Down:
            return 0;
        }
      }
      else if (blockDataXml.Buffer == (byte) 4)
      {
        ITMPluginBlocks pluginBlocks = ModManager.GetPluginBlocks((byte) item.ItemID);
        return pluginBlocks != null ? pluginBlocks.GetAuxForPlacement(viewDirection, swingTarget, swingFace, facePos, (Block) itemRaw) : (byte) 0;
      }
      switch (item.ItemID)
      {
        case Item.ClimbingIvy:
          if (swingFace != BlockFace.Down)
            return (byte) swingFace;
          return (byte) ((uint) this.map.GetAuxData(swingTarget) & 7U);
        case Item.Key:
          return this.ConvertDataToAux((Block) item.ItemID, (byte) (itemRaw - (ushort) 330));
        case Item.Crop:
          switch (itemRaw)
          {
            case Item.SugarCaneSeed:
              return 16;
            case Item.TomatoSeed:
              return 32;
            case Item.Potato:
              return 48;
            case Item.Corn:
              return 64;
            default:
              return 0;
          }
        default:
          return 0;
      }
    }

    public bool AddBlockNoPermissionCheck(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool autoTrigger,
      bool transmit,
      bool commit,
      object tagData)
    {
      Block block = blockID;
      bool flag;
      if ((uint) block <= 117U)
      {
        switch (block)
        {
          case Block.Water:
          case Block.Lava:
            flag = this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit) != null;
            goto label_9;
          case Block.WoodDoorTop:
          case Block.SteelDoorTop:
            break;
          case Block.Sign:
            flag = !autoTrigger && this.AddSign(p, blockID, auxData, method, playerID, transmit);
            goto label_9;
          default:
            goto label_8;
        }
      }
      else
      {
        switch (block)
        {
          case Block.Book:
            flag = !autoTrigger && this.AddBookBlock(p, tagData, method, playerID, transmit);
            goto label_9;
          case Block.BedFoot:
            flag = this.AddBed(p, blockID, auxData, method, playerID, transmit);
            goto label_9;
          case Block.LockedDoorTop:
            break;
          default:
            goto label_8;
        }
      }
      flag = this.AddDoor(p, blockID, auxData, method, playerID, transmit);
      goto label_9;
label_8:
      this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit);
      flag = true;
label_9:
      if (flag)
      {
        if (blockID > Block.None)
        {
          ++p.Y;
          Block blockId = (Block) this.map.GetBlockID(p);
          switch (blockId)
          {
            case Block.Water:
            case Block.Lava:
              this.MapStrategyTM.AddLiquidFlow(p, (byte) blockId, method);
              break;
          }
          --p.Y;
        }
        if (commit)
          this.map.Commit();
      }
      return flag;
    }

    private bool CanPlayerPlaceItem(Player player, Item itemID, GlobalPoint3D p)
    {
      Item obj = itemID;
      if ((uint) obj <= 125U)
      {
        if ((uint) obj <= 114U)
        {
          if (obj != Item.SpiderEgg && obj != Item.AmbientSoundBlock)
            goto label_11;
        }
        else if (obj != Item.SteelSpikes && obj != Item.InvisibleBarrier)
          goto label_11;
      }
      else if ((uint) obj <= 143U)
      {
        switch (obj)
        {
          case Item.NPCSpawn:
          case Item.SentryTurret:
          case Item.ProximityDetector:
            break;
          default:
            goto label_11;
        }
      }
      else
      {
        switch (obj)
        {
          case Item.WifiTransmitter:
          case Item.WifiReceiver:
          case Item.ScriptBlock:
          case Item.BucketOfWater:
            break;
          case Item.BucketOfLava:
            if (!player.HasPermission(Permissions.Grief, true))
              return false;
            goto label_11;
          default:
            goto label_11;
        }
      }
      if (!player.HasPermission(Permissions.Creative, true))
        return false;
label_11:
      return !this.IsInZoneType(p, ZoneType.NoEdit, player.GamerID);
    }

    public void AddSapling(GlobalPoint3D p, GamerID playerID)
    {
      if (this.map == null)
        return;
      if (p.Y >= this.Map.MapBound.Max.Y)
        p.Y = this.Map.MapBound.Max.Y - 1;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.Y -= this.map.TileSize * 0.25f;
      blockCenter.X -= 0.25f;
      this.particleManager.AddNew(ParticleType.None, 90f, blockCenter, Vector3.Zero, 0.3f, new InventoryItem(Item.Sapling, 1), this.ParticleModifiers.BlockPickupParticleModifier, 0.0f, -1, (byte) 0, playerID, false, true);
    }

    public bool ClearBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      ClearBlockResult clearBlockResult = this.map.ClearBlock(p, method, gamerID, transmit);
      switch (clearBlockResult)
      {
        case ClearBlockResult.Success:
          return true;
        case ClearBlockResult.AlreadyClear:
          return false;
        default:
          if (this.GetLocalPlayer(gamerID) != null)
          {
            CoreGlobals.Message.ShowMessage(Utils.InsertSpacesBeforeCapitals(clearBlockResult.ToString()), new Vector2(0.0f, -1f), 2f, 2.5f, Color.Red);
            goto case ClearBlockResult.AlreadyClear;
          }
          else
            goto case ClearBlockResult.AlreadyClear;
      }
    }

    public GamerID IsBlockOpen(GlobalPoint3D p)
    {
      lock (this.blockOpenTable)
      {
        GamerID gamerId;
        if (this.blockOpenTable.TryGetValue(this.map.GetGlobalHashCode(p), out gamerId))
          return gamerId;
        return GamerID.Sys1;
      }
    }

    public bool FlagBlockIsOpen(GlobalPoint3D p, Block blockID, GamerID gamerID)
    {
      if ((blockID == Block.ItemShop || blockID == Block.BlockShop) && this.map.GetAuxData(p) == (byte) 0)
        return true;
      long globalHashCode = this.map.GetGlobalHashCode(p);
      lock (this.blockOpenTable)
      {
        GamerID gamerId;
        if (this.blockOpenTable.TryGetValue(globalHashCode, out gamerId))
          return gamerId == gamerID;
        this.blockOpenTable.Add(globalHashCode, gamerID);
        return true;
      }
    }

    public void FlagBlockIsClosed(GamerID gamerID, bool transmit)
    {
      if (transmit)
        this.networkManager.SendCloseBlock(gamerID);
      lock (this.blockOpenTable)
      {
        foreach (KeyValuePair<long, GamerID> keyValuePair in this.blockOpenTable)
        {
          if (keyValuePair.Value == gamerID)
            this.blockFlagToRemove.Add(keyValuePair.Key);
        }
        foreach (long key in this.blockFlagToRemove)
          this.blockOpenTable.Remove(key);
      }
      foreach (long hash in this.blockFlagToRemove)
      {
        GlobalPoint3D pointFromGlobalHash = this.map.GetPointFromGlobalHash(hash);
        Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(pointFromGlobalHash);
        if (blockIdNoCache == Block.Chest)
          this.DeliverPower(pointFromGlobalHash, blockIdNoCache, BlockFace.ProxyDefault, false, UpdateBlockMethod.Player, gamerID, false, false);
      }
      this.blockFlagToRemove.Clear();
    }

    public void FlagBlockIsClosed(long hash)
    {
      lock (this.blockOpenTable)
        this.blockOpenTable.Remove(hash);
    }

    public bool HasBlockOpen(GamerID gamerID)
    {
      lock (this.blockOpenTable)
      {
        foreach (GamerID gamerId in this.blockOpenTable.Values)
        {
          if (gamerId == gamerID)
            return true;
        }
      }
      return false;
    }

    public GameScreen OpenSpecialBlock(Player player, GlobalPoint3D p, Block blockID)
    {
      return this.OpenSpecialBlock(player, p, blockID, (Hand) null);
    }

    public GameScreen OpenSpecialBlock(
      Player player,
      GlobalPoint3D p,
      Block blockID,
      Hand hand)
    {
      return this.OpenSpecialBlock(player, p, blockID, hand, true);
    }

    public GameScreen OpenSpecialBlock(
      Player player,
      GlobalPoint3D p,
      Block blockID,
      Hand hand,
      bool includeSecurityChecks)
    {
      if (player == null || !player.IsLocalGamer || includeSecurityChecks && !this.CanOpenBlock(player, p, blockID, hand))
        return (GameScreen) null;
      if (blockID == Block.Workbench)
        return this.OpenBlockConfirmationCore(player, p, blockID);
      return this.OpenSpecialBlockCore(player, p, blockID);
    }

    public bool CanOpenBlock(Player player, GlobalPoint3D p, Block blockID, Hand hand)
    {
      if (player != null && player.Gamer != null)
      {
        if (player.IsGod)
          return true;
        Permissions blockOpenPermission = this.GetSpecialBlockOpenPermission(p, blockID);
        if (blockOpenPermission == Permissions.None || player.HasPermissionAny(blockOpenPermission))
        {
          switch (blockID)
          {
            case Block.LockedChest:
              ChestBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as ChestBlock;
              if (hand == null)
                return false;
              return this.IsKeyAndCanOpen(player, (PlayerBlock) dataBlock, p, hand.ItemID);
            case Block.Safe:
              return this.MapStrategyTM.IsBlockReceivingPower(p);
            default:
              return true;
          }
        }
      }
      return false;
    }

    private Permissions GetSpecialBlockOpenPermission(GlobalPoint3D p, Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 114U)
      {
        if ((uint) block <= 50U)
        {
          switch (block)
          {
            case Block.Bookcase:
            case Block.Furnace:
            case Block.Chest:
              break;
            default:
              goto label_15;
          }
        }
        else
        {
          switch (block)
          {
            case Block.ItemShop:
            case Block.BlockShop:
              if (((int) this.map.GetAuxDataNoCache(p) & 7) == 1)
                return Permissions.Adventure;
              return !this.IsFiniteResources ? Permissions.Creative | Permissions.SystemShops : Permissions.SystemShops;
            case Block.AmbientSoundBlock:
              goto label_8;
            default:
              goto label_15;
          }
        }
      }
      else if ((uint) block <= 137U)
      {
        switch (block)
        {
          case Block.LockedChest:
          case Block.LitFurnace:
            break;
          case Block.NPCSpawn:
            goto label_8;
          default:
            goto label_15;
        }
      }
      else
      {
        switch (block)
        {
          case Block.SentryTurret:
            goto label_8;
          case Block.Safe:
            break;
          case Block.ScriptBlock:
            return Permissions.Admin;
          default:
            goto label_15;
        }
      }
      return Permissions.Adventure;
label_8:
      return Permissions.Creative;
label_15:
      return Permissions.None;
    }

    private GameScreen OpenSpecialBlockCore(
      Player player,
      GlobalPoint3D p,
      Block blockID)
    {
      if (this.map.IsHost)
      {
        GamerID gamerId = this.IsBlockOpen(p);
        return this.OpenBlockConfirmation(player, p, blockID, !gamerId.IsGamer || gamerId == player.GamerID);
      }
      if (!player.HasActionRequest(p, blockID))
      {
        this.networkManager.SendOpenBlockRequest(p, blockID, player.Gamer.ID);
        player.AddActionRequest(p, blockID);
      }
      else
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
      return (GameScreen) null;
    }

    public GameScreen OpenBlockConfirmation(
      Player player,
      GlobalPoint3D p,
      Block blockID,
      bool success)
    {
      GameScreen gameScreen = (GameScreen) null;
      if (success)
      {
        success = this.FlagBlockIsOpen(p, blockID, player.GamerID);
        if (success)
          gameScreen = this.OpenBlockConfirmationCore(player, p, blockID);
      }
      player.CloseActionRequest(p, blockID);
      if (!success && player.Gamer.IsLocal)
      {
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Another player has this block open.\nYou must wait for them to close it.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player);
        messageBoxScreenTm.FadeToBlack = 0.0f;
        messageBoxScreenTm.TransitionOnTime = TimeSpan.Zero;
        this.AddScreen((GameScreen) messageBoxScreenTm, player);
      }
      return gameScreen;
    }

    private GameScreen OpenBlockConfirmationCore(
      Player player,
      GlobalPoint3D p,
      Block blockID)
    {
      GameScreen screen = (GameScreen) null;
      if (player.Gamer.IsLocal)
      {
        screen = this.GetSpecialBlockScreen(player, p, blockID);
        if (screen != null)
        {
          this.OpenBlockAction(player, p, blockID);
          this.AddScreen(screen, player);
        }
      }
      return screen;
    }

    private void OpenBlockAction(Player player, GlobalPoint3D p, Block blockID)
    {
      if (blockID != Block.Chest)
        return;
      this.SetSwitch(p, true, UpdateBlockMethod.Player, player, true);
      this.AddLootXPIfTreasureChest(player, p);
    }

    private void AddLootXPIfTreasureChest(Player player, GlobalPoint3D p)
    {
      byte auxDataNoCache = this.map.GetAuxDataNoCache(p);
      if (((int) auxDataNoCache & 2) != 2)
        return;
      ChestBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as ChestBlock;
      if (dataBlock == null)
        return;
      for (int index = 0; index < dataBlock.Inventory.Count; ++index)
        player.SkillsData.LootGained(player, dataBlock.Inventory[index]);
      player.Raise_TreasureChestOpened(p, this.map.GetBlockData(p));
      this.map.SetAuxData(p, auxDataNoCache, (byte) ((uint) auxDataNoCache & 253U), UpdateBlockMethod.Strategy, GamerID.Sys1, true);
    }

    public void CloseSpecialBlockScreen(Player player, DataBlock block, bool blockRemoved)
    {
      this.CloseSpecialBlockScreen(Player.GetGamerID(player), block, blockRemoved);
    }

    public void CloseSpecialBlockScreen(GamerID gamerID, DataBlock block, bool blockRemoved)
    {
      this.FlagBlockIsClosed(gamerID, false);
      if (blockRemoved || block.ClassType != this.map.BlockData[(int) this.map.GetBlockID(block.Point)].ClassType)
      {
        this.MapStrategyTM.RemoveDataBlock(block);
        blockRemoved = true;
      }
      if (blockRemoved)
        this.networkManager.SendDataBlockRemove(block);
      else
        this.networkManager.SendDataBlockChange(block, true, UpdateBlockMethod.Player);
    }

    private GameScreen GetSpecialBlockScreen(
      Player player,
      GlobalPoint3D p,
      Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 114U)
      {
        if ((uint) block <= 65U)
        {
          switch (block)
          {
            case Block.Bookcase:
            case Block.Chest:
              break;
            case Block.Workbench:
              return (GameScreen) new WorkbenchScreen(this, player);
            case Block.Furnace:
              goto label_11;
            case Block.ItemShop:
            case Block.BlockShop:
              if (Globals2.UseOldMenu)
                return (GameScreen) new ShopScreen(this, player, blockID);
              return (GameScreen) new PauseMenuScreen2(this, player, (NewGuiMenu) new ShopMenu(this, player, p));
            default:
              goto label_28;
          }
        }
        else
        {
          if (block == Block.HealthBlock)
            return (GameScreen) new HealthBlockScreen(this, player, p);
          if (block == Block.AmbientSoundBlock)
            return (GameScreen) new AmbientSoundsScreen(this, player, p);
          goto label_28;
        }
      }
      else if ((uint) block <= 143U)
      {
        switch (block)
        {
          case Block.ParticleEmitter:
            return (GameScreen) new ParticleEmitterBlockScreen(this, player, p);
          case Block.LockedChest:
          case Block.Crate:
            break;
          case Block.LitFurnace:
            goto label_11;
          case Block.NPCSpawn:
            if (Globals2.UseOldMenu)
              return (GameScreen) new NPCSpawnScreen(this, player, p);
            return (GameScreen) new PauseMenuScreen2(this, player, (NewGuiMenu) new NPCSpawnBlockScreen(this, player, p));
          case Block.SentryTurret:
            return (GameScreen) new SentryTurretScreen(this, player, p);
          case Block.ProximityDetector:
            return (GameScreen) new ProximityDetectorBlockScreen(this, player, p);
          default:
            goto label_28;
        }
      }
      else
      {
        switch (block)
        {
          case Block.WifiTransmitter:
            return (GameScreen) new WifiTransmitterScreen(this, player, p);
          case Block.WifiReceiver:
            return (GameScreen) new WifiReceiverScreen(this, player, p);
          case Block.Safe:
            break;
          case Block.Sundial:
            return (GameScreen) new SundialBlockScreen(this, player, p);
          case Block.ScriptBlock:
            return (GameScreen) new ScriptBlockScreen(this, player, p);
          default:
            goto label_28;
        }
      }
      if (Globals2.UseOldMenu)
        return (GameScreen) new ChestScreen(this, player, p, blockID);
      return (GameScreen) new PauseMenuScreen2(this, player, (NewGuiMenu) new ChestMenu(this, player, p));
label_11:
      return (GameScreen) new FurnaceScreen(this, player, p);
label_28:
      return this.GetItemCustomSetupScreen(player, p, blockID);
    }

    public void OnOpenBlockScreenRemoved(object sender, EventArgs e)
    {
      if (this.CurrentOpenBlock != sender)
        return;
      this.CurrentOpenBlock.ScreenRemoved -= new EventHandler<EventArgs>(this.OnOpenBlockScreenRemoved);
      this.CurrentOpenBlock = (GameScreen) null;
    }

    public void AddItemCustomSetup(Item itemID, Permissions permission)
    {
      for (int index = 0; index < this.itemCustomSetup.Count; ++index)
      {
        GameInstance.ItemCustomSetup itemCustomSetup = this.itemCustomSetup[index];
        if (itemCustomSetup.ItemID == itemID)
        {
          itemCustomSetup.Permission = permission;
          this.itemCustomSetup[index] = itemCustomSetup;
          return;
        }
      }
      this.itemCustomSetup.Add(new GameInstance.ItemCustomSetup()
      {
        ItemID = itemID,
        Permission = permission
      });
    }

    public bool ItemHasCustomSetup(Item itemID, Player player)
    {
      foreach (GameInstance.ItemCustomSetup itemCustomSetup in this.itemCustomSetup)
      {
        if (itemCustomSetup.ItemID == itemID)
          return player.HasPermission(itemCustomSetup.Permission);
      }
      return false;
    }

    private GameScreen GetItemCustomSetupScreen(
      Player player,
      GlobalPoint3D p,
      Block blockID)
    {
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.PluginGUI != null)
        {
          NewGuiMenu customSetupScreen = activeMod.PluginGUI.GetItemCustomSetupScreen((ITMGame) this, (ITMPlayer) player, p, (Item) blockID);
          if (customSetupScreen != null)
            return (GameScreen) new PauseMenuScreen2(this, player, customSetupScreen);
        }
      }
      return (GameScreen) null;
    }

    public Item ConvertBlockIDToPickupItemID(Block blockID, byte auxData, bool isProspect)
    {
      Item itemId = ItemData.ConvertBlockIDToItemID((Item) blockID);
      if ((int) itemId != (int) blockID)
        return itemId;
      byte level = (byte) ((uint) auxData >> 4);
      switch (blockID)
      {
        case Block.Key:
          byte num = this.ConvertAuxToData(Block.Key, auxData);
          if ((int) num >= this.KeyList.Length)
            num = (byte) (this.KeyList.Length - 1);
          return this.KeyList[(int) num];
        case Block.RaresChest:
          if (!isProspect)
            return this.SelectRandomRare(level);
          return Item.RaresChest;
        case Block.Crop:
          return !isProspect ? Item.None : Item.Crop;
        default:
          return (Item) blockID;
      }
    }

    public List<InventoryItem> ConvertBlockIDToPickupItemsSecondary(
      Player player,
      Block blockID,
      byte auxData)
    {
      switch (blockID)
      {
        case Block.LongGrass:
        case Block.BerryBush:
          if (this.map.HasChanged(auxData))
            return (List<InventoryItem>) null;
          goto case Block.Crop;
        case Block.Crop:
          List<InventoryItem> items = new List<InventoryItem>();
          switch (blockID)
          {
            case Block.LongGrass:
              this.AddPickupsFromLongGrass(player, items);
              break;
            case Block.Crop:
              this.AddPickupsFromCrop(player, items, auxData);
              break;
            case Block.BerryBush:
              this.AddPickupsFromBerryBush(player, items);
              break;
          }
          return items;
        default:
          return (List<InventoryItem>) null;
      }
    }

    private void AddPickupsFromCrop(Player player, List<InventoryItem> items, byte auxData)
    {
      int max1 = (int) auxData & 7;
      int num = (int) auxData >> 4;
      int max2 = 6;
      Item itemID1 = Item.WheatSeed;
      Item itemID2 = Item.Wheat;
      switch (num)
      {
        case 1:
          itemID1 = Item.SugarCaneSeed;
          itemID2 = Item.Sugar;
          break;
        case 2:
          itemID1 = Item.TomatoSeed;
          itemID2 = Item.Tomato;
          break;
        case 3:
          itemID1 = Item.Potato;
          itemID2 = Item.Potato;
          break;
        case 4:
          itemID1 = Item.Corn;
          itemID2 = Item.Corn;
          break;
      }
      if (this.Random.Next(max2) < max1)
        items.Add(new InventoryItem(itemID1, this.Random.Next(1, 4)));
      if (max1 <= 1 || player == null || !player.Inventory.IsEquippedInHand(ItemSubType.HarvestTool))
        return;
      int count = this.Random.Next(0, max1);
      if (count <= 0)
        return;
      items.Add(new InventoryItem(itemID2, count));
    }

    private void AddPickupsFromLongGrass(Player player, List<InventoryItem> items)
    {
      if (!this.Random.RandomChance(0.2))
        return;
      int num = this.Random.Next(0, 13);
      if (num < 4)
        items.Add(new InventoryItem(Item.WheatSeed, 1));
      else if (num < 7)
        items.Add(new InventoryItem(Item.SugarCaneSeed, 1));
      else if (num < 10)
        items.Add(new InventoryItem(Item.TomatoSeed, 1));
      else if (num < 11)
        items.Add(new InventoryItem(Item.Potato, 1));
      else if (num < 12)
      {
        items.Add(new InventoryItem(Item.Corn, 1));
      }
      else
      {
        switch (this.Random.Next(0, 8))
        {
          case 0:
            items.Add(new InventoryItem(Item.Apple, 1));
            break;
          case 1:
            items.Add(new InventoryItem(Item.Orange, 1));
            break;
          case 2:
            items.Add(new InventoryItem(Item.Plum, 1));
            break;
          case 3:
            items.Add(new InventoryItem(Item.Olives, 1));
            break;
          case 4:
            items.Add(new InventoryItem(Item.Banana, 1));
            break;
          case 5:
            items.Add(new InventoryItem(Item.Lemon, 1));
            break;
          case 6:
            items.Add(new InventoryItem(Item.Lime, 1));
            break;
          case 7:
            items.Add(new InventoryItem(Item.Grapefruit, 1));
            break;
        }
      }
    }

    private void AddPickupsFromBerryBush(Player player, List<InventoryItem> items)
    {
      if (!this.Random.RandomChance(0.6))
        return;
      switch (this.Random.Next(0, 7))
      {
        case 0:
          items.Add(new InventoryItem(Item.Strawberries, 1));
          break;
        case 1:
          items.Add(new InventoryItem(Item.Blueberries, 1));
          break;
        case 2:
          items.Add(new InventoryItem(Item.Raspberries, 1));
          break;
        case 3:
          items.Add(new InventoryItem(Item.Gooseberries, 1));
          break;
        case 4:
          items.Add(new InventoryItem(Item.Cherries, 1));
          break;
        case 5:
          items.Add(new InventoryItem(Item.Grapes, 1));
          break;
        default:
          items.Add(new InventoryItem(Item.Blackberries, 1));
          break;
      }
    }

    public Block ConvertItemIDToBlockID(Item item)
    {
      return (Block) this.ConvertItemIDToBlockID(new InventoryItem(item)).ItemID;
    }

    public InventoryItem ConvertItemIDToBlockID(InventoryItem item)
    {
      Item idForTextureIndex = this.ConvertItemIDToBlockIDForTextureIndex(item.ItemID);
      if (idForTextureIndex != item.ItemID)
      {
        item.ItemID = idForTextureIndex;
        return item;
      }
      Item itemId = item.ItemID;
      if ((uint) itemId <= 295U)
      {
        switch (itemId)
        {
          case Item.RaresChest:
            item.Durability = (ushort) this.Random.Next(Globals2.MaxRareLevel + 1);
            goto label_11;
          case Item.BucketOfWater:
            item.ItemID = Item.Water;
            goto label_11;
          case Item.BucketOfLava:
            item.ItemID = Item.Lava;
            goto label_11;
        }
      }
      else
      {
        switch (itemId)
        {
          case Item.WheatSeed:
          case Item.SugarCaneSeed:
          case Item.TomatoSeed:
          case Item.Potato:
          case Item.Corn:
            item.ItemID = Item.Crop;
            goto label_11;
        }
      }
      int keyId = this.GetKeyID(item.ItemID);
      if (keyId >= 0)
      {
        item.Durability = (ushort) keyId;
        item.ItemID = Item.Key;
      }
label_11:
      return item;
    }

    public Item ConvertItemIDToBlockIDForTextureIndex(Item itemID)
    {
      switch (itemID)
      {
        case Item.WoodDoor:
          return Item.WoodDoorTop;
        case Item.SteelDoor:
          return Item.SteelDoorTop;
        case Item.Bed:
          return Item.BedFoot;
        case Item.LockedDoor:
          return Item.LockedDoorTop;
        default:
          return ItemData.ConvertItemIDToBlockID(itemID);
      }
    }

    public byte ConvertDataToAux(Block blockID, byte data)
    {
      switch (blockID)
      {
        case Block.Key:
        case Block.RaresChest:
          return (byte) ((((int) data & 248) << 1) + ((int) data & 7));
        default:
          return data;
      }
    }

    public byte ConvertAuxToData(Block blockID, byte aux)
    {
      switch (blockID)
      {
        case Block.Key:
        case Block.RaresChest:
          return (byte) (((int) aux & 7) + (((int) aux & 240) >> 1));
        default:
          return (byte) ((uint) aux & 247U);
      }
    }

    private void UpdateCharacterCollision()
    {
      for (int index1 = 0; index1 < this.actorList.Count; ++index1)
      {
        Actor actor1 = this.actorList[index1];
        if (actor1 != null && !actor1.IsDeadOrInactiveOrDisabled)
        {
          for (int index2 = index1 + 1; index2 < this.actorList.Count; ++index2)
          {
            Actor actor2 = this.actorList[index2];
            if (actor2 != null && !actor2.IsDeadOrInactiveOrDisabled)
            {
              Vector3 vector3;
              vector3.X = actor2.Position.X - actor1.Position.X;
              vector3.Y = actor2.Position.Y - actor1.Position.Y;
              vector3.Z = actor2.Position.Z - actor1.Position.Z;
              float num1 = vector3.LengthSquared();
              float num2 = actor1.Sphere.Radius + actor2.Sphere.Radius;
              if ((double) num1 > 0.0 && (double) num1 < (double) num2 * (double) num2)
              {
                float num3 = vector3.Length();
                float num4 = 1f / num3;
                float num5 = vector3.X * num4;
                float num6 = vector3.Y * num4;
                float num7 = vector3.Z * num4;
                float num8 = (float) (((double) num2 - (double) num3) * 0.5);
                Vector3 displacement1;
                displacement1.X = num5 * num8;
                displacement1.Y = num6 * num8;
                displacement1.Z = num7 * num8;
                Vector3 displacement2;
                displacement2.X = -displacement1.X;
                displacement2.Y = -displacement1.Y;
                displacement2.Z = -displacement1.Z;
                actor1.OnCollision(actor2, displacement2);
                actor2.OnCollision(actor1, displacement1);
              }
            }
          }
        }
      }
    }

    public HitTarget GetFirstHitTarget(
      BoundingBox box,
      HitTargetOptions options,
      bool localsOnly)
    {
      HitTarget hitTarget = new HitTarget();
      bool flag = (options & HitTargetOptions.CriticalHit) > HitTargetOptions.None;
      if ((options & HitTargetOptions.PlayersAndNpcs) == HitTargetOptions.Players)
      {
        foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
        {
          if (localEnabledPlayer != null && !localEnabledPlayer.IsDeadOrInactiveOrDisabled && box.Intersects(localEnabledPlayer.Box))
          {
            hitTarget.Target = (Actor) localEnabledPlayer;
            hitTarget.Distance = 0.0f;
            hitTarget.IsCriticalHit = flag && box.Intersects(localEnabledPlayer.CriticalHitBox);
            return hitTarget;
          }
        }
        if (!localsOnly)
        {
          foreach (Gamer remoteEnabledGamer in this.networkManager.RemoteEnabledGamers)
          {
            Player tag = remoteEnabledGamer.Tag as Player;
            if (tag != null && !tag.IsDeadOrInactiveOrDisabled && box.Intersects(tag.Box))
            {
              hitTarget.Target = (Actor) tag;
              hitTarget.Distance = 0.0f;
              hitTarget.IsCriticalHit = flag && box.Intersects(tag.CriticalHitBox);
              return hitTarget;
            }
          }
        }
      }
      else if ((options & HitTargetOptions.Npcs) == HitTargetOptions.Npcs)
      {
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor = this.actorList[index];
          if (actor != null && !actor.IsDeadOrInactiveOrDisabled && (!localsOnly || actor.IsLocalGamer) && box.Intersects(actor.Box))
          {
            hitTarget.Target = actor;
            hitTarget.Distance = 0.0f;
            hitTarget.IsCriticalHit = flag && box.Intersects(actor.CriticalHitBox);
            return hitTarget;
          }
        }
      }
      return hitTarget;
    }

    public HitTarget BuildHitTarget(
      Ray ray,
      Actor owner,
      HitTargetOptions options,
      List<ActorType> excludeTypes)
    {
      HitTarget result = new HitTarget();
      result.Distance = float.MaxValue;
      bool criticalHits = (options & HitTargetOptions.CriticalHit) > HitTargetOptions.None;
      bool flag1 = excludeTypes != null && excludeTypes.Count > 0;
      if ((options & HitTargetOptions.PlayersAndNpcs) == HitTargetOptions.Players)
      {
        for (int index = 0; index < this.centralPlayerList.Count; ++index)
        {
          Actor centralPlayer = (Actor) this.centralPlayerList[index];
          if (centralPlayer != null && centralPlayer != owner && !centralPlayer.IsDeadOrInactiveOrDisabled && (!flag1 || !excludeTypes.Contains(centralPlayer.ActorType)))
            this.CheckHitTargetCore(ray, centralPlayer, criticalHits, ref result);
        }
      }
      else if ((options & HitTargetOptions.Npcs) == HitTargetOptions.Npcs)
      {
        bool flag2 = (options & HitTargetOptions.Players) > HitTargetOptions.None;
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor = this.actorList[index];
          if (actor != null && actor != owner && !actor.IsDeadOrInactiveOrDisabled && ((flag2 || !actor.IsPlayer) && (!flag1 || !excludeTypes.Contains(actor.ActorType))))
            this.CheckHitTargetCore(ray, actor, criticalHits, ref result);
        }
      }
      return result;
    }

    private void CheckHitTargetCore(Ray ray, Actor c, bool criticalHits, ref HitTarget result)
    {
      float? nullable1 = ray.Intersects(c.Box);
      if (nullable1.HasValue)
      {
        float? nullable2 = nullable1;
        float distance = result.Distance;
        if (((double) nullable2.GetValueOrDefault() >= (double) distance ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
        {
          result.Target = c;
          result.Distance = nullable1.Value;
          result.IsCriticalHit = criticalHits && ray.Intersects(c.CriticalHitBox).HasValue;
          return;
        }
      }
      if (!criticalHits || nullable1.HasValue)
        return;
      nullable1 = ray.Intersects(c.CriticalHitBox);
      if (!nullable1.HasValue)
        return;
      float? nullable3 = nullable1;
      float distance1 = result.Distance;
      if (((double) nullable3.GetValueOrDefault() >= (double) distance1 ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
        return;
      result.Target = c;
      result.Distance = nullable1.Value;
      result.IsCriticalHit = true;
    }

    public HitTarget BuildHitTarget(
      BoundingBox box,
      Actor owner,
      HitTargetOptions options)
    {
      HitTarget result = new HitTarget();
      result.Distance = float.MaxValue;
      Vector3 position = owner.Position;
      bool criticalHits = (options & HitTargetOptions.CriticalHit) > HitTargetOptions.None;
      if ((options & HitTargetOptions.PlayersAndNpcs) == HitTargetOptions.Players)
      {
        for (int index = 0; index < this.centralPlayerList.Count; ++index)
        {
          Actor centralPlayer = (Actor) this.centralPlayerList[index];
          if (centralPlayer != null && centralPlayer != owner && !centralPlayer.IsDeadOrInactiveOrDisabled)
            this.CheckHitTargetCore(box, centralPlayer, position, criticalHits, ref result);
        }
      }
      else if ((options & HitTargetOptions.Npcs) == HitTargetOptions.Npcs)
      {
        bool flag = (options & HitTargetOptions.Players) > HitTargetOptions.None;
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor = this.actorList[index];
          if (actor != null && actor != owner && !actor.IsDeadOrInactiveOrDisabled && (flag || !actor.IsPlayer))
            this.CheckHitTargetCore(box, actor, position, criticalHits, ref result);
        }
      }
      return result;
    }

    private void CheckHitTargetCore(
      BoundingBox box,
      Actor c,
      Vector3 proximity,
      bool criticalHits,
      ref HitTarget result)
    {
      float num = Vector3.Distance(proximity, c.Position);
      if ((double) num < (double) result.Distance)
      {
        if (!box.Intersects(c.Box))
          return;
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = criticalHits && box.Intersects(c.CriticalHitBox);
      }
      else
      {
        if (!criticalHits || !box.Intersects(c.CriticalHitBox))
          return;
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = true;
      }
    }

    public HitTarget BuildHitTarget(
      BoundingFrustum frustum,
      Actor owner,
      HitTargetOptions options)
    {
      HitTarget result = new HitTarget();
      result.Distance = float.MaxValue;
      Vector3 position = owner.Position;
      bool criticalHits = (options & HitTargetOptions.CriticalHit) > HitTargetOptions.None;
      if ((options & HitTargetOptions.PlayersAndNpcs) == HitTargetOptions.Players)
      {
        for (int index = 0; index < this.centralPlayerList.Count; ++index)
        {
          Actor centralPlayer = (Actor) this.centralPlayerList[index];
          if (centralPlayer != null && centralPlayer != owner && !centralPlayer.IsDeadOrInactiveOrDisabled)
            this.CheckHitTargetCore(frustum, centralPlayer, position, criticalHits, ref result);
        }
      }
      else if ((options & HitTargetOptions.Npcs) == HitTargetOptions.Npcs)
      {
        bool flag = (options & HitTargetOptions.Players) > HitTargetOptions.None;
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor = this.actorList[index];
          if (actor != null && actor != owner && !actor.IsDeadOrInactiveOrDisabled && (flag || !actor.IsPlayer))
            this.CheckHitTargetCore(frustum, actor, position, criticalHits, ref result);
        }
      }
      return result;
    }

    private void CheckHitTargetCore(
      BoundingFrustum frustum,
      Actor c,
      Vector3 proximity,
      bool criticalHits,
      ref HitTarget result)
    {
      float num = Vector3.Distance(proximity, c.Position);
      if ((double) num < (double) result.Distance)
      {
        if (!frustum.Intersects(c.Box))
          return;
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = criticalHits && frustum.Intersects(c.CriticalHitBox);
      }
      else
      {
        if (!criticalHits || !frustum.Intersects(c.CriticalHitBox))
          return;
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = true;
      }
    }

    public HitTarget BuildHitTarget(
      BoundingSphere sphere,
      Actor owner,
      HitTargetOptions options)
    {
      HitTarget result = new HitTarget();
      result.Distance = float.MaxValue;
      Vector3 position = owner.Position;
      bool criticalHits = (options & HitTargetOptions.CriticalHit) > HitTargetOptions.None;
      if ((options & HitTargetOptions.PlayersAndNpcs) == HitTargetOptions.Players)
      {
        for (int index = 0; index < this.centralPlayerList.Count; ++index)
        {
          Actor centralPlayer = (Actor) this.centralPlayerList[index];
          if (centralPlayer != null && centralPlayer != owner && !centralPlayer.IsDeadOrInactiveOrDisabled)
            this.CheckHitTargetCore(sphere, centralPlayer, position, criticalHits, ref result);
        }
      }
      else if ((options & HitTargetOptions.Npcs) == HitTargetOptions.Npcs)
      {
        bool flag = (options & HitTargetOptions.Players) > HitTargetOptions.None;
        for (int index = 0; index < this.actorList.Count; ++index)
        {
          Actor actor = this.actorList[index];
          if (actor != null && actor != owner && !actor.IsDeadOrInactiveOrDisabled && (flag || !actor.IsPlayer))
            this.CheckHitTargetCore(sphere, actor, position, criticalHits, ref result);
        }
      }
      return result;
    }

    private void CheckHitTargetCore(
      BoundingSphere sphere,
      Actor c,
      Vector3 proximity,
      bool criticalHits,
      ref HitTarget result)
    {
      float num = Vector3.Distance(proximity, c.Position);
      if ((double) num >= (double) result.Distance)
        return;
      if (sphere.Intersects(c.Box))
      {
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = criticalHits && sphere.Intersects(c.CriticalHitBox);
      }
      else
      {
        if (!criticalHits || !sphere.Intersects(c.CriticalHitBox))
          return;
        result.Target = c;
        result.Distance = num;
        result.IsCriticalHit = true;
      }
    }

    public bool CheckPointsToIgnore(GlobalPoint3D p)
    {
      for (int index = 0; index < this.pointsForCollisionToIgnore.Count; ++index)
      {
        GameInstance.PointToIgnore pointToIgnore = this.pointsForCollisionToIgnore[index];
        if (pointToIgnore.Point == p)
        {
          pointToIgnore.Counter = 2;
          this.pointsForCollisionToIgnore[index] = pointToIgnore;
          return true;
        }
      }
      return false;
    }

    public void BroadcastSound(Vector3 origin, Actor broadcaster, SoundType soundType)
    {
      this.broadcastSounds.Add(new SoundBroadcast()
      {
        Origin = origin,
        Actor = (ITMActor) broadcaster,
        SoundType = soundType,
        Tick = Globals1.ElapsedWatch.ElapsedTicks
      });
    }

    public Ray GetCalcBlockTargetRay(Vector3 position, Vector3 dir)
    {
      return new Ray(position + dir * 0.2f, dir);
    }

    public HitTest CalcBlockTarget(Vector3 position, Vector3 dir, float range)
    {
      return this.CalcBlockTarget(position, dir, range, (List<byte>) null, false, true, false, false);
    }

    public HitTest CalcBlockTarget(
      Vector3 position,
      Vector3 dir,
      float range,
      List<byte> nonSwingTargets,
      bool solidBlocksOnly,
      bool ignoreIcons,
      bool isOnRope,
      bool checkPlayerLiquid)
    {
      if (solidBlocksOnly)
        ignoreIcons = true;
      HitTest hitTest = new HitTest();
      GlobalPoint3D globalPoint3D = GlobalPoint3D.Negate(this.map.MapBound.Min);
      BoxInt mapBound = this.map.MapBound;
      mapBound.Min += globalPoint3D;
      mapBound.Max += globalPoint3D;
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      Vector3 vector3_3 = new Vector3();
      Vector3 position1 = position;
      position1.X += (float) globalPoint3D.X;
      position1.Y += (float) globalPoint3D.Y;
      position1.Z += (float) globalPoint3D.Z;
      GlobalPoint3D point = this.map.GetPoint(position1);
      int x = point.X;
      int z = point.Z;
      int num1;
      int num2;
      if ((double) dir.X > 0.0)
      {
        num1 = 1;
        num2 = mapBound.Max.X;
        vector3_1.X = (float) (mapBound.Min.X + (point.X + 1));
      }
      else
      {
        num1 = -1;
        num2 = mapBound.Min.X - 1;
        vector3_1.X = (float) (mapBound.Min.X + point.X);
      }
      int num3;
      int num4;
      if ((double) dir.Y > 0.0)
      {
        num3 = 1;
        num4 = mapBound.Max.Y;
        vector3_1.Y = (float) (mapBound.Min.Y + (point.Y + 1));
      }
      else
      {
        num3 = -1;
        num4 = mapBound.Min.Y - 1;
        vector3_1.Y = (float) (mapBound.Min.Y + point.Y);
      }
      int num5;
      int num6;
      if ((double) dir.Z > 0.0)
      {
        num5 = 1;
        num6 = mapBound.Max.Z;
        vector3_1.Z = (float) (mapBound.Min.Z + (point.Z + 1));
      }
      else
      {
        num5 = -1;
        num6 = mapBound.Min.Z - 1;
        vector3_1.Z = (float) (mapBound.Min.Z + point.Z);
      }
      if ((double) dir.X != 0.0)
      {
        float num7 = 1f / dir.X;
        vector3_2.X = (vector3_1.X - position1.X) * num7;
        vector3_3.X = (float) num1 * num7;
      }
      else
        vector3_2.X = float.MaxValue;
      if ((double) dir.Y != 0.0)
      {
        float num7 = 1f / dir.Y;
        vector3_2.Y = (vector3_1.Y - position1.Y) * num7;
        vector3_3.Y = (float) num3 * num7;
      }
      else
        vector3_2.Y = float.MaxValue;
      if ((double) dir.Z != 0.0)
      {
        float num7 = 1f / dir.Z;
        vector3_2.Z = (vector3_1.Z - position1.Z) * num7;
        vector3_3.Z = (float) num5 * num7;
      }
      else
        vector3_2.Z = float.MaxValue;
      float num8 = range * range;
      int num9 = 0;
      while ((double) Vector3.DistanceSquared(position1, this.map.GetBlockCenter(point)) < (double) num8)
      {
        byte num7 = this.map.GetBlockID(point - globalPoint3D);
        if (num7 > (byte) 0)
        {
          if (nonSwingTargets != null && nonSwingTargets.Contains(num7))
          {
            num7 = (byte) 0;
          }
          else
          {
            if (solidBlocksOnly)
            {
              if (this.map.BlockData[(int) num7].Buffer > (byte) 1)
                num7 = (byte) 0;
            }
            else if (ignoreIcons && this.map.BlockData[(int) num7].IsIcon)
              num7 = (byte) 0;
            if (num7 > (byte) 0)
            {
              Block block = (Block) num7;
              if (isOnRope && block == Block.Rope && (point.X == x && point.Z == z))
                num7 = (byte) 0;
              else if (!checkPlayerLiquid && (block == Block.Water || block == Block.Lava))
                num7 = (byte) 0;
              else if (block == Block.Cloud && !this.map.IsNextTo(point - globalPoint3D, (byte) 0, -1, true, false))
                num7 = (byte) 0;
            }
          }
          if (num7 > (byte) 0)
          {
            Ray ray = new Ray();
            ray.Position.X = position1.X;
            ray.Position.Y = position1.Y;
            ray.Position.Z = position1.Z;
            ray.Direction.X = dir.X;
            ray.Direction.Y = dir.Y;
            ray.Direction.Z = dir.Z;
            BoundingBox blockBox = this.GetBlockBox(point - globalPoint3D);
            blockBox.Min.X += (float) globalPoint3D.X;
            blockBox.Min.Y += (float) globalPoint3D.Y;
            blockBox.Min.Z += (float) globalPoint3D.Z;
            blockBox.Max.X += (float) globalPoint3D.X;
            blockBox.Max.Y += (float) globalPoint3D.Y;
            blockBox.Max.Z += (float) globalPoint3D.Z;
            float? nullable = ray.Intersects(blockBox);
            if (nullable.HasValue)
            {
              hitTest.Distance = nullable.Value;
              hitTest.Point.X = point.X - globalPoint3D.X;
              hitTest.Point.Y = point.Y - globalPoint3D.Y;
              hitTest.Point.Z = point.Z - globalPoint3D.Z;
              hitTest.IsValid = true;
              break;
            }
          }
        }
        if ((double) vector3_2.X < (double) vector3_2.Y)
        {
          if ((double) vector3_2.X < (double) vector3_2.Z)
          {
            point.X += num1;
            if (point.X == num2)
              return hitTest;
            vector3_2.X += vector3_3.X;
          }
          else
          {
            point.Z += num5;
            if (point.Z == num6)
              return hitTest;
            vector3_2.Z += vector3_3.Z;
          }
        }
        else if ((double) vector3_2.Y < (double) vector3_2.Z)
        {
          point.Y += num3;
          if (point.Y == num4)
            return hitTest;
          vector3_2.Y += vector3_3.Y;
        }
        else
        {
          point.Z += num5;
          if (point.Z == num6)
            return hitTest;
          vector3_2.Z += vector3_3.Z;
        }
        ++num9;
      }
      return hitTest;
    }

    public BlockFace GetSwingFace(Ray ray, BoundingBox box)
    {
      BlockFace blockFace = BlockFace.ProxyDefault;
      float maxValue = float.MaxValue;
      float? nullable1 = ray.Intersects(this.GetBoxFace(box, BlockFace.Left));
      if (nullable1.HasValue && (double) nullable1.Value < (double) maxValue)
      {
        maxValue = nullable1.Value;
        blockFace = BlockFace.Left;
      }
      float? nullable2 = ray.Intersects(this.GetBoxFace(box, BlockFace.Forward));
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        maxValue = nullable2.Value;
        blockFace = BlockFace.Forward;
      }
      float? nullable3 = ray.Intersects(this.GetBoxFace(box, BlockFace.Right));
      if (nullable3.HasValue && (double) nullable3.Value < (double) maxValue)
      {
        maxValue = nullable3.Value;
        blockFace = BlockFace.Right;
      }
      float? nullable4 = ray.Intersects(this.GetBoxFace(box, BlockFace.Backward));
      if (nullable4.HasValue && (double) nullable4.Value < (double) maxValue)
      {
        maxValue = nullable4.Value;
        blockFace = BlockFace.Backward;
      }
      float? nullable5 = ray.Intersects(this.GetBoxFace(box, BlockFace.Up));
      if (nullable5.HasValue && (double) nullable5.Value < (double) maxValue)
      {
        maxValue = nullable5.Value;
        blockFace = BlockFace.Up;
      }
      float? nullable6 = ray.Intersects(this.GetBoxFace(box, BlockFace.Down));
      if (nullable6.HasValue && (double) nullable6.Value < (double) maxValue)
      {
        float num = nullable6.Value;
        blockFace = BlockFace.Down;
      }
      return blockFace;
    }

    public BoundingBox GetBoxFace(BoundingBox box, BlockFace face)
    {
      BoundingBox boundingBox = new BoundingBox();
      switch (face)
      {
        case BlockFace.Left:
          boundingBox.Min = new Vector3(box.Min.X, box.Min.Y, box.Min.Z);
          boundingBox.Max = new Vector3(box.Min.X, box.Max.Y, box.Max.Z);
          break;
        case BlockFace.Forward:
          boundingBox.Min = new Vector3(box.Min.X, box.Min.Y, box.Min.Z);
          boundingBox.Max = new Vector3(box.Max.X, box.Max.Y, box.Min.Z);
          break;
        case BlockFace.Right:
          boundingBox.Min = new Vector3(box.Max.X, box.Min.Y, box.Min.Z);
          boundingBox.Max = new Vector3(box.Max.X, box.Max.Y, box.Max.Z);
          break;
        case BlockFace.Backward:
          boundingBox.Min = new Vector3(box.Min.X, box.Min.Y, box.Max.Z);
          boundingBox.Max = new Vector3(box.Max.X, box.Max.Y, box.Max.Z);
          break;
        case BlockFace.Up:
          boundingBox.Min = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
          boundingBox.Max = new Vector3(box.Max.X, box.Max.Y, box.Max.Z);
          break;
        case BlockFace.Down:
          boundingBox.Min = new Vector3(box.Min.X, box.Min.Y, box.Min.Z);
          boundingBox.Max = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
          break;
      }
      return boundingBox;
    }

    protected override void DrawCore(DrawState state)
    {
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      if (this.MapRenderer.IsEnabled)
        this.MapRenderer.Draw(player, virtualPlayer);
      foreach (Mod activePlugin in ModManager.ActivePlugins)
        activePlugin.Plugin.Draw((ITMPlayer) player, (ITMPlayer) virtualPlayer);
      if (this.hudRenderer.IsEnabled && player.CurrentArcadeMachine == null)
      {
        this.hudRenderer.Draw(player, virtualPlayer);
        if (!player.IsAssemblingPhoto)
          this.hotBarRenderer.Draw(player, virtualPlayer);
        this.dialogRenderer.Draw(player, virtualPlayer);
      }
      if (!virtualPlayer.DisplayPleaseWaitMessage)
        return;
      CoreGlobals.SpriteBatch.Begin();
      int num = player.Viewport.Height / 2 - 45;
      CoreGlobals.SpriteBatch.DrawStringCentered(CoreGlobals.GameFont, "Loading. Please wait...", (float) num, Color.Black, 1f);
      CoreGlobals.SpriteBatch.DrawStringCentered(CoreGlobals.GameFont, "Loading. Please wait...", (float) (num - 2), Color.White, 1f);
      CoreGlobals.SpriteBatch.End();
    }

    public void PostDraw()
    {
      this.AddScreens();
      if (!this.IsEnabledField || !this.isGamePaused)
        return;
      if (this.IsSinglePlayer)
      {
        if (StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager.GetScreenCount(this.controllingPlayer) != 0)
          return;
        this.isGamePaused = false;
        Globals1.ElapsedWatch.Start();
      }
      else
        this.isGamePaused = false;
    }

    public void AddMiningParticles(GlobalPoint3D point, Block blockID)
    {
      this.AddMiningParticles(point, blockID, (byte) 0);
    }

    public void AddMiningParticles(GlobalPoint3D point, Block blockID, byte auxData)
    {
      this.AddMiningParticles(point, blockID, auxData, this.map.IsBlockIcon((byte) blockID) ? 12 : 18, 0.13f, 0.02f, 1f, 1.5f);
    }

    public void AddMiningParticles(
      GlobalPoint3D point,
      Block blockID,
      byte auxData,
      int count,
      float radiusBase,
      float radiusRandom,
      float velFac,
      float age)
    {
      Item itemID = (Item) blockID;
      Vector3 blockCenter = this.map.GetBlockCenter(point);
      Vector3 zero = Vector3.Zero;
      if (blockID != Block.Painting && blockID != Block.StainedGlass && (blockID != Block.StainedGlassPane && blockID != Block.ArcadeMachine) && this.Map.GetBlockTextureIndex(blockID) >= 0)
        itemID = (Item) this.Map.GetBlockTextureIDForDrawing(blockID, (int) auxData >> 4);
      InventoryItem inventoryItem = new InventoryItem(itemID, 1);
      for (int index = 0; index < count; ++index)
      {
        zero.X = blockCenter.X + (float) ((this.Random.NextDouble() - 0.5) * (double) this.map.TileSize * 0.5);
        zero.Y = blockCenter.Y + (float) ((this.Random.NextDouble() - 0.5) * (double) this.map.TileSize * 0.5);
        zero.Z = blockCenter.Z + (float) ((this.Random.NextDouble() - 0.5) * (double) this.map.TileSize * 0.5);
        if (this.map.IsPassable(zero))
        {
          Vector3 velocity = this.GetMiningParticleVelocity(point, BlockFace.Left) * velFac;
          this.particleManager.AddNew(ParticleType.Debris, age, zero, velocity, radiusBase + (float) this.Random.NextDouble() * radiusRandom, inventoryItem, this.ParticleModifiers.MiningParticleModifier, 0.0f, -1, (byte) 0, GamerID.Sys1, false, true);
        }
      }
    }

    public void AddMiningParticles(
      Vector3 pos,
      Block blockID,
      int count,
      float radiusBase,
      float radiusRandom,
      float velFac,
      float age)
    {
      Vector3 zero = Vector3.Zero;
      InventoryItem inventoryItem = new InventoryItem((Item) blockID, 1);
      for (int index = 0; index < count; ++index)
      {
        zero.X = (float) (this.Random.NextDouble() - 0.5) * 0.05f * velFac;
        zero.Z = (float) (this.Random.NextDouble() - 0.5) * 0.05f * velFac;
        zero.Y = (float) this.Random.NextDouble() * 0.075f * velFac;
        this.particleManager.AddNew(ParticleType.Debris, age, pos, zero, radiusBase + (float) this.Random.NextDouble() * radiusRandom, inventoryItem, this.ParticleModifiers.MiningParticleModifier, 0.0f, -1, (byte) 0, GamerID.Sys1, false, true);
      }
    }

    public void AddMiningParticle(GlobalPoint3D point)
    {
      Block blockID = (Block) this.map.GetBlockID(point);
      if (blockID != Block.Painting && this.Map.GetBlockTextureIndex(blockID) >= 0)
        blockID = this.Map.GetBlockTextureIDForDrawing(blockID, (int) this.Map.GetAuxHighDataNoCache(point));
      if (blockID == Block.Wisdom || blockID == Block.Blueprint)
        return;
      this.AddMiningParticle(point, blockID, BlockFace.Up);
      this.AddMiningParticle(point, blockID, BlockFace.Left);
      this.AddMiningParticle(point, blockID, BlockFace.Forward);
      this.AddMiningParticle(point, blockID, BlockFace.Right);
      this.AddMiningParticle(point, blockID, BlockFace.Backward);
      this.AddMiningParticle(point, blockID, BlockFace.Down);
    }

    public void AddMiningParticle(GlobalPoint3D point, Block blockID, BlockFace face)
    {
      Vector3 particlePosition = this.GetMiningParticlePosition(point, face);
      if (!this.map.IsPassable(particlePosition))
        return;
      Vector3 particleVelocity = this.GetMiningParticleVelocity(point, face);
      this.particleManager.AddNew(ParticleType.Debris, 1.5f, particlePosition, particleVelocity, 0.11f, new InventoryItem((Item) blockID, 1), this.ParticleModifiers.MiningParticleModifier, 0.0f, -1, (byte) 0, GamerID.Sys1, false, true);
    }

    private Vector3 GetMiningParticlePosition(GlobalPoint3D point, BlockFace face)
    {
      Vector3 zero = Vector3.Zero;
      float num1 = this.map.TileSize * 0.6f;
      float num2 = 0.5f;
      switch (face)
      {
        case BlockFace.Left:
          zero.X = -num1;
          zero.Y = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Z = (float) (this.Random.NextDouble() - 0.5) * num2;
          break;
        case BlockFace.Forward:
          zero.X = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Y = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Z = -num1;
          break;
        case BlockFace.Right:
          zero.X = num1;
          zero.Y = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Z = (float) (this.Random.NextDouble() - 0.5) * num2;
          break;
        case BlockFace.Backward:
          zero.X = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Y = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Z = num1;
          break;
        case BlockFace.Up:
          zero.X = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Y = num1;
          zero.Z = (float) (this.Random.NextDouble() - 0.5) * num2;
          break;
        case BlockFace.Down:
          zero.X = (float) (this.Random.NextDouble() - 0.5) * num2;
          zero.Y = -num1;
          zero.Z = (float) (this.Random.NextDouble() - 0.5) * num2;
          break;
      }
      return zero + this.map.GetBlockCenter(point);
    }

    private Vector3 GetMiningParticleVelocity(GlobalPoint3D point, BlockFace face)
    {
      Vector3 vector3 = new Vector3();
      float num1 = 3f;
      float num2 = 0.6f;
      float num3 = 4.5f;
      switch (face)
      {
        case BlockFace.Left:
          vector3.X = (float) this.Random.NextDouble() * -num2;
          vector3.Y = (float) this.Random.NextDouble() * num3;
          vector3.Z = (float) (this.Random.NextDouble() - 0.5) * num1;
          break;
        case BlockFace.Forward:
          vector3.X = (float) (this.Random.NextDouble() - 0.5) * num1;
          vector3.Y = (float) this.Random.NextDouble() * num3;
          vector3.Z = (float) this.Random.NextDouble() * -num2;
          break;
        case BlockFace.Right:
          vector3.X = (float) this.Random.NextDouble() * num2;
          vector3.Y = (float) this.Random.NextDouble() * num3;
          vector3.Z = (float) (this.Random.NextDouble() - 0.5) * num1;
          break;
        case BlockFace.Backward:
          vector3.X = (float) (this.Random.NextDouble() - 0.5) * num1;
          vector3.Y = (float) this.Random.NextDouble() * num3;
          vector3.Z = (float) this.Random.NextDouble() * num2;
          break;
        case BlockFace.Up:
          vector3.X = (float) (this.Random.NextDouble() - 0.5) * num1;
          vector3.Y = (float) this.Random.NextDouble() * num3;
          vector3.Z = (float) (this.Random.NextDouble() - 0.5) * num1;
          break;
        case BlockFace.Down:
          vector3.X = (float) (this.Random.NextDouble() - 0.5) * num1;
          vector3.Y = (float) this.Random.NextDouble() * (-1f / 1000f);
          vector3.Z = (float) (this.Random.NextDouble() - 0.5) * num1;
          break;
      }
      return vector3;
    }

    public void ClearAllParticles(bool transmit)
    {
      this.particleManager.ClearAll();
      if (!transmit)
        return;
      this.networkManager.SendCommand(NetworkCommand.ClearParticles);
    }

    public void AddProjectile(
      Item itemID,
      Vector3 position,
      Vector3 velocity,
      GamerID playerID,
      bool cameFromRemote,
      bool transmit)
    {
      if (itemID == Item.Grenade)
      {
        this.particleManager.AddNew(ParticleType.None, 10f, position, velocity, 0.2f, new InventoryItem(itemID, 1), this.ParticleModifiers.GrenadeParticleModifier, 0.0f, -1, (byte) 0, playerID, cameFromRemote, true);
        Sounds.PlaySound(Item.GrenadeLauncher, ItemSoundType.Use, position, (ITMActor) null);
      }
      else
      {
        this.particleManager.AddNew(ParticleType.Projectile, 10f, position, velocity, 0.4f, new InventoryItem(itemID, 1), this.ParticleModifiers.ProjectileParticleModifier, 0.0f, -1, (byte) 0, playerID, cameFromRemote, true);
        Sounds.PlaySound(itemID, ItemSoundType.Use, position, (ITMActor) null);
      }
      if (!transmit)
        return;
      this.networkManager.SendProjectile(position, velocity, itemID, playerID, cameFromRemote);
    }

    public void LoadAvatar(ActorType actorType, OnAvatarLoaded callback)
    {
      if (this.SystemVoxelModelManager == null)
        return;
      MapModel model = this.SystemVoxelModelManager.LoadComponent("System Avatars", actorType.ToString(), true);
      MapModel crouch = this.SystemVoxelModelManager.LoadComponent("System Avatars", actorType.ToString() + " Crouch", true);
      callback(model, crouch);
      this.UnloadUnusedAvatars();
    }

    public void UnloadUnusedAvatars()
    {
      List<MapModel> mapModelList = new List<MapModel>();
      for (int index = 0; index < this.centralPlayerList.Count; ++index)
        this.centralPlayerList[index].AddModels(mapModelList);
      NpcManager npcManager = this.npcManager;
      this.SystemVoxelModelManager.UnloadComponents(mapModelList, ModelFlags.IsPlayer, ModelFlags.IsNPC);
    }

    public bool ShouldAddPickUp(Player player, Block blockID)
    {
      if (!Globals1.ItemData[(int) blockID].IsEnabled || player != null && (blockID == Block.Water || blockID == Block.Lava) && player.Inventory.IsEquippedInHand(Item.Bucket))
        return false;
      Block block = blockID;
      if ((uint) block <= 121U)
      {
        switch (block)
        {
          case Block.Wisdom:
          case Block.Blueprint:
          case Block.WoodDoorBottom:
          case Block.SteelDoorBottom:
          case Block.Fire:
          case Block.Book:
            break;
          default:
            goto label_7;
        }
      }
      else
      {
        switch (block)
        {
          case Block.BedFoot:
          case Block.LockedDoorBottom:
            break;
          case Block.NPCSpawn:
            return this.IsItemUnlocked(Item.NPCSpawn);
          default:
            goto label_7;
        }
      }
      return false;
label_7:
      return true;
    }

    public bool AddPickup(ref BlockClearedEventArgs e)
    {
      bool flag1 = false;
      if (this.IsFiniteResources || e.IgnoreFiniteModePickupRestriction)
      {
        Item pickupItemId = this.ConvertBlockIDToPickupItemID((Block) e.BlockData.BlockID, e.BlockData.AuxData, false);
        if (pickupItemId > Item.None && pickupItemId < (Item) Globals1.ItemData.Length)
        {
          InventoryItem inventoryItem = new InventoryItem(pickupItemId, 1);
          switch (pickupItemId)
          {
            case Item.Wisdom:
              WisdomScrollBlock dataBlock1 = this.MapStrategyTM.GetDataBlock(e.Point) as WisdomScrollBlock;
              if (dataBlock1 != null)
              {
                inventoryItem.Durability = dataBlock1.Index;
                break;
              }
              break;
            case Item.Blueprint:
              int index = -1;
              BlueprintBlock dataBlock2 = this.MapStrategyTM.GetDataBlock(e.Point) as BlueprintBlock;
              if (dataBlock2 != null)
              {
                index = (int) Blueprints.GetBlueprintIndex(dataBlock2.ID);
                if (Blueprints.BlueprintList[index].IsEnabled)
                  index = -1;
              }
              if (index < 0)
                index = this.GetMissingBlueprintIndex(e.Point);
              if (index >= 0)
              {
                inventoryItem.Durability = (ushort) index;
                Blueprints.BlueprintList[index].IsUnearthed = true;
                break;
              }
              break;
            case Item.Book:
              BookBlock dataBlock3 = this.MapStrategyTM.GetDataBlock(e.Point) as BookBlock;
              if (dataBlock3 != null)
              {
                inventoryItem.Durability = dataBlock3.ID;
                break;
              }
              break;
            default:
              switch ((Block) e.BlockData.BlockID)
              {
                case Block.Stack:
                case Block.UpsideDownStack:
                case Block.SnowLayer:
                case Block.Stack2:
                  inventoryItem.Count = ((int) e.BlockData.AuxData & 7) + 1;
                  break;
              }
              if (this.map.BlockData[(int) e.BlockData.BlockID].IsOreDeposit && !this.map.HasChanged(e.BlockData.AuxData))
              {
                Player player = this.GetPlayer(e.PlayerID);
                if (player != null && player.IsItemEquippedAndUsable(Item.NecklaceOfHypocrisy))
                {
                  inventoryItem.Count = this.Random.RandomChance(0.800000011920929) ? 2 : 0;
                  player.OnItemUsed(player.Inventory.GetEquipSlotID(Item.NecklaceOfHypocrisy));
                  break;
                }
                break;
              }
              break;
          }
          if (inventoryItem.Count > 0)
            flag1 = this.AddPickup(ParticleType.None, e.Point, inventoryItem, Vector2.Zero, 0.0f, e.Method, e.PlayerID);
        }
        if (e.Method == UpdateBlockMethod.Player || e.Method == UpdateBlockMethod.PlayerRelated)
        {
          List<InventoryItem> pickupItemsSecondary = this.ConvertBlockIDToPickupItemsSecondary(this.GetPlayer(e.PlayerID), (Block) e.BlockData.BlockID, e.BlockData.AuxData);
          if (pickupItemsSecondary != null && pickupItemsSecondary.Count > 0)
          {
            foreach (InventoryItem inventoryItem in pickupItemsSecondary)
            {
              bool flag2 = this.AddPickup(ParticleType.None, e.Point, inventoryItem, Vector2.Zero, 0.0f, e.Method, e.PlayerID);
              if (!flag1)
                flag1 = flag2;
            }
          }
        }
      }
      return flag1;
    }

    public void AddDamageParticles(Vector3 pos, float damage, DamageType damageType)
    {
      if ((double) damage <= 1.0 && this.Random.Next(6) != 0 || this.EmitterParticleSystem == null)
        return;
      float num1 = 6f;
      float num2 = 6.6f;
      int num3 = (int) (((double) damage + 5.0) / 5.0) * 3;
      if (num3 > 20)
        num3 = 20;
      ParticleData data = new ParticleData();
      data.Duration = (ushort) 600;
      data.Gravity = (short) 400;
      float num4 = damageType == DamageType.Burning ? 0.1f : 0.04f;
      data.Size = new Vector4(num4, num4, num4, 1.2f);
      data.StartColor = data.EndColor = damageType == DamageType.Burning ? Color.LightGray : Color.DarkRed;
      for (int index = 0; index < num3; ++index)
      {
        data.Velocity.X = (float) (this.Random.NextDouble() - 0.5) * num1;
        data.Velocity.Y = (float) this.Random.NextDouble() * num2;
        data.Velocity.Z = (float) (this.Random.NextDouble() - 0.5) * num1;
        this.EmitterParticleSystem.AddParticle(pos, ref data);
      }
    }

    public bool AddPickup(
      ParticleType type,
      GlobalPoint3D p,
      InventoryItem item,
      Vector2 velocity,
      float minPickupAge,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      float age = this.GetAge(item.ItemID, method);
      return this.AddPickup(type, p, item, velocity, age, minPickupAge, playerID, true);
    }

    public bool AddPickup(
      ParticleType type,
      GlobalPoint3D p,
      InventoryItem item,
      Vector2 velocity,
      float age,
      float minPickupAge,
      GamerID playerID,
      bool tryMerge)
    {
      if (item.ItemID <= Item.None)
        return false;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.Y += this.map.TileSize * 0.25f;
      velocity.X += (float) (this.Random.NextDouble() * 5.0 - 2.5);
      velocity.Y += (float) (this.Random.NextDouble() * 5.0 - 2.5);
      Vector3 vector3 = new Vector3(velocity.X, 0.0f, velocity.Y);
      if (this.IsHost)
        return this.AddPickupFromClient(type, age, blockCenter, vector3, 0.24f, item, minPickupAge, playerID, tryMerge);
      this.networkManager.SendPickupCreateToHost(type, age, blockCenter, vector3, 0.24f, item, minPickupAge, playerID);
      return true;
    }

    public void AddPickupFromHost(
      ParticleType type,
      float age,
      Vector3 pos,
      Vector3 vel,
      float radius,
      InventoryItem item,
      float minPickupAge,
      int particleID,
      GamerID playerID)
    {
      if (item.ItemID == Item.Blueprint)
      {
        int durability = (int) item.Durability;
        if (durability >= 0 && durability < Blueprints.BlueprintList.Length)
          Blueprints.BlueprintList[durability].IsUnearthed = true;
      }
      this.particleManager.AddNew(type, age, pos, vel, radius, item, this.GetPickupParticleModifier(item), minPickupAge, particleID, (byte) 0, playerID, true, false);
    }

    public bool AddPickupFromClient(
      ParticleType type,
      float age,
      Vector3 pos,
      Vector3 vel,
      float radius,
      InventoryItem item,
      float minPickupAge,
      GamerID playerID,
      bool tryMerge)
    {
      if (ItemData.IsEnabled(item.ItemID))
      {
        if (!this.IsMultiplayer && tryMerge && this.TryToMergeParticle(type, item, pos, age, playerID))
          return true;
        int particleID = this.particleManager.AddNew(type, age, pos, vel, radius, item, this.GetPickupParticleModifier(item), minPickupAge, 0, (byte) 0, playerID, true, false);
        if (particleID >= 0)
        {
          this.networkManager.SendPickupCreateToClients(type, age, pos, vel, 0.24f, item, minPickupAge, particleID, playerID);
          return true;
        }
      }
      return false;
    }

    private ItemParticleModifier GetPickupParticleModifier(InventoryItem item)
    {
      if (item.ItemID == Item.Blueprint)
        return this.ParticleModifiers.BlueprintPickupParticleModifier;
      return this.ParticleModifiers.BlockPickupParticleModifier;
    }

    public bool ConfirmPickup(int particleID)
    {
      return this.particleManager.GetParticleFromID(particleID).HasValue;
    }

    public void FinalizePickup(GamerID gamerID, int particleID)
    {
      ItemParticle? particleFromId = this.particleManager.GetParticleFromID(particleID);
      if (!particleFromId.HasValue)
        return;
      Player localPlayer = this.GetLocalPlayer(gamerID);
      if (localPlayer != null)
        localPlayer.PickupItemCore(particleFromId.Value.Item, particleID);
      else
        this.ProcessPickup(particleFromId.Value.Item, (Player) null);
      this.particleManager.Deactivate(particleFromId.Value);
    }

    private bool TryToMergeParticle(
      ParticleType type,
      InventoryItem item,
      Vector3 pos,
      float age,
      GamerID playerID)
    {
      if (item.MaxDurability == (ushort) 0)
      {
        ItemParticle? closeParticle = this.particleManager.GetCloseParticle(type, pos, 2.2f, item.ItemID, this.GetPickupParticleModifier(item));
        if (closeParticle.HasValue)
        {
          ItemParticle particle = closeParticle.Value;
          if (particle.Item.Count + item.Count <= ItemData.GetStackSize(item.ItemID) && particle.PlayerID == playerID)
          {
            particle.Item.Count += item.Count;
            particle.Age = age;
            particle.Radius += 1f / 500f * (float) item.Count;
            if ((double) particle.Radius > 0.400000005960464)
              particle.Radius = 0.4f;
            this.particleManager.SetParticle(particle);
            if (this.IsHost)
              this.networkManager.SendPickupCreateToClients(type, age, particle.Position, particle.Velocity, 0.2f, item, particle.MinPickupAge, particle.ParticleID, particle.PlayerID);
            return true;
          }
        }
      }
      return false;
    }

    private float GetAge(Item item, UpdateBlockMethod method)
    {
      if (item == Item.Clipboard)
        return 3f;
      if (this.IsInDestructable(item))
        return float.MaxValue;
      if (!this.IsFiniteResources)
        return 10f;
      switch (method)
      {
        case UpdateBlockMethod.DropTimeShort:
          return 60f;
        case UpdateBlockMethod.DropTimeLong:
          return 420f;
        case UpdateBlockMethod.DropTimeDeath:
          return 420f;
        default:
          return 45f;
      }
    }

    public bool IsInDestructable(Item item)
    {
      switch (item)
      {
        case Item.Wisdom:
        case Item.Blueprint:
          return true;
        default:
          return false;
      }
    }

    public bool DropItem(
      ParticleType type,
      GlobalPoint3D p,
      InventoryItem item,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      return this.DropItem(type, p, item, Vector2.Zero, method, playerID);
    }

    public bool DropItem(
      ParticleType type,
      GlobalPoint3D p,
      InventoryItem item,
      Vector2 velocity,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      return this.DropItem(type, p, item, velocity, 3f, method, playerID);
    }

    public bool DropItem(
      ParticleType type,
      GlobalPoint3D p,
      InventoryItem item,
      Vector2 velocity,
      float minPickupAge,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      return this.AddPickup(type, p, item, velocity, minPickupAge, method, playerID);
    }

    public void ProcessPickup(InventoryItem item, Player player)
    {
      switch (item.ItemID)
      {
        case Item.Wisdom:
          this.PickupWisdom(ref item, player);
          break;
        case Item.Blueprint:
          this.PickupBlueprint(ref item, player);
          break;
        case Item.Book:
          this.UnlockItem(player, item.ItemID, false);
          this.PickupBook(ref item, player);
          break;
        default:
          this.UnlockItem(player, item.ItemID, false);
          break;
      }
    }

    private int GetMissingBlueprintIndex(GlobalPoint3D p)
    {
      this.RepairBlueprints();
      BlueprintBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as BlueprintBlock;
      if (dataBlock != null)
        return (int) dataBlock.ID;
      int num1 = this.map.MapHeight - p.Y;
      for (int index = this.BlueprintsToPlace.Count - 1; index >= 0; --index)
      {
        Blueprint blueprint = this.BlueprintsToPlace[index];
        float num2 = blueprint.Depth.X * (float) this.map.MapHeight;
        float num3 = blueprint.Depth.Y * (float) this.map.MapHeight;
        if ((double) num1 >= (double) num2 && (double) num1 <= (double) num3)
        {
          this.BlueprintsToPlace.RemoveAt(index);
          return (int) blueprint.ID;
        }
      }
      Blueprint blueprint1 = (Blueprint) null;
      float num4 = float.MaxValue;
      foreach (Blueprint blueprint2 in this.BlueprintsToPlace)
      {
        if ((double) blueprint2.Depth.X < (double) num4)
        {
          num4 = blueprint2.Depth.X;
          blueprint1 = blueprint2;
        }
      }
      if (blueprint1 == null)
        return -1;
      this.BlueprintsToPlace.Remove(blueprint1);
      return (int) blueprint1.ID;
    }

    private void RepairBlueprints()
    {
      Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
      bool[] flagArray = new bool[Blueprints.BlueprintList.Length];
      for (int index = 0; index < flagArray.Length; ++index)
        flagArray[index] = Blueprints.BlueprintList[index].IsEnabled;
      List<long> longList = new List<long>();
      foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
      {
        if (keyValuePair.Value.ClassType == DataBlockType.Blueprint)
        {
          if (this.map.GetBlockIDAndAuxFromPending(keyValuePair.Value.Point).BlockID == (byte) 57)
          {
            short blueprintIndex = Blueprints.GetBlueprintIndex(((BlueprintBlock) keyValuePair.Value).ID);
            if (blueprintIndex >= (short) 0)
              flagArray[(int) blueprintIndex] = true;
          }
          else
            longList.Add(keyValuePair.Key);
        }
      }
      foreach (long key in longList)
        dataBlocks.Remove(key);
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        Blueprint blueprint = Blueprints.BlueprintList[index];
        if (!flagArray[index] && blueprint.Point.Y > 0)
        {
          this.MapStrategyTM.AddDataBlock((DataBlock) new BlueprintBlock(blueprint.Point)
          {
            ID = blueprint.ID
          }, UpdateBlockMethod.Generation);
          this.map.GetChunk(blueprint.Point)?.SetChunkFlag(ChunkFlags.HasSpecialBlocks);
        }
      }
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        if (!flagArray[index])
        {
          Blueprint blueprint = Blueprints.BlueprintList[index];
          blueprint.IsGenerated = blueprint.IsUnearthed = blueprint.IsEnabled = false;
        }
      }
    }

    private void RepairWisdomScrolls()
    {
      Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
      bool[] flagArray = new bool[Wisdom.WisdomList.Length];
      for (int index = 0; index < flagArray.Length; ++index)
        flagArray[index] = Wisdom.WisdomList[index].IsEnabled;
      List<long> longList = new List<long>();
      foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
      {
        if (keyValuePair.Value.ClassType == DataBlockType.WisdomScroll)
        {
          if (this.map.GetBlockIDAndAuxFromPending(keyValuePair.Value.Point).BlockID == (byte) 56)
          {
            ushort index = ((WisdomScrollBlock) keyValuePair.Value).Index;
            if (index >= (ushort) 0)
              flagArray[(int) index] = true;
          }
          else
            longList.Add(keyValuePair.Key);
        }
      }
      foreach (long key in longList)
        dataBlocks.Remove(key);
      for (int index = 0; index < Wisdom.WisdomList.Length; ++index)
      {
        WisdomItem wisdom = Wisdom.WisdomList[index];
        if (!flagArray[index] && wisdom.Point.Y > 0)
        {
          this.MapStrategyTM.AddDataBlock((DataBlock) new WisdomScrollBlock(wisdom.Point)
          {
            Index = (ushort) index
          }, UpdateBlockMethod.Generation);
          this.map.GetChunk(wisdom.Point)?.SetChunkFlag(ChunkFlags.HasSpecialBlocks);
        }
      }
      for (int index = 0; index < Wisdom.WisdomList.Length; ++index)
      {
        if (!flagArray[index])
        {
          WisdomItem wisdom = Wisdom.WisdomList[index];
          wisdom.IsGenerated = wisdom.IsEnabled = false;
        }
      }
    }

    private void PickupBlueprint(ref InventoryItem item, Player player)
    {
      int durability = (int) item.Durability;
      if (durability < 0 || durability >= Blueprints.BlueprintList.Length)
        return;
      Blueprint blueprint = Blueprints.BlueprintList[durability];
      if (!blueprint.IsValid || blueprint.IsEnabled)
        return;
      blueprint.IsEnabled = true;
      this.UnlockItem(player, blueprint.Result.ItemID, false);
      if (player == null)
        return;
      this.AddNotification(player, " has found the " + ItemData.ToString(blueprint.Result.ItemID) + " blueprint", NotifyRecipient.Remote);
      this.AddScreen((GameScreen) new BlueprintPickupScreen(player, durability, 0.1), player);
      player.Raise_FindBlueprint(durability);
    }

    private void PickupWisdom(ref InventoryItem item, Player player)
    {
      int durability = (int) item.Durability;
      if (durability < 0 || durability >= Wisdom.WisdomList.Length || Wisdom.WisdomList[durability].IsEnabled)
        return;
      Wisdom.WisdomList[durability].IsEnabled = true;
      if (player == null)
        return;
      this.AddNotification(player, " has found a wisdom scroll", NotifyRecipient.Remote);
      this.AddScreen((GameScreen) new WisdomPickupScreen(player, durability, 1.0), player);
      player.Raise_FindWisdomScroll(durability);
    }

    private void PickupBook(ref InventoryItem item, Player player)
    {
      BookData bookData = this.GetBookData((ushort) (byte) item.Durability);
      if (bookData == null || player == null)
        return;
      this.AddScreen((GameScreen) new BookCoverScreen(this, player, bookData, -1), player);
      Sounds.PlaySound(Item.Book, ItemSoundType.Use);
    }

    public void BlueprintLoaded(BlueprintBlock block)
    {
      if (block == null || this.IsHost)
        return;
      Blueprint blueprint = Blueprints.GetBlueprint((int) block.ID);
      if (blueprint == null)
        return;
      blueprint.IsGenerated = true;
    }

    public void WisdomScrollLoaded(WisdomScrollBlock block)
    {
      if (block == null || this.IsHost)
        return;
      WisdomItem wisdom = Wisdom.GetWisdom((int) block.Index);
      if (wisdom == null)
        return;
      wisdom.IsGenerated = true;
    }

    public void CreateBlast(GlobalPoint3D p, Block blockID, GamerID playerID)
    {
      int num1;
      switch (blockID)
      {
        case Block.TNT:
          num1 = 5;
          break;
        case Block.C4:
          num1 = 7;
          break;
        default:
          num1 = 0;
          break;
      }
      int radius = num1;
      int num2;
      switch (blockID)
      {
        case Block.TNT:
          num2 = 25;
          break;
        case Block.C4:
          num2 = 40;
          break;
        default:
          num2 = 0;
          break;
      }
      float strength = (float) num2;
      if ((double) strength <= 0.0)
        return;
      this.CreateBlast(p, (Item) blockID, strength, radius, playerID);
    }

    public void CreateBlast(
      GlobalPoint3D p,
      Item itemID,
      float strength,
      int radius,
      GamerID playerID)
    {
      if (!this.HasPermission(playerID, Permissions.Grief))
        return;
      ushort seed = (ushort) this.Random.Next();
      this.EnqueueBlast(p, itemID, strength, radius, this.blastRandom, playerID, seed);
      this.networkManager.SendBlast(p, itemID, strength, radius, playerID, seed);
    }

    public void CreateRemoteBlast(
      GlobalPoint3D p,
      Item itemID,
      float strength,
      int radius,
      GamerID playerID,
      ushort seed)
    {
      this.EnqueueBlast(p, itemID, strength, radius, this.blastRandom, playerID, seed);
    }

    private void EnqueueBlast(
      GlobalPoint3D p,
      Item itemID,
      float strength,
      int radius,
      PcgRandom rand,
      GamerID playerID,
      ushort seed)
    {
      if (!this.map.IsValidPoint(p))
        return;
      QueuedBlast queuedBlast = new QueuedBlast()
      {
        Point = p,
        ItemID = itemID,
        Strength = strength,
        Radius = radius,
        Random = rand,
        PlayerID = playerID,
        Seed = seed
      };
      lock (this.queuedBlasts)
        this.queuedBlasts.Enqueue(queuedBlast);
    }

    private void ExplodeBlasts()
    {
      if (this.blastExplosionsWorkingCount >= 20)
        return;
      lock (this.queuedBlasts)
      {
        while (this.queuedBlasts.Count > 0 && this.blastExplosionsWorkingCount < 20)
          this.ExplodeBlast(this.queuedBlasts.Dequeue());
      }
    }

    private void ExplodeBlast(QueuedBlast blast)
    {
      ++this.blastExplosionsWorkingCount;
      int next = Explosion.Pool.GetNext();
      Explosion explosion = Explosion.Pool.List[next];
      explosion.Initialize(next, this, blast);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) explosion, true, PriorityLevel.Priority);
    }

    public void BlastExploded(QueuedBlast blast)
    {
      --this.blastExplosionsWorkingCount;
      Player player = this.GetPlayer(blast.PlayerID);
      for (int index = 0; index < this.actorList.Count; ++index)
        this.actorList[index]?.OnBlastCreated(blast, player, blast.ItemID);
      Sounds.PlaySound(ItemSoundGroup.EnvExplosion, ItemSoundType.Use, blast.Point);
      this.BroadcastSound(this.map.GetPosition(blast.Point), (Actor) null, SoundType.Explosion);
      player?.Raise_DetonateExplosive(blast.ItemID);
    }

    public BoundingBox GetBlockBox(GlobalPoint3D p)
    {
      return this.GetBlockBox(p, (Block) this.map.GetBlockID(p));
    }

    public BoundingBox GetBlockBox(GlobalPoint3D p, Block blockID)
    {
      BlockDataXML blockDataXml = this.map.BlockData[(int) blockID];
      if (blockDataXml.IsIcon)
      {
        if (blockID == Block.Book)
          return this.GetBookBox(p, blockID);
        return this.GetIconBox(p, blockID);
      }
      if (blockDataXml.Buffer == (byte) 4)
        return ModManager.GetPluginBlocks(blockID).GetBlockBox(p, blockID);
      Block block = blockID;
      if ((uint) block <= 140U)
      {
        if ((uint) block <= 77U)
        {
          switch (block)
          {
            case Block.Torch:
              return this.GetTorchBox(p);
            case Block.Ladder:
              break;
            case Block.WoodDoorTop:
            case Block.SteelDoorTop:
              goto label_24;
            case Block.Rope:
              return this.GetRopeBox(p);
            case Block.Pane:
            case Block.StainedGlassPane:
              return this.GetPaneBox(p);
            case Block.Post:
              goto label_21;
            default:
              goto label_33;
          }
        }
        else
        {
          switch (block)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
            case Block.LockedDoorTop:
              goto label_24;
            case Block.Sign:
              return this.GetSignBox(p);
            case Block.ClimbingIvy:
              break;
            case Block.Crop:
              return this.GetCropBox(p);
            case Block.Stack:
              goto label_13;
            case Block.UpsideDownStack:
              return this.GetUpsideDownStackableBlockBox(p);
            case Block.BedHead:
            case Block.BedFoot:
              return this.GetBedBox(p);
            case Block.Fence:
              return this.GetFenceBox(p);
            default:
              goto label_33;
          }
        }
        return this.GetLadderBox(p);
      }
      if ((uint) block <= 160U)
      {
        switch (block)
        {
          case Block.SnowLayer:
            goto label_13;
          case Block.HalfBlock:
            break;
          case Block.Painting:
            return this.GetPaintingBox(p);
          default:
            goto label_33;
        }
      }
      else
      {
        switch (block)
        {
          case Block.PressurePlate:
            return this.GetPlateBox(p);
          case Block.Switch:
            return this.GetSwitchBox(p);
          case Block.Button:
            return this.GetButtonBox(p);
          case Block.TrapDoor:
            return this.GetTrapDoorBox(p);
          case Block.HalfBlock2:
            break;
          case Block.Stack2:
            goto label_13;
          case Block.Post2:
            goto label_21;
          case Block.SidePost:
          case Block.SidePost2:
            return this.GetSidePostBox(p);
          case Block.CornerBlock:
          case Block.CornerBlock2:
            return this.GetCornerBlockBox(p);
          case Block.LockedDoorBottom:
            goto label_24;
          default:
            goto label_33;
        }
      }
      return this.GetHalfBlockBox(p);
label_13:
      return this.GetStackableBlockBox(p);
label_21:
      return this.GetPostBox(p);
label_24:
      return this.GetDoorBox(p);
label_33:
      return this.GetBlockBoxCore(p);
    }

    public BoundingBox GetBlockBoxCore(GlobalPoint3D p)
    {
      float tileSize = this.map.TileSize;
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = {
          X = (float) p.X * tileSize,
          Y = (float) p.Y * tileSize,
          Z = (float) p.Z * tileSize
        }
      };
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + tileSize;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      return boundingBox;
    }

    private BoundingBox GetIconBox(GlobalPoint3D p, Block blockID)
    {
      double num1;
      switch (blockID)
      {
        case Block.LongGrass:
          num1 = (double) this.map.TileSize * 0.75;
          break;
        case Block.BerryBush:
          num1 = (double) this.map.TileSize * 0.850000023841858;
          break;
        default:
          num1 = (double) this.map.TileSize * 0.5;
          break;
      }
      float num2 = (float) num1;
      float num3 = num2 * 0.5f;
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetBlockCenter(p);
      boundingBox.Min.X -= num3;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Min.Z -= num3;
      boundingBox.Max.X = boundingBox.Min.X + num2;
      boundingBox.Max.Y = boundingBox.Min.Y + num2;
      boundingBox.Max.Z = boundingBox.Min.Z + num2;
      return boundingBox;
    }

    private BoundingBox GetCropBox(GlobalPoint3D p)
    {
      float num1;
      float num2;
      switch ((int) this.map.GetAuxDataNoCache(p) & 7)
      {
        case 0:
          num1 = 0.19f;
          num2 = 0.19f;
          break;
        case 1:
          num1 = 0.6f;
          num2 = 0.42f;
          break;
        case 2:
          num1 = 0.8f;
          num2 = 0.78f;
          break;
        default:
          num1 = 0.95f;
          num2 = 0.95f;
          break;
      }
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetBlockCenter(p);
      boundingBox.Min.X -= num1 * 0.5f;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Min.Z -= num1 * 0.5f;
      boundingBox.Max.X = boundingBox.Min.X + num1;
      boundingBox.Max.Y = boundingBox.Min.Y + num2;
      boundingBox.Max.Z = boundingBox.Min.Z + num1;
      return boundingBox;
    }

    public BoundingBox GetHalfBlockBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num = tileSize * 0.5f;
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= this.map.GetAuxData(p) == (byte) 0 ? tileSize : num;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      return boundingBox;
    }

    public BoundingBox GetCornerBlockBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetPosition(p);
      byte auxData = this.map.GetAuxData(p);
      float num = this.map.TileSize * 0.5f;
      switch (auxData)
      {
        case 1:
        case 5:
          boundingBox.Min.X += num;
          break;
        case 2:
        case 6:
          boundingBox.Min.X += num;
          boundingBox.Min.Z += num;
          break;
        case 3:
        case 7:
          boundingBox.Min.Z += num;
          break;
      }
      boundingBox.Min.Y -= this.map.TileSize;
      if (auxData > (byte) 3)
        boundingBox.Min.Y += num;
      boundingBox.Max.X = boundingBox.Min.X + num;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + num;
      return boundingBox;
    }

    public BoundingBox GetFenceBox(GlobalPoint3D p)
    {
      float num1 = 0.1f;
      float num2 = this.map.TileSize * 0.5f - num1;
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = this.map.GetBlockCenter(p)
      };
      boundingBox.Max.X = boundingBox.Min.X;
      boundingBox.Max.Z = boundingBox.Min.Z;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Max.Y = boundingBox.Min.Y + this.map.TileSize;
      boundingBox.Min.X -= num1;
      boundingBox.Min.Z -= num1;
      boundingBox.Max.X += num1;
      boundingBox.Max.Z += num1;
      --p.X;
      if (this.IsFenceJoinedTo(this.map.GetBlockID(p)))
        boundingBox.Min.X -= num2;
      ++p.X;
      ++p.X;
      if (this.IsFenceJoinedTo(this.map.GetBlockID(p)))
        boundingBox.Max.X += num2;
      --p.X;
      --p.Z;
      if (this.IsFenceJoinedTo(this.map.GetBlockID(p)))
        boundingBox.Min.Z -= num2;
      ++p.Z;
      ++p.Z;
      if (this.IsFenceJoinedTo(this.map.GetBlockID(p)))
        boundingBox.Max.Z += num2;
      return boundingBox;
    }

    public bool IsFenceJoinedTo(byte block)
    {
      if (block > (byte) 0 && this.map.BlockData[(int) block].Buffer < (byte) 2)
        return true;
      switch ((Block) block)
      {
        case Block.Pane:
        case Block.StainedGlassPane:
        case Block.Fence:
          return true;
        default:
          return false;
      }
    }

    public BoundingBox GetPaneBox(GlobalPoint3D p)
    {
      float num1 = 0.1f;
      float num2 = this.map.TileSize * 0.5f - num1;
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = this.map.GetBlockCenter(p)
      };
      boundingBox.Max.X = boundingBox.Min.X;
      boundingBox.Max.Z = boundingBox.Min.Z;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Max.Y = boundingBox.Min.Y + this.map.TileSize;
      boundingBox.Min.X -= num1;
      boundingBox.Min.Z -= num1;
      boundingBox.Max.X += num1;
      boundingBox.Max.Z += num1;
      int num3 = 0;
      --p.X;
      if (BlockData.ShouldDrawPaneSection(this.map, (Block) this.map.GetBlockID(p)))
      {
        boundingBox.Min.X -= num2;
        ++num3;
      }
      ++p.X;
      ++p.X;
      if (BlockData.ShouldDrawPaneSection(this.map, (Block) this.map.GetBlockID(p)))
      {
        boundingBox.Max.X += num2;
        ++num3;
      }
      --p.X;
      --p.Z;
      if (BlockData.ShouldDrawPaneSection(this.map, (Block) this.map.GetBlockID(p)))
      {
        boundingBox.Min.Z -= num2;
        ++num3;
      }
      ++p.Z;
      ++p.Z;
      if (BlockData.ShouldDrawPaneSection(this.map, (Block) this.map.GetBlockID(p)))
      {
        boundingBox.Max.Z += num2;
        ++num3;
      }
      --p.Z;
      if (num3 == 0)
        boundingBox = this.GetBlockBoxCore(p);
      return boundingBox;
    }

    public BoundingBox GetPostBox(GlobalPoint3D p)
    {
      float num1 = this.map.TileSize * 0.5f;
      float num2 = num1 * 0.5f;
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetPosition(p);
      switch ((int) this.map.GetAuxDataNoCache(p) & 7)
      {
        case 0:
          boundingBox.Min.X += num2;
          boundingBox.Min.Z += num2;
          break;
        case 2:
          boundingBox.Min.X += num1;
          break;
        case 3:
          boundingBox.Min.X += num1;
          boundingBox.Min.Z += num1;
          break;
        case 4:
          boundingBox.Min.Z += num1;
          break;
      }
      boundingBox.Max.X = boundingBox.Min.X + num1;
      boundingBox.Max.Y = boundingBox.Min.Y;
      boundingBox.Max.Z = boundingBox.Min.Z + num1;
      boundingBox.Min.Y -= this.map.TileSize;
      return boundingBox;
    }

    public BoundingBox GetSidePostBox(GlobalPoint3D p)
    {
      float num = this.map.TileSize * 0.5f;
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= this.map.TileSize;
      byte auxDataNoCache = this.map.GetAuxDataNoCache(p);
      if (auxDataNoCache < (byte) 4)
      {
        switch (auxDataNoCache)
        {
          case 1:
            boundingBox.Min.X += num;
            break;
          case 2:
            boundingBox.Min.Y += num;
            break;
          case 3:
            boundingBox.Min.X += num;
            boundingBox.Min.Y += num;
            break;
        }
        boundingBox.Max.X = boundingBox.Min.X + num;
        boundingBox.Max.Y = boundingBox.Min.Y + num;
        boundingBox.Max.Z = boundingBox.Min.Z + this.map.TileSize;
      }
      else
      {
        switch (auxDataNoCache)
        {
          case 4:
            boundingBox.Min.Z += num;
            break;
          case 6:
            boundingBox.Min.Y += num;
            boundingBox.Min.Z += num;
            break;
          case 7:
            boundingBox.Min.Y += num;
            break;
        }
        boundingBox.Max.X = boundingBox.Min.X + this.map.TileSize;
        boundingBox.Max.Y = boundingBox.Min.Y + num;
        boundingBox.Max.Z = boundingBox.Min.Z + num;
      }
      return boundingBox;
    }

    public BoundingBox GetRopeBox(GlobalPoint3D p)
    {
      float num = 0.09f;
      double tileSize = (double) this.map.TileSize;
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = this.map.GetBlockCenter(p)
      };
      boundingBox.Max.X = boundingBox.Min.X;
      boundingBox.Max.Z = boundingBox.Min.Z;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Max.Y = boundingBox.Min.Y + this.map.TileSize;
      boundingBox.Min.X -= num;
      boundingBox.Min.Z -= num;
      boundingBox.Max.X += num;
      boundingBox.Max.Z += num;
      return boundingBox;
    }

    public BoundingBox GetTorchBox(GlobalPoint3D p)
    {
      float num1 = (float) ((double) (GraphicStatics.TexturePack.TorchSpriteWidth / 3) / (double) GraphicStatics.TexturePack.BlockTextureSize() + 0.100000001490116);
      float num2 = this.map.TileSize * 0.5f - num1;
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = this.map.GetBlockCenter(p)
      };
      boundingBox.Max.X = boundingBox.Min.X;
      boundingBox.Max.Z = boundingBox.Min.Z;
      boundingBox.Min.Y -= this.map.TileSize * 0.4f;
      boundingBox.Max.Y = boundingBox.Min.Y + this.map.TileSize * 0.7f;
      boundingBox.Min.X -= num1;
      boundingBox.Min.Z -= num1;
      boundingBox.Max.X += num1;
      boundingBox.Max.Z += num1;
      switch (this.map.GetAuxData(p))
      {
        case 0:
          boundingBox.Max.X += num2;
          boundingBox.Min.X += 0.2f;
          break;
        case 1:
          boundingBox.Max.Z += num2;
          boundingBox.Min.Z += 0.2f;
          break;
        case 2:
          boundingBox.Min.X -= num2;
          boundingBox.Max.X -= 0.2f;
          break;
        case 3:
          boundingBox.Min.Z -= num2;
          boundingBox.Max.Z -= 0.2f;
          break;
        case 4:
          float num3 = this.map.TileSize * 0.1f;
          boundingBox.Min.Y -= num3;
          boundingBox.Max.Y -= num3;
          break;
      }
      return boundingBox;
    }

    public BoundingBox GetLadderBox(GlobalPoint3D p)
    {
      BoundingBox blockBoxCore = this.GetBlockBoxCore(p);
      byte auxData = this.map.GetAuxData(p);
      float num1 = 0.15f;
      float num2 = 0.07f;
      switch (auxData)
      {
        case 0:
          blockBoxCore.Min.X = blockBoxCore.Max.X - num1;
          blockBoxCore.Min.Z += num2;
          blockBoxCore.Max.Z -= num2;
          break;
        case 1:
          blockBoxCore.Min.Z = blockBoxCore.Max.Z - num1;
          blockBoxCore.Min.X += num2;
          blockBoxCore.Max.X -= num2;
          break;
        case 2:
          blockBoxCore.Max.X = blockBoxCore.Min.X + num1;
          blockBoxCore.Min.Z += num2;
          blockBoxCore.Max.Z -= num2;
          break;
        case 3:
          blockBoxCore.Max.Z = blockBoxCore.Min.Z + num1;
          blockBoxCore.Min.X += num2;
          blockBoxCore.Max.X -= num2;
          break;
      }
      return blockBoxCore;
    }

    public BoundingBox GetSignBox(GlobalPoint3D p)
    {
      BoundingBox blockBoxCore = this.GetBlockBoxCore(p);
      byte auxData = this.map.GetAuxData(p);
      if (auxData < (byte) 4)
      {
        float num1 = 0.1f;
        float num2 = (float) (((double) blockBoxCore.Max.X - (double) blockBoxCore.Min.X) * 0.5) + blockBoxCore.Min.X;
        float num3 = (float) (((double) blockBoxCore.Max.Z - (double) blockBoxCore.Min.Z) * 0.5) + blockBoxCore.Min.Z;
        switch (auxData)
        {
          case 0:
          case 2:
            blockBoxCore.Min.X = num2 - num1;
            blockBoxCore.Max.X = num2 + num1;
            break;
          case 1:
          case 3:
            blockBoxCore.Min.Z = num3 - num1;
            blockBoxCore.Max.Z = num3 + num1;
            break;
        }
      }
      else
      {
        byte num1 = (byte) ((uint) auxData - 4U);
        float num2 = 0.15f;
        float num3 = 0.04f;
        switch (num1)
        {
          case 0:
            blockBoxCore.Min.X = blockBoxCore.Max.X - num2;
            blockBoxCore.Min.Z += num3;
            blockBoxCore.Max.Z -= num3;
            break;
          case 1:
            blockBoxCore.Min.Z = blockBoxCore.Max.Z - num2;
            blockBoxCore.Min.X += num3;
            blockBoxCore.Max.X -= num3;
            break;
          case 2:
            blockBoxCore.Max.X = blockBoxCore.Min.X + num2;
            blockBoxCore.Min.Z += num3;
            blockBoxCore.Max.Z -= num3;
            break;
          case 3:
            blockBoxCore.Max.Z = blockBoxCore.Min.Z + num2;
            blockBoxCore.Min.X += num3;
            blockBoxCore.Max.X -= num3;
            break;
        }
        blockBoxCore.Min.Y += 0.35f;
        blockBoxCore.Max.Y -= 0.1f;
      }
      return blockBoxCore;
    }

    public BoundingBox GetPaintingBox(GlobalPoint3D p)
    {
      BoundingBox blockBoxCore = this.GetBlockBoxCore(p);
      byte auxData = this.map.GetAuxData(p);
      float num1 = 0.1f;
      float num2 = 0.0f;
      if (((int) auxData & 4) > 0)
      {
        blockBoxCore.Max.Y -= this.Map.TileSize - num1;
      }
      else
      {
        switch ((int) auxData & 3)
        {
          case 0:
            blockBoxCore.Min.X = blockBoxCore.Max.X - num1;
            blockBoxCore.Min.Z += num2;
            blockBoxCore.Max.Z -= num2;
            break;
          case 1:
            blockBoxCore.Min.Z = blockBoxCore.Max.Z - num1;
            blockBoxCore.Min.X += num2;
            blockBoxCore.Max.X -= num2;
            break;
          case 2:
            blockBoxCore.Max.X = blockBoxCore.Min.X + num1;
            blockBoxCore.Min.Z += num2;
            blockBoxCore.Max.Z -= num2;
            break;
          case 3:
            blockBoxCore.Max.Z = blockBoxCore.Min.Z + num1;
            blockBoxCore.Min.X += num2;
            blockBoxCore.Max.X -= num2;
            break;
        }
      }
      return blockBoxCore;
    }

    public BoundingBox GetSwitchBox(GlobalPoint3D p)
    {
      BoundingBox blockBoxCore = this.GetBlockBoxCore(p);
      byte auxData = this.map.GetAuxData(p);
      float num1 = 0.1f;
      float num2 = 0.2f;
      Vector3 vector3 = (blockBoxCore.Max - blockBoxCore.Min) * 0.5f + blockBoxCore.Min;
      switch ((int) auxData & 7)
      {
        case 0:
          blockBoxCore.Min.X = blockBoxCore.Max.X - num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 1:
          blockBoxCore.Min.Z = blockBoxCore.Max.Z - num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          break;
        case 2:
          blockBoxCore.Max.X = blockBoxCore.Min.X + num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 3:
          blockBoxCore.Max.Z = blockBoxCore.Min.Z + num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          break;
        case 4:
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          blockBoxCore.Max.Y = blockBoxCore.Min.Y + num1;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 5:
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          blockBoxCore.Min.Y = blockBoxCore.Max.Y - num1;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
      }
      blockBoxCore.Min -= new Vector3(0.04f, 0.04f, 0.04f);
      blockBoxCore.Max += new Vector3(0.04f, 0.04f, 0.04f);
      return blockBoxCore;
    }

    private bool IsBlockDeliveringPower(GlobalPoint3D p)
    {
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
        return mapStrategy.IsBlockDeliveringPower(p);
      return false;
    }

    public BoundingBox GetButtonBox(GlobalPoint3D p)
    {
      BoundingBox blockBoxCore = this.GetBlockBoxCore(p);
      byte auxData = this.map.GetAuxData(p);
      float num1 = this.IsBlockDeliveringPower(p) ? 0.04f : 0.12f;
      float num2 = 0.15f;
      Vector3 vector3 = (blockBoxCore.Max - blockBoxCore.Min) * 0.5f + blockBoxCore.Min;
      switch ((int) auxData & 7)
      {
        case 0:
          blockBoxCore.Min.X = blockBoxCore.Max.X - num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 1:
          blockBoxCore.Min.Z = blockBoxCore.Max.Z - num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          break;
        case 2:
          blockBoxCore.Max.X = blockBoxCore.Min.X + num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 3:
          blockBoxCore.Max.Z = blockBoxCore.Min.Z + num1;
          blockBoxCore.Min.Y = vector3.Y - num2;
          blockBoxCore.Max.Y = vector3.Y + num2;
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          break;
        case 4:
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          blockBoxCore.Max.Y = blockBoxCore.Min.Y + num1;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
        case 5:
          blockBoxCore.Min.X = vector3.X - num2;
          blockBoxCore.Max.X = vector3.X + num2;
          blockBoxCore.Min.Y = blockBoxCore.Max.Y - num1;
          blockBoxCore.Min.Z = vector3.Z - num2;
          blockBoxCore.Max.Z = vector3.Z + num2;
          break;
      }
      blockBoxCore.Min -= new Vector3(0.04f, 0.04f, 0.04f);
      blockBoxCore.Max += new Vector3(0.04f, 0.04f, 0.04f);
      return blockBoxCore;
    }

    public BoundingBox GetBedBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= tileSize;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + tileSize * 0.565f;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      return boundingBox;
    }

    public BoundingBox GetStackableBlockBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num = tileSize * (float) ((double) ((int) this.map.GetAuxData(p) + 1) * 1.0 / 8.0);
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= tileSize;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      return boundingBox;
    }

    public BoundingBox GetUpsideDownStackableBlockBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num1 = Math.Max(1.5f, (float) ((int) this.map.GetAuxData(p) + 1));
      float num2 = (float) ((double) tileSize * (double) num1 * 1.0 / 8.0);
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      boundingBox.Min.Y -= tileSize * num2;
      return boundingBox;
    }

    public BoundingBox GetPlateBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num = tileSize * GraphicStatics.PlateHeight;
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= tileSize;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      return boundingBox;
    }

    public BoundingBox GetDoorBox(GlobalPoint3D p)
    {
      float tileSize = this.map.TileSize;
      float num = tileSize * 0.1f;
      byte auxData = this.map.GetAuxData(p);
      if (auxData > (byte) 3)
        auxData -= (byte) 4;
      Vector3 vector3_1 = new Vector3(0.0f, -tileSize, 0.0f);
      Vector3 vector3_2 = new Vector3(tileSize, 0.0f, tileSize);
      switch (auxData)
      {
        case 1:
          vector3_2.Z = vector3_1.Z + num;
          break;
        case 2:
          vector3_1.X = vector3_2.X - num;
          break;
        case 3:
          vector3_1.Z = vector3_2.Z - num;
          break;
        default:
          vector3_2.X = vector3_1.X + num;
          break;
      }
      Vector3 position = this.map.GetPosition(p);
      return new BoundingBox()
      {
        Min = position + vector3_1,
        Max = position + vector3_2
      };
    }

    public BoundingBox GetTrapDoorBox(GlobalPoint3D p)
    {
      float tileSize = this.map.TileSize;
      float num = tileSize * 0.1f;
      byte auxData = this.map.GetAuxData(p);
      Vector3 vector3_1 = new Vector3(0.0f, -num, 0.0f);
      Vector3 vector3_2 = new Vector3(tileSize, 0.0f, tileSize);
      if (auxData > (byte) 3)
      {
        switch (auxData)
        {
          case 4:
            vector3_1.Y = -tileSize;
            vector3_1.Z = vector3_2.Z - num;
            break;
          case 5:
            vector3_1.Y = -tileSize;
            vector3_2.X = vector3_1.X + num;
            break;
          case 6:
            vector3_1.Y = -tileSize;
            vector3_2.Z = vector3_1.Z + num;
            break;
          default:
            vector3_1.Y = -tileSize;
            vector3_1.X = vector3_2.X - num;
            break;
        }
      }
      Vector3 position = this.map.GetPosition(p);
      return new BoundingBox()
      {
        Min = position + vector3_1,
        Max = position + vector3_2
      };
    }

    private BoundingBox GetBookBox(GlobalPoint3D p, Block blockID)
    {
      float num1 = this.map.TileSize * 0.55f;
      float num2 = num1 * 0.5f;
      float num3 = num1 * 0.6f;
      BoundingBox boundingBox = new BoundingBox();
      boundingBox.Min = this.map.GetBlockCenter(p);
      boundingBox.Min.X -= num2;
      boundingBox.Min.Y -= this.map.TileSize * 0.5f;
      boundingBox.Min.Z -= num3 * 0.5f;
      boundingBox.Max.X = boundingBox.Min.X + num1;
      boundingBox.Max.Y = boundingBox.Min.Y + num1;
      boundingBox.Max.Z = boundingBox.Min.Z + num3;
      return boundingBox;
    }

    private byte GetSignAux(GlobalPoint3D p, BlockFace swingFace, Player player)
    {
      this.map.GetBlockCenter(p);
      byte auxRotate = this.GetAuxRotate(p, Block.Sign, swingFace, player);
      if (swingFace != BlockFace.Up)
        auxRotate += (byte) 4;
      return auxRotate;
    }

    private bool AddSign(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit);
      if (playerID.IsGamer)
      {
        this.newSignPoint = p;
        Player player = this.GetPlayer(playerID);
        if (player != null)
          Guide.BeginShowKeyboardInput(StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager, player.PlayerIndex, "Enter the Sign text", "Use _ (underscore) to separate lines (new line). You can have up to 4 lines", (string) null, new AsyncCallback(this.OnSignTextEntered), (object) null);
      }
      return true;
    }

    private void OnSignTextEntered(IAsyncResult ar)
    {
      string s = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      string text = Utils.StripChars(s, 32, 160);
      this.AddSignFromRemote(this.newSignPoint, text);
      this.networkManager.SendSignText(this.newSignPoint, text);
    }

    public void AddSignFromRemote(GlobalPoint3D p, string text)
    {
      if (this.map == null || !this.MapStrategyTM.AddSignBlock(p, text, UpdateBlockMethod.Strategy))
        return;
      this.MapRenderer.SignsChanged(false);
    }

    private byte GetBedAux(GlobalPoint3D p, Player player)
    {
      byte num = 2;
      GlobalPoint3D bedHeadOffset = this.GetBedHeadOffset((Actor) player, p);
      if (bedHeadOffset.X == -1)
        num = (byte) 0;
      else if (bedHeadOffset.Z == -1)
        num = (byte) 1;
      else if (bedHeadOffset.Z == 1)
        num = (byte) 3;
      return num;
    }

    private bool AddBed(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit);
      GlobalPoint3D headOffsetFromAux = this.GetBedHeadOffsetFromAux(auxData);
      this.map.SetBlockData(p + headOffsetFromAux, (byte) (blockID - (byte) 1), auxData, method, playerID, transmit);
      return true;
    }

    public GlobalPoint3D GetBedHeadOffset(Actor player, GlobalPoint3D p)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      zero.X = 1;
      if (player != null)
      {
        Vector3 blockCenter = this.map.GetBlockCenter(p);
        Vector3 position = player.Position;
        float num = Math.Abs(position.X - blockCenter.X);
        if ((double) Math.Abs(position.Z - blockCenter.Z) < (double) num)
        {
          if ((double) position.X > (double) blockCenter.X)
            zero.X = -1;
        }
        else if ((double) position.Z > (double) blockCenter.Z)
        {
          zero.X = 0;
          zero.Z = -1;
        }
        else
        {
          zero.X = 0;
          zero.Z = 1;
        }
      }
      return zero;
    }

    public GlobalPoint3D GetBedHeadOffsetFromAux(byte auxData)
    {
      switch (auxData)
      {
        case 0:
          return GlobalPoint3D.Left;
        case 1:
          return GlobalPoint3D.Forward;
        case 2:
          return GlobalPoint3D.Right;
        default:
          return GlobalPoint3D.Backward;
      }
    }

    public float TimeSleptInSeconds
    {
      get
      {
        return this.sleepTimer;
      }
    }

    public bool IsSleeping
    {
      get
      {
        if ((double) this.sleepPeriod == -1.0 || (double) this.sleepTimer < (double) this.sleepPeriod)
          return this.allPlayersSleeping;
        return false;
      }
    }

    public bool AllPlayersSleeping
    {
      get
      {
        return this.allPlayersSleeping;
      }
    }

    public void StartSleep(Player player)
    {
      if (player.IsSleeping)
        return;
      player.IsSleeping = true;
      this.AddScreen((GameScreen) new SleepingScreen(this, player), player);
    }

    public void StartSleep(Player player, float hours)
    {
      if (this.IsSleeping)
        return;
      this.StartSleep(player);
      this.StartSleep(hours);
    }

    public void StartSleep(float hours)
    {
      if ((double) hours == -1.0)
      {
        if (this.SunMoon != null)
          this.SunMoon.SunriseEnded += new EventHandler(this.OnSunriseEndedForSleep);
        this.sleepPeriod = -1f;
      }
      else
        this.sleepPeriod = (float) ((double) hours * 60.0 * 60.0 * 0.0138888889923692);
      this.sleepTimer = 0.0f;
      this.StartSleepThread();
    }

    private void StartSleepThread()
    {
      if (this.IsSleeping && !ThreadQueueManager.Instance.QueueContainsItem((IThreadWorkItem) this.sleeper, PriorityLevel.Priority))
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.sleeper, true, PriorityLevel.Priority);
        this.networkManager.SendSleepState(this.GetHoursFromSleepPeriod(this.sleepPeriod));
      }
      else
      {
        if (this.PlayersSleepingCount != 0)
          return;
        this.ClearSleepState();
      }
    }

    public void ClearSleepState()
    {
      if ((double) this.sleepPeriod == -1.0 && this.SunMoon != null)
        this.SunMoon.SunriseEnded -= new EventHandler(this.OnSunriseEndedForSleep);
      float num = this.GetHoursFromSleepPeriod(this.sleepTimer);
      if ((double) num > 0.0 && (double) num < 1.0)
        num = 1f;
      Globals2.GameProperties.SaveGame.Header.HoursSlept += (int) num;
      this.sleepPeriod = this.sleepTimer = 0.0f;
      this.FlagAllPlayersSleepHasFinished();
    }

    private void OnSunriseEndedForSleep(object sender, EventArgs e)
    {
      this.ClearSleepState();
    }

    private float GetHoursFromSleepPeriod(float sleepPeriod)
    {
      if ((double) sleepPeriod == -1.0)
        return -1f;
      return (float) ((double) sleepPeriod / 60.0 / 60.0 / 0.0138888889923692);
    }

    public void RecalcAllPlayersSleeping()
    {
      bool flag = true;
      foreach (Gamer allEnabledGamer in this.networkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null && !tag.IsSleeping)
          flag = false;
      }
      this.allPlayersSleeping = flag;
      this.StartSleepThread();
    }

    public void FlagAllPlayersSleepHasFinished()
    {
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null)
          tag.IsSleeping = false;
      }
    }

    public void NotifyBehaviourChanged(string treeName)
    {
      if (this.npcManager == null)
        return;
      this.npcManager.NotifyBehaviourChanged(treeName);
    }

    private byte GetHalfBlockAux(BlockFace swingFace)
    {
      return swingFace == BlockFace.Up ? (byte) 0 : (byte) 1;
    }

    public byte GetAuxRotate(GlobalPoint3D p, Block blockID, BlockFace swingFace, Player player)
    {
      byte num1 = (byte) swingFace;
      if ((swingFace == BlockFace.Up || swingFace == BlockFace.Down) && player != null)
      {
        num1 = (byte) 0;
        Vector3 blockCenter = this.map.GetBlockCenter(p);
        Vector3 position = player.Position;
        float num2 = Math.Abs(position.X - blockCenter.X);
        if ((double) Math.Abs(position.Z - blockCenter.Z) < (double) num2)
        {
          if ((double) position.X > (double) blockCenter.X)
            num1 = (byte) 2;
        }
        else
          num1 = (double) position.Z <= (double) blockCenter.Z ? (byte) 1 : (byte) 3;
        Block block = blockID;
        if ((uint) block <= 150U)
        {
          if (block != Block.Stairs && block != Block.Ramp)
            goto label_11;
        }
        else
        {
          switch (block)
          {
            case Block.Painting:
              num1 += (byte) 4;
              goto label_11;
            case Block.Stairs2:
            case Block.Ramp2:
              break;
            default:
              goto label_11;
          }
        }
        if (swingFace == BlockFace.Down)
          num1 |= (byte) 4;
      }
label_11:
      return num1;
    }

    public BoundingBox GetStairBoxLow(GlobalPoint3D p, bool isYTest)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num = tileSize * 0.5f;
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= tileSize;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      if (!isYTest)
      {
        switch ((int) this.map.GetAuxDataNoCache(p) & 3)
        {
          case 0:
            boundingBox.Max.X -= num;
            break;
          case 1:
            boundingBox.Max.Z -= num;
            break;
          case 2:
            boundingBox.Min.X += num;
            break;
          case 3:
            boundingBox.Min.Z += num;
            break;
        }
      }
      return boundingBox;
    }

    public BoundingBox GetStairBoxHigh(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox();
      float tileSize = this.map.TileSize;
      float num = tileSize * 0.5f;
      boundingBox.Min = this.map.GetPosition(p);
      boundingBox.Min.Y -= num;
      boundingBox.Max.X = boundingBox.Min.X + tileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + num;
      boundingBox.Max.Z = boundingBox.Min.Z + tileSize;
      switch ((int) this.map.GetAuxDataNoCache(p) & 3)
      {
        case 0:
          boundingBox.Min.X += num;
          break;
        case 1:
          boundingBox.Min.Z += num;
          break;
        case 2:
          boundingBox.Max.X -= num;
          break;
        case 3:
          boundingBox.Max.Z -= num;
          break;
      }
      return boundingBox;
    }

    public void HitDoor(GlobalPoint3D p, Player player, Hand hand)
    {
      if (!this.CanOpenDoor(p, player, hand))
        return;
      MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(p);
      if (blockIdAndAux.BlockID == (byte) 171)
      {
        byte aux = (byte) ((uint) this.SwitchDoorState(blockIdAndAux.AuxData) + ((uint) blockIdAndAux.AuxData & 240U));
        this.HitDoorCore(p, aux, (Actor) player);
        this.map.Commit();
      }
      else
      {
        GlobalPoint3D globalPoint3D = GlobalPoint3D.Up;
        switch ((Block) blockIdAndAux.BlockID)
        {
          case Block.WoodDoorTop:
          case Block.SteelDoorTop:
          case Block.LockedDoorTop:
            --p.Y;
            break;
        }
        if (!ItemData.IsSubType((Item) this.map.GetBlockID(p + globalPoint3D), ItemSubType.Door))
          globalPoint3D = GlobalPoint3D.Zero;
        else if (!ItemData.IsSubType((Item) this.map.GetBlockID(p), ItemSubType.Door))
        {
          globalPoint3D = GlobalPoint3D.Zero;
          ++p.Y;
        }
        if (!ItemData.IsSubType((Item) this.map.GetBlockID(p), ItemSubType.Door))
          return;
        byte num = (byte) ((uint) this.SwitchDoorState(blockIdAndAux.AuxData) + ((uint) blockIdAndAux.AuxData & 240U));
        this.HitDoorCore(p, p + globalPoint3D, num, (Actor) player);
        this.networkManager.SendDoorChangeConfirm(p, p + globalPoint3D, num);
        this.MapStrategyTM.TogglePowerReceipt(p);
        this.MapStrategyTM.TogglePowerReceipt(p + globalPoint3D);
        this.map.Commit();
      }
    }

    public bool CanOpenDoor(GlobalPoint3D p, Player player, Hand hand)
    {
      Block blockId = (Block) this.map.GetBlockID(p);
      switch (blockId)
      {
        case Block.LockedDoorTop:
        case Block.LockedDoorBottom:
          if (player != null && player.Gamer != null)
          {
            if (blockId == Block.LockedDoorTop)
              --p.Y;
            PlayerBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as PlayerBlock;
            if (hand != null)
              return this.IsKeyAndCanOpen(player, dataBlock, p, hand.ItemID);
            if (this.IsKeyAndCanOpen(player, dataBlock, p, player.LeftHand.ItemID) || this.IsKeyAndCanOpen(player, dataBlock, p, player.RightHand.ItemID))
              return true;
          }
          return false;
        default:
          return true;
      }
    }

    private bool IsKeyAndCanOpen(Player player, PlayerBlock block, GlobalPoint3D p, Item itemID)
    {
      if (player.IsGod)
        return true;
      if (block == null)
        return false;
      if (block.IsOwner(player) && !this.IsItemAKey(itemID))
        return true;
      return this.IsKeyGood(p, itemID);
    }

    public bool IsKeyGood(GlobalPoint3D p, Item key)
    {
      if (key == Item.SkeletonKey)
        return true;
      byte auxHighData = this.map.GetAuxHighData(p);
      int blockTextureId = (int) this.map.GetBlockTextureID((Block) this.map.GetBlockID(p), (int) auxHighData);
      if (blockTextureId >= this.KeyList.Length)
        return false;
      return this.KeyList[blockTextureId < 0 ? 0 : blockTextureId] == key;
    }

    public bool IsItemAKey(Item item)
    {
      if (this.KeyList != null)
      {
        foreach (Item key in this.KeyList)
        {
          if (key == item)
            return true;
        }
      }
      return false;
    }

    public void HitDoorCore(GlobalPoint3D p, byte aux, Actor c)
    {
      this.map.SetAuxData(p, aux, UpdateBlockMethod.Player, GamerID.Sys1, false);
      this.pointsForCollisionToIgnore.Add(new GameInstance.PointToIgnore()
      {
        Point = p,
        Counter = 2
      });
      Sounds.PlaySound((Item) this.map.GetBlockIDNoCache(p), ItemSoundType.Use, p, (ITMActor) c);
    }

    public void HitDoorCore(GlobalPoint3D p1, GlobalPoint3D p2, byte aux, Actor c)
    {
      this.map.SetAuxData(p1, aux, UpdateBlockMethod.Player, GamerID.Sys1, false);
      this.map.SetAuxData(p2, aux, UpdateBlockMethod.Player, GamerID.Sys1, false);
      this.pointsForCollisionToIgnore.Add(new GameInstance.PointToIgnore()
      {
        Point = p1,
        Counter = 2
      });
      this.pointsForCollisionToIgnore.Add(new GameInstance.PointToIgnore()
      {
        Point = p2,
        Counter = 2
      });
      Sounds.PlaySound((Item) this.map.GetBlockIDNoCache(p1), ItemSoundType.Use, p1, (ITMActor) c);
    }

    public byte SwitchDoorState(byte aux)
    {
      switch ((int) aux & 7)
      {
        case 0:
          return 7;
        case 1:
          return 4;
        case 2:
          return 5;
        case 3:
          return 6;
        case 4:
          return 1;
        case 5:
          return 2;
        case 6:
          return 3;
        case 7:
          return 0;
        default:
          return aux;
      }
    }

    private byte GetDoorAux(GlobalPoint3D p, Player player)
    {
      byte auxData = 0;
      if (player != null && p.Y < this.map.MapBound.Max.Y - 1 && this.map.GetBlockID(p + GlobalPoint3D.Up) == (byte) 0)
      {
        Vector3 blockCenter = this.map.GetBlockCenter(p);
        Vector3 position = player.Position;
        float num = Math.Abs(position.X - blockCenter.X);
        if ((double) Math.Abs(position.Z - blockCenter.Z) < (double) num)
        {
          if ((double) position.X > (double) blockCenter.X)
            auxData = (byte) 2;
        }
        else
          auxData = (double) position.Z <= (double) blockCenter.Z ? (byte) 1 : (byte) 3;
        auxData = this.SwapDoorHinge(p, auxData);
      }
      return auxData;
    }

    private bool AddDoor(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (p.Y >= this.map.MapBound.Max.Y - 1 || this.map.GetBlockID(p + GlobalPoint3D.Up) != (byte) 0)
        return false;
      ++p.Y;
      this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit);
      this.pointsForCollisionToIgnore.Add(new GameInstance.PointToIgnore()
      {
        Point = p,
        Counter = 2
      });
      switch (blockID)
      {
        case Block.WoodDoorTop:
          blockID = Block.WoodDoorBottom;
          break;
        case Block.SteelDoorTop:
          blockID = Block.SteelDoorBottom;
          break;
        default:
          blockID = Block.LockedDoorBottom;
          break;
      }
      --p.Y;
      this.map.SetBlockData(p, (byte) blockID, auxData, method, playerID, transmit);
      this.pointsForCollisionToIgnore.Add(new GameInstance.PointToIgnore()
      {
        Point = p,
        Counter = 2
      });
      return true;
    }

    private byte SwapDoorHinge(GlobalPoint3D p, byte auxData)
    {
      switch (auxData)
      {
        case 1:
          Block blockId1 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Right);
          int num1;
          switch (blockId1)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num1 = 1;
              break;
            default:
              num1 = blockId1 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num1 != 0)
            return auxData;
          Block blockId2 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Left);
          int num2;
          switch (blockId2)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num2 = 1;
              break;
            default:
              num2 = blockId2 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num2 != 0)
            return (byte) ((uint) auxData + 4U);
          bool flag1 = this.map.IsSolid(p + GlobalPoint3D.Right) || this.map.IsSolid(p + GlobalPoint3D.Right + GlobalPoint3D.Up);
          bool flag2 = this.map.IsSolid(p + GlobalPoint3D.Left) || this.map.IsSolid(p + GlobalPoint3D.Left + GlobalPoint3D.Up);
          return !flag1 || flag2 ? auxData : (byte) ((int) auxData + 4);
        case 2:
          Block blockId3 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Backward);
          int num3;
          switch (blockId3)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num3 = 1;
              break;
            default:
              num3 = blockId3 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num3 != 0)
            return auxData;
          Block blockId4 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Forward);
          int num4;
          switch (blockId4)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num4 = 1;
              break;
            default:
              num4 = blockId4 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num4 != 0)
            return (byte) ((uint) auxData + 4U);
          bool flag3 = this.map.IsSolid(p + GlobalPoint3D.Backward) || this.map.IsSolid(p + GlobalPoint3D.Backward + GlobalPoint3D.Up);
          bool flag4 = this.map.IsSolid(p + GlobalPoint3D.Forward) || this.map.IsSolid(p + GlobalPoint3D.Forward + GlobalPoint3D.Up);
          return !flag3 || flag4 ? auxData : (byte) ((int) auxData + 4);
        case 3:
          Block blockId5 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Left);
          int num5;
          switch (blockId5)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num5 = 1;
              break;
            default:
              num5 = blockId5 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num5 != 0)
            return auxData;
          Block blockId6 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Right);
          int num6;
          switch (blockId6)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num6 = 1;
              break;
            default:
              num6 = blockId6 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num6 != 0)
            return (byte) ((uint) auxData + 4U);
          bool flag5 = this.map.IsSolid(p + GlobalPoint3D.Left) || this.map.IsSolid(p + GlobalPoint3D.Left + GlobalPoint3D.Up);
          bool flag6 = this.map.IsSolid(p + GlobalPoint3D.Right) || this.map.IsSolid(p + GlobalPoint3D.Right + GlobalPoint3D.Up);
          return !flag5 || flag6 ? auxData : (byte) ((int) auxData + 4);
        default:
          Block blockId7 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Forward);
          int num7;
          switch (blockId7)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num7 = 1;
              break;
            default:
              num7 = blockId7 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num7 != 0)
            return auxData;
          Block blockId8 = (Block) this.map.GetBlockID(p + GlobalPoint3D.Backward);
          int num8;
          switch (blockId8)
          {
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
              num8 = 1;
              break;
            default:
              num8 = blockId8 == Block.LockedDoorBottom ? 1 : 0;
              break;
          }
          if (num8 != 0)
            return (byte) ((uint) auxData + 4U);
          bool flag7 = this.map.IsSolid(p + GlobalPoint3D.Forward) || this.map.IsSolid(p + GlobalPoint3D.Forward + GlobalPoint3D.Up);
          bool flag8 = this.map.IsSolid(p + GlobalPoint3D.Backward) || this.map.IsSolid(p + GlobalPoint3D.Backward + GlobalPoint3D.Up);
          return !flag7 || flag8 ? auxData : (byte) ((int) auxData + 4);
      }
    }

    public string GetArcadeMachineName(int gameID)
    {
      if (gameID == 128)
        return GameInstance.arcadeMachineNames[0];
      if (gameID < 3)
        return GameInstance.arcadeMachineNames[gameID];
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.PluginArcade != null && activeMod.TypeCounts.ArcadeMachine > (ushort) 0 && (gameID >= (int) activeMod.TypeOffsets.ArcadeMachine && gameID < (int) activeMod.TypeOffsets.ArcadeMachine + (int) activeMod.TypeCounts.ArcadeMachine))
        {
          string arcadeMachineName = activeMod.PluginArcade.GetArcadeMachineName(gameID - (int) activeMod.TypeOffsets.ArcadeMachine);
          return arcadeMachineName.IsEmpty() ? "Unknown" : arcadeMachineName;
        }
      }
      return "Unknown";
    }

    public ArcadeMachine HitArcadeMachine(
      Player player,
      GlobalPoint3D p,
      BlockFace face,
      Item itemID)
    {
      ArcadeMachine arcadeMachine = this.FindArcadeMachine(p, face) ?? this.ResetArcadeMachine(player, p, face);
      arcadeMachine?.PlayerHitBlock((ITMPlayer) player, itemID);
      return arcadeMachine;
    }

    public ArcadeMachine ResetArcadeMachine(
      Player player,
      GlobalPoint3D p,
      BlockFace face)
    {
      this.RemoveArcadeMachine(player);
      byte auxHighData = this.map.GetAuxHighData(p);
      ArcadeMachine arcadeMachine = (ArcadeMachine) null;
      switch (auxHighData)
      {
        case 1:
          arcadeMachine = (ArcadeMachine) new StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders(this, (ITMMap) this.map, player, p, face);
          arcadeMachine.Renderer = StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ArcadeMachineRenderer.InvadersRenderer;
          break;
        case 2:
          arcadeMachine = (ArcadeMachine) new StudioForge.TotalMiner.Arcade.TotalRush.TotalRush(this, (ITMMap) this.map, player, p, face);
          arcadeMachine.Renderer = StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ArcadeMachineRenderer.RushRenderer;
          break;
        default:
          foreach (Mod activeMod in ModManager.ActiveMods)
          {
            if (activeMod.PluginArcade != null && activeMod.TypeCounts.ArcadeMachine > (ushort) 0 && ((int) auxHighData >= (int) activeMod.TypeOffsets.ArcadeMachine && (int) auxHighData < (int) activeMod.TypeOffsets.ArcadeMachine + (int) activeMod.TypeCounts.ArcadeMachine))
            {
              arcadeMachine = activeMod.PluginArcade.GetArcadeMachine((int) auxHighData - (int) activeMod.TypeOffsets.ArcadeMachine, (ITMGame) this, (ITMMap) this.map, (ITMPlayer) player, p, face);
              arcadeMachine.Renderer = activeMod.PluginArcade.GetArcadeMachineRenderer((int) auxHighData - (int) activeMod.TypeOffsets.ArcadeMachine);
              break;
            }
          }
          if (arcadeMachine == null)
          {
            arcadeMachine = (ArcadeMachine) new ArcadeGameSelector(this, (ITMMap) this.map, player, p, face);
            arcadeMachine.Renderer = StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ArcadeMachineRenderer.GameSelectorRenderer;
            break;
          }
          break;
      }
      arcadeMachine.Initialize((StudioForge.Engine.Integration.InitState) null);
      arcadeMachine.LoadContent((StudioForge.Engine.Integration.InitState) null);
      this.ArcadeMachines.Add(arcadeMachine);
      player.CurrentArcadeMachine = arcadeMachine;
      return arcadeMachine;
    }

    public ArcadeMachine FindArcadeMachine(GlobalPoint3D p, BlockFace face)
    {
      for (int index = 0; index < this.ArcadeMachines.Count; ++index)
      {
        ArcadeMachine arcadeMachine = this.ArcadeMachines[index];
        if (arcadeMachine != null && arcadeMachine.Point == p && arcadeMachine.Face == face)
          return arcadeMachine;
      }
      return (ArcadeMachine) null;
    }

    public void RemoveArcadeMachine(GlobalPoint3D p, UpdateBlockMethod method)
    {
      for (int index = this.ArcadeMachines.Count - 1; index >= 0; --index)
      {
        if (this.ArcadeMachines[index].Point == p)
        {
          if (method == UpdateBlockMethod.Strategy && !this.ArcadeMachines[index].CanDeactivate)
            break;
          foreach (Player localEnabledPlayer in this.networkManager.LocalEnabledPlayers)
          {
            if (localEnabledPlayer.CurrentArcadeMachine == this.ArcadeMachines[index])
              localEnabledPlayer.CurrentArcadeMachine = (ArcadeMachine) null;
          }
          this.ArcadeMachines[index].UnloadContent();
          this.ArcadeMachines.RemoveAt(index);
          this.UnloadArcadeMachineBuffers();
          break;
        }
      }
    }

    private void RemoveArcadeMachine(Player player)
    {
      if (player.CurrentArcadeMachine == null)
        return;
      player.CurrentArcadeMachine.UnloadContent();
      this.ArcadeMachines.Remove(player.CurrentArcadeMachine);
      this.UnloadArcadeMachineBuffers();
      player.CurrentArcadeMachine = (ArcadeMachine) null;
    }

    private void UnloadArcadeMachineBuffers()
    {
    }

    public int BookCount
    {
      get
      {
        return this.MapStrategyTM.BookDataList.Count;
      }
    }

    public BookData GetBookData(ushort bookID)
    {
      return this.MapStrategyTM.GetBookData(bookID);
    }

    private bool AddBookBlock(
      GlobalPoint3D p,
      object ID,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      if (this.map.SetBlockData(p, (byte) 121, (byte) 0, method, playerID, transmit) == null)
        return false;
      if (ID is ushort)
      {
        BookBlock orAddDataBlock = this.MapStrategyTM.GetOrAddDataBlock(p, Block.Book, method, playerID, true) as BookBlock;
        if (orAddDataBlock != null)
        {
          orAddDataBlock.ID = (ushort) ID;
          this.networkManager.SendDataBlockChange((DataBlock) orAddDataBlock, true, method);
        }
      }
      return true;
    }

    public void AddBookData(BookData book, Player player, short slotID, bool transmit)
    {
      if (book == null || player == null)
        return;
      if (this.IsHost)
      {
        this.AllocateAndConfirmNewBookID(book, player.GamerID, slotID);
        this.SetBookIDInInventory(book, player, (int) slotID);
        if (!transmit)
          return;
        this.networkManager.SendBookUpdate(book);
      }
      else
        this.networkManager.SendBookIDRequest(book, player, slotID);
    }

    public void AllocateAndConfirmNewBookID(BookData book, GamerID gamerID, short slotID)
    {
      if (!this.IsHost)
        return;
      book = this.MapStrategyTM.AddBookData(book);
      if (book == null)
        return;
      this.networkManager.SendBookIDConfirm(book, gamerID, slotID);
    }

    public void OnBookIDConfirmed(BookData bookData, GamerID gamerID, int slotID)
    {
      if (slotID < 0 || this.MapStrategyTM.AddBookData(bookData) == null)
        return;
      this.SetBookIDInInventory(bookData, this.GetLocalPlayer(gamerID), slotID);
    }

    public void BookChanged(BookData book)
    {
      if (book == null)
        return;
      this.networkManager.SendBookUpdate(book);
    }

    private void SetBookIDInInventory(BookData book, Player player, int slotID)
    {
      if (player == null || slotID < 0)
        return;
      InventoryItem inventoryItem = player.Inventory[slotID];
      if (inventoryItem.ItemID != Item.Book)
        return;
      inventoryItem.Durability = book.ID;
      player.Inventory[slotID] = inventoryItem;
      this.RaiseBookIDConfirmed(player, book, slotID);
    }

    public void UpdateBookDetails(ushort bookID, string title, string[] text)
    {
      bool flag = false;
      BookData book = this.GetBookData(bookID);
      if (book == null)
      {
        book = new BookData();
        flag = true;
      }
      book.ID = bookID;
      book.Title = title;
      book.Text = text;
      if (!flag)
        return;
      this.MapStrategyTM.AddBookData(book);
    }

    public GameScreen ReadBook(Player player, GlobalPoint3D p)
    {
      BookBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as BookBlock;
      if (dataBlock == null)
        return (GameScreen) null;
      BookCoverScreen bookCoverScreen = new BookCoverScreen(this, player, this.GetBookData(dataBlock.ID), -1);
      this.AddScreen((GameScreen) bookCoverScreen, player);
      Sounds.PlaySound(Item.Book, ItemSoundType.Use);
      return (GameScreen) bookCoverScreen;
    }

    public bool CopyBook(Player player, GlobalPoint3D p)
    {
      BookBlock dataBlock = this.MapStrategyTM.GetDataBlock(p) as BookBlock;
      if (dataBlock != null)
        return this.CopyBook(player, dataBlock.ID);
      return false;
    }

    public bool CopyBook(Player player, ushort bookID)
    {
      int index = player.Inventory.FindItem(Item.Book, 1, (ushort) 1);
      if (index >= 0)
      {
        InventoryItem inventoryItem = player.Inventory[index];
        inventoryItem.Durability = bookID;
        player.Inventory[index] = inventoryItem;
        return true;
      }
      this.AddScreen((GameScreen) new MessageBoxScreenTM("You must have an empty book in your inventory to copy to", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), player);
      return false;
    }

    private Item SelectRandomRare(byte level)
    {
      this.tempRares.Clear();
      foreach (RareDataXML rareDataXml in Globals1.RareData)
      {
        if ((int) rareDataXml.Level == (int) level)
          this.tempRares.Add(rareDataXml.ItemID);
      }
      if (this.tempRares.Count <= 0)
        return Item.None;
      return this.tempRares[this.Random.Next(this.tempRares.Count)];
    }

    public bool CreateSliderBlock(
      GlobalPoint3D p,
      GamerID playerID,
      UpdateBlockMethod method,
      bool transmit)
    {
      if (this.map == null)
        return false;
      byte blockId = this.map.GetBlockID(p);
      byte textureIndex = this.map.UsesBlockTextureTable((Block) blockId) ? this.map.GetAuxHighData(p) : (byte) 0;
      int num = (int) this.map.ClearBlock(p, method, playerID, transmit);
      this.particleManager.AddNew(ParticleType.None, 100f, this.map.GetPosition(p), Vector3.Zero, this.map.TileSize, new InventoryItem((Item) blockId, 1), this.ParticleModifiers.SliderParticleModifier, 0.0f, -1, textureIndex, playerID, false, true);
      if (transmit)
        this.networkManager.SendSlider(p, playerID, method);
      return true;
    }

    public void SuspendRumble()
    {
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
        (localGamer.Tag as Player)?.SuspendRumble();
    }

    public void ResumeRumble()
    {
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
        (localGamer.Tag as Player)?.ResumeRumble();
    }

    public List<SavePlayerState> BuildPlayerSaveStates()
    {
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null)
          MapSaver.BuildPlayerData(this, tag);
      }
      return this.playerSaveState;
    }

    public void RefreshPlayerSaveStats()
    {
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null)
          this.GetPlayerStateData(tag).Statistics = tag.Statistics.Clone();
      }
    }

    public void FreeupMemoryForSave()
    {
      ThreadQueueManager.Instance.SuspendWorkerThreadsAndWait(10000);
      this.UnloadChunkMeshesAndCaches((StudioForge.BlockWorld.Map) this.map);
      GC.WaitForPendingFinalizers();
      ThreadQueueManager.Instance.ResumeWorkerThreads();
    }

    private void UnloadChunkMeshesAndCaches(StudioForge.BlockWorld.Map map)
    {
      foreach (MapRegion mapRegion in map.Regions.Values)
      {
        for (int index = 0; index < mapRegion.Chunks.Length; ++index)
          (mapRegion.Chunks[index] as MapChunkTM)?.UnloadContent();
      }
    }

    public void ResetPlayerSpawnPoints()
    {
      for (int index = 0; index < this.playerSaveState.Count; ++index)
      {
        SavePlayerState savePlayerState = this.playerSaveState[index];
        savePlayerState.Position = new Vector3(-1f, -1f, -1f);
        savePlayerState.Permission = Globals2.DefaultPermission;
      }
    }

    public void WorldRateReceived(byte stars, NetworkGamer gamer)
    {
      SavePlayerState playerStateData = this.GetPlayerStateData(gamer.Gamertag);
      if (playerStateData == null)
        return;
      NetworkGamer host = this.networkManager.Session.Host;
      if (playerStateData.RatingStars == (byte) 0 && host.IsLocal)
        (host.Tag as Player)?.Raise_WorldRated((Player) null);
      this.networkManager.UpdateLiveRating((int) stars - (int) playerStateData.RatingStars, playerStateData.RatingStars == (byte) 0);
      playerStateData.RatingStars = stars;
    }

    public void WorldFavoriteReceived(NetworkGamer gamer)
    {
      if (!(gamer.Tag is Player))
        return;
      NetworkGamer host = this.networkManager.Session.Host;
      if (!host.IsLocal)
        return;
      (host.Tag as Player)?.Raise_WorldFavorited(gamer.Tag as Player);
    }

    public void AddNotification(string message, NotifyRecipient recType)
    {
      this.AddNotification((Player) null, message, recType);
    }

    public void AddNotification(Player player, string message, NotifyRecipient recType)
    {
      this.AddNotification(player, message, new Color?(), recType, recType, (string) null);
    }

    public void AddNotification(
      Player player,
      string message,
      Color? color,
      NotifyRecipient recType,
      NotifyRecipient origRecType,
      string clanName)
    {
      if (!this.IsMapActive || recType == NotifyRecipient.None)
        return;
      if (player != null && player.Gamer != null)
        message = player.DisplayGamertag + message;
      if (recType != NotifyRecipient.Remote)
      {
        bool flag1 = false;
        bool flag2 = (recType & NotifyRecipient.Admin) > NotifyRecipient.None;
        if ((recType & NotifyRecipient.Local) > NotifyRecipient.None || (recType & NotifyRecipient.Clan) > NotifyRecipient.None && clanName.IsNotEmpty())
          flag1 = true;
        else if (flag2)
          flag1 = this.networkManager.HasLocalAdminPlayer();
        if (flag1)
        {
          if (color.HasValue)
            TotalMinerGame.Instance.AddNotification(message, color.Value, false);
          else
            TotalMinerGame.Instance.AddNotification(message, false);
          this.AddMessageToChatLog(origRecType, clanName, message);
        }
      }
      if (this.networkManager == null || (recType & (NotifyRecipient.Remote | NotifyRecipient.Admin | NotifyRecipient.Clan)) <= NotifyRecipient.None)
        return;
      this.networkManager.SendNotification(message, recType, clanName, (NetworkGamer) null);
    }

    public void AddMessageToChatLog(NotifyRecipient recType, string clanName, string message)
    {
      bool flag1 = (recType & NotifyRecipient.Admin) > NotifyRecipient.None;
      bool flag2 = (recType & NotifyRecipient.Global) == NotifyRecipient.Global;
      bool flag3 = !flag1 && (recType & NotifyRecipient.Clan) > NotifyRecipient.None && clanName != null && clanName.Length > 0;
      this.ChatLog.Add("[Notify To " + (flag1 ? "Admins" : (flag3 ? clanName : (flag2 ? "All" : "Local"))) + "] " + message);
    }

    public void AddHostNotification(Player player, string message)
    {
      if (!this.IsMapActive)
        return;
      if (player != null)
        message = player.DisplayGamertag + message;
      if (!this.networkManager.IsHost)
        this.networkManager.SendNotification(message, NotifyRecipient.Remote, (string) null, this.networkManager.Session.Host);
      else
        TotalMinerGame.Instance.AddNotification(message, false);
    }

    public void RemoteMachineJoined(NetworkMachine machine)
    {
    }

    public void RegisterDamage(
      DamageType damageType,
      float damage,
      GamerID victumID,
      GamerID attackerID,
      Item weaponID,
      GlobalPoint3D? healthBlock)
    {
      if (healthBlock.HasValue)
      {
        HealthBlock dataBlock = this.MapStrategyTM.GetDataBlock(healthBlock.Value) as HealthBlock;
        if (dataBlock == null)
          return;
        double damageAndDisplay = (double) dataBlock.TakeDamageAndDisplay(this, damageType, damage, this.GetCharacter(attackerID), weaponID, SkillType.Attack);
      }
      else
      {
        Actor character = this.GetCharacter(victumID);
        if (character == null)
          return;
        double damageAndDisplay = (double) character.TakeDamageAndDisplay(damageType, damage, Vector3.Zero, this.GetCharacter(attackerID), weaponID, SkillType.Attack);
      }
    }

    public void KillConfirm(
      DamageType damageType,
      GamerID victumID,
      GamerID victumAttackTargetID,
      GamerID attackerID,
      Item weaponID)
    {
      Player localPlayer = this.GetLocalPlayer(attackerID);
      Actor character = this.GetCharacter(victumID);
      Player player = this.GetPlayer(victumAttackTargetID);
      if (localPlayer == null || character == null)
        return;
      if (character is Player)
      {
        ++localPlayer.Statistics.TotalKills;
        ++localPlayer.Statistics.PlayerKills;
      }
      else if (character is NpcBase)
      {
        ++localPlayer.Statistics.TotalKills;
        ++localPlayer.Statistics.NPCKills;
      }
      localPlayer.Raise_KillCharacter(character, (Actor) player, weaponID);
    }

    public void RemoteIsLoaded()
    {
      if (!this.IsHost)
        return;
      Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
      List<NetworkManager.DataBlockChange> blocks = new List<NetworkManager.DataBlockChange>(100);
      lock (dataBlocks)
      {
        foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
        {
          if (keyValuePair.Value.ClassType == DataBlockType.Furnace)
          {
            NetworkManager.DataBlockChange dataBlockChange = new NetworkManager.DataBlockChange()
            {
              DataBlock = keyValuePair.Value,
              IsClosed = !this.IsBlockOpen(keyValuePair.Value.Point).IsGamer,
              Method = UpdateBlockMethod.Strategy
            };
            blocks.Add(dataBlockChange);
          }
        }
      }
      this.networkManager.SendDataBlockChanges(blocks);
    }

    public bool HitSwitch(GlobalPoint3D p, UpdateBlockMethod method, Player player, bool transmit)
    {
      bool flag = this.SetSwitch(p, !this.MapStrategyTM.IsBlockDeliveringPower(p), method, player, transmit);
      Sounds.PlaySound((Item) this.map.GetBlockIDNoCache(p), flag ? ItemSoundType.Use : ItemSoundType.UseFail, p, (ITMActor) player);
      return flag;
    }

    public bool HitButton(GlobalPoint3D p, UpdateBlockMethod method, Player player, bool transmit)
    {
      bool flag = this.SetSwitch(p, true, method, player, transmit);
      Sounds.PlaySound((Item) this.map.GetBlockIDNoCache(p), flag ? ItemSoundType.Use : ItemSoundType.UseFail, p, (ITMActor) player);
      return flag;
    }

    public void ReleaseButton(GlobalPoint3D p, UpdateBlockMethod method, GamerID playerID)
    {
      if (!this.IsHost)
        return;
      this.SetSwitch(p, false, method, playerID, true);
    }

    public bool SetSwitch(
      GlobalPoint3D p,
      bool power,
      UpdateBlockMethod method,
      Player player,
      bool transmit)
    {
      MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
      return this.DeliverPower(p, (Block) blockIdAndAuxNoCache.BlockID, (BlockFace) ((uint) blockIdAndAuxNoCache.AuxData & 7U), power, method, Player.GetGamerID(player), transmit, player != null);
    }

    public bool SetSwitch(
      GlobalPoint3D p,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
      return this.DeliverPower(p, (Block) blockIdAndAuxNoCache.BlockID, (BlockFace) ((uint) blockIdAndAuxNoCache.AuxData & 7U), power, method, playerID, transmit, false);
    }

    private void GamerJoinedEventHandler(object sender, GamerEventArgs e)
    {
      if (this.networkManager == null)
        TotalMinerGame.Assert("networkManager null reference");
      else if (Globals2.GameProperties == null)
        TotalMinerGame.Assert("Globals2.GameProperties null reference");
      else if (Globals2.GameProperties.SaveGame == null)
      {
        TotalMinerGame.Assert("Globals2.GameProperties.SaveGame null reference");
      }
      else
      {
        Player tag1 = e.Gamer.Tag as Player;
        if (tag1 == null || !this.networkManager.IsSessionOpen)
          return;
        this.AddPlayer(tag1);
        if (this.networkManager.AllGamerCount > Globals2.MaxConcurrentPlayers)
          Globals2.MaxConcurrentPlayers = this.networkManager.AllGamerCount;
        if (e.Gamer.IsLocal)
        {
          tag1.SetVoiceState();
          StudioForge.TotalMiner.Screens.BackgroundScreen backgroundScreen = new StudioForge.TotalMiner.Screens.BackgroundScreen();
          this.AddScreen((GameScreen) backgroundScreen, tag1);
          this.AddScreen((GameScreen) new LoadingPlayerViewScreen(tag1, this.chunkLoader, (GameScreen) backgroundScreen, true), tag1);
          this.networkManager.SendPlayerSkills(tag1, (NetworkGamer) null, true, false);
          if (this.IsLocalSkills)
            this.networkManager.SendPlayerSkills(tag1, (NetworkGamer) null, !this.IsHost, true);
        }
        else
        {
          this.AddNotification(tag1, " has joined the game", NotifyRecipient.Local);
          if (this.IsHost)
          {
            Player tag2 = this.networkManager.Session.Host.Tag as Player;
            if (tag2 != null && this.networkManager.Session.Host != e.Gamer)
              tag2.Raise_WorldVisited(e.Gamer.Tag as Player);
          }
        }
        foreach (Mod activePlugin in ModManager.ActivePlugins)
          activePlugin.Plugin.PlayerJoined((ITMPlayer) tag1);
      }
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      Player tag = e.Gamer.Tag as Player;
      if (tag == null)
        return;
      foreach (Mod activePlugin in ModManager.ActivePlugins)
        activePlugin.Plugin.PlayerLeft((ITMPlayer) tag);
      tag.ClearRumble();
      this.RemovePlayer(tag, true, false);
      if (this.MapStrategyTM != null)
        this.MapStrategyTM.GamerLeft(e.Gamer);
      if (tag.Gamer.IsLocal)
      {
        if (StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance != null)
          StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager.RemoveScreens(tag.PlayerIndex);
      }
      else
        this.AddNotification(tag, " has left the game", NotifyRecipient.Local);
      tag.UnloadContent();
      tag.Gamer.Tag = (object) null;
    }

    public List<string> GetListOfClansThatHaveHistory()
    {
      List<string> stringList = new List<string>();
      if (this.clanHistory != null && this.clanHistory.Count > 0)
      {
        foreach (KeyValuePair<string, History> keyValuePair in this.clanHistory)
          stringList.Add(keyValuePair.Key);
      }
      return stringList;
    }

    public List<string> GetListOfClansInTheGame()
    {
      List<string> stringList = new List<string>();
      foreach (Gamer allEnabledGamer in this.networkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null && tag.ClanName.IsNotEmpty() && !stringList.Contains(tag.ClanName))
          stringList.Add(tag.ClanName);
      }
      return stringList;
    }

    public History GetClanHistory(string clanName)
    {
      if (clanName == null || clanName.Length < 1)
        return (History) null;
      if (this.clanHistory == null)
        return (History) null;
      History history = (History) null;
      this.clanHistory.TryGetValue(clanName, out history);
      return history;
    }

    public History GetOrCreateClanHistory(string clanName)
    {
      if (clanName == null || clanName.Length < 1)
        return (History) null;
      if (this.clanHistory == null)
        this.clanHistory = new Dictionary<string, History>();
      History history = (History) null;
      if (this.clanHistory.TryGetValue(clanName, out history))
        return history;
      history = new History();
      this.clanHistory.Add(clanName, history);
      return history;
    }

    public void StartCaveIn(GlobalPoint3D origin, int seed, bool transmit)
    {
      this.cavein.StartNewCaveIn(origin, seed, transmit);
    }

    public void StartDeathmatch(
      Player startedBy,
      DeathmatchWinType winType,
      bool eating,
      bool transmit)
    {
      this.AbortMiniGame(transmit);
      this.MiniGame = (IMiniGame) new DeathmatchMiniGame(winType, eating);
      this.MiniGame.Start(this, startedBy);
      if (!transmit)
        return;
      this.networkManager.SendDeathmatchStart(winType, eating, startedBy.Gamer.ID);
    }

    public void AbortMiniGame(bool transmit)
    {
      if (this.MiniGame == null)
        return;
      if (transmit)
        this.networkManager.SendMiniGameAbort();
      this.AddNotification(Utils.InsertSpacesBeforeCapitals(this.MiniGame.GameType.ToString()) + " has been aborted", NotifyRecipient.Local);
      this.MiniGame.Abort();
      this.MiniGame = (IMiniGame) null;
    }

    public void FloodPhysics(GlobalPoint3D p, Block blockID, GamerID playerID, bool transmit)
    {
      switch (this.map.GetClearBlockResult(p, UpdateBlockMethod.Flood, playerID))
      {
        case ClearBlockResult.Success:
        case ClearBlockResult.AlreadyClear:
          this.map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Flood, playerID, true);
          Player player = this.GetPlayer(playerID);
          if (player != null)
            player.HasAbortedFloods = false;
          FloodFill floodFill = new FloodFill();
          floodFill.Initialize(PriorityLevel.Normal, this, this.map, p, blockID, UpdateBlockMethod.Flood, playerID);
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) floodFill, false, PriorityLevel.Normal);
          if (!transmit)
            break;
          this.networkManager.SendFlood(p, blockID, playerID);
          break;
      }
    }

    public void StartLiveFire(
      GlobalPoint3D fuelPoint,
      Block fuelBlockID,
      GlobalPoint3D firePoint,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      if (ItemData2.GetBurnTime(this.map, fuelPoint, (Item) fuelBlockID) <= (ushort) 0)
        return;
      bool flag = true;
      Player player = (Player) null;
      if (method == UpdateBlockMethod.Player)
      {
        player = this.GetPlayer(gamerID);
        if (player != null)
          flag = player.HasPermission(Permissions.Grief);
      }
      if (!flag)
        return;
      switch (this.Map.GetClearBlockResult(firePoint, method, gamerID))
      {
        case ClearBlockResult.Success:
        case ClearBlockResult.AlreadyClear:
          this.map.SetBlockData(firePoint, (byte) 118, (byte) 1, method, gamerID, transmit);
          this.map.Commit();
          Sounds.PlaySound(Item.Fire, ItemSoundType.Use, firePoint, (ITMActor) player);
          break;
      }
    }

    public void TreeRemoved(GlobalPoint3D p)
    {
      if (this.floraManager == null)
        return;
      this.floraManager.TreeRemoved(p);
    }

    public Zone EditZone(
      string name,
      GamerID gamerID,
      ZoneEditType action,
      ZoneType type,
      GlobalPoint3D? min,
      GlobalPoint3D? max,
      ZoneBuilderType builderType,
      string builder,
      string onEntryScript,
      string onExitScript,
      short combatLevelDifference,
      float speedModifier,
      float gravityModifier)
    {
      Zone zone = this.MapStrategyTM.GetZone(name, gamerID);
      if (zone == null)
      {
        if (action == ZoneEditType.AddOrEdit)
        {
          zone = new Zone(name, type, min.Value, max.Value);
          zone.GamerID = gamerID;
          zone.Builder = builder;
          zone.BuilderType = builderType;
          zone.OnEntryScriptName = onEntryScript;
          zone.OnExitScriptName = onExitScript;
          zone.CombatLevelDifference = combatLevelDifference;
          zone.SpeedMultiplier = speedModifier;
          zone.GravityMultiplier = gravityModifier;
          this.MapStrategyTM.AddZone(zone);
        }
      }
      else
      {
        switch (action)
        {
          case ZoneEditType.AddOrEdit:
            zone.ZoneType = type;
            if (min.HasValue)
              zone.Min = min.Value;
            if (max.HasValue)
              zone.Max = max.Value;
            zone.Builder = builder;
            zone.BuilderType = builderType;
            zone.OnEntryScriptName = onEntryScript;
            zone.OnExitScriptName = onExitScript;
            zone.CombatLevelDifference = combatLevelDifference;
            zone.SpeedMultiplier = speedModifier;
            zone.GravityMultiplier = gravityModifier;
            break;
          case ZoneEditType.Delete:
            this.DeleteZone(zone);
            break;
        }
      }
      return zone;
    }

    public void DeleteZone(Zone zone)
    {
      this.MapStrategyTM.RemoveZone(zone);
      this.proximityChecker.ZoneDeleted(zone);
    }

    public void DeleteZone(string name)
    {
      this.DeleteZone(name, GamerID.Sys1);
    }

    public void DeleteZone(string name, GamerID gamerID)
    {
      this.EditZone(name, gamerID, ZoneEditType.Delete, ZoneType.None, new GlobalPoint3D?(), new GlobalPoint3D?(), ZoneBuilderType.None, (string) null, (string) null, (string) null, (short) 0, 0.0f, 0.0f);
    }

    public bool IsInZoneType(GlobalPoint3D p, ZoneType type, GamerID gamerID)
    {
      MapStrategyTM mapStrategyTm = this.MapStrategyTM;
      if (mapStrategyTm == null)
        return false;
      return mapStrategyTm.IsInZoneType(p, type, gamerID);
    }

    public bool IsInZoneType(GlobalPoint3D min, GlobalPoint3D max, ZoneType type, GamerID gamerID)
    {
      MapStrategyTM mapStrategyTm = this.MapStrategyTM;
      if (mapStrategyTm == null)
        return false;
      return mapStrategyTm.IsInZoneType(min, max, type, gamerID);
    }

    public bool IsInZoneType(BoundingBox box, ZoneType type, GamerID gamerID)
    {
      MapStrategyTM mapStrategyTm = this.MapStrategyTM;
      if (mapStrategyTm == null)
        return false;
      return mapStrategyTm.IsInZoneType(box, type, gamerID);
    }

    public bool IsInZone(BoundingBox box, string zoneName)
    {
      MapStrategyTM mapStrategyTm = this.MapStrategyTM;
      if (mapStrategyTm == null)
        return false;
      return mapStrategyTm.IsInZone(box, zoneName);
    }

    public void TeleportEntities(
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D dest,
      bool relative)
    {
      Vector3 position1 = this.map.GetPosition(min);
      position1.Y -= this.map.TileSize;
      Vector3 position2 = this.map.GetPosition(max);
      position2.X += this.map.TileSize;
      position2.Z += this.map.TileSize;
      BoundingBox boundingBox = new BoundingBox(position1, position2);
      Vector3 blockCenter = this.map.GetBlockCenter(dest);
      blockCenter.Y -= this.map.HalfTileSize - this.map.HalfTileSize * 0.1f;
      Vector3 vector3_1 = new Vector3(this.map.TileSize);
      foreach (Actor actor in this.actorList)
      {
        if (actor != null && boundingBox.Contains(actor.Box) != ContainmentType.Disjoint)
        {
          Vector3 pos = blockCenter;
          if (relative)
          {
            Vector3 vector3_2 = actor.Position - position1;
            vector3_2.X -= this.map.HalfTileSize;
            vector3_2.Z -= this.map.HalfTileSize;
            pos += vector3_2;
          }
          actor.TeleportTo(pos);
        }
      }
    }

    public void QueueGameDataRequest(NetworkGamer requester)
    {
      SendGameData sendGameData = new SendGameData();
      sendGameData.Initialize(this, requester);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) sendGameData, false, PriorityLevel.Normal);
    }

    public void ClearBlockInventory(GlobalPoint3D p)
    {
      (this.MapStrategyTM.GetDataBlock(p) as ChestBlock)?.Inventory.ClearItems();
    }

    public void AddToBlockInventory(GlobalPoint3D p, Block blockID, Item itemID, int count)
    {
      if (!this.IsInventoryBlock(blockID))
        return;
      ChestBlock chestBlock = this.MapStrategyTM.GetDataBlock(p) as ChestBlock;
      bool flag = false;
      if (chestBlock == null)
      {
        chestBlock = (ChestBlock) this.MapStrategyTM.NewDataBlock(p, blockID, GamerID.Sys1);
        flag = true;
      }
      chestBlock.Inventory.AddToInventory(itemID, count);
      if (!flag || !chestBlock.Inventory.HasItems())
        return;
      this.MapStrategyTM.AddDataBlock((DataBlock) chestBlock, UpdateBlockMethod.Strategy, true);
    }

    public bool IsInventoryBlock(Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 50U)
      {
        if (block != Block.Bookcase && block != Block.Chest)
          goto label_4;
      }
      else if (block != Block.LockedChest && block != Block.Crate && block != Block.Safe)
        goto label_4;
      return true;
label_4:
      return false;
    }

    public void FlagMapChunksForMeshReload()
    {
      foreach (MapRegion mapRegion in this.map.Regions.Values)
      {
        foreach (MapChunk chunk in mapRegion.Chunks)
          chunk?.SetChunkFlag(ChunkFlags.MeshDirty);
      }
    }

    private void UpdateMarkers()
    {
      lock (this.GraveMarkers)
      {
        for (int index = this.GraveMarkers.Count - 1; index >= 0; --index)
        {
          if ((double) this.GraveMarkers[index].Time < (double) Services.TotalTime - 420.0)
            this.GraveMarkers.RemoveAt(index);
        }
      }
    }

    public void AddMapMarker(GlobalPoint3D p, string text, MapMarkerType type, bool transmit)
    {
      if (this.MapMarkers == null || !this.map.IsValidPoint(p))
        return;
      if (transmit)
        this.networkManager.SendTopMapMarkerUpdate(p, text, type);
      List<MapMarker> mapMarkerList = type == MapMarkerType.Graveyard ? this.GraveMarkers : this.MapMarkers;
      lock (mapMarkerList)
      {
        for (int index = 0; index < mapMarkerList.Count; ++index)
        {
          MapMarker mapMarker = mapMarkerList[index];
          if (mapMarker.Point == p)
          {
            mapMarker.Label = text;
            mapMarker.Type = type;
            mapMarkerList[index] = mapMarker;
            return;
          }
        }
        mapMarkerList.Add(new MapMarker()
        {
          Point = p,
          Label = text,
          Type = type,
          Time = Services.TotalTime
        });
      }
    }

    public void RemoveMapMarker(GlobalPoint3D p, bool transmit)
    {
      this.RemoveMapMarker(this.GetMapMarkerIndex(p), transmit);
    }

    public void RemoveMapMarker(string name, bool transmit)
    {
      this.RemoveMapMarker(this.GetMapMarkerIndex(name), transmit);
    }

    public void RemoveMapMarker(int i, bool transmit)
    {
      if (i < 0)
        return;
      GlobalPoint3D? nullable = new GlobalPoint3D?();
      lock (this.MapMarkers)
      {
        if (i <= this.MapMarkers.Count)
        {
          nullable = new GlobalPoint3D?(this.MapMarkers[i].Point);
          this.MapMarkers.RemoveAt(i);
        }
      }
      if (!transmit || !nullable.HasValue)
        return;
      this.networkManager.SendTopMapMarkerRemove(nullable.Value);
    }

    public MapMarker? GetMapMarker(GlobalPoint3D p)
    {
      int mapMarkerIndex = this.GetMapMarkerIndex(p);
      if (mapMarkerIndex < 0)
        return new MapMarker?();
      return new MapMarker?(this.MapMarkers[mapMarkerIndex]);
    }

    public int GetMapMarkerIndex(GlobalPoint3D p)
    {
      int num = 5;
      for (int index = this.MapMarkers.Count - 1; index >= 0; --index)
      {
        GlobalPoint3D point = this.MapMarkers[index].Point;
        if (p.X >= point.X - num && p.X < point.X + num && (p.Z >= point.Z - num && p.Z < point.Z + num))
          return index;
      }
      return -1;
    }

    public int GetMapMarkerIndex(string label)
    {
      for (int index = 0; index < this.MapMarkers.Count; ++index)
      {
        if (this.MapMarkers[index].Label.Equals(label, StringComparison.OrdinalIgnoreCase))
          return index;
      }
      return -1;
    }

    public MapMarker? GetMapMarker(string label)
    {
      if (label != null && label.Length > 0)
      {
        lock (this.MapMarkers)
        {
          foreach (MapMarker mapMarker in this.MapMarkers)
          {
            if (mapMarker.Label.Equals(label, StringComparison.OrdinalIgnoreCase))
              return new MapMarker?(mapMarker);
          }
        }
      }
      return new MapMarker?();
    }

    public void TakeOwnershipOfShop(Player player, GlobalPoint3D p)
    {
      switch ((Block) this.map.GetBlockID(p))
      {
        case Block.ItemShop:
        case Block.BlockShop:
          if (this.map.GetAuxData(p) != (byte) 0)
            break;
          this.map.SetAuxData(p, (byte) 0, (byte) 1, UpdateBlockMethod.Player, player.GamerID, true);
          this.AddScreen((GameScreen) new MessageBoxScreenTM("This shop is now yours\nIt has no stock\nYou can stock it up with items from your inventory\nYou can set the prices of the items\nYou can retrieve the proceeds from the shop at any time\n\nOther players can sell to the shop only if the\nshop has enough gold to buy the items", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), player);
          break;
      }
    }

    public void InstantiateBlock(Player player, GlobalPoint3D p)
    {
      if (this.map.GetBlockID(p) != (byte) 50 || this.map.GetAuxData(p) != (byte) 0)
        return;
      this.map.SetAuxData(p, (byte) 0, (byte) 1, UpdateBlockMethod.Player, player.GamerID, true);
      this.AddScreen((GameScreen) new MessageBoxScreenTM("This chest is now instanced.\n\nEach (non admin) player has their own instance of this\nchests inventory, so if they remove items from the chest\nthe items will still be there for other players.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), player);
    }

    public void SetPower(GlobalPoint3D p, bool power, GamerID playerID)
    {
      this.MapStrategyTM.SignalPointExternal(new SignalData()
      {
        Point = p,
        Power = power,
        Flow = false,
        Method = UpdateBlockMethod.Strategy,
        PlayerID = playerID
      });
    }

    public bool DeliverPower(
      GlobalPoint3D p,
      Block blockID,
      BlockFace face,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit,
      bool needResultNow)
    {
      if (!transmit || this.IsHost)
        return this.MapStrategyTM.DeliverPower(p, blockID, face, power, method, playerID, transmit);
      this.networkManager.SendPowerDeliver(p, blockID, face, power, method, playerID);
      return true;
    }

    private void UpdateAutoSave()
    {
      this.autoSaveInProgressTimer -= Services.ElapsedTime;
      if (Globals2.GameSettings.AutoSave == AutoSaveSetting.None)
        return;
      this.autoSaveTimer -= Services.ElapsedTime;
      if ((double) this.autoSaveTimer >= 0.0)
        return;
      this.StartAutoSave();
    }

    public void OnAutoSaveChanged()
    {
      float newAutoSaveTime = MapSaveWorker.GetNewAutoSaveTime();
      if ((double) newAutoSaveTime <= 0.0 || (double) newAutoSaveTime >= (double) this.autoSaveTimer)
        return;
      this.autoSaveTimer = newAutoSaveTime;
    }

    private void StartAutoSave()
    {
      this.autoSaveInProgressTimer = 0.0f;
      this.autoSaveTimer = MapSaveWorker.GetNewAutoSaveTime();
      this.AddNotification("Auto Save initiated", NotifyRecipient.Local);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.autoSaver, true, PriorityLevel.Normal);
    }

    private void OnAutoSaveComplete(bool success, bool otherDiskActivityInProgress)
    {
      if (success)
        this.AddNotification("Auto Save successful", NotifyRecipient.Local);
      else if (otherDiskActivityInProgress)
        this.AddNotification("Auto Save failed - other disk activity is in progress", NotifyRecipient.Local);
      else
        this.AddNotification("Auto Save failed", NotifyRecipient.Local);
    }

    public void AddHistory(string key, GamerID gamerID)
    {
      if (gamerID.IsGamer)
        this.GetPlayer(gamerID)?.History.AddHistory(key);
      else
        this.History.AddHistory(key);
    }

    public History GetHistory(GamerID gamerID)
    {
      if (!gamerID.IsGamer)
        return this.History;
      return this.GetPlayer(gamerID)?.History;
    }

    public void AddActionLog(GamerID gamerID, Item itemID, ItemAction action)
    {
      this.GetPlayer(gamerID)?.ActionLog.AddAction(itemID, action);
    }

    public Script GetScript(string scriptName)
    {
      if (scriptName != null && scriptName.Length > 0 && this.scripts != null)
      {
        lock (this.scripts)
        {
          foreach (Script script in this.scripts)
          {
            if (script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase) || script.Alias.Equals(scriptName, StringComparison.OrdinalIgnoreCase))
              return script;
          }
        }
      }
      return (Script) null;
    }

    public Script GetScript(int editID)
    {
      if (this.scripts != null)
      {
        lock (this.scripts)
        {
          foreach (Script script in this.scripts)
          {
            if (script.EditID == editID)
              return script;
          }
        }
      }
      return (Script) null;
    }

    public bool ExecuteEventScript(ScriptEvent e, ScriptExecuteData data)
    {
      Script eventScript = this.GetEventScript(e);
      if (eventScript == null)
        return false;
      bool transmit = e == ScriptEvent.PlayerJoin || e == ScriptEvent.PlayerLeave;
      this.ExecuteScript(eventScript, data, transmit);
      return true;
    }

    public void ExecuteItemSwingEventScript(Actor actor, Item itemID)
    {
      this.ExecuteItemEventScript(actor, this.eventScriptItemSwing, itemID);
    }

    public void ExecuteItemEquipEventScript(Actor actor, Item oldItemID, Item newItemID)
    {
      if (oldItemID == newItemID)
        return;
      if (oldItemID != Item.None && oldItemID != Item.Hand)
        this.ExecuteItemEventScript(actor, this.eventScriptItemUnequip, oldItemID);
      if (newItemID == Item.None || newItemID == Item.Hand)
        return;
      this.ExecuteItemEventScript(actor, this.eventScriptItemEquip, newItemID);
    }

    private void ExecuteItemEventScript(Actor actor, Script[] scripts, Item itemID)
    {
      Script script1 = scripts[(int) itemID];
      if (script1 != null)
      {
        this.ExecuteScript(script1, new ScriptExecuteData()
        {
          Actor = actor
        }, false);
      }
      else
      {
        Script script2 = scripts[0];
        if (script2 == null)
          return;
        this.ExecuteScript(script2, new ScriptExecuteData()
        {
          Actor = actor
        }, false);
      }
    }

    public void ExecuteScript(string scriptName, ScriptExecuteData data, bool transmit)
    {
      this.ExecuteScript(this.GetScript(scriptName), data, transmit);
    }

    public void ExecuteScript(Script script, ScriptExecuteData data, bool transmit)
    {
      if (script == null || this.IsDigDeepMode && this.IsItemLocked(Item.ScriptBlock))
        return;
      if (transmit)
        this.networkManager.SendScriptExecute(script, data);
      this.scriptRuntimeWorker.QueueScript(script, data);
    }

    public void ExecuteByteCode(byte[] byteCode)
    {
      Script script = new Script("")
      {
        ByteCode = new MemoryStream(byteCode)
      };
      script.ByteCodeReader = new BinaryReader((Stream) script.ByteCode);
      ScriptExecuteData data = new ScriptExecuteData();
      this.scriptRuntimeWorker.QueueScript(script, data);
    }

    public bool HasItemSwingEventScript(Item itemID)
    {
      if (this.eventScriptItemSwing[0] == null)
        return this.eventScriptItemSwing[(int) itemID] != null;
      return true;
    }

    public void CancelScript(string scriptName, Actor actor)
    {
      this.CancelScript(scriptName, actor, true);
    }

    public void CancelScript(string scriptName, Actor actor, bool transmit)
    {
      this.CancelScript(this.GetScript(scriptName), actor, transmit);
    }

    public bool CancelScript(Script script, Actor actor)
    {
      return this.CancelScript(script, actor, true);
    }

    private bool CancelScript(Script script, Actor actor, bool transmit)
    {
      if (script == null)
        return false;
      if (transmit)
        this.networkManager.SendScriptCancelled(script.Name);
      return this.scriptRuntimeWorker.CancelScript(script, actor);
    }

    public void DeleteScript(string scriptName)
    {
      this.DeleteScript(this.GetScript(scriptName));
    }

    public void DeleteScript(Script script)
    {
      if (script == null)
        return;
      lock (this.eventScripts)
      {
        this.RemoveEventScript(ScriptEvent.PlayerDeath, script);
        this.RemoveEventScript(ScriptEvent.PlayerJoin, script);
        this.RemoveEventScript(ScriptEvent.PlayerLeave, script);
        this.RemoveEventScript(ScriptEvent.PlayerRespawn, script);
        this.RemoveEventScript(ScriptEvent.CustomMenu, script);
      }
      lock (this.adventureScripts)
        this.adventureScripts.Remove(script);
      lock (this.scripts)
        this.scripts.Remove(script);
      lock (this.eventScriptItemSwing)
      {
        for (int index = 0; index < this.eventScriptItemSwing.Length; ++index)
        {
          if (this.eventScriptItemSwing[index] == script)
            this.eventScriptItemSwing[index] = (Script) null;
        }
      }
      lock (this.eventScriptItemEquip)
      {
        for (int index = 0; index < this.eventScriptItemEquip.Length; ++index)
        {
          if (this.eventScriptItemEquip[index] == script)
            this.eventScriptItemEquip[index] = (Script) null;
        }
      }
      lock (this.eventScriptItemUnequip)
      {
        for (int index = 0; index < this.eventScriptItemUnequip.Length; ++index)
        {
          if (this.eventScriptItemUnequip[index] == script)
            this.eventScriptItemUnequip[index] = (Script) null;
        }
      }
      this.networkManager.SendScriptDeleted(script.Name);
    }

    public void RenameScript(Script script, string oldName, string newName)
    {
      if (oldName.EndsWith("\\") && newName.EndsWith("\\"))
      {
        lock (this.scripts)
        {
          foreach (Script script1 in new List<Script>((IEnumerable<Script>) this.scripts))
          {
            if (script1.Name.StartsWith(oldName, StringComparison.OrdinalIgnoreCase))
              this.AddOrOverwriteScript(script1.Name, new Script(script1)
              {
                Name = newName + script1.Name.Substring(oldName.Length)
              }, true);
          }
        }
      }
      else
      {
        if (script == null)
          script = this.GetScript(oldName);
        this.AddOrOverwriteScript(oldName, new Script(script)
        {
          Name = newName
        }, true);
      }
    }

    public void AddOrOverwriteScript(string origName, Script script, bool transmit)
    {
      lock (this.eventScripts)
      {
        List<KeyValuePair<ScriptEvent, Script>> keyValuePairList = new List<KeyValuePair<ScriptEvent, Script>>();
        foreach (KeyValuePair<ScriptEvent, Script> eventScript in this.eventScripts)
        {
          if (eventScript.Value != null && eventScript.Value.Name.Equals(origName, StringComparison.OrdinalIgnoreCase))
            keyValuePairList.Add(new KeyValuePair<ScriptEvent, Script>(eventScript.Key, script));
        }
        foreach (KeyValuePair<ScriptEvent, Script> keyValuePair in keyValuePairList)
          this.eventScripts[keyValuePair.Key] = keyValuePair.Value;
      }
      lock (this.adventureScripts)
      {
        for (int index = 0; index < this.adventureScripts.Count; ++index)
        {
          if (this.adventureScripts[index].Name.Equals(origName, StringComparison.OrdinalIgnoreCase))
          {
            this.adventureScripts[index] = script;
            break;
          }
        }
      }
      lock (this.scripts)
      {
        bool flag = false;
        for (int index = 0; index < this.scripts.Count; ++index)
        {
          if (this.scripts[index].Name.Equals(origName, StringComparison.OrdinalIgnoreCase))
          {
            script.IsChanged = true;
            this.scripts[index] = script;
            flag = true;
            break;
          }
        }
        if (!flag)
          this.scripts.Add(script);
      }
      for (int index = 0; index < this.eventScriptItemSwing.Length; ++index)
      {
        if (this.eventScriptItemSwing[index] != null && this.eventScriptItemSwing[index].Name == origName)
          this.eventScriptItemSwing[index] = script;
      }
      for (int index = 0; index < this.eventScriptItemEquip.Length; ++index)
      {
        if (this.eventScriptItemEquip[index] != null && this.eventScriptItemEquip[index].Name == origName)
          this.eventScriptItemEquip[index] = script;
      }
      for (int index = 0; index < this.eventScriptItemUnequip.Length; ++index)
      {
        if (this.eventScriptItemUnequip[index] != null && this.eventScriptItemUnequip[index].Name == origName)
          this.eventScriptItemUnequip[index] = script;
      }
      if (origName != script.Name)
      {
        Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
        lock (dataBlocks)
        {
          foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
            keyValuePair.Value.RenameScript(origName, script.Name);
        }
        ScriptCompiler scriptCompiler = new ScriptCompiler(this);
        foreach (Script script1 in this.scripts)
        {
          if (script1 != script)
            scriptCompiler.RenameReferencedScript(script1, origName, script.Name);
        }
      }
      if (!transmit)
        return;
      this.NetworkManager.SendScriptEdited(origName, script);
    }

    private void RemoveEventScript(ScriptEvent e, Script script)
    {
      lock (this.eventScripts)
      {
        if (!this.eventScripts.ContainsKey(e) || this.eventScripts[e] != script)
          return;
        this.eventScripts.Remove(e);
      }
    }

    public void ReceiveScriptIntersectResult(string name, GamerID gamerID, GamerID targetID)
    {
      if (this.scriptRuntimeWorker == null)
        return;
      this.scriptRuntimeWorker.PostIntersectResult(name, gamerID, targetID);
    }

    public void ReceiveScriptInputResult(string name, GamerID gamerID, double? val)
    {
      if (this.scriptRuntimeWorker == null)
        return;
      this.scriptRuntimeWorker.PostInputResult(name, gamerID, val);
    }

    public void OpenNumberInput(
      Player player,
      double defaultValue,
      NumberEntered callback,
      object state)
    {
      if (player == null || !player.IsLocalGamer)
        return;
      this.AddScreen((GameScreen) new NumberEntryScreen(player, callback, defaultValue, true, true, state), player);
    }

    public void OpenMessageBox(
      Player player,
      string message,
      string aText,
      string aScript,
      GlobalPoint3D? aPoint,
      string xText,
      string xScript,
      GlobalPoint3D? xPoint,
      string yText,
      string yScript,
      GlobalPoint3D? yPoint,
      string bText)
    {
      if (player == null || !player.IsLocalGamer)
        return;
      bool disableCancel = bText.IsEmpty();
      if (aText.IsEmpty() && bText.IsEmpty() && (xText.IsEmpty() && yText.IsEmpty()))
      {
        aText = "Ok";
        disableCancel = false;
      }
      this.AddScreen((GameScreen) new MessageBoxScreenTMScript(player, message, aText, aScript, aPoint, xText, xScript, xPoint, yText, yScript, yPoint, bText, disableCancel), player);
    }

    public bool IsScriptedScreenOpen(Player player)
    {
      foreach (GameScreen screen in StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager.GetScreens(new PlayerIndex?(player.PlayerIndex)))
      {
        if (screen is MessageBoxScreenTMScript || screen is ScriptedMenuScreen)
          return true;
      }
      return false;
    }

    public List<Script> GetAllUsedGlobalScripts()
    {
      List<Script> list = new List<Script>();
      this.GetAllUsedScriptsCore(list);
      for (int index = list.Count - 1; index >= 0; --index)
      {
        if (!list[index].Name.StartsWith("global\\", StringComparison.OrdinalIgnoreCase))
          list.RemoveAt(index);
      }
      return list;
    }

    private void GetAllUsedScriptsCore(List<Script> list)
    {
      this.GetAllFirstLevelScripts(list);
      ScriptCompiler scriptCompiler = new ScriptCompiler(this);
      List<string> list1 = new List<string>();
      int num = 0;
      for (int count = list.Count; num < count; count = list.Count)
      {
        for (int index = num; index < count; ++index)
        {
          list1.Clear();
          scriptCompiler.GetReferencedScripts(list[index], list1);
          foreach (string name in list1)
            this.AddScriptToList(name, list);
        }
        num = count;
      }
    }

    private void GetAllFirstLevelScripts(List<Script> list)
    {
      List<Zone> zones = this.MapStrategyTM.GetZones(ZoneType.None);
      lock (zones)
      {
        foreach (Zone zone in zones)
        {
          string onEntryScriptName = zone.OnEntryScriptName;
          if (onEntryScriptName.IsNotEmpty())
            this.AddScriptToList(onEntryScriptName, list);
          string onExitScriptName = zone.OnExitScriptName;
          if (onExitScriptName.IsNotEmpty())
            this.AddScriptToList(onExitScriptName, list);
        }
      }
      Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
      lock (dataBlocks)
      {
        foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
        {
          switch (keyValuePair.Value.ClassType)
          {
            case DataBlockType.NPCSpawn:
              NpcSpawnBlock npcSpawnBlock = keyValuePair.Value as NpcSpawnBlock;
              if (npcSpawnBlock != null && npcSpawnBlock.KillScript.IsNotEmpty())
              {
                this.AddScriptToList(npcSpawnBlock.KillScript, list);
                continue;
              }
              continue;
            case DataBlockType.Script:
              ScriptBlock scriptBlock = keyValuePair.Value as ScriptBlock;
              if (scriptBlock != null)
              {
                if (scriptBlock.PowerOnScript.IsNotEmpty())
                  this.AddScriptToList(scriptBlock.PowerOnScript, list);
                if (scriptBlock.PowerOffScript.IsNotEmpty())
                {
                  this.AddScriptToList(scriptBlock.PowerOffScript, list);
                  continue;
                }
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      lock (this.eventScripts)
      {
        foreach (KeyValuePair<ScriptEvent, Script> eventScript in this.eventScripts)
        {
          if (!list.Contains(eventScript.Value))
            list.Add(eventScript.Value);
        }
      }
      for (int index = 0; index < this.eventScriptItemSwing.Length; ++index)
      {
        Script script = this.eventScriptItemSwing[index];
        if (script != null && !list.Contains(script))
          list.Add(script);
      }
      for (int index = 0; index < this.eventScriptItemEquip.Length; ++index)
      {
        Script script = this.eventScriptItemEquip[index];
        if (script != null && !list.Contains(script))
          list.Add(script);
      }
      for (int index = 0; index < this.eventScriptItemUnequip.Length; ++index)
      {
        Script script = this.eventScriptItemUnequip[index];
        if (script != null && !list.Contains(script))
          list.Add(script);
      }
    }

    private void AddScriptToList(string name, List<Script> list)
    {
      Script script = this.GetScript(name);
      if (script == null || list.Contains(script))
        return;
      list.Add(script);
    }

    public bool IsScriptUsedInWorld(Script script)
    {
      return this.GetScriptUsedBy(script) != null;
    }

    public string GetScriptUsedBy(Script script)
    {
      lock (this.usedScriptList)
      {
        string scriptUsedBy = this.GetScriptUsedBy(new ScriptCompiler(this), script);
        this.usedScriptList.Clear();
        return scriptUsedBy;
      }
    }

    private string GetScriptUsedBy(ScriptCompiler compiler, Script script)
    {
      if (this.usedScriptList.Contains(script))
        return (string) null;
      this.usedScriptList.Add(script);
      string str = this.IsScriptAssigned(script);
      if (str != null)
        return str;
      for (int index = 0; index < this.scripts.Count; ++index)
      {
        Script script1 = this.scripts[index];
        if (script1 != script && compiler.IsScriptReferenced(script1, script) && this.GetScriptUsedBy(compiler, script1) != null)
          return "called by " + script1.Name;
      }
      return (string) null;
    }

    public string IsScriptAssigned(Script script)
    {
      List<Zone> zones = this.MapStrategyTM.GetZones(ZoneType.None);
      lock (zones)
      {
        foreach (Zone zone in zones)
        {
          string onEntryScriptName = zone.OnEntryScriptName;
          if (onEntryScriptName.IsNotEmpty() && (onEntryScriptName == script.Name || onEntryScriptName == script.Alias))
            return "a Zone";
          string onExitScriptName = zone.OnExitScriptName;
          if (onExitScriptName.IsNotEmpty() && (onExitScriptName == script.Name || onExitScriptName == script.Alias))
            return "a Zone";
        }
      }
      Dictionary<long, DataBlock> dataBlocks = this.MapStrategyTM.DataBlocks;
      lock (dataBlocks)
      {
        foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
        {
          switch (keyValuePair.Value.ClassType)
          {
            case DataBlockType.ProximityDetector:
              ProximityDetectorBlock proximityDetectorBlock = keyValuePair.Value as ProximityDetectorBlock;
              if (proximityDetectorBlock != null && (proximityDetectorBlock.OnEntryScriptName.IsNotEmpty() && (proximityDetectorBlock.OnEntryScriptName == script.Name || proximityDetectorBlock.OnEntryScriptName == script.Alias) || proximityDetectorBlock.OnExitScriptName.IsNotEmpty() && (proximityDetectorBlock.OnExitScriptName == script.Name || proximityDetectorBlock.OnExitScriptName == script.Alias)))
                return "a proximity detector block";
              continue;
            case DataBlockType.NPCSpawn:
              NpcSpawnBlock npcSpawnBlock = keyValuePair.Value as NpcSpawnBlock;
              if (npcSpawnBlock != null && npcSpawnBlock.KillScript.IsNotEmpty() && (npcSpawnBlock.KillScript == script.Name || npcSpawnBlock.KillScript == script.Alias))
                return "an NPCSpawn block";
              continue;
            case DataBlockType.Script:
              ScriptBlock scriptBlock = keyValuePair.Value as ScriptBlock;
              if (scriptBlock != null && (scriptBlock.PowerOnScript.IsNotEmpty() && (scriptBlock.PowerOnScript == script.Name || scriptBlock.PowerOnScript == script.Alias) || scriptBlock.PowerOffScript.IsNotEmpty() && (scriptBlock.PowerOffScript == script.Name || scriptBlock.PowerOffScript == script.Alias)))
                return "a script block";
              continue;
            case DataBlockType.Health:
              HealthBlock healthBlock = keyValuePair.Value as HealthBlock;
              if (healthBlock != null && healthBlock.KillScript.IsNotEmpty() && (healthBlock.KillScript == script.Name || healthBlock.KillScript == script.Alias))
                return "a health block";
              continue;
            default:
              continue;
          }
        }
      }
      lock (this.eventScripts)
      {
        foreach (KeyValuePair<ScriptEvent, Script> eventScript in this.eventScripts)
        {
          if (eventScript.Value.Name == script.Name)
            return "an Event script";
        }
      }
      for (int index = 0; index < this.eventScriptItemSwing.Length; ++index)
      {
        if (this.eventScriptItemSwing[index] != null && this.eventScriptItemSwing[index].Name == script.Name)
          return "an ItemSwing Event script";
      }
      for (int index = 0; index < this.eventScriptItemEquip.Length; ++index)
      {
        if (this.eventScriptItemEquip[index] != null && this.eventScriptItemEquip[index].Name == script.Name)
          return "an ItemEquip Event script";
      }
      for (int index = 0; index < this.eventScriptItemUnequip.Length; ++index)
      {
        if (this.eventScriptItemUnequip[index] != null && this.eventScriptItemUnequip[index].Name == script.Name)
          return "an ItemUnequip Event script";
      }
      return (string) null;
    }

    public void AddAdventureScript(Script script)
    {
      if (script == null)
        return;
      lock (this.adventureScripts)
      {
        if (this.adventureScripts.Contains(script))
          return;
        this.adventureScripts.Add(script);
      }
    }

    public void RemoveAdventureScript(Script script)
    {
      if (script == null)
        return;
      lock (this.adventureScripts)
        this.adventureScripts.Remove(script);
    }

    public List<string> GetAdventureScriptNameList()
    {
      lock (this.adventureScripts)
      {
        List<string> stringList = new List<string>(this.adventureScripts.Count);
        for (int index = 0; index < this.adventureScripts.Count; ++index)
          stringList.Add(this.adventureScripts[index].Name);
        return stringList;
      }
    }

    public Dictionary<ScriptEvent, string> GetEventScriptNameList()
    {
      lock (this.eventScripts)
      {
        Dictionary<ScriptEvent, string> dictionary = new Dictionary<ScriptEvent, string>(this.eventScripts.Count);
        foreach (KeyValuePair<ScriptEvent, Script> eventScript in this.eventScripts)
          dictionary.Add(eventScript.Key, eventScript.Value.Name);
        return dictionary;
      }
    }

    public Script GetEventScript(ScriptEvent e)
    {
      lock (this.eventScripts)
      {
        Script script;
        if (this.eventScripts.TryGetValue(e, out script))
          return script;
      }
      return (Script) null;
    }

    public void SetEventScript(ScriptEvent e, string name)
    {
      this.SetEventScript(e, this.GetScript(name));
    }

    public void SetEventScript(ScriptEvent e, Script script)
    {
      lock (this.eventScripts)
      {
        if (this.eventScripts.ContainsKey(e))
        {
          if (script != null)
            this.eventScripts[e] = script;
          else
            this.eventScripts.Remove(e);
        }
        else
        {
          if (script == null)
            return;
          this.eventScripts.Add(e, script);
        }
      }
    }

    public void SetEventScript(ScriptEvent e, string name, Item itemID)
    {
      switch (e)
      {
        case ScriptEvent.ItemSwing:
          this.eventScriptItemSwing[(int) itemID] = this.GetScript(name);
          break;
        case ScriptEvent.ItemEquip:
          this.eventScriptItemEquip[(int) itemID] = this.GetScript(name);
          break;
        case ScriptEvent.ItemUnequip:
          this.eventScriptItemUnequip[(int) itemID] = this.GetScript(name);
          break;
      }
    }

    public string[] ListOfSortedScriptNames(string path)
    {
      return this.ListOfSortedScriptNamesCore(this.scripts, path);
    }

    public string[] ListOfSortedAdventureScriptNames(string path)
    {
      lock (this.adventureScripts)
        return this.ListOfSortedScriptNamesCore(this.adventureScripts, path);
    }

    public string[] ListOfSortedQueuedScriptNames(string path)
    {
      return this.ListOfSortedScriptNamesCore(this.scriptRuntimeWorker.GetListOfQueuedScripts(), path);
    }

    private string[] ListOfSortedScriptNamesCore(List<Script> scripts, string path)
    {
      if (path == null)
        path = "";
      List<string> stringList = new List<string>(scripts.Count);
      lock (scripts)
      {
        for (int index = 0; index < scripts.Count; ++index)
        {
          string name = scripts[index].Name;
          if (name.StartsWith(path, StringComparison.OrdinalIgnoreCase))
          {
            string str1 = name.Substring(path.Length);
            while (str1.StartsWith("\\"))
              str1 = str1.Substring(1);
            int num = str1.IndexOf('\\');
            if (num >= 0)
            {
              string str2 = str1.Substring(0, num + 1);
              if (!stringList.Contains(str2))
                stringList.Add(str2);
            }
            else
              stringList.Add(str1);
          }
        }
        stringList.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
      }
      return stringList.ToArray();
    }

    public void AddScriptIntersectDisplay(Ray ray, float length)
    {
      lock (this.ScriptIntersectDisplays)
        this.ScriptIntersectDisplays.Add(new ScriptIntersectDisplay()
        {
          Shape = ScriptShape.Ray,
          Box = new BoundingBox(ray.Position, ray.Direction),
          Length = length
        });
    }

    public void AddScriptIntersectDisplay(BoundingBox box)
    {
      lock (this.ScriptIntersectDisplays)
        this.ScriptIntersectDisplays.Add(new ScriptIntersectDisplay()
        {
          Shape = ScriptShape.Box,
          Box = box
        });
    }

    public void AddScriptIntersectDisplay(BoundingSphere sphere)
    {
      lock (this.ScriptIntersectDisplays)
        this.ScriptIntersectDisplays.Add(new ScriptIntersectDisplay()
        {
          Shape = ScriptShape.Sphere,
          Box = new BoundingBox(sphere.Center, new Vector3(sphere.Radius, 0.0f, 0.0f))
        });
    }

    public void AddScriptIntersectDisplay(Matrix frustum)
    {
      lock (this.ScriptIntersectDisplays)
        this.ScriptIntersectDisplays.Add(new ScriptIntersectDisplay()
        {
          Shape = ScriptShape.Frustum,
          Frustum = frustum
        });
    }

    public List<Script> ScriptSearchText(string path, string text)
    {
      List<Script> scriptList = new List<Script>();
      foreach (Script script in this.scripts)
      {
        if (path == null || path.Length < 1 || script.Name.StartsWith(path))
        {
          foreach (string command in script.Commands)
          {
            if (command.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
            {
              scriptList.Add(script);
              break;
            }
          }
        }
      }
      return scriptList;
    }

    public List<Script> ScriptSearchNames(string path, string text)
    {
      List<Script> scriptList = new List<Script>();
      foreach (Script script in this.scripts)
      {
        if ((path == null || path.Length < 1 || script.Name.StartsWith(path)) && (script.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 || script.Alias.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0))
          scriptList.Add(script);
      }
      return scriptList;
    }

    public void ScriptReplaceText(
      Script.ReplaceTextType type,
      Script script,
      ScriptEditScreen screen,
      string oldText,
      string newText,
      out int count,
      out int scriptCount)
    {
      count = 0;
      scriptCount = 0;
      switch (type)
      {
        case Script.ReplaceTextType.Script:
          if (script == null)
            break;
          count = this.ScriptReplaceTextCore(script, oldText, newText, 0, script.Commands.Count);
          scriptCount = 1;
          break;
        case Script.ReplaceTextType.SelectedText:
          if (script == null || screen == null || (screen.MinLineMarked < 0 || screen.MaxLineMarked < screen.MinLineMarked))
            break;
          count = this.ScriptReplaceTextCore(script, oldText, newText, screen.GetCmdIndex(screen.MinLineMarked), screen.GetCmdIndex(screen.MaxLineMarked) + 1);
          scriptCount = 1;
          break;
        case Script.ReplaceTextType.Folder:
          if (script == null)
            break;
          int num1 = this.ScriptReplaceTextCore(script, oldText, newText, 0, script.Commands.Count);
          if (num1 > 0)
          {
            ++scriptCount;
            count += num1;
          }
          int num2 = script.Name.LastIndexOf('\\');
          if (num2 < 0)
            break;
          string str = script.Name.Substring(0, num2 + 1);
          using (List<Script>.Enumerator enumerator = this.scripts.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Script current = enumerator.Current;
              if (current.Name.StartsWith(str) && !current.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase))
              {
                int num3 = this.ScriptReplaceTextCore(current, oldText, newText, 0, script.Commands.Count);
                if (num3 > 0)
                {
                  ++scriptCount;
                  count += num3;
                }
              }
            }
            break;
          }
        case Script.ReplaceTextType.AllScripts:
          int num4 = this.ScriptReplaceTextCore(script, oldText, newText, 0, script.Commands.Count);
          if (num4 > 0)
          {
            ++scriptCount;
            count += num4;
          }
          using (List<Script>.Enumerator enumerator = this.scripts.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Script current = enumerator.Current;
              if (!current.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase))
              {
                int num3 = this.ScriptReplaceTextCore(current, oldText, newText, 0, script.Commands.Count);
                if (num3 > 0)
                {
                  ++scriptCount;
                  count += num3;
                }
              }
            }
            break;
          }
      }
    }

    private int ScriptReplaceTextCore(
      Script script,
      string oldText,
      string newText,
      int startIndex,
      int endIndex)
    {
      int num = 0;
      for (int index = startIndex; index >= 0 && index < endIndex && index < script.Commands.Count; ++index)
      {
        if (script.Commands[index].Contains(oldText))
        {
          script.Commands[index] = script.Commands[index].Replace(oldText, newText);
          script.IsChanged = true;
          ++num;
        }
      }
      return num;
    }

    public int GetSkillRankLocal(SkillType skill, double xp)
    {
      int num = 1;
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag.SkillsData[(int) skill].CurrentXP > xp)
          ++num;
      }
      return num;
    }

    public int GetSkillTotalRankLocal(int level)
    {
      int num = 1;
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag.SkillsData.TotalLevel > level)
          ++num;
      }
      return num;
    }

    public int GetSkillCombatRankLocal(int level)
    {
      int num = 1;
      foreach (Gamer allGamer in this.networkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag.SkillsData.CombatLevel > level)
          ++num;
      }
      return num;
    }

    public void ToggleSkillSystem()
    {
      bool skillsEnabled = Globals2.GameProperties.SaveGame.Header.SkillsEnabled;
      Globals2.GameProperties.SaveGame.Header.SkillsEnabled = !skillsEnabled;
      this.SkillSystemChanged(skillsEnabled);
    }

    public void SkillSystemChanged(bool oldSkillSetting)
    {
      if (oldSkillSetting == this.IsSkillsEnabled)
        return;
      foreach (Gamer localGamer in this.networkManager.LocalGamers)
        (localGamer.Tag as Player)?.ResetPropertiesAfterSkillToggle(oldSkillSetting);
    }

    public void SendTextMessage(
      NetworkGamer recipient,
      Player sender,
      string clan,
      bool admin,
      string message)
    {
      if (message == null || message.Length <= 0 || sender == null)
        return;
      bool flag = !admin && recipient == null && clan != null && clan.Length > 0;
      string str = admin ? "Admins" : (flag ? clan : (recipient != null ? recipient.Gamertag : "All"));
      TextMsgTarget textMsgTarget = admin ? TextMsgTarget.Admins : (flag ? TextMsgTarget.Clan : (recipient == null ? TextMsgTarget.AllGamers : TextMsgTarget.Gamer));
      TextMessage msg = new TextMessage()
      {
        Target = textMsgTarget,
        Timer = 30f,
        Header = "[Me > " + str + "]",
        Message = message,
        Color = this.GetTextMessageColor(sender),
        BackColor = Color.Black * 0.4f,
        ClanID = sender.IsGod ? (byte) 80 : (byte) sender.ClanBannerID
      };
      this.TextMessages.Add(msg);
      this.ChatLog.Add(msg.Header + " " + message);
      if (admin || flag)
      {
        foreach (NetworkGamer allEnabledGamer in this.networkManager.AllEnabledGamers)
        {
          if (!allEnabledGamer.IsLocal)
          {
            Player tag = allEnabledGamer.Tag as Player;
            if (tag != null && (admin && tag.IsAdmin || tag.ClanName == clan))
              this.networkManager.SendTextMessage(msg, sender.Gamer, allEnabledGamer);
          }
        }
      }
      else
        this.networkManager.SendTextMessage(msg, sender.Gamer, recipient);
    }

    public void ReceiveTextMessage(TextMsgTarget target, NetworkGamer sender, string message)
    {
      if (sender == null || message == null || message.Length <= 0)
        return;
      Player tag = sender.Tag as Player;
      if (tag == null || !Globals2.GameSettings.HasNotification(NotificationType.TextMsg))
        return;
      string str1;
      switch (target)
      {
        case TextMsgTarget.AllGamers:
          str1 = " > All";
          break;
        case TextMsgTarget.Admins:
          str1 = " > Admins";
          break;
        case TextMsgTarget.Clan:
          str1 = " > Clan";
          break;
        case TextMsgTarget.Gamer:
          str1 = " > You";
          break;
        default:
          str1 = "";
          break;
      }
      string str2 = str1;
      TextMessage textMessage = new TextMessage()
      {
        Target = target,
        Timer = 30f,
        Header = "[" + sender.Gamertag + str2 + "]",
        Message = message,
        Color = this.GetTextMessageColor(tag),
        BackColor = Color.Black * 0.4f,
        ClanID = tag.IsGod ? (byte) 80 : (byte) tag.ClanBannerID
      };
      this.TextMessages.Add(textMessage);
      this.ChatLog.Add(textMessage.Header + " " + message);
      if (!Globals2.GameSettings.HasNotification(NotificationType.Audio))
        return;
      Sounds.PlaySound(ItemSoundGroup.GuiTxtMsgIn);
    }

    private Color GetTextMessageColor(Player sender)
    {
      if (sender.IsGod || sender.ActorType == ActorType.Zeus)
        return Color.LightGoldenrodYellow;
      if (sender.ActorType == ActorType.HermesWraith)
        return Color.Purple;
      if (sender.IsHost)
        return Color.Blue;
      if (!sender.IsAdmin)
        return Color.Yellow;
      return Color.Cyan;
    }

    private void UpdateTextMessages()
    {
      for (int index = this.TextMessages.Count - 1; index >= 0; --index)
      {
        TextMessage textMessage = this.TextMessages[index];
        textMessage.Timer -= Services.ElapsedTime;
        if ((double) textMessage.Timer <= 0.0)
          this.TextMessages.RemoveAt(index);
        else
          this.TextMessages[index] = textMessage;
      }
    }

    private void UpdateBroadcastSounds()
    {
      long num = Globals1.ElapsedWatch.ElapsedTicks - Globals1.StopWatchFreq * 2L;
      for (int index = 0; index < this.broadcastSounds.Count && this.broadcastSounds[index].Tick < num; ++index)
        this.broadcastSounds.RemoveAt(index);
    }

    private bool RunConsoleCommand(
      string command,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      if (command.IsNotEmpty() && command.StartsWith("tm ", StringComparison.OrdinalIgnoreCase))
      {
        command = command.Substring(3);
        int length = command.IndexOf(' ');
        string command1 = length < 0 ? command : command.Substring(0, length);
        switch (command1.Trim().ToLower())
        {
          case "kick":
            this.ConsoleKick(command1, caller, player, log);
            return true;
        }
      }
      return false;
    }

    private void LogConsole(IOutputLog log, string s)
    {
      log?.WriteLine(s);
    }

    private void ConsoleKick(string command, ITMPlayer caller, ITMPlayer player, IOutputLog log)
    {
      if (caller != null && player != null)
      {
        if (caller.HasPermission(Permissions.Admin))
        {
          string[] strArray = command.Split(' ');
          if (strArray[1].IsNotEmpty())
          {
            player = (ITMPlayer) null;
            NetworkGamer gamer = this.networkManager.GetGamer(strArray[1]);
            if (gamer != null)
              player = gamer.Tag as ITMPlayer;
            if (player == null)
            {
              this.LogConsole(log, "unknown player: " + strArray[1]);
              return;
            }
          }
          if (!player.Gamer.IsHost)
          {
            this.networkManager.KickGamer(player.Gamer, false);
            this.LogConsole(log, player.Gamer.Gamertag + " is kicked");
          }
          else
            this.LogConsole(log, "cannot kick the host");
        }
        else
          this.LogConsole(log, "you must be an admin to use this command");
      }
      else
        this.LogConsole(log, "unknown player");
    }

    public void RebuildLocalLight(Player player)
    {
      List<MapChunk> list = new List<MapChunk>(100);
      BoundingSphere sphere = new BoundingSphere(player.EyePosition, 128f);
      foreach (MapRegion mapRegion in this.map.Regions.Values)
      {
        list.Clear();
        Vector3 offset = mapRegion.Offset.ToVector3() * this.map.TileSize;
        mapRegion.Octree.GetObjectsInsideSphere(sphere, list, offset);
        foreach (MapChunk mapChunk in list)
          mapChunk?.SetChunkFlag(ChunkFlags.LightDirty);
      }
    }

    private struct ScreenToAdd
    {
      public GameScreen Screen;
      public Player Player;
    }

    private struct PointToIgnore
    {
      public GlobalPoint3D Point;
      public int Counter;
    }

    private struct ItemCustomSetup
    {
      public Item ItemID;
      public Permissions Permission;
    }

    public delegate void BookIDConfirmedHandler(
      object sender,
      Player player,
      BookData book,
      int slotID);
  }
}
