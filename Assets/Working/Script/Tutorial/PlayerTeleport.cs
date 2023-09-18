using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public GameObject player;
    public GameObject obj;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = obj.transform.position;
    }

    public void Teleport()
    {
        player.transform.position = new Vector3(pos.x, player.transform.position.y, pos.z);
    }   
}
