// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMHand
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner.API
{
  public interface ITMHand
  {
    /// <summary>
    /// The entity which the hand belongs to. Maybe a Player or NPC.
    /// </summary>
    ITMActor Owner { get; }

    /// <summary>
    /// The player the hand belongs too. If the hand belongs to an NPC, this propery will be null. It is always necessary to check if this property is null before using it as a reference.
    /// </summary>
    ITMPlayer Player { get; }

    /// <summary>The item currently equiped in the hand.</summary>
    Item ItemID { get; }

    /// <summary>
    /// The inventory slot ID of the item currently equipped by the hand.
    /// </summary>
    int HandIndex { get; }

    /// <summary>The hand type.</summary>
    InventoryHand HandType { get; }

    /// <summary>True if the hand is currently swinging.</summary>
    bool IsSwinging { get; }

    /// <summary>Directly set the item equipped by the hand.</summary>
    /// <param name="itemID"></param>
    void SetItem(Item itemID);
  }
}
