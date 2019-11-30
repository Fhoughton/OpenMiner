// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.DummyDataBlock
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  public class DummyDataBlock : DataBlock
  {
    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.None;
      }
    }

    public DummyDataBlock()
    {
    }

    public DummyDataBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
    }
  }
}
