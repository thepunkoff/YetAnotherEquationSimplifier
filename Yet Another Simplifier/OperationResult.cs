using System;
using System.Collections.Generic;
using System.Text;
using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public Token Result { get; set; }
        public string ErrorMessage { get; set; }

        public static OperationResult CreateFailure(string errorMessage)
        {
            return new OperationResult { Success = false, ErrorMessage = errorMessage };
        }

        public static OperationResult CreateSuccess(Token result)
        {
            return new OperationResult { Success = true, Result = result };
        }

        public override string ToString()
        {
            if (Success)
            {
                return Result.ToString();
            }
            else
            {
                return ErrorMessage;
            }
        }
    }
}
