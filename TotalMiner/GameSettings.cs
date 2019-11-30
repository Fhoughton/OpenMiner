// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GameSettings
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner
{
  internal class GameSettings
  {
    public string WindowBorder = "Blade1";
    public float SoundVolume = 0.5f;
    public float MusicVolume = 0.5f;
    public float ViewDistance = Globals2.DefaultViewDistance;
    public float TextureSmoothing = Globals2.DefaultTextureSmoothing;
    public ShaderDetail ShaderDetail = ShaderDetail.High;
    public bool ViewClouds = true;
    public bool FloraAnimation = true;
    public bool ToolTips = true;
    public NotificationType Notifications = NotificationType.Visual | NotificationType.Audio | NotificationType.Song | NotificationType.TextMsg;
    public bool UseMipMaps = true;
    public LeafMeshType LeafMesh = LeafMeshType.Below;
    public AutoSaveSetting AutoSave = AutoSaveSetting.Every15Minutes;
    public string TexturePack;
    public bool ViewZones;
    public bool ViewSounds;
    public bool SplitScreenVertical;
    public bool OldskoolLight;
    public bool ShadowMaps;

    public GameSettings Clone()
    {
      return new GameSettings()
      {
        TexturePack = this.TexturePack,
        WindowBorder = this.WindowBorder,
        SoundVolume = this.SoundVolume,
        MusicVolume = this.MusicVolume,
        ViewDistance = this.ViewDistance,
        TextureSmoothing = this.TextureSmoothing,
        ShaderDetail = this.ShaderDetail,
        ViewClouds = this.ViewClouds,
        ViewSounds = this.ViewSounds,
        ViewZones = this.ViewZones,
        FloraAnimation = this.FloraAnimation,
        ToolTips = this.ToolTips,
        SplitScreenVertical = this.SplitScreenVertical,
        Notifications = this.Notifications,
        OldskoolLight = this.OldskoolLight,
        UseMipMaps = this.UseMipMaps,
        LeafMesh = this.LeafMesh,
        AutoSave = this.AutoSave
      };
    }

    public bool HasNotification(NotificationType type)
    {
      return (this.Notifications & type) == type;
    }

    public void ToggleNotification(NotificationType type)
    {
      if ((this.Notifications & type) != type)
        this.Notifications |= type;
      else
        this.Notifications &= ~type;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      if (version <= 164)
        return;
      this.AutoSave = (AutoSaveSetting) reader.ReadByte();
      this.ShaderDetail = (ShaderDetail) reader.ReadByte();
      this.SoundVolume = reader.ReadSingle();
      this.MusicVolume = reader.ReadSingle();
      this.ViewClouds = reader.ReadBoolean();
      this.ViewSounds = version > 277 && reader.ReadBoolean();
      this.ViewZones = version > 277 && reader.ReadBoolean();
      this.FloraAnimation = version <= 227 || reader.ReadBoolean();
      this.ToolTips = version <= 228 || reader.ReadBoolean();
      this.UseMipMaps = reader.ReadBoolean();
      this.OldskoolLight = reader.ReadBoolean();
      if (version > 177)
      {
        this.Notifications = (NotificationType) reader.ReadByte();
        if (version < 231)
          this.Notifications |= NotificationType.TextMsg;
      }
      else
      {
        this.Notifications = NotificationType.TextMsg;
        if (reader.ReadBoolean())
          this.Notifications = NotificationType.Visual | NotificationType.TextMsg;
        if (version > 176 && reader.ReadBoolean())
          this.ToggleNotification(NotificationType.Audio);
      }
      this.SplitScreenVertical = reader.ReadBoolean();
      this.TextureSmoothing = reader.ReadSingle();
      this.ViewDistance = reader.ReadSingle();
      if (version > 276)
        this.LeafMesh = (LeafMeshType) reader.ReadUInt16();
      if (version <= 175)
        return;
      this.TexturePack = reader.ReadString();
      this.WindowBorder = reader.ReadString();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write((byte) this.AutoSave);
      writer.Write((byte) this.ShaderDetail);
      writer.Write(this.SoundVolume);
      writer.Write(this.MusicVolume);
      writer.Write(this.ViewClouds);
      writer.Write(this.ViewSounds);
      writer.Write(this.ViewZones);
      writer.Write(this.FloraAnimation);
      writer.Write(this.ToolTips);
      writer.Write(this.UseMipMaps);
      writer.Write(this.OldskoolLight);
      writer.Write((byte) this.Notifications);
      writer.Write(this.SplitScreenVertical);
      writer.Write(this.TextureSmoothing);
      writer.Write(this.ViewDistance);
      writer.Write((ushort) this.LeafMesh);
      writer.Write(this.TexturePack != null ? this.TexturePack : "");
      writer.Write(this.WindowBorder);
    }
  }
}
