namespace TestAPI.Model
{
	public class VueInfo
	{
		public int id { get; set; }
		public string name { get; set; }
		public string designation { get; set; }
	}
	public class vueInfoRequest
	{
		public string name { get; set; }
		public string designation { get; set; }
	}
	public class prod_get
	{
		public int id { get; set; }
		public IFormFile formFile { get; set; }
	}
	public class login_Request
	{
		public int id { get; set; }
		public string name { get; set; }
		public string password { get; set; }
	}

	public class StoredProcedureInfo
	{
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Text { get; set; }
	}

}
