// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlockMaterialDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public struct BlockMaterialDataXML
  {
    public BlockMaterial Material;
    public BlockMaterialFlags Flags;
    public ushort Resistance;
    public ushort BaseEfficiency;
    public ushort PickEfficiency;
    public ushort ShovelEfficiency;
    public ushort HatchetEfficiency;
    public ushort WeaponEfficiency;
    public float XPAdjust;

    public bool HasFlag(BlockMaterialFlags flags)
    {
      return (this.Flags & flags) > BlockMaterialFlags.None;
    }

    public bool HasFlags(BlockMaterialFlags flags)
    {
      return (this.Flags & flags) == flags;
    }
  }
}
