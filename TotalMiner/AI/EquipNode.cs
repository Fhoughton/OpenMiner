// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.EquipNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Equip", BehaviourTreeNodeType.Action)]
  internal class EquipNode : BehaviourTreeNode
  {
    public Item LeftHandItem;
    public Item RightHandItem;
    public ItemType LeftHandType;
    public ItemType RightHandType;
    public ItemSubType LeftHandSubType;
    public ItemSubType RightHandSubType;

    public override string ToStringParms
    {
      get
      {
        string s = "";
        if (this.LeftHandItem != Item.None)
          s = s + this.LeftHandItem.ToString() + " ";
        if (this.RightHandItem != Item.None)
          s = s + this.RightHandItem.ToString() + " ";
        if (this.LeftHandType != ItemType.None)
          s = s + this.LeftHandType.ToString() + " ";
        if (this.RightHandType != ItemType.None)
          s = s + this.RightHandType.ToString() + " ";
        if (this.LeftHandSubType != ItemSubType.None)
          s = s + this.LeftHandSubType.ToString() + " ";
        if (this.RightHandSubType != ItemSubType.None)
          s += this.RightHandSubType.ToString();
        return this.CutString(s, 10);
      }
    }

    public EquipNode()
    {
    }

    public EquipNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      bool flag1 = false;
      bool flag2 = false;
      if (this.LeftHandItem != Item.None && this.npc.EquipItem(InventoryHand.Left, this.LeftHandItem != Item.Hand ? this.LeftHandItem : Item.None))
        flag1 = true;
      if (!flag1 && this.LeftHandType != ItemType.None && this.npc.EquipItem(InventoryHand.Left, this.LeftHandType))
        flag1 = true;
      if (!flag1 && this.LeftHandSubType != ItemSubType.None && this.npc.EquipItem(InventoryHand.Left, this.LeftHandSubType))
        ;
      if (this.RightHandItem != Item.None && this.npc.EquipItem(InventoryHand.Right, this.RightHandItem != Item.Hand ? this.RightHandItem : Item.None))
        flag2 = true;
      if (!flag2 && this.RightHandType != ItemType.None && this.npc.EquipItem(InventoryHand.Right, this.RightHandType))
        flag2 = true;
      if (!flag2 && this.RightHandSubType != ItemSubType.None && this.npc.EquipItem(InventoryHand.Right, this.RightHandSubType))
        ;
      this.Status = BehaviourTreeNodeStatus.Success;
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "LeftHandItem":
        case "RightHandItem":
          win.SetToolTip("Specify the exact item to equip");
          break;
        case "LeftHandType":
        case "RightHandType":
          win.SetToolTip("Specify an item type to equip. If the NPC has multiple items of the same type in it's inventory, it will equip the item of the most (shop) value");
          break;
        case "LeftHandSubType":
        case "RightHandSubType":
          win.SetToolTip("Specify an item sub type to equip. If the NPC has multiple items of the same sub type in it's inventory, it will equip the item of the most (shop) value");
          break;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.LeftHandItem = (Item) reader.ReadUInt16();
      this.RightHandItem = (Item) reader.ReadUInt16();
      if (version <= 271)
        return;
      this.LeftHandType = (ItemType) reader.ReadUInt16();
      this.RightHandType = (ItemType) reader.ReadUInt16();
      this.LeftHandSubType = (ItemSubType) reader.ReadUInt16();
      this.RightHandSubType = (ItemSubType) reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((ushort) this.LeftHandItem);
      writer.Write((ushort) this.RightHandItem);
      writer.Write((ushort) this.LeftHandType);
      writer.Write((ushort) this.RightHandType);
      writer.Write((ushort) this.LeftHandSubType);
      writer.Write((ushort) this.RightHandSubType);
    }
  }
}
