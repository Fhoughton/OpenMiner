// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.SurroundingChunkEnumerator
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  public class SurroundingChunkEnumerator
  {
    public GlobalPoint3D Current;
    private int count;
    private Point3D chunksize;
    private Direction dir;
    private int dirCount;
    private int dirCurrent;
    private float endCount;
    private int xdelta;
    private int zdelta;
    private GlobalPoint3D origP;
    private Map map;
    private BoxInt mapBound;

    public void Reset(Map map, GlobalPoint3D p, int count)
    {
      this.map = map;
      this.count = 0;
      this.Current = p;
      this.origP = p;
      this.endCount = (float) count;
      this.chunksize = map.ChunkSize;
      this.dirCount = 2;
      this.dirCurrent = 0;
      this.dir = Direction.Left;
      this.xdelta = -this.chunksize.X;
      this.mapBound = map.MapBound;
    }

    public bool MoveNext()
    {
      while ((double) this.count < (double) this.endCount)
      {
        this.MoveNextCore();
        if (this.Current.X >= this.mapBound.Min.X && this.Current.X < this.mapBound.Max.X && (this.Current.Z >= this.mapBound.Min.Z && this.Current.Z < this.mapBound.Max.Z))
          return true;
      }
      return false;
    }

    private void MoveNextCore()
    {
      if (this.count > 2)
      {
        ++this.count;
        this.Current.X += this.xdelta;
        this.Current.Z += this.zdelta;
        if (++this.dirCurrent != this.dirCount)
          return;
        this.dirCurrent = 0;
        switch (this.dir)
        {
          case Direction.Left:
            this.dir = Direction.Backward;
            this.xdelta = 0;
            this.zdelta = this.chunksize.Z;
            break;
          case Direction.Forward:
            this.dir = Direction.Left;
            this.xdelta = -this.chunksize.X;
            this.zdelta = 0;
            ++this.dirCount;
            break;
          case Direction.Right:
            this.dir = Direction.Forward;
            this.xdelta = 0;
            this.zdelta = -this.chunksize.Z;
            break;
          case Direction.Backward:
            this.dir = Direction.Right;
            this.xdelta = this.chunksize.X;
            this.zdelta = 0;
            ++this.dirCount;
            break;
        }
      }
      else if (this.count == 2)
      {
        this.Current.Z -= this.chunksize.Z;
        this.count = 3;
      }
      else if (this.count == 1)
      {
        this.Current.X += this.chunksize.X;
        this.count = 2;
      }
      else
        this.count = 1;
    }
  }
}
