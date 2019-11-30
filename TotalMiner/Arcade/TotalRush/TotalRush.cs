// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.TotalRush
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

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class TotalRush : ArcadeMachine
  {
    public static int HighScore = 0;
    public static string HighScoreGamer = "";
    public static string HighScoreVersion = "";
    public float DifficultyFactor = 1f;
    public const float ElapsedTime = 0.01666667f;
    public const float BigShipKillGraceTime = 3f;
    public const float SmartBombShakeTime = 1f;
    private const int maxActors = 50;
    private const int maxBullets = 2000;
    private const int maxParticles = 700;
    public StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState State;
    public Point ScreenSize;
    public int Score;
    public PcgRandom Random;
    public static string HighScoreText;
    public string ScoreText;
    public int Rank;
    public Actor[] Actors;
    public Bullet[] Bullets;
    public Particle[] Particles;
    public ActorBehaviour[] Behaviours;
    public BulletPatternBase[] BulletPatterns;
    public LinkedList<Point> UsedActors;
    public LinkedList<Point> UsedBullets;
    public LinkedList<Point> UsedParticles;
    public ActorDataXML[] ActorData;
    public ParticleDataXML[] ParticleData;
    public Stack<int> UnusedActors;
    public Stack<int> UnusedBullets;
    public Stack<int> UnusedParticles;
    public ActorDataXML PlayerBulletData;
    public int PlayerID;
    public Vector2 WorldShake;
    public float BigShipKillGraceTimer;
    public float SmartBombShakeTimer;
    public Sound Sound;
    private GameInstance instance;
    private float gameOverTransitionTimer;
    private LevelDataXML[] levelData;
    private float levelTimer;
    private int levelCounter;
    private bool waitingForBossKill;
    private float drawRankTextTimer;
    private int gamePlayFrameCounter;
    private int bossSpawnFrameCounter;
    private bool highscoreGamerUpdated;
    private Stack<int> unusedNodes;
    private List<LinkedListNode<Point>> nodes;

    public override bool CanDeactivate
    {
      get
      {
        return this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOver;
      }
    }

    public string CurrLevelCount
    {
      get
      {
        return string.Format("{0} / {1}", (object) this.levelCounter, (object) (this.levelData != null ? this.levelData.Length : 0));
      }
    }

    public Actor Player
    {
      get
      {
        return this.Actors[this.PlayerID];
      }
    }

    public bool IsPlayerMaxPower
    {
      get
      {
        BulletPattern bulletPattern = this.Actors[this.PlayerID].BulletPatterns.Array[0];
        return (double) bulletPattern.BulletsPerSecond >= (double) bulletPattern.MaxBulletsPerSecond;
      }
    }

    public bool DrawRankMessage
    {
      get
      {
        return (double) this.drawRankTextTimer > 0.0;
      }
    }

    public TotalRush(
      GameInstance instance,
      ITMMap map,
      StudioForge.TotalMiner.Player player,
      GlobalPoint3D point,
      BlockFace face)
      : base((ITMGame) instance, map, (ITMPlayer) player, point, face)
    {
      this.instance = instance;
      this.Random = new PcgRandom(instance.Random.Next());
      this.GpPerCredit = 2;
      this.Behaviours = new ActorBehaviour[18];
      this.Behaviours[1] = (ActorBehaviour) new PlayerBehaviour(this);
      this.Behaviours[2] = (ActorBehaviour) new TailShipBehaviour(this);
      this.Behaviours[4] = (ActorBehaviour) new PickupBehaviour(this);
      this.Behaviours[5] = this.Behaviours[4];
      this.Behaviours[7] = (ActorBehaviour) new TurretBehaviour(this);
      this.Behaviours[8] = this.Behaviours[7];
      this.Behaviours[9] = this.Behaviours[7];
      this.Behaviours[10] = (ActorBehaviour) new EnemyBehaviour(this);
      this.Behaviours[11] = this.Behaviours[10];
      this.Behaviours[12] = this.Behaviours[10];
      this.Behaviours[13] = this.Behaviours[10];
      this.Behaviours[14] = this.Behaviours[10];
      this.Behaviours[15] = this.Behaviours[10];
      this.Behaviours[16] = this.Behaviours[15];
      this.Behaviours[17] = this.Behaviours[15];
      this.BulletPatterns = new BulletPatternBase[14];
      this.BulletPatterns[0] = (BulletPatternBase) new PlayerBulletPattern(this.Random);
      this.BulletPatterns[1] = (BulletPatternBase) new PlayerTailShipBulletPattern(this.Random);
      this.BulletPatterns[2] = (BulletPatternBase) new AtTargetBulletPattern(this.Random);
      this.BulletPatterns[3] = (BulletPatternBase) new AtTargetAndRotateParentBulletPattern(this.Random);
      this.BulletPatterns[4] = (BulletPatternBase) new ParentAimBulletPattern(this.Random);
      this.BulletPatterns[5] = (BulletPatternBase) new StarBulletPattern(this.Random);
      this.BulletPatterns[6] = (BulletPatternBase) new CrossRotatedBulletPattern(this.Random);
      this.BulletPatterns[7] = (BulletPatternBase) new CircleBulletPattern(this.Random);
      this.BulletPatterns[8] = (BulletPatternBase) new CircleDoubleBulletPattern(this.Random);
      this.BulletPatterns[9] = (BulletPatternBase) new CircleSmallRandomBlobsBulletPattern(this.Random);
      this.BulletPatterns[10] = (BulletPatternBase) new CircleRandomSliceBulletPattern(this.Random);
      this.BulletPatterns[11] = (BulletPatternBase) new CircleDoubleRandomSliceBulletPattern(this.Random);
      this.BulletPatterns[12] = (BulletPatternBase) new SpikeBulletPattern(this.Random);
      this.BulletPatterns[13] = (BulletPatternBase) new SpikePincerBulletPattern(this.Random);
    }

    public override void LoadContent(InitState state)
    {
      base.LoadContent(state);
      this.ScreenSize = new Point(this.renderTarget.Width, this.renderTarget.Height);
      this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOver;
      this.Sound = new Sound();
      this.ResetHud();
      TotalRushLoader totalRushLoader = new TotalRushLoader();
      totalRushLoader.Initialize(this);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) totalRushLoader, false, PriorityLevel.Urgent);
      this.Actors = new Actor[50];
      this.UsedActors = new LinkedList<Point>();
      this.UnusedActors = new Stack<int>(50);
      int initialCapacity = 10;
      for (int index1 = 0; index1 < 50; ++index1)
      {
        Actor actor = this.Actors[index1] = new Actor();
        actor.BulletPatterns = new CustomArray<BulletPattern>(initialCapacity, 2f);
        for (int index2 = 0; index2 < initialCapacity; ++index2)
          actor.BulletPatterns.Add(new BulletPattern());
        actor.BulletPatterns.Clear();
      }
      this.Bullets = new Bullet[2000];
      this.UsedBullets = new LinkedList<Point>();
      this.UnusedBullets = new Stack<int>(2000);
      this.Particles = new Particle[700];
      this.UsedParticles = new LinkedList<Point>();
      this.UnusedParticles = new Stack<int>(2000);
      this.unusedNodes = new Stack<int>(2000);
      this.nodes = new List<LinkedListNode<Point>>(2000);
    }

    public override void UpdateState(int highscore, string highscoreGamer, string highscoreVersion)
    {
      if (highscore <= StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore)
        return;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore = highscore;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer = highscoreGamer;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion = highscoreVersion;
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore.ToString();
    }

    public void OnDataLoaded(
      ActorDataXML[] actorData,
      ParticleDataXML[] particleData,
      LevelDataXML[] levelData)
    {
      this.ActorData = actorData;
      this.levelData = levelData;
      this.ParticleData = particleData;
      this.PlayerBulletData = this.ActorData[3];
    }

    private void ResetHud()
    {
      this.Score = 0;
      this.ScoreText = "Score: 0";
      StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore.ToString();
      this.highscoreGamerUpdated = false;
    }

    public override void StartGame()
    {
      if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play || this.ActorData == null || this.Credits <= 0)
        return;
      this.ChangeCredits(-1);
      this.ResetHud();
      this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play;
      this.DifficultyFactor = 1f;
      this.levelTimer = 0.0f;
      this.levelCounter = 0;
      this.waitingForBossKill = false;
      this.WorldShake = Vector2.Zero;
      this.Rank = 0;
      this.drawRankTextTimer = 4f;
      this.gamePlayFrameCounter = 0;
      this.UsedActors.Clear();
      this.UnusedActors.Clear();
      for (int index = 0; index < 50; ++index)
        this.UnusedActors.Push(index);
      this.UsedBullets.Clear();
      this.UnusedBullets.Clear();
      for (int index = 0; index < 2000; ++index)
        this.UnusedBullets.Push(index);
      this.UsedParticles.Clear();
      this.UnusedParticles.Clear();
      for (int index = 0; index < 700; ++index)
        this.UnusedParticles.Push(index);
      this.nodes.Clear();
      this.unusedNodes.Clear();
      for (int index = 0; index < this.nodes.Capacity; ++index)
        this.unusedNodes.Push(index);
      this.CreateActor((Actor) null, ActorType.Player, new Vector2((float) this.ScreenSize.X * 0.5f, (float) this.ScreenSize.Y * 0.7f));
      this.Sound.Initialize();
      GC.Collect();
    }

    public void GameOver(bool transition)
    {
      if (transition)
      {
        if (this.State != StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition)
        {
          this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition;
          this.gameOverTransitionTimer = 10f;
        }
      }
      else
      {
        this.drawRankTextTimer = 0.0f;
        this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOver;
      }
      if (!this.highscoreGamerUpdated)
        return;
      this.instance.NetworkManager.SendArcadeState();
      this.highscoreGamerUpdated = false;
    }

    public LinkedListNode<Point> GetListNode(int i)
    {
      if (this.unusedNodes.Count == 0)
      {
        for (int index = 0; index < 10; ++index)
          this.unusedNodes.Push(index + this.nodes.Count);
      }
      int index1 = this.unusedNodes.Pop();
      while (index1 >= this.nodes.Count)
        this.nodes.Add(new LinkedListNode<Point>(new Point()));
      LinkedListNode<Point> node = this.nodes[index1];
      node.Value = new Point() { X = i, Y = index1 };
      return node;
    }

    public void FreeNode(LinkedListNode<Point> node)
    {
      node.List.Remove(node);
      this.unusedNodes.Push(node.Value.Y);
    }

    public override bool HandleInput()
    {
      this.OnLeftStick();
      this.OnRightStick();
      if (this.OnFireButton() || this.OnAButton(InputManager1.IsInputPressedNew(this.tmPlayer.PlayerIndex, GuiInput.SelectItem)) || this.OnBButton(InputManager.IsButtonReleasedNew(this.tmPlayer.PlayerIndex, Buttons.B)))
        return true;
      if (InputManager.IsButtonReleasedNew(this.tmPlayer.PlayerIndex, Buttons.Start))
        return false;
      if (this.State != StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play)
        return this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition;
      return true;
    }

    private bool OnLeftStick()
    {
      if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play)
      {
        Actor actor = this.Actors[this.PlayerID];
        if (actor.IsAlive && !actor.IsDestroyed)
        {
          Vector2 gamepadLeftStick = InputManager.GetGamepadLeftStick(this.tmPlayer.PlayerIndex);
          if ((double) gamepadLeftStick.X == 0.0)
          {
            if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveLeft))
              gamepadLeftStick.X = -1f;
            else if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveRight))
              gamepadLeftStick.X = 1f;
          }
          actor.Velocity.X = actor.Speed.X * gamepadLeftStick.X;
          float num1 = actor.WorldPosition.X + actor.Velocity.X;
          if ((double) num1 < 4.0 && (double) actor.Velocity.X < 0.0 || (double) num1 > (double) (this.ScreenSize.X - 4) && (double) actor.Velocity.X > 0.0)
            actor.Velocity.X = 0.0f;
          if ((double) gamepadLeftStick.Y == 0.0)
          {
            if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveBackward))
              gamepadLeftStick.Y = -1f;
            else if (InputManager1.IsInputPressed(this.tmPlayer.PlayerIndex, PlayerInput.MoveForward))
              gamepadLeftStick.Y = 1f;
          }
          actor.Velocity.Y = actor.Speed.Y * -gamepadLeftStick.Y;
          float num2 = actor.WorldPosition.Y + actor.Velocity.Y;
          if ((double) num2 < 6.0 && (double) actor.Velocity.Y < 0.0 || (double) num2 > (double) (this.ScreenSize.Y - 6) && (double) actor.Velocity.Y > 0.0)
            actor.Velocity.Y = 0.0f;
        }
        return true;
      }
      return this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition;
    }

    private bool OnRightStick()
    {
      if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play)
      {
        Vector2 vector2 = InputManager.GetGamepadRightStick(this.tmPlayer.PlayerIndex);
        if ((double) vector2.X == 0.0 && (double) vector2.Y == 0.0)
          vector2 = InputManager.GetMousePosDelta(this.tmPlayer.PlayerIndex);
        if ((double) vector2.X != 0.0 || (double) vector2.Y != 0.0)
        {
          Actor actor = this.Actors[this.PlayerID];
          if (actor.IsAlive && !actor.IsDestroyed)
          {
            actor.Aim.X = vector2.X;
            actor.Aim.Y = -vector2.Y;
            actor.Rotation = MyMathHelper.GetAngle(Vector2.Zero, actor.Aim) + 1.570796f;
            this.Behaviours[(int) actor.ActorType].FireWeapons(actor);
          }
        }
        return true;
      }
      return this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition;
    }

    private bool OnAButton(bool newPress)
    {
      if (newPress)
      {
        if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play)
        {
          if (this.tmPlayer.IsGod)
            this.ActivateSmartBombCore(this.Actors[this.PlayerID]);
          return true;
        }
        if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOver)
        {
          this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.ControlsScreen;
          return true;
        }
        if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.ControlsScreen)
        {
          this.State = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOver;
          return true;
        }
      }
      return false;
    }

    private bool OnBButton(bool newPress)
    {
      if (newPress)
      {
        if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play)
        {
          this.GameOver(false);
          return true;
        }
        if (this.State == StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition)
        {
          this.gameOverTransitionTimer = 0.0f;
          return true;
        }
      }
      return false;
    }

    private bool OnFireButton()
    {
      if (this.State != StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play || !InputManager1.IsInputPressedNew(this.tmPlayer.PlayerIndex, PlayerInput.ArcadeFireWeapon) && !InputManager.IsButtonPressedNew(this.tmPlayer.PlayerIndex, Buttons.LeftShoulder) && !InputManager.IsButtonPressedNew(this.tmPlayer.PlayerIndex, Buttons.RightShoulder))
        return false;
      this.ActivateSmartBomb();
      return true;
    }

    private void ActivateSmartBomb()
    {
      Actor actor = this.Actors[this.PlayerID];
      if (actor.TailShipCount <= 0)
        return;
      this.ActivateSmartBombCore(actor);
    }

    private void ActivateSmartBombCore(Actor actor)
    {
      if (!actor.IsAlive || actor.IsDestroyed)
        return;
      PlayerBehaviour behaviour = this.Behaviours[1] as PlayerBehaviour;
      if (behaviour == null)
        return;
      behaviour.ActivateSmartBomb(actor);
      this.SmartBombShakeTimer = 1f;
    }

    public override void Update()
    {
      if (!this.tmPlayer.IsInputEnabled)
        return;
      try
      {
        switch (this.State)
        {
          case StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.Play:
            this.UpdatePlayState();
            break;
          case StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.GameState.GameOverTransition:
            this.UpdateGameOverTransitionState();
            break;
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(1, ex);
        this.GameOver(false);
      }
    }

    private void UpdatePlayState()
    {
      this.UpdateLevel();
      this.UpdateActors();
      this.UpdateBullets();
      this.UpdateParticles();
      ++this.gamePlayFrameCounter;
    }

    private void UpdateGameOverTransitionState()
    {
      this.gameOverTransitionTimer -= Services.ElapsedTime;
      if ((double) this.gameOverTransitionTimer > 0.0)
      {
        this.UpdateActors();
        this.UpdateBullets();
        this.UpdateParticles();
      }
      else
        this.GameOver(false);
    }

    private void UpdateLevel()
    {
      if (!this.waitingForBossKill)
      {
        if (this.levelCounter < this.levelData.Length)
        {
          this.levelTimer += 0.01666667f;
          float num = this.levelData[this.levelCounter].SpawnTime / this.DifficultyFactor;
          if ((double) this.levelTimer > (double) num)
          {
            this.SpawnEnemy(this.levelCounter++);
            this.levelTimer -= num;
          }
        }
        else if (this.NoEnemiesAlive)
        {
          ++this.Rank;
          this.levelCounter = 0;
          this.levelTimer = 0.0f;
          this.DifficultyFactor += 0.25f;
          this.drawRankTextTimer = 4f;
        }
      }
      if ((!this.waitingForBossKill || this.gamePlayFrameCounter < this.bossSpawnFrameCounter + 18000) && this.Random.Next((int) (400.0 / (double) this.DifficultyFactor)) == 0)
        this.SpawnEnemy(this.Random.Next(10));
      if ((double) this.BigShipKillGraceTimer > 0.0)
      {
        this.BigShipKillGraceTimer -= 0.01666667f;
        if ((double) this.BigShipKillGraceTimer > 0.0)
        {
          float shipKillGraceTimer = this.BigShipKillGraceTimer;
          this.WorldShake.X = (float) (this.Random.NextDouble() * (double) shipKillGraceTimer * 2.0) - shipKillGraceTimer;
          this.WorldShake.Y = (float) (this.Random.NextDouble() * (double) shipKillGraceTimer * 2.0) - shipKillGraceTimer;
        }
        else
        {
          this.BigShipKillGraceTimer = 0.0f;
          this.WorldShake = Vector2.Zero;
          this.Actors[this.PlayerID].IsVulnerable = true;
        }
      }
      if ((double) this.SmartBombShakeTimer > 0.0)
      {
        this.SmartBombShakeTimer -= 0.01666667f;
        if ((double) this.SmartBombShakeTimer > 0.0)
        {
          float num = this.SmartBombShakeTimer * 1.5f;
          this.WorldShake.X = (float) (this.Random.NextDouble() * (double) num * 2.0) - num;
          this.WorldShake.Y = (float) (this.Random.NextDouble() * (double) num * 2.0) - num;
        }
        else
        {
          this.SmartBombShakeTimer = 0.0f;
          this.WorldShake = Vector2.Zero;
        }
      }
      this.drawRankTextTimer -= 0.01666667f;
    }

    private bool NoEnemiesAlive
    {
      get
      {
        for (LinkedListNode<Point> linkedListNode = this.UsedActors.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          switch (this.Actors[linkedListNode.Value.X].ActorType)
          {
            case ActorType.Player:
            case ActorType.TailShip:
            case ActorType.FirePowerPickup:
            case ActorType.TailShipPickup:
              continue;
            default:
              return false;
          }
        }
        return true;
      }
    }

    private void UpdateActors()
    {
      LinkedListNode<Point> next;
      for (LinkedListNode<Point> node = this.UsedActors.First; node != null; node = next)
      {
        next = node.Next;
        Actor actor = this.Actors[node.Value.X];
        this.Behaviours[(int) actor.ActorType].Update(actor);
        if (!actor.IsAlive)
        {
          this.DeactivateActor(node);
          if (node.Value.X == this.PlayerID)
            this.GameOver(true);
        }
      }
    }

    private void DeactivateActor(LinkedListNode<Point> node)
    {
      Actor actor = this.Actors[node.Value.X];
      actor.IsAlive = false;
      this.Behaviours[(int) actor.ActorType].OnDeactivate(actor);
      this.UnusedActors.Push(node.Value.X);
      this.FreeNode(node);
    }

    private void UpdateBullets()
    {
      Actor actor1 = this.Actors[this.PlayerID];
      LinkedListNode<Point> node = this.UsedBullets.First;
      Point screenSize = this.ScreenSize;
      screenSize.X += 10;
      screenSize.Y += 10;
      LinkedListNode<Point> next1;
      for (; node != null; node = next1)
      {
        next1 = node.Next;
        Bullet bullet = this.Bullets[node.Value.X];
        bullet.Position.X += bullet.Velocity.X;
        bullet.Position.Y += bullet.Velocity.Y;
        bool flag = false;
        LinkedListNode<Point> next2;
        if ((double) bullet.Position.X < -10.0 || (double) bullet.Position.X > (double) (screenSize.X + 10) || (double) bullet.Position.Y > (double) (screenSize.Y + 10) || (double) bullet.Position.Y < -10.0 && (double) bullet.Velocity.Y < 0.0)
          flag = true;
        else if (bullet.ActorType == ActorType.PlayerBullet)
        {
          for (LinkedListNode<Point> linkedListNode = this.UsedActors.First; linkedListNode != null; linkedListNode = next2)
          {
            next2 = linkedListNode.Next;
            if (linkedListNode.Value.X != this.PlayerID)
            {
              Actor actor2 = this.Actors[linkedListNode.Value.X];
              if (actor2.IsVulnerable && (double) Vector2.DistanceSquared(bullet.Position, actor2.HitBoxCenter) <= (double) bullet.HitBoxRadius2 + (double) actor2.HitBoxRadius2)
              {
                flag = true;
                this.Behaviours[(int) actor2.ActorType].TakeDamage(actor2, 1);
                if (this.Random.Next(4) == 0)
                  this.AddDamageParticles(actor2.ActorType, bullet.Position, ParticleType.ExplosionFront, (float) (this.Random.NextDouble() * 0.5 + 0.25), 0.5f, Vector2.Zero, 1);
              }
            }
          }
        }
        else if (actor1.IsVulnerable && (double) Vector2.DistanceSquared(bullet.Position, actor1.HitBoxCenter) <= (double) bullet.HitBoxRadius2 + (double) actor1.HitBoxRadius2)
        {
          this.DestroyActor(actor1);
          flag = true;
        }
        if (flag)
          this.DeactivateBullet(node);
        else
          this.Bullets[node.Value.X] = bullet;
      }
    }

    private void DeactivateBullet(LinkedListNode<Point> node)
    {
      this.UnusedBullets.Push(node.Value.X);
      this.FreeNode(node);
    }

    private void UpdateParticles()
    {
      if (this.Random.Next(10) == 0)
        this.AddParticle(new Vector2((float) this.Random.Next(this.ScreenSize.X), (float) this.Random.Next(this.ScreenSize.Y)), ParticleType.Dust, 1f, 3f, new Vector2((float) (this.Random.NextDouble() * 2.0 + 0.100000001490116 - 1.04999995231628), (float) (this.Random.NextDouble() * 2.0 + 0.100000001490116 - 1.04999995231628)), Color.White * (float) (this.Random.NextDouble() * 0.5 + 0.5));
      if (this.Random.Next(4) == 0)
      {
        Vector2 position = new Vector2((float) this.Random.Next(this.ScreenSize.X), 0.0f);
        Vector2 velocity = new Vector2(0.0f, (float) (this.Random.NextDouble() * 0.5 + 1.0));
        Color color = Color.White * (float) (this.Random.NextDouble() * 0.25 + 0.75);
        this.AddParticle(position, ParticleType.Dust, 1f, (float) ((double) this.ScreenSize.Y / ((double) velocity.Y * 60.0) * 1.5), velocity, color);
      }
      LinkedListNode<Point> node = this.UsedParticles.First;
      float num = 0.01666667f;
      LinkedListNode<Point> next;
      for (; node != null; node = next)
      {
        next = node.Next;
        Particle particle = this.Particles[node.Value.X];
        particle.Position.X += particle.Velocity.X;
        particle.Position.Y += particle.Velocity.Y;
        particle.Rotation += 0.05f;
        particle.Age -= num;
        if ((double) particle.Age <= 0.0 || (double) particle.Position.X < 0.0 || ((double) particle.Position.X > (double) this.ScreenSize.X || (double) particle.Position.Y < 0.0) || (double) particle.Position.Y > (double) this.ScreenSize.Y)
          this.DeactivateParticle(node);
        else
          this.Particles[node.Value.X] = particle;
      }
    }

    private void DeactivateParticle(LinkedListNode<Point> node)
    {
      this.UnusedParticles.Push(node.Value.X);
      this.FreeNode(node);
    }

    public void UpdateScore(int inc)
    {
      if (inc == 0)
        return;
      this.Score += inc;
      this.ScoreText = "Score: " + this.Score.ToString();
      if (this.Score > StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore)
      {
        StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore = this.Score;
        StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreText = "High Score: " + StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore.ToString();
        if (!this.highscoreGamerUpdated)
        {
          StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer = this.tmPlayer.Gamer.Gamertag;
          int num = 27302;
          StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion = string.Format("V{0}.{1}", (object) (num / 10000), (object) (num % 10000 / 100));
          this.highscoreGamerUpdated = true;
        }
      }
      (this.tmPlayer as StudioForge.TotalMiner.Player)?.Raise_TotalRushScore(this.Score);
    }

    public void UpdatePower(int power)
    {
      Actor actor = this.Actors[this.PlayerID];
      BulletPattern bulletPattern = actor.BulletPatterns.Array[0];
      bulletPattern.BulletsPerSecond = Math.Min(bulletPattern.BulletsPerSecond + (float) power, bulletPattern.MaxBulletsPerSecond);
      actor.BulletPatterns.Array[0] = bulletPattern;
    }

    private void BossKilled(Actor actor)
    {
      this.waitingForBossKill = false;
    }

    public void BigShipKill()
    {
      this.BigShipKillGraceTimer = 3f;
      this.Actors[this.PlayerID].IsVulnerable = false;
    }

    public Actor CreateActor(Actor parent, ActorType type, Vector2 position)
    {
      Actor actor = (Actor) null;
      if (this.UnusedActors.Count > 0)
      {
        int i = this.UnusedActors.Pop();
        this.UsedActors.AddLast(this.GetListNode(i));
        actor = this.Actors[i];
        if (type == ActorType.Player)
          this.PlayerID = i;
        actor.Parent = parent;
        actor.ActorType = type;
        actor.ChildCount = 0;
        this.Behaviours[(int) actor.ActorType].InitializeActor(actor);
        actor.Position = position;
        actor.OriginalPosition = actor.Position;
        actor.CurrentTargetTravelTime = 0.0f;
        if (parent != null)
          ++parent.ChildCount;
        actor.WorldPosition = actor.Parent != null ? actor.Parent.WorldPosition + actor.Position : actor.Position;
        actor.HitBoxCenter.X = actor.WorldPosition.X + actor.HitBoxOffset.X * actor.Scale;
        actor.HitBoxCenter.Y = actor.WorldPosition.Y + actor.HitBoxOffset.Y * actor.Scale;
        if (this.ActorData[(int) type].IsBoss)
        {
          actor.DestroyedHandler = new Action<Actor>(this.BossKilled);
          this.bossSpawnFrameCounter = this.gamePlayFrameCounter;
          this.waitingForBossKill = true;
        }
      }
      return actor;
    }

    public void SetActorDefaults(Actor actor, ActorType type)
    {
      float num = type == ActorType.Player || type == ActorType.TailShip ? 1f : this.DifficultyFactor;
      ActorDataXML actorDataXml = this.ActorData[(int) type];
      actor.ActorType = type;
      actor.Scale = 1f;
      actor.Speed = actorDataXml.Speed;
      actor.Hitpoints = (int) ((double) actorDataXml.Hitpoints * (double) num);
      actor.IsVulnerable = actorDataXml.IsVulnerable;
      actor.MaxPosY = actorDataXml.MaxPosY;
      actor.HitBoxRadius = actorDataXml.HitBoxRadius * actorDataXml.Scale;
      actor.HitBoxRadius2 = actor.HitBoxRadius * actor.HitBoxRadius;
      actor.HitBoxOffset.X = actorDataXml.HitBoxOffset.X * actorDataXml.Scale;
      actor.HitBoxOffset.Y = actorDataXml.HitBoxOffset.Y * actorDataXml.Scale;
      actor.Score = actorDataXml.Score;
      actor.Rotation = actorDataXml.RotationOffset;
      actor.RotationSpeed = actorDataXml.RotationSpeed;
      actor.IsDestroyed = false;
      actor.DestroyedAge = 0.0f;
      actor.DestroyedHandler = (Action<Actor>) null;
      actor.DropType = ActorType.None;
      actor.DropChance = 0;
      actor.BulletsFired = 0;
      actor.ExhaustTimer = 0;
      actor.Velocity = Vector2.Zero;
      actor.TailShipID = 0;
      actor.TailShipCount = 0;
      actor.BulletPatterns.Clear();
      if (actorDataXml.BulletPatterns == null)
        return;
            for (int i = 0; i < actorDataXml.BulletPatterns.Length; i++)
            {
                actorDataXml.BulletPatterns[i].BulletsPerSecond *= num;
                actorDataXml.BulletPatterns[i].MaxBulletsPerSecond *= num;
                actorDataXml.BulletPatterns[i].InitialDelay *= num;
                actorDataXml.BulletPatterns[i].HitPointsStart = (int)((double)actorDataXml.BulletPatterns[i].HitPointsStart * (double)num);
                actorDataXml.BulletPatterns[i].HitPointsEnd = (int)((double)actorDataXml.BulletPatterns[i].HitPointsEnd * (double)num);
                actor.BulletPatterns.Add(actorDataXml.BulletPatterns[i]);
            }
        }

    public void DestroyActor(Actor actor)
    {
      if (actor == null || actor.IsDestroyed)
        return;
      this.Behaviours[(int) actor.ActorType].DestroyActor(actor);
    }

    private void SpawnEnemy(int levelCount)
    {
      LevelDataXML levelDataXml = this.levelData[levelCount];
      ActorDataXML actorDataXml = this.ActorData[(int) levelDataXml.ActorType];
      Vector2 vector = this.AddRandomToVector(levelDataXml.Position, 50f, 0.0f);
      vector.Y = (float) (-(double) actorDataXml.Origin.Y * (double) actorDataXml.Scale - 4.0);
      Actor actor = this.CreateActor((Actor) null, levelDataXml.ActorType, vector);
      if (actor == null)
        return;
      actor.DropType = levelDataXml.DropType;
      actor.DropChance = levelDataXml.DropChance;
      if (actor.DropChance == 0 && actor.DropType != ActorType.None)
        actor.DropChance = 3;
      actor.TargetPosition = this.AddRandomToVector(levelDataXml.InitialTargetPosition, 60f, 60f);
      int num = this.ScreenSize.Y * actor.MaxPosY / 100;
      if ((double) actor.TargetPosition.Y > (double) num)
        actor.TargetPosition.Y = (float) num;
      actor.TotalTargetTravelTime = ((actor.TargetPosition - actor.Position) / (actor.Speed * 60f)).Length();
    }

    private Vector2 AddRandomToVector(Vector2 v, float width, float height)
    {
      if ((double) width != 0.0)
        v.X += (float) (this.Random.NextDouble() * ((double) width * 2.0)) - width;
      if ((double) height != 0.0)
        v.Y += (float) (this.Random.NextDouble() * ((double) height * 2.0)) - height;
      return v;
    }

    public void SpawnPickup(ActorType type, Vector2 position, Vector2 velocity)
    {
      if (type == ActorType.None)
        return;
      Actor actor = this.CreateActor((Actor) null, type, position);
      if (actor == null)
        return;
      actor.Velocity2 = velocity;
    }

    public void AddDamageParticles(
      ActorType actorType,
      Vector2 position,
      ParticleType particleType,
      float scale,
      float age,
      Vector2 velocity,
      int dustCount)
    {
      this.AddParticle(position, particleType, scale, age, velocity, Color.White * 0.9f);
      ActorDataXML actorDataXml = this.ActorData[(int) actorType];
      Vector2 zero = Vector2.Zero;
      float num1 = scale * 8f;
      float num2 = scale * 4f;
      float num3 = scale;
      for (int index = 0; index < dustCount; ++index)
      {
        zero.X = (float) this.Random.NextDouble() * num1 - num2;
        zero.Y = (float) this.Random.NextDouble() * num1 - num2;
        this.AddParticle(position, ParticleType.Dust, (float) (this.Random.NextDouble() * (double) num3 + 1.0), 3f, zero, actorDataXml.DustColor);
      }
    }

    public void AddParticle(
      Vector2 position,
      ParticleType type,
      float scale,
      float age,
      Vector2 velocity,
      Color color)
    {
      if (this.UnusedParticles.Count <= 0)
        return;
      int i = this.UnusedParticles.Pop();
      this.UsedParticles.AddLast(this.GetListNode(i));
      Particle particle = this.Particles[i];
      particle.Age = age;
      particle.OrigAge = age;
      particle.Scale = scale;
      particle.ParticleType = type;
      particle.Position = position;
      particle.Velocity = velocity;
      particle.Color = color;
      this.Particles[i] = particle;
    }

    public void ClearSurroundingBullets(ActorType bulletType, Vector2 position, float radius)
    {
      LinkedListNode<Point> node = this.UsedBullets.First;
      float num = radius * radius;
      LinkedListNode<Point> next;
      for (; node != null; node = next)
      {
        next = node.Next;
        Bullet bullet = this.Bullets[node.Value.X];
        if (bullet.ActorType == bulletType && (double) Vector2.DistanceSquared(bullet.Position, position) < (double) num)
        {
          this.AddParticle(bullet.Position, ParticleType.ExplosionFront, 0.4f, 1f, bullet.Velocity * 0.2f, Color.White);
          this.DeactivateBullet(node);
        }
      }
    }

    public enum GameState
    {
      Play,
      GameOverTransition,
      GameOver,
      ControlsScreen,
    }
  }
}
