// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PlayerSurroundings
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class PlayerSurroundings : TimedThreadWorkItem
  {
    private GlobalPoint3D p = new GlobalPoint3D();
    private GlobalPoint3D op = new GlobalPoint3D();
    private List<GlobalPoint3D> possibleTorches = new List<GlobalPoint3D>(100);
    private List<GlobalPoint3D> possibleLeafDrops = new List<GlobalPoint3D>(100);
    private Dictionary<int, bool> possibleLeafDropHash = new Dictionary<int, bool>(100);
    private Color[] emberColors = new Color[5]
    {
      Color.White,
      Color.Black,
      Color.Yellow,
      Color.Red,
      Color.Orange
    };
    private const int radiusX = 10;
    private const int radiusY = 8;
    private GameInstance instance;
    private MapTM map;
    private MapStrategyTM strategy;
    private PcgRandom random;
    private bool needCommit;
    private int lavaCount;
    private int waterCount;
    private int fireCount;

    public override string Name
    {
      get
      {
        return nameof (PlayerSurroundings);
      }
    }

    public PlayerSurroundings(GameInstance instance, PriorityLevel priority)
      : base(priority, 1000)
    {
      this.instance = instance;
      this.map = instance.Map;
      this.strategy = instance.MapStrategyTM;
      this.random = this.map.Random;
    }

    protected override void UpdateCore()
    {
      if (!this.instance.IsMapActive || this.strategy == null)
        return;
      this.needCommit = false;
      this.lavaCount = this.waterCount = this.fireCount = 0;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
        this.UpdatePlayer(localEnabledPlayer.VirtualPlayer);
      this.strategy.EnvManager.SetSurroundings((float) this.fireCount / 80f, (float) this.lavaCount / 2000f, (float) this.waterCount / 100f);
      if (!this.needCommit)
        return;
      this.map.Commit();
    }

    public void UpdatePlayer(Player player)
    {
      this.op = this.map.GetPoint(player.EyePosition);
      this.possibleTorches.Clear();
      this.possibleLeafDrops.Clear();
      this.possibleLeafDropHash.Clear();
      for (this.p.Y = this.op.Y - 8; this.p.Y <= this.op.Y + 8; ++this.p.Y)
      {
        for (this.p.Z = this.op.Z - 10; this.p.Z <= this.op.Z + 10; ++this.p.Z)
        {
          for (this.p.X = this.op.X - 10; this.p.X <= this.op.X + 10; ++this.p.X)
            this.UpdateBlock();
        }
      }
      if (this.possibleLeafDrops.Count <= 0)
        return;
      this.UpdateFallingLeaf(player);
    }

    private void UpdateBlock()
    {
      bool flag = true;
      Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(this.p);
      Block block = blockIdNoCache;
      if ((uint) block <= 46U)
      {
        switch (block)
        {
          case Block.Leaves:
            break;
          case Block.Water:
            if (!flag || this.map.GetAuxDataNoCache(this.p) <= (byte) 0)
              return;
            ++this.waterCount;
            return;
          case Block.Copper:
            return;
          case Block.Lava:
            if (flag)
            {
              if (this.map.GetAuxDataNoCache(this.p) > (byte) 0)
                this.lavaCount += 100;
              else
                ++this.lavaCount;
            }
            if (this.instance.IsFiniteResources && this.random.Next(5) == 0)
              this.UpdateLavaBlock();
            if (this.random.Next(15) != 0)
              return;
            ++this.p.Y;
            byte blockId = this.map.GetBlockID(this.p);
            --this.p.Y;
            if (blockId != (byte) 0)
              return;
            this.AddLavaParticle();
            return;
          case Block.Torch:
            if (this.random.Next(3) != 0)
              return;
            this.UpdateTorchEmbers(this.p);
            return;
          default:
            return;
        }
      }
      else
      {
        switch (block)
        {
          case Block.PineLeaves:
          case Block.MapleLeaves:
            break;
          case Block.Fire:
            if (flag)
              ++this.fireCount;
            if (this.random.Next(2) != 0)
              return;
            this.UpdateFireEmbers(this.p);
            return;
          default:
            return;
        }
      }
      this.UpdateFindLeafToDrop(blockIdNoCache);
    }

    private void AddLavaParticle()
    {
      Vector3 blockCenter = this.map.GetBlockCenter(this.p);
      ParticleData data = new ParticleData();
      data.Duration = (ushort) (this.random.NextDouble() * 400.0 + 2600.0);
      data.StartColor = this.emberColors[this.random.Next(this.emberColors.Length)];
      data.EndColor = new Color(100, 100, 100, 230);
      float num = (float) (this.random.NextDouble() * 0.0399999991059303 + 0.0199999995529652);
      data.Size = new Vector4(num, num, num, 1f);
      data.Velocity.X = (float) (this.random.NextDouble() * 0.0500000007450581 - 0.025000000372529);
      data.Velocity.Y = (float) (this.random.NextDouble() * 0.25 + 0.25);
      data.Velocity.Z = (float) (this.random.NextDouble() * 0.0500000007450581 - 0.025000000372529);
      this.instance.EmitterParticleSystem.AddParticle(blockCenter, ref data);
    }

    private void UpdateLavaBlock()
    {
      GlobalPoint3D p = this.p;
      int num = this.map.Random.Next(3);
      switch (num)
      {
        case 0:
        case 1:
          ++p.Y;
          GlobalPoint3D randomPoint1 = this.GetRandomPoint(p);
          if (this.map.GetBlockIDNoCache(randomPoint1) != (byte) 0)
            break;
          if (this.HasAdjacentFlammable(randomPoint1))
          {
            this.map.SetBlockData(randomPoint1, (byte) 118, (byte) 1, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            this.needCommit = true;
            break;
          }
          if (num != 1)
            break;
          ++p.Y;
          GlobalPoint3D randomPoint2 = this.GetRandomPoint(p);
          if (this.map.GetBlockIDNoCache(randomPoint2) != (byte) 0 || !this.HasAdjacentFlammable(randomPoint2))
            break;
          this.map.SetBlockData(randomPoint2, (byte) 118, (byte) 1, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
          this.needCommit = true;
          break;
        default:
          for (int index = 0; index < 3; ++index)
          {
            GlobalPoint3D randomPoint3 = this.GetRandomPoint(p);
            if (ItemData2.GetBurnTime(this.map, this.p, (Item) this.map.GetBlockIDNoCache(this.p)) > (ushort) 0)
            {
              ++randomPoint3.Y;
              if (this.map.GetBlockIDNoCache(randomPoint3) == (byte) 0)
              {
                this.map.SetBlockData(randomPoint3, (byte) 118, (byte) 1, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
                this.needCommit = true;
                break;
              }
            }
          }
          break;
      }
    }

    private GlobalPoint3D GetRandomPoint(GlobalPoint3D p)
    {
      p.X += this.map.Random.Next(3) - 1;
      p.Z += this.map.Random.Next(3) - 1;
      return p;
    }

    private bool HasAdjacentFlammable(GlobalPoint3D p)
    {
      --p.X;
      if (ItemData2.GetBurnTime(this.map, p, (Item) this.map.GetBlockIDNoCache(p)) > (ushort) 0)
        return true;
      p.X += 2;
      if (ItemData2.GetBurnTime(this.map, p, (Item) this.map.GetBlockIDNoCache(p)) > (ushort) 0)
        return true;
      --p.X;
      --p.Z;
      if (ItemData2.GetBurnTime(this.map, p, (Item) this.map.GetBlockIDNoCache(p)) > (ushort) 0)
        return true;
      p.Z += 2;
      return ItemData2.GetBurnTime(this.map, p, (Item) this.map.GetBlockIDNoCache(p)) > (ushort) 0;
    }

    private void UpdateFireEmbers(GlobalPoint3D p)
    {
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.Y += this.map.TileSize * 0.4f;
      Vector3 pos = new Vector3();
      ParticleData data = new ParticleData();
      data.EndColor = new Color(50, 50, 50, 200);
      int num1 = this.random.Next(1, 3);
      for (int index = 0; index < num1; ++index)
      {
        pos.X = (float) ((double) blockCenter.X + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        pos.Y = (float) ((double) blockCenter.Y + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        pos.Z = (float) ((double) blockCenter.Z + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        data.Duration = (ushort) (this.random.NextDouble() * 400.0 + 1000.0);
        data.StartColor = this.emberColors[this.random.Next(this.emberColors.Length)];
        float num2 = (float) (this.random.NextDouble() * 0.0399999991059303 + 0.0399999991059303);
        data.Size = new Vector4(num2, num2, num2, 1.2f);
        data.Velocity.X = (float) (this.random.NextDouble() * 0.300000011920929 - 0.150000005960464);
        data.Velocity.Y = (float) (this.random.NextDouble() * 0.200000002980232 + 0.100000001490116);
        data.Velocity.Z = (float) (this.random.NextDouble() * 0.300000011920929 - 0.150000005960464);
        this.instance.EmitterParticleSystem.AddParticle(pos, ref data);
      }
    }

    private void UpdateTorchEmbers(GlobalPoint3D p)
    {
      Vector3 vector3 = this.map.GetBlockCenter(p) + GraphicStatics.TorchParticlesOffset[(int) this.map.GetAuxDataNoCache(p) % GraphicStatics.TorchParticlesOffset.Length];
      Vector3 pos = new Vector3();
      ParticleData data = new ParticleData();
      data.EndColor = new Color(50, 50, 50, 200);
      int num1 = this.random.Next(1, 3);
      for (int index = 0; index < num1; ++index)
      {
        pos.X = (float) ((double) vector3.X + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        pos.Y = (float) ((double) vector3.Y + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        pos.Z = (float) ((double) vector3.Z + this.random.NextDouble() * 0.200000002980232 - 0.100000001490116);
        data.Duration = (ushort) (this.random.NextDouble() * 400.0 + 1000.0);
        data.StartColor = this.emberColors[this.random.Next(this.emberColors.Length)];
        float num2 = (float) (this.random.NextDouble() * 0.0399999991059303 + 0.0399999991059303);
        data.Size = new Vector4(num2, num2, num2, 1.2f);
        data.Velocity.X = (float) (this.random.NextDouble() * 0.300000011920929 - 0.150000005960464);
        data.Velocity.Y = (float) (this.random.NextDouble() * 0.200000002980232 + 0.100000001490116);
        data.Velocity.Z = (float) (this.random.NextDouble() * 0.300000011920929 - 0.150000005960464);
        this.instance.EmitterParticleSystem.AddParticle(pos, ref data);
      }
    }

    private void UpdateFindLeafToDrop(Block blockID)
    {
      if (this.p.Y <= this.map.MapBound.Min.Y + 1)
        return;
      --this.p.Y;
      if (this.map.GetBlockIDNoCache(this.p) == (byte) 0)
      {
        int key = ((int) (short) (this.p.X - this.op.X) << 16) + (int) (short) (this.p.Z - this.op.Z);
        bool flag;
        if (!this.possibleLeafDropHash.TryGetValue(key, out flag))
        {
          this.possibleLeafDrops.Add(this.p);
          this.possibleLeafDropHash.Add(key, true);
        }
      }
      ++this.p.Y;
    }

    private void UpdateFallingLeaf(Player player)
    {
      for (player.LeavesDroppingCount += (float) (((double) this.possibleLeafDrops.Count + (double) this.instance.Wind.WindVelocity.Length() * 5.0) / 40.0); (int) player.LeavesDroppingCount >= 1; --player.LeavesDroppingCount)
        this.UpdateFallingLeaf(player, this.map.Random.Next(this.possibleLeafDrops.Count));
    }

    private void UpdateFallingLeaf(Player player, int i)
    {
      GlobalPoint3D possibleLeafDrop = this.possibleLeafDrops[i];
      GlobalPoint3D p = possibleLeafDrop;
      --p.Y;
      for (byte blockIdNoCache = this.map.GetBlockIDNoCache(p); p.Y > this.map.MapBound.Min.Y && this.map.BlockData[(int) blockIdNoCache].IsPassable; blockIdNoCache = this.map.GetBlockIDNoCache(p))
        --p.Y;
      float lightNormalized = this.map.GetLightNormalized(possibleLeafDrop);
      ++possibleLeafDrop.Y;
      Vector3 blockCenter = this.map.GetBlockCenter(possibleLeafDrop);
      blockCenter.Y -= this.map.TileSize * 0.5f;
      Block blockIdNoCache1 = (Block) this.map.GetBlockIDNoCache(possibleLeafDrop);
      ParticleData data = new ParticleData();
      Globals2.SetParticleDataFromTemplate(12, ref data);
      data.Rotation = (float) (this.map.Random.NextDouble() * 4.0 - 2.0);
      data.EndColor = data.StartColor = GraphicStatics.TexturePack.GetLeafColor(this.instance, blockIdNoCache1) * lightNormalized;
      data.EndColor.A = data.StartColor.A = byte.MaxValue;
      data.VelocityVariance.Y = 0.0f;
      data.Velocity.Y = -(float) (this.map.Random.NextDouble() * 1.0 + 1.5);
      data.Duration = (ushort) ((double) Math.Min(8f, (float) (((double) blockCenter.Y - ((double) p.Y + (double) this.map.TileSize)) / -(double) data.Velocity.Y)) * 1000.0);
      this.instance.EmitterParticleSystem.AddParticle(blockCenter, ref data);
    }
  }
}
