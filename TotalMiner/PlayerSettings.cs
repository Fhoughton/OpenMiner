// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PlayerSettings
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class PlayerSettings
  {
    public static float AutoPlaceFast = 0.134f;
    public static float AutoPlaceVeryFast = 0.067f;
    public float GamePadSensitivity = 0.7f;
    public float MouseSensitivity = 0.3f;
    public float FOVNormalized = 0.6f;
    public bool CompassTop = true;
    public bool MapVisible = true;
    public bool RumbleOn = true;
    public NamePlateSetting Nameplates = NamePlateSetting.Short;
    public bool MobNameplates = true;
    public bool BlueprintFinderVisible = true;
    public bool Bobbing = true;
    public ActorType MobType = ActorType.Boy;
    public WieldType WieldType = WieldType.BothHands;
    public CameraType CameraType = CameraType.Momentum;
    public Color BackColor = Color.LightBlue * 0.8f;
    private bool hudVisible = true;
    public bool DisplayXPGains;
    public bool InvertY;
    public float AutoplaceTime;
    public byte HotBarToTransparentTime;
    public UserControlSetting UserControlSetting;
    private int hideHUDCount;

    public bool HudVisible
    {
      get
      {
        if (this.hudVisible)
          return this.hideHUDCount == 0;
        return false;
      }
      set
      {
        this.hudVisible = value;
      }
    }

    public bool HudVisibleForSettingsSave
    {
      get
      {
        return this.hudVisible;
      }
    }

    public void AddHideHUD()
    {
      ++this.hideHUDCount;
    }

    public void RemoveHideHUD()
    {
      --this.hideHUDCount;
      if (this.hideHUDCount >= 0)
        return;
      this.hideHUDCount = 0;
    }

    public PlayerSettings Clone()
    {
      return new PlayerSettings()
      {
        GamePadSensitivity = this.GamePadSensitivity,
        MouseSensitivity = this.MouseSensitivity,
        FOVNormalized = this.FOVNormalized,
        MapVisible = this.MapVisible,
        CompassTop = this.CompassTop,
        RumbleOn = this.RumbleOn,
        Nameplates = this.Nameplates,
        MobNameplates = this.MobNameplates,
        DisplayXPGains = this.DisplayXPGains,
        BlueprintFinderVisible = this.BlueprintFinderVisible,
        InvertY = this.InvertY,
        AutoplaceTime = this.AutoplaceTime,
        CameraType = this.CameraType,
        UserControlSetting = this.UserControlSetting,
        HotBarToTransparentTime = this.HotBarToTransparentTime,
        MobType = this.MobType,
        Bobbing = this.Bobbing,
        WieldType = this.WieldType,
        hudVisible = this.hudVisible,
        hideHUDCount = this.hideHUDCount,
        BackColor = this.BackColor
      };
    }

    public void ToggleAutoPlace()
    {
      if ((double) this.AutoplaceTime == 0.0)
        this.AutoplaceTime = PlayerSettings.AutoPlaceFast;
      else if ((double) this.AutoplaceTime == (double) PlayerSettings.AutoPlaceFast)
        this.AutoplaceTime = PlayerSettings.AutoPlaceVeryFast;
      else
        this.AutoplaceTime = 0.0f;
    }

    public void ToggleHotbarTransparency()
    {
      this.HotBarToTransparentTime = this.HotBarToTransparentTime > (byte) 0 ? (byte) 0 : (byte) 8;
    }

    public string GetAutoPlaceSettingText()
    {
      if ((double) this.AutoplaceTime == (double) PlayerSettings.AutoPlaceFast)
        return "Fast";
      return (double) this.AutoplaceTime == (double) PlayerSettings.AutoPlaceVeryFast ? "Very Fast" : "Normal";
    }

    public string GetHotbarTransparencyText()
    {
      return this.HotBarToTransparentTime <= (byte) 0 ? "Off" : "On";
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.GamePadSensitivity = reader.ReadSingle();
      if (version > 254)
        this.MouseSensitivity = reader.ReadSingle();
      this.FOVNormalized = reader.ReadSingle();
      this.RumbleOn = reader.ReadBoolean();
      this.DisplayXPGains = reader.ReadBoolean();
      this.InvertY = reader.ReadBoolean();
      this.AutoplaceTime = reader.ReadSingle();
      if (version < 194)
        this.AutoplaceTime = 0.0f;
      this.HotBarToTransparentTime = reader.ReadByte();
      this.CameraType = (CameraType) reader.ReadByte();
      this.UserControlSetting = (UserControlSetting) reader.ReadByte();
      this.BackColor = Color.LightBlue * 0.8f;
      if (version <= 164)
        return;
      this.CompassTop = reader.ReadBoolean();
      if (version > 171)
        this.MobType = (ActorType) reader.ReadByte();
      if (version > 191)
        this.WieldType = (WieldType) reader.ReadByte();
      if (version > 190)
        this.Bobbing = reader.ReadBoolean();
      if (version <= 286)
        return;
      this.BackColor.PackedValue = reader.ReadUInt32();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.GamePadSensitivity);
      writer.Write(this.MouseSensitivity);
      writer.Write(this.FOVNormalized);
      writer.Write(this.RumbleOn);
      writer.Write(this.DisplayXPGains);
      writer.Write(this.InvertY);
      writer.Write(this.AutoplaceTime);
      writer.Write(this.HotBarToTransparentTime);
      writer.Write((byte) this.CameraType);
      writer.Write((byte) this.UserControlSetting);
      writer.Write(this.CompassTop);
      writer.Write((byte) this.MobType);
      writer.Write((byte) this.WieldType);
      writer.Write(this.Bobbing);
      writer.Write(this.BackColor.PackedValue);
    }
  }
}
