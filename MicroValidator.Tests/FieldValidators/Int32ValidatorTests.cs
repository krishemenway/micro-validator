using FluentAssertions;
using MicroValidator.FieldValidators;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MicroValidator.Tests.FieldValidators
{
	[TestFixture]
	public class Int32ValidatorTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenMinimumValue = 0;
			GivenMaximumValue = 100;
			GivenFieldId = "FieldId";
			GivenRequest = new FakeRequest();
		}

		[Test]
		public void ShouldReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = true;
			GivenRequest.FakeInt = null;
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage("Value must be provided");
		}

		[Test]
		public void ShouldNotReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = false;
			GivenRequest.FakeInt = null;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			GivenMaximumValue = 100;
			GivenRequest.FakeInt = 101;
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Value must be less than {GivenMaximumValue}");
		}

		[Test]
		public void ShouldNotReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			GivenMaximumValue = 100;
			GivenRequest.FakeInt = 100;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			GivenMinimumValue = 100;
			GivenRequest.FakeInt = 99;
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Value must be greater than {GivenMinimumValue}");
		}

		[Test]
		public void ShouldNotReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			GivenMinimumValue = 100;
			GivenRequest.FakeInt = 100;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		private void WhenValidatingRequest()
		{
			ThenValidationResults = new Int32Validator<FakeRequest>(GivenFieldId, (request) => request.FakeInt, isRequired: GivenIsRequired, minimumValue: GivenMinimumValue, maximumValue: GivenMaximumValue).ValidateRequest(GivenRequest).ToList();
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

		private int GivenMinimumValue { get; set; }
		private int GivenMaximumValue { get; set; }

		private string GivenFieldId { get; set; }
		private FakeRequest GivenRequest { get; set; }

		private List<KeyValuePair<string, string>> ThenValidationResults { get; set; }
	}
}
