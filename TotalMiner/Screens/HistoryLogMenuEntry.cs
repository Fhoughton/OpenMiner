// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HistoryLogMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class HistoryLogMenuEntry : BlockMenuEntry
  {
    public long Count;
    private HistoryLogScreen screen;

    public HistoryLogMenuEntry(HistoryLogScreen screen, string history, long count)
      : base((BlockMenuScreen) screen, history)
    {
      this.screen = screen;
      this.Count = count;
    }

    protected override void DrawText(string text, Vector2 pos, Color color, Vector2 measure)
    {
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text, pos, color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      string text1 = this.Count.ToString();
      pos.X += (float) (this.screen.HighlightRect.Width - 72) - this.Screen.ItemFont.MeasureString(text1).X * this.Screen.ItemTextScale;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text1, pos, color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }
  }
}
