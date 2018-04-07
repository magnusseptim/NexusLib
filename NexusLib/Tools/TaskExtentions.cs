using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NexusLib.Tools
{
    public static class TaskExtentions
    {
        public static Task<T> RunMultiThread<T>(this Task<T> func)
        {
            return Task.Run<T>(() =>
            {
                return func;
            });
        }
    }
}
