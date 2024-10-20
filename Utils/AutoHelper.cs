using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WhereMyTreasure.Utils
{
    public enum ActionType
    {
        Tap = 0,
        Swipe = 1,

    }
    internal class AutoHelper
    {

        bool _isStop = false;
        public int[] _delayTimeRange = { 5, 10 };
        public int _maxWaitTime = 30;
        public int _waitTimeCount = 5;
        List<Action<string?>> _listAction = new List<Action<string?>>();
        public void AddAutoFuncion(ActionType type, dynamic param)
        {
            _listAction.Add((string _deviceID) =>
            {
                if (_isStop)
                {
                    return;
                }

                switch (type)
                {
                    case 0:
                        KAutoHelper.ADBHelper.Tap(_deviceID, param.X, param.Y, param.Time);

                        break;

                }
                //Action here
                //end action
                Random rd = new Random();
                Delay(rd.Next(_delayTimeRange[0], _delayTimeRange[1]));
            });

        }
        public void RunAutoScript(string deviceID)
        {
            Task t = new Task(() =>
            {
                while (true)
                {
                    foreach (var action in _listAction)
                    {
                        action.Invoke(deviceID);
                    }
                }
            });
            t.Start();
        }
        public void Stop()
        {
            _isStop = true;
        }
        private void Delay(int delay)
        {
            while (!_isStop)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                delay--;
                if (_isStop)
                {
                    break;
                }
            }
        }
    }
}
