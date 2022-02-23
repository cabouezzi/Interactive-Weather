using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour {

    static float[,] u;
    static float[,] v;
    static Vector2[] vectors;

    static void LoadFiles () {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = new FileStream("Assets/Data/u.dat", FileMode.Open);
        u = (float[,]) formatter.Deserialize(file);
        file.Close();

        file = new FileStream("Assets/Data/v.dat", FileMode.Open);
        v = (float[,]) formatter.Deserialize(file);
        file.Close();

        int height = u.GetLength(0);
        int width = u.GetLength(1);
        int length = width * height;
        vectors = new Vector2[length];

        Debug.Log(width);
        Debug.Log(height);
        
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int index = (width * y) + x;
                // Debug.Log(index);
                vectors[index] = new Vector2(u[y, x], v[y, x]);
            }
        }

    }

    public ComputeShader computeShader;
    public ComputeBuffer computeBuffer;
    public RenderTexture renderTexture;
    // Start is called before the first frame update
    void Start() {

        LoadFiles();

        renderTexture = new RenderTexture(720, 360, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        int kernelID = computeShader.FindKernel("CSMain");

        computeShader.SetTexture(kernelID, "Result", renderTexture);

    }

    // Update is called once per frame
    void Update() {



    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {

        if (renderTexture == null) {
            renderTexture = new RenderTexture(720, 360, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("Resolution", renderTexture.width);

        int kernelID = computeShader.FindKernel("CSMain");
        
        computeBuffer = new ComputeBuffer(ComputeShaderTest.u.Length, sizeof(float) * 2);
        computeBuffer.SetData(ComputeShaderTest.vectors);

        computeShader.SetBuffer(kernelID, "vectors", computeBuffer);
        computeShader.Dispatch(kernelID, renderTexture.width / 8, renderTexture.height / 8, 1);

        Graphics.Blit(renderTexture, dest);

    }

    void OnDestroy() {
        if (computeBuffer != null) {
            computeBuffer.Release();
        }
    }

}
