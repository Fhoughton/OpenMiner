// Decompiled with JetBrains decompiler
// Type: CubeRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;

public static class CubeRenderer
{
  public static VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[24];
  public static int[] indices = new int[36]
  {
    0,
    1,
    2,
    0,
    2,
    3,
    4,
    5,
    6,
    4,
    6,
    7,
    8,
    9,
    10,
    8,
    10,
    11,
    12,
    13,
    14,
    12,
    14,
    15,
    16,
    17,
    18,
    16,
    18,
    19,
    20,
    21,
    22,
    20,
    22,
    23
  };
  private static Vector3[] Normals = new Vector3[6]
  {
    Vector3.Backward,
    Vector3.Left,
    Vector3.Forward,
    Vector3.Right,
    Vector3.Up,
    Vector3.Down
  };
  private static DepthStencilState depthState = DepthStencilState.Default;
  private static RasterizerState rasterState = RasterizerState.CullNone;
  public static int FacePicked;
  private static BasicEffect effect;

  public static void Render(
    BoundingBox box,
    GraphicsDevice graphicsDevice,
    Matrix world,
    Matrix view,
    Matrix projection,
    Color color,
    Texture2D texture)
  {
    CubeRenderer.Render(box, graphicsDevice, world, view, projection, color, texture, new Ray?(), (CubeRenderer.GetFaceTexCoords) null, true);
  }

