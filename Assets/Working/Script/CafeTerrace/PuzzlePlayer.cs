using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePlayer : MonoBehaviour
{
    public static PuzzlePlayer Instance;

    private CanvasGroup canvasGroup;

    private PointerEventData eventData;
    
    private PuzzleMaker puzzleMaker;    
    private PuzzleNode[,] puzzle;
    private List<PuzzleElement> elements;
    private bool isPlaying = false;    

    public float speedMult = 0.5f;    
    
    private PuzzleNode currNode;
    private PuzzleNode destNode;
    private Transform indicator;

    private int enterNodeNum;
    private int exitNodeNum;
 
    private Stack<PuzzleNode> pathStack = new Stack<PuzzleNode>();

    private int[] yDir = { 1, 0, -1, 0 };
    private int[] xDir = { 0, 1, 0, -1 };

    private float clampValue;
    private float enterClampValue;

    public float minDistanceToMove = 1f;
    public float minDistanceToExitNode = 1f;

    public float fadeTime = 1f;
    public float puzzleChangeInterval = 1f;

    public AudioSource successAudio;
    public AudioSource failureAudio;
    public AudioSource elementSound;

    private CaffeEventHandler eventHandler;

    public float failEffectDelay = 2f;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {   
        canvasGroup = GetComponent<CanvasGroup>();
        puzzleMaker = GetComponent<PuzzleMaker>();
        eventHandler = GetComponent<CaffeEventHandler>();
        eventData = VRUISystem.Instance.EventData;
    }
    public void StartPlay()
    {
        if (isPlaying)
            return;

        puzzle = puzzleMaker.GetPuzzle();
        elements = puzzleMaker.GetInstancedElements();
        pathStack.Clear();

        SetClampValue();
        ActivePoint(puzzleMaker.GetEnterPoint());
        indicator = puzzleMaker.GetIndicator().transform;
        currNode = puzzleMaker.GetEnterNode();
        enterNodeNum = currNode.NodeNumber;
        exitNodeNum = puzzleMaker.GetExitNode().NodeNumber;

        currNode.OnVisitAction();

        isPlaying = true;
    }
    private void SetClampValue()
    {
        float interval = puzzleMaker.CurrData.nodeInterval;

        clampValue = (interval - 0.15f) / interval;

        enterClampValue = (interval - 0.225f) / interval;
    }
    public void ActivePoint(RawImage point) => point.color = puzzleMaker.CurrData.playerColor;

    private void Update()
    {
        if (!isPlaying)
            return;

        if (eventData.pointerCurrentRaycast.gameObject == null)
            return;

        var currTarget = eventData.pointerCurrentRaycast.module;

        if (currTarget == null || currTarget.GetType() != typeof(GraphicRaycaster))
            return;

        Vector3 currInputPos = eventData.pointerCurrentRaycast.worldPosition;            

        Vector3 pointerDir = currInputPos - indicator.position;

        if (Vector3.SqrMagnitude(pointerDir) < minDistanceToMove)
            return;
        
        float currProgress = currNode.GetProgress();

        var puzzleDir = GetLargeDirValue(pointerDir);        

        if (currProgress <= 0f)
        {
            if (Vector3.SqrMagnitude(pointerDir) < minDistanceToExitNode)
                return;
            
            SetDestNode(puzzleDir);
        }

        if (destNode == null)
            return;

        //currNode.UpdateLine(pointerDir, speed * Time.deltaTime);

        currNode.UpdateProgress(speedMult * Time.deltaTime * pointerDir);

        indicator.transform.position = Vector3.Lerp(currNode.transform.position, destNode.transform.position, currProgress);

        if (destNode.IsVisited())
            currNode.ClampProgress(clampValue);

        if(destNode.NodeNumber == enterNodeNum)
            currNode.ClampProgress(enterClampValue);

        if (currProgress >= 1f)
        {   
            VisitDestNode();
        }
    }

    private void SetDestNode(PuzzleDir dir)
    {
        destNode = null;

        if (dir == PuzzleDir.None)
            return;        

        if (!currNode.GetPathable(dir))
            return;

        if (pathStack.Count > 0 && dir == pathStack.Peek().GetOppositeDirection())
        {
            currNode.CancleVisit();

            destNode = currNode;
            currNode = pathStack.Pop();

            return;
        } 

        int Idx = (int)dir;
        destNode = puzzle[currNode.Pos.posY + yDir[Idx], currNode.Pos.posX + xDir[Idx]];

        currNode.SetDestDir(dir);        
    }

    private void VisitDestNode()
    {
        destNode.OnVisitAction();

        if (destNode.NodeNumber == exitNodeNum)
        {
            VisitExitNode();
            return;
        }

        pathStack.Push(currNode);
        currNode = destNode;        
    }
    private void VisitExitNode()
    {
        isPlaying = false;        

        ActivePoint(puzzleMaker.GetExitPoint());

        if (IsAllElemntsWorked())
        {
            StartCoroutine(PuzzleSuccessCoroutine());
            return;
        }

        StartCoroutine(PuzzleFailureCoroutine());
    }    

    private bool IsAllElemntsWorked()
    {
        for(int i = 0; i < elements.Count; i++)
        {
            if(!elements[i].IsWorked())
                return false;
        }

        return true;
    }
    private void ClearGame()
    {
        Debug.Log("게임 클리어");
    }
    private IEnumerator PuzzleFadeCoroutine(bool inToOut)
    {
        float timer = 0f;
        float inverseFadeTime = 1 / fadeTime;

        float sour = inToOut ? 1 : 0;
        float dest = inToOut ? 0 : 1;

        while (timer < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(sour, dest, timer * inverseFadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = dest;

        yield break;
    }

    private IEnumerator PuzzleFailureCoroutine()
    {
        failureAudio.Play();
        foreach (var element in elements)
        {
            if (!element.IsWorked())
                element.PlayFailEffect();
        }

        StartCoroutine(PuzzleFadeCoroutine(true));

        yield return new WaitForSeconds(fadeTime + failEffectDelay);

        puzzleMaker.ResetPuzzle();

        StartCoroutine(PuzzleFadeCoroutine(false));
    }
    private IEnumerator PuzzleSuccessCoroutine()
    {
        successAudio.Play();
        StartCoroutine(PuzzleFadeCoroutine(true));

        var evt = puzzleMaker.GetCurrEvt();

        if (evt != null)
            evt.OnEvent(elements);

        yield return new WaitForSeconds(evt.GetEvtTime());

        if (!puzzleMaker.IsRemainPuzzle())
        {
            ClearGame();
            yield break;
        }

        puzzleMaker.SetNextStep();
        puzzleMaker.ResetPuzzle();

        StartCoroutine(PuzzleFadeCoroutine(false));

        yield break;
    }    

    private void ResetGame()
    {
        puzzleMaker.ResetPuzzle();
    }

    private PuzzleDir GetLargeDirValue(Vector2 look)
    {
        if (look == Vector2.zero)
            return PuzzleDir.None;

        if(Mathf.Abs(look.x) > Mathf.Abs(look.y))
        {
            return look.x > 0 ? PuzzleDir.Left : PuzzleDir.Right;
        }

        return look.y > 0 ? PuzzleDir.Up : PuzzleDir.Down;
    }
    public void StopPlay() => isPlaying = false;
    public void PlayElementSound() => elementSound.Play();
}
