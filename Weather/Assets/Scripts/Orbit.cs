 using UnityEngine;
 using System.Collections;
 
 [AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
 public class Orbit : MonoBehaviour
 {
     public Transform target;
     public float distance = 10.0f;
     public float xSpeed = 5.0f;
     public float ySpeed = 30.0f;
     public float zoomSpeed = 0.05f;
     public float yMinLimit = -80f;
     public float yMaxLimit = 80f;
     public float distanceMin = 6f;
     public float distanceMax = 18f;
     public float smoothTime = 4f;
     float rotationYAxis = 0.0f;
     float rotationXAxis = 0.0f;
     float velocityX = 0.0f;
     float velocityY = 0.0f;

     float timeInactive = 0;
     
     // Use this for initialization
     void Start()
     {

        Cursor.visible = true;
        mousePosition = Input.mousePosition;
        
         Vector3 angles = transform.eulerAngles;
         rotationYAxis = angles.y;
         rotationXAxis = angles.x;
         // Make the rigid body not change rotation
         if (GetComponent<Rigidbody>())
         {
             GetComponent<Rigidbody>().freezeRotation = true;
         }
     }

     Vector3 mousePosition;

     void Update()
     {

         if (target)
         {

             AutoRotate();
             timeInactive += Time.deltaTime;
             if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
             {
                 Vector3 dm = Input.mousePosition - mousePosition;

                 timeInactive = 0;
                 velocityX += xSpeed * dm.x * distance * 0.02f;
                 velocityY += ySpeed * dm.y * 0.02f;
             }
             if (Input.GetMouseButtonUp(0)) {

             }

             rotationYAxis += velocityX;
             rotationXAxis -= velocityY;
             rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
             Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
             Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
             Quaternion rotation = toRotation;
             
             distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, distanceMin, distanceMax);
             RaycastHit hit;
             if (Physics.Linecast(target.position, transform.position, out hit))
             {
                 distance -= hit.distance;
             }
             Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
             Vector3 position = rotation * negDistance + target.position;
             
             transform.rotation = rotation;
             transform.position = position;
             velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
             velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
         }

        mousePosition = Input.mousePosition;

     }
     public static float ClampAngle(float angle, float min, float max)
     {
         if (angle < -360F)
             angle += 360F;
         if (angle > 360F)
             angle -= 360F;
         return Mathf.Clamp(angle, min, max);
     }
     
    
    void AutoRotate () {

        if (timeInactive > 10) {
            velocityX = 0.05f;
        }

    }

 }