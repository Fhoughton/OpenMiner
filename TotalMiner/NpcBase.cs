// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.NpcBase
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
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class NpcBase : Actor2
  {
    private NpcBase.MobNetworkInstanceData netData = new NpcBase.MobNetworkInstanceData();
    public NpcSpawnBlock SpawnBlock;
    public CombatStats CombatStats;
    public LootTable LootTable;
    public DayOrNight DayOrNight;
    public ActorState LastSendState;
    public float LastSendHealth;
    public Vector3 LastSendPosition;
    public Vector3 LastSendViewDir;
    public long LastNetUpdate;
    protected Player currentClosestPlayer;
    protected float currentClosestPlayerDistance;
    protected Player currentClosestPlayerInView;
    protected float currentClosestPlayerDistanceInView;
    protected NpcBase currentClosestMobInView;
    protected float currentClosestMobDistanceInView;
    protected Vector3 currentClosestPlayerInViewFrom;
    protected Vector3 currentClosestPlayerInViewDir;
    protected float jumpTimer;
    protected NpcAnimContent npcContent;
    protected GamerID npcID;
    protected Script killScript;
    private float behaviourTreeTrackDelay;
    private string combatLevelString;
    private string displayGamertag;
    private float deathThrowsTimer;

    public string BehaviourName
    {
      get
      {
        if (this.behaviourTree == null)
          return (string) null;
        return this.behaviourTree.Name;
      }
    }

    protected override float ExplodeBlocksRatio
    {
      get
      {
        return 0.04f;
      }
    }

    protected override Vector2 ExplodeBlocksScale
    {
      get
      {
        return new Vector2(1.9f, 2.4f);
      }
    }

    public override bool IceEffectActive
    {
      get
      {
        if (!this.map.IsHost)
          return this.netData.IceAffectOn;
        return (double) this.FreezeTimer > 0.0;
      }
    }

    protected override bool DisableCameraBackOffset
    {
      get
      {
        return true;
      }
    }

    public override GamerID GamerID
    {
      get
      {
        return this.npcID;
      }
    }

    public override bool IsLocalGamer
    {
      get
      {
        return this.map.IsHost;
      }
    }

    protected override int HealthLevelCore(bool addBonuses)
    {
      return this.CombatStats.HealthLevel;
    }

    protected override int AttackLevelCore(bool addBonuses)
    {
      return this.CombatStats.AttackLevel;
    }

    protected override int StrengthLevelCore(bool addBonuses)
    {
      return this.CombatStats.StrengthLevel;
    }

    protected override int DefenceLevelCore(bool addBonuses)
    {
      return this.CombatStats.DefenceLevel;
    }

    protected override int RangedLevelCore(bool addBonuses)
    {
      return this.CombatStats.RangedLevel;
    }

    public override bool IsCustomMob
    {
      get
      {
        return !this.CombatStats.IsEqual(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.ActorType].LevelType]);
      }
    }

    public override string CombatLevelString
    {
      get
      {
        if (this.combatLevelString == null)
          this.combatLevelString = this.CombatLevel.ToString();
        return this.combatLevelString;
      }
    }

    protected override NpcSpawnBlock GetSpawnBlock()
    {
      return this.SpawnBlock;
    }

    public override string DisplayGamertag
    {
      get
      {
        if (this.displayGamertag == null)
          this.displayGamertag = this.SpawnBlock == null || this.SpawnBlock.Name == null ? "NPC: " + this.npcID.ToString() : this.SpawnBlock.Name;
        return this.displayGamertag;
      }
    }

    public NpcBase(GameInstance instance, MapTM map, NpcAnimContent content)
      : base(instance, map, (NetworkGamer) null, content.ActorType)
    {
      this.npcContent = content;
    }

    protected override void InitializeCore(InitState state)
    {
      this.SetCombatStats(Globals1.NpcTypeData[(int) this.ActorType].LevelType);
      base.InitializeCore(state);
      this.InitializeForRecycle();
      this.nonSwingTargets = new List<byte>();
      this.nonSwingTargets.Add((byte) 53);
      this.nonSwingTargets.Add(this.map.OutOfBoundsBlockID);
      if (this.instance.IsHost)
        this.effectManager = new CharacterEffectManager((ITMActor) this, (ITMActor) null);
      this.Inventory.ItemChanged += new InventoryEventHandler(this.OnInventoryItemChanged);
    }

    private void InitializeForRecycle()
    {
      this.displayGamertag = (string) null;
      this.Reach = 8;
      this.playerPainSoundDelay = 2f;
      this.halfSizeFactor = 0.36f;
      this.Health = this.MaxHealth;
      this.Oxygen = this.MaxOxygen;
      this.LastSendState = ActorState.Alive;
      this.positionInterpolator.Reset();
      if (this.effectManager == null)
        return;
      this.effectManager.DeleteAllEffects();
    }

    public void SetCombatStats(CombatStats stats)
    {
      this.CombatStats = stats;
      this.SetSkillDataFromCombatStats();
    }

    public void SetCombatStats(ActorLevelType levelType)
    {
      this.CombatStats.SetFromXML(Globals1.NpcLevelData[(int) levelType]);
      this.SetSkillDataFromCombatStats();
    }

    private void SetSkillDataFromCombatStats()
    {
      this.SkillsData = new CharacterSkillsData();
      this.SkillsData.Attack.SetCurrentXPRaw((double) SkillData.GetXP(this.CombatStats.AttackLevel));
      this.SkillsData.Defence.SetCurrentXPRaw((double) SkillData.GetXP(this.CombatStats.DefenceLevel));
      this.SkillsData.Health.SetCurrentXPRaw((double) SkillData.GetXP(this.CombatStats.HealthLevel));
      this.SkillsData.Ranged.SetCurrentXPRaw((double) SkillData.GetXP(this.CombatStats.RangedLevel));
      this.SkillsData.Strength.SetCurrentXPRaw((double) SkillData.GetXP(this.CombatStats.StrengthLevel));
    }

    protected override void LoadContentCore(InitState state)
    {
      base.LoadContentCore(state);
      MapModel model = this.npcContent.Frames[0].Model;
      if (model == null)
        return;
      float scale = NpcContentFrame.GetFullModelHeight(this.ActorType) / (float) model.ModelSize.Y;
      this.SetSize(model.ModelSize, scale);
    }

    public override void NpcSpawn(Vector3 pos, GamerID npcID, Script killScript)
    {
      base.NpcSpawn(pos, npcID, (Script) null);
      this.npcID = npcID;
      this.killScript = killScript;
      this.jumpTimer = 0.0f;
      this.strikeTimer = this.StrikeDelay;
      this.currentClosestPlayer = (Player) null;
      this.currentClosestPlayerInView = (Player) null;
      this.ViewDirection = Vector3.Forward;
      this.displayGamertag = (string) null;
      this.UpdateBounds();
      this.UpdateMatrices();
    }

    private void OnInventoryItemChanged(object sender, InventoryItemEventArgs e)
    {
      if (this.Inventory[e.SlotID].ItemID == e.Item.ItemID)
        return;
      if (this.Inventory.LeftHandIndex == this.Inventory.RightHandIndex && e.SlotID == this.Inventory.LeftHandIndex)
      {
        Item itemId = this.Inventory[e.SlotID].ItemID;
        if (itemId == this.LeftHand.ItemID || itemId == this.RightHand.ItemID)
          return;
        if (itemId == Item.None)
        {
          this.LeftHand.SetItem(itemId);
          this.RightHand.SetItem(itemId);
        }
        else if (this.LeftHand.HasItem && !this.RightHand.HasItem)
          this.LeftHand.SetItem(itemId);
        else if (this.RightHand.HasItem && !this.LeftHand.HasItem)
          this.RightHand.SetItem(itemId);
        else if (ItemData.GetItemEquipIndex(itemId) == EquipIndex.RightHand)
          this.RightHand.SetItem(itemId);
        else
          this.LeftHand.SetItem(itemId);
      }
      else if (e.SlotID == this.Inventory.LeftHandIndex)
      {
        this.LeftHand.SetItem(this.Inventory[e.SlotID].ItemID);
      }
      else
      {
        if (e.SlotID != this.Inventory.RightHandIndex)
          return;
        this.RightHand.SetItem(this.Inventory[e.SlotID].ItemID);
      }
    }

    public override bool ChangeState(ActorState newState)
    {
      if (!base.ChangeState(newState))
        return false;
      if (newState == ActorState.InActive)
      {
        if (this.effectManager != null)
          this.effectManager.DeleteAllEffects();
        this.instance.NpcManager.DeactivateNpc(this);
      }
      return true;
    }

    protected override void UpdateState()
    {
      base.UpdateState();
      if (this.State == ActorState.InActive || this.State == ActorState.Despawning || !this.TimeToDespawn)
        return;
      this.ChangeState(ActorState.Despawning);
    }

    protected virtual bool TimeToDespawn
    {
      get
      {
        if (this.DayOrNight == DayOrNight.Night)
        {
          if (this.instance.Random.RandomChance(0.000666666666666667) && (double) this.map.LightCycle * (double) this.map.GetLight(this.map.GetPoint(this.EyePosition)).SunLight > 10.0 || this.instance.SunMoon.IsDayTime && this.instance.CurrentDaysGameTime.Hours >= 10)
            return true;
        }
        else if (this.DayOrNight == DayOrNight.Day && (this.instance.Random.RandomChance(0.000666666666666667) && (double) this.map.LightCycle * (double) this.map.GetLight(this.map.GetPoint(this.EyePosition)).SunLight < 5.0 || this.instance.SunMoon.IsNightTime && this.instance.CurrentDaysGameTime.Hours >= 22))
          return true;
        return false;
      }
    }

    protected override void ExplodeModel()
    {
      if (this.npcContent == null)
        return;
      this.npcContent.Frames[this.avatarAnim.CurrentFrame].Explode(this.instance, this.Position, this.DrawScale * this.ExplodeBlocksScale, this.ExplodeBlocksRatio);
    }

    protected override void UpdateCore(UpdateState state)
    {
      this.LeftHand.ClearSwing();
      this.RightHand.ClearSwing();
      if (this.map.IsHost)
      {
        if (this.behaviourTree != null)
        {
          if (this.behaviourTree.TrackType == BehaviourTrackType.DebugTime)
          {
            Player player = this.instance.NetworkManager.LocalEnabledPlayers.Count > 0 ? this.instance.NetworkManager.LocalEnabledPlayers[0] : (Player) null;
            if (this.instance.IsCreativeMode || player != null && player.IsGodOrTester)
            {
              this.behaviourTreeTrackDelay -= Services.ElapsedTime;
              if ((double) this.behaviourTreeTrackDelay <= 0.0)
                this.behaviourTreeTrackDelay = 0.2f;
              else
                goto label_10;
            }
          }
          this.behaviourEngine.Update((ITMWorld) this.instance, this.behaviourTree);
        }
        base.UpdateCore(state);
      }
      else
      {
        this.UpdateRemote();
        this.playPainSoundTimer -= Services.ElapsedTime;
        if (!this.IsInactive)
        {
          this.UpdateGeneral();
          this.UpdateMatrices();
        }
      }
label_10:
      int frameCount = this.npcContent.Frames.Length - 1;
      if (frameCount > 0)
        this.avatarAnim.Update(Services.ElapsedTime, frameCount, this.VisualVelocity, this.MaxVelocity, 0.075f);
      if (this.npcContent.Frames[this.avatarAnim.CurrentFrame] == null)
        return;
      this.instance.NpcManager.AddNpcContentFrame((Actor) this, this.npcContent.Frames[this.avatarAnim.CurrentFrame]);
    }

    protected override void UpdatePhysics()
    {
      base.UpdatePhysics();
    }

    protected override void UpdateGeneral()
    {
      float y = 0.0f;
      if (this.map.IsHost && (double) this.FreezeTimer <= 0.0 && this.State != ActorState.Despawning)
      {
        Vector3 finalLookAtPosition = this.GetFinalLookAtPosition(this.lookAtType, this.lookAtPosition);
        y = Vector3.Normalize(finalLookAtPosition - this.EyePosition).Y;
        if ((double) finalLookAtPosition.X != 0.0 || (double) finalLookAtPosition.Z != 0.0)
          this.viewAngle.X = this.GetRotation(this.EyePosition, this.viewAngle.X, finalLookAtPosition, this.RotateSpeed);
      }
      Vector2 vector2 = Vector2.Transform(new Vector2(0.0f, 1f), Matrix.CreateRotationZ(this.viewAngle.X - 1.570796f));
      this.ViewDirection = Vector3.Normalize(new Vector3(vector2.X, y, vector2.Y));
      this.LeftHand.UpdateSwing();
      this.RightHand.UpdateSwing();
      base.UpdateGeneral();
      if (++this.calcSwingTargetDelay != 4)
        return;
      this.HitTarget.Clear();
      this.CalcSwingTarget(8);
      this.calcSwingTargetDelay = 0;
      this.UpdateDialog(4);
    }

    private void UpdateDialog(int ticksPerCall)
    {
      if (this.CurrentDialog != null)
        return;
      this.lastDialogTimer -= Services.ElapsedTime * (float) ticksPerCall;
      if ((double) this.lastDialogTimer > 7.0)
        return;
      DialogNode dialogNode = (DialogNode) null;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
      {
        if (!localEnabledPlayer.DialogHandler.InConversation && this.Frustum.Intersects(localEnabledPlayer.Box) && (double) Vector3.DistanceSquared(this.Position, localEnabledPlayer.Position) < 36.0)
        {
          if (this.CurrentDialogTarget == null)
            this.CurrentDialogTarget = (INPCBehaviour) localEnabledPlayer;
          if (dialogNode == null)
            dialogNode = localEnabledPlayer.DialogHandler.FindOpeningLine(this);
          if (dialogNode != this.lastDialog || (double) this.lastDialogTimer <= 0.0)
          {
            this.CurrentDialog = dialogNode;
            localEnabledPlayer.DialogHandler.NpcSaidSomething(this, this.CurrentDialog);
          }
        }
      }
    }

    public void EnteredDirectDialog(Player player, DialogNode node)
    {
      this.lastDialogTimer = this.SpawnBlock != null ? (float) this.SpawnBlock.DialogDelay : 10f;
      this.CurrentDialog = node;
      this.CurrentDialogTarget = (INPCBehaviour) player;
    }

    public void StoppedTalking()
    {
      this.lastDialogTimer = this.SpawnBlock != null ? (float) this.SpawnBlock.DialogDelay : 10f;
      this.lastDialog = this.CurrentDialog;
      this.CurrentDialog = (DialogNode) null;
      this.CurrentDialogTarget = (INPCBehaviour) null;
    }

    private void UpdateRemote()
    {
      Vector3 position = this.Position;
      if (this.netData.IsUpdated)
      {
        this.Health = this.netData.Health;
        ActorState actorState = this.actorState;
        this.ChangeState(this.netData.State);
        if (this.netData.State == ActorState.Dying && actorState != ActorState.Dying)
          this.Die(DamageType.Unknown, (Actor) null, Item.None, 0.0f);
        if (float.IsNaN(this.Position.X) || float.IsNaN(this.Position.Y) || float.IsNaN(this.Position.Z))
          this.Position = this.netData.Position;
        this.Velocity.Y = this.netData.VelocityY;
        this.Position.Y = this.netData.Position.Y;
      }
      Vector3 vector3_1 = this.netData.Position - this.Position;
      float num = vector3_1.Length();
      if ((double) num > 0.0)
      {
        Vector3 vector3_2 = Vector3.Normalize(vector3_1);
        float acceleration = this.Acceleration;
        if ((double) num < 2.0)
        {
          this.Velocity.X = (float) ((double) vector3_2.X * (double) acceleration * 0.899999976158142);
          this.Velocity.Z = (float) ((double) vector3_2.Z * (double) acceleration * 0.899999976158142);
        }
        else if ((double) num < 6.0)
        {
          this.Velocity.X = (float) ((double) vector3_2.X * (double) acceleration * 1.0);
          this.Velocity.Z = (float) ((double) vector3_2.Z * (double) acceleration * 1.0);
        }
        else
        {
          this.Velocity.X = (float) ((double) vector3_2.X * (double) acceleration * 1.5);
          this.Velocity.Z = (float) ((double) vector3_2.Z * (double) acceleration * 1.5);
        }
        if ((double) Math.Abs(this.Velocity.X) > (double) Math.Abs(vector3_1.X))
          this.Velocity.X = vector3_1.X;
        if ((double) Math.Abs(this.Velocity.Z) > (double) Math.Abs(vector3_1.Z))
          this.Velocity.Z = vector3_1.Z;
        this.Position.X += this.Velocity.X;
        this.Position.Z += this.Velocity.Z;
        this.Position.Y += this.Velocity.Y;
        if (!this.map.BlockData[(int) this.map.GetBlockID(this.map.GetPoint(this.Position))].IsPassable)
        {
          this.Position.Y -= this.Velocity.Y;
          this.Velocity.Y = 0.0f;
        }
        else if ((double) this.Velocity.Y > 0.0)
          this.Velocity.Y += this.Gravity;
      }
      if (!this.map.IsValidPoint(this.map.GetPoint(this.Position)))
        this.Position = position;
      this.netData.IsUpdated = false;
    }

    protected override void UpdateItemEffects()
    {
    }

    protected override void UpdateControlPhysics(
      GlobalPoint3D underFootPoint,
      Block footBlockID,
      Block midBlockID,
      Block eyeBlockID,
      float speedModifier)
    {
      if (!this.IsFloatingInWater || this.isOnLadder || (this.isOnRope || eyeBlockID != Block.Water))
        return;
      this.Velocity.Y += this.JumpSpeed * 0.5f;
      float max = 0.05f;
      this.Velocity.Y = MathHelper.Clamp(this.Velocity.Y, -max, max);
    }

    protected override bool SkipCollision
    {
      get
      {
        if (this.State != ActorState.Despawning)
          return base.SkipCollision;
        return true;
      }
    }

    protected float GetRotation(
      Vector3 position,
      float currentAngle,
      Vector3 target,
      float turnSpeed)
    {
      return MyMathHelper.TurnToFace2(new Vector2(position.X, position.Z), new Vector2(target.X, target.Z), currentAngle, turnSpeed);
    }

    protected override Vector3 GetFinalLookAtPosition(CoordType type, Vector3 pos)
    {
      if (type == CoordType.SpawnRelative)
        return pos + (this.SpawnBlock != null ? this.map.GetBlockCenter(this.SpawnBlock.Point) : this.Position);
      return base.GetFinalLookAtPosition(type, pos);
    }

    protected override void UpdateDying()
    {
      this.deathThrowsTimer += Services.ElapsedTime;
      if ((double) this.deathThrowsTimer <= 2.0)
        return;
      this.deathThrowsTimer = 0.0f;
      this.ChangeState(ActorState.InActive);
    }

    protected override float TakeDamageLocal(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      Actor attacker,
      Item weaponID)
    {
      float num = this.Health - damage;
      if ((double) num <= 0.0 || (double) num > (double) this.MaxHealth * 0.100000001490116 && this.random.Next(8) < 5)
        this.playPainSoundTimer = 1f;
      return base.TakeDamageLocal(damageType, damage, knockForce, attacker, weaponID);
    }

    protected override bool ShouldDropItemsOnDeath(
      DamageType damageType,
      Actor attacker,
      Item weaponID)
    {
      if (damageType == DamageType.Drowning || attacker is NpcBase)
        return false;
      if (damageType != DamageType.Effect)
        return base.ShouldDropItemsOnDeath(damageType, attacker, weaponID);
      if (this.IsUsingCustomLootTable)
        return true;
      if (this.instance.IsCreativeMode)
        return this.Inventory.HasItems();
      return false;
    }

    protected override bool IsUsingCustomLootTable
    {
      get
      {
        if (this.LootTable == null)
          return false;
        if (this.LootTable.Count <= 0)
          return this.LootTable.Point.HasValue;
        return true;
      }
    }

    protected override void SetDeathDropItems(DamageType damageType, Actor attacker)
    {
      if (this.IsUsingCustomLootTable)
        this.SetDeathDropItemsFromLootTable(damageType, attacker);
      else
        this.SetDeathDropItemsDefault(damageType, attacker);
    }

    protected void SetDeathDropItemsFromLootTable(DamageType damageType, Actor attacker)
    {
      List<LootDrop> table = this.LootTable.Table;
      Inventory inventory = (Inventory) null;
      if (this.LootTable.Point.HasValue)
      {
        ChestBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(this.LootTable.Point.Value) as ChestBlock;
        if (dataBlock != null)
          inventory = dataBlock.Inventory;
        if (inventory == null)
          return;
      }
      if (table.Count == 0)
      {
        if (inventory == null)
          return;
        int randomItem = inventory.GetRandomItem(this.instance.Random);
        if (randomItem < 0)
          return;
        InventoryItem newItem = inventory[randomItem];
        if (newItem.ItemID == Item.None || newItem.ItemID == Item.Chance)
          return;
        inventory.DecrementItem(randomItem, newItem.Count);
        this.AddToInventory(newItem);
      }
      else
      {
        LootDrop lootDrop1 = new LootDrop();
        float lootLevel = (float) this.CombatLevel / 2f;
        if (attacker != null)
          lootLevel += (float) attacker.LootingLevel(true);
        for (int index = table.Count - 1; index >= 0; --index)
        {
          LootDrop lootDrop2 = table[index];
          if ((double) lootDrop2.Percent > 0.0 && lootDrop2.ItemID != Item.None && (double) this.random.Next(100) < (double) lootDrop2.Percent)
          {
            if (inventory != null)
            {
              int slotID = lootDrop2.ItemID == Item.Chance ? inventory.GetRandomItem(this.instance.Random) : inventory.FindItem(lootDrop2.ItemID);
              if (slotID >= 0)
              {
                InventoryItem inventoryItem = inventory[slotID];
                if (inventoryItem.ItemID != Item.Chance)
                {
                  int count = Math.Min(lootDrop2.Count, inventoryItem.Count);
                  inventory.DecrementItem(slotID, count);
                  this.AddToInventory(inventoryItem.ItemID, count);
                }
              }
            }
            else if (lootDrop2.ItemID == Item.Chance)
              this.SetDeathDropItem(damageType, lootDrop2.Count, lootLevel);
            else
              this.AddToInventory(lootDrop2.ItemID, Math.Max(1, Math.Min(lootDrop2.Count, ItemData.GetStackSize(lootDrop2.ItemID))));
          }
        }
      }
    }

    protected virtual void SetDeathDropItemsDefault(DamageType damageType, Actor attacker)
    {
      LootItem[] lootTable = this.NpcTypeData.LootTable;
      if (lootTable != null && lootTable.Length > 0)
      {
        foreach (LootItem lootItem in lootTable)
        {
          if ((lootItem.Damage == DamageType.Unknown || lootItem.Damage == damageType) && this.random.Next(100) < lootItem.Percent)
          {
            int count = this.instance.Random.Next(lootItem.Count2 < lootItem.Count1 ? 0 : lootItem.Count1, (lootItem.Count2 < lootItem.Count1 ? lootItem.Count1 : lootItem.Count2) + 1);
            if (count > 0)
              this.AddToInventory(lootItem.Item, count);
          }
        }
      }
      if (!this.Properties.DropRandomLootOnDeath.HasValue || !this.Properties.DropRandomLootOnDeath.Value)
        return;
      float lootLevel = (float) this.CombatLevel / 2f;
      if (attacker != null)
        lootLevel += (float) attacker.LootingLevel(true);
      int count1 = this.random.Next(1, 4 + (int) ((double) lootLevel / 10.0));
      this.SetDeathDropItem(damageType, count1, lootLevel);
    }

    private void SetDeathDropItem(DamageType damageType, int count, float lootLevel)
    {
      float num = (float) (25.0 + (double) lootLevel * 5.0);
      while (count > 0)
      {
        Item itemID = (Item) this.random.Next(1, Globals1.ItemData.Length);
        ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID];
        if (itemDataXml.IsValid && itemDataXml.MinCSPrice >= 50 && this.CanDrop(itemID))
        {
          if ((Globals1.ItemData[(int) itemID].CanDropIfLocked || this.instance.IsItemUnlocked(itemID)) && this.random.Next(itemDataXml.DropChance > (ushort) 0 ? (int) (double) itemDataXml.DropChance : (int) ((double) itemDataXml.MinCSPrice / (double) num)) == 0)
          {
            int count1 = 1;
            if (itemID == Item.GoldPieces)
              count1 = this.random.Next(100, (int) ((double) num * 100.0));
            this.AddToInventory(itemID, count1);
            count = 0;
          }
          --count;
        }
      }
    }

    protected virtual bool CanDrop(Item item)
    {
      Item obj = item;
      if ((uint) obj <= 118U)
      {
        if (obj != Item.Teleport && obj != Item.Fire)
          goto label_4;
      }
      else if (obj != Item.InvisibleBarrier && obj != Item.SkeletonKey)
        goto label_4;
      return false;
label_4:
      return true;
    }

    protected override void OnDeathLocal(
      DamageType deathType,
      Actor attacker,
      Item weaponID,
      float damage)
    {
      if (deathType != DamageType.Combat && deathType != DamageType.Effect || (this.killScript == null || this.killScript.Commands.Count <= 0))
        return;
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) this,
        Killer = attacker
      };
      if (this.SpawnBlock != null)
        data.BlockOffset = new GlobalPoint3D?(this.SpawnBlock.Point);
      this.instance.ExecuteScript(this.killScript, data, true);
    }

    protected override void UpdateDespawning()
    {
      if (this.map.BlockData[(int) this.map.GetBlockIDNoCache(this.map.GetPoint(this.EyePosition + new Vector3(0.0f, 0.1f, 0.0f)))].Buffer < (byte) 2)
      {
        this.ChangeState(ActorState.InActive);
      }
      else
      {
        this.Velocity.X = this.Velocity.Z = 0.0f;
        this.VisualVelocity.X = this.VisualVelocity.Z = 0.0f;
        this.Velocity.Y = this.VisualVelocity.Y = -3f / 500f;
        this.Position.Y += this.Velocity.Y;
        if (!this.RandomChance(0.2))
          return;
        GlobalPoint3D point = this.map.GetPoint(this.Position);
        ++point.Y;
        byte blockIdNoCache = this.map.GetBlockIDNoCache(point);
        if (this.map.BlockData[(int) blockIdNoCache].Buffer > (byte) 1)
        {
          --point.Y;
          blockIdNoCache = this.map.GetBlockIDNoCache(point);
        }
        this.instance.AddMiningParticle(point, (Block) blockIdNoCache, BlockFace.Up);
      }
    }

    public bool IsLostFromNetwork
    {
      get
      {
        return Globals1.ElapsedWatch.ElapsedMilliseconds - this.netData.TimeLastUpdate > 300000L;
      }
    }

    public void UpdateFromNetworkData(
      byte stateBits,
      float health,
      Vector3 pos,
      float velocityY,
      float viewDirX,
      float viewDirZ)
    {
      this.netData.IsUpdated = true;
      this.netData.State = (ActorState) ((int) stateBits & (int) sbyte.MaxValue);
      this.netData.Health = health;
      this.netData.Position = pos;
      this.netData.VelocityY = velocityY;
      this.netData.ViewDirX = viewDirX;
      this.netData.ViewDirZ = viewDirZ;
      this.netData.IceAffectOn = ((int) stateBits & 128) > 0;
      this.netData.TimeLastUpdate = Globals1.ElapsedWatch.ElapsedMilliseconds;
    }

    public void ReloadBehaviour(string name)
    {
      if (!name.IsNotEmpty() || this.behaviourTree == null || !(this.behaviourTree.Name == name))
        return;
      this.LoadBehaviourCore(name);
    }

    protected override void LoadBehaviourCore(string name)
    {
      BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.AI, name);
      if (behaviour == null)
        return;
      this.behaviourTree = behaviour.Clone((INPCBehaviour) this);
      this.behaviourEngine = new ExecutionEngine();
      this.behaviourEngine.SetNode(this.behaviourTree.Root as BehaviourTreeNode);
    }

    protected override void LoadDialogCore(string name)
    {
      BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.Dialog, name);
      if (behaviour == null)
        return;
      this.dialogTree = behaviour.Clone((INPCBehaviour) this);
      (this.CurrentDialogTarget as Player)?.DialogHandler.EndConversation();
    }

    private struct MobNetworkInstanceData
    {
      public bool IsUpdated;
      public ActorState State;
      public float Health;
      public Vector3 Position;
      public float VelocityY;
      public float ViewDirX;
      public float ViewDirZ;
      public bool IceAffectOn;
      public long TimeLastUpdate;
    }
  }
}
