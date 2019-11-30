// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlockGroupDataSide
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  internal class BlockGroupDataSide
  {
    public BlockGroupData Data;
    protected MapChunk chunk;
    protected MapChunk left;
    protected MapChunk forward;
    protected MapChunk right;
    protected MapChunk backward;
    protected MapChunk up;
    protected MapChunk down;
    protected MapChunk leftForward;
    protected MapChunk leftBackward;
    protected MapChunk leftUp;
    protected MapChunk leftDown;
    protected MapChunk leftForwardUp;
    protected MapChunk leftForwardDown;
    protected MapChunk leftBackwardUp;
    protected MapChunk leftBackwardDown;
    protected MapChunk rightForward;
    protected MapChunk rightBackward;
    protected MapChunk rightUp;
    protected MapChunk rightDown;
    protected MapChunk rightForwardUp;
    protected MapChunk rightForwardDown;
    protected MapChunk rightBackwardUp;
    protected MapChunk rightBackwardDown;
    protected MapChunk forwardUp;
    protected MapChunk forwardDown;
    protected MapChunk backwardUp;
    protected MapChunk backwardDown;
    protected int mapIndex;
    protected int leftIndex;
    protected int forwardIndex;
    protected int rightIndex;
    protected int backwardIndex;
    protected int upIndex;
    protected int downIndex;
    protected int leftForwardIndex;
    protected int leftBackwardIndex;
    protected int leftUpIndex;
    protected int leftDownIndex;
    protected int leftForwardUpIndex;
    protected int leftForwardDownIndex;
    protected int leftBackwardUpIndex;
    protected int leftBackwardDownIndex;
    protected int rightForwardIndex;
    protected int rightBackwardIndex;
    protected int rightUpIndex;
    protected int rightDownIndex;
    protected int rightForwardUpIndex;
    protected int rightForwardDownIndex;
    protected int rightBackwardUpIndex;
    protected int rightBackwardDownIndex;
    protected int forwardUpIndex;
    protected int forwardDownIndex;
    protected int backwardUpIndex;
    protected int backwardDownIndex;
    protected Map map;
    protected int increment;
    protected Direction dir;
    protected bool oldSkoolLight;
    protected int chunkSizeX;
    protected int chunkSizeZ;
    protected int chunksizeX_1;
    protected int chunksizeY_1;
    protected int chunksizeZ_1;
    protected int chunkPlaneSize;
    private MapBlock getDataResult;

    public BlockGroupDataSide()
    {
    }

    public BlockGroupDataSide(
      Map map,
      MapChunk chunk,
      MapChunk chunk2,
      Direction dir,
      bool oldSkoolLight)
    {
      this.map = map;
      this.chunk = chunk;
      this.dir = dir;
      this.oldSkoolLight = oldSkoolLight;
      this.chunkSizeX = map.ChunkSize.X;
      this.chunkSizeZ = map.ChunkSize.Z;
      this.chunksizeX_1 = map.ChunkSize.X - 1;
      this.chunksizeY_1 = map.ChunkSize.Y - 1;
      this.chunksizeZ_1 = map.ChunkSize.Z - 1;
      this.chunkPlaneSize = this.chunkSizeX * this.chunkSizeZ;
      this.Data = new BlockGroupData();
      this.Data.Point = this.GetStartPoint(dir);
      this.increment = this.GetIncrement(dir);
      this.AssignChunks(chunk, chunk2, dir);
      this.AssignIndexes(chunk, dir);
      this.GetData();
    }

    public bool MoveNext()
    {
      this.IncrementIndexes(this.increment);
      if (!this.IncrementPoint(this.dir))
        return false;
      this.GetData();
      return true;
    }

    protected void IncrementIndexes(int increment)
    {
      this.mapIndex += increment;
      this.leftIndex += increment;
      this.forwardIndex += increment;
      this.rightIndex += increment;
      this.backwardIndex += increment;
      this.upIndex += increment;
      this.downIndex += increment;
      if (this.oldSkoolLight)
        return;
      this.leftUpIndex += increment;
      this.leftDownIndex += increment;
      this.leftForwardIndex += increment;
      this.leftForwardUpIndex += increment;
      this.leftForwardDownIndex += increment;
      this.leftBackwardIndex += increment;
      this.leftBackwardUpIndex += increment;
      this.leftBackwardDownIndex += increment;
      this.rightUpIndex += increment;
      this.rightDownIndex += increment;
      this.rightForwardIndex += increment;
      this.rightForwardUpIndex += increment;
      this.rightForwardDownIndex += increment;
      this.rightBackwardIndex += increment;
      this.rightBackwardUpIndex += increment;
      this.rightBackwardDownIndex += increment;
      this.forwardUpIndex += increment;
      this.forwardDownIndex += increment;
      this.backwardUpIndex += increment;
      this.backwardDownIndex += increment;
    }

    protected void GetData()
    {
      this.Data.Center.BlockID = this.chunk.BlockData.GetData(this.chunk, this.mapIndex);
      this.Data.Center.Light = MapLight.FromByte(this.chunk.LightData.GetData(this.chunk, this.mapIndex));
      this.Data.Center.AuxData = this.chunk.AuxData.GetData(this.chunk, this.mapIndex);
      this.Data.Left = this.GetData(this.left, this.leftIndex);
      this.Data.Forward = this.GetData(this.forward, this.forwardIndex);
      this.Data.Right = this.GetData(this.right, this.rightIndex);
      this.Data.Backward = this.GetData(this.backward, this.backwardIndex);
      this.Data.Up = this.GetData(this.up, this.upIndex);
      this.Data.Down = this.GetData(this.down, this.downIndex);
      if (this.oldSkoolLight)
        return;
      this.Data.LeftUp = this.GetData(this.leftUp, this.leftUpIndex);
      this.Data.LeftDown = this.GetData(this.leftDown, this.leftDownIndex);
      this.Data.LeftForward = this.GetData(this.leftForward, this.leftForwardIndex);
      this.Data.LeftForwardUp = this.GetData(this.leftForwardUp, this.leftForwardUpIndex);
      this.Data.LeftForwardDown = this.GetData(this.leftForwardDown, this.leftForwardDownIndex);
      this.Data.LeftBackward = this.GetData(this.leftBackward, this.leftBackwardIndex);
      this.Data.LeftBackwardUp = this.GetData(this.leftBackwardUp, this.leftBackwardUpIndex);
      this.Data.LeftBackwardDown = this.GetData(this.leftBackwardDown, this.leftBackwardDownIndex);
      this.Data.RightUp = this.GetData(this.rightUp, this.rightUpIndex);
      this.Data.RightDown = this.GetData(this.rightDown, this.rightDownIndex);
      this.Data.RightForward = this.GetData(this.rightForward, this.rightForwardIndex);
      this.Data.RightForwardUp = this.GetData(this.rightForwardUp, this.rightForwardUpIndex);
      this.Data.RightForwardDown = this.GetData(this.rightForwardDown, this.rightForwardDownIndex);
      this.Data.RightBackward = this.GetData(this.rightBackward, this.rightBackwardIndex);
      this.Data.RightBackwardUp = this.GetData(this.rightBackwardUp, this.rightBackwardUpIndex);
      this.Data.RightBackwardDown = this.GetData(this.rightBackwardDown, this.rightBackwardDownIndex);
      this.Data.ForwardUp = this.GetData(this.forwardUp, this.forwardUpIndex);
      this.Data.ForwardDown = this.GetData(this.forwardDown, this.forwardDownIndex);
      this.Data.BackwardUp = this.GetData(this.backwardUp, this.backwardUpIndex);
      this.Data.BackwardDown = this.GetData(this.backwardDown, this.backwardDownIndex);
    }

    protected MapBlock GetData(MapChunk chunk, int index)
    {
      if (chunk != null)
      {
        this.getDataResult.BlockID = chunk.BlockData.GetData(chunk, index);
        this.getDataResult.Light = MapLight.FromByte(chunk.LightData.GetData(chunk, index));
        this.getDataResult.AuxData = chunk.AuxData.GetData(chunk, index);
      }
      else
      {
        this.getDataResult.BlockID = this.map.OutOfBoundsBlockID;
        this.getDataResult.Light.SunLight = (byte) 15;
        this.getDataResult.Light.BlockLight = (byte) 0;
        this.getDataResult.AuxData = (byte) 0;
      }
      return this.getDataResult;
    }

    private bool IncrementPoint(Direction dir)
    {
      switch (dir)
      {
        case Direction.Left:
        case Direction.Right:
          if (++this.Data.Point.Z == this.chunksizeZ_1)
          {
            this.IncrementIndexes(this.chunkSizeX + this.chunkSizeX);
            this.Data.Point.Z = 1;
            if (++this.Data.Point.Y == this.chunksizeY_1)
              return false;
          }
          return true;
        case Direction.Forward:
        case Direction.Backward:
          if (++this.Data.Point.X == this.chunksizeX_1)
          {
            this.IncrementIndexes(this.chunkPlaneSize - (this.chunkSizeX - 2));
            this.Data.Point.X = 1;
            if (++this.Data.Point.Y == this.chunksizeY_1)
              return false;
          }
          return true;
        case Direction.Up:
        case Direction.Down:
          if (++this.Data.Point.X == this.chunksizeX_1)
          {
            this.IncrementIndexes(2);
            this.Data.Point.X = 1;
            if (++this.Data.Point.Z == this.chunksizeZ_1)
              return false;
          }
          return true;
        default:
          return false;
      }
    }

    private int GetIncrement(Direction dir)
    {
      switch (dir)
      {
        case Direction.Left:
        case Direction.Right:
          return this.chunkSizeX;
        case Direction.Forward:
        case Direction.Backward:
        case Direction.Up:
        case Direction.Down:
          return 1;
        default:
          return 0;
      }
    }

    private Point3D GetStartPoint(Direction dir)
    {
      switch (dir)
      {
        case Direction.Left:
          return new Point3D(0, 1, 1);
        case Direction.Forward:
          return new Point3D(1, 1, 0);
        case Direction.Right:
          return new Point3D(this.chunksizeX_1, 1, 1);
        case Direction.Backward:
          return new Point3D(1, 1, this.chunksizeZ_1);
        case Direction.Up:
          return new Point3D(1, this.chunksizeY_1, 1);
        case Direction.Down:
          return new Point3D(1, 0, 1);
        default:
          return Point3D.Zero;
      }
    }

    protected void AssignIndexes(MapChunk chunk, Direction dir)
    {
      this.mapIndex = chunk.GetMapIndex(this.Data.Point);
      this.leftIndex = this.mapIndex - 1;
      this.forwardIndex = this.mapIndex - this.chunkSizeX;
      this.rightIndex = this.mapIndex + 1;
      this.backwardIndex = this.mapIndex + this.chunkSizeX;
      this.upIndex = this.mapIndex + this.chunkPlaneSize;
      this.downIndex = this.mapIndex - this.chunkPlaneSize;
      if (!this.oldSkoolLight)
      {
        this.forwardUpIndex = this.forwardIndex + this.chunkPlaneSize;
        this.forwardDownIndex = this.forwardIndex - this.chunkPlaneSize;
        this.backwardUpIndex = this.backwardIndex + this.chunkPlaneSize;
        this.backwardDownIndex = this.backwardIndex - this.chunkPlaneSize;
        this.leftUpIndex = this.upIndex - 1;
        this.leftDownIndex = this.downIndex - 1;
        this.leftForwardIndex = this.forwardIndex - 1;
        this.leftForwardUpIndex = this.forwardUpIndex - 1;
        this.leftForwardDownIndex = this.forwardDownIndex - 1;
        this.leftBackwardIndex = this.backwardIndex - 1;
        this.leftBackwardUpIndex = this.backwardUpIndex - 1;
        this.leftBackwardDownIndex = this.backwardDownIndex - 1;
        this.rightUpIndex = this.upIndex + 1;
        this.rightDownIndex = this.downIndex + 1;
        this.rightForwardIndex = this.forwardIndex + 1;
        this.rightForwardUpIndex = this.forwardUpIndex + 1;
        this.rightForwardDownIndex = this.forwardDownIndex + 1;
        this.rightBackwardIndex = this.backwardIndex + 1;
        this.rightBackwardUpIndex = this.backwardUpIndex + 1;
        this.rightBackwardDownIndex = this.backwardDownIndex + 1;
      }
      switch (dir)
      {
        case Direction.Left:
          this.leftIndex += this.chunkSizeX;
          if (this.oldSkoolLight)
            break;
          this.leftUpIndex += this.chunkSizeX;
          this.leftDownIndex += this.chunkSizeX;
          this.leftForwardIndex += this.chunkSizeX;
          this.leftForwardUpIndex += this.chunkSizeX;
          this.leftForwardDownIndex += this.chunkSizeX;
          this.leftBackwardIndex += this.chunkSizeX;
          this.leftBackwardUpIndex += this.chunkSizeX;
          this.leftBackwardDownIndex += this.chunkSizeX;
          break;
        case Direction.Forward:
          this.forwardIndex += this.chunkPlaneSize;
          if (this.oldSkoolLight)
            break;
          this.forwardUpIndex += this.chunkPlaneSize;
          this.forwardDownIndex += this.chunkPlaneSize;
          this.leftForwardIndex += this.chunkPlaneSize;
          this.leftForwardUpIndex += this.chunkPlaneSize;
          this.leftForwardDownIndex += this.chunkPlaneSize;
          this.rightForwardIndex += this.chunkPlaneSize;
          this.rightForwardUpIndex += this.chunkPlaneSize;
          this.rightForwardDownIndex += this.chunkPlaneSize;
          break;
        case Direction.Right:
          this.rightIndex -= this.chunkSizeX;
          if (this.oldSkoolLight)
            break;
          this.rightUpIndex -= this.chunkSizeX;
          this.rightDownIndex -= this.chunkSizeX;
          this.rightForwardIndex -= this.chunkSizeX;
          this.rightForwardUpIndex -= this.chunkSizeX;
          this.rightForwardDownIndex -= this.chunkSizeX;
          this.rightBackwardIndex -= this.chunkSizeX;
          this.rightBackwardUpIndex -= this.chunkSizeX;
          this.rightBackwardDownIndex -= this.chunkSizeX;
          break;
        case Direction.Backward:
          this.backwardIndex -= this.chunkPlaneSize;
          if (this.oldSkoolLight)
            break;
          this.backwardUpIndex -= this.chunkPlaneSize;
          this.backwardDownIndex -= this.chunkPlaneSize;
          this.leftBackwardIndex -= this.chunkPlaneSize;
          this.leftBackwardUpIndex -= this.chunkPlaneSize;
          this.leftBackwardDownIndex -= this.chunkPlaneSize;
          this.rightBackwardIndex -= this.chunkPlaneSize;
          this.rightBackwardUpIndex -= this.chunkPlaneSize;
          this.rightBackwardDownIndex -= this.chunkPlaneSize;
          break;
        case Direction.Up:
          int num1 = this.chunkPlaneSize * (this.chunksizeY_1 + 1);
          this.upIndex -= num1;
          if (this.oldSkoolLight)
            break;
          this.forwardUpIndex -= num1;
          this.backwardUpIndex -= num1;
          this.leftUpIndex -= num1;
          this.leftForwardUpIndex -= num1;
          this.leftBackwardUpIndex -= num1;
          this.rightUpIndex -= num1;
          this.rightForwardUpIndex -= num1;
          this.rightBackwardUpIndex -= num1;
          break;
        case Direction.Down:
          int num2 = this.chunkPlaneSize * (this.chunksizeY_1 + 1);
          this.downIndex += num2;
          if (this.oldSkoolLight)
            break;
          this.forwardDownIndex += num2;
          this.backwardDownIndex += num2;
          this.leftDownIndex += num2;
          this.leftForwardDownIndex += num2;
          this.leftBackwardDownIndex += num2;
          this.rightDownIndex += num2;
          this.rightForwardDownIndex += num2;
          this.rightBackwardDownIndex += num2;
          break;
      }
    }

    private void AssignChunks(MapChunk chunk, MapChunk chunk2, Direction dir)
    {
      this.left = chunk;
      this.forward = chunk;
      this.right = chunk;
      this.backward = chunk;
      this.up = chunk;
      this.down = chunk;
      this.leftForward = chunk;
      this.leftBackward = chunk;
      this.leftUp = chunk;
      this.leftDown = chunk;
      this.leftForwardUp = chunk;
      this.leftForwardDown = chunk;
      this.leftBackwardUp = chunk;
      this.leftBackwardDown = chunk;
      this.rightForward = chunk;
      this.rightBackward = chunk;
      this.rightUp = chunk;
      this.rightDown = chunk;
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
          this.leftForward = chunk2;
          this.leftBackward = chunk2;
          this.leftUp = chunk2;
          this.leftDown = chunk2;
          this.leftForwardUp = chunk2;
          this.leftForwardDown = chunk2;
          this.leftBackwardUp = chunk2;
          this.leftBackwardDown = chunk2;
          break;
        case Direction.Forward:
          this.forward = chunk2;
          this.leftForward = chunk2;
          this.leftForwardUp = chunk2;
          this.leftForwardDown = chunk2;
          this.rightForward = chunk2;
          this.rightForwardUp = chunk2;
          this.rightForwardDown = chunk2;
          this.forwardUp = chunk2;
          this.forwardDown = chunk2;
          break;
        case Direction.Right:
          this.right = chunk2;
          this.rightForward = chunk2;
          this.rightBackward = chunk2;
          this.rightUp = chunk2;
          this.rightDown = chunk2;
          this.rightForwardUp = chunk2;
          this.rightForwardDown = chunk2;
          this.rightBackwardUp = chunk2;
          this.rightBackwardDown = chunk2;
          break;
        case Direction.Backward:
          this.backward = chunk2;
          this.leftBackward = chunk2;
          this.leftBackwardUp = chunk2;
          this.leftBackwardDown = chunk2;
          this.rightBackward = chunk2;
          this.rightBackwardUp = chunk2;
          this.rightBackwardDown = chunk2;
          this.backwardUp = chunk2;
          this.backwardDown = chunk2;
          break;
        case Direction.Up:
          this.up = chunk2;
          this.leftUp = chunk2;
          this.leftForwardUp = chunk2;
          this.leftBackwardUp = chunk2;
          this.rightUp = chunk2;
          this.rightForwardUp = chunk2;
          this.rightBackwardUp = chunk2;
          this.forwardUp = chunk2;
          this.backwardUp = chunk2;
          break;
        case Direction.Down:
          this.down = chunk2;
          this.leftDown = chunk2;
          this.leftForwardDown = chunk2;
          this.leftBackwardDown = chunk2;
          this.rightDown = chunk2;
          this.rightForwardDown = chunk2;
          this.rightBackwardDown = chunk2;
          this.forwardDown = chunk2;
          this.backwardDown = chunk2;
          break;
      }
    }
  }
}
