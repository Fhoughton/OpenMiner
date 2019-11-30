// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BlockMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.Engine.Renderers;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class BlockMenuScreen : PanelMenuScreen
  {
    protected SpriteBatchSafe spriteBatch2;
    protected Player player;

    public Player Player
    {
      get
      {
        return this.player;
      }
    }

    protected GamerID PlayerID
    {
      get
      {
        if (this.player == null)
          return GamerID.Sys1;
        return this.player.GamerID;
      }
    }

    public BlockMenuScreen(string title, Player player)
      : base(title)
    {
      this.player = player;
      this.ItemHeight = 30;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.6f;
      this.DrawEntryLines = this.DrawItemLines = false;
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
    }

    public override void LoadContent()
    {
      this.spriteBatch2 = GraphicStatics.SpriteBatchPool.GetNextItem();
      base.LoadContent();
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch2);
    }

    protected override void ResetMenuRect(Rectangle rect)
    {
      base.ResetMenuRect(rect);
      this.PanelRect = new Rectangle(this.MenuRect.X + this.HighlightRect.Width, this.MenuRect.Y, this.MenuRect.Width - this.HighlightRect.Width, this.MenuRect.Height);
      this.UpdateMatrix();
    }

    private void GamerJoinedEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    protected void UpdateMatrix()
    {
      if (this.player == null)
        return;
      Rectangle menuRect = this.MenuRect;
      menuRect.Inflate(48, 48);
      this.Matrix = this.player.GetScreenMatrix(menuRect);
    }

    public override Vector2 ButtonStartPosition
    {
      get
      {
        return new Vector2((float) (this.MenuRect.X + 92 - this.ItemHeight), (float) (this.MenuRect.Y + this.MenuRect.Height + 5));
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp))
      {
        this.ScrollUpPage();
        return true;
      }
      if (!InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown))
        return base.HandleInput(input);
      this.ScrollDownPage();
      return true;
    }

    protected override void DrawBackground()
    {
      this.SpriteBatch.End();
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.MenuRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.SpriteBatch.End();
      this.SpriteBatch.BeginTM(this.Matrix);
      if (!this.DrawPanel || this.MenuEntries.Count <= 0)
        return;
      Color color = Color.White * (float) ((double) this.TransitionAlpha / (double) byte.MaxValue * 0.400000005960464);
      float num = (float) (this.PanelRect.X - 1);
      Vector4 line = new Vector4(num, (float) this.MenuRect.Y, num, (float) (this.MenuRect.Y + this.MenuRect.Height));
      LineRenderer2D.DrawLine(this.SpriteBatch, this.ScreenManager.BlankTexture, color, line);
    }
  }
}
