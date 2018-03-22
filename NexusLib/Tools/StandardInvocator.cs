using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusLib.Tools
{
    public class StandardInvocator
    {
        public Task<BaseResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<ResourceResponse<BaseResponseGenericType>>> func) 
            where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new()
        {
            BaseResponse<BaseResponseGenericType> doneCorrect = new BaseResponse<BaseResponseGenericType>(true);
            try
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    var resourceResponse = await func();
                    doneCorrect = new BaseResponse<BaseResponseGenericType>(true);
                    doneCorrect.ResourceResponse = resourceResponse;
                });
            }
            catch (Exception ex)
            {
                doneCorrect = new BaseResponse<BaseResponseGenericType>(false, ex.Message);
            }

            return Task.FromResult(doneCorrect);
        }
    }
}
