// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.DataBlock
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  public abstract class DataBlock
  {
    public GlobalPoint3D Point;

    public abstract DataBlockType ClassType { get; }

    public virtual bool HasInventory
    {
      get
      {
        return false;
      }
    }

    public DataBlock()
    {
    }

    public DataBlock(GlobalPoint3D p)
    {
      this.Point = p;
    }

    public virtual void SetScript(string name, DataBlockScriptType type)
    {
    }

    public virtual void RenameScript(string oldName, string newName)
    {
    }

    public virtual void BlockOpened()
    {
    }

    public virtual void BlockClosed()
    {
    }

    public virtual void CopyFrom(DataBlock from)
    {
      this.Point = from.Point;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.ReadStateCore(reader, version);
    }

    protected virtual void ReadStateCore(BinaryReader reader, int version)
    {
      this.Point.X = reader.ReadInt32();
      this.Point.Y = (int) reader.ReadInt16();
      this.Point.Z = reader.ReadInt32();
    }

    public void WriteState(BinaryWriter writer)
    {
      this.WriteStateCore(writer);
    }

    protected virtual void WriteStateCore(BinaryWriter writer)
    {
      writer.Write(this.Point.X);
      writer.Write((short) this.Point.Y);
      writer.Write(this.Point.Z);
    }
  }
}
