﻿using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Tapeti.DataAnnotations.Extensions
{
    /// <summary>
    /// Can be used on DateTime fields which are supposed to be Required, as the Required attribute does
    /// not work for structs and making them Nullable is counter-intuitive.
    /// </summary>
    /// <remarks>
    /// By default only dates within the range of 1900-01-01 to 2100-01-01 are considered valid.
    /// Provide the MinValue and/or MaxValue attributes to change this range if you must support
    /// dates before or after that.
    /// </remarks>
    public class RequiredDateTimeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does not contain a valid DateTime value";
        private const string InvalidTypeErrorMessage = "'{0}' is not of type DateTime";


        private DateTime minDateTimeValue = new DateTime(1900, 1, 1);
        private DateTime maxDateTimeValue = new DateTime(2100, 1, 1);


        /// <summary>
        /// The minimum date considered valid. Defaults to 1900-01-01.
        /// </summary>
        /// <remarks>
        /// This must be provided as a string instead of a DateTime, as the latter is not valid in an attribute context.
        /// </remarks>
        public string MinValue
        {
            get => minDateTimeValue.ToString("O");
            set => minDateTimeValue = DateTime.Parse(value);
        }

        /// <summary>
        /// The maximum date considered valid. Defaults to 2100-01-01.
        /// </summary>
        /// <remarks>
        /// This must be provided as a string instead of a DateTime, as the latter is not valid in an attribute context.
        /// </remarks>
        public string MaxValue
        {
            get => maxDateTimeValue.ToString("");
            set => maxDateTimeValue = DateTime.Parse(value);
        }

        /// <inheritdoc />
        public RequiredDateTimeAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            if (value.GetType() != typeof(DateTime))
                return new ValidationResult(string.Format(InvalidTypeErrorMessage, validationContext.DisplayName));

            var dateTimeValue = (DateTime)value;
            return dateTimeValue < minDateTimeValue || dateTimeValue > maxDateTimeValue
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : null;
        }
    }
}
