// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TargetingSystem
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public static class TargetingSystem
  {
    private static List<TargetData> tempList = new List<TargetData>();
    private static INPCBehaviour sortTargeted;
    private static NpcQueryPreference sortType;
    private static Dictionary<GamerID, List<TargetData>> targets;

    public static void Initialize()
    {
      if (TargetingSystem.targets == null)
        TargetingSystem.targets = new Dictionary<GamerID, List<TargetData>>();
      else
        TargetingSystem.targets.Clear();
    }

    public static void Update()
    {
      foreach (KeyValuePair<GamerID, List<TargetData>> target in TargetingSystem.targets)
      {
        List<TargetData> targetDataList = target.Value;
        if (targetDataList != null && targetDataList.Count > 0)
        {
          for (int index = targetDataList.Count - 1; index >= 0; --index)
          {
            TargetData targetData = targetDataList[index];
            if (--targetData.Ticks < 1)
              targetDataList.RemoveAt(index);
            else
              targetDataList[index] = targetData;
          }
        }
      }
    }

    public static void Target(INPCBehaviour actor, INPCBehaviour target)
    {
      TargetingSystem.Target(actor, target, 1800, NpcQueryPreference.Source | NpcQueryPreference.Agressive);
    }

    public static void Target(INPCBehaviour actor, INPCBehaviour target, int ticks)
    {
      TargetingSystem.Target(actor, target, ticks, NpcQueryPreference.Source | NpcQueryPreference.Agressive);
    }

    public static void Target(
      INPCBehaviour actor,
      INPCBehaviour target,
      int ticks,
      NpcQueryPreference pref)
    {
      if (actor == null || target == null)
        return;
      List<TargetData> targetDataList;
      if (!TargetingSystem.targets.TryGetValue(target.GamerID, out targetDataList))
      {
        targetDataList = new List<TargetData>();
        targetDataList.Add(new TargetData()
        {
          Targeter = actor,
          Query = pref,
          Ticks = ticks
        });
        TargetingSystem.targets.Add(target.GamerID, targetDataList);
      }
      else
      {
        bool flag = false;
        for (int index = targetDataList.Count - 1; index >= 0; --index)
        {
          TargetData targetData = targetDataList[index];
          if (targetData.Targeter == actor)
          {
            targetData.Query |= pref;
            targetDataList[index] = targetData;
            flag = true;
            break;
          }
        }
        if (flag)
          return;
        targetDataList.Add(new TargetData()
        {
          Targeter = actor,
          Query = pref,
          Ticks = ticks
        });
      }
    }

    public static void TargetEnd(INPCBehaviour actor, INPCBehaviour target)
    {
      List<TargetData> targetDataList;
      if (target == null || !TargetingSystem.targets.TryGetValue(target.GamerID, out targetDataList))
        return;
      if (actor == null)
      {
        targetDataList.Clear();
      }
      else
      {
        for (int index = targetDataList.Count - 1; index >= 0; --index)
        {
          if (targetDataList[index].Targeter == actor)
          {
            targetDataList.RemoveAt(index);
            break;
          }
        }
      }
    }

    public static void TargetInactive(INPCBehaviour target)
    {
      if (target == null)
        return;
      TargetingSystem.targets.Remove(target.GamerID);
      foreach (KeyValuePair<GamerID, List<TargetData>> target1 in TargetingSystem.targets)
      {
        List<TargetData> targetDataList = target1.Value;
        for (int index = targetDataList.Count - 1; index >= 0; --index)
        {
          if (targetDataList[index].Targeter == target)
          {
            targetDataList.RemoveAt(index);
            break;
          }
        }
      }
    }

    public static List<INPCBehaviour> FindTargets(
      NpcQueryPreference preference,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes)
    {
      return (List<INPCBehaviour>) null;
    }

    public static TargetData? GetLastTargetedBy(INPCBehaviour target)
    {
      List<TargetData> targetDataList;
      if (target != null && TargetingSystem.targets.TryGetValue(target.GamerID, out targetDataList) && targetDataList.Count > 0)
        return new TargetData?(targetDataList[targetDataList.Count - 1]);
      return new TargetData?();
    }

    public static List<TargetData> GetTargetedBy(
      INPCBehaviour target,
      NpcQueryPreference preference,
      List<ActorType> searchTypes,
      List<ActorType> excludeTypes)
    {
      List<TargetData> tempList;
      if (target == null || !TargetingSystem.targets.TryGetValue(target.GamerID, out tempList) || tempList.Count <= 0)
        return (List<TargetData>) null;
      bool flag1 = searchTypes != null && searchTypes.Count > 0;
      bool flag2 = excludeTypes != null && excludeTypes.Count > 0;
      TargetingSystem.tempList.Clear();
      TargetingSystem.tempList.AddRange((IEnumerable<TargetData>) tempList);
      for (int index = TargetingSystem.tempList.Count - 1; index >= 0; --index)
      {
        TargetData temp = TargetingSystem.tempList[index];
        INPCBehaviour targeter = temp.Targeter;
        if (targeter != null)
        {
          bool flag3 = false;
          if (!targeter.IsAlive)
          {
            flag3 = true;
          }
          else
          {
            NpcQueryPreference npcQueryPreference = preference & NpcQueryPreference.QueryTypes;
            if (npcQueryPreference != NpcQueryPreference.None && (npcQueryPreference & temp.Query) != npcQueryPreference)
            {
              flag3 = true;
            }
            else
            {
              ActorType actorType = targeter is ITMPlayer ? ActorType.Player : targeter.ActorType;
              if (flag1 && !searchTypes.Contains(actorType))
                flag3 = true;
              else if (flag2 && excludeTypes.Contains(actorType))
                flag3 = true;
            }
          }
          if (flag3)
            TargetingSystem.tempList.RemoveAt(index);
        }
      }
      tempList = TargetingSystem.tempList;
      TargetingSystem.sortType = preference;
      TargetingSystem.sortTargeted = target;
      tempList.Sort(new Comparison<TargetData>(TargetingSystem.SortTargets));
      return tempList;
    }

    private static int SortTargets(TargetData t1, TargetData t2)
    {
      if ((TargetingSystem.sortType & NpcQueryPreference.Weakest) > NpcQueryPreference.None)
      {
        float health = t1.Targeter.Health;
        int num = t2.Targeter.Health.CompareTo(health);
        if (num != 0)
          return num;
      }
      if ((TargetingSystem.sortType & NpcQueryPreference.Strongest) > NpcQueryPreference.None)
      {
        int num = t1.Targeter.Health.CompareTo(t2.Targeter.Health);
        if (num != 0)
          return num;
      }
      if ((TargetingSystem.sortType & NpcQueryPreference.LowestHP) > NpcQueryPreference.None)
      {
        float health = t1.Targeter.Health;
        int num = t2.Targeter.Health.CompareTo(health);
        if (num != 0)
          return num;
      }
      if ((TargetingSystem.sortType & NpcQueryPreference.HighestHP) > NpcQueryPreference.None)
      {
        int num = t1.Targeter.Health.CompareTo(t2.Targeter.Health);
        if (num != 0)
          return num;
      }
      if ((TargetingSystem.sortType & NpcQueryPreference.Agressive) > NpcQueryPreference.None)
      {
        int num = (t1.Query & NpcQueryPreference.Agressive).CompareTo((object) (t2.Query & NpcQueryPreference.Agressive));
        if (num != 0)
          return num;
      }
      return Vector3.DistanceSquared(TargetingSystem.sortTargeted.Position, t1.Targeter.Position).CompareTo(Vector3.DistanceSquared(TargetingSystem.sortTargeted.Position, t2.Targeter.Position));
    }
  }
}
