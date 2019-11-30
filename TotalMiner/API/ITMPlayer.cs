// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPlayer
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.API
{
  public interface ITMPlayer : ITMActor
  {
    /// <summary>Custom data for the player.</summary>
    object Tag { get; set; }

    /// <summary>Controller player index.</summary>
    PlayerIndex PlayerIndex { get; }

    GamerID GamerID { get; }

    NetworkGamer Gamer { get; }

    /// <summary>Spectate or CCTV virtualization.</summary>
    ITMPlayer VirtualPlayer { get; }

    string ClanName { get; }

    bool IsGod { get; }

    bool IsInputEnabled { get; }

    Matrix WorldShake { get; }

    Matrix WorldToolShake { get; }

    /// <summary>
    /// The quarter position of the reticle on the block face.
    /// A block face is divided into 4 quarters and a center quarter.
    /// Values 0 - 3 represent the 4 corner quarters.
    /// If Value is &gt; 3 then the recticle is also in the center quarter. In this case, AND the value with 0x03 to extract the corner quarter.
    /// </summary>
    int SwingFacePos { get; }

    /// <summary>
    /// The block face the reticle is currently intersecting at SwingTarget.
    /// Value = ProxyDefault if reticle is currently not intersecting a block.
    /// </summary>
    BlockFace SwingFace { get; }

    /// <summary>
    /// The voxel location where the reticle intersects a block.
    /// First check if SwingFace != ProxyDefault to determine if the reticle is actually intersecting a block.
    /// </summary>
    GlobalPoint3D SwingTarget { get; }

    /// <summary>
    /// The distance in meters between the actors EyePosition and the reticles intersection of a block.
    /// </summary>
    float SwingTargetDistance { get; }

    /// <summary>
    /// The voxel location where a block would be placed if the player was to place a block.
    /// </summary>
    GlobalPoint3D PlaceTarget { get; }

    History History { get; }

    ITMActor ActorInReticle { get; }

    Dictionary<string, TeleportMark> Teleports { get; }

    ITMPlayer CreateCamera(ITMPlayer player);

    void RemoveCamera(ITMPlayer player, ITMPlayer virtualPlayer);

    void AddTeleport(string name);

    bool RemoveTeleport(string name);

    bool TeleportTo(string name);
  }
}
