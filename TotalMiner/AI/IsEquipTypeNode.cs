// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsEquipTypeNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.API;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsEquipType", BehaviourTreeNodeType.Conditional)]
  internal class IsEquipTypeNode : BehaviourTreeNode
  {
    public BehaviourTreeNodeCompareTarget Target;
    public InventoryHand Hand;
    public Item Item;
    public ItemType Type;
    public ItemSubType SubType;
    public ItemTypeClass Class;

    public override string ToStringParms
    {
      get
      {
        if (this.Item == Item.None)
          return (string) null;
        return this.Item.ToString();
      }
    }

    public IsEquipTypeNode()
    {
    }

    public IsEquipTypeNode(INPCBehaviour npc)
    {
      this.npc = npc;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        bool flag = false;
        INPCBehaviour npcBehaviour = this.Target == BehaviourTreeNodeCompareTarget.Self ? this.npc : this.npc.AITarget;
        if (npcBehaviour != null)
        {
          if (this.Hand == InventoryHand.None)
          {
            flag = this.CheckIsEquipped(npcBehaviour.Inventory, npcBehaviour.LeftHand.HandIndex) || this.CheckIsEquipped(npcBehaviour.Inventory, npcBehaviour.RightHand.HandIndex);
          }
          else
          {
            int slotID = this.Hand == InventoryHand.Left ? npcBehaviour.LeftHand.HandIndex : npcBehaviour.RightHand.HandIndex;
            flag = this.CheckIsEquipped(npcBehaviour.Inventory, slotID);
          }
        }
        this.Status = flag ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    private bool CheckIsEquipped(ITMInventory inventory, int slotID)
    {
      if (slotID >= 0 && slotID < inventory.Items.Count)
      {
        InventoryItem inventoryItem = inventory.Items[slotID];
        if (inventoryItem.ItemID != Item.None && (this.Item == Item.None || inventoryItem.ItemID == this.Item) && ((this.Type == ItemType.None || ItemData.IsItemType(inventoryItem.ItemID, this.Type)) && (this.SubType == ItemSubType.None || ItemData.IsSubType(inventoryItem.ItemID, this.SubType))) && (this.Class == ItemTypeClass.None || ItemData.IsItemTypeClass(inventoryItem.ItemID, this.Class)))
          return true;
      }
      return false;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Target = version <= 281 ? BehaviourTreeNodeCompareTarget.Self : (BehaviourTreeNodeCompareTarget) reader.ReadByte();
      this.Hand = (InventoryHand) reader.ReadByte();
      this.Item = (Item) reader.ReadUInt16();
      this.Type = (ItemType) reader.ReadByte();
      this.SubType = (ItemSubType) reader.ReadUInt16();
      this.Class = (ItemTypeClass) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.Target);
      writer.Write((byte) this.Hand);
      writer.Write((ushort) this.Item);
      writer.Write((byte) this.Type);
      writer.Write((ushort) this.SubType);
      writer.Write((byte) this.Class);
    }
  }
}
