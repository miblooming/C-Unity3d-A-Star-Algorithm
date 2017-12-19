using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class AStar4Direction
    {
        private static int target;
        private static int w;
        private static int h;

        private static Comparison<Point> comparison = new Comparison<Point>
        ((Point a, Point b) =>
        {
            return (a.cost + dist(a.p, target)) - (b.cost + dist(b.p, target));
        });

        private static int dist(int p, int target)
        {
            return Math.Abs(target%w - p%w) + Math.Abs(target/w - p/w);
        }

        private class Point{
            public Point(int p)
            {
                this.p = p;
                this.cost = int.MaxValue;
            }
            public Point partent;
            public int p;
            public int cost;
        }

        //左上是原点，序号从0开始，先向右增加，再向下增加
        public static int[] Search(int[] map,int w,int h,int start,int target)
        {
            if(map.Length != w * h)
            {
                throw (new Exception("地图数据与长宽不匹配"));
            }

            AStar4Direction.target = target;
            AStar4Direction.w = w;
            AStar4Direction.h = h;

            List<Point> open = new List<Point>();
            List<int> close = new List<int>();
            Point[] pMap = new Point[map.Length];
            for(int i = 0; i < map.Length; i++)
            {
                pMap[i] = new Point(i);
                if(map[i] != 0)
                {
                    close.Add(i);
                }
            }

            if (map[start] == 0)
            {
                open.Add(pMap[start]);
            }
            open.Sort(comparison);

            while (open.Count != 0)
            {
                //寻找当前格
                int cur = open[0].p;

                if(cur == target)
                {
                    return create_path(pMap[cur]);
                }

                open.RemoveAt(0);
                close.Add(cur);

                //寻找周围的4格子
                List<int> neighbor = new List<int>();
                if (cur % w != 0) { neighbor.Add(cur - 1); }
                if ((cur + 1) % w != 0) { neighbor.Add(cur + 1); }
                if ((cur - w) >= 0) { neighbor.Add(cur - w); }
                if ((cur + w) < map.Length){ neighbor.Add(cur + w); }

                foreach(int p in neighbor)
                {
                    if (!close.Contains(p))
                    {
                        open.Add(pMap[p]);
                        int cost = pMap[cur].cost + 1;
                        if (pMap[p].cost > cost)
                        {
                            pMap[p].cost = cost;
                            pMap[p].partent = pMap[cur];
                        }
                    }
                }
                open.Sort(comparison);


            }

            return null;
        }

        private static int[] create_path(Point end)
        {
            List<int> list = new List<int>();
            
            while (end != null)
            {
                list.Add(end.p);
                end = end.partent;
            }
            list.Reverse();
            return list.ToArray();
        }
    }
}
