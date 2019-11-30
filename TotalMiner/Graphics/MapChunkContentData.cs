// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.MapChunkContentData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct MapChunkContentData
  {
    public VertexBuffer VertexBuffer;
    public VertexBuffer NewVertexBuffer;
    public bool VertexBufferChanged;
    public int VertexCount;
    public int NewVertexCount;
    public int WaterVertexCount;
    public int NewWaterVertexCount;
  }
}
