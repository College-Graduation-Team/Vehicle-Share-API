﻿using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripDriverModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendPrice { get; set; }
        public int AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver
    }
}
