// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SleepingScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class SleepingScreen : GameScreen
  {
    private Rectangle screenRect;
    private SpriteBatchSafe spriteBatch;
    private Player player;
    private GameInstance instance;
    private static long lastTimeMessageSent;
    private int oldWaitingCount;

    public SleepingScreen(GameInstance instance, Player player)
    {
      this.instance = instance;
      this.player = player;
      if (player == null)
        return;
      this.Matrix = player.GetScreenMatrix();
    }

    public override void LoadContent()
    {
      this.screenRect = MyExtensions.CenterOfViewport(576, 192);
      base.LoadContent();
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.Font = this.ScreenManager.GameFont;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (!InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
        return base.HandleInput(input);
      this.player.IsSleeping = false;
      return true;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
    }

    protected override void DrawCore()
    {
      Rectangle screenRect = this.screenRect;
      screenRect.X -= 48;
      screenRect.Y -= 48;
      screenRect.Width += 96;
      screenRect.Height += 96;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      int num1 = this.instance.AllPlayerEnabledCount - this.instance.PlayersSleepingCount;
      if (num1 == 0)
      {
        int num2 = (int) ((double) this.instance.TimeSleptInSeconds / 0.0138888889923692);
        int num3 = num2 / 3600;
        int num4 = (num2 - num3 * 3600) / 60;
        string str1 = num3.ToString().PadLeft(2, '0');
        string str2 = num4.ToString().PadLeft(2, '0');
        this.spriteBatch.DrawString(CoreGlobals.GameFont, string.Format("Time Slept: {0}:{1}", (object) str1, (object) str2), new Vector2((float) (this.screenRect.X + 136), (float) (this.screenRect.Y + this.screenRect.Height / 2 - 20)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      }
      else
      {
        if (this.oldWaitingCount != num1 && this.player.IsSleeping && Globals1.ElapsedWatch.ElapsedMilliseconds - SleepingScreen.lastTimeMessageSent > 4000L)
        {
          this.instance.AddNotification(string.Format("{0} is waiting on {1} other player(s) to sleep", (object) this.player.Gamertag, (object) num1), NotifyRecipient.Remote);
          SleepingScreen.lastTimeMessageSent = Globals1.ElapsedWatch.ElapsedMilliseconds;
        }
        this.spriteBatch.DrawString(CoreGlobals.GameFont, string.Format("Waiting on {0} other players", (object) num1), new Vector2((float) (this.screenRect.X + 48), (float) (this.screenRect.Y + 48)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        Vector2 position = new Vector2((float) (this.screenRect.X + 20), (float) (this.screenRect.Y + this.screenRect.Height - 40));
        this.spriteBatch.Draw(CoreGlobals.ButtonTextureB, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0.0f);
        position.X += 42f;
        position.Y += 4f;
        this.spriteBatch.DrawString(this.ScreenManager.GameFont, "Close", position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      }
      this.spriteBatch.End();
      this.oldWaitingCount = num1;
      if (this.player.IsSleeping)
        return;
      this.ExitScreen();
    }
  }
}
