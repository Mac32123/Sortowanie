using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sortowanie
{
    class RWBuffor          //R = 18B, B = 2kB
    {
        private const int blockSize = 9 * 100;
        public const int recordSize = 9;              //rozmiar rekordu na dysku (8 liter alfabetu łacińskiego i cyfr + znak nowej linii)
        private int currentBlockW = 0;          //blok zapisu aktualnie załadowany do pamięci
        private string currentFileW = "";        //plik do którego zapisany będzie blok aktualnie załadowany do pamięci
        private int currentBlockR = 0;          //blok z pliku odczytu aktualnie załadowany do pamięci
        private string currentFileR = "";        //plik odczytu z którego blok aktualnie jest załadowany do pamięci
        private string[] records = new string[blockSize / recordSize];
        private string file;
        private bool rw;
        private int which = 0;
        public int blockOperaions {set; get;}

        public RWBuffor(string file, bool rw)
        {
            this.file = file;
            this.rw = rw; //wczytywanie (true) lub zapisywanie (false)
            this.blockOperaions = 0;
        }

        private void changeBlockR (int block)
        {
            FileStream fs;
            byte[] buffer;
            currentBlockR = block;
            currentFileR = file;
            buffer = new byte[blockSize];
            fs = new FileStream(file, FileMode.Open);
            fs.Seek(block * blockSize, SeekOrigin.Begin);
            fs.Read(buffer, 0, blockSize);
            fs.Close();
            blockOperaions++;
            string str = Encoding.UTF8.GetString(buffer).Replace("\0", "").Replace("\r", "");
            string[] strarr = str.Split('\n');
            for (int i = 0; i < blockSize / recordSize; i++)
            {
                if (i < strarr.Length)
                {
                    records[i] = strarr[i];
                }
                else records[i] = null;
            }
        }

        private void changeBlockW (int block)
        {
            FileStream fs;
            byte[] buffer;
            if (currentFileW != "")          //zapisywanie pliku
            {
                string str = string.Join("\n", records);
                str = str.TrimEnd('\r', '\n');                 // jeżeli bufor nie jest pełen
                str += "\n";
                buffer = Encoding.UTF8.GetBytes(str);
                fs = new FileStream(currentFileW, FileMode.OpenOrCreate);
                fs.Seek(currentBlockW * blockSize, SeekOrigin.Begin);
                fs.Write(buffer, 0, Math.Min(buffer.Length, blockSize));
                fs.Close();
                blockOperaions++;
            }

            currentBlockW = block;
            currentFileW = file;
            records = new string[blockSize / recordSize];
        }

        private void changeBlock(int block, int rw)     // block = -1 oznacza ostatni blok w pliku lub nowy jeżeli w ostatnim nie ma miejsca, //jeżeli file = null to jedynie zapis
        {
            if (rw == 0)
            {
                changeBlockR(block);
            }
            else
            {
                changeBlockW(block);
            }
        }

        public void write(string rejestracja)
        {
            if (rw) return;
            int blockNumber = (which * recordSize / blockSize);
            int recordNumber = which % (blockSize / recordSize);
            if (file != currentFileW) changeBlock(blockNumber, 1);
            else if (blockNumber != currentBlockW) changeBlock(blockNumber, 1);
            which++;
            records[recordNumber] = rejestracja;
        }

        public string read ()
        {
            if (!rw) return "";
            int blockNumber = (which*recordSize / blockSize);
            int recordNumber = which % (blockSize / recordSize);
            string record;
            if (file != currentFileR) changeBlock(blockNumber, 0);
            else if (currentBlockR != blockNumber) changeBlock(blockNumber, 0);
            which++;
            record = records[recordNumber];
            if (record == null) record = "";
            else record = record.Trim('\n');
            return record;
        }

        public void saveFile()
        {
            changeBlock(-1, 1);
        }
    }
}
