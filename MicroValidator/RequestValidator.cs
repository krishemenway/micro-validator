using MicroValidator.FieldValidators;
using System.Collections.Generic;
using System.Linq;

namespace MicroValidator
{
	public interface IRequestValidator<TRequest>
	{
		bool Validate(TRequest request, out ValidationResult validationResult);
	}

	public class RequestValidator<TRequest> : IRequestValidator<TRequest>
	{
		public RequestValidator(IReadOnlyList<IValidator<TRequest>> requestValidators = null)
		{
			_requestValidators = requestValidators ?? new List<IValidator<TRequest>>();
		}

		public bool Validate(TRequest request, out ValidationResult validationResult)
		{
			validationResult = new ValidationResult();

			foreach(var validationMessageByFieldId in _requestValidators.SelectMany(x => x.ValidateRequest(request).ToList()))
			{
				validationResult.ValidationMessagesByFieldId.TryAdd(validationMessageByFieldId.Key, validationMessageByFieldId.Value);
			}

			return validationResult.Success;
		}

		private readonly IReadOnlyList<IValidator<TRequest>> _requestValidators;
	}

	public class ValidationResult
	{
		public ValidationResult()
		{
			FailureMessage = "";
			ValidationMessagesByFieldId = new Dictionary<string, string>();
		}

		public bool Success => string.IsNullOrEmpty(FailureMessage) && (!ValidationMessagesByFieldId.Any() || ValidationMessagesByFieldId.All(message => string.IsNullOrEmpty(message.Value)));
		public string FailureMessage { get; set; }

		public Dictionary<string, string> ValidationMessagesByFieldId { get; }
	}
}
