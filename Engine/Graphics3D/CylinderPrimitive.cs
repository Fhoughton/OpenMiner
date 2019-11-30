// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.CylinderPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class CylinderPrimitive : GeometricPrimitive<VertexPositionNormalTexture>
  {
    public CylinderPrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, (BasicEffect) null, 1f, 1f, 32, true, true)
    {
    }

    public CylinderPrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float height,
      float diameter,
      int tessellation)
      : this(graphicsDevice, effect, height, diameter, tessellation, true, true)
    {
    }

    public CylinderPrimitive(
      GraphicsDevice graphicsDevice,
      BasicEffect effect,
      float height,
      float diameter,
      int tessellation,
      bool includeTopCap,
      bool includeBottomCap)
      : base(effect)
    {
      if (tessellation < 3)
        throw new ArgumentOutOfRangeException(nameof (tessellation));
      height /= 2f;
      float radius = diameter / 2f;
      for (int i = 0; i < tessellation; ++i)
      {
        Vector3 circleVector = CylinderPrimitive.GetCircleVector(i, tessellation);
        Vector2 tex = new Vector2(circleVector.X, circleVector.Z);
        this.AddVertex(circleVector * radius + Vector3.Up * height, circleVector, tex);
        this.AddVertex(circleVector * radius + Vector3.Down * height, circleVector, tex);
        this.AddIndex(i * 2);
        this.AddIndex(i * 2 + 1);
        this.AddIndex((i * 2 + 2) % (tessellation * 2));
        this.AddIndex(i * 2 + 1);
        this.AddIndex((i * 2 + 3) % (tessellation * 2));
        this.AddIndex((i * 2 + 2) % (tessellation * 2));
      }
      if (includeTopCap)
        this.CreateCap(tessellation, height, radius, Vector3.Up);
      if (includeBottomCap)
        this.CreateCap(tessellation, height, radius, Vector3.Down);
      this.InitializePrimitive(graphicsDevice);
    }

    private void CreateCap(int tessellation, float height, float radius, Vector3 normal)
    {
      for (int index = 0; index < tessellation - 2; ++index)
      {
        if ((double) normal.Y > 0.0)
        {
          this.AddIndex(this.CurrentVertex);
          this.AddIndex(this.CurrentVertex + (index + 1) % tessellation);
          this.AddIndex(this.CurrentVertex + (index + 2) % tessellation);
        }
        else
        {
          this.AddIndex(this.CurrentVertex);
          this.AddIndex(this.CurrentVertex + (index + 2) % tessellation);
          this.AddIndex(this.CurrentVertex + (index + 1) % tessellation);
        }
      }
      for (int i = 0; i < tessellation; ++i)
      {
        Vector3 position = CylinderPrimitive.GetCircleVector(i, tessellation) * radius + normal * height;
        Vector2 tex = new Vector2(position.X / radius, position.Z / radius);
        this.AddVertex(position, normal, tex);
      }
    }

    private static Vector3 GetCircleVector(int i, int tessellation)
    {
      float num = (float) i * 6.283185f / (float) tessellation;
      return new Vector3((float) Math.Cos((double) num), 0.0f, (float) Math.Sin((double) num));
    }

    private void AddVertex(Vector3 position, Vector3 normal, Vector2 tex)
    {
      this.AddVertex(new VertexPositionNormalTexture(position, normal, tex));
    }
  }
}
