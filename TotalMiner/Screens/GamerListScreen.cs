// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GamerListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GamerListScreen : BlockMenuScreen
  {
    private int allGamersIndex = -1;
    private Action<NetworkGamer, bool, string> action;
    private bool exitScreenOnAction;

    public GamerListScreen(
      Player player,
      Action<NetworkGamer, bool, string> action,
      bool exitScreenOnAction,
      string gamertagToExclude,
      bool includeAllGamers,
      bool ignoreLocalGamers)
      : this(player, action, exitScreenOnAction, gamertagToExclude, includeAllGamers, ignoreLocalGamers, (string[]) null, (string[]) null)
    {
    }

    public GamerListScreen(
      Player player,
      Action<NetworkGamer, bool, string> action,
      bool exitScreenOnAction,
      string gamertagToExclude,
      bool includeAllGamers,
      bool ignoreLocalGamers,
      string[] extraItemsAtTop)
      : this(player, action, exitScreenOnAction, gamertagToExclude, includeAllGamers, ignoreLocalGamers, extraItemsAtTop, (string[]) null)
    {
    }

    public GamerListScreen(
      Player player,
      Action<NetworkGamer, bool, string> action,
      bool exitScreenOnAction,
      string gamertagToExclude,
      bool includeAllGamers,
      bool ignoreLocalGamers,
      string[] extraItemsAtTop,
      string[] extraItemsAtBottom)
      : base("Gamer List", player)
    {
      this.action = action;
      this.exitScreenOnAction = exitScreenOnAction;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (includeAllGamers)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, "All Gamers");
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnGamer);
        this.allGamersIndex = blockMenuEntryList.Count;
        blockMenuEntryList.Add(blockMenuEntry);
      }
      if (extraItemsAtTop != null)
      {
        foreach (string text in extraItemsAtTop)
        {
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, text);
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnGamer);
          blockMenuEntryList.Add(blockMenuEntry);
        }
      }
      foreach (NetworkGamer allGamer in NetworkManager.Instance.AllGamers)
      {
        if ((gamertagToExclude == null || allGamer.Gamertag != gamertagToExclude) && (!ignoreLocalGamers || !allGamer.IsLocal))
        {
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, allGamer.Gamertag);
          blockMenuEntry.Tag = (object) allGamer;
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnGamer);
          blockMenuEntryList.Add(blockMenuEntry);
        }
      }
      if (extraItemsAtBottom != null)
      {
        foreach (string text in extraItemsAtBottom)
        {
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, text);
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.ActionOnGamer);
          blockMenuEntryList.Add(blockMenuEntry);
        }
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

    private void ActionOnGamer(object sender, PlayerIndexEventArgs e)
    {
      BlockMenuEntry blockMenuEntry = sender as BlockMenuEntry;
      this.action(blockMenuEntry.Tag as NetworkGamer, this.selectedEntry == this.allGamersIndex, blockMenuEntry.Text);
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
