using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the position of an object on a grid. Mostly for destroyable blocks.
/// </summary>

public class GridPositioningTracker : MonoBehaviour
{
    BrickSpawnerSP BSSP;
    int xGridPos;
    int yGridPos;
    public int gridSize;
    public bool mobile = false; //Does the object's grid position change over time?
    public bool blocks = true; //Does this object count as a non-navigatable position?

    // Use this for initialization
    void Start()
    {
        BSSP = FindObjectOfType<BrickSpawnerSP>();
        SetGridPosition(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (mobile) SetGridPosition();
    }

    void SetGridPosition(bool firstTime = false)
    {
        if (!firstTime)
        {
            int newXGridPos = (Mathf.RoundToInt(transform.position.x) + (BSSP.mapSizeX / 2)) / gridSize;
            int newYGridPos = (Mathf.RoundToInt(transform.position.z) + (BSSP.mapSizeY / 2)) / gridSize;
            if (blocks)
            {
                BSSP.accessibilityGrid[newXGridPos, newYGridPos] = true;
                if (newXGridPos != xGridPos || newYGridPos != yGridPos) BSSP.accessibilityGrid[xGridPos, yGridPos] = false;
            }
            xGridPos = newXGridPos;
            yGridPos = newYGridPos;
        }
        else
        {
            xGridPos = (Mathf.RoundToInt(transform.position.x) + (BSSP.mapSizeX / 2)) / gridSize;
            yGridPos = (Mathf.RoundToInt(transform.position.z) + (BSSP.mapSizeY / 2)) / gridSize;
            if (blocks)
            {
                BSSP.accessibilityGrid[xGridPos, yGridPos] = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (blocks)
        {
            BSSP.accessibilityGrid[xGridPos,yGridPos] = true;
        }
    }
}
