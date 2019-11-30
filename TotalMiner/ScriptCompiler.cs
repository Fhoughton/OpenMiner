// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptCompiler
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class ScriptCompiler : Parser
  {
    private static string[] invalidVarNames = new string[11]
    {
      "var",
      "true",
      "false",
      "on",
      "off",
      "player",
      "actor",
      "clan",
      "history",
      "syshistory",
      "clanhistory"
    };
    private static string invalidVarChars = " :~`!@#$%^&*()-=+|\\/?<>,.";
    public const int MaxVars = 256;
    private static string[] copyTypes;
    private static string[] npcTypes;
    private ScriptCompiler.TestData testData;
    private GameInstance instance;
    private bool IsTest;
    private long lastCommandPC;
    private int errorVerbosity;
    private List<ScriptMenuParam> menuParams;
    private int varCount;
    private string[] varNames;

    public ScriptCompiler(GameInstance instance)
    {
      this.instance = instance;
      this.testData = new ScriptCompiler.TestData();
    }

    public void CompileScript(Script script)
    {
      if (script.ByteCode != null)
        script.ByteCodeWriter.Close();
      this.varCount = 0;
      this.lastCommandPC = 0L;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          for (int cmdIndex = 0; cmdIndex < script.Commands.Count; ++cmdIndex)
            this.CompileCommand(writer, script, cmdIndex);
          if (writer.BaseStream.Position > 0L)
            this.WriteNextCommandOffset(writer);
          writer.Flush();
          script.ByteCode = new MemoryStream();
          script.ByteCodeWriter = new BinaryWriter((Stream) script.ByteCode);
          this.WriteHeader(script);
          script.ByteCodeWriter.Write(memoryStream.ToArray());
        }
      }
      script.IsChanged = false;
    }

    private void WriteHeader(Script script)
    {
      script.ByteCodeWriter.Write((ushort) this.varCount);
      if (this.varCount <= 0)
        return;
      string[] strArray = new string[this.varCount];
      Array.Copy((Array) this.varNames, (Array) strArray, this.varCount);
      script.VarNames = strArray;
    }

    private void CompileCommand(BinaryWriter writer, Script script, int cmdIndex)
    {
      string command = script.Commands[cmdIndex];
      if (command == null || command.Length < 1 || command.Length > 1 && command[0] == '/' && command[1] == '/')
        return;
      string str = command;
      string lower = command.ToLower();
      Parser.Token nextToken = this.GetNextToken(lower, 0, char.MinValue, ' ');
      switch (nextToken.Lexeme)
      {
        case "if":
          break;
        case "then":
          break;
        case "blueprint":
          this.CompileCommandBlueprint(writer, lower, nextToken.EndIndex + 1);
          break;
        case "canequip":
          this.CompileCommandCanEquip(writer, lower, nextToken.EndIndex + 1);
          break;
        case "cavein":
          this.CompileCommandCaveIn(writer, lower, nextToken.EndIndex + 1);
          break;
        case "cctv":
          this.CompileCommandCCTV(writer, lower, nextToken.EndIndex + 1);
          break;
        case "clan":
          this.CompileCommandClan(writer, lower, nextToken.EndIndex + 1);
          break;
        case "commit":
          this.CompileCommandCommit(writer, lower, nextToken.EndIndex + 1);
          break;
        case "context":
          this.CompileCommandContext(writer, lower, nextToken.EndIndex + 1);
          break;
        case "copyblock":
          this.CompileCommandCopyBlock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "copyregion":
          this.CompileCommandCopyRegion(writer, lower, nextToken.EndIndex + 1);
          break;
        case "else":
          this.CompileCommandElse(writer, lower, nextToken.EndIndex + 1);
          break;
        case "elseif":
          this.CompileCommandElseif(writer, lower, nextToken.EndIndex + 1);
          break;
        case "endif":
          this.CompileCommandEndif(writer, lower, nextToken.EndIndex + 1);
          break;
        case "equip":
          this.CompileCommandEquip(writer, lower, nextToken.EndIndex + 1);
          break;
        case "exit":
          this.CompileCommandExit(writer, lower, nextToken.EndIndex + 1);
          break;
        case "explosion":
          this.CompileCommandExplosion(writer, lower, nextToken.EndIndex + 1);
          break;
        case "fog":
          this.CompileCommandFog(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hail":
          this.CompileCommandHail(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasaction":
          this.CompileCommandHasAction(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hashistory":
          this.CompileCommandHasHistory(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasinventory":
          this.CompileCommandHasInventory(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasmarker":
          this.CompileCommandHasMarker(writer, lower, nextToken.EndIndex + 1);
          break;
        case "haspermission":
          this.CompileCommandHasPermission(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasactor":
        case "hasplayer":
          this.CompileCommandHasActor(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasskill":
          this.CompileCommandHasSkill(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hasstatbonus":
          this.CompileCommandHasStatBonus(writer, lower, nextToken.EndIndex + 1);
          break;
        case "health":
          this.CompileCommandHealth(writer, lower, nextToken.EndIndex + 1);
          break;
        case "history":
          this.CompileCommandHistory(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hudbar":
          this.CompileCommandHUDBar(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "hudcounter":
          this.CompileCommandHUDCounter(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "hudshape":
          this.CompileCommandHUDShape(writer, lower, nextToken.EndIndex + 1);
          break;
        case "hudtext":
          this.CompileCommandHUDText(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "input":
          this.CompileCommandInput(writer, lower, nextToken.EndIndex + 1);
          break;
        case "intersect":
          this.CompileCommandIntersect(writer, lower, nextToken.EndIndex + 1);
          break;
        case "inventory":
          this.CompileCommandInventory(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isavatar":
          this.CompileCommandIsAvatar(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblock":
          this.CompileCommandIsBlock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockdeliveringpower":
          this.CompileCommandIsBlockDeliveringPower(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockedited":
          this.CompileCommandIsBlockEdited(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblocklightsource":
          this.CompileCommandIsBlockLightSource(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockopen":
          this.CompileCommandIsBlockOpen(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockore":
          this.CompileCommandIsBlockOre(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockpassable":
          this.CompileCommandIsBlockPassable(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockreceivingpower":
          this.CompileCommandIsBlockReceivingPower(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblockresistance":
          this.CompileCommandIsBlockResistance(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblocksolid":
          this.CompileCommandIsBlockSolid(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblocktexture":
          this.CompileCommandIsBlockTexture(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isclan":
          this.CompileCommandIsClan(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isclock":
          this.CompileCommandIsClock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "iscombat":
          this.CompileCommandIsCombat(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isdaytime":
          this.CompileCommandIsDayTime(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isdistance":
          this.CompileCommandIsDistance(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isequipped":
          this.CompileCommandIsEquipped(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isfiniteresources":
          this.CompileCommandIsFiniteResources(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isgamercount":
          this.CompileCommandIsGamerCount(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isinzone":
          this.CompileCommandIsInZone(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isblocklit":
        case "islit":
        case "islight":
          this.CompileCommandIsLight(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isnpccount":
        case "ismobcount":
          this.CompileCommandIsNpcCount(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isnameplate":
          this.CompileCommandIsNameplate(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isnighttime":
          this.CompileCommandIsNightTime(writer, lower, nextToken.EndIndex + 1);
          break;
        case "israndom":
          this.CompileCommandIsRandom(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isskills":
          this.CompileCommandIsSkills(writer, lower, nextToken.EndIndex + 1);
          break;
        case "istime":
          this.CompileCommandIsTime(writer, lower, nextToken.EndIndex + 1);
          break;
        case "isvar":
          this.CompileCommandIsVar(writer, lower, nextToken.EndIndex + 1);
          break;
        case "item":
          this.CompileCommandItem(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "kick":
          this.CompileCommandKick(writer, lower, nextToken.EndIndex + 1);
          break;
        case "loop":
          this.CompileCommandLoop(writer, lower, nextToken.EndIndex + 1);
          break;
        case "marker":
          this.CompileCommandMarker(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "menu":
          this.CompileCommandMenu(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "messagebox":
          this.CompileCommandMessageBox(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "npchealth":
        case "mobhealth":
          this.CompileCommandNpcHealth(writer, lower, nextToken.EndIndex + 1);
          break;
        case "npcspawn":
        case "spawnmob":
        case "mobspawn":
          this.CompileCommandNpcSpawn(writer, lower, nextToken.EndIndex + 1);
          break;
        case "npcstate":
        case "mobstate":
          this.CompileCommandNpcState(writer, lower, nextToken.EndIndex + 1);
          break;
        case "moveblock":
          this.CompileCommandMoveBlock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "moveregion":
          this.CompileCommandMoveRegion(writer, lower, nextToken.EndIndex + 1);
          break;
        case "nop":
          break;
        case "notify":
          this.CompileCommandNotify(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "openblock":
          this.CompileCommandOpenBlock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "particle":
          this.CompileCommandParticle(writer, lower, nextToken.EndIndex + 1);
          break;
        case "particleemitter":
          this.CompileCommandParticleEmitter(writer, lower, nextToken.EndIndex + 1);
          break;
        case "paste":
          this.CompileCommandPaste(writer, lower, nextToken.EndIndex + 1);
          break;
        case "permission":
          this.CompileCommandPermission(writer, lower, nextToken.EndIndex + 1);
          break;
        case "pickup":
        case "addpickup":
          this.CompileCommandPickup(writer, lower, nextToken.EndIndex + 1);
          break;
        case "replaceregion":
          this.CompileCommandReplaceRegion(writer, lower, nextToken.EndIndex + 1);
          break;
        case "rain":
          this.CompileCommandRain(writer, lower, nextToken.EndIndex + 1);
          break;
        case "random":
          this.CompileCommandRandom(writer, lower, nextToken.EndIndex + 1);
          break;
        case nameof (script):
          this.CompileCommandScript(writer, lower, nextToken.EndIndex + 1);
          break;
        case "addblock":
        case "clearblock":
        case "setblock":
          this.CompileCommandSetBlock(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setblockscript":
          this.CompileCommandSetBlockScript(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "seteventscript":
          this.CompileCommandSetEventScript(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "setnameplate":
          this.CompileCommandSetNameplate(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setpower":
          this.CompileCommandSetPower(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setreach":
          this.CompileCommandSetReach(writer, lower, nextToken.EndIndex + 1);
          break;
        case "clearregion":
        case "fillregion":
        case "setregion":
          this.CompileCommandSetRegion(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setregionaux":
          this.CompileCommandSetRegionAux(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setsphere":
          this.CompileCommandSetSphere(writer, lower, nextToken.EndIndex + 1);
          break;
        case "setswitch":
          this.CompileCommandSetSwitch(writer, lower, nextToken.EndIndex + 1);
          break;
        case "settext":
          this.CompileCommandSetText(writer, lower, str, nextToken.EndIndex + 1);
          break;
        case "texture":
        case "settexture":
          this.CompileCommandSetTexture(writer, lower, nextToken.EndIndex + 1);
          break;
        case "skill":
          this.CompileCommandSkill(writer, lower, nextToken.EndIndex + 1);
          break;
        case "skillxp":
          this.CompileCommandSkillXP(writer, lower, nextToken.EndIndex + 1);
          break;
        case "skycolor":
          this.CompileCommandSkyColor(writer, lower, nextToken.EndIndex + 1);
          break;
        case "sound":
          this.CompileCommandSound(writer, lower, nextToken.EndIndex + 1);
          break;
        case "teleport":
          this.CompileCommandTeleport(writer, lower, nextToken.EndIndex + 1);
          break;
        case "tintcolor":
          this.CompileCommandTintColor(writer, lower, nextToken.EndIndex + 1);
          break;
        case "unequip":
          this.CompileCommandUnequip(writer, lower, nextToken.EndIndex + 1);
          break;
        case "var":
          this.CompileCommandVar(writer, lower, nextToken.EndIndex + 1);
          break;
        case "wait":
          this.CompileCommandWait(writer, lower, nextToken.EndIndex + 1);
          break;
        case "waypoint":
          this.CompileCommandWaypoint(writer, lower, nextToken.EndIndex + 1);
          break;
        case "zone":
          this.CompileCommandZone(writer, lower, str, nextToken.EndIndex + 1);
          break;
        default:
          if (!this.IsTest)
            break;
          this.LogTestError("Unknown command: " + nextToken.Lexeme);
          break;
      }
    }

    public List<string> TestScript(Script script, int verbosity)
    {
      this.errorVerbosity = verbosity;
      if (this.testData.TestResult == null)
        this.testData.TestResult = new List<string>();
      else
        this.testData.TestResult.Clear();
      this.testData.ScriptName = script.Name;
      this.testData.LineNo = 0;
      this.IsTest = true;
      try
      {
        this.TestCommandSyntax(script);
        this.TestForUnclosedIfBlock(script);
      }
      finally
      {
        this.IsTest = false;
      }
      return this.testData.TestResult;
    }

    private void TestCommandSyntax(Script script)
    {
      this.varCount = 0;
      for (int cmdIndex = 0; cmdIndex < script.Commands.Count; ++cmdIndex)
      {
        this.testData.LineNo = cmdIndex + 1;
        this.CompileCommand((BinaryWriter) null, script, cmdIndex);
      }
    }

    private void TestForUnclosedIfBlock(Script script)
    {
      List<string> stringList = new List<string>();
      stringList.Add("if");
      for (int index = 0; index < script.Commands.Count; ++index)
      {
        this.testData.LineNo = index + 1;
        string lower = script.Commands[index].Trim().ToLower();
        switch (lower)
        {
          case "if":
          case "then":
          case "elseif":
          case "else":
          case "endif":
            if (!stringList.Contains(lower))
            {
              this.LogTestError("Unexpected command: " + lower);
              break;
            }
            break;
        }
        switch (lower)
        {
          case "if":
            stringList.Remove("if");
            stringList.Add("then");
            break;
          case "then":
            stringList.Remove("then");
            stringList.Add("else");
            stringList.Add("elseif");
            stringList.Add("endif");
            break;
          case "elseif":
            stringList.Remove("elseif");
            stringList.Add("then");
            break;
          case "else":
            stringList.Remove("else");
            stringList.Add("endif");
            break;
          case "endif":
            stringList.Remove("else");
            stringList.Remove("elseif");
            stringList.Remove("then");
            stringList.Remove("endif");
            stringList.Add("if");
            break;
        }
      }
      if (!stringList.Contains("then"))
        return;
      this.LogTestError("If command does not have corresponding Then command");
    }

    private void CompileCommandBlueprint(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptBPItem? bpItem1 = this.ParseBPItem(nextToken1, 0);
      if (!bpItem1.HasValue)
        this.LogTestError("product not found");
      else if (Blueprints.GetBlueprint(bpItem1.Value.ItemID) == null)
      {
        this.LogTestError("this item cannot be crafted");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptSingle smeltTime = (ScriptSingle) Globals1.ItemData[(int) bpItem1.Value.ItemID].SmeltTime;
        bool flag = nextToken2.Lexeme.StartsWith("furnace");
        int indexStart;
        int length;
        if (flag)
        {
          this.GetNamedToken(nextToken2, "furnace", out indexStart, out length);
          if (indexStart >= 0 && length > 0)
          {
            ScriptSingle? floatFromToken2 = this.GetFloatFromToken2(nextToken2.Lexeme.Substring(indexStart, length));
            if (!floatFromToken2.HasValue)
              this.LogTestWarning("smelt time not recognized. default of 4.5 seconds used");
            else
              smeltTime = floatFromToken2.Value;
          }
        }
        if (flag || nextToken2.Lexeme == "craft")
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        SkillType? nullable1 = new SkillType?();
        ScriptInt32? nullable2 = new ScriptInt32?();
        List<ScriptBPItem> scriptBpItemList = (List<ScriptBPItem>) null;
        int lastID = 0;
        for (; nextToken2.StartIndex < nextToken2.EndIndex; nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1))
        {
          if (!nullable1.HasValue)
          {
            this.GetNamedToken(nextToken2, "skill", out indexStart, out length);
            if (indexStart >= 0 && length > 0)
            {
              nullable1 = this.GetSkillFromToken(nextToken2.Lexeme.Substring(indexStart, length));
              if (!nullable1.HasValue)
              {
                this.LogTestWarning("skill not recognized");
                continue;
              }
              continue;
            }
          }
          if (!nullable2.HasValue)
          {
            nullable2 = this.GetIntFromNamedToken2(nextToken2, "level");
            if (nullable2.HasValue)
              continue;
          }
          ScriptBPItem? bpItem2 = this.ParseBPItem(nextToken2, lastID);
          if (bpItem2.HasValue)
          {
            if (scriptBpItemList == null)
              scriptBpItemList = new List<ScriptBPItem>();
            scriptBpItemList.Add(bpItem2.Value);
            lastID = bpItem2.Value.ID;
          }
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Blueprint);
        writer.Write((ushort) bpItem1.Value.ItemID);
        this.WriteInt32(writer, bpItem1.Value.Count);
        writer.Write(flag);
        if (flag)
          this.WriteSingle(writer, smeltTime);
        writer.Write(nullable1.HasValue);
        if (nullable1.HasValue)
          writer.Write((byte) nullable1.Value);
        writer.Write(nullable2.HasValue);
        if (nullable2.HasValue)
          this.WriteInt32(writer, nullable2.Value);
        writer.Write(scriptBpItemList != null ? scriptBpItemList.Count : 0);
        if (scriptBpItemList == null)
          return;
        foreach (ScriptBPItem scriptBpItem in scriptBpItemList)
        {
          writer.Write((byte) scriptBpItem.ID);
          writer.Write((ushort) scriptBpItem.ItemID);
          this.WriteInt32(writer, scriptBpItem.Count);
          this.WriteInt32(writer, scriptBpItem.Durability);
        }
      }
    }

    private ScriptBPItem? ParseBPItem(Parser.Token token, int lastID)
    {
      ScriptBPItem scriptBpItem = new ScriptBPItem();
      int length = token.Lexeme.IndexOf(':');
      if (length < 0)
      {
        Item? itemIdFromToken = this.GetItemIDFromToken(token);
        if (!itemIdFromToken.HasValue)
          return new ScriptBPItem?();
        scriptBpItem.ID = lastID + 1;
        scriptBpItem.ItemID = itemIdFromToken.Value;
        scriptBpItem.Count = (ScriptInt32) 1;
        return new ScriptBPItem?(scriptBpItem);
      }
      int num1 = length;
      string token1 = token.Lexeme.Substring(0, length);
      int? nullable = this.GetIntFromToken(token1);
      if (nullable.HasValue)
      {
        if (nullable.Value < 1 || nullable.Value > 9)
          nullable = new int?(lastID + 1);
        num1 = token.Lexeme.IndexOf(':', length + 1);
        token1 = num1 < 0 ? token.Lexeme.Substring(length + 1) : token.Lexeme.Substring(length + 1, num1 - (length + 1));
      }
      else
        nullable = new int?(lastID + 1);
      Item? itemIdFromToken1 = this.GetItemIDFromToken(token1, false, true, true);
      if (!itemIdFromToken1.HasValue)
        return new ScriptBPItem?();
      scriptBpItem.ID = nullable.Value;
      scriptBpItem.ItemID = itemIdFromToken1.Value;
      scriptBpItem.Count = (ScriptInt32) 1;
      scriptBpItem.Durability = (ScriptInt32) 0;
      if (num1 >= 0)
      {
        int num2 = token.Lexeme.IndexOf(':', num1 + 1);
        ScriptInt32? intFromToken2_1 = this.GetIntFromToken2(num2 < 0 ? token.Lexeme.Substring(num1 + 1) : token.Lexeme.Substring(num1 + 1, num2 - (num1 + 1)));
        if (intFromToken2_1.HasValue)
        {
          scriptBpItem.Count = intFromToken2_1.Value;
          if (num2 >= 0)
          {
            ScriptInt32? intFromToken2_2 = this.GetIntFromToken2(token.Lexeme.Substring(num2 + 1));
            if (intFromToken2_2.HasValue)
              scriptBpItem.Durability = intFromToken2_2.Value;
          }
        }
      }
      return new ScriptBPItem?(scriptBpItem);
    }

    private void CompileCommandCanEquip(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.CanEquip);
        writer.Write((ushort) itemIdFromToken.Value);
        writer.Write(flag);
      }
    }

    private void CompileCommandCaveIn(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        ScriptInt32? nullable = this.GetIntFromToken2(this.GetNextToken(command, nextToken.EndIndex + 1));
        if (!nullable.HasValue)
          nullable = new ScriptInt32?((ScriptInt32) 0);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.CaveIn);
        this.WriteCoord(writer, coord, pointFromToken);
        this.WriteInt32(writer, nullable.Value);
      }
    }

    private void CompileCommandCCTV(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      bool flag1 = nextToken1.Lexeme == "admin";
      if (flag1)
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag2 = false;
        BlockFace? nullable = new BlockFace?();
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (pointFromToken2.HasValue)
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        else
        {
          flag2 = nextToken2.Lexeme == "track";
          nullable = flag2 ? new BlockFace?(BlockFace.Backward) : this.GetDirFromToken(nextToken2);
          if (!nullable.HasValue)
          {
            nullable = new BlockFace?(BlockFace.Backward);
            this.LogTestWarning("[direction] not found. default N (North) used");
          }
        }
        Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptInt32? intFromToken2_1 = this.GetIntFromToken2(nextToken3);
        Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
        ScriptInt32? intFromToken2_2 = this.GetIntFromToken2(nextToken4);
        ScriptInt32? intFromToken2_3 = this.GetIntFromToken2(this.GetNextToken(command, nextToken4.EndIndex + 1));
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.CCTV);
        writer.Write(flag1);
        this.WriteCoord(writer, coord1, pointFromToken1);
        this.WriteCoord(writer, coord2, pointFromToken2);
        if (coord2 == ScriptCoordType.None)
        {
          writer.Write(flag2);
          if (!flag2)
            writer.Write((byte) nullable.Value);
        }
        writer.Write(intFromToken2_1.HasValue);
        if (intFromToken2_1.HasValue)
          this.WriteInt32(writer, intFromToken2_1.Value);
        writer.Write(intFromToken2_2.HasValue);
        if (intFromToken2_2.HasValue)
          this.WriteInt32(writer, intFromToken2_2.Value);
        writer.Write(intFromToken2_3.HasValue);
        if (!intFromToken2_3.HasValue)
          return;
        this.WriteInt32(writer, intFromToken2_3.Value);
      }
    }

    private void CompileCommandClan(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      string lexeme = nextToken.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid clan name or [remove] parameter missing");
      }
      else
      {
        ScriptInt32? nullable = this.GetIntFromToken2(this.GetNextToken(command, nextToken.EndIndex + 1));
        if (!nullable.HasValue)
          nullable = new ScriptInt32?((ScriptInt32) ((int) byte.MaxValue));
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Clan);
        bool flag = lexeme == "remove";
        writer.Write(flag);
        if (flag)
          return;
        writer.Write(lexeme);
        this.WriteInt32(writer, nullable.Value);
      }
    }

    private void CompileCommandCommit(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Commit);
    }

    private void CompileCommandContext(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.Lexeme != "player" && nextToken1.Lexeme != "actor")
      {
        this.LogTestError("only [actor] context supported");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptContext? nullable = nextToken2.Lexeme.IsNotEmpty() ? this.GetContextFromToken(nextToken2) : new ScriptContext?(ScriptContext.PlayerDefault);
        if (!nullable.HasValue)
        {
          this.LogTestError("context not found. must be either [default], [target] or [killer]");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Context);
          writer.Write((byte) nullable.Value);
        }
      }
    }

    private void CompileCommandCopyBlock(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("source [x,y,z] not found");
      }
      else
      {
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(this.GetNextToken(command, nextToken.EndIndex + 1), out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("destination [x,y,z] not found");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.CopyBlock);
          this.WriteCoord(writer, coord1, pointFromToken1);
          this.WriteCoord(writer, coord2, pointFromToken2);
        }
      }
    }

    private void CompileCommandCopyRegion(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("max [x,y,z] not found");
        }
        else
        {
          ScriptCoordType coord3;
          ScriptPoint3D? pointFromToken3 = this.GetPointFromToken(this.GetNextToken(command, nextToken2.EndIndex + 1), out coord3);
          if (!pointFromToken3.HasValue || coord3 == ScriptCoordType.None)
          {
            this.LogTestError("destination [x,y,z] not found");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.CopyRegion);
            this.WriteCoord(writer, coord1, pointFromToken1);
            this.WriteCoord(writer, coord2, pointFromToken2);
            this.WriteCoord(writer, coord3, pointFromToken3);
          }
        }
      }
    }

    private void CompileCommandElse(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Else);
    }

    private void CompileCommandElseif(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Elseif);
    }

    private void CompileCommandEndif(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Endif);
    }

    private void CompileCommandEquip(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      InventoryHand inventoryHand = InventoryHand.None;
      if (nextToken.Lexeme == "left")
      {
        inventoryHand = InventoryHand.Left;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "right")
      {
        inventoryHand = InventoryHand.Right;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "body")
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Equip);
        writer.Write((byte) inventoryHand);
        writer.Write((ushort) itemIdFromToken.Value);
      }
    }

    private void CompileCommandExit(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Exit);
    }

    private void CompileCommandExplosion(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptInt32? nullable1 = this.GetIntFromToken2(nextToken2);
        if (!nullable1.HasValue)
          nullable1 = new ScriptInt32?((ScriptInt32) 0);
        ScriptInt32? nullable2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
        if (!nullable2.HasValue)
          nullable2 = nullable1;
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Explosion);
        this.WriteCoord(writer, coord, pointFromToken);
        this.WriteInt32(writer, nullable1.Value);
        this.WriteInt32(writer, nullable2.Value);
      }
    }

    private void CompileCommandFog(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptPoint3D? point2DfromToken = this.GetPoint2DFromToken(nextToken1, out coord);
      if (!point2DfromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,z] not found");
      }
      else
      {
        ScriptSingle? nullable1 = new ScriptSingle?();
        ScriptSingle? nullable2 = new ScriptSingle?();
        ScriptSingle? nullable3 = new ScriptSingle?();
        ScriptInt32? nullable4 = new ScriptInt32?();
        ScriptColor? c = new ScriptColor?();
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag = nextToken2.Lexeme == "delete";
        if (!flag)
        {
          nullable1 = this.GetFloatFromToken2(nextToken2);
          if (!nullable1.HasValue)
          {
            this.LogTestError("[radius] not found");
            return;
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable2 = this.GetFloatFromToken2(nextToken3);
          if (!nullable2.HasValue)
          {
            this.LogTestError("[duration] not found");
            return;
          }
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          nullable3 = this.GetFloatFromToken2(nextToken4);
          if (!nullable3.HasValue)
          {
            this.LogTestError("[intensity] not found");
            return;
          }
          Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          c = this.GetColor3FromToken2(nextToken5);
          if (c.HasValue)
          {
            nullable4 = this.GetIntFromToken2(this.GetNextToken(command, nextToken5.EndIndex + 1));
            if (!nullable4.HasValue)
              this.LogTestWarning("[visibility] not found");
          }
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Fog);
        this.WriteCoord2D(writer, coord, point2DfromToken);
        writer.Write(flag);
        if (flag)
          return;
        this.WriteSingle(writer, nullable1.Value);
        this.WriteSingle(writer, nullable2.Value);
        this.WriteSingle(writer, nullable3.Value);
        this.WriteColor(writer, c);
        writer.Write(nullable4.HasValue);
        if (!nullable4.HasValue)
          return;
        this.WriteInt32(writer, nullable4.Value);
      }
    }

    private void CompileCommandHail(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptPoint3D? point2DfromToken = this.GetPoint2DFromToken(nextToken1, out coord);
      if (!point2DfromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,z] not found");
      }
      else
      {
        ScriptSingle? nullable1 = new ScriptSingle?();
        ScriptSingle? nullable2 = new ScriptSingle?();
        ScriptSingle? nullable3 = new ScriptSingle?();
        ScriptColor? c = new ScriptColor?();
        ScriptVector2? v = new ScriptVector2?();
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag = nextToken2.Lexeme == "delete";
        if (!flag)
        {
          nullable1 = this.GetFloatFromToken2(nextToken2);
          if (!nullable1.HasValue)
          {
            this.LogTestError("[radius] not found");
            return;
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable2 = this.GetFloatFromToken2(nextToken3);
          if (!nullable2.HasValue)
          {
            this.LogTestError("[duration] not found");
            return;
          }
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          nullable3 = this.GetFloatFromToken2(nextToken4);
          if (!nullable3.HasValue)
          {
            this.LogTestError("[intensity] not found");
            return;
          }
          Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          c = this.GetColor4FromToken2(nextToken5);
          if (c.HasValue)
            v = this.GetVector2FromToken(this.GetNextToken(command, nextToken5.EndIndex + 1));
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Hail);
        this.WriteCoord2D(writer, coord, point2DfromToken);
        writer.Write(flag);
        if (flag)
          return;
        this.WriteSingle(writer, nullable1.Value);
        this.WriteSingle(writer, nullable2.Value);
        this.WriteSingle(writer, nullable3.Value);
        this.WriteColor(writer, c);
        this.WriteVector2(writer, v);
      }
    }

    private void CompileCommandHasAction(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken1);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ItemAction? actionFromToken = this.GetActionFromToken(nextToken2);
        if (!actionFromToken.HasValue)
        {
          this.LogTestError("action not found. possible values are [mined|used|cratfted|collected]");
        }
        else
        {
          ScriptComparison comparison = this.ParseComparison(command, nextToken2.EndIndex + 1);
          if (this.IsTest || comparison.Type == Parser.CompareState.None)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.HasAction);
          writer.Write((byte) actionFromToken.Value);
          writer.Write((ushort) itemIdFromToken.Value);
          this.WriteComparison(writer, comparison);
        }
      }
    }

    private void CompileCommandHasHistory(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      string lexeme = nextToken.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid history name");
      }
      else
      {
        ScriptComparison targetedComparison = this.ParseTargetedComparison(command, nextToken.EndIndex + 1);
        if (targetedComparison.Type == Parser.CompareState.Binary)
        {
          targetedComparison.Count = (ScriptInt32) 0;
          targetedComparison.Type = targetedComparison.BoolResult ? Parser.CompareState.NotEqual : Parser.CompareState.Equal;
        }
        if (this.IsTest || targetedComparison.Type == Parser.CompareState.None)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HasHistory);
        writer.Write(lexeme);
        this.WriteComparison(writer, targetedComparison);
      }
    }

    private void CompileCommandHasInventory(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptPoint3D? p = new ScriptPoint3D?();
      ScriptCoordType coord = ScriptCoordType.None;
      if (nextToken1.Lexeme != "player" && nextToken1.Lexeme != "actor")
      {
        p = this.GetPointFromToken(nextToken1, out coord);
        if (!p.HasValue)
        {
          this.LogTestError("[x,y,z] not found");
          return;
        }
      }
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken2);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        ScriptComparison comparison = this.ParseComparison(command, nextToken2.EndIndex + 1);
        if (this.IsTest || comparison.Type == Parser.CompareState.None)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HasInventory);
        this.WriteCoord(writer, coord, p);
        writer.Write((ushort) itemIdFromToken.Value);
        this.WriteComparison(writer, comparison);
      }
    }

    private void CompileCommandHasMarker(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      string lexeme = nextToken.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid marker name");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HasMarker);
        writer.Write(lexeme);
        writer.Write(flag);
      }
    }

    private void CompileCommandHasPermission(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      Permissions? permissionFromToken = this.GetPermissionFromToken(nextToken, true);
      if (!permissionFromToken.HasValue)
      {
        this.LogTestError("permission not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HasPermission);
        writer.Write((ushort) permissionFromToken.Value);
        writer.Write(flag);
      }
    }

    private void CompileCommandHasActor(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptPlayerProperty scriptPlayerProperty = nextToken1.Lexeme == "health" ? ScriptPlayerProperty.Health : (nextToken1.Lexeme == "oxygen" ? ScriptPlayerProperty.Oxygen : ScriptPlayerProperty.None);
      if (scriptPlayerProperty == ScriptPlayerProperty.None)
      {
        this.LogTestError("property not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found.");
        }
        else
        {
          bool isPercent;
          ScriptInt32? percentFromToken2 = this.GetIntOrPercentFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1), out isPercent);
          if (!percentFromToken2.HasValue)
            this.LogTestError("qty not found");
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.HasActor);
          writer.Write((byte) scriptPlayerProperty);
          writer.Write((byte) compareFromToken.Value);
          this.WriteInt32(writer, percentFromToken2.Value);
          writer.Write(isPercent);
        }
      }
    }

    private void CompileCommandHasSkill(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      SkillType? skillFromToken = this.GetSkillFromToken(nextToken1);
      if (!skillFromToken.HasValue)
      {
        this.LogTestError("skill not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found.");
        }
        else
        {
          ScriptInt32? nullable = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (!nullable.HasValue)
            nullable = new ScriptInt32?((ScriptInt32) 1);
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.HasSkill);
          writer.Write((byte) skillFromToken.Value);
          writer.Write((byte) compareFromToken.Value);
          this.WriteInt32(writer, nullable.Value);
        }
      }
    }

    private void CompileCommandHasStatBonus(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      SkillType? skillFromToken = this.GetSkillFromToken(nextToken1);
      if (!skillFromToken.HasValue)
      {
        this.LogTestError("skill not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found");
        }
        else
        {
          ScriptInt32? intFromToken2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (!intFromToken2.HasValue)
          {
            this.LogTestError("count not found");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.HasStatBonus);
            writer.Write((byte) skillFromToken.Value);
            writer.Write((byte) compareFromToken.Value);
            this.WriteInt32(writer, intFromToken2.Value);
          }
        }
      }
    }

    private void CompileCommandHealth(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.Lexeme == "delete")
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Health);
        writer.Write(true);
        this.WriteString(writer, nextToken2.Lexeme);
      }
      else
      {
        string s1 = (string) null;
        ScriptInt32? nullable1 = new ScriptInt32?((ScriptInt32) 0);
        string s2 = (string) null;
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken1);
        if (!intFromToken2.HasValue || intFromToken2.Value.IsZero)
        {
          this.LogTestError("invalid qty value");
        }
        else
        {
          Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          ScriptInt32? nullable2 = this.GetIntFromToken2(nextToken2);
          if (!nullable2.HasValue)
          {
            nullable2 = new ScriptInt32?((ScriptInt32) 0);
          }
          else
          {
            Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            int indexStart;
            int length;
            this.GetNamedToken(nextToken3, "name", out indexStart, out length);
            if (indexStart >= 0)
            {
              s2 = nextToken3.Lexeme.Substring(indexStart, length);
            }
            else
            {
              nullable1 = this.GetIntFromToken2(nextToken3);
              if (!nullable1.HasValue)
                s1 = nextToken3.Lexeme.IsEmpty() ? (string) null : nextToken3.Lexeme;
              nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
              if (indexStart >= 0)
                s2 = nextToken3.Lexeme.Substring(indexStart, length);
            }
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Health);
          writer.Write(false);
          this.WriteString(writer, s2);
          this.WriteInt32(writer, intFromToken2.Value);
          this.WriteInt32(writer, nullable2.Value);
          if (this.WriteString(writer, s1))
            return;
          writer.Write(nullable1.HasValue);
          if (!nullable1.HasValue)
            return;
          this.WriteInt32(writer, nullable1.Value);
        }
      }
    }

    private void CompileCommandHistory(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid history name");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme == "decrease";
        bool flag2 = nextToken2.Lexeme == "clear";
        if (flag1 || flag2)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        bool flag3 = nextToken2.Lexeme == "player";
        bool flag4 = !flag3 && nextToken2.Lexeme == "clan";
        if (flag3 || flag4)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        bool flag5 = nextToken2.Lexeme == "delete";
        ScriptNumType scriptNumType = ScriptNumType.Abs;
        ScriptVarNumber? n = new ScriptVarNumber?();
        if (!flag5 && !flag2 && !flag1)
        {
          if (nextToken2.Lexeme.Length > 0)
          {
            if (nextToken2.Lexeme[0] == '+')
              scriptNumType = ScriptNumType.Inc;
            else if (nextToken2.Lexeme[0] == '-')
              scriptNumType = ScriptNumType.Dec;
            if (scriptNumType != ScriptNumType.Abs)
              nextToken2.Lexeme = nextToken2.Lexeme.Substring(1, nextToken2.Lexeme.Length - 1);
          }
          n = this.ParseVarToken(nextToken2);
          if (!n.HasValue)
          {
            n = new ScriptVarNumber?(new ScriptVarNumber()
            {
              Type = ScriptValueType.NumLiterial,
              Value = 1.0
            });
            scriptNumType = ScriptNumType.Inc;
          }
        }
        else if (flag1)
        {
          scriptNumType = ScriptNumType.Dec;
          n = new ScriptVarNumber?(new ScriptVarNumber()
          {
            Type = ScriptValueType.NumLiterial,
            Value = 1.0
          });
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.History);
        writer.Write(lexeme);
        writer.Write(flag4 ? (byte) 3 : (flag3 ? (byte) 2 : (byte) 1));
        writer.Write(flag5 || flag2);
        if (flag5 || flag2)
          return;
        writer.Write((byte) scriptNumType);
        this.WriteVarNumber(writer, n);
      }
    }

    private void CompileCommandHUDBar(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(originalCaseCommand, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid name");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme == "player";
        if (flag1)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptInt32? nullable1 = new ScriptInt32?();
        ScriptRectangle? nullable2 = new ScriptRectangle?();
        ScriptSingle? nullable3 = new ScriptSingle?();
        ScriptColor? c = new ScriptColor?();
        HUDElementProps hudElementProps = HUDElementProps.None;
        ScriptComparison result = new ScriptComparison();
        bool flag2 = nextToken2.Lexeme == "delete";
        if (!flag2)
        {
          this.GetHistoryRefToken(nextToken2, ref result);
          if (result.CountTarget != ScriptTarget.System && result.CountTarget != ScriptTarget.Actor)
          {
            this.LogTestError("[history] not recognized");
            return;
          }
          if (result.CountKey.IsEmpty())
          {
            this.LogTestError("[history] not recognized");
            return;
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable1 = this.GetIntFromToken2(nextToken3);
          if (!nullable1.HasValue)
          {
            this.LogTestError("[maxvalue] not found");
            return;
          }
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          nullable2 = this.GetRectFromToken2(nextToken4);
          if (!nullable2.HasValue)
          {
            this.LogTestError("[x,y,w,h] not recognized.");
            return;
          }
          Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          nullable3 = this.GetFloatFromToken2(nextToken5);
          if (!nullable3.HasValue)
          {
            this.LogTestWarning("[scale] not recognized. 1 used.");
            nullable3 = new ScriptSingle?((ScriptSingle) 1f);
          }
          else
            nextToken5 = this.GetNextToken(command, nextToken5.EndIndex + 1);
          c = this.GetColor4FromToken2(nextToken5);
          if (!c.HasValue)
            this.LogTestWarning("[color] not recognized. White used.");
          else
            nextToken5 = this.GetNextToken(command, nextToken5.EndIndex + 1);
          if (nextToken5.Lexeme.Contains("vertical"))
            hudElementProps |= HUDElementProps.Vertical;
          if (nextToken5.Lexeme.Contains("numbers"))
            hudElementProps |= HUDElementProps.ShowNumbers;
          if (nextToken5.Lexeme.Contains("label"))
            hudElementProps |= HUDElementProps.ShowLabel;
          if (nextToken5.Lexeme.Contains("right"))
            hudElementProps |= HUDElementProps.RightJustify;
          if (nextToken5.Lexeme.Contains("abs"))
            hudElementProps |= HUDElementProps.Absolute;
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HUDBar);
        writer.Write(lexeme);
        writer.Write(flag1 ? (byte) 2 : (byte) 1);
        writer.Write(flag2);
        if (flag2)
          return;
        writer.Write((byte) result.CountTarget);
        writer.Write(result.CountKey);
        this.WriteInt32(writer, nullable1.Value);
        this.WriteInt32(writer, nullable2.Value.X);
        this.WriteInt32(writer, nullable2.Value.Y);
        this.WriteInt32(writer, nullable2.Value.W);
        this.WriteInt32(writer, nullable2.Value.H);
        this.WriteSingle(writer, nullable3.Value);
        this.WriteColor(writer, c);
        writer.Write((byte) hudElementProps);
      }
    }

    private void CompileCommandHUDCounter(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(originalCaseCommand, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid name");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme == "player";
        if (flag1)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptVector2? nullable1 = new ScriptVector2?();
        ScriptSingle? nullable2 = new ScriptSingle?();
        ScriptColor? c = new ScriptColor?();
        HUDElementProps hudElementProps = HUDElementProps.None;
        ScriptComparison result = new ScriptComparison();
        bool flag2 = nextToken2.Lexeme == "delete";
        if (!flag2)
        {
          this.GetHistoryRefToken(nextToken2, ref result);
          if (result.CountTarget != ScriptTarget.System && result.CountTarget != ScriptTarget.Actor)
          {
            this.LogTestError("[history] not recognized");
            return;
          }
          if (result.CountKey.IsEmpty())
          {
            this.LogTestError("[history] not recognized");
            return;
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable1 = this.GetVector2FromToken(nextToken3);
          if (!nullable1.HasValue)
          {
            this.LogTestError("[x,y] not recognized.");
            return;
          }
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          nullable2 = this.GetFloatFromToken2(nextToken4);
          if (!nullable2.HasValue)
          {
            this.LogTestWarning("[scale] not recognized. 1 used.");
            nullable2 = new ScriptSingle?((ScriptSingle) 1f);
          }
          else
            nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          c = this.GetColor4FromToken2(nextToken4);
          if (!c.HasValue)
            this.LogTestWarning("[color] not recognized. White used.");
          else
            nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          if (nextToken4.Lexeme.Contains("label"))
            hudElementProps |= HUDElementProps.ShowLabel;
          if (nextToken4.Lexeme.Contains("abs"))
            hudElementProps |= HUDElementProps.Absolute;
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HUDCounter);
        writer.Write(lexeme);
        writer.Write(flag1 ? (byte) 2 : (byte) 1);
        writer.Write(flag2);
        if (flag2)
          return;
        writer.Write((byte) result.CountTarget);
        writer.Write(result.CountKey);
        this.WriteSingle(writer, nullable1.Value.X);
        this.WriteSingle(writer, nullable1.Value.Y);
        this.WriteSingle(writer, nullable2.Value);
        this.WriteColor(writer, c);
        writer.Write((byte) hudElementProps);
      }
    }

    private void CompileCommandHUDShape(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string name = nextToken1.Lexeme;
      if (name == null || name.Length < 1)
      {
        this.LogTestError("invalid name");
      }
      else
      {
        ScriptInt32? nullable1 = new ScriptInt32?();
        int length = name.IndexOf(':');
        if (length > 0)
        {
          name = nextToken1.Lexeme.Substring(0, length);
          nullable1 = this.GetIntFromNamedToken2(nextToken1, name);
        }
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme == "player";
        if (flag1)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptRectangle? nullable2 = new ScriptRectangle?();
        ScriptColor? c = new ScriptColor?();
        HUDElementProps hudElementProps = HUDElementProps.None;
        bool flag2 = nextToken2.Lexeme == "delete";
        if (!flag2)
        {
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable2 = this.GetRectFromToken2(nextToken3);
          if (!nullable2.HasValue)
          {
            this.LogTestError("[x,y,w,h] not recognized.");
            return;
          }
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          c = this.GetColor4FromToken2(nextToken4);
          if (!c.HasValue)
            this.LogTestWarning("[color] not recognized. White used.");
          else
            nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
          if (nextToken4.Lexeme.Contains("abs"))
            hudElementProps |= HUDElementProps.Absolute;
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.HUDShape);
        writer.Write(name);
        writer.Write(nullable1.HasValue);
        if (nullable1.HasValue)
          this.WriteInt32(writer, nullable1.Value);
        writer.Write(flag1 ? (byte) 2 : (byte) 1);
        writer.Write(flag2);
        if (flag2)
          return;
        this.WriteInt32(writer, nullable2.Value.X);
        this.WriteInt32(writer, nullable2.Value.Y);
        this.WriteInt32(writer, nullable2.Value.W);
        this.WriteInt32(writer, nullable2.Value.H);
        this.WriteColor(writer, c);
        writer.Write((byte) hudElementProps);
      }
    }

    private void CompileCommandHUDText(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string lexeme1 = nextToken1.Lexeme;
      if (lexeme1 == null || lexeme1.Length < 1)
      {
        this.LogTestError("invalid name");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(originalCaseCommand, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme.ToLower() == "player";
        if (flag1)
          nextToken2 = this.GetNextToken(originalCaseCommand, nextToken2.EndIndex + 1);
        string lexeme2 = nextToken2.Lexeme;
        if (lexeme2 == null || lexeme2.Length < 1)
        {
          this.LogTestError("invalid text");
        }
        else
        {
          ScriptVector2? nullable1 = new ScriptVector2?();
          ScriptColor? c = new ScriptColor?();
          ScriptSingle? nullable2 = new ScriptSingle?();
          ScriptSingle? nullable3 = new ScriptSingle?();
          HUDElementProps hudElementProps = HUDElementProps.None;
          bool flag2 = nextToken2.Lexeme.ToLower() == "delete";
          if (!flag2)
          {
            Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            nullable1 = this.GetVector2FromToken(nextToken3);
            if (!nullable1.HasValue)
            {
              this.LogTestError("[x,y] not recognized.");
              return;
            }
            Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            nullable2 = this.GetFloatFromToken2(nextToken4);
            if (!nullable2.HasValue)
            {
              this.LogTestWarning("[scale] not recognized. 1 used.");
              nullable2 = new ScriptSingle?((ScriptSingle) 1f);
            }
            else
              nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
            c = this.GetColor4FromToken2(nextToken4);
            if (!c.HasValue)
            {
              this.LogTestWarning("[color] not recognized. White used.");
              c = new ScriptColor?((ScriptColor) Color.White);
            }
            else
              nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
            nullable3 = this.GetFloatFromToken2(nextToken4);
            if (!nullable3.HasValue)
            {
              this.LogTestWarning("[rot] not recognized. 0 used.");
              nullable3 = new ScriptSingle?((ScriptSingle) 0.0f);
            }
            else
              nextToken4 = this.GetNextToken(command, nextToken4.EndIndex + 1);
            if (nextToken4.Lexeme.Contains("vertical"))
              hudElementProps |= HUDElementProps.Vertical;
            if (nextToken4.Lexeme.Contains("right"))
              hudElementProps |= HUDElementProps.RightJustify;
            if (nextToken4.Lexeme.Contains("abs"))
              hudElementProps |= HUDElementProps.Absolute;
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.HUDText);
          writer.Write(lexeme1);
          writer.Write(flag1 ? (byte) 2 : (byte) 1);
          writer.Write(flag2);
          if (flag2)
            return;
          writer.Write(lexeme2);
          this.WriteSingle(writer, nullable1.Value.X);
          this.WriteSingle(writer, nullable1.Value.Y);
          this.WriteColor(writer, c);
          writer.Write((byte) hudElementProps);
          this.WriteSingle(writer, nullable2.Value);
          this.WriteSingle(writer, nullable3.Value);
        }
      }
    }

    private void CompileCommandInput(BinaryWriter writer, string command, int index)
    {
      string lexeme = this.GetNextToken(command, index).Lexeme;
      if (lexeme == null || lexeme.Length < 1 || !this.IsValidVarName(lexeme))
      {
        this.LogTestError("invalid variable name");
      }
      else
      {
        int num = this.GetVarIndex(lexeme);
        if (num < 0)
        {
          num = this.AddVariable(lexeme);
          if (num < 0)
            return;
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Input);
        writer.Write((ushort) num);
      }
    }

    private void CompileCommandIntersect(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptShape? shapeFromToken = this.GetShapeFromToken(nextToken1);
      if (!shapeFromToken.HasValue)
      {
        this.LogTestError("invalid shape. must be either ray, box, sphere or frustum");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord1;
        ScriptVector3? vector3FromToken = this.GetVector3FromToken(nextToken2, out coord1);
        if (!vector3FromToken.HasValue || coord1 == ScriptCoordType.None)
        {
          this.LogTestError("[x,y,z] not found");
        }
        else
        {
          ScriptCoordType coord2 = ScriptCoordType.None;
          ScriptVector3? v = new ScriptVector3?();
          ScriptSingle? nullable1 = new ScriptSingle?();
          ScriptSingle? nullable2 = new ScriptSingle?();
          ScriptShape? nullable3 = shapeFromToken;
          if ((nullable3.GetValueOrDefault() != ScriptShape.Sphere ? 1 : (!nullable3.HasValue ? 1 : 0)) != 0)
          {
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            v = this.GetVector3FromToken(nextToken2, out coord2);
            if (!v.HasValue || coord2 == ScriptCoordType.None)
            {
              this.LogTestError("[x,y,z] not found");
              return;
            }
          }
          ScriptShape? nullable4 = shapeFromToken;
          if ((nullable4.GetValueOrDefault() != ScriptShape.Sphere ? 0 : (nullable4.HasValue ? 1 : 0)) != 0)
          {
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            nullable1 = this.GetFloatFromToken2(nextToken2);
            if (!nullable1.HasValue)
            {
              this.LogTestError("radius not found");
              return;
            }
            if (nullable1.Value.IsZero)
            {
              this.LogTestError("invalid radius. must be greater than zero");
              return;
            }
          }
          else
          {
            ScriptShape? nullable5 = shapeFromToken;
            if ((nullable5.GetValueOrDefault() != ScriptShape.Frustum ? 0 : (nullable5.HasValue ? 1 : 0)) != 0)
            {
              nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
              nullable2 = this.GetFloatFromToken2(nextToken2);
              if (!nullable2.HasValue)
              {
                this.LogTestError("field of view (fov) not found");
                return;
              }
            }
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          HitTargetOptions hitTargetOptions = HitTargetOptions.PlayersAndNpcs;
          bool flag = nextToken3.Lexeme == "display";
          if (!flag)
          {
            hitTargetOptions = HitTargetOptions.None;
            if (nextToken3.Lexeme.Contains("player"))
              hitTargetOptions |= HitTargetOptions.Players;
            if (nextToken3.Lexeme.Contains("npc") || nextToken3.Lexeme.Contains("mob"))
              hitTargetOptions |= HitTargetOptions.Npcs;
            if (hitTargetOptions == HitTargetOptions.None)
              hitTargetOptions = HitTargetOptions.PlayersAndNpcs;
            else
              flag = this.GetNextToken(command, nextToken3.EndIndex + 1).Lexeme == "display";
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Intersect);
          writer.Write((byte) shapeFromToken.Value);
          writer.Write((byte) hitTargetOptions);
          writer.Write(flag);
          this.WriteVector3(writer, coord1, vector3FromToken);
          switch (shapeFromToken.Value)
          {
            case ScriptShape.Ray:
            case ScriptShape.Box:
              this.WriteVector3(writer, coord2, v);
              break;
            case ScriptShape.Sphere:
              this.WriteSingle(writer, nullable1.Value);
              break;
            case ScriptShape.Frustum:
              this.WriteVector3(writer, coord2, v);
              this.WriteSingle(writer, nullable2.Value);
              break;
          }
        }
      }
    }

    private void CompileCommandInventory(BinaryWriter writer, string command, int index)
    {
      bool? isPlayer;
      ScriptInventoryCmdType cmdType;
      Item? itemID;
      ScriptInt32? qty;
      ScriptCoordType coord1;
      ScriptPoint3D? p1;
      ScriptCoordType coord2;
      ScriptPoint3D? p2;
      ScriptCoordType coord3;
      ScriptPoint3D? p3;
      bool playerAfterP;
      this.GetInventoryParams(command, index, out isPlayer, out cmdType, out itemID, out qty, out coord1, out p1, out coord2, out p2, out coord3, out p3, out playerAfterP);
      if (cmdType == ScriptInventoryCmdType.None)
      {
        this.LogTestError("No command type given [add, clear, etc]");
      }
      else
      {
        if (!this.instance.IsCreativeMode && (cmdType == ScriptInventoryCmdType.Add || cmdType == ScriptInventoryCmdType.Copy))
          this.LogTestWarning("Add or Copy options are only available in Creative worlds");
        if (!qty.HasValue && itemID.HasValue && (cmdType == ScriptInventoryCmdType.Add || cmdType == ScriptInventoryCmdType.Take || cmdType == ScriptInventoryCmdType.Move))
        {
          if (cmdType == ScriptInventoryCmdType.Add)
          {
            qty = new ScriptInt32?((ScriptInt32) 1);
            this.LogTestWarning("qty not defined. defaults to 1");
          }
          else
          {
            qty = new ScriptInt32?((ScriptInt32) int.MaxValue);
            this.LogTestWarning("qty not defined. defaults to all");
          }
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Inventory);
        writer.Write(isPlayer.HasValue && isPlayer.Value);
        writer.Write((byte) cmdType);
        writer.Write(itemID.HasValue ? (ushort) itemID.Value : (ushort) 0);
        if (itemID.HasValue && cmdType != ScriptInventoryCmdType.Clear)
          this.WriteInt32(writer, qty.Value);
        this.WriteCoord(writer, coord1, p1);
        if (coord1 == ScriptCoordType.None)
          return;
        writer.Write(playerAfterP);
        this.WriteCoord(writer, coord2, p2);
        if (coord2 == ScriptCoordType.None)
          return;
        this.WriteCoord(writer, coord3, p3);
      }
    }

    private void GetInventoryParams(
      string command,
      int index,
      out bool? isPlayer,
      out ScriptInventoryCmdType cmdType,
      out Item? itemID,
      out ScriptInt32? qty,
      out ScriptCoordType coord1,
      out ScriptPoint3D? p1,
      out ScriptCoordType coord2,
      out ScriptPoint3D? p2,
      out ScriptCoordType coord3,
      out ScriptPoint3D? p3,
      out bool playerAfterP)
    {
      isPlayer = new bool?();
      playerAfterP = false;
      cmdType = ScriptInventoryCmdType.None;
      itemID = new Item?();
      qty = new ScriptInt32?();
      coord1 = ScriptCoordType.None;
      coord2 = ScriptCoordType.None;
      coord3 = ScriptCoordType.None;
      p1 = new ScriptPoint3D?();
      p2 = new ScriptPoint3D?();
      p3 = new ScriptPoint3D?();
      for (Parser.Token nextToken = this.GetNextToken(command, index); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        switch (nextToken.Lexeme)
        {
          case "player":
          case "actor":
            if (!isPlayer.HasValue)
            {
              isPlayer = new bool?(true);
              playerAfterP = p1.HasValue;
              break;
            }
            break;
          case "add":
            cmdType = ScriptInventoryCmdType.Add;
            break;
          case "clear":
            cmdType = ScriptInventoryCmdType.Clear;
            break;
          case "take":
            cmdType = ScriptInventoryCmdType.Take;
            break;
          case "copy":
            cmdType = ScriptInventoryCmdType.Copy;
            break;
          case "move":
            cmdType = ScriptInventoryCmdType.Move;
            break;
          default:
            ScriptCoordType coord;
            ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
            if (pointFromToken.HasValue && coord != ScriptCoordType.None)
            {
              if (!p1.HasValue)
              {
                p1 = pointFromToken;
                coord1 = coord;
                break;
              }
              if (!p2.HasValue)
              {
                p2 = pointFromToken;
                coord2 = coord;
                break;
              }
              if (!p3.HasValue)
              {
                p3 = pointFromToken;
                coord3 = coord;
                break;
              }
              break;
            }
            qty = this.GetIntFromToken2(nextToken);
            if (!qty.HasValue)
            {
              Item? itemIdFromToken = this.GetItemIDFromToken(nextToken);
              if (itemIdFromToken.HasValue)
              {
                itemID = itemIdFromToken;
                break;
              }
              this.LogTestWarning("item not recognized: " + nextToken.Lexeme);
              break;
            }
            break;
        }
      }
    }

    private void CompileCommandIsAvatar(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ActorType? actorTypeFromToken = this.GetActorTypeFromToken(nextToken);
      if (!actorTypeFromToken.HasValue)
      {
        this.LogTestError("avatar not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsAvatar);
        writer.Write((byte) actorTypeFromToken.Value);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlock(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Item? itemIdFromToken = this.GetItemIDFromToken(nextToken2.Lexeme, false, true, false);
        if (!itemIdFromToken.HasValue)
        {
          this.LogTestError("block id not recognized");
        }
        else
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken2);
          if (intFromToken2.HasValue)
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          bool flag = nextToken2.Lexeme != "false";
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.IsBlock);
          this.WriteCoord(writer, coord, pointFromToken);
          writer.Write((byte) this.instance.ConvertItemIDToBlockID(itemIdFromToken.Value));
          writer.Write(intFromToken2.HasValue);
          if (intFromToken2.HasValue)
            this.WriteInt32(writer, intFromToken2.Value);
          writer.Write(flag);
        }
      }
    }

    private void CompileCommandIsBlockDeliveringPower(
      BinaryWriter writer,
      string command,
      int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockDeliveringPower);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockEdited(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockEdited);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockLightSource(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockLightSource);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockOpen(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockOpen);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockOre(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockOre);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockPassable(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockPassable);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockReceivingPower(
      BinaryWriter writer,
      string command,
      int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockReceivingPower);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockResistance(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found");
        }
        else
        {
          ScriptInt32? intFromToken2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (!intFromToken2.HasValue)
          {
            this.LogTestError("count not found");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.IsBlockResistance);
            this.WriteCoord(writer, coord, pointFromToken);
            writer.Write((byte) compareFromToken.Value);
            this.WriteInt32(writer, intFromToken2.Value);
          }
        }
      }
    }

    private void CompileCommandIsBlockSolid(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsBlockSolid);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsBlockTexture(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? nullable = this.GetCompareFromToken(nextToken2);
        if (nullable.HasValue)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken2);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("texture id not found");
        }
        else
        {
          bool flag = true;
          if (!nullable.HasValue)
          {
            flag = this.GetNextToken(command, nextToken2.EndIndex + 1).Lexeme != "false";
            nullable = new Parser.CompareState?(Parser.CompareState.Binary);
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.IsBlockTexture);
          this.WriteCoord(writer, coord, pointFromToken);
          writer.Write((byte) nullable.Value);
          writer.Write(flag);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandIsClan(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      string lexeme = nextToken.Lexeme;
      bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsClan);
      writer.Write(lexeme);
      writer.Write(flag);
    }

    private void CompileCommandIsClock(BinaryWriter writer, string command, int index)
    {
      ScriptComparison comparison = this.ParseComparison(command, 0);
      if (comparison.Type == Parser.CompareState.None)
        comparison.Type = Parser.CompareState.Binary;
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsClock);
      this.WriteComparison(writer, comparison);
    }

    private void CompileCommandIsCombat(BinaryWriter writer, string command, int index)
    {
      bool flag = this.GetNextToken(command, index).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsCombat);
      writer.Write(flag);
    }

    private void CompileCommandIsDayTime(BinaryWriter writer, string command, int index)
    {
      bool flag = this.GetNextToken(command, index).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsDayTime);
      writer.Write(flag);
    }

    private void CompileCommandIsDistance(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found.");
        }
        else
        {
          ScriptDouble? doubleFromToken2 = this.GetDoubleFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (!doubleFromToken2.HasValue)
          {
            this.LogTestError("invalid distance value");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.IsDistance);
            this.WriteCoord(writer, coord, pointFromToken);
            writer.Write((byte) compareFromToken.Value);
            this.WriteDouble(writer, doubleFromToken2.Value);
          }
        }
      }
    }

    private void CompileCommandIsEquipped(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      InventoryHand inventoryHand = InventoryHand.None;
      if (nextToken.Lexeme == "left")
      {
        inventoryHand = InventoryHand.Left;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "right")
      {
        inventoryHand = InventoryHand.Right;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "body")
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
        bool flag = nextToken.Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsEquipped);
        writer.Write((ushort) itemIdFromToken.Value);
        writer.Write((byte) inventoryHand);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsFiniteResources(BinaryWriter writer, string command, int index)
    {
      bool flag = this.GetNextToken(command, index).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsFiniteResources);
      writer.Write(flag);
    }

    private void CompileCommandIsGamerCount(BinaryWriter writer, string command, int index)
    {
      ScriptCoordType coord1 = ScriptCoordType.None;
      ScriptCoordType coord2 = ScriptCoordType.None;
      ScriptPoint3D? nullable1 = new ScriptPoint3D?();
      ScriptPoint3D? p = new ScriptPoint3D?();
      ScriptSingle? nullable2 = new ScriptSingle?((ScriptSingle) 0.0f);
      string s = (string) null;
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord1);
      if (pointFromToken.HasValue && coord1 != ScriptCoordType.None)
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        p = this.GetPointFromToken(nextToken2, out coord2);
        if (!p.HasValue || coord2 == ScriptCoordType.None)
        {
          nullable2 = this.GetFloatFromToken2(nextToken2);
          if (!nullable2.HasValue)
          {
            this.LogTestError("radius or second [x,y,z] not found");
            return;
          }
        }
        nextToken1 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      else if (nextToken1.Lexeme != "local" && nextToken1.Lexeme != "remote" && nextToken1.Lexeme != "all")
      {
        s = nextToken1.Lexeme;
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      }
      ScriptGamerTarget scriptGamerTarget = nextToken1.Lexeme == "local" ? ScriptGamerTarget.Local : (nextToken1.Lexeme == "remote" ? ScriptGamerTarget.Remote : ScriptGamerTarget.All);
      nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken1);
      if (!compareFromToken.HasValue)
      {
        this.LogTestError("comparison operator not found.");
      }
      else
      {
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken1);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("invalid count value");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.IsGamerCount);
          this.WriteCoord(writer, coord1, pointFromToken);
          if (coord1 != ScriptCoordType.None)
          {
            this.WriteCoord(writer, coord2, p);
            if (coord2 == ScriptCoordType.None)
              this.WriteSingle(writer, nullable2.Value);
          }
          else
            this.WriteString(writer, s);
          writer.Write((byte) scriptGamerTarget);
          writer.Write((byte) compareFromToken.Value);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandIsInZone(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ZoneType? zoneTypeFromToken = this.GetZoneTypeFromToken(nextToken);
      if (!zoneTypeFromToken.HasValue)
      {
        this.LogTestError("zone type not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme != "false";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsInZone);
        writer.Write((byte) zoneTypeFromToken.Value);
        writer.Write(flag);
      }
    }

    private void CompileCommandIsLight(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme.Contains("sun");
        bool flag2 = nextToken2.Lexeme.Contains("moon");
        bool flag3 = nextToken2.Lexeme.Contains("block");
        if (flag1 || flag2 || flag3)
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        else
        {
          int num;
          flag3 = (num = 1) != 0;
          flag2 = num != 0;
          flag1 = num != 0;
        }
        bool flag4 = nextToken2.Lexeme != "false";
        ScriptInt32? nullable1 = new ScriptInt32?((ScriptInt32) 0);
        Parser.CompareState? nullable2 = this.GetCompareFromToken(nextToken2);
        if (!nullable2.HasValue)
        {
          nullable2 = new Parser.CompareState?(Parser.CompareState.Binary);
        }
        else
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable1 = this.GetIntFromToken2(nextToken2);
          if (!nullable1.HasValue)
          {
            this.LogTestError("count not found");
            return;
          }
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsLight);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(flag1);
        writer.Write(flag2);
        writer.Write(flag3);
        writer.Write((byte) nullable2.Value);
        if (nullable2.Value == Parser.CompareState.Binary)
          writer.Write(flag4);
        else
          this.WriteInt32(writer, nullable1.Value);
      }
    }

    private void CompileCommandIsNpcCount(BinaryWriter writer, string command, int index)
    {
      ScriptCoordType coord1 = ScriptCoordType.None;
      ScriptCoordType coord2 = ScriptCoordType.None;
      ScriptPoint3D? nullable1 = new ScriptPoint3D?();
      ScriptPoint3D? p = new ScriptPoint3D?();
      ScriptSingle? nullable2 = new ScriptSingle?((ScriptSingle) 0.0f);
      string s = (string) null;
      ActorType? nullable3 = new ActorType?();
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord1);
      if (pointFromToken.HasValue && coord1 != ScriptCoordType.None)
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        p = this.GetPointFromToken(nextToken2, out coord2);
        if (!p.HasValue || coord2 == ScriptCoordType.None)
        {
          nullable2 = this.GetFloatFromToken2(nextToken2);
          if (!nullable2.HasValue)
          {
            this.LogTestError("radius or second [x,y,z] not found");
            return;
          }
        }
        nextToken1 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      else
      {
        ActorType? actorTypeFromToken = this.GetActorTypeFromToken(nextToken1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken1);
        if (!actorTypeFromToken.HasValue && !compareFromToken.HasValue)
        {
          s = nextToken1.Lexeme;
          nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        }
      }
      ActorType? actorTypeFromToken1 = this.GetActorTypeFromToken(nextToken1);
      if (actorTypeFromToken1.HasValue)
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      Parser.CompareState? compareFromToken1 = this.GetCompareFromToken(nextToken1);
      if (!compareFromToken1.HasValue)
      {
        this.LogTestError("comparison operator not found.");
      }
      else
      {
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken1);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("invalid count value");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.IsNpcCount);
          this.WriteCoord(writer, coord1, pointFromToken);
          if (coord1 != ScriptCoordType.None)
          {
            this.WriteCoord(writer, coord2, p);
            if (coord2 == ScriptCoordType.None)
              this.WriteSingle(writer, nullable2.Value);
          }
          else
            this.WriteString(writer, s);
          writer.Write(actorTypeFromToken1.HasValue ? (byte) actorTypeFromToken1.Value : (byte) 0);
          writer.Write((byte) compareFromToken1.Value);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandIsNameplate(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      NamePlateSetting namePlateSetting = NamePlateSetting.None;
      bool flag = false;
      switch (nextToken.Lexeme)
      {
        case "short":
          namePlateSetting = NamePlateSetting.Short;
          break;
        case "far":
          namePlateSetting = NamePlateSetting.Far;
          break;
        case "":
        case "true":
          flag = true;
          break;
        case "false":
          flag = false;
          break;
        default:
          this.LogTestError("[true|false|short|far] not recognized");
          return;
      }
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsNameplate);
      writer.Write((byte) namePlateSetting);
      writer.Write(flag);
    }

    private void CompileCommandIsNightTime(BinaryWriter writer, string command, int index)
    {
      bool flag = this.GetNextToken(command, index).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsNightTime);
      writer.Write(flag);
    }

    private void CompileCommandIsRandom(BinaryWriter writer, string command, int index)
    {
      ScriptPoint3D? point2DfromToken = this.GetPoint2DFromToken(this.GetNextToken(command, index));
      if (!point2DfromToken.HasValue)
      {
        this.LogTestError("chance parameters not found");
      }
      else
      {
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.IsRandom);
        this.WriteInt32(writer, point2DfromToken.Value.X);
        this.WriteInt32(writer, point2DfromToken.Value.Z);
      }
    }

    private void CompileCommandIsSkills(BinaryWriter writer, string command, int index)
    {
      bool flag = this.GetNextToken(command, index).Lexeme != "false";
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.IsSkills);
      writer.Write(flag);
    }

    private void CompileCommandIsTime(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      bool flag = false;
      if (nextToken.Lexeme == "player")
      {
        flag = true;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken);
      if (!compareFromToken.HasValue)
      {
        this.LogTestError("comparison operator not found.");
      }
      else
      {
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("invalid time value");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.IsTime);
          writer.Write(flag);
          writer.Write((byte) compareFromToken.Value);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandIsVar(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      int varIndex = this.GetVarIndex(nextToken1.Lexeme);
      if (varIndex < 0)
      {
        this.LogTestError("variable [" + nextToken1.Lexeme + "] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken2);
        if (!compareFromToken.HasValue)
        {
          this.LogTestError("comparison operator not found.");
        }
        else
        {
          ScriptDouble? doubleFromToken2 = this.GetDoubleFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (!doubleFromToken2.HasValue)
          {
            this.LogTestError("invalid count value");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.IsVar);
            writer.Write((ushort) varIndex);
            writer.Write((byte) compareFromToken.Value);
            this.WriteDouble(writer, doubleFromToken2.Value);
          }
        }
      }
    }

    private void CompileCommandItem(
      BinaryWriter writer,
      string command,
      string origCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken1.Lexeme, false, true, false);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("item not found");
      }
      else
      {
        bool? nullable1 = new bool?();
        Parser.Token nextToken2 = this.GetNextToken(origCommand, nextToken1.EndIndex + 1);
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        if (nextToken1.Lexeme == "enable")
          nullable1 = new bool?(true);
        else if (nextToken1.Lexeme == "disable")
          nullable1 = new bool?(false);
        if (nullable1.HasValue)
        {
          nextToken2 = this.GetNextToken(origCommand, nextToken1.EndIndex + 1);
          nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        }
        string s1 = (string) null;
        string s2 = (string) null;
        ScriptInt32? nullable2 = new ScriptInt32?();
        SkillType? nullable3 = new SkillType?();
        ScriptInt32? nullable4 = new ScriptInt32?();
        ScriptSingle? nullable5 = new ScriptSingle?();
        ScriptSingle? nullable6 = new ScriptSingle?();
        ScriptSingle? nullable7 = new ScriptSingle?();
        ScriptSingle? nullable8 = new ScriptSingle?();
        ScriptInt32? nullable9 = new ScriptInt32?();
        ScriptInt32? nullable10 = new ScriptInt32?();
        ScriptInt32? nullable11 = new ScriptInt32?();
        ScriptInt32? nullable12 = new ScriptInt32?();
        ScriptInt32? nullable13 = new ScriptInt32?();
        ScriptInt32? nullable14 = new ScriptInt32?();
        ScriptInt32? nullable15 = new ScriptInt32?();
        for (; nextToken1.StartIndex < nextToken1.EndIndex; nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1))
        {
          if (s1 == null)
          {
            int indexStart;
            int length;
            this.GetNamedToken(nextToken2, "name", out indexStart, out length);
            if (length > 0)
            {
              s1 = nextToken2.Lexeme.Substring(indexStart, length);
              goto label_48;
            }
          }
          if (s2 == null)
          {
            int indexStart;
            int length;
            this.GetNamedToken(nextToken2, "desc", out indexStart, out length);
            if (length > 0)
            {
              s2 = nextToken2.Lexeme.Substring(indexStart, length);
              goto label_48;
            }
          }
          if (nextToken1.Lexeme.StartsWith("skill="))
          {
            nullable3 = this.GetSkillFromToken(nextToken1.Lexeme.Substring(6).Trim());
            if (!nullable3.HasValue)
              this.LogTestWarning("skill not recognized");
          }
          else
          {
            if (!nullable4.HasValue)
            {
              nullable4 = this.GetIntFromNamedToken2(nextToken1, "level");
              if (nullable4.HasValue)
                goto label_48;
            }
            if (!nullable2.HasValue)
            {
              nullable2 = this.GetIntFromNamedToken2(nextToken1, "durability");
              if (nullable2.HasValue)
              {
                if (Globals1.ItemData[(int) itemIdFromToken.Value].Durability == (ushort) 0)
                {
                  this.LogTestWarning("cannot change durability for this item");
                  nullable2 = new ScriptInt32?();
                  goto label_48;
                }
                else
                  goto label_48;
              }
            }
            if (!nullable5.HasValue)
            {
              nullable5 = this.GetFloatFromNamedToken2(nextToken1, "damage");
              if (nullable5.HasValue)
                goto label_48;
            }
            if (!nullable6.HasValue)
            {
              nullable6 = this.GetFloatFromNamedToken2(nextToken1, "reach");
              if (nullable6.HasValue)
                goto label_48;
            }
            if (!nullable7.HasValue)
            {
              nullable7 = this.GetFloatFromNamedToken2(nextToken1, "speed");
              if (nullable7.HasValue)
                goto label_48;
            }
            if (!nullable8.HasValue)
            {
              nullable8 = this.GetFloatFromNamedToken2(nextToken1, "delay");
              if (nullable8.HasValue)
                goto label_48;
            }
            if (!nullable9.HasValue)
            {
              nullable9 = this.GetIntFromNamedToken2(nextToken1, "heal");
              if (nullable9.HasValue)
                goto label_48;
            }
            if (!nullable10.HasValue)
            {
              nullable10 = this.GetIntFromNamedToken2(nextToken1, "health");
              if (nullable10.HasValue)
                goto label_48;
            }
            if (!nullable11.HasValue)
            {
              nullable11 = this.GetIntFromNamedToken2(nextToken1, "strength");
              if (nullable11.HasValue)
                goto label_48;
            }
            if (!nullable12.HasValue)
            {
              nullable12 = this.GetIntFromNamedToken2(nextToken1, "attack");
              if (nullable12.HasValue)
                goto label_48;
            }
            if (!nullable13.HasValue)
            {
              nullable13 = this.GetIntFromNamedToken2(nextToken1, "defence");
              if (!nullable13.HasValue)
                nullable13 = this.GetIntFromNamedToken2(nextToken1, "defense");
              if (nullable13.HasValue)
                goto label_48;
            }
            if (!nullable14.HasValue)
            {
              nullable14 = this.GetIntFromNamedToken2(nextToken1, "ranged");
              if (nullable14.HasValue)
                goto label_48;
            }
            if (!nullable15.HasValue)
            {
              nullable15 = this.GetIntFromNamedToken2(nextToken1, "looting");
              int num = nullable15.HasValue ? 1 : 0;
            }
          }
label_48:
          nextToken2 = this.GetNextToken(origCommand, nextToken1.EndIndex + 1);
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Item);
        writer.Write((ushort) itemIdFromToken.Value);
        writer.Write(nullable1.HasValue);
        if (nullable1.HasValue)
          writer.Write(nullable1.Value);
        writer.Write(s1.IsNotEmpty());
        if (s1.IsNotEmpty())
          writer.Write(s1);
        writer.Write(s2.IsNotEmpty());
        if (s2.IsNotEmpty())
          writer.Write(s2);
        writer.Write(nullable2.HasValue);
        if (nullable2.HasValue)
          this.WriteInt32(writer, nullable2.Value);
        writer.Write(nullable3.HasValue);
        if (nullable3.HasValue)
          writer.Write((byte) nullable3.Value);
        writer.Write(nullable4.HasValue);
        if (nullable4.HasValue)
          this.WriteInt32(writer, nullable4.Value);
        writer.Write(nullable5.HasValue);
        if (nullable5.HasValue)
          this.WriteSingle(writer, nullable5.Value);
        writer.Write(nullable6.HasValue);
        if (nullable6.HasValue)
          this.WriteSingle(writer, nullable6.Value);
        writer.Write(nullable7.HasValue);
        if (nullable7.HasValue)
          this.WriteSingle(writer, nullable7.Value);
        writer.Write(nullable8.HasValue);
        if (nullable8.HasValue)
          this.WriteSingle(writer, nullable8.Value);
        writer.Write(nullable9.HasValue);
        if (nullable9.HasValue)
          this.WriteInt32(writer, nullable9.Value);
        writer.Write(nullable10.HasValue);
        if (nullable10.HasValue)
          this.WriteInt32(writer, nullable10.Value);
        writer.Write(nullable11.HasValue);
        if (nullable11.HasValue)
          this.WriteInt32(writer, nullable11.Value);
        writer.Write(nullable12.HasValue);
        if (nullable12.HasValue)
          this.WriteInt32(writer, nullable12.Value);
        writer.Write(nullable13.HasValue);
        if (nullable13.HasValue)
          this.WriteInt32(writer, nullable13.Value);
        writer.Write(nullable14.HasValue);
        if (nullable14.HasValue)
          this.WriteInt32(writer, nullable14.Value);
        writer.Write(nullable15.HasValue);
        if (!nullable15.HasValue)
          return;
        this.WriteInt32(writer, nullable15.Value);
      }
    }

    private void CompileCommandKick(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Kick);
    }

    private void CompileCommandLoop(BinaryWriter writer, string command, int index)
    {
      ScriptInt32? nullable = this.GetIntFromToken2(this.GetNextToken(command, index));
      if (!nullable.HasValue || nullable.Value.IsZero)
      {
        nullable = new ScriptInt32?(new ScriptInt32()
        {
          I = 16
        });
        this.LogTestWarning("[millisecs] not found. default 16 used");
      }
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Loop);
      this.WriteInt32(writer, nullable.Value);
    }

    private void CompileCommandMarker(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(originalCaseCommand, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("invalid marker name");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag1 = nextToken2.Lexeme == "delete";
        ScriptCoordType coord = ScriptCoordType.None;
        ScriptPoint3D? p = new ScriptPoint3D?();
        bool flag2 = false;
        if (!flag1)
        {
          p = this.GetPoint2DFromToken(nextToken2, out coord);
          if (!p.HasValue || coord == ScriptCoordType.None)
          {
            this.LogTestError("[x,y,z] not found");
            return;
          }
          flag2 = this.GetNextToken(command, nextToken2.EndIndex + 1).Lexeme == "admin";
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Marker);
        writer.Write(lexeme);
        writer.Write(flag1);
        if (flag1)
          return;
        this.WriteCoord2D(writer, coord, p);
        writer.Write(flag2);
      }
    }

    private void CompileCommandMenu(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken = this.GetNextToken(originalCaseCommand, index);
      ScriptMenuOptions scriptMenuOptions = ScriptMenuOptions.None;
      if (nextToken.Lexeme.Equals("nocancel", StringComparison.OrdinalIgnoreCase))
      {
        scriptMenuOptions |= ScriptMenuOptions.NoCancel;
        nextToken = this.GetNextToken(originalCaseCommand, nextToken.EndIndex + 1);
      }
      if (this.menuParams == null)
        this.menuParams = new List<ScriptMenuParam>();
      else
        this.menuParams.Clear();
      for (; nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(originalCaseCommand, nextToken.EndIndex + 1))
      {
        ScriptMenuParam menuParam = this.ParseMenuParam(nextToken.Lexeme);
        if (this.ValidateMenuParam("menu item", menuParam))
          this.menuParams.Add(menuParam);
      }
      if (this.IsTest || this.menuParams.Count <= 0)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Menu);
      writer.Write((byte) scriptMenuOptions);
      writer.Write((byte) this.menuParams.Count);
      foreach (ScriptMenuParam menuParam in this.menuParams)
        this.WriteMenuParam(writer, menuParam);
    }

    private void WriteMenuParam(BinaryWriter writer, ScriptMenuParam param)
    {
      writer.Write(param.Text);
      writer.Write(param.Script.IsNotEmpty());
      if (param.Script.IsNotEmpty())
        writer.Write(param.Script);
      this.WriteCoord(writer, param.Coord, param.Point);
    }

    private ScriptMenuParam ParseMenuParam(string command)
    {
      ScriptMenuParam scriptMenuParam = new ScriptMenuParam();
      Parser.Token nextToken1 = this.GetNextToken(command, 0);
      scriptMenuParam.Text = nextToken1.Lexeme;
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      scriptMenuParam.Script = nextToken2.Lexeme;
      Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      scriptMenuParam.Point = this.GetPointFromToken(nextToken3, out scriptMenuParam.Coord);
      return scriptMenuParam;
    }

    private bool ValidateMenuParam(string desc, ScriptMenuParam param)
    {
      bool flag = true;
      if (param.Text.IsEmpty())
      {
        this.LogTestError(desc + ": text cannot be empty");
        flag = false;
      }
      if (param.Script.IsEmpty())
        this.LogTestWarning(desc + ": no script specified");
      return flag;
    }

    private void CompileCommandMessageBox(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken = this.GetNextToken(originalCaseCommand, index);
      ScriptMenuOptions scriptMenuOptions = ScriptMenuOptions.None;
      if (nextToken.Lexeme.Equals("nocancel", StringComparison.OrdinalIgnoreCase))
      {
        scriptMenuOptions |= ScriptMenuOptions.NoCancel;
        nextToken = this.GetNextToken(originalCaseCommand, nextToken.EndIndex + 1);
      }
      string s = (string) null;
      ScriptMenuParam scriptMenuParam1 = new ScriptMenuParam();
      ScriptMenuParam scriptMenuParam2 = new ScriptMenuParam();
      ScriptMenuParam scriptMenuParam3 = new ScriptMenuParam();
      ScriptMenuParam scriptMenuParam4 = new ScriptMenuParam();
      for (; nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(originalCaseCommand, nextToken.EndIndex + 1))
      {
        if (nextToken.Lexeme.StartsWith("a=", StringComparison.OrdinalIgnoreCase))
        {
          ScriptMenuParam menuParam = this.ParseMenuParam(nextToken.Lexeme);
          if (this.ValidateMenuParam("A button", menuParam))
            scriptMenuParam1 = menuParam;
        }
        else if (nextToken.Lexeme.StartsWith("x=", StringComparison.OrdinalIgnoreCase))
        {
          ScriptMenuParam menuParam = this.ParseMenuParam(nextToken.Lexeme);
          if (this.ValidateMenuParam("X button", menuParam))
            scriptMenuParam2 = menuParam;
        }
        else if (nextToken.Lexeme.StartsWith("y=", StringComparison.OrdinalIgnoreCase))
        {
          ScriptMenuParam menuParam = this.ParseMenuParam(nextToken.Lexeme);
          if (this.ValidateMenuParam("Y button", menuParam))
            scriptMenuParam3 = menuParam;
        }
        else
          s = nextToken.Lexeme;
      }
      if (this.IsTest || !s.IsNotEmpty() && !scriptMenuParam1.Text.IsNotEmpty() && (!scriptMenuParam2.Text.IsNotEmpty() && !scriptMenuParam3.Text.IsNotEmpty()))
        return;
      this.WriteCommandHeader(writer, ScriptCommand.MessageBox);
      writer.Write((byte) scriptMenuOptions);
      writer.Write(s != null);
      if (s != null)
        writer.Write(s);
      writer.Write(scriptMenuParam1.Text != null);
      if (scriptMenuParam1.Text != null)
        this.WriteMenuParam(writer, scriptMenuParam1);
      writer.Write(scriptMenuParam2.Text != null);
      if (scriptMenuParam2.Text != null)
        this.WriteMenuParam(writer, scriptMenuParam2);
      writer.Write(scriptMenuParam3.Text != null);
      if (scriptMenuParam3.Text == null)
        return;
      this.WriteMenuParam(writer, scriptMenuParam3);
    }

    private void CompileCommandNpcHealth(BinaryWriter writer, string command, int index)
    {
      ScriptCoordType coord1 = ScriptCoordType.None;
      ScriptCoordType coord2 = ScriptCoordType.None;
      ScriptPoint3D? p1 = new ScriptPoint3D?();
      ScriptPoint3D? p2 = new ScriptPoint3D?();
      ActorType? nullable1 = new ActorType?();
      string s = "";
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      bool flag1 = nextToken1.Lexeme == "target";
      Parser.Token nextToken2;
      if (flag1)
      {
        nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      }
      else
      {
        p1 = this.GetPointFromToken(nextToken1, out coord1);
        if (!p1.HasValue || coord1 == ScriptCoordType.None)
        {
          s = nextToken1.Lexeme;
          if (s.IsEmpty())
          {
            this.LogTestError("[zone] not recognized");
            return;
          }
        }
        else
        {
          nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          p2 = this.GetPointFromToken(nextToken1, out coord2);
          if (!p2.HasValue)
          {
            this.LogTestError("[x,y,z] not found");
            return;
          }
        }
        nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        nullable1 = this.GetActorTypeFromToken(nextToken2);
        if (nullable1.HasValue)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken2);
      if (!intFromToken2.HasValue || intFromToken2.Value.IsZero)
      {
        this.LogTestError("invalid qty value");
      }
      else
      {
        Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        ScriptInt32? nullable2 = this.GetIntFromToken2(nextToken3);
        if (!nullable2.HasValue)
          nullable2 = new ScriptInt32?((ScriptInt32) 0);
        ScriptInt32? nullable3 = this.GetIntFromToken2(this.GetNextToken(command, nextToken3.EndIndex + 1));
        if (!nullable3.HasValue)
          nullable3 = new ScriptInt32?((ScriptInt32) 0);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.NpcHealth);
        this.WriteInt32(writer, intFromToken2.Value);
        this.WriteInt32(writer, nullable2.Value);
        this.WriteInt32(writer, nullable3.Value);
        writer.Write(flag1);
        if (flag1)
          return;
        writer.Write(nullable1.HasValue ? (byte) nullable1.Value : (byte) 0);
        bool flag2 = coord1 == ScriptCoordType.None;
        writer.Write(flag2);
        if (flag2)
        {
          writer.Write(s);
        }
        else
        {
          this.WriteCoord(writer, coord1, p1);
          this.WriteCoord(writer, coord2, p2);
        }
      }
    }

    private void CompileCommandNpcSpawn(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ActorType? actorTypeFromToken = this.GetActorTypeFromToken(nextToken2);
        if (!actorTypeFromToken.HasValue)
        {
          this.LogTestError("npc type not found");
        }
        else
        {
          string ai;
          string dialog;
          string killScript;
          LootTable lootTable;
          CombatStats? stats;
          this.GetNpcSpawnParam(command, nextToken2.EndIndex + 1, out ai, out dialog, out killScript, out lootTable, out stats);
          if (!this.instance.IsCreativeMode)
          {
            if (lootTable != null)
            {
              if (!lootTable.Point.HasValue)
              {
                this.LogTestError("Loot must be sourced from a chest or any storage block in this game mode");
                return;
              }
              GlobalPoint3D p = lootTable.Point.Value;
              if (!(this.instance.MapStrategyTM.GetDataBlock(p) is ChestBlock))
                this.LogTestWarning(string.Format("No storage block found at coord [{0},{1},{2}]", (object) p.X, (object) p.Y, (object) p.Z));
            }
            else
            {
              lootTable = new LootTable();
              lootTable.Table.Add(new LootDrop());
            }
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.NpcSpawn);
          this.WriteCoord(writer, coord, pointFromToken);
          writer.Write((byte) actorTypeFromToken.Value);
          this.WriteString(writer, ai);
          this.WriteString(writer, dialog);
          this.WriteString(writer, killScript);
          writer.Write(stats.HasValue);
          if (stats.HasValue)
            stats.Value.WriteState(writer);
          writer.Write(lootTable != null);
          lootTable?.WriteState(writer);
        }
      }
    }

    private void GetNpcSpawnParam(
      string command,
      int index,
      out string ai,
      out string dialog,
      out string killScript,
      out LootTable lootTable,
      out CombatStats? stats)
    {
      ai = (string) null;
      dialog = (string) null;
      killScript = (string) null;
      lootTable = (LootTable) null;
      stats = new CombatStats?();
      for (Parser.Token nextToken = this.GetNextToken(command, index); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        int length = nextToken.Lexeme.IndexOf('=');
        if (length >= 0)
        {
          string str = nextToken.Lexeme.Substring(0, length);
          string command1 = nextToken.Lexeme.Substring(length + 1, nextToken.Lexeme.Length - length - 1);
          switch (str)
          {
            case nameof (ai):
              killScript = command1;
              continue;
            case nameof (dialog):
              killScript = command1;
              continue;
            case "kill":
              killScript = command1;
              continue;
            case "custom":
              this.ParseCombatStatsTable(command1, out stats);
              continue;
            case "loot":
              this.ParseLootTable(command1, lootTable = new LootTable());
              continue;
            default:
              continue;
          }
        }
      }
    }

    private void ParseLootTable(string command, LootTable lootTable)
    {
      for (Parser.Token nextToken = this.GetNextToken(command, 0); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken);
        if (pointFromToken.HasValue)
        {
          lootTable.Point = new GlobalPoint3D?((GlobalPoint3D) pointFromToken.Value);
        }
        else
        {
          LootDrop? lootDrop = this.GetLootDrop(nextToken.Lexeme);
          if (lootDrop.HasValue)
            lootTable.Table.Add(lootDrop.Value);
        }
      }
    }

    private LootDrop? GetLootDrop(string command)
    {
      LootDrop? nullable1 = new LootDrop?();
      Parser.Token nextToken1 = this.GetNextToken(command, 0, char.MinValue, ',');
      Item? itemIdFromToken = this.GetItemIDFromToken(nextToken1);
      if (!itemIdFromToken.HasValue)
      {
        this.LogTestError("Loot item: " + nextToken1.Lexeme + " not recognized");
        return new LootDrop?();
      }
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1, char.MinValue, ',');
      int? nullable2 = this.GetIntFromToken(nextToken2);
      if (!nullable2.HasValue || nullable2.Value <= 0)
        nullable2 = new int?(1);
      int? nullable3 = this.GetIntFromToken(this.GetNextToken(command, nextToken2.EndIndex + 1, char.MinValue, ','));
      if (!nullable3.HasValue || nullable3.Value <= 0 || nullable3.Value > 100)
        nullable3 = new int?(100);
      nullable1 = new LootDrop?(new LootDrop()
      {
        ItemID = itemIdFromToken.Value,
        Count = nullable2.Value,
        Percent = (float) nullable3.Value
      });
      return nullable1;
    }

    private void ParseCombatStatsTable(string command, out CombatStats? stats)
    {
      stats = new CombatStats?();
      CombatStats combatStats = new CombatStats();
      for (Parser.Token nextToken = this.GetNextToken(command, 0); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        ScriptCompiler.NpcStat? npcStat = this.GetNpcStat(nextToken.Lexeme);
        if (npcStat.HasValue)
        {
          switch (npcStat.Value.SkillType)
          {
            case SkillType.Health:
              combatStats.HealthLevel = npcStat.Value.Level;
              continue;
            case SkillType.Strength:
              combatStats.StrengthLevel = npcStat.Value.Level;
              continue;
            case SkillType.Attack:
              combatStats.AttackLevel = npcStat.Value.Level;
              continue;
            case SkillType.Defence:
              combatStats.DefenceLevel = npcStat.Value.Level;
              continue;
            case SkillType.Ranged:
              combatStats.RangedLevel = npcStat.Value.Level;
              continue;
            default:
              continue;
          }
        }
      }
      if (combatStats.HealthLevel <= 0 && combatStats.AttackLevel <= 0 && (combatStats.StrengthLevel <= 0 && combatStats.DefenceLevel <= 0) && combatStats.RangedLevel <= 0)
        return;
      stats = new CombatStats?(combatStats);
    }

    private ScriptCompiler.NpcStat? GetNpcStat(string command)
    {
      ScriptCompiler.NpcStat? nullable = new ScriptCompiler.NpcStat?();
      Parser.Token nextToken1 = this.GetNextToken(command, 0, char.MinValue, ',');
      SkillType? skillFromToken = this.GetSkillFromToken(nextToken1);
      if (!skillFromToken.HasValue || skillFromToken.Value != SkillType.Health && skillFromToken.Value != SkillType.Attack && (skillFromToken.Value != SkillType.Strength && skillFromToken.Value != SkillType.Defence) && skillFromToken.Value != SkillType.Ranged)
      {
        this.LogTestError("Custom Stat Skill: " + nextToken1.Lexeme + " not recognized");
        return new ScriptCompiler.NpcStat?();
      }
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1, char.MinValue, ',');
      int? intFromToken = this.GetIntFromToken(nextToken2);
      if (!intFromToken.HasValue || intFromToken.Value <= 0)
      {
        this.LogTestError("Custom Stat Level: " + nextToken2.Lexeme + " not recognized");
        return new ScriptCompiler.NpcStat?();
      }
      nullable = new ScriptCompiler.NpcStat?(new ScriptCompiler.NpcStat()
      {
        SkillType = skillFromToken.Value,
        Level = intFromToken.Value
      });
      return nullable;
    }

    private void CompileCommandNpcState(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.Lexeme == "move")
      {
        ScriptCoordType coord = ScriptCoordType.None;
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptVector3? vector3FromToken = this.GetVector3FromToken(nextToken1, out coord);
        if (!vector3FromToken.HasValue || coord == ScriptCoordType.None)
        {
          this.LogTestError("[x,y,z] not recognized");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.NpcState);
          writer.Write((byte) 1);
          this.WriteVector3(writer, coord, vector3FromToken);
        }
      }
      else
      {
        ScriptCoordType coord1 = ScriptCoordType.None;
        ScriptCoordType coord2 = ScriptCoordType.None;
        ScriptPoint3D? p1 = new ScriptPoint3D?();
        ScriptPoint3D? p2 = new ScriptPoint3D?();
        string s = "";
        bool flag1 = nextToken1.Lexeme == "target";
        if (!flag1)
        {
          p1 = this.GetPointFromToken(nextToken1, out coord1);
          if (!p1.HasValue || coord1 == ScriptCoordType.None)
          {
            s = nextToken1.Lexeme;
            if (s.IsEmpty())
            {
              this.LogTestError("[zone] not recognized");
              return;
            }
          }
          else
          {
            nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
            p2 = this.GetPointFromToken(nextToken1, out coord2);
            if (!p2.HasValue)
            {
              this.LogTestError("[x,y,z] not found");
              return;
            }
          }
        }
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ActorState actorState = nextToken2.Lexeme == "death" ? ActorState.Dying : (nextToken2.Lexeme == "despawn" ? ActorState.Despawning : (nextToken2.Lexeme == "delete" ? ActorState.InActive : ActorState.Alive));
        if (actorState == ActorState.Alive)
        {
          this.LogTestError("[state] not found. state must be either [death], [despawn] or [delete]");
        }
        else
        {
          ActorType? actorTypeFromToken = this.GetActorTypeFromToken(this.GetNextToken(command, nextToken2.EndIndex + 1));
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.NpcState);
          writer.Write((byte) actorState);
          writer.Write(flag1);
          if (flag1)
            return;
          writer.Write(actorTypeFromToken.HasValue ? (byte) actorTypeFromToken.Value : (byte) 0);
          bool flag2 = coord1 == ScriptCoordType.None;
          writer.Write(flag2);
          if (flag2)
          {
            writer.Write(s);
          }
          else
          {
            this.WriteCoord(writer, coord1, p1);
            this.WriteCoord(writer, coord2, p2);
          }
        }
      }
    }

    private void CompileCommandMoveBlock(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("source [x,y,z] not found");
      }
      else
      {
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(this.GetNextToken(command, nextToken.EndIndex + 1), out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("destination [x,y,z] not found");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.MoveBlock);
          this.WriteCoord(writer, coord1, pointFromToken1);
          this.WriteCoord(writer, coord2, pointFromToken2);
        }
      }
    }

    private void CompileCommandMoveRegion(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("max [x,y,z] not found");
        }
        else
        {
          ScriptCoordType coord3;
          ScriptPoint3D? pointFromToken3 = this.GetPointFromToken(this.GetNextToken(command, nextToken2.EndIndex + 1), out coord3);
          if (!pointFromToken3.HasValue || coord3 == ScriptCoordType.None)
          {
            this.LogTestError("destination [x,y,z] not found");
          }
          else
          {
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.MoveRegion);
            this.WriteCoord(writer, coord1, pointFromToken1);
            this.WriteCoord(writer, coord2, pointFromToken2);
            this.WriteCoord(writer, coord3, pointFromToken3);
          }
        }
      }
    }

    private void CompileCommandNop(BinaryWriter writer, string command, int index)
    {
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Nop);
    }

    private void CompileCommandNotify(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(originalCaseCommand, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("no message specified");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptColor? color4FromToken2 = this.GetColor4FromToken2(nextToken2);
        if (color4FromToken2.HasValue)
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        NotifyRecipient notifyRecipient = NotifyRecipient.None;
        if (nextToken2.Lexeme.Contains("local"))
          notifyRecipient |= NotifyRecipient.Local;
        if (nextToken2.Lexeme.Contains("remote"))
          notifyRecipient |= NotifyRecipient.Remote;
        if (nextToken2.Lexeme.Contains("global"))
          notifyRecipient |= NotifyRecipient.Global;
        if (nextToken2.Lexeme.Contains("admin"))
          notifyRecipient |= NotifyRecipient.Admin;
        if (nextToken2.Lexeme.Contains("clan"))
          notifyRecipient |= NotifyRecipient.Clan;
        if (notifyRecipient == NotifyRecipient.None)
          notifyRecipient = NotifyRecipient.Local;
        if (!color4FromToken2.HasValue)
          color4FromToken2 = this.GetColor4FromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
        if (this.IsTest || notifyRecipient == NotifyRecipient.None)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Notify);
        writer.Write((byte) notifyRecipient);
        writer.Write(lexeme);
        this.WriteColor(writer, color4FromToken2);
      }
    }

    private void CompileCommandOpenBlock(BinaryWriter writer, string command, int index)
    {
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(this.GetNextToken(command, index), out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.OpenBlock);
        this.WriteCoord(writer, coord, pointFromToken);
      }
    }

    private void CompileCommandParticle(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptVector3? vector3FromToken = this.GetVector3FromToken(nextToken, out coord);
      if (!vector3FromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        ScriptParticleData data = new ScriptParticleData();
        int index1 = nextToken.EndIndex + 1;
        ushort? templateIdFromName = this.GetParticleTemplateIDFromName(this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme);
        if (!templateIdFromName.HasValue && !this.ParseParticleData(command, index1, ref data) || this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Particle);
        this.WriteVector3(writer, coord, vector3FromToken);
        writer.Write(templateIdFromName.HasValue);
        if (templateIdFromName.HasValue)
          writer.Write(templateIdFromName.Value);
        else
          this.WriteParticleState(writer, data);
      }
    }

    private void CompileCommandParticleEmitter(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptVector3? vector3FromToken = this.GetVector3FromToken(nextToken1, out coord);
      if (!vector3FromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptSingle? floatFromToken2 = this.GetFloatFromToken2(nextToken2);
        if (!floatFromToken2.HasValue)
        {
          this.LogTestError("[emitterduration] not found. this is the time in seconds that the emitter exists");
        }
        else
        {
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          ushort? templateIdFromName = this.GetParticleTemplateIDFromName(nextToken3.Lexeme);
          if (templateIdFromName.HasValue)
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          ScriptSingle? nullable = this.GetFloatFromToken2(nextToken3);
          if (templateIdFromName.HasValue && !nullable.HasValue)
            nullable = new ScriptSingle?((ScriptSingle) 0.0f);
          if (!nullable.HasValue || nullable.Value.IsZero && !templateIdFromName.HasValue)
          {
            this.LogTestError("either [template] or [emitfreq] not found");
          }
          else
          {
            ScriptParticleData data = new ScriptParticleData();
            data.EmitFreq = nullable.Value;
            if (!templateIdFromName.HasValue && !this.ParseParticleData(command, nextToken3.EndIndex + 1, ref data) || this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.ParticleEmitter);
            this.WriteVector3(writer, coord, vector3FromToken);
            writer.Write(templateIdFromName.HasValue);
            if (templateIdFromName.HasValue)
            {
              writer.Write(templateIdFromName.Value);
              this.WriteSingle(writer, floatFromToken2.Value);
              this.WriteSingle(writer, data.EmitFreq);
            }
            else
            {
              this.WriteSingle(writer, floatFromToken2.Value);
              this.WriteParticleState(writer, data);
            }
          }
        }
      }
    }

    private ushort? GetParticleTemplateIDFromName(string name)
    {
      for (int index = 0; index < Globals2.SystemParticleData.Length; ++index)
      {
        if (Globals2.SystemParticleData[index].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return new ushort?((ushort) index);
      }
      for (int index = 0; index < Globals2.CustomParticleData.Count; ++index)
      {
        if (Globals2.CustomParticleData[index].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return new ushort?((ushort) (index + Globals2.SystemParticleData.Length));
      }
      return new ushort?();
    }

    private bool ParseParticleData(string command, int index, ref ScriptParticleData data)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptSingle? floatFromToken2_1 = this.GetFloatFromToken2(nextToken1);
      if (!floatFromToken2_1.HasValue || floatFromToken2_1.Value.IsZero)
      {
        this.LogTestError("[duration] not found. this is the time in seconds that the particle is alive");
        return false;
      }
      data.Duration = floatFromToken2_1.Value;
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptVector3? vector3FromToken1 = this.GetVector3FromToken(nextToken2, out coord);
      if (!vector3FromToken1.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[velocity] not found");
        return false;
      }
      data.Velocity = vector3FromToken1.Value;
      data.VelocityType = coord;
      if (coord == ScriptCoordType.PlayerRelative || coord == ScriptCoordType.Relative || coord == ScriptCoordType.CursorRelative)
      {
        this.LogTestError("[velocity] does not support [rel:], [prel:] or [crel:]. You can use [vrel:] and [hvrel:]");
        return false;
      }
      Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      ScriptVector4? vector4FromToken = this.GetVector4FromToken(nextToken3);
      if (!vector4FromToken.HasValue)
      {
        this.LogTestError("[size] not found");
        return false;
      }
      data.Size = vector4FromToken.Value;
      Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
      ScriptColor? color4FromToken2_1 = this.GetColor4FromToken2(nextToken4);
      if (!color4FromToken2_1.HasValue)
      {
        this.LogTestError("[startcolor] not found. must be in RGBA format");
        return false;
      }
      data.StartColor = color4FromToken2_1.Value;
      Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
      ScriptSingle? floatFromToken2_2 = this.GetFloatFromToken2(nextToken5);
      data.Gravity = floatFromToken2_2.HasValue ? floatFromToken2_2.Value : new ScriptSingle();
      Parser.Token nextToken6 = this.GetNextToken(command, nextToken5.EndIndex + 1);
      floatFromToken2_2 = this.GetFloatFromToken2(nextToken6);
      data.Rotation = floatFromToken2_2.HasValue ? floatFromToken2_2.Value : new ScriptSingle();
      Parser.Token nextToken7 = this.GetNextToken(command, nextToken6.EndIndex + 1);
      ScriptVector3? vector3FromToken2 = this.GetVector3FromToken(nextToken7);
      data.VelocityVariance = vector3FromToken2.HasValue ? vector3FromToken2.Value : (ScriptVector3) Vector3.Zero;
      Parser.Token nextToken8 = this.GetNextToken(command, nextToken7.EndIndex + 1);
      ScriptColor? color4FromToken2_2 = this.GetColor4FromToken2(nextToken8);
      data.EndColor = color4FromToken2_2.HasValue ? color4FromToken2_2.Value : data.StartColor;
      Parser.Token nextToken9 = this.GetNextToken(command, nextToken8.EndIndex + 1);
      vector3FromToken2 = this.GetVector3FromToken(nextToken9);
      data.EmitPosVariance = vector3FromToken2.HasValue ? vector3FromToken2.Value : (ScriptVector3) Vector3.Zero;
      vector3FromToken2 = this.GetVector3FromToken(this.GetNextToken(command, nextToken9.EndIndex + 1));
      data.EmitPosOffset = vector3FromToken2.HasValue ? vector3FromToken2.Value : (ScriptVector3) Vector3.Zero;
      return true;
    }

    private void WriteParticleState(BinaryWriter writer, ScriptParticleData data)
    {
      this.WriteSingle(writer, data.EmitFreq);
      this.WriteSingle(writer, data.Duration);
      this.WriteSingle(writer, data.Rotation);
      writer.Write((byte) data.VelocityType);
      this.WriteVector3(writer, data.Velocity);
      this.WriteVector3(writer, data.VelocityVariance);
      this.WriteVector3(writer, data.EmitPosOffset);
      this.WriteVector3(writer, data.EmitPosVariance);
      this.WriteVector4(writer, data.Size);
      this.WriteSingle(writer, data.WindFactor);
      this.WriteSingle(writer, data.Gravity);
      this.WriteColor(writer, data.StartColor);
      this.WriteColor(writer, data.EndColor);
    }

    private void CompileCommandPaste(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 3)
      {
        this.LogTestError("component pack not found");
      }
      else
      {
        int length = lexeme.IndexOf(':');
        if (length < 0)
        {
          this.LogTestError("component name not found");
        }
        else
        {
          string str1 = lexeme.Substring(0, length);
          string str2 = lexeme.Substring(length + 1, lexeme.Length - (length + 1)).Replace('\\', '_');
          if (str2 == null || str2.Length < 1)
          {
            this.LogTestError("component name not found");
          }
          else
          {
            Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
            ScriptCoordType coord;
            ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken2, out coord);
            if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
            {
              this.LogTestError("[x,y,z] not found");
            }
            else
            {
              Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
              BlockFace? nullable1 = this.GetFaceFromToken(nextToken3);
              if (!nullable1.HasValue)
              {
                nullable1 = new BlockFace?(BlockFace.Left);
                this.LogTestWarning("facing defaulted to left");
              }
              Map.CopyType? nullable2 = this.GetCopyTypeFromToken(this.GetNextToken(command, nextToken3.EndIndex + 1));
              if (!nullable2.HasValue)
              {
                nullable2 = new Map.CopyType?(Map.CopyType.Overwrite);
                this.LogTestWarning("copy type defaulted to overwrite");
              }
              if (this.IsTest)
                return;
              this.WriteCommandHeader(writer, ScriptCommand.Paste);
              writer.Write(str1);
              writer.Write(str2);
              this.WriteCoord(writer, coord, pointFromToken);
              writer.Write((byte) nullable1.Value);
              writer.Write((byte) nullable2.Value);
            }
          }
        }
      }
    }

    private void CompileCommandPermission(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      Permissions? permissionFromToken = this.GetPermissionFromToken(nextToken, false);
      if (!permissionFromToken.HasValue)
      {
        this.LogTestError("permission not found");
      }
      else
      {
        bool flag = this.GetNextToken(command, nextToken.EndIndex + 1).Lexeme == "on";
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Permission);
        writer.Write((ushort) permissionFromToken.Value);
        writer.Write(flag);
      }
    }

    private void CompileCommandPickup(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      bool flag = nextToken1.Lexeme == "clear";
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptPoint3D? p = new ScriptPoint3D?();
      Item? nullable1 = new Item?();
      ScriptInt32? nullable2 = new ScriptInt32?();
      if (!flag)
      {
        p = this.GetPointFromToken(nextToken1, out coord);
        if (!p.HasValue || coord == ScriptCoordType.None)
        {
          this.LogTestError("[x,y,z] not found");
          return;
        }
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        nullable1 = this.GetItemIDFromToken(nextToken2);
        if (!nullable1.HasValue)
        {
          this.LogTestError("item not found");
          return;
        }
        nullable2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
        if (!nullable2.HasValue)
          nullable2 = new ScriptInt32?((ScriptInt32) 1);
      }
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Pickup);
      writer.Write(flag);
      if (flag)
        return;
      this.WriteCoord(writer, coord, p);
      writer.Write((ushort) nullable1.Value);
      this.WriteInt32(writer, nullable2.Value);
    }

    private void CompileCommandRain(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptPoint3D? point2DfromToken = this.GetPoint2DFromToken(nextToken1, out coord);
      if (!point2DfromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,z] not found");
      }
      else
      {
        ScriptSingle? nullable1 = new ScriptSingle?();
        ScriptSingle? nullable2 = new ScriptSingle?();
        ScriptSingle? nullable3 = new ScriptSingle?();
        ScriptColor? c = new ScriptColor?();
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag = nextToken2.Lexeme == "delete";
        if (!flag)
        {
          nullable1 = this.GetFloatFromToken2(nextToken2);
          if (!nullable1.HasValue)
          {
            this.LogTestError("[radius] not found");
            return;
          }
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable2 = this.GetFloatFromToken2(nextToken2);
          if (!nullable2.HasValue)
          {
            this.LogTestError("[duration] not found");
            return;
          }
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable3 = this.GetFloatFromToken2(nextToken2);
          if (!nullable3.HasValue)
          {
            this.LogTestError("[intensity] not found");
            return;
          }
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          c = this.GetColor4FromToken2(nextToken2);
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Rain);
        this.WriteCoord2D(writer, coord, point2DfromToken);
        writer.Write(flag);
        if (flag)
          return;
        this.WriteSingle(writer, nullable1.Value);
        this.WriteSingle(writer, nullable2.Value);
        this.WriteSingle(writer, nullable3.Value);
        this.WriteColor(writer, c);
      }
    }

    private void CompileCommandRandom(BinaryWriter writer, string command, int index)
    {
      ScriptInt32? intFromToken2 = this.GetIntFromToken2(this.GetNextToken(command, index));
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Random);
      writer.Write(intFromToken2.HasValue);
      if (!intFromToken2.HasValue)
        return;
      this.WriteInt32(writer, intFromToken2.Value);
    }

    private void CompileCommandReplaceRegion(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("max [x,y,z] not found");
        }
        else
        {
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          Block? blockIdFromToken1 = this.GetBlockIDFromToken(nextToken3);
          if (!blockIdFromToken1.HasValue)
          {
            this.LogTestError("[block1] not found");
          }
          else
          {
            Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            Block? blockIdFromToken2 = this.GetBlockIDFromToken(nextToken4);
            if (!blockIdFromToken2.HasValue)
            {
              this.LogTestError("[block2] not found");
            }
            else
            {
              Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
              ScriptInt32? nullable1 = this.GetIntFromToken2(nextToken5);
              if (!nullable1.HasValue)
                nullable1 = new ScriptInt32?((ScriptInt32) 100);
              ScriptInt32? nullable2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken5.EndIndex + 1));
              if (!nullable2.HasValue)
                nullable2 = new ScriptInt32?((ScriptInt32) 0);
              if (this.IsTest)
                return;
              this.WriteCommandHeader(writer, ScriptCommand.ReplaceRegion);
              this.WriteCoord(writer, coord1, pointFromToken1);
              this.WriteCoord(writer, coord2, pointFromToken2);
              writer.Write((byte) blockIdFromToken1.Value);
              writer.Write((byte) blockIdFromToken2.Value);
              this.WriteInt32(writer, nullable1.Value);
              this.WriteInt32(writer, nullable2.Value);
            }
          }
        }
      }
    }

    private void CompileCommandScript(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string lexeme = nextToken1.Lexeme;
      if (this.instance.GetScript(lexeme) == null)
        this.LogTestWarning("script: " + lexeme + " could not be found");
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      ScriptPoint3D? p = new ScriptPoint3D?();
      ScriptCoordType coord = ScriptCoordType.None;
      List<ushort> ushortList = (List<ushort>) null;
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      if (nextToken2.Lexeme == "cancel")
      {
        flag1 = true;
        nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        if (nextToken2.Lexeme == "all")
          flag2 = true;
      }
      else
      {
        if (nextToken2.Lexeme == "wait")
        {
          flag3 = true;
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        if (nextToken2.Lexeme == "offset")
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          p = this.GetPointFromToken(nextToken2, out coord);
          if (!p.HasValue || coord == ScriptCoordType.None)
          {
            this.LogTestError("offset [x,y,z] is invalid");
            return;
          }
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        if (nextToken2.Lexeme.StartsWith("vars:"))
        {
          string[] strArray = nextToken2.Lexeme.Substring(5).Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray != null && strArray.Length > 0)
          {
            ushortList = new List<ushort>();
            foreach (string str in strArray)
            {
              int varIndex = this.GetVarIndex(str.Trim());
              if (varIndex >= 0 && ushortList.Count < 256)
                ushortList.Add((ushort) varIndex);
              else
                this.LogTestWarning("Variable " + str + " does not exist");
            }
          }
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        if (nextToken2.Lexeme.StartsWith("out:"))
        {
          string[] strArray = nextToken2.Lexeme.Substring(4).Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray != null && strArray.Length > 0)
          {
            if (ushortList == null)
              ushortList = new List<ushort>();
            foreach (string str in strArray)
            {
              int varIndex = this.GetVarIndex(str.Trim());
              if (varIndex >= 0 && ushortList.Count < 256)
              {
                int num = varIndex | 32768;
                ushortList.Add((ushort) num);
                flag3 = true;
              }
              else
                this.LogTestWarning("Variable " + str + " does not exist");
            }
          }
        }
      }
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Script);
      writer.Write(lexeme);
      writer.Write(flag1);
      if (flag1)
      {
        writer.Write(flag2);
      }
      else
      {
        writer.Write(flag3);
        this.WriteCoord(writer, coord, p);
        ushort num1 = ushortList != null ? (ushort) ushortList.Count : (ushort) 0;
        writer.Write(num1);
        if (num1 <= (ushort) 0)
          return;
        foreach (ushort num2 in ushortList)
          writer.Write(num2);
      }
    }

    private void CompileCommandSetBlock(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        Block? nullable1 = this.GetBlockIDFromToken(nextToken2);
        if (!nullable1.HasValue)
          nullable1 = new Block?(Block.None);
        ScriptInt32? nullable2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken2.EndIndex + 1));
        if (nullable1.Value != Block.None && !nullable2.HasValue)
          nullable2 = new ScriptInt32?((ScriptInt32) 0);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.SetBlock);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write((byte) nullable1.Value);
        if (nullable1.Value == Block.None)
          return;
        this.WriteInt32(writer, nullable2.Value);
      }
    }

    private void CompileCommandSetBlockScript(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(originalCaseCommand, nextToken1.EndIndex + 1);
        string lexeme = nextToken2.Lexeme;
        if (lexeme.IsNotEmpty() && this.instance.GetScript(lexeme) == null)
          this.LogTestWarning("script: " + lexeme + " could not be found");
        DataBlockScriptType? nullable = this.GetDataBlockScriptTypeFromToken(this.GetNextToken(command, nextToken2.EndIndex + 1));
        if (!nullable.HasValue)
          nullable = new DataBlockScriptType?(DataBlockScriptType.None);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.SetBlockScript);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write((byte) nullable.Value);
        writer.Write(lexeme.IsNotEmpty() ? lexeme : "");
      }
    }

    private void CompileCommandSetEventScript(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptEvent? eventTypeFromToken = this.GetScriptEventTypeFromToken(nextToken1);
      if (!eventTypeFromToken.HasValue)
      {
        this.LogTestError("event type not found");
      }
      else
      {
        Item? nullable1 = new Item?();
        string s = (string) null;
        bool flag1 = false;
        ScriptVector2? v = new ScriptVector2?();
        ScriptSingle? nullable2 = new ScriptSingle?();
        bool flag2 = eventTypeFromToken.Value == ScriptEvent.ItemSwing || eventTypeFromToken.Value == ScriptEvent.ItemEquip || eventTypeFromToken.Value == ScriptEvent.ItemUnequip;
        Parser.Token nextToken2;
        if (flag2)
        {
          nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          nullable1 = this.GetItemIDFromToken(nextToken2.Lexeme, true, false, true);
          if (!nullable1.HasValue)
          {
            if (nextToken2.Lexeme.IsNotEmpty() && this.instance.GetScript(nextToken2.Lexeme) == null)
            {
              this.LogTestError("item or script not found: " + nextToken2.Lexeme);
              return;
            }
          }
          else
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        else if (eventTypeFromToken.Value == ScriptEvent.ButtonX || eventTypeFromToken.Value == ScriptEvent.ButtonY || eventTypeFromToken.Value == ScriptEvent.ButtonB)
        {
          nextToken2 = this.GetNextToken(originalCaseCommand, nextToken1.EndIndex + 1);
          s = nextToken2.Lexeme;
          flag1 = true;
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        }
        else
          nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        string lexeme = nextToken2.Lexeme;
        if (!flag1 && lexeme.IsNotEmpty() && this.instance.GetScript(lexeme) == null)
          this.LogTestWarning("script: " + lexeme + " could not be found");
        if (flag1)
        {
          nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          v = this.GetVector2FromToken(nextToken2);
          if (v.HasValue)
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          nullable2 = this.GetFloatFromToken2(nextToken2);
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.SetEventScript);
        writer.Write((byte) eventTypeFromToken.Value);
        writer.Write(lexeme.IsNotEmpty() ? lexeme : "");
        if (flag2)
        {
          writer.Write(nullable1.HasValue);
          if (nullable1.HasValue)
            writer.Write((ushort) nullable1.Value);
        }
        if (!flag1)
          return;
        writer.Write(s.IsNotEmpty() ? s : "");
        this.WriteVector2(writer, v);
        writer.Write(nullable2.HasValue);
        if (!nullable2.HasValue)
          return;
        this.WriteSingle(writer, nullable2.Value);
      }
    }

    private void CompileCommandSetNameplate(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      NamePlateSetting namePlateSetting = NamePlateSetting.None;
      switch (nextToken1.Lexeme)
      {
        case "on":
        case "short":
          namePlateSetting = NamePlateSetting.Short;
          goto case "off";
        case "far":
          namePlateSetting = NamePlateSetting.Far;
          goto case "off";
        case "off":
          Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          ScriptTarget scriptTarget = nextToken2.Lexeme == "player" ? ScriptTarget.Actor : (nextToken2.Lexeme == "clan" ? ScriptTarget.Clan : ScriptTarget.None);
          bool flag;
          if (scriptTarget == ScriptTarget.None)
          {
            flag = nextToken2.Lexeme == "npc" || nextToken2.Lexeme == "mobs";
          }
          else
          {
            Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            flag = nextToken3.Lexeme == "npc" || nextToken3.Lexeme == "mobs";
          }
          if (this.IsTest)
            break;
          this.WriteCommandHeader(writer, ScriptCommand.SetNameplate);
          writer.Write((byte) namePlateSetting);
          writer.Write((byte) scriptTarget);
          writer.Write(flag);
          break;
        default:
          this.LogTestError("[on|off|short|far] not recognized");
          break;
      }
    }

    private void CompileCommandSetPower(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        if (nextToken2.Lexeme != "on" && nextToken2.Lexeme != "off")
        {
          this.LogTestError("[on|off] not found");
        }
        else
        {
          bool flag = nextToken2.Lexeme == "on";
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.SetPower);
          this.WriteCoord(writer, coord, pointFromToken);
          writer.Write(flag);
        }
      }
    }

    private void CompileCommandSetReach(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptInt32? nullable = this.GetIntFromToken2(nextToken);
      if (nullable.HasValue)
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      else
        nullable = new ScriptInt32?((ScriptInt32) 0);
      ScriptTarget scriptTarget = nextToken.Lexeme == "player" || nextToken.Lexeme == "actor" ? ScriptTarget.Actor : (nextToken.Lexeme == "clan" ? ScriptTarget.Clan : ScriptTarget.None);
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.SetReach);
      this.WriteInt32(writer, nullable.Value);
      writer.Write((byte) scriptTarget);
    }

    private void CompileCommandSetRegion(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("max [x,y,z] not found");
        }
        else
        {
          ScriptInt32? nullable1 = new ScriptInt32?((ScriptInt32) 100);
          ScriptInt32? nullable2 = new ScriptInt32?((ScriptInt32) 0);
          Block? nullable3 = new Block?(Block.None);
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          if (nextToken3.Lexeme.IsNotEmpty())
          {
            nullable3 = this.GetBlockIDFromToken(nextToken3);
            if (!nullable3.HasValue)
            {
              this.LogTestError("[block] not found");
              return;
            }
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            nullable1 = this.GetIntFromToken2(nextToken3);
            if (!nullable1.HasValue)
              nullable1 = new ScriptInt32?((ScriptInt32) 100);
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            int? intFromToken = this.GetIntFromToken(nextToken3);
            nullable2 = intFromToken.HasValue ? new ScriptInt32?((ScriptInt32) intFromToken.GetValueOrDefault()) : new ScriptInt32?();
            if (!nullable2.HasValue)
              nullable2 = new ScriptInt32?((ScriptInt32) 0);
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.SetRegion);
          this.WriteCoord(writer, coord1, pointFromToken1);
          this.WriteCoord(writer, coord2, pointFromToken2);
          writer.Write((byte) nullable3.Value);
          this.WriteInt32(writer, nullable1.Value);
          this.WriteInt32(writer, nullable2.Value);
        }
      }
    }

    private void CompileCommandSetRegionAux(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord1;
      ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
      {
        this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptCoordType coord2;
        ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken2, out coord2);
        if (!pointFromToken2.HasValue || coord2 == ScriptCoordType.None)
        {
          this.LogTestError("max [x,y,z] not found");
        }
        else
        {
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          bool flag = nextToken3.Lexeme == "high";
          Parser.Token nextToken4 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken4);
          if (!intFromToken2.HasValue)
          {
            this.LogTestError("[aux] not found");
          }
          else
          {
            Parser.Token nextToken5 = this.GetNextToken(command, nextToken4.EndIndex + 1);
            ScriptInt32? nullable1 = this.GetIntFromToken2(nextToken5);
            if (!nullable1.HasValue)
              nullable1 = new ScriptInt32?((ScriptInt32) 100);
            ScriptInt32? nullable2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken5.EndIndex + 1));
            if (!nullable2.HasValue)
              nullable2 = new ScriptInt32?((ScriptInt32) 0);
            if (this.IsTest)
              return;
            this.WriteCommandHeader(writer, ScriptCommand.SetRegionAux);
            this.WriteCoord(writer, coord1, pointFromToken1);
            this.WriteCoord(writer, coord2, pointFromToken2);
            writer.Write(flag);
            this.WriteInt32(writer, intFromToken2.Value);
            this.WriteInt32(writer, nullable1.Value);
            this.WriteInt32(writer, nullable2.Value);
          }
        }
      }
    }

    private void CompileCommandSetSphere(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("center [x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken2);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("radius not found");
        }
        else
        {
          ScriptInt32? nullable1 = new ScriptInt32?((ScriptInt32) 100);
          ScriptInt32? nullable2 = new ScriptInt32?((ScriptInt32) 0);
          Block? nullable3 = new Block?(Block.None);
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          if (nextToken3.Lexeme.IsNotEmpty())
          {
            nullable3 = this.GetBlockIDFromToken(nextToken3);
            if (!nullable3.HasValue)
            {
              this.LogTestError("[block] not found");
              return;
            }
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            nullable1 = this.GetIntFromToken2(nextToken3);
            if (!nullable1.HasValue)
              nullable1 = new ScriptInt32?((ScriptInt32) 100);
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            int? intFromToken = this.GetIntFromToken(nextToken3);
            nullable2 = intFromToken.HasValue ? new ScriptInt32?((ScriptInt32) intFromToken.GetValueOrDefault()) : new ScriptInt32?();
            if (!nullable2.HasValue)
              nullable2 = new ScriptInt32?((ScriptInt32) 0);
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.SetSphere);
          this.WriteCoord(writer, coord, pointFromToken);
          this.WriteInt32(writer, intFromToken2.Value);
          writer.Write((byte) nullable3.Value);
          this.WriteInt32(writer, nullable1.Value);
          this.WriteInt32(writer, nullable2.Value);
        }
      }
    }

    private void CompileCommandSetSwitch(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        ScriptSwitch scriptSwitch = ScriptSwitch.Toggle;
        switch (nextToken2.Lexeme)
        {
          case "":
          case "toggle":
            if (this.IsTest)
              break;
            this.WriteCommandHeader(writer, ScriptCommand.SetSwitch);
            this.WriteCoord(writer, coord, pointFromToken);
            writer.Write((byte) scriptSwitch);
            break;
          case "on":
            scriptSwitch = ScriptSwitch.On;
            goto case "";
          case "off":
            scriptSwitch = ScriptSwitch.Off;
            goto case "";
          default:
            this.LogTestError("type not found");
            break;
        }
      }
    }

    private void CompileCommandSetText(
      BinaryWriter writer,
      string command,
      string originalCaseCommand,
      int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(originalCaseCommand, nextToken1.EndIndex + 1);
        string lexeme = nextToken2.Lexeme;
        Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
        bool flag = nextToken3.Lexeme == "name";
        ScriptInt32? nullable = new ScriptInt32?();
        if (!flag)
          nullable = this.GetIntFromToken2(nextToken3);
        if (!nullable.HasValue)
          nullable = new ScriptInt32?((ScriptInt32) 0);
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.SetText);
        this.WriteCoord(writer, coord, pointFromToken);
        writer.Write(lexeme);
        writer.Write(flag);
        if (flag)
          return;
        this.WriteInt32(writer, nullable.Value);
      }
    }

    private void CompileCommandSetTexture(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptCoordType coord;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken, out coord);
      if (!pointFromToken.HasValue || coord == ScriptCoordType.None)
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(this.GetNextToken(command, nextToken.EndIndex + 1));
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("texture id not found");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.SetTexture);
          this.WriteCoord(writer, coord, pointFromToken);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandSkill(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      SkillType? skillFromToken = this.GetSkillFromToken(nextToken1);
      if (!skillFromToken.HasValue || skillFromToken.Value == SkillType.Combat || skillFromToken.Value == SkillType.Total)
      {
        this.LogTestError("skill not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag = nextToken2.Lexeme[0] != '+' && nextToken2.Lexeme[0] != '-';
        ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken2);
        if (!intFromToken2.HasValue)
        {
          this.LogTestError("level not found");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Skill);
          writer.Write((byte) skillFromToken.Value);
          writer.Write(flag);
          this.WriteInt32(writer, intFromToken2.Value);
        }
      }
    }

    private void CompileCommandSkillXP(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      SkillType? skillFromToken = this.GetSkillFromToken(nextToken1);
      if (!skillFromToken.HasValue || skillFromToken.Value == SkillType.Combat || skillFromToken.Value == SkillType.Total)
      {
        this.LogTestError("skill not found");
      }
      else
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        bool flag = nextToken2.Lexeme[0] != '+' && nextToken2.Lexeme[0] != '-';
        ScriptDouble? doubleFromToken2 = this.GetDoubleFromToken2(nextToken2);
        if (!doubleFromToken2.HasValue)
        {
          this.LogTestError("xp not found");
        }
        else
        {
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.SkillXP);
          writer.Write((byte) skillFromToken.Value);
          writer.Write(flag);
          this.WriteDouble(writer, doubleFromToken2.Value);
        }
      }
    }

    private void CompileCommandSkyColor(BinaryWriter writer, string command, int index)
    {
      ScriptVector3? nullable1 = new ScriptVector3?();
      ScriptInt32? nullable2 = new ScriptInt32?();
      ScriptInt32? nullable3 = new ScriptInt32?();
      bool flag1 = false;
      bool flag2 = false;
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.Lexeme == "delete")
      {
        nullable1 = new ScriptVector3?((ScriptVector3) Vector3.Zero);
        nullable2 = new ScriptInt32?((ScriptInt32) 0);
        flag1 = true;
      }
      else
      {
        nullable1 = this.GetVector3FromToken(nextToken1);
        if (!nullable1.HasValue)
        {
          this.LogTestError("color [r,g,b] not found");
          return;
        }
      }
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      if (nextToken2.Lexeme == "player")
      {
        flag2 = true;
        nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      if (!flag1)
      {
        nullable2 = this.GetIntFromToken2(nextToken2);
        if (!nullable2.HasValue)
          nullable2 = new ScriptInt32?((ScriptInt32) 100);
        nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      int? intFromToken = this.GetIntFromToken(nextToken2);
      ScriptInt32? nullable4 = intFromToken.HasValue ? new ScriptInt32?((ScriptInt32) intFromToken.GetValueOrDefault()) : new ScriptInt32?();
      if (!nullable4.HasValue)
        nullable4 = new ScriptInt32?((ScriptInt32) 3000);
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.SkyColor);
      writer.Write(flag2);
      this.WriteVector3(writer, nullable1.Value);
      this.WriteInt32(writer, nullable2.Value);
      this.WriteInt32(writer, nullable4.Value);
    }

    private void CompileCommandSound(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string str1 = nextToken1.Lexeme;
      if (str1 == null || str1.Length < 1 || str1 == "none")
      {
        this.LogTestError("name not found");
      }
      else
      {
        bool flag1 = false;
        if (str1.IndexOf(' ') >= 0)
          str1 = str1.Replace(" ", "");
        foreach (AmbientSoundXML ambientSoundXml in Globals1.AmbientSoundData)
        {
          if (ambientSoundXml.IsValid)
          {
            string str2 = ambientSoundXml.Name;
            if (str2.IndexOf(' ') >= 0)
              str2 = str2.Replace(" ", "");
            if (str2.Equals(str1, StringComparison.OrdinalIgnoreCase))
            {
              str1 = ambientSoundXml.CueName;
              flag1 = true;
              break;
            }
          }
        }
        if (!flag1)
        {
          this.LogTestError("name not found");
        }
        else
        {
          ScriptCoordType coord1 = ScriptCoordType.None;
          ScriptCoordType coord2 = ScriptCoordType.None;
          ScriptPoint3D? nullable1 = new ScriptPoint3D?();
          ScriptPoint3D? nullable2 = new ScriptPoint3D?();
          ScriptInt32? nullable3 = new ScriptInt32?();
          ScriptInt32? nullable4 = new ScriptInt32?();
          string s = (string) null;
          bool flag2 = false;
          ScriptInt32 loopDelay = (ScriptInt32) 0;
          ScriptInt32 loopCount = (ScriptInt32) 1;
          Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          ScriptPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken2, out coord1);
          if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
          {
            s = nextToken2.Lexeme;
            if (s.IsEmpty())
            {
              this.LogTestError("[x,y,z] not found");
              return;
            }
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          ScriptPoint3D? pointFromToken2 = this.GetPointFromToken(nextToken3, out coord2);
          if (pointFromToken2.HasValue)
            nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
          bool flag3 = nextToken3.Lexeme == "delete";
          if (!flag3)
          {
            flag2 = nextToken3.Lexeme.StartsWith("loop", StringComparison.OrdinalIgnoreCase);
            if (flag2)
            {
              this.GetLoopParams(nextToken3.Lexeme, out loopDelay, out loopCount);
              nextToken3 = this.GetNextToken(command, nextToken3.EndIndex + 1);
            }
            nullable3 = this.GetIntFromToken2(nextToken3);
            if (!nullable3.HasValue || nullable3.Value.IsZero)
              nullable3 = new ScriptInt32?((ScriptInt32) 100);
            nullable4 = this.GetIntFromToken2(this.GetNextToken(command, nextToken3.EndIndex + 1));
            if (!nullable4.HasValue || nullable4.Value.IsZero)
              nullable4 = new ScriptInt32?((ScriptInt32) 32);
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Sound);
          writer.Write(str1);
          this.WriteCoord(writer, coord1, pointFromToken1);
          if (!pointFromToken1.HasValue || coord1 == ScriptCoordType.None)
            writer.Write(s);
          else
            this.WriteCoord(writer, coord2, pointFromToken2);
          writer.Write(flag3);
          if (flag3)
            return;
          this.WriteInt32(writer, nullable3.Value);
          this.WriteInt32(writer, nullable4.Value);
          writer.Write(flag2);
          if (!flag2)
            return;
          this.WriteInt32(writer, loopDelay);
          this.WriteInt32(writer, loopCount);
        }
      }
    }

    private void GetLoopParams(
      string command,
      out ScriptInt32 loopDelay,
      out ScriptInt32 loopCount)
    {
      loopCount = (ScriptInt32) 1;
      loopDelay = (ScriptInt32) 0;
      command = command.Replace(" ", "");
      string[] tokens = this.GetTokens(command, ',');
      if (tokens == null)
        return;
      for (int index = 0; index < tokens.Length; ++index)
      {
        ScriptInt32? intFromNamedToken2 = this.GetIntFromNamedToken2(tokens[index], "delay");
        if (intFromNamedToken2.HasValue)
        {
          loopDelay = intFromNamedToken2.Value;
        }
        else
        {
          intFromNamedToken2 = this.GetIntFromNamedToken2(tokens[index], "count");
          if (intFromNamedToken2.HasValue)
            loopCount = intFromNamedToken2.Value;
        }
      }
    }

    private void CompileCommandTeleport(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      ScriptTarget scriptTarget = nextToken1.Lexeme == "player" || nextToken1.Lexeme == "actor" ? ScriptTarget.Actor : ScriptTarget.None;
      if (scriptTarget != ScriptTarget.None)
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      ScriptCoordType coord1 = ScriptCoordType.None;
      ScriptCoordType coord2 = ScriptCoordType.None;
      ScriptCoordType coord3 = ScriptCoordType.None;
      ScriptPoint3D? nullable = new ScriptPoint3D?();
      ScriptPoint3D? p1 = new ScriptPoint3D?();
      ScriptPoint3D? p2 = new ScriptPoint3D?();
      bool flag = false;
      ScriptPoint3D? pointFromToken = this.GetPointFromToken(nextToken1, out coord1);
      if (!pointFromToken.HasValue || coord1 == ScriptCoordType.None)
      {
        if (scriptTarget != ScriptTarget.None)
          this.LogTestError("destination [x,y,z] not found");
        else
          this.LogTestError("min [x,y,z] not found");
      }
      else
      {
        if (scriptTarget == ScriptTarget.None)
        {
          Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
          p1 = this.GetPointFromToken(nextToken2, out coord2);
          if (!p1.HasValue || coord2 == ScriptCoordType.None)
          {
            this.LogTestError("max [x,y,z] not found");
            return;
          }
          Parser.Token nextToken3 = this.GetNextToken(command, nextToken2.EndIndex + 1);
          p2 = this.GetPointFromToken(nextToken3, out coord3);
          if (!p2.HasValue || coord3 == ScriptCoordType.None)
          {
            this.LogTestError("destination [x,y,z] not found");
            return;
          }
          flag = this.GetNextToken(command, nextToken3.EndIndex + 1).Lexeme == "relative";
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Teleport);
        writer.Write((byte) scriptTarget);
        this.WriteCoord(writer, coord1, pointFromToken);
        if (scriptTarget == ScriptTarget.Actor)
          return;
        this.WriteCoord(writer, coord2, p1);
        this.WriteCoord(writer, coord3, p2);
        writer.Write(flag);
      }
    }

    private void CompileCommandTintColor(BinaryWriter writer, string command, int index)
    {
      ScriptVector3? nullable1 = new ScriptVector3?();
      ScriptInt32? nullable2 = new ScriptInt32?();
      bool flag = false;
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.Lexeme == "delete")
      {
        nullable1 = new ScriptVector3?((ScriptVector3) new Vector3(100f, 100f, 100f));
      }
      else
      {
        nullable1 = this.GetVector3FromToken(nextToken1);
        if (!nullable1.HasValue)
        {
          this.LogTestError("tint [r,g,b] not recognized");
          return;
        }
      }
      Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      if (nextToken2.Lexeme == "player")
      {
        flag = true;
        nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      ScriptInt32? nullable3 = this.GetIntFromToken2(nextToken2);
      if (!nullable3.HasValue)
        nullable3 = new ScriptInt32?((ScriptInt32) 3000);
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.TintColor);
      writer.Write(flag);
      this.WriteVector3(writer, nullable1.Value);
      this.WriteInt32(writer, nullable3.Value);
    }

    private void CompileCommandUnequip(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      InventoryHand inventoryHand = InventoryHand.None;
      bool flag = false;
      if (nextToken.Lexeme == "left")
      {
        inventoryHand = InventoryHand.Left;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "right")
      {
        inventoryHand = InventoryHand.Right;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      else if (nextToken.Lexeme == "body")
      {
        flag = true;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
      }
      Item? nullable = this.GetItemIDFromToken(nextToken);
      if (nullable.HasValue && nullable.Value == Item.None)
        nullable = new Item?();
      if (this.IsTest)
        return;
      this.WriteCommandHeader(writer, ScriptCommand.Unequip);
      writer.Write((byte) inventoryHand);
      writer.Write(flag);
      writer.Write(nullable.HasValue ? (ushort) nullable.Value : (ushort) 0);
    }

    private int GetVarIndex(string varName)
    {
      for (int index = 0; index < this.varCount; ++index)
      {
        if (varName.Equals(this.varNames[index], StringComparison.OrdinalIgnoreCase))
          return index;
      }
      return -1;
    }

    private bool IsValidVarName(string name)
    {
      if (name.Length < 1)
        return false;
      for (int index = 0; index < ScriptCompiler.invalidVarChars.Length; ++index)
      {
        if (name.IndexOf(ScriptCompiler.invalidVarChars[index]) >= 0)
          return false;
      }
      for (int index = 0; index < ScriptCompiler.invalidVarNames.Length; ++index)
      {
        if (name == ScriptCompiler.invalidVarNames[index])
          return false;
      }
      return true;
    }

    private int AddVariable(string name)
    {
      if (this.varNames == null)
        this.varNames = new string[256];
      else if (this.varCount >= 256)
      {
        this.LogTestError("Maximum number of variables defined");
        return -1;
      }
      this.varNames[this.varCount++] = name;
      return this.varCount - 1;
    }

    private void CompileCommandVar(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      string lexeme = nextToken1.Lexeme;
      if (lexeme == null || lexeme.Length < 1 || !this.IsValidVarName(lexeme))
      {
        this.LogTestError("invalid variable name. variable names cannot contain spaces or quotes or any of the following characters:~`!@#$%^&*()-=+|\\/?<>,.");
      }
      else
      {
        int endIndex = nextToken1.EndIndex;
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        if (command.IndexOf('=', endIndex + 1, nextToken2.StartIndex - endIndex - 1) < 0)
        {
          this.CompileCommandVarDec(writer, command, index);
        }
        else
        {
          int num1 = this.GetVarIndex(lexeme);
          if (num1 < 0)
          {
            num1 = this.AddVariable(lexeme);
            if (num1 < 0)
              return;
          }
          List<ScriptCompiler.VarOp> varOpList = new List<ScriptCompiler.VarOp>();
          ScriptCompiler.VarOp op = new ScriptCompiler.VarOp();
          while (nextToken2.StartIndex < nextToken2.EndIndex && varOpList.Count < 256)
          {
            double result1;
            if (double.TryParse(nextToken2.Lexeme, out result1))
            {
              op.ValueType = ScriptValueType.NumLiterial;
              op.ValueNum = result1;
            }
            else if (nextToken2.Lexeme.StartsWith("history:"))
            {
              op.ValueType = ScriptValueType.HistoryKey;
              op.ValueString = nextToken2.Lexeme.Substring(8);
            }
            else if (nextToken2.Lexeme.StartsWith("syshistory:"))
            {
              op.ValueType = ScriptValueType.SysHistoryKey;
              op.ValueString = nextToken2.Lexeme.Substring(11);
            }
            else if (nextToken2.Lexeme.StartsWith("clanhistory:"))
            {
              op.ValueType = ScriptValueType.ClanHistoryKey;
              op.ValueString = nextToken2.Lexeme.Substring(12);
            }
            else if (nextToken2.Lexeme.StartsWith("skill:"))
            {
              op.ValueType = ScriptValueType.Skill;
              SkillType? skillFromToken = this.GetSkillFromToken(new Parser.Token()
              {
                Lexeme = nextToken2.Lexeme.Substring(6)
              });
              if (skillFromToken.HasValue)
              {
                op.ValueNum = (double) skillFromToken.Value;
              }
              else
              {
                this.LogTestError("could not evaluate [skill:x] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("skillxp:"))
            {
              op.ValueType = ScriptValueType.SkillXP;
              SkillType? skillFromToken = this.GetSkillFromToken(new Parser.Token()
              {
                Lexeme = nextToken2.Lexeme.Substring(8)
              });
              if (skillFromToken.HasValue)
              {
                op.ValueNum = (double) skillFromToken.Value;
              }
              else
              {
                this.LogTestError("could not evaluate [skillxp:x] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("rand:"))
            {
              op.ValueType = ScriptValueType.Random;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(5);
              int result2;
              if (int.TryParse(str, out result2))
              {
                op.ValueNum = (double) result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [rand:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("abs:"))
            {
              op.ValueType = ScriptValueType.Abs;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [abs:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("single:"))
            {
              op.ValueType = ScriptValueType.Single;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(7);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [single:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("int:"))
            {
              op.ValueType = ScriptValueType.Int;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [int:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("sin:"))
            {
              op.ValueType = ScriptValueType.Sin;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [sin:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("cos:"))
            {
              op.ValueType = ScriptValueType.Cos;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [cos:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("tan:"))
            {
              op.ValueType = ScriptValueType.Tan;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [tan:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("sqrt:"))
            {
              op.ValueType = ScriptValueType.Sqrt;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(5);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [sqrt:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("neg:") || nextToken2.Lexeme.StartsWith("-"))
            {
              op.ValueType = ScriptValueType.Neg;
              op.VarIndex = -1;
              string str = nextToken2.Lexeme.Substring(nextToken2.Lexeme[0] == '-' ? 1 : 4);
              double result2;
              if (double.TryParse(str, out result2))
              {
                op.ValueNum = result2;
              }
              else
              {
                int varIndex = this.GetVarIndex(str);
                if (varIndex >= 0)
                {
                  op.VarIndex = varIndex;
                }
                else
                {
                  this.LogTestError("could not evaluate [neg:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("aux:"))
            {
              op.ValueType = ScriptValueType.Aux;
              string s = nextToken2.Lexeme.Substring(4);
              op.OpPoint1 = this.GetPointFromToken(s, out op.OpCoord1);
              if (!op.OpPoint1.HasValue || op.OpCoord1 == ScriptCoordType.None)
              {
                this.LogTestError("could not evaluate coord in [aux:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("sunlight:"))
            {
              op.ValueType = ScriptValueType.SunLight;
              string s = nextToken2.Lexeme.Substring(9);
              op.OpPoint1 = this.GetPointFromToken(s, out op.OpCoord1);
              if (!op.OpPoint1.HasValue || op.OpCoord1 == ScriptCoordType.None)
              {
                this.LogTestError("could not evaluate coord in [sunlight:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("blocklight:"))
            {
              op.ValueType = ScriptValueType.BlockLight;
              string s = nextToken2.Lexeme.Substring(11);
              op.OpPoint1 = this.GetPointFromToken(s, out op.OpCoord1);
              if (!op.OpPoint1.HasValue || op.OpCoord1 == ScriptCoordType.None)
              {
                this.LogTestError("could not evaluate coord in [blocklight:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("light:"))
            {
              op.ValueType = ScriptValueType.Light;
              string s = nextToken2.Lexeme.Substring(6);
              op.OpPoint1 = this.GetPointFromToken(s, out op.OpCoord1);
              if (!op.OpPoint1.HasValue || op.OpCoord1 == ScriptCoordType.None)
              {
                this.LogTestError("could not evaluate coord in [light:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("block:"))
            {
              op.ValueType = ScriptValueType.BlockID;
              string s = nextToken2.Lexeme.Substring(6);
              op.OpPoint1 = this.GetPointFromToken(s, out op.OpCoord1);
              if (!op.OpPoint1.HasValue || op.OpCoord1 == ScriptCoordType.None)
              {
                this.LogTestError("could not evaluate coord in [block:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("inv:"))
            {
              op.ValueType = ScriptValueType.Inv;
              bool flag = false;
              Parser.Token nextToken3 = this.GetNextToken(nextToken2.Lexeme, 0);
              string token;
              if (nextToken3.IsEmpty)
              {
                int num2 = nextToken2.Lexeme.IndexOf(':', 4);
                flag = num2 < 0;
                token = nextToken2.Lexeme.Substring(4, flag ? nextToken2.Lexeme.Length - 4 : num2 - 4);
              }
              else
              {
                token = nextToken3.Lexeme;
                nextToken3 = this.GetNextToken(nextToken2.Lexeme, nextToken3.EndIndex + 1);
              }
              op.OpItemID = this.GetItemIDFromToken(token, false, false, true);
              if (!op.OpItemID.HasValue)
              {
                this.LogTestError("could not evaluate item in [inv:] expression");
                return;
              }
              if (!flag)
              {
                op.OpPoint1 = this.GetPointFromToken(nextToken3, out op.OpCoord1);
                if (!op.OpPoint1.HasValue)
                {
                  this.LogTestError("could not evaluate coord in [inv:] expression");
                  return;
                }
              }
            }
            else if (nextToken2.Lexeme.StartsWith("distance:"))
            {
              op.ValueType = ScriptValueType.Distance;
              Parser.Token nextToken3 = this.GetNextToken(nextToken2.Lexeme, 0);
              op.OpVector1 = this.GetVector3FromToken(nextToken3, out op.OpCoord1);
              if (!op.OpVector1.HasValue)
              {
                this.LogTestError("could not evaluate first coord in [distance:] expression");
                return;
              }
              Parser.Token nextToken4 = this.GetNextToken(nextToken2.Lexeme, nextToken3.EndIndex + 1);
              op.OpVector2 = this.GetVector3FromToken(nextToken4, out op.OpCoord2);
              if (!op.OpVector2.HasValue)
              {
                this.LogTestError("could not evaluate second coord in [distance:] expression");
                return;
              }
            }
            else if (nextToken2.Lexeme.StartsWith("hash:"))
            {
              op.ValueType = ScriptValueType.Hash;
              op.ValueString = nextToken2.Lexeme.Substring(5);
            }
            else if (nextToken2.Lexeme.StartsWith("gamercount:"))
            {
              op.ValueType = ScriptValueType.GamerCount;
              this.ParseGamerCount(nextToken2.Lexeme, 11, ref op);
            }
            else if (nextToken2.Lexeme.StartsWith("npccount:") || nextToken2.Lexeme.StartsWith("mobcount:"))
            {
              op.ValueType = ScriptValueType.NpcCount;
              this.ParseNpcCount(nextToken2.Lexeme, 9, ref op);
            }
            else if (nextToken2.Lexeme.StartsWith("clock:"))
              op.ValueType = ScriptValueType.Clock;
            else if (nextToken2.Lexeme == "pos:x" || nextToken2.Lexeme == "eye:x")
              op.ValueType = ScriptValueType.PosX;
            else if (nextToken2.Lexeme == "pos:y")
              op.ValueType = ScriptValueType.PosY;
            else if (nextToken2.Lexeme == "pos:z" || nextToken2.Lexeme == "eye:z")
              op.ValueType = ScriptValueType.PosZ;
            else if (nextToken2.Lexeme == "eye:y")
              op.ValueType = ScriptValueType.EyeY;
            else if (nextToken2.Lexeme == "vel:x")
              op.ValueType = ScriptValueType.VelX;
            else if (nextToken2.Lexeme == "vel:y")
              op.ValueType = ScriptValueType.VelY;
            else if (nextToken2.Lexeme == "vel:z")
              op.ValueType = ScriptValueType.VelZ;
            else if (nextToken2.Lexeme == "view:x")
              op.ValueType = ScriptValueType.ViewX;
            else if (nextToken2.Lexeme == "view:y")
              op.ValueType = ScriptValueType.ViewY;
            else if (nextToken2.Lexeme == "view:z")
              op.ValueType = ScriptValueType.ViewZ;
            else if (nextToken2.Lexeme == "rel:x")
              op.ValueType = ScriptValueType.RelX;
            else if (nextToken2.Lexeme == "rel:y")
              op.ValueType = ScriptValueType.RelY;
            else if (nextToken2.Lexeme == "rel:z")
              op.ValueType = ScriptValueType.RelZ;
            else if (nextToken2.Lexeme == "prel:x")
              op.ValueType = ScriptValueType.PRelX;
            else if (nextToken2.Lexeme == "prel:y")
              op.ValueType = ScriptValueType.PRelY;
            else if (nextToken2.Lexeme == "prel:z")
              op.ValueType = ScriptValueType.PRelZ;
            else if (nextToken2.Lexeme == "crel:x")
              op.ValueType = ScriptValueType.CRelX;
            else if (nextToken2.Lexeme == "crel:y")
              op.ValueType = ScriptValueType.CRelY;
            else if (nextToken2.Lexeme == "crel:z")
              op.ValueType = ScriptValueType.CRelZ;
            else if (nextToken2.Lexeme == "script:x")
              op.ValueType = ScriptValueType.ScriptX;
            else if (nextToken2.Lexeme == "script:y")
              op.ValueType = ScriptValueType.ScriptY;
            else if (nextToken2.Lexeme == "script:z")
              op.ValueType = ScriptValueType.ScriptZ;
            else if (nextToken2.Lexeme == "health:")
              op.ValueType = ScriptValueType.Health;
            else if (nextToken2.Lexeme == "maxhealth:")
              op.ValueType = ScriptValueType.MaxHealth;
            else if (nextToken2.Lexeme == "reach:")
              op.ValueType = ScriptValueType.Reach;
            else if (nextToken2.Lexeme == "pi:")
            {
              op.ValueType = ScriptValueType.Pi;
            }
            else
            {
              int varIndex = this.GetVarIndex(nextToken2.Lexeme);
              if (varIndex >= 0)
              {
                op.ValueType = ScriptValueType.Variable;
                op.ValueNum = (double) varIndex;
              }
              else
              {
                this.LogTestError("could not evaluate expression");
                return;
              }
            }
            varOpList.Add(op);
            int startIndex = nextToken2.EndIndex + 1;
            nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            if (nextToken2.StartIndex < nextToken2.EndIndex)
            {
              string str1 = nextToken2.Lexeme;
              bool flag = false;
              if (nextToken2.StartIndex > startIndex)
              {
                string str2 = command.Substring(startIndex, nextToken2.StartIndex - startIndex - 1).Trim();
                if (str2.Length > 0)
                {
                  str1 = str2;
                  flag = true;
                }
              }
              switch (str1)
              {
                case "+":
                  op.OpType = ScriptVarOp.Addition;
                  break;
                case "-":
                  op.OpType = ScriptVarOp.Subtraction;
                  break;
                case "*":
                  op.OpType = ScriptVarOp.Multiplication;
                  break;
                case "/":
                  op.OpType = ScriptVarOp.Division;
                  break;
                case "%":
                  op.OpType = ScriptVarOp.Modulus;
                  break;
                default:
                  op.OpType = ScriptVarOp.Assignment;
                  break;
              }
              if (!flag)
                nextToken2 = this.GetNextToken(command, nextToken2.EndIndex + 1);
            }
            else
              break;
          }
          if (this.IsTest)
            return;
          this.WriteCommandHeader(writer, ScriptCommand.Var);
          writer.Write((ushort) num1);
          writer.Write((byte) varOpList.Count);
          for (int index1 = 0; index1 < varOpList.Count; ++index1)
          {
            op = varOpList[index1];
            writer.Write((byte) op.ValueType);
            switch (op.ValueType)
            {
              case ScriptValueType.NumLiterial:
                writer.Write(op.ValueNum);
                goto case ScriptValueType.Health;
              case ScriptValueType.Variable:
              case ScriptValueType.Skill:
              case ScriptValueType.SkillXP:
                writer.Write((ushort) op.ValueNum);
                goto case ScriptValueType.Health;
              case ScriptValueType.Random:
                bool flag1 = op.VarIndex >= 0;
                writer.Write(flag1 ? (byte) 1 : (byte) 0);
                if (flag1)
                {
                  writer.Write((ushort) op.VarIndex);
                  goto case ScriptValueType.Health;
                }
                else
                {
                  writer.Write((int) op.ValueNum);
                  goto case ScriptValueType.Health;
                }
              case ScriptValueType.Health:
              case ScriptValueType.MaxHealth:
              case ScriptValueType.Reach:
              case ScriptValueType.PosX:
              case ScriptValueType.PosY:
              case ScriptValueType.PosZ:
              case ScriptValueType.EyeY:
              case ScriptValueType.VelX:
              case ScriptValueType.VelY:
              case ScriptValueType.VelZ:
              case ScriptValueType.ViewX:
              case ScriptValueType.ViewY:
              case ScriptValueType.ViewZ:
              case ScriptValueType.RelX:
              case ScriptValueType.RelY:
              case ScriptValueType.RelZ:
              case ScriptValueType.CRelX:
              case ScriptValueType.CRelY:
              case ScriptValueType.CRelZ:
              case ScriptValueType.PRelX:
              case ScriptValueType.PRelY:
              case ScriptValueType.PRelZ:
              case ScriptValueType.ScriptX:
              case ScriptValueType.ScriptY:
              case ScriptValueType.ScriptZ:
              case ScriptValueType.Clock:
              case ScriptValueType.Pi:
                writer.Write((byte) op.OpType);
                continue;
              case ScriptValueType.Abs:
              case ScriptValueType.Sin:
              case ScriptValueType.Cos:
              case ScriptValueType.Tan:
              case ScriptValueType.Sqrt:
              case ScriptValueType.Single:
              case ScriptValueType.Int:
              case ScriptValueType.Neg:
                bool flag2 = op.VarIndex >= 0;
                writer.Write(flag2 ? (byte) 1 : (byte) 0);
                if (flag2)
                {
                  writer.Write((ushort) op.VarIndex);
                  goto case ScriptValueType.Health;
                }
                else
                {
                  writer.Write(op.ValueNum);
                  goto case ScriptValueType.Health;
                }
              case ScriptValueType.Inv:
                writer.Write((ushort) op.OpItemID.Value);
                writer.Write(op.OpPoint1.HasValue);
                if (op.OpPoint1.HasValue)
                {
                  this.WriteCoord(writer, op.OpCoord1, op.OpPoint1);
                  goto case ScriptValueType.Health;
                }
                else
                  goto case ScriptValueType.Health;
              case ScriptValueType.Distance:
                this.WriteVector3(writer, op.OpCoord1, op.OpVector1);
                this.WriteVector3(writer, op.OpCoord2, op.OpVector2);
                goto case ScriptValueType.Health;
              case ScriptValueType.GamerCount:
              case ScriptValueType.NpcCount:
                writer.Write((byte) op.OPInt1.Value.I);
                this.WriteCoord(writer, op.OpCoord1, op.OpPoint1);
                if (op.OpCoord1 != ScriptCoordType.None)
                {
                  this.WriteCoord(writer, op.OpCoord2, op.OpPoint2);
                  if (op.OpCoord2 == ScriptCoordType.None)
                  {
                    this.WriteSingle(writer, op.OPSingle1.Value);
                    goto case ScriptValueType.Health;
                  }
                  else
                    goto case ScriptValueType.Health;
                }
                else
                {
                  this.WriteString(writer, op.ValueString);
                  goto case ScriptValueType.Health;
                }
              case ScriptValueType.Light:
              case ScriptValueType.BlockID:
              case ScriptValueType.Aux:
              case ScriptValueType.SunLight:
              case ScriptValueType.BlockLight:
                this.WriteCoord(writer, op.OpCoord1, op.OpPoint1);
                goto case ScriptValueType.Health;
              default:
                writer.Write(op.ValueString);
                goto case ScriptValueType.Health;
            }
          }
        }
      }
    }

    private void CompileCommandVarDec(BinaryWriter writer, string command, int index)
    {
      for (Parser.Token nextToken = this.GetNextToken(command, index); nextToken.StartIndex < nextToken.EndIndex && this.varCount < 256; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        string lexeme = nextToken.Lexeme;
        if (lexeme == null || lexeme.Length < 1 || !this.IsValidVarName(lexeme))
        {
          this.LogTestError("invalid variable name. variable names cannot contain spaces or quotes or any of the following characters:~`!@#$%^&*()-=+|\\/?<>,.");
          break;
        }
        if (this.GetVarIndex(lexeme) < 0 && this.AddVariable(lexeme) < 0)
          break;
      }
    }

    private void ParseGamerCount(string command, int index, ref ScriptCompiler.VarOp op)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.IsEmpty)
        nextToken1 = this.GetNextToken(command, index, char.MinValue, char.MinValue);
      op.OpPoint1 = this.GetPointFromToken(nextToken1, out op.OpCoord1);
      if (op.OpPoint1.HasValue && op.OpCoord1 != ScriptCoordType.None)
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        op.OpPoint2 = this.GetPointFromToken(nextToken2, out op.OpCoord2);
        if (!op.OpPoint2.HasValue || op.OpCoord2 == ScriptCoordType.None)
        {
          op.OPSingle1 = this.GetFloatFromToken2(nextToken2);
          if (!op.OPSingle1.HasValue)
          {
            this.LogTestError("radius or second [x,y,z] not found");
            return;
          }
        }
        nextToken1 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      else if (!nextToken1.IsEmpty && nextToken1.Lexeme != "local" && (nextToken1.Lexeme != "remote" && nextToken1.Lexeme != "all"))
      {
        op.ValueString = nextToken1.Lexeme;
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      }
      ScriptGamerTarget scriptGamerTarget = nextToken1.Lexeme == "local" ? ScriptGamerTarget.Local : (nextToken1.Lexeme == "remote" ? ScriptGamerTarget.Remote : ScriptGamerTarget.All);
      op.OPInt1 = new ScriptInt32?((ScriptInt32) ((int) scriptGamerTarget));
    }

    private void ParseNpcCount(string command, int index, ref ScriptCompiler.VarOp op)
    {
      Parser.Token nextToken1 = this.GetNextToken(command, index);
      if (nextToken1.IsEmpty)
        nextToken1 = this.GetNextToken(command, index, char.MinValue, char.MinValue);
      op.OpPoint1 = this.GetPointFromToken(nextToken1, out op.OpCoord1);
      if (op.OpPoint1.HasValue && op.OpCoord1 != ScriptCoordType.None)
      {
        Parser.Token nextToken2 = this.GetNextToken(command, nextToken1.EndIndex + 1);
        op.OpPoint2 = this.GetPointFromToken(nextToken2, out op.OpCoord2);
        if (!op.OpPoint2.HasValue || op.OpCoord2 == ScriptCoordType.None)
        {
          op.OPSingle1 = this.GetFloatFromToken2(nextToken2);
          if (!op.OPSingle1.HasValue)
          {
            this.LogTestError("radius or second [x,y,z] not found");
            return;
          }
        }
        nextToken1 = this.GetNextToken(command, nextToken2.EndIndex + 1);
      }
      else if (!this.GetActorTypeFromToken(nextToken1).HasValue)
      {
        op.ValueString = nextToken1.Lexeme;
        nextToken1 = this.GetNextToken(command, nextToken1.EndIndex + 1);
      }
      ActorType? actorTypeFromToken = this.GetActorTypeFromToken(nextToken1);
      op.OPInt1 = new ScriptInt32?((ScriptInt32) (actorTypeFromToken.HasValue ? (int) actorTypeFromToken.Value : 0));
    }

    private void CompileCommandWait(BinaryWriter writer, string command, int index)
    {
      ScriptInt32? intFromToken2 = this.GetIntFromToken2(this.GetNextToken(command, index));
      if (!intFromToken2.HasValue || intFromToken2.Value.IsZero)
      {
        this.LogTestError("[millisecs] not found");
      }
      else
      {
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Wait);
        this.WriteInt32(writer, intFromToken2.Value);
      }
    }

    private void CompileCommandWaypoint(BinaryWriter writer, string command, int index)
    {
      Parser.Token nextToken = this.GetNextToken(command, index);
      bool flag = nextToken.Lexeme == "delete";
      ScriptCoordType coord = ScriptCoordType.None;
      ScriptPoint3D? p = flag ? new ScriptPoint3D?() : this.GetPoint2DFromToken(nextToken, out coord);
      if (!flag && (!p.HasValue || coord == ScriptCoordType.None))
      {
        this.LogTestError("[x,y,z] not found");
      }
      else
      {
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Waypoint);
        this.WriteCoord2D(writer, coord, p);
      }
    }

    private void CompileCommandZone(
      BinaryWriter writer,
      string command,
      string origCommand,
      int index)
    {
      Parser.Token nextToken = this.GetNextToken(origCommand, index);
      string lexeme = nextToken.Lexeme;
      if (lexeme == null || lexeme.Length < 1)
      {
        this.LogTestError("zone name not found");
      }
      else
      {
        ScriptCompiler.ScriptZoneParams zone = (ScriptCompiler.ScriptZoneParams) null;
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
        bool flag1 = nextToken.Lexeme == "temp";
        if (flag1)
          nextToken = this.GetNextToken(command, nextToken.EndIndex + 1);
        bool flag2 = nextToken.Lexeme == "delete";
        if (!flag2)
        {
          zone = new ScriptCompiler.ScriptZoneParams();
          for (; !nextToken.Lexeme.IsEmpty(); nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
            this.GetZoneParam(zone, nextToken);
        }
        if (this.IsTest)
          return;
        this.WriteCommandHeader(writer, ScriptCommand.Zone);
        writer.Write(flag1);
        writer.Write(lexeme);
        writer.Write(flag2);
        if (flag2)
          return;
        writer.Write(zone.TypeSpawn.HasValue);
        if (zone.TypeSpawn.HasValue)
          writer.Write(zone.TypeSpawn.Value);
        writer.Write(zone.TypeNoEdit.HasValue);
        if (zone.TypeNoEdit.HasValue)
          writer.Write(zone.TypeNoEdit.Value);
        writer.Write(zone.TypeNoCombat.HasValue);
        if (zone.TypeNoCombat.HasValue)
          writer.Write(zone.TypeNoCombat.Value);
        writer.Write(zone.TypeNoFly.HasValue);
        if (zone.TypeNoFly.HasValue)
          writer.Write(zone.TypeNoFly.Value);
        writer.Write(zone.TypeNoMobs.HasValue);
        if (zone.TypeNoMobs.HasValue)
          writer.Write(zone.TypeNoMobs.Value);
        writer.Write(zone.TypeNoEscape.HasValue);
        if (zone.TypeNoEscape.HasValue)
          writer.Write(zone.TypeNoEscape.Value);
        this.WriteCoord(writer, zone.MinCoord, zone.Min);
        this.WriteCoord(writer, zone.MaxCoord, zone.Max);
        writer.Write(zone.BuilderType.HasValue);
        if (zone.BuilderType.HasValue)
          writer.Write((byte) zone.BuilderType.Value);
        writer.Write(zone.OnEntryScriptName != null);
        if (zone.OnEntryScriptName != null)
          writer.Write(zone.OnEntryScriptName);
        writer.Write(zone.OnExitScriptName != null);
        if (zone.OnExitScriptName != null)
          writer.Write(zone.OnExitScriptName);
        writer.Write(zone.CombatLevelDifference.HasValue);
        if (zone.CombatLevelDifference.HasValue)
          this.WriteInt32(writer, zone.CombatLevelDifference.Value);
        writer.Write(zone.SpeedMultiplier.HasValue);
        if (zone.SpeedMultiplier.HasValue)
          this.WriteSingle(writer, zone.SpeedMultiplier.Value);
        writer.Write(zone.GravityMultiplier.HasValue);
        if (!zone.GravityMultiplier.HasValue)
          return;
        this.WriteSingle(writer, zone.GravityMultiplier.Value);
      }
    }

    private void GetZoneParam(ScriptCompiler.ScriptZoneParams zone, Parser.Token token)
    {
      int length = token.Lexeme.IndexOf('=');
      if (length >= 0)
      {
        string str = token.Lexeme.Substring(0, length);
        string token1 = token.Lexeme.Substring(length + 1, token.Lexeme.Length - length - 1);
        switch (str)
        {
          case "fly":
            zone.TypeNoFly = new bool?(token1 == "off");
            break;
          case "pvp":
            zone.TypeNoCombat = new bool?(token1 == "off");
            break;
          case "npc":
          case "mobs":
            zone.TypeNoMobs = new bool?(token1 == "off");
            break;
          case "edit":
            zone.TypeNoEdit = new bool?(token1 == "off");
            break;
          case "escape":
            zone.TypeNoEscape = new bool?(token1 == "off");
            break;
          case "entry":
            zone.OnEntryScriptName = token1;
            break;
          case "exit":
            zone.OnExitScriptName = token1;
            break;
          case "builder":
            zone.BuilderType = new ZoneBuilderType?(token1 == "player" ? ZoneBuilderType.Player : (token1 == "clan" ? ZoneBuilderType.Clan : ZoneBuilderType.None));
            break;
          case "cld":
            zone.CombatLevelDifference = this.GetIntFromToken2(token1);
            break;
          case "speed":
            zone.SpeedMultiplier = this.GetFloatFromToken2(token1);
            break;
          case "gravity":
            zone.GravityMultiplier = this.GetFloatFromToken2(token1);
            break;
          default:
            this.LogTestError("zone parameter [" + str + "] not found");
            break;
        }
      }
      else
      {
        ScriptCoordType coord;
        ScriptPoint3D? pointFromToken = this.GetPointFromToken(token, out coord);
        if (!pointFromToken.HasValue)
          return;
        if (!zone.MinIsSet)
        {
          zone.MinCoord = coord;
          zone.Min = pointFromToken;
          zone.MinIsSet = true;
        }
        else
        {
          zone.MaxCoord = coord;
          zone.Max = pointFromToken;
        }
      }
    }

    protected void SetMinMax(GlobalPoint3D? nmin, GlobalPoint3D? nmax)
    {
      if (!nmin.HasValue || !nmax.HasValue)
        return;
      GlobalPoint3D min = nmin.Value;
      GlobalPoint3D max = nmax.Value;
      this.SetMinMax(ref min, ref max);
      nmin = new GlobalPoint3D?(min);
      nmax = new GlobalPoint3D?(max);
    }

    private ScriptInt32? GetIntFromToken2(Parser.Token token)
    {
      return this.GetIntFromToken2(token.Lexeme);
    }

    private ScriptInt32? GetIntFromToken2(string token)
    {
      if (token.IndexOf(',') >= 0)
        return new ScriptInt32?();
      int result;
      if (int.TryParse(token, out result))
        return new ScriptInt32?(new ScriptInt32()
        {
          I = result
        });
      int varIndex = this.GetVarIndex(token);
      if (varIndex >= 0)
        return new ScriptInt32?(new ScriptInt32()
        {
          I = varIndex,
          T = ScriptValueType.Variable
        });
      if (this.IsValidVarName(token))
        this.LogTestWarning("possible undeclared variable: [" + token + "]. if this is a variable, ensure there is a preceding var [" + token + "] command");
      return new ScriptInt32?();
    }

    public ScriptInt32? GetIntOrPercentFromToken2(Parser.Token token, out bool isPercent)
    {
      isPercent = false;
      if (token.Lexeme.Length <= 0 || token.Lexeme[token.Lexeme.Length - 1] != '%')
        return this.GetIntFromToken2(token);
      isPercent = true;
      return this.GetIntFromToken2(token.Lexeme.Substring(0, token.Lexeme.Length - 1));
    }

    private ScriptInt32? GetIntFromNamedToken2(Parser.Token token, string name)
    {
      return this.GetIntFromNamedToken2(token.Lexeme, name);
    }

    public ScriptInt32? GetIntFromNamedToken2(string token, string name)
    {
      int indexStart;
      int length;
      this.GetNamedToken(token, name, out indexStart, out length);
      if (indexStart >= 0 && length > 0)
        return this.GetIntFromToken2(token.Substring(indexStart, length));
      return new ScriptInt32?();
    }

    private ScriptSingle? GetFloatFromNamedToken2(Parser.Token token, string name)
    {
      return this.GetFloatFromNamedToken2(token.Lexeme, name);
    }

    private ScriptSingle? GetFloatFromNamedToken2(string token, string name)
    {
      int indexStart;
      int length;
      this.GetNamedToken(token, name, out indexStart, out length);
      if (indexStart >= 0 && length > 0)
        return this.GetFloatFromToken2(token.Substring(indexStart, length));
      return new ScriptSingle?();
    }

    private ScriptSingle? GetFloatFromToken2(Parser.Token token)
    {
      return this.GetFloatFromToken2(token.Lexeme);
    }

    private ScriptSingle? GetFloatFromToken2(string token)
    {
      if (token.IndexOf(',') >= 0)
        return new ScriptSingle?();
      float result;
      if (float.TryParse(token, out result))
        return new ScriptSingle?(new ScriptSingle()
        {
          I = result
        });
      int varIndex = this.GetVarIndex(token);
      if (varIndex >= 0)
        return new ScriptSingle?(new ScriptSingle()
        {
          I = (float) varIndex,
          T = ScriptValueType.Variable
        });
      if (this.IsValidVarName(token))
        this.LogTestWarning("possible undeclared variable: [" + token + "]. if this is a variable, ensure there is a preceding var [" + token + "] command");
      return new ScriptSingle?();
    }

    private ScriptDouble? GetDoubleFromToken2(Parser.Token token)
    {
      return this.GetDoubleFromToken2(token.Lexeme);
    }

    private ScriptDouble? GetDoubleFromToken2(string token)
    {
      if (token.IndexOf(',') >= 0)
        return new ScriptDouble?();
      double result;
      if (double.TryParse(token, out result))
        return new ScriptDouble?(new ScriptDouble()
        {
          I = result
        });
      int varIndex = this.GetVarIndex(token);
      if (varIndex >= 0)
        return new ScriptDouble?(new ScriptDouble()
        {
          I = (double) varIndex,
          T = ScriptValueType.Variable
        });
      if (this.IsValidVarName(token))
        this.LogTestWarning("possible undeclared variable: [" + token + "]. if this is a variable, ensure there is a preceding var [" + token + "] command");
      return new ScriptDouble?();
    }

    private ScriptPoint3D? GetPointFromToken(Parser.Token token)
    {
      ScriptCoordType coord;
      return this.GetPointFromToken(token, out coord);
    }

    private ScriptPoint3D? GetPointFromToken(
      Parser.Token token,
      out ScriptCoordType coord)
    {
      return this.GetPointFromToken(token.Lexeme, out coord);
    }

    private ScriptPoint3D? GetPointFromToken(string s, out ScriptCoordType coord)
    {
      coord = ScriptCoordType.None;
      if (s.Length <= 4 || s.IndexOf(',') == s.LastIndexOf(','))
        return new ScriptPoint3D?();
      if (s.StartsWith("rel:"))
      {
        coord = ScriptCoordType.Relative;
        s = s.Substring(4);
      }
      else if (s.StartsWith("prel:"))
      {
        coord = ScriptCoordType.PlayerRelative;
        s = s.Substring(5);
      }
      else if (s.StartsWith("crel:"))
      {
        coord = ScriptCoordType.CursorRelative;
        s = s.Substring(5);
      }
      else if (s.StartsWith("vrel:"))
      {
        coord = ScriptCoordType.ViewRelative;
        s = s.Substring(5);
      }
      else if (s.StartsWith("hvrel:"))
      {
        coord = ScriptCoordType.ViewHorizRelative;
        s = s.Substring(6);
      }
      else if (s.StartsWith("trel:"))
      {
        coord = ScriptCoordType.TargetRelative;
        s = s.Substring(5);
      }
      else if (s.StartsWith("tvrel:"))
      {
        coord = ScriptCoordType.TargetViewRelative;
        s = s.Substring(6);
      }
      else if (s.StartsWith("krel:"))
      {
        coord = ScriptCoordType.KillerRelative;
        s = s.Substring(5);
      }
      else if (s.StartsWith("kvrel:"))
      {
        coord = ScriptCoordType.KillerViewRelative;
        s = s.Substring(6);
      }
      ScriptPoint3D scriptPoint3D = new ScriptPoint3D();
      Parser.Token nextToken = this.GetNextToken(s, 0, char.MinValue, ',');
      if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.X.I))
      {
        scriptPoint3D.X.I = this.GetVarIndex(nextToken.Lexeme);
        if (scriptPoint3D.X.I < 0)
          return new ScriptPoint3D?();
        scriptPoint3D.X.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(s, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.Y.I))
      {
        scriptPoint3D.Y.I = this.GetVarIndex(nextToken.Lexeme);
        if (scriptPoint3D.Y.I < 0)
          return new ScriptPoint3D?();
        scriptPoint3D.Y.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(s, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.Z.I))
      {
        scriptPoint3D.Z.I = this.GetVarIndex(nextToken.Lexeme);
        if (scriptPoint3D.Z.I < 0)
          return new ScriptPoint3D?();
        scriptPoint3D.Z.T = ScriptValueType.Variable;
      }
      if (coord == ScriptCoordType.None)
        coord = ScriptCoordType.Absolute;
      return new ScriptPoint3D?(scriptPoint3D);
    }

    private ScriptPoint3D? GetPoint2DFromToken(Parser.Token token)
    {
      ScriptCoordType coord;
      return this.GetPoint2DFromToken(token, out coord);
    }

    private ScriptPoint3D? GetPoint2DFromToken(
      Parser.Token token,
      out ScriptCoordType coord)
    {
      coord = ScriptCoordType.None;
      string command = token.Lexeme;
      if (command.Length <= 2 || command.IndexOf(',') < 0)
        return new ScriptPoint3D?();
      if (command.StartsWith("rel:"))
      {
        coord = ScriptCoordType.Relative;
        command = command.Substring(4);
      }
      else if (command.StartsWith("prel:"))
      {
        coord = ScriptCoordType.PlayerRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("crel:"))
      {
        coord = ScriptCoordType.CursorRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("vrel:"))
      {
        coord = ScriptCoordType.ViewRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("hvrel:"))
      {
        coord = ScriptCoordType.ViewHorizRelative;
        command = command.Substring(6);
      }
      else if (command.StartsWith("trel:"))
      {
        coord = ScriptCoordType.TargetRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("tvrel:"))
      {
        coord = ScriptCoordType.TargetViewRelative;
        command = command.Substring(6);
      }
      else if (command.StartsWith("trel:"))
      {
        coord = ScriptCoordType.KillerRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("kvrel:"))
      {
        coord = ScriptCoordType.KillerViewRelative;
        command = command.Substring(6);
      }
      ScriptPoint3D scriptPoint3D = new ScriptPoint3D();
      Parser.Token nextToken = this.GetNextToken(command, 0, char.MinValue, ',');
      if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.X.I))
      {
        scriptPoint3D.X.I = this.GetVarIndex(nextToken.Lexeme);
        if (scriptPoint3D.X.I < 0)
          return new ScriptPoint3D?();
        scriptPoint3D.X.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(command, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.Z.I))
      {
        scriptPoint3D.Z.I = this.GetVarIndex(nextToken.Lexeme);
        if (scriptPoint3D.Z.I < 0)
          return new ScriptPoint3D?();
        scriptPoint3D.Z.T = ScriptValueType.Variable;
      }
      if (command.IndexOf(',') != command.LastIndexOf(','))
      {
        nextToken = this.GetNextToken(command, nextToken.EndIndex + 1, char.MinValue, ',');
        if (!int.TryParse(nextToken.Lexeme, out scriptPoint3D.Z.I))
        {
          scriptPoint3D.Z.I = this.GetVarIndex(nextToken.Lexeme);
          if (scriptPoint3D.Z.I < 0)
            return new ScriptPoint3D?();
          scriptPoint3D.Z.T = ScriptValueType.Variable;
        }
      }
      if (coord == ScriptCoordType.None)
        coord = ScriptCoordType.Absolute;
      return new ScriptPoint3D?(scriptPoint3D);
    }

    private ScriptVector2? GetVector2FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length <= 2 || lexeme.IndexOf(',') < 0)
        return new ScriptVector2?();
      ScriptVector2 scriptVector2 = new ScriptVector2();
      Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
      if (!float.TryParse(nextToken.Lexeme, out scriptVector2.X.I))
      {
        scriptVector2.X.I = (float) this.GetVarIndex(nextToken.Lexeme);
        if ((double) scriptVector2.X.I < 0.0)
          return new ScriptVector2?();
        scriptVector2.X.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!float.TryParse(nextToken.Lexeme, out scriptVector2.Y.I))
      {
        scriptVector2.Y.I = (float) this.GetVarIndex(nextToken.Lexeme);
        if ((double) scriptVector2.Y.I < 0.0)
          return new ScriptVector2?();
        scriptVector2.Y.T = ScriptValueType.Variable;
      }
      if (lexeme.IndexOf(',') != lexeme.LastIndexOf(','))
      {
        nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
        if (!float.TryParse(nextToken.Lexeme, out scriptVector2.Y.I))
        {
          scriptVector2.Y.I = (float) this.GetVarIndex(nextToken.Lexeme);
          if ((double) scriptVector2.Y.I < 0.0)
            return new ScriptVector2?();
          scriptVector2.Y.T = ScriptValueType.Variable;
        }
      }
      return new ScriptVector2?(scriptVector2);
    }

    private ScriptVector3? GetVector3FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 4)
      {
        int num = lexeme.IndexOf(',');
        if (num >= 0 && lexeme.IndexOf(',', num + 1) >= 0)
        {
          ScriptVector3 scriptVector3 = new ScriptVector3();
          Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
          if (!float.TryParse(nextToken.Lexeme, out scriptVector3.X.I))
          {
            scriptVector3.X.I = (float) this.GetVarIndex(nextToken.Lexeme);
            if ((double) scriptVector3.X.I < 0.0)
              return new ScriptVector3?();
            scriptVector3.X.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!float.TryParse(nextToken.Lexeme, out scriptVector3.Y.I))
          {
            scriptVector3.Y.I = (float) this.GetVarIndex(nextToken.Lexeme);
            if ((double) scriptVector3.Y.I < 0.0)
              return new ScriptVector3?();
            scriptVector3.Y.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!float.TryParse(nextToken.Lexeme, out scriptVector3.Z.I))
          {
            scriptVector3.Z.I = (float) this.GetVarIndex(nextToken.Lexeme);
            if ((double) scriptVector3.Z.I < 0.0)
              return new ScriptVector3?();
            scriptVector3.Z.T = ScriptValueType.Variable;
          }
          return new ScriptVector3?(scriptVector3);
        }
      }
      return new ScriptVector3?();
    }

    private ScriptVector3? GetVector3FromToken(
      Parser.Token token,
      out ScriptCoordType coord)
    {
      coord = ScriptCoordType.None;
      string command = token.Lexeme;
      if (command.Length <= 4 || command.IndexOf(',') == command.LastIndexOf(','))
        return new ScriptVector3?();
      if (command.StartsWith("rel:"))
      {
        coord = ScriptCoordType.Relative;
        command = command.Substring(4);
      }
      else if (command.StartsWith("prel:"))
      {
        coord = ScriptCoordType.PlayerRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("crel:"))
      {
        coord = ScriptCoordType.CursorRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("vrel:"))
      {
        coord = ScriptCoordType.ViewRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("hvrel:"))
      {
        coord = ScriptCoordType.ViewHorizRelative;
        command = command.Substring(6);
      }
      else if (command.StartsWith("trel:"))
      {
        coord = ScriptCoordType.TargetRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("tvrel:"))
      {
        coord = ScriptCoordType.TargetViewRelative;
        command = command.Substring(6);
      }
      else if (command.StartsWith("krel:"))
      {
        coord = ScriptCoordType.KillerRelative;
        command = command.Substring(5);
      }
      else if (command.StartsWith("kvrel:"))
      {
        coord = ScriptCoordType.KillerViewRelative;
        command = command.Substring(6);
      }
      ScriptVector3 scriptVector3 = new ScriptVector3();
      Parser.Token nextToken = this.GetNextToken(command, 0, char.MinValue, ',');
      if (!float.TryParse(nextToken.Lexeme, out scriptVector3.X.I))
      {
        scriptVector3.X.I = (float) this.GetVarIndex(nextToken.Lexeme);
        if ((double) scriptVector3.X.I < 0.0)
          return new ScriptVector3?();
        scriptVector3.X.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(command, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!float.TryParse(nextToken.Lexeme, out scriptVector3.Y.I))
      {
        scriptVector3.Y.I = (float) this.GetVarIndex(nextToken.Lexeme);
        if ((double) scriptVector3.Y.I < 0.0)
          return new ScriptVector3?();
        scriptVector3.Y.T = ScriptValueType.Variable;
      }
      nextToken = this.GetNextToken(command, nextToken.EndIndex + 1, char.MinValue, ',');
      if (!float.TryParse(nextToken.Lexeme, out scriptVector3.Z.I))
      {
        scriptVector3.Z.I = (float) this.GetVarIndex(nextToken.Lexeme);
        if ((double) scriptVector3.Z.I < 0.0)
          return new ScriptVector3?();
        scriptVector3.Z.T = ScriptValueType.Variable;
      }
      if (coord == ScriptCoordType.None)
        coord = ScriptCoordType.Absolute;
      return new ScriptVector3?(scriptVector3);
    }

    public ScriptVector4? GetVector4FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 6)
      {
        int num1 = lexeme.IndexOf(',');
        if (num1 >= 0)
        {
          int num2 = lexeme.IndexOf(',', num1 + 1);
          if (num2 >= 0 && lexeme.IndexOf(',', num2 + 1) >= 0)
          {
            ScriptVector4 scriptVector4 = new ScriptVector4();
            Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
            if (!float.TryParse(nextToken.Lexeme, out scriptVector4.X.I))
            {
              scriptVector4.X.I = (float) this.GetVarIndex(nextToken.Lexeme);
              if ((double) scriptVector4.X.I < 0.0)
                return new ScriptVector4?();
              scriptVector4.X.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!float.TryParse(nextToken.Lexeme, out scriptVector4.Y.I))
            {
              scriptVector4.Y.I = (float) this.GetVarIndex(nextToken.Lexeme);
              if ((double) scriptVector4.Y.I < 0.0)
                return new ScriptVector4?();
              scriptVector4.Y.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!float.TryParse(nextToken.Lexeme, out scriptVector4.Z.I))
            {
              scriptVector4.Z.I = (float) this.GetVarIndex(nextToken.Lexeme);
              if ((double) scriptVector4.Z.I < 0.0)
                return new ScriptVector4?();
              scriptVector4.Z.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!float.TryParse(nextToken.Lexeme, out scriptVector4.W.I))
            {
              scriptVector4.W.I = (float) this.GetVarIndex(nextToken.Lexeme);
              if ((double) scriptVector4.W.I < 0.0)
                return new ScriptVector4?();
              scriptVector4.W.T = ScriptValueType.Variable;
            }
            return new ScriptVector4?(scriptVector4);
          }
        }
      }
      return new ScriptVector4?();
    }

    public ScriptColor? GetColor3FromToken2(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 4)
      {
        int num = lexeme.IndexOf(',');
        if (num >= 0 && lexeme.IndexOf(',', num + 1) >= 0)
        {
          ScriptColor scriptColor = new ScriptColor();
          Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.R.I))
          {
            scriptColor.R.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.R.I < 0)
              return new ScriptColor?();
            scriptColor.R.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.G.I))
          {
            scriptColor.G.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.G.I < 0)
              return new ScriptColor?();
            scriptColor.G.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.B.I))
          {
            scriptColor.B.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.B.I < 0)
              return new ScriptColor?();
            scriptColor.B.T = ScriptValueType.Variable;
          }
          scriptColor.A.I = (int) byte.MaxValue;
          return new ScriptColor?(scriptColor);
        }
      }
      return new ScriptColor?();
    }

    public ScriptColor? GetColor4FromToken2(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 4)
      {
        int num = lexeme.IndexOf(',');
        if (num >= 0 && lexeme.IndexOf(',', num + 1) >= 0)
        {
          ScriptColor scriptColor = new ScriptColor();
          Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.R.I))
          {
            scriptColor.R.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.R.I < 0)
              return new ScriptColor?();
            scriptColor.R.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.G.I))
          {
            scriptColor.G.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.G.I < 0)
              return new ScriptColor?();
            scriptColor.G.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.B.I))
          {
            scriptColor.B.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.B.I < 0)
              return new ScriptColor?();
            scriptColor.B.T = ScriptValueType.Variable;
          }
          nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
          if (!int.TryParse(nextToken.Lexeme, out scriptColor.A.I) || scriptColor.A.I == 0)
          {
            scriptColor.A.I = this.GetVarIndex(nextToken.Lexeme);
            if (scriptColor.A.I < 0)
              scriptColor.A.I = (int) byte.MaxValue;
            else
              scriptColor.A.T = ScriptValueType.Variable;
          }
          return new ScriptColor?(scriptColor);
        }
      }
      return new ScriptColor?();
    }

    public ScriptRectangle? GetRectFromToken2(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 6)
      {
        int num1 = lexeme.IndexOf(',');
        if (num1 >= 0)
        {
          int num2 = lexeme.IndexOf(',', num1 + 1);
          if (num2 >= 0 && lexeme.IndexOf(',', num2 + 1) >= 0)
          {
            ScriptRectangle scriptRectangle = new ScriptRectangle();
            Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
            if (!int.TryParse(nextToken.Lexeme, out scriptRectangle.X.I))
            {
              scriptRectangle.X.I = this.GetVarIndex(nextToken.Lexeme);
              if (scriptRectangle.X.I < 0)
                return new ScriptRectangle?();
              scriptRectangle.X.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!int.TryParse(nextToken.Lexeme, out scriptRectangle.Y.I))
            {
              scriptRectangle.Y.I = this.GetVarIndex(nextToken.Lexeme);
              if (scriptRectangle.Y.I < 0)
                return new ScriptRectangle?();
              scriptRectangle.Y.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!int.TryParse(nextToken.Lexeme, out scriptRectangle.W.I))
            {
              scriptRectangle.W.I = this.GetVarIndex(nextToken.Lexeme);
              if (scriptRectangle.W.I < 0)
                return new ScriptRectangle?();
              scriptRectangle.W.T = ScriptValueType.Variable;
            }
            nextToken = this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',');
            if (!int.TryParse(nextToken.Lexeme, out scriptRectangle.H.I))
            {
              scriptRectangle.H.I = this.GetVarIndex(nextToken.Lexeme);
              if (scriptRectangle.H.I < 0)
                return new ScriptRectangle?();
              scriptRectangle.H.T = ScriptValueType.Variable;
            }
            return new ScriptRectangle?(scriptRectangle);
          }
        }
      }
      return new ScriptRectangle?();
    }

    private Block? GetBlockIDFromToken(Parser.Token token)
    {
      Item? itemIdFromToken = this.GetItemIDFromToken(token);
      if (itemIdFromToken.HasValue)
      {
        Item itemId = this.instance.ConvertItemIDToBlockID(new InventoryItem(itemIdFromToken.Value)).ItemID;
        if (itemId < Item.zLastBlockID && ItemData.IsEnabled(itemId) && itemId != Item.Bedrock)
          return new Block?((Block) itemId);
      }
      return new Block?();
    }

    private Map.CopyType? GetCopyTypeFromToken(Parser.Token token)
    {
      if (ScriptCompiler.copyTypes == null)
        ScriptCompiler.copyTypes = Utils.BuildEnumStringArray<Map.CopyType>((string) null, true);
      for (int index = 0; index < ScriptCompiler.copyTypes.Length; ++index)
      {
        if (ScriptCompiler.copyTypes[index] == token.Lexeme)
          return new Map.CopyType?((Map.CopyType) index);
      }
      return new Map.CopyType?();
    }

    private ActorType? GetActorTypeFromToken(Parser.Token token)
    {
      return this.GetActorTypeFromToken(token.Lexeme);
    }

    private ActorType? GetActorTypeFromToken(string token)
    {
      if (ScriptCompiler.npcTypes == null)
      {
        ScriptCompiler.npcTypes = Utils.BuildEnumStringArray<ActorType>();
        for (int index = 0; index < ScriptCompiler.npcTypes.Length; ++index)
          ScriptCompiler.npcTypes[index] = ScriptCompiler.npcTypes[index].ToLower();
      }
      for (int index = 0; index < ScriptCompiler.npcTypes.Length; ++index)
      {
        if (ScriptCompiler.npcTypes[index] == token)
          return new ActorType?((ActorType) index);
      }
      return new ActorType?();
    }

    public ScriptEvent? GetScriptEventTypeFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "itemswing":
          return new ScriptEvent?(ScriptEvent.ItemSwing);
        case "itemequip":
          return new ScriptEvent?(ScriptEvent.ItemEquip);
        case "itemunequip":
          return new ScriptEvent?(ScriptEvent.ItemUnequip);
        case "playerdeath":
          return new ScriptEvent?(ScriptEvent.PlayerDeath);
        case "playerjoin":
          return new ScriptEvent?(ScriptEvent.PlayerJoin);
        case "playerleave":
          return new ScriptEvent?(ScriptEvent.PlayerLeave);
        case "playerrespawn":
          return new ScriptEvent?(ScriptEvent.PlayerRespawn);
        case "custommenu":
          return new ScriptEvent?(ScriptEvent.CustomMenu);
        case "buttonx":
          return new ScriptEvent?(ScriptEvent.ButtonX);
        case "buttony":
          return new ScriptEvent?(ScriptEvent.ButtonY);
        case "buttonb":
          return new ScriptEvent?(ScriptEvent.ButtonB);
        default:
          return new ScriptEvent?();
      }
    }

    public ScriptShape? GetShapeFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "ray":
          return new ScriptShape?(ScriptShape.Ray);
        case "box":
          return new ScriptShape?(ScriptShape.Box);
        case "sphere":
          return new ScriptShape?(ScriptShape.Sphere);
        case "frustum":
          return new ScriptShape?(ScriptShape.Frustum);
        default:
          return new ScriptShape?();
      }
    }

    public ScriptContext? GetContextFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "default":
          return new ScriptContext?(ScriptContext.PlayerDefault);
        case "target":
          return new ScriptContext?(ScriptContext.PlayerTarget);
        case "killer":
          return new ScriptContext?(ScriptContext.PlayerKiller);
        default:
          return new ScriptContext?();
      }
    }

    public DataBlockScriptType? GetDataBlockScriptTypeFromToken(
      Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "entry":
          return new DataBlockScriptType?(DataBlockScriptType.Entry);
        case "exit":
          return new DataBlockScriptType?(DataBlockScriptType.Exit);
        case "poweron":
          return new DataBlockScriptType?(DataBlockScriptType.PowerOn);
        case "poweroff":
          return new DataBlockScriptType?(DataBlockScriptType.PowerOff);
        default:
          return new DataBlockScriptType?();
      }
    }

    private ScriptComparison ParseComparison(string command, int index)
    {
      ScriptComparison result = new ScriptComparison()
      {
        Type = Parser.CompareState.Binary,
        BoolResult = true
      };
      Parser.Token nextToken = this.GetNextToken(command, index);
      ScriptInt32? nullable = new ScriptInt32?();
      for (; nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        if (nextToken.Lexeme == "true" || nextToken.Lexeme == "false")
        {
          result.BoolResult = nextToken.Lexeme == "true";
          if (result.Type == Parser.CompareState.Equal)
          {
            if (!result.BoolResult)
            {
              result.Type = Parser.CompareState.NotEqual;
              break;
            }
            break;
          }
          result.Type = Parser.CompareState.Binary;
          break;
        }
        if (result.Type == Parser.CompareState.None || !nullable.HasValue && result.CountTarget == ScriptTarget.None)
        {
          Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken);
          if (compareFromToken.HasValue)
          {
            result.Type = compareFromToken.Value;
          }
          else
          {
            nullable = this.GetIntFromToken2(nextToken);
            if (nullable.HasValue)
            {
              result.Target = ScriptTarget.None;
              result.Count = nullable.Value;
              if (result.Type == Parser.CompareState.Binary)
                result.Type = Parser.CompareState.Equal;
            }
            else
              this.GetHistoryRefToken(nextToken, ref result);
          }
        }
        else
          break;
      }
      return result;
    }

    private ScriptComparison ParseTargetedComparison(string command, int index)
    {
      ScriptComparison result = new ScriptComparison()
      {
        Type = Parser.CompareState.Binary,
        BoolResult = true
      };
      for (Parser.Token nextToken = this.GetNextToken(command, index); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(command, nextToken.EndIndex + 1))
      {
        if (result.Type == Parser.CompareState.Binary && (nextToken.Lexeme == "player" || nextToken.Lexeme == "actor"))
          result.Target = ScriptTarget.Actor;
        else if (result.Type == Parser.CompareState.Binary && nextToken.Lexeme == "clan")
        {
          result.Target = ScriptTarget.Clan;
        }
        else
        {
          if (nextToken.Lexeme == "true" || nextToken.Lexeme == "false")
          {
            result.BoolResult = nextToken.Lexeme == "true";
            result.Type = Parser.CompareState.Binary;
            break;
          }
          Parser.CompareState? compareFromToken = this.GetCompareFromToken(nextToken);
          if (compareFromToken.HasValue)
          {
            result.Type = compareFromToken.Value;
          }
          else
          {
            ScriptInt32? intFromToken2 = this.GetIntFromToken2(nextToken);
            if (intFromToken2.HasValue)
            {
              result.CountTarget = ScriptTarget.None;
              result.Count = intFromToken2.Value;
              break;
            }
            this.GetHistoryRefToken(nextToken, ref result);
            break;
          }
        }
      }
      return result;
    }

    private ScriptVarNumber? ParseVarToken(Parser.Token token)
    {
      if (token.Lexeme.StartsWith("syshistory:"))
        return new ScriptVarNumber?(new ScriptVarNumber()
        {
          Type = ScriptValueType.SysHistoryKey,
          Key = token.Lexeme.Substring(11, token.Lexeme.Length - 11)
        });
      if (token.Lexeme.StartsWith("clanhistory:"))
        return new ScriptVarNumber?(new ScriptVarNumber()
        {
          Type = ScriptValueType.ClanHistoryKey,
          Key = token.Lexeme.Substring(12, token.Lexeme.Length - 12)
        });
      if (token.Lexeme.StartsWith("history:"))
        return new ScriptVarNumber?(new ScriptVarNumber()
        {
          Type = ScriptValueType.HistoryKey,
          Key = token.Lexeme.Substring(8, token.Lexeme.Length - 8)
        });
      if (token.Lexeme.StartsWith("rand:"))
      {
        ScriptVarNumber scriptVarNumber = new ScriptVarNumber()
        {
          Type = ScriptValueType.Random
        };
        int result;
        if (!int.TryParse(token.Lexeme.Substring(5, token.Lexeme.Length - 5), out result))
          result = 1000;
        scriptVarNumber.Value = (double) result;
        return new ScriptVarNumber?(scriptVarNumber);
      }
      int result1;
      if (int.TryParse(token.Lexeme, out result1))
        return new ScriptVarNumber?(new ScriptVarNumber()
        {
          Type = ScriptValueType.NumLiterial,
          Value = (double) result1
        });
      int varIndex = this.GetVarIndex(token.Lexeme);
      if (varIndex < 0)
        return new ScriptVarNumber?();
      return new ScriptVarNumber?(new ScriptVarNumber()
      {
        Type = ScriptValueType.Variable,
        Value = (double) varIndex
      });
    }

    private void GetHistoryRefToken(Parser.Token token, ref ScriptComparison result)
    {
      result.CountTarget = ScriptTarget.None;
      int startIndex = 0;
      if (token.Lexeme.StartsWith("history:"))
      {
        result.CountTarget = ScriptTarget.Actor;
        startIndex = 8;
      }
      else if (token.Lexeme.StartsWith("syshistory:"))
      {
        result.CountTarget = ScriptTarget.System;
        startIndex = 11;
      }
      else if (token.Lexeme.StartsWith("clanhistory:"))
      {
        result.CountTarget = ScriptTarget.Clan;
        startIndex = 12;
      }
      if (result.CountTarget == ScriptTarget.None)
        return;
      result.CountKey = token.Lexeme.Substring(startIndex, token.Lexeme.Length - startIndex);
    }

    private void WriteCommandHeader(BinaryWriter writer, ScriptCommand command)
    {
      this.WriteNextCommandOffset(writer);
      writer.Write((byte) command);
      this.lastCommandPC = writer.BaseStream.Position;
      writer.Write((ushort) 0);
    }

    private void WriteNextCommandOffset(BinaryWriter writer)
    {
      if (this.lastCommandPC <= 0L)
        return;
      long position = writer.BaseStream.Position;
      writer.BaseStream.Position = this.lastCommandPC;
      writer.Write((ushort) ((ulong) (position - this.lastCommandPC) - 2UL));
      writer.BaseStream.Position = position;
    }

    private void WriteVarNumber(BinaryWriter writer, ScriptVarNumber? n)
    {
      if (n.HasValue)
      {
        writer.Write((byte) n.Value.Type);
        switch (n.Value.Type)
        {
          case ScriptValueType.NumLiterial:
            writer.Write(n.Value.Value);
            break;
          case ScriptValueType.Variable:
            writer.Write((ushort) n.Value.Value);
            break;
          case ScriptValueType.Random:
            writer.Write((uint) n.Value.Value);
            break;
          default:
            writer.Write(n.Value.Key);
            break;
        }
      }
      else
      {
        writer.Write((byte) 0);
        writer.Write(0);
      }
    }

    private void WriteComparison(BinaryWriter writer, ScriptComparison compare)
    {
      writer.Write((byte) compare.Target);
      writer.Write((byte) compare.Type);
      if (compare.Type == Parser.CompareState.Binary)
      {
        writer.Write(compare.BoolResult);
      }
      else
      {
        writer.Write((byte) compare.CountTarget);
        if (compare.CountTarget == ScriptTarget.None)
          this.WriteInt32(writer, compare.Count);
        else
          writer.Write(compare.CountKey);
      }
    }

    private void WriteInt32(BinaryWriter writer, ScriptInt32 i)
    {
      writer.Write((byte) i.T);
      if (i.T == ScriptValueType.NumLiterial)
        writer.Write(i.I);
      else
        writer.Write((ushort) i.I);
    }

    private void WriteSingle(BinaryWriter writer, ScriptSingle i)
    {
      writer.Write((byte) i.T);
      if (i.T == ScriptValueType.NumLiterial)
        writer.Write(i.I);
      else
        writer.Write((ushort) i.I);
    }

    private void WriteDouble(BinaryWriter writer, ScriptDouble i)
    {
      writer.Write((byte) i.T);
      if (i.T == ScriptValueType.NumLiterial)
        writer.Write(i.I);
      else
        writer.Write((ushort) i.I);
    }

    private void WriteColor(BinaryWriter writer, ScriptColor c)
    {
      this.WriteInt32(writer, c.R);
      this.WriteInt32(writer, c.G);
      this.WriteInt32(writer, c.B);
      this.WriteInt32(writer, c.A);
    }

    private void WriteColor(BinaryWriter writer, ScriptColor? c)
    {
      writer.Write(c.HasValue);
      if (!c.HasValue)
        return;
      this.WriteInt32(writer, c.Value.R);
      this.WriteInt32(writer, c.Value.G);
      this.WriteInt32(writer, c.Value.B);
      this.WriteInt32(writer, c.Value.A);
    }

    private void WriteCoord(BinaryWriter writer, ScriptCoordType coord, ScriptPoint3D? p)
    {
      writer.Write(p.HasValue ? (byte) coord : (byte) 0);
      if (!p.HasValue || coord == ScriptCoordType.None)
        return;
      this.WriteInt32(writer, p.Value.X);
      this.WriteInt32(writer, p.Value.Y);
      this.WriteInt32(writer, p.Value.Z);
    }

    private void WriteCoord2D(BinaryWriter writer, ScriptCoordType coord, ScriptPoint3D? p)
    {
      writer.Write(p.HasValue ? (byte) coord : (byte) 0);
      if (!p.HasValue || coord == ScriptCoordType.None)
        return;
      this.WriteInt32(writer, p.Value.X);
      this.WriteInt32(writer, p.Value.Z);
    }

    private void WriteVector2(BinaryWriter writer, ScriptVector2 v)
    {
      this.WriteSingle(writer, v.X);
      this.WriteSingle(writer, v.Y);
    }

    private void WriteVector2(BinaryWriter writer, ScriptVector2? v)
    {
      writer.Write(v.HasValue);
      if (!v.HasValue)
        return;
      this.WriteSingle(writer, v.Value.X);
      this.WriteSingle(writer, v.Value.Y);
    }

    private void WriteVector3(BinaryWriter writer, ScriptVector3 v)
    {
      this.WriteSingle(writer, v.X);
      this.WriteSingle(writer, v.Y);
      this.WriteSingle(writer, v.Z);
    }

    private void WriteVector3(BinaryWriter writer, ScriptCoordType coord, ScriptVector3? v)
    {
      writer.Write(v.HasValue ? (byte) coord : (byte) 0);
      if (!v.HasValue || coord == ScriptCoordType.None)
        return;
      this.WriteSingle(writer, v.Value.X);
      this.WriteSingle(writer, v.Value.Y);
      this.WriteSingle(writer, v.Value.Z);
    }

    private void WriteVector4(BinaryWriter writer, ScriptVector4 v)
    {
      this.WriteSingle(writer, v.X);
      this.WriteSingle(writer, v.Y);
      this.WriteSingle(writer, v.Z);
      this.WriteSingle(writer, v.W);
    }

    private bool WriteString(BinaryWriter writer, string s)
    {
      bool flag = s != null && s.Length > 0;
      writer.Write(flag);
      if (flag)
        writer.Write(s);
      return flag;
    }

    private void LogTestError(string error)
    {
      if (!this.IsTest)
        return;
      if (this.errorVerbosity > 0)
        this.testData.TestResult.Add(string.Format("Script: {0}, Line: {1}, Error: {2}", (object) this.testData.ScriptName, (object) this.testData.LineNo, (object) error));
      else
        this.testData.TestResult.Add(string.Format("Line: {0}, Error: {1}", (object) this.testData.LineNo, (object) error));
    }

    private void LogTestWarning(string warning)
    {
      if (!this.IsTest)
        return;
      if (this.errorVerbosity > 0)
        this.testData.TestResult.Add(string.Format("Script: {0}, Line: {1}, Warning: {2}", (object) this.testData.ScriptName, (object) this.testData.LineNo, (object) warning));
      else
        this.testData.TestResult.Add(string.Format("Line: {0}, Warning: {1}", (object) this.testData.LineNo, (object) warning));
    }

    public static string GetCalledScriptName(string command)
    {
      string str = (string) null;
      try
      {
        if (command.StartsWith("script", StringComparison.OrdinalIgnoreCase))
          str = command.Substring(command.IndexOf('[') + 1, command.IndexOf(']') - command.IndexOf('[') - 1);
        else if (command.StartsWith("seteventscript", StringComparison.OrdinalIgnoreCase))
          str = command.Substring(command.LastIndexOf('[') + 1, command.LastIndexOf(']') - command.LastIndexOf('[') - 1);
        else if (command.StartsWith("setblockscript", StringComparison.OrdinalIgnoreCase))
        {
          int num1 = command.IndexOf('[');
          if (num1 >= 0)
          {
            int num2 = command.IndexOf(']', num1 + 1);
            if (num2 >= 0)
              str = command.Substring(command.IndexOf('[', num2 + 1) + 1, command.IndexOf(']', num2 + 1) - command.IndexOf('[', num2 + 1) - 1);
          }
        }
      }
      catch (Exception ex)
      {
      }
      return str;
    }

    public bool IsScriptReferenced(Script caller, Script callee)
    {
      for (int index = 0; index < caller.Commands.Count; ++index)
      {
        string command = caller.Commands[index];
        if (command != null && command.Length >= 1 && (command.Length <= 1 || command[0] != '/' || command[1] != '/'))
        {
          string lower = command.ToLower();
          Parser.Token nextToken = this.GetNextToken(lower, 0, char.MinValue, ' ');
          bool flag = false;
          switch (nextToken.Lexeme)
          {
            case "script":
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              flag = true;
              break;
            case "seteventscript":
            case "setblockscript":
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              if (nextToken.Lexeme.Equals("itemswing", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.Equals("xbutton", StringComparison.OrdinalIgnoreCase))
                nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              flag = true;
              break;
            case "npcspawn":
            case "spawnmob":
            case "mobspawn":
            case "zone":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex && !flag; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                int length = nextToken.Lexeme.IndexOf('=');
                if (length >= 0)
                {
                  string str1 = nextToken.Lexeme.Substring(0, length);
                  string str2 = nextToken.Lexeme.Substring(length + 1, nextToken.Lexeme.Length - length - 1);
                  if (str1 == "kill" || str1 == "entry" || str1 == "exit")
                  {
                    nextToken.Lexeme = str2;
                    flag = true;
                    break;
                  }
                }
              }
              break;
            case "menu":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                if (this.menuParams == null)
                  this.menuParams = new List<ScriptMenuParam>();
                this.menuParams.Add(this.ParseMenuParam(nextToken.Lexeme));
              }
              if (this.menuParams != null)
              {
                using (List<ScriptMenuParam>.Enumerator enumerator = this.menuParams.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    ScriptMenuParam current = enumerator.Current;
                    if (current.Script.Equals(callee.Name, StringComparison.OrdinalIgnoreCase) || callee.Alias.IsNotEmpty() && current.Script.Equals(callee.Alias, StringComparison.OrdinalIgnoreCase))
                      return true;
                  }
                  break;
                }
              }
              else
                break;
            case "messagebox":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                if (nextToken.Lexeme.StartsWith("a=", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.StartsWith("x=", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.StartsWith("y=", StringComparison.OrdinalIgnoreCase))
                {
                  ScriptMenuParam menuParam = this.ParseMenuParam(nextToken.Lexeme);
                  if (menuParam.Script.Equals(callee.Name, StringComparison.OrdinalIgnoreCase) || callee.Alias.IsNotEmpty() && menuParam.Script.Equals(callee.Alias, StringComparison.OrdinalIgnoreCase))
                    return true;
                }
              }
              break;
          }
          if (flag && nextToken.Lexeme.Equals(callee.Name, StringComparison.OrdinalIgnoreCase) || callee.Alias.IsNotEmpty() && nextToken.Lexeme.Equals(callee.Alias, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public void GetReferencedScripts(Script script, List<string> list)
    {
      for (int index = 0; index < script.Commands.Count; ++index)
      {
        string command = script.Commands[index];
        if (command != null && command.Length >= 1 && (command.Length <= 1 || command[0] != '/' || command[1] != '/'))
        {
          string lower = command.ToLower();
          Parser.Token nextToken = this.GetNextToken(lower, 0, char.MinValue, ' ');
          switch (nextToken.Lexeme)
          {
            case nameof (script):
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              this.AddScript(nextToken.Lexeme, list);
              continue;
            case "seteventscript":
            case "setblockscript":
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              if (nextToken.Lexeme.Equals("itemswing", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.Equals("xbutton", StringComparison.OrdinalIgnoreCase))
                nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              this.AddScript(nextToken.Lexeme, list);
              continue;
            case "npcspawn":
            case "spawnmob":
            case "mobspawn":
            case "zone":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                int length = nextToken.Lexeme.IndexOf('=');
                if (length >= 0)
                {
                  string str = nextToken.Lexeme.Substring(0, length);
                  string name = nextToken.Lexeme.Substring(length + 1, nextToken.Lexeme.Length - length - 1);
                  if (str == "kill" || str == "entry" || str == "exit")
                    this.AddScript(name, list);
                }
              }
              continue;
            case "menu":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
                this.AddScript(this.ParseMenuParam(nextToken.Lexeme).Script, list);
              continue;
            case "messagebox":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                if (nextToken.Lexeme.StartsWith("a=", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.StartsWith("x=", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.StartsWith("y=", StringComparison.OrdinalIgnoreCase))
                  this.AddScript(this.ParseMenuParam(nextToken.Lexeme).Script, list);
              }
              continue;
            default:
              continue;
          }
        }
      }
    }

    public bool RenameReferencedScript(Script script, string oldName, string newName)
    {
      oldName = oldName.ToLower();
      for (int index = 0; index < script.Commands.Count; ++index)
      {
        string command1 = script.Commands[index];
        if (command1 != null && command1.Length >= 1 && (command1.Length <= 1 || command1[0] != '/' || command1[1] != '/'))
        {
          string lower = command1.ToLower();
          Parser.Token nextToken = this.GetNextToken(lower, 0, char.MinValue, ' ');
          switch (nextToken.Lexeme)
          {
            case nameof (script):
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              if (nextToken.Lexeme == oldName)
              {
                string command2 = script.Commands[index];
                script.Commands[index] = command2.Substring(0, nextToken.StartIndex) + newName + command2.Substring(nextToken.EndIndex);
                script.IsChanged = true;
                continue;
              }
              continue;
            case "seteventscript":
            case "setblockscript":
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              if (nextToken.Lexeme.Equals("itemswing", StringComparison.OrdinalIgnoreCase) || nextToken.Lexeme.Equals("xbutton", StringComparison.OrdinalIgnoreCase))
                nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1);
              if (nextToken.Lexeme == oldName)
              {
                string command2 = script.Commands[index];
                script.Commands[index] = command2.Substring(0, nextToken.StartIndex) + newName + command2.Substring(nextToken.EndIndex);
                script.IsChanged = true;
                continue;
              }
              continue;
            case "npcspawn":
            case "spawnmob":
            case "mobspawn":
            case "zone":
              for (nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1); nextToken.StartIndex < nextToken.EndIndex; nextToken = this.GetNextToken(lower, nextToken.EndIndex + 1))
              {
                int length = nextToken.Lexeme.IndexOf('=');
                if (length >= 0)
                {
                  string str = nextToken.Lexeme.Substring(0, length);
                  if (nextToken.Lexeme.Substring(length + 1, nextToken.Lexeme.Length - length - 1) == oldName && (str == "kill" || str == "entry" || str == "exit"))
                  {
                    string command2 = script.Commands[index];
                    script.Commands[index] = command2.Substring(0, length) + newName + command2.Substring(nextToken.EndIndex);
                    script.IsChanged = true;
                  }
                }
              }
              continue;
            default:
              continue;
          }
        }
      }
      return false;
    }

    private void AddScript(string name, List<string> list)
    {
      if (name == null || name.Length <= 0 || list.Contains(name))
        return;
      list.Add(name);
    }

    private class ScriptZoneParams
    {
      public bool MinIsSet;
      public bool? TypeSpawn;
      public bool? TypeNoEdit;
      public bool? TypeNoCombat;
      public bool? TypeNoFly;
      public bool? TypeNoMobs;
      public bool? TypeNoEscape;
      public ScriptCoordType MinCoord;
      public ScriptPoint3D? Min;
      public ScriptCoordType MaxCoord;
      public ScriptPoint3D? Max;
      public ZoneBuilderType? BuilderType;
      public string OnEntryScriptName;
      public string OnExitScriptName;
      public ScriptInt32? CombatLevelDifference;
      public ScriptSingle? SpeedMultiplier;
      public ScriptSingle? GravityMultiplier;
    }

    private struct TestData
    {
      public string ScriptName;
      public int LineNo;
      public List<string> TestResult;
    }

    private struct NpcStat
    {
      public SkillType SkillType;
      public int Level;
    }

    private struct VarOp
    {
      public ScriptValueType ValueType;
      public double ValueNum;
      public string ValueString;
      public int VarIndex;
      public ScriptVarOp OpType;
      public Item? OpItemID;
      public ScriptInt32? OPInt1;
      public ScriptSingle? OPSingle1;
      public ScriptCoordType OpCoord1;
      public ScriptCoordType OpCoord2;
      public ScriptPoint3D? OpPoint1;
      public ScriptPoint3D? OpPoint2;
      public ScriptVector3? OpVector1;
      public ScriptVector3? OpVector2;
    }
  }
}
