using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Graph
{
    // A list of graph connections.
    private List<NewConnection> WaypointConnections = new List<NewConnection>();
    public Graph()
    {
    }
    // Add connection.
    public void AddConnection(NewConnection aConnection)
    {
        WaypointConnections.Add(aConnection);
    }
    // Get the connections from a node to the nodes it is connected to.
    public List<NewConnection> GetConnections(GameObject FromNode)
    {
        List<NewConnection> TmpConnections = new List<NewConnection>();
        foreach (NewConnection aConnection in WaypointConnections)
        {
            if (aConnection.GetFromNode().Equals(FromNode))
            {
                TmpConnections.Add(aConnection);
            }
        }
        return TmpConnections;
    }
}