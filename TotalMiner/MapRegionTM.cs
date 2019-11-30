// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapRegionTM
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  internal class MapRegionTM : MapRegion
  {
    public int LastGoodSaveNum = -1;

    public int TotalMeshSize
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.Chunks.Length; ++index)
        {
          MapChunk chunk = this.Chunks[index];
          if (chunk != null)
            num += ((MapChunkTM) chunk).TotalMeshSize;
        }
        return num;
      }
    }

    protected override MapChunk CreateChunk(Point3D offset)
    {
      MapChunkTM mapChunkTm = new MapChunkTM((MapRegion) this, offset);
      mapChunkTm.LoadContent((InitState) null);
      return (MapChunk) mapChunkTm;
    }
  }
}
