using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Password
{
  class Program
  {
    static string[] codes = { "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad", 
                              "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
                              "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f"};   // хэш-ключи
    
    static int codeNumber;
    
    static char[] alph = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    static Stopwatch st = new Stopwatch(); // таймер для подсчёта времени работы программы
    
    static void Main(string[] args)
    {
      Console.WriteLine("Введите номер хэш-ключа");
      codeNumber = Int32.Parse(Console.ReadLine()) - 1;
      
      Thread[] threads = new Thread[26]; // создание массива потоков. 1 буква алфавита = 1 поток
      
      st.Start(); // старт таймера
      
      for (int i = 0; i < threads.Length; i++) // в цикле инициализируются потоки
      {
        var i1 = i;
        threads[i] = new Thread(Brute);
        threads[i].Start(i);
      }
     
    }
    
    static string CreateHash(char[] word)
    {
      StringBuilder builder = new StringBuilder();
      
      SHA256 hash = SHA256.Create();

      byte[] result = hash.ComputeHash(Encoding.ASCII.GetBytes(word)); // массив байтов для хранения хэша 
     
      foreach (byte b in result)
        builder.Append(b.ToString("x2"));
      
      return builder.ToString(); // возвращает хэш в виде строки
    }
    
    static void Brute(object letNumb)
    {
        char[] pass = { 'a', 'a', 'a', 'a', 'a' }; // инициализация пароля
        
        pass[0] = alph[(int) letNumb]; // первая буква = передаваемая буква (соответствует номеру потока)
        
        // в цикле перебираютя буквы от а до z для каждой буквы после 1 ->
        // -> т.е. так выглядит перебор:
        // 1 поток: 1-ая буква = а , остальные буквы перебирает от a до z
        // 2 поток: 1-ая буква = b , остальные буквы перебирает от a до z
        // 3 поток: 1-ая буква = c , остальные буквы перебирает от a до z
        
        foreach (char letter2 in alph) // перебор 2-ой буквы
        {
          pass[1] = letter2;
          
          foreach (char letter3 in alph) // перебор 3-ой буквы
          {
            pass[2] = letter3;
            
            foreach (char letter4 in alph) // перебор 4-ой буквы
            {
              pass[3] = letter4;
              
              foreach (char letter5 in alph) // перебор 5-ой буквы
              {
                pass[4] = letter5;

                String password = CreateHash(pass);
                if (password == codes[codeNumber]) // совпал ли хэш
                {
                  foreach (char ch in pass) 
                  {
                    Console.Write(ch); 
                  }
                  
                  st.Stop(); // стоп таймер
                  Console.WriteLine("\nВремя подсчёта: {0} секунд", st.Elapsed.TotalSeconds);
                  
                  Environment.Exit(0); 
                }
                
              }
            }
          }
        }
    }
  }
}
