using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using GameServer.PlayerSystem;
using Newtonsoft.Json.Linq;
using Serilog;

namespace GameServer.TaskSystem
{
    public class TaskManager
    {
        private struct RequirementFormat
        {
            public string Type;
            public int Number;
            public float Probability;
        }

        public Player PlayerOwner;
        public List<int> TaskList = new();

        public bool AcceptTask(int taskId)
        {
            if (TaskList.Contains(taskId))
            {
                Log.Warning("重复接取任务");
                return false;
            }
            TaskList.Add(taskId);
            return true;
        }

        public bool SubmitTask(int taskId)
        {
            if (!DataManager.Instance.TaskDict.TryGetValue(taskId, out var taskDefine)) return false;
            
            JArray arr = JArray.Parse(taskDefine.Requirement);
            foreach (var requirement in arr)
            {
                var obj = requirement as JObject;
                if (obj == null) continue;
                switch (obj["Type"]?.Value<String>())
                {
                    case "Kill":

                        break;
                    case "Collect":
                        var itemId = obj["ItemId"]?.Value<int>();
                        if (itemId == null) continue;
                        if (!PlayerOwner.Knapsack.HasItem((int)itemId, obj["Number"]?.Value<int>() ?? 1)) return false;
                        break;
                }
            }
            return true;
        }
    }
}
