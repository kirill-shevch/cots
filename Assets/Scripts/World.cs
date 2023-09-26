using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class World
    {
        public static int _maxX = 50;
        public static int _maxY = 50;
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
            Build(1, 1, 0, 0);
            Build(3, 4, 10, 10, true);
        }

        public static void Build(float xSize, float ySize, float xCord, float yCord, bool addToGrid = false)
        {
            if (xSize < 0 || ySize < 0 || (xSize + xCord) >= _maxX || (ySize + yCord) >= _maxY)
            {
                return;
            }
            GameObject go = new GameObject();
            go.name = Guid.NewGuid().ToString();
            go.transform.position = new Vector2(xCord + xSize / 2, yCord + ySize / 2);
            go.transform.localScale = new Vector2(xSize, ySize);
            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = Resources.Load<Sprite>("Square");
            if (addToGrid)
            {
                for (int i = (int)xCord; i < xCord + xSize; i++)
                {
                    for (int j = (int)yCord; j < yCord + ySize; j++)
                    {
                        grid[i, j] = false;
                    }
                }
            }
        }

        public static List<(int x, int y)> FindShortestPath((int x, int y) start, (int x, int y) end)
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
            List<(int x, int y)> path = new List<(int x, int y)>();
            var node = end;
            while (node != start)
            {
                path.Add(node);
                node = parent[node.x, node.y];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}