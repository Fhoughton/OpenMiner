// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.BaseQuadWithHolePrimitive`1
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public abstract class BaseQuadWithHolePrimitive<V> : GeometricPrimitive<V> where V : struct
  {
    public BaseQuadWithHolePrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f, 0.5f)
    {
    }

    public BaseQuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float size,
      float holeSize)
      : this(graphicsDevice, effect, new Vector2(size), new Vector2(holeSize), Vector2.One)
    {
    }

    public BaseQuadWithHolePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector2 size,
      Vector2 holeSize,
      Vector2 texCoord)
      : base(effect)
    {
      float x1 = holeSize.X;
      float y1 = holeSize.Y;
      float x2 = texCoord.X * ((float) (((double) size.X - (double) holeSize.X) / 2.0) / size.X);
      float y2 = texCoord.Y * ((float) (((double) size.Y - (double) holeSize.Y) / 2.0) / size.Y);
      Vector2[] vector2Array = new Vector2[16]
      {
        new Vector2(0.0f, texCoord.Y),
        new Vector2(0.0f, texCoord.Y - y2),
        new Vector2(x2, texCoord.Y - y2),
        new Vector2(x2, texCoord.Y),
        new Vector2(0.0f, y2),
        new Vector2(0.0f, 0.0f),
        new Vector2(x2, 0.0f),
        new Vector2(x2, y2),
        new Vector2(texCoord.X - x2, y2),
        new Vector2(texCoord.X - x2, 0.0f),
        new Vector2(texCoord.X, 0.0f),
        new Vector2(texCoord.X, y2),
        new Vector2(texCoord.X - x2, texCoord.Y),
        new Vector2(texCoord.X - x2, texCoord.Y - y2),
        new Vector2(texCoord.X, texCoord.Y - y2),
        new Vector2(texCoord.X, texCoord.Y)
      };
      Vector3 normal = new Vector3(0.0f, 1f, 0.0f);
      this.AddIndex(0);
      this.AddIndex(1);
      this.AddIndex(2);
      this.AddIndex(0);
      this.AddIndex(2);
      this.AddIndex(3);
      this.AddIndex(1);
      this.AddIndex(4);
      this.AddIndex(7);
      this.AddIndex(1);
      this.AddIndex(7);
      this.AddIndex(2);
      this.AddIndex(4);
      this.AddIndex(5);
      this.AddIndex(6);
      this.AddIndex(4);
      this.AddIndex(6);
      this.AddIndex(7);
      this.AddIndex(7);
      this.AddIndex(6);
      this.AddIndex(9);
      this.AddIndex(7);
      this.AddIndex(9);
      this.AddIndex(8);
      this.AddIndex(8);
      this.AddIndex(9);
      this.AddIndex(10);
      this.AddIndex(8);
      this.AddIndex(10);
      this.AddIndex(11);
      this.AddIndex(13);
      this.AddIndex(8);
      this.AddIndex(11);
      this.AddIndex(13);
      this.AddIndex(11);
      this.AddIndex(14);
      this.AddIndex(12);
      this.AddIndex(13);
      this.AddIndex(14);
      this.AddIndex(12);
      this.AddIndex(14);
      this.AddIndex(15);
      this.AddIndex(3);
      this.AddIndex(2);
      this.AddIndex(13);
      this.AddIndex(3);
      this.AddIndex(13);
      this.AddIndex(12);
      this.AddVertex(new Vector3(-size.X, 0.0f, size.Y), normal, vector2Array[0]);
      this.AddVertex(new Vector3(-size.X, 0.0f, y1), normal, vector2Array[1]);
      this.AddVertex(new Vector3(-x1, 0.0f, y1), normal, vector2Array[2]);
      this.AddVertex(new Vector3(-x1, 0.0f, size.Y), normal, vector2Array[3]);
      this.AddVertex(new Vector3(-size.X, 0.0f, -y1), normal, vector2Array[4]);
      this.AddVertex(new Vector3(-size.X, 0.0f, -size.Y), normal, vector2Array[5]);
      this.AddVertex(new Vector3(-x1, 0.0f, -size.Y), normal, vector2Array[6]);
      this.AddVertex(new Vector3(-x1, 0.0f, -y1), normal, vector2Array[7]);
      this.AddVertex(new Vector3(x1, 0.0f, -y1), normal, vector2Array[8]);
      this.AddVertex(new Vector3(x1, 0.0f, -size.Y), normal, vector2Array[9]);
      this.AddVertex(new Vector3(size.X, 0.0f, -size.Y), normal, vector2Array[10]);
      this.AddVertex(new Vector3(size.X, 0.0f, -y1), normal, vector2Array[11]);
      this.AddVertex(new Vector3(x1, 0.0f, size.Y), normal, vector2Array[12]);
      this.AddVertex(new Vector3(x1, 0.0f, y1), normal, vector2Array[13]);
      this.AddVertex(new Vector3(size.X, 0.0f, y1), normal, vector2Array[14]);
      this.AddVertex(new Vector3(size.X, 0.0f, size.Y), normal, vector2Array[15]);
      this.InitializePrimitive(graphicsDevice);
    }

    protected abstract void AddVertex(Vector3 position, Vector3 normal, Vector2 uv);
  }
}
