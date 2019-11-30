// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FindSpace
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class FindSpace : IThreadWorkItem
  {
    public bool IsBusy;
    private GameInstance instance;
    private GlobalPoint3D origin;
    private int radius;
    private int distanceFromOrigin;
    private Action<GlobalPoint3D, GlobalPoint3D> onFound;
    private List<GlobalPoint3D> points;

    public string Name
    {
      get
      {
        return nameof (FindSpace);
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

    public void Initialize(
      GameInstance instance,
      Action<GlobalPoint3D, GlobalPoint3D> onFound,
      GlobalPoint3D origin,
      int radius,
      int distanceFromOrigin)
    {
      this.IsBusy = true;
      this.instance = instance;
      this.onFound = onFound;
      this.origin = origin;
      this.radius = radius;
      this.distanceFromOrigin = distanceFromOrigin;
      if (this.points != null)
        return;
      this.points = new List<GlobalPoint3D>(100);
    }

    public void Update()
    {
      try
      {
        this.instance.Map.FindPassableSpace(this.origin, this.radius, this.distanceFromOrigin, this.points);
        if (this.points.Count <= 0)
          return;
        this.onFound(this.origin, GlobalPoint3D.GetClosest(this.points, this.origin));
      }
      finally
      {
        this.points.Clear();
        this.IsBusy = false;
      }
    }
  }
}
