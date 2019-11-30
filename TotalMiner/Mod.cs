// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Mod
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StudioForge.TotalMiner
{
  internal class Mod
  {
    public readonly string Name;
    public List<Assembly> Assemblies;
    private readonly ushort modID;
    private string path;
    private bool isActive;
    private EnumTypeOffsets typeOffsets;
    private EnumTypeOffsets typeCounts;
    private List<string> itemIDs;
    private List<string> itemTypeClasses;
    private List<string> itemCombats;
    private List<string> itemSwings;
    private List<string> itemModels;
    private List<string> actorTypes;
    private List<string> actorAIs;
    private List<string> actorLevels;
    private List<string> actorPhysics;
    private List<ItemDataXML> newItemDataXML;
    private List<ItemTypeClassDataXML> newItemTypeClassDataXML;
    private List<ItemCombatDataXML> newItemCombatDataXML;
    private List<ItemSwingDataXML> newItemSwingDataXML;
    private List<ItemModelDataXML> newItemModelDataXML;
    private List<BlueprintXML> newBlueprintDataXML;
    private List<ActorTypeDataXML> newActorTypeDataXML;
    private List<ActorAIDataXML> newActorAIDataXML;
    private List<ActorLevelDataXML> newActorLevelDataXML;
    private List<ActorPhysicsDataXML> newActorPhysicsDataXML;

    public ITMPlugin Plugin { get; private set; }

    public ITMPluginNet PluginNet { get; private set; }

    public ITMPluginGUI PluginGUI { get; private set; }

    public ITMPluginBlocks PluginBlocks { get; private set; }

    public ITMPluginArcade PluginArcade { get; private set; }

    public ITMNetworkManager NetworkManager { get; private set; }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
    }

    public ushort ModID
    {
      get
      {
        return this.modID;
      }
    }

    public EnumTypeOffsets TypeOffsets
    {
      get
      {
        return this.typeOffsets;
      }
    }

    public EnumTypeOffsets TypeCounts
    {
      get
      {
        return this.typeCounts;
      }
    }

    public Mod(ushort modID, string name)
    {
      this.modID = modID;
      this.Name = name;
      this.Assemblies = new List<Assembly>();
    }

    public string Load()
    {
      this.path = ModManager.ModsPath + this.Name + (object) '\\';
      if (!FileSystem.IsDirExist(this.path))
        return "Mod folder not found: " + this.Name;
      List<string> err = new List<string>(20);
      this.LoadAssemblies(err);
      this.LoadCodeLists();
      err.Add(this.LoadBlockData());
      err.Add(this.LoadBlockMaterialData());
      err.Add(this.LoadItemCombatData());
      err.Add(this.LoadItemTypeClassData());
      err.Add(this.LoadItemSwingData());
      err.Add(this.LoadItemModelData());
      err.Add(this.LoadItemData());
      err.Add(this.LoadItemTypeData());
      err.Add(this.LoadItemSwingTimeData());
      err.Add(this.LoadItemSoundData());
      err.Add(this.LoadSkillData());
      err.Add(this.LoadBlueprintData());
      err.Add(this.LoadParticleData());
      err.Add(this.LoadActorLevelData());
      err.Add(this.LoadActorPhysicsData());
      err.Add(this.LoadActorAIData());
      err.Add(this.LoadActorAudioData());
      err.Add(this.LoadActorTypeData());
      err.Add(this.LoadBehavioursAndDialog());
      return Mod.BuildErrString(err);
    }

    private void LoadCodeLists()
    {
      this.itemIDs = new List<string>(Globals1.ItemData.Length + 10);
      foreach (ItemDataXML itemDataXml in Globals1.ItemData)
        this.itemIDs.Add(itemDataXml.IDString);
      this.itemTypeClasses = new List<string>(Globals1.ItemTypeClassData.Length + 10);
      foreach (ItemTypeClassDataXML typeClassDataXml in Globals1.ItemTypeClassData)
        this.itemTypeClasses.Add(typeClassDataXml.ItemClass.ToString());
      this.itemCombats = new List<string>(Globals1.ItemCombatData.Length + 10);
      foreach (ItemCombatDataXML itemCombatDataXml in Globals1.ItemCombatData)
        this.itemCombats.Add(itemCombatDataXml.CombatID.ToString());
      this.itemSwings = new List<string>(Globals1.ItemSwingData.Length + 10);
      foreach (ItemSwingDataXML itemSwingDataXml in Globals1.ItemSwingData)
        this.itemSwings.Add(itemSwingDataXml.SwingType.ToString());
      this.itemModels = new List<string>(Globals1.ItemModelData.Length + 10);
      foreach (ItemModelDataXML itemModelDataXml in Globals1.ItemModelData)
        this.itemModels.Add(itemModelDataXml.ItemType.ToString());
      this.actorTypes = new List<string>(Globals1.NpcTypeData.Length + 10);
      foreach (ActorTypeDataXML actorTypeDataXml in Globals1.NpcTypeData)
        this.actorTypes.Add(actorTypeDataXml.IDString);
      this.actorLevels = new List<string>(Globals1.NpcLevelData.Length + 10);
      foreach (ActorLevelDataXML actorLevelDataXml in Globals1.NpcLevelData)
        this.actorLevels.Add(actorLevelDataXml.ActorLevelType.ToString());
      this.actorPhysics = new List<string>(Globals1.NpcPhysicsData.Length + 10);
      foreach (ActorPhysicsDataXML actorPhysicsDataXml in Globals1.NpcPhysicsData)
        this.actorPhysics.Add(actorPhysicsDataXml.ActorPhysicsType.ToString());
      this.actorAIs = new List<string>(Globals1.NpcAIData.Length + 10);
      foreach (ActorAIDataXML actorAiDataXml in Globals1.NpcAIData)
        this.actorAIs.Add(actorAiDataXml.ActorAIType.ToString());
    }

    private void LoadAssemblies(List<string> err)
    {
      string[] files = FileSystem.GetFiles(this.path, "*.dll");
      if (files.Length <= 0)
        return;
      foreach (string assemblyName in files)
      {
        err.Add(this.LoadAssembly(assemblyName));
        if (this.Plugin != null)
          break;
      }
      if (this.Plugin != null)
        return;
      err.Add("TMPluginProvider not provided");
    }

    private string LoadAssembly(string assemblyName)
    {
      try
      {
        Assembly assembly = Assembly.LoadFile(FileSystem.RootPath + assemblyName);
        if (assembly == (Assembly) null)
          return "Could not load mod assembly: " + assemblyName;
        this.Assemblies.Add(assembly);
        ITMPluginProvider pluginProvider = ModManager.GetPluginProvider(assembly);
        if (pluginProvider != null)
        {
          this.Plugin = pluginProvider.GetPlugin();
          if (this.Plugin == null)
            return "Plugin not provided";
          this.PluginNet = pluginProvider.GetPluginNet();
          this.PluginGUI = pluginProvider.GetPluginGUI();
          this.PluginBlocks = pluginProvider.GetPluginBlocks();
          this.PluginArcade = pluginProvider.GetPluginArcade();
          this.typeOffsets = ModManager.InterfaceManager.Offsets;
          this.Plugin.Initialize(ModManager.InterfaceManager, this.path);
          this.typeCounts = ModManager.InterfaceManager.Offsets - this.typeOffsets;
          if (this.PluginNet != null)
            this.NetworkManager = this.PluginNet.GetNetworkManager();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "LoadAssembly: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private string LoadBlockData()
    {
      try
      {
        string path = this.path + "BlockData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModBlockDataXML modBlockDataXml in FileSystem.Deserialize<ModBlockDataXML[]>(path))
          {
            int blockId = (int) modBlockDataXml.BlockID;
            if (blockId > 0 && blockId < Globals1.BlockData.Length)
            {
              Mod.ReplaceItem(modBlockDataXml, blockId);
              ModManager.RegisterBlockIDs(modBlockDataXml.BlockID, this);
            }
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "BlockData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private static void ReplaceItem(ModBlockDataXML item, int i)
    {
      BlockDataXML blockDataXml = Globals1.BlockData[i];
      if (item.Material.HasValue)
        blockDataXml.Material = item.Material.Value;
      if (item.ClassType.HasValue)
        blockDataXml.ClassType = item.ClassType.Value;
      if (item.Opacity.HasValue)
        blockDataXml.Opacity = item.Opacity.Value;
      if (item.Luminance.HasValue)
        blockDataXml.Luminance = item.Luminance.Value;
      if (item.Friction.HasValue)
        blockDataXml.Friction = item.Friction.Value;
      if (item.Dampen.HasValue)
        blockDataXml.Dampen = item.Dampen.Value;
      if (item.Buffer.HasValue)
        blockDataXml.Buffer = item.Buffer.Value;
      if (item.IsIcon.HasValue)
        blockDataXml.IsIcon = item.IsIcon.Value;
      if (item.IsAttached.HasValue)
        blockDataXml.IsAttached = item.IsAttached.Value;
      if (item.IsPassable.HasValue)
        blockDataXml.IsPassable = item.IsPassable.Value;
      if (item.IsRotated.HasValue)
        blockDataXml.IsRotated = item.IsRotated.Value;
      if (item.IsOrientated.HasValue)
        blockDataXml.IsOrientated = item.IsOrientated.Value;
      if (item.IsOreDeposit.HasValue)
        blockDataXml.IsOreDeposit = item.IsOreDeposit.Value;
      if (item.IsPowerEmitter.HasValue)
        blockDataXml.IsPowerEmitter = item.IsPowerEmitter.Value;
      if (item.IsPoweredMechanism.HasValue)
        blockDataXml.IsPoweredMechanism = item.IsPoweredMechanism.Value;
      if (item.IsVertSunlightUnhindered.HasValue)
        blockDataXml.IsVertSunlightUnhindered = item.IsVertSunlightUnhindered.Value;
      if (item.BlastResistance.HasValue)
        blockDataXml.BlastResistance = item.BlastResistance.Value;
      if (item.WindAffect.HasValue)
        blockDataXml.WindAffect = item.WindAffect.Value;
      if (item.TextureID.HasValue)
        blockDataXml.TextureID = item.TextureID.Value;
      Globals1.BlockData[i] = blockDataXml;
    }

    private string LoadBlockMaterialData()
    {
      try
      {
        string path = this.path + "BlockMaterialData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModBlockMaterialDataXML blockMaterialDataXml in FileSystem.Deserialize<ModBlockMaterialDataXML[]>(path))
            Mod.ReplaceItem(blockMaterialDataXml);
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "BlockMaterialData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private static void ReplaceItem(ModBlockMaterialDataXML item)
    {
      int material = (int) item.Material;
      if (material < 0 || material >= Globals1.BlockMaterialData.Length)
        return;
      BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[material];
      if (item.Flags.HasValue)
      {
        int num1 = (int) item.Flags.Value;
      }
      if (item.Resistance.HasValue)
      {
        int num2 = (int) item.Resistance.Value;
      }
      if (item.BaseEfficiency.HasValue)
      {
        int num3 = (int) item.BaseEfficiency.Value;
      }
      if (item.PickEfficiency.HasValue)
      {
        int num4 = (int) item.PickEfficiency.Value;
      }
      if (item.ShovelEfficiency.HasValue)
      {
        int num5 = (int) item.ShovelEfficiency.Value;
      }
      if (item.HatchetEfficiency.HasValue)
      {
        int num6 = (int) item.HatchetEfficiency.Value;
      }
      if (item.WeaponEfficiency.HasValue)
      {
        int num7 = (int) item.WeaponEfficiency.Value;
      }
      if (!item.XPAdjust.HasValue)
        return;
      double num8 = (double) item.XPAdjust.Value;
    }

    private string LoadItemData()
    {
      try
      {
        string path = this.path + "ItemData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemDataXML modItemDataXml in FileSystem.Deserialize<ModItemDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(modItemDataXml.ItemID);
            if (index < 0)
              this.AddNewItem(modItemDataXml);
            else
              this.ReplaceItem(index, modItemDataXml);
          }
          if (this.newItemDataXML != null && this.newItemDataXML.Count > 0)
            this.ExpandItemData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModItemDataXML item)
    {
      if (this.newItemDataXML == null)
        this.newItemDataXML = new List<ItemDataXML>();
      ItemDataXML data = new ItemDataXML();
      data.ItemID = (Item) this.itemIDs.Count;
      data.IDString = item.ItemID;
      data.IsEnabled = true;
      data.IsValid = true;
      data.MinCSPrice = 1;
      data.Plural = PluralType.S;
      data.StackSize = 100;
      this.ReplaceItemCore(item, data);
      this.newItemDataXML.Add(data);
      this.itemIDs.Add(item.ItemID);
    }

    private void ReplaceItem(int index, ModItemDataXML item)
    {
      if (index >= Globals1.ItemData.Length)
        throw new ModLoadException("Item: " + item.ItemID + " is duplicated. Either remove the duplicate or make it's name (ItemID) unique.");
      ItemDataXML data = Globals1.ItemData[index];
      this.ReplaceItemCore(item, data);
    }

    private void ReplaceItemCore(ModItemDataXML item, ItemDataXML data)
    {
      if (item.Name.IsNotEmpty())
        data.Name = item.Name;
      if (item.Desc.IsNotEmpty())
        data.Desc = item.Desc;
      if (item.IsValid.HasValue)
        data.IsValid = item.IsValid.Value;
      if (item.IsEnabled.HasValue)
        data.IsEnabled = item.IsEnabled.Value;
      if (item.LockedDD.HasValue)
        data.LockedDD = item.LockedDD.Value;
      if (item.LockedCR.HasValue)
        data.LockedCR = item.LockedCR.Value;
      if (item.LockedSU.HasValue)
        data.LockedSU = item.LockedSU.Value;
      if (item.MinCSPrice.HasValue)
        data.MinCSPrice = item.MinCSPrice.Value;
      if (item.StackSize.HasValue)
        data.StackSize = item.StackSize.Value;
      if (item.Durability.HasValue)
        data.Durability = item.Durability.Value;
      if (item.StrikeDamage.HasValue)
        data.StrikeDamage = item.StrikeDamage.Value;
      if (item.StrikeReach.HasValue)
        data.StrikeReach = item.StrikeReach.Value;
      if (item.HealPower.HasValue)
        data.HealPower = item.HealPower.Value;
      if (item.BurnTime.HasValue)
        data.BurnTime = item.BurnTime.Value;
      if (item.SmeltTime.HasValue)
        data.SmeltTime = item.SmeltTime.Value;
      if (item.ParticleLight.HasValue)
        data.ParticleLight = item.ParticleLight.Value;
      if (item.SelectFlag.HasValue)
        data.SelectFlag = item.SelectFlag.Value;
      if (item.CanDropIfLocked.HasValue)
        data.CanDropIfLocked = item.CanDropIfLocked.Value;
      if (item.DropChance.HasValue)
        data.DropChance = item.DropChance.Value;
      if (!item.Plural.HasValue)
        return;
      data.Plural = item.Plural.Value;
    }

    private void ExpandItemData()
    {
      int length1 = Globals1.ItemData.Length;
      int length2 = length1 + this.newItemDataXML.Count;
      ItemDataXML[] array = new ItemDataXML[length2];
      ItemTypeDataXML[] itemTypeDataXmlArray = new ItemTypeDataXML[length2];
      ItemSwingTimeDataXML[] swingTimeDataXmlArray = new ItemSwingTimeDataXML[length2];
      ItemSoundDataXML[] itemSoundDataXmlArray = new ItemSoundDataXML[length2];
      SkillDataXML[] skillDataXmlArray = new SkillDataXML[length2];
      Globals1.ItemData.CopyTo((Array) array, 0);
      Globals1.ItemTypeData.CopyTo((Array) itemTypeDataXmlArray, 0);
      Globals1.ItemSwingTimeData.CopyTo((Array) swingTimeDataXmlArray, 0);
      Globals1.ItemSoundData.CopyTo((Array) itemSoundDataXmlArray, 0);
      Globals1.SkillData.CopyTo((Array) skillDataXmlArray, 0);
      for (int index = length1; index < length2; ++index)
      {
        itemTypeDataXmlArray[index].Equip = EquipIndex.LeftHand;
        itemTypeDataXmlArray[index].Inv = ItemInvType.Other;
        itemTypeDataXmlArray[index].Model = ItemModelType.Item;
        itemTypeDataXmlArray[index].Swing = ItemSwingType.Item;
        itemTypeDataXmlArray[index].Type = ItemType.Item;
        itemTypeDataXmlArray[index].Use = ItemUse.Item;
        swingTimeDataXmlArray[index] = Globals1.ItemSwingTimeData[281];
        itemSoundDataXmlArray[index] = Globals1.ItemSoundData[281];
        skillDataXmlArray[index] = Globals1.SkillData[281];
      }
      this.newItemDataXML.CopyTo(array, length1);
      Globals1.ItemData = array;
      Globals1.ItemTypeData = itemTypeDataXmlArray;
      Globals1.ItemSwingTimeData = swingTimeDataXmlArray;
      Globals1.ItemSoundData = itemSoundDataXmlArray;
      Globals1.SkillData = skillDataXmlArray;
      ItemModelManager.ResetCache();
    }

    private string LoadItemCombatData()
    {
      try
      {
        string path = this.path + "ItemCombatData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemCombatDataXML itemCombatDataXml in FileSystem.Deserialize<ModItemCombatDataXML[]>(path))
          {
            int index = this.itemCombats.IndexOf(itemCombatDataXml.CombatID);
            if (index < 0)
              this.AddNewItem(itemCombatDataXml);
            else
              this.ReplaceItem(index, itemCombatDataXml);
          }
          if (this.newItemCombatDataXML != null && this.newItemCombatDataXML.Count > 0)
            this.ExpandItemCombatData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemCombatData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModItemCombatDataXML item)
    {
      if (this.newItemCombatDataXML == null)
        this.newItemCombatDataXML = new List<ItemCombatDataXML>();
      ItemCombatDataXML data = new ItemCombatDataXML();
      data.CombatID = (CombatItem) this.itemCombats.Count;
      this.ReplaceItemCore(item, ref data);
      this.newItemCombatDataXML.Add(data);
      this.itemCombats.Add(item.CombatID);
    }

    private void ReplaceItem(int index, ModItemCombatDataXML item)
    {
      ItemCombatDataXML data = Globals1.ItemCombatData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.ItemCombatData[index] = data;
    }

    private void ReplaceItemCore(ModItemCombatDataXML item, ref ItemCombatDataXML data)
    {
      if (item.Health.HasValue)
        data.Health = item.Health.Value;
      if (item.Attack.HasValue)
        data.Attack = item.Attack.Value;
      if (item.Strength.HasValue)
        data.Strength = item.Strength.Value;
      if (item.Defence.HasValue)
        data.Defence = item.Defence.Value;
      if (item.Ranged.HasValue)
        data.Ranged = item.Ranged.Value;
      if (!item.Looting.HasValue)
        return;
      data.Looting = item.Looting.Value;
    }

    private void ExpandItemCombatData()
    {
      int length = Globals1.ItemCombatData.Length;
      ItemCombatDataXML[] array = new ItemCombatDataXML[length + this.newItemCombatDataXML.Count];
      Globals1.ItemCombatData.CopyTo((Array) array, 0);
      this.newItemCombatDataXML.CopyTo(array, length);
      Globals1.ItemCombatData = array;
    }

    private string LoadItemTypeClassData()
    {
      try
      {
        string path = this.path + "ItemTypeClassData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemTypeClassDataXML typeClassDataXml in FileSystem.Deserialize<ModItemTypeClassDataXML[]>(path))
          {
            int index = this.itemTypeClasses.IndexOf(typeClassDataXml.ClassID);
            if (index < 0)
              this.AddNewItem(typeClassDataXml);
            else
              this.ReplaceItem(index, typeClassDataXml);
          }
          if (this.newItemTypeClassDataXML != null && this.newItemTypeClassDataXML.Count > 0)
            this.ExpandItemTypeClassData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemTypeClassData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModItemTypeClassDataXML item)
    {
      if (this.newItemTypeClassDataXML == null)
        this.newItemTypeClassDataXML = new List<ItemTypeClassDataXML>();
      ItemTypeClassDataXML data = new ItemTypeClassDataXML();
      data.ItemClass = (ItemTypeClass) this.itemTypeClasses.Count;
      this.ReplaceItemCore(item, ref data);
      this.newItemTypeClassDataXML.Add(data);
      this.itemTypeClasses.Add(item.ClassID);
    }

    private void ReplaceItem(int index, ModItemTypeClassDataXML item)
    {
      if (index >= Globals1.ItemTypeClassData.Length)
        throw new ModLoadException("Item: " + item.ClassID + " is duplicated. Either remove the duplicate or make it's name (ClassID) unique.");
      ItemTypeClassDataXML data = Globals1.ItemTypeClassData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.ItemTypeClassData[index] = data;
    }

    private void ReplaceItemCore(ModItemTypeClassDataXML item, ref ItemTypeClassDataXML data)
    {
      if (item.Power.HasValue)
        data.Power = item.Power.Value;
      if (!item.MaxResistance.HasValue)
        return;
      data.MaxResistance = item.MaxResistance.Value;
    }

    private void ExpandItemTypeClassData()
    {
      int length = Globals1.ItemTypeClassData.Length;
      ItemTypeClassDataXML[] array = new ItemTypeClassDataXML[length + this.newItemTypeClassDataXML.Count];
      Globals1.ItemTypeClassData.CopyTo((Array) array, 0);
      this.newItemTypeClassDataXML.CopyTo(array, length);
      Globals1.ItemTypeClassData = array;
    }

    private string LoadItemTypeData()
    {
      try
      {
        string path = this.path + "ItemTypeData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemTypeDataXML modItemTypeDataXml in FileSystem.Deserialize<ModItemTypeDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(modItemTypeDataXml.ItemID);
            if (index >= 0)
              this.ReplaceItem(index, modItemTypeDataXml);
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemTypeData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void ReplaceItem(int index, ModItemTypeDataXML item)
    {
      if (index >= Globals1.ItemTypeData.Length)
        throw new ModLoadException("Item: " + item.ItemID + " is duplicated. Either remove the duplicate or make it's name (ItemID) unique.");
      ItemTypeDataXML itemTypeDataXml = Globals1.ItemTypeData[index];
      if (item.Use.HasValue)
        itemTypeDataXml.Use = item.Use.Value;
      if (item.Type.HasValue)
        itemTypeDataXml.Type = item.Type.Value;
      if (item.SubType.HasValue)
        itemTypeDataXml.SubType = item.SubType.Value;
      if (item.ClassID != null)
        itemTypeDataXml.Class = (ItemTypeClass) this.itemTypeClasses.IndexOf(item.ClassID);
      if (item.Inv.HasValue)
        itemTypeDataXml.Inv = item.Inv.Value;
      if (item.CombatID != null)
        itemTypeDataXml.Combat = (CombatItem) this.itemCombats.IndexOf(item.CombatID);
      if (item.Model.HasValue)
        itemTypeDataXml.Model = item.Model.Value;
      if (item.Swing.HasValue)
        itemTypeDataXml.Swing = item.Swing.Value;
      if (item.Equip.HasValue)
        itemTypeDataXml.Equip = item.Equip.Value;
      Globals1.ItemTypeData[index] = itemTypeDataXml;
    }

    private string LoadItemSwingData()
    {
      try
      {
        string path = this.path + "ItemSwingData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemSwingDataXML itemSwingDataXml in FileSystem.Deserialize<ModItemSwingDataXML[]>(path))
          {
            int index = this.itemSwings.IndexOf(itemSwingDataXml.SwingType);
            if (index < 0)
              this.AddNewItem(itemSwingDataXml);
            else
              this.ReplaceItem(index, itemSwingDataXml);
          }
          if (this.newItemSwingDataXML != null && this.newItemSwingDataXML.Count > 0)
            this.ExpandItemSwingData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemSwingData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModItemSwingDataXML item)
    {
      if (this.newItemSwingDataXML == null)
        this.newItemSwingDataXML = new List<ItemSwingDataXML>();
      ItemSwingDataXML data = Globals1.ItemSwingData[2];
      data.SwingType = (ItemSwingType) this.itemSwings.Count;
      this.ReplaceItemCore(item, ref data);
      this.newItemSwingDataXML.Add(data);
      this.itemSwings.Add(item.SwingType);
    }

    private void ReplaceItem(int index, ModItemSwingDataXML item)
    {
      ItemSwingDataXML data = Globals1.ItemSwingData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.ItemSwingData[index] = data;
    }

    private void ReplaceItemCore(ModItemSwingDataXML item, ref ItemSwingDataXML data)
    {
      if (item.IsSwingable.HasValue)
        data.IsSwingable = item.IsSwingable.Value;
      if (item.SwingTime.HasValue)
        data.SwingTime = item.SwingTime.Value;
      if (item.RestPosition.HasValue)
        data.RestPosition = item.RestPosition.Value;
      if (item.RestRotation.HasValue)
        data.RestRotation = item.RestRotation.Value;
      if (item.ExtendedPosition.HasValue)
        data.ExtendedPosition = item.ExtendedPosition.Value;
      if (item.ExtendedPositionFPV.HasValue)
        data.ExtendedPositionFPV = item.ExtendedPositionFPV.Value;
      if (item.ExtendedRotation.HasValue)
        data.ExtendedRotation = item.ExtendedRotation.Value;
      if (item.ExtendedRotationFPV.HasValue)
        data.ExtendedRotationFPV = item.ExtendedRotationFPV.Value;
      if (item.CircularY.HasValue)
        data.CircularY = item.CircularY.Value;
      if (item.CircularZ.HasValue)
        data.CircularZ = item.CircularZ.Value;
      if (!item.CircularYFPV.HasValue)
        return;
      data.CircularYFPV = item.CircularYFPV.Value;
    }

    private void ExpandItemSwingData()
    {
      int length = Globals1.ItemSwingData.Length;
      ItemSwingDataXML[] array = new ItemSwingDataXML[length + this.newItemSwingDataXML.Count];
      Globals1.ItemSwingData.CopyTo((Array) array, 0);
      this.newItemSwingDataXML.CopyTo(array, length);
      Globals1.ItemSwingData = array;
    }

    private string LoadItemSwingTimeData()
    {
      try
      {
        string path = this.path + "ItemSwingTimeData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemSwingTimeDataXML swingTimeDataXml in FileSystem.Deserialize<ModItemSwingTimeDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(swingTimeDataXml.ItemID);
            if (index >= 0)
              this.ReplaceItem(index, swingTimeDataXml);
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemSwingTimeData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void ReplaceItem(int index, ModItemSwingTimeDataXML item)
    {
      ItemSwingTimeDataXML swingTimeDataXml = Globals1.ItemSwingTimeData[index];
      if (item.Time.HasValue)
        swingTimeDataXml.Time = item.Time.Value;
      if (item.Pause.HasValue)
        swingTimeDataXml.Pause = item.Pause.Value;
      if (item.ExtendedPause.HasValue)
        swingTimeDataXml.ExtendedPause = item.ExtendedPause.Value;
      if (item.RetractTime.HasValue)
        swingTimeDataXml.RetractTime = item.RetractTime.Value;
      if (item.RetractSmooth.HasValue)
        swingTimeDataXml.RetractSmooth = item.RetractSmooth.Value;
      Globals1.ItemSwingTimeData[index] = swingTimeDataXml;
    }

    private string LoadItemModelData()
    {
      try
      {
        string path = this.path + "ItemModelData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemModelDataXML itemModelDataXml in FileSystem.Deserialize<ModItemModelDataXML[]>(path))
          {
            int index = this.itemModels.IndexOf(itemModelDataXml.ItemType);
            if (index < 0)
              this.AddNewItem(itemModelDataXml);
            else
              this.ReplaceItem(index, itemModelDataXml);
          }
          if (this.newItemModelDataXML != null && this.newItemModelDataXML.Count > 0)
            this.ExpandItemModelData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemModelData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModItemModelDataXML item)
    {
      if (this.newItemModelDataXML == null)
        this.newItemModelDataXML = new List<ItemModelDataXML>();
      ItemModelDataXML data = Globals1.ItemModelData[4];
      data.ItemType = (ItemModelType) this.itemModels.Count;
      this.ReplaceItemCore(item, ref data);
      this.newItemModelDataXML.Add(data);
      this.itemModels.Add(item.ItemType);
    }

    private void ReplaceItem(int index, ModItemModelDataXML item)
    {
      ItemModelDataXML data = Globals1.ItemModelData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.ItemModelData[index] = data;
    }

    private void ReplaceItemCore(ModItemModelDataXML item, ref ItemModelDataXML data)
    {
      if (item.Scale.HasValue)
        data.Scale = item.Scale.Value;
      if (item.HandOffset.HasValue)
        data.HandOffset = item.HandOffset.Value;
      if (item.HandYPR.HasValue)
        data.HandYPR = item.HandYPR.Value;
      if (item.HUDScale.HasValue)
        data.HUDScale = item.HUDScale.Value;
      if (!item.HUDOffset.HasValue)
        return;
      data.HUDOffset = item.HUDOffset.Value;
    }

    private void ExpandItemModelData()
    {
      int length = Globals1.ItemModelData.Length;
      ItemModelDataXML[] array = new ItemModelDataXML[length + this.newItemModelDataXML.Count];
      Globals1.ItemModelData.CopyTo((Array) array, 0);
      this.newItemModelDataXML.CopyTo(array, length);
      Globals1.ItemModelData = array;
    }

    private string LoadItemSoundData()
    {
      try
      {
        string path = this.path + "ItemSoundData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModItemSoundDataXML itemSoundDataXml in FileSystem.Deserialize<ModItemSoundDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(itemSoundDataXml.ItemID);
            if (index >= 0)
              this.ReplaceItem(index, itemSoundDataXml);
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ItemSoundData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void ReplaceItem(int index, ModItemSoundDataXML item)
    {
      ItemSoundDataXML itemSoundDataXml = Globals1.ItemSoundData[index];
      if (item.Group.HasValue)
        itemSoundDataXml.Group = item.Group.Value;
      if (item.Sounds.HasValue)
        itemSoundDataXml.Sounds = item.Sounds.Value;
      Globals1.ItemSoundData[index] = itemSoundDataXml;
    }

    private string LoadSkillData()
    {
      try
      {
        string path = this.path + "SkillData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModSkillDataXML modSkillDataXml in FileSystem.Deserialize<ModSkillDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(modSkillDataXml.ItemID);
            if (index >= 0)
              this.ReplaceItem(index, modSkillDataXml);
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "SkillData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void ReplaceItem(int index, ModSkillDataXML item)
    {
      SkillDataXML skillDataXml = Globals1.SkillData[index];
      if (item.MineReq.HasValue)
        skillDataXml.MineReq = item.MineReq.Value;
      if (item.UseReq.HasValue)
        skillDataXml.UseReq = item.UseReq.Value;
      if (item.UseSkill.HasValue)
        skillDataXml.UseSkill = item.UseSkill.Value;
      if (item.CraftReq.HasValue)
        skillDataXml.CraftReq = item.CraftReq.Value;
      if (item.CraftSkill.HasValue)
        skillDataXml.CraftSkill = item.CraftSkill.Value;
      Globals1.SkillData[index] = skillDataXml;
    }

    private string LoadBlueprintData()
    {
      try
      {
        string path = this.path + "BlueprintData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModBlueprintDataXML blueprintDataXml in FileSystem.Deserialize<ModBlueprintDataXML[]>(path))
          {
            int index = this.itemIDs.IndexOf(blueprintDataXml.ItemID);
            if (index >= 0)
            {
              BlueprintXML blueprintData = Globals2.GetBlueprintData((Item) index);
              if (blueprintData == null)
                this.AddBlueprint(index, blueprintDataXml);
              else
                this.ReplaceItem(blueprintData, blueprintDataXml);
            }
          }
          if (this.newBlueprintDataXML != null && this.newBlueprintDataXML.Count > 0)
            this.ExpandBlueprintDataFiles();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "BlueprintData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddBlueprint(int index, ModBlueprintDataXML item)
    {
      if (this.newBlueprintDataXML == null)
        this.newBlueprintDataXML = new List<BlueprintXML>();
      BlueprintXML data = new BlueprintXML();
      data.CraftType = BlueprintCraftType.Crafting;
      data.Depth = new Vector2(0.0f, 1f);
      data.IsValid = true;
      this.ReplaceItem(data, item);
      this.newBlueprintDataXML.Add(data);
    }

    private void ReplaceItem(BlueprintXML data, ModBlueprintDataXML item)
    {
      if (item.CraftType.HasValue)
        data.CraftType = item.CraftType.Value;
      if (item.IsValid.HasValue)
        data.IsValid = item.IsValid.Value;
      if (item.IsDefault.HasValue)
        data.IsDefault = item.IsDefault.Value;
      if (item.Depth.HasValue)
        data.Depth = item.Depth.Value;
      if (item.Result.HasValue)
        data.Result = this.ConvertFromModData(item.Result.Value);
      data.Material11 = item.Material11.HasValue ? this.ConvertFromModData(item.Material11.Value) : new InventoryItemXML();
      data.Material12 = item.Material12.HasValue ? this.ConvertFromModData(item.Material12.Value) : new InventoryItemXML();
      data.Material13 = item.Material13.HasValue ? this.ConvertFromModData(item.Material13.Value) : new InventoryItemXML();
      data.Material21 = item.Material21.HasValue ? this.ConvertFromModData(item.Material21.Value) : new InventoryItemXML();
      data.Material22 = item.Material22.HasValue ? this.ConvertFromModData(item.Material22.Value) : new InventoryItemXML();
      data.Material23 = item.Material23.HasValue ? this.ConvertFromModData(item.Material23.Value) : new InventoryItemXML();
      data.Material31 = item.Material31.HasValue ? this.ConvertFromModData(item.Material31.Value) : new InventoryItemXML();
      data.Material32 = item.Material32.HasValue ? this.ConvertFromModData(item.Material32.Value) : new InventoryItemXML();
      data.Material33 = item.Material33.HasValue ? this.ConvertFromModData(item.Material33.Value) : new InventoryItemXML();
    }

    private InventoryItemXML ConvertFromModData(ModInventoryItemXML mod)
    {
      return new InventoryItemXML()
      {
        ItemID = (Item) this.itemIDs.IndexOf(mod.ItemID),
        Durability = mod.Durability,
        Count = mod.Count
      };
    }

    private InventoryItemNDXML ConvertFromModData(ModInventoryItemNDXML mod)
    {
      return new InventoryItemNDXML()
      {
        ItemID = (Item) this.itemIDs.IndexOf(mod.ItemID),
        Count = mod.Count
      };
    }

    private void ExpandBlueprintDataFiles()
    {
      int length1 = Globals1.BlueprintData.Length;
      int length2 = length1 + this.newBlueprintDataXML.Count;
      BlueprintXML[] array = new BlueprintXML[length2];
      Globals1.BlueprintData.CopyTo((Array) array, 0);
      int num = length1;
      while (num < length2)
        ++num;
      this.newBlueprintDataXML.CopyTo(array, Globals1.BlueprintData.Length);
      Globals1.BlueprintData = array;
    }

    private string LoadParticleData()
    {
      try
      {
        string path = this.path + "ParticleData.xml";
        if (FileSystem.IsFileExist(path))
        {
          ParticleData[] particleDataArray = FileSystem.Deserialize<ParticleData[]>(path);
          Globals2.CustomParticleData.AddRange((IEnumerable<ParticleData>) particleDataArray);
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ParticleData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private string LoadActorTypeData()
    {
      try
      {
        string path = this.path + "ActorTypeData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModActorTypeDataXML actorTypeDataXml in FileSystem.Deserialize<ModActorTypeDataXML[]>(path))
          {
            int index = this.actorTypes.IndexOf(actorTypeDataXml.ActorType);
            if (index < 0)
              this.AddNewItem(actorTypeDataXml);
            else
              this.ReplaceItem(index, actorTypeDataXml);
          }
          if (this.newActorTypeDataXML != null && this.newActorTypeDataXML.Count > 0)
            this.ExpandActorTypeData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ActorTypeData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModActorTypeDataXML item)
    {
      if (this.newActorTypeDataXML == null)
        this.newActorTypeDataXML = new List<ActorTypeDataXML>();
      ActorTypeDataXML data = new ActorTypeDataXML();
      data.ActorType = (ActorType) this.actorTypes.Count;
      data.IDString = item.ActorType;
      data.LevelType = ActorLevelType.Base;
      data.PhysicsType = ActorPhysicsType.Base;
      data.AIType = ActorAIType.Base;
      data.ModelHeight = 1.8f;
      data.IsValid = true;
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[29];
      data.ExplodeBlocksRatio = actorTypeDataXml.ExplodeBlocksRatio;
      data.ExplodeBlocksScale = actorTypeDataXml.ExplodeBlocksScale;
      data.EyeOffset = actorTypeDataXml.EyeOffset;
      data.StrikePointOffset = actorTypeDataXml.StrikePointOffset;
      data.ItemModelOffset = actorTypeDataXml.ItemModelOffset;
      data.BoxOffset = actorTypeDataXml.BoxOffset;
      data.BoxOffsetCrouch = actorTypeDataXml.BoxOffsetCrouch;
      data.CriticalBoxOffset = actorTypeDataXml.CriticalBoxOffset;
      data.CriticalBoxOffsetCrouch = actorTypeDataXml.CriticalBoxOffsetCrouch;
      data.BoxScale = actorTypeDataXml.BoxScale;
      data.BoxScaleCrouch = actorTypeDataXml.BoxScaleCrouch;
      data.CriticalBoxScale = actorTypeDataXml.CriticalBoxScale;
      data.CriticalBoxScaleCrouch = actorTypeDataXml.CriticalBoxScaleCrouch;
      this.ReplaceItemCore(item, data);
      this.newActorTypeDataXML.Add(data);
      this.actorTypes.Add(item.ActorType);
    }

    private void ReplaceItem(int index, ModActorTypeDataXML item)
    {
      ActorTypeDataXML data = Globals1.NpcTypeData[index];
      this.ReplaceItemCore(item, data);
    }

    private void ReplaceItemCore(ModActorTypeDataXML item, ActorTypeDataXML data)
    {
      if (item.LevelType.IsNotEmpty())
        data.LevelType = (ActorLevelType) this.actorLevels.IndexOf(item.LevelType);
      if (item.PhysicsType.IsNotEmpty())
        data.PhysicsType = (ActorPhysicsType) this.actorPhysics.IndexOf(item.PhysicsType);
      if (item.AIType.IsNotEmpty())
        data.AIType = (ActorAIType) this.actorAIs.IndexOf(item.AIType);
      if (item.ComName.IsNotEmpty())
      {
        data.ComName = item.ComName;
        data.ComModID = (int) this.modID;
      }
      if (item.ComNameWalk != null && item.ComNameWalk.Length > 0)
      {
        data.ComNameWalk = new string[item.ComNameWalk.Length];
        item.ComNameWalk.CopyTo((Array) data.ComNameWalk, 0);
      }
      if (item.ModelHeight.HasValue)
        data.ModelHeight = item.ModelHeight.Value;
      if (item.ModelYRotation.HasValue)
        data.ModelYRotation = item.ModelYRotation.Value;
      if (item.IsValid.HasValue)
        data.IsValid = item.IsValid.Value;
      if (item.IsFemale.HasValue)
        data.IsFemale = item.IsFemale.Value;
      if (item.IsPassive.HasValue)
        data.IsPassive = item.IsPassive.Value;
      if (item.IsImmuneToFire.HasValue)
        data.IsImmuneToFire = item.IsImmuneToFire.Value;
      if (item.CanBreatheUnderWater.HasValue)
        data.CanBreatheUnderWater = item.CanBreatheUnderWater.Value;
      if (item.ShowHitBoxes.HasValue)
        data.ShowHitBoxes = item.ShowHitBoxes.Value;
      if (item.HasNameplate.HasValue)
        data.HasNameplate = item.HasNameplate.Value;
      if (item.HandMaxHit.HasValue)
        data.HandMaxHit = item.HandMaxHit.Value;
      if (item.NaturalSpawnFreq.HasValue)
        data.NaturalSpawnFreq = item.NaturalSpawnFreq.Value;
      if (item.NaturalBehaviour.IsNotEmpty())
        data.NaturalBehaviour = item.NaturalBehaviour;
      if (item.LootTable == null)
        return;
      data.LootTable = item.LootTable;
    }

    private void ExpandActorTypeData()
    {
      int length1 = Globals1.NpcTypeData.Length;
      int length2 = length1 + this.newActorTypeDataXML.Count;
      ActorTypeDataXML[] array = new ActorTypeDataXML[length2];
      ActorAudioDataXML[] actorAudioDataXmlArray = new ActorAudioDataXML[length2];
      Globals1.NpcTypeData.CopyTo((Array) array, 0);
      Globals1.NpcAudioData.CopyTo((Array) actorAudioDataXmlArray, 0);
      for (int index = length1; index < length2; ++index)
        actorAudioDataXmlArray[index] = Globals1.NpcAudioData[29];
      this.newActorTypeDataXML.CopyTo(array, length1);
      Globals1.NpcTypeData = array;
      Globals1.NpcAudioData = actorAudioDataXmlArray;
    }

    private string LoadActorLevelData()
    {
      try
      {
        string path = this.path + "ActorLevelData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModActorLevelDataXML actorLevelDataXml in FileSystem.Deserialize<ModActorLevelDataXML[]>(path))
          {
            int index = this.actorLevels.IndexOf(actorLevelDataXml.ActorLevelType);
            if (index < 0)
              this.AddNewItem(actorLevelDataXml);
            else
              this.ReplaceItem(index, actorLevelDataXml);
          }
          if (this.newActorLevelDataXML != null && this.newActorLevelDataXML.Count > 0)
            this.ExpandActorLevelData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ActorLevelData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModActorLevelDataXML item)
    {
      if (this.newActorLevelDataXML == null)
        this.newActorLevelDataXML = new List<ActorLevelDataXML>();
      ActorLevelDataXML data = Globals1.NpcLevelData[16];
      data.ActorLevelType = (ActorLevelType) this.actorLevels.Count;
      this.ReplaceItemCore(item, ref data);
      this.newActorLevelDataXML.Add(data);
      this.actorLevels.Add(item.ActorLevelType);
    }

    private void ReplaceItem(int index, ModActorLevelDataXML item)
    {
      ActorLevelDataXML data = Globals1.NpcLevelData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.NpcLevelData[index] = data;
    }

    private void ReplaceItemCore(ModActorLevelDataXML item, ref ActorLevelDataXML data)
    {
      if (item.HealthLevel.HasValue)
        data.HealthLevel = item.HealthLevel.Value;
      if (item.AttackLevel.HasValue)
        data.AttackLevel = item.AttackLevel.Value;
      if (item.StrengthLevel.HasValue)
        data.StrengthLevel = item.StrengthLevel.Value;
      if (item.DefenceLevel.HasValue)
        data.DefenceLevel = item.DefenceLevel.Value;
      if (!item.RangedLevel.HasValue)
        return;
      data.RangedLevel = item.RangedLevel.Value;
    }

    private void ExpandActorLevelData()
    {
      int length = Globals1.NpcLevelData.Length;
      ActorLevelDataXML[] array = new ActorLevelDataXML[length + this.newActorLevelDataXML.Count];
      Globals1.NpcLevelData.CopyTo((Array) array, 0);
      this.newActorLevelDataXML.CopyTo(array, length);
      Globals1.NpcLevelData = array;
    }

    private string LoadActorPhysicsData()
    {
      try
      {
        string path = this.path + "ActorPhysicsData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModActorPhysicsDataXML actorPhysicsDataXml in FileSystem.Deserialize<ModActorPhysicsDataXML[]>(path))
          {
            int index = this.actorPhysics.IndexOf(actorPhysicsDataXml.ActorPhysicsType);
            if (index < 0)
              this.AddNewItem(actorPhysicsDataXml);
            else
              this.ReplaceItem(index, actorPhysicsDataXml);
          }
          if (this.newActorPhysicsDataXML != null && this.newActorPhysicsDataXML.Count > 0)
            this.ExpandActorPhysicsData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ActorPhysicsData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModActorPhysicsDataXML item)
    {
      if (this.newActorPhysicsDataXML == null)
        this.newActorPhysicsDataXML = new List<ActorPhysicsDataXML>();
      ActorPhysicsDataXML data = Globals1.NpcPhysicsData[16];
      data.ActorPhysicsType = (ActorPhysicsType) this.actorPhysics.Count;
      this.ReplaceItemCore(item, ref data);
      this.newActorPhysicsDataXML.Add(data);
      this.actorPhysics.Add(item.ActorPhysicsType);
    }

    private void ReplaceItem(int index, ModActorPhysicsDataXML item)
    {
      ActorPhysicsDataXML data = Globals1.NpcPhysicsData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.NpcPhysicsData[index] = data;
    }

    private void ReplaceItemCore(ModActorPhysicsDataXML item, ref ActorPhysicsDataXML data)
    {
      if (item.Acceleration.HasValue)
        data.Acceleration = item.Acceleration.Value;
      if (item.MoveSpeed.HasValue)
        data.MoveSpeed = item.MoveSpeed.Value;
      if (item.JumpSpeed.HasValue)
        data.JumpSpeed = item.JumpSpeed.Value;
      if (!item.RotateSpeed.HasValue)
        return;
      data.RotateSpeed = item.RotateSpeed.Value;
    }

    private void ExpandActorPhysicsData()
    {
      int length = Globals1.NpcPhysicsData.Length;
      ActorPhysicsDataXML[] array = new ActorPhysicsDataXML[length + this.newActorPhysicsDataXML.Count];
      Globals1.NpcPhysicsData.CopyTo((Array) array, 0);
      this.newActorPhysicsDataXML.CopyTo(array, length);
      Globals1.NpcPhysicsData = array;
    }

    private string LoadActorAIData()
    {
      try
      {
        string path = this.path + "ActorAIData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModActorAIDataXML modActorAiDataXml in FileSystem.Deserialize<ModActorAIDataXML[]>(path))
          {
            int index = this.actorAIs.IndexOf(modActorAiDataXml.ActorAIType);
            if (index < 0)
              this.AddNewItem(modActorAiDataXml);
            else
              this.ReplaceItem(index, modActorAiDataXml);
          }
          if (this.newActorAIDataXML != null && this.newActorAIDataXML.Count > 0)
            this.ExpandActorAIData();
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ActorAIData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void AddNewItem(ModActorAIDataXML item)
    {
      if (this.newActorAIDataXML == null)
        this.newActorAIDataXML = new List<ActorAIDataXML>();
      ActorAIDataXML data = Globals1.NpcAIData[15];
      data.ActorAIType = (ActorAIType) this.actorAIs.Count;
      this.ReplaceItemCore(item, ref data);
      this.newActorAIDataXML.Add(data);
      this.actorAIs.Add(item.ActorAIType);
    }

    private void ReplaceItem(int index, ModActorAIDataXML item)
    {
      ActorAIDataXML data = Globals1.NpcAIData[index];
      this.ReplaceItemCore(item, ref data);
      Globals1.NpcAIData[index] = data;
    }

    private void ReplaceItemCore(ModActorAIDataXML item, ref ActorAIDataXML data)
    {
      if (item.StrikeDelay.HasValue)
        data.StrikeDelay = item.StrikeDelay.Value;
      if (item.StrikeRange.HasValue)
        data.StrikeRange = item.StrikeRange.Value;
      if (item.RegardRange.HasValue)
        data.RegardRange = item.RegardRange.Value;
      if (item.HearingRange.HasValue)
        data.HearingRange = item.HearingRange.Value;
      if (item.AttackRange.HasValue)
        data.AttackRange = item.AttackRange.Value;
      if (!item.InactiveRange.HasValue)
        return;
      data.InactiveRange = item.InactiveRange.Value;
    }

    private void ExpandActorAIData()
    {
      int length = Globals1.NpcAIData.Length;
      ActorAIDataXML[] array = new ActorAIDataXML[length + this.newActorAIDataXML.Count];
      Globals1.NpcAIData.CopyTo((Array) array, 0);
      this.newActorAIDataXML.CopyTo(array, length);
      Globals1.NpcAIData = array;
    }

    private string LoadActorAudioData()
    {
      try
      {
        string path = this.path + "ActorAudioData.xml";
        if (FileSystem.IsFileExist(path))
        {
          foreach (ModActorAudioDataXML actorAudioDataXml in FileSystem.Deserialize<ModActorAudioDataXML[]>(path))
          {
            int index = this.actorTypes.IndexOf(actorAudioDataXml.ActorType);
            if (index >= 0)
              this.ReplaceItem(index, actorAudioDataXml);
          }
        }
        return (string) null;
      }
      catch (Exception ex)
      {
        string str = "ActorAudioData.xml: " + ex.Message;
        if (ex.InnerException != null)
          str = str + "\n" + ex.InnerException.Message;
        return str;
      }
    }

    private void ReplaceItem(int index, ModActorAudioDataXML item)
    {
      ActorAudioDataXML actorAudioDataXml = Globals1.NpcAudioData[index];
      item.AudioPain.IsNotEmpty();
      item.AudioStrike.IsNotEmpty();
      item.AudioWarning.IsNotEmpty();
      item.AudioDeath.IsNotEmpty();
    }

    public Item[] LoadTexturePackBlocks(int size)
    {
      return this.LoadTexturePackItemsCore(this.path + "BlockTextures" + size.ToString() + ".xml");
    }

    public Item[] LoadTexturePackItems(int size)
    {
      return this.LoadTexturePackItemsCore(this.path + "ItemTextures" + size.ToString() + ".xml");
    }

    private Item[] LoadTexturePackItemsCore(string filename)
    {
      try
      {
        if (!FileSystem.IsFileExist(filename))
          return (Item[]) null;
        ItemXML[] itemXmlArray = FileSystem.Deserialize<ItemXML[]>(filename);
        Item[] objArray = new Item[itemXmlArray.Length];
        for (int index = 0; index < itemXmlArray.Length; ++index)
        {
          int num = this.itemIDs.IndexOf(itemXmlArray[index].ItemID);
          objArray[index] = (Item) num;
        }
        return objArray;
      }
      catch (Exception ex)
      {
        return (Item[]) null;
      }
    }

    private string LoadBehavioursAndDialog()
    {
      Globals1.ImportBehaviourTrees(this.path + "behaviour.db");
      Globals1.ImportBehaviourTrees(this.path + "dialog.db");
      return (string) null;
    }

    private static string BuildErrString(List<string> err)
    {
      string str1 = (string) null;
      foreach (string str2 in err)
      {
        if (str2 != null)
        {
          if (str1 != null)
            str1 += (string) (object) '\n';
          str1 += str2;
        }
      }
      return str1;
    }
  }
}
