 using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum TriggerType
{

    None = -1, Goal = 0, Remove, Call, Receive, IsActive, IsDeactive, Delay
}
public enum TriggerShape
{
    Sphere = 0,
    Box,
    Capsule,
}



[System.Serializable]
public struct TriggerEvent
{
    public string name;
    public TriggerType triggerType;
    public TriggerShape triggerShape;
    public Vector3 shapeCenter;
    public Quaternion shapeRotation;
    public Vector3 shapeSize;
    public Transform shapeParent;
    public Transform target;
    public string receiveKey;
    public float duration;

    public UnityEvent events;

    public void GenerateTriggerShape(Transform tf, out bool targetHasCollider, out Collider thisCol, out Collider targetCol)
    {
        //tf.parent = parent;
        targetCol = null;

        if (shapeParent == null)
        { tf.SetPositionAndRotation(shapeCenter, shapeRotation); }
        else
        {
            tf.SetPositionAndRotation(shapeParent.TransformPoint(shapeCenter),
                Quaternion.LookRotation(shapeParent.TransformDirection(shapeRotation * Vector3.forward), shapeParent.TransformDirection(shapeRotation * Vector3.up)));
        }
        tf.localScale = Vector3.one;
        thisCol = null;
        switch (triggerShape)
        {
            case TriggerShape.Sphere:
                SphereCollider sphere;
                if (!tf.TryGetComponent<SphereCollider>(out sphere))
                {
                    sphere = tf.gameObject.AddComponent<SphereCollider>();
                }
                sphere.radius = shapeSize.x;
                sphere.center = Vector3.zero;
                sphere.isTrigger = true;
                sphere.enabled = true;
                thisCol = sphere;
                break;
            case TriggerShape.Box:
                BoxCollider box;
                if (!tf.TryGetComponent<BoxCollider>(out box))
                {
                    box = tf.gameObject.AddComponent<BoxCollider>();
                }
                box.size = shapeSize;
                box.center = Vector3.zero;
                box.isTrigger = true;
                box.enabled = true;
                thisCol = box;
                break;
            case TriggerShape.Capsule:
                CapsuleCollider capsule;
                if (!tf.TryGetComponent<CapsuleCollider>(out capsule))
                {
                    capsule = tf.gameObject.AddComponent<CapsuleCollider>();
                }
                capsule.direction = (Mathf.Approximately(shapeSize.x, shapeSize.y) ? 2 :
                    (Mathf.Approximately(shapeSize.y, shapeSize.z) ? 0 : 1));
                switch (capsule.direction)
                {
                    case 0:
                        capsule.radius = shapeSize.y;
                        capsule.height = shapeSize.x;
                        break;
                    case 1:
                        capsule.radius = shapeSize.x;
                        capsule.height = shapeSize.y;
                        break;
                    case 2:
                        capsule.radius = shapeSize.x;
                        capsule.height = shapeSize.z;
                        break;
                }
                capsule.center = Vector3.zero;
                capsule.isTrigger = true;
                capsule.enabled = true;
                thisCol = capsule;
                break;
        }
        targetHasCollider = CheckTargetHasCollider(out targetCol);
    }

    public bool CheckTargetHasCollider(out Collider targetCol)
    {
        bool hasCollider = false;
        targetCol = null;
        if (target != null)
        {
            if (target.TryGetComponent<Collider>(out targetCol))
            {
                hasCollider = true;
            }
            else
            {
                hasCollider = false;
            }
        }
        return hasCollider;
    }
}

[ExecuteInEditMode()]
public class ActionTriggerManager : MonoBehaviour, IReceiver
{
    public static ActionTriggerManager instance;

    [SerializeField] private int currentSelectedNum;

    private int selectedNum;

    [SerializeField] private UnityEvent startEvent;

    [SerializeField] private TriggerEvent[] scenarioEvents;

    [SerializeField] private UnityEvent endEvent;

    private bool targetHasCollider;
    private Collider thisCol;
    private Collider targetCol;
    private Collider[] tempCol;

    private bool allowSwitchTarget;

