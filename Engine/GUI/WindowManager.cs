// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.WindowManager
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GUI
{
  public class WindowManager
  {
    public static Color NavigableColor = Color.White * 0.5f;
    public float Alpha;
    public SpriteSortMode SpriteSortMode;
    private PlayerIndex playerIndex;
    private SpriteBatchSafe spriteBatch;
    private Window windowHovered;
    private Canvas canvasHovered;
    private Window windowPressed;
    private Window lastWindowHovered;
    private Window lastWindowPressed;
    private Window currentNavigable;
    private Window draggingWindow;
    private Window draggingWindowProxy;
    private Window draggingWindowHovered;
    private Vector2 draggingWindowPosition;
    private MouseButtons dragStartButton;
    private float dragEnablePressTimer;
    private float tooltipDelayTimer;
    private ITextInputWindow inputWindow;
    private ITextInput textInput;
    private Texture2D blankTexture;
    private RasterizerState scissorState;
    private GraphicsDevice graphicsDevice;
    private Window root;
    private Viewport viewport;
    private Rectangle bound;
    private Rectangle scissor;
    private Rectangle oldScissor;
    private List<WindowManager.DeferredDraw> deferredDraws;
    private CustomArray<Keys> keysDiff;
    private RenderProfile defaultRenderProfile;
    private RenderProfile currentRenderProfile;
    private List<Func<bool>> inputHandlers;

    public PlayerIndex PlayerIndex
    {
      get
      {
        return this.playerIndex;
      }
    }

    public WindowManager(PlayerIndex playerIndex)
    {
      this.playerIndex = playerIndex;
      this.IsEnabled = true;
      this.Alpha = 1f;
      this.keysDiff = new CustomArray<Keys>();
      this.deferredDraws = new List<WindowManager.DeferredDraw>();
      this.inputHandlers = new List<Func<bool>>();
    }

    public void LoadContent()
    {
      this.graphicsDevice = CoreGlobals.GraphicsDevice;
      this.viewport = this.graphicsDevice.Viewport;
      this.spriteBatch = new SpriteBatchSafe(this.graphicsDevice);
      this.blankTexture = CoreGlobals.BlankTexture;
      this.scissorState = new RasterizerState()
      {
        ScissorTestEnable = true
      };
      this.defaultRenderProfile = new RenderProfile();
      this.root = new Window()
      {
        Position = new Vector2((float) this.viewport.X, (float) this.viewport.Y),
        Size = new Point(this.viewport.Width, this.viewport.Height)
      };
    }

    private WindowEventArgs GetWinArgs(Window win)
    {
      return this.GetWinArgs(win, false);
    }

    private WindowEventArgs GetWinArgs(Window win, bool keyboardRaised)
    {
      Vector2 worldPosition = win.WorldPosition;
      Point mousePos = InputManager.GetMousePos(this.playerIndex);
      Point mousePosition = new Point((int) ((double) mousePos.X - (double) worldPosition.X), (int) ((double) mousePos.Y - (double) worldPosition.Y));
      return new WindowEventArgs(this, win, this.windowHovered, mousePosition, keyboardRaised);
    }

    private WindowDragEventArgs GetDraggingWinArgs()
    {
      return new WindowDragEventArgs(this, this.draggingWindow, this.draggingWindowHovered, this.draggingWindowProxy, new Point((int) this.draggingWindowPosition.X, (int) this.draggingWindowPosition.Y), this.dragStartButton == MouseButtons.RightButton);
    }

    public void RaiseClickHandler(Window win)
    {
      win?.RaiseClickHandler(this.GetWinArgs(win));
    }

    public void PushInputHandler(Func<bool> handler)
    {
      if (handler == null)
        return;
      this.inputHandlers.Add(handler);
    }

    public void PopInputHandler()
    {
      if (this.inputHandlers.Count <= 0)
        return;
      this.inputHandlers.RemoveAt(this.inputHandlers.Count - 1);
    }

    public Window Root
    {
      get
      {
        return this.root;
      }
    }

    public bool IsEnabled { get; set; }

    public bool HasActiveInputHandler
    {
      get
      {
        if (this.inputWindow != null)
          return this.textInput != null;
        return false;
      }
    }

    public Window CurrentNavigable
    {
      get
      {
        return this.currentNavigable;
      }
      set
      {
        this.SetNavigable(value);
      }
    }

    public void SetNavigable(Window window)
    {
      if (this.currentNavigable != null && this.currentNavigable.HasFlag(Window.WinFlags.KeepFocus) || (window == null || !window.IsEnabled) || !window.IsVisible)
        return;
      Window navigable = window.GetNavigable();
      if (navigable == null || !navigable.IsEnabled || (!navigable.IsVisible || !navigable.IsKeyNavigable))
        return;
      this.currentNavigable = navigable;
    }

    public void SetInputWindow(ITextInputWindow win)
    {
      if (win == this.inputWindow)
        return;
      this.EndInput();
      if (win == null)
        return;
      this.textInput = win.GetNewTextInputHandlerOnClick();
      this.inputWindow = win;
    }

    public bool HandleInput()
    {
      if (this.draggingWindow == null)
      {
        if (this.inputHandlers.Count > 0 && this.inputHandlers[this.inputHandlers.Count - 1]())
          return true;
        if (this.inputWindow != null && this.textInput != null)
        {
          if (this.textInput.InputCompleted)
          {
            Window inputWindow = this.inputWindow as Window;
            if (inputWindow != null)
              this.SetNavigable(inputWindow);
            this.textInput = (ITextInput) null;
            this.inputWindow = (ITextInputWindow) null;
          }
          else if (this.textInput.HandleInput(this.playerIndex) || this.textInput.FullInputControl)
            return true;
        }
        if (this.windowHovered != null && this.windowHovered.HandleInput(this.GetWinArgs(this.windowHovered)))
          return true;
        if (this.currentNavigable != null)
        {
          if (this.HandleKeyInput(this.currentNavigable))
            return true;
          if (this.inputWindow == null && this.textInput == null && !this.currentNavigable.HasFlag(Window.WinFlags.KeepFocus))
          {
            if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.Up) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.DPadUp))
            {
              if (this.MoveNavigableUp())
                return true;
            }
            else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.Down) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.DPadDown))
            {
              if (this.MoveNavigableDown())
                return true;
            }
            else if (InputManager.IsKeyPressedNew(this.playerIndex, Keys.Left) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.DPadLeft))
            {
              if (this.MoveNavigableLeft())
                return true;
            }
            else if ((InputManager.IsKeyPressedNew(this.playerIndex, Keys.Right) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.DPadRight)) && this.MoveNavigableRight())
              return true;
            if (this.currentNavigable == null)
              return false;
          }
          if (this.currentNavigable.HasFlag(Window.WinFlags.LeftClickOnPress) ? InputManager.IsKeyPressedNew(this.playerIndex, Keys.Enter) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.Start) : InputManager.IsKeyReleasedNew(this.playerIndex, Keys.Enter) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.Start))
          {
            if (this.inputWindow != null)
            {
              if (!this.inputWindow.EqualsInputWindow((object) this.currentNavigable))
                this.EndInput();
            }
            else
            {
              ITextInputWindow currentNavigable = this.currentNavigable as ITextInputWindow;
              if (currentNavigable != null)
              {
                this.textInput = currentNavigable.GetNewTextInputHandlerOnClick();
                this.inputWindow = currentNavigable;
              }
              WindowEventArgs winArgs = this.GetWinArgs(this.currentNavigable, true);
              this.currentNavigable.OnClick(winArgs);
              this.currentNavigable.RaiseClickHandler(winArgs);
            }
            return true;
          }
        }
      }
      return this.HandleMouseInput();
    }

    private bool HandleKeyInput(Window win)
    {
      bool flag1 = false;
      Keys[] pressedKeysPrev = InputManager.GetPressedKeysPrev(this.playerIndex);
      Keys[] pressedKeys = InputManager.GetPressedKeys(this.playerIndex);
      this.keysDiff.Clear();
      foreach (Keys t in pressedKeys)
      {
        bool flag2 = false;
        foreach (Keys keys in pressedKeysPrev)
        {
          if (keys == t)
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
          this.keysDiff.Add(t);
      }
      if (this.keysDiff.Count > 0)
      {
        WindowEventArgs winArgs = this.GetWinArgs(win, true);
        if (win.OnKeyPress(winArgs, this.keysDiff.Array))
          flag1 = true;
      }
      this.keysDiff.Clear();
      foreach (Keys t in pressedKeysPrev)
      {
        bool flag2 = false;
        foreach (Keys keys in pressedKeys)
        {
          if (keys == t)
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
          this.keysDiff.Add(t);
      }
      if (this.keysDiff.Count > 0)
      {
        WindowEventArgs winArgs = this.GetWinArgs(win, true);
        if (win.OnKeyRelease(winArgs, this.keysDiff.Array))
          flag1 = true;
      }
      return flag1;
    }

    private bool HandleMouseInput()
    {
      this.lastWindowHovered = this.windowHovered;
      this.CheckForMouseHover(out this.windowHovered, out this.canvasHovered);
      bool flag1 = this.windowHovered != null && this.windowHovered.IsWorldEnabled;
      if (this.draggingWindow == null && flag1)
      {
        bool flag2 = InputManager.IsMouseButtonPressed(this.playerIndex, MouseButtons.LeftButton) || InputManager.IsButtonPressed(this.playerIndex, Buttons.A);
        bool flag3 = !flag2 && (InputManager.IsMouseButtonPressed(this.playerIndex, MouseButtons.RightButton) || InputManager.IsButtonPressed(this.playerIndex, Buttons.Y));
        if (flag2 || flag3)
        {
          if (this.windowPressed == null)
          {
            this.windowPressed = this.lastWindowPressed = this.windowHovered;
            this.dragStartButton = flag2 ? MouseButtons.LeftButton : MouseButtons.RightButton;
            this.dragEnablePressTimer = 0.0f;
          }
        }
        else
          this.windowPressed = (Window) null;
      }
      if (this.windowHovered != this.lastWindowHovered)
      {
        this.tooltipDelayTimer = 0.0f;
        if (this.lastWindowHovered != null && this.lastWindowHovered.IsWorldEnabled)
        {
          if (this.windowHovered == null || !this.windowHovered.IsChildOf((Node) this.lastWindowHovered))
            this.lastWindowHovered.RaiseHoverEndHandler(this.GetWinArgs(this.lastWindowHovered));
          if (this.windowHovered == null)
          {
            for (Window parent = this.lastWindowHovered.Parent as Window; parent != null; parent = parent.Parent as Window)
              parent.RaiseHoverEndHandler(this.GetWinArgs(parent));
          }
          else
          {
            for (Window parent = this.lastWindowHovered.Parent as Window; parent != null; parent = parent.Parent as Window)
            {
              if (!this.windowHovered.IsChildOf((Node) parent))
                parent.RaiseHoverEndHandler(this.GetWinArgs(parent));
            }
          }
        }
        if (flag1 && (this.windowHovered == this.windowPressed || this.draggingWindow == this.windowPressed || this.windowPressed == null))
          this.windowHovered.RaiseHoverStartHandler(this.GetWinArgs(this.windowHovered));
      }
      if (this.canvasHovered != null && this.canvasHovered != this.windowHovered && this.canvasHovered.IsWorldEnabled)
        this.canvasHovered.OnHover(this.GetWinArgs((Window) this.canvasHovered));
      if (flag1)
        this.HandleWindowHovered(this.windowHovered, this.canvasHovered);
      if (this.windowPressed != null)
        this.HandleWindowPressed();
      if (this.draggingWindow == null && this.windowPressed != null && this.windowPressed.HasFlag(Window.WinFlags.IsDragable))
      {
        this.dragEnablePressTimer += Services.ElapsedTime;
        if ((double) this.dragEnablePressTimer >= (double) this.windowPressed.DragEnablePressTime)
        {
          this.dragEnablePressTimer = 0.0f;
          this.draggingWindow = this.windowPressed;
          this.draggingWindowProxy = this.windowPressed.GetDragProxy(this.GetDraggingWinArgs());
          this.draggingWindow.RaiseDragStartHandler(this.GetDraggingWinArgs());
          if (!this.draggingWindow.HasFlagAny(Window.WinFlags.DragCopy))
            this.draggingWindow.IsVisible = false;
        }
      }
      if (this.draggingWindow != null)
        this.DragWindow();
      if (this.draggingWindow == null)
        return this.windowPressed != null;
      return true;
    }

    private void HandleWindowHovered(Window windowHovered, Canvas canvasHovered)
    {
      WindowEventArgs winArgs = this.GetWinArgs(windowHovered);
      windowHovered.OnHover(winArgs);
      windowHovered.RaiseHoverHandler(winArgs);
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.playerIndex);
      if (mouseWheelDelta != 0)
        windowHovered.OnMouseWheelDelta(winArgs, mouseWheelDelta);
      if (this.draggingWindow != null)
        return;
      if (InputManager.IsMouseButtonPressedNew(this.playerIndex, MouseButtons.LeftButton) || InputManager.IsButtonPressedNew(this.playerIndex, Buttons.A))
      {
        if (this.inputWindow != null && !this.inputWindow.EqualsInputWindow((object) windowHovered))
          this.EndInput();
        this.SetNavigable(windowHovered);
        if (!windowHovered.HasFlag(Window.WinFlags.LeftClickOnPress))
          return;
        this.HandleLeftClick(windowHovered);
      }
      else
      {
        if (windowHovered != this.lastWindowPressed)
          return;
        if (InputManager.IsMouseButtonReleasedNew(this.playerIndex, MouseButtons.LeftButton) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.A))
        {
          if (windowHovered.HasFlag(Window.WinFlags.LeftClickOnPress))
            return;
          this.HandleLeftClick(windowHovered);
        }
        else if (InputManager.IsMouseButtonReleasedNew(this.playerIndex, MouseButtons.RightButton) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.Y))
        {
          windowHovered.OnRightClick(winArgs);
          windowHovered.RaiseRightClickHandler(winArgs);
        }
        else
        {
          if (!InputManager.IsMouseButtonReleasedNew(this.playerIndex, MouseButtons.MiddleButton) && !InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.RightStick))
            return;
          windowHovered.OnMiddleClick(winArgs);
          windowHovered.RaiseMiddleClickHandler(winArgs);
        }
      }
    }

    private void HandleWindowPressed()
    {
      WindowEventArgs winArgs = this.GetWinArgs(this.windowPressed);
      if (InputManager.IsMouseButtonPressed(this.playerIndex, MouseButtons.LeftButton) || InputManager.IsButtonPressed(this.playerIndex, Buttons.A))
      {
        this.windowPressed.OnClickDown(winArgs);
        this.windowPressed.RaiseClickDownHandler(winArgs);
      }
      else if (InputManager.IsMouseButtonPressed(this.playerIndex, MouseButtons.RightButton) || InputManager.IsButtonPressed(this.playerIndex, Buttons.Y))
      {
        this.windowPressed.OnRightClickDown(winArgs);
        this.windowPressed.RaiseRightDownClickHandler(winArgs);
      }
      else
      {
        if (!InputManager.IsMouseButtonPressed(this.playerIndex, MouseButtons.MiddleButton) && !InputManager.IsButtonPressed(this.playerIndex, Buttons.RightStick))
          return;
        this.windowPressed.OnMiddleClickDown(winArgs);
        this.windowPressed.RaiseMiddleDownClickHandler(winArgs);
      }
    }

    private void HandleLeftClick(Window win)
    {
      ITextInputWindow textInputWindow = win as ITextInputWindow;
      if (textInputWindow != null)
      {
        this.textInput = textInputWindow.GetNewTextInputHandlerOnClick();
        this.inputWindow = textInputWindow;
      }
      WindowEventArgs winArgs = this.GetWinArgs(win);
      win.OnClick(winArgs);
      win.RaiseClickHandler(winArgs);
      this.dragEnablePressTimer = 0.0f;
    }

    private void EndInput()
    {
      if (this.inputWindow == null)
        return;
      if (this.textInput != null)
        this.textInput.EndInput(true);
      else
        this.inputWindow.EndInput(true);
      this.textInput = (ITextInput) null;
      this.inputWindow = (ITextInputWindow) null;
    }

    public bool CheckForMouseHover(out Window win, out Canvas canvas)
    {
      win = (Window) null;
      canvas = (Canvas) null;
      Vector4 bound = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
      this.CheckForMouseHover(this.root, Vector2.Zero, 1f, ref win, ref canvas, ref bound, true);
      foreach (WindowManager.DeferredDraw deferredDraw in this.deferredDraws)
        this.CheckForMouseHover(deferredDraw.Window, deferredDraw.Position, deferredDraw.Scale, ref win, ref canvas, ref bound, false);
      if (this.draggingWindow != null)
      {
        this.draggingWindowHovered = win;
        win = this.draggingWindow;
      }
      return win != null;
    }

    private void CheckForMouseHover(
      Window parent,
      Vector2 pos,
      float scale,
      ref Window winHovered,
      ref Canvas canvasHovered,
      ref Vector4 bound,
      bool fromRoot)
    {
      Canvas canvas = parent as Canvas;
      if (canvas != null)
      {
        pos.X += canvas.Offset.X;
        pos.Y += canvas.Offset.Y;
      }
      for (Window window = fromRoot ? parent.FirstChild as Window : parent; window != null; window = window.NextSibling as Window)
      {
        if (window.IsVisible && window != this.draggingWindow && !window.HasFlagAny(Window.WinFlags.IsNotHoverable))
        {
          Point point1 = window.SizeScaled(scale);
          Vector2 vector2_1 = new Vector2(pos.X + window.Position.X * scale, pos.Y + window.Position.Y * scale);
          Vector2 vector2_2 = new Vector2(vector2_1.X + (float) point1.X, vector2_1.Y + (float) point1.Y);
          if ((double) vector2_1.X < (double) bound.X)
            bound.X = vector2_1.X;
          if ((double) vector2_1.Y < (double) bound.Y)
            bound.Y = vector2_1.Y;
          if ((double) vector2_2.X > (double) bound.Z)
            bound.Z = vector2_2.X;
          if ((double) vector2_2.Y > (double) bound.W)
            bound.W = vector2_2.Y;
          Point mousePos = InputManager.GetMousePos(this.playerIndex);
          if (window == this.lastWindowHovered && window.HasFlag(Window.WinFlags.TrapMouse) && window.IsEnabled)
          {
            Point point2 = mousePos;
            if ((double) point2.X < (double) vector2_1.X)
              point2.X = (int) vector2_1.X;
            else if ((double) point2.X > (double) vector2_2.X)
              point2.X = (int) vector2_2.X;
            if ((double) point2.Y < (double) vector2_1.Y)
              point2.Y = (int) vector2_1.Y;
            else if ((double) point2.Y > (double) vector2_2.Y)
              point2.Y = (int) vector2_2.Y;
            if (point2.X != mousePos.X || point2.Y != mousePos.Y)
            {
              mousePos.X = point2.X;
              mousePos.Y = point2.Y;
              InputManager.SetMousePos(point2.X, point2.Y);
            }
          }
          if ((double) mousePos.X >= (double) vector2_1.X && (double) mousePos.X <= (double) vector2_2.X && ((double) mousePos.Y >= (double) vector2_1.Y && (double) mousePos.Y <= (double) vector2_2.Y) && window.IsEnabled)
          {
            winHovered = window;
            if (window is Canvas)
              canvasHovered = window as Canvas;
          }
        }
        if (!fromRoot)
          break;
      }
      for (Window parent1 = parent.FirstChild as Window; parent1 != null; parent1 = parent1.NextSibling as Window)
      {
        if (parent1.IsVisible && parent1 != this.draggingWindow && (!parent1.HasFlagAny(Window.WinFlags.IsNotHoverable) && parent1.HasChildren))
        {
          Vector4 bound1 = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
          Vector2 pos1 = new Vector2(pos.X + parent1.Position.X * scale, pos.Y + parent1.Position.Y * scale);
          this.CheckForMouseHover(parent1, pos1, scale * parent1.Scale, ref winHovered, ref canvasHovered, ref bound1, true);
          if (parent1 is Canvas)
            ((Canvas) parent1).InnerBound = bound1;
        }
      }
    }

    private void DragWindow()
    {
      Point mousePos = InputManager.GetMousePos(this.playerIndex);
      this.draggingWindowPosition.X = (float) mousePos.X;
      this.draggingWindowPosition.Y = (float) mousePos.Y;
      WindowDragEventArgs draggingWinArgs = this.GetDraggingWinArgs();
      this.draggingWindow.RaiseDragHandler(draggingWinArgs);
      if (!(this.dragStartButton == MouseButtons.LeftButton ? InputManager.IsMouseButtonReleasedNew(this.playerIndex, MouseButtons.LeftButton) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.A) : InputManager.IsMouseButtonReleasedNew(this.playerIndex, MouseButtons.RightButton) || InputManager.IsButtonReleasedNew(this.playerIndex, Buttons.Y)))
        return;
      this.draggingWindow.RaiseDragEndHandler(draggingWinArgs);
      if (!this.draggingWindow.HasFlagAny(Window.WinFlags.DragCopy))
        this.draggingWindow.IsVisible = true;
      this.windowHovered = this.windowPressed = this.draggingWindow = this.draggingWindowHovered = this.draggingWindowProxy = (Window) null;
    }

    private bool MoveNavigableUp()
    {
      for (Window prevSibling = this.currentNavigable.PrevSibling as Window; prevSibling != null; prevSibling = prevSibling.PrevSibling as Window)
      {
        if (prevSibling.IsEnabled && prevSibling.IsVisible)
        {
          Window navigable = prevSibling.GetNavigable();
          if (navigable == this.currentNavigable)
            return false;
          if (navigable != null && navigable.IsKeyNavigable)
          {
            this.currentNavigable = navigable;
            return true;
          }
        }
      }
      this.currentNavigable = (Window) null;
      return false;
    }

    private bool MoveNavigableDown()
    {
      if (this.currentNavigable.Parent == null)
        return false;
      for (Window window = this.currentNavigable.NextSibling != null ? this.currentNavigable.NextSibling as Window : this.currentNavigable.Parent.FirstChild as Window; window != null; window = window.NextSibling != null ? window.NextSibling as Window : this.currentNavigable.Parent.FirstChild as Window)
      {
        if (window.IsEnabled && window.IsVisible)
        {
          Window navigable = window.GetNavigable();
          if (navigable == this.currentNavigable)
            return false;
          if (navigable != null && navigable.IsKeyNavigable)
          {
            this.currentNavigable = navigable;
            return true;
          }
        }
      }
      this.currentNavigable = (Window) null;
      return false;
    }

    private bool MoveNavigableLeft()
    {
      Window prevSibling = this.currentNavigable.PrevSibling as Window;
      Vector2 worldPosition1 = this.currentNavigable.WorldPosition;
      for (; prevSibling != null; prevSibling = prevSibling.PrevSibling as Window)
      {
        if (prevSibling.IsEnabled && prevSibling.IsVisible)
        {
          Window navigable = prevSibling.GetNavigable();
          if (navigable == this.currentNavigable)
            return false;
          if (navigable != null && navigable.IsKeyNavigable)
          {
            Vector2 worldPosition2 = navigable.WorldPosition;
            if ((double) worldPosition2.X + (double) navigable.Size.X < (double) worldPosition1.X && (double) worldPosition2.Y > (double) worldPosition1.Y - (double) this.currentNavigable.Size.Y && (double) worldPosition2.Y < (double) worldPosition1.Y + (double) this.currentNavigable.Size.Y)
            {
              this.currentNavigable = navigable;
              return true;
            }
          }
        }
      }
      this.currentNavigable = (Window) null;
      return false;
    }

    private bool MoveNavigableRight()
    {
      if (this.currentNavigable.Parent == null)
        return false;
      Window window = this.currentNavigable.NextSibling != null ? this.currentNavigable.NextSibling as Window : this.currentNavigable.Parent.FirstChild as Window;
      Vector2 worldPosition1 = this.currentNavigable.WorldPosition;
      for (; window != null; window = window.NextSibling != null ? window.NextSibling as Window : this.currentNavigable.Parent.FirstChild as Window)
      {
        if (window.IsEnabled && window.IsVisible)
        {
          Window navigable = window.GetNavigable();
          if (navigable == this.currentNavigable)
            return false;
          if (navigable != null && navigable.IsKeyNavigable)
          {
            Vector2 worldPosition2 = navigable.WorldPosition;
            if ((double) worldPosition2.X > (double) worldPosition1.X + (double) this.currentNavigable.Size.X && (double) worldPosition2.Y > (double) worldPosition1.Y - (double) this.currentNavigable.Size.Y && (double) worldPosition2.Y < (double) worldPosition1.Y + (double) this.currentNavigable.Size.Y)
            {
              this.currentNavigable = navigable;
              return true;
            }
          }
        }
      }
      this.currentNavigable = (Window) null;
      return false;
    }

    public void Update()
    {
    }

    public void Draw()
    {
      this.deferredDraws.Clear();
      this.viewport = this.graphicsDevice.Viewport;
      this.currentRenderProfile = this.defaultRenderProfile;
      for (Window win = this.root.FirstChild as Window; win != null; win = win.NextSibling as Window)
      {
        if (win.IsVisible)
          this.DrawWindow(win, Vector2.Zero, 1f, this.Alpha, true);
      }
      foreach (WindowManager.DeferredDraw deferredDraw in this.deferredDraws)
      {
        if (deferredDraw.Window.IsVisible)
        {
          deferredDraw.Window.ClearFlags(Window.WinFlags.DeferDraw);
          this.DrawWindow(deferredDraw.Window, deferredDraw.Position, deferredDraw.Scale, deferredDraw.Alpha, true);
          deferredDraw.Window.AddFlags(Window.WinFlags.DeferDraw);
        }
      }
      if (this.draggingWindow != null)
      {
        bool isVisible = this.draggingWindowProxy.IsVisible;
        this.draggingWindowProxy.IsVisible = true;
        Window.WinFlags flags = this.draggingWindowProxy.Flags;
        this.draggingWindowProxy.ClearFlags(Window.WinFlags.DeferDraw);
        Vector2 position = this.draggingWindowProxy.Position;
        this.draggingWindowProxy.Position = this.draggingWindowPosition;
        this.DrawWindow(this.draggingWindowProxy, Vector2.Zero, this.draggingWindowProxy.Scale, this.draggingWindowProxy.Alpha, true);
        this.draggingWindowProxy.Position = position;
        this.draggingWindowProxy.Flags = flags;
        this.draggingWindowProxy.IsVisible = isVisible;
      }
      if (this.windowHovered != null && this.windowHovered != this.draggingWindow && (this.windowHovered.ToolTip != null && this.windowHovered.ToolTip.IsValid))
      {
        this.tooltipDelayTimer += Services.ElapsedTime;
        if ((double) this.tooltipDelayTimer >= (double) this.windowHovered.ToolTip.Delay)
        {
          if (!this.spriteBatch.BeginCalled)
            this.spriteBatch.Begin(this.SpriteSortMode, this.defaultRenderProfile.Blend, this.defaultRenderProfile.Sampler, this.defaultRenderProfile.DepthStencil, this.defaultRenderProfile.Rasterizer, this.defaultRenderProfile.Effect, Matrix.Identity);
          this.DrawToolTip();
        }
      }
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private void DrawWindow(
      Window win,
      Vector2 worldPos,
      float worldScale,
      float worldAlpha,
      bool worldIsEnabled)
    {
      Vector2 worldPos1 = new Vector2(worldPos.X + win.Position.X * worldScale, worldPos.Y + win.Position.Y * worldScale);
      float num1 = worldScale * win.Scale;
      float num2 = worldAlpha * win.Alpha;
      bool flag1 = win.IsEnabled && worldIsEnabled;
      if (win.HasFlag(Window.WinFlags.DeferDraw))
      {
        this.deferredDraws.Add(new WindowManager.DeferredDraw()
        {
          Window = win,
          Position = worldPos,
          Scale = worldScale,
          Alpha = worldAlpha
        });
      }
      else
      {
        int borderThickness = win.BorderThickness;
        Color color = flag1 ? win.Colors.BackColor : win.Colors.BackDisabledColor;
        bool flag2 = false;
        if (win.Size.X > 0 && win.Size.Y > 0)
        {
          this.bound.X = (int) worldPos1.X;
          this.bound.Y = (int) worldPos1.Y;
          Point point = win.SizeScaled(worldScale);
          this.bound.Width = Math.Max(1, point.X);
          this.bound.Height = Math.Max(1, point.Y);
          this.scissor.X = this.bound.X;
          this.scissor.Y = this.bound.Y;
          this.scissor.Width = this.bound.Width;
          this.scissor.Height = this.bound.Height;
          if (this.scissor.X < this.viewport.Width && this.scissor.Y < this.viewport.Height && (this.scissor.X + this.scissor.Width >= 0 && this.scissor.Y + this.scissor.Height >= 0))
          {
            if (this.scissor.X < 0)
            {
              this.scissor.Width += this.scissor.X;
              this.scissor.X = 0;
            }
            if (this.scissor.Y < 0)
            {
              this.scissor.Height += this.scissor.Y;
              this.scissor.Y = 0;
            }
            if (this.scissor.X + this.scissor.Width >= this.viewport.Width)
              this.scissor.Width = this.viewport.Width - this.scissor.X;
            if (this.scissor.Y + this.scissor.Height >= this.viewport.Height)
              this.scissor.Height = this.viewport.Height - this.scissor.Y;
            if (flag1 && (win == this.windowHovered || win == this.draggingWindowProxy || win == this.draggingWindowHovered && this.draggingWindowHovered.HasFlag(Window.WinFlags.UseHoverColorIfDraggedOver)) && (this.windowHovered == this.windowPressed || this.draggingWindow == this.windowPressed || this.windowPressed == null))
              color = this.windowPressed == null || !this.windowPressed.HasLeftClickHandler && !this.windowPressed.HasRightClickHandler ? win.Colors.BackHoverColor : win.Colors.BackClickColor;
            flag2 = true;
          }
        }
        RenderProfile currentRenderProfile = this.currentRenderProfile;
        this.currentRenderProfile = win.RenderProfile != null ? win.RenderProfile : this.defaultRenderProfile;
        bool flag3 = this.spriteBatch.BeginCalled && this.currentRenderProfile == currentRenderProfile;
        bool flag4 = this.currentRenderProfile.Rasterizer != null && this.currentRenderProfile.Rasterizer.ScissorTestEnable;
        bool flag5 = win.HasFlag(Window.WinFlags.ClipChildren);
        if (flag5 || !flag3)
        {
          RasterizerState rasterizerState = this.currentRenderProfile.Rasterizer;
          if (flag2 && flag5)
          {
            this.oldScissor = this.graphicsDevice.ScissorRectangle;
            this.graphicsDevice.ScissorRectangle = this.scissor;
            if (rasterizerState != null)
              rasterizerState.ScissorTestEnable = true;
            else
              rasterizerState = this.scissorState;
          }
          this.spriteBatch.Begin(this.SpriteSortMode, this.currentRenderProfile.Blend, this.currentRenderProfile.Sampler, this.currentRenderProfile.DepthStencil, rasterizerState, this.currentRenderProfile.Effect, Matrix.Identity);
        }
        if (flag2)
        {
          win.DrawBackground(this.spriteBatch, this.bound, num1, num2, win.GetBackColorOverride(color));
          win.Draw(this.spriteBatch, this.bound, num1, num2, flag1);
          if (borderThickness > 0)
            win.DrawBorder(this.spriteBatch, this.bound, num1, num2, 0.0f);
          if (win == this.currentNavigable && !win.HasFlag(Window.WinFlags.HideNavBorder))
            this.spriteBatch.DrawBox(CoreGlobals.BlankTexture, this.bound.Expand(-1), 2, WindowManager.NavigableColor * worldAlpha, 0.0f);
          (win.Parent as Window)?.DrawingChild(this.bound, worldScale);
        }
        Canvas canvas = win as Canvas;
        if (canvas != null)
        {
          worldPos1.X += canvas.Offset.X * worldScale;
          worldPos1.Y += canvas.Offset.Y * worldScale;
        }
        win.DrawPreChild();
        for (Window win1 = win.FirstChild as Window; win1 != null; win1 = win1.NextSibling as Window)
        {
          if (win1.IsVisible)
            this.DrawWindow(win1, worldPos1, num1, num2, flag1);
        }
        if (!flag5 && flag3)
          return;
        this.spriteBatch.End();
        if (!flag2 || !flag5)
          return;
        this.graphicsDevice.ScissorRectangle = this.oldScissor;
        if (this.currentRenderProfile.Rasterizer == null)
          return;
        this.currentRenderProfile.Rasterizer.ScissorTestEnable = flag4;
      }
    }

    private void DrawToolTip()
    {
      Rectangle rect = this.windowHovered.ToolTip.GetRect();
      Point mousePos = InputManager.GetMousePos(this.playerIndex);
      rect.X = mousePos.X;
      rect.Y = mousePos.Y - rect.Height - 4;
      int num1 = (int) ((double) this.graphicsDevice.Viewport.Width * 0.899999976158142);
      if (rect.X + rect.Width > num1 - 4)
        rect.X = num1 - 4 - rect.Width + 1;
      int num2 = (int) ((double) CoreGlobals.GraphicsDevice.Viewport.Height * 0.899999976158142);
      if (rect.Y + rect.Height > num2 - 4)
        rect.Y = num2 - 4 - rect.Height + 1;
      this.windowHovered.ToolTip.Draw(this.spriteBatch, rect);
    }

    private struct DeferredDraw
    {
      public Window Window;
      public Vector2 Position;
      public float Scale;
      public float Alpha;
    }
  }
}
