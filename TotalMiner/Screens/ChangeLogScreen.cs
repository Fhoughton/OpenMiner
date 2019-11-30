// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ChangeLogScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ChangeLogScreen : BlockMenuScreen
  {
    private ChangeLog changeLog;

    public ChangeLogScreen(Player player, ChangeLog changeLog)
      : base("Change Log", player)
    {
      this.changeLog = changeLog;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (changeLog == player.ChangeLog)
      {
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clear Change Log"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ClearChangeLogSelected);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
      foreach (ChangeLogItem changeLogItem in changeLog.ToList())
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, changeLogItem.Time.ToString() + "  -  " + changeLogItem.Log);
        blockMenuEntryList.Add(blockMenuEntry);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "-------------------------------------------------------------------------------------------"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 1054;
      this.ItemHeight = 24;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.5f;
      this.ItemsPerPage = this.player.GameInstance.LocalPlayerCount == 1 || this.player.GameInstance.LocalPlayerCount == 2 && Globals2.GameSettings.SplitScreenVertical ? 20 : 10;
      this.DrawLastLine = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void ClearChangeLogSelected(object sender, PlayerIndexEventArgs e)
    {
      this.changeLog.Clear();
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
