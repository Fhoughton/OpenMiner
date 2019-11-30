// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FlatBiome
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System;

namespace StudioForge.TotalMiner
{
  internal class FlatBiome : BiomeBase
  {
    public static StudioForge.Engine.Core.Pool<FlatBiome> Pool = new StudioForge.Engine.Core.Pool<FlatBiome>();
    private Item groundID;
    private byte groundBlockID;

    public override void Initialize(GameInstance instance, MapTM map, BiomeParams biomeParams)
    {
      base.Initialize(instance, map, biomeParams);
      this.Initialize(biomeParams, map.SeaLevel);
      this.groundID = Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock;
      this.groundBlockID = this.groundID == Item.SkyWorld || this.groundID == Item.SpaceWorld ? (byte) 0 : (byte) this.groundID;
    }

    protected override void GenerateChunkCore()
    {
      if (this.chunkGlobalOffset.Y > (int) this.map.SeaLevel)
        return;
      this.allAirBlocks = false;
      this.allSolidBlocks = this.chunkGlobalOffset.Y + this.chunkSizeY < (int) this.map.SeaLevel;
      if (this.groundID != Item.NaturalWorld)
        this.FillFlatChunk();
      else
        this.FillFlatNaturalChunk();
    }

    private void FillFlatChunk()
    {
      if (this.chunkGlobalOffset.Y == 0)
      {
        this.FillChunkOnBedrock(this.chunkGlobalOffset);
      }
      else
      {
        if (this.groundBlockID == (byte) 0)
          return;
        MapBlock data = new MapBlock();
        if (this.allSolidBlocks)
        {
          data.BlockID = this.groundBlockID;
          this.chunk.Fill(data, false);
        }
        else
        {
          data.BlockID = this.groundBlockID;
          GlobalPoint3D to = this.chunkGlobalOffset + this.map.ChunkSize;
          to.Y = (int) this.map.SeaLevel;
          this.chunk.Fill(this.chunkGlobalOffset, to, data, false);
        }
      }
    }

    private void FillFlatNaturalChunk()
    {
      GlobalPoint3D chunkGlobalOffset = this.chunkGlobalOffset;
      chunkGlobalOffset.Y += this.map.ChunkSize.Y;
      --chunkGlobalOffset.Y;
      GlobalPoint3D to = this.chunkGlobalOffset + this.map.ChunkSize;
      to.Y = chunkGlobalOffset.Y;
      if (to.Y > (int) this.map.SeaLevel)
        to.Y = (int) this.map.SeaLevel;
      int y = this.chunkGlobalOffset.Y;
      MapBlock data = new MapBlock();
label_20:
      while (to.Y >= y)
      {
        if (to.Y == (int) this.map.SeaLevel)
        {
          data.BlockID = (byte) 1;
          chunkGlobalOffset.Y = to.Y;
          this.chunk.Fill(chunkGlobalOffset, to, data, false);
          --to.Y;
        }
        else if (to.Y > (int) this.map.SeaLevel - 3)
        {
          data.BlockID = (byte) 2;
          chunkGlobalOffset.Y = to.Y - 1;
          if (chunkGlobalOffset.Y < y)
            chunkGlobalOffset.Y = y;
          this.chunk.Fill(chunkGlobalOffset, to, data, false);
          to.Y = chunkGlobalOffset.Y - 1;
        }
        else if (to.Y == 0)
        {
          data.BlockID = (byte) 29;
          chunkGlobalOffset.Y = to.Y;
          this.chunk.Fill(chunkGlobalOffset, to, data, false);
          --to.Y;
        }
        else
        {
          int num1 = 14;
          float num2 = (float) ((int) this.map.SeaLevel - 3);
          float num3 = (float) ((int) this.map.SeaLevel - 4) / (float) num1;
          data.BlockID = (byte) 15;
          chunkGlobalOffset.Y = to.Y;
          while (true)
          {
            if ((double) num2 >= (double) y && (double) num2 > 0.0)
            {
              if (to.Y <= (int) num2 && to.Y > (int) ((double) num2 - (double) num3))
              {
                chunkGlobalOffset.Y = (int) ((double) num2 - (double) num3) + 1;
                if (chunkGlobalOffset.Y < y)
                  chunkGlobalOffset.Y = y;
                if (y == 0 && chunkGlobalOffset.Y > 1 && data.BlockID == (byte) 28)
                  chunkGlobalOffset.Y = 1;
                this.chunk.Fill(chunkGlobalOffset, to, data, false);
                to.Y = chunkGlobalOffset.Y - 1;
              }
              num2 -= num3;
              ++data.BlockID;
            }
            else
              goto label_20;
          }
        }
      }
    }

    private void FillChunkOnBedrock(GlobalPoint3D p1)
    {
      MapBlock data = new MapBlock();
      data.BlockID = (byte) 29;
      GlobalPoint3D to = p1 + this.map.ChunkSize;
      to.Y = p1.Y;
      this.chunk.Fill(p1, to, data, false);
      if (this.groundBlockID == (byte) 0)
        return;
      data.BlockID = this.groundBlockID;
      to.Y = Math.Min((int) this.map.SeaLevel, p1.Y + this.chunkSizeY);
      ++p1.Y;
      this.chunk.Fill(p1, to, data, false);
    }

    protected override int GetPlaneData(int x, int z)
    {
      return (int) Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel;
    }

    public override int GetGroundHeightGlobal(Map map, int x, int z)
    {
      return (int) Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel;
    }

    protected override void GetBlock(Point3D p, int globalY)
    {
    }

    protected override bool MustDecorate
    {
      get
      {
        return false;
      }
    }
  }
}
