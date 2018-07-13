using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Data.Currency
{
    public class CurrencyEntity : AuditableEntity
    {
        [Required]
        [StringLength(16)]
        public string Code { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public bool IsPrimary { get; set; }

        [Column(TypeName = "Money")]
        public decimal ExchangeRate { get; set; }

        [StringLength(16)]
        public string Symbol { get; set; }

        [StringLength(64)]
        public string CustomFormatting { get; set; }

        public virtual Core.Currency.Currency ToModel(Core.Currency.Currency currency)
        {
            currency.Code = Code;
            currency.Name = Name;
            currency.IsPrimary = IsPrimary;
            currency.ExchangeRate = ExchangeRate;
            currency.Symbol = Symbol;
            currency.CustomFormatting = CustomFormatting;
            return currency;
        }

        public virtual CurrencyEntity FromModel(Core.Currency.Currency currency)
        {
            Code = currency.Code;
            Name = currency.Name;
            IsPrimary = currency.IsPrimary;
            ExchangeRate = currency.ExchangeRate;
            Symbol = currency.Symbol;
            CustomFormatting = currency.CustomFormatting;
            return this;
        }

        public virtual void Patch(CurrencyEntity target)
        {
            target.Code = Code;
            target.Name = Name;
            target.IsPrimary = IsPrimary;
            target.ExchangeRate = ExchangeRate;
            target.Symbol = Symbol;
            target.CustomFormatting = CustomFormatting;
        }

    }
}
