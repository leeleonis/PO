using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Xml.Serialization;
using CarrierApi.DHL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.DHLApi
{
    public class DHLData
    {
        public int TransferID { get; set; }
        public string Name { get; set; }
        public int MethodType { get; set; }
        public int BoxType { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 匯率
        /// </summary>
        public decimal EXRate { get; set; }
        public Warehouse WinitWarehouse { get; set; }
        public List<WinitTransferBox> WinitTransferBoxList { get; set; }
    }
    public class DHL_API
    {
        private string api_siteID;
        private string api_password;
        private string api_account;
        protected static string LangID = "en-US";
        private DateTime today = DateTime.Now;

        public DHL_API(ApiSetting Api)
        {
            api_siteID = Api.ApiKey;
            api_password = Api.ApiPassword;
            api_account = Api.ApiAccount;
        }
        /// <summary>
        /// 追蹤提單
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public TrackingResponse Tracking(string trackingNumber)
        {
            TrackingResponse result;
            KnownTrackingRequest track = SetTracking(new string[] { trackingNumber });

            XmlSerializer serializer = new XmlSerializer(typeof(TrackingResponse));
            string request = SendRequest(track);
            using (TextReader reader = new StringReader(request))
            {
                try
                {
                    result = (TrackingResponse)serializer.Deserialize(reader);
                }
                catch (Exception e)
                {
                    TextReader errorReader = new StringReader(request);
                    XmlSerializer errorSerializer = new XmlSerializer(typeof(ShipmentTrackingErrorResponse));
                    ShipmentTrackingErrorResponse error = errorSerializer.Deserialize(errorReader) as ShipmentTrackingErrorResponse;
                    errorReader.Dispose();

                    result = new TrackingResponse()
                    {
                        AWBInfo = new AWBInfo[] {
                            new AWBInfo() { Status = new Status() { ActionStatus = string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData)) } }
                        }
                    };
                }
            }

            return result;
        }

        private KnownTrackingRequest SetTracking(string[] items)
        {
            var tracking = new KnownTrackingRequest()
            {
                Request = new Request()
                {
                    ServiceHeader = new ServiceHeader()
                    {
                        SiteID = api_siteID,
                        Password = api_password,
                        MessageReference = "Esteemed Courier Service of DHL",
                        MessageTime = today
                    }
                },
                LanguageCode = "tw",
                Items = items,
                ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.AWBNumber },
                LevelOfDetails = LevelOfDetails.ALL_CHECK_POINTS,
                PiecesEnabled = KnownTrackingRequestPiecesEnabled.S,
                PiecesEnabledSpecified = true,
                schemaVersion = 1.0M
            };

            return tracking;
        }
        /// <summary>
        /// 建立提單
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public ShipmentResponse Create(DHLData DHLData)
        {
            ShipmentResponse result;
            ShipmentRequest shipmentRequest = SetShipment(DHLData);

            //MyHelp.Log("Packages", package.ID, "執行DHL Create Api");

            XmlSerializer serializer = new XmlSerializer(typeof(ShipmentResponse));
            string request = SendRequest(shipmentRequest);
            using (TextReader reader = new StringReader(request))
            {
                try
                {
                    result = (ShipmentResponse)serializer.Deserialize(reader);
                }
                catch (Exception e)
                {
                    string errorMsg;
                    TextReader errorReader = new StringReader(request);
                    XmlSerializer errorSerializer = new XmlSerializer(typeof(ShipmentValidateErrorResponse));
                    try
                    {
                        ShipmentValidateErrorResponse error = errorSerializer.Deserialize(errorReader) as ShipmentValidateErrorResponse;
                        errorMsg = string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData));
                    }
                    catch (Exception)
                    {
                        errorSerializer = new XmlSerializer(typeof(OtherErrorResponse));
                        OtherErrorResponse error = errorSerializer.Deserialize(new StringReader(request)) as OtherErrorResponse;
                        errorMsg = string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData));
                    }
                    errorReader.Dispose();

                    //MyHelp.Log("Packages", package.ID, string.Format("執行DHL Create Api失敗 - {0}", errorMsg));
                    throw new Exception(errorMsg);
                }
            }

            return result;
        }
        /// <summary>
        /// 設定提單內容
        /// </summary>
        /// <param name="DHLData"></param>
        /// <returns></returns>
        private ShipmentRequest SetShipment(DHLData DHLData)
        {
            //MyHelp.Log("Packages", package.ID, "設定Create Data");

            var shipment = new ShipmentRequest();

            shipment.Request = RequsetInit("6.1");//版本
            shipment.LanguageCode = "tw";//
            shipment.PiecesEnabled = PiecesEnabled.Y;
            shipment.Reference = new Reference[] { new Reference() { ReferenceID = DHLData.TransferID.ToString() } };//參考資料
            shipment.LabelImageFormat = LabelImageFormat.PDF;
            shipment.LabelImageFormatSpecified = true;
            shipment.RequestArchiveDoc = YesNo.Y;
            shipment.RequestArchiveDocSpecified = true;
            shipment.Label = new Label() { LabelTemplate = LabelTemplate.Item8X4_A4_PDF };
            shipment.EProcShip = YesNo.N;
            shipment.EProcShipSpecified = true;

            shipment.Billing = new Billing()
            {
                ShipperAccountNumber = api_account,
                ShippingPaymentType = ShipmentPaymentType.S,
                BillingAccountNumber = api_account,
                DutyPaymentType = DutyTaxPaymentType.S,
                DutyPaymentTypeSpecified = true,
                DutyAccountNumber = api_account
            };

            shipment.Shipper = new Shipper()//寄送者資訊
            {
                ShipperID = api_account,
                CompanyName = "Zhi You Wan LTD",
                AddressLine = new string[] { "No.51, Sec.3 Jianguo N. Rd.,", "South Dist.," },
                City = "Taichung City",
                PostalCode = "403",
                CountryCode = "TW",
                CountryName = "Taiwan",
                Contact = new Contact() { PersonName = "Huai Wei Ho", PhoneNumber = "0423718118" }
            };
            Contact contact = new Contact()
            {
                PersonName = DHLData.WinitWarehouse.Company,
                PhoneNumber = DHLData.WinitWarehouse.Phone
            };

            shipment.Consignee = new Consignee()
            {
                CompanyName =  contact.PersonName,
                AddressLine = new string[] { DHLData.WinitWarehouse.Address1, DHLData.WinitWarehouse.Address2 },
                City = DHLData.WinitWarehouse.City,
                Division = DHLData.WinitWarehouse.State,
                PostalCode = DHLData.WinitWarehouse.Postcode,
                CountryCode = DHLData.WinitWarehouse.WinitWarehouse,
                CountryName = DHLData.WinitWarehouse.Country,
                Contact = contact
            };

            decimal weight = shipment.Shipper.CountryCode.Equals("US") ? 453 : 1000;//
            WeightUnit weightUnit = shipment.Shipper.CountryCode.Equals("US") ? WeightUnit.L : WeightUnit.K;
            var DeclaredValue = 0m;
            List<Piece> pieceList = new List<Piece>();//箱數列表
            var PieceID = 1;
            foreach (var WinitTransferBox in DHLData.WinitTransferBoxList)
            {
                var itemList = WinitTransferBox.WinitTransferBoxItem;
                var Weight = itemList.Sum(i => i.Weight).Value / weight;//單件的重量, 如果是US的話要轉成磅
                DeclaredValue += itemList.Sum(y => y.Value).Value;
                pieceList.Add(new Piece()//單件的資訊
                {
                    //PieceID= PieceID.ToString(),
                    PackageType = PackageType.YP,
                    PackageTypeSpecified = true,
                    Weight = Weight,
                    WeightSpecified = true,
                    PieceContents = "BOX" + PieceID,//內容清單
                    //DimWeight = Weight,
                    //Width= WinitTransferBox.Width?.ToString("F0"),
                    //Height = WinitTransferBox.Heigth?.ToString("F0"),
                    //Depth = WinitTransferBox.Length?.ToString("F0"),
                });
                PieceID++;
            }

            shipment.ShipmentDetails = new ShipmentDetails()
            {
                 
                NumberOfPieces = pieceList.Count().ToString(),//件數
                Pieces = pieceList.ToArray(),//
                Weight = pieceList.Sum(p => p.Weight),//總重量, 如果是US的話要轉成磅
                WeightUnit = weightUnit,//重量單位，
                GlobalProductCode = "P",
                LocalProductCode = "P",
                Date = today,
                Contents = string.Join(", ", pieceList.Select(x=>x.PieceContents)),
                DoorTo = DoorTo.DD,//
                DoorToSpecified = true,
                DimensionUnit = DimensionUnit.C,
                DimensionUnitSpecified = true,
                PackageType = PackageType.YP,
                PackageTypeSpecified = true,
                IsDutiable = YesNo.Y,//應稅
                IsDutiableSpecified = true,
                CurrencyCode = DHLData.Currency//貨幣代碼
            };

            shipment.Dutiable = new Dutiable()
            {
                DeclaredValue = 0,//總價值
                DeclaredValueSpecified = true,
                DeclaredCurrency = DHLData.Currency,
                TermsOfTrade = TermsOfTrade.DDP,
                TermsOfTradeSpecified = true
            };

            shipment.SpecialService = new SpecialService[] { new SpecialService() {
                //SpecialServiceType = "DD",
                SpecialServiceType = "WY"
            }};

            //報關資料
            var ExportLineItemList = new List<ExportLineItem>();
            int lineNo = 1;
            shipment.UseDHLInvoice = YesNo.Y;
            shipment.DHLInvoiceLanguageCode = InvLanguageCode.en;
            shipment.DHLInvoiceType = InvoiceType.CMI;
            foreach (var item in DHLData.WinitTransferBoxList.FirstOrDefault().WinitTransfer.Transfer.TransferSKU.Where(x => x.IsEnable))
            {
                var Qty = item.SerialsLlist.Count(x => x.IsEnable && x.SerialsType == "TransferOut");
                var Description = item.SerialsLlist.FirstOrDefault().TransferSKU.SKU.SkuType.SkuTypeLang.Where(x => x.LangID == LangID).FirstOrDefault().Name;
                if (Description.Length > 70)
                {
                    Description = Description.Substring(0, 70);
                }
                var Value = Math.Round(item.SerialsLlist.FirstOrDefault().TransferSKU.SKU.Logistic.Price / 1.05m / DHLData.EXRate, 2);
                var ShippingWeight = Qty * item.SerialsLlist.FirstOrDefault().TransferSKU.SKU.Logistic.ShippingWeight;
                var ManufactureCountryCode = item.SerialsLlist.FirstOrDefault().TransferSKU.SKU.Logistic.OriginCountry;
                var Weight = new ExportLineItemWeight() { Weight = (ShippingWeight / weight), WeightUnit = weightUnit };
                var GrossWeight = new ExportLineItemGrossWeight() { Weight = (ShippingWeight / weight), WeightSpecified = true, WeightUnit = weightUnit, WeightUnitSpecified = true };
                ExportLineItemList.Add(new ExportLineItem
                {
                    Quantity = Qty.ToString(),
                    Description = Description,
                    Value = (float)Value,
                    Weight = Weight,
                    GrossWeight = GrossWeight,
                    ManufactureCountryCode = ManufactureCountryCode
                });
                shipment.Dutiable.DeclaredValue += Qty * Value;//產品總價
            }
            shipment.ExportDeclaration = new ExportDeclaration()
            {
                SignatureName = "Demi Tian",
                InvoiceNumber = DHLData.TransferID.ToString(),
                InvoiceDate = today,
                BillToCompanyName = shipment.Shipper.CompanyName,
                BillToContanctName = shipment.Shipper.Contact.PersonName,
                BillToAddressLine = shipment.Shipper.AddressLine,
                BillToCity = shipment.Shipper.City,
                BillToPostcode = shipment.Shipper.PostalCode,
                BillToCountryName = shipment.Shipper.CountryName,
                BillToPhoneNumber = shipment.Shipper.Contact.PhoneNumber,
                ExportLineItem = ExportLineItemList.Select(i => new ExportLineItem()
                {
                    LineNumber = (lineNo++).ToString(),
                    Quantity = i.Quantity,
                    QuantityUnit = QuantityUnit.PCS,
                    Description = i.Description,
                    Value = i.Value,
                    Weight = i.Weight,
                    GrossWeight = i.GrossWeight,
                    ManufactureCountryCode = i.ManufactureCountryCode
                }).ToArray()
            };

            //MyHelp.Log("Packages", package.ID, "設定Create Data完成");

            return shipment;
        }

        //public ShipmentResponse CreateBox(Box box, DirectLine directLine)
        //{
        //    ShipmentResponse result;
        //    ShipmentRequest shipment = new ShipmentRequest();

        //    shipment.Request = RequsetInit("6.1");
        //    shipment.NewShipper = YesNo.N;
        //    shipment.NewShipperSpecified = true;
        //    shipment.LanguageCode = "tw";
        //    shipment.PiecesEnabled = PiecesEnabled.Y;
        //    shipment.Reference = new Reference[] { new Reference() { ReferenceID = box.BoxID } };
        //    shipment.LabelImageFormat = LabelImageFormat.PDF;
        //    shipment.LabelImageFormatSpecified = true;
        //    shipment.RequestArchiveDoc = YesNo.Y;
        //    shipment.RequestArchiveDocSpecified = true;
        //    shipment.Label = new Label() { LabelTemplate = LabelTemplate.Item8X4_A4_PDF };
        //    shipment.EProcShip = YesNo.N;
        //    shipment.EProcShipSpecified = true;

        //    shipment.Billing = new Billing()
        //    {
        //        ShipperAccountNumber = api_account,
        //        ShippingPaymentType = ShipmentPaymentType.S,
        //        BillingAccountNumber = api_account,
        //        DutyPaymentType = DutyTaxPaymentType.S,
        //        DutyPaymentTypeSpecified = true,
        //        DutyAccountNumber = api_account
        //    };

        //    shipment.Shipper = new Shipper()
        //    {
        //        ShipperID = api_account,
        //        CompanyName = "Zhi You Wan LTD",
        //        AddressLine = new string[] { "No.51, Sec.3 Jianguo N. Rd.,", "South Dist.," },
        //        City = "Taichung City",
        //        PostalCode = "403",
        //        CountryCode = "TW",
        //        CountryName = "Taiwan",
        //        Contact = new Contact() { PersonName = "Huai Wei Ho", PhoneNumber = "0423718118" }
        //    };

        //    Contact contact = new Contact()
        //    {
        //        PersonName = directLine.ContactName,
        //        PhoneNumber = directLine.PhoneNumber
        //    };

        //    shipment.Consignee = new Consignee()
        //    {
        //        CompanyName = !string.IsNullOrEmpty(directLine.CompanyName) ? directLine.CompanyName : contact.PersonName,
        //        AddressLine = new string[] { directLine.StreetLine1, directLine.StreetLine2 },
        //        City = directLine.City,
        //        Division = directLine.StateName,
        //        PostalCode = directLine.PostalCode,
        //        CountryCode = directLine.CountryCode,
        //        CountryName = directLine.CountryName,
        //        Contact = contact
        //    };

        //    decimal weight = shipment.Shipper.CountryCode.Equals("US") ? 453 : 1000;
        //    WeightUnit weightUnit = shipment.Shipper.CountryCode.Equals("US") ? WeightUnit.L : WeightUnit.K;

        //    List<Items> itemList = box.Packages.Where(p => p.IsEnable.Value).SelectMany(p => p.Items.Where(i => i.IsEnable.Value)).ToList();

        //    List<StockKeepingUnit.SkuData> SkuData = new List<StockKeepingUnit.SkuData>();
        //    using (StockKeepingUnit stock = new StockKeepingUnit())
        //    {
        //        var IDs = itemList.Where(i => i.IsEnable.Value).Select(i => i.ProductID).Distinct().ToArray();
        //        SkuData = stock.GetSkuData(IDs);
        //    }

        //    List<Piece> pieceList = new List<Piece>();
        //    pieceList.Add(new Piece()
        //    {
        //        PackageType = PackageType.YP,
        //        PackageTypeSpecified = true,
        //        Weight = itemList.Sum(i => i.Qty.Value * ((decimal)(SkuData.Any(s => s.Sku.Equals(i.ProductID)) ? SkuData.First(s => s.Sku.Equals(i.ProductID)).Weight : i.Skus.ShippingWeight) / weight)),
        //        WeightSpecified = true,
        //        PieceContents = itemList.First().Skus.ProductType.ProductTypeName
        //    });

        //    shipment.ShipmentDetails = new ShipmentDetails()
        //    {
        //        NumberOfPieces = pieceList.Count().ToString(),
        //        Pieces = pieceList.ToArray(),
        //        Weight = pieceList.Sum(p => p.Weight),
        //        WeightUnit = weightUnit,
        //        GlobalProductCode = "P",
        //        LocalProductCode = "P",
        //        Date = today,
        //        Contents = string.Join(", ", itemList.Select(i => i.Skus.ProductType.ProductTypeName).Distinct().ToArray()),
        //        DoorTo = DoorTo.DD,
        //        DoorToSpecified = true,
        //        DimensionUnit = DimensionUnit.C,
        //        DimensionUnitSpecified = true,
        //        PackageType = PackageType.YP,
        //        PackageTypeSpecified = true,
        //        IsDutiable = YesNo.Y,
        //        IsDutiableSpecified = true,
        //        CurrencyCode = "USD"
        //    };

        //    shipment.Dutiable = new Dutiable()
        //    {
        //        DeclaredValue = box.Packages.Sum(p => p.DeclaredTotal),
        //        DeclaredValueSpecified = true,
        //        DeclaredCurrency = shipment.ShipmentDetails.CurrencyCode,
        //        TermsOfTrade = TermsOfTrade.DDP,
        //        TermsOfTradeSpecified = true
        //    };

        //    shipment.SpecialService = new SpecialService[] { new SpecialService() {
        //        //SpecialServiceType = "DD",
        //        SpecialServiceType = "WY"
        //    } };

        //    int lineNo = 1;
        //    shipment.UseDHLInvoice = YesNo.Y;
        //    shipment.DHLInvoiceLanguageCode = InvLanguageCode.en;
        //    shipment.DHLInvoiceType = InvoiceType.CMI;
        //    shipment.ExportDeclaration = new ExportDeclaration()
        //    {
        //        SignatureName = "Demi Tian",
        //        InvoiceNumber = box.BoxID,
        //        InvoiceDate = today,
        //        BillToCompanyName = shipment.Shipper.CompanyName,
        //        BillToContanctName = shipment.Shipper.Contact.PersonName,
        //        BillToAddressLine = shipment.Shipper.AddressLine,
        //        BillToCity = shipment.Shipper.City,
        //        BillToPostcode = shipment.Shipper.PostalCode,
        //        BillToCountryName = shipment.Shipper.CountryName,
        //        BillToPhoneNumber = shipment.Shipper.Contact.PhoneNumber,
        //        ExportLineItem = itemList.Select(i => new ExportLineItem()
        //        {
        //            LineNumber = (lineNo++).ToString(),
        //            Quantity = i.Qty.ToString(),
        //            QuantityUnit = QuantityUnit.PCS,
        //            Description = i.Skus.ProductType.ProductTypeName + " - " + i.Skus.ProductName,
        //            Value = (float)i.UnitPrice.Value,
        //            Weight = new ExportLineItemWeight() { Weight = (decimal)(SkuData.Any(s => s.Sku.Equals(i.ProductID)) ? SkuData.First(s => s.Sku.Equals(i.ProductID)).Weight : i.Skus.ShippingWeight) / weight, WeightUnit = weightUnit },
        //            GrossWeight = new ExportLineItemGrossWeight() { Weight = (decimal)(SkuData.Any(s => s.Sku.Equals(i.ProductID)) ? SkuData.First(s => s.Sku.Equals(i.ProductID)).Weight : i.Skus.ShippingWeight) / weight, WeightSpecified = true, WeightUnit = weightUnit, WeightUnitSpecified = true },
        //            ManufactureCountryCode = i.Skus.Origin
        //        }).ToArray()
        //    };

        //    XmlSerializer serializer = new XmlSerializer(typeof(ShipmentResponse));
        //    string request = SendRequest(shipment);
        //    using (TextReader reader = new StringReader(request))
        //    {
        //        try
        //        {
        //            result = (ShipmentResponse)serializer.Deserialize(reader);
        //        }
        //        catch (Exception e)
        //        {
        //            TextReader errorReader = new StringReader(request);
        //            XmlSerializer errorSerializer = new XmlSerializer(typeof(ShipmentValidateErrorResponse));
        //            ShipmentValidateErrorResponse error = errorSerializer.Deserialize(errorReader) as ShipmentValidateErrorResponse;
        //            errorReader.Dispose();
        //            throw new Exception(string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData)));
        //        }
        //    }

        //    return result;
        //}

        public ShipmentResponse UploadInvoice(ShipmentResponse AWBResult, byte[] image)
        {
            ShipmentResponse result;
            ShipmentRequest shipment = new ShipmentRequest()
            {
                Request = new Request()
                {
                    ServiceHeader = new ServiceHeader()
                    {
                        SiteID = api_siteID,
                        Password = api_password,
                        MessageReference = "Esteemed Courier Service of DHL",
                        MessageTime = today
                    }
                },
                RegionCode = AWBResult.RegionCode,
                RegionCodeSpecified = true,
                ShipmentDetails = new ShipmentDetails()
                {
                    GlobalProductCode = "P",
                    LocalProductCode = "P",
                    Date = today,
                },
                Shipper = new Shipper()
                {
                    OriginServiceAreaCode = AWBResult.Shipper.OriginServiceAreaCode,
                    OriginFacilityCode = AWBResult.Shipper.OriginFacilityCode,
                    CountryCode = AWBResult.Shipper.CountryCode
                },
                Airwaybill = AWBResult.AirwayBillNumber,
                DocImages = new DocImage[]
                {
                    new DocImage()
                    {
                        Type = CarrierApi.DHL.Type.CIN,
                        Image = image,
                        ImageFormat = ImageFormat.PDF
                    }
                },
                schemaVersion = 1.0m
            };

            XmlSerializer serializer = new XmlSerializer(typeof(ShipmentResponse));
            string request = SendRequest(shipment);
            using (TextReader reader = new StringReader(request))
            {
                try
                {
                    result = (ShipmentResponse)serializer.Deserialize(reader);
                }
                catch (Exception e)
                {
                    string errorMsg;
                    TextReader errorReader = new StringReader(request);
                    XmlSerializer errorSerializer = new XmlSerializer(typeof(ShipmentValidateErrorResponse));
                    try
                    {
                        ShipmentValidateErrorResponse error = errorSerializer.Deserialize(errorReader) as ShipmentValidateErrorResponse;
                        errorMsg = string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData));
                    }
                    catch (Exception)
                    {
                        errorSerializer = new XmlSerializer(typeof(OtherErrorResponse));
                        OtherErrorResponse error = errorSerializer.Deserialize(errorReader) as OtherErrorResponse;
                        errorMsg = string.Join("; ", error.Response.Status.Condition.Select(c => c.ConditionData));

                    }
                    errorReader.Dispose();
                    throw new Exception(errorMsg);
                }
            }

            return result;
        }

        private Request RequsetInit(string version)
        {
            return new Request()
            {
                ServiceHeader = new ServiceHeader()
                {
                    SiteID = api_siteID,
                    Password = api_password,
                    MessageReference = "Esteemed Courier Service of DHL",
                    MessageTime = today
                },
                MetaData = new MetaData()
                {
                    SoftwareName = "3PV",
                    SoftwareVersion = version
                }
            };
        }
        /// <summary>
        /// 表頭設定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestType"></param>
        /// <returns></returns>
        private static string SendRequest<T>(T requestType) where T : class
        {
            try
            {
                // Create a request for the URL. 
                WebRequest request = WebRequest.Create("https://xmlpi-ea.dhl.com/XMLShippingServlet");
                //request.Timeout = 60000;

                // If required by the server, set the credentials.
                // request.Credentials = CredentialCache.DefaultCredentials;

                // Wrap the request stream with a text-based writer
                request.Method = "POST";        // Post method
                request.ContentType = "text/xml";

                var stream = request.GetRequestStream();
                StreamWriter writer = new StreamWriter(stream);

                // Write the XML text into the stream
                //var soapWriter = new XmlSerializer(typeof(T));

                //using (var sww = new StringWriter())
                //{
                //    using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sww))
                //    {
                //        soapWriter.Serialize(xmlWriter, requestType);
                //        var xml = sww.ToString(); // your xml
                //    }
                //}

                var soapWriter = new XmlSerializer(typeof(T));

                //add namespaces and/or prefixes ( e.g " <req:BookPickupRequestEA xmlns:req="http://www.dhl.com"> ... </req:BookPickupRequestEA>"
                var ns = new XmlSerializerNamespaces();
                ns.Add("req", "http://www.dhl.com");
                soapWriter.Serialize(writer, requestType, ns);
                writer.Close();

                // Get the response.
                WebResponse response = request.GetResponse();

                // Display the status.
                Trace.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                // Display the content.
                Trace.WriteLine(responseFromServer);

                // Clean up the streams and the response.
                reader.Close();
                response.Close();

                return responseFromServer;
            }
            catch (Exception e)
            {
                string errorMsg = e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
                //MyHelp.Log("", null, string.Format("SendRequest Error - {0}", errorMsg));
                throw new Exception(errorMsg);
            }
        }
    }
    public class ShipProcess
    {
        public Dictionary<string,string> DHL_SaveFile(ShipmentResponse result)
        {
            string basePath = HostingEnvironment.MapPath("~/FileUploads");
            var pdfFile = new Dictionary<string, string>();
            var filePath = Path.Combine(basePath, "DHL_" + result.AirwayBillNumber);
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);//沒資料夾就新增
            pdfFile.Add("AWB",Convert.ToBase64String(Crop(result.LabelImage.First().OutputImage, 97f, 30f, 356f, 553f)));
            //pdfFile.Add("Invoice", Convert.ToBase64String(result.LabelImage.First().MultiLabels.First().DocImageVal));
            //File.WriteAllBytes(Path.Combine(filePath, "AirWaybill.pdf"), );
            File.WriteAllBytes(Path.Combine(filePath, "Invoice_" + result.AirwayBillNumber + ".pdf"), result.LabelImage.First().MultiLabels.First().DocImageVal);
            return pdfFile;
        }
        private byte[] Crop(byte[] pdfbytes, float llx, float lly, float urx, float ury)
        {
            byte[] rslt = null;
            // Allows PdfReader to read a pdf document without the owner's password
            PdfReader.unethicalreading = true;
            // Reads the PDF document
            using (PdfReader pdfReader = new PdfReader(pdfbytes))
            {
                // Set which part of the source document will be copied.
                // PdfRectangel(bottom-left-x, bottom-left-y, upper-right-x, upper-right-y)
                PdfRectangle rect = new PdfRectangle(llx, lly, urx, ury);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Create a new document
                    //using (Document doc = 
                    //	new Document(new Rectangle(288f,432f)))
                    using (Document doc = new Document(PageSize.A4))
                    {
                        // Make a copy of the document
                        PdfSmartCopy smartCopy = new PdfSmartCopy(doc, ms)
                        {
                            PdfVersion = PdfWriter.VERSION_1_7
                        };
                        smartCopy.CloseStream = false;
                        // Open the newly created document                        
                        doc.Open();
                        // Loop through all pages of the source document
                        for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                        {
                            doc.NewPage();// net necessary line
                            // Get a page
                            var page = pdfReader.GetPageN(i);
                            // Apply the rectangle filter we created
                            page.Put(PdfName.CROPBOX, rect);
                            page.Put(PdfName.MEDIABOX, rect);
                            // Copy the content and insert into the new document
                            var copiedPage = smartCopy.GetImportedPage(pdfReader, i);
                            smartCopy.AddPage(copiedPage);
                        }
                        smartCopy.FreeReader(pdfReader);
                        smartCopy.Close();
                        ms.Position = 0;
                        rslt = ms.GetBuffer();
                        // Close the output document
                        doc.Close();
                    }
                }
                return rslt;
            }
        }
    }
}
