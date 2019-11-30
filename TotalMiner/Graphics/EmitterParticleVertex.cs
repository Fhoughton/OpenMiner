// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.EmitterParticleVertex
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct EmitterParticleVertex
  {
    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[8]
    {
      new VertexElement(0, VertexElementFormat.Short4, VertexElementUsage.Position, 0),
      new VertexElement(8, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
      new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
      new VertexElement(32, VertexElementFormat.HalfVector4, VertexElementUsage.Normal, 1),
      new VertexElement(40, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
      new VertexElement(56, VertexElementFormat.Color, VertexElementUsage.Color, 0),
      new VertexElement(60, VertexElementFormat.Color, VertexElementUsage.Color, 1)
    });
    public const int SizeInBytes = 64;
    public Short4 Corner;
    public Vector3 Position;
    public Vector3 Velocity;
    public HalfVector4 Size;
    public Vector2 Rotation;
    public Vector2 Time;
    public Color Color1;
    public Color Color2;
  }
}
