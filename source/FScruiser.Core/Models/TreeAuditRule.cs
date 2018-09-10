using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Validation;
using System;
using System.Reflection;

namespace FScruiser.Models
{
    public class TreeAuditRule : IValidationRule<Tree>
    {
        private string _field;
        private PropertyInfo _propertyInfo;

        [Field("Field")]
        public string Field
        {
            get { return _field; }
            set
            {
                _field = value;
                _propertyInfo = typeof(Tree).GetProperty(value) ?? throw new ArgumentException("invalid property value :" + value);
            }
        }

        [Field("Min")]
        public int Min { get; set; }

        [Field("Max")]
        public int Max { get; set; }

        [Field("Required")]
        public bool Required { get; set; }

        [Field("ValueSet")]
        public string ValueSet { get; set; }

        public string Property => Field;

        public ValidationLevel Level => ValidationLevel.Warning;

        public string Message
        {
            get
            {
                return BuildErrorMessage();
            }
        }

        public ValidationError Validate(Tree target)
        {
            var value = _propertyInfo.GetValue(target);
            if (!Validate(target, value))
            {
                return new ValidationError(this);
            }
            else { return null; }
        }

        public bool Validate(Tree sender, object value)
        {
            bool isValid = true;//valid until proven invalid

            if (this.Required == true && value == null) { isValid = false; }

            if (isValid && this.Min != 0)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num < this.Min) { isValid = false; }
                }
            }
            if (isValid && this.Max != 0)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num > this.Max) { isValid = false; }
                }
            }

            if (isValid && value != null && !string.IsNullOrEmpty(ValueSet))
            {
                if (!(ValueSet.IndexOf(value.ToString(), System.StringComparison.Ordinal) >= 0)) { isValid = false; }
            }
            return isValid;
        }

        private string BuildErrorMessage()
        {
            var sb = new System.Text.StringBuilder();
            if (this.Min > 0)
            {
                sb.AppendFormat(null, "{0} should be greater than {1}", this.Field, this.Min);
            }
            if (this.Max > 0)
            {
                sb.AppendFormat(null, "{0} should be less than {1}", this.Field, this.Max);
            }
            if (this.Required)
            {
                sb.AppendFormat(null, "{0} is Required", this.Field);
            }
            if (!string.IsNullOrEmpty(this.ValueSet))
            {
                sb.AppendFormat(null, "Value for {0} is not valid", this.Field);
            }
            return sb.ToString();
        }
    }
}