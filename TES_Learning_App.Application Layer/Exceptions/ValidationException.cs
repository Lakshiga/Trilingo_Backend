using System;
using System.Collections.Generic;

namespace TES_Learning_App.Application_Layer.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException() : base("One or more validation errors occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<string> errors) : base("One or more validation errors occurred.")
        {
            Errors = new Dictionary<string, string[]>();
            foreach (var error in errors)
            {
                Errors["General"] = new[] { error };
            }
        }

        // Helper constructor for single field validation
        public ValidationException(string fieldName, string[] errors) : base($"Validation failed for {fieldName}")
        {
            Errors = new Dictionary<string, string[]>
            {
                { fieldName, errors }
            };
        }
    }
}
