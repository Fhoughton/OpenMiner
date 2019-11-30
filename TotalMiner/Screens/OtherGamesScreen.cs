// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.OtherGamesScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class OtherGamesScreen : GameScreen
  {
    private Texture2D boxartTexture;

    public OtherGamesScreen()
    {
      this.IsPopup = true;
      this.TransitionOnTime = TimeSpan.FromSeconds(2.0);
      this.TransitionOffTime = TimeSpan.Zero;
    }

    protected override void SetContentManager()
    {
      this.content = (IContentManager) new ContentManager((IServiceProvider) this.ScreenManager.Game.Services);
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.boxartTexture = this.content.Load<Texture2D>("Content\\Textures\\FootballGamesRoomBoxArt");
    }

    public override bool HandleInput(InputState input)
    {
      PlayerIndex playerIndex;
      if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex) || input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
        this.ExitScreen();
      return true;
    }

    protected override void DrawCore()
    {
      this.ScreenManager.FadeBackBufferToBlack((int) byte.MaxValue);
      this.ScreenManager.SpriteBatch.Begin();
      this.ScreenManager.SpriteBatch.DrawStringCentered(CoreGlobals.GameFont, "Other Games by Studio Forge", 80f, Color.DarkGreen * this.TransitionAlphaFloat, 1f);
      float num = 480f / (float) this.boxartTexture.Height;
      Rectangle destinationRectangle = MyExtensions.CenterOfViewport((int) ((double) this.boxartTexture.Width * (double) num), (int) ((double) this.boxartTexture.Height * (double) num));
      destinationRectangle.Y += 50;
      this.ScreenManager.SpriteBatch.Draw(this.boxartTexture, destinationRectangle, Color.White * this.TransitionAlphaFloat);
      this.ScreenManager.SpriteBatch.End();
    }
  }
}
