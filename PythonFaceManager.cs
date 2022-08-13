using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;

public class PythonFaceManager
{

    private static readonly Color[] faceColors = new Color[] { Color.green, Color.yellow, Color.cyan, Color.magenta, Color.red };
    private static readonly string[] faceColorsName = new string[] { "green", "yellow", "cyan", "magenta", "red" };

    public PythonFaceStructure[] GetResult(string serverUrl) 
    {
        Debug.Log(serverUrl);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result_js = reader.ReadToEnd();

        Debug.Log(result_js);

        PythonFaceList result_ls = JsonUtility.FromJson<PythonFaceList>(result_js);
        PythonFaceStructure[] result_struct = ResultParser(result_ls);

        return result_struct;
    }

    public PythonFaceStructure[] ResultParser(PythonFaceList result)
    {
        int resultCount = result.faceList.Length;
        int faceCount = resultCount / 6;
        int faceCurrentIdx = 0;
        
        PythonFaceStructure[] faceStructureList = new PythonFaceStructure[faceCount];
        for (int i = 0; i < resultCount; i = i + 6)
        {
            faceStructureList[faceCurrentIdx] = new PythonFaceStructure();
            faceStructureList[faceCurrentIdx].age = result.faceList[i];
            faceStructureList[faceCurrentIdx].gender = result.faceList[i + 1] == 0 ? "male" : "female";
            faceStructureList[faceCurrentIdx].faceRectangle = new int[] {   result.faceList[i+ 2],
                                                                            result.faceList[i+ 3],
                                                                            result.faceList[i+ 4],
                                                                            result.faceList[i+ 5]};
            faceStructureList[faceCurrentIdx].faceColor = faceColors[faceCurrentIdx % faceColors.Length];
            faceStructureList[faceCurrentIdx].faceColorName = faceColorsName[faceCurrentIdx % faceColorsName.Length];
            faceCurrentIdx++;
        }

        return faceStructureList;
    }
}
