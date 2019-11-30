// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MapMarkerListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class MapMarkerListScreen : BlockMenuScreen
  {
    private Action<string> action;
    private bool exitScreenOnAction;
    private GameInstance instance;

    public MapMarkerListScreen(
      GameInstance instance,
      Player player,
      Action<string> action,
      bool exitScreenOnAction)
      : base("Marker List", player)
    {
      this.instance = instance;
      this.action = action;
      this.exitScreenOnAction = exitScreenOnAction;
      List<string> stringList = new List<string>();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      lock (instance.MapMarkers)
      {
        foreach (MapMarker mapMarker in instance.MapMarkers)
          stringList.Add(mapMarker.Label);
      }
      stringList.Sort();
      foreach (string text in stringList)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, text);
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnMarker);
        blockMenuEntryList.Add(blockMenuEntry);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 432;
      this.ItemsPerPage = 10;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void ActionOnMarker(object sender, PlayerIndexEventArgs e)
    {
      this.action((sender as BlockMenuEntry).Text);
      if (!this.exitScreenOnAction)
        return;
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
