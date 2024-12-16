using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

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

        // Проверяет, что возвращается ошибка NotFound, если партнер с указанным ID не найден.
        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerNotFound_ReturnsNotFound()
        {
            //Arrange
            _partnersRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                   .ReturnsAsync(null as Partner);

            var request = new Fixture().Create<SetPartnerPromoCodeLimitRequest>();

            //Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(It.IsAny<Guid>(), request);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // Проверяет, что возвращается ошибка BadRequest, если партнер заблокирован (IsActive=false).
        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerNotActive_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture();

            var partner = fixture.Build<Partner>()
                .With(x => x.IsActive, false)
                .Without(x => x.PartnerLimits)
                .Create();

            _partnersRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                   .ReturnsAsync(partner);

            var request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // Проверяет, что количество выданных промокодов обнуляется, если текущий лимит активен (без CancelDate).
        [Fact]
        public async Task SetPartnerPromoCodeLimit_WhenPartnerHasIssueNumbersWithoutCancelDateLimit_ThenPromocodeReset()
        {
            // Arrange
            var fixture = new Fixture();
            var NumberIssuedPromoCodesCount = new Random().Next(1, 10);

            var partner = CreateBasePartner();
            partner.NumberIssuedPromoCodes = NumberIssuedPromoCodesCount;

            var limit = fixture.Build<PartnerPromoCodeLimit>()
                .With(x => x.Partner, partner)
                .Without(x => x.CancelDate)
                .Create();

            partner.PartnerLimits.Add(limit);

            _partnersRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                   .ReturnsAsync(partner);

            var request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);
            var result = await _partnersRepositoryMock.Object.GetByIdAsync(It.IsAny<Guid>());

            //Assert
            result.NumberIssuedPromoCodes.Should().Be(0);
        }

        // Проверяет, что у существующего активного лимита устанавливается дата отмены при добавлении нового лимита.
        [Fact]
        public async Task SetPartnerPromoCodeLimit_WhenLimitDoesntHaveCancelDate_ThenFillCancelDate()
        {
            // Arrange
            var fixture = new Fixture();
            var partner = CreateBasePartner();

            var limit = fixture.Build<PartnerPromoCodeLimit>()
                .With(x => x.Partner, partner)
                .Without(x => x.CancelDate)
                .Create();

            partner.PartnerLimits.Add(limit);

            _partnersRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);
            var result = await _partnersRepositoryMock.Object.GetByIdAsync(It.IsAny<Guid>());

            // Assert
            result.PartnerLimits.FirstOrDefault(x => x.CancelDate.HasValue)
                                .Should()
                                .NotBeNull();
        }

        // Проверяет, что возвращается ошибка BadRequest, если новый лимит меньше или равен 0.
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SetPartnerPromoCodeLimit_WhenLimitLessEqualsZero_ThenBadRequest(int limitValue)
        {
            // Arrange
            var fixture = new Fixture();

            var partner = CreateBasePartner();

            _partnersRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var request = fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, limitValue).Create();

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // Проверяет, что новый лимит успешно сохраняется в базе данных.
        [Fact]
        public async Task SetPartnerPromoCodeLimit_WhenNewLimit_ThenSavedInDatabase()
        {
            // Arrange
            var fixture = new Fixture();

            var partner = CreateBasePartner();

            _partnersRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            var request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);
            var result = await _partnersRepositoryMock.Object.GetByIdAsync(It.IsAny<Guid>());

            // Assert
            result.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue).Should().NotBeNull();
        }
    }
}