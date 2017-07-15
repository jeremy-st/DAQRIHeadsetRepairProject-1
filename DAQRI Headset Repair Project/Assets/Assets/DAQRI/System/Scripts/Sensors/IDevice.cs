using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAQRI{

	public enum DEVICE_STATE {
		IDLE,
		RUNNING,
		FAILED
	}

	public interface IDeviceNativeCall{
		bool StartDeviceNativeCall(int preset);
		bool StopDeviceNativeCall();
		void GetEffectiveRateNativeCall(float[] rate);
		void GetTheoreticalRateNativeCall(float[] rate);
		void GetDataFormatNativeCall(int[] format, bool requested);
	}

	public interface IDevice {

		/*bool StartDevice (int preset = -1);

		bool StopDevice ();

		void UpdateDevice();*/

		DATA_FORMAT GetDataFormat();

		float GetEffectiveRate();

		float GetTheoreticalRate();

		Vector2 GetFrameSize();

		DEVICE_IDENTIFIER GetDeviceType ();

		DEVICE_STATE GetDeviceCurrentState ();
	}
}