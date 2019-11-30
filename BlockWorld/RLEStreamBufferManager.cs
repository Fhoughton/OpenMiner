// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.RLEStreamBufferManager
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class RLEStreamBufferManager
  {
    public byte[][] Stream;
    private int freeIndex;
    private short freeStreamID;
    private int initialStreamSize;
    private int expansionStreamSize;
    private int allocatedBytes;
    private LinkedList<RLEStreamBufferManager.ReleasedData> releasedList;
    private List<RLEStreamBufferManager.ReleasedData2Bytes> releasedList2Bytes;

    public event EventHandler CacheCompacted;

    private void RaiseCacheCompacted()
    {
      if (this.CacheCompacted == null)
        return;
      this.CacheCompacted((object) this, EventArgs.Empty);
    }

    public event EventHandler CacheExpanded;

    private void RaiseCacheExpanded()
    {
      if (this.CacheExpanded == null)
        return;
      this.CacheExpanded((object) this, EventArgs.Empty);
    }

    public int Allocated
    {
      get
      {
        int num = 0;
        try
        {
          for (int index = 0; index < (int) this.freeStreamID; ++index)
          {
            if (index < this.Stream.Length)
            {
              if (this.Stream[index] != null)
                num += this.Stream[index].Length;
            }
            else
              break;
          }
        }
        catch (Exception ex)
        {
        }
        return num + this.freeIndex;
      }
    }

    public int ArraySize
    {
      get
      {
        int num = 0;
        try
        {
          num += this.releasedList.Count * 32 + this.releasedList2Bytes.Count * 8;
          for (int index = 0; index < this.Stream.Length; ++index)
          {
            if (this.Stream[index] != null)
              num += this.Stream[index].Length + 8;
          }
        }
        catch (Exception ex)
        {
        }
        return num;
      }
    }

    public int ReleasedCount
    {
      get
      {
        return this.releasedList.Count + this.releasedList2Bytes.Count;
      }
    }

    public int ReleasedCount2Bytes
    {
      get
      {
        return this.releasedList2Bytes.Count;
      }
    }

    public int ReleasedCountVarBytes
    {
      get
      {
        return this.releasedList.Count;
      }
    }

    public int ReleasedBytesFree
    {
      get
      {
        int num = 0;
        try
        {
          for (LinkedListNode<RLEStreamBufferManager.ReleasedData> linkedListNode = this.releasedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
            num += linkedListNode.Value.BufferSize;
          num += this.releasedList2Bytes.Count * 2;
        }
        catch (Exception ex)
        {
        }
        return num;
      }
    }

    public int ForCollection()
    {
      return this.Allocated - this.allocatedBytes;
    }

    public void Initialize(int _initialStreamSize, int _expansionStreamSize)
    {
      this.initialStreamSize = _initialStreamSize;
      this.expansionStreamSize = _expansionStreamSize;
      this.Stream = new byte[1][];
      this.Stream[0] = new byte[this.initialStreamSize];
      this.freeStreamID = (short) 0;
      this.freeIndex = 0;
      this.releasedList = new LinkedList<RLEStreamBufferManager.ReleasedData>();
      this.releasedList2Bytes = new List<RLEStreamBufferManager.ReleasedData2Bytes>();
    }

    public void Release()
    {
      lock (BuffLock.StreamLock)
      {
        if (this.Stream[0].Length > this.initialStreamSize)
          this.Stream[0] = new byte[this.initialStreamSize];
        for (int index = 1; index < this.Stream.Length; ++index)
          this.Stream[index] = (byte[]) null;
        this.freeIndex = 0;
        this.freeStreamID = (short) 0;
        this.releasedList.Clear();
        this.releasedList2Bytes.Clear();
      }
    }

    public void Release(RLEStreamByte rle)
    {
      if (rle.StreamID < (short) 0)
        return;
      lock (BuffLock.StreamLock)
      {
        this.ReleaseCore(rle.StreamID, rle.StreamIndex, rle.BufferSize);
        rle.StreamID = (short) -1;
      }
    }

    public void Release(short streamID, int streamIndex, int bufferSize)
    {
      if (streamID < (short) 0)
        return;
      lock (BuffLock.StreamLock)
        this.ReleaseCore(streamID, streamIndex, bufferSize);
    }

    private void ReleaseCore(short streamID, int streamIndex, int bufferSize)
    {
      if (bufferSize == 2)
        this.releasedList2Bytes.Add(new RLEStreamBufferManager.ReleasedData2Bytes()
        {
          StreamID = streamID,
          StreamIndex = streamIndex
        });
      else
        this.releasedList.AddFirst(new RLEStreamBufferManager.ReleasedData()
        {
          StreamID = streamID,
          StreamIndex = streamIndex,
          BufferSize = bufferSize
        });
    }

    public void Allocate(int size, out short streamID, out int streamIndex)
    {
      lock (BuffLock.StreamLock)
      {
        this.allocatedBytes += size;
        this.AllocateFromReleaseListExact(size, out streamID, out streamIndex);
        if (streamID != (short) -1)
          return;
        if (this.freeIndex + size >= this.Stream[(int) this.freeStreamID].Length)
          this.Expand(size);
        streamID = this.freeStreamID;
        streamIndex = this.freeIndex;
        this.freeIndex += size;
      }
    }

    public void Allocate(RLEStreamByte rle, int size)
    {
      lock (BuffLock.StreamLock)
      {
        this.allocatedBytes = this.allocatedBytes - rle.BufferSize + size;
        if (size >= rle.BufferSize - 10 && size <= rle.BufferSize && rle.StreamID >= (short) 0)
        {
          rle.StreamSize = size;
        }
        else
        {
          this.Release(rle);
          short streamID;
          int streamIndex;
          int bufferSize;
          this.AllocateFromReleaseList(size, out streamID, out streamIndex, out bufferSize);
          if (streamID >= (short) 0)
          {
            rle.StreamID = streamID;
            rle.StreamIndex = streamIndex;
            rle.StreamSize = size;
            rle.BufferSize = bufferSize;
          }
          else
          {
            if (this.freeIndex + size >= this.Stream[(int) this.freeStreamID].Length)
              this.Expand(size);
            rle.StreamID = this.freeStreamID;
            rle.StreamIndex = this.freeIndex;
            rle.StreamSize = size;
            rle.BufferSize = size;
            this.freeIndex += size;
          }
        }
      }
    }

    private void AllocateFromReleaseList(
      int streamSize,
      out short streamID,
      out int streamIndex,
      out int bufferSize)
    {
      streamID = (short) -1;
      streamIndex = 0;
      bufferSize = 0;
      if (streamSize == 2)
      {
        if (this.releasedList2Bytes.Count <= 0)
          return;
        RLEStreamBufferManager.ReleasedData2Bytes releasedList2Byte = this.releasedList2Bytes[this.releasedList2Bytes.Count - 1];
        streamID = releasedList2Byte.StreamID;
        streamIndex = releasedList2Byte.StreamIndex;
        bufferSize = 2;
        this.releasedList2Bytes.RemoveAt(this.releasedList2Bytes.Count - 1);
      }
      else
      {
        LinkedListNode<RLEStreamBufferManager.ReleasedData> node = (LinkedListNode<RLEStreamBufferManager.ReleasedData>) null;
        for (LinkedListNode<RLEStreamBufferManager.ReleasedData> linkedListNode = this.releasedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (linkedListNode.Value.BufferSize >= streamSize && linkedListNode.Value.BufferSize <= (int) ((double) streamSize * 1.5) && (node == null || linkedListNode.Value.BufferSize - streamSize < node.Value.BufferSize - streamSize))
            node = linkedListNode;
        }
        if (node == null)
          return;
        streamID = node.Value.StreamID;
        streamIndex = node.Value.StreamIndex;
        bufferSize = node.Value.BufferSize;
        this.releasedList.Remove(node);
      }
    }

    private void AllocateFromReleaseListExact(
      int streamSize,
      out short streamID,
      out int streamIndex)
    {
      streamID = (short) -1;
      streamIndex = 0;
      if (streamSize == 2)
      {
        if (this.releasedList2Bytes.Count <= 0)
          return;
        RLEStreamBufferManager.ReleasedData2Bytes releasedList2Byte = this.releasedList2Bytes[this.releasedList2Bytes.Count - 1];
        streamID = releasedList2Byte.StreamID;
        streamIndex = releasedList2Byte.StreamIndex;
        this.releasedList2Bytes.RemoveAt(this.releasedList2Bytes.Count - 1);
      }
      else
      {
        for (LinkedListNode<RLEStreamBufferManager.ReleasedData> node = this.releasedList.First; node != null; node = node.Next)
        {
          if (node.Value.BufferSize == streamSize)
          {
            streamID = node.Value.StreamID;
            streamIndex = node.Value.StreamIndex;
            this.releasedList.Remove(node);
            break;
          }
        }
      }
    }

    private void Expand(int sizeRequested)
    {
      if (this.Stream.Length <= (int) this.freeStreamID + 1)
      {
        byte[][] numArray = new byte[(int) this.freeStreamID + 3][];
        Array.Copy((Array) this.Stream, (Array) numArray, this.Stream.Length);
        this.Stream = numArray;
      }
      this.Stream[(int) ++this.freeStreamID] = new byte[Math.Max(sizeRequested, this.expansionStreamSize)];
      this.freeIndex = 0;
      this.RaiseCacheExpanded();
    }

    private struct ReleasedData
    {
      public short StreamID;
      public int StreamIndex;
      public int BufferSize;
    }

    private struct ReleasedData2Bytes
    {
      public short StreamID;
      public int StreamIndex;
    }
  }
}
