using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Both spawns bricks on the playfield and marks which spaces are blocked or not.
/// </summary>

public class BrickSpawnerSP : MonoBehaviour
{

    public GameObject brickPrefab;
    public int mapSizeX;
    public int mapSizeY;
    public float blockDensity;
    public GameObject[] spawnPositions;
    public float spawnPositionFreespace;
    public LayerMask levelMask;
    public float enemyRoom;
    [HideInInspector]
    public bool[,] accessibilityGrid;

    public void Start()
    {
        accessibilityGrid = new bool[mapSizeX+1,mapSizeY+1];
        print("I ran.");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int halfMapSizeX = mapSizeX / 2;
        int halfMapSizeY = mapSizeY / 2;
        for (int x = 0; x <= mapSizeX; x++)
        {
            for (int y = 0; y <= mapSizeY; y++)
            {
                Vector3 blockPosition = new Vector3(x-halfMapSizeX, 0.5f, y-halfMapSizeY);
                //print(blockPosition);

                //First roll the dice to see whether to put down a block or not.

                if (Random.Range(0f, 1f) > blockDensity)
                {
                    //If the random number generated is beyond the block density percentage, don't spawn a block there and mark it as clear.
                    accessibilityGrid[x, y] = true;
                    continue;
                }

                bool placeable = true;

                //No breakable bricks should spawn if it's somewhere where a pillar is.
                if (Physics.OverlapSphere(blockPosition, 0.1f, levelMask).Length > 0)
                {
                    //Mark the space as inaccessible.
                    accessibilityGrid[x, y] = false;
                    continue;
                }
                
                //Go through all the enemies to ensure that they have enough room.
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (Vector3.Distance(enemies[i].transform.position, blockPosition) < enemyRoom)
                    {
                        //Places where enemies are or have wiggle room should be marked as accessible.
                        accessibilityGrid[x, y] = true;
                        placeable = false;
                        break;
                    }
                }

                //Finally check if the spawning location is far enough from where players appear.
                for (int i = 0; i < spawnPositions.Length; i++)
                {
                    if (Vector3.Distance(spawnPositions[i].transform.position, blockPosition) < spawnPositionFreespace)
                    {
                        accessibilityGrid[x, y] = true;
                        placeable = false;
                        break;
                    }
                }
                if (!placeable) continue;

                //AND IF ALL THAT WORKED, place a block.
                //And mark the place on the grid as inaccessible.

                accessibilityGrid[x, y] = false;
                GameObject block = Instantiate(brickPrefab, blockPosition, Quaternion.identity);
            }
        }
    }
}
