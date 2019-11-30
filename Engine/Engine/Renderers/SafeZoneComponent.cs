// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.SafeZoneComponent
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Renderers
{
  public class SafeZoneComponent : DrawableGameComponent
  {
    private SpriteBatchSafe spriteBatch;

    public SafeZoneComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      this.Enabled = this.Visible = false;
      this.DrawOrder = int.MaxValue;
    }

    protected override void LoadContent()
    {
      this.spriteBatch = new SpriteBatchSafe(this.GraphicsDevice);
      base.LoadContent();
    }

    public override void Draw(GameTime donotuse)
    {
      this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
      this.RenderZone(SafeZone.GetTitleSafeArea(this.GraphicsDevice, 0.8f), Color.Yellow * 0.3f);
      this.RenderZone(SafeZone.GetTitleSafeArea(this.GraphicsDevice, 0.9f), Color.Red * 0.3f);
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
      base.Draw(donotuse);
    }

    private void RenderZone(Rectangle rect, Color color)
    {
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, 0, viewport.Width, rect.Y), color);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, rect.Y, rect.X, rect.Height), color);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(rect.X + rect.Width, rect.Y, viewport.Width - (rect.X + rect.Width), rect.Height), color);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, rect.Y + rect.Height, viewport.Width, viewport.Height - (rect.Y + rect.Height)), color);
    }
  }
}
