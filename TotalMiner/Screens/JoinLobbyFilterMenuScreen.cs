// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.JoinLobbyFilterMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class JoinLobbyFilterMenuScreen : BlockMenuScreen
  {
    private bool changed;
    private NetworkSessionProperties filter;
    private Action callBackIfFilterChanged;

    public JoinLobbyFilterMenuScreen(Action callBackIfFilterChanged)
      : base("Find Filter", (Player) null)
    {
      this.callBackIfFilterChanged = callBackIfFilterChanged;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      this.filter = NewJoinLobbyMenuScreen.Filter;
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.filter[2] = this.filter[2].HasValue ? (this.filter[2].Value != 1 ? (this.filter[2].Value != 3 ? (this.filter[2].Value != 4 ? new int?() : new int?(2)) : new int?(4)) : new int?(3)) : new int?(1);
        this.changed = true;
        this.ResetMenuEntries();
      });
      blockMenuEntryList[1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.filter[5] = this.filter[5].HasValue ? (this.filter[5].Value != 2 ? (this.filter[5].Value != 0 ? (this.filter[5].Value != 1 ? (this.filter[5].Value != 5 ? (this.filter[5].Value != 3 ? (this.filter[5].Value != 6 ? (this.filter[5].Value != 9 ? (this.filter[5].Value != 4 ? (this.filter[5].Value != 7 ? (this.filter[5].Value != 8 ? new int?() : new int?(10)) : new int?(8)) : new int?(7)) : new int?(4)) : new int?(9)) : new int?(6)) : new int?(3)) : new int?(5)) : new int?(1)) : new int?(0)) : new int?(2);
        this.changed = true;
        this.ResetMenuEntries();
      });
      blockMenuEntryList[2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.filter[4] = this.filter[4].HasValue ? (this.filter[4].Value != 1 ? new int?() : new int?(0)) : new int?(1);
        this.changed = true;
        this.ResetMenuEntries();
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.ResetMenuEntries();
    }

    private void ResetMenuEntries()
    {
      this.MenuEntries[0].Text = "Game Mode: " + (this.filter[2].HasValue ? ((GameMode) this.filter[2].Value).ToString() : "Any");
      this.MenuEntries[1].Text = "Attribute: " + (this.filter[5].HasValue ? ((MapAttribute) this.filter[5].Value).ToString() : "Any");
      this.MenuEntries[2].Text = "Skills: " + (this.filter[4].HasValue ? (this.filter[4].Value == 1 ? "On" : "Off") : "Any");
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 384;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      if (this.callBackIfFilterChanged == null || !this.changed)
        return;
      this.callBackIfFilterChanged();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
