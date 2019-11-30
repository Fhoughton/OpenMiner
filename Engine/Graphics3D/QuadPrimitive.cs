// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.QuadPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public class QuadPrimitive : BaseQuadPrimitive<VertexPositionNormal>
  {
    public QuadPrimitive(GraphicsDevice graphicsDevice)
      : base(graphicsDevice)
    {
    }

    public QuadPrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, float size)
      : base(graphicsDevice, effect, size)
    {
    }

    public QuadPrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, Vector2 size)
      : base(graphicsDevice, effect, size, new Vector4(0.0f, 0.0f, 1f, 1f))
    {
    }

    protected override void AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
