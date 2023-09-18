using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrop : MonoBehaviour
{
    public List<GameObject> dropPointList;
    public List<Color> colors;
    public GameObject prefab_Ball;
    GameObject ball;
    int ranNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBall());
    }

    IEnumerator SpawnBall()
    {
        while (true)
        {
            ranNum = Random.Range(0, dropPointList.Count);
            ball = Instantiate(prefab_Ball, dropPointList[ranNum].transform.position, Quaternion.identity, dropPointList[ranNum].transform);
            ball.GetComponent<MeshRenderer>().material.color = colors[Random.Range(0, colors.Count)];
            StartCoroutine(DropBall(ball));
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator DropBall(GameObject obj)
    {
        while (true)
        {
            if (obj.transform.position.y <= 0)
                break;
            obj.transform.Translate(Vector3.down * Time.deltaTime / 2);
            yield return null;
        }
        Destroy(obj);
        yield return null;
    }
}
