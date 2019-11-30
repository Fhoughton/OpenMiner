// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMInventory
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.API
{
  public interface ITMInventory
  {
    /// <summary>The number of slots in the pack inventory.</summary>
    short PackSize { get; }

    /// <summary>The number of slots in the equip (body) inventory.</summary>
    short EquipSize { get; }

    /// <summary>The number of slots in the temp (crafting) inventory.</summary>
    short TempSize { get; }

    /// <summary>
    /// The total number of inventory slots (pack + equip + temp).
    /// </summary>
    short TotalSize { get; }

    /// <summary>The start slot ID for equip inventory.</summary>
    short EquipIndexStart { get; }

    /// <summary>The end slot ID for equip inventory.</summary>
    short EquipIndexEnd { get; }

    /// <summary>The start slot ID for temp inventory.</summary>
    short TempIndexStart { get; }

    /// <summary>The end slot ID for temp inventory.</summary>
    short TempIndexEnd { get; }

    /// <summary>
    /// The raw list of inventory items. Includes all pack, equip and temp items. It is best not to add/remove items from this list directly.
    /// </summary>
    List<InventoryItem> Items { get; }

    /// <summary>
    /// Query how many of a certain item are in (pack) inventory.
    /// </summary>
    /// <param name="itemID">The item to count</param>
    /// <returns></returns>
    int ItemCount(Item itemID);

    /// <summary>Clear all items from inventory.</summary>
    void Clear();

    /// <summary>Add an item to inventory pack.</summary>
    /// <param name="itemID">The item to add.</param>
    /// <returns>The quantity added.</returns>
    int AddToInventory(Item itemID);

    /// <summary>Add an item to inventory pack.</summary>
    /// <param name="itemID">The item to add.</param>
    /// <param name="count">The quantity to add.</param>
    /// <returns>The quantity added.</returns>
    int AddToInventory(Item itemID, int count);

    /// <summary>Add an item to inventory pack.</summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The quantity added.</returns>
    int AddToInventory(InventoryItem item);

    /// <summary>
    /// Decrement an items count. If the item count reaches zero, it is also correctly removed from the players hand, hotbar etc.
    /// </summary>
    /// <param name="itemID">The item to decrement.</param>
    /// <param name="qty">The quantity to decrement.</param>
    /// <returns>The quantity remaining, i.e. qty - actual.</returns>
    int DecrementItem(Item itemID, int qty);

    /// <summary>Find an item in pack inventory.</summary>
    /// <param name="itemID">The item to find.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(Item itemID);

    /// <summary>Find an item in pack inventory.</summary>
    /// <param name="itemID">The item to find.</param>
    /// <param name="mustBeUnequipped">True if the item must be equipped.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(Item itemID, bool mustBeUnequipped);

    /// <summary>Find an item in a region of inventory.</summary>
    /// <param name="fromSlotID">Slot ID to start search.</param>
    /// <param name="toSlotID">Slot ID to end search.</param>
    /// <param name="itemID">The item to Find.</param>
    /// <param name="mustBeUnequipped">True if the item must be equipped.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(int fromSlotID, int toSlotID, Item itemID, bool mustBeUnequipped);

    /// <summary>Find an item in pack inventory.</summary>
    /// <param name="itemID">The item to find.</param>
    /// <param name="count">The count the inventory item must have exactly.</param>
    /// <param name="durability">The durability the inventory item must have exactly.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(Item itemID, int count, ushort durability);

    /// <summary>Find an item of a specific type in pack inventory.</summary>
    /// <param name="itemType">The type of item to find.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(ItemType itemType);

    /// <summary>Find an item of a specific subtype in pack inventory.</summary>
    /// <param name="itemSubType">The subtype of item to find.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItem(ItemSubType itemSubType);

    /// <summary>
    /// Find the highest valued item of a specific type in pack inventory.
    /// </summary>
    /// <param name="itemType">The type of item to find.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItemHighestValue(ItemType itemType);

    /// <summary>
    /// Find the highest value item of a specific subtype in pack inventory.
    /// </summary>
    /// <param name="itemSubType">The subtype of item to find.</param>
    /// <returns>The slot ID of the item or -1 if not found.</returns>
    int FindItemHighestValue(ItemSubType itemSubType);

    /// <summary>
    /// Deserialize the Inventory object from a binary stream.
    /// </summary>
    /// <param name="reader">BinaryReader.</param>
    /// <param name="version">The version number of the serialized data.</param>
    void ReadState(BinaryReader reader, int version);

    /// <summary>Serialize the Inventory object to a binary stream.</summary>
    /// <param name="writer">BinaryWriter.</param>
    void WriteState(BinaryWriter writer);
  }
}
