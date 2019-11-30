// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.LoadComponentResult
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner.Graphics
{
  internal class LoadComponentResult : IProgressBarRef
  {
    public System.Action<bool, object> Action;
    public MapModel Model;
    public VoxelModelManager VoxelModelManager;
    public string ErrorDesc;
    public object State;

    IProgressBar IProgressBarRef.ProgressBar
    {
      get
      {
        return this.State as IProgressBar;
      }
    }
  }
}
