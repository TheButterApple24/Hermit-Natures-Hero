using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    float speed = 0.3f;
    float xRot = 0.0f;
    float yRot = 0.0f;
    // Update is called once per frame
    void Update()
    {
        xRot += Input.GetAxis("Mouse X") * 45 * Time.deltaTime;
        yRot += Input.GetAxis("Mouse Y") * 45 * Time.deltaTime;

        yRot = Mathf.Clamp(yRot, -90.0f, 90.0f);

        transform.localRotation = Quaternion.AngleAxis(xRot, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(yRot, Vector3.left);


        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += transform.forward * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position -= transform.forward * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position -= transform.right * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += transform.right * speed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            gameObject.transform.position -= transform.up * speed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            gameObject.transform.position += transform.up * speed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            speed += 0.005f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            speed -= 0.005f;
        }

        speed = Mathf.Clamp(speed, 0.0f, 3.0f);

        
    }
}
