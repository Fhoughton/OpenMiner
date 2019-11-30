// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ModActorTypeDataXML
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  public struct ModActorTypeDataXML
  {
    public string ActorType;
    public string LevelType;
    public string PhysicsType;
    public string AIType;
    public string ComName;
    public string[] ComNameWalk;
    public float? ModelHeight;
    public float? ModelYRotation;
    public bool? IsValid;
    public bool? IsFemale;
    public bool? IsPassive;
    public bool? IsImmuneToFire;
    public bool? CanBreatheUnderWater;
    public bool? ShowHitBoxes;
    public bool? HasNameplate;
    public int? HandMaxHit;
    public float? NaturalSpawnFreq;
    public string NaturalBehaviour;
    public LootItem[] LootTable;
  }
}
