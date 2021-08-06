using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewConnection
{
    private float Cost = 0;
    private GameObject FromNode;
    private GameObject ToNode;
    private float distance;
    private float PheromoneLevel;
    private float PathProbability;
    public NewConnection()
    {
    }
    public void SetConnection(GameObject FromNode, GameObject ToNode, float DefaultPheromoneLevel)
    {
        this.FromNode = FromNode;
        this.ToNode = ToNode;
        distance = Vector3.Distance(FromNode.transform.position, ToNode.transform.position);
        PheromoneLevel = DefaultPheromoneLevel;
        PathProbability = 0;
    }
    public GameObject GetFromNode()
    {
        return FromNode;
    }
    public GameObject GetToNode()
    {
        return ToNode;
    }
    public float GetDistance()
    {
        return distance;
    }
    public float GetPheromoneLevel()
    {
        return PheromoneLevel;
    }
    public void SetPheromoneLevel(float PheromoneLevel)
    {
        this.PheromoneLevel = PheromoneLevel;
    }
    public float GetPathProbability()
    {
        return PathProbability;
    }
    public void SetPathProbability(float PathProbability)
    {
        this.PathProbability = PathProbability;
    }
    //For the A* algorithm
    public float GetCost()
    {
        if (Cost == 0)
        {
            Cost = distance;
        }
        return Cost;
    }
    public void SetFromNode(GameObject FromNode)
    {
        this.FromNode = FromNode; Cost = 0;
    }
    public void SetToNode(GameObject ToNode)
    {
        this.ToNode = ToNode; Cost = 0;
    }
}

