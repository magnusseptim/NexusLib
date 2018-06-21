using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using NexusLib.Tools.Interfaces;
using System;
using System.Threading.Tasks;

namespace NexusLib.Tools
{
    public class StandardInvocator : IStandardInvocator
    {
        public async Task<BaseResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<ResourceResponse<BaseResponseGenericType>>> func)
            where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new()
        {
            BaseResponse<BaseResponseGenericType> doneCorrect = new BaseResponse<BaseResponseGenericType>(true);
            try
            {
                doneCorrect = new BaseResponse<BaseResponseGenericType>(true)
                {
                    ResourceResponse = await func()
                };
            }
            catch (Exception ex)
            {
                doneCorrect = new BaseResponse<BaseResponseGenericType>(false, ex.Message);
            }

            return doneCorrect;
        }

        public async Task<FeedResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<FeedResponse<BaseResponseGenericType>>> func)
          where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new()
        {
            FeedResponse<BaseResponseGenericType> doneCorrect = new FeedResponse<BaseResponseGenericType>();
            try
            {
                doneCorrect = await func();
            }
            catch (Exception)
            {
                doneCorrect = new FeedResponse<BaseResponseGenericType>();
            }

            return doneCorrect;
        }

        public Task<BaseResponse<BaseResponseGenericType>> GetDefaultRespone<BaseResponseGenericType>()
            where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new()
        {
            return new Task<BaseResponse<BaseResponseGenericType>>(() =>
            {
                return new BaseResponse<BaseResponseGenericType>(false, Vault.VStandardInvocator.ErrorGetDefaultResponeMessage);
            });
        }
    }
}
