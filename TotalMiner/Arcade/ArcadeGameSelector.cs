// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.ArcadeGameSelector
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Screens;

namespace StudioForge.TotalMiner.Arcade
{
  internal class ArcadeGameSelector : ArcadeMachine
  {
    public Point ScreenSize;
    private GameInstance instance;
    private int gameCount;

    public override bool CanDeactivate
    {
      get
      {
        return true;
      }
    }

    public bool CanSetGame
    {
      get
      {
        return this.tmPlayer.HasPermissionAny(Permissions.Creative | Permissions.Admin);
      }
    }

    public ArcadeGameSelector(
      GameInstance instance,
      ITMMap map,
      Player player,
      GlobalPoint3D point,
      BlockFace face)
      : base((ITMGame) instance, map, (ITMPlayer) player, point, face)
    {
      this.instance = instance;
      this.gameCount = 2;
      foreach (Mod activeMod in ModManager.ActiveMods)
        this.gameCount += (int) activeMod.TypeCounts.ArcadeMachine;
    }

    public override void LoadContent(InitState state)
    {
      base.LoadContent(state);
      this.ScreenSize = new Point(this.renderTarget.Width, this.renderTarget.Height);
    }

    public override bool HandleInput()
    {
      if (!InputManager1.IsInputReleasedNew(this.tmPlayer.PlayerIndex, GuiInput.SelectItem))
        return InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, GuiInput.SelectItem);
      if (this.CanSetGame)
      {
        GameInstance game = this.game as GameInstance;
        if (game != null)
        {
          BlockSelectionScreen blockSelectionScreen = new BlockSelectionScreen(game, this.tmPlayer as Player, new SelectBlockCallBack(this.OnGameSelected), "Select Game", BlockSelectMode.SelectingArcadeGame, Block.ArcadeMachine, 0);
          game.AddScreen((GameScreen) blockSelectionScreen, this.tmPlayer as Player);
        }
      }
      return true;
    }

    private bool OnGameSelected(Player player, Block textureID)
    {
      int num1 = (int) textureID;
      if (num1 < 1 || num1 > this.gameCount)
        return false;
      int num2 = (int) this.instance.Map.ChangeBlockTexture(player, this.Point, Block.ArcadeMachine, textureID);
      this.instance.ResetArcadeMachine(player, this.Point, this.Face)?.StartGame();
      return true;
    }

    public override void Update()
    {
      int num = this.tmPlayer.IsInputEnabled ? 1 : 0;
    }

    public override void StartGame()
    {
    }
  }
}
