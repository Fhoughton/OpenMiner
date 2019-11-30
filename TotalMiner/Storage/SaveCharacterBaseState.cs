// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveCharacterBaseState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveCharacterBaseState
  {
    public ActorType MobType;
    public Vector3 Position;
    public float Health;
    public int Seed;
    public SaveInventoryState Inventory;
  }
}
