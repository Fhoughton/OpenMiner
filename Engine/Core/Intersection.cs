// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Intersection
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.Engine.Core
{
  public static class Intersection
  {
    private static float cosAngle = (float) Math.Cos(Math.PI / 4.0);
    private static float lastViewAngle = 0.7853982f;
    public static BoundingFrustum FrustumUsedForLastConeCheck = new BoundingFrustum(Matrix.Identity);

    public static Vector3 GetIntersection(
      Vector3 positionA,
      Vector3 velocityA,
      Vector3 positionB,
      float speedB,
      out Vector3 velocityB)
    {
      Vector2 velocityB1;
      Vector2 intersection = Intersection.GetIntersection(new Vector2(positionA.X, positionA.Z), new Vector2(velocityA.X, velocityA.Z), new Vector2(positionB.X, positionB.Z), speedB, out velocityB1);
      velocityB = Vector3.Zero;
      velocityB.X = velocityB1.X;
      velocityB.Z = velocityB1.Y;
      return new Vector3(intersection.X, 0.0f, intersection.Y);
    }

    public static Vector2 GetIntersection(
      Vector2 positionA,
      Vector2 velocityA,
      Vector2 positionB,
      float speedB,
      out Vector2 velocityB)
    {
      float num1 = positionA.X - positionB.X;
      float num2 = positionA.Y - positionB.Y;
      float num3 = Vector2.Subtract(positionA, positionB).LengthSquared();
      float num4 = Math.Abs((float) ((-((double) num1 * (double) velocityA.X + (double) num2 * (double) velocityA.Y) + Math.Sqrt(2.0 * (double) num1 * (double) num2 * (double) velocityA.X * (double) velocityA.Y + (double) num3 * Math.Pow((double) speedB, 2.0) - Math.Pow((double) velocityA.X * (double) num2, 2.0) - Math.Pow((double) velocityA.Y * (double) num1, 2.0))) / ((double) velocityA.LengthSquared() - Math.Pow((double) speedB, 2.0))));
      Vector2 vector2 = Vector2.Add(positionA, Vector2.Multiply(velocityA, num4));
      velocityB = Vector2.Divide(Vector2.Subtract(vector2, positionB), num4);
      if (float.IsNaN(vector2.X) || float.IsNaN(vector2.Y))
        throw new BadValueException();
      return vector2;
    }

    public static Vector2[] IntersectionPoint(
      Vector2 startPos,
      Vector2 endPos,
      float radius)
    {
      Vector2 zero = Vector2.Zero;
      double num1 = ((double) endPos.X - (double) startPos.X) * ((double) endPos.X - (double) startPos.X) + ((double) endPos.Y - (double) startPos.Y) * ((double) endPos.Y - (double) startPos.Y);
      double num2 = 2.0 * (((double) endPos.X - (double) startPos.X) * ((double) startPos.X - (double) zero.X) + ((double) endPos.Y - (double) startPos.Y) * ((double) startPos.Y - (double) zero.Y));
      double num3 = (double) zero.X * (double) zero.X + (double) zero.Y * (double) zero.Y + (double) startPos.X * (double) startPos.X + (double) startPos.Y * (double) startPos.Y - 2.0 * ((double) zero.X * (double) startPos.X + (double) zero.Y * (double) startPos.Y) - (double) radius * (double) radius;
      double num4 = num2 * num2 - 4.0 * num1 * num3;
      if (num4 == 0.0)
      {
        Vector2[] vector2Array = new Vector2[1];
        double num5 = -num2 / (2.0 * num1);
        vector2Array[0] = new Vector2(startPos.X + (float) (num5 * ((double) endPos.X - (double) startPos.X)), startPos.Y + (float) (num5 * ((double) endPos.Y - (double) startPos.Y)));
        return vector2Array;
      }
      if (num4 <= 0.0)
        return new Vector2[0];
      Vector2[] vector2Array1 = new Vector2[2];
      double num6 = (-num2 + Math.Sqrt(num2 * num2 - 4.0 * num1 * num3)) / (2.0 * num1);
      vector2Array1[0] = new Vector2(startPos.X + (float) (num6 * ((double) endPos.X - (double) startPos.X)), startPos.Y + (float) (num6 * ((double) endPos.Y - (double) startPos.Y)));
      double num7 = (-num2 - Math.Sqrt(num2 * num2 - 4.0 * num1 * num3)) / (2.0 * num1);
      vector2Array1[1] = new Vector2(startPos.X + (float) (num7 * ((double) endPos.X - (double) startPos.X)), startPos.Y + (float) (num7 * ((double) endPos.Y - (double) startPos.Y)));
      return vector2Array1;
    }

    public static Vector2? IntersectionPoint(Vector4 firstLine, Vector4 secondLine)
    {
      double num1 = (((double) secondLine.Z - (double) secondLine.X) * ((double) firstLine.Y - (double) secondLine.Y) - ((double) secondLine.W - (double) secondLine.Y) * ((double) firstLine.X - (double) secondLine.X)) / (((double) secondLine.W - (double) secondLine.Y) * ((double) firstLine.Z - (double) firstLine.X) - ((double) secondLine.Z - (double) secondLine.X) * ((double) firstLine.W - (double) firstLine.Y));
      double num2 = (((double) firstLine.Z - (double) firstLine.X) * ((double) firstLine.Y - (double) secondLine.Y) - ((double) firstLine.W - (double) firstLine.Y) * ((double) firstLine.X - (double) secondLine.X)) / (((double) secondLine.W - (double) secondLine.Y) * ((double) firstLine.Z - (double) firstLine.X) - ((double) secondLine.Z - (double) secondLine.X) * ((double) firstLine.W - (double) firstLine.Y));
      if (num1 >= 0.0 && num1 <= 1.0 && (num2 >= 0.0 && num2 <= 1.0))
        return new Vector2?(new Vector2(firstLine.X + (float) (num1 * ((double) firstLine.Z - (double) firstLine.X)), firstLine.Y + (float) (num1 * ((double) firstLine.W - (double) firstLine.Y))));
      return new Vector2?();
    }

    public static float? IntersectionPoint(Vector4 line, float x)
    {
      float y = line.Y;
      int num = (int) ((double) line.Z - (double) line.X);
      for (int x1 = (int) line.X; (double) x1 <= (double) line.X + (double) num; ++x1)
      {
        if ((double) x1 == (double) x)
          return new float?(y);
        y += (line.W - line.Y) / (float) num;
      }
      return new float?();
    }

    public static float DistanceFromPointToLineSegment(Vector3 point, Vector4 segment)
    {
      return Intersection.DistanceFromPointToLineSegment(point, new Vector3(segment.X, segment.Y, 0.0f), new Vector3(segment.Z, segment.W, 0.0f));
    }

    public static float DistanceFromPointToLineSegment(Vector3 point, Vector3 anchor, Vector3 end)
    {
      Vector3 vector2 = end - anchor;
      float num1 = vector2.Length();
      if (vector2 == Vector3.Zero)
        return (point - anchor).Length();
      vector2.Normalize();
      float num2 = Vector3.Dot(point - anchor, vector2);
      if ((double) num2 < 0.0)
        return (point - anchor).Length();
      if ((double) num2 > (double) num1)
        return (point - end).Length();
      return (point - (anchor + vector2 * num2)).Length();
    }

    public static bool IsInsideCone(
      Vector2 point,
      Vector2 coneOrigin,
      Vector2 coneDir,
      float viewAngle)
    {
      if ((double) viewAngle != (double) Intersection.lastViewAngle)
      {
        Intersection.cosAngle = (float) Math.Cos((double) viewAngle);
        Intersection.lastViewAngle = viewAngle;
      }
      Vector2 vector2 = Vector2.Normalize(point - coneOrigin);
      return (double) Vector2.Dot(Vector2.Normalize(coneDir), vector2) > (double) Intersection.cosAngle;
    }

    public static bool IsInsideCone2(
      Vector2 point,
      Vector2 coneOrigin,
      Vector2 coneDir,
      float coneAngle)
    {
      return (double) Math.Abs((float) MyMathHelper.GetSignedAngleBetween2DVectors(coneDir - coneOrigin, point - coneOrigin)) < (double) coneAngle;
    }

    public static bool IsInsideCone(
      BoundingSphere sphere,
      Vector3 coneOrigin,
      BoundingSphere coneEnd)
    {
      float num = Vector3.DistanceSquared(coneEnd.Center, coneOrigin);
      if ((double) Vector3.DistanceSquared(sphere.Center, coneOrigin) > (double) num)
        return false;
      float fieldOfView = 0.1f;
      Matrix matrix = Matrix.CreateLookAt(coneOrigin, coneEnd.Center, Vector3.Up) * Matrix.CreatePerspectiveFieldOfView(fieldOfView, 1f, 0.1f, MathHelper.Max(1f, (float) Math.Sqrt((double) num)));
      Intersection.FrustumUsedForLastConeCheck.Matrix = matrix;
      return Intersection.FrustumUsedForLastConeCheck.Intersects(sphere);
    }

    public static float? RayIntersectsTriangle(
      ref Ray ray,
      ref Vector3 vertex1,
      ref Vector3 vertex2,
      ref Vector3 vertex3)
    {
      Vector3 result1;
      Vector3.Subtract(ref vertex2, ref vertex1, out result1);
      Vector3 result2;
      Vector3.Subtract(ref vertex3, ref vertex1, out result2);
      Vector3 result3;
      Vector3.Cross(ref ray.Direction, ref result2, out result3);
      float result4;
      Vector3.Dot(ref result1, ref result3, out result4);
      if ((double) result4 > -1.40129846432482E-45 && (double) result4 < 1.40129846432482E-45)
        return new float?();
      float num1 = 1f / result4;
      Vector3 result5;
      Vector3.Subtract(ref ray.Position, ref vertex1, out result5);
      float result6;
      Vector3.Dot(ref result5, ref result3, out result6);
      float num2 = result6 * num1;
      if ((double) num2 < 0.0 || (double) num2 > 1.0)
        return new float?();
      Vector3 result7;
      Vector3.Cross(ref result5, ref result1, out result7);
      float result8;
      Vector3.Dot(ref ray.Direction, ref result7, out result8);
      result8 *= num1;
      if ((double) result8 < 0.0 || (double) num2 + (double) result8 > 1.0)
        return new float?();
      float result9;
      Vector3.Dot(ref result2, ref result7, out result9);
      float num3 = result9 * num1;
      if ((double) num3 < 0.0)
        return new float?();
      return new float?(num3);
    }
  }
}
