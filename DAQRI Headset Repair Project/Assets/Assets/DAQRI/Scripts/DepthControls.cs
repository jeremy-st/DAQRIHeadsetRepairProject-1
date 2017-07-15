using UnityEngine;
using System.Collections;
using DAQRI;

public class DepthControls : MonoBehaviour {

	public GameObject depthCameraPreview;

	// Depth vision is overlaid on the real world
	public void DepthOverlayToggled(bool isOn)
	{
		if (isOn){
			DisplayManager.Instance.TurnDepthBackgroundOn();
		} else {
			DisplayManager.Instance.TurnDepthBackgroundOff();
		}
	}

	// Depth previews are used in user interfaces
	public void DepthPreviewToggled(bool isOn)
	{
		if (isOn){
			depthCameraPreview.SetActive(true);
		} else {
			depthCameraPreview.SetActive(false);
		}
	}
}
