// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SliderMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Screens
{
  internal class SliderMenuEntry : BlockMenuEntry
  {
    private Player player;
    private SliderValue value;
    private int xOffset;
    private int yOffset;

    public SliderMenuEntry(
      BlockMenuScreen screen,
      Player player,
      string text,
      SliderValue value,
      int xOffset)
      : this(screen, player, text, value, xOffset, 13)
    {
    }

    public SliderMenuEntry(
      BlockMenuScreen screen,
      Player player,
      string text,
      SliderValue value,
      int xOffset,
      int yOffset)
      : base(screen, text, (string) null)
    {
      this.player = player;
      this.value = value;
      this.xOffset = xOffset;
      this.yOffset = yOffset;
    }

    protected override void DrawExtra(Vector2 position, Color color, float scale)
    {
      Rectangle rect = new Rectangle((int) position.X + this.xOffset, (int) position.Y + this.yOffset, 120, 8);
      this.Screen.SpriteBatch.DrawFilledBox(rect, 2, Color.Black * this.Screen.TransitionAlphaFloat, Color.LightGray * this.Screen.TransitionAlphaFloat);
      rect.X += (int) ((double) (rect.Width - 8) * ((double) this.value.Value - (1.0 - (double) this.value.Range)) * (1.0 / (double) this.value.Range));
      rect.Width = 8;
      rect.Height = 24;
      rect.Y -= 8;
      this.Screen.SpriteBatch.DrawFilledBox(rect, 2, Color.LightGray * this.Screen.TransitionAlphaFloat, Color.Black * this.Screen.TransitionAlphaFloat);
    }
  }
}
