// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainScriptsMenu
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
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainScriptsMenu : NewGuiMenu2
  {
    private Window mainMenuContainer;
    private StudioForge.Engine.GUI.ListBox scriptNames;
    private StudioForge.Engine.GUI.ListBox advScriptNames;
    private StudioForge.Engine.GUI.TextBox pathName;
    private StudioForge.Engine.GUI.TextBox scriptModeWin;
    private Player scriptPlayer;
    private string path;

    public override string Name
    {
      get
      {
        return "Scripts";
      }
    }

    private string PlayerJoinsScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerJoin);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string PlayerLeavesScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerLeave);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string PlayerDiesScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerDeath);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string PlayerRespawnScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.PlayerRespawn);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    private string CustomMenuScriptName
    {
      get
      {
        Script eventScript = this.instance.GetEventScript(ScriptEvent.CustomMenu);
        if (eventScript == null)
          return "";
        return eventScript.Name;
      }
    }

    public MainScriptsMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private void InitMainContainer()
    {
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -150;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      bool flag = this.player.IsAdmin || this.IsGodOrTester;
      int x1 = 120;
      int y1 = 110;
      int width1 = 360;
      int height1 = 34;
      int num1 = 4;
      int num2 = flag ? 18 : 4;
      int num3 = height1 * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      Window window1 = this.mainMenuContainer = new Window((string) null, x1, y1, this.canvas.Size.X - x1 * 2, num3 + this.canvas.Size.Y - num1 - num3)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      int num4;
      int num5 = num4 = 0;
      int y2;
      int x2 = y2 = 0;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      string str = flag ? "Edit" : "View";
      Window window2;
      this.initialNavigable = window2 = (Window) (this.scriptModeWin = new StudioForge.Engine.GUI.TextBox("Mode: " + str, x2, y2, width1, height1, textScale));
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.IsEnabled = flag;
      window2.ClickHandler += new Window.WindowHandler(this.ClickScriptMode);
      window2.SetToolTip("Click to change what happens when you click on a script in the list below.");
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      if (flag)
      {
        Window window3 = (Window) new StudioForge.Engine.GUI.TextBox("Old Scripts Menu", window1.Size.X - width1, y2, width1, height1, textScale);
        window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window3.IsEnabled = flag;
        window3.ClickHandler += new Window.WindowHandler(this.ClickOldScriptsMenu);
        window1.AddChild((StudioForge.Engine.Core.Node) window3);
      }
      int y3 = y2 + (height1 + num1);
      if (flag)
      {
        Window window3 = (Window) new StudioForge.Engine.GUI.TextBox("Run Script for another Player", x2, y3, width1, height1, textScale);
        window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window3.IsEnabled = flag;
        window3.ClickHandler += new Window.WindowHandler(this.ClickRunScriptForAnotherPlayer);
        window1.AddChild((StudioForge.Engine.Core.Node) window3);
        Window window4 = (Window) new StudioForge.Engine.GUI.TextBox("History Log", window1.Size.X - width1, y3, width1, height1, textScale);
        window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window4.IsEnabled = flag;
        window4.ClickHandler += new Window.WindowHandler(this.ClickHistoryLog);
        window1.AddChild((StudioForge.Engine.Core.Node) window4);
        int y4 = y3 + (height1 + num1) + (height1 + num1);
        Window window5 = (Window) new StudioForge.Engine.GUI.TextBox("New Script", x2, y4, width1, height1, textScale);
        window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window5.IsEnabled = flag;
        window5.ClickHandler += new Window.WindowHandler(this.ClickNewScript);
        window1.AddChild((StudioForge.Engine.Core.Node) window5);
        Window window6 = (Window) new StudioForge.Engine.GUI.TextBox("Cancel Running Script", window1.Size.X - width1, y4, width1, height1, textScale);
        window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window6.IsEnabled = flag;
        window6.ClickHandler += new Window.WindowHandler(this.ClickCancelScript);
        window1.AddChild((StudioForge.Engine.Core.Node) window6);
        int y5 = y4 + (height1 + num1);
        Window window7 = (Window) new StudioForge.Engine.GUI.TextBox("New Script from Change Log", x2, y5, width1, height1, textScale);
        window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window7.IsEnabled = flag;
        window7.ClickHandler += new Window.WindowHandler(this.ClickNewScriptFromChangeLog);
        window1.AddChild((StudioForge.Engine.Core.Node) window7);
        int y6 = y5 + (height1 + num1) + (height1 + num1);
        StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
        int width2 = 300;
        Window window8 = (Window) new StudioForge.Engine.GUI.TextBox("Run Single Script Command:", x2, y6, width2, height1, textScale);
        window8.Colors = (Window.ColorProfile) Colors.LabelColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window8);
        int x3 = x2 + width2 + num1;
        DataField dataField = new DataField((string) null, x3, y6, window1.Size.X - x3, height1, textScale);
        Window window9 = (Window) dataField;
        window9.Colors = (Window.ColorProfile) Colors.DataFieldColors;
        window9.IsEnabled = flag;
        ((ITextInputWindow) dataField).OnValidateInput = new Action<ITextInputWindow>(this.ValidateRunSingleScriptCommand);
        window1.AddChild((StudioForge.Engine.Core.Node) window9);
        y3 = y6 + (height1 + num1);
        int width3 = 80;
        Window window10 = (Window) new StudioForge.Engine.GUI.TextBox("Editor:", x2, y3, width3, height1, textScale);
        window10.Colors = (Window.ColorProfile) Colors.LabelColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window10);
        int x4 = x2 + width3 + num1;
        StudioForge.Engine.GUI.TextBox textBox = new StudioForge.Engine.GUI.TextBox(Globals2.ExternalScriptEditor, x4, y3, window1.Size.X - x4, height1, textScale);
        textBox.Name = "editor";
        Window window11 = (Window) textBox;
        window11.Colors = (Window.ColorProfile) Colors.ButtonColors;
        GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
        window11.IsEnabled = flag && (service != null && !service.IsFullScreen);
        window11.ClickHandler += new Window.WindowHandler(this.ClickSelectExternalEditor);
        window11.SetToolTip("Click to set which external editor to use for script editing.");
        window1.AddChild((StudioForge.Engine.Core.Node) window11);
      }
      int y7 = y3 + (height1 + num1);
      int width4 = 80;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window12 = (Window) new StudioForge.Engine.GUI.TextBox("[..]", x2, y7, width4, height1, textScale);
      window12.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window12.ClickHandler += new Window.WindowHandler(this.ClickPathBack);
      window12.SetToolTip("Click to go back a Folder.");
      window1.AddChild((StudioForge.Engine.Core.Node) window12);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int x5 = x2 + width4 + num1;
      Window window13 = (Window) (this.pathName = new StudioForge.Engine.GUI.TextBox("Path: ", x5, y7, window1.Size.X - x5, height1, textScale));
      window13.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window13);
      int y8 = y7 + (height1 + num1);
      Window window14 = (Window) (this.scriptNames = new StudioForge.Engine.GUI.ListBox((string) null, x2, y8, window1.Size.X - x2 * 2 - 460, window1.Size.Y - y8 - num1));
      window14.Colors = (Window.ColorProfile) Colors.ListBoxColors;
      this.scriptNames.ClearFlags(Window.WinFlags.FilteringEnabled);
      this.scriptNames.TextScale = textScale;
      window14.Size.Y = this.scriptNames.Spacing * 14;
      this.scriptNames.ItemSelectedHandler += new Window.WindowHandler(this.ClickScriptName);
      if (flag)
      {
        window14.AddFlags(Window.WinFlags.IsDragable | Window.WinFlags.DragCopy);
        window14.DragStartHandler += new Window.WindowDragHandler(this.DragStartScriptNames);
        window14.DragEndHandler += new Window.WindowDragHandler(this.DragEndScriptNames);
        StudioForge.Engine.GUI.ListBox scriptNames = this.scriptNames;
        StudioForge.Engine.GUI.TextBox textBox1 = new StudioForge.Engine.GUI.TextBox((string) null, (int) this.scriptNames.Position.X, 0, this.scriptNames.Size.X, this.scriptNames.Spacing);
        textBox1.Colors = (Window.ColorProfile) Colors.ButtonColors;
        textBox1.TextScale = textScale;
        StudioForge.Engine.GUI.TextBox textBox2 = textBox1;
        scriptNames.DragProxyWin = (Window) textBox2;
      }
      window1.AddChild((StudioForge.Engine.Core.Node) window14);
      this.LoadScriptNames("");
      int num6 = height1;
      int height2 = this.scriptNames.Spacing - num1;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      int x6 = x2 + this.scriptNames.Size.X + num1;
      Window window15 = (Window) new StudioForge.Engine.GUI.TextBox("Adventure Scripts", x6, y8, window1.Size.X - x6, height2, textScale);
      window15.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window15);
      int y9 = y8 + (height2 + num1);
      int height3 = num6;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window16 = (Window) (this.advScriptNames = new StudioForge.Engine.GUI.ListBox((string) null, x6, y9, window1.Size.X - x6, window1.Size.Y - y9 - num1));
      window16.Colors = (Window.ColorProfile) Colors.ListBoxColors;
      this.advScriptNames.AddFlags(Window.WinFlags.KeepItemsSorted);
      this.advScriptNames.ClearFlags(Window.WinFlags.FilteringEnabled);
      this.advScriptNames.TextScale = textScale;
      window16.Size.Y = this.advScriptNames.Spacing * 13;
      this.advScriptNames.ItemSelectedHandler += new Window.WindowHandler(this.ClickAdvScriptName);
      this.advScriptNames.AddRange((IEnumerable<string>) this.instance.GetAdventureScriptNameList());
      this.advScriptNames.SetToolTip(flag ? "The scripts are available for Adventure permission players to execute manually.\n\nDrag scripts from the main list." : "Click a script here to execute it");
      window1.AddChild((StudioForge.Engine.Core.Node) window16);
      if (!flag)
        return;
      int y10 = y9 + (this.advScriptNames.Size.Y + height3 + num1);
      int width5 = 200;
      Window window17 = (Window) new StudioForge.Engine.GUI.TextBox("Player Joins:", x2, y10, width5, height3, textScale);
      window17.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window17);
      int x7 = x2 + width5 + num1;
      StudioForge.Engine.GUI.TextBox textBox3 = new StudioForge.Engine.GUI.TextBox(this.PlayerJoinsScriptName, x7, y10, window1.Size.X - x7, height3, textScale);
      textBox3.Name = "player joins";
      Window window18 = (Window) textBox3;
      window18.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window18.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
      window18.SetToolTip("This script will be executed for a player when they first join this world.\n\nTo set the script to execute for this event, drag a script from the list above and drop it here.");
      window1.AddChild((StudioForge.Engine.Core.Node) window18);
      int y11 = y10 + (height3 + num1);
      Window window19 = (Window) new StudioForge.Engine.GUI.TextBox("Player Leaves:", x2, y11, width5, height3, textScale);
      window19.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window19);
      StudioForge.Engine.GUI.TextBox textBox4 = new StudioForge.Engine.GUI.TextBox(this.PlayerLeavesScriptName, x7, y11, window1.Size.X - x7, height3, textScale);
      textBox4.Name = "player leaves";
      Window window20 = (Window) textBox4;
      window20.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window20.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
      window20.SetToolTip("This script will be executed for a player if they leave this world.\n\nTo set the script to execute for this event, drag a script from the list above and drop it here.");
      window1.AddChild((StudioForge.Engine.Core.Node) window20);
      int y12 = y11 + (height3 + num1);
      Window window21 = (Window) new StudioForge.Engine.GUI.TextBox("Player Dies:", x2, y12, width5, height3, textScale);
      window21.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window21);
      StudioForge.Engine.GUI.TextBox textBox5 = new StudioForge.Engine.GUI.TextBox(this.PlayerDiesScriptName, x7, y12, window1.Size.X - x7, height3, textScale);
      textBox5.Name = "player dies";
      Window window22 = (Window) textBox5;
      window22.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window22.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
      window22.SetToolTip("This script will be executed for a player when they die.\n\nTo set the script to execute for this event, drag a script from the list above and drop it here.");
      window1.AddChild((StudioForge.Engine.Core.Node) window22);
      int y13 = y12 + (height3 + num1);
      Window window23 = (Window) new StudioForge.Engine.GUI.TextBox("Player Respawn:", x2, y13, width5, height3, textScale);
      window23.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window23);
      StudioForge.Engine.GUI.TextBox textBox6 = new StudioForge.Engine.GUI.TextBox(this.PlayerRespawnScriptName, x7, y13, window1.Size.X - x7, height3, textScale);
      textBox6.Name = "player respawn";
      Window window24 = (Window) textBox6;
      window24.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window24.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
      window24.SetToolTip("This script will be executed for a player when they respawn from death.\n\nTo set the script to execute for this event, drag a script from the list above and drop it here.");
      window1.AddChild((StudioForge.Engine.Core.Node) window24);
      int y14 = y13 + (height3 + num1);
      Window window25 = (Window) new StudioForge.Engine.GUI.TextBox("Custom Menu:", x2, y14, width5, height3, textScale);
      window25.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window25);
      StudioForge.Engine.GUI.TextBox textBox7 = new StudioForge.Engine.GUI.TextBox(this.CustomMenuScriptName, x7, y14, window1.Size.X - x7, height3, textScale);
      textBox7.Name = "custom menu";
      Window window26 = (Window) textBox7;
      window26.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window26.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
      window26.SetToolTip("This script will be executed for a player if they click the Custom Menu on the Game Menu.\n\nTo set the script to execute for this event, drag a script from the list above and drop it here.");
      window1.AddChild((StudioForge.Engine.Core.Node) window26);
      num4 = y14 + (height3 + num1);
    }

    protected override bool HandleInput()
    {
      if (!InputManager1.IsInputReleasedNew(this.playerIndex, GuiInput.ExitScreen))
        return base.HandleInput();
      this.ExitScreen();
      return true;
    }

    private void ClickScriptMode(object sender, WindowEventArgs e)
    {
      if (this.scriptModeWin.Text.StartsWith("Mode: Edit"))
        this.scriptModeWin.Text = "Mode: Run";
      else if (this.scriptModeWin.Text == "Mode: Run")
      {
        this.scriptModeWin.Text = "Mode: Delete";
        this.scriptModeWin.Colors = (Window.ColorProfile) Colors.ButtonWarnColors;
      }
      else
      {
        this.scriptModeWin.Text = "Mode: Edit";
        this.scriptModeWin.Colors = (Window.ColorProfile) Colors.ButtonColors;
        this.scriptPlayer = (Player) null;
      }
    }

    private void ClickOldScriptsMenu(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ScriptMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
    }

    private void ClickHistoryLog(object sender, WindowEventArgs e)
    {
      List<string> clansThatHaveHistory = this.instance.GetListOfClansThatHaveHistory();
      string[] extraItemsAtTop = new string[clansThatHaveHistory.Count + 1];
      extraItemsAtTop[0] = "System";
      for (int index = 0; index < clansThatHaveHistory.Count; ++index)
        extraItemsAtTop[index + 1] = "Clan: " + clansThatHaveHistory[index];
      this.screenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnHistoryLogGamerSelected), false, (string) null, false, false, extraItemsAtTop), new PlayerIndex?(this.playerIndex));
    }

    private void OnHistoryLogGamerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      Player player = (Player) null;
      string clanName = (string) null;
      if (gamer != null)
        player = gamer.Tag as Player;
      else if (text.StartsWith("Clan: "))
      {
        clanName = text.Substring(6);
        if (clanName == "")
          clanName = (string) null;
      }
      this.screenManager.AddScreen((GameScreen) new HistoryLogScreen(this.instance, player, clanName, (string) null), new PlayerIndex?(this.playerIndex));
    }

    private void ClickNewScript(object sender, WindowEventArgs e)
    {
      this.parentScreen.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, new Script("New Script" + (this.instance.Scripts.Count + 1).ToString())
      {
        Commands = {
          ""
        }
      }, false, (ScriptEditScreen) null, new Action(this.OnScriptSaved)), new PlayerIndex?(this.playerIndex));
    }

    private void ClickNewScriptFromChangeLog(object sender, WindowEventArgs e)
    {
      Script script = new Script("New Script" + (this.instance.Scripts.Count + 1).ToString(), this.player.ChangeLog.Count);
      this.player.ChangeLog.WriteItems(script.Commands);
      script.IsChanged = script.Commands.Count > 0;
      this.parentScreen.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, new Action(this.OnScriptSaved)), new PlayerIndex?(this.playerIndex));
    }

    private void ClickCancelScript(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ScriptCancelListMenuScreen(this.instance, this.player, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnScriptSelectedForCancel)), new PlayerIndex?(this.playerIndex));
    }

    private bool OnScriptSelectedForCancel(MenuEntry scriptItem)
    {
      Script script = this.instance.GetScript((string) scriptItem.Tag + scriptItem.Text);
      if (script != null && !this.instance.CancelScript(script, (Actor) null))
        this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This script could not be found on the execution queue", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
      return true;
    }

    private void ClickRunScriptForAnotherPlayer(object sender, WindowEventArgs e)
    {
      if (this.instance.NetworkManager.AllGamers.Count == 1)
        this.OnPlayerSelected(this.player.Gamer, false, (string) null);
      else
        this.screenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.OnPlayerSelected), true, (string) null, false, false), new PlayerIndex?(this.playerIndex));
    }

    private void OnPlayerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      if (gamer == null)
        return;
      this.scriptPlayer = gamer.Tag as Player;
      if (this.scriptPlayer == null)
        return;
      this.scriptModeWin.Text = "Mode: Run (" + this.scriptPlayer.Gamertag + ")";
      this.scriptModeWin.Colors = (Window.ColorProfile) Colors.ButtonColors;
    }

    private void ValidateRunSingleScriptCommand(ITextInputWindow win)
    {
      if (!win.Text.IsNotEmpty())
        return;
      Script script = new Script("temp", 1);
      Player gamer1 = (Player) null;
      string gamer2 = this.ExtractGamer(win.Text, out gamer1);
      script.Commands.Add(gamer2);
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) gamer1
      };
      this.instance.ExecuteScript(script, data, true);
      this.ExitScreen();
    }

    private string ExtractGamer(string cmd, out Player gamer)
    {
      gamer = this.player;
      foreach (NetworkGamer allGamer in this.instance.NetworkManager.AllGamers)
      {
        if (cmd.StartsWith(allGamer.Gamertag + " "))
        {
          cmd = cmd.Substring(allGamer.Gamertag.Length + 1);
          gamer = allGamer.Tag as Player;
          break;
        }
      }
      return cmd;
    }

    private void ClickSelectExternalEditor(object sender, WindowEventArgs e)
    {
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.AutoUpgradeEnabled = true;
      openFileDialog1.DefaultExt = ".exe";
      openFileDialog1.Filter = "Executable (*.exe)|*.exe";
      openFileDialog1.InitialDirectory = Globals2.ExternalScriptEditor.Substring(Globals2.ExternalScriptEditor.LastIndexOf('\\'));
      openFileDialog1.Multiselect = false;
      openFileDialog1.Title = "Select editor executable";
      OpenFileDialog openFileDialog2 = openFileDialog1;
      if (openFileDialog2.ShowDialog() != DialogResult.OK)
        return;
      Globals2.ExternalScriptEditor = openFileDialog2.FileName;
      StudioForge.Engine.GUI.TextBox child = this.canvas.FindChild("mainContainer").FindChild("editor") as StudioForge.Engine.GUI.TextBox;
      if (child != null)
        child.Text = Globals2.ExternalScriptEditor;
      TextFileParser.WriteString("game.ini", "Editor", Globals2.ExternalScriptEditor);
    }

    private void ClickPathBack(object sender, WindowEventArgs args)
    {
      if (!this.path.IsNotEmpty())
        return;
      int num = this.path.Substring(0, this.path.Length - 1).LastIndexOf('\\');
      if (num < 0)
        this.LoadScriptNames("");
      else
        this.LoadScriptNames(this.path.Substring(0, num + 1));
    }

    private void ClickScriptName(object sender, WindowEventArgs args)
    {
      if (this.scriptNames.Text.EndsWith("\\"))
        this.LoadScriptNames(this.path + this.scriptNames.Text);
      else if (this.scriptModeWin.Text == "Mode: Run")
        this.ExecuteScript(this.path + this.scriptNames.Text);
      else if (this.scriptModeWin.Text == "Mode: Edit" || this.scriptModeWin.Text == "Mode: View")
      {
        Script script = this.instance.GetScript(this.path + this.scriptNames.Text);
        if (script == null)
          return;
        this.parentScreen.ScreenManager.AddScreen((GameScreen) new ScriptEditScreen(this.instance, this.player, script, false, (ScriptEditScreen) null, new Action(this.OnScriptSaved)), new PlayerIndex?(this.playerIndex));
      }
      else
      {
        if (!(this.scriptModeWin.Text == "Mode: Delete"))
          return;
        this.instance.DeleteScript(this.path + this.scriptNames.Text);
        this.LoadScriptNames(this.path);
      }
    }

    private void ExecuteScript(string name)
    {
      ScriptExecuteData data = new ScriptExecuteData()
      {
        Actor = (Actor) this.player
      };
      this.instance.ExecuteScript(name, data, true);
      this.ExitScreen();
    }

    private void DragStartScriptNames(object sender, WindowDragEventArgs e)
    {
      StudioForge.Engine.GUI.TextBox draggingProxy = e.DraggingProxy as StudioForge.Engine.GUI.TextBox;
      if (draggingProxy == null)
        return;
      draggingProxy.Text = this.scriptNames.Text;
    }

    private void DragEndScriptNames(object sender, WindowDragEventArgs e)
    {
      StudioForge.Engine.GUI.TextBox draggingProxy = e.DraggingProxy as StudioForge.Engine.GUI.TextBox;
      if (draggingProxy == null)
        return;
      if (e.Hovered == this.advScriptNames)
      {
        this.TransferScriptsToAdv(this.path + draggingProxy.Text);
      }
      else
      {
        StudioForge.Engine.GUI.TextBox hovered = e.Hovered as StudioForge.Engine.GUI.TextBox;
        if (hovered == null || this.instance.GetScript(this.path + draggingProxy.Text) == null)
          return;
        if (hovered.Name == "player joins")
        {
          hovered.Text = this.path + draggingProxy.Text;
          this.instance.SetEventScript(ScriptEvent.PlayerJoin, hovered.Text);
        }
        else if (hovered.Name == "player leaves")
        {
          hovered.Text = this.path + draggingProxy.Text;
          this.instance.SetEventScript(ScriptEvent.PlayerLeave, hovered.Text);
        }
        else if (hovered.Name == "player dies")
        {
          hovered.Text = this.path + draggingProxy.Text;
          this.instance.SetEventScript(ScriptEvent.PlayerDeath, hovered.Text);
        }
        else if (hovered.Name == "player respawn")
        {
          hovered.Text = this.path + draggingProxy.Text;
          this.instance.SetEventScript(ScriptEvent.PlayerRespawn, hovered.Text);
        }
        else
        {
          if (!(hovered.Name == "custom menu"))
            return;
          hovered.Text = this.path + draggingProxy.Text;
          this.instance.SetEventScript(ScriptEvent.CustomMenu, hovered.Text);
        }
      }
    }

    private void TransferScriptsToAdv(string name)
    {
      if (name.EndsWith("\\"))
      {
        foreach (string sortedScriptName in this.instance.ListOfSortedScriptNames(name))
          this.TransferScriptsToAdv(name + sortedScriptName);
      }
      else
      {
        this.advScriptNames.AddItem(name);
        this.instance.AddAdventureScript(this.instance.GetScript(name));
        this.instance.NetworkManager.SendAdventureScript(name, false);
      }
    }

    private void LoadScriptNames(string path)
    {
      this.path = path;
      this.pathName.Text = "Path: " + path;
      if (path == null)
        path = "";
      this.scriptNames.ClearItems();
      this.scriptNames.AddRange((IEnumerable<string>) this.instance.ListOfSortedScriptNames(path));
    }

    private void OnScriptSaved()
    {
      this.LoadScriptNames(this.path);
    }

    private void ClickAdvScriptName(object sender, WindowEventArgs args)
    {
      if (this.player.IsAdmin)
      {
        this.instance.RemoveAdventureScript(this.instance.GetScript(this.advScriptNames.Text));
        this.advScriptNames.RemoveItem();
      }
      else
        this.ExecuteScript(this.advScriptNames.Text);
    }
  }
}
