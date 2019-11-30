// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ViewClanBannerScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class ViewClanBannerScreen : GameScreen
  {
    private bool viewNumbers;

    public override void LoadContent()
    {
      base.LoadContent();
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.viewNumbers = true;
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (!InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
        return base.HandleInput(input);
      this.viewNumbers = !this.viewNumbers;
      return true;
    }

    protected override void DrawCore()
    {
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      Texture2D clanBanners = GraphicStatics.ClanBanners;
      int num1 = 4;
      float scale = 0.5f;
      Rectangle boxRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, clanBanners.Width * num1, clanBanners.Height * num1 + 30);
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      spriteBatch.End();
      spriteBatch.BeginTM(SamplerState.PointClamp, this.Matrix);
      Rectangle destinationRectangle = new Rectangle(boxRect.X, boxRect.Y, boxRect.Width, boxRect.Height - 30);
      Color color = Color.White * this.TransitionAlphaFloat;
      spriteBatch.Draw(clanBanners, destinationRectangle, color);
      if (this.viewNumbers)
      {
        Vector2 position = new Vector2((float) (destinationRectangle.X + 1), (float) (destinationRectangle.Y - 3));
        for (int index1 = 0; index1 < clanBanners.Height / 16; ++index1)
        {
          for (int index2 = 0; index2 < clanBanners.Width / 16; ++index2)
          {
            int num2 = index2 + 1 + index1 * (clanBanners.Width / 16);
            if (num2 <= 74)
            {
              Vector2 vector2 = CoreGlobals.GameFont.MeasureString(num2.ToString()) * scale;
              spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle((int) position.X - 1, (int) position.Y + 3, (int) vector2.X + 2, (int) vector2.Y - 8), Color.Black * 0.8f);
              spriteBatch.DrawString(CoreGlobals.GameFont, num2.ToString(), position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }
            position.X += (float) (16 * num1);
          }
          position.Y += (float) (16 * num1);
          position.X = (float) (destinationRectangle.X + 1);
        }
      }
      Rectangle rectangle = new Rectangle(destinationRectangle.X + destinationRectangle.Width - 16 * num1, destinationRectangle.Y + destinationRectangle.Height - 16 * num1, 16 * num1, 16 * num1);
      spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, color);
      rectangle = new Rectangle(boxRect.X, boxRect.Y + boxRect.Height - 29, boxRect.Width, 1);
      rectangle.X += 8;
      rectangle.Y += 4;
      rectangle.Width = rectangle.Height = 20;
      GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.SelectItem, rectangle, Color.White);
      spriteBatch.DrawString(CoreGlobals.GameFont, "Toggle ID's", new Vector2((float) (rectangle.X + 30), (float) (rectangle.Y - 1)), color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
