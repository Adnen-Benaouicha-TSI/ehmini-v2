using Ehmini.Application.DTOs;
using Ehmini.Application.DTOs.Brouillon;
using Ehmini.Application.DTOs.Contracts;
using Ehmini.Application.DTOs.Document;
using Ehmini.Application.DTOs.QuotationList;
using Ehmini.Application.DTOs.Quotes;
using Ehmini.Application.Interfaces;
using Ehmini.Application.Interfaces.QuoteProvider;
using Ehmini.Core.Entities;
using Ehmini.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ehmini.Application.Services;

public class DocumentOrchestrationService : IDocumentOrchestrationService
{
    private readonly IQuoteProviderFactory _providerFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DocumentOrchestrationService(
        IQuoteProviderFactory providerFactory,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _providerFactory = providerFactory;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DocumentResponseDto> ProcessAndSaveQuoteAsync(qModel quoteRequest, Guid userIdClaim, CancellationToken cancellationToken)
    {


        var activeProvider = _providerFactory.GetActiveProvider();
        ProviderQuoteResponseDto providerResponse = await activeProvider.GenerateQuoteAsync(quoteRequest, cancellationToken);

        if (string.IsNullOrEmpty(providerResponse.Reference))
        {
            throw new InvalidOperationException("Le prestataire Phoenix n'a pas pu initialiser le devis.");
        }

        var documentEntity = Document.Create(
            documentType: "Devis",
            providerName: activeProvider.Provider.ToString(),
            externalReference: providerResponse.Reference,
            userId: userIdClaim
        );

        documentEntity.TotalAmount = providerResponse.TotalAmount;

        var detailResponses = new List<DocumentDetailResponseDto>();

        foreach (var line in providerResponse.Lines)
        {
            var detailId = Guid.NewGuid();
            var detailEntity = new DocumentDetail
            {
                Id = detailId,
                DocumentId = documentEntity.Id,
                ItemDescription = $"{line.Description}",
                Quantity = line.Quantite,
                UnitPrice = line.UnitPrice,
                LineTotal = line.UnitPrice * line.Quantite
            };

            documentEntity.Details.Add(detailEntity);

            detailResponses.Add(new DocumentDetailResponseDto(
                Id: detailId,
                ItemDescription: detailEntity.ItemDescription,
                Quantity: detailEntity.Quantity,
                UnitPrice: detailEntity.UnitPrice,
                LineTotal: detailEntity.LineTotal
            ));
        }

        await _unitOfWork.Documents.AddAsync(documentEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DocumentResponseDto(
            Id: documentEntity.Id,
            DocumentType: documentEntity.DocumentType,
            TotalAmount: documentEntity.TotalAmount,
            ProviderName: documentEntity.ProviderName,
            Reference: documentEntity.ExternalReference,
            CreatedAt: documentEntity.CreatedAt,
            Details: detailResponses
        );
    }
    public async Task<DocumentResponseDto> UpdateAndSaveQuoteAsync(Guid documentId, qModel quoteRequest, CancellationToken cancellationToken)
    {
        // 1. Validation de l'utilisateur connecté
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // 2. Récupération du document avec ses détails
        var existingDocument = await _unitOfWork.Documents.GetByIdAsync(documentId);
        if (existingDocument == null)
        {
            throw new KeyNotFoundException($"Le document avec l'ID {documentId} n'existe pas.");
        }

        // 3. Vérification des droits d'accès
        if (existingDocument.UserId != userId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier ce document.");
        }

        // 4. Appel de mise à jour du prestataire (Phoenix)
        var activeProvider = _providerFactory.GetActiveProvider();
        ProviderQuoteResponseDto providerResponse = await activeProvider.UpdateQuoteAsync(quoteRequest, cancellationToken);

        // 5. Mise à jour de l'en-tête du document local
        existingDocument.ProviderName = activeProvider.Provider.ToString();
        existingDocument.ExternalReference = providerResponse.Reference;
        existingDocument.TotalAmount = providerResponse.TotalAmount;

        if (existingDocument.Details == null)
        {
            existingDocument.Details = new List<DocumentDetail>();
        }

        // 6. Indexation des lignes actuelles par Description pour éviter de modifier l'Id
        var existingDetailsMap = existingDocument.Details.ToDictionary(d => d.ItemDescription);
        var updatedDetailIds = new List<Guid>();
        var detailResponses = new List<DocumentDetailResponseDto>();

        // 7. Traitement dynamique des lignes renvoyées par Phoenix
        foreach (var line in providerResponse.Lines)
        {
            DocumentDetail detailEntity;

            if (existingDetailsMap.TryGetValue(line.Description, out var existingDetail))
            {
                // ✅ MODIFICATION : La ligne existe déjà, on met à jour ses valeurs sans changer son Id
                detailEntity = existingDetail;
                detailEntity.Quantity = line.Quantite;
                detailEntity.UnitPrice = line.UnitPrice;
                detailEntity.LineTotal = line.UnitPrice * line.Quantite;

                updatedDetailIds.Add(detailEntity.Id);
            }
            else
            {
                // ➕ AJOUT : Nouvelle ligne de garantie ajoutée dans Phoenix
                var detailId = Guid.NewGuid();
                detailEntity = new DocumentDetail
                {
                    Id = detailId,
                    DocumentId = existingDocument.Id,
                    ItemDescription = line.Description,
                    Quantity = line.Quantite,
                    UnitPrice = line.UnitPrice,
                    LineTotal = line.UnitPrice * line.Quantite
                };

                existingDocument.Details.Add(detailEntity);
                updatedDetailIds.Add(detailId);
            }

            // Construction du DTO de réponse pour la ligne
            detailResponses.Add(new DocumentDetailResponseDto(
                Id: detailEntity.Id,
                ItemDescription: detailEntity.ItemDescription,
                Quantity: detailEntity.Quantity,
                UnitPrice: detailEntity.UnitPrice,
                LineTotal: detailEntity.LineTotal
            ));
        }

        // 8. 🗑️ SUPPRESSION : Retirer les garanties qui ne sont plus présentes dans le retour de Phoenix
        var detailsToRemove = existingDocument.Details
            .Where(d => !updatedDetailIds.Contains(d.Id))
            .ToList();

        foreach (var oldDetail in detailsToRemove)
        {
            existingDocument.Details.Remove(oldDetail);
        }

        // 9. Sauvegarde finale des modifications (Updates, Inserts et Deletes d'un coup)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 10. Retour du DTO synchronisé
        return new DocumentResponseDto(
            Id: existingDocument.Id,
            DocumentType: existingDocument.DocumentType,
            TotalAmount: existingDocument.TotalAmount,
            ProviderName: existingDocument.ProviderName,
            Reference: existingDocument.ExternalReference,
            CreatedAt: existingDocument.CreatedAt,
            Details: detailResponses
        );
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        var existingDocument = await _unitOfWork.Documents.GetByIdAsync(documentId);
        if (existingDocument == null)
        {
            throw new KeyNotFoundException($"Le document avec l'ID {documentId} n'existe pas.");
        }

        if (existingDocument.UserId != userId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à supprimer ce document.");
        }

        _unitOfWork.Documents.Delete(existingDocument);

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
    public async Task<List<ProviderQuotationDto>> GetQuotationsAsync(
       int language,
       CancellationToken cancellationToken)
    {
        return await _providerFactory
            .GetActiveProvider()
            .GetQuotationsAsync(language, cancellationToken);
    }
    public async Task<List<qModel>> GetContractsAsync(
    CancellationToken cancellationToken)
    {
        return await _providerFactory
            .GetActiveProvider()
            .GetContractsAsync(cancellationToken);
    }
    public async Task<List<BrouillonDto>> GetBrouillonsByUserAsync(
CancellationToken cancellationToken)
    {
        var activeProvider = _providerFactory.GetActiveProvider();

        return await activeProvider.GetBrouillonsByUserAsync(
            cancellationToken);
    }
}