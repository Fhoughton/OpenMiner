// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LootTableScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class LootTableScreen : BlockMenuScreen
  {
    public readonly int MaxColumns = 2;
    public int Column;
    private GameInstance instance;
    private Inventory inventory;
    private LootTable lootTable;
    private Action onExit;

    public LootTableScreen(Player player, LootTable lootTable, Action onExit)
      : base("Loot Table", player)
    {
      this.instance = player.GameInstance;
      this.lootTable = lootTable;
      this.onExit = onExit;
      this.inventory = new Inventory(20);
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Item                                             Count      Chance(%)"));
      for (int index = 0; index < lootTable.Table.Count; ++index)
      {
        LootDrop lootDrop = lootTable.Table[index];
        LootDropMenuEntry lootDropMenuEntry = new LootDropMenuEntry(this, lootTable, index);
        blockMenuEntryList.Add((BlockMenuEntry) lootDropMenuEntry);
        this.inventory.AddToInventory(new InventoryItem(lootDrop.ItemID, lootDrop.Count));
      }
      BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------");
      blockMenuEntryList.Add(blockMenuEntry1);
      BlockMenuEntry blockMenuEntry2 = new BlockMenuEntry((BlockMenuScreen) this, "Edit Items");
      blockMenuEntry2.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEditItemsSelected);
      blockMenuEntryList.Add(blockMenuEntry2);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 768;
      this.ItemsPerPage = 16;
      this.DrawItemLines = this.DrawEntryLines = false;
      this.DrawLastLine = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.onExit == null)
        return;
      this.onExit();
    }

    public override bool HandleInput(InputState input)
    {
      return base.HandleInput(input);
    }

    private void OnEditItemsSelected(object sender, EventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ShopScreen(this.instance, this.player, this.inventory, new Action(this.OnItemsEdited)), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnDeleteItemSelected(object sender, EventArgs e)
    {
      int tag = (int) this.MenuEntries[this.selectedEntry].Tag;
      if (tag >= 0 && tag < this.lootTable.Table.Count)
        this.lootTable.Table.RemoveAt(tag);
      this.ScreenManager.AddScreen((GameScreen) new LootTableScreen(this.player, this.lootTable, this.onExit), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnItemsEdited()
    {
      for (int index = 0; index <= this.inventory.LastItemIndex; ++index)
      {
        InventoryItem inventoryItem = this.inventory[index];
        while (index >= this.lootTable.Table.Count)
          this.lootTable.Table.Add(new LootDrop());
        LootDrop lootDrop = this.lootTable.Table[index];
        lootDrop.ItemID = inventoryItem.ItemID;
        lootDrop.Count = inventoryItem.Count;
        this.lootTable.Table[index] = lootDrop;
      }
      while (this.lootTable.Table.Count > this.inventory.LastItemIndex + 1)
        this.lootTable.Table.RemoveAt(this.lootTable.Table.Count - 1);
      this.ScreenManager.AddScreen((GameScreen) new LootTableScreen(this.player, this.lootTable, this.onExit), this.ControllingPlayer);
      this.ExitScreen();
    }

    protected override void DrawBottomBar()
    {
      Rectangle rectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 120, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, rectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      rectangle.Y += 7;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
