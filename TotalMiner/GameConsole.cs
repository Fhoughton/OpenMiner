// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GameConsole
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class GameConsole
  {
    private static string helpHelp = "Display help information. \r\n\r\nhelp [command]\r\n\r\n[command]  -- Enter a command to view full help for that command.\r\n              Omit command to view brief help for all commands.\r\n\r\nExamples:\r\nhelp  -- display a list of all commands with a brief description of each.\r\nhelp kill  -- display full help information for the kill command.";
    private static string helpConsole = "Adjust the position or size of the console window. \r\n\r\nconsole -fs -o -p -s [scale] [lines] [x] [y]\r\n\r\n-fs [scale] -- Adjust the scale used to draw the console font. 0.75 = default. \r\n\r\n-o [lines]  -- Set how many lines of text are kept in the console output stream before they are dropped off the end. The default is 1000.\r\n\r\n-p [x] [y]  -- Adjust the position of the console window. \r\n               x & y is the new screen position in pixels.\r\n\r\n-s [x] [y]  -- Adjust the size of the console window. \r\n               x & y is the new size in columns and rows.\r\n\r\nExamples:\r\nconsole -p 20 50  -- changes the console screen position to 20 pixels across and 50 pixels down.\r\nconsole -s 30 20  -- changes the console size to 30 characters wide and 20 rows high.\r\nconsole -fs 0.5   -- makes the console font smaller.\r\nconsole -o 500    -- set the consoles output stream size to 500 lines.";
    private static string helpInv = "Manipulate the current players inventory.\r\n\r\ninv -a -e -r -ra [item] [qty]\r\n\r\n-a   -- Add the item to inventory. If [qty] is omitted, the items default stack size will be added. If no other -args are used, -a is assumed.\r\n-e   -- Equip the item. Optional. Can be used in conjunction with -a.\r\n-r   -- Remove the item from inventory. If [qty] is omitted, all stacks of the item will be removed.\r\n-ra  -- Remove all items.\r\n[item]  -- The item to add/remove/equip/list.\r\n[qty]  -- The number of units. \r\n\r\nExamples:\r\ninv grass  -- add a stack of grass (-a assumed).\r\ninv -a -e dirt 50  -- add 50 dirt and equip.\r\ninv -e dirt  -- equip dirt if in inventory.\r\ninv -r dirt  -- remove all stacks of dirt.\r\ninv -r dirt 50  -- remove 50 dirt.\r\ninv -ra  -- remove all items.";
    private static string helpKill = "Kill the current player, all NPCs or all NPCs of a type.\r\n\r\nkill -a -p [npctype]\r\n\r\n-a  -- kill all NPCs.\r\n-p  -- kill the current player\r\n[npctype]  -- kill all NPCs of this type.\r\n\r\nExamples:\r\nkill -a  -- this will kill all NPCs.\r\nkill -p  -- this will kill the current player.\r\nkill goblin  -- this will kill all goblins.";
    private static string helpList = "list: List various game information.\r\n\r\nlist -i -m -p -pe -s -tp -z [pattern]\r\n\r\n-i   -- list all items in the current players inventory.\r\n-m   -- list map markers.\r\n-mg  -- list map grave markers.\r\n-p   -- list all players in the world.\r\n-pe  -- list the current players permissions.\r\n-s   -- list scripts.\r\n-tp  -- list the current players teleport points.\r\n-z   -- list zones.\r\n\r\n[pattern] -- any output that does not start with pattern is ignored. \r\n             e.g. list -i sa  -- list all items in inventory that start with 'sa'\r\n                  list -p j  -- list all players whose name starts with 'j'";
    private static string helpMsg = "Send a text message to other players.\r\n\r\nmsg -a [player] [message]\r\n\r\n-a  -- Send message only to players with admin permission.\r\n[player] - Send private message to named player.\r\n[message] - The message text. surround with double quotes for messages that contain spaces.\r\n\r\nexample:\r\nmsg \"Hello everyone\"\r\nSend the msg \"Hello everyone\" to all players in the world.\r\n\r\nmsg -a \"some important secret message\"\r\nSend the message only to admins.\r\n\r\nexample:\r\nmsg hellosailor Ahoy!\r\nSend the message \"Ahoy!\" to the gamer named hellosailor.";
    private static string helpNotify = "Add a notification to top left of screen.\r\n\r\nnotify -a -c -g -l -r [message]\r\n\r\n-a  -- Notification appears on the screen for admins.\r\n-c  -- Notification appears on the screen for all members of the current players clan.\r\n-g  -- Notification appears on the screen for all gamers (default).\r\n-l  -- Notification only appears on your screen (local).\r\n-r  -- Notification appears on the screen for all remote gamers, but not yours.\r\n[message]  -- The notification text. Surround the message with double quotes if it contains spaces.\r\n\r\nExample:\r\nnotify \"hello everyone\"\r\nnotify -g \"hello everyone\"\r\nThe notification appears on every gamers screen.\r\n\r\nnotify -a \"meeting time\"\r\nThe notification appears only on admin screens.";
    private static string helpPlayer = "Change the current player. Certain commands act on the current player.\r\n\r\nplayer [name]\r\n\r\n[name]  -- The name of the player to set as the current. \r\n           Omit to change back to the original player who opened the console.\r\n\r\nExamples:\r\nplayer  -- changes the current player to the original player that opened the console.\r\nplayer john10  -- changes the current player to john10.";
    private static string helpScript = "Execute an existing script or single script command.\r\n\r\nscript -c -s [command|script]\r\n\r\n-c  -- Run a single script command.\r\n-s  -- Run a script (default switch, can be omitted).\r\n[command] -- A single command to run.\r\n[script]  -- A full script name to run, including folders. \r\n\r\nExamples:\r\nscript Events\\PlayerJoined\r\nscript -s Events\\PlayerJoined\r\nRuns the script called Events\\PlayerJoined.\r\n\r\nscript -c notify [hello [gamertag]]\r\nRuns the single script command: notify [hello [gamertag]]";
    private static string helpTp = "Teleport the current player or manipulate teleport points.\r\n\r\ntp -a -r -ra [name]\r\n\r\n-a   -- Add a teleport point for the current player (where the current player is currently standing).\r\n-r   -- Remove the named teleport point for the current player.\r\n-ra  -- Remove all teleport points from the current player.\r\n[name]  -- Name of the teleport point or map marker.\r\n\r\nExamples:\r\ntp home  -- teleport the current player to the teleport point or map marker named 'home'.\r\ntp -a home  -- add the teleport point 'home' to the current player.\r\ntp -r home  -- remove the teleport point 'home' for the current player.\r\ntp -ra  -- remove all teleport points from the current player.";
    private static Parser parser = new Parser();
    private static List<string> commands = new List<string>();
    private static List<string> commandHelp = new List<string>();
    private static List<string> commandFullHelp = new List<string>();
    private static List<Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>> actions = new List<Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>>();
    private ConsoleWindow consoleWin;
    private ITMGame game;
    private ITMPlayer origPlayer;
    private ITMPlayer player;

    static GameConsole()
    {
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdCls), "cls", "Clear the screen.", "");
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdConsole), "console", "Edit console properties.", GameConsole.helpConsole);
      GameConsole.commands.Add("exit");
      GameConsole.actions.Add((Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>) null);
      GameConsole.commandHelp.Add("Close the console.");
      GameConsole.commandFullHelp.Add("");
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdHelp), "help", "Display help information.", GameConsole.helpHelp);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdInv), "inv", "Manipulate the current players inventory.", GameConsole.helpInv);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdKill), "kill", "Kill the current player, all NPCs or all NPCs of a type.", GameConsole.helpKill);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdList), "list", "List various game information.", GameConsole.helpList);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdMsg), "msg", "Send a text message to other players.", GameConsole.helpMsg);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdNotify), "notify", "Add a notification to top left of screen.", GameConsole.helpNotify);
      GameConsole.commands.Add(nameof (player));
      GameConsole.actions.Add((Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>) null);
      GameConsole.commandHelp.Add("Change the current player.");
      GameConsole.commandFullHelp.Add(GameConsole.helpPlayer);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdScript), "script", "Execute an existing script.", GameConsole.helpScript);
      GameConsole.AddCommand(new Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog>(GameConsole.CmdTp), "tp", "Teleport the current player or manipulate teleport points.", GameConsole.helpTp);
    }

    public GameConsole(ConsoleWindow consoleWin, ITMGame game, ITMPlayer player)
    {
      this.consoleWin = consoleWin;
      this.game = game;
      this.SetPlayer(this.origPlayer = player);
    }

    public static void AddCommand(
      Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog> action,
      string cmd,
      string briefHelp,
      string fullHelp)
    {
      if (GameConsole.commands == null || action == null || (cmd == null || cmd.Length <= 0))
        return;
      GameConsole.commands.Add(cmd);
      GameConsole.actions.Add(action);
      GameConsole.commandHelp.Add(briefHelp);
      GameConsole.commandFullHelp.Add(fullHelp);
    }

    private void SetPlayer(ITMPlayer player)
    {
      if (this.player == player)
        return;
      this.player = player;
      if (this.consoleWin == null)
        return;
      this.consoleWin.SetPrompt(player != null ? player.Name + ">" : ">");
    }

    private static string[] ParseToArgs(string command)
    {
      return command.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static List<string> ParseToArgsDblQuotes(string command)
    {
      return Parser.Split(command, ' ', '"');
    }

    private static bool HasAnySwitch(string[] args)
    {
      for (int index = 1; index < args.Length; ++index)
      {
        if (args[index][0] == '-' || args[index][0] == '/')
          return true;
      }
      return false;
    }

    private static bool HasAnySwitch(List<string> args)
    {
      for (int index = 1; index < args.Count; ++index)
      {
        if (args[index][0] == '-' || args[index][0] == '/')
          return true;
      }
      return false;
    }

    private static bool HasSwitch(string[] args, string arg)
    {
      if (arg == null || arg.Length < 1)
        return false;
      for (int index = 1; index < args.Length; ++index)
      {
        string str = args[index];
        if (str[0] == '/')
          str = 45.ToString() + str.Substring(1);
        if (arg.Equals(str, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static bool HasSwitch(List<string> args, string arg)
    {
      if (arg == null || arg.Length < 1)
        return false;
      for (int index = 1; index < args.Count; ++index)
      {
        string str = args[index];
        if (str[0] == '/')
          str = 45.ToString() + str.Substring(1);
        if (arg.Equals(str, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static string GetParam(List<string> args, int index)
    {
      if (args != null && args.Count > index)
        return args[index];
      return (string) null;
    }

    private static string GetParam(string[] args, int index)
    {
      if (index >= 0 && args != null && args.Length > index)
        return args[index];
      return (string) null;
    }

    private static int GetFirstParamIndex(List<string> args)
    {
      for (int index = 1; index < args.Count; ++index)
      {
        if (args[index] != null && args[index].Length > 0 && (args[index][0] != '-' && args[index][0] != '/'))
          return index;
      }
      return -1;
    }

    private static int GetFirstParamIndex(string[] args)
    {
      for (int index = 1; index < args.Length; ++index)
      {
        if (args[index] != null && args[index].Length > 0 && (args[index][0] != '-' && args[index][0] != '/'))
          return index;
      }
      return -1;
    }

    private static string GetFirstParam(string[] args)
    {
      return GameConsole.GetParam(args, GameConsole.GetFirstParamIndex(args));
    }

    private static string GetFirstParam(List<string> args)
    {
      return GameConsole.GetParam(args, GameConsole.GetFirstParamIndex(args));
    }

    private static ActorType? GetActorType(string type)
    {
      string[] strArray = Utils.BuildEnumStringArray<ActorType>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (type.Equals(strArray[index], StringComparison.OrdinalIgnoreCase))
          return new ActorType?((ActorType) index);
      }
      return new ActorType?();
    }

    public void RunCommand(string command)
    {
      if (!command.IsNotEmpty())
        return;
      int length = command.IndexOf(' ');
      string str = (length < 0 ? command : command.Substring(0, length)).ToLower().Trim();
      while (str.StartsWith("/"))
        str = str.Substring(1);
      if (str == "player")
      {
        this.CmdPlayer(command, this.game, this.origPlayer, this.player, (IOutputLog) this.consoleWin);
        this.consoleWin.WriteLine("");
      }
      else
      {
        if (str.Equals(this.consoleWin.ExitCommand))
          return;
        bool flag = true;
        for (int index = 0; index < GameConsole.commands.Count; ++index)
        {
          if (GameConsole.commands[index] == str)
          {
            GameConsole.actions[index](command, this.game, this.origPlayer, this.player, (IOutputLog) this.consoleWin);
            flag = false;
            break;
          }
        }
        if (flag && !this.game.RunConsoleCommand(command, this.origPlayer, this.player, (IOutputLog) this.consoleWin))
          this.consoleWin.WriteLine("unknown command: " + str);
        if (!(str != "cls"))
          return;
        this.consoleWin.WriteLine("");
      }
    }

    private static void CmdCls(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      (log as ConsoleWindow)?.ClearScreen();
    }

    private static void CmdConsole(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      ConsoleWindow consoleWindow = log as ConsoleWindow;
      if (consoleWindow == null)
        return;
      string[] toArgs = GameConsole.ParseToArgs(command);
      if (GameConsole.HasSwitch(toArgs, "-fs"))
      {
        if (toArgs.Length < 3)
        {
          log.WriteLine("insufficient arguments.");
          return;
        }
        float result = consoleWindow.TextScale;
        if (float.TryParse(GameConsole.GetFirstParam(toArgs), out result))
          consoleWindow.SetConsoleFontSize(result);
        else
          log.WriteLine("unknown scale");
      }
      if (GameConsole.HasSwitch(toArgs, "-o"))
      {
        if (toArgs.Length < 3)
        {
          log.WriteLine("insufficient arguments.");
        }
        else
        {
          int result;
          if (int.TryParse(GameConsole.GetFirstParam(toArgs), out result))
          {
            result = MyMathHelper.Clamp(result, 100, 10000);
            consoleWindow.SetConsoleStreamSize(result);
            log.WriteLine("Output stream size set to " + (object) result);
          }
          else
            log.WriteLine("unknown argument");
        }
      }
      else if (GameConsole.HasSwitch(toArgs, "-p"))
      {
        if (toArgs.Length < 4)
        {
          log.WriteLine("insufficient arguments.");
        }
        else
        {
          int firstParamIndex = GameConsole.GetFirstParamIndex(toArgs);
          int result1 = -1;
          int result2 = -1;
          if (!int.TryParse(GameConsole.GetParam(toArgs, firstParamIndex), out result1) || !int.TryParse(GameConsole.GetParam(toArgs, firstParamIndex + 1), out result2))
            return;
          if (result1 >= 0)
            consoleWindow.Position.X = (float) result1;
          if (result2 >= 0)
            consoleWindow.Position.Y = (float) result2;
          GamertagData gamertagData = Globals2.GamertagData.GetGamertagData(player.PlayerIndex);
          if (gamertagData == null)
            return;
          gamertagData.ConsolePos = new Point(result1, result2);
        }
      }
      else if (GameConsole.HasSwitch(toArgs, "-s"))
      {
        if (toArgs.Length < 4)
        {
          log.WriteLine("insufficient arguments.");
        }
        else
        {
          int firstParamIndex = GameConsole.GetFirstParamIndex(toArgs);
          int result1 = -1;
          int result2 = -1;
          if (!int.TryParse(GameConsole.GetParam(toArgs, firstParamIndex), out result1) || !int.TryParse(GameConsole.GetParam(toArgs, firstParamIndex + 1), out result2))
            return;
          consoleWindow.SetConsoleSize(result1, result2);
          GamertagData gamertagData = Globals2.GamertagData.GetGamertagData(player.PlayerIndex);
          if (gamertagData == null)
            return;
          gamertagData.ConsoleSize = consoleWindow.Size;
        }
      }
      else
        log.WriteLine("argument 1 is invalid.");
    }

    private static void CmdHelp(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      string[] toArgs = GameConsole.ParseToArgs(command);
      if (toArgs.Length > 1)
      {
        ConsoleWindow consoleWindow = log as ConsoleWindow;
        if (consoleWindow == null)
          return;
        string firstParam = GameConsole.GetFirstParam(toArgs);
        for (int index = 0; index < GameConsole.commands.Count; ++index)
        {
          if (GameConsole.commands[index].Equals(firstParam, StringComparison.OrdinalIgnoreCase))
          {
            foreach (string breakIntoLine in Utils.BreakIntoLines(consoleWindow.Font, consoleWindow.Size.X, consoleWindow.TextScale, GameConsole.commandFullHelp[index], true, (char[]) null, false))
              log.WriteLine(breakIntoLine);
            return;
          }
        }
        log.WriteLine("unknown command: " + firstParam);
      }
      else
      {
        for (int index = 0; index < GameConsole.commands.Count; ++index)
          log.WriteLine(GameConsole.commands[index] + "  -- " + GameConsole.commandHelp[index]);
      }
    }

    private static void CmdInv(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      string[] toArgs = GameConsole.ParseToArgs(command);
      if (toArgs.Length < 2)
      {
        log.WriteLine("insufficient arguments.");
      }
      else
      {
        int firstParamIndex = GameConsole.GetFirstParamIndex(toArgs);
        Item? itemIdFromToken = GameConsole.parser.GetItemIDFromToken(GameConsole.GetParam(toArgs, firstParamIndex), false, false, false);
        int? nullable = GameConsole.parser.GetIntFromToken(GameConsole.GetParam(toArgs, firstParamIndex + 1));
        if (GameConsole.HasSwitch(toArgs, "-a") || !GameConsole.HasAnySwitch(toArgs))
        {
          if (!game.World.IsCreativeMode)
            log.WriteLine("command only valid on creative worlds");
          else if (itemIdFromToken.HasValue)
          {
            if (!nullable.HasValue)
              nullable = new int?(ItemData.GetStackSize(itemIdFromToken.Value));
            int inventory = player.AddToInventory(new InventoryItem(itemIdFromToken.Value, nullable.Value));
            if (inventory > 0)
            {
              bool flag = GameConsole.HasSwitch(toArgs, "-e");
              if (flag)
                player.EquipFromInventory(itemIdFromToken.Value);
              string str = inventory.ToString() + " " + ItemData.ToString(itemIdFromToken.Value) + " added";
              if (flag)
                str += " and equipped";
              log.WriteLine(str + ".");
            }
            else
              log.WriteLine("could not add item");
          }
          else
            log.WriteLine("item not found");
        }
        else if (GameConsole.HasSwitch(toArgs, "-e"))
        {
          if (itemIdFromToken.HasValue)
            player.EquipFromInventory(itemIdFromToken.Value);
          else
            log.WriteLine("item not found");
        }
        else if (GameConsole.HasSwitch(toArgs, "-r"))
        {
          if (itemIdFromToken.HasValue)
          {
            if (!nullable.HasValue)
              nullable = new int?(int.MaxValue);
            int num = player.Inventory.DecrementItem(itemIdFromToken.Value, nullable.Value);
            log.WriteLine(string.Format("{0} {1} removed.", (object) (nullable.Value - num), (object) ItemData.ToString(itemIdFromToken.Value)));
          }
          else
            log.WriteLine("item not found");
        }
        else if (GameConsole.HasSwitch(toArgs, "-ra"))
        {
          player.Inventory.Clear();
          log.WriteLine("inventory cleared");
        }
        else
          log.WriteLine("unknown switch");
      }
    }

    private static void CmdKill(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      if (!game.World.IsCreativeMode)
      {
        log.WriteLine("command only valid on creative worlds");
      }
      else
      {
        string[] toArgs = GameConsole.ParseToArgs(command);
        if (toArgs.Length < 2)
          log.WriteLine("insufficient arguments.");
        else if (GameConsole.HasSwitch(toArgs, "-p"))
        {
          player?.ChangeState(ActorState.Dying);
        }
        else
        {
          ActorType? nullable1 = GameConsole.HasSwitch(toArgs, "-a") ? new ActorType?(ActorType.None) : GameConsole.GetActorType(toArgs[1]);
          if (!nullable1.HasValue)
          {
            log.WriteLine("unknown argument: " + toArgs[1]);
          }
          else
          {
            int num = 0;
            List<ITMActor> npcList = game.World.NpcManager.NpcList;
            for (int index = npcList.Count - 1; index >= 0; --index)
            {
              ActorType? nullable2 = nullable1;
              if ((nullable2.GetValueOrDefault() != ActorType.None ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
              {
                ActorType actorType = npcList[index].ActorType;
                ActorType? nullable3 = nullable1;
                if ((actorType != nullable3.GetValueOrDefault() ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
                  continue;
              }
              game.World.NpcManager.DeactivateNpc(npcList[index]);
              ++num;
            }
            log.WriteLine(num.ToString() + " npcs terminated");
          }
        }
      }
    }

    private static void CmdList(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      string[] toArgs = GameConsole.ParseToArgs(command);
      if (toArgs.Length < 2)
        log.WriteLine("insufficient arguments.");
      else if (GameConsole.HasSwitch(toArgs, "-i"))
        GameConsole.CmdListInv(toArgs, player, log);
      else if (GameConsole.HasSwitch(toArgs, "-m"))
        GameConsole.CmdListMarkers(toArgs, game, log, false);
      else if (GameConsole.HasSwitch(toArgs, "-mg"))
        GameConsole.CmdListMarkers(toArgs, game, log, true);
      else if (GameConsole.HasSwitch(toArgs, "-p"))
        GameConsole.CmdListPlayers(toArgs, game, log);
      else if (GameConsole.HasSwitch(toArgs, "-pe"))
        GameConsole.CmdListPerms(toArgs, player, log);
      else if (GameConsole.HasSwitch(toArgs, "-s"))
        GameConsole.CmdListScripts(toArgs, game, log);
      else if (GameConsole.HasSwitch(toArgs, "-tp"))
        GameConsole.CmdListTp(toArgs, player, log);
      else if (GameConsole.HasSwitch(toArgs, "-z"))
        GameConsole.CmdListZones(toArgs, game, log);
      else
        log.WriteLine("unknown argument: " + toArgs[1]);
    }

    private static void CmdListInv(string[] args, ITMPlayer player, IOutputLog log)
    {
      int count = player.Inventory.Items.Count;
      if (count > 0)
      {
        int lineCount = log.LineCount;
        string firstParam = GameConsole.GetFirstParam(args);
        for (int index = 0; index < count; ++index)
        {
          InventoryItem inventoryItem = player.Inventory.Items[index];
          if (inventoryItem.ItemID != Item.None && (firstParam == null || Globals1.ItemData[(int) inventoryItem.ItemID].IDString.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase)))
            log.WriteLine(string.Format("{0}: {1}", (object) inventoryItem.ItemID, (object) inventoryItem.Count));
        }
        if (lineCount != log.LineCount)
          return;
        log.WriteLine("no matching inventory items found");
      }
      else
        log.WriteLine("inventory is empty");
    }

    private static void CmdListMarkers(string[] args, ITMGame game, IOutputLog log, bool graves)
    {
      int lineCount = log.LineCount;
      string firstParam = GameConsole.GetFirstParam(args);
      foreach (MapMarker mapMarker in graves ? game.World.GraveMarkers : game.World.MapMarkers)
      {
        if (firstParam == null || mapMarker.Label.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
          log.WriteLine(string.Format("{0}  [{1},{2},{3}]", (object) mapMarker.Label, (object) mapMarker.Point.X, (object) mapMarker.Point.Y, (object) mapMarker.Point.Z));
      }
      if (lineCount != log.LineCount)
        return;
      log.WriteLine("no " + (firstParam != null ? "matching " : "") + "markers found");
    }

    private static void CmdListPlayers(string[] args, ITMGame game, IOutputLog log)
    {
      List<ITMPlayer> result = new List<ITMPlayer>(24);
      game.GetAllPlayers(result);
      int lineCount = log.LineCount;
      string firstParam = GameConsole.GetFirstParam(args);
      foreach (ITMPlayer tmPlayer in result)
      {
        if (firstParam == null || tmPlayer.Name.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
          log.WriteLine(tmPlayer.Name);
      }
      if (lineCount != log.LineCount)
        return;
      log.WriteLine("no matching players found");
    }

    private static void CmdListPerms(string[] args, ITMPlayer player, IOutputLog log)
    {
      int lineCount = log.LineCount;
      string firstParam = GameConsole.GetFirstParam(args);
      foreach (string buildEnumString in Utils.BuildEnumStringArray<Permissions>())
      {
        if (firstParam == null || buildEnumString.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
        {
          Permissions? enumFromString = Utils.GetEnumFromString<Permissions>(buildEnumString);
          if (player.HasPermission(enumFromString.Value))
            log.WriteLine(buildEnumString);
        }
      }
      if (lineCount != log.LineCount)
        return;
      log.WriteLine("player has no " + (firstParam != null ? "matching " : "") + "permissions");
    }

    private static void CmdListScripts(string[] args, ITMGame game, IOutputLog log)
    {
      GameInstance gameInstance = (GameInstance) game;
      int lineCount = log.LineCount;
      string firstParam = GameConsole.GetFirstParam(args);
      foreach (Script script in gameInstance.Scripts)
      {
        if (firstParam == null || script.Name.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
          log.WriteLine(script.Name);
      }
      if (lineCount != log.LineCount)
        return;
      log.WriteLine("no " + (firstParam != null ? "matching " : "") + "scripts found");
    }

    private static void CmdListTp(string[] args, ITMPlayer player, IOutputLog log)
    {
      if (player.Teleports != null && player.Teleports.Count > 0)
      {
        int lineCount = log.LineCount;
        string firstParam = GameConsole.GetFirstParam(args);
        foreach (KeyValuePair<string, TeleportMark> teleport in player.Teleports)
        {
          if (firstParam == null || teleport.Key.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
            log.WriteLine(string.Format("{0}: {1}", (object) teleport.Key, (object) teleport.Value.Position));
        }
        if (lineCount != log.LineCount)
          return;
        log.WriteLine("no matching teleports found");
      }
      else
        log.WriteLine(player.Name + " has no teleports set");
    }

    private static void CmdListZones(string[] args, ITMGame game, IOutputLog log)
    {
      int lineCount = log.LineCount;
      string firstParam = GameConsole.GetFirstParam(args);
      foreach (Zone zone in game.World.Zones)
      {
        if (firstParam == null || zone.Name.StartsWith(firstParam, StringComparison.OrdinalIgnoreCase))
        {
          string name = zone.Name;
          if (zone.HasZoneType(ZoneType.Spawn))
            name += " [spawn]";
          if (zone.HasZoneType(ZoneType.NoCombat))
            name += " [no pvp]";
          if (zone.HasZoneType(ZoneType.NoEdit))
            name += " [no edit]";
          if (zone.HasZoneType(ZoneType.NoFly))
            name += " [no fly]";
          if (zone.HasZoneType(ZoneType.NoEscape))
            name += " [no escape]";
          if (zone.HasZoneType(ZoneType.NoMobs))
            name += " [no mobs]";
          log.WriteLine(name + string.Format("  [{0},{1},{2} - {3},{4},{5}]", (object) zone.Min.X, (object) zone.Min.Y, (object) zone.Min.Z, (object) zone.Max.X, (object) zone.Max.Y, (object) zone.Max.Z));
        }
      }
      if (lineCount != log.LineCount)
        return;
      log.WriteLine("no " + (firstParam != null ? "matching " : "") + "zones found");
    }

    private static void CmdMsg(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      List<string> toArgsDblQuotes = GameConsole.ParseToArgsDblQuotes(command);
      if (toArgsDblQuotes.Count < 2)
      {
        log.WriteLine("insufficient arguments.");
      }
      else
      {
        bool admins = GameConsole.HasSwitch(toArgsDblQuotes, "-a");
        ITMPlayer recipient = (ITMPlayer) null;
        if (!admins && toArgsDblQuotes.Count > 2)
        {
          string lower = toArgsDblQuotes[1].Trim().ToLower();
          recipient = game.GetPlayer(lower);
          if (recipient == null)
          {
            log.WriteLine("player not found: " + lower);
            return;
          }
        }
        game.SendTextMessage(toArgsDblQuotes[toArgsDblQuotes.Count - 1], caller, recipient, false, admins);
      }
    }

    private static void CmdNotify(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      List<string> toArgsDblQuotes = GameConsole.ParseToArgsDblQuotes(command);
      if (toArgsDblQuotes.Count < 2)
      {
        log.WriteLine("insufficient arguments.");
      }
      else
      {
        NotifyRecipient recType = NotifyRecipient.Global;
        NotifyRecipient notifyRecipient;
        if (toArgsDblQuotes.Count > 2)
        {
          if (GameConsole.HasSwitch(toArgsDblQuotes, "-a"))
            notifyRecipient = NotifyRecipient.Admin;
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-c"))
            notifyRecipient = NotifyRecipient.Clan;
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-g"))
            notifyRecipient = NotifyRecipient.Global;
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-l"))
            notifyRecipient = NotifyRecipient.Local;
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-r"))
            notifyRecipient = NotifyRecipient.Remote;
          else
            log.WriteLine("unknown recipient type");
        }
        else
          game.AddNotification(toArgsDblQuotes[toArgsDblQuotes.Count - 1], recType);
      }
    }

    private void CmdPlayer(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      string[] toArgs = GameConsole.ParseToArgs(command);
      if (toArgs.Length < 2)
      {
        this.SetPlayer(caller);
      }
      else
      {
        ITMPlayer player1 = game.GetPlayer(toArgs[1]);
        if (player1 != null)
          this.SetPlayer(player1);
        else
          log.WriteLine("player: " + toArgs[1] + " not found.");
      }
    }

    private static void CmdScript(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      if (!game.World.IsCreativeMode)
        log.WriteLine("command only valid on creative worlds");
      else if (GameConsole.HasSwitch(GameConsole.ParseToArgs(command), "-c"))
      {
        int num = command.IndexOf(" -c ");
        if (num < 0)
          command.IndexOf(" /c ");
        if (num < 0)
          log.WriteLine("cannot parse command");
        else
          game.RunSingleScriptCommand(command.Substring(num + 4).Trim(), (ITMActor) player);
      }
      else
      {
        int num = command.IndexOf(" -s ");
        if (num < 0)
          command.IndexOf(" /s ");
        if (num < 0)
          num = 3;
        if (command.Length > num + 4)
        {
          string script = command.Substring(num + 4).Trim().Trim();
          if (game.RunScript(script, (ITMActor) player))
            return;
          log.WriteLine("script not found: " + script);
        }
        else
          log.WriteLine("no script specified");
      }
    }

    private static void CmdTp(
      string command,
      ITMGame game,
      ITMPlayer caller,
      ITMPlayer player,
      IOutputLog log)
    {
      if (!game.World.IsCreativeMode)
      {
        log.WriteLine("command only valid on creative worlds");
      }
      else
      {
        List<string> toArgsDblQuotes = GameConsole.ParseToArgsDblQuotes(command);
        if (toArgsDblQuotes.Count < 2)
        {
          log.WriteLine("insufficient arguments.");
        }
        else
        {
          string str = toArgsDblQuotes[toArgsDblQuotes.Count - 1];
          if (str.IsEmpty())
            log.WriteLine("invalid arguments.");
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-a"))
          {
            player.AddTeleport(str);
            log.WriteLine("teleport point added.");
          }
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-r"))
          {
            if (player.RemoveTeleport(str))
              log.WriteLine("teleport point added.");
            else
              log.WriteLine("teleport point not found.");
          }
          else if (GameConsole.HasSwitch(toArgsDblQuotes, "-ra"))
          {
            List<string> stringList = new List<string>(player.Teleports.Count);
            foreach (KeyValuePair<string, TeleportMark> teleport in player.Teleports)
              stringList.Add(teleport.Key);
            foreach (string name in stringList)
              player.RemoveTeleport(name);
          }
          else
          {
            if (player.TeleportTo(str))
              return;
            log.WriteLine("teleport point not found.");
          }
        }
      }
    }
  }
}
