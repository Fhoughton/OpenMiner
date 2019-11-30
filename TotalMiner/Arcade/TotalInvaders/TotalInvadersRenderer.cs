// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvadersRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Arcade.TotalInvaders
{
  internal class TotalInvadersRenderer : IArcadeMachineRenderer, IHasContent
  {
    private SpriteBatchSafe spriteBatch;
    private StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders game;
    private Texture2D texture;

    public void LoadContent(InitState state)
    {
      this.spriteBatch = CoreGlobals.SpriteBatch;
      this.texture = CoreGlobals.Content.Load<Texture2D>("Textures\\TotalInvaders");
    }

    public void UnloadContent()
    {
    }

    public void LoadTexturePack()
    {
    }

    public void Draw(ArcadeMachine baseGame)
    {
      this.game = baseGame as StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders;
      CoreGlobals.GraphicsDevice.SetRenderTarget(this.game.RenderTarget);
      CoreGlobals.GraphicsDevice.Clear(Color.Black);
      this.spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, SamplerState.PointClamp, DepthStencilState.None, (RasterizerState) null);
      if (this.game.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOver)
      {
        this.DrawGameOver();
        this.DrawHud();
      }
      else
      {
        this.DrawWave();
        this.DrawPlayer();
        this.DrawBullets();
        this.DrawPickups();
        this.DrawParticles();
        this.DrawHud();
      }
      this.spriteBatch.End();
    }

    private void DrawWave()
    {
      int num = this.game.AnimFrame % 2 == 0 ? 0 : 15;
      Rectangle destinationRectangle = new Rectangle()
      {
        Width = 16,
        Height = 14
      };
      Rectangle rectangle = new Rectangle()
      {
        X = 0,
        Y = num,
        Width = 16,
        Height = 14
      };
      InvaderWave invaderWave = this.game.InvaderWave;
      foreach (Invader invader in invaderWave.Invaders)
      {
        if (invader.IsAlive)
        {
          destinationRectangle.X = (int) ((double) invader.Position.X + (double) invaderWave.Position.X);
          destinationRectangle.Y = (int) ((double) invader.Position.Y + (double) invaderWave.Position.Y);
          this.spriteBatch.Draw(this.texture, destinationRectangle, new Rectangle?(rectangle), Color.White);
        }
      }
      if (!this.game.UFO.IsAlive)
        return;
      destinationRectangle.X = (int) this.game.UFO.Position.X;
      destinationRectangle.Y = (int) this.game.UFO.Position.Y;
      destinationRectangle.Width = 22;
      destinationRectangle.Height = 8;
      rectangle.X = 17;
      rectangle.Y = 13;
      rectangle.Width = 22;
      rectangle.Height = 8;
      this.spriteBatch.Draw(this.texture, destinationRectangle, new Rectangle?(rectangle), Color.White);
    }

    private void DrawPlayer()
    {
      Rectangle destinationRectangle = new Rectangle()
      {
        Width = 18,
        Height = 12
      };
      Rectangle rectangle = new Rectangle()
      {
        X = 17,
        Y = 0,
        Width = 18,
        Height = 12
      };
      for (int index = 0; index < this.game.Players.Count; ++index)
      {
        destinationRectangle.X = (int) this.game.Players[index].Position.X;
        destinationRectangle.Y = (int) this.game.Players[index].Position.Y;
        this.spriteBatch.Draw(this.texture, destinationRectangle, new Rectangle?(rectangle), Color.White);
      }
    }

    private void DrawBullets()
    {
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.Width = 2;
      destinationRectangle.Height = 10;
      foreach (Bullet playerBullet in this.game.PlayerBullets)
      {
        destinationRectangle.X = (int) playerBullet.Position.X;
        destinationRectangle.Y = (int) playerBullet.Position.Y;
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.White);
      }
      foreach (Bullet invaderBullet in this.game.InvaderBullets)
      {
        destinationRectangle.X = (int) invaderBullet.Position.X;
        destinationRectangle.Y = (int) invaderBullet.Position.Y;
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.White);
      }
    }

    private void DrawPickups()
    {
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.Width = 8;
      destinationRectangle.Height = 6;
      Rectangle rectangle = new Rectangle()
      {
        X = 17,
        Y = 22,
        Width = 8,
        Height = 6
      };
      foreach (Pickup pickup in this.game.Pickups)
      {
        destinationRectangle.X = (int) pickup.Position.X;
        destinationRectangle.Y = (int) pickup.Position.Y;
        this.spriteBatch.Draw(this.texture, destinationRectangle, new Rectangle?(rectangle), Color.White);
      }
    }

    private void DrawParticles()
    {
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.Width = 2;
      destinationRectangle.Height = 2;
      foreach (Particle particle in this.game.Particles)
      {
        destinationRectangle.X = (int) particle.Position.X;
        destinationRectangle.Y = (int) particle.Position.Y;
        float num = (double) particle.Age < 0.5 ? particle.Age * 2f : 1f;
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, particle.Color * num);
      }
    }

    private void DrawHud()
    {
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, this.game.ScoreText, new Vector2(4f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreText, new Vector2(105f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, this.game.WaveText, new Vector2(250f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
    }

    private void DrawGameOver()
    {
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion, new Vector2(4f, 10f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer, new Vector2(105f, 10f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Total", 45f, Color.White, 2.2f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Invaders", 75f, Color.White, 2.2f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Game Over", 140f, Color.White, 1.6f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, this.game.CreditText, 180f, Color.White, 1f);
    }
  }
}
