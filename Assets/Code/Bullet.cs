using UnityEngine;

namespace AsteroidBench
{
    public class Bullet : CollidableObj
    {
        public float tillInstantiation;

        public Bullet(float xPos, float yPos, Vector2 v, float rad) : base(xPos, yPos, v, rad)
        {
          
        }


    }
}