// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CloudMapBuilder
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class CloudMapBuilder : IThreadWorkItem
  {
    private Map map;
    private GameInstance instance;
    private ContentManager content;
    private Texture2D texture;
    private int lastGrayscale;
    private int grayscale;
    private byte threshold;
    private CloudMap cloudMap;
    private Color[] colorData;

    public string Name
    {
      get
      {
        return nameof (CloudMapBuilder);
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

    public CloudMapBuilder(GameInstance instance, Map map)
    {
      this.map = map;
      this.instance = instance;
      this.content = new ContentManager(Services.GetService<IServiceProvider>(), "Content");
      this.grayscale = -1;
    }

    public void InitBuild(int grayscale, byte threshold, CloudMap cloudMap)
    {
      this.lastGrayscale = this.grayscale;
      this.grayscale = grayscale;
      this.threshold = threshold;
      this.cloudMap = cloudMap;
    }

    public void UnloadContent()
    {
      if (this.content == null)
        return;
      this.content.Unload();
      this.content = (ContentManager) null;
    }

    public void Update()
    {
      if (this.grayscale != this.lastGrayscale)
      {
        if (this.texture != null)
          this.content.Unload();
        this.texture = this.content.Load<Texture2D>("Textures\\cloudmap" + (object) (this.grayscale + 1));
      }
      this.BuildCloudMap();
    }

    private void BuildCloudMap()
    {
      int tileSize = 8;
      BoxInt mapBound = new BoxInt()
      {
        Max = this.map.MapSize * 2 / tileSize
      };
      mapBound.Max.Y = 1;
      if (this.cloudMap == null)
      {
        this.cloudMap = new CloudMap(this.instance, tileSize, mapBound, Globals1.BlockData, this.map.Seed);
        this.cloudMap.PregenerateRegions(true, false, (IProgressBar) null);
      }
      if (this.colorData == null)
        this.colorData = new Color[this.texture.Width * this.texture.Height];
      this.texture.GetData<Color>(this.colorData);
      this.cloudMap.AddClouds(this.colorData, this.threshold);
      MapChunkTM chunk = this.cloudMap.Regions[0].Chunks[0] as MapChunkTM;
      chunk.Content.ReloadChunk(this.instance, chunk, false, false, false, true, (IProgressBar) null);
      Vector3 vector3 = this.cloudMap.MapSize * this.cloudMap.TileSize;
      float y = this.instance.CloudHeight + this.map.TileSize * 0.5f;
      this.cloudMap.RestartPosition = new Vector3((float) (-(double) vector3.X * 0.25), y, (float) (-(double) vector3.Z * 0.25));
      this.cloudMap.Position = this.cloudMap.RestartPosition;
      this.instance.CloudMapManager.OnCloudMapBuilt(this.cloudMap);
    }
  }
}
