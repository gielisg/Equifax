using System;
using System.Collections.Generic;

namespace IdMatrixSoapLib.Models
{
    public class IdMatrixRequest
    {
        public string ClientReference { get; set; }
        public string Reason { get; set; }
        public Consents Consents { get; set; }
        public string FamilyName { get; set; }
        public string FirstGivenName { get; set; }
        public string OtherGivenName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Addresses Addresses { get; set; }
    }

    public class Consents
    {
        public bool VedaCreditBureau { get; set; }
        public bool DriversLicence { get; set; }
        public bool Medicare { get; set; }
        public bool AustralianPassport { get; set; }
        public bool VisaEntitlementVerificationOnline { get; set; }
    }

    public class Addresses
    {
        public Address CurrentAddress { get; set; }
        public Address PreviousAddress { get; set; }
    }

    public class Address
    {
        public string Property { get; set; }
        public string UnitNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string UnformattedAddress { get; set; }
        public Phone Phone { get; set; }
        public Employment Employment { get; set; }
        public string EmailAddress { get; set; }
        public string AlternativeEmailAddress { get; set; }
        public DriversLicenceDetails DriversLicenceDetails { get; set; }
        // Add other details as needed
    }

    public class Phone
    {
        public PhoneNumbers Numbers { get; set; }
        public PhoneAuthentication PhoneAuthentication { get; set; }
    }

    public class PhoneNumbers
    {
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }
    }

    public class PhoneAuthentication
    {
        public string Number { get; set; }
        public string SendPinMethod { get; set; }
    }

    public class Employment
    {
        public string Employer { get; set; }
        public string Occupation { get; set; }
        public string ANZSICClassCode { get; set; }
        public string EmployerABN { get; set; }
    }

    public class DriversLicenceDetails
    {
        public string StateCode { get; set; }
        public string Number { get; set; }
        public string CardNumber { get; set; }
    }
}
