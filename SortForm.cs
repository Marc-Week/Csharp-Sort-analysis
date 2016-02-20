using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SortAnalysis
{
    public partial class SortForm : Form
    {
        enum setType { RAND=0, REV, FEWU, NEAR }//types of sets

        public System.Random rnd = new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF);//Random seed
        public int[] nSizes = { 1000, 5000 }; //, 10000, 20000, 50000 };// 100000, 200000, 500000, 1000000};// n(array siz) values to be tested
        public List<List<int[]>> allsets = Enumerable.Range(0,4).Select((i) => new List<int[]>()).ToList();//set of all sets to be tested
        public String CSV = "";//raw print out of tests

        public SortForm()
        {
            InitializeComponent();
            foreach (int size in nSizes)
                CSV += size + ",";
            CSV += "\n";


           
            

            genSets();//create the sets



            rnd = new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF);


            //Runs each sort out against randomized types
            runAnalysis(insertionSort, "Insertion Sort");
            runAnalysis(mergeSort, "Merge Sort");
            runAnalysis(selectSort, "Selection Sort");
            runAnalysis(quickSort, "Quick Sort");
            runAnalysis(bucketSort, "Bucket Sort");
            runAnalysis(mergeSelectionSort, "Merge-to-Selection Sort");
            runAnalysis(cSharpSort, "C# linQ Sort");

            //runs each sort against eachtoer
            runHead2Head();

            //prints the CSV
            System.IO.File.WriteAllText(@"SortAnalysis.CSV", CSV);

        }


       


        /// <summary>
        /// Creates all the sets and adds  them to the list
        /// </summary>
        private void genSets()
        {
            
            foreach (int n in nSizes)
            {
                allsets[(int)setType.RAND].Add(createRand(n));
                allsets[(int)setType.REV].Add(createReverse(n));
                allsets[(int)setType.FEWU].Add(createFewUniques(n));
                allsets[(int)setType.NEAR].Add(createNearlySorted(n));
            }

        }


        //completely random 
        private int[] createRand(int n)
        {
            return Enumerable.Range(0, n).ToArray().OrderBy(r => rnd.Next()).ToArray();

        }

        //reverse order
        private int[] createReverse(int n)
        {
            return Enumerable.Range(0, n).Reverse().ToArray();
        }

        //Semi orderd list
        private int[] createNearlySorted(int n)
        {
            int[] set = Enumerable.Range(0, n).ToArray();

            int sub = (int)(n * .1);

            for (int i = 0; i < sub; ++i)
            {
                int j = rnd.Next(0, n);
                int k = rnd.Next(0, n);
                int t = set[j];
                set[j] = set[k];
                set[k] = t;
            }

            return set;
        }


        //Random but large number of duplicates
        private int[] createFewUniques(int n)
        {
            int[] set = new int[n];
            int sub = (int)(n * .1);
            int j = 0;
            set[0] = j;
            for (int i = 1; i < n; ++i)
            {
                if (i % sub == 0) j = i;

                set[i] = j;
            }
            return set.OrderBy(r => rnd.Next()).ToArray(); ;
        }


        //selection sort
        private int[] selectSort(int[] set)
        {
            for (int i = 0; i < set.Length - 1; ++i)
            {
                int smallest = i;
                for (int j = i + 1; j < set.Length; ++j)
                {
                    if (set[j] < set[smallest])
                        smallest = j;
                }
                int temp = set[i];
                set[i] = set[smallest];
                set[smallest] = temp;
            }

            return set;
        }


        //insertion sort
       private int[] insertionSort(int[] set)
        {
            for (int i = 1; i < set.Length; ++i)
            {
                int j = i;
                while (j > 0 && set[j] < set[j - 1])
                {
                    int temp = set[j];
                    set[j] = set[j - 1];
                    set[j - 1] = temp;
                    --j;
                }
            }

            return set;
        }


        //insertion sort for list
        private List<int> insertionSort(List<int> set)
        {
            for (int i = 1; i < set.Count; ++i)
            {
                int j = i;
                while (j > 0 && set[j] < set[j - 1])
                {
                    int temp = set[j];
                    set[j] = set[j - 1];
                    set[j - 1] = temp;
                    --j;
                }
            }

            return set;
        }

        //merge sort
        private int[] mergeSort(int[] set)
        {
            if (set.Length <= 1) return set;

            int N = set.Length / 2;
            int Np = set.Length - (N);
            int[] firstHalf = new int[N];
            int[] secondHalf = new int[Np];
            Array.Copy(set, 0, firstHalf, 0, N);
            Array.Copy(set, N, secondHalf, 0, Np);
            firstHalf = mergeSort(firstHalf);
            secondHalf = mergeSort(secondHalf);
            int i = 0;
            int j = 0;
            int k = 0;
            while (j < firstHalf.Length && k < secondHalf.Length)
            {
                if (firstHalf[j] < secondHalf[k])
                {
                    set[i] = firstHalf[j];
                    ++j;
                }
                else
                {
                    set[i] = secondHalf[k];
                    ++k;
                }
                ++i;
            }
            while (j < firstHalf.Length)
            {
                set[i] = firstHalf[j];
                ++j;
                ++i;
            }
            while (k < secondHalf.Length)
            {
                set[i] = secondHalf[k];
                ++k;
                ++i;
            }
            return set;
        }

        //quick sort wrapper
        private int[] quickSort(int[] set)
        {
           quickSort(ref set, 0, (set.Length - 1));
           return set;
        }

        //quck sort
        private void quickSort(ref int[] set, int start, int end)
        {
            int s = start;
            int e = end;

            if(end - start >= 1)
            {
                int pivot = set[rnd.Next(start, end)];


                while(e > s)
                {
                    while (set[s] <= pivot && s <= end && e > s)  
                        s++;                                    
                    while (set[e] > pivot && e >= start && e >= s) 
                        e--;                                       
                    if (e > s)                                       
                        swap( ref set, s, e);                     
                }
                swap(ref set, start, e);

                quickSort(ref set, start, e - 1);
                quickSort(ref set, s + 1, end);

            }
            else
            {
                return;
            }

        }
       

        //bucke sort
        private int[] bucketSort(int[] set)
        {
            int n = set.Length;
            int min = set.Min();
            List<int>[] buckets = Enumerable.Range(0, n).Select((i) => new List<int>()).ToArray();
            for (int i = 1; i < n; ++i)
            {
                buckets[set[i] - min].Add(set[i]);
            }
            List<int> final = new List<int>();
            foreach (List<int> l in buckets)
            {
                List<int> a = insertionSort(l);

                final = final.Concat(a).ToList();

            }


            return final.ToArray();
        }

        //my own sort, merges intil smaller than 15 units then selection sort
        private int[] mergeSelectionSort(int[] set)
        {
            if (set.Length >= 15) return selectSort(set);

            int N = set.Length / 2;
            int Np = set.Length - (N);
            int[] firstHalf = new int[N];
            int[] secondHalf = new int[Np];
            Array.Copy(set, 0, firstHalf, 0, N);
            Array.Copy(set, N, secondHalf, 0, Np);
            firstHalf = mergeSort(firstHalf);
            secondHalf = mergeSort(secondHalf);
            int i = 0;
            int j = 0;
            int k = 0;
            while (j < firstHalf.Length && k < secondHalf.Length)
            {
                if (firstHalf[j] < secondHalf[k])
                {
                    set[i] = firstHalf[j];
                    ++j;
                }
                else
                {
                    set[i] = secondHalf[k];
                    ++k;
                }
                ++i;
            }
            while (j < firstHalf.Length)
            {
                set[i] = firstHalf[j];
                ++j;
                ++i;
            }
            while (k < secondHalf.Length)
            {
                set[i] = secondHalf[k];
                ++k;
                ++i;
            }
            return set;

        }

        //wrapper for the c# libreay sort
        private int[] cSharpSort(int[] set)
        {

           Array.Sort(set);

            return set;
        }



        
        /// <summary>
        /// Test the function time based on proccessing time
        /// </summary>
        /// <param name="set">Set to sort</param>
        /// <param name="f">Function to test</param>
        /// <returns>Time in millecsecconds </returns>
        private double evalFunc(int[] set, Func<int[], int[]> f)
        {
            int[] copyset = new int[set.Length];
            set.CopyTo(copyset, 0);



            TimeSpan start = Process.GetCurrentProcess().TotalProcessorTime;
            f(copyset);
            TimeSpan end = Process.GetCurrentProcess().TotalProcessorTime;

            TimeSpan t = end - start;

            return t.TotalMilliseconds;
        }



        /// <summary>
        /// Tests the Function agauins a set of different sized arrays
        /// </summary>
        /// <param name="sets">list of arrays to sort</param>
        /// <param name="f">fuction to test</param>
        /// <param name="sorttype">what sort type to sort</param>
        /// <param name="chart">what chart to update</param>
        private void testAlgorithm(List<int []> sets, Func<int[], int[]> f, int sorttype, Chart chart)
        {
            chart.Series[sorttype].Points.Clear();
            foreach (int[] set in sets)
            {
                double time = evalFunc(set, f);
                chart.Series[sorttype].Points.AddXY(set.Length, time);
                CSV += time + ",";
                Thread.Sleep(200);
            }
            CSV += "\n";
        }


        /// <summary>
        /// Runs analsysis on a single function on all type of randomized lists
        /// </summary>
        /// <param name="f">fucntion to test</param>
        /// <param name="name">name of function</param>
        private void runAnalysis(Func<int[], int[]> f, String name)
        {
            chart1.Titles[0].Text = name;
            CSV += name + "\n";

            for (int i = 0; i < 4; ++i)
            {
                testAlgorithm(allsets[i], f, i, chart1);

                
            }

            chart1.Update();


            chart1.SaveImage(name + ".png", ChartImageFormat.Png);
            
        }


        /// <summary>
        /// runs all sorts against eachother on each sort type
        /// </summary>
        private void runHead2Head()
        {

            for (int i = 0; i < 4; ++i)
            {
                chart2.Titles[0].Text = chart1.Series[i].Name; 
                CSV += "HEAD TO HEAD- "+ i + "\n";
                CSV += "Insertion Sort\n";
                testAlgorithm(allsets[i], insertionSort, 0, chart2);
                CSV += "Merge Sort\n";
                testAlgorithm(allsets[i], mergeSort, 1 , chart2);
                CSV += "Selection Sort\n";
                testAlgorithm(allsets[i],selectSort, 2 , chart2);
                CSV += "Quick Sort\n";
                testAlgorithm(allsets[i], quickSort, 3, chart2);
                CSV += "Bucket Sort\n";
                testAlgorithm(allsets[i], bucketSort, 4, chart2);
                CSV += "Merge-to-Selection Sort\n";
                testAlgorithm(allsets[i], mergeSelectionSort, 5, chart2);
                CSV += "C# linQ Sort\n";
                testAlgorithm(allsets[i], cSharpSort, 6, chart2);

                chart2.SaveImage(chart1.Series[i].Name + ".png", ChartImageFormat.Png);
            }
        }


        /// <summary>
        /// Simple helper swap function
        /// </summary>
        /// <param name="set"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void swap(ref int[] set, int a, int b)
        {
            int temp = set[a];
            set[a] = set[b];
            set[b] = temp;
        }
    }
}
