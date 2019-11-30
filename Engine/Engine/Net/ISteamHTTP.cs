// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.ISteamHTTP
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;

namespace StudioForge.Engine.Net
{
  public abstract class ISteamHTTP
  {
    public abstract IntPtr GetIntPtr();

    public abstract uint CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL);

    public abstract bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue);

    public abstract bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds);

    public abstract bool SetHTTPRequestHeaderValue(
      uint hRequest,
      string pchHeaderName,
      string pchHeaderValue);

    public abstract bool SetHTTPRequestGetOrPostParameter(
      uint hRequest,
      string pchParamName,
      string pchParamValue);

    public abstract bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle);

    public abstract bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle);

    public abstract bool DeferHTTPRequest(uint hRequest);

    public abstract bool PrioritizeHTTPRequest(uint hRequest);

    public abstract bool GetHTTPResponseHeaderSize(
      uint hRequest,
      string pchHeaderName,
      ref uint unResponseHeaderSize);

    public abstract bool GetHTTPResponseHeaderValue(
      uint hRequest,
      string pchHeaderName,
      IntPtr pHeaderValueBuffer,
      uint unBufferSize);

    public abstract bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize);

    public abstract bool GetHTTPResponseBodyData(
      uint hRequest,
      IntPtr pBodyDataBuffer,
      uint unBufferSize);

    public abstract bool GetHTTPStreamingResponseBodyData(
      uint hRequest,
      uint cOffset,
      IntPtr pBodyDataBuffer,
      uint unBufferSize);

    public abstract bool ReleaseHTTPRequest(uint hRequest);

    public abstract bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut);

    public abstract bool SetHTTPRequestRawPostBody(
      uint hRequest,
      string pchContentType,
      IntPtr pubBody,
      uint unBodyLen);

    public abstract uint CreateCookieContainer(bool bAllowResponsesToModify);

    public abstract bool ReleaseCookieContainer(uint hCookieContainer);

    public abstract bool SetCookie(
      uint hCookieContainer,
      string pchHost,
      string pchUrl,
      string pchCookie);

    public abstract bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer);

    public abstract bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo);

    public abstract bool SetHTTPRequestRequiresVerifiedCertificate(
      uint hRequest,
      bool bRequireVerifiedCertificate);

    public abstract bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds);

    public abstract bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut);
  }
}
