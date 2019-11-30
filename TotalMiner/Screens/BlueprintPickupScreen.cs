// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BlueprintPickupScreen
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

namespace StudioForge.TotalMiner.Screens
{
  internal class BlueprintPickupScreen : MinerToolScreen
  {
    private int blueprintIndex;
    private Texture2D gridTexture;
    private int rowHeight;
    private string textItem;
    private string[] textDesc;
    private Texture2D backgroundTexture;
    private SpriteBatchSafe spriteBatchPoint;
    private Color colorWhite;
    private Color colorBlack;
    private Color colorBlue;

    public BlueprintPickupScreen(Player player, int blueprintIndex)
      : this(player, blueprintIndex, 0.0)
    {
    }

    public BlueprintPickupScreen(Player player, int blueprintIndex, double transitionOn)
      : base(player)
    {
      this.blueprintIndex = blueprintIndex;
      this.TransitionOnTime = TimeSpan.FromSeconds(transitionOn);
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 769, 385);
      base.LoadContent();
      this.gridTexture = this.content.Load<Texture2D>("Textures\\grid");
      this.backgroundTexture = this.content.Load<Texture2D>("Textures\\Blueprint");
      this.rowHeight = this.gridTexture.Height + 7;
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      if (this.blueprintIndex < 0 || this.blueprintIndex >= Blueprints.BlueprintList.Length)
        return;
      Blueprint blueprint = Blueprints.BlueprintList[this.blueprintIndex];
      this.textItem = ItemData2.ForDisplay(this.player.GameInstance, blueprint.Result);
      this.textDesc = Utils.BreakIntoLines(this.Font, 720, 1f, blueprint.Description, true);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
    }

    public override bool HandleInput(InputState input)
    {
      if (!InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen) && !InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
        return base.HandleInput(input);
      CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuMoveSound);
      this.ExitScreen();
      return true;
    }

    public override int FadeBackBufferAlpha
    {
      get
      {
        return base.FadeBackBufferAlpha / 2;
      }
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      float num = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num;
      this.colorBlack = Color.Black * num;
      this.colorBlue = Color.Blue * num;
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, this.Matrix);
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatch.Draw(this.backgroundTexture, this.screenRect, new Color(200, 200, 200, (int) this.TransitionAlpha));
      this.spriteBatch.DrawBox(this.screenRect.Expand(6), 6, new Color(23, 64, 91, (int) this.TransitionAlpha), 0.0f);
      Blueprint blueprint = Blueprints.BlueprintList[this.blueprintIndex];
      Vector2 pos = new Vector2((float) (this.screenRect.X + 30), (float) (this.screenRect.Y + 8));
      this.spriteBatch.DrawStringCentered(this.Font, "Blueprint: " + this.textItem, (float) (this.screenRect.Y + 20), this.colorWhite, 1.2f);
      pos.X = (float) (this.screenRect.X + 90);
      pos.Y = (float) (this.screenRect.Y + 76);
      this.DrawBlueprintGrid(blueprint, pos);
      this.spriteBatch.DrawString(this.Font, "=", pos + new Vector2(342f, 45f), Color.White);
      this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.GetTexureForItem(blueprint.Result.ItemID), new Rectangle((int) pos.X + 375, (int) pos.Y + 36, 64, 64), new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(blueprint.Result.ItemID)), Color.White);
      pos.Y += 150f;
      if (this.textDesc != null)
      {
        for (int index = 0; index < this.textDesc.Length; ++index)
        {
          this.spriteBatch.DrawStringCentered(this.Font, this.textDesc[index], pos.Y, this.colorWhite, 0.8f);
          pos.Y += 35f;
          if ((double) pos.Y > (double) (this.screenRect.Y + this.screenRect.Height - 60))
            break;
        }
      }
      Rectangle rect = new Rectangle(this.screenRect.X + this.screenRect.Width / 2 - 86, this.screenRect.Y + this.screenRect.Height - 32, 24, 24);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, rect);
      this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (rect.X + 32), (float) (rect.Y - 5)), Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      this.spriteBatchPoint.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private void DrawBlueprintGrid(Blueprint blueprint, Vector2 pos)
    {
      Rectangle rectangle1 = new Rectangle((int) pos.X + 200, (int) ((double) pos.Y + 3.0), this.gridTexture.Width, this.gridTexture.Height);
      int num1 = 40;
      InventoryItem[] items = blueprint.Items;
      Rectangle destinationRectangle = new Rectangle(0, 0, 24, 24);
      Rectangle rectangle2 = new Rectangle(0, 0, 16, 16);
      for (int index = 0; index < 9; ++index)
      {
        int num2 = index % 3;
        int num3 = index / 3;
        destinationRectangle.X = rectangle1.X + 3 + index % 3 * num1;
        destinationRectangle.Y = rectangle1.Y + 3 + (2 - index / 3) * num1;
        this.DrawSlot(destinationRectangle.X, destinationRectangle.Y);
        Item itemId = items[index].ItemID;
        if (itemId != Item.None)
        {
          Rectangle rectangle3 = GraphicStatics.TexturePack.ItemSrcRect(itemId);
          destinationRectangle.X += 8;
          destinationRectangle.Y += 8;
          this.spriteBatch.Draw(GraphicStatics.TexturePack.GetTexureForItem(itemId), destinationRectangle, new Rectangle?(rectangle3), this.colorWhite);
        }
      }
    }

    private void DrawSlot(int x, int y)
    {
      int num = 40;
      Rectangle rect = new Rectangle(x, y, num, num);
      Color color1 = new Color(0.8f, 0.8f, 0.8f, 1f);
      Color color2 = new Color(0.4f, 0.4f, 0.4f, 1f);
      this.spriteBatch.DrawFilledBox(rect, 3, color1, color2 * 0.7f);
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.X = rect.X + 3;
      destinationRectangle.Y = rect.Y + 3;
      destinationRectangle.Width = rect.Width - 6;
      destinationRectangle.Height = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
      destinationRectangle.Height = rect.Height - 6;
      destinationRectangle.Width = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
    }
  }
}
