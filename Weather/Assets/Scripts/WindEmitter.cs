using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEmitter : MonoBehaviour {

    public ParticleSystem system;
    WindEmitter.Particle[] particles2d;

    public float radius;
    public bool plotSphere;

    struct Particle {
        public float longitude;
        public float latitude;

        public Particle (float longitude, float latitude) {
            this.longitude = longitude;
            this.latitude = latitude;
        }
    }

    // Start is called before the first frame update
    void Start() {

        Data.LoadFiles();

        var main = system.main;
        main.maxParticles = 94 * 192;

        particles2d = new WindEmitter.Particle[94 * 192];

        InitialParticles();
        
    }

    void InitialParticles () {

        int numParticles = 192*94;

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float deltaAngle = 2 * Mathf.PI * goldenRatio;

        for (int i = 0; i < numParticles; i++) {

            float t = (float) i / numParticles;
            float incline = Mathf.Acos (1 - 2 * t);
            float rot = deltaAngle * i;

            Vector2 position = new Vector2(rot * 180 / Mathf.PI, incline * 180 / Mathf.PI);
            WindEmitter.Particle particle = new WindEmitter.Particle(position.x, position.y);

            particles2d[i] = particle;
            PlotParticle(particle);
            UpdateParticle(ref particles2d[i]);

        }

    }

    // Update is called once per frame
    void Update() {

        system.Clear();
        
        for (int p = 0; p < particles2d.Length; p++) {

            UpdateParticle(ref particles2d[p]);
            PlotParticle(particles2d[p]);

        }

    }

    void PlotParticle (WindEmitter.Particle particle) {

        ParticleSystem.EmitParams emission = new ParticleSystem.EmitParams();

        Vector3 position3 = GetPosition3D(particle.longitude, particle.latitude);

        if (plotSphere) {
            emission.position = position3;
        }
        else {
            emission.position = new Vector3(particle.longitude/18.0f - 10, particle.latitude/18.0f - 5, 0);
        }

        system.Emit(emission, 1);

    }

    void UpdateParticle (ref WindEmitter.Particle particle2d) {

        Vector2 wind = Data.GetWind(particle2d.longitude, particle2d.latitude) / 50;

        particle2d.longitude += wind.x;
        particle2d.latitude += wind.y;

        if (particle2d.longitude < 0) {
            particle2d.longitude = 360 + particle2d.longitude % -360;
        } else if (particle2d.longitude > 360) {
            particle2d.longitude %= 360;
        }

        if (particle2d.latitude < 0) {
            particle2d.latitude = 180 + particle2d.latitude % -180;
        } else if (particle2d.latitude > 180) {
            particle2d.latitude %= 180;
        }

    }

    Vector3 GetPosition3D (float longitude, float latitude) {

        float lonRadian = longitude * Mathf.PI / 180.0f;
        float latRadian = latitude  * Mathf.PI / 180.0f;

        // float x = Mathf.Sin(lonRadian) * Mathf.Cos(latRadian);
        // float y = Mathf.Sin(lonRadian) * Mathf.Sin(latRadian);
        // float z = Mathf.Cos(lonRadian);

        float dx = Mathf.Sin(latRadian) * Mathf.Cos(lonRadian);
        float dy = Mathf.Sin(latRadian) * Mathf.Sin(lonRadian);
        float dz = Mathf.Cos(latRadian);


        return new Vector3(dx * radius, dy * radius, dz * radius);

    }

}
