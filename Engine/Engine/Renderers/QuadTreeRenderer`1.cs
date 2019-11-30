// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.QuadTreeRenderer`1
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Renderers
{
  public class QuadTreeRenderer<T> : DrawableGameObjectBase where T : ISpatialNode
  {
    private GraphicsDevice graphicsDevice;
    private ICamera camera;
    private Vector2 heightClamp;
    private bool heightIsClamped;

    protected override void LoadContentCore(InitState state)
    {
      this.graphicsDevice = CoreGlobals.GraphicsDevice;
    }

    public void ClampHeight(Vector2 clamp)
    {
      this.heightClamp = clamp;
      this.heightIsClamped = clamp != Vector2.Zero;
    }

    public StudioForge.Engine.Core.QuadTree<T> QuadTree { get; set; }

    protected override void DrawCore(DrawState state)
    {
      if (this.QuadTree == null)
        return;
      this.camera = CoreGlobals.Camera;
      if (this.camera == null)
        return;
      this.DrawQuadTree(this.QuadTree);
    }

    private void DrawQuadTree(StudioForge.Engine.Core.QuadTree<T> quadTree)
    {
      BoundingBox boundingBox = quadTree.BoundingBox;
      if (this.heightIsClamped)
      {
        boundingBox.Min.Y = this.heightClamp.X;
        boundingBox.Max.Y = this.heightClamp.Y;
      }
      BoundingBoxRenderer.Render(boundingBox, this.graphicsDevice, Matrix.Identity, this.camera.ViewMatrix, this.camera.ProjectionMatrix, Color.Yellow);
      if (quadTree.IsLeaf)
        return;
      this.DrawQuadTree(quadTree.TopLeft);
      this.DrawQuadTree(quadTree.TopRight);
      this.DrawQuadTree(quadTree.BottomLeft);
      this.DrawQuadTree(quadTree.BottomRight);
    }
  }
}
