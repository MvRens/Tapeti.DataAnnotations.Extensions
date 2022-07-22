using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Tapeti.DataAnnotations.Extensions
{
    /// <summary>
    /// Can be used on numeric fields which are supposed to be Required, as the Required attribute does
    /// not work for these simple types and making them Nullable is counter-intuitive.
    /// </summary>
    /// <remarks>
    /// 0 is not considered a valid value for purposes of this check, as it is the default value
    /// of the supported types.
    /// </remarks>
    public class RequiredNumberAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does not contain a valid numeric value or is 0";
        private const string InvalidTypeErrorMessage = "'{0}' is not a type supported by the RequiredNumber attribute";

        /// <inheritdoc />
        public RequiredNumberAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            bool invalid;

            switch (value)
            {
                case short shortValue:
                    invalid = shortValue == 0;
                    break;

                case ushort ushortValue:
                    invalid = ushortValue == 0;
                    break;

                case byte byteValue:
                    invalid = byteValue == 0;
                    break;

                case decimal decimalValue:
                    invalid = decimalValue == 0;
                    break;

                case float floatValue:
                    invalid = floatValue == 0;
                    break;

                case int intValue:
                    invalid = intValue == 0;
                    break;

                case long longValue:
                    invalid = longValue == 0;
                    break;

                case sbyte sbyteValue:
                    invalid = sbyteValue == 0;
                    break;

                case uint uintValue:
                    invalid = uintValue == 0;
                    break;

                case ulong ulongValue:
                    invalid = ulongValue == 0;
                    break;

                default:
                    return new ValidationResult(string.Format(InvalidTypeErrorMessage, validationContext.DisplayName));
            }

            return invalid
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : null;
        }
    }
}
