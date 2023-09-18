using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

// Deprecated
[ExecuteInEditMode()]
public class DrawingBrush : MonoBehaviour
{

    [SerializeField] Gradient maskGradient;

    [SerializeField] LineRenderer line;

    //[SerializeField] float repeatBrushLength = 0.5f;

    Vector3[] positions;

    // Start is called before the first frame update
    void Awake()
    {
        positions = new Vector3[128];
    }

    private void OnValidate()
    {
    Gradient g = new Gradient();
        GradientColorKey[] ck = new GradientColorKey[2];
        ck[0] = new GradientColorKey(Color.red, 0f);
        ck[1] = new GradientColorKey(Color.red, 1f);
        GradientAlphaKey[] ak = new GradientAlphaKey[2];
        ak[0] = new GradientAlphaKey(0, 0);
        ak[1] = new GradientAlphaKey(1, 1);
        g.SetKeys(ck, ak);

        line.colorGradient = g;


        SetLineVertexColor();

    }


    void SetLineVertexColor()
    {
        Debug.Log("Try to set");

        float length = GetLineLength(line);

        if (length < 1)
        {
            ShortLine(length);
        }
        else
        {
            LongLine(length);
        }
    }

    public float GetLineLength(LineRenderer _line)
    {
        if (positions.Length < _line.positionCount)
        {
            positions = new Vector3[_line.positionCount];
        }
        int count = _line.GetPositions(positions);
        float length = 0;
        for (int i = 1; i < count; ++i)
        {
            length += Vector3.Distance(positions[i], positions[i - 1]);
        }

        return length;
    }

    public void ShortLine(float length)
    {
        Gradient g = new Gradient();
        GradientColorKey[] ck = new GradientColorKey[2];
        ck[0] = new GradientColorKey(Color.red, 0f);
        ck[1] = new GradientColorKey(Color.red, 1f);
        GradientAlphaKey[] ak = new GradientAlphaKey[2];
        ak[0] = new GradientAlphaKey(0, 0);
        ak[1] = new GradientAlphaKey(1, 1);
        g.SetKeys(ck, ak);


    }

    public Color GetRandomColor()
    {
        int value = Random.Range(0, 3);

        switch (value)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
        }
        return Color.white;
    }
    public Color GetRandomColor(Color exclude)
    {
        int value = Random.Range(0, 3);

        switch (value)
        {
            case 0:
                if(exclude == Color.red)
                {
                    return Random.Range(0, 2) == 1 ? Color.blue : Color.green;
                }
                return Color.red;
            case 1:
                if (exclude == Color.blue)
                {
                    return Random.Range(0, 2) == 1 ? Color.red : Color.green;
                }
                return Color.blue;
            case 2:
                if (exclude == Color.green)
                {
                    return Random.Range(0, 2) == 1 ? Color.red : Color.blue;
                }
                return Color.green;
        }
        return Color.white;
    }

    public float LengthToPercentage(float fullLength, float targetLength)
    {
        return targetLength / fullLength;
    }

    public void LongLine(float length)
    {
        List<GradientColorKey> ckList = new List<GradientColorKey>();
        List<GradientAlphaKey> akList = new List<GradientAlphaKey>();

        Color prevColor = GetRandomColor();
        ckList.Add(new GradientColorKey(prevColor, 0));
        akList.Add(new GradientAlphaKey(0, 0));


        bool edge = false;
        for(float f = 0.1f; f < length; f+=0.8f)
        {
            akList.Add(new GradientAlphaKey(edge?0.1f:0.9f , f));
            if (f + 0.4f < length)
            {
                Color c = GetRandomColor(prevColor);
                ckList.Add(new GradientColorKey(c, f+0.4f));
                prevColor = c;
            }
            if (f + 0.5f < length)
            {
                Color c = GetRandomColor(prevColor);
                ckList.Add(new GradientColorKey(c, f + 0.4f));
                prevColor = c;
            }
            edge = !edge;
        }

        for(int i =0; i < ckList.Count; ++i)
        {
            ckList[i] = new GradientColorKey(ckList[i].color,
                ckList[i].time/length);
        }
        for (int i = 0; i < akList.Count; ++i)
        {
            akList[i] = new GradientAlphaKey(akList[i].alpha,
                akList[i].time / length);
        }

        ckList.Add(new GradientColorKey (GetRandomColor(prevColor), 1));
        akList.Add(new GradientAlphaKey(1, 1));

        line.colorGradient.SetKeys(ckList.ToArray(), akList.ToArray());
    }
}
