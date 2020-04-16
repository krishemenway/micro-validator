using FluentAssertions;
using MicroValidator.FieldValidators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicroValidator.Tests.FieldValidators
{
	[TestFixture]
	public class DateTimeOffsetValidator
	{
		[SetUp]
		public void SetUp()
		{
			NotBeforeTime = null;
			NotAfterTime = null;
			GivenFieldId = "FieldId";
			GivenRequest = new FakeRequest();
		}

		[Test]
		public void ShouldReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = true;
			GivenRequest.FakeTime = null;
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage("Value must be provided");
		}

		[Test]
		public void ShouldNotReturnIsRequiredMessageWhenIsRequiredAndValueIsMissing()
		{
			GivenIsRequired = false;
			GivenRequest.FakeTime = null;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			NotAfterTime = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			GivenRequest.FakeTime = NotAfterTime.Value.AddSeconds(1);
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Must be before {NotAfterTime}");
		}

		[Test]
		public void ShouldNotReturnExceededMaximumMessageWhenValueIsPresentAndExceedsMaximum()
		{
			NotAfterTime = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			GivenRequest.FakeTime = NotAfterTime;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		[Test]
		public void ShouldReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			NotBeforeTime = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			GivenRequest.FakeTime = NotBeforeTime.Value.AddSeconds(-1);
			WhenValidatingRequest();
			ThenShouldHaveFailureMessage($"Must be after {NotBeforeTime}");
		}

		[Test]
		public void ShouldNotReturnExceededMinimumMessageWhenValueIsPresentAndExceedsMinimum()
		{
			NotBeforeTime = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			GivenRequest.FakeTime = NotBeforeTime;
			WhenValidatingRequest();
			ThenShouldNotHaveFailureMessage();
		}

		private void WhenValidatingRequest()
		{
			ThenValidationResults = new DateTimeOffsetValidator<FakeRequest>(GivenFieldId, (request) => request.FakeTime, isRequired: GivenIsRequired, notBeforeTime: NotBeforeTime, notAfterTime: NotAfterTime).ValidateRequest(GivenRequest).ToList();
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

		private DateTimeOffset? NotBeforeTime { get; set; }
		private DateTimeOffset? NotAfterTime { get; set; }

		private string GivenFieldId { get; set; }
		private FakeRequest GivenRequest { get; set; }

		private List<KeyValuePair<string, string>> ThenValidationResults { get; set; }
	}
}
