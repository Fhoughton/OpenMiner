// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.PanelMenuScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Renderers;
using System;

namespace StudioForge.Engine.GameState
{
  public abstract class PanelMenuScreen : MenuScreen
  {
    public float PanelTextScale = 0.7f;
    public bool DrawPanel = true;
    public bool DrawLastLine = true;
    public bool DrawLeftMarginLine = true;
    public bool DrawEntryLines = true;
    public bool CenterMenuRect = true;
    public Rectangle HighlightRect;
    public Rectangle MenuRect;
    public Rectangle PanelRect;
    public Rectangle HalfPanelRect;
    private string menuSubNoGbg;
    private Vector2 titleMeasure;

    public PanelMenuScreen(string title)
      : this(title, (string) null)
    {
    }

    public PanelMenuScreen(string title, string subTitle)
      : base(title, subTitle)
    {
      this.TitleStripColor = Color.Black;
      this.DrawItemTextures = true;
      this.DrawItemLines = true;
      this.DrawItemTextureBorder = true;
      this.HighlightRect = new Rectangle(0, 0, 100, 48);
      this.MenuRect = new Rectangle(590, 300, 100, 240);
      if (this.MenuSubTitle.IsEmpty())
        return;
      this.menuSubNoGbg = "(" + this.MenuSubTitle + ")";
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.ItemsPerPage = Math.Min(Math.Max(1, this.ItemsPerPage), this.MenuEntries.Count);
      this.HighlightRect.Height = this.ItemHeight + this.ItemGapY;
      this.ResetMenuRect();
      this.ToolTipX = this.MenuRect.X + this.MenuRect.Width - 8;
    }

    protected virtual int ButtonBarHeight
    {
      get
      {
        return 0;
      }
    }

    protected virtual int MenuRectWidthExt
    {
      get
      {
        return 0;
      }
    }

    protected virtual void ResetMenuRect()
    {
      this.ResetMenuRect(new Rectangle(this.MenuRect.X, this.MenuRect.Y, this.HighlightRect.Width + this.MenuRectWidthExt, this.HighlightRect.Height * this.ItemsPerPage + this.ButtonBarHeight));
    }

    protected virtual void ResetMenuRect(Rectangle rect)
    {
      if (this.CenterMenuRect)
        rect = MyExtensions.CenterOfViewport(CoreGlobals.GraphicsDevice.Viewport, rect.Width, rect.Height);
      this.MenuRect = rect;
      this.titleMeasure = this.Font.MeasureString(this.MenuTitle) * this.titleScale;
      this.titlePosition = new Vector2((float) (((double) this.MenuRect.Width - (double) this.titleMeasure.X - (double) (this.Font.MeasureString("  ()" + this.MenuSubTitle) * this.titleScale * 0.5f).X) / 2.0) + (float) this.MenuRect.X, (float) ((double) this.MenuRect.Y + (50.0 - (double) this.titleMeasure.Y) / 2.0 + 4.0));
      this.PanelRect = new Rectangle(this.MenuRect.X + 560, this.MenuRect.Y + 65, 350, 218);
      this.HalfPanelRect = new Rectangle(this.PanelRect.X, this.PanelRect.Y, 96, 96);
    }

    public PanelMenuEntry GetMenuEntry(int i)
    {
      return this.MenuEntries[i] as PanelMenuEntry;
    }

    protected override void DrawBackground()
    {
      this.SpriteBatch.DrawRoundedFilledBox(this.MenuRect.Expand(2), 2, this.ColorWhite, Color.Black * (float) ((double) this.TransitionAlpha / (double) byte.MaxValue * 0.75));
      if (!this.DrawPanel)
        return;
      Color color = Color.White * (float) ((double) this.TransitionAlpha / (double) byte.MaxValue * 0.400000005960464);
      float num = (float) ((double) this.StartPosition.X + (double) this.HighlightRect.Width + 20.0);
      Vector4 line1 = new Vector4(num, (float) (this.MenuRect.Y + 60), num, this.ButtonStartPosition.Y - 15f);
      if (this.MenuEntries.Count > 0)
        LineRenderer2D.DrawLine(this.SpriteBatch, this.ScreenManager.BlankTexture, color, line1);
      if (this.DrawLeftMarginLine)
      {
        line1.X = line1.Z = (float) (this.MenuRect.X + 20);
        LineRenderer2D.DrawLine(this.SpriteBatch, this.ScreenManager.BlankTexture, color, line1);
      }
      Vector4 line2 = new Vector4((float) (this.MenuRect.X + 20), this.ButtonStartPosition.Y - 8f, (float) (this.MenuRect.X + this.MenuRect.Width - 20), this.ButtonStartPosition.Y - 8f);
      LineRenderer2D.DrawLine(this.SpriteBatch, this.ScreenManager.BlankTexture, color, line2);
    }

    protected override void DrawTitle()
    {
      Color color = this.TitleColor * ((float) this.TransitionAlpha / (float) byte.MaxValue);
      this.SpriteBatch.DrawString(this.Font, this.MenuTitle, this.titlePosition, color, 0.0f, Vector2.Zero, this.titleScale, SpriteEffects.None, 0.0f);
      if (!this.MenuSubTitle.IsEmpty())
        this.SpriteBatch.DrawString(this.Font, this.menuSubNoGbg, this.titlePosition + new Vector2(this.titleMeasure.X + 20f, this.titleMeasure.Y * 0.25f), color, 0.0f, Vector2.Zero, this.titleScale * 0.5f, SpriteEffects.None, 0.0f);
      this.SpriteBatch.DrawString(this.Font, this.MenuTitle, this.titlePosition, color, 0.0f, Vector2.Zero, this.titleScale, SpriteEffects.None, 0.0f);
      this.SpriteBatch.Draw(this.ScreenManager.BlankTexture, new Rectangle(this.MenuRect.X, this.MenuRect.Y + 50, this.MenuRect.Width, 2), this.ColorWhite);
    }

    protected override void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      base.DrawEntry(menuEntry, entryID, position, isSelected);
      this.DrawEntryLinesCore(menuEntry, entryID, position, isSelected);
    }

    protected void DrawEntryLinesCore(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      if (!this.DrawEntryLines || !this.DrawLastLine && entryID == this.itemAtTopOfPage + this.ItemsPerPage - 1)
        return;
      LineRenderer2D.DrawLine(this.SpriteBatch, this.ScreenManager.BlankTexture, Color.White * (float) ((double) this.TransitionAlpha / (double) byte.MaxValue * 0.400000005960464), new Vector4(position.X + (float) this.HighlightRect.X, (float) ((double) position.Y + (double) this.ItemHeight + (double) this.ItemGapY - 1.0), position.X + (float) this.HighlightRect.X + (float) this.HighlightRect.Width, (float) ((double) position.Y + (double) this.ItemHeight + (double) this.ItemGapY - 1.0)));
    }

    protected override Vector2 SlidePositionForTransition(Vector2 position)
    {
      position.X += (float) this.MenuRect.X;
      position.Y += (float) this.MenuRect.Y;
      return position;
    }
  }
}
