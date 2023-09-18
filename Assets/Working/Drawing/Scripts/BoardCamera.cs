using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardCamera : MonoBehaviour
{
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {

        if (BoardCameraManager.manager == null)
        {
            GameObject managerGO = new GameObject("Board Camera Manager");
            var manager = managerGO.AddComponent(typeof(BoardCameraManager));

        }
        if (TryGetComponent<Camera>(out Camera thisCam))
        {
            BoardCameraManager.manager.AddBoardCamera(thisCam);

            cam = thisCam;
        }
    }

    private void OnDestroy()
    {
        if (cam != null)
        {
            SaveRTToFile(cam.targetTexture);
            if (BoardCameraManager.manager != null)
            {
                BoardCameraManager.manager.RemoveBoardCamera(cam);
            }
        }
    }
    public static void SaveRTToFile(RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();
        string fileName = System.DateTime.Now.ToString().Replace(@"\", "-");
        string path = System.IO.Path.Combine( Application.streamingAssetsPath, fileName  + ".png"); //AssetDatabase.GetAssetPath(rt) + ".png";
        System.IO.File.WriteAllBytes(path, bytes);
        
        Debug.Log("Saved to " + path);
    }
}
