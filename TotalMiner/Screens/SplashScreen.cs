// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SplashScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class SplashScreen : GameScreen
  {
    private string dotString = "...";
    private PlayerIndex? startControllingPlayer;
    private GamePadButtons prevButtons;
    private KeyboardState prevKeyboard;
    private BackgroundScreen bkgdScreen;
    private int dotCounter;
    private int state;

    public SplashScreen(BackgroundScreen bkgdScreen)
    {
      this.TransitionOffTime = TimeSpan.Zero;
      this.bkgdScreen = bkgdScreen;
      this.state = 0;
      Globals2.AutoStartMap = 0;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      Globals2.Initialize();
      GraphicStatics.Initialize();
    }

    public override bool HandleInput(InputState input)
    {
      if (this.state == 0)
      {
        PlayerIndex? nullable = new PlayerIndex?();
        nullable = new PlayerIndex?(PlayerIndex.One);
        if (nullable.HasValue)
        {
          if (InputManager1.IsInputPressedNew(nullable.Value, GuiInput.MsgBoxX))
            Globals2.AutoStartMap = -2;
          else if (InputManager1.IsInputPressedNew(nullable.Value, GuiInput.MsgBoxY))
            Globals2.AutoStartMap = -1;
          this.startControllingPlayer = nullable;
          InputManager.SetPlayerIndex(nullable.Value);
        }
        if (this.startControllingPlayer.HasValue && (GamePad.GetState(this.startControllingPlayer.Value).Buttons == this.prevButtons && Keyboard.GetState() == this.prevKeyboard) && (Mouse.GetState().LeftButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Released))
        {
          this.state = 1;
          Globals2.LocalGamer = new Gamer(new GamerID(Math.Abs((short) (new PcgRandom(new Random().Next()).Next() + 1))), "LocalTest", this.startControllingPlayer.Value);
        }
      }
      return false;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.state <= 0)
        return;
      if (!this.startControllingPlayer.HasValue)
        this.state = 0;
      else if (this.state == 1)
      {
        this.state = 2;
      }
      else
      {
        if (++this.dotCounter == 30)
        {
          this.dotCounter = 0;
          this.dotString += ".";
          if (this.dotString.Length > 4)
            this.dotString = ".";
        }
        if (this.state == 2)
          this.state = 3;
        else if (this.state == 3)
        {
          this.state = 4;
          this.LoadMenu();
        }
        else
        {
          if (this.state != 5)
            return;
          this.ExitScreen();
        }
      }
    }

    private void LoadMenu()
    {
      new Thread(new ThreadStart(this.LoadMenuCore))
      {
        CurrentCulture = Globals1.CultureInfo,
        CurrentUICulture = Globals1.CultureInfo
      }.Start();
    }

    private void LoadMenuCore()
    {
      CoreGlobals.ClearReferenceCache();
      GlobalGamerSettings globalGamerSettings = this.startControllingPlayer.HasValue ? Globals2.GamertagData.GetGlobalGamerSettings(this.startControllingPlayer.Value) : (GlobalGamerSettings) null;
      if (globalGamerSettings != null && globalGamerSettings.GameSettings != null)
      {
        GameScreen.SetToolTips(globalGamerSettings.GameSettings.ToolTips);
        if (!GraphicStatics.LoadWindowBorder(globalGamerSettings.GameSettings.WindowBorder))
        {
          globalGamerSettings.GameSettings.WindowBorder = "Blade1";
          GraphicStatics.LoadWindowBorder(globalGamerSettings.GameSettings.WindowBorder);
        }
      }
      else
        GraphicStatics.LoadWindowBorder("Blade1");
      if (Globals2.AutoStartMap == -2)
        Globals2.AutoStartMap = Globals2.LastMapPlayed;
      this.ScreenManager.AddScreen((GameScreen) new MainMenuScreen(), this.startControllingPlayer);
      this.state = 5;
    }

    protected override void DrawCore()
    {
      if (this.state > 0)
        return;
      this.ScreenManager.SpriteBatch.Begin();
      Vector2 position = new Vector2(146f, (float) (this.GraphicsDevice.Viewport.Height - 150));
      this.ScreenManager.SpriteBatch.DrawGradient(new Rectangle((int) position.X - 80, (int) ((double) position.Y - 10.0), 480, 70), 80, 80, Color.Black * 0.5f, Matrix.Identity);
      this.ScreenManager.SpriteBatch.DrawString(CoreGlobals.GameFont, "Press any Key", position + new Vector2(2f, 2f), Color.Black, 0.0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0.0f);
      this.ScreenManager.SpriteBatch.DrawString(CoreGlobals.GameFont, "Press any Key", position, Color.White, 0.0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0.0f);
      position.X = (float) (this.GraphicsDevice.Viewport.Width - 480);
      this.ScreenManager.SpriteBatch.DrawGradient(new Rectangle((int) position.X - 80, (int) ((double) position.Y - 10.0), 380, 70), 80, 80, Color.Black * 0.5f, Matrix.Identity);
      position.Y += 5f;
      GraphicStatics.DrawInputIcon(this.ScreenManager.SpriteBatch, GuiInput.MsgBoxX, new Rectangle((int) position.X, (int) position.Y, 24, 24));
      this.ScreenManager.SpriteBatch.DrawString(CoreGlobals.GameFont, "= Continue Last Save", position + new Vector2(34f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      position.Y += 20f;
      GraphicStatics.DrawInputIcon(this.ScreenManager.SpriteBatch, GuiInput.MsgBoxY, new Rectangle((int) position.X, (int) position.Y, 24, 24));
      this.ScreenManager.SpriteBatch.DrawString(CoreGlobals.GameFont, "= New Creative Flat Map", position + new Vector2(34f, 0.0f), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      this.ScreenManager.SpriteBatch.End();
    }
  }
}
