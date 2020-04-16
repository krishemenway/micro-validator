using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MicroValidator.FieldValidators
{
	public class StringValidator<TRequest> : IValidator<TRequest>
	{
		public StringValidator(
			string fieldId,
			Func<TRequest, string> accessPropertyFunc,
			bool isRequired = true,
			int minimumCharacterLength = 0,
			int maximumCharacterLength = int.MaxValue,
			bool ignoreLeadingWhitespace = true,
			bool ignoreTrailingWhitespace = true,
			Regex matchesRegularExpression = null)
		{
			FieldId = fieldId;
			AccessPropertyFunc = accessPropertyFunc;

			IsRequired = isRequired;

			MinimumCharacterLength = minimumCharacterLength;
			MaximumCharacterLength = maximumCharacterLength;

			IgnoreLeadingWhitespace = ignoreLeadingWhitespace;
			IgnoreTrailingWhitespace = ignoreTrailingWhitespace;

			MatchesRegularExpression = matchesRegularExpression;
		}

		public IEnumerable<KeyValuePair<string, string>> ValidateRequest(TRequest request)
		{
			var value = AccessPropertyFunc(request) ?? "";

			if (IgnoreLeadingWhitespace)
			{
				value = value.TrimStart();
			}

			if (IgnoreTrailingWhitespace)
			{
				value = value.TrimEnd();
			}

			if (IsRequired && string.IsNullOrEmpty(value))
			{
				yield return new KeyValuePair<string, string>(FieldId, "Value must be provided");
			}

			if (value != null && value.Length < MinimumCharacterLength)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Must be between {MinimumCharacterLength} and {MaximumCharacterLength} characters");
			}

			if (value != null && value.Length > MaximumCharacterLength)
			{
				yield return new KeyValuePair<string, string>(FieldId, $"Must be between {MinimumCharacterLength} and {MaximumCharacterLength} characters");
			}

			if (value != null && MatchesRegularExpression != null && MatchesRegularExpression.IsMatch(value))
			{
				yield return new KeyValuePair<string, string>(FieldId, "Format is invalid");
			}
		}

		public string FieldId { get; }
		public Func<TRequest, string> AccessPropertyFunc { get; }

		public bool IsRequired { get; }

		public bool IgnoreLeadingWhitespace { get; }
		public bool IgnoreTrailingWhitespace { get; }

		public int MinimumCharacterLength { get; }
		public int MaximumCharacterLength { get; }

		public Regex MatchesRegularExpression { get; }
	}
}
