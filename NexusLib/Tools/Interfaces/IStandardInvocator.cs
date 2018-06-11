using Microsoft.Azure.Documents.Client;
using NexusLib.Model;
using System;
using System.Threading.Tasks;

namespace NexusLib.Tools.Interfaces
{
    public interface IStandardInvocator
    {
        Task<BaseResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<ResourceResponse<BaseResponseGenericType>>> func)
          where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new();

        Task<FeedResponse<BaseResponseGenericType>> InvokeStandardThreadPoolAction<BaseResponseGenericType>(Func<Task<FeedResponse<BaseResponseGenericType>>> func)
          where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new();

        Task<BaseResponse<BaseResponseGenericType>> GetDefaultRespone<BaseResponseGenericType>()
            where BaseResponseGenericType : Microsoft.Azure.Documents.Resource, new();
    }
}
