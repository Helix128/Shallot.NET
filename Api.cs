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
        
        public static List<Character> GetCharactersByName(string name)
        {
            if (!Utils.isInitialized)
            {
                Utils.Initialize();
            }

            return Utils.characterData.characters.FindAll((x)=>x.name.Contains(name));
        }
        public static Character GetCharacterById(string id)
        {
            if (!Utils.isInitialized)
            {
                Utils.Initialize();
            }

            return Utils.characterData.characters.Find((x) => x.id==id);
        }
    }
}   