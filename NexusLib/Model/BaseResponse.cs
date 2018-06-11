using Microsoft.Azure.Documents.Client;

namespace  NexusLib.Model
{
    public class BaseResponse<T> where T : Microsoft.Azure.Documents.Resource, new()
    {
        public bool Status { get; private set; }
        public string ErrorMessage { get; private set; }
        public ResourceResponse<T> ResourceResponse { get; set; }

        public BaseResponse(bool status, string errorMessage = Vault.BaseResponseErrMsg)
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }
    }
}
