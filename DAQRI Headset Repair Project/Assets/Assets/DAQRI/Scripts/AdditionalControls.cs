using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdditionalControls : MonoBehaviour {

	public void CameraHDResolutionOnOff (bool isOn) {
		if (isOn) {
			DAQRI.ServiceManager.Instance.RegisterVideoTextureUser (this, true);
		} else {
			DAQRI.ServiceManager.Instance.RegisterVideoTextureUser (this, false);
		}
	}

	public void DepthHistogramOnOff (bool isOn) {
		DAQRI.ServiceManager.Instance.SetDepthRenderType (isOn ? DAQRI.DSH_DEPTH_RENDER_TYPE.HISTOGRAM : DAQRI.DSH_DEPTH_RENDER_TYPE.RAW);
	}

	//currently not supported by our libraries
	/*public void ColorCameraWhiteBalanceOnOff (bool isOn) {
		DAQRI.ServiceManager.Instance.SetWhiteBalanceOnOff(isOn);
	}

	public void ColorCameraAutoExposureOnOff (bool isOn) {
		DAQRI.ServiceManager.Instance.SetAutoExposureOnOff(isOn);
	}*/
}
