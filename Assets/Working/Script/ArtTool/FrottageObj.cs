using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class FrottageObj : GrabbableEvents
    {
        public Transform pattern;
        public Texture2D patternTexture;

        private new Renderer renderer;
        private Texture2D originTexture2D, instanceTexture2D;

        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        GameObject FrottageInstance;

        public float RaycastLength = 0.01f;

        public float MinDrawDistance = 0.02f;
        public float ReuseTolerance = 0.001f;

        private void Start()
        {
            renderer = pattern.transform.GetComponent<Renderer>();
            originTexture2D = renderer.material.mainTexture as Texture2D;
        }

        public override void OnGrab(Grabber grabber)
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Collider>().isTrigger = false;
            if (FrottageInstance != null) 
            { 
                Destroy(FrottageInstance);
                pattern.transform.localEulerAngles = Vector3.zero;
            }
            base.OnGrab(grabber);
        }

        public override void OnRelease()
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            DrawRay ();
            base.OnRelease();
        }

        private void SetTexture()
        {
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
            ResetTexture();
        }

        private void ResetTexture()
        {
            instanceTexture2D = Instantiate<Texture2D>(originTexture2D);
            instanceTexture2D.Apply();
            renderer.material.mainTexture = instanceTexture2D;
        }

        void DrawRay()
        {
            if (Physics.Raycast(RaycastStart.position, -RaycastStart.up, out RaycastHit hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 drawStart = hit.point + (-RaycastStart.up * 0.0005f);
                Quaternion drawRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                float distance = Vector3.Distance(hit.transform.position, transform.position);
                
                if (distance <= MinDrawDistance)
                {
                    float rotateY = pattern.transform.rotation.eulerAngles.y;
                    transform.GetComponent<Rigidbody>().isKinematic = true;
                    transform.position = drawStart;
                    transform.rotation = drawRotation;

                    pattern.transform.localEulerAngles=new Vector3(0,rotateY,0);

                    instantiantFrottage();
                    transform.GetComponent<Collider>().isTrigger = true;
                }
            }
        }

        void instantiantFrottage()
        {
            FrottageInstance = Instantiate(pattern.gameObject,transform.position,pattern.transform.rotation);

            Vector3 temp = transform.position;
            temp.z += 0.002f;
            FrottageInstance.transform.position = temp;

            FrottageInstance.AddComponent<MeshCollider>();
            FrottageInstance.AddComponent<FrottageInstance>();
            FrottageInstance.GetComponent<FrottageInstance>().SetTexture(renderer,patternTexture);

            FrottageInstance.gameObject.layer = LayerMask.NameToLayer("WhiteBoard");
        }

    }
}