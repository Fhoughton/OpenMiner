// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.RLEEnumerator
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  internal class RLEEnumerator
  {
    public MapBlock Data;
    public MapBlockX DataX;
    public MapChunk Chunk;
    private Map map;
    private RLEStreamByte blockData;
    private RLEStreamByte lightData;
    private RLEStreamByte auxData;
    private int mapIndex;
    private int chunkSize;
    private Point3D p;

    public RLEEnumerator(MapChunk chunk)
    {
      this.Chunk = chunk;
      this.map = chunk.Region.Map;
      this.p = Point3D.Zero;
      this.mapIndex = 0;
      this.blockData = chunk.BlockData;
      this.lightData = chunk.LightData;
      this.auxData = chunk.AuxData;
      this.Data.BlockID = this.blockData.GetData(chunk, this.mapIndex);
      this.Data.Light = MapLight.FromByte(this.lightData.GetData(chunk, this.mapIndex));
      this.Data.AuxData = (byte) ((uint) this.auxData.GetData(chunk, this.mapIndex) & 7U);
      this.DataX.Data = this.Data;
      this.DataX.Point = chunk.GetGlobalPoint(this.p);
      this.chunkSize = this.map.ChunkSize.X * this.map.ChunkSize.Y * this.map.ChunkSize.Z;
    }

    public bool MoveNext()
    {
      if (++this.mapIndex >= this.chunkSize)
        return false;
      this.DataX.Data.BlockID = this.Data.BlockID = this.blockData.GetData(this.Chunk, this.mapIndex);
      this.DataX.Data.Light = this.Data.Light = MapLight.FromByte(this.lightData.GetData(this.Chunk, this.mapIndex));
      this.DataX.Data.AuxData = this.Data.AuxData = (byte) ((uint) this.auxData.GetData(this.Chunk, this.mapIndex) & 7U);
      if (++this.p.X == this.map.ChunkSize.X)
      {
        this.p.X = 0;
        if (++this.p.Z == this.map.ChunkSize.Z)
        {
          this.p.Z = 0;
          ++this.p.Y;
        }
      }
      this.DataX.Point = this.Chunk.GetGlobalPoint(this.p);
      return true;
    }
  }
}
