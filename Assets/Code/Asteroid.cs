using UnityEngine;

namespace AsteroidBench
{
    public class Asteroid : CollidableObj
    {
        public float tillInstantiation;
        public float instantiationTime = 1;
        public bool readyToInstantiate;

        public Asteroid(float xPos, float yPos, Vector2 v, float rad) : base(xPos, yPos, v, rad)
        {
            xPosition = xPos;
            yPosition = yPos;
            velocity = v;
            diameter = rad;
        }
        public override void UpdateObject()
        {
            if (simulated)
            {
                xPosition = xPosition + velocity.x * Time.deltaTime;
                yPosition = yPosition + velocity.y * Time.deltaTime;
            }
            else
            {
                tillInstantiation -= Time.deltaTime;
                if (tillInstantiation < 0)
                {
                    // readyToInstantiate = true;
                    spawned = true;

                }
            }
            updatedInLoop = !updatedInLoop;

        }

        public override void Instantiate()
        {
            readyToInstantiate = false;
        }

        public override void OnCollision()
        {
            tillInstantiation = instantiationTime;
            simulated = false;
        }

    }
}