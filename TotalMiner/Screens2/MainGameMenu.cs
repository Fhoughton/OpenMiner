// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainGameMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainGameMenu : NewGuiMenu2
  {
    private GameProperties gameProperties;
    private SaveMapHead header;
    private TextBox showGuiHelpWin;
    private Window guiHelpContainer;

    public override string Name
    {
      get
      {
        return "Game";
      }
    }

    public MainGameMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
      this.gameProperties = Globals2.GameProperties;
      this.header = this.gameProperties.SaveGame.Header;
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private void InitMainContainer()
    {
      PlayerStats.Stat[] mapStatsAsText = PlayerStats.GetMapStatsAsText(this.instance);
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -100;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int y1 = 110;
      int width1 = 150;
      int width2 = 350;
      int height1 = 34;
      int num1 = 4;
      int num2 = 13 + mapStatsAsText.Length;
      int height2 = height1 * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      Window window1 = new Window((string) null, winRect.Width / 2 - 100 - (width1 + 1 + width2), y1, width1 + 1 + width2, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int y2;
      int x1 = y2 = 0;
      Window window2 = (Window) new TextBox("World:", x1, y2, width1, height1, textScale);
      window2.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      DataField dataField1;
      DataField dataField2 = dataField1 = new DataField(this.header.MapName, x1 + width1 + 1, y2, width2, height1, textScale)
      {
        MaxLength = 14
      };
      Window window3 = (Window) dataField1;
      this.initialNavigable = (Window) dataField1;
      window3.IsEnabled = this.player.IsHost;
      window3.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateWorldName);
      window1.AddChild((StudioForge.Engine.Core.Node) dataField2);
      int y3 = y2 + (height1 + num1);
      Window window4 = (Window) new TextBox("Creator:", x1, y3, width1, height1, textScale);
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      Window window5 = (Window) new TextBox(this.header.OwnerGamerTag, x1 + width1 + 1, y3, width2, height1, textScale);
      window5.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      int y4 = y3 + (height1 + num1);
      Window window6 = (Window) new TextBox("Created:", x1, y4, width1, height1, textScale);
      window6.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      Window window7 = (Window) new TextBox(Utils.DateFromBinary(this.header.DateCreated).ToString(), x1 + width1 + 1, y4, width2, height1, textScale);
      window7.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      int y5 = y4 + (height1 + num1);
      Window window8 = (Window) new TextBox("Mode:", x1, y5, width1, height1, textScale);
      window8.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      Window window9 = (Window) new TextBox(this.header.GameMode.ToString(), x1 + width1 + 1, y5, width2, height1, textScale);
      window9.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window9);
      int y6 = y5 + (height1 + num1);
      Window window10 = (Window) new TextBox("Attribute:", x1, y6, width1, height1, textScale);
      window10.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window10);
      DropDown dropDown1;
      Window window11 = (Window) (dropDown1 = new DropDown(this.header.Attribute.ToString(), x1 + width1 + 1, y6, width2, height1, 360, textScale));
      dropDown1.AddFlags(Window.WinFlags.KeepItemsSorted);
      window11.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      dropDown1.PopulateList = new Action<Window, List<string>, string>(this.PopulateAttributes);
      ((ITextInputWindow) dropDown1).OnValidateInput = new Action<ITextInputWindow>(this.ValidateAttribute);
      window1.AddChild((StudioForge.Engine.Core.Node) window11);
      int y7 = y6 + (height1 + num1);
      Window window12 = (Window) new TextBox("Seed:", x1, y7, width1, height1, textScale);
      window12.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window12);
      Window window13 = (Window) new TextBox(this.header.MapSeed.ToString(), x1 + width1 + 1, y7, width2, height1, textScale);
      window13.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window13);
      int y8 = y7 + (height1 + num1);
      Window window14 = (Window) new TextBox("Season:", x1, y8, width1, height1, textScale);
      window14.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window14);
      Window window15 = (Window) new TextBox(this.instance.SunMoon.Season.ToString(), x1 + width1 + 1, y8, width2, height1, textScale);
      window15.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window15);
      int y9 = y8 + (height1 + num1) + (height1 + num1);
      Window window16 = (Window) new TextBox("Host:", x1, y9, width1, height1, textScale);
      window16.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window16);
      Window window17 = (Window) new TextBox(this.header.OwnerGamerTag, x1 + width1 + 1, y9, width2, height1, textScale);
      window17.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window17);
      int y10 = y9 + (height1 + num1);
      Window window18 = (Window) new TextBox("Session:", x1, y10, width1, height1, textScale);
      window18.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window18);
      Window window19 = (Window) new TextBox(this.instance.NetworkManager.SessionType.ToString(), x1 + width1 + 1, y10, width2, height1, textScale);
      window19.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window19);
      int y11 = y10 + (height1 + num1);
      Window window20 = (Window) new TextBox("Players:", x1, y11, width1, height1, textScale);
      window20.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window20);
      Window window21 = (Window) new TextBox(this.instance.NetworkManager.AllGamerCount.ToString(), x1 + width1 + 1, y11, width2, height1, textScale);
      window21.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window21);
      int y12 = y11 + (height1 + num1);
      Window window22 = (Window) new TextBox("Active Mods:", x1, y12, width1, height1, textScale);
      window22.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window22);
      int count = ModManager.ActiveMods.Count;
      DropDown dropDown2;
      Window window23 = (Window) (dropDown2 = new DropDown(count.ToString(), x1 + width1 + 1, y12, width2, height1, 360, textScale));
      dropDown2.AddFlags(Window.WinFlags.KeepItemsSorted);
      window23.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateActiveMods);
      ((ITextInputWindow) dropDown2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateActiveMods);
      window1.AddChild((StudioForge.Engine.Core.Node) window23);
      int y13 = y12 + (height1 + num1) + (height1 + num1);
      int width3 = 325;
      int width4 = 175;
      for (int index = 5; index < mapStatsAsText.Length; ++index)
      {
        PlayerStats.Stat stat = mapStatsAsText[index];
        Window window24 = (Window) new TextBox(stat.Desc, x1, y13, width3, height1, textScale);
        window24.Colors = (Window.ColorProfile) Colors.LabelColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window24);
        Window window25 = (Window) new TextBox(stat.Value.ToString(), x1 + width3 + 1, y13, width4, height1, textScale, WinTextAlignX.Right, WinTextAlignY.Center);
        window25.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window25);
        y13 += height1 + num1;
      }
      int num3 = 9;
      int y14 = 110;
      int width5 = 300;
      int height3 = height1 * num3 + num1 * (num3 - 1);
      Window window26 = new Window((string) null, winRect.Width / 2 + 100, y14, width5, height3)
      {
        Name = "gameOptionsContainer"
      };
      window26.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window26);
      int y15;
      int x2 = y15 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window27 = (Window) new TextBox("Save World", x2, y15, width5, height1, textScale);
      window27.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window27.ClickHandler += new Window.WindowHandler(this.ClickSaveGame);
      window26.AddChild((StudioForge.Engine.Core.Node) window27);
      int y16 = y15 + (height1 + num1);
      Window window28 = (Window) new TextBox("Save World and Quit", x2, y16, width5, height1, textScale);
      window28.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window28.ClickHandler += new Window.WindowHandler(this.ClickSaveGameAndQuit);
      window26.AddChild((StudioForge.Engine.Core.Node) window28);
      int y17 = y16 + (height1 + num1);
      Window window29 = (Window) new TextBox("Quit to Main Menu", x2, y17, width5, height1, textScale);
      window29.Colors = (Window.ColorProfile) Colors.ButtonWarnColors;
      window29.ClickHandler += new Window.WindowHandler(this.ClickQuitToMenu);
      window26.AddChild((StudioForge.Engine.Core.Node) window29);
      int y18 = y17 + (height1 + num1) + (height1 + num1);
      Window window30 = (Window) new TextBox("Custom Menu", x2, y18, width5, height1, textScale);
      window30.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window30.IsEnabled = this.instance.GetEventScript(ScriptEvent.CustomMenu) != null;
      window30.ClickHandler += new Window.WindowHandler(this.ClickCustomMenu);
      window26.AddChild((StudioForge.Engine.Core.Node) window30);
      int y19 = y18 + (height1 + num1);
      Window window31 = (Window) new TextBox("Multiplayer", x2, y19, width5, height1, textScale);
      window31.IsEnabled = this.instance.IsMultiplayer || this.instance.IsSplitScreen || this.IsGodOrTester;
      window31.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window31.ClickHandler += new Window.WindowHandler(this.ClickMultiplayer);
      window26.AddChild((StudioForge.Engine.Core.Node) window31);
      int y20 = y19 + (height1 + num1);
      Window window32 = (Window) new TextBox("Scripts", x2, y20, width5, height1, textScale);
      window32.IsEnabled = this.IsGodOrTester || this.instance.IsItemUnlocked(Item.ScriptBlock) && this.player.HasPermissionAny(Permissions.Adventure | Permissions.Admin);
      window32.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window32.ClickHandler += new Window.WindowHandler(this.ClickScripts);
      window26.AddChild((StudioForge.Engine.Core.Node) window32);
      int y21 = y20 + (height1 + num1);
      Window window33 = (Window) new TextBox("Zones", x2, y21, width5, height1, textScale);
      window33.IsEnabled = this.player.IsAdmin && !this.instance.IsAvatarDesigner;
      window33.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window33.ClickHandler += new Window.WindowHandler(this.ClickZones);
      window26.AddChild((StudioForge.Engine.Core.Node) window33);
      int y22 = y21 + (height1 + num1);
      Window window34 = (Window) new TextBox("Old Menu", x2, y22, width5, height1, textScale);
      window34.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window34.ClickHandler += new Window.WindowHandler(this.ClickOldMenu);
      window26.AddChild((StudioForge.Engine.Core.Node) window34);
      int y23 = y22 + (height1 + num1);
      Window window35 = (Window) new TextBox("Game Help", x2, y23, width5, height1, textScale);
      window35.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window35.ClickHandler += new Window.WindowHandler(this.ClickGameHelp);
      window26.AddChild((StudioForge.Engine.Core.Node) window35);
      int y24 = y23 + (height1 + num1);
      Window window36 = (Window) (this.showGuiHelpWin = new TextBox((Globals2.GuiHelpVisible ? "Hide " : "") + "Gui Help", x2, y24, width5, height1, textScale));
      window36.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window36.ClickHandler += new Window.WindowHandler(this.ClickToggleGuiHelpText);
      window26.AddChild((StudioForge.Engine.Core.Node) window36);
      int y25 = y24 + (height1 + num1) - 2;
      int width6 = 612;
      int height4 = 24;
      int num4 = 0;
      Window window37;
      this.guiHelpContainer = window37 = new Window((string) null, x2, y25, width6 + 8, height4 * 12 + 10)
      {
        Name = "guiHelpContainer",
        BorderThickness = 1
      };
      window37.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
      window26.AddChild((StudioForge.Engine.Core.Node) window37);
      this.guiHelpContainer.IsVisible = Globals2.GuiHelpVisible;
      int y26;
      int x3 = y26 = 4;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window38 = (Window) new TextBox("This graphical interface uses a sliding system to allow for", x3, y26, width6, height4, textScale);
      window38.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window38);
      int y27 = y26 + (height4 + num4);
      Window window39 = (Window) new TextBox("screens wider than the physical viewport and to facilitate", x3, y27, width6, height4, textScale);
      window39.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window39);
      int y28 = y27 + (height4 + num4);
      Window window40 = (Window) new TextBox("faster navigation for gamers who use controllers.", x3, y28, width6, height4, textScale);
      window40.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window40);
      int y29 = y28 + (height4 + num4) + (height4 + num4);
      Window window41 = (Window) new TextBox("If you do not like the sliding, you can toggle it off by", x3, y29, width6, height4, textScale);
      window41.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window41);
      int y30 = y29 + (height4 + num4);
      Window window42 = (Window) new TextBox("pressing the Scroll Lock key on your keyboard. You can", x3, y30, width6, height4, textScale);
      window42.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window42);
      int y31 = y30 + (height4 + num4);
      Window window43 = (Window) new TextBox("use the mouse wheel to zoom the interface in or out to", x3, y31, width6, height4, textScale);
      window43.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window43);
      int y32 = y31 + (height4 + num4);
      Window window44 = (Window) new TextBox("ensure it is fully visible on the screen before locking.", x3, y32, width6, height4, textScale);
      window44.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window44);
      int y33 = y32 + (height4 + num4);
      Window window45 = (Window) new TextBox("Click the mouse wheel to reset the zoom.", x3, y33, width6, height4, textScale);
      window45.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window45);
      int y34 = y33 + (height4 + num4) + (height4 + num4);
      Window window46 = (Window) new TextBox("Locking is set per tab. A small padlock icon at the top", x3, y34, width6, height4, textScale);
      window46.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window46);
      int y35 = y34 + (height4 + num4);
      Window window47 = (Window) new TextBox("right of the screen indicates if the tab is locked or not.", x3, y35, width6, height4, textScale);
      window47.Colors = (Window.ColorProfile) Colors.BlackText;
      this.guiHelpContainer.AddChild((StudioForge.Engine.Core.Node) window47);
    }

    private void ValidateWorldName(ITextInputWindow win)
    {
      if (win.Text.IsNotEmpty())
        Globals2.GameProperties.SaveGame.Header.MapName = win.Text;
      win.Text = Globals2.GameProperties.SaveGame.Header.MapName;
    }

    private void PopulateAttributes(Window win, List<string> list, string input)
    {
      list.Clear();
      list.AddRange((IEnumerable<string>) Utils.BuildEnumStringArray<MapAttribute>());
      list.Remove("zLast");
      list.Remove("AvatarDesigner");
    }

    private void ValidateAttribute(ITextInputWindow win)
    {
      this.header.Attribute = (MapAttribute) Utils.GetEnumFromString(typeof (MapAttribute), win.Text);
    }

    private void PopulateActiveMods(Window win, List<string> list, string input)
    {
      list.Clear();
      foreach (Mod activeMod in ModManager.ActiveMods)
        list.Add(activeMod.Name);
    }

    private void ValidateActiveMods(ITextInputWindow win)
    {
      win.Text = ModManager.ActiveMods.Count.ToString();
    }

    private void ClickSaveGame(object sender, WindowEventArgs e)
    {
      if (!this.SaveGame(new Action<bool, bool>(this.OnSaveComplete)))
        return;
      this.ExitScreen();
    }

    private void ClickSaveGameAndQuit(object sender, WindowEventArgs e)
    {
      if (!this.SaveGame(new Action<bool, bool>(this.OnSaveCompleteThenQuit)))
        return;
      this.ExitScreen();
    }

    private bool SaveGame(Action<bool, bool> onComplete)
    {
      if (!this.CheckSaveIsAllowed())
        return false;
      this.SaveGameCore(onComplete);
      return true;
    }

    private bool CheckSaveIsAllowed()
    {
      if (Globals2.GameProperties.IsSystemMap)
      {
        this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("System worlds cannot be saved.\nIf you want to play and save on a system world\nthen make a copy of it first and play on the copy.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
        return false;
      }
      if (this.player.IsHost || this.player.IsGod)
        return true;
      this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You do not have permission to save this world", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
      return false;
    }

    private void ShowAutoSaveInProgress()
    {
      this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("An Auto Save is in progress.\nPlease try again after it has finished.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
    }

    private void SaveGameCore(Action<bool, bool> onComplete)
    {
      this.instance.MapStrategyTM.ResetAllButtons();
      this.screenManager.AddScreen((GameScreen) new SavingScreen(this.instance, this.player, onComplete), new PlayerIndex?(this.playerIndex));
    }

    private void OnSaveComplete(bool saveSuccessful, bool anotherSaveInProgress)
    {
      this.OnSaveComplete(anotherSaveInProgress);
    }

    private void OnSaveCompleteThenQuit(bool saveSuccessful, bool anotherSaveInProgress)
    {
      this.OnSaveComplete(anotherSaveInProgress);
      if (!saveSuccessful || anotherSaveInProgress)
        return;
      this.ExitGameCore();
    }

    private void OnSaveComplete(bool anotherSaveInProgress)
    {
      if (anotherSaveInProgress)
      {
        this.ShowAutoSaveInProgress();
      }
      else
      {
        Globals2.LastMapPlayed = Globals2.GameProperties.SaveGame.DirNumber;
        Globals2.SaveGlobalData();
      }
    }

    private void ClickToggleGuiHelpText(object sender, WindowEventArgs args)
    {
      Globals2.GuiHelpVisible = !Globals2.GuiHelpVisible;
      this.guiHelpContainer.IsVisible = Globals2.GuiHelpVisible;
      this.showGuiHelpWin.Text = (Globals2.GuiHelpVisible ? "Hide " : "") + "Gui Help";
      Globals2.SaveGlobalData();
    }

    private void ClickQuitToMenu(object sender, WindowEventArgs args)
    {
      this.ExitGameCore();
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
              this.screenManager.AddScreen((GameScreen) new RateWorldMenuScreen(this.instance, this.player, new Action(this.ExitGameCoreNonHostSetup)), new PlayerIndex?(this.playerIndex));
            }
            else
            {
              if (this.player.SaveState.RatingStars == (byte) 0)
                Globals2.GamertagData.AddServerRating(this.player.SignedInGamer, (byte) 0);
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
    }

    private void ExitGameCoreNonHost()
    {
      this.instance.RemovePlayer(this.player, false, false);
      Globals2.SaveGamertagDataThreaded(Globals2.HighscoreDataChanged, true);
      this.ExitScreen();
    }

    private void ClickOldMenu(object sender, WindowEventArgs args)
    {
      this.screenManager.AddScreen((GameScreen) new PauseMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickCustomMenu(object sender, WindowEventArgs e)
    {
      this.instance.ExecuteEventScript(ScriptEvent.CustomMenu, new ScriptExecuteData()
      {
        Actor = (Actor) this.player
      });
      this.ExitScreen();
    }

    private void ClickMultiplayer(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new MultiplayerOptionsMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickScripts(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickZones(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ZoneMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickGameHelp(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new HowToMenuScreen(this.instance, this.player, HowToIndex.Main), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }
  }
}
