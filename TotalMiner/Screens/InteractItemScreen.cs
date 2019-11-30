// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.InteractItemScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class InteractItemScreen : MinerToolScreen
  {
    private float textScale = 0.75f;
    private string heading;
    private string[] text;
    private Vector2 textMeasure;
    private Vector2 headingMeasure;
    private InventoryItem item;
    private Item itemID;
    private ItemDataXML itemData;
    private ItemTypeDataXML itemTypeData;
    private SkillDataXML skillData;
    private ItemCombatDataXML combatData;
    private ItemSwingTimeDataXML swingTimeData;
    private GlobalPoint3D? point;
    private GameInstance instance;
    private Color colorWhite;
    private Color colorBlack;
    private Color headingColor;
    private bool canOwn;
    private bool canRead;
    private bool canSetChannel;
    private bool canSetPhoto;
    private bool canSetGame;
    private bool canSetKey;
    private bool canSetDecal;
    private bool canSetComponent;
    private bool canSetup;
    private bool canCustomizeItem;
    private bool canInstance;
    private bool isBlockOwned;
    private bool isBlockInstanced;
    private bool isOwner;
    private string pstr;
    private int powerCount;
    private Texture2D powerTex;
    private int customBlockSlotID;
    private int customBlockDirNum;

    private bool CanPickItem
    {
      get
      {
        if (!this.point.HasValue)
          return false;
        Item itemId = this.itemID;
        if ((uint) itemId <= 29U)
        {
          switch (itemId)
          {
            case Item.Water:
            case Item.Lava:
            case Item.Bedrock:
              break;
            default:
              goto label_6;
          }
        }
        else
        {
          switch (itemId)
          {
            case Item.Wisdom:
            case Item.Blueprint:
            case Item.Crop:
            case Item.Hand:
              break;
            default:
              goto label_6;
          }
        }
        return false;
label_6:
        if (!this.player.HasPermission(Permissions.Edit))
          return false;
        if (!ItemData.IsItemUse(this.itemID, ItemUse.Block))
          return ItemData.IsSubType(this.itemID, ItemSubType.Key);
        return true;
      }
    }

    private bool CanStockUpItem
    {
      get
      {
        if (!this.instance.IsFiniteResources || this.player.IsAdmin && this.instance.IsCreativeMode || this.player.IsGodOrTester)
          return this.CanPickItem;
        return false;
      }
    }

    private bool CanChangeTexture
    {
      get
      {
        Permissions permission = this.item.ItemID == Item.ArcadeMachine ? Permissions.Adventure : (this.item.ItemID < Item.CustomBlock1 || this.item.ItemID > Item.CustomBlock10 ? Permissions.Edit : Permissions.Creative);
        if (!this.instance.IsItemUnlocked(this.item.ItemID) || !this.player.HasPermission(permission))
          return false;
        if (this.canSetDecal)
          return true;
        if (this.instance.Map.UsesBlockTextureTable(this.itemID))
          return this.isOwner;
        return false;
      }
    }

    private bool CanRead
    {
      get
      {
        return this.canRead;
      }
    }

    private bool CanSetup
    {
      get
      {
        return this.canSetup;
      }
    }

    private bool CanCustomizeItem
    {
      get
      {
        return this.canCustomizeItem;
      }
    }

    private bool CanInstance
    {
      get
      {
        return this.canInstance;
      }
    }

    private bool CanOwn
    {
      get
      {
        return this.canOwn;
      }
    }

    private bool HasGeneralStats
    {
      get
      {
        if (this.itemData.Durability > (ushort) 0 && this.itemData.ItemID != Item.Book && (this.itemData.ItemID != Item.Blueprint && this.itemData.ItemID != Item.Wisdom) || this.itemData.HealPower > (short) 0)
          return true;
        if (this.itemTypeData.Class != ItemTypeClass.None)
          return this.itemTypeData.Class != ItemTypeClass.CantMine;
        return false;
      }
    }

    private bool HasMaterial
    {
      get
      {
        return this.itemData.ItemID <= Item.zLastBlockID;
      }
    }

    private bool HasAttackStats
    {
      get
      {
        return (double) this.itemData.StrikeDamage > 0.0;
      }
    }

    private bool HasStatBonuses
    {
      get
      {
        return this.combatData.CombatID != CombatItem.None;
      }
    }

    private bool HasSkillStats
    {
      get
      {
        if (this.skillData.UseSkill == SkillType.None && this.skillData.CraftReq <= 0)
          return this.skillData.MineReq > 0;
        return true;
      }
    }

    public InteractItemScreen(GameInstance instance, Player player, InventoryItem item)
      : this(instance, player, item, new GlobalPoint3D?())
    {
    }

    public InteractItemScreen(
      GameInstance instance,
      Player player,
      InventoryItem item,
      GlobalPoint3D? point)
      : base(player)
    {
      this.instance = instance;
      this.item = item;
      this.itemID = item.ItemID_Raw;
      this.point = point;
      this.itemData = Globals1.ItemData[(int) this.itemID];
      this.itemTypeData = Globals1.ItemTypeData[(int) this.itemID];
      this.skillData = Globals1.SkillData[(int) this.itemID];
      this.combatData = Globals1.ItemCombatData[(int) this.itemTypeData.Combat];
      this.swingTimeData = Globals1.ItemSwingTimeData[(int) this.itemID];
      if (!point.HasValue)
        return;
      this.pstr = string.Format("[{0},{1},{2}]", (object) point.Value.X, (object) point.Value.Y, (object) point.Value.Z);
      ScriptEditScreen.SetClipboard(this.pstr);
      this.isOwner = true;
      MapBlock blockIdAndAux = instance.Map.GetBlockIDAndAux(point.Value);
      byte num = (byte) ((uint) blockIdAndAux.AuxData & 7U);
      this.canSetDecal = player != null && player.HasDecalApplicatorEquipped && instance.Map.BlockData[(int) blockIdAndAux.BlockID].Buffer == (byte) 0 && (int) blockIdAndAux.AuxData >> 4 > 0;
      Item itemId = this.itemID;
      if ((uint) itemId <= 121U)
      {
        if ((uint) itemId <= 65U)
        {
          switch (itemId)
          {
            case Item.Obsidian:
              this.isOwner = this.canSetChannel = instance.MapStrategyTM.TeleportExists(point.Value);
              goto label_29;
            case Item.Chest:
              goto label_29;
            case Item.ItemShop:
            case Item.BlockShop:
              this.isBlockOwned = num == (byte) 1;
              this.isOwner = this.canOwn = !this.isBlockOwned;
              goto label_29;
            default:
              goto label_19;
          }
        }
        else
        {
          switch (itemId)
          {
            case Item.HealthBlock:
            case Item.AmbientSoundBlock:
              break;
            case Item.Book:
              this.canRead = true;
              goto label_29;
            default:
              goto label_19;
          }
        }
      }
      else if ((uint) itemId <= 143U)
      {
        switch (itemId)
        {
          case Item.ArcadeMachine:
            this.canSetGame = player.HasPermission(Permissions.Adventure);
            goto label_29;
          case Item.ParticleEmitter:
          case Item.SentryTurret:
          case Item.ProximityDetector:
            break;
          case Item.NPCSpawn:
            this.canSetup = instance.IsItemUnlocked(Item.NPCSpawn) && player.HasPermission(Permissions.Creative);
            goto label_29;
          default:
            goto label_19;
        }
      }
      else
      {
        switch (itemId)
        {
          case Item.Painting:
            this.canSetPhoto = player.HasPermission(Permissions.Edit);
            goto label_29;
          case Item.WifiTransmitter:
          case Item.WifiReceiver:
            break;
          case Item.ScriptBlock:
            this.canSetup = player.HasPermission(Permissions.Admin);
            goto label_29;
          case Item.Wand:
            this.canCustomizeItem = player.IsAdmin;
            goto label_29;
          default:
            goto label_19;
        }
      }
      this.canSetup = player.HasPermission(Permissions.Creative);
      goto label_29;
label_19:
      if (this.itemID >= Item.CustomBlock1 && this.itemID <= Item.CustomBlock10)
        this.canSetComponent = true;
      else if (ItemData.CanItemUseKey(this.itemID))
      {
        PlayerBlock dataBlock = instance.MapStrategyTM.GetDataBlock(point.Value) as PlayerBlock;
        if (dataBlock == null)
        {
          if (this.itemID == Item.LockedDoor)
          {
            GlobalPoint3D p = point.Value;
            --p.Y;
            dataBlock = instance.MapStrategyTM.GetDataBlock(p) as PlayerBlock;
          }
          if (dataBlock == null)
            dataBlock = (PlayerBlock) (instance.MapStrategyTM.GetDataBlock(point.Value) as ChestBlock);
        }
        this.canSetKey = dataBlock != null && player.HasPermission(Permissions.Edit);
        this.isOwner = this.canSetKey && dataBlock.IsOwner(player);
      }
      else
        this.canSetup = instance.ItemHasCustomSetup(this.itemID, player);
label_29:
      if (this.isOwner && !player.HasPermission(Permissions.Edit))
        this.isOwner = false;
      if (!this.canOwn || player.HasPermission(Permissions.Edit))
        return;
      this.canOwn = false;
    }

    public override void LoadContent()
    {
      this.Font = CoreGlobals.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.powerTex = CoreGlobals.Content.Load<Texture2D>("Textures\\powerbolt");
      this.heading = this.itemID != Item.Book ? (this.itemID != Item.Blueprint ? ItemData2.ForDisplay(this.instance, this.item) : "Blueprint") : "Book";
      this.headingColor = Color.Yellow;
      if (Globals2.IsRareItem(this.item.ItemID))
      {
        this.heading += " (Rare Item)";
        this.headingColor = Color.Blue;
      }
      else if (this.isBlockInstanced)
        this.heading += " (Instanced)";
      else if (this.isBlockOwned)
        this.heading += " (Owned)";
      this.headingMeasure = this.Font.MeasureString(this.heading);
      string text = (string) null;
      if (this.item.ItemID == Item.Book && this.point.HasValue)
      {
        BookBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(this.point.Value) as BookBlock;
        if (dataBlock != null)
        {
          BookData bookData = this.instance.GetBookData(dataBlock.ID);
          if (bookData != null && bookData.Title != null && bookData.Title.Length > 0)
            text = bookData.Title.Replace('_', ' ');
        }
      }
      int val2 = 32;
      if (this.HasGeneralStats)
        val2 += 294;
      if (this.HasSkillStats)
        val2 += 264;
      if (this.HasMaterial)
        val2 += 294;
      if (this.HasAttackStats)
        val2 += 200;
      if (this.HasStatBonuses)
        val2 += 200;
      int num = Math.Max(650, val2);
      if (text == null)
        text = this.itemData.Desc;
      if (this.point.HasValue && (this.item.ItemID == Item.LockedChest || this.item.ItemID == Item.BlockShop || this.item.ItemID == Item.ItemShop))
      {
        PlayerBlock dataBlock = this.instance.MapStrategyTM.GetDataBlock(this.point.Value) as PlayerBlock;
        if (dataBlock != null)
          text = "This " + dataBlock.ClassType.ToString() + " belongs to: " + dataBlock.Gamertag + "\n" + text;
      }
      this.text = Utils.BreakIntoLines(this.Font, num - 50, this.textScale, text, true);
      this.textMeasure = Utils.MeasureText(this.Font, this.text, this.textScale);
      this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, (int) Math.Max((float) num, this.textMeasure.X + 50f), (int) ((double) this.textMeasure.Y + (double) this.headingMeasure.Y + 32.0));
      base.LoadContent();
      this.powerCount = this.point.HasValue ? this.instance.MapStrategyTM.GetPowerCount(this.point.Value) : -1;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitHandler((object) this, EventArgs.Empty);
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ItemOptions) && this.ItemAction())
        return true;
      if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.PickItem) && this.PickItem())
        this.ExitScreen();
      return base.HandleInput(input);
    }

    private void ExitHandler(object sender, EventArgs e)
    {
      CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuCancelSound);
      this.ExitScreen();
    }

    private void PickHandler(object sender, EventArgs e)
    {
      if (!this.PickItem())
        return;
      this.ExitScreen();
    }

    private void ItemActionHandler(object sender, EventArgs e)
    {
      this.ItemAction();
    }

    private bool ItemAction()
    {
      if (this.CanCustomizeItem)
      {
        this.OpenCustomizeItemScreen();
        this.ExitScreen();
        return true;
      }
      if (this.CanOwn)
      {
        this.instance.TakeOwnershipOfShop(this.player, this.point.Value);
        this.ExitScreen();
        return true;
      }
      if (this.CanRead)
      {
        this.instance.ReadBook(this.player, this.point.Value);
        this.ExitScreen();
        return true;
      }
      if (this.CanSetup)
      {
        this.instance.OpenSpecialBlock(this.player, this.point.Value, (Block) this.itemID);
        this.ExitScreen();
        return true;
      }
      if (this.CanInstance)
      {
        this.instance.InstantiateBlock(this.player, this.point.Value);
        this.ExitScreen();
        return true;
      }
      if (!this.CanChangeTexture)
        return false;
      BlockSelectMode mode = this.canSetChannel ? BlockSelectMode.SelectingChannel : (this.canSetPhoto ? BlockSelectMode.SelectingPhoto : (this.canSetGame ? BlockSelectMode.SelectingArcadeGame : (this.canSetKey ? BlockSelectMode.SelectingKey : (this.itemID == Item.MultiTextureBlock || this.itemID == Item.MultiTextureBlock2 ? BlockSelectMode.SelectingBlockTextureForMultTextureBlock : (this.itemID == Item.StainedGlass || this.itemID == Item.StainedGlassPane ? BlockSelectMode.SelectingStainedGlass : (this.itemID == Item.CoverBlock ? BlockSelectMode.SelectingUsedBlockTexture : (this.canSetDecal ? BlockSelectMode.SelectingDecal : (this.canSetComponent ? BlockSelectMode.SelectingCustomBlock : BlockSelectMode.SelectingBlockTexture))))))));
      string selectDesc = this.canSetChannel ? "Select Teleport Channel" : (this.canSetKey ? "Select Key" : (this.canSetPhoto ? "Select Photo" : (this.canSetGame ? "Set Game" : (this.canSetDecal ? "Set Decal" : (this.canSetComponent ? "Set Component" : "Select Block Texture")))));
      int slotID = this.point.HasValue ? (int) this.instance.Map.GetAuxHighData(this.point.Value) : 0;
      Block playerBlockID = mode == BlockSelectMode.SelectingDecal ? Block.zLastBlockID : (Block) this.instance.ConvertItemIDToBlockIDForTextureIndex(this.itemID);
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.SelectTextureBlockCallBack), selectDesc, mode, playerBlockID, slotID), this.ControllingPlayer);
      this.ExitScreen();
      return true;
    }

    private bool PickItem()
    {
      if (this.CanPickItem)
      {
        if (this.player.EquipFromInventory(this.itemID))
        {
          this.SetCurrentBlockTexture();
          this.ExitScreen();
          return true;
        }
        if (this.CanStockUpItem && this.player.AddToInventory(this.itemID, ItemData.GetStackSize(this.itemID)) > 0 && this.player.EquipFromInventory(this.itemID))
        {
          this.SetCurrentBlockTexture();
          this.ExitScreen();
          return true;
        }
      }
      return false;
    }

    private void OpenCustomizeItemScreen()
    {
      GameScreen screen = (GameScreen) null;
      int itemId = (int) this.itemID;
      if (screen == null)
        return;
      this.ScreenManager.AddScreen(screen, this.ControllingPlayer);
    }

    private void SetCurrentBlockTexture()
    {
      if (!this.point.HasValue)
        return;
      Block idForTextureIndex = (Block) this.instance.ConvertItemIDToBlockIDForTextureIndex(this.itemID);
      this.player.SetCurrentBlockAux(idForTextureIndex, this.instance.Map.GetAuxData(this.point.Value));
      if (!this.instance.Map.UsesBlockTextureTable(this.itemID))
        return;
      this.player.SetCurrentBlockTexture(idForTextureIndex, this.instance.Map.GetBlockTextureIndexFromExistingBlock(this.point.Value));
    }

    private bool SelectTextureBlockCallBack(Player player, Item item, int slotID, object tagData)
    {
      if (item == Item.None && !this.canSetPhoto && !this.canSetComponent)
        return false;
      if (this.point.HasValue && this.CanChangeTexture)
      {
        if (this.canSetComponent)
        {
          this.customBlockSlotID = slotID;
          this.ScreenManager.AddScreen((GameScreen) new LoadComponentPackScreen(false, false, new Action<int>(this.OnCustomBlockComponentPackSelected)), this.ControllingPlayer);
          return true;
        }
        if (this.canSetKey)
          item -= Item.SkeletonKey;
        Block blockID = this.canSetDecal ? Block.zLastBlockID : (Block) this.instance.Map.GetBlockID(this.point.Value);
        MapTM.BlockTextureChangeResult textureChangeResult = this.instance.Map.ChangeBlockTexture(player, this.point.Value, blockID, (Block) item);
        if (textureChangeResult != MapTM.BlockTextureChangeResult.None)
        {
          if (this.canSetGame)
            this.instance.RemoveArcadeMachine(this.point.Value, UpdateBlockMethod.Player);
          if (this.canSetKey && (blockID == Block.LockedDoorTop || blockID == Block.LockedDoorBottom))
          {
            GlobalPoint3D globalPoint3D = blockID == Block.LockedDoorTop ? GlobalPoint3D.Down : GlobalPoint3D.Up;
            int num = (int) this.instance.Map.ChangeBlockTexture(player, this.point.Value + globalPoint3D, blockID, (Block) item);
          }
          if (this.canSetPhoto)
          {
            bool flag = false;
            if (textureChangeResult == MapTM.BlockTextureChangeResult.NewTextureSelected)
            {
              byte auxHighData = this.instance.Map.GetAuxHighData(this.point.Value);
              GraphicStatics.PhotoData.ClearPhotoThumbnailColorData((int) auxHighData);
              if (tagData != null)
              {
                PhotoTag photoTag = (PhotoTag) tagData;
                GraphicStatics.PhotoData.LoadPhotoThumbnail(auxHighData, photoTag.Texture);
                GraphicStatics.PhotoData.LoadPhotoThumbnail(photoTag.PhotoID, auxHighData, PhotoFileType.SDThumbnail);
              }
              this.instance.NetworkManager.SendPhotoThumbnail((NetworkGamer) null, auxHighData);
              flag = true;
            }
            if (slotID > 0 && slotID < 16 && item == Item.None)
              flag = true;
            if (flag)
              this.instance.LoadTexturePack(true);
          }
        }
      }
      return true;
    }

    private void OnCustomBlockComponentPackSelected(int dirNum)
    {
      if (dirNum <= 0)
        return;
      this.ScreenManager.AddScreen((GameScreen) new ComponentListMenuScreen(this.instance, this.player, this.customBlockDirNum = dirNum, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnCustomBlockComponentSelected), false), this.ControllingPlayer);
    }

    private bool OnCustomBlockComponentSelected(MenuEntry entry)
    {
      this.instance.Map.CustomBlockModels[(int) (this.itemID - (ushort) 81), this.customBlockSlotID] = this.instance.VoxelModelManager.LoadComponent(this.customBlockDirNum, ((string) entry.Tag + entry.Text).Replace('\\', '_'), true);
      this.instance.Map.SetAuxData(this.point.Value, (byte) (this.customBlockSlotID << 4), UpdateBlockMethod.Player, this.player.GamerID, true);
      return true;
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      int num1 = 0;
      float num2 = (float) this.TransitionAlpha / (float) byte.MaxValue;
      this.colorWhite = Color.White * num2;
      this.colorBlack = Color.Black * num2;
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, this.screenRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(SamplerState.PointWrap, this.Matrix);
      Vector2 pos = new Vector2((float) (this.screenRect.X + 20), (float) (this.screenRect.Y + 10));
      Vector2 vector2 = new Vector2((float) (this.screenRect.X + 20), (float) (this.screenRect.Y + this.screenRect.Height - 44));
      this.spriteBatch.Draw(GraphicStatics.TexturePack.GetTexureForItem(this.item.ItemID), new Rectangle((int) pos.X, (int) pos.Y + 4, 32, 32), new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(this.item.ItemID)), Color.White);
      this.spriteBatch.End();
      this.spriteBatch.BeginTM(this.Matrix);
      this.spriteBatch.DrawString(this.Font, this.heading, pos + new Vector2(48f, 0.0f), this.headingColor * num2);
      Vector2 position = pos + new Vector2((float) (this.screenRect.Width - 220), 10f);
      if (this.pstr != null)
        this.spriteBatch.DrawString(this.Font, this.pstr, position, Color.White * num2, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      if (this.powerCount >= 0)
        this.spriteBatch.Draw(this.powerTex, new Rectangle((int) ((double) position.X - 26.0), (int) ((double) position.Y + 9.0), 20, 20), Color.White);
      Rectangle screenRect = this.screenRect;
      screenRect.X += 6;
      screenRect.Y = (int) ((double) pos.Y + (double) this.headingMeasure.Y + 8.0);
      screenRect.Width -= 12;
      screenRect.Height = 1;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, this.colorWhite);
      pos.X += 5f;
      pos.Y += this.headingMeasure.Y + 18f;
      foreach (string text in this.text)
      {
        this.spriteBatch.DrawString(this.Font, text, pos, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += this.Font.MeasureString(text).Y * this.textScale;
      }
      pos.Y += 12f;
      int num3 = 2;
      this.DrawItemStats(ref pos, screenRect);
      pos.X -= 5f;
      screenRect.Y = (int) pos.Y + 8;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, screenRect, this.colorWhite);
      pos.Y += 16f;
      Rectangle rect = new Rectangle((int) pos.X, (int) pos.Y, 28, 28);
      Rectangle r = new Rectangle(rect.X - 2, rect.Y - 2, 28, 34);
      GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, rect, this.colorWhite);
      pos.X += 42f;
      pos.Y += (float) num3;
      this.spriteBatch.DrawString(this.ScreenManager.GameFont, "Close", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
      if (this.drawFrameCount == 1)
      {
        r.Width = (int) ((double) pos.X + 78.0 - (double) r.X);
        this.AddWinRect(r, new EventHandler<EventArgs>(this.ExitHandler));
      }
      pos.X += 94f;
      if (this.CanPickItem)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.PickItem, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, "Pick", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 78f;
        if (this.drawFrameCount == 1)
        {
          r.X += r.Width + 3;
          r.Width -= 4;
          this.AddWinRect(r, new EventHandler<EventArgs>(this.PickHandler));
        }
      }
      if (this.CanOwn)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ItemOptions, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, "Economize", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 120f;
      }
      else if (this.CanRead)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ItemOptions, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, "Read", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 120f;
      }
      else if (this.CanSetup)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ItemOptions, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, "Setup", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 120f;
      }
      else if (this.CanInstance)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ItemOptions, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, "Instance", pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 120f;
      }
      else if (this.CanChangeTexture)
      {
        rect.X = (int) pos.X;
        rect.Y = (int) ((double) pos.Y - (double) num3);
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ItemOptions, rect, this.colorWhite);
        pos.X += 42f;
        this.spriteBatch.DrawString(this.Font, this.canSetChannel ? "Set Channel" : (this.canSetKey ? "Set Key" : (this.canSetPhoto ? "Set Photo" : (this.canSetGame ? "Set Game" : (this.canSetDecal ? "Set Decal" : (this.canSetComponent ? "Set Component" : "Set Texture"))))), pos + TMFont.yVec, this.colorWhite, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.X += 120f;
      }
      if (this.drawFrameCount == 1 && (this.CanOwn || this.CanRead || (this.CanSetup || this.CanInstance) || this.CanChangeTexture))
      {
        r.X += r.Width + 3;
        r.Width = (int) ((double) pos.X - (double) r.X - 8.0);
        this.AddWinRect(r, new EventHandler<EventArgs>(this.ItemActionHandler));
      }
      pos.Y += 14f;
      int num4 = (int) ((double) pos.Y + 1.0) - this.screenRect.Y;
      if (num4 > num1)
      {
        this.screenRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, this.screenRect.Width, num4 + 16);
        this.UpdateMatrix();
      }
      this.spriteBatch.End();
      CoreGlobals.FrameRateCounter.SpriteCalls += 2;
    }

    private void DrawItemStats(ref Vector2 pos, Rectangle lineRect)
    {
      float scale = 0.6f;
      int num1 = 26;
      lineRect.Y = (int) pos.Y;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, lineRect, this.colorWhite);
      pos.Y += 12f;
      float x = pos.X;
      float y = pos.Y;
      float num2 = 0.0f;
      if (this.HasGeneralStats)
      {
        int num3 = this.itemData.HealPower != (short) 0 ? 132 : 106;
        this.spriteBatch.DrawString(this.Font, "General Stats:", pos, Color.Yellow, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += 32f;
        if (this.itemTypeData.Class != ItemTypeClass.None && this.itemTypeData.Class != ItemTypeClass.CantMine)
        {
          this.spriteBatch.DrawString(this.Font, "Class: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, this.itemTypeData.Class.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
          ItemTypeClassDataXML typeClassDataXml = Globals1.ItemTypeClassData[(int) this.itemTypeData.Class];
          this.spriteBatch.DrawString(this.Font, "Power: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, typeClassDataXml.Power.ToString() + "/" + typeClassDataXml.MaxResistance.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        if (this.itemData.Durability > (ushort) 0)
        {
          this.spriteBatch.DrawString(this.Font, "Durability: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, string.Format("{0}/{1}", (object) this.item.Durability, (object) Math.Max(this.item.Durability, this.itemData.Durability)), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        if (this.itemData.HealPower != (short) 0)
        {
          this.spriteBatch.DrawString(this.Font, "Heal Power: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, this.itemData.HealPower.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        pos.X += 294f;
      }
      if ((double) pos.Y > (double) num2)
        num2 = pos.Y;
      if (this.instance.IsSkillsEnabled && this.HasSkillStats)
      {
        bool flag = !this.instance.IsSkillsEnabled;
        int num3 = 122;
        pos.Y = y;
        this.spriteBatch.DrawString(this.Font, "Skill Stats:", pos, Color.Yellow, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += 32f;
        if (this.skillData.UseSkill != SkillType.None)
        {
          int level = this.player.SkillsData[(int) this.skillData.UseSkill].Level;
          Color color = flag || level >= this.skillData.UseReq ? this.colorWhite : Color.IndianRed;
          this.spriteBatch.DrawString(this.Font, "Use Req: ", pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, this.skillData.UseSkill.ToString() + " " + this.skillData.UseReq.ToString(), pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
          this.spriteBatch.DrawString(this.Font, "Use XP*:", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          float num4 = this.skillData.UseExp * CharacterSkillsData.GetToolXPModifier(this.itemData.ItemID);
          if (this.itemData.ItemID < Item.zLastBlockID)
            num4 *= CharacterSkillsData.GetBlockXPModifier(this.instance.Map, (Block) this.itemData.ItemID);
          this.spriteBatch.DrawString(this.Font, num4.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        if (this.skillData.CraftReq > 0)
        {
          int level = this.player.SkillsData[(int) this.skillData.CraftSkill].Level;
          Color color = flag || level >= this.skillData.CraftReq ? this.colorWhite : Color.IndianRed;
          this.spriteBatch.DrawString(this.Font, "Craft Req: ", pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, this.skillData.CraftSkill.ToString() + " " + this.skillData.CraftReq.ToString(), pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
          this.spriteBatch.DrawString(this.Font, "Craft XP*:", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, this.skillData.CraftExp.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        if (this.skillData.MineReq > 0)
        {
          int level = this.player.SkillsData.Mining.Level;
          Color color = flag || level >= this.skillData.MineReq ? this.colorWhite : Color.IndianRed;
          this.spriteBatch.DrawString(this.Font, "Mining Req: ", pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          this.spriteBatch.DrawString(this.Font, "Level " + this.skillData.MineReq.ToString(), pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
          this.spriteBatch.DrawString(this.Font, "Mining XP*:", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X += (float) num3;
          float mineExp = this.skillData.MineExp;
          if (this.itemData.ItemID < Item.zLastBlockID)
            mineExp *= CharacterSkillsData.GetBlockXPModifier(this.instance.Map, (Block) this.itemData.ItemID);
          this.spriteBatch.DrawString(this.Font, mineExp.ToString("N1"), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          pos.X -= (float) num3;
          pos.Y += (float) num1;
        }
        pos.X += 264f;
      }
      if ((double) pos.Y > (double) num2)
        num2 = pos.Y;
      if (this.HasMaterial)
      {
        int num3 = 200;
        pos.Y = y;
        BlockDataXML blockDataXml = this.instance.Map.BlockData[(int) this.itemData.ItemID];
        this.spriteBatch.DrawString(this.Font, "Material: " + (object) blockDataXml.Material, pos, Color.Yellow, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += 32f;
        BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[(int) blockDataXml.Material];
        this.spriteBatch.DrawString(this.Font, "Resistance: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, blockMaterialDataXml.Resistance.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Pick Efficiency: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.GetEfficiencyString(blockMaterialDataXml.PickEfficiency), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Shovel Efficiency: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.GetEfficiencyString(blockMaterialDataXml.ShovelEfficiency), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Hatchet Efficiency: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.GetEfficiencyString(blockMaterialDataXml.HatchetEfficiency), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Weapon Efficiency: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.GetEfficiencyString(blockMaterialDataXml.WeaponEfficiency), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        pos.X += 294f;
      }
      if ((double) pos.Y > (double) num2)
        num2 = pos.Y;
      if (this.HasAttackStats)
      {
        int num3 = 100;
        pos.Y = y;
        this.spriteBatch.DrawString(this.Font, "Attack Stats:", pos, Color.Yellow, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += 32f;
        this.spriteBatch.DrawString(this.Font, "Damage: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.itemData.StrikeDamage.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Reach: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.itemData.StrikeReach.ToString("N2"), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Speed: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.swingTimeData.Time.ToString("N2"), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Delay: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.swingTimeData.Pause.ToString("N2"), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        pos.X += 200f;
      }
      if ((double) pos.Y > (double) num2)
        num2 = pos.Y;
      if (this.HasStatBonuses)
      {
        int num3 = 100;
        pos.Y = y;
        this.spriteBatch.DrawString(this.Font, "Stat Bonuses:", pos, Color.Yellow, 0.0f, Vector2.Zero, this.textScale, SpriteEffects.None, 0.0f);
        pos.Y += 32f;
        this.spriteBatch.DrawString(this.Font, "Health: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.combatData.Health.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Strength: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.combatData.Strength.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Attack: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.combatData.Attack.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Defense: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.combatData.Defence.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        this.spriteBatch.DrawString(this.Font, "Ranged: ", pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X += (float) num3;
        this.spriteBatch.DrawString(this.Font, this.combatData.Ranged.ToString(), pos, this.colorWhite, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        pos.X -= (float) num3;
        pos.Y += (float) num1;
        pos.X += 200f;
      }
      if ((double) pos.Y > (double) num2)
        num2 = pos.Y;
      pos.X = x;
      pos.Y = num2 + 4f;
    }

    private string GetEfficiencyString(ushort efficiency)
    {
      if (efficiency > (ushort) 0)
        return efficiency.ToString() + "%";
      return "Invalid";
    }
  }
}
