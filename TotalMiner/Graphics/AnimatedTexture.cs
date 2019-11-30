// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.AnimatedTexture
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal class AnimatedTexture
  {
    private int frameCount;
    private int frameDelay;
    private float rotation;
    private bool isDrawing;
    private string texname;
    private Vector2 origin;
    private Vector2 position;
    private Texture2D texture;
    private AnimateTextureCondition condition;

    public AnimatedTexture(
      string texname,
      Vector2 position,
      int frameDelay,
      AnimateTextureCondition condition)
    {
      this.texname = texname;
      this.position = position;
      this.frameDelay = frameDelay;
      this.condition = condition;
    }

    public void LoadContent()
    {
      this.texture = CoreGlobals.Content.Load<Texture2D>(this.texname);
      this.origin = new Vector2((float) this.texture.Width * 0.5f, (float) this.texture.Height * 0.5f);
    }

    public void UpdatePosition(Vector2 position)
    {
      this.position = position;
    }

    public void Draw(SpriteBatchSafe spriteBatch)
    {
      if (++this.frameCount > this.frameDelay)
      {
        this.frameCount = 0;
        this.isDrawing = this.condition == null || this.condition();
      }
      if (!this.isDrawing)
        return;
      this.rotation += (float) Math.PI / 40f;
      spriteBatch.Draw(this.texture, this.position, new Rectangle?(), Color.White, this.rotation, this.origin, 1f, SpriteEffects.None, 0.0f);
    }
  }
}
