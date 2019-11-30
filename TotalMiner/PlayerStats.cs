// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PlayerStats
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.Storage;

namespace StudioForge.TotalMiner
{
  internal class PlayerStats
  {
    public double SecondsPlayed;
    public float DistanceWalked;
    public float DistanceFlown;
    public int BlocksCleared;
    public int BlocksPlaced;
    public int BlocksPickedUp;
    public int ItemsPickedUp;
    public float DamageDealt;
    public float DamageTaken;
    public int TotalKills;
    public int TotalDeaths;
    public int PlayerKills;
    public int NPCKills;
    public int LootValue;
    public int GrenadesLaunched;
    public int FillerInt1;
    public int FillerInt2;
    public int FillerInt3;
    public int FillerInt4;
    public int FillerInt5;
    public int FillerInt6;
    public int FillerInt7;
    public int FillerInt8;
    public int FillerInt9;
    public int FillerInt10;
    public float FillerFloat1;
    public float FillerFloat2;
    public float FillerFloat3;
    public float FillerFloat4;
    public float FillerFloat5;
    public GameInstance Instance;

    public PlayerStats Clone()
    {
      return new PlayerStats()
      {
        Instance = this.Instance,
        SecondsPlayed = this.SecondsPlayed,
        DistanceWalked = this.DistanceWalked,
        DistanceFlown = this.DistanceFlown,
        BlocksCleared = this.BlocksCleared,
        BlocksPlaced = this.BlocksPlaced,
        BlocksPickedUp = this.BlocksPickedUp,
        ItemsPickedUp = this.ItemsPickedUp,
        DamageDealt = this.DamageDealt,
        DamageTaken = this.DamageTaken,
        TotalKills = this.TotalKills,
        TotalDeaths = this.TotalDeaths,
        PlayerKills = this.PlayerKills,
        NPCKills = this.NPCKills,
        LootValue = this.LootValue,
        GrenadesLaunched = this.GrenadesLaunched,
        FillerInt1 = this.FillerInt1,
        FillerInt2 = this.FillerInt2,
        FillerInt3 = this.FillerInt3,
        FillerInt4 = this.FillerInt4,
        FillerInt5 = this.FillerInt5,
        FillerInt6 = this.FillerInt6,
        FillerInt7 = this.FillerInt7,
        FillerInt8 = this.FillerInt8,
        FillerInt9 = this.FillerInt9,
        FillerInt10 = this.FillerInt10,
        FillerFloat1 = this.FillerFloat1,
        FillerFloat2 = this.FillerFloat2,
        FillerFloat3 = this.FillerFloat3,
        FillerFloat4 = this.FillerFloat4,
        FillerFloat5 = this.FillerFloat5
      };
    }

    public PlayerStats.Stat[] GetPlayerStatsAsText()
    {
      PlayerStats.Stat[] statArray = new PlayerStats.Stat[15];
      if (this.SecondsPlayed > 7200.0)
      {
        statArray[0].Desc = "Hours Played";
        statArray[0].Value = (this.SecondsPlayed / 3600.0).ToString("N1");
      }
      else
      {
        statArray[0].Desc = "Minutes Played";
        statArray[0].Value = ((int) (this.SecondsPlayed / 60.0)).ToString();
      }
      statArray[1].Desc = "Distance Walked";
      statArray[1].Value = ((int) this.DistanceWalked).ToString();
      statArray[2].Desc = "Distance Flown";
      statArray[2].Value = ((int) this.DistanceFlown).ToString();
      statArray[3].Desc = "Blocks Cleared";
      statArray[3].Value = this.BlocksCleared.ToString();
      statArray[4].Desc = "Blocks Placed";
      statArray[4].Value = this.BlocksPlaced.ToString();
      statArray[5].Desc = "Blocks Picked Up";
      statArray[5].Value = this.BlocksPickedUp.ToString();
      statArray[6].Desc = "Items Picked Up";
      statArray[6].Value = this.ItemsPickedUp.ToString();
      statArray[7].Desc = "Damage Taken";
      statArray[7].Value = ((int) this.DamageTaken).ToString();
      statArray[8].Desc = "Damage Dealt";
      statArray[8].Value = ((int) this.DamageDealt).ToString();
      statArray[9].Desc = "Total Deaths";
      statArray[9].Value = this.TotalDeaths.ToString();
      statArray[10].Desc = "Total Kills";
      statArray[10].Value = this.TotalKills.ToString();
      statArray[11].Desc = "Player Kills";
      statArray[11].Value = this.PlayerKills.ToString();
      statArray[12].Desc = "NPC Kills";
      statArray[12].Value = this.NPCKills.ToString();
      statArray[13].Desc = "Loot Value";
      statArray[13].Value = this.LootValue.ToString();
      statArray[14].Desc = "Grenades Launched";
      statArray[14].Value = this.GrenadesLaunched.ToString();
      return statArray;
    }

