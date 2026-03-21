using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("References")]
    public SunData sun;

    [Header("Planet properties")]

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    [Header("Dynamics setup")]

    public Vector3 initialPosition;
    public Vector3 initialVelocity;


    public float totalTime = 100;
    private float time = 0;
    public float stepTime = 0.01f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = initialPosition;
        velocity = initialVelocity;

        transform.position = position;
    }
    // Update is called once per frame
    void Update()
    {
        if (time < totalTime)
        {
            acceleration = CalculateAcceleration(position);
            (position, velocity, time) = RungeKutta4(position, velocity, time);

            transform.position = position;
        }
    }

    Vector3 CalculateAcceleration(Vector3 position)
    {
        Vector3 newAcceleration;

        float distanceSquared = position.magnitude * position.magnitude;
        Vector3 unitVecor = position.normalized;
        newAcceleration = -(sun.mass / distanceSquared) * unitVecor;
        return newAcceleration;
    }

    (Vector3, Vector3, float) RungeKutta4(Vector3 position, Vector3 velocity, float time)
    {
        Vector3 K1p, K1v, K2p, K2v, K3p, K3v, K4p, K4v;

        Vector3 newPosition, newVelocity;

        K1p = velocity;
        K1v = CalculateAcceleration(position);
        K2p = velocity + 0.5f * stepTime * K1v;
        K2v = CalculateAcceleration(position + 0.5f * stepTime * K1p);
        K3p = velocity + 0.5f * stepTime * K2v;
        K3v = CalculateAcceleration(position + 0.5f * stepTime * K2p);
        K4p = velocity + stepTime * K3v;
        K4v = CalculateAcceleration(position + stepTime * K3p);

        newPosition = position + stepTime / 6f * (K1p + 2 * K2p + 2 * K3p + K4p);
        newVelocity = velocity + stepTime / 6f * (K1v + 2 * K2v + 2 * K3v + K4v);

        time += stepTime;

        return (newPosition, newVelocity, time);
    }
}
