using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public abstract class FirmDetailBase
    {
        public string? FirmName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public List<string> TradingNames { get; set; } = new List<string>();
        public bool IsTradingNamesChanged { get; set; }
        public string? FirmType { get; set; }
        public string? RegisteredAddress { get; set; }
        public bool IsRegisteredAddressChanged { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; }
        public bool IsTradingSameAsRegisteredAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
    }
}
