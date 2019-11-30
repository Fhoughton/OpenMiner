// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.SkyCurtain
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
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class SkyCurtain : IHasInitialization, IHasContent, IUnmanagedBuffer
  {
    public VertexBuffer VertexBuffer;
    public IndexBuffer IndexBuffer;
    public float CenterY;
    private GameInstance instance;
    private Map map;
    private List<VertexPositionTexture> vertices;
    private List<short> indices;

    protected short CurrentVertex
    {
      get
      {
        return (short) this.vertices.Count;
      }
    }

    public SkyCurtain(GameInstance instance)
    {
      this.instance = instance;
      this.map = (Map) instance.Map;
    }

    public virtual void Initialize(InitState state)
    {
    }

    public virtual void LoadContent(InitState state)
    {
      Vector4 vector4 = new Vector4((float) this.map.MapBound.Min.X, (float) this.map.MapBound.Min.Z, (float) this.map.MapBound.Max.X, (float) this.map.MapBound.Max.Z);
      GraphicStatics.SkyCurtainShader.MapBound.SetValue(vector4 * this.map.TileSize);
      GraphicStatics.SkyCurtainShader.FloorY.SetValue((int) this.map.SeaLevel);
      Effect effect = GraphicStatics.SkyCurtainShader.Effect;
      effect.CurrentTechnique = effect.Techniques["SkyCurtainShader"];
      this.LoadGeometry();
    }

    public void LoadGeometry()
    {
      this.LoadGeometry(this.instance.MaxFarClip + 100f, 30);
    }

    private void LoadGeometry(float radius, int tessellation)
    {
      this.vertices = new List<VertexPositionTexture>();
      this.indices = new List<short>();
      int index = Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld ? 4 : 0;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[0, index]];
      Vector2 textureCoordinate1 = MapChunkContent.TexCoords2[MapChunkContent.TexOffsets[0, index]];
      Vector2 vector2_2 = MapChunkContent.TexCoords3[MapChunkContent.TexOffsets[0, index]];
      Vector2 textureCoordinate2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[0, index]];
      Vector3 vector3_1 = new Vector3(0.0f, (float) (this.map.MapBound.Max.Y - (int) this.map.SeaLevel) * 0.5f * this.map.TileSize, 0.0f);
      Vector3 vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
      this.CenterY = vector3_1.Y * 0.5f;
      for (int i = 0; i < tessellation; ++i)
      {
        Vector3 circleVector1 = SkyCurtain.GetCircleVector(i, tessellation);
        Vector3 circleVector2 = SkyCurtain.GetCircleVector(i + 1 == tessellation ? 0 : i + 1, tessellation);
        this.vertices.Add(new VertexPositionTexture(circleVector1 * radius + vector3_2, vector2_2));
        this.vertices.Add(new VertexPositionTexture(circleVector1 * radius + vector3_1, vector2_1));
        this.vertices.Add(new VertexPositionTexture(circleVector2 * radius + vector3_1, textureCoordinate1));
        this.vertices.Add(new VertexPositionTexture(circleVector2 * radius + vector3_2, textureCoordinate2));
        this.indices.Add((short) (i * 4));
        this.indices.Add((short) (i * 4 + 1));
        this.indices.Add((short) (i * 4 + 2));
        this.indices.Add((short) (i * 4));
        this.indices.Add((short) (i * 4 + 2));
        this.indices.Add((short) (i * 4 + 3));
      }
      this.CreateCap(tessellation, vector3_2.Y, radius, Vector3.Up, vector2_2);
      vector3_2.Y = vector3_1.Y;
      vector3_1.Y = (float) (this.map.MapBound.Max.Y + 10 - (int) this.map.SeaLevel) * this.map.TileSize;
      short currentVertex = this.CurrentVertex;
      for (int i = 0; i < tessellation; ++i)
      {
        Vector3 circleVector1 = SkyCurtain.GetCircleVector(i, tessellation);
        Vector3 circleVector2 = SkyCurtain.GetCircleVector(i + 1 == tessellation ? 0 : i + 1, tessellation);
        this.vertices.Add(new VertexPositionTexture(circleVector1 * radius + vector3_2, vector2_1));
        this.vertices.Add(new VertexPositionTexture(circleVector1 * radius + vector3_1, vector2_1));
        this.vertices.Add(new VertexPositionTexture(circleVector2 * radius + vector3_1, textureCoordinate1));
        this.vertices.Add(new VertexPositionTexture(circleVector2 * radius + vector3_2, textureCoordinate1));
        this.indices.Add((short) ((int) currentVertex + i * 4));
        this.indices.Add((short) ((int) currentVertex + i * 4 + 1));
        this.indices.Add((short) ((int) currentVertex + i * 4 + 2));
        this.indices.Add((short) ((int) currentVertex + i * 4));
        this.indices.Add((short) ((int) currentVertex + i * 4 + 2));
        this.indices.Add((short) ((int) currentVertex + i * 4 + 3));
      }
      this.CreateCap(tessellation, vector3_1.Y, radius, Vector3.Down, vector2_1);
      this.VertexBuffer = new VertexBuffer(CoreGlobals.GraphicsDevice, VertexPositionTexture.VertexDeclaration, this.vertices.Count, BufferUsage.WriteOnly);
      this.VertexBuffer.SetData<VertexPositionTexture>(this.vertices.ToArray());
      this.IndexBuffer = new IndexBuffer(CoreGlobals.GraphicsDevice, IndexElementSize.SixteenBits, this.indices.Count, BufferUsage.WriteOnly);
      this.IndexBuffer.SetData<short>(this.indices.ToArray());
      this.vertices = (List<VertexPositionTexture>) null;
      this.indices = (List<short>) null;
    }

    private void CreateCap(int tessellation, float y, float radius, Vector3 normal, Vector2 tc)
    {
      for (int index = 0; index < tessellation - 2; ++index)
      {
        if ((double) normal.Y > 0.0)
        {
          this.indices.Add(this.CurrentVertex);
          this.indices.Add((short) ((int) this.CurrentVertex + (index + 1) % tessellation));
          this.indices.Add((short) ((int) this.CurrentVertex + (index + 2) % tessellation));
        }
        else
        {
          this.indices.Add(this.CurrentVertex);
          this.indices.Add((short) ((int) this.CurrentVertex + (index + 2) % tessellation));
          this.indices.Add((short) ((int) this.CurrentVertex + (index + 1) % tessellation));
        }
      }
      for (int i = 0; i < tessellation; ++i)
      {
        Vector3 position = SkyCurtain.GetCircleVector(i, tessellation) * radius + normal;
        position.Y = y;
        this.vertices.Add(new VertexPositionTexture(position, tc));
      }
    }

    private static Vector3 GetCircleVector(int i, int tessellation)
    {
      float num = (float) i * 6.283185f / (float) tessellation;
      return new Vector3((float) Math.Cos((double) num), 0.0f, (float) Math.Sin((double) num));
    }

    public virtual void UnloadContent()
    {
    }

    public long BufferSize
    {
      get
      {
        long num = 0;
        if (this.VertexBuffer != null)
          num += this.VertexBuffer.BufferSize();
        if (this.IndexBuffer != null)
          num += this.IndexBuffer.BufferSize();
        return num;
      }
    }
  }
}
