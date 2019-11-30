// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlockParticle
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct BlockParticle
  {
    public float Age;
    public Vector3 Position;
    public Vector3 Velocity;
    public float Radius;
    public Vector3 Rotation;
    public Color Color;
    public BlockParticleModifier Modifier;
  }
}
