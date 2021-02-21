using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningPoint : MonoBehaviour
{
    public TurningPoint[] neighborsTurningPoints;
    public Vector2[] directionToNeighborTurningPoint;
    public bool isPortal;
    public GameObject destinationPortal;

    void Awake()
    {
        InitializeDirectionsToNeighbors();
    }

    private void InitializeDirectionsToNeighbors()
    {
        directionToNeighborTurningPoint = new Vector2[neighborsTurningPoints.Length];
        for (int i = 0; i < neighborsTurningPoints.Length; i++)
        {
            directionToNeighborTurningPoint[i] = (neighborsTurningPoints[i].transform.position - transform.position).normalized;
        }
    }
}
