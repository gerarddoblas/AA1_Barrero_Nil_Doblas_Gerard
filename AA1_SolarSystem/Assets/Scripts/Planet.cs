using UnityEngine;
using TMPro;

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

    public float stepTime = 0.001f;
    public float minStepTime = 0.0001f;
    public float maxStepTime = 0.005f;

    

    private float energy;

    public bool showEnergy = false;

    public TextMeshProUGUI methodText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI stepText;

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
        //inicia la simulacion
        ResetSimulation();

        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        //control de la velocidad con flechas
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stepTime *= 2f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stepTime *= 0.5f;
        }

        stepTime = Mathf.Clamp(stepTime, minStepTime, maxStepTime);

        //cambiar de metodo con 'E'

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
            (position, velocity, acceleration, time) =
                Verlet(position, velocity, acceleration, time);
        }

        //actualiza la posicion del objeto en unity
        transform.position = position;

        //calculamos la energia total
        energy = CalculateEnergy(position, velocity);

        //mostrar que método esta activo
        if (method == SimulationMethod.RungeKutta4)
            methodText.text = "Method: Runge-Kutta 4";
        else
            methodText.text = "Method: Verlet";


        //mostrar energia
        if (showEnergy)
        {
            energyText.text = "Energy: " + energy.ToString("F5");
        }

        stepText.text = "StepTime: " + stepTime.ToString("F5");
    }
    //cambiamos entre RK4 o verlet
    void ChangeMethod()
    {
        if (method == SimulationMethod.RungeKutta4)
            method = SimulationMethod.Verlet;
        else
            method = SimulationMethod.RungeKutta4;

        ResetSimulation();
    }

    //reinicia la simulacion
    void ResetSimulation()
    {
        position = initialPosition;
        velocity = initialVelocity;
        acceleration = CalculateAcceleration(position);

        time = 0;

        transform.position = position;
    }

    //calculamos aceleracion gravitatoria
    Vector3 CalculateAcceleration(Vector3 position)
    {
        Vector3 newAcceleration;

        float distanceSquared = position.magnitude * position.magnitude;
        Vector3 unitVecor = position.normalized;
        newAcceleration = -(sun.mass / distanceSquared) * unitVecor;
        return newAcceleration;
    }

    //RK4
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

    //Verlet
    (Vector3, Vector3, Vector3, float) Verlet(Vector3 position, Vector3 velocity, Vector3 acceleration, float time)
    {
        Vector3 newPosition = position + velocity * stepTime + 0.5f * acceleration * stepTime * stepTime;
        Vector3 newAcceleration = CalculateAcceleration(newPosition);
        Vector3 newVelocity = velocity + 0.5f * (acceleration + newAcceleration) * stepTime;
        time += stepTime;

        return (newPosition, newVelocity, newAcceleration, time);
    }

    //energia total
    float CalculateEnergy(Vector3 position, Vector3 velocity)
    {
        //energia cinetica
        float cinetic = 0.5f * (velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);

        //distancia sol
        float distance = Mathf.Sqrt(position.x * position.x + position.y * position.y + position.z * position.z);

        //energia potencial
        float potential = -sun.mass / distance;

        return cinetic + potential;
    }
}
