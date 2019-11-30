// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.VirtualKeyboardCarousal
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System.Text;

namespace StudioForge.Engine.GUI
{
  public class VirtualKeyboardCarousal : TextBox, IInputHandler
  {
    private const int pageScrollSize = 7;
    private const int symbolCount = 10;
    public static Window DefParent;
    public static Point DefPosition;
    public static Point DefSize;
    public static TextBox.ColorProfile DefColors;
    private char[] chars;
    private int lettersIndex;
    private int cursor;
    private int cursorOffset;
    private int slotSize;
    private int pageSize;
    private int pageScrollCount;
    private int pageScrollDir;
    private float scrollTime;
    private float scrollTimer;
    private bool capsLock;
    private bool numbersOnly;
    private ITextInput inputHandler;
    private Texture2D symbolsTex;

    bool IInputHandler.FullInputControl
    {
      get
      {
        return true;
      }
    }

    private bool CanMoveCursor
    {
      get
      {
        return this.pageScrollCount < 1;
      }
    }

    public VirtualKeyboardCarousal(
      string text,
      int x,
      int y,
      int width,
      int height,
      ITextInput inputHandler,
      bool numbersOnly)
      : base(text, x, y, width, height)
    {
      this.inputHandler = inputHandler;
      this.numbersOnly = numbersOnly;
      this.InitChars();
      this.scrollTime = 0.02142857f;
      this.slotSize = 32;
      this.symbolsTex = CoreGlobals.Content.Load<Texture2D>("Textures\\virtkeysymbols");
      this.cursorOffset = 0;
      this.cursor = this.lettersIndex;
      this.CapsLock(true);
    }

    private void InitChars()
    {
      int num1 = 78;
      if (this.numbersOnly)
        num1 -= 27;
      this.chars = new char[num1 + 10];
      int num2 = 0;
      for (int index = 0; index < 10; ++index)
        this.chars[num2++] = char.MinValue;
      char[] chars1 = this.chars;
      int index1 = num2;
      int num3 = index1 + 1;
      chars1[index1] = ' ';
      this.lettersIndex = num3;
      if (!this.numbersOnly)
      {
        for (int index2 = 65; index2 <= 90; ++index2)
          this.chars[num3++] = (char) index2;
        this.chars[num3++] = ' ';
      }
      char[] chars2 = this.chars;
      int index3 = num3;
      int num4 = index3 + 1;
      chars2[index3] = ',';
      char[] chars3 = this.chars;
      int index4 = num4;
      int num5 = index4 + 1;
      chars3[index4] = '.';
      for (int index2 = 48; index2 <= 57; ++index2)
        this.chars[num5++] = (char) index2;
      for (int index2 = 32; index2 <= 43; ++index2)
        this.chars[num5++] = (char) index2;
      char[] chars4 = this.chars;
      int index5 = num5;
      int num6 = index5 + 1;
      chars4[index5] = '-';
      char[] chars5 = this.chars;
      int index6 = num6;
      int num7 = index6 + 1;
      chars5[index6] = '/';
      for (int index2 = 58; index2 <= 64; ++index2)
        this.chars[num7++] = (char) index2;
      for (int index2 = 91; index2 <= 96; ++index2)
        this.chars[num7++] = (char) index2;
      for (int index2 = 123; index2 <= 126; ++index2)
        this.chars[num7++] = (char) index2;
      for (int index2 = 145; index2 <= 148; ++index2)
        this.chars[num7++] = (char) index2;
      char[] chars6 = this.chars;
      int index7 = num7;
      int num8 = index7 + 1;
      chars6[index7] = '\x0082';
      char[] chars7 = this.chars;
      int index8 = num8;
      int num9 = index8 + 1;
      chars7[index8] = '\x0084';
      char[] chars8 = this.chars;
      int index9 = num9;
      int num10 = index9 + 1;
      chars8[index9] = '\x0098';
    }

