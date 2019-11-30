// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TemperateBiomeGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class TemperateBiomeGenerator : BiomeGenerator
  {
    public override float GetHeightNoise1(float x, float z)
    {
      return (float) (((double) SimplexNoise1.noise(x / 2700f, z / 2700f, this.perm) * 10.0 + (double) SimplexNoise1.noise(x / 195f, z / 195f, this.perm) + (double) SimplexNoise1.noise(x / 23f, z / 23f, this.perm) * 0.100000001490116) / 11.1000003814697);
    }

    public override float GetHeightNoise2(float x, float z)
    {
      return (float) (((double) SimplexNoise1.noise(x / 400f, z / 400f, this.perm) * 8.0 + (double) SimplexNoise1.noise(x / 40f, z / 40f, this.perm) * 2.0 + (double) SimplexNoise1.noise(x / 4f, z / 4f, this.perm)) / 11.0);
    }

    public override float GetHeightNoise3(float x, float z)
    {
      return (float) (((double) SimplexNoise1.noise(x / 80f, z / 80f, this.perm) + (double) SimplexNoise1.noise(x / 8f, z / 8f, this.perm) * 0.100000001490116) / 11.1000003814697);
    }

    public override float GetDensityNoise(float x, float y, float z, float n1, float n2)
    {
      float num1 = (float) (200.0 + (double) n1 * 50.0);
      float num2 = num1 * 0.1f;
      return (float) (((double) SimplexNoise1.noise(x / num1, y / num1, z / num1, this.perm) * 5.0 + (double) SimplexNoise1.noise(x / num2, y / num2, z / num2, this.perm)) / 6.0);
    }
  }
}