    public void Merge(PlayerStats stats)
    {
      if (stats == null)
        return;
      if (stats.SecondsPlayed > this.SecondsPlayed)
        this.SecondsPlayed = stats.SecondsPlayed;
      if ((double) stats.DistanceWalked > (double) this.DistanceWalked)
        this.DistanceWalked = stats.DistanceWalked;
      if ((double) stats.DistanceFlown > (double) this.DistanceFlown)
        this.DistanceFlown = stats.DistanceFlown;
      if (stats.BlocksCleared > this.BlocksCleared)
        this.BlocksCleared = stats.BlocksCleared;
      if (stats.BlocksPlaced > this.BlocksPlaced)
        this.BlocksPlaced = stats.BlocksPlaced;
      if (stats.BlocksPickedUp > this.BlocksPickedUp)
        this.BlocksPickedUp = stats.BlocksPickedUp;
      if (stats.ItemsPickedUp > this.ItemsPickedUp)
        this.ItemsPickedUp = stats.ItemsPickedUp;
      if ((double) stats.DamageDealt > (double) this.DamageDealt)
        this.DamageDealt = stats.DamageDealt;
      if ((double) stats.DamageTaken > (double) this.DamageTaken)
        this.DamageTaken = stats.DamageTaken;
      if (stats.TotalKills > this.TotalKills)
        this.TotalKills = stats.TotalKills;
      if (stats.TotalDeaths > this.TotalDeaths)
        this.TotalDeaths = stats.TotalDeaths;
      if (stats.PlayerKills > this.PlayerKills)
        this.PlayerKills = stats.PlayerKills;
      if (stats.NPCKills > this.NPCKills)
        this.NPCKills = stats.NPCKills;
      if (stats.LootValue > this.LootValue)
        this.LootValue = stats.LootValue;
      if (stats.GrenadesLaunched <= this.GrenadesLaunched)
        return;
      this.GrenadesLaunched = stats.GrenadesLaunched;
    }

