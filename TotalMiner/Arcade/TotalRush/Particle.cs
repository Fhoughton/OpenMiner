// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.Particle
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal struct Particle
  {
    public ParticleType ParticleType;
    public float Scale;
    public float Age;
    public float OrigAge;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;
    public Color Color;
  }
}
