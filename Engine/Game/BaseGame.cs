// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Game.BaseGame
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace StudioForge.Engine.Game
{
  public abstract class BaseGame : Microsoft.Xna.Framework.Game, IGuideEvents, ICaughtExceptions, INotificationManager
  {
    private static List<Process> otherProcesses = new List<Process>();
    private static List<FileSystemWatcher> fileWatchers = new List<FileSystemWatcher>();
    public Color BackColor = Color.Black;
    public Dictionary<string, int> CaughtExceptions = new Dictionary<string, int>();
    public static int OOMCount;
    public static bool IsUpdating;
    public FillMode FillMode;
    public string GameName;
    public long LastFrameTicks;
    public long LastUpdateAndDrawTicks;
    public long LastUpdateAndDrawMillisecs;
    protected SafeZoneComponent safeZoneRenderer;
    protected Stopwatch drawStopwatch;
    protected Stopwatch frameStopwatch;
    protected Stopwatch updateStopwatch;
    protected GraphicsDeviceManager graphics;
    private FrameRateCounter fpsCounter;
    private bool runEvenIfNotActive;
    private UpdateState updateState;

    public bool IsCPUBound
    {
      get
      {
        return this.LastUpdateAndDrawTicks > Stopwatch.Frequency / 60L;
      }
    }

    public bool IsGPUBound
    {
      get
      {
        return !this.IsCPUBound;
      }
    }

    public InputState InputState { get; protected set; }

    public static void AddProcess(Process proc)
    {
      BaseGame.otherProcesses.Add(proc);
    }

    public static void RemoveProcess(Process proc)
    {
      BaseGame.otherProcesses.Remove(proc);
    }

    public static void KillProcess(Process proc)
    {
      if (proc == null)
        return;
      if (!proc.HasExited)
      {
        try
        {
          proc.Kill();
        }
        catch (Exception ex)
        {
        }
      }
      BaseGame.RemoveProcess(proc);
    }

    public static void AddFileWatcher(FileSystemWatcher watcher)
    {
      BaseGame.fileWatchers.Add(watcher);
    }

    public static FileSystemWatcher GetFileWatcher(string filename)
    {
      foreach (FileSystemWatcher fileWatcher in BaseGame.fileWatchers)
      {
        if (fileWatcher.Filter == filename)
          return fileWatcher;
      }
      return (FileSystemWatcher) null;
    }

    public static void DisposeFileWatcher(FileSystemWatcher watcher)
    {
      if (watcher == null)
        return;
      watcher.Dispose();
      BaseGame.fileWatchers.Remove(watcher);
    }

    public event EventHandler PreClearDevice;

    public event EventHandler GuideClosed;

    private void OnGuideClosed()
    {
      if (this.GuideClosed == null)
        return;
      this.GuideClosed((object) this, EventArgs.Empty);
    }

    protected virtual void OnClientSizeChanged(object sender, EventArgs e)
    {
    }

    private void OnPreClearDevice()
    {
      if (this.PreClearDevice == null)
        return;
      this.PreClearDevice((object) this, EventArgs.Empty);
    }

    public BaseGame()
      : this(true, false, false)
    {
    }

    public BaseGame(bool isFixedTimeStep, bool allowUserResizing, bool isMouseVisible)
      : this(isFixedTimeStep, allowUserResizing, isMouseVisible, DepthFormat.Depth24)
    {
    }

    public BaseGame(
      bool isFixedTimeStep,
      bool allowUserResizing,
      bool isMouseVisible,
      DepthFormat depthFormat)
    {
      StudioForge.Engine.Services.ExceptionReporter = (ICaughtExceptions) this;
      StudioForge.Engine.Services.Instance = (IServiceProvider) this.Services;
      this.graphics = new GraphicsDeviceManager((Microsoft.Xna.Framework.Game) this);
      this.IsMouseVisible = isMouseVisible;
      this.IsFixedTimeStep = isFixedTimeStep;
      this.graphics.SynchronizeWithVerticalRetrace = this.IsFixedTimeStep;
      this.graphics.PreferMultiSampling = true;
      this.graphics.PreferredDepthStencilFormat = depthFormat;
      this.Window.AllowUserResizing = allowUserResizing;
      using (StreamReader reader = TextFileParser.GetReader("game.ini"))
      {
        this.graphics.PreferredBackBufferWidth = TextFileParser.ReadInt(reader, "DisplayWidth", 1280);
        this.graphics.PreferredBackBufferHeight = TextFileParser.ReadInt(reader, "DisplayHeight", 720);
        this.graphics.IsFullScreen = TextFileParser.ReadBool(reader, "Fullscreen", false);
      }
      this.InputState = new InputState();
      this.fpsCounter = new FrameRateCounter();
      this.safeZoneRenderer = new SafeZoneComponent((Microsoft.Xna.Framework.Game) this);
      IMessageDisplay displayComponent = this.GetNewMessageDisplayComponent();
      this.AddCriticalSequenceComponents();
      this.Components.Add((IGameComponent) this.safeZoneRenderer);
      if (displayComponent is IGameComponent)
      {
        this.Components.Add((IGameComponent) displayComponent);
        if (displayComponent is DrawableGameComponent)
          ((DrawableGameComponent) displayComponent).DrawOrder = int.MaxValue;
      }
      this.Services.AddService(typeof (Microsoft.Xna.Framework.Game), (object) this);
      this.Services.AddService(typeof (INotificationManager), (object) this);
      this.Services.AddService(typeof (GraphicsDeviceManager), (object) this.graphics);
      this.Services.AddService(typeof (IServiceProvider), (object) this.Services);
      this.Services.AddService(typeof (IContentManager), (object) new ContentManager((IServiceProvider) this.Services, "Content"));
      this.Services.RemoveService(typeof (IMessageDisplay));
      this.Services.AddService(typeof (IMessageDisplay), (object) displayComponent);
      this.Services.AddService(typeof (IFrameRateCounter), (object) this.fpsCounter);
      this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.OnClientSizeChanged);
    }

    protected virtual void AddCriticalSequenceComponents()
    {
    }

    protected virtual IMessageDisplay GetNewMessageDisplayComponent()
    {
      return (IMessageDisplay) new FadeOutMessageRenderer((Microsoft.Xna.Framework.Game) this, "GameFont");
    }

    protected override void Initialize()
    {
      Texture2D texture2D = new Texture2D(this.GraphicsDevice, 1, 1);
      texture2D.SetData<Color>(new Color[1]
      {
        Color.White
      });
      this.Services.AddService(typeof (Texture2D), (object) texture2D);
      this.CreateSpriteBatch();
      this.drawStopwatch = new Stopwatch();
      this.frameStopwatch = new Stopwatch();
      this.updateStopwatch = new Stopwatch();
      base.Initialize();
    }

    protected void CreateSpriteBatch()
    {
      this.AddService<SpriteBatchSafe>(new SpriteBatchSafe(this.GraphicsDevice));
      CoreGlobals.ClearReferenceCache();
    }

    public virtual void CleanupForBluescreen()
    {
    }

    public virtual bool RunEvenIfNotActive
    {
      get
      {
        return this.runEvenIfNotActive;
      }
      set
      {
        this.runEvenIfNotActive = value;
      }
    }

    protected override void Update(GameTime gameTime)
    {
      BaseGame.IsUpdating = true;
      this.LastUpdateAndDrawTicks = this.updateStopwatch.ElapsedTicks + this.drawStopwatch.ElapsedTicks;
      this.LastUpdateAndDrawMillisecs = this.updateStopwatch.ElapsedMilliseconds + this.drawStopwatch.ElapsedMilliseconds;
      this.LastFrameTicks = this.frameStopwatch.ElapsedTicks;
      this.frameStopwatch.Reset();
      this.frameStopwatch.Start();
      this.updateStopwatch.Reset();
      this.updateStopwatch.Start();
      if (this.IsActive || this.RunEvenIfNotActive)
      {
        this.SetTime(gameTime);
        if (this.IsActive)
        {
          this.InputState.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
          InputManager.Update();
          if (this.HandleInput())
          {
            this.InputState.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            InputManager.Update();
          }
        }
        this.fpsCounter.Update(this.updateState);
      }
      base.Update(gameTime);
      this.updateStopwatch.Stop();
      BaseGame.IsUpdating = false;
    }

    protected virtual void SetTime(GameTime gameTime)
    {
      StudioForge.Engine.Services.TotalTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
      StudioForge.Engine.Services.ElapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
      StudioForge.Engine.Services.RealElapsedTime = StudioForge.Engine.Services.ElapsedTime;
      StudioForge.Engine.Services.IsRunningSlowly = gameTime.IsRunningSlowly;
    }

    public void ResetTime()
    {
      this.SetTime(new GameTime());
    }

    protected virtual bool HandleInput()
    {
      if (this.InputState.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftAlt) && this.InputState.CurrentKeyboardStates[0].IsKeyDown(Keys.Enter) && this.InputState.LastKeyboardStates[0].IsKeyUp(Keys.Enter))
      {
        this.graphics.ToggleFullScreen();
        return true;
      }
      if (this.InputState.CurrentKeyboardStates[0].IsKeyDown(Keys.F3) && this.InputState.LastKeyboardStates[0].IsKeyUp(Keys.F3))
      {
        this.safeZoneRenderer.Enabled = !this.safeZoneRenderer.Enabled;
        this.safeZoneRenderer.Visible = this.safeZoneRenderer.Enabled;
        return true;
      }
      if (!this.InputState.CurrentKeyboardStates[0].IsKeyDown(Keys.F4) || !this.InputState.LastKeyboardStates[0].IsKeyUp(Keys.F4))
        return false;
      this.FillMode = this.FillMode != FillMode.Solid ? FillMode.Solid : FillMode.WireFrame;
      return true;
    }

    protected override void Draw(GameTime donotuse)
    {
      this.drawStopwatch.Reset();
      this.drawStopwatch.Start();
      if (this.IsActive || this.RunEvenIfNotActive)
      {
        if (!this.IsActive)
          this.Update(donotuse);
        this.OnPreClearDevice();
        this.ClearDevice();
        this.DrawCore();
        base.Draw(donotuse);
        this.DrawCorePostComponents();
        this.fpsCounter.DrawUpdate();
      }
      this.drawStopwatch.Stop();
    }

    protected virtual void DrawCore()
    {
    }

    protected virtual void DrawCorePostComponents()
    {
    }

    protected virtual void ClearDevice()
    {
      this.GraphicsDevice.Clear(this.BackColor);
    }

    public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap)
    {
      return RenderTargetBuilder.CreateRenderTarget(this.GraphicsDevice, width, height, mipMap, this.GraphicsDevice.PresentationParameters.BackBufferFormat, this.GraphicsDevice.PresentationParameters.DepthStencilFormat);
    }

    public void AddService<T>(T o)
    {
      if (this.Services.GetService(typeof (T)) != null)
        this.Services.RemoveService(typeof (T));
      this.Services.AddService(typeof (T), (object) o);
    }

    public static void Run<T>() where T : BaseGame, new()
    {
      T obj = default (T);
      try
      {
        try
        {
          using (obj = new T())
            obj.Run();
        }
        finally
        {
          foreach (Component fileWatcher in BaseGame.fileWatchers)
            fileWatcher.Dispose();
          foreach (Process otherProcess in BaseGame.otherProcesses)
            otherProcess.Kill();
        }
      }
      catch (Exception ex)
      {
        obj.CleanupForBluescreen();
        obj.Exit();
        using (ExceptionGame exceptionGame = new ExceptionGame(ex, (object) obj != null ? obj.GameName : "Unknown Game"))
          exceptionGame.Run();
      }
    }

    public virtual void ReportExceptionCaught(int id, Exception e)
    {
      try
      {
        string stackTraceTrunc = this.GetStackTraceTrunc(e.StackTrace);
        string str1 = e.InnerException != null ? this.GetStackTraceTrunc(e.InnerException.StackTrace) : (string) null;
        string str2 = stackTraceTrunc ?? (string) null;
        string str3 = str2 + (str1 != null ? (str2 != null ? "\ninner: " + str1 : str1) : (string) null);
        string str4 = (e.GetType().ToString() + ": " + e.Message.Substring(0, Math.Min(100, e.Message.Length)) + ": " + str3).Replace('\n', ' ');
        string str5 = id.ToString() + "__" + str4;
        int num = 0;
        if (e is OutOfMemoryException)
          ++BaseGame.OOMCount;
        if (this.ShouldSendError(id, e))
        {
          if (!this.CaughtExceptions.TryGetValue(str5, out num))
          {
            this.CaughtExceptions.Add(str5, 1);
          }
          else
          {
            Dictionary<string, int> caughtExceptions;
            string index;
            (caughtExceptions = this.CaughtExceptions)[index = str5] = caughtExceptions[index] + 1;
          }
        }
        this.AddErrorNotification(str5);
      }
      catch (Exception ex)
      {
      }
    }

    private string GetStackTraceTrunc(string stackTrace)
    {
      string str = (string) null;
      if (stackTrace != null)
      {
        try
        {
          str = stackTrace;
          int num1 = stackTrace.IndexOf('\n');
          int length;
          if (num1 >= 0)
          {
            int num2 = stackTrace.IndexOf('\n', num1 + 1);
            if (num2 >= 0)
            {
              int num3 = stackTrace.IndexOf('\n', num2 + 1);
              if (num3 >= 0)
              {
                int num4 = stackTrace.IndexOf('\n', num3 + 1);
                length = num4 < 0 ? stackTrace.Length : num4 - 1;
              }
              else
                length = stackTrace.Length;
            }
            else
              length = stackTrace.Length;
          }
          else
            length = stackTrace.Length;
          str = stackTrace.Substring(0, length);
        }
        catch (Exception ex)
        {
        }
      }
      return str;
    }

    public virtual void AddNotification(string text, bool ignoreGuide)
    {
    }

    public virtual void AddNotification(string text, Color color, bool ignoreGuide)
    {
    }

    public virtual void AddTestNotification(string text)
    {
      this.AddNotification(text, false);
    }

    public virtual void AddErrorNotification(string text)
    {
      this.AddNotification(text, false);
    }

    protected virtual bool ShouldSendError(int id, Exception e)
    {
      return true;
    }
  }
}
