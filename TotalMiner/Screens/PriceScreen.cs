// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PriceScreen
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
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class PriceScreen : MinerToolScreen
  {
    private string itemTypeName = "";
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private PriceList.PriceListType origPriceType;
    private string priceTypeDesc;
    private int thumbstickTimer;
    private int currentSlot;
    private Item itemID;
    private PriceList priceList;
    private PriceList.Price price;
    private PriceList.Price oldPrice;
    private ShopBlock shopBlockForOptions;
    private Vector2 leftstick;
    private Vector2 rightstick;
    private Vector2 lastleftstick;
    private Vector2 lastrightstick;
    private GameInstance instance;
    private Action<PriceList.Price> onPriceUpdated;

    public PriceScreen(
      GameInstance instance,
      Player player,
      PriceList priceList,
      Item itemID,
      ShopBlock shopBlockForOptions,
      Action<PriceList.Price> onPriceUpdated)
      : base(player)
    {
      this.instance = instance;
      this.priceList = priceList;
      this.itemID = itemID;
      this.shopBlockForOptions = shopBlockForOptions;
      this.onPriceUpdated = onPriceUpdated;
      this.oldPrice = this.price = priceList.Prices[(int) itemID];
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = MyExtensions.CenterOfViewport(528, 240);
      base.LoadContent();
      this.checkboxOn = this.content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = this.content.Load<Texture2D>("Textures\\CheckboxOff");
      this.itemTypeName = ItemData.ToString(this.itemID);
      this.currentSlot = 0;
    }

    private void SetPriceListTypeDesc()
    {
      if (this.priceTypeDesc != null && this.priceList.Type == this.origPriceType)
        return;
      this.priceTypeDesc = this.priceList.Type != PriceList.PriceListType.PlayerDefault ? "(Shop)" : "(Global)";
      this.origPriceType = this.priceList.Type;
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.instance.NetworkManager.SendPriceChange(this.player, this.shopBlockForOptions, this.priceList.Type, this.itemID, this.oldPrice, this.price);
      if (this.onPriceUpdated == null)
        return;
      this.onPriceUpdated(this.price);
    }

    public virtual bool HandleInput(GamePadState pad, GamePadState lastpad)
    {
      return false;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (this.IsExitButtonPressed(currentGamePadState, lastGamePadState))
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        return true;
      }
      if (currentGamePadState.Buttons.A == ButtonState.Pressed && lastGamePadState.Buttons.A == ButtonState.Released)
      {
        this.AButtonPressed();
        return true;
      }
      if (currentGamePadState.Buttons.Y == ButtonState.Pressed && lastGamePadState.Buttons.Y == ButtonState.Released)
      {
        this.YButtonPressed();
        return true;
      }
      int num1 = 10;
      int num2 = 15;
      this.lastleftstick = this.leftstick;
      this.lastrightstick = this.rightstick;
      this.leftstick = currentGamePadState.ThumbSticks.Left;
      this.rightstick = currentGamePadState.ThumbSticks.Right;
      float num3 = 0.2f;
      if ((double) this.leftstick.X > -(double) num3 && (double) this.leftstick.X < (double) num3)
        this.leftstick.X = 0.0f;
      if ((double) this.leftstick.Y > -(double) num3 && (double) this.leftstick.Y < (double) num3)
        this.leftstick.Y = 0.0f;
      if ((double) this.rightstick.X > -(double) num3 && (double) this.rightstick.X < (double) num3)
        this.rightstick.X = 0.0f;
      if ((double) this.rightstick.Y > -(double) num3 && (double) this.rightstick.Y < (double) num3)
        this.rightstick.Y = 0.0f;
      if (this.leftstick != Vector2.Zero || this.rightstick != Vector2.Zero)
      {
        if (this.IsEitherStickMoved(this.leftstick, this.rightstick, this.lastleftstick, this.lastrightstick))
          this.thumbstickTimer = num2 + 1;
        else
          ++this.thumbstickTimer;
      }
      else
        this.thumbstickTimer = 0;
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorLeft) || (double) this.leftstick.X < 0.0 && this.thumbstickTimer > num1 || (double) this.rightstick.X < 0.0 && this.thumbstickTimer > num1)
      {
        this.MoveLeft();
        this.thumbstickTimer = 0;
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorRight) || (double) this.leftstick.X > 0.0 && this.thumbstickTimer > num1 || (double) this.rightstick.X > 0.0 && this.thumbstickTimer > num1)
      {
        this.MoveRight();
        this.thumbstickTimer = 0;
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown) || (double) this.leftstick.Y < 0.0 && this.thumbstickTimer > num2 || (double) this.rightstick.Y < 0.0 && this.thumbstickTimer > num2)
      {
        this.MoveDown();
        this.thumbstickTimer = 0;
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp) || (double) this.leftstick.Y > 0.0 && this.thumbstickTimer > num2 || (double) this.rightstick.Y > 0.0 && this.thumbstickTimer > num2)
      {
        this.MoveUp();
        this.thumbstickTimer = 0;
        return true;
      }
      if (base.HandleInput(input))
        return true;
      return this.HandleInput(currentGamePadState, lastGamePadState);
    }

    private void AButtonPressed()
    {
      switch (this.currentSlot)
      {
        case 0:
          this.price.ForSale = !this.price.ForSale;
          this.priceList.Prices[(int) this.itemID] = this.price;
          break;
        case 1:
          this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.SellPriceEntered), this.price.Sell, false), this.ControllingPlayer);
          break;
        case 2:
          if (!this.price.UsePerc)
          {
            this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.BuyPriceEntered), this.price.Buy, false), this.ControllingPlayer);
            break;
          }
          this.price.UsePerc = false;
          this.priceList.Prices[(int) this.itemID] = this.price;
          break;
        case 3:
          if (this.price.UsePerc)
          {
            this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.BuyPercEntered), this.price.Perc, false), this.ControllingPlayer);
            break;
          }
          this.price.UsePerc = true;
          this.priceList.Prices[(int) this.itemID] = this.price;
          break;
      }
    }

    private void YButtonPressed()
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Price Options", "Get price of " + ItemData.ToString(this.itemID) + " from your global price list", "Assign price of " + ItemData.ToString(this.itemID) + " to your global price list", "Review your global price list", "Other options", this.Font, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.A1ButtonOption);
      messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.X1ButtonOption);
      messageBoxScreenTm.ButtonY += new EventHandler<PlayerIndexEventArgs>(this.Y1ButtonOption);
      messageBoxScreenTm.ButtonB += new EventHandler<PlayerIndexEventArgs>(this.B1ButtonOption);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void A1ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.DefaultPriceList == null || this.priceList == null)
        return;
      this.price = this.player.DefaultPriceList.Prices[(int) this.itemID];
      this.priceList.Prices[(int) this.itemID] = this.price;
    }

    private void X1ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.DefaultPriceList == null)
        return;
      PriceList.Price price = this.player.DefaultPriceList.Prices[(int) this.itemID];
      this.player.DefaultPriceList.Prices[(int) this.itemID] = this.price;
      this.instance.NetworkManager.SendPriceChange(this.player, (ShopBlock) null, PriceList.PriceListType.PlayerDefault, this.itemID, price, this.price);
    }

    private void Y1ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.DefaultPriceList == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new PriceListScreen(this.player, this.player.DefaultPriceList, this.shopBlockForOptions), this.ControllingPlayer);
    }

    private void B1ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      string heading = "Price List Options";
      string aMessage = "This shop uses your global price list (Default)";
      string xMessage = "This shop uses it's own price list";
      if (this.shopBlockForOptions != null && this.shopBlockForOptions.PriceList != null)
        xMessage += " (Current)";
      else
        aMessage += " (Current)";
      string yMessage = "Copy this shops price list to your global price list";
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM(heading, aMessage, xMessage, yMessage, "Exit", this.Font, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.A2ButtonOption);
      messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.X2ButtonOption);
      messageBoxScreenTm.ButtonY += new EventHandler<PlayerIndexEventArgs>(this.Y2ButtonOption);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void A2ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("This shop will now use your global price list.\nAny changes to your global price list will also change this shop price.\nAny changes to this shop price will also change your global price.\n\nConfirm", "Yes", (string) null, (string) null, "No", this.Font, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ConfirmA2Button);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void ConfirmA2Button(object sender, PlayerIndexEventArgs e)
    {
      this.shopBlockForOptions.PriceList = (PriceList) null;
      this.priceList = this.player.DefaultPriceList;
      this.price = this.priceList.Prices[(int) this.itemID];
      this.instance.NetworkManager.SendPriceList(NetworkManager.PriceListChangeType.ShopUsesDefault, this.player, this.shopBlockForOptions);
    }

    private void X2ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("This shop will now use a copy of your global price list.\nAny changes to your global price list will not affect this shop and vice versa.\n\nConfirm", "Yes", (string) null, (string) null, "No", this.Font, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ConfirmX2Button);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void ConfirmX2Button(object sender, PlayerIndexEventArgs e)
    {
      this.shopBlockForOptions.PriceList = new PriceList(PriceList.PriceListType.PlayerShop, this.player.DefaultPriceList);
      this.priceList = this.shopBlockForOptions.PriceList;
      this.price = this.priceList.Prices[(int) this.itemID];
      this.instance.NetworkManager.SendPriceList(NetworkManager.PriceListChangeType.ShopCopyOfDefault, this.player, this.shopBlockForOptions);
    }

    private void Y2ButtonOption(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Your global price list will be a copy of this shops price list.\nAny changes to your global price list will not affect this shop and vice versa.\n\n\nConfirm", "Yes", (string) null, (string) null, "No", this.Font, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.ConfirmY2Button);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void ConfirmY2Button(object sender, PlayerIndexEventArgs e)
    {
      this.player.DefaultPriceList = new PriceList(PriceList.PriceListType.PlayerDefault, this.shopBlockForOptions.PriceList);
      this.instance.NetworkManager.SendPriceList(NetworkManager.PriceListChangeType.DefaultCopyOfShop, this.player, this.shopBlockForOptions);
    }

    private void SellPriceEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.price.Sell = MyMathHelper.Clamp((int) number, 0, 100000000);
      this.priceList.Prices[(int) this.itemID] = this.price;
    }

    private void BuyPriceEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.price.Buy = MyMathHelper.Clamp((int) number, 0, 100000000);
      this.priceList.Prices[(int) this.itemID] = this.price;
    }

    private void BuyPercEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.price.Perc = MyMathHelper.Clamp((int) number, 1, 100);
      this.priceList.Prices[(int) this.itemID] = this.price;
    }

    private void MoveLeft()
    {
    }

    private void MoveRight()
    {
    }

    private void MoveUp()
    {
      if (--this.currentSlot >= 0)
        return;
      this.currentSlot = 3;
    }

    private void MoveDown()
    {
      if (++this.currentSlot <= 3)
        return;
      this.currentSlot = 0;
    }

    protected virtual bool IsExitButtonPressed(GamePadState pad, GamePadState lastpad)
    {
      return pad.Buttons.Back == ButtonState.Pressed && lastpad.Buttons.Back == ButtonState.Released || pad.Buttons.B == ButtonState.Pressed && lastpad.Buttons.B == ButtonState.Released;
    }

    private bool IsEitherStickMoved(
      Vector2 leftstick,
      Vector2 rightstick,
      Vector2 lastleftstick,
      Vector2 lastrightstick)
    {
      if (Math.Sign(leftstick.X) == Math.Sign(lastleftstick.X) && Math.Sign(leftstick.Y) == Math.Sign(lastleftstick.Y) && Math.Sign(rightstick.X) == Math.Sign(lastrightstick.X))
        return Math.Sign(rightstick.Y) != Math.Sign(lastrightstick.Y);
      return true;
    }

    protected override void DrawCore()
    {
      int num = 0;
      int width = 492;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      if (this.itemTypeName != null && this.itemTypeName.Length > 0)
        this.spriteBatch.DrawString(this.Font, this.itemTypeName, new Vector2((float) (this.screenRect.X + 24), (float) (this.screenRect.Y + 13)), Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.SetPriceListTypeDesc();
      this.spriteBatch.DrawString(this.Font, this.priceTypeDesc, new Vector2((float) (this.screenRect.X + this.screenRect.Width - 100), (float) (this.screenRect.Y + 13)), Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      Rectangle rectangle = new Rectangle(this.screenRect.X + 24, this.screenRect.Y + 60, 24, 24);
      this.spriteBatch.Draw(this.price.ForSale ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      rectangle.Y += 68;
      this.spriteBatch.Draw(this.price.UsePerc ? this.checkboxOff : this.checkboxOn, rectangle, Color.White);
      rectangle.Y += 34;
      this.spriteBatch.Draw(this.price.UsePerc ? this.checkboxOn : this.checkboxOff, rectangle, Color.White);
      Vector2 position = new Vector2((float) (this.screenRect.X + 60), (float) (this.screenRect.Y + 64)) + TMFont.yVec;
      if (this.currentSlot == 0)
        num = (int) position.Y;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, "For Sale", position, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      position.Y += 34f;
      if (this.currentSlot == 1)
        num = (int) position.Y;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, string.Format("Sell Price:  {0:N0} gp", (object) this.GetSellPrice()), position, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      position.Y += 34f;
      if (this.currentSlot == 2)
        num = (int) position.Y;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, string.Format("Buy Price:  {0:N0} gp", (object) this.GetBuyPrice()), position, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      position.Y += 34f;
      if (this.currentSlot == 3)
        num = (int) position.Y;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, string.Format("          or {0}% of Sell Price ({1:N0} gp)", (object) this.GetBuySellPerc(), (object) this.GetCalcedBuyPrice()), position, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      rectangle = new Rectangle(this.screenRect.X + 16, num - 4, width, 34);
      this.spriteBatch.DrawBox(rectangle, 1, Color.Yellow, 0.0f);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, Color.Yellow * 0.05f);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 14;
      screenRect.Y += 42;
      screenRect.Width -= 28;
      screenRect.Height = 1;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White);
      screenRect.Y += this.screenRect.Height - 80;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, Color.White);
      position.X = (float) (screenRect.X + 6);
      position.Y = (float) (screenRect.Y + 12);
      float scale = 0.6f;
      position.Y -= 6f;
      this.spriteBatch.Draw(CoreGlobals.ButtonTextureA, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);
      position.X += 42f;
      position.Y += 6f;
      bool flag = this.currentSlot == 1 || this.currentSlot == 2 && !this.price.UsePerc || this.currentSlot == 3 && this.price.UsePerc;
      this.spriteBatch.DrawString(this.Font, flag ? "Edit" : "Select", position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      position.X += flag ? 76f : 100f;
      position.Y -= 6f;
      this.spriteBatch.Draw(CoreGlobals.ButtonTextureB, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);
      position.X += 42f;
      position.Y += 6f;
      this.spriteBatch.DrawString(this.ScreenManager.GameFont, "Close", position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      position.X += 96f;
      position.Y -= 6f;
      this.spriteBatch.Draw(CoreGlobals.ButtonTextureY, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0.0f);
      position.X += 42f;
      position.Y += 6f;
      this.spriteBatch.DrawString(this.ScreenManager.GameFont, "Options", position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      position.X += 106f;
      this.spriteBatch.End();
    }

    private int GetSellPrice()
    {
      return this.price.Sell;
    }

    private int GetSellPrice(Item itemID)
    {
      return this.priceList.Prices[(int) itemID].Sell;
    }

    private int GetBuyPrice()
    {
      return this.price.Buy;
    }

    private int GetBuyPrice(Item itemID)
    {
      return this.priceList.Prices[(int) itemID].Buy;
    }

    private int GetBuySellPerc()
    {
      return this.price.Perc;
    }

    private int GetCalcedBuyPrice()
    {
      return (int) ((double) this.price.Sell * ((double) this.price.Perc / 100.0));
    }
  }
}
