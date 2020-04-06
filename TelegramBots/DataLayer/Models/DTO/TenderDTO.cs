namespace DataLayer.Models.DTO
{
	public class TenderDTO
	{
		public string Id { get; set; }
		public string TenderId { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		public TenderValue Value { get; set; }
		public TenderProcuringEntity ProcuringEntity { get; set; }
	}

	public class TenderValue
	{
		public string Currency { get; set; }
		public double Amount { get; set; }
	}

	public class TenderProcuringEntity
	{
		public string Name { get; set; }
		public TenderContactPoint ContactPoint { get; set; }
	}

	public class TenderContactPoint
	{
		public string Telephone { get; set; }
		public string Url { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
	}
}