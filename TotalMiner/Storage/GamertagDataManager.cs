// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.GamertagDataManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class GamertagDataManager
  {
    public const int MaxHowToReadID = 10;
    public List<StudioForge.TotalMiner.Storage.GamertagData> GamertagData;
    public HighScoreData HighScoreData;
    public Dictionary<string, CharacterSkillsData> HighScoreCache;
    public bool IsServer;
    public bool SkipSkillMergeInternal;

    public bool IsHighScoresLoaded
    {
      get
      {
        if (this.HighScoreData != null)
          return this.HighScoreData.HighScores.Count > 0;
        return false;
      }
    }

    public GamertagDataManager()
    {
      this.GamertagData = new List<StudioForge.TotalMiner.Storage.GamertagData>();
      this.HighScoreCache = new Dictionary<string, CharacterSkillsData>();
    }

    public void LoadGamertagData()
    {
      lock (Globals1.SaveSemaphore)
      {
        string path = "GamertagData.db";
        if (!FileSystem.IsFileExist(path))
          return;
        using (Stream stream = FileSystem.OpenRead(path))
        {
          if (stream.Length <= 0L)
            return;
          byte[] numArray = new byte[stream.Length];
          stream.Read(numArray, 0, numArray.Length);
          using (MemoryStream memoryStream = new MemoryStream(numArray))
          {
            using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
            {
              int version = reader.ReadInt32();
              if (!Globals2.CheckHash(numArray, version) && !this.IsServer)
                throw new BadHashException();
              bool exceptionOccurred = false;
              int num = reader.ReadInt32();
              if (num > 0)
                this.GamertagData.Clear();
              for (int index = 0; index < num && !exceptionOccurred; ++index)
                this.GamertagData.Add(this.ReadGamerTagData(reader, version, out exceptionOccurred));
            }
          }
        }
      }
    }

    public void LoadHighScoreDataGlobal()
    {
      if (this.IsHighScoresLoaded)
        return;
      this.HighScoreData = this.LoadHighScoreDataLocal();
    }

    private HighScoreData LoadHighScoreDataLocal()
    {
      HighScoreData highScores = new HighScoreData();
      try
      {
        lock (Globals1.SaveSemaphore)
        {
          if (FileSystem.IsFileExist("HighScores.db"))
          {
            using (Stream stream = FileSystem.OpenRead("HighScores.db"))
            {
              if (stream.Length > 0L)
              {
                byte[] numArray = new byte[stream.Length];
                stream.Read(numArray, 0, numArray.Length);
                using (MemoryStream memoryStream = new MemoryStream(numArray))
                {
                  using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
                  {
                    int version = reader.ReadInt32();
                    if (!Globals2.CheckHash(numArray, version) && !this.IsServer)
                      throw new BadHashException();
                    if (version > 187)
                      Globals2.HighscoreUpdateTimestamp = reader.ReadInt32();
                    if (version < 202)
                      Globals2.HighscoreUpdateTimestamp = 0;
                    int num1 = reader.ReadInt32();
                    int num2 = version <= 194 || version >= 202 ? 0 : reader.ReadInt32();
                    if (version < 201)
                      num2 = 0;
                    for (int index = 0; index < num1; ++index)
                      this.ReadHighScoreItem(reader, highScores, version);
                    for (int index = 0; index < num2; ++index)
                      this.ReadHighScoreOverride(reader, highScores, version);
                    highScores.ReadStateBanned(reader, version);
                  }
                }
              }
            }
          }
        }
      }
      catch (BadHashException ex)
      {
        FileSystem.DeleteFile("HighScores.db");
      }
      catch (Exception ex)
      {
      }
      return highScores;
    }

    public void SaveGamertagData(bool saveHighScores, bool merge)
    {
      try
      {
        lock (Globals1.SaveSemaphore)
        {
          this.SaveGamertagDataNoLockNoFlushCore(merge);
          if (!saveHighScores)
            return;
          this.SaveHighScoreDataNoLockNoFlushCore();
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(64, ex);
      }
    }

    public bool RepairGamertagData()
    {
      return this.RepairSkillDataFromHighscores(this.GamertagData, this.HighScoreData);
    }

    public bool RepairSkillDataFromHighscores(HighScoreData highScores)
    {
      return this.RepairSkillDataFromHighscores(this.GamertagData, highScores);
    }

    private bool RepairSkillDataFromHighscores(
      List<StudioForge.TotalMiner.Storage.GamertagData> gamertagData,
      HighScoreData highScores)
    {
      bool flag1 = false;
      if (this.GamertagData != null && this.GamertagData.Count > 0 && highScores != null)
      {
        foreach (StudioForge.TotalMiner.Storage.GamertagData gamertagData1 in this.GamertagData)
        {
          HighScoreItem data;
          if (highScores.HighScores.TryGetValue(gamertagData1.Gamertag, out data))
          {
            bool flag2 = false;
            if (gamertagData1.SkillData == null)
            {
              gamertagData1.SkillData = new CharacterSkillsData(data);
              flag2 = flag1 = true;
            }
            else
            {
              for (int index = 0; index < data.XPList.Length; ++index)
              {
                SkillData skillData = gamertagData1.SkillData[index + 1];
                if (skillData.CurrentXP < (double) data.XPList[index])
                {
                  skillData.SetCurrentXPRaw((double) data.XPList[index]);
                  gamertagData1.SkillData[index + 1] = skillData;
                  flag2 = flag1 = true;
                }
              }
            }
            if (flag2)
              TotalMinerGame.Instance.AddNotification("Skills XP for " + gamertagData1.Gamertag + " has been recovered from the Highscore Table", true);
          }
        }
      }
      return flag1;
    }

    public GlobalGamerSettings GetGlobalGamerSettings(PlayerIndex playerIndex)
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(playerIndex);
      if (signedInGamer != null)
        return this.GetOrAddGamertagData(signedInGamer).Settings;
      return (GlobalGamerSettings) null;
    }

    public PlayerUnlockableData GetPlayerUnlocks(Gamer gamer)
    {
      return this.GetOrAddGamertagData(gamer)?.UnlockData;
    }

    public StudioForge.TotalMiner.Storage.GamertagData GetGamertagData(Gamer gamer)
    {
      if (gamer == null)
        return (StudioForge.TotalMiner.Storage.GamertagData) null;
      return this.GetGamertagData(gamer.Gamertag);
    }

    public StudioForge.TotalMiner.Storage.GamertagData GetGamertagData(
      PlayerIndex playerIndex)
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(playerIndex);
      if (signedInGamer != null)
        return this.GetGamertagData(signedInGamer);
      return (StudioForge.TotalMiner.Storage.GamertagData) null;
    }

    public StudioForge.TotalMiner.Storage.GamertagData GetOrAddGamertagData(
      PlayerIndex playerIndex)
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(playerIndex);
      if (signedInGamer != null)
        return this.GetOrAddGamertagData(signedInGamer);
      return (StudioForge.TotalMiner.Storage.GamertagData) null;
    }

    private StudioForge.TotalMiner.Storage.GamertagData GetGamertagData(string gamertag)
    {
      if (gamertag != null && gamertag.Length > 0)
      {
        foreach (StudioForge.TotalMiner.Storage.GamertagData gamertagData in this.GamertagData)
        {
          if (gamertagData != null && gamertagData.Gamertag == gamertag)
            return gamertagData;
        }
      }
      return (StudioForge.TotalMiner.Storage.GamertagData) null;
    }

    private StudioForge.TotalMiner.Storage.GamertagData GetOrAddGamertagData(Gamer gamer)
    {
      StudioForge.TotalMiner.Storage.GamertagData gamertagData = this.GetGamertagData(gamer);
      if (gamertagData == null && gamer != null)
      {
        gamertagData = new StudioForge.TotalMiner.Storage.GamertagData(gamer);
        this.GamertagData.Add(gamertagData);
      }
      return gamertagData;
    }

    public CharacterSkillsData GetPlayerSkillData(Gamer gamer)
    {
      if (gamer == null)
        return (CharacterSkillsData) null;
      return this.GetOrAddGamertagData(gamer).SkillData;
    }

    public bool UpdateBadBoys(List<BadBoyData> badboyList)
    {
      bool flag = false;
      for (int index = 0; index < this.GamertagData.Count; ++index)
      {
        StudioForge.TotalMiner.Storage.GamertagData gamertagData = this.GamertagData[index];
        if (gamertagData != null && gamertagData.UnlockData.BadBoy != BadBoyType.None)
          gamertagData.UnlockData.BadBoy = BadBoyType.None;
      }
      if (badboyList != null && badboyList.Count > 0)
      {
        foreach (BadBoyData badboy in badboyList)
        {
          if (badboy.Gamertag.Length > 0)
          {
            StudioForge.TotalMiner.Storage.GamertagData gamertagData = this.GetGamertagData(badboy.Gamertag);
            if (gamertagData != null)
            {
              gamertagData.UnlockData.BadBoy = badboy.Type;
              flag = true;
            }
          }
        }
      }
      return flag;
    }

    public void AddHighScoreCacheEntry(Gamer gamer, CharacterSkillsData skillsData)
    {
      if (gamer == null)
        return;
      this.AddHighScoreCacheEntry(gamer.Gamertag, skillsData);
    }

    public void AddHighScoreCacheEntry(string gamertag, CharacterSkillsData skillsData)
    {
      if (gamertag == null || gamertag.Length <= 0)
        return;
      lock (this.HighScoreCache)
      {
        if (this.HighScoreCache.ContainsKey(gamertag))
          return;
        this.HighScoreCache.Add(gamertag, skillsData);
      }
    }

    public void AddHighScoreEntry(
      HighScoreData highScores,
      Gamer gamer,
      CharacterSkillsData skillsData)
    {
      this.AddHighScoreEntry(highScores, gamer, new HighScoreItem(skillsData));
    }

    public void AddHighScoreEntry(HighScoreData highScores, Gamer gamer, HighScoreItem data)
    {
      if (highScores == null || gamer == null)
        return;
      this.AddHighScoreEntry(highScores, gamer.Gamertag, data);
    }

    public bool AddHighScoreEntry(HighScoreData highScores, string gamertag, HighScoreItem data)
    {
      bool flag = false;
      if (gamertag != null && gamertag.Length > 0)
      {
        HighScoreItem highScoreItem;
        if (highScores.HighScores.TryGetValue(gamertag, out highScoreItem))
        {
          highScoreItem.Ticks = data.Ticks;
          for (int index = 0; index < data.XPList.Length; ++index)
          {
            if (data.XPList[index] > highScoreItem.XPList[index])
            {
              highScoreItem.XPList[index] = data.XPList[index];
              flag = true;
            }
          }
          highScores.HighScores[gamertag] = highScoreItem;
        }
        else if (data.TotalLevels >= 100)
        {
          highScores.HighScores.Add(gamertag, data);
          flag = true;
        }
      }
      return flag;
    }

    public int GetSkillRank(HighScoreData highScores, SkillType skill, double xp)
    {
      int num = 1;
      if (highScores != null)
      {
        foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
        {
          if (!highScores.IsGamertagBanned(highScore.Key) && (double) highScore.Value.XPList[(int) (skill - (byte) 1)] > xp)
            ++num;
        }
      }
      return num;
    }

    public int GetSkillTotalRank(HighScoreData highScores, int level)
    {
      int num = 1;
      if (highScores != null)
      {
        foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
        {
          if (!highScores.IsGamertagBanned(highScore.Key) && highScore.Value.TotalLevels > level)
            ++num;
        }
      }
      return num;
    }

    public int GetSkillCombatRank(HighScoreData highScores, int level)
    {
      int num = 1;
      if (highScores != null)
      {
        foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
        {
          if (!highScores.IsGamertagBanned(highScore.Key) && highScore.Value.CombatLevel > level)
            ++num;
        }
      }
      return num;
    }

    public List<HighScoreSkillRank> GetSkillSortedRank(
      HighScoreData highScores,
      SkillType skill,
      bool isServer)
    {
      if (highScores == null)
        return (List<HighScoreSkillRank>) null;
      List<HighScoreSkillRank> highScoreSkillRankList = new List<HighScoreSkillRank>(highScores.HighScores.Count);
      foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
      {
        if (isServer || !highScores.IsGamertagBanned(highScore.Key))
        {
          HighScoreSkillRank highScoreSkillRank = new HighScoreSkillRank()
          {
            Gamertag = highScore.Key,
            XP = (long) highScore.Value.XPList[(int) (skill - (byte) 1)]
          };
          highScoreSkillRank.Level = highScore.Value.GetLevel((double) highScoreSkillRank.XP);
          highScoreSkillRankList.Add(highScoreSkillRank);
        }
      }
      highScoreSkillRankList.Sort(new Comparison<HighScoreSkillRank>(this.SortHighScoreSkillRankByXP));
      for (int index = 0; index < highScoreSkillRankList.Count; ++index)
      {
        HighScoreSkillRank highScoreSkillRank = highScoreSkillRankList[index];
        highScoreSkillRank.Rank = index + 1;
        highScoreSkillRankList[index] = highScoreSkillRank;
      }
      return highScoreSkillRankList;
    }

    public List<HighScoreSkillRank> GetSkillCombatSortedRank(
      HighScoreData highScores,
      bool isServer)
    {
      if (highScores == null)
        return (List<HighScoreSkillRank>) null;
      List<HighScoreSkillRank> highScoreSkillRankList = new List<HighScoreSkillRank>(highScores.HighScores.Count);
      foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
      {
        if (isServer || !highScores.IsGamertagBanned(highScore.Key))
          highScoreSkillRankList.Add(new HighScoreSkillRank()
          {
            Gamertag = highScore.Key,
            XP = 0L,
            Level = SkillData.CombatLevel((float) highScore.Value.GetLevel((double) highScore.Value.XPList[2]), (float) highScore.Value.GetLevel((double) highScore.Value.XPList[1]), (float) highScore.Value.GetLevel((double) highScore.Value.XPList[2]), (float) highScore.Value.GetLevel((double) highScore.Value.XPList[3]), (float) highScore.Value.GetLevel((double) highScore.Value.XPList[4]))
          });
      }
      highScoreSkillRankList.Sort(new Comparison<HighScoreSkillRank>(this.SortHighScoreSkillRankByLevel));
      for (int index = 0; index < highScoreSkillRankList.Count; ++index)
      {
        HighScoreSkillRank highScoreSkillRank = highScoreSkillRankList[index];
        highScoreSkillRank.Rank = index + 1;
        highScoreSkillRankList[index] = highScoreSkillRank;
      }
      return highScoreSkillRankList;
    }

    public List<HighScoreSkillRank> GetSkillTotalSortedRank(
      HighScoreData highScores,
      bool isServer)
    {
      if (highScores == null)
        return (List<HighScoreSkillRank>) null;
      List<HighScoreSkillRank> highScoreSkillRankList = new List<HighScoreSkillRank>(highScores.HighScores.Count);
      foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
      {
        if (isServer || !highScores.IsGamertagBanned(highScore.Key))
          highScoreSkillRankList.Add(new HighScoreSkillRank()
          {
            Gamertag = highScore.Key,
            XP = highScore.Value.TotalXP,
            Level = highScore.Value.TotalLevels
          });
      }
      highScoreSkillRankList.Sort(new Comparison<HighScoreSkillRank>(this.SortHighScoreSkillRankByLevel));
      for (int index = 0; index < highScoreSkillRankList.Count; ++index)
      {
        HighScoreSkillRank highScoreSkillRank = highScoreSkillRankList[index];
        highScoreSkillRank.Rank = index + 1;
        highScoreSkillRankList[index] = highScoreSkillRank;
      }
      return highScoreSkillRankList;
    }

    private int SortHighScoreSkillRankByXP(HighScoreSkillRank r1, HighScoreSkillRank r2)
    {
      return r2.XP.CompareTo(r1.XP);
    }

    private int SortHighScoreSkillRankByLevel(HighScoreSkillRank r1, HighScoreSkillRank r2)
    {
      if (r1.Level == r2.Level)
        return r2.XP.CompareTo(r1.XP);
      return r2.Level.CompareTo(r1.Level);
    }

    public bool AddServerEntry(Gamer gamer, int ID, string mapName)
    {
      bool flag = false;
      if (gamer != null && ID != 0)
      {
        ServerEntry serverEntry = this.GetServerEntry(gamer, ID);
        if (serverEntry.ID != ID || serverEntry.MapName != mapName)
        {
          serverEntry.ID = ID;
          serverEntry.MapName = mapName;
          flag = this.UpdateServerEntry(gamer, serverEntry);
          Globals2.SaveGamertagDataThreaded(false, true);
        }
      }
      return flag;
    }

    public bool AddServerRating(Gamer gamer, byte rating)
    {
      if (gamer != null)
      {
        int mapId = NetworkManager.Instance.MapID;
        if (mapId != 0)
        {
          ServerEntry serverEntry = this.GetServerEntry(gamer, mapId);
          if (serverEntry.ID == mapId)
          {
            serverEntry.MyRating = rating;
            return this.UpdateServerEntry(gamer, serverEntry);
          }
        }
      }
      return false;
    }

    private bool UpdateServerEntry(Gamer gamer, ServerEntry entry)
    {
      StudioForge.TotalMiner.Storage.GamertagData orAddGamertagData = this.GetOrAddGamertagData(gamer);
      if (orAddGamertagData == null)
        return false;
      for (int index = 0; index < orAddGamertagData.ServerData.Count; ++index)
      {
        if (orAddGamertagData.ServerData[index].ID == entry.ID)
        {
          orAddGamertagData.ServerData[index] = entry;
          return false;
        }
      }
      orAddGamertagData.ServerData.Add(entry);
      return true;
    }

    public ServerEntry GetServerEntry(Gamer gamer, int ID)
    {
      if (gamer != null && ID != 0)
      {
        StudioForge.TotalMiner.Storage.GamertagData gamertagData = this.GetGamertagData(gamer);
        if (gamertagData != null && gamertagData.ServerData != null)
        {
          foreach (ServerEntry serverEntry in gamertagData.ServerData)
          {
            if (serverEntry.ID == ID)
              return serverEntry;
          }
        }
      }
      return new ServerEntry() { ID = ID };
    }

    public void FlagServerAsFavourite(Gamer gamer, bool flag)
    {
      this.FlagServerAsFavourite(gamer, NetworkManager.Instance.MapID, flag);
    }

    public void FlagServerAsFavourite(Gamer gamer, int ID, bool flag)
    {
      if (ID == 0)
        return;
      ServerEntry serverEntry = this.GetServerEntry(gamer, ID);
      serverEntry.IsFavourite = flag;
      this.UpdateServerEntry(gamer, serverEntry);
    }

    public bool IsServerFavourite(Gamer gamer)
    {
      return this.IsServerFavourite(gamer, NetworkManager.Instance.MapID);
    }

    public bool IsServerFavourite(Gamer gamer, int ID)
    {
      if (gamer == null)
        return false;
      return this.GetServerEntry(gamer, ID).IsFavourite;
    }

    public void AddTextMessagePreset(Gamer gamer, string text)
    {
      StudioForge.TotalMiner.Storage.GamertagData orAddGamertagData = this.GetOrAddGamertagData(gamer);
      if (orAddGamertagData == null)
        return;
      orAddGamertagData.TextMessagePresets.Add(text);
      orAddGamertagData.TextMessagePresets.Sort();
    }

    public void RemoveTextMessagePreset(Gamer gamer, string text)
    {
      this.GetOrAddGamertagData(gamer)?.TextMessagePresets.Remove(text);
    }

    private StudioForge.TotalMiner.Storage.GamertagData ReadGamerTagData(
      BinaryReader reader,
      int version,
      out bool exceptionOccurred)
    {
      exceptionOccurred = false;
      StudioForge.TotalMiner.Storage.GamertagData data = new StudioForge.TotalMiner.Storage.GamertagData(Globals2.ReadGamertag(reader));
      if (version > 199)
      {
        try
        {
          data.ReadState(reader, version);
        }
        catch (Exception ex)
        {
          exceptionOccurred = true;
        }
      }
      else
      {
        data.UnlockData = new PlayerUnlockableData(data.Gamertag);
        this.ReadPlayerUnlockDataOld(reader, version, data.UnlockData);
        data.SkillData = this.ReadPlayerSkillData(reader, version);
        data.ServerData = this.ReadServerData(reader, version);
        data.Settings = new GlobalGamerSettings();
        data.Settings.ReadState(reader, version);
      }
      this.ValidateGamertagData(data);
      return data;
    }

    private void ValidateGamertagData(StudioForge.TotalMiner.Storage.GamertagData data)
    {
      if (Player.IsActorTypeValidForAvatar(data.Settings.PlayerSettings.MobType))
        return;
      data.Settings.PlayerSettings.MobType = ActorType.Boy;
    }

    private CharacterSkillsData ReadPlayerSkillData(
      BinaryReader reader,
      int version)
    {
      CharacterSkillsData characterSkillsData = new CharacterSkillsData();
      if (version > 199)
        characterSkillsData.ReadState(reader, version);
      else if (version > 16)
      {
        int num = (int) reader.ReadByte();
        for (int index = 0; index < num; ++index)
        {
          SkillData skillData = characterSkillsData[index];
          skillData.SetCurrentXPRaw(reader.ReadDouble());
          characterSkillsData[index] = skillData;
        }
      }
      characterSkillsData.AdjustXP(version);
      return characterSkillsData;
    }

    private void ReadPlayerUnlockDataOld(
      BinaryReader reader,
      int version,
      PlayerUnlockableData data)
    {
      data.IndianArrowsCrafted = reader.ReadInt32();
      data.KnightUnlocked = reader.ReadBoolean();
      data.CavemanBlocksCleared = reader.ReadInt32();
      data.ExplorerBlueprintsFound = reader.ReadInt32();
      data.IndianBowCrafted = reader.ReadBoolean();
      reader.ReadInt32();
      reader.ReadInt32();
      reader.ReadInt32();
      if (version > 1)
        data.MadmanDetonations = reader.ReadInt32();
      data.IndianEnemiesKilled = reader.ReadInt32();
      data.TerminatorEnemiesKilled = reader.ReadInt32();
      data.CowboyEnemiesKilled = reader.ReadInt32();
      if (version < 169 && reader.ReadInt32() < 50)
        data.KnightUnlocked = false;
      if (version < 6)
        data.EntrepreneurGoldEarned = reader.ReadInt32();
      data.SoldierGrenadesCrafted = reader.ReadInt32();
      data.SoldierGrenadesLaunched = reader.ReadInt32();
      data.SoldierGrenadeLauncherCrafted = reader.ReadBoolean();
      data.MedicHealedSelf = reader.ReadInt32();
      data.MedicHealedOther = reader.ReadInt32();
      data.PupilHowToRead = PlayerUnlockableData.InitHowToUnlockData();
      if (version < 15)
      {
        int num = version < 12 ? 10 : reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          reader.ReadBoolean();
      }
      else
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          this.ReadHowToUnlockData(reader, version, data.PupilHowToRead);
      }
      if (version < 6)
      {
        int length = reader.ReadInt32();
        bool[] flagArray = new bool[length];
        for (int index = 0; index < length; ++index)
          flagArray[index] = reader.ReadBoolean();
      }
      data.TotalInvadersScore = reader.ReadInt32();
      if (version > 222)
        data.TotalRushScore = reader.ReadInt32();
      data.NinjaKillStreakGamerID = new List<int>();
      if (version > 170)
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          data.NinjaKillStreakGamerID.Add(reader.ReadInt32());
      }
      else
      {
        reader.ReadInt32();
        reader.ReadInt32();
      }
      data.AngelPlayersSaved = reader.ReadInt32();
      data.JamaicanUnlocked = reader.ReadBoolean();
      data.RefugeePlayerKills = reader.ReadInt32();
      reader.ReadInt32();
      reader.ReadBoolean();
      if (version > 3)
        data.HippieFlowersThrownAtEnemy = reader.ReadInt32();
      data.LumberJackTreesChopped = data.TreeHuggerTreesChopped = reader.ReadInt32();
      if (version > 2)
        data.LumberJackSaplingsPlanted = reader.ReadInt32();
      if (version > 2)
        data.LumberJackWoodPlanksCrafted = reader.ReadInt32();
      data.PirateChestsOpened = reader.ReadInt32();
      if (version < 6)
        reader.ReadInt32();
      if (version < 6)
        data.ExplorerWisdomsFound = reader.ReadInt32();
      data.CarpenterWorkbenchCrafted = reader.ReadBoolean();
      data.DiabloUnlocked = reader.ReadBoolean();
      if (version < 143)
        data.DiabloUnlocked = false;
      if (version > 4)
        data.PrisonerUnlocked = reader.ReadBoolean();
      if (version > 6)
      {
        reader.ReadBoolean();
        data.EntrepreneurUnlocked = reader.ReadBoolean();
        data.GoldenKnightUnlocked = reader.ReadBoolean();
        data.SageUnlocked = reader.ReadBoolean();
        if (version < 246)
          data.GoldenKnightUnlocked = false;
      }
      if (version > 7)
        data.ExplorerUnlocked = reader.ReadBoolean();
      if (version > 9)
        data.Girl2ShopPurchase = reader.ReadBoolean();
      if (version > 10)
      {
        data.AstronautWorldsVisited = new List<int>();
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          data.AstronautWorldsVisited.Add(reader.ReadInt32());
        data.KingUnlocked = reader.ReadBoolean();
      }
      if (version > 13)
        data.DemiGodUnlocked = reader.ReadBoolean();
      if (version > 152)
        data.BadBoy = version > 153 ? (BadBoyType) reader.ReadByte() : (reader.ReadBoolean() ? BadBoyType.NoDemi : BadBoyType.None);
      if (version > 15)
        data.ZombieDaysSurvived = reader.ReadInt32();
      ActionLog actionLog = new ActionLog();
      if (version > 18)
      {
        if (version > 163)
        {
          actionLog.ReadState(reader, version);
        }
        else
        {
          ItemDataXML[] itemData = Globals1.ItemData;
          int num1 = reader.ReadInt32();
          int num2 = reader.ReadInt32();
          for (int index1 = 0; index1 < num1; ++index1)
          {
            for (int index2 = 0; index2 < num2; ++index2)
            {
              ItemAction action = index1 == 0 ? ItemAction.Mined : (ItemAction) (index1 - 1);
              actionLog.SetAction((Item) index2, action, reader.ReadInt32());
            }
          }
        }
      }
      else if (version > 17)
      {
        int num1 = reader.ReadInt32();
        for (int index = 0; index < num1; ++index)
        {
          int num2 = (int) reader.ReadByte();
        }
      }
      for (int index = 0; index < 594; ++index)
      {
        if (actionLog.HasAction((Item) index, ItemAction.Crafted))
          data.ChefActions[index] = data.HandymanActions[index] = true;
      }
      this.FilterBadData(data);
    }

    private void ReadHowToUnlockData(BinaryReader reader, int version, bool[][] howToData)
    {
      int index1 = reader.ReadInt32();
      int num = reader.ReadInt32();
      for (int index2 = 0; index2 < num; ++index2)
      {
        if (index1 < howToData.Length && index2 < howToData[index1].Length)
          howToData[index1][index2] = reader.ReadBoolean();
        else
          reader.ReadBoolean();
      }
    }

    private List<ServerEntry> ReadServerData(BinaryReader reader, int version)
    {
      List<ServerEntry> serverEntryList = new List<ServerEntry>();
      if (version > 8)
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
        {
          ServerEntry serverEntry = new ServerEntry();
          serverEntry.ReadState(reader, version);
          serverEntryList.Add(serverEntry);
        }
      }
      return serverEntryList;
    }

    private void ReadHighScoreItem(BinaryReader reader, HighScoreData highScores, int version)
    {
      HighScoreItem data = new HighScoreItem();
      string key = Globals2.ReadGamertag(reader);
      if (version > 186)
        data.Ticks = reader.ReadInt32();
      int[] numArray = data.XPList = new int[15];
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        if (index < numArray.Length)
        {
          int num2 = reader.ReadInt32();
          if (num2 < 0 || num2 > 999999999)
            num2 = 999999999;
          numArray[index] = num2;
        }
      }
      this.AdjustXP(data.XPList, version);
      if (version < 205 && new CharacterSkillsData(data).TotalLevel < 100)
        return;
      highScores.HighScores.Add(key, data);
    }

    private void ReadHighScoreOverride(BinaryReader reader, HighScoreData highScores, int version)
    {
      Globals2.ReadGamertag(reader);
      HighScoreOverride[] highScoreOverrideArray = new HighScoreOverride[15];
      int num = (int) reader.ReadByte();
      for (int index = 0; index < num; ++index)
      {
        highScoreOverrideArray[index].XP = reader.ReadInt32();
        highScoreOverrideArray[index].LocalUpdated = reader.ReadBoolean();
      }
    }

    private void AdjustXP(int[] xp, int version)
    {
      if (version >= 204)
        return;
      int val2 = xp[2] + xp[3] + xp[4];
      if (xp[14] <= Math.Min(5000000, val2 * 3))
        return;
      xp[14] = Math.Min(xp[14], val2);
    }

    private void FilterBadData(PlayerUnlockableData data)
    {
    }

    public void SaveGamertagDataNoLockNoFlushCore(bool merge)
    {
      if (this.GamertagData == null || this.GamertagData.Count <= 0)
        return;
      if (merge)
        this.LoadAndMergeExistingGamertagData();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          writer.Write(294);
          writer.Write(this.GamertagData.Count);
          for (int index = 0; index < this.GamertagData.Count; ++index)
            this.GamertagData[index].WriteState(writer);
          Globals2.WriteFileWithHash("GamertagData.db", (Stream) memoryStream, writer, 294);
        }
      }
    }

    public void SaveHighScoreDataNoLockNoFlushCore()
    {
      if (!this.IsHighScoresLoaded)
        this.LoadHighScoreDataGlobal();
      this.UpdateHighScoreListFromCache(this.HighScoreData);
      this.SaveHighScoreDataRawNoLockNoFlushCore(this.HighScoreData);
    }

    public void SaveHighScoreDataRawNoLockNoFlushCore(HighScoreData highScores)
    {
      if (highScores == null || highScores.HighScores.Count <= 0)
        return;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          writer.Write(294);
          writer.Write(Globals2.HighscoreUpdateTimestamp);
          writer.Write(highScores.HighScores.Count);
          foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
            this.WriteHighScoreItem(writer, highScore.Key, highScore.Value);
          highScores.WriteStateBanned(writer);
          Globals2.WriteFileWithHash("HighScores.db", (Stream) memoryStream, writer, 294);
        }
      }
    }

    private void UpdateHighScoreListFromCache(HighScoreData highScores)
    {
      lock (this.HighScoreCache)
      {
        foreach (KeyValuePair<string, CharacterSkillsData> keyValuePair in this.HighScoreCache)
          this.AddHighScoreEntry(highScores, keyValuePair.Key, new HighScoreItem(keyValuePair.Value));
      }
    }

    private void WritePlayerSkillData(BinaryWriter writer, CharacterSkillsData data)
    {
      int skillCount = data.SkillCount;
      writer.Write((byte) skillCount);
      for (int index = 0; index < skillCount; ++index)
        writer.Write(data[index].CurrentXP);
    }

    private void WriteHighScoreItem(BinaryWriter writer, string gamertag, HighScoreItem data)
    {
      Globals2.WriteGamertag(writer, gamertag);
      writer.Write(data.Ticks);
      writer.Write(data.XPList.Length);
      for (int index = 0; index < data.XPList.Length; ++index)
        writer.Write(data.XPList[index]);
    }

    private void LoadAndMergeExistingGamertagData()
    {
      GamertagDataManager disk = new GamertagDataManager();
      try
      {
        disk.LoadGamertagData();
        this.CompareAndMergeIfNeeded(disk);
      }
      catch (Exception ex)
      {
      }
    }

    private void CompareAndMergeIfNeeded(GamertagDataManager disk)
    {
      for (int index = 0; index < this.GamertagData.Count; ++index)
      {
        StudioForge.TotalMiner.Storage.GamertagData gamertagData1 = this.GamertagData[index];
        StudioForge.TotalMiner.Storage.GamertagData gamertagData2 = disk.GetGamertagData(gamertagData1.Gamertag);
        if (gamertagData2 != null)
        {
          this.MergeUnlockData(gamertagData1.UnlockData, gamertagData2.UnlockData);
          this.MergeSkillData(gamertagData1.SkillData, gamertagData2.SkillData);
        }
      }
    }

    private void MergeUnlockData(PlayerUnlockableData data, PlayerUnlockableData diskData)
    {
      data.AngelPlayersSaved = Math.Max(data.AngelPlayersSaved, diskData.AngelPlayersSaved);
      foreach (int num in diskData.AstronautWorldsVisited)
      {
        if (!data.AstronautWorldsVisited.Contains(num))
          data.AstronautWorldsVisited.Add(num);
      }
      data.CarpenterWorkbenchCrafted |= diskData.CarpenterWorkbenchCrafted;
      data.CavemanBlocksCleared = Math.Max(data.CavemanBlocksCleared, diskData.CavemanBlocksCleared);
      data.CowboyEnemiesKilled = Math.Max(data.CowboyEnemiesKilled, diskData.CowboyEnemiesKilled);
      data.DemiGodUnlocked |= diskData.DemiGodUnlocked;
      data.DiabloUnlocked |= diskData.DiabloUnlocked;
      data.EntrepreneurGoldEarned = Math.Max(data.EntrepreneurGoldEarned, diskData.EntrepreneurGoldEarned);
      data.EntrepreneurUnlocked |= diskData.EntrepreneurUnlocked;
      data.ExplorerBlueprintsFound = Math.Max(data.ExplorerBlueprintsFound, diskData.ExplorerBlueprintsFound);
      data.ExplorerWisdomsFound = Math.Max(data.ExplorerWisdomsFound, diskData.ExplorerWisdomsFound);
      data.ExplorerUnlocked |= diskData.ExplorerUnlocked;
      data.Girl2ShopPurchase |= diskData.Girl2ShopPurchase;
      data.GoldenKnightUnlocked |= diskData.GoldenKnightUnlocked;
      data.HippieFlowersThrownAtEnemy = Math.Max(data.HippieFlowersThrownAtEnemy, diskData.HippieFlowersThrownAtEnemy);
      data.IndianArrowsCrafted = Math.Max(data.IndianArrowsCrafted, diskData.IndianArrowsCrafted);
      data.IndianBowCrafted |= diskData.IndianBowCrafted;
      data.IndianEnemiesKilled = Math.Max(data.IndianEnemiesKilled, diskData.IndianEnemiesKilled);
      data.TotalInvadersScore = Math.Max(data.TotalInvadersScore, diskData.TotalInvadersScore);
      data.TotalRushScore = Math.Max(data.TotalRushScore, diskData.TotalRushScore);
      data.JamaicanUnlocked |= diskData.JamaicanUnlocked;
      data.KingUnlocked |= diskData.KingUnlocked;
      data.LumberJackSaplingsPlanted = Math.Max(data.LumberJackSaplingsPlanted, diskData.LumberJackSaplingsPlanted);
      data.LumberJackTreesChopped = Math.Max(data.LumberJackTreesChopped, diskData.LumberJackTreesChopped);
      data.MadmanDetonations = Math.Max(data.MadmanDetonations, diskData.MadmanDetonations);
      data.MedicHealedOther = Math.Max(data.MedicHealedOther, diskData.MedicHealedOther);
      data.MedicHealedSelf = Math.Max(data.MedicHealedSelf, diskData.MedicHealedSelf);
      foreach (int num in diskData.NinjaKillStreakGamerID)
      {
        if (!data.NinjaKillStreakGamerID.Contains(num))
          data.NinjaKillStreakGamerID.Add(num);
      }
      data.PirateChestsOpened = Math.Max(data.PirateChestsOpened, diskData.PirateChestsOpened);
      data.PrisonerUnlocked |= diskData.PrisonerUnlocked;
      data.RefugeePlayerKills = Math.Max(data.RefugeePlayerKills, diskData.RefugeePlayerKills);
      data.SageUnlocked |= diskData.SageUnlocked;
      data.SoldierGrenadeLauncherCrafted |= diskData.SoldierGrenadeLauncherCrafted;
      data.SoldierGrenadesCrafted = Math.Max(data.SoldierGrenadesCrafted, diskData.SoldierGrenadesCrafted);
      data.SoldierGrenadesLaunched = Math.Max(data.SoldierGrenadesLaunched, diskData.SoldierGrenadesLaunched);
      data.TerminatorEnemiesKilled = Math.Max(data.TerminatorEnemiesKilled, diskData.TerminatorEnemiesKilled);
      data.ZombieDaysSurvived = Math.Max(data.ZombieDaysSurvived, diskData.ZombieDaysSurvived);
    }

    private void MergeSkillData(CharacterSkillsData data, CharacterSkillsData diskData)
    {
      if (!this.SkipSkillMergeInternal)
      {
        for (int index = 0; index < data.SkillCount; ++index)
        {
          if (index < diskData.SkillCount)
          {
            SkillData skillData = data[index];
            skillData.SetCurrentXPRaw(Math.Max(skillData.CurrentXP, diskData[index].CurrentXP));
            data[index] = skillData;
          }
        }
      }
      this.SkipSkillMergeInternal = false;
    }
  }
}
