using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XML_split_price
{
    public partial class XML_split_price
    {
        /// <summary>
        /// Metoda pobiera ścieżkę do pliku xml i zwraca listę z danymi: nr fv, nip, data wystawienia, data płatności, nazwa, ilość, cena netto, vat, total.
        /// Lista jest podzielona aby unikąć różnicy w cene total. 
        /// </summary>
        /// <param name="XMLPath"></param>
        /// <returns></returns>
        public static Invoice GetSplittedListProducts(string XMLPath)
        {

            Console.WriteLine("testyetsds");
            Invoice invoice = new Invoice();
           
                List<Product> listProducts = new List<Product>();

                XDocument document = LoadDocument(XMLPath);

                List<Details> listDetails = new List<Details>();

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
                    listDetails.Add(detailsInfo);
                };

                List<Seller> listSellers = new List<Seller>();

                //----------------SELLER
                var seller = from s in document.Descendants("Seller")
                             select new
                             {
                                 TaxID = s.Element("TaxID").Value,
                             };
                foreach (var s in seller)
                {
                    Seller sellerDetails = new Seller(s.TaxID);
                    listSellers.Add(sellerDetails);
                }


                List<Summary> listSummaries = new List<Summary>();
                //-----------------SUMMARY
                var summary = from s in document.Descendants("Invoice-Summary")
                              select new
                              {
                                  TotalNetAmount = s.Element("TotalNetAmount").Value,
                              };

                Summary sum = new Summary();
                double x = 0;
                foreach (var item in summary)
                {
                //Console.WriteLine(item.TotalNetAmount);
                Double.TryParse(item.TotalNetAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                sum = new Summary(x);
                    listSummaries.Add(sum);
                
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

                double iq, iup, iunp, tr = 0;
                foreach (var r in products)
                {
                Double.TryParse(r.InvoiceQuantity, NumberStyles.Any, CultureInfo.InvariantCulture, out iq); 
                Double.TryParse(r.InvoiceUnitPacksize, NumberStyles.Any, CultureInfo.InvariantCulture, out iup); 
                Double.TryParse(r.InvoiceUnitNetPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out iunp);
                Double.TryParse(r.TaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out tr);
                listProducts.Add(new Product(r.ItemDescription, iq, iup, iunp, tr));
                }

                List<Product> wynik = new List<Product>();
                foreach (Product item in listProducts)
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
                    if (Math.Abs(wyn - sum.TotalNetAmount) > 0.001)
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

           
                invoice.products = wynik;
               
               //object t = details.ElementAt(0);
                invoice.details = listDetails[0];
                invoice.seller = listSellers[0];
                //invoice.ivoiceDate = listDetails[1];
                //invoice.paymentDueDate = listDetails[2];
                invoice.summary = listSummaries[0];


            

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
}
