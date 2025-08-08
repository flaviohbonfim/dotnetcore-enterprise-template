namespace DotNetCore.EnterpriseTemplate.WebApi.Common;

public class ApiResponseWithData<T> : ApiResponse
{
    public T? Data { get; set; }
}
