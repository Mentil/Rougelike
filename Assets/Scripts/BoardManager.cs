
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        public int Columns = 16;
        public int Rows = 16;
        public (int Min, int Max) WallCount = (50, 80);
        public (int Min, int Max) FoodCount = (5, 10);
        public (int Min, int Max) EnemyCount = (1, 2);
        public GameObject Exit;            
        public GameObject[] FloorTiles;
        public GameObject[] WallTiles;
        public GameObject[] FoodTiles;
        public GameObject[] EnemyTiles;
        public GameObject[] OuterWallTiles;

        private Transform _boardHolder;
        private readonly List<Vector3> _gridPositions = new List<Vector3>();

        void InitialiseList()
        {
            _gridPositions.Clear();

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if ((x == 0 && y == 0) || (x == Columns - 1 && y == Rows - 1))
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

            for (int x = -1; x < Columns + 1; x++)
            {
                for (int y = -1; y < Rows + 1; y++)
                {

                    GameObject toInstantiate = FloorTiles[Random.Range(0, FloorTiles.Length)];

                    if (x == -1 || x == Columns || y == -1 || y == Rows)
                        toInstantiate = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];

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
            LayoutObjectAtRandom(WallTiles, WallCount.Min, WallCount.Max);
            LayoutObjectAtRandom(FoodTiles, FoodCount.Min, FoodCount.Max);
            Instantiate(Exit, new Vector3(Columns - 1, Rows - 1, 0f), Quaternion.identity);

            EnemyCount.Min = (int)Mathf.Log(level, 2f) * 10;
            EnemyCount.Max = (int) Mathf.Log(level, 2f) * 20;
            LayoutObjectAtRandom(EnemyTiles, EnemyCount.Min, EnemyCount.Max);
        }
    }
}
