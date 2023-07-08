using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Linq;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Shallot
{
    public class Api
    {
        static bool isInitialized = false;
        static ConcurrentDictionary<int,Character> characters = new ConcurrentDictionary<int, Character>();
        static CharacterData characterData;
        static int characterCount = 0;
        static void Initialize()
        {
            isInitialized = true;
            characters = new ConcurrentDictionary<int, Character>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

              
                Console.WriteLine("Loading data...");
                LoadData();

            }
        }
        static int maxIndex;
        internal static void Download(string url)
        {
            string[] splitUrl = url.Split("/");
            int i = int.Parse(splitUrl[splitUrl.Length - 1]);
            Console.WriteLine("Downloading character " + i + ".");
            string html = Utils.GetSiteAsHTML(i);
            if (html == "error")
            {
                Console.WriteLine("Invalid character at index " + i + ". Skipping...");
                return;
            }
            Character character = Utils.ParseCharacterData(html);
            if (character.name != "INVALID")
            {
                characters.TryAdd(i, character);
                //Shallot`s id is 9000 for some reason. 
                if (i > maxIndex&&i<1000)
                {
                    maxIndex = i;
                }
                Console.WriteLine("Downloaded character " + character.name + " at index "+i+".");
            }
 

        }
        static void DownloadData()
        {
            string html = Utils.GetSiteAsHTML();
            HtmlDocument doc = new HtmlDocument(); doc.LoadHtml(html);
            var links = Utils.GetCharacterLinks(doc);
            characterCount = links.Count;

            Console.WriteLine("Download started...");
            Parallel.ForEach(links,new ParallelOptions() { MaxDegreeOfParallelism=-1},Download);
            Console.WriteLine("Download finished.");
         
            var path = AppDomain.CurrentDomain.BaseDirectory + "/characters.json";
            characterData = new CharacterData() { characters = characters.Values.ToList(), count=maxIndex};
            var data = JsonConvert.SerializeObject(characterData);
            File.WriteAllText(path, data);
        }
        static void LoadData()
        {
            string html = Utils.GetSiteAsHTML();
            HtmlDocument doc = new HtmlDocument(); doc.LoadHtml(html);
            var links = Utils.GetCharacterLinks(doc);
            characterCount = links.Count;
            foreach (string url in links)
            {
                string[] splitUrl = url.Split("/");
                int i = int.Parse(splitUrl[splitUrl.Length - 1]);
                if (i > maxIndex && i < 1000)
                {
                    maxIndex = i;
                }
            }
            var path = AppDomain.CurrentDomain.BaseDirectory + "/characters.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Error: Missing or invalid characters.json.");
                DownloadData();
                return;
            }

            CharacterData? data = JsonConvert.DeserializeObject<CharacterData>(File.ReadAllText(path));
            characterData = data;
            if (characterData == null)
            {
                Console.WriteLine("Missing characters.json file. Downloading data... (This may take a few minutes)");
                DownloadData();
                return;
            }
            else if (characterCount != characterData.count)
            {
                Console.WriteLine("Outdated characters.json file. Downloading data... (This may take a few minutes)");
                DownloadData();
                return;
            }
            Console.WriteLine("Found characters.json with " + characterData.count);
            
        }

        public static List<Character> GetCharactersByName(string name)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            return characterData.characters.FindAll((x)=>x.name.Contains(name));
        }
        public static Character GetCharacterById(string id)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            return characterData.characters.Find((x) => x.id==id);
        }
    }
}   