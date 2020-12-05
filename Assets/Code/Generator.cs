using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AsteroidBench
{
    public class Generator : MonoBehaviour
    {
        public int asteroidRes;
        // public GameObject asteroid;
        public float asteroidMaxV;
        public float asteroidInstantiationTime;
        public List<CollidableObj> asteroids = new List<CollidableObj>();
        public List<Matrix4x4> asteroidsInFrustum4x4 = new List<Matrix4x4>();


        public List<Asteroid> bullets = new List<Asteroid>();
        Asteroid player;
        public Mesh asteroid;
        public Material asteroidMaterial;
        public Transform mainCamTrans;
        public Camera mainCam;
        private float camSizeX;
        private float camSizeY;
        public float frustumMargin;
        public float asteroidRadious;
        void Start()
        {
            camSizeX = mainCam.orthographicSize * mainCam.aspect + frustumMargin;
            camSizeY = mainCam.orthographicSize + frustumMargin;
            Random.InitState(42);
            for (int i = 0; i < asteroidRes; i++)
            {
                for (int j = 0; j < asteroidRes; j++)
                {

                    asteroids.Add(new Asteroid(i, j, new Vector2(Random.Range(-asteroidMaxV, asteroidMaxV), Random.Range(-asteroidMaxV, asteroidMaxV))));
                }
            }
        }



        private void Update()
        {
            asteroidsInFrustum4x4.Clear();

            Vector2 campPos = mainCamTrans.position;
            float right = campPos.x + camSizeX;
            float left = campPos.x - camSizeX;
            float top = campPos.y + camSizeY;
            float bottom = campPos.y - camSizeY;

            for (int i = 0; i < asteroids.Count; i++)
            {
                UpdateObject(asteroids[i]);


                if (asteroidsInFrustum4x4.Count < 1023)
                {
                    if (asteroids[i].GetType() == typeof(Asteroid) && asteroids[i].inFrustum(top, bottom, left, right))
                    {
                        asteroidsInFrustum4x4.Add(asteroids[i].Matrice());
                    }
                }

                int j = i;
                while (j + 1 < asteroids.Count && Mathf.Pow(asteroids[j + 1].xPosition - asteroids[i].xPosition, 2) > asteroidRadious)
                {
                    j++;
                    if (Mathf.Pow(asteroids[j].yPosition - asteroids[i].yPosition, 2) < asteroidRadious)
                    {
                        // print("fuck");
                    }
                }

                // return false;

            }
            foreach (Asteroid bullet in bullets)
            {
                UpdateObject(bullet);
            }

            Graphics.DrawMeshInstanced(asteroid, 0, asteroidMaterial, asteroidsInFrustum4x4);

            asteroids.Sort((a, b) => (a.xPosition.CompareTo(b.xPosition)));

        }

        void UpdateObject(CollidableObj obj)
        {
            obj.UpdateObject();

        }
    }
}

