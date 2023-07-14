﻿namespace SmartWalletClient.MVC.ResponseDtos
{
    public class BalanceHistoricResponseDto
    {
        public int Id { get; set; }
        public decimal Variation { get; set; }
        public bool IsIncome { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
