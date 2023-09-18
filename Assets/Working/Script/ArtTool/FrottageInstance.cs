using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrottageInstance : MonoBehaviour
{

    private new Renderer renderer;
    private Texture2D originTexture2D, instanceTexture2D;

    public void SetTexture(Renderer render,Texture2D texture)
    {
        renderer = render;
        originTexture2D = Instantiate<Texture2D>(texture);

        Vector2 originPixelCoord = renderer.material.mainTextureScale;
        originPixelCoord.x *= originTexture2D.width;
        originPixelCoord.y *= originTexture2D.height;

        for (int pixelCoordX = 0; pixelCoordX < originPixelCoord.x; pixelCoordX++)
        {
            for (int pixelCoordY = 0; pixelCoordY < originPixelCoord.y; pixelCoordY++)
            {
                Color surfaceColor = originTexture2D.GetPixel(pixelCoordX, pixelCoordY);
                surfaceColor.a = 0;
                originTexture2D.SetPixel(pixelCoordX, pixelCoordY, surfaceColor);
            }
        }
        originTexture2D.Apply();
        renderer = transform.GetComponent<Renderer>();
        renderer.material.mainTexture = originTexture2D;
        ResetTexture();
    }

    public void ResetTexture()
    {
        instanceTexture2D = Instantiate<Texture2D>(originTexture2D);
        instanceTexture2D.Apply();
        renderer.material.mainTexture = instanceTexture2D;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.transform.GetComponent<BNG.Frottage>())
            return;

        if (GameObject.Find("MarkerLineHolder")) // 라인이 덮어씌우기 방지
            Destroy(GameObject.Find("MarkerLineHolder"));

        ResetTexture();
    }
}
