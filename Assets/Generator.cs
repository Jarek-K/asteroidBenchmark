using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int asteroidRes;
    public GameObject asteroid;
    public float asteroidMaxV;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(42);
        for (int i = 0; i < asteroidRes; i++)
        {
            for (int j = 0; j < asteroidRes; j++)
            {
                GameObject tmpAsteroid = Object.Instantiate(asteroid, new Vector3(i, j, 0), Quaternion.identity);
                tmpAsteroid.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-asteroidMaxV, asteroidMaxV), Random.Range(-asteroidMaxV, asteroidMaxV));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
