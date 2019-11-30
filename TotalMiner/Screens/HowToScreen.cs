// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HowToScreen
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
  internal class HowToScreen : MinerToolScreen
  {
    public int HowToID = -1;
    private string heading;
    private string howToText;
    private string[] text;
    private Vector2 textMeasure;
    private Vector2 headingMeasure;
    private float textScale;
    private int current;
    private int pagesize;
    private float repeatTimer;
    private int textHeight;
    private bool readAll;
    private Color colorWhite;
    private Color colorBlack;

    public HowToScreen(Player player, string heading, string howToText)
      : this(player, heading, howToText, 0.6f)
    {
    }

    public HowToScreen(Player player, string heading, string howToText, float textScale)
      : base(player)
    {
      this.heading = heading;
      this.howToText = howToText;
      this.textScale = textScale;
    }

    public override void LoadContent()
    {
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.Font = CoreGlobals.GameFont;
      this.text = Utils.BreakIntoLines(this.Font, 1040, this.textScale, this.howToText, true);
      this.textMeasure = Utils.MeasureText(this.Font, this.text, this.textScale);
      this.pagesize = 22;
      if (this.player != null && this.player.GameInstance != null && this.player.GameInstance.LocalPlayerCount > 1 && (this.player.GameInstance.LocalPlayerCount != 2 || !Globals2.GameSettings.SplitScreenVertical))
        this.pagesize = 12;
      this.textHeight = (int) (32.0 * (double) this.textScale) + 1;
      this.heading = Utils.InsertSpacesBeforeCapitals(this.heading);
      this.headingMeasure = CoreGlobals.GameFont.MeasureString(this.heading);
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, (int) Math.Max(200f, Math.Max(this.textMeasure.X, this.headingMeasure.X) + 32f), Math.Min(this.text.Length, this.pagesize) * this.textHeight + 130);
      base.LoadContent();
      if (this.HowToID < 0 || this.text.Length >= this.pagesize)
        return;
      this.readAll = true;
      if (this.player == null)
        return;
      this.player.Raise_ReadHowTo(this.HowToID);
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      KeyboardState currentKeyboardState = input.CurrentKeyboardStates[(int) this.ControllingPlayer.Value];
      KeyboardState lastKeyboardState = input.LastKeyboardStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen) || InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.A))
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
      {
        this.ScrollUpPage();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
      {
        this.ScrollDownPage();
        return true;
      }
      Vector2 left = currentGamePadState.ThumbSticks.Left;
      if ((double) left.Y != 0.0 && Math.Sign(left.Y) == Math.Sign(lastGamePadState.ThumbSticks.Left.Y) || currentGamePadState.DPad.Up == ButtonState.Pressed && lastGamePadState.DPad.Up == ButtonState.Pressed || (currentGamePadState.DPad.Down == ButtonState.Pressed && lastGamePadState.DPad.Down == ButtonState.Pressed || (currentKeyboardState.IsKeyDown(Keys.Up) && lastKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.Down) && lastKeyboardState.IsKeyDown(Keys.Down))))
      {
        this.repeatTimer -= Services.ElapsedTime;
        if ((double) this.repeatTimer > 0.0)
          return true;
      }
      bool flag1 = (double) left.Y > 0.0 || currentGamePadState.DPad.Up == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Up);
      bool flag2 = (double) left.Y < 0.0 || currentGamePadState.DPad.Down == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Down);
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(this.ControllingPlayer.Value);
      if (mouseWheelDelta > 0)
        flag1 = true;
      else if (mouseWheelDelta < 0)
        flag2 = true;
      if (flag1 || flag2)
        this.repeatTimer = 0.1f;
      if (flag2)
      {
        if (this.current < this.text.Length - this.pagesize)
          ++this.current;
        if (!this.readAll && this.HowToID >= 0)
        {
          this.readAll = true;
          if (this.player != null)
            this.player.Raise_ReadHowTo(this.HowToID);
        }
        return true;
      }
      if (!flag1)
        return base.HandleInput(input);
      if (this.current > 0)
        --this.current;
      return true;
    }

    private void ScrollUpPage()
    {
      if (this.current <= 0)
        return;
      this.current -= this.pagesize;
      if (this.current >= 0)
        return;
      this.current = 0;
    }

    private void ScrollDownPage()
    {
      if (this.current + this.pagesize >= this.text.Length)
        return;
      this.current += this.pagesize;
      if (this.current + this.pagesize < this.text.Length)
        return;
      this.current = this.text.Length - this.pagesize;
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      float num = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num;
      this.colorBlack = Color.Black * num;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      Vector2 position = new Vector2((float) (this.screenRect.X + 16), (float) (this.screenRect.Y + 8));
      this.spriteBatch.DrawString(CoreGlobals.GameFont, this.heading, position, Color.Yellow * num);
      Rectangle screenRect = this.screenRect;
      screenRect.Y = (int) ((double) position.Y + (double) this.headingMeasure.Y + 8.0);
      screenRect.Height = 1;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White * num);
      position.Y = (float) (screenRect.Y + 8);
      for (int current = this.current; current < this.current + this.pagesize && current < this.text.Length; ++current)
      {
        this.spriteBatch.DrawString(this.Font, this.text[current], position, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        position.Y += (float) this.textHeight;
      }
      screenRect.Y = (int) position.Y + 18;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White * num);
      position.Y = (float) (screenRect.Y + 8);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, new Rectangle((int) position.X, (int) position.Y + 2, 24, 24), Color.White * num);
      position.X += 42f;
      position.Y += 5f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "Close.  Use Up/Down/Triggers to scroll.", position + TMFont.yVec, Color.White * num, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      CoreGlobals.FrameRateCounter.SpriteCalls += 2;
    }
  }
}
