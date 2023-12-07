using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tapeti.DataAnnotations.Extensions
{
    /// <summary>
    /// Can be used on an IEnumerable property to validate the items
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public class ValidateItemsAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null case has to be handled with the Required attribute
            if (value == null)
                return ValidationResult.Success;

            if (!(value is IEnumerable enumerableValue))
                return new ValidationResult($"{validationContext.MemberName} must be an IEnumerable for the ValidateItems attribute");

            var innerResults = new List<ValidationResult>();
            var enumerator = enumerableValue.GetEnumerator();
            var itemIndex = 0;
            var allValid = true;

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item == null)
                {
                    itemIndex++;
                    continue;
                }

                var itemResults = new List<ValidationResult>();
                var itemContext = new ValidationContext(item, new ValidationContextServiceProvider(validationContext), null);

                if (!Validator.TryValidateObject(item, itemContext, itemResults, true))
                    allValid = false;

                var propertyPath = $"{validationContext.MemberName}[{itemIndex}].";

                innerResults.AddRange(itemResults.Select(itemResult =>
                    new ValidationResult(itemResult.ErrorMessage, itemResult.MemberNames.Select(m => propertyPath + m))));

                itemIndex++;
            }

            return allValid
                ? ValidationResult.Success
                : new NestedValidationResult(
                    "One or more items failed to validate",
                    new[] { validationContext.MemberName ?? "" },
                    innerResults);
        }


        private class ValidationContextServiceProvider : IServiceProvider
        {
            private readonly ValidationContext validationContext;


            public ValidationContextServiceProvider(ValidationContext validationContext)
            {
                this.validationContext = validationContext;
            }


            public object GetService(Type serviceType)
            {
                return validationContext.GetService(serviceType);
            }
        }
    }


    /// <summary>
    /// Class for nested ValidationResults
    /// </summary>
    public class NestedValidationResult : ValidationResult
    {
        /// <summary>
        /// The inner results of the validation Result
        /// </summary>
        public IReadOnlyList<ValidationResult> InnerResults { get; }


        /// <inheritdoc />
        protected NestedValidationResult(ValidationResult validationResult, IReadOnlyList<ValidationResult> innerResults) : base(validationResult)
        {
            InnerResults = innerResults;
        }


        /// <inheritdoc />
        public NestedValidationResult(string errorMessage, IReadOnlyList<ValidationResult> innerResults) : base(errorMessage)
        {
            InnerResults = innerResults;
        }


        /// <inheritdoc />
        public NestedValidationResult(string errorMessage, IEnumerable<string> memberNames, IReadOnlyList<ValidationResult> innerResults) : base(errorMessage, memberNames)
        {
            InnerResults = innerResults;
        }
    }
}
