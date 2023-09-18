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

        [Tooltip("브러쉬로부터 캔버스까지 Raycast를 실행할 기준점을 지정합니다")]
        public Transform rayPivot;
        [Tooltip("브러쉬의 끝 부분을 지정합니다")]
        public Transform endPoint;
        [Tooltip("브러쉬가 사용할 Material을 지정합니다")]
        public Material mat;

        public Color crayonColor;

        [Tooltip("캔버스에 출력될 Render Texture를 지정합니다")]
        public CustomRenderTexture rt;

        [Tooltip("텍스쳐의 해상도를 정합니다")]
        public int res = 512;
        [Tooltip("브러쉬의 사이즈를 정합니다")]
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

            if (isCrayon)             // 붓 텍스쳐가 따로 없다면...
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

                brushTex.Apply();                               // 원형 텍스쳐를 새로 만들어서 적용
            }
            else                                                // 아니라면 해당 텍스쳐 복사
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

                cor = StartCoroutine(Drawing());                        // Drawing() 코루틴 시작
                endPoint.localEulerAngles = new Vector3(60.0f, 0, 0);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Canvas"))
            {
                if (cor != null)
                    StopCoroutine(cor);                                 // Drawing() 코루틴 정지
                prevCoord = -(Vector2.one);
                endPoint.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// Render Texture에 브러쉬 텍스쳐를 정해진 크기만큼 그려줍니다.
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
                        StartCoroutine(Interpolation(rHit.textureCoord, dist));     // 이전 지점과 현재 붓 지점 사이의 그림을 보간합니다
                    }
                    prevCoord = rHit.textureCoord;
                }
                yield return null;
            }
        }

        /// <summary>
        /// 특정 지점에서 이전 지점 사이의 그림을 보간합니다. 두 지점 사이를 그 거리만큼의 비율로 덧그려 직선을 그립니다.
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
