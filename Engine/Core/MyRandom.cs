// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.MyRandom
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.Core
{
  public class MyRandom : IRandom
  {
    private Random rand;

    public void Reinitialize()
    {
      this.rand = new Random();
    }

    public void Reinitialize(int seed)
    {
      this.rand = new Random(seed);
    }

    public int Next()
    {
      return this.rand.Next();
    }

    public int Next(int upper)
    {
      return this.rand.Next(upper);
    }

    public int Next(int lower, int upper)
    {
      return this.rand.Next(lower, upper);
    }

    public double NextDouble()
    {
      return this.rand.NextDouble();
    }
  }
}
