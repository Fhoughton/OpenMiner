// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.SpriteBatchSafe
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace StudioForge.Engine
{
  public class SpriteBatchSafe : IDisposable
  {
    private SpriteBatch spritebatch;
    private bool beginCalled;
    private static Exception caught;

    public bool BeginCalled
    {
      get
      {
        return this.beginCalled;
      }
    }

    public GraphicsDevice GraphicsDevice
    {
      get
      {
        return this.spritebatch.GraphicsDevice;
      }
    }

    public SpriteBatchSafe(GraphicsDevice graphicsDevice)
    {
      this.spritebatch = new SpriteBatch(graphicsDevice);
      this.beginCalled = false;
    }

    public void Dispose()
    {
      this.spritebatch.Dispose();
    }

    public void Begin()
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin();
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Begin(SpriteSortMode sortMode, BlendState blendState)
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin(sortMode, blendState);
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Begin(
      SpriteSortMode sortMode,
      BlendState blendState,
      SamplerState samplerState,
      DepthStencilState depthStencilState,
      RasterizerState rasterizerState)
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState);
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Begin(
      SpriteSortMode sortMode,
      BlendState blendState,
      SamplerState samplerState,
      DepthStencilState depthStencilState,
      RasterizerState rasterizerState,
      Effect effect)
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Begin(
      SpriteSortMode sortMode,
      BlendState blendState,
      SamplerState samplerState,
      DepthStencilState depthStencilState,
      RasterizerState rasterizerState,
      Effect effect,
      Matrix transformMatrix)
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Begin(Matrix transformMatrix)
    {
      if (this.beginCalled)
        this.End();
      if (this.beginCalled)
        return;
      if (this.spritebatch.IsDisposed)
        return;
      try
      {
        this.spritebatch.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, (RasterizerState) null, (Effect) null, transformMatrix);
        this.beginCalled = true;
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
      }
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, destinationRectangle, color);
    }

    public void Draw(Texture2D texture, Vector2 position, Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, position, color);
    }

    public void Draw(
      Texture2D texture,
      Rectangle destinationRectangle,
      Rectangle? sourceRectangle,
      Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, destinationRectangle, sourceRectangle, color);
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, position, sourceRectangle, color);
    }

    public void Draw(
      Texture2D texture,
      Rectangle destinationRectangle,
      Rectangle? sourceRectangle,
      Color color,
      float rotation,
      Vector2 origin,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
    }

    public void Draw(
      Texture2D texture,
      Vector2 position,
      Rectangle? sourceRectangle,
      Color color,
      float rotation,
      Vector2 origin,
      float scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    public void Draw(
      Texture2D texture,
      Vector2 position,
      Rectangle? sourceRectangle,
      Color color,
      float rotation,
      Vector2 origin,
      Vector2 scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color);
    }

    public void DrawString(
      SpriteFont spriteFont,
      StringBuilder text,
      Vector2 position,
      Color color)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color);
    }

    public void DrawString(
      SpriteFont spriteFont,
      string text,
      Vector2 position,
      Color color,
      float rotation,
      Vector2 origin,
      float scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
    }

    public void DrawString(
      SpriteFont spriteFont,
      string text,
      Vector2 position,
      Color color,
      float rotation,
      Vector2 origin,
      Vector2 scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
    }

    public void DrawString(
      SpriteFont spriteFont,
      StringBuilder text,
      Vector2 position,
      Color color,
      float rotation,
      Vector2 origin,
      float scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
    }

    public void DrawString(
      SpriteFont spriteFont,
      StringBuilder text,
      Vector2 position,
      Color color,
      float rotation,
      Vector2 origin,
      Vector2 scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (!this.beginCalled)
        return;
      this.spritebatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
    }

    public void End()
    {
      if (!this.beginCalled)
        return;
      try
      {
        this.spritebatch.End();
      }
      catch (InvalidOperationException ex)
      {
        this.ReportException((Exception) ex);
        this.spritebatch.Dispose();
        this.spritebatch = new SpriteBatch(CoreGlobals.GraphicsDevice);
      }
      finally
      {
        this.beginCalled = false;
      }
    }

    private void ReportException(Exception e)
    {
      SpriteBatchSafe.caught = e;
    }

    private void BadTexture()
    {
    }
  }
}
