// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.GameScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GameState
{
  public abstract class GameScreen
  {
    public static TimeSpan DefaultTransitionOnTime = TimeSpan.FromSeconds(1.0);
    public static TimeSpan DefaultTransitionOffTime = TimeSpan.FromSeconds(1.0);
    public Matrix Matrix = Matrix.Identity;
    protected float clientBackAlpha = 0.9f;
    protected int borderWidth = 8;
    protected Color borderColor = new Color(0.1f, 0.1f, 0.1f);
    protected Color clientBackColor = new Color(0.2f, 0.2f, 0.2f);
    protected Color backColor = Color.CornflowerBlue;
    private TimeSpan transitionOnTime = GameScreen.DefaultTransitionOnTime;
    private TimeSpan transitionOffTime = GameScreen.DefaultTransitionOffTime;
    public SpriteFont Font;
    public bool IsPopup;
    public bool FadeBehindIfPopup;
    public bool WinRectClickPress;
    protected static bool isToolTipsEnabledGlobal;
    protected bool otherScreenHasFocus;
    protected IContentManager content;
    protected int drawFrameCount;
    protected WindowManager windowManager;
    protected int winRectHovered;
    protected int winRectPressed;
    protected List<Rectangle> winRects;
    protected List<EventHandler<EventArgs>> winRectHandlers;
    private PlayerIndex? controllingPlayer;
    private float transitionPosition;
    private ScreenState screenState;
    private bool isExiting;

    public event EventHandler<EventArgs> ScreenRemoved;

    public GameScreen()
    {
      this.FadeBehindIfPopup = true;
    }

    public virtual void LoadContent()
    {
      if (!this.IsContentLoaded)
      {
        this.SetContentManager();
        this.IsContentLoaded = true;
      }
      this.Font = CoreGlobals.GameFont;
      this.windowManager = new WindowManager(this.controllingPlayer.HasValue ? this.controllingPlayer.Value : PlayerIndex.One);
      this.windowManager.LoadContent();
    }

    protected virtual void SetContentManager()
    {
      this.content = CoreGlobals.Content;
    }

    public virtual void UnloadContent()
    {
      if (this.content != null && this.content != CoreGlobals.Content)
        this.content.Unload();
      this.IsContentLoaded = false;
    }

    public virtual bool InputHandled { get; set; }

    public bool HasWinRects
    {
      get
      {
        if (this.winRects != null)
          return this.winRects.Count > 0;
        return false;
      }
    }

    public Color BackColor
    {
      get
      {
        return this.backColor;
      }
      set
      {
        this.backColor = value;
      }
    }

    public TimeSpan TransitionOnTime
    {
      get
      {
        return this.transitionOnTime;
      }
      set
      {
        this.transitionOnTime = value;
      }
    }

    public TimeSpan TransitionOffTime
    {
      get
      {
        return this.transitionOffTime;
      }
      set
      {
        this.transitionOffTime = value;
      }
    }

    public float TransitionPosition
    {
      get
      {
        return this.transitionPosition;
      }
      protected set
      {
        this.transitionPosition = value;
      }
    }

    public byte TransitionAlpha
    {
      get
      {
        return (byte) ((double) this.TransitionAlphaFloat * (double) byte.MaxValue);
      }
    }

    public float TransitionAlphaFloat
    {
      get
      {
        float num = 0.0f;
        switch (this.screenState)
        {
          case ScreenState.TransitionOn:
          case ScreenState.TransitionOff:
            num = MathHelper.Lerp(0.0f, 1f, this.transitionPosition);
            break;
          case ScreenState.Active:
            num = 1f;
            break;
        }
        return num;
      }
    }

    public ScreenState ScreenState
    {
      get
      {
        return this.screenState;
      }
      protected set
      {
        ScreenState screenState = this.screenState;
        this.screenState = value;
        if (value == screenState)
          return;
        this.OnScreenStateChanged(screenState);
      }
    }

    protected virtual void OnScreenStateChanged(ScreenState oldScreenState)
    {
    }

    public bool IsExiting
    {
      get
      {
        return this.isExiting;
      }
      protected internal set
      {
        this.isExiting = value;
      }
    }

    public bool IsActive
    {
      get
      {
        if (this.otherScreenHasFocus)
          return false;
        if (this.ScreenState != ScreenState.TransitionOn)
          return this.ScreenState == ScreenState.Active;
        return true;
      }
    }

    public ScreenManager ScreenManager { get; set; }

    public SpriteBatchSafe SpriteBatch
    {
      get
      {
        return this.ScreenManager.SpriteBatch;
      }
    }

    public WindowManager WindowManager
    {
      get
      {
        return this.windowManager;
      }
    }

    public PlayerIndex? ControllingPlayer
    {
      get
      {
        return this.controllingPlayer;
      }
      internal set
      {
        this.controllingPlayer = value;
      }
    }

    public void ClearControllingPlayer()
    {
      this.controllingPlayer = new PlayerIndex?();
    }

    public GraphicsDevice GraphicsDevice
    {
      get
      {
        return this.ScreenManager.GraphicsDevice;
      }
    }

    public static void SetToolTips(bool enabled)
    {
      GameScreen.isToolTipsEnabledGlobal = enabled;
    }

    public virtual int FadeBackBufferAlpha
    {
      get
      {
        return (int) this.TransitionAlpha * 2 / 3;
      }
    }

    public bool IsContentLoaded { get; protected set; }

    public virtual int MemorySizeInBytes
    {
      get
      {
        return 144;
      }
    }

    public virtual int MemorySizeInBytesUnmanaged
    {
      get
      {
        return 0;
      }
    }

    public void ExitScreen()
    {
      this.ExitScreen(false);
    }

    public void ExitScreen(bool force)
    {
      if (this.TransitionOffTime == TimeSpan.Zero || force)
      {
        if (this.ScreenManager == null)
          return;
        this.ScreenManager.RemoveScreen(this);
      }
      else
        this.isExiting = true;
    }

    public void OnScreenAdded()
    {
      this.OnScreenAddedCore();
    }

    protected virtual void OnScreenAddedCore()
    {
    }

    public void OnScreenRemoved()
    {
      this.OnScreenRemovedCore();
      if (this.ScreenRemoved == null)
        return;
      this.ScreenRemoved((object) this, EventArgs.Empty);
    }

    protected virtual void OnScreenRemovedCore()
    {
    }

    public void InitializeTransitionOn()
    {
      if (this.UpdateTransition(this.transitionOnTime, 1))
        this.ScreenState = ScreenState.TransitionOn;
      else
        this.ScreenState = ScreenState.Active;
    }

    public void AddWinRect(Rectangle r, EventHandler<EventArgs> handler)
    {
      if (this.winRects == null)
      {
        this.winRects = new List<Rectangle>();
        this.winRectHandlers = new List<EventHandler<EventArgs>>();
      }
      this.winRects.Add(r);
      this.winRectHandlers.Add(handler);
    }

    public virtual bool HandleInput(InputState input)
    {
      return this.HasWinRects && this.HandleWinRectInput() || this.windowManager != null && this.windowManager.IsEnabled && this.windowManager.HandleInput();
    }

    protected virtual bool HandleWinRectInput()
    {
      this.winRectHovered = -1;
      this.winRectPressed = -1;
      PlayerIndex playerIndex = this.controllingPlayer.HasValue ? this.controllingPlayer.Value : PlayerIndex.One;
      bool flag1 = InputManager.IsMouseButtonPressed(playerIndex, StudioForge.Engine.Integration.MouseButtons.LeftButton);
      Point mousePos = InputManager.GetMousePos(playerIndex);
      for (int index = this.winRects.Count - 1; index >= 0 && this.winRectHovered == -1; --index)
      {
        if (this.winRects[index].Contains(mousePos.X, mousePos.Y))
        {
          this.winRectHovered = index;
          if (flag1)
          {
            this.winRectPressed = index;
            break;
          }
          break;
        }
      }
      if (this.winRectHovered < 0)
        return false;
      bool flag2 = this.WinRectClickPress ? InputManager.IsMouseButtonPressedNew(playerIndex, StudioForge.Engine.Integration.MouseButtons.LeftButton) || InputManager.IsButtonPressedNew(playerIndex, Buttons.A) : !flag1 && (InputManager.IsMouseButtonReleasedNew(playerIndex, StudioForge.Engine.Integration.MouseButtons.LeftButton) || InputManager.IsButtonReleasedNew(playerIndex, Buttons.A));
      if (flag2 && this.winRectHandlers[this.winRectHovered] != null)
        this.winRectHandlers[this.winRectHovered]((object) this, EventArgs.Empty);
      if (!flag2)
        return flag1;
      return true;
    }

    public void Update(bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      try
      {
        this.otherScreenHasFocus = otherScreenHasFocus;
        this.UpdateCore(coveredByOtherScreen);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(-1, ex);
      }
    }

    protected virtual void UpdateCore(bool coveredByOtherScreen)
    {
      this.UpdateTransition(coveredByOtherScreen);
    }

    protected void UpdateTransition(bool coveredByOtherScreen)
    {
      if (this.ScreenState == ScreenState.UserHidden)
        return;
      if (this.isExiting)
      {
        this.ScreenState = ScreenState.TransitionOff;
        if (this.UpdateTransition(this.transitionOffTime, -1))
          return;
        this.ScreenManager.RemoveScreen(this);
      }
      else if (coveredByOtherScreen)
      {
        if (this.UpdateTransition(this.transitionOffTime, -1))
          this.ScreenState = ScreenState.TransitionOff;
        else
          this.ScreenState = ScreenState.Hidden;
      }
      else if (this.UpdateTransition(this.transitionOnTime, 1))
      {
        this.ScreenState = ScreenState.TransitionOn;
      }
      else
      {
        this.ScreenState = ScreenState.Active;
        if (this.windowManager == null || !this.windowManager.IsEnabled)
          return;
        this.windowManager.Update();
      }
    }

    private bool UpdateTransition(TimeSpan time, int direction)
    {
      this.transitionPosition += (!(time == TimeSpan.Zero) ? Services.ElapsedTime / (float) time.TotalSeconds : 1f) * (float) direction;
      if ((direction >= 0 || (double) this.transitionPosition > 0.0) && (direction <= 0 || (double) this.transitionPosition < 1.0))
        return true;
      this.transitionPosition = MathHelper.Clamp(this.transitionPosition, 0.0f, 1f);
      return false;
    }

    public void Draw()
    {
      try
      {
        this.DrawCore();
        if (this.HasWinRects)
          this.DrawWinRects();
        if (this.windowManager != null)
        {
          if (this.windowManager.IsEnabled)
          {
            this.windowManager.Alpha = (float) this.TransitionAlpha / (float) byte.MaxValue;
            this.windowManager.Draw();
          }
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(-2, ex);
      }
      ++this.drawFrameCount;
    }

    protected virtual void DrawWinRects()
    {
      this.SpriteBatch.Begin();
      if (CoreGlobals.DebugVerbosity > 1)
      {
        for (int index = 0; index < this.winRects.Count; ++index)
        {
          int thickness = index == this.winRectPressed ? 2 : 1;
          Color color = index == this.winRectPressed || index == this.winRectHovered ? Color.White : Color.White * 0.5f;
          this.SpriteBatch.DrawBox(this.winRects[index], thickness, color, 0.0f);
        }
      }
      else if (this.winRectHovered >= 0 || this.winRectPressed >= 0)
      {
        int thickness = this.winRectPressed >= 0 ? 2 : 1;
        Color white = Color.White;
        this.SpriteBatch.DrawBox(this.winRects[this.winRectPressed >= 0 ? this.winRectPressed : this.winRectHovered], thickness, white, 0.0f);
      }
      this.SpriteBatch.End();
    }

    protected virtual void DrawCore()
    {
    }
  }
}
