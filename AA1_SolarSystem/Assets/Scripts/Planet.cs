using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("References")]
    public SunData sun;

    [Header("Planet properties")]

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 accelleration;

    [Header("Dynamics setup")]

    public Vector3 initialPosition;
    public Vector3 initialVelocity;


    public float totalTime = 100;
    private float time = 0;
    public float stepTime = 0.01f;

    [Header("Simulation")]
    public SimulationMethod method = SimulationMethod.RungeKutta4;

    public enum SimulationMethod
    {
        RungeKutta4,
        Verlet
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetSimulation();
        position = initialPosition;
        velocity = initialVelocity;

        transform.position = position;
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

        if (time < totalTime)
        {
            accelleration = CalculateAcceleration(position);
            (position, velocity, time) = RungeKutta4(position, velocity, time);

            transform.position = position;
        }
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
    (Vector3, Vector3, Vector3, float) VerletMethod(Vector3 position, Vector3 velocity, Vector3 acceleration, float time)
    {
        Vector3 newPosition = position + velocity * stepTime + 0.5f * acceleration * stepTime * stepTime;
        Vector3 newAcceleration = CalculateAcceleration(newPosition);
        Vector3 newVelocity = velocity + 0.5f * (acceleration + newAcceleration) * stepTime;
        time += stepTime;

        return (newPosition, newVelocity, newAcceleration, time);
    }
    void OnGUI()
    {
        string methodName = (method == SimulationMethod.RungeKutta4) ? "Runge-Kutta 4" : "Verlet";

        GUI.Label(new Rect(10, 10, 200, 30), "Method: " + methodName);
    }

}
