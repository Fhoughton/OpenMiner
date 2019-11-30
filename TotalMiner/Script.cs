// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Script
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class Script
  {
    public static int NextEditID;
    public string Name;
    public string Alias;
    public int EditID;
    public List<string> Commands;
    public MemoryStream ByteCode;
    public BinaryWriter ByteCodeWriter;
    public BinaryReader ByteCodeReader;
    public bool IsChanged;
    public long LastExecutionTicks;
    public long TotalExecutionTicks;
    public int ExecutionCount;
    public int ByteCodeSize;
    public string[] VarNames;
    public Dictionary<int, LootTable> LootTables;

    public static string NameEditID(Script script)
    {
      if (script.Name == null || script.Name.Length < 1)
        return "_" + script.EditID.ToString();
      int num = script.Name.LastIndexOf('\\');
      return (num >= 0 ? script.Name.Substring(num + 1) : script.Name) + "_" + script.EditID.ToString();
    }

    public static int GetEditID(string name)
    {
      int num = name.LastIndexOf('_');
      int result;
      if (int.TryParse(name.Substring(num + 1, name.Length - (num + 1) - 4), out result))
        return result;
      return -1;
    }

    public string Path
    {
      get
      {
        int num = this.Name.LastIndexOf('\\');
        if (num >= 0)
          return this.Name.Substring(0, num + 1);
        return "";
      }
    }

    public int RAMUsedScriptCode
    {
      get
      {
        int num = 64 + (this.Name != null ? this.Name.Length / 2 * 4 : 0);
        foreach (string command in this.Commands)
          num += 24 + command.Length / 2 * 4;
        return num;
      }
    }

    public int RAMUsedByteCode
    {
      get
      {
        return this.ByteCodeSize;
      }
    }

    public bool IsInConditionalBlock(int line)
    {
      if (this.Commands[line].Equals("if", StringComparison.OrdinalIgnoreCase) || this.Commands[line].Equals("then", StringComparison.OrdinalIgnoreCase) || (this.Commands[line].Equals("elseif", StringComparison.OrdinalIgnoreCase) || this.Commands[line].Equals("else", StringComparison.OrdinalIgnoreCase)) || this.Commands[line].Equals("endif", StringComparison.OrdinalIgnoreCase))
        return false;
      for (int index = line - 1; index >= 0; --index)
      {
        if (index < this.Commands.Count)
        {
          if (this.Commands[index].StartsWith("endif", StringComparison.OrdinalIgnoreCase))
            return false;
          if (this.Commands[index].StartsWith("elseif", StringComparison.OrdinalIgnoreCase) || this.Commands[index].StartsWith("else", StringComparison.OrdinalIgnoreCase) || (this.Commands[index].StartsWith("then", StringComparison.OrdinalIgnoreCase) || this.Commands[index].StartsWith("if", StringComparison.OrdinalIgnoreCase)))
            return true;
        }
      }
      return false;
    }

    public Script(string name)
    {
      this.Name = name;
      this.Alias = "";
      this.Commands = new List<string>();
      this.EditID = ++Script.NextEditID;
    }

    public Script(string name, int commandCount)
    {
      this.Name = name;
      this.Alias = "";
      this.Commands = new List<string>(commandCount);
      this.EditID = ++Script.NextEditID;
    }

    public Script(Script script)
    {
      this.Name = script.Name;
      this.Alias = script.Alias;
      this.IsChanged = script.IsChanged;
      this.Commands = new List<string>((IEnumerable<string>) script.Commands);
      this.LastExecutionTicks = script.LastExecutionTicks;
      this.TotalExecutionTicks = script.TotalExecutionTicks;
      this.ExecutionCount = script.ExecutionCount;
      this.ByteCodeSize = script.ByteCodeSize;
    }

    public enum ReplaceTextType
    {
      Script,
      SelectedText,
      Folder,
      AllScripts,
    }
  }
}
