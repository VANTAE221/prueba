using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class comportamientoCamara : MonoBehaviour
{
    public Transform rightController;      // Referencia al controlador derecho
    public Camera vrCamera;                // C�mara de la vista VR principal
    public float cameraViewDistance = 0.3f; // Distancia a la cara para activar la vista de c�mara
    public float zoomLevel = 2f;           // Nivel de zoom de la c�mara (ajustable)
    public LayerMask targetLayer;          // Capa para los GameObjects que queremos capturar (los que tienen el script de vuelo)

    public InputActionReference triggerPressedRef; //Referencia al input del trigger del control
    private const float maxDistance = 0.5f; // Distancia máxima para calificación (ajusta según necesidad)
    
    public bool isCameraViewActive = false;   // Indica si est� en vista de c�mara
    private float originalFOV;                // Almacena el FOV original de la c�mara

    public Text scoreTextPrefab; // Prefab del texto de puntaje
    public Transform canvasTransform; // Transform del Canvas
    public float textMoveSpeed = 50f; // Velocidad de movimiento del texto de puntaje
    public float fadeDuration = 1.5f; // Duración del desvanecimiento    


    void Start()
    {
        originalFOV = vrCamera.fieldOfView;
        triggerPressedRef.action.performed += gatilloDerechoPresioado;

    }

    //detecta cuando se presiona el gatillo del control derecho
    private void gatilloDerechoPresioado(InputAction.CallbackContext ctx)
    {
        if(isCameraViewActive){
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
        float score=0;
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
                        score = CalculateScore(obj.transform);
                        Debug.Log("Puntuación de la foto: " + score);
                        points++;
                        capturedObjects.Add(obj.transform.name);
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
        ShowFloatingScore((int)score);
    }

    private void ShowFloatingScore(int points)
    {
        // Instanciar el texto del puntaje y configurarlo en el Canvas
        Text scoreText = Instantiate(scoreTextPrefab, canvasTransform);
        scoreText.text = points.ToString();

        // Posiciona el texto en la esquina derecha del centro de la pantalla
        //scoreText.transform.position = vrCamera.WorldToScreenPoint(vrCamera.transform.position + vrCamera.transform.right * 0.2f);

        // Cambiar color del texto según el puntaje (de rojo a verde)
        float colorValue = Mathf.Clamp01(points / 100f);
        scoreText.color = Color.Lerp(Color.red, Color.green, colorValue);

        // Iniciar la animación de desvanecimiento
        StartCoroutine(FadeAndMoveText(scoreText));
    }

    private System.Collections.IEnumerator FadeAndMoveText(Text scoreText)
    {
        float elapsedTime = 0f;
        Color originalColor = scoreText.color;
        Vector3 originalPosition = scoreText.transform.position;

        while (elapsedTime < fadeDuration)
        {
            // Mover el texto hacia arriba
            scoreText.transform.position = originalPosition + Vector3.up * textMoveSpeed * (elapsedTime / fadeDuration);

            // Ajustar la opacidad gradualmente
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            scoreText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(scoreText.gameObject);
    }

    private float CalculateScore(Transform bird)
    {
        // Convertimos la posición del ave a la posición de la pantalla de la cámara
        Vector3 viewportPos = vrCamera.WorldToViewportPoint(bird.position);

        // Calculamos la distancia desde el centro de la pantalla (0.5, 0.5)
        float distanceFromCenter = Vector2.Distance(new Vector2(viewportPos.x, viewportPos.y), new Vector2(0.5f, 0.5f));

        // Normalizamos la distancia para obtener una puntuación entre 1 y 100
        float score = Mathf.Clamp(100 * (1 - (distanceFromCenter / maxDistance)), 1, 100);
        return score;
    }
}