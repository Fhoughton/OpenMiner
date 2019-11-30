// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapOld
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class MapOld
  {
    private static float[] lightTable = new float[16]
    {
      0.1f,
      0.16f,
      0.22f,
      0.28f,
      0.34f,
      0.4f,
      0.46f,
      0.52f,
      0.58f,
      0.64f,
      0.7f,
      0.76f,
      0.82f,
      0.88f,
      0.94f,
      1f
    };
    private List<Point3D> pointsOfPenetration = new List<Point3D>(9);
    private CustomArray<byte> chunkData1 = new CustomArray<byte>(32768, 2f);
    private CustomArray<byte> chunkData2 = new CustomArray<byte>(32768, 2f);
    public float NullLight = MapOld.lightTable[0];
    private const int maxStaticDataArraySize = 66060288;
    public ushort[] HeightMap;
    public PcgRandom Random;
    public Vector3 Position;
    public Matrix World;
    public float TileSize;
    public float HalfTileSize;
    public Point3D MapSize;
    public int Seed;
    public int WaterLevel;
    public bool SaveDataEnabled;
    public byte OutOfBoundsBlockID;
    public byte[] DynamicBlockIDs;
    public byte[] LightPassBlockIDs;
    public byte WaterBlockID;
    public byte LavaBlockID;
    public byte RopeBlockID;
    public bool AllowEdgeClear;
    public MapStrategyOld MapStrategy;
    public bool IsHost;
    public readonly byte SunLight;
    protected ushort[] Data;
    protected ushort[] Data2;
    protected int zIndex;
    protected int yIndex;
    protected int maxIndex;
    protected int dataLength;
    private Point3D chunkSize;
    private Point3D blastPoint;

    protected virtual byte GetOpacity(byte blockID)
    {
      return 0;
    }

    protected virtual byte GetLuminance(byte blockID)
    {
      return 15;
    }

    protected virtual byte GetBlastResistance(byte blockID)
    {
      return 0;
    }

    public MapOld(float tileSize, Point3D mapSize, Point3D chunkSize, int seed)
    {
      this.Seed = seed;
      this.MapSize.X = mapSize.X;
      this.MapSize.Y = mapSize.Y;
      this.MapSize.Z = mapSize.Z;
      this.chunkSize = chunkSize;
      this.TileSize = tileSize;
      this.HalfTileSize = tileSize * 0.5f;
      this.IsHost = true;
      this.zIndex = mapSize.X;
      this.yIndex = mapSize.X * mapSize.Z;
      this.maxIndex = mapSize.X * mapSize.Y * mapSize.Z;
      this.SunLight = seed == 2400 ? (byte) 1 : (byte) 15;
      int length = Math.Min(this.maxIndex, 66060288);
      this.Data = new ushort[length];
      if (length < this.maxIndex)
        this.Data2 = new ushort[this.maxIndex - length];
      this.dataLength = this.Data.Length;
      this.MapStrategy = (MapStrategyOld) new DummyMapStrategyOld(false);
    }

    public void ClearMap(ushort blockData)
    {
      for (int index = 0; index < this.Data.Length; ++index)
        this.Data[index] = blockData;
      if (this.Data2 == null)
        return;
      for (int index = 0; index < this.Data2.Length; ++index)
        this.Data2[index] = blockData;
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
      this.Data = (ushort[]) null;
      this.Data2 = (ushort[]) null;
      if (this.MapStrategy != null)
      {
        this.MapStrategy.UnloadContent();
        this.MapStrategy = (MapStrategyOld) null;
      }
      this.HeightMap = (ushort[]) null;
      this.DynamicBlockIDs = (byte[]) null;
    }

    protected virtual void UnloadContentCore()
    {
    }

    public void Update()
    {
      this.UpdateCore();
      this.MapStrategy.Update();
      this.World = Matrix.CreateTranslation(this.Position);
    }

    protected virtual void UpdateCore()
    {
    }

    public Vector3 FullMapSize
    {
      get
      {
        return new Vector3((float) this.MapSize.X * this.TileSize, (float) this.MapSize.Y * this.TileSize, (float) this.MapSize.Z * this.TileSize);
      }
    }

    public BoundingBox GetFullMapBound()
    {
      Vector3 translation = this.World.Translation;
      return new BoundingBox(translation - new Vector3(0.0f, (float) this.MapSize.Y * this.TileSize, 0.0f), translation + new Vector3((float) this.MapSize.X * this.TileSize, 0.0f, (float) this.MapSize.Z * this.TileSize));
    }

    public Point3D GetPoint(Vector3 position)
    {
      int num1 = (int) ((double) position.X / (double) this.TileSize);
      int num2 = (int) ((double) position.Y / (double) this.TileSize);
      int num3 = (int) ((double) position.Z / (double) this.TileSize);
      return new Point3D() { X = num1, Y = num2, Z = num3 };
    }

    public Point3D GetGroundPoint(Point3D p)
    {
      if (!this.IsPassable(p))
      {
        ++p.Y;
        while (p.Y < 0 && !this.IsPassable(p))
          ++p.Y;
        --p.Y;
      }
      else
      {
        while (p.Y > -(this.MapSize.Y - 1) && this.IsPassable(p))
          --p.Y;
      }
      return p;
    }

    public Point3D Clamp(Point3D p)
    {
      if (p.X < 0)
        p.X = 0;
      else if (p.X >= this.MapSize.X)
        p.X = this.MapSize.X - 1;
      if (p.Z < 0)
        p.Z = 0;
      else if (p.Z >= this.MapSize.Z)
        p.Z = this.MapSize.Z - 1;
      if (p.Y > 0)
        p.Y = 0;
      else if (p.Y <= -this.MapSize.Y)
        p.Y = -(this.MapSize.Y - 1);
      return p;
    }

    public Point3D Clamp(Point3D p, int edge)
    {
      if (p.X < edge)
        p.X = edge;
      else if (p.X >= this.MapSize.X - edge)
        p.X = this.MapSize.X - edge - 1;
      if (p.Z < edge)
        p.Z = edge;
      else if (p.Z >= this.MapSize.Z - edge)
        p.Z = this.MapSize.Z - edge - 1;
      if (p.Y > -edge)
        p.Y = -edge;
      else if (p.Y <= -this.MapSize.Y + edge)
        p.Y = -(this.MapSize.Y - edge - 1);
      return p;
    }

    public BoundingBox GetBox(Point3D p)
    {
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = {
          X = (float) p.X * this.TileSize,
          Y = (float) p.Y * this.TileSize,
          Z = (float) p.Z * this.TileSize
        }
      };
      boundingBox.Max.X = boundingBox.Min.X + this.TileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + this.TileSize;
      boundingBox.Max.Z = boundingBox.Min.Z + this.TileSize;
      return boundingBox;
    }

    public byte GetBlockID(Vector3 position)
    {
      return this.GetBlockID(this.GetPoint(position));
    }

    public byte GetBlockID(Point3D p)
    {
      return (byte) this.GetBlockData(p);
    }

    public ushort GetBlockData(Point3D p)
    {
      int index = p.X + p.Z * this.zIndex + -p.Y * this.yIndex;
      if (index < 0 || index >= this.maxIndex)
        return this.BuildBlockData(this.OutOfBoundsBlockID, this.SunLight, (byte) 0);
      if (index < this.dataLength)
        return this.Data[index];
      return this.Data2[index - this.dataLength];
    }

    public void SetBlockData(
      Point3D p,
      byte blockID,
      byte auxData,
      UpdateBlockMethod method,
      short playerID,
      bool transmit)
    {
      if (p.X < 0 || p.Z < 0 || (p.X >= this.MapSize.X || p.Z >= this.MapSize.Z))
        return;
      ushort blockData = this.GetBlockData(p);
      ushort num = this.BuildBlockData(blockID, this.GetLight(blockData), auxData);
      this.SetBlockDataInternal(p, num);
      if (this.HeightMap != null)
        this.RecalculateHeight(p, blockID);
      this.MapStrategy.BlockChanged(p, blockData, num, method, playerID, transmit);
    }

    public void SetBlockDataInternal(Point3D p, ushort blockData)
    {
      int index = p.X + p.Z * this.zIndex + -p.Y * this.yIndex;
      if (index < 0 || index >= this.maxIndex)
        return;
      if (this.SaveDataEnabled)
        blockData |= (ushort) 2048;
      if (index < this.dataLength)
        this.Data[index] = blockData;
      else
        this.Data2[index - this.dataLength] = blockData;
    }

    public void SetBlockAusDataInternal(Point3D p, byte auxData)
    {
      int num = (int) this.GetBlockData(p) & 61695 | (int) auxData << 8;
      this.SetBlockDataInternal(p, (ushort) num);
    }

    public void SetBlockLight(Point3D p, ushort blockData, byte light)
    {
      int num = ((int) light & 15) << 12;
      blockData &= (ushort) 4095;
      blockData += (ushort) num;
      this.SetBlockDataInternal(p, blockData);
    }

    public ushort BuildBlockData(byte blockID, byte light, byte auxData)
    {
      return (ushort) ((uint) ((((int) light & 15) << 4) + ((int) auxData & 15) << 8) + (uint) blockID);
    }

    public static ushort BuildBlockDataStatic(byte blockID, byte light, byte auxData)
    {
      return (ushort) ((uint) ((((int) light & 15) << 4) + ((int) auxData & 15) << 8) + (uint) blockID);
    }

    public ClearBlockResult ClearBlock(
      Point3D p,
      UpdateBlockMethod method,
      short playerID,
      bool transmit)
    {
      ClearBlockResult clearBlockResult = ClearBlockResult.AlreadyClear;
      byte blockData = (byte) this.GetBlockData(p);
      if (blockData > (byte) 0)
      {
        clearBlockResult = this.GetClearBlockResult(p, blockData, method, playerID);
        if (clearBlockResult == ClearBlockResult.Success)
        {
          if (method == UpdateBlockMethod.Generation && p.Y < this.WaterLevel - 40)
          {
            this.SetBlockDataInternal(p, (ushort) 0);
          }
          else
          {
            this.SetBlockData(p, (byte) 0, (byte) 0, method, playerID, transmit);
            if (p.Y > this.WaterLevel || p.Y < 0 && method != UpdateBlockMethod.Generation)
              this.CheckForRelatedClears(p, method, playerID, transmit);
          }
        }
      }
      return clearBlockResult;
    }

    public ClearBlockResult GetClearBlockResult(
      Point3D p,
      UpdateBlockMethod method,
      short playerID)
    {
      if (this.IsValidPoint(p))
        return this.GetClearBlockResult(p, this.GetBlockID(p), method, playerID);
      return ClearBlockResult.OutOfBounds;
    }

    public ClearBlockResult GetClearBlockResult(
      Point3D p,
      byte blockID,
      UpdateBlockMethod method,
      short playerID)
    {
      if (p.Y <= this.WaterLevel && this.IsOnEdge(p) && !this.AllowEdgeClear)
        return ClearBlockResult.OutOfBounds;
      if (-p.Y == this.MapSize.Y - 1)
        return ClearBlockResult.BedRock;
      if (p.Y < 0 && (int) blockID == (int) this.RopeBlockID && (int) this.GetBlockID(p + Point3D.Up) == (int) this.RopeBlockID)
        return ClearBlockResult.CantCutRope;
      return this.MapStrategy.GetClearBlockResult(p, blockID, method, playerID);
    }

    public void CheckForRelatedClears(
      Point3D p,
      UpdateBlockMethod method,
      short playerID,
      bool transmit)
    {
      ++p.Y;
      if (p.Y <= 0 && (this.IsAttached(p, (byte) 0) || this.IsIcon(p)))
      {
        int num1 = (int) this.ClearBlock(p, method, playerID, transmit);
      }
      --p.Y;
      --p.X;
      if (p.X >= 0 && this.IsAttached(p, (byte) 1))
      {
        int num2 = (int) this.ClearBlock(p, method, playerID, transmit);
      }
      p.X += 2;
      if (p.X < this.MapSize.X && this.IsAttached(p, (byte) 2))
      {
        int num3 = (int) this.ClearBlock(p, method, playerID, transmit);
      }
      --p.X;
      --p.Z;
      if (p.Z >= 0 && this.IsAttached(p, (byte) 3))
      {
        int num4 = (int) this.ClearBlock(p, method, playerID, transmit);
      }
      p.Z += 2;
      if (p.Z < this.MapSize.Z && this.IsAttached(p, (byte) 4))
      {
        int num5 = (int) this.ClearBlock(p, method, playerID, transmit);
      }
      --p.Z;
      --p.Y;
      if (p.Y <= -this.MapSize.Y || !this.IsAttached(p, (byte) 5))
        return;
      int num6 = (int) this.ClearBlock(p, method, playerID, transmit);
    }

    public bool HasAttachment(Point3D p)
    {
      ++p.Y;
      if (p.Y <= 0 && this.IsAttached(p, (byte) 0))
        return true;
      --p.Y;
      --p.X;
      if (p.X >= 0 && this.IsAttached(p, (byte) 1))
        return true;
      p.X += 2;
      if (p.X < this.MapSize.X && this.IsAttached(p, (byte) 2))
        return true;
      --p.X;
      --p.Z;
      if (p.Z >= 0 && this.IsAttached(p, (byte) 3))
        return true;
      p.Z += 2;
      if (p.Z < this.MapSize.Z && this.IsAttached(p, (byte) 4))
        return true;
      --p.Z;
      --p.Y;
      return p.Y > -this.MapSize.Y && this.IsAttached(p, (byte) 5);
    }

    public void MemCopy(int startY, int endY, ushort blockData)
    {
      int num = Math.Min((endY + 1) * this.yIndex, this.maxIndex);
      for (int index = startY * this.yIndex; index < num; ++index)
      {
        if (index < this.dataLength)
          this.Data[index] = blockData;
        else
          this.Data2[index - this.dataLength] = blockData;
      }
    }

    public Point3D FindFirst(byte blockID)
    {
      for (int i = 0; i < this.Data.Length; ++i)
      {
        if ((int) (byte) this.Data[i] == (int) blockID)
          return this.GetPointFromIndex(i);
      }
      if (this.Data2 != null && this.Data2.Length > 0)
      {
        for (int index = 0; index < this.Data2.Length; ++index)
        {
          if ((int) (byte) this.Data2[index] == (int) blockID)
            return this.GetPointFromIndex(index + this.Data.Length);
        }
      }
      return new Point3D(0, 1, 0);
    }

    public Point3D[] FindAll(byte blockID)
    {
      List<Point3D> point3DList = new List<Point3D>();
      for (int i = 0; i < this.Data.Length; ++i)
      {
        if ((int) (byte) this.Data[i] == (int) blockID)
          point3DList.Add(this.GetPointFromIndex(i));
      }
      if (this.Data2 != null && this.Data2.Length > 0)
      {
        for (int i = 0; i < this.Data2.Length; ++i)
        {
          if ((int) (byte) this.Data2[i] == (int) blockID)
            point3DList.Add(this.GetPointFromIndex(i));
        }
      }
      return point3DList.ToArray();
    }

    public Point3D FindSpace(Point3D p, int rad, int distanceFromP)
    {
      if (this.CheckFreeSpaceAround(p, rad))
        return p;
      Point3D point3D = p;
      for (int index = 1; index <= distanceFromP; ++index)
      {
        p = point3D;
        p.Y += index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.Y -= index;
        p.Y -= index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.Y += index;
        p.X += index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.X -= index;
        p.X -= index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.X -= index;
        p.Z += index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.Z -= index;
        p.Z -= index;
        if (this.CheckFreeSpaceAround(p, rad))
          return p;
        p.Z += index;
      }
      return p;
    }

    private bool CheckFreeSpaceAround(Point3D p, int rad)
    {
      Point3D p1;
      for (p1.Y = p.Y - rad; p1.Y <= p.Y + rad; ++p1.Y)
      {
        if (p1.Y <= 0 && p1.Y > -this.MapSize.Y)
        {
          for (p1.Z = p.Z - rad; p1.Z <= p.Z + rad; ++p1.Z)
          {
            if (p1.Z > 0 && p1.Z < this.MapSize.Z - 1)
            {
              for (p1.X = p.X - rad; p1.X <= p.X + rad; ++p1.X)
              {
                if (p1.X > 0 && p1.X < this.MapSize.X - 1 && !this.IsPassable(p1))
                  return false;
              }
            }
          }
        }
      }
      return true;
    }

    private Point3D GetPointFromIndex(int i)
    {
      ushort num1 = (ushort) (i % this.MapSize.X);
      int num2 = (int) (ushort) (i / this.yIndex);
      ushort num3 = (ushort) ((i - num2 * this.yIndex) / this.zIndex);
      return new Point3D()
      {
        X = (int) num1,
        Y = -num2,
        Z = (int) num3
      };
    }

    public void Replace(byte blockID, byte withID)
    {
      if ((int) withID == (int) blockID)
        return;
      for (int i = 0; i < this.Data.Length; ++i)
      {
        ushort blockData1 = this.Data[i];
        if ((int) (byte) blockData1 == (int) blockID)
        {
          ushort blockData2 = this.BuildBlockData(withID, this.GetLight(blockData1), this.GetAuxData(blockData1));
          this.SetBlockDataInternal(this.GetPointFromIndex(i), blockData2);
        }
      }
    }

    public void RemoveAll(byte blockID, short playerID)
    {
      for (int i = 0; i < this.Data.Length; ++i)
      {
        if ((int) (byte) this.Data[i] == (int) blockID)
          this.SetBlockData(this.GetPointFromIndex(i), (byte) 0, (byte) 0, UpdateBlockMethod.Generation, playerID, true);
      }
      if (this.Data2 == null)
        return;
      for (int index = 0; index < this.Data2.Length; ++index)
      {
        if ((int) (byte) this.Data2[index] == (int) blockID)
          this.SetBlockData(this.GetPointFromIndex(index + this.dataLength), (byte) 0, (byte) 0, UpdateBlockMethod.Generation, playerID, true);
      }
    }

    public Vector3 GetBlockCenter(Point3D point)
    {
      return new Vector3((float) point.X * this.TileSize + this.HalfTileSize, (float) point.Y * this.TileSize - this.HalfTileSize, (float) point.Z * this.TileSize + this.HalfTileSize);
    }

    public Vector3 GetBlockPos(Point3D point)
    {
      return new Vector3((float) point.X * this.TileSize, (float) point.Y * this.TileSize, (float) point.Z * this.TileSize);
    }

    private int GetRealHeight(Point3D p)
    {
      p.Y = this.WaterLevel;
      if ((int) this.GetLight((ushort) (byte) this.GetBlockData(p)) < (int) this.SunLight)
      {
        do
          ;
        while (++p.Y < 0 && this.IsSolid(p));
        return p.Y - 1;
      }
      do
        ;
      while (--p.Y > -this.MapSize.Y - 1 && !this.IsSolid(p));
      return p.Y;
    }

    public int GetHeight(Point3D p)
    {
      if (this.HeightMap != null)
        return (int) -this.HeightMap[p.X + p.Z * this.MapSize.X];
      return this.GetRealHeight(p);
    }

    public int MostCommonHeight(ushort[] heightMap, Point range)
    {
      int num1 = 0;
      int num2 = 0;
      int[] numArray = new int[range.Y + 1];
      for (int index1 = 0; index1 < this.MapSize.Z; ++index1)
      {
        for (int index2 = 0; index2 < this.MapSize.X; ++index2)
        {
          int height = (int) heightMap[index2 + index1 * this.MapSize.X];
          if (height >= range.X && height <= range.Y)
          {
            ++numArray[height];
            if (numArray[height] > num2)
            {
              num2 = numArray[height];
              num1 = height;
            }
          }
        }
      }
      return num1;
    }

    private void RecalculateHeight(Point3D p, byte blockID)
    {
      if (this.IsIcon(blockID))
        return;
      int num = (int) -this.HeightMap[p.X + p.Z * this.MapSize.X];
      if (blockID > (byte) 0)
      {
        if (num >= p.Y)
          return;
        this.SetHeight(p);
      }
      else
      {
        if (num != p.Y)
          return;
        do
          ;
        while (--p.Y > -this.MapSize.Y && !this.IsSolid(p));
        this.SetHeight(p);
      }
    }

    private void SetHeight(Point3D p)
    {
      if (p.Y < 0)
        p.Y = -p.Y;
      this.HeightMap[p.X + p.Z * this.MapSize.X] = (ushort) p.Y;
    }

    public bool IsSolid(byte blockID)
    {
      if (blockID > (byte) 0)
        return !this.IsIcon(blockID);
      return false;
    }

    public bool IsSolid(Point3D p)
    {
      return this.IsSolid(this.GetBlockID(p));
    }

    public bool IsPassable(Vector3 position)
    {
      return this.IsPassable(this.GetBlockID(this.GetPoint(position)));
    }

    public bool IsPassable(Point3D p)
    {
      return this.IsPassable(this.GetBlockID(p));
    }

    public virtual bool IsPassable(byte blockID)
    {
      return false;
    }

    public bool IsIcon(Vector3 position)
    {
      return this.IsIcon(this.GetPoint(position));
    }

    public bool IsIcon(Point3D p)
    {
      return this.IsIcon(this.GetBlockID(p));
    }

    public virtual bool IsIcon(byte blockID)
    {
      return false;
    }

    public bool IsAttached(Point3D p, byte face)
    {
      ushort blockData = this.GetBlockData(p);
      if (this.IsAttachable((byte) blockData))
        return (int) this.GetAuxData(blockData) == (int) face;
      return false;
    }

    public virtual bool IsAttachable(byte blockID)
    {
      return false;
    }

    public bool IsClearAndAbove(Point3D p, int andAboveCount)
    {
      if (this.GetBlockID(p) != (byte) 0)
        return false;
      for (; andAboveCount > 0; --andAboveCount)
      {
        ++p.Y;
        if (this.GetBlockID(p) > (byte) 0)
          return false;
      }
      return true;
    }

    public bool IsDynamic(byte blockID)
    {
      if (this.DynamicBlockIDs != null && this.DynamicBlockIDs.Length > 0)
      {
        for (int index = 0; index < this.DynamicBlockIDs.Length; ++index)
        {
          if ((int) blockID == (int) this.DynamicBlockIDs[index])
            return true;
        }
      }
      return false;
    }

    public bool IsLightSource(byte blockID)
    {
      return this.GetLuminance(blockID) > (byte) 0;
    }

    public bool IsLightBlocker(byte blockID)
    {
      if (blockID == (byte) 0)
        return false;
      if (this.LightPassBlockIDs != null)
      {
        for (int index = 0; index < this.LightPassBlockIDs.Length; ++index)
        {
          if ((int) blockID == (int) this.LightPassBlockIDs[index])
            return false;
        }
      }
      return !this.IsIcon(blockID);
    }

    public bool CanLightReachPoint(Point3D p)
    {
      if (p.X > 0)
      {
        --p.X;
        if (!this.IsLightBlocker(this.GetBlockID(p)))
          return true;
        ++p.X;
        if (p.X < this.MapSize.X - 1)
        {
          ++p.X;
          if (!this.IsLightBlocker(this.GetBlockID(p)))
            return true;
          --p.X;
        }
      }
      if (p.Z > 0)
      {
        --p.Z;
        if (!this.IsLightBlocker(this.GetBlockID(p)))
          return true;
        ++p.Z;
        if (p.Z < this.MapSize.Z - 1)
        {
          ++p.Z;
          if (!this.IsLightBlocker(this.GetBlockID(p)))
            return true;
          --p.Z;
        }
      }
      if (p.Y > -(this.MapSize.Y - 1))
      {
        --p.Y;
        if (!this.IsLightBlocker(this.GetBlockID(p)))
          return true;
        ++p.Y;
        if (p.Y < 0)
        {
          ++p.Y;
          if (!this.IsLightBlocker(this.GetBlockID(p)))
            return true;
          --p.Y;
        }
      }
      return false;
    }

    public bool IsValidPoint(int x, int y, int z)
    {
      if (x >= 0 && x < this.MapSize.X && (y <= 0 && y > -this.MapSize.Y) && z >= 0)
        return z < this.MapSize.Z;
      return false;
    }

    public bool IsValidPoint(Point3D p)
    {
      if (p.X >= 0 && p.X < this.MapSize.X && (p.Y <= 0 && p.Y > -this.MapSize.Y) && p.Z >= 0)
        return p.Z < this.MapSize.Z;
      return false;
    }

    public bool IsInsideMap(Point3D p)
    {
      if (p.X > 0 && p.Z > 0 && (p.Y < 0 && p.Y > -this.MapSize.Y - 1) && p.X < this.MapSize.X - 1)
        return p.Z < this.MapSize.Z - 1;
      return false;
    }

    public bool IsNextTo(Point3D p, byte blockID)
    {
      return this.IsNextTo(p, blockID, false, false);
    }

    public bool IsNextTo(Point3D p, byte blockID, bool ignoreSelf, bool ignoreBelow)
    {
      if (!ignoreSelf && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      --p.X;
      if (p.X >= 0 && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      ++p.X;
      ++p.Y;
      if (p.Y <= 0 && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      --p.Y;
      --p.Z;
      if (p.Z >= 0 && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      ++p.Z;
      ++p.X;
      if (p.X < this.MapSize.X && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      --p.X;
      ++p.Z;
      if (p.Z < this.MapSize.Z && (int) this.GetBlockID(p) == (int) blockID)
        return true;
      --p.Z;
      if (!ignoreBelow)
      {
        --p.Y;
        if (p.Y > -this.MapSize.Y && (int) this.GetBlockID(p) == (int) blockID)
          return true;
        ++p.Y;
      }
      return false;
    }

    public bool IsSupportingBlock(Point3D p, byte blockID)
    {
      ushort num = (ushort) blockID;
      if ((int) this.GetBlockData(p) == (int) num)
        return true;
      --p.X;
      if (p.X >= 0)
      {
        ushort blockData = this.GetBlockData(p);
        if ((int) blockData == (int) num && this.GetAuxData(blockData) == (byte) 2)
          return true;
      }
      ++p.X;
      --p.Z;
      if (p.Z >= 0)
      {
        ushort blockData = this.GetBlockData(p);
        if ((int) blockData == (int) num && this.GetAuxData(blockData) == (byte) 3)
          return true;
      }
      ++p.Z;
      ++p.X;
      if (p.X < this.MapSize.X)
      {
        ushort blockData = this.GetBlockData(p);
        if ((int) blockData == (int) num && this.GetAuxData(blockData) == (byte) 1)
          return true;
      }
      --p.X;
      --p.Y;
      if (p.Y > -this.MapSize.Y)
      {
        ushort blockData = this.GetBlockData(p);
        if ((int) blockData == (int) num && this.GetAuxData(blockData) == (byte) 0)
          return true;
      }
      ++p.Y;
      ++p.Z;
      if (p.Z < this.MapSize.Z)
      {
        ushort blockData = this.GetBlockData(p);
        if ((int) blockData == (int) num && this.GetAuxData(blockData) == (byte) 4)
          return true;
      }
      return false;
    }

    private bool IsOnEdge(Point3D p)
    {
      if (p.X != 0 && p.X != this.MapSize.X - 1 && (p.Z != 0 && p.Z != this.MapSize.Z - 1) && p.Y != 0)
        return p.Y == this.MapSize.Y - 1;
      return true;
    }

    public byte GetLight(ushort blockData)
    {
      return (byte) ((uint) blockData >> 12);
    }

    public byte GetLight(Point3D p)
    {
      return (byte) ((uint) this.GetBlockData(p) >> 12);
    }

    public float GetLightNormalized(Point3D p)
    {
      byte light = this.GetLight(p);
      return MapOld.lightTable[(int) light];
    }

    public float GetLightNormalized(ushort blockData)
    {
      byte num = (byte) ((uint) blockData >> 12);
      return MapOld.lightTable[(int) num];
    }

    public static float LightTableValue(int index)
    {
      return MapOld.lightTable[index];
    }

    public byte GetAuxData(ushort blockData)
    {
      return (byte) ((int) blockData >> 8 & 7);
    }

    public byte GetAuxData(Point3D p)
    {
      return this.GetAuxData(this.GetBlockData(p));
    }

    public bool HasChanged(ushort blockData)
    {
      return ((int) blockData >> 8 & 8) > 0;
    }

    public BlastResultOld CreateBlast(
      Point3D p,
      float blastStrength,
      int blastRadius,
      PcgRandom rand,
      bool loading,
      bool buildPointsOnly,
      short playerID)
    {
      BlastResultOld result = new BlastResultOld()
      {
        BuildPointsOnly = buildPointsOnly
      };
      if (buildPointsOnly)
        result.PointsCleared = new List<Point3D>(400);
      result.LowestY = int.MaxValue;
      this.blastPoint = p;
      for (int index = 0; index < blastRadius; ++index)
      {
        this.CreateBlastPlaneFast(p, p + Point3D.Down * index, blastStrength, blastRadius, rand, loading, playerID, ref result);
        if (index > 0)
          this.CreateBlastPlaneFast(p, p + Point3D.Up * index, blastStrength, blastRadius, rand, loading, playerID, ref result);
      }
      return result;
    }

    private void CreateBlastPlaneFast(
      Point3D blastOrigin,
      Point3D p,
      float blastStrength,
      int blastRadius,
      PcgRandom rand,
      bool loading,
      short playerID,
      ref BlastResultOld result)
    {
      if (p.Y > 0 || p.Y <= -this.MapSize.Y)
        return;
      float num1 = (float) (blastRadius * blastRadius);
      UpdateBlockMethod method = loading ? UpdateBlockMethod.Generation : UpdateBlockMethod.Blast;
      Vector3 vector3_1 = new Vector3((float) blastOrigin.X, (float) blastOrigin.Y, (float) blastOrigin.Z);
      Vector3 vector3_2 = vector3_1;
      vector3_2.Y = (float) p.Y;
      bool flag = loading;
      if (!flag)
      {
        float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
        if ((double) num2 == 0.0)
          num2 = 1f;
        flag = (double) this.GetBlastResistance(this.GetBlockID(p)) < (double) blastStrength / (double) num2;
      }
      if (flag)
      {
        if (result.BuildPointsOnly)
        {
          result.PointsCleared.Add(p);
        }
        else
        {
          int num2 = (int) this.ClearBlock(p, method, playerID, false);
        }
        if (p.Y < result.LowestY)
          result.LowestY = p.Y;
      }
      for (int index = 1; index <= blastRadius; ++index)
      {
        Point3D p1 = p;
        p1.X = p.X - index;
        vector3_2.X = (float) p1.X;
        if (p1.X >= 0)
        {
          for (p1.Z = p.Z - index; p1.Z <= p.Z + index && p1.Z < this.MapSize.Z - 1; ++p1.Z)
          {
            if (p1.Z > 0 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              vector3_2.Z = (float) p1.Z;
              float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
              if ((double) num2 <= (double) num1 && (loading || (double) this.GetBlastResistance(this.GetBlockID(p1)) < (double) blastStrength / (double) num2))
              {
                if (result.BuildPointsOnly)
                {
                  result.PointsCleared.Add(p1);
                }
                else
                {
                  int num3 = (int) this.ClearBlock(p1, method, playerID, false);
                }
                if (p1.Y < result.LowestY)
                  result.LowestY = p1.Y;
              }
            }
          }
        }
        --p1.Z;
        vector3_2.Z = (float) p1.Z;
        if (p1.Z >= 0)
        {
          for (; p1.X <= p.X + index && p1.X < this.MapSize.X - 1; ++p1.X)
          {
            if (p1.X > 0 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              byte blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
              if (loading || (double) blastResistance < (double) blastStrength)
              {
                vector3_2.X = (float) p1.X;
                float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
                if ((double) num2 <= (double) num1 && (loading || (double) blastResistance < (double) blastStrength / (double) num2))
                {
                  if (result.BuildPointsOnly)
                  {
                    result.PointsCleared.Add(p1);
                  }
                  else
                  {
                    int num3 = (int) this.ClearBlock(p1, method, playerID, false);
                  }
                  if (p1.Y < result.LowestY)
                    result.LowestY = p1.Y;
                }
              }
            }
          }
        }
        --p1.X;
        vector3_2.X = (float) p1.X;
        if (p1.X >= 0)
        {
          for (; p1.Z > p.Z - index && p1.Z > 0; --p1.Z)
          {
            if (p1.Z < this.MapSize.Z - 1 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              byte blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
              if (loading || (double) blastResistance < (double) blastStrength)
              {
                vector3_2.Z = (float) p1.Z;
                float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
                if ((double) num2 <= (double) num1 && (loading || (double) blastResistance < (double) blastStrength / (double) num2))
                {
                  if (result.BuildPointsOnly)
                  {
                    result.PointsCleared.Add(p1);
                  }
                  else
                  {
                    int num3 = (int) this.ClearBlock(p1, method, playerID, false);
                  }
                  if (p1.Y < result.LowestY)
                    result.LowestY = p1.Y;
                }
              }
            }
          }
        }
        for (; p1.X > p.X - index && p1.X > 0; --p1.X)
        {
          if (p1.X < this.MapSize.X - 1 && (index < blastRadius - 1 || rand.Next(4) > 0))
          {
            byte blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
            if (loading || (double) blastResistance < (double) blastStrength)
            {
              vector3_2.X = (float) p1.X;
              float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
              if ((double) num2 <= (double) num1 && (loading || (double) blastResistance < (double) blastStrength / (double) num2))
              {
                if (result.BuildPointsOnly)
                {
                  result.PointsCleared.Add(p1);
                }
                else
                {
                  int num3 = (int) this.ClearBlock(p1, method, playerID, false);
                }
                if (p1.Y < result.LowestY)
                  result.LowestY = p1.Y;
              }
            }
          }
        }
      }
    }

    public List<Point3D> GetPointsOfPenetration(Vector3 min, Vector3 max)
    {
      this.pointsOfPenetration.Clear();
      Point3D zero1 = Point3D.Zero;
      Vector3 zero2 = Vector3.Zero;
      for (float y = min.Y; (double) y < (double) max.Y + (double) this.TileSize; y += this.TileSize)
      {
        zero2.Y = y;
        if ((double) zero2.Y > (double) max.Y)
          zero2.Y = max.Y;
        for (float z = min.Z; (double) z < (double) max.Z + (double) this.TileSize; z += this.TileSize)
        {
          zero2.Z = z;
          if ((double) zero2.Z > (double) max.Z)
            zero2.Z = max.Z;
          for (float x = min.X; (double) x < (double) max.X + (double) this.TileSize; x += this.TileSize)
          {
            zero2.X = x;
            if ((double) zero2.X > (double) max.X)
              zero2.X = max.X;
            zero1.X = (int) ((double) zero2.X / (double) this.TileSize);
            zero1.Y = (int) ((double) zero2.Y / (double) this.TileSize);
            zero1.Z = (int) ((double) zero2.Z / (double) this.TileSize);
            this.pointsOfPenetration.Add(zero1);
          }
        }
      }
      return this.pointsOfPenetration;
    }

    public delegate bool AddToBlastedListHandler(Point3D orig, Point3D p);
  }
}
