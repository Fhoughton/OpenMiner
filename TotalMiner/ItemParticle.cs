// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemParticle
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;

namespace StudioForge.TotalMiner
{
  internal struct ItemParticle
  {
    public ParticleType Type;
    public float Age;
    public InventoryItem Item;
    public Vector3 Position;
    public Vector3 Velocity;
    public float Radius;
    public float Rotation;
    public float MinPickupAge;
    public float OwnLight;
    public ItemParticleModifier Modifier;
    public int ParticleID;
    public long AttachedTo;
    public GamerID PlayerID;
    public byte TextureIndex;
    public bool CameFromRemote;

    public bool HasType(ParticleType types)
    {
      return (this.Type & types) > ParticleType.None;
    }

    public bool IsType(ParticleType types)
    {
      return (this.Type & types) == types;
    }

    public BoundingBox Box
    {
      get
      {
        float num = this.Radius * 0.5f;
        Vector3 position1 = this.Position;
        position1.X -= num;
        position1.Y -= num;
        position1.Z -= num;
        Vector3 position2 = this.Position;
        position2.X += num;
        position2.Y += num;
        position2.Z += num;
        return new BoundingBox(position1, position2);
      }
    }
  }
}
