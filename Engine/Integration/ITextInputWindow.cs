// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.ITextInputWindow
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using System;

namespace StudioForge.Engine.Integration
{
  public interface ITextInputWindow
  {
    float TextScale { get; }

    string Text { get; set; }

    object Tag { get; }

    ITextInput InputHandler { get; }

    Action<ITextInputWindow> OnBeginInput { get; set; }

    Action<ITextInputWindow> OnValidateInput { get; set; }

    RawInputFunc OnRawInput { get; set; }

    void CursorMoved(int oldPos, int newPos);

    bool EqualsInputWindow(object win);

    void EndInput(bool needValidate);

    ITextInput GetNewTextInputHandlerOnClick();

    ITextInput GetNewTextInputHandlerOnHover();
  }
}
