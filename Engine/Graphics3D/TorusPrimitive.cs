// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.TorusPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class TorusPrimitive : GeometricPrimitive<VertexPositionNormal>
  {
    public TorusPrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f, 0.333f, 32)
    {
    }

    public TorusPrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float diameter,
      float thickness,
      int tessellation)
      : base(effect)
    {
      if (tessellation < 3)
        throw new ArgumentOutOfRangeException(nameof (tessellation));
      for (int index1 = 0; index1 < tessellation; ++index1)
      {
        float radians = (float) index1 * 6.283185f / (float) tessellation;
        Matrix matrix = Matrix.CreateTranslation(diameter / 2f, 0.0f, 0.0f) * Matrix.CreateRotationY(radians);
        for (int index2 = 0; index2 < tessellation; ++index2)
        {
          float num1 = (float) index2 * 6.283185f / (float) tessellation;
          Vector3 normal = new Vector3((float) Math.Cos((double) num1), (float) Math.Sin((double) num1), 0.0f);
          Vector3 position = Vector3.Transform(normal * thickness / 2f, matrix);
          normal = Vector3.TransformNormal(normal, matrix);
          this.AddVertex(position, normal);
          int num2 = (index1 + 1) % tessellation;
          int num3 = (index2 + 1) % tessellation;
          this.AddIndex(index1 * tessellation + index2);
          this.AddIndex(index1 * tessellation + num3);
          this.AddIndex(num2 * tessellation + index2);
          this.AddIndex(index1 * tessellation + num3);
          this.AddIndex(num2 * tessellation + num3);
          this.AddIndex(num2 * tessellation + index2);
        }
      }
      this.InitializePrimitive(graphicsDevice);
    }

    private void AddVertex(Vector3 position, Vector3 normal)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
