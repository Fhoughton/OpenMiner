// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.AABBox
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Core
{
  public class AABBox
  {
    private Vector3 min;
    private Vector3 max;

    public AABBox(Vector3 min, Vector3 max)
    {
      this.min = min;
      this.max = max;
    }

    public bool IsInsideFullX(Vector3 p, float radius)
    {
      if ((double) p.X - (double) radius > (double) this.min.X)
        return (double) p.X + (double) radius < (double) this.max.X;
      return false;
    }

    public bool IsInsideFullZ(Vector3 p, float radius)
    {
      if ((double) p.Z - (double) radius > (double) this.min.Z)
        return (double) p.Z + (double) radius < (double) this.max.Z;
      return false;
    }

    public bool IsInsideFull(Vector3 p, float radius)
    {
      if (this.IsInsideFullX(p, radius))
        return this.IsInsideFullZ(p, radius);
      return false;
    }

    public bool IsInsideX(Vector3 p, float radius)
    {
      if ((double) p.X + (double) radius >= (double) this.min.X)
        return (double) p.X - (double) radius <= (double) this.max.X;
      return false;
    }

    public bool IsInsideZ(Vector3 p, float radius)
    {
      if ((double) p.Z + (double) radius >= (double) this.min.Z)
        return (double) p.Z - (double) radius <= (double) this.max.Z;
      return false;
    }

    public bool IsInside(Vector3 p, float radius)
    {
      if (this.IsInsideX(p, radius))
        return this.IsInsideZ(p, radius);
      return false;
    }

    public Vector3 Min
    {
      get
      {
        return this.min;
      }
    }

    public Vector3 Max
    {
      get
      {
        return this.max;
      }
    }

    public Vector3 Center
    {
      get
      {
        return new Vector3(this.min.X + (float) (((double) this.max.X - (double) this.min.X) * 0.5), this.min.Y + (float) (((double) this.max.Y - (double) this.min.Y) * 0.5), this.min.Z + (float) (((double) this.max.Z - (double) this.min.Z) * 0.5));
      }
    }

    public BoundingBox ToBoundingBox()
    {
      return new BoundingBox(this.min, this.max);
    }
  }
}
