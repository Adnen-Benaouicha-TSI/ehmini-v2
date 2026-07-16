using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ehmini.Application.DTOs.Document;
using Ehmini.Application.Interfaces;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;

namespace Ehmini.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DocumentResponseDto> CreateDocumentAsync(Guid userId, CreateDocumentRequestDto dto)
    {
        var document = Document.Create(dto.DocumentType, dto.ProviderName, dto.ExternalReference, userId);

        foreach (var detailDto in dto.Details)
        {
            var detail = new DocumentDetail
            {
                Id = Guid.NewGuid(),
                ItemDescription = detailDto.ItemDescription,
                Quantity = detailDto.Quantity,
                UnitPrice = detailDto.UnitPrice,
                LineTotal = detailDto.Quantity * detailDto.UnitPrice
            };
            document.Details.Add(detail);
        }

        document.TotalAmount = document.Details.Sum(d => d.LineTotal);

        await _unitOfWork.Documents.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(document);
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByUserAsync(Guid userId)
    {
        var documents = await _unitOfWork.Documents.GetByUserIdAsync(userId);
        return documents.Select(MapToResponseDto);
    }

    private static DocumentResponseDto MapToResponseDto(Document document)
    {
        return new DocumentResponseDto(
            document.Id,
            document.DocumentType,
            document.TotalAmount,
            document.ProviderName,
            document.ExternalReference,
            document.CreatedAt,
            document.Details.Select(d => new DocumentDetailResponseDto(
                d.Id,
                d.ItemDescription,
                d.Quantity,
                d.UnitPrice,
                d.LineTotal
            )).ToList()
        );
    }
}
