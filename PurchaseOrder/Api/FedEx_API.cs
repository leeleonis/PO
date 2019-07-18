using PurchaseOrderSys.FedExShipService;
using PurchaseOrderSys.FedExTrackService;
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;


namespace PurchaseOrderSys.FedExApi
{
    public class FedExData
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
    public class FedExTrackingData
    {
        public int index { get; set; }
        public string Tracking { get; set; }
        public byte[] FedExZpl { get; set; }
    }
    public class FedEx_API
    {
        private string api_key;
        private string api_password;
        private string api_accountNumber;
        private string api_meterNumber;

        public string endpoint;

        public FedEx_API(ApiSetting Api)
        {
            api_key = Api.ApiKey;
            api_password = Api.ApiPassword;
            api_accountNumber = Api.ApiAccount;
            api_meterNumber = Api.ApiMeter;
            //api_key = "WrgHTsKUAieD5eVD";
            //api_password = "FbppS0UhJEUDL0JQ13BgTwZ0i";
            //api_accountNumber = "504470423";
            //api_meterNumber = "110786452";
        }

        public List<FedExTrackingData> CreateBox(FedExData FedExData)
        {
            var FedExTrackingDataList = new List<FedExTrackingData>();
            ProcessShipmentRequest request = _shipmentInit();

            request.TransactionDetail = new PurchaseOrderSys.FedExShipService.TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "*** Process Shipment Request ***";

            request.RequestedShipment = new RequestedShipment()
            {
                ShipTimestamp = DateTime.Today,
                DropoffType = DropoffType.REGULAR_PICKUP,
                ServiceType = (FedExShipService.ServiceType)FedExData.MethodType,
                PackagingType = (PurchaseOrderSys.FedExShipService.PackagingType)FedExData.BoxType,
                Shipper = _shipperInit(),
                ShippingChargesPayment = new Payment() { PaymentType = PaymentType.SENDER, Payor = new Payor() { ResponsibleParty = _shipperInit() } },
                PackageCount = FedExData.WinitTransferBoxList.Count().ToString()//總箱數
            };

            request.RequestedShipment.Recipient = new Party()
            {
                Contact = new FedExShipService.Contact()
                {
                    PersonName = FedExData.WinitWarehouse.Company,
                    CompanyName = FedExData.WinitWarehouse.Company,
                    PhoneNumber = FedExData.WinitWarehouse.Phone,
                },
                Address = new PurchaseOrderSys.FedExShipService.Address()
                {
                    StreetLines = new string[] { FedExData.WinitWarehouse.Address1, FedExData.WinitWarehouse.Address2 },
                    City = FedExData.WinitWarehouse.City,
                    StateOrProvinceCode = FedExData.WinitWarehouse.State,
                    PostalCode = FedExData.WinitWarehouse.Postcode,
                    CountryName = FedExData.WinitWarehouse.Country,
                    CountryCode = FedExData.WinitWarehouse.WinitWarehouse
                }
            };

            int NumberOfPieces = 1;
            //string[] IDS = new string[] { "IDS", "IDS (US)" };
            //string currency = IDS.Contains(directLine.Abbreviation) ? "USD" : Enum.GetName(typeof(PurchaseOrderSys.OrderService.CurrencyCodeType2), boxList[0].Packages.First(p => p.IsEnable.Value).Orders.OrderCurrencyCode.Value);
            //string currency = Enum.GetName(typeof(PurchaseOrderSys.OrderService.CurrencyCodeType2), box.Packages.First(p => p.IsEnable.Value).Orders.OrderCurrencyCode.Value);
            var commodityList = new List<FedExShipService.Commodity>();
            var itemLineList = new List<RequestedPackageLineItem>();
            FedExShipService.Money customsValue = new FedExShipService.Money() { Currency = FedExData.Currency, Amount = Math.Round(FedExData.WinitTransferBoxList.Sum(x => x.WinitTransferBoxItem.Sum(y => y.Value)).Value / 1.05m / FedExData.EXRate, 2) };//海關價值(總價)

            foreach (var WinitTransferBox in FedExData.WinitTransferBoxList)
            {
                var itemList = WinitTransferBox.WinitTransferBoxItem;

                FedExShipService.Commodity commodity = new FedExShipService.Commodity
                {
                    NumberOfPieces = itemList.Count().ToString(),//箱內物品數
                    Description = string.Join(", ", itemList.Select(i => i.Name).Distinct().ToArray()),
                    CountryOfManufacture = "CN",
                    Weight = new FedExShipService.Weight()
                    {
                        Units = request.RequestedShipment.Shipper.Address.CountryCode.Equals("US") ? FedExShipService.WeightUnits.LB : FedExShipService.WeightUnits.KG,
                        Value = (itemList.Sum(i => i.Weight)/ (request.RequestedShipment.Shipper.Address.CountryCode.Equals("US") ? 453 : 1000)).Value
                    },
                    Quantity = 1,
                    QuantityUnits = "EA",
                    UnitPrice = new FedExShipService.Money() { Currency = FedExData.Currency, Amount = Math.Round(itemList.Sum(y => y.Value).Value / 1.05m / FedExData.EXRate, 2)  },//整箱的價值
                    CustomsValue = customsValue,//海關價值(總價)
                    QuantitySpecified = true
                };

                commodityList.Add(commodity);
                itemLineList.Add(new RequestedPackageLineItem()
                {
                    InsuredValue = new FedExShipService.Money() { Amount = 0, Currency = FedExData.Currency },
                    Weight = commodity.Weight,
                    CustomerReferences = new CustomerReference[]
                    {
                        new CustomerReference()
                        {
                            CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE,
                            Value = "TransferWinitBox:"+WinitTransferBox.WinitTransferID+"-"+NumberOfPieces
                        }
                    },
                    SequenceNumber = NumberOfPieces++.ToString()
                });
            }

            request.RequestedShipment.TotalWeight = new FedExShipService.Weight()
            {
                Units = request.RequestedShipment.Shipper.Address.CountryCode.Equals("US") ? FedExShipService.WeightUnits.LB : FedExShipService.WeightUnits.KG,
                Value = commodityList.Select(c => c.Weight).Sum(w => w.Value)
            };

            request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail()
            {
                DutiesPayment = new Payment() { PaymentType = PaymentType.SENDER, Payor = new Payor() { ResponsibleParty = _shipperInit() } },
                DocumentContent = InternationalDocumentContentType.DOCUMENTS_ONLY,
                Commodities = new FedExShipService.Commodity[1],
                DocumentContentSpecified = true
            };

            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];

