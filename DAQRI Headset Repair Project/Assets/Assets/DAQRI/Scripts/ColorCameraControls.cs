using UnityEngine;
using System.Collections;
using DAQRI;

public class ColorCameraControls : MonoBehaviour {

	public GameObject colorCameraPreview;

	// Color camera is overlaid on the real world
	public void ColorCameraOverlayToggled(bool isOn)
	{
		if (isOn) {
			DisplayManager.Instance.TurnVideoBackgroundOn ();
		} else {
			DisplayManager.Instance.TurnVideoBackgroundOff();
		}
	}

	// Color camera are used in user interfaces
	public void ColorCameraPreviewToggled(bool isOn)
	{
		if (isOn){
			colorCameraPreview.SetActive(true);
		} else {
			colorCameraPreview.SetActive(false);
		}
	}
}
