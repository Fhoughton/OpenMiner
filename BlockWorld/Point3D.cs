// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.Point3D
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace StudioForge.BlockWorld
{
  [DebuggerDisplay("X={X}, Y={Y}, Z={Z}")]
  public struct Point3D : IComparable<Point3D>, IEquatable<Point3D>
  {
    private static Point3D forward = new Point3D()
    {
      Z = -1
    };
    private static Point3D backward = new Point3D()
    {
      Z = 1
    };
    private static Point3D left = new Point3D()
    {
      X = -1
    };
    private static Point3D right = new Point3D()
    {
      X = 1
    };
    private static Point3D up = new Point3D()
    {
      Y = 1
    };
    private static Point3D down = new Point3D()
    {
      Y = -1
    };
    private static Point3D zero = new Point3D();
    private static Point3D one = new Point3D(1, 1, 1);
    private static Point3D invalid = new Point3D(0, -1, 0);
    public int X;
    public int Y;
    public int Z;

    public Point3D(int x, int y, int z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public Point3D(GlobalPoint3D p)
    {
      this.X = p.X;
      this.Y = p.Y;
      this.Z = p.Z;
    }

    public static explicit operator GlobalPoint3D(Point3D p)
    {
      return new GlobalPoint3D()
      {
        X = p.X,
        Y = p.Y,
        Z = p.Z
      };
    }

    public static Point3D FromHash(int hash)
    {
      return new Point3D()
      {
        X = hash >> 20 & 1023,
        Z = hash >> 10 & 1023,
        Y = hash & 1023
      };
    }

    public static Point3D FromHashOld(int hash)
    {
      return new Point3D()
      {
        X = hash >> 20 & 1023,
        Z = hash >> 10 & 1023,
        Y = hash & 1023
      };
    }

    public override int GetHashCode()
    {
      return ((this.X & 1023) << 20) + ((this.Z & 1023) << 10) + (this.Y & 1023);
    }

    [DebuggerStepThrough]
    public static Point3D operator +(Point3D p1, Point3D p2)
    {
      p1.X += p2.X;
      p1.Y += p2.Y;
      p1.Z += p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static Point3D operator -(Point3D p1, Point3D p2)
    {
      p1.X -= p2.X;
      p1.Y -= p2.Y;
      p1.Z -= p2.Z;
      return p1;
    }

    [DebuggerStepThrough]
    public static Point3D operator *(Point3D p1, int i)
    {
      p1.X *= i;
      p1.Y *= i;
      p1.Z *= i;
      return p1;
    }

    [DebuggerStepThrough]
    public static Point3D operator /(Point3D p1, int i)
    {
      p1.X /= i;
      p1.Y /= i;
      p1.Z /= i;
      return p1;
    }

    public bool Equals(Point3D p)
    {
      if (this.X == p.X && this.Y == p.Y)
        return this.Z == p.Z;
      return false;
    }

    public override bool Equals(object obj)
    {
      Point3D point3D = (Point3D) obj;
      if (this.X == point3D.X && this.Y == point3D.Y)
        return this.Z == point3D.Z;
      return false;
    }

    public static bool operator ==(Point3D p1, Point3D p2)
    {
      if (p1.X == p2.X && p1.Y == p2.Y)
        return p1.Z == p2.Z;
      return false;
    }

    public static bool operator !=(Point3D p1, Point3D p2)
    {
      if (p1.X == p2.X && p1.Y == p2.Y)
        return p1.Z != p2.Z;
      return true;
    }

    public static float Distance(Point3D p1, Point3D p2)
    {
      return Vector3.Distance(new Vector3((float) p1.X, (float) p1.Y, (float) p1.Z), new Vector3((float) p2.X, (float) p2.Y, (float) p2.Z));
    }

    public static float DistanceSquared(Point3D p1, Point3D p2)
    {
      return Vector3.DistanceSquared(new Vector3((float) p1.X, (float) p1.Y, (float) p1.Z), new Vector3((float) p2.X, (float) p2.Y, (float) p2.Z));
    }

    [DebuggerStepThrough]
    public static Point3D Negate(Point3D p)
    {
      p.X = -p.X;
      p.Y = -p.Y;
      p.Z = -p.Z;
      return p;
    }

    public static Point3D Forward
    {
      get
      {
        return Point3D.forward;
      }
    }

    public static Point3D Backward
    {
      get
      {
        return Point3D.backward;
      }
    }

    public static Point3D Left
    {
      get
      {
        return Point3D.left;
      }
    }

    public static Point3D Right
    {
      get
      {
        return Point3D.right;
      }
    }

    public static Point3D Up
    {
      get
      {
        return Point3D.up;
      }
    }

    public static Point3D Down
    {
      get
      {
        return Point3D.down;
      }
    }

    public static Point3D Zero
    {
      get
      {
        return Point3D.zero;
      }
    }

    public static Point3D One
    {
      get
      {
        return Point3D.one;
      }
    }

    public static Point3D Invalid
    {
      get
      {
        return Point3D.invalid;
      }
    }

    [DebuggerStepThrough]
    public Point3D GetLeft(int distance)
    {
      return new Point3D()
      {
        X = this.X - distance,
        Y = this.Y,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public Point3D GetRight(int distance)
    {
      return new Point3D()
      {
        X = this.X + distance,
        Y = this.Y,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public Point3D GetUp(int distance)
    {
      return new Point3D()
      {
        X = this.X,
        Y = this.Y - distance,
        Z = this.Z
      };
    }

    [DebuggerStepThrough]
    public Point3D GetDown(int distance)
    {
      return new Point3D()
      {
        X = this.X,
        Y = this.Y + distance,
        Z = this.Z
      };
    }

    public int CompareTo(Point3D other)
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

    public static Point3D Clamp(Point3D min, Point3D max, Point3D p)
    {
      if (p.X < min.X)
        p.X = min.X;
      else if (p.X >= max.X)
        p.X = max.X - 1;
      if (p.Y < min.Y)
        p.Y = min.Y;
      else if (p.Y >= max.Y)
        p.Y = max.Y - 1;
      if (p.Z < min.Z)
        p.Z = min.Z;
      else if (p.Z >= max.Z)
        p.Z = max.Z - 1;
      return p;
    }
  }
}