    public bool HandleInput(PlayerIndex playerIndex)
    {
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.A) || InputManager.IsMouseButtonPressedNew(playerIndex, MouseButtons.LeftButton))
      {
        switch (this.cursorOffset + this.cursor)
        {
          case 0:
            this.inputHandler.AbortInput();
            break;
          case 2:
            this.inputHandler.EndInput(true);
            break;
          case 4:
            this.inputHandler.HandleInput(Keys.Home);
            break;
          case 5:
            this.inputHandler.HandleInput(Keys.End);
            break;
          case 6:
            this.inputHandler.HandleInput(Keys.Back);
            break;
          case 7:
            this.inputHandler.HandleInput(Keys.Left);
            break;
          case 8:
            this.inputHandler.HandleInput(Keys.Right);
            break;
          case 9:
            this.CapsLock(!this.capsLock);
            break;
          default:
            this.inputHandler.InsertChar(this.chars[this.cursorOffset + this.cursor]);
            break;
        }
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.X))
      {
        this.inputHandler.HandleInput(Keys.Back);
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.Y))
      {
        this.inputHandler.HandleInput(Keys.End);
        return true;
      }
      if (InputManager.IsButtonReleasedNew(playerIndex, Buttons.B))
      {
        this.inputHandler.AbortInput();
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.Back))
      {
        this.inputHandler.HandleInput(Keys.Home);
        return true;
      }
      if (InputManager.IsButtonReleasedNew(playerIndex, Buttons.Start))
      {
        this.inputHandler.EndInput(true);
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.DPadLeft))
      {
        if (this.CanMoveCursor)
        {
          if (this.cursorOffset + this.cursor > 0)
          {
            this.MoveLeft();
          }
          else
          {
            this.cursorOffset = this.chars.Length - this.pageSize;
            this.cursor = this.pageSize - 1;
          }
        }
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.DPadRight))
      {
        if (this.CanMoveCursor)
        {
          if (this.cursorOffset + this.cursor < this.chars.Length - 1)
            this.MoveRight();
          else
            this.cursorOffset = this.cursor = 0;
        }
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.LeftShoulder))
      {
        this.inputHandler.HandleInput(Keys.Left);
        return true;
      }
      if (InputManager.IsButtonPressedNew(playerIndex, Buttons.RightShoulder))
      {
        this.inputHandler.HandleInput(Keys.Right);
        return true;
      }
      if (InputManager.IsButtonPressed(playerIndex, Buttons.LeftTrigger))
      {
        if (this.CanMoveCursor)
        {
          int num = this.cursorOffset + this.cursor;
          this.pageScrollCount = 7;
          if (num > this.lettersIndex && num < this.lettersIndex + this.pageScrollCount)
            this.pageScrollCount = num - this.lettersIndex + 2;
          this.pageScrollDir = -1;
          this.scrollTimer = this.scrollTime;
        }
        return true;
      }
      if (InputManager.IsButtonPressed(playerIndex, Buttons.RightTrigger))
      {
        if (this.CanMoveCursor)
        {
          int num = this.cursorOffset + this.cursor;
          this.pageScrollCount = 7;
          if (num < this.lettersIndex - 2)
            this.pageScrollCount = this.lettersIndex - num - 2;
          this.pageScrollDir = 1;
          this.scrollTimer = this.scrollTime;
        }
        return true;
      }
      if (InputManager.IsButtonPressed(playerIndex, Buttons.DPadUp))
      {
        if (this.CanMoveCursor)
        {
          this.pageScrollCount = this.cursorOffset + this.cursor;
          this.pageScrollDir = -1;
          this.scrollTimer = this.scrollTime;
        }
        return true;
      }
      if (InputManager.IsButtonPressed(playerIndex, Buttons.DPadDown))
      {
        if (this.CanMoveCursor)
        {
          this.pageScrollCount = this.chars.Length - (this.cursorOffset + this.cursor);
          this.pageScrollDir = 1;
          this.scrollTimer = this.scrollTime;
        }
        return true;
      }
      int mouseWheelDelta = InputManager.GetMouseWheelDelta(playerIndex);
      if (mouseWheelDelta != 0 && this.CanMoveCursor)
      {
        if (mouseWheelDelta > 0)
          this.MoveLeft();
        else
          this.MoveRight();
        return true;
      }
      if (InputManager.GetMousePosDelta(playerIndex) != Vector2.Zero)
      {
        Point mousePos = InputManager.GetMousePos(playerIndex);
        Vector2 worldPosition = this.WorldPosition;
        if ((double) mousePos.X >= (double) worldPosition.X && (double) mousePos.X < (double) worldPosition.X + (double) this.Size.X && ((double) mousePos.Y >= (double) worldPosition.Y && (double) mousePos.Y < (double) worldPosition.Y + (double) this.Size.Y))
        {
          this.cursor = (int) (((double) mousePos.X - (double) worldPosition.X) / (double) this.slotSize);
          return true;
        }
      }
      return false;
    }

    private void CapsLock(bool caps)
    {
      this.capsLock = caps;
      if (this.numbersOnly)
        return;
      int num = caps ? 65 : 97;
      int lettersIndex = this.lettersIndex;
      for (int index = num; index < num + 26; ++index)
        this.chars[lettersIndex++] = (char) index;
    }

    private void MoveLeft()
    {
      if (this.cursor > 0)
      {
        --this.cursor;
      }
      else
      {
        if (this.cursorOffset <= 0)
          return;
        --this.cursorOffset;
      }
    }

    private void MoveRight()
    {
      if (this.cursor < this.pageSize - 1)
      {
        ++this.cursor;
      }
      else
      {
        if (this.cursorOffset + this.pageSize >= this.chars.Length)
          return;
        ++this.cursorOffset;
      }
    }

    private void Update()
    {
      if ((double) this.scrollTimer <= 0.0)
        return;
      this.scrollTimer -= Services.ElapsedTime;
      if ((double) this.scrollTimer > 0.0)
        return;
      if (this.pageScrollDir < 0)
        this.MoveLeft();
      else
        this.MoveRight();
      if (--this.pageScrollCount <= 0)
        return;
      this.scrollTimer = this.scrollTime;
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      this.Update();
      base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
      int height = bound.Height - 4;
      Vector2 vector2_1 = new Vector2((float) bound.X, (float) bound.Y);
      Color textColor = ((TextBox.ColorProfile) this.Colors).TextColor;
      StringBuilder text = new StringBuilder();
      this.pageSize = 0;
      int cursorOffset = this.cursorOffset;
      while (cursorOffset < this.chars.Length)
      {
        if (cursorOffset >= 10)
        {
          text.Append(this.chars[cursorOffset]);
          Vector2 vector2_2 = this.Font.MeasureString(text.ToString()) * this.TextScale;
          Vector2 position = new Vector2((float) (((double) this.slotSize - (double) vector2_2.X) * 0.5) + vector2_1.X, (float) (((double) height - (double) vector2_2.Y) * 0.5 + (double) vector2_1.Y + 2.0));
          spriteBatch.DrawString(this.Font, text, position, textColor, this.TextRotation, this.TextOrigin, this.TextScale, SpriteEffects.None, 0.0f);
          text.Clear();
        }
        else
        {
          Rectangle rectangle = new Rectangle(cursorOffset * 31, 0, 30, 32);
          Rectangle destinationRectangle = new Rectangle((int) vector2_1.X + 1, (int) ((double) (height - 32) * 0.5 + (double) vector2_1.Y), 30, 32);
          spriteBatch.Draw(this.symbolsTex, destinationRectangle, new Rectangle?(rectangle), textColor);
          Texture2D buttonTexture = this.GetButtonTexture(cursorOffset);
          if (buttonTexture != null)
          {
            destinationRectangle.Y += 22;
            destinationRectangle.Width = destinationRectangle.Height = 16;
            destinationRectangle.X += (this.slotSize - 16) / 2 - 1;
            spriteBatch.Draw(buttonTexture, destinationRectangle, textColor);
          }
        }
        vector2_1.X += (float) this.slotSize;
        if ((double) vector2_1.X <= (double) (bound.X + bound.Width - this.slotSize))
        {
          ++cursorOffset;
          ++this.pageSize;
        }
        else
          break;
      }
      Rectangle rect = new Rectangle(bound.X + this.cursor * this.slotSize - 1, bound.Y + 2, this.slotSize + 2, height);
      spriteBatch.DrawFilledBox(rect, 1, Color.Green, Color.DarkGreen * 0.5f);
    }

    private Texture2D GetButtonTexture(int i)
    {
      switch (i)
      {
        case 0:
          return CoreGlobals.ButtonTextureB;
        case 2:
          return CoreGlobals.ButtonTextureStart;
        case 4:
          return CoreGlobals.ButtonTextureBack;
        case 5:
          return CoreGlobals.ButtonTextureY;
        case 6:
          return CoreGlobals.ButtonTextureX;
        case 7:
          return CoreGlobals.ButtonTextureLB;
        case 8:
          return CoreGlobals.ButtonTextureRB;
        default:
          return (Texture2D) null;
      }
    }
  }
}
