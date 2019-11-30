// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.OctreeLeaf`1
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class OctreeLeaf<T> : OctreeBase<T>
  {
    public const int MaxObjectsPerLeaf = 8;
    public int BoundedObjectCount;
    public T[] BoundedObjects;

    public OctreeLeaf(Octree<T> parent, BoundingBox box)
      : base(parent, box)
    {
      this.BoundedObjects = new T[8];
    }

    protected override void ClearCore()
    {
      this.BoundedObjects = (T[]) null;
    }

    public override bool IsLeaf
    {
      get
      {
        return true;
      }
    }

    public override int LeafCount
    {
      get
      {
        return 1;
      }
    }

    public override int MemorySize
    {
      get
      {
        return base.MemorySize + 4;
      }
    }

    public override OctreeLeaf<T> AddObject(T o, BoundingBox box)
    {
      if (this.Box.Contains(box) != ContainmentType.Contains || this.BoundedObjectCount >= 8)
        return (OctreeLeaf<T>) null;
      this.BoundedObjects[this.BoundedObjectCount++] = o;
      return (OctreeLeaf<T>) null;
    }

    public override int AddLeavesWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<OctreeLeaf<T>> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      if (frustum.FastIntersect(ref box))
        list.Add(this);
      return 1;
    }

    public override int AddObjectsWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<T> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      if (frustum.FastIntersect(ref box))
      {
        foreach (T boundedObject in this.BoundedObjects)
        {
          if ((object) boundedObject != null)
            list.Add(boundedObject);
        }
      }
      return 1;
    }

    public override int AddObjectsInsideSphere(
      BoundingSphere sphere,
      List<T> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      if (sphere.Contains(box) != ContainmentType.Disjoint)
      {
        foreach (T boundedObject in this.BoundedObjects)
        {
          if ((object) boundedObject != null)
            list.Add(boundedObject);
        }
      }
      return 1;
    }
  }
}
