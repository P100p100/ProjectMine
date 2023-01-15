using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenChunks_M : MonoBehaviour
{
    public GameObject prefab;
    [Range(0,100)]
    public int size = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                Instantiate(prefab, new Vector3(x * 128, 0, z * 128), Quaternion.identity);
            }
        }
    }
}
