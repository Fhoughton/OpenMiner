// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChangeLog
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ChangeLog
  {
    private LinkedList<ChangeLogItem> items;

    public ChangeLog()
    {
      this.items = new LinkedList<ChangeLogItem>();
    }

    public int Count
    {
      get
      {
        return this.items.Count;
      }
    }

    public void Clear()
    {
      this.items.Clear();
    }

    private void AddChangeLog(ChangeLogItem item)
    {
      this.items.AddLast(item);
      if (this.items.Count <= 1000)
        return;
      this.items.RemoveFirst();
    }

    public List<ChangeLogItem> ToList()
    {
      List<ChangeLogItem> changeLogItemList = new List<ChangeLogItem>(this.items.Count);
      foreach (ChangeLogItem changeLogItem in this.items)
        changeLogItemList.Add(changeLogItem);
      return changeLogItemList;
    }

    public void WriteItems(List<string> list)
    {
      foreach (ChangeLogItem changeLogItem in this.items)
        list.Add(changeLogItem.Log);
    }

    public void LogHitDoor(GameInstance instance, Player player, GlobalPoint3D p)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "HitDoor " + this.LogParamPoint(p)
      });
    }

    public void LogHitSwitch(GameInstance instance, Player player, GlobalPoint3D p)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "HitSwitch " + this.LogParamPoint(p)
      });
    }

    public void LogHitButton(GameInstance instance, Player player, GlobalPoint3D p)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "HitButton " + this.LogParamPoint(p)
      });
    }

    public void LogSetBlock(
      GameInstance instance,
      Player player,
      GlobalPoint3D p,
      Item blockID,
      byte auxData)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "SetBlock " + this.LogParamPoint(p) + " " + this.LogParamBlock(blockID) + " " + this.LogParamInt((int) auxData, auxData > (byte) 0)
      });
    }

    public void LogSetRegion(
      GameInstance instance,
      Player player,
      GlobalPoint3D min,
      GlobalPoint3D max,
      Item blockID,
      int percent,
      int seed)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "SetRegion " + this.LogParamPoint(min) + " " + this.LogParamPoint(max) + " " + this.LogParamBlock(blockID) + " " + this.LogParamInt(percent, percent < 100) + " " + this.LogParamInt(seed, percent < 100)
      });
    }

    public void LogReplaceRegion(
      GameInstance instance,
      Player player,
      GlobalPoint3D min,
      GlobalPoint3D max,
      Item blockID1,
      Item blockID2,
      int percent,
      int seed)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "ReplaceRegion " + this.LogParamPoint(min) + " " + this.LogParamPoint(max) + " " + this.LogParamBlock(blockID1) + " " + this.LogParamBlock(blockID2) + " " + this.LogParamInt(percent, percent < 100) + " " + this.LogParamInt(seed, percent < 100)
      });
    }

    public void LogSetSphere(
      GameInstance instance,
      Player player,
      GlobalPoint3D p,
      int radius,
      Item blockID,
      int percent,
      int seed)
    {
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = "SetSphere " + this.LogParamPoint(p) + " " + this.LogParamInt(radius, true) + " " + this.LogParamBlock(blockID) + " " + this.LogParamInt(percent, percent < 100) + " " + this.LogParamInt(seed, percent < 100)
      });
    }

    public void LogPaste(
      GameInstance instance,
      Player player,
      int comDirNum,
      string comName,
      GlobalPoint3D p,
      BlockFace facing,
      Map.CopyType copyType)
    {
      if (comName != null)
        comName = comName.Replace('_', '\\');
      string str = comDirNum < 0 || comName == null ? "Paste [clipboard] " : (comDirNum == 0 ? string.Format("Paste [System:{0}] ", (object) comName) : string.Format("Paste [{0}:{1}] ", (object) instance.VoxelModelManager.GetPackName(comDirNum), (object) comName));
      this.AddChangeLog(new ChangeLogItem()
      {
        Time = instance.CurrentDaysGameTime,
        Log = str + this.LogParamPoint(p) + " " + this.LogParamString(facing.ToString()) + " " + this.LogParamString(copyType.ToString())
      });
    }

    private string LogParamPoint(GlobalPoint3D p)
    {
      return string.Format("[{0},{1},{2}]", (object) p.X, (object) p.Y, (object) p.Z);
    }

    private string LogParamBlock(Item blockID)
    {
      string str = blockID.ToString();
      if (str.EndsWith("Icon"))
        str = str.Substring(0, str.Length - 4);
      return string.Format("[{0}]", (object) str);
    }

    private string LogParamInt(int i, bool dontOmit)
    {
      if (!dontOmit)
        return (string) null;
      return string.Format("[{0}]", (object) i);
    }

    private string LogParamString(string s)
    {
      return string.Format("[{0}]", (object) s);
    }
  }
}
