// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.FadeOutMessageRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Renderers
{
  public class FadeOutMessageRenderer : DrawableGameComponent, IMessageDisplay
  {
    public Vector2 PositionOffset;
    public int MaxMsgCount;
    protected SpriteFont gameFont;
    protected SpriteBatchSafe spriteBatch;
    internal List<MessageRendererInstance> messages;
    private string fontPath;

    public FadeOutMessageRenderer(Microsoft.Xna.Framework.Game game, string fontPath)
      : base(game)
    {
      this.fontPath = fontPath;
      this.PositionOffset = Vector2.Zero;
    }

    public override void Initialize()
    {
      this.Enabled = this.Visible = true;
      this.messages = new List<MessageRendererInstance>();
      base.Initialize();
    }

    protected override void LoadContent()
    {
      this.spriteBatch = new SpriteBatchSafe(this.GraphicsDevice);
      this.gameFont = CoreGlobals.Content.Load<SpriteFont>(this.fontPath);
      base.LoadContent();
    }

    public SpriteFont Font
    {
      get
      {
        return this.gameFont;
      }
    }

    public Vector2 ShowMessage(string message, params object[] parameters)
    {
      return this.ShowMessage(message, 1f, Color.White);
    }

    public Vector2 ShowMessage(string message, float scale, Color color)
    {
      return this.ShowMessage(message, 1.5f, scale, color);
    }

    public Vector2 ShowMessage(string message, float time, float scale, Color color)
    {
      return this.ShowMessage(message, new Vector2(0.0f, -1f), time, scale, color);
    }

    public Vector2 ShowMessage(string message, Vector2 velocity, float scale, Color color)
    {
      return this.ShowMessage(message, velocity, 1.5f, scale, color);
    }

    public Vector2 ShowMessage(
      string message,
      Vector2 velocity,
      float time,
      float scale,
      Color color)
    {
      return this.ShowMessage(message, velocity, time, scale, color, Matrix.Identity);
    }

    public Vector2 ShowMessage(
      string message,
      Vector2 velocity,
      float time,
      float scale,
      Color color,
      Matrix matrix)
    {
      Point center = this.Game.GraphicsDevice.Viewport.TitleSafeArea.Center;
      return this.ShowMessage(message, new Vector2((float) center.X, (float) center.Y), velocity, time, scale, color, true, matrix);
    }

    public Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color)
    {
      return this.ShowMessage(message, position, velocity, seconds, scale, color, true);
    }

    public Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered)
    {
      return this.ShowMessage(message, position, velocity, seconds, scale, color, centered, Matrix.Identity);
    }

    public Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered,
      Matrix matrix)
    {
      MessageRendererInstance m = new MessageRendererInstance()
      {
        Text = message,
        Position = position,
        Velocity = velocity,
        Seconds = seconds,
        Scale = scale,
        Color = color,
        Matrix = matrix
      };
      Vector2 vector2 = this.gameFont.MeasureString(message) * scale;
      m.Position = new Vector2(position.X, position.Y - (float) (int) ((double) vector2.Y * 0.5));
      if (centered)
        m.Position.X -= (float) (int) ((double) vector2.X * 0.5);
      lock (this.messages)
      {
        if (this.MaxMsgCount > 0 && this.messages.Count >= this.MaxMsgCount)
          return Vector2.Zero;
        this.messages.Add(m);
      }
      this.SetupMessage(m);
      return vector2;
    }

    internal virtual void SetupMessage(MessageRendererInstance m)
    {
    }

    public override void Draw(GameTime donotuse)
    {
      try
      {
        bool flag = false;
        Matrix identity = Matrix.Identity;
        lock (this.messages)
        {
          for (int i = this.messages.Count - 1; i >= 0; --i)
          {
            MessageRendererInstance message = this.messages[i];
            if (message != null)
            {
              if (!flag || message.Matrix != identity)
              {
                this.spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, (RasterizerState) null, (Effect) null, message.Matrix);
                flag = true;
                ++CoreGlobals.FrameRateCounter.SpriteCalls;
              }
              this.messages[i].Timer += Services.ElapsedTime;
              this.DrawCore(i);
            }
          }
        }
        if (!flag)
          return;
        this.spriteBatch.End();
        this.spriteBatch.ResetRenderStates();
      }
      catch (OutOfMemoryException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(-3, (Exception) ex);
      }
    }

    protected virtual void DrawCore(int i)
    {
      MessageRendererInstance message = this.messages[i];
      if ((double) message.Timer >= (double) message.Seconds)
      {
        this.messages.RemoveAt(i);
      }
      else
      {
        float num1 = message.Seconds * 0.25f;
        float num2 = (double) message.Timer > (double) message.Seconds - (double) num1 ? MathHelper.Lerp(1f, 0.05f, (message.Timer - (message.Seconds - num1)) / num1) : 1f;
        message.Position += message.Velocity * Services.ElapsedTime;
        this.spriteBatch.DrawString(this.gameFont, message.Text, message.Position + this.PositionOffset, message.Color * num2, 0.0f, Vector2.Zero, message.Scale, SpriteEffects.None, 1f);
      }
    }
  }
}
