// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.HailParticleVertex
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct HailParticleVertex
  {
    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[3]
    {
      new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
      new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
    });
    public const int SizeInBytes = 28;
    public Vector4 Position;
    public Vector2 Time;
    public Color Color;
  }
}
