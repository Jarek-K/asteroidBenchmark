using UnityEngine;

namespace AsteroidBench
{
    public class Asteroid : CollidableObj
    {
        public float tillInstantiation;
        public float instantiationTime = 1;

        public Asteroid(float xPos, float yPos, Vector2 v, float rad) : base(xPos, yPos, v, rad)
        {

        }
        public Asteroid(float xPos, float yPos, Vector2 v, float rad, float instaTime) : base(xPos, yPos, v, rad)
        {

            instantiationTime = instaTime;
        }
        //I think that virtual functions are slower, in this situation it would probably save around .2ms 
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



        public override void OnCollision()
        {
            tillInstantiation = instantiationTime;
            simulated = false;
        }

    }
}