            request.RequestedShipment.LabelSpecification = new LabelSpecification()
            {
                LabelOrder = LabelOrderType.SHIPPING_LABEL_FIRST,
                LabelFormatType = LabelFormatType.COMMON2D,
                ImageType = ShippingDocumentImageType.ZPLII,
                LabelStockType = LabelStockType.STOCK_4X6,
                LabelPrintingOrientation = LabelPrintingOrientationType.BOTTOM_EDGE_OF_TEXT_FIRST,
                LabelOrderSpecified = true,
                ImageTypeSpecified = true,
                LabelStockTypeSpecified = true,
                LabelPrintingOrientationSpecified = true
            };

            ProcessShipmentReply reply = new ProcessShipmentReply();
            using (ShipPortTypeClient client = new ShipPortTypeClient())
            {
                var endpoint = client.Endpoint;
                ConsoleOutputBehavior consoleOutputBehavior = new ConsoleOutputBehavior();
                client.Endpoint.Behaviors.Add(consoleOutputBehavior);

                try
                {
                    for (int i = 0; i < itemLineList.Count(); i++)
                    {
                        if (i != 0)//第二筆之後
                        {
                            request.RequestedShipment.TotalWeight = null;
                            request.RequestedShipment.MasterTrackingId = reply.CompletedShipmentDetail.MasterTrackingId;//帶入第一筆的TrackingId
                        }
                        request.RequestedShipment.CustomsClearanceDetail.CustomsValue = commodityList[i].CustomsValue;
                        request.RequestedShipment.CustomsClearanceDetail.Commodities[0] = commodityList[i];
                        request.RequestedShipment.RequestedPackageLineItems[0] = itemLineList[i];

                        reply = client.processShipment(request);
                        if (reply.HighestSeverity.Equals(PurchaseOrderSys.FedExShipService.NotificationSeverityType.ERROR) || reply.HighestSeverity.Equals(PurchaseOrderSys.FedExShipService.NotificationSeverityType.FAILURE))
                        {
                            throw new Exception(string.Join("\n", reply.Notifications.Select(n => n.Message).ToArray()));
                        }

                        var TrackingNumber = reply.CompletedShipmentDetail.CompletedPackageDetails.First().TrackingIds.Select(t => t.TrackingNumber).First();//TrackingIds要回存
                        var FedExZpl = reply.CompletedShipmentDetail.CompletedPackageDetails.First().Label.Parts.First().Image;
                        FedExTrackingDataList.Add(new FedExTrackingData
                        {
                            index = i,
                            Tracking = TrackingNumber,
                            FedExZpl = FedExZpl
                        });
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                    //PurchaseOrderSys.FedExShipService.Notification notification = new PurchaseOrderSys.FedExShipService.Notification();

                    //if (!string.IsNullOrEmpty(consoleOutputBehavior.ConsoleOutputInspector.ResponseXML))
                    //{
                    //    XElement element = XElement.Parse(consoleOutputBehavior.ConsoleOutputInspector.ResponseXML);
                    //    notification.Message = element.Attributes("Message").Any() ? element.Attributes("Message").First().Value : element.Attributes("Desc").First().Value;
                    //}
                    //else
                    //{
                    //    notification.Message = e.Message;
                    //}

                    //reply = new ProcessShipmentReply() { Notifications = new PurchaseOrderSys.FedExShipService.Notification[] { notification } };
                }
            }

            return FedExTrackingDataList;
        }

