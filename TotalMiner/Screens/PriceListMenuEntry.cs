// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PriceListMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner.Screens
{
  internal class PriceListMenuEntry : BlockMenuEntry
  {
    public Item ItemID;
    private PriceList.Price price;
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private PriceListScreen screen;
    private string sellPrice;
    private string buyPrice;
    private float sellMeasureX;
    private float buyMeasureX;

    public PriceListMenuEntry(PriceListScreen screen, Item itemID, PriceList.Price price)
      : base((BlockMenuScreen) screen, ItemData.ToString(itemID))
    {
      this.screen = screen;
      this.price = price;
      this.ItemID = itemID;
      this.ColorHighlighted = Color.DarkGray;
    }

    protected override void LoadContentCore()
    {
      base.LoadContentCore();
      this.checkboxOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
      this.UpdatePrice(this.price);
    }

    public void UpdatePrice(PriceList.Price price)
    {
      this.price = price;
      this.sellPrice = price.Sell.ToString("N0");
      this.buyPrice = price.FinalBuy.ToString("N0");
      this.sellMeasureX = this.Screen.ItemFont.MeasureString(this.sellPrice).X * this.screen.ItemTextScale;
      this.buyMeasureX = this.Screen.ItemFont.MeasureString(this.buyPrice).X * this.screen.ItemTextScale;
    }

    private void SelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
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
      --position.Y;
      position.X += 32f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, position, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 356f;
      this.Screen.SpriteBatch.Draw(this.price.ForSale ? this.checkboxOn : this.checkboxOff, new Rectangle((int) position.X, (int) position.Y + 4, 16, 16), Color.White);
      position.X += 150f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.sellPrice, position - new Vector2(this.sellMeasureX, 0.0f), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 140f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.buyPrice, position - new Vector2(this.buyMeasureX, 0.0f), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }
  }
}
