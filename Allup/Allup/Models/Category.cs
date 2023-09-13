﻿using System.ComponentModel.DataAnnotations;

namespace Allup.Models
{
    public class Category : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [Display(Name ="Is A Top Category")]
        public bool IsMain { get; set; }
        [Display(Name ="Select A Top Category")]
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }

        public IEnumerable<Category>? Children { get; set; }
        public IEnumerable<Product>? Products { get; set; }


    }
}
