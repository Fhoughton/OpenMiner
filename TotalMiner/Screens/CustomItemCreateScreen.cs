// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CustomItemCreateScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CustomItemCreateScreen : BlockMenuScreen
  {
    private Item itemID;
    private GameInstance instance;

    public CustomItemCreateScreen(GameInstance instance, Player player, Item itemID)
      : base("Custom Items Menu", player)
    {
      this.instance = instance;
      this.itemID = itemID;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Item: " + ItemData.ToString(itemID)));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.NameEntrySelected);
      this.AddCustomMenuItems();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    protected virtual void AddCustomMenuItems()
    {
    }

    private void ResetMenuItems()
    {
      this.MenuEntries[1].Text = "Name: ";
      this.ResetMenuItemsCustom();
    }

    protected virtual void ResetMenuItemsCustom()
    {
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

    private void NameEntrySelected(object sender, PlayerIndexEventArgs e)
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
