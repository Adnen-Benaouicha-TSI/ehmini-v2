using System;
using System.Collections.Generic;

namespace Ehmini.Core.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ApplicationUser? User { get; set; }
    public ICollection<DocumentDetail> Details { get; set; } = new List<DocumentDetail>();

    public Document() { }

    public static Document Create(string documentType, string providerName, string externalReference, Guid userId)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            DocumentType = documentType,
            ProviderName = providerName,
            ExternalReference = externalReference,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 0
        };
    }
}
