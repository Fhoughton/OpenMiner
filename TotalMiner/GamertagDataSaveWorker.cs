// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GamertagDataSaveWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class GamertagDataSaveWorker : IThreadWorkItem
  {
    private bool merge;
    private bool saveHighscores;

    public string Name
    {
      get
      {
        return "GamertagDataSaver";
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

    public GamertagDataSaveWorker(bool saveHighscores, bool merge)
    {
      this.saveHighscores = saveHighscores;
      this.merge = merge;
    }

    public void Update()
    {
      Globals2.SaveGamertagData(this.saveHighscores, this.merge);
    }
  }
}
