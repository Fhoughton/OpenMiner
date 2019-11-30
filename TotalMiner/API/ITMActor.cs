// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMActor
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.API
{
  public interface ITMActor
  {
    ActorType ActorType { get; }

    ActorState ActorState { get; }

    string Name { get; }

    /// <summary>
    /// The position of the actor in world space. The actors origin is bottom center of the actors feet.
    /// </summary>
    Vector3 Position { get; set; }

    /// <summary>
    /// The offset of the actors eyes from origin (bottom center of feet).
    /// </summary>
    Vector3 EyeOffset { get; set; }

    /// <summary>
    /// The position of the actors eyes in world space = Position + EyeOffset
    /// </summary>
    Vector3 EyePosition { get; }

    /// <summary>The actors current velocity in meters per second.</summary>
    Vector3 Velocity { get; set; }

    /// <summary>Normalized view direction.</summary>
    Vector3 ViewDirection { get; set; }

    /// <summary>View Matrix in world space.</summary>
    Matrix ViewMatrix { get; }

    /// <summary>
    /// View Matrix in local space (used for local space rendering).
    /// </summary>
    Matrix ViewMatrixLocal { get; }

    Matrix ProjectionMatrix { get; set; }

    /// <summary>The actors view frustum in world space.</summary>
    BoundingFrustum Frustum { get; }

    /// <summary>The actors main bounding box in world space.</summary>
    BoundingBox Box { get; }

    ITMHand LeftHand { get; }

    ITMHand RightHand { get; }

    ITMInventory Inventory { get; }

    AudioEmitter AudioEmitter { get; }

    /// <summary>Current oxygen.</summary>
    float Oxygen { get; set; }

    /// <summary>Current health.</summary>
    float Health { get; set; }

    /// <summary>The maximum health this actor can have.</summary>
    float MaxHealth { get; }

    FlyMode FlyMode { get; set; }

    /// <summary>The actors current reach in blocks (meters).</summary>
    int Reach { get; set; }

    /// <summary>Flag indicating if this actor is a player (gamer).</summary>
    bool IsPlayer { get; }

    /// <summary>
    /// True if the actor is currently rested on a solid block.
    /// </summary>
    bool IsOnGround { get; }

    bool IsDeadOrInactiveOrDisabled { get; }

    /// <summary>Add an item to the actors inventory.</summary>
    /// <returns>The quantity added.</returns>
    int AddToInventory(InventoryItem item);

    /// <summary>
    /// Add an item to the actors inventory, returning the slot id where the item was added.
    /// </summary>
    /// <returns>The quantity added.</returns>
    int AddToInventory(InventoryItem item, out int slotID);

    /// <summary>
    /// Equip an item from the actors inventory into the items default hand.
    /// </summary>
    /// <returns>True if the item was successfully equipped.</returns>
    bool EquipFromInventory(Item itemID);

    /// <summary>
    /// Equip an item from the actors inventory into a specified hand.
    /// </summary>
    /// <param name="hand">The hand to equip the item into. If null, result = EquipFromInventory(Item itemID)</param>
    /// <param name="itemID"></param>
    /// <returns>True if the item was successfully equipped.</returns>
    bool EquipFromInventory(ITMHand hand, Item itemID);

    bool UnequipToInventory(EquipIndex equipIndex);

    bool IsItemEquipped(Item itemID);

    bool IsItemEquippedAndUsable(Item itemID);

    int GetItemEquippedSlot(Item itemID);

    /// <summary>
    /// Drop an item from the players hand or inventory to the ground, as a pickup.
    /// </summary>
    void DropItem(int slotID);

    /// <summary>Change the players current state.</summary>
    /// <param name="newState"></param>
    /// <returns></returns>
    bool ChangeState(ActorState newState);

    /// <summary>
    /// Cause damage to the player. Damage particles are spawned. Player dies if resulting health is less than or equal to 0.
    /// </summary>
    /// <returns>The actual damage taken.</returns>
    float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce);

    /// <summary>
    /// Cause damage to the player. Damage particles are spawned. Player dies if resulting health is less than or equal to 0.
    /// </summary>
    /// <returns>The actual damage taken.</returns>
    float TakeDamageAndDisplay(
      DamageType damageType,
      float damage,
      Vector3 knockForce,
      ITMActor attacker,
      Item weaponID,
      SkillType attackType);

    /// <summary>
    /// Returns true if the actor has all of the specified (or'd) permissions.
    /// </summary>
    bool HasPermission(Permissions permissions);

    /// <summary>
    /// Returns true if the actor has at least one of the specified (or'd) permissions.
    /// </summary>
    bool HasPermissionAny(Permissions permissions);

    bool HasHistory(string key);

    /// <summary>
    /// Casts a ray from EyePosition in a specified direction for a specified distance. Returns true if no solid block is intersected by the ray.
    /// </summary>
    /// <param name="dir">The ray direction.</param>
    /// <param name="distance">The distance to test.</param>
    bool LineOfSightTest(Vector3 dir, float distance);

    /// <summary>Teleport the actor to a world space map position.</summary>
    /// <param name="pos"></param>
    void TeleportTo(Vector3 pos);

    void UpdateMatrices();
  }
}
