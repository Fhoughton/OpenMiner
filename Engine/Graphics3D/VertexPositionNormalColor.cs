// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.VertexPositionNormalColor
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public struct VertexPositionNormalColor : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
      new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
    };
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration(28, VertexPositionNormalColor.VertexElements);
    public Vector3 Position;
    public Vector3 Normal;
    public Color Color;

    public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
    {
      this.Position = position;
      this.Normal = normal;
      this.Color = color;
    }

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexPositionNormalColor.vertexDeclaration;
      }
    }
  }
}
