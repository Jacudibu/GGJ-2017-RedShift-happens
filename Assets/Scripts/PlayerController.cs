using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 Velocity
    {
        get
        {
            return velocity;
        }
    }

    [SerializeField]
    public float lastDirectionChangeTime = 0f;

    float speedChangeTime = 0f;

    public int shifts;
    public float floatshifts;

    float acceleration = 0.05f;
    float curVelocity = 0f;
    float maxVelocity = 0.1f;
    float floatshifttime = 0f;

    [SerializeField]
    private float speed;

    //private Vector3 lastChangePosition;

    private const float velocityDeath = 0.01f;
    private float yRotation;

    private Vector3 velocity;

    public delegate void ShiftAction();
    public ShiftAction OnShiftHappens;
    public ShiftAction OnNoShiftHappens;

    private void Start()
    {
        //lastChangePosition = transform.position;
        speedChangeTime = Time.time;
        floatshifttime = Time.time;
    }

    void Update()
    {
        //transform.position = transform.position + Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        // change direction
        bool changedirection = false;
        if ((transform.position.x + curVelocity) >= 9.9) {
            acceleration = -1*Mathf.Abs(acceleration);
            changedirection = true;
        } else if ((transform.position.x + curVelocity) <= -9.9) {
            acceleration = Mathf.Abs(acceleration);
            changedirection = true;
        } else if(Input.GetButtonDown("Jump") && (Time.time > lastDirectionChangeTime + 0.1f)){
            lastDirectionChangeTime = Time.time;
            acceleration *= -1;
            changedirection = true;
            //lastChangePosition = transform.position;
        }

        if ((Mathf.Abs(curVelocity+acceleration) < maxVelocity) && (Time.time > speedChangeTime + 0.1f)){
            curVelocity += acceleration;
            speedChangeTime = Time.time;
        }
        transform.position = transform.position + Vector3.right * curVelocity;
        
        // set floatshifts
        if (changedirection){
            floatshifts = Mathf.Min(floatshifts+2f,10f);//0.4f*Mathf.Abs(lastChangePosition.x);
        } else if (Time.time > floatshifttime + 0.2f){
            floatshifts = Mathf.Max(floatshifts*0.8f,0.001f);
            floatshifttime = Time.time;
        }

        // IMMA DRAWIN MAH LAZOR!!!
        if (OnNoShiftHappens != null)
            OnNoShiftHappens.Invoke();

        // RotateInInputDirection();
    }

    private void RotateInInputDirection()
    {
        if (Input.GetAxis("Horizontal") == 0f)
        {
            yRotation = yRotation * Time.deltaTime;
        }
        else
        {
            yRotation += -Input.GetAxis("Horizontal") * Time.deltaTime * 90f;
            yRotation = Mathf.Clamp(yRotation, -45, 45);
        }

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
