// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WisdomListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class WisdomListScreen : BlockMenuScreen
  {
    private Action<string> action;
    private GameInstance instance;

    public WisdomListScreen(GameInstance instance, Player player, Action<string> action)
      : base("Wisdom List", player)
    {
      this.instance = instance;
      this.action = action;
      List<WisdomItem> wisdomItemList = new List<WisdomItem>();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      foreach (WisdomItem wisdom in Wisdom.WisdomList)
      {
        if (wisdom.IsGenerated && !wisdom.IsEnabled)
          wisdomItemList.Add(wisdom);
      }
      wisdomItemList.Sort(new Comparison<WisdomItem>(this.SortWisdoms));
      foreach (WisdomItem wisdomItem in wisdomItemList)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, string.Format("{0}: {1},{2},{3}", (object) wisdomItem.ID, (object) wisdomItem.Point.X, (object) wisdomItem.Point.Y, (object) wisdomItem.Point.Z));
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnWisdom);
        blockMenuEntryList.Add(blockMenuEntry);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private int SortWisdoms(WisdomItem b1, WisdomItem b2)
    {
      if (b1.Point.Y == b2.Point.Y)
        return b1.ID.CompareTo(b2.ID);
      return b1.Point.Y.CompareTo(b2.Point.Y);
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 432;
      this.ItemsPerPage = 20;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void ActionOnWisdom(object sender, PlayerIndexEventArgs e)
    {
      this.action((sender as BlockMenuEntry).Text);
      this.ExitScreen();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
