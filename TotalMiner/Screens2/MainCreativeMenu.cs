// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainCreativeMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainCreativeMenu : NewGuiMenu2
  {
    private CreativeToolMenu toolMenu;
    private Window toolContainer;
    private Vector2 toolOffset;
    private static CreativeCommand lastCommand;

    public override string Name
    {
      get
      {
        return "Creative";
      }
    }

    public MainCreativeMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
      switch (MainCreativeMenu.lastCommand)
      {
        case CreativeCommand.Clear:
          this.OpenClearToolMenu();
          break;
        case CreativeCommand.Fill:
          this.OpenFillToolMenu();
          break;
        case CreativeCommand.Replace:
          this.OpenReplaceToolMenu();
          break;
        case CreativeCommand.ReplaceTexture:
          this.OpenReplaceTextureToolMenu();
          break;
        case CreativeCommand.Flood:
          this.OpenFloodToolMenu();
          break;
        case CreativeCommand.Line:
          this.OpenLineToolMenu();
          break;
        case CreativeCommand.Sphere:
          this.OpenSphereToolMenu();
          break;
        case CreativeCommand.Path:
          this.OpenPathToolMenu();
          break;
        case CreativeCommand.Wall:
          this.OpenWallToolMenu();
          break;
        case CreativeCommand.Trees:
          this.OpenTreesToolMenu();
          break;
      }
    }

    private void InitMainContainer()
    {
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -100;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int y1 = 110;
      int width1 = 280;
      int num1 = 0;
      int height1 = 34;
      int num2 = 4;
      int num3 = 16;
      int height2 = height1 * num3 + num2 * (num3 - 1);
      float textScale = 0.6f;
      int x1 = winRect.Width / 2 - 300 - (width1 + 1 + num1);
      bool clipboardEquipped = this.player.IsClipboardEquipped;
      Window window1 = new Window((string) null, x1, y1, width1 + 1 + num1, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window1);
      int y2;
      int x2 = y2 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window2;
      this.initialNavigable = window2 = (Window) new TextBox("Remove Markers", x2, y2, width1, height1, textScale);
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickRemoveMarkers);
      window1.AddChild((Node) window2);
      int y3 = y2 + (height1 + num2);
      Window window3 = (Window) new TextBox("Copy to Clipboard", x2, y3, width1, height1, textScale);
      window3.IsEnabled = !clipboardEquipped;
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickCopyToClipboard);
      window1.AddChild((Node) window3);
      int y4 = y3 + (height1 + num2);
      Window window4 = (Window) new TextBox("Load Component", x2, y4, width1, height1, textScale);
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickLoadComponent);
      window1.AddChild((Node) window4);
      int y5 = y4 + (height1 + num2);
      Window window5 = (Window) new TextBox("Save Component", x2, y5, width1, height1, textScale);
      window5.IsEnabled = this.player.HasUnsavedComponentEquipped && this.player.HasPermission(Permissions.Save);
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickSaveComponent);
      window1.AddChild((Node) window5);
      int y6 = y5 + (height1 + num2) + (height1 + num2);
      TextBox textBox1 = new TextBox("Fill", x2, y6, width1, height1, textScale);
      textBox1.Name = "fill";
      Window window6 = (Window) textBox1;
      window6.IsEnabled = !clipboardEquipped;
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.ClickHandler += new Window.WindowHandler(this.ClickFillTool);
      window1.AddChild((Node) window6);
      this.toolOffset = new Vector2((float) (winRect.Width / 2 - 100), (float) y6 + window1.Position.Y);
      int y7 = y6 + (height1 + num2);
      TextBox textBox2 = new TextBox("Clear", x2, y7, width1, height1, textScale);
      textBox2.Name = "clear";
      Window window7 = (Window) textBox2;
      window7.IsEnabled = !clipboardEquipped;
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.ClickHandler += new Window.WindowHandler(this.ClickClearTool);
      window1.AddChild((Node) window7);
      int y8 = y7 + (height1 + num2);
      TextBox textBox3 = new TextBox(clipboardEquipped ? "Replace (Clipboard)" : "Replace", x2, y8, width1, height1, textScale);
      textBox3.Name = "replace";
      Window window8 = (Window) textBox3;
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      if (clipboardEquipped)
        window8.ClickHandler += new Window.WindowHandler(this.ClickReplaceClipboardTool);
      else
        window8.ClickHandler += new Window.WindowHandler(this.ClickReplaceTool);
      window1.AddChild((Node) window8);
      int y9 = y8 + (height1 + num2);
      TextBox textBox4 = new TextBox("Replace Texture", x2, y9, width1, height1, textScale);
      textBox4.Name = "replacetexture";
      Window window9 = (Window) textBox4;
      window9.IsEnabled = !clipboardEquipped;
      window9.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window9.ClickHandler += new Window.WindowHandler(this.ClickReplaceTextureTool);
      window1.AddChild((Node) window9);
      int y10 = y9 + (height1 + num2);
      TextBox textBox5 = new TextBox("Line", x2, y10, width1, height1, textScale);
      textBox5.Name = "line";
      Window window10 = (Window) textBox5;
      window10.IsEnabled = !clipboardEquipped;
      window10.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window10.ClickHandler += new Window.WindowHandler(this.ClickLineTool);
      window1.AddChild((Node) window10);
      int y11 = y10 + (height1 + num2);
      TextBox textBox6 = new TextBox("Wall", x2, y11, width1, height1, textScale);
      textBox6.Name = "wall";
      Window window11 = (Window) textBox6;
      window11.IsEnabled = !clipboardEquipped;
      window11.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window11.ClickHandler += new Window.WindowHandler(this.ClickWallTool);
      window1.AddChild((Node) window11);
      int y12 = y11 + (height1 + num2);
      TextBox textBox7 = new TextBox("Path", x2, y12, width1, height1, textScale);
      textBox7.Name = "path";
      Window window12 = (Window) textBox7;
      window12.IsEnabled = !clipboardEquipped;
      window12.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window12.ClickHandler += new Window.WindowHandler(this.ClickPathTool);
      window1.AddChild((Node) window12);
      int y13 = y12 + (height1 + num2);
      TextBox textBox8 = new TextBox("Sphere", x2, y13, width1, height1, textScale);
      textBox8.Name = "sphere";
      Window window13 = (Window) textBox8;
      window13.IsEnabled = !clipboardEquipped;
      window13.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window13.ClickHandler += new Window.WindowHandler(this.ClickSphereTool);
      window1.AddChild((Node) window13);
      int y14 = y13 + (height1 + num2);
      TextBox textBox9 = new TextBox("Trees", x2, y14, width1, height1, textScale);
      textBox9.Name = "trees";
      Window window14 = (Window) textBox9;
      window14.IsEnabled = !clipboardEquipped;
      window14.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window14.ClickHandler += new Window.WindowHandler(this.ClickTreesTool);
      window1.AddChild((Node) window14);
      int y15 = y14 + (height1 + num2);
      TextBox textBox10 = new TextBox("Flood", x2, y15, width1, height1, textScale);
      textBox10.Name = "flood";
      Window window15 = (Window) textBox10;
      window15.IsEnabled = !clipboardEquipped && this.player.HasPermission(Permissions.Grief);
      window15.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window15.ClickHandler += new Window.WindowHandler(this.ClickFloodTool);
      window1.AddChild((Node) window15);
      int y16 = y15 + (height1 + num2);
      Window window16 = (Window) new TextBox("Abort Active Floods", x2, y16, width1, height1, textScale);
      window16.IsEnabled = this.player.HasActiveFloods && this.player.HasPermission(Permissions.Grief);
      window16.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window16.ClickHandler += new Window.WindowHandler(this.ClickAbortFloodTool);
      window1.AddChild((Node) window16);
      int num4 = y16 + (height1 + num2);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int num5 = 4;
      int y17 = 110;
      int width2 = 170;
      int width3 = 580;
      int height3 = height1 * num5 + num2 * (num5 - 1);
      Window window17 = new Window((string) null, winRect.Width / 2 - 100, y17, width2 + 1 + width3, height3)
      {
        Name = "measureContainer"
      };
      window17.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window17);
      int y18;
      int x3 = y18 = 0;
      GlobalPoint3D min = GlobalPoint3D.Zero;
      GlobalPoint3D max = GlobalPoint3D.Zero;
      GlobalPoint3D xmin = GlobalPoint3D.Zero;
      GlobalPoint3D xmax = GlobalPoint3D.Zero;
      int excludeCount;
      bool flag = this.instance.CreativeModeHelper.MarkerBlockCount(this.player.GamerID, out excludeCount) > 1;
      if (flag)
        this.instance.CreativeModeHelper.GetMinMax(this.player.Gamer.ID, out min, out max, out xmin, out xmax);
      string str1;
      if (!flag)
        str1 = "-----";
      else
        str1 = string.Format("[{0}, {1}, {2}] [{3}, {4}, {5}]", (object) min.X, (object) min.Y, (object) min.Z, (object) max.X, (object) max.Y, (object) max.Z);
      string text1 = str1;
      string str2;
      if (!flag)
        str2 = "-----";
      else
        str2 = string.Format("{0} x {1} x {2} - {3:N0} blocks", (object) (max.X - min.X + 1), (object) (max.Y - min.Y + 1), (object) (max.Z - min.Z + 1), (object) ((max.X - min.X + 1) * (max.Z - min.Z + 1) * (max.Y - min.Y + 1)));
      string text2 = str2;
      string text3 = flag ? string.Format("{0}, {1}, {2}", (object) (float) ((double) (max.X - min.X) * 0.5 + (double) min.X), (object) (float) ((double) (max.Y - min.Y) * 0.5 + (double) min.Y), (object) (float) ((double) (max.Z - min.Z) * 0.5 + (double) min.Z)) : "-----";
      string str3;
      if (excludeCount <= 1)
        str3 = "-----";
      else
        str3 = string.Format("[{0}, {1}, {2}] [{3}, {4}, {5}]", (object) xmin.X, (object) xmin.Y, (object) xmin.Z, (object) xmax.X, (object) xmax.Y, (object) xmax.Z);
      string text4 = str3;
      Window window18 = (Window) new TextBox("Region:", x3, y18, width2, height1, textScale);
      window18.Colors = (Window.ColorProfile) Colors.LabelColors;
      window17.AddChild((Node) window18);
      Window window19 = (Window) new TextBox(text1, x3 + width2 + 1, y18, width3, height1, textScale);
      window19.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window17.AddChild((Node) window19);
      int y19 = y18 + (height1 + num2);
      Window window20 = (Window) new TextBox("Measure:", x3, y19, width2, height1, textScale);
      window20.Colors = (Window.ColorProfile) Colors.LabelColors;
      window17.AddChild((Node) window20);
      Window window21 = (Window) new TextBox(text2, x3 + width2 + 1, y19, width3, height1, textScale);
      window21.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window17.AddChild((Node) window21);
      int y20 = y19 + (height1 + num2);
      Window window22 = (Window) new TextBox("Center:", x3, y20, width2, height1, textScale);
      window22.Colors = (Window.ColorProfile) Colors.LabelColors;
      window17.AddChild((Node) window22);
      Window window23 = (Window) new TextBox(text3, x3 + width2 + 1, y20, width3, height1, textScale);
      window23.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window17.AddChild((Node) window23);
      int y21 = y20 + (height1 + num2);
      Window window24 = (Window) new TextBox("Exclude:", x3, y21, width2, height1, textScale);
      window24.Colors = (Window.ColorProfile) Colors.LabelColors;
      window17.AddChild((Node) window24);
      Window window25 = (Window) new TextBox(text4, x3 + width2 + 1, y21, width3, height1, textScale);
      window25.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window17.AddChild((Node) window25);
      num4 = y21 + (height1 + num2);
    }

    private void CloseCurrentToolMenu()
    {
      if (this.toolContainer == null)
        return;
      this.toolContainer.RemoveSelf();
    }

    protected override bool HandleInput()
    {
      Window window = (Window) null;
      if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.F))
        window = this.canvas.FindChild("maincontainer").FindChild(this.windowManager.CurrentNavigable.Name.Equals("fill", StringComparison.OrdinalIgnoreCase) ? "flood" : "fill");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.C))
        window = this.canvas.FindChild("maincontainer").FindChild("clear");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.R))
        window = this.canvas.FindChild("maincontainer").FindChild(this.windowManager.CurrentNavigable.Name.Equals("replace", StringComparison.OrdinalIgnoreCase) ? "replacetexture" : "replace");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.L))
        window = this.canvas.FindChild("maincontainer").FindChild("line");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.W))
        window = this.canvas.FindChild("maincontainer").FindChild("wall");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.P))
        window = this.canvas.FindChild("maincontainer").FindChild("path");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.S))
        window = this.canvas.FindChild("maincontainer").FindChild("sphere");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.T))
        window = this.canvas.FindChild("maincontainer").FindChild("trees");
      else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.A) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.Y))
      {
        if (this.toolContainer != null)
          this.windowManager.SetNavigable(this.toolContainer.FindChild("apply"));
      }
      else if ((InputManager.IsKeyReleasedNew(this.playerIndex, Keys.A) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.Y)) && this.toolContainer != null)
        window = this.toolContainer.FindChild("apply");
      if (window == null)
        return base.HandleInput();
      this.windowManager.SetNavigable(window);
      this.windowManager.RaiseClickHandler(window);
      return true;
    }

    private void ClickRemoveMarkers(object sender, WindowEventArgs e)
    {
      this.instance.CreativeModeHelper.RemoveMarkers(this.player.GamerID, true);
      this.ExitScreen();
    }

    private void ClickCopyToClipboard(object sender, WindowEventArgs e)
    {
      this.instance.CreativeModeHelper.CopyToClipboard(this.player.GamerID, Map.CopyAccess.Restricted);
      this.ExitScreen();
    }

    private void ClickLoadComponent(object sender, WindowEventArgs e)
    {
      if (this.instance.TotalClipboardsSizeInBytes >= (long) this.instance.TotalClipboardsSizeCapacity)
      {
        this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Component RAM capacity reached.\nDiscard some clipboards before loading a new component.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
      }
      else
      {
        LoadComponentPackScreen componentPackScreen = new LoadComponentPackScreen(this.instance, this.player, false, true);
        componentPackScreen.IsPopup = true;
        this.screenManager.AddScreen((GameScreen) componentPackScreen, new PlayerIndex?(this.playerIndex));
        this.ExitScreen();
      }
    }

    private void ClickSaveComponent(object sender, WindowEventArgs e)
    {
      LoadComponentPackScreen componentPackScreen = new LoadComponentPackScreen(this.instance, this.player, true, false);
      componentPackScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) componentPackScreen, new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickFillTool(object sender, WindowEventArgs e)
    {
      this.OpenFillToolMenu();
    }

    private void OpenFillToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolFillMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Fill;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickClearTool(object sender, WindowEventArgs e)
    {
      this.OpenClearToolMenu();
    }

    private void OpenClearToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolClearMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Clear;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickLineTool(object sender, WindowEventArgs e)
    {
      this.OpenLineToolMenu();
    }

    private void OpenLineToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolLineMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Line;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickReplaceTool(object sender, WindowEventArgs e)
    {
      this.OpenReplaceToolMenu();
    }

    private void OpenReplaceToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolReplaceMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Replace;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickReplaceClipboardTool(object sender, WindowEventArgs e)
    {
      this.OpenReplaceClipboardToolMenu();
    }

    private void OpenReplaceClipboardToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolReplaceClipboardMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Replace;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickReplaceTextureTool(object sender, WindowEventArgs e)
    {
      this.OpenReplaceTextureToolMenu();
    }

    private void OpenReplaceTextureToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolReplaceTextureMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.ReplaceTexture;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickWallTool(object sender, WindowEventArgs e)
    {
      this.OpenWallToolMenu();
    }

    private void OpenWallToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolWallMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Wall;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickPathTool(object sender, WindowEventArgs e)
    {
      this.OpenPathToolMenu();
    }

    private void OpenPathToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolPathMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Path;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickSphereTool(object sender, WindowEventArgs e)
    {
      this.OpenSphereToolMenu();
    }

    private void OpenSphereToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolSphereMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Sphere;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickTreesTool(object sender, WindowEventArgs e)
    {
      this.OpenTreesToolMenu();
    }

    private void OpenTreesToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolTreesMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Trees;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickFloodTool(object sender, WindowEventArgs e)
    {
      this.OpenFloodToolMenu();
    }

    private void OpenFloodToolMenu()
    {
      this.CloseCurrentToolMenu();
      this.toolMenu = (CreativeToolMenu) new CreativeToolFloodMenu(this.parentScreen, this.instance, this.player, new Action(((NewGuiMenu) this).ExitScreen));
      this.toolContainer = this.toolMenu.InitWindows();
      this.toolContainer.Position = this.toolOffset;
      MainCreativeMenu.lastCommand = CreativeCommand.Flood;
      this.canvas.AddChild((Node) this.toolContainer);
    }

    private void ClickAbortFloodTool(object sender, WindowEventArgs e)
    {
    }
  }
}
