// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Hand
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Screens2;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class Hand : ITMHand
  {
    private bool lastInputItemNotSwingable = true;
    public Item ItemID;
    public ItemModel ItemModel;
    public ItemSwing ItemSwing;
    public bool AutoTrigger;
    public InventoryHand HandType;
    private bool updateSwing;
    private Actor owner;
    private Player player;
    private MapTM map;
    private GameInstance instance;
    private EquipmentInventory inventory;
    private Item nextItemID;
    private int alreadyPlacedBlockThisSwing;
    private bool allowModelUpdateWhileSwinging;
    private bool blockWasPlacedThisSwing;
    private int handIndexOnSwingStart;
    private GlobalPoint3D? pointOnSwingStart;
    private List<ActorType> excludeTypes;
    private NpcBase currentDebug;

    ITMActor ITMHand.Owner
    {
      get
      {
        return (ITMActor) this.owner;
      }
    }

    ITMPlayer ITMHand.Player
    {
      get
      {
        return (ITMPlayer) this.player;
      }
    }

    Item ITMHand.ItemID
    {
      get
      {
        return this.ItemID;
      }
    }

    int ITMHand.HandIndex
    {
      get
      {
        return this.HandIndex;
      }
    }

    InventoryHand ITMHand.HandType
    {
      get
      {
        return this.HandType;
      }
    }

    bool ITMHand.IsSwinging
    {
      get
      {
        return this.IsSwinging;
      }
    }

    void ITMHand.SetItem(Item itemID)
    {
      this.SetItem(itemID, true);
    }

    public bool CanDraw
    {
      get
      {
        if (this.ItemModel == null || this.ItemModel.VertexCount <= 0)
          return false;
        if (this.ItemID == Item.Hand)
          return this.IsSwinging;
        return true;
      }
    }

    public bool IsSwinging
    {
      get
      {
        if (!this.updateSwing)
          return this.ItemSwing.IsSwinging;
        return true;
      }
    }

    public bool IsSwingExtended
    {
      get
      {
        return this.ItemSwing.IsSwingExtended;
      }
    }

    private int HotBarSlotID
    {
      get
      {
        return this.HandIndex;
      }
    }

    public int HandIndex
    {
      get
      {
        if (this.HandType != InventoryHand.Left)
          return this.inventory.RightHandIndex;
        return this.inventory.LeftHandIndex;
      }
    }

    public int HandIndexOnSwingStart
    {
      get
      {
        if (this.handIndexOnSwingStart < 0)
          return this.HandIndex;
        return this.handIndexOnSwingStart;
      }
    }

    public int OtherHandIndex
    {
      get
      {
        if (this.HandType != InventoryHand.Left)
          return this.inventory.LeftHandIndex;
        return this.inventory.RightHandIndex;
      }
    }

    private GlobalPoint3D SwingTarget
    {
      get
      {
        return this.owner.SwingTarget;
      }
    }

    public bool IsSingleWieldHand
    {
      get
      {
        WieldType wieldType = this.owner.WieldType;
        switch (wieldType)
        {
          case WieldType.RightHand:
            if (this.HandType == InventoryHand.Right)
              return true;
            break;
        }
        if (wieldType == WieldType.LeftHand)
          return this.HandType == InventoryHand.Left;
        return false;
      }
    }

    public bool IsDualWielding
    {
      get
      {
        return this.owner.WieldType == WieldType.BothHands;
      }
    }

    public bool IsSingleWielding
    {
      get
      {
        return this.owner.WieldType != WieldType.BothHands;
      }
    }

    public Hand OtherHand
    {
      get
      {
        if (this.HandType != InventoryHand.Left)
          return this.owner.LeftHand;
        return this.owner.RightHand;
      }
    }

    public PlayerInput HandToUse
    {
      get
      {
        return this.HandType != InventoryHand.Left ? PlayerInput.RightHand : PlayerInput.LeftHand;
      }
    }

    public bool HasItem
    {
      get
      {
        WieldType wieldType = this.owner.WieldType;
        if (wieldType != WieldType.BothHands && (wieldType == WieldType.LeftHand && this.HandType == InventoryHand.Right || wieldType == WieldType.RightHand && this.HandType == InventoryHand.Left) && this.ItemID == this.OtherHand.ItemID || this.ItemID == Item.None)
          return false;
        return this.ItemID != Item.Hand;
      }
    }

    public Hand(Actor owner, InventoryHand hand)
    {
      this.owner = owner;
      this.HandType = hand;
      this.instance = owner.GameInstance;
      this.map = this.instance.Map;
      this.player = owner as Player;
      this.inventory = owner.Inventory;
      this.ItemSwing = new ItemSwing(owner, hand);
      this.ItemModel = new ItemModel(this.instance, owner);
      this.ItemSwing.SwingStart += new EventHandler(this.OnSwingStart);
      this.ItemSwing.SwingFullyExtended += new EventHandler(this.OnSwingFullyExtended);
      this.ItemSwing.SwingComplete += new EventHandler(this.OnSwingComplete);
      this.handIndexOnSwingStart = -1;
      this.SetItem(Item.Hand);
    }

    public void UnloadContent()
    {
      if (this.ItemSwing == null)
        return;
      this.ItemSwing.SwingFullyExtended -= new EventHandler(this.OnSwingFullyExtended);
      this.ItemSwing.SwingComplete -= new EventHandler(this.OnSwingComplete);
    }

    public void OnTexturePackChanged()
    {
      this.ItemModel.ReloadModel();
    }

    public void ClearSwing()
    {
      this.updateSwing = false;
    }

    public void SetIsSwinging(bool b)
    {
      this.updateSwing = b;
      this.excludeTypes = (List<ActorType>) null;
    }

    public void SetIsSwinging(bool b, List<ActorType> excludeTypes)
    {
      this.updateSwing = b;
      this.excludeTypes = excludeTypes;
    }

    public void SetItem(Item newItemID)
    {
      this.SetItem(newItemID, true);
    }

    public void SetItem(Item newItemID, bool overrideOtherHand)
    {
      switch (newItemID)
      {
        case Item.None:
          newItemID = Item.Hand;
          goto case Item.Hand;
        case Item.Hand:
          Item itemId = this.ItemID;
          if (newItemID != itemId && !this.IsSwinging)
          {
            this.ItemID = newItemID;
            this.ItemID = newItemID;
            this.OnItemChanged(itemId);
            this.ItemModel.Initialize(this.ItemID);
          }
          else
            this.nextItemID = newItemID;
          if (!this.owner.IsPlayer)
            break;
          if (this.OtherHand != null)
          {
            if (this.owner.WieldType == WieldType.BothHands)
            {
              if (this.OtherHand.HotBarSlotID == this.HotBarSlotID)
              {
                if (newItemID != Item.Hand && overrideOtherHand)
                  this.OtherHand.SetItem(Item.Hand);
              }
              else if (!this.OtherHand.HasItem && this.inventory[this.OtherHand.HotBarSlotID].ItemID != Item.None)
                this.OtherHand.SetItem(this.inventory[this.OtherHand.HotBarSlotID].ItemID);
            }
            else if (this.IsSingleWieldHand)
            {
              if (!this.SetItemForNonSingleWieldHand(itemId, newItemID))
              {
                this.BindHands();
                this.OtherHand.SetItem(Item.Hand);
              }
            }
            else if (newItemID == Item.Hand)
              this.BindHands();
          }
          this.player.ExecuteItemEquipEventScript(itemId, newItemID, this.HotBarSlotID);
          break;
        default:
          if (this.owner.WieldType == WieldType.BothHands)
          {
            if (this.HandType == InventoryHand.Right && newItemID == Item.Clipboard)
            {
              newItemID = Item.Hand;
              goto case Item.Hand;
            }
            else if (!overrideOtherHand && this.OtherHand != null && (this.OtherHand.HotBarSlotID == this.HotBarSlotID && this.OtherHand.HasItem))
            {
              newItemID = Item.Hand;
              goto case Item.Hand;
            }
            else
              goto case Item.Hand;
          }
          else if (this.owner.IsPlayer)
          {
            if (this.owner.WieldType == WieldType.LeftHand)
            {
              if (this.HandType == InventoryHand.Right && this.HotBarSlotID == this.player.HotBarLeftSlotID)
              {
                newItemID = Item.Hand;
                goto case Item.Hand;
              }
              else
                goto case Item.Hand;
            }
            else if (this.owner.WieldType == WieldType.RightHand && this.HandType == InventoryHand.Left && this.HotBarSlotID == this.player.HotBarRightSlotID)
            {
              newItemID = Item.Hand;
              goto case Item.Hand;
            }
            else
              goto case Item.Hand;
          }
          else
            goto case Item.Hand;
      }
    }

    private bool SetItemForNonSingleWieldHand(Item oldItemID, Item newItemID)
    {
      if (this.owner != null)
      {
        if (ItemData.IsSubType(newItemID, ItemSubType.Bow))
        {
          if (newItemID != oldItemID && ItemData.IsSubType(oldItemID, ItemSubType.Bow))
            return true;
          return this.owner.SwitchArrows(this.OtherHand);
        }
        if (ItemData.IsSubType(newItemID, ItemSubType.GrenadeLauncher))
          return this.owner.EquipFromInventory(this.OtherHand, Item.Grenade);
        if (this.ItemPairsWithShield(newItemID))
        {
          if (newItemID != oldItemID && this.ItemPairsWithShield(oldItemID))
            return true;
          return this.owner.SetShield(this.OtherHand);
        }
      }
      return false;
    }

    private bool ItemPairsWithShield(Item itemID)
    {
      if (ItemData.IsItemType(itemID, ItemType.Weapon))
        return !ItemData.IsSubTypeAny(itemID, ItemSubType.Bow | ItemSubType.Arrow | ItemSubType.Grenade | ItemSubType.GrenadeLauncher);
      return false;
    }

    private void BindHands()
    {
      if (this.owner.WieldType == WieldType.LeftHand)
        this.inventory.HotBarRightSlotID = this.HotBarSlotID;
      else
        this.inventory.HotBarLeftSlotID = this.HotBarSlotID;
    }

    public bool HandleInput(GamePadState pad, GamePadState lastpad)
    {
      if (this.owner.IsPlayer)
      {
        if (this.IsCurrentItemSwingable)
        {
          if (InputManager1.IsInputPressed(this.player.PlayerIndex, this.HandToUse))
          {
            bool newButtonPress = this.lastInputItemNotSwingable | InputManager1.IsInputPressedNew(this.player.PlayerIndex, this.HandToUse);
            this.lastInputItemNotSwingable = false;
            if (this.owner.NextHand == null || this.owner.NextHand == this || !InputManager1.IsInputPressed(this.player.PlayerIndex, this.OtherHand.HandToUse))
            {
              if (this.ShouldPlaceBlockOnBuildButtonPress(false))
                this.BuildButtonPressed(newButtonPress);
              else if (newButtonPress && (this.blockWasPlacedThisSwing || this.ItemSwing.HasSwingExtended) && ItemData2.IsRapidSwingItem(this.owner, this.ItemID))
              {
                this.ItemSwing.SetUserSwingTimeOverride(0.0f);
                this.ItemSwing.ResetSwing();
                this.blockWasPlacedThisSwing = false;
              }
              else if (newButtonPress)
                this.ItemSwing.SetUserSwingTimeOverride(0.0f);
              this.updateSwing = true;
              this.owner.NextHand = this.OtherHand;
              this.OtherHand.AutoTrigger = false;
              if (newButtonPress)
                this.handIndexOnSwingStart = this.HandIndex;
              return false;
            }
          }
          else if (InputManager1.IsInputPressed(this.player.PlayerIndex, this.OtherHand.HandToUse) && this.IsSingleWielding)
          {
            bool newButtonPress = this.lastInputItemNotSwingable | InputManager1.IsInputPressedNew(this.player.PlayerIndex, this.OtherHand.HandToUse);
            if (this.ShouldPlaceBlockOnBuildButtonPress(true) && (newButtonPress || !this.IsSwinging))
            {
              this.lastInputItemNotSwingable = false;
              if (this.BuildButtonPressed(newButtonPress))
              {
                this.updateSwing = true;
                this.owner.NextHand = this;
                return false;
              }
            }
            else if (this.IsSingleWieldHand && this.IsTargetingSpecialBlock)
            {
              this.updateSwing = true;
              this.owner.NextHand = this;
              return false;
            }
          }
        }
        else
          this.lastInputItemNotSwingable = true;
      }
      this.AutoTrigger = false;
      return false;
    }

    private bool BuildButtonPressed(bool newButtonPress)
    {
      if ((this.IsSwinging || this.alreadyPlacedBlockThisSwing >= 1) && !newButtonPress)
        return false;
      if (this.player.PlaceBlock(this))
        this.blockWasPlacedThisSwing = true;
      if ((double) this.player.Settings.AutoplaceTime > 0.0 && this.ItemID != Item.Hand && !ItemData.IsSubType(this.ItemID, ItemSubType.Key))
      {
        this.ItemSwing.SetUserSwingTimeOverride(newButtonPress ? 0.4f : this.player.Settings.AutoplaceTime);
        this.AutoTrigger = !newButtonPress;
      }
      if (newButtonPress)
        this.alreadyPlacedBlockThisSwing = 10;
      if (!ItemData.IsSubType(this.ItemID, ItemSubType.Gun))
        this.instance.ExecuteItemSwingEventScript((Actor) this.player, this.ItemID);
      return true;
    }

    private bool ShouldPlaceBlockOnBuildButtonPress(bool usingOtherHandTrigger)
    {
      if (!usingOtherHandTrigger && this.IsSingleWielding && this.IsSingleWieldHand)
        return false;
      if (this.ItemID != Item.Clipboard && ItemData.GetItemUse(this.ItemID) != ItemUse.Block)
        return ItemData.IsSubType(this.ItemID, ItemSubType.Key);
      return true;
    }

    private bool IsCurrentItemSwingable
    {
      get
      {
        Item itemId = this.ItemID;
        if (!ItemData.IsItemSwingable(itemId) || this.IsSingleWielding && !this.IsSingleWieldHand && itemId == Item.Hand)
          return false;
        if (!this.OtherHand.IsSwinging)
          return true;
        if (ItemData.GetItemUse(itemId) == ItemData.GetItemUse(this.OtherHand.ItemID))
          return false;
        if (ItemData.GetItemUse(this.OtherHand.ItemID) == ItemUse.Item)
          return this.OtherHand.ItemSwing.HasSwingExtended;
        return true;
      }
    }

    private bool IsTargetingSpecialBlock
    {
      get
      {
        return this.owner.SwingTargetIsValid && ItemData.IsSubType((Item) this.map.GetBlockID(this.owner.SwingTarget), ItemSubType.BlockCanBeOpened);
      }
    }

    public void UpdateSwing()
    {
      if (this.updateSwing)
        this.ItemSwing.StartSwing();
      this.ItemSwing.Update((UpdateState) null);
      --this.alreadyPlacedBlockThisSwing;
      this.UpdateModel();
    }

    private void UpdateModel()
    {
      if (this.ItemModel.ItemID == this.ItemID || !this.allowModelUpdateWhileSwinging && this.IsSwinging)
        return;
      Item itemID = this.ItemID;
      if (itemID == Item.Hand && (this.player == null || !this.owner.IsLocalGamer))
        itemID = Item.None;
      this.ItemModel.Initialize(itemID);
    }

    public void OnItemDegraded()
    {
      if (this.HandType == InventoryHand.Left)
        this.ItemModel.CrumbleLeftHand();
      else
        this.ItemModel.CrumbleRightHand();
      this.SetItem(Item.None);
    }

    private void OnSwingStart(object sender, EventArgs e)
    {
      this.pointOnSwingStart = new GlobalPoint3D?();
      if (this.owner.SwingTargetIsValid)
        this.pointOnSwingStart = new GlobalPoint3D?(this.owner.SwingTarget);
      if (this.player != null && this.player.IsLocalGamer)
      {
        if (this.HandType == InventoryHand.Left)
          ++this.player.LeftSwingCountNet;
        else if (this.HandType == InventoryHand.Right)
          ++this.player.RightSwingCountNet;
      }
      if (!ItemData.IsSubType(this.ItemID, ItemSubType.Gun))
        return;
      Sounds.PlaySound(this.ItemID, ItemSoundType.Use, (ITMActor) this.owner, true);
      this.instance.ExecuteItemSwingEventScript(this.owner, this.ItemID);
    }

    private void OnSwingFullyExtended(object sender, EventArgs e)
    {
      if (this.blockWasPlacedThisSwing)
        this.blockWasPlacedThisSwing = false;
      else if (this.owner.IsLocalGamer)
        this.OnSwingFullyExtendedLocal(sender, e);
      else
        this.OnSwingFullyExtendedRemote(sender, e);
    }

    private void OnSwingFullyExtendedLocal(object sender, EventArgs e)
    {
      this.instance.RaiseEventItemSwing(this.ItemID, this);
      if (this.ItemID == Item.Clipboard)
        return;
      Block blockID = Block.zLastBlockID;
      bool flag1 = false;
      bool swingTargetIsValid = this.owner.SwingTargetIsValid;
      if (this.player != null && swingTargetIsValid)
      {
        if (this.ItemID == Item.GoldPieces)
        {
          blockID = (Block) this.map.GetBlockID(this.owner.SwingTarget);
          if (blockID == Block.ArcadeMachine)
            flag1 = true;
        }
        if (!flag1)
        {
          if (this.IsSingleWielding)
            flag1 = !this.IsSingleWieldHand || ItemData.IsSubType(this.ItemID, ItemSubType.Key) || this.IsSingleWieldHand && this.owner.NextHand == this;
          else if (ItemData.GetItemUse(this.ItemID) == ItemUse.Block || ItemData.IsSubType(this.ItemID, ItemSubType.Key))
            flag1 = true;
          else if (ItemData.GetItemUse(this.OtherHand.ItemID) != ItemUse.Block)
          {
            if (this.ItemID == Item.Hand)
              flag1 = this.OtherHand.ItemID != Item.Hand || this.HandType == InventoryHand.Left;
            else if (this.OtherHand.ItemID != Item.Hand)
              flag1 = ItemData.GetItemType(this.ItemID) != ItemType.Tool ? ItemData.GetItemType(this.OtherHand.ItemID) == ItemType.Tool || this.HandType == InventoryHand.Left : this.HandType == InventoryHand.Left && ItemData.GetItemType(this.OtherHand.ItemID) == ItemType.Tool;
          }
        }
        if (flag1)
        {
          if (blockID == Block.zLastBlockID)
            blockID = (Block) this.map.GetBlockID(this.owner.SwingTarget);
          if (this.player.HitSpecialBlock(this, blockID))
            return;
        }
      }
      if (!ItemData.IsSubType(this.ItemID, ItemSubType.Gun))
        this.instance.ExecuteItemSwingEventScript(this.owner, this.ItemID);
      if (ItemData.IsSubType(this.ItemID, ItemSubType.Arrow) && ItemData.IsSubType(this.OtherHand.ItemID, ItemSubType.Bow))
      {
        this.allowModelUpdateWhileSwinging = true;
        this.owner.SwitchArrows(this);
        this.allowModelUpdateWhileSwinging = false;
      }
      else if (ItemData.IsSubTypeAny(this.ItemID, ItemSubType.Bow | ItemSubType.GrenadeLauncher | ItemSubType.Gun))
      {
        this.ShootWeapon(this.ItemID);
      }
      else
      {
        if (!this.owner.SkillsData.UseReqsMet(this.owner, this.ItemID))
          return;
        if (this.ItemID == Item.Camera)
        {
          if (this.player == null)
            return;
          this.player.TakePhoto(this);
        }
        else if (this.ItemID == Item.Binoculars)
        {
          this.updateSwing = false;
          if (this.player == null)
            return;
          this.player.StartBinocularView();
        }
        else
        {
          this.owner.HitTarget = this.owner.BuildHitTargetData(this.owner.ViewDirection, Vector3.Zero, HitTargetOptions.All, this.excludeTypes);
          this.owner.LastHitTarget = this.owner.HitTarget.Target as NpcBase;
          if (!this.owner.IsPlayer)
          {
            if (this.owner.HitTarget.Target != null)
              this.owner.CalcSwingTarget((int) ((double) this.owner.StrikeRange + 0.5));
          }
          else if (this.ItemID == Item.DebugTool)
          {
            StudioForge.TotalMiner.Screens.GameplayScreen screenInstance = StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance;
            if (this.currentDebug != this.owner.LastHitTarget)
            {
              this.CloseBehaviourDebug();
              this.currentDebug = (NpcBase) null;
              if (this.owner.LastHitTarget == null)
                return;
              Rectangle rectangle = GraphicStatics.HUDPos();
              this.player.BTCanvas = (BehaviourTreeDesignWindow) new AIBehaviourTreeDesignWindow(this.player.PlayerIndex, rectangle.X + 2, rectangle.Y + 6, 600, 400, (BehaviourTree) null, this.currentDebug = this.owner.LastHitTarget, (Texture2D) null, Point.Zero);
              this.player.BTCanvas.Colors = Window.TransparentColorProfile;
              this.player.BTCanvas.AdjustSizeToContainAllChildrenDeep();
              this.player.BTCanvas.Scale = MathHelper.Min(1f, 1280f / (float) this.player.BTCanvas.Size.X);
              screenInstance.WindowManager.Root.AddChild((StudioForge.Engine.Core.Node) this.player.BTCanvas);
              this.currentDebug.BehaviourTree.TrackType = BehaviourTrackType.DebugTime;
              return;
            }
            if (this.currentDebug == null)
              return;
            if (this.currentDebug.BehaviourTree.TrackType == BehaviourTrackType.DebugTime)
            {
              this.currentDebug.BehaviourTree.TrackType = BehaviourTrackType.RealTime;
              return;
            }
            this.CloseBehaviourDebug();
            this.instance.AddScreen((GameScreen) new InventoryScreen(this.instance, this.player, (Actor) this.currentDebug), this.player);
            this.currentDebug = (NpcBase) null;
            return;
          }
          if (ItemData.IsSubType(this.ItemID, ItemSubType.Edible))
            this.owner.EatFood(this, this.ItemID);
          else if (this.owner.HitTarget.Target != null && ((double) this.owner.HitTarget.Distance < (double) this.owner.SwingTargetDistance || !swingTargetIsValid))
            this.StrikeHitTarget();
          else if (swingTargetIsValid)
          {
            if (ItemData.CanItemBreakBlocks(this.ItemID))
            {
              if (this.AutoTrigger)
                return;
              if (!ItemData.IsItemUse(this.ItemID, ItemUse.Item))
              {
                if (!this.pointOnSwingStart.HasValue)
                  return;
                GlobalPoint3D? pointOnSwingStart = this.pointOnSwingStart;
                GlobalPoint3D swingTarget = this.owner.SwingTarget;
                if ((!pointOnSwingStart.HasValue ? 0 : (pointOnSwingStart.GetValueOrDefault() == swingTarget ? 1 : 0)) == 0)
                  return;
              }
              Block blockId = (Block) this.map.GetBlockID(this.SwingTarget);
              bool flag2 = true;
              if (blockId == Block.HealthBlock)
              {
                HealthBlock dataBlock = ((MapStrategyTM) this.map.MapStrategy).GetDataBlock(this.SwingTarget) as HealthBlock;
                if (dataBlock != null && dataBlock.IsCombatEnabled)
                {
                  dataBlock.Struck(this.instance, this.owner, SkillType.Attack, this.ItemID);
                  flag2 = false;
                }
              }
              if (!flag2)
                return;
              byte auxHighDataNoCache = this.map.GetAuxHighDataNoCache(this.SwingTarget);
              if (this.owner.SkillsData.MineReqsMet(this.owner, blockId, this.ItemID))
              {
                this.AccumulateSplinterTime();
                this.StrikeBlock(blockId);
                if ((double) this.owner.SplinterProgress < 1.0)
                  return;
                this.owner.ClearBlock(this, this.ItemID);
                --this.owner.SplinterProgress;
                this.owner.PlayMineSound(this.SwingTarget, blockId, (int) auxHighDataNoCache);
              }
              else
                this.owner.PlayMineSound(this.SwingTarget, blockId, (int) auxHighDataNoCache);
            }
            else if (this.ItemID == Item.DecalApplicator)
            {
              if (this.player == null)
                return;
              this.player.ApplyDecal(this);
              this.owner.OnItemUsed(this);
            }
            else
            {
              if (!this.instance.HasItemSwingEventScript(this.ItemID))
                return;
              this.owner.OnItemUsed(this);
            }
          }
          else
          {
            Sounds.PlaySound(this.ItemID, ItemSoundType.UseFail, (ITMActor) this.owner, false);
            if (!this.instance.HasItemSwingEventScript(this.ItemID))
              return;
            this.owner.OnItemUsed(this);
          }
        }
      }
    }

    private void CloseBehaviourDebug()
    {
      if (this.player.BTCanvas == null)
        return;
      this.player.BTCanvas.RemoveSelf();
      this.player.BTCanvas = (BehaviourTreeDesignWindow) null;
      if (this.currentDebug == null || this.currentDebug.BehaviourTree == null)
        return;
      this.currentDebug.BehaviourTree.TrackType = BehaviourTrackType.None;
    }

    private void AccumulateSplinterTime()
    {
      this.owner.SplinterProgress += ItemData2.GetStrikeBlockPower(this.owner, this.ItemID, (Block) this.map.GetBlockID(this.SwingTarget));
    }

    private void OnSwingFullyExtendedRemote(object sender, EventArgs e)
    {
      if (this.owner.SwingTargetIsValid && ItemData.CanItemBreakBlocks(this.ItemID) && this.owner.SkillsData.UseReqsMet(this.owner, this.ItemID) && (this.IsSingleWieldHand || this.IsDualWielding && ItemData.IsItemUse(this.ItemID, ItemUse.Item)))
      {
        Block blockId = (Block) this.map.GetBlockID(this.SwingTarget);
        if (this.owner.SkillsData.MineReqsMet(this.owner, blockId, this.ItemID))
        {
          this.AccumulateSplinterTime();
          this.instance.AddMiningParticle(this.SwingTarget);
        }
        Sounds.PlaySound((Item) this.map.GetBlockTextureIDForDrawing(blockId, this.SwingTarget), ItemSoundType.Mine, this.SwingTarget, (ITMActor) this.owner);
      }
      if (!this.owner.IsPlayer || ItemData.IsSubType(this.ItemID, ItemSubType.Gun))
        return;
      this.instance.ExecuteItemSwingEventScript((Actor) this.player, this.ItemID);
    }

    private void StrikeHitTarget()
    {
      Item itemId = this.ItemID;
      float num = itemId != Item.Hand || this.player != null ? ItemData.GetItemStrikeReach(itemId) : this.owner.StrikeRange;
      if (this.owner.IsGod)
        num = 30f;
      Actor target = this.owner.HitTarget.Target;
      if ((double) num >= (double) this.owner.HitTarget.Distance)
      {
        TargetingSystem.Target((INPCBehaviour) this.owner, (INPCBehaviour) target);
        if (itemId == Item.Bucket && !target.IsPlayer && (target.ActorType == ActorType.AyrshireCow || target.ActorType == ActorType.HighlandCow))
        {
          if (!this.instance.Random.RandomChance(0.5))
            return;
          target.PlayWarningSound();
          this.FillBucket(Item.BucketOfMilk);
        }
        else if (target.Struck(this.owner, SkillType.Attack, itemId, this.owner.HitTarget.IsCriticalHit))
        {
          if (itemId == Item.Hand)
            return;
          this.owner.OnItemUsed(this);
        }
        else
          Sounds.PlaySound(itemId, ItemSoundType.UseFail, (ITMActor) this.owner, false);
      }
      else
        Sounds.PlaySound(itemId, ItemSoundType.UseFail, (ITMActor) this.owner, false);
    }

    private void StrikeBlock(Block blockID)
    {
      switch (this.ItemID)
      {
        case Item.Bucket:
          if (this.owner.HasPermission(Permissions.Edit) && this.FillBucket(blockID))
            return;
          break;
        case Item.Lighter:
          Sounds.PlaySound(Item.Lighter, (ITMActor) this.owner, false);
          if (!this.instance.Random.RandomChance(0.25))
            return;
          this.instance.StartLiveFire(this.SwingTarget, blockID, this.player.PlaceTarget, UpdateBlockMethod.Player, this.owner.GamerID, true);
          this.owner.OnItemUsed(this);
          return;
        case Item.BoneMeal:
          if (this.owner.HasPermission(Permissions.Creative) && this.ApplyBoneMeal(this.SwingTarget, blockID))
          {
            this.owner.SplinterProgress = 0.0f;
            this.owner.Splinter = -1;
            this.owner.OnItemUsed(this);
            return;
          }
          break;
      }
      if (blockID == Block.ArcadeMachine)
      {
        this.instance.HitArcadeMachine(this.player, this.SwingTarget, this.owner.SwingFace, this.ItemID);
      }
      else
      {
        if ((double) this.owner.SplinterProgress > 0.0)
          this.instance.AddMiningParticle(this.SwingTarget);
        this.owner.PlayMineSound(this.SwingTarget, blockID, (int) this.map.GetAuxHighDataNoCache(this.SwingTarget));
      }
    }

    private bool ApplyBoneMeal(GlobalPoint3D p, Block blockID)
    {
      Block block1 = blockID;
      if ((uint) block1 <= 79U)
      {
        switch (block1)
        {
          case Block.Grass:
          case Block.Dirt:
          case Block.GrassyStone:
            break;
          default:
            goto label_10;
        }
      }
      else if (block1 != Block.GrassShaded && block1 != Block.TilledEarth)
        goto label_10;
      GlobalPoint3D p1 = p + GlobalPoint3D.Up;
      bool flag = !this.instance.IsInZoneType(p1, ZoneType.NoEdit, this.owner.GamerID);
      switch ((Block) this.map.GetBlockID(p1))
      {
        case Block.Sapling:
          if (flag)
            this.ApplyBoneMealToSapling(p1);
          return true;
        case Block.Crop:
          if (flag)
            this.ApplyBoneMealToCrop(p1);
          return true;
      }
label_10:
      if (this.instance.IsInZoneType(p, ZoneType.NoEdit, this.owner.GamerID))
        return false;
      Block block2 = blockID;
      if ((uint) block2 <= 79U)
      {
        switch (block2)
        {
          case Block.Grass:
          case Block.Dirt:
          case Block.GrassyStone:
            break;
          case Block.Sapling:
            this.ApplyBoneMealToSapling(p);
            return true;
          default:
            goto label_17;
        }
      }
      else
      {
        switch (block2)
        {
          case Block.Crop:
            this.ApplyBoneMealToCrop(p);
            return true;
          case Block.GrassShaded:
          case Block.TilledEarth:
            break;
          default:
            goto label_17;
        }
      }
      this.ApplyBoneMealToGrass(p);
      return true;
label_17:
      return false;
    }

    private void ApplyBoneMealToSapling(GlobalPoint3D p)
    {
      if (this.instance.MapStrategyTM.ActivateTimedBlock(p))
        return;
      this.instance.MapStrategyTM.SpawnTree(p, true);
    }

    private void ApplyBoneMealToCrop(GlobalPoint3D p)
    {
      this.instance.MapStrategyTM.GrowCropBlockOneStage(p, this.instance.IsCreativeMode && (!this.instance.IsFiniteResources || this.owner.IsAdmin));
    }

    private void ApplyBoneMealToGrass(GlobalPoint3D p)
    {
      VegetationGenerator.GrassDecoration((Map) this.map, p, 1f, 1, 5, 5, 0.25f, 10, this.map.MapHeight, this.map.Random, UpdateBlockMethod.Player, this.owner.GamerID, true);
      VegetationGenerator.FlowerDecoration((Map) this.map, p, 1f, 1, 5, 6, 0.5f, this.map.MapHeight, this.map.Random, UpdateBlockMethod.Player, this.owner.GamerID, true);
      this.map.Commit();
    }

    private bool FillBucket()
    {
      return this.FillBucket((Block) this.map.GetBlockID(this.SwingTarget));
    }

    private bool FillBucket(Block blockID)
    {
      bool flag = false;
      if (!this.map.MapStrategyTM.IsInZoneType(this.SwingTarget, ZoneType.NoEdit, this.owner.GamerID) && this.map.GetAuxData(this.SwingTarget) == (byte) 0)
      {
        switch (blockID)
        {
          case Block.Water:
            flag = this.FillBucket(Item.BucketOfWater);
            break;
          case Block.Lava:
            flag = this.FillBucket(Item.BucketOfLava);
            break;
        }
        if (flag)
        {
          this.instance.AddMiningParticles(this.SwingTarget, blockID, (byte) 0, 5, 0.08f, 0.04f, 1f, 1.5f);
          this.instance.ClearBlock(this.SwingTarget, UpdateBlockMethod.Player, this.owner.GamerID, true);
          this.map.Commit();
        }
      }
      return flag;
    }

    private bool FillBucket(Item itemID)
    {
      if (this.owner.AddToInventory(itemID) <= 0)
        return false;
      this.inventory.DecrementItem(this.HandIndexOnSwingStart);
      return true;
    }

    private void OnSwingComplete(object sender, EventArgs e)
    {
      if (this.nextItemID != Item.None)
      {
        this.updateSwing = false;
        Item itemId = this.ItemID;
        this.ItemID = this.nextItemID;
        this.owner.SplinterProgress = 0.0f;
        this.OnItemChanged(itemId);
      }
      this.handIndexOnSwingStart = -1;
    }

    private void OnItemChanged(Item oldItemID)
    {
      if (oldItemID < Item.zLastBlockID)
        this.owner.SetCurrentBlockAux((Block) oldItemID, (byte) 0);
      if (this.player != null)
        this.player.SetCurrentBlockTexture(Block.zLastBlockID, 0);
      this.ItemSwing.Initialize(this.ItemID);
      this.nextItemID = Item.None;
    }

    private void ShootWeapon(Item weapon)
    {
      bool flag = false;
      if (this.owner.SkillsData.UseReqsMet(this.owner, this.ItemID) && this.inventory.HasAmmunition(weapon))
      {
        if (weapon == Item.GrenadeLauncher)
          flag = this.ShootGrenade();
        else if (ItemData.IsSubType(weapon, ItemSubType.Bow))
          flag = this.ShootBow();
        else if (ItemData.IsSubType(weapon, ItemSubType.Gun))
        {
          this.owner.OnItemUsed(this);
          flag = true;
        }
      }
      if (flag)
        return;
      Sounds.PlaySound(weapon, ItemSoundType.UseFail, (ITMActor) this.owner, true);
    }

    private bool ShootBow()
    {
      InventoryItem inventoryItem = this.inventory[this.OtherHandIndex];
      if (this.owner.SkillsData.UseReqsMet(this.owner, inventoryItem.ItemID))
      {
        Vector3 currYawPitchRoll = this.ItemSwing.AnimData.CurrYawPitchRoll;
        Vector3 currPosition = this.ItemSwing.AnimData.CurrPosition;
        if (this.HandType == InventoryHand.Left)
        {
          currPosition.X = -currPosition.X;
          currYawPitchRoll.X = -currYawPitchRoll.X;
          currYawPitchRoll.Z = -currYawPitchRoll.Z;
        }
        Vector3 vector3_1 = Vector3.Transform(Vector3.Zero, Matrix.CreateFromYawPitchRoll(currYawPitchRoll.X, currYawPitchRoll.Y, currYawPitchRoll.Z) * Matrix.CreateTranslation(currPosition) * Matrix.Invert(this.owner.ViewMatrix));
        Vector3 vector3_2 = this.owner.Position - (vector3_1 - this.owner.Position) + this.owner.ViewDirection * 30f;
        vector3_2.Y += 6f;
        Vector3 position = vector3_1 + this.owner.ViewDirection * 0.15f;
        Vector3 velocity = vector3_2 - position;
        if ((double) position.Y > (double) this.map.MapBound.Min.Y + (double) this.map.TileSize && this.instance.ParticleManager.AddNew(ParticleType.Projectile, 10f, position, velocity, 0.4f, new InventoryItem(inventoryItem.ItemID, 1), this.instance.ParticleModifiers.ProjectileParticleModifier, 0.0f, -1, (byte) 0, this.owner.GamerID, false, true) >= 0)
        {
          this.owner.OnItemUsed(this);
          if (this.ItemID != Item.ElvenBow)
            this.owner.OnItemUsed(this.OtherHand);
          NetworkManager.Instance.SendProjectile(position, velocity, inventoryItem.ItemID, this.owner.GamerID, true);
          if (!Sounds.PlaySound(this.ItemID, (ITMActor) this.owner, true))
            Sounds.PlaySound(inventoryItem.ItemID, (ITMActor) this.owner, true);
          return true;
        }
      }
      return false;
    }

    private bool ShootGrenade()
    {
      InventoryItem inventoryItem = this.inventory[this.OtherHandIndex];
      if (this.owner.SkillsData.UseReqsMet(this.owner, inventoryItem.ItemID))
      {
        Vector3 currYawPitchRoll = this.ItemSwing.AnimData.CurrYawPitchRoll;
        Vector3 currPosition = this.ItemSwing.AnimData.CurrPosition;
        if (this.HandType == InventoryHand.Left)
        {
          currPosition.X = -currPosition.X;
          currYawPitchRoll.X = -currYawPitchRoll.X;
          currYawPitchRoll.Z = -currYawPitchRoll.Z;
        }
        Vector3 vector3_1 = Vector3.Transform(Vector3.Zero, Matrix.CreateFromYawPitchRoll(currYawPitchRoll.X, currYawPitchRoll.Y, currYawPitchRoll.Z) * Matrix.CreateTranslation(currPosition) * Matrix.Invert(this.owner.ViewMatrix));
        Vector3 vector3_2 = this.owner.Position - (vector3_1 - this.owner.Position) + this.owner.ViewDirection * 30f;
        vector3_2.Y += 6f;
        Vector3 position = vector3_1 + this.owner.ViewDirection * 0.15f;
        Vector3 velocity = vector3_2 - position;
        if (this.instance.ParticleManager.AddNew(ParticleType.None, 10f, position, velocity, 0.2f, new InventoryItem(Item.Obsidian, 1), this.instance.ParticleModifiers.GrenadeParticleModifier, 0.0f, -1, (byte) 0, this.owner.GamerID, false, true) >= 0)
        {
          this.owner.OnItemUsed(this);
          this.owner.OnItemUsed(this.OtherHand);
          NetworkManager.Instance.SendProjectile(position, velocity, inventoryItem.ItemID, this.owner.GamerID, true);
          if (Sounds.PlaySound(this.ItemID, (ITMActor) this.owner, true))
            Sounds.PlaySound(inventoryItem.ItemID, (ITMActor) this.owner, true);
          if (this.player != null)
          {
            ++this.player.Statistics.GrenadesLaunched;
            this.player.Raise_GrenadeLaunched();
          }
          return true;
        }
      }
      return false;
    }

    private float GetShootTime(Item weapon)
    {
      return Globals1.ItemSwingTimeData[(int) weapon].Time * MathHelper.Lerp(1f, 0.6f, this.owner.SkillsData.GetUseSkill(weapon).LevelWithBonuses(this.owner) / 99f);
    }
  }
}
