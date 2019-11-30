// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.CubePrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public class CubePrimitive : BaseCubePrimitive<VertexPositionNormal>
  {
    public CubePrimitive(GraphicsDevice graphicsDevice)
      : base(graphicsDevice)
    {
    }

    public CubePrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, float size)
      : base(graphicsDevice, effect, size)
    {
    }

    public CubePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector3 size,
      Vector3 texFac)
      : base(graphicsDevice, effect, size, size, texFac)
    {
    }

    public CubePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector3 frontSize,
      Vector3 backSize,
      Vector3 texFac)
      : base(graphicsDevice, effect, frontSize, backSize, texFac)
    {
    }

    protected override void AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
