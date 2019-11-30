// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.StatsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class StatsScreen : MinerToolScreen
  {
    private int rowHeight;
    private int rowsPerPage;
    private int scrollOffset;
    private float current;
    private float repeatTimer;
    private float lrrepeatTimer;
    private PlayerStats.Stat[] stats;
    private GameInstance instance;
    private string gamerTag;
    private float gamerTagSizeX;
    private int currentIndex;
    private bool mapStats;
    private Player currentPlayer;

    public StatsScreen(GameInstance instance, Player player, bool mapStats)
      : base(player)
    {
      this.instance = instance;
      this.mapStats = mapStats;
      instance.NetworkManager.PlayerStatsReceived += new IntEventHandler(this.OnPlayerStatsReceived);
    }

    public override void LoadContent()
    {
      this.Font = CoreGlobals.GameFont;
      this.Reload(this.player);
      this.rowHeight = 24;
      this.rowsPerPage = 20;
      int length = this.stats.Length;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 432, (this.mapStats ? 64 : 96) + length * this.rowHeight);
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.instance.NetworkManager.PlayerStatsReceived -= new IntEventHandler(this.OnPlayerStatsReceived);
    }

    private void OnPlayerStatsReceived(object sender, IntEventArgs e)
    {
      if (e.Value != this.currentIndex)
        return;
      this.Reload(e.Value);
    }

    private void Reload(Player player)
    {
      this.gamerTag = player.Gamer.Gamertag;
      this.gamerTagSizeX = this.Font.MeasureString(this.gamerTag).X;
      if (this.mapStats)
      {
        this.stats = PlayerStats.GetMapStatsAsText(this.instance);
      }
      else
      {
        this.currentPlayer = player;
        if (this.currentPlayer == null)
          return;
        this.stats = this.currentPlayer.GetPlayerStatsAsText();
        this.currentIndex = 0;
        while (this.currentIndex < this.instance.PlayerSaves.Count && !(this.instance.PlayerSaves[this.currentIndex].Gamertag == this.currentPlayer.Gamer.Gamertag))
          ++this.currentIndex;
      }
    }

    private void Reload(int i)
    {
      if (this.mapStats)
      {
        this.stats = PlayerStats.GetMapStatsAsText(this.instance);
        this.gamerTag = this.player.Gamer.Gamertag;
      }
      else
      {
        this.gamerTag = this.instance.PlayerSaves[i].Gamertag;
        this.currentPlayer = this.instance.GetPlayer(this.gamerTag);
        if (this.currentPlayer != null)
        {
          this.stats = this.currentPlayer.GetPlayerStatsAsText();
        }
        else
        {
          if (this.gamerTag == null || this.gamerTag == "")
          {
            this.gamerTag = "Loading...";
            this.instance.NetworkManager.SendPlayerStatisticsRequest(i);
          }
          this.stats = this.instance.PlayerSaves[i].Statistics.GetPlayerStatsAsText();
        }
        this.currentIndex = i;
      }
      this.gamerTagSizeX = this.Font.MeasureString(this.gamerTag).X;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        if (!this.mapStats && this.currentPlayer != null && this.player.IsGodOrTester)
          this.ScreenManager.AddScreen((GameScreen) new SkillsScreen(this.player, this.currentPlayer), this.ControllingPlayer);
        this.ExitScreen();
        return true;
      }
      if (!this.mapStats)
      {
        Vector2 left = currentGamePadState.ThumbSticks.Left;
        if ((double) left.Y != 0.0 && Math.Sign(left.Y) == Math.Sign(lastGamePadState.ThumbSticks.Left.Y) || currentGamePadState.DPad.Down == ButtonState.Pressed && lastGamePadState.DPad.Down == ButtonState.Pressed || currentGamePadState.DPad.Up == ButtonState.Pressed && lastGamePadState.DPad.Up == ButtonState.Pressed)
        {
          this.repeatTimer -= Services.ElapsedTime;
          if ((double) this.repeatTimer > 0.0)
            return true;
        }
        bool flag1 = (double) left.Y > 0.0 || currentGamePadState.DPad.Up == ButtonState.Pressed;
        bool flag2 = (double) left.Y < 0.0 || currentGamePadState.DPad.Down == ButtonState.Pressed;
        if (flag1 || flag2)
          this.repeatTimer = 0.2f;
        if (flag2)
        {
          ++this.current;
          if ((double) this.current >= (double) this.stats.Length)
            this.current = (float) (this.stats.Length - 1);
          if ((double) this.scrollOffset < (double) this.current - (double) (this.rowsPerPage - 1))
            this.scrollOffset = (int) this.current - (this.rowsPerPage - 1);
          Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
          return true;
        }
        if (flag1)
        {
          --this.current;
          if ((double) this.current < 0.0)
            this.current = 0.0f;
          if ((double) this.scrollOffset > (double) this.current)
            this.scrollOffset = (int) this.current;
          Sounds.PlaySound(ItemSoundGroup.GuiMoveCursor);
          return true;
        }
        if ((double) left.X != 0.0 && Math.Sign(left.X) == Math.Sign(lastGamePadState.ThumbSticks.Left.X) || currentGamePadState.DPad.Left == ButtonState.Pressed && lastGamePadState.DPad.Left == ButtonState.Pressed || currentGamePadState.DPad.Right == ButtonState.Pressed && lastGamePadState.DPad.Right == ButtonState.Pressed)
        {
          this.lrrepeatTimer -= Services.ElapsedTime;
          if ((double) this.lrrepeatTimer > 0.0)
            return true;
        }
        bool flag3 = (double) left.X < 0.0 || currentGamePadState.DPad.Left == ButtonState.Pressed;
        bool flag4 = (double) left.X > 0.0 || currentGamePadState.DPad.Right == ButtonState.Pressed;
        if ((flag3 || flag4) && (double) this.lrrepeatTimer < 0.5)
          this.lrrepeatTimer = 0.1f;
        if (flag3)
          this.GetPrevPlayer();
        else if (flag4)
          this.GetNextPlayer();
        else if (lastGamePadState.DPad.Left == ButtonState.Released && (lastGamePadState.DPad.Right == ButtonState.Released && (double) left.X == 0.0))
          this.lrrepeatTimer = 0.5f;
      }
      return base.HandleInput(input);
    }

    private void GetPrevPlayer()
    {
      if (--this.currentIndex < 0)
        this.currentIndex = this.instance.PlayerCountToSave - 1;
      this.Reload(this.currentIndex);
    }

    private void GetNextPlayer()
    {
      if (++this.currentIndex == this.instance.PlayerCountToSave)
        this.currentIndex = 0;
      this.Reload(this.currentIndex);
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      spriteBatch.End();
      spriteBatch.BeginTM(this.Matrix);
      Vector2 vector2_1 = new Vector2((float) (this.screenRect.X + 16), (float) (this.screenRect.Y + 12));
      spriteBatch.DrawString(this.Font, Globals2.GameProperties.SaveGame.Header.MapName, vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 6;
      screenRect.Y = (int) ((double) vector2_1.Y + 32.0);
      screenRect.Width -= 12;
      screenRect.Height = 1;
      spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White);
      Color white = Color.White;
      Color wheat = Color.Wheat;
      float scale = 0.6f;
      vector2_1 = new Vector2((float) this.screenRect.X, 0.0f);
      for (int scrollOffset = this.scrollOffset; scrollOffset < this.scrollOffset + this.rowsPerPage && scrollOffset < this.stats.Length; ++scrollOffset)
      {
        PlayerStats.Stat stat = this.stats[scrollOffset];
        Color color = scrollOffset % 2 == 0 ? white : wheat;
        vector2_1.Y = (float) (this.screenRect.Y + 38 + (scrollOffset - this.scrollOffset) * this.rowHeight);
        spriteBatch.DrawString(this.Font, stat.Desc, vector2_1 + new Vector2(16f, 16f), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        string text = stat.Value;
        Vector2 vector2_2 = this.Font.MeasureString(text);
        spriteBatch.DrawString(this.Font, text, vector2_1 + new Vector2((float) (416.0 - (double) vector2_2.X * (double) scale), 16f), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      }
      if (!this.mapStats)
      {
        screenRect.Y = this.screenRect.Y + this.screenRect.Height - 32;
        spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White);
        vector2_1.X = (float) (this.screenRect.X + this.screenRect.Width - 12);
        vector2_1.Y = (float) (this.screenRect.Y + 18);
        string text = string.Format("{0} of {1}", (object) (this.currentIndex + 1), (object) this.instance.PlayerCountToSave);
        float num = this.Font.MeasureString(text).X * 0.5f;
        vector2_1.X -= num;
        spriteBatch.DrawString(this.Font, text, vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        vector2_1.X -= (float) (10.0 + (double) this.gamerTagSizeX * 0.5);
        spriteBatch.DrawString(this.Font, this.gamerTag, vector2_1 + TMFont.yVec, Color.Wheat, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        vector2_1.X = (float) (this.screenRect.X + 16);
        vector2_1.Y = (float) (this.screenRect.Y + this.screenRect.Height - 22);
        spriteBatch.DrawString(this.Font, "Left or Right to view other player stats", vector2_1 + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
      }
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
