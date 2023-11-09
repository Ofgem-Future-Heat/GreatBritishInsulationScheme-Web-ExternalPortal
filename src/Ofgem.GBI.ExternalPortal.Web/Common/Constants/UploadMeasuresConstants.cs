namespace Ofgem.GBI.ExternalPortal.Web.Common.Constants
{
    public static class UploadMeasuresConstants
    {
        public static class MeasuresUploadValidationValues
        {
            public const int MaxRows = 10000;
            public const int MaxFileSize = 20000000;
            public const string ValidFileExtension = ".csv";
            public const string MeasureCsvTemplateHeader = "Supplier_Reference,Measure_Reference_Number,Measure_Type,Purpose_of_Notification,Date_of_Completed_Installation,Building_Number,Building_Name,Flat_Name/Number,Street_Name,Town,Post_Code,Unique_Property_Reference_Number_(UPRN),Starting_SAP_Rating,Floor_Area,Rural,Off_Gas,Private_Domestic_Premises,Tenure_Type,Property,Eligibility_Type,Verification_Method,DWP_Reference_Number,Council_Tax_Band,PRS_SAP_Band_Exception,Associated_Insulation_MRN_for_Heating_Measures,Associated_In-fill_Measure_1,Associated_In-fill_Measure_2,Associated_In-fill_Measure_3,Flex_Referral_Route,LA_Declaration_Reference_Number,Date_Of_Householder_Eligibility,Percentage_Of_Property_Treated,Heating_Source,Installer_Name,Innovation_Measure_Number,Trustmark_Business_Licence_Number,Trustmark_Unique_Measure_Reference,Trustmark_Lodged_CertificateID,Trustmark_Project_Reference_Number,TrustMark_Completed_Project_Cert_ID";
        }

        public static class TemplateText
        {
            public const string CsvValidationNoFileText = "Select a measure file.";
            public const string CsvValidationFileIsEmptyText = "The selected file is empty.";
            public const string CsvValidationNotGBISSchemaText = "The selected CSV must use the offical GB Insulation Scheme template.";
            public const string CsvValidationExceedsMaxRowsText = "You can only upload up to 10,000 rows in a single CSV.";
            public const string CsvValidationInvalidFileTypeText = "The selected file must be a CSV.";
            public const string CsvValidationExceedsMaxFileSizeText = "The selected file must be smaller than 20MB.";
        }
    }
}
