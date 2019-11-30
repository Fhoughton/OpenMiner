// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.RainParticleVertex
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct RainParticleVertex
  {
    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[4]
    {
      new VertexElement(0, VertexElementFormat.HalfVector2, VertexElementUsage.Position, 0),
      new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.Position, 1),
      new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
    });
    public const int SizeInBytes = 28;
    public HalfVector2 Position;
    public Vector2 PosYVel;
    public Vector3 UserData;
    public Color Color;
  }
}
