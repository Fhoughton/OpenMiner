// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.PauseMenuScreen2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens2
{
  internal class PauseMenuScreen2 : MinerToolScreen
  {
    public const int GameTab = 0;
    public const int CharacterTab = 1;
    public const int CreativeTab = 2;
    public const int ScriptsTab = 3;
    public const int BehavioursTab = 4;
    public const int OptionsTab = 5;
    public const int GraphicsTab = 6;
    public const int OtherTab = 7;
    private static int lastTabID;
    private GameInstance instance;
    private NewGuiMenu[] menu;
    private TextBox[] tab;
    private Texture2D backTex;
    private Window tabHighLight;
    private NewGuiMenu otherTab;
    private Stack<NewGuiMenu> otherTabStack;
    private Stack<int> otherTabPrevIDStack;
    private int tabID;

    public Texture2D BackTexture
    {
      get
      {
        return this.backTex;
      }
    }

    public Rectangle ScreenRect
    {
      get
      {
        return this.screenRect;
      }
    }

    public NewGuiMenu CurrentMenu
    {
      get
      {
        if (this.tabID >= this.menu.Length || this.menu[this.tabID] == null)
          return (NewGuiMenu) null;
        return this.menu[this.tabID];
      }
    }

    public PauseMenuScreen2(GameInstance instance, Player player)
      : this(instance, player, (NewGuiMenu) null)
    {
    }

    public PauseMenuScreen2(GameInstance instance, Player player, int tabID)
      : base(player)
    {
      this.instance = instance;
      InputManager.PushVirtualMouse();
      this.tabID = PauseMenuScreen2.lastTabID = tabID;
      instance.PauseGame();
    }

    public PauseMenuScreen2(GameInstance instance, Player player, NewGuiMenu otherTab)
      : base(player)
    {
      this.instance = instance;
      this.otherTab = otherTab;
      InputManager.PushVirtualMouse();
      this.tabID = PauseMenuScreen2.lastTabID;
      instance.PauseGame();
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.backTex = this.content.Load<Texture2D>("Textures\\NewGuiBkgd");
      Viewport viewport = this.GraphicsDevice.Viewport;
      Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
      this.screenRect.X = this.screenRect.Y = 0;
      this.screenRect.Width = viewport.Width;
      this.screenRect.Height = viewport.Height;
      this.InitScreen();
      int x = 10;
      int y = 10;
      VirtualKeyboardCarousal.DefPosition = new Point(x, y);
      VirtualKeyboardCarousal.DefParent = this.windowManager.Root.FindChild("topPane");
      VirtualKeyboardCarousal.DefSize = new Point(VirtualKeyboardCarousal.DefParent.Size.X - x * 2, VirtualKeyboardCarousal.DefParent.Size.Y - y * 2);
      VirtualKeyboardCarousal.DefColors = Colors.PauseMenuKeyboard;
    }

    protected override void OnScreenRemovedCore()
    {
      foreach (NewGuiMenu newGuiMenu in this.menu)
        newGuiMenu?.OnParentExit();
      base.OnScreenRemovedCore();
      PauseMenuScreen2.lastTabID = this.tabID;
      if (this.player != null && this == this.player.PauseMenuScreen)
        this.player.PauseMenuScreen = (GameScreen) null;
      InputManager.PopVirtualMouse();
    }

    private void InitScreen()
    {
      InputManager.SetMousePos(548, 118);
      this.menu = new NewGuiMenu[8];
      this.tab = new TextBox[8];
      Window window1 = new Window((string) null, 0, 0, this.ScreenRect.Width, 70)
      {
        Name = "topPane"
      };
      window1.Colors = Colors.PauseMenuTop;
      this.windowManager.Root.AddChild((StudioForge.Engine.Core.Node) window1);
      int x1 = 20;
      int y = 18;
      int height = 39;
      int width = 144;
      int num1 = 12;
      float textScale = 0.6f;
      this.tabHighLight = new Window((string) null, 0, height - 5, width, 5);
      this.tabHighLight.Colors = Colors.PauseMenuTabHighlight;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      TextBox[] tab1 = this.tab;
      TextBox textBox1;
      TextBox textBox2 = textBox1 = new TextBox("Game", x1, y, width, height, textScale);
      TextBox textBox3 = textBox1;
      tab1[0] = textBox1;
      Window window2 = (Window) textBox3;
      textBox2.Tag = (object) 0;
      window2.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox2.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      int x2 = x1 + (width + num1);
      TextBox[] tab2 = this.tab;
      TextBox textBox4;
      TextBox textBox5 = textBox4 = new TextBox("Character", x2, y, width, height, textScale);
      TextBox textBox6 = textBox4;
      tab2[1] = textBox4;
      Window window3 = (Window) textBox6;
      textBox5.Tag = (object) 1;
      window3.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox5.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window3);
      int x3 = x2 + (width + num1);
      TextBox[] tab3 = this.tab;
      TextBox textBox7;
      TextBox textBox8 = textBox7 = new TextBox("Creative", x3, y, width, height, textScale);
      TextBox textBox9 = textBox7;
      tab3[2] = textBox7;
      Window window4 = (Window) textBox9;
      window4.IsEnabled = this.instance.CanOpenCreativeMenu(this.player);
      textBox8.Tag = (object) 2;
      window4.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox8.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      int x4 = x3 + (width + num1);
      TextBox[] tab4 = this.tab;
      TextBox textBox10;
      TextBox textBox11 = textBox10 = new TextBox("Scripts", x4, y, width, height, textScale);
      TextBox textBox12 = textBox10;
      tab4[3] = textBox10;
      Window window5 = (Window) textBox12;
      textBox11.Tag = (object) 3;
      window5.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox11.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      int x5 = x4 + (width + num1);
      TextBox[] tab5 = this.tab;
      TextBox textBox13;
      TextBox textBox14 = textBox13 = new TextBox("Behaviours", x5, y, width, height, textScale);
      TextBox textBox15 = textBox13;
      tab5[4] = textBox13;
      Window window6 = (Window) textBox15;
      textBox14.Tag = (object) 4;
      window6.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox14.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      int x6 = x5 + (width + num1);
      TextBox[] tab6 = this.tab;
      TextBox textBox16;
      TextBox textBox17 = textBox16 = new TextBox("Options", x6, y, width, height, textScale);
      TextBox textBox18 = textBox16;
      tab6[5] = textBox16;
      Window window7 = (Window) textBox18;
      textBox17.Tag = (object) 5;
      window7.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox17.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      int x7 = x6 + (width + num1);
      TextBox[] tab7 = this.tab;
      TextBox textBox19;
      TextBox textBox20 = textBox19 = new TextBox("Graphics", x7, y, width, height, textScale);
      TextBox textBox21 = textBox19;
      tab7[6] = textBox19;
      Window window8 = (Window) textBox21;
      textBox20.Tag = (object) 6;
      window8.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox20.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      int x8 = x7 + (width + num1);
      bool flag = this.otherTab != null;
      TextBox[] tab8 = this.tab;
      TextBox textBox22;
      TextBox textBox23 = textBox22 = new TextBox(flag ? this.otherTab.Name : (string) null, x8, y, width, height, textScale);
      TextBox textBox24 = textBox22;
      tab8[7] = textBox22;
      Window window9 = (Window) textBox24;
      window9.IsVisible = flag;
      this.menu[7] = this.otherTab;
      textBox23.Tag = (object) 7;
      window9.Colors = (Window.ColorProfile) Colors.PauseMenuTab;
      textBox23.ClickHandler += new Window.WindowHandler(this.ClickChangeTab);
      window1.AddChild((StudioForge.Engine.Core.Node) window9);
      int num2 = x8 + (width + num1);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window10 = new Window((string) null, 0, 70, this.ScreenRect.Width, 5)
      {
        Name = "topPaneLine1"
      };
      window10.Colors = Colors.PauseMenuLine1.Copy(new Window.ColorProfile());
      window10.IsEnabled = false;
      this.windowManager.Root.AddChild((StudioForge.Engine.Core.Node) window10);
      Window window11 = new Window((string) null, 0, 75, this.ScreenRect.Width, 5)
      {
        Name = "topPaneLine2"
      };
      window11.Colors = Colors.PauseMenuLine2.Copy(new Window.ColorProfile());
      window11.IsEnabled = false;
      this.windowManager.Root.AddChild((StudioForge.Engine.Core.Node) window11);
      if (this.otherTab != null)
        this.tabID = PauseMenuScreen2.lastTabID = 7;
      this.SetTab(PauseMenuScreen2.lastTabID, false);
    }

    private void SetTab(int tabID)
    {
      this.SetTab(tabID, true);
    }

    private void SetTab(int tabID, bool checkForLast)
    {
      int num = tabID > this.tabID ? 1 : -1;
      for (; !checkForLast || tabID != this.tabID; tabID += num)
      {
        if (tabID == 7 && this.menu[tabID] == null)
          ++tabID;
        while (tabID < 0)
          tabID += this.menu.Length;
        if (tabID == 7 && this.menu[tabID] == null)
          --tabID;
        while (tabID >= this.menu.Length)
          tabID -= this.menu.Length;
        TextBox textBox = this.tab[tabID];
        if (textBox.IsEnabled || this.player.IsGodOrTester)
        {
          textBox.TextOffset.Y -= 3f;
          if (this.tabHighLight.Parent == null)
          {
            textBox.AddChild((StudioForge.Engine.Core.Node) this.tabHighLight);
          }
          else
          {
            ((TextBox) this.tabHighLight.Parent).TextOffset.Y += 3f;
            this.tabHighLight.ChangeParent((StudioForge.Engine.Core.Node) textBox);
          }
          if (this.menu[tabID] == null)
          {
            switch (tabID)
            {
              case 0:
                this.menu[tabID] = (NewGuiMenu) new MainGameMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 1:
                this.menu[tabID] = (NewGuiMenu) new MainCharacterMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 2:
                this.menu[tabID] = (NewGuiMenu) new MainCreativeMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 3:
                this.menu[tabID] = (NewGuiMenu) new MainScriptsMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 4:
                this.menu[tabID] = (NewGuiMenu) new MainBehavioursMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 5:
                this.menu[tabID] = (NewGuiMenu) new MainOptionsMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              case 6:
                this.menu[tabID] = (NewGuiMenu) new MainGraphicsMenu(this.instance, this.player);
                this.tab[tabID].Text = this.menu[tabID].Name;
                break;
              default:
                this.menu[tabID] = (NewGuiMenu) new DummyMenu(this.instance, this.player);
                break;
            }
          }
          GamertagData orAddGamertagData = Globals2.GamertagData.GetOrAddGamertagData(this.player.PlayerIndex);
          TabData tabData = (TabData) null;
          orAddGamertagData.TabData.TryGetValue(this.menu[tabID].Name, out tabData);
          if (tabData == null)
          {
            tabData = new TabData()
            {
              Sliding = true,
              Scale = 1f
            };
            orAddGamertagData.TabData.Add(this.menu[tabID].Name, tabData);
          }
          NewGuiMenu prevOpen = tabID != this.tabID ? this.CurrentMenu : (NewGuiMenu) null;
          NewGuiMenu2 newGuiMenu2 = this.menu[tabID] as NewGuiMenu2;
          if (newGuiMenu2 != null)
            newGuiMenu2.Open(this, prevOpen, tabData, orAddGamertagData.Settings.PlayerSettings.BackColor);
          else
            this.menu[tabID].Open(this.WindowManager, this.ScreenRect, this.BackTexture, prevOpen, new Action(((GameScreen) this).ExitScreen), tabData, orAddGamertagData.Settings.PlayerSettings.BackColor);
          this.tabID = tabID;
          break;
        }
      }
    }

    public void PushOtherTab(NewGuiMenu tab)
    {
      if (this.otherTabStack == null)
      {
        this.otherTabStack = new Stack<NewGuiMenu>();
        this.otherTabPrevIDStack = new Stack<int>();
      }
      this.otherTabStack.Push(this.menu[7]);
      this.otherTabPrevIDStack.Push(this.tabID);
      this.menu[7] = this.otherTab = tab;
      this.SetTab(7, true);
    }

    public void PopOtherTab()
    {
      if (this.otherTabStack.Count <= 0)
        return;
      if (this.menu[7] != null)
        this.menu[7].Close();
      this.menu[7] = this.otherTab = this.otherTabStack.Pop();
      this.SetTab(this.otherTabPrevIDStack.Pop(), true);
    }

    public override bool HandleInput(InputState input)
    {
      if (this.windowManager.HasActiveInputHandler)
        return base.HandleInput(input);
      if (base.HandleInput(input))
        return true;
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        if (this.tabID == 7 && this.otherTabStack != null && this.otherTabStack.Count > 0)
        {
          this.PopOtherTab();
        }
        else
        {
          this.menu[this.tabID]?.Close();
          this.ExitScreen();
        }
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.LockCanvas))
      {
        this.menu[this.tabID].ToggleCanvasLock();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PrevTab))
      {
        this.SetTab(this.tabID - 1, true);
        return true;
      }
      if (!InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.NextTab))
        return false;
      this.SetTab(this.tabID + 1, true);
      return true;
    }

    private void ClickChangeTab(object sender, WindowEventArgs e)
    {
      int tag = (int) e.Window.Tag;
      if (tag == this.tabID)
        return;
      this.SetTab(tag, false);
    }

    private void OpenOldMenu()
    {
      this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen(this.instance, this.player), this.ControllingPlayer);
      this.ExitScreen();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
    }

    protected override void DrawCore()
    {
      this.menu[this.tabID].Draw();
    }
  }
}
