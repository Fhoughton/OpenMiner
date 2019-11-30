// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlockDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner
{
  public struct BlockDataXML
  {
    public Block Name;
    public BlockMaterial Material;
    public DataBlockType ClassType;
    public byte Opacity;
    public byte Luminance;
    public float Friction;
    public float Dampen;
    public byte Buffer;
    public bool IsIcon;
    public bool IsAttached;
    public bool IsPassable;
    public bool IsRotated;
    public bool IsOrientated;
    public bool IsOreDeposit;
    public bool IsPowerEmitter;
    public bool IsPoweredMechanism;
    public bool IsVertSunlightUnhindered;
    public ushort BlastResistance;
    public byte WindAffect;
    public byte TextureID;
  }
}
