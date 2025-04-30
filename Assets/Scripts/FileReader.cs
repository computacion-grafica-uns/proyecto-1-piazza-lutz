using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class FileReader
{
    private GameObject obj;
    private Color[] colores;
    private Vector3[] vertices;
    private int[] triangles;
    private int cantVert = 0;
    private int cantTrig = 0;
    private float minX, maxX, minY, maxY, minZ, maxZ = 0f;

    public void read(String fileName){
        String path = "Assets/Modelos3D/" + fileName + ".obj";

        StreamReader reader = new StreamReader(path);
        string fileData = (reader.ReadToEnd());

        ReadEachLine(fileData);

        reader.Close();

        obj = new GameObject(fileName);
        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.AddComponent<MeshRenderer>();

        UpdateMesh();
        CreateMaterial();
    }

    public GameObject getGameObject()
    {
        return obj;
    }

    private void ReadEachLine(string fileData)
    {
        bool barrera = true; // Tomo las primeras coordenadas para comparar y centrar los objetos en el (0,0,0)
        int posVert = 0;
        int posTrig = 0;
        float reposx, reposy, reposz;

        string[] lines = fileData.Split('\n');

        //Mucho más fácil contar la cantidad de vertices y caras, luego inicializar los arreglos con esos datos :)
        for(int i = 0; i < lines.Length; i++)
        {
            if(lines[i].StartsWith("v ")){
                cantVert++;
            }
            if(lines[i].StartsWith("f ")){
                cantTrig++;
            }
        }

        //Inicializo el color de los vertices
        colores = new Color[cantVert];
        vertices = new Vector3[cantVert];
        triangles = new int[cantTrig*6]; 



        for(int i = 0; i < lines.Length; i++)
        {
            if(lines[i].StartsWith("v "))
            {
                string[] coord = lines[i].Split(' ');

                float x = float.Parse(coord[1], CultureInfo.InvariantCulture);
                float y = float.Parse(coord[2], CultureInfo.InvariantCulture);
                float z = float.Parse(coord[3], CultureInfo.InvariantCulture);
                 

                if(barrera){
                    barrera = false; //Entra la primera vez

                    minX = maxX = x;
                    minY = maxY = y;
                    minZ = maxZ = z;
                }
                else{
                    if(x < minX) { minX = x; }
                    if(y < minY) { minY = y; }
                    if(z < minZ) { minZ = z; }
                    if(x > maxX) { maxX = x; }
                    if(y > maxY) { maxY = y; }
                    if(z > maxZ) { maxZ = z; }
                }
               
                vertices[posVert] = new Vector3(x,y,z);
                posVert++;
                
            }else{
                if(lines[i].StartsWith("f "))
                {
                    string[] cara = lines[i].Split(' '); //Separo los vertices

                    if(cara.Length == 5) // es un vector de 4 vertices
                    {
                        int[] quadIndices = new int[4];
                        for(int j = 0; j < 4; j++)
                        {
                            string[] partes = cara[j+1].Split('/');
                            quadIndices[j] = int.Parse(partes[0])-1;
                        }

                        triangles[posTrig] = quadIndices[0]; posTrig++;
                        triangles[posTrig] = quadIndices[1]; posTrig++;
                        triangles[posTrig] = quadIndices[2]; posTrig++;
                        
                        triangles[posTrig] = quadIndices[0]; posTrig++;
                        triangles[posTrig] = quadIndices[2]; posTrig++;
                        triangles[posTrig] = quadIndices[3]; posTrig++;
                        
                    }
                    else
                    {
                        int[] quadIndices = new int[3];
                        for(int j = 0; j < 3; j++)
                        {
                            string[] partes = cara[j+1].Split('/');
                            quadIndices[j] = int.Parse(partes[0])-1;
                        }

                        triangles[posTrig++] = quadIndices[0];
                        triangles[posTrig++] = quadIndices[1];
                        triangles[posTrig++] = quadIndices[2];
                    }

                }
            }
        }

        reposx = (minX + maxX)/2;
        reposy = (minY + maxY)/2;
        reposz = (minZ + maxZ)/2;

        for(int i = 0; i < vertices.Length; i++){
            vertices[i].x = vertices[i].x - reposx;
            //vertices[i].y = vertices[i].y - reposy;
            vertices[i].z = vertices[i].z - reposz;

            colores[i] = new Color(0.5f, 0.5f, 0.5f);
        }
        
    }

    private void UpdateMesh()
    {    
        obj.GetComponent<MeshFilter>().mesh.vertices = vertices;
        obj.GetComponent<MeshFilter>().mesh.triangles = triangles;
        obj.GetComponent<MeshFilter>().mesh.colors = colores;
        obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    private void CreateMaterial()
    {
        Material material = new Material(Shader.Find("ShaderBasico"));
        obj.GetComponent<MeshRenderer>().material = material;
    }

    public void setColor(float r, float g, float b)
    {
        colores = new Color[cantVert];
        for(int i = 0; i < cantVert; i++)
        {
            colores[i] = new Color(r,g,b,1);
        }
        obj.GetComponent<MeshFilter>().mesh.colors = colores;
    }
}
