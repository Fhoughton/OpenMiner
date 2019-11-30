// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.INPCBehaviour
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public interface INPCBehaviour : ITMActor
  {
    GamerID GamerID { get; }

    INPCBehaviour AITarget { get; }

    ActorAIDataXML AIData { get; }

    float Age { get; }

    float StrikeRange { get; }

    bool IsAlive { get; }

    GlobalPoint3D SpawnPoint { get; }

    ITMMap Map { get; }

    BehaviourTree DialogTree { get; }

    BehaviourTree BehaviourTree { get; }

    BehaviourTreeNode LastNode { get; set; }

    GlobalPoint3D SwingTarget { get; }

    BlockFace SwingFace { get; }

    bool SwingTargetIsValid { get; }

    DialogNode CurrentDialog { get; }

    INPCBehaviour CurrentDialogTarget { get; }

    NpcProperties Properties { get; }

    PcgRandom Random { get; }

    float MoveSpeed { get; }

    bool IsInZone(ZoneType type);

    bool IsInZone(string zoneName);

    void SwingHand(InventoryHand hand);

    void SwingHand(InventoryHand hand, List<ActorType> excludeTypes);

    void MoveTo(Vector3 pos, float velMod, bool canJump, MoveType moveType);

    bool LookAt(CoordType lookAtType, Vector3 pos, bool instant);

    void StandStill();

    Vector3 GetRandomPositionNearPoint(Vector3 pos, float distance);

    INPCBehaviour FindActor(
      NpcQueryPreference preference,
      float distance,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes);

    SoundBroadcast? FindSound(
      NpcQueryPreference preference,
      float distance,
      List<SoundType> soundTypes,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes);

    void SetProperties(NpcProperties properties);

    void Jump(float height);

    bool EquipItem(InventoryHand hand, Item item);

    bool EquipItem(InventoryHand hand, ItemType itemType);

    bool EquipItem(InventoryHand hand, ItemSubType itemSubType);

    Vector3 GetFinalPosition(CoordType type, Vector3 pos);

    void LoadBehaviour(BehaviourTreeType type, string tree);
  }
}
