// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.MiniMapWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Renderers
{
  internal class MiniMapWorker : IThreadWorkItem
  {
    private MapTM map;
    private GameInstance instance;
    private MiniMapRenderer renderer;
    private Player player;
    private Player virtualPlayer;
    private bool playerIsAdmin;

    public string Name
    {
      get
      {
        return "MiniMapBuilder";
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

    public MiniMapWorker(
      GameInstance instance,
      MapTM map,
      MiniMapRenderer renderer,
      Player player)
    {
      this.instance = instance;
      this.map = map;
      this.renderer = renderer;
      this.player = player;
    }

    public void SetData(Player virtualPlayer)
    {
      this.virtualPlayer = virtualPlayer;
    }

    public void Update()
    {
      this.playerIsAdmin = this.player.IsAdmin;
      GlobalPoint3D point = this.map.GetPoint(this.virtualPlayer.Position);
      GlobalPoint3D globalPoint3D = point;
      --globalPoint3D.Y;
      bool flag1 = (double) Math.Abs(this.virtualPlayer.ViewDirection.X) > (double) Math.Abs(this.virtualPlayer.ViewDirection.Z);
      bool flag2 = flag1 ? (double) this.virtualPlayer.ViewDirection.X < 0.0 : (double) this.virtualPlayer.ViewDirection.Z < 0.0;
      int y = 0;
      point.Y = globalPoint3D.Y + 10;
      while (point.Y > globalPoint3D.Y - 7)
      {
        int x = 0;
        if (flag1)
        {
          if (flag2)
          {
            point.X = globalPoint3D.X + 11;
            while (point.X > globalPoint3D.X - 12)
            {
              this.DrawBlock(this.virtualPlayer, ref point, x, y);
              --point.X;
              ++x;
            }
          }
          else
          {
            point.X = globalPoint3D.X - 11;
            while (point.X < globalPoint3D.X + 12)
            {
              this.DrawBlock(this.virtualPlayer, ref point, x, y);
              ++point.X;
              ++x;
            }
          }
        }
        else if (flag2)
        {
          point.Z = globalPoint3D.Z + 11;
          while (point.Z > globalPoint3D.Z - 12)
          {
            this.DrawBlock(this.virtualPlayer, ref point, x, y);
            --point.Z;
            ++x;
          }
        }
        else
        {
          point.Z = globalPoint3D.Z - 11;
          while (point.Z < globalPoint3D.Z + 12)
          {
            this.DrawBlock(this.virtualPlayer, ref point, x, y);
            ++point.Z;
            ++x;
          }
        }
        --point.Y;
        ++y;
      }
      this.renderer.OnBlocksUpdated();
    }

    private void DrawBlock(Player virtualPlayer, ref GlobalPoint3D p, int x, int y)
    {
      if (!this.map.IsValidPoint(p))
        return;
      byte blockID = this.map.GetBlockID(p);
      if (!this.playerIsAdmin && blockID == (byte) 114)
        blockID = this.GetDifferentNeighbourBlockID(p, blockID);
      else if (!this.player.IsGodOrTester && blockID == (byte) 137 && !this.map.HasChanged(p))
        blockID = this.GetDifferentNeighbourBlockID(p, blockID);
      bool flag = blockID == (byte) 0 || this.map.BlockData[(int) blockID].Buffer > (byte) 1;
      this.renderer.BlockIDs[x, y] = (Block) blockID;
      if (!flag)
        return;
      this.renderer.Light[x, y] = this.map.GetLightNormalized(p);
    }

    private byte GetDifferentNeighbourBlockID(GlobalPoint3D p, byte blockID)
    {
      GlobalPoint3D min = this.map.MapBound.Min;
      GlobalPoint3D max = this.map.MapBound.Max;
      if (p.X > min.X)
      {
        --p.X;
        byte blockId1 = this.map.GetBlockID(p);
        if (this.map.BlockData[(int) blockId1].Buffer < (byte) 2)
          return blockId1;
        ++p.X;
        ++p.X;
        if (p.X < max.X)
        {
          byte blockId2 = this.map.GetBlockID(p);
          if (this.map.BlockData[(int) blockId2].Buffer < (byte) 2)
            return blockId2;
        }
        --p.X;
      }
      if (p.Z > min.Z)
      {
        --p.Z;
        byte blockId1 = this.map.GetBlockID(p);
        if (this.map.BlockData[(int) blockId1].Buffer < (byte) 2)
          return blockId1;
        ++p.Z;
        ++p.Z;
        if (p.Z < max.Z)
        {
          byte blockId2 = this.map.GetBlockID(p);
          if (this.map.BlockData[(int) blockId2].Buffer < (byte) 2)
            return blockId2;
        }
        --p.Z;
      }
      if (p.Y > min.Y)
      {
        --p.Y;
        byte blockId1 = this.map.GetBlockID(p);
        if (this.map.BlockData[(int) blockId1].Buffer < (byte) 2)
          return blockId1;
        ++p.Y;
        ++p.Y;
        if (p.Y < max.Y)
        {
          byte blockId2 = this.map.GetBlockID(p);
          if (this.map.BlockData[(int) blockId2].Buffer < (byte) 2)
            return blockId2;
        }
        --p.Y;
      }
      return blockID;
    }
  }
}
