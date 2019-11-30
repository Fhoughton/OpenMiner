// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalInvaders.Player
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.Arcade.TotalInvaders
{
  internal struct Player
  {
    public bool IsAlive;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Speed;
    public float ShotDelay;
    public float ShotDelayTimer;
    public Vector2 BulletVelocity;

    public Player(bool isAlive)
    {
      this.IsAlive = isAlive;
      this.Position = new Vector2(10f, 220f);
      this.Velocity = Vector2.Zero;
      this.Speed = 0.8f;
      this.ShotDelay = 0.5f;
      this.ShotDelayTimer = 0.0f;
      this.BulletVelocity = new Vector2(0.0f, -7f);
    }
  }
}
