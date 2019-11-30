// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMWorld
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Graphics;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.API
{
  public interface ITMWorld
  {
    /// <summary>Returns True if this is a Creative Mode world.</summary>
    bool IsCreativeMode { get; }

    /// <summary>Returns True if this is a Survival Mode world.</summary>
    bool IsSurvivalMode { get; }

    /// <summary>Returns True if this is a Peaceful Mode world.</summary>
    bool IsPeacefulMode { get; }

    /// <summary>Returns True if this is a Dig Deep Mode world.</summary>
    bool IsDigDeepMode { get; }

    /// <summary>Returns True if Finite Resources is enabled.</summary>
    bool IsFiniteResources { get; }

    /// <summary>Returns True if Local or Global Skills are enabled.</summary>
    bool IsSkillsEnabled { get; }

    /// <summary>Returns True if Local Skills are enabled.</summary>
    bool IsLocalSkillsEnabled { get; }

    /// <summary>Returns True if this is a Local Skills world.</summary>
    bool IsLocalSkills { get; }

    /// <summary>Returns True if Peaceful Difficulty is enabled.</summary>
    bool IsPeacefulDifficulty { get; }

    /// <summary>Returns True if Easy Difficulty is enabled.</summary>
    bool IsEasyDifficulty { get; }

    /// <summary>Returns True if Normal Difficulty is enabled.</summary>
    bool IsNormalDifficulty { get; }

    /// <summary>Returns True if Legendary Difficulty is enabled.</summary>
    bool IsLegendaryDifficulty { get; }

    /// <summary>Returns a reference to the Map object.</summary>
    ITMMap Map { get; }

    /// <summary>
    /// Returns header data about the current world. This property returns a clone so that if you change a field on the object it does not effect the actual world header. For this reason it is best to cache this object and only call this getter again if it is critical to have any changes that might have been made since the last time you called the getter (such as a map rename).
    /// </summary>
    SaveMapHead Header { get; }

    /// <summary>Returns the full file path for the current world.</summary>
    string WorldPath { get; }

    /// <summary>Returns the worlds current Biome.</summary>
    BiomeType CurrentBiome { get; }

    ITMEntityManager EntityManager { get; }

    ITMEnvManager EnvironManager { get; }

    ITMNpcManager NpcManager { get; }

    List<MapMarker> MapMarkers { get; }

    List<MapMarker> GraveMarkers { get; }

    /// <summary>Returns a List of all Zones in the world.</summary>
    List<Zone> Zones { get; }

    /// <summary>
    /// Returns a bounding box that correctly surrounds the block at a given position, and is suitable for collision detection.
    /// </summary>
    /// <param name="p">The position of the block.</param>
    /// <param name="blockID">The blockID at position p.</param>
    /// <returns>The bounding box.</returns>
    BoundingBox GetBlockBox(GlobalPoint3D p, Block blockID);

    /// <summary>Spawn a new particle into the game world.</summary>
    /// <param name="pos">World start position of the particle (center).</param>
    /// <param name="data">Data describing the particle.</param>
    /// <returns>Returns true if the particle was spawned successfully.</returns>
    bool AddParticle(Vector3 pos, ref ParticleData data);

    /// <summary>Adds a projectile textured for the itemID.</summary>
    /// <param name="itemID">The item of the projectile.</param>
    /// <param name="position">The origin position of the projectile.</param>
    /// <param name="velocity">The start velocity of the projectile. The projectile is affected by the world gravity.</param>
    /// <param name="player">The player who created the projectile.</param>
    /// <param name="transmit">True if the projectile should be created on remote clients.</param>
    void AddProjectile(
      Item itemID,
      Vector3 position,
      Vector3 velocity,
      ITMPlayer player,
      bool transmit);

    /// <summary>Add a Marker to the world overview map.</summary>
    /// <param name="p">The map position for the marker. Y is ignored.</param>
    /// <param name="text">The marker text or label.</param>
    /// <param name="type">The type of marker.</param>
    /// <param name="transmit">True if remotes should also add the marker.</param>
    void AddMapMarker(GlobalPoint3D p, string text, MapMarkerType type, bool transmit);

    /// <summary>Casts a ray test for block collision detection.</summary>
    /// <param name="position">The origin position of the ray.</param>
    /// <param name="dir">The direction of the ray.</param>
    /// <param name="range">The range (distance in meters) of the ray. For performance considerations it is best to limit the range to the shortest necessary.</param>
    /// <returns>The result of the test.</returns>
    HitTest RayBlockTest(Vector3 position, Vector3 dir, float range);

    /// <summary>
    /// Returns true if a block position is currently delivering power.
    /// </summary>
    /// <param name="p">Block position</param>
    /// <returns>Returns true if a block position is currently delivering power.</returns>
    bool IsBlockDeliveringPower(GlobalPoint3D p);

    /// <summary>
    /// Returns true if a block position is currently receiving power.
    /// </summary>
    /// <param name="p">Block position</param>
    /// <returns>Returns true if a block position is currently receiving power.</returns>
    bool IsBlockReceivingPower(GlobalPoint3D p);

    /// <summary>
    /// Set power on or off for a block at a position in the map. Any powered mechanism block at that position will react. Setting power on for a block that is already powered has no effect, likewise setting power off for a block that is not powered has no effect.
    /// </summary>
    /// <param name="p">Map position.</param>
    /// <param name="power">True (on) or False (off).</param>
    /// <param name="player">The player who powered the block or -1 if no player.</param>
    void SetPower(GlobalPoint3D p, bool power, ITMPlayer player);

    /// <summary>
    /// Returns the AudioListener object of the closest local player to a (sound source) position.
    /// </summary>
    /// <param name="position">The source position of the sound.</param>
    /// <returns>The AudioListener object of the closest local player, or null.</returns>
    AudioListener GetClosestListener(Vector3 position);

    /// <summary>
    /// Instruct the game to broadcast a sound emission to other actors.
    /// </summary>
    /// <param name="origin">The origin position of the sound emission.</param>
    /// <param name="actor">The actor who emitted the sound.</param>
    /// <param name="soundType">The type of sound emitted.</param>
    void BroadcastSound(Vector3 origin, ITMActor actor, SoundType soundType);

    /// <summary>
    /// Returns true if any local player is within a proximity
    /// </summary>
    /// <param name="pos">World position of proximity center.</param>
    /// <param name="range">Proximity radius.</param>
    /// <param name="eye">True = Test players Eye position. False = Test players Foot position.</param>
    /// <returns>True if at least one local player is within proximity.</returns>
    bool IsAnyLocalPlayerInProximity(Vector3 pos, float range, bool eye);

    /// <summary>Create a blast explosion</summary>
    /// <param name="p">World Tile position at blast center.</param>
    /// <param name="itemID">Item that created the blast.</param>
    /// <param name="strength">Blast strength.</param>
    /// <param name="radius">Blast radius.</param>
    /// <param name="player">Player that created the blast or -1 if no player.</param>
    void CreateBlast(GlobalPoint3D p, Item itemID, float strength, int radius, ITMPlayer player);

    /// <summary>
    /// Causes a block to start falling until it hits another block.
    /// </summary>
    /// <param name="p">World Tile position of source block.</param>
    /// <param name="player">Player who caused the block to fall or -1 if no player.</param>
    /// <param name="method">Update method.</param>
    /// <param name="transmit">True = Transmit over the network to remote players. False = Local operation only.</param>
    /// <returns></returns>
    bool CreateFallingBlock(
      GlobalPoint3D p,
      ITMPlayer player,
      UpdateBlockMethod method,
      bool transmit);

    /// <summary>Creates a flood.</summary>
    /// <param name="p">Flood source position.</param>
    /// <param name="blockID">Block to flood with.</param>
    /// <param name="player">Player who created the flood or -1 if no player.</param>
    /// <param name="transmit">True = Transmit flood over network to remote players. False = Local operation only.</param>
    void FloodPhysics(GlobalPoint3D p, Block blockID, ITMPlayer player, bool transmit);

    /// <summary>Teleport all entities in an area to another position.</summary>
    /// <param name="min">The minimum Tile position of the area.</param>
    /// <param name="max">The maximum Tile position of the area. Using both min and max, a cubic area is defined.</param>
    /// <param name="dest">The destination Tile position to teleport the entities to.</param>
    /// <param name="relative">False = all entities are teleported directly to tile position 'dest' regardless of their position in the area. True = all entities are teleported to a tile position relative to 'dest' and their original position relative to 'min'. e.g. final position = dest + (entity position - min)</param>
    void TeleportEntities(GlobalPoint3D min, GlobalPoint3D max, GlobalPoint3D dest, bool relative);
  }
}
