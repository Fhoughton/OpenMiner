// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ActionLog
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class ActionLog
  {
    private int[] mined;
    private int[,] actions;

    private void LazyCreateMined()
    {
      if (this.mined != null)
        return;
      this.mined = new int[256];
    }

    private void LazyCreateActions()
    {
      if (this.actions != null)
        return;
      this.actions = new int[Globals1.ItemData.Length + 1, 4];
    }

    public ActionLog()
    {
    }

    public ActionLog(ActionLog clone)
    {
      if (clone.mined != null)
      {
        this.mined = new int[clone.mined.Length];
        Array.Copy((Array) clone.mined, (Array) this.mined, clone.mined.Length);
      }
      if (clone.actions == null)
        return;
      this.actions = new int[clone.actions.GetLength(0), clone.actions.GetLength(1)];
      Array.Copy((Array) clone.actions, (Array) this.actions, clone.actions.GetLength(0) * clone.actions.GetLength(1));
    }

    public void Merge(ActionLog other)
    {
      if (other.mined != null)
      {
        this.LazyCreateMined();
        for (int index = 0; index < this.mined.Length && index < other.mined.Length; ++index)
        {
          if (this.mined[index] < other.mined[index])
            this.mined[index] = other.mined[index];
        }
      }
      if (other.actions == null)
        return;
      this.LazyCreateActions();
      for (int index1 = 0; index1 < this.actions.GetLength(0) && index1 < other.actions.GetLength(0); ++index1)
      {
        for (int index2 = 0; index2 < this.actions.GetLength(1) && index2 < other.actions.GetLength(1); ++index2)
        {
          if (this.actions[index1, index2] < other.actions[index1, index2])
            this.actions[index1, index2] = other.actions[index1, index2];
        }
      }
    }

    public void AddAction(Block block)
    {
      this.LazyCreateMined();
      ++this.mined[(int) block];
    }

    public void AddAction(Item item, ItemAction action)
    {
      if (action == ItemAction.Mined)
      {
        if (item >= Item.zLastBlockID)
          return;
        this.LazyCreateMined();
        ++this.mined[(int) item];
      }
      else
      {
        this.LazyCreateActions();
        if (item >= (Item) this.actions.Length)
          return;
        ++this.actions[(int) item, (int) action];
      }
    }

    public void SetAction(Item item, ItemAction action, int count)
    {
      if (action == ItemAction.Mined)
      {
        if (item >= Item.zLastBlockID || count <= 0 && this.mined == null)
          return;
        this.LazyCreateMined();
        this.mined[(int) item] = count;
      }
      else
      {
        if (count <= 0 && this.actions == null)
          return;
        this.LazyCreateActions();
        if (item >= (Item) this.actions.Length)
          return;
        this.actions[(int) item, (int) action] = count;
      }
    }

    public bool HasAction(Item item, ItemAction action)
    {
      return this.GetAction(item, action) > 0;
    }

    public int GetAction(Item item, ItemAction action)
    {
      if (action == ItemAction.Mined)
      {
        if (this.mined == null || item >= Item.zLastBlockID)
          return 0;
        return this.mined[(int) item];
      }
      if (this.actions == null || item >= (Item) this.actions.Length)
        return 0;
      return this.actions[(int) item, (int) action];
    }

    public void ReadState(BinaryReader reader, int version)
    {
      int num1 = reader.ReadInt32();
      this.mined = (int[]) null;
      if (num1 > 0)
      {
        this.LazyCreateMined();
        for (int index = 0; index < num1 && index < this.mined.Length; ++index)
          this.mined[index] = reader.ReadInt32();
      }
      int num2 = reader.ReadInt32();
      int num3 = reader.ReadInt32();
      this.actions = (int[,]) null;
      if (num2 <= 0 || num3 <= 0)
        return;
      this.LazyCreateActions();
      for (int index1 = 0; index1 < num2 && index1 < this.actions.GetLength(0); ++index1)
      {
        for (int index2 = 0; index2 < num3 && index2 < this.actions.GetLength(1); ++index2)
          this.actions[index1, index2] = reader.ReadInt32();
      }
    }

    public void WriteState(BinaryWriter writer)
    {
      bool flag1 = false;
      if (this.mined != null)
      {
        foreach (int num in this.mined)
        {
          if (num > 0)
          {
            flag1 = true;
            break;
          }
        }
      }
      if (!flag1)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(this.mined.Length);
        foreach (int num in this.mined)
          writer.Write(num);
      }
      bool flag2 = false;
      if (this.actions != null)
      {
        for (int index1 = 0; index1 < this.actions.GetLength(0) && !flag2; ++index1)
        {
          for (int index2 = 0; index2 < this.actions.GetLength(1) && !flag2; ++index2)
          {
            if (this.actions[index1, index2] > 0)
              flag2 = true;
          }
        }
      }
      if (!flag2)
      {
        writer.Write(0);
        writer.Write(0);
      }
      else
      {
        writer.Write(this.actions.GetLength(0));
        writer.Write(this.actions.GetLength(1));
        for (int index1 = 0; index1 < this.actions.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < this.actions.GetLength(1); ++index2)
            writer.Write(this.actions[index1, index2]);
        }
      }
    }
  }
}
