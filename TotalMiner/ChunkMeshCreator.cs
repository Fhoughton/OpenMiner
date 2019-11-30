// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkMeshCreator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ChunkMeshCreator : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<ChunkMeshCreator> Pool = new StudioForge.Engine.Core.Pool<ChunkMeshCreator>();
    private List<MapChunk> neighbours = new List<MapChunk>(27);
    private int poolHandle;
    private bool newBuilder;
    private MapChunkTM chunk;
    private GameInstance instance;
    private Action<bool, object> action;
    private object state;

    public string Name
    {
      get
      {
        return nameof (ChunkMeshCreator);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return false;
      }
    }

    public void Initialize(
      int poolHandle,
      GameInstance instance,
      MapChunkTM chunk,
      bool newBuilder,
      Action<bool, object> action,
      object state)
    {
      this.poolHandle = poolHandle;
      this.instance = instance;
      this.chunk = chunk;
      this.action = action;
      this.state = state;
      this.newBuilder = newBuilder;
    }

    public void Update()
    {
      if (this.chunk == null)
      {
        ChunkMeshCreator.Pool.Release(this.poolHandle);
        this.chunk = (MapChunkTM) null;
        this.instance = (GameInstance) null;
        this.action = (Action<bool, object>) null;
        this.state = (object) null;
      }
      else
      {
        Vector3 vector3 = this.chunk.Region.Offset.ToVector3() * this.chunk.Region.Map.TileSize;
        BoundingBox box = this.chunk.Box;
        box.Min += vector3;
        box.Max += vector3;
        bool success = false;
        try
        {
          bool splitLoad = false;
          bool fadeIn = false;
          MapTM map = this.chunk.Region.Map as MapTM;
          if (map != null && map.AllowMeshCreatorToSplitOrFade)
          {
            splitLoad = false;
            fadeIn = this.instance.IsInAnyLocalPlayerView(box);
            if (fadeIn)
            {
              int num = -1;
              int y = this.chunk.GlobalOffset.Y;
              if (this.instance.CurrentBiome == BiomeType.DigDeep)
                num = Globals2.GameProperties.SaveGame.Header.SaveVersion > 291 ? DigDeepBiome2.GetLavaLevelViewingID((Map) map, y) : DigDeepBiome.GetLavaLevelID((Map) map, y);
              if (num == -1 && y + map.ChunkSize.Y < (int) map.SeaLevel)
                fadeIn = false;
            }
          }
          IProgressBar progressBar = this.state as IProgressBar;
          if (progressBar == null)
          {
            IProgressBarRef state = this.state as IProgressBarRef;
            if (state != null)
              progressBar = state.ProgressBar;
          }
          success = this.chunk.Content.ReloadChunk(this.instance, this.chunk, splitLoad, fadeIn, false, this.newBuilder, progressBar);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(59, ex);
        }
        finally
        {
          try
          {
            this.chunk.LoadMeshEnd(success);
            this.chunk.ClearTimeStamps();
            this.neighbours.Clear();
            this.chunk.GetNeighbours(this.neighbours, (ChunkTest) null);
            foreach (MapChunk neighbour in this.neighbours)
              neighbour.ClearTimeStamps();
          }
          catch (Exception ex)
          {
          }
          finally
          {
            if (this.action != null)
              this.action(success, this.state);
            ChunkMeshCreator.Pool.Release(this.poolHandle);
            this.chunk = (MapChunkTM) null;
            this.instance = (GameInstance) null;
            this.action = (Action<bool, object>) null;
            this.state = (object) null;
          }
        }
      }
    }
  }
}
