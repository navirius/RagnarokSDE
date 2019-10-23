using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDE.Editor.Generic.YamlModel
{
    public class PetDbModel
    {
        public PetHeaderModel Header { get; set; }
        public List<PetItemBodyModel> Body { get; set; } = new List<PetItemBodyModel>();
    }

    public class PetHeaderModel
    {
        public string Type { get; set; }
        public int Version { get; set; }
    }


    public class PetItemBodyModel
    {
        public string Mob { get; set; }
        public string TameItem { get; set; }
        public string EggItem { get; set; }
        public string EquipItem { get; set; }
        public string FoodItem { get; set; }
        public int Fullness { get; set; }
        public int HungryDelay { get; set; }
        public int HungerIncrease { get; set; }
        public int IntimacyStart { get; set; }
        public int IntimacyFed { get; set; }
        public int IntimacyOverfed { get; set; }
        public int IntimacyHungry { get; set; }
        public int IntimacyOwnerDie { get; set; }
        public int CaptureRate { get; set;}
        public bool SpecialPerformance { get; set; }
        public int AttackRate { get; set; }
        public int RetaliateRate { get; set; }
        public int ChangeTargetRate { get; set; }
        public bool AllowAutoFeed { get; set; }
        public string Script { get; set; }
        public string SupportScript { get; set; }
        public List<PetEvolutionItemModel> Evolution { get; set; } = new List<PetEvolutionItemModel>();

    }

    public class PetEvolutionItemModel
    {
        public string Target { get; set; }
        public List<PetEvolutionItemRequirementItemModel> ItemRequirements { get; set; } = new List<PetEvolutionItemRequirementItemModel>();
    }

    public class PetEvolutionItemRequirementItemModel
    {
        public string Item { get; set; }
        public int Amount { get; set; }
    }

    
}
