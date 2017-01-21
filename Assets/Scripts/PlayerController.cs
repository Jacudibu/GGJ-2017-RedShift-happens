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
            StartCoroutine(CountShift());
            if (OnShiftHappens != null)
                OnShiftHappens.Invoke();
        }

        lastPosition = transform.position;
	}

    private IEnumerator CountShift()
    {
        shifts++;
        yield return new WaitForSeconds(intervalCheckTime);
        shifts--;

        if (shifts == 0)
        {
            if (OnNoShiftHappens != null)
                OnNoShiftHappens.Invoke();
        }
    }
}
