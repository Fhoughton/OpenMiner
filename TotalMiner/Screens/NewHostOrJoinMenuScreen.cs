// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NewHostOrJoinMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NewHostOrJoinMenuScreen : BlockMenuScreen
  {
    public NewHostOrJoinMenuScreen()
      : base("Host or Join", (Player) null)
    {
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Host a Game"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Join a Game"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.HostGameMenuEntrySelected);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.JoinGameMenuEntrySelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 288;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void HostGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.HostOrJoin = HostOrJoin.Host;
      this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void JoinGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.HostOrJoin = HostOrJoin.Join;
      this.ScreenManager.AddScreen((GameScreen) new NewJoinLobbyMenuScreen(), this.ControllingPlayer);
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
