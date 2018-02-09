using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrickSpawnerSP : MonoBehaviour
{

    public GameObject brickPrefab;
    public int mapSize;
    public float blockDensity;
    public GameObject[] spawnPositions;
    public float spawnPositionFreespace;
    public LayerMask levelMask;
    public float enemyRoom;

    public void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int halfMapSize = mapSize / 2;
        for (int x = -halfMapSize; x <= halfMapSize; x++)
        {
            for (int y = -halfMapSize; y <= halfMapSize; y++)
            {
                Vector3 blockPosition = new Vector3(x, 0.5f, y);
                //print(blockPosition);

                //First roll the dice to see whether to put down a block or not.

                if (Random.Range(0f, 1f) > blockDensity)
                {
                    continue;
                }

                bool placeable = true;

                //No breakable bricks should spawn if it's somewhere where a pillar is.
                if (Physics.OverlapSphere(blockPosition, 0.1f, levelMask).Length > 0)
                {
                    continue;
                }
                
                //Go through all the enemies to ensure that they have enough room.
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (Vector3.Distance(enemies[i].transform.position, blockPosition) < enemyRoom)
                    {
                        placeable = false;
                        break;
                    }
                }

                //Finally check if the spawning location is far enough from where players appear.
                for (int i = 0; i < spawnPositions.Length; i++)
                {
                    if (Vector3.Distance(spawnPositions[i].transform.position, blockPosition) < spawnPositionFreespace)
                    {
                        placeable = false;
                        break;
                    }
                }
                if (!placeable) continue;

                //AND IF ALL THAT WORKED, place a block.
                
                GameObject block = Instantiate(brickPrefab, blockPosition, Quaternion.identity);
            }
        }
    }
}
