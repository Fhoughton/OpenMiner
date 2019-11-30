// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ClanListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ClanListScreen : BlockMenuScreen
  {
    private Action<string> action;
    private bool exitScreenOnAction;

    public ClanListScreen(Player player, Action<string> action, bool exitScreenOnAction)
      : base("Clan List", player)
    {
      this.action = action;
      this.exitScreenOnAction = exitScreenOnAction;
      List<string> stringList = new List<string>();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      foreach (Gamer allGamer in NetworkManager.Instance.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag.ClanName != null && (tag.ClanName.Length > 0 && !stringList.Contains(tag.ClanName)))
        {
          stringList.Add(tag.ClanName);
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, tag.ClanName);
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnClanSelected);
          blockMenuEntryList.Add(blockMenuEntry);
        }
      }
      foreach (SavePlayerState playerSave in player.GameInstance.PlayerSaves)
      {
        if (playerSave.ClanName != null && playerSave.ClanName.Length > 0 && !stringList.Contains(playerSave.ClanName))
        {
          stringList.Add(playerSave.ClanName);
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, playerSave.ClanName);
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnClanSelected);
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

    private void OnClanSelected(object sender, PlayerIndexEventArgs e)
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
