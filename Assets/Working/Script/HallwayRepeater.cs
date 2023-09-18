using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayRepeater : MonoBehaviour
{
    Transform toObj;
    public BNG.ScreenFader fade;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Respawn"))
        {
            toObj = hit.transform.GetChild(0);
            StartCoroutine(TeleportToObj(hit));
        }
    }

    IEnumerator TeleportToObj(ControllerColliderHit hit)
    {
        GetComponent<CharacterController>().enabled = false;
        fade.DoFadeIn();

        yield return new WaitForSeconds(1.0f);
        
        gameObject.transform.SetPositionAndRotation(new Vector3(toObj.position.x, transform.position.y, toObj.position.z), toObj.rotation);
        GetComponent<CharacterController>().enabled = true;
        fade.DoFadeOut();

        yield return null;
    }
}
