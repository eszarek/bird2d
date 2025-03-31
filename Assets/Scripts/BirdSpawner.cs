using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Assign prefabs in the Inspector
    [SerializeField] private Bird.BirdType[] BirdTypes; // Assign BirdTypes in the Inspector

    void Awake()
    {
        for (int i = 0; i < BirdTypes.Length; i++)
        {
            int birdCount = Mathf.RoundToInt(BirdTypes[i].BirdCount); // Ensure integer count
            for (int j = 0; j < birdCount; j++)
            {
                SpawnBird(BirdTypes[i]);
            }
        }
    }

    void SpawnBird(Bird.BirdType birdType)
    {
        if (prefabs.Length == 0)
        {
            Debug.LogError("No bird prefabs assigned!");
            return;
        }

        GameObject birdObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position, Quaternion.identity);

        Bird bird = birdObject.GetComponent<Bird>();
        if (bird != null)
        {
            bird.Type = birdType;
            bird.ResetBird();
        }
        else
        {
            Debug.LogError("Spawned bird prefab does not have a Bird script attached!");
        }
    }
}
