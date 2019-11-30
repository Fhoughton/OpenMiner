// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LootDropMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class LootDropMenuEntry : BlockMenuEntry
  {
    private LootTableScreen screen;
    private LootTable lootTable;
    private int index;
    private LootDrop drop;

    public LootDropMenuEntry(LootTableScreen screen, LootTable lootTable, int index)
      : base((BlockMenuScreen) screen, (string) null)
    {
      this.screen = screen;
      this.lootTable = lootTable;
      this.index = index;
      this.drop = lootTable.Table[index];
      this.SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (--screen.Column != -1)
          return;
        screen.Column = screen.MaxColumns;
      });
      this.SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (++screen.Column != screen.MaxColumns + 1)
          return;
        screen.Column = 0;
      });
      this.Selected += new EventHandler<PlayerIndexEventArgs>(this.SelectedEventHandler);
    }

    private void SelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      switch (this.screen.Column)
      {
        case 1:
          this.screen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.screen.Player, new NumberEntered(this.OnCountEntered), this.drop.Count, false), this.screen.ControllingPlayer);
          break;
        case 2:
          this.screen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.screen.Player, new NumberEntered(this.OnPercentEntered), this.drop.Percent, true, false), this.screen.ControllingPlayer);
          break;
      }
    }

    private void OnCountEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      LootDrop lootDrop = this.lootTable.Table[this.index];
      lootDrop.Count = (int) Math.Min(number, (double) ItemData.GetStackSize(lootDrop.ItemID));
      this.lootTable.Table[this.index] = lootDrop;
    }

    private void OnPercentEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      LootDrop lootDrop = this.lootTable.Table[this.index];
      lootDrop.Percent = MathHelper.Clamp((float) number, 0.0f, 100f);
      this.lootTable.Table[this.index] = lootDrop;
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      this.DrawItem(position, color);
      this.DrawTexture(position, color);
    }

    private void DrawItem(Vector2 position, Color color)
    {
      this.drop = this.lootTable.Table[this.index];
      position.X += 32f;
      position.Y += 4f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, ItemData.ToString(this.drop.ItemID), position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.Y -= 2f;
      float num = position.X + 500f;
      position.X += 500f;
      string text1 = Globals2.IntToString(this.drop.Count);
      position.X = num - this.Screen.ItemFont.MeasureString(text1).X * this.screen.ItemTextScale;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text1, position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      string text2 = Globals2.NumToString(this.drop.Percent);
      position.X = num + 158f;
      position.X -= this.Screen.ItemFont.MeasureString(text2).X * this.screen.ItemTextScale;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, text2, position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawHighLight(Vector2 position, Color color)
    {
      Rectangle highlightRect = ((PanelMenuScreen) this.Screen).HighlightRect;
      highlightRect.X += (int) position.X;
      highlightRect.Y += (int) position.Y - 4;
      Color color1 = this.ColorHighlighted * ((float) this.Screen.TransitionAlpha / (float) byte.MaxValue);
      Color fillColor = this.ColorHighlighted * (float) ((double) this.Screen.TransitionAlpha / (double) byte.MaxValue * 0.5);
      int num1 = 416;
      int num2 = 150;
      int num3 = 148;
      switch (this.screen.Column)
      {
        case 1:
          highlightRect.X += num1;
          highlightRect.Width = num2;
          break;
        case 2:
          highlightRect.X += num1 + num2;
          highlightRect.Width = num3;
          break;
        default:
          highlightRect.Width = num1;
          break;
      }
      this.Screen.SpriteBatch.DrawFilledBox(highlightRect, 1, color1, fillColor);
    }
  }
}
