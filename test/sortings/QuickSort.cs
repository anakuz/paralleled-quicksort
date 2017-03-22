using System;
using System.Threading;

namespace test
{
    public class QuickSort
    {
        public static bool IsLoggingEnabled { get; set; }

        private static int Partition<T>(T[] arr, int low, int high) where T : IComparable<T>
        {
            int pivotPos = (high + low) / 2;
            T pivot = arr[pivotPos];
            Helper.Swap(arr, low, pivotPos);

            int left = low;
            for (int i = low + 1; i <= high; i++)
            {
                if (arr[i].CompareTo(pivot) < 0)
                {
                    left++;
                    Helper.Swap(arr, i, left);
                }
            }

            Helper.Swap(arr, low, left);
            return left;
        }

        public static void Sort<T>(T[] arr, int left, int right) where T : IComparable<T>
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right);
                Sort(arr, left, pivot - 1);
                Sort(arr, pivot + 1, right);
                if (IsLoggingEnabled)
                {
                    Console.WriteLine("[{0}..{1}]", left, right);
                }
            }
        }

        public void SortParallel<T>(T[] array2) where T : IComparable<T>
        {
            QuickSortParallel.Sort_(array2);
        }
    }

    public class QuickSortParallel
    {
        private readonly Semaphore _semaphore;
        private static int _syncCounter;

        public QuickSortParallel(int maxThreadsCount = 7)
        {
            _semaphore = new Semaphore(maxThreadsCount, maxThreadsCount);
        }

        public void Sort<T>(T[] array2) where T : IComparable<T>
        {
            _syncCounter = 0;
            Sort(array2, 0, array2.Length - 1);
            while (_syncCounter != 0)
            {
                Thread.Sleep(1);
            }
        }

        private void Sort<T>(T[] array2, int left, int right) where T : IComparable<T>
        {
            if (left >= right)
                return;
            _semaphore.WaitOne();

            Helper.Swap(array2, left, (left + right) / 2); //median pivot
            var last = left;
            for (int current = left + 1; current <= right; ++current)
            {
                //CompareTo, compares current array index value with
                if (array2[current].CompareTo(array2[left]) < 0)
                {
                    ++last;
                    Helper.Swap(array2, last, current);
                }
            }

            Helper.Swap(array2, left, last);
            //Recursive
            //Executes each of the provided actions in parallel.
            Interlocked.Add(ref _syncCounter, 2);
            ThreadPool.QueueUserWorkItem(x =>
            {
                Sort(array2, left, last - 1);
                Interlocked.Decrement(ref _syncCounter);
            });
            ThreadPool.QueueUserWorkItem(x =>
            {
                Sort(array2, last + 1, right);
                Interlocked.Decrement(ref _syncCounter);
            });
            var prevCount = _semaphore.Release();
            if (QuickSort.IsLoggingEnabled)
            {
                Console.WriteLine("{0}, [{1}..{2}]", prevCount + 1, left, right);
            }
        }

        public static void Sort_<T>(T[] array2, int maxThreadsCount = 7) where T : IComparable<T>
        {
            new QuickSortParallel(maxThreadsCount).Sort(array2);
        }
    }
}
