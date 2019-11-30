// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.BWRandom
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class BWRandom
  {
    private int[] SeedArray = new int[56];
    private const int MBIG = 2147483647;
    private const int MSEED = 161803398;
    private const int MZ = 0;
    private int inext;
    private int inextp;
    private bool useLock;
    private object thislock;

    public BWRandom(bool useLock)
      : this(Environment.TickCount, useLock)
    {
    }

    public BWRandom(int seed, bool useLock)
    {
      this.Reseed(seed, useLock);
    }

    public void Reseed(int seed, bool useLock)
    {
      this.useLock = useLock;
      if (useLock && this.thislock == null)
        this.thislock = new object();
      else if (!useLock && this.thislock != null)
        this.thislock = (object) null;
      if (useLock)
      {
        lock (this.thislock)
          this.ReseedCore(seed);
      }
      else
        this.ReseedCore(seed);
    }

    private void ReseedCore(int seed)
    {
      int num1 = 161803398 - Math.Abs(seed);
      this.SeedArray[55] = num1;
      int num2 = 1;
      for (int index1 = 1; index1 < 55; ++index1)
      {
        int index2 = 21 * index1 % 55;
        this.SeedArray[index2] = num2;
        num2 = num1 - num2;
        if (num2 < 0)
          num2 += int.MaxValue;
        num1 = this.SeedArray[index2];
      }
      for (int index1 = 1; index1 < 5; ++index1)
      {
        for (int index2 = 1; index2 < 56; ++index2)
        {
          this.SeedArray[index2] -= this.SeedArray[1 + (index2 + 30) % 55];
          if (this.SeedArray[index2] < 0)
            this.SeedArray[index2] += int.MaxValue;
        }
      }
      this.inext = 0;
      this.inextp = 21;
    }

    protected double Sample()
    {
      if (!this.useLock)
        return this.SampleCore();
      lock (this.thislock)
        return this.SampleCore();
    }

    protected virtual double SampleCore()
    {
      int inext = this.inext;
      int inextp = this.inextp;
      int index1;
      if ((index1 = inext + 1) >= 56)
        index1 = 1;
      int index2;
      if ((index2 = inextp + 1) >= 56)
        index2 = 1;
      int num = this.SeedArray[index1] - this.SeedArray[index2];
      if (num < 0)
        num += int.MaxValue;
      this.SeedArray[index1] = num;
      this.inext = index1;
      this.inextp = index2;
      return (double) num * 4.6566128752458E-10;
    }

    public virtual int Next()
    {
      return (int) (this.Sample() * (double) int.MaxValue);
    }

    public virtual int Next(int minValue, int maxValue)
    {
      if (minValue > maxValue)
        throw new ArgumentOutOfRangeException(nameof (minValue), string.Format("Argument_MinMaxValue", (object) nameof (minValue), (object) nameof (maxValue)));
      int num = maxValue - minValue;
      if (num < 0)
        return (int) ((long) (this.Sample() * (double) ((long) maxValue - (long) minValue)) + (long) minValue);
      return (int) (this.Sample() * (double) num) + minValue;
    }

    public virtual int Next(int maxValue)
    {
      if (maxValue < 0)
        throw new ArgumentOutOfRangeException(nameof (maxValue), string.Format("ArgumentOutOfRange_MustBePositive", (object) nameof (maxValue)));
      return (int) (this.Sample() * (double) maxValue);
    }

    public virtual double NextDouble()
    {
      return this.Sample();
    }

    public virtual void NextBytes(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = (byte) (this.Sample() * 256.0);
    }

    public bool RandomChance(double chance)
    {
      return this.Sample() <= chance;
    }

    public bool RandomChanceTime(double seconds)
    {
      return (int) (this.Sample() * (seconds * 60.0)) == 0;
    }
  }
}
