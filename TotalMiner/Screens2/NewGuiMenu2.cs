// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.NewGuiMenu2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.API;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class NewGuiMenu2 : NewGuiMenu
  {
    protected GameInstance instance;
    protected Player player;
    protected ScreenManager screenManager;
    protected PauseMenuScreen2 parentScreen;

    public GameInstance Instance
    {
      get
      {
        return this.instance;
      }
    }

    public Player Player
    {
      get
      {
        return this.player;
      }
    }

    protected bool IsGodOrTester
    {
      get
      {
        if (this.player == null)
          return false;
        return this.player.IsGodOrTester;
      }
    }

    public NewGuiMenu2(GameInstance instance, Player player)
      : base((ITMGame) instance, (ITMPlayer) player)
    {
      this.instance = instance;
      this.player = player;
      this.playerIndex = player.PlayerIndex;
      this.containerColor = Color.Transparent;
    }

    public void Open(
      PauseMenuScreen2 parentScreen,
      NewGuiMenu prevOpen,
      TabData tabData,
      Color backColor)
    {
      this.parentScreen = parentScreen;
      this.screenManager = parentScreen.ScreenManager;
      this.Open(parentScreen.WindowManager, parentScreen.ScreenRect, parentScreen.BackTexture, prevOpen, new Action(((GameScreen) parentScreen).ExitScreen), tabData, backColor);
    }
  }
}
