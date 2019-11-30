// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.EnumTypeOffsets
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.Diagnostics;

namespace StudioForge.TotalMiner.API
{
  public struct EnumTypeOffsets
  {
    public ushort BlockID;
    public ushort ItemID;
    public ushort DataBlockType;
    public ushort ArcadeMachine;
    public ushort PacketType;

    [DebuggerStepThrough]
    public static EnumTypeOffsets operator -(EnumTypeOffsets a, EnumTypeOffsets b)
    {
      EnumTypeOffsets enumTypeOffsets = new EnumTypeOffsets();
      if ((int) a.BlockID > (int) b.BlockID)
        enumTypeOffsets.BlockID = (ushort) ((uint) a.BlockID - (uint) b.BlockID);
      if ((int) a.ItemID > (int) b.ItemID)
        enumTypeOffsets.ItemID = (ushort) ((uint) a.ItemID - (uint) b.ItemID);
      if ((int) a.DataBlockType > (int) b.DataBlockType)
        enumTypeOffsets.DataBlockType = (ushort) ((uint) a.DataBlockType - (uint) b.DataBlockType);
      if ((int) a.ArcadeMachine > (int) b.ArcadeMachine)
        enumTypeOffsets.ArcadeMachine = (ushort) ((uint) a.ArcadeMachine - (uint) b.ArcadeMachine);
      if ((int) a.PacketType > (int) b.PacketType)
        enumTypeOffsets.PacketType = (ushort) ((uint) a.PacketType - (uint) b.PacketType);
      return enumTypeOffsets;
    }
  }
}
