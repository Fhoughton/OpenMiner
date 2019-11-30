// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.BackgroundScreen
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.GameState
{
  public class BackgroundScreen : GameScreen
  {
    protected float userFade = 1f;
    protected Texture2D backgroundTexture;
    protected SpriteBatchSafe spriteBatch;

    public override void LoadContent()
    {
      if (this.content == null)
      {
        this.IsContentLoaded = true;
        this.content = (IContentManager) new ContentManager((IServiceProvider) this.ScreenManager.Game.Services, "Content");
      }
      base.LoadContent();
      this.backgroundTexture = this.LoadBackgroundTexture();
      this.spriteBatch = new SpriteBatchSafe(this.GraphicsDevice);
    }

    protected virtual Texture2D LoadBackgroundTexture()
    {
      return this.content.Load<Texture2D>(Services.ScreenManagerPath + nameof (BackgroundScreen));
    }

    protected override void DrawCore()
    {
      float fade = this.TransitionAlphaFloat * this.userFade;
      this.SpriteBatchStart(this.spriteBatch);
      this.DrawCore2(fade);
      this.spriteBatch.End();
      this.DrawOverlay();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
      base.DrawCore();
    }

    protected virtual void DrawCore2(float fade)
    {
      if (this.backgroundTexture == null)
        return;
      if (this.DrawByRectangle)
        this.spriteBatch.Draw(this.backgroundTexture, this.GraphicsDevice.Viewport.Rectangle(), new Rectangle?(this.DrawSourceRect), new Color(fade, fade, fade));
      else
        this.spriteBatch.Draw(this.backgroundTexture, this.DrawPosition, new Rectangle?(this.DrawSourceRect), new Color(fade, fade, fade));
    }

    protected virtual void DrawOverlay()
    {
    }

    protected virtual void SpriteBatchStart(SpriteBatchSafe spriteBatch)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
    }

    protected virtual bool DrawByRectangle
    {
      get
      {
        return true;
      }
    }

    protected virtual Vector2 DrawPosition
    {
      get
      {
        return Vector2.Zero;
      }
    }

    protected virtual Rectangle DrawSourceRect
    {
      get
      {
        if (this.backgroundTexture != null)
          return new Rectangle(0, 0, this.backgroundTexture.Width, this.backgroundTexture.Height);
        return new Rectangle();
      }
    }
  }
}
