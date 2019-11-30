// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.MapChunkContent
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Threading;

namespace StudioForge.TotalMiner.Graphics
{
  internal class MapChunkContent
  {
    public static int ChunksLoadedCount = 0;
    public static int ChunksUnloadedCount = 0;
    public static int VBNewCount = 0;
    public static int VBRecycledCount = 0;
    private static Vector3 halfVector3 = new Vector3(0.5f, 0.5f, 0.5f);
    public byte Alpha;
    public Matrix World;
    public MapChunkContentData ContentData;
    private MapChunkContentFlags flags;
    public static int[] Indices;
    public static IndexBuffer IndexBuffer;
    public static Vector2[] TexCoords1;
    public static Vector2[] TexCoords2;
    public static Vector2[] TexCoords3;
    public static Vector2[] TexCoords4;
    public static int[,] TexOffsets;

    public MapChunkContentData GetVertexData(
      MapChunk chunk,
      MapChunkContentData[] dataList,
      int index)
    {
      lock (dataList)
      {
        MapChunkContentData data = dataList[index];
        if (data.VertexBufferChanged)
        {
          this.UpdateVertexBufferChange(ref data);
          dataList[index] = data;
        }
        return data;
      }
    }

    public MapChunkContentData GetVertexData()
    {
      if (this.ContentData.VertexBufferChanged)
        this.UpdateVertexBufferChange(ref this.ContentData);
      return this.ContentData;
    }

    private void UpdateVertexBufferChange(ref MapChunkContentData data)
    {
      if (data.VertexBuffer != null && data.VertexBuffer != data.NewVertexBuffer)
        data.VertexBuffer.Dispose();
      data.VertexCount = data.NewVertexCount;
      data.WaterVertexCount = data.NewWaterVertexCount;
      data.VertexBuffer = data.NewVertexBuffer;
      data.NewVertexBuffer = (VertexBuffer) null;
      data.VertexBufferChanged = false;
    }

    public int TotalMeshSize(MapChunk chunk)
    {
      MapTM map = chunk.Region.Map as MapTM;
      bool flag = false;
      MapChunkContentData[] chunkContentDataArray;
      lock (map.MapChunkContentBreakdown)
        flag = map.MapChunkContentBreakdown.TryGetValue(chunk.GetGlobalHashCode(), out chunkContentDataArray);
      int num = 0;
      if (!flag)
        return this.GetMeshSize(this.ContentData);
      foreach (MapChunkContentData data in chunkContentDataArray)
        num += this.GetMeshSize(data);
      return num;
    }

    private int GetMeshSize(MapChunkContentData data)
    {
      int num = 0;
      if (data.NewVertexBuffer != null && !data.NewVertexBuffer.IsDisposed)
        num += (int) data.NewVertexBuffer.BufferSize();
      if (data.VertexBuffer != null && !data.VertexBuffer.IsDisposed)
        num += (int) data.VertexBuffer.BufferSize();
      return num;
    }

    public bool IsContentFlagSet(MapChunkContentFlags flag)
    {
      return (this.flags & flag) > MapChunkContentFlags.None;
    }

    public bool IsContentFlagsSet(MapChunkContentFlags flag)
    {
      return (this.flags & flag) == flag;
    }

    public void SetContentFlag(MapChunkContentFlags flag)
    {
      this.flags |= flag;
    }

    public void ClearContentFlag(MapChunkContentFlags flag)
    {
      this.flags &= ~flag;
    }

    public bool IsContentSplit
    {
      get
      {
        return (this.flags & MapChunkContentFlags.ContentSplit) > MapChunkContentFlags.None;
      }
    }

    public MapChunkContent(MapChunk chunk)
    {
      this.World = Matrix.CreateTranslation(chunk.GlobalOffset.ToVector3() * chunk.Region.Map.TileSize);
    }

    public bool ReloadChunk(
      GameInstance instance,
      MapChunkTM chunk,
      bool splitLoad,
      bool fadeIn,
      bool addExtraVerts,
      bool newBuilderInstance,
      IProgressBar progressBar)
    {
      if (!Monitor.TryEnter((object) chunk))
        return false;
      try
      {
        return this.ReloadChunkCore(instance, chunk, splitLoad, fadeIn, addExtraVerts, newBuilderInstance, progressBar);
      }
      finally
      {
        Monitor.Exit((object) chunk);
      }
    }

