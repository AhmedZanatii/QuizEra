using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Common
{
    public class CustomResponse<TResult>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TResult Data { get; set; }

        public static CustomResponse<TResult> Succeded(TResult data, string message = null)
        {
            return new CustomResponse<TResult>
            {
                Success = true,
                Message = message ?? "Request successful",
                Data = data
            };
        }

        public static CustomResponse<TResult> Fail(string message)
        {
            return new CustomResponse<TResult>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }

    }
}
