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
    private GameObject paredes;
    private GameObject piso;
    private GameObject techo;
    private GameObject toilet;

    // Camaras
    private Vector3 targetOrbital = Vector3.zero;
    private float distancia = 10f;
    private float velocidadRotacion = 100f;
    private float mouseSensitivity = 1000f;
    private float moveSpeed = 5f;
    private float pitch = 20f; //pitch
    private float yaw = 180f; //Yaw
    private float pitchFP = 0f;
    private float yawFP = 180f;
    private Vector3 posCamara, posCamaraFP, forwardOrbital, forwardFP, right, up;
    private bool isOrbital;

    // Start
    void Start()
    {
        generarEstructura();
        GenerateBathroom();
        CreateCamera();

        RecalcularMatrices();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.V))
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
    }

    private void generarEstructura()
    {
        lector.read("piso");
        lector.setColor(210f / 255f, 105f / 255f, 30f / 255f);
        piso = lector.getGameObject();
        CreateModel(piso, Vector3.zero, Vector3.zero, Vector3.one); 

        lector.read("techo");
        lector.setColor(1,0,0);
        techo = lector.getGameObject();
        CreateModel(techo, new Vector3(0,2.5f,0), Vector3.zero, Vector3.one);

        lector.read("pared_fix_v2");
        paredes = lector.getGameObject();
        CreateModel(paredes, new Vector3(0, 1.25f, 0), Vector3.zero, Vector3.one);

        monoAmbiente.Add(piso);
        monoAmbiente.Add(techo);
        monoAmbiente.Add(paredes);
    }

    private void GenerateBathroom()
    {
        lector.read("toilet1");
        lector.setColor(1, 1, 1);
        toilet = lector.getGameObject();
        monoAmbiente.Add(toilet);
    }

    private void CreateCamera()
    {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;

        posCamara = new Vector3(0, 10f, 0);
        posCamaraFP = new Vector3(0,1.7f,-3f);
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
        pitch = Mathf.Clamp(pitch, -15f, 60f);
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

        posCamaraFP += movimiento * moveSpeed * Time.deltaTime;

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