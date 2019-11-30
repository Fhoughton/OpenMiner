// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.FurnaceScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class FurnaceScreen : BaseInventoryScreen
  {
    public const int ProductResultSlot = 34;
    public const int Ore1Slot = 31;
    public const int Ore2Slot = 32;
    public const int Ore3Slot = 33;
    public const int FuelSlot = 30;
    private FurnaceBlock furnace;
    private float missingItemsTimer;
    private Pulsator missingItemsPulsator;
    private bool missingItemsCatchup;

    protected override int CoWindowHeight
    {
      get
      {
        return 204;
      }
    }

    protected override Inventory CoWindowInventory
    {
      get
      {
        return this.furnace.Inventory;
      }
    }

    protected override bool IsLiftValid
    {
      get
      {
        if (this.currentSlotID == 30 && (double) this.furnace.TotalBurnTime(this.player) > 0.0 && this.CursorItemCount == 1)
          return false;
        return base.IsLiftValid;
      }
    }

    protected override bool IsUnliftValid
    {
      get
      {
        if (this.currentSlotID == 34)
          return false;
        if (this.currentSlotID != 30)
          return base.IsUnliftValid;
        if ((double) this.furnace.TotalBurnTime(this.player) != 0.0)
          return this.CursorItem.ItemID == this.liftedItem.ItemID;
        return true;
      }
    }

    protected override Inventory CursorInventory
    {
      get
      {
        if (this.currentSlotID <= 29)
          return base.CursorInventory;
        return this.furnace.Inventory;
      }
    }

    protected override InventoryItem CursorItem
    {
      get
      {
        if (this.currentSlotID <= 29)
          return this.player.Inventory[this.currentSlotID];
        return this.furnace.Inventory[this.currentSlotID - 30];
      }
      set
      {
        if (this.currentSlotID > 29)
          this.furnace.Inventory[this.currentSlotID - 30] = value;
        else
          this.player.Inventory[this.currentSlotID] = value;
      }
    }

    protected override int CursorItemCount
    {
      get
      {
        return this.CursorItem.Count;
      }
      set
      {
        if (this.currentSlotID > 29)
          this.furnace.Inventory.SetItemCount(this.currentSlotID - 30, value);
        else
          this.player.Inventory.SetItemCount(this.currentSlotID, value);
      }
    }

    protected InventoryItem ProductItem
    {
      get
      {
        return this.furnace.ProductItem;
      }
      set
      {
        this.furnace.ProductItem = value;
      }
    }

    protected override InventoryItem ItemToExamine
    {
      get
      {
        if (this.currentSlotID != 34 || this.furnace.Product == null)
          return base.ItemToExamine;
        return new InventoryItem(this.ProductItem.ItemID_Raw, 1);
      }
    }

    public FurnaceScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base(instance, player, (Inventory) player.Inventory, 0)
    {
      this.furnace = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.Furnace, UpdateBlockMethod.Player, this.PlayerID, true) as FurnaceBlock;
      this.furnace.ItemSmelted += new EventHandler(this.OnItemSmelted);
      this.missingItemsPulsator = new Pulsator();
      this.showItemCounts = true;
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      this.furnace.ItemSmelted -= new EventHandler(this.OnItemSmelted);
      this.instance.FlagBlockIsClosed(this.player.GamerID, true);
      this.player.Raise_FurnaceClosed();
    }

    protected override bool CanEditInventory
    {
      get
      {
        return this.player.HasPermission(Permissions.Adventure);
      }
    }

    private void OnItemSmelted(object sender, EventArgs e)
    {
      this.RefreshItemTypeName();
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EasyCraft))
      {
        this.ScreenManager.AddScreen((GameScreen) new CraftSelectBlueprintScreen((BaseInventoryScreen) this, BlueprintCraftType.Furnace), this.ControllingPlayer);
        return true;
      }
      if (!InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.TransferItem))
        return base.HandleInput(input);
      if (this.currentSlotID < (int) this.player.Inventory.PackSize)
      {
        if (Globals1.ItemData[(int) this.CursorItem.ItemID].BurnTime > (ushort) 0)
        {
          if (this.furnace.FuelItem.ItemID == Item.None)
          {
            InventoryItem cursorItem = this.CursorItem;
            this.CursorItem = this.furnace.FuelItem;
            this.furnace.FuelItem = cursorItem;
            this.furnace.Raise_FurnaceBurnStarted();
            if (!this.furnace.HasPlayer)
              this.furnace.Gamertag = this.player.Gamertag;
            this.instance.NetworkManager.SendDataBlockChange((DataBlock) this.furnace, false, UpdateBlockMethod.Player);
          }
          else if (this.furnace.FuelItem.ItemID == this.CursorItem.ItemID)
          {
            int num = Math.Min(this.CursorItemCount, ItemData.GetStackSize(this.furnace.FuelItem.ItemID) - this.furnace.FuelItemCount);
            if (num > 0)
            {
              InventoryItem fuelItem = this.furnace.FuelItem;
              fuelItem.Count += num;
              this.CursorItemCount -= num;
              this.furnace.FuelItem = fuelItem;
              if (!this.furnace.HasPlayer)
                this.furnace.Gamertag = this.player.Gamertag;
              this.instance.NetworkManager.SendDataBlockChange((DataBlock) this.furnace, false, UpdateBlockMethod.Player);
            }
          }
        }
        else if (this.furnace.Ore1ItemCount == 0)
        {
          InventoryItem cursorItem = this.CursorItem;
          this.CursorItem = this.furnace.Ore1Item;
          this.furnace.Ore1Item = cursorItem;
          this.SetProduct(this.player);
        }
        else if (this.furnace.Ore1Item.ItemID == this.CursorItem.ItemID)
        {
          int num = Math.Min(this.CursorItemCount, ItemData.GetStackSize(this.furnace.Ore1Item.ItemID) - this.furnace.Ore1ItemCount);
          if (num > 0)
          {
            InventoryItem ore1Item = this.furnace.Ore1Item;
            ore1Item.Count += num;
            this.CursorItemCount -= num;
            this.furnace.Ore1Item = ore1Item;
            this.SetProduct(this.player);
          }
        }
        else if (this.furnace.Ore2ItemCount == 0)
        {
          InventoryItem cursorItem = this.CursorItem;
          this.CursorItem = this.furnace.Ore2Item;
          this.furnace.Ore2Item = cursorItem;
          this.SetProduct(this.player);
        }
        else if (this.furnace.Ore2Item.ItemID == this.CursorItem.ItemID)
        {
          int num = Math.Min(this.CursorItemCount, ItemData.GetStackSize(this.furnace.Ore2Item.ItemID) - this.furnace.Ore2ItemCount);
          if (num > 0)
          {
            InventoryItem ore2Item = this.furnace.Ore2Item;
            ore2Item.Count += num;
            this.CursorItemCount -= num;
            this.furnace.Ore2Item = ore2Item;
            this.SetProduct(this.player);
          }
        }
        else if (this.furnace.Ore3ItemCount == 0)
        {
          InventoryItem cursorItem = this.CursorItem;
          this.CursorItem = this.furnace.Ore3Item;
          this.furnace.Ore3Item = cursorItem;
          this.SetProduct(this.player);
        }
        else if (this.furnace.Ore3Item.ItemID == this.CursorItem.ItemID)
        {
          int num = Math.Min(this.CursorItemCount, ItemData.GetStackSize(this.furnace.Ore3Item.ItemID) - this.furnace.Ore3ItemCount);
          if (num > 0)
          {
            InventoryItem ore3Item = this.furnace.Ore3Item;
            ore3Item.Count += num;
            this.CursorItemCount -= num;
            this.furnace.Ore3Item = ore3Item;
            this.SetProduct(this.player);
          }
        }
      }
      else if (this.currentSlotID != 30)
      {
        this.TransferItems();
        this.SetProduct(this.player);
      }
      else
      {
        InventoryItem fuelItem = this.furnace.FuelItem;
        if (fuelItem.Count > 1)
        {
          --fuelItem.Count;
          int num = this.inventory.TransferTo(fuelItem);
          if (num > 0)
          {
            this.furnace.FuelItemCount -= num;
            Sounds.PlaySound(ItemSoundGroup.GuiTransfer);
            this.instance.NetworkManager.SendDataBlockChange((DataBlock) this.furnace, false, UpdateBlockMethod.Player);
          }
          else
            Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
        }
      }
      return true;
    }

    protected override void OnLiftAllButtonPressedCore()
    {
      this.OnLiftAnyButtonPressed();
    }

    protected override void OnLiftSingleButtonPressedCore()
    {
      this.OnLiftAnyButtonPressed();
    }

    private void OnLiftAnyButtonPressed()
    {
      if (this.currentSlotID == 31 || this.currentSlotID == 32 || this.currentSlotID == 33)
      {
        this.SetProduct(this.player);
      }
      else
      {
        if (this.currentSlotID != 30)
          return;
        if (this.furnace.HasFuel)
          this.furnace.Raise_FurnaceBurnStarted();
        this.instance.NetworkManager.SendDataBlockChange((DataBlock) this.furnace, false, UpdateBlockMethod.Player);
      }
    }

    protected override void LiftItem()
    {
      InventoryItem cursorItem = this.CursorItem;
      if (cursorItem.ItemID == Item.None)
        return;
      if (this.currentSlotID == 30 && (double) this.furnace.TotalBurnTime(this.player) > 0.0)
      {
        if (cursorItem.Count <= 1)
          return;
        --cursorItem.Count;
        this.liftedItem = cursorItem;
        this.CursorItemCount = 1;
        this.isItemLifted = true;
      }
      else
      {
        this.isItemLifted = true;
        this.liftedItem = cursorItem;
        this.CursorItemCount = 0;
        if (this.currentSlotID != 34)
          return;
        this.SetProduct(this.player);
      }
    }

    protected override void MoveLeftCore()
    {
      if (this.currentSlotID == 30)
        this.currentSlotID = 34;
      else
        --this.currentSlotID;
    }

    protected override void MoveRightCore()
    {
      if (this.currentSlotID >= 34)
        this.currentSlotID = 30;
      else
        ++this.currentSlotID;
    }

    protected override void MoveUpCore()
    {
      if (this.currentSlotID > 29)
      {
        if (this.currentSlotID == 34)
          this.currentSlotID = 4;
        else if (this.currentSlotID == 31)
          this.currentSlotID = 0;
        else if (this.currentSlotID == 32)
          this.currentSlotID = 1;
        else if (this.currentSlotID == 33)
          this.currentSlotID = 2;
        else if (this.currentSlotID == 30)
          this.currentSlotID = 31;
        else
          this.currentSlotID -= 20;
      }
      else
      {
        this.currentSlotID += 10;
        if (this.currentSlotID == 31)
          this.currentSlotID = 32;
        else if (this.currentSlotID == 32)
          this.currentSlotID = 33;
        else if (this.currentSlotID == 34)
        {
          this.currentSlotID = 34;
        }
        else
        {
          if (this.currentSlotID == 30)
            return;
          this.currentSlotID -= 30;
        }
      }
    }

    protected override void MoveDownCore()
    {
      if (this.currentSlotID == 31)
        this.currentSlotID = 30;
      else if (this.currentSlotID == 32)
        this.currentSlotID = 21;
      else if (this.currentSlotID == 33)
        this.currentSlotID = 22;
      else if (this.currentSlotID == 30)
        this.currentSlotID = 20;
      else if (this.currentSlotID == 34)
        this.currentSlotID = 24;
      else if (this.currentSlotID == 0)
        this.currentSlotID = 31;
      else if (this.currentSlotID == 1)
        this.currentSlotID = 32;
      else if (this.currentSlotID == 2)
      {
        this.currentSlotID = 33;
      }
      else
      {
        this.currentSlotID -= 10;
        if (this.currentSlotID >= 0)
          return;
        if (this.currentSlotID == -10)
          this.currentSlotID = 31;
        else if (this.currentSlotID == -9)
          this.currentSlotID = 32;
        else if (this.currentSlotID == -8)
          this.currentSlotID = 33;
        else if (this.currentSlotID == -6)
          this.currentSlotID = 34;
        else
          this.currentSlotID += 30;
      }
    }

    protected override bool SetCurrentSlotCoWindow(Point pos)
    {
      int slotID1 = 30;
      if (this.GetSlotRect(slotID1).Contains(pos))
      {
        this.currentSlotID = slotID1;
        return true;
      }
      int slotID2 = 31;
      if (this.GetSlotRect(slotID2).Contains(pos))
      {
        this.currentSlotID = slotID2;
        return true;
      }
      int slotID3 = 32;
      if (this.GetSlotRect(slotID3).Contains(pos))
      {
        this.currentSlotID = slotID3;
        return true;
      }
      int slotID4 = 33;
      if (this.GetSlotRect(slotID4).Contains(pos))
      {
        this.currentSlotID = slotID4;
        return true;
      }
      int slotID5 = 34;
      if (!this.GetSlotRect(slotID5).Contains(pos))
        return false;
      this.currentSlotID = slotID5;
      return true;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      this.CheckForClear();
    }

    protected virtual void CheckForClear()
    {
      switch ((Block) this.instance.Map.GetBlockID(this.furnace.Point))
      {
        case Block.Furnace:
          break;
        case Block.LitFurnace:
          break;
        default:
          this.ExitScreen();
          break;
      }
    }

    protected override void DrawCoWindow()
    {
      Rectangle rectangle1 = new Rectangle(0, 0, 49, 49);
      Rectangle rectangle2 = new Rectangle(0, 0, 32, 32);
      int slotID1 = 30;
      Rectangle slotRect1 = this.GetSlotRect(slotID1);
      this.spriteBatch.DrawString(this.Font, "Fuel", new Vector2((float) (slotRect1.X + slotRect1.Width + 12), (float) (slotRect1.Y + 16)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.DrawSlot(slotRect1.X, slotRect1.Y);
      InventoryItem inventoryItem = !this.IsItemLifted || slotID1 != this.currentSlotID ? this.furnace.FuelItem : this.liftedItem;
      if (inventoryItem.ItemID != Item.None && inventoryItem.Count > 0)
        this.DrawItem(slotRect1, slotID1, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
      int slotID2 = 31;
      Rectangle slotRect2 = this.GetSlotRect(slotID2);
      this.spriteBatch.DrawString(this.Font, "Materials", new Vector2((float) slotRect2.X, (float) (slotRect2.Y - 28)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.DrawSlot(slotRect2.X, slotRect2.Y);
      inventoryItem = !this.IsItemLifted || slotID2 != this.currentSlotID ? this.furnace.Ore1Item : this.liftedItem;
      if (inventoryItem.ItemID != Item.None && inventoryItem.Count > 0)
        this.DrawItem(slotRect2, slotID2, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
      int slotID3 = 32;
      slotRect2 = this.GetSlotRect(slotID3);
      this.DrawSlot(slotRect2.X, slotRect2.Y);
      inventoryItem = !this.IsItemLifted || slotID3 != this.currentSlotID ? this.furnace.Ore2Item : this.liftedItem;
      if (inventoryItem.ItemID != Item.None && inventoryItem.Count > 0)
        this.DrawItem(slotRect2, slotID3, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
      int slotID4 = 33;
      slotRect2 = this.GetSlotRect(slotID4);
      this.DrawSlot(slotRect2.X, slotRect2.Y);
      inventoryItem = !this.IsItemLifted || slotID4 != this.currentSlotID ? this.furnace.Ore3Item : this.liftedItem;
      if (inventoryItem.ItemID != Item.None && inventoryItem.Count > 0)
        this.DrawItem(slotRect2, slotID4, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
      if ((double) this.missingItemsTimer > 0.0)
      {
        this.missingItemsTimer -= Services.ElapsedTime;
        this.missingItemsPulsator.Update();
        Color color = Color.Orange * this.missingItemsPulsator.Value;
        this.spriteBatch.DrawString(this.Font, " - Some Missing", new Vector2((float) (slotRect2.X + 49 + 12), (float) (slotRect2.Y - 27)) + TMFont.yVec, color, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      slotRect2 = this.GetSlotRect(31);
      slotRect2.X -= 2;
      slotRect2.Y += 58;
      slotRect2.Height /= 2;
      slotRect2.Width = 146;
      this.spriteBatch.DrawBox(slotRect2, 4, Color.Black, 0.0f);
      if ((double) this.furnace.CurrentSmeltTime > 0.0 || (double) this.furnace.CurrentBurnTime > 0.0)
      {
        slotRect2.X += 4;
        slotRect2.Y += 4;
        slotRect2.Height -= 8;
        slotRect2.Height /= 2;
        int num = 139;
        if ((double) this.furnace.CurrentSmeltTime > 0.0)
        {
          slotRect2.Width = (int) ((double) this.furnace.SmeltCompleteNormalized(this.instance) * (double) num);
          this.spriteBatch.Draw(CoreGlobals.BlankTexture, slotRect2, Color.Green);
        }
        slotRect2.Y += slotRect2.Height;
        if ((double) this.furnace.CurrentBurnTime > 0.0)
        {
          Rectangle blockSrcRect = GraphicStatics.TexturePack.BlockSrcRects[13];
          slotRect2.Width = num - (int) ((double) this.furnace.BurnCompleteNormalized(this.instance) * (double) num);
          blockSrcRect.Width = (int) ((double) blockSrcRect.Width * ((double) slotRect2.Width / (double) num));
          this.spriteBatch.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect2, new Rectangle?(blockSrcRect), Color.White);
        }
      }
      int slotID5 = 34;
      slotRect2 = this.GetSlotRect(slotID5);
      this.spriteBatch.DrawString(this.Font, "Product", new Vector2((float) (slotRect2.X + slotRect2.Width + 12), (float) (slotRect2.Y + 16)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.DrawSlot(slotRect2.X, slotRect2.Y);
      if (this.ProductItem.ItemID_Raw != Item.None)
        this.DrawItem(slotRect2, slotID5, this.furnace.ProductItem, false, BaseInventoryScreen.SkillCompare.Craft);
      if (this.currentSlotID == 34 && this.IsItemLifted)
        this.DrawItem(slotRect2, this.currentSlotID, this.liftedItem, false, BaseInventoryScreen.SkillCompare.Equip);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 15;
      screenRect.Y += this.CoWindowHeight - 18;
      screenRect.Width = 25;
      screenRect.Height = 25;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.EasyCraft, screenRect);
      this.spriteBatch.DrawString(this.Font, "Easy Smelt", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X += 194;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.TransferItem, screenRect);
      this.spriteBatch.DrawString(this.Font, "Transfer", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X += 182;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, screenRect);
      this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override Rectangle GetSlotRect(int slotID)
    {
      Rectangle slotRect = base.GetSlotRect(slotID);
      if (slotID > 29)
      {
        if (slotID == 34)
        {
          slotRect.X = 184 + this.screenRect.X + 15;
          slotRect.Y = (int) ((double) (this.screenRect.Y + this.screenRect.Height - 68) - 69.0 - 196.0);
        }
        else
        {
          slotRect.X = this.screenRect.X + 12;
          if (slotID == 32)
            slotRect.X += 46;
          else if (slotID == 33)
            slotRect.X += 92;
          float num = slotID == 30 ? 0.05f : 2f;
          slotRect.Y = (int) ((double) (this.screenRect.Y + this.screenRect.Height - 80) - (double) num * 46.0 - 196.0);
        }
      }
      return slotRect;
    }

    protected override bool IsDrawItemRedBackground(
      int slotID,
      InventoryItem item,
      BaseInventoryScreen.SkillCompare skillcompare)
    {
      if (slotID == 34)
        item.Count = 1;
      return base.IsDrawItemRedBackground(slotID, item, skillcompare);
    }

    public override void BlueprintSelected(Blueprint blueprint)
    {
      this.PutCraftItemsBackIntoInventory();
      this.missingItemsTimer = 0.0f;
      for (int slotID = 0; slotID < blueprint.Items.Length && slotID < 3; ++slotID)
      {
        InventoryItem inventoryItem1 = blueprint.Items[slotID];
        if (inventoryItem1.ItemID != Item.None)
        {
          int num1 = this.player.Inventory.ItemCount(inventoryItem1.ItemID);
          if (ItemData.GetItemDurability(inventoryItem1.ItemID) > (ushort) 0)
            num1 = 1;
          int itemSlotCount = blueprint.GetItemSlotCount(inventoryItem1.ItemID, slotID);
          int itemTotalCount = blueprint.GetItemTotalCount(inventoryItem1.ItemID);
          int num2 = num1 / itemTotalCount;
          int val2 = (int) ((double) num1 * ((double) itemSlotCount / (double) itemTotalCount));
          int count = Math.Min(ItemData.GetStackSize(inventoryItem1.ItemID), val2);
          if (count == 0)
          {
            this.missingItemsTimer = 8f;
            this.missingItemsPulsator.Start(1f, 0.1f, 0.75f);
            break;
          }
          if (count > 1)
          {
            this.furnace.Inventory[slotID + 1] = new InventoryItem(inventoryItem1.ItemID, count);
          }
          else
          {
            int index = this.furnace.Inventory.FindItem(inventoryItem1.ItemID);
            if (index >= 0)
            {
              InventoryItem inventoryItem2 = new InventoryItem(inventoryItem1.ItemID, 1, this.furnace.Inventory[index].Durability);
              this.furnace.Inventory[slotID + 1] = inventoryItem2;
            }
          }
        }
      }
      for (int index = 0; index < blueprint.Items.Length && index < 3; ++index)
      {
        InventoryItem inventoryItem = this.furnace.Inventory[index + 1];
        if (inventoryItem.ItemID != Item.None)
        {
          if ((double) this.missingItemsTimer > 0.0)
            this.furnace.Inventory[index + 1] = InventoryItem.Empty;
          else
            this.player.Inventory.DecrementItem(inventoryItem.ItemID, inventoryItem.Count);
        }
      }
      this.SetProduct(this.player);
      this.currentSlotID = 34;
      this.missingItemsCatchup = (double) this.missingItemsTimer > 0.0;
    }

    private void SetProduct(Player player)
    {
      this.SetProduct(player?.Gamertag);
    }

    private void SetProduct(string gamertag)
    {
      Blueprint product = this.furnace.Product;
      Player player = this.instance.GetPlayer(gamertag);
      this.furnace.Product = Blueprints.GetSmeltResult(player, this.furnace.Ore1Item, this.furnace.Ore2Item, this.furnace.Ore3Item);
      if (this.furnace.Product != null)
        this.furnace.Gamertag = gamertag;
      if (product != this.furnace.Product)
      {
        this.furnace.ResetSmeltTime(player, 0.0f);
        if (this.furnace.ProductItemCount == 0)
        {
          if (this.furnace.Product == null)
          {
            this.furnace.ProductItem = InventoryItem.Empty;
          }
          else
          {
            this.furnace.ProductItem = new InventoryItem(this.furnace.Product.Result.ItemID, 1);
            this.furnace.ProductItem = new InventoryItem(this.furnace.Product.Result.ItemID, 0);
          }
        }
      }
      this.instance.NetworkManager.SendDataBlockChange((DataBlock) this.furnace, false, UpdateBlockMethod.Player);
    }

    private void MissingItemCatchup(object sender, EventArgs e)
    {
      if (!this.missingItemsCatchup)
        return;
      this.missingItemsTimer = 8f;
      this.missingItemsPulsator.Start(1f, 0.1f, 0.75f);
    }

    protected void PutCraftItemsBackIntoInventory()
    {
      for (int index = 0; index < (int) this.furnace.Inventory.PackSize; ++index)
      {
        if (index != 0)
        {
          InventoryItem transferItem = this.furnace.Inventory[index];
          if (transferItem.Count > 0)
          {
            int num = this.player.Inventory.TransferTo(transferItem);
            transferItem.Count -= num;
            if (transferItem.Count > 0)
            {
              this.instance.DropItem(ParticleType.None, this.furnace.Point, transferItem, Vector2.Zero, 3f, UpdateBlockMethod.DropTimeLong, GamerID.Sys1);
              transferItem.Count = 0;
            }
            this.furnace.Inventory[index] = transferItem;
          }
        }
      }
    }
  }
}
