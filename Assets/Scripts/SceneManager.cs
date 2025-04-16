using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SceneManager : MonoBehaviour
{
    //arreglo para almacenar los vertices
    private Vector3[] vertices;
    //arreglo para almacenar los indices a los vertices
    private int[] triangles;
    //referecia al objetoTriangulo que vamos a crear
    private GameObject objetoTriangulo;

    //objeto camara
    private GameObject miCamara;

    private Color[] colores;

    // Start is called before the first frame update
    void Start()
    {
        objetoTriangulo = new GameObject();

        objetoTriangulo.AddComponent<MeshFilter>();
        objetoTriangulo.GetComponent<MeshFilter>().mesh = new Mesh();

        objetoTriangulo.AddComponent<MeshRenderer>();

        CreateModel();
        UpdateMesh();
        CreateMaterial();

        CreateCamera();

        Vector3 newPosition = new Vector3(0,0,0); // definimos una traslacion 
        Vector3 newRotation = new Vector3(0, 0, 0); // Rotamos 45 en el eje Y
        Vector3 newScale = new Vector3(1f,1f,1f); // Definimos un escalado

        //Calculamos la matrix de modelado
        Matrix4x4 modelMatrix = CreateModelMatrix1(newPosition, newRotation, newScale);

        //Le decimos al shader que utilice esta matriz de modelado
        objetoTriangulo.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateModel()
    {
        FileReader lector = new FileReader();

    }

    private void UpdateMesh()
    {    
        objetoTriangulo.GetComponent<MeshFilter>().mesh.vertices = vertices;
        objetoTriangulo.GetComponent<MeshFilter>().mesh.triangles = triangles;
        objetoTriangulo.GetComponent<MeshFilter>().mesh.colors = colores;
    }

    private void CreateCamera()
    {
        miCamara = new GameObject();
        miCamara.AddComponent<Camera>();
        miCamara.transform.position = new Vector3(0,0,-50);
        miCamara.transform.rotation = Quaternion.Euler(0,0,0);

        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;
    }

    private void CreateMaterial()
    {
        Material newMaterial = new Material(Shader.Find("ShaderBasico"));

        objetoTriangulo.GetComponent<MeshRenderer>().material = newMaterial;
    }

    private Matrix4x4 CreateModelMatrix1(Vector3 newPosition, Vector3 newRotation, Vector3 newScale)
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
}
