// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.ToolTip
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;

namespace StudioForge.Engine.GUI
{
  public class ToolTip
  {
    public static Color BackColor = Color.LightYellow;
    public static Color BorderColor = Color.Black;
    public static Color TextColor = Color.Black;
    public static float DefaultTooltipDelay = 1.5f;
    private int maxWidth = 320;
    private float textScale = 0.5f;
    public float Delay;
    public bool IsEnabled;
    private string text;
    private string textFormatted;
    private Vector2 textMeasure;
    private SpriteFont font;

    public bool IsValid
    {
      get
      {
        if (this.IsEnabled && this.textFormatted != null)
          return this.textFormatted.Length > 0;
        return false;
      }
    }

    public Rectangle GetRect()
    {
      return new Rectangle(0, 0, (int) this.textMeasure.X + 16, (int) this.textMeasure.Y + 12);
    }

    public string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        if (!(this.text != value))
          return;
        this.text = value;
        this.UpdateState();
      }
    }

    public int MaxWidth
    {
      get
      {
        return this.maxWidth;
      }
      set
      {
        this.maxWidth = value;
        this.UpdateState();
      }
    }

    public float TextScale
    {
      get
      {
        return this.textScale;
      }
      set
      {
        this.textScale = value;
        this.UpdateState();
      }
    }

    public ToolTip(SpriteFont font)
    {
      this.font = font;
      this.IsEnabled = true;
      this.Delay = ToolTip.DefaultTooltipDelay;
    }

    private void UpdateState()
    {
      if (this.text != null && this.text.Length > 0 && (this.maxWidth > 0 && (double) this.textScale > 0.0))
      {
        this.textFormatted = Utils.InsertNewLines(this.font, this.maxWidth, this.textScale, this.text, true);
        this.textMeasure = this.font.MeasureString(this.textFormatted) * this.textScale;
      }
      else
      {
        this.textFormatted = (string) null;
        this.textMeasure = Vector2.Zero;
      }
    }

    public void Draw(SpriteBatchSafe spriteBatch, Rectangle rect)
    {
      spriteBatch.DrawFilledBox(rect, 2, ToolTip.BorderColor, ToolTip.BackColor);
      spriteBatch.DrawString(this.font, this.textFormatted, new Vector2((float) (rect.X + 8), (float) (rect.Y + 6)), ToolTip.TextColor, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
    }
  }
}
