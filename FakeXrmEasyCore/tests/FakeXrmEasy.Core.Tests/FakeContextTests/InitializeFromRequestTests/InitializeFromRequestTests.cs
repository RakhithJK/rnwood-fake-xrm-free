using Crm;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace FakeXrmEasy.Tests.FakeContextTests.InitializeFromRequestTests
{
    public class InitializeFromRequestTests: FakeXrmEasyTests
    {
        [Fact]
        public void When_Calling_InitializeFromRequest_Should_Return_InitializeFromResponse()
        {
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Contact)));
            
            var lead = new Entity
            {
                LogicalName = "Lead",
                Id = Guid.NewGuid(),
                ["FirstName"] = "Arjen",
                ["LastName"] = "Stortelder"
            };

            _context.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference("Lead", lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = "Contact",
                TargetFieldType = TargetFieldType.All
            };

            Assert.IsType<InitializeFromResponse>(_service.Execute(req));
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_As_Entity_Of_Type_TargetEntityName()
        {
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Contact)));

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            _context.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);
            Assert.IsType<Contact>(result.Entity);
            Assert.Equal(Contact.EntityLogicalName, result.Entity.LogicalName);
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_With_Attributes_Set_From_The_Mapping()
        {
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Contact)));

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            _context.Initialize(new List<Entity> { lead });
            (_context as XrmFakedContext).AddAttributeMapping(Lead.EntityLogicalName, "firstname", Contact.EntityLogicalName, "firstname");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);
            var contact = result.Entity.ToEntity<Contact>();
            Assert.Equal("Arjen", contact.FirstName);
            Assert.Equal(null, contact.LastName);
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_With_EntityReference()
        {
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Contact)));

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            _context.Initialize(new List<Entity> { lead });
            (_context as XrmFakedContext).AddAttributeMapping(Lead.EntityLogicalName, "leadid", Contact.EntityLogicalName, "originatingleadid");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);
            var contact = result.Entity;
            var originatingleadid = contact["originatingleadid"];
            Assert.IsType<EntityReference>(originatingleadid);
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_Without_Id()
        {
            _context.EnableProxyTypes(Assembly.GetAssembly(typeof(Contact)));

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            _context.Initialize(new List<Entity> { lead });
            (_context as XrmFakedContext).AddAttributeMapping(Lead.EntityLogicalName, "firstname", Contact.EntityLogicalName, "firstname");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);
            var contact = result.Entity.ToEntity<Contact>();
            Assert.Equal(Guid.Empty, contact.Id);
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_With_Early_Bound_Classes_Should_Return_Early_Bound_Entity()
        {
            

            

            var lead = new Lead
            {
                Id = Guid.NewGuid()
            };

            // This will set ProxyTypesAssembly = true
            _context.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);

            Assert.IsType<Contact>(result.Entity);
        }

        [Fact]
        public void When_Calling_InitializeFromRequest_With_Late_Bound_Classes_Should_Return_Late_Bound_Entity()
        {
            
            string sourceEntityLogicalName = "lead";
            string targetEntityLogicalName = "contact";

            

            var lead = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = sourceEntityLogicalName
            };

            _context.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(sourceEntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = targetEntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)_service.Execute(req);

            Assert.IsType<Entity>(result.Entity);
            Assert.Equal(targetEntityLogicalName, result.Entity.LogicalName);
        }
    }
}