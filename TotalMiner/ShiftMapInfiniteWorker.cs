// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ShiftMapInfiniteWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class ShiftMapInfiniteWorker : IThreadWorkItem
  {
    public bool IsBusy;
    private Map map;
    private BlockFace direction;

    public string Name
    {
      get
      {
        return "ShiftMap";
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

    public void Initialize(Map map, BlockFace direction)
    {
      this.IsBusy = true;
      this.map = map;
      this.direction = direction;
    }

    public void Update()
    {
      try
      {
        switch (this.direction)
        {
          case BlockFace.Left:
            this.map.ShiftLeft();
            break;
          case BlockFace.Forward:
            this.map.ShiftForward();
            break;
          case BlockFace.Right:
            this.map.ShiftRight();
            break;
          case BlockFace.Backward:
            this.map.ShiftBackward();
            break;
        }
      }
      finally
      {
        this.IsBusy = false;
      }
    }
  }
}
