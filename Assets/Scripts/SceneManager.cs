using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SceneManager : MonoBehaviour
{
    //referecia al objetoTriangulo que vamos a crear
    private GameObject objeto;
    //objeto camara
    private GameObject miCamara;

    private Transform t;
    private float distancia = 10f;
    private float velocidadRotacion = 50f;

    private float anguloX = 20f; //pitch
    private float anguloY = 0f; //Yaw
    
    public FileReader lector = new FileReader();


    // Start is called before the first frame update
    void Start()
    {
        lector.read("cubo"); 
        objeto = lector.getGameObject();  
        t = objeto.transform;   

        if (objeto == null)
        Debug.LogError("El objeto no se generó correctamente");
        else
        Debug.Log("Objeto generado: " + objeto.name);  

        
        CreateCamera();
        RecalcularMatrices();
    }

    // Update is called once per frame
    void Update()
    {
          // Movimiento con flechas del teclado
        if (Input.GetKey(KeyCode.UpArrow)) anguloX -= velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) anguloX += velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) anguloY -= velocidadRotacion * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) anguloY += velocidadRotacion * Time.deltaTime;

        // Limitar pitch (evita que se dé vuelta completamente)
        anguloX = Mathf.Clamp(anguloX, -60f, 60f);

        RecalcularMatrices();
    }

     private void CreateCamera()
    {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;

    }

    private void RecalcularMatrices()
    {
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
        objeto.GetComponent<Renderer>().material.SetMatrix("_ViewMatrix", viewMatrix);

        float fov = 90;
        float aspectRatio = 16 / (float)9;
        float nearClipPlane = 0.1f;
        float farClipPlane = 1000;

        Matrix4x4 projectionMatrix = CalculatePerspectiveProjectionMatrix(fov, aspectRatio, nearClipPlane, farClipPlane);
        objeto.GetComponent<Renderer>().material.SetMatrix("_ProjectionMatrix", GL.GetGPUProjectionMatrix(projectionMatrix, true));

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

}
