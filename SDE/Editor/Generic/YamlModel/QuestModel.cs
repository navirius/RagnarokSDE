using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDE.Editor.Generic.YamlModel
{
    public class QuestModel
    {
        public HeaderModel Header { get; set; }
        public List<BodyModel> Body { get; set; } = new List<BodyModel>();
    }
    public class HeaderModel
    {
        public string Type { get; set; }
        public int Version { get; set; }
    }
    public class BodyModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TimeLimit { get; set; } = 0;
        public int TimeInDay { get; set; } = 0;
        public int TimeAtHour { get; set; } = 0;
        public int TimeAtMinute { get; set; } = 0;
        public List<TargetModel> Target { get; set; } = new List<TargetModel>();
        public List<DropModel> Drop { get; set; } = new List<DropModel>();

    }

    public class TargetModel
    {
        public string Mob { get; set; }
        public int Count { get; set; }
    }

    public class DropModel
    {
        public string Mob { get; set; }
        public string Item { get; set; }
        public int Rate { get; set; }
    }
}
