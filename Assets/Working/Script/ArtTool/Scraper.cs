using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class Scraper : GrabbableEvents
    {
        public Material DrawMaterial;
        public Color DrawColor = Color.magenta;

        [Range(0.0005f,0.002f)]
        public float LineWidth = 0.001f;

        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        public float RaycastLength = 0.01f;
        public ParticleSystem particleObject;

        public float scratchingEffectDistance = 0.7f;
        public float MinDrawDistance = 0.02f;
        public float ReuseTolerance = 0.001f;

        bool IsNewDraw = false;
        Vector3 lastDrawPoint;
        LineRenderer LineRenderer;

        Transform root;
        Transform lastTransform;
        Coroutine drawRoutine = null;
        float lastLineWidth = 0;
        int renderLifeTime = 0;

        private Vector3 StartPos;
        public CapsuleCollider[] capsuleCollider;

        private void Start()
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            setColliderTrigger(true);
        }
        void setColliderTrigger(bool state)
        {
            for (int i = 0; i < capsuleCollider.Length; i++)
            {
                capsuleCollider[i].isTrigger = state;
            }
        }

        void DestroyHolder()
        {
            if (root != null)
                Destroy(root.gameObject);
        }

        public override void OnGrab(Grabber grabber)
        {
            StartPos = transform.position;

            if (drawRoutine == null)
            {
                drawRoutine = StartCoroutine(WriteRoutine());
            }
            base.OnGrab(grabber);
            setColliderTrigger(false);
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

            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = StartPos;
            transform.localRotation = Quaternion.Euler(Vector3.right * 90);
            setColliderTrigger(true);
        }

        IEnumerator WriteRoutine()
        {
            while (true)
            {
                if (Physics.Raycast(RaycastStart.position, RaycastStart.up, out RaycastHit hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore))
                {
                    float tipDistance = Vector3.Distance(hit.point, RaycastStart.transform.position);
                    float tipDercentage = tipDistance / RaycastLength;
                    Vector3 drawStart = hit.point + (-RaycastStart.up * 0.0005f);
                    Quaternion drawRotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
                    float lineWidth = LineWidth * (1 - tipDercentage);
                    InitDraw(drawStart, drawRotation, lineWidth, DrawColor);
                }
                else
                {
                    IsNewDraw = true;
                    DestroyHolder();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        void InitDraw(Vector3 position, Quaternion rotation, float lineWidth, Color lineColor)
        {
            if (IsNewDraw)
            {
                lastDrawPoint = position;
                DrawPoint(lastDrawPoint, position, lineWidth, lineColor, rotation);
                IsNewDraw = false;
            }
            else
            {
                float dist = Vector3.Distance(lastDrawPoint, position);
                if (dist > MinDrawDistance)
                {
                    lastDrawPoint = DrawPoint(lastDrawPoint, position, lineWidth, DrawColor, rotation);
                }
                if (dist > scratchingEffectDistance)//긁는 가루 효과
                {
                    particleObject.Play();
                }
            }
        }
        Vector3 DrawPoint(Vector3 lastDrawPoint, Vector3 endPosition, float lineWidth, Color lineColor, Quaternion rotation)
        {
            var dif = Mathf.Abs(lastLineWidth - lineWidth);
            lastLineWidth = lineWidth;
            if (dif > ReuseTolerance || renderLifeTime >= 98)
            {
                LineRenderer = null;
                renderLifeTime = 0;
            }
            else
            {
                renderLifeTime += 1;
            }
            if (IsNewDraw || LineRenderer == null)
            {
                lastTransform = new GameObject().transform;
                lastTransform.name = "DrawLine";
                lastTransform.gameObject.layer = LayerMask.NameToLayer("WhiteBoardCover");
                if (root == null)
                {
                    root = new GameObject().transform;
                    root.name = "MarkerLineHolder";
                }
                lastTransform.parent = root;
                lastTransform.position = endPosition;
                lastTransform.rotation = rotation;
                LineRenderer = lastTransform.gameObject.AddComponent<LineRenderer>();

                LineRenderer.startColor = lineColor;
                LineRenderer.endColor = lineColor;
                LineRenderer.startWidth = lineWidth;
                LineRenderer.endWidth = lineWidth;
                var curve = new AnimationCurve();
                curve.AddKey(0, lineWidth);
                //curve.AddKey(1, lineWidth);
                LineRenderer.widthCurve = curve;

                if (DrawMaterial)
                {
                    LineRenderer.material = DrawMaterial;
                }
                LineRenderer.numCapVertices = 5;
                LineRenderer.alignment = LineAlignment.TransformZ;
                LineRenderer.useWorldSpace = true;
                LineRenderer.SetPosition(0, lastDrawPoint);
                LineRenderer.SetPosition(1, endPosition);
            }
            else
            {
                particleObject.Play();
                if (LineRenderer != null)
                {
                    LineRenderer.widthMultiplier = 1;
                    LineRenderer.positionCount += 1;
                    var curve = LineRenderer.widthCurve;
                    curve.AddKey((LineRenderer.positionCount - 1) / 100, lineWidth);
                    LineRenderer.widthCurve = curve;
                    LineRenderer.SetPosition(LineRenderer.positionCount - 1, endPosition);
                }
            }
            return endPosition;
        }

        void OnDrawGizmosSelected()
        {
            // Show Grip Point
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RaycastStart.position, RaycastStart.position + RaycastStart.up * RaycastLength);
        }
    }
}

