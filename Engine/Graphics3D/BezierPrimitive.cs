// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.BezierPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public abstract class BezierPrimitive : GeometricPrimitive<VertexPositionNormal>
  {
    public BezierPrimitive()
      : base((BasicEffect) null)
    {
    }

    public BezierPrimitive(BasicEffect effect)
      : base(effect)
    {
    }

    protected void CreatePatchIndices(int tessellation, bool isMirrored)
    {
      int num1 = tessellation + 1;
      for (int index1 = 0; index1 < tessellation; ++index1)
      {
        for (int index2 = 0; index2 < tessellation; ++index2)
        {
          int[] numArray = new int[6]
          {
            index1 * num1 + index2,
            (index1 + 1) * num1 + index2,
            (index1 + 1) * num1 + index2 + 1,
            index1 * num1 + index2,
            (index1 + 1) * num1 + index2 + 1,
            index1 * num1 + index2 + 1
          };
          if (isMirrored)
            Array.Reverse((Array) numArray);
          foreach (int num2 in numArray)
            this.AddIndex(this.CurrentVertex + num2);
        }
      }
    }

    protected void CreatePatchVertices(Vector3[] patch, int tessellation, bool isMirrored)
    {
      for (int index1 = 0; index1 <= tessellation; ++index1)
      {
        float t1 = (float) index1 / (float) tessellation;
        for (int index2 = 0; index2 <= tessellation; ++index2)
        {
          float t2 = (float) index2 / (float) tessellation;
          Vector3 p1_1 = BezierPrimitive.Bezier(patch[0], patch[1], patch[2], patch[3], t1);
          Vector3 p2_1 = BezierPrimitive.Bezier(patch[4], patch[5], patch[6], patch[7], t1);
          Vector3 p3_1 = BezierPrimitive.Bezier(patch[8], patch[9], patch[10], patch[11], t1);
          Vector3 p4_1 = BezierPrimitive.Bezier(patch[12], patch[13], patch[14], patch[15], t1);
          Vector3 position = BezierPrimitive.Bezier(p1_1, p2_1, p3_1, p4_1, t2);
          Vector3 p1_2 = BezierPrimitive.Bezier(patch[0], patch[4], patch[8], patch[12], t2);
          Vector3 p2_2 = BezierPrimitive.Bezier(patch[1], patch[5], patch[9], patch[13], t2);
          Vector3 p3_2 = BezierPrimitive.Bezier(patch[2], patch[6], patch[10], patch[14], t2);
          Vector3 p4_2 = BezierPrimitive.Bezier(patch[3], patch[7], patch[11], patch[15], t2);
          Vector3 normal = Vector3.Cross(BezierPrimitive.BezierTangent(p1_1, p2_1, p3_1, p4_1, t2), BezierPrimitive.BezierTangent(p1_2, p2_2, p3_2, p4_2, t1));
          if ((double) normal.Length() > 9.99999974737875E-05)
          {
            normal.Normalize();
            if (isMirrored)
              normal = -normal;
          }
          else
            normal = (double) position.Y <= 0.0 ? Vector3.Down : Vector3.Up;
          this.AddVertex(position, normal);
        }
      }
    }

    private static float Bezier(float p1, float p2, float p3, float p4, float t)
    {
      return (float) ((double) p1 * (1.0 - (double) t) * (1.0 - (double) t) * (1.0 - (double) t) + (double) p2 * 3.0 * (double) t * (1.0 - (double) t) * (1.0 - (double) t) + (double) p3 * 3.0 * (double) t * (double) t * (1.0 - (double) t) + (double) p4 * (double) t * (double) t * (double) t);
    }

    private static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
      return new Vector3()
      {
        X = BezierPrimitive.Bezier(p1.X, p2.X, p3.X, p4.X, t),
        Y = BezierPrimitive.Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t),
        Z = BezierPrimitive.Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t)
      };
    }

    private static float BezierTangent(float p1, float p2, float p3, float p4, float t)
    {
      return (float) ((double) p1 * (2.0 * (double) t - 1.0 - (double) t * (double) t) + (double) p2 * (1.0 - 4.0 * (double) t + 3.0 * (double) t * (double) t) + (double) p3 * (2.0 * (double) t - 3.0 * (double) t * (double) t) + (double) p4 * ((double) t * (double) t));
    }

    private static Vector3 BezierTangent(
      Vector3 p1,
      Vector3 p2,
      Vector3 p3,
      Vector3 p4,
      float t)
    {
      Vector3 vector3 = new Vector3();
      vector3.X = BezierPrimitive.BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
      vector3.Y = BezierPrimitive.BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
      vector3.Z = BezierPrimitive.BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);
      vector3.Normalize();
      return vector3;
    }

    private void AddVertex(Vector3 position, Vector3 normal)
    {
      this.AddVertex(new VertexPositionNormal(position, normal));
    }
  }
}
