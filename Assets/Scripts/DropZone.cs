using UnityEngine;

public class DropZone : MonoBehaviour
{
    [Header("UI")]
    public GameObject successText; 

    private bool finished = false;

    void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Package") || other.CompareTag("Player")) 
        {
            Finish();
        }
    }

    void Finish()
    {
        finished = true;
        
        if (successText) successText.SetActive(true);
        
        GameManager.Instance.LevelCompleted();
    }
}