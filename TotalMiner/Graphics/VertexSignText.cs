// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.VertexSignText
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct VertexSignText : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[2]
    {
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(12, VertexElementFormat.NormalizedShort2, VertexElementUsage.TextureCoordinate, 0)
    };
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexSignText.VertexElements);
    public Vector3 Position;
    public NormalizedShort2 TexCoord;

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexSignText.vertexDeclaration;
      }
    }
  }
}
