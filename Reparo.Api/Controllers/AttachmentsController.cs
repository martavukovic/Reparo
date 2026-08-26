using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;
using Reparo.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AttachmentsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet("fault-report/{faultReportId}")]
    public async Task<ActionResult<List<AttachmentDto>>> GetByFaultReport(int faultReportId)
    {
        var attachments = await _context.Attachments
            .Where(a => a.FaultReportId == faultReportId)
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                FaultReportId = a.FaultReportId,
                InterventionId = a.InterventionId,
                Purpose = a.Purpose,
                OriginalFileName = a.OriginalFileName,
                ContentType = a.ContentType,
                UploadedAt = a.UploadedAt
            })
            .ToListAsync();

        return Ok(attachments);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AttachmentDto>> Upload(
        [FromForm] int faultReportId,
        [FromForm] int? interventionId,
        [FromForm] string purpose,
        IFormFile file)
    {
        // Provjera vrste datoteke
        var allowedTypes = new[]
        {
            "image/jpeg", "image/png", "image/webp", "application/pdf"
        };

        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Dopuštene vrste: JPEG, PNG, WEBP, PDF.");

        // Provjera veličine (max 10 MB)
        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("Datoteka ne smije biti veća od 10 MB.");

        // Provjera namjene
        var allowedPurposes = new[]
        {
            "Fotografija prije rada",
            "Fotografija nakon rada",
            "Dokument"
        };

        if (!allowedPurposes.Contains(purpose))
            return BadRequest("Neispravna namjena privitka.");

        // Spremi datoteku
        var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, storedFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        var attachment = new Attachment
        {
            FaultReportId = faultReportId,
            InterventionId = interventionId,
            Purpose = purpose,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = userId
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();

        return Ok(new AttachmentDto
        {
            Id = attachment.Id,
            FaultReportId = attachment.FaultReportId,
            InterventionId = attachment.InterventionId,
            Purpose = attachment.Purpose,
            OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType,
            UploadedAt = attachment.UploadedAt
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var attachment = await _context.Attachments.FindAsync(id);

        if (attachment is null)
            return NotFound();

        // Obriši fizičku datoteku
        var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        var filePath = Path.Combine(uploadsFolder, attachment.StoredFileName);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}