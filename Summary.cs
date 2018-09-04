using System;

namespace XML_read
{
   
        class Summary
        {
            public double TotalNetAmount { get; }

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
