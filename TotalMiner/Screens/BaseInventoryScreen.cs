// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BaseInventoryScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal abstract class BaseInventoryScreen : MinerToolScreen
  {
    protected int oldSlot = -1;
    protected string itemTypeName = "";
    protected const int slotSize = 49;
    protected SpriteBatchSafe spriteBatchPoint;
    protected SpriteBatchSafe spriteBatchText;
    protected GameInstance instance;
    protected Actor inventoryOwner;
    protected int currentSlotID;
    protected int thumbstickTimer;
    protected bool isItemLifted;
    protected InventoryItem liftedItem;
    protected InventoryScreenType screenType;
    protected Texture2D lockedTexture;
    protected bool showItemCounts;
    protected int singlePressQuantity;
    protected Inventory inventory;
    protected Action onExit;
    protected bool ignoreLocked;
    private float rapidSinglePressTimer;
    private float rapidSinglePressAlreadyHeldTimer;
    private Vector2 leftstick;
    private Vector2 rightstick;
    private Vector2 lastleftstick;
    private Vector2 lastrightstick;
    private Texture2D crossOutTexture;

    protected abstract int CoWindowHeight { get; }

    protected virtual int CoWindowWidth
    {
      get
      {
        return 495;
      }
    }

    protected virtual Inventory CoWindowInventory
    {
      get
      {
        return (Inventory) null;
      }
    }

    protected bool IsPlayerInventory
    {
      get
      {
        if (this.player != null)
          return this.inventory == this.player.Inventory;
        return false;
      }
    }

    protected virtual bool AllowRapidLiftSinglePressX
    {
      get
      {
        return false;
      }
    }

    protected virtual bool AllowRapidLiftSinglePressA
    {
      get
      {
        return false;
      }
    }

    protected virtual Inventory CursorInventory
    {
      get
      {
        return this.inventory;
      }
    }

    protected virtual InventoryItem CursorItem
    {
      get
      {
        if (this.inventory[this.currentSlotID].Count <= 0)
          return InventoryItem.Empty;
        return this.inventory[this.currentSlotID];
      }
      set
      {
        this.inventory[this.currentSlotID] = value;
      }
    }

    protected virtual int CursorItemCount
    {
      get
      {
        return this.CursorItem.Count;
      }
      set
      {
        if (this.currentSlotID < 0)
          return;
        this.inventory.SetItemCount((int) (ushort) this.currentSlotID, value);
      }
    }

    protected bool CursorItemLocked
    {
      get
      {
        if (this.instance.IsItemLocked(this.CursorInventory.AllowZeroCountItems ? this.CursorItem.ItemID_Raw : this.CursorItem.ItemID))
          return !this.player.IsGodOrTester;
        return false;
      }
    }

    protected bool IsItemLifted
    {
      get
      {
        if (this.isItemLifted)
          return this.liftedItem.ItemID != Item.None;
        return false;
      }
    }

    protected virtual bool IsFiniteMode
    {
      get
      {
        if (this.instance != null)
          return this.instance.IsFiniteResources;
        return false;
      }
    }

    protected virtual bool IsCreativeMode
    {
      get
      {
        if (this.instance != null)
          return this.instance.IsCreativeMode;
        return false;
      }
    }

    protected virtual InventoryItem ItemToExamine
    {
      get
      {
        return this.CursorItem;
      }
    }

    public BaseInventoryScreen(
      GameInstance instance,
      Player player,
      Inventory inventory,
      int cursorSlotID)
      : this(instance, player, (Actor) null, inventory, cursorSlotID)
    {
    }

    public BaseInventoryScreen(
      GameInstance instance,
      Player player,
      Actor inventoryOwner,
      int cursorSlotID)
      : this(instance, player, inventoryOwner, (Inventory) null, cursorSlotID)
    {
    }

    public BaseInventoryScreen(
      GameInstance instance,
      Player player,
      Actor inventoryOwner,
      Inventory inventory,
      int cursorSlotID)
      : base(player)
    {
      this.instance = instance;
      this.inventoryOwner = inventoryOwner;
      if (inventory == null)
        inventory = (Inventory) inventoryOwner.Inventory;
      this.inventory = inventory;
      this.currentSlotID = cursorSlotID;
      this.screenType = InventoryScreenType.Hand;
      inventory.SuspendItemsChangedTransmission = true;
      inventory.ItemCleared += new IntEventHandler(this.OnItemCleared);
      inventory.ItemsCleared += new EventHandler(this.OnItemsCleared);
      ++player.HotBarVisibilityStack;
      if (!this.IsPlayerInventory)
        return;
      if (inventoryOwner == null)
        inventoryOwner = (Actor) player;
      player.OverrideIsEnabledInShop = false;
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchText = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.screenRect = MyExtensions.CenterOfViewport(this.CoWindowWidth, 199 + this.CoWindowHeight);
      base.LoadContent();
      this.lockedTexture = this.content.Load<Texture2D>("Textures\\smalllocked");
      this.crossOutTexture = this.content.Load<Texture2D>("Textures\\crossout");
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchText);
    }

    protected override void OnScreenRemovedCore()
    {
      --this.player.HotBarVisibilityStack;
      this.inventory.ItemCleared -= new IntEventHandler(this.OnItemCleared);
      this.inventory.ItemsCleared -= new EventHandler(this.OnItemsCleared);
      if (this.IsPlayerInventory)
        this.player.OverrideIsEnabledInShop = false;
      base.OnScreenRemovedCore();
      this.OnScreenClosed();
      this.inventory.SuspendItemsChangedTransmission = false;
      if (this.onExit == null)
        return;
      this.onExit();
    }

    protected virtual void OnScreenClosed()
    {
      if (!this.IsPlayerInventory || !this.IsItemLifted)
        return;
      this.liftedItem.Count -= this.player.AddToInventory(this.liftedItem);
      if (this.liftedItem.Count <= 0)
        return;
      this.player.DropItem(ParticleType.None, this.liftedItem, UpdateBlockMethod.DropTimeShort);
    }

    private void OnItemCleared(object sender, IntEventArgs e)
    {
      if ((Item) e.Value != this.liftedItem.ItemID)
        return;
      this.liftedItem = InventoryItem.Empty;
    }

    private void OnItemsCleared(object sender, EventArgs e)
    {
      this.liftedItem = InventoryItem.Empty;
    }

    public virtual bool HandleInput(GamePadState pad, GamePadState lastpad)
    {
      return false;
    }

    public override bool HandleInput(InputState input)
    {
      if (base.HandleInput(input))
        return true;
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp) || InputManager.GetMouseWheelDelta(this.ControllingPlayer) < 0)
      {
        this.PrevPageButtonPressed();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown) || InputManager.GetMouseWheelDelta(this.ControllingPlayer) > 0)
      {
        this.NextPageButtonPressed();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.ExamineItem))
      {
        this.ExamineButtonPressed();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.DropItem))
      {
        this.DropKeyPressed();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.TransferItem) && this.CanEditInventory)
      {
        this.TransferItems();
        return true;
      }
      if (InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.SelectItem))
      {
        if (this.AllowRapidLiftSinglePressA)
        {
          if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem))
          {
            this.rapidSinglePressTimer = 0.8f;
            this.rapidSinglePressAlreadyHeldTimer = 0.0f;
            if (this.CanEditInventory)
            {
              this.LiftAllButtonPressed();
              return true;
            }
          }
          else
          {
            this.rapidSinglePressTimer -= Services.ElapsedTime;
            this.rapidSinglePressAlreadyHeldTimer += Services.ElapsedTime;
          }
          if ((double) this.rapidSinglePressTimer < 0.0 && (double) this.rapidSinglePressAlreadyHeldTimer > 0.200000002980232)
          {
            this.rapidSinglePressAlreadyHeldTimer = 0.0f;
            if (this.CanEditInventory)
            {
              this.LiftAllButtonPressed();
              return true;
            }
          }
        }
        else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem) && this.CanEditInventory)
        {
          this.LiftAllButtonPressed();
          return true;
        }
      }
      if (InputManager1.IsInputPressed(this.ControllingPlayer, GuiInput.LiftItemSingle))
      {
        if (this.AllowRapidLiftSinglePressX)
        {
          if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.LiftItemSingle))
          {
            this.rapidSinglePressTimer = 0.0f;
            this.rapidSinglePressAlreadyHeldTimer = 0.0f;
            this.singlePressQuantity = 100;
          }
          else
          {
            this.rapidSinglePressTimer -= Services.ElapsedTime;
            this.rapidSinglePressAlreadyHeldTimer += Services.ElapsedTime;
          }
        }
        if ((!this.AllowRapidLiftSinglePressX && InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.LiftItemSingle) || this.AllowRapidLiftSinglePressX && (double) this.rapidSinglePressTimer <= 0.0) && this.CanEditInventory)
        {
          this.LiftSingleButtonPressed();
          if (this.AllowRapidLiftSinglePressX)
          {
            this.rapidSinglePressTimer = this.GetRapidSinglePressTimerDuration();
            this.singlePressQuantity = this.GetRapidSinglePressQuantity();
          }
          return true;
        }
      }
      if (InputManager.IsMouseMoved(this.ControllingPlayer.Value))
      {
        Point mousePos = InputManager.GetMousePos(this.ControllingPlayer);
        if (!this.SetCurrentSlotCoWindow(mousePos))
          this.SetCurrentSlot(mousePos);
        return true;
      }
      int num1 = 10;
      int num2 = 15;
      this.lastleftstick = this.leftstick;
      this.lastrightstick = this.rightstick;
      this.leftstick = currentGamePadState.ThumbSticks.Left;
      this.rightstick = currentGamePadState.ThumbSticks.Right;
      float stickDeadzone = Globals1.StickDeadzone;
      if ((double) this.leftstick.X > -(double) stickDeadzone && (double) this.leftstick.X < (double) stickDeadzone)
        this.leftstick.X = 0.0f;
      if ((double) this.leftstick.Y > -(double) stickDeadzone && (double) this.leftstick.Y < (double) stickDeadzone)
        this.leftstick.Y = 0.0f;
      if ((double) this.rightstick.X > -(double) stickDeadzone && (double) this.rightstick.X < (double) stickDeadzone)
        this.rightstick.X = 0.0f;
      if ((double) this.rightstick.Y > -(double) stickDeadzone && (double) this.rightstick.Y < (double) stickDeadzone)
        this.rightstick.Y = 0.0f;
      if (this.leftstick != Vector2.Zero || this.rightstick != Vector2.Zero)
      {
        if (this.IsEitherStickMoved(this.leftstick, this.rightstick, this.lastleftstick, this.lastrightstick))
          this.thumbstickTimer = num2 + 1;
        else
          ++this.thumbstickTimer;
      }
      else
        this.thumbstickTimer = 0;
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorLeft) || (double) this.leftstick.X < 0.0 && this.thumbstickTimer > num1 || (double) this.rightstick.X < 0.0 && this.thumbstickTimer > num1)
        this.MoveLeft();
      else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorRight) || (double) this.leftstick.X > 0.0 && this.thumbstickTimer > num1 || (double) this.rightstick.X > 0.0 && this.thumbstickTimer > num1)
        this.MoveRight();
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown) || (double) this.leftstick.Y < 0.0 && this.thumbstickTimer > num2 || (double) this.rightstick.Y < 0.0 && this.thumbstickTimer > num2)
        this.MoveDown();
      else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp) || (double) this.leftstick.Y > 0.0 && this.thumbstickTimer > num2 || (double) this.rightstick.Y > 0.0 && this.thumbstickTimer > num2)
        this.MoveUp();
      if (this.currentSlotID == this.oldSlot)
        return this.HandleInput(currentGamePadState, lastGamePadState);
      this.CursorMoved();
      return true;
    }

    protected virtual void SetCurrentSlot(Point pos)
    {
      int num1 = (int) this.inventory.PackSize / 10;
      int num2 = 30;
      Rectangle rectangle = new Rectangle(0, 0, 46, 46);
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          rectangle.X = index2 * 46 + this.screenRect.X + 16;
          rectangle.Y = this.screenRect.Y + this.screenRect.Height - num2 - (index1 + 1) * 46 - 3;
          int num3 = index2 + index1 * 10;
          if (num3 > 9)
            rectangle.Y -= 16;
          if (rectangle.Contains(pos))
          {
            this.currentSlotID = num3;
            return;
          }
        }
      }
    }

    protected virtual bool SetCurrentSlotCoWindow(Point pos)
    {
      return false;
    }

    private float GetRapidSinglePressTimerDuration()
    {
      if ((double) this.rapidSinglePressAlreadyHeldTimer < 2.0)
        return 0.7f;
      if ((double) this.rapidSinglePressAlreadyHeldTimer < 5.0)
        return 0.4f;
      return (double) this.rapidSinglePressAlreadyHeldTimer < 7.0 ? 0.2f : 0.1f;
    }

    private int GetRapidSinglePressQuantity()
    {
      if ((double) this.rapidSinglePressAlreadyHeldTimer < 4.0)
        return 100;
      if ((double) this.rapidSinglePressAlreadyHeldTimer < 8.0)
        return 1000;
      return (double) this.rapidSinglePressAlreadyHeldTimer < 12.0 ? 10000 : 20000;
    }

    protected virtual bool CanEditInventory
    {
      get
      {
        return true;
      }
    }

    protected virtual void ShowPermissionDenied()
    {
      CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You do not have permission to do this", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, this.player), this.ControllingPlayer);
    }

    protected virtual void PrevPageButtonPressed()
    {
    }

    protected virtual void NextPageButtonPressed()
    {
    }

    private void ExamineButtonPressed()
    {
      InventoryItem itemToExamine = this.ItemToExamine;
      if (itemToExamine.ItemID == Item.None)
        return;
      if (!this.CursorItemLocked)
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuMoveSound);
        if (this.ExamineItemOverride(itemToExamine))
          return;
        switch (itemToExamine.ItemID)
        {
          case Item.Wisdom:
            this.ScreenManager.AddScreen((GameScreen) new WisdomPickupScreen(this.player, (int) itemToExamine.Durability), this.ControllingPlayer);
            break;
          case Item.Blueprint:
            this.ScreenManager.AddScreen((GameScreen) new BlueprintPickupScreen(this.player, (int) itemToExamine.Durability), this.ControllingPlayer);
            break;
          case Item.Book:
            this.ScreenManager.AddScreen((GameScreen) new BookCoverScreen(this.instance, this.player, this.instance.GetBookData((ushort) (byte) itemToExamine.Durability), this.currentSlotID), this.ControllingPlayer);
            Sounds.PlaySound(Item.Book, ItemSoundType.Use);
            break;
          default:
            this.ScreenManager.AddScreen((GameScreen) new InteractItemScreen(this.instance, this.player, itemToExamine), this.ControllingPlayer);
            break;
        }
      }
      else
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
    }

    protected virtual bool ExamineItemOverride(InventoryItem item)
    {
      return false;
    }

    private bool IsEitherStickMoved(
      Vector2 leftstick,
      Vector2 rightstick,
      Vector2 lastleftstick,
      Vector2 lastrightstick)
    {
      if (Math.Sign(leftstick.X) == Math.Sign(lastleftstick.X) && Math.Sign(leftstick.Y) == Math.Sign(lastleftstick.Y) && Math.Sign(rightstick.X) == Math.Sign(lastrightstick.X))
        return Math.Sign(rightstick.Y) != Math.Sign(lastrightstick.Y);
      return true;
    }

    protected void CursorMoved()
    {
      this.RefreshItemTypeName();
      this.oldSlot = this.currentSlotID;
    }

    protected void RefreshItemTypeName()
    {
      InventoryItem inventoryItem = this.IsItemLifted ? this.liftedItem : this.CursorItem;
      this.itemTypeName = ItemData2.ForDisplay(this.player, new InventoryItem(this.CursorInventory.AllowZeroCountItems ? inventoryItem.ItemID_Raw : inventoryItem.ItemID, !this.CursorInventory.AllowZeroCountItems || inventoryItem.Count != 0 ? inventoryItem.Count : 1, inventoryItem.Durability));
      if (inventoryItem.Count > 1)
        this.itemTypeName += string.Format(" ({0:N0})", (object) inventoryItem.Count);
      if (this.itemTypeName.Length <= this.MaxItemTypeDescLength)
        return;
      this.itemTypeName = this.itemTypeName.Substring(0, this.MaxItemTypeDescLength) + "..";
    }

    protected virtual int MaxItemTypeDescLength
    {
      get
      {
        return 40;
      }
    }

    protected virtual void DropKeyPressed()
    {
      if (this.currentSlotID >= (int) this.inventory.PackSize || (this.CursorItem.Count <= 0 || !this.IsPlayerInventory))
        return;
      this.player.Raise_ItemThrown(this.CursorItem.ItemID);
      this.player.DropItem(ParticleType.None, this.currentSlotID, UpdateBlockMethod.DropTimeShort);
      CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuMoveSound);
    }

    protected virtual bool IsLiftValid
    {
      get
      {
        return true;
      }
    }

    protected virtual bool IsLiftAllValid
    {
      get
      {
        return true;
      }
    }

    protected virtual bool IsUnliftValid
    {
      get
      {
        return true;
      }
    }

    protected virtual bool IsLiftSingleValid
    {
      get
      {
        return true;
      }
    }

    protected virtual void LiftAllButtonPressed()
    {
      if (this.IsLiftAllValid)
      {
        this.oldSlot = -1;
        if (this.IsItemLifted)
          this.UnliftItem();
        else
          this.LiftItem();
        this.OnLiftAllButtonPressedCore();
      }
      else
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
    }

    protected virtual void LiftItem()
    {
      if (!this.IsLiftValid)
        return;
      InventoryItem cursorItem = this.CursorItem;
      if (cursorItem.ItemID == Item.None)
        return;
      this.isItemLifted = true;
      this.liftedItem = cursorItem;
      this.CursorItemCount = 0;
      if (!this.IsPlayerInventory)
        return;
      this.player.Raise_InventoryScreenItemSelected(this.liftedItem, this.currentSlotID);
    }

    protected virtual void UnliftItem()
    {
      InventoryItem cursorItem = this.CursorItem;
      if (cursorItem.ItemID == this.liftedItem.ItemID && cursorItem.MaxDurability == (ushort) 0)
      {
        if (!this.IsUnliftValid)
          return;
        int num = Math.Min(this.liftedItem.Count, ItemData.GetStackSize(this.liftedItem.ItemID) - cursorItem.Count);
        if (num <= 0)
          return;
        this.CursorItemCount += num;
        this.liftedItem.Count -= num;
        this.isItemLifted = this.liftedItem.Count > 0;
      }
      else
      {
        if (!this.IsUnliftValid)
          return;
        this.CursorItem = this.liftedItem;
        if (this.IsPlayerInventory)
          this.player.Raise_InventoryScreenItemPlaced(this.liftedItem, this.currentSlotID);
        if (cursorItem.ItemID != Item.None)
        {
          this.liftedItem = cursorItem;
          if (!this.IsPlayerInventory)
            return;
          this.player.Raise_InventoryScreenItemSelected(cursorItem, this.currentSlotID);
        }
        else
          this.isItemLifted = false;
      }
    }

    protected virtual void OnLiftAllButtonPressedCore()
    {
    }

    protected virtual void OnLiftSingleButtonPressedCore()
    {
    }

    protected virtual void LiftSingleButtonPressed()
    {
      this.oldSlot = -1;
      if (this.IsLiftSingleValid)
      {
        if (this.IsItemLifted)
        {
          InventoryItem cursorItem = this.CursorItem;
          if (cursorItem.ItemID == this.liftedItem.ItemID)
          {
            if (this.IsUnliftValid && cursorItem.MaxDurability == (ushort) 0 && ItemData.GetStackSize(cursorItem.ItemID) - this.CursorItemCount > 0)
            {
              int num = 1;
              this.liftedItem.Count -= num;
              this.CursorItemCount += num;
              if (this.IsPlayerInventory)
                this.player.Raise_InventoryScreenItemPlaced(this.CursorItem, this.currentSlotID);
            }
          }
          else if (cursorItem.ItemID == Item.None && this.liftedItem.Count > 1)
          {
            if (this.IsUnliftValid)
              this.UnliftSingleItem();
          }
          else if (this.IsUnliftValid && (cursorItem.ItemID == Item.None || this.IsLiftValid))
          {
            this.CursorItem = this.liftedItem;
            if (cursorItem.ItemID != Item.None)
              this.liftedItem = cursorItem;
            else
              this.isItemLifted = false;
            if (this.IsPlayerInventory)
              this.player.Raise_InventoryScreenItemPlaced(this.CursorItem, this.currentSlotID);
          }
        }
        else if (this.IsLiftValid)
        {
          InventoryItem cursorItem = this.CursorItem;
          if (cursorItem.ItemID != Item.None)
          {
            this.isItemLifted = true;
            this.liftedItem = cursorItem;
            int count = cursorItem.Count;
            this.liftedItem.Count = (count + 1) / 2;
            this.CursorItemCount = count - this.liftedItem.Count;
            if (this.IsPlayerInventory)
              this.player.Raise_InventoryScreenItemSelected(cursorItem, this.currentSlotID);
          }
        }
      }
      this.OnLiftSingleButtonPressedCore();
    }

    protected virtual void UnliftSingleItem()
    {
      this.CursorItem = this.liftedItem;
      int num = 1;
      this.liftedItem.Count -= num;
      this.CursorItemCount = num;
      if (!this.IsPlayerInventory)
        return;
      this.player.Raise_InventoryScreenItemPlaced(this.CursorItem, this.currentSlotID);
    }

    protected virtual void MoveLeft()
    {
      if (this.currentSlotID == 0)
        this.currentSlotID = 9;
      else if (this.currentSlotID == 10)
        this.currentSlotID = 19;
      else if (this.currentSlotID == 20)
        this.currentSlotID = 29;
      else
        this.MoveLeftCore();
      this.thumbstickTimer = 0;
    }

    protected virtual void MoveLeftCore()
    {
      --this.currentSlotID;
    }

    protected virtual void MoveRight()
    {
      if (this.currentSlotID == 9)
        this.currentSlotID = 0;
      else if (this.currentSlotID == 19)
        this.currentSlotID = 10;
      else if (this.currentSlotID == 29)
        this.currentSlotID = 20;
      else
        this.MoveRightCore();
      this.thumbstickTimer = 0;
    }

    protected virtual void MoveRightCore()
    {
      ++this.currentSlotID;
    }

    private void MoveUp()
    {
      if (this.currentSlotID < (int) this.inventory.PackSize - 10)
        this.currentSlotID += 10;
      else
        this.MoveUpCore();
      this.thumbstickTimer = 0;
    }

    protected virtual void MoveUpCore()
    {
    }

    private void MoveDown()
    {
      if (this.currentSlotID > 9 && this.currentSlotID < (int) this.inventory.PackSize)
        this.currentSlotID -= 10;
      else
        this.MoveDownCore();
      this.thumbstickTimer = 0;
    }

    protected virtual void MoveDownCore()
    {
    }

    protected virtual void TransferItems()
    {
      if (this.CursorItemCount <= 0)
        return;
      if (this.currentSlotID >= (int) this.inventory.PackSize)
        this.TransferCursorItemTo(this.inventory);
      else
        this.TransferCursorItemTo(this.CoWindowInventory);
    }

    protected void TransferCursorItemTo(Inventory inventory)
    {
      if (inventory == null)
        return;
      int num = inventory.TransferTo(this.CursorItem);
      if (num > 0)
      {
        this.CursorItemCount -= num;
        Sounds.PlaySound(ItemSoundGroup.GuiTransfer);
      }
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    protected override void DrawCore()
    {
      this.DrawBorder(this.spriteBatch, this.screenRect);
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatchText.BeginTM(this.Matrix);
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, this.Matrix);
      this.DrawGrid((int) this.inventory.PackSize / 10, 30, this.inventory, 0, 0, false, false);
      this.DrawCoWindow();
      this.DrawCursor();
      this.DrawBaseLine();
      this.spriteBatch.End();
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
    }

    protected virtual void DrawBorder(SpriteBatchSafe spriteBatch, Rectangle rect)
    {
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, rect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      spriteBatch.End();
    }

    protected virtual void DrawBaseLine()
    {
      if (this.itemTypeName == null || this.itemTypeName.Length <= 0)
        return;
      this.spriteBatch.DrawString(this.Font, this.ItemDescriptionPanelText, new Vector2((float) (this.screenRect.X + 16), (float) (this.screenRect.Y + this.screenRect.Height - 22)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected virtual string ItemDescriptionPanelText
    {
      get
      {
        return this.itemTypeName;
      }
    }

    protected virtual void DrawCoWindow()
    {
    }

    protected void DrawGrid(
      int height,
      int yoffset,
      Inventory inventory,
      int itemSlotOffset,
      int pageOffset,
      bool drawZeroCountItems,
      bool crossOutZeroCountItems)
    {
      Rectangle slotRect = new Rectangle(0, 0, 49, 49);
      for (int index1 = 0; index1 < height; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          slotRect.X = index2 * 46 + this.screenRect.X + 16;
          slotRect.Y = this.screenRect.Y + this.screenRect.Height - yoffset - (index1 + 1) * 46 - 3;
          int slotID = index2 + index1 * 10 + itemSlotOffset + pageOffset;
          if (slotID > 9)
            slotRect.Y -= 16;
          this.DrawSlot(slotRect.X, slotRect.Y);
          InventoryItem inventoryItem = !this.IsItemLifted || slotID != this.currentSlotID + pageOffset ? inventory[slotID - itemSlotOffset] : this.liftedItem;
          if (inventoryItem.ItemID != Item.None || inventoryItem.ItemID_Raw != Item.None && drawZeroCountItems)
            this.DrawItem(slotRect, slotID, inventoryItem, crossOutZeroCountItems, BaseInventoryScreen.SkillCompare.Equip);
        }
      }
    }

    protected virtual void DrawCursor()
    {
      if (this.currentSlotID < 0)
        return;
      GraphicStatics.DrawCursor(this.spriteBatch, this.GetSlotRect(this.currentSlotID), this.IsItemLifted ? (this.IsUnliftValid ? Color.Blue : Color.Red) : Color.Yellow);
    }

    protected void DrawSlot(int x, int y)
    {
      Rectangle rect = new Rectangle(x, y, 49, 49);
      Color color = new Color(0.8f, 0.8f, 0.8f, 1f);
      this.spriteBatch.DrawFilledBox(rect, 3, color, color * 0.25f);
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.X = rect.X + 3;
      destinationRectangle.Y = rect.Y + 3;
      destinationRectangle.Width = rect.Width - 6;
      destinationRectangle.Height = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
      destinationRectangle.Height = rect.Height - 6;
      destinationRectangle.Width = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
    }

    protected void DrawItem(
      Rectangle slotRect,
      int slotID,
      InventoryItem item,
      bool crossOutZeroCountItems,
      BaseInventoryScreen.SkillCompare skillCompare)
    {
      Rectangle rectangle = slotRect;
      slotRect.X += 9;
      slotRect.Y += 9;
      slotRect.Width = slotRect.Height = 32;
      float drawItemColorAlpha = this.GetDrawItemColorAlpha(slotID, item);
      bool flag = this.player != null && this.player.OverrideIsEnabledInShop;
      if (slotID >= (int) this.inventory.PackSize && !flag && (this.instance.IsItemLocked(item.ItemID_Raw) && !this.player.IsGodOrTester))
      {
        this.spriteBatchPoint.Draw(this.lockedTexture, slotRect, Color.White * drawItemColorAlpha);
      }
      else
      {
        if (this.IsDrawItemRedBackground(slotID, item, skillCompare))
          this.spriteBatchPoint.Draw(CoreGlobals.BlankTexture, rectangle.Expand(-3), Color.DarkRed * 0.3f);
        Item itemID = crossOutZeroCountItems || this.showItemCounts ? item.ItemID_Raw : item.ItemID;
        if (itemID != Item.None)
        {
          this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.GetTexureForItem(itemID), slotRect, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(itemID)), Color.White * drawItemColorAlpha);
          if (crossOutZeroCountItems && item.Count == 0)
            this.spriteBatchPoint.Draw(this.crossOutTexture, slotRect, Color.White * drawItemColorAlpha);
          if (this.ShouldDrawQuantity(slotID))
            GraphicStatics.DrawItemData(this.spriteBatch, this.spriteBatchPoint, this.spriteBatchText, slotRect, item, this.ShowDurabilityBar(slotID, item), this.showItemCounts, 1f);
        }
      }
      this.DrawItemExtra(rectangle, slotID, item);
    }

    protected virtual bool IsDrawItemRedBackground(
      int slotID,
      InventoryItem item,
      BaseInventoryScreen.SkillCompare skillcompare)
    {
      Item itemId = item.ItemID;
      switch (skillcompare)
      {
        case BaseInventoryScreen.SkillCompare.Equip:
          if (this.inventoryOwner != null)
            return !this.inventoryOwner.CanUseItem(itemId);
          return false;
        case BaseInventoryScreen.SkillCompare.Craft:
          if (this.inventoryOwner != null)
            return !this.inventoryOwner.CanCraftItem(itemId);
          return false;
        default:
          return false;
      }
    }

    protected virtual bool ShouldDrawQuantity(int slotID)
    {
      return true;
    }

    protected virtual float GetDrawItemColorAlpha(int slotID, InventoryItem item)
    {
      return 1f;
    }

    protected virtual void DrawItemExtra(Rectangle slotRect, int slotID, InventoryItem item)
    {
    }

    protected virtual bool ShowDurabilityBar(int slotID, InventoryItem item)
    {
      return item.ShowDurabilityBar;
    }

    protected virtual Rectangle GetSlotRect(int slotID)
    {
      int x = slotID % 10 * 46 + this.screenRect.X + 16;
      int y = this.screenRect.Y + this.screenRect.Height - 36 - (slotID / 10 + 1) * 46 + 3;
      if (slotID > 9)
        y -= 16;
      return new Rectangle(x, y, 49, 49);
    }

    public virtual void BlueprintSelected(Blueprint blueprint)
    {
    }

    protected enum SkillCompare
    {
      None,
      Equip,
      Craft,
    }
  }
}
