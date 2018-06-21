using Microsoft.Azure.Documents;
using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NexusLib.Extensions
{
    public static class ResultRetriever
    {
        // TODO : Test it with real usecase
        /// <summary>
        /// Retrieve data from Task<BaseResponse<T> mainly used in base DocumentClient 
        /// </summary>
        /// <typeparam name="T">Type of data to retrieved</typeparam>
        /// <param name="documentTask">Task from client method which contains data</param>
        /// <param name="logger">Logger used to log possible exception</param>
        /// <returns>Retrieved object</returns>
        public static T RetrieveResult<T>(this Task<BaseResponse<T>> documentTask, NLog.Logger logger) where T : Resource, new()
        {
            T returned;
            try
            {
                returned = (T)(dynamic)documentTask.Result.ResourceResponse;
            }
            catch (AggregateException ex)
            {
                logger.Error(ex, string.Format(Vault.VResultRetriever.RetrieveResultAggrException, ex.Source));
                returned = null;
            }

            return returned;
        }
    }
}
