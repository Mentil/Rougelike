
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [Serializable]
        public class Count
        {
            public int minimum;
            public int maximum;

            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }


        public int columns = 16;
        public int rows = 16;
        public Count wallCount = new Count(50, 80);
        public Count foodCount = new Count(5, 10);
        public Count enemyCount = new Count(1, 2);
        public GameObject exit;            
        public GameObject[] floorTiles;
        public GameObject[] wallTiles;
        public GameObject[] foodTiles;
        public GameObject[] enemyTiles;
        public GameObject[] outerWallTiles;

        private Transform _boardHolder;
        private readonly List<Vector3> _gridPositions = new List<Vector3>();

        void InitialiseList()
        {
            _gridPositions.Clear();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if ((x == 0 && y == 0) || (x == columns - 1 && y == rows - 1))
                    {
                        continue;
                    }
                    _gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }

        void BoardSetup()
        {
            _boardHolder = new GameObject("Board").transform;

            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {

                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    if (x == -1 || x == columns || y == -1 || y == rows)
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                    var instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);

                    instance.transform.SetParent(_boardHolder);
                }
            }
        }

        Vector3 RandomPosition()
        {
            int randomIndex = Random.Range(0, _gridPositions.Count);
            Vector3 randomPosition = _gridPositions[randomIndex];
            _gridPositions.RemoveAt(randomIndex);

            return randomPosition;
        }

        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        public void SetupScene(int level)
        {
            BoardSetup();
            InitialiseList();
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

            enemyCount.minimum = (int)Mathf.Log(level, 2f) * 10;
            enemyCount.maximum = (int) Mathf.Log(level, 2f) * 20;
            LayoutObjectAtRandom(enemyTiles, enemyCount.minimum, enemyCount.maximum);
        }
    }
}
