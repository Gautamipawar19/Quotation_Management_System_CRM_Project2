using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Entities.Enums;
using QuotationManagementWebApi.Infrastructure.Data;

namespace QuotationManagementWebApi.DataSeeder
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(QuotationDbContext context)
        {
            await context.Database.MigrateAsync();

            await SeedTemplatesAsync(context);
            await SeedQuotationsAsync(context);
        }

        private static async Task SeedTemplatesAsync(QuotationDbContext context)
        {
            var templates = new List<QuotationTemplate>
            {
                new QuotationTemplate
                {
                    TemplateName = "Standard Corporate Template",
                    CompanyName = "ABC Technologies",
                    Email = "sales@abctech.com",
                    ContactNumber = "9876543210",
                    CompanyAddress = "Pune, Maharashtra",
                    TermsAndConditions = "Payment due within 30 days."
                },
                new QuotationTemplate
                {
                    TemplateName = "Premium Proposal Template",
                    CompanyName = "XYZ Solutions",
                    Email = "contact@xyzsolutions.com",
                    ContactNumber = "9123456780",
                    CompanyAddress = "Mumbai, Maharashtra",
                    TermsAndConditions = "Warranty applicable for 1 year."
                },
                new QuotationTemplate
                {
                    TemplateName = "Quick Sales Template",
                    CompanyName = "QuickTrade Pvt Ltd",
                    Email = "support@quicktrade.com",
                    ContactNumber = "9988776655",
                    CompanyAddress = "Bangalore, Karnataka",
                    TermsAndConditions = "Offer valid for 15 days."
                }
            };

            foreach (var template in templates)
            {
                var exists = await context.QuotationTemplates
                    .AnyAsync(t => t.TemplateName == template.TemplateName);

                if (!exists)
                {
                    context.QuotationTemplates.Add(template);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedQuotationsAsync(QuotationDbContext context)
        {
            var quotations = new List<Quotation>
            {
                new Quotation
                {
                    QuoteNumber = "QT-2026-0101",
                    LeadId = 1,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(15),
                    Status = QuoteStatus.Draft,
                    DiscountAmount = 100,
                    CreatedBy = "sakshi.lahoti",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 101900,
                    TaxAmount = 18342,
                    GrandTotal = 120142,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Laptop",
                            Quantity = 2,
                            UnitPrice = 50000,
                            Discount = 1000,
                            LineTotal = 99000
                        },
                        new QuotationLineItem
                        {
                            ItemDescription = "Mouse",
                            Quantity = 3,
                            UnitPrice = 1000,
                            Discount = 100,
                            LineTotal = 2900
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0102",
                    LeadId = 2,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(20),
                    Status = QuoteStatus.Draft,
                    DiscountAmount = 200,
                    CreatedBy = "sales.rep1",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 60000,
                    TaxAmount = 10800,
                    GrandTotal = 70600,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Monitor",
                            Quantity = 4,
                            UnitPrice = 15000,
                            Discount = 0,
                            LineTotal = 60000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0103",
                    LeadId = 3,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(10),
                    Status = QuoteStatus.Sent,
                    DiscountAmount = 50,
                    CreatedBy = "sales.rep2",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 15000,
                    TaxAmount = 2700,
                    GrandTotal = 17650,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Keyboard",
                            Quantity = 10,
                            UnitPrice = 1500,
                            Discount = 0,
                            LineTotal = 15000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0104",
                    LeadId = 4,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(18),
                    Status = QuoteStatus.Sent,
                    DiscountAmount = 150,
                    CreatedBy = "manager.user",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 40000,
                    TaxAmount = 7200,
                    GrandTotal = 47050,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Headset",
                            Quantity = 20,
                            UnitPrice = 2000,
                            Discount = 0,
                            LineTotal = 40000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0105",
                    LeadId = 5,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(12),
                    Status = QuoteStatus.Viewed,
                    DiscountAmount = 100,
                    CreatedBy = "sakshi.lahoti",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 30000,
                    TaxAmount = 5400,
                    GrandTotal = 35300,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Tablet",
                            Quantity = 3,
                            UnitPrice = 10000,
                            Discount = 0,
                            LineTotal = 30000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0106",
                    LeadId = 6,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(25),
                    Status = QuoteStatus.Viewed,
                    DiscountAmount = 500,
                    CreatedBy = "sales.rep1",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 80000,
                    TaxAmount = 14400,
                    GrandTotal = 93900,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Printer",
                            Quantity = 5,
                            UnitPrice = 16000,
                            Discount = 0,
                            LineTotal = 80000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0107",
                    LeadId = 7,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(30),
                    Status = QuoteStatus.Accepted,
                    DiscountAmount = 250,
                    CreatedBy = "sakshi.lahoti",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 50000,
                    TaxAmount = 9000,
                    GrandTotal = 58750,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Workstation",
                            Quantity = 1,
                            UnitPrice = 50000,
                            Discount = 0,
                            LineTotal = 50000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0108",
                    LeadId = 8,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(22),
                    Status = QuoteStatus.Accepted,
                    DiscountAmount = 100,
                    CreatedBy = "sales.rep2",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 25000,
                    TaxAmount = 4500,
                    GrandTotal = 29400,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Projector",
                            Quantity = 2,
                            UnitPrice = 12500,
                            Discount = 0,
                            LineTotal = 25000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0109",
                    LeadId = 9,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date,
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(5),
                    Status = QuoteStatus.Rejected,
                    DiscountAmount = 100,
                    CreatedBy = "sales.rep1",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 20000,
                    TaxAmount = 3600,
                    GrandTotal = 23500,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Headset",
                            Quantity = 10,
                            UnitPrice = 2000,
                            Discount = 0,
                            LineTotal = 20000
                        }
                    }
                },
                new Quotation
                {
                    QuoteNumber = "QT-2026-0110",
                    LeadId = 10,
                    CustomerId = null,
                    QuoteDate = DateTime.UtcNow.Date.AddDays(-20),
                    ExpiryDate = DateTime.UtcNow.Date.AddDays(-5),
                    Status = QuoteStatus.Expired,
                    DiscountAmount = 0,
                    CreatedBy = "manager.user",
                    CreatedDate = DateTime.UtcNow,
                    RevisionNumber = 1,
                    SubTotal = 30000,
                    TaxAmount = 5400,
                    GrandTotal = 35400,
                    LineItems = new List<QuotationLineItem>
                    {
                        new QuotationLineItem
                        {
                            ItemDescription = "Tablet",
                            Quantity = 3,
                            UnitPrice = 10000,
                            Discount = 0,
                            LineTotal = 30000
                        }
                    }
                }
            };

            foreach (var quotation in quotations)
            {
                var exists = await context.Quotations
                    .IgnoreQueryFilters()
                    .AnyAsync(q => q.QuoteNumber == quotation.QuoteNumber);

                if (!exists)
                {
                    context.Quotations.Add(quotation);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}