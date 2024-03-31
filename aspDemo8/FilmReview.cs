﻿// Demo 8 - Complete Application; LV
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo8.Models;

[Table("FilmReview")]
public partial class FilmReview
{
    [Key]
    public int ReviewPK { get; set; }

    [Column(TypeName = "datetime")]
    [Display(Name = "Review Date")]
    public DateTime ReviewDate { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Review Summary")]
    [RegularExpression(@"^[a-zA-Z][\w\s\.]*[a-zA-Z\.]$", ErrorMessage = "Invalid characters")]
    public string ReviewSummary { get; set; }

    [Required]
    [Display(Name = "Star Rating (1-10)")]
    [Range(1, 10)]
    public short ReviewRating { get; set; }

    [Column(TypeName = "datetime")]
    [Display(Name = "Update Date")]
    public DateTime? ReviewUpdateDate { get; set; }

    public int FilmFK { get; set; }

    public int ContactFK { get; set; }

    [ForeignKey("ContactFK")]
    [InverseProperty("FilmReviews")]
    [Display(Name = "Reviewer")]
    public virtual Contact ContactFKNavigation { get; set; }

    [ForeignKey("FilmFK")]
    [InverseProperty("FilmReviews")]
    public virtual Film FilmFKNavigation { get; set; }
}