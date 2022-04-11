using AutoMapper;
using CustomersApi.Domain;
using CustomersApi.DTO.Requests;
using CustomersApi.Mapping;
using CustomersApi.Repositories;
using CustomersApi.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CustomersTest
{
    public class CustomerServiceTests
    {
        private readonly ICustomerService _sut;
        private readonly ICustomerRepository _customerRepositoryMock = Substitute.For<ICustomerRepository>();
        private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>()).CreateMapper();

        public CustomerServiceTests()
        {
            _sut = new CustomerService(_customerRepositoryMock, _mapper);
        }

        [Fact]
        public async Task GetById_ShouldReturnCustomer_WhenCustomerExists()
        {
            //Arrange
            var customerId = new Guid();
            var customerName = "Brian Jones";
            
            var customerOutput = new Customer
            {
                Id = customerId,
                Name = customerName
            };
            
            _customerRepositoryMock.GetById(customerId).Returns(customerOutput);

            //Act
            var customer = await _sut.GetById(customerId);

            //Assert
            Assert.Equal(customerId, customer.Id);
            Assert.Equal(customerName, customer.Name);            
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCustomerDoesNotExists()
        {
            //Arrange
            Customer? customerOutput = null;
            _customerRepositoryMock.GetById(Arg.Any<Guid>()).Returns(customerOutput);

            //Act
            var customer = await _sut.GetById(Guid.NewGuid());

            //Assert
            Assert.Null(customer);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCustomers_WhenCustomersExists()
        {
            //Arrange
            var customersList = new List<Customer>();
            
            customersList.Add(new Customer
            {
                Id = new Guid(),
                Name = "Janis Joplin"
            });

            customersList.Add(new Customer
            {
                Id = new Guid(),
                Name = "Kurt Cobain"
            });
            
            _customerRepositoryMock.GetAll().Returns(customersList);

            //Act
            var customers = await _sut.GetAll();

            //Assert
            Assert.True(customers.Count() > 1);
        }

        [Fact]
        public async Task Insert_ShouldCreateAndReturnCustomer()
        {
            //Arrange
            var customerOutput = new Customer()
            {
                Id = new Guid(),
                Name = "Janis Joplin"
            };

            _customerRepositoryMock.Insert(Arg.Any<Customer>()).Returns(customerOutput);

            var createCustomerRequest = new CreateCustomerRequest()
            {
                Name = "Janis Joplin"
            };

            //Act
            var createCustomerResponse = await _sut.Insert(createCustomerRequest);

            //Assert
            Assert.True(createCustomerResponse.Id == customerOutput.Id);
        }        
    }
}