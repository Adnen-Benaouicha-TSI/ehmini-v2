using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ehmini.Application.DTOs.Document;
using Ehmini.Application.Services;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ehmini.Application.UnitTests.Services;

public class DocumentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDocumentRepository> _documentRepoMock; // On mock le repository spécifique
    private readonly DocumentService _documentService;

    public DocumentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _documentRepoMock = new Mock<IDocumentRepository>();

        // On lie le repository mocké à la propriété Documents du UnitOfWork mocké
        _unitOfWorkMock.Setup(u => u.Documents).Returns(_documentRepoMock.Object);

        _documentService = new DocumentService(_unitOfWorkMock.Object);
    }

    #region 1. CreateDocumentAsync Tests
    [Fact]
    public async Task CreateDocumentAsync_WithValidDetails_ShouldCalculateTotalsAndSaveSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // 💡 Correction ici : Utilisation de DocumentDetailDto au lieu de CreateDocumentDetailRequestDto
        var detailsDto = new List<DocumentDetailDto>
        {
            new("Assurance Responsabilité Civile", 1, 120.00m),
            new("Frais de dossier", 2, 15.50m)
        };

        var dto = new CreateDocumentRequestDto(
            DocumentType: "Facture",
            ProviderName: "Ehmini Insurance Provider",
            ExternalReference: "REF-2026-XYZ",
            Details: detailsDto
        );

        // On configure le comportement attendu des mocks du UnitOfWork
        _documentRepoMock.Setup(r => r.AddAsync(It.IsAny<Document>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _documentService.CreateDocumentAsync(userId, dto);

        // Assert
        result.Should().NotBeNull();
        result.DocumentType.Should().Be(dto.DocumentType);
        result.ProviderName.Should().Be(dto.ProviderName);
        //result.ExternalReference.Should().Be(dto.ExternalReference);

        result.Details.Should().HaveCount(2);

        var firstLine = result.Details.First(d => d.ItemDescription == "Assurance Responsabilité Civile");
        firstLine.LineTotal.Should().Be(120.00m);

        var secondLine = result.Details.First(d => d.ItemDescription == "Frais de dossier");
        secondLine.LineTotal.Should().Be(31.00m);

        result.TotalAmount.Should().Be(151.00m);

        _documentRepoMock.Verify(r => r.AddAsync(It.IsAny<Document>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    #endregion

    #region 2. GetDocumentsByUserAsync Tests
    [Fact]
    public async Task GetDocumentsByUserAsync_WhenDocumentsExist_ShouldReturnMappedResponseDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Création de fausses entités métiers (Domain Entities) pour simuler la base de données
        var doc1 = Document.Create("Attestation", "AXA", "EXT-001", userId);
        doc1.TotalAmount = 50.00m;

        var doc2 = Document.Create("Quittance", "STAR", "EXT-002", userId);
        doc2.TotalAmount = 250.00m;

        var dbDocuments = new List<Document> { doc1, doc2 };

        // Configuration du Mock pour retourner notre liste factice
        _documentRepoMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(dbDocuments);

        // Act
        var result = await _documentService.GetDocumentsByUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);

        //resultList[0].ExternalReference.Should().Be("EXT-001");
        resultList[0].TotalAmount.Should().Be(50.00m);

        //resultList[1].ExternalReference.Should().Be("EXT-002");
        resultList[1].TotalAmount.Should().Be(250.00m);

        _documentRepoMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetDocumentsByUserAsync_WhenNoDocumentsExist_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _documentRepoMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Document>()); // Retourne une liste vide

        // Act
        var result = await _documentService.GetDocumentsByUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    #endregion
}