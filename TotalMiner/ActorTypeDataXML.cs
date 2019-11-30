// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ActorTypeDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  public class ActorTypeDataXML
  {
    public ActorType ActorType;
    public string IDString;
    public ActorLevelType LevelType;
    public ActorPhysicsType PhysicsType;
    public ActorAIType AIType;
    public string ComName;
    public string[] ComNameWalk;
    public int ComModID;
    public float ModelHeight;
    public float ModelYRotation;
    public bool IsValid;
    public bool IsFemale;
    public bool IsPassive;
    public bool IsImmuneToFire;
    public bool CanBreatheUnderWater;
    public bool ShowHitBoxes;
    public bool HasNameplate;
    public int HandMaxHit;
    public string NPCText;
    public float NaturalSpawnFreq;
    public string NaturalBehaviour;
    public float ExplodeBlocksRatio;
    public Vector2 ExplodeBlocksScale;
    public Vector3 EyeOffset;
    public Vector3 StrikePointOffset;
    public Vector3 ItemModelOffset;
    public float BoxOffset;
    public float BoxOffsetCrouch;
    public float CriticalBoxOffset;
    public float CriticalBoxOffsetCrouch;
    public Vector2 BoxScale;
    public Vector2 BoxScaleCrouch;
    public Vector2 CriticalBoxScale;
    public Vector2 CriticalBoxScaleCrouch;
    public LootItem[] LootTable;
  }
}