    private bool ReloadChunkCore(
      GameInstance instance,
      MapChunkTM chunk,
      bool splitLoad,
      bool fadeIn,
      bool addExtraVerts,
      bool newBuilderInstance,
      IProgressBar progressBar)
    {
      bool flag = true;
      if (!newBuilderInstance)
      {
        int i = 0;
        try
        {
          i = VoxelMeshBuilder.Pool.GetNext();
          VoxelMeshBuilder builder = VoxelMeshBuilder.Pool.List[i];
          builder.Initialize(instance);
          this.RebuildVertices(builder, chunk, splitLoad, addExtraVerts, progressBar);
        }
        catch (Exception ex)
        {
          flag = false;
          Services.ExceptionReporter.ReportExceptionCaught(55, ex);
        }
        finally
        {
          VoxelMeshBuilder.Pool.Release(i);
          if (!chunk.IsMeshLoaded && flag)
          {
            if (this.Alpha == (byte) 0)
              this.Alpha = fadeIn ? (byte) 0 : byte.MaxValue;
            ++MapChunkContent.ChunksLoadedCount;
          }
        }
      }
      else
      {
        VoxelMeshBuilder builder = (VoxelMeshBuilder) null;
        try
        {
          builder = new VoxelMeshBuilder();
          builder.Initialize(instance);
          this.RebuildVertices(builder, chunk, splitLoad, addExtraVerts, progressBar);
        }
        catch (Exception ex)
        {
          flag = false;
          Services.ExceptionReporter.ReportExceptionCaught(55, ex);
        }
        finally
        {
          builder?.UnloadContent();
        }
      }
      return flag;
    }

    public void UnloadChunk(MapChunkTM chunk)
    {
      if (!chunk.IsMeshLoaded)
        return;
      this.UnloadChunkCore(chunk);
      chunk.ClearChunkFlag(ChunkFlags.MeshLoaded);
      chunk.SetChunkFlag(ChunkFlags.MeshDirty);
      this.flags = MapChunkContentFlags.None;
      this.Alpha = (byte) 0;
      ++MapChunkContent.ChunksUnloadedCount;
    }

    private void UnloadChunkCore(MapChunkTM chunk)
    {
      this.RemoveBreakdown(chunk);
      this.ContentData.VertexCount = 0;
      this.ContentData.NewVertexCount = 0;
      this.ContentData.WaterVertexCount = 0;
      this.ContentData.NewWaterVertexCount = 0;
      this.ContentData.VertexBufferChanged = false;
      if (this.ContentData.VertexBuffer != null)
        this.ContentData.VertexBuffer.Dispose();
      if (this.ContentData.NewVertexBuffer != null)
        this.ContentData.NewVertexBuffer.Dispose();
      this.ContentData.VertexBuffer = (VertexBuffer) null;
      this.ContentData.NewVertexBuffer = (VertexBuffer) null;
    }

    public void RemoveBreakdown(MapChunkTM chunk)
    {
      this.ClearContentFlag(MapChunkContentFlags.ContentSplit);
      MapTM map = chunk.Region.Map as MapTM;
      long globalHashCode = chunk.GetGlobalHashCode();
      lock (map.MapChunkContentBreakdown)
      {
        MapChunkContentData[] t;
        if (!map.MapChunkContentBreakdown.TryGetValue(globalHashCode, out t))
          return;
        map.MapChunkContentBreakdown.Remove(globalHashCode);
        map.MapChunkContentBreakdownPool.Release(t);
      }
    }

    private void RebuildVertices(
      VoxelMeshBuilder builder,
      MapChunkTM chunk,
      bool splitLoad,
      bool addExtraVerts,
      IProgressBar progressBar)
    {
      if (!splitLoad)
        builder.BuildChunkMesh_NewFormat(chunk, Globals2.GameSettings.OldskoolLight, addExtraVerts, progressBar);
      else
        builder.BuildChunkMeshSplit_NewFormat(chunk, Globals2.GameSettings.OldskoolLight);
    }

    public static void BuildChunkIndices(int maxPrimitives, bool forceRebuild)
    {
      if (MapChunkContent.IndexBuffer != null && !forceRebuild)
        return;
      int[] numArray = new int[maxPrimitives * 3];
      GraphicStatics.InitIndices(numArray);
      IndexBuffer indexBuffer = new IndexBuffer(CoreGlobals.GraphicsDevice, IndexElementSize.ThirtyTwoBits, numArray.Length, BufferUsage.WriteOnly);
      indexBuffer.SetData<int>(numArray);
      MapChunkContent.IndexBuffer = indexBuffer;
      MapChunkContent.Indices = numArray;
    }
  }
}
