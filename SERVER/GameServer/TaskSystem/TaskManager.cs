using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using GameServer.PlayerSystem;
using MMORPG.Common.Proto.Npc;
using MMORPG.Common.Proto.Task;
using Newtonsoft.Json.Linq;
using Serilog;

namespace GameServer.TaskSystem
{
    public class TaskManager
    {
        public Player PlayerOwner;
        public List<int> TaskList = new();
        private TaskInfo? _taskInfo;
        private bool _hasChange;

        public TaskInfo GetTaskInfo()
        {
            if (_hasChange || _taskInfo == null)
            {
                _taskInfo = new();
                _taskInfo.TaskArr.AddRange(TaskList.Select(x => new TaskRecord() { TaskId = x }));
                _hasChange = false;
            }
            return _taskInfo;
        }

        public void LoadTaskInfo(byte[]? taskInfoData)
        {
            if (taskInfoData == null)
            {
                return;
            }
            //DialogueInfo info = DialogueInfo.Parser.ParseFrom(taskInfoData);
            //foreach (var record in info.DialogueArr)
            //{
            //    if (!DataManager.Instance.NpcDict.TryGetValue(record.NpcId, out var define))
            //    {
            //        Log.Error($"NpcId不存在:{record.NpcId}");
            //        continue;
            //    }
            //    _recordDict[record.NpcId] = record;
            //}
        }

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
                    case "Collect":
                        var itemId = obj["ItemId"]?.Value<int>();
                        if (itemId == null) continue;
                        if (!PlayerOwner.Knapsack.HasItem((int)itemId, obj["Number"]?.Value<int>() ?? 1)) return false;
                        break;
                }
            }
            foreach (var requirement in arr)
            {
                var obj = requirement as JObject;
                if (obj == null) continue;
                switch (obj["Type"]?.Value<String>())
                {
                    case "Collect":
                        var itemId = obj["ItemId"]?.Value<int>();
                        if (itemId == null) continue;
                        PlayerOwner.Knapsack.RemoveItem((int)itemId, obj["Number"]?.Value<int>() ?? 1);
                        break;
                }
            }
            return true;
        }
    }
}
