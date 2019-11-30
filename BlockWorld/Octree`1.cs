// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.Octree`1
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class Octree<T> : OctreeBase<T>
  {
    private OctreeBase<T> node1;
    private OctreeBase<T> node2;
    private OctreeBase<T> node3;
    private OctreeBase<T> node4;
    private OctreeBase<T> node5;
    private OctreeBase<T> node6;
    private OctreeBase<T> node7;
    private OctreeBase<T> node8;

    public Octree(Octree<T> parent, BoundingBox box)
      : base(parent, box)
    {
    }

    public void ReplaceLeaf(OctreeLeaf<T> replace, Octree<T> with)
    {
      if (this.node1 == replace)
        this.node1 = (OctreeBase<T>) with;
      else if (this.node2 == replace)
        this.node2 = (OctreeBase<T>) with;
      else if (this.node3 == replace)
        this.node3 = (OctreeBase<T>) with;
      else if (this.node4 == replace)
        this.node4 = (OctreeBase<T>) with;
      else if (this.node5 == replace)
        this.node5 = (OctreeBase<T>) with;
      else if (this.node6 == replace)
        this.node6 = (OctreeBase<T>) with;
      else if (this.node7 == replace)
      {
        this.node7 = (OctreeBase<T>) with;
      }
      else
      {
        if (this.node8 != replace)
          return;
        this.node8 = (OctreeBase<T>) with;
      }
    }

    protected override void ClearCore()
    {
      if (this.node1 != null)
        this.node1.Clear();
      if (this.node2 != null)
        this.node2.Clear();
      if (this.node3 != null)
        this.node3.Clear();
      if (this.node4 != null)
        this.node4.Clear();
      if (this.node5 != null)
        this.node5.Clear();
      if (this.node6 != null)
        this.node6.Clear();
      if (this.node7 != null)
        this.node7.Clear();
      if (this.node8 != null)
        this.node8.Clear();
      this.node1 = (OctreeBase<T>) null;
      this.node2 = (OctreeBase<T>) null;
      this.node3 = (OctreeBase<T>) null;
      this.node4 = (OctreeBase<T>) null;
      this.node5 = (OctreeBase<T>) null;
      this.node6 = (OctreeBase<T>) null;
      this.node7 = (OctreeBase<T>) null;
      this.node8 = (OctreeBase<T>) null;
    }

    public override bool IsLeaf
    {
      get
      {
        return false;
      }
    }

    public override int LeafCount
    {
      get
      {
        int num = 0;
        if (this.node1 != null)
          num += this.node1.LeafCount;
        if (this.node2 != null)
          num += this.node2.LeafCount;
        if (this.node3 != null)
          num += this.node3.LeafCount;
        if (this.node4 != null)
          num += this.node4.LeafCount;
        if (this.node5 != null)
          num += this.node5.LeafCount;
        if (this.node6 != null)
          num += this.node6.LeafCount;
        if (this.node7 != null)
          num += this.node7.LeafCount;
        if (this.node8 != null)
          num += this.node8.LeafCount;
        return num;
      }
    }

    public override int MemorySize
    {
      get
      {
        int memorySize = base.MemorySize;
        if (this.node1 != null)
          memorySize += this.node1.MemorySize;
        if (this.node2 != null)
          memorySize += this.node2.MemorySize;
        if (this.node3 != null)
          memorySize += this.node3.MemorySize;
        if (this.node4 != null)
          memorySize += this.node4.MemorySize;
        if (this.node5 != null)
          memorySize += this.node5.MemorySize;
        if (this.node6 != null)
          memorySize += this.node6.MemorySize;
        if (this.node7 != null)
          memorySize += this.node7.MemorySize;
        if (this.node8 != null)
          memorySize += this.node8.MemorySize;
        return memorySize;
      }
    }

    public override OctreeLeaf<T> AddObject(T o, BoundingBox box)
    {
      OctreeLeaf<T> octreeLeaf = (OctreeLeaf<T>) null;
      if (this.Box.Contains(box) == ContainmentType.Contains && (this.node1 == null || (octreeLeaf = this.node1.AddObject(o, box)) == null) && ((this.node2 == null || (octreeLeaf = this.node2.AddObject(o, box)) == null) && (this.node3 == null || (octreeLeaf = this.node3.AddObject(o, box)) == null)) && ((this.node4 == null || (octreeLeaf = this.node4.AddObject(o, box)) == null) && (this.node5 == null || (octreeLeaf = this.node5.AddObject(o, box)) == null) && ((this.node6 == null || (octreeLeaf = this.node6.AddObject(o, box)) == null) && (this.node7 == null || (octreeLeaf = this.node7.AddObject(o, box)) == null))) && this.node8 != null)
        octreeLeaf = this.node8.AddObject(o, box);
      return octreeLeaf;
    }

    public override int AddLeavesWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<OctreeLeaf<T>> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      int num = 1;
      if (frustum.FastIntersect(ref box))
      {
        if (this.node1 != null)
          num += this.node1.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node2 != null)
          num += this.node2.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node3 != null)
          num += this.node3.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node4 != null)
          num += this.node4.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node5 != null)
          num += this.node5.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node6 != null)
          num += this.node6.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node7 != null)
          num += this.node7.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node8 != null)
          num += this.node8.AddLeavesWithBoxInsideFrustrum(frustum, list, ref offset);
      }
      return num;
    }

    public override int AddObjectsWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<T> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      int num = 1;
      if (frustum.FastIntersect(ref box))
      {
        if (this.node1 != null)
          num += this.node1.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node2 != null)
          num += this.node2.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node3 != null)
          num += this.node3.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node4 != null)
          num += this.node4.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node5 != null)
          num += this.node5.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node6 != null)
          num += this.node6.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node7 != null)
          num += this.node7.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
        if (this.node8 != null)
          num += this.node8.AddObjectsWithBoxInsideFrustrum(frustum, list, ref offset);
      }
      return num;
    }

    public override int AddObjectsInsideSphere(
      BoundingSphere sphere,
      List<T> list,
      ref Vector3 offset)
    {
      BoundingBox box = this.Box;
      box.Min += offset;
      box.Max += offset;
      int num = 1;
      if (sphere.Contains(box) != ContainmentType.Disjoint)
      {
        if (this.node1 != null)
          num += this.node1.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node2 != null)
          num += this.node2.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node3 != null)
          num += this.node3.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node4 != null)
          num += this.node4.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node5 != null)
          num += this.node5.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node6 != null)
          num += this.node6.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node7 != null)
          num += this.node7.AddObjectsInsideSphere(sphere, list, ref offset);
        if (this.node8 != null)
          num += this.node8.AddObjectsInsideSphere(sphere, list, ref offset);
      }
      return num;
    }

    public void Split(Map map, int count)
    {
      float x1 = this.Box.Max.X - this.Box.Min.X;
      float y1 = this.Box.Max.Y - this.Box.Min.Y;
      float z1 = this.Box.Max.Z - this.Box.Min.Z;
      float x2 = x1 * 0.5f;
      float y2 = y1 * 0.5f;
      float z2 = z1 * 0.5f;
      Vector3 vector3 = new Vector3(x2, y2, z2);
      BoundingBox box1 = new BoundingBox(this.Box.Min + new Vector3(0.0f, y2, 0.0f), this.Box.Min + new Vector3(x2, y1, z2));
      BoundingBox box2 = new BoundingBox(this.Box.Min + new Vector3(0.0f, y2, z2), this.Box.Min + new Vector3(x2, y1, z1));
      BoundingBox box3 = new BoundingBox(this.Box.Min + new Vector3(x2, y2, 0.0f), this.Box.Min + new Vector3(x1, y1, z2));
      BoundingBox box4 = new BoundingBox(this.Box.Min + new Vector3(x2, y2, z2), this.Box.Max);
      BoundingBox box5 = new BoundingBox(this.Box.Min, this.Box.Min + vector3);
      BoundingBox box6 = new BoundingBox(this.Box.Min + new Vector3(0.0f, 0.0f, z2), this.Box.Min + new Vector3(x2, y2, z1));
      BoundingBox box7 = new BoundingBox(this.Box.Min + new Vector3(x2, 0.0f, 0.0f), this.Box.Min + new Vector3(x1, y2, z2));
      BoundingBox box8 = new BoundingBox(this.Box.Min + new Vector3(x2, 0.0f, z2), this.Box.Min + new Vector3(x1, y2, z1));
      if (count == 1)
      {
        this.node1 = (OctreeBase<T>) new OctreeLeaf<T>(this, box1);
        this.node2 = (OctreeBase<T>) new OctreeLeaf<T>(this, box2);
        this.node3 = (OctreeBase<T>) new OctreeLeaf<T>(this, box3);
        this.node4 = (OctreeBase<T>) new OctreeLeaf<T>(this, box4);
        this.node5 = (OctreeBase<T>) new OctreeLeaf<T>(this, box5);
        this.node6 = (OctreeBase<T>) new OctreeLeaf<T>(this, box6);
        this.node7 = (OctreeBase<T>) new OctreeLeaf<T>(this, box7);
        this.node8 = (OctreeBase<T>) new OctreeLeaf<T>(this, box8);
      }
      else
      {
        this.node1 = (OctreeBase<T>) new Octree<T>(this, box1);
        this.node2 = (OctreeBase<T>) new Octree<T>(this, box2);
        this.node3 = (OctreeBase<T>) new Octree<T>(this, box3);
        this.node4 = (OctreeBase<T>) new Octree<T>(this, box4);
        this.node5 = (OctreeBase<T>) new Octree<T>(this, box5);
        this.node6 = (OctreeBase<T>) new Octree<T>(this, box6);
        this.node7 = (OctreeBase<T>) new Octree<T>(this, box7);
        this.node8 = (OctreeBase<T>) new Octree<T>(this, box8);
        --count;
        ((Octree<T>) this.node1).Split(map, count);
        ((Octree<T>) this.node2).Split(map, count);
        ((Octree<T>) this.node3).Split(map, count);
        ((Octree<T>) this.node4).Split(map, count);
        ((Octree<T>) this.node5).Split(map, count);
        ((Octree<T>) this.node6).Split(map, count);
        ((Octree<T>) this.node7).Split(map, count);
        ((Octree<T>) this.node8).Split(map, count);
      }
    }
  }
}
