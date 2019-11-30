// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.TextBox
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.GUI
{
  public class TextBox : Window
  {
    public static TextBox.ColorProfile DefaultColorProfile;
    public static WinTextAlignX DefaultTextAlignX;
    public static Color DefaultTextColor;
    public float TextScale;
    public float TextRotation;
    public Vector2 TextOrigin;
    public Vector2 TextOffset;
    public WinTextAlignX TextAlignX;
    public WinTextAlignY TextAlignY;
    public SpriteFont Font;
    public ushort MinTextEdge;
    protected string text;
    protected string drawText;
    protected bool textChanged;

    protected override Window.ColorProfile InitColorProfile()
    {
      return (Window.ColorProfile) TextBox.DefaultColorProfile;
    }

    public bool IsTextEmpty
    {
      get
      {
        if (this.text != null)
          return this.text.Length < 1;
        return true;
      }
    }

    public string Text
    {
      get
      {
        if (this.text == null)
          return "";
        return this.text;
      }
      set
      {
        this.text = value;
        this.textChanged = true;
      }
    }

    public TextBox()
      : base((string) null, 0, 0, 10, 10)
    {
    }

    public TextBox(string text, int x, int y, int width, int height)
      : this(text, x, y, width, height, 1f)
    {
    }

    public TextBox(string text, int x, int y, int width, int height, float textScale)
      : this(text, x, y, width, height, textScale, TextBox.DefaultTextAlignX, WinTextAlignY.Center)
    {
    }

    public TextBox(
      string text,
      int x,
      int y,
      int width,
      int height,
      float textScale,
      WinTextAlignX alignX,
      WinTextAlignY alignY)
      : base(text, x, y, width, height)
    {
      this.Text = text;
      this.TextScale = textScale;
      this.TextAlignX = alignX;
      this.TextAlignY = alignY;
      this.Font = CoreGlobals.GameFont;
    }

    public TextBox(Window win)
      : base(win)
    {
      this.TextScale = 1f;
      this.TextAlignX = TextBox.DefaultTextAlignX;
      this.TextAlignY = WinTextAlignY.Center;
    }

    public TextBox(TextBox win)
      : base((Window) win)
    {
      this.Text = win.Text;
      this.TextScale = win.TextScale;
      this.TextRotation = win.TextRotation;
      this.TextOrigin = win.TextOrigin;
      this.TextOffset = win.TextOffset;
      this.TextAlignX = win.TextAlignX;
      this.TextAlignY = win.TextAlignY;
      this.Font = win.Font;
      this.MinTextEdge = win.MinTextEdge;
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
      if (this.textChanged)
      {
        if (this.text != null && this.text.Length > 0)
        {
          int num = (int) ((double) bound.Width / (double) scale - ((double) this.TextOffset.X + 4.0));
          this.drawText = this.text;
          for (Vector2 vector2 = this.Font.MeasureString(this.drawText) * this.TextScale; (double) vector2.X >= (double) num; vector2 = this.Font.MeasureString(this.drawText) * this.TextScale)
            this.drawText = this.drawText.Substring(0, this.drawText.Length - 1);
        }
        else
          this.drawText = "";
      }
      if (this.drawText.Length <= 0)
        return;
      Vector2 vector2_1 = this.Font.MeasureString(this.drawText);
      vector2_1.X *= scale * this.TextScale;
      float x = this.TextAlignX == WinTextAlignX.Center ? (float) ((double) bound.X + (double) bound.Width * 0.5 + (double) this.TextOffset.X * (double) scale - (double) vector2_1.X * 0.5) : (this.TextAlignX == WinTextAlignX.Right ? (float) (bound.X + bound.Width) - (6f + this.TextOffset.X) * scale - vector2_1.X : (float) bound.X + (6f + this.TextOffset.X) * scale);
      if (this.MinTextEdge > (ushort) 0 && (double) x < (double) (bound.X + (int) this.MinTextEdge))
      {
        float num = (float) (bound.X + (int) this.MinTextEdge) - x;
        scale *= (float) (1.0 - (double) num / ((double) bound.Width * 0.5 - (double) this.MinTextEdge + (double) num));
        x = (float) (bound.X + (int) this.MinTextEdge);
      }
      vector2_1.Y *= scale * this.TextScale;
      float y = this.TextAlignY == WinTextAlignY.Center ? (float) ((double) bound.Y + (double) bound.Height * 0.5 + ((double) this.TextOffset.Y + 2.0) * (double) scale - (double) vector2_1.Y * 0.5) : (this.TextAlignY == WinTextAlignY.Bottom ? (float) (bound.Y + bound.Height) - (3f + this.TextOffset.Y) * scale - vector2_1.Y : (float) bound.Y + (3f + this.TextOffset.Y) * scale);
      TextBox.ColorProfile colors = (TextBox.ColorProfile) this.Colors;
      spriteBatch.DrawString(this.Font, this.drawText, new Vector2(x, y), colors.TextColor, this.TextRotation, this.TextOrigin, scale * this.TextScale, SpriteEffects.None, 0.0f);
    }

    static TextBox()
    {
      TextBox.ColorProfile colorProfile = new TextBox.ColorProfile();
      colorProfile.ForeColor = Color.White;
      colorProfile.BackColor = new Color(192, 192, 192);
      colorProfile.BackHoverColor = new Color(224, 224, 224);
      colorProfile.BackClickColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      colorProfile.BackDisabledColor = new Color(160, 160, 160);
      colorProfile.BorderColor = Color.Black;
      colorProfile.TextColor = Color.Black;
      TextBox.DefaultColorProfile = colorProfile;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      TextBox.DefaultTextColor = Color.Black;
    }

    public class ColorProfile : Window.ColorProfile
    {
      public Color TextColor;

      public override Window.ColorProfile Copy(Window.ColorProfile result)
      {
        TextBox.ColorProfile colorProfile = base.Copy(result) as TextBox.ColorProfile;
        if (colorProfile != null)
          colorProfile.TextColor = this.TextColor;
        return result;
      }
    }
  }
}
