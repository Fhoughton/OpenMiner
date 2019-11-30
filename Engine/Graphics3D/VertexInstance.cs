// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.VertexInstance
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Graphics3D
{
  public struct VertexInstance : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[4]
    {
      new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
      new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
      new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
      new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
    };
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration(64, VertexInstance.VertexElements);
    public Matrix World;

    public VertexInstance(Matrix world)
    {
      this.World = world;
    }

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexInstance.vertexDeclaration;
      }
    }
  }
}
