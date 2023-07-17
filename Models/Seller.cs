namespace restaurant_franchise.Models {
    public class Seller {
        public Guid Id {get; set;}
        public string ShopName {get; set; } = string.Empty;
        public int PhoneNumber {get; set; } 
        public string AboutBusiness {get; set; } = string.Empty;
        public string ShopAddress {get; set;} = string.Empty;
        public string ProfileImage {get; set;} = string.Empty;
        public User User {get; set; }
    } 
}