// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamWarningMessageHookDelegate
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System.Runtime.InteropServices;
using System.Text;

namespace StudioForge.Engine.Net
{
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void SteamWarningMessageHookDelegate(int severity, StringBuilder builder);
}
