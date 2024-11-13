using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class comportamientoCamara : MonoBehaviour
{
    public Transform rightController;      // Referencia al controlador derecho
    public XRController xrRightController;      // Referencia al controlador derecho
    public Camera vrCamera;                // C�mara de la vista VR principal
    public float cameraViewDistance = 0.3f; // Distancia a la cara para activar la vista de c�mara
    public float zoomLevel = 2f;           // Nivel de zoom de la c�mara (ajustable)
    public LayerMask targetLayer;          // Capa para los GameObjects que queremos capturar (los que tienen el script de vuelo)

    public InputActionReference triggerPressedRef;
    
    public bool isCameraViewActive = false;   // Indica si est� en vista de c�mara
    private float originalFOV;                // Almacena el FOV original de la c�mara

      
    void Start()
    {
        originalFOV = vrCamera.fieldOfView;
        triggerPressedRef.action.performed += gatilloDerechoPresioado;


    }

    //detecta cuando se presiona el gatillo del control derecho
    private void gatilloDerechoPresioado(InputAction.CallbackContext ctx)
    {
        if(isCameraViewActive){
            Debug.Log("asdasdasd");
            TakePhoto();
        }
    }

    void Update()
    {
        if (rightController == null)
            return;

        float distanceToController = Vector3.Distance(rightController.position, vrCamera.transform.position);

        if (distanceToController <= cameraViewDistance && !isCameraViewActive)
        {
            ActivateCameraView();
        }
        else if (distanceToController > cameraViewDistance && isCameraViewActive)
        {
            DeactivateCameraView();
        }


    }

    private void ActivateCameraView()
    {
        isCameraViewActive = true;
        vrCamera.fieldOfView = originalFOV / zoomLevel;
    }

    private void DeactivateCameraView()
    {
        isCameraViewActive = false;
        vrCamera.fieldOfView = originalFOV;
    }

    private void TakePhoto()
    {
        int points = 0;
        List<string> capturedObjects = new List<string>();

        // Raycasting para verificar objetos en el campo de visi�n de la c�mara
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Flyable")) // Aseg�rate de que los objetos voladores tengan la etiqueta "Flyable"
        {
            Vector3 screenPoint = vrCamera.WorldToViewportPoint(obj.transform.position);

            // Verifica si el objeto est� en la vista de la c�mara
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                if (Physics.Linecast(vrCamera.transform.position, obj.transform.position, out RaycastHit hit, targetLayer))
                {
                    if (hit.collider.gameObject == obj)
                    {
                        points++;
                        capturedObjects.Add(obj.name);
                    }
                }
            }
        }

        // Imprime el resultado de la foto
        foreach (string objName in capturedObjects)
        {
            Debug.Log($"Foto tomada al objeto: {objName}");
        }

        Debug.Log($"Puntos obtenidos: {points}");
    }
}