using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    // Integrated in DrawingTool.cs
    public class PointillismTool : GrabbableEvents
    {
        public PaintUI paintUI;

        [Tooltip("브러쉬로부터 캔버스까지 Raycast를 실행할 기준점을 지정합니다")]
        public Transform rayPivot;

        [Tooltip("브러쉬가 사용할 Material을 지정합니다")]
        public Material mat;

        [Tooltip("캔버스에 출력될 Render Texture를 지정합니다")]
        public CustomRenderTexture rt;

        [Tooltip("텍스쳐의 해상도를 정합니다")]
        public int res = 512;
        [Tooltip("브러쉬의 사이즈를 정합니다")]
        public float brushSize = 0.01f;

        public bool isReady = false;

        Texture2D brushTex;

        Vector3 pos;
        Quaternion rot;
        RaycastHit rHit;

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
        }

        void Start()
        {
            Color temp = mat.color;
            temp.a = 0;

            if (mat.GetTexture("_DrawTex") == null)
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
                            brushTex.SetPixel(j, i, mat.color);
                        }
                        else brushTex.SetPixel(j, i, temp);
                    }
                }

                brushTex.Apply();
                mat.SetTexture("_DrawTex", brushTex);
            }
            else
            {
                brushTex = mat.GetTexture("_DrawTex") as Texture2D;
                Color32[] colorData = brushTex.GetPixels32();
                Color32 matColor32 = mat.color;
                for (int i = 0; i < colorData.Length; i++)
                {
                    colorData[i].r = matColor32.r;
                    colorData[i].g = matColor32.g;
                    colorData[i].b = matColor32.b;
                }
                brushTex.SetPixels32(colorData);
                brushTex.Apply();
            }
            
            isReady = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Canvas"))
            {
                Vector2 size = 0.01f * res * Vector2.one;

                if (Physics.Raycast(new Ray(rayPivot.position, rayPivot.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
                {
                    Debug.Log("OK");
                    RenderTexture.active = rt;
                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, res, 0, res);

                    Vector2 pos = res * rHit.textureCoord - brushSize * res * 0.5f * Vector2.one;
                    Debug.Log(Mathf.Floor(brushSize / 0.011f));
                    for (int i = 0; i < Mathf.Floor(brushSize / 0.011f); i++)
                    {
                        for (int j = 0; j < Mathf.Floor(brushSize / 0.011f); j++)
                        {
                            if (Vector2.Distance(res * rHit.textureCoord, pos + new Vector2((j * 0.011f + 0.005f) * res, (i * 0.011f + 0.005f) * res)) < brushSize * res * 0.5f)
                            {
                                Graphics.DrawTexture(
                                    new Rect(
                                        pos + new Vector2(j * res * 0.011f, i * res * 0.011f),
                                        size
                                        ),
                                    brushTex
                                );
                            }
                        }
                    }
                    /*Graphics.DrawTexture(
                        new Rect(
                            rHit.textureCoord.x * res - (0.005f * res),
                            rHit.textureCoord.y * res - (0.005f * res),
                            0.01f * res,
                            0.01f * res
                            ),
                        brushTex
                    );*/

                    GL.PopMatrix();
                    RenderTexture.active = null;
                }
            }
        }
    }
};