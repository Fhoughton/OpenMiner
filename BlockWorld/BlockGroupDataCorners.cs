// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlockGroupDataCorners
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  internal class BlockGroupDataCorners : BlockGroupDataSide
  {
    private BlockGroupDataCorners.Corner corner;

    public BlockGroupDataCorners(Map map, MapChunk chunk, bool oldSkoolLight)
    {
      this.map = map;
      this.chunk = chunk;
      this.oldSkoolLight = oldSkoolLight;
      this.chunkSizeX = map.ChunkSize.X;
      this.chunkSizeZ = map.ChunkSize.Z;
      this.chunksizeX_1 = map.ChunkSize.X - 1;
      this.chunksizeY_1 = map.ChunkSize.Y - 1;
      this.chunksizeZ_1 = map.ChunkSize.Z - 1;
      this.chunkPlaneSize = this.chunkSizeX * this.chunkSizeZ;
      this.Data = new BlockGroupData();
      this.corner = BlockGroupDataCorners.Corner.LeftForwardDown;
      this.AssignChunks(this.corner);
      this.AssignIndexes(this.corner);
      this.GetData();
    }

    public new bool MoveNext()
    {
      if (++this.corner > BlockGroupDataCorners.Corner.LeftBackwardUp)
        return false;
      this.AssignChunks(this.corner);
      this.AssignIndexes(this.corner);
      this.GetData();
      return true;
    }

    private void AssignChunks(BlockGroupDataCorners.Corner corner)
    {
      this.left = this.chunk;
      this.forward = this.chunk;
      this.right = this.chunk;
      this.backward = this.chunk;
      this.up = this.chunk;
      this.down = this.chunk;
      switch (corner)
      {
        case BlockGroupDataCorners.Corner.LeftForwardDown:
          this.left = this.chunk.LeftNeighbour();
          this.forward = this.chunk.ForwardNeighbour();
          this.down = this.chunk.DownNeighbour();
          break;
        case BlockGroupDataCorners.Corner.RightForwardDown:
          this.right = this.chunk.RightNeighbour();
          this.forward = this.chunk.ForwardNeighbour();
          this.down = this.chunk.DownNeighbour();
          break;
        case BlockGroupDataCorners.Corner.RightForwardUp:
          this.right = this.chunk.RightNeighbour();
          this.forward = this.chunk.ForwardNeighbour();
          this.up = this.chunk.UpNeighbour();
          break;
        case BlockGroupDataCorners.Corner.LeftForwardUp:
          this.left = this.chunk.LeftNeighbour();
          this.forward = this.chunk.ForwardNeighbour();
          this.up = this.chunk.UpNeighbour();
          break;
        case BlockGroupDataCorners.Corner.LeftBackwardDown:
          this.left = this.chunk.LeftNeighbour();
          this.backward = this.chunk.BackwardNeighbour();
          this.down = this.chunk.DownNeighbour();
          break;
        case BlockGroupDataCorners.Corner.RightBackwardDown:
          this.right = this.chunk.RightNeighbour();
          this.backward = this.chunk.BackwardNeighbour();
          this.down = this.chunk.DownNeighbour();
          break;
        case BlockGroupDataCorners.Corner.RightBackwardUp:
          this.right = this.chunk.RightNeighbour();
          this.backward = this.chunk.BackwardNeighbour();
          this.up = this.chunk.UpNeighbour();
          break;
        case BlockGroupDataCorners.Corner.LeftBackwardUp:
          this.left = this.chunk.LeftNeighbour();
          this.backward = this.chunk.BackwardNeighbour();
          this.up = this.chunk.UpNeighbour();
          break;
      }
      if (this.oldSkoolLight)
        return;
      this.leftUp = (MapChunk) null;
      this.leftDown = (MapChunk) null;
      this.leftForward = (MapChunk) null;
      this.leftBackward = (MapChunk) null;
      this.leftForwardUp = (MapChunk) null;
      this.leftForwardDown = (MapChunk) null;
      this.leftBackwardUp = (MapChunk) null;
      this.leftBackwardDown = (MapChunk) null;
      this.rightUp = (MapChunk) null;
      this.rightDown = (MapChunk) null;
      this.rightForward = (MapChunk) null;
      this.rightBackward = (MapChunk) null;
      this.rightForwardUp = (MapChunk) null;
      this.rightForwardDown = (MapChunk) null;
      this.rightBackwardUp = (MapChunk) null;
      this.rightBackwardDown = (MapChunk) null;
      this.forwardUp = (MapChunk) null;
      this.forwardDown = (MapChunk) null;
      this.backwardUp = (MapChunk) null;
      this.backwardDown = (MapChunk) null;
      if (this.left != null)
      {
        this.leftUp = this.left.UpNeighbour();
        this.leftDown = this.left.DownNeighbour();
        this.leftForward = this.left.ForwardNeighbour();
        this.leftBackward = this.left.BackwardNeighbour();
        if (this.leftForward != null)
        {
          this.leftForwardUp = this.leftForward.UpNeighbour();
          this.leftForwardDown = this.leftForward.DownNeighbour();
        }
        if (this.leftBackward != null)
        {
          this.leftBackwardUp = this.leftBackward.UpNeighbour();
          this.leftBackwardDown = this.leftBackward.DownNeighbour();
        }
      }
      if (this.right != null)
      {
        this.rightUp = this.right.UpNeighbour();
        this.rightDown = this.right.DownNeighbour();
        this.rightForward = this.right.ForwardNeighbour();
        this.rightBackward = this.right.BackwardNeighbour();
        if (this.rightForward != null)
        {
          this.rightForwardUp = this.rightForward.UpNeighbour();
          this.rightForwardDown = this.rightForward.DownNeighbour();
        }
        if (this.rightBackward != null)
        {
          this.rightBackwardUp = this.rightBackward.UpNeighbour();
          this.rightBackwardDown = this.rightBackward.DownNeighbour();
        }
      }
      if (this.forward != null)
      {
        this.forwardUp = this.forward.UpNeighbour();
        this.forwardDown = this.forward.DownNeighbour();
      }
      if (this.backward == null)
        return;
      this.backwardUp = this.backward.UpNeighbour();
      this.backwardDown = this.backward.DownNeighbour();
    }

    private void AssignIndexes(BlockGroupDataCorners.Corner corner)
    {
      switch (corner)
      {
        case BlockGroupDataCorners.Corner.LeftForwardDown:
          this.Data.Point.X = this.Data.Point.Y = this.Data.Point.Z = 0;
          this.mapIndex = this.GetLeftForwardDownIndex();
          this.leftIndex = this.GetRightForwardDownIndex();
          this.forwardIndex = this.GetLeftBackwardDownIndex();
          this.rightIndex = this.mapIndex + 1;
          this.backwardIndex = this.mapIndex + this.chunkSizeX;
          this.upIndex = this.mapIndex + this.chunkPlaneSize;
          this.downIndex = this.GetLeftForwardUpIndex();
          this.forwardUpIndex = this.forwardIndex + this.chunkPlaneSize;
          this.forwardDownIndex = this.GetLeftBackwardUpIndex();
          this.backwardUpIndex = this.upIndex + this.chunkSizeX;
          this.backwardDownIndex = this.downIndex + this.chunkSizeX;
          this.leftForwardIndex = this.GetRightBackwardDownIndex();
          this.leftBackwardIndex = this.leftIndex + this.chunkSizeX;
          this.leftUpIndex = this.leftIndex + this.chunkPlaneSize;
          this.leftDownIndex = this.GetRightForwardUpIndex();
          this.leftForwardUpIndex = this.leftForwardIndex + this.chunkPlaneSize;
          this.leftForwardDownIndex = this.GetRightBackwardUpIndex();
          this.leftBackwardUpIndex = this.upIndex + this.chunkSizeX;
          this.leftBackwardDownIndex = this.leftDownIndex + this.chunkSizeX;
          this.rightForwardIndex = this.forwardIndex + 1;
          this.rightBackwardIndex = this.backwardIndex + 1;
          this.rightUpIndex = this.upIndex + 1;
          this.rightDownIndex = this.downIndex + 1;
          this.rightForwardUpIndex = this.forwardUpIndex + 1;
          this.rightForwardDownIndex = this.forwardDownIndex + 1;
          this.rightBackwardUpIndex = this.backwardUpIndex + 1;
          this.rightBackwardDownIndex = this.backwardDownIndex + 1;
          break;
        case BlockGroupDataCorners.Corner.RightForwardDown:
          this.Data.Point.X = this.chunksizeX_1;
          this.mapIndex = this.GetRightForwardDownIndex();
          this.leftIndex = this.mapIndex - 1;
          this.forwardIndex = this.GetRightBackwardDownIndex();
          this.rightIndex = this.GetLeftForwardDownIndex();
          this.backwardIndex = this.mapIndex + this.chunkSizeX;
          this.upIndex = this.mapIndex + this.chunkPlaneSize;
          this.downIndex = this.GetRightForwardUpIndex();
          this.forwardUpIndex = this.forwardIndex + this.chunkPlaneSize;
          this.forwardDownIndex = this.GetRightBackwardUpIndex();
          this.backwardUpIndex = this.upIndex + this.chunkSizeX;
          this.backwardDownIndex = this.downIndex + this.chunkSizeX;
          this.leftForwardIndex = this.forwardIndex - 1;
          this.leftBackwardIndex = this.backwardIndex - 1;
          this.leftUpIndex = this.upIndex - 1;
          this.leftDownIndex = this.downIndex - 1;
          this.leftForwardUpIndex = this.forwardUpIndex - 1;
          this.leftForwardDownIndex = this.forwardDownIndex - 1;
          this.leftBackwardUpIndex = this.backwardUpIndex - 1;
          this.leftBackwardDownIndex = this.backwardDownIndex - 1;
          this.rightForwardIndex = this.GetLeftBackwardDownIndex();
          this.rightBackwardIndex = this.rightIndex + this.chunkSizeX;
          this.rightUpIndex = this.rightIndex + this.chunkPlaneSize;
          this.rightDownIndex = this.GetLeftForwardUpIndex();
          this.rightForwardUpIndex = this.rightForwardIndex + this.chunkPlaneSize;
          this.rightForwardDownIndex = this.GetLeftBackwardUpIndex();
          this.rightBackwardUpIndex = this.rightIndex + this.chunkSizeX;
          this.rightBackwardDownIndex = this.rightDownIndex + this.chunkSizeX;
          break;
        case BlockGroupDataCorners.Corner.RightForwardUp:
          this.Data.Point.Y = this.chunksizeY_1;
          this.mapIndex = this.GetRightForwardUpIndex();
          this.leftIndex = this.mapIndex - 1;
          this.forwardIndex = this.GetRightBackwardUpIndex();
          this.rightIndex = this.GetLeftForwardUpIndex();
          this.backwardIndex = this.mapIndex + this.chunkSizeX;
          this.upIndex = this.GetRightForwardDownIndex();
          this.downIndex = this.mapIndex - this.chunkPlaneSize;
          this.forwardUpIndex = this.GetRightBackwardDownIndex();
          this.forwardDownIndex = this.forwardIndex - this.chunkPlaneSize;
          this.backwardUpIndex = this.upIndex + this.chunkSizeX;
          this.backwardDownIndex = this.downIndex + this.chunkSizeX;
          this.leftForwardIndex = this.forwardIndex - 1;
          this.leftBackwardIndex = this.backwardIndex - 1;
          this.leftUpIndex = this.upIndex - 1;
          this.leftDownIndex = this.downIndex - 1;
          this.leftForwardUpIndex = this.forwardUpIndex - 1;
          this.leftForwardDownIndex = this.forwardDownIndex - 1;
          this.leftBackwardUpIndex = this.backwardUpIndex - 1;
          this.leftBackwardDownIndex = this.backwardDownIndex - 1;
          this.rightForwardIndex = this.GetLeftBackwardUpIndex();
          this.rightBackwardIndex = this.rightIndex + this.chunkSizeX;
          this.rightUpIndex = this.GetLeftForwardDownIndex();
          this.rightDownIndex = this.rightIndex - this.chunkPlaneSize;
          this.rightForwardUpIndex = this.GetLeftBackwardDownIndex();
          this.rightForwardDownIndex = this.rightForwardIndex - this.chunkPlaneSize;
          this.rightBackwardUpIndex = this.rightUpIndex + this.chunkSizeX;
          this.rightBackwardDownIndex = this.rightDownIndex + this.chunkSizeX;
          break;
        case BlockGroupDataCorners.Corner.LeftForwardUp:
          this.Data.Point.X = 0;
          this.mapIndex = this.GetLeftForwardUpIndex();
          this.leftIndex = this.GetRightForwardUpIndex();
          this.forwardIndex = this.GetLeftBackwardUpIndex();
          this.rightIndex = this.mapIndex + 1;
          this.backwardIndex = this.mapIndex + this.chunkSizeX;
          this.upIndex = this.GetLeftForwardDownIndex();
          this.downIndex = this.mapIndex - this.chunkPlaneSize;
          this.forwardUpIndex = this.GetLeftBackwardDownIndex();
          this.forwardDownIndex = this.forwardIndex - this.chunkPlaneSize;
          this.backwardUpIndex = this.upIndex + this.chunkSizeX;
          this.backwardDownIndex = this.downIndex + this.chunkSizeX;
          this.leftForwardIndex = this.GetRightBackwardUpIndex();
          this.leftBackwardIndex = this.leftIndex + this.chunkSizeX;
          this.leftUpIndex = this.GetRightForwardDownIndex();
          this.leftDownIndex = this.leftIndex - this.chunkPlaneSize;
          this.leftForwardUpIndex = this.GetRightBackwardDownIndex();
          this.leftForwardDownIndex = this.leftForwardIndex - this.chunkPlaneSize;
          this.leftBackwardUpIndex = this.leftUpIndex + this.chunkSizeX;
          this.leftBackwardDownIndex = this.leftDownIndex + this.chunkSizeX;
          this.rightForwardIndex = this.forwardIndex + 1;
          this.rightBackwardIndex = this.backwardIndex + 1;
          this.rightUpIndex = this.upIndex + 1;
          this.rightDownIndex = this.downIndex + 1;
          this.rightForwardUpIndex = this.forwardUpIndex + 1;
          this.rightForwardDownIndex = this.forwardDownIndex + 1;
          this.rightBackwardUpIndex = this.backwardUpIndex + 1;
          this.rightBackwardDownIndex = this.backwardDownIndex + 1;
          break;
        case BlockGroupDataCorners.Corner.LeftBackwardDown:
          this.Data.Point.Y = 0;
          this.Data.Point.Z = this.chunksizeZ_1;
          this.mapIndex = this.GetLeftBackwardDownIndex();
          this.leftIndex = this.GetRightBackwardDownIndex();
          this.forwardIndex = this.mapIndex - this.chunkSizeX;
          this.rightIndex = this.mapIndex + 1;
          this.backwardIndex = this.GetLeftForwardDownIndex();
          this.upIndex = this.mapIndex + this.chunkPlaneSize;
          this.downIndex = this.GetLeftBackwardUpIndex();
          this.forwardUpIndex = this.upIndex - this.chunkSizeX;
          this.forwardDownIndex = this.downIndex - this.chunkSizeX;
          this.backwardUpIndex = this.backwardIndex + this.chunkPlaneSize;
          this.backwardDownIndex = this.GetLeftForwardUpIndex();
          this.leftForwardIndex = this.leftIndex - this.chunkSizeX;
          this.leftBackwardIndex = this.GetRightForwardDownIndex();
          this.leftUpIndex = this.leftIndex + this.chunkPlaneSize;
          this.leftDownIndex = this.GetRightBackwardUpIndex();
          this.leftForwardUpIndex = this.leftForwardIndex + this.chunkPlaneSize;
          this.leftForwardDownIndex = this.leftDownIndex - this.chunkSizeX;
          this.leftBackwardUpIndex = this.leftBackwardIndex + this.chunkPlaneSize;
          this.leftBackwardDownIndex = this.GetRightForwardUpIndex();
          this.rightForwardIndex = this.forwardIndex + 1;
          this.rightBackwardIndex = this.backwardIndex + 1;
          this.rightUpIndex = this.upIndex + 1;
          this.rightDownIndex = this.downIndex + 1;
          this.rightForwardUpIndex = this.forwardUpIndex + 1;
          this.rightForwardDownIndex = this.forwardDownIndex + 1;
          this.rightBackwardUpIndex = this.backwardUpIndex + 1;
          this.rightBackwardDownIndex = this.backwardDownIndex + 1;
          break;
        case BlockGroupDataCorners.Corner.RightBackwardDown:
          this.Data.Point.X = this.chunksizeX_1;
          this.mapIndex = this.GetRightBackwardDownIndex();
          this.leftIndex = this.mapIndex - 1;
          this.forwardIndex = this.mapIndex - this.chunkSizeX;
          this.rightIndex = this.GetLeftBackwardDownIndex();
          this.backwardIndex = this.GetRightForwardDownIndex();
          this.upIndex = this.mapIndex + this.chunkPlaneSize;
          this.downIndex = this.GetRightBackwardUpIndex();
          this.forwardUpIndex = this.forwardIndex + this.chunkPlaneSize;
          this.forwardDownIndex = this.downIndex - this.chunkSizeX;
          this.backwardUpIndex = this.backwardIndex + this.chunkPlaneSize;
          this.backwardDownIndex = this.GetRightForwardUpIndex();
          this.leftForwardIndex = this.forwardIndex - 1;
          this.leftBackwardIndex = this.backwardIndex - 1;
          this.leftUpIndex = this.upIndex - 1;
          this.leftDownIndex = this.downIndex - 1;
          this.leftForwardUpIndex = this.forwardUpIndex - 1;
          this.leftForwardDownIndex = this.forwardDownIndex - 1;
          this.leftBackwardUpIndex = this.backwardUpIndex - 1;
          this.leftBackwardDownIndex = this.backwardDownIndex - 1;
          this.rightForwardIndex = this.rightIndex - this.chunkSizeX;
          this.rightBackwardIndex = this.GetLeftForwardDownIndex();
          this.rightUpIndex = this.rightIndex + this.chunkPlaneSize;
          this.rightDownIndex = this.GetLeftBackwardUpIndex();
          this.rightForwardUpIndex = this.rightForwardIndex + this.chunkPlaneSize;
          this.rightForwardDownIndex = this.rightDownIndex - this.chunkSizeX;
          this.rightBackwardUpIndex = this.rightBackwardIndex + this.chunkPlaneSize;
          this.rightBackwardDownIndex = this.GetLeftForwardUpIndex();
          break;
        case BlockGroupDataCorners.Corner.RightBackwardUp:
          this.Data.Point.Y = this.chunksizeY_1;
          this.mapIndex = this.GetRightBackwardUpIndex();
          this.leftIndex = this.mapIndex - 1;
          this.forwardIndex = this.mapIndex - this.chunkSizeX;
          this.rightIndex = this.GetLeftBackwardUpIndex();
          this.backwardIndex = this.GetRightForwardUpIndex();
          this.upIndex = this.GetRightBackwardDownIndex();
          this.downIndex = this.mapIndex - this.chunkPlaneSize;
          this.forwardUpIndex = this.upIndex - this.chunkSizeX;
          this.forwardDownIndex = this.downIndex - this.chunkSizeX;
          this.backwardUpIndex = this.GetRightForwardDownIndex();
          this.backwardDownIndex = this.backwardIndex - this.chunkPlaneSize;
          this.leftForwardIndex = this.forwardIndex - 1;
          this.leftBackwardIndex = this.backwardIndex - 1;
          this.leftUpIndex = this.upIndex - 1;
          this.leftDownIndex = this.downIndex - 1;
          this.leftForwardUpIndex = this.forwardUpIndex - 1;
          this.leftForwardDownIndex = this.forwardDownIndex - 1;
          this.leftBackwardUpIndex = this.backwardUpIndex - 1;
          this.leftBackwardDownIndex = this.backwardDownIndex - 1;
          this.rightForwardIndex = this.rightIndex - this.chunkSizeX;
          this.rightBackwardIndex = this.GetLeftForwardUpIndex();
          this.rightUpIndex = this.GetLeftBackwardDownIndex();
          this.rightDownIndex = this.rightIndex - this.chunkPlaneSize;
          this.rightForwardUpIndex = this.rightUpIndex - this.chunkSizeX;
          this.rightForwardDownIndex = this.rightDownIndex - this.chunkSizeX;
          this.rightBackwardUpIndex = this.GetLeftForwardDownIndex();
          this.rightBackwardDownIndex = this.rightBackwardIndex - this.chunkPlaneSize;
          break;
        case BlockGroupDataCorners.Corner.LeftBackwardUp:
          this.Data.Point.X = 0;
          this.mapIndex = this.GetLeftBackwardUpIndex();
          this.leftIndex = this.GetRightBackwardUpIndex();
          this.forwardIndex = this.mapIndex - this.chunkSizeX;
          this.rightIndex = this.mapIndex + 1;
          this.backwardIndex = this.GetLeftForwardUpIndex();
          this.upIndex = this.GetLeftBackwardDownIndex();
          this.downIndex = this.mapIndex - this.chunkPlaneSize;
          this.forwardUpIndex = this.upIndex - this.chunkSizeX;
          this.forwardDownIndex = this.downIndex - this.chunkSizeX;
          this.backwardUpIndex = this.GetLeftForwardDownIndex();
          this.backwardDownIndex = this.backwardIndex - this.chunkPlaneSize;
          this.leftForwardIndex = this.leftIndex - this.chunkSizeX;
          this.leftBackwardIndex = this.GetRightForwardUpIndex();
          this.leftUpIndex = this.GetRightBackwardDownIndex();
          this.leftDownIndex = this.leftIndex - this.chunkPlaneSize;
          this.leftForwardUpIndex = this.leftUpIndex - this.chunkSizeX;
          this.leftForwardDownIndex = this.leftForwardIndex - this.chunkPlaneSize;
          this.leftBackwardUpIndex = this.GetRightForwardUpIndex();
          this.leftBackwardDownIndex = this.leftBackwardIndex - this.chunkPlaneSize;
          this.rightForwardIndex = this.forwardIndex + 1;
          this.rightBackwardIndex = this.backwardIndex + 1;
          this.rightUpIndex = this.upIndex + 1;
          this.rightDownIndex = this.downIndex + 1;
          this.rightForwardUpIndex = this.forwardUpIndex + 1;
          this.rightForwardDownIndex = this.forwardDownIndex + 1;
          this.rightBackwardUpIndex = this.backwardUpIndex + 1;
          this.rightBackwardDownIndex = this.backwardDownIndex + 1;
          break;
      }
    }

    private int GetLeftForwardDownIndex()
    {
      return 0;
    }

    private int GetRightForwardDownIndex()
    {
      return this.chunksizeX_1;
    }

    private int GetRightForwardUpIndex()
    {
      return this.chunkPlaneSize * this.chunksizeY_1 + this.chunksizeX_1;
    }

    private int GetLeftForwardUpIndex()
    {
      return this.chunkPlaneSize * this.chunksizeY_1;
    }

    private int GetLeftBackwardDownIndex()
    {
      return this.chunkPlaneSize - this.chunkSizeX;
    }

    private int GetRightBackwardDownIndex()
    {
      return this.chunkPlaneSize - 1;
    }

    private int GetRightBackwardUpIndex()
    {
      return this.chunkPlaneSize * this.map.ChunkSize.Y - 1;
    }

    private int GetLeftBackwardUpIndex()
    {
      return this.chunkPlaneSize * this.map.ChunkSize.Y - this.map.ChunkSize.X;
    }

    private enum Corner
    {
      LeftForwardDown,
      RightForwardDown,
      RightForwardUp,
      LeftForwardUp,
      LeftBackwardDown,
      RightBackwardDown,
      RightBackwardUp,
      LeftBackwardUp,
    }
  }
}
