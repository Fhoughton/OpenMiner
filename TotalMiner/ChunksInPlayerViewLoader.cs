// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunksInPlayerViewLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ChunksInPlayerViewLoader
  {
    public static Pool<List<MapChunk>> ChunksToDrawPool = new Pool<List<MapChunk>>(2);
    private BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
    private List<MapRegion> regionsInViewRange = new List<MapRegion>(6);
    public Vector3 EyePositionForSort;
    private Map map;
    private GameInstance instance;
    private Vector3 halfVector3ForSort;
    private Comparison<MapChunk> sortChunksFrontToBack;

    public static void ReleaseChunks(List<MapChunk> list)
    {
      list.Clear();
      ChunksInPlayerViewLoader.ChunksToDrawPool.Release(list);
    }

    public void Initialize(GameInstance instance, Map map)
    {
      this.map = map;
      this.instance = instance;
      this.halfVector3ForSort = new Vector3(map.TileSize * 0.5f);
      this.sortChunksFrontToBack = new Comparison<MapChunk>(this.SortChunksFrontToBack);
    }

    public void LoadChunksInView(Player player, Player virtualPlayer)
    {
      this.regionsInViewRange.Clear();
      this.EyePositionForSort = virtualPlayer.EyePosition;
      this.frustum.Matrix = virtualPlayer.ViewMatrix * player.ProjectionMatrix;
      foreach (MapRegion mapRegion in this.map.Regions.Values)
      {
        if (this.frustum.Contains(mapRegion.Box) != ContainmentType.Disjoint)
          this.regionsInViewRange.Add(mapRegion);
      }
      if (this.regionsInViewRange.Count <= 0)
        return;
      this.LoadChunksInViewForRegions(player, virtualPlayer);
    }

    private void LoadChunksInViewForRegions(Player player, Player virtualPlayer)
    {
      int next = ChunksInPlayerViewLoader.ChunksToDrawPool.GetNext();
      List<MapChunk> list = ChunksInPlayerViewLoader.ChunksToDrawPool.List[next];
      while (list.Capacity < 900)
        list.Add((MapChunk) null);
      list.Clear();
      int num = 0;
      foreach (MapRegion mapRegion in this.regionsInViewRange)
        num += mapRegion.Octree.GetObjectsWithBoxInsideFrustum(this.frustum, list, mapRegion.Box.Min);
      virtualPlayer.ChunksToDraw.AddItem(list);
    }

    public int SortChunksFrontToBack(MapChunk chunk1, MapChunk chunk2)
    {
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      GlobalPoint3D offset1 = chunk1.Region.Offset;
      Vector3 min1 = chunk1.Box.Min;
      Vector3 max1 = chunk1.Box.Max;
      vector3_1.X = (max1.X - min1.X) * this.halfVector3ForSort.X + min1.X + (float) offset1.X;
      vector3_1.Y = (max1.Y - min1.Y) * this.halfVector3ForSort.Y + min1.Y + (float) offset1.Y;
      vector3_1.Z = (max1.Z - min1.Z) * this.halfVector3ForSort.Z + min1.Z + (float) offset1.Z;
      GlobalPoint3D offset2 = chunk2.Region.Offset;
      Vector3 min2 = chunk2.Box.Min;
      Vector3 max2 = chunk2.Box.Max;
      vector3_2.X = (max2.X - min2.X) * this.halfVector3ForSort.X + min2.X + (float) offset2.X;
      vector3_2.Y = (max2.Y - min2.Y) * this.halfVector3ForSort.Y + min2.Y + (float) offset2.Y;
      vector3_2.Z = (max2.Z - min2.Z) * this.halfVector3ForSort.Z + min2.Z + (float) offset2.Z;
      return Vector3.DistanceSquared(this.EyePositionForSort, vector3_1).CompareTo(Vector3.DistanceSquared(this.EyePositionForSort, vector3_2));
    }
  }
}
