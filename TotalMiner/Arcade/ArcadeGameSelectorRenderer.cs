// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.ArcadeGameSelectorRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Arcade
{
  internal class ArcadeGameSelectorRenderer : IArcadeMachineRenderer, IHasContent
  {
    private SpriteBatchSafe spriteBatch;
    private ArcadeGameSelector game;

    public void LoadContent(InitState state)
    {
      this.spriteBatch = CoreGlobals.SpriteBatch;
    }

    public void UnloadContent()
    {
    }

    public void LoadTexturePack()
    {
    }

    public void Draw(ArcadeMachine baseGame)
    {
      this.game = baseGame as ArcadeGameSelector;
      CoreGlobals.GraphicsDevice.SetRenderTarget(this.game.RenderTarget);
      CoreGlobals.GraphicsDevice.Clear(Color.Black);
      this.spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, SamplerState.PointClamp, DepthStencilState.None, (RasterizerState) null);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Total Miner", 30f, Color.White, 2f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Arcade", 60f, Color.White, 2f);
      if (this.game.CanSetGame)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, new Rectangle(100, 160, 20, 20));
        this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Set Game", new Vector2(126f, 160f), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      }
      else
        this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "No Game Selected", 160f, Color.White, 1f);
      this.spriteBatch.End();
    }
  }
}
