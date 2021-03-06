﻿using System.ComponentModel.DataAnnotations;

namespace Entites.DTOs.ProductDTO
{
    public class ProductForCreationDTO
    {
        [Required(ErrorMessage = "Название продукта обязательно")]
        [StringLength(30, ErrorMessage = "Длина названия не может быыть больше 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Содержимое обязательно")]
        [StringLength(120, ErrorMessage = "Длина состава не может быть больше 120 символов")]
        public string Composition { get; set; }

        [Required(ErrorMessage = "Бесплатный напиток? Серьезно?")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Тип продукта обязателен")]
        [StringLength(10, ErrorMessage = "Все типы меньше 10 символов, серьезно")]
        public string Type { get; set; }

        [StringLength(20, ErrorMessage = "Больше 20 это норм?")]
        public string Portion { get; set; }
    }
}
