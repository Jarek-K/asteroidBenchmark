using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AsteroidBench
{
    public class Generator : MonoBehaviour
    {
        [Header("general settings")]
        public int asteroidAmountX;
        public bool centerSpawnOnPlayer;
        public Text scoreText;
        public GameObject lostScreen;
        private int score;


        [Header("asteroid settings")]
        public float asteroidMaxV;
        public float asteroidInstantiationTime;
        public Mesh asteroidMesh;
        public Material asteroidMaterial;
        public float asteroidRadious;
        [Header("player settings")]

        public Mesh playerMesh;
        public Material playerMaterial;
        public float playerRadious;

        public float playerVelocity;
        public float playerAngularVelocity;

        private CollidableObj playerObj;
        [Header("bullet settings")]

        public Mesh bulletMesh;
        public Material bulletMaterial;
        public float bulletRadious;
        public float bulletVelocity;
        public float bulletLifetime;
        public float bulletFireTime;

        [Header("camera settings")]
        public Transform mainCamTrans;
        public float frustumMargin;
        public Camera mainCam;

        private List<CollidableObj> collidables = new List<CollidableObj>();
        private List<Matrix4x4> asteroidsInFrustum4x4 = new List<Matrix4x4>();


        private List<CollidableObj> bullets = new List<CollidableObj>();

        private List<List<CollidableObj>> collidableMatrix = new List<List<CollidableObj>>();
        private float camSizeX;
        private float camSizeY;
        private float left, right, top, bottom;
        private Vector2 camPos;
        void Start()
        {
            camSizeX = mainCam.orthographicSize * mainCam.aspect + frustumMargin;
            camSizeY = mainCam.orthographicSize + frustumMargin;
            StartGame();

        }



        private void Update()
        {
            asteroidsInFrustum4x4.Clear();

            camPos = mainCamTrans.position;
            right = camPos.x + camSizeX;
            left = camPos.x - camSizeX;
            top = camPos.y + camSizeY;
            bottom = camPos.y - camSizeY;
            float avgChecks = 0;

            for (int i = 0; i < collidables.Count; i++)
            {
                UpdateObject(collidables[i]);
                if (collidables[i].simulated == true)
                {


                    if (collidables[i].GetType() == typeof(Asteroid))
                    {
                        if (collidables[i].inFrustum(top, bottom, left, right))
                        {
                            if (asteroidsInFrustum4x4.Count < 1023)
                            {
                                asteroidsInFrustum4x4.Add(collidables[i].Matrice());
                            }
                        }
                    }


                    // int j = i;

                    // while (j - i < 80)// && j + 1 < collidables.Count )//&& collidables[j + 1].xPosition - collidables[i].xPosition < collidables[j + 1].radius + collidables[i].radius)
                    // {
                    //     j++;

                    //     // if (collidables[i].simulated != false && (collidables[j].yPosition - collidables[i].yPosition) * (collidables[j].yPosition - collidables[i].yPosition) < 0.25f)//Mathf.Pow(collidables[j].yPosition - collidables[i].yPosition, 2) + Mathf.Pow(collidables[j].xPosition - collidables[i].xPosition, 2) < Mathf.Pow(collidables[j].radius + collidables[i].radius, 2))
                    //     // {
                    //     //     // collidables[i].OnCollision();
                    //     //     // collidables[j].OnCollision();
                    //     //     // break;
                    //     //     // int obb = collidables.Find(collidables[i]);
                    //     // }
                    //     avgChecks++;
                    // }
                    for (int j = i; j < i + 80; j++)
                    {
                        if (collidables[i].xPosition < 15)
                        {

                        }
                        if (j + 1 < collidables.Count && collidables[i].simulated != false && collidables[j + 1].xPosition - collidables[i].xPosition < collidables[j + 1].radius + collidables[i].radius && (collidables[j+1].yPosition - collidables[i].yPosition) * (collidables[j+1].yPosition - collidables[i].yPosition) < 0.25f)//Mathf.Pow(collidables[j].yPosition - collidables[i].yPosition, 2) + Mathf.Pow(collidables[j].xPosition - collidables[i].xPosition, 2) < Mathf.Pow(collidables[j].radius + collidables[i].radius, 2))
                        {
                            // collidables[i].OnCollision();
                            // collidables[j].OnCollision();
                            break;
                            // int obb = collidables.Find(collidables[i]);
                        }
                    }
                }

                // return false;


            }
            print(avgChecks / collidables.Count);

            Graphics.DrawMeshInstanced(asteroidMesh, 0, asteroidMaterial, asteroidsInFrustum4x4);

            collidables.Sort((a, b) => (a.xPosition.CompareTo(b.xPosition)));

        }

        void UpdateObject(CollidableObj obj)
        {
            obj.UpdateObject();

        }
        void Die()
        {
            lostScreen.SetActive(true);
            //pause simulation
        }
        private void StartGame()
        {
            Random.InitState(42);
            playerObj = new Bullet(0, 0, Vector2.zero);
            collidables.Add(playerObj);
            for (int i = 0; i < bulletLifetime / bulletFireTime; i++)
            {
                bullets.Add(new Bullet(0, 0, Vector2.zero));
                collidables.Add(bullets[i]);
            }
            for (int i = 0; i < asteroidAmountX; i++)
            {
                collidableMatrix.Add(new List<CollidableObj>());
                for (int j = 0; j < asteroidAmountX; j++)
                {
                    collidables.Add(new Asteroid(i, j, new Vector2(Random.Range(-asteroidMaxV, asteroidMaxV), Random.Range(-asteroidMaxV, asteroidMaxV))));
                    collidableMatrix[i].Add(collidables[collidables.Count - 1]);
                }
            }


        }
        public void RestartGame()
        {
            Random.InitState(42);
            collidables.Clear();
            asteroidsInFrustum4x4.Clear();
            score = 0;
            mainCamTrans.position = new Vector3(80, 80, -10);

            StartGame();
            lostScreen.SetActive(false);
            //startSimulation
        }
    }
}

