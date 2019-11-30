// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TotalMinerGame
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.Engine.Renderers;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class TotalMinerGame : GameWithScreenManager
  {
    public readonly List<ProgressOperation> Operations = new List<ProgressOperation>();
    public float ElapsedFactor = 1f;
    private const int nextExceptionID = 47;
    public const int MaxLight = 15;
    public const int ExeVersion = 27302;
    public const string Revision = "15.03.18";
    public static TotalMinerGame Instance;
    public static GameInstance GameInstance;
    public string ExitMessage;
    public static int LastOOMCount;
    private AudioManager audioManagerFiles;
    private AudioManagerXACT audioManagerXACT;
    private NetworkManager networkManager;
    private NotificationRenderer notificationRenderer;
    private StudioForge.TotalMiner.Screens.BackgroundScreen bkgdScreen;
    private Thread audioThread;
    private AnimatedTexture gearcog;
    private int evenFrame;

    public IAudioManagerStream AudioManagerFiles
    {
      get
      {
        return (IAudioManagerStream) this.audioManagerFiles;
      }
    }

    public TotalMinerGame()
      : base(true, false, false, DepthFormat.Depth24Stencil8)
    {
      TotalMinerGame.Instance = this;
      this.ScreenManager.TraceEnabled = false;
      this.GameName = "Total Miner.  Please take a photo of this screen and post it on TotalMinerForums.net, so that we can fix it. Thank you.";
      MessageBoxScreen.DefaultTransitionOnTime = MessageBoxScreen.DefaultTransitionOffTime = TimeSpan.Zero;
      InputManager1.Initialize((InputProfile) null);
    }

    protected override void Initialize()
    {
      Map.RLEStreamBufferManager = new RLEStreamBufferManager();
      Map.RLEStreamBufferManager.Initialize(8000000, 4000000);
      this.audioManagerXACT = new AudioManagerXACT("Content\\Audio\\xact.xgs");
      this.audioManagerXACT.Initialize();
      this.AddService<IAudioManager>((IAudioManager) this.audioManagerXACT);
      this.audioManagerFiles = new AudioManager((IServiceProvider) this.Services);
      this.audioManagerFiles.Initialize();
      this.networkManager = new NetworkManager();
      NetworkManager.Instance = this.networkManager;
      this.networkManager.Initialize();
      GraphicStatics.Construct();
      base.Initialize();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      Globals1.FontConsolas = CoreGlobals.Content.Load<SpriteFont>(StudioForge.Engine.Services.FontPath + "Consolas");
      this.ScreenManager.GameFont = CoreGlobals.Content.Load<SpriteFont>(StudioForge.Engine.Services.FontPath + "Default");
      CoreGlobals.ClearReferenceCache();
      CoreGlobals.GameFont = this.ScreenManager.GameFont;
      CoreGlobals.MenuFont = this.ScreenManager.GameFont;
      CoreGlobals.ButtonTextureA = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonA");
      CoreGlobals.ButtonTextureA = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonA");
      CoreGlobals.ButtonTextureB = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonB");
      CoreGlobals.ButtonTextureX = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonX");
      CoreGlobals.ButtonTextureY = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonY");
      CoreGlobals.ButtonTextureLB = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonLB");
      CoreGlobals.ButtonTextureRB = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonRB");
      CoreGlobals.ButtonTextureBack = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonBack");
      CoreGlobals.ButtonTextureStart = CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonStart");
      this.gearcog = new AnimatedTexture("Textures\\gearcog", new Vector2((float) (GraphicStatics.DefaultViewport.Width - 40), 40f), 10, new AnimateTextureCondition(this.ShouldDisplayCog));
      this.gearcog.LoadContent();
      this.audioManagerXACT.LoadContent((InitState) null);
      this.audioManagerXACT.SoundVolume = 0.4f;
      this.audioManagerXACT.MusicVolume = 0.4f;
      this.audioManagerXACT.SongStartedPlaying += new EventHandler(this.OnSongStarted);
      this.audioManagerFiles.LoadContent((InitState) null);
      this.notificationRenderer = new NotificationRenderer();
      this.notificationRenderer.LoadContent((InitState) null);
      this.networkManager.LoadContent();
      GraphicStatics.DefaultViewport = this.GraphicsDevice.Viewport;
      InputManager.PushVirtualMouse();
    }

    public void BackgroundScreenRemoved()
    {
      this.bkgdScreen = (StudioForge.TotalMiner.Screens.BackgroundScreen) null;
    }

    private bool ShouldDisplayCog()
    {
      bool flag = Monitor.TryEnter(Globals1.SaveSemaphore);
      if (flag)
        Monitor.Exit(Globals1.SaveSemaphore);
      return !flag;
    }

    protected override void UnloadContent()
    {
      base.UnloadContent();
      this.networkManager.EndSession(true);
      this.networkManager.UnloadContent();
      if (this.audioThread == null)
        return;
      this.audioThread.Abort();
    }

    public override GameScreen GetNewBackgroundScreen()
    {
      return (GameScreen) (this.bkgdScreen = new StudioForge.TotalMiner.Screens.BackgroundScreen());
    }

    public override GameScreen GetNewMainMenuScreen()
    {
      return (GameScreen) new SplashScreen(this.bkgdScreen);
    }

    public override void CleanupForBluescreen()
    {
      if (NetworkManager.Instance == null)
        return;
      NetworkManager.Instance.EndSession(true);
    }

    protected override IMessageDisplay GetNewMessageDisplayComponent()
    {
      return (IMessageDisplay) new FadeOutMessageRenderer((Microsoft.Xna.Framework.Game) this, "Fonts\\DefaultBold")
      {
        MaxMsgCount = 50
      };
    }

    private void OnSongStarted(object sender, EventArgs e)
    {
      if (!Globals2.GameSettings.HasNotification(NotificationType.Song))
        return;
      TotalMinerGame.Instance.AddNotification("Song: " + this.audioManagerXACT.CurrentCue.Name, true);
    }

    public void UpdateExternal()
    {
      this.Update(new GameTime());
    }

    protected override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      this.networkManager.Update((UpdateState) null);
      GraphicStatics.Update();
      if (++this.evenFrame % 4 == 0)
      {
        this.audioManagerXACT.Update((UpdateState) null);
        this.audioManagerFiles.Update((UpdateState) null);
      }
      CoreGlobals.ThreadException = (Exception) null;
    }

    public void UpdatePokes()
    {
      this.audioManagerXACT.Update((UpdateState) null);
      this.audioManagerFiles.Update((UpdateState) null);
    }

    protected override void SetTime(GameTime gameTime)
    {
        StudioForge.Engine.Services.RealElapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
        StudioForge.Engine.Services.ElapsedTime = StudioForge.Engine.Services.RealElapsedTime * this.ElapsedFactor;
        StudioForge.Engine.Services.TotalTime += StudioForge.Engine.Services.ElapsedTime;
        StudioForge.Engine.Services.IsRunningSlowly = gameTime.IsRunningSlowly;
    }

    protected override void ClearDevice()
    {
      if (StudioForge.TotalMiner.Screens.GameplayScreen.ScreenInstance != null)
        return;
      base.ClearDevice();
    }

    protected override void EndDraw()
    {
      try
      {
        base.EndDraw();
      }
      catch (InvalidOperationException ex)
      {
        StudioForge.Engine.Services.ExceptionReporter.ReportExceptionCaught(35, (Exception) ex);
      }
      catch (ArgumentException ex)
      {
        StudioForge.Engine.Services.ExceptionReporter.ReportExceptionCaught(114, (Exception) ex);
      }
    }

    protected override void DrawCorePostComponents()
    {
      this.notificationRenderer.Draw((DrawState) null);
      this.ScreenManager.SpriteBatch.Begin();
      try
      {
        this.networkManager.Draw();
        this.DrawOperations();
        this.gearcog.Draw(this.ScreenManager.SpriteBatch);
        if (TotalMinerGame.GameInstance != null)
          TotalMinerGame.GameInstance.PostDraw();
      }
      catch (Exception ex)
      {
        StudioForge.Engine.Services.ExceptionReporter.ReportExceptionCaught(92, ex);
      }
      finally
      {
        try
        {
          this.ScreenManager.SpriteBatch.End();
        }
        catch (InvalidOperationException ex)
        {
          this.CreateSpriteBatch();
        }
      }
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private void DrawOperations()
    {
      int operationsStartY = StudioForge.TotalMiner.Screens.GameplayScreen.OperationsStartY;
      for (int index = 0; index < this.Operations.Count; ++index)
      {
        ProgressOperation operation = this.Operations[index];
        string text = (double) operation.Progress <= 0.0 ? operation.Desc + ": Queued" : string.Format("{0}: {1:N0}%", (object) operation.Desc, (object) (float) ((double) operation.Progress * 100.0));
        Vector2 vector2 = CoreGlobals.GameFont.MeasureString(text) * 0.6f;
        this.ScreenManager.SpriteBatch.DrawString(CoreGlobals.GameFont, text, new Vector2((float) (1140 - (int) vector2.X), (float) operationsStartY), Color.Yellow * 0.5f, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        operationsStartY += 24;
      }
      this.Operations.Clear();
    }

    public MessageBoxScreen ShowExceptionMessageBox(
      string heading,
      Exception e,
      PlayerIndex? controllingPlayer)
    {
      return this.ShowExceptionMessageBox((heading + e.Message).Replace(". ", ".\n"), controllingPlayer);
    }

    public MessageBoxScreen ShowExceptionMessageBox(
      string message,
      PlayerIndex? controllingPlayer)
    {
      MessageBoxScreen messageBoxScreen = new MessageBoxScreen(message, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground));
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreen, controllingPlayer);
      return messageBoxScreen;
    }

    private void InitializeGlobalDataThreaded()
    {
      GraphicStatics.Initialize();
    }

    public static void Assert(string desc)
    {
    }

    private void HandleOutOfMemoryError(Exception e)
    {
      if (TotalMinerGame.GameInstance != null)
      {
        GC.Collect();
        TotalMinerGame.GameInstance.UnloadContent();
        GC.Collect();
      }
      this.ScreenManager.ExitAllPlayerScreens(true);
      this.ScreenManager.ExitAllScreens(true);
      this.ShowExceptionMessageBox("", e, new PlayerIndex?()).ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnErrorOk);
    }

    private void OnErrorOk(object sender, EventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new StudioForge.TotalMiner.Screens.BackgroundScreen(), new PlayerIndex?());
      this.ScreenManager.AddScreen((GameScreen) new MainMenuScreen(), new PlayerIndex?());
    }

    public static void ShowNoPermissionScreen(
      ScreenManager screenManager,
      PlayerIndex? controllingPlayer,
      Player player)
    {
      Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("You do not have permission to use this feature.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player);
      screenManager.AddScreen((GameScreen) messageBoxScreenTm, controllingPlayer);
    }

    public void ShowInvalidChoiceScreen(string message, Player player)
    {
      if (this.ScreenManager == null)
        return;
      Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(message, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    public void ShowInvalidChoiceScreen(string message, PlayerIndex playerIndex)
    {
      if (this.ScreenManager == null)
        return;
      Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(message, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), (Player) null), new PlayerIndex?(playerIndex));
    }

    protected override bool ShouldSendError(int i, Exception e)
    {
      if (i == 49)
        return false;
      return !(e is ThreadAbortException);
    }

    public override void AddNotification(string text, bool ignoreGuide)
    {
      if (!this.CanAddNotification(ignoreGuide))
        return;
      this.notificationRenderer.AddNotification(text);
    }

    public override void AddNotification(string text, Color color, bool ignoreGuide)
    {
      if (!this.CanAddNotification(ignoreGuide))
        return;
      this.notificationRenderer.AddNotification(text, color);
    }

    private bool CanAddNotification(bool ignoreGuide)
    {
      if (Globals2.GameSettings.HasNotification(NotificationType.Visual))
        return this.notificationRenderer != null;
      return false;
    }

    public override void AddTestNotification(string text)
    {
    }

    public override void AddErrorNotification(string text)
    {
    }

    protected override void OnActivated(object sender, EventArgs e)
    {
      base.OnActivated(sender, e);
      if (this.audioManagerXACT == null || this.audioManagerXACT.CurrentCue == null)
        return;
      this.audioManagerXACT.CurrentCue.Resume();
    }

    protected override void OnDeactivated(object sender, EventArgs e)
    {
      base.OnDeactivated(sender, e);
      if (this.audioManagerXACT != null && this.audioManagerXACT.CurrentCue != null)
        this.audioManagerXACT.CurrentCue.Pause();
      if (TotalMinerGame.GameInstance == null)
        return;
      TotalMinerGame.GameInstance.PauseGame();
    }

    public void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
    {
      if (TotalMinerGame.GameInstance != null && e.EndReason != NetworkSessionEndReason.ClientSignedOut)
        this.ExitMessage = e.EndReason == NetworkSessionEndReason.Disconnected ? "The game session has been ended" : (e.EndReason == NetworkSessionEndReason.HostEndedSession ? "The game has been ended by the Host" : "You have been kicked from this game by the Host");
      this.ExitBackToMainMenu();
    }

    public void ExitBackToMainMenu()
    {
      GameScreen screen1 = (GameScreen) null;
      StudioForge.TotalMiner.Screens.BackgroundScreen bkgdScreen = new StudioForge.TotalMiner.Screens.BackgroundScreen();
      PlayerIndex? controllingPlayer = new PlayerIndex?();
      GameScreen screen2;
      if (TotalMinerGame.GameInstance == null)
      {
        screen2 = (GameScreen) new SplashScreen(bkgdScreen);
      }
      else
      {
        Globals2.SaveGamertagDataThreaded(Globals2.HighscoreDataChanged, true);
        controllingPlayer = TotalMinerGame.GameInstance.ControllingPlayer;
        screen2 = (GameScreen) new MainMenuScreen();
        if (this.ExitMessage != null)
          screen1 = (GameScreen) new MessageBoxScreen(this.ExitMessage, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground));
      }
      this.ExitMessage = (string) null;
      this.ScreenManager.ExitAllPlayerScreens(true);
      this.ScreenManager.ExitAllScreens(true);
      this.ScreenManager.AddScreen((GameScreen) bkgdScreen, new PlayerIndex?());
      this.ScreenManager.AddScreen(screen2, controllingPlayer);
      if (screen1 == null)
        return;
      this.ScreenManager.AddScreen(screen1, controllingPlayer);
    }
  }
}
