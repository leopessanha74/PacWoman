using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour
{
    public GameObject[,] mapPoints = new GameObject[27,30];
    void Awake()
    {
        InitializeMapPoints();
	}
    void InitializeMapPoints()
    {
        Object[] allObjects = FindObjectsOfType(typeof(GameObject));

        foreach (GameObject GO in allObjects)
        {
            if (GO.CompareTag("TurningPoints"))
            {
                mapPoints[(int)GO.transform.position.x, (int)GO.transform.position.y] = GO;
            }
        }
    }
}
