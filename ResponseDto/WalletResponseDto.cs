﻿

namespace VuelingExchangeManagerClient.ResponseDtos

{
    public class WalletResponseDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public CoinResponseDto Coin { get; set; } = new CoinResponseDto();
        public List<BalanceHistoricResponseDto> BalanceHistorics { get; set; } = new List<BalanceHistoricResponseDto>();
    }
}
