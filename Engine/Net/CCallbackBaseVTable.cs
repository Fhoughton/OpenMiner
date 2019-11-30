// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.CCallbackBaseVTable
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  [StructLayout(LayoutKind.Sequential)]
  internal class CCallbackBaseVTable
  {
    [MarshalAs(UnmanagedType.FunctionPtr)]
    [NonSerialized]
    public CCallbackBaseVTable.RunCallResultDelegate runCallResult;
    [MarshalAs(UnmanagedType.FunctionPtr)]
    [NonSerialized]
    public CCallbackBaseVTable.RunCallbackDelegate runCallback;
    [MarshalAs(UnmanagedType.FunctionPtr)]
    [NonSerialized]
    public CCallbackBaseVTable.GetCallbackSizeBytesDelegate getCallbackSize;

    public CCallbackBaseVTable(
      CCallbackBaseVTable.RunCallbackDelegate cbDel,
      CCallbackBaseVTable.RunCallResultDelegate crDel,
      CCallbackBaseVTable.GetCallbackSizeBytesDelegate getCbSizeDel)
    {
      this.runCallback = cbDel;
      this.runCallResult = crDel;
      this.getCallbackSize = getCbSizeDel;
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void RunCallbackDelegate(IntPtr thisPtr, IntPtr pvParam);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void RunCallResultDelegate(
      IntPtr thisPtr,
      IntPtr pvParam,
      [MarshalAs(UnmanagedType.I1)] bool bIOFailure,
      ulong hAPICall);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate int GetCallbackSizeBytesDelegate(IntPtr thisPtr);
  }
}
