using System;
using Ehmini.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ehmini.Infrastructure.Persistence.Configurations;

public class AppVersionConfiguration : IEntityTypeConfiguration<AppVersion>
{
    public void Configure(EntityTypeBuilder<AppVersion> builder)
    {
        builder.HasData(
            new AppVersion
            {
                Id = 1,
                Platform = "android",
                LatestVersion = "1.0.0",
                MinRequiredVersion = "1.0.0",
                StoreUrl = "https://play.google.com/store/apps/details?id=com.ctama.ehmini",
                ReleaseNotes = "Initial release",
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AppVersion
            {
                Id = 2,
                Platform = "ios",
                LatestVersion = "1.0.0",
                MinRequiredVersion = "1.0.0",
                StoreUrl = "https://apps.apple.com/app/ehmini/id000000000",
                ReleaseNotes = "Initial release",
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
