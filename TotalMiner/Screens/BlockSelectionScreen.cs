// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BlockSelectionScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class BlockSelectionScreen : MinerToolScreen
  {
    private int initialSlot = -1;
    private int oldSlot = -1;
    private string itemTypeName = "";
    private const int slotSize = 49;
    private BlockSelectMode mode;
    private Inventory inventory;
    private Inventory selectedInventory;
    private SpriteBatchSafe spriteBatchPoint;
    private SpriteBatchSafe spriteBatchText;
    private GameInstance instance;
    private int currentSlot;
    private int thumbstickTimer;
    private int inventorySize;
    private string selectDesc;
    private Block playerBlockID;
    private SelectBlockCallBack blockCallback;
    private SelectItemCallBack itemCallback;
    private int page;
    private int pagesize;
    private string pageString;
    private Texture2D lockedTexture;
    private Rectangle triggerRect;
    private List<PhotoTag> photos;
    private PhotoLoader photoLoader;
    private int photoCount;

    protected int PageCount
    {
      get
      {
        if (this.pagesize <= 0)
          return 0;
        return ((int) this.inventory.PackSize - 1) / this.pagesize + 1;
      }
    }

    protected virtual InventoryItem CursorItem
    {
      get
      {
        return this.GetItem(this.currentSlot);
      }
      set
      {
        this.SetItem(this.currentSlot, value);
      }
    }

    protected InventoryItem GetItem(int slot)
    {
      if (!this.IsSelectingBlockTexture)
        return this.inventory[slot + this.page * this.pagesize];
      if (slot >= 20)
        return this.inventory[slot - 20 + this.page * this.pagesize];
      if (slot >= 16)
        return InventoryItem.Empty;
      return this.selectedInventory[slot];
    }

    private Inventory GetInventory(int slot)
    {
      if (!this.IsSelectingBlockTexture || slot >= 20)
        return this.inventory;
      return this.selectedInventory;
    }

    protected void SetItem(int slot, InventoryItem item)
    {
      this.GetInventory(slot)[this.GetFullSlotID(slot)] = item;
    }

    private int GetFullSlotID(int slot)
    {
      if (!this.IsSelectingBlockTexture)
        return slot + this.page * this.pagesize;
      if (slot < 20)
        return slot;
      return slot - 20 + this.page * this.pagesize;
    }

    private bool CanRemoveItem
    {
      get
      {
        if (this.mode == BlockSelectMode.SelectingArcadeGame || this.playerBlockID == Block.CoverBlock)
          return false;
        bool flag = this.IsSelectingBlockTexture && this.currentSlot < 20 && this.CursorItem.ItemID != Item.None;
        if (this.mode == BlockSelectMode.SelectingChannel && (this.CursorItem.ItemID == Item.ColorRed && !this.player.IsAdmin))
          flag = false;
        if (this.mode == BlockSelectMode.SelectingStainedGlass)
          flag = false;
        if (this.currentSlot <= 0)
          return false;
        if (this.mode != BlockSelectMode.SelectingPhoto)
          return flag;
        if (flag)
          return this.instance.IsHost;
        return false;
      }
    }

    private bool CanSelectNewOrRemoveExisting
    {
      get
      {
        if (this.player != null)
          return this.player.HasPermission(Permissions.Creative);
        return true;
      }
    }

    private bool IsSelectingBlockTexture
    {
      get
      {
        if (this.mode != BlockSelectMode.SelectingBlockTexture && this.mode != BlockSelectMode.SelectingUsedBlockTexture && (this.mode != BlockSelectMode.SelectingBlockTextureForMultTextureBlock && this.mode != BlockSelectMode.SelectingChannel) && (this.mode != BlockSelectMode.SelectingPhoto && this.mode != BlockSelectMode.SelectingStainedGlass && (this.mode != BlockSelectMode.SelectingArcadeGame && this.mode != BlockSelectMode.SelectingCustomBlock)) && this.mode != BlockSelectMode.SelectingDecal)
          return this.mode == BlockSelectMode.SelectingKey;
        return true;
      }
    }

    private bool IsGod
    {
      get
      {
        if (this.player != null)
          return this.player.IsGod;
        return false;
      }
    }

    public BlockSelectionScreen(
      GameInstance instance,
      Player player,
      SelectBlockCallBack callback,
      string selectDesc,
      BlockSelectMode mode)
      : base(player)
    {
      this.blockCallback = callback;
      this.Init(instance, player, selectDesc, mode, Block.None);
    }

    public BlockSelectionScreen(
      GameInstance instance,
      Player player,
      SelectItemCallBack callback,
      string selectDesc,
      BlockSelectMode mode,
      Block playerBlockID,
      int slotID)
      : base(player)
    {
      this.itemCallback = callback;
      this.Init(instance, player, selectDesc, mode, playerBlockID);
      this.initialSlot = this.currentSlot = slotID;
    }

    public BlockSelectionScreen(
      GameInstance instance,
      Player player,
      SelectBlockCallBack callback,
      string selectDesc,
      BlockSelectMode mode,
      Block playerBlockID,
      int slotID)
      : base(player)
    {
      this.blockCallback = callback;
      this.Init(instance, player, selectDesc, mode, playerBlockID);
      this.initialSlot = this.currentSlot = slotID;
    }

    private void Init(
      GameInstance instance,
      Player player,
      string selectDesc,
      BlockSelectMode mode,
      Block playerBlockID)
    {
      this.instance = instance;
      this.selectDesc = selectDesc;
      this.mode = mode;
      this.playerBlockID = playerBlockID;
      this.currentSlot = 0;
      int num;
      switch (mode)
      {
        case BlockSelectMode.SelectingUsedBlockTexture:
        case BlockSelectMode.SelectingChannel:
        case BlockSelectMode.SelectingKey:
          num = 30;
          break;
        case BlockSelectMode.SelectingBlockForReplaceTexture:
        case BlockSelectMode.SelectingDecal:
          num = 50;
          break;
        case BlockSelectMode.SelectingBlockTextureForMultTextureBlock:
          num = 60;
          break;
        case BlockSelectMode.SelectingPhoto:
          num = 256;
          break;
        case BlockSelectMode.SelectingStainedGlass:
        case BlockSelectMode.SelectingArcadeGame:
        case BlockSelectMode.SelectingCustomBlock:
          num = 20;
          break;
        default:
          num = this.IsGod || mode == BlockSelectMode.CreativeClear || mode == BlockSelectMode.CreativeReplace ? 280 : 210;
          break;
      }
      this.inventorySize = num;
      this.pageString = "1";
      this.pagesize = Math.Min(this.inventorySize, 70);
      this.inventory = new Inventory(this.inventorySize);
      if (!this.IsSelectingBlockTexture)
        return;
      this.selectedInventory = new Inventory(16);
    }

    public override void LoadContent()
    {
      if (GraphicStatics.TexturePack == null)
        GraphicStatics.LoadTexturePack((MapTM) null, Globals2.GameProperties.SaveGame.Header.TexturePack, false, false);
      int rectWidth = 495;
      int rectHeight = 49 + 46 * (this.pagesize / 10 - 1) + 68;
      if (this.IsSelectingBlockTexture)
        rectHeight += 49 + 46 * (((int) this.selectedInventory.PackSize + 9) / 10 - 1) + 18;
      this.screenRect = MyExtensions.CenterOfViewport(rectWidth, rectHeight);
      base.LoadContent();
      this.AddBlocksToInventory();
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchText = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.Font = this.ScreenManager.GameFont;
      this.lockedTexture = this.content.Load<Texture2D>("Textures\\smalllocked");
      this.triggerRect = new Rectangle(0, 0, 12, 24);
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchText);
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
      if (Globals2.AutoStartMap >= 0)
        return;
      this.SelectItem();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.photoLoader == null)
        return;
      this.photoLoader.End(0);
    }

    private bool OnPhotoLoaded(int photoID, Texture2D texture, bool backwards)
    {
      if (texture != null)
      {
        lock (this.photos)
          this.photos.Add(new PhotoTag()
          {
            PhotoID = photoID,
            Texture = texture
          });
        this.AddToInventory((Item) photoID);
      }
      return true;
    }

    private bool ShouldLoadPhoto(int photoID)
    {
      for (int textureIndex = 1; textureIndex < 16; ++textureIndex)
      {
        Block blockTextureId = this.instance.Map.GetBlockTextureID(Block.Painting, textureIndex);
        if ((Block) photoID == blockTextureId)
          return false;
      }
      return true;
    }

    private void AddBlocksToInventory()
    {
      if (this.mode == BlockSelectMode.SelectingPhoto)
      {
        this.photos = new List<PhotoTag>();
        if (this.photoCount == 0 && this.instance.IsHost)
        {
          this.photoLoader = new PhotoLoader();
          this.photos.Add(new PhotoTag());
          this.photoCount = Globals2.GetPhotoCount();
          this.photoLoader.Start(1, false, PhotoFileType.HDThumbnail, new PhotoLoaded(this.OnPhotoLoaded), (Action) null, new ShouldLoadPhoto(this.ShouldLoadPhoto));
        }
        else
        {
          lock (this.photos)
          {
            foreach (PhotoTag photo in this.photos)
              this.AddToInventory((Item) photo.PhotoID);
          }
        }
      }
      if (this.IsSelectingBlockTexture)
      {
        if (this.selectedInventory.Count > 0)
          this.selectedInventory.ClearItems();
        int blockTextureIndex = this.instance.Map.GetBlockTextureIndex(this.playerBlockID);
        if (blockTextureIndex >= 0)
        {
          for (int index = 0; index < 16; ++index)
          {
            Item blockTexture = (Item) this.instance.Map.BlockTextures[blockTextureIndex, index];
            if (blockTexture != Item.None || index == 0 && this.mode == BlockSelectMode.SelectingKey)
            {
            if (this.mode == BlockSelectMode.SelectingKey)
                blockTexture += 330; //Item.SkeletonKey;
              this.AddToInventoryCore(this.selectedInventory, blockTexture, index);
            }
          }
        }
        if (!this.instance.Map.HasFreeBlockTextureSlot(this.playerBlockID))
          return;
      }
      if (!this.CanSelectNewOrRemoveExisting || this.mode == BlockSelectMode.SelectingPhoto || this.mode == BlockSelectMode.SelectingCustomBlock)
        return;
      if (this.mode == BlockSelectMode.SelectingKey)
      {
        foreach (Item key in this.instance.KeyList)
        {
          if (key != Item.SkeletonKey)
            this.AddToInventory(key);
        }
      }
      else if (this.mode == BlockSelectMode.SelectingDecal)
      {
        for (int index = 1; index < MapTM.DecalNames.Length; ++index)
          this.AddToInventory((Item) index);
      }
      else if (this.mode == BlockSelectMode.SelectingChannel)
      {
        this.AddToInventory(Item.ColorRed);
        this.AddToInventory(Item.ColorBlack);
        this.AddToInventory(Item.ColorWhite);
        this.AddToInventory(Item.ColorDarkGray);
        this.AddToInventory(Item.ColorGray);
        this.AddToInventory(Item.ColorDarkBlue);
        this.AddToInventory(Item.ColorBlue);
        this.AddToInventory(Item.ColorLightBlue);
        this.AddToInventory(Item.ColorCyan);
        this.AddToInventory(Item.ColorDarkGreen);
        this.AddToInventory(Item.ColorGreen);
        this.AddToInventory(Item.ColorLightGreen);
        this.AddToInventory(Item.ColorYellow);
        this.AddToInventory(Item.ColorLightYellow);
        this.AddToInventory(Item.ColorDarkRed);
        this.AddToInventory(Item.ColorOrange);
        this.AddToInventory(Item.ColorLightOrange);
        this.AddToInventory(Item.ColorDarkBrown);
        this.AddToInventory(Item.ColorBrown);
        this.AddToInventory(Item.ColorLightBrown);
        this.AddToInventory(Item.ColorTan);
        this.AddToInventory(Item.ColorCreme);
        this.AddToInventory(Item.ColorPurple);
        this.AddToInventory(Item.ColorPink);
      }
      else if (this.mode == BlockSelectMode.SelectingBlockTextureForMultTextureBlock)
      {
        this.AddToInventory(Item.Rope);
        this.AddToInventory(Item.HalfBlock);
        this.AddToInventory(Item.HalfBlock2);
        this.AddToInventory(Item.CornerBlock);
        this.AddToInventory(Item.CornerBlock2);
        this.AddToInventory(Item.Ramp);
        this.AddToInventory(Item.Ramp2);
        this.AddToInventory(Item.Stairs);
        this.AddToInventory(Item.Stairs2);
        this.AddToInventory(Item.Sign);
        this.AddToInventory(Item.Fence);
        this.AddToInventory(Item.Table);
        this.AddToInventory(Item.Cylinder);
        this.AddToInventory(Item.Post);
        this.AddToInventory(Item.Post2);
        this.AddToInventory(Item.SidePost);
        this.AddToInventory(Item.SidePost2);
        this.AddToInventory(Item.Stack);
        this.AddToInventory(Item.Stack2);
        this.AddToInventory(Item.UpsideDownStack);
        this.AddToInventory(Item.Pane);
        this.AddToInventory(Item.PressurePlate);
        this.AddToInventory(Item.Crop);
        this.AddToInventory(Item.Grass);
        this.AddToInventory(Item.Bedrock);
        this.AddToInventory(Item.TilledEarth);
        this.AddToInventory(Item.Painting);
        this.AddToInventory(Item.Crate);
        this.AddToInventory(Item.Chest);
        this.AddToInventory(Item.LockedChest);
        this.AddToInventory(Item.Safe);
        this.AddToInventory(Item.Workbench);
        this.AddToInventory(Item.Furnace);
        this.AddToInventory(Item.ParticleEmitter);
        this.AddToInventory(Item.HealthBlock);
        this.AddToInventory(Item.SentryTurret);
        this.AddToInventory(Item.ProximityDetector);
        this.AddToInventory(Item.NPCSpawn);
        this.AddToInventory(Item.SpiderEgg);
        this.AddToInventory(Item.Teflon);
        this.AddToInventory(Item.Obsidian);
        this.AddToInventory(Item.OneWayGlass);
        this.AddToInventory(Item.StainedGlass);
        this.AddToInventory(Item.StainedGlassPane);
        this.AddToInventory(Item.Console);
        this.AddToInventory(Item.TechLamp);
        this.AddToInventory(Item.TechFurnace);
        this.AddToInventory(Item.PlasmaConduit);
        this.AddToInventory(Item.RaresChest);
        this.AddToInventory(Item.ArcadeMachine);
        this.AddToInventory(Item.AmbientSoundBlock);
        this.AddToInventory(Item.TNT);
        this.AddToInventory(Item.C4);
        this.AddToInventory(Item.Marker);
        this.AddToInventory(Item.ExcludeMarker);
      }
      else if (this.mode == BlockSelectMode.SelectingBlockForReplaceTexture)
      {
        this.AddToInventory(Item.MultiTextureBlock);
        this.AddToInventory(Item.MultiTextureBlock2);
        for (int index = 1; index < (int) byte.MaxValue; ++index)
        {
          if (this.instance.Map.UsesBlockTextureTable((Block) index))
          {
            Item itemId = ItemData.ConvertBlockIDToItemID((Item) index);
            Item obj = itemId;
            if ((uint) obj <= 132U)
            {
              if (obj == Item.Obsidian || obj == Item.MultiTextureBlock || obj == Item.LockedChest)
                continue;
            }
            else if (obj == Item.Painting || obj == Item.MultiTextureBlock2 || obj == Item.LockedDoor)
              continue;
            this.AddToInventory(itemId);
          }
        }
      }
      else
      {
        if (this.mode == BlockSelectMode.SelectingUsedBlockTexture)
          return;
        if (this.mode == BlockSelectMode.CreativeReplace)
          this.AddToInventory(Item.Marker);
        else if (this.mode == BlockSelectMode.SelectingGround)
        {
          this.AddToInventoryNoCheck(this.inventory, Item.NaturalWorld);
          this.AddToInventoryNoCheck(this.inventory, Item.SkyWorld);
          this.AddToInventoryNoCheck(this.inventory, Item.SpaceWorld);
        }
        if (this.mode == BlockSelectMode.CreativeClear)
          this.AddToInventory(Item.zLastBlockID);
        this.AddToInventory(Item.Grass);
        this.AddToInventory(Item.GrassShaded);
        this.AddToInventory(Item.GrassyStone);
        this.AddToInventory(Item.Dirt);
        this.AddToInventory(Item.TilledEarth);
        this.AddToInventory(Item.Sand);
        this.AddToInventory(Item.Scoria);
        this.AddToInventory(Item.Wood);
        this.AddToInventory(Item.WoodPlank);
        this.AddToInventory(Item.BirchWood);
        this.AddToInventory(Item.BirchWoodPlank);
        this.AddToInventory(Item.Leaves);
        this.AddToInventory(Item.PineLeaves);
        this.AddToInventory(Item.MapleLeaves);
        this.AddToInventory(Item.WovenLeaves);
        if (this.mode == BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.Crop);
        if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
          this.AddToInventory(Item.ClimbingIvy);
        this.AddToInventory(Item.Clay);
        this.AddToInventory(Item.Sandstone);
        this.AddToInventory(Item.Limestone);
        this.AddToInventory(Item.Basalt);
        this.AddToInventory(Item.Andesite);
        this.AddToInventory(Item.Dacite);
        this.AddToInventory(Item.Diorite);
        this.AddToInventory(Item.Tuff);
        this.AddToInventory(Item.Serpentine);
        this.AddToInventory(Item.Gabbro);
        this.AddToInventory(Item.Granite);
        this.AddToInventory(Item.Komatiite);
        this.AddToInventory(Item.Marble);
        this.AddToInventory(Item.Rhyolite);
        if (this.mode == BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.Bedrock);
        this.AddToInventory(Item.Carbon);
        if (this.mode != BlockSelectMode.SelectingGround && this.mode != BlockSelectMode.CreativeFill || this.IsGod)
          this.AddToInventory(Item.Obsidian);
        this.AddToInventory(Item.Coal);
        this.AddToInventory(Item.Flint);
        this.AddToInventory(Item.Copper);
        this.AddToInventory(Item.Cassiterite);
        this.AddToInventory(Item.Iron);
        this.AddToInventory(Item.Gold);
        this.AddToInventory(Item.Sapphire);
        this.AddToInventory(Item.Diamond);
        this.AddToInventory(Item.Ruby);
        this.AddToInventory(Item.Platinum);
        this.AddToInventory(Item.Opal);
        this.AddToInventory(Item.Greenstone);
        this.AddToInventory(Item.SaltBlock);
        this.AddToInventory(Item.Sulphur);
        this.AddToInventory(Item.Cyclonite);
        this.AddToInventory(Item.Fluorite);
        this.AddToInventory(Item.Titanium);
        this.AddToInventory(Item.Uranium);
        if (this.mode != BlockSelectMode.SelectingGround)
        {
          this.AddToInventory(Item.Workbench);
          this.AddToInventory(Item.Furnace);
          this.AddToInventory(Item.Crate);
          this.AddToInventory(Item.Chest);
          this.AddToInventory(Item.LockedChest);
          this.AddToInventory(Item.Safe);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.Ladder);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.Torch);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.TrapDoor);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.StairsIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Stairs);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.Stairs2Icon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Stairs2);
          this.AddToInventory(this.mode == BlockSelectMode.SelectingBlockTexture ? Item.HalfBlock : Item.HalfBlockIcon);
          this.AddToInventory(this.mode == BlockSelectMode.SelectingBlockTexture ? Item.HalfBlock2 : Item.HalfBlock2Icon);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.CornerBlock);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.CornerBlock2);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.RampIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Ramp);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.Ramp2Icon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Ramp2);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.CylinderIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Cylinder);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Post);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Post2);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.SidePost);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.SidePost2);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Stack);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Stack2);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.UpsideDownStack);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.SignIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Sign);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.FenceIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Fence);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.TableIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Table);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
            this.AddToInventory(Item.RopeIcon);
          else if (this.mode == BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.Rope);
          this.AddToInventory(Item.BlockShop);
          this.AddToInventory(Item.ItemShop);
          this.AddToInventory(Item.Pane);
        }
        this.AddToInventory(Item.Glass);
        if (this.mode != BlockSelectMode.SelectingGround)
        {
          this.AddToInventory(Item.OneWayGlass);
          this.AddToInventory(Item.StainedGlass);
          this.AddToInventory(Item.StainedGlassPane);
          if (this.mode == BlockSelectMode.CreativeFill || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.SelectingBlockTexture && (this.playerBlockID == Block.NPCSpawn || this.playerBlockID == Block.ParticleEmitter))
            this.AddToInventory(Item.InvisibleBarrier);
        }
        this.AddToInventory(Item.SteelPortcullis);
        this.AddToInventory(Item.Scaffold);
        this.AddToInventory(Item.MultiTextureBlock);
        this.AddToInventory(Item.MultiTextureBlock2);
        if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
        {
          this.AddToInventory(Item.ScriptBlock);
          this.AddToInventory(Item.AmbientSoundBlock);
        }
        this.AddToInventory(Item.PoweredLight);
        this.AddToInventory(Item.SunBox);
        if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.PressurePlate);
        if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
        {
          this.AddToInventory(Item.SwitchIcon);
          this.AddToInventory(Item.ButtonIcon);
          this.AddToInventory(Item.WifiTransmitter);
          this.AddToInventory(Item.WifiReceiver);
        }
        if (this.mode != BlockSelectMode.SelectingGround)
        {
          this.AddToInventory(Item.TNT);
          this.AddToInventory(Item.C4);
          if (this.mode != BlockSelectMode.SelectingBlockTexture)
            this.AddToInventory(Item.SteelSpikes);
          if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear)
          {
            this.AddToInventory(Item.SentryTurret);
            this.AddToInventory(Item.ProximityDetector);
            this.AddToInventory(Item.ParticleEmitter);
            this.AddToInventory(Item.NPCSpawn);
            this.AddToInventory(Item.SpiderEgg);
            this.AddToInventory(Item.Book);
            this.AddToInventory(Item.Fire);
            this.AddToInventory(Item.Painting);
          }
          this.AddToInventory(Item.Bookcase);
        }
        this.AddToInventory(Item.GoldBlock);
        this.AddToInventory(Item.WoodVeneer);
        this.AddToInventory(Item.GildedWoodPanel);
        if (this.mode == BlockSelectMode.CreativeReplace || this.mode == BlockSelectMode.CreativeClear || this.mode == BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.Teflon);
        this.AddToInventory(Item.Ice);
        this.AddToInventory(Item.Bricks);
        this.AddToInventory(Item.SandBrick);
        this.AddToInventory(Item.ConcreteBrick);
        this.AddToInventory(Item.StoneBrick);
        this.AddToInventory(Item.StoneWall);
        this.AddToInventory(Item.Cobblestone);
        this.AddToInventory(Item.MossyCobblestone);
        this.AddToInventory(Item.TerracottaTile);
        this.AddToInventory(Item.Rasta);
        this.AddToInventory(Item.WhiteTile);
        this.AddToInventory(Item.Checkered);
        this.AddToInventory(Item.Retro);
        this.AddToInventory(Item.SteelPlating);
        this.AddToInventory(Item.CherryMetal);
        this.AddToInventory(Item.Static);
        this.AddToInventory(Item.Chess);
        this.AddToInventory(Item.Turkish);
        this.AddToInventory(Item.Camouflage);
        this.AddToInventory(Item.BlueBox);
        this.AddToInventory(Item.Console);
        this.AddToInventory(Item.TechPanel);
        this.AddToInventory(Item.TechLamp);
        this.AddToInventory(Item.TechFurnace);
        this.AddToInventory(Item.PlasmaConduit);
        this.AddToInventory(Item.SteelVent);
        if (this.mode != BlockSelectMode.SelectingGround && this.mode != BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.ArcadeMachine);
        if (this.mode != BlockSelectMode.SelectingGround && this.mode != BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.Teleport);
        this.AddToInventory(Item.Snow);
        if (this.mode != BlockSelectMode.SelectingGround)
          this.AddToInventory(Item.SnowLayer);
        if (this.mode != BlockSelectMode.SelectingGround && this.mode != BlockSelectMode.SelectingBlockTexture)
          this.AddToInventory(Item.Cloud);
        this.AddToInventory(Item.WhiteWool);
        this.AddToInventory(Item.Bone);
        this.AddToInventory(Item.Cactus);
        if (this.mode != BlockSelectMode.SelectingGround && this.mode != BlockSelectMode.SelectingBlockTexture)
        {
          this.AddToInventory(Item.BerryBush);
          this.AddToInventory(Item.LongGrass);
          this.AddToInventory(Item.WhiteFlowers);
          this.AddToInventory(Item.YellowFlowers);
          this.AddToInventory(Item.RedFlowers);
          this.AddToInventory(Item.PurpleFlowers);
          this.AddToInventory(Item.Sapling);
          this.AddToInventory(Item.RedMushroom);
        }
        if (this.mode != BlockSelectMode.SelectingBlockTexture)
        {
          this.AddToInventory(Item.Water);
          this.AddToInventory(Item.Lava);
        }
        this.AddToInventory(Item.PolishedBasalt);
        this.AddToInventory(Item.PolishedBasaltBrick);
        this.AddToInventory(Item.PolishedAndesite);
        this.AddToInventory(Item.PolishedAndesiteBrick);
        this.AddToInventory(Item.PolishedDacite);
        this.AddToInventory(Item.PolishedDaciteBrick);
        this.AddToInventory(Item.PolishedDiorite);
        this.AddToInventory(Item.PolishedDioriteBrick);
        this.AddToInventory(Item.PolishedTuff);
        this.AddToInventory(Item.PolishedTuffBrick);
        this.AddToInventory(Item.PolishedSerpentine);
        this.AddToInventory(Item.PolishedSerpentineBrick);
        this.AddToInventory(Item.PolishedGabbro);
        this.AddToInventory(Item.PolishedGabbroBrick);
        this.AddToInventory(Item.PolishedGranite);
        this.AddToInventory(Item.PolishedGraniteBrick);
        this.AddToInventory(Item.PolishedKomatiite);
        this.AddToInventory(Item.PolishedKomatiiteBrick);
        this.AddToInventory(Item.PolishedMarble);
        this.AddToInventory(Item.PolishedMarbleBrick);
        this.AddToInventory(Item.PolishedRhyolite);
        this.AddToInventory(Item.PolishedRhyoliteBrick);
        this.AddToInventory(Item.PolishedGreenstone);
        this.AddToInventory(Item.PolishedGreenstoneBrick);
        this.AddToInventory(Item.ColorBlack);
        this.AddToInventory(Item.ColorWhite);
        this.AddToInventory(Item.ColorDarkGray);
        this.AddToInventory(Item.ColorGray);
        this.AddToInventory(Item.ColorSmoothGray);
        this.AddToInventory(Item.ColorDarkBlue);
        this.AddToInventory(Item.ColorBlue);
        this.AddToInventory(Item.ColorLightBlue);
        this.AddToInventory(Item.ColorCyan);
        this.AddToInventory(Item.ColorDarkGreen);
        this.AddToInventory(Item.ColorGreen);
        this.AddToInventory(Item.ColorLightGreen);
        this.AddToInventory(Item.ColorLime);
        this.AddToInventory(Item.ColorYellow);
        this.AddToInventory(Item.ColorLightYellow);
        this.AddToInventory(Item.ColorDarkRed);
        this.AddToInventory(Item.ColorRed);
        this.AddToInventory(Item.ColorBloodOrange);
        this.AddToInventory(Item.ColorOrange);
        this.AddToInventory(Item.ColorLightOrange);
        this.AddToInventory(Item.ColorDarkBrown);
        this.AddToInventory(Item.ColorBrown);
        this.AddToInventory(Item.ColorLightBrown);
        this.AddToInventory(Item.ColorLeather);
        this.AddToInventory(Item.ColorTan);
        this.AddToInventory(Item.ColorLightTan);
        this.AddToInventory(Item.ColorCreme);
        this.AddToInventory(Item.ColorPurple);
        this.AddToInventory(Item.ColorPink);
        if (!this.IsGod)
          return;
        foreach (Block block in Utils.GetValues<Block>())
        {
          if (!this.inventory.HasItem((Item) block))
            this.AddToInventory((Item) block);
        }
        this.inventory[this.inventory.Count - 1] = InventoryItem.Empty;
      }
    }

    private void AddToInventory(Item itemID)
    {
      ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID];
      if ((!itemDataXml.IsValid || !itemDataXml.IsEnabled) && (this.player == null || !this.player.IsGod) || this.instance != null && this.instance.Map.HasBlockTexture(this.playerBlockID, itemID))
        return;
      this.AddToInventoryNoCheck(this.inventory, itemID);
    }

    private void AddToInventoryNoCheck(Inventory inventory, Item itemID)
    {
      InventoryItem inventoryItem = new InventoryItem(itemID, 1);
      int getFreeSlotForItem = inventory.FindOrGetFreeSlotForItem(inventoryItem);
      if (getFreeSlotForItem < 0)
        return;
      this.AddToInventoryCore(inventory, itemID, getFreeSlotForItem);
    }

    private void AddToInventoryCore(Inventory inventory, Item itemID, int index)
    {
      InventoryItem inventoryItem = inventory[index];
      inventoryItem.ItemID = itemID;
      ++inventoryItem.Count;
      inventory[index] = inventoryItem;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageUp) || InputManager.GetMouseWheelDelta(this.ControllingPlayer) < 0)
      {
        this.PrevPageButtonPressed();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PageDown) || InputManager.GetMouseWheelDelta(this.ControllingPlayer) > 0)
      {
        this.NextPageButtonPressed();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
      {
        this.SelectItem();
        return true;
      }
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.TransferItem) && this.CanRemoveItem && this.CanSelectNewOrRemoveExisting)
      {
        this.RemoveItem();
        return true;
      }
      if (InputManager.IsMouseMoved(this.ControllingPlayer.Value))
      {
        this.SetCurrentSlot(InputManager.GetMousePos(this.ControllingPlayer));
        return true;
      }
      int num1 = 10;
      int num2 = 15;
      if (currentGamePadState.ThumbSticks.Left != Vector2.Zero || currentGamePadState.ThumbSticks.Right != Vector2.Zero)
      {
        if (this.EitherStickMoved(currentGamePadState, lastGamePadState))
          this.thumbstickTimer = num2 + 1;
        else
          ++this.thumbstickTimer;
      }
      else
        this.thumbstickTimer = 0;
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorLeft) || (double) currentGamePadState.ThumbSticks.Left.X < 0.0 && this.thumbstickTimer > num1 || (double) currentGamePadState.ThumbSticks.Right.X < 0.0 && this.thumbstickTimer > num1)
        this.MoveLeft();
      else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorRight) || (double) currentGamePadState.ThumbSticks.Left.X > 0.0 && this.thumbstickTimer > num1 || (double) currentGamePadState.ThumbSticks.Right.X > 0.0 && this.thumbstickTimer > num1)
        this.MoveRight();
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown) || (double) currentGamePadState.ThumbSticks.Left.Y < 0.0 && this.thumbstickTimer > num2 || (double) currentGamePadState.ThumbSticks.Right.Y < 0.0 && this.thumbstickTimer > num2)
        this.MoveDown();
      else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp) || (double) currentGamePadState.ThumbSticks.Left.Y > 0.0 && this.thumbstickTimer > num2 || (double) currentGamePadState.ThumbSticks.Right.Y > 0.0 && this.thumbstickTimer > num2)
        this.MoveUp();
      if (this.currentSlot != this.oldSlot)
        this.CursorMoved();
      return base.HandleInput(input);
    }

    protected void SetCurrentSlot(Point pos)
    {
      int num1 = this.pagesize / 10;
      if (this.IsSelectingBlockTexture)
        num1 += ((int) this.selectedInventory.PackSize + 9) / 10;
      int num2 = 32;
      Rectangle rectangle = new Rectangle(0, 0, 46, 46);
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          rectangle.X = index2 * 46 + this.screenRect.X + 16;
          rectangle.Y = this.screenRect.Y + this.screenRect.Height - num2 - (index1 + 1) * 46 - 3;
          int num3 = index2 + index1 * 10;
          if (num3 > 19)
            rectangle.Y -= 16;
          if (rectangle.Contains(pos))
          {
            this.currentSlot = num3;
            this.CursorMoved();
            return;
          }
        }
      }
    }

    private bool EitherStickMoved(GamePadState pad, GamePadState lastpad)
    {
      if (Math.Sign(pad.ThumbSticks.Left.X) == Math.Sign(lastpad.ThumbSticks.Left.X) && Math.Sign(pad.ThumbSticks.Left.Y) == Math.Sign(lastpad.ThumbSticks.Left.Y) && Math.Sign(pad.ThumbSticks.Right.X) == Math.Sign(lastpad.ThumbSticks.Right.X))
        return Math.Sign(pad.ThumbSticks.Right.Y) != Math.Sign(lastpad.ThumbSticks.Right.Y);
      return true;
    }

    private void CursorMoved()
    {
      this.ValidateCurrentSlot();
      this.itemTypeName = this.GetBlockDesc(this.mode, this.CursorItem.ItemID);
      this.oldSlot = this.currentSlot;
    }

    private string GetBlockDesc(BlockSelectMode mode, Item itemID)
    {
      switch (mode)
      {
        case BlockSelectMode.SelectingStainedGlass:
          return "Stained Glass";
        case BlockSelectMode.SelectingArcadeGame:
          return this.instance.GetArcadeMachineName((int) itemID);
        case BlockSelectMode.SelectingDecal:
          int index = (int) itemID;
          if (index >= MapTM.DecalNames.Length)
            return "None";
          return MapTM.DecalNames[index];
        case BlockSelectMode.SelectingCustomBlock:
          return this.instance.Map.CustomBlockModels[(int) (this.playerBlockID - (byte) 81), (int) itemID]?.ComName ?? "";
        default:
          if (this.playerBlockID == Block.CoverBlock && this.currentSlot < MapTM.CoverBlockTop.Length)
            return MapTM.CoverBlockTop[this.currentSlot].ToString();
          if (itemID == Item.None || itemID >= (Item) Globals1.ItemData.Length)
            return "";
          return ItemData2.ForDisplay((GameInstance) null, itemID);
      }
    }

    private void SelectItem()
    {
      if (this.mode == BlockSelectMode.SelectingChannel && !this.player.IsAdmin)
      {
        if (this.CursorItem.ItemID == Item.ColorRed)
          return;
        if (this.GetItem(this.initialSlot).ItemID == Item.ColorRed)
        {
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This Teleport is set to the Admin channel.\nOnly Admins can change that.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 1f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
          return;
        }
      }
      if (this.mode != BlockSelectMode.SelectingChannel && this.mode != BlockSelectMode.SelectingPhoto && (this.mode != BlockSelectMode.SelectingCustomBlock && this.instance != null) && (!this.instance.IsItemUnlocked(this.CursorItem.ItemID) && (this.player == null || !this.player.IsGodOrTester)) || this.currentSlot >= 20 && (this.mode == BlockSelectMode.SelectingUsedBlockTexture || this.mode == BlockSelectMode.SelectingArcadeGame || this.mode == BlockSelectMode.SelectingStainedGlass))
        return;
      if (this.blockCallback != null)
      {
        if (!this.blockCallback(this.player, (Block) ItemData.ConvertItemIDToBlockID(this.CursorItem.ItemID)))
          return;
        this.ExitScreen();
      }
      else
      {
        if (this.itemCallback == null || !this.itemCallback(this.player, this.CursorItem.ItemID, this.currentSlot, this.GetTagData(this.CursorItem.ItemID, this.currentSlot)))
          return;
        this.ExitScreen();
      }
    }

    private object GetTagData(Item itemID, int slotID)
    {
      if (this.mode == BlockSelectMode.SelectingPhoto)
      {
        foreach (PhotoTag photo in this.photos)
        {
          if ((Item) photo.PhotoID == itemID)
            return (object) photo;
        }
      }
      return (object) null;
    }

    private void MoveLeft()
    {
      if (this.currentSlot % 10 == 0)
        this.currentSlot += 9;
      else
        --this.currentSlot;
      int packSize = (int) this.GetInventory(this.currentSlot).PackSize;
      int fullSlotId = this.GetFullSlotID(this.currentSlot);
      if (fullSlotId >= packSize)
        this.currentSlot -= fullSlotId - (packSize - 1);
      this.thumbstickTimer = 0;
    }

    private void MoveRight()
    {
      if (this.currentSlot % 10 == 9)
        this.currentSlot -= 9;
      else
        ++this.currentSlot;
      int packSize = (int) this.GetInventory(this.currentSlot).PackSize;
      int fullSlotId = this.GetFullSlotID(this.currentSlot);
      if (fullSlotId >= packSize)
        this.currentSlot -= fullSlotId - (packSize - packSize % 10);
      this.thumbstickTimer = 0;
    }

    private void MoveUp()
    {
      if (this.IsSelectingBlockTexture)
      {
        if (this.currentSlot < 6 || this.currentSlot > 9)
          this.currentSlot += 10;
        else
          this.currentSlot += 20;
        if (this.currentSlot >= 20 && this.currentSlot - 20 >= this.pagesize)
          this.currentSlot -= this.pagesize + 20;
        int fullSlotId = this.GetFullSlotID(this.currentSlot);
        if (this.currentSlot >= 20 && fullSlotId >= this.inventorySize)
          this.currentSlot = 20 + fullSlotId % 10;
      }
      else
      {
        this.currentSlot += 10;
        if (this.currentSlot >= this.pagesize)
          this.currentSlot -= this.pagesize;
        int fullSlotId = this.GetFullSlotID(this.currentSlot);
        if (fullSlotId >= this.inventorySize)
          this.currentSlot = fullSlotId % 10;
      }
      this.thumbstickTimer = 0;
    }

    private void MoveDown()
    {
      if (this.IsSelectingBlockTexture)
      {
        if (this.currentSlot < 26 || this.currentSlot > 29)
          this.currentSlot -= 10;
        else
          this.currentSlot -= 20;
        if (this.currentSlot < 0)
          this.currentSlot += this.pagesize + 20;
        this.ValidateCurrentSlot();
      }
      else if (this.currentSlot > 9)
      {
        this.currentSlot -= 10;
      }
      else
      {
        this.currentSlot += this.pagesize - 10;
        this.ValidateCurrentSlot();
      }
      this.thumbstickTimer = 0;
    }

    private void ValidateCurrentSlot()
    {
      int fullSlotId = this.GetFullSlotID(this.currentSlot);
      bool flag = this.IsSelectingBlockTexture && this.currentSlot >= 20;
      if (fullSlotId >= this.inventorySize)
      {
        this.currentSlot = this.inventorySize % this.pagesize - (this.inventorySize % 10 - this.currentSlot % 10);
        if (flag)
          this.currentSlot += 20;
      }
      while (this.GetFullSlotID(this.currentSlot) >= this.inventorySize)
        this.currentSlot -= 10;
    }

    private void PrevPageButtonPressed()
    {
      if (--this.page < 0)
        this.page = this.PageCount - 1;
      this.pageString = (this.page + 1).ToString();
      this.CursorMoved();
    }

    private void NextPageButtonPressed()
    {
      if (++this.page >= this.PageCount)
        this.page = 0;
      this.pageString = (this.page + 1).ToString();
      this.CursorMoved();
    }

    private void RemoveItem()
    {
      this.CursorItem = Inventory.EmptyItem;
      this.instance.Map.SetBlockTexture(this.playerBlockID, this.currentSlot, Block.None);
      if (this.mode == BlockSelectMode.SelectingChannel || this.mode == BlockSelectMode.SelectingPhoto || this.instance == null || (this.instance.IsItemUnlocked(this.CursorItem.ItemID) || this.player != null && this.player.IsGodOrTester))
      {
        if (this.blockCallback != null)
        {
          int num1 = this.blockCallback(this.player, (Block) this.CursorItem.ItemID) ? 1 : 0;
        }
        else if (this.itemCallback != null)
        {
          int num2 = this.itemCallback(this.player, this.CursorItem.ItemID, this.currentSlot, (object) null) ? 1 : 0;
        }
      }
      this.instance.NetworkManager.SendBlockTextureRemoved(this.playerBlockID, this.currentSlot);
      this.inventory.ClearItems();
      this.AddBlocksToInventory();
    }

    protected override void DrawCore()
    {
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatchText.BeginTM(this.Matrix);
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, this.Matrix);
      int num = 7;
      this.spriteBatch.DrawString(this.Font, this.selectDesc, new Vector2((float) (this.screenRect.X + 14), (float) (this.screenRect.Y + num)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (this.PageCount > 1)
      {
        this.spriteBatch.DrawString(this.Font, "Page " + this.pageString, new Vector2((float) (this.screenRect.X + this.screenRect.Width - 154), (float) (this.screenRect.Y + num)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.spriteBatch.DrawString(this.Font, "Flip", new Vector2((float) (this.screenRect.X + this.screenRect.Width - 50), (float) (this.screenRect.Y + num)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.triggerRect.X = this.screenRect.X + this.screenRect.Width - 70;
        this.triggerRect.Y = this.screenRect.Y + num - 2;
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.PageUp, this.triggerRect);
      }
      if (this.IsSelectingBlockTexture)
      {
        this.DrawGrid(((int) this.selectedInventory.PackSize + 9) / 10, -8, this.selectedInventory, 0, 0);
        this.DrawGrid(this.pagesize / 10, 102, this.inventory, 0, this.page * this.pagesize);
      }
      else
        this.DrawGrid(this.pagesize / 10, -8, this.inventory, 0, this.page * this.pagesize);
      if (this.CanRemoveItem && this.CanSelectNewOrRemoveExisting)
      {
        Rectangle destinationRectangle = new Rectangle(this.screenRect.X + this.screenRect.Width - 120, this.screenRect.Y + this.screenRect.Height - 28, 24, 24);
        this.spriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
        this.spriteBatch.DrawString(this.Font, "Remove", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      this.DrawCursor();
      if (this.itemTypeName != null && this.itemTypeName.Length > 0 && this.mode != BlockSelectMode.SelectingPhoto)
        this.spriteBatch.DrawString(this.Font, this.ItemDescriptionPanelText, new Vector2((float) (this.screenRect.X + 16), (float) (this.screenRect.Y + this.screenRect.Height - 26)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
    }

    protected virtual string ItemDescriptionPanelText
    {
      get
      {
        if (this.mode == BlockSelectMode.SelectingChannel)
        {
          if (this.itemTypeName == "Red")
            this.itemTypeName = "Admin";
          else if (this.itemTypeName == "Obsidian")
            this.itemTypeName = "Open";
          return Utils.InsertSpacesBeforeCapitals(this.itemTypeName) + " Channel";
        }
        if (this.mode == BlockSelectMode.CreativeClear)
        {
          if (this.itemTypeName == "zLast Block ID")
            this.itemTypeName = "All Blocks";
        }
        else if (this.mode == BlockSelectMode.SelectingCustomBlock)
          return this.itemTypeName;
        return Utils.InsertSpacesBeforeCapitals(this.itemTypeName);
      }
    }

    protected void DrawGrid(
      int height,
      int yoffset,
      Inventory inventory,
      int itemSlotOffset,
      int pageOffset)
    {
      Rectangle slotRect = new Rectangle(0, 0, 49, 49);
      for (int index1 = 0; index1 < height; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          int slotID = index2 + index1 * 10 + itemSlotOffset + pageOffset;
          if (slotID < (int) inventory.PackSize)
          {
            slotRect.X = index2 * 46 + this.screenRect.X + 16;
            slotRect.Y = this.screenRect.Y + this.screenRect.Height - yoffset - (index1 + 1) * 46 - 45;
            this.DrawSlot(slotRect.X, slotRect.Y);
            InventoryItem inventoryItem = inventory[slotID];
            bool selectedInventory = inventory == this.selectedInventory;
            if (inventoryItem.ItemID != Item.None || selectedInventory && this.mode == BlockSelectMode.SelectingPhoto)
              this.DrawItem(slotRect, slotID, inventoryItem, selectedInventory);
          }
        }
      }
    }

    protected virtual void DrawCursor()
    {
      if (this.currentSlot < 0)
        return;
      GraphicStatics.DrawCursor(this.spriteBatch, this.GetSlotRect(this.currentSlot), Color.Yellow);
    }

    protected void DrawSlot(int x, int y)
    {
      Rectangle rect = new Rectangle(x, y, 49, 49);
      Color color = new Color(0.8f, 0.8f, 0.8f, 1f);
      this.spriteBatch.DrawFilledBox(rect, 3, color, color * 0.25f);
      Rectangle destinationRectangle = new Rectangle();
      destinationRectangle.X = rect.X + 3;
      destinationRectangle.Y = rect.Y + 3;
      destinationRectangle.Width = rect.Width - 6;
      destinationRectangle.Height = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
      destinationRectangle.Height = rect.Height - 6;
      destinationRectangle.Width = 2;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Black);
    }

    protected void DrawItem(
      Rectangle slotRect,
      int slotID,
      InventoryItem item,
      bool selectedInventory)
    {
      slotRect.X += 9;
      slotRect.Y += 9;
      slotRect.Width = slotRect.Height = 32;
      int num = GraphicStatics.TexturePack.BlockTextureSize();
      if (this.mode == BlockSelectMode.SelectingStainedGlass)
      {
        if (item.ItemID == Item.StainedGlass || item.ItemID == Item.StainedGlassPane)
          item.ItemID = Item.None;
        Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect(item.ItemID);
        rectangle.X += num * 16;
        rectangle.Y += num * 9;
        this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect, new Rectangle?(rectangle), Color.White);
      }
      else if (this.mode == BlockSelectMode.SelectingArcadeGame)
      {
        if (!selectedInventory)
          return;
        Item itemID = item.ItemID;
        switch (itemID)
        {
          case Item.None:
          case Item.Grass:
          case Item.Dirt:
            Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect(itemID);
            rectangle.X += num * 10;
            rectangle.Y += num * 9;
            this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect, new Rectangle?(rectangle), Color.White);
            break;
          default:
            itemID = Item.ArcadeMachine;
            num = 0;
            goto case Item.None;
        }
      }
      else if (this.mode == BlockSelectMode.SelectingUsedBlockTexture && this.playerBlockID == Block.CoverBlock && item.ItemID != Item.CoverBlock)
      {
        Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect(item.ItemID);
        int itemId = (int) item.ItemID;
        if (itemId < 3)
        {
          rectangle.X += num * 13;
          rectangle.Y += num * 9;
        }
        else if (itemId < 5)
        {
          rectangle.X += num * 27;
          rectangle.Y += num * 10;
        }
        else
        {
          rectangle.X += num * 8;
          rectangle.Y += num * 9;
        }
        this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect, new Rectangle?(rectangle), Color.White);
      }
      else if (this.mode == BlockSelectMode.SelectingDecal && item.ItemID != Item.None)
      {
        if (item.ItemID == Item.zLastBlockID)
          return;
        Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect(item.ItemID);
        rectangle.X -= num;
        rectangle.Y += num * 11;
        if (item.ItemID > Item.Obsidian)
        {
          rectangle.X += num * 20;
          rectangle.Y -= num * 3;
        }
        else if (item.ItemID > Item.Rhyolite)
        {
          rectangle.X -= num * 12;
          rectangle.Y -= num * 2;
        }
        else if (item.ItemID > Item.Cassiterite)
        {
          rectangle.X -= num * 14;
          rectangle.Y += num;
        }
        this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect, new Rectangle?(rectangle), Color.White);
      }
      else if (this.mode == BlockSelectMode.SelectingCustomBlock)
      {
        if (!selectedInventory)
          return;
        Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect((Item) this.playerBlockID);
        this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, slotRect, new Rectangle?(rectangle), Color.White);
      }
      else if (this.mode == BlockSelectMode.SelectingPhoto)
      {
        if (selectedInventory)
        {
          Block blockTextureId = this.instance.Map.GetBlockTextureID(Block.Painting, slotID);
          if (blockTextureId == Block.None && slotID != 0)
            return;
          this.DrawPhotoFromTexturePack(blockTextureId == Block.None ? 0 : slotID, slotRect);
        }
        else
        {
          if (!this.instance.Map.IsHost)
            return;
          this.DrawPhotoRaw((int) item.ItemID, slotRect);
        }
      }
      else if (!selectedInventory && this.instance != null && this.instance.IsItemLocked(item.ItemID) && (this.player == null || !this.player.IsGodOrTester))
        this.spriteBatchPoint.Draw(this.lockedTexture, slotRect, Color.White);
      else
        this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.GetTexureForItem(item.ItemID), slotRect, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(item.ItemID)), Color.White);
    }

    private void DrawPhotoRaw(int photoID, Rectangle slotRect)
    {
      lock (this.photos)
      {
        for (int index = 1; index < this.photos.Count; ++index)
        {
          if (this.photos[index].PhotoID == photoID)
          {
            this.spriteBatchPoint.Draw(this.photos[index].Texture, slotRect, Color.White);
            break;
          }
        }
      }
    }

    private void DrawPhotoFromTexturePack(int index, Rectangle slotRect)
    {
      if (this.instance.IsRemote)
        this.instance.NetworkManager.SendPhotoThumbnailRequest((byte) index);
      Texture2D blockTexture = GraphicStatics.TexturePack.BlockTexture;
      this.spriteBatchPoint.Draw(blockTexture, slotRect, new Rectangle?(GraphicStatics.PhotoData.GetPhotoBlockDestRect(GraphicStatics.TexturePack, blockTexture, (byte) index)), Color.White);
    }

    protected virtual Rectangle GetSlotRect(int slotID)
    {
      int x = slotID % 10 * 46 + this.screenRect.X + 16;
      int y;
      if (!this.IsSelectingBlockTexture)
      {
        y = this.screenRect.Y + this.screenRect.Height - 37 - (slotID / 10 + 1) * 46;
      }
      else
      {
        y = this.screenRect.Y + this.screenRect.Height - 37 - (slotID / 10 + 1) * 46;
        if (slotID >= 20)
          y -= 18;
      }
      return new Rectangle(x, y, 49, 49);
    }
  }
}
