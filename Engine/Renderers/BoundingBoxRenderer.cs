// Decompiled with JetBrains decompiler
// Type: BoundingBoxRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;

public static class BoundingBoxRenderer
{
  private static VertexPositionColor[] verts = new VertexPositionColor[8];
  private static int[] indices = new int[24]
  {
    0,
    1,
    1,
    2,
    2,
    3,
    3,
    0,
    0,
    4,
    1,
    5,
    2,
    6,
    3,
    7,
    4,
    5,
    5,
    6,
    6,
    7,
    7,
    4
  };
  private static DepthStencilState depthState = DepthStencilState.Default;
  private static RasterizerState rasterState = new RasterizerState()
  {
    CullMode = CullMode.None,
    DepthBias = -0.01f,
    FillMode = FillMode.Solid
  };
  private static Vector3[] corners = new Vector3[8];
  private static BasicEffect effect;

  public static void Render(
    AABBox box,
    GraphicsDevice graphicsDevice,
    Matrix world,
    Matrix view,
    Matrix projection,
    Color color)
  {
    BoundingBoxRenderer.Render(new BoundingBox(box.Min, box.Max), graphicsDevice, world, view, projection, color, true);
  }

  public static void Render(
    BoundingBox box,
    GraphicsDevice graphicsDevice,
    Matrix world,
    Matrix view,
    Matrix projection,
    Color color)
  {
    BoundingBoxRenderer.Render(box, graphicsDevice, world, view, projection, color, true);
  }

  public static void Render(
    BoundingBox box,
    GraphicsDevice graphicsDevice,
    Matrix world,
    Matrix view,
    Matrix projection,
    Color color,
    bool addToDrawCallCount)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    if (BoundingBoxRenderer.effect == null)
    {
      BoundingBoxRenderer.effect = new BasicEffect(graphicsDevice);
      BoundingBoxRenderer.effect.VertexColorEnabled = true;
      BoundingBoxRenderer.effect.LightingEnabled = false;
    }
    box.GetCorners(BoundingBoxRenderer.corners);
    for (int index = 0; index < 8; ++index)
    {
      BoundingBoxRenderer.verts[index].Position = BoundingBoxRenderer.corners[index];
      BoundingBoxRenderer.verts[index].Color = color;
    }
    BoundingBoxRenderer.effect.World = world;
    BoundingBoxRenderer.effect.View = view;
    BoundingBoxRenderer.effect.Projection = projection;
    graphicsDevice.DepthStencilState = BoundingBoxRenderer.depthState;
    graphicsDevice.RasterizerState = BoundingBoxRenderer.rasterState;
    for (int index = 0; index < BoundingBoxRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      BoundingBoxRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, BoundingBoxRenderer.verts, 0, 8, BoundingBoxRenderer.indices, 0, BoundingBoxRenderer.indices.Length / 2);
    }
    if (!addToDrawCallCount)
      return;
    ++CoreGlobals.FrameRateCounter.DrawCalls;
  }
}
