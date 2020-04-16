using FluentAssertions;
using MicroValidator.FieldValidators;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace MicroValidator.Tests
{
	[TestFixture]
	public class RequestValidatorTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRequest = new FakeRequest();

			GivenValidatorOneResults = new List<KeyValuePair<string, string>>();
			GivenValidatorTwoResults = new List<KeyValuePair<string, string>>();

			GivenValidatorOne = new Mock<IValidator<FakeRequest>>();
			GivenValidatorOne.Setup(x => x.ValidateRequest(GivenRequest)).Returns(GivenValidatorOneResults);

			GivenValidatorTwo = new Mock<IValidator<FakeRequest>>();
			GivenValidatorTwo.Setup(x => x.ValidateRequest(GivenRequest)).Returns(GivenValidatorTwoResults);

			GivenRequestValidators = new List<IValidator<FakeRequest>>()
			{
				GivenValidatorOne.Object,
				GivenValidatorTwo.Object,
			};

			_requestValidator = new RequestValidator<FakeRequest>(GivenRequestValidators);
		}

		[Test]
		public void ShouldCombineValidatorResultsUsingTheFirstMessageAddedPerFieldId()
		{
			GivenValidatorOneResults.Add(new KeyValuePair<string, string>("FieldOne", "FieldOne Message 1"));
			GivenValidatorOneResults.Add(new KeyValuePair<string, string>("FieldOne", "FieldOne Message 2"));

			GivenValidatorTwoResults.Add(new KeyValuePair<string, string>("FieldTwo", "FieldTwo Message 1"));
			GivenValidatorTwoResults.Add(new KeyValuePair<string, string>("FieldTwo", "FieldTwo Message 2"));

			WhenValidatingRequest();

			ThenIsValid.Should().BeFalse();
			ThenValidationResult.Success.Should().BeFalse();
			ThenValidationResult.ValidationMessagesByFieldId.Should().Contain("FieldOne", "FieldOne Message 1");
			ThenValidationResult.ValidationMessagesByFieldId.Should().Contain("FieldTwo", "FieldTwo Message 1");
		}

		[Test]
		public void ShouldReturnSuccessResultWhenValidatorResultIsPresentWithEmptyMessage()
		{
			GivenValidatorOneResults.Add(new KeyValuePair<string, string>("FieldOne", ""));
			GivenValidatorTwoResults.Add(new KeyValuePair<string, string>("FieldTwo", ""));

			WhenValidatingRequest();

			ThenIsValid.Should().BeTrue();
			ThenValidationResult.Success.Should().BeTrue();
			ThenValidationResult.ValidationMessagesByFieldId.Should().Contain("FieldOne", "");
			ThenValidationResult.ValidationMessagesByFieldId.Should().Contain("FieldTwo", "");
		}

		[Test]
		public void ShouldReturnSuccessResultWhenNoValidatorResultsAreAdded()
		{
			GivenValidatorOneResults.Clear();
			GivenValidatorTwoResults.Clear();

			WhenValidatingRequest();

			ThenIsValid.Should().BeTrue();
			ThenValidationResult.Success.Should().BeTrue();
			ThenValidationResult.ValidationMessagesByFieldId.Should().BeEmpty();
		}

		private void WhenValidatingRequest()
		{
			ThenIsValid = _requestValidator.Validate(GivenRequest, out var validationResult);
			ThenValidationResult = validationResult;
		}

		private List<KeyValuePair<string, string>> GivenValidatorOneResults { get; set; }
		private List<KeyValuePair<string, string>> GivenValidatorTwoResults { get; set; }

		private Mock<IValidator<FakeRequest>> GivenValidatorOne { get; set; }
		private Mock<IValidator<FakeRequest>> GivenValidatorTwo { get; set; }

		private List<IValidator<FakeRequest>> GivenRequestValidators { get; set; }
		private FakeRequest GivenRequest { get; set; }

		private bool ThenIsValid { get; set; }
		private ValidationResult ThenValidationResult { get; set; }

		private RequestValidator<FakeRequest> _requestValidator;
	}
}
