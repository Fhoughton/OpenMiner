// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.IPropertyEditorControl
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

namespace StudioForge.Engine.GUI
{
  public interface IPropertyEditorControl
  {
    string ToString(string propertyName, object data);

    object Validate(string propertyName, string input, out string adjustedInput);

    bool IsPropertyEnabled(string name);

    void SetPropertyDefaults();

    void SetPropertyEditorDefaults(string name, Window win);
  }
}
