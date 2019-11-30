// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.VertexInstanceTC
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public struct VertexInstanceTC : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[5]
    {
      new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
      new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
      new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
      new VertexElement(64, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 4)
    };
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration(72, VertexInstanceTC.VertexElements);
    public Matrix World;
    public Vector2 TexCoord;

    public VertexInstanceTC(Matrix world, Vector2 texCoord)
    {
      this.World = world;
      this.TexCoord = texCoord;
    }

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexInstanceTC.vertexDeclaration;
      }
    }
  }
}
