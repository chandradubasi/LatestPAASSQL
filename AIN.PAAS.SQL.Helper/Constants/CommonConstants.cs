using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Helper.Constants
{
    public static class CommonConstants
    {
        public class Hospital
        {
            public const string HospitalAPIControllerRoute = "api/Hospitals";
            public const string GetHospitals = "hospitals";
        }
        public class Site
        {
            public const string SiteAPIControllerRoute = "api/Sites";
            public const string GetSiteById = "siteId";
            public const string AllSites = "getallsites";
        }

        public class Lab
        {
            public const string LabAPIControllerRoute = "api/Labs";
            public const string GetLabById = "labId";
            public const string AllLabs = "getalllabs";
        }

        public class Location
        {
            public const string LocationsAPIControllerRoute = "api/Locations";
            public const string GetLocationById = "locationId";
            public const string AllLocations = "getalllocations";
        }

        public class Storage
        {
            public const string LocationsAPIControllerRoute = "api/Storages";
            public const string GetStorageById = "storageId";
            public const string AllStorages = "getallstorages";

        }
        public class Inventory
        {
            public const string InventoryAPIControllerRoute = "api/inventory";
            public const string CreateInventoryRoute = "workflows/checkin";
            public const string CheckOutInventoryRoute = "workflows/checkout";
            public const string GetInventoryByStatus = "workflows/{status}";
            public const string GetInventoryById = "inventoryId";
            public const string InventoryTransfer = "workflows/transfer";
            public const string GetInventoryByPaging = "allitemsbypaging";
            public const string GetInventoryBySearch = "itemsbysearchwithsortandpaging";
            public const string GetInventoryByFilter = "itemsbyfilter";


        }

        public class APIConstant
        {
            public const string InternalServerError = "Internal Server Error";

        }
        
    }
}
