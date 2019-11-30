// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CloudMap
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class CloudMap : MapTM
  {
    public float DriftSpeed;
    public Vector3 RestartPosition;
    private bool fadeIn;
    private float alpha;
    private float alphaChangeRate;
    private PcgRandom rand;

    public float Alpha
    {
      get
      {
        return this.alpha;
      }
    }

    public CloudMap(
      GameInstance instance,
      int tileSize,
      BoxInt mapBound,
      BlockDataXML[] blockData,
      int seed)
      : base(instance, nameof (CloudMap), (float) tileSize, false, mapBound, mapBound, new Point3D(mapBound.Max.X - mapBound.Min.X, mapBound.Max.Y - mapBound.Min.Y, mapBound.Max.Z - mapBound.Min.Z), new Point3D(mapBound.Max.X - mapBound.Min.X, mapBound.Max.Y - mapBound.Min.Y, mapBound.Max.Z - mapBound.Min.Z), blockData, 15, seed, (ushort) 1, 3, (MapStrategy) new DummyMapStrategy(), true, false)
    {
      this.DriftSpeed = 0.0f;
      this.rand = new PcgRandom(seed);
      this.FadeIn();
    }

    public void FadeIn()
    {
      this.alpha = 0.0f;
      this.fadeIn = true;
    }

    public void FadeOut()
    {
      this.fadeIn = false;
    }

    public void SetAlphaChangeRate(float value)
    {
      this.alphaChangeRate = value;
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (this.fadeIn && (double) this.alpha < 1.0)
        this.alpha = Math.Min(1f, this.alpha + this.alphaChangeRate);
      else if (!this.fadeIn && (double) this.alpha > 0.0)
        this.alpha = Math.Max(0.0f, this.alpha - this.alphaChangeRate);
      this.Position.X += this.DriftSpeed;
      CloudMap cloudMap = this;
      cloudMap.Position = cloudMap.Position + this.Instance.Wind.WindVelocity / 120f;
      if ((double) this.Position.X <= (double) this.Instance.Map.MapBound.Max.X && (double) this.Position.Z <= (double) this.Instance.Map.MapBound.Max.Z && ((double) this.Position.X + (double) this.MapBound.Max.X * (double) this.TileSize >= (double) this.Instance.Map.MapBound.Min.X && (double) this.Position.Z + (double) this.MapBound.Max.Z * (double) this.TileSize >= (double) this.Instance.Map.MapBound.Min.Z))
        return;
      this.Position = this.RestartPosition;
      this.FadeIn();
    }

    public void AddClouds(Color[] data, byte threshold)
    {
      if (data.Length != this.MapSize.X * this.MapSize.Z)
        return;
      GlobalPoint3D p = new GlobalPoint3D();
      byte num1 = 10;
      int num2 = 0;
      MapBlock empty = MapBlock.Empty;
      MapBlock newBlockData = this.BuildBlockData(this.Regions[0].Chunks[0], (byte) 0, new MapLight()
      {
        BlockLight = (byte) 0,
        SunLight = (byte) 15
      }, (byte) 0);
      for (p.Z = 0; p.Z < this.MapSize.Z; ++p.Z)
      {
        for (p.X = 0; p.X < this.MapSize.X; ++p.X)
        {
          newBlockData.BlockID = (int) data[num2++].R > (int) threshold ? num1 : (byte) 0;
          this.SetBlockDataInternal(p, empty, newBlockData, UpdateBlockMethod.Generation);
        }
      }
    }
  }
}
