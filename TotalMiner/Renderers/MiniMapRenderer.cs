// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.MiniMapRenderer
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
using System;

namespace StudioForge.TotalMiner.Renderers
{
  internal class MiniMapRenderer : DrawableGameObjectBase
  {
    private string[] compassString = new string[4]
    {
      "N",
      "E",
      "S",
      "W"
    };
    public Block[,] BlockIDs;
    public float[,] Light;
    public RenderTarget2D RenderTarget;
    public Rectangle WinRect;
    private MapTM map;
    private Player player;
    private SpriteBatchSafe spriteBatch;
    private GameInstance instance;
    private Texture2D stickmanTexture;
    private Texture2D stickmanCrouchTexture;
    private bool playerIsAdmin;
    private MiniMapWorker worker;
    private GlobalPoint3D prevPoint;
    private bool prevXsection;
    private bool prevReverse;
    private bool isRenderTargetDirty;
    private EventHandler<EventArgs> onMapDataChanged;

    public MiniMapRenderer(GameInstance instance, MapTM map, Player player)
    {
      this.instance = instance;
      this.map = map;
      this.player = player;
      this.BlockIDs = new Block[23, 17];
      this.Light = new float[23, 17];
      this.worker = new MiniMapWorker(instance, map, this, player);
      this.onMapDataChanged = new EventHandler<EventArgs>(this.OnMapDataChanged);
      this.isRenderTargetDirty = true;
    }

    protected override void LoadContentCore(InitState state)
    {
      this.spriteBatch = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.stickmanTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Stickman");
      this.stickmanCrouchTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\StickmanCrouch");
      this.instance.MapStrategyTM.MapDataChanged += this.onMapDataChanged;
      this.WinRect = new Rectangle(116, 70, 234, 174);
    }

    protected override void UnloadContentCore()
    {
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      if (mapStrategyTm != null)
        mapStrategyTm.MapDataChanged -= this.onMapDataChanged;
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch);
      base.UnloadContentCore();
    }

    private void OnMapDataChanged(object sender, EventArgs e)
    {
      this.OnMapDataChanged();
    }

    public void OnMapDataChanged()
    {
      this.prevPoint = new GlobalPoint3D(-1, -1, -1);
    }

    public void OnBlocksUpdated()
    {
      this.isRenderTargetDirty = true;
    }

    public void RenderTargetIsDirty()
    {
      this.isRenderTargetDirty = true;
    }

    protected override void DrawCore(DrawState state)
    {
    }

