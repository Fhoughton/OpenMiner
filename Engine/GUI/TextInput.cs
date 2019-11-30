// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.TextInput
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.GUI
{
  public class TextInput : ITextInput, IInputHandler
  {
    private string origText;
    private string Text;
    private int cursor;
    private int selectedTextCursorStart;
    private float delayTimer;
    private ITextInputWindow window;
    private string clipboard;
    private Keys currentKey;
    private bool shift;
    private bool ctrl;
    private bool alt;
    private bool capsLock;
    private bool newPress;
    private bool firstFrame;
    private bool initialEnter;
    private Func<Keys, bool, bool, bool> onKey;
    private Func<Keys[], bool> onLastKeys;

    bool IInputHandler.FullInputControl
    {
      get
      {
        if (this.SniffHandler != null)
          return this.SniffHandler.FullInputControl;
        return false;
      }
    }

    public int Cursor
    {
      get
      {
        return this.cursor;
      }
    }

    public int MaxLength { get; set; }

    public int SelectedTextCursorStart
    {
      get
      {
        return this.selectedTextCursorStart;
      }
    }

    public bool InputCompleted { get; protected set; }

    public bool CanCarotBlink
    {
      get
      {
        return true;
      }
    }

    public IInputHandler SniffHandler { get; set; }

    public void EndInput(bool needsValidation)
    {
      this.InputCompleted = true;
      this.window.EndInput(needsValidation);
    }

    public void AbortInput()
    {
      this.window.Text = this.origText;
      this.EndInput(false);
    }

    bool ITextInput.HandleInput(Keys key)
    {
      return this.HandleInput(key);
    }

    void ITextInput.InsertChar(char c)
    {
      this.InsertChar(c);
    }

    public TextInput()
      : this((Func<Keys, bool, bool, bool>) null, (Func<Keys[], bool>) null)
    {
    }

    public TextInput(Func<Keys, bool, bool, bool> onKey, Func<Keys[], bool> onLastKeys)
    {
      this.onKey = onKey;
      this.onLastKeys = onLastKeys;
      this.capsLock = false;
      this.selectedTextCursorStart = -1;
      this.firstFrame = true;
      InputManager.ResetInputs();
    }

    public void SetWindow(ITextInputWindow window)
    {
      this.window = window;
      this.origText = this.Text = window.Text;
      this.SetCursor(this.Text.IsNotEmpty() ? this.Text.Length : 0);
    }

    private void SetText(string text)
    {
      this.Text = text;
      if (this.window == null)
        return;
      this.window.Text = text;
    }

    public bool HandleInput(PlayerIndex playerIndex)
    {
      if (this.firstFrame)
      {
        this.initialEnter = InputManager.IsKeyPressed(playerIndex, Keys.Enter);
        this.firstFrame = false;
      }
      if (this.initialEnter)
      {
        if (InputManager.IsKeyPressed(playerIndex, Keys.Enter))
          return true;
        this.initialEnter = false;
        return true;
      }
      if (this.SniffHandler != null && this.SniffHandler.HandleInput(playerIndex))
        return true;
      Keys[] pressedKeys = InputManager.GetPressedKeys(playerIndex);
      Keys[] pressedKeysPrev = InputManager.GetPressedKeysPrev(playerIndex);
      if (pressedKeys != null && pressedKeys.Length > 0 || pressedKeysPrev != null && pressedKeysPrev.Length > 0)
      {
        if (this.window.OnRawInput != null)
        {
          bool endInput;
          bool flag = this.window.OnRawInput(this.window, out endInput);
          if (endInput)
          {
            this.EndInput(true);
            return true;
          }
          if (flag)
            return true;
        }
        if (pressedKeys != null && pressedKeys.Length > 0)
        {
          this.newPress = false;
          if (pressedKeys.Length == 1 && this.IgnoreKey(pressedKeys[0]))
          {
            this.currentKey = Keys.None;
          }
          else
          {
            foreach (Keys key in pressedKeys)
            {
              if (!this.IgnoreKey(key))
              {
                bool flag = false;
                foreach (Keys keys in pressedKeysPrev)
                {
                  if (keys == key)
                  {
                    flag = true;
                    break;
                  }
                }
                if (!flag)
                {
                  this.currentKey = key;
                  this.newPress = true;
                  this.delayTimer = 0.0f;
                  break;
                }
              }
            }
          }
          this.shift = this.IsKeyHeld(pressedKeys, Keys.LeftShift) || this.IsKeyHeld(pressedKeys, Keys.RightShift);
          this.ctrl = this.IsKeyHeld(pressedKeys, Keys.LeftControl) || this.IsKeyHeld(pressedKeys, Keys.RightControl);
          this.alt = this.IsKeyHeld(pressedKeys, Keys.LeftAlt) || this.IsKeyHeld(pressedKeys, Keys.RightAlt);
          if (this.currentKey != Keys.None)
          {
            this.delayTimer -= Services.ElapsedTime;
            if ((double) this.delayTimer <= 0.0)
            {
              this.delayTimer = this.newPress ? 0.6f : 0.06f;
              if (this.HandleInput(this.currentKey))
                return true;
            }
          }
        }
        else if (pressedKeysPrev != null && pressedKeysPrev.Length > 0 && this.ProcessLastKeys(pressedKeysPrev))
          return true;
      }
      else
      {
        this.shift = this.ctrl = this.alt = false;
        if (this.window.OnRawInput != null)
        {
          bool endInput;
          bool flag = this.window.OnRawInput(this.window, out endInput);
          if (!endInput)
            return flag;
          this.EndInput(true);
          return true;
        }
      }
      return false;
    }

    private bool ProcessLastKeys(Keys[] lastKeys)
    {
      if (this.onLastKeys != null)
        return this.onLastKeys(lastKeys);
      switch (lastKeys[0])
      {
        case Keys.Enter:
          this.EndInput(true);
          return true;
        case Keys.Escape:
          this.AbortInput();
          return true;
        default:
          return false;
      }
    }

    private void SetCursor(int c)
    {
      if (c > this.Text.Length)
        c = this.Text.Length;
      if (this.cursor == c)
        return;
      int cursor = this.cursor;
      this.cursor = c;
      this.window.CursorMoved(cursor, this.cursor);
    }

    private bool HandleInput(Keys key)
    {
      switch (key)
      {
        case Keys.Back:
          if (this.cursor > 0 || this.selectedTextCursorStart >= 0 && this.selectedTextCursorStart > this.cursor)
          {
            if (this.selectedTextCursorStart >= 0)
            {
              this.DeleteSelectedText();
            }
            else
            {
              this.SetText((this.cursor > 1 ? this.Text.Substring(0, this.cursor - 1) : (string) null) + (this.cursor < this.Text.Length ? this.Text.Substring(this.cursor) : (string) null));
              this.SetCursor(this.cursor - 1);
            }
          }
          return true;
        case Keys.CapsLock:
          this.capsLock = !this.capsLock;
          return true;
        case Keys.End:
          this.SetSelectedTextCursorStart(this.shift);
          if (this.Text != null)
            this.SetCursor(this.Text.Length);
          return true;
        case Keys.Home:
          this.SetSelectedTextCursorStart(this.shift);
          this.SetCursor(0);
          return true;
        case Keys.Left:
          this.SetSelectedTextCursorStart(this.shift);
          if (this.ctrl)
          {
            if (this.cursor > 0)
            {
              bool flag = false;
              int index = this.cursor - 1;
              while (index > 0 && this.Text[index] == ' ')
                --index;
              for (; index > 0 && !flag; --index)
                flag = this.Text[index] == ' ';
              this.SetCursor(flag ? index + 2 : index);
            }
          }
          else if (this.cursor > 0)
            this.SetCursor(this.cursor - 1);
          return true;
        case Keys.Right:
          this.SetSelectedTextCursorStart(this.shift);
          if (this.ctrl)
          {
            if (this.cursor < this.Text.Length)
            {
              bool flag = false;
              int c;
              for (c = this.cursor + 1; c < this.Text.Length && !flag; ++c)
                flag = this.Text[c] == ' ';
              while (flag && c < this.Text.Length && this.Text[c] == ' ')
                ++c;
              this.SetCursor(c);
            }
          }
          else if (this.Text != null && this.cursor < this.Text.Length)
            this.SetCursor(this.cursor + 1);
          return true;
        case Keys.Insert:
          if (this.clipboard.IsNotEmpty() && (this.MaxLength == 0 || this.Text.Length < this.MaxLength))
          {
            if (this.selectedTextCursorStart >= 0)
              this.DeleteSelectedText();
            int num = this.Text.Length + this.clipboard.Length;
            if (this.MaxLength > 0 && num > this.MaxLength)
              this.clipboard = this.clipboard.Substring(0, this.clipboard.Length - (num - this.MaxLength));
            this.SetText(this.Text.Substring(0, this.cursor) + this.clipboard + this.Text.Substring(this.cursor));
            this.SetCursor(this.cursor + this.clipboard.Length);
          }
          return true;
        case Keys.Delete:
          if (this.Text != null && (this.cursor < this.Text.Length || this.selectedTextCursorStart >= 0 && this.selectedTextCursorStart < this.cursor))
          {
            if (this.selectedTextCursorStart >= 0)
            {
              int startIndex = Math.Min(this.cursor, this.selectedTextCursorStart);
              int num = Math.Max(this.cursor, this.selectedTextCursorStart);
              this.clipboard = this.Text.Substring(startIndex, num - startIndex);
              this.DeleteSelectedText();
            }
            else
              this.SetText((this.cursor > 0 ? this.Text.Substring(0, this.cursor) : (string) null) + (this.cursor + 1 < this.Text.Length ? this.Text.Substring(this.cursor + 1) : (string) null));
          }
          return true;
        case Keys.C:
          if (this.ctrl)
          {
            if (this.selectedTextCursorStart >= 0)
            {
              int startIndex = Math.Min(this.cursor, this.selectedTextCursorStart);
              int num = Math.Max(this.cursor, this.selectedTextCursorStart);
              this.clipboard = this.Text.Substring(startIndex, num - startIndex);
            }
            return true;
          }
          break;
        case Keys.V:
          if (this.ctrl)
          {
            if (this.clipboard.IsNotEmpty())
            {
              if (this.selectedTextCursorStart >= 0)
                this.DeleteSelectedText();
              this.SetText(this.Text.Substring(0, this.cursor) + this.clipboard + this.Text.Substring(this.cursor));
              this.SetCursor(this.cursor + this.clipboard.Length);
            }
            return true;
          }
          break;
        default:
          if (this.onKey != null && this.onKey(key, this.shift, this.ctrl))
            return true;
          break;
      }
      if ((key < Keys.D0 || key > Keys.Z) && (key != Keys.Space && key != Keys.OemMinus) && (key != Keys.OemPlus && key != Keys.OemPeriod && (key != Keys.OemBackslash && key != Keys.OemComma)) && (key != Keys.OemQuestion && key != Keys.OemPipe && (key != Keys.OemQuotes && key != Keys.OemOpenBrackets) && (key != Keys.OemCloseBrackets && key != Keys.OemSemicolon)))
        return false;
      char c1 = (char) key;
      switch (key)
      {
        case Keys.OemSemicolon:
          c1 = this.shift ? ':' : ';';
          break;
        case Keys.OemPlus:
          c1 = this.shift ? '+' : '=';
          break;
        case Keys.OemComma:
          c1 = this.shift ? '<' : ',';
          break;
        case Keys.OemMinus:
          c1 = this.shift ? '_' : '-';
          break;
        case Keys.OemPeriod:
          c1 = this.shift ? '>' : '.';
          break;
        case Keys.OemQuestion:
          c1 = this.shift ? '?' : '/';
          break;
        case Keys.OemOpenBrackets:
          c1 = this.shift ? '{' : '[';
          break;
        case Keys.OemPipe:
        case Keys.OemBackslash:
          c1 = this.shift ? '|' : '\\';
          break;
        case Keys.OemCloseBrackets:
          c1 = this.shift ? '}' : ']';
          break;
        case Keys.OemQuotes:
          c1 = this.shift ? '"' : '\'';
          break;
      }
      bool flag1 = this.capsLock;
      if (this.shift)
      {
        flag1 = !flag1;
        switch (key)
        {
          case Keys.D0:
            c1 = ')';
            break;
          case Keys.D1:
            c1 = '!';
            break;
          case Keys.D2:
            c1 = '@';
            break;
          case Keys.D3:
            c1 = '#';
            break;
          case Keys.D4:
            c1 = '$';
            break;
          case Keys.D5:
            c1 = '%';
            break;
          case Keys.D6:
            c1 = '^';
            break;
          case Keys.D7:
            c1 = '&';
            break;
          case Keys.D8:
            c1 = '*';
            break;
          case Keys.D9:
            c1 = '(';
            break;
        }
      }
      if (!flag1 && key >= Keys.A && key <= Keys.Z)
        c1 = (char) ((uint) (byte) key + 32U);
      if (this.selectedTextCursorStart >= 0)
        this.DeleteSelectedText();
      this.InsertChar(c1);
      return true;
    }

    private void InsertChar(char c)
    {
      if (c <= char.MinValue || this.MaxLength != 0 && this.Text.Length >= this.MaxLength)
        return;
      if (this.cursor == 0)
        this.SetText(((int) c).ToString() + this.Text);
      else if (this.cursor == this.Text.Length)
        this.SetText(this.Text + (object) c);
      else
        this.SetText(this.Text.Substring(0, this.cursor) + (object) c + this.Text.Substring(this.cursor));
      this.SetCursor(this.cursor + 1);
    }

    private Keys GetNonCtrlKey(Keys[] keys)
    {
      for (int index = 0; index < keys.Length; ++index)
      {
        if (!this.IgnoreKey(keys[index]) || keys[index] == this.currentKey)
          return keys[index];
      }
      return Keys.None;
    }

    private bool IgnoreKey(Keys key)
    {
      switch (key)
      {
        case Keys.LeftWindows:
        case Keys.RightWindows:
        case Keys.LeftShift:
        case Keys.RightShift:
        case Keys.LeftControl:
        case Keys.RightControl:
        case Keys.LeftAlt:
        case Keys.RightAlt:
          return true;
        default:
          return false;
      }
    }

    private void SetSelectedTextCursorStart(bool shift)
    {
      if (shift)
      {
        if (this.selectedTextCursorStart != -1)
          return;
        this.selectedTextCursorStart = this.cursor;
      }
      else
        this.selectedTextCursorStart = -1;
    }

    private void DeleteSelectedText()
    {
      int num = Math.Min(this.cursor, this.selectedTextCursorStart);
      int startIndex = Math.Max(this.cursor, this.selectedTextCursorStart);
      this.SetText((num > 0 ? this.Text.Substring(0, num) : (string) null) + (startIndex < this.Text.Length ? this.Text.Substring(startIndex) : (string) null));
      this.SetCursor(num);
      this.selectedTextCursorStart = -1;
    }

    private bool IsKeyHeld(Keys[] keys, Keys key)
    {
      for (int index = 0; index < keys.Length; ++index)
      {
        if (keys[index] == key)
          return true;
      }
      return false;
    }
  }
}
