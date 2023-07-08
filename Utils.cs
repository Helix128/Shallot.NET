using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Data;
using System.Net.Http.Headers;
using System.Linq;
using System;
using System.IO;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Diagnostics;

namespace Shallot
{
    public class Utils
    {
        public static List<string> GetCharacterLinks(HtmlDocument document)
        {
            var selectChar = ".chara.list > a";
            var extractedLinks = document.DocumentNode
                .QuerySelectorAll(selectChar)
                .Select(element => element.GetAttributeValue("href", string.Empty))
                .ToList();

            return extractedLinks;
        }
        public static string GetSiteAsHTML(string url = "https://legends.dbz.space/characters/")
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                string response = client.GetStringAsync(url).Result;

                return response;
            }

        }
        public static string GetSiteAsHTML(int characterId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                var response = client.GetAsync("https://legends.dbz.space/characters/"+characterId).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string urlContents = response.Content.ReadAsStringAsync().Result;
                    return urlContents;
                }
                else
                {
                    return "error";
                }
        
            }

        }
  
        public static string GetInnerHtml(HtmlDocument document, string tagName)
        {
            var element = document.DocumentNode
       .SelectSingleNode("//div[contains(@class,'" + tagName + "')]");

            if (element == null)
            {
                throw new Exception("Element not found");
            }

            string innerHtml = RemoveTabsAndNewlines(element.InnerText);

            return innerHtml;
        }

        public static List<string> GetFullInnerHtml(HtmlDocument document, string tagName, string elementType = "div")
        {
            var elements = document.DocumentNode.SelectNodes("//" + elementType + "[contains(@class,'" + tagName + "')]");

            if (elements == null)
            {
                throw new Exception("Element not found");
            }

            List<string> strings = new List<string>();
            foreach (HtmlNode element in elements)
            {
                strings.Add(RemoveTabsAndNewlines(element.InnerText));
            }

            return strings;
        }

        private static string RemoveTabsAndNewlines(string html)
        {
            return html.Replace("\t", "").Replace("\r", "").Replace("\n", "");
        }

        public static Character ParseCharacterData(string html)
        {
            Character character = new Character();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode;

     
            if (node.QuerySelector(".head.name.large.img_back")==null)
            {
                character.name = "INVALID";
                return character; 
            }
            string name = node.QuerySelector(".head.name.large.img_back").InnerText.Trim();
            character.name = name;

            string id = node.QuerySelector(".head.name.id-right.small.img_back").InnerHtml.Trim();
            character.id = id;

            Color color = (Color)Enum.Parse(typeof(Color), node.QuerySelector("div.element").InnerHtml.Trim());
            character.color = color;

            Rarity rarity = (Rarity)Enum.Parse(typeof(Rarity), node.QuerySelector(".rarity").InnerHtml.Trim());
            character.rarity = rarity;

            List<string> tags = GetFullInnerHtml(doc, "tag", "a");
            character.tags = tags;

            string mainAbilityName = node.QuerySelector("div.frm.form0>.ability.medium").InnerHtml.Trim();
            string mainAbilityDesc = node.QuerySelector("div.frm.form0>.ability_text.small").InnerHtml.Trim();
            character.main_ability = new MainAbility() { name = mainAbilityName, effect = mainAbilityDesc };

            if (rarity == Rarity.ULTRA)
            {
                string ultraAbilityName = node.QuerySelector("#charaultra + div.ability_text > .frm.form0 > span").InnerHtml.Trim();
                string ultraAbilityDesc = node.QuerySelector("#charaultra + div.ability_text > .frm.form0 > div").InnerHtml.Trim();
                UltraAbility ultraAbility = new UltraAbility() { name = ultraAbilityName, effect = ultraAbilityDesc };
                character.ultra_ability = ultraAbility;
            }

            HtmlNode uniqueAbilityNode = node.QuerySelector("#charaunique + div.ability_text");

            character.unique_ability = new UniqueAbilities();
            character.unique_ability.unique_start_abilities = new List<UniqueAbility>();
            character.unique_ability.unique_zenkai_abilities = new List<UniqueAbility>();
            IEnumerable<HtmlNode> abilityNodes = uniqueAbilityNode.QuerySelectorAll(".frm.form0");
            foreach (HtmlNode abilityNode in abilityNodes)
            {
                char tab = '\u0009';
                string line = abilityNode.InnerText.Trim().Replace(tab.ToString(), "");
                character.unique_ability.unique_start_abilities.Add(new UniqueAbility(line));
            }
            IEnumerable<HtmlNode> zenkaiNodes = uniqueAbilityNode.QuerySelectorAll(".frm.form1");
            foreach (HtmlNode zenkaiNode in zenkaiNodes)
            {
                char tab = '\u0009';
                string line = zenkaiNode.InnerText.Trim().Replace(tab.ToString(), "");
                character.unique_ability.unique_zenkai_abilities.Add(new UniqueAbility(line));
            }

            character.has_zenkai = character.unique_ability.unique_zenkai_abilities.Count != 0;

            List<string> stats = new List<string>();

            for (int i = 1; i < 7; i++)
            {
                string selector = string.Format(".row.lvlbreak.lvb1 > div:nth-child({0}) > div.val", i);
                stats.Add(node.QuerySelector(selector).GetAttributeValue("raw", null));
            }

            var baseStats = new Stats
            {
                power = int.Parse(stats[0]),
                health = int.Parse(stats[1]),
                strike_atk = int.Parse(stats[2]),
                strike_def = int.Parse(stats[3]),
                blast_atk = int.Parse(stats[4]),
                blast_def = int.Parse(stats[5]),
            };
            stats.Clear();
            for (int i = 1; i < 7; i++)
            {
                string selector = string.Format(".row.lvlbreak.lvb5000 > div:nth-child({0}) > div.val", i);
                stats.Add(node.QuerySelector(selector).GetAttributeValue("raw", null));

            }
            var maxStats = new Stats
            {
                power = int.Parse(stats[0]),
                health = int.Parse(stats[1]),
                strike_atk = int.Parse(stats[2]),
                strike_def = int.Parse(stats[3]),
                blast_atk = int.Parse(stats[4]),
                blast_def = int.Parse(stats[5]),
            };
            character.base_stats = baseStats;
            character.max_stats = maxStats;
            string z1 = node.QuerySelector(".zability.zI > div").InnerHtml.Trim();
            string z2 = node.QuerySelector(".zability.zII > div").InnerHtml.Trim();
            string z3 = node.QuerySelector(".zability.zIII > div").InnerHtml.Trim();
            string z4 = node.QuerySelector(".zability.zIV > div").InnerHtml.Trim();
            character.z_ability = new List<string>
            {
                z1,
                z2,
                z3,
                z4
            };
            string strike = node.QuerySelector("#charastrike ~ .ability_text.arts > .frm.form0 >.ability_text.small").InnerHtml;
            character.strike = strike;
            string shot = node.QuerySelector("#charashot ~ .ability_text.arts > .frm.form0 >.ability_text.small").InnerHtml;
            character.shot = shot;

            string spcName = node.QuerySelector("#charaspecial_move + div > div > span.ability.medium").InnerText;
            string spcDesc = node.QuerySelector("#charaspecial_move + div > div > div.ability_text.small").InnerText;
            character.special_move = new SpecialMove() { name = spcName.Trim(), effect = spcDesc.Trim() };
            string spcSkillName = node.QuerySelector("#charaspecial_skill + div > div > span.ability.medium").InnerText;
            string spcSkillDesc = node.QuerySelector("#charaspecial_skill + div > div > div.ability_text.small").InnerText;
            character.special_skill = new SpecialSkill() { name = spcSkillName.Trim(), effect = spcSkillDesc.Trim() };
            if (node.QuerySelector("#charaultimate_skill + div > div > span.ability.medium") != null)
            {
                string ultSkillName = node.QuerySelector("#charaultimate_skill + div > div > span.ability.medium").InnerText;
                string ultSkillDesc = node.QuerySelector("#charaultimate_skill + div > div > div.ability_text.small").InnerText;
                character.ultimate_skill = new UltimateSkill() { name = ultSkillName.Trim(), effect = ultSkillDesc.Trim() };
            }
            character.image_url = node.QuerySelector(".cutin.trs0.form0").GetAttributeValue("src", "");
            character.is_lf = node.QuerySelector(".legends-limited") != null;
            return character;
        }
    }
}