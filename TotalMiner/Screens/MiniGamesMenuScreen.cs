// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MiniGamesMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class MiniGamesMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;

    public MiniGamesMenuScreen(GameInstance instance, Player player)
      : base("Mini Games Options", player)
    {
      MiniGamesMenuScreen miniGamesMenuScreen = this;
      this.instance = instance;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Deathmatch"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        miniGamesMenuScreen.ScreenManager.AddScreen((GameScreen) new DeathmatchOptionsScreen(instance, player), miniGamesMenuScreen.ControllingPlayer);
        miniGamesMenuScreen.ExitScreen();
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 334;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      this.ScreenManager.AddScreen((GameScreen) new GameMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
