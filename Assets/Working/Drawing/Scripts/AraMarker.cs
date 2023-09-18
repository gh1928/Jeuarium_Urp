using Ara;
using System.Collections;
using UnityEngine;

namespace BNG
{
    public class AraMarker : GrabbableEvents
    {
        public CapsuleCollider[] capsuleColliders;

        public Material DrawMaterial;
        public Color DrawColor = Color.black;
        public float LineWidth = 0.02f;
        
        public Transform RaycastStart;
        public LayerMask DrawingLayers;
        
        public Camera CanvasCamera;

        public float RaycastLength = 0.01f;

        public GameObject BrushTip;
        /*LineRenderer*/public AraTrail LineRenderer;

       // Transform root;
        Coroutine drawRoutine = null;
        private void Start()
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            setColliderTrigger(true);
        }


        void setColliderTrigger(bool state)
        {
            for (int i = 0; i < capsuleColliders.Length; i++)
            {
                capsuleColliders[i].isTrigger = state;
            }
        }
        void DestroyHolder()
        {
            //  if (root != null)
            //      Destroy(root.gameObject);

            LineRenderer.emit = true;
        }

        public override void OnGrab(Grabber grabber)
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Collider>().isTrigger = false;
            //if (drawRoutine == null)
            //{
            //    drawRoutine = StartCoroutine(WriteRoutine());
            //}
            base.OnGrab(grabber); 
            setColliderTrigger(false);
        }

        public override void OnRelease()
        {
            //if (drawRoutine != null)
            //{
            //    StopCoroutine(drawRoutine);
            //    drawRoutine = null;
            //}
            base.OnRelease();
            //DestroyHolder();
            LineRenderer.Clear();

            transform.GetComponent<Rigidbody>().isKinematic = true;
            setColliderTrigger(true);
        }

        IEnumerator WriteRoutine()
        {
            while (true)
            {

                if (Physics.Raycast(RaycastStart.position, RaycastStart.up, out RaycastHit hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore))
                { 
                    DrawMaterial.color = DrawColor;

                    //if (root == null)
                    {
                        //root = Instantiate(BrushTip).transform;
                        //root.parent = RaycastStart;
                        //root.transform.position = RaycastStart.transform.position;

                        //LineRenderer = root.GetComponent<AraTrail>();
                        if (BoardCameraManager.manager != null)
                        {
                            CanvasCamera = BoardCameraManager.manager.GetNearestBoardCamera(BrushTip.transform.position);
                            LineRenderer.canvasCamera = CanvasCamera;
                        }
                        LineRenderer.initialThickness = LineWidth;
                       // LineRenderer.emit = true;
                    }
                }
                else
                {
                    // DestroyHolder();
                   // LineRenderer.emit = false;
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

