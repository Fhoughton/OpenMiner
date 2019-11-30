// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.PlayerUnlockableData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GamerServices;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class PlayerUnlockableData
  {
    public readonly string Gamertag;
    public BadBoyType BadBoy;
    public int AngelPlayersSaved;
    public List<int> AstronautWorldsVisited;
    public bool CarpenterWorkbenchCrafted;
    public int CavemanBlocksCleared;
    public bool[] ChefActions;
    public int CowboyEnemiesKilled;
    public bool DemiGodUnlocked;
    public bool DiabloUnlocked;
    public int EntrepreneurGoldEarned;
    public bool EntrepreneurUnlocked;
    public int ExplorerBlueprintsFound;
    public int ExplorerWisdomsFound;
    public bool ExplorerUnlocked;
    public bool Girl2ShopPurchase;
    public int GoldenKnightRatesReceived;
    public bool GoldenKnightUnlocked;
    public bool[] HandymanActions;
    public int HippieFlowersThrownAtEnemy;
    public int IndianArrowsCrafted;
    public bool IndianBowCrafted;
    public int IndianEnemiesKilled;
    public int TotalInvadersScore;
    public int TotalRushScore;
    public bool JamaicanUnlocked;
    public bool KingUnlocked;
    public bool KnightBedrockReached;
    public int KnightEnemiesKilled;
    public bool KnightUnlocked;
    public int LumberJackSaplingsPlanted;
    public int LumberJackTreesChopped;
    public int LumberJackWoodPlanksCrafted;
    public int MadmanDetonations;
    public int MedicHealedOther;
    public int MedicHealedSelf;
    public List<int> NinjaKillStreakGamerID;
    public int PirateChestsOpened;
    public bool PrisonerUnlocked;
    public bool[][] PupilHowToRead;
    public int RefugeePlayerKills;
    public int SageWisdomsFound;
    public bool SageUnlocked;
    public bool SoldierGrenadeLauncherCrafted;
    public int SoldierGrenadesCrafted;
    public int SoldierGrenadesLaunched;
    public int TerminatorEnemiesKilled;
    public int TreeHuggerTreesChopped;
    public int ZombieDaysSurvived;
    public int DemiBlocksMined;
    public int DemiBlocksPlaced;
    public int DemiRatesGiven;
    public int DemiRatesReceived;
    public List<int> DemiWorldsVisited;
    public List<int> DemiWorldsVisited10Mins;
    public List<string> DemiFavoriters;

    public PlayerUnlockableData(Gamer gamer)
      : this(gamer.Gamertag)
    {
    }

    public PlayerUnlockableData(string gamertag)
    {
      this.Gamertag = gamertag;
      this.NinjaKillStreakGamerID = new List<int>();
      this.AstronautWorldsVisited = new List<int>();
      this.ChefActions = new bool[594];
      this.HandymanActions = new bool[594];
      this.DemiWorldsVisited = new List<int>();
      this.DemiWorldsVisited10Mins = new List<int>();
      this.DemiFavoriters = new List<string>();
      this.PupilHowToRead = PlayerUnlockableData.InitHowToUnlockData();
    }

    public static bool[][] InitHowToUnlockData()
    {
      bool[][] flagArray = new bool[4][];
      for (int i = 0; i < flagArray.Length; ++i)
        flagArray[i] = new bool[PlayerUnlockableData.GetMaxHowToUnlockIDCount(i) + 1];
      return flagArray;
    }

    private static int GetMaxHowToUnlockIDCount(int i)
    {
      switch (i)
      {
        case 1:
          return 9;
        case 2:
          return 3;
        case 3:
          return 4;
        default:
          return -1;
      }
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.AngelPlayersSaved = reader.ReadInt32();
      this.CarpenterWorkbenchCrafted = reader.ReadBoolean();
      this.CavemanBlocksCleared = reader.ReadInt32();
      this.CowboyEnemiesKilled = reader.ReadInt32();
      this.DemiGodUnlocked = reader.ReadBoolean();
      this.DiabloUnlocked = reader.ReadBoolean();
      this.EntrepreneurGoldEarned = reader.ReadInt32();
      this.EntrepreneurUnlocked = reader.ReadBoolean();
      this.ExplorerBlueprintsFound = reader.ReadInt32();
      this.ExplorerWisdomsFound = reader.ReadInt32();
      this.ExplorerUnlocked = reader.ReadBoolean();
      this.Girl2ShopPurchase = reader.ReadBoolean();
      if (version < 246)
        reader.ReadInt32();
      this.GoldenKnightUnlocked = reader.ReadBoolean();
      this.HippieFlowersThrownAtEnemy = reader.ReadInt32();
      this.IndianArrowsCrafted = reader.ReadInt32();
      this.IndianBowCrafted = reader.ReadBoolean();
      this.IndianEnemiesKilled = reader.ReadInt32();
      this.TotalInvadersScore = reader.ReadInt32();
      if (version > 222)
        this.TotalRushScore = reader.ReadInt32();
      if (version < 232)
        this.TotalRushScore = 0;
      this.JamaicanUnlocked = reader.ReadBoolean();
      this.KingUnlocked = reader.ReadBoolean();
      this.KnightBedrockReached = reader.ReadBoolean();
      this.KnightEnemiesKilled = reader.ReadInt32();
      this.KnightUnlocked = reader.ReadBoolean();
      this.LumberJackSaplingsPlanted = reader.ReadInt32();
      this.LumberJackTreesChopped = reader.ReadInt32();
      this.LumberJackWoodPlanksCrafted = reader.ReadInt32();
      this.MadmanDetonations = reader.ReadInt32();
      this.MedicHealedOther = reader.ReadInt32();
      this.MedicHealedSelf = reader.ReadInt32();
      this.PirateChestsOpened = reader.ReadInt32();
      this.PrisonerUnlocked = reader.ReadBoolean();
      this.RefugeePlayerKills = reader.ReadInt32();
      this.SageWisdomsFound = reader.ReadInt32();
      this.SageUnlocked = reader.ReadBoolean();
      this.SoldierGrenadeLauncherCrafted = reader.ReadBoolean();
      this.SoldierGrenadesCrafted = reader.ReadInt32();
      this.SoldierGrenadesLaunched = reader.ReadInt32();
      this.TerminatorEnemiesKilled = reader.ReadInt32();
      this.TreeHuggerTreesChopped = reader.ReadInt32();
      this.ZombieDaysSurvived = reader.ReadInt32();
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.AstronautWorldsVisited.Add(reader.ReadInt32());
      int num2 = reader.ReadInt32();
      for (int index = 0; index < num2; ++index)
        this.ChefActions[index] = reader.ReadBoolean();
      int num3 = reader.ReadInt32();
      for (int index = 0; index < num3; ++index)
        this.HandymanActions[index] = reader.ReadBoolean();
      int num4 = reader.ReadInt32();
      for (int index = 0; index < num4; ++index)
        this.NinjaKillStreakGamerID.Add(reader.ReadInt32());
      int num5 = reader.ReadInt32();
      for (int index = 0; index < num5; ++index)
        this.ReadHowToUnlockData(reader, version, this.PupilHowToRead);
      if (version <= 253)
        return;
      this.DemiBlocksMined = reader.ReadInt32();
      this.DemiBlocksPlaced = reader.ReadInt32();
      this.DemiRatesGiven = reader.ReadInt32();
      this.DemiRatesReceived = reader.ReadInt32();
      int capacity1 = reader.ReadInt32();
      this.DemiWorldsVisited = new List<int>(capacity1);
      for (int index = 0; index < capacity1; ++index)
        this.DemiWorldsVisited.Add(reader.ReadInt32());
      int capacity2 = reader.ReadInt32();
      this.DemiWorldsVisited10Mins = new List<int>(capacity2);
      for (int index = 0; index < capacity2; ++index)
        this.DemiWorldsVisited10Mins.Add(reader.ReadInt32());
      int capacity3 = reader.ReadInt32();
      this.DemiFavoriters = new List<string>(capacity3);
      for (int index = 0; index < capacity3; ++index)
        this.DemiFavoriters.Add(reader.ReadString());
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

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.AngelPlayersSaved);
      writer.Write(this.CarpenterWorkbenchCrafted);
      writer.Write(this.CavemanBlocksCleared);
      writer.Write(this.CowboyEnemiesKilled);
      writer.Write(this.DemiGodUnlocked);
      writer.Write(this.DiabloUnlocked);
      writer.Write(this.EntrepreneurGoldEarned);
      writer.Write(this.EntrepreneurUnlocked);
      writer.Write(this.ExplorerBlueprintsFound);
      writer.Write(this.ExplorerWisdomsFound);
      writer.Write(this.ExplorerUnlocked);
      writer.Write(this.Girl2ShopPurchase);
      writer.Write(this.GoldenKnightUnlocked);
      writer.Write(this.HippieFlowersThrownAtEnemy);
      writer.Write(this.IndianArrowsCrafted);
      writer.Write(this.IndianBowCrafted);
      writer.Write(this.IndianEnemiesKilled);
      writer.Write(this.TotalInvadersScore);
      writer.Write(this.TotalRushScore);
      writer.Write(this.JamaicanUnlocked);
      writer.Write(this.KingUnlocked);
      writer.Write(this.KnightBedrockReached);
      writer.Write(this.KnightEnemiesKilled);
      writer.Write(this.KnightUnlocked);
      writer.Write(this.LumberJackSaplingsPlanted);
      writer.Write(this.LumberJackTreesChopped);
      writer.Write(this.LumberJackWoodPlanksCrafted);
      writer.Write(this.MadmanDetonations);
      writer.Write(this.MedicHealedOther);
      writer.Write(this.MedicHealedSelf);
      writer.Write(this.PirateChestsOpened);
      writer.Write(this.PrisonerUnlocked);
      writer.Write(this.RefugeePlayerKills);
      writer.Write(this.SageWisdomsFound);
      writer.Write(this.SageUnlocked);
      writer.Write(this.SoldierGrenadeLauncherCrafted);
      writer.Write(this.SoldierGrenadesCrafted);
      writer.Write(this.SoldierGrenadesLaunched);
      writer.Write(this.TerminatorEnemiesKilled);
      writer.Write(this.TreeHuggerTreesChopped);
      writer.Write(this.ZombieDaysSurvived);
      writer.Write(this.AstronautWorldsVisited.Count);
      for (int index = 0; index < this.AstronautWorldsVisited.Count; ++index)
        writer.Write(this.AstronautWorldsVisited[index]);
      writer.Write(this.ChefActions.Length);
      for (int index = 0; index < this.ChefActions.Length; ++index)
        writer.Write(this.ChefActions[index]);
      writer.Write(this.HandymanActions.Length);
      for (int index = 0; index < this.HandymanActions.Length; ++index)
        writer.Write(this.HandymanActions[index]);
      lock (this.NinjaKillStreakGamerID)
      {
        writer.Write(this.NinjaKillStreakGamerID.Count);
        foreach (int num in this.NinjaKillStreakGamerID)
          writer.Write(num);
      }
      writer.Write(this.PupilHowToRead.Length);
      for (int index1 = 0; index1 < this.PupilHowToRead.Length; ++index1)
      {
        writer.Write(index1);
        writer.Write(this.PupilHowToRead[index1].Length);
        for (int index2 = 0; index2 < this.PupilHowToRead[index1].Length; ++index2)
          writer.Write(this.PupilHowToRead[index1][index2]);
      }
      writer.Write(this.DemiBlocksMined);
      writer.Write(this.DemiBlocksPlaced);
      writer.Write(this.DemiRatesGiven);
      writer.Write(this.DemiRatesReceived);
      writer.Write(this.DemiWorldsVisited.Count);
      foreach (int num in this.DemiWorldsVisited)
        writer.Write(num);
      writer.Write(this.DemiWorldsVisited10Mins.Count);
      foreach (int worldsVisited10Min in this.DemiWorldsVisited10Mins)
        writer.Write(worldsVisited10Min);
      writer.Write(this.DemiFavoriters.Count);
      foreach (string demiFavoriter in this.DemiFavoriters)
        writer.Write(demiFavoriter);
    }
  }
}
