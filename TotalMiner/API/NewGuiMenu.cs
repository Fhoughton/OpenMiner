// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.NewGuiMenu
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.API
{
  public abstract class NewGuiMenu
  {
    protected ITMGame game;
    protected ITMPlayer player;
    protected PlayerIndex playerIndex;
    protected Window baseWindow;
    protected Window initialNavigable;
    protected WindowManager windowManager;
    protected Action onExit;
    public Canvas canvas;
    protected Rectangle screenRect;
    protected Color containerColor;
    protected TabData tabData;
    private Texture2D lockTex;

    public abstract string Name { get; }

    protected Window TopPaneWindow
    {
      get
      {
        return this.windowManager.Root.FindChild("topPane");
      }
    }

    public NewGuiMenu(ITMGame game, ITMPlayer player)
    {
      this.game = game;
      this.player = player;
      this.playerIndex = player.PlayerIndex;
      this.containerColor = Color.Transparent;
    }

    public virtual void Open(
      WindowManager windowManager,
      Rectangle screenRect,
      Texture2D backTexture,
      NewGuiMenu prevOpen,
      Action onExit,
      TabData tabData,
      Color backColor)
    {
      this.windowManager = windowManager;
      this.screenRect = screenRect;
      this.onExit = onExit;
      this.tabData = tabData;
      prevOpen?.Close();
      if (this.baseWindow == null)
        this.InitWindows(backTexture);
      windowManager.Root.InsertNode((Node) null, (Node) this.baseWindow);
      windowManager.SetNavigable(this.initialNavigable);
      windowManager.PushInputHandler(new Func<bool>(this.HandleInput));
      this.SetTintColor(backColor);
      this.ResetCanvasTabData();
    }

    protected virtual void ResetCanvasTabData()
    {
      this.canvas.SlidingScroll = this.tabData.Sliding;
      this.canvas.Scale = this.tabData.Scale;
      this.canvas.Offset = this.tabData.Offset;
      if (this.canvas.SlidingScroll)
      {
        Point mousePos = InputManager.GetMousePos(this.playerIndex);
        this.canvas.SetOffsetForMousePos(new Point((int) ((double) mousePos.X - (double) this.canvas.Position.X), (int) ((double) mousePos.Y - (double) this.canvas.Position.Y)));
      }
      this.UpdateCanvasLockIcon();
    }

    protected void SetTintColor(Color color)
    {
      this.baseWindow.Texture.TintColor = color;
      Window child1 = this.windowManager.Root.FindChild("topPaneLine1");
      if (child1 == null)
        return;
      Window child2 = this.windowManager.Root.FindChild("topPaneLine2");
      Color color1 = new Color((int) ((double) color.R * 1.39999997615814), (int) ((double) color.G * 1.39999997615814), (int) ((double) color.B * 1.39999997615814), (int) child2.Colors.BackDisabledColor.A);
      Color color2 = new Color((int) ((double) color1.R * 0.5), (int) ((double) color1.G * 0.5), (int) ((double) color1.B * 0.5), (int) child1.Colors.BackDisabledColor.A);
      child1.Colors.BackDisabledColor = color1;
      child2.Colors.BackDisabledColor = color2;
    }

    public virtual void Close()
    {
      this.windowManager.PopInputHandler();
      this.windowManager.Root.RemoveChild((Node) this.baseWindow);
    }

    protected virtual void InitWindows(Texture2D backTexture)
    {
      this.lockTex = CoreGlobals.Content.Load<Texture2D>("Textures\\lock");
      this.baseWindow = new Window((string) null, this.screenRect.X, this.screenRect.Y, this.screenRect.Width, this.screenRect.Height)
      {
        Name = "baseWindow"
      };
      this.baseWindow.Colors = Window.TransparentColorProfile;
      this.baseWindow.LoadTexture(backTexture, true, true, 1f);
      this.baseWindow.Texture.DestRect = new Rectangle?(new Rectangle(0, 0, this.screenRect.Width, this.screenRect.Height));
      Canvas canvas = new Canvas((string) null, this.screenRect.X, this.screenRect.Y, this.screenRect.Width, this.screenRect.Height);
      canvas.Name = "canvas";
      this.canvas = canvas;
      this.canvas.Colors = Window.TransparentColorProfile;
      this.canvas.ResetScaleInput = (ushort) 212;
      this.canvas.ZoomHandler += new Window.WindowHandler(this.OnCanvasZoom);
      this.baseWindow.AddChild((Node) this.canvas);
      Window window = new Window((string) null, this.screenRect.X + this.screenRect.Width - 9, this.screenRect.Y + 82, 7, 11)
      {
        Name = "lock"
      };
      window.Colors = Colors.IconColors;
      window.LoadTexture(this.lockTex, true);
      window.Texture.SrRect = new Rectangle?(new Rectangle(0, 0, 7, 11));
      this.baseWindow.AddChild((Node) window);
    }

    public void ToggleCanvasLock()
    {
      this.canvas.SlidingScroll = !this.canvas.SlidingScroll;
      this.tabData.Sliding = this.canvas.SlidingScroll;
      this.tabData.Offset = this.canvas.Offset;
      this.tabData.Scale = this.canvas.Scale;
      this.UpdateCanvasLockIcon();
    }

    private void UpdateCanvasLockIcon()
    {
      Window child = this.baseWindow.FindChild("lock");
      if (child == null)
        return;
      child.IsVisible = !this.canvas.SlidingScroll;
    }

    private void OnCanvasZoom(object Sender, WindowEventArgs args)
    {
      Canvas window = args.Window as Canvas;
      if (window == null)
        return;
      this.tabData.Scale = window.Scale;
    }

    public void ExitScreen()
    {
      this.windowManager.PopInputHandler();
      if (this.onExit == null)
        return;
      this.onExit();
    }

    public virtual void OnParentExit()
    {
    }

    protected string OnOff(bool o)
    {
      return !o ? "Off" : "On";
    }

    protected virtual bool HandleInput()
    {
      return false;
    }

    public void Draw()
    {
      this.DrawCore();
    }

    protected virtual void DrawCore()
    {
    }
  }
}
