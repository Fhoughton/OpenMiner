// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.TexturedQuadPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public class TexturedQuadPrimitive : BaseQuadPrimitive<VertexPositionNormalTexture>
  {
    public TexturedQuadPrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f)
    {
    }

    public TexturedQuadPrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, float size)
      : this(graphicsDevice, effect, new Vector2(size))
    {
    }

    public TexturedQuadPrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, Vector2 size)
      : base(graphicsDevice, effect, size, new Vector4(0.0f, 0.0f, 1f, 1f))
    {
    }

    public TexturedQuadPrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector4 UV)
      : base(graphicsDevice, effect, size, UV)
    {
    }

    protected override void AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
      this.AddVertex(new VertexPositionNormalTexture(position, normal, uv));
    }
  }
}
