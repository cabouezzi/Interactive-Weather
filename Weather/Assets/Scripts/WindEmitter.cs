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
        public float age;

        public Particle (float longitude, float latitude) {
            this.longitude = longitude;
            this.latitude = latitude;
            this.age = Random.Range(1f, 3f);
        }
    }

    // Start is called before the first frame update
    void Start() {

        Data.LoadFiles();

        var main = system.main;
        main.maxParticles = 10000000;

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

            // particles2d[i] = particle;
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

    Vector3 Velocity (float lon, float lat) {

        Vector2 wind = Data.GetWind(lon, lat);

        Vector3 position = GetPosition3D(lon, lat);
        Vector3 newPos = GetPosition3D(lon + wind.x, lat + wind.y);

        return (newPos - position) * 0.001f;

    }

    void ClampToRadius (ref ParticleSystem.Particle particle) {
        Vector3 clamped = particle.position.normalized * radius;
        particle.position = clamped;
    }

    void PlotParticle (WindEmitter.Particle particle) {

        ParticleSystem.EmitParams emission = new ParticleSystem.EmitParams();

        Vector3 position3 = GetPosition3D(particle.longitude, particle.latitude);

        if (plotSphere) {
            emission.position = position3;
            // emission.velocity = Velocity(particle.longitude, particle.latitude);
        }
        else {
            emission.position = new Vector3(particle.longitude/18.0f - 10, particle.latitude/18.0f - 5, 0);
        }

        system.Emit(emission, 1);

    }

    void UpdateParticle (ref WindEmitter.Particle particle) {

        Vector2 wind = Data.GetWind(particle.longitude, particle.latitude) / 10f;

        particle.longitude += wind.x;
        particle.latitude += wind.y;

        if (particle.longitude < 0) {
            particle.longitude = 360 + particle.longitude % -360;
        } else if (particle.longitude >= 360) {
            particle.longitude %= 360;
        }

        if (particle.latitude < 0) {
            particle.latitude = 180 + particle.latitude % -180;
        } else if (particle.latitude >= 180) {
            particle.latitude %= 180;
        }

        particle.age -= 0.01f;

        if (particle.age < 0) {
            particle.longitude = Random.Range(0f, 359f);
            particle.latitude = Random.Range(0f, 179);
            particle.age = Random.Range(1f, 3f);
        }

    }

    Vector3 GetPosition3D (float longitude, float latitude) {

        float lonRadian = (longitude - 180) * Mathf.PI / 180.0f;
        float latRadian = (latitude - 180)  * Mathf.PI / 180.0f;

        // float x = Mathf.Sin(lonRadian) * Mathf.Cos(latRadian);
        // float y = Mathf.Sin(lonRadian) * Mathf.Sin(latRadian);
        // float z = Mathf.Cos(lonRadian);

        float dx = Mathf.Sin(latRadian) * Mathf.Cos(lonRadian);
        float dy = Mathf.Sin(latRadian) * Mathf.Sin(lonRadian);
        float dz = Mathf.Cos(latRadian);


        return new Vector3(dx * radius, dy * radius, dz * radius);

    }

}
