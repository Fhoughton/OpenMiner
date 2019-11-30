// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ModManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace StudioForge.TotalMiner
{
  internal static class ModManager
  {
    public static string ModsPath = "Mods\\";
    public static List<Mod> ActiveMods = new List<Mod>();
    public static List<Mod> ActivePlugins = new List<Mod>();
    public static Mod NetMod;
    public static string NetModName;
    private static EnumTypeOffsets enumOffsets;
    private static ushort[] blockIDMods;
    private static ITMPluginManager imgr;

    public static ITMPluginManager InterfaceManager
    {
      get
      {
        return ModManager.imgr ?? (ModManager.imgr = (ITMPluginManager) new ModManager.ModManagerInterface());
      }
    }

    public static bool IsActiveMod(string name)
    {
      return ModManager.GetActiveMod(name) != null;
    }

    public static Mod GetActiveMod(string name)
    {
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return activeMod;
      }
      return (Mod) null;
    }

    public static string GetPath(int modID)
    {
      if (modID < 1 || modID > ModManager.ActiveMods.Count)
        return (string) null;
      return "Mods\\" + ModManager.ActiveMods[modID - 1].Name;
    }

    public static ITMPluginBlocks GetPluginBlocks(byte blockID)
    {
      ushort blockIdMod = ModManager.blockIDMods[(int) blockID];
      if (blockIdMod <= (ushort) 0)
        return (ITMPluginBlocks) null;
      return ModManager.ActiveMods[(int) blockIdMod - 1].PluginBlocks;
    }

    public static ITMPluginBlocks GetPluginBlocks(Block blockID)
    {
      ushort blockIdMod = ModManager.blockIDMods[(int) blockID];
      if (blockIdMod <= (ushort) 0)
        return (ITMPluginBlocks) null;
      return ModManager.ActiveMods[(int) blockIdMod - 1].PluginBlocks;
    }

    public static bool Matches(string name, ModFilter modFilter)
    {
      foreach (string file in FileSystem.GetFiles(ModManager.ModsPath + name + (object) '\\', "*.dll"))
      {
        try
        {
          Assembly assembly = Assembly.LoadFile(FileSystem.RootPath + file);
          if (assembly != (Assembly) null)
          {
            ITMPluginProvider pluginProvider = ModManager.GetPluginProvider(assembly);
            if (pluginProvider != null)
            {
              if (modFilter == ModFilter.HasPluginNet)
              {
                if (pluginProvider.GetPluginNet() != null)
                  return true;
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      return false;
    }

    public static ITMPluginProvider GetPluginProvider(Assembly assembly)
    {
      foreach (Type type1 in assembly.GetTypes())
      {
        foreach (Type type2 in type1.GetInterfaces())
        {
          if (type2 == typeof (ITMPluginProvider))
          {
            ITMPluginProvider instance = assembly.CreateInstance(type1.FullName) as ITMPluginProvider;
            if (instance != null)
              return instance;
          }
        }
      }
      return (ITMPluginProvider) null;
    }

    public static void Initialize()
    {
      ModManager.enumOffsets.BlockID = (ushort) Globals1.BlockData.Length;
      ModManager.enumOffsets.ItemID = (ushort) Globals1.ItemData.Length;
      ModManager.enumOffsets.DataBlockType = (ushort) 27;
      ModManager.enumOffsets.ArcadeMachine = (ushort) 3;
      ModManager.enumOffsets.PacketType = (ushort) 0;
    }

    public static Mod LoadMod(string name)
    {
      string errorMessage;
      return ModManager.LoadMod(name, out errorMessage);
    }

    public static Mod LoadMod(string name, out string errorMessage)
    {
      errorMessage = "Mod already active";
      Mod activeMod = ModManager.GetActiveMod(name);
      if (activeMod != null)
        return activeMod;
      errorMessage = (string) null;
      ItemDataXML[] itemData = Globals1.ItemData;
      Mod mod = new Mod((ushort) (ModManager.ActiveMods.Count + 1), name);
      errorMessage = mod.Load();
      if (errorMessage != null)
        return (Mod) null;
      ModManager.enumOffsets.ItemID = (ushort) Globals1.ItemData.Length;
      ModManager.enumOffsets.BlockID = (ushort) Globals1.BlockData.Length;
      ModManager.ActiveMods.Add(mod);
      if (mod.Plugin != null)
        ModManager.ActivePlugins.Add(mod);
      return mod;
    }

    public static void RegisterBlockIDs(Block blockID, Mod mod)
    {
      int index = (int) blockID;
      if (ModManager.blockIDMods == null || ModManager.blockIDMods.Length <= index)
      {
        if (ModManager.blockIDMods == null)
        {
          ModManager.blockIDMods = new ushort[index + 1];
        }
        else
        {
          ushort[] numArray = new ushort[index + 1];
          ModManager.blockIDMods.CopyTo((Array) numArray, 0);
          ModManager.blockIDMods = numArray;
        }
      }
      ModManager.blockIDMods[index] = mod.ModID;
    }

    public static void UnloadAllActiveMods()
    {
      if (ModManager.ActiveMods.Count <= 0)
        return;
      ModManager.ActiveMods.Clear();
      ModManager.ActivePlugins.Clear();
      Globals2.Reinitialize(true);
      GraphicStatics.LoadTexturePack((MapTM) null, GraphicStatics.TexturePack.Name, false, false);
      if (GameInstance.Instance == null)
        return;
      GameInstance.Instance.ModsLoaded();
    }

    public static void UnloadMod(string name)
    {
      Mod activeMod = ModManager.GetActiveMod(name);
      if (activeMod == null)
        return;
      ModManager.ActiveMods.Remove(activeMod);
      ModManager.ActivePlugins.Remove(activeMod);
      if (activeMod.Plugin != null)
        activeMod.Plugin.UnloadMod();
      List<string> modNames = ModManager.GetModNames(ModManager.ActiveMods);
      ModManager.ActiveMods.Clear();
      ModManager.ActivePlugins.Clear();
      Globals2.Reinitialize(true);
      ModManager.StartupActiveMods(modNames);
      GraphicStatics.LoadTexturePack((MapTM) null, GraphicStatics.TexturePack.Name, false, false);
      if (GameInstance.Instance == null)
        return;
      GameInstance.Instance.ModsLoaded();
    }

    public static void HotLoadMods(GameInstance instance)
    {
      if (instance == null)
        return;
      List<string> modNames = ModManager.GetModNames(ModManager.ActiveMods);
      ModManager.ActiveMods.Clear();
      ModManager.ActivePlugins.Clear();
      Globals2.Reinitialize(true);
      ModManager.StartupActiveMods(modNames);
      GraphicStatics.TexturePack.LoadTexturePack();
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.Plugin != null)
          activeMod.Plugin.InitializeGame((ITMGame) instance);
        if (activeMod.PluginBlocks != null)
          activeMod.PluginBlocks.InitializeGame((ITMGame) instance);
      }
    }

    public static List<string> GetModNames(List<Mod> mods)
    {
      List<string> stringList = new List<string>();
      foreach (Mod mod in mods)
        stringList.Add(mod.Name);
      return stringList;
    }

    public static void StartupActiveMods(List<string> modNames)
    {
      try
      {
        ModManager.enumOffsets.BlockID = (ushort) Globals1.BlockData.Length;
        ModManager.enumOffsets.ItemID = (ushort) Globals1.ItemData.Length;
        ModManager.enumOffsets.DataBlockType = (ushort) 0;
        ModManager.enumOffsets.ArcadeMachine = (ushort) 3;
        ModManager.enumOffsets.PacketType = (ushort) 0;
        if (modNames == null)
          return;
        foreach (string modName in modNames)
        {
          string errorMessage;
          if (modName.IsNotEmpty() && ModManager.LoadMod(modName, out errorMessage) == null)
          {
            int num = (int) MessageBox.Show("This mod could not be loaded: " + modName + "\n" + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    private class ModManagerInterface : ITMPluginManager
    {
      EnumTypeOffsets ITMPluginManager.Offsets
      {
        get
        {
          return ModManager.enumOffsets;
        }
      }

      void ITMPluginManager.RegisterEnumCounts(EnumTypeOffsets offsets)
      {
        ModManager.enumOffsets.DataBlockType += offsets.DataBlockType;
        ModManager.enumOffsets.ArcadeMachine += offsets.ArcadeMachine;
        ModManager.enumOffsets.PacketType += offsets.PacketType;
      }
    }
  }
}
