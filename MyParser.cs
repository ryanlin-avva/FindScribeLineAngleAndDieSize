using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace CsGetTgs
{
    public class MyParser
    {
        NodeList[] lists = new NodeList[4];
        List<Point>[] ori_lists = new List<Point>[4];
        public NodeList GetList(int idx) { return lists[idx]; }
        public NodeList SelectedStart { get { return lists[selectedStart]; } }
        public NodeList SelectedEnd { get { return lists[selectedEnd]; } }
        public int DieWidth { get; set; }
        public int ScribeWidth { get; set; }
        int selectedStart { get; set; }
        int selectedEnd { get; set; }
        int upperbound = 0;
        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                lists[i].Clear();
            }
        }
        public MyParser(int scribe)
        {
            for (int i = 0; i < 4; i++)
            {
                lists[i] = new NodeList(i);
            }
            ScribeWidth = scribe;
        }
        public void Select()
        {
            int min1=0, min2=0;
            int score1 = CompScore(0, 2, 3, ref min1);
            int score2 = CompScore(1, 2, 3, ref min2);
            if (score1 <= score2)
            {
                selectedStart = 0; selectedEnd = min1;
            } else
            {
                selectedStart = 1; selectedEnd = min2;
            }
            Console.WriteLine("Selected lists are :" + selectedStart.ToString()
                            + " And " + selectedEnd.ToString());
        }
        int CompScore(int idx_target, int idx1, int idx2, ref int idx_selected)
        {
            int s1 = lists[idx_target].CompScore(lists[idx1]);
            int s2 = lists[idx_target].CompScore(lists[idx2]);
            string str = string.Format("Compare List {0} with {1},{2} Get Score {3} {4}"
                                    , idx_target, idx1, idx2, s1, s2);
            Console.WriteLine(str);
            if (s1 <= s2)
            {
                idx_selected = idx1;
                return s1;
            }
            else
            {
                idx_selected = idx2;
                return s2;
            }
        }
        public void Build(List<Point> list, int coor
                        , List<Point> list1, int coor1
                        , List<Point> list2, int coor2
                        , List<Point> list3, int coor3, bool isHor)
        {
            ori_lists[0] = list;
            ori_lists[1] = list1;
            ori_lists[2] = list2;
            ori_lists[3] = list3;

            BuildOne(list, 0, coor, isHor);
            BuildOne(list1, 1, coor1, isHor);
            BuildOne(list2, 2, coor2, isHor);
            BuildOne(list3, 3, coor3, isHor);
            int dieWidth = 0;
            int scribeWidth = 0;
            int diecnt = lists.Length;
            int scribecnt = lists.Length;
            for (int i = 0; i < lists.Length; i++)
            {
                int die=0, scribe=0;
                lists[i].Median(ref die, ref scribe);
                if (die > 0) dieWidth += die;
                else diecnt--;
                if (scribe > 0) scribeWidth += scribe;
                else scribecnt--;

                Console.WriteLine(lists[i]);
            }
            DieWidth = (diecnt > 0) ? dieWidth / diecnt : 0;
            ScribeWidth = (scribecnt > 0) ? scribeWidth / scribecnt : 0;
        }
        void BuildOne(List<Point> list, int no, int coor, bool isHor)
        {
            if (list.Count == 0) return;
            lists[no].AssoicateCoor = coor;
            int start = isHor ? list[0].Y : list[0].X;
            int prev = start;
            int now = start;
            int withLine = (int)(0.5 * ScribeWidth);
            int widthLowerBound = (int)(0.4 * ScribeWidth);
            int widthUpperBound = (int)(1.5 * ScribeWidth);
            for (int i = 1; i < list.Count; i++)
            {
                now = isHor ? list[i].Y : list[i].X;
                if (now - prev < withLine) //origin value:7
                {
                    prev = now;
                    continue;
                }
                int w = prev - start;
                if (w > widthLowerBound //origin value:5
                    && w < widthUpperBound)
                {
                    lists[no].Add(start, prev);
                    start = now;
                }
                prev = now;
            }
            lists[no].Add(start, now);
            lists[no].CompMin();
            if (upperbound < now) upperbound = now;
        }
    }
    public class NodeList
    {
        int seq_no;
        public List<Node> Target = new List<Node>();
        public double Average { get; set; }
        public double MyMin { get; set; }
        public int Count { get { return Target.Count; } }
        public int AssoicateCoor { get; set; }
        public int GetCenter(int node_idx) { return Target[node_idx].Mid; }
        public void Median(ref int dieWidth, ref int scribeWidth) {
            dieWidth = 0;
            scribeWidth = 0;
            if (Target.Count == 0) return;
            List<int> die = new List<int>();
            List<int> scribe = new List<int>();
            scribe.Add(Target[0].Length);
            for (int i=1; i<Target.Count; i++)
            {
                die.Add(Target[i].Mid - Target[i - 1].Mid);
                scribe.Add(Target[i].Length);
            }
            if (die.Count > 0)
            {
                die.Sort();
                dieWidth = die.ElementAt(die.Count / 2);
            } 
            if (scribe.Count > 0)
            {
                scribe.Sort();
                scribeWidth = scribe.ElementAt(scribe.Count / 2);
            }
        }
        public override string ToString()
        {
            string str = "List " + seq_no.ToString() 
                       + " ("+AssoicateCoor.ToString()+"):\n";
            foreach (var t in Target)
            {
                str += t.ToString() + "\n";
            }
            return str;
        }
        public NodeList(int no)
        {
            seq_no = no;
        }
        public void Clear()
        {
            Target.Clear();
        }
        public int CompScore(NodeList a)
        {
            int i0 = 0;
            int i1 = 0;
            List<Node> other = a.Target;
            int diff = (int)Math.Min(MyMin, a.MyMin) * 3;
            int score = 0;
            while (i0 < Target.Count && i1 < other.Count)
            {
                if (Math.Abs(Target[i0].StartPos - other[i1].StartPos) < diff)
                {
                    score += Math.Abs(Target[i0].Length - other[i1].Length);
                    i0++; i1++;
                }
                else
                {
                    if (Target[i0].StartPos < other[i1].StartPos)
                    {
                        score += Target[i0].Length;
                        i0++;
                    }
                    else
                    {
                        score += other[i1].Length;
                        i1++;
                    }
                }
            }
            while (i0 < Target.Count)
            {
                score += Target[i0].Length;
                i0++;
            }
            while (i1 < other.Count)
            {
                score += other[i1].Length;
                i1++;
            }
            return score;
        }
        public void CompAverage()
        {
            Average = Target.Select(x => x.Length).DefaultIfEmpty(0).Average();
        }
        public void CompMin()
        {
            MyMin = Target.Select(x => x.Length).DefaultIfEmpty(0).Min();
        }

        public void Add(int s, int e)
        {
            Node n = new Node(s, e);
            Target.Add(n);
        }
    }
    public class Node
    {
        public int Length { get; set; }
        public int StartPos { get; set; }
        public int EndPos { get; set; }
        public int Mid { get; set; }
        public Node(int s, int e)
        {
            StartPos = s;
            EndPos = e;
            Length = e - s;
            Mid = s + Length / 2;
        }
        public override string ToString()
        {
            return "Node: " + StartPos.ToString() + "--" + EndPos.ToString();
        }
    }
}
