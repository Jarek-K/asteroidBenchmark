using UnityEngine;

namespace AsteroidBench
{
    public class Asteroid : CollidableObj
    {
        public float tillInstantiation;
        public float instantiationTime;

        public Asteroid(float xPos, float yPos, Vector2 v) : base(xPos, yPos, v)
        {
            xPosition = xPos;
            yPosition = yPos;
            velocity = v;
        }

        public override void OnCollision()
        {
            tillInstantiation = instantiationTime;
            simulated = false;
        }

    }
}