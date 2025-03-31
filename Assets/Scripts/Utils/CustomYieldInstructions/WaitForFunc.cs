using System;
using UnityEngine;

namespace ChessMonsterTactics
{
    public class WaitForFunc : CustomYieldInstruction
    {
        private readonly Func<bool> _func;
        public override bool keepWaiting { get => _func?.Invoke() == false; }

        public WaitForFunc(Func<bool> func)
        {
            _func = func;
        }
    }
}
