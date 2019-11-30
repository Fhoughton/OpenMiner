// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Globals1
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace StudioForge.TotalMiner
{
  public static class Globals1
  {
    public static NumberStyles NumberStyle = NumberStyles.Number;
    public static CultureInfo CultureInfo = new CultureInfo("en-US", false);
    public static long StopWatchFreq = Stopwatch.Frequency;
    public static long StopWatchFreqAlt = Stopwatch.Frequency;
    public static Stopwatch ElapsedWatch = new Stopwatch();
    public static object SaveSemaphore = new object();
    public static float StickDeadzone = 0.2f;
    public const int SaveVersion = 294;
    public static SpriteFont FontConsolas;
    public static BlockDataXML[] BlockData;
    public static BlockMaterialDataXML[] BlockMaterialData;
    public static ItemDataXML[] ItemData;
    public static ItemTypeDataXML[] ItemTypeData;
    public static ItemTypeClassDataXML[] ItemTypeClassData;
    public static ItemSwingDataXML[] ItemSwingData;
    public static ItemSwingTimeDataXML[] ItemSwingTimeData;
    public static ItemModelDataXML[] ItemModelData;
    public static ItemCombatDataXML[] ItemCombatData;
    public static ItemSoundGroupXML[] ItemSoundGroups;
    public static ItemSoundDataXML[] ItemSoundData;
    public static AmbientSoundXML[] AmbientSoundData;
    public static RareDataXML[] RareData;
    public static SkillDataXML[] SkillData;
    public static SkillBonusXML[] SkillBonusData;
    public static ActorTypeDataXML[] NpcTypeData;
    public static ActorLevelDataXML[] NpcLevelData;
    public static ActorPhysicsDataXML[] NpcPhysicsData;
    public static ActorAIDataXML[] NpcAIData;
    public static ActorAudioDataXML[] NpcAudioData;
    public static BlueprintXML[] BlueprintData;
    public static List<BehaviourTree> BehaviourTrees;

    public static void Initialize()
    {
      Globals1.CultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
      Globals1.CultureInfo.NumberFormat.NumberDecimalSeparator = ".";
      Globals1.CultureInfo.NumberFormat.PercentDecimalSeparator = ".";
      Globals1.CultureInfo.NumberFormat.NegativeSign = "-";
      Globals1.CultureInfo.NumberFormat.PositiveSign = "+";
      Globals1.ElapsedWatch.Reset();
      Globals1.ElapsedWatch.Start();
      Globals1.ItemSoundGroups = Utils.Deserialize1<ItemSoundGroupXML[]>("Content\\Map\\ItemSoundGroups.xml");
      Globals1.AmbientSoundData = Utils.Deserialize1<AmbientSoundXML[]>("Content\\Map\\AmbientSoundData.xml");
      Globals1.RareData = Utils.Deserialize1<RareDataXML[]>("Content\\Map\\RareData.xml");
      Globals1.SkillBonusData = Utils.Deserialize1<SkillBonusXML[]>("Content\\Map\\SkillBonusData.xml");
    }

    public static void Reinitialize()
    {
      Globals1.Reinitialize("");
    }

    public static void Reinitialize(string path)
    {
      Globals1.BlockData = Utils.Deserialize1<BlockDataXML[]>(path + "Content\\Map\\BlockData.xml");
      Globals1.BlockMaterialData = Utils.Deserialize1<BlockMaterialDataXML[]>(path + "Content\\Map\\BlockMaterialData.xml");
      Globals1.ItemData = Utils.Deserialize1<ItemDataXML[]>(path + "Content\\Map\\ItemData.xml");
      Globals1.ItemTypeData = Utils.Deserialize1<ItemTypeDataXML[]>(path + "Content\\Map\\ItemTypeData.xml");
      Globals1.ItemTypeClassData = Utils.Deserialize1<ItemTypeClassDataXML[]>(path + "Content\\Map\\ItemTypeClassData.xml");
      Globals1.ItemCombatData = Utils.Deserialize1<ItemCombatDataXML[]>(path + "Content\\Map\\ItemCombatData.xml");
      Globals1.ItemSwingData = Utils.Deserialize1<ItemSwingDataXML[]>(path + "Content\\Map\\ItemSwingData.xml");
      Globals1.ItemSwingTimeData = Utils.Deserialize1<ItemSwingTimeDataXML[]>(path + "Content\\Map\\ItemSwingTimeData.xml");
      Globals1.ItemModelData = Utils.Deserialize1<ItemModelDataXML[]>(path + "Content\\Map\\ItemModelData.xml");
      Globals1.ItemSoundData = Utils.Deserialize1<ItemSoundDataXML[]>(path + "Content\\Map\\ItemSoundData.xml");
      Globals1.SkillData = Utils.Deserialize1<SkillDataXML[]>(path + "Content\\Map\\SkillData.xml");
      Globals1.BlueprintData = Utils.Deserialize1<BlueprintXML[]>(path + "Content\\Map\\BlueprintData.xml");
      Globals1.NpcTypeData = Utils.Deserialize1<ActorTypeDataXML[]>(path + "Content\\Map\\MobTypeData.xml");
      Globals1.NpcLevelData = Utils.Deserialize1<ActorLevelDataXML[]>(path + "Content\\Map\\MobLevelData.xml");
      Globals1.NpcPhysicsData = Utils.Deserialize1<ActorPhysicsDataXML[]>(path + "Content\\Map\\MobPhysicsData.xml");
      Globals1.NpcAIData = Utils.Deserialize1<ActorAIDataXML[]>(path + "Content\\Map\\MobAIData.xml");
      Globals1.NpcAudioData = Utils.Deserialize1<ActorAudioDataXML[]>(path + "Content\\Map\\MobAudioData.xml");
      foreach (ActorTypeDataXML actorTypeDataXml in Globals1.NpcTypeData)
      {
        if (actorTypeDataXml.IDString == null)
          actorTypeDataXml.IDString = actorTypeDataXml.ActorType.ToString();
        if (actorTypeDataXml.ComName == null || actorTypeDataXml.ComName.Length < 1)
          actorTypeDataXml.ComName = actorTypeDataXml.ActorType.ToString();
      }
    }

    public static List<string> ReadStringList(BinaryReader reader)
    {
      int capacity = reader.ReadInt32();
      List<string> stringList = new List<string>(capacity);
      for (int index = 0; index < capacity; ++index)
        stringList.Add(reader.ReadString());
      return stringList;
    }

    public static List<bool> ReadBoolList(BinaryReader reader)
    {
      List<bool> boolList = new List<bool>();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        boolList.Add(reader.ReadBoolean());
      return boolList;
    }

    public static List<GlobalPoint3D> ReadGlobalPoint3DList(BinaryReader reader)
    {
      List<GlobalPoint3D> globalPoint3DList = new List<GlobalPoint3D>();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        GlobalPoint3D globalPoint3D;
        globalPoint3D.X = (int) reader.ReadInt16();
        globalPoint3D.Y = (int) reader.ReadInt16();
        globalPoint3D.Z = (int) reader.ReadInt16();
        globalPoint3DList.Add(globalPoint3D);
      }
      return globalPoint3DList;
    }

    public static void ReadRandBuffer(BinaryReader reader)
    {
      byte num1 = reader.ReadByte();
      for (int index = 0; index < (int) num1; ++index)
      {
        int num2 = (int) reader.ReadByte();
      }
    }

    public static void WriteRandBuffer(BinaryWriter writer, int max)
    {
      Random random = new Random();
      byte num = (byte) random.Next(max);
      writer.Write(num);
      for (int index = 0; index < (int) num; ++index)
        writer.Write((byte) random.Next((int) byte.MaxValue));
    }

    public static void WriteStringList(BinaryWriter writer, List<string> list)
    {
      if (list == null)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(list.Count);
        for (int index = 0; index < list.Count; ++index)
          writer.Write(list[index]);
      }
    }

    public static void WriteBoolArray(BinaryWriter writer, bool[] array)
    {
      if (array == null)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(array.Length);
        foreach (bool flag in array)
          writer.Write(flag);
      }
    }

    public static void WriteGlobalPoint3DList(BinaryWriter writer, List<GlobalPoint3D> list)
    {
      if (list == null)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(list.Count);
        foreach (GlobalPoint3D globalPoint3D in list)
        {
          writer.Write((short) globalPoint3D.X);
          writer.Write((short) globalPoint3D.Y);
          writer.Write((short) globalPoint3D.Z);
        }
      }
    }

    public static void Breakpoint()
    {
    }

    public static bool IsCuePlaying(Cue cue)
    {
      if (cue != null && !cue.IsStopped && !cue.IsStopping)
        return !cue.IsDisposed;
      return false;
    }

    public static bool IsLocked(int i, GameMode mode)
    {
      if (i >= 0 && i < Globals1.ItemData.Length)
      {
        switch (mode)
        {
          case GameMode.DigDeep:
            return Globals1.ItemData[i].LockedDD;
          case GameMode.Creative:
            return Globals1.ItemData[i].LockedCR;
          case GameMode.Survival:
          case GameMode.Peaceful:
            return Globals1.ItemData[i].LockedSU;
        }
      }
      return false;
    }

    public static BehaviourTree GetBehaviour(BehaviourTreeType type, string name)
    {
      foreach (BehaviourTree behaviourTree in Globals1.BehaviourTrees)
      {
        if (behaviourTree.TreeType == type && behaviourTree.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return behaviourTree;
      }
      return (BehaviourTree) null;
    }

    public static void LoadBehaviourTrees()
    {
      Globals1.BehaviourTrees = Globals1.LoadBehaviourTreesCore();
    }

    private static List<BehaviourTree> LoadBehaviourTreesCore()
    {
      List<BehaviourTree> behaviourTreeList = new List<BehaviourTree>();
      try
      {
        string path1 = "BehaviourTrees.db";
        if (FileSystem.IsFileExist(path1))
        {
          using (Stream input = FileSystem.OpenRead(path1))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              int num = reader.ReadInt32();
              for (int index = 0; index < num; ++index)
              {
                BehaviourTree behaviourTree = new BehaviourTree(BehaviourTreeType.None, false);
                behaviourTree.ReadState(reader, version);
                if (behaviourTree.Name.IsNotEmpty())
                  behaviourTreeList.Add(behaviourTree);
              }
            }
          }
        }
        string path2 = "Content\\Map\\BehaviourTreesSys.db";
        if (TitleFileSystem.IsFileExist(path2))
        {
          using (Stream input = TitleFileSystem.OpenFile(path2, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              int num = reader.ReadInt32();
              for (int index = 0; index < num; ++index)
              {
                BehaviourTree behaviourTree = new BehaviourTree(BehaviourTreeType.None, true);
                behaviourTree.ReadState(reader, version);
                if (behaviourTree.Name.IsNotEmpty())
                  behaviourTreeList.Add(behaviourTree);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return behaviourTreeList;
    }

    public static void SaveBehaviourTrees()
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = FileSystem.OpenWrite("BehaviourTrees.db"))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            long position = output.Position;
            int num = 0;
            writer.Write(num);
            foreach (BehaviourTree behaviourTree in Globals1.BehaviourTrees)
            {
              if (!behaviourTree.Immutable && behaviourTree.Name.StartsWith("Global\\", StringComparison.OrdinalIgnoreCase))
              {
                behaviourTree.WriteState(writer);
                ++num;
              }
            }
            output.Position = position;
            writer.Write(num);
          }
        }
      }
    }

    public static void ExportBehaviourTrees(BehaviourTreeType type, string filename, string path)
    {
      lock (Globals1.SaveSemaphore)
      {
        using (FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write))
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) fileStream))
          {
            writer.Write(294);
            if (path.IsEmpty())
              path = (string) null;
            long position = fileStream.Position;
            int num = 0;
            writer.Write(num);
            foreach (BehaviourTree behaviourTree in Globals1.BehaviourTrees)
            {
              if ((type == BehaviourTreeType.None || behaviourTree.TreeType == type) && !behaviourTree.Immutable && (path == null || behaviourTree.Name.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
              {
                behaviourTree.WriteState(writer);
                ++num;
              }
            }
            fileStream.Position = position;
            writer.Write(num);
          }
        }
      }
    }

    public static void ImportBehaviourTrees(string filename)
    {
      if (!FileSystem.IsFileExist(filename))
        return;
      using (Stream input = FileSystem.OpenRead(filename))
      {
        using (BinaryReader reader = new BinaryReader(input))
        {
          int version = reader.ReadInt32();
          int num = reader.ReadInt32();
          for (int index = 0; index < num; ++index)
          {
            BehaviourTree behaviourTree = new BehaviourTree(BehaviourTreeType.None, true);
            behaviourTree.ReadState(reader, version);
            if (behaviourTree.Name.IsNotEmpty())
              Globals1.BehaviourTrees.Add(behaviourTree);
          }
        }
      }
    }

    public static void DeleteBehaviourTree(BehaviourTreeType type, string name)
    {
      for (int index = Globals1.BehaviourTrees.Count - 1; index >= 0; --index)
      {
        BehaviourTree behaviourTree = Globals1.BehaviourTrees[index];
        if (behaviourTree.TreeType == type && behaviourTree.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          Globals1.BehaviourTrees.RemoveAt(index);
          break;
        }
      }
    }

    public static void RemoveNonGlobalBehaviourTrees()
    {
      if (Globals1.BehaviourTrees == null)
        return;
      for (int index = Globals1.BehaviourTrees.Count - 1; index >= 0; --index)
      {
        BehaviourTree behaviourTree = Globals1.BehaviourTrees[index];
        if (!behaviourTree.Immutable && !behaviourTree.Name.StartsWith("Global\\", StringComparison.OrdinalIgnoreCase))
          Globals1.BehaviourTrees.RemoveAt(index);
      }
    }

    public static string GetFullExceptionMessageForDisplay(Exception e)
    {
      string str = Globals1.GetExceptionMessageForDisplay(e);
      if (e.InnerException != null)
        str = str + "\n" + Globals1.GetExceptionMessageForDisplay(e.InnerException);
      return str;
    }

    public static string GetExceptionMessageForDisplay(Exception e)
    {
      string str = e.Message.Replace(". ", ".\n");
      if (e.TargetSite != (MethodBase) null)
        str = str + "\n" + e.TargetSite.Name;
      return str;
    }
  }
}
