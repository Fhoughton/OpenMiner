// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NumberEntryScreen
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

namespace StudioForge.TotalMiner.Screens
{
  internal class NumberEntryScreen : MinerToolScreen
  {
    private int currentSlotX;
    private int currentSlotY;
    private int slotSize;
    private int slotGap;
    private int topGap;
    private string[,] slotText;
    private string value;
    private bool decAllowed;
    private bool negAllowed;
    private Vector2 leftstick;
    private Vector2 rightstick;
    private Vector2 lastleftstick;
    private Vector2 lastrightstick;
    private NumberEntered callback;
    private object state;

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      int defaultValue,
      bool negAllowed)
      : this(player, callback, defaultValue, negAllowed, (object) null)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      int defaultValue,
      bool negAllowed,
      object state)
      : this(player, callback, defaultValue.ToString(), false, negAllowed, state)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      float defaultValue,
      bool decAllowed,
      bool negAllowed)
      : this(player, callback, defaultValue, decAllowed, negAllowed, (object) null)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      float defaultValue,
      bool decAllowed,
      bool negAllowed,
      object state)
      : this(player, callback, decAllowed ? defaultValue.ToString() : ((int) defaultValue).ToString(), decAllowed, negAllowed, state)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      long defaultValue,
      bool negAllowed)
      : this(player, callback, defaultValue, negAllowed, (object) null)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      long defaultValue,
      bool negAllowed,
      object state)
      : this(player, callback, defaultValue.ToString(), false, negAllowed, state)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      double defaultValue,
      bool decAllowed,
      bool negAllowed)
      : this(player, callback, defaultValue, decAllowed, negAllowed, (object) null)
    {
    }

    public NumberEntryScreen(
      Player player,
      NumberEntered callback,
      double defaultValue,
      bool decAllowed,
      bool negAllowed,
      object state)
      : this(player, callback, decAllowed ? defaultValue.ToString() : ((int) defaultValue).ToString(), decAllowed, negAllowed, state)
    {
    }

    private NumberEntryScreen(
      Player player,
      NumberEntered callback,
      string defaultValue,
      bool decAllowed,
      bool negAllowed,
      object state)
      : base(player)
    {
      this.callback = callback;
      this.decAllowed = decAllowed;
      this.negAllowed = negAllowed;
      this.state = state;
      this.value = defaultValue;
      if (decAllowed)
        return;
      int length = this.value.IndexOf('.');
      if (length < 0)
        return;
      this.value = this.value.Substring(0, length);
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.currentSlotX = 2;
      this.currentSlotY = 4;
      this.slotSize = 48;
      this.slotGap = 8;
      this.topGap = this.slotSize;
      this.screenRect = MyExtensions.CenterOfViewport(80 + this.slotSize * 3 + this.slotGap * 2, 32 + this.topGap + this.slotSize * 5 + this.slotGap * 5);
      base.LoadContent();
      this.slotText = new string[3, 5];
      this.slotText[0, 0] = "C";
      this.slotText[1, 0] = "B";
      this.slotText[2, 0] = "-";
      this.slotText[0, 1] = "9";
      this.slotText[1, 1] = "8";
      this.slotText[2, 1] = "7";
      this.slotText[0, 2] = "6";
      this.slotText[1, 2] = "5";
      this.slotText[2, 2] = "4";
      this.slotText[0, 3] = "3";
      this.slotText[1, 3] = "2";
      this.slotText[2, 3] = "1";
      this.slotText[0, 4] = "0";
      this.slotText[1, 4] = ".";
      this.slotText[2, 4] = "OK";
    }

    public virtual bool HandleInput(GamePadState pad, GamePadState lastpad)
    {
      return false;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        this.AButtonPressed();
        return true;
      }
      if (InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.X) || InputManager.IsKeyReleasedNew(this.ControllingPlayer.Value, Keys.Back))
      {
        this.BackPressed();
        return true;
      }
      if (InputManager.IsButtonReleasedNew(this.ControllingPlayer.Value, Buttons.Start) || InputManager.IsKeyReleasedNew(this.ControllingPlayer.Value, Keys.Enter))
      {
        this.OkPressed();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        if (this.callback != null)
          this.callback(0.0, true, this.state);
        return true;
      }
      this.lastleftstick = this.leftstick;
      this.lastrightstick = this.rightstick;
      this.leftstick = currentGamePadState.ThumbSticks.Left;
      this.rightstick = currentGamePadState.ThumbSticks.Right;
      float num = 0.2f;
      if ((double) this.leftstick.X > -(double) num && (double) this.leftstick.X < (double) num)
        this.leftstick.X = 0.0f;
      if ((double) this.leftstick.Y > -(double) num && (double) this.leftstick.Y < (double) num)
        this.leftstick.Y = 0.0f;
      if ((double) this.rightstick.X > -(double) num && (double) this.rightstick.X < (double) num)
        this.rightstick.X = 0.0f;
      if ((double) this.rightstick.Y > -(double) num && (double) this.rightstick.Y < (double) num)
        this.rightstick.Y = 0.0f;
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.CursorLeft))
      {
        this.MoveLeft();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.CursorRight))
      {
        this.MoveRight();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.CursorDown))
      {
        this.MoveDown();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.CursorUp))
      {
        this.MoveUp();
        return true;
      }
      Keys numKeyPressedNew = InputManager.GetNumKeyPressedNew(this.ControllingPlayer.Value);
      if (numKeyPressedNew != Keys.None)
      {
        if (this.value == "0")
          this.value = "";
        this.value += ((int) (numKeyPressedNew - 48)).ToString();
        return true;
      }
      if (InputManager.IsMouseMoved(this.ControllingPlayer.Value))
      {
        Point mousePos = InputManager.GetMousePos(this.ControllingPlayer.Value);
        bool flag = true;
        for (int y = 0; y < 5 && flag; ++y)
        {
          for (int x = 0; x < 3 && flag; ++x)
          {
            Rectangle slotRect = this.GetSlotRect(x, y);
            if (mousePos.X > slotRect.X && mousePos.X < slotRect.X + slotRect.Width - 1 && (mousePos.Y > slotRect.Y && mousePos.Y < slotRect.Y + slotRect.Height - 1))
            {
              this.currentSlotX = x;
              this.currentSlotY = y;
              flag = false;
            }
          }
        }
      }
      if (base.HandleInput(input))
        return true;
      return this.HandleInput(currentGamePadState, lastGamePadState);
    }

    private void AButtonPressed()
    {
      if (this.currentSlotX == 0 && this.currentSlotY == 0)
        this.value = "0";
      else if (this.currentSlotX == 1 && this.currentSlotY == 0)
        this.BackPressed();
      else if (this.currentSlotX == 2 && this.currentSlotY == 0)
      {
        if (!(this.value != "0") || !this.value.IsNotEmpty())
          return;
        if (this.value[0] == '-')
          this.value = this.value.Substring(1, this.value.Length - 1);
        else
          this.value = 45.ToString() + this.value;
      }
      else if (this.currentSlotX == 2 && this.currentSlotY == 4)
      {
        this.OkPressed();
      }
      else
      {
        if (this.slotText.GetLength(0) <= this.currentSlotX || this.slotText.GetLength(1) <= this.currentSlotY || (this.slotText[this.currentSlotX, this.currentSlotY].Length <= 0 || !this.IsValidInput()))
          return;
        if (this.value == "0")
          this.value = "";
        this.value += this.slotText[this.currentSlotX, this.currentSlotY];
      }
    }

    private bool IsValidInput()
    {
      return (this.decAllowed || this.currentSlotX != 1 || this.currentSlotY != 4) && (this.negAllowed || this.currentSlotX != 2 || this.currentSlotY != 0) && this.value.Length <= 9;
    }

    private void BackPressed()
    {
      if (this.value.Length > 1)
        this.value = this.value.Substring(0, this.value.Length - 1);
      else
        this.value = "0";
    }

    private void OkPressed()
    {
      double result;
      if (!double.TryParse(this.value, out result))
        return;
      this.callback(result, false, this.state);
      this.ExitScreen();
    }

    private void MoveLeft()
    {
      if (--this.currentSlotX >= 0)
        return;
      this.currentSlotX = 2;
    }

    private void MoveRight()
    {
      if (++this.currentSlotX <= 2)
        return;
      this.currentSlotX = 0;
    }

    private void MoveUp()
    {
      if (--this.currentSlotY >= 0)
        return;
      this.currentSlotY = 4;
    }

    private void MoveDown()
    {
      if (++this.currentSlotY <= 4)
        return;
      this.currentSlotY = 0;
    }

    protected override void DrawCore()
    {
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.SpriteBatch.End();
      this.SpriteBatch.BeginTM(this.Matrix);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 16;
      screenRect.Y += 16;
      screenRect.Width -= 32;
      screenRect.Height = this.topGap;
      this.SpriteBatch.DrawBox(screenRect, 1, Color.White, 0.0f);
      string text = this.value.ToString();
      Vector2 vector2 = this.Font.MeasureString(text);
      this.SpriteBatch.DrawString(this.Font, text, new Vector2((float) (screenRect.X + screenRect.Width - 10) - vector2.X, (float) (screenRect.Y + 4)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      this.DrawGrid();
      this.SpriteBatch.DrawFilledBox(this.GetSlotRect(this.currentSlotX, this.currentSlotY), 2, Color.Yellow, Color.Yellow * 0.1f);
      this.spriteBatch.End();
    }

    private void DrawGrid()
    {
      for (int y = 0; y < 5; ++y)
      {
        for (int x = 0; x < 3; ++x)
          this.DrawSlot(x, y);
      }
    }

    private void DrawSlot(int x, int y)
    {
      Rectangle slotRect = this.GetSlotRect(x, y);
      this.SpriteBatch.DrawBox(slotRect, 1, Color.White, 0.0f);
      Vector2 position = new Vector2((float) (slotRect.X + 14), (float) (slotRect.Y + 4));
      bool flag = x == 2 && y == 4;
      float scale = flag ? 0.8f : 1f;
      if (flag)
      {
        position.X -= 8f;
        position.Y += 2f;
      }
      this.SpriteBatch.DrawString(this.Font, this.slotText[x, y], position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }

    private Rectangle GetSlotRect(int x, int y)
    {
      Rectangle screenRect = this.screenRect;
      screenRect.X += 40 + x * (this.slotSize + this.slotGap);
      screenRect.Y += 16 + this.topGap + this.slotGap + y * (this.slotSize + this.slotGap);
      screenRect.Width = this.slotSize;
      screenRect.Height = this.slotSize;
      return screenRect;
    }
  }
}
