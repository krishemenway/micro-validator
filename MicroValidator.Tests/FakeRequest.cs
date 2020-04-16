using System;

namespace MicroValidator.Tests
{
	public class FakeRequest
	{
		public string FakeString { get; set; } = "FakeString";
		public int? FakeInt { get; set; } = 0;
		public long? FakeLong { get; set; } = 0;
		public decimal? FakeDecimal { get; set; } = 0;
		public DateTimeOffset? FakeTime { get; set; } = null;
	}
}
