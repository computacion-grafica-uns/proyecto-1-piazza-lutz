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

    //Variables para la camara
    private Transform t;
    private float distancia = 10f;
    private float velocidadRotacion = 50f;
    private float anguloX = 20f; //pitch
    private float anguloY = 0f; //Yaw

    private enum ModoCamara { FPS, ORBITAL}
    private ModoCamara modoCamara = ModoCamara.ORBITAL;
    private float mouseSensitivity = 2f;
    private float velocidadMovimiento = 5f;

    //Todos los objetos
    private GameObject paredes;
    private GameObject piso;
    private GameObject techo;
    private GameObject toilet;

    // Start is called before the first frame update
    void Start()
    {
        generarEstructura();

        GenerateBathroom();

        CreateCamera();
        RecalcularMatrices();
    }

    // Update is called once per frame
    void Update()
    {
        // Cambiar de camara FPS <-> ORBITAL
        if (Input.GetKeyDown(KeyCode.V))
        {
            modoCamara = (modoCamara == ModoCamara.FPS) ? ModoCamara.ORBITAL : ModoCamara.FPS;

            if (modoCamara == ModoCamara.FPS)
            {
                // Posicionar la cámara al centro al cambiar a FPS
                miCamara.transform.position = t.position + new Vector3(0, 1.5f, 0);
                miCamara.transform.rotation = Quaternion.Euler(anguloX, anguloY, 0);
            }
        }

        if (modoCamara == ModoCamara.FPS)
        {
            ControlPrimeraPersona();
        }
        else
        {
            ControlOrbital();
            RecalcularMatrices(); // Solo en modo orbital
        }

    }

    private void generarEstructura()
    {
        lector.read("piso");
        lector.setColor(210f/255f, 105f / 255f, 30f/255f);
        piso = lector.getGameObject();
        t = piso.transform;
        
        lector.read("techo");
        techo = lector.getGameObject();
        techo.transform.position = new Vector3(0, 2.5f, 0);

        lector.read("pared_fix_v2");
        paredes = lector.getGameObject();
        paredes.transform.position = new Vector3(0, 1.25f,0);
    }

    private void GenerateBathroom()
    {
        lector.read("toilet1");
        lector.setColor(1,1,1);
        toilet = lector.getGameObject();
        toilet.transform.position = new Vector3(-4.5f, 0.6f, -2.5f);
        toilet.transform.rotation = Quaternion.Euler(0,-90,0);
        toilet.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }


     private void CreateCamera()
    {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;

        miCamara.transform.position = t.position + new Vector3(0, 2f, -distancia);
        miCamara.transform.LookAt(t.position);
    }

    private void RecalcularMatrices()
    {
        if (modoCamara == ModoCamara.FPS) return;

        float RadX = Mathf.Deg2Rad*anguloX;
        float RadY = Mathf.Deg2Rad*anguloY;

        Vector3 offset = new Vector3(
            distancia * MathF.Cos(RadX) * Mathf.Sin(RadY),
            distancia * Mathf.Sin(RadX),
            distancia * Mathf.Cos(RadX) * Mathf.Cos(RadY)
            );

        Vector3 pos = t.position + offset; 
        Vector3 up = Vector3.up;
       
        Matrix4x4 viewMatrix = CreateViewMatrix(pos, t.position, up);
        piso.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);
        techo.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);

        float fov = 90;
        float aspectRatio = 16 / (float)9;
        float nearClipPlane = 0.1f;
        float farClipPlane = 1000;

        Matrix4x4 projectionMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, nearClipPlane, farClipPlane);
        piso.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projectionMatrix, true));
        techo.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projectionMatrix, true));

        miCamara.transform.position = pos;
        miCamara.transform.LookAt(t.position);
    }

    private Matrix4x4 CreateViewMatrix(Vector3 pos, Vector3 targ, Vector3 up)
    {
        Vector3 forward = (targ - pos).normalized;
        Vector3 right = Vector3.Cross(forward,up).normalized;
        Matrix4x4 finalMatrix =  new Matrix4x4(
            new Vector4(right.x, right.y, right.z, Vector3.Dot(-right,pos)),
            new Vector4(up.x, up.y, up.z, Vector3.Dot(-up, pos)),
            new Vector4(-forward.x, -forward.y, -forward.z, Vector3.Dot(forward, pos)),
            new Vector4(0f, 0f, 0f, 1f)
        );
        finalMatrix = finalMatrix.transpose;

        return (finalMatrix);
    }

    private Matrix4x4 CalculatePerspectiveProjectionMatrix(float fov, float asp, float near, float far)
    {
        Matrix4x4 finalMatrix = new Matrix4x4(
            new Vector4(1/ asp * Mathf.Tan(fov/2), 0, 0, 0),
            new Vector4(0,1/Mathf.Tan(fov/2),0,0),
            new Vector4(0,0,(far+near)/(near-far), 2*far*near/(near-far)),
            new Vector4(0f, 0f, -1f, 0f)
        );
        finalMatrix = finalMatrix.transpose;

        return (finalMatrix);
    }

    private Matrix4x4 CreateModelMatrix(Vector3 newPosition, Vector3 newRotation, Vector3 newScale)
    {
        Matrix4x4 positionMatrix = new Matrix4x4(
            new Vector4(1f, 0f, 0f, newPosition.x),
            new Vector4(0f, 1f, 0f, newPosition.y),
            new Vector4(0f, 0f, 1f, newPosition.z),
            new Vector4(0f, 0f, 0f, 1f)
        );
        positionMatrix = positionMatrix.transpose;

        Matrix4x4 rotationMatrixX = new Matrix4x4(
            new Vector4(1f, 0f, 0f, 0f),
            new Vector4(0f, Mathf.Cos(newRotation.x), -Mathf.Sin(newRotation.x), 0f),
            new Vector4(0f, Mathf.Sin(newRotation.x), Mathf.Cos(newRotation.x), 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

         Matrix4x4 rotationMatrixY = new Matrix4x4(
            new Vector4(MathF.Cos(newRotation.y), 0f, Mathf.Sin(newRotation.y), 0f),
            new Vector4(0f, 1f, 0f, 0f),
            new Vector4(-Mathf.Sin(newRotation.y),0f, Mathf.Cos(newRotation.y), 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

           Matrix4x4 rotationMatrixZ = new Matrix4x4(
            new Vector4(MathF.Cos(newRotation.z), -Mathf.Sin(newRotation.z), 0f, 0f),
            new Vector4( Mathf.Sin(newRotation.z), Mathf.Cos(newRotation.z) , 0f, 0f),
            new Vector4(0f, 0f, 1f, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

        Matrix4x4 rotationMatrix = rotationMatrixZ * rotationMatrixY * rotationMatrixX;
        rotationMatrix = rotationMatrix.transpose;

        Matrix4x4 scaleMatrix = new Matrix4x4(
            new Vector4(newScale.x, 0f, 0f, 0f),
            new Vector4(0f, newScale.y, 0f, 0f),
            new Vector4(0f, 0f, newScale.z, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );
        scaleMatrix = scaleMatrix.transpose;

        Matrix4x4 finalMatrix = positionMatrix;
        finalMatrix *= rotationMatrix;
        finalMatrix *= scaleMatrix;

        return (finalMatrix);
    }

    private void ControlPrimeraPersona()
    {
        // Rotación con mouse (sin necesidad de botón derecho)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        anguloY += mouseX;
        anguloX -= mouseY;
        anguloX = Mathf.Clamp(anguloX, -60f, 60f);

        // Aplicar rotación a la cámara (FPS style)
        miCamara.transform.rotation = Quaternion.Euler(anguloX, anguloY, 0);

        // Movimiento con teclado (WASD)
        Vector3 direccion = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direccion += miCamara.transform.forward;
        if (Input.GetKey(KeyCode.S)) direccion -= miCamara.transform.forward;
        if (Input.GetKey(KeyCode.A)) direccion -= miCamara.transform.right;
        if (Input.GetKey(KeyCode.D)) direccion += miCamara.transform.right;

        direccion.y = 0; // Movimiento plano horizontal
        direccion.Normalize();

        miCamara.transform.position += direccion * velocidadMovimiento * Time.deltaTime;

    }

private void ControlOrbital()
    {
        // Movimiento con flechas del teclado
        if (Input.GetKey(KeyCode.UpArrow)) anguloX += velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) anguloX -= velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) anguloY -= velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) anguloY += velocidadRotacion * Time.deltaTime;

        // Rotacion mouse y click derecho
        if (Input.GetMouseButton(1)) // Botón derecho del mouse
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            anguloY += mouseX;
            anguloX -= mouseY;
        }

        // Limitar pitch (evita que se dé vuelta completamente)
        anguloX = Mathf.Clamp(anguloX, -60f, 60f);
    }

}
