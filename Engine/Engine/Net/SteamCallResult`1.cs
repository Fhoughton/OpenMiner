// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamCallResult`1
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  internal class SteamCallResult<T>
  {
    private readonly int structureSize = Marshal.SizeOf(typeof (T));
    private const ulong STEAMAPICALLHANDLE_INVALID = 0;
    private CCallbackBaseVTable vTable;
    private IntPtr vTablePointer;
    private CCallbackBase callback;
    private GCHandle callbackPointer;
    private ulong steamAPICallHandle;

    private event SteamCallResult<T>.DispatchDelegate function;

    public SteamCallResult(SteamCallResult<T>.DispatchDelegate func)
    {
      this.function = func;
      this.vTable = new CCallbackBaseVTable(new CCallbackBaseVTable.RunCallbackDelegate(this.OnRunCallback), new CCallbackBaseVTable.RunCallResultDelegate(this.OnRunCallResult), new CCallbackBaseVTable.GetCallbackSizeBytesDelegate(this.OnGetCallbackSizeBytes));
      this.vTablePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (CCallbackBaseVTable)));
      Marshal.StructureToPtr((object) this.vTable, this.vTablePointer, false);
      this.callback = new CCallbackBase()
      {
        m_vTable = this.vTablePointer,
        m_nCallbackFlags = (byte) 0,
        m_iCallback = CallbackIdentities.GetCallbackIdentity(typeof (T))
      };
      this.callbackPointer = GCHandle.Alloc((object) this.callback, GCHandleType.Pinned);
    }

    ~SteamCallResult()
    {
      this.Cancel();
      if (this.vTablePointer != IntPtr.Zero)
        Marshal.FreeHGlobal(this.vTablePointer);
      if (!this.callbackPointer.IsAllocated)
        return;
      this.callbackPointer.Free();
    }

    public void Set(ulong hAPICall)
    {
      if (this.function == null)
        throw new Exception("CallResult function was null, you must either set it in the CallResult Constructor or in Set()");
      if (this.steamAPICallHandle != 0UL)
        SteamAPI.UnregisterCallResult(this.callbackPointer.AddrOfPinnedObject(), this.steamAPICallHandle);
      this.steamAPICallHandle = hAPICall;
      if (hAPICall == 0UL)
        return;
      SteamAPI.RegisterCallResult(this.callbackPointer.AddrOfPinnedObject(), hAPICall);
    }

    public void Set(ulong hAPICall, SteamCallResult<T>.DispatchDelegate func)
    {
      if (func != null)
        this.function = func;
      if (this.function == null)
        throw new Exception("CallResult function was null, you must either set it in the CallResult Constructor or in Set()");
      if (this.steamAPICallHandle != 0UL)
        SteamAPI.UnregisterCallResult(this.callbackPointer.AddrOfPinnedObject(), this.steamAPICallHandle);
      this.steamAPICallHandle = hAPICall;
      if (hAPICall == 0UL)
        return;
      SteamAPI.RegisterCallResult(this.callbackPointer.AddrOfPinnedObject(), hAPICall);
    }

    public bool IsActive()
    {
      return this.steamAPICallHandle != 0UL;
    }

    public void Cancel()
    {
      if (this.steamAPICallHandle == 0UL)
        return;
      SteamAPI.UnregisterCallResult(this.callbackPointer.AddrOfPinnedObject(), this.steamAPICallHandle);
      this.steamAPICallHandle = 0UL;
    }

    private void OnRunCallback(IntPtr thisptr, IntPtr pvParam)
    {
      this.steamAPICallHandle = 0UL;
      try
      {
        this.function((T) Marshal.PtrToStructure(pvParam, typeof (T)), false);
      }
      catch (Exception ex)
      {
        CallbackDispatcher.ExceptionHandler(ex);
      }
    }

    private void OnRunCallResult(
      IntPtr thisptr,
      IntPtr pvParam,
      bool bFailed,
      ulong hSteamAPICall)
    {
      if ((long) hSteamAPICall != (long) this.steamAPICallHandle)
        return;
      this.function((T) Marshal.PtrToStructure(pvParam, typeof (T)), bFailed);
      if ((long) hSteamAPICall != (long) this.steamAPICallHandle)
        return;
      this.steamAPICallHandle = 0UL;
    }

    private int OnGetCallbackSizeBytes(IntPtr thisptr)
    {
      return this.structureSize;
    }

    public delegate void DispatchDelegate(T value, bool ioFailure);
  }
}
