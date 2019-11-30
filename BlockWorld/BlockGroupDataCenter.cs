// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlockGroupDataCenter
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  internal class BlockGroupDataCenter
  {
    public BlockGroupData Data;
    private Map map;
    private MapChunk chunk;
    private int mapIndex;
    private bool oldSkoolLight;
    private int chunkSizeX;
    private int chunkSizeZ;
    private int chunksizeX_1;
    private int chunksizeY_1;
    private int chunksizeZ_1;
    private int chunkPlaneSize;

    public BlockGroupDataCenter(Map map, MapChunk chunk, bool oldSkoolLight)
    {
      this.map = map;
      this.chunk = chunk;
      this.oldSkoolLight = oldSkoolLight;
      this.Data = new BlockGroupData();
      this.chunkSizeX = map.ChunkSize.X;
      this.chunkSizeZ = map.ChunkSize.Z;
      this.chunksizeX_1 = map.ChunkSize.X - 1;
      this.chunksizeY_1 = map.ChunkSize.Y - 1;
      this.chunksizeZ_1 = map.ChunkSize.Z - 1;
      this.chunkPlaneSize = this.chunkSizeX * this.chunkSizeZ;
      this.Data.Point = Point3D.One;
      this.mapIndex = this.chunkSizeX * this.chunkSizeZ + this.chunkSizeX + 1;
      this.GetData();
    }

    public bool MoveNext()
    {
      ++this.mapIndex;
      if (++this.Data.Point.X == this.chunksizeX_1)
      {
        this.mapIndex += 2;
        this.Data.Point.X = 1;
        if (++this.Data.Point.Z == this.chunksizeZ_1)
        {
          this.mapIndex += this.chunkSizeX;
          this.mapIndex += this.chunkSizeX;
          this.Data.Point.Z = 1;
          if (++this.Data.Point.Y == this.chunksizeY_1)
            return false;
        }
      }
      this.GetData();
      return true;
    }

    private void GetData()
    {
      this.Data.Center.BlockID = this.chunk.BlockData.GetData(this.chunk, this.mapIndex);
      this.Data.Center.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, this.mapIndex));
      this.Data.Center.AuxData = this.chunk.AuxData.GetData(this.chunk, this.mapIndex);
      int mapIndex1 = this.mapIndex - 1;
      this.Data.Left.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex1);
      this.Data.Left.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex1));
      this.Data.Left.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex1);
      int mapIndex2 = this.mapIndex - this.chunkSizeX;
      this.Data.Forward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex2);
      this.Data.Forward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex2));
      this.Data.Forward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex2);
      int mapIndex3 = this.mapIndex + 1;
      this.Data.Right.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex3);
      this.Data.Right.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex3));
      this.Data.Right.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex3);
      int mapIndex4 = this.mapIndex + this.chunkSizeX;
      this.Data.Backward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex4);
      this.Data.Backward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex4));
      this.Data.Backward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex4);
      int mapIndex5 = this.mapIndex + this.chunkPlaneSize;
      this.Data.Up.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex5);
      this.Data.Up.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex5));
      this.Data.Up.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex5);
      int mapIndex6 = this.mapIndex - this.chunkPlaneSize;
      this.Data.Down.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex6);
      this.Data.Down.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex6));
      this.Data.Down.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex6);
      if (this.oldSkoolLight)
        return;
      int mapIndex7 = mapIndex5 - this.chunkSizeX;
      this.Data.ForwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex7);
      this.Data.ForwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex7));
      this.Data.ForwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex7);
      int mapIndex8 = mapIndex6 - this.chunkSizeX;
      this.Data.ForwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex8);
      this.Data.ForwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex8));
      this.Data.ForwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex8);
      int mapIndex9 = mapIndex5 + this.chunkSizeX;
      this.Data.BackwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex9);
      this.Data.BackwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex9));
      this.Data.BackwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex9);
      int mapIndex10 = mapIndex6 + this.chunkSizeX;
      this.Data.BackwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex10);
      this.Data.BackwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex10));
      this.Data.BackwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex10);
      int mapIndex11 = mapIndex5 - 1;
      this.Data.LeftUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex11);
      this.Data.LeftUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex11));
      this.Data.LeftUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex11);
      int mapIndex12 = mapIndex6 - 1;
      this.Data.LeftDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex12);
      this.Data.LeftDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex12));
      this.Data.LeftDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex12);
      int mapIndex13 = mapIndex2 - 1;
      this.Data.LeftForward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex13);
      this.Data.LeftForward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex13));
      this.Data.LeftForward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex13);
      int mapIndex14 = mapIndex7 - 1;
      this.Data.LeftForwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex14);
      this.Data.LeftForwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex14));
      this.Data.LeftForwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex14);
      int mapIndex15 = mapIndex8 - 1;
      this.Data.LeftForwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex15);
      this.Data.LeftForwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex15));
      this.Data.LeftForwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex15);
      int mapIndex16 = mapIndex4 - 1;
      this.Data.LeftBackward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex16);
      this.Data.LeftBackward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex16));
      this.Data.LeftBackward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex16);
      int mapIndex17 = mapIndex9 - 1;
      this.Data.LeftBackwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex17);
      this.Data.LeftBackwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex17));
      this.Data.LeftBackwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex17);
      int mapIndex18 = mapIndex10 - 1;
      this.Data.LeftBackwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex18);
      this.Data.LeftBackwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex18));
      this.Data.LeftBackwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex18);
      int mapIndex19 = mapIndex2 + 1;
      this.Data.RightForward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex19);
      this.Data.RightForward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex19));
      this.Data.RightForward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex19);
      int mapIndex20 = mapIndex5 + 1;
      this.Data.RightUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex20);
      this.Data.RightUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex20));
      this.Data.RightUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex20);
      int mapIndex21 = mapIndex6 + 1;
      this.Data.RightDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex21);
      this.Data.RightDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex21));
      this.Data.RightDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex21);
      int mapIndex22 = mapIndex7 + 1;
      this.Data.RightForwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex22);
      this.Data.RightForwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex22));
      this.Data.RightForwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex22);
      int mapIndex23 = mapIndex8 + 1;
      this.Data.RightForwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex23);
      this.Data.RightForwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex23));
      this.Data.RightForwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex23);
      int mapIndex24 = mapIndex4 + 1;
      this.Data.RightBackward.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex24);
      this.Data.RightBackward.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex24));
      this.Data.RightBackward.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex24);
      int mapIndex25 = mapIndex9 + 1;
      this.Data.RightBackwardUp.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex25);
      this.Data.RightBackwardUp.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex25));
      this.Data.RightBackwardUp.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex25);
      int mapIndex26 = mapIndex10 + 1;
      this.Data.RightBackwardDown.BlockID = this.chunk.BlockData.GetData(this.chunk, mapIndex26);
      this.Data.RightBackwardDown.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, mapIndex26));
      this.Data.RightBackwardDown.AuxData = this.chunk.AuxData.GetData(this.chunk, mapIndex26);
    }
  }
}
