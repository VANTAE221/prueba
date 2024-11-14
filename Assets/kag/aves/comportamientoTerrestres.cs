using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class comportamientoTerrestres : MonoBehaviour
{
    public float moveSpeed = 2f;            // Velocidad de movimiento
    public float rotationSpeed = 120f;      // Velocidad de rotaci�n
    public float idleTime = 1.5f;           // Tiempo de espera entre movimientos
    public float moveDistance = 3f;         // Distancia m�xima de cada movimiento

    private Rigidbody rb;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Configura el Rigidbody para evitar ca�das
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        
        StartCoroutine(RandomMovement());
    }

    private IEnumerator RandomMovement()
    {
        while (true)
        {
            // Espera un tiempo aleatorio antes de moverse de nuevo
            yield return new WaitForSeconds(Random.Range(idleTime, idleTime * 2));

            // Genera una direcci�n aleatoria y rota la gallina en el eje Y
            float randomAngle = Random.Range(-180f, 180f);
            Quaternion targetRotation = Quaternion.Euler(0, randomAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Mueve la gallina hacia adelante en la nueva direcci�n
            isMoving = true;
            float moveDuration = moveDistance / moveSpeed;
            float timer = 0f;

            while (timer < moveDuration)
            {
                rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            // Detener el movimiento despu�s de avanzar
            isMoving = false;
        }
    }

    void Update()
    {
        // Si necesitas animar mientras la gallina se mueve, puedes usar `isMoving`
        if (isMoving)
        {
            // A�adir c�digo para la animaci�n aqu�
        }
    }
}