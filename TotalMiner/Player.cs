// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Player
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
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Achievements;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Renderers;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Screens2;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class Player : Actor2, ITMPlayer, ITMActor
  {
    public float ClipboardZoom = 1f;
    public ConcurrentPool<List<MapChunk>> ChunksToDraw = new ConcurrentPool<List<MapChunk>>(new Action<List<MapChunk>>(ChunksInPlayerViewLoader.ReleaseChunks));
    public ConcurrentPool<List<OctreeLeaf<MapChunk>>> OctreeLeavesToDraw = new ConcurrentPool<List<OctreeLeaf<MapChunk>>>(new Action<List<OctreeLeaf<MapChunk>>>(OctreeLeavesInPlayerViewLoader.ReleaseChunks));
    public ConcurrentPool<List<QuadtreeLeaf<MapChunk>>> QuadtreeLeavesToDraw = new ConcurrentPool<List<QuadtreeLeaf<MapChunk>>>(new Action<List<QuadtreeLeaf<MapChunk>>>(QuadtreeLeavesInPlayerViewLoader.ReleaseChunks));
    public Vector2 MainMapPosition = new Vector2(-1f, -1f);
    public float MainMapScale = 1f;
    public float ScreenTransitionOnTime = 4f;
    public History History = new History();
    public ActionLog ActionLog = new ActionLog();
    public Vec3Interpolator CustomTintColor = new Vec3Interpolator();
    public Vec4Interpolator CustomSkyColor = new Vec4Interpolator();
    private float stickThreshold = 0.05f;
    private Player.NetworkInstanceData netData = new Player.NetworkInstanceData();
    private Player.NetworkInstanceData prevNetData = new Player.NetworkInstanceData();
    private Player.NetworkInstanceData2 netData2 = new Player.NetworkInstanceData2();
    private Player.NetworkInstanceData2 prevNetData2 = new Player.NetworkInstanceData2();
    private int[] currentBlockTexture = new int[62];
    private FloatInterpolator fovInterpolator = new FloatInterpolator();
    public const int HotBarSize = 10;
    public readonly PlayerIndex PlayerIndex;
    public PlayerSettings Settings;
    public Matrix WorldShake;
    public Matrix WorldToolShake;
    public Matrix BlueprintFinderWorld;
    public Matrix LightProjection;
    public ArcadeMachine CurrentArcadeMachine;
    public Vector2 GamertagMeasure;
    public int NewVisitorTimer;
    public string NewVisitorMsg;
    public Unlockables Unlockables;
    public PlayerUnlockableData UnlockData;
    public SavePlayerState SaveState;
    public string PosString;
    public string CursorString;
    public PropertyToString<int> DepthString;
    public string HealthHUDString;
    public string OxygenHUDString;
    public Matrix ProjectionFarMatrix;
    public float ClipboardRotate;
    public Matrix ClipboardModelWorldMatrix;
    public PriceList DefaultPriceList;
    public Rectangle CamRect;
    public bool IsTakingPhoto;
    public bool IsAssemblingPhoto;
    public float CurrentScreenTransitionOnTime;
    public ChangeLog ChangeLog;
    public PlayerStateDataToSend StateDataToSend;
    public MiniMapRenderer MiniMapRenderer;
    public ushort LastTransmitterFrequency;
    public PlayerNetStateData LastStateSent;
    public GameScreen PauseMenuScreen;
    public UserControls UserControls;
    public RainParticleSystem RainParticleSystem;
    public HailParticleSystem HailParticleSystem;
    public float FogIntensity;
    public int FogVisibility;
    public Vector3 FogColor;
    public AudioListener AudioListener;
    public short CurrentZoneCombatLevelDifference;
    public string ClanName;
    public int ClanBannerID;
    public CharacterSkillsData LocalSkillsData;
    public bool IsHermes;
    public Block FootStepBlockForSound;
    public List<Player.ActionRequest> ActionRequests;
    public DeltaClampVec3 TintClamp;
    public DeltaClampVec3 SkyClamp;
    public float LeavesDroppingCount;
    public HUDElementManager HUDElementManager;
    public int LeftSwingCountNet;
    public int RightSwingCountNet;
    public bool OverrideIsEnabledInShop;
    public TextMessageViewLogScreen.FilterType TextMessageFilterType;
    public BehaviourTreeDesignWindow BTCanvas;
    public bool HasAbortedFloods;
    public Actor ActorInReticle;
    public DialogHandler DialogHandler;
    private object tag;
    private int lastHealthHud;
    private int lastMaxHealthHud;
    private int lastOxygenHud;
    private int lastMaxOxygenHud;
    private float timeSinceLastEdit;
    private bool inputEnabled;
    private GamePadState pad;
    private GamePadState lastpad;
    private GlobalPoint3D lastBlockPlaced;
    private bool isBusy;
    private float deathRecoveryTime;
    private float deathRecoveryTimer;
    private float bloodTintTimer;
    private Rumble rumble;
    private int botLeftStickCounter;
    private int botRightStickCounter;
    private Vector2 botLeftStick;
    private Vector2 botRightStick;
    private float blueprintFinderRoll;
    private float shopBelow100WarningTimer;
    private PlayerStats stats;
    private PlayerStats oldStats;
    private int killStreak;
    private float synchStatsTimer;
    private float reloadComponentTimer;
    private MapChunkTM reloadComponentChunk;
    private MapModel model;
    private MapModel crouchModel;
    private List<Player.Clipboard> clipboards;
    private GlobalPoint3D undoPoint;
    private int undoFacing;
    private GlobalPoint3D rebuildDepthStringLastPoint;
    private TakePhotoWorker takePhotoWorker;
    private bool isHeldCrouch;
    private double timeRubberBanding;
    private int hotBarVisibilityStack;
    private float hotBarToTransparencyTimer;
    private bool movedHotBarReverse;
    private bool leftHotbarCursorHasVisualPriority;
    private bool leftHotbarButtonFirst;
    private bool rightHotbarButtonFirst;
    private float hotbarButtonHeldTimer;
    private float sendInventoryTimer;
    private float cctvTimer;
    private bool isLobbyPermission;
    private bool adjustedSunMoonLastFrame;
    private bool adjustedSunMoonThisFrame;
    private bool binocularsView;
    private bool justClosedBinoculars;
    private Player.FOVState fovSaveState;
    private CreativeOperationData creativeFillDefaults;
    private CreativeOperationData creativePathDefaults;
    private CreativeOperationData creativeWallDefaults;
    private CreativeOperationData creativeLineDefaults;
    private CreativeOperationData creativeSphereDefaults;
    private CreativeOperationData creativeClearDefaults;
    private CreativeOperationData creativeFloodDefaults;
    private CreativeOperationData creativeTreesDefaults;
    private CreativeOperationData creativeReplaceDefaults;
    private CreativeOperationData creativeReplaceTextureDefaults;
    private CreativeOperationData creativeReplaceClipboardDefaults;
    private Dictionary<Buttons, Player.ButtonScript> buttonScripts;
    private Dictionary<string, TeleportMark> teleportMarks;
    private Hand tempDecalApplicatorHand;
    private Actor cctvTarget;
    private Vector3 drawOffsetPos;
    private Player virtualPlayer;
    private bool isBot;
    private int rightTriggerReleased;
    private int leftTriggerReleased;
    public Vector3 posDiff;
    private double lastSecondsPlayed;
    private ScriptBlock lastScriptBlockActivatedBySight;
    private float currentTime;
    private bool isNoClipMode;
    private Vector2 lastLeftStick;
    private Vector2 lastRightStick;
    public bool IsWorldShaking;
    private float saveFOVNormalised;
    private GameConsole console;
    private ConsoleWindow consoleWin;
    private float saveMouseScale;

    object ITMPlayer.Tag
    {
      get
      {
        return this.tag;
      }
      set
      {
        this.tag = value;
      }
    }

    PlayerIndex ITMPlayer.PlayerIndex
    {
      get
      {
        return this.PlayerIndex;
      }
    }

    NetworkGamer ITMPlayer.Gamer
    {
      get
      {
        return this.Gamer;
      }
    }

    ITMPlayer ITMPlayer.VirtualPlayer
    {
      get
      {
        return (ITMPlayer) this.VirtualPlayer;
      }
    }

    string ITMPlayer.ClanName
    {
      get
      {
        return this.ClanName;
      }
    }

    bool ITMPlayer.IsGod
    {
      get
      {
        return this.IsGod;
      }
    }

    bool ITMPlayer.IsInputEnabled
    {
      get
      {
        return this.IsInputEnabled;
      }
    }

    Matrix ITMPlayer.WorldShake
    {
      get
      {
        return this.WorldShake;
      }
    }

    Matrix ITMPlayer.WorldToolShake
    {
      get
      {
        return this.WorldToolShake;
      }
    }

    int ITMPlayer.SwingFacePos
    {
      get
      {
        return this.SwingFacePos;
      }
    }

    BlockFace ITMPlayer.SwingFace
    {
      get
      {
        return this.SwingFace;
      }
    }

    GlobalPoint3D ITMPlayer.SwingTarget
    {
      get
      {
        return this.SwingTarget;
      }
    }

    float ITMPlayer.SwingTargetDistance
    {
      get
      {
        return this.SwingTargetDistance;
      }
    }

    GlobalPoint3D ITMPlayer.PlaceTarget
    {
      get
      {
        return this.PlaceTarget;
      }
    }

    History ITMPlayer.History
    {
      get
      {
        return this.History;
      }
    }

    ITMActor ITMPlayer.ActorInReticle
    {
      get
      {
        return (ITMActor) this.ActorInReticle;
      }
    }

    Dictionary<string, TeleportMark> ITMPlayer.Teleports
    {
      get
      {
        return this.teleportMarks;
      }
    }

    ITMPlayer ITMPlayer.CreateCamera(ITMPlayer player)
    {
      if (this.IsCCTVView)
        this.EndCCTV();
      else
        this.saveFOVNormalised = 0.0f;
      Player player1 = new Player((NetworkGamer) null, this.PlayerIndex);
      player1.InitVirtual(this.instance, this.map, this, this.EyePosition, this.ViewDirection, this.FOVNormalized, this.Settings.GamePadSensitivity, (Actor) null);
      player1.EyeOffset = Vector3.Zero;
      this.virtualPlayer = player1;
      return (ITMPlayer) player1;
    }

    void ITMPlayer.RemoveCamera(ITMPlayer player, ITMPlayer virtualPlayer)
    {
      if (this.virtualPlayer != virtualPlayer)
        return;
      this.virtualPlayer = (Player) null;
    }

    public void AddTeleport(string name)
    {
      if (this.teleportMarks == null)
        this.teleportMarks = new Dictionary<string, TeleportMark>();
      else
        this.teleportMarks.Remove(name);
      this.teleportMarks.Add(name, new TeleportMark()
      {
        Position = this.Position,
        ViewAngle = this.viewAngle
      });
    }

    public bool RemoveTeleport(string name)
    {
      if (this.teleportMarks == null || !this.teleportMarks.ContainsKey(name))
        return false;
      this.teleportMarks.Remove(name);
      return true;
    }

    public bool TeleportTo(string name)
    {
      TeleportMark teleportMark;
      if (this.teleportMarks == null || !this.teleportMarks.TryGetValue(name, out teleportMark))
        return false;
      this.Position = teleportMark.Position;
      this.viewAngle = teleportMark.ViewAngle;
      return true;
    }

    public event EventHandler LeftStickPressed;

    public event EventHandler LeftStickReleased;

    public event EventHandler RightStickPressed;

    public event EventHandler RightStickReleased;

    public event BlockEventHandler BlockTargeted;

    public event BlockEventHandler CreativeFill;

    public event BlockEventHandler BlockCleared;

    public event BlockEventHandler BlockPlaced;

    public event ItemEventHandler ItemPickup;

    public event ItemEventHandler ItemProspect;

    public event ItemEventHandler ItemCrafted;

    public event ItemEventHandler ItemDropped;

    public event ItemEventHandler DetonateExplosive;

    public event EventHandler InventoryOpened;

    public event EventHandler InventoryClosed;

    public event InventoryEventHandler InventoryScreenItemSelected;

    public event InventoryEventHandler InventoryScreenItemPlaced;

    public event EventHandler WorkBenchOpened;

    public event EventHandler WorkBenchClosed;

    public event EventHandler ChestOpened;

    public event EventHandler ChestClosed;

    public event EventHandler FurnaceOpened;

    public event EventHandler FurnaceClosed;

    public event EventHandler EscapedToSurface;

    public event EventHandler MinutePlayed;

    public event BlockEventHandler TreasureChestOpened;

    public event EventHandler GrenadeLaunched;

    public event IntEventHandler FindBlueprint;

    public event IntEventHandler FindWisdomScroll;

    public event IntEventHandler TotalInvadersScore;

    public event IntEventHandler TotalRushScore;

    public event Player.PlayerEventHandler WorldVisited;

    public event Player.PlayerEventHandler WorldFavorited;

    public event Player.PlayerEventHandler WorldRated;

    public event Player.PlayerEventHandler RatedWorld;

    public event Player.PlayerEventHandler HealPlayer;

    public event Player.CharacterEventHandler PlayerDied;

    public event Player.CharacterEventHandler KillCharacter;

    public event IntEventHandler ReadHowTo;

    public event Player.CharacterAndItemEventHandler ItemThrown;

    public event Player.TradeEventHandler ItemTraded;

    public event MapEventHandler EnterMap;

    public event Player.MobEventHandler NPCPlacedOnMyMap;

    public void Raise_ItemThrown(Item itemID)
    {
      if (this.ItemThrown == null)
        return;
      if (this.HitTarget.Target == null)
        this.HitTarget = this.BuildHitTargetData(this.ViewDirection, Vector3.Zero, HitTargetOptions.PlayersAndNpcs, (List<ActorType>) null);
      this.ItemThrown((object) this, new ActorItemEventArgs(this.HitTarget.Target, itemID));
    }

    private void Raise_LeftStickPressed()
    {
      if (this.LeftStickPressed == null)
        return;
      this.LeftStickPressed((object) this, EventArgs.Empty);
    }

    private void Raise_LeftStickReleased()
    {
      if (this.LeftStickReleased == null)
        return;
      this.LeftStickReleased((object) this, EventArgs.Empty);
    }

    private void Raise_RightStickPressed()
    {
      if (this.RightStickPressed == null)
        return;
      this.RightStickPressed((object) this, EventArgs.Empty);
    }

    private void Raise_RightStickReleased()
    {
      if (this.RightStickReleased == null)
        return;
      this.RightStickReleased((object) this, EventArgs.Empty);
    }

    private void Raise_BlockTargeted(GlobalPoint3D p, MapBlock blockData)
    {
      if (this.BlockTargeted == null)
        return;
      this.BlockTargeted((object) this, new BlockEventArgs(p, blockData));
    }

    private void Raise_BlockCleared(GlobalPoint3D p, MapBlock blockData, Item itemID)
    {
      if (this.BlockCleared == null)
        return;
      this.BlockCleared((object) this, new BlockEventArgs(p, blockData, itemID));
    }

    private void Raise_BlockPlaced(GlobalPoint3D p, MapBlock blockData, Item itemID)
    {
      if (this.BlockPlaced == null)
        return;
      this.BlockPlaced((object) this, new BlockEventArgs(p, blockData, itemID));
    }

    public void Raise_CreativeFill(GlobalPoint3D p, MapBlock blockData)
    {
      if (this.CreativeFill == null)
        return;
      this.CreativeFill((object) this, new BlockEventArgs(p, blockData));
    }

    private void Raise_ItemPickup(Item itemID)
    {
      if (this.ItemPickup == null)
        return;
      this.ItemPickup((object) this, new ItemEventArgs(itemID));
    }

    private void Raise_ItemProspect(Item itemID)
    {
      if (this.ItemProspect == null)
        return;
      this.ItemProspect((object) this, new ItemEventArgs(itemID));
    }

    public void Raise_ItemCrafted(Item itemID)
    {
      if (this.ItemCrafted == null)
        return;
      this.ItemCrafted((object) this, new ItemEventArgs(itemID));
    }

    public void Raise_ItemDropped(Item itemID)
    {
      if (this.ItemDropped == null)
        return;
      this.ItemDropped((object) this, new ItemEventArgs(itemID));
    }

    public void Raise_DetonateExplosive(Item itemID)
    {
      if (this.DetonateExplosive == null)
        return;
      this.DetonateExplosive((object) this, new ItemEventArgs(itemID));
    }

    private void Raise_InventoryOpened()
    {
      if (this.InventoryOpened == null)
        return;
      this.InventoryOpened((object) this, EventArgs.Empty);
    }

    public void Raise_InventoryClosed()
    {
      if (this.InventoryClosed == null)
        return;
      this.InventoryClosed((object) this, EventArgs.Empty);
    }

    public void Raise_WorkBenchOpened()
    {
      if (this.WorkBenchOpened == null)
        return;
      this.WorkBenchOpened((object) this, EventArgs.Empty);
    }

    public void Raise_WorkBenchClosed()
    {
      if (this.WorkBenchClosed == null)
        return;
      this.WorkBenchClosed((object) this, EventArgs.Empty);
    }

    public void Raise_TreasureChestOpened(GlobalPoint3D p, MapBlock blockData)
    {
      if (this.TreasureChestOpened == null)
        return;
      this.TreasureChestOpened((object) this, new BlockEventArgs(p, blockData));
    }

    public void Raise_ChestOpened()
    {
      if (this.ChestOpened == null)
        return;
      this.ChestOpened((object) this, EventArgs.Empty);
    }

    public void Raise_ChestClosed()
    {
      if (this.ChestClosed == null)
        return;
      this.ChestClosed((object) this, EventArgs.Empty);
    }

    public void Raise_FurnaceOpened()
    {
      if (this.FurnaceOpened == null)
        return;
      this.FurnaceOpened((object) this, EventArgs.Empty);
    }

    public void Raise_FurnaceClosed()
    {
      if (this.FurnaceClosed == null)
        return;
      this.FurnaceClosed((object) this, EventArgs.Empty);
    }

    public void Raise_EscapedToSurface()
    {
      if (this.EscapedToSurface == null)
        return;
      this.EscapedToSurface((object) this, EventArgs.Empty);
    }

    public void Raise_MinutePlayed()
    {
      if (this.MinutePlayed == null)
        return;
      this.MinutePlayed((object) this, EventArgs.Empty);
    }

    public void Raise_InventoryScreenItemSelected(InventoryItem item, int slotID)
    {
      if (this.InventoryScreenItemSelected == null)
        return;
      this.InventoryScreenItemSelected((object) this, new InventoryItemEventArgs(item, slotID));
    }

    public void Raise_InventoryScreenItemPlaced(InventoryItem item, int slotID)
    {
      if (this.InventoryScreenItemPlaced == null)
        return;
      this.InventoryScreenItemPlaced((object) this, new InventoryItemEventArgs(item, slotID));
    }

    public void Raise_ItemTraded(Item item, int qty, int value, bool sell)
    {
      if (this.ItemTraded == null)
        return;
      this.ItemTraded((object) this, new TradeEventArgs(item, qty, value, sell));
    }

    public void Raise_FindBlueprint(int id)
    {
      if (this.FindBlueprint == null)
        return;
      this.FindBlueprint((object) this, new IntEventArgs(id));
    }

    public void Raise_FindWisdomScroll(int id)
    {
      if (this.FindWisdomScroll == null)
        return;
      this.FindWisdomScroll((object) this, new IntEventArgs(id));
    }

    public void Raise_TotalInvadersScore(int score)
    {
      if (this.TotalInvadersScore == null)
        return;
      this.TotalInvadersScore((object) this, new IntEventArgs(score));
    }

    public void Raise_TotalRushScore(int score)
    {
      if (this.TotalRushScore == null)
        return;
      this.TotalRushScore((object) this, new IntEventArgs(score));
    }

    public void Raise_WorldVisited(Player visitor)
    {
      if (this.WorldVisited == null)
        return;
      this.WorldVisited((object) this, new PlayerEventArgs(visitor));
    }

    public void Raise_RatedWorld(Player rater)
    {
      if (this.RatedWorld == null)
        return;
      this.RatedWorld((object) this, new PlayerEventArgs(rater));
    }

    public void Raise_WorldRated(Player rater)
    {
      if (this.WorldRated == null)
        return;
      this.WorldRated((object) this, new PlayerEventArgs(rater));
    }

    public void Raise_WorldFavorited(Player rater)
    {
      if (this.WorldFavorited == null)
        return;
      this.WorldFavorited((object) this, new PlayerEventArgs(rater));
    }

    public void Raise_HealPlayer(Player healed)
    {
      if (this.HealPlayer == null)
        return;
      this.HealPlayer((object) this, new PlayerEventArgs(healed));
    }

    private void Raise_PlayerDied(Actor killer)
    {
      if (this.PlayerDied == null)
        return;
      this.PlayerDied((object) this, new ActorEventArgs(killer, (Actor) null, Item.None));
    }

    public void Raise_KillCharacter(Actor killed, Actor target, Item weapon)
    {
      if (this.KillCharacter == null)
        return;
      this.KillCharacter((object) this, new ActorEventArgs(killed, target, weapon));
    }

    public void Raise_GrenadeLaunched()
    {
      if (this.GrenadeLaunched == null)
        return;
      this.GrenadeLaunched((object) this, EventArgs.Empty);
    }

    public void Raise_ReadHowTo(int id)
    {
      if (this.ReadHowTo == null)
        return;
      this.ReadHowTo((object) this, new IntEventArgs(id));
    }

    public void Raise_EnterMap(Map map)
    {
      if (this.EnterMap == null)
        return;
      this.EnterMap((object) this, new MapEventArgs(map));
    }

    public void Raise_NPCPlacedOnMyMap(Player placer, ActorType mobType)
    {
      if (this.NPCPlacedOnMyMap == null)
        return;
      this.NPCPlacedOnMyMap((object) placer, new NpcEventArgs(mobType));
    }

    public void OnComponentLoaded(bool success, object state)
    {
      if (success)
      {
        LoadComponentResult loadComponentResult = state as LoadComponentResult;
        if (loadComponentResult != null)
        {
          this.CloseActionRequest(loadComponentResult.State as Player.ActionRequest);
          if (loadComponentResult.Model != null && loadComponentResult.ErrorDesc == null && this.AddClipboard(loadComponentResult.Model, loadComponentResult.VoxelModelManager))
            return;
          if (loadComponentResult.ErrorDesc == null)
            loadComponentResult.ErrorDesc = "Inventory is full";
          this.instance.AddScreen((GameScreen) new MessageBoxScreenTM(loadComponentResult.ErrorDesc, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this), this);
          return;
        }
      }
      this.CloseActionRequests("Loading Component:");
      this.instance.AddNotification(this, "An unknown error occurred while loading the Component", NotifyRecipient.Local);
    }

    public static GamerID GetGamerID(Player player)
    {
      if (player == null)
        return GamerID.Sys1;
      return player.GamerID;
    }

    public int HotBarLeftSlotID
    {
      get
      {
        if (this.Inventory == null)
          return 0;
        return this.Inventory.HotBarLeftSlotID;
      }
    }

    public int HotBarRightSlotID
    {
      get
      {
        if (this.Inventory == null)
          return 0;
        return this.Inventory.HotBarRightSlotID;
      }
    }

    public override bool IsPlayer
    {
      get
      {
        return true;
      }
    }

    public PlayerStats Statistics
    {
      get
      {
        return this.stats ?? new PlayerStats();
      }
    }

    public bool IsViewingMainMap { get; set; }

    public bool IsHotBarVisible
    {
      get
      {
        if (this.Settings.HudVisible)
          return this.hotBarVisibilityStack < 1;
        return false;
      }
    }

    public int HotBarVisibilityStack
    {
      get
      {
        return this.hotBarVisibilityStack;
      }
      set
      {
        this.hotBarVisibilityStack = value;
      }
    }

    public int ScreenID
    {
      get
      {
        return (int) this.PlayerIndex;
      }
    }

    public override WieldType WieldType
    {
      get
      {
        return this.Settings.WieldType;
      }
    }

    public float BloodTint
    {
      get
      {
        float num = (double) this.deathRecoveryTimer > 0.0 ? this.deathRecoveryTimer / this.deathRecoveryTime : 0.0f;
        if ((double) this.bloodTintTimer <= (double) num)
          return num;
        return this.bloodTintTimer;
      }
    }

    public float HotBarTransparency
    {
      get
      {
        if (this.Settings.HotBarToTransparentTime == (byte) 0 || (double) this.hotBarToTransparencyTimer < (double) this.Settings.HotBarToTransparentTime)
          return 1f;
        return Math.Max(0.1f, (float) (1.0 - (double) (this.hotBarToTransparencyTimer - (float) this.Settings.HotBarToTransparentTime) / 3.0));
      }
    }

    public Color HotbarLeftCursorColor
    {
      get
      {
        if (!this.leftHotbarButtonFirst || this.movedHotBarReverse || (double) this.hotbarButtonHeldTimer <= 1.0)
          return Color.Yellow;
        return Color.LightGray;
      }
    }

    public Color HotbarRightCursorColor
    {
      get
      {
        if (!this.rightHotbarButtonFirst || this.movedHotBarReverse || (double) this.hotbarButtonHeldTimer <= 1.0)
          return Color.White;
        return Color.LightGray;
      }
    }

    public bool HotbarLeftCursorHasVisualPriority
    {
      get
      {
        return this.leftHotbarCursorHasVisualPriority;
      }
    }

    public Viewport Viewport
    {
      get
      {
        if (this.instance == null)
          return GraphicStatics.DefaultViewport;
        return this.instance.Viewports[this.ScreenID];
      }
      set
      {
        if (this.instance == null)
          return;
        this.instance.Viewports[this.ScreenID] = value;
        this.ResetPerspectiveMatrix();
      }
    }

    public Viewport Viewport2
    {
      get
      {
        Viewport viewport = this.Viewport;
        Viewport defaultViewport = GraphicStatics.DefaultViewport;
        if (viewport.X != defaultViewport.X && viewport.Y != defaultViewport.Y)
          return defaultViewport;
        return viewport;
      }
    }

    protected override CameraType GetCameraType()
    {
      return this.Settings.CameraType;
    }

    protected override float Gravity
    {
      get
      {
        if (this.FlyMode != FlyMode.None)
          return 0.0f;
        float gravity = base.Gravity;
        if (this.IsItemEquippedAndUsable(Item.TenLeagueBoots))
          gravity *= 0.5f;
        return gravity;
      }
    }

    private float HealthReplenishmentRate
    {
      get
      {
        return !this.instance.IsLegendaryDifficulty ? 0.01666667f : 0.0f;
      }
    }

    public override ActorTypeDataXML NpcTypeData
    {
      get
      {
        return Globals1.NpcTypeData[1];
      }
    }

    public override GamerID GamerID
    {
      get
      {
        if (this.Gamer == null)
          return GamerID.Sys1;
        return this.Gamer.ID;
      }
    }

    public TakePhotoWorker TakePhotoWorker
    {
      get
      {
        return this.takePhotoWorker ?? (this.takePhotoWorker = new TakePhotoWorker());
      }
    }

    protected override bool DisableCameraBackOffset
    {
      get
      {
        return true;
      }
    }

    public bool CanUndo
    {
      get
      {
        return false;
      }
    }

    public bool HasActiveFloods
    {
      get
      {
        if (this.Gamer != null)
        {
          foreach (IThreadWorkItem mainWorkItem in ThreadQueueManager.Instance.MainWorkItems)
          {
            FloodFill floodFill = mainWorkItem as FloodFill;
            if (floodFill != null && floodFill.PlayerID == this.GamerID)
              return true;
          }
        }
        return false;
      }
    }

    public Vector2 FOVRange
    {
      get
      {
        if (this.virtualPlayer != null && this.virtualPlayer != this)
          return this.virtualPlayer.FOVRange;
        Vector2 vector2;
        vector2.X = 45f;
        vector2.Y = 100f;
        if (this.Gamer == null)
          vector2.X = 3f;
        else if (this.binocularsView)
        {
          vector2.X = MathHelper.Lerp(vector2.X, 3f, this.fovInterpolator.CurrentValue);
          vector2.Y = MathHelper.Lerp(vector2.Y, 25f, this.fovInterpolator.CurrentValue);
        }
        return vector2;
      }
    }

    public float FOVNormalized
    {
      get
      {
        return this.Settings.FOVNormalized;
      }
      set
      {
        this.Settings.FOVNormalized = MathHelper.Clamp(value, 0.0f, 1f);
        this.fov = this.Settings.FOVNormalized * (this.FOVRange.Y - this.FOVRange.X) + this.FOVRange.X;
        this.ResetPerspectiveMatrix();
      }
    }

    public bool IsHost
    {
      get
      {
        if (this.Gamer == null)
          return false;
        return this.Gamer.IsHost;
      }
    }

    public override bool IsAdmin
    {
      get
      {
        if (!this.HasPermission(Permissions.Admin))
          return this.IsGod;
        return true;
      }
    }

    public bool IsRoboticAvatar
    {
      get
      {
        return this.ActorType == ActorType.Robotic;
      }
    }

    public bool IsHermesWraithAvatar
    {
      get
      {
        return this.ActorType == ActorType.HermesWraith;
      }
    }

    public bool HasUnsavedComponentEquipped
    {
      get
      {
        if (this.IsClipboardEquipped && this.ClipboardModel.DirNum == 0)
          return this.ClipboardModel.ComName == null;
        return false;
      }
    }

    public void ResetScreenTransition()
    {
      this.CurrentScreenTransitionOnTime = this.ScreenTransitionOnTime;
    }

    public PlayerStats GetStatisticsClone()
    {
      if (this.stats == null)
        return new PlayerStats();
      return this.stats.Clone();
    }

    public int GetCurrentBlockTexture(Block blockID)
    {
      int blockTextureIndex = this.map.GetBlockTextureIndex(blockID);
      if (blockTextureIndex < 0)
        return 0;
      return this.currentBlockTexture[blockTextureIndex];
    }

    public void SetCurrentBlockTexture(Block blockID, int texture)
    {
      int blockTextureIndex = this.map.GetBlockTextureIndex(blockID);
      if (blockTextureIndex < 0)
        return;
      this.currentBlockTexture[blockTextureIndex] = texture;
    }

    public bool IsInputEnabled
    {
      get
      {
        if (this.inputEnabled)
          return !this.IsViewingMainMap;
        return false;
      }
    }

    public bool IsSleeping
    {
      get
      {
        return this.actorState == ActorState.Sleeping;
      }
      set
      {
        if (value)
        {
          if (this.actorState == ActorState.Sleeping)
            return;
          this.actorState = ActorState.Sleeping;
          this.instance.RecalcAllPlayersSleeping();
        }
        else
        {
          if (this.actorState != ActorState.Sleeping)
            return;
          this.actorState = ActorState.Alive;
          this.instance.RecalcAllPlayersSleeping();
        }
      }
    }

    public Matrix AvatarWorld
    {
      get
      {
        Matrix matrix = Matrix.CreateTranslation(this.drawOffsetPos) * Matrix.CreateLookAt(Vector3.Zero, new Vector3(-this.ViewDirection.X, 0.0f, this.ViewDirection.Z), Vector3.Up);
        if ((double) this.DrawScale != 1.0)
          matrix *= Matrix.CreateScale(this.DrawScale);
        return matrix * Matrix.CreateTranslation(this.Position);
      }
    }

    public Matrix NameplateWorld
    {
      get
      {
        return Matrix.Identity * Matrix.CreateTranslation(this.Position);
      }
    }

    public bool IsBusy
    {
      get
      {
        if (!this.isBusy && !this.LeftHand.IsSwinging && !this.RightHand.IsSwinging)
          return (double) this.timeSinceLastEdit < 3.0;
        return true;
      }
      set
      {
        this.isBusy = value;
        if (this.isBusy)
          return;
        this.timeSinceLastEdit = float.MaxValue;
      }
    }

    public GlobalPoint3D SpawnPoint
    {
      get
      {
        if (this.instance == null || this.map == null)
          return GlobalPoint3D.Zero;
        GlobalPoint3D globalPoint3D = this.GetZoneSpawnPoint();
        if (globalPoint3D.Y == 0)
          globalPoint3D = this.GetShopSpawnPoint();
        this.ViewDirection = Vector3.Backward;
        return globalPoint3D;
      }
    }

    private GlobalPoint3D GetZoneSpawnPoint()
    {
      return Player.GetZoneSpawnPoint(this.instance, (Map) this.map, this.Position);
    }

    public static GlobalPoint3D GetZoneSpawnPoint(
      GameInstance instance,
      Map map,
      Vector3 position)
    {
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D(0, 0, 0);
      MapStrategyTM mapStrategy = map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
      {
        Zone zone1 = (Zone) null;
        float num1 = float.MaxValue;
        List<Zone> zones = mapStrategy.GetZones(ZoneType.Spawn);
        lock (zones)
        {
          foreach (Zone zone2 in zones)
          {
            GlobalPoint3D point = (zone2.Max - zone2.Min) / 2 + zone2.Min;
            float num2 = Vector3.DistanceSquared(position, map.GetPosition(point));
            if ((double) num2 < (double) num1)
            {
              num1 = num2;
              zone1 = zone2;
            }
          }
        }
        if (zone1 != null)
        {
          GlobalPoint3D globalPoint3D2 = GlobalPoint3D.Min(zone1.Min, zone1.Max);
          GlobalPoint3D globalPoint3D3 = GlobalPoint3D.Max(zone1.Min, zone1.Max);
          globalPoint3D1.X = instance.Random.Next(globalPoint3D3.X - globalPoint3D2.X) + globalPoint3D2.X;
          globalPoint3D1.Z = instance.Random.Next(globalPoint3D3.Z - globalPoint3D2.Z) + globalPoint3D2.Z;
          globalPoint3D1.Y = Math.Max(map.MapBound.Min.Y + 1, globalPoint3D2.Y - 1);
        }
      }
      return globalPoint3D1;
    }

    public GlobalPoint3D GetShopSpawnPoint()
    {
      return Player.GetShopSpawnPoint(this.instance, (Map) this.map);
    }

    public static GlobalPoint3D GetShopSpawnPoint(GameInstance instance, Map map)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (map.MapBound.Max.X - map.MapBound.Min.X) / 2 + map.MapBound.Min.X;
      p.Z = (map.MapBound.Max.Z - map.MapBound.Min.Z) / 2 + map.MapBound.Min.Z;
      p.X -= p.X % map.ChunkSize.X;
      p.Z -= p.Z % map.ChunkSize.Z;
      p.X += map.ChunkSize.X / 2;
      p.Z += map.ChunkSize.Z / 2;
      p.X += instance.Random.Next(8) - 4;
      p.Z += instance.Random.Next(8) - 4;
      p.Y = (int) map.GetHeight(p);
      if (p.Y == (int) map.SeaLevel)
        p.Y = instance.GetGeneratedHeight(p);
      ++p.Y;
      return p;
    }

    public Color PlaceTargetColor
    {
      get
      {
        if (!this.PlaceTargetColorCheck(this.LeftHand) || !this.PlaceTargetColorCheck(this.RightHand))
          return Color.Red;
        return new Color(228, 228, 228);
      }
    }

    private bool PlaceTargetColorCheck(Hand hand)
    {
      if (ItemData.GetItemUse(hand.ItemID) == ItemUse.Block || ItemData.IsSubType(hand.ItemID, ItemSubType.Key))
        return this.PlaceTargetIsValid(hand);
      return true;
    }

    public bool IsInPlayerSpace(GlobalPoint3D p)
    {
      return this.Box.Intersects(this.instance.GetBlockBox(p));
    }

    public Player VirtualPlayer
    {
      get
      {
        Player virtualPlayer = this.virtualPlayer;
        if (virtualPlayer == null || !virtualPlayer.IsEnabledField || virtualPlayer.Gamer != null && virtualPlayer.Gamer.Tag == null)
          this.virtualPlayer = this;
        return this.virtualPlayer;
      }
    }

    public string Gamertag
    {
      get
      {
        if (this.Gamer == null)
          return "Unknown";
        return this.Gamer.Gamertag;
      }
    }

    public bool IsBot
    {
      get
      {
        return this.isBot;
      }
      set
      {
        this.isBot = value;
      }
    }

    public Matrix GetScreenMatrix()
    {
      return this.GetScreenMatrix(false);
    }

    public override Matrix GetScreenMatrix(bool globalScreenSpace)
    {
      Viewport viewport1 = CoreGlobals.GraphicsDevice.Viewport;
      Viewport viewport2 = this.instance.Viewports[this.ScreenID];
      int num1 = viewport2.Width / viewport1.Width;
      int num2 = viewport2.Height / viewport1.Height;
      Vector3 one = Vector3.One;
      Vector3 zero = Vector3.Zero;
      if (num1 > num2)
      {
        one.X = one.Y = (float) num2;
        zero.X += (float) viewport2.Width * 0.5f * (float) num2;
      }
      else if (num2 > num1)
      {
        one.X = one.Y = (float) num1;
        zero.Y += (float) viewport2.Height * 0.5f * (float) num1;
      }
      if (globalScreenSpace)
      {
        zero.X += (float) viewport2.X;
        zero.Y += (float) viewport2.Y;
      }
      return Matrix.CreateScale(one) * Matrix.CreateTranslation(zero);
    }

    public Matrix GetScreenMatrix(ScreenForScale scale)
    {
      return ScreenMatrix.GetScreenMatrix(this.instance, this, scale);
    }

    public Matrix GetScreenMatrix(Rectangle rect)
    {
      return ScreenMatrix.GetScreenMatrix(this.instance, this, rect);
    }

    public void GetScreenOffset(ScreenForScale screenScale, out float scale, out Vector3 pos)
    {
      ScreenMatrix.GetScreenOffset(this.instance, this, screenScale, out scale, out pos);
    }

    public void GetScreenOffset(Rectangle rect, out float scale, out Vector3 pos)
    {
      ScreenMatrix.GetScreenOffsetAndScale(this.instance, this, rect, out scale, out pos);
    }

    public PlayerStats.Stat[] GetPlayerStatsAsText()
    {
      return this.stats.GetPlayerStatsAsText();
    }

    private GlobalPoint3D BedFootTarget
    {
      get
      {
        GlobalPoint3D zero = GlobalPoint3D.Zero;
        if ((double) Math.Abs(this.ViewDirection.X) > (double) Math.Abs(this.ViewDirection.Z))
          zero.X += Math.Sign(this.ViewDirection.X);
        else
          zero.Z += Math.Sign(this.ViewDirection.Z);
        return this.PlaceTarget + zero;
      }
    }

    public bool IsInViewOfSky
    {
      get
      {
        GlobalPoint3D point = this.map.GetPoint(this.EyePosition);
        if (point.Y > (int) this.map.SeaLevel - 20 || point.Y > (int) this.map.GetHeight(point) - 20)
          return true;
        return this.map.GetLight(point).SunLight > (byte) 0;
      }
    }

    public int ViewingLavaLevel
    {
      get
      {
        if (this.instance.CurrentBiome != BiomeType.DigDeep || this.IsInViewOfSky)
          return -1;
        GlobalPoint3D point = this.map.GetPoint(this.EyePosition);
        if (Globals2.GameProperties.SaveGame.Header.SaveVersion <= 291)
          return DigDeepBiome.GetLavaLevelID((Map) this.map, point.Y);
        return DigDeepBiome2.GetLavaLevelViewingID((Map) this.map, point.Y);
      }
    }

    public void ToggleMobNameplateSetting()
    {
      this.Settings.MobNameplates = !this.Settings.MobNameplates;
    }

    public void ToggleNameplateSetting()
    {
      switch (this.Settings.Nameplates)
      {
        case NamePlateSetting.None:
          this.Settings.Nameplates = NamePlateSetting.Short;
          break;
        case NamePlateSetting.Short:
          if (this.HasPermission(Permissions.Edit))
          {
            this.Settings.Nameplates = NamePlateSetting.Far;
            break;
          }
          this.Settings.Nameplates = NamePlateSetting.None;
          break;
        default:
          this.Settings.Nameplates = NamePlateSetting.None;
          break;
      }
    }

    public MapModel AvatarModel
    {
      get
      {
        if (!this.IsCrouching)
          return this.model;
        return this.crouchModel;
      }
    }

    public string GetNameplateSettingText()
    {
      if (this.Settings.Nameplates == NamePlateSetting.Short)
        return "Short";
      return this.Settings.Nameplates == NamePlateSetting.Far ? "Far" : "Off";
    }

    public string GetMobNameplateSettingText()
    {
      return !this.Settings.MobNameplates ? "Off" : "On";
    }

    protected override int HealthLevelCore(bool addBonuses)
    {
      SkillData health = this.SkillsData.Health;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? health.Level : Globals1.NpcLevelData[1].HealthLevel;
      if (addBonuses && isSkillsEnabled)
        num += health.GetBonusLevels((Actor) this);
      return num;
    }

    protected override int AttackLevelCore(bool addBonuses)
    {
      SkillData attack = this.SkillsData.Attack;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? attack.Level : Globals1.NpcLevelData[1].AttackLevel;
      if (addBonuses && isSkillsEnabled)
        num += attack.GetBonusLevels((Actor) this);
      return num;
    }

    protected override int StrengthLevelCore(bool addBonuses)
    {
      SkillData strength = this.SkillsData.Strength;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? strength.Level : Globals1.NpcLevelData[1].StrengthLevel;
      if (addBonuses && isSkillsEnabled)
        num += strength.GetBonusLevels((Actor) this);
      return num;
    }

    protected override int DefenceLevelCore(bool addBonuses)
    {
      SkillData defence = this.SkillsData.Defence;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? defence.Level : Globals1.NpcLevelData[1].DefenceLevel;
      if (addBonuses && isSkillsEnabled)
        num += defence.GetBonusLevels((Actor) this);
      return num;
    }

    protected override int RangedLevelCore(bool addBonuses)
    {
      SkillData ranged = this.SkillsData.Ranged;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? ranged.Level : Globals1.NpcLevelData[1].RangedLevel;
      if (addBonuses && isSkillsEnabled)
        num += ranged.GetBonusLevels((Actor) this);
      return num;
    }

    protected override int LootingLevelCore(bool addBonuses)
    {
      SkillData looting = this.SkillsData.Looting;
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      int num = isSkillsEnabled ? looting.Level : 50;
      if (addBonuses && isSkillsEnabled)
        num += looting.GetBonusLevels((Actor) this);
      return num;
    }

    public override string CombatLevelString
    {
      get
      {
        return Globals2.GetItemCountString(this.CombatLevel);
      }
    }

    public void SetHealthHudString()
    {
      int num = (int) this.MaxHealth;
      if ((int) this.Health == this.lastHealthHud && num == this.lastMaxHealthHud)
        return;
      if ((double) num < (double) this.Health)
        num = (int) this.Health;
      this.HealthHUDString = string.Format("{0} / {1}", (object) (int) Math.Max(this.Health, 1f), (object) num);
      this.lastHealthHud = (int) this.Health;
      this.lastMaxHealthHud = num;
    }

    public void SetOxygenHudString()
    {
      int num = (int) this.MaxOxygen;
      if ((int) this.Oxygen == this.lastOxygenHud && num == this.lastMaxOxygenHud)
        return;
      if ((double) num < (double) this.Oxygen)
        num = (int) this.Oxygen;
      this.OxygenHUDString = string.Format("{0} / {1}", (object) (int) Math.Max(this.Oxygen, 1f), (object) num);
      this.lastOxygenHud = (int) this.Oxygen;
      this.lastMaxOxygenHud = num;
    }

    public void AddModels(List<MapModel> models)
    {
      models.Add(this.model);
      models.Add(this.crouchModel);
    }

    public override bool HasHistory(string key)
    {
      return this.History.HasHistory(key);
    }

    public bool IsEscapeEnabled
    {
      get
      {
        return !this.instance.MapStrategyTM.IsInZoneType(this.Box, ZoneType.NoEscape, this.GamerID);
      }
    }

    public static bool IsActorTypeValidForAvatar(ActorType type)
    {
      if (type != ActorType.Zeus && type != ActorType.Robotic && (type != ActorType.DemiGod && type != ActorType.DemiGoddess) && type != ActorType.HermesWraith)
        return type != ActorType.TesterMan;
      return false;
    }

    protected void SetTesterFlags()
    {
      this.IsDeveloper = this.GetIsDeveloper();
      this.IsTester = this.GetIsTester();
      this.IsHermes = this.GetIsHermes();
      this.IsTesterman = this.IsTester && this.ActorType == ActorType.TesterMan;
      this.IsGod = this.IsDeveloper && this.ActorType == ActorType.Zeus;
      this.IsGodOrTester = this.IsGod || this.IsTesterman;
      this.IsGodOrTesterRetail = this.IsGod || this.IsTester && this.ActorType == ActorType.TesterMan;
    }

    private bool GetIsDeveloper()
    {
      return false;
    }

    private bool GetIsTester()
    {
      return false;
    }

    public static bool IsRoboticGamerTag(PlayerIndex playerIndex, bool isBadBoy)
    {
      return false;
    }

    private bool GetIsHermes()
    {
      return false;
    }

    public Player(NetworkGamer gamer, PlayerIndex playerIndex)
      : base((GameInstance) null, (MapTM) null, gamer, ActorType.Boy)
    {
      this.PlayerIndex = playerIndex;
      if (gamer != null)
        gamer.Tag = (object) this;
      this.SetTesterFlags();
      this.Settings = new PlayerSettings();
      this.UserControls = new UserControls();
      this.PosString = "(0,0,0)";
      this.CursorString = "(- - -)";
      this.DepthString = new PropertyToString<int>()
      {
        Value = 0,
        Format = "Depth: {0}"
      };
      this.AudioListener = new AudioListener();
      this.AudioListener.Up = Vector3.Up;
      this.SkillsData = this.LocalSkillsData = new CharacterSkillsData();
      this.useCustomWalkSoundLogic = gamer != null && gamer.IsLocal;
      this.ActionRequests = new List<Player.ActionRequest>();
      this.HUDElementManager = new HUDElementManager();
      this.DialogHandler = new DialogHandler(this);
      this.Properties.CanFight = this.Properties.CanPickup = this.Properties.CanBeHealedByOther = new bool?(true);
    }

    protected void InitVirtual(
      GameInstance instance,
      MapTM map,
      Player player,
      Vector3 position,
      Vector3 viewDir,
      float fovNormalized,
      float gamePadSensitivity,
      Actor target)
    {
      this.instance = instance;
      this.map = map;
      this.Position = position;
      this.ViewDirection = viewDir;
      this.IsEnabled = this.IsEnabledField = true;
      this.WorldShake = Matrix.Identity;
      this.WorldToolShake = Matrix.Identity;
      this.actorState = ActorState.Alive;
      this.cctvTarget = target;
      this.virtualPlayer = (Player) null;
      this.nearClip = player.nearClip;
      this.farClip = player.farClip;
      this.CustomSkyColor = player.CustomSkyColor;
      this.CustomTintColor = player.CustomTintColor;
      this.SkillsData = player.SkillsData;
      this.Health = player.Health;
      this.Oxygen = player.Oxygen;
      this.Inventory = this.CreateInventory();
      this.Settings = player.Settings.Clone();
      this.Settings.GamePadSensitivity = gamePadSensitivity;
      this.Settings.BlueprintFinderVisible = false;
      this.Settings.HudVisible = false;
      this.Settings.MapVisible = false;
      this.Settings.RumbleOn = false;
      this.Reach = 0;
      InputManager1.Profile.GamePadRumble = false;
      InputManager1.Profile.GamePadSensitivity = gamePadSensitivity;
      this.Frustum = new BoundingFrustum(Matrix.Identity);
      this.Spheres = new List<BoundingSphere>();
      this.AudioEmitter = new AudioEmitter();
      this.AudioEmitter.Up = Vector3.Up;
      this.LeftHand = new Hand((Actor) this, InventoryHand.Left);
      this.RightHand = new Hand((Actor) this, InventoryHand.Right);
      this.FOVNormalized = fovNormalized;
      this.UpdateMatrices();
    }

    public void InitToPlay(
      GameInstance instance,
      MapTM map,
      int seed,
      CharacterSkillsData skillsDataOverride)
    {
      this.instance = instance;
      this.map = map;
      this.WorldShake = Matrix.Identity;
      this.WorldToolShake = Matrix.Identity;
      this.stats = new PlayerStats();
      this.oldStats = new PlayerStats();
      this.synchStatsTimer = 15f;
      this.hotBarVisibilityStack = 0;
      this.virtualPlayer = (Player) null;
      Player virtualPlayer = this.VirtualPlayer;
      this.reach = 0;
      this.binocularsView = false;
      if (seed > 0)
        this.random.Seed(seed);
      if (skillsDataOverride != null)
      {
        this.SkillsData = skillsDataOverride;
      }
      else
      {
        if (instance.IsLocalSkills)
          return;
        this.SkillsData = this.IsLocalGamer ? Globals2.GamertagData.GetPlayerSkillData((Gamer) this.Gamer) : new CharacterSkillsData();
        Globals2.GamertagData.AddHighScoreCacheEntry((Gamer) this.Gamer, this.SkillsData);
      }
    }

    public bool HasBeenInitializedForPlay
    {
      get
      {
        return this.stats != null;
      }
    }

    protected override void InitializeCore(InitState state)
    {
      base.InitializeCore(state);
      if (this.RightHand.ItemSwing.Bobbing != null)
        this.RightHand.ItemSwing.Bobbing.BobCentered += new EventHandler(this.OnBobCentered);
      this.nearClip = 0.05f;
      this.farClip = this.GetFarClip();
      this.FOV = 75f;
      this.deathRecoveryTime = 3f;
      this.halfSizeFactor = 0.36f;
      this.lastBlockPlaced = new GlobalPoint3D(0, 1, 0);
      this.ChangeLog = new ChangeLog();
      this.nonSwingTargets = new List<byte>();
      this.nonSwingTargets.Add((byte) 53);
      this.nonSwingTargets.Add(this.map.OutOfBoundsBlockID);
      this.clipboards = new List<Player.Clipboard>();
      this.buttonScripts = new Dictionary<Buttons, Player.ButtonScript>();
      this.CustomSkyColor.Reset(Vector4.Zero);
      this.CustomTintColor.Reset(Vector3.One);
      this.TintClamp = new DeltaClampVec3(3f, 6f);
      this.SkyClamp = new DeltaClampVec3(3f, 6f);
      if (!this.IsLocalGamer)
        return;
      this.effectManager = new CharacterEffectManager((ITMActor) this, (ITMActor) null);
    }

    private void OnBobCentered(object sender, EventArgs e)
    {
      this.PlayWalkSound();
    }

    protected override void PlayFootStepSound(GlobalPoint3D p, Block blockUnderFoot)
    {
      base.PlayFootStepSound(p, blockUnderFoot);
      this.FootStepBlockForSound = blockUnderFoot;
      this.StateDataToSend |= PlayerStateDataToSend.FootSound;
    }

    protected override EquipmentInventory CreateInventory()
    {
      return new EquipmentInventory(30, 7, 9);
    }

    public override void SetSize(GlobalPoint3D modelSize, float scale)
    {
      base.SetSize(modelSize, scale);
      this.drawOffsetPos = new Vector3((float) -modelSize.X * 0.5f, -1f, (float) -modelSize.Z * 0.5f);
      if ((double) this.fullHeight <= 1.96000003814697)
        return;
      this.fullHeight = this.Size.Y = 1.96f;
      this.crouchHeight = this.fullHeight * 0.5f;
      this.fullEyeHeight = this.EyeOffset.Y;
      if (!this.IsCrouching)
        return;
      this.EyeOffset.Y = this.Size.Y = this.crouchHeight;
    }

    public void OnViewDistanceChanged()
    {
      this.farClip = this.GetFarClip();
      this.ResetPerspectiveMatrix();
    }

    protected override float GetFarClip()
    {
      return Globals2.GetFarClip(this.instance);
    }

    protected override void ResetPerspectiveMatrix(float fov, float nearClip, float farClip)
    {
      float aspectRatio = this.Viewport.AspectRatio;
      if ((double) aspectRatio == 0.0 || (double) nearClip == 0.0 || (double) farClip == 0.0)
        return;
      this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearClip, farClip);
      this.ProjectionFarMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearClip, this.GetFarClip() + 2000f);
    }

    protected override void LoadContentCore(InitState state)
    {
      this.ResetScreenTransition();
      base.LoadContentCore(state);
      if (this.Gamer.IsLocal)
      {
        this.MiniMapRenderer = new MiniMapRenderer(this.instance, this.map, this);
        this.MiniMapRenderer.Initialize();
        this.MiniMapRenderer.LoadContent();
        this.MiniMapRenderer.IsEnabled = true;
        this.RainParticleSystem = new RainParticleSystem();
        this.RainParticleSystem.Initialize();
        this.RainParticleSystem.LoadContent();
        this.HailParticleSystem = new HailParticleSystem();
        this.HailParticleSystem.Initialize(this.map);
        this.HailParticleSystem.LoadContent();
      }
      this.GamertagMeasure = CoreGlobals.GameFont.MeasureString(this.DisplayGamertag);
    }

    protected override void UnloadContentCore()
    {
      if (this.MiniMapRenderer != null)
        this.MiniMapRenderer.UnloadContent();
      if (this.RainParticleSystem != null)
        this.RainParticleSystem.UnloadContent();
      if (this.HailParticleSystem != null)
        this.HailParticleSystem.UnloadContent();
      if (this.LeftHand != null)
        this.LeftHand.UnloadContent();
      if (this.RightHand != null)
      {
        if (this.RightHand.ItemSwing.Bobbing != null)
          this.RightHand.ItemSwing.Bobbing.BobCentered -= new EventHandler(this.OnBobCentered);
        this.RightHand.UnloadContent();
      }
      if (this.rumble != null)
        this.rumble.StartRumble(RumbleType.None);
      base.UnloadContentCore();
    }

    public void LoadData(SavePlayerState playerData, int version)
    {
      this.SaveState = playerData;
      this.Position = this.lastPosition = playerData.Position;
      if (!this.map.IsValidPoint(this.map.GetPoint(playerData.Position)) && !this.map.IsInsideMap(this.map.GetPoint(playerData.Position), Point3D.One))
      {
        this.Position = this.map.GetPosition(this.GetShopSpawnPoint());
        this.DefaultRespawn();
      }
      this.random = new PcgRandom(playerData.Seed);
      this.Settings = playerData.Settings.Clone();
      this.LocalSkillsData = playerData.SkillsData;
      if (this.instance.IsLocalSkills)
        this.SkillsData = this.LocalSkillsData;
      this.stats = playerData.Statistics.Clone();
      this.stats.Instance = this.instance;
      this.lastSecondsPlayed = this.stats.SecondsPlayed;
      if (this.IsLocalGamer)
      {
        GamertagData gamertagData = Globals2.GamertagData.GetGamertagData((Gamer) this.Gamer) ?? new GamertagData((Gamer) this.Gamer);
        if (gamertagData.Settings.GlobalOverwrite && !this.instance.IsAvatarDesigner)
        {
          this.Settings = gamertagData.Settings.PlayerSettings.Clone();
          this.Settings.BlueprintFinderVisible = playerData.Settings.BlueprintFinderVisible;
          this.Settings.MapVisible = playerData.Settings.MapVisible;
          this.Settings.HudVisible = playerData.Settings.HudVisible;
          this.Settings.MobNameplates = playerData.Settings.MobNameplates;
          this.Settings.Nameplates = playerData.Settings.Nameplates;
        }
        this.UnlockData = gamertagData.UnlockData;
        this.UnlockData.SageWisdomsFound = playerData.ScrollsFound;
        this.UnlockData.EntrepreneurGoldEarned = playerData.GoldEarned;
        this.UnlockData.KnightBedrockReached = playerData.BedRockProspected;
        this.UnlockData.KnightEnemiesKilled = playerData.EnemiesKilledBeforeBedrock;
        this.UnlockData.GoldenKnightRatesReceived = 0;
        if (this.Gamer.Gamertag == Globals2.GameProperties.SaveGame.Header.OwnerGamerTag)
          this.UnlockData.GoldenKnightRatesReceived = Globals2.GameProperties.SaveGame.Header.RatingCount;
        this.Unlockables = new Unlockables(this);
      }
      this.EyeOffset.Y = this.Size.Y = this.crouchHeight;
      this.EyePosition = this.Position + this.EyeOffset;
      this.FOVNormalized = this.Settings.FOVNormalized;
      this.DefaultPriceList = playerData.DefaultPriceList;
      if (!this.isLobbyPermission)
      {
        this.Permission = playerData.Permission;
        if (version < 208)
        {
          if (this.HasPermission(Permissions.Admin))
            this.Permission |= Permissions.SystemShops | Permissions.ViewScripts;
          else if (this.HasPermission(Permissions.Creative))
            this.Permission |= Permissions.SystemShops;
        }
        if (version < 227)
          this.Permission |= Permissions.TextChat;
      }
      else
        this.isLobbyPermission = false;
      this.Reach = playerData.Reach;
      this.ClanName = playerData.ClanName == null || playerData.ClanName.Length <= 0 ? (string) null : playerData.ClanName;
      this.ClanBannerID = playerData.ClanBannerID;
      this.Waypoint = playerData.Waypoint;
      this.History = new History(playerData.History);
      this.ActionLog = new ActionLog(playerData.ActionLog);
      this.LastTransmitterFrequency = (ushort) playerData.LastTransmitterFrequency;
      this.UserControls.Initialize(this.Settings.UserControlSetting);
      if (this.Settings.MobType == ActorType.None)
        this.Settings.MobType = ActorType.Boy;
      this.ActorType = this.Settings.MobType;
      this.SetAvatar(this.ActorType, true);
      if (!this.instance.IsDigDeepMode)
        this.Settings.BlueprintFinderVisible = false;
      if (this.instance.IsLegendaryDifficulty)
      {
        this.Settings.MapVisible = false;
        this.Settings.BlueprintFinderVisible = false;
      }
      if (!this.HasPermission(Permissions.Map))
        this.Settings.MapVisible = false;
      if (!playerData.IsNewPlayer)
      {
        this.Health = playerData.Health;
        this.Oxygen = playerData.Oxygen;
      }
      this.FlyMode = playerData.JetPackActive ? FlyMode.Slow : FlyMode.None;
      if ((double) this.Health <= 0.0)
      {
        this.actorState = ActorState.Dying;
        this.deathRecoveryTimer = this.deathRecoveryTime * 0.5f;
      }
      this.viewAngle = playerData.ViewAngle;
      this.UpdateMatrices();
      if (playerData.IsNewPlayer)
      {
        this.AddInitialInventory();
      }
      else
      {
        this.Inventory.LoadFromSaveData(playerData.Inventory);
        if (this.Inventory.HotBarRightSlotID >= 0)
          this.RightHand.SetItem(this.Inventory[this.Inventory.HotBarRightSlotID].ItemID);
        if (this.Inventory.HotBarLeftSlotID >= 0)
          this.LeftHand.SetItem(this.Inventory[this.Inventory.HotBarLeftSlotID].ItemID);
      }
      if (!this.IsAllowedToFly)
      {
        this.FlyMode = FlyMode.None;
        this.isFlightAscending = this.isFlightDescending = false;
      }
      this.rebuildDepthStringLastPoint = GlobalPoint3D.Zero;
      this.RebuildDepthString();
      this.Inventory.ItemChanged += new InventoryEventHandler(this.OnInventoryItemChanged);
      if (!this.IsLocalGamer)
        return;
      this.instance.NetworkManager.SendPlayerLoaded(this, playerData);
      playerData.IsNewPlayer = false;
    }

    public void OnWieldTypeChanged(WieldType oldType)
    {
      if (this.Inventory == null)
        return;
      int hotBarLeftSlotId = this.HotBarLeftSlotID;
      int hotBarRightSlotId = this.HotBarRightSlotID;
      this.Inventory.HotBarLeftSlotID = this.Inventory.HotBarRightSlotID = -1;
      if (this.WieldType == WieldType.LeftHand)
        this.SetLeftHotBarSlot(hotBarLeftSlotId);
      else if (this.WieldType == WieldType.RightHand)
      {
        this.SetRightHotBarSlot(hotBarRightSlotId);
      }
      else
      {
        this.SetRightHotBarSlot(hotBarRightSlotId);
        this.SetLeftHotBarSlot(hotBarRightSlotId);
      }
      this.LeftHand.UpdateSwing();
      this.RightHand.UpdateSwing();
    }

    private void OnInventoryItemChanged(object sender, InventoryItemEventArgs e)
    {
      if (this.Inventory[e.SlotID].ItemID == e.Item.ItemID)
        return;
      bool flag = false;
      switch (this.Settings.WieldType)
      {
        case WieldType.LeftHand:
        case WieldType.RightHand:
          Hand hand = this.Settings.WieldType == WieldType.LeftHand ? this.LeftHand : this.RightHand;
          if (e.SlotID == hand.HandIndex)
          {
            hand.SetItem(this.Inventory[e.SlotID].ItemID);
            flag = true;
            break;
          }
          if (e.SlotID == hand.OtherHand.HandIndex)
          {
            if (this.IsSingleWieldHandsBoundByItem(hand.ItemID, this.Inventory[e.SlotID].ItemID))
              this.SetHotBarSlot(hand.OtherHand, hand.HandType == InventoryHand.Left ? this.HotBarLeftSlotID : this.HotBarRightSlotID);
            else
              hand.OtherHand.SetItem(this.Inventory[e.SlotID].ItemID);
            flag = true;
            break;
          }
          break;
        default:
          if (this.Inventory.LeftHandIndex == this.Inventory.RightHandIndex && e.SlotID == this.Inventory.LeftHandIndex)
          {
            Item itemId = this.Inventory[e.SlotID].ItemID;
            if (itemId != this.LeftHand.ItemID && itemId != this.RightHand.ItemID)
            {
              if (itemId == Item.None)
              {
                this.LeftHand.SetItem(itemId);
                this.RightHand.SetItem(itemId);
                flag = true;
                break;
              }
              if (this.LeftHand.HasItem && !this.RightHand.HasItem)
              {
                this.LeftHand.SetItem(itemId);
                flag = true;
                break;
              }
              if (this.RightHand.HasItem && !this.LeftHand.HasItem)
              {
                this.RightHand.SetItem(itemId);
                flag = true;
                break;
              }
              if (ItemData.GetItemEquipIndex(itemId) == EquipIndex.RightHand)
              {
                this.RightHand.SetItem(itemId);
                flag = true;
                break;
              }
              this.LeftHand.SetItem(itemId);
              flag = true;
              break;
            }
            break;
          }
          if (e.SlotID == this.Inventory.LeftHandIndex)
          {
            this.LeftHand.SetItem(this.Inventory[e.SlotID].ItemID);
            flag = true;
            break;
          }
          if (e.SlotID == this.Inventory.RightHandIndex)
          {
            this.RightHand.SetItem(this.Inventory[e.SlotID].ItemID);
            flag = true;
            break;
          }
          break;
      }
      if (flag)
        return;
      this.ExecuteItemEquipEventScript(e.Item.ItemID, this.Inventory[e.SlotID].ItemID, e.SlotID);
    }

    public void ExecuteItemEquipEventScript(Item oldItemID, Item newItemID, int slotID)
    {
      EquipIndex itemEquipIndex1 = ItemData.GetItemEquipIndex(oldItemID);
      switch (itemEquipIndex1)
      {
        case EquipIndex.LeftHand:
        case EquipIndex.RightHand:
          if ((itemEquipIndex1 == EquipIndex.LeftHand || itemEquipIndex1 == EquipIndex.RightHand) && (slotID != this.Inventory.LeftHandIndex && slotID != this.Inventory.RightHandIndex))
          {
            oldItemID = Item.None;
            break;
          }
          break;
        default:
          if (this.Inventory.GetEquipSlotID(itemEquipIndex1) != slotID)
          {
            oldItemID = Item.None;
            break;
          }
          goto case EquipIndex.LeftHand;
      }
      EquipIndex itemEquipIndex2 = ItemData.GetItemEquipIndex(newItemID);
      switch (itemEquipIndex2)
      {
        case EquipIndex.LeftHand:
        case EquipIndex.RightHand:
          if ((itemEquipIndex2 == EquipIndex.LeftHand || itemEquipIndex2 == EquipIndex.RightHand) && (slotID != this.Inventory.LeftHandIndex && slotID != this.Inventory.RightHandIndex))
          {
            newItemID = Item.None;
            break;
          }
          break;
        default:
          if (this.Inventory.GetEquipSlotID(itemEquipIndex2) != slotID)
          {
            newItemID = Item.None;
            break;
          }
          goto case EquipIndex.LeftHand;
      }
      this.instance.ExecuteItemEquipEventScript((Actor) this, oldItemID, newItemID);
    }

    private void AddInitialInventory()
    {
      if (this.instance.IsCreativeMode)
      {
        if (this.Gamer.IsHost)
        {
          this.Inventory[0] = new InventoryItem(Item.Marker, 100);
          this.Inventory[1] = new InventoryItem(Item.SledgeHammer, 1);
        }
        else
          this.Inventory.CopyFromWithPreClear(this.instance.SpawnInventory);
        this.SetLeftHotBarSlot(0);
        this.SetRightHotBarSlot(1);
      }
      else
      {
        if (!this.Gamer.IsHost || this.instance.IsLegendaryDifficulty)
          return;
        if (this.instance.IsDigDeepMode)
        {
          this.AddToInventory(Item.GoldPieces, 500);
          this.AddToInventory(Item.Obsidian, 2);
          this.AddToInventory(Item.ItemShop, 1);
          this.AddToInventory(Item.WoodPickaxe, 1);
          this.SetLeftHotBarSlot(2);
          this.SetRightHotBarSlot(3);
        }
        else if (this.instance.IsPeacefulMode || this.instance.IsSurvivalMode && this.instance.IsEasyDifficulty)
        {
          this.AddToInventory(Item.ItemShop, 1);
          this.AddToInventory(Item.WoodPickaxe, 1);
          this.SetLeftHotBarSlot(0);
          this.SetRightHotBarSlot(1);
        }
        else
        {
          this.Inventory[1] = new InventoryItem(Item.WoodPickaxe, 1);
          this.SetLeftHotBarSlot(0);
          this.SetRightHotBarSlot(1);
        }
      }
    }

    private bool CheckLastGoodPosition()
    {
      Vector3 position = this.Position;
      position.Y += 0.02f;
      GlobalPoint3D point = this.map.GetPoint(position);
      if (this.map.IsPassable(point))
      {
        if (this.IsCrouching)
          return true;
        ++point.Y;
        if (this.map.IsPassable(point))
          return true;
      }
      return false;
    }

    public void ClearInput()
    {
      this.inputEnabled = false;
      this.ClearInputCore();
    }

    private void ClearInputCore()
    {
      this.LeftHand.ClearSwing();
      this.RightHand.ClearSwing();
      this.movementInput = Vector2.Zero;
      this.viewInput = Vector2.Zero;
      this.jumpingInput = false;
      this.isFlightAscending = this.isFlightDescending = false;
      if (this.rumble == null)
        return;
      this.rumble.SetNewRumble(0.0f, 0.0f, 0.0f);
    }

    public bool IsReticleVisible
    {
      get
      {
        return this.DialogHandler.CurrentDialog == null;
      }
    }

    protected override bool HandleInputCore(InputState input, PlayerIndex dontuse)
    {
      return false;
    }

    public bool HandleInput(GamePadState pad, GamePadState lastpad)
    {
      if (this.IsConsoleOpen)
        return false;
      if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenConsole))
      {
        this.OpenConsole();
        return true;
      }
      this.inputEnabled = true;
      this.adjustedSunMoonThisFrame = false;
      bool flag = false;
      this.pad = pad;
      this.lastpad = lastpad;
      if (this.rumble == null)
        this.rumble = new Rumble(this);
      if (!this.IsViewingMainMap)
        flag = this.HandlePlayInput();
      else
        this.ClearInputCore();
      if (this.adjustedSunMoonLastFrame != this.adjustedSunMoonThisFrame)
      {
        this.adjustedSunMoonLastFrame = this.adjustedSunMoonThisFrame;
        this.GameInstance.NetworkManager.SendGameState(true);
      }
      return flag;
    }

    private bool HandlePlayInput()
    {
      this.LeftHand.ClearSwing();
      this.RightHand.ClearSwing();
      if (this.DialogHandler.IsActive)
      {
        if (this.DialogHandler.HandleInput())
          return true;
      }
      else if (this.IsCCTVView)
      {
        if (this.HandleCCTVInput())
          return true;
      }
      else if (this.binocularsView && this.HandleBinocularInput())
        return true;
      this.jumpingInput = false;
      this.isFlightAscending = this.isFlightDescending = false;
      if (!this.IsDeadOrInactiveOrDisabled)
      {
        if (this.CurrentArcadeMachine != null && this.CurrentArcadeMachine.HandleInput())
        {
          this.movementInput = this.viewInput = Vector2.Zero;
          return true;
        }
        if (this.HandlePlayInputWhenSpecialKeyPressed())
          return true;
        if (this.ButtonEventScriptPressed())
        {
          this.ClearRubberBanding();
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.DropLeftItem))
        {
          this.DropItem(ParticleType.None, this.HotBarLeftSlotID, UpdateBlockMethod.DropTimeLong);
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.DropRightItem))
        {
          this.DropItem(ParticleType.None, this.HotBarRightSlotID, UpdateBlockMethod.DropTimeLong);
          return true;
        }
        if (this.InteractButtonPressed())
        {
          this.ClearRubberBanding();
          this.ProspectBlock();
          return true;
        }
        if (this.CheckHotBarButtons() || this.LeftHand.HandleInput(this.pad, this.lastpad) || (this.RightHand.HandleInput(this.pad, this.lastpad) || this.CheckCrouchButtons()) || (this.CheckCreativeModeButtons() || this.CheckMapButtons() || (this.CheckInventoryButtons() || this.CheckPauseMenuButtons())))
          return true;
        this.movementInput = this.GetMovementInput();
        this.viewInput = this.GetLookAroundInput();
        if (!this.IceEffectActive)
        {
          if (this.CheckFlyButtons())
            return true;
          this.jumpingInput = this.GetJump();
        }
        foreach (Mod activePlugin in ModManager.ActivePlugins)
        {
          if (activePlugin.Plugin.HandleInput((ITMPlayer) this))
            return true;
        }
        if (this.IsGodOrTester && InputManager.IsKeyReleasedNew(this.PlayerIndex, Keys.P))
        {
          this.instance.AddScreen((GameScreen) new PlayerMenuScreen(this.instance, this), this);
          return true;
        }
      }
      else if (this.actorState == ActorState.Dying)
      {
        this.movementInput = Vector2.Zero;
        this.viewInput = this.GetLookAroundInput();
        if (this.CheckCreativeModeButtons() || this.CheckMapButtons() || this.CheckPauseMenuButtons())
          return true;
      }
      return false;
    }

    public bool IsCCTVView
    {
      get
      {
        if (this.virtualPlayer != null && this.virtualPlayer.Gamer == null)
          return (double) this.saveFOVNormalised > 0.0;
        return false;
      }
    }

    public bool IsSpectating
    {
      get
      {
        if (this.virtualPlayer != null)
          return this.virtualPlayer.Gamer != null;
        return false;
      }
    }

    public bool ShowItemsInHand
    {
      get
      {
        if (!this.binocularsView)
          return !this.fovInterpolator.IsActive;
        return false;
      }
    }

    public bool IsBinocularView
    {
      get
      {
        return this.binocularsView;
      }
    }

    private bool HandleCCTVInput()
    {
      if (this.virtualPlayer == null || this.virtualPlayer.Settings == null || InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.BackButton))
      {
        this.EndCCTV();
      }
      else
      {
        if (this.CheckInventoryButtons() || this.CheckPauseMenuButtons())
          return true;
        bool flag = this.virtualPlayer.cctvTarget != null;
        if ((double) InputManager1.Profile.GamePadSensitivity > 0.0)
        {
          float fovNormalized = this.virtualPlayer.FOVNormalized;
          float num = (float) (0.0199999995529652 * ((double) this.virtualPlayer.fov / (double) this.virtualPlayer.FOVRange.Y));
          if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.ZoomOut))
          {
            this.virtualPlayer.FOVNormalized = Math.Min(1f, fovNormalized + num);
            this.FOVNormalized = this.virtualPlayer.FOVNormalized;
          }
          else if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.ZoomIn))
          {
            this.virtualPlayer.FOVNormalized = Math.Max(0.0f, fovNormalized - num);
            this.FOVNormalized = this.virtualPlayer.FOVNormalized;
          }
          if (!flag)
          {
            this.virtualPlayer.viewInput = this.GetLookAroundInput();
            this.virtualPlayer.UpdateViewDirection();
          }
        }
        this.movementInput = this.GetMovementInput();
        this.jumpingInput = this.GetJump();
        if (flag)
        {
          this.viewInput = this.GetLookAroundInput();
          this.virtualPlayer.ViewDirection = Vector3.Normalize(this.virtualPlayer.cctvTarget.EyePosition - this.virtualPlayer.EyePosition);
        }
        this.virtualPlayer.UpdateMatrices();
      }
      return true;
    }

    private bool HandleBinocularInput()
    {
      if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.LeftHand))
        ++this.leftTriggerReleased;
      if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.RightHand))
        ++this.rightTriggerReleased;
      if (InputManager1.IsInputChanged(this.PlayerIndex, PlayerInput.BackButton) || this.leftTriggerReleased > 1 || this.rightTriggerReleased > 1)
      {
        this.EndBinocularView();
      }
      else
      {
        if (this.CheckFlyButtons() || this.CheckInventoryButtons() || this.CheckPauseMenuButtons())
          return true;
        float fovNormalized = this.FOVNormalized;
        float num = (float) (0.0199999995529652 * ((double) this.fov / (double) this.FOVRange.Y));
        int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.PlayerIndex);
        if (mouseWheelDelta != 0)
          this.FOVNormalized = Math.Max(Math.Min(1f, fovNormalized - (float) ((double) num * (double) mouseWheelDelta * 0.0399999991059303)), 0.0f);
        else if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.ZoomOut))
          this.FOVNormalized = Math.Min(1f, fovNormalized + num);
        else if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.ZoomIn))
          this.FOVNormalized = Math.Max(0.0f, fovNormalized - num);
        this.movementInput = this.GetMovementInput();
        this.viewInput = this.GetLookAroundInput();
        this.jumpingInput = this.GetJump();
      }
      return true;
    }

    private bool CheckHotBarButtons()
    {
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.PlayerIndex);
      if (this.Settings.WieldType == WieldType.LeftHand)
      {
        if (mouseWheelDelta > 0)
        {
          this.MoveLeftHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        if (mouseWheelDelta < 0)
        {
          this.MoveLeftHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarLeft))
        {
          this.MoveLeftHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarRight))
        {
          this.MoveLeftHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        Keys numKeyPressedNew = InputManager.GetNumKeyPressedNew(this.PlayerIndex);
        if (numKeyPressedNew != Keys.None)
        {
          int num = (int) (numKeyPressedNew - 48);
          this.SetLeftHotBarSlot(num != 0 ? num - 1 : 9);
          this.ClearRubberBanding();
          return true;
        }
      }
      else if (this.Settings.WieldType == WieldType.RightHand)
      {
        if (mouseWheelDelta > 0)
        {
          this.MoveRightHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        if (mouseWheelDelta < 0)
        {
          this.MoveRightHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarLeft))
        {
          this.MoveRightHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarRight))
        {
          this.MoveRightHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        Keys numKeyPressedNew = InputManager.GetNumKeyPressedNew(this.PlayerIndex);
        if (numKeyPressedNew != Keys.None)
        {
          int num = (int) (numKeyPressedNew - 48);
          this.SetRightHotBarSlot(num != 0 ? num - 1 : 9);
          this.ClearRubberBanding();
          return true;
        }
      }
      else
      {
        if (mouseWheelDelta > 0)
        {
          if (InputManager.IsKeyPressed(this.PlayerIndex, Keys.LeftShift))
            this.MoveLeftHotBarCursor(-1);
          else
            this.MoveRightHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        if (mouseWheelDelta < 0)
        {
          if (InputManager.IsKeyPressed(this.PlayerIndex, Keys.LeftShift))
            this.MoveLeftHotBarCursor(1);
          else
            this.MoveRightHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        Keys numKeyPressedNew = InputManager.GetNumKeyPressedNew(this.PlayerIndex);
        if (numKeyPressedNew != Keys.None)
        {
          int num = (int) (numKeyPressedNew - 48);
          int slotID = num != 0 ? num - 1 : 9;
          if (InputManager.IsKeyPressed(this.PlayerIndex, Keys.LeftShift))
            this.SetLeftHotBarSlot(slotID);
          else
            this.SetRightHotBarSlot(slotID);
          this.ClearRubberBanding();
          return true;
        }
        if (!this.leftHotbarButtonFirst && !this.rightHotbarButtonFirst)
        {
          if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.HotbarLeft))
          {
            this.leftHotbarButtonFirst = true;
            this.leftHotbarCursorHasVisualPriority = true;
            this.hotbarButtonHeldTimer = 0.0f;
          }
          else if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.HotbarRight))
          {
            this.rightHotbarButtonFirst = true;
            this.leftHotbarCursorHasVisualPriority = false;
            this.hotbarButtonHeldTimer = 0.0f;
          }
        }
        if (this.leftHotbarButtonFirst)
        {
          this.hotBarToTransparencyTimer = 0.0f;
          return this.CheckHotBarLeftButtons();
        }
        if (this.rightHotbarButtonFirst)
        {
          this.hotBarToTransparencyTimer = 0.0f;
          return this.CheckHotBarRightButtons();
        }
        this.movedHotBarReverse = false;
      }
      this.hotBarToTransparencyTimer += Services.ElapsedTime;
      return false;
    }

    private bool CheckHotBarLeftButtons()
    {
      if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.HotbarLeft))
      {
        this.hotbarButtonHeldTimer += Services.ElapsedTime;
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarRight))
        {
          this.MoveLeftHotBarCursor(1);
          this.ClearRubberBanding();
          this.movedHotBarReverse = true;
          return true;
        }
      }
      else
      {
        if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.HotbarLeft) && !this.movedHotBarReverse)
        {
          if ((double) this.hotbarButtonHeldTimer < 1.0)
            this.MoveLeftHotBarCursor(-1);
          this.ClearRubberBanding();
          return true;
        }
        this.leftHotbarButtonFirst = false;
      }
      return false;
    }

    private bool CheckHotBarRightButtons()
    {
      if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.HotbarRight))
      {
        this.hotbarButtonHeldTimer += Services.ElapsedTime;
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.HotbarLeft))
        {
          this.MoveRightHotBarCursor(-1);
          this.ClearRubberBanding();
          this.movedHotBarReverse = true;
          return true;
        }
      }
      else
      {
        if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.HotbarRight) && !this.movedHotBarReverse)
        {
          if ((double) this.hotbarButtonHeldTimer < 1.0)
            this.MoveRightHotBarCursor(1);
          this.ClearRubberBanding();
          return true;
        }
        this.rightHotbarButtonFirst = false;
      }
      return false;
    }

    private bool CheckCreativeModeButtons()
    {
      if (this.instance.IsCreativeMode || this.IsGodOrTester)
      {
        if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenCreativeMenu))
        {
          if (this.instance.CanOpenCreativeMenu(this))
          {
            if (Globals2.UseOldMenu)
              this.instance.AddScreen((GameScreen) new CreativeMenuScreen(this.instance, this), this);
            else
              this.instance.AddScreen((GameScreen) new PauseMenuScreen2(this.instance, this, 2), this);
          }
          return true;
        }
        if ((this.HasPermission(Permissions.Admin) || !this.instance.IsFiniteResources && this.HasPermission(Permissions.Creative)) && InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenShop))
        {
          this.instance.AddScreen((GameScreen) new PauseMenuScreen2(this.instance, this, (NewGuiMenu) new ShopMenu(this.instance, this)), this);
          return true;
        }
      }
      return false;
    }

    private bool CheckCrouchButtons()
    {
      bool isHeldCrouch = this.isHeldCrouch;
      if (this.IsCrouchButtonPressed() && this.FlyMode == FlyMode.None && !this.isOnLadder)
        this.Crouch();
      else
        this.Uncrouch();
      return isHeldCrouch != this.isHeldCrouch;
    }

    private bool CheckFlyButtons()
    {
      if (!this.IsAllowedToFly)
      {
        this.FlyMode = FlyMode.None;
        this.isFlightAscending = this.isFlightDescending = false;
      }
      else
      {
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.Fly))
        {
          switch (this.FlyMode)
          {
            case FlyMode.None:
              this.FlyMode = FlyMode.Slow;
              break;
            case FlyMode.Slow:
              this.FlyMode = FlyMode.Fast;
              break;
            default:
              this.FlyMode = FlyMode.None;
              break;
          }
          this.DisableFlyIfInNoFlyZone();
          this.ClearRubberBanding();
        }
        if (this.FlyMode != FlyMode.None)
        {
          this.isFlightAscending = InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.FlyAscend);
          this.isFlightDescending = InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.FlyDescend);
        }
      }
      return false;
    }

    public void DisableFlyIfInNoFlyZone()
    {
      if (this.FlyMode == FlyMode.None || this.IsAdmin || !this.IsInNoFlyZone)
        return;
      this.FlyMode = FlyMode.None;
      this.isFlightAscending = this.isFlightDescending = false;
    }

    private bool IsInNoFlyZone
    {
      get
      {
        return this.instance.MapStrategyTM.IsInZoneType(this.Box, ZoneType.NoFly, this.GamerID);
      }
    }

    private bool CheckInventoryButtons()
    {
      if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenInventory))
      {
        this.instance.AddScreen((GameScreen) new InventoryScreen(this.instance, this, (Actor) this), this);
        this.ClearRubberBanding();
        return true;
      }
      if (!InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenCrafting))
        return false;
      this.instance.AddScreen((GameScreen) new CraftingScreen(this.instance, this), this);
      this.ClearRubberBanding();
      return true;
    }

    private bool CheckPauseMenuButtons()
    {
      if (InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenPauseMenu))
      {
        if (Globals2.UseOldMenu)
          this.instance.AddScreen(this.PauseMenuScreen = (GameScreen) new StudioForge.TotalMiner.Screens.PauseMenuScreen(this.instance, this), this);
        else
          this.instance.AddScreen(this.PauseMenuScreen = (GameScreen) new PauseMenuScreen2(this.instance, this), this);
        this.ClearRubberBanding();
        return true;
      }
      if (!InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.RebuildLocalLight))
        return false;
      this.instance.RebuildLocalLight(this);
      return true;
    }

    private bool CheckMapButtons()
    {
      if (!InputManager1.IsInputReleasedNew(this.PlayerIndex, PlayerInput.OpenMap))
        return false;
      if (!this.instance.IsSplitScreen)
      {
        if (this.HasPermission(Permissions.Map))
          this.instance.AddScreen((GameScreen) new MapTopViewScreen(this.instance, this), this);
      }
      else
        this.instance.AddScreen((GameScreen) new MessageBoxScreenTM("Overhead Map is not available in split screen mode", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this), this);
      return true;
    }

    private bool HandlePlayInputWhenSpecialKeyPressed()
    {
      if (!InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.Special))
        return false;
      if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.OpenTextChat))
      {
        if (this.HasPermission(Permissions.TextChat))
          this.instance.AddScreen((GameScreen) new TextMessageMenuScreen(this.instance, this, (string) null), this);
        return true;
      }
      if (this.IsClipboardEquipped)
      {
        if ((double) this.pad.ThumbSticks.Right.X != 0.0 || (double) this.pad.ThumbSticks.Right.Y != 0.0)
        {
          this.ClipboardZoom = MathHelper.Clamp(this.ClipboardZoom + this.pad.ThumbSticks.Right.Y * 0.1f, 0.4f, 15f);
          this.ClipboardRotate = MathHelper.WrapAngle(this.ClipboardRotate + this.pad.ThumbSticks.Right.X * 0.15f);
        }
        else if (InputManager.IsKeyPressed(this.PlayerIndex, Keys.LeftShift))
          this.ClipboardRotate = MathHelper.WrapAngle(this.ClipboardRotate + (float) InputManager.GetMouseWheelDelta(this.PlayerIndex) * 0.15f);
        else
          this.ClipboardZoom = MathHelper.Clamp(this.ClipboardZoom + (float) InputManager.GetMouseWheelDelta(this.PlayerIndex) * 0.0015f, 0.4f, 15f);
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.ClipboardPasteMerge))
        {
          this.PasteClipboardModel(Map.CopyType.Merge);
          if (this.Settings.WieldType == WieldType.RightHand)
            this.RightHand.SetIsSwinging(true);
          else
            this.LeftHand.SetIsSwinging(true);
        }
        else if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.ClipboardPasteNoOverwrite))
        {
          this.PasteClipboardModel(Map.CopyType.NoOverwrite);
          if (this.Settings.WieldType == WieldType.RightHand)
            this.RightHand.SetIsSwinging(true);
          else
            this.LeftHand.SetIsSwinging(true);
        }
        return true;
      }
      if (this.HasPermission(Permissions.Spectate))
      {
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.SpectatePrev))
        {
          this.SelectVirtualPlayer(-1);
          return true;
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.SpectateNext))
        {
          this.SelectVirtualPlayer(1);
          return true;
        }
      }
      if (this.HasPermission(Permissions.Admin) && (this.instance.IsCreativeMode || this.IsGodOrTester && this.instance.SunMoon != null))
      {
        if (this.instance.SunMoon != null)
        {
          float num = this.pad.ThumbSticks.Right.Y;
          if ((double) num == 0.0)
            num = (float) InputManager.GetMouseWheelDelta(this.PlayerIndex) * 0.015f;
          if (this.adjustedSunMoonThisFrame = (double) num != 0.0)
          {
            this.instance.SunMoon.Rotation += 0.01f * num;
            this.instance.SunMoon.Rotation = MyMathHelper.WrapAngle(this.instance.SunMoon.Rotation);
            return true;
          }
        }
        if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.NoClip))
        {
          if ((this.isNoClipMode = !this.isNoClipMode) && this.FlyMode == FlyMode.None)
            this.FlyMode = FlyMode.Slow;
          this.instance.AddNotification("No Clip Mode " + (this.isNoClipMode ? "Activated" : "Deactivated"), NotifyRecipient.Local);
          return true;
        }
      }
      return true;
    }

    private bool InteractButtonPressed()
    {
      if (!this.IsInputEnabled)
        return false;
      return InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.Interact);
    }

    private bool ButtonEventScriptPressed()
    {
      return this.IsInputEnabled && (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.EventScriptX) && this.ExecuteButtonScript(Buttons.X) || InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.EventScriptY) && this.ExecuteButtonScript(Buttons.Y) || InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.EventScriptB) && this.ExecuteButtonScript(Buttons.B));
    }

    private bool ExecuteButtonScript(Buttons button)
    {
      Player.ButtonScript buttonScript;
      if (!this.buttonScripts.TryGetValue(button, out buttonScript))
        return false;
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) this
      };
      this.instance.ExecuteScript(buttonScript.Script, data, true);
      return true;
    }

    private bool BotRightTriggerPressed()
    {
      if (!this.SwingTargetIsValid)
        return false;
      this.BotSelectRightHand(this.GetBotTool((Block) this.map.GetBlockID(this.SwingTarget)));
      return true;
    }

    private void BotSelectRightHand(Item item)
    {
      for (int index = 0; index < this.Inventory.Count; ++index)
      {
        if (this.Inventory[index].ItemID == item)
          return;
      }
      int getFreeSlotForItem = this.Inventory.FindOrGetFreeSlotForItem(item);
      if (getFreeSlotForItem < 0)
      {
        this.DropAllItems((Item[]) null, UpdateBlockMethod.DropTimeShort);
        getFreeSlotForItem = this.Inventory.FindOrGetFreeSlotForItem(item);
        if (getFreeSlotForItem < 0)
          return;
      }
      this.Inventory[getFreeSlotForItem] = new InventoryItem(item, 1);
    }

    private void BotSelectLeftHand(Item item)
    {
      for (int index = 0; index < this.Inventory.Count; ++index)
      {
        if (this.Inventory[index].ItemID == item)
          return;
      }
      int getFreeSlotForItem = this.Inventory.FindOrGetFreeSlotForItem(item);
      if (getFreeSlotForItem < 0)
      {
        this.DropAllItems((Item[]) null, UpdateBlockMethod.DropTimeShort);
        getFreeSlotForItem = this.Inventory.FindOrGetFreeSlotForItem(item);
        if (getFreeSlotForItem < 0)
          return;
      }
      this.Inventory[getFreeSlotForItem] = new InventoryItem(item, this.instance.Random.Next(1, 5));
    }

    private Item GetBotTool(Block blockID)
    {
      Block block = blockID;
      if ((uint) block <= 71U)
      {
        switch (block)
        {
          case Block.Grass:
          case Block.Dirt:
          case Block.Sand:
          case Block.Scoria:
            break;
          case Block.Wood:
          case Block.Leaves:
          case Block.PineLeaves:
          case Block.WovenLeaves:
            goto label_14;
          default:
            goto label_23;
        }
      }
      else if ((uint) block <= 145U)
      {
        switch (block)
        {
          case Block.GrassyStone:
          case Block.Snow:
          case Block.SnowLayer:
            break;
          default:
            goto label_23;
        }
      }
      else
      {
        switch (block)
        {
          case Block.GrassShaded:
          case Block.SaltBlock:
            break;
          case Block.MapleLeaves:
            goto label_14;
          default:
            goto label_23;
        }
      }
      SkillData digging = this.SkillsData.Digging;
      if (digging.Level >= Globals1.SkillData[270].UseReq)
        return Item.DiamondShovel;
      if (digging.Level >= Globals1.SkillData[405].UseReq)
        return Item.GreenstoneGoldShovel;
      if (digging.Level >= Globals1.SkillData[269].UseReq)
        return Item.SteelShovel;
      return digging.Level >= Globals1.SkillData[268].UseReq ? Item.IronShovel : Item.WoodShovel;
label_14:
      SkillData chopping = this.SkillsData.Chopping;
      if (chopping.Level >= Globals1.SkillData[266].UseReq)
        return Item.DiamondHatchet;
      if (chopping.Level >= Globals1.SkillData[404].UseReq)
        return Item.GreenstoneGoldHatchet;
      if (chopping.Level >= Globals1.SkillData[265].UseReq)
        return Item.SteelHatchet;
      return chopping.Level >= Globals1.SkillData[264].UseReq ? Item.IronHatchet : Item.WoodHatchet;
label_23:
      SkillData mining = this.SkillsData.Mining;
      if (mining.Level >= Globals1.SkillData[262].UseReq)
        return Item.TitaniumPickaxe;
      if (mining.Level >= Globals1.SkillData[261].UseReq)
        return Item.RubyPickaxe;
      if (mining.Level >= Globals1.SkillData[260].UseReq)
        return Item.DiamondPickaxe;
      if (mining.Level >= Globals1.SkillData[403].UseReq)
        return Item.GreenstoneGoldPickaxe;
      if (mining.Level >= Globals1.SkillData[259].UseReq)
        return Item.SteelPickaxe;
      return mining.Level >= Globals1.SkillData[258].UseReq ? Item.IronPickaxe : Item.WoodPickaxe;
    }

    private bool IsCrouchButtonPressed()
    {
      if (!this.IsInputEnabled)
        return this.isHeldCrouch;
      bool flag = InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.Crouch);
      if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.CrouchHold))
        this.isHeldCrouch = flag;
      if (!flag)
        return this.isHeldCrouch;
      return true;
    }

    private void Crouch()
    {
      this.Crouch(2f * this.RotationSpeedModifier);
    }

    public void MoveLeftHotBarCursor(int dir)
    {
      int slotID = this.Inventory.HotBarLeftSlotID + dir;
      if (slotID < 0)
        slotID = 9;
      else if (slotID > 9)
        slotID = 0;
      this.SetLeftHotBarSlot(slotID, true);
    }

    public void MoveRightHotBarCursor(int dir)
    {
      int slotID = this.Inventory.HotBarRightSlotID + dir;
      if (slotID < 0)
        slotID = 9;
      else if (slotID > 9)
        slotID = 0;
      this.SetRightHotBarSlot(slotID, true);
    }

    private void SetHotBarSlot(Hand hand, int slotID)
    {
      if (hand == null)
        return;
      if (hand.HandType == InventoryHand.Left)
      {
        this.SetLeftHotBarSlot(slotID);
      }
      else
      {
        if (hand.HandType != InventoryHand.Right)
          return;
        this.SetRightHotBarSlot(slotID);
      }
    }

    public void SetLeftHotBarSlot(int slotID)
    {
      this.SetLeftHotBarSlot(slotID, false);
    }

    public void SetLeftHotBarSlot(int slotID, bool soft)
    {
      int hotBarLeftSlotId = this.HotBarLeftSlotID;
      if (slotID == hotBarLeftSlotId || this.Inventory == null || this.LeftHand == null)
        return;
      this.CheckForClipboardLoadUnLoad(this.Inventory.HotBarLeftSlotID, slotID);
      this.Inventory.HotBarLeftSlotID = slotID;
      this.LeftHand.SetItem(this.Inventory[slotID].ItemID, !soft);
    }

    public void SetRightHotBarSlot(int slotID)
    {
      this.SetRightHotBarSlot(slotID, false);
    }

    public void SetRightHotBarSlot(int slotID, bool soft)
    {
      int hotBarRightSlotId = this.HotBarRightSlotID;
      if (slotID == hotBarRightSlotId || this.Inventory == null || this.RightHand == null)
        return;
      this.Inventory.HotBarRightSlotID = slotID;
      this.RightHand.SetItem(this.Inventory[slotID].ItemID, !soft);
    }

    private bool IsAllowedToFly
    {
      get
      {
        if (this.HasFlyConcession)
          return true;
        if (!this.HasPermission(Permissions.Fly, true))
          return false;
        if (!this.instance.IsCreativeMode)
          return this.IsItemEquippedAndUsable(Item.AmuletOfFlight);
        return true;
      }
    }

    private bool HasFlyConcession
    {
      get
      {
        if (!this.IsRoboticAvatar && !this.IsHermesWraithAvatar)
          return this.IsGodOrTesterRetail;
        return true;
      }
    }

    private void SelectVirtualPlayer(int dir)
    {
      int gamerEnabledCount = this.instance.NetworkManager.AllGamerEnabledCount;
      Player virtualPlayer = this.VirtualPlayer;
      List<NetworkGamer> allEnabledGamers = this.instance.NetworkManager.AllEnabledGamers;
      if (allEnabledGamers.Count > 0)
      {
        for (int index1 = 0; index1 < allEnabledGamers.Count; ++index1)
        {
          Player tag = allEnabledGamers[index1].Tag as Player;
          if (virtualPlayer == tag)
          {
            int index2 = index1 + dir;
            if (index2 >= allEnabledGamers.Count)
              index2 = 0;
            else if (index2 < 0)
              index2 = allEnabledGamers.Count - 1;
            this.virtualPlayer = allEnabledGamers[index2].Tag as Player;
            return;
          }
        }
      }
      this.virtualPlayer = (Player) null;
    }

    protected override void OnItemDropped(InventoryItem item, int slotID)
    {
      this.Raise_ItemDropped(item.ItemID);
      switch (item.ItemID)
      {
        case Item.Binoculars:
          if (!this.IsBinocularView || !this.IsCorrectEquipSlot(item.ItemID, slotID))
            break;
          this.EndBinocularView();
          break;
        case Item.Clipboard:
          this.DiscardClipboard(item, slotID);
          break;
      }
    }

    public void SetButtonScript(
      Buttons button,
      string scriptName,
      string text,
      Vector2? pos,
      float? scale)
    {
      if (scriptName.IsEmpty() || text.IsEmpty())
      {
        if (!this.buttonScripts.ContainsKey(button))
          return;
        this.buttonScripts.Remove(button);
      }
      else
      {
        Player.ButtonScript buttonScript = new Player.ButtonScript()
        {
          Script = scriptName.IsNotEmpty() ? scriptName : (string) null,
          Text = text.IsNotEmpty() ? Globals2.SubstituteText(text, this.instance, this) : (string) null,
          Pos = pos,
          Scale = scale
        };
        if (this.buttonScripts.ContainsKey(button))
          this.buttonScripts[button] = buttonScript;
        else
          this.buttonScripts.Add(button, buttonScript);
      }
    }

    public Player.ButtonScript? GetButtonScript(Buttons button)
    {
      Player.ButtonScript buttonScript;
      if (this.buttonScripts.TryGetValue(button, out buttonScript))
        return new Player.ButtonScript?(buttonScript);
      return new Player.ButtonScript?();
    }

    public override void OnCollision(Actor other, Vector3 displacement)
    {
      Player player = other as Player;
      if (player != null && !player.Gamer.IsLocal)
        displacement *= 0.2f;
      this.KnockForce = displacement;
    }

    protected override void UpdatePhysics()
    {
      Vector3 vector3 = new Vector3();
      vector3.X = this.Position.X;
      vector3.Y = this.Position.Y;
      vector3.Z = this.Position.Z;
      double x = (double) this.Velocity.X;
      double y = (double) this.Velocity.Y;
      double z = (double) this.Velocity.Z;
      base.UpdatePhysics();
      this.posDiff = new Vector3();
      this.posDiff.X = this.Position.X - vector3.X;
      this.posDiff.Y = this.Position.Y - vector3.Y;
      this.posDiff.Z = this.Position.Z - vector3.Z;
      this.AddMovementStats(this.posDiff);
      this.RebuildDepthString();
      this.AudioListener.Position = this.EyePosition;
      this.AudioListener.Forward = this.ViewDirection;
      this.AudioListener.Velocity = this.Velocity;
    }

    private void AddMovementStats(Vector3 posDiff)
    {
      if (this.FlyMode != FlyMode.None)
      {
        if (!this.IsAllowedToFly)
        {
          this.FlyMode = FlyMode.None;
          this.isFlightAscending = this.isFlightDescending = false;
        }
        else
          this.stats.DistanceFlown += posDiff.Length();
      }
      else
      {
        posDiff.Y = 0.0f;
        this.stats.DistanceWalked += posDiff.Length();
      }
    }

    private void ClearRubberBanding()
    {
      this.IsRubberBanding = false;
      this.timeRubberBanding = 0.0;
    }

    private void RebuildDepthString()
    {
      GlobalPoint3D point = this.map.GetPoint(new Vector3()
      {
        X = this.Position.X,
        Y = this.Position.Y + 0.05f,
        Z = this.Position.Z
      });
      if (!(point != this.rebuildDepthStringLastPoint))
        return;
      if (this.instance.IsDigDeepMode)
      {
        this.PosString = string.Format("({0},{1})", (object) point.X, (object) point.Z);
        int num = this.map.MapHeight - point.Y;
        this.DepthString.Value = num;
        if (num > Globals2.GameProperties.SaveGame.Header.DepthReached)
          Globals2.GameProperties.SaveGame.Header.DepthReached = num;
      }
      else
        this.PosString = string.Format("({0}, {1}, {2})", (object) point.X, (object) point.Y, (object) point.Z);
      this.rebuildDepthStringLastPoint = point;
    }

    private void UpdateViewDirection()
    {
      float rotateSpeed = this.RotateSpeed;
      float num1 = rotateSpeed * 0.2f;
      float num2 = this.viewInput.X * MathHelper.Lerp(num1, rotateSpeed, Math.Abs(this.viewInput.X));
      float num3 = this.viewInput.Y * MathHelper.Lerp(num1, rotateSpeed, Math.Abs(this.viewInput.Y));
      this.viewAngle.X -= num2;
      this.viewAngle.Y = MathHelper.Clamp(this.viewAngle.Y + num3, -1.560796f, 1.560796f);
      this.ViewDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.viewAngle.X, this.viewAngle.Y, 0.0f));
    }

    protected override void UpdateGeneral()
    {
      this.UpdateViewDirection();
      if ((double) this.cctvTimer > 0.0)
      {
        this.cctvTimer -= Services.ElapsedTime;
        if ((double) this.cctvTimer <= 0.0)
          this.EndCCTV();
      }
      if (!this.justClosedBinoculars)
      {
        if (this.LeftHand != null)
          this.LeftHand.UpdateSwing();
        if (this.RightHand != null)
          this.RightHand.UpdateSwing();
      }
      this.timeSinceLastEdit += Services.ElapsedTime;
      this.stats.SecondsPlayed += (double) Services.ElapsedTime;
      if (this.stats.SecondsPlayed > this.lastSecondsPlayed + 60.0)
      {
        this.lastSecondsPlayed = this.stats.SecondsPlayed;
        this.Raise_MinutePlayed();
      }
      this.bloodTintTimer -= Services.ElapsedTime;
      if ((double) this.bloodTintTimer < 0.0)
        this.bloodTintTimer = 0.0f;
      if (++this.calcSwingTargetDelay == 2)
      {
        this.CalcSwingTarget(this.reach);
        this.calcSwingTargetDelay = 0;
        if (this.CurrentArcadeMachine != null && this.SwingTarget != this.CurrentArcadeMachine.Point)
          this.instance.RemoveArcadeMachine(this.CurrentArcadeMachine.Point, UpdateBlockMethod.Player);
        HitTarget hitTarget = this.instance.BuildHitTarget(new Ray(this.EyePosition, this.ViewDirection), (Actor) this, HitTargetOptions.PlayersAndNpcs, (List<ActorType>) null);
        this.ActorInReticle = hitTarget.Target == null || (double) hitTarget.Distance >= (double) this.SwingTargetDistance || (double) hitTarget.Distance >= (double) this.reach ? (Actor) null : hitTarget.Target;
      }
      if ((double) this.reloadComponentTimer > 0.0 && this.reloadComponentChunk != null)
      {
        this.reloadComponentTimer -= Services.ElapsedTime;
        if ((double) this.reloadComponentTimer <= 0.0 && this.GetMapModelChunk(this.GetClipboardModel(this.Inventory.LeftHand)) == this.reloadComponentChunk)
        {
          this.reloadComponentChunk.LoadMesh(false, true);
          this.reloadComponentChunk = (MapChunkTM) null;
        }
      }
      if (this.fovInterpolator.IsActive)
      {
        double num = (double) this.fovInterpolator.Update();
        this.FOVNormalized = this.fovSaveState.FOVNormalized - this.fovInterpolator.CurrentValue * 0.2f;
      }
      this.UpdateBlueprintFinder();
      this.SetSplinterIndex();
      this.CheckDepthIfShopInInventory();
      this.SendInventoryIfChanged((Inventory) this.Inventory);
      this.SendStatisticsIfChanged();
      if (this.CustomSkyColor.IsActive)
        this.CustomSkyColor.Update();
      if (this.CustomTintColor.IsActive)
        this.CustomTintColor.Update();
      this.RainParticleSystem.Update();
      this.HailParticleSystem.Update();
      bool flag = this.SwingTarget != this.lastSwingTarget;
      if (flag)
        this.MiniMapRenderer.RenderTargetIsDirty();
      if (flag || (double) this.lastSwingTargetDistance != (double) this.SwingTargetDistance)
        this.CheckForScriptStaring();
      if (this.IsClipboardEquipped)
        this.UpdateClipboardModelWorldMatrix();
      if (this.rumble != null)
        this.rumble.Update();
      if (this.DialogHandler != null)
        this.DialogHandler.Update();
      this.timeRubberBanding += (double) Services.ElapsedTime;
      this.IsRubberBanding = this.timeRubberBanding > 240.0;
      if (!this.IsDying && (double) this.Health < (double) this.MaxHealth)
        this.Health += this.HealthReplenishmentRate;
      base.UpdateGeneral();
      foreach (Mod activePlugin in ModManager.ActivePlugins)
        activePlugin.Plugin.Update((ITMPlayer) this);
    }

    protected override void SetViewDirecton(Vector3 dir)
    {
      this.viewAngle.X = (float) -(Math.Atan2((double) dir.Z, (double) dir.X) + 1.57079637050629);
      this.viewAngle.Y = dir.Y * 1.570796f;
    }

    protected override void OnSwingTargetChanged()
    {
      this.SplinterProgress = 0.0f;
      this.Splinter = -1;
      if (!this.instance.IsCreativeMode)
        return;
      string str;
      if (!this.SwingTargetIsValid)
        str = "(- - -)";
      else
        str = string.Format("{0} ({1}, {2}, {3})", (object) ItemData2.ForDisplay(this.instance, (Item) this.map.GetBlockID(this.SwingTarget)), (object) this.SwingTarget.X, (object) this.SwingTarget.Y, (object) this.SwingTarget.Z);
      this.CursorString = str;
    }

    protected override void DepleteItemDurabilityForUsage()
    {
      base.DepleteItemDurabilityForUsage();
      if (!this.IsBinocularView || !this.IsItemEquippedAndUsable(Item.Binoculars) || this.DepleteItemDurabilityForUsage(this.GetItemEquippedSlot(Item.Binoculars)) != 0)
        return;
      this.EndBinocularView();
    }

    private void CheckForScriptStaring()
    {
      ScriptBlock scriptBlock = (ScriptBlock) null;
      if (this.SwingTargetIsValid)
        scriptBlock = this.instance.MapStrategyTM.GetDataBlock(this.SwingTarget) as ScriptBlock;
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) this,
        BlockOffset = new GlobalPoint3D?(this.SwingTarget)
      };
      if (this.lastScriptBlockActivatedBySight != null && (scriptBlock != this.lastScriptBlockActivatedBySight || (double) this.SwingTargetDistance > (double) this.lastScriptBlockActivatedBySight.PlayerLookRange))
      {
        this.instance.ExecuteScript(this.lastScriptBlockActivatedBySight.PowerOffScript, data, this.IsLocalGamer);
        this.lastScriptBlockActivatedBySight = (ScriptBlock) null;
      }
      if (scriptBlock == null || scriptBlock == this.lastScriptBlockActivatedBySight || ((double) scriptBlock.PlayerLookRange <= 0.0 || (double) this.SwingTargetDistance > (double) scriptBlock.PlayerLookRange))
        return;
      this.instance.ExecuteScript(scriptBlock.PowerOnScript, data, this.IsLocalGamer);
      this.lastScriptBlockActivatedBySight = scriptBlock;
    }

    protected override void ClampPositionToMapBound()
    {
      if (this.IsGod || this.instance.IsAvatarDesigner)
        return;
      base.ClampPositionToMapBound();
    }

    public bool IsRegionInViewRange(MapRegion region, int dist)
    {
      if (region == null)
        return false;
      GlobalPoint3D point = this.map.GetPoint(this.EyePosition);
      return region.Offset.X <= point.X + dist && region.Offset.X + this.map.RegionSize.X >= point.X - dist && (region.Offset.Y <= point.Y + dist && region.Offset.Y + this.map.RegionSize.Y >= point.Y - dist) && (region.Offset.Z <= point.Z + dist && region.Offset.Z + this.map.RegionSize.Z >= point.Z - dist);
    }

    protected override void UpdateDying()
    {
      this.deathRecoveryTimer += Services.ElapsedTime;
      if ((double) this.deathRecoveryTimer >= (double) this.deathRecoveryTime)
      {
        this.deathRecoveryTimer = this.deathRecoveryTime;
        CoreGlobals.Message.ShowMessage("You died", Vector2.Zero, 2f, 3f, Color.White, this.GetScreenMatrix(true));
        this.ChangeState(ActorState.Respawning);
        this.DefaultRespawn();
        this.Health = this.MaxHealth;
        this.Oxygen = this.MaxOxygen;
        this.instance.ExecuteEventScript(ScriptEvent.PlayerRespawn, new ScriptExecuteData()
        {
          Actor = (Actor) this
        });
        this.ClearInputCore();
      }
      else
        this.viewInput = this.GetLookAroundInput();
    }

    protected override void UpdateRespawning()
    {
      this.deathRecoveryTimer -= Services.ElapsedTime;
      if ((double) this.deathRecoveryTimer > 0.0)
        return;
      this.deathRecoveryTimer = 0.0f;
      this.ChangeState(ActorState.Alive);
    }

    public void UpdateFromNetworkData(
      float sizeY,
      ActorState state,
      bool jetPackActive,
      byte leftHandSwing,
      byte rightHandSwing,
      bool positionReset,
      Block FootSoundBlock,
      bool iceEffectActive,
      float health,
      long elapsedMillisecs)
    {
      this.prevNetData = this.netData;
      this.netData.IsUpdated = true;
      this.netData.SizeY = sizeY;
      this.netData.State = state;
      this.netData.IsFlying = jetPackActive;
      this.netData.LeftHandSwingCount = leftHandSwing;
      this.netData.RightHandSwingCount = rightHandSwing;
      this.netData.PositionReset = positionReset;
      this.netData.FootSoundBlock = FootSoundBlock;
      this.netData.IsIceEffectActive = iceEffectActive;
      this.netData.Health = health;
      this.netData.PrevElapsedMillisecs = this.netData.ElapsedMillisecs;
      this.netData.CurrentElapsedMillisecs = this.netData.ElapsedMillisecs;
      this.netData.ElapsedMillisecs = elapsedMillisecs;
    }

    public void UpdateFromNetworkData(Vector3 pos, Vector2 vd)
    {
      if ((double) this.currentTime == 0.0)
        this.currentTime = this.netData2.Time;
      this.prevNetData2.Position = this.Position;
      this.prevNetData2.ViewDirection = new Vector3(this.ViewDirection.X, 0.0f, this.ViewDirection.Z);
      this.prevNetData2.Time = this.currentTime;
      this.netData2.Position = pos;
      this.netData2.ViewDirection = new Vector3(vd.X, 0.0f, vd.Y);
      this.netData2.Time = Services.TotalTime;
    }

    public void UpdateRemote()
    {
      this.playPainSoundTimer -= Services.ElapsedTime;
      Vector3 position = this.Position;
      this.stats.SecondsPlayed += (double) Services.ElapsedTime;
      this.netData.CurrentElapsedMillisecs += (long) ((double) Services.ElapsedTime * 1000.0);
      this.SetSplinterIndex();
      this.LeftHand.ClearSwing();
      this.RightHand.ClearSwing();
      this.IsUnderWater = this.IsUnderLava = this.IsInWater = false;
      this.duckHead = 0.0f;
      if (this.netData.IsUpdated)
      {
        this.Size.Y = this.netData.SizeY;
        this.UpdateEyeOffsetFromSize();
        this.FlyMode = this.netData.IsFlying ? FlyMode.Slow : FlyMode.None;
        this.Health = this.netData.Health;
        this.LeftSwingCountNet += (int) this.netData.LeftHandSwingCount;
        this.RightSwingCountNet += (int) this.netData.RightHandSwingCount;
        ActorState actorState = this.actorState;
        this.ChangeState(this.netData.State);
        if (this.netData.State == ActorState.Dying && actorState != ActorState.Dying)
          this.Die(DamageType.Unknown, (Actor) null, Item.None, 0.0f);
        if (this.netData.PositionReset || float.IsNaN(this.Position.X) || (float.IsNaN(this.Position.Y) || float.IsNaN(this.Position.Z)))
          this.Position = this.netData.Position;
        this.RebuildDepthString();
        this.FreezeTimer = this.netData.IsIceEffectActive ? 1f : 0.0f;
        if (this.netData.FootSoundBlock != Block.None)
          Sounds.PlaySound((Item) this.netData.FootSoundBlock, ItemSoundType.Step, (ITMActor) this, false);
      }
      if ((double) this.currentTime > 0.0)
      {
        this.currentTime += Services.ElapsedTime;
        float num1 = this.netData2.Time - this.prevNetData2.Time;
        if ((double) this.currentTime > (double) this.netData2.Time)
          this.currentTime = this.netData2.Time;
        float num2 = this.currentTime - this.prevNetData2.Time;
        float amount = (double) num2 == (double) num1 ? 1f : num2 / num1;
        this.Position = Vector3.Lerp(this.prevNetData2.Position, this.netData2.Position, amount);
        this.ViewDirection = Vector3.Lerp(this.prevNetData2.ViewDirection, this.netData2.ViewDirection, amount);
      }
      if (!this.map.IsValidPoint(this.map.GetPoint(this.Position)))
        this.Position = position;
      if (++this.calcSwingTargetDelay == 4)
      {
        this.CalcSwingTarget(this.reach);
        this.calcSwingTargetDelay = 0;
      }
      if (this.LeftSwingCountNet > 0 && !this.LeftHand.IsSwinging)
      {
        this.LeftHand.SetIsSwinging(true);
        --this.LeftSwingCountNet;
      }
      if (this.RightSwingCountNet > 0 && !this.RightHand.IsSwinging)
      {
        this.RightHand.SetIsSwinging(true);
        --this.RightSwingCountNet;
      }
      this.LeftHand.UpdateSwing();
      this.RightHand.UpdateSwing();
      if (this.CustomSkyColor.IsActive)
        this.CustomSkyColor.Update();
      if (this.CustomTintColor.IsActive)
        this.CustomTintColor.Update();
      this.UpdateMatrices();
      switch ((Block) this.map.GetBlockID(this.EyePosition))
      {
        case Block.Water:
          this.IsUnderWater = this.IsUnderLiquid(Block.Water);
          break;
        case Block.Lava:
          this.IsUnderLava = this.IsUnderLiquid(Block.Lava);
          break;
      }
      this.netData.IsUpdated = false;
    }

    public void OnRemoteInventoryUpdated()
    {
      if (!this.instance.IsHost)
        return;
      MapSaver.BuildPlayerData(this.instance, this);
    }

    private void SendInventoryIfChanged(Inventory inventory)
    {
      if (!this.IsLocalGamer)
        return;
      if (inventory.HasItemsChanged && !inventory.SuspendItemsChangedTransmission)
      {
        this.instance.NetworkManager.SendInventoryChanged(this, inventory);
        inventory.ItemsChanged.Clear();
        inventory.HasItemsChanged = false;
      }
      if (this.instance.NetworkManager.IsHost)
        return;
      this.sendInventoryTimer += Services.ElapsedTime;
      if ((double) this.sendInventoryTimer <= 30.0)
        return;
      this.sendInventoryTimer = 0.0f;
      this.instance.NetworkManager.SendInventory(this);
    }

    private bool PlayerHasInventoryBlockOpen
    {
      get
      {
        return this.instance.HasBlockOpen(this.GamerID);
      }
    }

    private void CheckDepthIfShopInInventory()
    {
      if (this.isBot || !this.instance.IsDigDeepMode || ((double) this.Position.Y >= (double) (this.map.MapBound.Max.Y - 200) || !this.IsInputEnabled) || this.IsGodOrTester)
        return;
      this.shopBelow100WarningTimer -= Services.ElapsedTime;
      if ((double) this.shopBelow100WarningTimer >= 0.0 || !this.Inventory.HasItem(Item.BlockShop) && !this.Inventory.HasItem(Item.ItemShop))
        return;
      this.instance.AddScreen((GameScreen) new MessageBoxScreenTM("You have shops in your inventory.\nYou cannot take them below 200 depth.\nEither place them down or drop them.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this), this);
      this.shopBelow100WarningTimer = 7f;
    }

    protected override float TakeDamageLocal(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      Actor attacker,
      Item weaponID)
    {
      float damageLocal = base.TakeDamageLocal(damageType, damage, knockForce, attacker, weaponID);
      if ((double) damageLocal > 0.0)
      {
        this.stats.DamageTaken += damageLocal;
        this.bloodTintTimer = (float) ((double) damageLocal / (double) this.MaxHealth * 2.0);
        if (this.rumble != null)
          this.rumble.StartRumble(RumbleType.Damage, damageLocal);
      }
      return damageLocal;
    }

    public void OnSuccessfulBlockClear(ref BlockClearedEventArgs e)
    {
      Block blockId = (Block) e.BlockData.BlockID;
      switch (blockId)
      {
        case Block.Water:
          break;
        case Block.Lava:
          break;
        default:
          if (this.map.IsHost && this.instance.ShouldAddPickUp(this, blockId))
            this.instance.AddPickup(ref e);
          if (blockId == Block.Wisdom || blockId == Block.Blueprint || (blockId == Block.Book || blockId == Block.Key))
            break;
          this.instance.AddMiningParticles(e.Point, (Block) e.BlockData.BlockID, e.BlockData.AuxData);
          if (this.rumble == null || !this.Gamer.IsLocal)
            break;
          this.rumble.StartRumble(RumbleType.ClearBlock);
          break;
      }
    }

    protected override void OnLeftHandItemDegraded()
    {
      this.LeftHand.OnItemDegraded();
    }

    protected override void OnRightHandItemDegraded()
    {
      this.RightHand.OnItemDegraded();
    }

    public override void OnBlastCreated(QueuedBlast blast, Player detonator, Item itemID)
    {
      base.OnBlastCreated(blast, detonator, itemID);
      if (this.rumble == null)
        return;
      float num = GlobalPoint3D.Distance(blast.Point, this.map.GetPoint(this.EyePosition));
      if ((double) num >= (double) blast.Strength)
        return;
      this.rumble.StartRumble(RumbleType.Explosion, MathHelper.Lerp(0.0f, 1f, num / blast.Strength));
    }

    public Blueprint BlueprintFinderTarget
    {
      get
      {
        return this.instance.ClosestBlueprints[this.ScreenID];
      }
    }

    private void UpdateBlueprintFinder()
    {
      if (!this.Settings.BlueprintFinderVisible)
        return;
      Blueprint blueprintFinderTarget = this.BlueprintFinderTarget;
      if (blueprintFinderTarget == null || !blueprintFinderTarget.IsGenerated || blueprintFinderTarget.Point.Y <= 0)
        return;
      Vector3 vector3 = this.map.GetBlockCenter(blueprintFinderTarget.Point) - (this.Position + this.EyeOffset * 0.5f);
      float num1 = MathHelper.Clamp(vector3.Length(), 1f, 200f);
      double num2 = (double) MathHelper.Lerp(0.65f, 0.05f, num1 / 200f);
      this.blueprintFinderRoll += MathHelper.Lerp(0.2f, 1f / 1000f, num1 / 200f);
      vector3.Normalize();
      this.BlueprintFinderWorld = Matrix.CreateRotationY(this.blueprintFinderRoll) * Matrix.CreateRotationX(1.570796f) * Matrix.Invert(Matrix.CreateLookAt(Vector3.Zero, -vector3, Vector3.Up)) * Matrix.CreateTranslation(this.EyePosition + this.ViewDirection * 5f);
    }

    public void RegisterDamageDealt(
      DamageType damageType,
      float damage,
      Actor target,
      Item weaponID,
      bool killed)
    {
      switch (damageType)
      {
        case DamageType.Combat:
          if (!this.instance.IsFiniteResources)
            break;
          if (damageType == DamageType.Combat)
            this.SkillsData.StrikeCharacter((Actor) this, weaponID, damage * (target.IsCustomMob ? 0.5f : 1f));
          if (this.IsRubberBanding)
            break;
          this.UpdateDamageStats(damageType, damage, target, weaponID, killed);
          if (!killed || !target.IsLocalGamer)
            break;
          this.Raise_KillCharacter(target, (Actor) null, weaponID);
          break;
        case DamageType.Blast:
          if (weaponID != Item.Grenade && weaponID != Item.BoomArrow)
            break;
          goto case DamageType.Combat;
      }
    }

    private void UpdateDamageStats(
      DamageType damageType,
      float damage,
      Actor target,
      Item weaponID,
      bool killed)
    {
      this.stats.DamageDealt += damage;
      if (!killed)
        return;
      ++this.stats.TotalKills;
      if (target.Gamer != null)
      {
        ++this.stats.PlayerKills;
        ++this.killStreak;
        if (this.killStreak == 4)
          this.instance.AddNotification(this, " achieved a 4 x killing streak", NotifyRecipient.Local);
        else if (this.killStreak == 10)
        {
          this.instance.AddNotification(this, " achieved a 10 x killing streak!", NotifyRecipient.Local);
        }
        else
        {
          if (this.killStreak != 50)
            return;
          this.instance.AddNotification(this, " achieved a 50 x killing streak!!!", NotifyRecipient.Local);
        }
      }
      else
        ++this.stats.NPCKills;
    }

    protected override void CollateBlockTargetResult(Ray ray, BoundingBox box, HitTest result)
    {
      base.CollateBlockTargetResult(ray, box, result);
      this.Raise_BlockTargeted(this.SwingTarget, this.map.GetBlockData(result.Point));
    }

    protected override bool PlayerStruckPlayer(Player attacker)
    {
      if (attacker == null)
        return false;
      if (Globals2.GameProperties.SaveGame.Header.ClanProtection && this.ClanName != null && (this.ClanName.Length > 0 && this.ClanName == attacker.ClanName))
      {
        CoreGlobals.Message.ShowMessage("Cannot damage fellow clan members", new Vector2(0.0f, -1f), 2f, 2.5f, Color.Red, attacker.GetScreenMatrix(true));
        return false;
      }
      if (this.instance.IsSkillsEnabled)
      {
        short num = Math.Min(this.CurrentZoneCombatLevelDifference, attacker.CurrentZoneCombatLevelDifference);
        if (num > (short) 0 && Math.Abs(this.CombatLevel - attacker.CombatLevel) > (int) num)
        {
          if (attacker.IsLocalGamer)
            CoreGlobals.Message.ShowMessage("Combat Level Difference Exceeded", new Vector2(0.0f, -1f), 2f, 2.5f, Color.Red, attacker.GetScreenMatrix(true));
          return false;
        }
      }
      if (!this.instance.IsInZoneType(this.Box, ZoneType.NoCombat, this.GamerID) && !this.instance.IsInZoneType(attacker.Box, ZoneType.NoCombat, attacker.GamerID))
        return true;
      if (attacker.IsLocalGamer)
        CoreGlobals.Message.ShowMessage("Non PvP Zone", new Vector2(0.0f, -1f), 2f, 2.5f, Color.Red, attacker.GetScreenMatrix(true));
      return false;
    }

    private void ProspectBlock()
    {
      if (!this.SwingTargetIsValid)
        return;
      Block blockId = (Block) this.map.GetBlockID(this.SwingTarget);
      if (blockId <= Block.None || blockId >= Block.zLastBlockID)
        return;
      Item pickupItemId = this.instance.ConvertBlockIDToPickupItemID(blockId, this.map.GetAuxFullData(this.SwingTarget), true);
      if (this.instance.IsCreativeMode || pickupItemId != Item.NPCSpawn)
        this.instance.UnlockItem(this, pickupItemId, true);
      if (pickupItemId == Item.Bedrock && this.SwingTarget.Y == 0)
      {
        this.instance.UnlockItem(this, Item.SledgeHammer, true);
        this.instance.UnlockItem(this, Item.GreenstoneGoldSledgeHammer, true);
        Globals2.GameProperties.SaveGame.Header.DepthReached = this.map.MapHeight;
      }
      this.instance.AddScreen((GameScreen) new InteractItemScreen(this.instance, this, new InventoryItem(pickupItemId), new GlobalPoint3D?(this.SwingTarget)), this);
      this.Raise_ItemProspect(pickupItemId);
    }

    public void TakePhoto(Hand hand)
    {
      if (this.IsTakingPhoto || this.IsAssemblingPhoto)
        return;
      if (Globals2.GetPhotoCount() >= 256)
      {
        this.instance.AddScreen((GameScreen) new MessageBoxScreenTM("You have reached the photo capacity.\n\nIn the next release you will be able to delete old photos\nto make room for new ones.", (string) null, (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this), this);
      }
      else
      {
        this.IsTakingPhoto = true;
        this.IsAssemblingPhoto = true;
        Sounds.PlaySound(Item.Camera, ItemSoundType.Use);
        this.OnItemUsed(hand);
      }
    }

    public void StartBinocularView()
    {
      this.binocularsView = true;
      this.fovSaveState.FOVNormalized = this.FOVNormalized;
      this.Settings.AddHideHUD();
      this.leftTriggerReleased = (double) this.pad.Triggers.Left > 0.0 ? 0 : 1;
      this.rightTriggerReleased = (double) this.pad.Triggers.Right > 0.0 ? 0 : 1;
      this.fovInterpolator.Start(0.0f, 1f, 1.0);
    }

    private void EndBinocularView()
    {
      this.binocularsView = false;
      this.fovInterpolator.IsActive = false;
      this.FOVNormalized = this.fovSaveState.FOVNormalized;
      this.Settings.RemoveHideHUD();
    }

    private bool IsCurrentArcadeMachine(GlobalPoint3D point, BlockFace face)
    {
      if (this.CurrentArcadeMachine != null && this.CurrentArcadeMachine.Point == point)
        return this.CurrentArcadeMachine.Face == face;
      return false;
    }

    private void SetSplinterIndex()
    {
      if ((double) this.SplinterProgress > 0.0 && this.HasPermission(Permissions.Edit) && (this.LeftHand.IsSwinging && ItemData.CanItemBreakBlocks(this.LeftHand.ItemID) || this.RightHand.IsSwinging && ItemData.CanItemBreakBlocks(this.RightHand.ItemID)) && (this.map.GetClearBlockResult(this.SwingTarget, UpdateBlockMethod.Player, this.GamerID) == ClearBlockResult.Success && this.IsSplinterable(this.map.GetBlockID(this.SwingTarget))))
      {
        this.Splinter = (int) ((double) Math.Min(this.SplinterProgress, 1f) * (double) (GraphicStatics.SplinterCount - 1));
      }
      else
      {
        this.SplinterProgress = 0.0f;
        this.Splinter = -1;
      }
    }

    private bool IsSplinterable(byte blockID)
    {
      return !this.map.IsBlockIcon(blockID);
    }

    protected override bool SkipCollision
    {
      get
      {
        return this.isNoClipMode;
      }
    }

    protected override float SpeedModifier
    {
      get
      {
        float speedModifier = base.SpeedModifier;
        if (this.FlyMode == FlyMode.None && this.IsItemEquippedAndUsable(Item.TenLeagueBoots))
          speedModifier *= 1.5f;
        if (this.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
          speedModifier *= 0.25f;
        return speedModifier;
      }
    }

    protected override bool GetHaltOnLadder()
    {
      if (this.FlyMode == FlyMode.None)
        return this.IsCrouchButtonPressed();
      return true;
    }

    protected override bool GetJump()
    {
      if (!this.IsInputEnabled)
        return false;
      if (!this.IsCrouching)
      {
        if (this.IsBot)
          return this.instance.Random.Next(120) == 0;
        if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.Jump))
        {
          if (this.IsFloatingInWater)
            return true;
          if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.Jump))
          {
            if (this.IsOnGround || this.isOnRope)
            {
              this.jumpCounter = 0;
              return true;
            }
            if (this.jumpCounter == 0 && (double) this.Velocity.Y > 0.0)
            {
              this.jumpCounter = 1;
              return true;
            }
          }
        }
      }
      else if (InputManager1.IsInputPressedNew(this.PlayerIndex, PlayerInput.Jump))
        this.isHeldCrouch = false;
      return false;
    }

    private float RandFloat
    {
      get
      {
        return (float) (this.instance.Random.NextDouble() * 2.0 - 1.0);
      }
    }

    protected override Vector2 GetMovementInput()
    {
      if (!this.IsInputEnabled)
        return Vector2.Zero;
      if (this.IsBot)
      {
        if (--this.botLeftStickCounter < 0)
        {
          this.botLeftStick = new Vector2(this.RandFloat, this.RandFloat);
          if (this.instance.Random.Next(5) == 0)
            this.botLeftStick = Vector2.Zero;
          this.botLeftStickCounter = this.instance.Random.Next(400);
        }
        return this.botLeftStick;
      }
      Vector2 gamepadLeftStick = InputManager.GetGamepadLeftStick(this.PlayerIndex);
      if ((double) gamepadLeftStick.X == 0.0)
      {
        bool flag1 = InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.MoveLeft);
        bool flag2 = InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.MoveRight);
        gamepadLeftStick.X = !flag1 || !flag2 ? (flag1 ? -1f : (flag2 ? 1f : 0.0f)) : 0.0f;
      }
      if ((double) gamepadLeftStick.Y == 0.0)
      {
        if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.MoveForward))
          gamepadLeftStick.Y = 1f;
        else if (InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.MoveBackward))
          gamepadLeftStick.Y = -1f;
      }
      if ((double) gamepadLeftStick.Length() > (double) this.stickThreshold)
      {
        this.Raise_LeftStickPressed();
        if (this.CheckVector2Movement(gamepadLeftStick, this.lastLeftStick, 0.1f))
          this.ClearRubberBanding();
        return this.lastLeftStick = gamepadLeftStick;
      }
      this.Raise_LeftStickReleased();
      return Vector2.Zero;
    }

    private bool CheckVector2Movement(Vector2 v1, Vector2 v2, float diff)
    {
      if ((double) Math.Abs(v1.X - v2.X) <= (double) diff)
        return (double) Math.Abs(v1.Y - v2.Y) > (double) diff;
      return true;
    }

    protected override Vector2 GetLookAroundInput()
    {
      if (this.actorState == ActorState.Dying)
        return new Vector2(1f, 0.2f);
      if (!this.IsInputEnabled || InputManager1.IsInputPressed(this.PlayerIndex, PlayerInput.Special))
        return Vector2.Zero;
      if (this.IsBot)
      {
        if (--this.botRightStickCounter < 0)
        {
          this.botRightStick = new Vector2(this.RandFloat * 0.1f, this.RandFloat * 0.1f);
          if (this.instance.Random.Next(5) == 0)
            this.botRightStick = Vector2.Zero;
          this.botRightStickCounter = this.instance.Random.Next(400);
        }
        return this.botRightStick;
      }
      Vector2 posDeltaSmoothed = InputManager.GetMousePosDeltaSmoothed(this.PlayerIndex);
      bool flag = this.IsCCTVView && this.virtualPlayer.AITarget == null;
      if ((double) posDeltaSmoothed.X != 0.0 || (double) posDeltaSmoothed.Y != 0.0)
      {
        if (!InputManager1.Profile.GamePadInvertY)
          posDeltaSmoothed.Y = -posDeltaSmoothed.Y;
        float num = flag ? InputManager1.Profile.MouseSensitivity : (float) (((double) InputManager1.Profile.MouseSensitivity * 0.949999988079071 + 0.0500000007450581) * 0.100000001490116);
        if (this.binocularsView || flag)
          num *= this.fov / this.FOVRange.Y * MathHelper.Lerp(1.5f, 1f, this.fov / this.FOVRange.Y);
        float max = MathHelper.Clamp(num * 30f, 0.0f, 2f);
        posDeltaSmoothed.X = MathHelper.Clamp(posDeltaSmoothed.X * num, -max, max);
        posDeltaSmoothed.Y = MathHelper.Clamp(posDeltaSmoothed.Y * num, -max, max);
        return posDeltaSmoothed;
      }
      Vector2 gamepadRightStick = InputManager.GetGamepadRightStick(this.PlayerIndex);
      if (this.CheckVector2Movement(gamepadRightStick, this.lastRightStick, 0.1f))
        this.ClearRubberBanding();
      this.lastRightStick = gamepadRightStick;
      if ((double) gamepadRightStick.Length() > (double) this.stickThreshold)
        this.Raise_RightStickPressed();
      else
        this.Raise_RightStickReleased();
      if (InputManager1.Profile.GamePadInvertY)
        gamepadRightStick.Y = -gamepadRightStick.Y;
      float num1 = flag ? InputManager1.Profile.GamePadSensitivity : (float) ((double) InputManager1.Profile.GamePadSensitivity * 0.850000023841858 + 0.349999994039536);
      if (this.binocularsView || flag)
        num1 *= this.fov / this.FOVRange.Y;
      return gamepadRightStick * num1;
    }

    protected override float LandingDamageMultiplier
    {
      get
      {
        if (this.FlyMode == FlyMode.None)
          return base.LandingDamageMultiplier;
        return 0.0f;
      }
    }

    public void OnTexturePackChanged()
    {
      this.LeftHand.OnTexturePackChanged();
      this.RightHand.OnTexturePackChanged();
    }

    protected override float ModifyCollisionDamage(float damage, Vector3 dir, Block blockID)
    {
      damage = base.ModifyCollisionDamage(damage, dir, blockID);
      if ((double) dir.Y != 0.0)
      {
        if (this.IsItemEquippedAndUsable(Item.TenLeagueBoots))
          damage *= 0.5f;
        else if (this.IsItemEquippedAndUsable(Item.SpiderRing))
          damage *= 0.25f;
      }
      return damage;
    }

    protected override void ClearBlockCore(Hand hand, Item tool)
    {
      this.timeSinceLastEdit = 0.0f;
      MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(this.SwingTarget);
      if (ItemData.IsSubType(tool, ItemSubType.TillTool) && BlockData.IsTillable((Block) blockIdAndAux.BlockID, tool))
      {
        this.map.SetBlockData(this.SwingTarget, (byte) 173, (byte) 0, UpdateBlockMethod.Player, this.GamerID, true);
        this.instance.RaiseEventBlockMined((Block) blockIdAndAux.BlockID, blockIdAndAux.AuxData, this.SwingTarget, hand);
        this.ChangeLog.LogSetBlock(this.instance, this, this.SwingTarget, Item.TilledEarth, (byte) 0);
        this.OnItemUsed(hand);
        this.SkillsData.BlockMined((Actor) this, tool, blockIdAndAux);
        --this.SplinterProgress;
        this.Splinter = -1;
        this.map.Commit();
      }
      else
      {
        if (!this.instance.ClearBlock(this.SwingTarget, UpdateBlockMethod.Player, this.Gamer.ID, true))
          return;
        this.map.Commit();
        this.instance.RaiseEventBlockMined((Block) blockIdAndAux.BlockID, blockIdAndAux.AuxData, this.SwingTarget, hand);
        this.SkillsData.BlockMined((Actor) this, tool, blockIdAndAux);
        this.OnItemUsed(hand, ItemUseType.ClearBlock);
        ++this.stats.BlocksCleared;
        this.Raise_BlockCleared(this.SwingTarget, blockIdAndAux, tool);
        this.ChangeLog.LogSetBlock(this.instance, this, this.SwingTarget, Item.None, (byte) 0);
        this.ActionLog.AddAction((Block) blockIdAndAux.BlockID);
      }
    }

    public void ResetPropertiesAfterSkillToggle(bool oldSkillsEnabled)
    {
      if (oldSkillsEnabled == this.instance.IsSkillsEnabled)
        return;
      this.Health = this.Health / SkillData.MaxHealth(oldSkillsEnabled ? this.SkillsData.Health.Level : Globals1.NpcLevelData[1].HealthLevel) * this.MaxHealth;
    }

    private void AutoEquipLeftHandBasedOnRightHandItem()
    {
    }

    public int GoldCoinsOnPerson
    {
      get
      {
        return this.Inventory.ItemCount(Item.GoldPieces);
      }
    }

    protected override void AddToInventoryCore(InventoryItem item, int slotID)
    {
      this.instance.UnlockItem(this, item.ItemID, false);
      if (slotID == this.Inventory.LeftHandIndex && this.LeftHand.ItemID == item.ItemID || slotID == this.Inventory.RightHandIndex && this.RightHand.ItemID == item.ItemID)
        return;
      this.GetDominantHand(slotID)?.SetItem(item.ItemID);
    }

    private Hand GetDominantHand(int slotID)
    {
      if (this.Inventory.HotBarRightSlotID == slotID && (this.Inventory.HotBarLeftSlotID != slotID || this.RightHand.ItemID != Item.Hand || this.LeftHand.ItemID == Item.Hand))
        return this.RightHand;
      if (this.Inventory.HotBarLeftSlotID == slotID && (this.Inventory.HotBarRightSlotID != slotID || this.LeftHand.ItemID != Item.Hand || this.RightHand.ItemID == Item.Hand))
        return this.LeftHand;
      return (Hand) null;
    }

    public bool HasItem(Item item)
    {
      return this.Inventory.HasItem(item);
    }

    private bool IsSingleWieldHand(Hand hand)
    {
      switch (this.Settings.WieldType)
      {
        case WieldType.LeftHand:
          return hand.HandType == InventoryHand.Left;
        case WieldType.RightHand:
          return hand.HandType == InventoryHand.Right;
        default:
          return true;
      }
    }

    public bool IsSingleWieldHandsBoundByItem(Item itemID1, Item itemID2)
    {
      if (this.Settings.WieldType == WieldType.BothHands)
        return false;
      if (ItemData.IsSubType(itemID1, ItemSubType.Bow))
        return !ItemData.IsSubType(itemID2, ItemSubType.Arrow);
      if (ItemData.IsSubType(itemID1, ItemSubType.GrenadeLauncher))
        return !ItemData.IsSubType(itemID2, ItemSubType.Grenade);
      if (ItemData.IsBindableWeapon(itemID1))
        return !ItemData.IsSubType(itemID2, ItemSubType.Shield);
      return true;
    }

    public override bool EquipFromInventory(Hand hand, int inventoryIndex, int upperBound)
    {
      if (inventoryIndex >= 0 && inventoryIndex < upperBound)
      {
        EquipIndex itemEquipIndex = ItemData.GetItemEquipIndex(this.Inventory[inventoryIndex].ItemID);
        if (hand == null && itemEquipIndex != EquipIndex.LeftHand && (itemEquipIndex != EquipIndex.RightHand && itemEquipIndex != EquipIndex.None))
          return this.EquipFromInventory(inventoryIndex, (int) ((byte) this.Inventory.EquipIndexStart + itemEquipIndex - (byte) 1));
        if (hand == null)
          hand = this.GetEquipHand(itemEquipIndex);
        if (hand != null)
        {
          if ((this.Settings.WieldType == WieldType.LeftHand && hand.HandType == InventoryHand.Right || this.Settings.WieldType == WieldType.RightHand && hand.HandType == InventoryHand.Left) && this.IsSingleWieldHandsBound)
            hand = hand.OtherHand;
          if (inventoryIndex != hand.HandIndex)
          {
            if (inventoryIndex < 10)
            {
              if (hand.HandType == InventoryHand.Left)
              {
                this.Inventory.HotBarLeftSlotID = -1;
                this.SetLeftHotBarSlot(inventoryIndex);
              }
              else
              {
                this.Inventory.HotBarRightSlotID = -1;
                this.SetRightHotBarSlot(inventoryIndex);
              }
            }
            else
            {
              this.leftHotbarCursorHasVisualPriority = hand.HandType == InventoryHand.Left;
              this.Inventory.SwapItem(inventoryIndex, hand.HandIndex);
            }
            return true;
          }
        }
      }
      return false;
    }

    public InventoryItem craftItem1
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart];
      }
    }

    public InventoryItem craftItem2
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 1];
      }
    }

    public InventoryItem craftItem3
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 2];
      }
    }

    public InventoryItem craftItem4
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 3];
      }
    }

    public InventoryItem craftItem5
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 4];
      }
    }

    public InventoryItem craftItem6
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 5];
      }
    }

    public InventoryItem craftItem7
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 6];
      }
    }

    public InventoryItem craftItem8
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 7];
      }
    }

    public InventoryItem craftItem9
    {
      get
      {
        return this.Inventory[(int) this.Inventory.TempIndexStart + 8];
      }
    }

    public Blueprint GetCraftBlueprint()
    {
      return Blueprints.GetResult(BlueprintCraftType.Crafting, this, this.craftItem1, this.craftItem2, this.craftItem3, this.craftItem4, this.craftItem5, this.craftItem6, this.craftItem7, this.craftItem8, this.craftItem9);
    }

    public InventoryItem GetCraftResult()
    {
      Blueprint craftBlueprint = this.GetCraftBlueprint();
      if (craftBlueprint == null)
        return InventoryItem.Empty;
      return craftBlueprint.Result;
    }

    protected override void OnBlockPlaced(Item itemRawID, Block blockID, byte auxData, Hand hand)
    {
      this.instance.RaiseEventBlockPlaced(blockID, this.SwingTarget, hand);
      this.ChangeLog.LogSetBlock(this.instance, this, this.PlaceTarget, itemRawID, auxData);
      Sounds.PlaySound((Item) this.map.GetBlockTextureIDForDrawing(blockID, this.PlaceTarget), ItemSoundType.Use, this.PlaceTarget, (ITMActor) this);
      this.timeSinceLastEdit = 0.0f;
      ++this.stats.BlocksPlaced;
      this.Raise_BlockPlaced(this.PlaceTarget, this.map.GetBlockData(this.PlaceTarget), itemRawID);
      this.ActionLog.AddAction((Item) blockID, ItemAction.Used);
    }

    public void AbortFloods()
    {
      if (this.Gamer == null)
        return;
      this.HasAbortedFloods = true;
      this.AbortFloodWorkItems();
      this.instance.NetworkManager.SendFloodAbort(this.GamerID);
    }

    private void AbortFloodWorkItems()
    {
      foreach (IThreadWorkItem priorityWorkItem in ThreadQueueManager.Instance.PriorityWorkItems)
      {
        FloodFill floodFill = priorityWorkItem as FloodFill;
        if (floodFill != null && floodFill.PlayerID == this.GamerID)
          ThreadQueueManager.Instance.CancelQueueItem(priorityWorkItem, PriorityLevel.Priority);
      }
      foreach (IThreadWorkItem mainWorkItem in ThreadQueueManager.Instance.MainWorkItems)
      {
        FloodFill floodFill = mainWorkItem as FloodFill;
        if (floodFill != null && floodFill.PlayerID == this.GamerID)
          ThreadQueueManager.Instance.CancelQueueItem(mainWorkItem, PriorityLevel.Normal);
      }
    }

    public override bool PickupItemCore(InventoryItem item, int particleID)
    {
      if (item.Count > 0)
      {
        this.instance.ProcessPickup(item, this);
        Item itemId = item.ItemID;
        if (itemId != Item.GoldPieces)
        {
          if (itemId > Item.zLastBlockID)
            this.stats.ItemsPickedUp += item.Count;
          else
            this.stats.BlocksPickedUp += item.Count;
        }
        if (this.AddToInventory(item) > 0)
        {
          this.AddLootValueAndXPFromPickup(item, particleID);
          this.Raise_ItemPickup(itemId);
          this.ActionLog.AddAction(itemId, ItemAction.Collected);
          Sounds.PlaySound(ItemSoundGroup.GenPickup, ItemSoundType.Use);
          return true;
        }
      }
      return true;
    }

    private void AddLootValueAndXPFromPickup(InventoryItem item, int particleID)
    {
      ItemParticle? particleFromId = this.instance.ParticleManager.GetParticleFromID(particleID);
      if (!particleFromId.HasValue || !particleFromId.Value.HasType(ParticleType.Loot))
        return;
      this.SkillsData.LootGained(this, item);
      this.stats.LootValue += item.PurchaseValue;
    }

    public bool HitSpecialBlock(Hand hand, Block blockID)
    {
      if (!ItemData.IsSubType((Item) blockID, ItemSubType.BlockCanBeOpened))
        return false;
      Block block = blockID;
      if ((uint) block <= 65U)
      {
        switch (block)
        {
          case Block.Bookcase:
          case Block.Workbench:
          case Block.Furnace:
          case Block.Chest:
          case Block.ItemShop:
          case Block.BlockShop:
            break;
          default:
            goto label_20;
        }
      }
      else
      {
        switch (block)
        {
          case Block.Book:
            if (!hand.AutoTrigger)
              this.instance.ReadBook(this, this.SwingTarget);
            return true;
          case Block.LockedChest:
          case Block.LitFurnace:
          case Block.Crate:
          case Block.Safe:
            break;
          case Block.BedHead:
          case Block.BedFoot:
            if (!hand.AutoTrigger)
              this.SleepOption();
            return true;
          case Block.Switch:
            if (!hand.AutoTrigger)
              this.HitSwitch();
            return true;
          case Block.Button:
            if (!hand.AutoTrigger)
              this.PressButton();
            return true;
          default:
            goto label_20;
        }
      }
      if (!hand.AutoTrigger)
        this.OpenSpecialBlock(hand, blockID);
      return true;
label_20:
      if (ItemData.IsSubType((Item) blockID, ItemSubType.Door))
      {
        if (!hand.AutoTrigger)
        {
          this.timeSinceLastEdit = 0.0f;
          this.instance.HitDoor(this.SwingTarget, this, hand);
          this.ChangeLog.LogHitDoor(this.instance, this, this.SwingTarget);
        }
        return true;
      }
      if (blockID != Block.ArcadeMachine)
        return false;
      if (!hand.AutoTrigger)
        this.instance.HitArcadeMachine(this, this.SwingTarget, this.SwingFace, hand.ItemID);
      return true;
    }

    private void OpenSpecialBlock(Hand hand, Block blockID)
    {
      if (this.isBot)
        return;
      this.instance.OpenSpecialBlock(this, this.SwingTarget, blockID, hand);
    }

    public bool HasActionRequest(GlobalPoint3D p, Block blockID)
    {
      return this.GetActionRequestIndex(p, blockID) >= 0;
    }

    public bool HasActionRequest(string messageStartingWith)
    {
      for (int index = this.ActionRequests.Count - 1; index >= 0; --index)
      {
        if (this.ActionRequests[index].Message.StartsWith(messageStartingWith, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public int AddActionRequest(string message)
    {
      return this.AddActionRequest(message, Color.Cyan);
    }

    public int AddActionRequest(string message, Color color)
    {
      return this.AddActionRequest(message, color, 1.0);
    }

    public int AddActionRequest(string message, Color color, double secondsHidden)
    {
      return this.AddActionRequest(new Player.ActionRequest(message)
      {
        Seconds = Globals1.ElapsedWatch.Elapsed.TotalSeconds,
        SecondsHidden = secondsHidden,
        Color = color,
        Message = message
      });
    }

    public int AddActionRequest(GlobalPoint3D p, Block blockID)
    {
      string message = "Opening " + ItemData2.ForDisplay(this.GameInstance, (Item) blockID) + ": Waiting for Host";
      return this.AddActionRequest(new Player.ActionRequest(message)
      {
        Point = p,
        BlockID = blockID,
        Seconds = Globals1.ElapsedWatch.Elapsed.TotalSeconds,
        Color = Color.Cyan,
        Message = message
      });
    }

    private int AddActionRequest(Player.ActionRequest request)
    {
      lock (this.ActionRequests)
      {
        this.ActionRequests.Add(request);
        return this.ActionRequests.Count - 1;
      }
    }

    public void CloseActionRequest(int index)
    {
      lock (this.ActionRequests)
      {
        if (index < 0 || index >= this.ActionRequests.Count)
          return;
        this.ActionRequests.RemoveAt(index);
      }
    }

    public void CloseActionRequest(Player.ActionRequest request)
    {
      lock (this.ActionRequests)
        this.ActionRequests.Remove(request);
    }

    public void CloseActionRequest(GlobalPoint3D p, Block blockID)
    {
      lock (this.ActionRequests)
      {
        int actionRequestIndex = this.GetActionRequestIndex(p, blockID);
        if (actionRequestIndex < 0)
          return;
        this.ActionRequests.RemoveAt(actionRequestIndex);
      }
    }

    public void CloseActionRequests(string messageStartingWith)
    {
      lock (this.ActionRequests)
      {
        for (int index = this.ActionRequests.Count - 1; index >= 0; --index)
        {
          if (this.ActionRequests[index].Message.StartsWith(messageStartingWith, StringComparison.OrdinalIgnoreCase))
            this.ActionRequests.RemoveAt(index);
        }
      }
    }

    public Player.ActionRequest GetActionRequest(int index)
    {
      lock (this.ActionRequests)
        return index < 0 || index >= this.ActionRequests.Count ? (Player.ActionRequest) null : this.ActionRequests[index];
    }

    private int GetActionRequestIndex(GlobalPoint3D p, Block blockID)
    {
      for (int index = this.ActionRequests.Count - 1; index >= 0; --index)
      {
        Player.ActionRequest actionRequest = this.ActionRequests[index];
        if (actionRequest.Point == p && actionRequest.BlockID == blockID)
          return index;
      }
      return -1;
    }

    private void HitSwitch()
    {
      if (this.isBot || !this.instance.HitSwitch(this.SwingTarget, UpdateBlockMethod.Player, this, true))
        return;
      this.ChangeLog.LogHitSwitch(this.instance, this, this.SwingTarget);
    }

    private void PressButton()
    {
      if (this.isBot || !this.instance.HitButton(this.SwingTarget, UpdateBlockMethod.Player, this, true))
        return;
      this.ChangeLog.LogHitButton(this.instance, this, this.SwingTarget);
    }

    public void CancelSleepWait(object sender, PlayerIndexEventArgs e)
    {
      this.IsSleeping = false;
    }

    private void SleepOption()
    {
      if (!this.CanUseBed)
        return;
      if (this.Gamer.IsHost)
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Sleeping is for the weak.\nBut in any case, how long would you like to sleep?", "4 hours", "8 hours", "Till sunrise", "I'm not weak!", CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.Sleep4Hours);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.Sleep8Hours);
        messageBoxScreenTm.ButtonY += new EventHandler<PlayerIndexEventArgs>(this.SleepTillSunriseHours);
        this.instance.AddScreen((GameScreen) messageBoxScreenTm, this);
      }
      else
        this.instance.StartSleep(this);
    }

    private bool CanUseBed
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.DayNightActive)
          return !this.instance.IsSleeping;
        return false;
      }
    }

    private void Sleep4Hours(object sender, PlayerIndexEventArgs e)
    {
      this.instance.StartSleep(this, 4f);
    }

    private void Sleep8Hours(object sender, PlayerIndexEventArgs e)
    {
      this.instance.StartSleep(this, 8f);
    }

    private void SleepTillSunriseHours(object sender, PlayerIndexEventArgs e)
    {
      this.instance.StartSleep(this, -1f);
    }

    public MapModel ClipboardModel
    {
      get
      {
        InventoryItem leftHand = this.Inventory.LeftHand;
        if (leftHand.ItemID == Item.Clipboard)
        {
          int index = (int) leftHand.Durability - 1;
          if (index >= 0 && index < this.clipboards.Count)
            return this.clipboards[index].Model;
        }
        return (MapModel) null;
      }
    }

    public MapModel GetClipboardModel(InventoryItem item)
    {
      int index = (int) item.Durability - 1;
      if (index < 0 || index >= this.clipboards.Count)
        return (MapModel) null;
      return this.clipboards[index].Model;
    }

    public bool AddClipboard(MapModel model, VoxelModelManager manager)
    {
      int slotID = -1;
      if (this.AddToInventory(new InventoryItem(Item.Clipboard, 1), out slotID) <= 0 || slotID < 0)
        return false;
      int index = this.FindClipboardModel(model);
      if (index < 0)
      {
        index = this.GetFreeClipboardIndex();
        Player.Clipboard clipboard = this.clipboards[index];
        clipboard.Model = model;
        clipboard.ModelManager = manager;
        this.clipboards[index] = clipboard;
      }
      this.Inventory.SetItemDurability(slotID, (ushort) (index + 1));
      this.EquipFromInventory(this.Settings.WieldType == WieldType.RightHand ? this.RightHand : this.LeftHand, slotID);
      this.ClipboardZoom = 1f;
      this.ClipboardRotate = 0.0f;
      return true;
    }

    private void DiscardClipboard(InventoryItem item, int slotID)
    {
      int index = (int) item.Durability - 1;
      if (index < 0 || index >= this.clipboards.Count)
        return;
      Player.Clipboard clipboard = this.clipboards[index];
      if (clipboard.ModelManager == null || Player.ClipboardLoadedElsewhere(this, slotID, index, clipboard.Model))
        return;
      clipboard.ModelManager.UnloadComponent(clipboard.Model);
      clipboard.Model = (MapModel) null;
      clipboard.ModelManager = (VoxelModelManager) null;
      this.clipboards[index] = clipboard;
    }

    private static bool ClipboardLoadedElsewhere(
      Player player,
      int slotID,
      int index,
      MapModel model)
    {
      using (List<Player>.Enumerator enumerator = player.instance.NetworkManager.LocalEnabledPlayers.GetEnumerator())
      {
label_13:
        while (enumerator.MoveNext())
        {
          Player current = enumerator.Current;
          if (current != player)
          {
            for (int index1 = 0; index1 < current.clipboards.Count; ++index1)
            {
              if (current.clipboards[index1].Model == model)
                return true;
            }
          }
          else
          {
            int index1 = 0;
            while (true)
            {
              if (index1 < (int) player.Inventory.PackSize && index1 < player.Inventory.Count)
              {
                if (index1 == slotID || (int) player.Inventory[index1].Durability - 1 != index)
                  ++index1;
                else
                  break;
              }
              else
                goto label_13;
            }
            return true;
          }
        }
      }
      return false;
    }

    private int FindClipboardModel(MapModel model)
    {
      for (int index = 0; index < this.clipboards.Count; ++index)
      {
        if (this.clipboards[index].Model == model)
          return index;
      }
      return -1;
    }

    private int GetFreeClipboardIndex()
    {
      for (int index = 0; index < this.clipboards.Count; ++index)
      {
        if (this.clipboards[index].Model == null)
          return index;
      }
      this.clipboards.Add(new Player.Clipboard());
      return this.clipboards.Count - 1;
    }

    public override bool IsClipboardEquipped
    {
      get
      {
        return this.ClipboardModel != null;
      }
    }

    protected override void PasteClipboardModel(Map.CopyType copyType)
    {
      if (!this.HasPermission(Permissions.Creative))
        return;
      this.instance.CreativeModeHelper.Paste(this, copyType);
      Sounds.PlaySound(Item.Clipboard, ItemSoundType.Use);
    }

    private void CheckForClipboardLoadUnLoad(int slotID1, int slotID2)
    {
      InventoryItem inventoryItem1 = this.Inventory[slotID1];
      if (inventoryItem1.ItemID == Item.Clipboard)
      {
        MapModel clipboardModel = this.GetClipboardModel(inventoryItem1);
        if (clipboardModel != null && clipboardModel.BufferSize > 500000L)
        {
          MapChunkTM mapModelChunk = this.GetMapModelChunk(clipboardModel);
          mapModelChunk?.Content.UnloadChunk(mapModelChunk);
        }
      }
      InventoryItem inventoryItem2 = this.Inventory[slotID2];
      if (inventoryItem2.ItemID != Item.Clipboard)
        return;
      MapModel clipboardModel1 = this.GetClipboardModel(inventoryItem2);
      if (clipboardModel1 == null)
        return;
      MapChunkTM mapModelChunk1 = this.GetMapModelChunk(clipboardModel1);
      if (mapModelChunk1 == null || mapModelChunk1.IsMeshLoaded)
        return;
      this.reloadComponentTimer = 0.5f;
      this.reloadComponentChunk = mapModelChunk1;
    }

    public void BuildUndoModel(
      GlobalPoint3D p,
      GlobalPoint3D size,
      int facing,
      IProgressBar progress)
    {
      this.undoPoint = p;
      this.undoFacing = facing;
    }

    public void UndoLastPaste()
    {
    }

    private void UpdateClipboardModelWorldMatrix()
    {
      MapModel clipboardModel = this.ClipboardModel;
      float rotationFromViewFacing = this.GetClipboardRotationFromViewFacing();
      Vector3 vector3 = clipboardModel.Map.MapSize.ToVector3() * clipboardModel.Map.TileSize;
      Vector3 position1 = this.map.GetPoint(this.Position + this.ViewDirection * (Math.Max(vector3.Y, Math.Max(vector3.X, vector3.Z)) * 0.5f + 3f) * this.ClipboardZoom).ToVector3() * this.map.TileSize;
      Vector3 position2 = new Vector3((float) (-(double) vector3.X * 0.5), 0.0f, (float) (-(double) vector3.Z * 0.5));
      position2.X = (float) (int) position2.X;
      position2.Z = (float) (int) position2.Z;
      this.ClipboardModelWorldMatrix = Matrix.CreateTranslation(position2) * Matrix.CreateRotationY(rotationFromViewFacing) * Matrix.CreateTranslation(position1);
      clipboardModel.World = this.map.GetPoint(Vector3.Transform(Vector3.Zero, this.ClipboardModelWorldMatrix)).ToVector3();
      this.ClipboardModelWorldMatrix *= Matrix.CreateTranslation(-this.Position);
    }

    private float GetClipboardRotationFromViewFacing()
    {
      switch (this.ClipboardModelViewFacing % 4)
      {
        case 0:
          return 0.0f;
        case 1:
          return -1.570796f;
        case 2:
          return 3.141593f;
        default:
          return 1.570796f;
      }
    }

    public int ClipboardModelViewFacing
    {
      get
      {
        if ((double) this.ClipboardRotate <= -1.57079637050629)
          return 1;
        if ((double) this.ClipboardRotate <= 0.0)
          return 0;
        return (double) this.ClipboardRotate >= 1.57079637050629 ? 2 : 3;
      }
    }

    private MapChunkTM GetMapModelChunk(MapModel model)
    {
      if (model != null && model.Map != null)
        return model.Map.GetChunk(GlobalPoint3D.Zero) as MapChunkTM;
      return (MapChunkTM) null;
    }

    protected override void Die(DamageType deathType, Actor attacker, Item weaponID, float damage)
    {
      bool isLocalGamer = this.IsLocalGamer;
      if (isLocalGamer && StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance != null)
        StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.ScreenManager.RemoveScreens(this.PlayerIndex, new ScreenManager.RemoveScreenCondition(this.ShouldRemoveScreenOnDeath));
      this.Raise_PlayerDied(attacker);
      this.ChangeState(ActorState.Dying);
      this.Velocity.X = this.Velocity.Z = 0.0f;
      this.positionInterpolator.Reset();
      this.ClearInputCore();
      if (isLocalGamer && !Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath && this.instance.IsFiniteResources)
        this.DropAllItems((Item[]) null, UpdateBlockMethod.DropTimeDeath);
      if (attacker != null && attacker.IsPlayer && !attacker.IsLocalGamer)
        this.instance.NetworkManager.SendKillConfirm(deathType, (Actor) this, (Actor) null, attacker, weaponID);
      this.deathRecoveryTimer = 0.0f;
      this.killStreak = 0;
      ++this.stats.TotalDeaths;
      if (isLocalGamer)
        this.SendDeathMessage(deathType, attacker);
      this.instance.AddMapMarker(this.map.GetPoint(this.Position), this.Gamertag, MapMarkerType.Graveyard, true);
      this.instance.ExecuteEventScript(ScriptEvent.PlayerDeath, new ScriptExecuteData()
      {
        Actor = (Actor) this,
        Killer = attacker
      });
    }

    private bool ShouldRemoveScreenOnDeath(GameScreen screen)
    {
      return screen != this.PauseMenuScreen && !(screen is ScriptEditScreen);
    }

    public static float V2ToAngle(Vector2 direction)
    {
      if (direction == Vector2.Zero)
        return 0.0f;
      direction.Normalize();
      return (float) Math.Atan2((double) direction.X, -(double) direction.Y);
    }

    public override void DefaultRespawn()
    {
      this.rebuildDepthStringLastPoint = GlobalPoint3D.Zero;
      this.RebuildDepthString();
      if (this.instance.MiniGame != null)
      {
        this.instance.MiniGame.RespawnOnDeath(this);
        this.instance.MiniGame.EquipOnDeath(this);
      }
      else
        this.lastPosition = this.Position = this.map.GetBlockCenter(this.SpawnPoint);
      this.PositionReset = true;
      this.Velocity = Vector3.Zero;
      this.ViewDirection = Vector3.Forward;
      this.instance.OpenLoadingPlayerViewScreen(this);
    }

    public void OnSkillLevelled(SkillData skillData)
    {
      if (!this.instance.IsLocalSkills)
        Globals2.SaveGamertagData(true, false, true);
      if (this.instance.NetworkManager == null)
        return;
      this.instance.NetworkManager.SendPlayerSkill(this, skillData);
    }

    public void ClearWorldShake()
    {
      this.WorldShake = Matrix.Identity;
      this.WorldToolShake = Matrix.Identity;
      this.IsWorldShaking = false;
    }

    public void ShakeWorld(float intensity)
    {
      float num = intensity * 0.5f;
      float x = (float) this.instance.Random.NextDouble() * intensity - num;
      float y = (float) this.instance.Random.NextDouble() * intensity - num;
      float z = (float) this.instance.Random.NextDouble() * intensity - num;
      this.WorldShake = Matrix.CreateTranslation(new Vector3(x, y, z));
      this.WorldToolShake = Matrix.CreateTranslation(new Vector3(x * 0.2f, y * 0.2f, z * 0.2f));
      this.IsWorldShaking = true;
    }

    public void SetNewRumble(float timer, float leftStrength, float rightStrength)
    {
      if (this.rumble == null)
        return;
      this.rumble.SetNewRumble(timer, leftStrength, rightStrength);
    }

    public void ClearRumble()
    {
      if (this.rumble == null)
        return;
      this.rumble.StartRumble(RumbleType.None);
    }

    public void SuspendRumble()
    {
      if (this.rumble == null)
        return;
      this.rumble.Suspend();
    }

    public void ResumeRumble()
    {
      if (this.rumble == null)
        return;
      this.rumble.Resume();
    }

    protected override void OnPermissionChanged()
    {
      base.OnPermissionChanged();
      if (this.instance == null)
        this.isLobbyPermission = true;
      if (!this.HasPermission(Permissions.Fly) && !this.HasFlyConcession)
      {
        this.FlyMode = FlyMode.None;
        this.isFlightAscending = this.isFlightDescending = false;
      }
      if (!this.HasPermission(Permissions.Map))
        this.Settings.MapVisible = false;
      if (!this.HasPermission(Permissions.Spectate) && this.IsSpectating)
        this.virtualPlayer = (Player) null;
      this.SetVoiceState();
    }

    public void ClearIsLobbyPermissionFlag()
    {
      this.isLobbyPermission = false;
    }

    protected override void NotifyPermissionDenied(Permissions permission)
    {
      if ((permission & Permissions.Edit) != Permissions.None || (permission & Permissions.Fly) != Permissions.None)
        return;
      this.instance.AddHostNotification(this, " attempted to perform a" + this.AddPermissionToMessage(Permissions.Admin, permission) + this.AddPermissionToMessage(Permissions.Creative, permission) + this.AddPermissionToMessage(Permissions.Grief, permission) + this.AddPermissionToMessage(Permissions.Map, permission) + this.AddPermissionToMessage(Permissions.Save, permission) + " operation but permission was denied");
    }

    private string AddPermissionToMessage(Permissions check, Permissions permission)
    {
      if ((permission & check) == check)
        return (check == Permissions.Admin ? "n " : " ") + check.ToString();
      return (string) null;
    }

    public void SetVoiceState()
    {
    }

    public void SetAvatar(Player player, ActorType mobType)
    {
      this.SetAvatar(mobType, false);
    }

    private void SetAvatar(ActorType mobType, bool force)
    {
      if (this.instance == null || this.ActorType == mobType && !force)
        return;
      this.ActorType = mobType;
      this.instance.LoadAvatar(mobType, new OnAvatarLoaded(this.OnAvatarLoadedCore));
      if (!this.IsLocalGamer)
        return;
      NetworkManager.Instance.SendPlayerSettings(this, (NetworkGamer) null);
      this.OnPermissionChanged();
    }

    private void OnAvatarLoadedCore(MapModel model, MapModel crouch)
    {
      this.model = model;
      this.crouchModel = crouch;
      model.Flags |= ModelFlags.IsPlayer;
      crouch.Flags |= ModelFlags.IsPlayer;
      bool isCrouching = this.IsCrouching;
      if (model == null)
        return;
      this.SetSize(model.ModelSize, Globals1.NpcTypeData[(int) this.ActorType].ModelHeight / (float) model.ModelSize.Y);
      if (isCrouching)
        this.EyeOffset.Y = this.Size.Y = this.crouchHeight;
      this.SetTesterFlags();
      if (!this.IsGod)
        return;
      this.Health = this.MaxHealth;
      if (this.Inventory.HasItem(Item.SledgeHammer))
        return;
      this.AddToInventory(Item.SledgeHammer, 1);
    }

    public void ToggleAutoPlace()
    {
      this.Settings.ToggleAutoPlace();
      if (this.LeftHand != null)
        this.LeftHand.ItemSwing.SetUserSwingTimeOverride(0.0f);
      if (this.RightHand == null)
        return;
      this.RightHand.ItemSwing.SetUserSwingTimeOverride(0.0f);
    }

    private void SendStatisticsIfChanged()
    {
      if (!this.IsInputEnabled || this.Gamer == null || !this.Gamer.IsLocal)
        return;
      this.synchStatsTimer -= Services.ElapsedTime;
      if ((double) this.synchStatsTimer >= 0.0)
        return;
      this.synchStatsTimer = 30f;
      if (this.stats.IsEqual(this.oldStats))
        return;
      NetworkManager.Instance.SendPlayerStatistics(this.instance.GetPlayerStateDataIndex((Gamer) this.Gamer), this.Gamertag, this.stats, (NetworkGamer) null);
      this.oldStats = this.stats.Clone();
    }

    public void StartCCTV(
      GlobalPoint3D p,
      GlobalPoint3D p2,
      int millisecs,
      float fovNormalized,
      float padSensitivity)
    {
      this.StartCCTV(p, this.map.GetBlockCenter(p2) - this.map.GetBlockCenter(p), millisecs, fovNormalized, padSensitivity, (Actor) null);
    }

    public void StartCCTV(
      GlobalPoint3D p,
      BlockFace dir,
      int millisecs,
      float fovNormalized,
      float padSensitivity,
      Actor target)
    {
      Vector3 viewDir;
      switch (dir)
      {
        case BlockFace.Left:
          viewDir = Vector3.Left;
          break;
        case BlockFace.Right:
          viewDir = Vector3.Right;
          break;
        case BlockFace.Backward:
          viewDir = Vector3.Backward;
          break;
        default:
          viewDir = Vector3.Forward;
          break;
      }
      this.StartCCTV(p, viewDir, millisecs, fovNormalized, padSensitivity, target);
    }

    public void StartCCTV(
      GlobalPoint3D p,
      Vector3 viewDir,
      int millisecs,
      float fovNormalized,
      float padSensitivity,
      Actor target)
    {
      if ((double) this.saveFOVNormalised == 0.0)
        this.saveFOVNormalised = this.FOVNormalized + 1f;
      Player player = new Player((NetworkGamer) null, this.PlayerIndex);
      player.InitVirtual(this.instance, this.map, this, this.map.GetBlockCenter(p), viewDir, fovNormalized, padSensitivity, target);
      this.cctvTimer = (float) millisecs / 1000f;
      this.FOVNormalized = fovNormalized;
      this.virtualPlayer = player;
    }

    private void EndCCTV()
    {
      this.virtualPlayer = (Player) null;
      this.cctvTimer = 0.0f;
      if ((double) this.saveFOVNormalised > 0.0)
        this.FOVNormalized = this.saveFOVNormalised - 1f;
      this.saveFOVNormalised = 0.0f;
    }

    public CreativeOperationData GetCreativeClearDefaults()
    {
      if (this.creativeClearDefaults == null)
      {
        this.creativeClearDefaults = new CreativeOperationData();
        this.creativeClearDefaults.Command = CreativeCommand.Clear;
        this.creativeClearDefaults.GamerID = this.GamerID;
        this.creativeClearDefaults.Desc = "Clear Region";
        this.creativeClearDefaults.Percent = (byte) 100;
        this.creativeClearDefaults.ClearMarkers = true;
      }
      return this.creativeClearDefaults;
    }

    public CreativeOperationData GetCreativeFillDefaults()
    {
      if (this.creativeFillDefaults == null)
      {
        this.creativeFillDefaults = new CreativeOperationData();
        this.creativeFillDefaults.Command = CreativeCommand.Fill;
        this.creativeFillDefaults.GamerID = this.GamerID;
        this.creativeFillDefaults.Desc = "Fill Region";
        this.creativeFillDefaults.Percent = (byte) 100;
        this.creativeFillDefaults.ClearMarkers = true;
      }
      return this.creativeFillDefaults;
    }

    public CreativeOperationData GetCreativeReplaceDefaults()
    {
      if (this.creativeReplaceDefaults == null)
      {
        this.creativeReplaceDefaults = new CreativeOperationData();
        this.creativeReplaceDefaults.Command = CreativeCommand.Replace;
        this.creativeReplaceDefaults.GamerID = this.GamerID;
        this.creativeReplaceDefaults.Desc = "Replace Region";
        this.creativeReplaceDefaults.Percent = (byte) 100;
        this.creativeReplaceDefaults.ClearMarkers = false;
      }
      this.creativeReplaceDefaults.OnCompletion = (Action<CreativeOperationData>) null;
      return this.creativeReplaceDefaults;
    }

    public CreativeOperationData GetCreativeReplaceClipboardDefaults()
    {
      if (this.creativeReplaceClipboardDefaults == null)
      {
        this.creativeReplaceClipboardDefaults = new CreativeOperationData();
        this.creativeReplaceClipboardDefaults.Command = CreativeCommand.Replace;
        this.creativeReplaceClipboardDefaults.GamerID = this.GamerID;
        this.creativeReplaceClipboardDefaults.Desc = "Replace Clipboard";
        this.creativeReplaceClipboardDefaults.Percent = (byte) 100;
      }
      return this.creativeReplaceClipboardDefaults;
    }

    public CreativeOperationData GetCreativeReplaceTextureDefaults()
    {
      if (this.creativeReplaceTextureDefaults == null)
      {
        this.creativeReplaceTextureDefaults = new CreativeOperationData();
        this.creativeReplaceTextureDefaults.Command = CreativeCommand.ReplaceTexture;
        this.creativeReplaceTextureDefaults.GamerID = this.GamerID;
        this.creativeReplaceTextureDefaults.Desc = "Replace Texture";
        this.creativeReplaceTextureDefaults.Percent = (byte) 100;
        this.creativeReplaceTextureDefaults.ClearMarkers = false;
      }
      return this.creativeReplaceTextureDefaults;
    }

    public CreativeOperationData GetCreativePathDefaults()
    {
      if (this.creativePathDefaults == null)
      {
        this.creativePathDefaults = new CreativeOperationData();
        this.creativePathDefaults.Command = CreativeCommand.Path;
        this.creativePathDefaults.GamerID = this.GamerID;
        this.creativePathDefaults.Desc = "Generate Path";
        this.creativePathDefaults.BlockID = (byte) 42;
        this.creativePathDefaults.BlockID1 = (byte) 1;
        this.creativePathDefaults.BlockID2 = (byte) 1;
      }
      return this.creativePathDefaults;
    }

    public CreativeOperationData GetCreativeWallDefaults()
    {
      if (this.creativeWallDefaults == null)
      {
        this.creativeWallDefaults = new CreativeOperationData();
        this.creativeWallDefaults.Command = CreativeCommand.Wall;
        this.creativeWallDefaults.GamerID = this.GamerID;
        this.creativeWallDefaults.Desc = "Generate Wall";
        this.creativeWallDefaults.BlockID = (byte) 42;
        this.creativeWallDefaults.BlockID1 = (byte) 1;
        this.creativeWallDefaults.BlockID2 = (byte) 1;
      }
      return this.creativeWallDefaults;
    }

    public CreativeOperationData GetCreativeLineDefaults()
    {
      if (this.creativeLineDefaults == null)
      {
        this.creativeLineDefaults = new CreativeOperationData();
        this.creativeLineDefaults.Command = CreativeCommand.Line;
        this.creativeLineDefaults.GamerID = this.GamerID;
        this.creativeLineDefaults.Desc = "Generate Line";
        this.creativeLineDefaults.BlockID = (byte) 42;
        this.creativeLineDefaults.BlockID1 = (byte) 1;
        this.creativeLineDefaults.BlockID2 = (byte) 1;
        this.creativeLineDefaults.ClearMarkers = true;
      }
      return this.creativeLineDefaults;
    }

    public CreativeOperationData GetCreativeSphereDefaults()
    {
      if (this.creativeSphereDefaults == null)
      {
        this.creativeSphereDefaults = new CreativeOperationData();
        this.creativeSphereDefaults.Command = CreativeCommand.Sphere;
        this.creativeSphereDefaults.GamerID = this.GamerID;
        this.creativeSphereDefaults.Desc = "Generate Sphere";
        this.creativeSphereDefaults.BlockID = (byte) 42;
        this.creativeSphereDefaults.BlockID1 = (byte) 10;
        this.creativeSphereDefaults.Percent = (byte) 100;
        this.creativeSphereDefaults.ClearMarkers = true;
      }
      return this.creativeSphereDefaults;
    }

    public CreativeOperationData GetCreativeFloodDefaults()
    {
      if (this.creativeFloodDefaults == null)
      {
        this.creativeFloodDefaults = new CreativeOperationData();
        this.creativeFloodDefaults.Command = CreativeCommand.Flood;
        this.creativeFloodDefaults.GamerID = this.GamerID;
        this.creativeFloodDefaults.Desc = "Flood";
        this.creativeFloodDefaults.BlockID = (byte) 0;
        this.creativeFloodDefaults.ClearMarkers = true;
      }
      return this.creativeFloodDefaults;
    }

    public CreativeOperationData GetCreativeTreesDefaults()
    {
      if (this.creativeTreesDefaults == null)
      {
        this.creativeTreesDefaults = new CreativeOperationData();
        this.creativeTreesDefaults.Command = CreativeCommand.Trees;
        this.creativeTreesDefaults.GamerID = this.GamerID;
        this.creativeTreesDefaults.Desc = "Generate Trees";
        this.creativeTreesDefaults.ClearMarkers = true;
        this.creativeTreesDefaults.Data = (object) new CreativeGenerateTreeData()
        {
          TreeCount = 1,
          CompsSelected = new bool[7]
        };
      }
      return this.creativeTreesDefaults;
    }

    public void SetCreativeClearDefaults(CreativeOperationData data)
    {
      this.creativeClearDefaults.ResetDefaults(data);
    }

    public void SetCreativeFillDefaults(CreativeOperationData data)
    {
      this.creativeFillDefaults.ResetDefaults(data);
    }

    public void SetCreativeReplaceDefaults(CreativeOperationData data)
    {
      this.creativeReplaceDefaults.ResetDefaults(data);
    }

    public void SetCreativeReplaceClipboardDefaults(CreativeOperationData data)
    {
      this.creativeReplaceClipboardDefaults.ResetDefaults(data);
    }

    public void SetCreativeReplaceTextureDefaults(CreativeOperationData data)
    {
      this.creativeReplaceTextureDefaults.ResetDefaults(data);
    }

    public void SetCreativePathDefaults(CreativeOperationData data)
    {
      this.creativePathDefaults.ResetDefaults(data);
    }

    public void SetCreativeWallDefaults(CreativeOperationData data)
    {
      this.creativeWallDefaults.ResetDefaults(data);
    }

    public void SetCreativeLineDefaults(CreativeOperationData data)
    {
      this.creativeLineDefaults.ResetDefaults(data);
    }

    public void SetCreativeSphereDefaults(CreativeOperationData data)
    {
      this.creativeSphereDefaults.ResetDefaults(data);
    }

    public void SetCreativeFloodDefaults(CreativeOperationData data)
    {
      this.creativeFloodDefaults.ResetDefaults(data);
    }

    public void SetCreativeTreesDefaults(CreativeOperationData data)
    {
      this.creativeTreesDefaults.ResetDefaults(data);
      CreativeGenerateTreeData data1 = data.Data as CreativeGenerateTreeData;
      if (data1 == null)
        return;
      CreativeGenerateTreeData data2 = this.creativeTreesDefaults.Data as CreativeGenerateTreeData;
      if (data2 == null)
        return;
      data2.TreeCount = data1.TreeCount;
      data1.CompsSelected.CopyTo((Array) data2.CompsSelected, 0);
    }

    public bool HasDecalApplicatorEquipped
    {
      get
      {
        if (this.tempDecalApplicatorHand == null && this.RightHand.ItemID != Item.DecalApplicator)
          return this.LeftHand.ItemID == Item.DecalApplicator;
        return true;
      }
    }

    public void ApplyDecal(Hand hand)
    {
      if (!this.SwingTargetIsValid || !this.HasPermission(Permissions.Edit) || (this.instance.IsInZoneType(this.SwingTarget, ZoneType.NoEdit, this.GamerID) || this.map.BlockData[(int) this.map.GetBlockID(this.SwingTarget)].Buffer != (byte) 0))
        return;
      byte auxFullData = this.map.GetAuxFullData(this.SwingTarget);
      int num = (int) auxFullData >> 4;
      int currentBlockTexture = this.GetCurrentBlockTexture(Block.zLastBlockID);
      if (currentBlockTexture == num)
      {
        this.OpenDecalScreen(hand);
      }
      else
      {
        byte auxData = (byte) (((int) auxFullData & 15) + (currentBlockTexture << 4));
        this.map.SetAuxData(this.SwingTarget, auxFullData, auxData, UpdateBlockMethod.Player, this.GamerID, true);
        this.map.Commit();
      }
    }

    public bool ApplyDecal(Player player, Block textureID)
    {
      if (this.tempDecalApplicatorHand == null)
      {
        if (this.RightHand.ItemID == Item.DecalApplicator)
          this.tempDecalApplicatorHand = this.RightHand;
        else if (this.LeftHand.ItemID == Item.DecalApplicator)
          this.tempDecalApplicatorHand = this.LeftHand;
      }
      if (this.tempDecalApplicatorHand == null)
        return false;
      int num = (int) this.instance.Map.ChangeBlockTexture(this, this.SwingTarget, Block.zLastBlockID, textureID);
      return true;
    }

    private void OpenDecalScreen(Hand hand)
    {
      this.tempDecalApplicatorHand = hand;
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this, new SelectBlockCallBack(this.ApplyDecal), "Select Decal", BlockSelectMode.SelectingDecal, Block.zLastBlockID, (int) this.instance.Map.GetAuxHighData(this.SwingTarget)), this);
    }

    public bool IsConsoleOpen
    {
      get
      {
        if (this.consoleWin != null)
          return this.consoleWin.Parent != null;
        return false;
      }
    }

    public void OpenConsole()
    {
      if (this.console == null)
      {
        GamertagData orAddGamertagData = Globals2.GamertagData.GetOrAddGamertagData(this.PlayerIndex);
        Point consolePos = orAddGamertagData.ConsolePos;
        Point consoleSize = orAddGamertagData.ConsoleSize;
        this.consoleWin = new ConsoleWindow(consolePos.X, consolePos.Y, consoleSize.X, consoleSize.Y, new Action<string>(this.OnConsoleCommand));
        this.consoleWin.Font = Globals1.FontConsolas;
        this.consoleWin.TextScale = 0.75f;
        this.consoleWin.AddFlags(Window.WinFlags.IsDragable | Window.WinFlags.ClipChildren | Window.WinFlags.TrapMouse);
        this.consoleWin.CloseHandler = new Action(this.CloseConsole);
        this.consoleWin.ExitCommand = "exit";
        this.consoleWin.ExitKey = InputManager1.GetInputKey(this.PlayerIndex, PlayerInput.OpenConsole);
        this.consoleWin.DragEndHandler += new Window.WindowDragHandler(this.ConsoleDragEnd);
        this.console = new GameConsole(this.consoleWin, (ITMGame) this.instance, (ITMPlayer) this);
      }
      if (this.consoleWin.Parent != null)
        return;
      this.consoleWin.SetPrompt(this.DisplayGamertag + ">");
      GameScreen topScreen = TotalMinerGame.Instance.ScreenManager.GetTopScreen(new PlayerIndex?(this.PlayerIndex));
      topScreen.WindowManager.Root.AddChild((StudioForge.Engine.Core.Node) this.consoleWin);
      topScreen.WindowManager.SetInputWindow((ITextInputWindow) this.consoleWin);
      InputManager.PushVirtualMouse();
      Mouse.SetPosition((int) ((double) this.consoleWin.Position.X + (double) this.consoleWin.Size.X), (int) this.consoleWin.Position.Y);
      this.saveMouseScale = TotalMinerGame.Instance.ScreenManager.MouseScale;
      TotalMinerGame.Instance.ScreenManager.MouseScale = 0.5f;
      this.ClearInput();
    }

    private void CloseConsole()
    {
      this.consoleWin.RemoveSelf();
      InputManager.PopVirtualMouse();
      TotalMinerGame.Instance.ScreenManager.MouseScale = this.saveMouseScale;
    }

    private void OnConsoleCommand(string cmd)
    {
      this.console.RunCommand(cmd);
    }

    private void ConsoleDragEnd(object sender, WindowDragEventArgs args)
    {
      GamertagData gamertagData = Globals2.GamertagData.GetGamertagData(this.PlayerIndex);
      if (gamertagData == null)
        return;
      gamertagData.ConsolePos = new Point((int) this.consoleWin.Position.X, (int) this.consoleWin.Position.Y);
    }

    public delegate void TradeEventHandler(object sender, TradeEventArgs e);

    public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);

    public delegate void PlayerTextEventHandler(object sender, PlayerTextEventArgs e);

    public delegate void CharacterEventHandler(object sender, ActorEventArgs e);

    public delegate void CharacterAndItemEventHandler(object sender, ActorItemEventArgs e);

    public delegate void MobEventHandler(object sender, NpcEventArgs e);

    private enum BotMode
    {
      Normal,
    }

    private struct NetworkInstanceData2
    {
      public float Time;
      public Vector3 Position;
      public Vector3 ViewDirection;
    }

    private struct NetworkInstanceData
    {
      public bool IsUpdated;
      public Vector3 Position;
      public Vector3 ViewDirection;
      public float SizeY;
      public ActorState State;
      public bool IsFlying;
      public bool PositionReset;
      public bool IsIceEffectActive;
      public byte LeftHandSwingCount;
      public byte RightHandSwingCount;
      public Block FootSoundBlock;
      public float Health;
      public long ElapsedMillisecs;
      public long PrevElapsedMillisecs;
      public long CurrentElapsedMillisecs;
    }

    private struct FOVState
    {
      public float FOVNormalized;
    }

    private struct Clipboard
    {
      public MapModel Model;
      public VoxelModelManager ModelManager;
    }

    public class ActionRequest : IProgressBar
    {
      public GlobalPoint3D Point;
      public Block BlockID;
      public string Message;
      public Color Color;
      public double Seconds;
      public double SecondsHidden;
      public float Progress;
      private float progressFactor;
      private string message;

      public ActionRequest(string message)
      {
        this.progressFactor = 1f;
        this.message = message;
        this.SetMessage();
      }

      float IProgressBar.Progress
      {
        get
        {
          return this.Progress;
        }
      }

      float IProgressBar.Factor
      {
        get
        {
          return this.progressFactor;
        }
        set
        {
          this.progressFactor = value;
        }
      }

      object IProgressBar.Tag { get; set; }

      string IProgressBar.Text
      {
        get
        {
          return this.Message;
        }
        set
        {
        }
      }

      void IProgressBar.Reset()
      {
        this.Progress = 0.0f;
      }

      void IProgressBar.Reset(float value)
      {
        this.Progress = value;
      }

      void IProgressBar.AddProgress(float increment)
      {
        this.Progress += increment * this.progressFactor;
        this.SetMessage();
      }

      private void SetMessage()
      {
        this.Message = string.Format("{0} {1}%", (object) this.message, (object) (int) ((double) this.Progress * 100.0));
      }
    }

    public struct ButtonScript
    {
      public string Script;
      public string Text;
      public Vector2? Pos;
      public float? Scale;
    }
  }
}
