using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Data;

public static class CatalogSeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Products.Any()) return;

        // ── Categories ──
        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Headphones, speakers, cameras, and smart devices." },
            new Category { Name = "Home & Office", Description = "Lamps, organizers, and workspace essentials." },
            new Category { Name = "Accessories", Description = "Watches, bags, wallets, and everyday carry." },
            new Category { Name = "Lifestyle", Description = "Bottles, pillows, fitness gear, and wellness." },
            new Category { Name = "Smart Devices", Description = "Connected devices, trackers, and smart home." }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        // ── Products with 3 genuinely different images each ──
        // Image 1: Hero/front view
        // Image 2: Alternative angle or detail
        // Image 3: Lifestyle or context shot

        var products = new (Product product, string img2, string img3)[]
        {
            // ─── ELECTRONICS ───
            Make(categories[0], "Aurora Wireless Headphones",
                "Over-ear wireless headphones with active noise cancellation, 40mm custom drivers, and up to 30 hours of battery life. Features Bluetooth 5.3 with multipoint connectivity.",
                4299m, 12m, 18, "AUR-WH-01", "Aurora",
                "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1484704849700-f032a568e944?w=600&h=600&fit=crop"),

            Make(categories[0], "Nova Studio Monitor Headphones",
                "Professional open-back headphones with a flat frequency response for accurate mixing and mastering. Detachable cable with 6.35mm adapter included.",
                5499m, 0m, 12, "NOV-SM-02", "Nova",
                "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1487215078519-e21cc028cb29?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1558756520-22cfe5d382ca?w=600&h=600&fit=crop"),

            Make(categories[0], "Pulse Bluetooth Speaker",
                "Portable waterproof speaker with 360-degree sound, 20W output, and 18-hour battery. IP67 rated for pool and beach use.",
                2199m, 15m, 30, "PLS-BS-03", "Pulse",
                "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1589003077984-894e133dabab?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1558089687-f282ffcbc126?w=600&h=600&fit=crop"),

            Make(categories[0], "Prism 4K Action Camera",
                "Compact action camera with 4K 60fps recording, electronic image stabilization, and waterproof housing up to 30 meters.",
                8999m, 5m, 15, "PRM-AC-04", "Prism",
                "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f?w=600&h=600&fit=crop"),

            Make(categories[0], "Zenith Wireless Earbuds",
                "True wireless earbuds with hybrid ANC, transparency mode, and wireless charging case. 8 hours playback per charge.",
                2799m, 10m, 45, "ZEN-WE-05", "Zenith",
                "/images/products/zenith-earbuds/1.jpg",
                "/images/products/zenith-earbuds/2.jpg",
                "/images/products/zenith-earbuds/3.jpg"),

            Make(categories[0], "Echo Noise-Cancelling Earbuds",
                "Compact in-ear monitors with adaptive noise cancellation, touch controls, and IPX4 sweat resistance for workouts.",
                1899m, 0m, 38, "ECH-NC-06", "Echo",
                "https://images.unsplash.com/photo-1598331668826-20cecc596b86?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1631176093617-63490a3d785a?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1572569511254-d8f925fe2cbb?w=600&h=600&fit=crop"),

            // ─── HOME & OFFICE ───
            Make(categories[1], "Lumen Brass Desk Lamp",
                "Adjustable LED task lamp with three color temperatures, touch dimmer, and brushed-brass finish. USB-C charging port built into the base.",
                2599m, 15m, 9, "LUM-DL-07", "Lumen",
                "/images/products/lumen-desk-lamp/1.jpg",
                "/images/products/lumen-desk-lamp/2.jpg",
                "/images/products/lumen-desk-lamp/3.jpg"),

            Make(categories[1], "Hearth Reading Lamp",
                "Compact bedside lamp with warm 2700K LED, adjustable arm, and memory function. Ideal for evening reading.",
                1899m, 0m, 14, "HEA-RL-08", "Hearth",
                "https://images.unsplash.com/photo-1543198126-a8ad8e47fb22?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1494438639946-1ebd1d20bf85?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=600&h=600&fit=crop"),

            Make(categories[1], "Slate Desk Organizer",
                "Bamboo desktop organizer with compartments for pens, cards, phone, and small accessories. Felt-lined base.",
                699m, 0m, 25, "SLT-DO-09", "Slate",
                "https://images.unsplash.com/photo-1589584649628-b597067e07a3?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1593062096033-9a26b09da705?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1518455027359-f3f8164ba6bd?w=600&h=600&fit=crop"),

            Make(categories[1], "Monarch Ergonomic Mouse",
                "Vertical ergonomic mouse with 4000 DPI optical sensor, silent clicks, and rechargeable battery. Reduces wrist strain.",
                899m, 20m, 20, "MNR-EM-10", "Monarch",
                "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1605379399642-870262d3d051?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=600&h=600&fit=crop"),

            Make(categories[1], "Vault Document Safe",
                "Fireproof document safe with digital keypad, backup key, and interior LED light. Fits A4 documents and laptops up to 15 inches.",
                3499m, 0m, 8, "VLT-DS-11", "Vault",
                "/images/products/vault-safe/1.jpg",
                "/images/products/vault-safe/2.jpg",
                "/images/products/vault-safe/3.jpg"),

            // ─── ACCESSORIES ───
            Make(categories[2], "Chronos Minimalist Watch",
                "Swiss-movement quartz watch with sapphire crystal, genuine leather strap, and 42mm case. Water resistant to 50 meters.",
                6499m, 20m, 11, "CHR-MW-12", "Chronos",
                "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1524592094714-0f0654e20314?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?w=600&h=600&fit=crop"),

            Make(categories[2], "Nomad Canvas Backpack",
                "Waxed canvas backpack with laptop compartment, YKK zippers, and padded shoulder straps. 25L capacity.",
                3299m, 10m, 16, "NMD-CB-13", "Nomad",
                "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1581605405669-fcdf81165afa?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1622547748225-3fc4abd2cca0?w=600&h=600&fit=crop"),

            Make(categories[2], "Ridgeline Leather Wallet",
                "Full-grain leather bifold wallet with RFID blocking, 8 card slots, and slim profile. Hand-stitched edges.",
                1299m, 0m, 30, "RDG-LW-14", "Ridgeline",
                "https://images.unsplash.com/photo-1627123424574-724758594e93?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1606503153255-59d8b2e4b0e4?w=600&h=600&fit=crop"),

            Make(categories[2], "Apex Sunglasses",
                "Polarized UV400 sunglasses with acetate frames and scratch-resistant lenses. Includes hardshell case and microfiber cloth.",
                1599m, 25m, 22, "APX-SS-15", "Apex",
                "https://images.unsplash.com/photo-1572635196237-14b3f281503f?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1511499767150-a48a237f0083?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1577803645773-f96470509666?w=600&h=600&fit=crop"),

            Make(categories[2], "Forge Key Organizer",
                "Titanium key organizer with quick-release mechanism, pocket clip, and integrated bottle opener. Holds up to 8 keys.",
                549m, 0m, 40, "FRG-KO-16", "Forge",
                "https://images.unsplash.com/photo-1622547748225-3fc4abd2cca0?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1582131503261-fca1d1c0589f?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1558089687-f282ffcbc126?w=600&h=600&fit=crop"),

            // ─── LIFESTYLE ───
            Make(categories[3], "Summit Insulated Bottle",
                "Double-wall vacuum-insulated stainless steel bottle. Keeps drinks cold for 24 hours and hot for 12. BPA-free.",
                899m, 10m, 42, "SUM-IB-17", "Summit",
                "https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1570831739435-6601aa3fa4fb?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1523362628745-0c100fc988a6?w=600&h=600&fit=crop"),

            Make(categories[3], "Nimbus Cloud Pillow",
                "Memory foam pillow with cooling gel layer and bamboo-derived cover. Medium firmness for side and back sleepers.",
                1299m, 0m, 28, "NMB-CP-18", "Nimbus",
                "/images/products/nimbus-pillow/1.jpg",
                "/images/products/nimbus-pillow/2.jpg",
                "/images/products/nimbus-pillow/3.jpg"),

            Make(categories[3], "Stride Yoga Mat",
                "Non-slip TPE yoga mat with alignment markings, 6mm thickness, and carrying strap. Eco-friendly materials.",
                799m, 15m, 35, "STR-YM-19", "Stride",
                "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=600&h=600&fit=crop"),

            Make(categories[3], "Terra Plant Pot Set",
                "Set of three ceramic plant pots with drainage holes and bamboo trays. Matte finish in neutral tones.",
                1099m, 0m, 20, "TRR-PP-20", "Terra",
                "https://images.unsplash.com/photo-1485955900006-10f4d324d411?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1459411552884-841db9b3cc2a?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1509423350716-97f9360b4e09?w=600&h=600&fit=crop"),

            Make(categories[3], "Bloom Aromatherapy Diffuser",
                "Ultrasonic essential oil diffuser with 300ml capacity, 7 LED colors, and auto shut-off timer. Runs up to 10 hours.",
                1499m, 0m, 18, "BLM-AD-21", "Bloom",
                "https://images.unsplash.com/photo-1602928321679-560bb453f190?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1545566241-0d2996c7e3e0?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?w=600&h=600&fit=crop"),

            // ─── SMART DEVICES ───
            Make(categories[4], "Orbit Smart Tracker",
                "Bluetooth item tracker with 12-month replaceable battery, loud ring, and crowd-find network. Works with iOS and Android.",
                699m, 0m, 50, "ORB-ST-22", "Orbit",
                "/images/products/orbit-tracker/1.jpg",
                "/images/products/orbit-tracker/2.jpg",
                "/images/products/orbit-tracker/3.jpg"),

            Make(categories[4], "Halo Smart LED Bulb",
                "Wi-Fi smart bulb with 16 million colors, voice control compatibility, and scheduling. No hub required.",
                399m, 0m, 60, "HAL-SB-23", "Halo",
                "/images/products/halo-led-bulb/1.jpg",
                "/images/products/halo-led-bulb/2.jpg",
                "/images/products/halo-led-bulb/3.jpg"),

            Make(categories[4], "Sentinel Smart Plug",
                "Wi-Fi enabled smart plug with energy monitoring, timer function, and voice control. Works with Alexa and Google Home.",
                349m, 0m, 45, "SNT-SP-24", "Sentinel",
                "/images/products/sentinel-plug/1.jpg",
                "/images/products/sentinel-plug/2.jpg",
                "/images/products/sentinel-plug/3.jpg"),

            Make(categories[4], "Nexus Smart Scale",
                "Bluetooth body composition scale measuring weight, BMI, muscle mass, and body fat. Syncs with health apps.",
                1199m, 10m, 22, "NXS-SS-25", "Nexus",
                "https://images.unsplash.com/photo-1576243345690-4e4b79b63288?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=600&h=600&fit=crop",
                "https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=600&h=600&fit=crop")
        };

        foreach (var (product, img2, img3) in products)
        {
            context.Products.Add(product);
            context.SaveChanges();

            context.ProductImages.AddRange(
                new ProductImage { ProductId = product.Id, Url = product.ImageUrl, AltText = product.Name + " — front view", SortOrder = 0 },
                new ProductImage { ProductId = product.Id, Url = img2, AltText = product.Name + " — detail view", SortOrder = 1 },
                new ProductImage { ProductId = product.Id, Url = img3, AltText = product.Name + " — lifestyle", SortOrder = 2 }
            );
        }
        context.SaveChanges();
    }

    private static (Product product, string img2, string img3) Make(
        Category category, string name, string description, decimal price, decimal discount,
        int quantity, string sku, string brand, string img1, string img2, string img3)
    {
        return (new Product
        {
            Name = name,
            Description = description,
            Price = price,
            DiscountPercent = discount,
            Quantity = quantity,
            CategoryId = category.Id,
            SKU = sku,
            Brand = brand,
            ImageUrl = img1
        }, img2, img3);
    }
}