  public static void Render(
    BoundingBox box,
    GraphicsDevice graphicsDevice,
    Matrix world,
    Matrix view,
    Matrix projection,
    Color color,
    Texture2D texture,
    Ray? nray,
    CubeRenderer.GetFaceTexCoords getTexCoords,
    bool addToDrawCallCount)
  {
    if (graphicsDevice == null)
      graphicsDevice = CoreGlobals.GraphicsDevice;
    if (CubeRenderer.effect == null)
    {
      CubeRenderer.effect = new BasicEffect(graphicsDevice);
      CubeRenderer.effect.VertexColorEnabled = false;
      CubeRenderer.effect.EnableDefaultLighting();
      CubeRenderer.effect.TextureEnabled = true;
    }
    bool flag = getTexCoords != null;
    Vector3 min = box.Min;
    Vector3 max = box.Max;
    Vector4 vector4_1 = new Vector4(0.0f, 0.0f, 1f, 1f);
    Vector4 vector4_2 = flag ? getTexCoords(0) : vector4_1;
    CubeRenderer.verts[0] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, max.Z), CubeRenderer.Normals[0], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[1] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, max.Z), CubeRenderer.Normals[0], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[2] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, max.Z), CubeRenderer.Normals[0], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[3] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, max.Z), CubeRenderer.Normals[0], new Vector2(vector4_2.Z, vector4_2.W));
    vector4_2 = flag ? getTexCoords(1) : vector4_1;
    CubeRenderer.verts[4] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, min.Z), CubeRenderer.Normals[1], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[5] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, min.Z), CubeRenderer.Normals[1], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[6] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, max.Z), CubeRenderer.Normals[1], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[7] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, max.Z), CubeRenderer.Normals[1], new Vector2(vector4_2.Z, vector4_2.W));
    vector4_2 = flag ? getTexCoords(2) : vector4_1;
    CubeRenderer.verts[8] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, min.Z), CubeRenderer.Normals[2], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[9] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, min.Z), CubeRenderer.Normals[2], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[10] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, min.Z), CubeRenderer.Normals[2], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[11] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, min.Z), CubeRenderer.Normals[2], new Vector2(vector4_2.Z, vector4_2.W));
    vector4_2 = flag ? getTexCoords(3) : vector4_1;
    CubeRenderer.verts[12] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, max.Z), CubeRenderer.Normals[3], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[13] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, max.Z), CubeRenderer.Normals[3], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[14] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, min.Z), CubeRenderer.Normals[3], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[15] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, min.Z), CubeRenderer.Normals[3], new Vector2(vector4_2.Z, vector4_2.W));
    vector4_2 = flag ? getTexCoords(4) : vector4_1;
    CubeRenderer.verts[16] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, max.Z), CubeRenderer.Normals[4], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[17] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, min.Z), CubeRenderer.Normals[4], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[18] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, min.Z), CubeRenderer.Normals[4], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[19] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, max.Z), CubeRenderer.Normals[4], new Vector2(vector4_2.Z, vector4_2.W));
    vector4_2 = flag ? getTexCoords(5) : vector4_1;
    CubeRenderer.verts[20] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, min.Z), CubeRenderer.Normals[5], new Vector2(vector4_2.X, vector4_2.W));
    CubeRenderer.verts[21] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, min.Z), CubeRenderer.Normals[5], new Vector2(vector4_2.X, vector4_2.Y));
    CubeRenderer.verts[22] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, max.Z), CubeRenderer.Normals[5], new Vector2(vector4_2.Z, vector4_2.Y));
    CubeRenderer.verts[23] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, max.Z), CubeRenderer.Normals[5], new Vector2(vector4_2.Z, vector4_2.W));
    CubeRenderer.effect.World = world;
    CubeRenderer.effect.View = view;
    CubeRenderer.effect.Projection = projection;
    CubeRenderer.effect.Texture = texture;
    graphicsDevice.DepthStencilState = CubeRenderer.depthState;
    graphicsDevice.RasterizerState = CubeRenderer.rasterState;
    for (int index = 0; index < CubeRenderer.effect.CurrentTechnique.Passes.Count; ++index)
    {
      CubeRenderer.effect.CurrentTechnique.Passes[index].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, CubeRenderer.verts, 0, 24, CubeRenderer.indices, 0, CubeRenderer.indices.Length / 3);
    }
    if (addToDrawCallCount)
      ++CoreGlobals.FrameRateCounter.DrawCalls;
    CubeRenderer.FacePicked = -1;
    if (!nray.HasValue)
      return;
    Ray ray = nray.Value;
    if (!ray.Intersects(box).HasValue)
      return;
    float maxValue = float.MaxValue;
    float? nullable1 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[0]].Position, ref CubeRenderer.verts[CubeRenderer.indices[1]].Position, ref CubeRenderer.verts[CubeRenderer.indices[2]].Position);
    if (nullable1.HasValue)
    {
      float? nullable2 = nullable1;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable1.Value;
        CubeRenderer.FacePicked = 0;
      }
    }
    float? nullable3 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[3]].Position, ref CubeRenderer.verts[CubeRenderer.indices[4]].Position, ref CubeRenderer.verts[CubeRenderer.indices[5]].Position);
    if (nullable3.HasValue)
    {
      float? nullable2 = nullable3;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable3.Value;
        CubeRenderer.FacePicked = 0;
      }
    }
    float? nullable4 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[6]].Position, ref CubeRenderer.verts[CubeRenderer.indices[7]].Position, ref CubeRenderer.verts[CubeRenderer.indices[8]].Position);
    if (nullable4.HasValue)
    {
      float? nullable2 = nullable4;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable4.Value;
        CubeRenderer.FacePicked = 1;
      }
    }
    float? nullable5 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[9]].Position, ref CubeRenderer.verts[CubeRenderer.indices[10]].Position, ref CubeRenderer.verts[CubeRenderer.indices[11]].Position);
    if (nullable5.HasValue)
    {
      float? nullable2 = nullable5;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable5.Value;
        CubeRenderer.FacePicked = 1;
      }
    }
    float? nullable6 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[12]].Position, ref CubeRenderer.verts[CubeRenderer.indices[13]].Position, ref CubeRenderer.verts[CubeRenderer.indices[14]].Position);
    if (nullable6.HasValue)
    {
      float? nullable2 = nullable6;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable6.Value;
        CubeRenderer.FacePicked = 2;
      }
    }
    float? nullable7 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[15]].Position, ref CubeRenderer.verts[CubeRenderer.indices[16]].Position, ref CubeRenderer.verts[CubeRenderer.indices[17]].Position);
    if (nullable7.HasValue)
    {
      float? nullable2 = nullable7;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable7.Value;
        CubeRenderer.FacePicked = 2;
      }
    }
    float? nullable8 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[18]].Position, ref CubeRenderer.verts[CubeRenderer.indices[19]].Position, ref CubeRenderer.verts[CubeRenderer.indices[20]].Position);
    if (nullable8.HasValue)
    {
      float? nullable2 = nullable8;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable8.Value;
        CubeRenderer.FacePicked = 3;
      }
    }
    float? nullable9 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[21]].Position, ref CubeRenderer.verts[CubeRenderer.indices[22]].Position, ref CubeRenderer.verts[CubeRenderer.indices[23]].Position);
    if (nullable9.HasValue)
    {
      float? nullable2 = nullable9;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable9.Value;
        CubeRenderer.FacePicked = 3;
      }
    }
    float? nullable10 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[24]].Position, ref CubeRenderer.verts[CubeRenderer.indices[25]].Position, ref CubeRenderer.verts[CubeRenderer.indices[26]].Position);
    if (nullable10.HasValue)
    {
      float? nullable2 = nullable10;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable10.Value;
        CubeRenderer.FacePicked = 4;
      }
    }
    float? nullable11 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[27]].Position, ref CubeRenderer.verts[CubeRenderer.indices[28]].Position, ref CubeRenderer.verts[CubeRenderer.indices[29]].Position);
    if (nullable11.HasValue)
    {
      float? nullable2 = nullable11;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable11.Value;
        CubeRenderer.FacePicked = 4;
      }
    }
    float? nullable12 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[30]].Position, ref CubeRenderer.verts[CubeRenderer.indices[31]].Position, ref CubeRenderer.verts[CubeRenderer.indices[32]].Position);
    if (nullable12.HasValue)
    {
      float? nullable2 = nullable12;
      float num = maxValue;
      if (((double) nullable2.GetValueOrDefault() >= (double) num ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
      {
        maxValue = nullable12.Value;
        CubeRenderer.FacePicked = 5;
      }
    }
    float? nullable13 = Intersection.RayIntersectsTriangle(ref ray, ref CubeRenderer.verts[CubeRenderer.indices[33]].Position, ref CubeRenderer.verts[CubeRenderer.indices[34]].Position, ref CubeRenderer.verts[CubeRenderer.indices[35]].Position);
    if (!nullable13.HasValue)
      return;
    float? nullable14 = nullable13;
    float num1 = maxValue;
    if (((double) nullable14.GetValueOrDefault() >= (double) num1 ? 0 : (nullable14.HasValue ? 1 : 0)) == 0)
      return;
    float num2 = nullable13.Value;
    CubeRenderer.FacePicked = 5;
  }

  public delegate Vector4 GetFaceTexCoords(int face);
}
