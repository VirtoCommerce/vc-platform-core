using System;
using System.Linq;
using System.Web.Http.Results;
using VirtoCommerce.CustomerModule.Web.Controllers.Api;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using Xunit;

namespace VirtoCommerce.CustomerModule.Test
{
    public class ContactControllerTest
    {
        [Fact]
        public void SearchContactsTest()
        {
            var controller = GetContactController();
            var result = controller.Search(new MembersSearchCriteria()) as OkNegotiatedContentResult<GenericSearchResult<Member>>;
            Assert.NotNull(result?.Content);
        }

        [Fact]
        public void CreateNewOrganization()
        {
            var controller = GetContactController();
            var org = new Organization
            {
                Id = "org2",
                BusinessCategory = "cat2",
                Name = "organization 2",
                ParentId = "org1"


            };
            var result = controller.CreateMember(org) as OkNegotiatedContentResult<Organization>;
            Assert.NotNull(result?.Content);
        }

        [Fact]
        public void SearchTest()
        {
            var controller = GetContactController();
            var result = controller.Search(new MembersSearchCriteria { MemberId = "org1" }) as OkNegotiatedContentResult<GenericSearchResult<Member>>;
        }

        [Fact]
        public void GetContact()
        {
            var controller = GetContactController();
            var result = controller.GetMemberById("testContact1") as OkNegotiatedContentResult<Contact>;
        }

        [Fact]
        public void CreateNewContact()
        {
            var controller = GetContactController();
            var contact = new Contact
            {
                Id = "testContact1",
                FullName = "Vasa2",
                BirthDate = DateTime.UtcNow,
                Organizations = new[] { "org1" },
                Addresses = new[]
                {
                    new Address {
                    Name = "some name",
                    AddressType = AddressType.Shipping,
                    City = "london",
                    Phone = "+68787687",
                    PostalCode = "22222",
                    CountryCode = "ENG",
                    CountryName = "England",
                    Email = "user@mail.com",
                    FirstName = "first name",
                    LastName = "last name",
                    Line1 = "line 1",
                    Organization = "org1"
                    }
                }.ToList(),
                Notes = new[] { new Note { Title = "1111", Body = "dfsdfs sdf sdf sdf sd" } },
                Emails = new[] { "uuu@mail.ru", "ssss@mail.ru" },
                Phones = new[] { "2322232", "32323232" },
                //DynamicPropertyValues = new[] { new DynamicPropertyObjectValue { Property = new DynamicProperty { Name = "testProp", ValueType = DynamicPropertyValueType.ShortText }, Values = new object[] { "sss" } } }.ToList(),
                DefaultLanguage = "ru"
            };
            var result = controller.CreateContact(contact) as OkNegotiatedContentResult<Contact>;
            Assert.NotNull(result?.Content);
        }

        [Fact]
        public void UpdateContact()
        {
            var controller = GetContactController();
            var result = controller.GetMemberById("testContact") as OkNegotiatedContentResult<Contact>;
            var contact = result.Content;

            contact.FullName = "diff name";
            contact.Emails.Remove(contact.Emails.FirstOrDefault());
            //contact.DynamicPropertyValues.Add(new DynamicPropertyObjectValue { Property = new DynamicProperty { Name = "setting2", ValueType = DynamicPropertyValueType.Integer }, Values = new object[] { "1223" } });

            controller.UpdateContact(contact);

            result = controller.GetMemberById("testContact") as OkNegotiatedContentResult<Contact>;

            contact = result.Content;
        }

        [Fact]
        public void PartialUpdateContact()
        {
            var controller = GetContactController();
            var contact = new Contact
            {
                Id = "testContact",
                FullName = "ET"
            };

            controller.UpdateContact(contact);

            var result = controller.GetMemberById("testContact") as OkNegotiatedContentResult<Contact>;

            contact = result.Content;
        }

        [Fact]
        public void DeleteContact()
        {
            var controller = GetContactController();
            controller.DeleteMembers(new[] { "testContact" });
            var result = controller.GetMemberById("testStore") as OkNegotiatedContentResult<Contact>;

            Assert.Null(result);
        }


        private static CustomerModuleController GetContactController()
        {

            return new CustomerModuleController(null, null);
        }
    }
}
