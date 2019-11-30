// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FloraManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Generators;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class FloraManager
  {
    private Queue<GlobalPoint3D> treeRemoved = new Queue<GlobalPoint3D>();
    private Queue<GlobalPoint3D> leavesToCheckForDecay = new Queue<GlobalPoint3D>();
    private const int mushroomDensity = 4;
    private Map map;
    private GameInstance instance;
    private GlobalPoint3D mapBoundMin;
    private GlobalPoint3D mapBoundMax;
    private int leafDecayTimer;
    private long genFreqFlora;
    private long lastFreqLongGrass;

    public FloraManager(GameInstance instance, Map map)
    {
      this.instance = instance;
      this.map = map;
      this.mapBoundMin = map.MapBound.Min;
      this.mapBoundMax = map.MapBound.Max;
      this.genFreqFlora = 30000L;
    }

    public void Update()
    {
      if (!this.instance.IsFiniteResources)
        return;
      this.UpdateLeafDecay();
      this.GenerateMushrooms();
    }

    public void UpdateLeafDecay()
    {
      if (--this.leafDecayTimer > 0)
        return;
      this.leafDecayTimer = 7;
      if (this.leavesToCheckForDecay.Count > 0)
      {
        this.UpdateLeafDecayCore(this.leavesToCheckForDecay.Dequeue());
      }
      else
      {
        GlobalPoint3D p = GlobalPoint3D.Zero;
        lock (this.treeRemoved)
        {
          if (this.treeRemoved.Count == 0)
            return;
          p = this.treeRemoved.Dequeue();
        }
        this.UpdateFindLeafDecay(p);
      }
    }

    private void UpdateLeafDecayCore(GlobalPoint3D p)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Y = p.Y - 4; zero.Y < p.Y + 5; ++zero.Y)
      {
        for (zero.Z = p.Z - 4; zero.Z < p.Z + 5; ++zero.Z)
        {
          for (zero.X = p.X - 4; zero.X < p.X + 5; ++zero.X)
          {
            if (this.map.GetBlockID(zero) == (byte) 5 && (this.map.IsNextTo(zero, (byte) 8, -1, true, false) || this.map.IsNextTo(zero, (byte) 63, -1, true, false) || this.map.IsNextTo(zero, (byte) 162, -1, true, false)))
              return;
          }
        }
      }
      this.instance.ClearBlock(p, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
      this.map.Commit();
    }

    private void UpdateFindLeafDecay(GlobalPoint3D p)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Y = p.Y - 5; zero.Y < p.Y + 6; ++zero.Y)
      {
        for (zero.Z = p.Z - 5; zero.Z < p.Z + 6; ++zero.Z)
        {
          for (zero.X = p.X - 5; zero.X < p.X + 6; ++zero.X)
          {
            if (ItemData.IsSubTypeAny((Block) this.map.GetBlockID(zero), ItemSubType.Leaves))
              this.leavesToCheckForDecay.Enqueue(zero);
          }
        }
      }
    }

    public void TreeRemoved(GlobalPoint3D p)
    {
      if (!this.instance.IsFiniteResources)
        return;
      lock (this.treeRemoved)
        this.treeRemoved.Enqueue(p);
    }

    private void GenerateMushrooms()
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.mapBoundMin.X + this.instance.Random.Next(this.map.MapSize.X);
      p.Z = this.mapBoundMin.Z + this.instance.Random.Next(this.map.MapSize.Z);
      MapRegion region = this.map.GetRegion(p);
      if (region == null)
        return;
      p.Y = (int) region.GetHeight(p);
      if (p.Y >= this.mapBoundMax.Y - 1)
        return;
      MapBlock blockIdAndAuxNoCache = this.map.GetBlockIDAndAuxNoCache(p);
      if (this.map.HasChanged(blockIdAndAuxNoCache))
        return;
      Block blockID = (Block) blockIdAndAuxNoCache.BlockID;
      if (ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves))
      {
        Block block = Block.Leaves;
        for (; p.Y > this.mapBoundMin.Y && (ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves) || blockID == Block.None); blockID = (Block) this.map.GetBlockIDNoCache(p))
        {
          --p.Y;
          block = blockID;
        }
        if (block != Block.None || !BlockData.IsGrassOrDirt(blockID))
          return;
        ++p.Y;
        if (this.map.GetLightNoCache(p).SunLight >= (byte) 6 || this.IsNextTo(p, (byte) 127) >= 2)
          return;
        this.map.SetBlockData(p, (byte) 127, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
        this.map.Commit();
      }
      else
      {
        if (blockID != Block.Grass)
          return;
        long elapsedMilliseconds = Globals1.ElapsedWatch.ElapsedMilliseconds;
        if (elapsedMilliseconds <= this.lastFreqLongGrass + this.genFreqFlora)
          return;
        ++p.Y;
        if (this.map.GetBlockIDNoCache(p) != (byte) 0)
          return;
        switch (this.map.Random.Next(6))
        {
          case 0:
          case 1:
          case 2:
            VegetationGenerator.GrassDecoration(this.map, p, 1f, 3, 10, 4, 0.4f, 10, this.mapBoundMax.Y - 2, this.map.Random, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            break;
          case 3:
          case 4:
            VegetationGenerator.FlowerDecoration(this.map, p, 1f, 3, 10, 4, 0.4f, this.mapBoundMax.Y - 2, this.map.Random, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            break;
          default:
            this.map.SetBlockData(p, (byte) 58, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            break;
        }
        this.map.Commit();
        this.lastFreqLongGrass = elapsedMilliseconds;
      }
    }

    private int IsNextTo(GlobalPoint3D p, byte blockID)
    {
      int num = 0;
      --p.X;
      if (p.X >= this.mapBoundMin.X && (int) this.map.GetBlockIDNoCache(p) == (int) blockID)
        ++num;
      ++p.X;
      --p.Z;
      if (p.Z >= this.mapBoundMin.Z && (int) this.map.GetBlockIDNoCache(p) == (int) blockID)
        ++num;
      ++p.Z;
      ++p.X;
      if (p.X < this.mapBoundMax.X && (int) this.map.GetBlockIDNoCache(p) == (int) blockID)
        ++num;
      --p.X;
      ++p.Z;
      if (p.Z < this.mapBoundMax.Z && (int) this.map.GetBlockIDNoCache(p) == (int) blockID)
        ++num;
      return num;
    }
  }
}
