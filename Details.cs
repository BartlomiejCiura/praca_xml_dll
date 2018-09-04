namespace XML_read
{   
        class Details
        {
            private string InvoiceNumber;
            private string InvoiceDate;
            private string InvoicePaymentDueDate;

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
