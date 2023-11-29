// nullReference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verseny
{
    public class Menuvalaszthato
    {
        // public enum Mozgatas { Fel, Le }
        int menupont = 0;
        string[] menupontok;
        int kezdoPont = 0;
        public Menuvalaszthato(string[] menupontok)
        {
            this.menupontok = menupontok;
        }
        private void Frissit()
        {
            ConsoleColor szinNemtartjarajtaHatter = ConsoleColor.Black;
            ConsoleColor szinTartjarajtaHatter = ConsoleColor.White;
            ConsoleColor szinTartjarajtaBetu = ConsoleColor.Black;
            ConsoleColor szinNemtartjarajtaBetu = ConsoleColor.White;
            for (int i = 0; i < menupontok.Length; i++)
            {
                if (i != menupont)
                {
                    Console.BackgroundColor = szinNemtartjarajtaHatter;
                    Console.ForegroundColor = szinNemtartjarajtaBetu;
                }
                else
                {
                    Console.BackgroundColor = szinTartjarajtaHatter;
                    Console.ForegroundColor = szinTartjarajtaBetu;
                }
                Console.SetCursorPosition(0, kezdoPont + i);
                Console.Write($"{menupontok[i]}{new string(' ', menupontok.MaxBy(x => x.Length).Length - menupontok[i].Length + 2)}");
                Console.ResetColor();
                Console.SetCursorPosition(0, kezdoPont + i + 1);
            }
        }
        public string Indit(int kezdoPont)
        {
            this.kezdoPont = kezdoPont;
            string beallitott;
            Frissit();
            while (true)
            {
                ConsoleKeyInfo lenyomott = Console.ReadKey();
                if (lenyomott.Key == ConsoleKey.W || lenyomott.Key == ConsoleKey.UpArrow)
                {
                    if (this.menupont > 0)
                    {
                        this.menupont -= 1;
                    }
                    else
                    {
                        this.menupont = this.menupontok.Length - 1;
                    }
                    Frissit();
                }
                else if (lenyomott.Key == ConsoleKey.S || lenyomott.Key == ConsoleKey.DownArrow)
                {
                    if (this.menupont < this.menupontok.Length - 1)
                    {
                        this.menupont += 1;
                    }
                    else
                    {
                        this.menupont = 0;
                    }
                    Frissit();
                }
                else if (lenyomott.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    return this.menupontok[this.menupont];

                }

            }
        }
    }
}
