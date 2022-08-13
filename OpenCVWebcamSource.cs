using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;
using OpenCvSharp;


public class OpenCVWebcamSource : MonoBehaviour, ImageSourceInterface
{
	[Tooltip("Whether the web-camera output needs to be flipped horizontally or not.")]
	public bool flipHorizontally = false;

	[Tooltip("Selected web-camera name, if any.")]
	public string webcamName;

	// the web-camera texture
	private WebCamTexture webcamTex;

	// whether the output aspect ratio is set
	private bool bTexResolutionSet = false;

	// OPENCV
	CascadeClassifier cascade;
	OpenCvSharp.Rect myFace;

	public virtual void Awake () 
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		
		// OPENCV
		cascade = new CascadeClassifier(Application.dataPath + @"\OpenCV+Unity\Demo\Face_Detector\" + "haarcascade_frontalface_default.xml");

		if (devices != null && devices.Length > 0)
		{
			// print available webcams
			StringBuilder sbWebcams = new StringBuilder();
			sbWebcams.Append("Available webcams:").AppendLine();

			foreach(WebCamDevice device in devices)
			{
				sbWebcams.Append(device.name).AppendLine();
			}

			Debug.Log(sbWebcams.ToString());

			// get 1st webcam name, if not set
			if(string.IsNullOrEmpty(webcamName))
			{
				webcamName = devices[2].name;
			}

			// create webcam tex
			webcamTex = new WebCamTexture(webcamName);

			OnApplyTexture(webcamTex);

			bTexResolutionSet = false;
		}

		if (flipHorizontally)
		{
			Vector3 scale = transform.localScale;
			transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
		}

		if (HasCamera())
		{
			webcamTex.Play();
		}
	}


	void Update()
	{
		if (!bTexResolutionSet && webcamTex != null && webcamTex.isPlaying)
		{
			OnSetAspectRatio(webcamTex.width, webcamTex.height);
			bTexResolutionSet = true;
		}

		
        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTex);
        findNewFace(frame);

		

    }

	void findNewFace(Mat frame)
    {
		var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);

		if (faces.Length >= 1)
        {
			myFace = faces[0];
			frame.Rectangle(myFace, new Scalar(250, 0, 0), 2);

			Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
			OnApplyTexture(newTexture);
        }
    }


	void OnDisable()
	{
		if(webcamTex)
		{
			webcamTex.Stop();
			webcamTex = null;
		}
	}


	/// <summary>
	/// Gets the image as texture2d.
	/// </summary>
	/// <returns>The image.</returns>
	public Texture2D GetImage()
	{
		Texture2D snap = new Texture2D(webcamTex.width, webcamTex.height, TextureFormat.ARGB32, false);

		if (webcamTex)
		{
			snap.SetPixels(webcamTex.GetPixels());
			snap.Apply();

			if (flipHorizontally)
			{
				snap = CloudTexTools.FlipTexture(snap);
			}
		}

		return snap;
	}


	// Check if there is web camera
	public bool HasCamera()
	{
		return webcamTex && !string.IsNullOrEmpty(webcamTex.deviceName);
	}


	public void OnApplyTexture(Texture tex)
    {
        RawImage rawimage = GetComponent<RawImage>();
        if (rawimage)
        {
			rawimage.texture = tex;
			//rawimage.material.mainTexture = tex;
        }
    }

	public void OnSetAspectRatio(int width, int height)
    {
        AspectRatioFitter ratioFitter = GetComponent<AspectRatioFitter>();
        if (ratioFitter)
        {
            ratioFitter.aspectRatio = (float)width / (float)height;
        }
    }
}
