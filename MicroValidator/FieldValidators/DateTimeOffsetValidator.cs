using System;
using System.Collections.Generic;

namespace MicroValidator.FieldValidators
{
	public class DateTimeOffsetValidator<TRequest> : IValidator<TRequest>
	{
		public DateTimeOffsetValidator(
			string fieldId,
			Func<TRequest, DateTimeOffset?> accessPropertyFunc,
			bool isRequired = true,
			DateTimeOffset? notBeforeTime = null,
			DateTimeOffset? notAfterTime = null)
		{
			FieldId = fieldId;
			AccessPropertyFunc = accessPropertyFunc;

			IsRequired = isRequired;

			NotBeforeTime = notBeforeTime;
			NotAfterTime = notAfterTime;
		}

		public IEnumerable<KeyValuePair<string, string>> ValidateRequest(TRequest request)
		{
			var value = AccessPropertyFunc(request);

			if (IsRequired && !value.HasValue)
			{
				yield return new KeyValuePair<string, string>(FieldId, "Value must be provided");
			}

			if (value.HasValue && NotBeforeTime.HasValue && value.Value < NotBeforeTime.Value)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Must be after {NotBeforeTime}");
			}

			if (value.HasValue && NotAfterTime.HasValue && value.Value > NotAfterTime.Value)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Must be before {NotAfterTime}");
			}
		}

		public string FieldId { get; }
		public Func<TRequest, DateTimeOffset?> AccessPropertyFunc { get; }

		public bool IsRequired { get; }

		public DateTimeOffset? NotBeforeTime { get; }
		public DateTimeOffset? NotAfterTime { get; }
	}
}
