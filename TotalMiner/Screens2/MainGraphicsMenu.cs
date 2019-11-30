// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainGraphicsMenu
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
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainGraphicsMenu : NewGuiMenu2
  {
    private GameSettings gameSettings;
    private SaveMapHead header;
    private bool isFullScreen;
    private bool initIsFullScreen;
    private int initDisplayWidth;
    private int initDisplayHeight;
    private Window deviceInfoWin;

    public override string Name
    {
      get
      {
        return "Graphics";
      }
    }

    private string ResolutionText
    {
      get
      {
        Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
        return viewport.Width.ToString() + " x " + viewport.Height.ToString();
      }
    }

    private string BackColorText
    {
      get
      {
        GamertagData orAddGamertagData = Globals2.GamertagData.GetOrAddGamertagData(this.player.PlayerIndex);
        if (orAddGamertagData.Settings.PlayerSettings.BackColor == Microsoft.Xna.Framework.Color.LightBlue * 0.8f)
          return "Default";
        if (orAddGamertagData.Settings.PlayerSettings.BackColor == new Microsoft.Xna.Framework.Color(33, 128, 31) * 0.8f)
          return "Green";
        if (orAddGamertagData.Settings.PlayerSettings.BackColor == new Microsoft.Xna.Framework.Color(143, 101, 69) * 0.8f)
          return "Brown";
        return string.Format("{0}, {1}, {2}, {3}", (object) orAddGamertagData.Settings.PlayerSettings.BackColor.R, (object) orAddGamertagData.Settings.PlayerSettings.BackColor.G, (object) orAddGamertagData.Settings.PlayerSettings.BackColor.B, (object) orAddGamertagData.Settings.PlayerSettings.BackColor.A);
      }
    }

    private string LeafMeshText
    {
      get
      {
        switch (this.gameSettings.LeafMesh)
        {
          case LeafMeshType.Above:
            return "Above";
          case LeafMeshType.Below:
            return "Below";
          case LeafMeshType.AboveAndBelow:
            return "Above and Below";
          case LeafMeshType.Sides:
            return "Sides";
          case LeafMeshType.SidesAndBelow:
            return "Sides and Below";
          case LeafMeshType.All:
            return "All";
          default:
            return "None";
        }
      }
    }

    public MainGraphicsMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
      this.gameSettings = Globals2.GameSettings;
      this.header = Globals2.GameProperties.SaveGame.Header;
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      this.initDisplayWidth = viewport.Width;
      this.initDisplayHeight = viewport.Height;
      this.initIsFullScreen = this.isFullScreen = Services.GetService<GraphicsDeviceManager>().IsFullScreen;
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    public override void OnParentExit()
    {
      base.OnParentExit();
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      if (viewport.Width != this.initDisplayWidth)
        TextFileParser.WriteInt("game.ini", "DisplayWidth", viewport.Width);
      if (viewport.Height != this.initDisplayHeight)
        TextFileParser.WriteInt("game.ini", "DisplayHeight", viewport.Height);
      if (this.isFullScreen == this.initIsFullScreen)
        return;
      TextFileParser.WriteBool("game.ini", "FullScreen", this.isFullScreen);
    }

    private void InitMainContainer()
    {
      Microsoft.Xna.Framework.Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -100;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int y1 = 110;
      int width1 = 250;
      int width2 = 250;
      int height1 = 34;
      int num1 = 4;
      int num2 = 14;
      int height2 = height1 * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      Window window1 = new Window((string) null, winRect.Width / 2 - 100 - (width1 + 1 + width2), y1, width1 + 1 + width2, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int y2;
      int x1 = y2 = 0;
      StudioForge.Engine.GUI.TextBox textBox1;
      Window window2 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Texture Pack:", x1, y2, width1, height1, textScale));
      window2.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      int width3 = winRect.Width / 2 + 100 + 300 - (winRect.Width / 2 - 100 - (width1 + 1 + width2) + width1 + 1);
      DropDown dropDown1;
      DropDown dropDown2 = dropDown1 = new DropDown(Globals2.GameProperties.SaveGame.Header.TexturePack, x1 + width1 + 1, y2, width3, height1, 600, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) dropDown1;
      Window window3 = (Window) dropDown1;
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateTexturePacks);
      ((ITextInputWindow) dropDown2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateTexturePack);
      window1.AddChild((StudioForge.Engine.Core.Node) window3);
      int y3 = y2 + (height1 + num1);
      Window window4 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Screen Resolution:", x1, y3, width1, height1, textScale));
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      DropDown dropDown3;
      DropDown dropDown4 = dropDown3 = new DropDown(this.ResolutionText, x1 + width1 + 1, y3, width2, height1, 400, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) dropDown3;
      Window window5 = (Window) dropDown3;
      this.initialNavigable = (Window) dropDown3;
      window5.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      dropDown4.PopulateList = new Action<Window, List<string>, string>(this.PopulateResolutions);
      ((ITextInputWindow) dropDown4).OnValidateInput = new Action<ITextInputWindow>(this.ValidateResolution);
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      int y4 = y3 + (height1 + num1);
      Window window6 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Full Screen:", x1, y4, width1, height1, textScale));
      window6.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      StudioForge.Engine.GUI.TextBox textBox2;
      Window window7 = (Window) (textBox2 = new StudioForge.Engine.GUI.TextBox(this.OnOff(this.isFullScreen), x1 + width1 + 1, y4, width2, height1, textScale));
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox2.ClickHandler += new Window.WindowHandler(this.ClickFullScreen);
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      int y5 = y4 + (height1 + num1);
      Window window8 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Background Color:", x1, y5, width1, height1, textScale));
      window8.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      DropDown dropDown5;
      DropDown dropDown6 = dropDown5 = new DropDown(this.BackColorText, x1 + width1 + 1, y5, width2, height1, 400, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) dropDown5;
      Window window9 = (Window) dropDown5;
      this.initialNavigable = (Window) dropDown5;
      window9.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      dropDown6.GetNewInputHandler = (GetTextInputHander) null;
      dropDown6.PopulateList = new Action<Window, List<string>, string>(this.PopulateBackColors);
      ((ITextInputWindow) dropDown6).OnValidateInput = new Action<ITextInputWindow>(this.ValidateBackColor);
      window9.SetToolTip("Select a different background color.\n\nSelect Random to generate a different color each time.\n\nEnter the color directly in R,G,B,A format, valid values 0-255.\n\nExample: 91,60,37,210 is a translucent brown.");
      window1.AddChild((StudioForge.Engine.Core.Node) window9);
      int y6 = y5 + (height1 + num1);
      Window window10 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Shader Detail:", x1, y6, width1, height1, textScale));
      window10.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window10);
      StudioForge.Engine.GUI.TextBox textBox3;
      Window window11 = (Window) (textBox3 = new StudioForge.Engine.GUI.TextBox(this.gameSettings.ShaderDetail.ToString(), x1 + width1 + 1, y6, width2, height1, textScale));
      window11.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox3.ClickHandler += new Window.WindowHandler(this.ClickShaderDetail);
      window1.AddChild((StudioForge.Engine.Core.Node) window11);
      int y7 = y6 + (height1 + num1);
      Window window12 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("View Distance:", x1, y7, width1, height1, textScale));
      window12.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window12);
      Slider slider1;
      Slider slider2 = slider1 = new Slider(x1 + width1 + 1, y7, width2, height1, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) slider1;
      Window window13 = (Window) slider1;
      window13.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider2.SetValue(this.gameSettings.ViewDistance);
      slider2.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderViewDistance);
      window1.AddChild((StudioForge.Engine.Core.Node) window13);
      int y8 = y7 + (height1 + num1);
      Window window14 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Field Of View:", x1, y8, width1, height1, textScale));
      window14.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window14);
      Slider slider3;
      Slider slider4 = slider3 = new Slider(x1 + width1 + 1, y8, width2, height1, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) slider3;
      Window window15 = (Window) slider3;
      window15.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider4.SetValue(this.player.Settings.FOVNormalized);
      slider4.Text = ((int) this.player.FOV).ToString();
      slider4.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderFOV);
      window1.AddChild((StudioForge.Engine.Core.Node) window15);
      int y9 = y8 + (height1 + num1);
      Window window16 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Texture Smoothing:", x1, y9, width1, height1, textScale));
      window16.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window16);
      Slider slider5;
      Slider slider6 = slider5 = new Slider(x1 + width1 + 1, y9, width2, height1, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) slider5;
      Window window17 = (Window) slider5;
      window17.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider6.SetValue(this.gameSettings.TextureSmoothing);
      slider6.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderTextureSmoothing);
      window1.AddChild((StudioForge.Engine.Core.Node) window17);
      int y10 = y9 + (height1 + num1);
      Window window18 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Texture Smoothing:", x1, y10, width1, height1, textScale));
      window18.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window18);
      StudioForge.Engine.GUI.TextBox textBox4;
      Window window19 = (Window) (textBox4 = new StudioForge.Engine.GUI.TextBox(this.OnOff(this.gameSettings.UseMipMaps), x1 + width1 + 1, y10, width2, height1, textScale));
      window19.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox4.ClickHandler += new Window.WindowHandler(this.ClickTextureSmoothing);
      window1.AddChild((StudioForge.Engine.Core.Node) window19);
      int y11 = y10 + (height1 + num1);
      Window window20 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Show Clouds:", x1, y11, width1, height1, textScale));
      window20.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window20);
      StudioForge.Engine.GUI.TextBox textBox5;
      Window window21 = (Window) (textBox5 = new StudioForge.Engine.GUI.TextBox(this.OnOff(this.gameSettings.ViewClouds), x1 + width1 + 1, y11, width2, height1, textScale));
      window21.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox5.ClickHandler += new Window.WindowHandler(this.ClickShowClouds);
      window1.AddChild((StudioForge.Engine.Core.Node) window21);
      int y12 = y11 + (height1 + num1);
      Window window22 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Show Sounds:", x1, y12, width1, height1, textScale));
      window22.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window22);
      StudioForge.Engine.GUI.TextBox textBox6;
      Window window23 = (Window) (textBox6 = new StudioForge.Engine.GUI.TextBox(this.OnOff(Globals2.GameSettings.ViewSounds), x1 + width1 + 1, y12, width2, height1, textScale));
      window23.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox6.ClickHandler += new Window.WindowHandler(this.ClickShowShounds);
      window1.AddChild((StudioForge.Engine.Core.Node) window23);
      int y13 = y12 + (height1 + num1);
      Window window24 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Flora Animation:", x1, y13, width1, height1, textScale));
      window24.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window24);
      StudioForge.Engine.GUI.TextBox textBox7;
      Window window25 = (Window) (textBox7 = new StudioForge.Engine.GUI.TextBox(this.OnOff(this.gameSettings.FloraAnimation), x1 + width1 + 1, y13, width2, height1, textScale));
      window25.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox7.ClickHandler += new Window.WindowHandler(this.ClickFloraAnimation);
      window1.AddChild((StudioForge.Engine.Core.Node) window25);
      int y14 = y13 + (height1 + num1);
      Window window26 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Leaf Mesh:", x1, y14, width1, height1, textScale));
      window26.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window26);
      DropDown dropDown7;
      DropDown dropDown8 = dropDown7 = new DropDown(this.LeafMeshText, x1 + width1 + 1, y14, width2, height1, 400, textScale);
      textBox1 = (StudioForge.Engine.GUI.TextBox) dropDown7;
      Window window27 = (Window) dropDown7;
      window27.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      dropDown8.PopulateList = new Action<Window, List<string>, string>(this.PopulateLeafMesh);
      ((ITextInputWindow) dropDown8).OnValidateInput = new Action<ITextInputWindow>(this.ValidateLeafMesh);
      window27.SetToolTip("Defines how leaves are rendered.\nGenerally 'Below' is good enough.\nNote: using Sides (or All) can greatly increase the amount of RAM needed for a worlds mesh. Easily an extra 100MB if there are many trees in the world, so only use that option if you are not pushing the bounds of available RAM. Conversly if you are pushing the bounds of RAM and have a lot of trees, consider using the None option to free up some RAM.");
      window1.AddChild((StudioForge.Engine.Core.Node) window27);
      int y15 = y14 + (height1 + num1);
      Window window28 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Tooltips:", x1, y15, width1, height1, textScale));
      window28.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window28);
      StudioForge.Engine.GUI.TextBox textBox8;
      Window window29 = (Window) (textBox8 = new StudioForge.Engine.GUI.TextBox(this.OnOff(this.gameSettings.ToolTips), x1 + width1 + 1, y15, width2, height1, textScale));
      window29.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox8.ClickHandler += new Window.WindowHandler(this.ClickTooltips);
      window1.AddChild((StudioForge.Engine.Core.Node) window29);
      int num3 = y15 + (height1 + num1);
      int num4 = 5;
      int num5 = 110;
      int width4 = 300;
      int height3 = height1 * num4 + num1 * (num4 - 1);
      int x2 = winRect.Width / 2 + 100;
      int y16 = num5 + (height1 + num1) + (height1 + num1);
      int x3 = x2;
      int num6 = y16;
      Window window30 = new Window((string) null, x2, y16, width4, height3)
      {
        Name = "mainContainer2"
      };
      window30.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window30);
      int y17;
      int x4 = y17 = 0;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window31 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Adjust HUD Position", x4, y17, width4, height1, textScale));
      window31.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window31.ClickHandler += new Window.WindowHandler(this.ClickAdjustHUD);
      window30.AddChild((StudioForge.Engine.Core.Node) window31);
      int y18 = y17 + (height1 + num1);
      Window window32 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Rebuild Local Light", x4, y18, width4, height1, textScale));
      window32.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window32.IsEnabled = this.player.IsAdmin;
      window32.ClickHandler += new Window.WindowHandler(this.ClickRebuildLight);
      window30.AddChild((StudioForge.Engine.Core.Node) window32);
      int y19 = y18 + (height1 + num1);
      Window window33 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Clear All Pickups", x4, y19, width4, height1, textScale));
      window33.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window33.IsEnabled = this.player.IsAdmin;
      window33.ClickHandler += new Window.WindowHandler(this.ClickClearParticles);
      window30.AddChild((StudioForge.Engine.Core.Node) window33);
      int y20 = y19 + (height1 + num1);
      Window window34 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("View Clan Banners", x4, y20, width4, height1, textScale));
      window34.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window34.ClickHandler += new Window.WindowHandler(this.ClickClanBanners);
      window30.AddChild((StudioForge.Engine.Core.Node) window34);
      int y21 = y20 + (height1 + num1);
      Window window35 = (Window) (textBox1 = new StudioForge.Engine.GUI.TextBox("Graphics Device Info", x4, y21, width4, height1, textScale));
      window35.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window35.ClickHandler += new Window.WindowHandler(this.ClickDeviceInfo);
      window30.AddChild((StudioForge.Engine.Core.Node) window35);
      num3 = y21 + (height1 + num1);
      StudioForge.Engine.GUI.ListBox listBox1 = new StudioForge.Engine.GUI.ListBox((string) null, x3, num6 + window30.Size.Y + 4, 480 + 8, 24 * 12 + 8);
      listBox1.Name = "deviceInfo";
      listBox1.BorderThickness = 1;
      StudioForge.Engine.GUI.ListBox listBox2;
      StudioForge.Engine.GUI.ListBox listBox3 = listBox2 = listBox1;
      Window window36 = (Window) listBox2;
      this.deviceInfoWin = (Window) listBox2;
      window36.AddFlags(Window.WinFlags.DynamicHeight);
      window36.Colors = (Window.ColorProfile) Colors.ListBoxColors;
      listBox3.TextScale = 0.5f;
      listBox3.MinTextEdge = (ushort) 6;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window36);
      this.deviceInfoWin.IsVisible = false;
      listBox3.AddItem(" Device: " + this.game.GraphicsDevice.Adapter.DeviceName);
      listBox3.AddItem(" Desc: " + this.game.GraphicsDevice.Adapter.Description);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
    }

    private void ClickTexturePack(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new TexturePackMenuScreen(this.instance, this.player), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ValidateTexturePack(ITextInputWindow win)
    {
      if (!(win.Text != Globals2.GameProperties.SaveGame.Header.TexturePack))
        return;
      this.instance.LoadTexturePack(win.Text, true, false, true);
    }

    private void PopulateTexturePacks(Window win, List<string> list, string input)
    {
      list.Clear();
      list.Add("Original");
      list.Add("Original Remade");
      list.Add("Original HD");
      list.Add("Original Autumn HD");
      list.Add("Original Winter HD");
      list.Add("Original Spring HD");
      list.Add("Steampunk HD");
      list.Add("Rupture HD by Gr1mT1m3Z");
      foreach (string file in TitleFileSystem.GetFiles(CoreGlobals.Content.RootDirectory + "\\Textures\\", "tp_*.png"))
      {
        int num1 = file.ToLower().IndexOf("tp_");
        int num2 = file.ToLower().IndexOf(".png");
        list.Add(file.Substring(num1 + 3, num2 - (num1 + 3)));
      }
      foreach (string file in TitleFileSystem.GetFiles(CoreGlobals.Content.RootDirectory + "\\Textures\\", "tp_*.xnb"))
      {
        int num1 = file.ToLower().IndexOf("tp_");
        int num2 = file.ToLower().IndexOf(".xnb");
        string str1 = file.Substring(num1 + 3, num2 - (num1 + 3));
        bool flag = false;
        foreach (string str2 in list)
        {
          if (str2.Equals(str1, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
        if (!flag && (!str1.StartsWith("AvatarPalette") || this.player != null && this.player.IsGod))
          list.Add(str1);
      }
    }

    private void PopulateResolutions(Window win, List<string> list, string input)
    {
      list.Clear();
      foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
      {
        if (supportedDisplayMode.Width >= 1280 && supportedDisplayMode.Format == SurfaceFormat.Color)
          list.Add(supportedDisplayMode.Width.ToString() + " x " + supportedDisplayMode.Height.ToString());
      }
    }

    private void ValidateResolution(ITextInputWindow win)
    {
      int num = win.Text.IndexOf('x');
      int result1;
      int.TryParse(win.Text.Substring(0, num - 1), out result1);
      int result2;
      int.TryParse(win.Text.Substring(num + 2), out result2);
      if (!this.ChangeWindowSize(result1, result2))
        return;
      this.ExitScreen();
    }

    private void PopulateBackColors(Window win, List<string> list, string input)
    {
      list.Clear();
      list.Add("Random");
      list.Add("Default");
      list.Add("Green");
      list.Add("Brown");
    }

    private void ValidateBackColor(ITextInputWindow win)
    {
      GamertagData orAddGamertagData = Globals2.GamertagData.GetOrAddGamertagData(this.player.PlayerIndex);
      switch (win.Text)
      {
        case "Random":
          orAddGamertagData.Settings.PlayerSettings.BackColor.R = (byte) (this.instance.Random.Next(190) + 10);
          orAddGamertagData.Settings.PlayerSettings.BackColor.G = (byte) (this.instance.Random.Next(190) + 10);
          orAddGamertagData.Settings.PlayerSettings.BackColor.B = (byte) (this.instance.Random.Next(190) + 10);
          orAddGamertagData.Settings.PlayerSettings.BackColor.A = byte.MaxValue;
          orAddGamertagData.Settings.PlayerSettings.BackColor *= 0.8f;
          break;
        case "Green":
          orAddGamertagData.Settings.PlayerSettings.BackColor = new Microsoft.Xna.Framework.Color(33, 128, 31) * 0.8f;
          break;
        case "Brown":
          orAddGamertagData.Settings.PlayerSettings.BackColor = new Microsoft.Xna.Framework.Color(143, 101, 69) * 0.8f;
          break;
        case "Default":
          orAddGamertagData.Settings.PlayerSettings.BackColor = Microsoft.Xna.Framework.Color.LightBlue * 0.8f;
          break;
        default:
          Microsoft.Xna.Framework.Color? color4FromToken = new Parser().GetColor4FromToken(win.Text);
          orAddGamertagData.Settings.PlayerSettings.BackColor = color4FromToken.HasValue ? color4FromToken.Value : this.baseWindow.Texture.TintColor;
          break;
      }
      this.SetTintColor(orAddGamertagData.Settings.PlayerSettings.BackColor);
    }

    private void PopulateDisplayModes(Window win, List<string> list, string input)
    {
      list.Clear();
      list.Add("Window");
      list.Add("Borderless Window");
      list.Add("Fullscreen");
    }

    private void ValidateDisplayMode(ITextInputWindow win)
    {
      switch (win.Text)
      {
        case "Window":
          Form form1 = Control.FromHandle(TotalMinerGame.Instance.Window.Handle).FindForm();
          if (form1 == null)
            break;
          form1.WindowState = FormWindowState.Normal;
          form1.FormBorderStyle = FormBorderStyle.Fixed3D;
          break;
        case "Borderless Window":
          Form form2 = Control.FromHandle(TotalMinerGame.Instance.Window.Handle).FindForm();
          if (form2 == null)
            break;
          form2.WindowState = FormWindowState.Maximized;
          form2.FormBorderStyle = FormBorderStyle.None;
          System.Drawing.Rectangle virtualScreen = SystemInformation.VirtualScreen;
          if (!this.ChangeWindowSize(virtualScreen.Width, virtualScreen.Height))
            break;
          this.ExitScreen();
          break;
        case "Fullscreen":
          GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
          if (service == null)
            break;
          service.ToggleFullScreen();
          this.isFullScreen = service.IsFullScreen;
          this.instance.MapRenderer.SignsChanged(true);
          break;
      }
    }

    private bool ChangeWindowSize(int width, int height)
    {
      GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
      if (service != null)
      {
        Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
        if (width != viewport.Width || height != viewport.Height)
        {
          service.PreferredBackBufferWidth = width;
          service.PreferredBackBufferHeight = height;
          service.ApplyChanges();
          foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
            localEnabledPlayer.Viewport = this.screenManager.GraphicsDevice.Viewport;
          GraphicStatics.DefaultViewport = CoreGlobals.GraphicsDevice.Viewport;
          GraphicStatics.SetHUDPos(GraphicStatics.HUDPos().X, GraphicStatics.HUDPos().Y);
          this.screenManager.SetViewport(new PlayerIndex?(), CoreGlobals.GraphicsDevice.Viewport);
          StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.SetupViewports();
          StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance.SetupRenderTargets();
          this.instance.MapRenderer.SignsChanged(true);
          MapTopViewScreen.DisposeRT(this.instance, true);
          return true;
        }
      }
      return false;
    }

    private void ClickFullScreen(object sender, WindowEventArgs e)
    {
      GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
      if (service == null)
        return;
      service.ToggleFullScreen();
      this.isFullScreen = service.IsFullScreen;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.isFullScreen);
      this.instance.MapRenderer.SignsChanged(true);
    }

    private void ClickShaderDetail(object sender, WindowEventArgs e)
    {
      this.gameSettings.ShaderDetail = this.gameSettings.ShaderDetail == ShaderDetail.Low ? ShaderDetail.High : ShaderDetail.Low;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.gameSettings.ShaderDetail.ToString();
    }

    private void DragSliderViewDistance(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      if (num < 10)
        num = 10;
      this.gameSettings.ViewDistance = (float) num / 100f;
      ((StudioForge.Engine.GUI.TextBox) sender).Text = num.ToString() + "%";
      this.instance.OnViewDistanceChanged();
    }

    private void DragSliderFOV(object sender, WindowDragEventArgs e)
    {
      this.player.Settings.FOVNormalized = (float) (int) ((double) (float) e.Tag * 100.0) / 100f;
      this.player.FOVNormalized = this.player.Settings.FOVNormalized;
      ((StudioForge.Engine.GUI.TextBox) sender).Text = ((int) this.player.FOV).ToString();
    }

    private void DragSliderTextureSmoothing(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      this.gameSettings.TextureSmoothing = (float) num / 100f;
      ((StudioForge.Engine.GUI.TextBox) sender).Text = num.ToString() + "%";
    }

    private void ClickTextureSmoothing(object sender, WindowEventArgs e)
    {
      this.gameSettings.UseMipMaps = !this.gameSettings.UseMipMaps;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.gameSettings.UseMipMaps);
    }

    private void ClickShowClouds(object sender, WindowEventArgs e)
    {
      this.gameSettings.ViewClouds = !this.gameSettings.ViewClouds;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.gameSettings.ViewClouds);
    }

    private void ClickShowShounds(object sender, WindowEventArgs e)
    {
      this.gameSettings.ViewSounds = !this.gameSettings.ViewSounds;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.gameSettings.ViewSounds);
    }

    private void ClickFloraAnimation(object sender, WindowEventArgs e)
    {
      this.gameSettings.FloraAnimation = !this.gameSettings.FloraAnimation;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.gameSettings.FloraAnimation);
    }

    private void ClickTooltips(object sender, WindowEventArgs e)
    {
      this.gameSettings.ToolTips = !this.gameSettings.ToolTips;
      ((StudioForge.Engine.GUI.TextBox) e.Window).Text = this.OnOff(this.gameSettings.ToolTips);
    }

    private void ClickAdjustHUD(object sender, WindowEventArgs e)
    {
      this.ExitScreen();
      this.screenManager.AddScreen((GameScreen) new HUDAdjustScreen(), new PlayerIndex?(this.playerIndex));
    }

    private void ClickRebuildLight(object sender, WindowEventArgs e)
    {
      this.instance.RebuildLocalLight(this.player);
      this.ExitScreen();
    }

    private void ClickClearParticles(object sender, WindowEventArgs e)
    {
      this.instance.ClearAllParticles(true);
      this.ExitScreen();
    }

    private void ClickClanBanners(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ViewClanBannerScreen(), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void ClickDeviceInfo(object sender, WindowEventArgs e)
    {
      this.deviceInfoWin.IsVisible = !this.deviceInfoWin.IsVisible;
    }

    private void PopulateLeafMesh(Window win, List<string> list, string input)
    {
      list.Clear();
      list.Add("None");
      list.Add("Above");
      list.Add("Below");
      list.Add("Above and Below");
      list.Add("Sides");
      list.Add("Sides and Below");
      list.Add("All");
    }

    private void ValidateLeafMesh(ITextInputWindow win)
    {
      switch (win.Text)
      {
        case "Above":
          this.gameSettings.LeafMesh = LeafMeshType.Above;
          break;
        case "Below":
          this.gameSettings.LeafMesh = LeafMeshType.Below;
          break;
        case "Above and Below":
          this.gameSettings.LeafMesh = LeafMeshType.AboveAndBelow;
          break;
        case "Sides":
          this.gameSettings.LeafMesh = LeafMeshType.Sides;
          break;
        case "Sides and Below":
          this.gameSettings.LeafMesh = LeafMeshType.SidesAndBelow;
          break;
        case "All":
          this.gameSettings.LeafMesh = LeafMeshType.All;
          break;
        default:
          this.gameSettings.LeafMesh = LeafMeshType.None;
          break;
      }
    }
  }
}
