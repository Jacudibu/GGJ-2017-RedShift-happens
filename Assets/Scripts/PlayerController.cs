using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public int points = 0;
    public int plHealth = 4;
    public int curHealth = 4;
    public int maxHealth = 4;

    float acceleration = 0.05f;
    float curVelocity = 0f;
    float maxVelocity = 0.17f;

    [SerializeField]
    private float speed;

    private Vector3 lastChangePosition;

    private const float velocityDeath = 0.01f;
    private float yRotation;

    private Vector3 velocity;

    public delegate void ShiftAction();
    public ShiftAction OnShiftHappens;
    public ShiftAction OnNoShiftHappens;

    private void Start()
    {
        lastChangePosition = transform.position;
        speedChangeTime = Time.time;
        lastChangeDistance = transform.position.x;
    }

    private float lastChangeDistance = 0f;
    void Update()
    {
        // change direction
        bool changedirection = false;
        if ((transform.position.x + curVelocity) >= 6f) {
            acceleration = -1*Mathf.Abs(acceleration);
            changedirection = true;
            curVelocity *= 0.1f;
        } else if ((transform.position.x + curVelocity) <= -6f) {
            acceleration = Mathf.Abs(acceleration);
            changedirection = true;
            curVelocity *= 0.1f;
        } else if(Input.anyKeyDown){
            acceleration *= -1;
            changedirection = true;
        }

        if ((Mathf.Abs(curVelocity+acceleration) < maxVelocity) && (Time.time > speedChangeTime + 0.00025f)){
            curVelocity += acceleration;
            speedChangeTime = Time.time;
        }
        transform.position = transform.position + Vector3.right * curVelocity;
        float currentDistance = Mathf.Abs(transform.position.x - lastChangePosition.x);
        // set floatshifts
        if (changedirection){
            floatshifts = Mathf.Max(Mathf.Min(currentDistance, 5f), 0.11f);
            lastChangeDistance = currentDistance;
            lastChangePosition = transform.position;
            lastDirectionChangeTime = Time.time;
            // IMMA DRAWIN MAH LAZOR!!!
            if (OnNoShiftHappens != null)
                OnNoShiftHappens.Invoke();
        } else if (currentDistance > lastChangeDistance){
            floatshifts = Mathf.Max(Mathf.Min(currentDistance,5f), 0.11f);
        // IMMA DRAWIN MAH LAZOR!!!
        if (OnNoShiftHappens != null)
            OnNoShiftHappens.Invoke();
        }
        if (plHealth < curHealth){
            GameObject nextObject;
            if (plHealth == 3){
                nextObject = GameObject.Find("waves_1");
                curHealth--;
                Destroy(nextObject);
            }
            if (plHealth == 2){
                nextObject = GameObject.Find("waves_2");
                curHealth--;
                Destroy(nextObject);
            }
            if (plHealth == 1){
                nextObject = GameObject.Find("waves_3");
                curHealth--;
                Destroy(nextObject);
            }
            if (plHealth < 1){
                SceneManager.LoadScene(0);
            }
        }
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
