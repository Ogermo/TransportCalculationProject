using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace TransportCalculationProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "Доставки по складам.xlsx ";

            Application excel;
            Workbook wb;
            Worksheet excelSheet;
            try
            {
                excel = new Application();
                wb = excel.Workbooks.Open(Path.GetFullPath(path));
                excelSheet = wb.ActiveSheet;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can not open Excel table: {e}");
                Console.ReadKey();
                return;
            }


            int totalColumns = excelSheet.UsedRange.Columns.Count;
            int totalRows = excelSheet.UsedRange.Rows.Count;

            //initiate graph
            List<Tuple<int, int>>[] graph = new List<Tuple<int, int>>[2];
            for (int i = 0; i < graph.Length; i++)
            {
                graph[i] = new List<Tuple<int, int>>();
            }

            for (int i = 2; i <= totalRows; i++)
            {
                try
                {
                    int pA = (int) excelSheet.Cells[i, 1].Value;
                    int pB = (int) excelSheet.Cells[i, 2].Value;
                    int weight = (int)excelSheet.Cells[i, 3].Value;

                    //graph changing dinamically, because we don't know how big it's must be

                    if (Math.Max(pA, pB) + 1 > graph.Length) 
                    {
                        int oldLength = graph.Length;
                        Array.Resize(ref graph, Math.Max(pA, pB) + 1);
                        for (int j = oldLength; j < graph.Length; j++)
                        {
                            graph[j] = new List<Tuple<int, int>>();
                        }
                    }

                    graph[pA].Add(new Tuple<int, int>(pB, weight));
                    graph[pB].Add(new Tuple<int, int>(pA, weight)); //because our graph is not oriented
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
                {
                    Console.WriteLine("Catched empty string, please check your table");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unpredicted exception {e}");
                    Console.ReadKey();
                    return;
                }
            }

            //check if input from console is incorrect or not existing, if it is - ask for new one
            int start;
            int end;

            if (!((args.Length == 2) && 
                (int.TryParse(args[0], out start)) && 
                (int.TryParse(args[1], out end)) &&
                (Math.Max(start,end) < graph.Length) &&
                (Math.Min(start, end) > 0)))
            {
                string[] strList = { };

                do
                {
                    Console.Clear();
                    Console.WriteLine("Input is incorrect or not existing, please try again\n" +
                        "Write it down as two numbers separated with space: ");
                    string input = Console.ReadLine();
                    strList = input.Split(' ');

                } while (!((strList.Length == 2) && //check new input the same way
                (int.TryParse(strList[0], out start)) &&
                (int.TryParse(strList[1], out end)) &&
                (Math.Max(start, end) < graph.Length) &&
                (Math.Min(start, end) > 0)));
            }

            //List<int> is path and int is total price
            Tuple<List<int>, int> answer;

            try
            {
                answer = Dijkstra.Execute(graph, start, end);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to complete Dijkstra alghoritm: {e}");
                Console.ReadKey();
                return;
            }

            bool first = true;
            foreach (int c in answer.Item1)
            {
                if (first)
                {
                    Console.Write($"Path: {c} ");
                    first = false;
                }
                else
                { 
                Console.Write($"-> {c} ");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Price: {answer.Item2}");
            Console.ReadKey();

            wb.Close();
        }
    }
}
