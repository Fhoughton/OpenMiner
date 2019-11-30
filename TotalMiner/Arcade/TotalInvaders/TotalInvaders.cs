// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Arcade.TotalInvaders
{
  internal class TotalInvaders : ArcadeMachine
  {
    public static int HighScore = 0;
    public static string HighScoreGamer = "";
    public static string HighScoreVersion = "";
    private Color[] explosionColors = new Color[3]
    {
      Color.Red,
      Color.Yellow,
      Color.Orange
    };
    public const int InvaderWidth = 16;
    public const int InvaderHeight = 14;
    public const int PlayerWidth = 18;
    public const int PlayerHeight = 12;
    public const int UFOWidth = 22;
    public const int UFOHeight = 8;
    public const int BulletWidth = 2;
    public const int BulletHeight = 10;
    public const int PickupWidth = 8;
    public const int PickupHeight = 6;
    public const int ParticleWidth = 1;
    public const int ParticleHeight = 1;
    public static string HighScoreText;
    public StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState State;
    public InvaderWave InvaderWave;
    public Point ScreenSize;
    public int Score;
    public int Lives;
    public int Wave;
    public List<Player> Players;
    public List<Bullet> PlayerBullets;
    public List<Bullet> InvaderBullets;
    public List<Pickup> Pickups;
    public List<Particle> Particles;
    public UFO UFO;
    public PcgRandom Random;
    public int FrameCounter;
    public int AnimFrame;
    public string ScoreText;
    public string WaveText;
    private GameInstance instance;
    private float ufoSpeed;
    private float pickupSpeed;
    private float gameOverTransitionTimer;
    private bool highscoreGamerUpdated;
    private GamePadState lastpad;

    public override bool CanDeactivate
    {
      get
      {
        return this.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOver;
      }
    }

    public TotalInvaders(
      GameInstance instance,
      ITMMap map,
      StudioForge.TotalMiner.Player player,
      GlobalPoint3D point,
      BlockFace face)
      : base((ITMGame) instance, map, (ITMPlayer) player, point, face)
    {
      this.instance = instance;
      this.GpPerCredit = 1;
    }

    public override void LoadContent(InitState state)
    {
      base.LoadContent(state);
      this.ScreenSize = new Point(this.renderTarget.Width, this.renderTarget.Height);
      this.InvaderWave = new InvaderWave(this);
      this.State = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOver;
      this.Random = new PcgRandom(new System.Random().Next());
      this.ufoSpeed = 1.4f;
      this.pickupSpeed = 0.8f;
      this.PlayerBullets = new List<Bullet>(10);
      this.InvaderBullets = new List<Bullet>(10);
      this.Players = new List<Player>();
      this.Particles = new List<Particle>();
      this.Pickups = new List<Pickup>();
      this.ResetHud();
    }

    public override void UpdateState(int highscore, string highscoreGamer, string highscoreVersion)
    {
      if (highscore <= StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore)
        return;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore = highscore;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer = highscoreGamer;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion = highscoreVersion;
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore.ToString();
    }

    private void ResetHud()
    {
      this.Score = 0;
      this.Lives = 3;
      this.Wave = 1;
      this.WaveText = "Wave: 1";
      this.ScoreText = "Score: 0";
      StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore.ToString();
      this.highscoreGamerUpdated = false;
    }

    public override void StartGame()
    {
      if (this.State != StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play)
      {
        if (this.Credits <= 0)
          return;
        this.ChangeCredits(-1);
        this.ResetHud();
        this.State = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play;
        this.Players.Clear();
        this.Players.Add(new Player(true));
        this.Wave = 0;
        this.InvaderWave.ResetWave();
        this.UFO.IsAlive = false;
      }
      else
        this.GameOver(false);
    }

    public void GameOver(bool transition)
    {
      if (transition)
      {
        this.State = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOverTransition;
        this.gameOverTransitionTimer = 3f;
      }
      else
      {
        this.State = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOver;
        this.PlayerBullets.Clear();
        this.InvaderBullets.Clear();
        this.Particles.Clear();
        this.Pickups.Clear();
      }
      if (!this.highscoreGamerUpdated)
        return;
      this.instance.NetworkManager.SendArcadeState();
      this.highscoreGamerUpdated = false;
    }

    public override bool HandleInput()
    {
      bool flag = this.OnLeftStick() | this.OnAButton(InputManager1.IsInputPressedNew(this.tmPlayer.PlayerIndex, PlayerInput.ArcadeFireWeapon));
      if (InputManager1.IsInputReleasedNew(this.tmPlayer.PlayerIndex, GuiInput.ExitScreen))
      {
        this.GameOver(false);
        flag = true;
      }
      if (InputManager.IsButtonReleasedNew(this.tmPlayer.PlayerIndex, Buttons.Start))
        return false;
      return flag;
    }

    private bool OnLeftStick()
    {
      Vector2 gamepadLeftStick = InputManager.GetGamepadLeftStick(this.tmPlayer.PlayerIndex);
      if ((double) gamepadLeftStick.X == 0.0)
      {
        if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveLeft))
          gamepadLeftStick.X = -1f;
        else if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveRight))
          gamepadLeftStick.X = 1f;
      }
      if (this.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play)
      {
        Player player = this.Players[0];
        player.Velocity.X = (double) gamepadLeftStick.X >= 0.0 ? ((double) gamepadLeftStick.X <= 0.0 ? 0.0f : player.Speed) : -player.Speed;
        this.Players[0] = player;
        return true;
      }
      return this.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOverTransition;
    }

    private bool OnAButton(bool newPress)
    {
      if (this.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play)
      {
        Player player = this.Players[0];
        player.ShotDelayTimer += Services.ElapsedTime;
        if (newPress)
          player.ShotDelayTimer = player.ShotDelay;
        if ((double) player.ShotDelayTimer >= (double) player.ShotDelay)
        {
          this.FirePlayerBullet();
          player.ShotDelayTimer = 0.0f;
        }
        return true;
      }
      return this.State == StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOverTransition;
    }

    private void FirePlayerBullet()
    {
      for (int index = 0; index < this.Players.Count; ++index)
      {
        this.PlayerBullets.Add(new Bullet());
        this.ActivateBullet(this.Players[index], this.PlayerBullets.Count - 1);
      }
      CoreGlobals.AudioManager.PlaySound("InvadersFire");
    }

    private void ActivateBullet(Player player, int i)
    {
      Bullet playerBullet = this.PlayerBullets[i];
      playerBullet.Position.X = (float) ((double) player.Position.X + 9.0 - 1.0);
      playerBullet.Position.Y = player.Position.Y;
      playerBullet.Velocity = this.Players[0].BulletVelocity;
      this.PlayerBullets[i] = playerBullet;
    }

    public override void Update()
    {
      if (!this.tmPlayer.IsInputEnabled)
        return;
      try
      {
        switch (this.State)
        {
          case StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play:
            this.UpdatePlayState();
            break;
          case StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.GameOverTransition:
            this.UpdateGameOverTransitionState();
            break;
        }
        if (!this.tmPlayer.IsGod || this.Players.Count <= 0)
          return;
        GamePadState state = GamePad.GetState(this.tmPlayer.PlayerIndex);
        if (state.Buttons.Y == ButtonState.Pressed && this.lastpad.Buttons.Y == ButtonState.Released)
          this.AddPlayerShip();
        this.lastpad = state;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(1, ex);
        this.GameOver(false);
      }
    }

    private void UpdatePlayState()
    {
      if (--this.FrameCounter < 0)
      {
        this.FrameCounter = (int) Math.Max(10f / Math.Abs(this.InvaderWave.Velocity.X), 10f);
        ++this.AnimFrame;
      }
      this.UpdatePlayers();
      this.InvaderWave.Update();
      this.UpdateUFO();
      this.UpdatePlayerBullets();
      this.UpdateInvaderBullets();
      this.UpdatePickups();
      this.UpdateParticles();
    }

    private void UpdateGameOverTransitionState()
    {
      this.gameOverTransitionTimer -= Services.ElapsedTime;
      if ((double) this.gameOverTransitionTimer > 0.0)
      {
        this.UpdateUFO();
        this.UpdatePlayerBullets();
        this.UpdateInvaderBullets();
        this.UpdatePickups();
        this.UpdateParticles();
      }
      else
        this.GameOver(false);
    }

    private void UpdateUFO()
    {
      if (this.UFO.IsAlive)
      {
        this.UFO.Position += this.UFO.Velocity;
        if ((double) this.UFO.Position.X + 22.0 >= 0.0 && (double) this.UFO.Position.X <= (double) this.ScreenSize.X)
          return;
        this.UFO.IsAlive = false;
      }
      else
      {
        if (this.State != StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.GameState.Play || this.Random.Next(800) != 0)
          return;
        this.UFO.IsAlive = true;
        if (this.Random.Next(2) == 0)
        {
          this.UFO.Position.X = -22f;
          this.UFO.Velocity.X = this.ufoSpeed;
        }
        else
        {
          this.UFO.Position.X = (float) this.ScreenSize.X;
          this.UFO.Velocity.X = -this.ufoSpeed;
        }
        this.UFO.Position.Y = 14f;
        this.UFO.Pickup = (PickupType) this.Random.Next(2);
        CoreGlobals.AudioManager.PlaySound("InvadersUfo");
      }
    }

    private void UpdatePlayers()
    {
      bool flag = false;
      if ((double) this.Players[0].Velocity.X < 0.0)
        flag = (double) this.Players[0].Position.X > 4.0;
      else if ((double) this.Players[0].Velocity.X > 0.0)
        flag = (double) this.Players[this.Players.Count - 1].Position.X + 18.0 < (double) (this.ScreenSize.X - 4);
      if (!flag)
        return;
      for (int index = 0; index < this.Players.Count; ++index)
      {
        Player player = this.Players[index];
        player.Position += this.Players[0].Velocity;
        this.Players[index] = player;
      }
    }

    private void UpdatePlayerBullets()
    {
      for (int index = this.PlayerBullets.Count - 1; index >= 0; --index)
      {
        Bullet playerBullet = this.PlayerBullets[index];
        playerBullet.Position += playerBullet.Velocity;
        if (this.InvaderWave.CheckHit(playerBullet))
        {
          this.PlayerBullets.RemoveAt(index);
          this.UpdateScore(10);
          CoreGlobals.AudioManager.PlaySound("InvadersExplosion");
        }
        else if (this.UFO.IsAlive && (double) playerBullet.Position.X <= (double) this.UFO.Position.X + 22.0 - 2.0 && ((double) playerBullet.Position.X + 2.0 > (double) this.UFO.Position.X && (double) playerBullet.Position.Y < (double) this.UFO.Position.Y + 8.0) && (double) playerBullet.Position.Y + 10.0 > (double) this.UFO.Position.Y)
        {
          this.UFODestroyed(index);
          this.PlayerBullets.RemoveAt(index);
        }
        else if ((double) playerBullet.Position.Y < -10.0)
          this.PlayerBullets.RemoveAt(index);
        else
          this.PlayerBullets[index] = playerBullet;
      }
    }

    private void UFODestroyed(int i)
    {
      this.AddExplosionParticles(this.UFO.Position + new Vector2(8f, 7f), this.UFO.Velocity, new Color(181, 0, 208), 30);
      CoreGlobals.AudioManager.PlaySound("InvadersPlayerExplosion");
      this.UFO.IsAlive = false;
      this.UpdateScore(50);
      if (this.Random.Next(3) != 0)
        return;
      int num = this.Random.Next(2);
      if (num <= 0)
        return;
      Vector2 position = this.UFO.Position;
      position.X += 7f;
      this.Pickups.Add(new Pickup()
      {
        IsAlive = true,
        PickupType = (PickupType) num,
        Position = position,
        Velocity = new Vector2(0.0f, this.pickupSpeed)
      });
    }

    private void UpdateInvaderBullets()
    {
      for (int index = this.InvaderBullets.Count - 1; index >= 0; --index)
      {
        Bullet invaderBullet = this.InvaderBullets[index];
        invaderBullet.Position += invaderBullet.Velocity;
        if (this.CheckHitOnPlayer(invaderBullet))
          this.InvaderBullets.RemoveAt(index);
        else if ((double) invaderBullet.Position.Y > (double) this.ScreenSize.Y)
          this.InvaderBullets.RemoveAt(index);
        else
          this.InvaderBullets[index] = invaderBullet;
      }
    }

    private void UpdatePickups()
    {
      for (int index = this.Pickups.Count - 1; index >= 0; --index)
      {
        Pickup pickup = this.Pickups[index];
        pickup.Position += pickup.Velocity;
        if (this.CheckHitOnPlayer(pickup))
        {
          this.PlayerGotPickup(pickup.PickupType);
          this.Pickups.RemoveAt(index);
        }
        else if ((double) pickup.Position.Y > (double) this.ScreenSize.Y)
          this.Pickups.RemoveAt(index);
        else
          this.Pickups[index] = pickup;
      }
    }

    private void PlayerGotPickup(PickupType type)
    {
      if (type != PickupType.PlayerShip)
        return;
      this.AddPlayerShip();
    }

    private bool CheckHitOnPlayer(Bullet b)
    {
      for (int index = this.Players.Count - 1; index >= 0; --index)
      {
        Player player = this.Players[index];
        if ((double) b.Position.X <= (double) player.Position.X + 18.0 - 2.0 && (double) b.Position.X + 2.0 > (double) player.Position.X && ((double) b.Position.Y < (double) player.Position.Y + 12.0 - 1.0 && (double) b.Position.Y + 10.0 > (double) player.Position.Y + 4.0))
        {
          this.AddExplosionParticles(player.Position + new Vector2(9f, 6f), player.Velocity, new Color(192, 192, 192), 120);
          this.Players.RemoveAt(index);
          CoreGlobals.AudioManager.PlaySound("InvadersPlayerExplosion");
          if (this.Players.Count == 0)
            this.GameOver(true);
          return true;
        }
      }
      return false;
    }

    private bool CheckHitOnPlayer(Pickup p)
    {
      for (int index = 0; index < this.Players.Count; ++index)
      {
        Player player = this.Players[index];
        if ((double) p.Position.X <= (double) player.Position.X + 18.0 - 2.0 && (double) p.Position.X + 8.0 > (double) player.Position.X && ((double) p.Position.Y < (double) player.Position.Y + 12.0 && (double) p.Position.Y + 6.0 > (double) player.Position.Y))
          return true;
      }
      return false;
    }

    private void UpdateParticles()
    {
      for (int index = this.Particles.Count - 1; index >= 0; --index)
      {
        Particle particle = this.Particles[index];
        particle.Position += particle.Velocity;
        if ((double) particle.Position.Y < 0.0 || (double) particle.Position.X < 0.0 || ((double) particle.Position.Y > (double) this.ScreenSize.Y || (double) particle.Position.X > (double) this.ScreenSize.X))
        {
          this.Particles.RemoveAt(index);
        }
        else
        {
          particle.Age -= Services.ElapsedTime;
          if ((double) particle.Age < 0.0)
          {
            this.Particles.RemoveAt(index);
          }
          else
          {
            particle.Velocity.X *= 0.99f;
            particle.Velocity.Y *= 0.99f;
            this.Particles[index] = particle;
          }
        }
      }
    }

    private void UpdateCollisions()
    {
    }

    private void UpdateScore(int inc)
    {
      if (inc == 0)
        return;
      this.Score += inc;
      this.ScoreText = "Score: " + this.Score.ToString();
      if (this.Score > StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore)
      {
        StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore = this.Score;
        StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore.ToString();
        if (!this.highscoreGamerUpdated)
        {
          StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer = this.tmPlayer.Gamer.Gamertag;
          int num = 27302;
          StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion = string.Format("V{0}.{1}", (object) (num / 10000), (object) (num % 10000 / 100));
          this.highscoreGamerUpdated = true;
        }
      }
      (this.tmPlayer as StudioForge.TotalMiner.Player)?.Raise_TotalRushScore(this.Score);
    }

    private void AddPlayerShip()
    {
      if (this.Players.Count >= 14)
        return;
      this.Players.Add(new Player(true)
      {
        Position = this.Players[this.Players.Count - 1].Position
      });
      float num = this.Players.Count == 1 ? 22f : this.Players[0].Position.X - 11f;
      for (int index = 0; index < this.Players.Count; ++index)
      {
        Player player = this.Players[index];
        player.Position.X = num;
        this.Players[index] = player;
        num += 22f;
      }
    }

    public void AddExplosionParticles(Vector2 position, Vector2 velocity, Color color, int count)
    {
      int length = this.explosionColors.Length;
      while (count-- > 0)
      {
        Particle particle = new Particle();
        particle.Position = position;
        particle.Age = (float) (this.Random.NextDouble() * 0.5 + 1.0);
        particle.Velocity = velocity * 0.5f;
        particle.Velocity.X += (float) (this.Random.NextDouble() * 2.5 - 1.25);
        particle.Velocity.Y += (float) (this.Random.NextDouble() * 2.5 - 1.25);
        int index = this.Random.Next(length * 2);
        particle.Color = index < length ? this.explosionColors[index] : color;
        this.Particles.Add(particle);
      }
    }

    public enum GameState
    {
      Play,
      GameOverTransition,
      GameOver,
    }
  }
}
