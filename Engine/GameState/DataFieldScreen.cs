// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.DataFieldScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.GameState
{
  internal class DataFieldScreen : TextBoxScreen, ITextInputWindow
  {
    private AsyncCallback callback;
    private object asynchState;
    private TextInput textInput;
    private int carotBlink;

    object ITextInputWindow.Tag
    {
      get
      {
        return (object) null;
      }
    }

    Action<ITextInputWindow> ITextInputWindow.OnBeginInput { get; set; }

    Action<ITextInputWindow> ITextInputWindow.OnValidateInput { get; set; }

    RawInputFunc ITextInputWindow.OnRawInput { get; set; }

    ITextInput ITextInputWindow.InputHandler
    {
      get
      {
        return (ITextInput) this.textInput;
      }
    }

    ITextInput ITextInputWindow.GetNewTextInputHandlerOnClick()
    {
      return (ITextInput) null;
    }

    ITextInput ITextInputWindow.GetNewTextInputHandlerOnHover()
    {
      return (ITextInput) null;
    }

    void ITextInputWindow.CursorMoved(int oldPos, int newPos)
    {
    }

    bool ITextInputWindow.EqualsInputWindow(object win)
    {
      return true;
    }

    void ITextInputWindow.EndInput(bool needValidate)
    {
      this.callback((IAsyncResult) new Guide.AsyncResult(this.asynchState)
      {
        AsyncString = this.Text
      });
      this.ExitScreen();
    }

    public DataFieldScreen(
      string title,
      string description,
      string defaultText,
      AsyncCallback callback,
      object asynchState,
      Rectangle rect,
      float scale,
      TextInput textInput)
      : base(defaultText, scale, rect)
    {
      this.callback = callback;
      this.asynchState = asynchState;
      this.textInput = textInput;
      textInput.SetWindow((ITextInputWindow) this);
      this.IsPopup = true;
      this.FadeBehindIfPopup = false;
      this.backColor = new Color(20, 20, 20, (int) byte.MaxValue);
      this.borderColor = Color.White;
      this.TextPos.Y = (float) rect.Y;
    }

    public override void LoadContent()
    {
      base.LoadContent();
    }

    public override bool HandleInput(InputState input)
    {
      if (this.textInput.HandleInput(this.ControllingPlayer.Value))
        return true;
      return base.HandleInput(input);
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
    }

    protected override void DrawCore()
    {
      base.DrawCore();
    }

    protected override void DrawText()
    {
      int cursor = this.textInput.Cursor;
      Vector2 vector2 = this.Text.Length > 0 ? this.Font.MeasureString(this.Text.Substring(0, Math.Max(1, cursor))) : Vector2.Zero;
      vector2.X *= this.TextScale;
      vector2.Y *= this.TextScale;
      int selectedTextCursorStart = this.textInput.SelectedTextCursorStart;
      int num1 = 0;
      int num2 = this.screenRect.X + 6 + (cursor > 0 ? (int) vector2.X : 0);
      int y = (int) (((double) this.screenRect.Height - (double) vector2.Y) / 2.0 + (double) this.TextPos.Y + 3.0);
      Point point = new Point() { X = num2, Y = y };
      if (point.X + 1 >= this.screenRect.X + this.screenRect.Width - 3)
        num1 += point.X + 1 - (this.screenRect.X + this.screenRect.Width - 3);
      if (selectedTextCursorStart >= 0)
      {
        int length1 = Math.Min(cursor, selectedTextCursorStart);
        int num3 = this.screenRect.X + 6 + (length1 > 0 ? (int) ((double) this.Font.MeasureString(this.Text.Substring(0, length1)).X * (double) this.TextScale) : 0);
        int length2 = Math.Max(cursor, selectedTextCursorStart);
        int num4 = this.screenRect.X + 6 + (length2 > 0 ? (int) ((double) this.Font.MeasureString(this.Text.Substring(0, length2)).X * (double) this.TextScale) : 0);
        this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(num3 - num1, y, num4 - num3, (int) ((double) this.TextScale * 38.0)), Color.Blue);
      }
      this.SpriteBatch.DrawString(this.Font, this.Text, new Vector2((float) (this.screenRect.X + 4), (float) (y - 2))
      {
        X = this.TextPos.X - (float) num1
      }, this.TextColor, 0.0f, Vector2.Zero, this.TextScale, SpriteEffects.None, 0.0f);
      if (++this.carotBlink > 60)
        this.carotBlink = 0;
      if (this.carotBlink >= 30 && this.textInput.CanCarotBlink)
        return;
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(point.X - num1, point.Y, 2, (int) ((double) this.TextScale * 30.0)), this.TextColor);
    }
  }
}
