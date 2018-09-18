using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class TestDiagPropertyItem
    {
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
    }

    public class TestDiagItem
    {
        public string TestName { get; set; }
        public string ErrorMessage { get; set; }
        public string Hint { get; set; }
        public string HintID { get; set; }

        public bool Success
        {
            get
            {
                return string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }

        private List<TestDiagPropertyItem> properties = null;

        public List<TestDiagPropertyItem> Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new List<TestDiagPropertyItem>();
                }

                return properties;
            }
            set
            {
                properties = value;
            }
        }

        public string PropertiesStr
        {
            get
            {
                return Properties.Any() ? string.Join(", ", Properties.Select(r => string.Format("{0}: {1}", r.PropertyName, r.PropertyValue))) : "";
            }
        }
    }

    public class TestDiagRequest
    {
        private string cultureName = "Def";

        public string CultureName
        {
            get
            {
                return cultureName;
            }
            set
            {
                cultureName = value;
            }
        }
    }

    public class TestDiagResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        private List<TestDiagItem> testDiagItems = null;

        public List<TestDiagItem> TestDiagItems
        {
            get
            {
                if (testDiagItems == null)
                {
                    testDiagItems = new List<TestDiagItem>();
                }

                return testDiagItems;
            }
            set
            {
                testDiagItems = null;
            }
        }

        public bool Success
        {
            get
            {
                return ReturnCode == 0;
            }
        }
    }
}
