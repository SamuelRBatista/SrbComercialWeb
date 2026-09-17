using System.Text.RegularExpressions;

namespace Domain.Entities.ValueObjects;

public class SeoInfo
{
    public string MetaTitle { get; private set; }
    public string MetaDescription { get; private set; }
    public string MetaKeywords { get; private set; }
    public string Slug { get; private set; }
    public string Tags { get; private set; }

    public SeoInfo(
        string name,
        string metaTitle = null,
        string metaDescription = null,
        string metaKeywords = null,
        string tags = null)
    {
        MetaTitle = metaTitle ?? name;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        Tags = tags;
        GenerateSlug(name);
    }

    public void Update(
        string name,
        string metaTitle = null,
        string metaDescription = null,
        string metaKeywords = null,
        string tags = null)
    {
        MetaTitle = metaTitle ?? name;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        Tags = tags;
        GenerateSlug(name);
    }

    public void GenerateSlug(string name, int? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        
        var slug = name.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = slug.Replace(" ", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        
        if (id.HasValue && id.Value > 0)
        {
            slug = $"{slug}-{id.Value}";
        }
        
        Slug = slug;
    }
}