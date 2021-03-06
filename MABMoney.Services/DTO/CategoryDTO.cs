﻿using System.ComponentModel.DataAnnotations;

namespace MABMoney.Services.DTO
{
    public class CategoryDTO
    {
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Account_AccountID { get; set; }
        public string AccountName { get; set; }
        [Required]
        public CategoryTypeDTO Type { get; set; }
    }
}
