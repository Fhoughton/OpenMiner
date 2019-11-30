// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TerrainData
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public class TerrainData
  {
    public BiomeType Biome;
    public int Iterations;
    public int MaxParticles;
    public Item GroundBlock;
    public ushort SeaLevel;

    public TerrainData Clone()
    {
      return new TerrainData()
      {
        Biome = this.Biome,
        Iterations = this.Iterations,
        MaxParticles = this.MaxParticles,
        GroundBlock = this.GroundBlock,
        SeaLevel = this.SeaLevel
      };
    }
  }
}
