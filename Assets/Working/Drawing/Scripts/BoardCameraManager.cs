using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardCameraManager : MonoBehaviour
{
    public static BoardCameraManager manager;
    public List<Camera> camList;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void AddBoardCamera(Camera cam)
    {
        if (camList == null)
        {
            camList = new List<Camera>();
        }

        if (cam.targetTexture == null)
            return;

        if (!camList.Contains(cam))
        { camList.Add(cam); }
    }

    public void RemoveBoardCamera(Camera cam)
    {
        if (camList == null)
        {
            return;
        }

        if (cam.targetTexture == null)
            return;

        if (camList.Contains(cam))
        { camList.Remove(cam); }

    }

    public Camera GetNearestBoardCamera(Vector3 position)
    {
        Camera resultCam = null;

        float tempDistance = Mathf.Infinity;

        foreach(Camera c in camList)
        {
            float distance = Vector3.Distance(c.transform.position, position);
            if ( distance < tempDistance)
            {
                resultCam = c;
                tempDistance = distance;
            }
        }

        return resultCam;
    }
}
