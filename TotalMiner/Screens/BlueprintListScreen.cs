// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BlueprintListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class BlueprintListScreen : BlockMenuScreen
  {
    private Action<string> action;
    private GameInstance instance;

    public BlueprintListScreen(GameInstance instance, Player player, Action<string> action)
      : base("Blueprint List", player)
    {
      this.instance = instance;
      this.action = action;
      List<Blueprint> blueprintList = new List<Blueprint>();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      foreach (Blueprint blueprint in Blueprints.BlueprintList)
      {
        if (blueprint.IsGenerated && !blueprint.IsEnabled)
          blueprintList.Add(blueprint);
      }
      blueprintList.Sort(new Comparison<Blueprint>(this.SortBPs));
      foreach (Blueprint blueprint in blueprintList)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, string.Format("{0}: {1},{2},{3}", (object) blueprint.Result.ItemID, (object) blueprint.Point.X, (object) blueprint.Point.Y, (object) blueprint.Point.Z));
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnBP);
        blockMenuEntryList.Add(blockMenuEntry);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    private int SortBPs(Blueprint b1, Blueprint b2)
    {
      if (b1.Point.Y == b2.Point.Y)
        return b1.Result.ItemID.CompareTo((object) b2.Result.ItemID);
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

    private void ActionOnBP(object sender, PlayerIndexEventArgs e)
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
