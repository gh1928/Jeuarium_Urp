using UnityEngine;
using UnityEngine.AI;
using RootMotion;
using RootMotion.FinalIK;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class FreeRoamAI : MonoBehaviour
{
    enum State { WaitDestinaiton, CalculatePath, Move}

    [SerializeField] private Vector2 delayDurationForGetDestination = new Vector2(0.8f, 1.2f);
    [SerializeField] private float distanceCheckThreshold = 0.1f;
    [SerializeField] private Animator anim;
    [SerializeField] private string forwardParamName="Forward";
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float lookPlayerRange;
    [SerializeField] private AnimationCurve lookCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 destination;
    private float untilNextBehavior;
    private State currentState;
    private Vector3[] corners = new Vector3[0];
    private NavMeshAgent agent;
    private NavMeshPath path;
    private int currentCornerIndex;
    private LookAtIK lookAtIK;

    public GameObject TargetObject;//목표지점

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        lookAtIK = GetComponent<LookAtIK>();
        StartCoroutine(SetLookAtIKWeight());
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        GetDestination();
    }

    bool GetDestination()
    {
        if (NonPlayableCharacterManager.instance == null)
        {
            untilNextBehavior = Random.Range(delayDurationForGetDestination.x, delayDurationForGetDestination.y);
            return false;
        }
        if (TargetObject)
            destination = TargetObject.transform.position;
        else
            destination = NonPlayableCharacterManager.GetRandomPointOnInstanceRange();
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (corners!=null)
        if (corners.Length > currentCornerIndex)
        {   
            Gizmos.DrawLine(transform.position, corners[currentCornerIndex]);
        }
        Gizmos.DrawWireSphere(transform.position, lookPlayerRange);
    }

    // Update is called once per frame
    void Update()
    {
        //SetLookAtIKWeight();

        switch (currentState)
        {
            case State.WaitDestinaiton:
                if (HaveToActingForNextBehavior())
                {
                    if (GetDestination())
                    {
                        currentState = State.CalculatePath;
                    }
                }
                break;
            case State.CalculatePath:
                CalculatePath();
                currentState = State.Move;
                break;
            case State.Move:
                if (CheckReachedEnd())
                {
                    StopMoving();
                    return;        
                }
                if (!CheckReachedTargetCorner())
                {
                    Moving();
                }
                break;
        }



    }

    IEnumerator SetLookAtIKWeight()
    {
        float Rand = 8f;
        while (true)
        {
            if (Rand < 0f)
                Rand = 8f;
            else if (Rand < 2f)
            {
                lookAtIK.solver.SetIKPositionWeight(lookAtIK.solver.GetIKPositionWeight() - Time.deltaTime);
            }
            else if (Rand < 4f)
            {
                lookAtIK.solver.SetIKPositionWeight(lookAtIK.solver.GetIKPositionWeight() + Time.deltaTime);
            }
            else
            {
                Vector3 targetPos = lookAtIK.solver.target.position;
                lookAtIK.solver.SetIKPositionWeight(Vector3.Distance(transform.position, targetPos) > lookPlayerRange ?
                  Mathf.Max(lookAtIK.solver.GetIKPositionWeight() - Time.deltaTime,
                  0) :
                  Mathf.Min(lookAtIK.solver.GetIKPositionWeight() + Time.deltaTime,
                  lookCurve.Evaluate(Mathf.Clamp01(Vector3.Dot(transform.forward,
                  Vector3.Normalize(targetPos - lookAtIK.solver.head.transform.position))))));
            }
            Rand -= Time.deltaTime;
            yield return new WaitForSeconds(0.01666f);
        }
    }


    /*
    void SetLookAtIKWeight()
    {
        Vector3 targetPos = lookAtIK.solver.target.position;
        lookAtIK.solver.SetIKPositionWeight(Vector3.Distance(transform.position, targetPos) > lookPlayerRange ?
          Mathf.Max(lookAtIK.solver.GetIKPositionWeight() - Time.deltaTime,
          0):
          Mathf.Min(lookAtIK.solver.GetIKPositionWeight() + Time.deltaTime,
          lookCurve.Evaluate(Mathf.Clamp01(Vector3.Dot(transform.forward,
          Vector3.Normalize(targetPos - lookAtIK.solver.head.transform.position))))));
    }*/


    void WaitDelayForNextBehavior()
    {
        if (untilNextBehavior > 0)
            untilNextBehavior -= Time.deltaTime;
    }

    bool HaveToActingForNextBehavior()
    {
        WaitDelayForNextBehavior();

        return untilNextBehavior < 0;
    }

    void CalculatePath()
    {
        currentCornerIndex = 0;
        agent.enabled = true;
        agent.CalculatePath(destination, path);

        corners = path.corners;
        agent.enabled = false;
    }

    bool CheckReachedEnd()
    {
        if (currentCornerIndex >= corners.Length)
        {
            currentState = State.WaitDestinaiton;
            return true;
        }
        return false;
    }

    bool CheckReachedTargetCorner()
    {
        if (Vector3.Distance(transform.position, corners[currentCornerIndex]) < distanceCheckThreshold)
        {
           
            currentCornerIndex++;
            return true;
        }
        return false;
    }

    void Moving()
    {
        anim.SetFloat(forwardParamName, 1f);
        //transform.LookAt(Vector3.Normalize(corners[currentCornerIndex] - transform.position)
        //    , Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(Vector3.Normalize(corners[currentCornerIndex] - transform.position), Vector3.up)
            , rotateSpeed * Time.deltaTime);
    }

    public void StopMoving()
    {
        anim.SetFloat(forwardParamName, 0);
    }

    //타겟오브젝트 설정
    public void setTargetObject(GameObject target)
    {
        TargetObject = target;
    }
}
