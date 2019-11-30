// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Guide
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using System;
using System.Threading;

namespace StudioForge.Engine.Core
{
  public static class Guide
  {
    public static IAsyncResult BeginShowKeyboardInput(
      ScreenManager screenManager,
      PlayerIndex player,
      string title,
      string description,
      string defaultText,
      AsyncCallback callback,
      object state)
    {
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      Rectangle rect = new Rectangle(0, 0, 640, 360);
      rect.X = viewport.Width / 2 - rect.Width / 2;
      rect.Y = viewport.Height / 2 - rect.Height / 2;
      return Guide.BeginShowKeyboardInput(screenManager, player, title, description, defaultText, callback, state, rect, 0.6f, false);
    }

    public static IAsyncResult BeginShowKeyboardInput(
      ScreenManager screenManager,
      PlayerIndex player,
      string title,
      string description,
      string defaultText,
      AsyncCallback callback,
      object state,
      MenuEntry menuEntry,
      bool numbersOnly)
    {
      if (menuEntry == null)
        return Guide.BeginShowKeyboardInput(screenManager, player, title, description, defaultText, callback, state);
      float itemTextScale = menuEntry.Screen.ItemTextScale;
      Rectangle lastHighLightRect = menuEntry.LastHighLightRect;
      int num1 = menuEntry.Text.IndexOf(':');
      int num2 = num1 < 0 ? -12 : (int) ((double) menuEntry.Screen.ItemFont.MeasureString(menuEntry.Text.Substring(0, num1 + 1)).X * (double) itemTextScale);
      int x = lastHighLightRect.X;
      lastHighLightRect.X += num2 + 5 + (int) menuEntry.TextOffset.X;
      lastHighLightRect.Width -= lastHighLightRect.X - x;
      return Guide.BeginShowKeyboardInput(screenManager, player, title, description, defaultText, callback, state, lastHighLightRect, itemTextScale, numbersOnly);
    }

    public static IAsyncResult BeginShowKeyboardInput(
      ScreenManager screenManager,
      PlayerIndex player,
      string title,
      string description,
      string defaultText,
      AsyncCallback callback,
      object state,
      Rectangle rect,
      float scale,
      bool numbersOnly)
    {
      DataFieldScreen screen = new DataFieldScreen(title, description, defaultText, callback, state, rect, scale, new TextInput());
      screenManager.AddScreen((GameScreen) screen, new PlayerIndex?(player));
      if (InputManager.IsUsingGamePad)
        Guide.OpenVirtualKeyboard(screen, numbersOnly);
      return (IAsyncResult) Guide.AsyncResult.Empty;
    }

    private static void OpenVirtualKeyboard(DataFieldScreen screen, bool numbersOnly)
    {
      ITextInput inputHandler = ((ITextInputWindow) screen).InputHandler;
      VirtualKeyboardCarousal keyboardCarousal = new VirtualKeyboardCarousal((string) null, 0, 0, CoreGlobals.GraphicsDevice.Viewport.Width, 60, inputHandler, numbersOnly);
      keyboardCarousal.Name = "keyboard";
      keyboardCarousal.Colors = (Window.ColorProfile) Colors.PauseMenuKeyboard;
      if (inputHandler != null)
        inputHandler.SniffHandler = (IInputHandler) keyboardCarousal;
      screen.WindowManager.Root.AddChild((Node) keyboardCarousal);
    }

    public static string EndShowKeyboardInput(IAsyncResult result)
    {
      return ((Guide.AsyncResult) result).AsyncString;
    }

    public struct AsyncResult : IAsyncResult
    {
      private object state;
      private WaitHandle handle;
      private bool completedSynchronously;
      private bool isCompleted;
      private string resultString;

      public object AsyncState
      {
        get
        {
          return this.state;
        }
      }

      public WaitHandle AsyncWaitHandle
      {
        get
        {
          return this.handle;
        }
      }

      public bool CompletedSynchronously
      {
        get
        {
          return this.completedSynchronously;
        }
      }

      public bool IsCompleted
      {
        get
        {
          return this.isCompleted;
        }
      }

      public string AsyncString
      {
        get
        {
          return this.resultString;
        }
        set
        {
          this.resultString = value;
        }
      }

      public static Guide.AsyncResult Empty
      {
        get
        {
          return new Guide.AsyncResult();
        }
      }

      public AsyncResult(object state)
      {
        this.state = state;
        this.handle = (WaitHandle) new Guide.GuideWaitHandle();
        this.isCompleted = false;
        this.completedSynchronously = false;
        this.resultString = (string) null;
      }
    }

    public class GuideWaitHandle : WaitHandle
    {
    }
  }
}
