// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WisdomPickupScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class WisdomPickupScreen : MinerToolScreen
  {
    private int wisdomIndex;
    private Texture2D backgroundTexture;
    private string[] text;
    private Color colorWhite;
    private Color colorBlack;
    private Color colorBrown;

    public WisdomPickupScreen(Player player, int wisdomIndex)
      : this(player, wisdomIndex, 0.0)
    {
    }

    public WisdomPickupScreen(Player player, int wisdomIndex, double transitionOn)
      : base(player)
    {
      this.wisdomIndex = wisdomIndex;
      this.TransitionOnTime = TimeSpan.FromSeconds(transitionOn);
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 957, 630);
      base.LoadContent();
      this.backgroundTexture = this.content.Load<Texture2D>("Textures\\Wisdom");
      if (this.wisdomIndex < 0 || this.wisdomIndex >= Wisdom.WisdomList.Length)
        return;
      this.text = Utils.BreakIntoLines(this.Font, 800, 1f, Wisdom.WisdomList[this.wisdomIndex].Text, true);
    }

    public override bool HandleInput(InputState input)
    {
      PlayerIndex playerIndex;
      if (!input.IsMenuSelect(this.ControllingPlayer, out playerIndex) && !input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
        return base.HandleInput(input);
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
      this.colorBrown = new Color(33, 22, 12, (int) byte.MaxValue) * num;
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatch.Draw(this.backgroundTexture, this.screenRect, this.colorWhite);
      Viewport viewport = this.Player.Viewport;
      if (this.text != null)
      {
        float y = (float) (MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 100, this.text.Length * 35).Y - 20);
        for (int index = 0; index < this.text.Length; ++index)
        {
          this.spriteBatch.DrawStringCentered(this.Font, this.text[index], y, this.colorBrown, 1f);
          y += 35f;
          if ((double) y > (double) (this.screenRect.Y + this.screenRect.Height - 60))
            break;
        }
      }
      Rectangle rect = new Rectangle(this.screenRect.X + this.screenRect.Width / 2 - 80, this.screenRect.Y + this.screenRect.Height - 72, 24, 24);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, rect);
      this.spriteBatch.DrawString(this.Font, "Close", new Vector2((float) (rect.X + 32), (float) (rect.Y - 5)), Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
