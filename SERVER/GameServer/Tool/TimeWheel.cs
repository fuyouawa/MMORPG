using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Tool
{
    public class TimeWheel
    {
        private const int CircleCount = 5;
        private const int SlotCount = 1 << 6;

        public struct TimeTask
        {
            public int Tick;
            public Action<TimeTask> Action;
        };

        private LinkedList<TimeTask> _pendingList;
        private LinkedList<TimeTask> _backupList;
        private LinkedList<TimeTask>[] _slot;
        private int[] _indexArr;
        private long _lastMs;
        private int _tickMs;  // 最小槽的时间范围，毫秒单位
        private volatile bool _stop;

        public TimeWheel(int precision = 10) {

            _pendingList = new();
            _backupList = new();
            _slot = new LinkedList<TimeTask>[SlotCount * CircleCount];
            _indexArr = new int[CircleCount];
            _tickMs = precision;
            _stop = false;

            for (int i = 0; i < SlotCount * CircleCount; i++)
            {
                _slot[i] = new();
            }
            for (int i = 0; i < CircleCount; i++)
            {
                _indexArr[i] = 0;
            }
        }

        public async Task Start()
        {
            _lastMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond / _tickMs;
            do
            {
                if (_pendingList.Count > 0)
                {
                    lock (_pendingList)
                    {
                        LinkedList<TimeTask> temp = _backupList;
                        _backupList = _pendingList;
                        _pendingList = temp;
                    }
                    DispatchTasksToSlot(_backupList);
                }

                // 根据上次循环至今经过的时间，逐格推进
                var nowMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond / _tickMs;
                var duration = nowMs - _lastMs;
                for (int i = 0; i < duration; i++)
                {
                    if (_slot[_indexArr[0]].Count > 0)
                    {
                        foreach (var task in _slot[_indexArr[0]])
                        {
                            task.Action(task);
                            // Task.Run(task.Action);
                        }
                        _slot[_indexArr[0]].Clear();
                    }

                    // 如果需要向上层时间轮推进，则将上层时间轮的队列向下派发
                    int j = 0;
                    ++_indexArr[0];
                    do
                    {
                        bool ge = _indexArr[j] >= SlotCount;
                        if (ge) _indexArr[j] = 0;
                        if (j > 0)
                        {
                            int index = j * SlotCount + _indexArr[j];
                            DispatchTasksToSlot(_slot[index]);
                        }
                        if (!ge || ++j >= CircleCount) break;
                        ++_indexArr[j];
                    } while (true);
                }
                _lastMs = nowMs;
                await Task.Delay(_tickMs);
            } while (_stop == false);
        }

        void Stop()
        {
            _stop = true;
        }

        private void DispatchTasksToSlot(LinkedList<TimeTask> list)
        {
            for (var task = list.First; task != null; )
            {
                var next = task.Next;
                int mask = 0x3f;        // 0011 1111
                // 根据时长插入对应的槽中
                int time = task.Value.Tick;
                for (int i = 0; i < CircleCount; i++)
                {
                    if ((time & ~mask) == 0)
                    {
                        // 应插入到当前层
                        int index = time & mask;
                        index = i * SlotCount + ((index + _indexArr[i]) % SlotCount);

                        // 清除在当前层的时长，在下次向下派发时可以插入到下层
                        int mask2 = ~(0x7fffffff << (i * 6));
                        task.ValueRef.Tick &= mask2;

                        list.Remove(task);
                        _slot[index].AddLast(task);
                        break;
                    }
                    time >>= 6;
                }
                task = next;
            }
        }

        /// <summary>
        /// 将延时任务追加到时间轮中
        /// 不可修改返回的task
        /// </summary>
        /// <param name="task"></param>
        public TimeTask AddTask(int ms, Action<TimeTask> action)
        {
            if (ms < _tickMs) {
                ms = _tickMs;
            }
            var task = new TimeTask()
            {
                Tick = ms / _tickMs,
                Action = action
            };
            lock (_pendingList)
            {
                _pendingList.AddLast(task);
            }
            return task;
        }
    }

    public class TimeWheelTest
    {
        public static async Task Start()
        {
            //Console.WriteLine($"start:{DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond}");
            int count = 0;
            var tw = new TimeWheel(1);

            var begin = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            tw.Start();
            byte[] randomNumber = new byte[4];
            for (int i = 0; i < 1000000; i++)
            {
                //using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                //{
                //    rng.GetBytes(randomNumber);
                //}
                //int randomValue = BitConverter.ToInt32(randomNumber, 0);
                //randomValue = Math.Abs(randomValue);
                //randomValue %= 10;

                int j = i;
                tw.AddTask(1, (task) => {
                    //Console.WriteLine($"[{j}][{count++}]{randomValue}ms:{DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond}");
                    //count++;
                    Interlocked.Increment(ref count);
                }
                );
            }
            while (count < 1000000)
            {
                await Task.Delay(1);
            }
            var end = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            //await Task.Delay(1000000);
            Console.WriteLine($"end:{end - begin}ms");
        }
    }

}
