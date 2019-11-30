// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PriceListScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class PriceListScreen : BlockMenuScreen
  {
    private PriceList priceList;
    private Texture2D checkOn;
    private Texture2D checkOff;
    private ShopBlock shopBlockForOptions;

    public PriceListScreen(Player player, PriceList priceList, ShopBlock shopBlockForOptions)
      : base("Price List", player)
    {
      this.priceList = priceList;
      this.shopBlockForOptions = shopBlockForOptions;
      List<PriceListMenuEntry> priceListMenuEntryList = new List<PriceListMenuEntry>();
      for (int index = 0; index < priceList.Prices.Length; ++index)
      {
        if (Globals1.ItemData[index].MinCSPrice > -1)
        {
          PriceList.Price price = priceList.Prices[index];
          PriceListMenuEntry priceListMenuEntry = new PriceListMenuEntry(this, (Item) index, price);
          priceListMenuEntryList.Add(priceListMenuEntry);
        }
      }
      priceListMenuEntryList.Sort(new Comparison<PriceListMenuEntry>(this.SortItems));
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "Item                               For Sale        Sell           Buy"));
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) priceListMenuEntryList.ToArray());
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      priceListMenuEntryList[priceListMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
    }

    private int SortItems(PriceListMenuEntry m1, PriceListMenuEntry m2)
    {
      return Globals1.ItemData[(int) m1.ItemID].Name.CompareTo(Globals1.ItemData[(int) m2.ItemID].Name);
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 728;
      this.ItemHeight = 20;
      this.ItemGapY = 4;
      this.ItemTextScale = 0.6f;
      this.ItemsPerPage = 20;
      this.DrawItemLines = this.DrawEntryLines = true;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.checkOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (input.IsNewButtonPress(Buttons.Y, this.ControllingPlayer.Value))
      {
        this.itemAtTopOfPage = 0;
        this.selectedEntry = 0;
        return true;
      }
      if (input.IsNewButtonPress(Buttons.A, this.ControllingPlayer.Value))
      {
        if (this.selectedEntry > 0 && this.selectedEntry < this.MenuEntries.Count - 1)
        {
          this.ScreenManager.AddScreen((GameScreen) new PriceScreen(this.player.GameInstance, this.player, this.priceList, ((PriceListMenuEntry) this.MenuEntries[this.selectedEntry]).ItemID, this.shopBlockForOptions, new Action<PriceList.Price>(this.OnPriceUpdated)), this.ControllingPlayer);
          return true;
        }
      }
      else if (input.IsNewButtonPress(Buttons.LeftTrigger, this.ControllingPlayer.Value))
      {
        int num = this.selectedEntry - this.itemAtTopOfPage;
        this.itemAtTopOfPage -= this.ItemsPerPage - 1;
        if (this.itemAtTopOfPage < 0)
          this.itemAtTopOfPage = 0;
        this.selectedEntry = this.itemAtTopOfPage + num;
      }
      else if (input.IsNewButtonPress(Buttons.RightTrigger, this.ControllingPlayer.Value))
      {
        int num = this.selectedEntry - this.itemAtTopOfPage;
        this.itemAtTopOfPage += this.ItemsPerPage - 1;
        if (this.itemAtTopOfPage + this.ItemsPerPage > this.MenuEntries.Count)
          this.itemAtTopOfPage = this.MenuEntries.Count - this.ItemsPerPage;
        this.selectedEntry = this.itemAtTopOfPage + num;
      }
      return base.HandleInput(input);
    }

    private void OnPriceUpdated(PriceList.Price price)
    {
      ((PriceListMenuEntry) this.MenuEntries[this.selectedEntry]).UpdatePrice(price);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + 32, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Top", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      destinationRectangle.X += 100;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Edit Price", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      destinationRectangle.X += 160;
      GraphicStatics.DrawInputIcon(this.SpriteBatch, GuiInput.PageUp, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 2, 12, 24));
      this.SpriteBatch.DrawString(this.Font, "Page Up/Down", new Vector2((float) (destinationRectangle.X + 24), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }
  }
}
