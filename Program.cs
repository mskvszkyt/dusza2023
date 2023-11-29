// nullReference
namespace verseny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Jatek> jatekok;
            if (File.Exists("jatekok.txt"))
            {
                string jatekokSzoveg = File.ReadAllText("jatekok.txt");
                string eredmenyekSzoveg = File.Exists("eredmenyek.txt") ? File.ReadAllText("eredmenyek.txt") : null;
                jatekok = Jatek.Szovegbol(jatekokSzoveg, eredmenyekSzoveg);
            }
            else
            {
                jatekok = new List<Jatek>();
            }


            List<Fogadas> fogadasok = File.Exists("fogadasok.txt") ? File.ReadAllLines("fogadasok.txt").Select(x => new Fogadas(x)).ToList() : new List<Fogadas>();

            int menupontKezdoSor = 4;
            int menupont = 0;
            string jatekNev = "Fogadási játék";
            int jatekNevMargin = 1;
            int jatekNevPadding = 3;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int i = 0; i < jatekNevMargin; i++)
            {
                Console.WriteLine(new string(' ', jatekNev.Length + jatekNevPadding * 2));
            }
            Console.WriteLine($"{new string(' ', jatekNevPadding)}{jatekNev}{new string(' ', jatekNevPadding)}");
            for (int i = 0; i < jatekNevMargin; i++)
            {
                Console.WriteLine(new string(' ', jatekNev.Length + jatekNevPadding * 2));
            }
            string[] menupontok = { "Játék létrehozása", "Fogadás leadása", "Játék lezárása", "Lekérdezések", "Kilépés" };
            Menuvalaszthato menuvalaszthato = new Menuvalaszthato(menupontok);
            while (true)
            {
                string valasztott = menuvalaszthato.Indit((menupontKezdoSor + jatekNevMargin * 2) + 1);
                Console.Clear();
                switch (valasztott)
                {
                    case "Játék létrehozása":
                        UjJatek(jatekok);
                        break;
                    case "Fogadás leadása":
                        FogadasLeadas(jatekok, fogadasok);
                        break;
                    case "Játék lezárása":
                        Lezaras(jatekok, fogadasok);
                        break;
                    case "Lekérdezések":
                        Lekerdezesek(jatekok, fogadasok);
                        break;
                    case "Kilépés":
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }
        static void Lekerdezesek(List<Jatek> jatekok, List<Fogadas> fogadasok)
        {
            string[] menupontok = { "Ranglista", "Játék statisztika" };
            Menuvalaszthato menuvalaszthato = new Menuvalaszthato(menupontok);
            string ertek = menuvalaszthato.Indit(0);
            switch (ertek)
            {
                case "Ranglista":
                    Dictionary<string, double> szamitott = Pontszamitas(jatekok, fogadasok);
                    var rendezett = szamitott.OrderByDescending(x => x.Value).ToList();
                    for (int i = 0; i < rendezett.Count(); i++)
                    {
                        Console.WriteLine($"{i + 1}. {rendezett[i].Key}");
                    }

                    break;
                case "Játék statisztika":
                    var jatekFogadasok = fogadasok.GroupBy(x => x.JatekMegnevezese);
                    foreach (var jatek in jatekFogadasok)
                    {
                        List<Eredmeny> eredmenyek = jatekok.Find(x => x.Nev == jatek.Key).Eredmenyek;
                        double nyertPontok = jatek.Where(x => x.Eredmeny == eredmenyek.Find(y => y.Alany == x.Alany && y.Esemeny == x.Esemeny)?.Ertek).Sum(x => x.FogadasOsszeg);

                        Console.WriteLine($"{jatek.Key}:");
                        Console.WriteLine($"\tFogadások száma: {jatek.Count()}, Tétek összpontszáma: {jatek.Sum(x => x.FogadasOsszeg)}, Nyeremények összpontszáma: {nyertPontok}");
                    }

                    break;
                default:
                    break;
            }
        }
        static Dictionary<string, double> Pontszamitas(List<Jatek> jatekok, List<Fogadas> fogadasok)
        {
            Dictionary<string, double> pontok = new Dictionary<string, double>();
            var jatekosok = fogadasok.GroupBy(x => x.FogadoNeve);

            foreach (var jatekos in jatekosok)
            {
                double pont = 100;

                foreach (Fogadas fogadas in jatekos)
                {
                    Jatek jatek = jatekok.Find(x => x.Nev == fogadas.JatekMegnevezese);
                    if (jatek.Lezart)
                    {
                        Eredmeny eredmeny = jatek.Eredmenyek.Find(x => x.Alany == fogadas.Alany && x.Esemeny == fogadas.Esemeny);
                        if (fogadas.Eredmeny == eredmeny.Ertek)
                        {
                            pont += fogadas.FogadasOsszeg * eredmeny.Szorzo;
                        }
                        else
                        {
                            pont -= fogadas.FogadasOsszeg;
                        }
                    }
                    else
                    {
                        pont -= fogadas.FogadasOsszeg;
                    }
                }

                pontok.Add(jatekos.Key, pont);
            }

            return pontok;
        }

        static void UjJatek(List<Jatek> jatekok)
        {
            Console.Write("Ki a szervező? ");
            string szervezo = Console.ReadLine();
            if (szervezo == "")
            {
                Console.WriteLine("A szervező nem lehet üres!");
                return;
            }

            Console.Write("Mi a játék neve? ");
            string nev = Console.ReadLine();
            if (nev == "")
            {
                Console.WriteLine("A játék neve nem lehet üres!");
                return;
            }
            if (jatekok.Any(x => x.Nev == nev))
            {
                Console.WriteLine("A játék nevének egyedinek kell lennie!");
                return;
            }

            List<string> alanyok = new List<string>();
            Console.WriteLine("Kik az alanyok? (üres sor a befejezéshez)");
            while (true)
            {
                string alany = Console.ReadLine();
                if (alany == "")
                {
                    break;
                }
                else if (alanyok.Contains(alany))
                {
                    Console.WriteLine("Ez az alany már meg lett adva!");
                }
                else
                {
                    alanyok.Add(alany);
                }
            }

            List<string> esemenyek = new List<string>();
            Console.WriteLine("Mik az események? (üres sor a befejezéshez)");
            while (true)
            {
                string esemeny = Console.ReadLine();
                if (esemeny == "")
                {
                    break;
                }
                else if (esemenyek.Contains(esemeny))
                {
                    Console.WriteLine("Ez az esemény már meg lett adva!");
                }
                else
                {
                    esemenyek.Add(esemeny);
                }
            }

            Jatek jatek = new Jatek(szervezo, nev, alanyok, esemenyek);
            File.AppendAllText("jatekok.txt", (File.Exists("jatekok.txt") ? "\n" : "") + jatek.ToString());
            Console.Clear();
        }

        static void Lezaras(List<Jatek> jatekok, List<Fogadas> fogadasok)
        {
            Console.Write("Mi a neved? ");
            string szervezo = Console.ReadLine();

            Jatek[] szurtJatekok = jatekok.Where(x => x.Lezart == false && x.Szervezo == szervezo).ToArray();
            if (szurtJatekok.Length == 0)
            {
                Console.WriteLine("Nincs lezárható játék!");
                return;
            }

            Console.Write("Játek kiválasztása: ");
            Menuvalaszthato menu = new Menuvalaszthato(szurtJatekok.Select(x => x.Nev).ToArray());
            string kivalasztott = menu.Indit(2);
            Jatek jatek = jatekok.Find(x => x.Nev == kivalasztott);

            Console.WriteLine("Add meg az eredményeket:");
            List<Eredmeny> eredmenyek = new List<Eredmeny>();
            foreach (string alany in jatek.Alanyok)
            {
                Console.WriteLine($"{alany}:");
                foreach (string esemeny in jatek.Esemenyek)
                {
                    Console.Write($"\t{esemeny}: ");
                    string eredmeny = Console.ReadLine();
                    int fogadasokSzama = fogadasok.Where(x => x.Alany == alany && x.Esemeny == esemeny).Count();
                    double szorzo = fogadasokSzama == 0 ? 0 : Math.Round(1 + (5 / Math.Pow(2, fogadasokSzama - 1)), 2);
                    eredmenyek.Add(new Eredmeny(alany, esemeny, eredmeny, szorzo));
                }
            }

            jatek.Eredmenyek = eredmenyek;
            File.AppendAllLines("eredmenyek.txt", eredmenyek.Select(x => x.ToString()).Prepend(jatek.Nev));
    }

        static void FogadasLeadas(List<Jatek> jatekok, List<Fogadas> fogadasok)
        {
            static void MegjelenitPont(double osszeg)
            {
                Console.Clear();
                Console.SetCursorPosition(Console.WindowWidth - 15, 0);
                Console.WriteLine("Pontok:{0}", osszeg);
                Console.SetCursorPosition(0, 0);
            }


            string fogadoNev;
            do
            {
                Console.Write("Adja meg a fogadó nevét: ");
                fogadoNev = Console.ReadLine();
                Console.WriteLine();
            } while (fogadoNev.Trim() == "");


            Dictionary<string, double> pontok = Pontszamitas(jatekok, fogadasok);
            double osszeg = pontok.ContainsKey(fogadoNev) ? pontok[fogadoNev] : 100;

            MegjelenitPont(osszeg);

            //fogadásnév
            Console.WriteLine("\nVálassza ki melyik fogadáson indul:");
            List<Jatek> aktivJatekok = jatekok.Select(x => x).Where(x => x.Lezart == false).ToList();
            string[] jatekNevek = aktivJatekok.Select(x => x.Nev).ToArray();

            Menuvalaszthato menupontok = new Menuvalaszthato(jatekNevek);
            string valasztottNev = menupontok.Indit(3);
            Jatek valasztottJatek = jatekok.Find(x => x.Nev == valasztottNev);

            string[] fogadottEsemenyek = fogadasok.Where(x => x.FogadoNeve == fogadoNev && x.JatekMegnevezese == valasztottJatek.Nev).Select(x => x.Esemeny).ToArray();

            MegjelenitPont(osszeg);

            //esemény
            Console.WriteLine("\nVálassza ki melyik eseményre fogad:");
            menupontok = new Menuvalaszthato(valasztottJatek.Esemenyek.Where(x => !fogadottEsemenyek.Contains(x)).ToArray());
            string valasztottEsemeny = menupontok.Indit(3);

            MegjelenitPont(osszeg);

            //alany
            Console.WriteLine("\nVálassza ki melyik alanyra fogad:");
            menupontok = new Menuvalaszthato(valasztottJatek.Alanyok.ToArray());
            string valasztottAlany = menupontok.Indit(3);

            MegjelenitPont(osszeg);


            string eredmeny;
            int fogadasOsszeg;
            do
            {
                Console.WriteLine();
                Console.Write("Adja meg az eredményt: ");
                eredmeny = Console.ReadLine();
                Console.WriteLine();
                Console.Write("Adja meg a fogadásra feltevendő pontokat: ");
                bool sikeresParse = int.TryParse(Console.ReadLine(), out fogadasOsszeg);
                if (!sikeresParse || fogadasOsszeg < 0)
                {
                    Console.WriteLine("Helytelen érték! Adjon meg egy pozitív egész számot pontként!");
                }
                if (fogadasOsszeg > osszeg)
                {
                    Console.WriteLine("Helytelen érték! Nincs ennyi pontod.");
                }
            } while (fogadasOsszeg <= 0 || osszeg - fogadasOsszeg < 0 || eredmeny == "");

            osszeg -= fogadasOsszeg;
            MegjelenitPont(osszeg);


            Fogadas ujFogadas = new Fogadas(fogadoNev, valasztottJatek.Nev, fogadasOsszeg, valasztottAlany, valasztottEsemeny, eredmeny);
            fogadasok.Add(ujFogadas);
            File.AppendAllText("fogadasok.txt", (File.Exists("fogadasok.txt") ? "\n" : "") + ujFogadas.ToString());
            Console.Clear();
        }
    }
}