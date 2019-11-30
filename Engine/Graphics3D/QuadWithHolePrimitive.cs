// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.QuadWithHolePrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public class QuadWithHolePrimitive : BaseQuadWithHolePrimitive<VertexPositionNormal>
  {
    public QuadWithHolePrimitive(GraphicsDevice graphicsDevice)
      : base(graphicsDevice)
    {
    }

    public QuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float size,
      float holeSize)
      : base(graphicsDevice, effect, size, holeSize)
    {
    }

    public QuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector2 holeSize)
      : base(graphicsDevice, effect, size, holeSize, Vector2.One)
    {
    }

    protected override void AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