    private void OnValidate()
    {
        if (scenarioEvents == null) return;

        if (currentSelectedNum >= scenarioEvents.Length)
        {
            currentSelectedNum = scenarioEvents.Length - 1;
        }
        if (currentSelectedNum < 0)
        {
            currentSelectedNum = 0;
        }

    }
    public static void CapsuleInfo(Vector3 sizeInput, out float radius, out float height, out int direction)
    {
        direction = (Mathf.Approximately(sizeInput.x, sizeInput.y) ? 2 :
                    (Mathf.Approximately(sizeInput.y, sizeInput.z) ? 0 : 1));
        radius = 0;
        height = 0;
        switch (direction)
        {
            case 0:
                radius = sizeInput.y;
                height = sizeInput.x;
                break;
            case 1:
                radius = sizeInput.x;
                height = sizeInput.y;
                break;
            case 2:
                radius = sizeInput.x;
                height = sizeInput.z;
                break;
        }
    }

    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, int direction, UnityEngine.Color _color = default(UnityEngine.Color))
    {
#if UNITY_EDITOR
        if (_color != default(UnityEngine.Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            Vector3 up;
            Vector3 left;
            Vector3 down;
            Vector3 back;
            switch (direction)
            {
                case 0:
                    up = Vector3.right;
                    left = Vector3.up;
                    down = Vector3.left;
                    back = Vector3.back;
                    break;
                case 1:
                    up = Vector3.up;
                    left = Vector3.left;
                    down = Vector3.down;
                    back = Vector3.back;
                    break;
                default:
                    up = Vector3.forward;
                    left = Vector3.left;
                    down = Vector3.back;
                    back = Vector3.up;
                    break;
            }

            //draw sideways
            Handles.DrawWireArc(up * pointOffset, left, back, -180, _radius);
            Handles.DrawWireArc(down * pointOffset, left, back, 180, _radius);

            //draw frontways
            Handles.DrawWireArc(up * pointOffset, back, left, 180, _radius);
            Handles.DrawWireArc(down * pointOffset, back, left, -180, _radius);

            switch (direction)
            {
                case 0:
                    Handles.DrawLine(new Vector3(pointOffset, 0, -_radius),
                        new Vector3(-pointOffset, 0, -_radius));
                    Handles.DrawLine(new Vector3(pointOffset, 0, _radius),
                        new Vector3(-pointOffset, 0, _radius));
                    Handles.DrawLine(new Vector3(pointOffset, -_radius, 0),
                        new Vector3(-pointOffset, -_radius, 0));
                    Handles.DrawLine(new Vector3(pointOffset, _radius, 0),
                        new Vector3(-pointOffset, _radius, 0));
                    break;
                case 1:
                    Handles.DrawLine(new Vector3(0, pointOffset, -_radius),
                        new Vector3(0, -pointOffset, -_radius));
                    Handles.DrawLine(new Vector3(0, pointOffset, _radius),
                        new Vector3(0, -pointOffset, _radius));
                    Handles.DrawLine(new Vector3(-_radius, pointOffset, 0),
                        new Vector3(-_radius, -pointOffset, 0));
                    Handles.DrawLine(new Vector3(_radius, pointOffset, 0),
                        new Vector3(_radius, -pointOffset, 0));
                    break;
                case 2:
                    Handles.DrawLine(new Vector3(-_radius, 0, pointOffset),
                        new Vector3(-_radius, 0, -pointOffset));
                    Handles.DrawLine(new Vector3(_radius, 0, pointOffset),
                        new Vector3(_radius, 0, -pointOffset));
                    Handles.DrawLine(new Vector3(0, -_radius, pointOffset),
                        new Vector3(0, -_radius, -pointOffset));
                    Handles.DrawLine(new Vector3(0, _radius, pointOffset),
                        new Vector3(0, _radius, -pointOffset));
                    break;
            }


            //draw center
            Handles.DrawWireDisc(up * pointOffset, up, _radius);
            Handles.DrawWireDisc(down * pointOffset, up, _radius);

        }
#endif
    }


    private void OnDrawGizmosSelected()
    {
        if (scenarioEvents == null) return;

        for (int i = 0; i < scenarioEvents.Length; ++i)
        {
            TriggerEvent e = scenarioEvents[i];

            Gizmos.color = i == currentSelectedNum? Color.white : Color.grey;

            if (e.triggerType != TriggerType.Goal && e.triggerType != TriggerType.Remove && e.triggerType != TriggerType.Receive) continue;

            Vector3 pos;
            Quaternion rot;
            if (e.shapeParent == null)
            {
                pos = e.shapeCenter;
                rot = e.shapeRotation;
            }
            else
            {
                pos = e.shapeParent.TransformPoint(e.shapeCenter);
                rot = Quaternion.LookRotation(e.shapeParent.TransformDirection(e.shapeRotation * Vector3.forward),
                    e.shapeParent.TransformDirection(e.shapeRotation * Vector3.up));

            }

            if(i > 0)
            {
                var prevEvent = scenarioEvents[i - 1];
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawLine(prevEvent.shapeParent?prevEvent.shapeParent.TransformPoint(e.shapeCenter): prevEvent.shapeCenter , pos);
            }

            Gizmos.matrix = Matrix4x4.TRS(pos, rot, Vector3.one);
            switch (e.triggerShape)
            {
                case TriggerShape.Sphere:
                    Gizmos.DrawWireSphere(Vector3.zero, e.shapeSize.x);
                    break;
                case TriggerShape.Box:
                    Gizmos.DrawWireCube(Vector3.zero, e.shapeSize);
                    break;
                case TriggerShape.Capsule:
                    CapsuleInfo(e.shapeSize, out float radius, out float height, out int dir);

                    DrawWireCapsule(pos, rot, radius, height, dir);
                    break;

            }
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            //      return;
            yield break;
#endif
        if (!inited)
        {
            instance = this;
            tempCol = new Collider[64];
        }
        inited = true;
        currentSelectedNum = 0;

        if (scenarioEvents.Length > 0)
        {
            switch (scenarioEvents[currentSelectedNum].triggerType)
            {
                case TriggerType.Goal:
                case TriggerType.Remove:
                case TriggerType.Receive:
                    scenarioEvents[currentSelectedNum].GenerateTriggerShape(this.transform, out targetHasCollider, out thisCol, out targetCol);
                    break;
            }
        }

        yield return new WaitForSeconds(0.01f);

        startEvent?.Invoke();

    }

    bool inited;

    void OnEnable()
    {
        if (inited)
        {
            StartCoroutine(Start());
        }
    }

    void OnDisable()
    {
        endEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Overlap with " + other.gameObject.name);
        if (scenarioEvents[currentSelectedNum].triggerType == TriggerType.Goal)
        {
            if (scenarioEvents[currentSelectedNum].target == null) return;

            if (other.transform == scenarioEvents[currentSelectedNum].target)
            {
                Debug.Log("OnTriggerEnter");
                Trigged();
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{

    //    if (scenarioEvents[currentSelectedNum].triggerType == TriggerType.Remove)
    //    {
    //        if (other.transform == scenarioEvents[currentSelectedNum].target)
    //        {
    //            Trigged(); 
    //        }
    //    }
    //}

    public static void TrigCall()
    {
        if (instance.scenarioEvents[instance.currentSelectedNum].triggerType == TriggerType.Call)
        {
            instance.Trigged();
        }
    }

    public static void TrigCall(int index)
    {
        if (instance.scenarioEvents[instance.currentSelectedNum].triggerType == TriggerType.Call && instance.currentSelectedNum == index)
        {
            instance.Trigged();
        }
    }

    public void CallTrig()
    {
        if (scenarioEvents[currentSelectedNum].triggerType == TriggerType.Call)
        {
            Trigged();
        }
    }

    public void CallTrig(int index)
    {
        if (scenarioEvents[currentSelectedNum].triggerType == TriggerType.Call && index == currentSelectedNum)
        {
            Trigged();
        }
    }

    IEnumerator TrigDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Trigged();
    }

    void Trigged()
    {
        if (currentSelectedNum >= scenarioEvents.Length) return;

        Debug.Log("Event Trigged");
        DestroyTrigger();

        scenarioEvents[currentSelectedNum].events.Invoke();

        ++currentSelectedNum;

        if (currentSelectedNum < scenarioEvents.Length)
        {
            switch (scenarioEvents[currentSelectedNum].triggerType)
            {
                case TriggerType.Goal:
                case TriggerType.Remove:
                case TriggerType.Receive:
                    scenarioEvents[currentSelectedNum].GenerateTriggerShape(this.transform, out targetHasCollider, out thisCol, out targetCol);
                    break;
                case TriggerType.Delay:
                    StartCoroutine(TrigDelay(scenarioEvents[currentSelectedNum].duration));
                    break;
            }
        }
    }

    void DestroyTrigger()
    {
        if (TryGetComponent<BoxCollider>(out BoxCollider box))
        {
            box.enabled = false;//   DestroyImmediate(box);
        }
        if (TryGetComponent<SphereCollider>(out SphereCollider sphere))
        {
            sphere.enabled = false;
            //DestroyImmediate(sphere);
        }
        if (TryGetComponent<CapsuleCollider>(out CapsuleCollider capsule))
        {
            capsule.enabled = false;
            //DestroyImmediate(capsule);
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (scenarioEvents == null || scenarioEvents.Length <= 0) return;

            Transform p = scenarioEvents[currentSelectedNum].shapeParent;
            if (selectedNum != currentSelectedNum)
            {
                selectedNum = currentSelectedNum;

                if (p == null)
                {
                    transform.position = scenarioEvents[selectedNum].shapeCenter;
                    transform.rotation = scenarioEvents[selectedNum].shapeRotation;
                }
                else
                {
                    transform.position = p.TransformPoint(scenarioEvents[selectedNum].shapeCenter);
                    transform.rotation = Quaternion.LookRotation(p.TransformDirection(scenarioEvents[selectedNum].shapeRotation * Vector3.forward),
                         p.TransformDirection(scenarioEvents[selectedNum].shapeRotation * Vector3.up));
                }
                transform.localScale = scenarioEvents[selectedNum].shapeSize;
            }
            else
            {
                if (p == null)
                {
                    scenarioEvents[selectedNum].shapeCenter = transform.position;
                    scenarioEvents[selectedNum].shapeRotation = transform.rotation;
                }
                else
                {
                    scenarioEvents[selectedNum].shapeCenter = p.InverseTransformPoint(transform.position);
                    scenarioEvents[selectedNum].shapeRotation = Quaternion.LookRotation(p.InverseTransformDirection(transform.rotation * Vector3.forward),
                        p.InverseTransformDirection(transform.rotation * Vector3.up));
                }
                scenarioEvents[selectedNum].shapeSize = transform.localScale;
            }

            return;
        }
        if (Input.GetKey(KeyCode.LeftControl)||Input.GetKeyDown(KeyCode.PageUp))
        {
            Debug.LogWarning("Jump to Next Trigger Event via Key Pressed");
            Trigged();
        }
#endif

        if (currentSelectedNum >= scenarioEvents.Length)
            return;

        CheckCondition();
    }

    void CheckCondition()
    {

        if ((scenarioEvents[currentSelectedNum].triggerType == TriggerType.IsActive || scenarioEvents[currentSelectedNum].triggerType == TriggerType.IsDeactive)
            && scenarioEvents[currentSelectedNum].target != null)
        {
            if ((scenarioEvents[currentSelectedNum].triggerType == TriggerType.IsActive && scenarioEvents[currentSelectedNum].target.gameObject.activeInHierarchy) ||
                (scenarioEvents[currentSelectedNum].triggerType == TriggerType.IsDeactive && !scenarioEvents[currentSelectedNum].target.gameObject.activeInHierarchy))
            {
                Trigged();
            }
        }
        else if (!targetHasCollider && scenarioEvents[currentSelectedNum].target != null)
        {
            int count = Physics.OverlapSphereNonAlloc(scenarioEvents[currentSelectedNum].target.position, 1f, tempCol);
            //Debug.Log(count);
            switch (scenarioEvents[currentSelectedNum].triggerType)
            {
                case TriggerType.Goal:
                    for (int i = 0; i < count; i++)
                    {
                        if (tempCol[i].transform == this.transform)
                        {
                            //Debug.Log("OverlapSphereNonAlloc");
                            Trigged();
                            break;
                        }
                    }
                    break;
                case TriggerType.Remove:
                    bool inside = false;
                    for (int i = 0; i < count; i++)
                    {
                        if (tempCol[i].transform == this.transform)
                        {
                            inside = true;
                            break;
                        }
                    }
                    if (!inside)
                    {
                        Debug.Log("!OverlapSphereNonAlloc");
                        Trigged();
                    }
                    break;
            }
        }
        else
        {
            if (scenarioEvents[currentSelectedNum].triggerType == TriggerType.Remove && !Physics.ComputePenetration(thisCol, transform.position, transform.rotation,
                targetCol, scenarioEvents[currentSelectedNum].target.position, scenarioEvents[currentSelectedNum].target.rotation,
                out Vector3 direction, out float distance))
            {
                Debug.Log("Isn't penetarted");
                Trigged();
            }
        }
    }

    public void SetTarget(Transform value)
    {
        if (allowSwitchTarget)
        {
            scenarioEvents[currentSelectedNum].target = value;
            scenarioEvents[currentSelectedNum].CheckTargetHasCollider(out Collider col);
        }
    }

    public void AllowSwitchTarget(bool value)
    {
        allowSwitchTarget = value;
    }

    public void OnReceive(string data)
    {

        if (scenarioEvents[currentSelectedNum].triggerType != TriggerType.Receive) return;

        if (scenarioEvents[currentSelectedNum].receiveKey.Equals(data))
        {
            Trigged();
        }
    }
}
