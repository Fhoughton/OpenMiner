// Decompiled with JetBrains decompiler
// Type: RayRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Integration;

public static class RayRenderer
{
  private static VertexPositionColor[] verts = new VertexPositionColor[2];
  private static VertexPositionColor[] arrowVerts = new VertexPositionColor[5]
  {
    new VertexPositionColor(Vector3.Zero, Color.White),
    new VertexPositionColor(new Vector3(0.5f, 0.0f, -0.5f), Color.White),
    new VertexPositionColor(new Vector3(-0.5f, 0.0f, -0.5f), Color.White),
    new VertexPositionColor(new Vector3(0.0f, 0.5f, -0.5f), Color.White),
    new VertexPositionColor(new Vector3(0.0f, -0.5f, -0.5f), Color.White)
  };
  private static int[] arrowIndexs = new int[8]
  {
    0,
    1,
    0,
    2,
    0,
    3,
    0,
    4
  };
  private static BasicEffect effect;

  public static void Render(Ray ray, float length, Color color)
  {
    ICamera camera = CoreGlobals.Camera;
    RayRenderer.Render(ray, length, (GraphicsDevice) null, camera.ViewMatrix, camera.ProjectionMatrix, color);
  }

  public static void Render(
    Ray ray,
    float length,
    GraphicsDevice graphicsDevice,
    Matrix view,
    Matrix projection,
    Color color)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    if (RayRenderer.effect == null)
    {
      RayRenderer.effect = new BasicEffect(graphicsDevice);
      RayRenderer.effect.VertexColorEnabled = false;
      RayRenderer.effect.LightingEnabled = false;
    }
    RayRenderer.verts[0] = new VertexPositionColor(ray.Position, Color.White);
    RayRenderer.verts[1] = new VertexPositionColor(ray.Position + ray.Direction * length, Color.White);
    RayRenderer.effect.DiffuseColor = color.ToVector3();
    RayRenderer.effect.Alpha = (float) color.A / (float) byte.MaxValue;
    RayRenderer.effect.World = Matrix.Identity;
    RayRenderer.effect.View = view;
    RayRenderer.effect.Projection = projection;
    for (int index = 0; index < RayRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      RayRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, RayRenderer.verts, 0, 1);
      RayRenderer.effect.World = Matrix.Invert(Matrix.CreateLookAt(RayRenderer.verts[1].Position, RayRenderer.verts[0].Position, ray.Direction != Vector3.Up ? Vector3.Up : Vector3.Left));
      RayRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, RayRenderer.arrowVerts, 0, 5, RayRenderer.arrowIndexs, 0, 4);
    }
  }
}
