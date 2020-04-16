using System.Collections.Generic;

namespace MicroValidator.FieldValidators
{
	public interface IValidator<TRequest>
	{
		IEnumerable<KeyValuePair<string, string>> ValidateRequest(TRequest request);
	}
}
