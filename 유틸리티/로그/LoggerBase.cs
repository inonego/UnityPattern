using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public abstract class LoggerBase : ILogger
    {
        [SerializeField]
        protected bool log = true;

        public abstract void Log(string message);
        public abstract void LogWarning(string message);
        public abstract void LogError(string message);
    }
}