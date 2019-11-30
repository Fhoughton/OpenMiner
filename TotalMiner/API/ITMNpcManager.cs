// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMNpcManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.API
{
  public interface ITMNpcManager
  {
    /// <summary>
    /// A readonly list of all NPCs. Do not alter the contents of the list. This list is not thread safe.
    /// Test npc.IsDeadOrInactiveOrDisabled before use if you are only interested in active/alive NPCs.
    /// </summary>
    List<ITMActor> NpcList { get; }

    /// <summary>
    /// Returns False if no more NPCs can be spawned (all slots are allocated).
    /// </summary>
    bool HasFreeNpcSlots { get; }

    /// <summary>
    /// Fills a provided list with all NPCs inside a specified region.
    /// If you are calling this method often, then cache the list to avoid garbage issues.
    /// </summary>
    /// <param name="min">The min bound of the region.</param>
    /// <param name="max">The max bound of the region.</param>
    /// <param name="actorType">The type of NPC. Pass ActorType.None to test all NPCs.</param>
    /// <param name="result">A valid list you must provide to the method. This list will be filled with NPCs that match the type and are found inside the region.</param>
    void GetNpcs(Vector3 min, Vector3 max, ActorType actorType, List<ITMActor> result);

    /// <summary>Spawn an NPC.</summary>
    /// <param name="actorType">The type of NPC.</param>
    /// <param name="pos">The absolute position of the NPC (center, feet).</param>
    /// <param name="ai">The name of the behaviour tree for the NPC behaviour. null = use default.</param>
    /// <param name="dayOrNight"></param>
    /// <param name="killScript">The name of the script to be executed if the NPC is killed. null = no script.</param>
    /// <param name="lootTable">A custom loot table for the NPC. null = use default.</param>
    /// <param name="combatStats">Custom combat stats for the NPC. null = use default.</param>
    /// <returns></returns>
    ITMActor SpawnNpc(
      ActorType actorType,
      Vector3 pos,
      string ai,
      DayOrNight dayOrNight,
      string killScript,
      LootTable lootTable,
      CombatStats? combatStats);

    /// <summary>Deactivate an NPC.</summary>
    /// <param name="npc">The NPC to deactivate.</param>
    void DeactivateNpc(ITMActor npc);
  }
}
