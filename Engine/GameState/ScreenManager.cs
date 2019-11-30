// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.ScreenManager
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StudioForge.Engine.GameState
{
  public class ScreenManager : DrawableGameComponent, IScreenManager
  {
    private bool traceEnabled = true;
    private float zoom = 1f;
    private List<GameScreen> screensToUpdate = new List<GameScreen>();
    private bool[] screenInputAlreadyHandled = new bool[1];
    public bool RunEvenIfInActive;
    public float MouseScale;
    public Vector2 MouseOrigin;
    public Texture2D mouseTexture;
    private List<GameScreen>[] screens;
    private GameScreen[] lastActiveScreen;
    private SpriteBatchSafe spriteBatch;
    private InputState input;
    private bool isInitialized;
    private Viewport[] viewports;

    public int MemorySizeInBytes
    {
      get
      {
        int num = 0;
        if (this.screens != null)
        {
          foreach (List<GameScreen> screen in this.screens)
          {
            if (screen != null)
            {
              for (int index = screen.Count - 1; index >= 0; --index)
              {
                if (screen[index] != null)
                  num += screen[index].MemorySizeInBytes;
              }
            }
          }
        }
        return num;
      }
    }

    public virtual int MemorySizeInBytesUnmanaged
    {
      get
      {
        int num = 0;
        foreach (List<GameScreen> screen in this.screens)
        {
          for (int index = screen.Count - 1; index >= 0; --index)
            num += screen[index].MemorySizeInBytesUnmanaged;
        }
        return num;
      }
    }

    public ScreenManager(Microsoft.Xna.Framework.Game game, InputState input)
      : base(game)
    {
      this.input = input;
    }

    public override void Initialize()
    {
      this.isInitialized = true;
      this.screens = new List<GameScreen>[2];
      this.viewports = new Viewport[2];
      for (int index = 0; index < this.screens.Length; ++index)
        this.screens[index] = new List<GameScreen>();
      this.lastActiveScreen = new GameScreen[2];
      base.Initialize();
    }

    protected override void LoadContent()
    {
      IContentManager content = CoreGlobals.Content;
      this.spriteBatch = CoreGlobals.SpriteBatch;
      this.BlankTexture = CoreGlobals.BlankTexture;
      this.GameFont = content.Load<SpriteFont>(Services.FontPath + "GameFont");
      this.mouseTexture = content.Load<Texture2D>(Services.ScreenManagerPath + "Cursor");
      this.MouseScale = 1f;
      this.MouseOrigin = new Vector2((float) (this.mouseTexture.Width / 2), (float) (this.mouseTexture.Height / 2));
      for (int index = 0; index < this.screens.Length; ++index)
      {
        this.viewports[index] = this.GraphicsDevice.Viewport;
        foreach (GameScreen gameScreen in this.screens[index])
          gameScreen.LoadContent();
      }
    }

    protected override void UnloadContent()
    {
      lock (this.screens)
      {
        for (int index = 0; index < this.screens.Length; ++index)
        {
          foreach (GameScreen gameScreen in this.screens[index])
            gameScreen.UnloadContent();
        }
      }
    }

    public void SetViewport(PlayerIndex? controllingPlayer, Viewport vp)
    {
      this.viewports[this.GetIndex(controllingPlayer)] = vp;
    }

    public Texture2D BlankTexture { get; private set; }

    public Point SafeZone
    {
      get
      {
        Viewport viewport = this.GraphicsDevice.Viewport;
        return new Point(viewport.Width / 100, viewport.Height / 100);
      }
    }

    public Point Center
    {
      get
      {
        Viewport viewport = this.GraphicsDevice.Viewport;
        return new Point(viewport.Width / 2, viewport.Height / 2);
      }
    }

    public float Zoom
    {
      get
      {
        return this.zoom;
      }
      set
      {
        this.zoom = value;
      }
    }

    public SpriteBatchSafe SpriteBatch
    {
      get
      {
        return this.spriteBatch;
      }
    }

    public SpriteFont GameFont { get; set; }

    public bool TraceEnabled
    {
      get
      {
        return this.traceEnabled;
      }
      set
      {
        this.traceEnabled = value;
      }
    }

    public InputState InputState
    {
      get
      {
        return this.input;
      }
    }

    public int ScreenCount(PlayerIndex? controllingPlayer)
    {
      lock (this.screens)
        return this.screens[this.GetIndex(controllingPlayer)].Count;
    }

    public bool IsScreenInputAlreadyHandled(PlayerIndex? controllingPlayer)
    {
      if (!controllingPlayer.HasValue)
        return false;
      return this.screenInputAlreadyHandled[(int) controllingPlayer.Value];
    }

    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
      lock (this.screens)
      {
        if (!this.AddScreenInternal(screen, controllingPlayer))
          return;
        if (!this.screens[this.GetIndex(controllingPlayer)].Contains(screen))
          this.screens[this.GetIndex(controllingPlayer)].Add(screen);
        screen.InitializeTransitionOn();
        screen.OnScreenAdded();
      }
    }

    private int GetIndex(PlayerIndex? controllingPlayer)
    {
      if (!controllingPlayer.HasValue)
        return 0;
      return (int) (controllingPlayer.Value + 1);
    }

    private int GetIndex(GameScreen screen)
    {
      return this.GetIndex(screen.ControllingPlayer);
    }

    public void InsertScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
      lock (this.screens)
      {
        if (!this.AddScreenInternal(screen, controllingPlayer))
          return;
        int num = this.screens[this.GetIndex(controllingPlayer)].IndexOf(screen);
        if (num > 0)
          this.screens[this.GetIndex(controllingPlayer)].Remove(screen);
        if (num != 0)
          this.screens[this.GetIndex(controllingPlayer)].Insert(0, screen);
        screen.OnScreenAdded();
        screen.InitializeTransitionOn();
      }
    }

    private bool AddScreenInternal(GameScreen screen, PlayerIndex? controllingPlayer)
    {
      screen.ControllingPlayer = controllingPlayer;
      screen.ScreenManager = this;
      screen.IsExiting = false;
      if (this.isInitialized)
      {
        if (!screen.IsContentLoaded)
        {
          try
          {
            screen.LoadContent();
          }
          catch (OutOfMemoryException ex)
          {
            screen.UnloadContent();
            GC.Collect();
            return false;
          }
        }
      }
      return true;
    }

    public void AddScreens(GameScreen[] screens, PlayerIndex? controllingPlayer)
    {
      foreach (GameScreen screen in screens)
        this.AddScreen(screen, controllingPlayer);
    }

    public void RemoveScreen(GameScreen screen)
    {
      if (screen == null)
        return;
      if (this.isInitialized)
      {
        screen.OnScreenRemoved();
        screen.UnloadContent();
      }
      lock (this.screens)
      {
        int index = this.GetIndex(screen);
        this.screens[index].Remove(screen);
        if (this.lastActiveScreen[index] != screen)
          return;
        this.lastActiveScreen[index] = (GameScreen) null;
      }
    }

    public void RemoveScreens(PlayerIndex playerIndex)
    {
      this.RemoveScreens(playerIndex, (ScreenManager.RemoveScreenCondition) null);
    }

    public void RemoveScreens(
      PlayerIndex playerIndex,
      ScreenManager.RemoveScreenCondition condition)
    {
      lock (this.screens)
      {
        int index1 = this.GetIndex(new PlayerIndex?(playerIndex));
        for (int index2 = this.screens[index1].Count - 1; index2 >= 0; --index2)
        {
          GameScreen screen = this.screens[index1][index2];
          if (screen != null && (condition == null || condition(screen)))
            this.RemoveScreen(screen);
        }
      }
    }

    public void ExitAllScreens(bool force)
    {
      lock (this.screens)
      {
        for (int i = 0; i < this.screens.Length; ++i)
        {
          this.ExitAllScreens(i, force);
          if (i > 0)
            this.viewports[i] = this.viewports[0];
        }
        this.screensToUpdate.Clear();
      }
    }

    private void ExitAllScreens(int i, bool force)
    {
      lock (this.screens)
      {
        for (int index = this.screens[i].Count - 1; index >= 0; index = index - (index - this.screens[i].Count) - 1)
          this.screens[i][index].ExitScreen(force);
      }
    }

    public void ExitAllPlayerScreens()
    {
      this.ExitAllPlayerScreens(false);
    }

    public void ExitAllPlayerScreens(PlayerIndex? playerIndex)
    {
      this.ExitAllScreens(this.GetIndex(playerIndex), false);
    }

    public void ExitAllPlayerScreens(bool force)
    {
      lock (this.screens)
      {
        for (int i = 1; i < this.screens.Length; ++i)
          this.ExitAllScreens(i, force);
      }
    }

    public void ExitAllScreensBackTo(GameScreen screen)
    {
      lock (this.screens)
      {
        for (int index1 = 0; index1 < this.screens.Length; ++index1)
        {
          for (int index2 = this.screens[index1].Count - 1; index2 >= 0 && this.screens[index1][index2] != screen; --index2)
            this.screens[index1][index2].ExitScreen();
        }
      }
    }

    public GameScreen[] GetScreens(PlayerIndex? controllingPlayer)
    {
      lock (this.screens)
        return this.screens[this.GetIndex(controllingPlayer)].ToArray();
    }

    public int GetScreenCount(PlayerIndex? controllingPlayer)
    {
      lock (this.screens)
        return this.screens[this.GetIndex(controllingPlayer)].Count;
    }

    public void FadeBackBufferToBlack(int alpha)
    {
      if (alpha <= 0)
        return;
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.spriteBatch.Begin();
      this.spriteBatch.Draw(this.BlankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, (int) (byte) alpha));
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    public override void Update(GameTime donotuse)
    {
      bool otherScreenHasFocus = !this.Game.IsActive && !this.RunEvenIfInActive;
      for (int index = 0; index < 1; ++index)
        this.screenInputAlreadyHandled[index] = false;
      lock (this.screens)
      {
        for (int index1 = 1; index1 < 2; ++index1)
        {
          if (this.screens[index1].Count > 0)
          {
            this.Update(index1, otherScreenHasFocus);
            int index2 = index1 - 1;
            if (index2 < this.screenInputAlreadyHandled.Length)
              this.screenInputAlreadyHandled[index2] = true;
          }
        }
        if (this.screens.Length <= 0 || this.screens[0] == null || this.screens[0].Count <= 0)
          return;
        this.Update(0, otherScreenHasFocus);
      }
    }

    private bool Update(int index, bool otherScreenHasFocus)
    {
      this.screensToUpdate.Clear();
      for (int index1 = 0; index1 < this.screens[index].Count; ++index1)
        this.screensToUpdate.Add(this.screens[index][index1]);
      bool coveredByOtherScreen = false;
      while (this.screensToUpdate.Count > 0)
      {
        GameScreen gameScreen = this.screensToUpdate[this.screensToUpdate.Count - 1];
        this.screensToUpdate.RemoveAt(this.screensToUpdate.Count - 1);
        bool otherScreenHasFocus1 = otherScreenHasFocus;
        if ((gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active) && (!otherScreenHasFocus && this.Game.IsActive))
        {
          otherScreenHasFocus = true;
          gameScreen.InputHandled = gameScreen.HandleInput(this.input);
          this.lastActiveScreen[index] = gameScreen;
        }
        gameScreen.Update(otherScreenHasFocus1, coveredByOtherScreen);
        if (!gameScreen.IsPopup)
          coveredByOtherScreen = true;
      }
      return otherScreenHasFocus;
    }

    public GameScreen GetTopScreen(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
      {
        int index = this.GetIndex(controllingPlayer);
        if (this.screens[index].Count > 0)
          return this.screens[index][this.screens[index].Count - 1];
      }
      else
      {
        for (int index = 1; index < 5; ++index)
        {
          if (this.screens[index].Count > 0)
            return this.screens[index][this.screens[index].Count - 1];
        }
      }
      int index1 = 0;
      if (this.screens[index1].Count <= 0)
        return (GameScreen) null;
      return this.screens[index1][this.screens[index1].Count - 1];
    }

    public GameScreen GetTopActiveScreen(PlayerIndex? controllingPlayer)
    {
      if (controllingPlayer.HasValue)
      {
        GameScreen gameScreen = this.lastActiveScreen[this.GetIndex(controllingPlayer)];
        if (gameScreen != null)
          return gameScreen;
      }
      else
      {
        GameScreen gameScreen1 = this.lastActiveScreen[1];
        if (gameScreen1 != null)
          return gameScreen1;
        GameScreen gameScreen2 = this.lastActiveScreen[2];
        if (gameScreen2 != null)
          return gameScreen2;
        GameScreen gameScreen3 = this.lastActiveScreen[3];
        if (gameScreen3 != null)
          return gameScreen3;
        GameScreen gameScreen4 = this.lastActiveScreen[4];
        if (gameScreen4 != null)
          return gameScreen4;
      }
      return this.lastActiveScreen[0];
    }

    public override void Draw(GameTime donotuse)
    {
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.Draw(false);
      this.Draw(true);
      this.SetViewport(viewport);
    }

    private void Draw(bool msgbox)
    {
      lock (this.screens)
      {
        for (int index1 = 0; index1 < this.screens.Length; ++index1)
        {
          if (this.screens[index1].Count > 0)
          {
            this.SetViewport(this.viewports[index1]);
            for (int index2 = 0; index2 < this.screens[index1].Count; ++index2)
            {
              GameScreen gameScreen = this.screens[index1][index2];
              if (gameScreen.ScreenState != ScreenState.Hidden && gameScreen.ScreenState != ScreenState.UserHidden && (msgbox && gameScreen is MessageBoxScreen || !msgbox && !(gameScreen is MessageBoxScreen)))
              {
                if (gameScreen.IsPopup && gameScreen.FadeBehindIfPopup)
                  this.FadeBackBufferToBlack(gameScreen.FadeBackBufferAlpha);
                gameScreen.Draw();
                if (index2 == this.screens[index1].Count - 1 && InputManager.UseVirtualMouse)
                {
                  this.spriteBatch.Begin();
                  Point mousePos = InputManager.GetMousePos(gameScreen.ControllingPlayer.HasValue ? gameScreen.ControllingPlayer.Value : PlayerIndex.One);
                  this.spriteBatch.Draw(this.mouseTexture, new Vector2((float) mousePos.X, (float) mousePos.Y), new Rectangle?(), Color.White, 0.0f, this.MouseOrigin, this.MouseScale, SpriteEffects.None, 0.0f);
                  this.spriteBatch.End();
                }
              }
            }
          }
        }
      }
    }

    private void SetViewport(Viewport vp)
    {
      try
      {
        this.GraphicsDevice.Viewport = vp;
      }
      catch (ArgumentException ex)
      {
        this.FixViewport();
      }
    }

    private void FixViewport()
    {
      int num1 = 1280;
      int num2 = 720;
      Viewport viewport = this.GraphicsDevice.Viewport;
      RenderTargetBinding[] renderTargets = this.GraphicsDevice.GetRenderTargets();
      if (renderTargets != null && renderTargets.Length > 0)
      {
        Texture2D renderTarget = renderTargets[0].RenderTarget as Texture2D;
        if (renderTarget != null)
        {
          num1 = renderTarget.Width;
          num2 = renderTarget.Height;
        }
      }
      viewport.X = 0;
      viewport.Y = 0;
      viewport.Width = num1;
      viewport.Height = num2;
      viewport.MinDepth = 0.0f;
      viewport.MaxDepth = 1f;
      this.GraphicsDevice.Viewport = viewport;
    }

    [SpecialName]
    GraphicsDevice get_GraphicsDevice()
    {
      return this.GraphicsDevice;
    }

    public delegate bool RemoveScreenCondition(GameScreen screen);
  }
}
