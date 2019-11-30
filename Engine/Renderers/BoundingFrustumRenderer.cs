// Decompiled with JetBrains decompiler
// Type: BoundingFrustumRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;

public static class BoundingFrustumRenderer
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
  private static BasicEffect effect;

  public static void Render(
    BoundingFrustum frustum,
    GraphicsDevice graphicsDevice,
    Matrix view,
    Matrix projection,
    Color color)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    if (BoundingFrustumRenderer.effect == null)
    {
      BoundingFrustumRenderer.effect = new BasicEffect(graphicsDevice);
      BoundingFrustumRenderer.effect.VertexColorEnabled = true;
      BoundingFrustumRenderer.effect.LightingEnabled = false;
    }
    Vector3[] corners = frustum.GetCorners();
    for (int index = 0; index < 8; ++index)
    {
      BoundingFrustumRenderer.verts[index].Position = corners[index];
      BoundingFrustumRenderer.verts[index].Color = color;
    }
    BoundingFrustumRenderer.effect.View = view;
    BoundingFrustumRenderer.effect.Projection = projection;
    for (int index = 0; index < BoundingFrustumRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      BoundingFrustumRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, BoundingFrustumRenderer.verts, 0, 8, BoundingFrustumRenderer.indices, 0, BoundingFrustumRenderer.indices.Length / 2);
    }
    ++CoreGlobals.FrameRateCounter.DrawCalls;
  }
}
