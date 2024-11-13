using System.Collections;
using UnityEngine;

public class comportamientoaves : MonoBehaviour
{
    public float takeOffSpeed = 5f;         // Velocidad al despegar
    public float flightSpeed = 3f;          // Velocidad durante el vuelo
    public float landingSpeed = 2f;         // Velocidad al aterrizar
    public float flightHeight = 10f;        // Altura máxima de vuelo
    public float landingHeight = 1f;        // Altura de aterrizaje
    public float flightRadius = 20f;        // Radio de vuelo aleatorio
    public float idleTime = 3f;             // Tiempo de espera entre despegue y aterrizaje

    private enum BirdState { Idle, TakeOff, Flying, Landing }
    private BirdState currentState;
    private Vector3 targetPosition;

    private void Start()
    {
        currentState = BirdState.Idle;
        StartCoroutine(BirdRoutine());
    }

    private IEnumerator BirdRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case BirdState.Idle:
                    yield return new WaitForSeconds(idleTime);
                    currentState = BirdState.TakeOff;
                    break;

                case BirdState.TakeOff:
                    targetPosition = new Vector3(
                        transform.position.x + Random.Range(-flightRadius, flightRadius),
                        flightHeight,
                        transform.position.z + Random.Range(-flightRadius, flightRadius)
                    );
                    currentState = BirdState.Flying;
                    break;

                case BirdState.Flying:
                    FlyToTarget(targetPosition);

                    if (Vector3.Distance(transform.position, targetPosition) < 1f)
                    {
                        if (Random.value > 0.5f)
                        {
                            currentState = BirdState.Landing;
                            targetPosition.y = landingHeight;
                        }
                        else
                        {
                            targetPosition = new Vector3(
                                transform.position.x + Random.Range(-flightRadius, flightRadius),
                                flightHeight,
                                transform.position.z + Random.Range(-flightRadius, flightRadius)
                            );
                        }
                    }
                    break;

                case BirdState.Landing:
                    FlyToTarget(targetPosition);

                    if (transform.position.y <= landingHeight + 0.1f)
                    {
                        currentState = BirdState.Idle;
                    }
                    break;
            }

            yield return null;
        }
    }

    private void FlyToTarget(Vector3 target)
    {
        float speed = currentState == BirdState.Flying ? flightSpeed : landingSpeed;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(target);
    }
}
