using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;



public class FileReader : MonoBehaviour
{
   // Start is called before the first frame update
    void Start()
    {
        string fileName = "objeto";
        string path = "Assets/Modelos3D/" + fileName + ".obj";

        StreamReader reader = new StreamReader(path);
        string fileData = (reader.ReadToEnd());

        reader.Close();
        
        Debug.Log(fileData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadEachLine(string fileData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int[]> faces = new List<int[]>();

        string[] lines = fileData.Split('\n');
        for(int i = 0; i < lines.Length; i++)
        {
            if(lines[i].StartsWith("v ")){
                String[] coords = lines[i].Split(' ');
                if(coords.Length >= 4){
                    float x = float.Parse(coords[1], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[2], CultureInfo.InvariantCulture);
                    float z = float.Parse(coords[3], CultureInfo.InvariantCulture);
                    Vector3 vertex = new Vector3(x,y,z);
                    vertices.Add(vertex);
                }

            }
            else if(lines[i].StartsWith("f ")){
                String[] face = lines[i].Split(' ');
                if(face.Length >= 4){
                    int[] faceIndices = new int[face.Length - 1];
                    for (int j = 1; j < face.Length; j++)
                    {
                        string part = face[j];
                        // Si la cara tiene formato con barras (v/vt/vn), quedate con el primer valor
                        string[] vertexParts = part.Split(' ');
                        int vertexIndex = int.Parse(vertexParts[0]);

                        // Los índices en .obj empiezan en 1, así que restamos 1
                        faceIndices[j - 1] = vertexIndex - 1;
                    }

                    faces.Add(faceIndices);
                }
            }
        }
    }
}
