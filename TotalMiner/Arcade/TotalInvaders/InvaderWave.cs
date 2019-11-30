// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalInvaders.InvaderWave
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalInvaders
{
  internal class InvaderWave
  {
    public Vector2 Position;
    public Vector2 Velocity;
    public Rectangle Box;
    public Invader[] Invaders;
    private StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders instance;
    private bool dropDown;
    private float dropStartY;
    private int invaderCount;
    private float newWavePauseTimer;
    private Vector2 invaderBulletVelocity;

    public InvaderWave(StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders instance)
    {
      this.instance = instance;
      this.Invaders = new Invader[55];
    }

    public void ResetWave()
    {
      this.newWavePauseTimer = 2f;
      for (int index = 0; index < this.Invaders.Length; ++index)
        this.Invaders[index].IsAlive = false;
    }

    private void NewWave()
    {
      this.newWavePauseTimer = 0.0f;
      this.dropDown = false;
      this.Position = new Vector2(0.0f, (float) Math.Min(this.instance.Wave, 30));
      this.Velocity = new Vector2(-MathHelper.Lerp(0.05f, 1f, (float) this.instance.Wave / 30f), 0.1f);
      this.invaderBulletVelocity = new Vector2(0.0f, 1f);
      int index1 = 0;
      for (int index2 = 0; index2 < 97; index2 += 24)
      {
        for (int index3 = 0; index3 < 264; index3 += 24)
        {
          this.Invaders[index1].IsAlive = true;
          this.Invaders[index1++].Position = new Vector2((float) (index3 + 50), (float) (index2 + 30));
        }
      }
      this.invaderCount = index1;
      this.RecalcBoxSize();
      this.instance.FrameCounter = (int) (10.0 / (double) Math.Abs(this.Velocity.X));
      this.instance.AnimFrame = 0;
    }

    private void RecalcBoxSize()
    {
      Vector2 vector2_1 = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 vector2_2 = new Vector2(float.MinValue, float.MinValue);
      foreach (Invader invader in this.Invaders)
      {
        if (invader.IsAlive)
        {
          if ((double) invader.Position.X < (double) vector2_1.X)
            vector2_1.X = invader.Position.X;
          if ((double) invader.Position.Y < (double) vector2_1.Y)
            vector2_1.Y = invader.Position.Y;
          if ((double) invader.Position.X + 16.0 > (double) vector2_2.X)
            vector2_2.X = invader.Position.X + 16f;
          if ((double) invader.Position.Y + 14.0 > (double) vector2_2.Y)
            vector2_2.Y = invader.Position.Y + 14f;
        }
      }
      float x = this.Position.X;
      float y = this.Position.Y;
      this.Position.X += vector2_1.X;
      this.Position.Y += vector2_1.Y;
      this.Box.Width = (int) ((double) vector2_2.X - (double) vector2_1.X);
      this.Box.Height = (int) ((double) vector2_2.Y - (double) vector2_1.Y);
      float num1 = this.Position.X - x;
      float num2 = this.Position.Y - y;
      for (int index = 0; index < this.Invaders.Length; ++index)
      {
        if (this.Invaders[index].IsAlive)
        {
          this.Invaders[index].Position.X -= num1;
          this.Invaders[index].Position.Y -= num2;
        }
      }
    }

    public bool CheckHit(Bullet b)
    {
      for (int index = 0; index < this.Invaders.Length; ++index)
      {
        Invader invader = this.Invaders[index];
        if (invader.IsAlive && (double) b.Position.X <= (double) invader.Position.X + 16.0 - 2.0 + (double) this.Position.X && ((double) b.Position.X + 2.0 > (double) invader.Position.X + (double) this.Position.X && (double) b.Position.Y < (double) invader.Position.Y + 14.0 + (double) this.Position.Y) && (double) b.Position.Y + 10.0 > (double) invader.Position.Y + (double) this.Position.Y)
        {
          this.instance.AddExplosionParticles(this.Position + invader.Position + new Vector2(8f, 7f), this.Velocity * 0.5f, new Color(39, 216, 17), 10);
          this.Invaders[index].IsAlive = false;
          this.RecalcBoxSize();
          this.Velocity.X *= 1.05f;
          this.Velocity.Y *= 1.05f;
          if (--this.invaderCount == 0)
            this.newWavePauseTimer = 2f;
          return true;
        }
      }
      return false;
    }

    public void Update()
    {
      if ((double) this.newWavePauseTimer > 0.0)
      {
        this.newWavePauseTimer -= Services.ElapsedTime;
        if ((double) this.newWavePauseTimer > 0.0)
          return;
        ++this.instance.Wave;
        this.instance.WaveText = "Wave: " + this.instance.Wave.ToString();
        this.NewWave();
      }
      else
      {
        this.CheckForBoundaryBounce();
        this.BombPlayers();
      }
    }

    private void BombPlayers()
    {
      if (this.instance.State != StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play || this.instance.Random.Next(2) != 0)
        return;
      Invader invader = this.Invaders[this.instance.Random.Next(this.Invaders.Length)];
      if (!invader.IsAlive || (double) invader.Position.Y != (double) (this.Box.Height - 14))
        return;
      this.instance.InvaderBullets.Add(new Bullet());
      this.ActivateBullet(invader, this.instance.InvaderBullets.Count - 1);
    }

    private void ActivateBullet(Invader invader, int i)
    {
      Bullet invaderBullet = this.instance.InvaderBullets[i];
      invaderBullet.Position.X = (float) ((double) this.Position.X + (double) invader.Position.X + 8.0 - 8.0);
      invaderBullet.Position.Y = (float) ((double) this.Position.Y + (double) invader.Position.Y + 7.0);
      invaderBullet.Velocity = this.invaderBulletVelocity;
      this.instance.InvaderBullets[i] = invaderBullet;
    }

    private void CheckForBoundaryBounce()
    {
      if (this.dropDown)
      {
        this.Position.Y += this.Velocity.Y;
        if ((double) this.Position.Y > (double) this.dropStartY + 6.0)
          this.dropDown = false;
      }
      else
        this.Position.X += this.Velocity.X;
      this.Box.X = (int) this.Position.X;
      this.Box.Y = (int) this.Position.Y;
      if (this.Box.Y + this.Box.Height >= 230)
      {
        this.instance.GameOver(true);
      }
      else
      {
        if (this.dropDown)
          return;
        if ((double) this.Velocity.X < 0.0)
        {
          if (this.Box.X >= 5)
            return;
          this.Velocity.X = -this.Velocity.X;
          this.dropDown = true;
          this.dropStartY = this.Position.Y;
        }
        else
        {
          if ((double) this.Velocity.X <= 0.0 || this.Box.X + this.Box.Width <= this.instance.ScreenSize.X - 5)
            return;
          this.Velocity.X = -this.Velocity.X;
          this.dropDown = true;
          this.dropStartY = this.Position.Y;
        }
      }
    }
  }
}
