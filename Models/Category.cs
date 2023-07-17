namespace restaurant_franchise.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


public class Category
{
    public Guid Id { get; set; }

    public Guid? CategoryKey { get; set; } // Optional foreign key property
    public Category? Parent { get; set; } // Optional reference navigation to principal
    public string ProductCategory {get ; set;} = string.Empty;
    public ICollection<Category> Child { get; set; } = new List<Category>(); // Collection navigation containing dependents
    public DateTime createdAt {get; set;} = DateTime.Now;
}