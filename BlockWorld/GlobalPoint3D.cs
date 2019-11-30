// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.GlobalPoint3D
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StudioForge.BlockWorld
{
  [DebuggerDisplay("X={X}, Y={Y}, Z={Z}")]
  public struct GlobalPoint3D : IComparable<GlobalPoint3D>, IEquatable<GlobalPoint3D>
  {
    private static GlobalPoint3D forward = new GlobalPoint3D()
    {
      Z = -1
    };
    private static GlobalPoint3D backward = new GlobalPoint3D()
    {
      Z = 1
    };
    private static GlobalPoint3D left = new GlobalPoint3D()
    {
      X = -1
    };
    private static GlobalPoint3D right = new GlobalPoint3D()
    {
      X = 1
    };
    private static GlobalPoint3D up = new GlobalPoint3D()
    {
      Y = 1
    };
    private static GlobalPoint3D down = new GlobalPoint3D()
    {
      Y = -1
    };
    private static GlobalPoint3D zero = new GlobalPoint3D();
    private static GlobalPoint3D one = new GlobalPoint3D(1, 1, 1);
    private static GlobalPoint3D two = new GlobalPoint3D(2, 2, 2);
    private static GlobalPoint3D minValue = new GlobalPoint3D(int.MinValue, int.MinValue, int.MinValue);
    private static GlobalPoint3D maxValue = new GlobalPoint3D(int.MaxValue, int.MaxValue, int.MaxValue);
    public int X;
    public int Y;
    public int Z;

    public GlobalPoint3D(int x, int y, int z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public GlobalPoint3D(Vector3 v)
    {
      this.X = (int) v.X;
      this.Y = (int) v.Y;
      this.Z = (int) v.Z;
    }

    public GlobalPoint3D(GlobalPoint3D p)
    {
      this.X = p.X;
      this.Y = p.Y;
      this.Z = p.Z;
    }

    public static explicit operator Point3D(GlobalPoint3D p)
    {
      return new Point3D() { X = p.X, Y = p.Y, Z = p.Z };
    }

    public override int GetHashCode()
    {
      return ((this.X & 1023) << 20) + ((this.Z & 1023) << 10) + (this.Y & 1023);
    }

    public static GlobalPoint3D FromHashCode(int hash)
    {
      return new GlobalPoint3D()
      {
        X = hash >> 20 & 1023,
        Z = hash >> 10 & 1023,
        Y = hash & 1023
      };
    }

    public static GlobalPoint3D FromHashCodeOld(int hash)
    {
      GlobalPoint3D zero = GlobalPoint3D.zero;
      zero.X = hash >> 21;
      zero.Y = hash & 1023;
      zero.Z = hash >> 10 & 2047;
      return zero;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator +(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      p1.X += p2.X;
      p1.Y += p2.Y;
      p1.Z += p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator +(GlobalPoint3D p1, Point3D p2)
    {
      p1.X += p2.X;
      p1.Y += p2.Y;
      p1.Z += p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator +(GlobalPoint3D p1, int i)
    {
      p1.X += i;
      p1.Y += i;
      p1.Z += i;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator -(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      p1.X -= p2.X;
      p1.Y -= p2.Y;
      p1.Z -= p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator -(GlobalPoint3D p1, Point3D p2)
    {
      p1.X -= p2.X;
      p1.Y -= p2.Y;
      p1.Z -= p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator -(GlobalPoint3D p1, int i)
    {
      p1.X -= i;
      p1.Y -= i;
      p1.Z -= i;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator *(GlobalPoint3D p1, int i)
    {
      p1.X *= i;
      p1.Y *= i;
      p1.Z *= i;
      return p1;
    }

    [DebuggerStepThrough]
    public static Vector3 operator *(GlobalPoint3D p1, Vector3 i)
    {
      i.X *= (float) p1.X;
      i.Y *= (float) p1.Y;
      i.Z *= (float) p1.Z;
      return i;
    }

    [DebuggerStepThrough]
    public static Vector3 operator *(GlobalPoint3D p1, float i)
    {
      return new Vector3()
      {
        X = i * (float) p1.X,
        Y = i * (float) p1.Y,
        Z = i * (float) p1.Z
      };
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D operator /(GlobalPoint3D p1, int i)
    {
      p1.X /= i;
      p1.Y /= i;
      p1.Z /= i;
      return p1;
    }

    public bool Equals(GlobalPoint3D p)
    {
      if (this.X == p.X && this.Y == p.Y)
        return this.Z == p.Z;
      return false;
    }

    public override bool Equals(object obj)
    {
      GlobalPoint3D globalPoint3D = (GlobalPoint3D) obj;
      if (this.X == globalPoint3D.X && this.Y == globalPoint3D.Y)
        return this.Z == globalPoint3D.Z;
      return false;
    }

    public static bool operator ==(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      if (p1.X == p2.X && p1.Y == p2.Y)
        return p1.Z == p2.Z;
      return false;
    }

    public static bool operator !=(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      if (p1.X == p2.X && p1.Y == p2.Y)
        return p1.Z != p2.Z;
      return true;
    }

    public static float Distance(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      return Vector3.Distance(new Vector3((float) p1.X, (float) p1.Y, (float) p1.Z), new Vector3((float) p2.X, (float) p2.Y, (float) p2.Z));
    }

    public static float DistanceSquared(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      return Vector3.DistanceSquared(new Vector3((float) p1.X, (float) p1.Y, (float) p1.Z), new Vector3((float) p2.X, (float) p2.Y, (float) p2.Z));
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D Negate(GlobalPoint3D p)
    {
      p.X = -p.X;
      p.Y = -p.Y;
      p.Z = -p.Z;
      return p;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D Min(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      if (p2.X < p1.X)
        p1.X = p2.X;
      if (p2.Y < p1.Y)
        p1.Y = p2.Y;
      if (p2.Z < p1.Z)
        p1.Z = p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static GlobalPoint3D Max(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      if (p2.X > p1.X)
        p1.X = p2.X;
      if (p2.Y > p1.Y)
        p1.Y = p2.Y;
      if (p2.Z > p1.Z)
        p1.Z = p2.Z;
      return p1;
    }

    public static GlobalPoint3D Forward
    {
      get
      {
        return GlobalPoint3D.forward;
      }
    }

    public static GlobalPoint3D Backward
    {
      get
      {
        return GlobalPoint3D.backward;
      }
    }

    public static GlobalPoint3D Left
    {
      get
      {
        return GlobalPoint3D.left;
      }
    }

    public static GlobalPoint3D Right
    {
      get
      {
        return GlobalPoint3D.right;
      }
    }

    public static GlobalPoint3D Up
    {
      get
      {
        return GlobalPoint3D.up;
      }
    }

    public static GlobalPoint3D Down
    {
      get
      {
        return GlobalPoint3D.down;
      }
    }

    public static GlobalPoint3D Zero
    {
      get
      {
        return GlobalPoint3D.zero;
      }
    }

    public static GlobalPoint3D One
    {
      get
      {
        return GlobalPoint3D.one;
      }
    }

    public static GlobalPoint3D Two
    {
      get
      {
        return GlobalPoint3D.two;
      }
    }

    public static GlobalPoint3D MinValue
    {
      get
      {
        return GlobalPoint3D.minValue;
      }
    }

    public static GlobalPoint3D MaxValue
    {
      get
      {
        return GlobalPoint3D.maxValue;
      }
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetLeft(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X - distance,
        Y = this.Y,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetForward(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X,
        Y = this.Y,
        Z = this.Z - distance
      };
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetRight(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X + distance,
        Y = this.Y,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetBackward(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X,
        Y = this.Y,
        Z = this.Z + distance
      };
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetUp(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X,
        Y = this.Y + distance,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public GlobalPoint3D GetDown(int distance)
    {
      return new GlobalPoint3D()
      {
        X = this.X,
        Y = this.Y - distance,
        Z = this.Z
      };
    }

    public int CompareTo(GlobalPoint3D other)
    {
      if (this.X < other.X)
        return -1;
      if (other.X < this.X)
        return 1;
      if (this.Z < other.Z)
        return -1;
      if (other.Z < this.Z)
        return 1;
      if (this.Y > other.Y)
        return -1;
      return other.Y > this.Y ? 1 : 0;
    }

    public Vector3 ToVector3()
    {
      return new Vector3()
      {
        X = (float) this.X,
        Y = (float) this.Y,
        Z = (float) this.Z
      };
    }

    public void Clamp(GlobalPoint3D min, GlobalPoint3D max)
    {
      if (this.X < min.X)
        this.X = min.X;
      else if (this.X > max.X)
        this.X = max.X;
      if (this.Y < min.Y)
        this.Y = min.Y;
      else if (this.Y > max.Y)
        this.Y = max.Y;
      if (this.Z < min.Z)
      {
        this.Z = min.Z;
      }
      else
      {
        if (this.Z <= max.Z)
          return;
        this.Z = max.Z;
      }
    }

    public static GlobalPoint3D Clamp(
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D p)
    {
      if (p.X < min.X)
        p.X = min.X;
      else if (p.X > max.X)
        p.X = max.X;
      if (p.Y < min.Y)
        p.Y = min.Y;
      else if (p.Y > max.Y)
        p.Y = max.Y;
      if (p.Z < min.Z)
        p.Z = min.Z;
      else if (p.Z > max.Z)
        p.Z = max.Z;
      return p;
    }

    public static GlobalPoint3D GetClosest(List<GlobalPoint3D> list, GlobalPoint3D p)
    {
      GlobalPoint3D globalPoint3D = p;
      float num1 = float.MaxValue;
      foreach (GlobalPoint3D p1 in list)
      {
        float num2 = GlobalPoint3D.DistanceSquared(p1, p);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          globalPoint3D = p1;
        }
      }
      return globalPoint3D;
    }
  }
}
