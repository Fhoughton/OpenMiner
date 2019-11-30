// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.PanelMenuEntry
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;

namespace StudioForge.Engine.GameState
{
  public class PanelMenuEntry : MenuEntry
  {
    public string ButtonPanelTextA;
    private PanelMenuScreen screen;

    public PanelMenuEntry(PanelMenuScreen screen, string text)
      : this(screen, text, (string) null)
    {
    }

    public PanelMenuEntry(PanelMenuScreen screen, string text, string entryTextureName)
      : base((MenuScreen) screen, text, entryTextureName)
    {
      this.screen = screen;
      this.ColorSelected = Color.White;
    }

    public override Rectangle GetHighLightRect(Vector2 position)
    {
      Rectangle highlightRect = this.screen.HighlightRect;
      highlightRect.X += (int) ((double) position.X + (double) this.HighlightBarOffset.X);
      highlightRect.Y += (int) ((double) position.Y + (double) this.HighlightBarOffset.Y);
      return highlightRect;
    }

    protected override void DrawHighLight(Vector2 position, Color color)
    {
      this.lastHighLightRect = this.GetHighLightRect(position);
      this.screen.SpriteBatch.DrawFilledBox(this.lastHighLightRect, 1, color * ((float) this.Screen.TransitionAlpha / (float) byte.MaxValue), color * (float) ((double) this.Screen.TransitionAlpha / (double) byte.MaxValue * 0.5));
      this.DrawPanel();
    }

    public virtual void DrawPanel()
    {
      if (!this.screen.DrawPanel || this.EntryTexture == null || this.Text.IsEmpty())
        return;
      Rectangle halfPanelRect = this.screen.HalfPanelRect;
      halfPanelRect.Width = (int) ((double) this.EntryTexture.Width * ((double) halfPanelRect.Height / (double) this.EntryTexture.Height));
      this.screen.SpriteBatch.Draw(this.EntryTexture, halfPanelRect, this.screen.ColorWhite);
      this.DrawPanelButtonAText();
    }

    public void DrawPanelButtonAText()
    {
      if (this.ButtonPanelTextA.IsEmpty())
        return;
      Vector2 position = new Vector2((float) (this.screen.PanelRect.X + 5), this.screen.ButtonStartPosition.Y - 20f - (this.screen.ItemFont.MeasureString(this.ButtonPanelTextA) * this.screen.PanelTextScale).Y);
      this.screen.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, position, this.screen.ColorWhite);
      position.X += (float) (CoreGlobals.ButtonTextureA.Width + 10);
      position.Y += 5f;
      this.screen.SpriteBatch.DrawString(this.screen.ItemFont, this.ButtonPanelTextA, position, this.screen.ColorWhite, 0.0f, Vector2.Zero, this.screen.PanelTextScale, SpriteEffects.None, 1f);
    }
  }
}
