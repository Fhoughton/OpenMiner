// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.SaveDataOld
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class SaveDataOld
  {
    public List<SaveDataBlockOld> Changes = new List<SaveDataBlockOld>();
    public GlobalPoint3D MapSize;
    public int MapSeed;
    public bool ReverseY;
    public int Version;

    public void AddChange(GlobalPoint3D point, ushort blockData)
    {
      this.Changes.Add(new SaveDataBlockOld()
      {
        X = (ushort) point.X,
        Y = (ushort) -point.Y,
        Z = (ushort) point.Z,
        Data = blockData
      });
    }
  }
}
