// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Globals2
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
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace StudioForge.TotalMiner
{
  internal static class Globals2
  {
    public static Globals2.ComPackData[] PublicSystemComPackNames = new Globals2.ComPackData[2]
    {
      new Globals2.ComPackData("System Avatars", 9),
      new Globals2.ComPackData("System", 1)
    };
    public static bool GuiHelpVisible = true;
    public static float DefaultViewDistance = 1f;
    public static float DefaultTextureSmoothing = 0.2f;
    public static GameSettings GameSettings = new GameSettings();
    public static bool ShowOres = false;
    public static bool MultiSampling = true;
    public static List<Script> SystemScripts = new List<Script>();
    public static List<Script> GlobalScripts = new List<Script>();
    public static List<string> KickedBy = new List<string>();
    public static List<string> BannerList = new List<string>();
    public static GlobalDataContents Contents = new GlobalDataContents()
    {
      FirstTimePlayed = false,
      TrialMapDirNum = 0
    };
    public static int Defence = 100;
    public static int DeviceVirtualization = 2;
    private static string[] IntStrings = new string[101]
    {
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11",
      "12",
      "13",
      "14",
      "15",
      "16",
      "17",
      "18",
      "19",
      "20",
      "21",
      "22",
      "23",
      "24",
      "25",
      "26",
      "27",
      "28",
      "29",
      "30",
      "31",
      "32",
      "33",
      "34",
      "35",
      "36",
      "37",
      "38",
      "39",
      "40",
      "41",
      "42",
      "43",
      "44",
      "45",
      "46",
      "47",
      "48",
      "49",
      "50",
      "51",
      "52",
      "53",
      "54",
      "55",
      "56",
      "57",
      "58",
      "59",
      "60",
      "61",
      "62",
      "63",
      "64",
      "65",
      "66",
      "67",
      "68",
      "69",
      "70",
      "71",
      "72",
      "73",
      "74",
      "75",
      "76",
      "77",
      "78",
      "79",
      "80",
      "81",
      "82",
      "83",
      "84",
      "85",
      "86",
      "87",
      "88",
      "89",
      "90",
      "91",
      "92",
      "93",
      "94",
      "95",
      "96",
      "97",
      "98",
      "99",
      "100"
    };
    private static object fileAccessLock = new object();
    private static bool[] photoIDsUsed = new bool[256];
    public const int InvalidY = 0;
    public const int MaxClanBannerID = 74;
    public const int ZeusClanBannerID = 80;
    public const string NewWorldText = "New World";
    public const string NewAvatarText = "New Avatar";
    public const string NewComponentText = "New Component";
    public const string NewComponentPackText = "New Component Pack";
    public const string NewQuestText = "Create New Quest";
    public const string SystemWorldsText = "System Worlds";
    public const string GamertagDBName = "GamertagData";
    public const string HighScoreDBName = "HighScores.db";
    private const string coordLabel = "[coord]";
    public static int AutoStartMap;
    public static int LastMapPlayed;
    public static int MaxConcurrentPlayers;
    public static int MapSeed;
    public static GameProperties GameProperties;
    public static GamertagDataManager GamertagData;
    public static Dictionary<string, bool> StreamedSounds;
    public static ParticleData[] SystemParticleData;
    public static List<ParticleData> CustomParticleData;
    public static bool NeedToReinitialize;
    public static int HighscoreUpdateTimestamp;
    public static bool HighscoreDataChanged;
    public static int ServerTimestamp;
    public static List<InputProfile> InputProfiles;
    public static string ExternalScriptEditor;
    public static bool UseOldMenu;
    public static Gamer LocalGamer;
    public static SteamManager SteamManager;
    private static bool initializedContentData;
    private static bool globalDataInitialized;
    public static int MaxRareLevel;

    public static bool IsValidGameHeader
    {
      get
      {
        if (Globals2.GameProperties != null && Globals2.GameProperties.SaveGame != null)
          return Globals2.GameProperties.SaveGame.Header != null;
        return false;
      }
    }

    public static string ComponentPath(int dirnum)
    {
      return Globals2.ComponentPath((string) null, dirnum);
    }

    public static string ComponentPath(string rootDir, int dirnum)
    {
      if (dirnum > 1000000)
        return ModManager.GetPath(dirnum - 1000000) + (object) '\\';
      string str = "000000" + dirnum.ToString();
      return rootDir + "Com\\" + str.Substring(str.Length - 6, 6) + (object) '\\';
    }

    public static Permissions DefaultPermission
    {
      get
      {
        if (Globals2.GameProperties == null || Globals2.GameProperties.SaveGame == null || Globals2.GameProperties.SaveGame.Header == null)
          return Permissions.None;
        return Globals2.GameProperties.SaveGame.Header.DefaultPermission;
      }
    }

    public static float GetFarClip(GameInstance instance)
    {
      return Globals2.GameSettings.ViewDistance * instance.MaxFarClip;
    }

    public static bool UpdateBannerList(string[] bannerList)
    {
      lock (Globals2.BannerList)
      {
        int count = Globals2.BannerList.Count;
        Globals2.BannerList.Clear();
        if (bannerList != null)
          Globals2.BannerList.AddRange((IEnumerable<string>) bannerList);
        return count != 0 || Globals2.BannerList.Count != 0;
      }
    }

    public static Gamer GetSignedInGamer(PlayerIndex? playerIndex)
    {
      if (!playerIndex.HasValue)
        return (Gamer) null;
      return Globals2.GetSignedInGamer(playerIndex.Value);
    }

    public static Gamer GetSignedInGamer(PlayerIndex playerIndex)
    {
      return Globals2.LocalGamer;
    }

    public static Gamer GetSignedInGamer(string gamertag)
    {
      return Globals2.LocalGamer;
    }

    public static void Initialize()
    {
      if (Globals2.globalDataInitialized)
        return;
      StreamReader reader = TextFileParser.GetReader("game.ini");
      string str = TextFileParser.ReadString(reader, "SaveRoot", "");
      reader.Close();
      if (str.Length < 1)
        str = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\TotalMiner";
      while (str.EndsWith("\\"))
        str = str.Substring(0, str.Length - 1);
      FileSystem.RootPath = str;
      FileSystem.CreateDir((string) null);
      Globals1.Initialize();
      Globals2.Reinitialize(true);
      Globals2.SystemParticleData = Utils.Deserialize1<ParticleData[]>("Content\\Map\\ParticleEmitterData.xml");
      Globals2.GamertagData = new GamertagDataManager();
      MenuScreen.DefaultMenuMoveSound = Globals1.ItemSoundGroups[10].Sounds.Use[0];
      MenuScreen.DefaultMenuCancelSound = Globals1.ItemSoundGroups[8].Sounds.Use[0];
      MenuScreen.DefaultMenuSelectSound = Globals1.ItemSoundGroups[11].Sounds.Use[0];
      MenuScreen.DefaultMenuInvalidOperationSound = Globals1.ItemSoundGroups[9].Sounds.Use[0];
      Globals2.LoadGlobalData();
      Globals2.globalDataInitialized = true;
    }

    public static void Reinitialize()
    {
      Globals2.Reinitialize(false);
    }

    public static void Reinitialize(bool force)
    {
      Globals1.RemoveNonGlobalBehaviourTrees();
      if (!force && !Globals2.NeedToReinitialize)
        return;
      Globals1.Reinitialize();
      Globals2.CustomParticleData = Globals2.LoadParticleTemplates();
      ModManager.Initialize();
      Globals2.NeedToReinitialize = false;
    }

    public static void ResetGameInstance()
    {
      if (Globals2.GameProperties == null)
        return;
      Globals2.GameProperties.CleanupInstanceStatics();
    }

    public static void InitializeForTest()
    {
      string path = "D:\\Documents\\Craig XNA\\TotalMiner\\TotalMiner\\TotalMiner\\bin\\win\\Debug\\";
      Globals2.NeedToReinitialize = false;
      Globals1.Reinitialize(path);
      Globals2.SystemParticleData = Utils.Deserialize1<ParticleData[]>(path + "Content\\Map\\ParticleEmitterData.xml");
      Globals2.GamertagData = new GamertagDataManager();
      Blueprints.InitializeBlueprints((GameInstance) null);
    }

    private static void DumpItemData()
    {
      List<BlockDataXML> blockDataXmlList = new List<BlockDataXML>((IEnumerable<BlockDataXML>) Globals1.BlockData);
      blockDataXmlList.Sort(new Comparison<BlockDataXML>(Globals2.SortBlockXP));
      for (int index = 0; index < blockDataXmlList.Count; ++index)
      {
        BlockDataXML blockDataXml = blockDataXmlList[index];
        if (Globals1.ItemData[(int) blockDataXml.Name].IsEnabled)
        {
          double blockMineXp = (double) Globals2.GetBlockMineXP(blockDataXml.Name);
          double blockPowerTestBase = (double) ItemData2.GetStrikeBlockPowerTestBase(blockDataXml.Name);
          SkillDataXML skillDataXml = Globals1.SkillData[(int) blockDataXml.Name];
        }
      }
    }

    private static int SortBlockXP(BlockDataXML b1, BlockDataXML b2)
    {
      float num = Globals2.GetBlockMineXP(b1.Name) * ItemData2.GetStrikeBlockPowerTestBase(b1.Name);
      return (Globals2.GetBlockMineXP(b2.Name) * ItemData2.GetStrikeBlockPowerTestBase(b2.Name)).CompareTo(num);
    }

    private static float GetBlockMineXP(Block b)
    {
      return Globals1.SkillData[(int) b].MineExp;
    }

    private static void ParseSoundItemXMLFiles()
    {
      int index = 0;
      ItemSoundDataXML itemSoundDataXml = new ItemSoundDataXML();
      Globals2.SoundElem elem = Globals2.SoundElem.None;
      List<string> sounds = new List<string>();
      using (FileStream fileStream = new FileStream("Content\\Map\\ItemSoundData.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream))
        {
          while (!streamReader.EndOfStream)
          {
            string str1 = streamReader.ReadLine();
            if (str1.IndexOf("<ItemSoundDataXML>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              elem = Globals2.SoundElem.None;
              itemSoundDataXml = Globals1.ItemSoundData[index];
            }
            else if (str1.IndexOf("</ItemSoundDataXML>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              Globals1.ItemSoundData[index] = itemSoundDataXml;
              ++index;
            }
            else if (str1.IndexOf("</Sounds>", StringComparison.OrdinalIgnoreCase) > 0)
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
            else if (str1.IndexOf("<Step>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Step;
            }
            else if (str1.IndexOf("<Mine>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Mine;
            }
            else if (str1.IndexOf("<Dig>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Dig;
            }
            else if (str1.IndexOf("<Chop>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Chop;
            }
            else if (str1.IndexOf("<Use>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Use;
            }
            else if (str1.IndexOf("<UseFail>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.UseFail;
            }
            else if (str1.IndexOf("<Hit>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundDataXml.Sounds = Globals2.SetLastSoundElement(itemSoundDataXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Hit;
            }
            else if (str1.IndexOf("<string>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              int num1 = str1.IndexOf("<string>", StringComparison.OrdinalIgnoreCase);
              int num2 = str1.IndexOf("</string>", StringComparison.OrdinalIgnoreCase);
              string str2 = str1.Substring(num1 + 8, num2 - (num1 + 8));
              sounds.Add(str2);
            }
          }
        }
      }
    }

    private static void ParseSoundGroupXMLFiles()
    {
      int index = 0;
      ItemSoundGroupXML itemSoundGroupXml = new ItemSoundGroupXML();
      Globals2.SoundElem elem = Globals2.SoundElem.None;
      List<string> sounds = new List<string>();
      using (FileStream fileStream = new FileStream("Content\\Map\\ItemSoundGroups.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream))
        {
          while (!streamReader.EndOfStream)
          {
            string str1 = streamReader.ReadLine();
            if (str1.IndexOf("<ItemSoundGroupXML>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              elem = Globals2.SoundElem.None;
              itemSoundGroupXml = Globals1.ItemSoundGroups[index];
            }
            else if (str1.IndexOf("</ItemSoundGroupXML>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              Globals1.ItemSoundGroups[index] = itemSoundGroupXml;
              ++index;
            }
            else if (str1.IndexOf("</Sounds>", StringComparison.OrdinalIgnoreCase) > 0)
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
            else if (str1.IndexOf("<Step>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Step;
            }
            else if (str1.IndexOf("<Mine>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Mine;
            }
            else if (str1.IndexOf("<Dig>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Dig;
            }
            else if (str1.IndexOf("<Chop>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Chop;
            }
            else if (str1.IndexOf("<Use>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Use;
            }
            else if (str1.IndexOf("<UseFail>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.UseFail;
            }
            else if (str1.IndexOf("<Hit>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              itemSoundGroupXml.Sounds = Globals2.SetLastSoundElement(itemSoundGroupXml.Sounds, elem, sounds);
              elem = Globals2.SoundElem.Hit;
            }
            else if (str1.IndexOf("<string>", StringComparison.OrdinalIgnoreCase) > 0)
            {
              int num1 = str1.IndexOf("<string>", StringComparison.OrdinalIgnoreCase);
              int num2 = str1.IndexOf("</string>", StringComparison.OrdinalIgnoreCase);
              string str2 = str1.Substring(num1 + 8, num2 - (num1 + 8));
              sounds.Add(str2);
            }
          }
        }
      }
    }

    private static ItemSoundXML SetLastSoundElement(
      ItemSoundXML xml,
      Globals2.SoundElem elem,
      List<string> sounds)
    {
      if (sounds.Count > 0)
      {
        switch (elem)
        {
          case Globals2.SoundElem.Step:
            xml.Step = sounds.ToArray();
            break;
          case Globals2.SoundElem.Mine:
            xml.Mine = sounds.ToArray();
            break;
          case Globals2.SoundElem.Dig:
            xml.Dig = sounds.ToArray();
            break;
          case Globals2.SoundElem.Chop:
            xml.Chop = sounds.ToArray();
            break;
          case Globals2.SoundElem.Use:
            xml.Use = sounds.ToArray();
            break;
          case Globals2.SoundElem.UseFail:
            xml.UseFail = sounds.ToArray();
            break;
          case Globals2.SoundElem.Hit:
            xml.Hit = sounds.ToArray();
            break;
        }
      }
      sounds.Clear();
      return xml;
    }

    private static void BuildSoundData()
    {
      string prefix1 = "Groups\\";
      string path1 = "Content\\Audio\\Effects\\" + prefix1;
      string[] files1 = TitleFileSystem.GetFiles(path1, "*.xnb");
      for (int index = 0; index < files1.Length; ++index)
        files1[index] = files1[index].Substring(path1.Length);
      for (int index = 0; index < Globals1.ItemSoundGroups.Length; ++index)
      {
        ItemSoundGroupXML itemSoundGroup = Globals1.ItemSoundGroups[index];
        itemSoundGroup.Sounds = Globals2.GetSounds(files1, itemSoundGroup.Group.ToString(), prefix1);
        Globals1.ItemSoundGroups[index] = itemSoundGroup;
      }
      ItemSoundXML sounds = Globals2.GetSounds(files1, "ItemDoor", prefix1);
      Globals1.ItemSoundGroups[23].Sounds.Use = sounds.Use;
      Globals1.ItemSoundGroups[24].Sounds.Use = sounds.Use;
      string prefix2 = "Items\\";
      string path2 = "Content\\Audio\\Effects\\" + prefix2;
      string[] files2 = TitleFileSystem.GetFiles(path2, "*.xnb");
      for (int index = 0; index < files2.Length; ++index)
        files2[index] = files2[index].Substring(path2.Length);
      for (int index = 0; index < Globals1.ItemSoundData.Length; ++index)
      {
        ItemSoundDataXML itemSoundDataXml = Globals1.ItemSoundData[index];
        itemSoundDataXml.Sounds = Globals2.GetSounds(files2, Globals1.ItemData[(int) itemSoundDataXml.ItemID].IDString, prefix2);
        Globals1.ItemSoundData[index] = itemSoundDataXml;
      }
    }

    private static void DebugSerialize()
    {
      Globals1.ItemSoundGroups = new ItemSoundGroupXML[62];
      for (int index = 0; index < Globals1.ItemSoundGroups.Length; ++index)
      {
        ItemSoundGroupXML itemSoundGroupXml = new ItemSoundGroupXML()
        {
          Group = (ItemSoundGroup) index
        };
        switch (itemSoundGroupXml.Group)
        {
          case ItemSoundGroup.ItemWoodDoor:
            itemSoundGroupXml.Parent = ItemSoundGroup.ItemWood;
            break;
          case ItemSoundGroup.ItemMetalDoor:
            itemSoundGroupXml.Parent = ItemSoundGroup.ItemMetal;
            break;
          default:
            itemSoundGroupXml.Parent = ItemSoundGroup.Base;
            break;
        }
        Globals1.ItemSoundGroups[index] = itemSoundGroupXml;
      }
      Globals1.ItemSoundGroups[1].Parent = ItemSoundGroup.None;
      Globals1.ItemSoundData = new ItemSoundDataXML[Globals1.ItemData.Length];
      for (int index = 0; index < Globals1.ItemSoundData.Length; ++index)
      {
        ItemSoundDataXML itemSoundDataXml = new ItemSoundDataXML();
        itemSoundDataXml.ItemID = (Item) index;
        itemSoundDataXml.Group = ItemSoundGroup.None;
        switch (itemSoundDataXml.ItemID)
        {
          case Item.None:
            itemSoundDataXml.Group = ItemSoundGroup.None;
            break;
          case Item.Grass:
          case Item.GrassShaded:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGrass;
            break;
          case Item.Dirt:
          case Item.Sand:
          case Item.TilledEarth:
            itemSoundDataXml.Group = ItemSoundGroup.ItemEarth;
            break;
          case Item.Scoria:
          case Item.Cactus:
          case Item.SaltBlock:
          case Item.Bone:
            itemSoundDataXml.Group = ItemSoundGroup.ItemPorous;
            break;
          case Item.Wood:
            itemSoundDataXml.Group = ItemSoundGroup.ItemTree;
            break;
          case Item.WoodPlank:
          case Item.WoodVeneer:
          case Item.Bookcase:
          case Item.Ladder:
          case Item.Workbench:
          case Item.Chest:
          case Item.ItemShop:
          case Item.BlockShop:
          case Item.Stairs:
          case Item.Sign:
          case Item.Crate:
          case Item.Fence:
          case Item.HalfBlock:
          case Item.Table:
          case Item.GildedWoodPanel:
          case Item.Painting:
          case Item.Stairs2:
          case Item.HalfBlock2:
          case Item.SignIcon:
          case Item.FenceIcon:
          case Item.StairsIcon:
          case Item.HalfBlockIcon:
          case Item.Stairs2Icon:
          case Item.HalfBlock2Icon:
            itemSoundDataXml.Group = ItemSoundGroup.ItemWood;
            break;
          case Item.Leaves:
          case Item.Sapling:
          case Item.WhiteFlowers:
          case Item.PurpleFlowers:
          case Item.RedFlowers:
          case Item.YellowFlowers:
          case Item.PineLeaves:
          case Item.WovenLeaves:
          case Item.LongGrass:
          case Item.ClimbingIvy:
          case Item.RedMushroom:
          case Item.MapleLeaves:
          case Item.BerryBush:
            itemSoundDataXml.Group = ItemSoundGroup.ItemFlora;
            break;
          case Item.Glass:
          case Item.ArcadeMachine:
          case Item.SunBox:
          case Item.OneWayGlass:
          case Item.PoweredLight:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGlass;
            break;
          case Item.Cloud:
          case Item.Turkish:
          case Item.Camouflage:
          case Item.WhiteWool:
          case Item.BedHead:
          case Item.BedFoot:
          case Item.Bed:
            itemSoundDataXml.Group = ItemSoundGroup.ItemWool;
            break;
          case Item.Water:
            itemSoundDataXml.Group = ItemSoundGroup.ItemWater;
            break;
          case Item.Lava:
            itemSoundDataXml.Group = ItemSoundGroup.ItemLava;
            break;
          case Item.Clay:
          case Item.Sandstone:
          case Item.Limestone:
          case Item.Basalt:
          case Item.Andesite:
          case Item.Dacite:
          case Item.Diorite:
          case Item.Tuff:
          case Item.Serpentine:
          case Item.Gabbro:
          case Item.Granite:
          case Item.Komatiite:
          case Item.Marble:
          case Item.Rhyolite:
          case Item.Bricks:
          case Item.GrassyStone:
          case Item.ConcreteBrick:
          case Item.TerracottaTile:
            itemSoundDataXml.Group = ItemSoundGroup.ItemRock;
            break;
          case Item.Bedrock:
          case Item.Cobblestone:
          case Item.Chess:
          case Item.Furnace:
          case Item.Teflon:
          case Item.MossyCobblestone:
          case Item.BlueBox:
          case Item.Rasta:
          case Item.WhiteTile:
          case Item.Checkered:
          case Item.Retro:
          case Item.LitFurnace:
          case Item.StoneWall:
          case Item.SandBrick:
            itemSoundDataXml.Group = ItemSoundGroup.ItemTile;
            break;
          case Item.Carbon:
          case Item.Coal:
          case Item.Flint:
          case Item.Gold:
          case Item.Platinum:
          case Item.Iron:
          case Item.Uranium:
          case Item.TNT:
          case Item.C4:
          case Item.Sulphur:
          case Item.Cyclonite:
          case Item.Titanium:
            itemSoundDataXml.Group = ItemSoundGroup.ItemOre;
            break;
          case Item.Obsidian:
          case Item.Greenstone:
          case Item.Diamond:
          case Item.Ruby:
          case Item.Opal:
          case Item.Fluorite:
          case Item.Sapphire:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGem;
            break;
          case Item.Torch:
          case Item.Teleport:
          case Item.Wisdom:
          case Item.Blueprint:
          case Item.Static:
          case Item.Ice:
          case Item.SpiderEgg:
          case Item.AmbientSoundBlock:
          case Item.Book:
          case Item.Key:
          case Item.InvisibleBarrier:
          case Item.Stack:
          case Item.UpsideDownStack:
          case Item.NPCSpawn:
          case Item.Ramp:
          case Item.Cylinder:
          case Item.WifiTransmitter:
          case Item.WifiReceiver:
          case Item.PressurePlate:
          case Item.Sundial:
          case Item.ScriptBlock:
          case Item.MobSpawn:
          case Item.Stack2:
          case Item.ColorLightOrange:
          case Item.ColorLightYellow:
          case Item.ColorLightBlue:
          case Item.ColorDarkBlue:
          case Item.ColorLightGreen:
          case Item.ColorDarkBrown:
          case Item.ColorTan:
          case Item.ColorCreme:
          case Item.ColorDarkRed:
          case Item.ColorDarkGray:
          case Item.ColorGray:
          case Item.ColorDarkGreen:
          case Item.ColorPink:
          case Item.ColorPurple:
          case Item.ColorCyan:
          case Item.ColorBlack:
          case Item.ColorWhite:
          case Item.ColorYellow:
          case Item.ColorRed:
          case Item.ColorOrange:
          case Item.ColorLightBrown:
          case Item.ColorBrown:
          case Item.ColorGreen:
          case Item.ColorBlue:
          case Item.Marker:
          case Item.ExcludeMarker:
            itemSoundDataXml.Group = ItemSoundGroup.ItemBlock;
            break;
          case Item.WoodDoorTop:
          case Item.WoodDoorBottom:
          case Item.TrapDoor:
          case Item.WoodDoor:
            itemSoundDataXml.Group = ItemSoundGroup.ItemWoodDoor;
            break;
          case Item.SteelDoorTop:
          case Item.SteelDoorBottom:
          case Item.LockedDoorTop:
          case Item.LockedDoorBottom:
          case Item.SteelDoor:
          case Item.LockedDoor:
            itemSoundDataXml.Group = ItemSoundGroup.ItemMetalDoor;
            break;
          case Item.Rope:
          case Item.RopeIcon:
            itemSoundDataXml.Group = ItemSoundGroup.ItemRope;
            break;
          case Item.MultiTextureBlock:
          case Item.CherryMetal:
          case Item.SteelSpikes:
          case Item.RaresChest:
          case Item.LockedChest:
          case Item.SentryTurret:
          case Item.ProximityDetector:
          case Item.Scaffold:
          case Item.SteelPortcullis:
          case Item.SteelPlating:
          case Item.GoldBlock:
          case Item.Safe:
          case Item.MultiTextureBlock2:
            itemSoundDataXml.Group = ItemSoundGroup.ItemMetal;
            break;
          case Item.Fire:
            itemSoundDataXml.Group = ItemSoundGroup.ItemFire;
            break;
          case Item.Crop:
          case Item.WheatSeed:
          case Item.SugarCaneSeed:
          case Item.Wheat:
          case Item.Sugar:
          case Item.Flour:
          case Item.Salt:
          case Item.Tomato:
          case Item.TomatoSeed:
          case Item.BoneMeal:
          case Item.Potato:
          case Item.Corn:
            itemSoundDataXml.Group = ItemSoundGroup.ItemCrop;
            break;
          case Item.Snow:
          case Item.SnowLayer:
            itemSoundDataXml.Group = ItemSoundGroup.ItemSnow;
            break;
          case Item.Switch:
          case Item.Button:
          case Item.SwitchIcon:
          case Item.ButtonIcon:
            itemSoundDataXml.Group = ItemSoundGroup.ItemActivate;
            break;
          case Item.Hand:
          case Item.WoodPickaxe:
          case Item.WoodHatchet:
          case Item.WoodShovel:
          case Item.WoodSpear:
          case Item.WoodSword:
          case Item.Stick:
          case Item.FlintArrow:
            itemSoundDataXml.Group = ItemSoundGroup.ItemWoodTool;
            break;
          case Item.IronPickaxe:
          case Item.IronHatchet:
          case Item.IronShovel:
          case Item.IronSpear:
          case Item.IronSword:
          case Item.IronArrow:
          case Item.IronHoe:
          case Item.IronScythe:
          case Item.IronBattleAxe:
            itemSoundDataXml.Group = ItemSoundGroup.ItemIronTool;
            break;
          case Item.SteelPickaxe:
          case Item.SteelHatchet:
          case Item.SteelShovel:
          case Item.SteelSpear:
          case Item.SteelSword:
          case Item.SteelArrow:
          case Item.SteelHoe:
          case Item.SteelScythe:
          case Item.SteelBattleAxe:
          case Item.SteelScimitar:
          case Item.SteelPike:
          case Item.SteelClaymore:
          case Item.SteelKatana:
            itemSoundDataXml.Group = ItemSoundGroup.ItemSteelTool;
            break;
          case Item.DiamondPickaxe:
          case Item.DiamondHatchet:
          case Item.DiamondShovel:
          case Item.DiamondSpear:
          case Item.DiamondSword:
          case Item.SledgeHammer:
          case Item.DiamondArrow:
          case Item.DiamondHoe:
          case Item.DiamondScythe:
          case Item.DiamondBattleAxe:
            itemSoundDataXml.Group = ItemSoundGroup.ItemDiamondTool;
            break;
          case Item.RubyPickaxe:
          case Item.RubyArrow:
          case Item.RubyBattleAxe:
          case Item.RubySword:
            itemSoundDataXml.Group = ItemSoundGroup.ItemRubyTool;
            break;
          case Item.TitaniumPickaxe:
          case Item.TitaniumArrow:
          case Item.BattleAxe:
          case Item.TitaniumBattleAxe:
          case Item.TitaniumSword:
          case Item.TitaniumKatana:
            itemSoundDataXml.Group = ItemSoundGroup.ItemTitaniumTool;
            break;
          case Item.RawFish:
          case Item.RawLambChops:
          case Item.RawDuckMeat:
          case Item.RawBeef:
            itemSoundDataXml.Group = ItemSoundGroup.ItemRaw;
            break;
          case Item.CookedFish:
          case Item.CookedLambChops:
          case Item.CookedDuckMeat:
          case Item.Bread:
          case Item.Cake:
          case Item.Pizza:
          case Item.CookedBeef:
            itemSoundDataXml.Group = ItemSoundGroup.ItemCooked;
            break;
          case Item.WoodShield:
          case Item.IronShield:
          case Item.SteelShield:
          case Item.DiamondShield:
          case Item.GreenstoneGoldShield:
          case Item.DiamantiumShield:
          case Item.TitaniumShield:
            itemSoundDataXml.Group = ItemSoundGroup.ItemShield;
            break;
          case Item.WoodBow:
          case Item.ElvenBow:
          case Item.GoldenBow:
          case Item.SpiderBow:
          case Item.TrollBow:
          case Item.TitaniumBow:
            itemSoundDataXml.Group = ItemSoundGroup.ItemBow;
            break;
          case Item.SkeletonKey:
          case Item.SkullKey:
          case Item.SilkKey:
          case Item.DiamondKey:
          case Item.TitaniumKey:
          case Item.GoldKey:
          case Item.ShadowKey:
          case Item.DwarfKey:
          case Item.DarkKey:
          case Item.KeyOfLight:
          case Item.BobKey:
          case Item.StoneKey:
          case Item.GhostKey:
          case Item.TitanKey:
          case Item.HeroKey:
          case Item.LightningKey:
          case Item.LunaKey:
          case Item.SolarKey:
          case Item.CelestialKey:
          case Item.SlimeKey:
          case Item.NatureKey:
          case Item.UndeadKey:
          case Item.SpiderKey:
          case Item.FishKey:
          case Item.RubyKey:
          case Item.NauticalKey:
          case Item.TeleportKey:
            itemSoundDataXml.Group = ItemSoundGroup.ItemKey;
            break;
          case Item.AmuletOfFlight:
          case Item.WaterTalisman:
          case Item.ShieldBadge:
          case Item.TenLeagueBoots:
            itemSoundDataXml.Group = ItemSoundGroup.ItemRareTool;
            break;
          case Item.GreenstoneGoldBattleAxe:
          case Item.GreenstoneGoldPickaxe:
          case Item.GreenstoneGoldHatchet:
          case Item.GreenstoneGoldShovel:
          case Item.GreenstoneGoldSword:
          case Item.GreenstoneGoldSledgeHammer:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGreenstoneTool;
            break;
          case Item.CowHide:
          case Item.Leather:
          case Item.Coif:
          case Item.LeatherBody:
          case Item.LeatherLeggings:
          case Item.LeatherHelmet:
          case Item.LeatherBoots:
          case Item.LeatherGauntlets:
          case Item.TrollHide:
          case Item.TrollHideBody:
          case Item.TrollHideLeggings:
          case Item.TrollHideHelmet:
          case Item.TrollHideBoots:
          case Item.TrollHideGauntlets:
            itemSoundDataXml.Group = ItemSoundGroup.ItemLeather;
            break;
          case Item.GoldRing:
          case Item.GoldAmulet:
          case Item.GoldNecklace:
          case Item.RingOfBob:
          case Item.AmuletOfFury:
          case Item.NecklaceOfKnowledge:
          case Item.SpiderRing:
          case Item.PredatorAmulet:
          case Item.NecklaceOfHypocrisy:
          case Item.RingOfExemption:
          case Item.AmuletOfStarlight:
          case Item.NecklaceOfFarsight:
          case Item.RingOfIce:
          case Item.UnknownAmulet:
          case Item.UnknownNecklace:
            itemSoundDataXml.Group = ItemSoundGroup.ItemJewelry;
            break;
          case Item.DiamantiumSword:
            itemSoundDataXml.Group = ItemSoundGroup.ItemDiamantiumTool;
            break;
          case Item.IronBody:
          case Item.IronLeggings:
          case Item.IronHelmet:
          case Item.IronBoots:
          case Item.IronGauntlets:
          case Item.SteelBody:
          case Item.SteelLeggings:
          case Item.SteelHelmet:
          case Item.SteelBoots:
          case Item.SteelGauntlets:
          case Item.DiamantiumBody:
          case Item.DiamantiumLeggings:
          case Item.DiamantiumHelmet:
          case Item.DiamantiumBoots:
          case Item.DiamantiumGauntlets:
          case Item.TitaniumBody:
          case Item.TitaniumLeggings:
          case Item.TitaniumHelmet:
          case Item.TitaniumBoots:
          case Item.TitaniumGauntlets:
            itemSoundDataXml.Group = ItemSoundGroup.ItemArmor;
            break;
          case Item.SkillCombat:
          case Item.SkillHealth:
          case Item.SkillStrength:
          case Item.SkillAttack:
          case Item.SkillDefence:
          case Item.SkillRanged:
          case Item.SkillMining:
          case Item.SkillDigging:
          case Item.SkillChopping:
          case Item.SkillBuilding:
          case Item.SkillCrafting:
          case Item.SkillSmelting:
          case Item.SkillSmithing:
          case Item.SkillFarming:
          case Item.SkillCooking:
          case Item.SkillLooting:
            itemSoundDataXml.Group = ItemSoundGroup.ItemSkill;
            break;
          case Item.Revolver:
          case Item.EldarPistol:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGun;
            break;
          case Item.GoldenSMG:
          case Item.SpiderSMG:
          case Item.LaserBlaster:
          case Item.ComboAssaultRifle:
            itemSoundDataXml.Group = ItemSoundGroup.ItemGunSMG;
            break;
        }
        if (itemSoundDataXml.ItemID != Item.None && itemSoundDataXml.ItemID < Item.zLastBlockID && (itemSoundDataXml.Group == ItemSoundGroup.None && Globals1.ItemData[(int) itemSoundDataXml.ItemID].IsEnabled))
          itemSoundDataXml.Group = ItemSoundGroup.ItemBlock;
        Globals1.ItemSoundData[index] = itemSoundDataXml;
      }
    }

    private static ItemSoundXML GetSounds(
      string[] soundFiles,
      string key,
      string prefix)
    {
      ItemSoundXML itemSoundXml = new ItemSoundXML();
      itemSoundXml.Step = Globals2.GetSoundsCore(soundFiles, key + "Step", prefix);
      itemSoundXml.Mine = Globals2.GetSoundsCore(soundFiles, key + "Mine", prefix);
      itemSoundXml.Dig = Globals2.GetSoundsCore(soundFiles, key + "Dig", prefix);
      itemSoundXml.Chop = Globals2.GetSoundsCore(soundFiles, key + "Chop", prefix);
      itemSoundXml.Use = Globals2.GetSoundsCore(soundFiles, key + "Use", prefix);
      itemSoundXml.UseFail = Globals2.GetSoundsCore(soundFiles, key + "UseFail", prefix);
      itemSoundXml.Hit = Globals2.GetSoundsCore(soundFiles, key + "Hit", prefix);
      string[] soundsCore = Globals2.GetSoundsCore(soundFiles, key, prefix);
      if (soundsCore != null)
      {
        if (itemSoundXml.Step == null)
          itemSoundXml.Step = soundsCore;
        if (itemSoundXml.Mine == null)
          itemSoundXml.Mine = soundsCore;
        if (itemSoundXml.Dig == null)
          itemSoundXml.Dig = soundsCore;
        if (itemSoundXml.Chop == null)
          itemSoundXml.Chop = soundsCore;
        if (itemSoundXml.Use == null)
          itemSoundXml.Use = soundsCore;
        if (itemSoundXml.UseFail == null)
          itemSoundXml.UseFail = soundsCore;
        if (itemSoundXml.Hit == null)
          itemSoundXml.Hit = soundsCore;
      }
      return itemSoundXml;
    }

    private static string[] GetSoundsCore(string[] soundFiles, string key, string prefix)
    {
      List<string> stringList = new List<string>();
      if (soundFiles != null)
      {
        foreach (string soundFile in soundFiles)
        {
          if (soundFile.StartsWith(key))
          {
            string s = soundFile.Replace(key, "");
            int length = s.LastIndexOf('.');
            string str = soundFile;
            if (length >= 0)
            {
              str = soundFile.Substring(0, length + key.Length);
              s = s.Substring(0, length);
            }
            int result = 0;
            if (s == "" || int.TryParse(s, out result))
              stringList.Add(prefix + str);
          }
        }
      }
      if (stringList.Count <= 0)
        return (string[]) null;
      return stringList.ToArray();
    }

    public static void LoadGlobalData()
    {
      StreamReader reader = TextFileParser.GetReader("game.ini");
      Globals2.ExternalScriptEditor = TextFileParser.ReadString(reader, "Editor", "C:\\Windows\\System32\\notepad.exe");
      Globals2.UseOldMenu = TextFileParser.ReadBool(reader, "OldMenu", false);
      FileSystem.DeleteDir("Temp", true);
      try
      {
        if (!Globals2.initializedContentData)
        {
          Globals2.LoadContentsData();
          Globals2.initializedContentData = true;
        }
        Globals2.GamertagData.LoadGamertagData();
        Globals2.GamertagData.LoadHighScoreDataGlobal();
        if (Globals2.GamertagData.RepairGamertagData())
          Globals2.GamertagData.SaveGamertagDataNoLockNoFlushCore(false);
      }
      catch (Exception ex)
      {
      }
      Globals1.LoadBehaviourTrees();
      Globals2.SystemScripts = Globals2.LoadSystemScripts();
      Globals2.GlobalScripts = Globals2.LoadGlobalScripts();
      Globals2.InputProfiles = Globals2.LoadInputProfiles();
      InputManager1.Initialize(Globals2.GetInputProfile(TextFileParser.ReadString(reader, "InputProfile", "")));
      Thread.Sleep(500);
      FileSystem.CreateDir("Temp");
      reader.Close();
    }

    public static void LoadContentsData()
    {
      Globals2.LoadContentsDataCore();
      if (Globals2.Contents.TrialMapDirNum == 0)
        Globals2.CopySystemMapsToUserStorageCore();
      Globals2.LoadPhotoDat();
    }

    private static void LoadContentsDataCore()
    {
      if (!FileSystem.IsFileExist("GlobalData.xml"))
      {
        Globals2.WriteNewContentsFile();
      }
      else
      {
        Globals2.ReadContentsFile();
        Globals2.CheckForSystemMapUpdate();
      }
    }

    private static void CheckForSystemMapUpdate()
    {
      if (Globals2.Contents.Version >= 19000)
        return;
      foreach (string dir in FileSystem.GetDirs("SystemMaps\\"))
        FileSystem.DeleteDir(dir, true);
      Globals2.Contents.Version = 27302;
      Globals2.Contents.TrialMapDirNum = 0;
      Globals2.SaveGlobalDataContents();
    }

    private static void ReadContentsFile()
    {
      Globals2.Contents = FileSystem.Deserialize<GlobalDataContents>("GlobalData.xml");
      if (!FileSystem.IsFileExist("GlobalData.dat"))
        return;
      using (Stream input = FileSystem.OpenRead("GlobalData.dat"))
      {
        using (BinaryReader reader = new BinaryReader(input))
        {
          int version = reader.ReadInt32();
          Globals2.ReadGlobalData(reader, version);
          Globals2.ReadBannerList(reader, version);
        }
      }
    }

    private static void ReadGlobalData(BinaryReader reader, int version)
    {
      if (version <= 158)
        return;
      try
      {
        GraphicStatics.SetHUDPos(reader.ReadInt32(), reader.ReadInt32());
        if (version <= 257)
          return;
        Globals2.LastMapPlayed = reader.ReadInt32();
        if (version <= 290)
          return;
        Globals2.GuiHelpVisible = reader.ReadBoolean();
      }
      catch (Exception ex)
      {
      }
    }

    private static void ReadBannerList(BinaryReader reader, int version)
    {
      try
      {
        int capacity = reader.ReadInt32();
        Globals2.BannerList = new List<string>(capacity);
        for (int index = 0; index < capacity; ++index)
          Globals2.BannerList.Add(reader.ReadString());
        if (version >= 294)
          return;
        Globals2.BannerList.Clear();
      }
      catch (EndOfStreamException ex)
      {
        Globals2.BannerList.Clear();
      }
    }

    private static void WriteNewContentsFile()
    {
      Globals2.Contents.Version = 27302;
      Globals2.Contents.FirstTimePlayed = true;
      Globals2.Contents.TrialMapDirNum = 0;
      Globals2.BannerList = new List<string>();
      Globals2.SaveGlobalDataContents();
    }

    public static void SaveGlobalDataContents()
    {
      using (Stream file = FileSystem.CreateFile("GlobalData.xml"))
        new XmlSerializer(typeof (GlobalDataContents)).Serialize(file, (object) Globals2.Contents);
    }

    public static void SaveGlobalData()
    {
      Globals2.SaveGlobalDataCore();
    }

    private static void SaveGlobalDataCore()
    {
      try
      {
        Globals2.SaveGlobalDataNoLockNoFlush();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(65, ex);
      }
    }

    public static void SaveGlobalDataNoLockNoFlush()
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = FileSystem.OpenFile("GlobalData.dat", FileMode.Create))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            Globals2.WriteGlobalData(writer);
            Globals2.WriteBannerList(writer);
          }
        }
      }
    }

    private static void WriteGlobalData(BinaryWriter writer)
    {
      writer.Write(GraphicStatics.HUDPos().X);
      writer.Write(GraphicStatics.HUDPos().Y);
      writer.Write(Globals2.LastMapPlayed);
      writer.Write(Globals2.GuiHelpVisible);
    }

    private static void WriteBannerList(BinaryWriter writer)
    {
      if (Globals2.BannerList != null)
      {
        writer.Write(Globals2.BannerList.Count);
        foreach (string banner in Globals2.BannerList)
          writer.Write(banner);
      }
      else
        writer.Write(0);
    }

    public static void SaveAllDataThreaded(bool writeGlobalData, bool writeGamertagData)
    {
      new Thread(new ParameterizedThreadStart(Globals2.SaveAllDataThreadedCore)).Start((object) new Globals2.SaveAllState()
      {
        WriteGlobalData = writeGlobalData,
        WriteGamertagData = writeGamertagData
      });
    }

    private static void SaveAllDataThreadedCore(object o)
    {
      Globals2.SaveAllState saveAllState = (Globals2.SaveAllState) o;
      lock (Globals1.SaveSemaphore)
      {
        if (saveAllState.WriteGlobalData)
          Globals2.SaveGlobalDataNoLockNoFlush();
        if (!saveAllState.WriteGamertagData)
          return;
        Globals2.SaveGamertagData(false, true, false);
      }
    }

    public static void SaveGamertagData(bool saveHighScores, bool merge)
    {
      Globals2.GamertagData.SaveGamertagData(saveHighScores, merge);
    }

    public static void SaveGamertagDataThreaded(bool saveHighScores, bool merge)
    {
      Thread thread = new Thread(new ParameterizedThreadStart(Globals2.SaveGamertagDataThreadedCore));
      int num = 0;
      if (saveHighScores)
        num |= 1;
      if (merge)
        num |= 2;
      thread.Start((object) num);
    }

    public static void SaveGamertagDataThreadedCore(object o)
    {
      int num = (int) o;
      bool saveHighScores = (num & 1) == 1;
      bool merge = (num & 2) == 2;
      Globals2.GamertagData.SaveGamertagData(saveHighScores, merge);
    }

    public static void SaveGamertagData(bool useThreadQueue, bool saveHighScores, bool merge)
    {
      if (!useThreadQueue)
        Globals2.GamertagData.SaveGamertagData(saveHighScores, merge);
      else
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) new GamertagDataSaveWorker(saveHighScores, merge), false, PriorityLevel.Normal);
    }

    public static void SaveGamertagDataNoLockNoFlush()
    {
      Globals2.GamertagData.SaveGamertagDataNoLockNoFlushCore(true);
    }

    public static string ReadGamertag(BinaryReader reader)
    {
      return Globals2.ReadEncryptedString(reader);
    }

    public static void WriteGamertag(BinaryWriter writer, string gamertag)
    {
      Globals2.WriteEncryptedString(writer, gamertag);
    }

    public static string ReadEncryptedString(BinaryReader reader)
    {
      int key = (int) reader.ReadByte();
      int count = (int) reader.ReadByte();
      byte[] numArray = new byte[count];
      if (count > 0)
        reader.Read(numArray, 0, count);
      return Utils.UnencryptString(numArray, key) ?? "";
    }

    public static void WriteEncryptedString(BinaryWriter writer, string s)
    {
      int key = new PcgRandom(new Random().Next()).Next(44, 86);
      writer.Write((byte) key);
      if (s == null)
      {
        writer.Write((byte) 0);
      }
      else
      {
        writer.Write((byte) s.Length);
        writer.Write(Utils.EncryptString(s, key));
      }
    }

    public static void WriteFileWithHash(
      string filename,
      Stream data,
      BinaryWriter writer,
      int version)
    {
      byte[] messageHash = Encryption.GetMessageHash(data, (int) data.Length, version);
      byte num = DataScrambler.RandomScramble(messageHash);
      writer.Write(messageHash);
      writer.Write(num);
      writer.Write(messageHash.Length);
      Globals2.WriteFileNoHash(filename, writer);
    }

    public static void WriteFileNoHash(string filename, BinaryWriter writer)
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream dest = FileSystem.OpenWrite(filename))
          Utils.CopyStream(writer.BaseStream, dest, 0, 0, 200000);
      }
    }

    public static void EmptyDirectory(string path)
    {
      lock (Globals1.SaveSemaphore)
      {
        if (!FileSystem.IsDirExist(path))
          return;
        if (!path.EndsWith("\\"))
          path += "\\";
        foreach (string file in FileSystem.GetFiles(path, "*.*"))
          FileSystem.DeleteFile(file);
      }
    }

    public static string RenameFile(string filename, string newfilename)
    {
      lock (Globals1.SaveSemaphore)
      {
        if (filename == newfilename)
          return "both files have the same name";
        if (!FileSystem.IsFileExist(filename))
          return "file not found";
        int num = newfilename.LastIndexOf('\\');
        if (newfilename.Length - (num + 1) > 40)
          return "file name is too long";
        if (FileSystem.IsFileExist(newfilename))
          return "the same filename already exists";
        using (Stream src = FileSystem.OpenRead(filename))
        {
          using (Stream file = FileSystem.CreateFile(newfilename))
            Utils.CopyStream(src, file, 0, 0, 100000);
        }
        FileSystem.DeleteFile(filename);
      }
      return (string) null;
    }

    public static SaveGameFileInfo ParseGameFile(
      MapType mapType,
      string filename,
      bool calcFileSize)
    {
      SaveGameFileInfo info = new SaveGameFileInfo(mapType)
      {
        Filename = filename
      };
      lock (Globals1.SaveSemaphore)
        info.Header = MapLoader.LoadMapHeader(info.Filename);
      if (calcFileSize)
        info.FileSize = Globals2.CalcFileSize(info);
      return info;
    }

    public static SaveGameFileInfo ParseGameFile(
      MapType mapType,
      int dirnum,
      bool calcFileSize,
      bool isAutoSave)
    {
      SaveGameFileInfo info = new SaveGameFileInfo(mapType)
      {
        DirNumber = dirnum,
        IsAutoSave = isAutoSave
      };
      lock (Globals1.SaveSemaphore)
        info.Header = MapLoader.LoadMapHeader(info.Filename);
      if (calcFileSize)
        info.FileSize = Globals2.CalcFileSize(info);
      return info;
    }

    public static void ParseGameFile(SaveGameFileInfo info, bool calcFileSize)
    {
      lock (Globals1.SaveSemaphore)
        info.Header = MapLoader.LoadMapHeader(info.Filename);
      if (!calcFileSize)
        return;
      info.FileSize = Globals2.CalcFileSize(info);
    }

    public static string ParseComPack(int dirNum)
    {
      string str1 = Globals2.ComponentPath(dirNum);
      string str2 = (string) null;
      try
      {
        lock (Globals1.SaveSemaphore)
        {
          using (Stream input = FileSystem.OpenRead(str1 + "\\header.dat"))
          {
            using (BinaryReader binaryReader = new BinaryReader(input))
              str2 = binaryReader.ReadString();
          }
        }
      }
      catch (IOException ex)
      {
      }
      return str2;
    }

    public static int CalcFileSize(SaveGameFileInfo info)
    {
      return Globals2.CalcFileSize(info, (Action<int>) null);
    }

    public static int CalcFileSize(SaveGameFileInfo info, Action<int> onComplete)
    {
      int num = 0;
      try
      {
        if (info.Header.SaveVersion < 55)
        {
          lock (Globals1.SaveSemaphore)
          {
            using (Stream stream = FileSystem.OpenRead(info.Filename))
              num = (int) stream.Length;
          }
        }
        else
          num = Globals2.GetTotalBytes(FileSystem.GetFiles(info.MapFilePath, "*.*"));
      }
      catch (IOException ex)
      {
      }
      if (onComplete != null)
        onComplete(num);
      return num;
    }

    public static bool CheckHash(byte[] data, int version)
    {
      if (version > 92)
      {
        try
        {
          int count = 0;
          byte scrambleID = 0;
          using (MemoryStream memoryStream = new MemoryStream(data, data.Length - 5, 5))
          {
            using (BinaryReader binaryReader = new BinaryReader((Stream) memoryStream))
            {
              scrambleID = binaryReader.ReadByte();
              count = binaryReader.ReadInt32();
            }
          }
          if (count < 1 || count > 64)
            return false;
          byte[] numArray = new byte[count];
          using (MemoryStream memoryStream = new MemoryStream(data, data.Length - 5 - count, count))
          {
            using (BinaryReader binaryReader = new BinaryReader((Stream) memoryStream))
              binaryReader.Read(numArray, 0, count);
          }
          DataScrambler.Unscramble(numArray, scrambleID);
          byte[] messageHash = Encryption.GetMessageHash(data, data.Length - 5 - count, version);
          if (messageHash.Length != count)
            return false;
          for (int index = 0; index < count; ++index)
          {
            if ((int) numArray[index] != (int) messageHash[index])
              return false;
          }
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(91, ex);
          return false;
        }
      }
      return true;
    }

    public static void SteamAPIDebugTextHook(int severity, StringBuilder builder)
    {
      string msg = builder.ToString();
      if (severity == 0)
        CoreGlobals.LogInfoMessage("Steam Info", msg);
      else if (severity == 1)
        CoreGlobals.LogWarningMessage("Steam Warning", msg);
      else
        CoreGlobals.LogErrorMessage("Steam Error", msg);
    }

    public static int SortNamesWithFoldersAtTop(string s1, string s2)
    {
      if (s1.EndsWith("\\") && !s2.EndsWith("\\"))
        return "a".CompareTo("b");
      if (s2.EndsWith("\\") && !s1.EndsWith("\\"))
        return "b".CompareTo("a");
      return s1.CompareTo(s2);
    }

    public static string GetItemCountString(int count)
    {
      if (count < 0)
        return count.ToString();
      if (count <= 100)
        return Globals2.IntStrings[count];
      if (count < 1000)
        return count.ToString();
      if (count <= 1000000)
        return string.Format("{0:G3}K", (object) (float) ((double) count / 1000.0));
      if (count <= 1000000000)
        return string.Format("{0:G3}M", (object) (float) ((double) count / 1000000.0));
      return string.Format("{0:G3}B", (object) (float) ((double) count / 1000000000.0));
    }

    public static string IntToString(int i)
    {
      if (i < 0 || i > 100)
        return i.ToString();
      return Globals2.IntStrings[i];
    }

    public static string NumToString(float i)
    {
      if ((double) i > 100.0 || (double) i != (double) (int) i)
        return i.ToString();
      return Globals2.IntStrings[(int) i];
    }

    public static int BitPackedRating(float rating, int count)
    {
      int num = (int) (byte) ((double) rating * 10.0) << 24;
      count &= 16777215;
      return num | count;
    }

    public static void UnpackedRating(int bitpackedRating, out float rating, out int count)
    {
      count = bitpackedRating & 16777215;
      byte num = (byte) (((long) bitpackedRating & 4278190080L) >> 24);
      rating = (float) num / 10f;
    }

    public static string StripBadChars(string name)
    {
      return Globals2.StripBadChars(name, false);
    }

    public static string StripBadChars(string name, bool isFileName)
    {
      string name1 = name;
      int num = isFileName ? 1 : 0;
      char[] exceptions;
      if (!isFileName)
        exceptions = new char[25]
        {
          '\\',
          '<',
          '>',
          '!',
          '.',
          '?',
          '+',
          '*',
          '%',
          '|',
          '\'',
          '`',
          '~',
          '@',
          '#',
          '$',
          '^',
          '&',
          '(',
          ')',
          '"',
          '{',
          '}',
          ';',
          '/'
        };
      else
        exceptions = (char[]) null;
      return Globals2.StripBadChars(name1, num != 0, exceptions);
    }

    public static string StripBadChars(string name, bool isFileName, char[] exceptions)
    {
      string str = "";
      if (name != null && name.Length > 0)
      {
        foreach (char c in name)
        {
          if (Globals2.IsValidChar(c, isFileName, exceptions))
            str += (string) (object) c;
        }
      }
      return str;
    }

    public static string StripFolderName(string name)
    {
      string str1 = Globals2.StripBadChars(name, false, new char[1]
      {
        '\\'
      }).Trim().Trim('\\');
      for (string str2 = str1.Replace("\\\\", "\\"); str2 != str1; str2 = str2.Replace("\\\\", "\\"))
        str1 = str2;
      return str1;
    }

    public static string RemovePath(string path)
    {
      if (path.IsEmpty())
        return path;
      int num;
      for (num = path.LastIndexOf('\\'); num == path.Length - 1; num = path.LastIndexOf('\\'))
        path = path.Substring(0, path.Length - 1);
      return path.Substring(num + 1);
    }

    private static bool IsValidChar(char c, bool isFileName, char[] exceptions)
    {
      if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
        return true;
      if (exceptions != null)
      {
        for (int index = 0; index < exceptions.Length; ++index)
        {
          if ((int) c == (int) exceptions[index])
            return true;
        }
      }
      if (isFileName)
      {
        switch (c)
        {
          case ' ':
          case '_':
            return true;
          default:
            return false;
        }
      }
      else
      {
        switch (c)
        {
          case ' ':
          case ',':
          case '-':
          case ':':
          case '=':
          case '[':
          case ']':
          case '_':
            return true;
          default:
            return false;
        }
      }
    }

    public static bool HasPermission(Permissions permission, Permissions test)
    {
      return (permission & test) == test;
    }

    public static bool HasDefaultPermission(Permissions permission)
    {
      if (Globals2.GameProperties != null && Globals2.GameProperties.SaveGame != null && Globals2.GameProperties.SaveGame.Header != null)
        return Globals2.HasPermission(Globals2.GameProperties.SaveGame.Header.DefaultPermission, permission);
      return false;
    }

    public static void TogglDefaultPermission(Permissions permission)
    {
      permission &= ~(Permissions.Save | Permissions.Admin | Permissions.Grief);
      if (permission == Permissions.None || !NetworkManager.Instance.IsHost || (Globals2.GameProperties == null || Globals2.GameProperties.SaveGame == null) || Globals2.GameProperties.SaveGame.Header == null)
        return;
      if (Globals2.HasDefaultPermission(permission))
      {
        if ((permission & Permissions.Adventure) == Permissions.Adventure)
          permission |= Permissions.Edit;
        if ((permission & Permissions.Edit) == Permissions.Edit)
          permission |= Permissions.Creative;
        Globals2.GameProperties.SaveGame.Header.DefaultPermission &= ~permission;
      }
      else
      {
        if ((permission & Permissions.Creative) == Permissions.Creative)
          permission |= Permissions.Edit;
        if ((permission & Permissions.Edit) == Permissions.Edit)
          permission |= Permissions.Adventure;
        Globals2.GameProperties.SaveGame.Header.DefaultPermission |= permission;
      }
    }

    public static int BuildMapID(SaveMapHead header)
    {
      if (header.SaveVersion <= 209)
        return (int) (header.DateCreated << 9) + header.MapSeed;
      return (int) header.DateCreated + header.MapSeed;
    }

    public static string GetFilesizeAsString(int filesize)
    {
      if (filesize > 1000000)
        return string.Format("{0:N1} MB", (object) (float) ((double) filesize / 1000000.0));
      return string.Format("{0:N0} KB", (object) (float) ((double) filesize / 1000.0));
    }

    public static BoundingBox GetBox(GlobalPoint3D min, GlobalPoint3D max, float offset)
    {
      return new BoundingBox()
      {
        Min = {
          X = (float) min.X + offset,
          Y = (float) (min.Y - 1) + offset,
          Z = (float) min.Z + offset
        },
        Max = {
          X = (float) (max.X + 1) - offset,
          Y = (float) max.Y - offset,
          Z = (float) (max.Z + 1) - offset
        }
      };
    }

    private static int SortFileNames(string a, string b)
    {
      string nameFromSuffix1 = Globals2.ExtractNameFromSuffix(a);
      string nameFromSuffix2 = Globals2.ExtractNameFromSuffix(b);
      if (nameFromSuffix1 != nameFromSuffix2)
        return nameFromSuffix1.CompareTo(nameFromSuffix2);
      return Globals2.ExtractNumericSuffix(a).CompareTo(Globals2.ExtractNumericSuffix(b));
    }

    public static int ExtractNumericSuffix(string name)
    {
      int num = name.Length - 1;
      do
        ;
      while (num >= 0 && char.IsDigit(name[num--]));
      if (num == name.Length - 2)
        return 0;
      int result;
      if (!int.TryParse(name.Substring(num + 2, name.Length - (num + 2)), out result))
        return -1;
      return result;
    }

    public static string ExtractNameFromSuffix(string name)
    {
      int num = name.Length - 1;
      do
        ;
      while (num >= 0 && char.IsDigit(name[num--]));
      if (num == name.Length - 2)
        return name;
      return name.Substring(0, num + 2);
    }

    public static int ParseDirNumber(string path)
    {
      int num = path.EndsWith("_auto") ? 11 : 6;
      int result;
      if (int.TryParse(path.Substring(path.Length - num, 6), out result))
        return result;
      return -1;
    }

    public static string GetMapFilePath(MapType mapType, int dirnumber)
    {
      return Globals2.GetFilePath(Globals2.GetMapTypeDirName(mapType), dirnumber);
    }

    public static string GetMapFilePath(MapType mapType, int dirnumber, bool isAutoSave)
    {
      string str = Globals2.GetMapFilePath(mapType, dirnumber);
      if (isAutoSave)
        str = str.Substring(0, str.Length - 1) + "_auto\\";
      return str;
    }

    public static string GetComPackFilePath(int dirnumber)
    {
      return Globals2.GetFilePath("Com", dirnumber);
    }

    public static string GetPhotoFilePath(int dirnumber)
    {
      return Globals2.GetFilePath("Photo", dirnumber);
    }

    public static string GetFilePath(MapType mapType, SessionType type, int dirnumber)
    {
      switch (type)
      {
        case SessionType.SharePhoto:
          return Globals2.GetPhotoFilePath(dirnumber);
        case SessionType.ShareComPack:
          return Globals2.GetComPackFilePath(dirnumber);
        default:
          return Globals2.GetMapFilePath(mapType, dirnumber);
      }
    }

    public static string GetFilePath(string dirname, int dirnumber)
    {
      string str = "00000" + dirnumber.ToString();
      return dirname + "\\" + str.Substring(str.Length - 6, 6) + "\\";
    }

    public static string ExtractPath(string filename)
    {
      int num = filename.LastIndexOf("\\");
      return filename.Substring(0, num + 1);
    }

    public static string ExtractFileFromPath(string filename)
    {
      int num = filename.LastIndexOf("\\");
      return filename.Substring(num + 1);
    }

    public static string GetPhotoFilename(int photoID, PhotoFileType type)
    {
      return Globals2.GetPhotoFilePath(photoID) + type.ToString() + ".dat";
    }

    public static int GetNewMapDirNumber(MapType mapType)
    {
      return Globals2.GetNewDirNumber(Globals2.GetMapTypeDirName(mapType));
    }

    public static int GetNewComDirNumber()
    {
      return Globals2.GetNewDirNumber("Com");
    }

    public static string GetMapTypeDirName(MapType mapType)
    {
      if (mapType == MapType.Avatar)
        return "Avatars";
      return mapType != MapType.System ? "Maps" : "SystemMaps";
    }

    public static int GetNewDirNumber(SessionType type)
    {
      return Globals2.GetNewDirNumber(type, MapType.Map);
    }

    public static int GetNewDirNumber(SessionType type, MapType mapType)
    {
      switch (type)
      {
        case SessionType.SharePhoto:
          return Globals2.GetNewPhotoNumber();
        case SessionType.ShareComPack:
          return Globals2.GetNewComDirNumber();
        default:
          return Globals2.GetNewMapDirNumber(mapType);
      }
    }

    public static int GetNewDirNumber(string path)
    {
      List<string> stringList = new List<string>();
      lock (Globals1.SaveSemaphore)
      {
        string[] dirs = FileSystem.GetDirs(path + "\\");
        if (dirs != null)
          stringList.AddRange((IEnumerable<string>) dirs);
      }
      stringList.Remove(path + "\\Backup");
      stringList.Sort();
      int num = 1;
      for (int index = 0; index < stringList.Count; ++index)
      {
        if (!stringList[index].EndsWith("_auto"))
        {
          int dirNumber = Globals2.GetDirNumber(stringList[index]);
          if (dirNumber > 0)
          {
            if (dirNumber <= num)
              ++num;
            else
              break;
          }
        }
      }
      return num;
    }

    public static int GetDirNumber(string d)
    {
      if (d.EndsWith("_auto"))
        d = d.Substring(0, d.Length - 5);
      int result;
      if (int.TryParse(d.Substring(d.Length - 6, 6), out result))
        return result;
      return -1;
    }

    public static int GetTotalBytes(string[] files)
    {
      int num = 0;
      if (files != null && files.Length > 0)
      {
        foreach (string file in files)
        {
          try
          {
            lock (Globals1.SaveSemaphore)
            {
              using (Stream stream = FileSystem.OpenRead(file))
                num += (int) stream.Length;
            }
          }
          catch (IOException ex)
          {
          }
          catch (InvalidOperationException ex)
          {
          }
        }
      }
      return num;
    }

    public static string GetAssetName(string filename)
    {
      string str = filename;
      int length = str.IndexOf('.');
      if (length >= 0)
        str = str.Substring(0, length);
      int num = str.LastIndexOf('\\');
      if (num >= 0)
        str = str.Substring(num + 1);
      return str;
    }

    public static PriceList.Price[] GetNewDefaultPriceList()
    {
      PriceList.Price[] priceArray = new PriceList.Price[Globals1.ItemData.Length];
      PriceList.Price price = new PriceList.Price();
      for (int index = 0; index < priceArray.Length; ++index)
      {
        price.Buy = Globals1.ItemData[index].MinCSPrice / 2;
        price.Sell = Globals1.ItemData[index].MinCSPrice;
        price.Perc = 50;
        price.UsePerc = true;
        price.ForSale = true;
        priceArray[index] = price;
      }
      return priceArray;
    }

    public static int CreateNewComPack(GameInstance instance, string newPackName)
    {
      int newComDirNumber = Globals2.GetNewComDirNumber();
      lock (Globals1.SaveSemaphore)
      {
        try
        {
          string str = Globals2.ComponentPath(newComDirNumber);
          FileSystem.CreateDir(str.Substring(0, str.Length - 1));
          using (Stream file = FileSystem.CreateFile(str + "header.dat"))
          {
            using (BinaryWriter binaryWriter = new BinaryWriter(file))
              binaryWriter.Write(newPackName);
          }
          if (instance != null)
          {
            if (instance.VoxelModelManager != null)
              instance.VoxelModelManager.NewCompPackDirAdded(newComDirNumber, newPackName);
          }
        }
        catch (Exception ex)
        {
        }
      }
      return newComDirNumber;
    }

    public static int CreateNewComPackIfDoesNotExist(GameInstance instance, string comPack)
    {
      if (instance == null || instance.VoxelModelManager == null)
        return -1;
      int dirNum = instance.VoxelModelManager.GetDirNum(comPack);
      if (dirNum >= 0)
        return dirNum;
      return Globals2.CreateNewComPack(instance, comPack);
    }

    public static Script GetSystemScript(string name)
    {
      return Globals2.GetScript(Globals2.SystemScripts, name);
    }

    public static Script GetGlobalScript(string name)
    {
      return Globals2.GetScript(Globals2.GlobalScripts, name);
    }

    private static Script GetScript(List<Script> list, string name)
    {
      for (int index = 0; index < list.Count; ++index)
      {
        if (name.Equals(list[index].Name, StringComparison.OrdinalIgnoreCase))
          return list[index];
      }
      return (Script) null;
    }

    public static bool Steam()
    {
      Globals2.SteamManager = new SteamManager(347600U);
      return Globals2.SteamManager.Initialize(new SteamWarningMessageHookDelegate(Globals2.SteamAPIDebugTextHook));
    }

    private static void BuildRareData()
    {
      Globals2.MaxRareLevel = 0;
      foreach (RareDataXML rareDataXml in Globals1.RareData)
      {
        if ((int) rareDataXml.Level > Globals2.MaxRareLevel)
          Globals2.MaxRareLevel = (int) rareDataXml.Level;
      }
    }

    public static bool IsRareItem(Item itemID)
    {
      foreach (RareDataXML rareDataXml in Globals1.RareData)
      {
        if (rareDataXml.ItemID == itemID)
          return true;
      }
      return false;
    }

    public static int GetNewPhotoNumber()
    {
      for (int index = 1; index < Globals2.photoIDsUsed.Length; ++index)
      {
        if (!Globals2.photoIDsUsed[index])
        {
          Globals2.photoIDsUsed[index] = true;
          Globals2.SavePhotoIDsUsed();
          return index;
        }
      }
      return 0;
    }

    public static int GetPhotoCount()
    {
      int num = 0;
      for (int index = 1; index < Globals2.photoIDsUsed.Length; ++index)
      {
        if (Globals2.photoIDsUsed[index])
          ++num;
      }
      return num;
    }

    private static void LoadPhotoDat()
    {
      bool flag = false;
      lock (Globals2.fileAccessLock)
      {
        if (!FileSystem.IsFileExist("photos.idx"))
        {
          flag = true;
          FileSystem.DeleteFile("photos.dat");
        }
        else
        {
          using (Stream input = FileSystem.OpenRead("photos.idx"))
          {
            using (BinaryReader binaryReader = new BinaryReader(input))
            {
              if (binaryReader.ReadInt32() < 137)
              {
                flag = true;
              }
              else
              {
                int num = binaryReader.ReadInt32();
                for (int index = 1; index <= num; ++index)
                  Globals2.photoIDsUsed[index] = binaryReader.ReadBoolean();
              }
            }
          }
        }
      }
      if (!flag)
        return;
      Globals2.BuildPhotoDat();
      Globals2.SavePhotoIDsUsed();
    }

    private static void BuildPhotoDat()
    {
      lock (Globals2.fileAccessLock)
      {
        for (int index = 1; index < Globals2.photoIDsUsed.Length; ++index)
          Globals2.photoIDsUsed[index] = false;
        if (!FileSystem.IsDirExist("Photo"))
          return;
        foreach (string dir in FileSystem.GetDirs("Photo\\"))
        {
          int result;
          if (int.TryParse(dir.Substring(dir.Length - 6), out result) && result > 0 && result < Globals2.photoIDsUsed.Length)
            Globals2.photoIDsUsed[result] = true;
        }
      }
    }

    private static void SavePhotoIDsUsed()
    {
      lock (Globals2.fileAccessLock)
      {
        using (Stream output = FileSystem.OpenWrite("photos.idx"))
        {
          using (BinaryWriter binaryWriter = new BinaryWriter(output))
          {
            binaryWriter.Write(294);
            binaryWriter.Write(Globals2.photoIDsUsed.Length - 1);
            for (int index = 1; index < Globals2.photoIDsUsed.Length; ++index)
              binaryWriter.Write(Globals2.photoIDsUsed[index]);
          }
        }
      }
    }

    public static void DeletePhoto(int photoID)
    {
      lock (Globals2.fileAccessLock)
      {
        if (photoID <= 0 || photoID >= Globals2.photoIDsUsed.Length)
          return;
        FileSystem.DeleteDir(Globals2.GetPhotoFilePath(photoID));
        Globals2.photoIDsUsed[photoID] = false;
        Globals2.SavePhotoIDsUsed();
      }
    }

    private static void CopySystemMapsToUserStorageCore()
    {
      string str1 = "SystemMaps";
      string str2 = "Content\\Map\\SystemMaps";
      Globals2.Contents.TrialMapDirNum = Globals2.GetNewDirNumber(str1);
      string filePath = Globals2.GetFilePath(str1, Globals2.Contents.TrialMapDirNum);
      FileSystem.CreateDir(filePath);
      Globals2.CopyFileFromTitleToUser(str2 + "\\000002\\header.dat", filePath + "header.dat");
      Globals2.CopyFileFromTitleToUser(str2 + "\\000002\\0.reg", filePath + "0.reg");
      Globals2.CopyFileFromTitleToUser(str2 + "\\000002\\1.reg", filePath + "1.reg");
      Globals2.CopyFileFromTitleToUser(str2 + "\\000002\\16384.reg", filePath + "16384.reg");
      Globals2.CopyFileFromTitleToUser(str2 + "\\000002\\16385.reg", filePath + "16385.reg");
      Globals2.SaveGlobalDataContents();
    }

    private static void CopyFileFromTitleToUser(string titleFilename, string userFilename)
    {
      using (Stream stream = TitleContainer.OpenStream(titleFilename))
      {
        byte[] numArray = new byte[stream.Length];
        stream.Read(numArray, 0, (int) stream.Length);
        byte num1 = 0;
        int num2 = 0;
        using (MemoryStream memoryStream = new MemoryStream(numArray, numArray.Length - 5, 5))
        {
          using (BinaryReader binaryReader = new BinaryReader((Stream) memoryStream))
          {
            num1 = binaryReader.ReadByte();
            num2 = binaryReader.ReadInt32();
          }
        }
        byte[] messageHash = Encryption.GetMessageHash(numArray, numArray.Length - 5 - num2, 0);
        byte num3 = DataScrambler.RandomScramble(messageHash);
        using (Stream file = FileSystem.CreateFile(userFilename))
        {
          using (BinaryWriter binaryWriter = new BinaryWriter(file))
          {
            binaryWriter.Write(numArray, 0, numArray.Length - 5 - num2);
            binaryWriter.Write(messageHash);
            binaryWriter.Write(num3);
            binaryWriter.Write(messageHash.Length);
          }
        }
      }
    }

    public static GamerType GetGamerType(GamerID gamerID)
    {
      switch (gamerID.ID)
      {
        case -3:
          return GamerType.ScriptMove;
        case -2:
          return GamerType.Script;
        case -1:
          return GamerType.Generation;
        default:
          return GamerType.Gamer;
      }
    }

    public static GamerID GetGamerType(GamerType gamerType)
    {
      switch (gamerType)
      {
        case GamerType.Generation:
          return GamerID.Sys1;
        case GamerType.Script:
          return GamerID.Sys2;
        case GamerType.ScriptMove:
          return GamerID.Sys3;
        default:
          return GamerID.Sys1;
      }
    }

    public static bool IsXboxLiveGamer(PlayerIndex playerIndex, bool checkAgainstGameDefaults)
    {
      return true;
    }

    public static string SubstituteText(ScriptInstance si, string text)
    {
      text = Globals2.SubstituteGeneral(si, text);
      text = Globals2.SubstituteGamertag(text, si.Player);
      text = Globals2.SubstituteClan(text, si.Player);
      text = Globals2.SubstituteHistory(si, text, si.Player);
      text = Globals2.SubstituteSysHistory(si, text, si.Instance);
      text = Globals2.SubstituteClanHistory(si, text, si.Player);
      text = Globals2.SubstituteVar(si, text);
      return text;
    }

    public static string SubstituteText(string text, GameInstance instance, Player player)
    {
      text = Globals2.SubstituteGamertag(text, player);
      text = Globals2.SubstituteClan(text, player);
      text = Globals2.SubstituteHistory((ScriptInstance) null, text, player);
      text = Globals2.SubstituteSysHistory((ScriptInstance) null, text, instance);
      text = Globals2.SubstituteClanHistory((ScriptInstance) null, text, player);
      return text;
    }

    public static string SubstituteGeneral(ScriptInstance si, string text)
    {
      if (si != null && si.BlockOffset.HasValue && text.Contains("[coord]"))
      {
        if (si.BlockOffsetString == null)
          si.BlockOffsetString = string.Format("{0}_{1}_{2}", (object) si.BlockOffset.Value.X, (object) si.BlockOffset.Value.Y, (object) si.BlockOffset.Value.Z);
        text = text.Replace("[coord]", si.BlockOffsetString);
      }
      return text;
    }

    public static string SubstituteGamertag(string text, Player player)
    {
      if (text != null && text.Length > 0)
      {
        int num = text.IndexOf("[gamertag]", StringComparison.OrdinalIgnoreCase);
        if (num >= 0)
        {
          string str = player != null ? player.Gamertag : "unknown";
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          for (; num >= 0; num = text.IndexOf("[gamertag]", startIndex, StringComparison.OrdinalIgnoreCase))
          {
            if (num > startIndex)
              stringBuilder.Append(text.Substring(startIndex, num - startIndex));
            stringBuilder.Append(str);
            startIndex = num + 10;
          }
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex, text.Length - startIndex));
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static string SubstituteClan(string text, Player player)
    {
      if (text != null && text.Length > 0)
      {
        int num = text.IndexOf("[clan]", StringComparison.OrdinalIgnoreCase);
        if (num >= 0)
        {
          string str = player != null ? player.ClanName : "unknown";
          if (str == null || str.Length < 1)
            str = "unknown";
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          for (; num >= 0; num = text.IndexOf("[clan]", startIndex, StringComparison.OrdinalIgnoreCase))
          {
            if (num > startIndex)
              stringBuilder.Append(text.Substring(startIndex, num - startIndex));
            stringBuilder.Append(str);
            startIndex = num + 6;
          }
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex, text.Length - startIndex));
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static string SubstituteVar(ScriptInstance si, string text)
    {
      if (text != null && text.Length > 2)
      {
        bool flag1 = false;
        int num1 = -1;
        for (int index1 = 0; !flag1 && index1 < text.Length; ++index1)
        {
          if (num1 < 0)
          {
            if (text[index1] == '[')
              num1 = index1 + 1;
          }
          else if ((text[index1] == ']' || text[index1] == ':') && index1 > num1)
          {
            for (int index2 = 0; index2 < si.VarCount; ++index2)
            {
              bool flag2 = true;
              for (int index3 = num1; index3 < index1; ++index3)
              {
                int index4 = index3 - num1;
                if (index4 < si.VarNames[index2].Length && (int) char.ToLower(text[index3]) != (int) char.ToLower(si.VarNames[index2][index4]))
                {
                  flag2 = false;
                  break;
                }
              }
              if (flag2)
              {
                flag1 = true;
                break;
              }
            }
          }
        }
        if (flag1)
        {
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          int num2 = text.IndexOf('[');
          bool flag2 = false;
          for (; num2 >= 0; num2 = text.IndexOf('[', startIndex))
          {
            int num3 = text.IndexOf(':', num2 + 1);
            int num4 = text.IndexOf(']', num2 + 1);
            if (num3 <= num2 || num3 > num4)
              num3 = num4;
            if (num3 > num2 + 1)
            {
              string str = text.Substring(num2 + 1, num3 - num2 - 1);
              for (int index = 0; index < si.VarCount; ++index)
              {
                if (str.Equals(si.VarNames[index], StringComparison.OrdinalIgnoreCase))
                {
                  stringBuilder.Append(text.Substring(startIndex, num2 - startIndex));
                  string format = (string) null;
                  int result;
                  if (num4 > num3 + 1 && int.TryParse(text.Substring(num3 + 1, num4 - num3 - 1), out result))
                    format = "{0:N" + (object) result + "}";
                  if (format != null)
                    stringBuilder.AppendFormat(format, (object) si.Vars[index]);
                  else
                    stringBuilder.Append(si.Vars[index]);
                  flag2 = true;
                  break;
                }
              }
            }
            if (!flag2)
            {
              if (num4 < startIndex)
                num4 = text.Length - 1;
              for (int index = startIndex; index <= num4; ++index)
                stringBuilder.Append(text[index]);
            }
            flag2 = false;
            startIndex = num4 + 1;
          }
          if (!flag2)
          {
            for (int index = startIndex; index < text.Length; ++index)
              stringBuilder.Append(text[index]);
          }
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static string SubstituteHistory(ScriptInstance si, string text, Player player)
    {
      if (text != null && text.Length > 0 && player != null)
      {
        int num1 = text.IndexOf("[history:", StringComparison.OrdinalIgnoreCase);
        if (num1 >= 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          for (; num1 >= 0; num1 = text.IndexOf("[history:", startIndex, StringComparison.OrdinalIgnoreCase))
          {
            if (num1 > startIndex)
              stringBuilder.Append(text.Substring(startIndex, num1 - startIndex));
            int num2 = text.IndexOf(']', num1 + 9);
            if (num2 >= 0)
            {
              string str = text.Substring(num1 + 9, num2 - (num1 + 9));
              long history = player.History.GetHistory(str.ToLower());
              stringBuilder.Append(history);
              startIndex = num2 + 1;
            }
            else
              break;
          }
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex, text.Length - startIndex));
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static string SubstituteSysHistory(
      ScriptInstance si,
      string text,
      GameInstance instance)
    {
      if (text != null && text.Length > 0 && instance != null)
      {
        int num1 = text.IndexOf("[syshistory:", StringComparison.OrdinalIgnoreCase);
        if (num1 >= 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          for (; num1 >= 0; num1 = text.IndexOf("[syshistory:", startIndex, StringComparison.OrdinalIgnoreCase))
          {
            if (num1 > startIndex)
              stringBuilder.Append(text.Substring(startIndex, num1 - startIndex));
            int num2 = text.IndexOf(']', num1 + 12);
            if (num2 >= 0)
            {
              string str = text.Substring(num1 + 12, num2 - (num1 + 12));
              long history = instance.History.GetHistory(str.ToLower());
              stringBuilder.Append(history);
              startIndex = num2 + 1;
            }
            else
              break;
          }
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex, text.Length - startIndex));
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static string SubstituteClanHistory(ScriptInstance si, string text, Player player)
    {
      if (text != null && text.Length > 0 && player != null)
      {
        int num1 = text.IndexOf("[clanhistory:", StringComparison.OrdinalIgnoreCase);
        if (num1 >= 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          int startIndex = 0;
          for (; num1 >= 0; num1 = text.IndexOf("[clanhistory:", startIndex, StringComparison.OrdinalIgnoreCase))
          {
            if (num1 > startIndex)
              stringBuilder.Append(text.Substring(startIndex, num1 - startIndex));
            int num2 = text.IndexOf(']', num1 + 13);
            if (num2 >= 0)
            {
              string str = text.Substring(num1 + 13, num2 - (num1 + 13));
              History clanHistory = player.GameInstance.GetClanHistory(player.ClanName);
              long num3 = clanHistory != null ? clanHistory.GetHistory(str.ToLower()) : 0L;
              stringBuilder.Append(num3);
              startIndex = num2 + 1;
            }
            else
              break;
          }
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex, text.Length - startIndex));
          return stringBuilder.ToString();
        }
      }
      return text;
    }

    public static bool GetParticleTemplate(int templateID, out ParticleData data)
    {
      data = new ParticleData();
      if (templateID < 1)
        return false;
      if (templateID < Globals2.SystemParticleData.Length)
      {
        data = Globals2.SystemParticleData[templateID];
        return true;
      }
      if (templateID >= Globals2.SystemParticleData.Length + Globals2.CustomParticleData.Count)
        return false;
      data = Globals2.CustomParticleData[templateID - Globals2.SystemParticleData.Length];
      return true;
    }

    public static string GetParticleTemplateName(ParticleData data)
    {
      foreach (ParticleData particleData in Globals2.CustomParticleData)
      {
        if (particleData.Equals((object) data))
          return particleData.Name;
      }
      return (string) null;
    }

    public static void AddParticleTemplate(string name, ParticleData data)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < Globals2.CustomParticleData.Count; ++index2)
      {
        if (Globals2.CustomParticleData[index2].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          index1 = index2;
          break;
        }
      }
      if (index1 >= 0)
      {
        data.Name = name;
        Globals2.CustomParticleData[index1] = data;
      }
      else
      {
        data.Name = name;
        Globals2.CustomParticleData.Add(data);
      }
    }

    public static void DeleteParticleTemplate(string template)
    {
      for (int index = Globals2.CustomParticleData.Count - 1; index >= 0; --index)
      {
        if (Globals2.CustomParticleData[index].Name.Equals(template, StringComparison.OrdinalIgnoreCase))
          Globals2.CustomParticleData.RemoveAt(index);
      }
    }

    public static List<ParticleData> LoadParticleTemplates()
    {
      try
      {
        if (FileSystem.IsFileExist("ParticleEmitters.db"))
        {
          using (Stream input = FileSystem.OpenRead("ParticleEmitters.db"))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              int capacity = reader.ReadInt32();
              List<ParticleData> particleDataList = new List<ParticleData>(capacity);
              for (int index = 0; index < capacity; ++index)
              {
                ParticleData particleData = new ParticleData();
                particleData.Name = reader.ReadString();
                particleData.ReadState(reader, version);
                particleDataList.Add(particleData);
              }
              return particleDataList;
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return new List<ParticleData>();
    }

    public static List<Script> LoadSystemScripts()
    {
      try
      {
        string path = "Content\\Map\\ScriptsSystem.db";
        if (TitleFileSystem.IsFileExist(path))
        {
          using (Stream input = TitleFileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              return MapLoader.ReadScripts(reader, version);
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return new List<Script>();
    }

    public static List<Script> LoadGlobalScripts()
    {
      try
      {
        if (FileSystem.IsFileExist("ScriptsGlobal.db"))
        {
          using (Stream input = FileSystem.OpenRead("ScriptsGlobal.db"))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              List<Script> result = MapLoader.ReadScripts(reader, version);
              Globals2.AddSystemScripts(result, version);
              return result;
            }
          }
        }
        else
        {
          List<Script> result = new List<Script>();
          result.Add(new Script("Global\\Introduction", 5)
          {
            Commands = {
              "// Global scripts are scripts that are accessible to all worlds.",
              "// To make a script global, simply prefix it's name with 'Global\\'",
              "// If you host a multiplayer world, your global scripts are accessible.",
              "// If you join a multiplayer world (not host), your global scripts are not accessible.",
              "// If you share a world, only the global scripts used in that world are included in the share."
            }
          });
          Globals2.AddSystemScripts(result, 0);
          return result;
        }
      }
      catch (Exception ex)
      {
      }
      return new List<Script>();
    }

    private static void AddSystemScripts(List<Script> result, int version)
    {
      if (version < 238)
      {
        result.Add(new Script("Global\\System\\Events\\ItemSwing", 4)
        {
          Commands = {
            "// Assign gun/spell scripts to items",
            "// The best place to put these commands is in the PlayerJoin event script",
            "SetEventScript [ItemSwing] [Revolver] [Global\\System\\Guns\\GunFire]",
            "SetEventScript [ItemSwing] [LightStaff] [Global\\System\\Spells\\Heal]"
          }
        });
        result.Add(new Script("Global\\System\\Guns\\GunFire", 12)
        {
          Commands = {
            "// A general gun script.",
            "// You must assign this script as the gun ItemSwing script - See SetEventScript [ItemSwing]",
            "// e.g. SetEventScript [ItemSwing] [Revolver] [Global\\System\\Guns\\GunFire]",
            "",
            "// put any gunshot particle effects here",
            "if",
            "intersect [ray] [vrel:0.2,0,0] [vrel:50,0,0] [players|mobs] // Gun has range of 50 blocks",
            "then",
            "context [player] [target]",
            "health [-50] // gun shot does 50 damage",
            "//put any gunshot hit/wound particle effects here that are relative to the target",
            "endif"
          }
        });
        result.Add(new Script("Global\\System\\Spells\\Heal", 12)
        {
          Commands = {
            "// A general heal script.",
            "// You must assign this script as the staff ItemSwing script - See SetEventScript [ItemSwing]",
            "// e.g. SetEventScript [ItemSwing] [LightStaff] [Global\\System\\Spells\\Heal]",
            "",
            "// put any spellcast particle effects here",
            "if",
            "intersect [sphere] [prel:10,2.5,0] [5] [players|mobs]",
            "then",
            "context [player] [target]",
            "health [50] // spell heals 50 points",
            "//put any healing particle effects here that are relative to the target",
            "endif"
          }
        });
      }
      if (version >= 247)
        return;
      result.Add(new Script("Global\\System\\Misc\\Loop1DUsingVars", 14)
      {
        Commands = {
          "// A script that performs a 1D loop using variables.",
          "var [x]",
          "",
          "// Add your loop reliant code here",
          "",
          "var [x] = [x] + [1]",
          "if",
          "isvar [x] [=] [10] // replace 10 with actual loop size",
          "then",
          "exit",
          "endif",
          "",
          "loop [1]"
        }
      });
      result.Add(new Script("Global\\System\\Misc\\Loop2DUsingVars", 21)
      {
        Commands = {
          "// A script that performs a 2D loop using variables.",
          "var [x] [y]",
          "",
          "// Add your loop reliant code here",
          "",
          "var [x] = [x] + [1]",
          "if",
          "isvar [x] [=] [10] // replace 10 with actual loop size",
          "then",
          "var [y] = [y] + [1]",
          "var [x] = [0]",
          "endif",
          "",
          "if",
          "isvar [y] [=] [10] // replace 10 with actual loop size",
          "then",
          "exit",
          "endif",
          "",
          "loop [1]"
        }
      });
      result.Add(new Script("Global\\System\\Misc\\Loop3DUsingVars", 28)
      {
        Commands = {
          "// A script that performs a 3D loop using variables.",
          "var [x] [y] [z]",
          "",
          "// Add your loop reliant code here",
          "",
          "var [x] = [x] + [1]",
          "if",
          "isvar [x] [=] [10] // replace 10 with actual X loop size",
          "then",
          "var [z] = [z] + [1]",
          "var [x] = [0]",
          "endif",
          "",
          "if",
          "isvar [z] [=] [10] // replace 10 with actual Z loop size",
          "then",
          "var [y] = [y] + [1]",
          "var [z] = [0]",
          "endif",
          "",
          "if",
          "isvar [y] [=] [10] // replace 10 with actual Y loop size",
          "then",
          "exit",
          "endif",
          "",
          "loop [1]"
        }
      });
    }

    public static void SaveParticleTemplates()
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = FileSystem.OpenWrite("ParticleEmitters.db"))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            writer.Write(Globals2.CustomParticleData.Count);
            foreach (ParticleData particleData in Globals2.CustomParticleData)
            {
              writer.Write(particleData.Name);
              particleData.WriteState(writer);
            }
          }
        }
      }
    }

    public static void SaveSystemScripts(List<Script> scripts)
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = TitleFileSystem.OpenFile("Content\\Map\\ScriptsSystem.db", FileMode.Create, FileAccess.Write, FileShare.Write))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            Globals2.SystemScripts = MapSaver.WriteSystemScripts(writer, scripts);
          }
        }
      }
    }

    public static void SaveGlobalScripts(List<Script> scripts)
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = FileSystem.OpenWrite("ScriptsGlobal.db"))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            Globals2.GlobalScripts = MapSaver.WriteGlobalScripts(writer, scripts);
          }
        }
      }
    }

    private static List<InputProfile> LoadInputProfiles()
    {
      List<InputProfile> inputProfileList = new List<InputProfile>();
      InputProfile profile = new InputProfile()
      {
        Account = "System",
        Name = ""
      };
      InputManager1.RestoreDefaults(profile);
      inputProfileList.Add(profile);
      try
      {
        if (FileSystem.IsFileExist("InputProfiles.db"))
        {
          using (Stream input = FileSystem.OpenRead("InputProfiles.db"))
          {
            using (BinaryReader reader = new BinaryReader(input))
            {
              int version = reader.ReadInt32();
              int num = reader.ReadInt32();
              for (int index = 0; index < num; ++index)
                inputProfileList.Add(Globals2.ReadInputProfile(reader, version));
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return inputProfileList;
    }

    private static InputProfile ReadInputProfile(BinaryReader reader, int version)
    {
      InputProfile profile = new InputProfile();
      InputManager1.RestoreDefaults(profile);
      profile.Account = reader.ReadString();
      profile.Name = reader.ReadString();
      profile.MouseLookAtSmoothing = reader.ReadByte();
      profile.MouseSensitivity = reader.ReadSingle();
      profile.GamePadSensitivity = reader.ReadSingle();
      profile.GamePadInvertY = reader.ReadBoolean();
      profile.GamePadRumble = reader.ReadBoolean();
      int num = (int) reader.ReadUInt16();
      for (int index1 = 0; index1 < num; ++index1)
      {
        ushort index2 = reader.ReadUInt16();
        profile.InputScheme[index2] = new InputItem()
        {
          Button = (Buttons) reader.ReadInt32(),
          Key = (Keys) reader.ReadInt32(),
          KeyAlt = reader.ReadBoolean(),
          KeyCtrl = reader.ReadBoolean(),
          KeyShift = reader.ReadBoolean(),
          MouseButton = (StudioForge.Engine.Integration.MouseButtons) reader.ReadInt32(),
          MouseAlt = reader.ReadBoolean(),
          MouseCtrl = reader.ReadBoolean(),
          MouseShift = reader.ReadBoolean(),
          EnabledKey = reader.ReadBoolean(),
          EnabledMouseButton = reader.ReadBoolean(),
          EnabledButton = reader.ReadBoolean()
        };
      }
      return profile;
    }

    public static void SaveInputProfiles()
    {
      lock (Globals1.SaveSemaphore)
      {
        using (Stream output = FileSystem.OpenWrite("InputProfiles.db"))
        {
          using (BinaryWriter writer = new BinaryWriter(output))
          {
            writer.Write(294);
            writer.Write(Globals2.InputProfiles.Count - 1);
            for (int index = 1; index < Globals2.InputProfiles.Count; ++index)
              Globals2.WriteInputProfile(writer, Globals2.InputProfiles[index]);
          }
        }
      }
    }

    private static void WriteInputProfile(BinaryWriter writer, InputProfile profile)
    {
      writer.Write(profile.Account);
      writer.Write(profile.Name);
      writer.Write(profile.MouseLookAtSmoothing);
      writer.Write(profile.MouseSensitivity);
      writer.Write(profile.GamePadSensitivity);
      writer.Write(profile.GamePadInvertY);
      writer.Write(profile.GamePadRumble);
      writer.Write((ushort) profile.InputScheme.Count);
      foreach (KeyValuePair<ushort, InputItem> keyValuePair in profile.InputScheme)
      {
        writer.Write(keyValuePair.Key);
        writer.Write((int) keyValuePair.Value.Button);
        writer.Write((int) keyValuePair.Value.Key);
        writer.Write(keyValuePair.Value.KeyAlt);
        writer.Write(keyValuePair.Value.KeyCtrl);
        writer.Write(keyValuePair.Value.KeyShift);
        writer.Write((int) keyValuePair.Value.MouseButton);
        writer.Write(keyValuePair.Value.MouseAlt);
        writer.Write(keyValuePair.Value.MouseCtrl);
        writer.Write(keyValuePair.Value.MouseShift);
        writer.Write(keyValuePair.Value.EnabledKey);
        writer.Write(keyValuePair.Value.EnabledMouseButton);
        writer.Write(keyValuePair.Value.EnabledButton);
      }
    }

    public static InputProfile GetInputProfile(string name)
    {
      foreach (InputProfile inputProfile in Globals2.InputProfiles)
      {
        if (inputProfile.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return inputProfile;
      }
      return (InputProfile) null;
    }

    public static void DeleteInputProfile(InputProfile profile)
    {
      for (int index = Globals2.InputProfiles.Count - 1; index >= 0; --index)
      {
        if (Globals2.InputProfiles[index].Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase))
          Globals2.InputProfiles.RemoveAt(index);
      }
    }

    public static void AddOrUpdateInputProfile(InputProfile profile)
    {
      if (profile == null || !profile.Name.IsNotEmpty())
        return;
      for (int index = 0; index < Globals2.InputProfiles.Count; ++index)
      {
        if (Globals2.InputProfiles[index].Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase))
        {
          Globals2.InputProfiles[index] = profile;
          return;
        }
      }
      Globals2.InputProfiles.Add(profile);
    }

    public static void SetParticleDataFromTemplate(int templateID, ref ParticleData data)
    {
      ParticleData data1;
      if (!Globals2.GetParticleTemplate(templateID, out data1))
        return;
      data.CopyFrom(ref data1);
      data.Name = data1.Name;
    }

    public static string RandomString(PcgRandom r, int minlen, int maxlen)
    {
      int num = r.Next(minlen, maxlen);
      string str = "";
      for (int index = 0; index < num; ++index)
        str += (string) (object) (char) r.Next(65, 90);
      return str;
    }

    public static long ValidateDateTime(long date)
    {
      try
      {
        Utils.DateFromBinary(date);
      }
      catch (Exception ex)
      {
        date = Utils.DateToBinary(DateTime.MinValue);
      }
      return date;
    }

    public static BlueprintXML GetBlueprintData(Item itemID)
    {
      foreach (BlueprintXML blueprintXml in Globals1.BlueprintData)
      {
        if (blueprintXml.ItemID == itemID)
          return blueprintXml;
      }
      return (BlueprintXML) null;
    }

    public static string StackTraceToString(StackTrace stackTrace)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < stackTrace.FrameCount; ++index)
      {
        MethodBase method = stackTrace.GetFrame(index).GetMethod();
        stringBuilder.Append("\n   at ");
        stringBuilder.Append(method.DeclaringType.FullName);
        stringBuilder.Append('.');
        stringBuilder.Append(method.Name);
        stringBuilder.Append("()");
      }
      return stringBuilder.ToString();
    }

    public struct ComPackData
    {
      public string PackName;
      public int DirNum;

      public ComPackData(string name, int dir)
      {
        this.PackName = name;
        this.DirNum = dir;
      }
    }

    private class GamertagDBNotFound : CoreException
    {
      public GamertagDBNotFound()
        : base((string) null)
      {
      }
    }

    private enum SoundElem
    {
      None,
      Step,
      Mine,
      Dig,
      Chop,
      Use,
      UseFail,
      Hit,
    }

    private struct SaveAllState
    {
      public bool WriteGlobalData;
      public bool WriteGamertagData;
    }
  }
}
