using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class PuzzleGame : MonoBehaviour
{
    public List<Transform> pieces;
    private List<Transform> pieces_Shuffle = new List<Transform>();
    private List<int> randNum = new List<int>();

    TriggerSet trigger;
    Transform first_Piece = null;
    bool isReady = false;
    void Start()
    {
        int rand;

        for (int i = 0; i < pieces.Count; i++)      // 퍼즐 조각을 무작위로 섞어 배치합니다.
        {
            do
            {
                rand = Random.Range(0, pieces.Count);
            } while (randNum.Contains(rand));
            pieces_Shuffle.Add(pieces[rand]);
            pieces[rand].localPosition = new Vector3(0.203f * (i % 3) - 0.203f, 0.3f - 0.2f * (i / 3), pieces[rand].localPosition.z);
            randNum.Add(rand);
        }

        if (!isReady)
        {
            isReady = true;
            gameObject.SetActive(false);
        }

        ANM_Basic_Start();
    }

    public void SetTrigger(TriggerSet t)
    {
        t.AddTrigger();
        trigger = t;
    }

    /// <summary>
    /// 선택된 조각끼리 위치를 바꿉니다. 퍼즐이 모두 맞춰지면 트리거를 활성화합니다.
    /// </summary>
    public void SwapPieces()
    {
        if (first_Piece == null)
        {
            first_Piece = EventSystem.current.currentSelectedGameObject.transform.parent;

            return;
        }
        else
        {
            if (first_Piece == EventSystem.current.currentSelectedGameObject.transform.parent)
            {
                first_Piece = null;
                return;
            }

            Transform second_Piece = EventSystem.current.currentSelectedGameObject.transform.parent;
            Vector3 tempPos = first_Piece.localPosition;
            first_Piece.localPosition = second_Piece.localPosition;
            second_Piece.localPosition = tempPos;

            int tempIndex = pieces_Shuffle.IndexOf(first_Piece);
            pieces_Shuffle[pieces_Shuffle.IndexOf(second_Piece)] = first_Piece;
            pieces_Shuffle[tempIndex] = second_Piece;

            first_Piece = null;

            if (pieces.SequenceEqual(pieces_Shuffle))
            {
                //trigger.SwitchOn();

                // 황영재 추가.
                ANM_Basic_SwapPieces();
            }
        }
    }

    //
    private void Update()
    {
        ANM_Basic_Update();
    }
}

partial class PuzzleGame
{
    [Header("BASIC ==================================================")]
    [SerializeField] ANM_Manager Basic_Manager;
    [SerializeField] float Basic_distanceMin;
    [SerializeField] float Basic_distanceMax;

    [Header("RUNNING")]
    [SerializeField] float Basic_distance;
    [SerializeField] float Basic_distanceRange;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    void ANM_Basic_SwapPieces()
    {
        Basic_Manager.ANM_Event_Trigger(this.gameObject);
    }

    ////////// Unity            //////////
    void ANM_Basic_Start()
    {
        Basic_distanceRange = Basic_distanceMax - Basic_distanceMin;
    }

    void ANM_Basic_Update()
    {
        Vector3 pos = this.transform.position;
        pos -= Basic_Manager.ANM_Player_body.position;
        float distance = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2) + Mathf.Pow(pos.z, 2));
        if (Basic_distance != distance)
        {
            Debug.Log(distance);
            Basic_distance = distance;

            float alpha = 0.0f;
            if(Basic_distance < Basic_distanceMin)
            {
                alpha = 1.0f;
            }
            else if(Basic_distance < Basic_distanceMax)
            {
                alpha = 1.0f - ((Basic_distance - Basic_distanceMin) / Basic_distanceRange);
            }

            for(int i = 0; i < pieces.Count; i++)
            {
                Color color = pieces[i].Find("Text").GetComponent<MeshRenderer>().materials[0].GetColor("_FaceColor");
                color.a = alpha;
                pieces[i].Find("Text").GetComponent<MeshRenderer>().materials[0].SetColor("_FaceColor", color);
            }
        }
    }
}