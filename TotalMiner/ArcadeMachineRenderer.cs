// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ArcadeMachineRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Arcade;
using StudioForge.TotalMiner.Arcade.TotalInvaders;
using StudioForge.TotalMiner.Arcade.TotalRush;

namespace StudioForge.TotalMiner
{
  internal class ArcadeMachineRenderer : IHasContent, IHasDraw
  {
    private GameInstance instance;

    public IArcadeMachineRenderer RushRenderer { get; private set; }

    public IArcadeMachineRenderer InvadersRenderer { get; private set; }

    public IArcadeMachineRenderer GameSelectorRenderer { get; private set; }

    public bool IsVisible { get; set; }

    public ArcadeMachineRenderer(GameInstance instance)
    {
      this.instance = instance;
    }

    public void LoadContent(InitState state)
    {
      this.InvadersRenderer = (IArcadeMachineRenderer) new TotalInvadersRenderer();
      this.InvadersRenderer.LoadContent(state);
      this.RushRenderer = (IArcadeMachineRenderer) new TotalRushRenderer();
      this.RushRenderer.LoadContent(state);
      this.GameSelectorRenderer = (IArcadeMachineRenderer) new ArcadeGameSelectorRenderer();
      this.GameSelectorRenderer.LoadContent(state);
    }

    public void LoadTexturePack()
    {
      this.RushRenderer.LoadTexturePack();
    }

    public void UnloadContent()
    {
      this.InvadersRenderer.UnloadContent();
      this.RushRenderer.UnloadContent();
      this.GameSelectorRenderer.UnloadContent();
    }

    public void Draw(DrawState state)
    {
    }

    public void Draw(ArcadeMachine machine)
    {
      machine.Renderer.Draw(machine);
    }
  }
}
