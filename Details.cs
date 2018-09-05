namespace XML_split_price
{   
        public class Details
        {
            public string InvoiceNumber;
            public string InvoiceDate;
            public string InvoicePaymentDueDate;

            public Details()
            {
            }

            public Details(string invoiceNumber, string invoiceDate, string invoicePaymentDueDate)
            {
                InvoiceNumber = invoiceNumber;
                InvoiceDate = invoiceDate;
                InvoicePaymentDueDate = invoicePaymentDueDate;
            }

            public override string ToString()
            {
                return "Nr zamowienia: " + InvoiceNumber + " \nData zakupu: " + InvoicePaymentDueDate + "\nData zamowienia: " + InvoiceDate;
            }
        }
    }
