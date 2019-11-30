// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ModBlockDataXML
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner
{
  public struct ModBlockDataXML
  {
    public Block BlockID;
    public BlockMaterial? Material;
    public DataBlockType? ClassType;
    public byte? Opacity;
    public byte? Luminance;
    public float? Friction;
    public float? Dampen;
    public byte? Buffer;
    public bool? IsIcon;
    public bool? IsAttached;
    public bool? IsPassable;
    public bool? IsRotated;
    public bool? IsOrientated;
    public bool? IsOreDeposit;
    public bool? IsPowerEmitter;
    public bool? IsPoweredMechanism;
    public bool? IsVertSunlightUnhindered;
    public ushort? BlastResistance;
    public byte? WindAffect;
    public byte? TextureID;
  }
}
