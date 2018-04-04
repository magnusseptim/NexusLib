using Microsoft.Azure.Documents;

namespace NexusLib.Model.Interfaces
{
    public interface IBaseResponse<T>
    {
        string ErrorMessage { get; }
        bool Status { get; }
        T ResourceResponseData { get; set; }
    }
}