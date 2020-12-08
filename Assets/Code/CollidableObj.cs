using UnityEngine;

namespace AsteroidBench
{
    public abstract class CollidableObj
    {
        public float xPosition;

        public float yPosition;

        public bool simulated = true;
        public Vector2 velocity;
        public float radius = 0.25f;
        public CircleCollider2D col;

        public CollidableObj(float xPos, float yPos, Vector2 v)
        {
            xPosition = xPos;
            yPosition = yPos;
            velocity = v;
        }
        public Matrix4x4 Matrice()
        {
            return Matrix4x4.identity * Matrix4x4.Translate(new Vector3(xPosition, yPosition, 0));
        }

        public bool inFrustum(float top, float bottom, float left, float right)
        {


            if (xPosition > right || xPosition < left || yPosition > top || yPosition < bottom)
            {
                return false;
            }
            return true;


        }

        public void UpdateObject()
        {
            if (simulated)
            {
                xPosition = xPosition + velocity.x * Time.deltaTime;
                yPosition = yPosition + velocity.y * Time.deltaTime;
            }

        }
        public virtual void OnCollision()
        {
            simulated = false;
        }


    }
}
