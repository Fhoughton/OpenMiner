// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Actor
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal abstract class Actor : GameObjectBase, INPCBehaviour, ITMActor
  {
    public static List<string> Logs = new List<string>();
    private static List<Actor> tempFindList = new List<Actor>(100);
    private static List<SoundBroadcast> tempSoundFindList = new List<SoundBroadcast>(10);
    protected static Vector3 leftfaceMin = new Vector3(0.0f, -1f, 0.0f);
    protected static Vector3 leftfaceMax = new Vector3(0.0f, 0.0f, 1f);
    protected static Vector3 rightfaceMin = new Vector3(1f, -1f, 0.0f);
    protected static Vector3 rightfaceMax = new Vector3(1f, 0.0f, 1f);
    protected static Vector3 forwardsfaceMin = new Vector3(0.0f, -1f, 0.0f);
    protected static Vector3 forwardsfaceMax = new Vector3(1f, 0.0f, 0.0f);
    protected static Vector3 backwardsfaceMin = new Vector3(0.0f, -1f, 1f);
    protected static Vector3 backwardsfaceMax = new Vector3(1f, 0.0f, 1f);
    protected static Vector3 upfaceMin = new Vector3(0.0f, 0.0f, 0.0f);
    protected static Vector3 upfaceMax = new Vector3(1f, 0.0f, 1f);
    protected static Vector3 downfaceMin = new Vector3(0.0f, -1f, 0.0f);
    protected static Vector3 downfaceMax = new Vector3(1f, -1f, 1f);
    public float SpeedMultiplier = 1f;
    public float GravityMultiplier = 1f;
    public NpcProperties Properties = new NpcProperties();
    protected float playerPainSoundDelay = 4f;
    protected float halfSizeFactor = 0.5f;
    protected PcgRandom random = new PcgRandom(new Random().Next());
    protected Vec3Interpolator positionInterpolator = new Vec3Interpolator();
    private List<Actor.TimedPoint> ppoints1 = new List<Actor.TimedPoint>();
    private List<GlobalPoint3D> ppoints2 = new List<GlobalPoint3D>();
    public const float FreezeEffectImmunityTime = 4f;
    public const float FreezeEffectTime = 3f;
    private const float timeRequiredForTeleportEntry = 2f;
    private bool isMovingTo;
    private Vector3 moveToPos;
    private float moveToVelMod;
    private bool moveToCanJump;
    private MoveType moveToType;
    protected float lastDialogTimer;
    protected DialogNode lastDialog;
    public Vector3 Position;
    public Vector3 EyeOffset;
    public Vector3 EyePosition;
    public Vector2 Size;
    public Vector3 Velocity;
    public Vector3 VisualVelocity;
    public Vector2 MaxVelocity;
    public ActorType ActorType;
    public float Oxygen;
    public BoundingBox Box;
    public BoundingBox BodyBox;
    public BoundingBox CriticalHitBox;
    public BoundingSphere Sphere;
    public List<BoundingSphere> Spheres;
    public float Health;
    public int Seed;
    public Vector3 ViewDirection;
    public Vector3 ViewDirNoYNormalized;
    public EquipmentInventory Inventory;
    public Vector3 KnockForce;
    public Matrix ViewMatrix;
    public Matrix ViewMatrixLocal;
    public Matrix ProjectionMatrix;
    public BoundingFrustum Frustum;
    public float DrawScale;
    public bool DrawUpDownViewDirection;
    public readonly NetworkGamer Gamer;
    public bool DisplayPleaseWaitMessage;
    public bool IsViewLoading;
    public int SwingFacePos;
    public BlockFace SwingFace;
    public GlobalPoint3D SwingTarget;
    public float SwingTargetDistance;
    public BoundingBox SwingTargetBox;
    public GlobalPoint3D PlaceTarget;
    public HitTarget HitTarget;
    public NpcBase LastHitTarget;
    public CharacterSkillsData SkillsData;
    public GlobalPoint3D HittingBlockOnXZ;
    public bool IsHittingBlockOnXZ;
    public float FreezeTimer;
    public float FreezeImmunityTimer;
    public bool PositionReset;
    public FlyMode FlyMode;
    public GlobalPoint3D? Waypoint;
    public float Alpha;
    public float HoverHeight;
    public AudioEmitter AudioEmitter;
    public bool IsGod;
    public bool IsTester;
    public bool IsTesterman;
    public bool IsDeveloper;
    public bool IsGodOrTester;
    public bool IsGodOrTesterRetail;
    public Hand LeftHand;
    public Hand RightHand;
    public Hand NextHand;
    public int LandingSoundDelay;
    public int Splinter;
    public float SplinterProgress;
    public bool IsRubberBanding;
    protected MapTM map;
    protected ActorState actorState;
    protected BehaviourTree behaviourTree;
    protected ExecutionEngine behaviourEngine;
    protected BehaviourTree dialogTree;
    protected float playPainSoundTimer;
    protected GameInstance instance;
    protected bool isOnGround;
    protected float fov;
    protected float farClip;
    protected float nearClip;
    protected Vector3 moveDir;
    protected Vector3 viewMatrixPosition;
    protected Vector3 lastPosition;
    protected Vector3 lastVelocity;
    protected float fullHeight;
    protected float fullEyeHeight;
    protected float crouchHeight;
    protected float strikeTimer;
    protected float disableSwingTargetTimer;
    protected List<byte> nonSwingTargets;
    protected GlobalPoint3D lastSwingTarget;
    protected float lastSwingTargetDistance;
    protected bool lastSkipCollision;
    protected bool isOnRope;
    protected bool isOnLadder;
    protected bool isFlightDescending;
    protected bool isFlightAscending;
    protected Vector3 accVel;
    protected float duckHead;
    protected bool useCustomWalkSoundLogic;
    protected CharacterEffectManager effectManager;
    protected byte currentBlockAux;
    protected float age;
    protected Permissions m_permission;
    protected int jumpCounter;
    protected bool jumpingInput;
    protected int reach;
    protected CoordType lookAtType;
    protected Vector3 lookAtPosition;
    protected bool lookAtInstant;
    protected ActorAnim avatarAnim;
    protected byte qtyPlaced;
    private float secondTimer;
    private float insideTeleportTimer;
    private bool teleportSoundStarted;
    private Vector3 lastWalkSoundPosition;
    private NpcQueryPreference sortType;
    private Vector3 lastMoveTarget;
    private float lastMoveTargetFullDistance;
    private float jumpTimer;
    public bool IsInWater;
    public bool IsUnderWater;
    public bool IsUnderLava;
    protected int calcSwingTargetDelay;

    ActorType ITMActor.ActorType
    {
      get
      {
        return this.ActorType;
      }
    }

    ActorState ITMActor.ActorState
    {
      get
      {
        return this.actorState;
      }
    }

    string ITMActor.Name
    {
      get
      {
        return this.DisplayGamertag;
      }
    }

    Vector3 ITMActor.Position
    {
      get
      {
        return this.Position;
      }
      set
      {
        this.Position = value;
      }
    }

    Vector3 ITMActor.EyeOffset
    {
      get
      {
        return this.EyeOffset;
      }
      set
      {
        this.EyeOffset = value;
      }
    }

    Vector3 ITMActor.EyePosition
    {
      get
      {
        return this.EyePosition;
      }
    }

    Vector3 ITMActor.Velocity
    {
      get
      {
        return this.Velocity;
      }
      set
      {
        this.Velocity = value;
      }
    }

    Vector3 ITMActor.ViewDirection
    {
      get
      {
        return this.ViewDirection;
      }
      set
      {
        this.SetViewDirecton(value);
      }
    }

    Matrix ITMActor.ViewMatrix
    {
      get
      {
        return this.ViewMatrix;
      }
    }

    Matrix ITMActor.ViewMatrixLocal
    {
      get
      {
        return this.ViewMatrixLocal;
      }
    }

    Matrix ITMActor.ProjectionMatrix
    {
      get
      {
        return this.ProjectionMatrix;
      }
      set
      {
        this.ProjectionMatrix = value;
      }
    }

    BoundingFrustum ITMActor.Frustum
    {
      get
      {
        return this.Frustum;
      }
    }

    BoundingBox ITMActor.Box
    {
      get
      {
        return this.Box;
      }
    }

    ITMInventory ITMActor.Inventory
    {
      get
      {
        return (ITMInventory) this.Inventory;
      }
    }

    ITMHand ITMActor.LeftHand
    {
      get
      {
        return (ITMHand) this.LeftHand;
      }
    }

    ITMHand ITMActor.RightHand
    {
      get
      {
        return (ITMHand) this.RightHand;
      }
    }

    AudioEmitter ITMActor.AudioEmitter
    {
      get
      {
        return this.AudioEmitter;
      }
    }

    float ITMActor.Oxygen
    {
      get
      {
        return this.Oxygen;
      }
      set
      {
        this.Oxygen = value;
      }
    }

    float ITMActor.Health
    {
      get
      {
        return this.Health;
      }
      set
      {
        this.Health = value;
      }
    }

    float ITMActor.MaxHealth
    {
      get
      {
        return this.MaxHealth;
      }
    }

    int ITMActor.Reach
    {
      get
      {
        return this.reach;
      }
      set
      {
        this.reach = MyMathHelper.Clamp(value, 1, 128);
      }
    }

    FlyMode ITMActor.FlyMode
    {
      get
      {
        return this.FlyMode;
      }
      set
      {
        this.FlyMode = value;
      }
    }

    bool ITMActor.IsOnGround
    {
      get
      {
        return this.isOnGround;
      }
    }

    bool ITMActor.IsDeadOrInactiveOrDisabled
    {
      get
      {
        return this.IsDeadOrInactiveOrDisabled;
      }
    }

    int ITMActor.AddToInventory(InventoryItem item)
    {
      return this.AddToInventory(item);
    }

    int ITMActor.AddToInventory(InventoryItem item, out int slotID)
    {
      return this.AddToInventory(item, out slotID);
    }

    bool ITMActor.EquipFromInventory(Item itemID)
    {
      return this.EquipFromInventory(itemID);
    }

    bool ITMActor.EquipFromInventory(ITMHand hand, Item itemID)
    {
      Hand hand1 = hand as Hand;
      if (hand1 == null)
        return false;
      return this.EquipFromInventory(hand1, itemID);
    }

    bool ITMActor.UnequipToInventory(EquipIndex equipIndex)
    {
      return this.UnequipToInventory(equipIndex);
    }

    bool ITMActor.IsItemEquipped(Item itemID)
    {
      return this.IsItemEquipped(itemID);
    }

    bool ITMActor.IsItemEquippedAndUsable(Item itemID)
    {
      return this.IsItemEquippedAndUsable(itemID);
    }

    int ITMActor.GetItemEquippedSlot(Item itemID)
    {
      return this.GetItemEquippedSlot(itemID);
    }

    void ITMActor.DropItem(int slotID)
    {
      this.DropItem(ParticleType.None, slotID, UpdateBlockMethod.DropTimeShort);
    }

    float ITMActor.TakeDamageAndDisplay(
      DamageType damageType,
      float damage,
      Vector3 knockForce)
    {
      return this.TakeDamageAndDisplay(damageType, damage, knockForce);
    }

    float ITMActor.TakeDamageAndDisplay(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      ITMActor attacker,
      Item weaponID,
      SkillType attackType)
    {
      return this.TakeDamageAndDisplay(damageType, damage, knockForce, attacker as Actor, weaponID, attackType);
    }

    bool ITMActor.HasPermission(Permissions permissions)
    {
      return this.HasPermission(permissions);
    }

    bool ITMActor.HasPermissionAny(Permissions permissions)
    {
      return this.HasPermissionAny(permissions);
    }

    bool ITMActor.LineOfSightTest(Vector3 targetPos, float distance)
    {
      return this.LineOfSightTest(targetPos, distance);
    }

    void ITMActor.TeleportTo(Vector3 pos)
    {
      this.TeleportTo(pos);
    }

    protected virtual void SetViewDirecton(Vector3 dir)
    {
      this.ViewDirection = dir;
    }

    void ITMActor.UpdateMatrices()
    {
      this.UpdateMatrices();
    }

    bool INPCBehaviour.IsAlive
    {
      get
      {
        if (this.IsEnabledField)
          return this.actorState == ActorState.Alive;
        return false;
      }
    }

    ActorAIDataXML INPCBehaviour.AIData
    {
      get
      {
        return this.AIData;
      }
    }

    float INPCBehaviour.Age
    {
      get
      {
        return this.age;
      }
    }

    ITMMap INPCBehaviour.Map
    {
      get
      {
        return (ITMMap) this.map;
      }
    }

    GlobalPoint3D INPCBehaviour.SwingTarget
    {
      get
      {
        return this.SwingTarget;
      }
    }

    BlockFace INPCBehaviour.SwingFace
    {
      get
      {
        return this.SwingFace;
      }
    }

    BehaviourTreeNode INPCBehaviour.LastNode { get; set; }

    NpcProperties INPCBehaviour.Properties
    {
      get
      {
        return this.Properties;
      }
    }

    PcgRandom INPCBehaviour.Random
    {
      get
      {
        return this.random;
      }
    }

    public DialogNode CurrentDialog { get; set; }

    public INPCBehaviour CurrentDialogTarget { get; set; }

    public INPCBehaviour AITarget
    {
      get
      {
        TargetData? lastTargetedBy = TargetingSystem.GetLastTargetedBy((INPCBehaviour) this);
        if (!lastTargetedBy.HasValue)
          return (INPCBehaviour) null;
        return lastTargetedBy.Value.Targeter;
      }
    }

    GlobalPoint3D INPCBehaviour.SpawnPoint
    {
      get
      {
        NpcSpawnBlock spawnBlock = this.GetSpawnBlock();
        if (spawnBlock == null)
          return GlobalPoint3D.Zero;
        return spawnBlock.Point;
      }
    }

    bool INPCBehaviour.LookAt(CoordType lookAtType, Vector3 pos, bool instant)
    {
      this.lookAtType = lookAtType;
      this.lookAtPosition = pos;
      this.lookAtInstant = instant;
      if (instant)
        return true;
      Vector3 finalLookAtPosition = this.GetFinalLookAtPosition(lookAtType, pos);
      if (this.SwingTargetIsValid)
        return this.SwingTarget == this.map.GetPoint(finalLookAtPosition);
      return lookAtType == CoordType.TargetRelative && this.BuildHitTargetData(this.ViewDirection, Vector3.Zero, HitTargetOptions.All, (List<ActorType>) null).Target == this.AITarget;
    }

    void INPCBehaviour.MoveTo(
      Vector3 pos,
      float velMod,
      bool canJump,
      MoveType moveType)
    {
      this.isMovingTo = true;
      this.moveToPos = pos;
      this.moveToVelMod = velMod;
      this.moveToCanJump = canJump;
      this.moveToType = moveType;
    }

    bool INPCBehaviour.IsInZone(ZoneType type)
    {
      return this.GameInstance.IsInZoneType(this.Box, type, GamerID.Sys1);
    }

    bool INPCBehaviour.IsInZone(string zoneName)
    {
      return this.GameInstance.IsInZone(this.Box, zoneName);
    }

    void INPCBehaviour.SwingHand(InventoryHand handType)
    {
      ((INPCBehaviour) this).SwingHand(handType, (List<ActorType>) null);
    }

    void INPCBehaviour.SwingHand(InventoryHand handType, List<ActorType> excludeTypes)
    {
      Hand hand1;
      switch (handType)
      {
        case InventoryHand.Left:
          hand1 = this.LeftHand;
          break;
        case InventoryHand.Right:
          hand1 = this.RightHand;
          break;
        default:
          hand1 = (Hand) null;
          break;
      }
      Hand hand2 = hand1;
      if (hand2 == null || this.LeftHand.IsSwinging || this.RightHand.IsSwinging)
        return;
      hand2.SetIsSwinging(true, excludeTypes);
    }

    Vector3 INPCBehaviour.GetRandomPositionNearPoint(
      Vector3 pos,
      float distance)
    {
      int num = 10;
      while (--num > 0 && this.map != null)
      {
        Vector2 vector2 = new Vector2((float) this.random.NextDouble() - 0.5f, (float) this.random.NextDouble() - 0.5f);
        vector2.Normalize();
        vector2.X *= distance;
        vector2.Y *= distance;
        Vector3 position = new Vector3(vector2.X, 0.0f, vector2.Y) + pos;
        GlobalPoint3D point = this.map.GetPoint(position);
        if (this.map.IsValidPoint(point) && this.map.GetChunk(point) != null)
        {
          GlobalPoint3D groundPoint = this.map.GetGroundPoint(point);
          if (groundPoint.Y < this.map.MapBound.Max.Y - 1)
          {
            switch ((Block) this.map.GetBlockID(groundPoint + GlobalPoint3D.Up))
            {
              case Block.Water:
                if (this.IsFloatingInWater)
                  return position;
                continue;
              case Block.Lava:
                if (this.IsFloatingInLiquid(Block.Lava))
                  return position;
                continue;
              case Block.Fire:
                if (this.map.GetBlockID(this.map.GetPoint(this.Position + new Vector3(0.0f, 0.1f, 0.0f))) == (byte) 118)
                  return position;
                continue;
              default:
                return position;
            }
          }
        }
      }
      return pos;
    }

    INPCBehaviour INPCBehaviour.FindActor(
      NpcQueryPreference preference,
      float distance,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes)
    {
      bool flag1 = searchTypes != null && searchTypes.Count > 0;
      bool flag2 = excludeTypes != null && excludeTypes.Count > 0;
      bool flag3 = flag1 && searchTypes.Contains(ActorType.Player);
      bool flag4 = (preference & NpcQueryPreference.Visible) > NpcQueryPreference.None;
      if ((double) distance == 0.0)
        distance = this.FarClip;
      List<Actor> moveableCharacters = this.instance.AllMoveableCharacters;
      for (int index = moveableCharacters.Count - 1; index >= 0; --index)
      {
        Actor target = moveableCharacters[index];
        if (target != this && target != null && !target.IsDeadOrInactiveOrDisabled && (!flag1 || flag3 && target.IsPlayer || searchTypes.Contains(target.ActorType)) && (!flag2 || !excludeTypes.Contains(target.IsPlayer ? ActorType.Player : target.ActorType)))
        {
          float distance1 = Vector3.Distance(this.Position, target.Position);
          if (!flag4 || (double) distance > (double) distance1 && !target.IsItemEquippedAndUsable(Item.PredatorAmulet) && (this.Frustum.Intersects(target.Box) && this.LineOfSightTest(target, distance1)))
            Actor.tempFindList.Add(target);
        }
      }
      if (Actor.tempFindList.Count <= 0)
        return (INPCBehaviour) null;
      this.sortType = preference;
      Actor.tempFindList.Sort(new Comparison<Actor>(this.SortTargets));
      Actor tempFind = Actor.tempFindList[0];
      Actor.tempFindList.Clear();
      return (INPCBehaviour) tempFind;
    }

    SoundBroadcast? INPCBehaviour.FindSound(
      NpcQueryPreference preference,
      float distance,
      List<SoundType> soundTypes,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes)
    {
      bool flag1 = soundTypes != null && soundTypes.Count > 0;
      bool flag2 = searchTypes != null && searchTypes.Count > 0;
      bool flag3 = excludeTypes != null && excludeTypes.Count > 0;
      bool flag4 = flag2 && searchTypes.Contains(ActorType.Player);
      bool flag5 = (preference & NpcQueryPreference.Visible) > NpcQueryPreference.None;
      if ((double) distance == 0.0)
        distance = this.HearingRange;
      List<SoundBroadcast> broadcastSounds = this.instance.BroadcastSounds;
      for (int index = broadcastSounds.Count - 1; index >= 0; --index)
      {
        SoundBroadcast soundBroadcast = broadcastSounds[index];
        if ((!flag1 || soundTypes.Contains(soundBroadcast.SoundType)) && (soundBroadcast.Actor == null || soundBroadcast.Actor != this && !soundBroadcast.Actor.IsDeadOrInactiveOrDisabled) && (!flag2 || soundBroadcast.Actor != null && (!flag4 || soundBroadcast.Actor.IsPlayer) && searchTypes.Contains(soundBroadcast.Actor.ActorType)) && (!flag3 || soundBroadcast.Actor == null || !excludeTypes.Contains(soundBroadcast.Actor.IsPlayer ? ActorType.Player : soundBroadcast.Actor.ActorType)))
        {
          float distance1 = Vector3.Distance(this.Position, soundBroadcast.Origin);
          if ((double) distance > (double) distance1 && (!flag5 || this.Frustum.Intersects(new BoundingSphere(soundBroadcast.Origin, 0.1f)) && this.LineOfSightTest(soundBroadcast.Origin, distance1)))
            Actor.tempSoundFindList.Add(soundBroadcast);
        }
      }
      if (Actor.tempSoundFindList.Count <= 0)
        return new SoundBroadcast?();
      this.sortType = preference;
      Actor.tempSoundFindList.Sort(new Comparison<SoundBroadcast>(this.SortSounds));
      SoundBroadcast tempSoundFind = Actor.tempSoundFindList[0];
      Actor.tempSoundFindList.Clear();
      return new SoundBroadcast?(tempSoundFind);
    }

    void INPCBehaviour.SetProperties(NpcProperties properties)
    {
      this.Properties.SetFrom(properties);
      if (properties.Reach.HasValue)
        this.Reach = this.Properties.Reach.Value;
      if (!properties.EquipBody.HasValue || !properties.EquipBody.Value)
        return;
      this.EquipBodyFromInventory();
    }

    void INPCBehaviour.Jump(float height)
    {
      if (this.IsOnGround || this.isOnRope)
      {
        this.jumpCounter = 0;
        this.jumpingInput = true;
        this.Velocity.Y = this.JumpSpeed;
      }
      else if ((double) this.jumpCounter < (double) height && (double) this.Velocity.Y > 0.0)
      {
        ++this.jumpCounter;
        this.jumpingInput = true;
        this.Velocity.Y = this.JumpSpeed;
      }
      else
        this.jumpingInput = false;
    }

    bool INPCBehaviour.EquipItem(InventoryHand hand, Item itemID)
    {
      return this.EquipFromInventory(hand == InventoryHand.Left ? this.LeftHand : this.RightHand, itemID);
    }

    bool INPCBehaviour.EquipItem(InventoryHand hand, ItemType itemType)
    {
      int itemHighestValue = this.Inventory.FindItemHighestValue(itemType);
      if (itemHighestValue >= 0)
        return this.EquipFromInventory(hand == InventoryHand.Left ? this.LeftHand : this.RightHand, itemHighestValue, (int) this.Inventory.PackSize);
      return false;
    }

    bool INPCBehaviour.EquipItem(InventoryHand hand, ItemSubType itemSubType)
    {
      int itemHighestValue = this.Inventory.FindItemHighestValue(itemSubType);
      if (itemHighestValue >= 0)
        return this.EquipFromInventory(hand == InventoryHand.Left ? this.LeftHand : this.RightHand, itemHighestValue, (int) this.Inventory.PackSize);
      return false;
    }

    Vector3 INPCBehaviour.GetFinalPosition(CoordType type, Vector3 pos)
    {
      return this.GetFinalLookAtPosition(type, pos);
    }

    public virtual void StandStill()
    {
      this.Velocity.X = this.Velocity.Z = 0.0f;
      this.isMovingTo = false;
    }

    private int SortTargets(Actor a1, Actor a2)
    {
      if ((this.sortType & NpcQueryPreference.Weakest) > NpcQueryPreference.None)
      {
        float health = a1.Health;
        int num = a2.Health.CompareTo(health);
        if (num != 0)
          return num;
      }
      if ((this.sortType & NpcQueryPreference.Strongest) > NpcQueryPreference.None)
      {
        int num = a1.Health.CompareTo(a2.Health);
        if (num != 0)
          return num;
      }
      if ((this.sortType & NpcQueryPreference.LowestHP) > NpcQueryPreference.None)
      {
        float health = a1.Health;
        int num = a2.Health.CompareTo(health);
        if (num != 0)
          return num;
      }
      if ((this.sortType & NpcQueryPreference.HighestHP) > NpcQueryPreference.None)
      {
        int num = a1.Health.CompareTo(a2.Health);
        if (num != 0)
          return num;
      }
      return Vector3.DistanceSquared(this.Position, a1.Position).CompareTo(Vector3.DistanceSquared(this.Position, a2.Position));
    }

    private int SortSounds(SoundBroadcast s1, SoundBroadcast s2)
    {
      if ((this.sortType & NpcQueryPreference.Closest) > NpcQueryPreference.None)
        return Vector3.DistanceSquared(this.Position, s1.Origin).CompareTo(Vector3.DistanceSquared(this.Position, s2.Origin));
      return s1.Tick.CompareTo(s2.Tick);
    }

    private bool LineOfSightTest(Actor target, float distance)
    {
      Vector3 position = target.Position;
      float y = position.Y;
      position.Y += 0.2f;
      if (this.LineOfSightTest(position, distance))
        return true;
      position.Y = (float) ((double) y + (double) target.EyeOffset.Y - 0.100000001490116);
      return this.LineOfSightTest(position, distance);
    }

    private bool LineOfSightTest(Vector3 targetPos, float distance)
    {
      Vector3 dir = new Vector3();
      dir.X = targetPos.X - this.EyePosition.X;
      dir.Y = targetPos.Y - this.EyePosition.Y;
      dir.Z = targetPos.Z - this.EyePosition.Z;
      dir.Normalize();
      HitTest hitTest = this.instance.CalcBlockTarget(this.EyePosition, dir, distance, (List<byte>) null, false, true, false, false);
      if (hitTest.IsValid)
        return (double) hitTest.Distance > (double) distance;
      return true;
    }

    protected virtual NpcSpawnBlock GetSpawnBlock()
    {
      return (NpcSpawnBlock) null;
    }

    public void LoadBehaviour(BehaviourTreeType type, string name)
    {
      if (!name.IsNotEmpty())
        return;
      if (type == BehaviourTreeType.AI)
      {
        this.LoadBehaviourCore(name);
      }
      else
      {
        if (type != BehaviourTreeType.Dialog)
          return;
        this.LoadDialogCore(name);
      }
    }

    protected virtual void LoadBehaviourCore(string name)
    {
    }

    protected virtual void LoadDialogCore(string name)
    {
    }

    public virtual void AddLog(string s)
    {
      Actor.Logs.Add(s);
      if (Actor.Logs.Count <= 42)
        return;
      Actor.Logs.RemoveAt(0);
    }

    public StudioForge.Engine.GamerServices.Gamer SignedInGamer
    {
      get
      {
        return (StudioForge.Engine.GamerServices.Gamer) this.Gamer;
      }
    }

    public BehaviourTree BehaviourTree
    {
      get
      {
        return this.behaviourTree;
      }
    }

    public BehaviourTree DialogTree
    {
      get
      {
        return this.dialogTree;
      }
    }

    public virtual WieldType WieldType
    {
      get
      {
        return WieldType.BothHands;
      }
    }

    public int Reach
    {
      get
      {
        return this.reach;
      }
      set
      {
        int max = this.instance.IsCreativeMode ? 16 : 8;
        if (value == 0)
          value = max;
        this.reach = MyMathHelper.Clamp(value, 1, max);
      }
    }

    public bool IsBobbing
    {
      get
      {
        if (this.FlyMode != FlyMode.None)
          return false;
        if (!this.isOnGround)
          return this.IsInWater;
        return true;
      }
    }

    public virtual bool IceEffectActive
    {
      get
      {
        return (double) this.FreezeTimer > 0.0;
      }
    }

    protected virtual bool IsUsingCustomLootTable
    {
      get
      {
        return false;
      }
    }

    public bool IsSkillsEnabled
    {
      get
      {
        return this.instance.IsSkillsEnabled;
      }
    }

    public int HealthLevel(bool addBonuses)
    {
      int num = this.HealthLevelCore(addBonuses);
      if (addBonuses)
        num += this.HealthTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int HealthLevelCore(bool addBonuses)
    {
      return this.LevelData.HealthLevel;
    }

    public int AttackLevel(bool addBonuses)
    {
      int num = this.AttackLevelCore(addBonuses);
      if (addBonuses)
        num += this.AttackTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int AttackLevelCore(bool addBonuses)
    {
      return this.LevelData.AttackLevel;
    }

    public int StrengthLevel(bool addBonuses)
    {
      int num = this.StrengthLevelCore(addBonuses);
      if (addBonuses)
        num += this.StrengthTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int StrengthLevelCore(bool addBonuses)
    {
      return this.LevelData.StrengthLevel;
    }

    public int DefenceLevel(bool addBonuses)
    {
      int num = this.DefenceLevelCore(addBonuses);
      if (addBonuses)
        num += this.DefenceTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int DefenceLevelCore(bool addBonuses)
    {
      return this.LevelData.DefenceLevel;
    }

    public int RangedLevel(bool addBonuses)
    {
      int num = this.RangedLevelCore(addBonuses);
      if (addBonuses)
        num += this.RangedTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int RangedLevelCore(bool addBonuses)
    {
      return this.LevelData.RangedLevel;
    }

    public int MagicLevel(bool addBonuses)
    {
      int num = this.MagicLevelCore(addBonuses);
      if (addBonuses)
        num += this.MagicTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int MagicLevelCore(bool addBonuses)
    {
      return this.LevelData.MagicLevel;
    }

    public int LootingLevel(bool addBonuses)
    {
      int num = this.LootingLevelCore(addBonuses);
      if (addBonuses)
        num += this.LootingTotalItemBonus() / 100;
      if (num <= 0)
        return 1;
      return num;
    }

    protected virtual int LootingLevelCore(bool addBonuses)
    {
      return 1;
    }

    public int GetItemEquippedSlot(Item itemID)
    {
      EquipIndex itemEquipIndex = ItemData.GetItemEquipIndex(itemID);
      int equipSlotId = this.Inventory.GetEquipSlotID(itemEquipIndex);
      if (itemEquipIndex == EquipIndex.LeftHand || itemEquipIndex == EquipIndex.RightHand)
      {
        if (itemEquipIndex == EquipIndex.LeftHand && this.Inventory[equipSlotId].ItemID != itemID)
          equipSlotId = this.Inventory.GetEquipSlotID(EquipIndex.RightHand);
        else if (itemEquipIndex == EquipIndex.RightHand && this.Inventory[equipSlotId].ItemID != itemID)
          equipSlotId = this.Inventory.GetEquipSlotID(EquipIndex.LeftHand);
      }
      return equipSlotId;
    }

    public bool IsItemEquipped(Item itemID)
    {
      return this.Inventory[this.GetItemEquippedSlot(itemID)].ItemID == itemID;
    }

    public bool IsItemEquippedAndUsable(Item itemID)
    {
      if (this.IsItemEquipped(itemID))
        return this.CanUseItem(itemID);
      return false;
    }

    public int GetItemEquipSlotID(Item itemID)
    {
      return this.Inventory.GetEquipSlotID(itemID);
    }

    private ItemCombatDataXML GetCombatData(Item itemID)
    {
      ItemTypeDataXML itemTypeDataXml = Globals1.ItemTypeData[(int) itemID];
      return Globals1.ItemCombatData[(int) itemTypeDataXml.Combat];
    }

    private ItemCombatDataXML GetUsuableCombatData(Item itemID)
    {
      if (!this.CanUseItem(itemID))
        return Globals1.ItemCombatData[0];
      return this.GetCombatData(itemID);
    }

    private ItemCombatDataXML GetUsuableCombatData(int slotID)
    {
      Item itemID = this.Inventory[slotID].ItemID;
      if (slotID < (int) this.Inventory.EquipIndexStart)
      {
        switch (Globals1.ItemTypeData[(int) itemID].Equip)
        {
          case EquipIndex.LeftHand:
          case EquipIndex.RightHand:
            if (slotID != this.Inventory.HotBarLeftSlotID && slotID != this.Inventory.HotBarRightSlotID)
            {
              itemID = Item.None;
              break;
            }
            break;
        }
      }
      return this.GetUsuableCombatData(itemID);
    }

    public int HealthTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Health;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Health;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Health;
      return num;
    }

    public int StrengthTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Strength;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Strength;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Strength;
      return num;
    }

    public int AttackTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Attack;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Attack;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Attack;
      return num;
    }

    public int DefenceTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Defence;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Defence;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Defence;
      return num;
    }

    public int RangedTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Ranged;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Ranged;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Ranged;
      return num;
    }

    public int MagicTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Magic;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Magic;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Magic;
      return num;
    }

    public int LootingTotalItemBonus()
    {
      int num = 0;
      for (int equipIndexStart = (int) this.Inventory.EquipIndexStart; equipIndexStart < (int) this.Inventory.EquipIndexEnd; ++equipIndexStart)
        num += (int) this.GetUsuableCombatData(equipIndexStart).Looting;
      if (this.LeftHand != null)
        num += (int) this.GetUsuableCombatData(this.LeftHand.ItemID).Looting;
      if (this.RightHand != null)
        num += (int) this.GetUsuableCombatData(this.RightHand.ItemID).Looting;
      return num;
    }

    public int CombatLevel
    {
      get
      {
        return SkillData.CombatLevel((float) this.HealthLevel(false), (float) this.StrengthLevel(false), (float) this.AttackLevel(false), (float) this.DefenceLevel(false), (float) this.RangedLevel(false));
      }
    }

    public abstract string CombatLevelString { get; }

    public virtual ActorTypeDataXML NpcTypeData
    {
      get
      {
        return Globals1.NpcTypeData[(int) this.ActorType];
      }
    }

    public virtual bool HasHistory(string key)
    {
      return false;
    }

    protected ActorLevelDataXML LevelData
    {
      get
      {
        return Globals1.NpcLevelData[(int) this.NpcTypeData.LevelType];
      }
    }

    protected ActorPhysicsDataXML PhysicsData
    {
      get
      {
        return Globals1.NpcPhysicsData[(int) this.NpcTypeData.PhysicsType];
      }
    }

    public virtual bool IsAdmin
    {
      get
      {
        return false;
      }
    }

    protected ActorAIDataXML AIData
    {
      get
      {
        return Globals1.NpcAIData[(int) this.NpcTypeData.AIType];
      }
    }

    public float RegardRange
    {
      get
      {
        return (float) this.AIData.RegardRange;
      }
    }

    public float HearingRange
    {
      get
      {
        return (float) this.AIData.HearingRange;
      }
    }

    public float AttackRange
    {
      get
      {
        return (float) this.AIData.AttackRange;
      }
    }

    public float StrikeRange
    {
      get
      {
        return this.AIData.StrikeRange;
      }
    }

    protected float StrikeDelay
    {
      get
      {
        return this.AIData.StrikeDelay;
      }
    }

    protected float InactiveRange
    {
      get
      {
        return (float) this.AIData.InactiveRange;
      }
    }

    public float Acceleration
    {
      get
      {
        return this.PhysicsData.Acceleration;
      }
    }

    public float RotateSpeed
    {
      get
      {
        return this.PhysicsData.RotateSpeed * this.RotationSpeedModifier;
      }
    }

    protected virtual float RotationSpeedModifier
    {
      get
      {
        float num = 1f;
        if (this.IceEffectActive)
          num *= 0.8f;
        return num;
      }
    }

    public float JumpSpeed
    {
      get
      {
        return this.PhysicsData.JumpSpeed;
      }
    }

    public float MoveSpeed
    {
      get
      {
        return this.PhysicsData.MoveSpeed;
      }
    }

    public float MaxOxygen
    {
      get
      {
        return Math.Max(25f, this.MaxHealth * 0.25f);
      }
    }

    public virtual bool IsPlayer
    {
      get
      {
        return false;
      }
    }

    public GameInstance GameInstance
    {
      get
      {
        return this.instance;
      }
    }

    public bool IsInactive
    {
      get
      {
        return this.actorState == ActorState.InActive;
      }
    }

    public bool IsDying
    {
      get
      {
        return this.actorState == ActorState.Dying;
      }
    }

    public bool IsDeadOrInactiveOrDisabled
    {
      get
      {
        if (this.actorState != ActorState.Dying && this.actorState != ActorState.InActive)
          return !this.IsEnabledField;
        return true;
      }
    }

    public bool IsInactiveOrDisabled
    {
      get
      {
        if (this.actorState != ActorState.InActive)
          return !this.IsEnabledField;
        return true;
      }
    }

    public virtual bool IsCanTouchOnTestY(GlobalPoint3D p, Block blockID)
    {
      if (!this.IsEnabledField || this.actorState == ActorState.Dying || this.actorState == ActorState.Despawning)
        return false;
      switch (blockID)
      {
        case Block.SteelSpikes:
          if (this.IsPlayer)
            return !this.IsItemEquippedAndUsable(Item.SpiderRing);
          return true;
        case Block.ScriptBlock:
          ScriptBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(p) as ScriptBlock;
          if (dataBlock != null)
            return dataBlock.ActAsPressurePlate;
          return false;
        default:
          return true;
      }
    }

    public bool IsFlying
    {
      get
      {
        return this.FlyMode != FlyMode.None;
      }
    }

    public abstract GamerID GamerID { get; }

    public ActorState State
    {
      get
      {
        return this.actorState;
      }
    }

    public float MaxHealth
    {
      get
      {
        return SkillData.MaxHealth(this.HealthLevel(true));
      }
    }

    public bool IsFemale
    {
      get
      {
        return this.NpcTypeData.IsFemale;
      }
    }

    public float FarClip
    {
      get
      {
        return this.farClip;
      }
    }

    public float NearClip
    {
      get
      {
        return this.nearClip;
      }
      set
      {
        this.nearClip = value;
        this.ResetPerspectiveMatrix();
      }
    }

    public float FOV
    {
      get
      {
        return this.fov;
      }
      set
      {
        this.fov = value;
        this.ResetPerspectiveMatrix();
      }
    }

    protected void ResetPerspectiveMatrix()
    {
      this.ResetPerspectiveMatrix(this.fov, this.nearClip, this.farClip);
    }

    protected virtual void ResetPerspectiveMatrix(float fov, float nearClip, float farClip)
    {
      this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), 1f, nearClip, farClip);
    }

    public bool IsOnGround
    {
      get
      {
        return this.isOnGround;
      }
    }

    public bool IsOnRope
    {
      get
      {
        return this.isOnRope;
      }
    }

    protected virtual bool DisableCameraBackOffset
    {
      get
      {
        return false;
      }
    }

    public virtual string DisplayGamertag
    {
      get
      {
        if (this.Gamer == null || this.Gamer.Gamertag == null)
          return "Unknown";
        return this.Gamer.Gamertag;
      }
    }

    public virtual bool IsLocalGamer
    {
      get
      {
        if (this.Gamer != null)
          return this.Gamer.IsLocal;
        return true;
      }
    }

    public bool IsCrouching
    {
      get
      {
        return (double) this.Size.Y < (double) this.fullHeight - ((double) this.fullHeight - (double) this.crouchHeight) * 0.5;
      }
    }

    private bool IsInsideSolidBlock(Vector3 pos, ItemSubType ignoreSubTypes)
    {
      byte blockId = this.map.GetBlockID(this.map.GetPoint(pos));
      if (!this.map.IsBlockPassable(blockId))
        return !ItemData.IsSubTypeAny((Item) blockId, ignoreSubTypes);
      return false;
    }

    public byte GetCurrentBlockAux(Block blockID)
    {
      if (this.IsBlockUseCurrentAux(blockID))
        return (byte) Math.Min((int) this.currentBlockAux, this.Inventory.ItemCount((Item) blockID) - 1);
      return 0;
    }

    public void SetCurrentBlockAux(Block blockID, byte aux)
    {
      if (!this.IsBlockUseCurrentAux(blockID))
        return;
      this.currentBlockAux = aux;
    }

    private bool IsBlockUseCurrentAux(Block blockID)
    {
      switch (blockID)
      {
        case Block.Stack:
        case Block.UpsideDownStack:
        case Block.SnowLayer:
        case Block.Stack2:
          return true;
        default:
          return false;
      }
    }

    protected virtual float LandingDamageMultiplier
    {
      get
      {
        if (!this.Properties.LandingDamageMultiplier.HasValue)
          return 1f;
        return this.Properties.LandingDamageMultiplier.Value;
      }
    }

    protected Actor(GameInstance instance, MapTM map, NetworkGamer gamer, ActorType mobType)
    {
      this.instance = instance;
      this.map = map;
      this.Gamer = gamer;
      this.ActorType = mobType;
      this.Alpha = 1f;
      this.IsDeveloper = this.IsTester = this.IsTesterman = this.IsGod = this.IsGodOrTester = false;
      this.Properties.CanPickup = new bool?(false);
      this.Properties.CanFight = new bool?(false);
      this.Properties.EquipBody = new bool?(true);
      this.Properties.Reach = new int?(0);
      this.Properties.ShowNamePlate = new bool?(true);
      this.Properties.ShowSwingTarget = new bool?(false);
    }

    protected override void InitializeCore(InitState state)
    {
      this.age = 0.0f;
      this.actorState = ActorState.Alive;
      this.avatarAnim = new ActorAnim();
      this.Frustum = new BoundingFrustum(Matrix.Identity);
      this.Spheres = new List<BoundingSphere>();
      this.ViewDirection = Vector3.Forward;
      this.AudioEmitter = new AudioEmitter();
      this.AudioEmitter.Up = Vector3.Up;
      this.MaxVelocity = new Vector2(this.MoveSpeed, 2.6f);
      this.nearClip = 0.1f;
      this.farClip = this.GetFarClip();
      this.FOV = 50f;
      this.HoverHeight = 0.0f;
      this.Inventory = this.CreateInventory();
      this.LeftHand = new Hand(this, InventoryHand.Left);
      this.RightHand = new Hand(this, InventoryHand.Right);
      this.Health = this.MaxHealth;
      this.Oxygen = this.MaxOxygen;
    }

    protected virtual EquipmentInventory CreateInventory()
    {
      return new EquipmentInventory(10, 7, 0);
    }

    protected override void LoadContentCore(InitState state)
    {
      this.UpdateMatrices();
      base.LoadContentCore(state);
    }

    protected virtual float GetFarClip()
    {
      return this.RegardRange;
    }

    public Hand GetHand(InventoryHand handType)
    {
      if (handType == InventoryHand.Left)
        return this.LeftHand;
      if (handType != InventoryHand.Right)
        return (Hand) null;
      return this.RightHand;
    }

    public virtual void SetSize(GlobalPoint3D modelSize, float scale)
    {
      this.Size = new Vector2((float) ((double) Math.Max(modelSize.X, modelSize.Z) * (double) scale * 0.800000011920929), (float) modelSize.Y * scale + this.HoverHeight);
      this.EyeOffset = this.NpcTypeData.EyeOffset;
      if ((double) this.EyeOffset.X == 0.0 && (double) this.EyeOffset.Y == 0.0 && (double) this.EyeOffset.Z == 0.0)
        this.EyeOffset.Y = this.Size.Y * 0.9f;
      this.fullHeight = this.Size.Y;
      this.crouchHeight = this.fullHeight * 0.5f;
      this.fullEyeHeight = this.EyeOffset.Y;
      if (this.IsCrouching)
        this.EyeOffset.Y = this.Size.Y = this.crouchHeight;
      this.DrawScale = scale;
    }

    public virtual void NpcSpawn(Vector3 pos, GamerID npcID, Script killScript)
    {
      this.age = 0.0f;
      this.avatarAnim.CurrentFrame = 0;
      this.Health = this.MaxHealth;
      this.Position = this.lastPosition = pos;
      this.Velocity = Vector3.Zero;
      if (this.random == null)
        this.random = new PcgRandom(this.Seed = (int) npcID.ID);
      else
        this.random.Seed((int) npcID.ID);
      this.EyePosition = this.Position + this.EyeOffset;
      this.UpdateBounds();
      this.ChangeState(ActorState.Alive);
      this.IsEnabledField = true;
    }

    protected bool RandomChanceTime(double seconds)
    {
      return this.random.Next((int) (seconds * 60.0)) == 0;
    }

    protected bool RandomChance(double chance)
    {
      return this.random.NextDouble() <= chance;
    }

    protected override void UpdateCore(UpdateState state)
    {
      this.playPainSoundTimer -= Services.ElapsedTime;
      this.DisplayPleaseWaitMessage = false;
      if (this.IsViewLoading || this.IsInactive)
        return;
      this.UpdateState();
      if (this.ShouldUpdatePhysics(this.Box))
      {
        if (this.State != ActorState.Despawning)
          this.UpdatePhysics();
      }
      else
        this.DisplayPleaseWaitMessage = true;
      this.UpdateGeneral();
      this.UpdateMatrices();
    }

    public bool ShouldUpdatePhysics(BoundingBox box)
    {
      if (!this.IsGodOrTester)
      {
        Vector3 min = box.Min;
        Vector3 max = box.Max;
        Vector3 pos = min;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.X = max.X;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.Z = max.Z;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.X = min.X;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.Y += this.EyeOffset.Y;
        pos.Z = min.Z;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.X = max.X;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.Z = max.Z;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
        pos.X = min.X;
        if (!this.IsChunkOkToOccupy(pos))
          return false;
      }
      return true;
    }

    private bool IsChunkOkToOccupy(Vector3 pos)
    {
      MapChunk chunk = this.map.GetChunk(pos);
      if (chunk != null)
        return chunk.IsDecorated;
      return true;
    }

    protected virtual void UpdateState()
    {
      switch (this.actorState)
      {
        case ActorState.Dying:
          this.UpdateDying();
          break;
        case ActorState.Respawning:
          this.UpdateRespawning();
          break;
        case ActorState.Despawning:
          this.UpdateDespawning();
          break;
        case ActorState.Custom:
          this.UpdateCustomState();
          break;
      }
    }

    public virtual bool ChangeState(ActorState newState)
    {
      if (this.actorState == newState)
        return false;
      switch (this.actorState)
      {
        case ActorState.InActive:
          return false;
        case ActorState.Dying:
          if (newState != ActorState.Respawning && newState != ActorState.InActive)
            return false;
          break;
        case ActorState.Despawning:
          if (newState != ActorState.Dying && newState != ActorState.InActive)
            return false;
          break;
      }
      bool flag = newState == ActorState.Sleeping || this.actorState == ActorState.Sleeping;
      this.actorState = newState;
      if (flag)
        this.instance.RecalcAllPlayersSleeping();
      switch (newState)
      {
        case ActorState.InActive:
          TargetingSystem.TargetInactive((INPCBehaviour) this);
          this.ClearPressurePoints();
          break;
        case ActorState.Dying:
          TargetingSystem.TargetInactive((INPCBehaviour) this);
          this.ClearPressurePoints();
          this.PlayDeathSound();
          this.ExplodeModel();
          break;
        case ActorState.Respawning:
          this.Velocity.X = this.Velocity.Y = this.Velocity.Z = 0.0f;
          this.positionInterpolator.Reset();
          this.ClearPressurePoints();
          break;
        case ActorState.Despawning:
          this.ClearPressurePoints();
          break;
      }
      return true;
    }

    protected virtual void UpdatePhysics()
    {
      this.CheckForTeleport();
      float gravity = this.Gravity;
      Vector3 position = this.Position;
      position.Y -= gravity;
      GlobalPoint3D point1 = this.map.GetPoint(position);
      GlobalPoint3D point2 = this.map.GetPoint(this.Position);
      GlobalPoint3D point3 = this.map.GetPoint(new Vector3(this.Position.X, (float) (((double) this.EyePosition.Y - (double) this.Position.Y) * 0.5) + this.Position.Y, this.Position.Z));
      GlobalPoint3D point4 = this.map.GetPoint(this.EyePosition);
      Block blockId = (Block) this.map.GetBlockID(point2);
      Block midBlockID = point3 != point2 ? (Block) this.map.GetBlockID(point3) : blockId;
      Block eyeBlockID = point4 != point3 ? (Block) this.map.GetBlockID(point4) : midBlockID;
      switch (eyeBlockID)
      {
        case Block.Water:
        case Block.Lava:
          float num1 = 0.0001f;
          if ((double) this.Velocity.X > -(double) num1 && (double) this.Velocity.X < (double) num1)
            this.Velocity.X = 0.0f;
          if ((double) this.Velocity.Z > -(double) num1 && (double) this.Velocity.Z < (double) num1)
            this.Velocity.Z = 0.0f;
          this.IsInWater = blockId == Block.Water || midBlockID == Block.Water || eyeBlockID == Block.Water;
          bool flag1 = blockId == Block.Lava || midBlockID == Block.Lava || eyeBlockID == Block.Lava;
          bool flag2 = blockId == Block.Fire || midBlockID == Block.Fire || eyeBlockID == Block.Fire;
          this.isOnRope = blockId == Block.Rope || midBlockID == Block.Rope || eyeBlockID == Block.Rope;
          this.isOnLadder = blockId == Block.Ladder || midBlockID == Block.Ladder || eyeBlockID == Block.Ladder;
          if (!this.isOnLadder)
            this.isOnLadder = blockId == Block.Scaffold || midBlockID == Block.Scaffold || eyeBlockID == Block.Scaffold;
          if (!this.isOnLadder)
            this.isOnLadder = blockId == Block.ClimbingIvy || midBlockID == Block.ClimbingIvy || eyeBlockID == Block.ClimbingIvy;
          if ((double) this.KnockForce.X != 0.0)
          {
            this.KnockForce.X *= 0.95f;
            if ((double) Math.Abs(this.KnockForce.X) < 0.0500000007450581)
              this.KnockForce.X = 0.0f;
          }
          if ((double) this.KnockForce.Z != 0.0)
          {
            this.KnockForce.Z *= 0.95f;
            if ((double) Math.Abs(this.KnockForce.Z) < 0.0500000007450581)
              this.KnockForce.Z = 0.0f;
          }
          float speedModifier = this.SpeedModifier;
          if (this.IsInWater && this.FlyMode == FlyMode.None)
            gravity *= 0.2f;
          this.IsUnderWater = this.IsUnderLava = false;
          this.duckHead = 0.0f;
          switch (eyeBlockID)
          {
            case Block.Water:
              this.IsUnderWater = this.IsUnderLiquid(Block.Water);
              break;
            case Block.Lava:
              this.IsUnderLava = this.IsUnderLiquid(Block.Lava);
              break;
          }
          this.Velocity.Y += gravity;
          if (this.isMovingTo)
            this.MoveToCore(this.moveToPos, speedModifier, this.moveToCanJump, this.moveToType);
          this.UpdateControlPhysics(point1, blockId, midBlockID, eyeBlockID, speedModifier);
          if (this.positionInterpolator.IsActive)
          {
            this.positionInterpolator.Update();
            this.Velocity = this.positionInterpolator.CurrentValue - this.Position;
          }
          this.IsHittingBlockOnXZ = false;
          this.HittingBlockOnXZ.Y = 0;
          bool skipCollision = this.SkipCollision;
          if (!skipCollision)
          {
            if (this.lastSkipCollision)
              this.Velocity = Vector3.Zero;
            this.lastVelocity.X = this.Velocity.X;
            this.lastVelocity.Y = this.Velocity.Y;
            this.lastVelocity.Z = this.Velocity.Z;
            this.CheckWorldCollision();
            this.IsHittingBlockOnXZ = (double) this.lastVelocity.X != 0.0 && (double) this.Velocity.X == 0.0 || (double) this.lastVelocity.Z != 0.0 && (double) this.Velocity.Z == 0.0;
          }
          else
            this.ClampToWorldBounds();
          float y = this.MaxVelocity.Y;
          if ((double) this.Velocity.Y < -(double) y)
            this.Velocity.Y = -y;
          else if ((double) this.Velocity.Y > (double) y)
            this.Velocity.Y = y;
          this.ClampVelocityHoriz(speedModifier);
          this.lastSkipCollision = skipCollision;
          this.lastPosition = this.Position;
          this.Position.X += this.Velocity.X;
          this.Position.Y += this.Velocity.Y;
          this.Position.Z += this.Velocity.Z;
          this.isOnGround = (double) this.lastVelocity.Y < 0.0 && (double) this.Velocity.Y == 0.0;
          if (this.IsPlayer)
          {
            float num2 = this.Size.X * this.halfSizeFactor;
            if (!this.ShouldUpdatePhysics(new BoundingBox()
            {
              Min = this.Position - new Vector3(num2, -0.01f, num2),
              Max = this.Position + new Vector3(num2, this.Size.Y, num2)
            }))
            {
              this.Position = this.lastPosition;
              this.DisplayPleaseWaitMessage = true;
            }
          }
          this.ClampPositionToMapBound();
          if (this.IsPlayer || !this.NpcTypeData.IsImmuneToFire)
          {
            if (flag1)
            {
              double damageAndDisplay1 = (double) this.TakeDamageAndDisplay(DamageType.Burning, 0.4f, Vector3.Zero);
            }
            else if (flag2)
            {
              double damageAndDisplay2 = (double) this.TakeDamageAndDisplay(DamageType.Burning, 0.05f, Vector3.Zero);
            }
          }
          if (!this.NpcTypeData.CanBreatheUnderWater)
          {
            if (this.IsUnderWater && !this.IsItemEquippedAndUsable(Item.WaterTalisman))
              this.Oxygen -= Services.ElapsedTime;
            else
              this.Oxygen = this.MaxOxygen;
            if ((double) this.Oxygen <= 0.0)
            {
              double damageAndDisplay3 = (double) this.TakeDamageAndDisplay(DamageType.Drowning, 0.1f, Vector3.Zero);
            }
          }
          this.VisualVelocity = this.Velocity;
          break;
        default:
          GlobalPoint3D point5 = this.map.GetPoint(new Vector3(this.Position.X, this.EyePosition.Y, this.Position.Z));
          if (point5.X != point4.X || point5.Z != point4.Z)
          {
            point4.X = point5.X;
            point4.Z = point5.Z;
            eyeBlockID = point4 != point3 ? (Block) this.map.GetBlockID(point4) : midBlockID;
            goto case Block.Water;
          }
          else
            goto case Block.Water;
      }
    }

    protected virtual void UpdateControlPhysics(
      GlobalPoint3D underFootPoint,
      Block footBlockID,
      Block midBlockID,
      Block eyeBlockID,
      float speedModifier)
    {
    }

    protected virtual float Gravity
    {
      get
      {
        float num = this.Properties.GravityMultiplier.HasValue ? this.Properties.GravityMultiplier.Value : 1f;
        return GameInstance.Gravity * this.GravityMultiplier * num;
      }
    }

    protected virtual bool SkipCollision
    {
      get
      {
        return false;
      }
    }

    protected virtual float SpeedModifier
    {
      get
      {
        float num = 1f;
        if (this.isOnRope)
          num = 2f;
        else if (this.FlyMode == FlyMode.Slow)
          num = 1.75f;
        else if (this.FlyMode == FlyMode.Fast)
          num = 7.5f;
        else if (this.IsCrouching)
          num = 0.6f;
        if (!this.SkipCollision)
          num *= this.SpeedMultiplier;
        if (this.IceEffectActive)
          num *= 0.1f;
        if (this.isMovingTo)
          num *= this.moveToVelMod;
        return num;
      }
    }

    protected virtual void PlayFootStepSound(GlobalPoint3D p, Block blockUnderFoot)
    {
      Sounds.PlaySound((Item) this.map.GetBlockTextureIDForDrawing(blockUnderFoot, p), ItemSoundType.Step, (ITMActor) this, !this.IsCrouching);
    }

    protected virtual void DepleteItemDurabilityForUsage()
    {
      if (this.IsItemEquippedAndUsable(Item.TenLeagueBoots))
        this.DepleteItemDurabilityForUsage(Item.TenLeagueBoots);
      if (this.IsItemEquippedAndUsable(Item.SpiderRing))
        this.DepleteItemDurabilityForUsage(Item.SpiderRing);
      if (this.IsItemEquippedAndUsable(Item.WaterTalisman) && this.IsUnderWater)
        this.DepleteItemDurabilityForUsage(Item.WaterTalisman);
      Item itemId = this.Inventory.Neck.ItemID;
      if (itemId == Item.None || !this.CanUseItem(itemId))
        return;
      switch (itemId)
      {
        case Item.AmuletOfFlight:
          if (this.FlyMode == FlyMode.None)
            break;
          goto case Item.PredatorAmulet;
        case Item.PredatorAmulet:
        case Item.AmuletOfStarlight:
        case Item.NecklaceOfFarsight:
          this.DepleteItemDurabilityForUsage(this.Inventory.NeckIndex);
          break;
      }
    }

    protected int DepleteItemDurabilityForUsage(Item itemID)
    {
      EquipIndex itemEquipIndex = ItemData.GetItemEquipIndex(itemID);
      int equipSlotId = this.Inventory.GetEquipSlotID(itemEquipIndex);
      if (itemEquipIndex != EquipIndex.LeftHand && itemEquipIndex != EquipIndex.RightHand)
        return this.DepleteItemDurabilityForUsage(equipSlotId);
      if (itemEquipIndex == EquipIndex.LeftHand && this.Inventory[equipSlotId].ItemID != itemID)
        equipSlotId = this.Inventory.GetEquipSlotID(EquipIndex.RightHand);
      else if (itemEquipIndex == EquipIndex.RightHand && this.Inventory[equipSlotId].ItemID != itemID)
        equipSlotId = this.Inventory.GetEquipSlotID(EquipIndex.LeftHand);
      if (this.Inventory[equipSlotId].ItemID == itemID)
        return this.DepleteItemDurabilityForUsage(equipSlotId);
      return 0;
    }

    protected int DepleteItemDurabilityForUsage(int slotID)
    {
      if (slotID < 0)
        return 0;
      InventoryItem inventoryItem = this.Inventory[slotID];
      if (inventoryItem.Durability < (ushort) 2)
      {
        if (this.IsPlayer && this.IsLocalGamer)
          this.instance.AddNotification("Your " + ItemData.ToString(inventoryItem.ItemID) + " has degraded", NotifyRecipient.Local);
        inventoryItem.Count = 0;
        inventoryItem.ItemID = Item.None;
        inventoryItem.Durability = (ushort) 0;
      }
      else
        --inventoryItem.Durability;
      this.Inventory[slotID] = inventoryItem;
      return (int) inventoryItem.Durability;
    }

    protected virtual void UpdateGeneral()
    {
      this.age += Services.ElapsedTime;
      if (this.effectManager != null)
        this.effectManager.Update();
      if ((double) this.Alpha < 1.0)
        this.Alpha += 0.01f;
      else
        this.Alpha = 1f;
      --this.LandingSoundDelay;
      this.secondTimer += Services.ElapsedTime;
      if ((double) this.secondTimer > 1.0)
      {
        --this.secondTimer;
        this.UpdateItemEffects();
      }
      if ((double) this.FreezeTimer > 0.0)
        this.FreezeTimer -= Services.ElapsedTime;
      else
        this.FreezeTimer = 0.0f;
      if ((double) this.FreezeImmunityTimer > 0.0)
        this.FreezeImmunityTimer -= Services.ElapsedTime;
      float maxHealth = this.MaxHealth;
      if ((double) this.Health > (double) maxHealth)
        this.Health = maxHealth;
      if (this.useCustomWalkSoundLogic || !this.isOnGround)
        return;
      if ((double) new Vector2()
      {
        X = (this.Position.X - this.lastWalkSoundPosition.X),
        Y = (this.Position.Z - this.lastWalkSoundPosition.Z)
      }.LengthSquared() <= 4.0)
        return;
      this.lastWalkSoundPosition = this.Position;
      this.PlayWalkSound();
    }

    protected virtual void UpdateItemEffects()
    {
      if (this.instance.IsFiniteResources)
        this.DepleteItemDurabilityForUsage();
      if (!this.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
        return;
      double damageAndDisplay = (double) this.TakeDamageAndDisplay(DamageType.ItemUse, 5f, Vector3.Zero);
    }

    public void UpdateBounds()
    {
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) this.ActorType];
      bool flag = !this.IsCrouching;
      Vector2 vector2_1 = flag ? actorTypeDataXml.BoxScale : actorTypeDataXml.BoxScaleCrouch;
      float num1 = flag ? actorTypeDataXml.BoxOffset : actorTypeDataXml.BoxOffsetCrouch;
      vector2_1.Y *= 2f;
      float num2 = num1 * 2f;
      this.BodyBox.Min.X = this.Position.X - vector2_1.X;
      this.BodyBox.Min.Y = this.Position.Y + 0.01f + num2;
      this.BodyBox.Min.Z = this.Position.Z - vector2_1.X;
      this.BodyBox.Max.X = this.Position.X + vector2_1.X;
      this.BodyBox.Max.Y = this.BodyBox.Min.Y + vector2_1.Y;
      this.BodyBox.Max.Z = this.Position.Z + vector2_1.X;
      Vector2 vector2_2 = flag ? actorTypeDataXml.CriticalBoxScale : actorTypeDataXml.CriticalBoxScaleCrouch;
      float num3 = flag ? actorTypeDataXml.CriticalBoxOffset : actorTypeDataXml.CriticalBoxOffsetCrouch;
      vector2_2.Y *= 2f;
      float num4 = num3 * 2f;
      this.CriticalHitBox.Min.X = this.Position.X - vector2_2.X;
      this.CriticalHitBox.Min.Y = this.Position.Y + 0.01f + num4;
      this.CriticalHitBox.Min.Z = this.Position.Z - vector2_2.X;
      this.CriticalHitBox.Max.X = this.Position.X + vector2_2.X;
      this.CriticalHitBox.Max.Y = this.CriticalHitBox.Min.Y + vector2_2.Y;
      this.CriticalHitBox.Max.Z = this.Position.Z + vector2_2.X;
      BoundingBox.CreateMerged(ref this.BodyBox, ref this.CriticalHitBox, out this.Box);
      Vector3 center = this.Sphere.Center;
      this.Sphere.Center = this.Position;
      this.Sphere.Center.Y += this.Size.Y * 0.5f;
      this.Sphere.Radius = this.Size.Y * 0.3f;
      Vector3 vector3 = this.Sphere.Center - center;
    }

    protected void UpdateMatrices()
    {
      this.UpdateBounds();
      this.viewMatrixPosition.X = this.Position.X + this.EyeOffset.X;
      this.viewMatrixPosition.Y = this.Position.Y + this.EyeOffset.Y;
      this.viewMatrixPosition.Z = this.Position.Z + this.EyeOffset.Z;
      if (!this.DisableCameraBackOffset)
      {
        this.viewMatrixPosition.X -= this.ViewDirection.X * 0.29f;
        this.viewMatrixPosition.Z -= this.ViewDirection.Z * 0.29f;
      }
      this.viewMatrixPosition.Y -= 0.1f;
      this.ViewDirNoYNormalized.X = this.ViewDirection.X;
      this.ViewDirNoYNormalized.Z = this.ViewDirection.Z;
      this.ViewDirNoYNormalized.Normalize();
      float num = this.nearClip * 1.5f;
      this.EyePosition.X = this.viewMatrixPosition.X + this.ViewDirNoYNormalized.X * num;
      this.EyePosition.Y = this.viewMatrixPosition.Y;
      this.EyePosition.Z = this.viewMatrixPosition.Z + this.ViewDirNoYNormalized.Z * num;
      if ((double) this.duckHead != 0.0)
        this.viewMatrixPosition.Y = this.duckHead;
      this.ViewMatrix = Matrix.CreateLookAt(this.viewMatrixPosition, this.viewMatrixPosition + this.ViewDirection, Vector3.Up);
      Vector3 cameraPosition = this.viewMatrixPosition - this.Position;
      this.ViewMatrixLocal = Matrix.CreateLookAt(cameraPosition, cameraPosition + this.ViewDirection, Vector3.Up);
      this.Frustum.Matrix = this.ViewMatrix * this.ProjectionMatrix;
      this.AudioEmitter.Position = this.EyePosition;
      this.AudioEmitter.Forward = this.ViewDirection;
      this.AudioEmitter.Velocity = this.Velocity;
    }

    protected virtual void UpdateDying()
    {
    }

    protected virtual void UpdateRespawning()
    {
    }

    protected virtual void UpdateDespawning()
    {
      this.ChangeState(ActorState.InActive);
    }

    protected virtual void UpdateCustomState()
    {
    }

    private void CheckForTeleport()
    {
      GlobalPoint3D point = this.map.GetPoint(this.EyePosition);
      if (this.map.GetBlockID(point) == (byte) 53)
      {
        --point.Y;
        if (this.map.GetBlockID(point) == (byte) 53)
          --point.Y;
        if (this.map.GetBlockID(point) != (byte) 31)
          return;
        this.insideTeleportTimer += Services.ElapsedTime;
        if ((double) this.insideTeleportTimer >= 2.0)
        {
          GlobalPoint3D randomTeleport = this.instance.MapStrategyTM.GetRandomTeleport(this as Player, point);
          if (randomTeleport != point)
          {
            Vector3 blockCenter = this.map.GetBlockCenter(randomTeleport);
            blockCenter.Y += this.map.TileSize * 0.5f;
            this.TeleportTo(blockCenter);
          }
          this.insideTeleportTimer = 0.0f;
          this.teleportSoundStarted = false;
        }
        else
        {
          if ((double) this.insideTeleportTimer < 1.0 || this.teleportSoundStarted)
            return;
          this.teleportSoundStarted = true;
          Sounds.PlaySound(Item.Teleport, ItemSoundType.Use, point, (ITMActor) this);
        }
      }
      else
      {
        this.insideTeleportTimer = 0.0f;
        this.teleportSoundStarted = false;
      }
    }

    protected virtual void ClampPositionToMapBound()
    {
      if ((double) this.Position.X < (double) this.map.MapBound.Min.X * (double) this.map.TileSize)
        this.Position.X = (float) this.map.MapBound.Min.X * this.map.TileSize;
      else if ((double) this.Position.X >= (double) this.map.MapBound.Max.X * (double) this.map.TileSize)
        this.Position.X = (float) ((double) this.map.MapBound.Max.X * (double) this.map.TileSize - 1.0 / 1000.0);
      if ((double) this.Position.Y < (double) this.map.MapBound.Min.Y * (double) this.map.TileSize)
        this.Position.Y = (float) this.map.MapBound.Min.Y * this.map.TileSize;
      else if ((double) this.EyePosition.Y >= (double) this.map.MapBound.Max.Y * (double) this.map.TileSize)
        this.Position.Y = (float) ((double) this.map.MapBound.Max.Y * (double) this.map.TileSize - 1.0 / 1000.0) - this.EyeOffset.Y;
      if ((double) this.Position.Z < (double) this.map.MapBound.Min.Z * (double) this.map.TileSize)
      {
        this.Position.Z = (float) this.map.MapBound.Min.Z * this.map.TileSize;
      }
      else
      {
        if ((double) this.Position.Z < (double) this.map.MapBound.Max.Z * (double) this.map.TileSize)
          return;
        this.Position.Z = (float) ((double) this.map.MapBound.Max.Z * (double) this.map.TileSize - 1.0 / 1000.0);
      }
    }

    protected void ClampVelocityHoriz(float speedModifier)
    {
      float num1 = this.MaxVelocity.X * speedModifier;
      float num2 = new Vector2(this.Velocity.X, this.Velocity.Z).Length();
      if ((double) num2 <= (double) num1)
        return;
      float num3 = num1 / num2;
      this.Velocity.X *= num3;
      this.Velocity.Z *= num3;
    }

    protected void Crouch(float crouchSpeed)
    {
      if ((double) this.Size.Y > (double) this.crouchHeight)
        this.Size.Y -= crouchSpeed * Services.ElapsedTime;
      else
        this.Size.Y = this.crouchHeight;
      if ((double) this.EyeOffset.Y > (double) this.crouchHeight)
        this.EyeOffset.Y -= crouchSpeed * Services.ElapsedTime;
      else
        this.EyeOffset.Y = this.crouchHeight;
    }

    protected void Uncrouch()
    {
      if ((double) this.Size.Y < (double) this.fullHeight && this.ShouldUpdatePhysics(this.Box))
      {
        float num1 = this.Size.X * this.halfSizeFactor;
        if ((double) this.Size.Y < (double) this.map.TileSize)
        {
          Vector3 position = this.Position;
          position.Y += this.Size.Y;
          position.X -= num1;
          position.Z -= num1;
          MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(this.map.GetPoint(position));
          if (blockIdAndAux.BlockID == (byte) 171 && ((int) blockIdAndAux.AuxData & 4) == 0)
            return;
          position.X += num1;
          position.X += num1;
          blockIdAndAux = this.map.GetBlockIDAndAux(this.map.GetPoint(position));
          if (blockIdAndAux.BlockID == (byte) 171 && ((int) blockIdAndAux.AuxData & 4) == 0)
            return;
          position.Z += num1;
          position.Z += num1;
          blockIdAndAux = this.map.GetBlockIDAndAux(this.map.GetPoint(position));
          if (blockIdAndAux.BlockID == (byte) 171 && ((int) blockIdAndAux.AuxData & 4) == 0)
            return;
          position.X -= num1;
          position.X -= num1;
          blockIdAndAux = this.map.GetBlockIDAndAux(this.map.GetPoint(position));
          if (blockIdAndAux.BlockID == (byte) 171 && ((int) blockIdAndAux.AuxData & 4) == 0)
            return;
        }
        Vector3 position1 = this.Position;
        float num2 = 0.035f * this.RotationSpeedModifier;
        float num3 = (double) this.Size.Y >= (double) this.map.TileSize ? this.Size.Y + num2 : this.map.TileSize + num2;
        position1.Y += num3;
        if (!this.IsInsideSolidBlock(position1 + new Vector3(-num1, 0.0f, -num1), ItemSubType.Door) && !this.IsInsideSolidBlock(position1 + new Vector3(-num1, 0.0f, num1), ItemSubType.Door) && (!this.IsInsideSolidBlock(position1 + new Vector3(num1, 0.0f, num1), ItemSubType.Door) && !this.IsInsideSolidBlock(position1 + new Vector3(num1, 0.0f, -num1), ItemSubType.Door)))
        {
          this.Size.Y += num2;
        }
        else
        {
          while (this.IsInsideSolidBlock(position1 + new Vector3(-num1, 0.0f, -num1), ItemSubType.Door) || this.IsInsideSolidBlock(position1 + new Vector3(-num1, 0.0f, num1), ItemSubType.Door) || (this.IsInsideSolidBlock(position1 + new Vector3(num1, 0.0f, num1), ItemSubType.Door) || this.IsInsideSolidBlock(position1 + new Vector3(num1, 0.0f, -num1), ItemSubType.Door)))
          {
            position1.Y -= num2;
            if ((double) position1.Y < (double) this.Position.Y + (double) this.crouchHeight)
            {
              position1.Y = this.Position.Y + this.crouchHeight;
              break;
            }
          }
          this.Size.Y = position1.Y - this.Position.Y;
        }
      }
      this.UpdateEyeOffsetFromSize();
    }

    protected void UpdateEyeOffsetFromSize()
    {
      if ((double) this.Size.Y < (double) this.fullHeight)
      {
        this.EyeOffset.Y = this.Size.Y - MathHelper.Lerp(0.0f, this.fullHeight - this.fullEyeHeight, (float) (((double) this.Size.Y - (double) this.crouchHeight) / ((double) this.fullHeight - (double) this.crouchHeight)));
      }
      else
      {
        this.Size.Y = this.fullHeight;
        this.EyeOffset.Y = this.fullEyeHeight;
      }
    }

    protected void MoveToCore(Vector3 pos, float speedModifier, bool canJump, MoveType moveType)
    {
      switch (moveType)
      {
        case MoveType.Jump:
          this.MoveToCoreJump(pos, speedModifier);
          break;
        case MoveType.Fly:
          this.MoveToCoreFly(pos, speedModifier);
          break;
        default:
          this.MoveToCore(pos, speedModifier, canJump);
          break;
      }
    }

    protected void MoveToCore(Vector3 pos, float speedModifier, bool canJump)
    {
      this.moveDir = pos - this.Position;
      this.moveDir.Y = 0.0f;
      if (this.IsHittingBlockOnXZ && this.HittingBlockOnXZ.Y > 0 && (this.isOnGround && canJump))
      {
        GlobalPoint3D hittingBlockOnXz = this.HittingBlockOnXZ;
        if (hittingBlockOnXz.Y == (int) Math.Round((double) this.Position.Y, MidpointRounding.AwayFromZero))
        {
          ++hittingBlockOnXz.Y;
          if (this.map.IsPassable(hittingBlockOnXz))
          {
            ++hittingBlockOnXz.Y;
            if (this.map.IsPassable(hittingBlockOnXz))
            {
              bool flag = this.map.GetBlockID(this.Position) == (byte) 11;
              this.Velocity.X = this.lastVelocity.X;
              this.Velocity.Y = this.JumpSpeed * 1.05f;
              this.Velocity.Z = this.lastVelocity.Z;
              if (flag)
              {
                this.Velocity.X *= 2f;
                this.Velocity.Z *= 2f;
              }
            }
          }
        }
        if ((double) this.Velocity.X != (double) this.lastVelocity.X || (double) this.Velocity.Z != (double) this.lastVelocity.Z)
          return;
      }
      if ((double) this.moveDir.X != 0.0 || (double) this.moveDir.Z != 0.0)
      {
        Vector3 vector3 = Vector3.Normalize(this.moveDir);
        float num = this.Acceleration * speedModifier;
        float val2_1 = vector3.X * num;
        float val2_2 = vector3.Z * num;
        this.Velocity.X = (double) this.moveDir.X < 0.0 ? Math.Max(this.moveDir.X, val2_1) : Math.Min(this.moveDir.X, val2_1);
        this.Velocity.Z = (double) this.moveDir.Z < 0.0 ? Math.Max(this.moveDir.Z, val2_2) : Math.Min(this.moveDir.Z, val2_2);
        this.Velocity.X += this.KnockForce.X;
        this.Velocity.Z += this.KnockForce.Z;
      }
      else
        this.isMovingTo = false;
    }

    protected void MoveToCoreFly(Vector3 pos, float speedModifier)
    {
      pos.Y = this.Position.Y;
      this.moveDir.X = pos.X - this.Position.X;
      this.moveDir.Y = 0.0f;
      this.moveDir.Z = pos.Z - this.Position.Z;
      float num = this.moveDir.Length();
      if ((double) this.lastMoveTarget.X != (double) pos.X || (double) this.lastMoveTarget.Z != (double) pos.Z)
      {
        this.lastMoveTarget = pos;
        this.lastMoveTargetFullDistance = num;
      }
      this.moveDir.Normalize();
      float acceleration = this.Acceleration;
      this.Velocity.X = this.moveDir.X * acceleration;
      this.Velocity.Z = this.moveDir.Z * acceleration;
      this.Velocity.Y = 0.01666667f;
      if ((double) num >= (double) this.lastMoveTargetFullDistance * 0.479999989271164)
        return;
      this.Velocity.Y = -0.01666667f;
    }

    protected void MoveToCoreJump(Vector3 pos, float speedModifier)
    {
      pos.Y = this.Position.Y;
      this.moveDir = pos - this.Position;
      if ((double) this.moveDir.Length() > (double) this.RegardRange)
      {
        this.ChangeState(ActorState.Alive);
      }
      else
      {
        this.moveDir.Y = 0.0f;
        double num = (double) this.moveDir.Length();
        this.moveDir.Normalize();
        this.moveDir.X *= this.Acceleration;
        this.moveDir.Z *= this.Acceleration;
        if (this.IsOnGround || !this.map.IsPassable(this.map.GetPoint(this.Position + new Vector3(0.0f, -0.1f, 0.0f))))
        {
          this.Velocity.X = 0.0f;
          this.Velocity.Z = 0.0f;
          if (this.IceEffectActive)
            return;
          this.jumpTimer -= Services.ElapsedTime;
          if ((double) this.jumpTimer > 0.0)
            return;
          this.Velocity.Y = (float) (this.random.NextDouble() * (double) this.JumpSpeed + 0.0500000007450581);
          this.jumpTimer = 1f;
        }
        else
        {
          this.Velocity.X = this.moveDir.X;
          this.Velocity.Z = this.moveDir.Z;
        }
      }
    }

    protected virtual Vector3 GetFinalLookAtPosition(CoordType type, Vector3 pos)
    {
      switch (type)
      {
        case CoordType.Absolute:
          return pos;
        case CoordType.PositionRelative:
          return this.Position + pos;
        case CoordType.VelocityRelative:
          Vector3 vector3_1 = (double) this.Velocity.X != 0.0 || (double) this.Velocity.Z != 0.0 ? this.Velocity : this.ViewDirection;
          vector3_1.Y = 0.0f;
          Vector3 vector3_2 = this.EyePosition + pos.X * Vector3.Normalize(vector3_1);
          vector3_2.Y += pos.Y;
          return vector3_2;
        case CoordType.ViewRelative:
          Vector3 vector3_3 = this.EyePosition + pos.X * Vector3.Normalize(this.ViewDirection);
          vector3_3.Y += pos.Y;
          if ((double) pos.Z != 0.0)
          {
            Vector3 right = this.ViewMatrix.Right;
            right.Y = 0.0f;
            right.Normalize();
            vector3_3.X += right.X * pos.Z;
            vector3_3.Z += right.Z * -pos.Z;
          }
          return vector3_3;
        case CoordType.TargetRelative:
          INPCBehaviour aiTarget1 = this.AITarget;
          if (aiTarget1 != null)
          {
            pos.X += aiTarget1.Position.X;
            pos.Z += aiTarget1.Position.Z;
            pos.Y += aiTarget1.Position.Y + (float) (((double) aiTarget1.EyePosition.Y - (double) aiTarget1.Position.Y) * 0.699999988079071);
            return pos;
          }
          break;
        case CoordType.TargetsTargetRelative:
          INPCBehaviour aiTarget2 = this.AITarget;
          if (aiTarget2 != null)
          {
            INPCBehaviour aiTarget3 = aiTarget2.AITarget;
            if (aiTarget3 != null)
            {
              pos.X += aiTarget3.Position.X;
              pos.Z += aiTarget3.Position.Z;
              pos.Y += aiTarget3.Position.Y + (float) (((double) aiTarget3.EyePosition.Y - (double) aiTarget3.Position.Y) * 0.699999988079071);
              return pos;
            }
            break;
          }
          break;
      }
      return pos + ((double) this.Velocity.X != 0.0 || (double) this.Velocity.Z != 0.0 ? this.EyePosition + Vector3.Normalize(this.Velocity) * (float) this.Reach : this.EyePosition + this.ViewDirection);
    }

    protected virtual void Die(DamageType deathType, Actor attacker, Item weaponID, float damage)
    {
      this.ChangeState(ActorState.Dying);
      if (!this.IsLocalGamer)
        return;
      bool flag = this.instance.IsFiniteResources && this.ShouldDropItemsOnDeath(deathType, attacker, weaponID);
      if (flag)
      {
        if (!this.Properties.DropInventoryOnDeath.HasValue || !this.Properties.DropInventoryOnDeath.Value || !this.instance.IsCreativeMode)
          this.Inventory.ClearItems();
        this.SetDeathDropItems(deathType, attacker);
      }
      if (attacker != null && attacker.IsPlayer && !attacker.IsLocalGamer)
        this.instance.NetworkManager.SendKillConfirm(deathType, this, (Actor) null, attacker, weaponID);
      if (this.IsPlayer || attacker != null && attacker.IsPlayer)
        this.SendDeathMessage(deathType, attacker);
      if (flag)
        this.DropAllItems((Item[]) null, UpdateBlockMethod.DropTimeShort);
      this.OnDeathLocal(deathType, attacker, weaponID, damage);
    }

    protected virtual void OnDeathLocal(
      DamageType deathType,
      Actor attacker,
      Item weaponID,
      float damage)
    {
    }

    public virtual bool IsCustomMob
    {
      get
      {
        return false;
      }
    }

    protected virtual void SetDeathDropItems(DamageType damageType, Actor attacker)
    {
    }

    protected virtual bool ShouldDropItemsOnDeath(
      DamageType damageType,
      Actor attacker,
      Item weaponID)
    {
      if (this.Properties.DropInventoryOnDeath.HasValue && this.Properties.DropInventoryOnDeath.Value)
        return true;
      if (this.Properties.DropRandomLootOnDeath.HasValue)
        return this.Properties.DropRandomLootOnDeath.Value;
      return false;
    }

    protected virtual void ExplodeModel()
    {
    }

    protected virtual float ExplodeBlocksRatio
    {
      get
      {
        return 0.03f;
      }
    }

    protected virtual Vector2 ExplodeBlocksScale
    {
      get
      {
        return new Vector2(1.5f, 20f);
      }
    }

    public void DropAllItems(Item[] itemsToKeep, UpdateBlockMethod method)
    {
      this.DropAllItems((StudioForge.TotalMiner.Inventory) this.Inventory, itemsToKeep, method);
    }

    private void DropAllItems(StudioForge.TotalMiner.Inventory inventory, Item[] itemsToKeep, UpdateBlockMethod method)
    {
      ParticleType type = this.IsPlayer || this.NpcTypeData.IsPassive || this.IsUsingCustomLootTable ? ParticleType.None : ParticleType.Loot;
      for (int slotID = 0; slotID < inventory.Count; ++slotID)
      {
        if (!this.ContainsItem(itemsToKeep, inventory[slotID].ItemID))
          this.DropItem(type, inventory, slotID, Vector2.Zero, 0.0f, method);
      }
    }

    private bool ContainsItem(Item[] array, Item item)
    {
      if (array == null || array.Length == 0)
        return false;
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == item)
          return true;
      }
      return false;
    }

    public bool DropItem(ParticleType type, InventoryItem item, UpdateBlockMethod method)
    {
      return this.DropItem(type, item, new Vector2(this.ViewDirection.X * 8f, this.ViewDirection.Z * 8f), 3f, method);
    }

    public bool DropItem(
      ParticleType type,
      InventoryItem item,
      Vector2 dir,
      float minPickupAge,
      UpdateBlockMethod method)
    {
      if (item.Count <= 0)
        return false;
      GlobalPoint3D point = this.instance.Map.GetPoint(this.EyePosition);
      return this.instance.DropItem(type, point, item, dir, minPickupAge, method, this.GamerID);
    }

    public void DropItem(ParticleType type, int slotID, UpdateBlockMethod method)
    {
      this.DropItem(type, (StudioForge.TotalMiner.Inventory) this.Inventory, slotID, method);
    }

    public void DropItem(
      ParticleType type,
      StudioForge.TotalMiner.Inventory inventory,
      int slotID,
      UpdateBlockMethod method)
    {
      this.DropItem(type, inventory, slotID, new Vector2(this.ViewDirection.X * 8f, this.ViewDirection.Z * 8f), 3f, method);
    }

    public void DropItem(
      ParticleType type,
      StudioForge.TotalMiner.Inventory inventory,
      int slotID,
      Vector2 dir,
      float minPickupAge,
      UpdateBlockMethod method)
    {
      if (slotID < 0 || slotID >= inventory.Count)
        return;
      InventoryItem inventoryItem = inventory[slotID];
      if (inventoryItem.Count <= 0)
        return;
      this.DropItem(type, inventoryItem, dir, minPickupAge, method);
      inventory.DecrementItem(slotID, inventoryItem.Count);
      this.OnItemDropped(inventoryItem, slotID);
    }

    protected virtual void OnItemDropped(InventoryItem item, int slotID)
    {
    }

    public bool EatFood(Hand hand, Item itemID)
    {
      bool flag = false;
      Actor actor;
      if (this.IsPlayer)
      {
        actor = this.HitTarget.Target;
        if (actor == null || !actor.Properties.CanBeHealedByOther.HasValue || (!actor.Properties.CanBeHealedByOther.Value || (double) Vector3.Distance(actor.Position, this.Position) > 10.0))
          actor = this;
      }
      else
        actor = this;
      if (actor.Heal(this, itemID))
      {
        if (this.instance.IsFiniteResources)
          this.Inventory.DecrementItem(hand.HandIndexOnSwingStart);
        flag = true;
        if (actor == this)
          Sounds.PlaySound(itemID, ItemSoundType.Use, (ITMActor) this, true);
      }
      return flag;
    }

    public bool Heal(Actor healer, Item itemID)
    {
      bool flag = false;
      if (this.instance.IsEatingOrHealingAllowed && (double) this.Health < (double) this.MaxHealth)
      {
        short healPower = Globals1.ItemData[(int) itemID].HealPower;
        if (healPower > (short) 0)
        {
          this.Health += (float) healPower;
          if ((double) this.Health > (double) this.MaxHealth)
            this.Health = this.MaxHealth;
          if (healer != this && healer.IsPlayer)
            this.instance.AddNotification(healer as Player, " healed " + this.DisplayGamertag, NotifyRecipient.Local);
          this.instance.NetworkManager.SendHeal(healer, this, (byte) healPower);
          flag = true;
        }
      }
      return flag;
    }

    public void HealFromNetwork(Player healer, int heal)
    {
      if ((double) this.Health >= (double) this.MaxHealth)
        return;
      this.Health += (float) heal;
      if ((double) this.Health > (double) this.MaxHealth)
        this.Health = this.MaxHealth;
      if (healer == this)
        return;
      this.instance.AddNotification(healer, " healed " + this.DisplayGamertag, NotifyRecipient.Local);
    }

    public bool Struck(Actor attacker, SkillType attackType, Item weapon, bool criticalRegion)
    {
      float damage;
      return this.Struck(attacker, attackType, weapon, criticalRegion, out damage);
    }

    public bool Struck(
      Actor attacker,
      SkillType attackType,
      Item weapon,
      bool isCriticalRegion,
      out float damage)
    {
      damage = 0.0f;
      if (!this.instance.IsCombatEnabled || !this.Properties.CanFight.Value || attacker != null && attacker.IsPlayer && (this.IsPlayer && !this.PlayerStruckPlayer(attacker as Player)))
        return false;
      DamageType damageType;
      damage = this.CalcStrikeDamage(attacker, attackType, weapon, isCriticalRegion, out damageType);
      Vector3 knockForce = (double) damage <= 0.0 || attacker == null ? Vector3.Zero : attacker.ViewDirection * Math.Max(0.01f, damage / 350f);
      damage = this.TakeDamageAndDisplay(damageType, damage, knockForce, attacker, weapon, attackType);
      return true;
    }

    protected virtual bool PlayerStruckPlayer(Player attacker)
    {
      return false;
    }

    private void AddAdditionalAttackEffects(
      Actor attacker,
      Item weapon,
      float damage,
      bool isLocalGamer)
    {
      if (attacker != null && isLocalGamer)
      {
        Actor.BonusData recoilData = this.GetRecoilData();
        if ((double) recoilData.Value > 0.0)
        {
          float damage1 = damage * recoilData.Value;
          double damageAndDisplay = (double) attacker.TakeDamageAndDisplay(DamageType.ItemUse, damage1, Vector3.Zero, this, this.Inventory[recoilData.SlotID].ItemID, SkillType.Attack);
          this.OnItemUsed(recoilData.SlotID);
        }
      }
      if (weapon != Item.IceArrow && (attacker == null || !attacker.IsItemEquippedAndUsable(Item.RingOfIce)))
        return;
      if (isLocalGamer && (double) this.FreezeImmunityTimer <= 0.0 && this.random.NextUint(2U) == 0U)
      {
        this.FreezeTimer += 3f;
        this.FreezeImmunityTimer = 4f;
      }
      if (attacker == null || !attacker.IsPlayer || (!attacker.IsLocalGamer || weapon == Item.IceArrow))
        return;
      attacker.OnItemUsed(attacker.Inventory.GetEquipSlotID(Item.RingOfIce));
    }

    public float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce)
    {
      return this.TakeDamageAndDisplay(damageType, damage, knockForce, (Actor) null, Item.None, SkillType.None);
    }

    public float TakeDamageAndDisplay(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      Actor attacker,
      Item weaponID,
      SkillType attackType)
    {
      if (this.IsDeadOrInactiveOrDisabled || !this.instance.IsCombatEnabled && this.IsPlayer)
        return 0.0f;
      if (this.IsGod)
      {
        this.DoGodDamage(damageType, damage, attacker, weaponID);
        return 0.0f;
      }
      bool isLocalGamer = this.IsLocalGamer;
      Player player = attacker as Player;
      bool flag = attacker != this && player != null && player.IsLocalGamer;
      if (flag && player.IsGod)
        damage = this.Health + 100f;
      float damage1 = damage;
      if (isLocalGamer)
        damage = this.TakeDamageLocal(damageType, damage, knockForce, attacker, weaponID);
      this.OnArmorUsed(damageType);
      if ((double) damage > 0.0)
      {
        if (damageType != DamageType.Drowning)
          this.instance.AddDamageParticles(this.Position + (this.EyePosition - this.Position) * 0.75f, damage, damageType);
        if ((double) this.playPainSoundTimer <= 0.0 && (damageType != DamageType.Effect || (double) this.playPainSoundTimer < -6.0))
        {
          this.PlayPainSound(damage);
          this.playPainSoundTimer = this.playerPainSoundDelay;
        }
        if (flag)
          player.RegisterDamageDealt(damageType, damage, this, weaponID, this.actorState == ActorState.Dying);
        if (damageType == DamageType.Combat)
          this.AddAdditionalAttackEffects(attacker, weaponID, damage, isLocalGamer);
      }
      if (attacker != null && attacker.IsLocalGamer || attacker == null && isLocalGamer)
        this.instance.NetworkManager.SendDamage(damageType, damage1, this, attacker, weaponID, new GlobalPoint3D?());
      if (attacker != null)
        Sounds.PlaySound(weaponID, (double) damage > 0.0 ? ItemSoundType.Use : ItemSoundType.UseFail, this.AudioEmitter, (ITMActor) attacker);
      switch (damageType)
      {
        case DamageType.Unknown:
        case DamageType.Combat:
        case DamageType.ItemUse:
        case DamageType.Blast:
        case DamageType.BlockFallingOnHead:
        case DamageType.ShieldDeflect:
          int damage2 = (double) damage >= 1.0 ? (int) damage : ((double) damage <= 0.0 ? 0 : (int) ((double) damage + 1.0));
          this.DisplayDamage(attacker, damage2, weaponID, attackType, damageType);
          break;
      }
      return damage;
    }

    private void DisplayDamage(
      Actor attacker,
      int damage,
      Item weaponID,
      SkillType attackType,
      DamageType damageType)
    {
      Player player = attacker as Player;
      int num1 = attacker != null ? attacker.GetMaxHit(weaponID, attackType) : 0;
      Color color = damage >= num1 ? Color.OrangeRed : (damage > 0 ? Color.Red : (damageType == DamageType.ShieldDeflect ? Color.Cyan : Color.Blue));
      int num2 = this.random.Next(30) + 30;
      if (this.random.NextDouble() > 0.5)
        num2 = -num2;
      float num3 = -60f;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
      {
        Player virtualPlayer = localEnabledPlayer.VirtualPlayer;
        float y = !this.IsPlayer || player != null ? num3 : -num3;
        if (virtualPlayer == this || virtualPlayer == attacker)
        {
          CoreGlobals.Message.ShowMessage(damage > 0 ? string.Format("+{0}", (object) damage) : "0", new Vector2((float) num2, y), 2f, 1.7f, color, localEnabledPlayer.GetScreenMatrix(true));
        }
        else
        {
          Vector3 position = this.Position;
          position.Y += this.EyeOffset.Y * 0.75f;
          Vector3 vector3 = localEnabledPlayer.Viewport.Project(position, localEnabledPlayer.ProjectionMatrix, virtualPlayer.ViewMatrix, Matrix.Identity);
          if ((double) vector3.Z < 1.0)
          {
            float num4 = Vector3.Distance(virtualPlayer.EyePosition, position);
            if ((double) num4 < 40.0)
            {
              float scale = (float) ((50.0 - (double) num4) * 0.0364999994635582);
              string str = damage > 0 ? string.Format("+{0}", (object) damage) : "0";
              vector3.X -= (float) ((double) CoreGlobals.Message.Font.MeasureString(str).X * (double) scale * 0.5);
              CoreGlobals.Message.ShowMessage(str, new Vector2(vector3.X, vector3.Y), new Vector2((float) num2 * scale, y * scale), 1.5f, scale, color, false, localEnabledPlayer.GetScreenMatrix(true));
            }
          }
        }
      }
    }

    protected virtual float TakeDamageLocal(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      Actor attacker,
      Item weaponID)
    {
      if ((double) damage <= 0.0)
        return 0.0f;
      bool flag = false;
      if ((double) damage >= (double) this.Health)
      {
        damage = this.Health;
        this.Health = 0.0f;
        flag = true;
      }
      else
        this.Health -= damage;
      this.KnockForce += knockForce;
      if (flag)
        this.Die(damageType, attacker, weaponID, damage);
      else if ((double) this.Health < (double) this.MaxHealth * 0.150000005960464)
      {
        Actor.BonusData lifeSavingItemSlot = this.GetLifeSavingItemSlot();
        if (lifeSavingItemSlot.SlotID >= 0)
        {
          this.OnItemUsed(lifeSavingItemSlot.SlotID);
          Sounds.PlaySound(Item.Teleport, (ITMActor) this, false);
          this.DefaultRespawn();
        }
      }
      return damage;
    }

    private float CalcStrikeDamage(
      Actor attacker,
      SkillType attackType,
      Item weapon,
      bool isCriticalRegion,
      out DamageType damageType)
    {
      if (attacker == null)
        return this.CalcStrikeDamageNoAttacker(attackType, weapon, 50, isCriticalRegion, out damageType);
      float num = 0.0f;
      int attackRoll = this.random.Next(attackType == SkillType.Ranged ? (int) ((double) (attacker.RangedLevel(true) + 2) * 100.0) : (int) ((double) (attacker.AttackLevel(true) + 2) * 100.0));
      int defenceRoll = this.GetDefenceRoll(attacker, attackRoll, out damageType);
      if (attackRoll > defenceRoll)
      {
        int maxHit = attacker.GetMaxHit(weapon, attackType);
        num = (float) this.random.Next((int) ((isCriticalRegion ? (double) ((float) maxHit * 0.25f) : 0.0) * 100.0), (int) (((isCriticalRegion ? (double) maxHit : (double) ((float) maxHit - (float) maxHit * 0.25f)) + 1.0) * 100.0)) * 0.01f;
        if ((double) num > 0.0 && (double) num < 1.0)
          num = 1f;
        damageType = DamageType.Combat;
        if ((double) num < (double) maxHit)
        {
          Actor.BonusData criticalBonus = attacker.GetCriticalBonus();
          if (criticalBonus.SlotID >= 0 && (double) criticalBonus.Value > 0.0 && this.random.NextDouble() <= (double) criticalBonus.Value)
          {
            num = (float) maxHit;
            attacker.OnItemUsed(criticalBonus.SlotID);
          }
        }
      }
      return num;
    }

    private float CalcStrikeDamageNoAttacker(
      SkillType attackType,
      Item weapon,
      int attackersAttackLevel,
      bool isCriticalRegion,
      out DamageType damageType)
    {
      float num = 0.0f;
      int attackRoll = this.random.Next((int) ((double) attackersAttackLevel * 100.0));
      int defenceRoll = this.GetDefenceRoll((Actor) null, attackRoll, out damageType);
      if (attackRoll > defenceRoll)
      {
        int max = (int) ItemData.GetItemStrikeDamage(weapon) + 1;
        int min = isCriticalRegion ? (int) ((double) max * 0.25) : 0;
        if (!isCriticalRegion)
          max -= (int) ((double) max * 0.25);
        num = (float) this.random.Next(min, max);
      }
      return num;
    }

    private int GetDefenceRoll(Actor attacker, int attackRoll, out DamageType damageType)
    {
      damageType = DamageType.Combat;
      int max = this.DefenceLevel(true) * Globals2.Defence;
      int num1 = this.random.Next(max);
      if (attackRoll > num1 && this.IsHoldingOutShield && (attacker == null || this.Frustum.Intersects(attacker.Box)))
      {
        int num2 = this.random.Next(max);
        if (num2 > num1)
        {
          if (num2 >= attackRoll)
            damageType = DamageType.ShieldDeflect;
          num1 = num2;
        }
      }
      return num1;
    }

    private bool IsHoldingOutShield
    {
      get
      {
        return this.LeftHand != null && ItemData.IsSubType(this.LeftHand.ItemID, ItemSubType.Shield) && this.LeftHand.IsSwingExtended || this.RightHand != null && ItemData.IsSubType(this.RightHand.ItemID, ItemSubType.Shield) && this.RightHand.IsSwingExtended;
      }
    }

    public int GetMaxHit(Item weapon, SkillType attackType)
    {
      int val1 = this.StrengthLevel(true);
      ActorType actorType = this.IsPlayer ? ActorType.Player : this.ActorType;
      float num1 = weapon == Item.Hand ? (float) Globals1.NpcTypeData[(int) actorType].HandMaxHit : ItemData.GetItemStrikeDamage(weapon);
      float num2 = attackType == SkillType.Ranged ? 0.005f : 0.01f;
      float num3 = (float) Math.Min(val1, 99) * num2;
      if (val1 > 99)
        num3 += (float) ((double) (val1 - 99) * (double) num2 * 0.300000011920929);
      return (int) ((double) num1 + (double) num1 * (double) num3 + 0.5);
    }

    private void OnArmorUsed(DamageType damageType)
    {
      switch (damageType)
      {
        case DamageType.Combat:
          int slotID1 = -1;
          switch (this.random.Next(6))
          {
            case 0:
              slotID1 = this.Inventory.BodyIndex;
              break;
            case 1:
              slotID1 = this.Inventory.LegsIndex;
              break;
            case 2:
              slotID1 = this.Inventory.HeadIndex;
              break;
            case 3:
              slotID1 = this.Inventory.LeftSideIndex;
              break;
            case 4:
              slotID1 = this.Inventory.FeetIndex;
              break;
            case 5:
              bool flag = this.LeftHand != null && ItemData.IsSubType(this.LeftHand.ItemID, ItemSubType.Shield);
              slotID1 = this.RightHand != null && ItemData.IsSubType(this.RightHand.ItemID, ItemSubType.Shield) ? (flag ? (this.random.Next(2) == 0 ? this.LeftHand.HandIndex : this.RightHand.HandIndex) : this.RightHand.HandIndex) : (flag ? this.LeftHand.HandIndex : -1);
              break;
          }
          if (slotID1 < 0)
            break;
          Item itemId = this.Inventory[slotID1].ItemID;
          if (itemId != Item.None)
          {
            if (Globals1.ItemTypeData[(int) itemId].Type != ItemType.Armor || !this.CanUseItem(itemId))
              break;
            this.OnItemUsed(slotID1);
            Sounds.PlaySound(itemId, (ITMActor) this, true);
            break;
          }
          Sounds.PlaySound(ItemSoundGroup.BodyHit, ItemSoundType.Use, (ITMActor) this, true);
          break;
        case DamageType.ShieldDeflect:
          Item itemID1 = this.LeftHand != null ? this.LeftHand.ItemID : Item.None;
          if (!ItemData.IsSubType(itemID1, ItemSubType.Shield))
            itemID1 = Item.None;
          Item itemID2 = this.RightHand != null ? this.RightHand.ItemID : Item.None;
          if (!ItemData.IsSubType(itemID2, ItemSubType.Shield))
            itemID2 = Item.None;
          if (itemID1 != Item.None && this.LeftHand.IsSwingExtended)
            itemID2 = Item.None;
          int slotID2 = itemID2 != Item.None ? this.RightHand.HandIndex : (itemID1 != Item.None ? this.LeftHand.HandIndex : -1);
          if (slotID2 >= 0)
          {
            this.OnItemUsed(slotID2);
            this.OnItemUsed(slotID2);
          }
          Sounds.PlaySound(ItemSoundGroup.ItemShield, ItemSoundType.Hit, (ITMActor) this, true);
          break;
      }
    }

    private void DoGodDamage(DamageType damageType, float damage, Actor attacker, Item weaponID)
    {
    }

    public virtual void DefaultRespawn()
    {
    }

    protected void SendDeathMessage(DamageType deathType, Actor attacker)
    {
      string message = (string) null;
      Player player = this as Player;
      if (attacker != null)
      {
        if (this.IsPlayer)
        {
          if (attacker.IsPlayer)
            message = this.GetKilledByMessage(attacker.DisplayGamertag);
          else if (attacker is NpcBase)
            message = " was killed by a " + Utils.InsertSpacesBeforeCapitals(attacker.ActorType.ToString());
        }
        else if (attacker.IsPlayer)
        {
          if (this.ActorType != ActorType.Diablo || this.IsCustomMob)
            return;
          message = this.GetMobKillMessage() + Utils.InsertSpacesBeforeCapitals(this.ActorType.ToString());
          player = attacker as Player;
        }
      }
      if (message == null)
      {
        switch (deathType)
        {
          case DamageType.Unknown:
            message = " died in some (probably horrible) unknown way";
            break;
          case DamageType.Drowning:
            message = " drowned";
            break;
          case DamageType.Burning:
            message = " burned to death";
            break;
          case DamageType.Blast:
            message = " was blown to pieces in an explosion";
            break;
          case DamageType.BlockFallingOnHead:
            message = " was crushed by falling blocks";
            break;
          default:
            switch (random.Next(3))
            {
                  case 0:
                    message = " was killed";
                    break;
                  case 1:
                    message = " made mistakes";
                    break;
                  case 2:
                    message = " just paid the ultimate price";
                    break;
            }
            break;

        }
      }
      instance.AddNotification(player, message, NotifyRecipient.Global);
    }

    protected string GetKilledByMessage(string killerGamertag)
    {
      switch (random.Next(6))
      {
        case 0:
          return " was killed by " + killerGamertag;
        case 1:
          return " was owned by " + killerGamertag;
        case 2:
          return " was pwned by " + killerGamertag;
        case 3:
          return " was smashed by " + killerGamertag;
        case 4:
          return " was taught a lesson by " + killerGamertag;
        case 5:
          return " was no match for " + killerGamertag;
        default:
          return " was killed";
      }
    }

    protected string GetMobKillMessage()
    {
      switch (this.random.Next(4))
      {
        case 0:
          return " killed a ";
        case 1:
          return " owned a ";
        case 2:
          return " pwned a ";
        default:
          return " smashed a ";
      }
    }

    public virtual bool IsLeftHandItemSwinging
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsRightHandItemSwinging
    {
      get
      {
        return false;
      }
    }

    public virtual void OnBlastCreated(QueuedBlast blast, Player detonator, Item itemID)
    {
      Vector3 direction = this.Position - this.map.GetBlockCenter(blast.Point);
      float distance = direction.Length();
      direction.Normalize();
      this.TakeBlastDamage(distance, blast.Strength, direction, detonator, itemID);
    }

    private void TakeBlastDamage(
      float distance,
      float blastStrength,
      Vector3 direction,
      Player player,
      Item itemID)
    {
      if ((double) distance > (double) blastStrength)
        return;
      float num = MathHelper.Lerp(0.2f, 0.02f, distance / blastStrength);
      double damageAndDisplay = (double) this.TakeDamageAndDisplay(DamageType.Blast, (float) ((double) blastStrength / (double) distance * 3.0), direction * num, (Actor) player, itemID, SkillType.None);
    }

    public virtual Matrix GetScreenMatrix(bool globalScreenSpace)
    {
      return Matrix.Identity;
    }

    private Actor.BonusData GetLifeSavingItemSlot()
    {
      Actor.BonusData bonusData = new Actor.BonusData()
      {
        SlotID = -1
      };
      if (this.IsItemEquippedAndUsable(Item.RingOfExemption))
        bonusData.SlotID = this.GetItemEquipSlotID(Item.RingOfExemption);
      return bonusData;
    }

    public Actor.BonusData GetCriticalBonus()
    {
      Actor.BonusData bonusData = new Actor.BonusData()
      {
        SlotID = -1
      };
      if (this.IsItemEquippedAndUsable(Item.AmuletOfFury))
      {
        bonusData.SlotID = this.GetItemEquipSlotID(Item.AmuletOfFury);
        bonusData.Value = 0.15f;
      }
      return bonusData;
    }

    private Actor.BonusData GetRecoilData()
    {
      Actor.BonusData bonusData = new Actor.BonusData()
      {
        SlotID = -1
      };
      if (this.IsItemEquippedAndUsable(Item.RingOfBob))
      {
        bonusData.SlotID = this.GetItemEquipSlotID(Item.RingOfBob);
        bonusData.Value = 0.1f;
      }
      return bonusData;
    }

    public HitTarget BuildHitTargetData(
      Vector3 dir,
      Vector3 basePosOffset,
      HitTargetOptions options,
      List<ActorType> excludeTypes)
    {
      return this.instance.BuildHitTarget(new Ray(this.viewMatrixPosition + basePosOffset, dir), this, options, excludeTypes);
    }

    protected virtual void CheckWorldCollision()
    {
      float num1 = this.Size.X * this.halfSizeFactor;
      if (this.isOnRope && (double) this.Size.X > 0.829999983310699)
        num1 = 0.83f * this.halfSizeFactor;
      float num2 = 0.1f;
      float num3 = num1 - num2;
      BoundingBox box = new BoundingBox();
      box.Min.X = this.Position.X + this.Velocity.X - num1;
      box.Min.Y = this.Position.Y + this.Velocity.Y + num2;
      box.Min.Z = this.Position.Z + this.Velocity.Z - num3;
      box.Max.X = this.Position.X + this.Velocity.X + num1;
      box.Max.Y = this.Position.Y + this.Velocity.Y + this.Size.Y - num2;
      box.Max.Z = this.Position.Z + this.Velocity.Z + num3;
      this.CheckWorldCollisionX(box);
      box.Min.X = this.Position.X + this.Velocity.X - num3;
      box.Min.Y = this.Position.Y + this.Velocity.Y + num2;
      box.Min.Z = this.Position.Z + this.Velocity.Z - num1;
      box.Max.X = this.Position.X + this.Velocity.X + num3;
      box.Max.Y = this.Position.Y + this.Velocity.Y + this.Size.Y - num2;
      box.Max.Z = this.Position.Z + this.Velocity.Z + num1;
      this.CheckWorldCollisionZ(box);
      box.Min.X = this.Position.X + this.Velocity.X - num3;
      box.Min.Y = this.Position.Y + this.Velocity.Y;
      box.Min.Z = this.Position.Z + this.Velocity.Z - num3;
      box.Max.X = this.Position.X + this.Velocity.X + num3;
      box.Max.Y = this.Position.Y + this.Velocity.Y + this.Size.Y;
      box.Max.Z = this.Position.Z + this.Velocity.Z + num3;
      this.CheckWorldCollisionY(box);
      box.Min.X = this.Position.X + this.Velocity.X - num3;
      box.Min.Y = this.Position.Y + this.Velocity.Y;
      box.Min.Z = this.Position.Z + this.Velocity.Z - num3;
      box.Max.X = this.Position.X + this.Velocity.X + num3;
      box.Max.Y = this.Position.Y + this.Velocity.Y + this.Size.Y;
      box.Max.Z = this.Position.Z + this.Velocity.Z + num3;
      this.ClipWorldEdge(box);
      lock (this.ppoints1)
      {
        for (int index = this.ppoints1.Count - 1; index >= 0; --index)
        {
          Actor.TimedPoint timedPoint = this.ppoints1[index];
          if (!this.ppoints2.Contains(timedPoint.Point) && this.instance.TotalGameTime - timedPoint.TimeStamp > (double) timedPoint.Period)
          {
            this.UpdatePressureBlock(timedPoint.Point, timedPoint.BlockID, false);
            this.ppoints1.RemoveAt(index);
          }
        }
        this.ppoints2.Clear();
      }
    }

    private void ClearPressurePoints()
    {
      lock (this.ppoints1)
      {
        for (int index = this.ppoints1.Count - 1; index >= 0; --index)
        {
          Actor.TimedPoint timedPoint = this.ppoints1[index];
          this.UpdatePressureBlock(timedPoint.Point, timedPoint.BlockID, false);
        }
        this.ppoints1.Clear();
      }
    }

    protected void ClampToWorldBounds()
    {
      float num = this.Size.X * this.halfSizeFactor - 0.1f;
      this.ClipWorldEdge(new BoundingBox()
      {
        Min = {
          X = this.Position.X + this.Velocity.X - num,
          Y = this.Position.Y + this.Velocity.Y,
          Z = this.Position.Z + this.Velocity.Z - num
        },
        Max = {
          X = this.Position.X + this.Velocity.X + num,
          Y = this.Position.Y + this.Velocity.Y + this.Size.Y,
          Z = this.Position.Z + this.Velocity.Z + num
        }
      });
    }

    private void CheckWorldCollisionY(BoundingBox box)
    {
      if ((double) this.Velocity.Y < 0.0)
      {
        this.ClipVelocity(box.Min, new Vector3(box.Max.X, box.Min.Y - this.Velocity.Y, box.Max.Z), Vector3.Up, box);
      }
      else
      {
        if ((double) this.Velocity.Y <= 0.0)
          return;
        this.ClipVelocity(new Vector3(box.Min.X, box.Max.Y - this.Velocity.Y, box.Min.Z), box.Max, Vector3.Down, box);
      }
    }

    private void CheckWorldCollisionX(BoundingBox box)
    {
      if ((double) this.Velocity.X < 0.0)
      {
        this.ClipVelocity(box.Min, new Vector3(box.Min.X - this.Velocity.X, box.Max.Y, box.Max.Z), Vector3.Left, box);
      }
      else
      {
        if ((double) this.Velocity.X <= 0.0)
          return;
        this.ClipVelocity(new Vector3(box.Max.X - this.Velocity.X, box.Min.Y, box.Min.Z), box.Max, Vector3.Right, box);
      }
    }

    private void CheckWorldCollisionZ(BoundingBox box)
    {
      if ((double) this.Velocity.Z < 0.0)
      {
        this.ClipVelocity(box.Min, new Vector3(box.Max.X, box.Max.Y, box.Min.Z - this.Velocity.Z), Vector3.Forward, box);
      }
      else
      {
        if ((double) this.Velocity.Z <= 0.0)
          return;
        this.ClipVelocity(new Vector3(box.Min.X, box.Min.Y, box.Max.Z - this.Velocity.Z), box.Max, Vector3.Backward, box);
      }
    }

    private void ClipWorldEdge(BoundingBox box)
    {
      bool flag = this.IsGod || this.instance.IsAvatarDesigner;
      int num1 = flag ? 15 : 0;
      if ((double) this.Velocity.Y < 0.0)
      {
        if ((double) box.Min.Y < (double) this.map.MapBound.Min.Y)
        {
          this.Velocity.Y = 0.0f;
          this.Position.Y = (float) this.map.MapBound.Min.Y;
        }
      }
      else if ((double) this.Velocity.Y > 0.0 && (double) box.Max.Y >= (double) (this.map.MapBound.Max.Y + num1))
      {
        this.Velocity.Y = 0.0f;
        this.Position.Y = (float) this.map.MapBound.Max.Y - this.Size.Y;
      }
      int num2 = flag ? 40 : 0;
      if ((double) this.Velocity.X < 0.0)
      {
        if ((double) box.Min.X + (double) this.Velocity.X < (double) (this.map.MapBound.Min.X - num2))
          this.Velocity.X = 0.0f;
      }
      else if ((double) this.Velocity.X > 0.0 && (double) box.Max.X + (double) this.Velocity.X >= (double) (this.map.MapBound.Max.X + num2))
        this.Velocity.X = 0.0f;
      if ((double) this.Velocity.Z < 0.0)
      {
        if ((double) box.Min.Z + (double) this.Velocity.Z >= (double) (this.map.MapBound.Min.Z - num2))
          return;
        this.Velocity.Z = 0.0f;
      }
      else
      {
        if ((double) this.Velocity.Z <= 0.0 || (double) box.Max.Z + (double) this.Velocity.Z < (double) (this.map.MapBound.Max.Z + num2))
          return;
        this.Velocity.Z = 0.0f;
      }
    }

    private void ClipVelocity(Vector3 min, Vector3 max, Vector3 normal, BoundingBox box)
    {
      lock (this.map.PointsOfPenetration)
      {
        this.map.GetPointsOfPenetration(min, max);
        foreach (GlobalPoint3D p1 in this.map.PointsOfPenetration)
        {
          byte blockId = this.map.GetBlockID(p1);
          Block blockID = (Block) blockId;
          if (!this.map.IsBlockPassable(blockId))
          {
            Block block = blockID;
            if ((uint) block <= 150U)
            {
              switch (block)
              {
                case Block.Stairs:
                  break;
                case Block.SnowLayer:
                  if (this.map.GetAuxData(p1) > (byte) 2)
                  {
                    BoundingBox blockBox = this.instance.GetBlockBox(p1);
                    this.ClipVelocityCore(p1, blockID, box, blockBox, normal);
                    continue;
                  }
                  continue;
                case Block.Ramp:
                  goto label_10;
                default:
                  goto label_40;
              }
            }
            else
            {
              switch (block)
              {
                case Block.PressurePlate:
                  BoundingBox plateBox = this.instance.GetPlateBox(p1);
                  this.ClipVelocityCore(p1, blockID, box, plateBox, normal);
                  if ((double) normal.Y != 0.0 && this.IsCanTouchOnTestY(p1, Block.PressurePlate))
                  {
                    Actor.TimedPoint timedPoint = new Actor.TimedPoint()
                    {
                      Point = p1,
                      TimeStamp = this.instance.TotalGameTime,
                      Period = 0.75f,
                      BlockID = Block.PressurePlate
                    };
                    lock (this.ppoints1)
                    {
                      int index = this.ppoints1.IndexOf(timedPoint);
                      if (index < 0)
                      {
                        if (this.UpdatePressureBlock(p1, Block.PressurePlate, true))
                          this.ppoints1.Add(timedPoint);
                      }
                      else
                        this.ppoints1[index] = timedPoint;
                      this.ppoints2.Add(p1);
                      continue;
                    }
                  }
                  else
                    continue;
                case Block.ScriptBlock:
                  BoundingBox blockBox1 = this.instance.GetBlockBox(p1);
                  this.ClipVelocityCore(p1, blockID, box, blockBox1, normal);
                  if ((double) normal.Y != 0.0 && this.IsCanTouchOnTestY(p1, Block.ScriptBlock))
                  {
                    Actor.TimedPoint timedPoint = new Actor.TimedPoint()
                    {
                      Point = p1,
                      TimeStamp = this.instance.TotalGameTime,
                      Period = 0.05f,
                      BlockID = Block.ScriptBlock
                    };
                    lock (this.ppoints1)
                    {
                      int index = this.ppoints1.IndexOf(timedPoint);
                      if (index < 0)
                      {
                        if (this.UpdatePressureBlock(p1, Block.ScriptBlock, true))
                          this.ppoints1.Add(timedPoint);
                      }
                      else
                        this.ppoints1[index] = timedPoint;
                      this.ppoints2.Add(p1);
                      continue;
                    }
                  }
                  else
                    continue;
                case Block.Stairs2:
                  break;
                case Block.Ramp2:
                  goto label_10;
                default:
                  goto label_40;
              }
            }
            if (((int) this.map.GetAuxData(p1) & 4) == 0)
            {
              BoundingBox stairBoxHigh = this.instance.GetStairBoxHigh(p1);
              this.ClipVelocityCore(p1, blockID, box, stairBoxHigh, normal);
              BoundingBox stairBoxLow = this.instance.GetStairBoxLow(p1, (double) normal.Y != 0.0);
              this.ClipVelocityCore(p1, blockID, box, stairBoxLow, normal);
              continue;
            }
            BoundingBox blockBox2 = this.instance.GetBlockBox(p1);
            this.ClipVelocityCore(p1, blockID, box, blockBox2, normal);
            continue;
label_10:
            if (((int) this.map.GetAuxData(p1) & 4) == 0)
            {
              if (p1.Y < this.map.MapBound.Max.Y - 2)
              {
                GlobalPoint3D p2 = p1;
                ++p2.Y;
                if (!this.IsCrouching)
                  ++p2.Y;
                if (!this.map.BlockData[(int) this.map.GetBlockID(p2)].IsPassable)
                {
                  BoundingBox blockBox3 = this.instance.GetBlockBox(p1);
                  this.ClipVelocityCore(p1, blockID, box, blockBox3, normal);
                  continue;
                }
              }
              this.ClipVelocityRamp(p1, normal, box);
              continue;
            }
            BoundingBox blockBox4 = this.instance.GetBlockBox(p1);
            this.ClipVelocityCore(p1, blockID, box, blockBox4, normal);
            continue;
label_40:
            BoundingBox blockBox5 = this.instance.GetBlockBox(p1);
            this.ClipVelocityCore(p1, blockID, box, blockBox5, normal);
          }
        }
      }
    }

    protected bool UpdatePressureBlock(GlobalPoint3D p, Block blockID, bool power)
    {
      switch (blockID)
      {
        case Block.PressurePlate:
          UpdateBlockMethod method = this.IsPlayer ? UpdateBlockMethod.Player : UpdateBlockMethod.Strategy;
          return this.instance.DeliverPower(p, blockID, BlockFace.ProxyDefault, power, method, this.GamerID, true, true);
        case Block.ScriptBlock:
          ScriptBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(p) as ScriptBlock;
          if (dataBlock == null)
            return false;
          ScriptExecuteData data = new ScriptExecuteData()
          {
            Actor = this,
            BlockOffset = new GlobalPoint3D?(p)
          };
          this.instance.ExecuteScript(power ? dataBlock.PowerOnScript : dataBlock.PowerOffScript, data, this.IsLocalGamer);
          return true;
        default:
          return false;
      }
    }

    private void ClipVelocityCore(
      GlobalPoint3D p,
      Block blockID,
      BoundingBox box,
      BoundingBox blockBox,
      Vector3 normal)
    {
      if (!box.Intersects(blockBox))
        return;
      bool flag1 = (double) normal.Y != 0.0;
      bool movingX = (double) normal.X != 0.0;
      float gravity = this.Gravity;
      if (!flag1 && ((double) gravity < 0.0 && (double) this.Velocity.Y >= (double) gravity || (double) gravity > 0.0 && (double) this.Velocity.Y <= (double) gravity) && (!this.positionInterpolator.IsActive && (double) blockBox.Max.Y < (double) box.Min.Y + (double) this.map.TileSize * 0.5))
      {
        GlobalPoint3D p1 = p;
        ++p1.Y;
        Block blockId1 = (Block) this.map.GetBlockID(p1);
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
        bool flag2 = num1 != 0;
        if (this.map.IsBlockPassable((byte) blockId1) || flag2)
        {
          bool flag3 = true;
          ++p1.Y;
          float num2 = blockBox.Max.Y - blockBox.Min.Y;
          box.Max.Y += num2;
          if (box.Intersects(this.instance.GetBlockBox(p1)))
          {
            Block blockId2 = (Block) this.map.GetBlockID(p1);
            int num3;
            switch (blockId2)
            {
              case Block.WoodDoorBottom:
              case Block.SteelDoorBottom:
              case Block.LockedDoorBottom:
                num3 = 1;
                break;
              default:
                num3 = this.map.IsBlockPassable((byte) blockId2) ? 1 : 0;
                break;
            }
            flag3 = num3 != 0;
          }
          box.Max.Y -= num2;
          if (flag3)
          {
            this.InterpolatePositionToTopOfBox(blockBox, movingX);
            return;
          }
        }
      }
      if (this.instance.CheckPointsToIgnore(p))
        return;
      bool flag4 = (double) normal.Y != 0.0 && !this.isOnRope && this.FlyMode == FlyMode.None;
      if (flag1)
      {
        if (this.isFlightDescending && (double) this.Velocity.Y < 0.0)
        {
          this.FlyMode = FlyMode.None;
          this.isFlightAscending = this.isFlightDescending = false;
          flag4 = false;
        }
        if ((double) this.Gravity < 0.0 && (double) this.Velocity.Y < (double) this.Gravity && this.LandingSoundDelay < 1)
        {
          this.PlayWalkSound(p);
          this.LandingSoundDelay = 20;
        }
      }
      else if (this.HittingBlockOnXZ.Y == 0)
        this.HittingBlockOnXZ = p;
      bool flag5 = (double) normal.X != 0.0 && ((double) this.Velocity.X > 0.0 && (double) this.Position.X > (double) blockBox.Max.X || (double) this.Velocity.X < 0.0 && (double) this.Position.X < (double) blockBox.Min.X) || (double) normal.Y != 0.0 && ((double) this.Velocity.Y > 0.0 && (double) this.Position.Y > (double) blockBox.Max.Y || (double) this.Velocity.Y < 0.0 && (double) this.Position.Y < (double) blockBox.Min.Y) || (double) normal.Z != 0.0 && ((double) this.Velocity.Z > 0.0 && (double) this.Position.Z > (double) blockBox.Max.Z || (double) this.Velocity.Z < 0.0 && (double) this.Position.Z < (double) blockBox.Min.Z);
      float num = Vector3.Dot(normal, this.Velocity);
      if ((double) normal.X != 0.0 && !flag5)
      {
        normal.X *= num;
        this.Velocity.X -= normal.X;
        this.accVel.X = 0.0f;
      }
      if ((double) normal.Y != 0.0 && !flag5)
      {
        normal.Y *= num;
        this.Velocity.Y -= normal.Y;
        this.accVel.Y = 0.0f;
      }
      if ((double) normal.Z != 0.0 && !flag5)
      {
        normal.Z *= num;
        this.Velocity.Z -= normal.Z;
        this.accVel.Z = 0.0f;
      }
      if (!flag4)
        return;
      float groundDamage = (double) normal.Y < 0.0 ? this.GetGroundDamage(blockID) : 0.0f;
      if (flag1 && (double) normal.Y == (double) gravity && (double) groundDamage <= 0.0)
        return;
      this.AddCollisionDamage(blockID, normal, groundDamage);
    }

    private void ClipVelocityRamp(GlobalPoint3D p, Vector3 normal, BoundingBox box)
    {
      BoundingBox blockBox = this.instance.GetBlockBox(p);
      if (!box.Intersects(blockBox))
        return;
      bool flag1 = (double) normal.Y != 0.0;
      blockBox.Max.Y = this.GetRampHeight(p, box);
      if (!box.Intersects(blockBox) || this.instance.CheckPointsToIgnore(p))
        return;
      bool flag2 = (double) normal.Y != 0.0 && !this.isOnRope && this.FlyMode == FlyMode.None;
      if ((double) normal.Y != 0.0 && this.isFlightDescending && (double) this.Velocity.Y < 0.0)
      {
        this.FlyMode = FlyMode.None;
        this.isFlightAscending = this.isFlightDescending = false;
        flag2 = false;
      }
      float num = Vector3.Dot(normal, this.Velocity);
      normal *= num;
      if ((double) blockBox.Max.Y < (double) box.Min.Y + 0.100000001490116 - (flag1 ? (double) this.Velocity.Y : 0.0))
      {
        this.Position.Y = blockBox.Max.Y;
        this.Velocity.Y = 0.0f;
      }
      else
        this.Velocity -= normal;
      if (!flag2)
        return;
      this.AddCollisionDamage(Block.Ramp, normal, 0.0f);
    }

    private float GetRampHeight(GlobalPoint3D p, BoundingBox box)
    {
      byte aux = (byte) ((uint) this.map.GetAuxData(p) & 3U);
      return Math.Max(Math.Max(Math.Max(this.GetRampHeight(p, box.Min, aux), this.GetRampHeight(p, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), aux)), this.GetRampHeight(p, new Vector3(box.Max.X, box.Min.Y, box.Max.Z), aux)), this.GetRampHeight(p, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), aux));
    }

    private float GetRampHeight(GlobalPoint3D p, Vector3 pos, byte aux)
    {
      Vector3 position = this.map.GetPosition(p);
      float num = position.Y - this.map.TileSize;
      switch (aux)
      {
        case 0:
          if ((double) pos.X <= (double) position.X)
            return num;
          if ((double) pos.X >= (double) position.X + (double) this.map.TileSize)
            return position.Y;
          return pos.X - position.X + num;
        case 1:
          if ((double) pos.Z <= (double) position.Z)
            return num;
          if ((double) pos.Z >= (double) position.Z + (double) this.map.TileSize)
            return position.Y;
          return pos.Z - position.Z + num;
        case 2:
          if ((double) pos.X >= (double) position.X + (double) this.map.TileSize)
            return num;
          if ((double) pos.X <= (double) position.X)
            return position.Y;
          return position.X + this.map.TileSize - pos.X + num;
        case 3:
          if ((double) pos.Z >= (double) position.Z + (double) this.map.TileSize)
            return num;
          if ((double) pos.Z <= (double) position.Z)
            return position.Y;
          return position.Z + this.map.TileSize - pos.Z + num;
        default:
          return position.Y;
      }
    }

    private void AddCollisionDamage(Block blockID, Vector3 normal, float groundDamage)
    {
      if ((double) normal.Y == 0.0 || this.isOnRope)
        return;
      float num1 = this.ModifyCollisionDamage(Math.Abs(normal.Y), normal, blockID);
      if (this.instance.IsLegendaryDifficulty)
        num1 *= 1.25f;
      float num2 = 0.3f;
      float damage;
      if ((double) num1 >= (double) num2)
      {
        if ((double) normal.Y > 0.0 && this.positionInterpolator.IsActive)
        {
          this.positionInterpolator.Reset();
          damage = 0.0f;
        }
        else
        {
          float num3 = 0.6f;
          damage = MathHelper.Lerp(2f, this.MaxHealth, Math.Min(num3 - num2, num1 - num2) * (float) (1.0 / ((double) num3 - (double) num2)));
        }
      }
      else
        damage = 0.0f;
      if ((double) normal.Y < 0.0)
        damage = (damage + groundDamage) * this.LandingDamageMultiplier;
      else if ((double) normal.Y > 0.0)
        damage += this.AddHeadDamage(blockID);
      if ((double) damage <= 0.0)
        return;
      double damageAndDisplay = (double) this.TakeDamageAndDisplay(DamageType.BlockCollision, damage, Vector3.Zero);
    }

    protected virtual float ModifyCollisionDamage(float damage, Vector3 dir, Block blockID)
    {
      return damage * this.LandingDamageMultiplier;
    }

    protected float GetGroundDamage(Block blockID)
    {
      if (blockID == Block.SteelSpikes && this.IsCanTouchOnTestY(GlobalPoint3D.Zero, Block.SteelSpikes))
        return this.MaxHealth + 1f;
      return 0.0f;
    }

    protected virtual float AddHeadDamage(Block blockID)
    {
      if (blockID == Block.SteelSpikes && this.IsCanTouchOnTestY(GlobalPoint3D.Zero, Block.SteelSpikes))
        return this.MaxHealth + 1f;
      return 0.0f;
    }

    private void InterpolatePositionToTopOfBox(BoundingBox blockBox, bool movingX)
    {
      Vector3 velocity = this.Velocity;
      if (movingX)
      {
        if ((double) Math.Abs(velocity.X) < 0.100000001490116)
          velocity.X = 0.1f * (float) Math.Sign(velocity.X);
      }
      else if ((double) Math.Abs(velocity.Z) < 0.100000001490116)
        velocity.Z = 0.1f * (float) Math.Sign(velocity.Z);
      this.positionInterpolator.Start(this.Position, new Vector3()
      {
        X = this.Position.X + velocity.X,
        Y = blockBox.Max.Y + 0.01f,
        Z = this.Position.Z + velocity.Z
      }, 0.109999999403954, true);
    }

    protected virtual bool IsCollisionPassable(GlobalPoint3D p)
    {
      Block blockId = (Block) this.map.GetBlockID(p);
      if (ItemData.IsSubType((Item) blockId, ItemSubType.Door))
        return !this.Box.Intersects(blockId == Block.TrapDoor ? this.instance.GetTrapDoorBox(p) : this.instance.GetDoorBox(p));
      return this.map.IsPassable(p);
    }

    public bool IsFloatingInWater
    {
      get
      {
        return this.IsFloatingInLiquid(Block.Water);
      }
    }

    public bool IsAboveCloudLevel
    {
      get
      {
        return (double) this.EyePosition.Y >= (double) this.instance.CloudHeight;
      }
    }

    protected bool IsUnderLiquid(Block liquidID)
    {
      Vector3 eyePosition = this.EyePosition;
      GlobalPoint3D point = this.map.GetPoint(eyePosition);
      if (this.map.IsValidPoint(point) && (Block) this.map.GetBlockID(point) == liquidID)
      {
        if (point.Y < this.map.MapBound.Max.Y - 1)
        {
          ++point.Y;
          if ((Block) this.map.GetBlockID(point) == liquidID)
            return true;
          --point.Y;
        }
        float liquidTopY = VoxelMeshBuilder.GetLiquidTopY((Map) this.map, eyePosition);
        if ((double) eyePosition.Y <= (double) liquidTopY + 0.0829999968409538)
        {
          if ((double) eyePosition.Y >= (double) liquidTopY - 0.068000003695488)
            this.duckHead = liquidTopY - 0.068f;
          return true;
        }
      }
      return false;
    }

    protected virtual bool IsFloatingInLiquid(Block liquidID)
    {
      Vector3 eyePosition = this.EyePosition;
      eyePosition.Y -= this.EyeOffset.Y * 0.35f;
      return (Block) this.map.GetBlockID(this.map.GetPoint(eyePosition)) == liquidID;
    }

    public bool IsInCloud
    {
      get
      {
        bool flag = this.IsInCloudCore((Map) this.map);
        if (!flag && Globals2.GameSettings.ViewClouds)
          flag = this.instance.CloudMapManager.IsCharacterInCloud(this);
        return flag;
      }
    }

    private bool IsInCloudCore(Map map)
    {
      if (map == null)
        return false;
      return map.GetBlockID(this.EyePosition) == (byte) 10;
    }

    protected bool StandingNextTo(Block block)
    {
      Vector3 position = this.Position;
      position.Y += 0.1f;
      GlobalPoint3D point = this.map.GetPoint(position);
      if (this.map.IsValidPoint(point) && this.StandingNextTo(point, block))
        return true;
      if ((int) (position.Y + this.EyeOffset.Y) > point.Y)
      {
        ++point.Y;
        if (this.map.IsValidPoint(point) && this.StandingNextTo(point, block))
          return true;
      }
      return false;
    }

    protected Block IsStandingNextToAtleastOneUnpassableBlock()
    {
      Vector3 position = this.Position;
      position.Y += 0.1f;
      GlobalPoint3D point = this.map.GetPoint(position);
      Block unpassable1 = (Block) this.StandingNextToUnpassable(point);
      if (unpassable1 != Block.zLastBlockID)
        return unpassable1;
      if ((int) (position.Y + this.EyeOffset.Y) > point.Y)
      {
        ++point.Y;
        Block unpassable2 = (Block) this.StandingNextToUnpassable(point);
        if (unpassable2 != Block.zLastBlockID)
          return unpassable2;
      }
      return Block.None;
    }

    protected bool StandingNextTo(GlobalPoint3D p, Block block)
    {
      if (p.X > this.map.MapBound.Min.X)
      {
        --p.X;
        if ((Block) this.map.GetBlockID(p) == block)
          return true;
        ++p.X;
      }
      if (p.X < this.map.MapBound.Max.X - 1)
      {
        ++p.X;
        if ((Block) this.map.GetBlockID(p) == block)
          return true;
        --p.X;
      }
      if (p.Z > this.map.MapBound.Min.Z)
      {
        --p.Z;
        if ((Block) this.map.GetBlockID(p) == block)
          return true;
        ++p.Z;
      }
      if (p.Z < this.map.MapBound.Max.Z - 1)
      {
        ++p.Z;
        if ((Block) this.map.GetBlockID(p) == block)
          return true;
        --p.Z;
      }
      return false;
    }

    protected byte StandingNextToUnpassable(GlobalPoint3D p)
    {
      if (p.X > this.map.MapBound.Min.X)
      {
        --p.X;
        byte blockId = this.map.GetBlockID(p);
        if (!this.map.BlockData[(int) blockId].IsPassable)
          return blockId;
        ++p.X;
      }
      if (p.X < this.map.MapBound.Max.X - 1)
      {
        ++p.X;
        byte blockId = this.map.GetBlockID(p);
        if (!this.map.BlockData[(int) blockId].IsPassable)
          return blockId;
        --p.X;
      }
      if (p.Z > this.map.MapBound.Min.Z)
      {
        --p.Z;
        byte blockId = this.map.GetBlockID(p);
        if (!this.map.BlockData[(int) blockId].IsPassable)
          return blockId;
        ++p.Z;
      }
      if (p.Z < this.map.MapBound.Max.Z - 1)
      {
        ++p.Z;
        byte blockId = this.map.GetBlockID(p);
        if (!this.map.BlockData[(int) blockId].IsPassable)
          return blockId;
        --p.Z;
      }
      return 0;
    }

    public virtual void OnCollision(Actor other, Vector3 displacement)
    {
      this.KnockForce = displacement;
    }

    public void DisableSwingTarget(float time)
    {
      this.disableSwingTargetTimer = time;
    }

    public void CalcSwingTarget(int reach)
    {
      if (this.IsPlayer)
        this.HitTarget.Clear();
      this.CalcSwingTarget(this.ViewDirection, this.IsGod ? 32f : (float) reach, false, false, this.LeftHand.ItemID == Item.Bucket || this.RightHand.ItemID == Item.Bucket, false);
    }

    protected void CalcSwingTarget(
      Vector3 dir,
      float range,
      bool solidBlocksOnly,
      bool ignoreIcons,
      bool isBucket,
      bool calcFacePos)
    {
      this.lastSwingTarget = this.SwingTarget;
      this.lastSwingTargetDistance = this.SwingTargetDistance;
      this.SwingFacePos = 4;
      this.SwingFace = BlockFace.ProxyDefault;
      this.SwingTargetDistance = float.MaxValue;
      if ((double) this.disableSwingTargetTimer > 0.0)
      {
        this.disableSwingTargetTimer -= Services.ElapsedTime;
      }
      else
      {
        Vector3 viewMatrixPosition = this.viewMatrixPosition;
        HitTest result = this.instance.CalcBlockTarget(viewMatrixPosition, dir, range, this.nonSwingTargets, solidBlocksOnly, ignoreIcons, this.isOnRope, isBucket);
        if (result.IsValid && (double) result.Distance < (double) this.HitTarget.Distance)
        {
          this.SwingTarget = result.Point;
          this.SwingFacePos = result.FacePos;
          this.SwingTargetDistance = result.Distance;
          this.CollateBlockTargetResult(this.instance.GetCalcBlockTargetRay(viewMatrixPosition, dir), this.instance.GetBlockBox(result.Point), result);
        }
        else
        {
          this.SwingFace = BlockFace.ProxyDefault;
          this.SwingTarget.X = this.SwingTarget.Y = this.SwingTarget.Z = int.MaxValue;
        }
        if (!(this.SwingTarget != this.lastSwingTarget))
          return;
        this.OnSwingTargetChanged();
      }
    }

    protected virtual void OnSwingTargetChanged()
    {
      this.SplinterProgress = 0.0f;
      this.Splinter = -1;
    }

    protected virtual void CollateBlockTargetResult(Ray ray, BoundingBox box, HitTest result)
    {
      this.CalcSwingFaceAndPos(ray, box, result.Point);
      this.SwingTargetBox = this.GetBoxFace(box, this.SwingFace);
      this.PlaceTarget = this.GetPlaceTargetFromSwingFace(result.Point, this.SwingFace);
    }

    protected GlobalPoint3D GetPlaceTargetFromSwingFace(
      GlobalPoint3D p,
      BlockFace face)
    {
      switch (face)
      {
        case BlockFace.Left:
          return p.GetLeft(1);
        case BlockFace.Forward:
          return p.GetForward(1);
        case BlockFace.Right:
          return p.GetRight(1);
        case BlockFace.Backward:
          return p.GetBackward(1);
        case BlockFace.Down:
          return p.GetDown(1);
        default:
          return p.GetUp(1);
      }
    }

    private void CalcSwingFaceAndPos(Ray ray, BoundingBox box, GlobalPoint3D p)
    {
      this.SwingFace = BlockFace.ProxyDefault;
      float maxValue = float.MaxValue;
      BoundingBox box1 = this.GetBoxFace(box, BlockFace.Left);
      float? nullable1 = ray.Intersects(box1);
      if (nullable1.HasValue && (double) nullable1.Value < (double) maxValue)
      {
        maxValue = nullable1.Value;
        this.SwingFace = BlockFace.Left;
        box1 = this.instance.GetBlockBoxCore(p);
      }
      BoundingBox box2 = this.GetBoxFace(box, BlockFace.Forward);
      float? nullable2 = ray.Intersects(box2);
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        maxValue = nullable2.Value;
        this.SwingFace = BlockFace.Forward;
        box2 = this.instance.GetBlockBoxCore(p);
      }
      BoundingBox box3 = this.GetBoxFace(box, BlockFace.Right);
      nullable2 = ray.Intersects(box3);
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        maxValue = nullable2.Value;
        this.SwingFace = BlockFace.Right;
        box3 = this.instance.GetBlockBoxCore(p);
      }
      BoundingBox box4 = this.GetBoxFace(box, BlockFace.Backward);
      nullable2 = ray.Intersects(box4);
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        maxValue = nullable2.Value;
        this.SwingFace = BlockFace.Backward;
        box4 = this.instance.GetBlockBoxCore(p);
      }
      BoundingBox box5 = this.GetBoxFace(box, BlockFace.Up);
      nullable2 = ray.Intersects(box5);
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        maxValue = nullable2.Value;
        this.SwingFace = BlockFace.Up;
        box5 = this.instance.GetBlockBoxCore(p);
      }
      BoundingBox box6 = this.GetBoxFace(box, BlockFace.Down);
      nullable2 = ray.Intersects(box6);
      if (nullable2.HasValue && (double) nullable2.Value < (double) maxValue)
      {
        float num = nullable2.Value;
        this.SwingFace = BlockFace.Down;
        box6 = this.instance.GetBlockBoxCore(p);
      }
      float halfTileSize = this.map.HalfTileSize;
      float num1 = halfTileSize * 0.5f;
      switch (this.SwingFace)
      {
        case BlockFace.Left:
          float y1 = box1.Min.Y;
          float z1 = box1.Min.Z;
          box1.Max.X = box1.Min.X;
          box1.Max.Y -= halfTileSize;
          box1.Max.Z -= halfTileSize;
          if (ray.Intersects(box1).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box1.Min.Y += halfTileSize;
            box1.Max.Y += halfTileSize;
            if (ray.Intersects(box1).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box1.Min.Z += halfTileSize;
              box1.Max.Z += halfTileSize;
              this.SwingFacePos = !ray.Intersects(box1).HasValue ? 3 : 2;
            }
          }
          box1.Min.Y = y1 + num1;
          box1.Max.Y = y1 + halfTileSize + num1;
          box1.Min.Z = z1 + num1;
          box1.Max.Z = z1 + halfTileSize + num1;
          if (!ray.Intersects(box1).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
        case BlockFace.Forward:
          float x1 = box2.Min.X;
          float y2 = box2.Min.Y;
          box2.Min.X += halfTileSize;
          box2.Max.Y -= halfTileSize;
          box2.Max.Z = box2.Min.Z;
          if (ray.Intersects(box2).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box2.Min.Y += halfTileSize;
            box2.Max.Y += halfTileSize;
            if (ray.Intersects(box2).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box2.Min.X -= halfTileSize;
              box2.Max.X -= halfTileSize;
              this.SwingFacePos = !ray.Intersects(box2).HasValue ? 3 : 2;
            }
          }
          box2.Min.X = x1 + num1;
          box2.Max.X = x1 + halfTileSize + num1;
          box2.Min.Y = y2 + num1;
          box2.Max.Y = y2 + halfTileSize + num1;
          if (!ray.Intersects(box2).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
        case BlockFace.Right:
          float y3 = box3.Min.Y;
          float z2 = box3.Min.Z;
          box3.Min.X = box3.Max.X;
          box3.Max.Y -= halfTileSize;
          box3.Min.Z += halfTileSize;
          if (ray.Intersects(box3).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box3.Min.Y += halfTileSize;
            box3.Max.Y += halfTileSize;
            if (ray.Intersects(box3).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box3.Min.Z -= halfTileSize;
              box3.Max.Z -= halfTileSize;
              this.SwingFacePos = !ray.Intersects(box3).HasValue ? 3 : 2;
            }
          }
          box3.Min.Y = y3 + num1;
          box3.Max.Y = y3 + halfTileSize + num1;
          box3.Min.Z = z2 + num1;
          box3.Max.Z = z2 + halfTileSize + num1;
          if (!ray.Intersects(box3).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
        case BlockFace.Backward:
          float x2 = box4.Min.X;
          float y4 = box4.Min.Y;
          box4.Max.X -= halfTileSize;
          box4.Max.Y -= halfTileSize;
          box4.Min.Z = box4.Max.Z;
          if (ray.Intersects(box4).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box4.Min.Y += halfTileSize;
            box4.Max.Y += halfTileSize;
            if (ray.Intersects(box4).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box4.Min.X += halfTileSize;
              box4.Max.X += halfTileSize;
              this.SwingFacePos = !ray.Intersects(box4).HasValue ? 3 : 2;
            }
          }
          box4.Min.X = x2 + num1;
          box4.Max.X = x2 + halfTileSize + num1;
          box4.Min.Y = y4 + num1;
          box4.Max.Y = y4 + halfTileSize + num1;
          if (!ray.Intersects(box4).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
        case BlockFace.Up:
          float x3 = box5.Min.X;
          float z3 = box5.Min.Z;
          box5.Max.X -= halfTileSize;
          box5.Min.Y = box5.Max.Y;
          box5.Max.Z -= halfTileSize;
          if (ray.Intersects(box5).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box5.Min.X += halfTileSize;
            box5.Max.X += halfTileSize;
            if (ray.Intersects(box5).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box5.Min.Z += halfTileSize;
              box5.Max.Z += halfTileSize;
              this.SwingFacePos = !ray.Intersects(box5).HasValue ? 3 : 2;
            }
          }
          box5.Min.X = x3 + num1;
          box5.Max.X = x3 + halfTileSize + num1;
          box5.Min.Z = z3 + num1;
          box5.Max.Z = z3 + halfTileSize + num1;
          if (!ray.Intersects(box5).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
        case BlockFace.Down:
          float x4 = box6.Min.X;
          float z4 = box6.Min.Z;
          box6.Max.X -= halfTileSize;
          box6.Max.Y = box6.Min.Y;
          box6.Max.Z -= halfTileSize;
          if (ray.Intersects(box6).HasValue)
          {
            this.SwingFacePos = 0;
          }
          else
          {
            box6.Min.X += halfTileSize;
            box6.Max.X += halfTileSize;
            if (ray.Intersects(box6).HasValue)
            {
              this.SwingFacePos = 1;
            }
            else
            {
              box6.Min.Z += halfTileSize;
              box6.Max.Z += halfTileSize;
              this.SwingFacePos = !ray.Intersects(box6).HasValue ? 3 : 2;
            }
          }
          box6.Min.X = x4 + num1;
          box6.Max.X = x4 + halfTileSize + num1;
          box6.Min.Z = z4 + num1;
          box6.Max.Z = z4 + halfTileSize + num1;
          if (!ray.Intersects(box6).HasValue)
            break;
          this.SwingFacePos += 4;
          break;
      }
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

    private bool IsPassableForSwingTarget(GlobalPoint3D p)
    {
      if (!this.map.IsValidPoint(p))
        return false;
      Block blockId = (Block) this.map.GetBlockID(p);
      if (this.map.IsBlockPassable((byte) blockId))
        return blockId != Block.Teleport;
      return false;
    }

    private BoundingBox GetArrangedBox(Vector3 min, Vector3 max)
    {
      if ((double) min.X > (double) max.X)
      {
        float x = min.X;
        min.X = max.X;
        max.X = x;
      }
      if ((double) min.Y > (double) max.Y)
      {
        float y = min.Y;
        min.Y = max.Y;
        max.Y = y;
      }
      if ((double) min.Z > (double) max.Z)
      {
        float z = min.Z;
        min.Z = max.Z;
        max.Z = z;
      }
      return new BoundingBox(min, max);
    }

    public bool SwingTargetIsValid
    {
      get
      {
        return this.SwingFace != BlockFace.ProxyDefault;
      }
    }

    public bool PlaceBlock(Hand hand)
    {
      if (this.IsClipboardEquipped && (hand.HandType == InventoryHand.Left || this.WieldType == WieldType.RightHand))
      {
        this.PasteClipboardModel(Map.CopyType.Overwrite);
      }
      else
      {
        Block block = Block.None;
        this.CalcSwingTarget(this.reach);
        if (this.SwingTargetIsValid)
          block = (Block) this.map.GetBlockID(this.SwingTarget);
        if (!ItemData.IsSubType((Item) block, ItemSubType.BlockCanBeOpened))
        {
          if (this.PlaceTargetIsValid(hand))
            this.PlaceBlockCore(hand);
          return true;
        }
      }
      return false;
    }

    private bool PlaceBlockCore(Hand hand)
    {
      if (this.HasPermission(Permissions.Edit, true))
      {
        InventoryItem inventoryItem = this.Inventory[hand.HandIndexOnSwingStart];
        Item itemIdRaw = inventoryItem.ItemID_Raw;
        if (this.SkillsData.UseReqsMet(this, itemIdRaw))
        {
          InventoryItem blockId = this.instance.ConvertItemIDToBlockID(inventoryItem);
          if (blockId.ItemID != Item.None && blockId.ItemID < Item.zLastBlockID)
          {
            Block itemId = (Block) blockId.ItemID;
            byte auxData = 0;
            object tagData = (object) null;
            if (inventoryItem.ItemID == Item.Book)
              tagData = (object) inventoryItem.Durability;
            if (this.instance.AddBlock(this.GetPlaceTarget(hand), itemId, out auxData, UpdateBlockMethod.Player, this.GamerID, hand.AutoTrigger, true, true, this.SwingTarget, this.SwingFace, this.SwingFacePos, itemIdRaw, tagData))
            {
              this.OnBlockPlaced(itemIdRaw, itemId, auxData, hand);
              this.OnItemUsed(hand);
              this.SkillsData.BlockPlaced(this, itemIdRaw, auxData);
              Sounds.PlaySound(itemId != Block.ArcadeMachine ? (Item) this.map.GetBlockTextureIDForDrawing(itemId, this.PlaceTarget) : Item.ArcadeMachine, ItemSoundType.Use, this.PlaceTarget, (ITMActor) this);
              return true;
            }
          }
        }
      }
      return false;
    }

    protected virtual void OnBlockPlaced(Item itemRawID, Block blockID, byte auxData, Hand hand)
    {
    }

    private GlobalPoint3D GetPlaceTarget(Hand hand)
    {
      if (hand.ItemID == Item.Bucket)
      {
        switch (hand.OtherHand.ItemID)
        {
          case Item.Water:
          case Item.Lava:
          case Item.BucketOfWater:
          case Item.BucketOfLava:
            switch ((Block) this.map.GetBlockID(this.SwingTarget))
            {
              case Block.Water:
              case Block.Lava:
                return this.SwingTarget;
              default:
                return this.PlaceTarget;
            }
        }
      }
      else if (hand.ItemID == Item.SnowLayer)
      {
        if (this.SwingFace == BlockFace.Up && this.map.GetBlockID(this.SwingTarget) == (byte) 145 && this.map.GetAuxData(this.SwingTarget) < (byte) 7)
          return this.SwingTarget;
      }
      else if (hand.ItemID == Item.Stack || hand.ItemID == Item.Stack2)
      {
        if (this.SwingFace == BlockFace.Down)
        {
          if (this.map.GetBlockID(this.SwingTarget) == (byte) 131 && this.map.GetAuxData(this.SwingTarget) < (byte) 7)
            return this.SwingTarget;
        }
        else if (this.SwingFace == BlockFace.Up)
        {
          switch ((Block) this.map.GetBlockID(this.SwingTarget))
          {
            case Block.Stack:
            case Block.Stack2:
              if (this.map.GetAuxData(this.SwingTarget) < (byte) 7)
                return this.SwingTarget;
              break;
          }
        }
      }
      else if (hand.ItemID == Item.UpsideDownStack)
      {
        if (this.SwingFace == BlockFace.Up)
        {
          switch ((Block) this.map.GetBlockID(this.SwingTarget))
          {
            case Block.Stack:
            case Block.Stack2:
              if (this.map.GetAuxData(this.SwingTarget) < (byte) 7)
                return this.SwingTarget;
              break;
          }
        }
        else if (this.SwingFace == BlockFace.Down && this.map.GetBlockID(this.SwingTarget) == (byte) 131 && this.map.GetAuxData(this.SwingTarget) < (byte) 7)
          return this.SwingTarget;
      }
      return this.PlaceTarget;
    }

    public bool PlaceTargetIsValid(Hand hand)
    {
      if (hand.ItemID == Item.Clipboard)
        return true;
      if (this.HitTarget.Target != null || !this.SwingTargetIsValid)
        return false;
      InventoryItem blockId1 = this.instance.ConvertItemIDToBlockID(this.Inventory[hand.HandIndex]);
      if (blockId1.ItemID >= Item.zLastBlockID && blockId1.ItemID != Item.GoldPieces)
        return false;
      Block block1 = (Block) this.map.GetBlockID(this.SwingTarget);
      switch (block1)
      {
        case Block.Stairs2:
          block1 = Block.Stairs;
          break;
        case Block.HalfBlock2:
          block1 = Block.HalfBlock;
          break;
        case Block.Ramp2:
          block1 = Block.Ramp;
          break;
        case Block.Stack2:
          block1 = Block.Stack;
          break;
        case Block.Post2:
          block1 = Block.Post;
          break;
        case Block.SidePost2:
          block1 = Block.SidePost;
          break;
        case Block.CornerBlock2:
          block1 = Block.CornerBlock;
          break;
        case Block.MultiTextureBlock2:
          block1 = Block.MultiTextureBlock;
          break;
      }
      if (ItemData.IsSubType((Item) block1, ItemSubType.BlockCanBeOpened))
        return true;
      Item obj = blockId1.ItemID;
      switch (obj)
      {
        case Item.Stack2:
          obj = Item.Stack;
          break;
        case Item.Post2:
          obj = Item.Post;
          break;
        case Item.SidePost2:
          obj = Item.SidePost;
          break;
        case Item.CornerBlock2:
          obj = Item.CornerBlock;
          break;
        case Item.MultiTextureBlock2:
          obj = Item.MultiTextureBlock;
          break;
        case Item.Stairs2Icon:
          obj = Item.StairsIcon;
          break;
        case Item.HalfBlock2Icon:
          obj = Item.HalfBlockIcon;
          break;
        case Item.Ramp2Icon:
          obj = Item.RampIcon;
          break;
      }
      Block block2 = (Block) obj;
      if (obj == Item.GoldPieces)
        return block1 == Block.ArcadeMachine;
      byte blockID = (byte) block2;
      if (!this.map.BlockData[(int) blockID].IsPassable && this.instance.IsInAnyPlayerSpace(this.PlaceTarget) || block1 == Block.Book)
        return false;
      Block blockId2 = (Block) this.map.GetBlockID(this.PlaceTarget);
      if (block2 == Block.Marker || block2 == Block.ExcludeMarker)
      {
        if (blockId2 != Block.None && blockId2 != Block.Water)
          return blockId2 == Block.Lava;
        return true;
      }
      byte auxData = this.map.GetAuxData(this.SwingTarget);
      if (auxData < (byte) 7)
      {
        if (block1 == Block.SnowLayer && this.SwingFace == BlockFace.Up)
          return block2 == Block.SnowLayer;
        if (block1 == Block.Stack && this.SwingFace == BlockFace.Up)
        {
          if (block2 != Block.Stack)
            return block2 == Block.UpsideDownStack;
          return true;
        }
        if (block1 == Block.UpsideDownStack && this.SwingFace == BlockFace.Down)
        {
          if (block2 != Block.UpsideDownStack)
            return block2 == Block.Stack;
          return true;
        }
      }
      switch (blockId2)
      {
        case Block.None:
        case Block.Cloud:
        case Block.Water:
        case Block.Lava:
          if (block2 != blockId2 && (blockId2 == Block.Water || blockId2 == Block.Lava) && (this.map.BlockData[(int) blockID].Buffer > (byte) 1 || this.map.IsBlockPassable(blockID) || ItemData.IsSubType((Item) blockId2, ItemSubType.Door)))
            return false;
          int buffer = (int) this.map.BlockData[(int) block2].Buffer;
          int num = (int) this.map.BlockData[(int) block1].Buffer;
          Block block3 = block1;
          if ((uint) block3 <= 125U)
          {
            switch (block3)
            {
              case Block.Leaves:
              case Block.Glass:
              case Block.PineLeaves:
              case Block.InvisibleBarrier:
                break;
              default:
                goto label_62;
            }
          }
          else
          {
            switch (block3)
            {
              case Block.Scaffold:
              case Block.SteelPortcullis:
              case Block.MapleLeaves:
              case Block.StainedGlass:
                break;
              case Block.Ramp:
                if (!this.map.IsBlockIcon((byte) block2) && !this.map.IsBlockAttachable((byte) block2) && (this.SwingFace == BlockFace.Up && auxData < (byte) 4 || this.SwingFace == BlockFace.Down && auxData > (byte) 3))
                  return true;
                goto label_62;
              default:
                goto label_62;
            }
          }
          num = 0;
label_62:
          Block block4 = block2;
          if ((uint) block4 <= (uint) sbyte.MaxValue)
          {
            if ((uint) block4 <= 72U)
            {
              switch (block4)
              {
                case Block.Torch:
                  if (this.SwingFace != BlockFace.Down)
                  {
                    if (num < 2 || block1 == Block.Cylinder)
                      return true;
                    if (this.SwingFace == BlockFace.Up)
                    {
                      if (block1 == Block.Fence || block1 == Block.Table || block1 == Block.Post && auxData == (byte) 0)
                        return true;
                      if (block1 == Block.Sign)
                        return auxData < (byte) 4;
                    }
                  }
                  return false;
                case Block.Ladder:
                  if (this.SwingFace != BlockFace.Up && this.SwingFace != BlockFace.Down)
                    return num < 2;
                  return false;
                case Block.WoodDoorTop:
                case Block.SteelDoorTop:
                  goto label_147;
                case Block.Wisdom:
                case Block.Blueprint:
                  return false;
                case Block.Sapling:
                case Block.WhiteFlowers:
                case Block.PurpleFlowers:
                case Block.RedFlowers:
                case Block.YellowFlowers:
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  if (num < 2 || block1 == Block.Table)
                    return true;
                  if (block1 == Block.Post)
                    return auxData == (byte) 0;
                  return false;
                case Block.Rope:
                  if (this.SwingFace == BlockFace.Down && blockId2 != Block.Rope)
                    return num < 2;
                  return false;
                default:
                  goto label_170;
              }
            }
            else
            {
              switch (block4)
              {
                case Block.LongGrass:
                  break;
                case Block.Sign:
                  if (this.SwingFace == BlockFace.Down)
                    return false;
                  if (num < 2 || block1 == Block.Stairs || block1 == Block.HalfBlock)
                    return true;
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  switch (block1)
                  {
                    case Block.Post:
                      return auxData == (byte) 0;
                    case Block.Sign:
                    case Block.Fence:
                    case Block.Table:
                      return true;
                    default:
                      return false;
                  }
                case Block.SteelSpikes:
                  if (this.SwingFace == BlockFace.Down && num < 2)
                    return true;
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  if (num >= 2)
                    return block1 == Block.Table;
                  return true;
                case Block.ClimbingIvy:
                  if (this.SwingFace == BlockFace.Up)
                    return false;
                  if (this.SwingFace == BlockFace.Down)
                    return block1 == Block.ClimbingIvy;
                  return num < 2;
                case Block.Book:
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  if (num >= 2)
                    return block1 == Block.Table;
                  return true;
                case Block.Key:
                  switch (block1)
                  {
                    case Block.LockedChest:
                    case Block.LockedDoorTop:
                    case Block.LockedDoorBottom:
                      return true;
                    default:
                      return this.SwingFace == BlockFace.Up;
                  }
                case Block.Crop:
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  return this.map.GetBlockID(this.PlaceTarget + GlobalPoint3D.Down) == (byte) 173;
                case Block.RedMushroom:
                  if (this.SwingFace != BlockFace.Up)
                    return false;
                  Block blockId3 = (Block) this.map.GetBlockID(this.PlaceTarget + GlobalPoint3D.Down);
                  if (!BlockData.IsGrassOrDirt(blockId3))
                    return blockId3 == Block.Wood;
                  return true;
                default:
                  goto label_170;
              }
            }
          }
          else if ((uint) block4 <= 160U)
          {
            switch (block4)
            {
              case Block.BedFoot:
                if (this.SwingFace != BlockFace.Up || !this.map.IsInsideMap(this.PlaceTarget, new Point3D(2, 2, 2)))
                  return false;
                GlobalPoint3D bedHeadOffset = this.instance.GetBedHeadOffset(this, this.PlaceTarget);
                if (this.instance.IsInAnyPlayerSpace(this.PlaceTarget + bedHeadOffset))
                  return false;
                return this.map.GetBlockID(this.PlaceTarget + bedHeadOffset) == (byte) 0;
              case Block.Fence:
                if (block1 == Block.Fence || num < 2)
                  return true;
                if (this.SwingFace != BlockFace.Up)
                  return false;
                if (block1 == Block.Sign || block1 == Block.Post && auxData == (byte) 0 || block1 == Block.Table)
                  return true;
                if (block1 == Block.HalfBlock)
                  return ((int) auxData & 3) == 1;
                return false;
              case Block.LockedDoorTop:
                goto label_147;
              case Block.Painting:
                if (this.SwingFace == BlockFace.Down)
                  return false;
                if (this.SwingFace == BlockFace.Up)
                {
                  switch (block1)
                  {
                    case Block.Stairs:
                    case Block.Ramp:
                      if (this.SwingFace == BlockFace.Up)
                        return auxData > (byte) 3;
                      break;
                    case Block.HalfBlock:
                      return auxData == (byte) 1;
                  }
                }
                if (num >= 2 && block1 != Block.Stairs && block1 != Block.Ramp)
                  return block1 == Block.HalfBlock;
                return true;
              default:
                goto label_170;
            }
          }
          else
          {
            switch (block4)
            {
              case Block.Switch:
              case Block.Button:
                if (num >= 2)
                {
                  if (this.SwingFace == BlockFace.Up)
                  {
                    switch (block1)
                    {
                      case Block.Post:
                        if (auxData == (byte) 0)
                          goto label_157;
                        else
                          break;
                      case Block.Table:
                        goto label_157;
                    }
                  }
                  if (this.SwingFace == BlockFace.Down && block1 == Block.Post)
                    return auxData == (byte) 0;
                  return false;
                }
label_157:
                return true;
              case Block.BerryBush:
                break;
              default:
                goto label_170;
            }
          }
          if (this.SwingFace != BlockFace.Up)
            return false;
          if (num >= 2)
            return block1 == Block.Table;
          return true;
label_147:
          if (this.SwingFace != BlockFace.Up || this.PlaceTarget.Y > this.map.MapBound.Max.Y - 2)
            return false;
          return this.map.GetBlockID(this.PlaceTarget + GlobalPoint3D.Up) == (byte) 0;
label_170:
          return true;
        case Block.Stack:
          if (block2 != Block.Stack)
            return block2 == Block.UpsideDownStack;
          return true;
        case Block.UpsideDownStack:
          if (block2 != Block.UpsideDownStack)
            return block2 == Block.Stack;
          return true;
        case Block.SnowLayer:
          return block2 == Block.SnowLayer;
        default:
          return false;
      }
    }

    public virtual bool IsClipboardEquipped
    {
      get
      {
        return false;
      }
    }

    protected virtual void PasteClipboardModel(Map.CopyType copyType)
    {
    }

    public void SetQtyPlaced(byte qty)
    {
      this.qtyPlaced = qty;
    }

    public Permissions Permission
    {
      get
      {
        return this.m_permission;
      }
      set
      {
        this.m_permission = value;
        this.OnPermissionChanged();
      }
    }

    public bool HasPermission(Permissions permission)
    {
      return this.HasPermission(permission, false);
    }

    public bool HasPermission(Permissions permission, bool notifyIfDenied)
    {
      bool flag = (this.m_permission & permission) == permission || this.IsGod || !this.IsPlayer;
      if (!flag && notifyIfDenied)
        this.NotifyPermissionDenied(permission);
      return flag;
    }

    public bool HasPermissionAny(Permissions permission)
    {
      return this.HasPermissionAny(permission, false);
    }

    public bool HasPermissionAny(Permissions permission, bool notifyIfDenied)
    {
      bool flag = (this.m_permission & permission) > Permissions.None || this.IsGod || !this.IsPlayer;
      if (!flag && notifyIfDenied)
        this.NotifyPermissionDenied(permission);
      return flag;
    }

    public void TogglePermission(Permissions permission)
    {
      this.TogglePermission(permission, (this.m_permission & permission) == Permissions.None);
    }

    public void TogglePermission(Permissions permission, bool enable)
    {
      if (enable)
      {
        if ((permission & Permissions.Creative) == Permissions.Creative)
          permission |= Permissions.Edit;
        if ((permission & Permissions.Edit) == Permissions.Edit)
          permission |= Permissions.Adventure;
        this.m_permission |= permission;
      }
      else
      {
        if ((permission & Permissions.Adventure) == Permissions.Adventure)
          permission |= Permissions.Edit;
        if ((permission & Permissions.Edit) == Permissions.Edit)
          permission |= Permissions.Creative;
        this.m_permission &= ~permission;
      }
      this.OnPermissionChanged();
    }

    protected virtual void OnPermissionChanged()
    {
    }

    protected virtual void NotifyPermissionDenied(Permissions permission)
    {
    }

    public int AddToInventory(Item itemID)
    {
      return this.AddToInventory(new InventoryItem(itemID, 1));
    }

    public int AddToInventory(Item itemID, int count)
    {
      return this.AddToInventory(new InventoryItem(itemID, count));
    }

    public int AddToInventory(InventoryItem newItem)
    {
      int slotID;
      return this.AddToInventory(newItem, out slotID);
    }

    public int AddToInventory(InventoryItem newItem, out int slotID)
    {
      slotID = -1;
      int num1 = 0;
      while (newItem.Count > 0 && newItem.ItemID_Raw != Item.None)
      {
        slotID = this.Inventory.FindOrGetFreeSlotForItem(newItem);
        if (slotID >= 0)
        {
          InventoryItem inventoryItem = this.Inventory[slotID];
          inventoryItem.ItemID = newItem.ItemID;
          int num2 = Math.Min(newItem.Count, ItemData.GetStackSize(newItem.ItemID) - inventoryItem.Count);
          newItem.Count -= num2;
          num1 += num2;
          inventoryItem.Count += num2;
          inventoryItem.Durability = newItem.Durability;
          this.Inventory[slotID] = inventoryItem;
          this.AddToInventoryCore(inventoryItem, slotID);
        }
        else
          break;
      }
      return num1;
    }

    protected virtual void AddToInventoryCore(InventoryItem item, int slotID)
    {
    }

    public bool OnItemUsed(Hand hand)
    {
      return this.OnItemUsed(hand, ItemUseType.General);
    }

    public bool OnItemUsed(Hand hand, ItemUseType useType)
    {
      if (this.instance.IsFiniteResources && hand.ItemID != Item.Hand)
        return this.OnItemUsedCore(hand.ItemID, hand.HandIndexOnSwingStart, useType);
      return false;
    }

    public bool OnItemUsed(int slotID)
    {
      if (this.instance.IsFiniteResources)
      {
        Item itemId = this.Inventory[slotID].ItemID;
        if (this.IsCorrectEquipSlot(itemId, slotID))
          return this.OnItemUsedCore(itemId, slotID, ItemUseType.General);
      }
      return false;
    }

    private bool OnItemUsedCore(Item itemID, int slotID, ItemUseType useType)
    {
      Item obj = itemID;
      if ((uint) obj <= 256U)
      {
        if (obj != Item.None && obj != Item.Hand)
          goto label_4;
      }
      else if (obj != Item.Bucket && obj != Item.Clipboard)
        goto label_4;
      return false;
label_4:
      if (this.Inventory[slotID].Durability > (ushort) 0 && itemID != Item.Book && (itemID != Item.Wisdom && itemID != Item.Blueprint) && itemID != Item.Key)
        return this.OnItemWithDurabilityUsed(slotID);
      return this.OnItemWithNoDurabilityUsed(slotID, useType);
    }

    protected bool IsCorrectEquipSlot(Item itemID, int slotID)
    {
      switch (Globals1.ItemTypeData[(int) itemID].Equip)
      {
        case EquipIndex.Head:
          return slotID == this.Inventory.HeadIndex;
        case EquipIndex.Neck:
          return slotID == this.Inventory.NeckIndex;
        case EquipIndex.Body:
          return slotID == this.Inventory.BodyIndex;
        case EquipIndex.Legs:
          return slotID == this.Inventory.LegsIndex;
        case EquipIndex.Feet:
          return slotID == this.Inventory.FeetIndex;
        case EquipIndex.LeftSide:
          return slotID == this.Inventory.LeftSideIndex;
        case EquipIndex.RightSide:
          return slotID == this.Inventory.RightSideIndex;
        case EquipIndex.LeftHand:
        case EquipIndex.RightHand:
          if (slotID != this.Inventory.LeftHandIndex)
            return slotID == this.Inventory.RightHandIndex;
          return true;
        default:
          return false;
      }
    }

    protected bool OnItemWithDurabilityUsed(int slotID)
    {
      if (this.Inventory[slotID].Durability <= (ushort) 0)
        return false;
      InventoryItem inventoryItem = this.Inventory[slotID];
      if (!this.IsGod)
      {
        --inventoryItem.Durability;
        this.Inventory[slotID] = inventoryItem;
        if (inventoryItem.Durability == (ushort) 0)
        {
          this.Inventory.DecrementItem(slotID, 1);
          if (slotID == this.Inventory.LeftHandIndex && this.LeftHand.ItemID == inventoryItem.ItemID)
            this.OnLeftHandItemDegraded();
          else if (slotID == this.Inventory.RightHandIndex && this.RightHand.ItemID == inventoryItem.ItemID)
            this.OnRightHandItemDegraded();
          if (this.IsPlayer && this.IsLocalGamer)
            this.instance.AddNotification("Your " + ItemData.ToString(inventoryItem.ItemID) + " has degraded", NotifyRecipient.Local);
          return false;
        }
      }
      return true;
    }

    protected bool OnItemWithNoDurabilityUsed(int slotID, ItemUseType useType)
    {
      if (useType == ItemUseType.General)
      {
        InventoryItem inventoryItem = this.Inventory[slotID];
        int count = 1;
        if (inventoryItem.ItemID < Item.zLastBlockID && this.IsBlockUseCurrentAux((Block) inventoryItem.ItemID))
          count = (int) this.qtyPlaced;
        if (this.Inventory.DecrementItem(slotID, count) == 0)
          this.EquipFromInventory(slotID, inventoryItem.ItemID);
        if (inventoryItem.ItemID == Item.BucketOfWater || inventoryItem.ItemID == Item.BucketOfLava)
          this.AddToInventory(Item.Bucket, 1);
      }
      return true;
    }

    public bool IsSingleWieldHandsBound
    {
      get
      {
        if (this.WieldType == WieldType.BothHands)
          return false;
        Hand hand = this.WieldType == WieldType.LeftHand ? this.LeftHand : this.RightHand;
        return !ItemData.IsSubTypeAny(hand.ItemID, ItemSubType.Bow | ItemSubType.GrenadeLauncher) && !ItemData.IsBindableWeapon(hand.ItemID);
      }
    }

    protected Hand GetEquipHand(Item itemID)
    {
      return this.GetEquipHand(ItemData.GetItemEquipIndex(itemID));
    }

    public Hand GetEquipHand(EquipIndex equipIndex)
    {
      if (equipIndex == EquipIndex.LeftHand)
        return this.LeftHand;
      if (equipIndex == EquipIndex.RightHand)
        return this.RightHand;
      return (Hand) null;
    }

    public void EquipBodyFromInventory()
    {
      if (this.Inventory.EquipIndexStart <= (short) 0)
        return;
      for (int inventoryIndex = 0; inventoryIndex < (int) this.Inventory.PackSize && inventoryIndex < this.Inventory.Count; ++inventoryIndex)
      {
        int equipSlotId = this.Inventory.GetEquipSlotID(this.Inventory[inventoryIndex].ItemID);
        if (equipSlotId >= (int) this.Inventory.EquipIndexStart)
          this.EquipFromInventory(inventoryIndex, equipSlotId);
      }
    }

    public bool EquipFromInventory(Hand hand, Item itemID)
    {
      if (hand.ItemID == itemID)
        return true;
      return this.EquipFromInventoryCore(hand, itemID, (int) this.Inventory.EquipIndexEnd);
    }

    public bool EquipFromInventory(int slotID, Item itemID)
    {
      return this.EquipFromInventoryCore(slotID == this.RightHand.HandIndex ? this.RightHand : this.LeftHand, itemID, (int) this.Inventory.PackSize);
    }

    public bool EquipFromInventory(Item itemID)
    {
      return this.EquipFromInventory(this.GetEquipHand(itemID), itemID);
    }

    protected bool EquipFromInventoryCore(Hand hand, Item itemID, int upperBound)
    {
      if (hand != null && hand.ItemID == itemID)
        return true;
      int inventoryIndex = this.Inventory.FindItem(itemID, true);
      if (inventoryIndex < 0)
        inventoryIndex = this.Inventory.FindItem(0, upperBound, itemID, false);
      return this.EquipFromInventory(hand, inventoryIndex, upperBound);
    }

    protected virtual void OnLeftHandItemDegraded()
    {
    }

    protected virtual void OnRightHandItemDegraded()
    {
    }

    public bool UnequipToInventory(int equipIndex)
    {
      if (equipIndex >= (int) this.Inventory.EquipIndexStart && equipIndex < (int) this.Inventory.EquipIndexEnd)
      {
        int inventoryIndex = this.Inventory.FindItem(Item.None, true);
        if (inventoryIndex >= 0)
          return this.EquipFromInventory(inventoryIndex, equipIndex);
      }
      return false;
    }

    public bool UnequipToInventory(EquipIndex equipIndex)
    {
      return this.UnequipToInventory(this.Inventory.GetEquipSlotID(equipIndex));
    }

    public bool EquipFromInventory(Hand hand, int inventoryIndex)
    {
      return this.EquipFromInventory(hand, inventoryIndex, (int) this.Inventory.PackSize);
    }

    public virtual bool EquipFromInventory(Hand hand, int inventoryIndex, int upperBound)
    {
      if (inventoryIndex >= 0 && inventoryIndex < upperBound)
      {
        InventoryItem inventoryItem = this.Inventory[inventoryIndex];
        EquipIndex itemEquipIndex = ItemData.GetItemEquipIndex(inventoryItem.ItemID);
        if (hand == null && itemEquipIndex != EquipIndex.LeftHand && (itemEquipIndex != EquipIndex.RightHand && itemEquipIndex != EquipIndex.None))
          return this.EquipFromInventory(inventoryIndex, (int) ((byte) this.Inventory.EquipIndexStart + itemEquipIndex - (byte) 1));
        if (hand == null)
          hand = this.GetEquipHand(itemEquipIndex);
        if (hand != null && inventoryIndex != hand.HandIndex)
        {
          if (hand.HandType == InventoryHand.Left)
          {
            this.Inventory.HotBarLeftSlotID = inventoryIndex;
            this.LeftHand.SetItem(inventoryItem.ItemID, true);
          }
          else if (hand.HandType == InventoryHand.Right)
          {
            this.Inventory.HotBarRightSlotID = inventoryIndex;
            this.RightHand.SetItem(inventoryItem.ItemID, true);
          }
          return true;
        }
      }
      return false;
    }

    public bool EquipFromInventory(int inventoryIndex, int equipmentIndex)
    {
      if (inventoryIndex < 0 || inventoryIndex >= (int) this.Inventory.PackSize || (equipmentIndex < (int) this.Inventory.EquipIndexStart || equipmentIndex >= (int) this.Inventory.EquipIndexEnd))
        return false;
      InventoryItem inventoryItem = this.Inventory[inventoryIndex];
      this.Inventory[inventoryIndex] = this.Inventory[equipmentIndex];
      this.Inventory[equipmentIndex] = inventoryItem;
      return true;
    }

    public void EquipItem(InventoryHand hand, int slotID)
    {
      Hand hand1;
      switch (hand)
      {
        case InventoryHand.None:
          hand1 = (Hand) null;
          break;
        case InventoryHand.Left:
          hand1 = this.LeftHand;
          break;
        default:
          hand1 = this.RightHand;
          break;
      }
      int inventoryIndex = slotID;
      this.EquipFromInventory(hand1, inventoryIndex);
    }

    public void EquipItem(Hand hand, int slotID)
    {
      this.EquipFromInventory(hand, slotID);
    }

    public bool CanUseItem(Item itemID)
    {
      if (itemID == Item.None || !this.instance.IsSkillsEnabled || this.IsGod)
        return true;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      return this.SkillsData[(int) skillDataXml.UseSkill].Level >= skillDataXml.UseReq;
    }

    public bool CanCraftItem(Item itemID)
    {
      if (itemID == Item.None || !this.instance.IsSkillsEnabled || this.IsGod)
        return true;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      return this.SkillsData[(int) skillDataXml.CraftSkill].Level >= skillDataXml.CraftReq;
    }

    protected virtual int SwitchItemUpperBound(Hand hand)
    {
      return (int) this.Inventory.PackSize;
    }

    public bool SwitchArrows(Hand hand)
    {
      Item obj = hand.ItemID;
      int upperBound = this.SwitchItemUpperBound(hand);
      if (!ItemData.IsSubType(obj, ItemSubType.Arrow))
      {
        if (this.EquipFromInventoryCore(hand, Item.IceArrow, upperBound))
          return true;
        obj = Item.IceArrow;
      }
      for (Item nextArrowType = this.GetNextArrowType(obj); nextArrowType != obj; nextArrowType = this.GetNextArrowType(nextArrowType))
      {
        if (this.EquipFromInventoryCore(hand, nextArrowType, upperBound))
          return true;
      }
      return false;
    }

    protected Item GetNextArrowType(Item arrow)
    {
      Item obj = arrow;
      if ((uint) obj <= 322U)
      {
        switch (obj)
        {
          case Item.FlintArrow:
            return Item.BoomArrow;
          case Item.IronArrow:
            return Item.BronzeArrow;
          case Item.SteelArrow:
            return Item.IronArrow;
          case Item.DiamondArrow:
            return Item.SteelArrow;
          case Item.RubyArrow:
            return Item.DiamondArrow;
          case Item.TitaniumArrow:
            return Item.RubyArrow;
          case Item.BoomArrow:
            return Item.FireArrow;
          case Item.FireArrow:
            return Item.IceArrow;
        }
      }
      else
      {
        if (obj == Item.IceArrow)
          return Item.TitaniumArrow;
        if (obj == Item.BronzeArrow)
          return Item.FlintArrow;
      }
      return Item.IceArrow;
    }

    public bool SetShield(Hand hand)
    {
      Item obj = hand.ItemID;
      int upperBound = this.SwitchItemUpperBound(hand);
      if (!ItemData.IsSubType(obj, ItemSubType.Shield))
      {
        if (this.EquipFromInventoryCore(hand, Item.ShieldBadge, upperBound))
          return true;
        obj = Item.ShieldBadge;
      }
      for (Item nextShieldType = this.GetNextShieldType(obj); nextShieldType != obj; nextShieldType = this.GetNextShieldType(nextShieldType))
      {
        if (this.EquipFromInventoryCore(hand, nextShieldType, upperBound))
          return true;
      }
      return false;
    }

    protected Item GetNextShieldType(Item shield)
    {
      Item obj = shield;
      if ((uint) obj <= 361U)
      {
        switch (obj)
        {
          case Item.IronShield:
            return Item.WoodShield;
          case Item.SteelShield:
            return Item.IronShield;
          case Item.DiamondShield:
            return Item.GreenstoneGoldShield;
          case Item.ShieldBadge:
            return Item.TitaniumShield;
        }
      }
      else
      {
        switch (obj)
        {
          case Item.GreenstoneGoldShield:
            return Item.SteelShield;
          case Item.DiamantiumShield:
            return Item.DiamondShield;
          case Item.TitaniumShield:
            return Item.DiamantiumShield;
        }
      }
      return Item.ShieldBadge;
    }

    public bool HasRoomForPickup(InventoryItem item)
    {
      return this.Inventory.FindOrGetFreeSlotForItem(item) >= 0;
    }

    public bool PickupItem(InventoryItem item, int particleID)
    {
      if (this.IsDeadOrInactiveOrDisabled)
        return false;
      if (!this.instance.NetworkManager.IsHost && this.IsPlayer)
      {
        this.instance.NetworkManager.SendPickupRequest((Player) this, particleID);
        return false;
      }
      if (!this.PickupItemCore(item, particleID))
        return false;
      this.instance.NetworkManager.SendPickupConfirm(this.GamerID, particleID);
      return true;
    }

    public virtual bool PickupItemCore(InventoryItem item, int particleID)
    {
      if (item.Count <= 0 || this.AddToInventory(item) <= 0)
        return true;
      Sounds.PlaySound(ItemSoundGroup.GenPickup, ItemSoundType.Use);
      return true;
    }

    public void TeleportTo(Vector3 pos)
    {
      this.TeleportToCore(pos, true);
    }

    public void TeleportTo(Actor target)
    {
      if (target.IsCrouching)
        this.Size.Y = this.crouchHeight;
      if (target.IsFlying)
        this.FlyMode = FlyMode.Slow;
      this.TeleportTo(target.Position);
    }

    public void TeleportTo(GlobalPoint3D p, bool useHeightMap)
    {
      p = this.map.Clamp(p);
      if (useHeightMap)
        p.Y = Math.Max((int) this.map.GetHeight(p), this.instance.GetGeneratedHeight(p));
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.Y -= this.map.HalfTileSize - this.map.HalfTileSize * 0.1f;
      this.TeleportTo(blockCenter);
    }

    private void TeleportToCore(Vector3 pos, bool retry)
    {
      if (this.map.IsPassable(new BoundingBox()
      {
        Min = {
          X = pos.X - (float) (((double) this.Box.Max.X - (double) this.Box.Min.X) * 0.5),
          Y = pos.Y,
          Z = pos.Z - (float) (((double) this.Box.Max.Z - (double) this.Box.Min.Z) * 0.5)
        },
        Max = {
          X = pos.X + (float) (((double) this.Box.Max.X - (double) this.Box.Min.X) * 0.5),
          Y = pos.Y + (this.Box.Max.Y - this.Box.Min.Y),
          Z = pos.Z + (float) (((double) this.Box.Max.Z - (double) this.Box.Min.Z) * 0.5)
        }
      }) || this.IsGodOrTester)
      {
        this.lastPosition = this.Position = pos;
        this.accVel.X = this.accVel.Y = this.accVel.Z = 0.0f;
        this.positionInterpolator.Reset();
        this.UpdateBounds();
        if (this.IsPlayer)
          this.instance.OpenLoadingPlayerViewScreen(this as Player);
        this.ClearPressurePoints();
      }
      else
      {
        if (!retry)
          return;
        pos.Y += this.map.TileSize;
        this.TeleportToCore(pos, false);
      }
    }

    public void PlayPainSound(float damage)
    {
      Sounds.PlaySound((ITMActor) this, ActorSoundType.Pain);
    }

    public void PlayWarningSound()
    {
      Sounds.PlaySound((ITMActor) this, ActorSoundType.Warning);
    }

    public void PlayStrikeSound()
    {
      Sounds.PlaySound((ITMActor) this, ActorSoundType.Strike);
    }

    public void PlayDeathSound()
    {
      Sounds.PlaySound((ITMActor) this, ActorSoundType.Death);
    }

    public void PlayRandomSound()
    {
      switch (this.instance.Random.Next(4))
      {
        case 0:
          this.PlayDeathSound();
          break;
        case 1:
          this.PlayWarningSound();
          break;
        case 2:
          this.PlayStrikeSound();
          break;
        case 3:
          this.PlayPainSound(50f);
          break;
      }
    }

    public void PlayWalkSound()
    {
      Vector3 position = this.Position;
      position.Y += this.Gravity;
      this.PlayWalkSound(this.map.GetPoint(position));
    }

    public void PlayWalkSound(GlobalPoint3D p)
    {
      if (this.LandingSoundDelay >= 1)
        return;
      Block blockId = (Block) this.map.GetBlockID(p);
      this.PlayFootStepSound(p, blockId);
    }

    public void PlayMineSound(GlobalPoint3D p, Block blockID, int textureIndex)
    {
      GlobalPoint3D p1 = p;
      if (blockID == Block.CoverBlock)
      {
        --p1.Y;
        blockID = (Block) this.map.GetBlockID(p1);
        if (blockID == Block.None)
          blockID = Block.CoverBlock;
      }
      Sounds.PlaySound(blockID != Block.ArcadeMachine ? (Item) this.map.GetBlockTextureIDForDrawing(blockID, textureIndex) : Item.ArcadeMachine, ItemSoundType.Mine, p, (ITMActor) this);
    }

    public void EffectDelete(string name)
    {
      if (this.effectManager == null)
        return;
      this.effectManager.DeleteEffect(name);
    }

    public void EffectDeleteAll()
    {
      if (this.effectManager == null)
        return;
      this.effectManager.DeleteAllEffects();
    }

    public void EffectAddHealth(string name, int qty, int millisecs, int duration)
    {
      if (this.effectManager == null)
        return;
      HealthEffect healthEffect = new HealthEffect();
      healthEffect.Name = name;
      healthEffect.Points = qty;
      healthEffect.Interval = (float) millisecs / 1000f;
      healthEffect.Duration = (float) duration / 1000f;
      this.effectManager.AddEffect((CharacterEffect) healthEffect);
    }

    public void EffectAddHealth(string name, int qty, int millisecs, string history)
    {
      if (this.effectManager == null)
        return;
      HealthEffect healthEffect = new HealthEffect();
      healthEffect.Name = name;
      healthEffect.Points = qty;
      healthEffect.Interval = (float) millisecs / 1000f;
      healthEffect.History = history;
      this.effectManager.AddEffect((CharacterEffect) healthEffect);
    }

    public void ClearBlock(Hand hand, Item tool)
    {
      if (tool == Item.Lighter || !this.SwingTargetIsValid || !this.HasPermission(Permissions.Edit, true))
        return;
      this.ClearBlockCore(hand, tool);
    }

    protected virtual void ClearBlockCore(Hand hand, Item tool)
    {
      MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(this.SwingTarget);
      if (ItemData.IsSubType(tool, ItemSubType.TillTool) && BlockData.IsTillable((Block) blockIdAndAux.BlockID, tool))
      {
        this.map.SetBlockData(this.SwingTarget, (byte) 173, (byte) 0, UpdateBlockMethod.Player, this.GamerID, true);
        this.OnItemUsed(hand);
        this.SkillsData.BlockMined(this, tool, blockIdAndAux);
        --this.SplinterProgress;
        this.Splinter = -1;
        this.map.Commit();
      }
      else
      {
        if (!this.instance.ClearBlock(this.SwingTarget, UpdateBlockMethod.Player, this.GamerID, true))
          return;
        this.map.Commit();
        this.SkillsData.BlockMined(this, tool, blockIdAndAux);
        this.OnItemUsed(hand, ItemUseType.ClearBlock);
      }
    }

    private struct TimedPoint : IComparable<Actor.TimedPoint>, IEquatable<Actor.TimedPoint>
    {
      public GlobalPoint3D Point;
      public double TimeStamp;
      public float Period;
      public Block BlockID;

      public bool Equals(Actor.TimedPoint other)
      {
        return this.Point == other.Point;
      }

      public int CompareTo(Actor.TimedPoint other)
      {
        return this.Point.CompareTo(other.Point);
      }
    }

    public struct BonusData
    {
      public float Value;
      public int SlotID;
    }
  }
}
