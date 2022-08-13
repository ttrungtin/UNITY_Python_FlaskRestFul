using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Events;
using System;
using TMPro;

// ADDDDD emotion

public class PythonFaceDetector : MonoBehaviour
{
    public OpenCVWebcamSource imageSource;

    public RawImage cameraShot;
    public TMP_Text hintText2;
    public TMP_Text resultText2;
    public string serverUrl;
    public Action<PythonFaceStructure> resultMethod;

    private AspectRatioFitter ratioFitter;
    private bool hasCamera = false;
    private string hintMessage;
    

    void Start()
    {

        if (cameraShot)
        {
            ratioFitter = cameraShot.GetComponent<AspectRatioFitter>();
        }

        hasCamera = imageSource != null && imageSource.HasCamera();

        hintMessage = hasCamera ? "정면을 응시후 위 카메라영역을 터치하세요" : "No camera found";

        SetHintText(hintMessage);

    }

    public void OnCameraClick()
    {
        if (!hasCamera)
        {
            Debug.Log("NonCamera");
            return;
        }

        if (DoCameraShot())
        {
            ClearResultText();
            StartCoroutine(DoFaceDetection());
        }
    }

    public void OnShotClick()
    {
        if (DoImageImport())
        {
            ClearResultText();
            StartCoroutine(DoFaceDetection());
        }
    }

    private bool DoImageImport()
    {
        Texture2D tex = FaceDetectionUtils.ImportImage();
        if (!tex) return false;

        SetShotImageTexture(tex);

        return true;
    }

    private IEnumerator DoFaceDetection()
    {
        PythonFaceStructure[] faces = null;
        Texture2D texCamShot = null;

        if (cameraShot)
        {
            texCamShot = (Texture2D)cameraShot.texture;
            SetHintText("잠시기다려주세요...");
        }

        PythonFaceManager faceManager = new PythonFaceManager();

        if (texCamShot)
        {
            System.IO.File.WriteAllBytes("./Assets/_____PYTHON/source/saved_image/recorded.png", texCamShot.EncodeToPNG());

            yield return null;

            AsyncTask<PythonFaceStructure[]> taskFace = new AsyncTask<PythonFaceStructure[]>(() =>
            {
                return faceManager.GetResult(serverUrl);
            });

            taskFace.Start();
            yield return null;

            while (taskFace.State == TaskState.Running)
            {
                yield return null;
            }

            if (string.IsNullOrEmpty(taskFace.ErrorMessage))
            {
                faces = taskFace.Result;

                if (faces != null && faces.Length > 0)
                {
                    DrawFaceRects(texCamShot, faces);
                    SetHintText("좌상단 카메라영역을 터치해주세요");
                    SetHintText(hintMessage);
                    SetResultText(faces);
                }
                else
                {
                    SetHintText("얼굴을 인식할 수 없습니다. \n");
                    SetHintText("정면을 응시한후 위 카메라영역을 터치하세요. \n");
                }
            }
            else
            {
                SetHintText(taskFace.ErrorMessage);
            }


        }

        yield return null;
    }

    private static string FaceToString(PythonFaceStructure face)
    {
        StringBuilder sbResult = new StringBuilder();

        sbResult.Append(string.Format(" gender:{0}", face.gender)).AppendLine();
        sbResult.Append(string.Format(" age:{0}", face.age)).AppendLine();

        sbResult.AppendLine();

        return sbResult.ToString();
    }

    private void SetResultText(PythonFaceStructure[] faces)
    {
        StringBuilder sbResult = new StringBuilder();

        if (faces != null && faces.Length > 0 )
        {
            for (int i = 0; i < faces.Length; i++)
            {
                PythonFaceStructure face = faces[i];

                string res = FaceToString(face);

                sbResult.Append(string.Format("<사각형영역={0}>{1}</color>", face.faceColorName, res));

            }
        }

        // JUST BASED ON LAST FACE DETECTED
        string result = sbResult.ToString();

        if (resultText2)
        {
            resultText2.text = result;
            resultMethod(faces[faces.Length-1]);
            Debug.Log(result);

        }
        else
        {
            Debug.Log(result);
        }

    }

    private static void DrawFaceRects(Texture2D tex, PythonFaceStructure[] faces)
    {
        for (int i = 0; i < faces.Length; i++)
        {
            PythonFaceStructure face = faces[i];
            
            int[] rect = face.faceRectangle;
            DrawRect(tex, rect[0], rect[1], rect[2], rect[3], face.faceColor);
        }

        tex.Apply();

    }

    private static void DrawRect(Texture2D a_Texture, int x, int y, int w, int h, Color a_Color)
    {
        DrawLine(a_Texture, x, y, x + w - 1, y, a_Color);  // top
        DrawLine(a_Texture, x + w - 1, y, x + w - 1, y + h - 1, a_Color);  // right
        DrawLine(a_Texture, x, y + h - 1, x + w - 1, y + h - 1, a_Color);  // bottom
        DrawLine(a_Texture, x, y, x, y + h - 1, a_Color);  // left
    }

    private  static void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
    {
        int width = a_Texture.width;
        int height = a_Texture.height;

        y1 = height - y1;
        y2 = height - y2;

        int dy = y2 - y1;
        int dx = x2 - x1;

        int stepy = 1;
        if (dy < 0)
        {
            dy = -dy;
            stepy = -1;
        }

        int stepx = 1;
        if (dx < 0)
        {
            dx = -dx;
            stepx = -1;
        }

        dy <<= 1;
        dx <<= 1;

        if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    a_Texture.SetPixel(x1 + x, y1 + y, a_Color);

        if (dx > dy)
        {
            int fraction = dy - (dx >> 1);

            while (x1 != x2)
            {
                if (fraction >= 0)
                {
                    y1 += stepy;
                    fraction -= dx;
                }

                x1 += stepx;
                fraction += dy;

                if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                            a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
            }
        }
        else
        {
            int fraction = dx - (dy >> 1);

            while (y1 != y2)
            {
                if (fraction >= 0)
                {
                    x1 += stepx;
                    fraction -= dy;
                }

                y1 += stepy;
                fraction += dx;

                if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                            a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
            }
        }

    }

    private bool DoCameraShot()
    {
        if (cameraShot != null && imageSource != null)
        {
            SetShotImageTexture(imageSource.GetImage());
            return true;
        }

        return false;
    }

    private void SetShotImageTexture(Texture2D tex)
    {
        if (ratioFitter)
        {
            ratioFitter.aspectRatio = (float)tex.width / (float)tex.height;
        }

        if (cameraShot)
        {
            cameraShot.texture = tex;
        }
    }

    private void SetHintText(string sHintText)
    {
        if (hintText2)
        {
            hintText2.text = sHintText;
        }
        else
        {
            Debug.Log(sHintText);
        }
    }

    private void ClearResultText()
    {
        if (resultText2)
        {
            resultText2.text = "";
        }
    }

}