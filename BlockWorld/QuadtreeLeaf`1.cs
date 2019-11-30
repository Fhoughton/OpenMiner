// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.QuadtreeLeaf`1
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class QuadtreeLeaf<T> : QuadtreeBase<T>
  {
    public const int MaxObjectsPerLeaf = 16;
    public int BoundedObjectCount;
    public T[] BoundedObjects;

    public QuadtreeLeaf(Quadtree<T> parent, BoundingBox box)
      : base(parent, box)
    {
      this.BoundedObjects = new T[16];
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

    public override QuadtreeLeaf<T> AddObject(T o, BoundingBox box)
    {
      if (this.Box.Contains(box) != ContainmentType.Contains || this.BoundedObjectCount >= 16)
        return (QuadtreeLeaf<T>) null;
      this.BoundedObjects[this.BoundedObjectCount++] = o;
      return (QuadtreeLeaf<T>) null;
    }

    public override int AddLeavesWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<QuadtreeLeaf<T>> list,
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
        this.AddObjects(list);
      return 1;
    }

    private void AddObjects(List<T> list)
    {
      for (int index = 0; index < this.BoundedObjects.Length; ++index)
      {
        if ((object) this.BoundedObjects[index] != null)
          list.Add(this.BoundedObjects[index]);
      }
    }
  }
}
