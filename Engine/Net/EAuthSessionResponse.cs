// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.EAuthSessionResponse
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

namespace StudioForge.Engine.Net
{
  public enum EAuthSessionResponse
  {
    k_EAuthSessionResponseOK,
    k_EAuthSessionResponseUserNotConnectedToSteam,
    k_EAuthSessionResponseNoLicenseOrExpired,
    k_EAuthSessionResponseVACBanned,
    k_EAuthSessionResponseLoggedInElseWhere,
    k_EAuthSessionResponseVACCheckTimedOut,
    k_EAuthSessionResponseAuthTicketCanceled,
    k_EAuthSessionResponseAuthTicketInvalidAlreadyUsed,
    k_EAuthSessionResponseAuthTicketInvalid,
    k_EAuthSessionResponsePublisherIssuedBan,
  }
}
