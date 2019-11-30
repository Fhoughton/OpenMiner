// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.GameplayScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

namespace StudioForge.Engine.GameState
{
  public abstract class GameplayScreen : GameScreen
  {
    public override void LoadContent()
    {
      base.LoadContent();
      this.ScreenManager.Game.ResetElapsedTime();
    }

    protected override void DrawCore()
    {
      if ((double) this.TransitionPosition <= 0.0)
        return;
      this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue - (int) this.TransitionAlpha);
    }
  }
}
