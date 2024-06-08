using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fare;

namespace Sortowanie
{
    class Program
    {

        static string generateFile(int quantity)
        {
            string fileName = "generated.txt";
            string city = File.ReadAllText("tablice.txt");
            city = "(" + city;
            city = city.Replace("\r\n", ")|(");
            city += ")";
            string regex = "(" + city + ")[A-Z0-9]{5}";
            string file = "";
            string record;
            var xeger = new Xeger(regex);
            for(int i = 0; i < quantity; i++)
            {
                record = xeger.Generate();
                if(record.Length == 7)
                {
                    record += " ";
                }
                file += record + "\n";
            }
            File.WriteAllText(fileName, file);
            return fileName;
        }

        static string generateFile(List<string> records)
        {
            string fileName = "input.txt";
            string file = "";
            foreach(string record in records)
            {
                file += record + "\n";
            }
            File.WriteAllText(fileName, file);
            return fileName;
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(RWBuffor.read(3, "plik.txt"));
            //Console.ReadLine();
            //RWBuffor.write("GTC02165", 9, "plik.txt");
            string file = "";
            int end = 0;
            while (end == 0)
            {
                Console.WriteLine("Aby wygenerować plik testowy wpisz 0. Aby wpisać rekordy ręcznie wpisz 1. Aby skorzystać z gotowego pliku wpisz 2.");
                string choise = Console.ReadLine();
                switch (choise)
                {
                    case "0":
                        Console.WriteLine("Wybrałeś generowanie pliku - podaj ilość rekordów do wygenerowania");
                        int quantity = int.Parse(Console.ReadLine());
                        file = generateFile(quantity);
                        end = 1;
                        break;
                    case "1":
                        Console.WriteLine("Wybrałeś wpisywanie rekordów ręcznie. Aby zakończyć wpisywanie rekordów wpisz 0");
                        string input = "";
                        List<string> records = new List<string>();
                        while(input != "0")
                        {
                            input = Console.ReadLine();
                            if (input != "0")
                            {
                                records.Add(input);
                            }
                        }
                        file = generateFile(records);
                        end = 1;
                        break;
                    case "2":
                        Console.WriteLine("Wybrałeś opcję skorzystania z gotowego pliku. Podaj ścieżkę do pliku");
                        file = Console.ReadLine();
                        while (!File.Exists(file))
                        {
                            Console.WriteLine("Podana ścieżka jest nieprawidłowa. Podaj prawidłową ścieżkę.");
                            file = Console.ReadLine();
                        }
                        end = 1;
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowa liczba. Wybierz jeszcze raz.");
                        break;

                }
            }
            while (true) {
                Console.WriteLine("Wpisz 0 aby posortować rekordy pokazując tylko stan początkowy i końcowy. Wpisz 1 aby posortować rekordy pokazując wszstkie stany pomiędzy");
                string choise = Console.ReadLine();
                if (choise == "0")
                {
                    PolSort.sort(file, 0);
                    break;
                }else if (choise == "1")
                {
                    PolSort.sort(file, 1);
                    break;
                }else
                {
                    Console.WriteLine("Nieprawidłowy argument. Wybierz jeszcze raz.");
                }

            }
            FileInfo fileInfo = new FileInfo(file);
            //int lenght = (int) (fileInfo.Length / RWBuffor.recordSize) ;
            //for (int i = 0; i <lenght; i++)
            //{
            //    Console.WriteLine(RWBuffor.read(i, file));
                //RWBuffor.write(RWBuffor.read(i, "plik.txt"), i, "nowy.txt");
            //}
            Console.ReadLine();
            //RWBuffor.closeBuffer();
        }
    }
}
