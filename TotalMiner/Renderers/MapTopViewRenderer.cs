// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.MapTopViewRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Renderers
{
  internal class MapTopViewRenderer : DrawableGameObjectBase
  {
    private float[] scale = new float[4]
    {
      53.333f,
      40f,
      32f,
      20f
    };
    public float Scale;
    private MapTM map;
    private Color color;
    private Rectangle rect;
    private Rectangle destRect;
    private int mapSizeX;
    private int mapSizeZ;
    private GlobalPoint3D mapBoundMin;
    private GlobalPoint3D mapBoundMax;
    private SpriteBatchSafe spriteBatch;
    private int lowestY;
    private int highestY;
    private bool drawnOnce;
    private Player player;
    private GameInstance instance;

    public MapTopViewRenderer(GameInstance instance, MapTM map)
    {
      this.instance = instance;
      this.map = map;
      this.Scale = 100f;
      this.lowestY = int.MaxValue;
      this.highestY = int.MinValue;
      this.drawnOnce = false;
      this.mapBoundMin = map.MapBound.Min;
      this.mapBoundMax = map.MapBound.Max;
    }

    protected override void LoadContentCore(InitState state)
    {
      this.spriteBatch = new SpriteBatchSafe(CoreGlobals.GraphicsDevice);
    }

    protected override void UnloadContentCore()
    {
      if (this.spriteBatch == null)
        return;
      this.spriteBatch.Dispose();
    }

    protected override void DrawCore(DrawState state)
    {
    }

    public void Draw(Player player, Rectangle rect, bool calcHeightRangeOnly)
    {
      this.player = player;
      this.rect = rect;
      this.mapSizeX = this.map.MapSize.X;
      this.mapSizeZ = this.map.MapSize.Z;
      this.Scale = 256f;
      this.destRect.Width = (int) (((double) rect.Width + ((double) this.Scale - 1.0)) / (double) this.Scale);
      this.destRect.Height = (int) (((double) rect.Height + ((double) this.Scale - 1.0)) / (double) this.Scale);
      this.color = Color.White;
      this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, (RasterizerState) null, (Effect) null);
      for (int x = 0; x < rect.Width; x += this.destRect.Width)
      {
        for (int z = 0; z < rect.Height; z += this.destRect.Height)
          this.DrawBlock(x, z, calcHeightRangeOnly);
      }
      this.spriteBatch.End();
      this.drawnOnce = true;
    }

    private void DrawBlock(int x, int z, bool calcHeightRangeOnly)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.mapBoundMax.X - (int) ((double) x / (double) this.rect.Width * (double) this.mapSizeX);
      p.Z = this.mapBoundMax.Z - (int) ((double) z / (double) this.rect.Height * (double) this.mapSizeZ);
      if (p.X < this.mapBoundMin.X || p.X >= this.mapBoundMax.X || (p.Z < this.mapBoundMin.Z || p.Z >= this.mapBoundMax.Z))
        return;
      p.Y = (int) this.map.GetHeight(p);
      if (p.Y < this.lowestY)
        this.lowestY = p.Y;
      if (p.Y > this.highestY)
        this.highestY = p.Y;
      if (calcHeightRangeOnly)
        return;
      byte blockIdNoCache = this.map.GetBlockIDNoCache(p);
      if (blockIdNoCache <= (byte) 0)
        return;
      this.destRect.X = x;
      this.destRect.Y = z;
      if (this.map.BlockData[(int) blockIdNoCache].Buffer > (byte) 1)
        this.DrawFirstSolid(p);
      this.DrawBlockCore(p.Y, blockIdNoCache, this.map.GetAuxDataNoCache(p));
    }

    private void DrawBlockCore(int y, byte blockID, byte aux)
    {
      if (this.drawnOnce)
      {
        float num1 = (float) (this.highestY - this.lowestY);
        if ((double) num1 == 0.0)
          num1 = 1f;
        byte num2 = (byte) ((double) byte.MaxValue * ((double) (y - this.lowestY) / (double) num1 * 0.400000005960464 + 0.600000023841858));
        this.color.R = num2;
        this.color.G = num2;
        this.color.B = num2;
      }
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) aux >> 4);
      this.spriteBatch.Draw(GraphicStatics.TexturePack.BlockTexture, this.destRect, new Rectangle?(GraphicStatics.TexturePack.BlockSrcRects[(int) textureIdForDrawing]), this.color);
    }

    private void DrawFirstSolid(GlobalPoint3D p)
    {
      while (--p.Y > this.map.MapBound.Min.Y)
      {
        byte blockIdNoCache = this.map.GetBlockIDNoCache(p);
        if (blockIdNoCache != (byte) 0 && this.map.BlockData[(int) blockIdNoCache].Buffer < (byte) 2)
        {
          this.DrawBlockCore(p.Y, blockIdNoCache, this.map.GetAuxDataNoCache(p));
          break;
        }
      }
    }
  }
}
