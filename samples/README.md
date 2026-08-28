# Samples — local development only

`Griesoft.OrchardCore.ContentReadTime.Sample` is a minimal Orchard Core 3.0.1 CMS host used to
verify the Content Read Time module against a real Orchard Core site. It is **not** part of the
published package and is intended for local development only.

Run it with:

```bash
dotnet run --project samples/Griesoft.OrchardCore.ContentReadTime.Sample
```

The site listens on **http://localhost:5103** and auto-provisions itself on first start using the
`Blog` recipe with a SQLite database (stored under the sample's `App_Data`, which is gitignored).
Dev-only admin credentials are `admin` / `SampleDev1!` (see `appsettings.json`).

To exercise the module: enable the **Content Read Time** feature (Configuration → Features), attach
the **ContentReadTimePart** to a content type such as `BlogPost` (Content → Content Definition),
pick the text source in the part settings, publish an item, and read
`ContentItem.Content.ContentReadTimePart.Minutes` from a template or the GraphQL API.

For example, with the **Templates** feature enabled, a site template named `Content__BlogPost`
(shape-alternate naming) can render
`{{ Model.ContentItem.Content.ContentReadTimePart.Minutes }}` on the blog post's display view, and
with **GraphQL** enabled the value is exposed as `blogPost { contentReadTime { minutes } }`.
