using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public class OrderType
    {
        public string DisplayName;
        public string Code;
        public string AlternativeName;
        private OrderType(string displayName, string code)
        {
            DisplayName = displayName;
            Code = code;
        }
        private OrderType(string displayName, string code, string alternativeName)
        {
            DisplayName = displayName;
            Code = code;
            AlternativeName = alternativeName;
        }

        public static readonly OrderType Manual = new OrderType("지정가", "010");
        public static readonly OrderType Market = new OrderType("시장가", "020");
        public static readonly OrderType MOO = new OrderType("MOO", "720", "장개시 시장가");
        public static readonly OrderType MOC = new OrderType("MOC", "740", "장마감 시장가");
        public static readonly OrderType LOO = new OrderType("LOO", "710", "장개시 지정가");
        public static readonly OrderType LOC = new OrderType("LOC", "730", "장마감 지정가");
        public static readonly OrderType TWAP = new OrderType("TWAP", "750", "시간분할주문");
        public static readonly OrderType VWAP = new OrderType("VWAP", "760", "수량분할주문");

    }

}
