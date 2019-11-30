// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.QuadTree`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class QuadTree<T> where T : ISpatialNode
  {
    private static List<QuadTree<T>> leavesInsideBound = new List<QuadTree<T>>();
    public readonly BoundingBox BoundingBox;
    public readonly List<T> Objects;
    public int MaxObjects;
    public QuadTree<T> Parent;
    public QuadTree<T> TopLeft;
    public QuadTree<T> TopRight;
    public QuadTree<T> BottomLeft;
    public QuadTree<T> BottomRight;

    public QuadTree(int maxObjects, BoundingBox box)
    {
      this.MaxObjects = maxObjects;
      box.Min.Y = float.MinValue;
      box.Max.Y = float.MaxValue;
      this.BoundingBox = box;
      this.Objects = new List<T>(maxObjects);
    }

    public QuadTree(int maxObjects, Vector3 position, Vector3 scale)
      : this(maxObjects, new BoundingBox(position - scale * 0.5f, position + scale * 0.5f))
    {
    }

    public bool IsLeaf
    {
      get
      {
        return this.TopLeft == null;
      }
    }

    public int LeafCount
    {
      get
      {
        if (!this.IsLeaf)
          return this.TopLeft.LeafCount + this.TopRight.LeafCount + this.BottomLeft.LeafCount + this.BottomRight.LeafCount;
        return 4;
      }
    }

    public QuadTree<T> AddObject(T spatial)
    {
      QuadTree<T> quadTree = (QuadTree<T>) null;
      if (this.BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
      {
        if (this.TopLeft == null)
        {
          if (this.Objects.Count < this.MaxObjects)
          {
            this.Objects.Add(spatial);
            return this;
          }
          this.Split();
          if (this.TopLeft == null)
          {
            this.MaxObjects *= 2;
            return this.AddObject(spatial);
          }
        }
        quadTree = this.TopLeft.AddObject(spatial) ?? this.TopRight.AddObject(spatial) ?? this.BottomLeft.AddObject(spatial) ?? this.BottomRight.AddObject(spatial);
      }
      return quadTree;
    }

    public void RemoveObject(T spatial)
    {
      this.FindLeaf(spatial.Position)?.Objects.Remove(spatial);
    }

    public QuadTree<T> ObjectMoved(T spatial, QuadTree<T> prevNode)
    {
      QuadTree<T> quadTree = (QuadTree<T>) null;
      if (this.BoundingBox.Contains(spatial.Position) == ContainmentType.Contains)
      {
        if (this.TopLeft == null)
        {
          if (this == prevNode)
            return this;
          prevNode?.Objects.Remove(spatial);
          return this.AddObject(spatial);
        }
        if ((quadTree = this.TopLeft.ObjectMoved(spatial, prevNode)) == null && (quadTree = this.TopRight.ObjectMoved(spatial, prevNode)) == null && (quadTree = this.BottomLeft.ObjectMoved(spatial, prevNode)) == null)
          quadTree = this.BottomRight.ObjectMoved(spatial, prevNode);
      }
      return quadTree;
    }

    public QuadTree<T> FindLeaf(Vector3 position)
    {
      if (this.BoundingBox.Contains(position) != ContainmentType.Contains)
        return (QuadTree<T>) null;
      if (this.TopLeft == null)
        return this;
      QuadTree<T> leaf;
      if ((leaf = this.TopLeft.FindLeaf(position)) == null && (leaf = this.TopRight.FindLeaf(position)) == null && (leaf = this.BottomLeft.FindLeaf(position)) == null)
        leaf = this.BottomRight.FindLeaf(position);
      return leaf;
    }

    public List<QuadTree<T>> GetLeavesInsideFrustrum(BoundingFrustum frustum)
    {
      QuadTree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideFrustrum(frustum);
      return QuadTree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideFrustrum(BoundingFrustum frustum)
    {
      if (frustum.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft == null)
      {
        QuadTree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft.AddLeavesInsideFrustrum(frustum);
        this.TopRight.AddLeavesInsideFrustrum(frustum);
        this.BottomLeft.AddLeavesInsideFrustrum(frustum);
        this.BottomRight.AddLeavesInsideFrustrum(frustum);
      }
    }

    public List<QuadTree<T>> GetLeavesInsideSphere(BoundingSphere sphere)
    {
      QuadTree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideSphere(sphere);
      return QuadTree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideSphere(BoundingSphere sphere)
    {
      if (sphere.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft == null)
      {
        QuadTree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft.AddLeavesInsideSphere(sphere);
        this.TopRight.AddLeavesInsideSphere(sphere);
        this.BottomLeft.AddLeavesInsideSphere(sphere);
        this.BottomRight.AddLeavesInsideSphere(sphere);
      }
    }

    public List<QuadTree<T>> GetLeavesInsideSphereBand(
      BoundingSphere innerSphere,
      BoundingSphere outerSphere)
    {
      QuadTree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideSphereBand(innerSphere, outerSphere);
      return QuadTree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideSphereBand(BoundingSphere innerSphere, BoundingSphere outerSphere)
    {
      if (outerSphere.Contains(this.BoundingBox) == ContainmentType.Disjoint || innerSphere.Contains(this.BoundingBox) == ContainmentType.Contains)
        return;
      if (this.TopLeft == null)
      {
        QuadTree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.TopRight.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomLeft.AddLeavesInsideSphereBand(innerSphere, outerSphere);
        this.BottomRight.AddLeavesInsideSphereBand(innerSphere, outerSphere);
      }
    }

    public List<QuadTree<T>> GetLeavesInsideBox(BoundingBox box)
    {
      QuadTree<T>.leavesInsideBound.Clear();
      this.AddLeavesInsideBox(box);
      return QuadTree<T>.leavesInsideBound;
    }

    private void AddLeavesInsideBox(BoundingBox box)
    {
      if (box.Contains(this.BoundingBox) == ContainmentType.Disjoint)
        return;
      if (this.TopLeft == null)
      {
        QuadTree<T>.leavesInsideBound.Add(this);
      }
      else
      {
        this.TopLeft.AddLeavesInsideBox(box);
        this.TopRight.AddLeavesInsideBox(box);
        this.BottomLeft.AddLeavesInsideBox(box);
        this.BottomRight.AddLeavesInsideBox(box);
      }
    }

    private void Split()
    {
      float x1 = (float) (((double) this.BoundingBox.Max.X - (double) this.BoundingBox.Min.X) * 0.5);
      float z1 = (float) (((double) this.BoundingBox.Max.Z - (double) this.BoundingBox.Min.Z) * 0.5);
      Vector3 scale = new Vector3(x1, 0.0f, z1);
      float x2 = x1 * 0.5f;
      float z2 = z1 * 0.5f;
      if ((double) x2 == 0.0 || (double) z2 == 0.0)
        return;
      Vector3 position1 = this.BoundingBox.Min + new Vector3(x2, 0.0f, z2);
      Vector3 position2 = this.BoundingBox.Min + new Vector3(x2 + x1, 0.0f, z2);
      Vector3 position3 = this.BoundingBox.Min + new Vector3(x2, 0.0f, z2 + z1);
      Vector3 position4 = this.BoundingBox.Min + new Vector3(x2 + x1, 0.0f, z2 + z1);
      this.TopLeft = new QuadTree<T>(this.MaxObjects, position1, scale);
      this.TopRight = new QuadTree<T>(this.MaxObjects, position2, scale);
      this.BottomLeft = new QuadTree<T>(this.MaxObjects, position3, scale);
      this.BottomRight = new QuadTree<T>(this.MaxObjects, position4, scale);
      this.TopLeft.Parent = this;
      this.TopRight.Parent = this;
      this.BottomLeft.Parent = this;
      this.BottomRight.Parent = this;
      this.ReassignObjects();
      this.Objects.Clear();
    }

    private void ReassignObjects()
    {
      foreach (T spatial in this.Objects)
      {
        if (this.TopLeft.AddObject(spatial) == null && this.TopRight.AddObject(spatial) == null && this.BottomLeft.AddObject(spatial) == null)
          this.BottomRight.AddObject(spatial);
      }
    }
  }
}
