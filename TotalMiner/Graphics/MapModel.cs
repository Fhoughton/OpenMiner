// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.MapModel
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class MapModel : IHasInitialization, IHasContent, IUnmanagedBuffer
  {
    private List<Point3D> explodePoints = new List<Point3D>(10);
    private List<Block> explodeBlockIDs = new List<Block>(10);
    public MapTM Map;
    public GlobalPoint3D ModelSize;
    public Vector3 World;
    public int DirNum;
    public string ComName;
    public bool HasMergedBlockTextures;
    public ModelFlags Flags;
    public bool IsSystemModel;
    private MapChunkTM chunk;
    private GameInstance instance;

    public GameInstance Instance
    {
      get
      {
        return this.instance;
      }
    }

    public MapChunkContentData MapChunkContentData
    {
      get
      {
        if (this.chunk == null || this.chunk.Content == null)
          return new MapChunkContentData();
        return this.chunk.Content.GetVertexData();
      }
    }

    public static GlobalPoint3D EdgeBufferHalf
    {
      get
      {
        return new GlobalPoint3D(2, 2, 2);
      }
    }

    public static GlobalPoint3D EdgeBufferFull
    {
      get
      {
        return new GlobalPoint3D(4, 4, 4);
      }
    }

    public MapModel(GameInstance instance, MapTM map)
    {
      this.instance = instance;
      this.Map = map;
      this.HasMergedBlockTextures = false;
      this.ModelSize = map != null ? map.MapSize : new GlobalPoint3D(20, 38, 16);
    }

    public virtual void Initialize(InitState state)
    {
    }

    public virtual void LoadContent(InitState state)
    {
      if (this.Map == null)
        return;
      this.chunk = this.Map.GetChunk(GlobalPoint3D.Zero) as MapChunkTM;
      this.chunk.Content.ReloadChunk(this.instance, this.chunk, false, false, false, true, (IProgressBar) null);
    }

    public virtual void LoadContent(bool buildMesh)
    {
      this.LoadContent(buildMesh, (Action<bool, object>) null, (object) null);
    }

    public virtual void LoadContent(bool buildMesh, Action<bool, object> action, object state)
    {
      if (this.Map != null && buildMesh)
      {
        this.chunk = this.Map.GetChunk(GlobalPoint3D.Zero) as MapChunkTM;
        if (!this.chunk.IsMeshLoaded)
        {
          this.chunk.LoadMesh(false, true, action, state);
          return;
        }
      }
      if (action == null)
        return;
      action(true, state);
    }

    public virtual void UnloadContent()
    {
      if (this.Map == null)
        return;
      this.Map.UnloadContent();
    }

    public long BufferSize
    {
      get
      {
        long num = 0;
        VertexBuffer vertexBuffer = this.MapChunkContentData.VertexBuffer;
        if (vertexBuffer != null)
          num += vertexBuffer.BufferSize();
        return num;
      }
    }

    public GlobalPoint3D? FindBlockInRow(Block block, int row)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.Y = row;
      byte num = (byte) block;
      for (p.X = this.Map.MapBound.Min.X; p.X < this.Map.MapBound.Max.X; ++p.X)
      {
        for (p.Z = this.Map.MapBound.Min.Z; p.Z < this.Map.MapBound.Max.Z; ++p.Z)
        {
          if ((int) this.Map.GetBlockID(p) == (int) num)
            return new GlobalPoint3D?(p);
        }
      }
      return new GlobalPoint3D?();
    }

    public void Explode(
      GameInstance instance,
      Vector3 position,
      Vector2 scale,
      float ratio,
      float violence)
    {
      if (this.explodePoints.Count == 0)
        this.BuildExplodeData(instance.Random, ratio);
      GlobalPoint3D mapSize = this.Map.MapSize;
      Point3D chunkSize = this.Map.ChunkSize;
      Vector3 vector3 = new Vector3((float) ((double) mapSize.X * (double) scale.X * -0.25), 0.0f, (float) ((double) mapSize.Z * (double) scale.X * -0.25));
      Vector3 velocity = new Vector3();
      Vector3 zero1 = Vector3.Zero;
      Vector3 zero2 = Vector3.Zero;
      for (int index = 0; index < this.explodePoints.Count; ++index)
      {
        Point3D explodePoint = this.explodePoints[index];
        zero2.X = position.X + (float) ((double) explodePoint.X * (double) scale.X * 0.5) + vector3.X;
        zero2.Y = position.Y + (float) ((double) explodePoint.Y * (double) scale.Y * 0.5) + vector3.Y;
        zero2.Z = position.Z + (float) ((double) explodePoint.Z * (double) scale.X * 0.5) + vector3.Z;
        velocity.X = (float) (instance.Random.NextDouble() * 2.0 - 1.0) * violence;
        velocity.Y = (float) (instance.Random.NextDouble() * 2.0 - 1.0) * violence;
        velocity.Z = (float) (instance.Random.NextDouble() * 2.0 - 1.0) * violence;
        InventoryItem inventoryItem = new InventoryItem((Item) this.explodeBlockIDs[index], 1);
        instance.ParticleManager.AddNew(ParticleType.Debris, 2.5f + (float) instance.Random.NextDouble(), zero2, velocity, scale.X, inventoryItem, instance.ParticleModifiers.ModelExplodeParticleModifier, 0.0f, -1, (byte) 0, GamerID.Sys1, false, true);
      }
    }

    private void BuildExplodeData(PcgRandom random, float explodeRatio)
    {
      this.explodePoints.Clear();
      this.explodeBlockIDs.Clear();
      GlobalPoint3D mapSize = this.Map.MapSize;
      Point3D chunkSize = this.Map.ChunkSize;
      Point3D p = new Point3D();
      MapChunk chunk = this.Map.Regions[0].Chunks[0];
      for (p.Y = 0; p.Y < chunkSize.Y; ++p.Y)
      {
        for (p.Z = 0; p.Z < chunkSize.Z; ++p.Z)
        {
          for (p.X = 0; p.X < chunkSize.X; ++p.X)
          {
            byte blockId = chunk.GetBlockID(p);
            if (blockId > (byte) 0 && chunk.IsNextTo((StudioForge.BlockWorld.Map) this.Map, p, (byte) 0) && random.RandomChance((double) explodeRatio))
            {
              this.explodePoints.Add(p);
              this.explodeBlockIDs.Add((Block) blockId);
            }
          }
        }
      }
    }
  }
}
