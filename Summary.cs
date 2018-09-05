using System;

namespace XML_split_price
{
   
        public class Summary
        {
        public double TotalNetAmount;

            public Summary(double totalNetAmount)
            {
                TotalNetAmount = Convert.ToDouble(totalNetAmount);
            }

            public Summary()
            {
            }

            public override string ToString()
            {
                return "Suma netto z dokumentu: " + TotalNetAmount;
            }
        }
    
}
