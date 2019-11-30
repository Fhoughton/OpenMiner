// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.ITextInput
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Input;

namespace StudioForge.Engine.Integration
{
  public interface ITextInput : IInputHandler
  {
    int Cursor { get; }

    int SelectedTextCursorStart { get; }

    bool CanCarotBlink { get; }

    bool InputCompleted { get; }

    int MaxLength { get; set; }

    IInputHandler SniffHandler { get; set; }

    void InsertChar(char c);

    bool HandleInput(Keys key);

    void EndInput(bool needsValidation);

    void AbortInput();
  }
}
