using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using static UnityEngine.Mathf;

public class Data {

    public static float[,] u;
    public static float[,] v;
    public static float[,] temp;

    public static void LoadFiles () {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file;
        
        file = new FileStream("Assets/Data/u.dat", FileMode.Open);
        u = (float[,]) formatter.Deserialize(file);
        file.Close();

        file = new FileStream("Assets/Data/v.dat", FileMode.Open);
        v = (float[,]) formatter.Deserialize(file);
        file.Close();

        file = new FileStream("Assets/Data/t.dat", FileMode.Open);
        temp = (float[,]) formatter.Deserialize(file);
        file.Close();
        // normalize temps
        for(int i = 0; i < 90; i++) {
            for(int j = 0; j < 180; j++) {
                float normalize = (temp[i, j] + 0.9539546567925773f)/2.3242494283278154f;
                float smoothstep = 3*Mathf.Pow(normalize, 2) - 2*Mathf.Pow(normalize, 3);
                temp[i, j] = smoothstep;
            }
        }
    }

    public static Vector2 GetWind(float longitude, float latitude) {

        int x = FloorToInt((longitude % 360) / 1.875f);
        int y = FloorToInt((latitude % 180) * 94.0f / 180.0f);

        x = Min(191, Max(0, x));
        y = Min(93,  Max(0, y));

        return new Vector2(u[y, x], v[y, x]);

        /*
        int i = Mathf.Min(Mathf.Max((int) (0.52503335f*latitude), 0), 93);
        int j = Mathf.Min(Mathf.Max((int) (0.5333333333f*longitude), 0), 191);

        int I = Mathf.Min(i + 1, 93);
        int J = Mathf.Min(j + 1, 191);

        float a = (0.52503335f*latitude) % 1f;
        float b = (0.5333333333f*longitude) % 1f;

        return Lerp(
            u[i, j], v[i, j], (1 - a)*(1 - b),
            u[I, j], v[I, j], a*(1 - b),
            u[i, J], v[i, J], (a - 1)*b,
            u[I, J], v[I, J], a*b
        );
        */
    }

    
    public static float GetTemp(float longitude, float latitude) {
        int x = FloorToInt((longitude % 360) / 2f);
        int y = FloorToInt((latitude % 180) / 2f);

        x = Min( 179 , Max(0, x));
        y = Min( 89 , Max(0, y));

        return temp[y, x];
    }

    static Vector2 Lerp(float u1, float v1, float w1,
                        float u2, float v2, float w2,
                        float u3, float v3, float w3,
                        float u4, float v4, float w4) {
        return new Vector2(
            u1*w1 + u2*w2 + u3*w3 + u4*w4,
            v1*w1 + v2*w2 + v3*w3 + v4*w4
        );
    }
}
