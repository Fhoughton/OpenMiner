// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ComponentLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ComponentLoader : IThreadWorkItem
  {
    private string comPack;
    private VoxelModelManager voxelModelManager;
    private Queue<string> assets;
    private Queue<float> heights;
    private Queue<int> indexes;
    private PriorityLevel priority;
    private Action<MapModel, int> onLoaded;
    private Func<int, bool> shouldLoadMesh;
    private bool run;
    private bool unload;

    public string Name
    {
      get
      {
        return nameof (ComponentLoader);
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
        return true;
      }
    }

    public ComponentLoader(
      PriorityLevel priority,
      VoxelModelManager voxelModelManager,
      string comPack,
      Action<MapModel, int> onLoaded,
      Func<int, bool> shouldLoadMesh)
    {
      this.priority = priority;
      this.voxelModelManager = voxelModelManager;
      this.comPack = comPack;
      this.onLoaded = onLoaded;
      this.shouldLoadMesh = shouldLoadMesh;
      this.assets = new Queue<string>();
      this.heights = new Queue<float>();
      this.indexes = new Queue<int>();
      this.run = true;
    }

    public void AddComponentToLoad(string asset, float height, int index)
    {
      lock (this.assets)
      {
        this.assets.Enqueue(asset);
        this.heights.Enqueue(height);
        this.indexes.Enqueue(index);
      }
    }

    public void DequeueComponent(string asset)
    {
    }

    public void Abort(bool unload)
    {
      this.run = false;
      this.unload = unload;
    }

    public void Update()
    {
      try
      {
        this.LoadNextComponent();
      }
      finally
      {
        if (this.run)
          ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
        else if (this.unload)
          this.voxelModelManager.UnloadContent();
      }
    }

    private void LoadNextComponent()
    {
      string componentName;
      int num1;
      lock (this.assets)
      {
        if (this.assets.Count < 1)
          return;
        componentName = this.assets.Dequeue();
        double num2 = (double) this.heights.Dequeue();
        num1 = this.indexes.Dequeue();
      }
      MapModel mapModel = this.voxelModelManager.LoadComponent(this.comPack, componentName, false);
      if (!this.run)
        return;
      this.onLoaded(mapModel, num1);
      if (!this.shouldLoadMesh(num1))
        return;
      mapModel.LoadContent((InitState) null);
    }
  }
}
