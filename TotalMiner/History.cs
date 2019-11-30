// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.History
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  public class History
  {
    public Dictionary<string, long> Table;

    public History()
    {
      this.Table = new Dictionary<string, long>();
    }

    public History(History clone)
    {
      if (clone != null && clone.Table != null)
      {
        lock (clone.Table)
          this.Table = new Dictionary<string, long>((IDictionary<string, long>) clone.Table);
      }
      else
        this.Table = new Dictionary<string, long>();
    }

    public void AddHistory(string key)
    {
      this.AdjustHistory(key, 1L);
    }

    public void DecrementHistory(string key)
    {
      this.AdjustHistory(key, -1L);
    }

    public void SetHistory(string key, long i)
    {
      if (key == null)
        return;
      if (key.HasUpperChars())
        key = key.ToLower();
      lock (this.Table)
      {
        if (this.Table.ContainsKey(key))
        {
          if (i != 0L)
            this.Table[key] = i;
          else
            this.Table.Remove(key);
        }
        else
        {
          if (i == 0L)
            return;
          this.Table.Add(key, i);
        }
      }
    }

    public void AdjustHistory(string key, long rel)
    {
      if (key == null)
        return;
      if (key.HasUpperChars())
        key = key.ToLower();
      lock (this.Table)
      {
        long num1;
        if (this.Table.TryGetValue(key, out num1))
        {
          long num2 = num1 + rel;
          if (num2 != 0L)
            this.Table[key] = num2;
          else
            this.Table.Remove(key);
        }
        else
        {
          if (rel == 0L)
            return;
          this.Table.Add(key, rel);
        }
      }
    }

    public void ClearHistory(string key)
    {
      if (key == null)
        return;
      lock (this.Table)
        this.Table.Remove(key);
    }

    public bool HasHistory(string key)
    {
      if (key == null)
        return false;
      lock (this.Table)
        return this.Table.ContainsKey(key);
    }

    public int TableCount
    {
      get
      {
        return this.Table.Count;
      }
    }

    public long GetHistory(string key)
    {
      if (key != null && key.Length > 0)
      {
        lock (this.Table)
        {
          long num;
          if (this.Table.TryGetValue(key, out num))
            return num;
        }
      }
      return 0;
    }

    public bool IsEquals(History h)
    {
      if (this.Table == null || this.Table.Count == 0)
      {
        if (h != null && h.Table != null)
          return h.Table.Count == 0;
        return true;
      }
      if (this.Table.Count != h.Table.Count)
        return false;
      lock (this.Table)
      {
        foreach (KeyValuePair<string, long> keyValuePair in this.Table)
        {
          long num;
          if (!h.Table.TryGetValue(keyValuePair.Key, out num) || num != keyValuePair.Value)
            return false;
        }
      }
      return true;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.Table = new Dictionary<string, long>();
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        string s = reader.ReadString();
        long num2 = 1;
        if (version > 242)
          num2 = reader.ReadInt64();
        else if (version > 162)
          num2 = (long) reader.ReadInt32();
        this.Table.Add(s.HasUpperChars() ? s.ToLower() : s, num2);
      }
    }

    public void WriteState(BinaryWriter writer)
    {
      if (this.Table != null)
      {
        lock (this.Table)
        {
          writer.Write(this.Table.Count);
          foreach (KeyValuePair<string, long> keyValuePair in this.Table)
          {
            writer.Write(keyValuePair.Key);
            writer.Write(keyValuePair.Value);
          }
        }
      }
      else
        writer.Write(0);
    }
  }
}
