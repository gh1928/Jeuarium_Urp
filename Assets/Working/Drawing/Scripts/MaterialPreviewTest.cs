using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for Test
public class MaterialPreviewTest : MonoBehaviour
{
    public Material mat;
    public RenderTexture rt;
    public int resolution = 512;
    public float brushSize = 0.01f;
    Texture2D brushTex;
    Color brushColor;
    RaycastHit rHit;
    Vector3 normal;
    Coroutine cor;

    private void Start()
    {
        brushColor = mat.color;

        if (mat.GetTexture("_DrawTex") == null)
        {
            brushTex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            brushTex.filterMode = FilterMode.Point;

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    if (Vector2.Distance(new Vector2(j, i), Vector2.one * resolution / 2) <= resolution / 2)
                    {
                        brushTex.SetPixel(j, i, brushColor);
                    }
                }
            }

            brushTex.Apply();
        }
        else
        {
            brushTex = mat.GetTexture("_DrawTex") as Texture2D;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Canvas"))
        {
            cor = StartCoroutine(Drawing());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Canvas"))
        {
            StopCoroutine(cor);
        }
    }

    IEnumerator Drawing()
    {
        while (true)
        {
            if (Physics.Raycast(new Ray(transform.position - transform.up, transform.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
            {
                Debug.Log(rHit.collider);
                RenderTexture.active = rt;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, resolution, resolution, 0);
                Graphics.DrawTexture(
                    new Rect(
                        rHit.lightmapCoord.x * resolution - (brushSize * resolution * 0.5f),
                        (rt.height - (rHit.lightmapCoord.y * resolution)) - (brushSize * resolution * 0.5f),
                        brushSize * resolution,
                        brushSize * resolution
                        ),
                    brushTex
                );
                GL.PopMatrix();
                RenderTexture.active = null;
            }
            else Debug.Log("NO");
            yield return null;
        }
    }
}
