using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Annoncer.Telegram
{
    public static class SubscribersList
    {
        private const string SubscribersFileName = "Subscribers.lst";

        private static readonly HashSet<string> List = new HashSet<string>();

        static SubscribersList()
        {
            Reload();
        }

        private static void Reload()
        {
            Console.WriteLine($"Reloading sucbscribers list from file [{SubscribersFileName}]");
            try
            {
                if (!File.Exists(SubscribersFileName))
                {
                    Console.WriteLine("File not found. Creating new one.");
                    File.CreateText(SubscribersFileName).Dispose();
                }
                List.Clear();
                var lst = File.ReadAllLines(SubscribersFileName);
                Console.WriteLine($"Subscribers list : [{string.Join(" - ", lst)}]");
                foreach (var s in lst)
                {
                    List.Add(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in List.Reload [{e}]");
            }
        }

        private static void Save()
        {
            Console.WriteLine($"Saving sucbscribers list to file [{SubscribersFileName}]");
            try
            {
                if (File.Exists(SubscribersFileName))
                {
                    File.Delete(SubscribersFileName);
                }
                File.WriteAllLines(SubscribersFileName, List);
                Console.WriteLine("Saved successfull");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in List.Save [{e}]");
            }
        }

        public static IEnumerable<string> GetList()
        {
            return List.ToList().AsReadOnly();
        }

        public static void Add(string value)
        {
            List.Add(value);
            Save();
        }

        public static void Remove(string value)
        {
            List.Remove(value);
            Save();
        }
    }
}