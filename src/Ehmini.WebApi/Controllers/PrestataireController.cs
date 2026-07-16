using Ehmini.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class PrestatairesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PrestatairesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prestataire>>> GetPrestataires()
    {
        return await _context.Prestataires.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Prestataire>> GetPrestataire(int id)
    {
        var prestataire = await _context.Prestataires.FindAsync(id);

        if (prestataire == null)
        {
            return NotFound();
        }

        return prestataire;
    }

    [HttpPost]
    public async Task<ActionResult<Prestataire>> PostPrestataire(Prestataire prestataire)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Prestataires.Add(prestataire);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPrestataire), new { id = prestataire.Id }, prestataire);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPrestataire(int id, Prestataire prestataire)
    {

        try
        {
            var p = await _context.Prestataires.FirstOrDefaultAsync(p => p.Id == id);
            p.title = prestataire.title;
            p.description = prestataire.description;
            p.status = prestataire.status;
            _context.Prestataires.Update(p);

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PrestataireExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // 204 - mis à jour avec succès
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrestataire(int id)
    {
        var prestataire = await _context.Prestataires.FindAsync(id);
        if (prestataire == null)
        {
            return NotFound();
        }

        _context.Prestataires.Remove(prestataire);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 - supprimé avec succès
    }

    private bool PrestataireExists(int id)
    {
        return _context.Prestataires.Any(e => e.Id == id);
    }
}