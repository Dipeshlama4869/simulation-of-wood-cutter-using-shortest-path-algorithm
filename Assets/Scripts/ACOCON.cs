using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ACOCON
{
    private float DefaultPheromone = 1.0f;
    private float Alpha = 1.0f;
    private float Beta = 0.0001f;
    private float EvaporationFactor = 0.5f;
    private float Q = 0.0006f;

    private List<Ant> Ants = new List<Ant>();

    private List<NewConnection> MyRoute = new List<NewConnection>();

    public ACOCON()
    {
    }

    public float GetDefaultPheromone()
    {
        return DefaultPheromone;
    }

    public List<NewConnection> ACO(int IterationThreshold, int TotalNumAnts, GameObject[] WaypointNodes, List<NewConnection> Connections, GameObject StartNode, int MaxPathLength)
    {
        if (StartNode == null)
        {
            Debug.Log("No Start node");
            return null;
        }

        GameObject currentNode;

        List<GameObject> VisitedNodes = new List<GameObject>();

        Ants.Clear();

        for (int i = 0; i < IterationThreshold; i++)
        {
            for (int i2 = 0; i2 < TotalNumAnts; i2++)
            {
                Ant aAnt = new Ant();

                currentNode = WaypointNodes[Random.Range(0, WaypointNodes.Length)];
                aAnt.SetStartNode(currentNode);
                VisitedNodes.Clear();

                while (VisitedNodes.Count < WaypointNodes.Length)
                {
                    List<NewConnection> ConnectionsFromNodeAndNotVisited = AllConnectionsFromNodeAndNotVisited(currentNode, Connections, VisitedNodes);
                    float TotalPheromoneAndVisibility = CalculateTotalPheromoneAndVisibility(ConnectionsFromNodeAndNotVisited);

                    foreach (NewConnection aConnection in ConnectionsFromNodeAndNotVisited)
                    {
                        float PathProbability = (Mathf.Pow(aConnection.GetPheromoneLevel(), Alpha) * Mathf.Pow((1 / aConnection.GetDistance()), Beta));
                        PathProbability = PathProbability / TotalPheromoneAndVisibility;

                        aConnection.SetPathProbability(PathProbability);
                    }
                    NewConnection largestProbability = null;
                    if (ConnectionsFromNodeAndNotVisited.Count > 0)
                    {
                        largestProbability = ConnectionsFromNodeAndNotVisited[0];
                        for (int i3 = 1; i3 < ConnectionsFromNodeAndNotVisited.Count; i3++)
                        {
                            if (ConnectionsFromNodeAndNotVisited[i3].GetPathProbability() > largestProbability.GetPathProbability())
                            {
                                largestProbability = ConnectionsFromNodeAndNotVisited[i3];
                            }
                            else if (ConnectionsFromNodeAndNotVisited[i3].GetPathProbability() == largestProbability.GetPathProbability())
                            {
                                if (ConnectionsFromNodeAndNotVisited[i3].GetDistance() < largestProbability.GetDistance())
                                {
                                    largestProbability = ConnectionsFromNodeAndNotVisited[i3];
                                }
                            }
                        }
                    }
                    VisitedNodes.Add(currentNode);

                    if (largestProbability != null)
                    {
                        currentNode = largestProbability.GetToNode();
                        aAnt.AddTravelledConnection(largestProbability);
                        aAnt.AddAntTourLength(largestProbability.GetDistance());
                    }
                }// End While loop

                Ants.Add(aAnt);
            }

            foreach (NewConnection aConnection in Connections)
            {
                float Sum = 0;
                foreach (Ant TmpAnt in Ants)
                {
                    List<NewConnection> TmpAntConnections = TmpAnt.GetConnections();
                    foreach (NewConnection tmpConnection in TmpAntConnections)
                    {
                        if (aConnection.Equals(tmpConnection))
                        {
                            Sum += Q / TmpAnt.GetAntTourLength();
                        }
                    }
                }
                float NewPheromoneLevel = (1 - EvaporationFactor) * aConnection.GetPheromoneLevel() + Sum;
                aConnection.SetPheromoneLevel(NewPheromoneLevel);
                aConnection.SetPathProbability(0);
            }

        }

        LogAnts();
        LogRoute(StartNode, MaxPathLength, WaypointNodes, Connections);
        LogConnections(Connections);

        MyRoute = GenerateRoute(StartNode, MaxPathLength, Connections);
        return MyRoute;
    }
    private List<NewConnection> AllConnectionsFromNode(GameObject FromNode, List<NewConnection> Connections)
    {
        List<NewConnection> ConnectionsFromNode = new List<NewConnection>();
        foreach (NewConnection aConnection in Connections)
        {
            if (aConnection.GetFromNode() == FromNode)
            {
                ConnectionsFromNode.Add(aConnection);
            }
        }
        return ConnectionsFromNode;
    }

    private List<NewConnection> AllConnectionsFromNodeAndNotVisited(GameObject FromNode, List<NewConnection> Connections, List<GameObject> VisitedList)
    {
        List<NewConnection> ConnectionsFromNode = new List<NewConnection>();
        foreach (NewConnection aConnection in Connections)
        {
            if (aConnection.GetFromNode() == FromNode)
            {
                if (!VisitedList.Contains(aConnection.GetToNode()))
                {
                    ConnectionsFromNode.Add(aConnection);
                }
            }
        }
        return ConnectionsFromNode;
    }

    private float CalculateTotalPheromoneAndVisibility(List<NewConnection> ConnectionsFromNodeAndNotVisited)
    {
        float TotalPheromoneAndVisibility = 0;

        foreach (NewConnection aConnection in ConnectionsFromNodeAndNotVisited)
        {
            TotalPheromoneAndVisibility += (Mathf.Pow(aConnection.GetPheromoneLevel(), Alpha) * Mathf.Pow((1 / aConnection.GetDistance()), Beta));
        }
        return TotalPheromoneAndVisibility;
    }

    public List<NewConnection> GenerateRoute(GameObject StartNode, int MaxPath, List<NewConnection> Connections)
    {
        GameObject CurrentNode = StartNode;
        List<NewConnection> Route = new List<NewConnection>();

        NewConnection HighestPheromoneConnection = null;
        int PathCount = 1;

        while (CurrentNode != null)
        {
            List<NewConnection> AllFromConnections = AllConnectionsFromNode(CurrentNode, Connections);

            if (AllFromConnections.Count > 0)
            {
                HighestPheromoneConnection = AllFromConnections[0];

                foreach (NewConnection aConnection in AllFromConnections)
                {
                    if (aConnection.GetPheromoneLevel() > HighestPheromoneConnection.GetPheromoneLevel())
                    {
                        HighestPheromoneConnection = aConnection;
                    }
                }
                Route.Add(HighestPheromoneConnection);
                CurrentNode = HighestPheromoneConnection.GetToNode();
            }
            else
            {
                CurrentNode = null;
            }
            if (CurrentNode.Equals(StartNode))
            {
                CurrentNode = null;
            }
            PathCount++;
        }
        return Route;
    }

    private void LogConnections(List<NewConnection> Connections)
    {
        foreach (NewConnection aConnection in Connections)
        {
            Debug.Log(">" + aConnection.GetFromNode().name + " | ---> " + aConnection.GetToNode().name + " = " + aConnection.GetPheromoneLevel());
        }
    }

    private void LogRoute(GameObject StartNode, int MaxPath, GameObject[] WaypointNodes, List<NewConnection> Connections)
    {
        GameObject CurrentNode = null;
        foreach (GameObject GameObjectNode in WaypointNodes)
        {
            if (GameObjectNode.Equals(StartNode))
            {
                CurrentNode = GameObjectNode;
            }
        }
        NewConnection HighestPheromoneConnection = null;
        string Output = "Route (Q: " + Q + ", Alpha: " + Alpha + ", Beta: " + Beta + ",EvaporationFactor: " + EvaporationFactor + ", DefaultPheromone: " + DefaultPheromone + "):\n";
        int PathCount = 1;
        while (CurrentNode != null)
        {
            List<NewConnection> AllFromConnections = AllConnectionsFromNode(CurrentNode, Connections);

            if (AllFromConnections.Count > 0)
            {
                HighestPheromoneConnection = AllFromConnections[0];

                foreach (NewConnection aConnection in AllFromConnections)
                {
                    if (aConnection.GetPheromoneLevel() > HighestPheromoneConnection.GetPheromoneLevel())
                    {
                        HighestPheromoneConnection = aConnection;
                    }
                }
                CurrentNode = HighestPheromoneConnection.GetToNode();
                Output += "| FROM: " + HighestPheromoneConnection.GetFromNode().name + ", TO: " + HighestPheromoneConnection.GetToNode().name + " (Pheromone Level: " + HighestPheromoneConnection.GetPheromoneLevel() + ") | \n";
            }
            else
            {
                CurrentNode = null;
            }
            if (CurrentNode.Equals(StartNode))
            {
                CurrentNode = null;
                Output += "HOME (Total Nodes:" + WaypointNodes.Length + ", Nodes in Route: " + PathCount + ").\n";
            }
            if (PathCount > MaxPath)
            {
                CurrentNode = null;
                Output += "MAX PATH (Total Nodes:" + WaypointNodes.Length + ", Nodes in Route: " + PathCount + ").\n";
            }
            PathCount++;
        }
        Debug.Log(Output);
    }

    private void LogAnts()
    {
        string Output = "Ants (Q: " + Q + ", Alpha: " + Alpha + ", Beta: " + Beta + ", EvaporationFactor:" + EvaporationFactor + ", DefaultPheromone: " + DefaultPheromone + "):\n";
        for (int i = 0; i < Ants.Count; i++)
        {
            Output += "Ant " + i + " - Start Node: " + Ants[i].GetStartNode().name + " | Tour Length: " + Ants[i].GetAntTourLength() + "\n";
        }
        Debug.Log(Output);
    }
}




