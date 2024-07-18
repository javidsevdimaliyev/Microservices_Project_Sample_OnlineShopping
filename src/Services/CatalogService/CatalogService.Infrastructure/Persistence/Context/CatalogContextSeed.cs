using CatalogService.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System.Globalization;
using System.IO.Compression;


namespace CatalogService.Infrastructure.Persistence.Context;

public class CatalogContextSeed
{
    public async Task SeedAsync(CatalogDbContext context, IHostEnvironment env, ILogger<CatalogContextSeed> logger)
    {
        var policy = Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, $"Exception with message detected on attempt of {retry} within time interval {timeSpan} for {ctx}");
                });

        var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
        var picturePath = "Pics";

        await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath));
    }

    private async Task ProcessSeeding(CatalogDbContext context, string setupDirPath, string picturePath)
    {
        if (!context.CatalogBrands.Any())
        {
            await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }

        if (!context.CatalogTypes.Any())
        {
            await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }

        if (!context.CatalogItems.Any())
        {
            await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath, context));

            await context.SaveChangesAsync();

            GetCatalogItemPictures(setupDirPath, picturePath);
        }
    }

    private IEnumerable<CatalogBrandEntity> GetCatalogBrandsFromFile(string contentPath)
    {
        IEnumerable<CatalogBrandEntity> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrandEntity>()
            {
                new CatalogBrandEntity() { Brand = "Azure" },
                new CatalogBrandEntity() { Brand = ".NET" },
                new CatalogBrandEntity() { Brand = "Visual Studio" },
                new CatalogBrandEntity() { Brand = "SQL Server" },
                new CatalogBrandEntity() { Brand = "Other" }
            };
        }

        string fileName = Path.Combine(contentPath, "BrandsTextFile.txt");

        if (!File.Exists(fileName))
        {
            GetPreconfiguredCatalogBrands();
        }

        var fileContent = File.ReadAllLines(fileName);

        var brandList = fileContent.Select(b => new CatalogBrandEntity()
        {
            Brand = b.Trim('"')
        }).Where(b => b != null);

        return brandList ?? GetPreconfiguredCatalogBrands();
    }

    private IEnumerable<CatalogTypeEntity> GetCatalogTypesFromFile(string contentPath)
    {
        IEnumerable<CatalogTypeEntity> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogTypeEntity>()
            {
                new CatalogTypeEntity() { Type = "Mug" },
                new CatalogTypeEntity() { Type = "T-Shirt" },
                new CatalogTypeEntity() { Type = "Sheet" },
                new CatalogTypeEntity() { Type = "USB Memory Stick" },
            };
        }

        string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredCatalogTypes();
        }

        var fileContent = File.ReadAllLines(fileName);

        var typeList = fileContent.Select(ct => new CatalogTypeEntity()
        {
            Type = ct.Trim('"')
        }).Where(ct => ct != null);

        return typeList ?? GetPreconfiguredCatalogTypes();
    }

    private IEnumerable<CatalogItemEntity> GetCatalogItemsFromFile(string contentPath, CatalogDbContext context)
    {
        IEnumerable<CatalogItemEntity> GetPreconfiguredItems()
        {
            return new List<CatalogItemEntity>()
            {
                new CatalogItemEntity { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie, and more", Name = ".NET Bot Black Hoodie", Price = 19.5m, PictureFileName = "1.png", OnReorder = false },
                new CatalogItemEntity { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 89, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price = 8.5m, PictureFileName = "2.png", OnReorder = true },
                new CatalogItemEntity { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 55, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5m, PictureFileName = "5.png", OnReorder = false }
            };
        }

        string fileName = Path.Combine(contentPath, "CatalogItems.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredItems();
        }

        var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
        var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

        var fileContent = File.ReadAllLines(fileName)
            .Skip(1) // Skip the header row
            .Select(f => f.Split(','))
            .Select(f => new CatalogItemEntity()
            {
                CatalogTypeId = catalogTypeIdLookup[f[0]],
                CatalogBrandId = catalogBrandIdLookup[f[1]],
                Description = f[2].Trim('"').Trim(),
                Name = f[3].ToString().Trim('"').Trim(),
                Price = decimal.Parse(f[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                PictureFileName = f[5].Trim('"').Trim(),
                AvailableStock = string.IsNullOrWhiteSpace(f[6]) ? 0 : int.Parse(f[6]),
                OnReorder = Convert.ToBoolean(f[7])
            });

        return fileContent;
    }

    private void GetCatalogItemPictures(string contentPath, string picturePath)
    {
        picturePath ??= "pics";

        if (picturePath != null)
        {
            DirectoryInfo directory = new DirectoryInfo(picturePath);

            // To delete previous pictures
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
            ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
        }
    }
}
