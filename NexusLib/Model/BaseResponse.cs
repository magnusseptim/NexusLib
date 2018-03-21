using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusLib.Model
{
    public class BaseResponse<T>  where T : Microsoft.Azure.Documents.Resource, new()
    {
        public bool Status { get; private set; }
        public string ErrorMessage { get; private set; }
        public ResourceResponse<T> ResourceResponse { get; set; }

        public BaseResponse(bool status, string errorMessage = "Done Correct")
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }
    }
}
