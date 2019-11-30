// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.HighScoreData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class HighScoreData
  {
    public Dictionary<string, HighScoreItem> HighScores = new Dictionary<string, HighScoreItem>();
    private Dictionary<string, bool> bannedGamertags = new Dictionary<string, bool>();

    public void Unload()
    {
      this.HighScores.Clear();
      this.HighScores = new Dictionary<string, HighScoreItem>();
      this.bannedGamertags.Clear();
      this.bannedGamertags = new Dictionary<string, bool>();
    }

    public void AddGamertagToHighscoresBanList(string gamertag, bool isBanned)
    {
      if (!this.bannedGamertags.ContainsKey(gamertag))
        this.bannedGamertags.Add(gamertag, isBanned);
      else
        this.bannedGamertags[gamertag] = isBanned;
    }

    public bool IsGamertagBanned(string gamertag)
    {
      bool flag;
      if (this.bannedGamertags.TryGetValue(gamertag, out flag))
        return flag;
      return false;
    }

    public void ReadStateBanned(BinaryReader reader, int version)
    {
      int num = version > 201 ? reader.ReadInt32() : 0;
      for (int index = 0; index < num; ++index)
      {
        string key = Globals2.ReadGamertag(reader);
        bool flag = reader.ReadBoolean();
        if (this.bannedGamertags.ContainsKey(key))
          this.bannedGamertags[key] = flag;
        else
          this.bannedGamertags.Add(key, flag);
      }
    }

    public void WriteStateBanned(BinaryWriter writer)
    {
      writer.Write(this.bannedGamertags.Count);
      foreach (KeyValuePair<string, bool> bannedGamertag in this.bannedGamertags)
      {
        Globals2.WriteGamertag(writer, bannedGamertag.Key);
        writer.Write(bannedGamertag.Value);
      }
    }

    public void RemoveTextGTs()
    {
    }

    private bool IsTestGamertag(string gamertag)
    {
      if (gamertag == null || gamertag.Length < 1)
        return true;
      for (int index = 0; index < gamertag.Length; ++index)
      {
        char c = gamertag[index];
        if (c == ' ' || char.IsLower(c))
          return false;
      }
      return true;
    }
  }
}
