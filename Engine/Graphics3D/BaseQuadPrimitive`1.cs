// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.BaseQuadPrimitive`1
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public abstract class BaseQuadPrimitive<V> : GeometricPrimitive<V> where V : struct
  {
    public BaseQuadPrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f)
    {
    }

    public BaseQuadPrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, float size)
      : this(graphicsDevice, effect, new Vector2(size), new Vector4(0.0f, 0.0f, 1f, 1f))
    {
    }

    public BaseQuadPrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector4 texCoord)
      : base(effect)
    {
      Vector2[] vector2Array = new Vector2[4]
      {
        new Vector2(texCoord.X, texCoord.W),
        new Vector2(texCoord.X, texCoord.Y),
        new Vector2(texCoord.Z, texCoord.Y),
        new Vector2(texCoord.Z, texCoord.W)
      };
      Vector3 normal = new Vector3(0.0f, 1f, 0.0f);
      this.AddIndex(0);
      this.AddIndex(1);
      this.AddIndex(2);
      this.AddIndex(0);
      this.AddIndex(2);
      this.AddIndex(3);
      this.AddVertex(new Vector3(-size.X, 0.0f, size.Y), normal, vector2Array[0]);
      this.AddVertex(new Vector3(-size.X, 0.0f, -size.Y), normal, vector2Array[1]);
      this.AddVertex(new Vector3(size.X, 0.0f, -size.Y), normal, vector2Array[2]);
      this.AddVertex(new Vector3(size.X, 0.0f, size.Y), normal, vector2Array[3]);
      this.InitializePrimitive(graphicsDevice);
    }

    protected abstract void AddVertex(Vector3 position, Vector3 normal, Vector2 uv);
  }
}
