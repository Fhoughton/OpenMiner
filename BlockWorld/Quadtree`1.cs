// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.Quadtree`1
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class Quadtree<T> : QuadtreeBase<T>
  {
    private QuadtreeBase<T> node1;
    private QuadtreeBase<T> node2;
    private QuadtreeBase<T> node3;
    private QuadtreeBase<T> node4;

    public Quadtree(Quadtree<T> parent, BoundingBox box)
      : base(parent, box)
    {
    }

    public void ReplaceLeaf(QuadtreeLeaf<T> replace, Quadtree<T> with)
    {
      if (this.node1 == replace)
        this.node1 = (QuadtreeBase<T>) with;
      else if (this.node2 == replace)
        this.node2 = (QuadtreeBase<T>) with;
      else if (this.node3 == replace)
      {
        this.node3 = (QuadtreeBase<T>) with;
      }
      else
      {
        if (this.node4 != replace)
          return;
        this.node4 = (QuadtreeBase<T>) with;
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
      this.node1 = (QuadtreeBase<T>) null;
      this.node2 = (QuadtreeBase<T>) null;
      this.node3 = (QuadtreeBase<T>) null;
      this.node4 = (QuadtreeBase<T>) null;
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
        return memorySize;
      }
    }

    public override QuadtreeLeaf<T> AddObject(T o, BoundingBox box)
    {
      QuadtreeLeaf<T> quadtreeLeaf = (QuadtreeLeaf<T>) null;
      if (this.Box.Contains(box) == ContainmentType.Contains && (this.node1 == null || (quadtreeLeaf = this.node1.AddObject(o, box)) == null) && ((this.node2 == null || (quadtreeLeaf = this.node2.AddObject(o, box)) == null) && (this.node3 == null || (quadtreeLeaf = this.node3.AddObject(o, box)) == null)) && this.node4 != null)
        quadtreeLeaf = this.node4.AddObject(o, box);
      return quadtreeLeaf;
    }

    public override int AddLeavesWithBoxInsideFrustrum(
      BoundingFrustum frustum,
      List<QuadtreeLeaf<T>> list,
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
      }
      return num;
    }

    public void Split(Map map, int count)
    {
      if (!map.IsIntersectingMap(this.Box))
        return;
      float x1 = this.Box.Max.X - this.Box.Min.X;
      float y = this.Box.Max.Y - this.Box.Min.Y;
      float z1 = this.Box.Max.Z - this.Box.Min.Z;
      float x2 = x1 * 0.5f;
      float z2 = z1 * 0.5f;
      BoundingBox box1 = new BoundingBox(this.Box.Min, this.Box.Min + new Vector3(x2, y, z2));
      BoundingBox box2 = new BoundingBox(this.Box.Min + new Vector3(0.0f, 0.0f, z2), this.Box.Min + new Vector3(x2, y, z1));
      BoundingBox box3 = new BoundingBox(this.Box.Min + new Vector3(x2, 0.0f, 0.0f), this.Box.Min + new Vector3(x1, y, z2));
      BoundingBox box4 = new BoundingBox(this.Box.Min + new Vector3(x2, 0.0f, z2), this.Box.Min + new Vector3(x1, y, z1));
      if (count == 1)
      {
        this.node1 = (QuadtreeBase<T>) new QuadtreeLeaf<T>(this, box1);
        this.node2 = (QuadtreeBase<T>) new QuadtreeLeaf<T>(this, box2);
        this.node3 = (QuadtreeBase<T>) new QuadtreeLeaf<T>(this, box3);
        this.node4 = (QuadtreeBase<T>) new QuadtreeLeaf<T>(this, box4);
      }
      else
      {
        this.node1 = (QuadtreeBase<T>) new Quadtree<T>(this, box1);
        this.node2 = (QuadtreeBase<T>) new Quadtree<T>(this, box2);
        this.node3 = (QuadtreeBase<T>) new Quadtree<T>(this, box3);
        this.node4 = (QuadtreeBase<T>) new Quadtree<T>(this, box4);
        --count;
        ((Quadtree<T>) this.node1).Split(map, count);
        ((Quadtree<T>) this.node2).Split(map, count);
        ((Quadtree<T>) this.node3).Split(map, count);
        ((Quadtree<T>) this.node4).Split(map, count);
      }
    }
  }
}
