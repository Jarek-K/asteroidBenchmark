﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace AsteroidBench
{
    public class Generator : MonoBehaviour
    {
        [Header("general settings")]
        public int asteroidAmountX;
        public int matrixSize;
        public bool centerSpawnOnPlayer;
        public Text scoreText;
        public GameObject lostScreen;
        private int score;
        public int scorePerAsteroid;

        [Header("asteroid settings")]
        public float asteroidMaxV;
        public float asteroidInstantiationTime;
        public Mesh asteroidMesh;
        public Material asteroidMaterial;
        public float asteroidRadious;
        [Header("player settings")]
        public GameObject playerGObj;
        public Mesh playerMesh;
        public Material playerMaterial;
        public float playerRadious;

        public float playerVelocity;
        public float playerAngularVelocity;
        private Vector2 playerDirection = Vector2.up;

        private CollidableObj playerObj;
        [Header("bullet settings")]

        public Mesh bulletMesh;
        public Material bulletMaterial;
        public float bulletRadious;
        public float bulletVelocity;
        public float bulletLifetime;
        public float bulletFireTime;
        private float bulletTimer = 0;
        private int lastUsedBullet = 0;

        [Header("camera settings")]
        public Transform mainCamTrans;
        public float frustumMargin;
        public Camera mainCam;

        private List<CollidableObj> collidables = new List<CollidableObj>();
        private List<Matrix4x4> asteroidsInFrustum4x4 = new List<Matrix4x4>();


        private List<CollidableObj> bullets = new List<CollidableObj>();

        private List<List<List<CollidableObj>>> collidableMatrix = new List<List<List<CollidableObj>>>();
        private float camSizeX;
        private float camSizeY;
        private float left, right, top, bottom;
        private Vector2 camPos;
        private bool dead = false;

        private bool loopUpdateCycle = true; //only one update per object per cycle
        void Start()
        {
            camSizeX = mainCam.orthographicSize * mainCam.aspect + frustumMargin;
            camSizeY = mainCam.orthographicSize + frustumMargin;
            StartGame();
            matrixSize = asteroidAmountX;

        }


        private int PositionToMatrix(float pos)
        {
            if (pos < 0)
                return 0;
            else if (pos > matrixSize - 1)
                return matrixSize - 1;
            else
                return (int)pos;
        }
        private void Update()
        {
            // if (dead)
            // {
            //     return;
            // }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                playerObj.xPosition = playerObj.xPosition + playerDirection.x * playerVelocity * Time.deltaTime;
                playerObj.yPosition = playerObj.yPosition + playerDirection.y * playerVelocity * Time.deltaTime;

            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                playerDirection = new Vector2(
                        playerDirection.x * Mathf.Cos(playerAngularVelocity * Time.deltaTime) - playerDirection.y * Mathf.Sin(playerAngularVelocity * Time.deltaTime),
                        playerDirection.x * Mathf.Sin(playerAngularVelocity * Time.deltaTime) + playerDirection.y * Mathf.Cos(playerAngularVelocity * Time.deltaTime)
                    );
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                playerDirection = new Vector2(
                       playerDirection.x * Mathf.Cos(-playerAngularVelocity * Time.deltaTime) - playerDirection.y * Mathf.Sin(-playerAngularVelocity * Time.deltaTime),
                       playerDirection.x * Mathf.Sin(-playerAngularVelocity * Time.deltaTime) + playerDirection.y * Mathf.Cos(-playerAngularVelocity * Time.deltaTime)
                   );
            }
            bulletTimer += Time.deltaTime;
            if (bulletTimer > bulletFireTime)
            {
                bulletTimer = 0;
                lastUsedBullet++;
                if (lastUsedBullet > bullets.Count - 1)
                    lastUsedBullet = 0;
                bullets[lastUsedBullet].simulated = true;

                collidableMatrix[PositionToMatrix(playerObj.xPosition + playerRadious * playerDirection.x)][PositionToMatrix(playerObj.yPosition + playerRadious * playerDirection.y)].Add(bullets[lastUsedBullet]);
                collidableMatrix[PositionToMatrix(bullets[lastUsedBullet].xPosition)][PositionToMatrix(bullets[lastUsedBullet].yPosition)].Remove(bullets[lastUsedBullet]);
                bullets[lastUsedBullet].xPosition = playerObj.xPosition + playerRadious * playerDirection.x;
                bullets[lastUsedBullet].yPosition = playerObj.yPosition + playerRadious * playerDirection.y;

                bullets[lastUsedBullet].velocity = playerDirection * bulletVelocity;
                UpdateUnviableGreyzone(PositionToMatrix(playerObj.xPosition), PositionToMatrix(playerObj.yPosition), matrixSize, bullets[lastUsedBullet]);

                // print("shot" + bullets[lastUsedBullet].xPosition + "dass " +playerObj.xPosition);

            }
            asteroidsInFrustum4x4.Clear();

            camPos = mainCamTrans.position;
            right = camPos.x + camSizeX;
            left = camPos.x - camSizeX;
            top = camPos.y + camSizeY;
            bottom = camPos.y - camSizeY;

            if (CheckCollisionMatrix(playerObj, PositionToMatrix(playerObj.xPosition), PositionToMatrix(playerObj.yPosition)) != null)
            {
                Die();
            }
            for (int i = 0; i < collidableMatrix.Count; i++)
            {
                // print("one line");
                for (int j = 0; j < collidableMatrix[i].Count; j++)
                {
                    for (int k = 0; k < collidableMatrix[i][j].Count; k++)
                    {
                        CollidableObj currentObj = collidableMatrix[i][j][k];
                        if (currentObj == null)
                        {
                            return;
                        }
                        if (loopUpdateCycle != currentObj.updatedInLoop)
                        {
                            UpdateObject(currentObj);
                        }

                        if (currentObj.simulated == true)
                        {
                            int grz = 0;
                            int chkBnds = collidableMatrix[i][j][k].CheckUnbound(i, j);
                            if (chkBnds != 0)
                            {
                                if (chkBnds != currentObj.unviableGreyzone)
                                {
                                    // if (currentObj.unviableGreyzone != null)
                                    // {

                                    // }
                                    // if (!(currentObj.unviableGreyzone != null && Array.Exists(currentObj.unviableGreyzone, element => element == chkBnds)))
                                    // {
                                    switch (chkBnds)
                                    {
                                        case 1:

                                            if (j + 1 < collidableMatrix[i].Count)
                                            {

                                                collidableMatrix[i][j + 1].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i, j + 1, collidableMatrix.Count, currentObj);
                                            }

                                            break;
                                        case 2:

                                            if (i + 1 < collidableMatrix.Count)
                                            {
                                                if (j + 1 < collidableMatrix[i + 1].Count)
                                                {


                                                    collidableMatrix[i + 1][j + 1].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i + 1, j + 1, collidableMatrix.Count, currentObj);

                                                }
                                                else
                                                {
                                                    collidableMatrix[i + 1][j].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i + 1, j, collidableMatrix.Count, currentObj);
                                                }

                                            }
                                            else if (j + 1 < collidableMatrix[i].Count)
                                            {


                                                collidableMatrix[i][j + 1].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i, j + 1, collidableMatrix.Count, currentObj);

                                            }
                                            break;
                                        case 3:

                                            if (i + 1 < collidableMatrix.Count)
                                            {
                                                if (j < collidableMatrix[i + 1].Count)
                                                {


                                                    collidableMatrix[i + 1][j].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i + 1, j, collidableMatrix.Count, currentObj);

                                                }

                                            }

                                            break;
                                        case 4:

                                            if (i + 1 < collidableMatrix.Count)
                                            {
                                                if (j != 0)// && j - 1 < collidableMatrix[i + 1].Count)
                                                {


                                                    collidableMatrix[i + 1][j - 1].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i + 1, j - 1, collidableMatrix.Count, currentObj);

                                                }
                                                else
                                                {
                                                    collidableMatrix[i + 1][j].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i + 1, j, collidableMatrix.Count, currentObj);
                                                }

                                            }
                                            else if (j != 0)// && j - 1 < collidableMatrix[i + 1].Count)
                                            {


                                                collidableMatrix[i][j - 1].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i, j - 1, collidableMatrix.Count, currentObj);

                                            }

                                            break;
                                        case 5:

                                            if (j != 0)
                                            {

                                                collidableMatrix[i][j - 1].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i, j - 1, collidableMatrix.Count, currentObj);
                                            }

                                            break;
                                        case 6:

                                            if (j != 0)
                                            {
                                                if (i != 0)
                                                {
                                                    collidableMatrix[i - 1][j - 1].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;

                                                    UpdateUnviableGreyzone(i - 1, j - 1, collidableMatrix.Count, currentObj);
                                                }
                                                else
                                                {
                                                    collidableMatrix[i][j - 1].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;

                                                    UpdateUnviableGreyzone(i, j - 1, collidableMatrix.Count, currentObj);
                                                }
                                            }
                                            else if (i != 0)
                                            {
                                                collidableMatrix[i - 1][j].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;

                                                UpdateUnviableGreyzone(i - 1, j, collidableMatrix.Count, currentObj);
                                            }

                                            break;
                                        case 7:

                                            if (i != 0)// < collidableMatrix[i].Count)
                                            {

                                                collidableMatrix[i - 1][j].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i - 1, j, collidableMatrix.Count, currentObj);
                                            }

                                            break;
                                        case 8:

                                            if (i != 0)
                                            {
                                                if (j + 1 < collidableMatrix[i - 1].Count)
                                                {
                                                    collidableMatrix[i - 1][j + 1].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i - 1, j + 1, collidableMatrix.Count, currentObj);
                                                }
                                                else
                                                {
                                                    collidableMatrix[i - 1][j].Add(collidableMatrix[i][j][k]);
                                                    collidableMatrix[i][j].RemoveAt(k);
                                                    k--;
                                                    UpdateUnviableGreyzone(i - 1, j, collidableMatrix.Count, currentObj);
                                                }
                                            }
                                            else if (j + 1 < collidableMatrix.Count)
                                            {
                                                collidableMatrix[i][j + 1].Add(collidableMatrix[i][j][k]);
                                                collidableMatrix[i][j].RemoveAt(k);
                                                k--;
                                                UpdateUnviableGreyzone(i, j + 1, collidableMatrix.Count, currentObj);
                                            }

                                            break;
                                        default: break;
                                    }
                                }
                                // }
                            }
                            grz = currentObj.CheckGeryzone(i, j);
                            CollidableObj collided = null;
                            collided = CheckCollisionMatrix(currentObj, i, j);
                            if (collided != null)
                            {
                                if (collided.GetType() == typeof(Bullet) || currentObj.GetType() == typeof(Bullet))
                                {
                                    AddScore();
                                }
                                collided.OnCollision();
                                currentObj.OnCollision();
                            }
                            else
                            if (grz != 0)
                            {
                                // if (!(currentObj.unviableGreyzone != null && Array.Exists(currentObj.unviableGreyzone, element => element == grz))){
                                if (currentObj.unviableGreyzone != grz)
                                {

                                    switch (grz)
                                    {

                                        case 1:
                                            if (j + 1 < collidableMatrix.Count)
                                                collided = CheckCollisionMatrix(currentObj, i, j + 1);
                                            break;
                                        case 2:
                                            if (j + 1 < collidableMatrix.Count)
                                            {
                                                if (i + 1 < collidableMatrix.Count)
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i + 1, j + 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i, j + 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i + 1, j);
                                                }


                                                else
                                                    collided = CheckCollisionMatrix(currentObj, i, j + 1);
                                            }
                                            else if (i + 1 < collidableMatrix.Count)
                                                collided = CheckCollisionMatrix(currentObj, i + 1, j);

                                            break;
                                        case 3:
                                            if (i + 1 < collidableMatrix.Count)
                                                collided = CheckCollisionMatrix(currentObj, i + 1, j);
                                            break;

                                        case 4:
                                            if (j != 0)
                                            {
                                                if (i + 1 < collidableMatrix.Count)
                                                {

                                                    collided = CheckCollisionMatrix(currentObj, i + 1, j - 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i, j - 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i + 1, j);


                                                }
                                                else
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i, j - 1);

                                                }
                                            }
                                            else if (i + 1 < collidableMatrix.Count)
                                            {
                                                collided = CheckCollisionMatrix(currentObj, i + 1, j);

                                            }
                                            break;
                                        case 5:
                                            if (j != 0)
                                                collided = CheckCollisionMatrix(currentObj, i, j - 1);
                                            break;
                                        case 6:
                                            if (j != 0)
                                            {
                                                if (i != 0)
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i - 1, j - 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i, j - 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i - 1, j);


                                                }
                                                else
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i, j - 1);

                                                }
                                            }
                                            else if (i != 0)
                                            {
                                                collided = CheckCollisionMatrix(currentObj, i - 1, j);

                                            }
                                            break;

                                        case 7:
                                            if (i != 0)
                                                collided = CheckCollisionMatrix(currentObj, i - 1, j);
                                            break;
                                        case 8:
                                            if (j + 1 < collidableMatrix.Count)
                                            {
                                                if (i != 0)
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i - 1, j + 1);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i - 1, j);
                                                    if (collided == null)

                                                        collided = CheckCollisionMatrix(currentObj, i, j + 1);

                                                }
                                                else
                                                {
                                                    collided = CheckCollisionMatrix(currentObj, i, j + 1);

                                                }
                                            }
                                            else if (i != 0)
                                            {
                                                collided = CheckCollisionMatrix(currentObj, i - 1, j);

                                            }
                                            break;



                                        default:
                                            break;
                                    }
                                    if (collided != null)
                                    {
                                        if (collided.GetType() == typeof(Bullet) || currentObj.GetType() == typeof(Bullet))
                                        {
                                            AddScore();

                                        }
                                        collided.OnCollision();
                                        currentObj.OnCollision();
                                    }
                                }
                                // }

                            }



                            if (currentObj.GetType() == typeof(Asteroid) || currentObj.GetType() == typeof(Bullet))
                            {
                                // if (currentObj.simulated == true)
                                // {
                                if (currentObj.inFrustum(top, bottom, left, right))
                                {
                                    if (asteroidsInFrustum4x4.Count < 1023) //I know it could be better but it's not really possible that there's more than 1023
                                    {
                                        asteroidsInFrustum4x4.Add(currentObj.Matrice());
                                    }
                                }

                            }

                        }
                        else
                        {
                            if (currentObj.GetType() == typeof(Asteroid))
                            {
                                // Asteroid aster = (Asteroid)currentObj;
                                if (currentObj.spawned == true)
                                {
                                    // print("reviwe");
                                    currentObj.spawned = false;
                                    // currentObj.Instantiate();
                                    currentObj.xPosition = UnityEngine.Random.Range(0, asteroidAmountX);

                                    if (currentObj.xPosition < left || currentObj.xPosition > right || bottom > asteroidAmountX || top < 0)
                                    {
                                        currentObj.yPosition = UnityEngine.Random.Range(0, asteroidAmountX);

                                    }
                                    else
                                    {
                                        do
                                        {
                                            currentObj.yPosition = UnityEngine.Random.Range(0, asteroidAmountX);
                                        } while (!(bottom > currentObj.yPosition || top < currentObj.yPosition));
                                    }
                                    currentObj.simulated = true;
                                    collidableMatrix[(int)currentObj.xPosition][(int)currentObj.yPosition].Add(currentObj);
                                    collidableMatrix[i][j].Remove(currentObj);

                                    currentObj.velocity = new Vector2(UnityEngine.Random.Range(-asteroidMaxV, asteroidMaxV), UnityEngine.Random.Range(-asteroidMaxV, asteroidMaxV));
                                    UpdateUnviableGreyzone((int)currentObj.xPosition, (int)currentObj.yPosition, matrixSize, currentObj);

                                }
                            }
                        }




                    }
                }
            }
            loopUpdateCycle = !loopUpdateCycle;

            Graphics.DrawMeshInstanced(asteroidMesh, 0, asteroidMaterial, asteroidsInFrustum4x4);
            playerGObj.transform.SetPositionAndRotation(new Vector3(playerObj.xPosition, playerObj.yPosition, 0), Quaternion.LookRotation(Vector3.forward, new Vector3(playerDirection.x, playerDirection.y, 0)));
            mainCamTrans.position = new Vector3(playerObj.xPosition, playerObj.yPosition, -10);
        }

        void UpdateObject(CollidableObj obj)
        {
            obj.UpdateObject();

        }
        void Die()
        {
            dead = true;
            lostScreen.SetActive(true);
        }
        private void StartGame()
        {
            UnityEngine.Random.InitState(42);

            for (int i = 0; i < asteroidAmountX; i++)
            {
                collidableMatrix.Add(new List<List<CollidableObj>>());
                for (int j = 0; j < asteroidAmountX; j++)
                {
                    // collidables.Add();
                    collidableMatrix[i].Add(new List<CollidableObj>());
                    //velocity == 0 is possible but i don't think that thats what this part of the task was about
                    collidableMatrix[i][j].Add(new Asteroid(i, j, new Vector2(UnityEngine.Random.Range(-asteroidMaxV, asteroidMaxV), UnityEngine.Random.Range(-asteroidMaxV, asteroidMaxV)), asteroidRadious));
                    UpdateUnviableGreyzone(i, j, matrixSize, collidableMatrix[i][j][0]);
                }
            }
            playerObj = new CollidableObj(asteroidAmountX / 2 + 0.5f, asteroidAmountX / 2 + 0.5f, Vector2.zero, playerRadious);
            // collidableMatrix[(int)playerObj.xPosition][(int)playerObj.yPosition].Add(playerObj);

            for (int i = 0; i < bulletLifetime / bulletFireTime; i++)
            {
                bullets.Add(new Bullet(0, 0, Vector2.zero, bulletRadious));
                bullets[i].simulated = false;
                collidableMatrix[0][0].Add(bullets[i]);
            }


        }


        public void UpdateUnviableGreyzone(int i, int j, int max, CollidableObj obj)
        {
            if (i == 0)
            {
                if (j == 0)
                {
                    obj.unviableGreyzone = 6;
                }
                else if (j + 1 == max)
                {
                    obj.unviableGreyzone = 8;

                }
                else
                {
                    obj.unviableGreyzone = 7;

                }
            }
            else if (i + 1 == max)
            {
                if (j == 0)
                {
                    obj.unviableGreyzone = 4;

                }
                else if (j + 1 == max)
                {
                    obj.unviableGreyzone = 2;

                }
                else
                {
                    obj.unviableGreyzone = 3;

                }
            }
            else if (j == 0)
            {
                obj.unviableGreyzone = 5;

            }
            else if (j + 1 == max)
            {
                obj.unviableGreyzone = 1;

            }
            else
                obj.unviableGreyzone = 0;

        }
        CollidableObj CheckCollisionMatrix(CollidableObj a, int i, int j)
        {

            for (int p = 0; p < collidableMatrix[i][j].Count; p++)
            {
                if (loopUpdateCycle != collidableMatrix[i][j][p].updatedInLoop)
                {
                    UpdateObject(collidableMatrix[i][j][p]);
                }
                if (a != collidableMatrix[i][j][p] && collidableMatrix[i][j][p].simulated == true)
                {
                    if ((a.xPosition - collidableMatrix[i][j][p].xPosition) * (a.xPosition - collidableMatrix[i][j][p].xPosition) + (a.yPosition - collidableMatrix[i][j][p].yPosition) * (a.yPosition - collidableMatrix[i][j][p].yPosition) < ((a.diameter + collidableMatrix[i][j][p].diameter) * (a.diameter + collidableMatrix[i][j][p].diameter)) / 4)
                    {
                        return collidableMatrix[i][j][p];
                    }
                }
            }
            return null;
        }


        private void AddScore()
        {
            score += scorePerAsteroid;
            scoreText.text = "Score: " + score;
        }


        public void RestartGame()
        {
            UnityEngine.Random.InitState(42);
            collidableMatrix.Clear();
            playerDirection = Vector2.up;
            asteroidsInFrustum4x4.Clear();
            score = 0;
            scoreText.text = "Score: " + score;
            dead = false;
            mainCamTrans.position = new Vector3(80, 80, -10);

            StartGame();
            lostScreen.SetActive(false);
            //startSimulation
        }


    }
}

