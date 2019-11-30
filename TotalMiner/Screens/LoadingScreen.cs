// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LoadingScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;
using System.IO;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class LoadingScreen : LoadingScreenBase
  {
    private Thread thread;
    private bool loadingFinished;
    private Exception loadingException;

    public LoadingScreen(Player player)
      : base(player)
    {
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      if (this.thread == null || !this.thread.IsAlive)
        return;
      this.thread.Abort();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.IsActive || this.otherScreenHasFocus || coveredByOtherScreen)
        return;
      if (this.thread == null)
      {
        this.StartLoadThread();
      }
      else
      {
        if (!this.loadingFinished)
          return;
        this.LoadingFinished();
      }
    }

    private void StartLoadThread()
    {
      this.thread = new Thread(new ThreadStart(this.LoadGame));
      this.thread.CurrentCulture = Globals1.CultureInfo;
      this.thread.CurrentUICulture = Globals1.CultureInfo;
      this.thread.Start();
      do
        ;
      while (!this.thread.IsAlive);
      Thread.Sleep(1);
    }

    private void LoadingFinished()
    {
      if (this.loadingException != null)
      {
        string heading = "Load Error:\n\n" + Globals1.GetFullExceptionMessageForDisplay(this.loadingException);
        if (this.loadingException is EndOfStreamException)
          heading += "\n\nThis save file is corrupted. It cannot be loaded.\n\nBacking up the Total Miner Data file to USB stick will\nallow you to recover a corrupted save file if it happens again.";
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM(heading, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ContinueAfterError);
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
      }
      else
      {
        this.ScreenManager.ExitAllPlayerScreens(true);
        this.ScreenManager.ExitAllScreens(true);
        this.ScreenManager.AddScreen((GameScreen) new GameplayScreen(this.instance), new PlayerIndex?());
      }
    }

    private void ContinueAfterError(object sender, EventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new MainMenuScreen(), this.ControllingPlayer);
    }

    private void LoadGame()
    {
      try
      {
        this.instance = new GameInstance(this.ControllingPlayer, (IProgressBar) this);
        this.instance.Initialize((InitState) null);
        this.instance.LoadContent((InitState) null);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(42, ex);
        if (this.instance != null)
        {
          this.instance.UnloadContent();
          this.instance = (GameInstance) null;
        }
        this.loadingException = ex;
      }
      this.loadingFinished = true;
    }
  }
}
