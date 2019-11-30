// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.ConsoleWindow
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GUI
{
  public class ConsoleWindow : DataField, IOutputLog
  {
    public new static DataField.ColorProfile DefaultColorProfile;
    public Action<string> CommandHandler;
    public Action CloseHandler;
    public string ExitCommand;
    public Keys ExitKey;
    private float lineHeight;
    private int pageSize;
    private int streamSize;
    private int carotWidth;
    private float pageTop;
    private List<string> commandHistory;
    private List<string> output;
    private string prompt;
    private int upKeyLine;

    int IOutputLog.LineCount
    {
      get
      {
        return this.output.Count;
      }
    }

    public void WriteLine(string s)
    {
      this.output.Add(s);
      if (this.output.Count - (int) this.pageTop >= this.pageSize)
        this.pageTop = (float) (this.output.Count - this.pageSize + 1);
      this.ClampStream();
    }

    protected override Window.ColorProfile InitColorProfile()
    {
      return (Window.ColorProfile) ConsoleWindow.DefaultColorProfile;
    }

    public override bool IsKeyNavigable
    {
      get
      {
        return false;
      }
    }

    public ConsoleWindow(int x, int y, int width, int height, Action<string> commandHandler)
      : this(x, y, width, height, 0.5f, commandHandler)
    {
    }

    public ConsoleWindow(
      int x,
      int y,
      int width,
      int height,
      float textScale,
      Action<string> commandHandler)
      : base((string) null, x, y, width, height, textScale)
    {
      this.CommandHandler = commandHandler;
      this.commandHistory = new List<string>();
      this.streamSize = 1000;
      this.output = new List<string>(this.streamSize);
      this.prompt = ">";
      this.TextAlignX = WinTextAlignX.Left;
      this.TextAlignY = WinTextAlignY.None;
      this.BorderThickness = 2;
      this.ExitCommand = "exit";
      this.DragEndHandler += new Window.WindowDragHandler(this.MyDragEndHandler);
    }

    private void MyDragEndHandler(object sender, WindowDragEventArgs args)
    {
      this.Position = new Vector2((float) args.MousePosition.X, (float) args.MousePosition.Y) - ((Window) this.parent).WorldPosition;
    }

    public void SetPrompt(string prompt)
    {
      this.prompt = prompt;
    }

    public void SetConsoleSize(int x, int y)
    {
      if (this.Font == null)
        return;
      Vector2 vector2 = this.Font.MeasureString("A") * this.TextScale;
      if (x > 0)
        this.Size.X = (int) ((double) vector2.X * (double) x);
      if (y <= 0)
        return;
      this.Size.Y = (int) ((double) vector2.Y * (double) y);
      if ((double) this.lineHeight <= 0.0)
        return;
      this.pageSize = (int) ((double) this.Size.Y / (double) this.lineHeight);
    }

    public void SetConsoleFontSize(float textScale)
    {
      if (this.Font == null)
        return;
      Vector2 vector2 = this.Font.MeasureString("A") * this.TextScale;
      int x = (int) ((double) this.Size.X / (double) vector2.X);
      int y = (int) ((double) this.Size.Y / (double) vector2.Y);
      this.TextScale = textScale;
      this.lineHeight = 0.0f;
      this.SetConsoleSize(x, y);
    }

    public void SetConsoleStreamSize(int streamSize)
    {
      this.streamSize = streamSize;
      this.ClampStream();
    }

    public void ClearScreen()
    {
      this.output.Clear();
      this.pageTop = 0.0f;
    }

    private void ClampStream()
    {
      if (this.output.Count <= this.streamSize)
        return;
      this.output.RemoveRange(0, this.output.Count - this.streamSize);
    }

    public override ITextInput GetNewTextInputHandlerOnClick()
    {
      TextInput textInput = new TextInput(new Func<Keys, bool, bool, bool>(this.HandleSpecialKey), new Func<Keys[], bool>(this.ProcessLastKeys));
      ITextInputWindow textInputWindow = (ITextInputWindow) this;
      if (textInputWindow.OnBeginInput != null)
        textInputWindow.OnBeginInput(textInputWindow);
      textInput.SetWindow((ITextInputWindow) this);
      this.inputHandler = (ITextInput) textInput;
      return (ITextInput) textInput;
    }

    private bool HandleSpecialKey(Keys key, bool shift, bool ctrl)
    {
      switch (key)
      {
        case Keys.Enter:
          string str = this.Text.Trim();
          if (str.Length > 0 && (this.commandHistory.Count == 0 || this.commandHistory[this.commandHistory.Count - 1] != this.Text))
            this.commandHistory.Add(this.Text);
          this.WriteLine(this.prompt + this.Text);
          this.upKeyLine = this.commandHistory.Count;
          if (this.CommandHandler != null)
            this.CommandHandler(str);
          this.Text = "";
          ((TextInput) this.inputHandler).SetWindow((ITextInputWindow) this);
          return true;
        case Keys.PageUp:
          this.pageTop -= (float) (this.pageSize - 1);
          if ((double) this.pageTop < 0.0)
            this.pageTop = 0.0f;
          return true;
        case Keys.PageDown:
          this.pageTop += (float) (this.pageSize - 1);
          if ((double) this.pageTop > (double) this.output.Count)
            this.pageTop = (float) Math.Max(0, this.output.Count);
          return true;
        case Keys.Up:
          if (this.upKeyLine > 0 && --this.upKeyLine < this.commandHistory.Count)
          {
            this.Text = this.commandHistory[this.upKeyLine];
            ((TextInput) this.inputHandler).SetWindow((ITextInputWindow) this);
          }
          else
            this.upKeyLine = 0;
          return true;
        case Keys.Down:
          if (++this.upKeyLine < this.commandHistory.Count)
          {
            this.Text = this.commandHistory[this.upKeyLine];
            ((TextInput) this.inputHandler).SetWindow((ITextInputWindow) this);
          }
          else
            this.upKeyLine = this.commandHistory.Count - 1;
          return true;
        default:
          return false;
      }
    }

    private bool ProcessLastKeys(Keys[] lastKeys)
    {
      switch (lastKeys[0])
      {
        case Keys.Enter:
          if (this.commandHistory.Count > 0 && this.commandHistory[this.commandHistory.Count - 1].Equals(this.ExitCommand, StringComparison.OrdinalIgnoreCase))
          {
            this.inputHandler.EndInput(false);
            return true;
          }
          break;
        case Keys.Escape:
          this.Text = "";
          ((TextInput) this.inputHandler).SetWindow((ITextInputWindow) this);
          return true;
        default:
          if (lastKeys[0] == this.ExitKey && this.ExitKey != Keys.None)
          {
            this.inputHandler.EndInput(false);
            return true;
          }
          break;
      }
      return false;
    }

    protected override void OnEndInputCore()
    {
      base.OnEndInputCore();
      if (this.CloseHandler == null)
        return;
      this.CloseHandler();
    }

    protected override bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      if (keys[0] == Keys.Enter || keys[0] == Keys.Escape)
        return false;
      return base.OnKeyReleaseCore(e, keys);
    }

    protected override void OnMouseWheelDeltaCore(WindowEventArgs e, int delta)
    {
      this.pageTop = MathHelper.Clamp(this.pageTop - (float) delta * 0.01f, 0.0f, (float) Math.Max(0, this.output.Count));
      base.OnMouseWheelDeltaCore(e, delta);
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
      if ((double) this.lineHeight == 0.0)
      {
        Vector2 vector2 = this.Font.MeasureString("A");
        this.lineHeight = (float) ((double) vector2.Y * (double) this.TextScale - 1.0);
        this.pageSize = (int) ((double) this.Size.Y / (double) this.lineHeight);
        this.carotWidth = (int) ((double) vector2.X * (double) this.TextScale);
      }
      TextBox.ColorProfile colors = this.Colors as TextBox.ColorProfile;
      Color color = colors != null ? colors.TextColor : this.Colors.ForeColor;
      float x = (float) bound.X + (6f + this.TextOffset.X) * scale;
      float y = (float) bound.Y + 3f * scale;
      Vector2 position = new Vector2(x, y);
      for (int pageTop = (int) this.pageTop; pageTop < this.output.Count; ++pageTop)
      {
        spriteBatch.DrawString(this.Font, this.output[pageTop], position, color, 0.0f, Vector2.Zero, this.TextScale, SpriteEffects.None, 0.0f);
        position.Y += this.lineHeight;
        if ((double) position.Y > (double) (bound.Y + bound.Height) - (double) this.lineHeight + 2.0)
          return;
      }
      spriteBatch.DrawString(this.Font, this.prompt, position, color, 0.0f, Vector2.Zero, this.TextScale, SpriteEffects.None, 0.0f);
      bound.X += (int) (((double) this.Font.MeasureString(this.prompt).X + (double) this.Font.Spacing) * (double) this.TextScale);
      this.TextOffset.Y = position.Y - y;
      base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
    }

    protected override void DrawCarot(
      SpriteBatchSafe spriteBatch,
      int x,
      int y,
      int h,
      float scale,
      Color color)
    {
      spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(x, (int) ((double) (y + h) - 2.0 * (double) scale), this.carotWidth, 3), color);
    }

    static ConsoleWindow()
    {
      DataField.ColorProfile colorProfile = new DataField.ColorProfile();
      colorProfile.ForeColor = DataField.DefaultColorProfile.ForeColor;
      colorProfile.BackColor = Color.Black * 0.9f;
      colorProfile.BackDisabledColor = Color.Black * 0.9f;
      colorProfile.BackClickColor = Color.Black * 0.9f;
      colorProfile.BackHoverColor = Color.Black * 0.9f;
      colorProfile.BorderColor = Color.White;
      colorProfile.TextColor = Color.White;
      colorProfile.BackInputColor = Color.Black * 0.9f;
      colorProfile.BackSelectedTextColor = Color.LightBlue * 0.7f;
      ConsoleWindow.DefaultColorProfile = colorProfile;
    }
  }
}
