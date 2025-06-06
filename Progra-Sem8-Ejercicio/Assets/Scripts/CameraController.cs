using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 20, 0); 
    public float smoothSpeed = 0.125f; 
    
    public Vector2 mapBoundsX = new Vector2(-10, 10); // Límites X del mapa
    public Vector2 mapBoundsZ = new Vector2(-10, 10); // Límites Z del mapa

    void LateUpdate()
    {
        if (target == null)
        {            
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("CameraController: Target (Player) no encontrado.");
                return;
            }
        }

        Vector3 desiredPosition = target.position + offset;
               
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

    }
}