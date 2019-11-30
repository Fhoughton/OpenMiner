// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ListBoxScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ListBoxScreen : BlockMenuScreen
  {
    public bool CloseOnSelect = true;
    protected int noneIndex = -1;
    protected ListBoxScreen.OnMenuItemSelected onSelected;
    protected bool includeNoneOption;
    protected float scale;
    protected int width;

    public ListBoxScreen(Player player)
      : base("List", player)
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
    }

    public ListBoxScreen(
      Player player,
      List<string> data,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool includeNoneOption)
      : this(player, data.ToArray(), onSelected, includeNoneOption)
    {
    }

    public ListBoxScreen(
      Player player,
      string[] data,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool includeNoneOption)
      : this(player, data, (string) null, onSelected, (string) null, (EventHandler<PlayerIndexEventArgs>) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, includeNoneOption, 0.0f, 0)
    {
    }

    public ListBoxScreen(
      Player player,
      string[] data,
      string selected,
      ListBoxScreen.OnMenuItemSelected onSelected,
      string XbuttonText,
      EventHandler<PlayerIndexEventArgs> onXButton,
      string YbuttonText,
      EventHandler<PlayerIndexEventArgs> onYButton,
      bool includeNoneOption,
      float scale,
      int width)
      : base("List", player)
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.InitializeCore(data, selected, onSelected, XbuttonText, onXButton, YbuttonText, onYButton, includeNoneOption, true, scale, width);
    }

    protected void InitializeCore(
      string[] data,
      string selected,
      ListBoxScreen.OnMenuItemSelected onSelected,
      string XbuttonText,
      EventHandler<PlayerIndexEventArgs> onXButton,
      string YbuttonText,
      EventHandler<PlayerIndexEventArgs> onYButton,
      bool includeNoneOption,
      bool includeBackItem,
      float scale,
      int width)
    {
      this.onSelected = onSelected;
      this.includeNoneOption = includeNoneOption;
      this.scale = (double) scale == 0.0 ? 0.6f : scale;
      this.width = width == 0 ? 720 : width;
      this.HighlightRect.Width = this.width;
      this.ItemHeight = (int) ((double) this.scale * 50.0);
      this.ItemGapY = (int) ((double) this.scale * 7.0);
      this.ItemTextScale = this.scale;
      this.ItemsPerPage = (int) ((1.0 - (double) this.scale) * 10.0 + 12.0);
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (includeNoneOption)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, "None");
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnItemSelectedHandler);
        blockMenuEntry.TextOffsetEx.X -= 17f;
        this.noneIndex = blockMenuEntryList.Count;
        blockMenuEntryList.Add(blockMenuEntry);
      }
      if (data != null)
      {
        foreach (string name in data)
        {
          BlockMenuEntry newMenuItem = this.GetNewMenuItem(name);
          newMenuItem.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnItemSelectedHandler);
          if (onXButton != null)
          {
            newMenuItem.SelectXButton += onXButton;
            newMenuItem.ButtonTextX = XbuttonText;
          }
          if (onYButton != null)
          {
            newMenuItem.SelectYButton += onYButton;
            newMenuItem.ButtonTextY = YbuttonText;
          }
          blockMenuEntryList.Add(newMenuItem);
          this.ItemInitialized((MenuEntry) newMenuItem, blockMenuEntryList.Count - 1);
          if (name == selected)
            this.selectedEntry = blockMenuEntryList.Count - 1;
        }
      }
      if (includeBackItem)
      {
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      }
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.SetSelectedEntry(this.selectedEntry);
    }

    protected virtual BlockMenuEntry GetNewMenuItem(string name)
    {
      return new BlockMenuEntry((BlockMenuScreen) this, name);
    }

    protected virtual void ItemInitialized(MenuEntry item, int entryID)
    {
    }

    public override void LoadContent()
    {
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    private void OnItemSelectedHandler(object sender, EventArgs e)
    {
      if (this.onSelected == null || (!this.onSelected(this.selectedEntry != this.noneIndex ? this.MenuEntries[this.selectedEntry] : (MenuEntry) null) || !this.CloseOnSelect))
        return;
      this.ExitScreen();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawBottomBar()
    {
      Rectangle rectangle = new Rectangle(this.MenuRect.X + 32, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 20, 20);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, rectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      rectangle.Y += 9;
      string text = "Page Up/Dn";
      float num1 = this.Font.MeasureString(text).X * this.ButtonScale;
      rectangle.X = this.MenuRect.X + this.MenuRect.Width - 20 - (int) num1 - 8 - rectangle.Width;
      GraphicStatics.DrawInputIcon(this.SpriteBatch, GuiInput.PageUp, rectangle, this.ColorWhite);
      float x1 = (float) (rectangle.X + rectangle.Width + 8);
      float y1 = (float) rectangle.Y;
      this.SpriteBatch.DrawString(this.Font, text, new Vector2(x1, y1), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      if (this.MenuEntries.Count <= 0)
        return;
      string buttonTextX = this.MenuEntries[this.selectedEntry].ButtonTextX;
      if (!buttonTextX.IsEmpty() && this.IsButtonXValid)
      {
        float num2 = this.Font.MeasureString(buttonTextX).X * this.ButtonScale;
        rectangle.X -= 20 + (int) num2 + 8 + rectangle.Width;
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureX, rectangle, this.ColorWhite);
        float x2 = (float) (rectangle.X + rectangle.Width + 8);
        float y2 = (float) rectangle.Y;
        this.SpriteBatch.DrawString(this.Font, buttonTextX, new Vector2(x2, y2), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      }
      string buttonTextY = this.MenuEntries[this.selectedEntry].ButtonTextY;
      if (!buttonTextY.IsEmpty() && this.IsButtonYValid)
      {
        float num2 = this.Font.MeasureString(buttonTextY).X * this.ButtonScale;
        rectangle.X -= 20 + (int) num2 + 8 + rectangle.Width;
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, rectangle, this.ColorWhite);
        float x2 = (float) (rectangle.X + rectangle.Width + 8);
        float y2 = (float) rectangle.Y;
        this.SpriteBatch.DrawString(this.Font, buttonTextY, new Vector2(x2, y2), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
      }
      string buttonTextA = this.MenuEntries[this.selectedEntry].ButtonTextA;
      if (buttonTextA.IsEmpty())
        return;
      float num3 = this.Font.MeasureString(buttonTextA).X * this.ButtonScale;
      rectangle.X -= 20 + (int) num3 + 8 + rectangle.Width;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, rectangle, this.ColorWhite);
      float x3 = (float) (rectangle.X + rectangle.Width + 8);
      float y3 = (float) rectangle.Y;
      this.SpriteBatch.DrawString(this.Font, buttonTextA, new Vector2(x3, y3), this.ColorWhite, 0.0f, Vector2.Zero, this.ButtonScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawButtons(int x)
    {
    }

    public delegate bool OnMenuItemSelected(MenuEntry item);
  }
}
