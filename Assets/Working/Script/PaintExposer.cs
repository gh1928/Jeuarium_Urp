using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintExposer : MonoBehaviour
{
    public List<RawImage> paints;

    public void ActivePaints()
    {
        foreach (RawImage g in paints)
        {
            StartCoroutine(AlphaPaints(g, 1));
        }
    }

    public void DeactivePaints()
    {
        foreach (RawImage g in paints)
        {
            StartCoroutine(AlphaPaints(g, 0));
        }
    }

    IEnumerator AlphaPaints(RawImage image, int i)
    {
        if (i == 1)
        {
            while (image.color != Color.white)
            {
                image.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), Color.white, image.color.a + Time.deltaTime);
                yield return null;
            }
        }

        else if (i == 0)
        {
            while (image.color != new Color(1.0f, 1.0f, 1.0f, 0.0f))
            {
                image.color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 1 - image.color.a + Time.deltaTime);
                yield return null;
            }
        }
    }
}
