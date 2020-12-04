using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int asteroidRes;
    public GameObject asteroid;
    public float asteroidMaxV;
    public float asteroidInstantiationTime;
    public List<Asteroid> asteroids = new List<Asteroid>();
    public List<Asteroid> bullets = new List<Asteroid>();
    Asteroid player;
    void Start()
    {
        Random.InitState(42);
        for (int i = 0; i < asteroidRes; i++)
        {
            for (int j = 0; j < asteroidRes; j++)
            {

                asteroids.Add(new Asteroid(i, j, new Vector2(Random.Range(-asteroidMaxV, asteroidMaxV), Random.Range(-asteroidMaxV, asteroidMaxV)), asteroidInstantiationTime));
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //start with movement

        //check collisions
        UpdateObject(player);

        foreach (Asteroid aster in asteroids)
        {
            UpdateObject(aster);

        }
        foreach (Asteroid bullet in bullets)
        {
            UpdateObject(bullet);
        }

        //sort list
    }

    void UpdateObject(Asteroid obj)
    {
        obj.xPosition = obj.xPosition + obj.velocity.x;
        obj.yPosition = obj.yPosition + obj.velocity.y;
        //check collision

    }
}




//maybe rename to collideable object or smth
public class Asteroid
{
    public float xPosition;

    public float yPosition;
    public bool instantiate;
    public float tillInstantiation;
    public bool simulated;
    public Vector2 velocity;

    public Asteroid(float xPos, float yPos, Vector2 v, float intantiationTime)
    {
        xPosition = xPos;
        yPosition = yPos;
        velocity = v;
        instantiate = false;
        tillInstantiation = intantiationTime;
        simulated = false;

    }
}
