using System.Collections;
using UnityEngine;

namespace BNG
{
    public class Frottage : GrabbableEvents
    {
        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        public float RaycastLength = 1f;

        public float MinDrawDistance = 0.02f;
        public float ReuseTolerance = 0.001f;

        Coroutine drawRoutine = null;

        Crayon crayon;
        private void Start()
        {
            crayon = GetComponent<Crayon>();
            RaycastStart = crayon.RaycastStart;

        }

        public override void OnGrab(Grabber grabber)
        {
            if (drawRoutine == null)
            {
                drawRoutine = StartCoroutine(WriteRoutine());
            }
            base.OnGrab(grabber);
        }

        public override void OnRelease()
        {
            if (drawRoutine != null)
            {
                StopCoroutine(drawRoutine);
                drawRoutine = null;
            }
            base.OnRelease();
        }


        IEnumerator WriteRoutine()
        {
            RaycastHit[] hits;
            Color backgroundColor = new Color(1, 1, 1, 0);

            while (true)
            {
                hits = Physics.RaycastAll(RaycastStart.position, RaycastStart.up, RaycastLength, DrawingLayers, QueryTriggerInteraction.Collide);
                for (int hitCount = 0; hitCount < hits.Length; hitCount++)
                {
                    Renderer hitObjRenderer = hits[hitCount].collider.GetComponent<MeshRenderer>();
                    Texture2D hitObjTexture2D = hitObjRenderer.material.mainTexture as Texture2D;

                    Vector2 pCoord = hits[hitCount].textureCoord;
                    pCoord.x *= hitObjTexture2D.width;
                    pCoord.y *= hitObjTexture2D.height;

                    Vector2 tiling = hitObjRenderer.material.mainTextureScale;

                    float penWidth = crayon.LineWidth * 2000;
                    for (int pixelCoordX = (int)(pCoord.x - penWidth); pixelCoordX < (int)pCoord.x + penWidth; pixelCoordX++)
                    {
                        for (int pixelCoordY = (int)(pCoord.y - penWidth); pixelCoordY < (int)pCoord.y + penWidth; pixelCoordY++)
                        {
                            Color surfaceColor = hitObjTexture2D.GetPixel(Mathf.FloorToInt(pixelCoordX * tiling.x), Mathf.FloorToInt(pixelCoordY * tiling.y));
                            if (surfaceColor != backgroundColor)
                            {
                                surfaceColor.r = Mathf.Min(crayon.DrawColor.r, surfaceColor.r);
                                surfaceColor.g = Mathf.Min(crayon.DrawColor.g, surfaceColor.g);
                                surfaceColor.b = Mathf.Min(crayon.DrawColor.b, surfaceColor.b);
                                surfaceColor.a = crayon.DrawColor.a;
                                hitObjTexture2D.SetPixel(Mathf.FloorToInt(pixelCoordX * tiling.x), Mathf.FloorToInt(pixelCoordY * tiling.y), surfaceColor);
                            }
                        }
                    }
                    hitObjTexture2D.Apply();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        void OnDrawGizmosSelected()
        {
            // Show Grip Point
            Gizmos.color = Color.green;
            if(RaycastStart!=null)
            Gizmos.DrawLine(RaycastStart.position, RaycastStart.position + RaycastStart.up * RaycastLength);
        }
    }
}

