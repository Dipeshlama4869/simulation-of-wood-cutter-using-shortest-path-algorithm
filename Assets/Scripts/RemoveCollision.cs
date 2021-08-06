using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCollision : MonoBehaviour
{
    public GameObject agentOne, agentTwo;
    public float stopSpeed;
    // Start is called before the first frame update
    void Start()
    {
        stopSpeed = GetComponent<ACOTester>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, agentOne.transform.position) < 3 || Vector3.Distance(transform.position, agentTwo.transform.position) < 3)
        {
            GetComponent<Transform>().position += new Vector3(0, 3, 0);
            if (GetComponent<ACOTester>().speed < agentOne.GetComponent<ACOTester>().speed || GetComponent<ACOTester>().speed < agentTwo.GetComponent<ACOTester>().speed)
            {
                GetComponent<ACOTester>().speed = 0;

                //Debug.Log(GetComponent<PathfindingTester>().collectingSpeed);
            }
            GetComponent<ACOTester>().speed = 0;
            //Debug.Log(GetComponent<PathfindingTester>().speed);
        }
        else { GetComponent<ACOTester>().speed = stopSpeed; }
    }

}
