// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Program
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using System;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal static class Program
  {
    public static int MainThreadID;

    [STAThread]
    private static void Main(string[] args)
    {
      Program.MainThreadID = Thread.CurrentThread.ManagedThreadId;
      Thread.CurrentThread.Priority = ThreadPriority.Highest;
      if (!Globals2.Steam())
        return;
      Program.SetStatics();
      BaseGame.Run<TotalMinerGame>();
    }

    private static void SetStatics()
    {
      Services.FontPath = "Fonts\\";
      Services.ScreenManagerPath = "Textures\\";
      AudioManager.RootSoundsDirectory = "Audio\\Effects\\";
      AudioManager.RootSongsDirectory = "Audio\\Music\\";
      MenuScreen.DefaultTitleScale = 1f;
      GameScreen.DefaultTransitionOnTime = TimeSpan.Zero;
      GameScreen.DefaultTransitionOffTime = TimeSpan.Zero;
      ToolTip.BackColor = Color.Black * 0.7f;
      ToolTip.BorderColor = Color.White;
      ToolTip.TextColor = Color.White;
      MessageBoxScreen.DefaultTransitionOffTime = TimeSpan.Zero;
      MessageBoxScreen.DefaultFadeToBlack = 0.3f;
      Thread.CurrentThread.CurrentCulture = Globals1.CultureInfo;
      Thread.CurrentThread.CurrentUICulture = Globals1.CultureInfo;
    }
  }
}
