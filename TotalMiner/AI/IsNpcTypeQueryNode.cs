// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsNpcTypeQueryNode
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class IsNpcTypeQueryNode : BehaviourTreeNode
  {
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<ActorType> SearchTypes = new List<ActorType>();
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<ActorType> ExcludeTypes = new List<ActorType>();
    public BehaviourTreeNodeCompareTarget CompareTarget;
    public NpcQueryPreference Preference;

    public override string ToStringParms
    {
      get
      {
        return ((int) this.Preference).ToString() + (this.SearchTypes == null || this.SearchTypes.Count <= 0 ? (object) (string) null : (object) this.CutString(" " + this.SearchTypes[0].ToString(), 8));
      }
    }

    public IsNpcTypeQueryNode()
    {
    }

    public IsNpcTypeQueryNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    public override void SetNPC(INPCBehaviour npc)
    {
      base.SetNPC(npc);
      if (npc == null)
        return;
      for (int index = 0; index < this.SearchTypes.Count; ++index)
      {
        if (this.SearchTypes[index] == ActorType.Self)
          this.SearchTypes[index] = npc.ActorType;
      }
      for (int index = 0; index < this.ExcludeTypes.Count; ++index)
      {
        if (this.ExcludeTypes[index] == ActorType.Self)
          this.ExcludeTypes[index] = npc.ActorType;
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.SearchTypes.Add(ActorType.Player);
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "SearchTypes":
          win.SetToolTip("The query will search for only these types. Leave empty to search all");
          break;
        case "ExcludeTypes":
          win.SetToolTip("The query will exclude these types from it's search. Leave empty to exclude none");
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
      this.CompareTarget = (BehaviourTreeNodeCompareTarget) reader.ReadByte();
      this.Preference = version < 251 ? (NpcQueryPreference) reader.ReadByte() : (NpcQueryPreference) reader.ReadUInt16();
      this.ReadTypeList(reader, this.SearchTypes);
      this.ReadTypeList(reader, this.ExcludeTypes);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.CompareTarget);
      writer.Write((ushort) this.Preference);
      if (this.SearchTypes != null)
      {
        for (int index = this.SearchTypes.Count - 1; index >= 0; --index)
        {
          if (this.SearchTypes[index] == ActorType.None)
            this.SearchTypes.RemoveAt(index);
          else if (this.npc != null && this.SearchTypes[index] == this.npc.ActorType)
            this.SearchTypes[index] = ActorType.Self;
        }
      }
      if (this.ExcludeTypes != null)
      {
        for (int index = this.ExcludeTypes.Count - 1; index >= 0; --index)
        {
          if (this.ExcludeTypes[index] == ActorType.None)
            this.ExcludeTypes.RemoveAt(index);
          else if (this.npc != null && this.ExcludeTypes[index] == this.npc.ActorType)
            this.ExcludeTypes[index] = ActorType.Self;
        }
      }
      this.WriteTypeList(writer, this.SearchTypes);
      this.WriteTypeList(writer, this.ExcludeTypes);
    }
  }
}
