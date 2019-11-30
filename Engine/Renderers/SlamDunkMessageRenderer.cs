// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.SlamDunkMessageRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Renderers
{
  public class SlamDunkMessageRenderer : FadeOutMessageRenderer
  {
    public SlamDunkMessageRenderer(Microsoft.Xna.Framework.Game game, string fontPath)
      : base(game, fontPath)
    {
    }

    protected override void DrawCore(int i)
    {
      MessageRendererInstance message = this.messages[i];
      if ((double) message.Timer >= (double) message.Seconds)
      {
        ++message.State;
        if (message.State == 3)
        {
          this.messages.RemoveAt(i);
          return;
        }
        message.Timer = 0.0f;
        message.Seconds *= 0.5f;
      }
      float num = 1f;
      float scale = message.Scale;
      if (message.State == 2)
      {
        num = MathHelper.Lerp(1f, 0.0f, message.Timer / message.Seconds);
        scale = message.Scale * MathHelper.Lerp(1f, 0.755f, message.Timer / message.Seconds);
      }
      else if (message.State != 1)
      {
        num = MathHelper.Lerp(0.0f, 2f, message.Timer / message.Seconds);
        if ((double) num > 1.0)
          num = 1f;
        scale = message.Scale * MathHelper.Lerp(0.0f, 1f, message.Timer / message.Seconds);
      }
      Vector2 vector2 = this.gameFont.MeasureString(message.Text) * scale;
      vector2.X = (float) (((double) this.GraphicsDevice.Viewport.Width - (double) vector2.X) / 2.0);
      vector2.Y = (float) (((double) this.GraphicsDevice.Viewport.Height - (double) vector2.Y) / 2.0);
      this.spriteBatch.DrawString(this.gameFont, message.Text, vector2 + this.PositionOffset, message.Color * num, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
    }
  }
}
