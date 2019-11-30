// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.OctreeBase`1
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public abstract class OctreeBase<T>
  {
    public readonly BoundingBox Box;
    public Octree<T> Parent;

    public OctreeBase(Octree<T> parent, BoundingBox box)
    {
      this.Parent = parent;
      this.Box = box;
    }

    public void Clear()
    {
      this.ClearCore();
      this.Parent = (Octree<T>) null;
    }

    protected abstract void ClearCore();

    public abstract bool IsLeaf { get; }

    public virtual int LeafCount
    {
      get
      {
        return 0;
      }
    }

    public virtual int MemorySize
    {
      get
      {
        return 52;
      }
    }

    public abstract OctreeLeaf<T> AddObject(T o, BoundingBox box);

    public int GetLeavesWithBoxInsideFrustum(
      BoundingFrustum frustum,
      List<OctreeLeaf<T>> list,
      Vector3 offset)
    {
      return this.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
    }

    public int GetObjectsWithBoxInsideFrustum(
      BoundingFrustum frustum,
      List<T> list,
      Vector3 offset)
    {
      return this.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
    }

    public int GetObjectsInsideSphere(BoundingSphere sphere, List<T> list, Vector3 offset)
    {
      return this.AddObjectsInsideSphere(sphere, list, ref offset);
    }

    public abstract int AddLeavesWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<OctreeLeaf<T>> list,
      ref Vector3 offset);

    public abstract int AddObjectsWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<T> list,
      ref Vector3 offset);

    public abstract int AddObjectsInsideSphere(
      BoundingSphere sphere,
      List<T> list,
      ref Vector3 offset);
  }
}
