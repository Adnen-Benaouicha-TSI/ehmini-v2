using Ehmini.Application.DTOs.Document;
using Ehmini.Application.DTOs.Quotes;
using Ehmini.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ehmini.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IDocumentOrchestrationService _orchestrationService;

    public DocumentsController(IDocumentService documentService, IDocumentOrchestrationService orchestrationService)
    {
        _documentService = documentService;
        _orchestrationService = orchestrationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequestDto request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var response = await _documentService.CreateDocumentAsync(userId, request);
        return CreatedAtAction(nameof(GetDocuments), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var documents = await _documentService.GetDocumentsByUserAsync(userId);
        return Ok(documents);
    }

    [HttpPost("quotes")]
    public async Task<IActionResult> CreateQuoteDocument([FromBody] qModel request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orchestrationService.ProcessAndSaveQuoteAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/quotes")]
    public async Task<IActionResult> UpdateQuoteDocument(Guid id, [FromBody] qModel request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orchestrationService.UpdateAndSaveQuoteAsync(id, request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var isDeleted = await _orchestrationService.DeleteDocumentAsync(id, cancellationToken);

            if (!isDeleted)
                return BadRequest("Impossible de supprimer le document.");

            return NoContent(); // Code 204 : Suppression réussie, pas de contenu à renvoyer
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
