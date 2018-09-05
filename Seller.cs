namespace XML_split_price
{    
    public class Seller
        {
            private string TaxID;

            public Seller(string taxID)
            {
                TaxID = taxID;
            }

            public override string ToString()
            {
                return "Nip sprzedawcy: " + TaxID + "\n";
            }
        }   
}
