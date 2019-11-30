// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HowToMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class HowToMenuScreen : BlockMenuScreen
  {
    public static string PermissionOverviewText = "The permission system allows for 5 tiers of players.\r\n 1. Non participants\r\n 2. Adventurers\r\n 3. Restricted Builders (and advanced adventurers)\r\n 4. Creators\r\n 5. Admins";
    public static string NonParticipantPermissionText = "Non Participant: [No permissions]\r\nThese players have no permissions and can't do much. It is better to give a player at least Adventure permission so that they can participate in the game to some degree.\r\nWhat they can do:\r\n   Access crates.\r\n   Walk around.\r\n   Kill mobs.\r\n   Read books (no edit).\r\n   Read NPC speech bubbles (no edit).\r\n   Open unlocked doors.";
    public static string AdventurePermissionText = "Adventurer: [Adventure permission]\r\nThese players have restricted permissions but are still able to play in the world.\r\nWhat they can do:\r\n  Collect Pickups.\r\n  Access Unlocked Chests, Furnaces, Bookcases.\r\n  Access Economized Shops (but not economize them).\r\n  Rate worlds.";
    public static string EditPermissionText = "Restricted Builders and Advanced Adventurers: [Edit Permission]\r\nThese players are able to individually edit *non grief blocks so that they can mine resources and build structures. They do not have access to any feature which allows serious griefing.\r\n*grief blocks: Spider Eggs, Steel Spikes, NPC Spawners, Turrets, Mines.\r\nWhat they can do:\r\n  Individually edit *non grief blocks.\r\n  Set (existing) Textures/Keys/Paintings/Teleport Channels.\r\n  Decal Applicator.\r\n  Economize shops.\r\n  Toggle their nameplate visibility to Far.\r\n  Creative -> Measure.\r\n  Creative -> Remove Markers.";
    public static string CreativePermissionText = "Creators: [Creative permission]\r\nThese players are your world builders, they have fairly unrestricted access to building and creative tools.\r\nWhat they can do:\r\n  Access infinite shops from anywhere on Creative worlds with Finite resource turned off.\r\n  Use all Creative Tools except Creative Flood (Creative mode only).\r\n  Place Spider eggs, Steel spikes, NPC spawners, Invisible Barriers.\r\n  Use Ambient Sound and Script blocks.\r\n  Change Frequencies on Wifi blocks.\r\n  Edit NPC speech.\r\n  Edit / Copy Books.\r\n  Add/Remove Textures/Keys/Paintings/Teleport Channels.";
    public static string GriefPermissionText = "Grief:\r\nTurning this permission off protects against accidental or intentional use of features that can cause serious grief.\r\nWhat they can do:\r\n  Start Fires.\r\n  Blast permission.\r\n  Use buckets to Flood.\r\n  Use Creative Flood (Creative mode only).";
    public static string FlyPermissionText = "Players with Fly Permission can:\r\n  Fly around the map.\r\n  Note: Players using either the Robotic, Testerman or Hermes Wraith Avatars are not bound by this permission, so they can fly even if they do not have the fly permission.";
    public static string MapPermissionText = "Players with Map Permission can:\r\n  View their mini map.\r\n  Toggle their mini map on or off.\r\n  View the top view surface map.";
    public static string VoiceChatPermissionText = "Players with VoiceChat Permission can:\r\n  Use in game voice chat.\r\n  Note: In game voice chat uses signficant network bandwidth. Turning this permission off can help when your world is under load.";
    public static string TextChatPermissionText = "Players with TextChat Permission can:\r\n  Use the in game text chat system.";
    public static string SpectatePermissionText = "Players with Spectate Permission can:\r\n  Spectate other players.";
    public static string ShopPermissionText = "Players with Shops Permission can:\r\n  Access System shops (with 1 exception, they cannot buy skeleton keys).\r\n  Note: If your world is reliant on a resource based economy then this permission should only be granted to Admins.";
    public static string ViewScriptsPermissionText = "Players with View Scripts Permission can:\r\n  View existing scripts.\r\n  They cannot add, modify, delete or assign scripts.";
    public static string SavePermissionText = "Players with Save Permission can:\r\n  Save their clipboard as a component to their own Xbox.";
    public static string AdminPermissionText = "Admin:\r\nThese players are your world administrators.\r\nWhat they can do:\r\n  Set permissions for other non admin players.\r\n  Use Zones.\r\n  Admins are unaffected by Zone settings.\r\n  Edit Scripts.\r\n  Teleport to Marker.\r\n  Teleport to Player.\r\n  No Clip (Creative maps only).\r\n  Time FFWD/REV (Creative maps only).\r\n  Access the Creative Menu even when Finite Resources is turned on (Creative maps only).\r\n  Access infinite shops from anywhere on Creative worlds.\r\n  Remove locked chests and locked doors placed by other players.\r\n  Edit inside Non-Edit Zones if (any) builder is assigned to the zone.\r\n  Buy Skeleton keys from system shops.\r\n  Use the Admin Teleport Channel (Red).\r\n  Unrestricted Mini Map.\r\n  Toggle most game options.";
    public static string PermissionHowToText = HowToMenuScreen.PermissionOverviewText + "\n\n" + HowToMenuScreen.NonParticipantPermissionText + "\n\n" + HowToMenuScreen.AdventurePermissionText + "\n\n" + HowToMenuScreen.EditPermissionText + "\n\n" + HowToMenuScreen.CreativePermissionText + "\n\n" + HowToMenuScreen.FlyPermissionText + "\n\n" + HowToMenuScreen.MapPermissionText + "\n\n" + HowToMenuScreen.VoiceChatPermissionText + "\n\n" + HowToMenuScreen.TextChatPermissionText + "\n\n" + HowToMenuScreen.SpectatePermissionText + "\n\n" + HowToMenuScreen.ShopPermissionText + "\n\n" + HowToMenuScreen.ViewScriptsPermissionText + "\n\n" + HowToMenuScreen.GriefPermissionText + "\n\n" + HowToMenuScreen.SavePermissionText + "\n\n" + HowToMenuScreen.AdminPermissionText + "\n\n";
    private string[] videoList = new string[11]
    {
      "Lobby",
      "Options",
      "Using Markers",
      "Fill Clear Replace",
      "Creative Flooding",
      "Copy and Paste",
      "Components",
      "Circuitry",
      "Zones",
      "Scripts",
      "Multiplayer"
    };
    private Dictionary<string, HowToMenuScreen.HowToData> itemData;
    private GameInstance instance;
    private HowToIndex index;

    public HowToMenuScreen(GameInstance instance, Player player, HowToIndex index)
      : base("How To", player)
    {
      this.instance = instance;
      this.index = index;
      this.LoadText();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      foreach (KeyValuePair<string, HowToMenuScreen.HowToData> keyValuePair in this.itemData)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, keyValuePair.Key);
        blockMenuEntry.Tag = (object) keyValuePair.Value;
        blockMenuEntryList.Add(blockMenuEntry);
        blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ItemSelected);
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 432;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void LoadText()
    {
      this.itemData = new Dictionary<string, HowToMenuScreen.HowToData>();
      switch (this.index)
      {
        case HowToIndex.Main:
          this.itemData.Add("General", new HowToMenuScreen.HowToData()
          {
            UnlockID = 0,
            Text = "Total Miner has four game modes.\r\n\r\nDig Deep\r\nSurvival\r\nPeaceful\r\nCreative\r\n\r\nDig Deep, Survival and Peaceful modes are all Finite Resouce modes. That means you must mine, collect or purchase any resources you need for your adventures.\r\n\r\nThe objective of Dig Deep is to explore, adventure and treasure hunt your way to the bottom of the world. Dig Deep worlds are very deep. You must make your way downwards, through progressively harder types of rock, using progressively stronger types of pick axe. In Dig Deep mode, you must also find blueprints for all items you wish to craft. Because blueprints are scattered around the world, you must explore all of the world to find them. Blueprints for higher level items are found deeper in the world.\r\n\r\nThe objective of Survival is to adventure and build while staying alive. Mobs will spawn at night and try to kill you.\r\n\r\nPeaceful is a game mode for builders who want to play in a finite resource world, but do not want to be bothered about annoying things like being killed.\r\n\r\nSurvival and Peaceful maps have 4 times the surface area of Dig Deep maps, but they are shallow so it is much easier to collect precious ores. They also do not have blueprints, so you can craft all items right from the start.\r\n\r\nCreative Mode is an infinite resource construction sand box. You can create any structure you can imagine without having to collect the resources first. Creative mode also gives you access to Creative tools which make certain types of building tasks much easier and faster (see the Creative Tools item on this How To menu).\r\n\r\nCreative Mode also allows you to switch to Finite Resource mode. This allows Map makers to build their worlds (like an RPG style adventure map) using infinite resources, and then change over to finite resources when they are ready to let other players join their worlds and play in a finite resource sandbox.\r\n\r\n\r\nBasic Controls:\r\n\r\nUse the left stick to move around the world.\r\nUse the right stick to look around you.\r\nPress A to jump. Double tap A to double jump.\r\nPress Y to open your Inventory screen where you can equip items to use.\r\nPress X to open your Crafting screen where you can craft new items.\r\nPress B to Prospect or Interact with the block your recepticle is targeting.\r\nPress Left Trigger to place a block.\r\nPress and hold the Right Trigger to mine a block.\r\nUse the Left and Right Bumpers to select items in your Hot-Bar.\r\nPress Back to open the World Map view.\r\nPress Start to open the Pause Menu.\r\nClick the Right stick to toggle fly mode.\r\n"
          });
          this.itemData.Add("Crafting", new HowToMenuScreen.HowToData()
          {
            Index = HowToIndex.Crafting
          });
          this.itemData.Add("Know Your Tools", new HowToMenuScreen.HowToData()
          {
            UnlockID = 1,
            Text = "Total Miner has over 200 different items and tools. They are all either collectable or craftable or both.\r\n\r\nThere are many types of Pickaxes, Shovels, Hatchets, Hoes, Scythes, Swords, Spears, Battle Axes, Bows, Arrows, and Armor. There are also many items or raw materials used for crafting, food, metal bars, keys, jewelry and rare items (both with special powers).\r\n\r\nMost tools have advantages and disadvantages. For example, a Pickaxe is great for mining rock, but not so good with porous material like dirt and sand. A Shovel is great for porous material, but not so good with rock. Similarly a Hatchet is great for chopping wood materials but not so good with rock or porous material.\r\n\r\nIf you want to chop wood, use a Hatchet. If you want to dig dirt, sand or other porous material, use a Shovel. If you want to mine rock, use a Pickaxe.\r\n\r\nSwords and Spears also have different properties making them useful for different combat situations. Spears have a longer reach than swords, but swing slower. Spears are typically better for defensive combat, swords are typically better for aggressive combat.\r\n\r\nTools and weapons made from wood are cheap and easy to come by (or craft) but they degrade quickly. Wood tools can only deal with porous material and soft rock. Iron and steel tools and weapons are stronger and last longer than their wood counterparts but are more expensive. Diamond, ruby and titanium tools and weapons are the best, but they are very expensive and it's hard to find the materials needed to craft them.\r\n\r\nKnow your tools and become a Total Miner!\r\n"
          });
          this.itemData.Add("Hotbar", new HowToMenuScreen.HowToData()
          {
            UnlockID = 9,
            Text = "The Hotbar in Total Miner works slightly differently to other similar games because it has to manage dual wielding (items can be equipped in both hands).\r\n\r\nThe Hotbar has two cursors. The yellow cursor is for equipping items in your left hand, and the white cursor is for equipping items in your right hand.\r\n\r\nTo equip an item in your left hand, click the left bumper (shoulder button) until the yellow cursor is on the item you wish to equip.\r\n\r\nTo equip an item in your right hand, click the right bumper until the white cursor is on the item you wish to equip.\r\n\r\nBy default the yellow (left hand) cursor moves left when you click the left bumper. The white (right hand) cursor moves right when you click the right bumper.\r\n\r\nYou can move the yellow cursor to the right by pressing and holding the left bumper and then clicking the right bumper to move it to the right.\r\n\r\nYou can move the white cursor to the left by pressing and holding the right bumper and then clicking the left bumper to move it to the left.\r\n\r\nEvery item in Total Miner can only be equipped in a particular hand, they are not interchangeable. It is up to you to remember which hand an item can be used in, but in general all blocks that can be placed down on the map will go in your left hand, and any items that are not placed on the map, like tools, weapons, food etc are used in your right hand.\r\n\r\nIf a cursor is over an item but that item is not equipped (you cannot see it in your hand), then the item must be equipped in the other hand.\r\n\r\nIf you find the Hotbar obscures your view you can go to Pause->Player->Options and set Hotbar Transparency On. This will tell the Hotbar to fade to semi-transparent when it is not in use.\r\n\r\nIf the Hotbar is transparent and you just want to see what is in your Hotbar without changing a cursor position, press and hold either bumper for 1 second or more. The cursor will turn gray and when you release the bumper, the cursor will not move.\r\n"
          });
          this.itemData.Add("Zones", new HowToMenuScreen.HowToData()
          {
            UnlockID = 2,
            Text = "Zones allow Admins to define areas inside the map with different attributes:\r\n\r\nTo create a zone, place Marker blocks to define the zone area, then press Pause -> Game -> Zones -> Add Zone. Enter the zone name, and the zone is created.\r\n\r\nSpawn Zones: Players will spawn in these zones when they first enter a map, escape or die. Spawn zones must be at least two blocks high and cannot have any solid blocks inside them.\r\n\r\nNo PvP Zones: Players cannot engage in PvP combat if they are in these zones.\r\n\r\nNo Edit Zones: Blocks inside these zones cannot be changed.\r\n\r\nNo Fly Zones: Players cannot fly inside these zones.\r\n\r\nBuilder: No Edit Zones have a 2nd option. You can nominate a single player as the builder to a No Edit Zone and only that player can edit inside that zone. If a builder is nominated, Admins can also edit inside that zone. If a builder is not nominated, nobody can edit inside that zone.\r\n\r\nScript: If you select a script here then it will be executed when (any) player enters the zone.\r\n\r\nA zone can be specified with one or any combination of the above attributes, except spawn zones which are always No Edit zones.\r\n\r\nZones can be of any size and multiple zones can overlap each other.\r\n\r\nIf a player is located in more than one zone (overlapping zones), then the zone with the strictest setting takes precedence, e.g. No Edit over Edit, No PvP over PvP, No Fly over Fly, unless the zone with the less strict setting is completely contained inside the zone with the stricter setting. This allows for Edit zones inside No Edit zones, PvP zones inside No PvP zones, Fly zones inside No Fly zones.\r\n\r\nPress Y on the Edit Zone screen to delete the highlighted zone.\r\n\r\nImportant: Admins are NOT affected by zones.\r\n"
          });
          this.itemData.Add("Permissions", new HowToMenuScreen.HowToData()
          {
            UnlockID = 3,
            Text = HowToMenuScreen.PermissionHowToText
          });
          this.itemData.Add("Player Skills", new HowToMenuScreen.HowToData()
          {
            UnlockID = 4,
            Text = "There are 15 player skills in Total Miner. \r\n\r\nAs your perform the actions of each skill type, you will gain experience points (XP) for that skill. \r\n\r\nAs you gain more XP, your level for that skill increases, allowing you to craft and use better items, and perform the relevant tasks faster or stronger.\r\n\r\nThe skill system can be disabled for maps on the Lobby screen. In creative maps, the skill system can be toggled if Finite Resources is on.\r\n"
          });
          this.itemData.Add("Circuitry", new HowToMenuScreen.HowToData()
          {
            UnlockID = 5,
            Text = "Circuitry is power. The power to manipulate the world around you using triggers and logic. \r\n\r\nBlocks such as pressure plates, switches and buttons will trigger power, and blocks such as steel doors, mob spawners, mines, turrets, sound blocks, and more will react to that power and make your world more dynamic.\r\n\r\nCircuitry in Total Miner does not use wires. If you need to transmit power over a distance, use the WIFI Transmitter and WIFI Receiver blocks.\r\n\r\nWIFI Transmitter and WIFI Receiver blocks are linked by frequency. WIFI Transmitter blocks will send a power 'on' signal when they receive power from an adjacent block, such as a switch being turned on. They will also send a power 'off' signal when they lose power from their adjacent blocks, such as when a switch is turned off. All WIFI Receiver blocks will receive this signal if they are within range and of the WIFI Transmitter block and if they have the same frequency. If they receive a power 'on' signal, they will deliver power to their adjacent blocks, otherwise they will cut off power.\r\n"
          });
          this.itemData.Add("Scripts", new HowToMenuScreen.HowToData()
          {
            UnlockID = 6,
            Text = "Scripts are collections of text based commands that can manipulate the world. \r\n\r\nThey can be executed manually by Admins, or triggered via circuitry and the Script block. \r\n\r\nScripts allow you to add, clear and move blocks around, create animations, like opening and closing castle gates, move bridges, etc. They can also fill, clear, replace and move whole regions at once, paste components, restock inventories of chests, spawn mobs, flick switches, manipulate weather effects and change the textures on multi-texture blocks.\r\n\r\nScripts are also useful for automating many admin tasks in adventure maps.\r\n\r\nThe easiest way to learn how script commands are formatted is to first clear your change log, then do the actions in game that you want to script, then create a new script from your change log (on the Script menu).\r\n\r\nNote: Scripts are available to use in Survival and Dig Deep but they are restricted to commands that cannot spawn new items and commands that cannot clear regions of blocks or add new blocks.\r\n\r\nNote: Scripts are only available to use in Dig Deep once the Script block blueprint has been unlocked.\r\n"
          });
          this.itemData.Add("Creative Tools", new HowToMenuScreen.HowToData()
          {
            Index = HowToIndex.CreativeTools
          });
          this.itemData.Add("Local Coop and Multiplayer", new HowToMenuScreen.HowToData()
          {
            UnlockID = 7,
            Text = "To play a local co-op game, start a single player game as usual, then up to three other players can join in by pressing the Start or A button on their controller.\r\n\r\nThe screen will automatically split. With two players you can configure the split as horizontal or vertical in the Options menu.\r\n\r\nCombat can be toggled on or off in the Game options menu.\r\n\r\nOnly the player who started the game can save the game, but all players are saved, so if a player quits and then rejoins later, they will continue as they were at the last save.\r\n\r\nTo spectate other players, hold the DPad UP button and press either the left or right bumper to cycle through the other players. You will see their gamertag at the top of your HUD and you will see the game through their eyes. You must have the Spectate permission to spectate other players.\r\n"
          });
          this.itemData.Add("Sharing", new HowToMenuScreen.HowToData()
          {
            UnlockID = 8,
            Text = "You can share your worlds, component packs and photo's over Xbox LIVE. \r\n\r\nWorld Sharing:\r\n\r\nThis feature is not online multiplayer. It simply lets you send your world file to another player so they can load your world on their Xbox and play in it in single player mode.\r\n\r\nThe sender and receiver both must be signed into Xbox LIVE Gold accounts.\r\n\r\nHow it works:\r\n\r\nThe sender has control of who can receive the world. The sender enters the Share menu from the Main menu and creates a Share Session. The sender then selects which world they want to share.\r\n\r\nThe receiver enters the Share menu and joins the session (gamertag) of the gamer they want to receive the world from.\r\n\r\nWhen the receiver joins a session, the world file is automatically sent to them.\r\n\r\nSend/Receive progress is displayed in the top right of the screen.\r\n\r\nIt is important the sender waits until the receiver has recieved the entire world file before closing the share session otherwise the receive will be aborted.\r\n\r\nDo not close the share session until the worlds have been transmitted. If either the sender or the receiver closes their session, all current and stacked send/receive operations will be aborted.\r\n\r\n\r\nComponent Sharing and Photo Sharing work the same way. From the Share Menu, the sender selects the component pack or photo they wish to share and the receiver joins the share session session to receive the component pack or photo.\r\n"
          });
          break;
        case HowToIndex.Crafting:
          this.itemData.Add("Crafting", new HowToMenuScreen.HowToData()
          {
            UnlockID = 0,
            Text = "Crafting allows you to make new items by combining a number of other items together.\r\n\r\nIn, the Dig Deep game mode, every craftable item has an associated blueprint. You must find the blueprint to unlock the item to be able to craft it. Some basic items are unlocked when you start a new game and you don't need to find a blueprint for them. In all other game modes, you do not need to find blueprints first.\r\n\r\nTo craft an item, open the crafting screen by pressing the X button.\r\n\r\nOn the crafting screen you will see a 2 x 2 grid at the top left. This grid is where you place the materials that are needed to craft an item. Materials are simply other items.\r\n\r\nThere are two ways to craft items.\r\n\r\n1. Manually place the materials directly into the crafting grid yourself using the cursor (they must be placed in the correct position).\r\n\r\n2. Press the Y button and select a blueprint from the displayed lists. Press the left and right bumpers to move between the different tabs on the list. Selecting a blueprint from the list is just an easy way of having the materials automatically put into the crafting grid. It also automates the task of splitting up the materials you have to maximize the number of items you can craft.\r\n\r\nOnce you have either placed the materials yourself, or selected a blueprint, the method to craft is the same: move the cursor over the item in the Product box and press A to craft the item. Keep pressing A or hold A down to craft more. Once you have crafted the number you want, or the Product stack is full, or your materials have depleted, move the crafted item(s) from the Product box into your inventory.\r\n\r\nNote. If you use method 2 (select blueprint from list) the cursor will automatically be placed over the Product box.\r\n\r\nSee the Workbench topic in the How To menu for more advanced crafting techniques.\r\n\r\nHappy Crafting!\r\n"
          });
          this.itemData.Add("Blueprints", new HowToMenuScreen.HowToData()
          {
            UnlockID = 1,
            Text = "In the Dig Deep game mode, you need a blueprint to craft most items. Blueprints are scattered all around the world, both on the surface, and in underground caves.\r\n\r\nBlueprints for basic items will be found on the surface, explore the surface to find them. Blueprints for advanced items will only be found underground.\r\n\r\nYou are equipped with a blueprint finder. This is a blue arrow on your HUD which points to the closest blueprint to you based on the direction you are looking. If there are no blueprints in the immediate area, it will point to the blueprint that is closest to the surface.\r\n\r\nWhen you find a blueprint, punch or dig it out and pick it up. When you pick up the blueprint, it will be displayed showing which item it is for and the materials needed to craft the item. Note the order of the materials in the grid. This order must be used to successfully craft the product item.\r\n\r\nLocked items become unlocked when you pick up their blueprint.\r\n\r\nOnce you have a blueprint, you don't need to carry it around in order to craft the item, so you can either just drop it on the ground, or place it in a chest as a collectable.\r\n"
          });
          this.itemData.Add("The Workbench", new HowToMenuScreen.HowToData()
          {
            UnlockID = 2,
            Text = "Your normal crafting screen gives you a 2 x 2 crafting grid. This is big enough for basic items, but for more advanced items you need a 3 x 3 crafting grid.\r\n\r\nThis is were the Workbench comes in. The Workbench is a normal block like any other, it can be mined or placed. But it is also a special block.\r\n\r\nIf you target it and press the Left Trigger, the Workbench screen is displayed. This screen looks and works exactly like the normal crafting screen except it gives you a 3 x 3 crafting grid, allowing you to craft any craftable item in the game.\r\n\r\nYou can either buy a Workbench at the shop or craft one using the normal 2 x 2 crafting screen grid.\r\n"
          });
          this.itemData.Add("The Furnace", new HowToMenuScreen.HowToData()
          {
            UnlockID = 3,
            Text = "Smelting is similar to crafting. The difference is items are smelted (or cooked) into other items using the heat of a furnace rather than the hands and tools of a craftsmen.\r\n\r\nOpen a Furnace like you would open a Workbench, by pressing the Left Trigger when the Furnace is targeted. \r\n\r\nTo smelt an item, place one or more of the required materials into the Material slots, or like the crafting screen, press Y for the Easy Smelt screen and select the item to smelt from the lists.\r\n\r\nSome items, such as Glass only need one material to be smelted. Other items such as Steel require two or three materials.\r\n\r\nSmelting requires fire, which requires fuel. Place one or more items of fuel into the Fuel slot to burn the fire. Almost any item that you would expect to burn can be used as fuel. Some items burn fast, some burn slow.\r\n\r\nWhen you have successfully put fuel items into the Fuel slot, a yellow bar of fire will slowly burn down, indicating how long that item of fuel can sustain the furnace fire.\r\n\r\nIf you have also successfully placed the correct materials in the Material boxes, a green bar will progress forward indicating how long it takes to smelt the item.\r\n\r\nWhen the item is smelted, it will be added to the Product box. You can take the finished items from the Product box at any time.\r\n\r\nIf you stack many materials and fuel blocks, you can close the furnace and go and do something else. The furnace will continue to smelt the items. Come back later to put the smelted items into your inventory.\r\n\r\nJust be sure to leave enough fuel in the Fuel slot to burn the fire long enough to smelt the stack.\r\n\r\nHappy Smelting!\r\n"
          });
          break;
        case HowToIndex.CreativeTools:
          this.itemData.Add("Marker Blocks", new HowToMenuScreen.HowToData()
          {
            UnlockID = 0,
            Text = "Most creative tools are controlled by marker blocks.\r\n\r\nAll tools except flooding operate on a cubic region (3D). \r\n\r\nAny cubic region can be defined by two or more marker blocks. \r\n\r\nThe marker blocks you place define the extents of the cubic region. \r\n\r\nUse as many marker blocks as you want for convenience when defining the region. It can often be more convenient to place a marker block on the extents of each axis when marking out an area, rather than trying to define it exactly with only two blocks. Only the outer most Marker blocks are considered.\r\n"
          });
          this.itemData.Add("Fill and Clear", new HowToMenuScreen.HowToData()
          {
            UnlockID = 1,
            Text = "Fill and Clear allow you to fill or clear large regions of the map in one operation.\r\n\r\nThe maximum size of a Fill or Clear operation is 500,000 blocks. \r\n\r\nDefine the region to Fill or Clear using two or more marker blocks. \r\n\r\nIf you select Fill, you will then be prompted to select a block. When selected, the region will be filled with that block.\r\n\r\nIf you select Clear, all blocks within the region will be cleared. If there are blocks attached to other blocks in the edges of the region, they will also be cleared.\r\n\r\nIf you select Clear Random, approx 10 percent of blocks at random locations within the region will be cleared. Run the operation multiple times to clear more.\r\n"
          });
          this.itemData.Add("Replace", new HowToMenuScreen.HowToData()
          {
            UnlockID = 2,
            Text = "Replace allows you to replace one block with another block over a large region of the map in one operation.\r\n\r\nThe maximum size of a Replace operation is 500,000 blocks.\r\n\r\nDefine the region to Replace using two or more marker blocks. \r\n\r\nWhen you select Replace, you will be prompted to select the block that is to be replaced. Once you have selected that, you will be prompted again for the block that will replace the old block.\r\n\r\nThere is also a Replace Clipboard option which is available on the Creative Tools menu when a clipboard is equipped. It works the same way as the regular Replace, except it operates on (all) the blocks in the clipboard, not the blocks in the map, and therefore does not require marker blocks.\r\n"
          });
          this.itemData.Add("Copy", new HowToMenuScreen.HowToData()
          {
            UnlockID = 3,
            Text = "Copy allows you to create a clipboard of blocks, which can then be pasted back into the map or saved as a component.\r\n\r\nDefine the region to Copy using two or more marker blocks. You do not have to define the exact minimum region. The game will only select extents that actually contain blocks.\r\n\r\nWhen you select Copy, you will now be holding a clipboard in your left hand, and you will see a ghost image of the copied blocks in front of you, with a yellow frame showing the clipboard extents.\r\n\r\nPress the left trigger to Paste the contents of the clipboard back into the map at a different location.\r\n\r\nYou can rotate the clipboard by holding DPad Up and moving the right stick left or right.\r\n\r\nThere is no limit to how many times you can paste a clipboard.\r\n\r\nSelect Save Component from the Creative Menu to save the clipboard as a component. All components are stored in Component Packs. When you save a component, you must first select the Component Pack the component is to be saved in, or create a new Component Pack. Then either select an existing component to overwrite it, or select a new component and enter the components name.\r\n\r\nComponents can be loaded later and pasted into the map, or loaded and pasted into a different map. \r\n\r\nComponent Packs can be shared with your friends over Xbox Live. Component Packs are the main mechanism for organizing and grouping large numbers of components.\r\n"
          });
          this.itemData.Add("Flood", new HowToMenuScreen.HowToData()
          {
            UnlockID = 4,
            Text = "Flood allows you to fill an irregular shaped region with a selected block. \r\n\r\nPlace one marker block at the top edge of the region. Flooding is controlled by gravity so the marker block must be at the top. A flood will never go higher than the marker block.\r\n\r\nSelect Flood and the select the block to flood with.\r\n\r\nYou can place multiple marker blocks to start multiple floods at the same time. An individual flood operation will start for each marker block.\r\n\r\nFloods have a maximum radius. This is to protect players from an accidental flood destroying their map.\r\n\r\nWhile a flood is in progress there is an 'Abort Active Floods' option on the Creative Tools Menu. Use this to abort all active floods.\r\n"
          });
          break;
      }
    }

    private void VideoeSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, this.videoList, new ListBoxScreen.OnMenuItemSelected(this.OnVideoSelected), false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private bool OnVideoSelected(MenuEntry videoName)
    {
      if (videoName != null)
        this.ScreenManager.AddScreen((GameScreen) new VideoPlayerScreen(this.player, "Video\\test"), this.ControllingPlayer);
      return true;
    }

    private void ItemSelected(object sender, PlayerIndexEventArgs e)
    {
      BlockMenuEntry blockMenuEntry = sender as BlockMenuEntry;
      if (blockMenuEntry == null)
        return;
      HowToMenuScreen.HowToData tag = (HowToMenuScreen.HowToData) blockMenuEntry.Tag;
      if (tag.Index != HowToIndex.None && tag.Index != this.index)
        this.ScreenManager.AddScreen((GameScreen) new HowToMenuScreen(this.instance, this.player, tag.Index), this.ControllingPlayer);
      else
        this.ShowText(blockMenuEntry.Text, tag.UnlockID);
    }

    private void ShowText(string key, int unlockID)
    {
      this.ScreenManager.AddScreen((GameScreen) new HowToScreen(this.player, key, this.itemData[key].Text)
      {
        HowToID = (((int) this.index << 16) + unlockID)
      }, this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    private struct HowToData
    {
      public string Text;
      public HowToIndex Index;
      public int UnlockID;
    }
  }
}
