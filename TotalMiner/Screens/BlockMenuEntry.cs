// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BlockMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GameState;

namespace StudioForge.TotalMiner.Screens
{
  internal class BlockMenuEntry : PanelMenuEntry
  {
    private bool dottedCheck;
    private string dottedText;
    private string lastText;

    public override Vector2 TextOffset
    {
      get
      {
        return base.TextOffset + TMFont.yVec + new Vector2(16f, 0.0f);
      }
    }

    public BlockMenuEntry(BlockMenuScreen screen, string text)
      : this(screen, text, (string) null)
    {
    }

    public BlockMenuEntry(BlockMenuScreen screen, string text, string entryTextureName)
      : base((PanelMenuScreen) screen, text, entryTextureName)
    {
      this.ColorHighlighted = Color.DarkGray;
    }

    protected override void DrawText(string text, Vector2 pos, Color color, Vector2 measure)
    {
      if (!this.dottedCheck || text != this.lastText)
      {
        this.BuildDottedText(text, pos, measure);
        this.lastText = text;
      }
      base.DrawText(this.dottedText != null ? this.dottedText : text, pos, color, measure);
    }

    private void BuildDottedText(string text, Vector2 pos, Vector2 measure)
    {
      this.dottedText = (string) null;
      PanelMenuScreen screen = this.Screen as PanelMenuScreen;
      if (screen != null)
      {
        float num = (float) (screen.MenuRect.X + screen.HighlightRect.Width - 8) - pos.X;
        for (string str = text; (double) measure.X > (double) num && str.Length > 1; measure = screen.ItemFont.MeasureString(text) * screen.ItemTextScale)
        {
          str = str.Substring(0, str.Length - 1);
          text = str + "...";
        }
        this.dottedText = text;
      }
      this.dottedCheck = true;
    }
  }
}
