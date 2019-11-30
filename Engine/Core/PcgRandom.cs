// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PcgRandom
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public class PcgRandom
  {
    private ulong state;
    private ulong inc;

    public PcgRandom()
    {
      this.Seed(9600629759793949339UL, 15726070495360670683UL);
    }

    public PcgRandom(int seed)
    {
      this.Seed(seed);
    }

    public PcgRandom(ulong seed)
    {
      this.Seed(seed, 15726070495360670683UL);
    }

    public PcgRandom(ulong seed, ulong initSeq)
    {
      this.Seed(seed, initSeq);
    }

    public void Seed(int seed)
    {
      this.Seed((ulong) seed, 15726070495360670683UL);
    }

    public void Seed(ulong seed, ulong initSeq)
    {
      this.state = 0UL;
      this.inc = initSeq << 1 | 1UL;
      this.Next();
      this.state += seed;
      this.Next();
    }

    public int Next()
    {
      int num = (int) this.NextUint();
      if (num < 0)
        num = -num;
      return num;
    }

    public int Next(int max)
    {
      int num = (int) this.NextUint((uint) max);
      if (num < 0)
        num = -num;
      return num;
    }

    public int Next(int min, int max)
    {
      return this.Next(max - min) + min;
    }

    public uint NextUint()
    {
      ulong state = this.state;
      this.state = state * 6364136223846793005UL + this.inc;
      uint num1 = (uint) ((state >> 18 ^ state) >> 27);
      int num2 = (int) (state >> 59);
      return num1 >> num2 | num1 << -num2;
    }

    public uint NextUint(uint max)
    {
      if (max == 0U)
        max = 1U;
      uint num1 = (uint) ((4294967296UL - (ulong) max) % (ulong) max);
      uint num2;
      do
      {
        num2 = this.NextUint();
      }
      while (num2 < num1);
      return num2 % max;
    }

    public uint NextUint(uint min, uint max)
    {
      return this.NextUint(max - min) + min;
    }

    public double NextDouble()
    {
      return (double) this.NextUint() / (double) uint.MaxValue;
    }

    public bool RandomChance(double chance)
    {
      return this.NextDouble() <= chance;
    }

    public bool RandomChanceTime(double seconds)
    {
      return (int) (this.NextDouble() * (seconds * 60.0)) == 0;
    }

    public override string ToString()
    {
      return string.Format("[Pcg state: {0}; sequence: {1}]", (object) this.state, (object) this.inc);
    }
  }
}
