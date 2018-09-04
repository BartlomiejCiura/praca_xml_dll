using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XML_read
{
   
        static Invoice GetSplittedListProducts(string XMLPath)
        {
            List<Product> productsList = new List<Product>();

            XDocument document = LoadDocument(XMLPath);

            //---------------DETAILS
            var details = from d in document.Descendants("Invoice-Header")
                          select new
                          {
                              InvoiceNumber = d.Element("InvoiceNumber").Value,
                              InvoiceDate = d.Element("InvoiceDate").Value,
                              InvoicePaymentDueDate = d.Element("InvoicePaymentDueDate").Value,
                          };

            foreach (var d in details)
            {
                Details detailsInfo = new Details(d.InvoiceNumber, d.InvoiceDate, d.InvoicePaymentDueDate);
                Console.WriteLine(detailsInfo.ToString());
            };
            
            //----------------SELLER
            var seller = from s in document.Descendants("Seller")
                         select new
                         {
                             TaxID = s.Element("TaxID").Value,
                         };
            foreach (var s in seller)
            {
                Seller sellerDetails = new Seller(s.TaxID);
                Console.WriteLine(sellerDetails.ToString());
            }

            //-----------------SUMMARY
            var summary = from s in document.Descendants("Invoice-Summary")
                          select new
                          {
                              TotalNetAmount = s.Element("TotalNetAmount").Value,
                          };

            Summary sum = new Summary();
            foreach (var item in summary)
            {
                sum = new Summary(Convert.ToDouble(item.TotalNetAmount));
            }


            // ------------- PRODUCTS
            var products = from r in document.Descendants("Line-Item")
                           select new
                           {
                               ItemDescription = r.Element("ItemDescription").Value,
                               InvoiceQuantity = r.Element("InvoiceQuantity").Value,
                               InvoiceUnitPacksize = r.Element("InvoiceUnitPacksize").Value,
                               InvoiceUnitNetPrice = r.Element("InvoiceUnitNetPrice").Value,
                               TaxRate = r.Element("TaxRate").Value,
                           };


            foreach (var r in products)
            {
                productsList.Add(new Product(r.ItemDescription, Convert.ToDouble(r.InvoiceQuantity), Convert.ToDouble(r.InvoiceUnitPacksize), Convert.ToDouble(r.InvoiceUnitNetPrice), Convert.ToDouble(r.TaxRate)));
            }

            List<Product> wynik = new List<Product>();
            foreach (Product item in productsList)
            {
                List<Product> splitedProduct = CalculatePrice(item);
                wynik.AddRange(splitedProduct);
                
            }



            double total = 0;// CalculateTotal(productsList);
            foreach (Product item in wynik)
            {
                total += item.InvoiceSingleQuantity * item.InvoiceSingleNetPrice;
                
            }

            //foreach  ( Product p in wynik)
            //{
            //    Console.WriteLine(p.ToString());
            //}

           // Console.WriteLine("Total: " + total);
            double wyn = Math.Round(total, 2);
            //Console.WriteLine(sum);

            try
            {
                if ( Math.Abs(wyn - sum.TotalNetAmount) > 0.001)        
                {
                    throw new Exception("UWAGA!!!! Dane dane się różnią!!!!!!!");
                }
                else
                    Console.WriteLine("Swietnie, wszystko sie zgadza");

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Invoice invoice = new Invoice();
            invoice.products = wynik;
            invoice.details = (Details) details.Take(0);
            invoice.seller = (Seller)seller.Take(0);
            invoice.ivoiceDate = (Details)details.Take(1);
            invoice.paymentDueDate = (Details)details.Take(2);
            invoice.summary = (Summary)summary.Take(0); 


            return invoice;
        }

        private static List<Product> CalculatePrice(Product item)
        {

            double wartoscZaokr = item.InvoiceQuantity * item.InvoiceUnitPacksize * item.GetInvoiceSingleNetPrice();
            double wartReal = item.InvoiceQuantity * item.InvoiceUnitNetPrice;
            double r = Math.Abs(wartoscZaokr - wartReal);
            int ilosc = (int)Math.Round(r * 100);

            Product p1 = new Product(item.ItemDescription, item.InvoiceQuantity, item.InvoiceUnitPacksize, item.InvoiceUnitNetPrice, item.TaxRate);
            p1.InvoiceSingleQuantity = ilosc;

            p1.InvoiceSingleNetPrice += 0.01;

            Product p2 = item;
            p2.InvoiceSingleQuantity = (int)(item.InvoiceQuantity * item.InvoiceUnitPacksize) - ilosc;
            List<Product> ret = new List<Product>();
            ret.Add(p1);
            ret.Add(p2);

            return ret;
        }

        private static XDocument LoadDocument(string path)
        {
            return XDocument.Load(path);
        }

        private static double CalculateTotal(List<Product> productsList)
        {
            return productsList.Sum(item => item.GetInvoiceUnitAllSingleNetPrice());
        }
    
}
