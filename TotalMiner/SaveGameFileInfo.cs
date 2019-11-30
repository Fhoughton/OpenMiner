// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SaveGameFileInfo
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal class SaveGameFileInfo
  {
    public SaveMapHead Header = new SaveMapHead();
    public int FileSize;
    public int DirNumber;
    public MapType MapType;
    private bool isAutoSave;
    private string filename;

    public SaveGameFileInfo(MapType mapType)
    {
      this.MapType = mapType;
    }

    public bool IsAutoSave
    {
      get
      {
        if (this.Header == null)
          return this.isAutoSave;
        return this.Header.IsAutoSave;
      }
      set
      {
        this.isAutoSave = value;
        if (this.Header == null)
          return;
        this.Header.IsAutoSave = value;
      }
    }

    public string MapFilePath
    {
      get
      {
        return Globals2.GetMapFilePath(this.MapType, this.DirNumber, this.IsAutoSave);
      }
    }

    public string Filename
    {
      get
      {
        if (this.DirNumber != 0)
          return this.MapFilePath + "header.dat";
        return this.filename;
      }
      set
      {
        this.filename = this.DirNumber == 0 ? value : (string) null;
      }
    }
  }
}
