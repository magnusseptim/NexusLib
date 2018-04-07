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
        public async Task<BaseResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<ResourceResponse<BaseResponseGenericType>>> func) 
            where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new()
        {
            BaseResponse<BaseResponseGenericType> doneCorrect = new BaseResponse<BaseResponseGenericType>(true);
            try
            {
                doneCorrect = new BaseResponse<BaseResponseGenericType>(true);
                doneCorrect.ResourceResponse =  await func();
            }
            catch (Exception ex)
            {
                doneCorrect = new BaseResponse<BaseResponseGenericType>(false, ex.Message);
            }

            return doneCorrect;
        }
    }
}
