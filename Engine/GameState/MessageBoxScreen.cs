// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.MessageBoxScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.Engine.GameState
{
  public class MessageBoxScreen : GameScreen
  {
    public new static TimeSpan DefaultTransitionOnTime = TimeSpan.FromSeconds(0.5);
    public new static TimeSpan DefaultTransitionOffTime = TimeSpan.FromSeconds(0.5);
    public static float DefaultFadeToBlack = 0.65f;
    public float FadeToBlack = MessageBoxScreen.DefaultFadeToBlack;
    protected float ygap = 20f;
    public object Sender;
    protected Rectangle imageRectangle;
    protected Rectangle backRectangle;
    protected string hMessage;
    protected string aMessage;
    protected string xMessage;
    protected string yMessage;
    protected string bMessage;
    protected bool cancelButtonDisabled;
    protected MessageBoxScreen.DrawMessageBoxBackgroundHandler backgroundHandler;
    protected Texture2D backTexture;
    protected Color backImageTint;
    protected SpriteFont font;
    protected float textScale;
    protected Vector2 hMeasure;
    protected Vector2 aMeasure;
    protected Vector2 xMeasure;
    protected Vector2 yMeasure;
    protected Vector2 bMeasure;
    private string backTextureName;
    private bool stretchBackImage;

    public override int FadeBackBufferAlpha
    {
      get
      {
        return (int) ((double) this.FadeToBlack * (double) byte.MaxValue);
      }
    }

    public event EventHandler<PlayerIndexEventArgs> ButtonA;

    public event EventHandler<PlayerIndexEventArgs> ButtonX;

    public event EventHandler<PlayerIndexEventArgs> ButtonY;

    public event EventHandler<PlayerIndexEventArgs> ButtonB;

    public MessageBoxScreen(string heading, string aMessage)
      : this(heading, aMessage, (string) null, (string) null, (string) null, CoreGlobals.GameFont, 1f, "")
    {
    }

    public MessageBoxScreen(string heading, string aMessage, string xMessage)
      : this(heading, aMessage, xMessage, (string) null, (string) null, CoreGlobals.GameFont, 1f, "")
    {
    }

    public MessageBoxScreen(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale)
      : this(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, "")
    {
    }

    public MessageBoxScreen(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      MessageBoxScreen.DrawMessageBoxBackgroundHandler backgroundHandler)
      : this(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, (string) null, false, Color.White)
    {
      this.backgroundHandler = backgroundHandler;
    }

    public MessageBoxScreen(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      string backTextureName)
      : this(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, backTextureName, false, Color.White)
    {
    }

    public MessageBoxScreen(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      string backTextureName,
      bool stretch,
      Color backImageTint)
    {
      this.hMessage = heading;
      this.aMessage = aMessage;
      this.xMessage = xMessage;
      this.yMessage = yMessage;
      this.bMessage = bMessage;
      this.backTextureName = backTextureName;
      this.font = font;
      this.textScale = textScale;
      this.stretchBackImage = stretch;
      this.backImageTint = backImageTint;
      this.Matrix = Matrix.Identity;
      this.IsPopup = true;
      this.TransitionOnTime = MessageBoxScreen.DefaultTransitionOnTime;
      this.TransitionOffTime = MessageBoxScreen.DefaultTransitionOffTime;
      InputManager.PushVirtualMouse();
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.hMessage = Utils.InsertNewLines(this.font, 960, this.textScale, this.hMessage, true);
      this.hMeasure = this.hMessage.IsEmpty() ? Vector2.Zero : this.font.MeasureString(this.hMessage) * this.textScale;
      this.aMeasure = this.aMessage.IsEmpty() ? Vector2.Zero : this.font.MeasureString(this.aMessage) * this.textScale;
      this.xMeasure = this.xMessage.IsEmpty() ? Vector2.Zero : this.font.MeasureString(this.xMessage) * this.textScale;
      this.yMeasure = this.yMessage.IsEmpty() ? Vector2.Zero : this.font.MeasureString(this.yMessage) * this.textScale;
      this.bMeasure = this.bMessage.IsEmpty() ? Vector2.Zero : this.font.MeasureString(this.bMessage) * this.textScale;
      int rectWidth = 48 + (int) Math.Max(Math.Max(Math.Max(Math.Max(this.hMeasure.X, this.aMeasure.X), this.bMeasure.X), this.xMeasure.X), this.yMeasure.X);
      int y = (int) this.hMeasure.Y;
      if ((double) this.aMeasure.Y > 0.0)
        y += (int) ((double) this.ygap + (double) this.aMeasure.Y);
      if ((double) this.xMeasure.Y > 0.0)
        y += (int) ((double) this.ygap + (double) this.xMeasure.Y);
      if ((double) this.yMeasure.Y > 0.0)
        y += (int) ((double) this.ygap + (double) this.yMeasure.Y);
      if ((double) this.bMeasure.Y > 0.0)
        y += (int) ((double) this.ygap + (double) this.bMeasure.Y);
      int rectHeight = y + (int) this.ygap;
      this.backRectangle = MyExtensions.CenterOfViewport(this.ScreenManager.GraphicsDevice.Viewport, rectWidth, rectHeight);
      if (!this.backTextureName.IsEmpty())
      {
        this.backTexture = this.content.Load<Texture2D>(this.backTextureName);
        rectWidth = this.backTexture.Width;
        rectHeight = this.backTexture.Height;
      }
      if (this.stretchBackImage)
        this.imageRectangle = this.ScreenManager.GraphicsDevice.Viewport.Rectangle();
      else
        this.imageRectangle = MyExtensions.CenterOfViewport(this.ScreenManager.GraphicsDevice.Viewport, rectWidth, rectHeight);
    }

    protected override void OnScreenRemovedCore()
    {
      InputManager.PopVirtualMouse();
      base.OnScreenRemovedCore();
    }

    public override bool HandleInput(InputState input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      if (base.HandleInput(input))
        return true;
      PlayerIndex playerIndex;
      if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex) && this.aMessage.IsNotEmpty())
      {
        this.ButtonAHandler((object) this, EventArgs.Empty);
        return true;
      }
      if (input.IsMenuXButton(this.ControllingPlayer, out playerIndex) && this.xMessage.IsNotEmpty())
      {
        this.ButtonXHandler((object) this, EventArgs.Empty);
        return true;
      }
      if (input.IsMenuYButton(this.ControllingPlayer, out playerIndex) && this.yMessage.IsNotEmpty())
      {
        this.ButtonYHandler((object) this, EventArgs.Empty);
        return true;
      }
      if (!input.IsMenuCancel(this.ControllingPlayer, out playerIndex) || this.cancelButtonDisabled)
        return false;
      this.ButtonBHandler((object) this, EventArgs.Empty);
      return true;
    }

    public void ButtonAHandler(object sender, EventArgs e)
    {
      if (this.ButtonA != null)
        this.ButtonA(this.Sender == null ? (object) this : this.Sender, new PlayerIndexEventArgs(this.ControllingPlayer));
      this.ExitScreen();
    }

    public void ButtonXHandler(object sender, EventArgs e)
    {
      if (this.ButtonX != null)
        this.ButtonX(this.Sender == null ? (object) this : this.Sender, new PlayerIndexEventArgs(this.ControllingPlayer));
      this.ExitScreen();
    }

    public void ButtonYHandler(object sender, EventArgs e)
    {
      if (this.ButtonY != null)
        this.ButtonY(this.Sender == null ? (object) this : this.Sender, new PlayerIndexEventArgs(this.ControllingPlayer));
      this.ExitScreen();
    }

    public void ButtonBHandler(object sender, EventArgs e)
    {
      if (this.ButtonB != null)
        this.ButtonB(this.Sender == null ? (object) this : this.Sender, new PlayerIndexEventArgs(this.ControllingPlayer));
      this.ExitScreen();
    }

    protected override void DrawCore()
    {
      Color color = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) this.TransitionAlpha);
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      if (this.backgroundHandler != null)
        this.backgroundHandler(spriteBatch, this.backRectangle, (float) this.TransitionAlpha / (float) byte.MaxValue, this.Matrix);
      if (this.Matrix == Matrix.Identity)
        spriteBatch.Begin();
      else
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, this.Matrix);
      if (this.backTexture != null)
        spriteBatch.Draw(this.backTexture, this.imageRectangle, this.backImageTint);
      int y = this.backRectangle.Y;
      if (this.hMessage.IsNotEmpty())
      {
        spriteBatch.DrawString(this.font, this.hMessage, new Vector2((float) this.backRectangle.X, (float) y), color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.hMeasure.Y + (double) this.ygap);
      }
      if (this.aMessage.IsNotEmpty())
      {
        Rectangle destinationRectangle = new Rectangle(this.backRectangle.X, y + (int) ((double) this.aMeasure.Y * 0.100000001490116), (int) ((double) this.aMeasure.Y * 0.800000011920929), (int) ((double) this.aMeasure.Y * 0.800000011920929));
        spriteBatch.Draw(CoreGlobals.ButtonTextureA, destinationRectangle, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.aMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.aMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.aMeasure.Y + (double) this.ygap);
      }
      if (this.xMessage.IsNotEmpty())
      {
        Rectangle destinationRectangle = new Rectangle(this.backRectangle.X, y + (int) ((double) this.xMeasure.Y * 0.100000001490116), (int) ((double) this.xMeasure.Y * 0.800000011920929), (int) ((double) this.xMeasure.Y * 0.800000011920929));
        spriteBatch.Draw(CoreGlobals.ButtonTextureX, destinationRectangle, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.xMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.xMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.xMeasure.Y + (double) this.ygap);
      }
      if (this.yMessage.IsNotEmpty())
      {
        Rectangle destinationRectangle = new Rectangle(this.backRectangle.X, y + (int) ((double) this.yMeasure.Y * 0.100000001490116), (int) ((double) this.yMeasure.Y * 0.800000011920929), (int) ((double) this.yMeasure.Y * 0.800000011920929));
        spriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.yMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.yMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.yMeasure.Y + (double) this.ygap);
      }
      if (this.bMessage.IsNotEmpty())
      {
        Rectangle destinationRectangle = new Rectangle(this.backRectangle.X, y + (int) ((double) this.bMeasure.Y * 0.100000001490116), (int) ((double) this.bMeasure.Y * 0.800000011920929), (int) ((double) this.bMeasure.Y * 0.800000011920929));
        spriteBatch.Draw(CoreGlobals.ButtonTextureB, destinationRectangle, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.bMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.bMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        int num = y + (int) ((double) this.bMeasure.Y + (double) this.ygap);
      }
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    public delegate void DrawMessageBoxBackgroundHandler(
      SpriteBatchSafe spriteBatch,
      Rectangle rect,
      float alpha,
      Matrix matrix);
  }
}
