// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ItemOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ItemOptionsScreen : ShopScreen
  {
    private bool atLeastOneItemHasBeenDisabled;

    protected override bool AllowRapidLiftSinglePressX
    {
      get
      {
        return false;
      }
    }

    private bool IsCursorItemValid
    {
      get
      {
        if (this.currentSlotID >= 30)
          return ItemData.IsEnabled(this.CursorItem.ItemID_Raw);
        return true;
      }
    }

    public ItemOptionsScreen(GameInstance instance, Player player, Block shopType)
      : base(instance, player, shopType)
    {
      if (player == null)
        return;
      player.OverrideIsEnabledInShop = true;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.showItemCounts = false;
    }

    protected override void OnScreenClosed()
    {
      if (!this.atLeastOneItemHasBeenDisabled)
        return;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Warning: Disabling blocks or items removes them from the game save. So if you disable an item or block, and save and exit, then when you reload the world, any instances of those disabled items or blocks will no longer exist in the world (including inventories, chests and their contents, etc).", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    protected override void AddItemsToShopInventory(Inventory inventory)
    {
      ShopScreen.AddToInventory(inventory, Item.BlockShop, this.player);
      ShopScreen.AddToInventory(inventory, Item.ItemShop, this.player);
      ShopScreen.AddToInventory(inventory, Item.Workbench, this.player);
      ShopScreen.AddToInventory(inventory, Item.Furnace, this.player);
      ShopScreen.AddToInventory(inventory, Item.LockedChest, this.player);
      ShopScreen.AddToInventory(inventory, Item.Safe, this.player);
      ShopScreen.AddToInventory(inventory, Item.LockedDoor, this.player);
      ShopScreen.AddToInventory(inventory, Item.Bed, this.player);
      ShopScreen.AddToInventory(inventory, Item.RopeIcon, this.player);
      ShopScreen.AddToInventory(inventory, Item.InvisibleBarrier, this.player);
      ShopScreen.AddToInventory(inventory, Item.NPCSpawn, this.player);
      ShopScreen.AddToInventory(inventory, Item.TNT, this.player);
      ShopScreen.AddToInventory(inventory, Item.C4, this.player);
      ShopScreen.AddToInventory(inventory, Item.Obsidian, this.player);
      ShopScreen.AddItemsToShopInventory(this.instance, this.player, inventory, false);
    }

    protected override bool CanEditInventory
    {
      get
      {
        return true;
      }
    }

    protected override void TransferItems()
    {
    }

    protected override void LiftAllButtonPressed()
    {
      if (this.currentSlotID > 29 && this.CursorItem.ItemID != Item.None)
        Globals1.ItemData[(int) this.CursorItem.ItemID].IsEnabled = !Globals1.ItemData[(int) this.CursorItem.ItemID].IsEnabled;
      if (Globals1.ItemData[(int) this.CursorItem.ItemID].IsEnabled)
        return;
      this.atLeastOneItemHasBeenDisabled = true;
    }

    protected override void LiftSingleButtonPressed()
    {
      List<string> data = new List<string>();
      foreach (ItemDataXML itemDataXml in Globals1.ItemData)
      {
        if (itemDataXml.IsValid && !itemDataXml.IsEnabled)
          data.Add(itemDataXml.Name);
      }
      if (data.Count == 0)
        data.Add("There are no Disabled Items");
      this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, data, (ListBoxScreen.OnMenuItemSelected) null, false), this.ControllingPlayer);
    }

    protected override void DrawCoWindow()
    {
      int height = this.pagesize / 10;
      int num1 = 171;
      this.DrawGrid(height, num1 + 36, this.chest.Inventory, 30, this.page * this.pagesize, true, false);
      int num2 = 6;
      this.spriteBatch.DrawString(this.Font, "Enable / Disable Items", new Vector2((float) (this.screenRect.X + 14), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (this.PageCount > 1)
      {
        this.spriteBatch.DrawString(this.Font, "Page " + this.pageString, new Vector2((float) (this.screenRect.X + this.screenRect.Width - 160), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.spriteBatch.DrawString(this.Font, "Flip", new Vector2((float) (this.screenRect.X + this.screenRect.Width - 50), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.triggerRect.X = this.screenRect.X + this.screenRect.Width - 70;
        this.triggerRect.Y = this.screenRect.Y + num2 - 1;
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.PrevTab, this.triggerRect);
      }
      Rectangle screenRect = this.screenRect;
      screenRect.X += 15;
      screenRect.Y = this.screenRect.Y + this.screenRect.Height - num1 - 46;
      screenRect.Width = 25;
      screenRect.Height = 25;
      if (this.currentSlotID > 29 && this.CursorItem.ItemID != Item.None)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, screenRect);
        this.spriteBatch.DrawString(this.Font, this.IsCursorItemValid ? "Disable" : "Enable", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      screenRect.X += 160;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.LiftItemSingle, screenRect);
      this.spriteBatch.DrawString(this.Font, "List Disabled", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      screenRect.X -= 160;
      screenRect.X += 388;
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, screenRect);
      this.spriteBatch.DrawString(this.Font, "Exit", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override float GetDrawItemColorAlpha(int slotID, InventoryItem item)
    {
      ChestBlock chest = this.chest;
      if (slotID <= 29 || ItemData.IsEnabled(item.ItemID_Raw))
        return base.GetDrawItemColorAlpha(slotID, item);
      return 0.6f;
    }

    protected override void DrawItemExtra(Rectangle slotRect, int slotID, InventoryItem item)
    {
      if (slotID <= 29 || ItemData.IsEnabled(item.ItemID_Raw))
        return;
      slotRect.X += 15;
      slotRect.Y += 14;
      slotRect.Width = slotRect.Height = 20;
      this.spriteBatchPoint.Draw(this.checkboxOff, slotRect, Color.White);
    }

    protected override string ItemDescriptionPanelText
    {
      get
      {
        return this.itemTypeName;
      }
    }
  }
}
