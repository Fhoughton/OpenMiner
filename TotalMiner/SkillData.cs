// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SkillData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System;

namespace StudioForge.TotalMiner
{
  internal struct SkillData
  {
    public const double IncFac = 1.0845;
    public const int SkillCount = 15;
    public const int MaxXP = 999999999;
    public SkillType SkillType;
    private double currentXP;
    private double nextLevelXP;
    private int level;
    public static double[] LevelXP;

    public double CurrentXP
    {
      get
      {
        return this.currentXP;
      }
      set
      {
        double num = value;
        if (num < 0.0 || num > 999999999.0)
          num = 999999999.0;
        this.SetNextLevelXP(this.currentXP == 0.0 ? num : this.currentXP);
        for (this.currentXP = num; this.currentXP >= this.nextLevelXP; this.nextLevelXP = (double) SkillData.GetXP(this.level + 1))
          ++this.level;
      }
    }

    public void SetCurrentXPRaw(double xp)
    {
      this.nextLevelXP = 0.0;
      if (xp < 0.0 || xp > 999999999.0)
        xp = 999999999.0;
      this.SetNextLevelXP(this.currentXP = xp);
    }

    public int Level
    {
      get
      {
        return this.level;
      }
    }

    public float LevelWithBonuses(Actor character)
    {
      return (float) (this.level + this.GetBonusLevels(character));
    }

    public int XPToNextLevel
    {
      get
      {
        this.SetNextLevelXP(this.CurrentXP);
        return (int) (this.nextLevelXP - this.CurrentXP);
      }
    }

    public int GetBonusLevels(Actor character)
    {
      float num = 0.0f;
      SkillBonusItemXML[] bonuses = Globals1.SkillBonusData[(int) this.SkillType].Bonuses;
      if (bonuses != null && character != null)
      {
        foreach (SkillBonusItemXML skillBonusItemXml in bonuses)
        {
          if (character.IsItemEquippedAndUsable(skillBonusItemXml.ItemID))
            num += skillBonusItemXml.Bonus;
        }
      }
      return (int) ((double) num + 0.5);
    }

    public static long GetXP(int level)
    {
      if (SkillData.LevelXP == null)
        SkillData.InitLevelXPArray();
      if (level > 0 && level < SkillData.LevelXP.Length)
        return (long) SkillData.LevelXP[level];
      if (level < 1)
        return 0;
      return SkillData.GetXPCore(level);
    }

    public static int CombatLevel(
      float healthLevel,
      float strengthLevel,
      float attackLevel,
      float defenceLevel,
      float rangedLevel)
    {
      return (int) ((double) Math.Max((float) ((double) attackLevel * 0.349999994039536 + (double) strengthLevel * 0.25 + (double) defenceLevel * 0.300000011920929), (float) ((double) rangedLevel * 0.600000023841858 + (double) defenceLevel * 0.300000011920929)) + (double) healthLevel * 0.100000001490116);
    }

    public static int CombatLevel(CombatStats stats)
    {
      return SkillData.CombatLevel((float) stats.HealthLevel, (float) stats.StrengthLevel, (float) stats.AttackLevel, (float) stats.DefenceLevel, (float) stats.RangedLevel);
    }

    public static float MaxHealth(int healthLevel)
    {
      int num = 10 + (Math.Min(healthLevel, 99) - 1) * 3;
      if (healthLevel > 99)
        num += healthLevel - 99;
      return (float) num;
    }

    private void SetNextLevelXP(double xp)
    {
      if (this.nextLevelXP != 0.0)
        return;
      this.level = SkillData.GetLevel(xp);
      this.nextLevelXP = (double) SkillData.GetXP(this.level + 1);
    }

    private static void InitLevelXPArray()
    {
      SkillData.LevelXP = new double[250];
      SkillData.LevelXP[1] = 0.0;
      for (int level = 2; level < SkillData.LevelXP.Length; ++level)
        SkillData.LevelXP[level] = (double) SkillData.GetXPCore(level);
    }

    private static long GetXPCore(int level)
    {
      int num1 = 1;
      double num2 = 60.0;
      double num3 = 60.0;
      for (; num1 < level - 1; ++num1)
      {
        double num4 = num2;
        num2 += num3 * 1.0845;
        num3 = num2 - num4;
      }
      return (long) num2;
    }

    public static int GetLevel(double xp)
    {
      int num1 = 1;
      double num2 = 60.0;
      double num3 = 60.0;
      while (xp >= num2)
      {
        double num4 = num2;
        num2 += num3 * 1.0845;
        num3 = num2 - num4;
        ++num1;
      }
      return num1;
    }
  }
}