    public void Draw(Player virtualPlayer)
    {
      GlobalPoint3D point = this.map.GetPoint(virtualPlayer.Position);
      bool xSection = (double) Math.Abs(virtualPlayer.ViewDirection.X) > (double) Math.Abs(virtualPlayer.ViewDirection.Z);
      bool reverse = xSection ? (double) virtualPlayer.ViewDirection.X < 0.0 : (double) virtualPlayer.ViewDirection.Z < 0.0;
      if (point != this.prevPoint || this.prevReverse != reverse || this.prevXsection != xSection)
      {
        this.worker.SetData(virtualPlayer);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.worker, true, PriorityLevel.Priority);
        this.prevPoint = point;
        this.prevReverse = reverse;
        this.prevXsection = xSection;
      }
      if (!this.isRenderTargetDirty)
        return;
      if (this.RenderTarget == null)
        this.RenderTarget = new RenderTarget2D(CoreGlobals.GraphicsDevice, this.WinRect.Width, this.WinRect.Height);
      CoreGlobals.GraphicsDevice.SetRenderTarget(this.RenderTarget);
      this.RenderToTarget(virtualPlayer, xSection, reverse);
      CoreGlobals.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      this.isRenderTargetDirty = false;
    }

    private void RenderToTarget(Player virtualPlayer, bool xSection, bool reverse)
    {
      Rectangle rect = new Rectangle(0, 0, this.WinRect.Width, this.WinRect.Height);
      Matrix identity = Matrix.Identity;
      this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, identity);
      this.spriteBatch.DrawFilledBox(rect, 2, Color.White * 0.9f, GraphicStatics.TexturePack.SkyColor * 0.7f);
      GlobalPoint3D point = this.map.GetPoint(virtualPlayer.Position);
      GlobalPoint3D globalPoint3D = point;
      --globalPoint3D.Y;
      Color color = Color.White * 0.5f;
      color.A = (byte) 179;
      int y = 0;
      this.playerIsAdmin = this.player.IsAdmin;
      Rectangle destRect = new Rectangle(2, 2, 10, 10);
      int num = virtualPlayer.SwingTargetIsValid ? 1 : 0;
      GlobalPoint3D swingTarget = virtualPlayer.SwingTarget;
      point.Y = globalPoint3D.Y + 10;
      while (point.Y > globalPoint3D.Y - 7)
      {
        int x = 0;
        if (xSection)
        {
          if (reverse)
          {
            point.X = globalPoint3D.X + 11;
            while (point.X > globalPoint3D.X - 12)
            {
              this.DrawBlock(virtualPlayer, ref point, ref destRect, color, x, y, point == swingTarget);
              destRect.X += destRect.Width;
              --point.X;
              ++x;
            }
          }
          else
          {
            point.X = globalPoint3D.X - 11;
            while (point.X < globalPoint3D.X + 12)
            {
              this.DrawBlock(virtualPlayer, ref point, ref destRect, color, x, y, point == swingTarget);
              destRect.X += destRect.Width;
              ++point.X;
              ++x;
            }
          }
        }
        else if (reverse)
        {
          point.Z = globalPoint3D.Z + 11;
          while (point.Z > globalPoint3D.Z - 12)
          {
            this.DrawBlock(virtualPlayer, ref point, ref destRect, color, x, y, point == swingTarget);
            destRect.X += destRect.Width;
            --point.Z;
            ++x;
          }
        }
        else
        {
          point.Z = globalPoint3D.Z - 11;
          while (point.Z < globalPoint3D.Z + 12)
          {
            this.DrawBlock(virtualPlayer, ref point, ref destRect, color, x, y, point == swingTarget);
            destRect.X += destRect.Width;
            ++point.Z;
            ++x;
          }
        }
        destRect.X = rect.X + 2;
        destRect.Y += destRect.Height;
        --point.Y;
        ++y;
      }
      destRect = new Rectangle(rect.X + (rect.Width - 10) / 2, rect.Y + (rect.Height - 6) / 2, 10, 20);
      this.spriteBatch.Draw(virtualPlayer.IsCrouching ? this.stickmanCrouchTexture : this.stickmanTexture, destRect, color);
      int index = !xSection ? (reverse ? 2 : 0) : (reverse ? 1 : 3);
      this.spriteBatch.DrawString(CoreGlobals.GameFont, this.compassString[index], new Vector2((float) (rect.X + 8), (float) (rect.Y + 9)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
    }

    private void DrawBlock(
      Player virtualPlayer,
      ref GlobalPoint3D p,
      ref Rectangle destRect,
      Color color,
      int x,
      int y,
      bool drawSwingTarget)
    {
      if (!this.map.IsValidPoint(p))
        return;
      Block blockId = this.BlockIDs[x, y];
      if (blockId == Block.None || this.map.BlockData[(int) blockId].Buffer > (byte) 1)
      {
        Vector3 vector3 = GraphicStatics.TexturePack.SkyColor.ToVector3();
        float num = this.Light[x, y];
        vector3.X *= num;
        vector3.Y *= num;
        vector3.Z *= num;
        Color color1 = new Color(vector3) * 0.5f;
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, destRect, color1);
      }
      if (blockId > Block.None)
        this.spriteBatch.Draw(GraphicStatics.TexturePack.BlockTexture, destRect, new Rectangle?(GraphicStatics.TexturePack.BlockSrcRects[(int) blockId]), color);
      if (!drawSwingTarget)
        return;
      this.spriteBatch.DrawBox(destRect, 1, color, 0.0f);
    }
  }
}
