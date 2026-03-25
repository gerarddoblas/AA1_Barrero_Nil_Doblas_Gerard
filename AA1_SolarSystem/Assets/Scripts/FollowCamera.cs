using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //Guardo en un array de transforms los planetas
    public Transform[] planets = new Transform[9];
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;

    [Header("Movement")]
    public float movementSpeed = 10f;
    public float rotateSpeed = 3f;
    private bool freeMovement = false;



    private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(planets.Length > 0)
            target = planets[0];
    }

    // Update is called once per frame
    void Update()
    {
        PlanetSelection();
        Zoom();
        FreeMovement();
        Rotation();
    }

    void LateUpdate()
    {
        if (freeMovement || target == null) return;

        Vector3 newPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }

    void PlanetSelection()
    {
        //Recorre todos los planetas
        for (int i = 0; i < planets.Length; i++)
        {
            //Si presiona el Input en donde justo está el planeta le asigna dicho planeta como target, ejemplo 2 = Mercurio
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && planets[i] != null)
            {
                target = planets[i];
                freeMovement = false;
            }
        }
    }

    //Acerco y alejo la cámara
    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        if (target != null)
        {
            //Calculo el radio del planeta según su escala local y el radio de la Mesh, que en todos es 0.5
            Vector3 direction = offset.normalized;
            float newDistance = offset.magnitude - scroll * zoomSpeed;

            float planetRadius = Mathf.Max(target.localScale.x, target.localScale.y, target.localScale.z) * 0.5f;
            newDistance = Mathf.Max(newDistance, planetRadius);

            offset = direction * newDistance;
        }
        else
            transform.position += transform.forward * scroll * zoomSpeed;
    }

    //Actualizo la posición de la cámara
    void FreeMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        if (move != Vector3.zero)
        {
            freeMovement = true;
            transform.position += move * movementSpeed * Time.deltaTime;
        }
    }
    //Rota con el click derecho
    void Rotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

            //Utilizo el Quaternion.AngleAxis para rotar el offset en vez de la cámara
            if (!freeMovement && target != null)
            {
                offset = Quaternion.AngleAxis(mouseX, Vector3.up) * offset;
                offset = Quaternion.AngleAxis(-mouseY, transform.right) * offset;
            }
            else
            {
                freeMovement = true;
                transform.Rotate(Vector3.up, mouseX, Space.World);
                transform.Rotate(Vector3.right, -mouseY, Space.Self);
            }
        }
    }
}
