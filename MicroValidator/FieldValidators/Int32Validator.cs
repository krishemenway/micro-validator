using System;
using System.Collections.Generic;

namespace MicroValidator.FieldValidators
{
	public class Int32Validator<TRequest> : IValidator<TRequest>
	{
		public Int32Validator(
			string fieldId,
			Func<TRequest, int?> accessPropertyFunc,
			bool isRequired = true,
			int minimumValue = int.MinValue,
			int maximumValue = int.MaxValue)
		{
			FieldId = fieldId;
			AccessPropertyFunc = accessPropertyFunc;

			IsRequired = isRequired;

			MinimumValue = minimumValue;
			MaximumValue = maximumValue;
		}

		public IEnumerable<KeyValuePair<string, string>> ValidateRequest(TRequest request)
		{
			var value = AccessPropertyFunc(request);

			if (IsRequired && !value.HasValue)
			{
				yield return new KeyValuePair<string, string>(FieldId, "Value must be provided");
			}

			if (value.HasValue && value.Value < MinimumValue)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Value must be greater than {MinimumValue}");
			}

			if (value.HasValue && value.Value > MaximumValue)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Value must be less than {MaximumValue}");
			}
		}

		public string FieldId { get; }
		public Func<TRequest, int?> AccessPropertyFunc { get; }

		public bool IsRequired { get; }

		public int MinimumValue { get; }
		public int MaximumValue { get; }
	}
}
