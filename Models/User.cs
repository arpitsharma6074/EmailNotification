namespace CosmosCrudApi.Models
{
    public class User
    {
        public string id { get; set; } = Guid.NewGuid().ToString();

        public string name { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public int age { get; set; }
    }
}