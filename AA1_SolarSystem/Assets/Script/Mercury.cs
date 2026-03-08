using UnityEngine;

public class Mercury : MonoBehaviour
{
    [Header("Earth properties")]

    private Vector2 position;
    private Vector2 velocity;
    private Vector2 accelleration;

    [Header("Dynamics setup")]

    private float gravityMassConstant = 39.478f;//1.66f * Mathf.Pow(10, -7);
    public Vector2 initialPosition = new Vector2(0.39f, 0);
    public Vector2 initialVelocity = new Vector2(0, 10.07f);


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
        accelleration = CalculateAcceleration(position);
        (position, velocity, time) = RungeKutta4(position, velocity, time);

        transform.position = position;
    }

    Vector2 CalculateAcceleration(Vector2 position)
    {
        Vector2 newAcceleration;

        float distanceSquared = position.magnitude * position.magnitude;
        Vector2 unitVecor = position.normalized;
        newAcceleration = -(gravityMassConstant / distanceSquared) * unitVecor;
        return newAcceleration;
    }

    (Vector2, Vector2, float) RungeKutta4(Vector2 position, Vector2 velocity, float time)
    {
        Vector2 K1p, K1v, K2p, K2v, K3p, K3v, K4p, K4v;

        Vector2 newPosition, newVelocity;

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
