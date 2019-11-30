// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlockGroupDataEdge
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  internal class BlockGroupDataEdge : BlockGroupDataSide
  {
    private Direction dir2;

    public BlockGroupDataEdge(
      Map map,
      MapChunk chunk,
      MapChunk chunk2,
      MapChunk chunk3,
      MapChunk chunk4,
      Direction dir,
      Direction dir2,
      bool oldSkoolLight)
    {
      this.map = map;
      this.chunk = chunk;
      this.dir = dir;
      this.dir2 = dir2;
      this.oldSkoolLight = oldSkoolLight;
      this.chunkSizeX = map.ChunkSize.X;
      this.chunkSizeZ = map.ChunkSize.Z;
      this.chunksizeX_1 = map.ChunkSize.X - 1;
      this.chunksizeY_1 = map.ChunkSize.Y - 1;
      this.chunksizeZ_1 = map.ChunkSize.Z - 1;
      this.chunkPlaneSize = this.chunkSizeX * this.chunkSizeZ;
      this.Data = new BlockGroupData();
      this.Data.Point = this.GetStartPoint(dir, dir2);
      this.increment = this.GetIncrement(dir, dir2);
      this.AssignChunks(chunk, chunk2, chunk3, chunk4, dir, dir2);
      this.AssignIndexes(chunk, dir, dir2);
      this.GetData();
    }

    public new bool MoveNext()
    {
      this.IncrementIndexes(this.increment);
      if (!this.IncrementPoint(this.dir, this.dir2))
        return false;
      this.GetData();
      return true;
    }

    private bool IncrementPoint(Direction dir, Direction dir2)
    {
      switch (dir)
      {
        case Direction.Left:
        case Direction.Right:
          switch (dir2)
          {
            case Direction.Forward:
            case Direction.Backward:
              return ++this.Data.Point.Y != this.chunksizeY_1;
            case Direction.Up:
            case Direction.Down:
              return ++this.Data.Point.Z != this.chunksizeZ_1;
            default:
              return false;
          }
        case Direction.Forward:
        case Direction.Backward:
          return ++this.Data.Point.X != this.chunksizeX_1;
        default:
          return false;
      }
    }

    private int GetIncrement(Direction dir, Direction dir2)
    {
      switch (dir)
      {
        case Direction.Left:
        case Direction.Right:
          switch (dir2)
          {
            case Direction.Forward:
            case Direction.Backward:
              return this.chunkPlaneSize;
            case Direction.Up:
            case Direction.Down:
              return this.chunkSizeX;
            default:
              return 0;
          }
        case Direction.Forward:
        case Direction.Backward:
          return 1;
        default:
          return 0;
      }
    }

    private Point3D GetStartPoint(Direction dir, Direction dir2)
    {
      switch (dir)
      {
        case Direction.Left:
          switch (dir2)
          {
            case Direction.Forward:
              return new Point3D(0, 1, 0);
            case Direction.Backward:
              return new Point3D(0, 1, this.chunksizeZ_1);
            case Direction.Up:
              return new Point3D(0, this.chunksizeY_1, 1);
            case Direction.Down:
              return new Point3D(0, 0, 1);
            default:
              return Point3D.Zero;
          }
        case Direction.Forward:
          switch (dir2)
          {
            case Direction.Up:
              return new Point3D(1, this.chunksizeY_1, 0);
            case Direction.Down:
              return new Point3D(1, 0, 0);
            default:
              return Point3D.Zero;
          }
        case Direction.Right:
          switch (dir2)
          {
            case Direction.Forward:
              return new Point3D(this.chunksizeX_1, 1, 0);
            case Direction.Backward:
              return new Point3D(this.chunksizeX_1, 1, this.chunksizeZ_1);
            case Direction.Up:
              return new Point3D(this.chunksizeX_1, this.chunksizeY_1, 1);
            case Direction.Down:
              return new Point3D(this.chunksizeX_1, 0, 1);
            default:
              return Point3D.Zero;
          }
        case Direction.Backward:
          switch (dir2)
          {
            case Direction.Up:
              return new Point3D(1, this.chunksizeY_1, this.chunksizeZ_1);
            case Direction.Down:
              return new Point3D(1, 0, this.chunksizeZ_1);
            default:
              return Point3D.Zero;
          }
        default:
          return Point3D.Zero;
      }
    }

    private void AssignIndexes(MapChunk chunk, Direction dir, Direction dir2)
    {
      this.AssignIndexes(chunk, dir);
      switch (dir2)
      {
        case Direction.Forward:
          this.forwardIndex += this.chunkPlaneSize;
          if (this.oldSkoolLight)
            break;
          this.leftForwardIndex += this.chunkPlaneSize;
          this.leftForwardUpIndex += this.chunkPlaneSize;
          this.leftForwardDownIndex += this.chunkPlaneSize;
          this.rightForwardIndex += this.chunkPlaneSize;
          this.rightForwardUpIndex += this.chunkPlaneSize;
          this.rightForwardDownIndex += this.chunkPlaneSize;
          this.forwardUpIndex += this.chunkPlaneSize;
          this.forwardDownIndex += this.chunkPlaneSize;
          break;
        case Direction.Backward:
          this.backwardIndex -= this.chunkPlaneSize;
          if (this.oldSkoolLight)
            break;
          this.leftBackwardIndex -= this.chunkPlaneSize;
          this.leftBackwardUpIndex -= this.chunkPlaneSize;
          this.leftBackwardDownIndex -= this.chunkPlaneSize;
          this.rightBackwardIndex -= this.chunkPlaneSize;
          this.rightBackwardUpIndex -= this.chunkPlaneSize;
          this.rightBackwardDownIndex -= this.chunkPlaneSize;
          this.backwardUpIndex -= this.chunkPlaneSize;
          this.backwardDownIndex -= this.chunkPlaneSize;
          break;
        case Direction.Up:
          int num1 = this.chunkPlaneSize * this.map.ChunkSize.Y;
          this.upIndex -= num1;
          if (this.oldSkoolLight)
            break;
          this.leftUpIndex -= num1;
          this.leftForwardUpIndex -= num1;
          this.leftBackwardUpIndex -= num1;
          this.rightUpIndex -= num1;
          this.rightForwardUpIndex -= num1;
          this.rightBackwardUpIndex -= num1;
          this.forwardUpIndex -= num1;
          this.backwardUpIndex -= num1;
          break;
        case Direction.Down:
          int num2 = this.chunkPlaneSize * this.map.ChunkSize.Y;
          this.downIndex += num2;
          if (this.oldSkoolLight)
            break;
          this.leftDownIndex += num2;
          this.leftForwardDownIndex += num2;
          this.leftBackwardDownIndex += num2;
          this.rightDownIndex += num2;
          this.rightForwardDownIndex += num2;
          this.rightBackwardDownIndex += num2;
          this.forwardDownIndex += num2;
          this.backwardDownIndex += num2;
          break;
      }
    }

    private void AssignChunks(
      MapChunk chunk,
      MapChunk chunk2,
      MapChunk chunk3,
      MapChunk chunk4,
      Direction dir,
      Direction dir2)
    {
      this.left = chunk;
      this.forward = chunk;
      this.right = chunk;
      this.backward = chunk;
      this.up = chunk;
      this.down = chunk;
      this.leftUp = chunk;
      this.leftDown = chunk;
      this.leftForward = chunk;
      this.leftBackward = chunk;
      this.leftForwardUp = chunk;
      this.leftForwardDown = chunk;
      this.leftBackwardUp = chunk;
      this.leftBackwardDown = chunk;
      this.rightUp = chunk;
      this.rightDown = chunk;
      this.rightForward = chunk;
      this.rightBackward = chunk;
      this.rightForwardUp = chunk;
      this.rightForwardDown = chunk;
      this.rightBackwardUp = chunk;
      this.rightBackwardDown = chunk;
      this.forwardUp = chunk;
      this.forwardDown = chunk;
      this.backwardUp = chunk;
      this.backwardDown = chunk;
      switch (dir)
      {
        case Direction.Left:
          this.left = chunk2;
          this.leftUp = chunk2;
          this.leftDown = chunk2;
          this.leftForward = chunk2;
          this.leftBackward = chunk2;
          this.leftForwardUp = chunk2;
          this.leftForwardDown = chunk2;
          this.leftBackwardUp = chunk2;
          this.leftBackwardDown = chunk2;
          switch (dir2)
          {
            case Direction.Forward:
              this.forward = chunk3;
              this.forwardUp = chunk3;
              this.forwardDown = chunk3;
              this.leftForward = chunk4;
              this.leftForwardUp = chunk4;
              this.leftForwardDown = chunk4;
              this.rightForward = chunk3;
              this.rightForwardUp = chunk3;
              this.rightForwardDown = chunk3;
              return;
            case Direction.Right:
              return;
            case Direction.Backward:
              this.backward = chunk3;
              this.leftBackward = chunk4;
              this.leftBackwardUp = chunk4;
              this.leftBackwardDown = chunk4;
              this.rightBackward = chunk3;
              this.rightBackwardUp = chunk3;
              this.rightBackwardDown = chunk3;
              this.backwardUp = chunk3;
              this.backwardDown = chunk3;
              return;
            case Direction.Up:
              this.up = chunk3;
              this.leftUp = chunk4;
              this.leftForwardUp = chunk4;
              this.leftBackwardUp = chunk4;
              this.rightUp = chunk3;
              this.rightForwardUp = chunk3;
              this.rightBackwardUp = chunk3;
              this.forwardUp = chunk3;
              this.backwardUp = chunk3;
              return;
            case Direction.Down:
              this.down = chunk3;
              this.leftDown = chunk4;
              this.leftForwardDown = chunk4;
              this.leftBackwardDown = chunk4;
              this.rightDown = chunk3;
              this.rightForwardDown = chunk3;
              this.rightBackwardDown = chunk3;
              this.forwardDown = chunk3;
              this.backwardDown = chunk3;
              return;
            default:
              return;
          }
        case Direction.Forward:
          this.forward = chunk2;
          this.forwardUp = chunk2;
          this.forwardDown = chunk2;
          this.leftForward = chunk2;
          this.leftForwardUp = chunk2;
          this.leftForwardDown = chunk2;
          this.rightForward = chunk2;
          this.rightForwardUp = chunk2;
          this.rightForwardDown = chunk2;
          switch (dir2)
          {
            case Direction.Up:
              this.up = chunk3;
              this.leftUp = chunk3;
              this.leftForwardUp = chunk4;
              this.leftBackwardUp = chunk3;
              this.rightUp = chunk3;
              this.rightForwardUp = chunk4;
              this.rightBackwardUp = chunk3;
              this.forwardUp = chunk4;
              this.backwardUp = chunk3;
              return;
            case Direction.Down:
              this.down = chunk3;
              this.leftDown = chunk3;
              this.leftForwardDown = chunk4;
              this.leftBackwardDown = chunk4;
              this.rightDown = chunk3;
              this.rightForwardDown = chunk4;
              this.rightBackwardDown = chunk3;
              this.forwardDown = chunk4;
              this.backwardDown = chunk3;
              return;
            default:
              return;
          }
        case Direction.Right:
          this.right = chunk2;
          this.rightUp = chunk2;
          this.rightDown = chunk2;
          this.rightForward = chunk2;
          this.rightBackward = chunk2;
          this.rightForwardUp = chunk2;
          this.rightForwardDown = chunk2;
          this.rightBackwardUp = chunk2;
          this.rightBackwardDown = chunk2;
          switch (dir2)
          {
            case Direction.Forward:
              this.forward = chunk3;
              this.forwardUp = chunk3;
              this.forwardDown = chunk3;
              this.leftForward = chunk3;
              this.leftForwardUp = chunk3;
              this.leftForwardDown = chunk3;
              this.rightForward = chunk4;
              this.rightForwardUp = chunk4;
              this.rightForwardDown = chunk4;
              return;
            case Direction.Right:
              return;
            case Direction.Backward:
              this.backward = chunk3;
              this.leftBackward = chunk3;
              this.leftBackwardUp = chunk3;
              this.leftBackwardDown = chunk3;
              this.rightBackward = chunk4;
              this.rightBackwardUp = chunk4;
              this.rightBackwardDown = chunk4;
              this.backwardUp = chunk3;
              this.backwardDown = chunk3;
              return;
            case Direction.Up:
              this.up = chunk3;
              this.leftUp = chunk3;
              this.leftForwardUp = chunk3;
              this.leftBackwardUp = chunk3;
              this.rightUp = chunk4;
              this.rightForwardUp = chunk4;
              this.rightBackwardUp = chunk4;
              this.forwardUp = chunk3;
              this.backwardUp = chunk3;
              return;
            case Direction.Down:
              this.down = chunk3;
              this.leftDown = chunk3;
              this.leftForwardDown = chunk3;
              this.leftBackwardDown = chunk3;
              this.rightDown = chunk4;
              this.rightForwardDown = chunk4;
              this.rightBackwardDown = chunk4;
              this.forwardDown = chunk3;
              this.backwardDown = chunk3;
              return;
            default:
              return;
          }
        case Direction.Backward:
          this.backward = chunk2;
          this.backwardUp = chunk2;
          this.backwardDown = chunk2;
          this.leftBackward = chunk2;
          this.leftBackwardUp = chunk2;
          this.leftBackwardDown = chunk2;
          this.rightBackward = chunk2;
          this.rightBackwardUp = chunk2;
          this.rightBackwardDown = chunk2;
          switch (dir2)
          {
            case Direction.Up:
              this.up = chunk3;
              this.leftUp = chunk3;
              this.rightUp = chunk3;
              this.forwardUp = chunk3;
              this.leftForwardUp = chunk3;
              this.rightForwardUp = chunk3;
              this.backwardUp = chunk4;
              this.leftBackwardUp = chunk4;
              this.rightBackwardUp = chunk4;
              return;
            case Direction.Down:
              this.down = chunk3;
              this.leftDown = chunk3;
              this.rightDown = chunk3;
              this.forwardDown = chunk3;
              this.leftForwardDown = chunk3;
              this.rightForwardDown = chunk3;
              this.backwardDown = chunk4;
              this.leftBackwardDown = chunk4;
              this.rightBackwardDown = chunk4;
              return;
            default:
              return;
          }
      }
    }
  }
}
