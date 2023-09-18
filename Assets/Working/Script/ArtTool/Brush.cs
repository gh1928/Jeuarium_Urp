using System.Collections;
using UnityEngine;

namespace BNG
{
    public class Brush : GrabbableEvents
    {
        public Color DrawColor = Color.red;

        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        public float RaycastLength = 0.01f;

        public GameObject BrushRangePrefab;
        GameObject brushRangeObj;
        Transform BrushPatternHolder;

        public float MinDrawDistance = 0.02f;
        public float ReuseTolerance = 0.001f;


        Coroutine drawRoutine = null;


        void DestroyHolder()
        {
            if (brushRangeObj != null)
                Destroy(brushRangeObj.gameObject);
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
            DestroyHolder();
        }

        bool isPressBrush;
        IEnumerator WriteRoutine()
        {
            while (true)
            {
                if (Physics.Raycast(RaycastStart.position, RaycastStart.up, out RaycastHit hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore))
                {
                    Vector3 drawStart = hit.point + (-RaycastStart.up * 0.0005f);
                    Quaternion drawRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    if (InputBridge.Instance.RightTriggerDown)
                    {
                        if (BrushPatternHolder != null)
                            Destroy(BrushPatternHolder.gameObject);
                        isPressBrush = true;
                    }

                    if (brushRangeObj == null)
                    {
                        brushRangeObj = Instantiate(BrushRangePrefab, drawStart, Quaternion.identity);
                        //brushRangeObj.transform.localScale *= DataManager.instance.BrushRange;
                    }
                    else
                    {
                        brushRangeObj.transform.position = hit.point;
                        brushRangeObj.transform.rotation = drawRotation;
                        if (isPressBrush)
                        {
                            BrushPatternHolder = brushRangeObj.transform.GetChild(0);
                            for (int count = 0; count < BrushPatternHolder.childCount; count++)
                            {
                                BrushPatternHolder.GetChild(count).gameObject.layer = LayerMask.NameToLayer("WhiteBoard");
                            }
                            brushRangeObj.transform.DetachChildren();

                            Destroy(brushRangeObj.gameObject);

                            isPressBrush = false;
                        }
                    }
                }
                else
                {
                    DestroyHolder();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        void OnDrawGizmosSelected()
        {
            // Show Grip Point
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RaycastStart.position, RaycastStart.position + RaycastStart.up * RaycastLength);
        }
    }
}

