// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.DataField
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.GUI
{
  public class DataField : TextBox, ITextInputWindow
  {
    protected Action<ITextInputWindow> beginInput;
    protected Action<ITextInputWindow> validateInput;
    protected RawInputFunc rawInput;
    protected ITextInput inputHandler;
    public GetTextInputHander GetNewInputHandler;
    public static DataField.ColorProfile DefaultColorProfile;
    public bool IsNumeric;
    public int MaxLength;
    protected Window virtualKeyboard;
    private int carotBlink;
    private int firstVisibleChar;
    private int lastCursor;
    private float carotMeasX;

    bool ITextInputWindow.EqualsInputWindow(object win)
    {
      return this.EqualsInputWindowCore(win as Window);
    }

    protected override bool EqualsInputWindowCore(Window win)
    {
      return win == this;
    }

    Action<ITextInputWindow> ITextInputWindow.OnBeginInput
    {
      get
      {
        return this.beginInput;
      }
      set
      {
        this.beginInput = value;
      }
    }

    Action<ITextInputWindow> ITextInputWindow.OnValidateInput
    {
      get
      {
        return this.validateInput;
      }
      set
      {
        this.validateInput = value;
      }
    }

    RawInputFunc ITextInputWindow.OnRawInput
    {
      get
      {
        return this.rawInput;
      }
      set
      {
        this.rawInput = value;
      }
    }

    float ITextInputWindow.TextScale
    {
      get
      {
        return this.TextScale;
      }
    }

    string ITextInputWindow.Text
    {
      get
      {
        return this.Text;
      }
      set
      {
        this.Text = value;
      }
    }

    object ITextInputWindow.Tag
    {
      get
      {
        return this.Tag;
      }
    }

    void ITextInputWindow.EndInput(bool needValidate)
    {
      this.inputHandler = (ITextInput) null;
      if (needValidate && this.validateInput != null)
        this.validateInput((ITextInputWindow) this);
      this.OnEndInputCore();
      if (this.virtualKeyboard == null)
        return;
      this.virtualKeyboard.RemoveSelf();
      this.virtualKeyboard = (Window) null;
    }

    protected virtual void OnEndInputCore()
    {
    }

    void ITextInputWindow.CursorMoved(int oldPos, int newPos)
    {
      this.CursorMoved(oldPos, newPos);
    }

    protected virtual void CursorMoved(int oldPos, int newPos)
    {
      this.lastCursor = oldPos;
      if (newPos < this.firstVisibleChar)
        this.firstVisibleChar = newPos;
      this.textChanged = true;
    }

    ITextInput ITextInputWindow.InputHandler
    {
      get
      {
        return this.inputHandler;
      }
    }

    ITextInput ITextInputWindow.GetNewTextInputHandlerOnHover()
    {
      return (ITextInput) null;
    }

    public virtual ITextInput GetNewTextInputHandlerOnClick()
    {
      if (this.GetNewInputHandler != null)
      {
        this.inputHandler = this.GetNewInputHandler((ITextInputWindow) this);
      }
      else
      {
        TextInput textInput = new TextInput()
        {
          MaxLength = this.MaxLength
        };
        ITextInputWindow textInputWindow = (ITextInputWindow) this;
        if (textInputWindow.OnBeginInput != null)
          textInputWindow.OnBeginInput(textInputWindow);
        this.inputHandler = (ITextInput) textInput;
        textInput.SetWindow((ITextInputWindow) this);
      }
      return this.inputHandler;
    }

    protected override Window.ColorProfile InitColorProfile()
    {
      return (Window.ColorProfile) DataField.DefaultColorProfile;
    }

    public override bool IsKeyNavigable
    {
      get
      {
        return true;
      }
    }

    public DataField()
      : base("", 0, 0, 10, 10)
    {
      this.AddFlags(Window.WinFlags.LeftClickOnPress);
    }

    public DataField(string text, int x, int y, int width, int height)
      : this(text, x, y, width, height, 1f)
    {
    }

    public DataField(string text, int x, int y, int width, int height, float textScale)
      : this(text, x, y, width, height, textScale, TextBox.DefaultTextAlignX, WinTextAlignY.Center)
    {
    }

    public DataField(
      string text,
      int x,
      int y,
      int width,
      int height,
      float textScale,
      WinTextAlignX alignX,
      WinTextAlignY alignY)
      : base(text == null ? "" : text, x, y, width, height, textScale, alignX, alignY)
    {
      this.AddFlags(Window.WinFlags.LeftClickOnPress);
    }

    public ITextInput EmptyInputHander(ITextInputWindow win)
    {
      return (ITextInput) null;
    }

    protected override bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      if (keys[0] != Keys.Enter)
        return base.OnKeyReleaseCore(e, keys);
      if (this.inputHandler != null)
        this.inputHandler.EndInput(true);
      return true;
    }

    protected override void OnClickCore(WindowEventArgs e)
    {
      base.OnClickCore(e);
      if (!InputManager.IsUsingGamePad)
        return;
      this.virtualKeyboard = (Window) new VirtualKeyboardCarousal((string) null, VirtualKeyboardCarousal.DefPosition.X, VirtualKeyboardCarousal.DefPosition.Y, VirtualKeyboardCarousal.DefSize.X, VirtualKeyboardCarousal.DefSize.Y, this.inputHandler, this.IsNumeric);
      this.virtualKeyboard.Name = "keyboard";
      this.virtualKeyboard.Colors = (Window.ColorProfile) VirtualKeyboardCarousal.DefColors;
      if (this.inputHandler != null)
        this.inputHandler.SniffHandler = this.virtualKeyboard as IInputHandler;
      if (VirtualKeyboardCarousal.DefParent != null)
        VirtualKeyboardCarousal.DefParent.AddChild((Node) this.virtualKeyboard);
      else
        e.WindowManager.Root.AddChild((Node) this.virtualKeyboard);
    }

    public override Color GetBackColorOverride(Color color)
    {
      if (this.inputHandler == null)
        return color;
      DataField.ColorProfile colors = this.Colors as DataField.ColorProfile;
      if (colors == null)
        return color;
      return colors.BackInputColor;
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      if ((double) scale <= 0.0 || (double) this.TextScale <= 0.0 || ((double) alpha <= 0.0 || this.Font == null) || spriteBatch == null)
        return;
      if (this.inputHandler == null)
      {
        base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
      }
      else
      {
        int val2 = this.inputHandler.Cursor;
        if (this.textChanged)
        {
          if (this.text != null && this.text.Length > 0)
          {
            int num = (int) ((double) bound.Width / (double) scale - ((double) this.TextOffset.X + 12.0));
            if (val2 > this.lastCursor)
            {
              this.drawText = this.text.Substring(this.firstVisibleChar, val2 - this.firstVisibleChar);
              for (Vector2 vector2 = this.Font.MeasureString(this.drawText) * this.TextScale; (double) vector2.X >= (double) num; vector2 = this.Font.MeasureString(this.drawText) * this.TextScale)
              {
                ++this.firstVisibleChar;
                this.drawText = this.text.Substring(this.firstVisibleChar, val2 - this.firstVisibleChar);
              }
            }
            this.drawText = this.text.Substring(this.firstVisibleChar);
            for (Vector2 vector2 = this.Font.MeasureString(this.drawText) * this.TextScale; (double) vector2.X >= (double) num; vector2 = this.Font.MeasureString(this.drawText) * this.TextScale)
              this.drawText = this.drawText.Substring(0, this.drawText.Length - 1);
            this.carotMeasX = val2 > this.firstVisibleChar ? this.Font.MeasureString(this.drawText.Substring(0, val2 - this.firstVisibleChar)).X * this.TextScale : 0.0f;
          }
          else
          {
            val2 = this.lastCursor = 0;
            this.firstVisibleChar = 0;
            this.carotMeasX = 0.0f;
            this.drawText = "";
          }
        }
        Vector2 vector2_1 = this.Font.MeasureString(this.drawText.Length > 0 ? this.drawText : "A");
        vector2_1.X *= scale * this.TextScale;
        vector2_1.Y *= scale * this.TextScale;
        float x = this.TextAlignX == WinTextAlignX.Center ? (float) ((double) bound.X + (double) bound.Width * 0.5 + (double) this.TextOffset.X * (double) scale - (double) vector2_1.X * 0.5) : (this.TextAlignX == WinTextAlignX.Right ? (float) (bound.X + bound.Width) - (6f + this.TextOffset.X) * scale - vector2_1.X : (float) bound.X + (6f + this.TextOffset.X) * scale);
        float y = this.TextAlignY == WinTextAlignY.Center ? (float) ((double) bound.Y + (double) bound.Height * 0.5 + ((double) this.TextOffset.Y + 2.0) * (double) scale - (double) vector2_1.Y * 0.5) : (this.TextAlignY == WinTextAlignY.Bottom ? (float) (bound.Y + bound.Height) - (3f + this.TextOffset.Y) * scale - vector2_1.Y : (float) bound.Y + (3f + this.TextOffset.Y) * scale);
        int selectedTextCursorStart = this.inputHandler.SelectedTextCursorStart;
        Point point = new Point()
        {
          X = (int) ((double) x + (double) this.carotMeasX * (double) scale),
          Y = (int) ((double) y + 2.0 * (double) scale)
        };
        DataField.ColorProfile colors1 = this.Colors as DataField.ColorProfile;
        if (selectedTextCursorStart >= 0 && selectedTextCursorStart != val2 && colors1 != null)
        {
          int num1 = Math.Max(Math.Min(selectedTextCursorStart, val2) - this.firstVisibleChar, 0);
          int num2 = Math.Min(Math.Max(selectedTextCursorStart, val2) - this.firstVisibleChar, this.drawText.Length);
          float num3 = this.Font.MeasureString(this.drawText.Substring(0, num1)).X * this.TextScale;
          Vector2 vector2_2 = this.Font.MeasureString(this.drawText.Substring(num1, num2 - num1)) * this.TextScale;
          Rectangle destinationRectangle = new Rectangle((int) ((double) x + (double) num3 * (double) scale), (int) ((double) y + 2.0 * (double) scale), (int) (((double) vector2_2.X + 2.0) * (double) scale), (int) (((double) vector2_2.Y - 6.0) * (double) scale));
          spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, colors1.BackSelectedTextColor);
        }
        TextBox.ColorProfile colors2 = this.Colors as TextBox.ColorProfile;
        Color color = colors2 != null ? colors2.TextColor : this.Colors.ForeColor;
        Vector2 position = new Vector2(x, y);
        spriteBatch.DrawString(this.Font, this.drawText, position, color, 0.0f, Vector2.Zero, this.TextScale * scale, SpriteEffects.None, 0.0f);
        if (++this.carotBlink > 60)
          this.carotBlink = 0;
        if (this.carotBlink >= 30 && this.inputHandler.CanCarotBlink)
          return;
        this.DrawCarot(spriteBatch, point.X, (int) ((double) point.Y + 2.0 * (double) scale), (int) ((double) vector2_1.Y - 8.0 * (double) scale), scale, color);
      }
    }

    protected virtual void DrawCarot(
      SpriteBatchSafe spriteBatch,
      int x,
      int y,
      int h,
      float scale,
      Color color)
    {
      spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(x, y, 2, h), color);
    }

    static DataField()
    {
      DataField.ColorProfile colorProfile = new DataField.ColorProfile();
      colorProfile.ForeColor = Color.White;
      colorProfile.BackColor = new Color(192, 192, 192);
      colorProfile.BackHoverColor = new Color(224, 224, 224);
      colorProfile.BackClickColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      colorProfile.BackDisabledColor = new Color(160, 160, 160);
      colorProfile.BorderColor = Color.Black;
      colorProfile.TextColor = Color.Black;
      colorProfile.BackInputColor = Color.LightBlue;
      colorProfile.BackSelectedTextColor = Color.CornflowerBlue;
      DataField.DefaultColorProfile = colorProfile;
    }

    public class ColorProfile : TextBox.ColorProfile
    {
      public Color BackInputColor;
      public Color BackSelectedTextColor;

      public override Window.ColorProfile Copy(Window.ColorProfile result)
      {
        DataField.ColorProfile colorProfile = base.Copy(result) as DataField.ColorProfile;
        if (colorProfile != null)
        {
          colorProfile.BackInputColor = this.BackInputColor;
          colorProfile.BackSelectedTextColor = this.BackSelectedTextColor;
        }
        return result;
      }
    }
  }
}
