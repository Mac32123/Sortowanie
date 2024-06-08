using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortowanie
{
    class PolSort
    {
        private static readonly string[] tapes = { "tape0", "tape1", "tape2" };
        private static int phases = 0;
        private static int blockOperations = 0;
        private static bool merge(int mode)
        {
            int runNum0 = 0, runNum1 = 0;
            bool moreRunsThanOne = false;
            string prevRec0 = "", prevRec1 = "";
            var read0 = new RWBuffor(tapes[0], true);
            var read1 = new RWBuffor(tapes[1], true);
            var write = new RWBuffor(tapes[2], false);
            string rec0, rec1;
            rec0 = read0.read();
            rec1 = read1.read();
            while(rec0.Length != 0 || rec1.Length != 0) {
                if (runNum0 == runNum1 || rec0.Length == 0 || rec1.Length == 0)
                {
                    if (rec0.Length == 0)
                    {
                        write.write(rec1);
                        prevRec1 = rec1;
                        rec1 = read1.read();
                    }
                    else if (rec1.Length == 0)
                    {
                        write.write(rec0);
                        prevRec0 = rec0;
                        rec0 = read0.read();
                    }
                    else if (string.Compare(rec0, rec1) <= 0)
                    {
                        write.write(rec0);
                        prevRec0 = rec0;
                        rec0 = read0.read();
                    }
                    else
                    {
                        write.write(rec1);
                        prevRec1 = rec1;
                        rec1 = read1.read();
                    }
                }
                else if ((runNum0 == 1 && runNum1 == 0) || (runNum0 == 2 && runNum1 == 1) || (runNum0 == 0 && runNum1 == 2))
                {
                    write.write(rec1);
                    prevRec1 = rec1;
                    rec1 = read1.read();
                }
                else
                {
                    write.write(rec0);
                    prevRec0 = rec0;
                    rec0 = read0.read();
                }
                if (string.Compare(rec0, prevRec0) < 0)
                {
                    runNum0++;
                    runNum0 = runNum0 % 3;
                    prevRec0 = "";
                }
                if (string.Compare(rec1, prevRec1) < 0)
                {
                    runNum1++;
                    runNum1 = runNum1 % 3;
                    prevRec1 = "";
                }
                if(runNum0 == 2 || runNum1 == 2)
                {
                    moreRunsThanOne = true;
                }
            }
            write.saveFile();
            blockOperations += read0.blockOperaions + read1.blockOperaions + write.blockOperaions;
            File.Delete("tape0");
            File.Delete("tape1");
            if(mode == 1)
            {
                show("tape2");
            }
            if (!moreRunsThanOne) return true;
            else return false;
        }

        private static void divide(string file)
        {
            bool inRun = true;
            string record = "";
            string prevRec = "";
            RWBuffor read = new RWBuffor(file, true);
            RWBuffor write0 = new RWBuffor(tapes[0], false);
            RWBuffor write1 = new RWBuffor(tapes[1], false);
            do
            {
                record = read.read();
                if (string.Compare(record, prevRec) < 0)
                {
                    inRun = !inRun;
                }
                prevRec = record;
                if (inRun)
                {
                    write0.write(record);
                }
                else
                {
                    write1.write(record);
                }
            } while (record.Length != 0);
            write0.saveFile();
            write1.saveFile();
            blockOperations += write0.blockOperaions + write1.blockOperaions + read.blockOperaions;
        }

        private static void show(string file)
        {
            string record;
            var reader = new RWBuffor(file, true);
            do
            {
                record = reader.read();
                Console.WriteLine(record);
            } while (record.Length != 0);
            Console.WriteLine();
        }

        public static void sort(string file, int mode)        //mode 0 oznacza brak wypisywania stanu pliku w każdej fazie, 1 oznacza wypisywanie
        {
            Console.WriteLine("File before sorting: ");
            show(file);
            divide(file);
            phases++;
            while (!merge(mode)) {
                divide(tapes[2]);
                phases++;        
            }
            File.Copy(tapes[2], "sorted.txt", true);
            File.Delete(tapes[2]);
            Console.WriteLine("Sorted file:");
            show("sorted.txt");
            Console.WriteLine($"Number of phases: {phases}");
            Console.WriteLine($"Number of block operations: {blockOperations}");
        }
    }
}
