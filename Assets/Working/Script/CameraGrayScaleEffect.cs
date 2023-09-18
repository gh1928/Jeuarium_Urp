using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class CameraGrayScaleEffect : MonoBehaviour
{
    [SerializeField]
    Material cameraMaterial = null;

    [SerializeField, Range(0, 1)]
    float grayScale = 0.0f;

    private void Awake()
    {
        cameraMaterial = new Material(Shader.Find("Hidden/Grayscale"));
    }

    //후처리 효과. src 이미지(현재 화면)를 dest 이미지로 교체
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cameraMaterial == null) return;

        cameraMaterial.SetFloat("_Grayscale", grayScale);
        Graphics.Blit(src, dest, cameraMaterial);
    }
}