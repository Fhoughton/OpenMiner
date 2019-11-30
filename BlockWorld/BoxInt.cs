// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BoxInt
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace StudioForge.BlockWorld
{
  [DebuggerDisplay("Min={Min}, Max={Max}")]
  public struct BoxInt
  {
    private static Vector3[] corners = new Vector3[8];
    public GlobalPoint3D Min;
    public GlobalPoint3D Max;

    public BoxInt(BoundingFrustum frustum)
    {
      this.Min = new GlobalPoint3D((int) ushort.MaxValue, (int) ushort.MaxValue, (int) ushort.MaxValue);
      this.Max = GlobalPoint3D.Zero;
      lock (BoxInt.corners)
      {
        frustum.GetCorners(BoxInt.corners);
        foreach (Vector3 corner in BoxInt.corners)
        {
          if ((double) corner.X < (double) this.Min.X)
            this.Min.X = (int) corner.X;
          if ((double) corner.X > (double) this.Max.X)
            this.Max.X = (int) corner.X;
          if ((double) corner.Y < (double) this.Min.Y)
            this.Min.Y = (int) corner.Y;
          if ((double) corner.Y > (double) this.Max.Y)
            this.Max.Y = (int) corner.Y;
          if ((double) corner.Z < (double) this.Min.Z)
            this.Min.Z = (int) corner.Z;
          if ((double) corner.Z > (double) this.Max.Z)
            this.Max.Z = (int) corner.Z;
        }
      }
    }

    public void Clamp(BoxInt bound)
    {
      if (this.Min.X < bound.Min.X)
        this.Min.X = bound.Min.X;
      if (this.Min.Y < bound.Min.Y)
        this.Min.Y = bound.Min.Y;
      if (this.Min.Z < bound.Min.Z)
        this.Min.Z = bound.Min.Z;
      if (this.Max.X > bound.Max.X)
        this.Max.X = bound.Max.X;
      if (this.Max.Y > bound.Max.Y)
        this.Max.Y = bound.Max.Y;
      if (this.Max.Z <= bound.Max.Z)
        return;
      this.Max.Z = bound.Max.Z;
    }

    public bool Intersects(BoxInt bound)
    {
      if (this.Min.X < bound.Max.X && this.Max.X > bound.Min.X && (this.Min.Y < bound.Max.Y && this.Max.Y > bound.Min.Y) && this.Min.Z < bound.Max.Z)
        return this.Max.Z > bound.Min.Z;
      return false;
    }

    public bool Contains(GlobalPoint3D p)
    {
      if (p.X >= this.Min.X && p.X < this.Max.X && (p.Y >= this.Min.Y && p.Y < this.Max.Y) && p.Z >= this.Min.Z)
        return p.Z < this.Max.Z;
      return false;
    }

    public GlobalPoint3D Center
    {
      get
      {
        return new GlobalPoint3D((this.Max.X - this.Min.X) / 2 + this.Min.X, (this.Max.Y - this.Min.Y) / 2 + this.Min.Y, (this.Max.Z - this.Min.Z) / 2 + this.Min.Z);
      }
    }
  }
}
