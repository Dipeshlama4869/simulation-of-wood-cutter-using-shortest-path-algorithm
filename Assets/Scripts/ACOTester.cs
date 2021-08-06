﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACOTester : MonoBehaviour
{
    // The ACO Controller.
    ACOCON MyACOCON = new ACOCON();
    // Array of possible waypoints.
    List<GameObject> Waypoints = new List<GameObject>();
    // Connections between nodes.
    private List<NewConnection> Connections = new List<NewConnection>();
    // The route generated by the ACO algorith.
    private List<NewConnection> MyRoute = new List<NewConnection>();
    // Debug line offset.
    private Vector3 OffSet = new Vector3(0, 0.5f, 0);
    private int count = 0;
    private int count2 = 0;
    private bool reverse = false;
    public float speed;
    private Rigidbody rb;
    public Text speedText;
    public Text countItemText;
    private int countItem;
    // The Start node for any created route.
    public GameObject StartNode;
    // The max length of a path created by the ACO.
    public int MaxPathLength;
    // Start is called before the first frame update
    void Start()
    {
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            if (tmpWaypointCon)
            {
                if (tmpWaypointCon.WaypointType == WaypointCON.waypointPropsList.Goal)
                {
                    // We are creating a waypoint map of only the goal nodes. We want out ACO ato create the shortest path between the goal nodes.
                    Waypoints.Add(waypoint);
                }
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                NewConnection aConnection = new NewConnection();
                aConnection.SetConnection(waypoint, WaypointConNode, MyACOCON.GetDefaultPheromone());
                Connections.Add(aConnection);
            }
        }
        MyRoute = MyACOCON.ACO(150, 50, Waypoints.ToArray(), Connections, StartNode, MaxPathLength);
    }
    void OnDrawGizmos()
    {
        // Draw path.
        if (MyRoute.Count > 0)
        {
            foreach (NewConnection aConnection in MyRoute)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine((aConnection.GetFromNode().transform.position + OffSet),
                (aConnection.GetToNode().transform.position + OffSet));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // To prevent index out of bound
        if (count < MyRoute.Count && count2 < (2 * MyRoute.Count))
        {
            Debug.Log("x = " + count2);
            Debug.Log("count = " + count);
            Debug.Log("Connec = " + MyRoute.Count);
            if (!reverse)
            {



                // To detect if cube reached its destination
                if (transform.position != MyRoute[count].GetToNode().transform.position)
                {
                    //move forward to a position
                    transform.position = Vector3.MoveTowards(transform.position, MyRoute[count].GetToNode().transform.position, Time.deltaTime * speed);
                }
                else
                {
                    // if cube is reached to its destination then increment count by 1
                    count2++;
                    count++;

                }
            }

        }
        //For Changing direction
        if (count == MyRoute.Count && count2 < 15)
        {
            //reset count
            count = 0;
            //reverse both variable and the list
            reverse = !reverse;
            MyRoute.Reverse();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            other.gameObject.SetActive(false);
            speed = speed - (0.1f) * speed;

            countItem++;

            SetDisplayPerformance();



        }


    }
    void SetDisplayPerformance()
    {
        countItemText.text = "Item Received: " + countItem.ToString();
        speedText.text = "Speed: " + speed.ToString();

    }
}
