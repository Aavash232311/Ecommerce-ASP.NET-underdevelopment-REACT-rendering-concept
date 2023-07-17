namespace restaurant_franchise.Models
{
    public class Product
    {
        public Guid Id {get; set;}
        public string name { get; set; } = string.Empty;
        public decimal discount_amount { get; set; } = 0;
        public int price {get; set;} = 0;
        public string main_image { get; set; } = string.Empty;
        public string cover_0 { get; set; } = string.Empty;
        public string cover_1 { get; set; } = string.Empty;
        public string cover_2 { get; set; } = string.Empty;
        public string cover_3 { get; set; } = string.Empty;
        public string cover_4 { get; set; } = string.Empty;
        public string description {get; set;} = string.Empty;
        public DateTime discount_valid_date {get; set;}
        public string product_condition {get; set;} = string.Empty;
        public User user {get; set;} = new User();
        public DateTime addedDate {get; set;} = DateTime.Now;
        public Category related_tags {get; set;} = new Category();
    }
}