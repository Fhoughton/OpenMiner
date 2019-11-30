// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.HealthBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.API;
using System;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class HealthBlock : DataBlock
  {
    public string KillScript;
    public string HistoryKey;
    public int HealthLevel;
    public int DefenceLevel;
    private float health;
    private AudioEmitter audioEmitter;

    public bool IsCombatEnabled
    {
      get
      {
        if (this.HealthLevel <= 0)
          return this.HistoryKey.IsNotEmpty();
        return true;
      }
    }

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Health;
      }
    }

    public float GetHealth(GameInstance instance)
    {
      if (instance == null || this.HistoryKey == null || this.HistoryKey.Length < 1)
        return this.health;
      return (float) instance.History.GetHistory(this.HistoryKey.ToLower());
    }

    public void SetHealthLevel(int level)
    {
      this.HealthLevel = Math.Min(9999, Math.Max(1, level));
      this.health = SkillData.MaxHealth(this.HealthLevel);
    }

    public HealthBlock()
    {
    }

    public HealthBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void RenameScript(string oldName, string newName)
    {
      if (!(this.KillScript == oldName))
        return;
      this.KillScript = newName;
    }

    public override void SetScript(string name, DataBlockScriptType type)
    {
      this.KillScript = name;
    }

    public bool Struck(GameInstance instance, Actor attacker, SkillType attackType, Item weapon)
    {
      DamageType damageType;
      float damage = this.CalcStrikeDamage(attacker, attackType, weapon, false, out damageType);
      float damageAndDisplay = this.TakeDamageAndDisplay(instance, damageType, damage, attacker, weapon, attackType);
      if (attacker != null && attacker.IsLocalGamer)
        instance.NetworkManager.SendDamage(damageType, damageAndDisplay, (Actor) null, attacker, weapon, new GlobalPoint3D?(this.Point));
      return (double) damageAndDisplay > 0.0;
    }

    public float TakeDamageAndDisplay(
      GameInstance instance,
      DamageType damageType,
      float damage,
      Actor attacker,
      Item weaponID,
      SkillType attackType)
    {
      Player player = attacker as Player;
      if (player != null && player.IsLocalGamer && player.IsGod)
        damage = this.GetHealth(instance) + 100f;
      damage = this.TakeDamageLocal(instance, damageType, damage, attacker, weaponID);
      if ((double) damage > 0.0)
        instance.AddDamageParticles(instance.Map.GetBlockCenter(this.Point), damage, damageType);
      if (attacker != null)
      {
        Sounds.PlaySound(weaponID, (double) damage > 0.0 ? ItemSoundType.Use : ItemSoundType.UseFail, (ITMActor) attacker);
        if ((double) damage > 0.0)
        {
          if (this.audioEmitter == null)
            this.audioEmitter = new AudioEmitter()
            {
              DopplerScale = 1f,
              Up = Vector3.Up
            };
          this.audioEmitter.Position = instance.Map.GetBlockCenter(this.Point);
          this.audioEmitter.Forward = Vector3.Normalize(attacker.Position - this.audioEmitter.Position);
          Sounds.PlaySound(ItemSoundGroup.BodyHit, ItemSoundType.Use, this.audioEmitter);
        }
      }
      switch (damageType)
      {
        case DamageType.Unknown:
        case DamageType.Combat:
        case DamageType.ItemUse:
        case DamageType.Blast:
        case DamageType.BlockFallingOnHead:
        case DamageType.ShieldDeflect:
          int damage1 = (double) damage >= 1.0 ? (int) damage : ((double) damage <= 0.0 ? 0 : (int) ((double) damage + 1.0));
          this.DisplayDamage(instance, attacker, damage1, weaponID, attackType, damageType);
          break;
      }
      return damage;
    }

    protected virtual float TakeDamageLocal(
      GameInstance instance,
      DamageType damageType,
      float damage,
      Actor attacker,
      Item weaponID)
    {
      if ((double) damage <= 0.0)
        return 0.0f;
      bool flag = false;
      float health = this.GetHealth(instance);
      float num;
      if ((double) damage >= (double) health)
      {
        damage = health;
        num = 0.0f;
        flag = true;
      }
      else
        num = health - damage;
      if (this.HistoryKey.IsNotEmpty())
        instance.History.SetHistory(this.HistoryKey.ToLower(), (long) (int) num);
      else
        this.health = num;
      if (flag)
      {
        this.DieBlock(instance, this.Point, this.Point, attacker != null ? attacker.GamerID : GamerID.Sys1);
        ScriptExecuteData data = new ScriptExecuteData()
        {
          Killer = attacker,
          Actor = (Actor) (attacker as Player),
          BlockOffset = new GlobalPoint3D?(this.Point)
        };
        instance.ExecuteScript(this.KillScript, data, false);
        instance.Map.Commit();
      }
      instance.MapRenderer.HealthBlockChanged();
      return damage;
    }

    private void DieBlock(
      GameInstance instance,
      GlobalPoint3D orig,
      GlobalPoint3D p,
      GamerID gamerID)
    {
      if (Math.Abs(p.X - orig.X) > 10 || Math.Abs(p.Y - orig.Y) > 10 || Math.Abs(p.Z - orig.Z) > 10)
        return;
      HealthBlock dataBlock = instance.MapStrategyTM.GetDataBlock(p) as HealthBlock;
      if (dataBlock == null || (double) dataBlock.GetHealth(instance) != 0.0)
        return;
      instance.Map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Player, gamerID, false);
      --p.Y;
      if (instance.Map.GetBlockID(p) == (byte) 78)
        this.DieBlock(instance, orig, p, gamerID);
      p.Y += 2;
      if (instance.Map.GetBlockID(p) == (byte) 78)
        this.DieBlock(instance, orig, p, gamerID);
      --p.Y;
      --p.X;
      if (instance.Map.GetBlockID(p) == (byte) 78)
        this.DieBlock(instance, orig, p, gamerID);
      p.X += 2;
      if (instance.Map.GetBlockID(p) == (byte) 78)
        this.DieBlock(instance, orig, p, gamerID);
      --p.X;
      --p.Z;
      if (instance.Map.GetBlockID(p) == (byte) 78)
        this.DieBlock(instance, orig, p, gamerID);
      p.Z += 2;
      if (instance.Map.GetBlockID(p) != (byte) 78)
        return;
      this.DieBlock(instance, orig, p, gamerID);
    }

    private float CalcStrikeDamage(
      Actor attacker,
      SkillType attackType,
      Item weapon,
      bool isCriticalRegion,
      out DamageType damageType)
    {
      float num1 = 0.0f;
      damageType = DamageType.Unknown;
      if (attacker != null)
      {
        PcgRandom random = attacker.GameInstance.Random;
        if (random != null)
        {
          int max = attackType == SkillType.Ranged ? (int) ((double) (attacker.RangedLevel(true) + 2) * 100.0) : (int) ((double) (attacker.AttackLevel(true) + 2) * 100.0);
          int attackRoll = random.Next(max);
          int defenceRoll = this.GetDefenceRoll(attacker, attackRoll, out damageType);
          if (attackRoll > defenceRoll)
          {
            int maxHit = attacker.GetMaxHit(weapon, attackType);
            float num2 = isCriticalRegion ? (float) maxHit * 0.25f : 0.0f;
            float num3 = isCriticalRegion ? (float) maxHit : (float) maxHit - (float) maxHit * 0.25f;
            num1 = (float) random.Next((int) ((double) num2 * 100.0), (int) (((double) num3 + 1.0) * 100.0)) * 0.01f;
            if ((double) num1 > 0.0 && (double) num1 < 1.0)
              num1 = 1f;
            damageType = DamageType.Combat;
            if ((double) num1 < (double) maxHit)
            {
              Actor.BonusData criticalBonus = attacker.GetCriticalBonus();
              if (criticalBonus.SlotID >= 0 && (double) criticalBonus.Value > 0.0 && random.NextDouble() <= (double) criticalBonus.Value)
              {
                num1 = (float) maxHit;
                attacker.OnItemUsed(criticalBonus.SlotID);
              }
            }
          }
        }
      }
      return num1;
    }

    private int GetDefenceRoll(Actor attacker, int attackRoll, out DamageType damageType)
    {
      damageType = DamageType.Combat;
      int max = this.DefenceLevel * Globals2.Defence;
      return attacker.GameInstance.Random.Next(max);
    }

    private void DisplayDamage(
      GameInstance instance,
      Actor attacker,
      int damage,
      Item weaponID,
      SkillType attackType,
      DamageType damageType)
    {
      int num1 = attacker != null ? attacker.GetMaxHit(weaponID, attackType) : 0;
      Color color = damage >= num1 ? Color.OrangeRed : (damage > 0 ? Color.Red : (damageType == DamageType.ShieldDeflect ? Color.Cyan : Color.Blue));
      int num2 = instance.Random.Next(30) + 30;
      if (instance.Random.NextDouble() > 0.5)
        num2 = -num2;
      float num3 = -60f;
      foreach (Player localEnabledPlayer in instance.NetworkManager.LocalEnabledPlayers)
      {
        Player virtualPlayer = localEnabledPlayer.VirtualPlayer;
        float y = num3;
        if (virtualPlayer == attacker)
        {
          CoreGlobals.Message.ShowMessage(damage > 0 ? string.Format("+{0}", (object) damage) : "0", new Vector2((float) num2, y), 2f, 1.7f, color, localEnabledPlayer.GetScreenMatrix(true));
        }
        else
        {
          Vector3 blockCenter = instance.Map.GetBlockCenter(this.Point);
          Vector3 vector3 = localEnabledPlayer.Viewport.Project(blockCenter, localEnabledPlayer.ProjectionMatrix, virtualPlayer.ViewMatrix, Matrix.Identity);
          if ((double) vector3.Z < 1.0)
          {
            float num4 = Vector3.Distance(virtualPlayer.EyePosition, blockCenter);
            if ((double) num4 < 40.0)
            {
              float scale = (float) ((50.0 - (double) num4) * 0.0364999994635582);
              string str = damage > 0 ? string.Format("+{0}", (object) damage) : "0";
              vector3.X -= (float) ((double) CoreGlobals.Message.Font.MeasureString(str).X * (double) scale * 0.5);
              CoreGlobals.Message.ShowMessage(str, new Vector2(vector3.X, vector3.Y), new Vector2((float) num2 * scale, y * scale), 1.5f, scale, color, false, localEnabledPlayer.GetScreenMatrix(true));
            }
          }
        }
      }
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      HealthBlock healthBlock = from as HealthBlock;
      this.KillScript = healthBlock.KillScript;
      this.HistoryKey = healthBlock.HistoryKey;
      this.HealthLevel = healthBlock.HealthLevel;
      this.DefenceLevel = healthBlock.DefenceLevel;
      this.health = healthBlock.health;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.KillScript = reader.ReadString();
      this.HistoryKey = version > 241 ? reader.ReadString() : (string) null;
      this.HealthLevel = reader.ReadInt32();
      this.DefenceLevel = reader.ReadInt32();
      this.health = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.KillScript == null ? "" : this.KillScript);
      writer.Write(this.HistoryKey == null ? "" : this.HistoryKey);
      writer.Write(this.HealthLevel);
      writer.Write(this.DefenceLevel);
      writer.Write(this.health);
    }
  }
}
