using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Manager;
using GameServer.PlayerSystem;
using Serilog;

namespace GameServer.TaskSystem
{
    public class TaskManager
    {
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
            if (!DataManager.Instance.TaskDict.TryGetValue(taskId, out var taskDefine))
            {
                return false;
            }

            switch (taskDefine.Type)
            {
                case "Kill":
                    
                    break;
                case "Collect":
                    
                    break;
                
            }
            return true;
        }
    }
}
