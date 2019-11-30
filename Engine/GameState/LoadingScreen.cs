// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.LoadingScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System;
using System.Diagnostics;
using System.Threading;

namespace StudioForge.Engine.GameState
{
  public class LoadingScreen : GameScreen
  {
    private bool loadingIsSlow;
    private GameScreen[] screensToLoad;
    private Thread backgroundThread;
    private EventWaitHandle backgroundThreadExit;
    protected GraphicsDevice graphicsDevice;
    protected IMessageDisplay messageDisplay;
    protected GameTime loadStartTime;
    protected TimeSpan loadAnimationTimer;

    protected virtual bool OtherScreensAreGone(PlayerIndex? controllingPlayer)
    {
      if (this.ScreenState == ScreenState.Active)
        return this.ScreenManager.GetScreens(controllingPlayer).Length == 1;
      return false;
    }

    protected LoadingScreen(
      ScreenManager screenManager,
      bool loadingIsSlow,
      GameScreen[] screensToLoad)
    {
      this.loadingIsSlow = loadingIsSlow;
      this.screensToLoad = screensToLoad;
      this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      if (!loadingIsSlow)
        return;
      this.backgroundThread = new Thread(new ThreadStart(this.BackgroundWorkerThread));
      this.backgroundThreadExit = (EventWaitHandle) new ManualResetEvent(false);
      this.graphicsDevice = screenManager.GraphicsDevice;
      this.messageDisplay = (IMessageDisplay) screenManager.Game.Services.GetService(typeof (IMessageDisplay));
    }

    public static void Load(
      ScreenManager screenManager,
      bool loadingIsSlow,
      PlayerIndex? controllingPlayer,
      params GameScreen[] screensToLoad)
    {
      foreach (GameScreen screen in screenManager.GetScreens(controllingPlayer))
        screen.ExitScreen();
      LoadingScreen loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
      screenManager.AddScreen((GameScreen) loadingScreen, controllingPlayer);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.OtherScreensAreGone(this.ControllingPlayer))
        return;
      if (this.backgroundThread != null)
      {
        this.loadStartTime = new GameTime(TimeSpan.FromSeconds((double) Services.TotalTime), TimeSpan.FromSeconds((double) Services.ElapsedTime));
        this.backgroundThread.Start();
      }
      this.ScreenManager.RemoveScreen((GameScreen) this);
      if (this.screensToLoad != null && this.screensToLoad.Length > 0)
      {
        foreach (GameScreen screen in this.screensToLoad)
          this.ScreenManager.AddScreen(screen, this.ControllingPlayer);
      }
      if (this.backgroundThread != null)
      {
        this.backgroundThreadExit.Set();
        this.backgroundThread.Join();
      }
      this.ScreenManager.Game.ResetElapsedTime();
    }

    protected override void DrawCore()
    {
      if (!this.loadingIsSlow)
        return;
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      SpriteFont gameFont = CoreGlobals.GameFont;
      string text1 = "Loading";
      Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
      Vector2 position = (new Vector2((float) viewport.Width, (float) viewport.Height) - gameFont.MeasureString(text1)) / 2f;
      Color color = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) this.TransitionAlpha);
      this.loadAnimationTimer += TimeSpan.FromSeconds((double) Services.ElapsedTime);
      int count = (int) (this.loadAnimationTimer.TotalSeconds * 5.0) % 10;
      string text2 = text1 + new string('.', count);
      spriteBatch.Begin();
      spriteBatch.DrawString(gameFont, text2, position, color);
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private void BackgroundWorkerThread()
    {
      Stopwatch.GetTimestamp();
      while (!this.backgroundThreadExit.WaitOne(33))
        this.DrawLoadAnimation();
    }

    private GameTime GetGameTime(ref long lastTime)
    {
      long timestamp = Stopwatch.GetTimestamp();
      long num = timestamp - lastTime;
      lastTime = timestamp;
      TimeSpan elapsedGameTime = TimeSpan.FromTicks(num * 10000000L / Stopwatch.Frequency);
      return new GameTime(this.loadStartTime.TotalGameTime + elapsedGameTime, elapsedGameTime);
    }

    private void DrawLoadAnimation()
    {
      if (this.graphicsDevice == null)
        return;
      if (this.graphicsDevice.IsDisposed)
        return;
      try
      {
        this.graphicsDevice.Clear(Color.Black);
        this.Draw();
        IMessageDisplay messageDisplay = this.messageDisplay;
        this.graphicsDevice.Present();
      }
      catch
      {
        this.graphicsDevice = (GraphicsDevice) null;
      }
    }
  }
}
