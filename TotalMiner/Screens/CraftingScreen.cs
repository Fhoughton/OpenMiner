// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CraftingScreen
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
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class CraftingScreen : BaseInventoryScreen
  {
    public int CraftResultSlot;
    private float missingItemsTimer;
    private Pulsator missingItemsPulsator;
    protected int gridsize;
    protected int gridStartSlotID;
    protected InventoryItem craftResult;
    protected Blueprint craftBlueprint;
    private bool missingItemsCatchup;

    protected override int CoWindowHeight
    {
      get
      {
        return 200 - (this.gridsize == 2 ? 46 : 0);
      }
    }

    protected override bool AllowRapidLiftSinglePressA
    {
      get
      {
        return this.currentSlotID == this.CraftResultSlot;
      }
    }

    public CraftingScreen(GameInstance instance, Player player)
      : base(instance, player, (Actor) player, 0)
    {
      this.gridsize = 2;
      this.missingItemsPulsator = new Pulsator();
      this.gridStartSlotID = (int) player.Inventory.TempIndexStart;
      this.CraftResultSlot = this.gridStartSlotID + 9;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.screenRect.Y = 548 - this.screenRect.Height;
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      this.PutCraftItemsBackIntoInventory();
      this.player.Raise_InventoryClosed();
    }

    protected void PutCraftItemsBackIntoInventory()
    {
      this.player.Inventory.SetItemCount(this.CraftResultSlot, 0);
      for (int tempIndexStart = (int) this.player.Inventory.TempIndexStart; tempIndexStart < (int) this.player.Inventory.TempIndexEnd; ++tempIndexStart)
      {
        InventoryItem transferItem = this.player.Inventory[tempIndexStart];
        if (transferItem.Count > 0)
        {
          int num = this.player.Inventory.TransferTo(transferItem);
          transferItem.Count -= num;
          this.player.Inventory[tempIndexStart] = transferItem;
          if (transferItem.Count > 0)
            this.player.DropItem(ParticleType.None, tempIndexStart, UpdateBlockMethod.DropTimeShort);
        }
      }
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EasyCraft))
      {
        this.ScreenManager.AddScreen((GameScreen) new CraftSelectBlueprintScreen((BaseInventoryScreen) this, BlueprintCraftType.Crafting), this.ControllingPlayer);
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.TransferItem))
      {
        if (this.currentSlotID == this.CraftResultSlot || this.IsItemLifted)
        {
          if (this.IsItemLifted)
          {
            InventoryItem liftedItem = this.liftedItem;
            int num = this.inventory.TransferTo(this.liftedItem);
            if (num > 0)
            {
              this.liftedItem.Count -= num;
              if (this.liftedItem.Count == 0)
                this.isItemLifted = false;
              Sounds.PlaySound(ItemSoundGroup.GuiAccept);
            }
            else
              Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
          }
          return true;
        }
        if (this.currentSlotID > 29)
        {
          this.TransferItems();
          this.SetCraftResult();
          return true;
        }
      }
      if (!InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EquipItemLeft) && !InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.EquipItemRight))
        return base.HandleInput(input);
      this.ScreenManager.AddScreen((GameScreen) new InventoryScreen(this.instance, this.player, this.currentSlotID), this.ControllingPlayer);
      this.ExitScreen();
      return true;
    }

    protected override InventoryItem CursorItem
    {
      get
      {
        if (this.currentSlotID != this.CraftResultSlot)
          return base.CursorItem;
        return this.craftResult;
      }
    }

    protected override InventoryItem ItemToExamine
    {
      get
      {
        if (this.currentSlotID != this.CraftResultSlot)
          return base.ItemToExamine;
        return this.craftResult;
      }
    }

    protected override bool IsUnliftValid
    {
      get
      {
        if (this.currentSlotID == this.CraftResultSlot)
          return false;
        return base.IsUnliftValid;
      }
    }

    protected override bool IsLiftSingleValid
    {
      get
      {
        return this.currentSlotID != this.CraftResultSlot;
      }
    }

    protected override bool IsLiftAllValid
    {
      get
      {
        if (this.currentSlotID != this.CraftResultSlot)
          return true;
        if (!this.player.CanCraftItem(this.craftResult.ItemID))
          return false;
        int count = this.craftResult.Count;
        if (this.IsItemLifted)
          count += this.liftedItem.Count;
        return count <= ItemData.GetStackSize(this.craftResult.ItemID);
      }
    }

    protected override void LiftItem()
    {
      InventoryItem inventoryItem = this.currentSlotID == this.CraftResultSlot ? this.craftResult : this.CursorItem;
      if (inventoryItem.ItemID == Item.None)
        return;
      if (this.currentSlotID == this.CraftResultSlot && this.IsItemLifted)
      {
        this.liftedItem.Count += inventoryItem.Count;
      }
      else
      {
        this.isItemLifted = true;
        this.liftedItem = inventoryItem;
      }
      if (this.currentSlotID == this.CraftResultSlot)
      {
        this.ReduceCraftItems();
        this.player.SkillsData.ItemCrafted(this.player, inventoryItem.ItemID);
        this.player.ActionLog.AddAction(inventoryItem.ItemID, ItemAction.Crafted);
        NetworkManager.Instance.SendActionLog(this.player.GamerID, inventoryItem.ItemID, ItemAction.Crafted);
        this.player.Raise_ItemCrafted(inventoryItem.ItemID);
      }
      else
        this.CursorItemCount = 0;
    }

    protected override void UnliftItem()
    {
      if (this.currentSlotID != this.CraftResultSlot)
      {
        base.UnliftItem();
      }
      else
      {
        InventoryItem craftResult = this.craftResult;
        if (craftResult.ItemID != this.liftedItem.ItemID || craftResult.MaxDurability != (ushort) 0)
          return;
        this.liftedItem.Count += this.craftResult.Count;
        this.ReduceCraftItems();
        this.player.SkillsData.ItemCrafted(this.player, craftResult.ItemID);
        this.player.ActionLog.AddAction(craftResult.ItemID, ItemAction.Crafted);
        NetworkManager.Instance.SendActionLog(this.player.GamerID, craftResult.ItemID, ItemAction.Crafted);
        this.player.Raise_ItemCrafted(this.liftedItem.ItemID);
      }
    }

    protected override void OnLiftAllButtonPressedCore()
    {
      this.SetCraftResult();
    }

    protected override void OnLiftSingleButtonPressedCore()
    {
      this.SetCraftResult();
    }

    private void SetCraftResult()
    {
      this.craftResult = InventoryItem.Empty;
      this.craftBlueprint = this.player.GetCraftBlueprint();
      if (this.craftBlueprint != null)
      {
        this.craftResult = this.craftBlueprint.Result;
        this.craftResult.Durability = ItemData.GetItemDurability(this.craftResult.ItemID);
      }
      this.player.Inventory[this.CraftResultSlot] = this.craftResult;
    }

    protected override void MoveLeftCore()
    {
      if (this.currentSlotID == this.CraftResultSlot)
        this.currentSlotID = this.gridStartSlotID + 2 + this.gridsize;
      else if (this.currentSlotID == this.gridStartSlotID + 3)
        this.currentSlotID = this.CraftResultSlot;
      else if (this.currentSlotID == this.gridStartSlotID || this.currentSlotID == this.gridStartSlotID + 6)
        this.currentSlotID += this.gridsize - 1;
      else
        --this.currentSlotID;
    }

    protected override void MoveRightCore()
    {
      if (this.currentSlotID == this.CraftResultSlot)
        this.currentSlotID = this.gridStartSlotID + 3;
      else if (this.currentSlotID == this.gridStartSlotID + this.gridsize + 2)
        this.currentSlotID = this.CraftResultSlot;
      else if (this.currentSlotID == this.gridStartSlotID + this.gridsize - 1)
        this.currentSlotID = this.CraftResultSlot;
      else if (this.currentSlotID == this.gridStartSlotID + this.gridsize + 5)
        this.currentSlotID = this.CraftResultSlot;
      else
        ++this.currentSlotID;
    }

    protected override void MoveUpCore()
    {
      if (this.currentSlotID >= this.gridStartSlotID)
      {
        if (this.currentSlotID == this.CraftResultSlot)
        {
          this.currentSlotID = this.gridsize == 2 ? 3 : 4;
        }
        else
        {
          this.currentSlotID += 3;
          if (this.currentSlotID >= this.CraftResultSlot)
          {
            this.currentSlotID = this.gridsize == 2 ? 3 : this.currentSlotID - (this.gridStartSlotID + 9);
          }
          else
          {
            if (this.gridsize != 2 || this.currentSlotID <= this.gridStartSlotID + 5)
              return;
            this.currentSlotID -= this.gridStartSlotID + 6;
          }
        }
      }
      else
      {
        this.currentSlotID += 10;
        if (this.currentSlotID > 34)
        {
          this.currentSlotID -= 30;
        }
        else
        {
          if (this.currentSlotID > 29)
            this.currentSlotID = this.currentSlotID - 30 + this.gridStartSlotID;
          if (this.gridsize == 2 && this.currentSlotID == this.gridStartSlotID + 3)
            this.currentSlotID = this.CraftResultSlot;
          else if (this.gridsize == 3 && this.currentSlotID == this.gridStartSlotID + 4)
          {
            this.currentSlotID = this.CraftResultSlot;
          }
          else
          {
            if (this.currentSlotID < this.gridStartSlotID + this.gridsize || this.currentSlotID == this.CraftResultSlot)
              return;
            this.currentSlotID -= this.gridStartSlotID;
          }
        }
      }
    }

    protected override void MoveDownCore()
    {
      if (this.currentSlotID > this.gridStartSlotID + 2)
      {
        if (this.currentSlotID == this.CraftResultSlot)
          this.currentSlotID = this.gridsize == 2 ? 23 : 24;
        else
          this.currentSlotID -= 3;
      }
      else if (this.currentSlotID >= this.gridStartSlotID)
      {
        this.currentSlotID -= this.gridStartSlotID - 20;
      }
      else
      {
        this.currentSlotID -= 10;
        if (this.currentSlotID >= 0)
          return;
        if (this.gridsize == 2)
        {
          if (this.currentSlotID == -8)
            this.currentSlotID = 22;
          else if (this.currentSlotID == -7)
            this.currentSlotID = this.CraftResultSlot;
          else if (this.currentSlotID > -7)
            this.currentSlotID += 30;
          else
            this.currentSlotID += this.gridStartSlotID + 13;
        }
        else if (this.currentSlotID == -7)
          this.currentSlotID = 23;
        else if (this.currentSlotID == -6)
          this.currentSlotID = this.CraftResultSlot;
        else if (this.currentSlotID > -6)
          this.currentSlotID += 30;
        else
          this.currentSlotID += this.gridStartSlotID + 16;
      }
    }

    private void ReduceCraftItems()
    {
      this.craftBlueprint.ReduceCraftItems((Inventory) this.player.Inventory, this.gridStartSlotID);
    }

    protected override bool SetCurrentSlotCoWindow(Point pos)
    {
      int craftResultSlot = this.CraftResultSlot;
      if (this.GetSlotRect(craftResultSlot).Contains(pos))
      {
        this.currentSlotID = craftResultSlot;
        return true;
      }
      for (int index1 = this.gridsize - 1; index1 >= 0; --index1)
      {
        for (int index2 = 0; index2 < this.gridsize; ++index2)
        {
          int slotID = index2 + index1 * 3 + this.gridStartSlotID;
          if (this.GetSlotRect(slotID).Contains(pos))
          {
            this.currentSlotID = slotID;
            return true;
          }
        }
      }
      return false;
    }

    protected override void DrawCoWindow()
    {
      Rectangle slotRect = new Rectangle(0, 0, 49, 49);
      for (int index1 = this.gridsize - 1; index1 >= 0; --index1)
      {
        for (int index2 = 0; index2 < this.gridsize; ++index2)
        {
          int slotID = index2 + index1 * 3 + this.gridStartSlotID;
          slotRect = this.GetSlotRect(slotID);
          this.DrawSlot(slotRect.X, slotRect.Y);
          InventoryItem inventoryItem = !this.IsItemLifted || slotID != this.currentSlotID ? this.player.Inventory[slotID] : this.liftedItem;
          if (inventoryItem.ItemID != Item.None)
            this.DrawItem(slotRect, slotID, inventoryItem, false, BaseInventoryScreen.SkillCompare.Equip);
          if (index1 == this.gridsize - 1 && index2 == 0)
          {
            this.spriteBatch.DrawString(this.Font, "Materials", new Vector2((float) slotRect.X, (float) (slotRect.Y - 27)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
            if ((double) this.missingItemsTimer > 0.0)
            {
              this.missingItemsTimer -= Services.ElapsedTime;
              this.missingItemsPulsator.Update();
              Color color = Color.Orange * this.missingItemsPulsator.Value;
              this.spriteBatch.DrawString(this.Font, " - Some Missing", new Vector2((float) (slotRect.X + 96), (float) (slotRect.Y - 27)) + TMFont.yVec, color, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
            }
          }
        }
      }
      slotRect.X = 141 + this.screenRect.X + 16;
      slotRect.Y = (int) ((double) (this.screenRect.Y + this.screenRect.Height - 38) - 69.0 - 196.0);
      if (this.gridsize == 3)
        slotRect.X += 49;
      slotRect = this.GetSlotRect(this.CraftResultSlot);
      this.DrawSlot(slotRect.X, slotRect.Y);
      this.spriteBatch.DrawString(this.Font, "Product", new Vector2((float) (slotRect.X + slotRect.Width + 12), (float) (slotRect.Y + 16)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      bool flag = false;
      if (this.currentSlotID == this.CraftResultSlot && this.IsItemLifted)
      {
        this.DrawItem(slotRect, this.currentSlotID, this.liftedItem, false, BaseInventoryScreen.SkillCompare.Equip);
        flag = true;
      }
      if (this.craftResult.ItemID != Item.None && !flag)
        this.DrawItem(slotRect, this.CraftResultSlot, this.craftResult, false, BaseInventoryScreen.SkillCompare.Craft);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 16;
      screenRect.Y += this.CoWindowHeight - 18;
      screenRect.Width = 25;
      screenRect.Height = 25;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.EasyCraft, screenRect);
      this.spriteBatch.DrawString(this.Font, "Easy Craft", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X += 196;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.TransferItem, screenRect);
      this.spriteBatch.DrawString(this.Font, this.currentSlotID > 29 || this.IsItemLifted ? "Transfer" : "Equip", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X += 178;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, screenRect);
      this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (screenRect.X + 33), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override Rectangle GetSlotRect(int slotID)
    {
      Rectangle slotRect = base.GetSlotRect(slotID);
      if (slotID >= this.gridStartSlotID)
      {
        if (slotID == this.CraftResultSlot)
        {
          slotRect.X = 141 + this.screenRect.X + 16;
          slotRect.Y = (int) ((double) (this.screenRect.Y + this.screenRect.Height - 56) - 69.0 - 196.0);
          if (this.gridsize == 3)
            slotRect.X += 49;
          else
            slotRect.Y += 24;
        }
        else
        {
          slotRect.X = (slotID - this.gridStartSlotID) % 3 * 46 + 3 + this.screenRect.X + 16;
          slotRect.Y = this.screenRect.Y + this.screenRect.Height - 58 - ((slotID - this.gridStartSlotID) / 3 + 1) * 46 - 196 + 24;
        }
      }
      return slotRect;
    }

    public override void BlueprintSelected(Blueprint blueprint)
    {
      this.PutCraftItemsBackIntoInventory();
      bool flag = false;
      this.missingItemsTimer = 0.0f;
      for (int index1 = 0; index1 < blueprint.Items.Length; ++index1)
      {
        InventoryItem inventoryItem1 = blueprint.Items[index1];
        if (inventoryItem1.ItemID != Item.None)
        {
          if (this.gridsize == 3 || index1 == 0 || (index1 == 1 || index1 == 3) || index1 == 4)
          {
            int num = this.player.Inventory.ItemCount(inventoryItem1.ItemID);
            if (ItemData.GetItemDurability(inventoryItem1.ItemID) > (ushort) 0)
              num = 1;
            int itemTotalCount = blueprint.GetItemTotalCount(inventoryItem1.ItemID);
            int count = Math.Min(ItemData.GetStackSize(inventoryItem1.ItemID), (int) ((double) num * ((double) inventoryItem1.Count / (double) itemTotalCount)));
            if (count == 0)
            {
              this.missingItemsTimer = 8f;
              this.missingItemsPulsator.Start(1f, 0.1f, 0.75f);
              break;
            }
            if (count > 1)
            {
              this.player.Inventory[index1 + (int) this.player.Inventory.TempIndexStart] = new InventoryItem(inventoryItem1.ItemID, count);
            }
            else
            {
              int index2 = this.player.Inventory.FindItem(inventoryItem1.ItemID);
              if (index2 >= 0)
              {
                InventoryItem inventoryItem2 = new InventoryItem(inventoryItem1.ItemID, 1, this.player.Inventory[index2].Durability);
                this.player.Inventory[index1 + (int) this.player.Inventory.TempIndexStart] = inventoryItem2;
              }
            }
          }
          else
          {
            flag = true;
            break;
          }
        }
      }
      for (int index = 0; index < blueprint.Items.Length; ++index)
      {
        InventoryItem inventoryItem = this.player.Inventory[index + (int) this.player.Inventory.TempIndexStart];
        if (inventoryItem.ItemID != Item.None)
        {
          if (flag || (double) this.missingItemsTimer > 0.0)
            this.player.Inventory[index + (int) this.player.Inventory.TempIndexStart] = InventoryItem.Empty;
          else
            this.player.Inventory.DecrementItem(inventoryItem.ItemID, inventoryItem.Count);
        }
      }
      this.SetCraftResult();
      this.currentSlotID = this.CraftResultSlot;
      this.missingItemsCatchup = (double) this.missingItemsTimer > 0.0;
      if (flag)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You have selected a blueprint that requires a workbench to craft.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        if (this.player.SaveState.CraftInstructionMessageShown)
          return;
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Any items you have in your inventory that are needed for \r\nthis blueprint are now populated into the crafting grid.\r\n\r\nThe yellow cursor is now positioned over the Product box. \r\nSimply press A to craft the item. The cursor will turn red \r\nwhen you have crafted the item. Keep pressing A to craft \r\nmore. A number will show how many copies you have crafted \r\n(if more than one). When you have crafted enough, move the \r\nitems to a free slot in your inventory.", "Very good", "Don't show me this message again", (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.MissingItemCatchup);
        messageBoxScreenTm.ButtonB += new EventHandler<PlayerIndexEventArgs>(this.MissingItemCatchup);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.DontShowInstructionAgain);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
    }

    private void DontShowInstructionAgain(object sender, EventArgs e)
    {
      this.player.SaveState.CraftInstructionMessageShown = true;
      this.MissingItemCatchup(sender, e);
    }

    private void MissingItemCatchup(object sender, EventArgs e)
    {
      if (!this.missingItemsCatchup)
        return;
      this.missingItemsTimer = 8f;
      this.missingItemsPulsator.Start(1f, 0.1f, 0.75f);
    }
  }
}
