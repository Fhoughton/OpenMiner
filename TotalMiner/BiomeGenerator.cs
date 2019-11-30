// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BiomeGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal abstract class BiomeGenerator
  {
    protected int seaLevel;
    protected MapTM map;
    protected int[] perm;

    public virtual void Initialize(MapTM map, int[] perm)
    {
      this.seaLevel = (int) map.SeaLevel;
      this.perm = perm;
    }

    public abstract float GetHeightNoise1(float x, float z);

    public abstract float GetHeightNoise2(float x, float z);

    public abstract float GetHeightNoise3(float x, float z);

    public abstract float GetDensityNoise(float x, float y, float z, float n1, float n2);
  }
}
