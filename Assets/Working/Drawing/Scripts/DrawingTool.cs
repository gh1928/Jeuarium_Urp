using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace BNG
{
    public partial class DrawingTool : GrabbableEvents
    {
        public enum DrawType
        {
            Brush,
            Roller,
            Crayon,
            Pointillism,
            Collage,
            Frottage
        }

        public DrawType drawType = DrawType.Brush;

        public PaintUI paintUI;

        [Tooltip("�귯���κ��� ĵ�������� Raycast�� ������ �������� �����մϴ�")]
        public Transform rayPivot;
        [Tooltip("�귯���� �� �κ�, �Ǵ� �ݶ����� �ؽ�ó ���� ������Ʈ�� �����մϴ�")]
        public Transform endPoint;
        [Tooltip("�귯���� ����� Material�� �����մϴ�")]
        public Material mat;
        [Tooltip("ũ�������� ����� ��� �� ���� �����մϴ�")]
        public Color crayonColor;
        [Tooltip("ĵ������ ��µ� Render Texture�� �����մϴ�")]
        public CustomRenderTexture rt;
        [Tooltip("�ؽ����� �ػ󵵸� ���մϴ�")]
        public int res = 512;
        [Tooltip("�귯���� ����� ���մϴ�")]
        public float brushSize = 0.01f;

        public bool isReady = false;

        Texture2D origin = null;
        Texture2D brushTex;

        Color32[] color_Data;
        Color32[] result_Data;

        Vector2 prevCoord = -(Vector2.one);
        Vector3 pos;
        Quaternion rot;
        RaycastHit rHit;
        Coroutine cor;

        public override void OnGrab(Grabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = false;

            base.OnGrab(grabber);
        }

        public override void OnRelease()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetPositionAndRotation(pos, rot);
            base.OnRelease();
        }

        protected override void Awake()
        {
            pos = transform.position;
            rot = transform.rotation;

            
            paintUI.colorUpdate += new PaintUI.ColorCallBack(TextureColorUpdate);

            grab = GetComponent<Grabbable>();
            input = InputBridge.Instance;
        }

        void Start()
        {
            if (drawType == DrawType.Crayon)                    // ũ���� ����� ��......
            {
                Color temp = crayonColor;
                temp.a = 0;
                brushTex = new Texture2D(res, res, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point
                };

                for (int i = 0; i < res; i++)
                {
                    for (int j = 0; j < res; j++)
                    {
                        if (Vector2.Distance(new Vector2(j, i), Vector2.one * res / 2) <= res / 2)
                        {
                            brushTex.SetPixel(j, i, crayonColor);
                        }
                        else brushTex.SetPixel(j, i, temp);
                    }
                }

                brushTex.Apply();                               // ���� �ؽ��ĸ� ���� ���� ����
            }

            else if (drawType == DrawType.Pointillism)
            {
                Color temp = mat.color;
                temp.a = 0;
                brushTex = new Texture2D(res, res, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point
                };

                for (int i = 0; i < res; i++)
                {
                    for (int j = 0; j < res; j++)
                    {
                        if (Vector2.Distance(new Vector2(j, i), Vector2.one * res / 2) <= res / 2)
                        {
                            brushTex.SetPixel(j, i, mat.color);
                        }
                        else brushTex.SetPixel(j, i, temp);
                    }
                }

                brushTex.Apply();

                mat.SetTexture("_DrawTex", brushTex);
            }

            else if (drawType == DrawType.Collage || drawType == DrawType.Frottage)
            {
                color_Data = ((Texture2D)mat.mainTexture).GetPixels32();

                StartCoroutine(RotateTex());
            }

            else                                                // �ƴ϶�� �ش� �ؽ��� ����
            {
                origin = mat.GetTexture("_DrawTex") as Texture2D;
                brushTex = new Texture2D(origin.width, origin.height, TextureFormat.RGBA32, false);
                Graphics.CopyTexture(origin, brushTex);
                brushTex.Apply();

                mat.SetTexture("_DrawTex", brushTex);
            }

            isReady = true;
        }

        private void OnDestroy()
        {
            if (origin != null)
                mat.SetTexture("_DrawTex", origin);
            rt.Initialize();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Canvas"))
            {
                paintUI.SaveHistory();

                cor = StartCoroutine(Drawing());                        // Drawing() �ڷ�ƾ ����
                
                if (drawType == DrawType.Brush)
                    endPoint.localEulerAngles = new Vector3(-60.0f, 0, 0);
            }
            else if(other.CompareTag("Paint"))
            {
                ANM_Basic_OnTriggerEnter__Paint(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Canvas"))
            {
                if (cor != null)
                    StopCoroutine(cor);                                 // Drawing() �ڷ�ƾ ����
                prevCoord = -(Vector2.one);

                if (drawType == DrawType.Brush)
                    endPoint.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// ������, ũ����, �ݶ���, ����Ÿ��(������) ������ �浹�� �����ϰ� ĵ������ �׸��� �׸��ϴ�.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Canvas") && (drawType != DrawType.Crayon || drawType != DrawType.Brush))
            {
                paintUI.SaveHistory();

                if (Physics.Raycast(new Ray(rayPivot.position, rayPivot.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
                {
                    RenderTexture.active = rt;
                    GL.PushMatrix();

                    if (drawType == DrawType.Pointillism)
                    {
                        GL.LoadPixelMatrix(0, res, 0, res);
                        Graphics.DrawTexture(
                            new Rect(
                                rHit.textureCoord.x * res - (0.5f * brushSize * res),
                                rHit.textureCoord.y * res - (0.5f * brushSize * res),
                                brushSize * res,
                                brushSize * res
                                ),
                            brushTex
                        );
                    }

                    else if (drawType == DrawType.Collage)
                    {
                        GL.LoadPixelMatrix(0, 1024, 1024, 0);
                        Graphics.DrawTexture(
                        new Rect(
                            1024f * rHit.textureCoord.x - 1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x) * 0.5f,
                            1024f - 1024f * rHit.textureCoord.y - 1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x) * 0.5f,
                            1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x),
                            1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x)
                            ),
                        brushTex
                        );
                    }

                    else if (drawType == DrawType.Frottage)
                    {
                        GL.LoadPixelMatrix(0, 1024, 1024, 0);
                        Graphics.DrawTexture(
                        new Rect(
                            1024f * rHit.textureCoord.x - 1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x) * 0.5f,
                            1024f - 1024f * rHit.textureCoord.y - 1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x) * 0.5f,
                            1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x),
                            1024f / (rHit.transform.lossyScale.x / endPoint.lossyScale.x)
                            ),
                        brushTex
                        );
                    }

                    GL.PopMatrix();
                    RenderTexture.active = null;
                }
            }
        }

        /// <summary>
        /// �귯�� �ؽ�ó�� ���� ������Ʈ�մϴ�.
        /// </summary>
        /// <param name="bColor">������Ʈ�� ��</param>
        void TextureColorUpdate(Color32 bColor)
        {
            if (mat.GetTexture("_DrawTex") != null)
            {
                Texture2D tempTex = mat.GetTexture("_DrawTex") as Texture2D;
                Color32[] tempColor = tempTex.GetPixels32();
                for (int i = 0; i < tempColor.Length; i++)
                {
                    tempColor[i].r = bColor.r;
                    tempColor[i].g = bColor.g;
                    tempColor[i].b = bColor.b;
                }
                tempTex.SetPixels32(tempColor);
                tempTex.Apply();
                mat.SetTexture("_DrawTex", tempTex);
            }
        }

        /// <summary>
        /// Render Texture�� �귯�� �ؽ��ĸ� ������ ũ�⸸ŭ �׷��ݴϴ�.
        /// </summary>
        /// <returns></returns>
        IEnumerator Drawing()
        {
            float dist;

            while (true)
            {
                Debug.DrawRay(rayPivot.position, rayPivot.up * 10.0f);
                if (Physics.Raycast(new Ray(rayPivot.position, rayPivot.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
                {
                    if (drawType == DrawType.Crayon)
                        dist = brushSize;
                    else 
                        dist = Mathf.Clamp(brushSize - rHit.distance, 0.001f, brushSize);

                    RenderTexture.active = rt;
                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, res, 0, res);
                    Graphics.DrawTexture(
                        new Rect(
                            rHit.textureCoord.x * res - (dist * res * 0.5f),
                            rHit.textureCoord.y * res - (dist * res * 0.5f),
                            dist * res,
                            dist * res
                            ),
                        brushTex
                    );
                    GL.PopMatrix();
                    RenderTexture.active = null;

                    if (prevCoord.x != -1 && prevCoord != rHit.textureCoord)
                    {
                        StartCoroutine(Interpolation(rHit.textureCoord, dist));     // ���� ������ ���� �� ���� ������ �׸��� �����մϴ�
                    }
                    prevCoord = rHit.textureCoord;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Ư�� �������� ���� ���� ������ �׸��� �����մϴ�. �� ���� ���̸� �� �Ÿ���ŭ�� ������ ���׷� ������ �׸��ϴ�.
        /// </summary>
        /// <param name="nowCoord"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IEnumerator Interpolation(Vector2 nowCoord, float size)
        {
            float dist = Vector2.Distance(nowCoord, prevCoord);
            Vector2 interval = new Vector2((nowCoord.x - prevCoord.x) * dist, (nowCoord.y - prevCoord.y) * dist);
            Vector2 midCoord = prevCoord + interval;

            for (int i = 0; i < (nowCoord.x - prevCoord.x) / interval.x; i++)
            {
                RenderTexture.active = rt;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, res, 0, res);
                Graphics.DrawTexture(
                    new Rect(
                        midCoord.x * res - (size * res * 0.5f),
                        midCoord.y * res - (size * res * 0.5f),
                        size * res,
                        size * res
                        ),
                    brushTex
                );
                GL.PopMatrix();
                RenderTexture.active = null;
                midCoord += interval;
            }
            yield break;
        }

        /// <summary>
        /// ���� ȸ���� ���߾� �ݶ����� �ؽ�ó�� ȸ����ŵ�ϴ�.
        /// </summary>
        /// <returns></returns>
        IEnumerator RotateTex()
        {
            float x_angle;
            float c;
            float s;

            while (true)
            {
                yield return new WaitUntil(() => { return transform.hasChanged; });

                x_angle = Vector3.SignedAngle(transform.up, Vector3.up, -transform.right);
                mat.SetFloat("_Rotate", x_angle < 0 ? 360.0f + x_angle : x_angle);

                c = Mathf.Cos(x_angle * Mathf.PI / 180.0f);
                s = Mathf.Sin(x_angle * Mathf.PI / 180.0f);

                brushTex = new Texture2D(mat.mainTexture.width, mat.mainTexture.height, TextureFormat.RGBA32, false);

                result_Data = new Color32[color_Data.Length];

                for (int i = 0; i < res; i++)
                {
                    for (int j = 0; j < res; j++)
                    {
                        int x = (int)Mathf.Floor(c * (i - res * 0.5f) + s * (j - res * 0.5f) + res * 0.5f);
                        int y = (int)Mathf.Floor(-s * (i - res * 0.5f) + c * (j - res * 0.5f) + res * 0.5f);

                        if (x >= 0 && x < res && y >= 0 && y < res)
                        {
                            result_Data[i + res * j] = color_Data[x + res * y];

                            if (drawType == DrawType.Frottage && result_Data[i + res * j].a > 0)
                            {
                                result_Data[i + res * j].a = color_Data[x + res * y].r;
                            }
                        }
                        else result_Data[i + res * j] = new Color(0, 0, 0, 0);
                    }
                }

                brushTex.SetPixels32(result_Data);
                brushTex.Apply();

                transform.hasChanged = false;
                yield return null;
            }
        }
    }

    partial class DrawingTool
    {

        ////////// Getter & Setter  //////////

        ////////// Method           //////////

        ////////// Unity            //////////
        void ANM_Basic_OnTriggerEnter__Paint(Collider _other)
        {
            if (!_other.GetComponent<ANM_GoghYellowhouse_Paint>().enabled)
            {
                Color color = _other.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
                //
                Texture2D tempTex = mat.GetTexture("_DrawTex") as Texture2D;
                Color32[] tempColor = tempTex.GetPixels32();
                for (int i = 0; i < tempColor.Length; i++)
                {
                    tempColor[i].r = (byte)(color.r * 255.0f);
                    tempColor[i].g = (byte)(color.g * 255.0f);
                    tempColor[i].b = (byte)(color.b * 255.0f);
                }
                tempTex.SetPixels32(tempColor);
                tempTex.Apply();
                mat.SetTexture("_DrawTex", tempTex);
            }
        }
    }
};