        private ProcessShipmentRequest _shipmentInit()
        {
            ProcessShipmentRequest request = new ProcessShipmentRequest();

            request.WebAuthenticationDetail = new PurchaseOrderSys.FedExShipService.WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new PurchaseOrderSys.FedExShipService.WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = api_key;
            request.WebAuthenticationDetail.UserCredential.Password = api_password;

            request.ClientDetail = new PurchaseOrderSys.FedExShipService.ClientDetail();
            request.ClientDetail.AccountNumber = api_accountNumber;
            request.ClientDetail.MeterNumber = api_meterNumber;

            request.Version = new PurchaseOrderSys.FedExShipService.VersionId();

            return request;
        }

        private Party _shipperInit()
        {
            Party shipper = new Party()
            {
                AccountNumber = api_accountNumber,
                Contact = new PurchaseOrderSys.FedExShipService.Contact()
                {
                    PersonName = "Demi Tian",
                    CompanyName = "Zhi You Wan LTD",
                    PhoneNumber = "0423718118",
                },
                Address = new PurchaseOrderSys.FedExShipService.Address()
                {
                    StreetLines = new string[] { "No.51, Sec.3 Jianguo N. Rd.,", "South Dist.," },
                    City = "Taichung City",
                    PostalCode = "403",
                    CountryName = "Taiwan",
                    CountryCode = "TW"
                }
            };

            return shipper;
        }

        public TrackReply Tracking(string trackingNumber)
        {
            TrackRequest request = _trackingInit();

            request.TransactionDetail = new PurchaseOrderSys.FedExTrackService.TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "*** Track Request ***";

            request.Version = new PurchaseOrderSys.FedExTrackService.VersionId();

            request.SelectionDetails = new TrackSelectionDetail[1] { new TrackSelectionDetail() };
            request.SelectionDetails[0].PackageIdentifier = new TrackPackageIdentifier();
            request.SelectionDetails[0].PackageIdentifier.Value = trackingNumber;
            request.SelectionDetails[0].PackageIdentifier.Type = TrackIdentifierType.TRACKING_NUMBER_OR_DOORTAG;

            TrackPortTypeClient client = new TrackPortTypeClient();
            TrackReply reply = client.track(request);
            return reply;
        }

        private TrackRequest _trackingInit()
        {
            TrackRequest request = new TrackRequest();

            request.WebAuthenticationDetail = new PurchaseOrderSys.FedExTrackService.WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new PurchaseOrderSys.FedExTrackService.WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = api_key;
            request.WebAuthenticationDetail.UserCredential.Password = api_password;

            request.ClientDetail = new PurchaseOrderSys.FedExTrackService.ClientDetail();
            request.ClientDetail.AccountNumber = api_accountNumber;
            request.ClientDetail.MeterNumber = api_meterNumber;

            request.ProcessingOptions = new TrackRequestProcessingOptionType[1];
            request.ProcessingOptions[0] = TrackRequestProcessingOptionType.INCLUDE_DETAILED_SCANS;

            return request;
        }
    }

    public class ConsoleOutputBehavior : IEndpointBehavior
    {
        public ConsoleOutputInspector ConsoleOutputInspector { get; private set; }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ConsoleOutputInspector = new ConsoleOutputInspector();
            clientRuntime.MessageInspectors.Add(ConsoleOutputInspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            throw new Exception("Behavior not supported on the server side!");
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class ConsoleOutputInspector : IClientMessageInspector
    {
        public string ResponseXML = string.Empty;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            ResponseXML = reply.ToString();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}