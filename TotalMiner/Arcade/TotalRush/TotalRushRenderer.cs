// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.TotalRushRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class TotalRushRenderer : IArcadeMachineRenderer, IHasContent
  {
    private string[] RankText = new string[6]
    {
      "Prepare for Hard Battle",
      "Prepare for Harder Battle",
      "Prepare for Expert Battle",
      "Prepare for Insane Battle",
      "Prepare for Godlike Battle",
      "Captain, you are Godlike!"
    };
    public static int MaxBulletColors;
    private SpriteBatchSafe spriteBatch;
    private StudioForge.TotalMiner.Arcade.TotalRush.TotalRush game;
    private Texture2D texture;
    private Color[] bulletColors;
    private Rectangle enemyBulletBorder;
    private Vector2 enemyBulletBorderOrigin;
    private float z;

    public void LoadContent(InitState state)
    {
      this.spriteBatch = CoreGlobals.SpriteBatch;
      this.LoadTexturePack();
      this.bulletColors = new Color[10]
      {
        Color.White,
        Color.Cyan,
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Yellow,
        Color.Purple,
        Color.Orange,
        Color.Pink,
        Color.CornflowerBlue
      };
      TotalRushRenderer.MaxBulletColors = this.bulletColors.Length;
      this.enemyBulletBorder = new Rectangle(35, 0, 5, 5);
      this.enemyBulletBorderOrigin = new Vector2(2.5f, 2.5f);
    }

    public void LoadTexturePack()
    {
      this.texture = CoreGlobals.Content.Load<Texture2D>("Textures\\TotalRushNeon");
    }

    public void UnloadContent()
    {
    }

    public void Draw(ArcadeMachine baseGame)
    {
      this.game = baseGame as StudioForge.TotalMiner.Arcade.TotalRush.TotalRush;
      CoreGlobals.GraphicsDevice.SetRenderTarget(this.game.RenderTarget);
      CoreGlobals.GraphicsDevice.Clear(Color.Black);
      this.z = 0.1f;
      this.spriteBatch.Begin(SpriteSortMode.FrontToBack, (BlendState) null, SamplerState.PointClamp, DepthStencilState.None, (RasterizerState) null);
      switch (this.game.State)
      {
        case StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play:
        case StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition:
          this.DrawParticles();
          this.DrawActors();
          this.DrawBullets();
          this.DrawHud();
          break;
        case StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.ControlsScreen:
          this.DrawControls();
          this.DrawHud();
          break;
        default:
          this.DrawGameOver();
          this.DrawHud();
          break;
      }
      this.spriteBatch.End();
    }

    private void DrawActors()
    {
      Vector2 worldShake = this.game.WorldShake;
      for (LinkedListNode<Point> linkedListNode = this.game.UsedActors.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        Actor actor = this.game.Actors[linkedListNode.Value.X];
        ActorDataXML actorDataXml = this.game.ActorData[(int) actor.ActorType];
        float num = actor.IsDestroyed ? actor.DestroyedAge / actorDataXml.DestructionTime : 1f;
        this.spriteBatch.Draw(this.texture, actor.WorldPosition + worldShake, new Rectangle?(actorDataXml.SrcRect), Color.White * num, actor.Rotation, actorDataXml.Origin, actorDataXml.Scale * actor.Scale, SpriteEffects.None, linkedListNode.Value.X == this.game.PlayerID ? 0.95f : this.z);
        if ((double) this.z < 0.949999988079071)
          this.z += 0.01f;
      }
    }

    private void DrawBullets()
    {
      Vector2 worldShake = this.game.WorldShake;
      for (LinkedListNode<Point> linkedListNode = this.game.UsedBullets.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        Bullet bullet = this.game.Bullets[linkedListNode.Value.X];
        ActorDataXML actorDataXml = this.game.ActorData[(int) bullet.ActorType];
        Color bulletColor = this.bulletColors[bullet.ColorIndex];
        this.spriteBatch.Draw(this.texture, bullet.Position + worldShake, new Rectangle?(actorDataXml.SrcRect), bulletColor, bullet.Rotation, actorDataXml.Origin, bullet.Scale, SpriteEffects.None, 1f);
        if (bullet.ActorType == ActorType.EnemyBullet)
          this.spriteBatch.Draw(this.texture, bullet.Position + worldShake, new Rectangle?(this.enemyBulletBorder), Color.White, bullet.Rotation, this.enemyBulletBorderOrigin, bullet.Scale, SpriteEffects.None, 1f);
      }
    }

    private void DrawParticles()
    {
      Vector2 worldShake = this.game.WorldShake;
      for (LinkedListNode<Point> linkedListNode = this.game.UsedParticles.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        Particle particle = this.game.Particles[linkedListNode.Value.X];
        ParticleDataXML particleDataXml = this.game.ParticleData[(int) particle.ParticleType];
        float num = particle.Age / particle.OrigAge;
        this.spriteBatch.Draw(this.texture, particle.Position + worldShake, new Rectangle?(particleDataXml.SrcRect), particle.Color * num, particle.Rotation, particleDataXml.Origin, particle.Scale, SpriteEffects.None, particleDataXml.LayerDepth);
      }
    }

    private void DrawHud()
    {
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, this.game.ScoreText, new Vector2(4f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreText, new Vector2(160f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      if (!this.game.DrawRankMessage)
        return;
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, this.RankText[Math.Min(this.RankText.Length - 1, this.game.Rank)], 100f, Color.White, 1f);
    }

    private void DrawGameOver()
    {
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion, new Vector2(4f, 10f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer, new Vector2(160f, 10f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Total", 35f, Color.White, 2.2f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Rush", 65f, Color.White, 2.2f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Game Over", 120f, Color.White, 1.6f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, this.game.CreditText, 156f, Color.White, 1f);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, new Rectangle(120, 200, 12, 12));
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Controls", new Vector2(136f, 199f), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Inspired by Score Rush from Xona Games (XBLIG)", 220f, Color.White, 0.5f);
    }

    private void DrawControls()
    {
      int x = 80;
      int y = 25;
      float scale = 0.62f;
      this.spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftStick), new Rectangle(x + 11, y, 14, 14), Color.White);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Move", new Vector2((float) (x + 44), (float) y), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
      this.spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightStick), new Rectangle(x + 11, y + 20, 14, 14), Color.White);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Aim/Fire", new Vector2((float) (x + 44), (float) (y + 20)), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
      this.spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftShoulder), new Rectangle(x, y + 40, 14, 14), Color.White);
      this.spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightShoulder), new Rectangle(x + 20, y + 40, 14, 14), Color.White);
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Smartbomb", new Vector2((float) (x + 44), (float) (y + 40)), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "The small red dot is your hitbox", (float) (y + 70), Color.White, scale);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Collect pickups to gain fire power", (float) (y + 90), Color.White, scale);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Collect pickups to gain tail ships", (float) (y + 110), Color.White, scale);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Smartbombs sacrifice a tail ship", (float) (y + 130), Color.White, scale);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "Tailship replaces Mothership on death", (float) (y + 150), Color.White, scale);
      this.spriteBatch.DrawStringCentered(GraphicStatics.InvadersFont, "You are invulnerable during blast shake", (float) (y + 170), Color.White, scale);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, new Rectangle(124, y + 193, 12, 12));
      this.spriteBatch.DrawString(GraphicStatics.InvadersFont, "Exit", new Vector2(140f, (float) (y + 192)), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
    }
  }
}
