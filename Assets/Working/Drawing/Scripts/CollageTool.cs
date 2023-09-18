using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    // Integrated in DrawingTool.cs
    public class CollageTool : GrabbableEvents
    {
        public PaintUI paintUI;

        public CustomRenderTexture rt;
        public Transform canvas;
        public Transform leaf;
        public Transform raycastPivot;
        public Material mat;

        public int canvas_Res = 1024;
        public int tex_Res = 512;

        Vector3 pos;
        Quaternion rot;

        Texture2D result;
        Color32[] color_Data;
        Color32[] result_Data;

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

        void Start()
        {
            pos = transform.position;
            rot = transform.rotation;

            color_Data = ((Texture2D)mat.mainTexture).GetPixels32();

            StartCoroutine(RotateTex());
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Canvas"))
            {
                paintUI.SaveHistory();

                if (Physics.Raycast(new Ray(raycastPivot.position, raycastPivot.up), out rHit, 3.0f, 1 << LayerMask.NameToLayer("Canvas")))
                {
                    RenderTexture.active = rt;
                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, canvas_Res, canvas_Res, 0);
                    Graphics.DrawTexture(
                        new Rect(
                            canvas_Res * rHit.textureCoord.x - canvas_Res / (canvas.lossyScale.x / leaf.lossyScale.x) * 0.5f,
                            canvas_Res - canvas_Res * rHit.textureCoord.y - canvas_Res / (canvas.lossyScale.x / leaf.lossyScale.x) * 0.5f,
                            canvas_Res / (canvas.lossyScale.x / leaf.lossyScale.x),
                            canvas_Res / (canvas.lossyScale.x / leaf.lossyScale.x)
                            ),
                        result
                        );
                    
                    GL.PopMatrix();
                    RenderTexture.active = null;
                }
            }
        }

        IEnumerator RotateTex()
        {
            float x_angle;
            float c;
            float s;

            while (true)
            {
                yield return new WaitUntil(() => { return transform.hasChanged; });

                x_angle = Vector3.SignedAngle(transform.up, Vector3.up, -transform.right);
                Debug.Log(x_angle);
                mat.SetFloat("_Rotate", x_angle < 0 ? 360.0f + x_angle : x_angle) ;

                c = Mathf.Cos(x_angle * Mathf.PI / 180.0f);
                s = Mathf.Sin(x_angle * Mathf.PI / 180.0f);

                result = new Texture2D(mat.mainTexture.width, mat.mainTexture.height, TextureFormat.RGBA32, false);
                
                result_Data = new Color32[color_Data.Length];

                for (int i = 0; i < tex_Res; i++)
                {
                    for (int j = 0; j < tex_Res; j++)
                    {
                        int x = (int)Mathf.Floor(c * (i - tex_Res * 0.5f) + s * (j - tex_Res * 0.5f) + tex_Res * 0.5f);
                        int y = (int)Mathf.Floor(-s * (i - tex_Res * 0.5f) + c * (j - tex_Res * 0.5f) + tex_Res * 0.5f);

                        if (x >= 0 && x < tex_Res && y >= 0 && y < tex_Res) 
                        {
                            result_Data[i + tex_Res * j] = color_Data[x + tex_Res * y];
                        }
                        else result_Data[i + tex_Res * j] = new Color(0, 0, 0, 0);
                    }
                }

                result.SetPixels32(result_Data);
                result.Apply();

                transform.hasChanged = false;
                yield return null;
            }
        }
    }
};