// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.MyMathHelper
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.Engine.Core
{
  public static class MyMathHelper
  {
    private static Random rng = new Random();

    public static Vector3 GetNewVelocity(Vector3 from, Vector3 to, Vector3 speed)
    {
      Vector3 vector3 = to - from;
      vector3.Normalize();
      return vector3 * speed;
    }

    public static int Clamp(int value, int min, int max)
    {
      if (value < min)
        value = min;
      else if (value > max)
        value = max;
      return value;
    }

    public static bool IsInsideThreshold(float value, float target, float threshhold)
    {
      if ((double) value >= (double) target - (double) threshhold)
        return (double) value <= (double) target + (double) threshhold;
      return false;
    }

    public static float GetAngle(Vector2 v1, Vector2 v2)
    {
      float num = v2.X - v1.X;
      return MyMathHelper.WrapAngle((float) Math.Atan2((double) (v2.Y - v1.Y), (double) num));
    }

    public static double GetSignedAngleBetween2DVectors(Vector3 fromVector, Vector3 destVector)
    {
      Vector3 vector1 = destVector - fromVector;
      vector1.Normalize();
      Vector3 destVectorsRight = Vector3.Cross(vector1, Vector3.Right);
      return MyMathHelper.GetSignedAngleBetween2DVectors(fromVector, destVector, destVectorsRight);
    }

    public static double GetSignedAngleBetween2DVectors(
      Vector3 fromVector,
      Vector3 destVector,
      Vector3 destVectorsRight)
    {
      fromVector.Normalize();
      destVector.Normalize();
      destVectorsRight.Normalize();
      float num1 = Vector3.Dot(fromVector, destVector);
      float num2 = Vector3.Dot(fromVector, destVectorsRight);
      double num3 = Math.Acos((double) MathHelper.Clamp(num1, -1f, 1f));
      if ((double) num2 < 0.0)
        num3 = -num3;
      if ((double) destVector.X > 0.0 && num3 > 0.0)
        num3 = -num3;
      return num3;
    }

    public static double GetSignedAngleBetween2DVectors(Vector2 fromVector, Vector2 destVector)
    {
      return MyMathHelper.GetSignedAngleBetween2DVectors(MyMathHelper.Convert3D(fromVector), MyMathHelper.Convert3D(destVector));
    }

    public static float TurnToFace(
      Vector2 fromVector,
      Vector2 destVector,
      float currentAngle,
      float turnSpeed)
    {
      float between2Dvectors = (float) MyMathHelper.GetSignedAngleBetween2DVectors(MyMathHelper.Convert3D(fromVector), MyMathHelper.Convert3D(destVector));
      float num = MathHelper.Clamp(between2Dvectors - currentAngle, -turnSpeed, turnSpeed);
      return MyMathHelper.WrapAngle((double) Math.Abs(between2Dvectors - currentAngle) > 3.14159274101257 ? currentAngle - num : currentAngle + num);
    }

    public static float TurnToFace(float currentAngle, float targetAngle, float turnSpeed)
    {
      float num = MathHelper.Clamp(targetAngle - currentAngle, -turnSpeed, turnSpeed);
      return MyMathHelper.WrapAngle((double) Math.Abs(targetAngle - currentAngle) > 3.14159274101257 ? currentAngle - num : currentAngle + num);
    }

    public static Vector2 Convert2D(Vector3 v)
    {
      return new Vector2(v.X, v.Z);
    }

    public static Vector3 Convert3D(Vector2 v)
    {
      return new Vector3(v.X, 0.0f, v.Y);
    }

    public static float TurnToFace2(
      Vector2 position,
      Vector2 faceThis,
      float currentAngle,
      float turnSpeed)
    {
      float num1 = faceThis.X - position.X;
      float num2 = MathHelper.Clamp(MyMathHelper.WrapAngle((float) Math.Atan2((double) (faceThis.Y - position.Y), (double) num1) - currentAngle), -turnSpeed, turnSpeed);
      return MyMathHelper.WrapAngle(currentAngle + num2);
    }

    public static float WrapAngle(float radians)
    {
      while ((double) radians < -3.14159274101257)
        radians += 6.283185f;
      while ((double) radians > 3.14159274101257)
        radians -= 6.283185f;
      return radians;
    }

    public static Vector2 RotateVector2ByAngle(Vector2 v, float angle)
    {
      Vector3 vector3 = Vector3.Transform(new Vector3(v.X, 0.0f, v.Y), Matrix.CreateRotationY(angle));
      return new Vector2(vector3.X, vector3.Z);
    }

    public static Vector3 RotateVector3ByAngle(Vector3 v, float angle)
    {
      return Vector3.Transform(v, Matrix.CreateRotationY(angle));
    }

    public static float Interpolate(float alpha, float x0, float x1)
    {
      return x0 + (x1 - x0) * alpha;
    }

    public static Vector3 Interpolate(float alpha, Vector3 x0, Vector3 x1)
    {
      return x0 + (x1 - x0) * alpha;
    }

    public static float Random()
    {
      return (float) MyMathHelper.rng.NextDouble();
    }

    public static float Random(float lowerBound, float upperBound)
    {
      return lowerBound + MyMathHelper.Random() * (upperBound - lowerBound);
    }

    public static float Clip(float x, float min, float max)
    {
      if ((double) x < (double) min)
        return min;
      if ((double) x > (double) max)
        return max;
      return x;
    }

    public static float RemapInterval(float x, float in0, float in1, float out0, float out1)
    {
      return MyMathHelper.Interpolate((float) (((double) x - (double) in0) / ((double) in1 - (double) in0)), out0, out1);
    }

    public static float RemapIntervalClip(float x, float in0, float in1, float out0, float out1)
    {
      return MyMathHelper.Interpolate(MyMathHelper.Clip((float) (((double) x - (double) in0) / ((double) in1 - (double) in0)), 0.0f, 1f), out0, out1);
    }

    public static int IntervalComparison(float x, float lowerBound, float upperBound)
    {
      if ((double) x < (double) lowerBound)
        return -1;
      return (double) x > (double) upperBound ? 1 : 0;
    }

    public static float ScalarRandomWalk(float initial, float walkSpeed, float min, float max)
    {
      float num = initial + (float) ((double) MyMathHelper.Random() * 2.0 - 1.0) * walkSpeed;
      if ((double) num < (double) min)
        return min;
      if ((double) num > (double) max)
        return max;
      return num;
    }

    public static float Square(float x)
    {
      return x * x;
    }

    public static void BlendIntoAccumulator(
      float smoothRate,
      float newValue,
      ref float smoothedAccumulator)
    {
      smoothedAccumulator = MyMathHelper.Interpolate(MyMathHelper.Clip(smoothRate, 0.0f, 1f), smoothedAccumulator, newValue);
    }

    public static void BlendIntoAccumulator(
      float smoothRate,
      Vector3 newValue,
      ref Vector3 smoothedAccumulator)
    {
      smoothedAccumulator = MyMathHelper.Interpolate(MyMathHelper.Clip(smoothRate, 0.0f, 1f), smoothedAccumulator, newValue);
    }

    public static Vector3 GetVectorFromAngle(int slice, int totalSlices)
    {
      float num = (float) slice * 6.283185f / (float) totalSlices;
      return new Vector3((float) Math.Cos((double) num), 0.0f, (float) Math.Sin((double) num));
    }

    public static Vector2 TransformToCircle(Vector2 position, float worldWidth)
    {
      float num = (float) (6.28318548202515 * ((double) (position.X - worldWidth * 0.25f) / (double) worldWidth));
      return new Vector2((float) Math.Cos((double) num) * position.Y, (float) Math.Sin((double) num) * position.Y);
    }
  }
}
