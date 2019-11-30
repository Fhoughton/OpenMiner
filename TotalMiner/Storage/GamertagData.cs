// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.GamertagData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class GamertagData
  {
    public Point ConsolePos = new Point(2, 2);
    public Point ConsoleSize = new Point(550, 376);
    public readonly string Gamertag;
    public GlobalGamerSettings Settings;
    public CharacterSkillsData SkillData;
    public List<ServerEntry> ServerData;
    public PlayerUnlockableData UnlockData;
    public List<string> TextMessagePresets;
    public Dictionary<string, StudioForge.TotalMiner.TabData> TabData;

    public GamertagData(Gamer gamer)
      : this(gamer.Gamertag)
    {
      this.Settings = new GlobalGamerSettings();
      this.ServerData = new List<ServerEntry>();
      this.UnlockData = new PlayerUnlockableData(gamer);
      this.TextMessagePresets = new List<string>();
      this.TabData = new Dictionary<string, StudioForge.TotalMiner.TabData>(20);
    }

    public GamertagData(string gamertag)
    {
      this.Gamertag = gamertag;
      this.SkillData = new CharacterSkillsData();
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.UnlockData = new PlayerUnlockableData(this.Gamertag);
      this.Settings = new GlobalGamerSettings();
      this.ServerData = new List<ServerEntry>();
      this.TextMessagePresets = new List<string>();
      this.TabData = new Dictionary<string, StudioForge.TotalMiner.TabData>(20);
      try
      {
        this.SkillData.ReadState(reader, version);
      }
      catch (Exception ex)
      {
        this.SkillData = new CharacterSkillsData();
        throw ex;
      }
      try
      {
        this.UnlockData.ReadState(reader, version);
      }
      catch (Exception ex)
      {
        this.UnlockData = new PlayerUnlockableData(this.Gamertag);
        throw ex;
      }
      try
      {
        this.Settings.ReadState(reader, version);
      }
      catch (Exception ex)
      {
        this.Settings = new GlobalGamerSettings();
        throw ex;
      }
      try
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
        {
          ServerEntry serverEntry = new ServerEntry();
          serverEntry.ReadState(reader, version);
          this.ServerData.Add(serverEntry);
        }
      }
      catch (Exception ex)
      {
        this.ServerData = new List<ServerEntry>();
        throw ex;
      }
      if (version > 221)
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          this.TextMessagePresets.Add(reader.ReadString());
      }
      if (version <= 285)
        return;
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.TabData.Add(reader.ReadString(), new StudioForge.TotalMiner.TabData()
        {
          Sliding = reader.ReadBoolean(),
          Scale = reader.ReadSingle(),
          Offset = new Vector2(reader.ReadSingle(), reader.ReadSingle())
        });
      if (version <= 287)
        return;
      this.ConsolePos.X = (int) reader.ReadInt16();
      this.ConsolePos.Y = (int) reader.ReadInt16();
      this.ConsoleSize.X = (int) reader.ReadInt16();
      this.ConsoleSize.Y = (int) reader.ReadInt16();
    }

    public void WriteState(BinaryWriter writer)
    {
      Globals2.WriteGamertag(writer, this.Gamertag);
      this.SkillData.WriteState(writer);
      this.UnlockData.WriteState(writer);
      this.Settings.WriteState(writer);
      writer.Write(this.ServerData.Count);
      foreach (ServerEntry serverEntry in this.ServerData)
        serverEntry.WriteState(writer);
      writer.Write(this.TextMessagePresets.Count);
      foreach (string textMessagePreset in this.TextMessagePresets)
        writer.Write(textMessagePreset);
      writer.Write(this.TabData.Count);
      foreach (KeyValuePair<string, StudioForge.TotalMiner.TabData> keyValuePair in this.TabData)
      {
        writer.Write(keyValuePair.Key);
        writer.Write(keyValuePair.Value.Sliding);
        writer.Write(keyValuePair.Value.Scale);
        writer.Write(keyValuePair.Value.Offset.X);
        writer.Write(keyValuePair.Value.Offset.Y);
      }
      writer.Write((short) this.ConsolePos.X);
      writer.Write((short) this.ConsolePos.Y);
      writer.Write((short) this.ConsoleSize.X);
      writer.Write((short) this.ConsoleSize.Y);
    }
  }
}
