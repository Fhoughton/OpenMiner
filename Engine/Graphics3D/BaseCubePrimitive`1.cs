// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.BaseCubePrimitive`1
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public abstract class BaseCubePrimitive<V> : GeometricPrimitive<V> where V : struct
  {
    public BaseCubePrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f)
    {
    }

    public BaseCubePrimitive(GraphicsDevice graphicsDevice, BasicEffect effect, float size)
      : this(graphicsDevice, effect, new Vector3(size), new Vector3(size), Vector3.One)
    {
    }

    public BaseCubePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      Vector3 frontSize,
      Vector3 backSize,
      Vector3 texFac)
      : base(effect)
    {
      Vector2[] vector2Array = new Vector2[12]
      {
        new Vector2(0.0f, texFac.Y),
        new Vector2(0.0f, 0.0f),
        new Vector2(texFac.X, 0.0f),
        new Vector2(texFac.X, texFac.Y),
        new Vector2(0.0f, texFac.Y),
        new Vector2(0.0f, 0.0f),
        new Vector2(texFac.Z, 0.0f),
        new Vector2(texFac.Z, texFac.Y),
        new Vector2(0.0f, texFac.Z),
        new Vector2(0.0f, 0.0f),
        new Vector2(texFac.X, 0.0f),
        new Vector2(texFac.X, texFac.Z)
      };
      Vector3[] vector3Array = new Vector3[6]
      {
        new Vector3(0.0f, 0.0f, 1f),
        new Vector3(0.0f, 0.0f, -1f),
        new Vector3(1f, 0.0f, 0.0f),
        new Vector3(-1f, 0.0f, 0.0f),
        new Vector3(0.0f, 1f, 0.0f),
        new Vector3(0.0f, -1f, 0.0f)
      };
      this.AddIndex(0);
      this.AddIndex(1);
      this.AddIndex(2);
      this.AddIndex(0);
      this.AddIndex(2);
      this.AddIndex(3);
      this.AddVertex(new Vector3(-backSize.X, -backSize.Y, backSize.Z), vector3Array[0], vector2Array[0]);
      this.AddVertex(new Vector3(-backSize.X, backSize.Y, backSize.Z), vector3Array[0], vector2Array[1]);
      this.AddVertex(new Vector3(backSize.X, backSize.Y, backSize.Z), vector3Array[0], vector2Array[2]);
      this.AddVertex(new Vector3(backSize.X, -backSize.Y, backSize.Z), vector3Array[0], vector2Array[3]);
      this.AddIndex(4);
      this.AddIndex(5);
      this.AddIndex(6);
      this.AddIndex(4);
      this.AddIndex(6);
      this.AddIndex(7);
      this.AddVertex(new Vector3(frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[1], vector2Array[0]);
      this.AddVertex(new Vector3(frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[1], vector2Array[1]);
      this.AddVertex(new Vector3(-frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[1], vector2Array[2]);
      this.AddVertex(new Vector3(-frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[1], vector2Array[3]);
      this.AddIndex(8);
      this.AddIndex(9);
      this.AddIndex(10);
      this.AddIndex(8);
      this.AddIndex(10);
      this.AddIndex(11);
      this.AddVertex(new Vector3(backSize.X, -backSize.Y, backSize.Z), vector3Array[2], vector2Array[4]);
      this.AddVertex(new Vector3(backSize.X, backSize.Y, backSize.Z), vector3Array[2], vector2Array[5]);
      this.AddVertex(new Vector3(frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[2], vector2Array[6]);
      this.AddVertex(new Vector3(frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[2], vector2Array[7]);
      this.AddIndex(12);
      this.AddIndex(13);
      this.AddIndex(14);
      this.AddIndex(12);
      this.AddIndex(14);
      this.AddIndex(15);
      this.AddVertex(new Vector3(-frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[3], vector2Array[4]);
      this.AddVertex(new Vector3(-frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[3], vector2Array[5]);
      this.AddVertex(new Vector3(-backSize.X, backSize.Y, backSize.Z), vector3Array[3], vector2Array[6]);
      this.AddVertex(new Vector3(-backSize.X, -backSize.Y, backSize.Z), vector3Array[3], vector2Array[7]);
      this.AddIndex(16);
      this.AddIndex(17);
      this.AddIndex(18);
      this.AddIndex(16);
      this.AddIndex(18);
      this.AddIndex(19);
      this.AddVertex(new Vector3(-backSize.X, backSize.Y, backSize.Z), vector3Array[4], vector2Array[8]);
      this.AddVertex(new Vector3(-frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[4], vector2Array[9]);
      this.AddVertex(new Vector3(frontSize.X, frontSize.Y, -frontSize.Z), vector3Array[4], vector2Array[10]);
      this.AddVertex(new Vector3(backSize.X, backSize.Y, backSize.Z), vector3Array[4], vector2Array[11]);
      this.AddIndex(20);
      this.AddIndex(21);
      this.AddIndex(22);
      this.AddIndex(20);
      this.AddIndex(22);
      this.AddIndex(23);
      this.AddVertex(new Vector3(backSize.X, -backSize.Y, backSize.Z), vector3Array[5], vector2Array[8]);
      this.AddVertex(new Vector3(frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[5], vector2Array[9]);
      this.AddVertex(new Vector3(-frontSize.X, -frontSize.Y, -frontSize.Z), vector3Array[5], vector2Array[10]);
      this.AddVertex(new Vector3(-backSize.X, -backSize.Y, backSize.Z), vector3Array[5], vector2Array[11]);
      this.InitializePrimitive(graphicsDevice);
    }

    protected abstract void AddVertex(Vector3 position, Vector3 normal, Vector2 uv);
  }
}
