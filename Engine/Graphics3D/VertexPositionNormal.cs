// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.VertexPositionNormal
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public struct VertexPositionNormal : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[2]
    {
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1)
    };
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexPositionNormal.VertexElements);
    public Vector3 Position;
    public Vector3 Normal;

    public VertexPositionNormal(Vector3 position, Vector3 normal)
    {
      this.Position = position;
      this.Normal = normal;
    }

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexPositionNormal.vertexDeclaration;
      }
    }
  }
}
