// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamHTMLSurface
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamHTMLSurface : ISteamHTMLSurface
  {
    private IntPtr m_pSteamHTMLSurface;

    public CSteamHTMLSurface(IntPtr SteamHTMLSurface)
    {
      this.m_pSteamHTMLSurface = SteamHTMLSurface;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamHTMLSurface;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamHTMLSurface == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override void DestructISteamHTMLSurface()
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_DestructISteamHTMLSurface(this.m_pSteamHTMLSurface);
    }

    public override bool Init()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTMLSurface_Init(this.m_pSteamHTMLSurface);
    }

    public override bool Shutdown()
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTMLSurface_Shutdown(this.m_pSteamHTMLSurface);
    }

    public override ulong CreateBrowser(string pchUserAgent, string pchUserCSS)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTMLSurface_CreateBrowser(this.m_pSteamHTMLSurface, pchUserAgent, pchUserCSS);
    }

    public override void RemoveBrowser(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_RemoveBrowser(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void LoadURL(uint unBrowserHandle, string pchURL, string pchPostData)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_LoadURL(this.m_pSteamHTMLSurface, unBrowserHandle, pchURL, pchPostData);
    }

    public override void SetSize(uint unBrowserHandle, uint unWidth, uint unHeight)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetSize(this.m_pSteamHTMLSurface, unBrowserHandle, unWidth, unHeight);
    }

    public override void StopLoad(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_StopLoad(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void Reload(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_Reload(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void GoBack(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_GoBack(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void GoForward(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_GoForward(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void AddHeader(uint unBrowserHandle, string pchKey, string pchValue)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_AddHeader(this.m_pSteamHTMLSurface, unBrowserHandle, pchKey, pchValue);
    }

    public override void ExecuteJavascript(uint unBrowserHandle, string pchScript)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_ExecuteJavascript(this.m_pSteamHTMLSurface, unBrowserHandle, pchScript);
    }

    public override void MouseUp(uint unBrowserHandle, uint eMouseButton)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_MouseUp(this.m_pSteamHTMLSurface, unBrowserHandle, eMouseButton);
    }

    public override void MouseDown(uint unBrowserHandle, uint eMouseButton)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_MouseDown(this.m_pSteamHTMLSurface, unBrowserHandle, eMouseButton);
    }

    public override void MouseDoubleClick(uint unBrowserHandle, uint eMouseButton)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_MouseDoubleClick(this.m_pSteamHTMLSurface, unBrowserHandle, eMouseButton);
    }

    public override void MouseMove(uint unBrowserHandle, int x, int y)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_MouseMove(this.m_pSteamHTMLSurface, unBrowserHandle, x, y);
    }

    public override void MouseWheel(uint unBrowserHandle, int nDelta)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_MouseWheel(this.m_pSteamHTMLSurface, unBrowserHandle, nDelta);
    }

    public override void KeyDown(uint unBrowserHandle, uint nNativeKeyCode, uint eHTMLKeyModifiers)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_KeyDown(this.m_pSteamHTMLSurface, unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
    }

    public override void KeyUp(uint unBrowserHandle, uint nNativeKeyCode, uint eHTMLKeyModifiers)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_KeyUp(this.m_pSteamHTMLSurface, unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
    }

    public override void KeyChar(uint unBrowserHandle, uint cUnicodeChar, uint eHTMLKeyModifiers)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_KeyChar(this.m_pSteamHTMLSurface, unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
    }

    public override void SetHorizontalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetHorizontalScroll(this.m_pSteamHTMLSurface, unBrowserHandle, nAbsolutePixelScroll);
    }

    public override void SetVerticalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetVerticalScroll(this.m_pSteamHTMLSurface, unBrowserHandle, nAbsolutePixelScroll);
    }

    public override void SetKeyFocus(uint unBrowserHandle, bool bHasKeyFocus)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetKeyFocus(this.m_pSteamHTMLSurface, unBrowserHandle, bHasKeyFocus);
    }

    public override void ViewSource(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_ViewSource(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void CopyToClipboard(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_CopyToClipboard(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void PasteFromClipboard(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_PasteFromClipboard(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void Find(
      uint unBrowserHandle,
      string pchSearchStr,
      bool bCurrentlyInFind,
      bool bReverse)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_Find(this.m_pSteamHTMLSurface, unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
    }

    public override void StopFind(uint unBrowserHandle)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_StopFind(this.m_pSteamHTMLSurface, unBrowserHandle);
    }

    public override void GetLinkAtPosition(uint unBrowserHandle, int x, int y)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_GetLinkAtPosition(this.m_pSteamHTMLSurface, unBrowserHandle, x, y);
    }

    public override void SetCookie(
      string pchHostname,
      string pchKey,
      string pchValue,
      string pchPath,
      ulong nExpires,
      bool bSecure,
      bool bHTTPOnly)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetCookie(this.m_pSteamHTMLSurface, pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure, bHTTPOnly);
    }

    public override void SetPageScaleFactor(
      uint unBrowserHandle,
      float flZoom,
      int nPointX,
      int nPointY)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetPageScaleFactor(this.m_pSteamHTMLSurface, unBrowserHandle, flZoom, nPointX, nPointY);
    }

    public override void SetBackgroundMode(uint unBrowserHandle, bool bBackgroundMode)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_SetBackgroundMode(this.m_pSteamHTMLSurface, unBrowserHandle, bBackgroundMode);
    }

    public override void AllowStartRequest(uint unBrowserHandle, bool bAllowed)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_AllowStartRequest(this.m_pSteamHTMLSurface, unBrowserHandle, bAllowed);
    }

    public override void JSDialogResponse(uint unBrowserHandle, bool bResult)
    {
      this.CheckIfUsable();
      NativeCalls.SteamAPI_ISteamHTMLSurface_JSDialogResponse(this.m_pSteamHTMLSurface, unBrowserHandle, bResult);
    }

    public override void FileLoadDialogResponse(uint unBrowserHandle, string pchSelectedFiles)
    {
      this.CheckIfUsable();
      pchSelectedFiles = "";
      NativeCalls.SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse(this.m_pSteamHTMLSurface, unBrowserHandle, pchSelectedFiles);
    }
  }
}
