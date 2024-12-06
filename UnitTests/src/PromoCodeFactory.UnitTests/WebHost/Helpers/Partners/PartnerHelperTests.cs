using AutoFixture;
using FluentAssertions;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Helpers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Helpers.Partners
{
    public class PartnerHelperTests
    {
        public Partner CreateBasePartner()
        {
            var partner = new Partner()
            {
                Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                Name = "Суперигрушки",
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                        CreateDate = new DateTime(2020, 07, 9),
                        EndDate = new DateTime(2020, 10, 9),
                        Limit = 100
                    }
                }
            };

            return partner;
        }

        [Fact]
        public void ProcessActiveLimit_WhenActiveLimitExists_ShouldUpdateCancelDateAndResetPromoCodes()
        {
            // Arrange
            var partner = CreateBasePartner();

            // Act
            partner.ProcessActiveLimit();

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
            partner.PartnerLimits.First().CancelDate.Should().NotBeNull();
            partner.PartnerLimits.First().CancelDate.Value.Date.Should().BeCloseTo(DateTime.Now.Date, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateNewLimit_ShouldReturnNewPartnerPromoCodeLimit()
        {
            // Arrange
            var partner = CreateBasePartner();

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 50,
                EndDate = DateTime.Now.AddMonths(1)
            };

            // Act
            var newLimit = PartnerHelper.CreateNewLimit(partner, request);

            // Assert
            newLimit.Should().NotBeNull();
            newLimit.Limit.Should().Be(request.Limit);
            newLimit.PartnerId.Should().Be(partner.Id);
            newLimit.CreateDate.Date.Should().BeCloseTo(DateTime.Now.Date, TimeSpan.FromSeconds(1));
            newLimit.EndDate.Date.Should().Be(request.EndDate.Date);
        }
    }
}
