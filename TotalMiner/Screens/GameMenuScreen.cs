// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameMenuScreen
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
  internal class GameMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private GameOptionsScreen optScreen;

    public GameMenuScreen(GameInstance instance, Player player)
      : base("Game Menu", player)
    {
      this.instance = instance;
      this.player = player;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Options"));
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Zones"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Scripts"));
      }
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "World Info"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "How To"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OptionsMenuEntrySelected);
      if (!instance.IsAvatarDesigner)
      {
        List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
        int index2 = num2;
        int num3 = index2 + 1;
        blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.ZoneMenuEntrySelected);
        blockMenuEntryList1[num3 - 1].IsEnabled = player.IsAdmin && !instance.IsAvatarDesigner;
        List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
        int index3 = num3;
        num2 = index3 + 1;
        blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.ScriptsMenuEntrySelected);
        blockMenuEntryList1[num2 - 1].IsEnabled = player.IsGodOrTester || instance.IsItemUnlocked(Item.ScriptBlock) && player.HasPermissionAny(Permissions.Adventure | Permissions.Admin);
      }
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num2;
      int num4 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.StatisticsEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num4;
      int num5 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.HowToMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num5;
      int num6 = index6 + 1;
      blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 239;
      this.ItemHeight = 36;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.optScreen = new GameOptionsScreen(this.instance, this.player);
      this.ScreenManager.AddScreen((GameScreen) this.optScreen, this.ControllingPlayer);
    }

    private void StatisticsEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new StatsScreen(this.instance, this.player, true), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void MiniGamesEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      GameScreen screen = (GameScreen) null;
      if (this.instance.MiniGame == null)
        screen = (GameScreen) new MiniGamesMenuScreen(this.instance, this.player);
      else if (this.instance.MiniGame.GameType == MiniGameType.Deathmatch)
        screen = (GameScreen) new DeathmatchOptionsScreen(this.instance, this.player);
      this.ScreenManager.AddScreen(screen, this.ControllingPlayer);
      this.ExitScreen();
    }

    private void HowToMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new HowToMenuScreen(this.instance, this.player, HowToIndex.Main), this.ControllingPlayer);
    }

    private void ZoneMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ZoneMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void ScriptsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
