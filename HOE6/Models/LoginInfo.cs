﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HandOnEx6.Models;

[Table("LoginInfo")]
public partial class LoginInfo
{
    [Key]
    public int UserPK { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Upper and Lower Case Letters")]
    [DisplayAttribute(Name = "Username")]
    public string Uname { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9\*\$]+$", ErrorMessage = "Letters, Digits, *, $")]
    [DisplayAttribute(Name = "Password")]
    [UIHint("password")]
    public string UPass { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z]+\s[a-zA-Z]+$", ErrorMessage = "First and Last Name; Upper and Lower Case letters")]
    [DisplayAttribute(Name = "First Name", Prompt = "First and last Name")]
    public string FullName { get; set; }

    //[Required]
    [StringLength(50)]
    public string URole { get; set; }

    [InverseProperty("CustomerFKNavigation")]
    public virtual ICollection<tblOrder> tblOrders { get; set; } = new List<tblOrder>();
}