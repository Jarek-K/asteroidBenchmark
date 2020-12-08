using UnityEngine;

namespace AsteroidBench
{
    public class Bullet : CollidableObj
    {
        public float tillInstantiation;

        public Bullet(float xPos, float yPos, Vector2 v) : base(xPos, yPos, v)
        {
            xPosition = xPos;
            yPosition = yPos;
            velocity = v;
        }


    }
}