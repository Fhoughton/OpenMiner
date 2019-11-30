// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptRuntime
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class ScriptRuntime
  {
    private CombatStats combatStats = new CombatStats();
    private HealthEffect healthEffect = new HealthEffect();
    private BoundingFrustum hitFrustum = new BoundingFrustum(Matrix.Identity);
    private Stopwatch clock;
    private ScriptCompiler compiler;
    private ScriptRuntime.ScriptCommandFunction[] commands;
    private ScriptRuntime.VarLoadFunction[] varload;

    public ScriptRuntime(GameInstance instance, Stopwatch clock)
    {
      this.clock = clock;
      this.compiler = new ScriptCompiler(instance);
      this.commands = new ScriptRuntime.ScriptCommandFunction[121];
      this.commands[0] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[1] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[2] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[3] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[4] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[5] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[6] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[7] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[8] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[9] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[10] = new ScriptRuntime.ScriptCommandFunction(this.CommandNotify);
      this.commands[11] = new ScriptRuntime.ScriptCommandFunction(this.CommandLoop);
      this.commands[12] = new ScriptRuntime.ScriptCommandFunction(this.CommandWait);
      this.commands[13] = new ScriptRuntime.ScriptCommandFunction(this.CommandClan);
      this.commands[14] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetBlock);
      this.commands[15] = new ScriptRuntime.ScriptCommandFunction(this.CommandSkill);
      this.commands[16] = new ScriptRuntime.ScriptCommandFunction(this.CommandSkillXP);
      this.commands[17] = new ScriptRuntime.ScriptCommandFunction(this.CommandExplosion);
      this.commands[18] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[19] = new ScriptRuntime.ScriptCommandFunction(this.CommandNop);
      this.commands[20] = new ScriptRuntime.ScriptCommandFunction(this.CommandElse);
      this.commands[21] = new ScriptRuntime.ScriptCommandFunction(this.CommandElse);
      this.commands[22] = new ScriptRuntime.ScriptCommandFunction(this.CommandEndif);
      this.commands[23] = new ScriptRuntime.ScriptCommandFunction(this.CommandCanEquip);
      this.commands[24] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasAction);
      this.commands[25] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasHistory);
      this.commands[26] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasInventory);
      this.commands[27] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasMarker);
      this.commands[28] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasPermission);
      this.commands[29] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasActor);
      this.commands[30] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasSkill);
      this.commands[31] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsAvatar);
      this.commands[32] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlock);
      this.commands[33] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockEdited);
      this.commands[34] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockLightSource);
      this.commands[35] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsLight);
      this.commands[36] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockOre);
      this.commands[37] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockPassable);
      this.commands[38] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockDeliveringPower);
      this.commands[39] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockReceivingPower);
      this.commands[40] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockSolid);
      this.commands[41] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockTexture);
      this.commands[42] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsClan);
      this.commands[43] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsClock);
      this.commands[44] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsCombat);
      this.commands[45] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsDayTime);
      this.commands[46] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsDistance);
      this.commands[47] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsEquipped);
      this.commands[48] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsFiniteResources);
      this.commands[49] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsGamerCount);
      this.commands[50] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsNpcCount);
      this.commands[51] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsNightTime);
      this.commands[52] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsRandom);
      this.commands[53] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsSkills);
      this.commands[54] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsTime);
      this.commands[55] = new ScriptRuntime.ScriptCommandFunction(this.CommandHistory);
      this.commands[56] = new ScriptRuntime.ScriptCommandFunction(this.CommandWaypoint);
      this.commands[57] = new ScriptRuntime.ScriptCommandFunction(this.CommandScript);
      this.commands[58] = new ScriptRuntime.ScriptCommandFunction(this.CommandPermission);
      this.commands[59] = new ScriptRuntime.ScriptCommandFunction(this.CommandZone);
      this.commands[60] = new ScriptRuntime.ScriptCommandFunction(this.CommandHealth);
      this.commands[61] = new ScriptRuntime.ScriptCommandFunction(this.CommandTeleport);
      this.commands[62] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetReach);
      this.commands[63] = new ScriptRuntime.ScriptCommandFunction(this.CommandMarker);
      this.commands[64] = new ScriptRuntime.ScriptCommandFunction(this.CommandPickup);
      this.commands[65] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetPower);
      this.commands[66] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetSwitch);
      this.commands[67] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetText);
      this.commands[68] = new ScriptRuntime.ScriptCommandFunction(this.CommandCommit);
      this.commands[69] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetTexture);
      this.commands[70] = new ScriptRuntime.ScriptCommandFunction(this.CommandSound);
      this.commands[71] = new ScriptRuntime.ScriptCommandFunction(this.CommandOpenBlock);
      this.commands[72] = new ScriptRuntime.ScriptCommandFunction(this.CommandCaveIn);
      this.commands[73] = new ScriptRuntime.ScriptCommandFunction(this.CommandEquip);
      this.commands[74] = new ScriptRuntime.ScriptCommandFunction(this.CommandUnequip);
      this.commands[75] = new ScriptRuntime.ScriptCommandFunction(this.CommandMoveBlock);
      this.commands[76] = new ScriptRuntime.ScriptCommandFunction(this.CommandMoveRegion);
      this.commands[77] = new ScriptRuntime.ScriptCommandFunction(this.CommandCopyBlock);
      this.commands[78] = new ScriptRuntime.ScriptCommandFunction(this.CommandCopyRegion);
      this.commands[79] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetRegion);
      this.commands[80] = new ScriptRuntime.ScriptCommandFunction(this.CommandReplaceRegion);
      this.commands[81] = new ScriptRuntime.ScriptCommandFunction(this.CommandCCTV);
      this.commands[82] = new ScriptRuntime.ScriptCommandFunction(this.CommandRain);
      this.commands[83] = new ScriptRuntime.ScriptCommandFunction(this.CommandHail);
      this.commands[84] = new ScriptRuntime.ScriptCommandFunction(this.CommandFog);
      this.commands[85] = new ScriptRuntime.ScriptCommandFunction(this.CommandMenu);
      this.commands[86] = new ScriptRuntime.ScriptCommandFunction(this.CommandMessageBox);
      this.commands[87] = new ScriptRuntime.ScriptCommandFunction(this.CommandInventory);
      this.commands[88] = new ScriptRuntime.ScriptCommandFunction(this.CommandSkyColor);
      this.commands[89] = new ScriptRuntime.ScriptCommandFunction(this.CommandNpcSpawn);
      this.commands[90] = new ScriptRuntime.ScriptCommandFunction(this.CommandTintColor);
      this.commands[91] = new ScriptRuntime.ScriptCommandFunction(this.CommandPaste);
      this.commands[92] = new ScriptRuntime.ScriptCommandFunction(this.CommandParticle);
      this.commands[93] = new ScriptRuntime.ScriptCommandFunction(this.CommandParticleEmitter);
      this.commands[94] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetEventScript);
      this.commands[95] = new ScriptRuntime.ScriptCommandFunction(this.CommandKick);
      this.commands[96] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsNameplate);
      this.commands[97] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetNameplate);
      this.commands[98] = new ScriptRuntime.ScriptCommandFunction(this.CommandExit);
      this.commands[99] = new ScriptRuntime.ScriptCommandFunction(this.CommandHUDBar);
      this.commands[100] = new ScriptRuntime.ScriptCommandFunction(this.CommandNpcState);
      this.commands[101] = new ScriptRuntime.ScriptCommandFunction(this.CommandNpcHealth);
      this.commands[102] = new ScriptRuntime.ScriptCommandFunction(this.CommandIntersect);
      this.commands[103] = new ScriptRuntime.ScriptCommandFunction(this.CommandContext);
      this.commands[104] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetBlockScript);
      this.commands[105] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockOpen);
      this.commands[106] = new ScriptRuntime.ScriptCommandFunction(this.CommandHUDCounter);
      this.commands[107] = new ScriptRuntime.ScriptCommandFunction(this.CommandHUDShape);
      this.commands[108] = new ScriptRuntime.ScriptCommandFunction(this.CommandHUDText);
      this.commands[109] = new ScriptRuntime.ScriptCommandFunction(this.CommandHasStatBonus);
      this.commands[110] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsBlockResistance);
      this.commands[111] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetSphere);
      this.commands[112] = new ScriptRuntime.ScriptCommandFunction(this.CommandVar);
      this.commands[114] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsVar);
      this.commands[115] = new ScriptRuntime.ScriptCommandFunction(this.CommandBlueprint);
      this.commands[116] = new ScriptRuntime.ScriptCommandFunction(this.CommandItem);
      this.commands[117] = new ScriptRuntime.ScriptCommandFunction(this.CommandInput);
      this.commands[118] = new ScriptRuntime.ScriptCommandFunction(this.CommandIsInZone);
      this.commands[119] = new ScriptRuntime.ScriptCommandFunction(this.CommandSetRegionAux);
      this.commands[120] = new ScriptRuntime.ScriptCommandFunction(this.CommandRandom);
      this.varload = new ScriptRuntime.VarLoadFunction[53];
      this.varload[0] = new ScriptRuntime.VarLoadFunction(this.VarLoadNumLiterial);
      this.varload[1] = new ScriptRuntime.VarLoadFunction(this.VarLoadVariable);
      this.varload[2] = new ScriptRuntime.VarLoadFunction(this.VarLoadHistoryKey);
      this.varload[3] = new ScriptRuntime.VarLoadFunction(this.VarLoadSysHistoryKey);
      this.varload[4] = new ScriptRuntime.VarLoadFunction(this.VarLoadClanHistoryKey);
      this.varload[5] = new ScriptRuntime.VarLoadFunction(this.VarLoadRandom);
      this.varload[6] = new ScriptRuntime.VarLoadFunction(this.VarLoadSkill);
      this.varload[7] = new ScriptRuntime.VarLoadFunction(this.VarLoadSkillXP);
      this.varload[8] = new ScriptRuntime.VarLoadFunction(this.VarLoadHealth);
      this.varload[9] = new ScriptRuntime.VarLoadFunction(this.VarLoadMaxHealth);
      this.varload[10] = new ScriptRuntime.VarLoadFunction(this.VarLoadReach);
      this.varload[11] = new ScriptRuntime.VarLoadFunction(this.VarLoadPosX);
      this.varload[12] = new ScriptRuntime.VarLoadFunction(this.VarLoadPosY);
      this.varload[13] = new ScriptRuntime.VarLoadFunction(this.VarLoadPosZ);
      this.varload[14] = new ScriptRuntime.VarLoadFunction(this.VarLoadEyeY);
      this.varload[15] = new ScriptRuntime.VarLoadFunction(this.VarLoadVelX);
      this.varload[16] = new ScriptRuntime.VarLoadFunction(this.VarLoadVelY);
      this.varload[17] = new ScriptRuntime.VarLoadFunction(this.VarLoadVelZ);
      this.varload[18] = new ScriptRuntime.VarLoadFunction(this.VarLoadViewX);
      this.varload[19] = new ScriptRuntime.VarLoadFunction(this.VarLoadViewY);
      this.varload[20] = new ScriptRuntime.VarLoadFunction(this.VarLoadViewZ);
      this.varload[21] = new ScriptRuntime.VarLoadFunction(this.VarLoadRelX);
      this.varload[22] = new ScriptRuntime.VarLoadFunction(this.VarLoadRelY);
      this.varload[23] = new ScriptRuntime.VarLoadFunction(this.VarLoadRelZ);
      this.varload[24] = new ScriptRuntime.VarLoadFunction(this.VarLoadCRelX);
      this.varload[25] = new ScriptRuntime.VarLoadFunction(this.VarLoadCRelY);
      this.varload[26] = new ScriptRuntime.VarLoadFunction(this.VarLoadCRelZ);
      this.varload[27] = new ScriptRuntime.VarLoadFunction(this.VarLoadPRelX);
      this.varload[28] = new ScriptRuntime.VarLoadFunction(this.VarLoadPRelY);
      this.varload[29] = new ScriptRuntime.VarLoadFunction(this.VarLoadPRelZ);
      this.varload[30] = new ScriptRuntime.VarLoadFunction(this.VarLoadScriptX);
      this.varload[31] = new ScriptRuntime.VarLoadFunction(this.VarLoadScriptY);
      this.varload[32] = new ScriptRuntime.VarLoadFunction(this.VarLoadScriptZ);
      this.varload[33] = new ScriptRuntime.VarLoadFunction(this.VarLoadAbs);
      this.varload[34] = new ScriptRuntime.VarLoadFunction(this.VarLoadSin);
      this.varload[35] = new ScriptRuntime.VarLoadFunction(this.VarLoadCos);
      this.varload[36] = new ScriptRuntime.VarLoadFunction(this.VarLoadTan);
      this.varload[37] = new ScriptRuntime.VarLoadFunction(this.VarLoadSqrt);
      this.varload[38] = new ScriptRuntime.VarLoadFunction(this.VarLoadSingle);
      this.varload[39] = new ScriptRuntime.VarLoadFunction(this.VarLoadInv);
      this.varload[40] = new ScriptRuntime.VarLoadFunction(this.VarLoadDistance);
      this.varload[41] = new ScriptRuntime.VarLoadFunction(this.VarLoadClock);
      this.varload[42] = new ScriptRuntime.VarLoadFunction(this.VarLoadGamerCount);
      this.varload[43] = new ScriptRuntime.VarLoadFunction(this.VarLoadNpcCount);
      this.varload[44] = new ScriptRuntime.VarLoadFunction(this.VarLoadPi);
      this.varload[45] = new ScriptRuntime.VarLoadFunction(this.VarLoadLight);
      this.varload[46] = new ScriptRuntime.VarLoadFunction(this.VarLoadHash);
      this.varload[47] = new ScriptRuntime.VarLoadFunction(this.VarLoadInt);
      this.varload[48] = new ScriptRuntime.VarLoadFunction(this.VarLoadNeg);
      this.varload[49] = new ScriptRuntime.VarLoadFunction(this.VarLoadBlockID);
      this.varload[50] = new ScriptRuntime.VarLoadFunction(this.VarLoadAux);
      this.varload[51] = new ScriptRuntime.VarLoadFunction(this.VarLoadSunLight);
      this.varload[52] = new ScriptRuntime.VarLoadFunction(this.VarLoadBlockLight);
    }

    public void ExecuteScript(ScriptInstance si)
    {
      Script script = si.Script;
      si.WaitTime = 0L;
      if (script.IsChanged || script.ByteCode == null)
      {
        this.compiler.CompileScript(script);
        script.ByteCodeReader = new BinaryReader((Stream) script.ByteCode);
        script.ByteCodeSize = 4 + (int) script.ByteCode.Length;
        si.PC = 0L;
        if (si.Random == null)
          si.OrigRandom.Seed((ulong) (uint) script.Name.GetHashCode(), (ulong) (uint) (script.ByteCodeSize * 987));
      }
      long elapsedTicks = this.clock.ElapsedTicks;
      BinaryReader byteCodeReader = script.ByteCodeReader;
      byteCodeReader.BaseStream.Position = si.PC;
      if (si.PC == 0L)
      {
        si.VarCount = (int) byteCodeReader.ReadUInt16();
        if (si.VarCount > 0)
          this.BuildVariableTable(si);
        si.PC = si.BeginPC = byteCodeReader.BaseStream.Position;
        if (si.Random == null)
          si.Random = si.OrigRandom;
        si.NeedsCommit = false;
      }
      if (si.PC == si.BeginPC)
      {
        ++script.ExecutionCount;
        script.LastExecutionTicks = 0L;
      }
      while (si.PC < byteCodeReader.BaseStream.Length && si.WaitTime == 0L && (si.WaitingOnResult == 0L && !si.IsCancelled))
      {
        si.CurentCmdPC = si.PC;
        byte num1 = byteCodeReader.ReadByte();
        ushort num2 = byteCodeReader.ReadUInt16();
        long position = byteCodeReader.BaseStream.Position;
        this.commands[(int) num1](si, byteCodeReader);
        if (si.UpdatePC)
          byteCodeReader.BaseStream.Position = si.PC = position + (long) num2;
        else
          si.UpdatePC = true;
      }
      si.IsComplete = si.PC >= byteCodeReader.BaseStream.Length;
      if (si.IsComplete)
      {
        si.Random = (PcgRandom) null;
        if (si.Parent != null)
        {
          if (si.ParentVars != null)
            this.ReturnVariables(si);
          if (si.Parent.WaitTime == long.MaxValue)
            si.Parent.WaitTime = 0L;
        }
        if (si.OnComplete != null)
          si.OnComplete(script, si.Player);
        if (si.NeedsCommit)
          si.MapTM.Commit();
      }
      if (!si.IsComplete && si.PC != si.BeginPC || script.ExecutionCount <= 1)
        return;
      long num = this.clock.ElapsedTicks - elapsedTicks;
      script.LastExecutionTicks += num;
      script.TotalExecutionTicks += num;
    }

    public bool CancelScript(ScriptInstance si, Script script, Actor actor)
    {
      if (si == null || !(si.Script.Name == script.Name) || actor != null && si.OrigActor != actor)
        return false;
      if (si.Parent != null)
      {
        if (si.ParentVars != null)
          this.ReturnVariables(si);
        if (si.Parent.WaitTime == long.MaxValue)
          si.Parent.WaitTime = 0L;
      }
      if (si.NeedsCommit)
        si.MapTM.Commit();
      si.IsCancelled = true;
      return true;
    }

    private void BuildVariableTable(ScriptInstance si)
    {
      si.VarNames = si.Script.VarNames;
      if (si.Vars == null || si.Vars.Length < si.VarCount)
        si.Vars = new double[si.VarCount];
      else
        Array.Clear((Array) si.Vars, 0, si.VarCount);
      int num = si.Parent == null || si.ParentVars == null ? 0 : si.ParentVars.Length;
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = (int) si.ParentVars[index1] & (int) short.MaxValue;
        for (int index3 = 0; index3 < si.VarCount; ++index3)
        {
          if (si.Parent.VarNames[index2] == si.VarNames[index3])
          {
            si.Vars[index3] = si.Parent.Vars[index2];
            break;
          }
        }
      }
    }

    private void ReturnVariables(ScriptInstance si)
    {
      for (int index1 = 0; index1 < si.ParentVars.Length; ++index1)
      {
        ushort parentVar = si.ParentVars[index1];
        if (((int) parentVar & 32768) > 0)
        {
          ushort num = (ushort) ((uint) parentVar & (uint) short.MaxValue);
          for (int index2 = 0; index2 < si.VarCount; ++index2)
          {
            if (si.Parent.VarNames[(int) num] == si.VarNames[index2])
            {
              si.Parent.Vars[(int) num] = si.Vars[index2];
              break;
            }
          }
        }
      }
    }

    private void JumpToFalseConditional(ScriptInstance si, BinaryReader reader)
    {
      while (reader.BaseStream.Position < reader.BaseStream.Length)
      {
        byte num1 = reader.ReadByte();
        ushort num2 = reader.ReadUInt16();
        reader.BaseStream.Position += (long) num2;
        si.PC = reader.BaseStream.Position;
        si.UpdatePC = false;
        switch (num1)
        {
          case 20:
            return;
          case 21:
            return;
          case 22:
            return;
          default:
            continue;
        }
      }
    }

    private void CommandBlueprint(ScriptInstance si, BinaryReader reader)
    {
      Item itemID1 = (Item) reader.ReadUInt16();
      if (!si.Instance.IsCreativeMode && !ItemData.IsSubTypeAny(itemID1, ItemSubType.Gun | ItemSubType.Potion))
        return;
      Blueprint blueprint = Blueprints.GetBlueprint(itemID1);
      if (blueprint == null)
        return;
      Globals2.NeedToReinitialize = true;
      for (int i = 0; i < 9; ++i)
        blueprint.SetItem(i, InventoryItem.Empty);
      blueprint.IsEnabled = false;
      int count1 = this.ReadInt32(si, reader);
      blueprint.Result = new InventoryItem(itemID1, count1);
      ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID1];
      bool flag = reader.ReadBoolean();
      blueprint.CraftType = flag ? BlueprintCraftType.Furnace : BlueprintCraftType.Crafting;
      if (flag)
      {
        float num = this.ReadSingle(si, reader);
        if ((double) num == 0.0)
          num = 4.5f;
        itemDataXml.SmeltTime = num;
      }
      SkillType? nullable1 = new SkillType?();
      int? nullable2 = new int?();
      if (reader.ReadBoolean())
      {
        nullable1 = new SkillType?((SkillType) reader.ReadByte());
        Globals1.SkillData[(int) itemID1].CraftSkill = nullable1.Value;
      }
      if (reader.ReadBoolean())
      {
        nullable2 = new int?(this.ReadInt32(si, reader));
        Globals1.SkillData[(int) itemID1].CraftReq = nullable2.Value;
      }
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        byte num2 = reader.ReadByte();
        Item itemID2 = (Item) reader.ReadUInt16();
        int count2 = this.ReadInt32(si, reader);
        ushort durability = (ushort) this.ReadInt32(si, reader);
        blueprint.SetItem((int) num2 - 1, new InventoryItem(itemID2, count2, durability));
        if (!blueprint.IsEnabled)
          blueprint.IsEnabled = num2 >= (byte) 0 && num2 < (byte) 9 && itemID2 != Item.None && count2 > 0;
      }
      if (blueprint.IsEnabled && !blueprint.IsValid)
        blueprint.IsValid = true;
      blueprint.BuildDescription();
      if (!si.Instance.IsMultiplayer || !si.Instance.IsHost)
        return;
      string str1 = "blueprint [" + itemID1.ToString() + ":" + count1.ToString() + "]" + " " + (flag ? "[furnace:" + itemDataXml.SmeltTime.ToString() : "[craft") + "]";
      for (int index = 0; index < blueprint.Items.Length; ++index)
      {
        InventoryItem inventoryItem = blueprint.Items[index];
        if (inventoryItem.ItemID != Item.None)
        {
          string str2 = str1 + " [" + index.ToString() + ":" + inventoryItem.ItemID.ToString();
          if (inventoryItem.Count != 1 || inventoryItem.Durability > (ushort) 0)
            str2 = str2 + ":" + inventoryItem.Count.ToString();
          if (inventoryItem.Durability > (ushort) 0)
            str2 = str2 + ":" + inventoryItem.Durability.ToString();
          str1 = str2 + "]";
        }
      }
      if (nullable1.HasValue)
        str1 = str1 + " [skill=" + (object) nullable1.Value + "]";
      if (nullable2.HasValue)
        str1 = str1 + " [level=" + (object) nullable2.Value + "]";
      si.Instance.ScriptCatchupCommands.Add(str1);
    }

    private void CommandCanEquip(ScriptInstance si, BinaryReader reader)
    {
      Item itemID = (Item) reader.ReadUInt16();
      bool flag = reader.ReadBoolean();
      if (si.Player != null && si.Player.CanUseItem(itemID) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandCaveIn(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      int seed = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      si.Instance.StartCaveIn(p, seed, false);
    }

    private void CommandCCTV(ScriptInstance si, BinaryReader reader)
    {
      bool flag1 = false;
      BlockFace dir = BlockFace.ProxyDefault;
      bool flag2 = reader.ReadBoolean();
      if (si.Player == null || flag2 && !si.Player.IsAdmin)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p1 = new GlobalPoint3D();
      p1.X = this.ReadInt32(si, reader);
      p1.Y = this.ReadInt32(si, reader);
      p1.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p1, type1))
        return;
      switch ((Block) si.MapTM.GetBlockID(p1))
      {
        case Block.None:
        case Block.InvisibleBarrier:
          ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
          GlobalPoint3D p2 = new GlobalPoint3D();
          if (type2 != ScriptCoordType.None)
          {
            p2.X = this.ReadInt32(si, reader);
            p2.Y = this.ReadInt32(si, reader);
            p2.Z = this.ReadInt32(si, reader);
            if (!this.AdjustCoord(si, ref p2, type2))
              break;
          }
          else
          {
            flag1 = reader.ReadBoolean();
            dir = flag1 ? BlockFace.Backward : (BlockFace) reader.ReadByte();
          }
          int millisecs = reader.ReadBoolean() ? this.ReadInt32(si, reader) : 0;
          int num1 = reader.ReadBoolean() ? this.ReadInt32(si, reader) : 0;
          int num2 = reader.ReadBoolean() ? this.ReadInt32(si, reader) : 0;
          if (num1 > 100)
            num1 = 100;
          if (num2 < 3)
            num2 = 60;
          else if (num2 > 90)
            num2 = 90;
          if (type2 == ScriptCoordType.None)
          {
            si.Player.StartCCTV(p1, dir, millisecs, (float) num2 / 100f, (float) num1 / 100f, flag1 ? si.Actor : (Actor) null);
            break;
          }
          si.Player.StartCCTV(p1, p2, millisecs, (float) num2 / 100f, (float) num1 / 100f);
          break;
      }
    }

    private void CommandClan(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      string str = (string) null;
      int num = 0;
      if (!flag)
      {
        str = reader.ReadString();
        num = this.ReadInt32(si, reader);
        if (num < 0 || num > 74)
          num = (int) byte.MaxValue;
      }
      if (si.Player == null)
        return;
      si.Player.ClanName = str;
      if (num == (int) byte.MaxValue)
        return;
      si.Player.ClanBannerID = num;
    }

    private void CommandCommit(ScriptInstance si, BinaryReader reader)
    {
      si.MapTM.Commit();
      si.NeedsCommit = false;
    }

    private void CommandContext(ScriptInstance si, BinaryReader reader)
    {
      si.SetContext((ScriptContext) reader.ReadByte());
    }

    private void CommandCopyBlock(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p1 = new GlobalPoint3D();
      p1.X = this.ReadInt32(si, reader);
      p1.Y = this.ReadInt32(si, reader);
      p1.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p2 = new GlobalPoint3D();
      p2.X = this.ReadInt32(si, reader);
      p2.Y = this.ReadInt32(si, reader);
      p2.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p1, type1) || !this.AdjustCoord(si, ref p2, type2))
        return;
      MapBlock blockIdAndAux = si.MapTM.GetBlockIDAndAux(p1);
      if (!Globals1.ItemData[(int) blockIdAndAux.BlockID].IsEnabled || si.Instance.IsInZoneType(p2, ZoneType.Spawn, Globals2.GetGamerType(GamerType.Script)))
        return;
      Block blockId = (Block) blockIdAndAux.BlockID;
      if (blockId == Block.Chest)
        blockIdAndAux.AuxData = (byte) 0;
      DataBlock dataBlock = si.MapTM.MapStrategyTM.GetDataBlock(p1);
      si.Instance.AddBlockNoPermissionCheck(p2, blockId, blockIdAndAux.AuxData, si.Method, Globals2.GetGamerType(GamerType.Script), false, false, false, (object) null);
      if (dataBlock != null)
        this.CopyDataBlock(si, p2, (Block) blockIdAndAux.BlockID, dataBlock, si.Method);
      si.NeedsCommit = true;
    }

    private void CopyDataBlock(
      ScriptInstance si,
      GlobalPoint3D p,
      Block blockID,
      DataBlock srcBlock,
      UpdateBlockMethod method)
    {
      DataBlock orAddDataBlock = si.MapTM.MapStrategyTM.GetOrAddDataBlock(p, blockID, method, GamerID.Sys1, true);
      if (orAddDataBlock == null)
        return;
      orAddDataBlock.CopyFrom(srcBlock);
      orAddDataBlock.Point = p;
      if (blockID != Block.Sign || !(srcBlock as SignBlock).HasText)
        return;
      si.Instance.MapRenderer.SignsChanged(false);
    }

    private void CommandCopyRegion(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D min = new GlobalPoint3D();
      min.X = this.ReadInt32(si, reader);
      min.Y = this.ReadInt32(si, reader);
      min.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D max = new GlobalPoint3D();
      max.X = this.ReadInt32(si, reader);
      max.Y = this.ReadInt32(si, reader);
      max.Z = this.ReadInt32(si, reader);
      ScriptCoordType type3 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref min, type1) || !this.AdjustCoord(si, ref max, type2) || !this.AdjustCoord(si, ref p, type3))
        return;
      this.SetMinMax(ref min, ref max);
      si.Instance.CreativeModeHelper.RunCopy(Globals2.GetGamerType(GamerType.Script), min, max, p, "Copy", false);
      si.NeedsCommit = true;
    }

    private void CommandElse(ScriptInstance si, BinaryReader reader)
    {
      while (reader.BaseStream.Position < reader.BaseStream.Length)
      {
        byte num1 = reader.ReadByte();
        ushort num2 = reader.ReadUInt16();
        reader.BaseStream.Position += (long) num2;
        if (num1 == (byte) 22)
        {
          si.SetContext(si.DefaultContext);
          si.PC = reader.BaseStream.Position;
          si.UpdatePC = false;
          return;
        }
      }
      si.PC = reader.BaseStream.Position;
      si.UpdatePC = false;
    }

    private void CommandEndif(ScriptInstance si, BinaryReader reader)
    {
      si.SetContext(si.DefaultContext);
    }

    private void CommandEquip(ScriptInstance si, BinaryReader reader)
    {
      InventoryHand inventoryHand = (InventoryHand) reader.ReadByte();
      Item itemID = (Item) reader.ReadUInt16();
      if (si.Actor == null)
        return;
      if (inventoryHand != InventoryHand.None)
        si.Actor.EquipFromInventory(inventoryHand == InventoryHand.Left ? si.Actor.LeftHand : si.Actor.RightHand, itemID);
      else
        si.Actor.EquipFromInventory(itemID);
    }

    private void CommandExit(ScriptInstance si, BinaryReader reader)
    {
      si.PC = reader.BaseStream.Length;
      si.UpdatePC = false;
    }

    private void CommandExplosion(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      int radius = this.ReadInt32(si, reader);
      int num = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      if (radius < 1)
        radius = 5;
      else if (radius > 25)
        radius = 25;
      if (num < 1)
        num = radius;
      else if (num > 300)
        num = 300;
      si.Instance.CreateRemoteBlast(p, Item.None, (float) num, radius, si.Player != null ? si.Player.GamerID : Globals2.GetGamerType(GamerType.Script), (ushort) si.Instance.Random.Next());
    }

    private void CommandFog(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (reader.ReadBoolean())
      {
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        si.Instance.MapStrategyTM.EnvManager.RemoveFog(p, false);
      }
      else
      {
        float radius = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 2000f);
        float duration = this.ReadSingle(si, reader);
        float num = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 100f);
        int visibility = -1;
        Color? nullable = this.ReadColorNull(si, reader);
        if (reader.ReadBoolean())
        {
          visibility = this.ReadInt32(si, reader);
          if (visibility < 2)
            visibility = 2;
          else if (visibility > 100)
            visibility = 100;
        }
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        if ((double) duration < 5.0)
          duration = 5f;
        if (nullable.HasValue)
        {
          if (visibility >= 0)
            si.Instance.MapStrategyTM.EnvManager.AddFog(p, radius, duration, 5f, num / 100f, nullable.Value, visibility, false);
          else
            si.Instance.MapStrategyTM.EnvManager.AddFog(p, radius, duration, num / 100f, nullable.Value, false);
        }
        else if (visibility >= 0)
          si.Instance.MapStrategyTM.EnvManager.AddFog(p, radius, duration, num / 100f, visibility, false);
        else
          si.Instance.MapStrategyTM.EnvManager.AddFog(p, radius, duration, num / 100f, false);
      }
    }

    private void CommandHail(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (reader.ReadBoolean())
      {
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        si.Instance.MapStrategyTM.EnvManager.RemoveHail(p, false);
      }
      else
      {
        float radius = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 2000f);
        float duration = this.ReadSingle(si, reader);
        float num1 = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 100f);
        float num2 = 0.0f;
        float num3 = 0.0f;
        Color? nullable = this.ReadColorNull(si, reader);
        if (reader.ReadBoolean())
        {
          num2 = this.ReadSingle(si, reader);
          num3 = this.ReadSingle(si, reader);
        }
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        if ((double) duration < 5.0)
          duration = 5f;
        if (nullable.HasValue)
        {
          if ((double) num2 > 0.0)
            si.Instance.MapStrategyTM.EnvManager.AddHail(p, radius, duration, 5f, num1 / 100f, nullable.Value, num2 / 100f, num3 / 100f, false);
          else
            si.Instance.MapStrategyTM.EnvManager.AddHail(p, radius, duration, num1 / 100f, nullable.Value, false);
        }
        else if ((double) num2 > 0.0)
          si.Instance.MapStrategyTM.EnvManager.AddHail(p, radius, duration, num1 / 100f, num2 / 100f, num3 / 100f, false);
        else
          si.Instance.MapStrategyTM.EnvManager.AddHail(p, radius, duration, num1 / 100f, false);
      }
    }

    private void CommandHasAction(ScriptInstance si, BinaryReader reader)
    {
      ItemAction action = (ItemAction) reader.ReadByte();
      Item itemID = (Item) reader.ReadUInt16();
      ScriptComparison compare = this.ReadComparison(si, reader);
      if (this.GetHasActionResult(si, action, itemID, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private bool GetHasActionResult(
      ScriptInstance si,
      ItemAction action,
      Item itemID,
      ScriptComparison compare)
    {
      if (si.Player == null)
        return false;
      if (compare.Type == Parser.CompareState.Binary)
        return si.Player.ActionLog.HasAction(itemID, action) == compare.BoolResult;
      return Parser.Compare(si.Player.ActionLog.GetAction(itemID, action), this.GetCompareCount(si, compare), compare.Type);
    }

    private void CommandHasHistory(ScriptInstance si, BinaryReader reader)
    {
      string name = Globals2.SubstituteGeneral(si, reader.ReadString());
      ScriptComparison compare = this.ReadComparison(si, reader);
      if (this.GetHasHistoryResult(si, name, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private bool GetHasHistoryResult(ScriptInstance si, string name, ScriptComparison compare)
    {
      if (compare.Target == ScriptTarget.Actor)
      {
        if (compare.Type == Parser.CompareState.Binary)
        {
          if (si.Player == null)
            return false;
          return si.Player.History.HasHistory(name) == compare.BoolResult;
        }
        if (si.Player == null)
          return false;
        return Parser.Compare((float) si.Player.History.GetHistory(name), (float) this.GetCompareCount(si, compare), compare.Type);
      }
      if (compare.Target == ScriptTarget.Clan)
      {
        if (compare.Type == Parser.CompareState.Binary)
        {
          if (si.Player == null)
            return false;
          History clanHistory = si.Instance.GetClanHistory(si.Player.ClanName);
          if (clanHistory == null)
            return false;
          return clanHistory.HasHistory(name) == compare.BoolResult;
        }
        if (si.Player == null)
          return false;
        History clanHistory1 = si.Instance.GetClanHistory(si.Player.ClanName);
        if (clanHistory1 == null)
          return false;
        return Parser.Compare((float) clanHistory1.GetHistory(name), (float) this.GetCompareCount(si, compare), compare.Type);
      }
      if (compare.Type == Parser.CompareState.Binary)
        return si.Instance.History.HasHistory(name) == compare.BoolResult;
      return Parser.Compare((float) si.Instance.History.GetHistory(name), (float) this.GetCompareCount(si, compare), compare.Type);
    }

    private void CommandHasInventory(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      bool isPlayer = type == ScriptCoordType.None;
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      if (!isPlayer)
      {
        zero.X = this.ReadInt32(si, reader);
        zero.Y = this.ReadInt32(si, reader);
        zero.Z = this.ReadInt32(si, reader);
      }
      Item itemID = (Item) reader.ReadUInt16();
      ScriptComparison compare = this.ReadComparison(si, reader);
      if ((isPlayer || this.AdjustCoord(si, ref zero, type)) && this.GetHasInventoryResult(si, isPlayer, zero, itemID, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private bool GetHasInventoryResult(
      ScriptInstance si,
      bool isPlayer,
      GlobalPoint3D p,
      Item itemID,
      ScriptComparison compare)
    {
      int actual = 0;
      if (isPlayer)
      {
        if (si.Player == null)
          return false;
        actual = si.Player.Inventory.GetItemCount(itemID);
      }
      else
      {
        ChestBlock dataBlock = si.Instance.MapStrategyTM.GetDataBlock(p) as ChestBlock;
        if (dataBlock != null)
          actual = dataBlock.Inventory.GetItemCount(itemID);
      }
      if (compare.Type != Parser.CompareState.Binary)
        return Parser.Compare(actual, this.GetCompareCount(si, compare), compare.Type);
      return actual > 0 == compare.BoolResult;
    }

    private void CommandHasMarker(ScriptInstance si, BinaryReader reader)
    {
      string label = reader.ReadString();
      bool flag = reader.ReadBoolean();
      if (si.Instance.GetMapMarker(label).HasValue == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandHasPermission(ScriptInstance si, BinaryReader reader)
    {
      Permissions permission = (Permissions) reader.ReadUInt16();
      bool flag = reader.ReadBoolean();
      if (si.Player != null && si.Player.HasPermission(permission) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandHasActor(ScriptInstance si, BinaryReader reader)
    {
      ScriptPlayerProperty scriptPlayerProperty = (ScriptPlayerProperty) reader.ReadByte();
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      int num1 = this.ReadInt32(si, reader);
      bool flag1 = reader.ReadBoolean();
      bool flag2 = false;
      if (si.Actor != null)
      {
        double num2;
        switch (scriptPlayerProperty)
        {
          case ScriptPlayerProperty.Health:
            num2 = (double) si.Actor.Health;
            break;
          case ScriptPlayerProperty.Oxygen:
            num2 = (double) si.Actor.Oxygen;
            break;
          default:
            num2 = 0.0;
            break;
        }
        float actual = (float) num2;
        if (!flag1)
        {
          flag2 = Parser.Compare(actual, (float) num1, compare);
        }
        else
        {
          double num3;
          switch (scriptPlayerProperty)
          {
            case ScriptPlayerProperty.Health:
              num3 = (double) si.Actor.MaxHealth;
              break;
            case ScriptPlayerProperty.Oxygen:
              num3 = (double) si.Actor.MaxOxygen;
              break;
            default:
              num3 = 0.0;
              break;
          }
          float num4 = (float) num3;
          flag2 = Parser.Compare(actual, (float) ((double) num4 * (double) num1 / 100.0), compare);
        }
      }
      if (flag2)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandHasSkill(ScriptInstance si, BinaryReader reader)
    {
      SkillType skillType = (SkillType) reader.ReadByte();
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      int compareWith = this.ReadInt32(si, reader);
      int num;
      switch (skillType)
      {
        case SkillType.Combat:
          num = si.Actor.SkillsData.CombatLevel;
          break;
        case SkillType.Total:
          num = si.Actor.SkillsData.TotalLevel;
          break;
        default:
          num = si.Actor.SkillsData[(int) skillType].Level;
          break;
      }
      int actual = num;
      if (si.Player != null && Parser.Compare(actual, compareWith, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandHasStatBonus(ScriptInstance si, BinaryReader reader)
    {
      if (si.Player == null)
      {
        this.JumpToFalseConditional(si, reader);
      }
      else
      {
        SkillType skillType = (SkillType) reader.ReadByte();
        Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
        int compareWith = this.ReadInt32(si, reader);
        int actual = 0;
        switch (skillType)
        {
          case SkillType.Health:
            actual = si.Player.HealthTotalItemBonus();
            break;
          case SkillType.Strength:
            actual = si.Player.StrengthTotalItemBonus();
            break;
          case SkillType.Attack:
            actual = si.Player.AttackTotalItemBonus();
            break;
          case SkillType.Defence:
            actual = si.Player.DefenceTotalItemBonus();
            break;
          case SkillType.Ranged:
            actual = si.Player.RangedTotalItemBonus();
            break;
          case SkillType.Looting:
            actual = si.Player.LootingTotalItemBonus();
            break;
        }
        if (Parser.Compare(actual, compareWith, compare))
          return;
        this.JumpToFalseConditional(si, reader);
      }
    }

    private void CommandHealth(ScriptInstance si, BinaryReader reader)
    {
      if (si.Actor == null)
        return;
      bool flag1 = reader.ReadBoolean();
      string name = reader.ReadBoolean() ? reader.ReadString() : (string) null;
      if (flag1)
      {
        if (name == null)
          si.Actor.EffectDeleteAll();
        else
          si.Actor.EffectDelete(name);
      }
      else
      {
        int qty = this.ReadInt32(si, reader);
        int millisecs = this.ReadInt32(si, reader);
        bool flag2 = reader.ReadBoolean();
        string history = flag2 ? reader.ReadString() : (string) null;
        int duration = 0;
        if (!flag2 && reader.ReadBoolean())
          duration = this.ReadInt32(si, reader);
        if (millisecs < 0)
          millisecs = 0;
        else if (millisecs > 0 && millisecs < 16)
          millisecs = 16;
        if (millisecs > 0)
        {
          if (duration > 0)
            si.Actor.EffectAddHealth(name, qty, millisecs, duration);
          else
            si.Actor.EffectAddHealth(name, qty, millisecs, history);
        }
        else
        {
          this.healthEffect.Points = qty;
          this.healthEffect.Update((ITMActor) si.Actor, (ITMActor) si.OrigActor);
        }
      }
    }

    private void CommandHistory(ScriptInstance si, BinaryReader reader)
    {
      string key = Globals2.SubstituteGeneral(si, reader.ReadString());
      ScriptTarget scriptTarget = (ScriptTarget) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      ScriptNumType scriptNumType = ScriptNumType.Inc;
      long i = 0;
      if (!flag)
      {
        scriptNumType = (ScriptNumType) reader.ReadByte();
        i = (long) this.ReadVarNumber(si, reader);
      }
      History history = (History) null;
      switch (scriptTarget)
      {
        case ScriptTarget.Actor:
          if (si.Player != null)
          {
            history = si.Player.History;
            break;
          }
          break;
        case ScriptTarget.Clan:
          if (si.Player != null)
          {
            history = si.Instance.GetClanHistory(si.Player.ClanName);
            break;
          }
          break;
        default:
          history = si.Instance.History;
          break;
      }
      if (flag)
      {
        history?.ClearHistory(key);
      }
      else
      {
        if (i == 0L)
          return;
        if (history == null && scriptTarget == ScriptTarget.Clan && si.Player != null)
          history = si.Instance.GetOrCreateClanHistory(si.Player.ClanName);
        if (history == null)
          return;
        if (scriptNumType == ScriptNumType.Abs)
          history.SetHistory(key, i);
        else
          history.AdjustHistory(key, scriptNumType == ScriptNumType.Dec ? -i : i);
      }
    }

    private void CommandHUDBar(ScriptInstance si, BinaryReader reader)
    {
      string name = reader.ReadString();
      ScriptTarget scriptTarget1 = (ScriptTarget) reader.ReadByte();
      if (scriptTarget1 == ScriptTarget.Actor && si.Player == null)
        return;
      HUDElementManager hudElementManager = scriptTarget1 == ScriptTarget.Actor ? si.Player.HUDElementManager : si.Instance.HUDElementManager;
      if (reader.ReadBoolean())
      {
        hudElementManager.RemoveElement(name);
      }
      else
      {
        ScriptTarget scriptTarget2 = (ScriptTarget) reader.ReadByte();
        string historyKey = reader.ReadString();
        int maxValue = this.ReadInt32(si, reader);
        Rectangle rect = new Rectangle(this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader));
        float scale = this.ReadSingle(si, reader);
        Color? nullable = this.ReadColorNull(si, reader);
        HUDElementProps props = (HUDElementProps) reader.ReadByte();
        if (maxValue < 2)
          maxValue = 2;
        if (scriptTarget2 == ScriptTarget.Actor && si.Player == null)
          return;
        History history = scriptTarget2 == ScriptTarget.Actor ? si.Player.History : si.Instance.History;
        hudElementManager.AddBar(name, history, historyKey, maxValue, rect, scale, nullable.Value, props);
      }
    }

    private void CommandHUDCounter(ScriptInstance si, BinaryReader reader)
    {
      string name = reader.ReadString();
      ScriptTarget scriptTarget1 = (ScriptTarget) reader.ReadByte();
      if (scriptTarget1 == ScriptTarget.Actor && si.Player == null)
        return;
      HUDElementManager hudElementManager = scriptTarget1 == ScriptTarget.Actor ? si.Player.HUDElementManager : si.Instance.HUDElementManager;
      if (reader.ReadBoolean())
      {
        hudElementManager.RemoveElement(name);
      }
      else
      {
        ScriptTarget scriptTarget2 = (ScriptTarget) reader.ReadByte();
        string historyKey = reader.ReadString();
        Vector2 pos = new Vector2(this.ReadSingle(si, reader), this.ReadSingle(si, reader));
        float scale = this.ReadSingle(si, reader);
        Color? nullable = this.ReadColorNull(si, reader);
        HUDElementProps props = (HUDElementProps) reader.ReadByte();
        if (scriptTarget2 == ScriptTarget.Actor && si.Player == null)
          return;
        History history = scriptTarget2 == ScriptTarget.Actor ? si.Player.History : si.Instance.History;
        hudElementManager.AddCounter(name, history, historyKey, pos, scale, nullable.Value, props);
      }
    }

    private void CommandHUDShape(ScriptInstance si, BinaryReader reader)
    {
      string name = reader.ReadString();
      int index = reader.ReadBoolean() ? this.ReadInt32(si, reader) : -1;
      ScriptTarget scriptTarget = (ScriptTarget) reader.ReadByte();
      if (scriptTarget == ScriptTarget.Actor && si.Player == null)
        return;
      HUDElementManager hudElementManager = scriptTarget == ScriptTarget.Actor ? si.Player.HUDElementManager : si.Instance.HUDElementManager;
      if (reader.ReadBoolean())
      {
        hudElementManager.RemoveElement(name, index);
      }
      else
      {
        Rectangle rect = new Rectangle(this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader));
        Color? nullable = this.ReadColorNull(si, reader);
        HUDElementProps props = (HUDElementProps) reader.ReadByte();
        hudElementManager.AddShape(name, index, rect, nullable.Value, props);
      }
    }

    private void CommandHUDText(ScriptInstance si, BinaryReader reader)
    {
      string name = reader.ReadString();
      ScriptTarget scriptTarget = (ScriptTarget) reader.ReadByte();
      if (scriptTarget == ScriptTarget.Actor && si.Player == null)
        return;
      HUDElementManager hudElementManager = scriptTarget == ScriptTarget.Actor ? si.Player.HUDElementManager : si.Instance.HUDElementManager;
      if (reader.ReadBoolean())
      {
        hudElementManager.RemoveElement(name);
      }
      else
      {
        string text = Globals2.SubstituteText(si, reader.ReadString());
        Vector2 pos = new Vector2(this.ReadSingle(si, reader), this.ReadSingle(si, reader));
        Color? nullable = this.ReadColorNull(si, reader);
        HUDElementProps props = (HUDElementProps) reader.ReadByte();
        float scale = this.ReadSingle(si, reader);
        float rot = this.ReadSingle(si, reader);
        hudElementManager.AddText(name, text, pos, scale, rot, nullable.Value, props);
      }
    }

    private void CommandInput(ScriptInstance si, BinaryReader reader)
    {
      if (si.Player == null || si.Instance.IsScriptedScreenOpen(si.Player))
        return;
      ushort num = reader.ReadUInt16();
      if (!si.Player.IsLocalGamer)
      {
        if (si.InputTarget(si))
        {
          ScriptRuntime.NumInputState numInputState = new ScriptRuntime.NumInputState()
          {
            si = si,
            VarIndex = num,
            Transmit = false
          };
          this.NumberEntered(si.InputResult.HasValue ? si.InputResult.Value : 0.0, !si.InputResult.HasValue, (object) numInputState);
        }
        else
        {
          si.WaitingOnResult = long.MaxValue;
          si.PC = si.CurentCmdPC;
          si.UpdatePC = false;
        }
      }
      else
      {
        ScriptRuntime.NumInputState numInputState = new ScriptRuntime.NumInputState()
        {
          si = si,
          VarIndex = num,
          Transmit = true
        };
        si.Instance.OpenNumberInput(si.Player, si.Vars[(int) num], new NumberEntered(this.NumberEntered), (object) numInputState);
        si.WaitTime = long.MaxValue;
      }
    }

    private void NumberEntered(double number, bool isCancelled, object state)
    {
      if (state == null)
        return;
      ScriptRuntime.NumInputState numInputState = (ScriptRuntime.NumInputState) state;
      ScriptInstance si = numInputState.si;
      if (si == null)
        return;
      double? val = new double?();
      if (!isCancelled && numInputState.VarIndex >= (ushort) 0 && (int) numInputState.VarIndex < si.VarCount)
      {
        si.Vars[(int) numInputState.VarIndex] = number;
        val = new double?(number);
      }
      si.WaitTime = 0L;
      if (!numInputState.Transmit)
        return;
      GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
      si.Instance.NetworkManager.SendScriptInputResult(si.Script.Name, gamerID, val);
    }

    private void CommandIntersect(ScriptInstance si, BinaryReader reader)
    {
      si.Target = (Actor) null;
      ScriptShape scriptShape = (ScriptShape) reader.ReadByte();
      HitTargetOptions options = (HitTargetOptions) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      Vector3 pos1 = new Vector3();
      pos1.X = this.ReadSingle(si, reader);
      pos1.Y = this.ReadSingle(si, reader);
      pos1.Z = this.ReadSingle(si, reader);
      ScriptCoordType type2 = ScriptCoordType.None;
      Vector3 pos2 = new Vector3();
      float radius = 0.0f;
      float degrees = 0.0f;
      switch (scriptShape)
      {
        case ScriptShape.Ray:
        case ScriptShape.Box:
          type2 = (ScriptCoordType) reader.ReadByte();
          pos2.X = this.ReadSingle(si, reader);
          pos2.Y = this.ReadSingle(si, reader);
          pos2.Z = this.ReadSingle(si, reader);
          break;
        case ScriptShape.Sphere:
          radius = this.ReadSingle(si, reader);
          break;
        case ScriptShape.Frustum:
          type2 = (ScriptCoordType) reader.ReadByte();
          pos2.X = this.ReadSingle(si, reader);
          pos2.Y = this.ReadSingle(si, reader);
          pos2.Z = this.ReadSingle(si, reader);
          degrees = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 120f);
          break;
      }
      if (!this.AdjustCoord(si, ref pos1, type1, false) || type2 != ScriptCoordType.None && !this.AdjustCoord(si, ref pos2, type2, false))
        return;
      if (si.Player != null && !si.Player.IsLocalGamer)
      {
        if (si.IntersectTarget(si))
        {
          if (!si.NegativeResult)
            return;
          si.NegativeResult = false;
          this.JumpToFalseConditional(si, reader);
        }
        else
        {
          si.WaitingOnResult = this.clock.ElapsedMilliseconds + 20000L;
          si.PC = si.CurentCmdPC;
          si.UpdatePC = false;
        }
      }
      else
      {
        switch (scriptShape)
        {
          case ScriptShape.Ray:
            Ray ray = new Ray();
            ray.Position = pos1;
            Vector3 vector3 = pos2 - pos1;
            float length = vector3.Length();
            ray.Direction = Vector3.Normalize(vector3);
            if (flag)
              si.Instance.AddScriptIntersectDisplay(ray, length);
            HitTarget hitTarget1 = si.Instance.BuildHitTarget(ray, si.Actor, options, (List<ActorType>) null);
            if (hitTarget1.Target != null && (double) hitTarget1.Distance <= (double) length)
            {
              HitTest hitTest = si.Instance.CalcBlockTarget(ray.Position, ray.Direction, hitTarget1.Distance, (List<byte>) null, false, true, false, false);
              si.LastRayHit = hitTest.Point;
              if (!hitTest.IsValid || (double) hitTest.Distance > (double) hitTarget1.Distance)
              {
                si.Target = hitTarget1.Target;
                break;
              }
              this.JumpToFalseConditional(si, reader);
              break;
            }
            this.JumpToFalseConditional(si, reader);
            break;
          case ScriptShape.Box:
            BoundingBox box = new BoundingBox(Vector3.Min(pos1, pos2), Vector3.Max(pos1, pos2));
            if (flag)
              si.Instance.AddScriptIntersectDisplay(box);
            HitTarget hitTarget2 = si.Instance.BuildHitTarget(box, si.Actor, options);
            if (hitTarget2.Target != null)
            {
              si.Target = hitTarget2.Target;
              break;
            }
            this.JumpToFalseConditional(si, reader);
            break;
          case ScriptShape.Sphere:
            BoundingSphere sphere = new BoundingSphere(pos1, radius);
            if (flag)
              si.Instance.AddScriptIntersectDisplay(sphere);
            HitTarget hitTarget3 = si.Instance.BuildHitTarget(sphere, si.Actor, options);
            if (hitTarget3.Target != null)
            {
              si.Target = hitTarget3.Target;
              break;
            }
            this.JumpToFalseConditional(si, reader);
            break;
          case ScriptShape.Frustum:
            this.hitFrustum.Matrix = Matrix.CreateLookAt(pos1, pos2, Vector3.Up) * Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(degrees), CoreGlobals.GraphicsDevice.Viewport.AspectRatio, 0.1f, Vector3.Distance(pos1, pos2));
            if (flag)
              si.Instance.AddScriptIntersectDisplay(this.hitFrustum.Matrix);
            HitTarget hitTarget4 = si.Instance.BuildHitTarget(this.hitFrustum, si.Actor, options);
            if (hitTarget4.Target != null)
            {
              si.Target = hitTarget4.Target;
              break;
            }
            this.JumpToFalseConditional(si, reader);
            break;
        }
        GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
        GamerID targetID = si.Target != null ? si.Target.GamerID : GamerID.Sys1;
        si.Instance.NetworkManager.SendScriptIntersectResult(si.Script.Name, gamerID, targetID);
      }
    }

    private void CommandInventory(ScriptInstance si, BinaryReader reader)
    {
      bool flag1 = reader.ReadBoolean();
      bool flag2 = false;
      ScriptInventoryCmdType cmd = (ScriptInventoryCmdType) reader.ReadByte();
      Item itemID = (Item) reader.ReadUInt16();
      int num = itemID == Item.None || cmd == ScriptInventoryCmdType.Clear ? 0 : this.ReadInt32(si, reader);
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      ScriptCoordType scriptCoordType1 = ScriptCoordType.None;
      ScriptCoordType scriptCoordType2 = ScriptCoordType.None;
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      if (type != ScriptCoordType.None)
      {
        p1.X = this.ReadInt32(si, reader);
        p1.Y = this.ReadInt32(si, reader);
        p1.Z = this.ReadInt32(si, reader);
        if (!this.AdjustCoord(si, ref p1, type))
          return;
        flag2 = reader.ReadBoolean();
        scriptCoordType1 = (ScriptCoordType) reader.ReadByte();
        if (scriptCoordType1 != ScriptCoordType.None)
        {
          p2.X = this.ReadInt32(si, reader);
          p2.Y = this.ReadInt32(si, reader);
          p2.Z = this.ReadInt32(si, reader);
          if (!this.AdjustCoord(si, ref p2, scriptCoordType1))
            return;
          scriptCoordType2 = (ScriptCoordType) reader.ReadByte();
          if (scriptCoordType2 != ScriptCoordType.None)
          {
            p3.X = this.ReadInt32(si, reader);
            p3.Y = this.ReadInt32(si, reader);
            p3.Z = this.ReadInt32(si, reader);
            if (!this.AdjustCoord(si, ref p3, scriptCoordType2))
              return;
          }
        }
      }
      switch (cmd)
      {
        case ScriptInventoryCmdType.Add:
        case ScriptInventoryCmdType.Copy:
          if (!si.Instance.IsCreativeMode && (si.Actor.IsPlayer || si.Actor.Properties.DropInventoryOnDeath.HasValue && si.Actor.Properties.DropInventoryOnDeath.Value))
            return;
          break;
      }
      if (flag1)
      {
        if (si.Actor == null)
          return;
        if (cmd == ScriptInventoryCmdType.Clear)
          this.ClearInventory((Inventory) si.Actor.Inventory, itemID);
        else if (type != ScriptCoordType.None)
        {
          MapStrategyTM mapStrategyTm = si.Instance.MapStrategyTM;
          ChestBlock chestBlock = mapStrategyTm.GetDataBlock(p1) as ChestBlock;
          if (chestBlock == null)
          {
            Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p1);
            if (blockIdNoCache != Block.None)
            {
              chestBlock = mapStrategyTm.NewDataBlock(p1, blockIdNoCache, Globals2.GetGamerType(GamerType.Script)) as ChestBlock;
              if (chestBlock != null)
                mapStrategyTm.AddDataBlock((DataBlock) chestBlock, UpdateBlockMethod.Strategy);
            }
          }
          if (chestBlock == null)
            return;
          if (itemID != Item.None)
          {
            InventoryItem inventoryItem = new InventoryItem(itemID, num);
            if (flag2)
              si.Actor.Inventory.MoveFrom(chestBlock.Inventory, inventoryItem);
            else
              chestBlock.Inventory.MoveFrom((Inventory) si.Actor.Inventory, inventoryItem);
          }
          else if (flag2)
            si.Actor.Inventory.MoveFrom(chestBlock.Inventory);
          else
            chestBlock.Inventory.MoveFrom((Inventory) si.Actor.Inventory);
          if (chestBlock.HasPlayer || chestBlock.Inventory.HasItems())
            return;
          mapStrategyTm.RemoveDataBlock((DataBlock) chestBlock, GamerType.Script);
        }
        else
        {
          switch (cmd)
          {
            case ScriptInventoryCmdType.Add:
              if (itemID == Item.None || !ItemData.IsEnabled(itemID))
                break;
              si.Actor.Inventory.AddToInventory(itemID, num);
              break;
            case ScriptInventoryCmdType.Take:
              si.Actor.Inventory.DecrementItem(itemID, num, true);
              break;
          }
        }
      }
      else
      {
        if (type == ScriptCoordType.None)
          return;
        switch (cmd)
        {
          case ScriptInventoryCmdType.Clear:
            this.ClearInventoryRange(si, p1, scriptCoordType1, p2, itemID);
            break;
          case ScriptInventoryCmdType.Add:
          case ScriptInventoryCmdType.Take:
            this.AddTakeInventoryRange(si, p1, scriptCoordType1, p2, cmd, itemID, num);
            break;
          case ScriptInventoryCmdType.Copy:
          case ScriptInventoryCmdType.Move:
            this.CopyMoveInventoryRange(si, p1, scriptCoordType1, p2, scriptCoordType2, p3, cmd, itemID, num);
            break;
        }
      }
    }

    private void ClearInventory(Inventory inventory, Item itemID)
    {
      if (itemID != Item.None)
        inventory.ClearItem(itemID);
      else
        inventory.ClearItems();
    }

    private void ClearInventoryRange(
      ScriptInstance si,
      GlobalPoint3D p1,
      ScriptCoordType coord2,
      GlobalPoint3D p2,
      Item itemID)
    {
      if (coord2 == ScriptCoordType.None)
        p2 = p1;
      else
        this.SetMinMax(ref p1, ref p2);
      MapStrategyTM mapStrategyTm = si.Instance.MapStrategyTM;
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Y = p1.Y; p.Y <= p2.Y; ++p.Y)
      {
        for (p.Z = p1.Z; p.Z <= p2.Z; ++p.Z)
        {
          for (p.X = p1.X; p.X <= p2.X; ++p.X)
          {
            ChestBlock dataBlock = mapStrategyTm.GetDataBlock(p) as ChestBlock;
            if (dataBlock != null)
              this.ClearInventory(dataBlock.Inventory, itemID);
          }
        }
      }
    }

    private void AddTakeInventoryRange(
      ScriptInstance si,
      GlobalPoint3D p1,
      ScriptCoordType coord2,
      GlobalPoint3D p2,
      ScriptInventoryCmdType cmd,
      Item itemID,
      int qty)
    {
      if (qty < 1 || itemID == Item.None || cmd == ScriptInventoryCmdType.Add && !ItemData.IsEnabled(itemID))
        return;
      if (coord2 == ScriptCoordType.None)
        p2 = p1;
      else
        this.SetMinMax(ref p1, ref p2);
      MapStrategyTM mapStrategyTm = si.Instance.MapStrategyTM;
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Y = p1.Y; p.Y <= p2.Y; ++p.Y)
      {
        for (p.Z = p1.Z; p.Z <= p2.Z; ++p.Z)
        {
          for (p.X = p1.X; p.X <= p2.X; ++p.X)
          {
            ChestBlock chestBlock = mapStrategyTm.GetDataBlock(p) as ChestBlock;
            if (chestBlock == null && cmd == ScriptInventoryCmdType.Add)
            {
              Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p);
              if (blockIdNoCache != Block.None)
              {
                chestBlock = mapStrategyTm.NewDataBlock(p, blockIdNoCache, Globals2.GetGamerType(GamerType.Script)) as ChestBlock;
                if (chestBlock != null)
                  mapStrategyTm.AddDataBlock((DataBlock) chestBlock, UpdateBlockMethod.Strategy);
              }
            }
            if (chestBlock != null)
            {
              if (cmd == ScriptInventoryCmdType.Add)
                chestBlock.Inventory.AddToInventory(new InventoryItem(itemID)
                {
                  Count = qty
                }, !chestBlock.Inventory.AllowZeroCountItems);
              else
                chestBlock.Inventory.DecrementItem(itemID, qty, true);
            }
          }
        }
      }
    }

    private void CopyMoveInventoryRange(
      ScriptInstance si,
      GlobalPoint3D p1,
      ScriptCoordType coord2,
      GlobalPoint3D p2,
      ScriptCoordType coord3,
      GlobalPoint3D p3,
      ScriptInventoryCmdType cmd,
      Item itemID,
      int qty)
    {
      if (coord3 == ScriptCoordType.None)
      {
        p3 = p2;
        p2 = p1;
      }
      else
        this.SetMinMax(ref p1, ref p2);
      MapStrategyTM mapStrategyTm = si.Instance.MapStrategyTM;
      ChestBlock chestBlock = mapStrategyTm.GetDataBlock(p3) as ChestBlock;
      if (chestBlock == null)
      {
        Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p3);
        if (blockIdNoCache != Block.None)
        {
          chestBlock = mapStrategyTm.NewDataBlock(p3, blockIdNoCache, Globals2.GetGamerType(GamerType.Script)) as ChestBlock;
          if (chestBlock == null)
            return;
        }
      }
      InventoryItem? nullable = new InventoryItem?();
      if (itemID != Item.None)
        nullable = new InventoryItem?(new InventoryItem(itemID, qty));
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Y = p1.Y; p.Y <= p2.Y; ++p.Y)
      {
        for (p.Z = p1.Z; p.Z <= p2.Z; ++p.Z)
        {
          for (p.X = p1.X; p.X <= p2.X; ++p.X)
          {
            ChestBlock dataBlock = mapStrategyTm.GetDataBlock(p) as ChestBlock;
            if (dataBlock != null)
            {
              if (cmd == ScriptInventoryCmdType.Copy)
              {
                if (nullable.HasValue)
                  chestBlock.Inventory.CopyFrom(dataBlock.Inventory, nullable.Value);
                else
                  chestBlock.Inventory.CopyFrom(dataBlock.Inventory);
              }
              else if (nullable.HasValue)
                chestBlock.Inventory.MoveFrom(dataBlock.Inventory, nullable.Value);
              else
                chestBlock.Inventory.MoveFrom(dataBlock.Inventory);
            }
          }
        }
      }
      mapStrategyTm.AddDataBlock((DataBlock) chestBlock, UpdateBlockMethod.Strategy);
    }

    private void CommandIsAvatar(ScriptInstance si, BinaryReader reader)
    {
      ActorType actorType = (ActorType) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      if (si.Actor != null && si.Actor.ActorType == actorType == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlock(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      Block block = (Block) reader.ReadByte();
      bool flag1 = reader.ReadBoolean();
      byte num = flag1 ? (byte) this.ReadInt32(si, reader) : (byte) 0;
      bool flag2 = reader.ReadBoolean();
      bool flag3 = false;
      if (this.AdjustCoord(si, ref p, type))
      {
        if (!flag1)
        {
          Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p);
          flag3 = block == blockIdNoCache == flag2;
        }
        else
        {
          MapBlock blockIdAndAuxNoCache = si.MapTM.GetBlockIDAndAuxNoCache(p);
          blockIdAndAuxNoCache.AuxData &= (byte) 7;
          flag3 = (block != (Block) blockIdAndAuxNoCache.BlockID ? 0 : ((int) num == (int) blockIdAndAuxNoCache.AuxData ? 1 : 0)) == (flag2 ? 1 : 0);
        }
      }
      if (flag3)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockDeliveringPower(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.Instance.MapStrategyTM.IsBlockDeliveringPower(p) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockEdited(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.MapTM.HasChanged(si.MapTM.GetAuxFullDataNoCache(p)) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockLightSource(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.MapTM.IsBlockLightSource(si.MapTM.GetBlockIDNoCache(p)) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockOpen(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag1 = reader.ReadBoolean();
      bool flag2 = false;
      if (this.AdjustCoord(si, ref p, type))
        flag2 = si.Instance.IsBlockOpen(p) != (short) -1 == flag1;
      if (flag2)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockOre(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.MapTM.IsBlockOre(si.MapTM.GetBlockIDNoCache(p)) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockPassable(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.MapTM.IsBlockPassable(si.MapTM.GetBlockIDNoCache(p)) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockReceivingPower(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.Instance.MapStrategyTM.IsBlockReceivingPower(p) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockResistance(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      int compareWith = this.ReadInt32(si, reader);
      bool flag = false;
      if (this.AdjustCoord(si, ref p, type))
      {
        byte blockId = si.MapTM.GetBlockID(p);
        flag = Parser.Compare((int) Globals1.BlockMaterialData[(int) si.MapTM.BlockData[(int) blockId].Material].Resistance, compareWith, compare);
      }
      if (flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockSolid(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (this.AdjustCoord(si, ref p, type) && si.MapTM.IsBlockSolid(si.MapTM.GetBlockIDNoCache(p)) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsBlockTexture(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      bool flag1 = reader.ReadBoolean();
      int compareWith = this.ReadInt32(si, reader);
      bool flag2 = false;
      if (this.AdjustCoord(si, ref p, type))
      {
        int actual = si.MapTM.GetBlockTextureIndexFromExistingBlock(p);
        if (actual < 0)
          actual = 0;
        flag2 = compare == Parser.CompareState.Binary ? compareWith == actual == flag1 : Parser.Compare(actual, compareWith, compare);
      }
      if (flag2)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsClan(ScriptInstance si, BinaryReader reader)
    {
      string s = reader.ReadString();
      if (s.IsEmpty())
        s = (string) null;
      bool flag = reader.ReadBoolean();
      if (si.Player != null && si.Player.ClanName == s == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsClock(ScriptInstance si, BinaryReader reader)
    {
      ScriptComparison compare = this.ReadComparison(si, reader);
      int currentHour = (int) si.Instance.SunMoon.CurrentHour;
      if (compare.Type == Parser.CompareState.Binary ? currentHour == compare.Count.I == compare.BoolResult : Parser.Compare(currentHour, this.GetCompareCount(si, compare), compare.Type))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsCombat(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      if (si.Instance.IsCombatEnabled == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsDayTime(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      if (si.Instance.SunMoon.IsDayTime == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsDistance(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      double compareWith = this.ReadDouble(si, reader);
      if (si.Actor != null && this.AdjustCoord(si, ref p, type) && Parser.Compare((double) Vector3.Distance(si.Actor.EyePosition, si.MapTM.GetBlockCenter(p)), compareWith, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsEquipped(ScriptInstance si, BinaryReader reader)
    {
      Item itemID = (Item) reader.ReadUInt16();
      InventoryHand inventoryHand = (InventoryHand) reader.ReadByte();
      bool flag1 = reader.ReadBoolean();
      bool flag2 = false;
      if (si.Player != null)
      {
        int num;
        switch (inventoryHand)
        {
          case InventoryHand.Left:
            num = si.Player.LeftHand.ItemID == itemID ? 1 : 0;
            break;
          case InventoryHand.Right:
            num = si.Player.RightHand.ItemID == itemID ? 1 : 0;
            break;
          default:
            num = si.Player.IsItemEquipped(itemID) ? 1 : 0;
            break;
        }
        flag2 = num != 0;
      }
      if (flag2 == flag1)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsFiniteResources(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      if (si.Instance.IsFiniteResources == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsGamerCount(ScriptInstance si, BinaryReader reader)
    {
      bool flag1 = false;
      bool flag2 = true;
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      string name = (string) null;
      float radius = -1f;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      if (type1 != ScriptCoordType.None)
      {
        p1.X = this.ReadInt32(si, reader);
        p1.Y = this.ReadInt32(si, reader);
        p1.Z = this.ReadInt32(si, reader);
        if (this.AdjustCoord(si, ref p1, type1))
        {
          ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
          if (type2 != ScriptCoordType.None)
          {
            p2.X = this.ReadInt32(si, reader);
            p2.Y = this.ReadInt32(si, reader);
            p2.Z = this.ReadInt32(si, reader);
            if (!this.AdjustCoord(si, ref p2, type2))
              flag2 = false;
          }
          else
            radius = this.ReadSingle(si, reader);
        }
        else
          flag2 = false;
      }
      else if (reader.ReadBoolean())
        name = reader.ReadString();
      if (flag2)
      {
        int actual = 0;
        ScriptGamerTarget gamerTarget = (ScriptGamerTarget) reader.ReadByte();
        Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
        int compareWith = this.ReadInt32(si, reader);
        List<NetworkGamer> allEnabledGamers = si.Instance.NetworkManager.AllEnabledGamers;
        if ((double) radius >= 0.0)
        {
          Vector3 blockCenter = si.MapTM.GetBlockCenter(p1);
          actual = this.GetGamerCount(allEnabledGamers, gamerTarget, blockCenter, radius);
        }
        else if (type1 != ScriptCoordType.None)
        {
          Vector3 position1 = si.MapTM.GetPosition(p1);
          Vector3 position2 = si.MapTM.GetPosition(p2);
          this.SetMinMax(ref position1, ref position2);
          position1.Y -= si.MapTM.TileSize;
          position2.X += si.MapTM.TileSize;
          position2.Z += si.MapTM.TileSize;
          actual = this.GetGamerCount(allEnabledGamers, gamerTarget, position1, position2);
        }
        else if (name != null)
        {
          GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
          Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
          if (zone == null && gamerID.IsGamer)
            zone = si.Instance.MapStrategyTM.GetZone(name);
          if (zone != null)
          {
            Vector3 position1 = si.MapTM.GetPosition(zone.Min);
            Vector3 position2 = si.MapTM.GetPosition(zone.Max);
            position1.Y -= si.MapTM.TileSize;
            position2.X += si.MapTM.TileSize;
            position2.Z += si.MapTM.TileSize;
            actual = this.GetGamerCount(allEnabledGamers, gamerTarget, position1, position2);
          }
        }
        else
        {
          int num;
          switch (gamerTarget)
          {
            case ScriptGamerTarget.Local:
              num = si.Instance.NetworkManager.LocalGamerCount;
              break;
            case ScriptGamerTarget.Remote:
              num = si.Instance.NetworkManager.RemoteGamerCount;
              break;
            default:
              num = si.Instance.NetworkManager.AllGamerCount;
              break;
          }
          actual = num;
        }
        flag1 = Parser.Compare(actual, compareWith, compare);
      }
      if (flag1)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private int GetGamerCount(
      List<NetworkGamer> gamerList,
      ScriptGamerTarget gamerTarget,
      Vector3 pos,
      float radius)
    {
      int num1 = 0;
      float num2 = radius * radius;
      foreach (Gamer gamer in gamerList)
      {
        Player tag = gamer.Tag as Player;
        if (tag != null && tag.IsEnabledField && (gamerTarget == ScriptGamerTarget.All || gamerTarget == ScriptGamerTarget.None || gamerTarget == ScriptGamerTarget.Local && tag.IsLocalGamer || gamerTarget == ScriptGamerTarget.Remote && !tag.IsLocalGamer) && (double) Vector3.DistanceSquared(pos, tag.Position) <= (double) num2)
          ++num1;
      }
      return num1;
    }

    private int GetGamerCount(
      List<NetworkGamer> gamerList,
      ScriptGamerTarget gamerTarget,
      Vector3 min,
      Vector3 max)
    {
      int num1 = 0;
      float num2 = 0.075f;
      BoundingBox box = new BoundingBox();
      box.Min.X = min.X + num2;
      box.Min.Y = min.Y + num2;
      box.Min.Z = min.Z + num2;
      box.Max.X = max.X - num2;
      box.Max.Y = max.X - num2;
      box.Max.Z = max.X - num2;
      foreach (Gamer gamer in gamerList)
      {
        Player tag = gamer.Tag as Player;
        if (tag != null && tag.IsEnabledField && tag.Box.Intersects(box) && (gamerTarget == ScriptGamerTarget.All || gamerTarget == ScriptGamerTarget.None || gamerTarget == ScriptGamerTarget.Local && tag.IsLocalGamer || gamerTarget == ScriptGamerTarget.Remote && !tag.IsLocalGamer))
          ++num1;
      }
      return num1;
    }

    private void CommandIsInZone(ScriptInstance si, BinaryReader reader)
    {
      ZoneType type = (ZoneType) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      if (si.Actor != null && si.Instance.IsInZoneType(si.Actor.Box, type, GamerID.Sys1) == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsLight(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool sunLight = reader.ReadBoolean();
      bool moonLight = reader.ReadBoolean();
      bool blockLight = reader.ReadBoolean();
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      bool flag1 = compare != Parser.CompareState.Binary || reader.ReadBoolean();
      int count = compare != Parser.CompareState.Binary ? this.ReadInt32(si, reader) : 0;
      bool flag2 = false;
      if (this.AdjustCoord(si, ref p, type))
      {
        MapLight light = compare == Parser.CompareState.Binary ? this.GetLight((Map) si.MapTM, p) : si.MapTM.GetLightNoCache(p);
        flag2 = this.TestLight((Map) si.MapTM, light, compare, count, sunLight, moonLight, blockLight) == flag1;
      }
      if (flag2)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private bool TestLight(
      Map map,
      MapLight light,
      Parser.CompareState compare,
      int count,
      bool sunLight,
      bool moonLight,
      bool blockLight)
    {
      bool flag = false;
      if (sunLight)
        flag = compare != Parser.CompareState.Binary ? Parser.Compare((float) light.SunLight * map.LightCycle, (float) count, compare) : (double) light.SunLight * (double) map.LightCycle > 0.0;
      if (moonLight && !flag)
        flag = compare != Parser.CompareState.Binary ? Parser.Compare((float) light.SunLight * (1f - map.LightCycle), (float) count, compare) : (double) light.SunLight * (1.0 - (double) map.LightCycle) > 0.0;
      if (blockLight && !flag)
        flag = compare != Parser.CompareState.Binary ? Parser.Compare((int) light.BlockLight, count, compare) : light.BlockLight > (byte) 0;
      return flag;
    }

    private MapLight GetLight(Map map, GlobalPoint3D p)
    {
      MapLight mapLight = new MapLight();
      byte opacity1 = map.GetOpacity(map.GetBlockIDNoCache(p));
      MapLight lightNoCache = map.GetLightNoCache(p);
      if ((int) lightNoCache.SunLight - (int) opacity1 > 0)
        mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity1);
      if ((int) lightNoCache.BlockLight - (int) opacity1 > 0)
        mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity1);
      if (p.X > map.MapBound.Min.X)
      {
        --p.X;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        ++p.X;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      if (p.X < map.MapBound.Max.X - 1)
      {
        ++p.X;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        --p.X;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      if (p.Y > map.MapBound.Min.Y)
      {
        --p.Y;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        ++p.Y;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      if (p.Y < map.MapBound.Max.Y)
      {
        ++p.Y;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        --p.Y;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      if (p.Z > map.MapBound.Min.Z)
      {
        --p.Z;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        ++p.Z;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      if (p.Z < map.MapBound.Max.Z - 1)
      {
        ++p.Z;
        byte opacity2 = map.GetOpacity(map.GetBlockIDNoCache(p));
        lightNoCache = map.GetLightNoCache(p);
        --p.Z;
        if ((int) lightNoCache.SunLight - (int) opacity2 > (int) mapLight.SunLight)
          mapLight.SunLight = (byte) ((uint) lightNoCache.SunLight - (uint) opacity2);
        if ((int) lightNoCache.BlockLight - (int) opacity2 > (int) mapLight.BlockLight)
          mapLight.BlockLight = (byte) ((uint) lightNoCache.BlockLight - (uint) opacity2);
      }
      return mapLight;
    }

    private void CommandIsNpcCount(ScriptInstance si, BinaryReader reader)
    {
      bool flag1 = false;
      bool flag2 = true;
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      string name = (string) null;
      float range = -1f;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      if (type1 != ScriptCoordType.None)
      {
        p1.X = this.ReadInt32(si, reader);
        p1.Y = this.ReadInt32(si, reader);
        p1.Z = this.ReadInt32(si, reader);
        if (this.AdjustCoord(si, ref p1, type1, false))
        {
          ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
          if (type2 != ScriptCoordType.None)
          {
            p2.X = this.ReadInt32(si, reader);
            p2.Y = this.ReadInt32(si, reader);
            p2.Z = this.ReadInt32(si, reader);
            if (!this.AdjustCoord(si, ref p2, type2, false))
              flag2 = false;
          }
          else
            range = this.ReadSingle(si, reader);
        }
        else
          flag2 = false;
      }
      else if (reader.ReadBoolean())
        name = reader.ReadString();
      NpcManager npcManager = si.Instance.NpcManager;
      if (flag2 && npcManager != null)
      {
        ActorType actorType = (ActorType) reader.ReadByte();
        Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
        int compareWith = this.ReadInt32(si, reader);
        int actual = 0;
        if ((double) range >= 0.0)
        {
          Vector3 blockCenter = si.MapTM.GetBlockCenter(p1);
          actual = npcManager.GetNpcCountNearPosition(actorType, blockCenter, range);
        }
        else if (type1 != ScriptCoordType.None)
        {
          Vector3 position1 = si.MapTM.GetPosition(p1);
          Vector3 position2 = si.MapTM.GetPosition(p2);
          this.SetMinMax(ref position1, ref position2);
          position1.Y -= si.MapTM.TileSize;
          position2.X += si.MapTM.TileSize;
          position2.Z += si.MapTM.TileSize;
          actual = npcManager.GetNpcCountNearPosition(actorType, position1, position2);
        }
        else if (name != null)
        {
          GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
          Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
          if (zone == null && gamerID.IsGamer)
            zone = si.Instance.MapStrategyTM.GetZone(name);
          if (zone != null)
          {
            Vector3 position1 = si.MapTM.GetPosition(zone.Min);
            Vector3 position2 = si.MapTM.GetPosition(zone.Max);
            position1.Y -= si.MapTM.TileSize;
            position2.X += si.MapTM.TileSize;
            position2.Z += si.MapTM.TileSize;
            actual = npcManager.GetNpcCountNearPosition(actorType, position1, position2);
          }
        }
        else
          actual = npcManager.GetNpcCount(actorType);
        flag1 = Parser.Compare(actual, compareWith, compare);
      }
      if (flag1)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsNameplate(ScriptInstance si, BinaryReader reader)
    {
      NamePlateSetting namePlateSetting = (NamePlateSetting) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      if (namePlateSetting != NamePlateSetting.None ? si.Player.Settings.Nameplates == namePlateSetting : (!flag ? si.Player.Settings.Nameplates == NamePlateSetting.None : si.Player.Settings.Nameplates != NamePlateSetting.None))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsNightTime(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      if (si.Instance.SunMoon.IsNightTime == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsRandom(ScriptInstance si, BinaryReader reader)
    {
      int num = this.ReadInt32(si, reader);
      int max = this.ReadInt32(si, reader);
      if (si.Random.Next(max) < num)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsSkills(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      if (si.Instance.IsSkillsEnabled == flag)
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsTime(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      int num = this.ReadInt32(si, reader);
      double actual = 0.0;
      if (flag)
      {
        if (si.Player != null)
          actual = si.Player.Statistics.SecondsPlayed;
      }
      else
      {
        foreach (SavePlayerState playerSave in si.Instance.PlayerSaves)
          actual += (double) (int) playerSave.Statistics.SecondsPlayed;
      }
      if (Parser.Compare(actual, (double) num, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandIsVar(ScriptInstance si, BinaryReader reader)
    {
      ushort varIndex = reader.ReadUInt16();
      Parser.CompareState compare = (Parser.CompareState) reader.ReadByte();
      double compareWith = this.ReadDouble(si, reader);
      if (Parser.Compare(si.GetVarValue(varIndex), compareWith, compare))
        return;
      this.JumpToFalseConditional(si, reader);
    }

    private void CommandItem(ScriptInstance si, BinaryReader reader)
    {
      Item itemID = (Item) reader.ReadUInt16();
      bool? nullable1 = new bool?();
      string str1 = (string) null;
      string str2 = (string) null;
      ushort? nullable2 = new ushort?();
      short? nullable3 = new short?();
      SkillType? nullable4 = new SkillType?();
      int? nullable5 = new int?();
      float? nullable6 = new float?();
      float? nullable7 = new float?();
      float? nullable8 = new float?();
      float? nullable9 = new float?();
      short? nullable10 = new short?();
      short? nullable11 = new short?();
      short? nullable12 = new short?();
      short? nullable13 = new short?();
      short? nullable14 = new short?();
      short? nullable15 = new short?();
      int index1 = (int) itemID;
      if (index1 < 0 || index1 >= Globals1.ItemData.Length)
        return;
      Globals2.NeedToReinitialize = true;
      ItemDataXML itemDataXml = Globals1.ItemData[index1];
      ItemTypeDataXML itemTypeDataXml = Globals1.ItemTypeData[index1];
      int index2 = 0;
      if (reader.ReadBoolean())
        nullable1 = new bool?(reader.ReadBoolean());
      if (reader.ReadBoolean())
        str1 = reader.ReadString();
      if (reader.ReadBoolean())
        str2 = reader.ReadString();
      bool flag = si.Instance.IsCreativeMode && Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      if (flag)
      {
        if (reader.ReadBoolean())
          nullable2 = new ushort?((ushort) this.ReadInt32(si, reader));
        if (reader.ReadBoolean())
          nullable4 = new SkillType?((SkillType) reader.ReadByte());
        if (reader.ReadBoolean())
          nullable5 = new int?(this.ReadInt32(si, reader));
        if (reader.ReadBoolean())
          nullable6 = new float?(this.ReadSingle(si, reader));
        if (reader.ReadBoolean())
          nullable7 = new float?(this.ReadSingle(si, reader));
        if (reader.ReadBoolean())
          nullable8 = new float?(this.ReadSingle(si, reader));
        if (reader.ReadBoolean())
          nullable9 = new float?(this.ReadSingle(si, reader));
        if (reader.ReadBoolean())
          nullable3 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -32000, 32000));
        index2 = (int) itemTypeDataXml.Combat;
        if (index2 > 0)
        {
          if (reader.ReadBoolean())
            nullable10 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
          if (reader.ReadBoolean())
            nullable11 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
          if (reader.ReadBoolean())
            nullable12 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
          if (reader.ReadBoolean())
            nullable13 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
          if (reader.ReadBoolean())
            nullable14 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
          if (reader.ReadBoolean())
            nullable15 = new short?((short) MyMathHelper.Clamp(this.ReadInt32(si, reader), -9999, 9999));
        }
      }
      if (nullable1.HasValue)
        itemDataXml.IsEnabled = nullable1.Value;
      if (str1 != null)
      {
        itemDataXml.Name = str1;
        Blueprints.GetBlueprint(itemID)?.BuildDescription();
      }
      if (str2 != null)
        itemDataXml.Desc = str2;
      if (flag)
      {
        if (nullable2.HasValue)
          itemDataXml.Durability = (ushort) MyMathHelper.Clamp((int) nullable2.Value, 0, (int) ushort.MaxValue);
        if (nullable4.HasValue)
          Globals1.SkillData[index1].UseSkill = nullable4.Value;
        if (nullable5.HasValue)
          Globals1.SkillData[index1].UseReq = MyMathHelper.Clamp(nullable5.Value, 0, 180);
        if (nullable6.HasValue)
          itemDataXml.StrikeDamage = MathHelper.Clamp(nullable6.Value, 0.0f, 10000f);
        if (nullable7.HasValue)
          itemDataXml.StrikeReach = MathHelper.Clamp(nullable7.Value, 0.0f, 20f);
        if (nullable3.HasValue)
          itemDataXml.HealPower = nullable3.Value;
        if (nullable10.HasValue)
          Globals1.ItemCombatData[index2].Health = nullable10.Value;
        if (nullable11.HasValue)
          Globals1.ItemCombatData[index2].Strength = nullable11.Value;
        if (nullable12.HasValue)
          Globals1.ItemCombatData[index2].Attack = nullable12.Value;
        if (nullable13.HasValue)
          Globals1.ItemCombatData[index2].Defence = nullable13.Value;
        if (nullable14.HasValue)
          Globals1.ItemCombatData[index2].Ranged = nullable14.Value;
        if (nullable15.HasValue)
          Globals1.ItemCombatData[index2].Looting = nullable15.Value;
        if (nullable8.HasValue || nullable9.HasValue)
        {
          ItemSwingTimeDataXML swingTimeDataXml = Globals1.ItemSwingTimeData[index1];
          if (nullable9.HasValue)
            swingTimeDataXml.Pause = MathHelper.Clamp(nullable9.Value, 0.0f, 10f);
          if (nullable8.HasValue)
          {
            swingTimeDataXml.Time = nullable8.Value;
            swingTimeDataXml.ExtendedPause = 0.0f;
            swingTimeDataXml.RetractTime = -1f;
          }
          swingTimeDataXml.Time = MathHelper.Clamp(nullable8.Value, 0.125f + swingTimeDataXml.Pause, 10f);
          Globals1.ItemSwingTimeData[index1] = swingTimeDataXml;
        }
      }
      if (!si.Instance.IsMultiplayer || !si.Instance.IsHost)
        return;
      string str3 = "item [" + itemID.ToString() + "]";
      if (nullable1.HasValue)
        str3 = str3 + " [" + (nullable1.Value ? "enable" : "disable") + "]";
      if (str1 != null)
        str3 = str3 + " [name=" + str1 + "]";
      if (str2 != null)
        str3 = str3 + " [desc=" + str2 + "]";
      if (flag)
      {
        if (nullable2.HasValue)
          str3 = str3 + " [durability=" + (object) nullable2.Value + "]";
        if (nullable10.HasValue)
          str3 = str3 + " [health=" + (object) nullable10.Value + "]";
        if (nullable11.HasValue)
          str3 = str3 + " [strength=" + (object) nullable11.Value + "]";
        if (nullable12.HasValue)
          str3 = str3 + " [attack=" + (object) nullable12.Value + "]";
        if (nullable13.HasValue)
          str3 = str3 + " [defence=" + (object) nullable13.Value + "]";
        if (nullable14.HasValue)
          str3 = str3 + " [ranged=" + (object) nullable14.Value + "]";
        if (nullable15.HasValue)
          str3 = str3 + " [looting=" + (object) nullable15.Value + "]";
        if (nullable6.HasValue)
          str3 = str3 + " [damage=" + (object) nullable6.Value + "]";
        if (nullable7.HasValue)
          str3 = str3 + " [reach=" + (object) nullable7.Value + "]";
        if (nullable8.HasValue)
          str3 = str3 + " [speed=" + (object) nullable8.Value + "]";
        if (nullable9.HasValue)
          str3 = str3 + " [delay=" + (object) nullable9.Value + "]";
        if (nullable4.HasValue)
          str3 = str3 + " [skill=" + (object) nullable4.Value + "]";
        if (nullable5.HasValue)
          str3 = str3 + " [level=" + (object) nullable5.Value + "]";
      }
      si.Instance.ScriptCatchupCommands.Add(str3);
    }

    private void CommandKick(ScriptInstance si, BinaryReader reader)
    {
      if (si.Player == null || si.Player.IsHost)
        return;
      si.Instance.NetworkManager.KickGamer(si.Player.Gamer, false);
    }

    private void CommandLoop(ScriptInstance si, BinaryReader reader)
    {
      int num = this.ReadInt32(si, reader);
      if (num < 1)
        num = 1;
      si.WaitTime = this.clock.ElapsedMilliseconds + (long) num;
      si.PC = si.BeginPC;
      si.UpdatePC = false;
    }

    private void CommandMarker(ScriptInstance si, BinaryReader reader)
    {
      string str = reader.ReadString();
      if (reader.ReadBoolean())
      {
        si.Instance.RemoveMapMarker(str, false);
      }
      else
      {
        ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
        GlobalPoint3D p = new GlobalPoint3D();
        p.X = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        bool flag = reader.ReadBoolean();
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        si.Instance.AddMapMarker(p, str, flag ? MapMarkerType.AdminX : MapMarkerType.X, false);
      }
    }

    private void CommandMenu(ScriptInstance si, BinaryReader reader)
    {
      if (si.Player == null || !si.Player.IsLocalGamer || si.Instance.IsScriptedScreenOpen(si.Player))
        return;
      ScriptMenuOptions scriptMenuOptions = (ScriptMenuOptions) reader.ReadByte();
      int num = (int) reader.ReadByte();
      if (num <= 0)
        return;
      ScriptedMenuScreen screen = new ScriptedMenuScreen(si.Player, si.ScriptOffset, si.BlockOffset);
      for (int index = 0; index < num; ++index)
      {
        ScriptMenuParam scriptMenuParam = this.ReadMenuParam(si, reader);
        screen.AddMenuEntry((MenuEntry) new ScriptedMenuEntry(screen, scriptMenuParam));
      }
      if ((scriptMenuOptions & ScriptMenuOptions.NoCancel) == ScriptMenuOptions.None)
        screen.AddCancelMenuEntry();
      si.Instance.AddScreen((GameScreen) screen, si.Player);
    }

    private ScriptMenuParam ReadMenuParam(ScriptInstance si, BinaryReader reader)
    {
      ScriptMenuParam scriptMenuParam = new ScriptMenuParam();
      scriptMenuParam.Text = Globals2.SubstituteText(si, reader.ReadString());
      if (reader.ReadBoolean())
        scriptMenuParam.Script = reader.ReadString();
      scriptMenuParam.Coord = (ScriptCoordType) reader.ReadByte();
      if (scriptMenuParam.Coord != ScriptCoordType.None)
      {
        GlobalPoint3D p = new GlobalPoint3D();
        p.X = this.ReadInt32(si, reader);
        p.Y = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        this.AdjustCoord(si, ref p, scriptMenuParam.Coord);
        scriptMenuParam.Point = new ScriptPoint3D?((ScriptPoint3D) p);
      }
      return scriptMenuParam;
    }

    private void CommandMessageBox(ScriptInstance si, BinaryReader reader)
    {
      if (si.Player == null || !si.Player.IsLocalGamer || si.Instance.IsScriptedScreenOpen(si.Player))
        return;
      ScriptMenuOptions scriptMenuOptions = (ScriptMenuOptions) reader.ReadByte();
      string message = reader.ReadBoolean() ? Globals2.SubstituteText(si, reader.ReadString()) : (string) null;
      ScriptMenuParam scriptMenuParam1 = reader.ReadBoolean() ? this.ReadMenuParam(si, reader) : new ScriptMenuParam();
      ScriptMenuParam scriptMenuParam2 = reader.ReadBoolean() ? this.ReadMenuParam(si, reader) : new ScriptMenuParam();
      ScriptMenuParam scriptMenuParam3 = reader.ReadBoolean() ? this.ReadMenuParam(si, reader) : new ScriptMenuParam();
      string bText = (scriptMenuOptions & ScriptMenuOptions.NoCancel) == ScriptMenuOptions.None ? "Exit" : (string) null;
      GlobalPoint3D? aPoint = new GlobalPoint3D?();
      GlobalPoint3D? xPoint = new GlobalPoint3D?();
      GlobalPoint3D? yPoint = new GlobalPoint3D?();
      if (scriptMenuParam1.Point.HasValue)
        aPoint = new GlobalPoint3D?((GlobalPoint3D) scriptMenuParam1.Point.Value);
      if (scriptMenuParam2.Point.HasValue)
        xPoint = new GlobalPoint3D?((GlobalPoint3D) scriptMenuParam2.Point.Value);
      if (scriptMenuParam3.Point.HasValue)
        yPoint = new GlobalPoint3D?((GlobalPoint3D) scriptMenuParam3.Point.Value);
      si.Instance.OpenMessageBox(si.Player, message, scriptMenuParam1.Text, scriptMenuParam1.Script, aPoint, scriptMenuParam2.Text, scriptMenuParam2.Script, xPoint, scriptMenuParam3.Text, scriptMenuParam3.Script, yPoint, bText);
    }

    private void CommandNpcState(ScriptInstance si, BinaryReader reader)
    {
      ActorState actorState = (ActorState) reader.ReadByte();
      if (actorState == ActorState.Alive)
      {
        NpcBase actor = si.Actor as NpcBase;
        if (actor == null || actor.IsDeadOrInactiveOrDisabled)
          return;
        ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
        if (AdjustCoord(si, new Vector3(ReadSingle(si, reader), ReadSingle(si, reader), ReadSingle(si, reader)), type));
      }
      else if (reader.ReadBoolean())
      {
        if (si.Target == null || si.Target.IsPlayer)
          return;
        si.Target.ChangeState(actorState);
      }
      else
      {
        ActorType npcType = (ActorType) reader.ReadByte();
        GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
        GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
        GlobalPoint3D p1;
        GlobalPoint3D p2;
        if (reader.ReadBoolean())
        {
          string name = reader.ReadString();
          GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
          Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
          if (zone == null && gamerID.IsGamer)
            zone = si.Instance.MapStrategyTM.GetZone(name);
          if (zone == null)
            return;
          p1 = zone.Min;
          p2 = zone.Max;
        }
        else
        {
          ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
          p1 = new GlobalPoint3D();
          p1.X = this.ReadInt32(si, reader);
          p1.Y = this.ReadInt32(si, reader);
          p1.Z = this.ReadInt32(si, reader);
          ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
          p2 = new GlobalPoint3D();
          p2.X = this.ReadInt32(si, reader);
          p2.Y = this.ReadInt32(si, reader);
          p2.Z = this.ReadInt32(si, reader);
          if (!this.AdjustCoord(si, ref p1, type1) || !this.AdjustCoord(si, ref p2, type2))
            return;
        }
        if (!si.Instance.IsHost)
          return;
        this.SetMinMax(ref p1, ref p2);
        si.Instance.NpcManager.SetNpcState(p1, p2, actorState, npcType);
      }
    }

    private void CommandNpcHealth(ScriptInstance si, BinaryReader reader)
    {
      int qty = this.ReadInt32(si, reader);
      int millisecs = this.ReadInt32(si, reader);
      int duration = this.ReadInt32(si, reader);
      bool flag = reader.ReadBoolean();
      if (millisecs < 0)
        millisecs = 0;
      else if (millisecs > 0 && millisecs < 16)
        millisecs = 16;
      if (flag)
      {
        if (si.Target == null)
          return;
        si.Instance.NpcManager.EffectAddHealth(si.Target, si.Actor, qty, millisecs, duration);
      }
      else
      {
        ActorType mobType = (ActorType) reader.ReadByte();
        GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
        GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
        GlobalPoint3D p1;
        GlobalPoint3D p2;
        if (reader.ReadBoolean())
        {
          string name = reader.ReadString();
          GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
          Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
          if (zone == null && gamerID.IsGamer)
            zone = si.Instance.MapStrategyTM.GetZone(name);
          if (zone == null)
            return;
          p1 = zone.Min;
          p2 = zone.Max;
        }
        else
        {
          ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
          p1 = new GlobalPoint3D();
          p1.X = this.ReadInt32(si, reader);
          p1.Y = this.ReadInt32(si, reader);
          p1.Z = this.ReadInt32(si, reader);
          ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
          p2 = new GlobalPoint3D();
          p2.X = this.ReadInt32(si, reader);
          p2.Y = this.ReadInt32(si, reader);
          p2.Z = this.ReadInt32(si, reader);
          if (!this.AdjustCoord(si, ref p1, type1) || !this.AdjustCoord(si, ref p2, type2))
            return;
        }
        this.SetMinMax(ref p1, ref p2);
        si.Instance.NpcManager.EffectAddHealth(mobType, si.Actor, p1, p2, qty, millisecs, duration);
      }
    }

    private void CommandNpcSpawn(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsHost)
        return;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      ActorType actorType = (ActorType) reader.ReadByte();
      if (reader.ReadBoolean())
        reader.ReadString();
      string name = reader.ReadBoolean() ? reader.ReadString() : (string) null;
      string scriptName = reader.ReadBoolean() ? reader.ReadString() : (string) null;
      bool flag1 = reader.ReadBoolean();
      if (flag1)
        this.combatStats.ReadState(reader, int.MaxValue);
      LootTable lootTable = (LootTable) null;
      bool flag2 = reader.ReadBoolean();
      if (flag2)
      {
        if (si.Script.LootTables == null)
          si.Script.LootTables = new Dictionary<int, LootTable>();
        if (!si.Script.LootTables.TryGetValue(si.CmdIndex, out lootTable))
        {
          lootTable = new LootTable();
          si.Script.LootTables.Add(si.CmdIndex, lootTable);
        }
        lootTable.ReadState(reader, int.MaxValue);
      }
      else if (si.Script.LootTables != null && si.Script.LootTables.ContainsKey(si.CmdIndex))
        si.Script.LootTables.Remove(si.CmdIndex);
      if (!this.AdjustCoord(si, ref p, type) || flag2 && !si.Instance.IsCreativeMode && (lootTable.Point.HasValue ? si.Instance.MapStrategyTM.GetDataBlock(lootTable.Point.Value) as ChestBlock : (ChestBlock) null) == null)
        return;
      NpcBase npcBase = !flag1 ? si.Instance.NpcManager.SpawnNpc(actorType, si.MapTM.GetPosition(p), (string) null, si.Instance.GetScript(scriptName), lootTable, new CombatStats?()) : si.Instance.NpcManager.SpawnNpc(actorType, si.MapTM.GetPosition(p), (string) null, si.Instance.GetScript(scriptName), lootTable, new CombatStats?(this.combatStats));
      if (npcBase == null || name == null)
        return;
      npcBase.LoadBehaviour(BehaviourTreeType.Dialog, name);
    }

    private void CommandMoveBlock(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p1 = new GlobalPoint3D();
      p1.X = this.ReadInt32(si, reader);
      p1.Y = this.ReadInt32(si, reader);
      p1.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p2 = new GlobalPoint3D();
      p2.X = this.ReadInt32(si, reader);
      p2.Y = this.ReadInt32(si, reader);
      p2.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p1, type1) || !this.AdjustCoord(si, ref p2, type2))
        return;
      MapBlock blockIdAndAux = si.MapTM.GetBlockIDAndAux(p1);
      if (!Globals1.ItemData[(int) blockIdAndAux.BlockID].IsEnabled || si.Instance.IsInZoneType(p1, ZoneType.Spawn, Globals2.GetGamerType(GamerType.Script)) || si.Instance.IsInZoneType(p2, ZoneType.Spawn, Globals2.GetGamerType(GamerType.Script)))
        return;
      DataBlock dataBlock = si.MapTM.MapStrategyTM.GetDataBlock(p1);
      if (dataBlock != null && si.Instance.IsFiniteResources && dataBlock.HasInventory)
        return;
      BlockData.AdjustBlockDataForMove(ref blockIdAndAux);
      if (!si.Instance.ClearBlock(p1, si.Method, Globals2.GetGamerType(GamerType.ScriptMove), false))
        return;
      si.Instance.AddBlockNoPermissionCheck(p2, (Block) blockIdAndAux.BlockID, blockIdAndAux.AuxData, si.Method, Globals2.GetGamerType(GamerType.ScriptMove), false, false, false, (object) null);
      if (dataBlock != null)
        this.MoveDataBlock(si, p2, dataBlock, si.Method);
      si.NeedsCommit = true;
    }

    private void MoveDataBlock(
      ScriptInstance si,
      GlobalPoint3D p,
      DataBlock dataBlock,
      UpdateBlockMethod method)
    {
      dataBlock.Point = p;
      si.MapTM.MapStrategyTM.AddDataBlock(dataBlock, si.Method, false);
      if (dataBlock.ClassType != DataBlockType.Sign)
        return;
      si.Instance.MapRenderer.SignsChanged(false);
    }

    private void CommandMoveRegion(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D min = new GlobalPoint3D();
      min.X = this.ReadInt32(si, reader);
      min.Y = this.ReadInt32(si, reader);
      min.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D max = new GlobalPoint3D();
      max.X = this.ReadInt32(si, reader);
      max.Y = this.ReadInt32(si, reader);
      max.Z = this.ReadInt32(si, reader);
      ScriptCoordType type3 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref min, type1) || !this.AdjustCoord(si, ref max, type2) || !this.AdjustCoord(si, ref p, type3))
        return;
      this.SetMinMax(ref min, ref max);
      si.Instance.CreativeModeHelper.RunMove(Globals2.GetGamerType(GamerType.ScriptMove), min, max, p, "Move", false);
      si.NeedsCommit = true;
    }

    private void CommandNop(ScriptInstance si, BinaryReader reader)
    {
    }

    private void CommandNotify(ScriptInstance si, BinaryReader reader)
    {
      NotifyRecipient origRecType = (NotifyRecipient) reader.ReadByte();
      string message = Globals2.SubstituteText(si, reader.ReadString());
      Color? color = this.ReadColorNull(si, reader);
      NotifyRecipient recType = NotifyRecipient.None;
      if ((origRecType & NotifyRecipient.Local) > NotifyRecipient.None && (si.Player == null || si.Player.IsLocalGamer))
        recType |= NotifyRecipient.Local;
      if ((origRecType & NotifyRecipient.Remote) > NotifyRecipient.None && si.Player != null && !si.Player.IsLocalGamer)
        recType |= NotifyRecipient.Local;
      if ((origRecType & NotifyRecipient.Admin) > NotifyRecipient.None && si.Instance.NetworkManager.HasLocalAdminPlayer())
        recType |= NotifyRecipient.Local;
      if ((origRecType & NotifyRecipient.Clan) > NotifyRecipient.None && si.Player != null && si.Instance.NetworkManager.HasLocalPlayerOfClan(si.Player.ClanName))
        recType |= NotifyRecipient.Local;
      si.Instance.AddNotification((Player) null, message, color, recType, origRecType, si.Player != null ? si.Player.ClanName : (string) null);
    }

    private void CommandOpenBlock(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      Block blockId = (Block) si.MapTM.GetBlockID(p);
      Block block = blockId;
      if ((uint) block <= 121U)
      {
        switch (block)
        {
          case Block.Bookcase:
          case Block.Workbench:
          case Block.Furnace:
          case Block.Chest:
          case Block.ItemShop:
          case Block.BlockShop:
            break;
          case Block.Torch:
            return;
          case Block.Ladder:
            return;
          case Block.Book:
            if (si.Instance.CurrentOpenBlock != null)
              return;
            si.Instance.CurrentOpenBlock = si.Instance.ReadBook(si.Player, p);
            if (si.Instance.CurrentOpenBlock == null)
              return;
            si.Instance.CurrentOpenBlock.ScreenRemoved += new EventHandler<EventArgs>(si.Instance.OnOpenBlockScreenRemoved);
            return;
          default:
            return;
        }
      }
      else
      {
        switch (block)
        {
          case Block.LockedChest:
          case Block.LitFurnace:
          case Block.Crate:
          case Block.Safe:
            break;
          default:
            return;
        }
      }
      if (si.Instance.CurrentOpenBlock != null)
        return;
      si.Instance.CurrentOpenBlock = si.Instance.OpenSpecialBlock(si.Player, p, blockId, (Hand) null, false);
      if (si.Instance.CurrentOpenBlock == null)
        return;
      si.Instance.CurrentOpenBlock.ScreenRemoved += new EventHandler<EventArgs>(si.Instance.OnOpenBlockScreenRemoved);
    }

    private void CommandParticle(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      Vector3 pos = new Vector3();
      pos.X = this.ReadSingle(si, reader);
      pos.Y = this.ReadSingle(si, reader);
      pos.Z = this.ReadSingle(si, reader);
      if (!this.AdjustCoord(si, ref pos, type))
        return;
      ParticleData data = new ParticleData();
      if (reader.ReadBoolean())
        Globals2.SetParticleDataFromTemplate((int) reader.ReadUInt16(), ref data);
      else
        this.ReadParticleState(si, reader, ref data);
      this.AdjustVelocity(si, ref data);
      si.Instance.EmitterParticleSystem.AddParticle(pos, ref data);
    }

    private void AdjustVelocity(ScriptInstance si, ref ParticleData data)
    {
      Actor actor;
      ScriptCoordType scriptCoordType;
      switch (data.VelocityType)
      {
        case ScriptCoordType.ViewRelative:
        case ScriptCoordType.ViewHorizRelative:
          actor = si.Actor;
          scriptCoordType = data.VelocityType;
          break;
        case ScriptCoordType.TargetRelative:
          return;
        case ScriptCoordType.TargetViewRelative:
          actor = si.Target;
          scriptCoordType = ScriptCoordType.ViewRelative;
          break;
        case ScriptCoordType.KillerRelative:
          return;
        case ScriptCoordType.KillerViewRelative:
          actor = si.Killer;
          scriptCoordType = ScriptCoordType.ViewRelative;
          break;
        default:
          return;
      }
      if (actor == null)
        return;
      switch (scriptCoordType)
      {
        case ScriptCoordType.ViewRelative:
          Vector3 velocity1 = data.Velocity;
          data.Velocity = velocity1.X * Vector3.Normalize(actor.ViewDirection);
          data.Velocity.Y += velocity1.Y;
          if ((double) velocity1.Z == 0.0)
            break;
          Vector3 right = actor.ViewMatrix.Right;
          right.Y = 0.0f;
          right.Normalize();
          right.X *= velocity1.Z;
          right.Z *= velocity1.Z;
          right.Z = -right.Z;
          data.Velocity.X += right.X;
          data.Velocity.Z += right.Z;
          break;
        case ScriptCoordType.ViewHorizRelative:
          Vector3 velocity2 = data.Velocity;
          data.Velocity = velocity2.X * actor.ViewDirNoYNormalized;
          data.Velocity.Y += velocity2.Y;
          if ((double) velocity2.Z == 0.0)
            break;
          Vector3 vector3 = actor.ViewMatrix.Right * velocity2.Z;
          vector3.Z = -vector3.Z;
          data.Velocity.X += vector3.X;
          data.Velocity.Z += vector3.Z;
          break;
      }
    }

    private void CommandParticleEmitter(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      Vector3 pos = new Vector3();
      pos.X = this.ReadSingle(si, reader);
      pos.Y = this.ReadSingle(si, reader);
      pos.Z = this.ReadSingle(si, reader);
      ParticleData data = new ParticleData();
      float num1;
      if (reader.ReadBoolean())
      {
        ushort num2 = reader.ReadUInt16();
        num1 = this.ReadSingle(si, reader);
        float num3 = this.ReadSingle(si, reader);
        Globals2.SetParticleDataFromTemplate((int) num2, ref data);
        if ((double) num3 > 0.0)
          data.EmitFreq = (int) ((double) num3 * 1000.0);
      }
      else
      {
        num1 = this.ReadSingle(si, reader);
        this.ReadParticleState(si, reader, ref data);
      }
      if (!this.AdjustCoord(si, ref pos, type))
        return;
      this.AdjustVelocity(si, ref data);
      si.Instance.ParticleEmitterWorker.AddEmitter(pos, (int) ((double) num1 * 1000.0), ref data);
    }

    private void ReadParticleState(ScriptInstance si, BinaryReader reader, ref ParticleData data)
    {
      data.EmitFreq = (int) ((double) this.ReadSingle(si, reader) * 1000.0);
      data.Duration = (ushort) Math.Min(8000f, this.ReadSingle(si, reader) * 1000f);
      data.Rotation = this.ReadSingle(si, reader);
      data.VelocityType = (ScriptCoordType) reader.ReadByte();
      data.Velocity.X = this.ReadSingle(si, reader);
      data.Velocity.Y = this.ReadSingle(si, reader);
      data.Velocity.Z = this.ReadSingle(si, reader);
      data.VelocityVariance.X = this.ReadSingle(si, reader);
      data.VelocityVariance.Y = this.ReadSingle(si, reader);
      data.VelocityVariance.Z = this.ReadSingle(si, reader);
      data.EmitPosOffset.X = this.ReadSingle(si, reader);
      data.EmitPosOffset.Y = this.ReadSingle(si, reader);
      data.EmitPosOffset.Z = this.ReadSingle(si, reader);
      data.EmitPosVariance.X = this.ReadSingle(si, reader);
      data.EmitPosVariance.Y = this.ReadSingle(si, reader);
      data.EmitPosVariance.Z = this.ReadSingle(si, reader);
      data.Size.X = this.ReadSingle(si, reader);
      data.Size.Y = this.ReadSingle(si, reader);
      data.Size.Z = this.ReadSingle(si, reader);
      data.Size.W = this.ReadSingle(si, reader);
      data.WindFactor = this.ReadSingle(si, reader);
      data.Gravity = (short) ((double) this.ReadSingle(si, reader) * 100.0);
      data.StartColor = this.ReadColor(si, reader);
      data.EndColor = this.ReadColor(si, reader);
    }

    private void CommandPaste(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      string str1 = reader.ReadString();
      string str2 = reader.ReadString();
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      BlockFace facing = (BlockFace) reader.ReadByte();
      Map.CopyType copyType = (Map.CopyType) reader.ReadByte();
      if (!this.AdjustCoord(si, ref p, type))
        return;
      MapModel model;
      if (str1 == "system")
      {
        model = si.Instance.SystemVoxelModelManager.LoadComponent(str1, str2, false, false);
        if (model != null)
          model.IsSystemModel = true;
      }
      else
        model = si.Instance.VoxelModelManager.LoadComponent(str1, str2, false, true);
      if (model != null)
      {
        si.Instance.CreativeModeHelper.Paste(Globals2.GetGamerType(GamerType.Script), model, p, facing, copyType, false, false);
        si.NeedsCommit = true;
      }
      else
      {
        MetaExecuteScript metaExecuteScript = new MetaExecuteScript()
        {
          ScriptName = si.Script.Name,
          LineNo = si.CmdIndex
        };
        si.Instance.NetworkManager.SendComponentAsTempRequest(str1, str2, (MetaExecuteBase) metaExecuteScript);
      }
    }

    private void CommandPermission(ScriptInstance si, BinaryReader reader)
    {
      Permissions permission = (Permissions) reader.ReadUInt16();
      bool enable = reader.ReadBoolean();
      if (si.Player == null)
        return;
      si.Player.TogglePermission(permission, enable);
    }

    private void CommandPickup(ScriptInstance si, BinaryReader reader)
    {
      if (!si.MapTM.IsHost || !si.Instance.IsCreativeMode)
        return;
      if (reader.ReadBoolean())
      {
        si.Instance.ClearAllParticles(false);
      }
      else
      {
        ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
        GlobalPoint3D p = new GlobalPoint3D();
        p.X = this.ReadInt32(si, reader);
        p.Y = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        Item itemID = (Item) reader.ReadUInt16();
        int count = this.ReadInt32(si, reader);
        if (count < 1 || !this.AdjustCoord(si, ref p, type))
          return;
        Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p);
        if (!this.IsBlockSupportParticles(si, p, blockIdNoCache))
          return;
        si.Instance.AddPickup(ParticleType.None, p, new InventoryItem(itemID, count), Vector2.Zero, 30f, 0.0f, Globals2.GetGamerType(GamerType.Script), false);
      }
    }

    private bool IsBlockSupportParticles(ScriptInstance si, GlobalPoint3D p, Block blockID)
    {
      if (si.MapTM.BlockData[(int) blockID].IsIcon)
        return true;
      Block block = blockID;
      if ((uint) block <= 131U)
      {
        if ((uint) block <= 13U)
        {
          switch (block)
          {
            case Block.None:
            case Block.Cloud:
            case Block.Water:
            case Block.Lava:
              break;
            default:
              goto label_12;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Torch:
            case Block.Ladder:
            case Block.WoodDoorTop:
            case Block.SteelDoorTop:
            case Block.Teleport:
            case Block.Rope:
            case Block.Pane:
            case Block.StainedGlassPane:
            case Block.WoodDoorBottom:
            case Block.SteelDoorBottom:
            case Block.Sign:
            case Block.Fire:
            case Block.SteelSpikes:
            case Block.ClimbingIvy:
            case Block.Crop:
              break;
            case Block.Stack:
              goto label_10;
            case Block.UpsideDownStack:
              return si.MapTM.GetAuxDataNoCache(p) < (byte) 4;
            default:
              goto label_12;
          }
        }
      }
      else if ((uint) block <= 160U)
      {
        switch (block)
        {
          case Block.Fence:
          case Block.LockedDoorTop:
          case Block.Painting:
            break;
          case Block.SnowLayer:
            goto label_10;
          default:
            goto label_12;
        }
      }
      else
      {
        switch (block)
        {
          case Block.PressurePlate:
          case Block.Switch:
          case Block.Button:
          case Block.TrapDoor:
          case Block.LockedDoorBottom:
            break;
          case Block.Stack2:
            goto label_10;
          default:
            goto label_12;
        }
      }
      return true;
label_10:
      return si.MapTM.GetAuxDataNoCache(p) < (byte) 2;
label_12:
      return false;
    }

    private void CommandRain(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (reader.ReadBoolean())
      {
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        si.Instance.MapStrategyTM.EnvManager.RemoveRain(p, false);
      }
      else
      {
        float radius = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 2000f);
        float duration = this.ReadSingle(si, reader);
        float num = MathHelper.Clamp(this.ReadSingle(si, reader), 1f, 100f);
        Color? nullable = this.ReadColorNull(si, reader);
        if (!this.AdjustCoord2D(si, ref p, type))
          return;
        if (nullable.HasValue)
          si.Instance.MapStrategyTM.EnvManager.AddRain(p, radius, duration, 5f, num / 100f, nullable.Value, false);
        else
          si.Instance.MapStrategyTM.EnvManager.AddRain(p, radius, duration, num / 100f, false);
      }
    }

    private void CommandRandom(ScriptInstance si, BinaryReader reader)
    {
      if (reader.ReadBoolean())
        si.Random.Seed(this.ReadInt32(si, reader));
      else
        si.Random.Seed((ulong) (uint) si.Script.Name.GetHashCode(), (ulong) (uint) (si.Script.ByteCodeSize * 987));
    }

    private void CommandReplaceRegion(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D min = new GlobalPoint3D();
      min.X = this.ReadInt32(si, reader);
      min.Y = this.ReadInt32(si, reader);
      min.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D max = new GlobalPoint3D();
      max.X = this.ReadInt32(si, reader);
      max.Y = this.ReadInt32(si, reader);
      max.Z = this.ReadInt32(si, reader);
      Block blockID1 = (Block) reader.ReadByte();
      Block blockID2 = (Block) reader.ReadByte();
      int num = this.ReadInt32(si, reader);
      int seed = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref min, type1) || !this.AdjustCoord(si, ref max, type2))
        return;
      if (num < 1)
        num = 1;
      else if (num > 100)
        num = 100;
      if (num < 100 && seed == 0)
        seed = si.Random.Next();
      this.SetMinMax(ref min, ref max);
      si.Instance.CreativeModeHelper.RunReplace(si.MapTM, Globals2.GetGamerType(GamerType.Script), min, max, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, blockID1, blockID2, (byte) num, seed, false, "Replace", (Action<CreativeOperationData>) null, false);
      si.NeedsCommit = true;
    }

    private void CommandScript(ScriptInstance si, BinaryReader reader)
    {
      string scriptName = reader.ReadString();
      if (reader.ReadBoolean())
      {
        bool flag = reader.ReadBoolean();
        si.Instance.CancelScript(scriptName, flag ? (Actor) null : (Actor) si.Player, false);
      }
      else
      {
        bool flag = reader.ReadBoolean();
        ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
        GlobalPoint3D p = new GlobalPoint3D();
        if (type != ScriptCoordType.None)
        {
          p.X = this.ReadInt32(si, reader);
          p.Y = this.ReadInt32(si, reader);
          p.Z = this.ReadInt32(si, reader);
          this.AdjustCoord(si, ref p, type);
        }
        ScriptExecuteData data = new ScriptExecuteData()
        {
          Parent = si,
          Actor = si.Actor,
          Target = si.Target,
          Killer = si.Killer,
          Context = si.Context,
          Seed = si.Seed,
          Delay = 1,
          ScriptOffset = type == ScriptCoordType.Absolute ? new GlobalPoint3D?(p) : si.ScriptOffset,
          BlockOffset = type == ScriptCoordType.Absolute || type == ScriptCoordType.None ? si.BlockOffset : new GlobalPoint3D?(p),
          Random = si.Random
        };
        ushort num = reader.ReadUInt16();
        if (num > (ushort) 0)
        {
          data.PassedVars = new ushort[(int) num];
          for (int index = 0; index < (int) num; ++index)
            data.PassedVars[index] = reader.ReadUInt16();
        }
        if (flag)
          si.WaitTime = long.MaxValue;
        si.Instance.ExecuteScript(scriptName, data, false);
      }
    }

    private void CommandSetBlock(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      Block blockID = (Block) reader.ReadByte();
      byte auxData = blockID != Block.None ? (byte) this.ValidateSetBlockAuxData(si, blockID, this.ReadInt32(si, reader)) : (byte) 0;
      if (!this.AdjustCoord(si, ref p, type) || si.Instance.IsInAnyPlayerSpace(p) || si.Instance.IsInZoneType(p, ZoneType.Spawn, Globals2.GetGamerType(GamerType.Script)))
        return;
      if (blockID == Block.None)
      {
        si.NeedsCommit = si.Instance.ClearBlock(p, si.Method, Globals2.GetGamerType(GamerType.Script), false);
      }
      else
      {
        if (!(si.NeedsCommit = si.Instance.AddBlockNoPermissionCheck(p, blockID, auxData, si.Method, Globals2.GetGamerType(GamerType.Script), false, false, false, (object) null)))
          return;
        switch (blockID)
        {
          case Block.LockedChest:
          case Block.LockedDoorBottom:
            if (si.Player == null)
              break;
            si.MapTM.MapStrategyTM.GetOrAddDataBlock(p, blockID, si.Method, si.Player.GamerID, true);
            break;
        }
      }
    }

    private int ValidateSetBlockAuxData(ScriptInstance si, Block blockID, int auxData)
    {
      int num = auxData & 7;
      Block block = blockID;
      if ((uint) block <= 124U)
      {
        if ((uint) block <= 113U)
        {
          switch (block)
          {
            case Block.None:
              return 0;
            case Block.Ladder:
              break;
            case Block.Stairs:
              goto label_8;
            default:
              goto label_20;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Sign:
              goto label_8;
            case Block.ClimbingIvy:
              break;
            case Block.Crop:
              if (num > 5)
                num = 5;
              if ((auxData & 240) >> 4 > 4)
              {
                auxData = 0;
                goto label_22;
              }
              else
                goto label_22;
            default:
              goto label_20;
          }
        }
        if (num > 3)
        {
          num = 0;
          goto label_22;
        }
        else
          goto label_22;
      }
      else
      {
        if ((uint) block <= 150U)
        {
          switch (block)
          {
            case Block.Stack:
            case Block.UpsideDownStack:
            case Block.SnowLayer:
            case Block.Ramp:
              goto label_8;
            case Block.HalfBlock:
              break;
            default:
              goto label_20;
          }
        }
        else
        {
          switch (block)
          {
            case Block.Painting:
            case Block.Stairs2:
            case Block.Ramp2:
            case Block.Stack2:
              goto label_8;
            case Block.Switch:
            case Block.Button:
              if (num > 5)
              {
                num = 5;
                goto label_22;
              }
              else
                goto label_22;
            case Block.HalfBlock2:
              break;
            default:
              goto label_20;
          }
        }
        if (num > 1)
        {
          num = 0;
          goto label_22;
        }
        else
          goto label_22;
      }
label_8:
      if (num > 7)
      {
        num = 0;
        goto label_22;
      }
      else
        goto label_22;
label_20:
      if (si.Instance.Map.BlockData[(int) blockID].IsAttached && num > 4)
        num = 4;
label_22:
      auxData = auxData & 240 | num;
      return auxData;
    }

    private void CommandSetBlockScript(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      DataBlockScriptType type2 = (DataBlockScriptType) reader.ReadByte();
      string name = reader.ReadString();
      if (!this.AdjustCoord(si, ref p, type1))
        return;
      si.Instance.MapStrategyTM.GetDataBlock(p)?.SetScript(name, type2);
    }

    private void CommandSetEventScript(ScriptInstance si, BinaryReader reader)
    {
      ScriptEvent e = (ScriptEvent) reader.ReadByte();
      string str = reader.ReadString();
      switch (e)
      {
        case ScriptEvent.ItemSwing:
        case ScriptEvent.ItemEquip:
        case ScriptEvent.ItemUnequip:
          Item itemID = reader.ReadBoolean() ? (Item) reader.ReadUInt16() : Item.None;
          si.Instance.SetEventScript(e, str, itemID);
          break;
        case ScriptEvent.ButtonX:
        case ScriptEvent.ButtonY:
        case ScriptEvent.ButtonB:
          if (si.Player == null)
            break;
          string text = reader.ReadString();
          Vector2? pos = new Vector2?();
          float? scale = new float?();
          if (reader.ReadBoolean())
            pos = new Vector2?(new Vector2(this.ReadSingle(si, reader), this.ReadSingle(si, reader)));
          if (reader.ReadBoolean())
            scale = new float?(this.ReadSingle(si, reader));
          int num;
          switch (e)
          {
            case ScriptEvent.ButtonX:
              num = 16384;
              break;
            case ScriptEvent.ButtonY:
              num = 32768;
              break;
            default:
              num = 8192;
              break;
          }
          Buttons button = (Buttons) num;
          si.Player.SetButtonScript(button, str, text, pos, scale);
          break;
        default:
          si.Instance.SetEventScript(e, str);
          break;
      }
    }

    private void CommandSetNameplate(ScriptInstance si, BinaryReader reader)
    {
      NamePlateSetting setting = (NamePlateSetting) reader.ReadByte();
      ScriptTarget scriptTarget = (ScriptTarget) reader.ReadByte();
      bool npc = reader.ReadBoolean();
      switch (scriptTarget)
      {
        case ScriptTarget.Actor:
          if (si.Player == null)
            break;
          this.SetNameplate(si.Player, setting, npc);
          break;
        case ScriptTarget.Clan:
          if (si.Player == null || !si.Player.ClanName.IsNotEmpty())
            break;
          using (List<NetworkGamer>.Enumerator enumerator = NetworkManager.Instance.LocalGamers.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Player tag = enumerator.Current.Tag as Player;
              if (tag != null && tag.ClanName == si.Player.ClanName)
                this.SetNameplate(tag, setting, npc);
            }
            break;
          }
        default:
          using (List<NetworkGamer>.Enumerator enumerator = NetworkManager.Instance.LocalGamers.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.SetNameplate(enumerator.Current.Tag as Player, setting, npc);
            break;
          }
      }
    }

    private void SetNameplate(Player player, NamePlateSetting setting, bool npc)
    {
      if (player == null)
        return;
      if (!npc)
        player.Settings.Nameplates = setting;
      else
        player.Settings.MobNameplates = setting != NamePlateSetting.None;
    }

    private void CommandSetPower(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      bool power = reader.ReadBoolean();
      if (!this.AdjustCoord(si, ref p, type))
        return;
      si.Instance.SetPower(p, power, Player.GetGamerID(si.Player));
    }

    private void CommandSetReach(ScriptInstance si, BinaryReader reader)
    {
      int num1 = this.ReadInt32(si, reader);
      if (num1 != 0)
      {
        int num2;
        num1 = MyMathHelper.Clamp(num2 = num1 + 1, 0, 32);
      }
      switch (reader.ReadByte())
      {
        case 2:
          if (si.Player == null)
            break;
          si.Player.Reach = num1;
          break;
        case 3:
          if (si.Player == null || !si.Player.ClanName.IsNotEmpty())
            break;
          using (List<NetworkGamer>.Enumerator enumerator = NetworkManager.Instance.LocalGamers.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Player tag = enumerator.Current.Tag as Player;
              if (tag != null && tag.ClanName == si.Player.ClanName)
                tag.Reach = num1;
            }
            break;
          }
        default:
          using (List<NetworkGamer>.Enumerator enumerator = NetworkManager.Instance.LocalGamers.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Player tag = enumerator.Current.Tag as Player;
              if (tag != null)
                tag.Reach = num1;
            }
            break;
          }
      }
    }

    private void CommandSetRegion(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D min = new GlobalPoint3D();
      min.X = this.ReadInt32(si, reader);
      min.Y = this.ReadInt32(si, reader);
      min.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D max = new GlobalPoint3D();
      max.X = this.ReadInt32(si, reader);
      max.Y = this.ReadInt32(si, reader);
      max.Z = this.ReadInt32(si, reader);
      Block blockID = (Block) reader.ReadByte();
      byte percent = (byte) this.ReadInt32(si, reader);
      int seed = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref min, type1) || !this.AdjustCoord(si, ref max, type2))
        return;
      if (percent < (byte) 1)
        percent = (byte) 1;
      else if (percent > (byte) 100)
        percent = (byte) 100;
      if (percent < (byte) 100 && seed == 0)
        seed = si.Random.Next();
      this.SetMinMax(ref min, ref max);
      si.Instance.CreativeModeHelper.RunClearFill(Globals2.GetGamerType(GamerType.Script), blockID, min, max, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, percent, seed, false, blockID == Block.None ? "Clear" : "Fill", false);
      si.NeedsCommit = true;
    }

    private void CommandSetRegionAux(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D min = new GlobalPoint3D();
      min.X = this.ReadInt32(si, reader);
      min.Y = this.ReadInt32(si, reader);
      min.Z = this.ReadInt32(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D max = new GlobalPoint3D();
      max.X = this.ReadInt32(si, reader);
      max.Y = this.ReadInt32(si, reader);
      max.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref min, type1) || !this.AdjustCoord(si, ref max, type2))
        return;
      this.SetMinMax(ref min, ref max);
      if ((max.X - min.X + 1) * (max.Y - min.Y + 1) * (max.Z - min.Z + 1) > 500000)
        return;
      bool flag = reader.ReadBoolean();
      byte num1 = (byte) (this.ReadInt32(si, reader) & (flag ? 15 : 7));
      if (flag)
        num1 <<= 4;
      int num2 = this.ReadInt32(si, reader);
      int num3 = this.ReadInt32(si, reader);
      if (si.Instance.IsInZoneType(min, max, ZoneType.Spawn, si.Actor.GamerID))
        return;
      if (num2 < 1)
        num2 = 1;
      else if (num2 > 100)
        num2 = 100;
      if (num2 < 100)
        si.Random.Seed(num3 == 0 ? si.Random.Next() : num3);
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Y = max.Y; zero.Y >= min.Y; --zero.Y)
      {
        for (zero.Z = min.Z; zero.Z <= max.Z; ++zero.Z)
        {
          for (zero.X = min.X; zero.X <= max.X; ++zero.X)
          {
            if (num2 == 100 || si.Random.Next(100) < num2)
            {
              MapBlock blockIdAndAuxNoCache = si.MapTM.GetBlockIDAndAuxNoCache(zero);
              int auxData = flag ? (int) blockIdAndAuxNoCache.AuxData & 15 | (int) num1 : (int) blockIdAndAuxNoCache.AuxData & 248 | (int) num1;
              int num4 = this.ValidateSetBlockAuxData(si, (Block) blockIdAndAuxNoCache.BlockID, auxData);
              si.MapTM.SetAuxData(zero, (byte) num4, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
            }
          }
        }
      }
      si.NeedsCommit = true;
    }

    private void CommandSetSphere(ScriptInstance si, BinaryReader reader)
    {
      if (!si.Instance.IsCreativeMode)
        return;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      byte radius = (byte) this.ReadInt32(si, reader);
      Block blockID1 = (Block) reader.ReadByte();
      byte percent = (byte) this.ReadInt32(si, reader);
      int seed = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      if (radius < (byte) 1)
        radius = (byte) 1;
      else if (radius > (byte) 150)
        radius = (byte) 150;
      if (percent < (byte) 1)
        percent = (byte) 1;
      else if (percent > (byte) 100)
        percent = (byte) 100;
      if (percent < (byte) 100 && seed == 0)
        seed = si.Random.Next();
      si.Instance.CreativeModeHelper.RunSetSphere(si.MapTM, Globals2.GetGamerType(GamerType.Script), p, blockID1, radius, percent, seed, "Sphere", (Action<CreativeOperationData>) null, false);
      si.NeedsCommit = true;
    }

    private void CommandSetSwitch(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      ScriptSwitch scriptSwitch = (ScriptSwitch) reader.ReadByte();
      if (!this.AdjustCoord(si, ref p, type))
        return;
      switch (scriptSwitch)
      {
        case ScriptSwitch.Off:
          si.NeedsCommit = si.Instance.SetSwitch(p, false, UpdateBlockMethod.Player, si.Player, false);
          break;
        case ScriptSwitch.On:
          si.NeedsCommit = si.Instance.SetSwitch(p, true, UpdateBlockMethod.Player, si.Player, false);
          break;
        case ScriptSwitch.Toggle:
          si.NeedsCommit = si.Instance.HitSwitch(p, UpdateBlockMethod.Player, si.Player, false);
          break;
      }
    }

    private void CommandSetText(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      string str = reader.ReadString();
      bool flag1 = reader.ReadBoolean();
      int num1 = flag1 ? 0 : this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      bool isHost = si.MapTM.IsHost;
      MapStrategyTM mapStrategyTm = si.Instance.MapStrategyTM;
      Block blockIdNoCache = (Block) si.MapTM.GetBlockIDNoCache(p);
      DataBlock block = si.Instance.MapStrategyTM.GetDataBlock(p);
      bool flag2 = false;
      bool flag3 = false;
      if (isHost && block == null && str.IsNotEmpty())
      {
        block = mapStrategyTm.NewDataBlock(p, blockIdNoCache, Globals2.GetGamerType(GamerType.Script));
        if (block != null)
          mapStrategyTm.AddDataBlock(block, si.Method, true);
        flag2 = true;
      }
      if (block == null)
        return;
      BookData book = (BookData) null;
      BookBlock bookBlock = block as BookBlock;
      if (blockIdNoCache == Block.Book && bookBlock != null)
      {
        ushort id = bookBlock.ID;
        if (id > (ushort) 1)
        {
          book = mapStrategyTm.GetBookData(id);
          if (isHost && book == null)
          {
            book = mapStrategyTm.AddBookData((BookData) null);
            book.ID = id;
            flag3 = true;
          }
        }
        else if (isHost && (flag1 || num1 > 0))
        {
          book = new BookData();
          si.Instance.AddBookData(book, si.Player, (short) -1, false);
          bookBlock.ID = book.ID;
          flag3 = true;
        }
      }
      switch (block.ClassType)
      {
        case DataBlockType.Sign:
          SignBlock signBlock = block as SignBlock;
          if (signBlock != null)
          {
            string text = Globals2.SubstituteText(si, str);
            signBlock.SetText(si.MapTM, text);
            si.Instance.MapRenderer.SignsChanged(false);
            break;
          }
          break;
        case DataBlockType.Book:
          if (book != null && bookBlock != null)
          {
            if (num1 > 0 && !flag1)
            {
              int num2 = num1 > 1000 ? 1000 : num1;
              book.SetText(str, num2 - 1);
            }
            else
              book.Title = str;
            if (flag3)
            {
              NetworkManager.Instance.SendBookUpdate(book);
              break;
            }
            break;
          }
          break;
      }
      if (!isHost || !flag2)
        return;
      NetworkManager.Instance.SendDataBlockChange(block, true, si.Method);
    }

    private void CommandSetTexture(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      int num = this.ReadInt32(si, reader);
      if (num < 0 || num > 16 || !this.AdjustCoord(si, ref p, type))
        return;
      MapBlock blockIdAndAuxNoCache = si.MapTM.GetBlockIDAndAuxNoCache(p);
      bool flag = si.MapTM.BlockData[(int) blockIdAndAuxNoCache.BlockID].Buffer == (byte) 0;
      if (!flag && !si.MapTM.UsesBlockTextureTable((Block) blockIdAndAuxNoCache.BlockID))
        return;
      int length1 = MapTM.DecalNames.Length;
      if (flag && num > length1)
        num = 0;
      int length2 = MapTM.CoverBlockTop.Length;
      if (blockIdAndAuxNoCache.BlockID == (byte) 126 && num > length2)
        num = (int) (byte) length2;
      byte auxData = (byte) (((int) blockIdAndAuxNoCache.AuxData & 15) + (num << 4));
      si.MapTM.SetAuxData(p, auxData, si.Method, Globals2.GetGamerType(GamerType.Script), false);
      si.NeedsCommit = true;
    }

    private void CommandSkill(ScriptInstance si, BinaryReader reader)
    {
      SkillType type = (SkillType) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      int num = this.ReadInt32(si, reader);
      if (si.Player == null || !Globals2.GameProperties.SaveGame.Header.SkillsLocal || num == 0)
        return;
      if (flag)
      {
        Math.Max(1, Math.Min(200, num));
        si.Player.SkillsData.SetLevel(type, num);
      }
      else
      {
        int level = Math.Max(1, Math.Min(200, si.Player.SkillsData[(int) type].Level + num));
        si.Player.SkillsData.SetLevel(type, level);
      }
    }

    private void CommandSkillXP(ScriptInstance si, BinaryReader reader)
    {
      SkillType type = (SkillType) reader.ReadByte();
      bool flag = reader.ReadBoolean();
      double xp1 = this.ReadDouble(si, reader);
      if (si.Player == null || !Globals2.GameProperties.SaveGame.Header.SkillsLocal || xp1 == 0.0)
        return;
      if (xp1 < -1000000.0)
        xp1 = -1000000.0;
      if (flag && xp1 > 10000000.0)
        xp1 = 10000000.0;
      if (!flag && xp1 > 1000000.0)
        xp1 = 1000000.0;
      if (flag)
      {
        si.Player.SkillsData.SetXPExternal(si.Player, type, xp1, false);
      }
      else
      {
        double xp2 = si.Player.SkillsData[(int) type].CurrentXP + xp1;
        if (xp2 < 0.0)
          xp2 = 0.0;
        si.Player.SkillsData.SetXPExternal(si.Player, type, xp2, true);
      }
    }

    private void CommandSkyColor(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      Vector3 vector3 = new Vector3(this.ReadSingle(si, reader), this.ReadSingle(si, reader), this.ReadSingle(si, reader));
      int num1 = this.ReadInt32(si, reader);
      int num2 = this.ReadInt32(si, reader);
      vector3.X /= (float) byte.MaxValue;
      vector3.Y /= (float) byte.MaxValue;
      vector3.Z /= (float) byte.MaxValue;
      if (num1 < 0 || num1 > 100)
        num1 = 100;
      if (num2 < 0)
        num2 = 3000;
      else if (num2 == 0)
        num2 = 1;
      if (flag)
      {
        if (si.Player == null)
          return;
        si.Player.CustomSkyColor.Start(si.Player.CustomSkyColor.CurrentValue, new Vector4(vector3, (float) num1 / 100f), (double) num2 / 1000.0);
      }
      else
        GraphicStatics.CustomSkyColor.Start(GraphicStatics.CustomSkyColor.CurrentValue, new Vector4(vector3, (float) num1 / 100f), (double) num2 / 1000.0);
    }

    private void CommandSound(ScriptInstance si, BinaryReader reader)
    {
      string cueName = reader.ReadString();
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      ScriptCoordType type2 = ScriptCoordType.None;
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      GlobalPoint3D max = new GlobalPoint3D();
      string name = (string) null;
      if (type1 != ScriptCoordType.None)
      {
        globalPoint3D.X = this.ReadInt32(si, reader);
        globalPoint3D.Y = this.ReadInt32(si, reader);
        globalPoint3D.Z = this.ReadInt32(si, reader);
        type2 = (ScriptCoordType) reader.ReadByte();
        if (type2 != ScriptCoordType.None)
        {
          max.X = this.ReadInt32(si, reader);
          max.Y = this.ReadInt32(si, reader);
          max.Z = this.ReadInt32(si, reader);
        }
      }
      else
        name = reader.ReadString();
      if (reader.ReadBoolean())
      {
        if (name != null)
        {
          Zone zone = si.MapTM.MapStrategyTM.GetZone(name);
          if (zone == null)
            return;
          si.Instance.AmbientSoundManager.RemoveSound(cueName, zone.Min, zone.Max);
        }
        else
        {
          if (!this.AdjustCoord(si, ref globalPoint3D, type1))
            return;
          if (type2 != ScriptCoordType.None)
          {
            if (!this.AdjustCoord(si, ref max, type2))
              return;
            this.SetMinMax(ref globalPoint3D, ref max);
            si.Instance.AmbientSoundManager.RemoveSound(cueName, globalPoint3D, max);
          }
          else
            si.Instance.AmbientSoundManager.RemoveSound(cueName, globalPoint3D);
        }
      }
      else
      {
        int num1 = MyMathHelper.Clamp(this.ReadInt32(si, reader), 0, 100);
        int num2 = this.ReadInt32(si, reader);
        if (num2 == 0)
          num2 = 20;
        bool flag = reader.ReadBoolean();
        int num3 = 0;
        int loopCount = 1;
        if (flag)
        {
          num3 = this.ReadInt32(si, reader);
          loopCount = this.ReadInt32(si, reader);
        }
        if (name != null)
        {
          Zone zone = si.MapTM.MapStrategyTM.GetZone(name);
          if (zone == null)
            return;
          si.Instance.AmbientSoundManager.AddSound(cueName, zone.Min, zone.Max, (float) num1 / 100f, loopCount, (float) num3 / 1000f);
        }
        else
        {
          if (!this.AdjustCoord(si, ref globalPoint3D, type1))
            return;
          if (type2 != ScriptCoordType.None)
          {
            if (!this.AdjustCoord(si, ref max, type2))
              return;
            this.SetMinMax(ref globalPoint3D, ref max);
            si.Instance.AmbientSoundManager.AddSound(cueName, globalPoint3D, max, (float) num1 / 100f, loopCount, (float) num3 / 1000f);
          }
          else
            si.Instance.AmbientSoundManager.AddSound(cueName, globalPoint3D, (float) num1 / 100f, (float) num2, loopCount, (float) num3 / 1000f);
        }
      }
    }

    private void CommandTeleport(ScriptInstance si, BinaryReader reader)
    {
      ScriptTarget scriptTarget = (ScriptTarget) reader.ReadByte();
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      globalPoint3D.X = this.ReadInt32(si, reader);
      globalPoint3D.Y = this.ReadInt32(si, reader);
      globalPoint3D.Z = this.ReadInt32(si, reader);
      if (scriptTarget == ScriptTarget.Actor)
      {
        if (!this.AdjustCoord(si, ref globalPoint3D, type1) || si.Actor == null || si.Actor.IsGod)
          return;
        si.Actor.TeleportTo(globalPoint3D, false);
      }
      else
      {
        ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
        GlobalPoint3D max = new GlobalPoint3D();
        max.X = this.ReadInt32(si, reader);
        max.Y = this.ReadInt32(si, reader);
        max.Z = this.ReadInt32(si, reader);
        ScriptCoordType type3 = (ScriptCoordType) reader.ReadByte();
        GlobalPoint3D p = new GlobalPoint3D();
        p.X = this.ReadInt32(si, reader);
        p.Y = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        bool relative = reader.ReadBoolean();
        if (!this.AdjustCoord(si, ref globalPoint3D, type1, false) || !this.AdjustCoord(si, ref max, type2, false) || !this.AdjustCoord(si, ref p, type3))
          return;
        this.SetMinMax(ref globalPoint3D, ref max);
        si.Instance.TeleportEntities(globalPoint3D, max, p, relative);
      }
    }

    private void CommandTintColor(ScriptInstance si, BinaryReader reader)
    {
      bool flag = reader.ReadBoolean();
      Vector3 vector3 = new Vector3(this.ReadSingle(si, reader), this.ReadSingle(si, reader), this.ReadSingle(si, reader));
      int num = this.ReadInt32(si, reader);
      vector3.X *= 0.01f;
      vector3.Y *= 0.01f;
      vector3.Z *= 0.01f;
      if (num < 0)
        num = 3000;
      else if (num == 0)
        num = 1;
      if (flag)
      {
        if (si.Player == null)
          return;
        si.Player.CustomTintColor.Start(si.Player.CustomTintColor.CurrentValue, vector3, (double) num / 1000.0);
      }
      else
        GraphicStatics.CustomTintColor.Start(GraphicStatics.CustomTintColor.CurrentValue, vector3, (double) num / 1000.0);
    }

    private void CommandUnequip(ScriptInstance si, BinaryReader reader)
    {
      InventoryHand inventoryHand = (InventoryHand) reader.ReadByte();
      bool flag1 = reader.ReadBoolean();
      Item itemID = (Item) reader.ReadUInt16();
      bool flag2 = itemID != Item.None;
      if (si.Player == null)
        return;
      Hand hand1;
      switch (inventoryHand)
      {
        case InventoryHand.None:
          if (!flag1)
          {
            if (!flag2)
            {
              si.Player.EquipFromInventory(si.Player.LeftHand, Item.None);
              si.Player.EquipFromInventory(si.Player.RightHand, Item.None);
              si.Player.UnequipToInventory(EquipIndex.Head);
              si.Player.UnequipToInventory(EquipIndex.Neck);
              si.Player.UnequipToInventory(EquipIndex.Body);
              si.Player.UnequipToInventory(EquipIndex.Legs);
              si.Player.UnequipToInventory(EquipIndex.Feet);
              si.Player.UnequipToInventory(EquipIndex.LeftSide);
              si.Player.UnequipToInventory(EquipIndex.RightSide);
            }
            else
            {
              if (si.Player.LeftHand.ItemID == itemID)
                si.Player.EquipFromInventory(si.Player.LeftHand, Item.None);
              if (si.Player.RightHand.ItemID == itemID)
                si.Player.EquipFromInventory(si.Player.RightHand, Item.None);
            }
          }
          if (!flag2)
            return;
          EquipIndex itemEquipIndex = ItemData.GetItemEquipIndex(itemID);
          switch (itemEquipIndex)
          {
            case EquipIndex.None:
              return;
            case EquipIndex.LeftHand:
              return;
            case EquipIndex.RightHand:
              return;
            default:
              si.Player.UnequipToInventory(itemEquipIndex);
              return;
          }
        case InventoryHand.Left:
          hand1 = si.Player.LeftHand;
          break;
        default:
          hand1 = si.Player.RightHand;
          break;
      }
      Hand hand2 = hand1;
      if (flag2 && hand2.ItemID != itemID)
        return;
      si.Player.EquipFromInventory(hand2, Item.None);
    }

    private void CommandVar(ScriptInstance si, BinaryReader reader)
    {
      int index1 = (int) reader.ReadUInt16();
      byte num1 = reader.ReadByte();
      if (num1 <= (byte) 0)
        return;
      double num2 = 0.0;
      for (int index2 = 0; index2 < (int) num1; ++index2)
      {
        double valnum = 0.0;
        this.varload[(int) reader.ReadByte()](si, reader, out valnum);
        switch (reader.ReadByte())
        {
          case 0:
            num2 = valnum;
            break;
          case 1:
            num2 += valnum;
            break;
          case 2:
            num2 -= valnum;
            break;
          case 3:
            num2 *= valnum;
            break;
          case 4:
            if (valnum != 0.0)
            {
              num2 /= valnum;
              break;
            }
            break;
          case 5:
            num2 = Math.Round(num2 % valnum, 10);
            break;
        }
      }
      si.Vars[index1] = num2;
    }

    private void VarLoadNumLiterial(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadDouble();
    }

    private void VarLoadVariable(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = si.GetVarValue(reader.ReadUInt16());
    }

    private void VarLoadHistoryKey(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      string key = reader.ReadString();
      if (si.Player == null)
        return;
      valueNum = (double) si.Player.History.GetHistory(key);
    }

    private void VarLoadSysHistoryKey(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      string key = reader.ReadString();
      valueNum = (double) si.Instance.History.GetHistory(key);
    }

    private void VarLoadClanHistoryKey(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      string key = reader.ReadString();
      if (si.Player == null)
        return;
      History clanHistory = si.Instance.GetClanHistory(si.Player.ClanName);
      if (clanHistory == null)
        return;
      valueNum = (double) clanHistory.GetHistory(key);
    }

    private void VarLoadRandom(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      int num = reader.ReadByte() != (byte) 1 ? reader.ReadInt32() : (int) si.GetVarValue(reader.ReadUInt16());
      valueNum = (double) si.Random.Next(num + 1);
    }

    private void VarLoadSkill(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      SkillType skillType = (SkillType) reader.ReadUInt16();
      if (si.Actor == null)
        return;
      if (si.Actor.IsPlayer)
      {
        ref double local = ref valueNum;
        int num1;
        switch (skillType)
        {
          case SkillType.Combat:
            num1 = si.Actor.SkillsData.CombatLevel;
            break;
          case SkillType.Total:
            num1 = si.Actor.SkillsData.TotalLevel;
            break;
          default:
            num1 = si.Actor.SkillsData[(int) skillType].Level;
            break;
        }
        double num2 = (double) num1;
        local = num2;
      }
      else
      {
        switch (skillType)
        {
          case SkillType.Health:
            valueNum = (double) si.Actor.HealthLevel(false);
            break;
          case SkillType.Strength:
            valueNum = (double) si.Actor.StrengthLevel(false);
            break;
          case SkillType.Attack:
            valueNum = (double) si.Actor.AttackLevel(false);
            break;
          case SkillType.Defence:
            valueNum = (double) si.Actor.DefenceLevel(false);
            break;
          case SkillType.Ranged:
            valueNum = (double) si.Actor.RangedLevel(false);
            break;
          case SkillType.Looting:
            valueNum = (double) si.Actor.LootingLevel(false);
            break;
          case SkillType.Combat:
            valueNum = (double) si.Actor.CombatLevel;
            break;
          default:
            valueNum = 0.0;
            break;
        }
      }
    }

    private void VarLoadSkillXP(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      SkillType skillType = (SkillType) reader.ReadUInt16();
      if (si.Actor == null)
        return;
      ref double local = ref valueNum;
      double num;
      switch (skillType)
      {
        case SkillType.Combat:
          num = 0.0;
          break;
        case SkillType.Total:
          num = si.Actor.SkillsData.TotalXP;
          break;
        default:
          num = si.Actor.SkillsData[(int) skillType].CurrentXP;
          break;
      }
      local = num;
    }

    private void VarLoadHealth(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.Health;
    }

    private void VarLoadMaxHealth(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.MaxHealth;
    }

    private void VarLoadReach(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Player == null)
        return;
      valueNum = (double) si.Player.Reach;
    }

    private void VarLoadPosX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.Position.X;
    }

    private void VarLoadPosY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.Position.Y;
    }

    private void VarLoadPosZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.Position.Z;
    }

    private void VarLoadEyeY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.EyePosition.Y;
    }

    private void VarLoadVelX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.VisualVelocity.X;
    }

    private void VarLoadVelY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.VisualVelocity.Y;
    }

    private void VarLoadVelZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.VisualVelocity.Z;
    }

    private void VarLoadViewX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.ViewDirection.X;
    }

    private void VarLoadViewY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.ViewDirection.Y;
    }

    private void VarLoadViewZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.Actor.ViewDirection.Z;
    }

    private void VarLoadRelX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.BlockOffset.HasValue)
        return;
      valueNum = (double) si.BlockOffset.Value.X;
    }

    private void VarLoadRelY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.BlockOffset.HasValue)
        return;
      valueNum = (double) si.BlockOffset.Value.Y;
    }

    private void VarLoadRelZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.BlockOffset.HasValue)
        return;
      valueNum = (double) si.BlockOffset.Value.Z;
    }

    private void VarLoadCRelX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null || !si.Actor.SwingTargetIsValid)
        return;
      valueNum = (double) si.Actor.SwingTarget.X;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.X;
    }

    private void VarLoadCRelY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null || !si.Actor.SwingTargetIsValid)
        return;
      valueNum = (double) si.Actor.SwingTarget.Y;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.Y;
    }

    private void VarLoadCRelZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null || !si.Actor.SwingTargetIsValid)
        return;
      valueNum = (double) si.Actor.SwingTarget.Z;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.Z;
    }

    private void VarLoadPRelX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.MapTM.GetPoint(si.Actor.Position).X;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.X;
    }

    private void VarLoadPRelY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.MapTM.GetPoint(si.Actor.Position).Y;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.Y;
    }

    private void VarLoadPRelZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (si.Actor == null)
        return;
      valueNum = (double) si.MapTM.GetPoint(si.Actor.Position).Z;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum -= (double) si.ScriptOffset.Value.Z;
    }

    private void VarLoadScriptX(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum = (double) si.ScriptOffset.Value.X;
    }

    private void VarLoadScriptY(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum = (double) si.ScriptOffset.Value.Y;
    }

    private void VarLoadScriptZ(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      if (!si.ScriptOffset.HasValue)
        return;
      valueNum = (double) si.ScriptOffset.Value.Z;
    }

    private void VarLoadAbs(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = Math.Abs(valueNum);
    }

    private void VarLoadSin(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = Math.Sin(valueNum);
    }

    private void VarLoadCos(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = Math.Cos(valueNum);
    }

    private void VarLoadTan(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = Math.Tan(valueNum);
    }

    private void VarLoadSqrt(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      if (valueNum == 0.0)
        return;
      valueNum = Math.Sqrt(valueNum);
    }

    private void VarLoadSingle(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = valueNum;
    }

    private void VarLoadInt(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = (double) (int) valueNum;
    }

    private void VarLoadNeg(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = reader.ReadByte() != (byte) 1 ? reader.ReadDouble() : si.GetVarValue(reader.ReadUInt16());
      valueNum = -valueNum;
    }

    private void VarLoadInv(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      Item itemID = (Item) reader.ReadUInt16();
      if (reader.ReadBoolean())
      {
        valueNum = 0.0;
        GlobalPoint3D p = new GlobalPoint3D();
        ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
        p.X = this.ReadInt32(si, reader);
        p.Y = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        if (!this.AdjustCoord(si, ref p, type))
          return;
        ChestBlock dataBlock = si.MapTM.MapStrategyTM.GetDataBlock(p) as ChestBlock;
        if (dataBlock == null || dataBlock.Inventory == null)
          return;
        valueNum = (double) dataBlock.Inventory.GetItemCount(itemID);
      }
      else
        valueNum = (double) si.Actor.Inventory.GetItemCount(itemID);
    }

    private void VarLoadDistance(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      Vector3 pos1 = new Vector3();
      pos1.X = this.ReadSingle(si, reader);
      pos1.Y = this.ReadSingle(si, reader);
      pos1.Z = this.ReadSingle(si, reader);
      ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
      Vector3 pos2 = new Vector3();
      pos2.X = this.ReadSingle(si, reader);
      pos2.Y = this.ReadSingle(si, reader);
      pos2.Z = this.ReadSingle(si, reader);
      if (!this.AdjustCoord(si, ref pos1, type1, false) || !this.AdjustCoord(si, ref pos2, type2, false))
        return;
      valueNum = (double) Vector3.Distance(pos1, pos2);
    }

    private void VarLoadClock(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = (double) si.Instance.SunMoon.CurrentHour;
    }

    private void VarLoadGamerCount(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      int num1 = 0;
      ScriptGamerTarget gamerTarget = (ScriptGamerTarget) reader.ReadByte();
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      if (type1 != ScriptCoordType.None)
      {
        GlobalPoint3D p1 = new GlobalPoint3D();
        p1.X = this.ReadInt32(si, reader);
        p1.Y = this.ReadInt32(si, reader);
        p1.Z = this.ReadInt32(si, reader);
        ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
        if (type2 != ScriptCoordType.None)
        {
          GlobalPoint3D p2 = new GlobalPoint3D();
          p2.X = this.ReadInt32(si, reader);
          p2.Y = this.ReadInt32(si, reader);
          p2.Z = this.ReadInt32(si, reader);
          if (this.AdjustCoord(si, ref p1, type1, false) && this.AdjustCoord(si, ref p2, type2, false))
          {
            Vector3 position1 = si.MapTM.GetPosition(p1);
            Vector3 position2 = si.MapTM.GetPosition(p2);
            this.SetMinMax(ref position1, ref position2);
            position1.Y -= si.MapTM.TileSize;
            position2.X += si.MapTM.TileSize;
            position2.Z += si.MapTM.TileSize;
            num1 = this.GetGamerCount(si.Instance.NetworkManager.AllEnabledGamers, gamerTarget, position1, position2);
          }
        }
        else
        {
          float radius = this.ReadSingle(si, reader);
          if (this.AdjustCoord(si, ref p1, type1, false))
          {
            Vector3 blockCenter = si.MapTM.GetBlockCenter(p1);
            num1 = this.GetGamerCount(si.Instance.NetworkManager.AllEnabledGamers, gamerTarget, blockCenter, radius);
          }
        }
      }
      else if (reader.ReadBoolean())
      {
        string name = reader.ReadString();
        GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
        Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
        if (zone == null && gamerID.IsGamer)
          zone = si.Instance.MapStrategyTM.GetZone(name);
        if (zone != null)
        {
          Vector3 position1 = si.MapTM.GetPosition(zone.Min);
          Vector3 position2 = si.MapTM.GetPosition(zone.Max);
          position1.Y -= si.MapTM.TileSize;
          position2.X += si.MapTM.TileSize;
          position2.Z += si.MapTM.TileSize;
          num1 = this.GetGamerCount(si.Instance.NetworkManager.AllEnabledGamers, gamerTarget, position1, position2);
        }
      }
      else
      {
        int num2;
        switch (gamerTarget)
        {
          case ScriptGamerTarget.Local:
            num2 = si.Instance.NetworkManager.LocalGamerCount;
            break;
          case ScriptGamerTarget.Remote:
            num2 = si.Instance.NetworkManager.RemoteGamerCount;
            break;
          default:
            num2 = si.Instance.NetworkManager.AllGamerCount;
            break;
        }
        num1 = num2;
      }
      valueNum = (double) num1;
    }

    private void VarLoadNpcCount(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      int num = 0;
      NpcManager npcManager = si.Instance.NpcManager;
      ActorType actorType = (ActorType) reader.ReadByte();
      ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
      if (type1 != ScriptCoordType.None)
      {
        GlobalPoint3D p1 = new GlobalPoint3D();
        p1.X = this.ReadInt32(si, reader);
        p1.Y = this.ReadInt32(si, reader);
        p1.Z = this.ReadInt32(si, reader);
        ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
        if (type2 != ScriptCoordType.None)
        {
          GlobalPoint3D p2 = new GlobalPoint3D();
          p2.X = this.ReadInt32(si, reader);
          p2.Y = this.ReadInt32(si, reader);
          p2.Z = this.ReadInt32(si, reader);
          if (this.AdjustCoord(si, ref p1, type1, false) && this.AdjustCoord(si, ref p2, type2, false))
          {
            Vector3 position1 = si.MapTM.GetPosition(p1);
            Vector3 position2 = si.MapTM.GetPosition(p2);
            this.SetMinMax(ref position1, ref position2);
            position1.Y -= si.MapTM.TileSize;
            position2.X += si.MapTM.TileSize;
            position2.Z += si.MapTM.TileSize;
            num = npcManager.GetNpcCountNearPosition(actorType, position1, position2);
          }
        }
        else
        {
          float range = this.ReadSingle(si, reader);
          if (this.AdjustCoord(si, ref p1, type1, false))
          {
            Vector3 blockCenter = si.MapTM.GetBlockCenter(p1);
            num = npcManager.GetNpcCountNearPosition(actorType, blockCenter, range);
          }
        }
      }
      else if (reader.ReadBoolean())
      {
        string name = reader.ReadString();
        GamerID gamerID = si.Player != null ? si.Player.GamerID : GamerID.Sys1;
        Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
        if (zone == null && gamerID.IsGamer)
          zone = si.Instance.MapStrategyTM.GetZone(name);
        if (zone != null)
        {
          Vector3 position1 = si.MapTM.GetPosition(zone.Min);
          Vector3 position2 = si.MapTM.GetPosition(zone.Max);
          position1.Y -= si.MapTM.TileSize;
          position2.X += si.MapTM.TileSize;
          position2.Z += si.MapTM.TileSize;
          num = npcManager.GetNpcCountNearPosition(actorType, position1, position2);
        }
      }
      else
        num = npcManager.GetNpcCount(actorType);
      valueNum = (double) num;
    }

    private void VarLoadPi(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = Math.PI;
    }

    private void VarLoadLight(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      MapLight lightNoCache = si.MapTM.GetLightNoCache(p);
      valueNum = (double) Math.Max(lightNoCache.BlockLight, lightNoCache.SunLight);
    }

    private void VarLoadHash(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = (double) Globals2.SubstituteGamertag(reader.ReadString(), si.Player).GetHashCode();
    }

    private void VarLoadBlockID(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      valueNum = (double) si.MapTM.GetBlockIDNoCache(p);
    }

    private void VarLoadAux(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      valueNum = (double) si.MapTM.GetAuxFullDataNoCache(p);
    }

    private void VarLoadSunLight(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      valueNum = (double) si.MapTM.GetLightNoCache(p).SunLight;
    }

    private void VarLoadBlockLight(ScriptInstance si, BinaryReader reader, out double valueNum)
    {
      valueNum = 0.0;
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.ReadInt32(si, reader);
      p.Y = this.ReadInt32(si, reader);
      p.Z = this.ReadInt32(si, reader);
      if (!this.AdjustCoord(si, ref p, type))
        return;
      valueNum = (double) si.MapTM.GetLightNoCache(p).BlockLight;
    }

    private void CommandWait(ScriptInstance si, BinaryReader reader)
    {
      int num = this.ReadInt32(si, reader);
      if (num <= 0)
        return;
      si.WaitTime = this.clock.ElapsedMilliseconds + (long) num;
    }

    private void CommandWaypoint(ScriptInstance si, BinaryReader reader)
    {
      ScriptCoordType type = (ScriptCoordType) reader.ReadByte();
      if (type == ScriptCoordType.None)
      {
        if (si.Player == null)
          return;
        si.Player.Waypoint = new GlobalPoint3D?();
      }
      else
      {
        GlobalPoint3D p = new GlobalPoint3D();
        p.X = this.ReadInt32(si, reader);
        p.Z = this.ReadInt32(si, reader);
        if (si.Player == null || !this.AdjustCoord2D(si, ref p, type))
          return;
        p.Y = (int) si.MapTM.GetHeight(p);
        si.Player.Waypoint = new GlobalPoint3D?(p);
      }
    }

    private void CommandZone(ScriptInstance si, BinaryReader reader)
    {
      bool flag1 = reader.ReadBoolean();
      if (flag1 && si.Player == null)
        return;
      string name = reader.ReadString();
      bool flag2 = reader.ReadBoolean();
      GamerID gamerID = flag1 ? si.Player.GamerID : GamerID.Sys1;
      if (flag2)
      {
        si.Instance.DeleteZone(name, gamerID);
      }
      else
      {
        Zone zone = si.Instance.MapStrategyTM.GetZone(name, gamerID);
        bool flag3 = zone == null;
        if (flag3)
          zone = new Zone(name, ZoneType.None, GlobalPoint3D.Zero, GlobalPoint3D.Zero)
          {
            GamerID = gamerID
          };
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.Spawn, reader.ReadBoolean());
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.NoEdit, reader.ReadBoolean());
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.NoCombat, reader.ReadBoolean());
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.NoFly, reader.ReadBoolean());
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.NoMobs, reader.ReadBoolean());
        if (reader.ReadBoolean())
          zone.SetType(ZoneType.NoEscape, reader.ReadBoolean());
        bool flag4 = false;
        ScriptCoordType type1 = (ScriptCoordType) reader.ReadByte();
        if (type1 != ScriptCoordType.None)
        {
          zone.Min.X = this.ReadInt32(si, reader);
          zone.Min.Y = this.ReadInt32(si, reader);
          zone.Min.Z = this.ReadInt32(si, reader);
          this.AdjustCoord(si, ref zone.Min, type1);
          zone.Min = GlobalPoint3D.Clamp(si.MapTM.MapBound.Min, si.MapTM.MapBound.Max - GlobalPoint3D.One, zone.Min);
          flag4 = true;
        }
        ScriptCoordType type2 = (ScriptCoordType) reader.ReadByte();
        if (type2 != ScriptCoordType.None)
        {
          zone.Max.X = this.ReadInt32(si, reader);
          zone.Max.Y = this.ReadInt32(si, reader);
          zone.Max.Z = this.ReadInt32(si, reader);
          this.AdjustCoord(si, ref zone.Max, type2);
          this.SetMinMax(ref zone.Min, ref zone.Max);
          zone.Max = GlobalPoint3D.Clamp(si.MapTM.MapBound.Min, si.MapTM.MapBound.Max - GlobalPoint3D.One, zone.Max);
          flag4 = true;
        }
        if (reader.ReadBoolean())
        {
          zone.BuilderType = (ZoneBuilderType) reader.ReadByte();
          if (si.Player != null)
          {
            switch (zone.BuilderType)
            {
              case ZoneBuilderType.Player:
                zone.Builder = si.Player.Gamertag;
                break;
              case ZoneBuilderType.Clan:
                zone.Builder = si.Player.ClanName;
                break;
              default:
                zone.Builder = (string) null;
                break;
            }
          }
        }
        if (reader.ReadBoolean())
          zone.OnEntryScriptName = reader.ReadString();
        if (reader.ReadBoolean())
          zone.OnExitScriptName = reader.ReadString();
        if (reader.ReadBoolean())
          zone.CombatLevelDifference = (short) this.ReadInt32(si, reader);
        if (reader.ReadBoolean())
          zone.SpeedMultiplier = this.ReadSingle(si, reader);
        if (reader.ReadBoolean())
          zone.GravityMultiplier = this.ReadSingle(si, reader);
        zone.CombatLevelDifference = (short) MathHelper.Clamp((float) zone.CombatLevelDifference, 0.0f, 200f);
        zone.SpeedMultiplier = MathHelper.Clamp((float) Math.Round((double) zone.SpeedMultiplier, 3), 0.0f, 10f);
        zone.GravityMultiplier = MathHelper.Clamp((float) Math.Round((double) zone.GravityMultiplier, 3), -10f, 10f);
        if ((double) zone.SpeedMultiplier == 0.0)
          zone.SpeedMultiplier = 1f;
        if ((double) zone.GravityMultiplier == 0.0)
          zone.GravityMultiplier = 1f;
        if (flag3)
        {
          si.Instance.MapStrategyTM.AddZone(zone);
        }
        else
        {
          if (!flag4)
            return;
          si.Instance.MapStrategyTM.UpdateZoneBound(zone);
        }
      }
    }

    private void SetMinMax(ref GlobalPoint3D min, ref GlobalPoint3D max)
    {
      if (min.X > max.X)
      {
        int x = min.X;
        min.X = max.X;
        max.X = x;
      }
      if (min.Y > max.Y)
      {
        int y = min.Y;
        min.Y = max.Y;
        max.Y = y;
      }
      if (min.Z <= max.Z)
        return;
      int z = min.Z;
      min.Z = max.Z;
      max.Z = z;
    }

    private void SetMinMax(ref Vector3 min, ref Vector3 max)
    {
      if ((double) min.X > (double) max.X)
      {
        float x = min.X;
        min.X = max.X;
        max.X = x;
      }
      if ((double) min.Y > (double) max.Y)
      {
        float y = min.Y;
        min.Y = max.Y;
        max.Y = y;
      }
      if ((double) min.Z <= (double) max.Z)
        return;
      float z = min.Z;
      min.Z = max.Z;
      max.Z = z;
    }

    private bool AdjustCoord(ScriptInstance si, ref GlobalPoint3D p, ScriptCoordType type)
    {
      return this.AdjustCoord(si, ref p, type, true);
    }

    private bool AdjustCoord(
      ScriptInstance si,
      ref GlobalPoint3D p,
      ScriptCoordType type,
      bool checkBounds)
    {
      return type != ScriptCoordType.None && this.AdjustCoordCore(si, ref p, type) && (!checkBounds || p.X >= si.MapTM.MapBound.Min.X && p.X < si.MapTM.MapBound.Max.X && (p.Y > si.MapTM.MapBound.Min.Y && p.Y < si.MapTM.MapBound.Max.Y) && (p.Z >= si.MapTM.MapBound.Min.Z && p.Z < si.MapTM.MapBound.Max.Z));
    }

    private bool AdjustCoord2D(ScriptInstance si, ref GlobalPoint3D p, ScriptCoordType type)
    {
      if (type == ScriptCoordType.None || !this.AdjustCoordCore(si, ref p, type))
        return false;
      p.Y = 0;
      return p.X >= si.MapTM.MapBound.Min.X && p.X < si.MapTM.MapBound.Max.X && (p.Z >= si.MapTM.MapBound.Min.Z && p.Z < si.MapTM.MapBound.Max.Z);
    }

    private bool AdjustCoordCore(ScriptInstance si, ref GlobalPoint3D p, ScriptCoordType type)
    {
      switch (type)
      {
        case ScriptCoordType.Relative:
          if (si.BlockOffset.HasValue)
          {
            p += si.BlockOffset.Value;
            break;
          }
          break;
        case ScriptCoordType.PlayerRelative:
          if (si.Actor == null)
            return false;
          p += si.MapTM.GetPoint(si.Actor.Position);
          break;
        case ScriptCoordType.ViewRelative:
          if (si.Actor == null)
            return false;
          GlobalPoint3D globalPoint3D1 = p;
          p = si.MapTM.GetPoint(si.Actor.EyePosition + (float) p.X * Vector3.Normalize(si.Actor.ViewDirection));
          p.Y += globalPoint3D1.Y;
          if (globalPoint3D1.Z != 0)
          {
            Vector3 vector3 = si.Actor.ViewMatrix.Right * (float) globalPoint3D1.Z;
            vector3.Z = -vector3.Z;
            p.X += (int) Math.Round((double) vector3.X);
            p.Y += (int) Math.Round((double) vector3.Y);
            p.Z += (int) Math.Round((double) vector3.Z);
            break;
          }
          break;
        case ScriptCoordType.ViewHorizRelative:
          if (si.Actor == null)
            return false;
          GlobalPoint3D globalPoint3D2 = p;
          p = si.MapTM.GetPoint(si.Actor.EyePosition + (float) p.X * si.Actor.ViewDirNoYNormalized);
          p.Y += globalPoint3D2.Y;
          if (globalPoint3D2.Z != 0)
          {
            Vector3 vector3 = si.Actor.ViewMatrix.Right * (float) globalPoint3D2.Z;
            vector3.Z = -vector3.Z;
            p.X += (int) Math.Round((double) vector3.X);
            p.Z += (int) Math.Round((double) vector3.Z);
            break;
          }
          break;
        case ScriptCoordType.TargetRelative:
          if (si.Target == null)
            return false;
          p += si.MapTM.GetPoint(si.Target.Position);
          break;
        case ScriptCoordType.TargetViewRelative:
          if (si.Target == null)
            return false;
          GlobalPoint3D globalPoint3D3 = p;
          p = si.MapTM.GetPoint(si.Target.EyePosition + (float) p.X * Vector3.Normalize(si.Target.ViewDirection));
          p.Y += globalPoint3D3.Y;
          if (globalPoint3D3.Z != 0)
          {
            Vector3 vector3 = si.Target.ViewMatrix.Right * (float) globalPoint3D3.Z;
            vector3.Z = -vector3.Z;
            p.X += (int) Math.Round((double) vector3.X);
            p.Y += (int) Math.Round((double) vector3.Y);
            p.Z += (int) Math.Round((double) vector3.Z);
            break;
          }
          break;
        case ScriptCoordType.KillerRelative:
          if (si.Killer == null)
            return false;
          p += si.MapTM.GetPoint(si.Killer.Position);
          break;
        case ScriptCoordType.KillerViewRelative:
          if (si.Killer == null)
            return false;
          GlobalPoint3D globalPoint3D4 = p;
          p = si.MapTM.GetPoint(si.Killer.EyePosition + (float) p.X * Vector3.Normalize(si.Killer.ViewDirection));
          p.Y += globalPoint3D4.Y;
          if (globalPoint3D4.Z != 0)
          {
            Vector3 vector3 = si.Killer.ViewMatrix.Right * (float) globalPoint3D4.Z;
            vector3.Z = -vector3.Z;
            p.X += (int) Math.Round((double) vector3.X);
            p.Y += (int) Math.Round((double) vector3.Y);
            p.Z += (int) Math.Round((double) vector3.Z);
            break;
          }
          break;
        case ScriptCoordType.CursorRelative:
          if (si.Actor == null || !si.Actor.SwingTargetIsValid)
            return false;
          p += si.Actor.SwingTarget;
          break;
        default:
          if (si.ScriptOffset.HasValue)
          {
            p += si.ScriptOffset.Value;
            break;
          }
          break;
      }
      return true;
    }

    private bool AdjustCoord(ScriptInstance si, ref Vector3 pos, ScriptCoordType type)
    {
      return this.AdjustCoord(si, ref pos, type, true);
    }

    private bool AdjustCoord(ScriptInstance si, Vector3 pos, ScriptCoordType type)
    {
        return this.AdjustCoord(si, ref pos, type, true);
    }

        private bool AdjustCoord(
      ScriptInstance si,
      ref Vector3 pos,
      ScriptCoordType type,
      bool checkCoords)
    {
      if (type == ScriptCoordType.None || !this.AdjustCoordCore(si, ref pos, type))
        return false;
      if (checkCoords)
      {
        GlobalPoint3D point = si.MapTM.GetPoint(pos);
        if (point.X < si.MapTM.MapBound.Min.X || point.X >= si.MapTM.MapBound.Max.X || (point.Y <= si.MapTM.MapBound.Min.Y || point.Y >= si.MapTM.MapBound.Max.Y) || (point.Z < si.MapTM.MapBound.Min.Z || point.Z >= si.MapTM.MapBound.Max.Z))
          return false;
      }
      return true;
    }

    private bool AdjustCoordCore(ScriptInstance si, ref Vector3 p, ScriptCoordType type)
    {
      switch (type)
      {
        case ScriptCoordType.Relative:
          if (si.BlockOffset.HasValue)
          {
            p += si.MapTM.GetBlockCenter(si.BlockOffset.Value);
            break;
          }
          break;
        case ScriptCoordType.PlayerRelative:
          if (si.Actor == null)
            return false;
          p += si.Actor.Position;
          break;
        case ScriptCoordType.ViewRelative:
          if (si.Actor == null)
            return false;
          Vector3 vector3_1 = p;
          p = si.Actor.EyePosition + p.X * Vector3.Normalize(si.Actor.ViewDirection);
          p.Y += vector3_1.Y;
          if ((double) vector3_1.Z != 0.0)
          {
            Vector3 right = si.Actor.ViewMatrix.Right;
            right.Y = 0.0f;
            right.Normalize();
            p.X += right.X * vector3_1.Z;
            p.Z += right.Z * -vector3_1.Z;
            break;
          }
          break;
        case ScriptCoordType.ViewHorizRelative:
          if (si.Actor == null)
            return false;
          Vector3 vector3_2 = p;
          p = si.Actor.EyePosition + p.X * si.Actor.ViewDirNoYNormalized;
          p.Y += vector3_2.Y;
          if ((double) vector3_2.Z != 0.0)
          {
            Vector3 right = si.Actor.ViewMatrix.Right;
            right.Y = 0.0f;
            right.Normalize();
            p.X += right.X * vector3_2.Z;
            p.Z += right.Z * -vector3_2.Z;
            break;
          }
          break;
        case ScriptCoordType.TargetRelative:
          if (si.Target == null)
            return false;
          p += si.Target.Position;
          break;
        case ScriptCoordType.TargetViewRelative:
          if (si.Target == null)
            return false;
          Vector3 vector3_3 = p;
          p = si.Target.EyePosition + p.X * Vector3.Normalize(si.Target.ViewDirection);
          p.Y += vector3_3.Y;
          if ((double) vector3_3.Z != 0.0)
          {
            Vector3 right = si.Target.ViewMatrix.Right;
            right.Y = 0.0f;
            right.Normalize();
            p.X += right.X * vector3_3.Z;
            p.Z += right.Z * -vector3_3.Z;
            break;
          }
          break;
        case ScriptCoordType.KillerRelative:
          if (si.Killer == null)
            return false;
          p += si.Killer.Position;
          break;
        case ScriptCoordType.KillerViewRelative:
          if (si.Killer == null)
            return false;
          Vector3 vector3_4 = p;
          p = si.Killer.EyePosition + p.X * Vector3.Normalize(si.Killer.ViewDirection);
          p.Y += vector3_4.Y;
          if ((double) vector3_4.Z != 0.0)
          {
            Vector3 right = si.Killer.ViewMatrix.Right;
            right.Y = 0.0f;
            right.Normalize();
            p.X += right.X * vector3_4.Z;
            p.Z += right.Z * -vector3_4.Z;
            break;
          }
          break;
        case ScriptCoordType.CursorRelative:
          if (si.Actor == null || !si.Actor.SwingTargetIsValid)
            return false;
          p += si.MapTM.GetBlockCenter(si.Actor.SwingTarget);
          break;
        default:
          if (si.ScriptOffset.HasValue)
          {
            p += si.MapTM.GetBlockCenter(si.ScriptOffset.Value);
            break;
          }
          break;
      }
      return true;
    }

    private int ReadInt32(ScriptInstance si, BinaryReader reader)
    {
      if (reader.ReadByte() != (byte) 0)
        return (int) si.GetVarValue(reader.ReadUInt16());
      return reader.ReadInt32();
    }

    private float ReadSingle(ScriptInstance si, BinaryReader reader)
    {
      if (reader.ReadByte() != (byte) 0)
        return (float) si.GetVarValue(reader.ReadUInt16());
      return reader.ReadSingle();
    }

    private double ReadDouble(ScriptInstance si, BinaryReader reader)
    {
      if (reader.ReadByte() != (byte) 0)
        return si.GetVarValue(reader.ReadUInt16());
      return reader.ReadDouble();
    }

    private Color? ReadColorNull(ScriptInstance si, BinaryReader reader)
    {
      if (!reader.ReadBoolean())
        return new Color?();
      return new Color?(this.ReadColor(si, reader));
    }

    private Color ReadColor(ScriptInstance si, BinaryReader reader)
    {
      return new Color(this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader), this.ReadInt32(si, reader));
    }

    private double ReadVarNumber(ScriptInstance si, BinaryReader reader)
    {
      switch (reader.ReadByte())
      {
        case 0:
          return reader.ReadDouble();
        case 1:
          return si.Vars[(int) reader.ReadUInt16()];
        case 2:
          string key1 = reader.ReadString();
          History history1 = si.Player != null ? si.Player.History : (History) null;
          return history1 != null ? (double) history1.GetHistory(key1) : 0.0;
        case 3:
          string key2 = reader.ReadString();
          History history2 = si.Instance.History;
          return history2 != null ? (double) history2.GetHistory(key2) : 0.0;
        case 4:
          string key3 = reader.ReadString();
          History history3 = si.Player != null ? si.Instance.GetClanHistory(si.Player.ClanName) : (History) null;
          return history3 != null ? (double) history3.GetHistory(key3) : 0.0;
        case 5:
          return (double) reader.ReadUInt32();
        default:
          return 0.0;
      }
    }

    private ScriptComparison ReadComparison(ScriptInstance si, BinaryReader reader)
    {
      ScriptComparison scriptComparison = new ScriptComparison();
      scriptComparison.Target = (ScriptTarget) reader.ReadByte();
      scriptComparison.Type = (Parser.CompareState) reader.ReadByte();
      if (scriptComparison.Type == Parser.CompareState.Binary)
      {
        scriptComparison.BoolResult = reader.ReadBoolean();
      }
      else
      {
        scriptComparison.CountTarget = (ScriptTarget) reader.ReadByte();
        if (scriptComparison.CountTarget == ScriptTarget.None)
          scriptComparison.Count = (ScriptInt32) this.ReadInt32(si, reader);
        else
          scriptComparison.CountKey = Globals2.SubstituteGeneral(si, reader.ReadString());
      }
      return scriptComparison;
    }

    private int GetCompareCount(ScriptInstance si, ScriptComparison compare)
    {
      if (compare.CountTarget == ScriptTarget.None)
        return compare.Count.I;
      if (compare.CountKey == null || compare.CountKey.Length == 0)
        return 0;
      History history = (History) null;
      switch (compare.CountTarget)
      {
        case ScriptTarget.System:
          history = si.Instance.History;
          break;
        case ScriptTarget.Actor:
          if (si.Player != null)
          {
            history = si.Player.History;
            break;
          }
          break;
        case ScriptTarget.Clan:
          if (si.Player != null)
          {
            history = si.Instance.GetClanHistory(si.Player.ClanName);
            break;
          }
          break;
      }
      if (history == null)
        return 0;
      return (int) history.GetHistory(compare.CountKey);
    }

    private delegate void ScriptCommandFunction(ScriptInstance instance, BinaryReader reader);

    private delegate void VarLoadFunction(
      ScriptInstance si,
      BinaryReader reader,
      out double valnum);

    private struct NumInputState
    {
      public ScriptInstance si;
      public ushort VarIndex;
      public bool Transmit;
    }
  }
}
