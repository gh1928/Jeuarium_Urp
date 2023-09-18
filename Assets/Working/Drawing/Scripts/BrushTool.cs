using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace BNG
{
    // Integrated in DrawingTool.cs
    public class BrushTool : GrabbableEvents
    {
        public PaintUI paintUI;

        [Tooltip("�귯���κ��� ĵ�������� Raycast�� ������ �������� �����մϴ�")]
        public Transform rayPivot;
        [Tooltip("�귯���� �� �κ��� �����մϴ�")]
        public Transform endPoint;
        [Tooltip("�귯���� ����� Material�� �����մϴ�")]
        public Material mat;

        public Color crayonColor;

        [Tooltip("ĵ������ ��µ� Render Texture�� �����մϴ�")]
        public CustomRenderTexture rt;

        [Tooltip("�ؽ����� �ػ󵵸� ���մϴ�")]
        public int res = 512;
        [Tooltip("�귯���� ����� ���մϴ�")]
        public float brushSize = 0.01f;

        public bool isReady = false;
        public bool isCrayon = false;

        Texture2D canvas;
        Texture2D origin = null;
        Texture2D brushTex;

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

            grab = GetComponent<Grabbable>();
            input = InputBridge.Instance;

            if (File.Exists(Application.dataPath + "/Save_Data/modification.png"))
            {
                canvas = new Texture2D(0, 0, TextureFormat.RGB24, false);
                var data = File.ReadAllBytes(Application.dataPath + "/Save_Data/modification.png");
                canvas.LoadImage(data);
                rt.initializationTexture = canvas;
            }
            else
                rt.initializationTexture = null;
        }

        void Start()
        {
            rt.Initialize();

            Color temp = crayonColor;
            temp.a = 0;

            if (isCrayon)             // �� �ؽ��İ� ���� ���ٸ�...
            {
                brushTex = new Texture2D(res, res, TextureFormat.ASTC_6x6, false)
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
                endPoint.localEulerAngles = new Vector3(60.0f, 0, 0);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Canvas"))
            {
                if (cor != null)
                    StopCoroutine(cor);                                 // Drawing() �ڷ�ƾ ����
                prevCoord = -(Vector2.one);
                endPoint.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// Render Texture�� �귯�� �ؽ��ĸ� ������ ũ�⸸ŭ �׷��ݴϴ�.
        /// </summary>
        /// <returns></returns>
        IEnumerator Drawing()
        {
            while (true)
            {
                Debug.DrawRay(rayPivot.position, rayPivot.up * 10.0f);
                if (Physics.Raycast(new Ray(rayPivot.position, rayPivot.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
                {
                    float dist = Mathf.Clamp(brushSize - rHit.distance, 0.001f, brushSize);
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
    }
};
