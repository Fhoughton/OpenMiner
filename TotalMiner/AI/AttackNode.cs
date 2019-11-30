// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.AttackNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Attack", BehaviourTreeNodeType.Action)]
  internal class AttackNode : FollowNode
  {
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<ActorType> SearchTypes = new List<ActorType>();
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<ActorType> ExcludeTypes = new List<ActorType>();
    public NpcQueryPreference Preference;

    public AttackNode()
    {
    }

    public AttackNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      List<TargetData> targetedBy = TargetingSystem.GetTargetedBy(this.npc, this.Preference, this.SearchTypes, this.ExcludeTypes);
      INPCBehaviour npcBehaviour = targetedBy == null || targetedBy.Count <= 0 ? this.npc.AITarget : targetedBy[0].Targeter;
      if (npcBehaviour != null && this.PositionType == CoordType.TargetsTargetRelative)
        npcBehaviour = npcBehaviour.AITarget;
      if (npcBehaviour == null || this.ExcludeTypes.Contains(npcBehaviour.ActorType) || (!this.npc.Properties.CanFight.Value || !npcBehaviour.Properties.CanFight.Value))
        return;
      Item itemId = this.npc.RightHand.ItemID;
      float range = itemId != Item.Hand ? ItemData.GetItemStrikeReach(itemId) : this.npc.StrikeRange;
      if ((double) range > 0.0)
        this.Distance = range - 0.2f;
      base.UpdateCore(engine);
      float num = (double) range > 0.0 ? range * range : this.Distance * this.Distance;
      if (npcBehaviour.IsAlive && (double) this.distanceSqFromPosition < (double) num || ItemData.IsSubType(this.npc.RightHand.ItemID, ItemSubType.RangedWeapon))
      {
        if (!this.npc.RightHand.IsSwinging && !engine.World.RayBlockTest(this.npc.EyePosition, this.npc.ViewDirection, range).IsValid)
          this.npc.SwingHand(InventoryHand.Right, this.ExcludeTypes);
        if ((double) this.distanceSqFromPosition < (double) num * 0.850000023841858)
          this.npc.StandStill();
      }
      this.Status = BehaviourTreeNodeStatus.Success;
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "Distance":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.Distance = 0.0f;
      this.PositionType = CoordType.TargetRelative;
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "ExcludeTypes":
          win.SetToolTip("The NPC will not attack any of these types. Leave empty to exclude none");
          break;
      }
    }

    protected override object ValidateCore(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      if (propertyName == "Preference")
      {
        string[] strArray = input.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
        IEnumerable<NpcQueryPreference> values = Utils.GetValues<NpcQueryPreference>();
        NpcQueryPreference npcQueryPreference1 = NpcQueryPreference.None;
        adjustedInput = "";
        foreach (string str in strArray)
        {
          foreach (NpcQueryPreference npcQueryPreference2 in values)
          {
            if (npcQueryPreference2.ToString().Equals(str.Trim(), StringComparison.OrdinalIgnoreCase))
            {
              if (adjustedInput.Length > 0)
                adjustedInput += ", ";
              adjustedInput += npcQueryPreference2.ToString();
              npcQueryPreference1 |= npcQueryPreference2;
              break;
            }
          }
        }
        return (object) npcQueryPreference1;
      }
      if (propertyName == "SearchTypes" || propertyName == "ExcludeTypes")
        return (object) Utils.ValidateTypeList<ActorType>(input, out adjustedInput);
      return base.ValidateCore(propertyName, input, out adjustedInput);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.ReadTypeList(reader, this.ExcludeTypes);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      this.WriteTypeList(writer, this.ExcludeTypes);
    }
  }
}
