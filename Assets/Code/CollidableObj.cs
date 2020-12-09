using UnityEngine;

namespace AsteroidBench
{
    public class CollidableObj
    {
        public float xPosition;

        public float yPosition;

        public bool simulated = true;
        public bool spawned = false;
        public Vector2 velocity;
        public float diameter;
        public CircleCollider2D col;
        public int greyzone = 0; //I know this should be some enum or sth
        public bool updatedInLoop = false;
        public int unviableGreyzone = 0;



        public CollidableObj(float xPos, float yPos, Vector2 v, float rad)
        {
            xPosition = xPos;
            yPosition = yPos;
            velocity = v;
            diameter = rad;
        }
        public Matrix4x4 Matrice()
        {
            return Matrix4x4.TRS(new Vector3(xPosition, yPosition, 0), Quaternion.identity, new Vector3(diameter, diameter, 1));// Matrix4x4.identity * Matrix4x4.Translate(new Vector3(xPosition, yPosition, 0));
        }

        public bool inFrustum(float top, float bottom, float left, float right)
        {


            if (xPosition > right || xPosition < left || yPosition > top || yPosition < bottom)
            {
                return false;
            }
            return true;


        }

        public virtual void UpdateObject()
        {
            if (simulated)
            {
                xPosition = xPosition + velocity.x * Time.deltaTime;
                yPosition = yPosition + velocity.y * Time.deltaTime;
            }
            updatedInLoop = !updatedInLoop;


        }

    

        public int CheckBounds(int x, int y, float cX, float cY, float rad)
        {
            if (cX > 1 + x - rad)
            {
                if (cY > 1 + y - rad)
                {
                    return 2;


                }
                if (cY < y + rad)
                {
                    return 4;


                }
                return 3;



            }
            if (cX < x + rad)
            {
                if (cY > 1 + y - rad)
                {
                    return 8;


                }
                if (cY < y + rad)
                {
                    return 6;


                }
                return 7;



            }
            if (cY > 1 + y - rad)
            {
                return 1;


            }
            if (cY < y + rad)
            {
                return 5;


            }
            return 0;


        }

        public int CheckGeryzone(int x, int y)
        {
            greyzone = CheckBounds(x, y, xPosition, yPosition, diameter / 2);
            return greyzone;
        }

        public int CheckUnbound(int x, int y)
        {
            return CheckBounds(x, y, xPosition, yPosition, 0.0f);
        }
        public virtual void OnCollision()
        {
            simulated = false;
        }


    }
}
