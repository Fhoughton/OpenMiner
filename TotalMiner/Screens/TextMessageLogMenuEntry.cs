// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TextMessageLogMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;

namespace StudioForge.TotalMiner.Screens
{
  internal class TextMessageLogMenuEntry : BlockMenuEntry
  {
    private string textWithNL;

    public TextMessageLogMenuEntry(BlockMenuScreen screen, string text)
      : base(screen, text)
    {
    }

    public override void DrawPanel()
    {
      PanelMenuScreen screen = this.Screen as PanelMenuScreen;
      if (this.textWithNL == null)
        this.textWithNL = Utils.InsertNewLines(this.Screen.ItemFont, screen.PanelRect.Width - 20, 0.5f, this.Text, true);
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.textWithNL, new Vector2((float) (screen.PanelRect.X + 10), (float) (screen.PanelRect.Y + 10)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
    }
  }
}
