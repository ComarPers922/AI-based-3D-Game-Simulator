using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace AMazeCS
{
    public enum TileType
    {
        Null = -1, Road, Wall
    }
    public class MazeData
    {
        public readonly TileType[,] Maze;
        public readonly int Size;
        public bool[,] IsPath { private set; get; }

        private readonly bool[,] visited;
        private readonly int[,] direction = new int[4,2]{{-1,0},{0,1},{1,0},{0,-1}};
        public readonly bool[,] solutionVisited;

        public readonly int entranceX = 0;
        public readonly int entranceY = 1;
        public readonly int exitX;
        public readonly int exitY; 

        public MazeData(int size)
        {
            Size = size % 2 == 0 ? size + 1 : size;
            exitX = Size - 1;
            exitY = Size - 2;
            Maze = new TileType[Size,Size];
            visited = new bool[Size,Size];
            IsPath = new bool[Size,Size];
            solutionVisited = new bool[Size,Size];
            Reset();
        }
        public bool InArea(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }
        public void Reset()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        Maze[i,j] = TileType.Road;
                    }
                    else
                    {
                        if (i != 0 && i != Size - 1 && j != 0 && j != Size - 1)
                        {
                            Maze[i, j] = UnityEngine.Random.Range(0,101) < 70 ? TileType.Wall : TileType.Road;
                        }
                        else
                        {
                            Maze[i, j] = TileType.Wall;
                        }
                    }
                    visited[i,j] = false;
                    solutionVisited[i, j] = false;
                    IsPath[i, j] = false;
                }
            }
            //Maze[entranceX,entranceY] = TileType.Road;
            //Maze[exitX,exitY] = TileType.Road;

            var queue = new RandomQueue<Position>();
            Position first = new Position(entranceX + 1, entranceY);
            queue.Enqueue(first);
            while(!queue.IsEmpty)
            {
                var currentPoint = queue.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    int newX = currentPoint.X + direction[i,0] * 2;
                    int newY = currentPoint.Y + direction[i,1] * 2;

                    if (InArea(newX, newY)
                            && !visited[newX,newY]
                            && Maze[newX,newY] == TileType.Road)
                    {
                        queue.Enqueue(new Position(newX, newY));
                        visited[newX,newY] = true;
                        Maze[currentPoint.X + direction[i,0], currentPoint.Y + direction[i,1]] = TileType.Road;
                    }
                }
            }
        }
        [Obsolete("Please use A* instead!")]
        public Stack<Position> Solve(int fromX, int fromY, int toX, int toY)
        {
            Stack<Position> result = null;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    solutionVisited[i,j] = false;
                    IsPath[i,j] = false;
                }
            }
            bool isSolved = false;
            Queue<Position> queue = new Queue<Position>();
            Position entrance = new Position(fromX, fromY);
            queue.Enqueue(entrance);
            visited[entrance.X, entrance.Y] = true;

            while (queue.Count != 0)
            {
                Position currentPosition = queue.Dequeue();

                if (currentPosition.X == toX && currentPosition.Y == toY)
                {
                    result = findPath(currentPosition);
                    isSolved = true;
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int newX = currentPosition.X + direction[i,0];
                    int newY = currentPosition.Y + direction[i,1];

                    if (InArea(newX, newY)
                            && !solutionVisited[newX, newY]
                            && Maze[newX, newY] == TileType.Road)
                    {
                        Position nextPos = new Position(newX, newY, currentPosition);
                        queue.Enqueue(nextPos);
                        solutionVisited[newX,newY] = true;
                    }
                }
            }
            if (!isSolved)
            {
                throw new Exception("Unsolvable Maze!");
            }
            return result;
        }
        private Stack<Position> findPath(Position destination)
        {
            var result = new Stack<Position>();
            Position currentPosition = destination;
            while (currentPosition != null)
            {
                result.Push(currentPosition);
                IsPath[currentPosition.X,currentPosition.Y] = true;
                currentPosition = currentPosition.From;
            }
            result.Pop();
            return result;
        }
    }
}
