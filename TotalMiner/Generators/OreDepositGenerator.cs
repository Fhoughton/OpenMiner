// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.OreDepositGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner.Generators
{
  internal class OreDepositGenerator : IThreadWorkItem
  {
    private Map map;
    private GlobalPoint3D min;
    private GlobalPoint3D max;
    private float density;
    private GamerID playerID;
    private float volume;
    private int opIndex;
    private IProgressBar progress;
    private CreativeModeHelper helper;

    public string Name
    {
      get
      {
        return nameof (OreDepositGenerator);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public void Initialize(
      Map map,
      CreativeModeHelper helper,
      int opIndex,
      GlobalPoint3D min,
      GlobalPoint3D max,
      float density,
      GamerID playerID,
      IProgressBar progress)
    {
      this.map = map;
      this.helper = helper;
      this.opIndex = opIndex;
      this.min = min;
      this.max = max;
      this.density = density;
      this.playerID = playerID;
      this.progress = progress;
      GlobalPoint3D globalPoint3D = max - min;
      this.volume = (float) (globalPoint3D.X * globalPoint3D.Y * globalPoint3D.Z);
    }

    public void Update()
    {
      this.density *= 12f;
      float inc = 1f / (this.progress != null ? (float) this.CountProgressTicks() : 0.0f);
      foreach (OreProperty ore in OreProperties.Ores)
        this.CreateOreDeposits(ore, inc);
    }

    private void CreateOreDeposits(OreProperty rock, float inc)
    {
      int oreDepositCount = this.GetOreDepositCount(rock);
      for (int index = 0; index < oreDepositCount; ++index)
      {
        if (this.progress != null)
          this.progress.AddProgress(inc);
        this.CreateOreDeposit(rock);
        this.map.Commit();
      }
    }

    private int CountProgressTicks()
    {
      int num = 0;
      foreach (OreProperty ore in OreProperties.Ores)
        num += this.GetOreDepositCount(ore);
      return num;
    }

    private int GetOreDepositCount(OreProperty ore)
    {
      float num = this.volume / 1.321206E+08f;
      return Math.Max(1, (int) ((double) ore.DepositFrequency * (double) this.density * (double) num));
    }

    private void CreateOreDeposit(OreProperty ore)
    {
      GlobalPoint3D globalPoint3D = this.max - this.min + GlobalPoint3D.One;
      int x = this.min.X + this.map.Random.Next(globalPoint3D.X);
      int z = this.min.Z + this.map.Random.Next(globalPoint3D.Z);
      int num = (int) ((double) ore.MinDepth * (double) globalPoint3D.Y);
      int y = this.max.Y - (this.map.Random.Next((int) ((double) ore.MaxDepth * (double) globalPoint3D.Y) - num) + num);
      this.CreateOreDeposit(ore, new GlobalPoint3D(x, y, z));
    }

    private void CreateOreDeposit(OreProperty ore, GlobalPoint3D p)
    {
      float num1 = Math.Min((float) ((double) this.volume / 132120576.0 * 10.0), 1f / this.density);
      int num2 = Math.Max(1, (int) (this.map.Random.NextDouble() * (double) ore.DepositSize * 0.200000002980232 + (double) ore.DepositSize * 0.800000011920929 * (double) this.density * (double) num1));
      for (int index = 0; index < num2; ++index)
      {
        p.X += this.map.Random.Next(3) - 1;
        p.Y += this.map.Random.Next(3) - 1;
        p.Z += this.map.Random.Next(3) - 1;
        p = this.map.Clamp(p, 2);
        if (p.X < this.min.X)
          p.X = this.min.X;
        else if (p.X > this.max.X)
          p.X = this.max.X;
        if (p.Y < this.min.Y)
          p.Y = this.min.Y;
        else if (p.Y > this.max.Y)
          p.Y = this.max.Y;
        if (p.Z < this.min.Z)
          p.Z = this.min.Z;
        else if (p.Z > this.max.Z)
          p.Z = this.max.Z;
        Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
        if (blockIdNoCache >= Block.None && blockIdNoCache < Block.Bedrock)
        {
          MapChunk chunk = this.map.SetBlockData(p, (byte) ore.BlockID, (byte) 0, UpdateBlockMethod.CreativeHelper, this.playerID, true);
          if (chunk != null)
            this.map.ChunkCacheManager.SetChunkCacheForImmedaiteClear(chunk);
        }
      }
    }
  }
}