    public static PlayerStats.Stat[] GetMapStatsAsText(GameInstance instance)
    {
      PlayerStats.Stat[] statArray1 = new PlayerStats.Stat[13];
      if (instance == null || instance.Map == null || (Globals2.GameProperties == null || Globals2.GameProperties.SaveGame == null))
        return statArray1;
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      instance.RefreshPlayerSaveStats();
      int index1 = 0;
      statArray1[index1].Desc = "Creator";
      PlayerStats.Stat[] statArray2 = statArray1;
      int index2 = index1;
      int index3 = index2 + 1;
      statArray2[index2].Value = header.OwnerGamerTag;
      statArray1[index3].Desc = "Seed";
      PlayerStats.Stat[] statArray3 = statArray1;
      int index4 = index3;
      int index5 = index4 + 1;
      statArray3[index4].Value = instance.Map.Seed.ToString();
      statArray1[index5].Desc = "Mode";
      PlayerStats.Stat[] statArray4 = statArray1;
      int index6 = index5;
      int index7 = index6 + 1;
      statArray4[index6].Value = header.GameMode.ToString();
      statArray1[index7].Desc = "Attribute";
      PlayerStats.Stat[] statArray5 = statArray1;
      int index8 = index7;
      int index9 = index8 + 1;
      statArray5[index8].Value = header.Attribute.ToString();
      statArray1[index9].Desc = "Current Season";
      PlayerStats.Stat[] statArray6 = statArray1;
      int index10 = index9;
      int index11 = index10 + 1;
      statArray6[index10].Value = instance.SunMoon != null ? instance.SunMoon.Season.ToString() : "None";
      statArray1[index11].Desc = "World Visitors";
      PlayerStats.Stat[] statArray7 = statArray1;
      int index12 = index11;
      int index13 = index12 + 1;
      statArray7[index12].Value = (instance.PlayerCountToSave - 1).ToString();
      statArray1[index13].Desc = "World Rating";
      PlayerStats.Stat[] statArray8 = statArray1;
      int index14 = index13;
      int index15 = index14 + 1;
      statArray8[index14].Value = string.Format("{0:N1}", (object) header.RatingStars);
      statArray1[index15].Desc = "# of Ratings";
      PlayerStats.Stat[] statArray9 = statArray1;
      int index16 = index15;
      int index17 = index16 + 1;
      statArray9[index16].Value = header.RatingCount.ToString();
      statArray1[index17].Desc = "Most Concurrent Players";
      PlayerStats.Stat[] statArray10 = statArray1;
      int index18 = index17;
      int index19 = index18 + 1;
      statArray10[index18].Value = Globals2.MaxConcurrentPlayers.ToString();
      statArray1[index19].Desc = "Total Man Hours Played";
      int num1 = 0;
      foreach (SavePlayerState playerSave in instance.PlayerSaves)
        num1 += (int) playerSave.Statistics.SecondsPlayed;
      PlayerStats.Stat[] statArray11 = statArray1;
      int index20 = index19;
      int index21 = index20 + 1;
      statArray11[index20].Value = ((int) ((double) num1 / 3600.0)).ToString("N1");
      statArray1[index21].Desc = "Blocks Cleared";
      int num2 = 0;
      foreach (SavePlayerState playerSave in instance.PlayerSaves)
        num2 += playerSave.Statistics.BlocksCleared;
      PlayerStats.Stat[] statArray12 = statArray1;
      int index22 = index21;
      int index23 = index22 + 1;
      statArray12[index22].Value = num2.ToString();
      statArray1[index23].Desc = "Blocks Placed";
      num2 = 0;
      foreach (SavePlayerState playerSave in instance.PlayerSaves)
        num2 += playerSave.Statistics.BlocksPlaced;
      PlayerStats.Stat[] statArray13 = statArray1;
      int index24 = index23;
      int index25 = index24 + 1;
      statArray13[index24].Value = num2.ToString();
      statArray1[index25].Desc = "Hours Slept";
      PlayerStats.Stat[] statArray14 = statArray1;
      int index26 = index25;
      int num3 = index26 + 1;
      statArray14[index26].Value = header.HoursSlept.ToString();
      return statArray1;
    }

    public bool IsEqual(PlayerStats stats)
    {
      if ((double) this.DistanceWalked == (double) stats.DistanceWalked && (double) this.DistanceFlown == (double) stats.DistanceFlown && (this.BlocksCleared == stats.BlocksCleared && this.BlocksPlaced == stats.BlocksPlaced) && (this.BlocksPickedUp == stats.BlocksPickedUp && this.ItemsPickedUp == stats.ItemsPickedUp && ((double) this.DamageDealt == (double) stats.DamageDealt && (double) this.DamageTaken == (double) stats.DamageTaken)) && (this.TotalDeaths == stats.TotalDeaths && this.TotalKills == stats.TotalKills && this.LootValue == stats.LootValue))
        return this.GrenadesLaunched == stats.GrenadesLaunched;
      return false;
    }

    public struct Stat
    {
      public string Desc;
      public string Value;
    }
  }
}
