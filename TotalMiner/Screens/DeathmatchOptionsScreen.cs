// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.DeathmatchOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class DeathmatchOptionsScreen : BlockMenuScreen
  {
    private bool eating = true;
    private DeathmatchWinType winType;
    private GameInstance instance;

    public DeathmatchOptionsScreen(GameInstance instance, Player player)
      : base("Deathmatch Options", player)
    {
      this.instance = instance;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, instance.MiniGame == null ? "Start Game" : "End Game"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnStartSelected);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnToWinSelected);
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnFoodSelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      this.ResetMenuEntries();
      base.LoadContent();
    }

    private void ResetMenuEntries()
    {
      this.MenuEntries[1].Text = "To Win: " + Utils.InsertSpacesBeforeCapitals(this.winType.ToString());
      this.MenuEntries[2].Text = "Eating / Healing: " + (this.eating ? "On" : "Off");
    }

    private void OnStartSelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Admin, true))
      {
        if (this.instance.MiniGame != null)
        {
          this.instance.AbortMiniGame(true);
          this.ExitScreen();
        }
        else
        {
          string startError = DeathmatchMiniGame.GetStartError(this.instance);
          if (startError != null)
          {
            this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(startError, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
          }
          else
          {
            this.instance.StartDeathmatch(this.player, this.winType, this.eating, true);
            this.ExitScreen();
          }
        }
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Only Admins can start and stop a Deathmatch", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnToWinSelected(object sender, PlayerIndexEventArgs e)
    {
      int num;
      if ((num = (int) (this.winType + 1)) == 6)
        num = 0;
      this.winType = (DeathmatchWinType) num;
      this.ResetMenuEntries();
    }

    private void OnFoodSelected(object sender, PlayerIndexEventArgs e)
    {
      this.eating = !this.eating;
      this.ResetMenuEntries();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
