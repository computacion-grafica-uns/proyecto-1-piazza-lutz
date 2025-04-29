using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class proyecto : MonoBehaviour
{
    private FileReader lector = new FileReader();
    private GameObject miCamara;

    // Objetos de la escena
    private List<GameObject> monoAmbiente = new List<GameObject>();
    // Estructura
    private GameObject paredes, piso, techo;
    // Baño
    private GameObject toilet, bath, mirror, sink;
    // Dormitorio
    private GameObject bed, nightstand, closet;
    // Comedor
    private GameObject table, chair1, chair2, chair3, pot, planta;
    // Sala de estar
    private GameObject sofa, cafeTable;
    private GameObject horno, alacena1, alacena2, mesada, heladera;

    // Camaras
    private Vector3 targetOrbital = Vector3.zero;
    private float distancia = 7f;
    private float velocidadRotacion = 100f;
    private float mouseSensitivity = 800f;
    private float moveSpeed = 1.8f;
    private float pitch = 20f; //pitch
    private float yaw = 180f; //Yaw
    private float pitchFP = 0f;
    private float yawFP = 180f;
    private Vector3 posCamara, posCamaraFP, forwardOrbital, forwardFP, right, up;

    //Matriz jerarquica
    private Matrix4x4 modelMatrixPot;

    private bool isOrbital;

    // Start
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GenerateStruct();
        GenerateBathroom();
        GenerateBedroom();
        GenerateDining();
        CreateCamera();
        GenerateLiving();
        GenerateKitchen();
        GeneratePotPlant();

        RecalcularMatrices();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isOrbital = !isOrbital;
        }

        if (isOrbital)
        {
            camaraOrbital();
        }
        else
        {
            camaraFP();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (paredes.activeSelf)
            {
                paredes.SetActive(false);
            }
            else
            {
                paredes.SetActive(true);
            }
                
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (techo.activeSelf)
            {
                techo.SetActive(false);
            }
            else
            {
                techo.SetActive(true);
            }
        }
    }

    private void GenerateStruct()
    {
        lector.read("piso");
        lector.setColor(0.7f,0.7f,0.7f);
        piso = lector.getGameObject();
        CreateModel(piso, new Vector3(0,0,0), Vector3.zero, Vector3.one); 

        lector.read("techo");
        lector.setColor(180f/255f,200f/255f,210f/255f);
        techo = lector.getGameObject();
        CreateModel(techo, new Vector3(0,0,0), Vector3.zero, Vector3.one);

        lector.read("pared");
        lector.setColor(196f/255f,216f/255f,226f/255f);
        paredes = lector.getGameObject();
        CreateModel(paredes, new Vector3(0, 0, 0), Vector3.zero, Vector3.one);

        monoAmbiente.Add(piso);
        monoAmbiente.Add(techo);
        monoAmbiente.Add(paredes);
    }

    private void GenerateBathroom()
    {
        lector.read("toilet1");
        lector.setColor(0.9f, 0.9f, 0.9f);
        toilet = lector.getGameObject();
        monoAmbiente.Add(toilet);
        CreateModel(toilet, new Vector3(-4.5f,0,-2.5f), new Vector3(0,Mathf.Deg2Rad*-90,0), new Vector3(0.9f,0.9f,0.9f));

        lector.read("Bath");
        lector.setColor(0.9f, 0.9f, 0.9f);
        bath = lector.getGameObject();
        monoAmbiente.Add(bath);
        CreateModel(bath, new Vector3(-3.9f,0f,-1f), new Vector3(0,Mathf.Deg2Rad*-270,0), new Vector3(0.9f,0.9f,0.9f));

        lector.read("mirror");
        lector.setColor(0.9f,0.9f,0.9f);
        mirror = lector.getGameObject();
        monoAmbiente.Add(mirror);
        CreateModel(mirror, new Vector3(-3f,0,-2.98f), new Vector3(0, Mathf.Deg2Rad*-90,0), Vector3.one);

        lector.read("sink");
        lector.setColor(0.9f,0.9f,0.9f);
        sink = lector.getGameObject();
        monoAmbiente.Add(sink);
        CreateModel(sink, new Vector3(-3f,0,-2.69f), new Vector3(0, Mathf.Deg2Rad*-90,0), Vector3.one);
    }

    private void GenerateBedroom()
    {
        lector.read("bed2");
        lector.setColor(0.85f, 0.4f, 0.4f);
        bed = lector.getGameObject();
        monoAmbiente.Add(bed);
        CreateModel(bed, new Vector3(3.74f,-0.16f,-2.42f), new Vector3(0, Mathf.Deg2Rad*-90,0), Vector3.one);

        lector.read("littleOne");
        lector.setColor(0.85f, 0.65f, 0.5f);
        nightstand = lector.getGameObject();
        monoAmbiente.Add(nightstand);
        CreateModel(nightstand, new Vector3(4.71f,0,-1.45f), new Vector3(0, Mathf.Deg2Rad*180,0), Vector3.one);

        lector.read("closet");
        lector.setColor(0.6f, 0.45f, 0.3f);
        closet = lector.getGameObject();
        monoAmbiente.Add(closet);
        CreateModel(closet, new Vector3(1.2f,0,-1.79f), new Vector3(0, Mathf.Deg2Rad*0,0), new Vector3(0.87f,0.87f,0.87f));

    }

    private void GenerateDining()
    {
        lector.read("chair4");
        lector.setColor(0.6f, 0.45f, 0.4f);
        chair1 = lector.getGameObject();
        CreateModel(chair1, new Vector3(1,0,1.5f), new Vector3(0, Mathf.Deg2Rad*180), Vector3.one);

        lector.read("chair4");
        lector.setColor(0.6f, 0.45f, 0.4f);
        chair2 = lector.getGameObject();
        CreateModel(chair2, new Vector3(-1,0,1.5f), Vector3.zero, Vector3.one);

        lector.read("chair4");
        lector.setColor(0.6f, 0.45f, 0.4f);
        chair3 = lector.getGameObject();
        CreateModel(chair3, new Vector3(0,0,0.5f), new Vector3(0, Mathf.Deg2Rad*-90), Vector3.one);

        lector.read("table");
        lector.setColor(0.60f, 0.45f, 0.4f);
        table = lector.getGameObject();
        CreateModel(table, new Vector3(0,0,1.5f), Vector3.zero, Vector3.one);

        monoAmbiente.Add(chair1);
        monoAmbiente.Add(chair2);
        monoAmbiente.Add(chair3);
        monoAmbiente.Add(table);
    }

    private void GenerateLiving()
    {
        lector.read("sofaWithLegs");
        lector.setColor(0,0.7f,0.8f);
        sofa = lector.getGameObject();
        CreateModel(sofa, new Vector3(3.45f,0,2.40f),new Vector3(0,Mathf.Deg2Rad*90,0), Vector3.one);

        lector.read("table");
        lector.setColor(0.5f,0.3f,0.4f);
        cafeTable = lector.getGameObject();
        CreateModel(cafeTable, new Vector3(3.45f,0,0.7f),new Vector3(0,0,0), new Vector3(1, 0.4f, 0.5f));

        monoAmbiente.Add(sofa);
        monoAmbiente.Add(cafeTable);
    }

    private void GenerateKitchen()
    {
        lector.read("UpperCabinet");
        lector.setColor(0.65f, 0.50f, 0.45f);
        alacena1 = lector.getGameObject();
        CreateModel(alacena1, new Vector3(-3.6f,0.8f,2.67f), new Vector3(0,Mathf.Deg2Rad*90), new Vector3(1,0.6f,1));

        lector.read("UpperCabinet");
        lector.setColor(0.65f, 0.50f, 0.45f);
        alacena2 = lector.getGameObject();
        CreateModel(alacena2, new Vector3(-4.5f,0.8f,2.67f), new Vector3(0,Mathf.Deg2Rad*90), new Vector3(1,0.6f,1));

        lector.read("KitchenCabinetRounded");
        lector.setColor(0.85f,0.6f,0.75f);
        mesada = lector.getGameObject();
        CreateModel(mesada, new Vector3(-3.85f,0,1.85f),Vector3.zero,Vector3.one);

        lector.read("KitchenStoveWithOven");
        lector.setColor(0.4f,0.4f,0.5f);
        horno = lector.getGameObject();
        CreateModel(horno, new Vector3(-4.5f,0,0.2f),Vector3.zero,Vector3.one);

        lector.read("Fridge");
        lector.setColor(1,1,7f);
        heladera = lector.getGameObject();
        CreateModel(heladera, new Vector3(-2.3f,0,2.5f), new Vector3(0,Mathf.Deg2Rad*90), Vector3.one);

        
        monoAmbiente.Add(alacena1);
        monoAmbiente.Add(alacena2);
        monoAmbiente.Add(mesada);
        monoAmbiente.Add(horno);
        monoAmbiente.Add(heladera);
    }

    private void GeneratePotPlant()
    {
        lector.read("pot");
        lector.setColor(0.3f, 0.05f, 0.05f);
        pot = lector.getGameObject();

        Vector3 posPot = new Vector3(0,1.12f,1.5f);
        Vector3 rotPot = Vector3.zero;
        Vector3 scalePot = new Vector3(0.1f,0.1f,0.1f); 

        modelMatrixPot = CreateModelMatrix(posPot, rotPot, scalePot);
        pot.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrixPot);
        
        lector.read("plant");
        lector.setColor(0.05f, 0.3f, 0.05f);
        planta = lector.getGameObject();

        Vector3 relativePosition = new Vector3(0, 4f, -0.3f); // posición relativa al pot
        Vector3 relativeRotation = new Vector3(Mathf.Deg2Rad*-90, 0, 0); // rotación relativa
        Vector3 relativeScale = new Vector3(3f, 3f, 3f);

        Matrix4x4 relativeMatrix = CreateModelMatrix(relativePosition, relativeRotation, relativeScale);
        Matrix4x4 modelMatrixPlanta = modelMatrixPot * relativeMatrix;
        planta.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrixPlanta);

        monoAmbiente.Add(pot);
        monoAmbiente.Add(planta);
    }

    private void CreateCamera()
    {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;

        posCamara = new Vector3(0, 10f, 0);
        posCamaraFP = new Vector3(0,1.65f,-3f);
    }

    private void CreateModel(GameObject obj, Vector3 pos, Vector3 rot, Vector3 esc)
    {
        Matrix4x4 modelMatrix = CreateModelMatrix(pos, rot, esc);
        obj.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix",  modelMatrix);
    }

    private void RecalcularMatrices()
    {
        Vector3 camaraNueva = (isOrbital ? posCamara : posCamaraFP);
        Vector3 target = (isOrbital ? targetOrbital : posCamaraFP + forwardFP);
        Vector3 up = Vector3.up;

        Matrix4x4 viewMatrix = CreateViewMatrix(camaraNueva, target, up);
        Matrix4x4 projectionMatrix = CalculatePerspectiveProjectionMatrix(75f, (float)Screen.width / Screen.height, 0.1f, 100f);

        foreach (GameObject obj in monoAmbiente)
        {
            obj.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);
            obj.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projectionMatrix, true));
        }
    }

    private Matrix4x4 CreateViewMatrix(Vector3 pos, Vector3 target, Vector3 up)
    {
        Vector3 forward = (target - pos).normalized;
        Vector3 right = Vector3.Cross(up, forward).normalized;
        Vector3 newUp = Vector3.Cross(forward, right);

        Matrix4x4 view = new Matrix4x4(
            new Vector4(right.x, newUp.x, -forward.x, 0f),
            new Vector4(right.y, newUp.y, -forward.y, 0f),
            new Vector4(right.z, newUp.z, -forward.z, 0f),
            new Vector4(-Vector3.Dot(right, pos), -Vector3.Dot(newUp, pos), Vector3.Dot(forward, pos), 1f)
        );

        return view;
    }

    private Matrix4x4 CalculatePerspectiveProjectionMatrix(float fov, float aspect, float near, float far)
    {
        float f = 1f / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

        Matrix4x4 projection = new Matrix4x4();
        projection[0, 0] = f / aspect;
        projection[1, 1] = f;
        projection[2, 2] = (far + near) / (near - far);
        projection[2, 3] = (2f * far * near) / (near - far);
        projection[3, 2] = -1f;
        projection[3, 3] = 0f;

        return projection;
    }

    private void camaraOrbital()
    {
        // Movimiento con flechas del teclado
        if (Input.GetKey(KeyCode.UpArrow)) pitch += velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) pitch -= velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) yaw += velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) yaw -= velocidadRotacion * Time.deltaTime;

        // Limitar pitch (evita que se dé vuelta completamente)
        pitch = Mathf.Clamp(pitch, -0.5f, 60f);
        float RadX = Mathf.Deg2Rad * pitch;
        float RadY = Mathf.Deg2Rad * yaw;

        forwardOrbital = new Vector3(
            distancia * MathF.Cos(RadX) * Mathf.Sin(RadY),
            distancia * Mathf.Sin(RadX),
            distancia * Mathf.Cos(RadX) * Mathf.Cos(RadY)
        );

        posCamara = targetOrbital + forwardOrbital;
        up = Vector3.up;

        RecalcularMatrices();
    }

    private void camaraFP()
    {
        yawFP += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitchFP += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitchFP = Mathf.Clamp(pitchFP, -89f, 89f);

        float RadX = Mathf.Deg2Rad * pitchFP;
        float RadY = Mathf.Deg2Rad * yawFP;

        forwardFP = new Vector3(
            Mathf.Cos(RadX) * Mathf.Sin(RadY),
            Mathf.Sin(RadX),
            Mathf.Cos(RadX) * Mathf.Cos(RadY)
        );

        right = Vector3.Cross(Vector3.up, forwardFP).normalized;
        up = Vector3.Cross(forwardFP, right).normalized;

        Vector3 movimiento = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movimiento += forwardFP;
        if (Input.GetKey(KeyCode.S)) movimiento -= forwardFP;
        if (Input.GetKey(KeyCode.A)) movimiento -= right;
        if (Input.GetKey(KeyCode.D)) movimiento += right;
        movimiento.y = 0f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            posCamaraFP += movimiento * moveSpeed * 2.5f * Time.deltaTime;
        }
        else
        {
            posCamaraFP += movimiento * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            posCamaraFP.y = 0.8f;
        }
        else
        {
            posCamaraFP.y = 1.65f;
        }
        

        RecalcularMatrices();
    }

    private Matrix4x4 CreateModelMatrix(Vector3 newPosition, Vector3 newRotation, Vector3 newScale)
    {
        Matrix4x4 positionMatrix = new Matrix4x4(
            new Vector4(1f, 0f, 0f, newPosition.x), // Primera columna 
            new Vector4(0f, 1f, 0f, newPosition.y), // Segunda columna 
            new Vector4(0f, 0f, 1f, newPosition.z), // Tercera columna 
            new Vector4(0f, 0f, 0f, 1f) // Cuarta columna 
        );
        positionMatrix = positionMatrix.transpose;

        Matrix4x4 rotationMatrixX = new Matrix4x4(
            new Vector4(1f, 0f, 0f, 0f), // Primera columna 
            new Vector4(0f, Mathf.Cos(newRotation.x), -Mathf.Sin(newRotation.x), 0f), // Segunda columna 
            new Vector4(0f, Mathf.Sin(newRotation.x), Mathf.Cos(newRotation.x), 0f), // Tercera columna 
            new Vector4(0f, 0f, 0f, 1f) // Cuarta columna 
        );
        Matrix4x4 rotationMatrixY = new Matrix4x4(
            new Vector4(Mathf.Cos(newRotation.y), 0f, Mathf.Sin(newRotation.y), 0f), // Primera columna 
            new Vector4(0f, 1f, 0f, 0f), // Segunda columma 
            new Vector4(-Mathf.Sin(newRotation.y), 0f, Mathf.Cos(newRotation.y), 0f), // Tercera columna 
            new Vector4(0f, 0f, 0f, 1f) // Cuarta columna 
        );
        Matrix4x4 rotationMatrixZ = new Matrix4x4(
        new Vector4(Mathf.Cos(newRotation.z), -Mathf.Sin(newRotation.z), 0f, 0f), // Primera columna 
        new Vector4(Mathf.Sin(newRotation.z), Mathf.Cos(newRotation.z), 0f, 0f), // Segunda columna 
        new Vector4(0f, 0f, 1f, 0f), // Tercera columna 
        new Vector4(0f, 0f, 0f, 1f) // Cuarta columna 
        );

        Matrix4x4 rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
        rotationMatrix = rotationMatrix.transpose;

        Matrix4x4 scaleMatrix = new Matrix4x4(
        new Vector4(newScale.x, 0f, 0f, 0f), // Primera columna 
        new Vector4(0f, newScale.y, 0f, 0f), // Segunda columna 
        new Vector4(0f, 0f, newScale.z, 0f), // Tercera columna 
        new Vector4(0f, 0f, 0f, 1f) // Cuarta columna 
        );
        scaleMatrix = scaleMatrix.transpose;

        Matrix4x4 finalMatrix = positionMatrix;
        finalMatrix *= rotationMatrix;
        finalMatrix *= scaleMatrix;
        return (finalMatrix);
    }
}