﻿//------------------------------------------------------------------------------
// Contributor(s): RJH 08/07/2009, mb 10/20/2010, AC 05/16/2011.
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Nop.Core;
using Nop.Plugin.Shipping.USPS.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS
{
    /// <summary>
    /// USPS computation method
    /// </summary>
    public class USPSComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly USPSService _uspsService;

        #endregion

        #region Ctor

        public USPSComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            USPSService uspsService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _uspsService = uspsService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest is null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.CountryFrom is null)
                return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };

            return _uspsService.GetRates(getShippingOptionRequest);
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingUSPS/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new USPSSettings
            {
                Url = USPSShippingDefaults.DEFAULT_URL,
                Username = "123",
                Password = "456",
                AdditionalHandlingCharge = 0,
                CarrierServicesOfferedDomestic = "",
                CarrierServicesOfferedInternational = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Url", "URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Url.Hint", "Specify USPS URL.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Username", "Username");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Username.Hint", "Specify USPS username.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Password", "Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.Password.Hint", "Specify USPS password.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge", "Additional handling charge");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic", "Domestic Carrier Services");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic.Hint", "Select the services you want to offer to customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational", "International Carrier Services");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational.Hint", "Select the services you want to offer to customers.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<USPSSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Url");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Url.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Username");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Username.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Password");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.Password.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational.Hint");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => new USPSShipmentTracker(_uspsService);

        #endregion
    }
}