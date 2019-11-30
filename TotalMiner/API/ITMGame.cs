// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMGame
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.API
{
  public interface ITMGame
  {
    ITMWorld World { get; }

    /// <summary>
    /// A general random number generator that can be used anywhere.
    /// </summary>
    PcgRandom Random { get; }

    IAudioManagerStream AudioManager { get; }

    GraphicsDevice GraphicsDevice { get; }

    WindowManager WindowManager { get; }

    ITMTexturePack TexturePack { get; }

    ITMPlayer GetPlayer(string gamertag);

    ITMPlayer GetLocalPlayer(PlayerIndex playerIndex);

    void GetAllPlayers(List<ITMPlayer> result);

    /// <summary>
    /// Adds a new 'pink' notification to the top left of the screen.
    /// </summary>
    /// <param name="message">The local notification message text.</param>
    void AddNotification(string message);

    /// <summary>
    /// Adds a new 'pink' notification to the top left of the screen.
    /// </summary>
    /// <param name="message">The notification message text.</param>
    /// <param name="recType">Who receives the notification.</param>
    void AddNotification(string message, NotifyRecipient recType);

    /// <summary>
    /// Adds an Action method that will be executed when a block is mined.
    /// </summary>
    /// <param name="blockID">The block that was mined or Block.None for all blocks.</param>
    /// <param name="action">The method to execute. Block=The block that was mined. Byte=Aux data of the block before it was mined. GlobalPoint3D=Block position. ITMHand=The hand that mined the block. From the hand you can get the owner/player.</param>
    void AddEventBlockMined(Block blockID, Action<Block, byte, GlobalPoint3D, ITMHand> action);

    /// <summary>
    /// Adds an Action method that will be executed when the block is placed.
    /// </summary>
    /// <param name="blockID">The block that was placed or Block.None for all blocks.</param>
    /// <param name="action">The method to execute. Block=The block that was placed. GlobalPoint3D=Block position. ITMHand=The hand that placed the block. From the hand you can get the owner/player.</param>
    void AddEventBlockPlaced(Block blockID, Action<Block, GlobalPoint3D, ITMHand> action);

    /// <summary>
    /// Adds an Action method that will be executed when the item is swung and the swing has reached it's extended position.
    /// </summary>
    /// <param name="itemID">The item being swung or Item.None for all items.</param>
    /// <param name="action">The method to execute. Item=The item swung. ITMHand=The hand that swung the item.</param>
    void AddEventItemSwing(Item itemID, Action<Item, ITMHand> action);

    /// <summary>
    /// Flags an item has having a custom setup screen (from the prospect/interact screen). When a player presses A for Setup on the Interact screen, your ITMPluginGUI object is called for a new instance of the setup screen.
    /// </summary>
    /// <param name="itemID">The Item that has a custom setup screen. This includes Blocks.</param>
    /// <param name="permission">The permissions the player must have to open the screen.</param>
    void AddItemCustomSetup(Item itemID, Permissions permission);

    /// <summary>Add a command to the game console window.</summary>
    /// <param name="action">The method to execute the command.</param>
    /// <param name="cmd">The command (text).</param>
    /// <param name="briefHelp">Brief help message, usually one line long, listing any parameters. Displayed by the 'help' command.</param>
    /// <param name="fullHelp">A full help message, describing the command and each parameter on a separate line. Displayed by the 'help [command]' command.</param>
    void AddConsoleCommand(
      Action<string, ITMGame, ITMPlayer, ITMPlayer, IOutputLog> action,
      string cmd,
      string briefHelp,
      string fullHelp);

    /// <summary>Run a raw unparsed console command.</summary>
    /// <param name="command">Raw unparsed command text.</param>
    /// <param name="caller">The player who executed the command.</param>
    /// <param name="player">Target player.</param>
    /// <param name="log">Output log.</param>
    /// <returns>True if the command was executed.</returns>
    bool RunConsoleCommand(string command, ITMPlayer caller, ITMPlayer player, IOutputLog log);

    /// <summary>Run an existing script.</summary>
    /// <param name="script">Name of script, including full path.</param>
    /// <param name="actor">Actor the script is executed for.</param>
    /// <returns>True if the script was found.</returns>
    bool RunScript(string script, ITMActor actor);

    /// <summary>Run a single script command.</summary>
    /// <param name="command">Full script command text, unparsed.</param>
    /// <param name="actor">Actor the script command is executed for.</param>
    void RunSingleScriptCommand(string command, ITMActor actor);

    /// <summary>Send a text message.</summary>
    /// <param name="message">The message text.</param>
    /// <param name="sender">The player who sent the message.</param>
    /// <param name="recipient">The recipient. Pass null to send to all players.</param>
    /// <param name="clan">True if the message is to be sent to all members of recipients clan.</param>
    /// <param name="admins">True if the message is to be sent to all admins (recipient and clan are ignored).</param>
    void SendTextMessage(
      string message,
      ITMPlayer sender,
      ITMPlayer recipient,
      bool clan,
      bool admins);

    /// <summary>
    /// Called by the game when a text message is received from a remote gamer.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="target">Recipient target(s).</param>
    void ReceiveTextMessage(string message, NetworkGamer sender, TextMsgTarget target);

    /// <summary>
    /// Removes an block mined event that was previously added with AddEventBlockMined(..).
    /// </summary>
    /// <param name="blockID">The block that was mined.</param>
    /// <param name="action">The method to execute.</param>
    void RemoveEventBlockMined(Block blockID, Action<Block, byte, GlobalPoint3D, ITMHand> action);

    /// <summary>
    /// Removes an block placed event that was previously added with AddEventBlockPlaced(..).
    /// </summary>
    /// <param name="blockID">The block that was placed.</param>
    /// <param name="action">The method to execute.</param>
    void RemoveEventBlockPlaced(Block blockID, Action<Block, GlobalPoint3D, ITMHand> action);

    /// <summary>
    /// Removes an item swing event that was previously added with AddEventItemSwing(..).
    /// </summary>
    /// <param name="itemID">The item being swung.</param>
    /// <param name="action">The method to execute.</param>
    void RemoveEventItemSwing(Item itemID, Action<Item, ITMHand> action);

    /// <summary>
    /// Opens the Pause menu with your custom menu pre-selected
    /// </summary>
    /// <param name="menu">A custom menu</param>
    /// <param name="player">The player who opened the screen</param>
    void OpenPauseMenu(NewGuiMenu menu, ITMPlayer player);
  }
}
