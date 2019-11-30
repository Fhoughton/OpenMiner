// Decompiled with JetBrains decompiler
// Type: BoundingSphereRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;

public static class BoundingSphereRenderer
{
  private static VertexBuffer vertBuffer;
  private static BasicEffect effect;
  private static int sphereResolution;

  public static void InitializeGraphics(GraphicsDevice graphicsDevice, int sphereResolution)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    BoundingSphereRenderer.sphereResolution = sphereResolution;
    BoundingSphereRenderer.effect = new BasicEffect(graphicsDevice);
    BoundingSphereRenderer.effect.LightingEnabled = false;
    BoundingSphereRenderer.effect.VertexColorEnabled = false;
    VertexPositionColor[] data = new VertexPositionColor[(sphereResolution + 1) * 3];
    int num1 = 0;
    float num2 = 6.283185f / (float) sphereResolution;
    for (float num3 = 0.0f; (double) num3 <= 6.28318548202515; num3 += num2)
      data[num1++] = new VertexPositionColor(new Vector3((float) Math.Cos((double) num3), (float) Math.Sin((double) num3), 0.0f), Color.White);
    for (float num3 = 0.0f; (double) num3 <= 6.28318548202515; num3 += num2)
      data[num1++] = new VertexPositionColor(new Vector3((float) Math.Cos((double) num3), 0.0f, (float) Math.Sin((double) num3)), Color.White);
    for (float num3 = 0.0f; (double) num3 <= 6.28318548202515; num3 += num2)
      data[num1++] = new VertexPositionColor(new Vector3(0.0f, (float) Math.Cos((double) num3), (float) Math.Sin((double) num3)), Color.White);
    BoundingSphereRenderer.vertBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, data.Length, BufferUsage.None);
    BoundingSphereRenderer.vertBuffer.SetData<VertexPositionColor>(data);
  }

  public static void Render(
    BoundingSphere sphere,
    GraphicsDevice graphicsDevice,
    Matrix view,
    Matrix projection,
    Color xyColor,
    Color xzColor,
    Color yzColor)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    if (BoundingSphereRenderer.vertBuffer == null)
      BoundingSphereRenderer.InitializeGraphics(graphicsDevice, 30);
    graphicsDevice.SetVertexBuffer(BoundingSphereRenderer.vertBuffer);
    BoundingSphereRenderer.effect.View = view;
    BoundingSphereRenderer.effect.Projection = projection;
    BoundingSphereRenderer.effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
    BoundingSphereRenderer.effect.DiffuseColor = xyColor.ToVector3();
    for (int index = 0; index < BoundingSphereRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      BoundingSphereRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, BoundingSphereRenderer.sphereResolution);
      BoundingSphereRenderer.effect.DiffuseColor = xzColor.ToVector3();
      BoundingSphereRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, BoundingSphereRenderer.sphereResolution + 1, BoundingSphereRenderer.sphereResolution);
      BoundingSphereRenderer.effect.DiffuseColor = yzColor.ToVector3();
      BoundingSphereRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, (BoundingSphereRenderer.sphereResolution + 1) * 2, BoundingSphereRenderer.sphereResolution);
    }
    CoreGlobals.FrameRateCounter.DrawCalls += 3;
  }

  public static void Render(
    BoundingSphere sphere,
    GraphicsDevice graphicsDevice,
    Matrix view,
    Matrix projection,
    Color color)
  {
    if (BoundingSphereRenderer.vertBuffer == null)
      BoundingSphereRenderer.InitializeGraphics(graphicsDevice, 30);
    graphicsDevice.SetVertexBuffer(BoundingSphereRenderer.vertBuffer);
    BoundingSphereRenderer.effect.View = view;
    BoundingSphereRenderer.effect.Projection = projection;
    BoundingSphereRenderer.effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
    BoundingSphereRenderer.effect.DiffuseColor = color.ToVector3();
    for (int index = 0; index < BoundingSphereRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      BoundingSphereRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, BoundingSphereRenderer.sphereResolution);
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, BoundingSphereRenderer.sphereResolution + 1, BoundingSphereRenderer.sphereResolution);
      graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, (BoundingSphereRenderer.sphereResolution + 1) * 2, BoundingSphereRenderer.sphereResolution);
    }
    CoreGlobals.FrameRateCounter.DrawCalls += 3;
  }
}
