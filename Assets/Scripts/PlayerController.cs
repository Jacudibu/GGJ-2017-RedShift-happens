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

    public int shifts;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float intervalCheckTime = 2f;


    private const float velocityDeath = 0.01f;
    private float yRotation;

    private Vector3 velocity;
    private Vector3 lastPosition;

    public delegate void ShiftAction();
    public ShiftAction OnShiftHappens;
    public ShiftAction OnNoShiftHappens;

    private void Start()
    {
        lastPosition = transform.position;
    }

	void Update()
    {
        transform.position = transform.position + Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        Vector3 lastVelocity = velocity;
        velocity = lastPosition - transform.position;

        if(Mathf.Sign(lastVelocity.x) != Mathf.Sign(velocity.x))
        {
            StartCoroutine(Coroutine_CountShift());
            if (OnShiftHappens != null)
                OnShiftHappens.Invoke();
        }

        if (velocity.magnitude < velocityDeath)
            StartCoroutine(Coroutine_CheckIfPlayerStandsStill());

        lastPosition = transform.position;

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

    private IEnumerator Coroutine_CheckIfPlayerStandsStill()
    {
        yield return null;
        if (velocity.magnitude < velocityDeath)
        {
            yield return null;
            if (velocity.magnitude < velocityDeath)
            {
                yield return null;
                if (velocity.magnitude < velocityDeath)
                {
                    yield return null;
                    if (velocity.magnitude < velocityDeath)
                    {
                        shifts = 0;
                        if (OnNoShiftHappens != null)
                            OnNoShiftHappens.Invoke();
                    }
                }
            }
        }
    }
            

    private IEnumerator Coroutine_CountShift()
    {
        shifts++;
        yield return new WaitForSeconds(intervalCheckTime);
        shifts--;

        if (shifts < 0)
            shifts = 0;
    }
}
