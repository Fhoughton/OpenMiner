// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ParticleEmitterBlockScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ParticleEmitterBlockScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private ParticleEmitterBlock block;
    private ParticleData data;
    private Parser parser;
    private int templateID;
    private string[] particleTypes;
    private int lastSpawnCount;
    private string templateName;
    private float copyToScriptTimer;

    private string ParticleTypeText
    {
      get
      {
        return "Particle Type: " + this.templateName;
      }
    }

    private string UsageText
    {
      get
      {
        int num = (int) ((double) this.instance.EmitterParticleSystem.MaxParticles / (double) this.instance.EmitterParticleSystem.ParticleDuration);
        return string.Format("Global Particle Usage: {0} of {1} per second: {2:N1}%", (object) this.lastSpawnCount, (object) num, (object) (float) ((double) this.lastSpawnCount / (double) num * 100.0));
      }
    }

    private string DurationText
    {
      get
      {
        return "Duration (seconds): " + (object) (float) ((double) this.data.Duration / 1000.0);
      }
    }

    private string EmitFreqText
    {
      get
      {
        return "Emit Frequency (seconds): " + (object) (float) ((double) this.data.EmitFreq / 1000.0);
      }
    }

    private string EmitPosOffsetText
    {
      get
      {
        return "Emit Position Offset: " + this.EmitPosOffsetTextForInput;
      }
    }

    private string EmitPosOffsetTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}", (object) this.data.EmitPosOffset.X, (object) this.data.EmitPosOffset.Y, (object) this.data.EmitPosOffset.Z);
      }
    }

    private string EmitPosVarianceText
    {
      get
      {
        return "Emit Position Variance: " + this.EmitPosVarianceTextForInput;
      }
    }

    private string EmitPosVarianceTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}", (object) this.data.EmitPosVariance.X, (object) this.data.EmitPosVariance.Y, (object) this.data.EmitPosVariance.Z);
      }
    }

    private string VelocityText
    {
      get
      {
        return "Velocity (mps): " + this.VelocityTextForInput;
      }
    }

    private string VelocityTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}", (object) this.data.Velocity.X, (object) this.data.Velocity.Y, (object) this.data.Velocity.Z);
      }
    }

    private string VelocityVarianceText
    {
      get
      {
        return "Velocity Variance (mps): " + this.VelocityVarianceTextForInput;
      }
    }

    private string VelocityVarianceTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}", (object) this.data.VelocityVariance.X, (object) this.data.VelocityVariance.Y, (object) this.data.VelocityVariance.Z);
      }
    }

    private string GravityText
    {
      get
      {
        return "Gravity: " + (object) (float) ((double) this.data.Gravity * 0.00999999977648258);
      }
    }

    private string RotationText
    {
      get
      {
        return "Rotation Speed: " + (object) this.data.Rotation;
      }
    }

    private string StartSizeText
    {
      get
      {
        return "Start Size: " + this.StartSizeTextForInput;
      }
    }

    private string StartSizeTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}", (object) this.data.Size.X, (object) this.data.Size.Y, (object) this.data.Size.Z);
      }
    }

    private string EndSizeText
    {
      get
      {
        return string.Format("End Size (Multiplier): {0}, ({1}, {2}, {3})", (object) this.data.Size.W, (object) (float) ((double) this.data.Size.X * (double) this.data.Size.W), (object) (float) ((double) this.data.Size.Y * (double) this.data.Size.W), (object) (float) ((double) this.data.Size.Z * (double) this.data.Size.W));
      }
    }

    private string EndSizeTextForInput
    {
      get
      {
        return this.data.Size.W.ToString();
      }
    }

    private string WindFactorText
    {
      get
      {
        return "Wind Affection: " + (object) this.data.WindFactor;
      }
    }

    private string StartColorText
    {
      get
      {
        return "Start Color: " + this.StartColorTextForInput;
      }
    }

    private string StartColorTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}, {3}", (object) this.data.StartColor.R, (object) this.data.StartColor.G, (object) this.data.StartColor.B, (object) this.data.StartColor.A);
      }
    }

    private string EndColorText
    {
      get
      {
        return "End Color: " + this.EndColorTextForInput;
      }
    }

    private string EndColorTextForInput
    {
      get
      {
        return string.Format("{0}, {1}, {2}, {3}", (object) this.data.EndColor.R, (object) this.data.EndColor.G, (object) this.data.EndColor.B, (object) this.data.EndColor.A);
      }
    }

    private string RequiresPowerText
    {
      get
      {
        return "Requires Power: " + (this.block.RequiresPower ? "Yes" : "No");
      }
    }

    private string ProximityText
    {
      get
      {
        return "Proximity: " + (object) this.data.Proximity;
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.ParticleEmitter, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    private bool CanSaveTemplateName
    {
      get
      {
        if (this.templateName.IsNotEmpty() && !this.templateName.StartsWith("system\\", StringComparison.OrdinalIgnoreCase))
          return this.templateName != "Select Template";
        return false;
      }
    }

    public ParticleEmitterBlockScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Particle Emitter", player)
    {
      ParticleEmitterBlockScreen emitterBlockScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.ParticleEmitter, UpdateBlockMethod.Player, this.PlayerID, true) as ParticleEmitterBlock;
      this.data = this.block.Data;
      this.lastSpawnCount = 0;
      this.templateName = Globals2.GetParticleTemplateName(this.data);
      if (this.templateName == null)
        this.templateName = "Select Template";
      int index1 = 0;
      this.particleTypes = new string[Globals2.SystemParticleData.Length + Globals2.CustomParticleData.Count];
      for (; index1 < Globals2.SystemParticleData.Length; ++index1)
      {
        if (index1 > 0)
          this.particleTypes[index1] = Globals2.SystemParticleData[index1].Name;
      }
      for (int index2 = 0; index2 < Globals2.CustomParticleData.Count; ++index2)
        this.particleTypes[index1 + index2] = Globals2.CustomParticleData[index2].Name;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.UsageText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.ParticleTypeText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Save as Custom Template"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.DurationText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EmitFreqText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EmitPosOffsetText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EmitPosVarianceText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.VelocityText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.VelocityVarianceText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GravityText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.RotationText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.StartSizeText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EndSizeText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.WindFactorText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.StartColorText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EndColorText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.ProximityText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.RequiresPowerText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Copy to Script Clipboard"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Close"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index3 = num1;
      int num2 = index3 + 1;
      blockMenuEntryList2[index3].IsEnabled = false;
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index4 = num2;
      int num3 = index4 + 1;
      blockMenuEntryList3[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new ParticleTemplateListScreen(instance, player, (string) null, new Action<string, string>(emitterBlockScreen.OnParticleTypeSelected), false), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index5 = num3;
      int num4 = index5 + 1;
      blockMenuEntryList4[index5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new ParticleTemplateListScreen(instance, player, (string) null, new Action<string, string>(emitterBlockScreen.OnSaveNameEntered), true), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index6 = num4;
      int num5 = index6 + 1;
      blockMenuEntryList5[index6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Particle Duration", "Enter the time in seconds the particle is alive. Max duration is 8.0. Enter a value of zero to disable the particle emitter.", ((float) this.data.Duration / 1000f).ToString(), new AsyncCallback(this.OnDurationEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index7 = num5;
      int num6 = index7 + 1;
      blockMenuEntryList6[index7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(emitterBlockScreen.OnEmitFreqEntered), (double) emitterBlockScreen.data.EmitFreq / 1000.0, true, false), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index8 = num6;
      int num7 = index8 + 1;
      blockMenuEntryList7[index8].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Emit Position Offset", "Enter the X, Y, Z emit position offset e.g. 1, 0.5, -1.5", this.EmitPosOffsetTextForInput, new AsyncCallback(this.OnEmitPosOffsetEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index9 = num7;
      int num8 = index9 + 1;
      blockMenuEntryList8[index9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Emit Position Variance", "Enter the X, Y, Z variances for the particle emit position e.g. 1, 0.5, -1.5", this.EmitPosVarianceTextForInput, new AsyncCallback(this.OnEmitPosVarianceEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index10 = num8;
      int num9 = index10 + 1;
      blockMenuEntryList9[index10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Velocity (meters per second)", "Enter the X, Y, Z velocities for the particle e.g. 1.5, 2.5, 0.5. Maximum velocity for an axis is 50.", this.VelocityTextForInput, new AsyncCallback(this.OnVelocityEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index11 = num9;
      int num10 = index11 + 1;
      blockMenuEntryList10[index11].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Velocity Variance", "Enter the X, Y, Z variances for the particle velocity e.g. 0.25, 0.5, 0.25. Maximum velocity variance for an axis is 50.", this.VelocityVarianceTextForInput, new AsyncCallback(this.OnVelocityVarianceEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
      int index12 = num10;
      int num11 = index12 + 1;
      blockMenuEntryList11[index12].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(emitterBlockScreen.OnGravityEntered), (double) emitterBlockScreen.data.Gravity * 0.01, true, true), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index13 = num11;
      int num12 = index13 + 1;
      blockMenuEntryList12[index13].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(emitterBlockScreen.OnRotationEntered), emitterBlockScreen.data.Rotation, true, true), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList13 = blockMenuEntryList1;
      int index14 = num12;
      int num13 = index14 + 1;
      blockMenuEntryList13[index14].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Start Size", "Enter the start size of the particles for the X, Y and Z axis e.g. 0.55, 0.45, 0.35. Maximum start size for an axis is 50.", this.StartSizeTextForInput, new AsyncCallback(this.OnStartSizeEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList14 = blockMenuEntryList1;
      int index15 = num13;
      int num14 = index15 + 1;
      blockMenuEntryList14[index15].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "End Size", "Enter a number which is multiplied by Start Size to calculate the End Size. If the particle size will not change, enter 1.", this.EndSizeTextForInput, new AsyncCallback(this.OnEndSizeEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList15 = blockMenuEntryList1;
      int index16 = num14;
      int num15 = index16 + 1;
      blockMenuEntryList15[index16].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(emitterBlockScreen.OnWindFactorEntered), emitterBlockScreen.data.WindFactor, true, false), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList16 = blockMenuEntryList1;
      int index17 = num15;
      int num16 = index17 + 1;
      blockMenuEntryList16[index17].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Start Color", "Enter the R, G, B, A values for the start color e.g. 100, 228, 200, 255", this.StartColorTextForInput, new AsyncCallback(this.OnStartColorEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList17 = blockMenuEntryList1;
      int index18 = num16;
      int num17 = index18 + 1;
      blockMenuEntryList17[index18].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "End Color", "Enter the R, G, B, A values for the end color e.g. 100, 228, 200, 255", this.EndColorTextForInput, new AsyncCallback(this.OnEndColorEntered), (object) null, this.MenuEntries[this.selectedEntry], true));
      List<BlockMenuEntry> blockMenuEntryList18 = blockMenuEntryList1;
      int index19 = num17;
      int num18 = index19 + 1;
      blockMenuEntryList18[index19].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => emitterBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(emitterBlockScreen.OnProximityEntered), emitterBlockScreen.data.Proximity, true, false), emitterBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList19 = blockMenuEntryList1;
      int index20 = num18;
      int num19 = index20 + 1;
      blockMenuEntryList19[index20].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.RequiresPower = !this.block.RequiresPower;
        this.ResetMenuItemText();
      });
      List<BlockMenuEntry> blockMenuEntryList20 = blockMenuEntryList1;
      int index21 = num19;
      int num20 = index21 + 1;
      blockMenuEntryList20[index21].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      List<BlockMenuEntry> blockMenuEntryList21 = blockMenuEntryList1;
      int index22 = num20;
      int num21 = index22 + 1;
      blockMenuEntryList21[index22].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        ScriptEditScreen.SetClipboard(this.GetEmitterScriptSyntax());
        this.MenuEntries[this.MenuEntries.Count - 2].IsEnabled = false;
        this.copyToScriptTimer = 1f;
      });
      List<BlockMenuEntry> blockMenuEntryList22 = blockMenuEntryList1;
      int index23 = num21;
      int num22 = index23 + 1;
      blockMenuEntryList22[index23].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
      this.selectedEntry = 1;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 624;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      this.ItemHeight = 28;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.6f;
      base.LoadContent();
      this.UpdateUsageText();
      this.MenuEntries[1].ToolTip.Text = "Set the particle emitter settings from a list of preset templates.";
      this.MenuEntries[2].ToolTip.Text = "Save this blocks settings as a custom preset template that can be reused on other blocks.";
      this.MenuEntries[5].ToolTip.Text = "You can offset the start position of the emitted particles for cases where you do not want the particles to be emitted directly from the emitter block. The SnowMachine and HeliRotor template use this field.";
      this.MenuEntries[6].ToolTip.Text = "Use variance to add randomness to the offset position. e.g. if you use 10 for the X variance, the particles will be emitted a random distance up to 5 meters either side of the X offset position. Several of the preset templates use this field for their effect.";
      this.MenuEntries[8].ToolTip.Text = "Use variance to add randomness to the velocity of the particle. e.g. if the particles X velocity is 5 and you use an X variance of 2, the particles X velocity will be a random value between 4 and 6; (5 - 1 / 5 + 1).";
      this.MenuEntries[10].ToolTip.Text = "Specify how fast the particle rotates in radians per second.\n\nA value of 6.28 (PI x 2) will be one full rotation per second.\n\nA value of zero = no rotation.\nA positive value = clockwise rotation.\nA negative value = anti-clockwise rotation.";
      this.MenuEntries[12].ToolTip.Text = "The End Size parameter is a single number which is multiplied by the Start Size to calculate the End Size.\n\nA particle is spawned with the Start Size and interpolates to the End Size over it's life time (Duration).";
      this.MenuEntries[13].ToolTip.Text = "This value determines how much the Wind affects the particle. A value of zero means the wind does not affect the particle at all. A value of 1 means normal.\n\nTechnical: Particle Velocity += Wind Velocity * Wind Affection.";
      this.MenuEntries[16].ToolTip.Text = "This is a distance value. If the player is further from the block than this value, then the particles will stop emitting. This mechanism is used to control and reduce particle usage for particles that cannot be seen by the player if the player is some distance from the emitter block.\n\nA value of zero means the proximity mechanism is not used and particles will always emit from the block regardless of how far away the player is.";
      this.MenuEntries[19].ToolTip.Text = "Copy this blocks settings into the Script Editor clipboard. Open a script and paste as a ParticleEmitter command.";
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      this.block.Data = this.data;
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, !this.HasParticleData);
    }

    private string GetEmitterScriptSyntax()
    {
      return string.Format("ParticleEmitter [x,y,z] [{0}] [{1}] [{2}] [{3},{4},{5}] [{6},{7},{8},{9}] [{10},{11},{12},{13}] [{14}] [{15}] [{16},{17},{18}] [{19},{20},{21},{22}] [{23},{24},{25}] [{26},{27},{28}]", (object) 2, (object) (float) ((double) this.data.EmitFreq / 1000.0), (object) (float) ((double) this.data.Duration / 1000.0), (object) this.data.Velocity.X, (object) this.data.Velocity.Y, (object) this.data.Velocity.Z, (object) this.data.Size.X, (object) this.data.Size.Y, (object) this.data.Size.Z, (object) this.data.Size.W, (object) this.data.StartColor.R, (object) this.data.StartColor.G, (object) this.data.StartColor.B, (object) this.data.StartColor.A, (object) (float) ((double) this.data.Gravity / 100.0), (object) this.data.Rotation, (object) this.data.VelocityVariance.X, (object) this.data.VelocityVariance.Y, (object) this.data.VelocityVariance.Z, (object) this.data.EndColor.R, (object) this.data.EndColor.G, (object) this.data.EndColor.B, (object) this.data.EndColor.A, (object) this.data.EmitPosVariance.X, (object) this.data.EmitPosVariance.Y, (object) this.data.EmitPosVariance.Z, (object) this.data.EmitPosOffset.X, (object) this.data.EmitPosOffset.Y, (object) this.data.EmitPosOffset.Z);
    }

    private bool HasParticleData
    {
      get
      {
        if (this.data.Duration <= (ushort) 0 && this.data.EmitFreq <= 0 && (!(this.data.EmitPosOffset != Vector3.Zero) && !(this.data.EmitPosVariance != Vector3.Zero)) && (!(this.data.Velocity != Vector3.Zero) && !(this.data.VelocityVariance != Vector3.Zero) && ((double) this.data.Rotation == 0.0 && (double) this.data.Size.X == 0.0)) && (double) this.data.Size.Y == 0.0)
          return (double) this.data.Size.Z != 0.0;
        return true;
      }
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.ParticleEmitter, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.ParticleEmitter, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.ResetMenuItemText();
      return true;
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.ResetMenuItemText();
    }

    private void ResetMenuItemText()
    {
      this.MenuEntries[1].Text = this.ParticleTypeText;
      this.MenuEntries[3].Text = this.DurationText;
      this.MenuEntries[4].Text = this.EmitFreqText;
      this.MenuEntries[5].Text = this.EmitPosOffsetText;
      this.MenuEntries[6].Text = this.EmitPosVarianceText;
      this.MenuEntries[7].Text = this.VelocityText;
      this.MenuEntries[8].Text = this.VelocityVarianceText;
      this.MenuEntries[9].Text = this.GravityText;
      this.MenuEntries[10].Text = this.RotationText;
      this.MenuEntries[11].Text = this.StartSizeText;
      this.MenuEntries[12].Text = this.EndSizeText;
      this.MenuEntries[13].Text = this.WindFactorText;
      this.MenuEntries[14].Text = this.StartColorText;
      this.MenuEntries[15].Text = this.EndColorText;
      this.MenuEntries[16].Text = this.ProximityText;
      this.MenuEntries[17].Text = this.RequiresPowerText;
      this.MenuEntries[18].Text = this.TextureText;
    }

    private void OnParticleTypeSelected(string path, string name)
    {
      string s = path + name;
      if (!s.IsNotEmpty())
        return;
      int index = 0;
      while (index < this.particleTypes.Length && !s.Equals(this.particleTypes[index], StringComparison.OrdinalIgnoreCase))
        ++index;
      if (index >= this.particleTypes.Length)
        return;
      this.templateID = index;
      this.block.SetValuesFromTemplate(this.templateID);
      this.data = this.block.Data;
      this.templateName = this.data.Name;
      this.ResetMenuItemText();
    }

    private void OnSaveNameEntered(string path, string text)
    {
      if (!text.IsNotEmpty())
        return;
      if (text.Equals("new template", StringComparison.OrdinalIgnoreCase))
        Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Template Name", "Enter the name of the new template to save these emitter settings.", path, new AsyncCallback(this.OnNewSaveNameEntered), (object) null);
      else
        this.SaveAsTemplate(path + text);
    }

    private void OnNewSaveNameEntered(IAsyncResult ar)
    {
      string str = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      this.SaveAsTemplate(str);
    }

    private void SaveAsTemplate(string text)
    {
      if (text.StartsWith("system\\", StringComparison.OrdinalIgnoreCase))
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Cannot save templates to System folder", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
      {
        Globals2.AddParticleTemplate(text, this.data);
        this.templateName = text;
        this.ResetMenuItemText();
      }
    }

    private void OnDurationEntered(IAsyncResult ar)
    {
      string s = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!s.IsNotEmpty())
        return;
      float result;
      if (float.TryParse(s, out result))
      {
        this.data.Duration = (ushort) ((double) Math.Min(8f, Math.Max(0.0f, result)) * 1000.0);
        this.ResetMenuItemText();
      }
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid duration entered", this.ControllingPlayer.Value);
    }

    private void OnEmitFreqEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.EmitFreq = (int) (Math.Max(0.0, number) * 1000.0);
      this.ResetMenuItemText();
    }

    private void OnEmitPosOffsetEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Vector3? vector3FromToken = this.parser.GetVector3FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!vector3FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid position offset entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.EmitPosOffset = Vector3.Clamp(vector3FromToken.Value, Vector3.One * -50f, Vector3.One * 50f);
        this.ResetMenuItemText();
      }
    }

    private void OnEmitPosVarianceEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Vector3? vector3FromToken = this.parser.GetVector3FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!vector3FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid position variance entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.EmitPosVariance = Vector3.Clamp(vector3FromToken.Value, Vector3.One * -50f, Vector3.One * 50f);
        this.ResetMenuItemText();
      }
    }

    private void OnVelocityEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Vector3? vector3FromToken = this.parser.GetVector3FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!vector3FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid velocity entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.Velocity = Vector3.Clamp(vector3FromToken.Value, Vector3.One * -50f, Vector3.One * 50f);
        this.ResetMenuItemText();
      }
    }

    private void OnVelocityVarianceEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Vector3? vector3FromToken = this.parser.GetVector3FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!vector3FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid velocity variance entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.VelocityVariance = Vector3.Clamp(vector3FromToken.Value, Vector3.One * -50f, Vector3.One * 50f);
        this.ResetMenuItemText();
      }
    }

    private void OnGravityEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.Gravity = (short) MyMathHelper.Clamp((int) (number * 100.0), -5000, 5000);
      this.ResetMenuItemText();
    }

    private void OnRotationEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.Rotation = MathHelper.Clamp((float) number, -100f, 100f);
      this.ResetMenuItemText();
    }

    private void OnStartSizeEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Vector3? vector3FromToken = this.parser.GetVector3FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!vector3FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid start size entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.Size = new Vector4(Vector3.Clamp(vector3FromToken.Value, Vector3.One * -50f, Vector3.One * 50f), this.data.Size.W);
        this.ResetMenuItemText();
      }
    }

    private void OnEndSizeEntered(IAsyncResult ar)
    {
      string s = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!s.IsNotEmpty())
        return;
      float result;
      if (float.TryParse(s, out result))
      {
        this.data.Size.W = MathHelper.Clamp(result, -50f, 50f);
        this.ResetMenuItemText();
      }
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid end size entered", this.ControllingPlayer.Value);
    }

    private void OnWindFactorEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.WindFactor = MathHelper.Clamp((float) number, 0.0f, 10f);
      this.ResetMenuItemText();
    }

    private void OnProximityEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.Proximity = MathHelper.Clamp((float) number, 0.0f, 1000f);
      this.ResetMenuItemText();
    }

    private void OnStartColorEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Color? color4FromToken = this.parser.GetColor4FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!color4FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid color entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.StartColor = color4FromToken.Value;
        this.ResetMenuItemText();
      }
    }

    private void OnEndColorEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (!str.IsNotEmpty())
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Color? color4FromToken = this.parser.GetColor4FromToken(this.parser.GetNextToken(str, 0, char.MinValue, char.MinValue));
      if (!color4FromToken.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid color entered", this.ControllingPlayer.Value);
      }
      else
      {
        this.data.EndColor = color4FromToken.Value;
        this.ResetMenuItemText();
      }
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if ((double) this.copyToScriptTimer > 0.0)
      {
        this.copyToScriptTimer -= Services.ElapsedTime;
        if ((double) this.copyToScriptTimer <= 0.0)
          this.MenuEntries[this.MenuEntries.Count - 2].IsEnabled = true;
      }
      this.UpdateUsageText();
    }

    private void UpdateUsageText()
    {
      if (this.lastSpawnCount == this.instance.EmitterParticleSystem.SpawnedCount)
        return;
      this.lastSpawnCount = this.instance.EmitterParticleSystem.SpawnedCount;
      this.MenuEntries[0].Text = this.UsageText;
      this.MenuEntries[0].ColorDisabled = this.lastSpawnCount > (int) ((double) this.instance.EmitterParticleSystem.MaxParticles / (double) this.instance.EmitterParticleSystem.ParticleDuration) ? Color.Red : this.MenuEntries[0].ColorUnselected;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
