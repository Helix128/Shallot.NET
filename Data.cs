using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shallot
{
    [System.Serializable]
    public enum Rarity
    {
        HERO,
        EXTREME,
        SPARKING,
        ULTRA
    }
    [System.Serializable]
    public enum Color
    {
        PUR,
        YEL,
        BLU,
        RED,
        GRN,
        LGT
    }
    [System.Serializable]
    public class MainAbility
    {
        public string name;
        public string effect;
    }
    [System.Serializable]
    public class UniqueAbility
    {
        public string ability;
    
    public UniqueAbility(string ability)
        {
            this.ability = ability;
        }
    }
    [System.Serializable]
    public class UniqueAbilities
    {
        public List<UniqueAbility> unique_start_abilities;
        public List<UniqueAbility> unique_zenkai_abilities;
    }
    [System.Serializable]
    public class UltraAbility
    {
        public string name;
        public string effect;
    }
    [System.Serializable]
    public class Stats
    {
        public int power;
        public int health;
        public int strike_atk;
        public int strike_def;
        public int blast_atk;
        public int blast_def;
    }
    [System.Serializable]
    public class ZAbility
    {
        public List<string> tags;
        public string effect;
    }
    [System.Serializable]
    public class SpecialMove
    {
        public string name;
        public string effect;
    }
    [System.Serializable]
    public class UltimateSkill
    {
        public string name;
        public string effect;
    }
    [System.Serializable]
    public class SpecialSkill
    {
        public string name;
        public string effect;
    }
    [System.Serializable]
    public class Character
    {
        public string name;
        public string id;
        public Color color;
        public Rarity rarity;
        public List<string> tags;
        public MainAbility main_ability;
        public UniqueAbilities unique_ability;
        public UltraAbility ultra_ability;
        public Stats base_stats;
        public Stats max_stats;
        public string strike;
        public string shot;
        public string image_url;
        public SpecialMove special_move;
        public SpecialSkill special_skill;
        public UltimateSkill ultimate_skill;
        public List<string> z_ability;
        public bool is_lf;
        public bool is_tag;
        public bool has_zenkai;

      
    }
    [System.Serializable]
    public class CharacterData
    {
        public int count;
        public List<Character> characters;
    }
}
