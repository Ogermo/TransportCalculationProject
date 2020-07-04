using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCalculationProject
{
    class Dijkstra
    {
        //Algorthitm return <path to end,value>
        static public Tuple<List<int>, int> Execute(List<Tuple<int, int>>[] graph, int start, int end)
        {
            int[] dist = new int[graph.Length];
            for (int i = 0; i < dist.Length; ++i)
            {
                dist[i] = int.MaxValue;
            }

            int[] parent = new int[graph.Length];

            dist[start] = 0;

            bool[] mark = new bool[graph.Length];

            for (int i = 1; i < graph.Length; ++i)
            {
                int v = -1;
                for (int j = 1; j < graph.Length; ++j)
                    if (!mark[j] && (v == -1 || dist[j] < dist[v]))
                        v = j;
                if (dist[v] == int.MaxValue)
                    break;
                mark[v] = true;

                for (int j = 0; j < graph[v].Count; ++j)
                {
                    int to = graph[v][j].Item1,
                        len = graph[v][j].Item2;
                    if (dist[v] + len < dist[to])
                    {
                        dist[to] = dist[v] + len;
                        parent[to] = v;
                    }
                }
            }
            List<int> path = new List<int>();
            for (int point = end; point != start; point = parent[point])
            {
                path.Add(point);
            }
            path.Add(start);
            path.Reverse();
            Tuple<List<int>, int> answer = new Tuple<List<int>, int>(path, dist[end]);
            return answer;
        }
    }
}
