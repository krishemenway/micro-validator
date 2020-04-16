using FluentAssertions;
using MicroValidator.FieldValidators;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicroValidator.Tests.FieldValidators
{
	[TestFixture]
	public class StringValidatorTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRegex = null;
			GivenMinimumCharacterLength = 0;
			GivenMaximumCharacterLength = 100;
			GivenFieldId = "FieldId";
			GivenRequest = new FakeRequest();
		}

		[Test]
		public void ShouldReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = true;
			GivenRequest.FakeString = null;
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage("Value must be provided");

			GivenRequest.FakeString = "";
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage("Value must be provided");
		}

		[Test]
		public void ShouldNotReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = false;
			GivenRequest.FakeString = null;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();

			GivenRequest.FakeString = "";
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			GivenMaximumCharacterLength = 100;
			GivenRequest.FakeString = new string('a', GivenMaximumCharacterLength + 1);
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Must be between {GivenMinimumCharacterLength} and {GivenMaximumCharacterLength} characters");
		}

		[Test]
		public void ShouldNotReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			GivenMaximumCharacterLength = 100;
			GivenRequest.FakeString = new string('a', GivenMaximumCharacterLength);
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			GivenMinimumCharacterLength = 100;
			GivenRequest.FakeString = new string('a', GivenMinimumCharacterLength - 1);
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Must be between {GivenMinimumCharacterLength} and {GivenMaximumCharacterLength} characters");
		}

		[Test]
		public void ShouldNotReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			GivenMinimumCharacterLength = 100;
			GivenRequest.FakeString = new string('a', GivenMinimumCharacterLength);
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnInvalidFormatMessageWhenValueIsPresentAndRegexIsSupplied()
		{
			GivenRegex = new Regex("[a-z]+", RegexOptions.IgnoreCase);
			GivenRequest.FakeString = "Some_String";
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage("Format is invalid");
		}

		private void WhenValidatingRequest()
		{
			ThenValidationResults = new StringValidator<FakeRequest>(GivenFieldId, (request) => request.FakeString, isRequired: GivenIsRequired, minimumCharacterLength: GivenMinimumCharacterLength, maximumCharacterLength: GivenMaximumCharacterLength, matchesRegularExpression: GivenRegex).ValidateRequest(GivenRequest).ToList();
		}

		private void ThenShouldHaveFailureMessage(string message)
		{
			ThenValidationResults.Should().Contain(new KeyValuePair<string, string>(GivenFieldId, message));
		}

		private void ThenShouldNotHaveFailureMessage()
		{
			ThenValidationResults.Should().NotContain(x => x.Key == GivenFieldId);
		}

		private bool GivenIsRequired { get; set; }
		private Regex GivenRegex { get; set; }
		private int GivenMinimumCharacterLength { get; set; }
		private int GivenMaximumCharacterLength { get; set; }

		private string GivenFieldId { get; set; }
		private FakeRequest GivenRequest { get; set; }

		private List<KeyValuePair<string, string>> ThenValidationResults { get; set; }
	}
}
