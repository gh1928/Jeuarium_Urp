using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteColor : MonoBehaviour
{

    public Color paletteColor;

    struct CMYK
    {
        public float C, M, Y, K;
    }

    private void Start()
    {
        //paletteColor = transform.GetComponent<Renderer>().material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.transform.gameObject.name.Contains("Brush"))
        {
            other.attachedRigidbody.GetComponent<BNG.AraMarker>().DrawColor = paletteColor;
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Contains("Marker")) 
            MixColor(paletteColor,0.85f);
    }

    public void MixColor(Color color, float blend)
    {
        Color MixColor;
        Color c, d, f;
        d = ColorInv(color);
        c = ColorInv(DataManager.instance.PaletteColor);

        f.r = Mathf.Max(0, 255 - c.r - d.r);
        f.g = Mathf.Max(0, 255 - c.g - d.g);
        f.b = Mathf.Max(0, 255 - c.b - d.b);
        f.a = 255;
        //float cd = ColorDistance(color, DataManager.instance.PaletteColor);
        cd = (float)(4.0 * blend * (1.0 - blend) * cd);
        MixColor = ColorMixLin(ColorMixLin(color, DataManager.instance.PaletteColor, blend), f, cd);

        MixColor.a = 255;
        DataManager.instance.PaletteColor = MixColor;
        Debug.Log(DataManager.instance.PaletteColor);
}

    Color ColorInv(Color color)
    {
        Color InvColor = new Color(255 - color.r, 255 - color.g, 255 - color.b, 255);
        return InvColor;
    }

        float ColorDistance(Color a, Color b)
    {
        float tt= (float)((a.r - b.r) * (a.r - b.r) + (a.g - b.g) * (a.g - b.g) + (a.b - b.b) * (a.b - b.b));
        tt = Mathf.Sqrt(tt) / (Mathf.Sqrt(3.0f) * 255); //scale to 0-1
        return tt;
    }
    Color ColorMixLin(Color a, Color b, float blend)
    {
        Color color;
        color.r = (float)((1.0 - blend) * a.r + blend * b.r);
        color.g = (float)(1.0 - blend) * a.g + blend * b.g;
        color.b = (float)(1.0 - blend) * a.b + blend * b.b;
        color.a = (float)(1.0 - blend) * a.a + blend * b.a;

        return color;
    }
    Color ColorMix(Color a, Color b, float blend)
    {
        Color color;
        color.r = Mathf.Sqrt((float)((1.0 - blend) * (a.r * a.r) + blend * (b.r * b.r)));
        color.g = Mathf.Sqrt((float)(1.0 - blend) * (a.g * a.g) + blend * (b.g * b.g));
        color.b = Mathf.Sqrt((float)(1.0 - blend) * (a.b * a.b) + blend * (b.b * b.b));
        color.a = (float)(1.0 - blend) * a.a + blend * b.a;

        return color;
    }
    */
}
