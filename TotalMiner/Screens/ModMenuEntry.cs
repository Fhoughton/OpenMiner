// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ModMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class ModMenuEntry : BlockMenuEntry
  {
    public bool IsActive;

    public ModMenuEntry(BlockMenuScreen screen, string name, bool isActive)
      : base(screen, name)
    {
      this.IsActive = isActive;
    }

    protected override void DrawText(string text, Vector2 pos, Color color, Vector2 measure)
    {
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, pos, color, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      pos.X += measure.X;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.IsActive ? "Active" : "Inactive", pos, this.IsActive ? Color.DarkGreen : Color.DarkRed, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }
  }
}
