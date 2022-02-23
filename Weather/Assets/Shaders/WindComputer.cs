using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Particle {
    public Vector3 position;
    public Vector3 velocity;
}

public class WindComputer : MonoBehaviour {

    int kernelID;
    public ComputeShader computeShader;
    public ComputeBuffer computeBuffer;

    public const int numParticles = 10;
    public Particle[] particles;
    public GameObject[] sprites;

    public Material material;

    private const int WARP_SIZE = 8;
    private int mWarpCount;


    // Start is called before the first frame update
    void Start() {

        CreateParticles();
        CreateComputeBuffer();

    }

    // Update is called once per frame
    void Update() {

        computeShader.SetFloat("deltaTime", Time.time);
        computeShader.Dispatch(kernelID, mWarpCount, 1, 1);


        for (int i = 0; i < numParticles; i++) {
            sprites[i].transform.position = particles[i].position;
        }

    }

    void CreateParticles () {

        mWarpCount = Mathf.CeilToInt(numParticles / WARP_SIZE);

        particles = new Particle[numParticles];
        sprites = new GameObject[numParticles];

        for (int i = 0; i < numParticles; i++) {
            particles[i].position.x = (float) Math.Cos(i);
            particles[i].position.y = (float) Math.Sin(i);
            particles[i].position.z = (float) Math.Sin(i);

            particles[i].velocity.x = 0;
            particles[i].velocity.y = 0;
            particles[i].velocity.z = 0;

            sprites[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sprites[i].transform.position = particles[i].position;
            sprites[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

    }

    void CreateComputeBuffer () {
        
        computeBuffer = new ComputeBuffer(numParticles, 24);
        computeBuffer.SetData(particles);

        kernelID = computeShader.FindKernel("CSParticle");

        computeShader.SetBuffer(kernelID, "particleBuffer", computeBuffer);
        material.SetBuffer("particuleBuffer", computeBuffer);

    }

    void OnDestroy() {
        if (computeBuffer != null) {
            computeBuffer.Release();
        }
    }
    
}
