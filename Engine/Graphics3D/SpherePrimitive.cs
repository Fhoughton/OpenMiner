// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.SpherePrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class SpherePrimitive : GeometricPrimitive<VertexPositionNormal>
  {
    public SpherePrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f, 16)
    {
    }

    public SpherePrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float diameter,
      int tessellation)
      : base(effect)
    {
      if (tessellation < 3)
        throw new ArgumentOutOfRangeException(nameof (tessellation));
      int num1 = tessellation;
      int num2 = tessellation * 2;
      float num3 = diameter / 2f;
      this.AddVertex(Vector3.Down * num3, Vector3.Down);
      for (int index1 = 0; index1 < num1 - 1; ++index1)
      {
        float num4 = (float) ((double) (index1 + 1) * 3.14159274101257 / (double) num1 - 1.57079637050629);
        float y = (float) Math.Sin((double) num4);
        float num5 = (float) Math.Cos((double) num4);
        for (int index2 = 0; index2 < num2; ++index2)
        {
          float num6 = (float) index2 * 6.283185f / (float) num2;
          float x = (float) Math.Cos((double) num6) * num5;
          float z = (float) Math.Sin((double) num6) * num5;
          Vector3 normal = new Vector3(x, y, z);
          this.AddVertex(normal * num3, normal);
        }
      }
      this.AddVertex(Vector3.Up * num3, Vector3.Up);
      for (int index = 0; index < num2; ++index)
      {
        this.AddIndex(0);
        this.AddIndex(1 + (index + 1) % num2);
        this.AddIndex(1 + index);
      }
      for (int index1 = 0; index1 < num1 - 2; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          int num4 = index1 + 1;
          int num5 = (index2 + 1) % num2;
          this.AddIndex(1 + index1 * num2 + index2);
          this.AddIndex(1 + index1 * num2 + num5);
          this.AddIndex(1 + num4 * num2 + index2);
          this.AddIndex(1 + index1 * num2 + num5);
          this.AddIndex(1 + num4 * num2 + num5);
          this.AddIndex(1 + num4 * num2 + index2);
        }
      }
      for (int index = 0; index < num2; ++index)
      {
        this.AddIndex(this.CurrentVertex - 1);
        this.AddIndex(this.CurrentVertex - 2 - (index + 1) % num2);
        this.AddIndex(this.CurrentVertex - 2 - index);
      }
      this.InitializePrimitive(graphicsDevice);
    }

    private void AddVertex(Vector3 position, Vector3 normal)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
