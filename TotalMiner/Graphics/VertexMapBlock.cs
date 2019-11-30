// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.VertexMapBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct VertexMapBlock : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement(0, VertexElementFormat.HalfVector4, VertexElementUsage.Position, 0),
      new VertexElement(8, VertexElementFormat.NormalizedShort2, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(12, VertexElementFormat.NormalizedShort2, VertexElementUsage.Color, 0)
    };
    public static VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexMapBlock.VertexElements);
    public HalfVector4 Position;
    public NormalizedShort2 TexCoord;
    public NormalizedShort2 Light;

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexMapBlock.vertexDeclaration;
      }
    }
  }
}
