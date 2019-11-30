// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.PropertyEditorFieldAttribute
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using System;

namespace StudioForge.Engine.GUI
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class PropertyEditorFieldAttribute : Attribute
  {
    public string Name;
    public PropertyEditorFieldAttribute.FlagTypes Flags;

    public bool IsVisible
    {
      get
      {
        return (this.Flags & PropertyEditorFieldAttribute.FlagTypes.IsVisible) > PropertyEditorFieldAttribute.FlagTypes.None;
      }
    }

    public bool IsEditable
    {
      get
      {
        return (this.Flags & PropertyEditorFieldAttribute.FlagTypes.IsEditable) > PropertyEditorFieldAttribute.FlagTypes.None;
      }
    }

    public bool IsCSV
    {
      get
      {
        return (this.Flags & PropertyEditorFieldAttribute.FlagTypes.IsCSV) > PropertyEditorFieldAttribute.FlagTypes.None;
      }
    }

    public PropertyEditorFieldAttribute(string name)
      : this(name, PropertyEditorFieldAttribute.FlagTypes.IsCSV)
    {
    }

    public PropertyEditorFieldAttribute(PropertyEditorFieldAttribute.FlagTypes flags)
      : this((string) null, flags)
    {
    }

    public PropertyEditorFieldAttribute(string name, PropertyEditorFieldAttribute.FlagTypes flags)
    {
      this.Name = name;
      this.Flags = flags;
      if (this.IsEditable)
        this.Flags |= PropertyEditorFieldAttribute.FlagTypes.IsVisible;
      if (!this.IsCSV)
        return;
      this.Flags |= PropertyEditorFieldAttribute.FlagTypes.IsCSV;
    }

    [System.Flags]
    public enum FlagTypes
    {
      None = 0,
      IsVisible = 1,
      IsEditable = 2,
      IsCSV = IsEditable | IsVisible, // 0x00000003
      NotVisible = 0,
    }
  }
}
