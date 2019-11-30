// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Octree`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class Octree<T> where T : ISpatialNode
  {
    private static List<Octree<T>> leavesInsideBound = new List<Octree<T>>();
    public readonly BoundingBox BoundingBox;
    public readonly List<T> Objects;
    public int MaxObjects;
    public Octree<T> Parent;
    public Octree<T> TopLeft1;
    public Octree<T> TopRight1;
    public Octree<T> BottomLeft1;
    public Octree<T> BottomRight1;
    public Octree<T> TopLeft2;
    public Octree<T> TopRight2;
    public Octree<T> BottomLeft2;
    public Octree<T> BottomRight2;

    public Octree(int maxObjects, BoundingBox box)
    {
      this.MaxObjects = maxObjects;
      this.BoundingBox = box;
      this.Objects = new List<T>(maxObjects);
    }

    public Octree(int maxObjects, Vector3 position, Vector3 scale)
      : this(maxObjects, new BoundingBox(position - scale * 0.5f, position + scale * 0.5f))
    {
    }

    public bool IsLeaf
    {
      get
      {
        return this.TopLeft1 == null;
      }
    }

    public int LeafCount
    {
      get
      {
        if (!this.IsLeaf)
          return this.TopLeft1.LeafCount + this.TopRight1.LeafCount + this.BottomLeft1.LeafCount + this.BottomRight1.LeafCount + this.TopLeft2.LeafCount + this.TopRight2.LeafCount + this.BottomLeft2.LeafCount + this.BottomRight2.LeafCount;
        return 8;
      }
    }

    public Octree<T> AddObject(T spatial)
    {
      Octree<T> octree = (Octree<T>) null;
      if (this.BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
      {
        if (this.TopLeft1 == null)
        {
          if (this.Objects.Count < this.MaxObjects)
          {
            this.Objects.Add(spatial);
            return this;
          }
          this.Split();
          if (this.TopLeft1 == null)
          {
            this.MaxObjects *= 2;
            return this.AddObject(spatial);
          }
        }
        octree = this.TopLeft1.AddObject(spatial) ?? this.TopRight1.AddObject(spatial) ?? this.BottomLeft1.AddObject(spatial) ?? this.BottomRight1.AddObject(spatial) ?? this.TopLeft2.AddObject(spatial) ?? this.TopRight2.AddObject(spatial) ?? this.BottomLeft2.AddObject(spatial) ?? this.BottomRight2.AddObject(spatial);
      }
      return octree;
    }

    public void RemoveObject(T spatial)
    {
      this.FindLeaf(spatial.Position)?.Objects.Remove(spatial);
    }

    public Octree<T> ObjectMoved(T spatial, Octree<T> prevNode)
    {
      Octree<T> octree = (Octree<T>) null;
      if (this.BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
      {
        if (this.TopLeft1 == null)
        {
          if (this == prevNode)
            return this;
          prevNode?.Objects.Remove(spatial);
          return this.AddObject(spatial);
        }
        if ((octree = this.TopLeft1.ObjectMoved(spatial, prevNode)) == null && (octree = this.TopRight1.ObjectMoved(spatial, prevNode)) == null && ((octree = this.BottomLeft1.ObjectMoved(spatial, prevNode)) == null && (octree = this.BottomRight1.ObjectMoved(spatial, prevNode)) == null) && ((octree = this.TopLeft2.ObjectMoved(spatial, prevNode)) == null && (octree = this.TopRight2.ObjectMoved(spatial, prevNode)) == null && (octree = this.BottomLeft2.ObjectMoved(spatial, prevNode)) == null))
          octree = this.BottomRight2.ObjectMoved(spatial, prevNode);
      }
      return octree;
    }

    public Octree<T> FindLeaf(Vector3 position)
    {
      if (this.BoundingBox.Contains(position) != ContainmentType.Contains)
        return (Octree<T>) null;
      if (this.TopLeft1 == null)
        return this;
      Octree<T> leaf;
      if ((leaf = this.TopLeft1.FindLeaf(position)) == null && (leaf = this.TopRight1.FindLeaf(position)) == null && ((leaf = this.BottomLeft1.FindLeaf(position)) == null && (leaf = this.BottomRight1.FindLeaf(position)) == null) && ((leaf = this.TopLeft2.FindLeaf(position)) == null && (leaf = this.TopRight2.FindLeaf(position)) == null && (leaf = this.BottomLeft2.FindLeaf(position)) == null))
        leaf = this.BottomRight2.FindLeaf(position);
      return leaf;
    }

    public List<Octree<T>> GetLeavesInsideFrustrum(BoundingFrustum frustum)
    {
      Octree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideFrustrum(frustum);
      return Octree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideFrustrum(BoundingFrustum frustum)
    {
      if (frustum.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft1 == null)
      {
        Octree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft1.AddLeavesInsideFrustrum(frustum);
        this.TopRight1.AddLeavesInsideFrustrum(frustum);
        this.BottomLeft1.AddLeavesInsideFrustrum(frustum);
        this.BottomRight1.AddLeavesInsideFrustrum(frustum);
        this.TopLeft2.AddLeavesInsideFrustrum(frustum);
        this.TopRight2.AddLeavesInsideFrustrum(frustum);
        this.BottomLeft2.AddLeavesInsideFrustrum(frustum);
        this.BottomRight2.AddLeavesInsideFrustrum(frustum);
      }
    }

    public List<Octree<T>> GetLeavesInsideSphere(BoundingSphere sphere)
    {
      Octree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideSphere(sphere);
      return Octree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideSphere(BoundingSphere sphere)
    {
      if (sphere.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft1 == null)
      {
        Octree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft1.AddLeavesInsideSphere(sphere);
        this.TopRight1.AddLeavesInsideSphere(sphere);
        this.BottomLeft1.AddLeavesInsideSphere(sphere);
        this.BottomRight1.AddLeavesInsideSphere(sphere);
        this.TopLeft2.AddLeavesInsideSphere(sphere);
        this.TopRight2.AddLeavesInsideSphere(sphere);
        this.BottomLeft2.AddLeavesInsideSphere(sphere);
        this.BottomRight2.AddLeavesInsideSphere(sphere);
      }
    }

    public List<Octree<T>> GetLeavesInsideSphereBand(
      BoundingSphere innerSphere,
      BoundingSphere outerSphere)
    {
      Octree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideSphereBand(innerSphere, outerSphere);
      return Octree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideSphereBand(BoundingSphere innerSphere, BoundingSphere outerSphere)
    {
      if (outerSphere.Contains(this.BoundingBox) == ContainmentType.Disjoint || innerSphere.Contains(this.BoundingBox) == ContainmentType.Contains)
        return;
      if (this.TopLeft1 == null)
      {
        Octree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft1.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.TopRight1.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomLeft1.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomRight1.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.TopLeft2.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.TopRight2.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomLeft2.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomRight2.AddLeavesInsideSphereBand(innerSphere, outerSphere);
      }
    }

    public List<Octree<T>> GetLeavesInsideBox(BoundingBox box)
    {
      Octree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideBox(box);
      return Octree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideBox(BoundingBox box)
    {
      if (box.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft1 == null)
      {
        Octree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft1.AddLeavesInsideBox(box);
        this.TopRight1.AddLeavesInsideBox(box);
        this.BottomLeft1.AddLeavesInsideBox(box);
        this.BottomRight1.AddLeavesInsideBox(box);
        this.TopLeft2.AddLeavesInsideBox(box);
        this.TopRight2.AddLeavesInsideBox(box);
        this.BottomLeft2.AddLeavesInsideBox(box);
        this.BottomRight2.AddLeavesInsideBox(box);
      }
    }

    private void Split()
    {
      float x1 = (float) (((double) this.BoundingBox.Max.X - (double) this.BoundingBox.Min.X) * 0.5);
      float num = (float) (((double) this.BoundingBox.Max.Y - (double) this.BoundingBox.Min.Y) * 0.5);
      float z1 = (float) (((double) this.BoundingBox.Max.Z - (double) this.BoundingBox.Min.Z) * 0.5);
      Vector3 scale = new Vector3(x1, 0.0f, z1);
      float x2 = x1 * 0.5f;
      float y = num * 0.5f;
      float z2 = z1 * 0.5f;
      if ((double) x2 == 0.0 || (double) z2 == 0.0)
        return;
      Vector3 position1 = this.BoundingBox.Min + new Vector3(x2, num + y, z2);
      Vector3 position2 = this.BoundingBox.Min + new Vector3(x2 + x1, num + y, z2);
      Vector3 position3 = this.BoundingBox.Min + new Vector3(x2, num + y, z2 + z1);
      Vector3 position4 = this.BoundingBox.Min + new Vector3(x2 + x1, num + y, z2 + z1);
      Vector3 position5 = this.BoundingBox.Min + new Vector3(x2, y, z2);
      Vector3 position6 = this.BoundingBox.Min + new Vector3(x2 + x1, y, z2);
      Vector3 position7 = this.BoundingBox.Min + new Vector3(x2, y, z2 + z1);
      Vector3 position8 = this.BoundingBox.Min + new Vector3(x2 + x1, y, z2 + z1);
      this.TopLeft1 = new Octree<T>(this.MaxObjects, position1, scale);
      this.TopRight1 = new Octree<T>(this.MaxObjects, position2, scale);
      this.BottomLeft1 = new Octree<T>(this.MaxObjects, position3, scale);
      this.BottomRight1 = new Octree<T>(this.MaxObjects, position4, scale);
      this.TopLeft2 = new Octree<T>(this.MaxObjects, position5, scale);
      this.TopRight2 = new Octree<T>(this.MaxObjects, position6, scale);
      this.BottomLeft2 = new Octree<T>(this.MaxObjects, position7, scale);
      this.BottomRight2 = new Octree<T>(this.MaxObjects, position8, scale);
      this.TopLeft1.Parent = this;
      this.TopRight1.Parent = this;
      this.BottomLeft1.Parent = this;
      this.BottomRight1.Parent = this;
      this.TopLeft2.Parent = this;
      this.TopRight2.Parent = this;
      this.BottomLeft2.Parent = this;
      this.BottomRight2.Parent = this;
      this.ReassignObjects();
      this.Objects.Clear();
    }

    private void ReassignObjects()
    {
      foreach (T spatial in this.Objects)
      {
        if (this.TopLeft1.AddObject(spatial) == null && this.TopRight1.AddObject(spatial) == null && (this.BottomLeft1.AddObject(spatial) == null && this.BottomRight1.AddObject(spatial) == null) && (this.TopLeft2.AddObject(spatial) == null && this.TopRight2.AddObject(spatial) == null && this.BottomLeft2.AddObject(spatial) == null))
          this.BottomRight2.AddObject(spatial);
      }
    }
  }
}
