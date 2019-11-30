// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.TexturedQuadWithHolePrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public class TexturedQuadWithHolePrimitive : BaseQuadWithHolePrimitive<VertexPositionNormalTexture>
  {
    public TexturedQuadWithHolePrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f, 0.5f)
    {
    }

    public TexturedQuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float size,
      float holeSize)
      : this(graphicsDevice, effect, new Vector2(size), new Vector2(holeSize), Vector2.One)
    {
    }

    public TexturedQuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector2 holeSize)
      : base(graphicsDevice, effect, size, holeSize, Vector2.One)
    {
    }

    public TexturedQuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector2 holeSize,
      Vector2 UV)
      : base(graphicsDevice, effect, size, holeSize, UV)
    {
    }

    protected override void AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
      this.AddVertex(new VertexPositionNormalTexture(position, normal, uv));
    }
  }
}
