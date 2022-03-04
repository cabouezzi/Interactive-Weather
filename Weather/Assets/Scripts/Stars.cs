using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {

    public ParticleSystem system;
    
    public const int numberOfParticles = 1000;

    public float minRadius;
    public float maxRadius;

    // Start is called before the first frame update
    void Start() {

        var main = system.main;
        main.maxParticles = numberOfParticles;
        main.loop = true;
        
        PlotSphere(numberOfParticles);
        
    }

    void PlotSphere (float n) {

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float deltaAngle = 2 * Mathf.PI * goldenRatio;

        for (int i = 0; i < n; i++) {

            float t = (float) i / n;
            float incline = Mathf.Acos (1 - 2 * t);
            float rot = (deltaAngle * i) % (2 * Mathf.PI);

            Vector2 position = new Vector2(rot * 180 / Mathf.PI, incline * 180 / Mathf.PI);

            PlotParticle(position.x, position.y);

        }

    }

    void PlotParticle (float longitude, float latitude) {

        ParticleSystem.EmitParams emission = new ParticleSystem.EmitParams();

        Vector3 position = GetPosition3D(longitude, latitude);

        emission.position = position;
        emission.startLifetime = Mathf.Infinity;
        emission.startSize = 1;

        system.Emit(emission, 1);

    }

    Vector3 GetPosition3D (float longitude, float latitude) {

        float lonRadian = (longitude % 360 - 180) * Mathf.PI / 180f;
        float latRadian = (latitude % 180 - 90) * Mathf.PI / 180f;

        float radius = Random.Range(minRadius, maxRadius);

        float x = radius * Mathf.Cos(latRadian) * Mathf.Sin(lonRadian);
        float y = radius * Mathf.Sin(latRadian);
        float z = radius * Mathf.Cos(latRadian) * Mathf.Cos(lonRadian);

        return new Vector3(x, y, z);

    }

}
