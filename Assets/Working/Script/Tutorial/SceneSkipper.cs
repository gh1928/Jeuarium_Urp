using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSkipper : MonoBehaviour
{


    public int sceneNum;
    public CapsuleCollider capsuleCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneNum);
        }
    }
}
