using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts
{
    public static class World
    {
        public static string _circleSprite = "Circle";
        public static string _nineSlicedSprite = "9-Sliced";

        public static int _maxX = 30;
        public static int _maxY = 30;
        public static int _builingPadding = 2;
        public static bool[,] grid;
        static World()
        {
            grid = new bool[_maxX, _maxY];
            for (int i = 0; i < _maxX; i++)
            {
                for (int j = 0; j < _maxY; j++)
                {
                    grid[i, j] = true;
                }
            }
        }

        public static void Initialize()
        {
            var user1 = Build(1, 1, 0, 0, _circleSprite);
            user1.GetComponent<SpriteRenderer>().color = Color.blue;
            user1.AddComponent<Routing>();
            user1.AddCollider<CircleCollider2D>();

            var user2 = Build(1, 1, 1, 1, _circleSprite);
            user2.GetComponent<SpriteRenderer>().color = Color.blue;
            user2.AddComponent<Routing>();
            user2.AddCollider<CircleCollider2D>();

            var user3 = Build(1, 1, 0, 1, _circleSprite);
            user3.GetComponent<SpriteRenderer>().color = Color.blue;
            user3.AddComponent<Routing>();
            user3.AddCollider<CircleCollider2D>();

            var user4 = Build(1, 1, 1, 0, _circleSprite);
            user4.GetComponent<SpriteRenderer>().color = Color.blue;
            user4.AddComponent<Routing>();
            user4.AddCollider<CircleCollider2D>();

            var user5 = Build(1, 1, 29, 29, _circleSprite);
            user5.GetComponent<SpriteRenderer>().color = Color.blue;
            user5.AddComponent<Routing>();
            user5.AddCollider<CircleCollider2D>();

            var user6 = Build(1, 1, 28, 28, _circleSprite);
            user6.GetComponent<SpriteRenderer>().color = Color.blue;
            user6.AddComponent<Routing>();
            user6.AddCollider<CircleCollider2D>();

            var user7 = Build(1, 1, 29, 28, _circleSprite);
            user7.GetComponent<SpriteRenderer>().color = Color.blue;
            user7.AddComponent<Routing>();
            user7.AddCollider<CircleCollider2D>();

            var user8 = Build(1, 1, 28, 29, _circleSprite);
            user8.GetComponent<SpriteRenderer>().color = Color.blue;
            user8.AddComponent<Routing>();
            user8.AddCollider<CircleCollider2D>();

            for (int i = 0, fails = 0; i < 20;)
            {
                float xSize = UnityEngine.Random.Range(3, 7);
                float ySize = UnityEngine.Random.Range(3, 7);
                float xCord = UnityEngine.Random.Range(1, 30 - (int)xSize);
                float yCord = UnityEngine.Random.Range(1, 30 - (int)ySize);
                if (IsValid(xSize, ySize, xCord, yCord) && IsFreeSpace(xSize, ySize, xCord, yCord))
                {
                    var go = Build(xSize, ySize, xCord, yCord, _nineSlicedSprite);
                    go.AddToGrid(xSize, ySize, xCord, yCord);
                    go.AddCollider<BoxCollider2D>();
                    i++;
                    fails = 0;
                }
                else
                {
                    fails++;
                    if (fails > 5)
                    {
                        i++;
                    }
                }
            }
        }

        public static GameObject Build(float xSize, float ySize, float xCord, float yCord, string spriteName)
        {
            GameObject go = new GameObject();
            go.name = Guid.NewGuid().ToString();
            go.transform.position = new Vector2(xCord + xSize / 2, yCord + ySize / 2);
            go.transform.localScale = new Vector2(xSize, ySize);
            var rigidBody = go.AddComponent<Rigidbody2D>();
            //rigidBody.isKinematic = addToGrid ? true : false;
            //if (addCollider)
            //{
            //    Collider2D boxCollider = addToGrid ? go.AddComponent<BoxCollider2D>() : go.AddComponent<CircleCollider2D>();
            //    var material = new PhysicsMaterial2D();
            //    material.friction = 0.0001f;
            //    boxCollider.sharedMaterial = material;
            //}
            rigidBody.gravityScale = 0f;
            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = Resources.Load<Sprite>(spriteName);
            //sprite.sprite = addToGrid ? Resources.Load<Sprite>("9-Sliced") : Resources.Load<Sprite>("Circle");
            return go;
        }

        public static void AddToGrid(this GameObject go, float xSize, float ySize, float xCord, float yCord)
        {
            var rigidBody = go.GetComponent<Rigidbody2D>();
            if (rigidBody != null)
            {
                rigidBody.isKinematic = true;
            }

            for (int i = (int)xCord; i < xCord + xSize; i++)
            {
                for (int j = (int)yCord; j < yCord + ySize; j++)
                {
                    grid[i, j] = false;
                }
            }
        }
        public static void AddCollider<T>(this GameObject go) where T : Collider2D
        {
            var collider = go.AddComponent<T>();
            var material = new PhysicsMaterial2D();
            material.friction = 0.0001f;
            collider.sharedMaterial = material;
        }

        public static bool IsValid(float xSize, float ySize, float xCord, float yCord)
        {
            return 
                xSize > 0 && 
                ySize > 0 && 
                xCord > 0 &&
                yCord > 0 &&
                (xSize / 2 + xCord) <= _maxX && 
                (ySize / 2 + yCord) <= _maxY;
        }

        public static bool IsFreeSpace(float xSize, float ySize, float xCord, float yCord)
        {
            for (int i = (int)xCord - _builingPadding; i < xCord + xSize + _builingPadding; i++)
            {
                for (int j = (int)yCord - _builingPadding; j < yCord + ySize + _builingPadding; j++)
                {
                    if (i < 0 || j < 0 || i >= _maxX || j >= _maxY || !grid[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Stack<(int x, int y)> FindShortestPath((int x, int y) start, (int x, int y) end)
        {
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            double[] weights = { Math.Sqrt(2), 1, Math.Sqrt(2), 1, 1, Math.Sqrt(2), 1, Math.Sqrt(2) };
            int n = grid.GetLength(0);
            int m = grid.GetLength(1);
            bool[,] visited = new bool[n, m];
            double[,] g = new double[n, m];
            double[,] h = new double[n, m];
            (int x, int y)[,] parent = new (int x, int y)[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    h[i, j] = Math.Sqrt(Math.Pow(i - end.x, 2) + Math.Pow(j - end.y, 2));
            Utils.PriorityQueue<(int x, int y), double> pq = new Utils.PriorityQueue<(int x, int y), double>();
            pq.Enqueue(start, h[start.x, start.y]);
            while (pq.Count > 0)
            {
                var curr = pq.Dequeue();
                if (visited[curr.x, curr.y])
                    continue;
                visited[curr.x, curr.y] = true;
                if (curr.x == end.x && curr.y == end.y)
                    break;
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = curr.x + dx[i];
                    int ny = curr.y + dy[i];
                    if (nx < 0 || nx >= n || ny < 0 || ny >= m || !grid[nx, ny] || visited[nx, ny])
                        continue;
                    double cost = g[curr.x, curr.y] + weights[i];
                    if (g[nx, ny] == 0 || g[nx, ny] > cost)
                    {
                        g[nx, ny] = cost;
                        parent[nx, ny] = curr;
                        pq.Enqueue((nx, ny), g[nx, ny] + h[nx, ny]);
                    }
                }
            }
            Stack<(int x, int y)> path = new Stack<(int x, int y)>();
            var node = end;
            while (node != start)
            {
                path.Push(node);
                node = parent[node.x, node.y];
            }
            //path.Push(start);
            //path.Reverse();
            return path;
        }
    }
}