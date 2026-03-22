using UnityEngine;

public class Mercury : MonoBehaviour
{
    public enum SimulationMethod
    {
        RungeKutta4,
        Verlet
    }

    [Header("Simulation")]
    public SimulationMethod method = SimulationMethod.RungeKutta4;

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
        ResetSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeMethod();
        }
        if (method == SimulationMethod.RungeKutta4)
        {
            (position, velocity, time) = RungeKutta4(position, velocity, time);
        }
        else
        {
            (position, velocity, accelleration, time) =
                VerletMethod(position, velocity, accelleration, time);
        }

        transform.position = position;

        (position, velocity, time) = RungeKutta4(position, velocity, time);

        (position, velocity, accelleration, time) = VerletMethod(position, velocity, accelleration, time);

        transform.position = position;
    }

    void ChangeMethod()
    {
        if (method == SimulationMethod.RungeKutta4)
            method = SimulationMethod.Verlet;
        else
            method = SimulationMethod.RungeKutta4;

        ResetSimulation();
    }

    void ResetSimulation()
    {
        position = initialPosition;
        velocity = initialVelocity;
        accelleration = CalculateAcceleration(position);
        time = 0;

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

    (Vector2, Vector2, Vector2, float) VerletMethod(Vector2 position, Vector2 velocity, Vector2 acceleration, float time)
    {
        Vector2 newPosition = position + velocity * stepTime + 0.5f * acceleration * stepTime * stepTime;
        Vector2 newAcceleration = CalculateAcceleration(newPosition);
        Vector2 newVelocity = velocity + 0.5f * (acceleration + newAcceleration) * stepTime;
        time += stepTime;

        return (newPosition, newVelocity, newAcceleration, time);
    }

    void OnGUI()
    {
        string methodName = (method == SimulationMethod.RungeKutta4) ? "Runge-Kutta 4" : "Verlet";

        GUI.Label(new Rect(10, 10, 200, 30), "Method: " + methodName);
    }
}
