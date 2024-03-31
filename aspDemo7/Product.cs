﻿//Demo 7 - Shopping Cart; LV
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo7.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int ProductPK { get; set; }

    [Display(Name = "Category")]
    public int CategoryFK { get; set; }

    [Display(Name = "Subcategory")]
    public int SubCategoryFK { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Model Number")]
    public string ModelNumber { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Product")]
    public string ModelName { get; set; }

    [StringLength(50)]
    [Display(Name = "Image")]
    [RegularExpression(@"^[\w-]{1,36}.jpg$", ErrorMessage = "Please enter a valid jpg file name")]
    public string ProductImage { get; set; }

    [Column(TypeName = "money")]
    [Display(Name = "Price")]
    [Range(2, 1000, ErrorMessage = "Please enter an amount between 2 and 1000")]
    public decimal UnitCost { get; set; }

    [Required]
    [Column(TypeName = "ntext")]
    public string Description { get; set; }

    [StringLength(50)]
    [Display(Name = "Image")]
    [RegularExpression(@"^[\w-]{1,36}.gif$", ErrorMessage = "Please enter a valid gif file name")]
    public string Thumbnail { get; set; }

    [StringLength(50)]
    public string Availability { get; set; }

    [ForeignKey("CategoryFK")]
    [InverseProperty("Products")]
    [Display(Name = "Category")]
    public virtual Category CategoryFKNavigation { get; set; }

    [ForeignKey("SubCategoryFK")]
    [InverseProperty("Products")]
    [Display(Name = "SubCategory")]
    public virtual SubCategory SubCategoryFKNavigation { get; set; }

    [InverseProperty("ProductFKNavigation")]
    public virtual ICollection<tblOrderDetail> tblOrderDetails { get; set; } = new List<tblOrderDetail>();
}