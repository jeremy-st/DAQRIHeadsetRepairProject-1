using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public abstract class SensorData{

	public readonly byte[] byteArray;
	public IntPtr Address { get; private set; }

	//private float refreshRate;
	private GCHandle handle;

	protected int width;
	protected int height;

	//getters
	public int GetWidth(){
		return width;
	}

	public int GetHeight(){
		return height;
	}

	/*public float GetRefreshRate(){
		return refreshRate;
	}*/

	internal SensorData(int arraysize)
	{
		byteArray = new byte[arraysize];
		handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
		Address = handle.AddrOfPinnedObject();
	}

	~SensorData()
	{
		Address = IntPtr.Zero;
		handle.Free();
	}
}
