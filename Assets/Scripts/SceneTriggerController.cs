using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SceneTriggerController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.LoadNextScene();
    }
}
