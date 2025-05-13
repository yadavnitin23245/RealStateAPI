using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealState.BAL.DTO
{
    public class ContactStatDTO
    {
        public string? InquiryReceived { get; set; }
        public string? AnsweredInquiry { get; set; }

        public string? UnAnsweredInquiry { get; set; }

        public string? pendingResponses { get; set; }
        public string? propertiesSold { get; set; }
        public string? monthlyGrowth { get; set; }

    }
}
