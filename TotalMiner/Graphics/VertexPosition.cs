// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.VertexPosition
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct VertexPosition : IVertexType
  {
    public static readonly VertexElement[] VertexElements = new VertexElement[1]
    {
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
    };
    public static VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexPosition.VertexElements);
    public Vector3 Position;

    public VertexPosition(Vector3 position)
    {
      this.Position = position;
    }

    public VertexDeclaration VertexDeclaration
    {
      get
      {
        return VertexPosition.vertexDeclaration;
      }
    }
  }
}
