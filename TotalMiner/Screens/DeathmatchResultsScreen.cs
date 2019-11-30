// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.DeathmatchResultsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class DeathmatchResultsScreen : GameScreen
  {
    private DeathmatchMiniGame game;

    public DeathmatchResultsScreen(DeathmatchMiniGame game)
    {
      this.game = game;
      this.TransitionOnTime = TimeSpan.FromSeconds(1.0);
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.Font = CoreGlobals.GameFont;
    }

    public override bool HandleInput(InputState input)
    {
      if (NetworkManager.Instance.IsSessionOpen)
      {
        foreach (NetworkGamer localGamer in NetworkManager.Instance.LocalGamers)
        {
          if (input.IsNewButtonPress(Buttons.A, localGamer.PlayerIndex) || input.IsNewButtonPress(Buttons.B, localGamer.PlayerIndex))
            this.ExitScreen();
        }
      }
      return base.HandleInput(input);
    }

    protected override void DrawCore()
    {
      if (!NetworkManager.Instance.IsSessionOpen)
        return;
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      Rectangle boxRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 864, 540);
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      spriteBatch.End();
      spriteBatch.Begin();
      spriteBatch.DrawStringCentered(this.Font, "The Deathmatch has ended", 160f, Color.White, 1.1f);
      DeathmatchMiniGame.PlayerData[] winners = this.game.GetWinners();
      string text1 = winners.Length == 1 ? "The Winner Is:" : "The Winners Are:";
      spriteBatch.DrawStringCentered(this.Font, text1, 220f, Color.Green, 0.9f);
      float y1 = 270f;
      int num1 = 0;
      int num2 = 0;
      foreach (DeathmatchMiniGame.PlayerData playerData in winners)
      {
        num1 = playerData.Kills;
        string text2 = playerData.Player.DisplayGamertag + "   Kills: " + (object) playerData.Kills;
        spriteBatch.DrawStringCentered(this.Font, text2, y1, Color.Yellow, 1f);
        y1 += 30f;
        if (num2++ == 8)
          break;
      }
      int num3 = 9 - num2;
      if (num3 > 0)
      {
        float y2 = y1 + 30f;
        if (NetworkManager.Instance.AllGamerCount < 8)
          y2 += 30f;
        spriteBatch.DrawStringCentered(this.Font, "Players of little consequence:", y2, Color.Orange, 0.9f);
        float y3 = y2 + 40f;
        foreach (NetworkGamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
        {
          if (allEnabledGamer.IsActive())
          {
            DeathmatchMiniGame.PlayerData playerData = this.game.GetPlayerData(allEnabledGamer.Tag as Player);
            if (playerData.Kills < num1)
            {
              string text2 = playerData.Player.DisplayGamertag + "   Kills: " + (object) playerData.Kills;
              spriteBatch.DrawStringCentered(this.Font, text2, y3, Color.DarkKhaki, 0.8f);
              y3 += 27f;
              if (--num3 == 0)
                break;
            }
          }
        }
      }
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
