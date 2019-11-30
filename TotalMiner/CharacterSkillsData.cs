// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CharacterSkillsData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StudioForge.TotalMiner
{
  internal class CharacterSkillsData
  {
    private static string[] UseDesc1 = new string[16]
    {
      "not used",
      "not used",
      "not used",
      "not used",
      "not used",
      "not used",
      "mine",
      "dig",
      "chop",
      "build with",
      "craft",
      "smelt",
      "smith",
      "farm",
      "cook",
      "not used"
    };
    private static string[] UseDesc2 = new string[16]
    {
      "not used",
      "use",
      "use",
      "use",
      "use",
      "use",
      "use",
      "use",
      "use",
      "not used",
      "not used",
      "not used",
      "not used",
      "use",
      "not used",
      "not used"
    };
    private const float levelUpScaleHead = 2.2f;
    private const float levelUpScaleDesc = 1.5f;
    public SkillType MostRecentlyUsedSkill;
    private SkillData[] skillData;
    private Stopwatch messageTimer;

    public SkillData Health
    {
      get
      {
        return this.skillData[1];
      }
      set
      {
        this.skillData[1] = value;
      }
    }

    public SkillData Strength
    {
      get
      {
        return this.skillData[2];
      }
      set
      {
        this.skillData[2] = value;
      }
    }

    public SkillData Attack
    {
      get
      {
        return this.skillData[3];
      }
      set
      {
        this.skillData[3] = value;
      }
    }

    public SkillData Defence
    {
      get
      {
        return this.skillData[4];
      }
      set
      {
        this.skillData[4] = value;
      }
    }

    public SkillData Ranged
    {
      get
      {
        return this.skillData[5];
      }
      set
      {
        this.skillData[5] = value;
      }
    }

    public SkillData Mining
    {
      get
      {
        return this.skillData[6];
      }
      set
      {
        this.skillData[6] = value;
      }
    }

    public SkillData Digging
    {
      get
      {
        return this.skillData[7];
      }
      set
      {
        this.skillData[7] = value;
      }
    }

    public SkillData Chopping
    {
      get
      {
        return this.skillData[8];
      }
      set
      {
        this.skillData[8] = value;
      }
    }

    public SkillData Building
    {
      get
      {
        return this.skillData[9];
      }
      set
      {
        this.skillData[9] = value;
      }
    }

    public SkillData Crafting
    {
      get
      {
        return this.skillData[10];
      }
      set
      {
        this.skillData[10] = value;
      }
    }

    public SkillData Smelting
    {
      get
      {
        return this.skillData[11];
      }
      set
      {
        this.skillData[11] = value;
      }
    }

    public SkillData Smithing
    {
      get
      {
        return this.skillData[12];
      }
      set
      {
        this.skillData[12] = value;
      }
    }

    public SkillData Farming
    {
      get
      {
        return this.skillData[13];
      }
      set
      {
        this.skillData[13] = value;
      }
    }

    public SkillData Cooking
    {
      get
      {
        return this.skillData[14];
      }
      set
      {
        this.skillData[14] = value;
      }
    }

    public SkillData Looting
    {
      get
      {
        return this.skillData[15];
      }
      set
      {
        this.skillData[15] = value;
      }
    }

    public SkillData this[int i]
    {
      get
      {
        if (i < 0 || i >= this.skillData.Length)
          i = 0;
        return this.skillData[i];
      }
      set
      {
        if (i < 0 || i >= this.skillData.Length)
          return;
        this.skillData[i] = value;
      }
    }

    public int SkillCount
    {
      get
      {
        return this.skillData.Length;
      }
    }

    public void SetLevel(SkillType type, int level)
    {
      int index = (int) type;
      if (index < 0 || index >= this.skillData.Length)
        return;
      SkillData skillData = this.skillData[index];
      skillData.SetCurrentXPRaw((double) (SkillData.GetXP((int) MathHelper.Clamp((float) level, 1f, 184f)) + 1L));
      this.skillData[index] = skillData;
    }

    public void SetXPExternal(Player player, SkillType type, double xp, bool display)
    {
      int index = (int) type;
      if (index < 0 || index >= this.skillData.Length)
        return;
      SkillData skillData = this.skillData[index];
      int level = skillData.Level;
      skillData.SetCurrentXPRaw(xp);
      this.skillData[index] = skillData;
      this.MostRecentlyUsedSkill = type;
      if (!display || skillData.Level <= level || (player == null || !player.IsLocalGamer))
        return;
      this.LevelUp(player, skillData, skillData.Level, Color.Cyan, player.GetScreenMatrix(true), false, false);
    }

    private bool IsSkillsEnabled
    {
      get
      {
        if (Globals2.GameProperties.SaveGame.Header.SkillsEnabled)
          return Globals2.GameProperties.SaveGame.Header.FiniteMode;
        return false;
      }
    }

    private bool IsSkillsLocal
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      }
    }

    private bool IsSkillsGlobal
    {
      get
      {
        return !Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      }
    }

    public SkillData GetUseSkill(StudioForge.TotalMiner.Item itemID)
    {
      return this[(int) Globals1.SkillData[(int) itemID].UseSkill];
    }

    public SkillData GetCraftSkill(StudioForge.TotalMiner.Item itemID)
    {
      return this[(int) Globals1.SkillData[(int) itemID].CraftSkill];
    }

    public int TotalLevel
    {
      get
      {
        int num = 0;
        for (int index = 1; index < this.skillData.Length; ++index)
          num += this.skillData[index].Level;
        return num;
      }
    }

    public double TotalXP
    {
      get
      {
        double num = 0.0;
        for (int index = 1; index < this.skillData.Length; ++index)
          num += this.skillData[index].CurrentXP;
        return num;
      }
    }

    public int CombatLevel
    {
      get
      {
        return SkillData.CombatLevel((float) this.Health.Level, (float) this.Strength.Level, (float) this.Attack.Level, (float) this.Defence.Level, (float) this.Ranged.Level);
      }
    }

    public CharacterSkillsData()
    {
      this.skillData = new SkillData[16];
      for (int index = 0; index < this.skillData.Length; ++index)
      {
        SkillData skillData = new SkillData()
        {
          SkillType = (SkillType) index
        };
        skillData.SetCurrentXPRaw(0.0);
        this.skillData[index] = skillData;
      }
      this.Initialize();
    }

    public CharacterSkillsData(CharacterSkillsData copy)
    {
      this.skillData = new SkillData[copy.skillData.Length];
      Array.Copy((Array) copy.skillData, (Array) this.skillData, this.skillData.Length);
      this.MostRecentlyUsedSkill = copy.MostRecentlyUsedSkill;
      this.Initialize();
    }

    public CharacterSkillsData(HighScoreItem data)
    {
      this.skillData = new SkillData[16];
      for (int index = 1; index < this.skillData.Length && index <= data.XPList.Length; ++index)
      {
        SkillData skillData = new SkillData()
        {
          SkillType = (SkillType) index
        };
        skillData.SetCurrentXPRaw((double) data.XPList[index - 1]);
        this.skillData[index] = skillData;
      }
      this.Initialize();
    }

    private void Initialize()
    {
      this.messageTimer = new Stopwatch();
      this.messageTimer.Start();
    }

    public bool UseReqsMet(Actor actor, StudioForge.TotalMiner.Item itemID)
    {
      if (!this.IsSkillsEnabled || actor.IsGod)
        return true;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      SkillData skillData = this[(int) skillDataXml.UseSkill];
      if ((double) skillData.LevelWithBonuses(actor) >= (double) skillDataXml.UseReq)
        return true;
      if (actor.IsPlayer && actor.IsLocalGamer)
        this.DisplayMessage(skillData.SkillType.ToString() + " Level " + skillDataXml.UseReq.ToString() + " is required to use this " + ItemData.ToString(itemID), 1000, -80, -40f, 3f, 1.2f, Color.Red, actor.GetScreenMatrix(true));
      return false;
    }

    public bool MineReqsMet(Actor actor, Block blockID, StudioForge.TotalMiner.Item itemID)
    {
      if (!this.IsSkillsEnabled || actor.IsGod)
        return true;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) blockID];
      SkillType skillType = Globals1.SkillData[(int) itemID].UseSkill;
      switch (skillType)
      {
        case SkillType.Digging:
        case SkillType.Chopping:
        case SkillType.Farming:
          SkillData skillData = this[(int) skillType];
          if ((double) skillData.LevelWithBonuses(actor) >= (double) skillDataXml.MineReq)
            return true;
          if (actor.IsPlayer && actor.IsLocalGamer)
            this.DisplayMessage(((int) skillData.SkillType).ToString() + " Level " + skillDataXml.MineReq.ToString() + " is required to " + CharacterSkillsData.UseDesc1[(int) skillData.SkillType] + " " + ItemData.ToString(blockID), 1000, -80, -40f, 3f, 1.2f, Color.Red, actor.GetScreenMatrix(true));
          return false;
        default:
          skillType = SkillType.Mining;
          goto case SkillType.Digging;
      }
    }

    public bool CraftReqsMet(Actor actor, StudioForge.TotalMiner.Item itemID)
    {
      if (!this.IsSkillsEnabled || actor.IsGod)
        return true;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      SkillData skillData = this[(int) skillDataXml.UseSkill];
      if ((double) skillData.LevelWithBonuses(actor) >= (double) skillDataXml.CraftReq)
        return true;
      if (actor.IsPlayer && actor.IsLocalGamer)
        this.DisplayMessage(skillData.SkillType.ToString() + " Level " + skillDataXml.CraftReq.ToString() + " is required to " + CharacterSkillsData.UseDesc1[(int) skillData.SkillType] + " this item", 1000, -80, -40f, 3f, 1.2f, Color.Red, actor.GetScreenMatrix(true));
      return false;
    }

    public void BlockMined(Actor actor, StudioForge.TotalMiner.Item tool, MapBlock data)
    {
      if (!this.IsSkillsEnabled || !actor.IsLocalGamer || actor.IsRubberBanding)
        return;
      if (tool == StudioForge.TotalMiner.Item.None)
        tool = StudioForge.TotalMiner.Item.Hand;
      SkillDataXML itemDataForTool = this.GetItemDataForTool(ref tool, (Block) data.BlockID);
      SkillDataXML skillDataXml = Globals1.SkillData[(int) data.BlockID];
      SkillData skillData = this[(int) itemDataForTool.UseSkill];
      this.MostRecentlyUsedSkill = itemDataForTool.UseSkill;
      int level1 = skillData.Level;
      float num = skillDataXml.MineExp * itemDataForTool.UseExp * CharacterSkillsData.GetToolXPModifier(tool) * this.GetBlockMinedXPModifier(actor, data) * this.GetGeneralXPModifier(actor, tool, true);
      skillData.CurrentXP += (double) num;
      this[(int) itemDataForTool.UseSkill] = skillData;
      if (this.IsSkillsGlobal)
        Globals2.HighscoreDataChanged = true;
      Matrix screenMatrix = actor.GetScreenMatrix(true);
      if (actor.IsPlayer)
      {
        Player player = actor as Player;
        this.DisplayXPGainedMessage(player, string.Format("+{0:N1}", (object) num), 0, 0, -60f, 1f, 0.75f, Color.Yellow, screenMatrix);
        int level2 = skillData.Level;
        if (level2 > level1)
          this.LevelUp(player, skillData, level2, Color.Cyan, screenMatrix, true, true);
      }
      this.StrengthGained(actor, (double) num * 0.14, screenMatrix);
    }

    public void BlockPlaced(Actor actor, StudioForge.TotalMiner.Item itemID, byte aux)
    {
      if (!this.IsSkillsEnabled || !actor.IsLocalGamer || actor.IsRubberBanding)
        return;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      SkillData skillData = this[(int) skillDataXml.UseSkill];
      this.MostRecentlyUsedSkill = skillDataXml.UseSkill;
      Block blockId = (Block) ItemData.ConvertItemIDToBlockID(itemID);
      int level1 = skillData.Level;
      float num = skillDataXml.UseExp * CharacterSkillsData.GetBlockXPModifier(actor.GameInstance.Map, blockId) * this.GetGeneralXPModifier(actor, itemID, true);
      skillData.CurrentXP += (double) num;
      this[(int) skillDataXml.UseSkill] = skillData;
      if (this.IsSkillsGlobal)
        Globals2.HighscoreDataChanged = true;
      Matrix screenMatrix = actor.GetScreenMatrix(true);
      if (actor.IsPlayer)
      {
        Player player = actor as Player;
        this.DisplayXPGainedMessage(player, string.Format("+{0:N1}", (object) num), 0, 0, -60f, 1f, 0.75f, Color.Yellow, screenMatrix);
        int level2 = skillData.Level;
        if (level2 > level1)
          this.LevelUp(player, skillData, level2, Color.Cyan, screenMatrix, true, false);
      }
      this.StrengthGained(actor, (double) num * 0.14, screenMatrix);
    }

    public void StrikeCharacter(Actor actor, StudioForge.TotalMiner.Item itemID, float damage)
    {
      if (!this.IsSkillsEnabled || !actor.IsLocalGamer || actor.IsRubberBanding)
        return;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      SkillType skillType = skillDataXml.UseSkill;
      float num1 = skillDataXml.UseExp;
      switch (skillType)
      {
        case SkillType.Strength:
        case SkillType.Attack:
        case SkillType.Defence:
        case SkillType.Ranged:
          SkillData skillData = this[(int) skillType];
          this.MostRecentlyUsedSkill = skillType;
          int level1 = skillData.Level;
          double num2 = (double) num1 * (double) damage / 12.0 * (double) this.GetGeneralXPModifier(actor, itemID, true);
          skillData.CurrentXP += num2;
          this[(int) skillType] = skillData;
          if (this.IsSkillsGlobal)
            Globals2.HighscoreDataChanged = true;
          Matrix screenMatrix = actor.GetScreenMatrix(true);
          if (actor.IsPlayer)
          {
            Player player = actor as Player;
            this.DisplayXPGainedMessage(player, string.Format("+{0:N1}", (object) num2), 0, -50, -60f, 1f, 0.75f, Color.Yellow, screenMatrix);
            int level2 = skillData.Level;
            if (level2 > level1)
              this.LevelUp(player, skillData, level2, Color.Cyan, screenMatrix, false, true);
          }
          this.StrengthGained(actor, num2 * 0.14, screenMatrix);
          this.HealthGained(actor, num2 * 0.3, screenMatrix);
          break;
        default:
          skillType = SkillType.Attack;
          num1 = 1f;
          goto case SkillType.Strength;
      }
    }

    public void ItemCrafted(Player player, StudioForge.TotalMiner.Item itemID)
    {
      if (!this.IsSkillsEnabled || !player.IsLocalGamer)
        return;
      SkillDataXML skillDataXml = Globals1.SkillData[(int) itemID];
      SkillData skillData = this[(int) skillDataXml.CraftSkill];
      this.MostRecentlyUsedSkill = skillDataXml.CraftSkill;
      int level1 = skillData.Level;
      float num = skillDataXml.CraftExp * this.GetGeneralXPModifier((Actor) player, itemID, true);
      skillData.CurrentXP += (double) num;
      this[(int) skillDataXml.CraftSkill] = skillData;
      if (this.IsSkillsGlobal)
        Globals2.HighscoreDataChanged = true;
      Matrix screenMatrix = player.GetScreenMatrix(true);
      this.DisplayXPGainedMessage(player, string.Format("+{0:N1}", (object) num), 0, new Vector2(-24f, -136f), -60f, 1f, 0.75f, Color.Yellow, screenMatrix);
      int level2 = skillData.Level;
      if (level2 <= level1)
        return;
      this.LevelUp(player, skillData, level2, Color.Cyan, screenMatrix, true, false);
    }

    public void LootGained(Player player, InventoryItem item)
    {
      if (!this.IsSkillsEnabled || !player.IsLocalGamer || (player.IsRubberBanding || item.Count <= 0))
        return;
      SkillData looting = this.Looting;
      this.MostRecentlyUsedSkill = SkillType.Looting;
      int level1 = looting.Level;
      int minCustBuyPrice = ItemData.GetMinCustBuyPrice(item.ItemID);
      int num1 = item.Count;
      if (item.ItemID == StudioForge.TotalMiner.Item.GoldPieces && num1 > 1000)
        num1 = 1000;
      float num2 = Math.Min(100f, Math.Max(1f, (float) minCustBuyPrice / 17f)) * (float) num1 * this.GetGeneralXPModifier((Actor) player, item.ItemID, true);
      looting.CurrentXP += (double) num2;
      this.Looting = looting;
      if (this.IsSkillsGlobal)
        Globals2.HighscoreDataChanged = true;
      Matrix screenMatrix = player.GetScreenMatrix(true);
      this.DisplayXPGainedMessage(player, string.Format("+{0:N1}", (object) num2), 0, 0, -60f, 2f, 0.75f, Color.Cyan, screenMatrix);
      int level2 = looting.Level;
      if (level2 <= level1)
        return;
      this.LevelUp(player, looting, level2, Color.Cyan, screenMatrix, true, false);
    }

    private void LevelUp(
      Player player,
      SkillData skillData,
      int newLevel,
      Color color,
      Matrix matrix,
      bool use1,
      bool use2)
    {
      string str = skillData.SkillType.ToString() + " Level: " + newLevel.ToString();
      CoreGlobals.Message.ShowMessage("Congratulations!\nYou are now " + str, new Vector2(0.0f, -100f), 3f, 2.2f, color, matrix);
      if (skillData.SkillType != SkillType.Looting)
      {
        float num = 0.0f;
        if (use1)
          num = this.ShowNewItems(skillData.SkillType, newLevel, false, 0.0f, matrix).Y;
        if (use2 && skillData.SkillType != SkillType.Farming)
          this.ShowNewItems(skillData.SkillType, newLevel, true, num + 10f, matrix);
      }
      string message = player.Gamertag + " is now " + str;
      player.GameInstance.AddNotification(message, NotifyRecipient.Remote);
      player.OnSkillLevelled(skillData);
    }

    private SkillDataXML GetItemDataForTool(ref StudioForge.TotalMiner.Item tool, Block blockID)
    {
      SkillDataXML skillDataXml = Globals1.SkillData[(int) tool];
      switch (skillDataXml.UseSkill)
      {
        case SkillType.Mining:
        case SkillType.Digging:
        case SkillType.Chopping:
          return skillDataXml;
        case SkillType.Farming:
          if (ItemData.IsSubType(tool, ItemSubType.TillTool) && !BlockData.IsTillable(blockID, tool) || ItemData.IsSubType(tool, ItemSubType.HarvestTool) && blockID != Block.Crop)
          {
            tool = StudioForge.TotalMiner.Item.WoodPickaxe;
            skillDataXml = Globals1.SkillData[(int) tool];
            goto case SkillType.Mining;
          }
          else
            goto case SkillType.Mining;
        default:
          tool = StudioForge.TotalMiner.Item.WoodPickaxe;
          skillDataXml = Globals1.SkillData[(int) tool];
          goto case SkillType.Mining;
      }
    }

    private float GetBlockMinedXPModifier(Actor actor, MapBlock data)
    {
      float num1 = 1f;
      Block blockId = (Block) data.BlockID;
      float num2;
      if (blockId == Block.Crop)
      {
        int num3 = (int) data.AuxData >> 4;
        float num4 = num1 + (float) num3 * 0.1f;
        int num5 = (int) data.AuxData & 7;
        if (num5 > 5)
          num5 = 5;
        num2 = num4 * ((float) (num5 + 1) / 6f);
      }
      else
        num2 = num1 * CharacterSkillsData.GetBlockXPModifier(actor.GameInstance.Map, blockId);
      return num2;
    }

    public static float GetToolXPModifier(StudioForge.TotalMiner.Item tool)
    {
      return !ItemData.IsItemTypeClass(tool, ItemTypeClass.SledgeHammer) ? 1f : 0.1f;
    }

    public static float GetBlockXPModifier(MapTM map, Block blockID)
    {
      return 1f;
    }

    private float GetGeneralXPModifier(Actor actor, StudioForge.TotalMiner.Item itemID, bool useItem)
    {
      bool flag = actor.IsItemEquippedAndUsable(StudioForge.TotalMiner.Item.NecklaceOfKnowledge);
      float num = flag ? 1.2f : 1f;
      if (useItem && flag)
        actor.OnItemUsed(actor.Inventory.GetEquipSlotID(StudioForge.TotalMiner.Item.NecklaceOfKnowledge));
      if (this.IsSkillsLocal)
        num *= Globals2.GameProperties.SaveGame.Header.XPMultiplier;
      return num;
    }

    private void StrengthGained(Actor actor, double xpGained, Matrix matrix)
    {
      if (!this.IsSkillsEnabled)
        return;
      SkillData strength = this.Strength;
      int level1 = strength.Level;
      strength.CurrentXP += xpGained;
      this.Strength = strength;
      if (!actor.IsPlayer)
        return;
      int level2 = strength.Level;
      if (level2 <= level1)
        return;
      this.LevelUp(actor as Player, strength, level2, Color.Cyan, matrix, false, true);
    }

    private void HealthGained(Actor actor, double xpGained, Matrix matrix)
    {
      if (!this.IsSkillsEnabled)
        return;
      SkillData health = this.Health;
      int level1 = health.Level;
      health.CurrentXP += xpGained;
      this.Health = health;
      if (!actor.IsPlayer)
        return;
      int level2 = health.Level;
      if (level2 <= level1)
        return;
      this.LevelUp(actor as Player, health, level2, Color.Cyan, matrix, false, true);
    }

    private Vector2 ShowNewItems(
      SkillType skillType,
      int level,
      bool use,
      float yOff,
      Matrix matrix)
    {
      bool flag = true;
      int num1 = 0;
      string[] strArray = use ? CharacterSkillsData.UseDesc2 : CharacterSkillsData.UseDesc1;
      StringBuilder stringBuilder = new StringBuilder("You can now ");
      stringBuilder.Append(strArray[(int) skillType]);
      stringBuilder.Append(":\n");
      foreach (SkillDataXML data in Globals1.SkillData)
      {
        ItemDataXML itemDataXml = Globals1.ItemData[(int) data.ItemID];
        if (itemDataXml.IsEnabled && !itemDataXml.HasItemProxy && this.SkillMatchNewLevel(data, skillType, level, use))
        {
          string name = itemDataXml.Name;
          int num2;
          if (flag)
          {
            stringBuilder.Append("   ");
            stringBuilder.Append(name);
            flag = false;
            num2 = 3;
          }
          else
          {
            stringBuilder.Append(", ");
            num2 = num1 + 2;
            if (num2 + name.Length > 54)
            {
              stringBuilder.Append("\n   ");
              num2 = 3;
            }
            stringBuilder.Append(name);
          }
          num1 = num2 + name.Length;
        }
      }
      if (flag)
        return Vector2.Zero;
      stringBuilder.Append('.');
      return this.DisplayMessageCore(stringBuilder.ToString(), new Vector2(120f, 460f + yOff), -80f, 6f, 1.5f, use ? Color.White : Color.LightGreen, matrix);
    }

    private bool SkillMatchNewLevel(SkillDataXML data, SkillType skillType, int level, bool use)
    {
      if (!use)
      {
        switch (skillType)
        {
          case SkillType.Mining:
          case SkillType.Digging:
          case SkillType.Chopping:
            return data.MineReq == level;
          case SkillType.Crafting:
          case SkillType.Smelting:
          case SkillType.Smithing:
          case SkillType.Cooking:
            if (data.CraftSkill == skillType)
              return data.CraftReq == level;
            return false;
        }
      }
      if (data.UseSkill == skillType)
        return data.UseReq == level;
      return false;
    }

    private Vector2 DisplayXPGainedMessage(
      Player player,
      string text,
      int freq,
      int yOff,
      float yVel,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      return this.DisplayXPGainedMessage(player, text, freq, new Vector2(0.0f, (float) yOff), yVel, seconds, scale, color, matrix);
    }

    private Vector2 DisplayXPGainedMessage(
      Player player,
      string text,
      int freq,
      Vector2 pOff,
      float yVel,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      if (player.Settings.DisplayXPGains && player.IsLocalGamer)
        return this.DisplayMessage(text, freq, pOff, yVel, seconds, scale, color, matrix);
      return Vector2.Zero;
    }

    private Vector2 DisplayMessage(
      string text,
      int freq,
      int yOff,
      float yVel,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      return this.DisplayMessage(text, freq, new Vector2(0.0f, (float) yOff), yVel, seconds, scale, color, matrix);
    }

    private Vector2 DisplayMessage(
      string text,
      int freq,
      Vector2 pOff,
      float yVel,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      if (this.messageTimer.ElapsedMilliseconds < (long) freq)
        return Vector2.Zero;
      if (freq > 0)
      {
        this.messageTimer.Reset();
        this.messageTimer.Start();
      }
      Point center = GraphicStatics.DefaultViewport.TitleSafeArea.Center;
      pOff -= CoreGlobals.GameFont.MeasureString(text) * (scale / 1.5f) * 0.5f;
      return this.DisplayMessageCore(text, new Vector2((float) center.X + pOff.X, (float) center.Y + pOff.Y), yVel, seconds, scale, color, matrix);
    }

    private Vector2 DisplayMessageCore(
      string text,
      Vector2 pOff,
      float yVel,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      return CoreGlobals.Message.ShowMessage(text, pOff, new Vector2(0.0f, yVel), seconds, scale * 1.5f, color, false, matrix);
    }

    public void ReadState(BinaryReader reader, int version)
    {
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        double xp = reader.ReadDouble();
        SkillData skillData = this.skillData[index];
        skillData.SetCurrentXPRaw(xp);
        this.skillData[index] = skillData;
      }
      this.AdjustXP(version);
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.skillData.Length);
      for (int index = 0; index < this.skillData.Length; ++index)
        writer.Write(this.skillData[index].CurrentXP);
    }

    public void AdjustXP(int version)
    {
      if (version >= 204)
        return;
      double val2 = this.Attack.CurrentXP + this.Defence.CurrentXP + this.Ranged.CurrentXP;
      if (this.Looting.CurrentXP <= Math.Min(5000000.0, val2 * 3.0))
        return;
      int index = 15;
      SkillData skillData = this[index];
      skillData.SetCurrentXPRaw(Math.Min(skillData.CurrentXP, val2));
      this[index] = skillData;
    }
  }
}
