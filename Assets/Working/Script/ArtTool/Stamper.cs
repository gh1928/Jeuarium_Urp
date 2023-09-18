using System.Collections;
using UnityEngine;

namespace BNG
{
    public class Stamper : GrabbableEvents
    {
        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        public float RaycastLength = 0.01f;

        public GameObject collagePatternPrefab;
        GameObject patternObj;

        ControllerHand grabHandSide;

        public float MinDrawDistance = 0.02f;
        public float ReuseTolerance = 0.001f;

        // Use this to store our Marker's LineRenderers
        Transform root;
        Coroutine drawRoutine = null;

        public bool isFrottage;
        void DestroyHolder()
        {
            if (patternObj != null)
                Destroy(patternObj.gameObject);
            if (root != null)
                Destroy(root.gameObject);
        }
        
        public override void OnGrab(Grabber grabber)
        {
            grabHandSide = grabber.HandSide;
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

        IEnumerator WriteRoutine()
        {
            while (true)
            {
                if (Physics.Raycast(RaycastStart.position, RaycastStart.up, out RaycastHit hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore))
                {
                    Vector3 drawStart = hit.point + (-RaycastStart.up * 0.0005f);
                    Quaternion drawRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    if (patternObj == null)
                    {
                        if (root != null)
                        {
                            if (isFrottage)
                                root = null;
                            else Destroy(root.gameObject);
                        }
                        patternObj = Instantiate(collagePatternPrefab, drawStart, drawRotation);
                        //patternObj.transform.localScale *= DataManager.instance.BrushSize;

                        if(isFrottage)
                            patternObj.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                    else
                    {
                        patternObj.transform.position = drawStart;

                        if (InputBridge.Instance.RightTriggerNear)
                        {
                            patternObj.transform.Rotate(Vector3.up * (60 * Time.deltaTime));
                        }
                        else if (InputBridge.Instance.LeftTriggerNear)
                        {
                            patternObj.transform.Rotate(Vector3.down * (60 * Time.deltaTime));
                        }
                        if (InputBridge.Instance.RightTriggerDown || InputBridge.Instance.LeftTriggerDown)
                        {
                            if (root == null)
                            {
                                root = new GameObject().transform;
                                root.name = "CollageHolder";
                            }

                            if (isFrottage)
                            {
                                patternObj.AddComponent<FrottageObj>();
                                patternObj.AddComponent<MeshCollider>();
                            }

                            patternObj.transform.parent = root;
                            patternObj.layer = LayerMask.NameToLayer("WhiteBoard");
                            patternObj = null;
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

