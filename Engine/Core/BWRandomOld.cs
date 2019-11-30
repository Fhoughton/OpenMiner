// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.BWRandomOld
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public struct BWRandomOld
  {
    private bool useLock;
    private Random random;

    public BWRandomOld(bool useLock)
    {
      this.useLock = useLock;
      this.random = new Random();
    }

    public BWRandomOld(int seed, bool useLock)
    {
      this.useLock = useLock;
      this.random = new Random(seed);
    }

    public void ReSeed(int seed, bool useLock)
    {
      this.useLock = useLock;
      this.random = new Random(seed);
    }

    public int Next()
    {
      if (!this.useLock)
        return this.random.Next();
      lock (this.random)
        return this.random.Next();
    }

    public int Next(int maxValue)
    {
      if (!this.useLock)
        return this.random.Next(maxValue);
      lock (this.random)
        return this.random.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
      if (!this.useLock)
        return this.random.Next(minValue, maxValue);
      lock (this.random)
        return this.random.Next(minValue, maxValue);
    }

    public void NextBytes(byte[] buffer)
    {
      if (this.useLock)
      {
        lock (this.random)
          this.random.NextBytes(buffer);
      }
      else
        this.random.NextBytes(buffer);
    }

    public double NextDouble()
    {
      if (!this.useLock)
        return this.random.NextDouble();
      lock (this.random)
        return this.random.NextDouble();
    }

    public bool RandomChance(double chance)
    {
      return this.NextDouble() <= chance;
    }

    public bool RandomChanceTime(double seconds)
    {
      return this.Next((int) (seconds * 60.0)) == 0;
    }
  }
}
