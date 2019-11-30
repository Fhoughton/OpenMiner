// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Game.ExceptionGame
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using System;
using System.Windows.Forms;

namespace StudioForge.Engine.Game
{
  public class ExceptionGame : Microsoft.Xna.Framework.Game
  {
    public static string FontName = "Fonts\\Consolas";
    public static float FontScale = 1f;
    public static int SessionProperty0;
    private Vector2 pos;
    private SpriteBatchSafe batch;
    private SpriteFont font;
    private string exceptionString;
    private bool drawOnce;
    private bool reportSent;
    private WindowManager windowManager;

    public ExceptionGame(Exception e, string gameName)
    {
      CoreGlobals.ClearReferenceCache();
      StudioForge.Engine.Services.Instance = (IServiceProvider) this.Services;
      GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager((Microsoft.Xna.Framework.Game) this)
      {
        PreferredBackBufferWidth = 1280,
        PreferredBackBufferHeight = 720
      };
      this.Content.RootDirectory = "Content";
      this.exceptionString = gameName + "\n\n" + e.ToString();
      if (this.exceptionString.Length <= 3000)
        return;
      this.exceptionString = this.exceptionString.Substring(0, 3000);
    }

    protected override void Initialize()
    {
      this.IsMouseVisible = true;
      base.Initialize();
    }

    protected override void LoadContent()
    {
      Texture2D texture2D = new Texture2D(this.GraphicsDevice, 1, 1);
      texture2D.SetData<Color>(new Color[1]
      {
        Color.White
      });
      this.Services.AddService(typeof (Texture2D), (object) texture2D);
      this.batch = new SpriteBatchSafe(this.GraphicsDevice);
      this.Services.AddService(typeof (SpriteBatchSafe), (object) new SpriteBatchSafe(this.GraphicsDevice));
      this.Services.AddService(typeof (IFrameRateCounter), (object) new FrameRateCounter());
      this.font = this.Content.Load<SpriteFont>(ExceptionGame.FontName);
      CoreGlobals.GameFont = CoreGlobals.MenuFont = this.font;
      this.pos = new Vector2((float) this.GraphicsDevice.Viewport.TitleSafeArea.X, (float) this.GraphicsDevice.Viewport.TitleSafeArea.Y);
      this.windowManager = new WindowManager(PlayerIndex.One);
      this.windowManager.LoadContent();
      StudioForge.Engine.GUI.TextBox textBox = new StudioForge.Engine.GUI.TextBox("Copy to Clipboard", 4, 4, 250, 30);
      textBox.Colors = (Window.ColorProfile) StudioForge.Engine.GUI.TextBox.DefaultColorProfile;
      textBox.ClickHandler += new Window.WindowHandler(this.ClickCopyToClipboard);
      this.windowManager.Root.AddChild((Node) textBox);
      this.windowManager.SetNavigable((Window) textBox);
      InputManager.PushVirtualMouse();
    }

    private void ClickCopyToClipboard(object sender, WindowEventArgs e)
    {
      Clipboard.SetText(this.exceptionString, TextDataFormat.Text);
    }

    protected override void Update(GameTime gameTime)
    {
      this.IsMouseVisible = true;
      base.Update(gameTime);
      GamePadState state1 = GamePad.GetState(PlayerIndex.One);
      GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
      GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
      GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
      Vector2 left1 = state1.ThumbSticks.Left;
      left1.Y = -left1.Y;
      Vector2 left2 = state2.ThumbSticks.Left;
      left2.Y = -left2.Y;
      Vector2 left3 = state3.ThumbSticks.Left;
      left3.Y = -left3.Y;
      Vector2 left4 = state4.ThumbSticks.Left;
      left4.Y = -left4.Y;
      this.pos += left1 * 5f;
      this.pos += left2 * 5f;
      this.pos += left3 * 5f;
      this.pos += left4 * 5f;
      if (!this.reportSent && this.drawOnce)
        this.SendReport();
      InputManager.Update();
      this.windowManager.HandleInput();
      this.windowManager.Update();
      this.IsMouseVisible = true;
    }

    protected override void Draw(GameTime donotuse)
    {
      this.GraphicsDevice.Clear(Color.DarkBlue);
      this.batch.Begin();
      this.batch.DrawString(this.font, this.exceptionString, this.pos, Color.White, 0.0f, Vector2.Zero, ExceptionGame.FontScale, SpriteEffects.None, 0.0f);
      this.batch.End();
      this.drawOnce = true;
      this.windowManager.Draw();
      base.Draw(donotuse);
    }

    private void SendReport()
    {
    }

    private void EndFindSession(IAsyncResult ar)
    {
    }

    private void SendTrace()
    {
    }
  }
}
