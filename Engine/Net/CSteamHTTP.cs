// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CSteamHTTP
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public class CSteamHTTP : ISteamHTTP
  {
    private IntPtr m_pSteamHTTP;

    public CSteamHTTP(IntPtr SteamHTTP)
    {
      this.m_pSteamHTTP = SteamHTTP;
    }

    public override IntPtr GetIntPtr()
    {
      return this.m_pSteamHTTP;
    }

    private void CheckIfUsable()
    {
      if (this.m_pSteamHTTP == IntPtr.Zero)
        throw new Exception("Steam Pointer not configured");
    }

    public override uint CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_CreateHTTPRequest(this.m_pSteamHTTP, eHTTPRequestMethod, pchAbsoluteURL);
    }

    public override bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(this.m_pSteamHTTP, hRequest, ulContextValue);
    }

    public override bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(this.m_pSteamHTTP, hRequest, unTimeoutSeconds);
    }

    public override bool SetHTTPRequestHeaderValue(
      uint hRequest,
      string pchHeaderName,
      string pchHeaderValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(this.m_pSteamHTTP, hRequest, pchHeaderName, pchHeaderValue);
    }

    public override bool SetHTTPRequestGetOrPostParameter(
      uint hRequest,
      string pchParamName,
      string pchParamValue)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(this.m_pSteamHTTP, hRequest, pchParamName, pchParamValue);
    }

    public override bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle)
    {
      this.CheckIfUsable();
      pCallHandle = 0UL;
      return NativeCalls.SteamAPI_ISteamHTTP_SendHTTPRequest(this.m_pSteamHTTP, hRequest, ref pCallHandle);
    }

    public override bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle)
    {
      this.CheckIfUsable();
      pCallHandle = 0UL;
      return NativeCalls.SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(this.m_pSteamHTTP, hRequest, ref pCallHandle);
    }

    public override bool DeferHTTPRequest(uint hRequest)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_DeferHTTPRequest(this.m_pSteamHTTP, hRequest);
    }

    public override bool PrioritizeHTTPRequest(uint hRequest)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(this.m_pSteamHTTP, hRequest);
    }

    public override bool GetHTTPResponseHeaderSize(
      uint hRequest,
      string pchHeaderName,
      ref uint unResponseHeaderSize)
    {
      this.CheckIfUsable();
      unResponseHeaderSize = 0U;
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(this.m_pSteamHTTP, hRequest, pchHeaderName, ref unResponseHeaderSize);
    }

    public override bool GetHTTPResponseHeaderValue(
      uint hRequest,
      string pchHeaderName,
      IntPtr pHeaderValueBuffer,
      uint unBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(this.m_pSteamHTTP, hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
    }

    public override bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize)
    {
      this.CheckIfUsable();
      unBodySize = 0U;
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(this.m_pSteamHTTP, hRequest, ref unBodySize);
    }

    public override bool GetHTTPResponseBodyData(
      uint hRequest,
      IntPtr pBodyDataBuffer,
      uint unBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(this.m_pSteamHTTP, hRequest, pBodyDataBuffer, unBufferSize);
    }

    public override bool GetHTTPStreamingResponseBodyData(
      uint hRequest,
      uint cOffset,
      IntPtr pBodyDataBuffer,
      uint unBufferSize)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(this.m_pSteamHTTP, hRequest, cOffset, pBodyDataBuffer, unBufferSize);
    }

    public override bool ReleaseHTTPRequest(uint hRequest)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_ReleaseHTTPRequest(this.m_pSteamHTTP, hRequest);
    }

    public override bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut)
    {
      this.CheckIfUsable();
      pflPercentOut = 0.0f;
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(this.m_pSteamHTTP, hRequest, ref pflPercentOut);
    }

    public override bool SetHTTPRequestRawPostBody(
      uint hRequest,
      string pchContentType,
      IntPtr pubBody,
      uint unBodyLen)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(this.m_pSteamHTTP, hRequest, pchContentType, pubBody, unBodyLen);
    }

    public override uint CreateCookieContainer(bool bAllowResponsesToModify)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_CreateCookieContainer(this.m_pSteamHTTP, bAllowResponsesToModify);
    }

    public override bool ReleaseCookieContainer(uint hCookieContainer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_ReleaseCookieContainer(this.m_pSteamHTTP, hCookieContainer);
    }

    public override bool SetCookie(
      uint hCookieContainer,
      string pchHost,
      string pchUrl,
      string pchCookie)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetCookie(this.m_pSteamHTTP, hCookieContainer, pchHost, pchUrl, pchCookie);
    }

    public override bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(this.m_pSteamHTTP, hRequest, hCookieContainer);
    }

    public override bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(this.m_pSteamHTTP, hRequest, pchUserAgentInfo);
    }

    public override bool SetHTTPRequestRequiresVerifiedCertificate(
      uint hRequest,
      bool bRequireVerifiedCertificate)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(this.m_pSteamHTTP, hRequest, bRequireVerifiedCertificate);
    }

    public override bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds)
    {
      this.CheckIfUsable();
      return NativeCalls.SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(this.m_pSteamHTTP, hRequest, unMilliseconds);
    }

    public override bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut)
    {
      this.CheckIfUsable();
      pbWasTimedOut = false;
      return NativeCalls.SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(this.m_pSteamHTTP, hRequest, ref pbWasTimedOut);
    }
  }
}
