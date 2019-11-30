// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamHTMLSurface
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamHTMLSurface
  {
    public abstract IntPtr GetIntPtr();

    public abstract void DestructISteamHTMLSurface();

    public abstract bool Init();

    public abstract bool Shutdown();

    public abstract ulong CreateBrowser(string pchUserAgent, string pchUserCSS);

    public abstract void RemoveBrowser(uint unBrowserHandle);

    public abstract void LoadURL(uint unBrowserHandle, string pchURL, string pchPostData);

    public abstract void SetSize(uint unBrowserHandle, uint unWidth, uint unHeight);

    public abstract void StopLoad(uint unBrowserHandle);

    public abstract void Reload(uint unBrowserHandle);

    public abstract void GoBack(uint unBrowserHandle);

    public abstract void GoForward(uint unBrowserHandle);

    public abstract void AddHeader(uint unBrowserHandle, string pchKey, string pchValue);

    public abstract void ExecuteJavascript(uint unBrowserHandle, string pchScript);

    public abstract void MouseUp(uint unBrowserHandle, uint eMouseButton);

    public abstract void MouseDown(uint unBrowserHandle, uint eMouseButton);

    public abstract void MouseDoubleClick(uint unBrowserHandle, uint eMouseButton);

    public abstract void MouseMove(uint unBrowserHandle, int x, int y);

    public abstract void MouseWheel(uint unBrowserHandle, int nDelta);

    public abstract void KeyDown(uint unBrowserHandle, uint nNativeKeyCode, uint eHTMLKeyModifiers);

    public abstract void KeyUp(uint unBrowserHandle, uint nNativeKeyCode, uint eHTMLKeyModifiers);

    public abstract void KeyChar(uint unBrowserHandle, uint cUnicodeChar, uint eHTMLKeyModifiers);

    public abstract void SetHorizontalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll);

    public abstract void SetVerticalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll);

    public abstract void SetKeyFocus(uint unBrowserHandle, bool bHasKeyFocus);

    public abstract void ViewSource(uint unBrowserHandle);

    public abstract void CopyToClipboard(uint unBrowserHandle);

    public abstract void PasteFromClipboard(uint unBrowserHandle);

    public abstract void Find(
      uint unBrowserHandle,
      string pchSearchStr,
      bool bCurrentlyInFind,
      bool bReverse);

    public abstract void StopFind(uint unBrowserHandle);

    public abstract void GetLinkAtPosition(uint unBrowserHandle, int x, int y);

    public abstract void SetCookie(
      string pchHostname,
      string pchKey,
      string pchValue,
      string pchPath,
      ulong nExpires,
      bool bSecure,
      bool bHTTPOnly);

    public abstract void SetPageScaleFactor(
      uint unBrowserHandle,
      float flZoom,
      int nPointX,
      int nPointY);

    public abstract void SetBackgroundMode(uint unBrowserHandle, bool bBackgroundMode);

    public abstract void AllowStartRequest(uint unBrowserHandle, bool bAllowed);

    public abstract void JSDialogResponse(uint unBrowserHandle, bool bResult);

    public abstract void FileLoadDialogResponse(uint unBrowserHandle, string pchSelectedFiles);
  }
}
