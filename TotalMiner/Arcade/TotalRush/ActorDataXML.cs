// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.ActorDataXML
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  public class ActorDataXML
  {
    public ActorType ActorType;
    public float Scale;
    public int Score;
    public int Hitpoints;
    public int SmartBombDamage;
    public int MaxPosY;
    public bool IsVulnerable;
    public float DestructionTime;
    public float ExplosionScale;
    public Color DustColor;
    public bool IsBoss;
    public bool IsBigShipKill;
    public Vector2 Speed;
    public float RotationSpeed;
    public float RotationOffset;
    public Rectangle SrcRect;
    public Vector2 Origin;
    public Vector2 HitBoxOffset;
    public float HitBoxRadius;
    public int ExhaustSpread;
    public int ExhaustFrequency;
    public int ExhaustOffsetY;
    public BulletPattern[] BulletPatterns;
    public ActorType[] ChildTypes;
    public Vector2[] ChildOffsets;
  }
}
