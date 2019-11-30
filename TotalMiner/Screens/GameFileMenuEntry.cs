// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameFileMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class GameFileMenuEntry : BlockMenuEntry
  {
    public bool FileSizeCalculated;
    public readonly SaveGameFileInfo GameInfo;

    public GameFileMenuEntry(BlockMenuScreen screen, SaveGameFileInfo gameInfo)
      : base(screen, gameInfo.Header.MapName)
    {
      this.GameInfo = gameInfo;
      this.ColorHighlighted = Color.DarkGray;
    }

    protected override void LoadContentCore()
    {
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      Vector2 vector2 = this.Screen.ItemFont.MeasureString(this.Text) * this.Screen.ItemTextScale;
      Vector2 pos = position + this.TextOffset;
      pos.Y = (float) (((double) this.Height - (double) vector2.Y) / 2.0 + (double) position.Y - 1.0);
      this.DrawItem(pos, color);
      this.DrawTexture(position, color);
    }

    private void DrawItem(Vector2 pos, Color color)
    {
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, pos, Color.DarkKhaki, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      if (this.GameInfo.Filename == null)
        return;
      Color color1 = Color.White;
      string text = Globals2.StripBadChars(this.GameInfo.Header.OwnerGamerTag.PadRight(20).Substring(0, 20));
      if (this.GameInfo.Header.IsAutoSave)
      {
        color1 = Color.Orange;
        text = "Auto Save";
      }
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text, pos + new Vector2(240f, 0.0f), color1, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      SpriteFont itemFont = this.Screen.ItemFont;
      Vector2 vector2 = new Vector2(536f, -2f);
      float scale1 = this.Screen.ItemTextScale * 0.7f;
      if (this.GameInfo.Header.GameMode == GameMode.DigDeep)
        this.Screen.SpriteBatch.DrawString(itemFont, "Dig Deep", pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, 0.0f);
      else if (this.GameInfo.Header.GameMode == GameMode.Survival)
        this.Screen.SpriteBatch.DrawString(itemFont, "Survival", pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, 0.0f);
      else if (this.GameInfo.Header.GameMode == GameMode.Peaceful)
        this.Screen.SpriteBatch.DrawString(itemFont, "Peaceful", pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, 0.0f);
      else
        this.Screen.SpriteBatch.DrawString(itemFont, "Creative", pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, 0.0f);
      vector2.Y += 16f;
      if (this.FileSizeCalculated)
        this.Screen.SpriteBatch.DrawString(itemFont, Globals2.GetFilesizeAsString(Math.Max(1, this.GameInfo.FileSize)), pos + vector2, Color.Yellow, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, 0.0f);
      vector2.Y -= 15f;
      vector2.X += 90f;
      float scale2 = this.Screen.ItemTextScale * 0.6f;
      DateTime dateTime = Utils.DateFromBinary(this.GameInfo.Header.DateSaved);
      this.Screen.SpriteBatch.DrawString(itemFont, dateTime.ToShortDateString(), pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale2, SpriteEffects.None, 0.0f);
      vector2.Y += 15f;
      this.Screen.SpriteBatch.DrawString(itemFont, dateTime.ToString("HH:mm:ss") + string.Format("  #{0}", (object) this.GameInfo.DirNumber), pos + vector2, Color.Wheat, 0.0f, Vector2.Zero, scale2, SpriteEffects.None, 0.0f);
    }
  }
}
