// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.NpcSpawnBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class NpcSpawnBlock : DataBlock
  {
    public float SpawnFrequency = 5f;
    public int MaxActiveInstances = 1;
    public int Proximity = 30;
    public LootTable LootTable = new LootTable();
    public ActorType ActorType;
    public string Name;
    public float SpawnTime;
    public DayOrNight DayOrNight;
    public bool RequiresPower;
    public CombatStats CombatStats;
    public Inventory Inventory;
    public string KillScript;
    public string BehaviourTree;
    public string DialogTree;
    public string DialogText;
    public StudioForge.TotalMiner.AI.BehaviourTree DialogTextCache;
    public ushort DialogDelay;
    public string OwnerGamertag;
    public UnlockType OwnerHasAvatarUnlocked;
    public bool ShowOwnerData;
    public bool IsOldMobSpawnBlock;

    public event Player.PlayerTextEventHandler TextChanged;

    private void RaiseTextChanged(Player player, string text)
    {
      if (this.TextChanged == null)
        return;
      this.TextChanged((object) this, new PlayerTextEventArgs(player, text));
    }

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.NPCSpawn;
      }
    }

    public override bool HasInventory
    {
      get
      {
        if (this.Inventory != null)
          return this.Inventory.HasItems();
        return false;
      }
    }

    public NpcSpawnBlock()
    {
      this.BehaviourTree = "System\\AI\\Default";
    }

    public NpcSpawnBlock(GlobalPoint3D p)
      : base(p)
    {
      this.BehaviourTree = "System\\AI\\Default";
    }

    public void SetActorType(ActorType actorType)
    {
      this.ActorType = actorType;
      this.CombatStats.SetFromXML(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) actorType].LevelType]);
    }

    public override void SetScript(string name, DataBlockScriptType type)
    {
      this.KillScript = name;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      NpcSpawnBlock npcSpawnBlock = from as NpcSpawnBlock;
      this.ActorType = npcSpawnBlock.ActorType;
      this.SpawnTime = npcSpawnBlock.SpawnTime;
      this.SpawnFrequency = npcSpawnBlock.SpawnFrequency;
      this.MaxActiveInstances = npcSpawnBlock.MaxActiveInstances;
      this.Proximity = npcSpawnBlock.Proximity;
      this.DayOrNight = npcSpawnBlock.DayOrNight;
      this.RequiresPower = npcSpawnBlock.RequiresPower;
      this.CombatStats = npcSpawnBlock.CombatStats;
      this.LootTable = npcSpawnBlock.LootTable.Clone();
      this.KillScript = npcSpawnBlock.KillScript;
      this.BehaviourTree = npcSpawnBlock.BehaviourTree;
      this.Inventory = npcSpawnBlock.HasInventory ? new Inventory(npcSpawnBlock.Inventory) : (Inventory) null;
      this.Name = npcSpawnBlock.Name;
      this.DialogText = npcSpawnBlock.DialogText;
      this.DialogTree = npcSpawnBlock.DialogTree;
      this.DialogDelay = npcSpawnBlock.DialogDelay;
      this.OwnerGamertag = npcSpawnBlock.OwnerGamertag;
      this.OwnerHasAvatarUnlocked = npcSpawnBlock.OwnerHasAvatarUnlocked;
      this.ShowOwnerData = npcSpawnBlock.ShowOwnerData;
    }

    public void LoadFromSaveData(SaveNPCState state)
    {
      this.ActorType = state.Type;
      this.DialogText = state.Text;
      this.DialogDelay = (ushort) 10;
      this.Name = this.ActorType.ToString();
      this.ShowOwnerData = false;
      this.OwnerGamertag = (string) null;
      this.OwnerHasAvatarUnlocked = UnlockType.Unknown;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version > (int) byte.MaxValue)
      {
        this.ActorType = (ActorType) reader.ReadByte();
        this.SpawnFrequency = reader.ReadSingle();
        this.DayOrNight = (DayOrNight) reader.ReadByte();
        this.RequiresPower = reader.ReadBoolean();
        if (version > 112)
          this.MaxActiveInstances = (int) reader.ReadUInt16();
        if (version > 140)
          this.Proximity = (int) reader.ReadUInt16();
        if (version > 165)
          this.KillScript = reader.ReadString();
        if (version > 111)
          this.CombatStats.ReadState(reader, version);
        else
          this.CombatStats.SetFromXML(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.ActorType].LevelType]);
        if (version > 130)
          this.LootTable.ReadState(reader, version);
        else
          this.LootTable = new LootTable();
        if (version > (int) byte.MaxValue)
          this.BehaviourTree = reader.ReadString();
        this.Inventory = new Inventory(10, 0, 0);
        this.DialogText = this.DialogTree = (string) null;
        this.DialogDelay = (ushort) 10;
        if (version > (int) byte.MaxValue)
        {
          this.Inventory.ReadState(reader, version);
          this.Name = reader.ReadString();
          this.DialogTree = reader.ReadString();
          if (version > 260)
            this.DialogText = reader.ReadString();
          this.ShowOwnerData = reader.ReadBoolean();
          this.OwnerHasAvatarUnlocked = (UnlockType) reader.ReadByte();
          this.OwnerGamertag = reader.ReadString();
          if (this.OwnerGamertag != null && this.OwnerGamertag.Length < 1)
            this.OwnerGamertag = (string) null;
          if (version <= 264)
            return;
          this.DialogDelay = reader.ReadUInt16();
        }
        else
        {
          this.Name = this.ActorType.ToString();
          this.ShowOwnerData = false;
          this.OwnerGamertag = (string) null;
          this.OwnerHasAvatarUnlocked = UnlockType.Unknown;
        }
      }
      else if (this.IsOldMobSpawnBlock)
      {
        this.ActorType = (ActorType) reader.ReadByte();
        this.SpawnFrequency = reader.ReadSingle();
        this.DayOrNight = (DayOrNight) reader.ReadByte();
        this.RequiresPower = reader.ReadBoolean();
        if (version > 112)
          this.MaxActiveInstances = (int) reader.ReadUInt16();
        if (version > 140)
          this.Proximity = (int) reader.ReadUInt16();
        if (version > 165)
          this.KillScript = reader.ReadString();
        if (version > 111)
          this.CombatStats.ReadState(reader, version);
        else
          this.CombatStats.SetFromXML(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.ActorType].LevelType]);
        if (version > 130)
          this.LootTable.ReadState(reader, version);
        else
          this.LootTable = new LootTable();
      }
      else
      {
        this.ActorType = (ActorType) reader.ReadByte();
        if (this.ActorType == ActorType.None)
          this.ActorType = ActorType.Boy;
        this.DialogText = reader.ReadString();
        this.DialogDelay = (ushort) 10;
        this.Name = version <= 163 ? this.ActorType.ToString() : reader.ReadString();
        reader.ReadString();
        double num1 = (double) reader.ReadSingle();
        double num2 = (double) reader.ReadSingle();
        if (version > 198)
        {
          this.ShowOwnerData = reader.ReadBoolean();
          this.OwnerHasAvatarUnlocked = (UnlockType) reader.ReadByte();
          this.OwnerGamertag = reader.ReadString();
          if (this.OwnerGamertag == null || this.OwnerGamertag.Length >= 1)
            return;
          this.OwnerGamertag = (string) null;
        }
        else
        {
          this.ShowOwnerData = false;
          this.OwnerGamertag = (string) null;
          this.OwnerHasAvatarUnlocked = UnlockType.Unknown;
        }
      }
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.ActorType);
      writer.Write(this.SpawnFrequency);
      writer.Write((byte) this.DayOrNight);
      writer.Write(this.RequiresPower);
      writer.Write((ushort) this.MaxActiveInstances);
      writer.Write((ushort) this.Proximity);
      writer.Write(this.KillScript != null ? this.KillScript : "");
      this.CombatStats.WriteState(writer);
      this.LootTable.WriteState(writer);
      writer.Write(this.BehaviourTree != null ? this.BehaviourTree : "");
      (this.Inventory != null ? this.Inventory : new Inventory(10, 0, 0)).WriteState(writer);
      writer.Write(this.Name != null ? this.Name : "");
      writer.Write(this.DialogTree != null ? this.DialogTree : "");
      writer.Write(this.DialogText != null ? this.DialogText : "");
      writer.Write(this.ShowOwnerData);
      writer.Write((byte) this.OwnerHasAvatarUnlocked);
      writer.Write(this.OwnerGamertag != null ? this.OwnerGamertag : "");
      writer.Write(this.DialogDelay);
    }
  }
}
