// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CustomItemMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CustomItemMenuScreen : BlockMenuScreen
  {
    private Item itemID;
    private GameInstance instance;

    public CustomItemMenuScreen(GameInstance instance, Player player)
      : base("Scripts Menu", player)
    {
      this.instance = instance;
      this.itemID = Item.Wand;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Item: " + ItemData.ToString(this.itemID)));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Create"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Remove"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int index1 = 0;
      blockMenuEntryList1[index1].IsEnabled = false;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index2 = index1;
      int index3 = index2 + 1;
      blockMenuEntryList2[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.SelectItemEntrySelected);
      blockMenuEntryList1[index3].IsEnabled = this.itemID != Item.None;
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index4 = index3;
      int index5 = index4 + 1;
      blockMenuEntryList3[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.CreateItemEntrySelected);
      blockMenuEntryList1[index5].IsEnabled = this.itemID != Item.None;
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index6 = index5;
      int index7 = index6 + 1;
      blockMenuEntryList4[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditItemEntrySelected);
      blockMenuEntryList1[index7].IsEnabled = this.itemID != Item.None;
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index8 = index7;
      int num1 = index8 + 1;
      blockMenuEntryList5[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.RemoveItemEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index9 = num1;
      int num2 = index9 + 1;
      blockMenuEntryList6[index9].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.selectedEntry = 1;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void SelectItemEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ShopScreen(this.instance, this.player, Block.ItemShop, new Action<Item>(this.OnItemSelected)), this.ControllingPlayer);
    }

    private void OnItemSelected(Item itemID)
    {
      if (itemID == Item.None)
        return;
      this.MenuEntries[0].Text = "Item: " + ItemData.ToString(itemID);
      this.itemID = itemID;
      this.MenuEntries[1].IsEnabled = true;
      this.MenuEntries[2].IsEnabled = true;
      this.MenuEntries[3].IsEnabled = true;
    }

    private void CreateItemEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new CustomItemCreateScreen(this.instance, this.player, this.itemID), this.ControllingPlayer);
    }

    private void EditItemEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    private void RemoveItemEntrySelected(object sender, PlayerIndexEventArgs e)
    {
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
