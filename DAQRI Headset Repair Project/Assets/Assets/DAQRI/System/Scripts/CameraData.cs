using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData : SensorData {
	int colorSpace;
	internal CameraData(int _width, int _height, int color_size) : base (_width*_height*color_size)
	{
		width = _width;
		height = _height;
	}

	~CameraData()
	{
		
	}
}
