// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Game.GameWithScreenManager
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Game
{
  public abstract class GameWithScreenManager : BaseGame, IGameScreenProvider
  {
    public GameWithScreenManager()
      : this(true, false, false)
    {
    }

    public GameWithScreenManager(bool isFixedTimeStep, bool allowUserResizing, bool isMouseVisible)
      : this(isFixedTimeStep, allowUserResizing, isMouseVisible, DepthFormat.Depth24)
    {
    }

    public GameWithScreenManager(
      bool isFixedTimeStep,
      bool allowUserResizing,
      bool isMouseVisible,
      DepthFormat depthFormat)
      : base(isFixedTimeStep, allowUserResizing, isMouseVisible, depthFormat)
    {
      this.Services.AddService(typeof (IScreenManager), (object) this.ScreenManager);
      this.Services.AddService(typeof (IGameScreenProvider), (object) this);
    }

    protected override void AddCriticalSequenceComponents()
    {
      base.AddCriticalSequenceComponents();
      this.Components.Add((IGameComponent) (this.ScreenManager = new ScreenManager((Microsoft.Xna.Framework.Game) this, this.InputState)));
    }

    protected override void Initialize()
    {
      base.Initialize();
      GameScreen backgroundScreen = this.GetNewBackgroundScreen();
      GameScreen newMainMenuScreen = this.GetNewMainMenuScreen();
      if (backgroundScreen != null)
        this.ScreenManager.AddScreen(backgroundScreen, new PlayerIndex?());
      if (newMainMenuScreen == null)
        return;
      this.ScreenManager.AddScreen(newMainMenuScreen, new PlayerIndex?());
    }

    public virtual GameScreen GetNewBackgroundScreen()
    {
      return (GameScreen) new BackgroundScreen();
    }

    public abstract GameScreen GetNewMainMenuScreen();

    public ScreenManager ScreenManager { get; private set; }

    public override bool RunEvenIfNotActive
    {
      get
      {
        return base.RunEvenIfNotActive;
      }
      set
      {
        base.RunEvenIfNotActive = value;
        this.ScreenManager.RunEvenIfInActive = value;
      }
    }
  }
}
