// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NewSessionTypeMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NewSessionTypeMenuScreen : BlockMenuScreen
  {
    private Action sessionTypeSelectedCallback;

    public NewSessionTypeMenuScreen(Action sessionTypeSelectedCallback)
      : base("Play", (Player) null)
    {
      this.sessionTypeSelectedCallback = sessionTypeSelectedCallback;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Single Player"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Online"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "LAN"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.LocalGameMenuEntrySelected);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnlineGameMenuEntrySelected);
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.SystemLinkGameMenuEntrySelected);
      blockMenuEntryList[3].Selected += new EventHandler<PlayerIndexEventArgs>(this.NetworkMenuEntrySelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[3].Text = "Network: " + (ModManager.NetMod != null ? ModManager.NetMod.Name : "Steam");
      this.MenuEntries[1].IsEnabled = ModManager.NetMod != null;
      this.MenuEntries[2].IsEnabled = false;
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
      if (Globals2.AutoStartMap == 0)
        return;
      this.LocalGameMenuEntrySelected((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    private void LocalGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      ModManager.NetMod = (Mod) null;
      ModManager.NetModName = (string) null;
      Globals2.GameProperties.NetworkSessionType = NetworkSessionType.Local;
      Globals2.GameProperties.SaveGame.Header.MaxPlayers = 4;
      if (this.sessionTypeSelectedCallback != null)
        this.sessionTypeSelectedCallback();
      else
        this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void OnlineGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.NetworkSessionType = NetworkSessionType.PlayerMatch;
      if (this.sessionTypeSelectedCallback != null)
        this.sessionTypeSelectedCallback();
      else
        this.ScreenManager.AddScreen((GameScreen) new NewHostOrJoinMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void SystemLinkGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.NetworkSessionType = NetworkSessionType.SystemLink;
      if (this.sessionTypeSelectedCallback != null)
        this.sessionTypeSelectedCallback();
      else
        this.ScreenManager.AddScreen((GameScreen) new NewHostOrJoinMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void NetworkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ModFilteredListMenuScreen(new Func<string, bool>(this.OnModSelected), ModFilter.HasPluginNet, new Func<string, bool>(this.IsNetModActive)), this.ControllingPlayer);
    }

    private bool IsNetModActive(string modName)
    {
      return modName == ModManager.NetModName;
    }

    private bool OnModSelected(string modName)
    {
      if (modName.IsNotEmpty())
      {
        if (ModManager.IsActiveMod(modName))
        {
          ModManager.UnloadMod(modName);
          if (modName == ModManager.NetModName)
          {
            ModManager.NetMod = (Mod) null;
            ModManager.NetModName = (string) null;
            this.ResetToggleItems();
            return true;
          }
        }
        string errorMessage;
        ModManager.NetMod = ModManager.LoadMod(modName, out errorMessage);
        if (ModManager.NetMod != null)
        {
          if (ModManager.NetMod.PluginNet != null)
          {
            ModManager.NetModName = modName;
            this.ResetToggleItems();
            return true;
          }
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("ITMPLuginNet not found.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.ControllingPlayer.Value));
        }
        else
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This mod could not be loaded: " + modName + "\n" + errorMessage, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.ControllingPlayer.Value));
      }
      this.ResetToggleItems();
      return false;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
