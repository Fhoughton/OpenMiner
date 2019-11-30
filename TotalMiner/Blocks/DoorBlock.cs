// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.DoorBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.Blocks
{
  internal class DoorBlock : PlayerBlock
  {
    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Door;
      }
    }

    public DoorBlock()
    {
    }

    public DoorBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public DoorBlock(GlobalPoint3D p, Player player)
      : base(p, player)
    {
    }
  }
}
