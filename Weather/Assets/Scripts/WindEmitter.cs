using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEmitter : MonoBehaviour {

    public ParticleSystem windSystem;
    public ParticleSystem kiteSystem;

    ParticleSystem.Particle[] windParticles;
    ParticleSystem.Particle[] kiteParticles;
    
    public const int numberOfParticles = 10000;

    public float radius;

    public float minLife;
    public float maxLife;
    public float velocityFactor;

    public float maxKiteDrag = 10;
    Vector3 touchPosition;

    // Start is called before the first frame update
    void Start() {

        Data.LoadFiles();

        var wmain = windSystem.main;
        wmain.maxParticles = numberOfParticles;
        wmain.loop = true;

        var kmain = kiteSystem.main;
        kmain.maxParticles = 50;
        kmain.loop = true;

        windParticles = new ParticleSystem.Particle[numberOfParticles];
        kiteParticles = new ParticleSystem.Particle[kiteSystem.main.maxParticles];

        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = radius;
        
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

            PlotParticle(position.x, position.y, ref windSystem);

        }

    }

    // Update is called once per frame
    void Update() {

        TouchHandler();
        UpdateWindParticles();
        UpdateKites();

    }

    void TouchHandler () {

        if (Input.GetMouseButtonDown(0)) {
            touchPosition = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {

            Vector3 drag = touchPosition - Input.mousePosition;

            if (drag.magnitude > maxKiteDrag) return;

            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            
            if (Physics.Raycast(raycast, out raycastHit)) {

                if (raycastHit.collider.CompareTag("Wind")) {

                    Vector3 hit = raycastHit.point;
                    hit.x *= -1;

                    Vector2 geo = GetPosition2D(hit);
                    PlotParticle(geo.x, geo.y, ref kiteSystem);
                    
                }

            }
        }

    }

    Vector3 GetVelocity (Vector3 position) {

        Vector2 geo = GetPosition2D(position);
        Vector2 wind = Data.GetWind(geo.x, geo.y);

        Vector3 position3 = GetPosition3D(geo.x, geo.y);

        float newX = (geo.x + wind.x * velocityFactor) % 360f;
        float newY = (geo.y  + wind.y * velocityFactor);

        if (newY <= 0) {
            newX += 180;
            newY = Mathf.Abs(newY);
        }
        else if (newY >= 180) {
            newX += 180;
            newY = 360 - newY;
        }

        Vector3 newPos3 = GetPosition3D(newX % 360, newY % 180);

        return (newPos3 - position3);

    }

    void PlotParticle (float longitude, float latitude, ref ParticleSystem system) {

        ParticleSystem.EmitParams emission = new ParticleSystem.EmitParams();

        Vector3 position = GetPosition3D(longitude, latitude);
        Vector3 velocity = GetVelocity(position);
        Color32 color = GetColor(longitude, latitude);

        emission.position = position;
        emission.velocity = velocity;
        if (system != kiteSystem) {
            emission.startLifetime = Random.Range(minLife, maxLife);
            emission.startColor = color;
        }

        system.Emit(emission, 1);

    }

    void UpdateKites () {

        int numAlive = kiteSystem.GetParticles(kiteParticles);

        for (int i = 0; i < numAlive; i++) {
            UpdateParticle(ref kiteParticles[i]);
        }

        kiteSystem.SetParticles(kiteParticles, numAlive);

    }

    void UpdateWindParticles () {

        int numAlive = windSystem.GetParticles(windParticles);

        for (int i = 0; i < numAlive; i++) {
            if (windParticles[i].velocity.magnitude < 0.01) {
                windParticles[i].remainingLifetime = 0;
            }
            UpdateParticle(ref windParticles[i]);
        }

        windSystem.SetParticles(windParticles, numAlive);

        // Respawns particles
        for (int i = 0; i < numberOfParticles - numAlive; i++) {
            PlotParticle(Random.Range(0, 359f), Random.Range(0, 179f), ref windSystem);
        }

    }

    void UpdateParticle (ref ParticleSystem.Particle particle) {

        ClampToRadius(ref particle);
        particle.velocity = GetVelocity(particle.position);

    }

    void ClampToRadius (ref ParticleSystem.Particle particle) {
        Vector3 clamped = particle.position.normalized * radius;
        particle.position = clamped;
    }

    Vector3 GetPosition3D (float longitude, float latitude) {

        float lonRadian = ((longitude + 360) % 360 - 180) * Mathf.PI / 180f;
        float latRadian = ((latitude + 180) % 180 - 90) * Mathf.PI / 180f;

        float x = radius * Mathf.Cos(latRadian) * Mathf.Sin(lonRadian);
        float y = radius * Mathf.Sin(latRadian);
        float z = radius * Mathf.Cos(latRadian) * Mathf.Cos(lonRadian);

        return new Vector3(x, y, z);

    }

    Vector2 GetPosition2D (Vector3 position) {

        float x = position.normalized.x;
        float y = position.normalized.y;
        float z = position.normalized.z;

        // arcsin -90 to 90
        float latitude = Mathf.Asin(y) * 180f / Mathf.PI;

        // arctan ranges -180 to 180
        float longitude = Mathf.Atan2(x, z) * 180f / Mathf.PI;

        latitude += 90;
        longitude += 180;

        return new Vector2 (longitude % 360, latitude % 180);

    }

    Color32 GetColor(float longitude, float latitude) {
        float temp = Data.GetTemp(longitude, latitude);
        return Color.HSVToRGB(.66f*temp, 1, 1);
    }

}
