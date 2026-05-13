using UnityEngine;
using UnityEngine.UI;

public class CursorController2D : MonoBehaviour
{
    public RectTransform canvasRect; 
    private RectTransform myRect;
    
    public Camera worldCamera; 
    public LayerMask eventRobotLayer; 

    void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Sicherstellen, dass der Skywriter da ist
        if (SkywriterInputManager.Instance == null) return;

        float x = SkywriterInputManager.Instance.InputX;
        float y = SkywriterInputManager.Instance.InputY;

        float xPos = (x - 0.5f) * canvasRect.rect.width;
        float yPos = (y - 0.5f) * canvasRect.rect.height;

        Vector2 targetPos = new Vector2(xPos, yPos);
        
        myRect.anchoredPosition = Vector2.Lerp(myRect.anchoredPosition, targetPos, Time.deltaTime * 15f);

        CheckInteraction(x, y);
    }

    void CheckInteraction(float xRaw, float yRaw)
    {
        if (SkywriterInputManager.Instance.IsTapped)
        {
            Vector3 screenPos = new Vector3(xRaw * Screen.width, yRaw * Screen.height, 0);
            
            Ray ray = worldCamera.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, eventRobotLayer))
            {
                Debug.Log("Getroffen: " + hit.collider.name);
                
                // Event starten
                GameManager.Instance.StartEventSequence(hit.collider.gameObject);

                // WICHTIG: Cursor sofort unsichtbar machen!
                gameObject.SetActive(false); 
            }
        }
    }
}