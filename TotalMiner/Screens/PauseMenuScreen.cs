// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PauseMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class PauseMenuScreen : BlockMenuScreen
  {
    private bool timeToExit;
    private bool timeToExitNonHost;
    private GameInstance instance;
    private int playersToExit;

    public PauseMenuScreen(GameInstance instance, Player player)
      : base("Game Menu", player)
    {
      this.instance = instance;
      this.player = player;
      instance.PauseGame();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Resume"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, instance.IsAvatarDesigner ? "Save Avatar" : "Save"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Player"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Game"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Custom"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Multiplayer"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Menu"));
      blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.ResumeMenuEntrySelected);
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.SaveMenuEntrySelected);
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.PlayerMenuEntrySelected);
      blockMenuEntryList[3].Selected += new EventHandler<PlayerIndexEventArgs>(this.GameMenuEntrySelected);
      blockMenuEntryList[4].Selected += new EventHandler<PlayerIndexEventArgs>(this.CustomMenuEntrySelected);
      blockMenuEntryList[5].Selected += new EventHandler<PlayerIndexEventArgs>(this.CreativeMenuEntrySelected);
      blockMenuEntryList[6].Selected += new EventHandler<PlayerIndexEventArgs>(this.MultiplayerMenuEntrySelected);
      blockMenuEntryList[7].Selected += new EventHandler<PlayerIndexEventArgs>(this.NewMenuEntrySelected);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Quit"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ExitMenuEntrySelected);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.MenuEntries[4].IsEnabled = instance.GetEventScript(ScriptEvent.CustomMenu) != null;
      this.MenuEntries[5].IsEnabled = instance.CanOpenCreativeMenu(player);
      this.MenuEntries[6].IsEnabled = instance.IsMultiplayer || instance.IsSplitScreen;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 240;
      this.ItemHeight = 36;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.player == null || this != this.player.PauseMenuScreen)
        return;
      this.player.PauseMenuScreen = (GameScreen) null;
    }

    private void ResumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.OnCancel(sender, e);
    }

    private void SaveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (!this.CheckSaveIsAllowed())
        return;
      this.SaveGame();
    }

    private void SaveGame()
    {
      this.instance.MapStrategyTM.ResetAllButtons();
      this.ScreenManager.AddScreen((GameScreen) new SavingScreen(this.instance, this.player, new Action<bool, bool>(this.OnSaveComplete)), new PlayerIndex?());
      this.ExitScreen();
    }

    private void OnSaveComplete(bool saveSuccessful, bool anotherSaveInProgress)
    {
      if (!anotherSaveInProgress)
        return;
      this.ShowAutoSaveInProgress();
    }

    private void PlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new PlayerMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void GameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new GameMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void CustomMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.ExecuteEventScript(ScriptEvent.CustomMenu, new ScriptExecuteData()
      {
        Actor = (Actor) this.player
      });
      this.ExitScreen();
    }

    private void CreativeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new CreativeMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void MultiplayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new MultiplayerOptionsMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    private void NewMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen2(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.instance.IsMultiplayer && !NetworkManager.Instance.IsHost)
      {
        if (NetworkManager.Instance.LocalGamerCount == 1)
        {
          MessageBoxScreen messageBoxScreen = (MessageBoxScreen) new MessageBoxScreenTM("Are you sure", "Yes", (string) null, (string) null, "No, don't quit", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
          messageBoxScreen.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ExitGameRemote);
          this.ScreenManager.AddScreen((GameScreen) messageBoxScreen, this.ControllingPlayer);
        }
        else
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("To quit you must sign out", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else if (this.player.IsHost)
      {
        MessageBoxScreen messageBoxScreen = (MessageBoxScreen) new MessageBoxScreenTM("Save before quitting?\n", "Yes, save then quit", "No, just quit", (string) null, "Don't quit", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreen.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.SaveThenExitGame);
        messageBoxScreen.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.ExitGame);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreen, this.ControllingPlayer);
      }
      else if (this.instance.LocalPlayerCount == 1)
        this.ExitGameCore();
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("To quit you must sign out", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void SaveThenExitGame(object sender, PlayerIndexEventArgs e)
    {
      if (!this.CheckSaveIsAllowed())
        return;
      Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerLeave);
      if (eventScript != null)
      {
        this.instance.AddMapActiveNotPausedOverride();
        this.playersToExit = NetworkManager.Instance.LocalEnabledPlayers.Count;
        ScriptExecuteData data = new ScriptExecuteData()
        {
          OnComplete = new Action<Script, Player>(this.SaveThenExitGameAfterScript)
        };
        foreach (Player localEnabledPlayer in NetworkManager.Instance.LocalEnabledPlayers)
        {
          data.Actor = (Actor) localEnabledPlayer;
          this.instance.ExecuteScript(eventScript, data, false);
        }
      }
      else
        this.SaveThenExitGameAfterScript((Script) null, this.player);
    }

    private void SaveThenExitGameAfterScript(Script script, Player player)
    {
      if (--this.playersToExit != 0 && script != null)
        return;
      this.instance.MapStrategyTM.ResetAllButtons();
      this.instance.RemoveMapActiveNotPausedOverride();
      this.ScreenState = ScreenState.UserHidden;
      this.ScreenManager.AddScreen((GameScreen) new SavingScreen(this.instance, player, new Action<bool, bool>(this.ExitGameAfterSave)), new PlayerIndex?());
    }

    private bool CheckSaveIsAllowed()
    {
      if (Globals2.GameProperties.IsSystemMap)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("System worlds cannot be saved.\nIf you want to play and save on a system world\nthen make a copy of it first and play on the copy.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        return false;
      }
      if (this.player.IsHost || this.player.IsGod)
        return true;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You do not have permission to save this world", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      return false;
    }

    private void ExitGame(object sender, PlayerIndexEventArgs e)
    {
      this.ExitGameCore();
    }

    private void ExitGameRemote(object sender, PlayerIndexEventArgs e)
    {
      Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerLeave);
      if (eventScript != null)
      {
        this.instance.AddMapActiveNotPausedOverride();
        ScriptExecuteData data = new ScriptExecuteData()
        {
          Actor = (Actor) this.player,
          OnComplete = new Action<Script, Player>(this.ExitGameRemoteAfterScript)
        };
        this.instance.ExecuteScript(eventScript, data, true);
      }
      else
        this.ExitGameCore();
    }

    private void ExitGameRemoteAfterScript(Script script, Player player)
    {
      this.instance.RemoveMapActiveNotPausedOverride();
      this.ExitGameCore();
    }

    private void ExitGameAfterSave(bool saveSuccessful, bool anotherSaveInProgress)
    {
      if (saveSuccessful)
        this.timeToExit = true;
      else if (anotherSaveInProgress)
      {
        this.ShowAutoSaveInProgress();
        this.ExitScreen();
      }
      else
        this.ExitScreen();
    }

    private void ShowAutoSaveInProgress()
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("An Auto Save is in progress.\nPlease try again after it has finished.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void ExitGameCore()
    {
      if (Monitor.TryEnter(Globals1.SaveSemaphore))
      {
        try
        {
          if (this.instance.IsHost)
          {
            TotalMinerGame.Instance.ExitBackToMainMenu();
          }
          else
          {
            NetworkManager.Instance.SendInventory(this.player);
            NetworkManager.Instance.Update();
            if (this.player.SaveState.RatingStars == (byte) 0 && this.player.HasPermission(Permissions.Adventure) && Globals2.GameProperties.SaveGame.Header.Attribute != MapAttribute.WorkInProgress)
            {
              this.ScreenManager.AddScreen((GameScreen) new RateWorldMenuScreen(this.instance, this.player, new Action(this.ExitGameCoreNonHostSetup)), this.ControllingPlayer);
            }
            else
            {
              if (this.player.SaveState.RatingStars == (byte) 0)
                Globals2.GamertagData.AddServerRating((Gamer) this.player.Gamer, (byte) 0);
              this.ExitGameCoreNonHostSetup();
            }
          }
        }
        finally
        {
          Monitor.Exit(Globals1.SaveSemaphore);
        }
      }
      else
        TotalMinerGame.Instance.AddNotification("Disk access in progress. Please try again when the spinning disk (top right) is gone.", false);
    }

    private void ExitGameCoreNonHostSetup()
    {
      this.timeToExitNonHost = true;
    }

    private void ExitGameCoreNonHost()
    {
      this.instance.RemovePlayer(this.player, false, false);
      Globals2.SaveGamertagDataThreaded(Globals2.HighscoreDataChanged, true);
      this.ExitScreen();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.timeToExit)
      {
        this.timeToExit = false;
        this.ScreenManager.ExitAllPlayerScreens();
        this.ExitGameCore();
      }
      else
      {
        if (!this.timeToExitNonHost)
          return;
        this.timeToExitNonHost = false;
        this.ExitGameCoreNonHost();
      }
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
      string buttonTextY = this.MenuEntries[this.selectedEntry].ButtonTextY;
      if (buttonTextY.IsEmpty())
        return;
      int x1 = this.MenuRect.X + 134;
      int y = this.MenuRect.Y + this.MenuRect.Height - 30;
      if (this.selectedEntry == 1)
      {
        if (this.instance.IsAvatarDesigner)
          return;
        x1 = this.MenuRect.X + 140;
        y = this.MenuRect.Y + 46;
      }
      SpriteFont gameFont = CoreGlobals.GameFont;
      Rectangle destinationRectangle = new Rectangle(x1, y, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, this.ColorWhite);
      int num1 = x1 + 32;
      if (this.selectedEntry != 1)
      {
        this.SpriteBatch.DrawString(gameFont, buttonTextY, new Vector2((float) num1, (float) y), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 1f);
      }
      else
      {
        int num2 = y - 7;
        this.SpriteBatch.DrawString(gameFont, "Change", new Vector2((float) num1, (float) num2), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale * 0.8f, SpriteEffects.None, 1f);
        int num3 = num1 + 2;
        int num4 = num2 + 20;
        this.SpriteBatch.DrawString(gameFont, "Device", new Vector2((float) num3, (float) num4), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale * 0.8f, SpriteEffects.None, 1f);
      }
    }
  }
}
