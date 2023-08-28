using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tapeti.DataAnnotations.Extensions
{
    /// <summary>
    /// Can be used on an object property to validate the objects fields
    /// </summary>
    public class ValidateObjectAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count == 0) 
                return ValidationResult.Success;

            var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");
            results.ForEach(compositeResults.AddResult);

            return compositeResults;
        }
    }

    /// <summary>
    /// Container class for a summary of validation results of a validation request.
    /// </summary>
    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> results = new List<ValidationResult>();

        /// <summary>
        /// The validation results
        /// </summary>
        public IEnumerable<ValidationResult> Results => results;

        /// <summary>
        /// Constructor that accepts an error message.
        /// </summary>
        /// <param name="errorMessage"></param>
        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

        /// <summary>
        /// Constructor that accepts an error message as well as a list of member names involved in the validation.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="memberNames"></param>
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationResult"></param>
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        /// <summary>
        /// Constructor that creates a copy of an existing ValidationResult.
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddResult(ValidationResult validationResult)
        {
            results.Add(validationResult);
        }
    }
}
