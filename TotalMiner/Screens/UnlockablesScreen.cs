// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.UnlockablesScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Achievements;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class UnlockablesScreen : BlockMenuScreen
  {
    private int lastEntrySelectedID = -1;
    private Matrix view;
    private Matrix proj;
    private Matrix world;
    private float z;
    private float roty;
    private Rectangle arrowRect;
    private Texture2D arrowTexture;
    private Unlockable unlocked;
    private Pulsator congratsPulse;
    private bool showRequirements;
    private bool npcSelecter;
    private Action<Player, ActorType> onSelected;
    private float screenScale;
    private Vector3 screenPos;
    private GameScreen parent;
    private Rectangle panelRect;
    private MapModel[] models;
    private GameInstance instance;
    private SamplerState pointClamp;
    private RasterizerState rasterStateCull;
    private VoxelModelManager voxelModelManager;
    private ActorType defaultSelection;
    private ComponentLoader comLoader;
    private ActorAnim anim;
    private int ry;

    public UnlockablesScreen(GameInstance instance, Player player, GameScreen parent)
      : this(instance, player, (Unlockable) null, true, false, new Action<Player, ActorType>(player.SetAvatar), player.ActorType, parent)
    {
    }

    public UnlockablesScreen(
      GameInstance instance,
      Player player,
      Unlockable unlocked,
      bool showRequirements,
      bool npcSelecter,
      Action<Player, ActorType> onSelected,
      ActorType defaultSelection,
      GameScreen parent)
      : base("Unlockables", player)
    {
      this.instance = instance;
      this.unlocked = unlocked;
      this.showRequirements = showRequirements;
      this.npcSelecter = npcSelecter;
      this.onSelected = onSelected;
      this.voxelModelManager = new VoxelModelManager(instance, "Content\\Map", true);
      this.defaultSelection = defaultSelection;
      this.parent = parent;
      this.selectedEntry = -1;
      List<BlockMenuEntry> items = new List<BlockMenuEntry>();
      if (npcSelecter)
      {
        foreach (ActorTypeDataXML actorTypeDataXml in Globals1.NpcTypeData)
        {
          if (this.ShowNPCType(actorTypeDataXml.ActorType))
            this.AddNPCItem(items, actorTypeDataXml.ActorType);
        }
      }
      else
      {
        foreach (Unlockable unlockable in player.Unlockables.UnlockableList)
        {
          if (!showRequirements || unlockable == null || !unlockable.IsNPC)
            this.AddUnlockableItem(items, unlockable);
        }
      }
      if (this.selectedEntry < 0)
        this.selectedEntry = 0;
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      items[items.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) items.ToArray());
    }

    private void AddUnlockableItem(List<BlockMenuEntry> items, Unlockable unlockable)
    {
      ActorType actorType = unlockable != null ? unlockable.ActorType : ActorType.None;
      BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, actorType.ToString());
      blockMenuEntry.Tag = (object) unlockable;
      if (!this.player.IsGod && unlockable != null && (this.showRequirements && !unlockable.IsUnlocked || !this.showRequirements && !this.HasAvailableNPCs(unlockable)))
      {
        blockMenuEntry.ColorOverride = Color.Gray;
        blockMenuEntry.OverrideColor = true;
        if (unlockable.HasProgress)
          blockMenuEntry.SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnProgressView);
        if (this.player.IsTesterman)
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.AvatarSelected);
      }
      else
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.AvatarSelected);
      if (this.unlocked != null && unlockable == this.unlocked || this.selectedEntry == -1 && actorType == this.defaultSelection)
      {
        this.selectedEntry = items.Count;
        this.congratsPulse = new Pulsator();
        this.congratsPulse.Start(1f, 0.1f, 0.5f, true);
      }
      blockMenuEntry.SelectUp += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.unlocked = (Unlockable) null);
      blockMenuEntry.SelectDown += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.unlocked = (Unlockable) null);
      items.Add(blockMenuEntry);
    }

    private void AddNPCItem(List<BlockMenuEntry> items, ActorType actorType)
    {
      BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, Globals1.NpcTypeData[(int) actorType].IDString);
      blockMenuEntry.Tag = (object) actorType;
      blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.AvatarSelected2);
      blockMenuEntry.SelectUp += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.unlocked = (Unlockable) null);
      blockMenuEntry.SelectDown += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.unlocked = (Unlockable) null);
      items.Add(blockMenuEntry);
    }

    private bool ShowNPCType(ActorType type)
    {
      if (type != ActorType.Player)
        return Globals1.NpcTypeData[(int) type].IsValid;
      return false;
    }

    private bool HasAvailableNPCs(Unlockable unlockable)
    {
      return true;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawItemTextures = false;
      this.DrawTitleStrip = this.DrawPanel = false;
      this.HighlightRect.Width = 230;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.ItemsPerPage = 10;
      this.DrawLastLine = false;
      base.LoadContent();
      this.Font = this.ItemFont = CoreGlobals.MenuFont;
      this.player.GetScreenOffset(this.MenuRect, out this.screenScale, out this.screenPos);
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 4 - this.arrowTexture.Width, 0, this.arrowTexture.Width, this.arrowTexture.Height);
      this.proj = Matrix.CreatePerspectiveFieldOfView(0.7853982f, 1f, 0.1f, 20f);
      this.z = 5.4f;
      this.roty = 3.141593f;
      this.pointClamp = new SamplerState()
      {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = TextureFilter.Point,
        MaxAnisotropy = 0,
        MaxMipLevel = 0
      };
      this.rasterStateCull = new RasterizerState()
      {
        CullMode = CullMode.CullCounterClockwiseFace,
        DepthBias = 0.0f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = false,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      if (this.selectedEntry > this.ItemsPerPage - 2)
        this.itemAtTopOfPage = this.selectedEntry - 8;
      if (this.instance != null && this.instance.LocalPlayerCount > 0)
      {
        this.comLoader = new ComponentLoader(PriorityLevel.Priority, this.voxelModelManager, "System Avatars", new Action<MapModel, int>(this.OnComponentLoaded), new Func<int, bool>(this.OnShouldLoadMesh));
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.comLoader, false, PriorityLevel.Priority);
      }
      this.anim = new ActorAnim();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.comLoader != null)
        this.comLoader.Abort(true);
      else
        this.voxelModelManager.UnloadContent();
    }

    private void AvatarSelected(object sender, PlayerIndexEventArgs e)
    {
      MenuEntry menuEntry = sender as MenuEntry;
      if (menuEntry != null && this.onSelected != null)
        this.onSelected(this.player, menuEntry.Tag == null ? ActorType.None : ((Unlockable) menuEntry.Tag).ActorType);
      this.ExitScreen();
    }

    private void AvatarSelected2(object sender, PlayerIndexEventArgs e)
    {
      MenuEntry menuEntry = sender as MenuEntry;
      if (menuEntry != null && this.onSelected != null)
        this.onSelected(this.player, (ActorType) menuEntry.Tag);
      this.ExitScreen();
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      this.roty += currentGamePadState.ThumbSticks.Right.X * 0.05f;
      this.z += currentGamePadState.ThumbSticks.Right.Y * 0.1f;
      this.z = MathHelper.Clamp(this.z, 1f, 6f);
      return base.HandleInput(input);
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      if (this.parent == null)
        return;
      this.ScreenManager.AddScreen(this.parent, this.ControllingPlayer);
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      Rectangle rectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - 36, 24, 24);
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 4;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - 4 - this.arrowTexture.Height;
      this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, Color.White);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawMenuExtra()
    {
      if (this.unlocked == null || this.congratsPulse == null)
        return;
      this.congratsPulse.Update();
      this.spriteBatch2.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, (RasterizerState) null, (Effect) null, this.Matrix);
      this.spriteBatch2.DrawStringCentered(CoreGlobals.GameFont, "Congratulations!!", (float) (this.GraphicsDevice.Viewport.Height / 2 - 70), Color.LightYellow * this.congratsPulse.Value, 1f);
      this.spriteBatch2.DrawStringCentered(CoreGlobals.GameFont, "You have unlocked the " + (object) this.unlocked.ActorType, (float) (this.GraphicsDevice.Viewport.Height / 2 - 20), Color.Yellow * this.congratsPulse.Value, 1f);
      this.spriteBatch2.End();
    }

    protected override void DrawMenuEntries()
    {
      this.spriteBatch2.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, this.Matrix);
      base.DrawMenuEntries();
      this.spriteBatch2.End();
    }

    protected override int MenuRectWidthExt
    {
      get
      {
        return 500;
      }
    }

    protected override void DrawEntry(
      MenuEntry menuEntry,
      int entryID,
      Vector2 position,
      bool isSelected)
    {
      if (GraphicStatics.TexturePack.NeedLightMap)
        GraphicStatics.TexturePack.LoadLightMap();
      base.DrawEntry(menuEntry, entryID, position, isSelected);
      if (menuEntry.Tag == null)
        return;
      Unlockable tag = menuEntry.Tag as Unlockable;
      if (!isSelected)
        return;
      this.panelRect = new Rectangle(this.MenuRect.X + this.HighlightRect.Width, this.MenuRect.Y, 0, 0);
      this.panelRect.Width = this.MenuRectWidthExt - 150;
      this.panelRect.Height = this.MenuRect.Height - 80;
      if (!this.npcSelecter && this.showRequirements && !tag.IsDisplayed)
      {
        Rectangle panelRect = this.panelRect;
        Texture2D lockedTexture = GraphicStatics.LockedTexture;
        panelRect.Width = lockedTexture.Width * 6;
        panelRect.Height = lockedTexture.Height * 6;
        panelRect.X += (this.panelRect.Width - panelRect.Width) / 2;
        panelRect.Y += (this.panelRect.Height - panelRect.Height) / 2;
        this.spriteBatch2.Draw(lockedTexture, panelRect, Color.White);
        if (tag.HasProgress)
        {
          panelRect.X -= 24;
          panelRect.Y += panelRect.Height + 20;
          panelRect.Width = panelRect.Height = 24;
          this.spriteBatch2.Draw(CoreGlobals.ButtonTextureX, panelRect, Color.White);
          this.spriteBatch2.DrawString(this.Font, "View Progress", new Vector2((float) (panelRect.X + 32), (float) (panelRect.Y + 6 + TMFont.yOff)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
        }
      }
      else
      {
        ActorType actorType = this.npcSelecter ? (ActorType) menuEntry.Tag : tag.ActorType;
        if (entryID != this.lastEntrySelectedID)
        {
          ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) actorType];
          try
          {
            int num = actorTypeDataXml.ComNameWalk != null ? actorTypeDataXml.ComNameWalk.Length : 0;
            this.models = new MapModel[1 + num];
            for (int index = 0; index < this.models.Length; ++index)
              this.models[index] = index >= num / 2 ? (index <= num / 2 ? this.LoadModel(actorType, actorTypeDataXml.ComName) : this.LoadModel(actorType, actorTypeDataXml.ComNameWalk[index - 1])) : this.LoadModel(actorType, actorTypeDataXml.ComNameWalk[index]);
          }
          catch (Exception ex)
          {
            Services.ExceptionReporter.ReportExceptionCaught(94, ex);
          }
          this.anim.CurrentFrame = 0;
          this.lastEntrySelectedID = entryID;
        }
        if (this.models != null)
        {
          this.anim.Update(Services.ElapsedTime, this.models.Length - 1, Vector3.One, Vector2.One, 0.25f);
          MapModel model1 = this.models[this.anim.CurrentFrame];
          if (model1 != null)
          {
            Viewport viewport1 = this.GraphicsDevice.Viewport;
            Viewport viewport2 = viewport1;
            Rectangle panelRect = this.panelRect;
            Viewport viewport3 = this.player.Viewport;
            panelRect.X = (int) ((double) panelRect.X * (double) this.screenScale + (double) this.screenPos.X + (double) viewport3.X);
            panelRect.Y = (int) ((double) panelRect.Y * (double) this.screenScale + (double) this.screenPos.Y + (double) viewport3.Y);
            panelRect.Width = (int) ((double) panelRect.Width * (double) this.screenScale);
            panelRect.Height = (int) ((double) panelRect.Height * (double) this.screenScale);
            viewport2.X = panelRect.X;
            viewport2.Y = panelRect.Y;
            viewport2.Width = panelRect.Width;
            viewport2.Height = panelRect.Height;
            this.GraphicsDevice.Viewport = viewport2;
            float farPlaneDistance = 100f;
            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, (float) ((double) model1.ModelSize.Y / (double) viewport2.Height * 60.0) + this.z);
            this.view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            this.proj = Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float) viewport2.Width / (float) viewport2.Height, 0.1f, farPlaneDistance);
            this.GraphicsDevice.RasterizerState = this.rasterStateCull;
            this.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.GraphicsDevice.SamplerStates[0] = this.pointClamp;
            this.GraphicsDevice.SamplerStates[1] = this.pointClamp;
            this.GraphicsDevice.SamplerStates[2] = this.pointClamp;
            this.GraphicsDevice.SamplerStates[3] = this.pointClamp;
            this.GraphicsDevice.SamplerStates[4] = this.pointClamp;
            //this.GraphicsDevice.ReferenceStencil = 0;
            GraphicStatics.AvatarShader.FarClip.SetValue(farPlaneDistance);
            GraphicStatics.AvatarShader.FadeStart.SetValue(farPlaneDistance);
            GraphicStatics.AvatarShader.LanturnColor.SetValue(0);
            GraphicStatics.AvatarShader.LanturnRange.SetValue(0);
            GraphicStatics.AvatarShader.FogStart.SetValue(farPlaneDistance);
            GraphicStatics.AvatarShader.FogEnd.SetValue(farPlaneDistance);
            GraphicStatics.AvatarShader.FogColor.SetValue(Vector4.Zero);
            GraphicStatics.AvatarShader.LightCycle.SetValue(1f);
            GraphicStatics.AvatarShader.MaxLight.SetValue(this.instance.Map.MaxLight + 1f);
            GraphicStatics.AvatarShader.SunDirection.SetValue(new Vector3(0.0f, 0.0f, -8f));
            GraphicStatics.AvatarShader.SunPosition.SetValue(new Vector3(0.0f, 100f, 0.0f));
            GraphicStatics.AvatarShader.RayDistance.SetValue(0);
            GraphicStatics.AvatarShader.Alpha.SetValue(1);
            GraphicStatics.AvatarShader.TintColor.SetValue(Vector4.One);
            float scale = 0.145f;
            this.world = Matrix.CreateTranslation(-((float) model1.ModelSize.X * 0.5f), -((float) model1.ModelSize.Y * 0.5f), -((float) model1.ModelSize.Z * 0.5f)) * Matrix.CreateRotationY(this.roty) * Matrix.CreateScale(scale);
            this.world.M44 = 49407f;
            GraphicStatics.AvatarShader.World.SetValue(this.world);
            GraphicStatics.AvatarShader.ViewProjection.SetValue(this.view * this.proj);
            GraphicStatics.AvatarShader.CameraPosition.SetValue(cameraPosition);
            GraphicStatics.AvatarShader.LightMapTexture.SetValue((Texture) GraphicStatics.TexturePack.LightMapTexture);
            GraphicStatics.AvatarShader.NightLightMapTexture.SetValue((Texture) GraphicStatics.TexturePack.NightLightMapTexture);
            MapChunkContentData chunkContentData = model1.MapChunkContentData;
            if (chunkContentData.VertexBuffer != null && chunkContentData.VertexCount > 0)
            {
              this.GraphicsDevice.Indices = MapChunkContent.IndexBuffer;
              this.GraphicsDevice.SetVertexBuffer(chunkContentData.VertexBuffer);
              Effect effect = GraphicStatics.AvatarShader.Effect;
              effect.CurrentTechnique = effect.Techniques["AvatarShader"];
              effect.CurrentTechnique.Passes[0].Apply();
              this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2);
            }
            this.GraphicsDevice.Viewport = viewport1;
          }
          if (!this.showRequirements && this.instance != null && (this.instance.NpcManager != null && this.npcSelecter))
          {
            this.ry = this.panelRect.Y + 20;
            this.DrawCriteria("Frames: " + (object) this.models.Length, Color.White, 0.4f);
            int num1 = 0;
            int num2 = 0;
            foreach (MapModel model2 in this.models)
            {
              if (model2 != null)
              {
                num1 += model2.Map.MemorySize;
                num2 += (int) model2.BufferSize;
              }
            }
            this.DrawCriteria(string.Format("Model Ram: {0:N1}kb", (object) (float) ((double) num1 / 1024.0)), Color.White, 0.4f);
            this.DrawCriteria(string.Format("Mesh Ram: {0:N1}kb", (object) (float) ((double) num2 / 1024.0)), Color.White, 0.4f);
            this.DrawCriteria(string.Format("Total Ram: {0:N1}kb", (object) (float) ((double) (num1 + num2) / 1024.0)), Color.White, 0.4f);
            this.ry = this.panelRect.Y + 290;
            ActorLevelDataXML actorLevelDataXml = Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) actorType].LevelType];
            this.DrawCriteria("Combat: " + (object) SkillData.CombatLevel((float) actorLevelDataXml.HealthLevel, (float) actorLevelDataXml.StrengthLevel, (float) actorLevelDataXml.AttackLevel, (float) actorLevelDataXml.DefenceLevel, (float) actorLevelDataXml.RangedLevel), Color.Yellow);
            this.DrawCriteria("Health: " + (object) actorLevelDataXml.HealthLevel, Color.Yellow);
            this.DrawCriteria("Attack: " + (object) actorLevelDataXml.AttackLevel, Color.Yellow);
            this.DrawCriteria("Strength: " + (object) actorLevelDataXml.StrengthLevel, Color.Yellow);
            this.DrawCriteria("Defense: " + (object) actorLevelDataXml.DefenceLevel, Color.Yellow);
            this.DrawCriteria("Ranged: " + (object) actorLevelDataXml.RangedLevel, Color.Yellow);
          }
        }
      }
      if (!this.showRequirements)
        return;
      this.DrawCriteria(tag);
    }

    private MapModel LoadModel(ActorType actorType, string comName)
    {
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) actorType];
      if (actorTypeDataXml.ComModID == 0)
        return this.voxelModelManager.LoadComponent("System Avatars", comName, true);
      return this.instance.VoxelModelManager.LoadComponent(actorTypeDataXml.ComModID + 1000000, comName, true) ?? this.voxelModelManager.LoadComponent("System Avatars", comName, true);
    }

    private void OnComponentLoaded(MapModel model, int index)
    {
    }

    private bool OnShouldLoadMesh(int index)
    {
      return index == this.selectedEntry;
    }

    private void DrawCriteria(Unlockable unlockable)
    {
      this.SpriteBatch.DrawString(this.Font, "To Unlock: ", new Vector2((float) (this.panelRect.X + 20), (float) (this.panelRect.Y + this.panelRect.Height + 1)), Color.Yellow, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.SpriteBatch.DrawString(this.Font, unlockable.UnlockInstruction, new Vector2((float) (this.panelRect.X + 20), (float) (this.panelRect.Y + this.panelRect.Height + 24)), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      if (unlockable.GameModes != null)
      {
        this.ry = this.panelRect.Y + 15;
        this.DrawCriteria("Game Modes:", Color.Yellow);
        for (int index = 0; index < unlockable.GameModes.Length; ++index)
          this.DrawCriteria(Utils.InsertSpacesBeforeCapitals(unlockable.GameModes[index].ToString()), Color.White);
      }
      if (unlockable.SessionTypes != null)
      {
        this.ry = this.panelRect.Y + 146;
        this.DrawCriteria("Game Types:", Color.Yellow);
        for (int index = 0; index < unlockable.SessionTypes.Length; ++index)
          this.DrawCriteria(unlockable.SessionTypes[index] == NetworkSessionType.Local ? "Local" : NetworkManager.GetNetworkTypeDesc(unlockable.SessionTypes[index]), Color.White);
      }
      if (unlockable.Difficulties == null)
        return;
      this.ry = this.panelRect.Y + 276;
      this.DrawCriteria("Difficulty:", Color.Yellow);
      for (int index = 0; index < unlockable.Difficulties.Length; ++index)
        this.DrawCriteria(unlockable.Difficulties[index].ToString(), Color.White);
    }

    private void DrawCriteria(string criteria, Color color)
    {
      this.DrawCriteria(criteria, color, 0.7f);
    }

    private void DrawCriteria(string criteria, Color color, float scale)
    {
      Vector2 vector2 = this.Font.MeasureString(criteria) * scale;
      this.SpriteBatch.DrawString(this.Font, criteria, new Vector2((float) (this.panelRect.X + this.panelRect.Width + 130 - (int) vector2.X), (float) this.ry), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.ry += (int) ((double) vector2.Y - 8.0 * (double) scale);
    }

    private void OnProgressView(object sender, PlayerIndexEventArgs e)
    {
      List<string> progressList = (((MenuEntry) sender).Tag as Unlockable).ProgressList;
      if (progressList == null || progressList.Count <= 0)
        return;
      float scale = 0.7f;
      float num1 = 0.0f;
      foreach (string text in progressList)
      {
        float num2 = CoreGlobals.GameFont.MeasureString(text).X * scale;
        if ((double) num2 > (double) num1)
          num1 = num2;
      }
      this.ScreenManager.AddScreen((GameScreen) new ListBoxScreen(this.player, progressList.ToArray(), (string) null, (ListBoxScreen.OnMenuItemSelected) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, (string) null, (EventHandler<PlayerIndexEventArgs>) null, false, scale, (int) num1 + 96), this.ControllingPlayer);
    }
  }
}
