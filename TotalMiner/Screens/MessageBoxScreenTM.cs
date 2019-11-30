// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MessageBoxScreenTM
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class MessageBoxScreenTM : MessageBoxScreen
  {
    public Player Player;

    public MessageBoxScreenTM(string heading, string aMessage, Player player)
      : base(heading, aMessage)
    {
      this.Player = player;
    }

    public MessageBoxScreenTM(string heading, string aMessage, string xMessage, Player player)
      : base(heading, aMessage, xMessage)
    {
      this.Player = player;
    }

    public MessageBoxScreenTM(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      Player player)
      : base(heading, aMessage, xMessage, yMessage, bMessage, font, textScale)
    {
      this.Player = player;
    }

    public MessageBoxScreenTM(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      MessageBoxScreen.DrawMessageBoxBackgroundHandler backgroundHandler,
      Player player)
      : base(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, backgroundHandler)
    {
      this.Player = player;
    }

    public MessageBoxScreenTM(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      string backTextureName,
      Player player)
      : base(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, backTextureName)
    {
      this.Player = player;
    }

    public MessageBoxScreenTM(
      string heading,
      string aMessage,
      string xMessage,
      string yMessage,
      string bMessage,
      SpriteFont font,
      float textScale,
      string backTextureName,
      bool stretch,
      Color backImageTint,
      Player player)
      : base(heading, aMessage, xMessage, yMessage, bMessage, font, textScale, backTextureName, stretch, backImageTint)
    {
      this.Player = player;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.UpdateMatrix();
      NetworkManager.Instance.GamerJoined += new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
      NetworkManager.Instance.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
    }

    protected override void OnScreenRemovedCore()
    {
      NetworkManager.Instance.GamerJoined -= new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
      NetworkManager.Instance.GamerLeft -= new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
      base.OnScreenRemovedCore();
    }

    private void GamerJoinedEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    private void UpdateMatrix()
    {
      if (this.Player == null)
        return;
      Rectangle backRectangle = this.backRectangle;
      backRectangle.Inflate(48, 48);
      this.Matrix = this.Player.GetScreenMatrix(backRectangle);
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
        Rectangle rect = new Rectangle(this.backRectangle.X, y + (int) ((double) this.aMeasure.Y * 0.100000001490116), (int) ((double) this.aMeasure.Y * 0.800000011920929), (int) ((double) this.aMeasure.Y * 0.800000011920929));
        GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.SelectItem, rect, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.aMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.aMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.aMeasure.Y + (double) this.ygap);
        if (this.drawFrameCount == 1)
        {
          rect.Width += (int) ((double) this.aMeasure.X + (double) this.aMeasure.Y);
          this.AddWinRect(rect.Expand(3), new EventHandler<EventArgs>(((MessageBoxScreen) this).ButtonAHandler));
        }
      }
      if (this.xMessage.IsNotEmpty())
      {
        Rectangle rect = new Rectangle(this.backRectangle.X, y + (int) ((double) this.xMeasure.Y * 0.100000001490116), (int) ((double) this.xMeasure.Y * 0.800000011920929), (int) ((double) this.xMeasure.Y * 0.800000011920929));
        GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.MsgBoxX, rect, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.xMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.xMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.xMeasure.Y + (double) this.ygap);
        if (this.drawFrameCount == 1)
        {
          rect.Width += (int) ((double) this.xMeasure.X + (double) this.xMeasure.Y);
          this.AddWinRect(rect.Expand(3), new EventHandler<EventArgs>(((MessageBoxScreen) this).ButtonXHandler));
        }
      }
      if (this.yMessage.IsNotEmpty())
      {
        Rectangle rect = new Rectangle(this.backRectangle.X, y + (int) ((double) this.yMeasure.Y * 0.100000001490116), (int) ((double) this.yMeasure.Y * 0.800000011920929), (int) ((double) this.yMeasure.Y * 0.800000011920929));
        GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.MsgBoxY, rect, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.yMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.yMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        y += (int) ((double) this.yMeasure.Y + (double) this.ygap);
        if (this.drawFrameCount == 1)
        {
          rect.Width += (int) ((double) this.yMeasure.X + (double) this.yMeasure.Y);
          this.AddWinRect(rect.Expand(3), new EventHandler<EventArgs>(((MessageBoxScreen) this).ButtonYHandler));
        }
      }
      if (this.bMessage.IsNotEmpty())
      {
        Rectangle rect = new Rectangle(this.backRectangle.X, y + (int) ((double) this.bMeasure.Y * 0.100000001490116), (int) ((double) this.bMeasure.Y * 0.800000011920929), (int) ((double) this.bMeasure.Y * 0.800000011920929));
        GraphicStatics.DrawInputIcon(spriteBatch, GuiInput.ExitScreen, rect, color);
        Vector2 position = new Vector2((float) (this.backRectangle.X + (int) this.bMeasure.Y + 6), (float) (y + 1));
        spriteBatch.DrawString(this.font, this.bMessage, position, color, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 1f);
        int num = y + (int) ((double) this.bMeasure.Y + (double) this.ygap);
        if (this.drawFrameCount == 1)
        {
          rect.Width += (int) ((double) this.bMeasure.X + (double) this.bMeasure.Y);
          this.AddWinRect(rect.Expand(3), new EventHandler<EventArgs>(((MessageBoxScreen) this).ButtonBHandler));
        }
      }
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
