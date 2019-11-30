// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FilesizeBuilder
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Screens;

namespace StudioForge.TotalMiner
{
  internal class FilesizeBuilder : ThreadedWorkerBase
  {
    private int current;
    private MenuEntry[] entries;

    public FilesizeBuilder()
      : base(0)
    {
    }

    public void Start(MenuEntry[] entries)
    {
      this.entries = entries;
      this.Start();
    }

    protected override void ThreadedUpdateCore()
    {
      if (this.current < this.entries.Length)
      {
        GameFileMenuEntry entry = this.entries[this.current] as GameFileMenuEntry;
        if (entry != null)
        {
          entry.GameInfo.FileSize = Globals2.CalcFileSize(entry.GameInfo);
          entry.FileSizeCalculated = true;
        }
        ++this.current;
      }
      else
        this.run = false;
    }
  }
}
